#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class DecorPaintObjectPlacementBrush : ScriptableObject, INamedEntity
    {
        #region Private Variables
        [SerializeField]
        private string _name = "";

        [SerializeField]
        private float _radius = 1.5f;
        [SerializeField]
        private int _maxNumberOfObjects = 10;
        [SerializeField]
        private float _distanceBetweenObjects = 0.5f;
        [SerializeField]
        private bool _ignoreObjectsOutsideOfPaintSurface = true;

        [SerializeField]
        private List<DecorPaintObjectPlacementBrushElement> _elements = new List<DecorPaintObjectPlacementBrushElement>();
        [SerializeField]
        private PrefabCategory _destinationCategoryForElementPrefabs;

        [SerializeField]
        private DecorPaintObjectPlacementBrushView _view;
        #endregion

        #region Public Static Properties
        public static float MinRadius { get { return 0.1f; } }
        public static int MinObjectAmount { get { return 1; } }
        public static float MinObjectScatter { get { return 0.0f; } }
        #endregion

        #region Public Properties
        public string Name { get { return _name; } set { if (!string.IsNullOrEmpty(value)) _name = value; } }
        public float Radius { get { return _radius; } set { _radius = Mathf.Max(value, MinRadius); } }
        public int MaxNumberOfObjects { get { return _maxNumberOfObjects; } set { _maxNumberOfObjects = Mathf.Max(value, MinObjectAmount); } }
        public float DistanceBetweenObjects { get { return _distanceBetweenObjects; } set { _distanceBetweenObjects = value; } }
        public bool IgnoreObjectsOutsideOfPaintSurface { get { return _ignoreObjectsOutsideOfPaintSurface; } set { _ignoreObjectsOutsideOfPaintSurface = value; } }
        public int NumberOfElements { get { return _elements.Count; } }
        public bool IsEmpty { get { return NumberOfElements == 0; } }
        public PrefabCategory DestinationCategoryForElementPrefabs
        {
            get
            {
                if (_destinationCategoryForElementPrefabs == null) _destinationCategoryForElementPrefabs = PrefabCategoryDatabase.Get().GetDefaultPrefabCategory();
                return _destinationCategoryForElementPrefabs;
            }
            set
            {
                if (value != null) _destinationCategoryForElementPrefabs = value;
            }
        }
        public DecorPaintObjectPlacementBrushView View { get { return _view; } }
        #endregion

        #region Constructors
        public DecorPaintObjectPlacementBrush()
        {
            _view = new DecorPaintObjectPlacementBrushView(this);
        }
        #endregion

        #region Public Methods
        public void RemoveAndDestroyElement(DecorPaintObjectPlacementBrushElement element)
        {
            if(ContainsElement(element) && element != null)
            {
                _elements.Remove(element);
                UndoEx.DestroyObjectImmediate(element);
            }
        }

        public bool ContainsElement(DecorPaintObjectPlacementBrushElement element)
        {
            return _elements.Contains(element);
        }

        public void RemoveAndDestroyAllElements()
        {
            while (!IsEmpty) RemoveAndDestroyElement(_elements[0]);
        }

        public void CreateNewElement()
        {
            DecorPaintObjectPlacementBrushElement newElement = Octave3DWorldBuilder.Instance.CreateScriptableObject<DecorPaintObjectPlacementBrushElement>();
            newElement.ParentBrush = this;
            _elements.Add(newElement);
        }

        public DecorPaintObjectPlacementBrushElement GetBrushElementByIndex(int elementIndex)
        {
            return _elements[elementIndex];
        }

        public List<DecorPaintObjectPlacementBrushElement> GetAllBrushElements()
        {
            return new List<DecorPaintObjectPlacementBrushElement>(_elements);
        }

        public List<DecorPaintObjectPlacementBrushElement> GetAllValidAndActiveBrushElements()
        {
            return _elements.FindAll(item => item != null && item.IsValid() && item.IsActive);
        }

        public List<Vector3> GetPrefabWorldScaleForAllValidAndActiveBrushElements()
        {
            if (IsEmpty) return new List<Vector3>();

            List<DecorPaintObjectPlacementBrushElement> allValidAndActiveBrushElements = GetAllValidAndActiveBrushElements();
            if (allValidAndActiveBrushElements.Count == 0) return new List<Vector3>();

            var prefabWorldScaleValues = new List<Vector3>(allValidAndActiveBrushElements.Count);
            for(int validElementIndex = 0; validElementIndex < allValidAndActiveBrushElements.Count; ++validElementIndex)
            {
                prefabWorldScaleValues.Add(allValidAndActiveBrushElements[validElementIndex].Prefab.UnityPrefab.transform.lossyScale);
            }

            return prefabWorldScaleValues;
        }

        public List<OrientedBox> GetPrefabWorldOrientedBoxesForAllValidAndActiveBrushElements()
        {
            if (IsEmpty) return new List<OrientedBox>();

            List<DecorPaintObjectPlacementBrushElement> allValidAndActiveBrushElements = GetAllValidAndActiveBrushElements();
            if (allValidAndActiveBrushElements.Count == 0) return new List<OrientedBox>();

            var prefabOrientedBoxes = new List<OrientedBox>(allValidAndActiveBrushElements.Count);
            for (int validElementIndex = 0; validElementIndex < allValidAndActiveBrushElements.Count; ++validElementIndex)
            {
                prefabOrientedBoxes.Add(allValidAndActiveBrushElements[validElementIndex].Prefab.UnityPrefab.GetHierarchyWorldOrientedBox());
            }

            return prefabOrientedBoxes;
        }

        public void RecordAllElementsForUndo()
        {
            foreach(var element in _elements)
            {
                UndoEx.RecordForToolAction(element);
            }
        }

        public void RemovePrefabAssociationForAllElements(Prefab prefab)
        {
            foreach(var element in _elements)
            {
                if (element.Prefab == prefab) element.Prefab = null;
            }
        }

        public void RemovePrefabAssociationForAllElements(List<Prefab> prefabs)
        {
            foreach (var element in _elements)
            {
                if (prefabs.Contains(element.Prefab)) element.Prefab = null;
            }
        }
        #endregion

        #region Private Methods
        private void OnDestroy()
        {
            if (_view != null) _view.Dispose();
            RemoveAndDestroyAllElements();
        }
        #endregion
    }
}
#endif