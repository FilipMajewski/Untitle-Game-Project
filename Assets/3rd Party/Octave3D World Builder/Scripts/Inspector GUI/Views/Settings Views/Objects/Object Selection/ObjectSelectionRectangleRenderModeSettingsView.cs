#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectSelectionRectangleRenderModeSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private ObjectSelectionRectangleRenderModeSettings _settings;
        #endregion

        #region Constructors
        public ObjectSelectionRectangleRenderModeSettingsView(ObjectSelectionRectangleRenderModeSettings settings)
        {
            _settings = settings;

            ToggleVisibilityBeforeRender = true;
            VisibilityToggleLabel = "Rectangle Mode";
            IndentContent = true;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderRectangleBorderColorField();
            RenderRectangleFillColorField();
        }
        #endregion

        #region Private Methods
        private void RenderRectangleBorderColorField()
        {
            Color newColor = EditorGUILayout.ColorField(GetContentForRectangleBorderColorField(), _settings.RectangleBorderColor);
            if (newColor != _settings.RectangleBorderColor)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.RectangleBorderColor = newColor;
            }
        }

        private GUIContent GetContentForRectangleBorderColorField()
        {
            var content = new GUIContent();
            content.text = "Border color";
            content.tooltip = "Allows you to specify the color which must be used to draw the rectangle border lines.";

            return content;
        }

        private void RenderRectangleFillColorField()
        {
            Color newColor = EditorGUILayout.ColorField(GetContentForRectangleFillColorField(), _settings.RectangleFillColor);
            if (newColor != _settings.RectangleFillColor)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.RectangleFillColor = newColor;
            }
        }

        private GUIContent GetContentForRectangleFillColorField()
        {
            var content = new GUIContent();
            content.text = "Fill color";
            content.tooltip = "Allows you to specify the color which must be used to draw the interior of the rectangle shape.";

            return content;
        }
        #endregion
    }
}
#endif