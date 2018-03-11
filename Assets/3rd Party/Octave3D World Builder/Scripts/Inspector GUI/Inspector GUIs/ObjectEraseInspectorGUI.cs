#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectEraseInspectorGUI : InspectorGUI
    {
        #region Private Variables
        [SerializeField]
        private ObjectEraserLookAndFeelSettingsView _lookAndFeelSettingsView = new ObjectEraserLookAndFeelSettingsView();
        #endregion

        #region Public Methods
        public override void Initialize()
        {
            base.Initialize();

            _lookAndFeelSettingsView.IsVisible = false;

            EllipseShapeRenderSettingsView circle2DShapeRenderSettingsView = ObjectEraser.Get().Circle2DMassEraseShapeRenderSettings.View;
            circle2DShapeRenderSettingsView.ToggleVisibilityBeforeRender = true;
            circle2DShapeRenderSettingsView.IndentContent = true;
            circle2DShapeRenderSettingsView.VisibilityToggleLabel = "2D Mass Erase Circle";

            XZOrientedEllipseShapeRenderSettingsView circle3DShapeRenderSettingsView = ObjectEraser.Get().Circle3DMassEraseShapeRenderSettings.View;
            circle3DShapeRenderSettingsView.ToggleVisibilityBeforeRender = true;
            circle3DShapeRenderSettingsView.IndentContent = true;
            circle3DShapeRenderSettingsView.VisibilityToggleLabel = "3D Mass Erase Circle";
        }

        public override void Render()
        {
            ObjectEraserSettings.Get().View.Render();
            _lookAndFeelSettingsView.Render();
        }
        #endregion
    }
}
#endif