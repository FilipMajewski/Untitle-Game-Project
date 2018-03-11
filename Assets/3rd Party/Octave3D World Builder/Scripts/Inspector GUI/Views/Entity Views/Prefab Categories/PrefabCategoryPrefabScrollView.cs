#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class PrefabCategoryPrefabScrollView : EntityView
    {
        #region Private Variables
        [NonSerialized]
        private PrefabCategory _prefabCategory;
        [NonSerialized]
        private List<Prefab> _filteredPrefabs;

        [SerializeField]
        private PrefabCategoryPrefabScrollViewData _viewData;
        #endregion

        #region Private Properties
        private PrefabCategoryPrefabScrollViewData ViewData
        {
            get
            {
                if (_viewData == null) _viewData = Octave3DWorldBuilder.Instance.CreateScriptableObject<PrefabCategoryPrefabScrollViewData>();
                return _viewData;
            }
        }
        #endregion

        #region Public Properties
        public PrefabCategory PrefabCategory { set { _prefabCategory = value; } }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            AcquireFilteredPrefabs();
            if (_prefabCategory != null)
            {
                _prefabCategory.PrefabFilter.View.Render();
                RenderPrefabScrollView();
                RenderLookAndFeelView();
            }
        }
        #endregion

        #region Private Methods
        private void AcquireFilteredPrefabs()
        {
            if (_prefabCategory == null) _filteredPrefabs = new List<Prefab>();
            else _filteredPrefabs = _prefabCategory.GetFilteredPrefabs();
        }

        private void RenderPrefabScrollView()
        {
            ViewData.PrefabScrollPosition = EditorGUILayout.BeginScrollView(ViewData.PrefabScrollPosition, GetStyleForPrefabScrollView(), GUILayout.Height(ViewData.PrefabScrollViewHeight));
            if (_prefabCategory.IsEmpty) EditorGUILayoutEx.InformativeLabel("Drop prefabs and prefab folders here to populate the category. Drop operations might take a while when dropping folders that contain a large number of prefabs.");
            else RenderPrefabPreviewRows();
            EditorGUILayout.EndScrollView();

            HandleDragAndDropEvent(GUILayoutUtility.GetLastRect());
        }

        private GUIStyle GetStyleForPrefabScrollView()
        {
            var style = new GUIStyle("Box");
            return style;
        }

        private void RenderPrefabPreviewRows()
        {
            for (int prefabIndex = 0; prefabIndex < _filteredPrefabs.Count; ++prefabIndex)
            {
                // Start a new row?
                if (prefabIndex % ViewData.NumberOfPrefabsPerRow == 0)
                {
                    if (prefabIndex != 0) EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                }

                // Render the prefab entry
                Prefab prefab = _filteredPrefabs[prefabIndex];
                var previewButtonRenderData = new PrefabPreviewButtonRenderData();
                previewButtonRenderData.ExtractFromPrefab(prefab, ViewData.PrefabPreviewScale);

                EditorGUILayout.BeginVertical(GUILayout.Width(previewButtonRenderData.ButtonWidth));
                
                // Render the prefab preview button
                EditorGUIColor.Push(prefab == _prefabCategory.ActivePrefab ? ViewData.ActivePrefabHighlightColor : Color.white);
                if (EditorGUILayoutEx.PrefabPreview(prefab, true, previewButtonRenderData))
                {
                    UndoEx.RecordForToolAction(_prefabCategory);
                    _prefabCategory.SetActivePrefab(prefab);
                }
                EditorGUIColor.Pop();

                // Render the prefab name labels if necessary
                if(ViewData.ShowPrefabNames)
                {
                    Rect previewRectangle = GUILayoutUtility.GetLastRect();
                    EditorGUILayoutEx.LabelInMiddleOfControlRect(previewRectangle, prefab.Name, previewButtonRenderData.ButtonHeight, GetStyleForPrefabNameLabel());
                }
       
                // Render the remove prefab button
                if (GUILayout.Button(GetRemovePrefabButtonContent()))
                {
                    UndoEx.RecordForToolAction(_prefabCategory);
                    _prefabCategory.RemoveAndDestroyPrefab(prefab);
                }

                EditorGUILayout.EndVertical();
            }

            // End the last row (if any)
            if (_filteredPrefabs.Count != 0) EditorGUILayout.EndHorizontal();
        }

        private GUIStyle GetStyleForPrefabNameLabel()
        {
            var style = new GUIStyle();
            style.fontStyle = FontStyle.Bold;
            return style;
        }

        private GUIContent GetRemovePrefabButtonContent()
        {
            var content = new GUIContent();
            content.text = "Remove";
            content.tooltip = "Removes the prefab from the active category.";

            return content;
        }

        private void HandleDragAndDropEvent(Rect dropRectangle)
        {
            PrefabsToCategoryDropEventHandler.Get().Handle(Event.current, dropRectangle);
        }

        private void RenderLookAndFeelView()
        {
            ToggleLookAndFeelViewVisibility();
            if (ViewData.IsLookAndFeelViewVisible)
            {
                RenderActivePrefabHighlightColorField();
                RenderPrefabsPerRowIntField();
                RenderPrefabPreviewScaleSlider();
                RenderPrefabScrollViewHeightSlider();
                RenderShowPrefabNamesToggle();
            }
        }

        private void ToggleLookAndFeelViewVisibility()
        {
            bool newBool = EditorGUILayout.Foldout(ViewData.IsLookAndFeelViewVisible, "Look and Feel");
            if (newBool != ViewData.IsLookAndFeelViewVisible)
            {
                UndoEx.RecordForToolAction(ViewData);
                ViewData.IsLookAndFeelViewVisible = newBool;
            }
        }

        private void RenderActivePrefabHighlightColorField()
        {
            Color newColor = EditorGUILayout.ColorField(GetContentForActivePrefabHighlighColorField(), ViewData.ActivePrefabHighlightColor);
            if (newColor != ViewData.ActivePrefabHighlightColor)
            {
                UndoEx.RecordForToolAction(ViewData);
                ViewData.ActivePrefabHighlightColor = newColor;
            }
        }

        private GUIContent GetContentForActivePrefabHighlighColorField()
        {
            var content = new GUIContent();
            content.text = "Active prefab highlight color";
            content.tooltip = "Allows you to specify the highligh/tint color for the active prefab preview. This is the prefab that is currently selected in the active prefab category.";

            return content;
        }

        private void RenderPrefabsPerRowIntField()
        {
            int newInt = EditorGUILayout.IntField(GetContentForPrefabsPerRowIntField(), ViewData.NumberOfPrefabsPerRow);
            if (ViewData.NumberOfPrefabsPerRow != newInt)
            {
                UndoEx.RecordForToolAction(ViewData);
                ViewData.NumberOfPrefabsPerRow = newInt;
            }
        }

        private GUIContent GetContentForPrefabsPerRowIntField()
        {
            var content = new GUIContent();
            content.text = "Prefabs per row";
            content.tooltip = "Allows you to specify how many prefab previews can be shown in one row inside the prefab scroll view.";

            return content;
        }

        private void RenderPrefabPreviewScaleSlider()
        {
            float newFloat = EditorGUILayout.Slider(GetContentForPrefabPreviewScaleSlider(), ViewData.PrefabPreviewScale, PrefabCategoryPrefabScrollViewData.MinPrefabPreviewScale, PrefabCategoryPrefabScrollViewData.MaxPrefabPreviewScale);
            if (newFloat != ViewData.PrefabPreviewScale)
            {
                UndoEx.RecordForToolAction(ViewData);
                ViewData.PrefabPreviewScale = newFloat;
            }
        }

        private GUIContent GetContentForPrefabPreviewScaleSlider()
        {
            var content = new GUIContent();
            content.text = "Prefab preview scale";
            content.tooltip = "Allows you to specify the scale of the prefab preview images (min: " + PrefabCategoryPrefabScrollViewData.MinPrefabPreviewScale + ", max: " + PrefabCategoryPrefabScrollViewData.MaxPrefabPreviewScale + ").";

            return content;
        }

        private void RenderPrefabScrollViewHeightSlider()
        {
            float newFloat = EditorGUILayout.Slider(GetContentForPrefabScrollViewHeightSlider(), ViewData.PrefabScrollViewHeight, PrefabCategoryPrefabScrollViewData.MinPrefabScrollViewHeight, PrefabCategoryPrefabScrollViewData.MaxPrefabScrollViewHeight);
            if (newFloat != ViewData.PrefabScrollViewHeight)
            {
                UndoEx.RecordForToolAction(ViewData);
                ViewData.PrefabScrollViewHeight = newFloat;
            }
        }

        private GUIContent GetContentForPrefabScrollViewHeightSlider()
        {
            var content = new GUIContent();
            content.text = "Scroll view height";
            content.tooltip = "Allows you to specify the height of the prefab scroll view (min: " + PrefabCategoryPrefabScrollViewData.MinPrefabScrollViewHeight + ", max: " + PrefabCategoryPrefabScrollViewData.MaxPrefabScrollViewHeight + ").";

            return content;
        }

        private void RenderShowPrefabNamesToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForShowPrefabNamesToggle(), ViewData.ShowPrefabNames);
            if(newBool != ViewData.ShowPrefabNames)
            {
                UndoEx.RecordForToolAction(ViewData);
                ViewData.ShowPrefabNames = newBool;
            }
        }

        private GUIContent GetContentForShowPrefabNamesToggle()
        {
            var content = new GUIContent();
            content.text = "Show prefab names";
            content.tooltip = "If this is checked, each prefab preview will contain a label with the prefab's name.";

            return content;
        }
        #endregion
    }
}
#endif