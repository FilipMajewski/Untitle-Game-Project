#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class GUIRenderableContentVisibilityData : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private bool _isVisible = true;
        #endregion

        #region Public Properties
        public bool IsVisible { get { return _isVisible; } set { _isVisible = value; } }
        #endregion
    }
}
#endif