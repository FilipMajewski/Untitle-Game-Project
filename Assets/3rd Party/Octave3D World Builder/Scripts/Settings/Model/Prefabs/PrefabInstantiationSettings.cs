#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class PrefabInstantiationSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private bool _instantiatedObjectsAreStatic = true;

        [SerializeField]
        private PrefabInstantiationSettingsView _view;
        #endregion

        #region Public Properties
        public bool InstantiatedObjectsAreStatic { get { return _instantiatedObjectsAreStatic; } set { _instantiatedObjectsAreStatic = value; } }
        public PrefabInstantiationSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public PrefabInstantiationSettings()
        {
            _view = new PrefabInstantiationSettingsView(this);
        }
        #endregion

        #region Private Methods
        private void OnDestroy()
        {
            if (_view != null) _view.Dispose();
        }
        #endregion
    }
}
#endif