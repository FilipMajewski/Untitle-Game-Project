#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectSelectionRectangleRenderModeSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private Color _rectangleBorderColor = Color.green;
        [SerializeField]
        private Color _rectangleFillColor = new Color(0.0f, 1.0f, 0.0f, 0.11f);

        [SerializeField]
        private ObjectSelectionRectangleRenderModeSettingsView _view;
        #endregion

        #region Public Properties
        public Color RectangleBorderColor { get { return _rectangleBorderColor; } set { _rectangleBorderColor = value; } }
        public Color RectangleFillColor { get { return _rectangleFillColor; } set { _rectangleFillColor = value; } }
        public ObjectSelectionRectangleRenderModeSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public ObjectSelectionRectangleRenderModeSettings()
        {
            _view = new ObjectSelectionRectangleRenderModeSettingsView(this);
        }
        #endregion
    }
}
#endif