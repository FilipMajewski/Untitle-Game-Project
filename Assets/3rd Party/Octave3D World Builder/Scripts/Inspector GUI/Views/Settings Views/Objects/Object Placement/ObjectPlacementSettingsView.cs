#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private ObjectPlacementSettings _settings;

        [SerializeField]
        private ObjectPlacementModeSelectionToolbar _objectPlacementModeSelectionToolbar = new ObjectPlacementModeSelectionToolbar();
        #endregion

        #region Public Properties
        public ObjectPlacementModeSelectionToolbar ObjectPlacementModeSelectionToolbar { get { return _objectPlacementModeSelectionToolbar; } }
        #endregion

        #region Constructors
        public ObjectPlacementSettingsView(ObjectPlacementSettings settings)
        {
            _settings = settings;

            ToggleVisibilityBeforeRender = true;
            VisibilityToggleLabel = "Object Placement Settings";
            SurroundWithBox = true;

            _objectPlacementModeSelectionToolbar.ButtonScale = 0.25f;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            EditorGUILayout.BeginHorizontal();
            RenderOpenMoreSettingsWindowButton();
            RenderOpenPrefabManagementWindowButton();
            RenderOpenPrefabTagsWindowButton();
            RenderOpenObjectLayersWindowButton();
            EditorGUILayout.EndHorizontal();

            _objectPlacementModeSelectionToolbar.Render();
            _settings.ObjectIntersectionSettings.View.Render();
            RenderMakePlacedObjectsChildrenOfHoveredObject();
            RenderAttachPlacedObjectsToActiveGroupToggle();

            EditorGUILayout.Separator();
            if (_settings.ObjectPlacementMode == ObjectPlacementMode.DecorPaint) RenderDecorPaintPlacementControls();
            else if (_settings.ObjectPlacementMode == ObjectPlacementMode.PointAndClick) RenderPointAndClickPlacementControls();
            else if (_settings.ObjectPlacementMode == ObjectPlacementMode.Path) RenderPathPlacementControls();
            else if (_settings.ObjectPlacementMode == ObjectPlacementMode.Block) RenderBlockPlacementControls();
        }
        #endregion

        #region Private Methods
        private void RenderOpenMoreSettingsWindowButton()
        {
            if(GUILayout.Button(GetContentForOpenMoreSettingsWindowButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth * 0.54f)))
            {
                ObjectPlacementSettingsWindow.Get().ShowOctave3DWindow();
            }
        }

        private GUIContent GetContentForOpenMoreSettingsWindowButton()
        {
            var content = new GUIContent();
            content.text = "More settings...";
            content.tooltip = "Opens up a window which allows you to modify additional settings.";

            return content;
        }

        private void RenderOpenPrefabManagementWindowButton()
        {
            if (GUILayout.Button(GetContentForOpenPrefabManagementWindowButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth * 0.4f)))
            {
                PrefabManagementWindow.Get().ShowOctave3DWindow();
            }
        }

        private GUIContent GetContentForOpenPrefabManagementWindowButton()
        {
            var content = new GUIContent();
            content.text = "Prefabs...";
            content.tooltip = "Opens up a window which allows you to manage prefabs and prefab categories.";

            return content;
        }

        private void RenderOpenPrefabTagsWindowButton()
        {
            if (GUILayout.Button(GetContentForOpenPrefabTagsWindowButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth * 0.45f)))
            {
                PrefabTagsWindow.Get().ShowOctave3DWindow();
            }
        }

        private GUIContent GetContentForOpenPrefabTagsWindowButton()
        {
            var content = new GUIContent();
            content.text = "Prefab tags...";
            content.tooltip = "Opens up a window which allows you to manage prefab tags.";

            return content;
        }

        private void RenderOpenObjectLayersWindowButton()
        {
            if (GUILayout.Button(GetContentForOpenObjectLayersWindowButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth * 0.48f)))
            {
                ObjectLayersWindow.Get().ShowOctave3DWindow();
            }
        }

        private GUIContent GetContentForOpenObjectLayersWindowButton()
        {
            var content = new GUIContent();
            content.text = "Object layers...";
            content.tooltip = "Opens up a window which allows you to perform object layer actions.";

            return content;
        }

        private void RenderMakePlacedObjectsChildrenOfHoveredObject()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForMakePlacedObjectsChildrenOfHoveredObject(), _settings.MakePlacedObjectsChildrenOfHoveredObject);
            if(newBool != _settings.MakePlacedObjectsChildrenOfHoveredObject)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.MakePlacedObjectsChildrenOfHoveredObject = newBool;
            }
        }

        private GUIContent GetContentForMakePlacedObjectsChildrenOfHoveredObject()
        {
            var content = new GUIContent();
            content.text = "Attach to hovered object";
            content.tooltip = "If this is checked, any object that is placed in the scene will be made a child of the object which was hovered " +
                              "when the object was instantiated. Note: This option is ignored when \'Attach to active object group\' is checked.";

            return content;
        }

        private void RenderAttachPlacedObjectsToActiveGroupToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForAttachPlacedObjectsToActiveGroupToggle(), _settings.AttachPlacedObjectsToActiveObjectGroup);
            if(newBool != _settings.AttachPlacedObjectsToActiveObjectGroup)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.AttachPlacedObjectsToActiveObjectGroup = newBool;
            }
        }

        private GUIContent GetContentForAttachPlacedObjectsToActiveGroupToggle()
        {
            var content = new GUIContent();
            content.text = "Attach to active object group";
            content.tooltip = "If this is checked, all placed objects will be attached to the currently active object group (if any).";

            return content;
        }

        private void RenderDecorPaintPlacementControls()
        {
            _settings.DecorPaintObjectPlacementSettings.View.Render();
        }

        private void RenderPointAndClickPlacementControls()
        {
            _settings.PointAndClickPlacementSettings.View.Render();
        }

        private void RenderPathPlacementControls()
        {
            _settings.PathPlacementSettings.View.Render();

            EditorGUILayout.Separator();
            PathObjectPlacement.Get().PathSettings.View.Render();

            EditorGUILayout.Separator();
            if (PathObjectPlacement.Get().PathSettings.ManualConstructionSettings.HeightAdjustmentSettings.HeightAdjustmentMode == ObjectPlacementPathHeightAdjustmentMode.AutomaticPattern)
                ObjectPlacementPathHeightPatternDatabase.Get().View.Render();
        }

        private void RenderBlockPlacementControls()
        {
            _settings.BlockObjectPlacementSettings.View.Render();

            EditorGUILayout.Separator();
            BlockObjectPlacement.Get().BlockSettings.View.Render();
        }
        #endregion
    }
}
#endif