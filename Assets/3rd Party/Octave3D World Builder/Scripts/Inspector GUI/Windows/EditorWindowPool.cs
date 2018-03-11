#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [ExecuteInEditMode]
    public class EditorWindowPool : MonoBehaviour
    {
        #region Private Variables
        [SerializeField]
        private PrefabManagementWindow _prefabManagementWindow;
        [SerializeField]
        private ObjectPlacementSettingsWindow _objectPlacementSettingsWindow;
        [SerializeField]
        private PrefabsToCategoryDropSettingsWindow _prefabsToCategoryDropSettingsWindow;
        [SerializeField]
        private PrefabTagsWindow _prefabTagsWindow;
        [SerializeField]
        private ObjectLayersWindow _objectLayersWindow;

        // Note: These would normally have to be associated with the Prefab Management Window. However,
        //       there seem to be some problems with serialization. It may be possible to solve those
        //       problems using 'EditorPrefs', but I would rather keep things simple and store these 
        //       variables here.
        [SerializeField]
        private ActivePrefabCategoryView _activePrefabCategoryView = new ActivePrefabCategoryView();
        [SerializeField]
        private ActivePrefabView _activePrefabView = new ActivePrefabView();
        #endregion

        #region Public Properties
        public ActivePrefabCategoryView ActivePrefabCategoryView { get { return _activePrefabCategoryView; } }
        public ActivePrefabView ActivePrefabView { get { return _activePrefabView; } }

        public PrefabManagementWindow PrefabManagementWindow
        {
            get
            {
                if (_prefabManagementWindow == null) _prefabManagementWindow = Octave3DEditorWindow.Create<PrefabManagementWindow>();
                return _prefabManagementWindow;
            }
        }
        public ObjectPlacementSettingsWindow ObjectPlacementSettingsWindow
        {
            get
            {
                if (_objectPlacementSettingsWindow == null) _objectPlacementSettingsWindow = Octave3DEditorWindow.Create<ObjectPlacementSettingsWindow>();
                return _objectPlacementSettingsWindow;
            }
        }
        public PrefabsToCategoryDropSettingsWindow PrefabsToCategoryDropSettingsWindow
        {
            get
            {
                if (_prefabsToCategoryDropSettingsWindow == null) _prefabsToCategoryDropSettingsWindow = Octave3DEditorWindow.Create<PrefabsToCategoryDropSettingsWindow>();
                return _prefabsToCategoryDropSettingsWindow;
            }
        }
        public PrefabTagsWindow PrefabTagsWindow
        {
            get
            {
                if (_prefabTagsWindow == null) _prefabTagsWindow = Octave3DEditorWindow.Create<PrefabTagsWindow>();
                return _prefabTagsWindow;
            }
        }
        public ObjectLayersWindow ObjectLayersWindow
        {
            get
            {
                if (_objectLayersWindow == null) _objectLayersWindow = Octave3DEditorWindow.Create<ObjectLayersWindow>();
                return _objectLayersWindow;
            }
        }
        #endregion

        #region Public Methods
        public void DestroyAllWindows()
        {
            if (_prefabManagementWindow != null) Octave3DEditorWindow.Destroy(_prefabManagementWindow);
            if (_prefabsToCategoryDropSettingsWindow != null) Octave3DEditorWindow.Destroy(_prefabsToCategoryDropSettingsWindow);
            if (_objectPlacementSettingsWindow != null) Octave3DEditorWindow.Destroy(_objectPlacementSettingsWindow);
            if (_prefabTagsWindow != null) Octave3DEditorWindow.Destroy(_prefabTagsWindow);
            if (_objectLayersWindow != null) Octave3DEditorWindow.Destroy(_objectLayersWindow);
        }

        public void RepaintAll()
        {
            PrefabManagementWindow.RepaintOctave3DWindow();
            PrefabsToCategoryDropSettingsWindow.RepaintOctave3DWindow();
            ObjectPlacementSettingsWindow.RepaintOctave3DWindow();
            PrefabTagsWindow.RepaintOctave3DWindow();
            ObjectLayersWindow.RepaintOctave3DWindow();
        }
        #endregion

        #region Private Methods
        private void Update()
        {
            if (Octave3DWorldBuilder.Instance == null) DestroyImmediate(this.gameObject);
        }
        #endregion
    }
}
#endif