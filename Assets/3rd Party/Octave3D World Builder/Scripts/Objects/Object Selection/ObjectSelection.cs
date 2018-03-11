#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class ObjectSelection : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private SerializableGameObjectHashSet _selectedObjects = new SerializableGameObjectHashSet();

        [SerializeField]
        private InteractableMirror _mirror;

        [SerializeField]
        private ObjectSelectionSettings _settings;
        [SerializeField]
        private ObjectSelectionPrefabCreationSettings _prefabCreationSettings;
        [SerializeField]
        private ObjectSelectionRenderSettings _renderSettings;

        [SerializeField]
        private ObjectSelectionShape _selectionShape = new ObjectSelectionShape();
        [SerializeField]
        private ObjectSelectionTransformGizmoSystem _objectSelectionTransformGizmoSystem;

        [SerializeField]
        private GameObject _firstSelectedGameObject;
        [SerializeField]
        private GameObject _lastSelectedGameObject;

        [SerializeField]
        private bool _wasInitialized = false;
        #endregion

        #region Private Properties
        private ObjectSelectionShape SelectionShape { get { return _selectionShape; } }
        #endregion

        #region Public Properties
        public InteractableMirror Mirror
        {
            get
            {
                if (_mirror == null) _mirror = Octave3DWorldBuilder.Instance.CreateScriptableObject<InteractableMirror>();
                return _mirror;
            }
        }
        public InteractableMirrorView MirrorView { get { return Mirror.View; } }
        public InteractableMirrorSettings MirrorSettings { get { return Mirror.Settings; } }
        public InteractableMirrorRenderSettings MirrorRenderSettings { get { return Mirror.RenderSettings; } }
        public ObjectSelectionTransformGizmoSystem ObjectSelectionTransformGizmoSystem
        {
            get
            {
                if (_objectSelectionTransformGizmoSystem == null) _objectSelectionTransformGizmoSystem = Octave3DWorldBuilder.Instance.CreateScriptableObject<ObjectSelectionTransformGizmoSystem>();
                return _objectSelectionTransformGizmoSystem;
            }
        }
        public int NumberOfSelectedObjects { get { return _selectedObjects.Count; } }
        public ObjectSelectionSettings Settings
        {
            get
            {
                if (_settings == null) _settings = Octave3DWorldBuilder.Instance.CreateScriptableObject<ObjectSelectionSettings>();
                return _settings;
            }
        }
        public ObjectSelectionPrefabCreationSettings PrefabCreationSettings
        {
            get
            {
                if (_prefabCreationSettings == null) _prefabCreationSettings = Octave3DWorldBuilder.Instance.CreateScriptableObject<ObjectSelectionPrefabCreationSettings>();
                return _prefabCreationSettings;
            }
        }
        public ObjectSelectionRenderSettings RenderSettings
        {
            get
            {
                if (_renderSettings == null) _renderSettings = Octave3DWorldBuilder.Instance.CreateScriptableObject<ObjectSelectionRenderSettings>();
                return _renderSettings;
            }
        }
        public RectangleShapeRenderSettings RectangleSelectionShapeRenderSettings { get { return SelectionShape.RectangleShapeRenderSettings; } }
        public EllipseShapeRenderSettings EllipseSelectionShapeRenderSettings { get { return SelectionShape.EllipseShapeRenderSettings; } }
        #endregion

        #region Public Static Functions
        public static ObjectSelection Get()
        {
            return Octave3DWorldBuilder.Instance.ObjectSelection;
        }
        #endregion

        #region Public Methods
        public void RenderGizmos()
        {
            SelectionShape.RenderGizmos();

            IObjectSelectionRenderer objectSelectionRenderer = ObjectSelectionRendererFactory.Create(RenderSettings.RenderMode);
            objectSelectionRenderer.Render(GetAllSelectedGameObjects());

            if (Mirror.IsActive)
            {
                Mirror.RenderGizmos();

                List<GameObject> topLevelParentsInSelection = GameObjectExtensions.GetTopParentsFromGameObjectCollection(_selectedObjects.HashSet);
                Mirror.RenderMirroredEntityOrientedBoxes(GameObjectExtensions.GetHierarchyWorldOrientedBoxes(topLevelParentsInSelection));
            }
        }

        public void RenderHandles()
        {
            ObjectSelectionTransformGizmoSystem.RenderHandles(_selectedObjects.HashSet);
        }

        public GameObject GetFirstSelectedGameObject()
        {
            if(_firstSelectedGameObject == null)
            {
                var selectedObjectsList = new List<GameObject>(_selectedObjects.HashSet);
                if (selectedObjectsList.Count != 0) _firstSelectedGameObject = selectedObjectsList[0];
            }

            return _firstSelectedGameObject;
        }

        public GameObject GetLastSelectedGameObject()
        {
            if (_lastSelectedGameObject == null)
            {
                var selectedObjectsList = new List<GameObject>(_selectedObjects.HashSet);
                if (selectedObjectsList.Count != 0) _lastSelectedGameObject = selectedObjectsList[selectedObjectsList.Count - 1];
            }

            return _lastSelectedGameObject;
        }

        public Vector3 GetWorldCenter()
        {
            if (NumberOfSelectedObjects == 0) return Vector3.zero;
            else
            {
                Vector3 objectCenterSum = Vector3.zero;
                foreach (GameObject selectedObject in _selectedObjects.HashSet)
                {
                    OrientedBox worldOrientedBox = selectedObject.GetWorldOrientedBox();
                    if (worldOrientedBox.IsValid()) objectCenterSum += worldOrientedBox.Center;
                    else objectCenterSum += selectedObject.transform.position;
                }

                return objectCenterSum / NumberOfSelectedObjects;
            }
        }

        public void Clear()
        {
            _selectedObjects.Clear();
            _firstSelectedGameObject = null;
            _lastSelectedGameObject = null;

            SceneView.RepaintAll();
        }

        public void AddGameObjectToSelection(GameObject gameObject)
        {
            if(CanGameObjectBeSelected(gameObject))
            {
                _selectedObjects.Add(gameObject.gameObject);
                _lastSelectedGameObject = gameObject.gameObject;

                if (NumberOfSelectedObjects == 1) _firstSelectedGameObject = _lastSelectedGameObject;
            }
        }

        public void AddGameObjectCollectionToSelection(IEnumerable<GameObject> gameObjects)
        {
            foreach (GameObject gameObject in gameObjects)
            {
                AddGameObjectToSelection(gameObject);
            }
        }

        public void AddEntireGameObjectHierarchyToSelection(GameObject gameObjectInHierarchy)
        {
            if (!CanGameObjectBeSelected(gameObjectInHierarchy)) return;

            // Note: Collect the entire hierarchy of objects, but do not go past the first pivot object
            //       that we encounter (e.g. terrain or tool object etc).
            GameObject firstParentChildOfPivot = Octave3DWorldBuilder.Instance.GetFirstParentWhisIsChildOfAPivot(gameObjectInHierarchy);
            List<GameObject> allChildrenIncludingSelf = firstParentChildOfPivot.GetAllChildrenIncludingSelf();
            AddGameObjectCollectionToSelection(allChildrenIncludingSelf);
        }

        public void AddEntireGameObjectHierarchyToSelection(IEnumerable<GameObject> gameObjectsInDifferentHierarchies)
        {
            foreach (GameObject gameObject in gameObjectsInDifferentHierarchies)
            {
                AddEntireGameObjectHierarchyToSelection(gameObject);
            }
        }

        public void RemoveGameObjectFromSelection(GameObject gameObject)
        {
            _selectedObjects.Remove(gameObject);
            _firstSelectedGameObject = null;
            _lastSelectedGameObject = null;
        }

        public void RemoveGameObjectCollectionFromSelection(IEnumerable<GameObject> gameObjects)
        {
            foreach (GameObject gameObject in gameObjects)
            {
                RemoveGameObjectFromSelection(gameObject);
            }
        }

        public void RemoveEntireGameObjectHierarchyFromSelection(GameObject gameObject)
        {
            GameObject firstParentChildOfPivot = Octave3DWorldBuilder.Instance.GetFirstParentWhisIsChildOfAPivot(gameObject);
            List<GameObject> allChildrenIncludingSelf = firstParentChildOfPivot.GetAllChildrenIncludingSelf();
            RemoveGameObjectCollectionFromSelection(allChildrenIncludingSelf);
        }

        public void RemoveEntireGameObjectHierarchyFromSelection(IEnumerable<GameObject> gameObjectsInDifferentHierarchies)
        {
            foreach (GameObject gameObject in gameObjectsInDifferentHierarchies)
            {
                RemoveEntireGameObjectHierarchyFromSelection(gameObject);
            }
        }

        public bool IsGameObjectSelected(GameObject gameObject)
        {
            return _selectedObjects.Contains(gameObject);
        }

        public bool IsSameAs(GameObject gameObject)
        {
            return NumberOfSelectedObjects == 1 && IsGameObjectSelected(gameObject);
        }

        public bool IsSameAs(List<GameObject> gameObjects)
        {
            if(gameObjects.Count != NumberOfSelectedObjects) return false;

            foreach (GameObject gameObject in gameObjects)
            {
                if (!_selectedObjects.Contains(gameObject)) return false;
            }

            return true;
        }

        public void HandleRepaintEvent(Event e)
        {
            if (Mirror.IsInteractionSessionActive)
            {
                Mirror.HandleRepaintEvent(e);
                return;
            }
        }

        public void HandleMouseMoveEvent(Event e)
        {
            if (Mirror.IsInteractionSessionActive)
            {
                e.DisableInSceneView();
                Mirror.HandleMouseMoveEvent(e);
                return;
            }

            SelectionShape.HandleMouseMoveEvent(e);
        }

        public void HandleMouseDragEvent(Event e)
        {
            SelectionShape.HandleMouseDragEvent(e);

            var mouseDragSelectionUpdateOperation = ObjectSelectionUpdateOperationFactory.Create(ObjectSelectionUpdateOperationType.MouseDrag);
            mouseDragSelectionUpdateOperation.Perform();

            SceneView.RepaintAll();
        }

        public void HandleMouseButtonDownEvent(Event e)
        {
            if (Mirror.IsInteractionSessionActive && e.InvolvesLeftMouseButton())
            {
                e.DisableInSceneView();
                Mirror.EndInteractionSession();
                return;
            }

            if(e.InvolvesLeftMouseButton() && NumberOfSelectedObjects != 0 && 
              !SelectionShape.IsVisible() && AllShortcutCombos.Instance.ReplacePrefabsForSelectedObjects.IsActive())
            {
                e.DisableInSceneView();
                UndoEx.RecordForToolAction(this);
                List<GameObject> newObjects = ObjectSelectionActions.ReplaceSelectedObjectsPrefabOnMouseClick();
                if (newObjects.Count != 0) AddGameObjectCollectionToSelection(newObjects);

                return;
            }

            SelectionShape.HandleMouseButtonDownEvent(e);
            if (CanPerformClickSelectionUpdateOperation())
            {
                var clickSelectionUpdateOperation = ObjectSelectionUpdateOperationFactory.Create(ObjectSelectionUpdateOperationType.Click);
                clickSelectionUpdateOperation.Perform();

                SceneView.RepaintAll();
            }
        }

        public void HandleMouseButtonUpEvent(Event e)
        {
            SelectionShape.HandleMouseButtonUpEvent(e);
        }

        public void HandleExecuteCommandEvent(Event e)
        {
            if(e.IsDuplicateSelectionCommand())
            {
                e.DisableInSceneView();
                UndoEx.RecordForToolAction(this);
                ObjectSelectionActions.DuplicateSelection();
            }
        }

        public void HandleKeyboardButtonDownEvent(Event e)
        {
            // Note: Don't disable this event if it's CTRL or CMD because transform
            //       handle snapping will no longer work.
            if (e.keyCode != KeyCode.LeftControl && e.keyCode != KeyCode.LeftCommand &&
                e.keyCode != KeyCode.RightControl && e.keyCode != KeyCode.RightCommand) e.DisableInSceneView();

            if (Mirror.IsInteractionSessionActive)
            {
                Mirror.HandleKeyboardButtonDownEvent(e);
                return;
            }

            if(Mirror.IsActive && AllShortcutCombos.Instance.MirrorSelectedObjects.IsActive())
            {
                List<GameObject> topParentsInSelectedObjects = GameObjectExtensions.GetTopParentsFromGameObjectCollection(_selectedObjects.HashSet);
                ObjectHierarchyRootsWerePlacedInSceneMessage.SendToInterestedListeners(Mirror.MirrorGameObjectHierarchies(topParentsInSelectedObjects), ObjectHierarchyRootsWerePlacedInSceneMessage.PlacementType.MirroredSelection);
                return;
            }

            if(AllShortcutCombos.Instance.DeleteSelectedObjects.IsActive())
            {
                UndoEx.RecordForToolAction(this);
                ObjectActions.EraseAllSelectedGameObjects();
            }
            else
            if(AllShortcutCombos.Instance.SelectAllObjectsWithSamePrefabAsCurrentSelection.IsActive())
            {
                UndoEx.RecordForToolAction(this);
                ObjectSelectionActions.SelectAllObjectsWithSamePrefabAsCurrentSelection();
                _objectSelectionTransformGizmoSystem.OnObjectSelectionUpdated();
            }
            else
            if(AllShortcutCombos.Instance.ToggleGizmosOnOff.IsActive())
            {
                UndoEx.RecordForToolAction(_objectSelectionTransformGizmoSystem);
                _objectSelectionTransformGizmoSystem.AreGizmosActive = !_objectSelectionTransformGizmoSystem.AreGizmosActive;
                Octave3DWorldBuilder.Instance.Inspector.EditorWindow.Repaint();
            }
            else
            if(AllShortcutCombos.Instance.ActivateMoveGizmo.IsActive())
            {
                UndoEx.RecordForToolAction(_objectSelectionTransformGizmoSystem);
                _objectSelectionTransformGizmoSystem.ActiveGizmoType = TransformGizmoType.Move;
                Octave3DWorldBuilder.Instance.Inspector.EditorWindow.Repaint();
            }
            else
            if(AllShortcutCombos.Instance.ActivateRotationGizmo.IsActive())
            {
                UndoEx.RecordForToolAction(_objectSelectionTransformGizmoSystem);
                _objectSelectionTransformGizmoSystem.ActiveGizmoType = TransformGizmoType.Rotate;
                Octave3DWorldBuilder.Instance.Inspector.EditorWindow.Repaint();
            }
            else
            if (AllShortcutCombos.Instance.ActivateScaleGizmo.IsActive())
            {
                UndoEx.RecordForToolAction(_objectSelectionTransformGizmoSystem);
                _objectSelectionTransformGizmoSystem.ActiveGizmoType = TransformGizmoType.Scale;
                Octave3DWorldBuilder.Instance.Inspector.EditorWindow.Repaint();
            }
        }

        public void HandleMouseScrollWheelEvent(Event e)
        {
            if (CanAdjustSelectionShapeSizeForMouseScrollWheel())
            {
                e.DisableInSceneView();
                AdjustSelectionShapeSizeForMouseWheelScroll(e);
            }
        }

        public List<GameObject> GetAllSelectedGameObjects()
        {
            return new List<GameObject>(_selectedObjects.HashSet);
        }

        public List<GameObject> GetAllGameObjectsOverlappedBySelectionShape()
        {
            if (SelectionShape.IsVisible())
            {
                List<GameObject> overlappedObjects = SelectionShape.GetOverlappedGameObjects();
                overlappedObjects.RemoveAll(item => !CanGameObjectBeSelected(item));
    
                return overlappedObjects;
            }
            else return new List<GameObject>();
        }

        public void RemoveNullGameObjectEntries()
        {
            _selectedObjects.RemoveNullEntries();
        }

        public MouseCursorRayHit GetObjectPickedByCursor()
        {
            MouseCursor.Instance.PushObjectPickMaskFlags(MouseCursorObjectPickFlags.ObjectTerrain);
            MouseCursorRayHit cursorRayHit = MouseCursor.Instance.GetRayHit();
            MouseCursor.Instance.PopObjectPickMaskFlags();

            return cursorRayHit;
        }
        #endregion

        #region Private Methods
        private static bool CanGameObjectBeSelected(GameObject gameObject)
        {
            return ObjectQueries.CanGameObjectBeInteractedWith(gameObject);
        }

        private bool CanPerformClickSelectionUpdateOperation()
        {
            // Click operations can only be performed when the selection mode is not set to
            // 'Paint'. This is because the 'Paint' mode works better (i.e. behaves in a more
            // intuitive way) if we ignore clicks.
            return _settings.SelectionMode != ObjectSelectionMode.Paint;
        }

        private bool CanAdjustSelectionShapeSizeForMouseScrollWheel()
        {
            return _settings.SelectionMode == ObjectSelectionMode.Paint && SelectionShape.IsVisible() &&
                    AllShortcutCombos.Instance.EnableScrollWheelSizeAdjustmentForSelectionShape.IsActive();
        }

        private void AdjustSelectionShapeSizeForMouseWheelScroll(Event e)
        {
            ObjectSelectionPaintModeSettings paintModeSettings = _settings.PaintModeSettings;
            int sizeAdjustAmount = (int)(-e.delta.y * paintModeSettings.ScrollWheelShapeSizeAdjustmentSpeed);
            
            UndoEx.RecordForToolAction(this);
            paintModeSettings.SelectionShapeWidthInPixels += sizeAdjustAmount;
            paintModeSettings.SelectionShapeHeightInPixels += sizeAdjustAmount;

            SceneView.RepaintAll();
        }

        private void OnEnable()
        {
            if(!_wasInitialized)
            {
                _selectionShape.EllipseShapeRenderSettings.FillColor = new Color(0.0f, 1.0f, 0.0f, 0.2f);
                _selectionShape.EllipseShapeRenderSettings.BorderLineColor = Color.green;

                _selectionShape.RectangleShapeRenderSettings.FillColor = new Color(0.0f, 1.0f, 0.0f, 0.2f);
                _selectionShape.RectangleShapeRenderSettings.BorderLineColor = Color.green;
             
                _wasInitialized = true;
            }
        }
        #endregion
    }
}
#endif