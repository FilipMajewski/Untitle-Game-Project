#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    public static class GameObjectExtensions
    {
        #region Extension Methods
        public static bool IsSceneObject(this GameObject gameObject)
        {
            PrefabType prefabType = PrefabUtility.GetPrefabType(gameObject);
            return prefabType == PrefabType.None || prefabType == PrefabType.PrefabInstance ||
                   prefabType == PrefabType.DisconnectedPrefabInstance || prefabType == PrefabType.MissingPrefabInstance;
        }

        public static List<Vector3> GetLocalAxes(this GameObject gameObject)
        {
            Transform gameObjectTransform = gameObject.transform;
            return new List<Vector3> { gameObjectTransform.right, gameObjectTransform.up, gameObjectTransform.forward };
        }

        public static void ApplyTransformDataToRootChildren(this GameObject root, GameObject sourceRoot)
        {
            List<GameObject> destChildren = root.GetAllChildren();
            List<GameObject> sourceChildren = sourceRoot.GetAllChildren();

            if (destChildren.Count != sourceChildren.Count) return;

            // Note: Assumes there will always be a 1 to 1 mapping between the children in the hierarchy. 
            for(int childIndex = 0; childIndex < destChildren.Count; ++childIndex)
            {
                destChildren[childIndex].transform.InheritWorldTransformFrom(sourceChildren[childIndex].transform);
            }
        }

        public static GameObject Clone(this GameObject gameObject, bool allowUndoRedo = true)
        {
            Transform gameObjectTransform = gameObject.transform;
            GameObject clone = GameObject.Instantiate(gameObject, gameObjectTransform.position, gameObjectTransform.rotation) as GameObject;
            if (allowUndoRedo) UndoEx.RegisterCreatedGameObject(clone);

            clone.name = gameObject.name;
            clone.isStatic = gameObject.isStatic;
            clone.layer = gameObject.layer;

            SceneViewCamera.Instance.SetObjectVisibilityDirty();

            return clone;
        }

        public static GameObject GetSourcePrefab(this GameObject gameObject)
        {
            return PrefabUtility.GetPrefabParent(gameObject) as GameObject;
        }

        public static GameObject GetSourcePrefabRoot(this GameObject gameObject)
        {
            GameObject sourcePrefab = gameObject.GetSourcePrefab();
            if (sourcePrefab == null) return null;

            Transform sourcePrefabTransform = sourcePrefab.transform;
            if (sourcePrefabTransform.root != null) sourcePrefab = sourcePrefabTransform.root.gameObject;

            return sourcePrefab;
        }

        public static bool RaycastBox(this GameObject gameObject, Ray ray, out GameObjectRayHit objectRayHit)
        {
            objectRayHit = null;
            OrientedBox objectWorldOrientedBox = gameObject.GetWorldOrientedBox();
        
            OrientedBoxRayHit objectBoxRayHit;
            if (objectWorldOrientedBox.Raycast(ray, out objectBoxRayHit))
                objectRayHit = new GameObjectRayHit(ray, gameObject, objectBoxRayHit, null, null, null);

            return objectRayHit != null;
        }

        public static bool RaycastSprite(this GameObject gameObject, Ray ray, out GameObjectRayHit objectRayHit)
        {
            objectRayHit = null;

            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null) return false;

            OrientedBox objectWorldOrientedBox = gameObject.GetWorldOrientedBox();

            OrientedBoxRayHit objectBoxRayHit;
            if(objectWorldOrientedBox.Raycast(ray, out objectBoxRayHit))
            {
                SpriteRayHit spriteHit = new SpriteRayHit(ray, objectBoxRayHit.HitEnter, spriteRenderer, objectBoxRayHit.HitPoint, objectBoxRayHit.HitNormal);
                objectRayHit = new GameObjectRayHit(ray, gameObject, null, null, null, spriteHit);
            }

            return objectRayHit != null;
        }

        public static bool RaycastMesh(this GameObject gameObject, Ray ray, out GameObjectRayHit objectRayHit)
        {
            objectRayHit = null;
            Mesh objectMesh = gameObject.GetMeshFromFilterOrSkinnedMeshRenderer();
            if (objectMesh == null) return false;

            Octave3DMesh octaveMesh = Octave3DMeshDatabase.Get().GetOctave3DMesh(objectMesh);
            if (octaveMesh == null) return false;

            MeshRayHit meshRayHit = octaveMesh.Raycast(ray, gameObject.transform.GetWorldMatrix());
            if (meshRayHit == null) return false;

            objectRayHit = new GameObjectRayHit(ray, gameObject, null, meshRayHit, null, null);
            return true;
        }

        public static bool RaycastTerrain(this GameObject gameObject, Ray ray, out GameObjectRayHit objectRayHit)
        {
            objectRayHit = null;
            if (!gameObject.HasTerrain()) return false;

            TerrainCollider terrainCollider = gameObject.GetComponent<TerrainCollider>();
            if (terrainCollider == null) return false;

            RaycastHit raycastHit;
            if (terrainCollider.Raycast(ray, out raycastHit, float.MaxValue))
            {
                TerrainRayHit terrainRayHit = new TerrainRayHit(ray, raycastHit);
                objectRayHit = new GameObjectRayHit(ray, gameObject, null, null, terrainRayHit, null);
            }

            return objectRayHit != null;
        }

        public static void SetHierarchyStatic(this GameObject hierarchyRoot, bool isStatic)
        {
            List<GameObject> allChildren = hierarchyRoot.GetAllChildrenIncludingSelf();
            foreach(GameObject child in allChildren)
            {
                child.isStatic = isStatic;
            }
        }

        public static void RotateHierarchyBoxAroundPoint(this GameObject root, float rotationInDegrees, Vector3 rotationAxis, Vector3 pivotPoint)
        {
            OrientedBox hierarchyWorldOrientedBox = root.GetHierarchyWorldOrientedBox();
            Transform rootTransform = root.transform;

            Vector3 fromCenterToPosition = rootTransform.position - hierarchyWorldOrientedBox.Center;
            Quaternion oldRotation = rootTransform.rotation;

            rotationAxis.Normalize();
            rootTransform.Rotate(rotationAxis, rotationInDegrees, Space.World);

            fromCenterToPosition = oldRotation.GetRelativeRotation(rootTransform.rotation) * fromCenterToPosition;
            rootTransform.position = hierarchyWorldOrientedBox.Center + fromCenterToPosition;
        }

        public static void SetHierarchyWorldRotationAndPreserveHierarchyCenter(this GameObject root, Quaternion rotation)
        {
            OrientedBox hierarchyWorldOrientedBox = root.GetHierarchyWorldOrientedBox();
            Transform rootTransform = root.transform;

            Vector3 fromCenterToPosition = rootTransform.position - hierarchyWorldOrientedBox.Center;
            Quaternion oldRotation = rootTransform.rotation;
            rootTransform.rotation = rotation;

            fromCenterToPosition = oldRotation.GetRelativeRotation(rootTransform.rotation) * fromCenterToPosition;
            rootTransform.position = hierarchyWorldOrientedBox.Center + fromCenterToPosition;
        }

        public static void SetHierarchyWorldScaleByPivotPoint(this GameObject root, float scale, Vector3 pivotPoint)
        {
            root.SetHierarchyWorldScaleByPivotPoint(new Vector3(scale, scale, scale), pivotPoint);
        }

        public static void SetHierarchyWorldScaleByPivotPoint(this GameObject root, Vector3 scale, Vector3 pivotPoint)
        {
            Transform rootTransform = root.transform;
            Vector3 fromPivotToPosition = rootTransform.position - pivotPoint;
            Vector3 oldScale = rootTransform.lossyScale;
            root.SetWorldScale(scale);

            Vector3 invOldScaleVector = new Vector3(1.0f / oldScale.x, 1.0f / oldScale.y, 1.0f / oldScale.z);
            Vector3 relativeScale = Vector3.Scale(scale, invOldScaleVector);
            fromPivotToPosition = Vector3.Scale(relativeScale, fromPivotToPosition);
            rootTransform.position = pivotPoint + fromPivotToPosition;
        }

        public static void PlaceHierarchyOnPlane(this GameObject root, Plane placementPlane)
        {
            OrientedBox hierarchyWorldOrientedBox = root.GetHierarchyWorldOrientedBox();

            Vector3 fromBoxCenterToRootPos = root.transform.position - hierarchyWorldOrientedBox.Center;
            Vector3 projectedBoxCenter = placementPlane.ProjectPoint(hierarchyWorldOrientedBox.Center);
            Vector3 newBoxCenter = projectedBoxCenter + 0.5f * placementPlane.normal * hierarchyWorldOrientedBox.GetSizeAlongDirection(placementPlane.normal);

            root.transform.position = newBoxCenter + fromBoxCenterToRootPos;
        }

        public static void SetSelectedHierarchyWireframeHidden(this GameObject hierarchyRoot, bool isWireframeHidden)
        {
            Renderer renderer = hierarchyRoot.GetComponent<Renderer>();
            if (renderer != null) EditorUtility.SetSelectedWireframeHidden(renderer, isWireframeHidden);

            Transform gameObjectTransform = hierarchyRoot.transform;
            for (int childIndex = 0; childIndex < gameObjectTransform.childCount; ++childIndex)
            {
                SetSelectedHierarchyWireframeHidden(gameObjectTransform.GetChild(childIndex).gameObject, isWireframeHidden);
            }
        }

        public static bool DoesHierarchyContainMesh(this GameObject root)
        {
            List<GameObject> allChildrenIncludingSelf = root.GetAllChildrenIncludingSelf();
            foreach(GameObject gameObject in allChildrenIncludingSelf)
            {
                if (gameObject.HasMesh()) return true;
            }

            return false;
        }

        public static List<GameObject> GetHierarchyObjectsWithMesh(this GameObject hierarchyRoot)
        {
            List<GameObject> allChildrenIncludingSelf = hierarchyRoot.GetAllChildrenIncludingSelf();
            allChildrenIncludingSelf.RemoveAll(item => !item.HasMesh());

            return allChildrenIncludingSelf;
        }

        public static List<GameObject> GetHierarchyObjectsWithSprites(this GameObject hierarchyRoot)
        {
            List<GameObject> allChildrenIncludingSelf = hierarchyRoot.GetAllChildrenIncludingSelf();
            allChildrenIncludingSelf.RemoveAll(item => !item.HasSpriteRendererWithSprite());

            return allChildrenIncludingSelf;
        }

        public static bool HasMesh(this GameObject gameObject)
        {
            return gameObject.HasMeshFilterWithValidMesh() || gameObject.HasSkinnedMeshRendererWithValidMesh();
        }

        public static bool HasMeshFilterWithValidMesh(this GameObject gameObject)
        {
            MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
            return meshFilter != null && meshFilter.sharedMesh != null;
        }

        public static bool HasSkinnedMeshRendererWithValidMesh(this GameObject gameObject)
        {
            SkinnedMeshRenderer skinnedMeshRenderer = gameObject.GetComponent<SkinnedMeshRenderer>();
            return skinnedMeshRenderer != null && skinnedMeshRenderer.sharedMesh != null;
        }

        public static bool HasTerrain(this GameObject gameObject)
        {
            return gameObject.GetComponent<Terrain>() != null;
        }

        public static bool HasLight(this GameObject gameObject)
        {
            return gameObject.GetComponent<Light>() != null;
        }

        public static bool HasParticleSystem(this GameObject gameObject)
        {
            return gameObject.GetComponent<ParticleSystem>() != null;
        }

        public static bool HasSpriteRenderer(this GameObject gameObject)
        {
            return gameObject.GetComponent<SpriteRenderer>() != null;
        }

        public static bool HasSpriteRendererWithSprite(this GameObject gameObject)
        {
            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null) return false;

            return spriteRenderer.sprite != null;
        }

        public static void AttachChildren(this GameObject gameObject, List<GameObject> children, bool allowUndoRedo)
        {
            if(allowUndoRedo)
            {
                Transform objectTransform = gameObject.transform;
                UndoEx.RecordForToolAction(objectTransform);

                foreach (GameObject child in children)
                {
                    Transform childTransform = child.transform;
                    UndoEx.RecordForToolAction(childTransform);
                    childTransform.parent = objectTransform;
                }
            }
            else
            {
                Transform objectTransform = gameObject.transform;
                foreach (GameObject child in children)
                {
                    child.transform.parent = objectTransform;
                }
            }
        }

        public static List<GameObject> GetAllChildrenIncludingSelf(this GameObject gameObject)
        {
            var finalObjectList = new List<GameObject> { gameObject };

            List<GameObject> allChildren = gameObject.GetAllChildren();
            if (allChildren.Count != 0) finalObjectList.AddRange(allChildren);

            return finalObjectList;
        }

        public static List<GameObject> GetAllChildren(this GameObject gameObject)
        {
            Transform objectTransform = gameObject.transform;
            Transform[] allChildTransforms = gameObject.GetComponentsInChildren<Transform>(true);

            var allChildren = new List<GameObject>();
            foreach(Transform childTransform in allChildTransforms)
            {
                if (objectTransform != childTransform) allChildren.Add(childTransform.gameObject);
            }

            return allChildren;
        }

        public static List<GameObject> GetImmediateChildren(this GameObject gameObject)
        {
            Transform objectTransform = gameObject.transform;
            List<Transform> immediateChildTransforms = objectTransform.GetImmediateChildTransforms();

            if (immediateChildTransforms.Count != 0)
            {
                List<GameObject> immediateChildren = new List<GameObject>(immediateChildTransforms.Count);
                foreach (Transform childTransform in immediateChildTransforms)
                {
                    immediateChildren.Add(childTransform.gameObject);
                }

                return immediateChildren;
            }
            else return new List<GameObject>();
        }

        public static void MoveImmediateChildrenUpOneLevel(this GameObject gameObject, bool allowUndoRedo)
        {
            Transform objectTransform = gameObject.transform;
            Transform objectParentTransform = objectTransform.parent;

            List<Transform> immediateChildTransforms = objectTransform.GetImmediateChildTransforms();

            if(allowUndoRedo)
            {
                foreach (Transform childTransform in immediateChildTransforms)
                {
                    UndoEx.SetTransformParent(childTransform, objectParentTransform);
                }
            }
            else
            {
                foreach (Transform childTransform in immediateChildTransforms)
                {
                    childTransform.parent = objectParentTransform;
                }
            }
        }

        public static GameObject GetParentWhichIsChildOf(this GameObject gameObject, GameObject targetParent)
        {
            // Store needed data for easy access
            Transform targetParentTransform = targetParent.transform;
            Transform currentTransform = gameObject.transform;

            // Keep moving up the hierarchy until we encounter the parent whose parent is 'targetParent'
            while (currentTransform != null && currentTransform.parent != targetParentTransform)
            {
                // Move up
                currentTransform = currentTransform.parent;
            }

            // If the current transform is not null, it means we found the parent which is a child of 'targetParent'.
            // Otherwise, it means that either 'gameObject' doesn't have any parents or its top parent is not a child
            // of 'targetParent'.
            return currentTransform != null ? currentTransform.gameObject : null;
        }

        public static GameObject GetParentWhichIsChildOf(this GameObject gameObject, List<Type> possibleParentTypes)
        {
            Transform currentTransform = gameObject.transform;

            while (currentTransform != null && currentTransform.parent != null)
            {
                GameObject currentParentObject = currentTransform.parent.gameObject;
                if (currentParentObject.HasComponentsOfAnyType(possibleParentTypes)) return currentTransform.gameObject;

                currentTransform = currentTransform.parent;
            }

            return null;
        }

        public static GameObject GetParentWhichIsChildOfParentByPredicate(this GameObject gameObject, Predicate<GameObject> predicate)
        {
            Transform currentTransform = gameObject.transform;
            while (currentTransform != null && currentTransform.parent != null)
            {
                GameObject currentParentObject = currentTransform.parent.gameObject;
                if (predicate.Invoke(currentParentObject)) return currentTransform.gameObject;

                currentTransform = currentTransform.parent;
            }

            return null;
        }

        public static bool HasComponentsOfAnyType(this GameObject gameObject, List<Type> possibleTypes)
        {
            foreach (var type in possibleTypes)
            {
                if (gameObject.GetComponents(type).Length != 0) return true;
            }

            return false;
        }

        public static void SetWorldScale(this GameObject gameObject, float worldScale)
        {
            gameObject.SetWorldScale(new Vector3(worldScale, worldScale, worldScale));
        }

        public static void SetWorldScale(this GameObject gameObject, Vector3 worldScale)
        {
            Transform objectTransform = gameObject.transform;
            Transform objectParent = objectTransform.parent;

            objectTransform.parent = null;
            objectTransform.localScale = worldScale;

            float minScale = 1e-4f;
            if (Mathf.Abs(objectTransform.localScale.x) < minScale) objectTransform.localScale = new Vector3(minScale, objectTransform.localScale.y, objectTransform.localScale.z);
            if (Mathf.Abs(objectTransform.localScale.y) < minScale) objectTransform.localScale = new Vector3(objectTransform.localScale.x, minScale, objectTransform.localScale.z);
            if (Mathf.Abs(objectTransform.localScale.z) < minScale) objectTransform.localScale = new Vector3(objectTransform.localScale.x, objectTransform.localScale.y, minScale);

            objectTransform.parent = objectParent;
        }

        public static Rect GetScreenRectangle(this GameObject gameObject, Camera camera)
        {
            Box worldBox = gameObject.GetWorldBox();
            if (worldBox.IsValid()) return worldBox.GetScreenRectangle(camera);

            return new Rect();
        }

        public static OrientedBox GetHierarchyWorldOrientedBox(this GameObject hierarchyRoot)
        {
            OrientedBox hierarchyWorldOrientedBox = hierarchyRoot.GetHierarchyModelSpaceOrientedBox();           
            hierarchyWorldOrientedBox.Transform(hierarchyRoot.transform);

            return hierarchyWorldOrientedBox;
        }

        public static OrientedBox GetHierarchyModelSpaceOrientedBox(this GameObject hierarchyRoot)
        {
            Box hierarchyModelSpaceBox = hierarchyRoot.GetHierarchyModelSpaceBox();
            return new OrientedBox(hierarchyModelSpaceBox);
        }

        public static Box GetHierarchyWorldBox(this GameObject hierarchyRoot)
        {
            Box hierarchyWorldBox = hierarchyRoot.GetHierarchyModelSpaceBox();
            hierarchyWorldBox = hierarchyWorldBox.Transform(hierarchyRoot.transform.GetWorldMatrix());
            return hierarchyWorldBox;
        }

        public static Box GetHierarchyModelSpaceBox(this GameObject hierarchyRoot)
        {
            Transform rootTransform = hierarchyRoot.transform;
            Transform[] allChildTransforms = hierarchyRoot.GetComponentsInChildren<Transform>();

            bool hierarchyContainsMesh = hierarchyRoot.DoesHierarchyContainMesh();
            Box hierarchyModelSpaceBox = hierarchyRoot.GetModelSpaceBox();
            foreach (Transform childTransform in allChildTransforms)
            {
                GameObject childObject = childTransform.gameObject;

                // If the hierarchy contains mesh objects, we will only take mesh objects into account
                if (!childObject.HasMesh() && hierarchyContainsMesh) continue;
                if (childObject != hierarchyRoot)
                {
                    Box childModelSpaceBox = childObject.GetModelSpaceBox();
                    if (childModelSpaceBox.IsValid())
                    {
                        // Note: Negative scale values are a pain to work with, so we will set the scale to a positive value.
                        //       Negative scale causes problems inside 'Box.Transform' because it modifies the translation
                        //       in an undesirable manner. However, is it a good idea to ignore negative scale ???!!!
                        TransformMatrix rootRelativeTransformMatrix = childTransform.GetRelativeTransformMatrix(rootTransform);
                        rootRelativeTransformMatrix.Scale = rootRelativeTransformMatrix.Scale.GetVectorWithPositiveComponents();

                        childModelSpaceBox = childModelSpaceBox.Transform(rootRelativeTransformMatrix);
                   
                        if (hierarchyModelSpaceBox.IsValid()) hierarchyModelSpaceBox.Encapsulate(childModelSpaceBox);
                        else hierarchyModelSpaceBox = new Box(childModelSpaceBox);
                    }
                }
            }

            return hierarchyModelSpaceBox;
        }

        public static OrientedBox GetWorldOrientedBox(this GameObject gameObject)
        {
            OrientedBox worldOrientedBox = gameObject.GetMeshWorldOrientedBox();
            if (worldOrientedBox.IsValid()) return worldOrientedBox;

            return gameObject.GetNonMeshWorldOrientedBox();
        }

        public static Box GetWorldBox(this GameObject gameObject)
        {
            Box worldBox = gameObject.GetMeshWorldBox();
            if (worldBox.IsValid()) return worldBox;

            return gameObject.GetNonMeshWorldBox();
        }

        public static OrientedBox GetModelSpaceOrientedBox(this GameObject gameObject)
        {
            OrientedBox modelSpaceOrientedBox = gameObject.GetMeshModelSpaceOrientedBox();
            if (modelSpaceOrientedBox.IsValid()) return modelSpaceOrientedBox;

            return gameObject.GetNonMeshModelSpaceOrientedBox();
        }

        public static Box GetModelSpaceBox(this GameObject gameObject)
        {
            Box modelSpaceBox = gameObject.GetMeshModelSpaceBox();
            if (modelSpaceBox.IsValid()) return modelSpaceBox;

            return gameObject.GetNonMeshModelSpaceBox();
        }

        public static OrientedBox GetMeshWorldOrientedBox(this GameObject gameObject)
        {
            Mesh mesh = gameObject.GetMeshFromMeshFilter();
            if (mesh != null) return new OrientedBox(new Box(mesh.bounds), gameObject.transform);

            mesh = gameObject.GetMeshFromSkinnedMeshRenderer();
            if (mesh != null) return new OrientedBox(new Box(gameObject.GetComponent<SkinnedMeshRenderer>().localBounds), gameObject.transform);

            return OrientedBox.GetInvalid();
        }

        public static Box GetMeshWorldBox(this GameObject gameObject)
        {
            Mesh mesh = gameObject.GetMeshFromMeshFilter();
            if (mesh != null) return new Box(mesh.bounds).Transform(gameObject.transform.GetWorldMatrix());

            mesh = gameObject.GetMeshFromSkinnedMeshRenderer();
            if (mesh != null) return new Box(gameObject.GetComponent<SkinnedMeshRenderer>().localBounds).Transform(gameObject.transform.GetWorldMatrix());

            return Box.GetInvalid();
        }

        public static OrientedBox GetNonMeshWorldOrientedBox(this GameObject gameObject)
        {
            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            if(spriteRenderer != null)
            {
                return new OrientedBox(spriteRenderer.GetModelSpaceBox(), gameObject.transform);
            }
            else
            {
                OrientedBox modelSpaceOrientedBox = gameObject.GetNonMeshModelSpaceOrientedBox();
                if (!modelSpaceOrientedBox.IsValid()) return modelSpaceOrientedBox;

                OrientedBox worldOrientedBox = new OrientedBox(modelSpaceOrientedBox);
                Transform objectTransform = gameObject.transform;
                worldOrientedBox.Center = objectTransform.position;
                worldOrientedBox.Rotation = objectTransform.rotation;
                worldOrientedBox.Scale = objectTransform.lossyScale;

                return worldOrientedBox;
            }
        }

        public static Box GetNonMeshWorldBox(this GameObject gameObject)
        {
            Box modelSpaceBox = gameObject.GetNonMeshModelSpaceBox();
            if (!modelSpaceBox.IsValid()) return modelSpaceBox;

            Box worldBox = modelSpaceBox.Transform(gameObject.transform.GetWorldMatrix());
            return worldBox;
        }

        public static OrientedBox GetMeshModelSpaceOrientedBox(this GameObject gameObject)
        {
            Mesh mesh = gameObject.GetMeshFromMeshFilter();
            if (mesh != null) return new OrientedBox(new Box(mesh.bounds), Quaternion.identity);

            mesh = gameObject.GetMeshFromSkinnedMeshRenderer();
            if (mesh != null) return new OrientedBox(new Box(gameObject.GetComponent<SkinnedMeshRenderer>().localBounds), Quaternion.identity);
          
            return OrientedBox.GetInvalid();
        }

        public static Box GetMeshModelSpaceBox(this GameObject gameObject)
        {
            Mesh mesh = gameObject.GetMeshFromMeshFilter();
            if (mesh != null) return new Box(mesh.bounds);

            mesh = gameObject.GetMeshFromSkinnedMeshRenderer();
            if (mesh != null) return new Box(gameObject.GetComponent<SkinnedMeshRenderer>().localBounds);

            return Box.GetInvalid();
        }

        public static OrientedBox GetNonMeshModelSpaceOrientedBox(this GameObject gameObject)
        {
            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && spriteRenderer.sprite != null) return new OrientedBox(spriteRenderer.GetModelSpaceBox());

            if (!gameObject.HasLight() && !gameObject.HasParticleSystem()) return OrientedBox.GetInvalid();
            return new OrientedBox(gameObject.GetNonMeshModelSpaceBox());
        }

        public static Box GetNonMeshModelSpaceBox(this GameObject gameObject)
        {
            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && spriteRenderer.sprite != null) return spriteRenderer.GetModelSpaceBox();

            if (!gameObject.HasLight() && !gameObject.HasParticleSystem()) return Box.GetInvalid();
            return new Box(Vector3.zero, Octave3DScene.VolumeSizeForNonMeshObjects);
        }

        public static Renderer GetRenderer(this GameObject gameObject)
        {
            return gameObject.GetComponent<Renderer>();
        }

        public static bool IsSprite(this GameObject gameObject)
        {
            if (gameObject.HasMesh()) return false;

            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            return spriteRenderer != null && spriteRenderer.sprite != null;
        }

        public static Mesh GetMeshFromFilterOrSkinnedMeshRenderer(this GameObject gameObject)
        {
            Mesh mesh = gameObject.GetMeshFromMeshFilter();
            if (mesh == null) mesh = gameObject.GetMeshFromSkinnedMeshRenderer();

            return mesh;
        }

        public static Mesh GetMeshFromMeshFilter(this GameObject gameObject)
        {
            MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
            if (meshFilter != null && meshFilter.sharedMesh != null) return meshFilter.sharedMesh;

            return null;
        }

        public static Mesh GetMeshFromSkinnedMeshRenderer(this GameObject gameObject)
        {
            SkinnedMeshRenderer skinnedMeshRenderer = gameObject.GetComponent<SkinnedMeshRenderer>();
            if (skinnedMeshRenderer != null && skinnedMeshRenderer.sharedMesh != null) return skinnedMeshRenderer.sharedMesh;

            return null;
        }
        #endregion

        #region Utilities
        public static List<GameObject> GetTopParentsFromGameObjectCollection(IEnumerable<GameObject> gameObjects)
        {
            List<GameObject> topParents = new List<GameObject>();
            foreach (GameObject gameObject in gameObjects)
            {
                bool foundParentForThisObject = false;
                Transform gameObjectTransform = gameObject.transform;

                foreach (GameObject potentialParent in gameObjects)
                {
                    if (gameObject != potentialParent && 
                        gameObjectTransform.IsChildOf(potentialParent.transform))
                    {
                        foundParentForThisObject = true;
                        break;
                    }
                }

                if (!foundParentForThisObject) topParents.Add(gameObject);
            }

            return topParents;
        }

        public static List<GameObject> GetAllObjectsInHierarchyCollection(List<GameObject> hierarchyRoots)
        {
            var allGameObjects = new List<GameObject>(hierarchyRoots.Count);
            foreach(GameObject root in hierarchyRoots)
            {
                allGameObjects.AddRange(root.GetAllChildrenIncludingSelf());
            }

            return allGameObjects;
        }

        public static void RecordObjectTransformsForUndo(IEnumerable<GameObject> gameObjects)
        {
            List<Transform> objectTransforms = GetObjectTransforms(gameObjects);
            UndoEx.RecordForToolAction(objectTransforms);
        }

        public static List<Transform> GetObjectTransforms(IEnumerable<GameObject> gameObjects)
        {
            var objectTransforms = new List<Transform>();
            foreach(GameObject gameObject in gameObjects)
            {
                objectTransforms.Add(gameObject.transform);
            }

            return objectTransforms;
        }

        public static List<OrientedBox> GetHierarchyWorldOrientedBoxes(List<GameObject> hierarchyRoots)
        {
            if (hierarchyRoots.Count == 0) return new List<OrientedBox>();

            var orientedBoxes = new List<OrientedBox>(hierarchyRoots.Count);
            foreach (GameObject selectedObject in hierarchyRoots)
            {
                OrientedBox orientedBox = selectedObject.GetHierarchyWorldOrientedBox();
                if (orientedBox.IsValid()) orientedBoxes.Add(orientedBox);
            }

            return orientedBoxes;
        }

        public static void SetSelectedHierarchyWireframeHidden(List<GameObject> hierarchyRoots, bool isWireframeHidden)
        {
            foreach(GameObject root in hierarchyRoots)
            {
                root.SetSelectedHierarchyWireframeHidden(isWireframeHidden);
            }
        }

        public static void AssignGameObjectsToLayer(List<GameObject> gameObjects, int objectLayer, bool allowUndoRedo)
        {
            if(allowUndoRedo)
            {
                foreach (GameObject gameObject in gameObjects)
                {
                    UndoEx.RecordForToolAction(gameObject);
                    gameObject.layer = objectLayer;
                }
            }
            else
            {
                foreach (GameObject gameObject in gameObjects)
                {
                    gameObject.layer = objectLayer;
                }
            }
        }

        public static List<GameObject> GetObjectsWithMesh(List<GameObject> gameObjects)
        {
            if (gameObjects.Count == 0) return new List<GameObject>();

            var objectsWithMesh = new List<GameObject>(gameObjects.Count);
            foreach(var gameObject in gameObjects)
            {
                if (gameObject.HasMesh()) objectsWithMesh.Add(gameObject);
            }

            return objectsWithMesh;
        }
        #endregion
    }
}
#endif