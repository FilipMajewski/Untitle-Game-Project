#if UNITY_EDITOR
using UnityEngine;
#if UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif
using UnityEditor;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [ExecuteInEditMode]
    public class Octave3DWorldBuilder : MonoBehaviour
    {
        #region Private Static Variables
        private static Octave3DWorldBuilder _instance = null;
        #endregion

        #region Private Variables
        [SerializeField]
        private Transform _transform;

        [SerializeField]
        private bool _showGUIHints = true;

        [SerializeField]
        private Octave3DScene _octave3DScene = new Octave3DScene();
        private SceneRenderer _sceneRenderer = new SceneRenderer();
        private ToolSupervisor _toolSupervisor = new ToolSupervisor();
        private ToolResources _toolResources = new ToolResources();

        [SerializeField]
        private ScriptableObjectPool _scriptableObjectPool;
        [SerializeField]
        private EditorWindowPool _editorWindowPool;

        [SerializeField]
        private PrefabCategoryDatabase _prefabCategoryDatabase;
        [SerializeField]
        private ObjectLayerDatabase _objectLayerDatabase;
        [SerializeField]
        private PrefabTagDatabase _prefabTagDatabase;
        [SerializeField]
        private ObjectPlacementPathHeightPatternDatabase _objectPlacementPathHeightPatternDatabase;
        [SerializeField]
        private ObjectPlacementPathTileConnectionConfigurationDatabase _objectPlacementPathTileConnectionConfigurationDatabase;
        [SerializeField]
        private DecorPaintObjectPlacementBrushDatabase _decorPaintObjectPlacementBrushDatabase;
        [SerializeField]
        private ObjectGroupDatabase _placementObjectGroupDatabase;

        [SerializeField]
        private ObjectPlacement _objectPlacement;
        [SerializeField]
        private ObjectSnapping _objectSnapping;
        [SerializeField]
        private ObjectEraser _objectEraser;
        [SerializeField]
        private ObjectSelection _objectSelection;

        [SerializeField]
        private MeshCombiner _meshCombiner = new MeshCombiner();

        [SerializeField]
        private PrefabsToCategoryDropEventHandler _prefabsToCategoryDropEventHandler = new PrefabsToCategoryDropEventHandler();
        [SerializeField]
        private PrefabsToPathTileConectionDropEventHandler _prefabsToPathTileConnectionDropEventHandler = new PrefabsToPathTileConectionDropEventHandler();
        [SerializeField]
        private PrefabsToDecorPaintBrushEventHandler _prefabsToDecorPaintBrushEventHandler = new PrefabsToDecorPaintBrushEventHandler();
        [SerializeField]
        private FolderToPrefabCreationFolderField _folderToPrefabCreationFolderField = new FolderToPrefabCreationFolderField();

        [SerializeField]
        private Inspector _inspector;
        #endregion

        #region Private Properties
        private ScriptableObjectPool ScriptableObjectPool
        {
            get
            {
                if(_scriptableObjectPool == null)
                {
                    _scriptableObjectPool = FindObjectOfType<ScriptableObjectPool>();
                    if (_scriptableObjectPool == null) CreateScriptableObjectPool();
                }

                return _scriptableObjectPool;
            }
        }

        private EditorWindowPool EditorWindowPool
        {
            get
            {
                if(_editorWindowPool == null)
                {
                    _editorWindowPool = FindObjectOfType<EditorWindowPool>();
                    if (_editorWindowPool == null) CreateEditorWindowPool();
                }

                return _editorWindowPool;
            }
        }
        #endregion

        #region Public Properties
        public bool ShowGUIHints { get { return _showGUIHints; } set { _showGUIHints = value; } }
        public Octave3DScene Octave3DScene { get { return _octave3DScene; } }
        public SceneRenderer SceneRenderer { get { return _sceneRenderer; } }
        public ToolSupervisor ToolSupervisor { get { return _toolSupervisor; } }
        public ToolResources ToolResources { get { return _toolResources; } }
        public PrefabCategoryDatabase PrefabCategoryDatabase
        {
            get
            {
                if (_prefabCategoryDatabase == null) _prefabCategoryDatabase = Octave3DWorldBuilder.Instance.CreateScriptableObject<PrefabCategoryDatabase>();
                return _prefabCategoryDatabase;
            }
        }
        public ObjectLayerDatabase ObjectLayerDatabase
        {
            get
            {
                if (_objectLayerDatabase == null) _objectLayerDatabase = Octave3DWorldBuilder.Instance.CreateScriptableObject<ObjectLayerDatabase>();
                return _objectLayerDatabase;
            }
        }
        public PrefabTagDatabase PrefabTagDatabase
        {
            get
            {
                if (_prefabTagDatabase == null) _prefabTagDatabase = Octave3DWorldBuilder.Instance.CreateScriptableObject<PrefabTagDatabase>();
                return _prefabTagDatabase;
            }
        }
        public ObjectPlacementPathHeightPatternDatabase ObjectPlacementPathHeightPatternDatabase
        {
            get
            {
                if (_objectPlacementPathHeightPatternDatabase == null) _objectPlacementPathHeightPatternDatabase = Octave3DWorldBuilder.Instance.CreateScriptableObject<ObjectPlacementPathHeightPatternDatabase>();
                return _objectPlacementPathHeightPatternDatabase;
            }
        }
        public ObjectPlacementPathTileConnectionConfigurationDatabase ObjectPlacementPathTileConnectionConfigurationDatabase
        {
            get
            {
                if (_objectPlacementPathTileConnectionConfigurationDatabase == null) _objectPlacementPathTileConnectionConfigurationDatabase = Octave3DWorldBuilder.Instance.CreateScriptableObject<ObjectPlacementPathTileConnectionConfigurationDatabase>();
                return _objectPlacementPathTileConnectionConfigurationDatabase;
            }
        }
        public DecorPaintObjectPlacementBrushDatabase DecorPaintObjectPlacementBrushDatabase
        {
            get
            {
                if (_decorPaintObjectPlacementBrushDatabase == null) _decorPaintObjectPlacementBrushDatabase = Octave3DWorldBuilder.Instance.CreateScriptableObject<DecorPaintObjectPlacementBrushDatabase>();
                return _decorPaintObjectPlacementBrushDatabase;
            }
        }
        public ObjectGroupDatabase PlacementObjectGroupDatabase
        {
            get
            {
                if (_placementObjectGroupDatabase == null) _placementObjectGroupDatabase = Octave3DWorldBuilder.Instance.CreateScriptableObject<ObjectGroupDatabase>();
                return _placementObjectGroupDatabase;
            }
        }

        public ObjectPlacement ObjectPlacement
        {
            get
            {
                if (_objectPlacement == null) _objectPlacement = Octave3DWorldBuilder.Instance.CreateScriptableObject<ObjectPlacement>();
                return _objectPlacement;
            }
        }
        public ObjectSnapping ObjectSnapping
        {
            get
            {
                if (_objectSnapping == null) _objectSnapping = Octave3DWorldBuilder.Instance.CreateScriptableObject<ObjectSnapping>();
                return _objectSnapping;
            }
        }
        public ObjectEraser ObjectEraser
        {
            get
            {
                if (_objectEraser == null) _objectEraser = Octave3DWorldBuilder.Instance.CreateScriptableObject<ObjectEraser>();
                return _objectEraser;
            }
        }
        public ObjectSelection ObjectSelection
        {
            get
            {
                if (_objectSelection == null) _objectSelection = Octave3DWorldBuilder.Instance.CreateScriptableObject<ObjectSelection>();
                return _objectSelection;
            }
        }

        public MeshCombiner MeshCombiner { get { return _meshCombiner; } }

        public PrefabsToCategoryDropEventHandler PrefabsToCategoryDropEventHandler { get { return _prefabsToCategoryDropEventHandler; } }
        public PrefabsToPathTileConectionDropEventHandler PrefabsToPathTileConectionDropEventHandler { get { return _prefabsToPathTileConnectionDropEventHandler; } }
        public PrefabsToDecorPaintBrushEventHandler PrefabsToDecorPaintBrushEventHandler { get { return _prefabsToDecorPaintBrushEventHandler; } }
        public FolderToPrefabCreationFolderField FolderToPrefabCreationFolderField { get { return _folderToPrefabCreationFolderField; } }

        public PrefabsToCategoryDropSettingsWindow PrefabsToCategoryDropSettingsWindow { get { return EditorWindowPool.PrefabsToCategoryDropSettingsWindow; } }
        public ObjectPlacementSettingsWindow ObjectPlacementSettingsWindow { get { return EditorWindowPool.ObjectPlacementSettingsWindow; } }
        public PrefabManagementWindow PrefabManagementWindow { get { return EditorWindowPool.PrefabManagementWindow; } }
        public ActivePrefabCategoryView ActivePrefabCategoryView { get { return EditorWindowPool.ActivePrefabCategoryView; } }
        public ActivePrefabView ActivePrefabView { get { return EditorWindowPool.ActivePrefabView; } }

        public PrefabTagsWindow PrefabTagsWindow { get { return EditorWindowPool.PrefabTagsWindow; } }
        public ObjectLayersWindow ObjectLayersWindow { get { return EditorWindowPool.ObjectLayersWindow; } }
        public Inspector Inspector
        {
            get
            {
                if (_inspector == null)
                {
                    _inspector = Octave3DWorldBuilder.Instance.CreateScriptableObject<Inspector>();
                    _inspector.Initialize();
                }
                return _inspector;
            }
        }
        #endregion

        #region Public Static Properties
        public static Octave3DWorldBuilder Instance 
        { 
            get 
            { 
                if(_instance == null && Application.isPlaying)
                {
                    _instance = FindObjectOfType<Octave3DWorldBuilder>();
                }
                return _instance; 
            } 
        }
        #endregion

        #region Public Methods
        public T CreateScriptableObject<T>() where T : ScriptableObject
        {
            return ScriptableObjectPool.CreateScriptableObject<T>();
        }

        public void RemoveNullScriptableObjects()
        {
            ScriptableObjectPool.RemoveNullEntries();
        }

        public void ShowGUIHint(string hint)
        {
            if(_showGUIHints) EditorGUILayout.HelpBox(hint, UnityEditor.MessageType.Info);
        }

        public bool ContainsChild(Transform potentialChild)
        {
            return potentialChild.IsChildOf(_transform);
        }

        public bool ContainsImmediateChild(Transform potentialChild)
        {
            return potentialChild.parent == _transform;
        }

        public List<GameObject> GetAllChildrenExcludingPlacementGuide()
        {
            List<GameObject> allChildObjects = gameObject.GetAllChildren();
            if (ObjectPlacementGuide.ExistsInScene) allChildObjects.RemoveAll(item => ObjectPlacementGuide.Equals(item) || ObjectPlacementGuide.ContainsChild(item.transform));

            return allChildObjects;
        }

        public List<GameObject> GetImmediateChildrenExcludingPlacementGuide()
        {
            List<GameObject> immediateChildren = gameObject.GetImmediateChildren();
            if (ObjectPlacementGuide.ExistsInScene) immediateChildren.RemoveAll(item => ObjectPlacementGuide.Equals(item) || ObjectPlacementGuide.ContainsChild(item.transform));

            return immediateChildren;
        }

        public List<GameObject> GetAllWorkingObjects()
        {
            List<GameObject> allWorkingObjects = GetAllChildrenExcludingPlacementGuide();
            allWorkingObjects.RemoveAll(item => IsPivotWorkingObject(item));

            return allWorkingObjects;
        }

        public List<GameObject> GetImmediateWorkingObjects()
        {
            List<GameObject> allImmediateWorkingObjects = GetImmediateChildrenExcludingPlacementGuide();
            allImmediateWorkingObjects.RemoveAll(item => IsPivotWorkingObject(item));

            return allImmediateWorkingObjects;
        }

        public bool IsWorkingObject(GameObject gameObject)
        {
            if (!ContainsChild(gameObject.transform)) return false;
            if (ObjectQueries.IsGameObjectPartOfPlacementGuideHierarchy(gameObject)) return false;

            return true;
        }

        public GameObject GetFirstParentWhisIsChildOfAPivot(GameObject workingObject)
        {
            if (!IsWorkingObject(workingObject)) return null;
            return workingObject.GetParentWhichIsChildOfParentByPredicate(item => IsPivotWorkingObject(item));
        }

        public List<GameObject> GetParentsChildrenOfPivots(List<GameObject> gameObjects)
        {
            if (gameObjects.Count == 0) return new List<GameObject>();

            var parents = new List<GameObject>(gameObjects.Count);
            foreach (GameObject gameObject in gameObjects)
            {
                GameObject firstParentChildOfPivot = Octave3DWorldBuilder.Instance.GetFirstParentWhisIsChildOfAPivot(gameObject);
                parents.Add(firstParentChildOfPivot);
            }

            return parents;
        }

        public bool IsPivotWorkingObject(GameObject workingObject)
        {
            if (!IsWorkingObject(workingObject)) return false;

            List<Type> pivotWorkingObjectTypes = GetPivotWorkingObjectTypes();
            return workingObject.HasComponentsOfAnyType(pivotWorkingObjectTypes) || PlacementObjectGroupDatabase.ContainsObjectGroup(workingObject);
        }

        public List<Type> GetPivotWorkingObjectTypes()
        {
            return new List<Type> { typeof(Octave3DWorldBuilder), typeof(Terrain) };
        }

        public void HideWireframeForAllWorkingObjectsAndPlacementGuide()
        {
            List<GameObject> topMostWorkingObjects = GetImmediateWorkingObjects();
            GameObjectExtensions.SetSelectedHierarchyWireframeHidden(topMostWorkingObjects, true);

            if (ObjectPlacementGuide.ExistsInScene) ObjectPlacementGuide.SceneObject.SetSelectedHierarchyWireframeHidden(true);
            SceneView.RepaintAll();
        }
        #endregion

        #region Private Static Functions
        [MenuItem("Tools/Octave3D/Deselect &D")]
        private static void DeselectObjectGrid3D()
        {
            if (Octave3DWorldBuilder.Instance != null)
            {
                Octave3DWorldBuilder.Instance.DeselectInSceneView();
            }
        }

        [MenuItem("Tools/Octave3D/Select &R")]
        private static void SelectObjectGrid3D()
        {
            if (Octave3DWorldBuilder.Instance != null)
            {
                Octave3DWorldBuilder.Instance.SelectInSceneView();
            }
        }
        #endregion

        #region Private methods
        private void Awake()
        {
            _transform = transform;
        }

        private void Start()
        {
            ToolWasStartedMessage.SendToInterestedListeners();
        }

        private void OnEnable()
        {
            if (!DoesSingletonExist()) SetAsSingleton();
            else if (LogMessageAndDestroyIfNotSameAsSingleton()) return;

            #if UNITY_5_4_OR_NEWER
            UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);
            #else
            UnityEngine.Random.seed = System.DateTime.Now.Millisecond;
            #endif
            ToolWasEnabledMessage.SendToInterestedListeners();

            HideWireframeForAllWorkingObjectsAndPlacementGuide();

            EditorApplication.update -= EditorUpdate;
            EditorApplication.update += EditorUpdate;
        }

        private bool LogMessageAndDestroyIfNotSameAsSingleton()
        {
            if (!IsSameAsSingleton())
            {
                Debug.LogWarning("Only one instance of the \'Octave3DWorldBuilder\' Monobehaviour can exist in the scene.");
                DestroyImmediate(this);
                return true;
            }

            return false;
        }

        private bool DoesSingletonExist()
        {
            return _instance != null;
        }
        
        private void SetAsSingleton()
        {
            _instance = this;
        }

        private bool IsSameAsSingleton()
        {
            return _instance == this;
        }

        private void Reset()
        {
            _transform = transform;
            MessageListenerDatabase.Instance.Clear();  // Note: This ensures that no scriptable object references are left behind.
            ScriptableObjectPool.DestroyAllScriptableObjects();
            EditorWindowPool.DestroyAllWindows();
            ToolWasResetMessage.SendToInterestedListeners();

            Octave3DScene.Get().RegisterObjectHierarchies(GetImmediateChildrenExcludingPlacementGuide());
        }

        private void OnDestroy()
        {
            if (IsSameAsSingleton())
            {
                MessageListenerDatabase.Instance.Clear();
                ToolResources.DisposeResources();

                //EditorWindowPool.DestroyAllWindows();
                GameObject poolObject = EditorWindowPool.gameObject;
                DestroyImmediate(EditorWindowPool);
                DestroyImmediate(poolObject);

                poolObject = ScriptableObjectPool.gameObject;
                DestroyImmediate(ScriptableObjectPool);
                DestroyImmediate(poolObject);

                _instance = null;
            }
        }

        private void OnDrawGizmosSelected()
        {
            if(_instance != null)
            {
                _sceneRenderer.RenderGizmos();
                //_octave3DScene.RenderGizmosDebug();
            }
        }

        private void Update()
        {
            if (_instance != null)
            {
                EnsureTransformDataIsCorrect();
                _octave3DScene.Update();
            }
        }

        private void EnsureTransformDataIsCorrect()
        {
            EnsureHasNoParent();
            EnsurePositionIsZero();
            EnsureRotationIsIdentity();
            EnsureScaleIsOne();
        }

        private void EnsureHasNoParent()
        {
            if (_transform.parent != null) _transform.parent = null;
        }

        private void EnsurePositionIsZero()
        {
            if (_transform.position != Vector3.zero) _transform.position = Vector3.zero;
        }

        private void EnsureRotationIsIdentity()
        {
            if (_transform.rotation.eulerAngles != Vector3.zero) _transform.rotation = Quaternion.identity;
        }

        private void EnsureScaleIsOne()
        {
            if (_transform.localScale != Vector3.one) _transform.localScale = Vector3.one;
        }

        private void CreateScriptableObjectPool()
        {
            GameObject poolObject = new GameObject("Scriptable Object Pool");
            UndoEx.RegisterCreatedGameObject(poolObject);       // To allow Undo/Redo when attaching tool script, and then Undo.
            _scriptableObjectPool = UndoEx.AddComponent<ScriptableObjectPool>(poolObject);
            poolObject.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
        }

        private void CreateEditorWindowPool()
        {
            GameObject poolObject = new GameObject("Editor Window Pool");
            UndoEx.RegisterCreatedGameObject(poolObject);       // To allow Undo/Redo when attaching tool script, and then Undo.
            _editorWindowPool = UndoEx.AddComponent<EditorWindowPool>(poolObject);
            poolObject.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
        }

        private void SelectInSceneView()
        {
            List<UnityEngine.Object> selectedObjects = new List<UnityEngine.Object>();
            selectedObjects.Add(Octave3DWorldBuilder.Instance.gameObject);
            Selection.objects = selectedObjects.ToArray();
        }

        private void DeselectInSceneView()
        {
            List<UnityEngine.Object> selectedObjects = new List<UnityEngine.Object>(Selection.objects);
            selectedObjects.Remove(Octave3DWorldBuilder.Instance.gameObject);
            Selection.objects = selectedObjects.ToArray();
        }

        private void EditorUpdate()
        {
            #if UNITY_5_3_OR_NEWER
            if(!Application.isPlaying && Octave3DWorldBuilder.Instance != null)
            {
                if (ScriptableObjectPool.gameObject.scene != Octave3DWorldBuilder.Instance.gameObject.scene) SceneManager.MoveGameObjectToScene(ScriptableObjectPool.gameObject, Octave3DWorldBuilder.Instance.gameObject.scene);
                if (EditorWindowPool.gameObject.scene != Octave3DWorldBuilder.Instance.gameObject.scene) SceneManager.MoveGameObjectToScene(EditorWindowPool.gameObject, Octave3DWorldBuilder.Instance.gameObject.scene);
            }
            #endif
        }
        #endregion
    } 
}
#endif