#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class ActivePrefabCategoryViewData : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private string _nameForNewPrefabCategory = "";
        [SerializeField]
        private PrefabCategory _destinationCategoryForAllPrefabs;
        [SerializeField]
        private PrefabCategory _destinationCategoryForFilteredPrefabs;
        [SerializeField]
        private PrefabCategory _destinationCategoryForActivePrefab;

        [SerializeField]
        private float _prefabOffsetFromGridSurface = 0.0f;
        [SerializeField]
        private float _prefabOffsetFromObjectSurface = 0.0f;

        [SerializeField]
        private bool _isActionViewVisible = false;
        #endregion

        #region Public Properties
        public string NameForNewPrefabCategory { get { return _nameForNewPrefabCategory; } set { if (value != null) _nameForNewPrefabCategory = value; } }
        public PrefabCategory DestinationCategoryForAllPrefabs 
        {
            get 
            {
                if (_destinationCategoryForAllPrefabs == null) _destinationCategoryForAllPrefabs = PrefabCategoryDatabase.Get().GetDefaultPrefabCategory();
                return _destinationCategoryForAllPrefabs; 
            }
            set { if (value != null) _destinationCategoryForAllPrefabs = value; } 
        }
        public PrefabCategory DestinationCategoryForFilteredPrefabs
        {
            get 
            {
                if (_destinationCategoryForFilteredPrefabs == null) _destinationCategoryForFilteredPrefabs = PrefabCategoryDatabase.Get().GetDefaultPrefabCategory();
                return _destinationCategoryForFilteredPrefabs; 
            }
            set { if (value != null) _destinationCategoryForFilteredPrefabs = value; } 
        }
        public PrefabCategory DestinationCategoryForActivePrefab
        {
            get
            {
                if (_destinationCategoryForActivePrefab == null) _destinationCategoryForActivePrefab = PrefabCategoryDatabase.Get().GetDefaultPrefabCategory();
                return _destinationCategoryForActivePrefab;
            }
            set { if (value != null) _destinationCategoryForActivePrefab = value; } 
        }
        public float PrefabOffsetFromGridSurface { get { return _prefabOffsetFromGridSurface; } set { _prefabOffsetFromGridSurface = value; } }
        public float PrefabOffsetFromObjectSurface { get { return _prefabOffsetFromObjectSurface; } set { _prefabOffsetFromObjectSurface = value; } }
        public bool IsActionViewVisible { get { return _isActionViewVisible; } set { _isActionViewVisible = value; } }
        #endregion
    }
}
#endif