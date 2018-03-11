#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class MeshCombiner
    {
        #region Private Classes
        private class MeshCombineOperationInfo
        {
            public Material CommonMaterial;
            public List<Mesh> CommonMaterialMeshes = new List<Mesh>();
            public List<int> CommonMaterialSubmeshIndices = new List<int>();
            public List<Matrix4x4> VertexTransformMatrices = new List<Matrix4x4>();
            public List<bool> ReverseWindingOrderFlags = new List<bool>();
        }

        private class ObjectMeshPair
        {
            public GameObject GameObject;
            public Mesh ObjectMesh;
        }
        #endregion

        #region Private Variables
        [SerializeField]
        private MeshCombineSettings _meshCombineSettings;

        [SerializeField]
        private MeshCombinerView _view;
        #endregion

        #region Public Properties
        public MeshCombineSettings MeshCombineSettings
        {
            get
            {
                if (_meshCombineSettings == null) _meshCombineSettings = Octave3DWorldBuilder.Instance.CreateScriptableObject<MeshCombineSettings>();
                return _meshCombineSettings;
            }
        }
        public MeshCombinerView View { get { return _view; } }
        #endregion

        #region Constructors
        public MeshCombiner()
        {
            _view = new MeshCombinerView(this);
        }
        #endregion

        #region Public Methods
        public void CombineRoots(List<GameObject> roots, string nameOfNewParent)
        {
            // First create the new parent objects which will hold the combined meshes
            if (string.IsNullOrEmpty(nameOfNewParent)) nameOfNewParent = "Parent Object (Combined Meshes)";
            GameObject newParentObject = new GameObject(nameOfNewParent);

            // The first step is to clone the hierarchy roots and attach them to the parent which we have just created
            CloneRootsAndAttachToParent(roots, newParentObject);

            // Now, we will combine the objects in the parent
            CombineObjectsInParent(newParentObject);
        }
        #endregion

        #region Private Methods
        private void CloneRootsAndAttachToParent(List<GameObject> roots, GameObject destinationParent)
        {
            Transform destinationParentTransform = destinationParent.transform;
            foreach (var root in roots)
            {
                GameObject clone = root.Clone();
                clone.transform.parent = destinationParentTransform;

                if (MeshCombineSettings.HideWireframe) clone.SetSelectedHierarchyWireframeHidden(true);
            }
        }

        private void CombineObjectsInParent(GameObject parentObject)
        {
            // First, associate each material with a list of object mesh pairs
            Dictionary<Material, List<ObjectMeshPair>> materialToObjectMeshPairs = MapMaterialsToObjectMeshPairs(parentObject);

            // Now use this dictionary to create a list of mesh combine operation info instances. We will need
            // these instances to start the mesh combine process.
            List<MeshCombineOperationInfo> meshCombineOpInfoInstances = CreateAllMeshCombineOperationInfoInstances(materialToObjectMeshPairs);

            // Start the mesh combine process
            CombineMeshes(meshCombineOpInfoInstances, parentObject);

            // The original objects whose meshes participated in the mesh combine process need to be destroyed
            DestroyCombinableObjects(materialToObjectMeshPairs);

            // Remove any empty hierarchies if necessary
            if (MeshCombineSettings.RemoveEmptyHierarchies) RemoveEmptyHierarchies(parentObject);
        }

        private void RemoveEmptyHierarchies(GameObject parentObject)
        {
            List<GameObject> immediateChildren = parentObject.GetImmediateChildren();
            for(int rootIndex = 0; rootIndex < immediateChildren.Count; ++rootIndex)
            {
                EditorUtility.DisplayProgressBar("Post processing", "Removing empty hierarchies. Please wait...", (float)rootIndex / immediateChildren.Count);

                GameObject root = immediateChildren[rootIndex];
                if (ObjectQueries.IsGameObjectHierarchyEmpty(root)) GameObject.DestroyImmediate(root);
            }
            EditorUtility.ClearProgressBar();
        }

        private Dictionary<Material, List<ObjectMeshPair>> MapMaterialsToObjectMeshPairs(GameObject parentObject)
        {
            var materialToObjectMeshPairs = new Dictionary<Material, List<ObjectMeshPair>>(parentObject.transform.childCount * 2);

            // Acquire all mesh renderers
            MeshRenderer[] allMeshRenderersInChildObjects = parentObject.GetComponentsInChildren<MeshRenderer>(true);
            foreach (MeshRenderer meshRenderer in allMeshRenderersInChildObjects)
            {
                // Are we allowed to combine this object?
                if (CanMeshObjectBeCombined(meshRenderer.gameObject))
                {
                    // Create a new object-mesh pair
                    var objectMeshPair = new ObjectMeshPair();
                    objectMeshPair.ObjectMesh = meshRenderer.gameObject.GetMeshFromMeshFilter();
                    objectMeshPair.GameObject = meshRenderer.gameObject;

                    // Associate each renderer material with the object mesh pair that we have just created
                    foreach (Material sharedMaterial in meshRenderer.sharedMaterials)
                    {
                        if (sharedMaterial == null) continue;

                        // Add a new entry for this material if one doesn't already exist
                        if (!materialToObjectMeshPairs.ContainsKey(sharedMaterial))
                            materialToObjectMeshPairs.Add(sharedMaterial, new List<ObjectMeshPair>());

                        // Associate the material with the object-mesh pair
                        materialToObjectMeshPairs[sharedMaterial].Add(objectMeshPair);
                    }
                }
            }

            return materialToObjectMeshPairs;
        }

        private void DestroyCombinableObjects(Dictionary<Material, List<ObjectMeshPair>> materialToObjectMeshPairs)
        {
            foreach(var pair in materialToObjectMeshPairs)
            {
                List<ObjectMeshPair> objectMeshPairs = pair.Value;
                foreach(var objectMeshPair in objectMeshPairs)
                {
                    GameObject gameObject = objectMeshPair.GameObject;
                    if (gameObject == null) continue;

                    gameObject.MoveImmediateChildrenUpOneLevel(false);
                    GameObject.DestroyImmediate(gameObject);
                }
            }
        }

        private bool CanMeshObjectBeCombined(GameObject meshObject)
        {
            if (MeshCombineSettings.IgnoreInactiveGameObjects && !meshObject.activeSelf) return false;

            // Note: We will only combine mesh objects that have a valid mesh filter with a mesh associated with it.
            //       Skinned meshes and terrains are ignored.
            if (!meshObject.HasMeshFilterWithValidMesh()) return false;
            if (meshObject.HasSkinnedMeshRendererWithValidMesh()) return false;
            if (meshObject.HasTerrain()) return false;

            // If we require only static meshes, but this is not a static object, ignore it
            if (MeshCombineSettings.CombineOnlyStaticMeshes && !meshObject.isStatic) return false;

            return true;
        }

        private List<MeshCombineOperationInfo> CreateAllMeshCombineOperationInfoInstances(Dictionary<Material, List<ObjectMeshPair>> materialToObjectMeshPairs)
        {
            var meshCombineOpInfoInstances = new List<MeshCombineOperationInfo>();
            foreach (KeyValuePair<Material, List<ObjectMeshPair>> pair in materialToObjectMeshPairs)
            {
                // Create a new mesh combine operation info instance and add it to the list
                var meshCombineOpInfo = new MeshCombineOperationInfo();
                meshCombineOpInfoInstances.Add(meshCombineOpInfo);

                // Now popluate the mesh combine operation info instance. First, specify the common material.
                meshCombineOpInfo.CommonMaterial = pair.Key;

                // Now, loop through all object mesh pairs mapped to the current material and populate the rest of the data
                foreach (ObjectMeshPair objectMeshPair in pair.Value)
                {
                    meshCombineOpInfo.CommonMaterialMeshes.Add(objectMeshPair.ObjectMesh);

                    Transform gameObjectTransform = objectMeshPair.GameObject.transform;
                    Matrix4x4 transformMatrix = gameObjectTransform.localToWorldMatrix;
                    meshCombineOpInfo.VertexTransformMatrices.Add(transformMatrix);

                    // We must also check if the chosen transform contains any negative scale. If it does, we will have to reverse the vertex winding order.
                    Vector3 globalScale = gameObjectTransform.lossyScale;
                    if (globalScale.x < 0.0f || globalScale.y < 0.0f || globalScale.z < 0.0f) meshCombineOpInfo.ReverseWindingOrderFlags.Add(true);
                    else meshCombineOpInfo.ReverseWindingOrderFlags.Add(false);

                    // Populate the submesh indices list. In order to do this, we will have to loop through all the materials which are associated with the 
                    // mesh renderer and find the material which is the same as the material that is associated with the dictionary key we are currently 
                    // processing. The material's index is the index of the submesh we are interested in.
                    Material[] sharedMaterials = objectMeshPair.GameObject.GetComponent<MeshRenderer>().sharedMaterials;
                    for (int sharedMaterialIndex = 0; sharedMaterialIndex < sharedMaterials.Length; ++sharedMaterialIndex)
                    {
                        // If we found the correct index, store it and break from the loop
                        if (sharedMaterials[sharedMaterialIndex] == pair.Key)
                        {
                            meshCombineOpInfo.CommonMaterialSubmeshIndices.Add(sharedMaterialIndex);
                            break;
                        }
                    }
                }
            }

            return meshCombineOpInfoInstances;
        }

        private void CombineMeshes(List<MeshCombineOperationInfo> meshCombineOpInfoInstances, GameObject parentObject)
        {
            if (meshCombineOpInfoInstances.Count == 0) return;

            int numberOfMaterialsToProcess = meshCombineOpInfoInstances.Count;
            int numberOfProcessedMaterials = 0;
            
            // We will need a list with all objects which contain combined meshes
            List<GameObject> allObjectsWithCombinedMeshes = new List<GameObject>(meshCombineOpInfoInstances.Count);

            // Loop through each mesh combine operation info instance
            Transform parentObjectTransform = parentObject.transform;
            foreach (MeshCombineOperationInfo meshCombineInfo in meshCombineOpInfoInstances)
            {
                // Update the progress bar
                ++numberOfProcessedMaterials;
                EditorUtility.DisplayProgressBar("Optimizing", "Combining meshes. Please wait...", (float)numberOfProcessedMaterials / numberOfMaterialsToProcess);

                // Combine the meshes in the current mesh combine operation info instance and retrieve the game objects which contain the combined meshes
                List<GameObject> objectsWithCombinedMeshes = CombineMeshes(meshCombineInfo, parentObject);
                allObjectsWithCombinedMeshes.AddRange(objectsWithCombinedMeshes);

                // Loop through all game objects which were returned to us and make them a child of the parent object. We will
                // also take this chance and apply any necessary settings which relate to the combined objects.
                foreach (GameObject objectWithCombinedMesh in objectsWithCombinedMeshes)
                {
                    objectWithCombinedMesh.transform.parent = parentObjectTransform;
                    if(MeshCombineSettings.HideWireframe) objectWithCombinedMesh.SetSelectedHierarchyWireframeHidden(true);
                    objectWithCombinedMesh.name = MeshCombineSettings.NameOfParentObjects;

                    // Attach a mesh collider component to the game object if necessary
                    if (MeshCombineSettings.AttachMeshCollidersToCombinedMeshes)
                    {
                        MeshCollider meshCollider = objectWithCombinedMesh.AddComponent<MeshCollider>();
                        meshCollider.convex = MeshCombineSettings.UseConvexMeshColliders;
                    }
                }
            }
            EditorUtility.ClearProgressBar(); 
          
            // Weld the vertices between all combined meshes if necesary
            if(MeshCombineSettings.WeldVertexPositions && !MeshCombineSettings.WeldVertexPositionsOnlyForCommonMaterial)
            {
                WeldVertexPositionsForMeshObjects(allObjectsWithCombinedMeshes);
            }
        }

        private List<GameObject> CombineMeshes(MeshCombineOperationInfo meshCombineOpInfo, GameObject parentObject)
        {
            var objectsWithCombinedMeshes = new List<GameObject>();

            // Create the first game object which will hold the first combined mesh. In most situations, this will be
            // the only object which exists in the list of objects returned to the user. When the mesh vertex limit is
            // reached, an additional object will need to be created.
            var objectWithCombinedMesh = new GameObject();
            objectsWithCombinedMeshes.Add(objectWithCombinedMesh);

            // The user has the possibility to choose whether or not combined mesh objects are static or dynamic. So we will
            // have to mark the object as static or dynamic based on what the user desires.
            objectWithCombinedMesh.isStatic = MeshCombineSettings.MakeCombinedMeshesStatic;

            // Attach a new mesh to the object. This is the mesh that will contain the combined result.
            MeshFilter meshFilter = objectWithCombinedMesh.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = new Mesh();
            meshFilter.sharedMesh.name = MeshCombineSettings.NameOfCombinedMeshes;

            // Store data for easy access
            Mesh combinedMesh = meshFilter.sharedMesh;
            List<Mesh> meshesWithCommonMaterial = meshCombineOpInfo.CommonMaterialMeshes;
            List<int> submeshIndices = meshCombineOpInfo.CommonMaterialSubmeshIndices;

            // We will need to handle the mesh vertex limit. Unity uses a maximum of 65000 vertices per mesh. However, as we will see later, we 
            // need to use the 'Unwrapping.GenerateSecondaryUVSet' for each combined mesh and this function can split vertices, which means that
            // the mesh's vertex count may double. In this case, a vertex limit of 65000 would not work because if we had a mesh with that many
            // vertices and the UV generation function would split all vertices (in the worst case scenario), we would end up with a mesh which
            // has 130000 vertices. This would cause the function to throw an exception. So, we will set the limit to 30000. If a mesh had this 
            // number of vertices and all of them would be split (worst case scenario), we would end up with 60000 vertices which is still valid.
            const int maxNumberOfVerticesPerMesh = 30000;

            // Normally, when we combine meshes for one material, only one combined mesh will be created. However, if the vertex
            // limit is reached, more than one combined mesh may be created. This list holds all combined meshes which were created 
            // for a single material. This is useful when vertex welding must be applied because it will allow us to weld vertices
            // between all meshes which were created.
            var combinedMeshesForSameMaterial = new List<Mesh>();
            combinedMeshesForSameMaterial.Add(combinedMesh);

            // Create the 2 lists which will hold the vertices and indices of the combined mesh
            var combinedVertices = new List<Vector3>();
            var combinedIndices = new List<int>();

            // We will also need a couple of lists which will contain all the vertex attributes a vertex can have.
            // Each element inside each of these lists maps to an element inside 'combinedVertices'. So there is
            // one vertex attribute in these lists for each vertex that will reside inside the combined mesh.
            var combinedColors = new List<Color32>();
            var combinedNormals = new List<Vector3>();
            var combinedTangents = new List<Vector4>();
            var combinedUV = new List<Vector2>();
            var combinedUV1 = new List<Vector2>();

            // When we populate the combined index and vertex lists, we will avoid storing the same vertex twice inside the 
            // vertex list. For example, if we encounter the following sequence of indices: 0, 1, 2, 1, 0, 3, we know that
            // we are dealing with only 4 vertices referenced by the indices 0, 1, 2, and 3. We will use a dictionary to check
            // if an index was already stored inside the combined index list. If it was, it means the vertex which it references
            // was also stored in the combined vertex list. In that case we will not add another vertex. We will only add the
            // index to the index list.
            // This dictionary also serves another purpose. When we process a new index, we will not store its value directly
            // inside the index list because that index is relative to the beginning of the source mesh's vertex buffer. We
            // want to map that index to a value which is relative to the beginning of our combined vertex list. So, when a
            // new index is encountered, we will map it to the correct value, store the mapped value inside the combined index
            // list and then create an entry inside the dictionary which maps the source vertex index to the mapped vertex index.
            // Later, if we encounter the same vertex index again, we will use the dictionary to retrieve the index value to which
            // it was mapped and store that in the combined indices list.
            var originalVertexIndexToNewVertexIndex = new Dictionary<int, int>();

            // As we add vertices to the combined vertex list, we will increment this value so that we know what should be the
            // vertex index of the next vertex inside the list.
            int currentVertexIndex = 0;

            // Some of the meshes which will need to be combined will be mapped to transform matrices which contain negative scale.
            // In that case, the vertex indices will need to be adjusted such that the vertex winding order is changed. The following
            // 2 variables will help us accomplish this task.
            bool reverseVertexWindingOrder = false;
            int offsetInCombinedIndicesList = -1;

            // Now it is time to loop through all meshes which reside inside the mesh combine operation info instance and combine them
            for (int meshIndex = 0; meshIndex < meshesWithCommonMaterial.Count; ++meshIndex)
            {
                Mesh meshToCombine = meshesWithCommonMaterial[meshIndex];
                reverseVertexWindingOrder = meshCombineOpInfo.ReverseWindingOrderFlags[meshIndex];

                // In case we need to reverse the vertex winding order, we need to store the index of the first vertex index which will be added to the 
                // combined indices list. The reversal process will need to start from this index because if we do need to flip the vertex winding order,
                // we will only want to flip the winding order of the vertices which belong to the current mesh that we are processing.
                offsetInCombinedIndicesList = combinedIndices.Count;

                // Retrieve the mesh vertices and indices. In order to retrieve the indices we will use the 'GetIndices' method which will
                // return the indices of a specified submesh. We will specify the index of the submesh which is given to us by the mesh
                // combine operation info instance and which is mapped to the current mesh that we are processing.
                Vector3[] meshVertices = meshToCombine.vertices;
                int[] submeshVertexIndices = meshToCombine.GetIndices(submeshIndices[meshIndex]);

                // Store all possible vertex attributes. We will use these to populate the combined mesh with data.
                Color32[] vertexColors = meshToCombine.colors32;
                Vector3[] vertexNormals = meshToCombine.normals;
                Vector4[] vertexTangents = meshToCombine.tangents;
                Vector2[] vertexUV = meshToCombine.uv;
                Vector2[] vertexUV1 = meshToCombine.uv2;

                // We will need two matrices. One matrix will be used to transform the vertices of the mesh and we will set it to the transform
                // matrix which sits inside the mesh combine operation info instance and which is mapped to the mesh we are currently processing.
                Matrix4x4 vertexTransformMatrix = meshCombineOpInfo.VertexTransformMatrices[meshIndex];

                // The second matrix that we need is an inverse transpose matrix of the vertex transform matrix. The mesh's vertices may have normals associated with them. 
                // When we transform the vertices, we also have to transform the normals. The problem is that the vertex transform matrix may also contain scale information. 
                // A normal vector is only used for the purposes of orientation. So scale must not affect it in any way. Only rotation must be applied to a normal vector. 
                // If we calculate the inverse of the vertex transform matrix we will get a matrix which cancels all the transformations applied to the vertex, including scale 
                // and rotation. If the vertex transform matrix containes a rotation of say 45 degrees around the Z axis, when we calculate its inverse, the inverse will transpose
                // the upper 3X3 portion of the matrix thus swithcing the sign of the rotation to -45 degrees. This is correct because that is what an inverse matrix should do. 
                // It should cancel (i.e. subtract) any transformation which exist inside the original matrix. The problem is that we need the original rotation information in 
                // order to transform our vertex normals. We can do that by transposing the inverse matrix which has the effect of switching the sign of the rotation such that the
                // rotation is restored to its original value of 45 degrees.
                Matrix4x4 inverseTranspose = vertexTransformMatrix.inverse.transpose;

                // Whenever we process a new mesh, we have to reset the dictionary becuase we are dealing with a new set of vertex and index data
                originalVertexIndexToNewVertexIndex.Clear();

                // Loop through all vertex indices which were retrieved from the submesh and use them to populate the combined vertex and index lists
                for (int submeshVertexIndex = 0; submeshVertexIndex < submeshVertexIndices.Length; ++submeshVertexIndex)
                {
                    // Store the vertex index for easy access. This is the vertex index as it is stored in the source mesh index buffer.
                    int vertexIndex = submeshVertexIndices[submeshVertexIndex];

                    // If this vertex index hasn't been encountered before, it means that it references a vertex which hasn't been added to the combined
                    // vertex list. In that case we want to add both the vertex and index to the combined lists.
                    if (!originalVertexIndexToNewVertexIndex.ContainsKey(vertexIndex))
                    {
                        // Whenever we have to add a new vertex, we first have to make sure that we haven't reached the limit of the number of vertices
                        // which can be stored inside a mesh. It may appear reasonable to believe that we could just check if the current number of 
                        // vertices which reside inside the combined vertex list has reached the maximum. However, we can't do that. Whenever we reach the
                        // maximum, we will have to commit the current data to the combined mesh. The problem is that if the number of combined indices
                        // is not a multiple of 3, we will get an exception. This is because the indices describe the triangles in the mesh and we always
                        // need 3 vertices to form a triangle. If the number of combined indices is not a multiple of 3, it means we have an incomplete
                        // triangle in our combined index list. In order to solve this issue, we will first check if the current number of combined indices
                        // is a multiple of 3. If it is, we will also check if the number of combined vertices is greater or equal to the maximum number of
                        // vertices per mesh - 2. If that is true, it means we can commit the data for the current combined mesh and start creating a new one.
                        // Note: By subtracting 2 from the maximum number of vertices per mesh we make sure we catch the case in which the current number of
                        //       combined indices divided by 3 yileds a remainder of 1 (or 2) and the number of vertices stored is 64998. In this case as soon,
                        //       as the number of combined indices will reach a value which is a multiple of 3, we can commit the combined mesh data. We don't
                        //       have to worry about the number of vertices exceeding the maximum. This is why we subtract 2 from the maximum number of vertices.
                        //       We want to make sure that in case more indices need to be added, there is room for at least 2 more vertices. If only one index 
                        //       needs to be added, there will only be one vertex added.
                        if (combinedIndices.Count % 3 == 0 && combinedVertices.Count >= maxNumberOfVerticesPerMesh - 2)
                        {
                            // Reverse the vertex winding order if necessary
                            if (reverseVertexWindingOrder)
                            {
                                // Calculate the number of triangles which are involved in the vetex winding order and reverse
                                int numberOfTrianglesToReverse = (combinedIndices.Count - offsetInCombinedIndicesList) / 3;
                                ReverseVertexWindingOrder(combinedIndices, offsetInCombinedIndicesList, numberOfTrianglesToReverse);
                            }

                            // We need to reset this because the combined indices list will be cleared
                            offsetInCombinedIndicesList = 0;

                            // Commit the combined mesh data to the combined mesh
                            combinedMesh.vertices = combinedVertices.ToArray();
                            combinedMesh.SetTriangles(combinedIndices.ToArray(), 0);
                            combinedMesh.colors32 = combinedColors.ToArray();
                            combinedMesh.normals = combinedNormals.ToArray();
                            combinedMesh.tangents = combinedTangents.ToArray();
                            combinedMesh.uv = combinedUV.ToArray();
                            combinedMesh.uv2 = combinedUV1.ToArray();

                            // Before we can upload the mesh data to the GPU, we have to make sure that we generate a secondary set of UVs. This
                            // will make sure that lightmapping will work correctly with our combined meshes.
                            Unwrapping.GenerateSecondaryUVSet(combinedMesh);
                            combinedMesh.UploadMeshData(true);

                            // Save the combined mesh as asset
                            SaveCombinedMeshAsAsset(combinedMesh);

                            // We will need a new game object to hold the new mesh
                            objectWithCombinedMesh = new GameObject();
                            objectsWithCombinedMeshes.Add(objectWithCombinedMesh);

                            // The user has the possibility to choose whether or not combined mesh objects are static or dynamic. So we will
                            // have to mark the object as static or dynamic based on what the user desires.
                            objectWithCombinedMesh.isStatic = MeshCombineSettings.MakeCombinedMeshesStatic;

                            // Create the new combined mesh
                            meshFilter = objectWithCombinedMesh.AddComponent<MeshFilter>();
                            meshFilter.sharedMesh = new Mesh();
                            meshFilter.sharedMesh.name = MeshCombineSettings.NameOfCombinedMeshes;

                            // Store the combined mesh for easy access
                            combinedMesh = meshFilter.sharedMesh;
                            combinedMeshesForSameMaterial.Add(combinedMesh);

                            // Clear the combined data
                            combinedVertices.Clear();
                            combinedIndices.Clear();
                            combinedColors.Clear();
                            combinedNormals.Clear();
                            combinedTangents.Clear();
                            combinedUV.Clear();
                            combinedUV1.Clear();

                            // Clear the dictionary and reset the current vertex index because we are now dealing with an empty combined vertex list
                            currentVertexIndex = 0;
                            originalVertexIndexToNewVertexIndex.Clear();
                        }

                        // When we reach this point, we can forget about what happened before. Whether a new mesh was created or not, we know that when we are dealing 
                        // with a vertex index which was not encountered before, we must add a new vertex to the combined vertex list. So, we will retrieve the vertex 
                        // position from the mesh vertex list, transform it using the vertex transform matrix associated with the mesh and then add the transformed vertex
                        // to the combined vertices list.
                        Vector3 vertexPosition = meshVertices[vertexIndex];
                        vertexPosition = vertexTransformMatrix.MultiplyPoint(vertexPosition);
                        combinedVertices.Add(vertexPosition);

                        // We will also add a new index inside the combined index list
                        combinedIndices.Add(currentVertexIndex);

                        // Note: This step is important. The next time we encounter the same vertex index, we don't want to add another vertex for it. So, we will map the
                        //       vertex index which was retrieved from the mesh to the vertex index which we just stored in the combined indices list. When we encounter the
                        //       same vertex index, we won't add a new vertex to the combined vertices list. We will just add the index value which maps to the vertex index
                        //       we encounter.
                        originalVertexIndexToNewVertexIndex.Add(vertexIndex, currentVertexIndex);

                        // Move to the next vertex in the combined vertex list
                        ++currentVertexIndex;

                        // Copy the corresponding vertex attribute data.
                        // Note: When we retrieve the vertex attribute data from the source mesh, we use the vertex index which was retrieved from the source mesh'es 
                        //       index list. This is because each element inside a vertex attribute array maps to a vertex inside the source mesh's vertex list.
                        if (vertexColors.Length != 0) combinedColors.Add(vertexColors[vertexIndex]);
                        if (vertexUV.Length != 0) combinedUV.Add(vertexUV[vertexIndex]);
                        if (vertexUV1.Length != 0) combinedUV1.Add(vertexUV1[vertexIndex]);

                        // Copy the vertex normal
                        if (vertexNormals.Length != 0)
                        {
                            // Note: Multiply by the inverse transpose, make sure the normal is normalized and then add it to the combined normals list
                            Vector3 vertexNormal = inverseTranspose.MultiplyVector(vertexNormals[vertexIndex]);
                            vertexNormal.Normalize();
                            combinedNormals.Add(vertexNormal);
                        }

                        // Copy the vertex tangent
                        if (vertexTangents.Length != 0)
                        {
                            // Note: We can't just add the tangent to the combined tangent list right away. The tangent is a unit length vector which extends along
                            //       the surface of a mesh. But we have to remember that every vertex inside the mesh is transformed using a transform matrix. When
                            //       we transform the vertices, we also transform the surfaces (i.e. triangles) that they form. In that case, if we were to just leave
                            //       the tangent to its original untransformed value, it would be incompatible with the new orientation of the surface to which it is
                            //       associated. So, we need to also tranform our tangent vectors using the same matrix that we used to transform the vertex normals.
                            //       We use the same matrix because, just as it was the case with normals, we don't want to have any scale applied to our tangents. Only
                            //       the rotation information is important.
                            // Note: In Unity, a tangent vector is a unit length vector which is stored inside a 'Vector4' instance. The fourth element holds a value which can be
                            //       either 1 or -1, and is used by Unity to calculate the binormal. Changing the sign of this value, has the effect of changing the handedness 
                            //       of the tangent space coordinate system. We want to keep the same handedness as the one which is stored in the original mesh. So, in order to 
                            //       transform the tangent, we will construct a separate 'Vector3' instance which contains the coordinates of the original tangent vector, transform
                            //       it using the inverse transpose matrix, normalize it, and then use it to construct the final 'Vector4' instance which contains the transformed 
                            //       tangent and the original handedness value in the fourth element.
                            Vector3 transformedTangent = new Vector3(vertexTangents[vertexIndex].x, vertexTangents[vertexIndex].y, vertexTangents[vertexIndex].z);
                            transformedTangent = inverseTranspose.MultiplyVector(transformedTangent);
                            transformedTangent.Normalize();

                            // Add the transformed tangent to the combined tanget list, but keep the original handedness value in the fourth element
                            combinedTangents.Add(new Vector4(transformedTangent.x, transformedTangent.y, transformedTangent.z, vertexTangents[vertexIndex].w));
                        }
                    }
                    else
                    {
                        // If the vertex index already exists, we will not add a new vertex. We will just add the vertex index to the combined indices list.
                        // Note: The index that we add to the combined indices list is not the vertex index that we retrieved from the mesh's index buffer. It is
                        //       the index which was calculated when we first encountered 'vertexIndex'.
                        combinedIndices.Add(originalVertexIndexToNewVertexIndex[vertexIndex]);
                    }
                }

                // Reverse the vertex winding order if necessary
                if (reverseVertexWindingOrder)
                {
                    // Calculate the number of triangles which are involved in the vetex winding order and reverse
                    int numberOfTrianglesToReverse = (combinedIndices.Count - offsetInCombinedIndicesList) / 3;
                    ReverseVertexWindingOrder(combinedIndices, offsetInCombinedIndicesList, numberOfTrianglesToReverse);
                }
            }

            // Commit the combined mesh data to the combined mesh
            combinedMesh.vertices = combinedVertices.ToArray();
            combinedMesh.SetTriangles(combinedIndices.ToArray(), 0);
            combinedMesh.colors32 = combinedColors.ToArray();
            combinedMesh.normals = combinedNormals.ToArray();
            combinedMesh.tangents = combinedTangents.ToArray();
            combinedMesh.uv = combinedUV.ToArray();
            combinedMesh.uv2 = combinedUV1.ToArray();

            // Weld vertex positions?
            if (MeshCombineSettings.WeldVertexPositions && MeshCombineSettings.WeldVertexPositionsOnlyForCommonMaterial)
            {
                // Note: We need to weld vertices for all combined meshes for the current material. Usually, there
                //       will only be one mesh, but there could be more if the vertex limit was reached.
                WeldVertexPositionsForMeshes(combinedMeshesForSameMaterial);
            }

            // Before we can upload the mesh data to the GPU, we have to make sure that we generate a secondary set of UVs. This
            // will make sure that lightmapping will work correctly with our combined meshes.
            Unwrapping.GenerateSecondaryUVSet(combinedMesh);
            combinedMesh.UploadMeshData(true);

            // Save the combined mesh as asset
            SaveCombinedMeshAsAsset(combinedMesh);

            // The final step is to loop through all the game objects which hold our combined meshes and attach a renderer to each of them.
            // The renderer has to use the same material as the one which is stored inside the mesh combine operation info instance. This is
            // the material which is used by all the meshes which were combined.
            foreach (GameObject gameObject in objectsWithCombinedMeshes)
            {
                Renderer objectRenderer = gameObject.AddComponent<MeshRenderer>();
                objectRenderer.sharedMaterial = meshCombineOpInfo.CommonMaterial;
            }

            // Return the object list to the caller
            return objectsWithCombinedMeshes;
        }

        private static void SaveCombinedMeshAsAsset(Mesh combinedMesh)
        {
            // Builds the name of the destination folder
            string absoluteFolderPath = FileSystem.GetToolFolderName() + "/Octave3D Combined Meshes";

            // If the folder doesn't exist, we will first have to create it
            if (!Directory.Exists(absoluteFolderPath)) Directory.CreateDirectory(absoluteFolderPath);

            // Now create an asset inside the combined mesh folder
            AssetDatabase.CreateAsset(combinedMesh, absoluteFolderPath + "/" + combinedMesh.name + "_" + combinedMesh.GetHashCode() + ".asset");
            AssetDatabase.SaveAssets();
        }

        private static void ReverseVertexWindingOrder(List<int> triangleIndices, int firstVertexIndex, int numberOfTriangles)
        {
            int vertexIndex = firstVertexIndex;
            for (int triangleIndex = 0; triangleIndex < numberOfTriangles; ++triangleIndex)
            {
                // In order to reverse the winding order, we can swap the last 2 indices which form the current triangleS
                int tempIndex = triangleIndices[vertexIndex + 1];
                triangleIndices[vertexIndex + 1] = triangleIndices[vertexIndex + 2];
                triangleIndices[vertexIndex + 2] = tempIndex;

                // Move to the next triangle
                vertexIndex += 3;
            }
        }

        private void WeldVertexPositionsForMeshObjects(List<GameObject> meshObjects)
        {
            if(meshObjects.Count == 0) return;

            var allMeshes = new List<Mesh>(meshObjects.Count);
            foreach(var gameObject in meshObjects)
            {
                Mesh mesh = gameObject.GetMeshFromMeshFilter();
                if (mesh != null) allMeshes.Add(mesh);
            }

            WeldVertexPositionsForMeshes(allMeshes);
        }

        private void WeldVertexPositionsForMeshes(List<Mesh> meshes)
        {
            var meshVertices = new List<Vector3>(meshes.Count * 100);
            for (int meshIndex = 0; meshIndex < meshes.Count; ++meshIndex)
            {
                Mesh mesh = meshes[meshIndex];
                meshVertices.AddRange(mesh.vertices);
            }
            WeldVertexPositions(meshVertices);

            int vertexOffset = 0;
            for (int meshIndex = 0; meshIndex < meshes.Count; ++meshIndex)
            {
                Mesh mesh = meshes[meshIndex];
                mesh.vertices = (meshVertices.GetRange(vertexOffset, mesh.vertexCount)).ToArray();

                vertexOffset += mesh.vertexCount;
            }
        }

        private void WeldVertexPositions(List<Vector3> vertexPositions)
        {
            VertexWeldOctree vertexWeldOctree = new VertexWeldOctree(1.0f);
            vertexWeldOctree.Build(vertexPositions);

            vertexPositions.Clear();
            vertexPositions.AddRange(vertexWeldOctree.WeldVertices(MeshCombineSettings.VertexPositionWeldEpsilon));           
        }
        #endregion
    }
}
#endif