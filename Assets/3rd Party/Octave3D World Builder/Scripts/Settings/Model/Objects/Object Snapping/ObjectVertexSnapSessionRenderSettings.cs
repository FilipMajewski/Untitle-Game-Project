#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectVertexSnapSessionRenderSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private bool _renderSourceVertex = true;
        [SerializeField]
        private Color _sourceVertexFillColor = Color.green;
        [SerializeField]
        private Color _sourceVertexBorderColor = Color.black;
        [SerializeField]
        private float _sourceVertexRadiusInPixels = 5.0f;

        [SerializeField]
        private bool _renderSnapPosition = true;
        [SerializeField]
        private Color _snapPositionFillColor = Color.blue;
        [SerializeField]
        private Color _snapPositionBorderColor = Color.black;
        [SerializeField]
        private float _snapPositionRadiusInPixels = 5.0f;

        [SerializeField]
        private ObjectVertexSnapSessionRenderSettingsView _view;
        #endregion

        #region Public Static Properties
        public static float MinPointRadiusInPixels { get { return 2.0f; } }
        #endregion

        #region Public Properties
        public bool RenderSourceVertex { get { return _renderSourceVertex; } set { _renderSourceVertex = value; } }
        public Color SourceVertexFillColor { get { return _sourceVertexFillColor; } set { _sourceVertexFillColor = value; } }
        public Color SourceVertexBorderColor { get { return _sourceVertexBorderColor; } set { _sourceVertexBorderColor = value; } }
        public float SourceVertexRadiusInPixels { get { return _sourceVertexRadiusInPixels; } set { _sourceVertexRadiusInPixels = Mathf.Max(value, MinPointRadiusInPixels); } }

        public bool RenderSnapPosition { get { return _renderSnapPosition; } set { _renderSnapPosition = value; } }
        public Color SnapPositionFillColor { get { return _snapPositionFillColor; } set { _snapPositionFillColor = value; } }
        public Color SnapPositionBorderColor { get { return _snapPositionBorderColor; } set { _snapPositionBorderColor = value; } }
        public float SnapPositionRadiusInPixels { get { return _snapPositionRadiusInPixels; } set { _snapPositionRadiusInPixels = Mathf.Max(value, MinPointRadiusInPixels); } }

        public ObjectVertexSnapSessionRenderSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        private ObjectVertexSnapSessionRenderSettings()
        {
            _view = new ObjectVertexSnapSessionRenderSettingsView(this);
        }
        #endregion
    }
}
#endif