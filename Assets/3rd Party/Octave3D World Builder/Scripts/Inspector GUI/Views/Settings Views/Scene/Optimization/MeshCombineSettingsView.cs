#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class MeshCombineSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private MeshCombineSettings _settings;
        #endregion

        #region Constructors
        public MeshCombineSettingsView(MeshCombineSettings settings)
        {
            ToggleVisibilityBeforeRender = true;
            VisibilityToggleLabel = "Mesh Combine Settings";
            SurroundWithBox = true;
            _settings = settings;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderCombineOnlyStaticMeshesToggle();
            RenderMakeCombinedMeshesStaticToggle();
            RenderIgnoreInactiveGameObjectsToggle();
            RenderHideWireframeToggle();
            RenderRemoveEmptyHierarchiesToggle();

            EditorGUILayout.Separator();
            RenderWeldVertexPositionsToggle();
            RenderWeldVertexPositionsOnlyForCommonMaterialToggle();
            RenderVertexPositionWeldEpsilonField();

            EditorGUILayout.Separator();
            RenderAttachMeshCollidersToCombinedMeshesToggle();
            RenderUseConvexMeshCollidersToggle();

            EditorGUILayout.Separator();
            RenderNameOfParentObjectsField();
            RenderNameOfCombinedMeshesField();
        }
        #endregion

        #region Private Methods
        private void RenderCombineOnlyStaticMeshesToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForCombineOnlyStaticMeshesToggle(), _settings.CombineOnlyStaticMeshes);
            if(newBool != _settings.CombineOnlyStaticMeshes)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.CombineOnlyStaticMeshes = newBool;
            }
        }

        private GUIContent GetContentForCombineOnlyStaticMeshesToggle()
        {
            var content = new GUIContent();
            content.text = "Combine only static meshes";
            content.tooltip = "If this is checked, only meshes attached to static game objects will be combined.";

            return content;
        }

        private void RenderMakeCombinedMeshesStaticToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForMakeCombinedMeshesStaticToggle(), _settings.MakeCombinedMeshesStatic);
            if (newBool != _settings.MakeCombinedMeshesStatic)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.MakeCombinedMeshesStatic = newBool;
            }
        }

        private GUIContent GetContentForMakeCombinedMeshesStaticToggle()
        {
            var content = new GUIContent();
            content.text = "Make combined meshes static";
            content.tooltip = "If this is checked, all objects associated with the combined meshes will be marked as static. If this is not checked, they will be marked as dynamic.";

            return content;
        }

        private void RenderIgnoreInactiveGameObjectsToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForIgnoreInactiveGameObjectsToggle(), _settings.IgnoreInactiveGameObjects);
            if(newBool != _settings.IgnoreInactiveGameObjects)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.IgnoreInactiveGameObjects = newBool;
            }
        }

        private GUIContent GetContentForIgnoreInactiveGameObjectsToggle()
        {
            var content = new GUIContent();
            content.text = "Ignore inactive game objects";
            content.tooltip = "If this is checked, objects which are not active in the scene will not participate in the mesh combine process.";

            return content;
        }

        private void RenderHideWireframeToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForHideWireframeToggle(), _settings.HideWireframe);
            if (newBool != _settings.HideWireframe)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.HideWireframe = newBool;
            }
        }

        private GUIContent GetContentForHideWireframeToggle()
        {
            var content = new GUIContent();
            content.text = "Hide wireframe";
            content.tooltip = "If this is checked, wireframe rendering will be disabled for all objects which are created during mesh combine (including those whose meshes did not participate in the combine process).";

            return content;
        }

        private void RenderRemoveEmptyHierarchiesToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForRemoveEmptyHierarchies(), _settings.RemoveEmptyHierarchies);
            if(newBool != _settings.RemoveEmptyHierarchies)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.RemoveEmptyHierarchies = newBool;
            }
        }

        private GUIContent GetContentForRemoveEmptyHierarchies()
        {
            var content = new GUIContent();
            content.text = "Remove empty hierarchies";
            content.tooltip = "The meshes which participate in the combine process will be removed from their hierarchies and it is possible that those hierarchies " + 
                              "will become empty (i.e. they contain no mesh, light, or particle system objects). If this is checked, those hierarchies will be removed after " + 
                              "the mesh combine process is finished.";

            return content;
        }

        private void RenderWeldVertexPositionsToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForWeldVertexPositionsToggle(), _settings.WeldVertexPositions);
            if(newBool != _settings.WeldVertexPositions)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.WeldVertexPositions = newBool;
            }
        }

        private GUIContent GetContentForWeldVertexPositionsToggle()
        {
            var content = new GUIContent();
            content.text = "Weld vertex positions";
            content.tooltip = "If this is checked, the mesh combiner will merge the positions of vertices which sit next to each other. " + 
                              "Note: As the label suggests, only the positions are affected. The mesh combiner will NOT remove vertices from the mesh.";

            return content;
        }

        private void RenderWeldVertexPositionsOnlyForCommonMaterialToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForWeldVertexPositionsOnlyForCommonMaterialToggle(), _settings.WeldVertexPositionsOnlyForCommonMaterial);
            if(newBool != _settings.WeldVertexPositionsOnlyForCommonMaterial)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.WeldVertexPositionsOnlyForCommonMaterial = newBool;
            }
        }

        private GUIContent GetContentForWeldVertexPositionsOnlyForCommonMaterialToggle()
        {
            var content = new GUIContent();
            content.text = "Weld only for common material";
            content.tooltip = "If this is checked, the vertex weld process is applied only to meshes which share the same material. Otherwise, vertices will be welded " + 
                              "between all meshes which participate in the combine process.";

            return content;
        }

        private void RenderVertexPositionWeldEpsilonField()
        {
            float newFloat = EditorGUILayout.FloatField(GetContentForVertexPositionWeldEpsilonField(), _settings.VertexPositionWeldEpsilon);
            if(newFloat != _settings.VertexPositionWeldEpsilon)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.VertexPositionWeldEpsilon = newFloat;
            }
        }

        private GUIContent GetContentForVertexPositionWeldEpsilonField()
        {
            var content = new GUIContent();
            content.text = "Vertex position weld epsilon";
            content.tooltip = "This is the vertex position weld epsilon. Vertex positions will only be merged if the distance between the vertices is <= this value.";

            return content;
        }

        private void RenderAttachMeshCollidersToCombinedMeshesToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForAttachMeshCollidersToCombinedMeshesToggle(), _settings.AttachMeshCollidersToCombinedMeshes);
            if(newBool != _settings.AttachMeshCollidersToCombinedMeshes)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.AttachMeshCollidersToCombinedMeshes = newBool;
            }
        }

        private GUIContent GetContentForAttachMeshCollidersToCombinedMeshesToggle()
        {
            var content = new GUIContent();
            content.text = "Attach mesh colliders to combined meshes";
            content.tooltip = "If this is checked, mesh colliders will be attached to the combined meshes.";

            return content;
        }

        private void RenderUseConvexMeshCollidersToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForUseConvexMeshColliders(), _settings.UseConvexMeshColliders);
            if (newBool != _settings.UseConvexMeshColliders)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.UseConvexMeshColliders = newBool;
            }
        }

        private GUIContent GetContentForUseConvexMeshColliders()
        {
            var content = new GUIContent();
            content.text = "Use convex mesh colliders";
            content.tooltip = "If this is checked, and if mesh colliders must be attached to the combined meshes, the mesh colliders will be marked as convex.";

            return content;
        }

        private void RenderNameOfParentObjectsField()
        {
            string newString = EditorGUILayout.TextField(GetContentForNameOfParentObjectsField(), _settings.NameOfParentObjects);
            if(newString != _settings.NameOfParentObjects)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.NameOfParentObjects = newString;
            }
        }

        private GUIContent GetContentForNameOfParentObjectsField()
        {
            var content = new GUIContent();
            content.text = "Name of parent objects";
            content.tooltip = "When meshes are combined, a game object will be created for each combined mesh. This is the name that must " + 
                              "be assigned to those objects.";

            return content;
        }

        private void RenderNameOfCombinedMeshesField()
        {
            string newString = EditorGUILayout.TextField(GetContentForNameOfCombinedMeshesField(), _settings.NameOfCombinedMeshes);
            if (newString != _settings.NameOfCombinedMeshes)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.NameOfCombinedMeshes = newString;
            }
        }

        private GUIContent GetContentForNameOfCombinedMeshesField()
        {
            var content = new GUIContent();
            content.text = "Name of combined meshes";
            content.tooltip = "When a combined mesh is created, its name will be set to this value.";

            return content;
        }
        #endregion
    }
}
#endif