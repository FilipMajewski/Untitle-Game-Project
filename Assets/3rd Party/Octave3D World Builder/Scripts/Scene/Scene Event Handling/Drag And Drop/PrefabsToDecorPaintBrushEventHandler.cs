#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class PrefabsToDecorPaintBrushEventHandler : DragAndDropEventHandler
    {
        #region Private Variables
        private DecorPaintObjectPlacementBrushElement _destinationDecorPaintBrushElement;
        #endregion

        #region Public Properties
        public DecorPaintObjectPlacementBrushElement DestinationDecorPaintBrushElement { get { return _destinationDecorPaintBrushElement; } set { _destinationDecorPaintBrushElement = value; } }
        #endregion

        #region Public Static Functions
        public static PrefabsToDecorPaintBrushEventHandler Get()
        {
            return Octave3DWorldBuilder.Instance.PrefabsToDecorPaintBrushEventHandler;
        }
        #endregion

        #region Protected Methods
        protected override void PerformDrop()
        {
            if (_destinationDecorPaintBrushElement == null) return;

            List<GameObject> validUnityPrefabsInvolvedInDropOperation = PrefabValidator.GetValidPrefabsFromEntityCollection(DragAndDrop.objectReferences, false);
            if (validUnityPrefabsInvolvedInDropOperation.Count != 0) PerformDropUsingFirstPrefabInValidUnityPrefabCollection(validUnityPrefabsInvolvedInDropOperation);
        }
        #endregion

        #region Private Methods
        private void PerformDropUsingFirstPrefabInValidUnityPrefabCollection(List<GameObject> validUnityPrefabs)
        {
            GameObject firstValidUnityPrefab = GetFirstUnityPrefabFromValidUnityPrefabsCollection(validUnityPrefabs);
            PrefabCategory categoryWhichContainsPrefab = PrefabCategoryDatabase.Get().GetPrefabCategoryWhichContainsPrefab(firstValidUnityPrefab);

            if (categoryWhichContainsPrefab != null) AssignPrefabToDestinationBrushElement(categoryWhichContainsPrefab.GetPrefabByUnityPrefab(firstValidUnityPrefab));
            else
            {
                Prefab firstValidPrefab = GetFirstPrefabFromValidPrefabsCollection(PrefabFactory.Create(validUnityPrefabs));
                CreatePrefabToCategoryAssociationAndAssignPrefabToDestinationBrushElement(firstValidPrefab);
            }
        }

        private GameObject GetFirstUnityPrefabFromValidUnityPrefabsCollection(List<GameObject> validUnityPrefabs)
        {
            return validUnityPrefabs[0];
        }

        private Prefab GetFirstPrefabFromValidPrefabsCollection(List<Prefab> validPrefabs)
        {
            return validPrefabs[0];
        }

        private void CreatePrefabToCategoryAssociationAndAssignPrefabToDestinationBrushElement(Prefab prefab)
        {
            PrefabWithPrefabCategoryAssociationQueue.Instance.Enqueue(PrefabWithPrefabCategoryAssociationFactory.Create(prefab, _destinationDecorPaintBrushElement.ParentBrush.DestinationCategoryForElementPrefabs));
            AssignPrefabToDestinationBrushElement(prefab);
        }

        private void AssignPrefabToDestinationBrushElement(Prefab prefab)
        {
            UndoEx.RecordForToolAction(_destinationDecorPaintBrushElement);
            _destinationDecorPaintBrushElement.Prefab = prefab;
        }
        #endregion
    }
}
#endif