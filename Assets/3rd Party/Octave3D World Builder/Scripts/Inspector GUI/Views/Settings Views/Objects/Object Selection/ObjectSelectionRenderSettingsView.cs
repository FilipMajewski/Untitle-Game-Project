#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectSelectionRenderSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private ObjectSelectionRenderSettings _settings;
        #endregion

        #region Constructors
        public ObjectSelectionRenderSettingsView(ObjectSelectionRenderSettings settings)
        {
            _settings = settings;

            ToggleVisibilityBeforeRender = true;
            VisibilityToggleLabel = "Object Selection Rendering";
            IndentContent = true;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderObjectSelectionRenderModeSelectionField();

            if (_settings.RenderMode == ObjectSelectionRenderMode.Box) _settings.BoxRenderModeSettings.View.Render();
            else if (_settings.RenderMode == ObjectSelectionRenderMode.Rectangle) _settings.RectangleRenderModeSettings.View.Render();
        }
        #endregion

        #region Private Methods
        private void RenderObjectSelectionRenderModeSelectionField()
        {
            ObjectSelectionRenderMode newSelectionRenderMode = (ObjectSelectionRenderMode)EditorGUILayout.EnumPopup(GetContentForObjectSelectionRenderModeSelectionPopup(), _settings.RenderMode);
            if (newSelectionRenderMode != _settings.RenderMode)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.RenderMode = newSelectionRenderMode;

                SceneView.RepaintAll();
            }
        }

        private GUIContent GetContentForObjectSelectionRenderModeSelectionPopup()
        {
            var content = new GUIContent();
            content.text = "Draw mode";
            content.tooltip = "Allows you to specify what kind of drawing is performed in the scene for the selected objects. For example, " +
                              "when the \'Box\' draw mode is active, a selection box will be drawn for each selected object.";

            return content;
        }
        #endregion
    }
}
#endif