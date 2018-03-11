#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectSelectionRenderSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private ObjectSelectionRenderMode _renderMode = ObjectSelectionRenderMode.Box;

        [SerializeField]
        private ObjectSelectionBoxRenderModeSettings _boxRenderModeSettings;
        [SerializeField]
        private ObjectSelectionRectangleRenderModeSettings _rectangleRenderModeSettings;

        [SerializeField]
        private ObjectSelectionRenderSettingsView _view;
        #endregion

        #region Public Properties
        public ObjectSelectionRenderMode RenderMode { get { return _renderMode; } set { _renderMode = value; } }
        public ObjectSelectionBoxRenderModeSettings BoxRenderModeSettings
        {
            get
            {
                if (_boxRenderModeSettings == null) _boxRenderModeSettings = Octave3DWorldBuilder.Instance.CreateScriptableObject<ObjectSelectionBoxRenderModeSettings>();
                return _boxRenderModeSettings;
            }
        }
        public ObjectSelectionRectangleRenderModeSettings RectangleRenderModeSettings
        {
            get
            {
                if (_rectangleRenderModeSettings == null) _rectangleRenderModeSettings = Octave3DWorldBuilder.Instance.CreateScriptableObject<ObjectSelectionRectangleRenderModeSettings>();
                return _rectangleRenderModeSettings;
            }
        }
        public ObjectSelectionRenderSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public ObjectSelectionRenderSettings()
        {
            _view = new ObjectSelectionRenderSettingsView(this);
        }
        #endregion

        #region Public Static Functions
        public static ObjectSelectionRenderSettings Get()
        {
            return ObjectSelection.Get().RenderSettings;
        }
        #endregion
    }
}
#endif