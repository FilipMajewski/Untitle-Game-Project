#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public static class ObjectSelectionActions
    {
        #region Public Static Functions
        public static void DuplicateSelection()
        {
            if (ObjectSelection.Get().NumberOfSelectedObjects == 0) return;

            ObjectSelection objectSelection = ObjectSelection.Get();
            List<GameObject> allSelectedObjects = objectSelection.GetAllSelectedGameObjects();
            List<GameObject> selectedParents = GameObjectExtensions.GetTopParentsFromGameObjectCollection(allSelectedObjects);

            var clonedObjects = new List<GameObject>();
            foreach(GameObject parent in selectedParents)
            {
                GameObject clonedParent = parent.Clone();
                clonedParent.transform.parent = parent.transform.parent;
                clonedObjects.Add(clonedParent);
            }

            if(clonedObjects.Count != 0)
            {
                objectSelection.Clear();
                objectSelection.AddGameObjectCollectionToSelection(clonedObjects);
                objectSelection.ObjectSelectionTransformGizmoSystem.OnObjectSelectionUpdated();
            }
        }

        public static void InvertSelection()
        {
            List<GameObject> allWorkingObjects = Octave3DWorldBuilder.Instance.GetAllWorkingObjects();
            List<GameObject> objectsToSelect = new List<GameObject>();
            ObjectSelection objectSelection = ObjectSelection.Get();

            foreach(var gameObject in allWorkingObjects)
            {
                if (!objectSelection.IsGameObjectSelected(gameObject)) objectsToSelect.Add(gameObject);
            }

            objectSelection.Clear();
            objectSelection.AddGameObjectCollectionToSelection(objectsToSelect);
            objectSelection.ObjectSelectionTransformGizmoSystem.OnObjectSelectionUpdated();
        }

        public static List<GameObject> ReplaceSelectedObjectsPrefabOnMouseClick()
        {
            MouseCursorRayHit cursorRayHit = ObjectSelection.Get().GetObjectPickedByCursor();
            if (cursorRayHit.WasAnObjectHit && !ObjectSelection.Get().IsGameObjectSelected(cursorRayHit.ClosestObjectRayHit.HitObject))
            {
                GameObject hitObject = cursorRayHit.ClosestObjectRayHit.HitObject;
                hitObject = Octave3DWorldBuilder.Instance.GetFirstParentWhisIsChildOfAPivot(hitObject);
                if (hitObject == null) return new List<GameObject>();

                GameObject newPrefabForSelectedObjects = hitObject.GetSourcePrefab();
                if (newPrefabForSelectedObjects == null) return new List<GameObject>();

                List<GameObject> allSelectedObjects = ObjectSelection.Get().GetAllSelectedGameObjects();
                ObjectSelection.Get().RemoveGameObjectCollectionFromSelection(allSelectedObjects);
                List<GameObject> newObjects = ObjectActions.ReplaceGameObjectHierarchyCollectionPrefab(allSelectedObjects, newPrefabForSelectedObjects);
                newObjects.RemoveAll(item => item == null);

                return newObjects;
            }

            return new List<GameObject>();
        }

        public static void SelectAllObjectsWithSamePrefabAsCurrentSelection()
        {
            ObjectSelection objectSelection = ObjectSelection.Get();
            List<GameObject> allSelectedObjects = objectSelection.GetAllSelectedGameObjects();
            Dictionary<GameObject, GameObject> prefabToObjectMappings = ObjectQueries.GetPrefabToObjectConnectionMappings(allSelectedObjects);

            List<GameObject> workingObjects = Octave3DWorldBuilder.Instance.GetAllWorkingObjects();
            foreach(GameObject gameObject in workingObjects)
            {
                GameObject sourcePrefab = gameObject.GetSourcePrefab();
                if(sourcePrefab != null)
                {
                    if (prefabToObjectMappings.ContainsKey(sourcePrefab)) objectSelection.AddGameObjectToSelection(gameObject);
                }
            }
        }

        public static void SelectAllObjectsWithSamePrefabAsObject(GameObject gameObject)
        {
            GameObject sourcePrefab = gameObject.GetSourcePrefab();
            if (sourcePrefab == null) return;

            ObjectSelection objectSelection = ObjectSelection.Get();
            List<GameObject> workingObjects = Octave3DWorldBuilder.Instance.GetAllWorkingObjects();

            foreach (GameObject workingObject in workingObjects)
            {
                GameObject workingObjectSourcePrefab = workingObject.GetSourcePrefab();
                if (workingObjectSourcePrefab != null && workingObjectSourcePrefab == sourcePrefab) objectSelection.AddGameObjectToSelection(workingObject);
            }
        }

        public static void AssignSelectedObjectsToLayer(int objectLayer)
        {
            List<GameObject> allSelectedGameObjects = ObjectSelection.Get().GetAllSelectedGameObjects();
            ObjectLayerDatabase.Get().AssignObjectsToLayer(allSelectedGameObjects, objectLayer);
        }

        public static void SelectAllObjectsInActiveLayer()
        {
            SelectAllObjectsInLayer(ObjectLayerDatabase.Get().ActiveLayer);
        }

        public static void SelectAllObjectsInAllLayers()
        {
            ObjectSelection objectSelection = ObjectSelection.Get();
            objectSelection.Clear();

            List<int> allLayers = ObjectLayerDatabase.Get().GetAllObjectLayers();
            foreach (int objectLayer in allLayers)
            {
                objectSelection.AddGameObjectCollectionToSelection(ObjectLayerDatabase.Get().GetAllGameObjectsInLayer(objectLayer));
            }
            ObjectSelection.Get().ObjectSelectionTransformGizmoSystem.OnObjectSelectionUpdated();
        }

        public static void SelectAllObjectsInLayer(int objectLayer)
        {
            List<GameObject> allObjectsInLayer = ObjectLayerDatabase.Get().GetAllGameObjectsInLayer(objectLayer);

            ObjectSelection objectSelection = ObjectSelection.Get();
            objectSelection.Clear();
            objectSelection.AddGameObjectCollectionToSelection(allObjectsInLayer);
            ObjectSelection.Get().ObjectSelectionTransformGizmoSystem.OnObjectSelectionUpdated();
        }

        public static void DeselectAllObjectsInActiveLayer()
        {
            DeselectAllObjectsInLayer(ObjectLayerDatabase.Get().ActiveLayer);
        }

        public static void DeselectAllObjectsInAllLayers()
        {
            List<int> allLayers = ObjectLayerDatabase.Get().GetAllObjectLayers();
            foreach (int objectLayer in allLayers)
            {
                DeselectAllObjectsInLayer(objectLayer);
            }
        }

        public static void AppendAllObjectsInActiveLayerToSelection()
        {
            AppendAllObjectsInLayerToSelection(ObjectLayerDatabase.Get().ActiveLayer);
        }

        public static void AppendAllObjectsInLayerToSelection(int objectLayer)
        {
            List<GameObject> allObjectsInLayer = ObjectLayerDatabase.Get().GetAllGameObjectsInLayer(objectLayer);

            ObjectSelection objectSelection = ObjectSelection.Get();
            objectSelection.AddGameObjectCollectionToSelection(allObjectsInLayer);
            ObjectSelection.Get().ObjectSelectionTransformGizmoSystem.OnObjectSelectionUpdated();
        }

        public static void DeselectAllObjectsInLayer(int objectLayer)
        {
            List<GameObject> allObjectsInLayer = ObjectLayerDatabase.Get().GetAllGameObjectsInLayer(objectLayer);
            ObjectSelection.Get().RemoveGameObjectCollectionFromSelection(allObjectsInLayer);
            ObjectSelection.Get().ObjectSelectionTransformGizmoSystem.OnObjectSelectionUpdated();
        }
        #endregion
    }
}
#endif