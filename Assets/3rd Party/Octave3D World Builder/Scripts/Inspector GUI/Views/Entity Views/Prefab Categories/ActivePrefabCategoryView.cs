#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class ActivePrefabCategoryView : EntityView
    {
        #region Private Constant Variables
        private const float _actionButtonScale = 0.8f;
        #endregion

        #region Private Variables
        [SerializeField]
        private ActivePrefabCategoryViewData _viewData;

        [SerializeField]
        private PrefabCategoryPrefabScrollView _activePrefabCategoryPrefabScrollView = new PrefabCategoryPrefabScrollView();
        #endregion

        #region Private Properties
        private ActivePrefabCategoryViewData ViewData
        {
            get
            {
                if (_viewData == null) _viewData = Octave3DWorldBuilder.Instance.CreateScriptableObject<ActivePrefabCategoryViewData>();
                return _viewData;
            }
        }
        #endregion

        #region Constructors
        public ActivePrefabCategoryView()
        {
            ToggleVisibilityBeforeRender = true;
            VisibilityToggleLabel = "Active Prefab Category View";
            SurroundWithBox = true;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            _activePrefabCategoryPrefabScrollView.PrefabCategory = PrefabCategoryDatabase.Get().ActivePrefabCategory;
            RenderActiveCategorySelectionPopup();
            RenderActiveCategoryNameChangeTextField();

            EditorGUILayout.Separator();
            RenderPrefabsToActiveCategoryDropOperationSettingsButton();
            _activePrefabCategoryPrefabScrollView.Render();
            RenderActionsView();
        }
        #endregion

        #region Private Methods
        private void RenderActiveCategorySelectionPopup()
        {
            PrefabCategoryDatabase prefabCategoryDatabase = PrefabCategoryDatabase.Get();
            int indexOfNewActiveCategory = EditorGUILayoutEx.Popup(GetContentForActivePrefabCategorySelectionPopup(), prefabCategoryDatabase.IndexOfActiveCategory, prefabCategoryDatabase.GetAllPrefabCategoryNames());
            if (indexOfNewActiveCategory != prefabCategoryDatabase.IndexOfActiveCategory)
            {
                UndoEx.RecordForToolAction(prefabCategoryDatabase);
                prefabCategoryDatabase.SetActivePrefabCategory(prefabCategoryDatabase.GetPrefabCategoryByIndex(indexOfNewActiveCategory));
            }
        }

        private GUIContent GetContentForActivePrefabCategorySelectionPopup()
        {
            var content = new GUIContent();
            content.text = "Active category";
            content.tooltip = "Allows you to change the active prefab category.";

            return content;
        }

        private void RenderActiveCategoryNameChangeTextField()
        {
            PrefabCategoryDatabase prefabCategoryDatabase = PrefabCategoryDatabase.Get();
            PrefabCategory activeCategory = prefabCategoryDatabase.ActivePrefabCategory;

            if (prefabCategoryDatabase.CanPrefabCategoryBeRenamed(activeCategory))
            {
                string newString = EditorGUILayoutEx.DelayedTextField(GetContentForActiveCategoryNameChangeField(), activeCategory.Name);
                if (newString != activeCategory.Name)
                {
                    UndoEx.RecordForToolAction(activeCategory);
                    prefabCategoryDatabase.RenamePrefabCategory(activeCategory, newString);
                }
            }
            else EditorGUILayoutEx.InformativeLabel("The default category can not be renamed.");
        }

        private GUIContent GetContentForActiveCategoryNameChangeField()
        {
            var content = new GUIContent();
            content.text = "Name:";
            content.tooltip = "Allows you to change the name of the active prefab category. Note: The default category can not be renamed.";

            return content;
        }

        private void RenderPrefabsToActiveCategoryDropOperationSettingsButton()
        {
            if (GUILayout.Button(GetContentForPrefabsToActiveCategoryDropOperationSettingsButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth * 0.75f)))
            {
                PrefabsToCategoryDropSettingsWindow.Get().ShowOctave3DWindow();
            }
        }

        private GUIContent GetContentForPrefabsToActiveCategoryDropOperationSettingsButton()
        {
            var content = new GUIContent();
            content.text = "Prefabs drop settings...";
            content.tooltip = "Opens a new window which allows you to specify different settings related to prefab and prefab folder drop actions.";
           
            return content;
        }

        private void RenderActionsView()
        {
            ToggleActionViewVisibility();
            if(ViewData.IsActionViewVisible)
            {
                RenderCreateNewCategoryControls();
                RenderRemoveCategoryControls();
                RenderClearCategoryControls();

                EditorGUILayout.Separator();
                RenderMoveAllPrefabsToCategoryControls();
                RenderMoveFilteredPrefabsToCategoryControls();
                RenderMoveActivePrefabToCategoryControls();

                EditorGUILayout.Separator();
                EditorGUILayout.BeginHorizontal();
                RenderSetPrefabOffsetFromGridSurfaceInActiveCategoryButton();
                RenderPrefabOffsetFromGridSurfaceField();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                RenderSetPrefabOffsetFromObjectSurfaceInActiveCategoryButton();
                RenderPrefabOffsetFromObjectSurfaceField();
                EditorGUILayout.EndHorizontal();
            }
        }

        private void ToggleActionViewVisibility()
        {
            bool newBool = EditorGUILayout.Foldout(ViewData.IsActionViewVisible, "Actions");
            if (newBool != ViewData.IsActionViewVisible)
            {
                UndoEx.RecordForToolAction(ViewData);
                ViewData.IsActionViewVisible = newBool;
            }
        }

        private void RenderCreateNewCategoryControls()
        {
            EditorGUILayout.BeginHorizontal();
            RenderCreateNewCategoryButton();
            RenderCreateNewCategoryNameChangeTextField();
            EditorGUILayout.EndHorizontal();
        }

        private void RenderCreateNewCategoryButton()
        {
            if (GUILayout.Button(GetContentForCreateNewCategoryButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth * _actionButtonScale)))
            {
                PrefabCategoryDatabase prefabCategoryDatabase = PrefabCategoryDatabase.Get();
                UndoEx.RecordForToolAction(prefabCategoryDatabase);

                PrefabCategory newCategory = prefabCategoryDatabase.CreatePrefabCategory(ViewData.NameForNewPrefabCategory);
                prefabCategoryDatabase.SetActivePrefabCategory(newCategory);
            }
        }

        private GUIContent GetContentForCreateNewCategoryButton()
        {
            var content = new GUIContent();
            content.text = "Create category";
            content.tooltip = "Creates a new prefab category using the name specified in the adjacent text field. " +
                              "Note: Names are automatically adjusted such that each category name is unique.";

            return content;
        }

        private void RenderCreateNewCategoryNameChangeTextField()
        {
            string newString = EditorGUILayout.TextField(ViewData.NameForNewPrefabCategory);
            if (newString != ViewData.NameForNewPrefabCategory)
            {
                UndoEx.RecordForToolAction(ViewData);
                ViewData.NameForNewPrefabCategory = newString;
            }
        }

        private void RenderRemoveCategoryControls()
        {
            EditorGUILayout.BeginHorizontal();
            RenderRemoveActiveCategoryButton();
            RenderRemoveAllCategoriesButton();
            RenderRemoveEmptyCategoriesButton();
            EditorGUILayout.EndHorizontal();
        }

        private void RenderRemoveActiveCategoryButton()
        {
            if (GUILayout.Button(GetContentForRemoveActiveCategoryButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth * _actionButtonScale)))
            {
                UndoEx.RecordForToolAction(PrefabCategoryDatabase.Get());
                PrefabCategoryDatabase.Get().RemoveAndDestroyPrefabCategory(PrefabCategoryDatabase.Get().ActivePrefabCategory);
            }
        }

        private GUIContent GetContentForRemoveActiveCategoryButton()
        {
            var content = new GUIContent();
            content.text = "Remove active category";
            content.tooltip = "Removes the active category. Note: The default category can not be removed.";

            return content;
        }

        private void RenderRemoveAllCategoriesButton()
        {
            if (GUILayout.Button(GetContentForRemoveAllCategoriesButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth * _actionButtonScale)))
            {
                UndoEx.RecordForToolAction(PrefabCategoryDatabase.Get());
                PrefabCategoryDatabase.Get().RemoveAndDestroyAllPrefabCategories();
            }
        }

        private GUIContent GetContentForRemoveAllCategoriesButton()
        {
            var content = new GUIContent();
            content.text = "Remove all categories";
            content.tooltip = "Removes all categories. Note: The default category can not be removed.";

            return content;
        }

        private void RenderRemoveEmptyCategoriesButton()
        {
            if (GUILayout.Button(GetContentForRemoveEmptyCategoriesButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth * _actionButtonScale)))
            {
                UndoEx.RecordForToolAction(PrefabCategoryDatabase.Get());
                PrefabCategoryDatabase.Get().RemoveAndDestroyEmptyPrefabCategories();
            }
        }

        private GUIContent GetContentForRemoveEmptyCategoriesButton()
        {
            var content = new GUIContent();
            content.text = "Remove empty categories";
            content.tooltip = "Removes all empty categories from the database. Note: The default category can not be removed.";

            return content;
        }

        private void RenderClearCategoryControls()
        {
            EditorGUILayout.BeginHorizontal();
            RenderClearActiveCategoryButton();
            RenderClearAllCategoriesButton();
            EditorGUILayout.EndHorizontal();
        }

        private void RenderClearActiveCategoryButton()
        {
            if (GUILayout.Button(GetContentForClearActiveCategoryButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth * _actionButtonScale)))
            {
                UndoEx.RecordForToolAction(PrefabCategoryDatabase.Get().ActivePrefabCategory);
                PrefabCategoryDatabase.Get().ActivePrefabCategory.RemoveAndDestroyAllPrefabs();
            }
        }

        private GUIContent GetContentForClearActiveCategoryButton()
        {
            var content = new GUIContent();
            content.text = "Clear active category";
            content.tooltip = "Removes all prefabs from the active prefab category.";

            return content;
        }

        private void RenderClearAllCategoriesButton()
        {
            if (GUILayout.Button(GetContentForClearAllCategoriesButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth * _actionButtonScale)))
            {
                PrefabCategoryDatabase.Get().RecordAllPrefabCategoriesForUndo();
                PrefabCategoryDatabase.Get().RemoveAndDestroyAllPrefabsInAllCategories();
            }
        }

        private GUIContent GetContentForClearAllCategoriesButton()
        {
            var content = new GUIContent();
            content.text = "Clear all categories";
            content.tooltip = "Removes all prefabs from all prefab categories.";

            return content;
        }

        private void RenderMoveAllPrefabsToCategoryControls()
        {
            EditorGUILayout.BeginHorizontal();
            RenderMoveAllPrefabsToCategoryButton();
            RenderCategorySelectionPopupForMoveAllPrefabsOperation();
            EditorGUILayout.EndHorizontal();
        }

        private void RenderMoveAllPrefabsToCategoryButton()
        {
            if (GUILayout.Button(GetContentForMoveAllPrefabsToCategoryButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth)))
            {
                PrefabCategory destinationCategory = ViewData.DestinationCategoryForAllPrefabs;
                if(destinationCategory != null)
                {
                    UndoEx.RecordForToolAction(destinationCategory);
                    UndoEx.RecordForToolAction(PrefabCategoryDatabase.Get().ActivePrefabCategory);
                    PrefabCategoryDatabase.Get().ActivePrefabCategory.TransferAllPrefabsToCategory(destinationCategory);
                }
            }
        }

        private GUIContent GetContentForMoveAllPrefabsToCategoryButton()
        {
            var content = new GUIContent();
            content.text = "Move all prefabs to category";
            content.tooltip = "Moves all prefabs inside the active category to the category specified in the adjacent selection popup.";

            return content;
        }

        private void RenderCategorySelectionPopupForMoveAllPrefabsOperation()
        {
            List<string> allPrefabCategoryNames = PrefabCategoryDatabase.Get().GetAllPrefabCategoryNames();
            string newString = EditorGUILayoutEx.Popup(GetContentForCategorySelectionPopupForMoveAllPrefabsOperation(), ViewData.DestinationCategoryForAllPrefabs.Name, allPrefabCategoryNames);
            if (newString != ViewData.DestinationCategoryForAllPrefabs.Name)
            {
                UndoEx.RecordForToolAction(ViewData);
                ViewData.DestinationCategoryForAllPrefabs = PrefabCategoryDatabase.Get().GetPrefabCategoryByName(newString);
            }
        }

        private GUIContent GetContentForCategorySelectionPopupForMoveAllPrefabsOperation()
        {
            var content = new GUIContent();
            return content;
        }

        private void RenderMoveFilteredPrefabsToCategoryControls()
        {
            EditorGUILayout.BeginHorizontal();
            RenderMoveFilteredPrefabsToCategoryButton();
            RenderCategorySelectionPopupForMoveFilteredPrefabsOperation();
            EditorGUILayout.EndHorizontal();
        }

        private void RenderMoveFilteredPrefabsToCategoryButton()
        {
            if (GUILayout.Button(GetContentForMoveFilteredPrefabsToCategoryButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth)))
            {
                PrefabCategory destinationCategory = ViewData.DestinationCategoryForFilteredPrefabs;
                if(destinationCategory != null)
                {
                    PrefabCategory activePrefabCategory = PrefabCategoryDatabase.Get().ActivePrefabCategory;
                    UndoEx.RecordForToolAction(destinationCategory);
                    UndoEx.RecordForToolAction(activePrefabCategory);

                    activePrefabCategory.TransferPrefabCollectionToCategory(activePrefabCategory.GetFilteredPrefabs(), destinationCategory);
                }
            }
        }

        private GUIContent GetContentForMoveFilteredPrefabsToCategoryButton()
        {
            var content = new GUIContent();
            content.text = "Move filtered prefabs to category";
            content.tooltip = "Moves the filtered prefabs inside the active category to the category specified in the adjacent selection popup.";
         
            return content;
        }

        private void RenderCategorySelectionPopupForMoveFilteredPrefabsOperation()
        {
            List<string> allPrefabCategoryNames = PrefabCategoryDatabase.Get().GetAllPrefabCategoryNames();
            string newString = EditorGUILayoutEx.Popup(GetContentForCategorySelectionPopupForMoveFilteredPrefabsOperation(), ViewData.DestinationCategoryForFilteredPrefabs.Name, allPrefabCategoryNames);
            if (newString != ViewData.DestinationCategoryForFilteredPrefabs.Name)
            {
                UndoEx.RecordForToolAction(ViewData);
                ViewData.DestinationCategoryForFilteredPrefabs = PrefabCategoryDatabase.Get().GetPrefabCategoryByName(newString);
            }
        }

        private GUIContent GetContentForCategorySelectionPopupForMoveFilteredPrefabsOperation()
        {
            var content = new GUIContent();
            return content;
        }

        private void RenderMoveActivePrefabToCategoryControls()
        {
            EditorGUILayout.BeginHorizontal();
            RenderMoveActivePrefabToCategoryButton();
            RenderCategorySelectionPopupForMoveActivePrefabOperation();
            EditorGUILayout.EndHorizontal();
        }

        private void RenderMoveActivePrefabToCategoryButton()
        {
            Prefab activePrefab = PrefabCategoryDatabase.Get().ActivePrefabCategory.ActivePrefab;
            if (GUILayout.Button(GetContentForMoveActivePrefabToCategoryButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth)) &&
                activePrefab != null)
            {
                PrefabCategory destinationCategory = ViewData.DestinationCategoryForActivePrefab;
                if (destinationCategory != null)
                {
                    PrefabCategory activePrefabCategory = PrefabCategoryDatabase.Get().ActivePrefabCategory;
                    UndoEx.RecordForToolAction(PrefabCategoryDatabase.Get());
                    UndoEx.RecordForToolAction(destinationCategory);
                    UndoEx.RecordForToolAction(activePrefabCategory);

                    activePrefabCategory.TransferPrefabToCategory(activePrefab, destinationCategory);
                    PrefabCategoryDatabase.Get().SetActivePrefabCategory(destinationCategory);
                    destinationCategory.SetActivePrefab(activePrefab);
                }
            }
        }

        private GUIContent GetContentForMoveActivePrefabToCategoryButton()
        {
            var content = new GUIContent();
            content.text = "Move active prefab to category";
            content.tooltip = "Moves the filtered prefabs inside the active category to the category specified in the adjacent selection popup.";

            return content;
        }

        private void RenderCategorySelectionPopupForMoveActivePrefabOperation()
        {
            List<string> allPrefabCategoryNames = PrefabCategoryDatabase.Get().GetAllPrefabCategoryNames();
            string newString = EditorGUILayoutEx.Popup(GetContentForCategorySelectionPopupForMoveActivePrefabOperation(), ViewData.DestinationCategoryForActivePrefab.Name, allPrefabCategoryNames);
            if (newString != ViewData.DestinationCategoryForActivePrefab.Name)
            {
                UndoEx.RecordForToolAction(ViewData);
                ViewData.DestinationCategoryForActivePrefab = PrefabCategoryDatabase.Get().GetPrefabCategoryByName(newString);
            }
        }

        private GUIContent GetContentForCategorySelectionPopupForMoveActivePrefabOperation()
        {
            var content = new GUIContent();
            return content;
        }

        private void RenderSetPrefabOffsetFromGridSurfaceInActiveCategoryButton()
        {
            if(GUILayout.Button(GetContentForSetPrefabOffsetFromGridSurfaceInActiveCategoryButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth)))
            {
                PrefabCategory activePrefabCategory = PrefabCategoryDatabase.Get().ActivePrefabCategory;
                UndoEx.RecordForToolAction(activePrefabCategory.GetAllPrefabs());
                PrefabCategoryActions.SetPrefabOffsetFromGridSurface(activePrefabCategory, ViewData.PrefabOffsetFromGridSurface);
            }
        }

        private GUIContent GetContentForSetPrefabOffsetFromGridSurfaceInActiveCategoryButton()
        {
            var content = new GUIContent();
            content.text = "Set prefab offset from grid surface";
            content.tooltip = "Pressing this button will change the offset from the grid surface for all prefabs which reside in the active category.";

            return content;
        }

        private void RenderPrefabOffsetFromGridSurfaceField()
        {
            float newFloat = EditorGUILayout.FloatField(ViewData.PrefabOffsetFromGridSurface);
            if (newFloat != ViewData.PrefabOffsetFromGridSurface)
            {
                UndoEx.RecordForToolAction(ViewData);
                ViewData.PrefabOffsetFromGridSurface = newFloat;
            }
        }

        private void RenderSetPrefabOffsetFromObjectSurfaceInActiveCategoryButton()
        {
            if (GUILayout.Button(GetContentForSetPrefabOffsetFromObjectSurfaceInActiveCategoryButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth)))
            {
                PrefabCategory activePrefabCategory = PrefabCategoryDatabase.Get().ActivePrefabCategory;
                UndoEx.RecordForToolAction(activePrefabCategory.GetAllPrefabs());
                PrefabCategoryActions.SetPrefabOffsetFromObjectSurface(activePrefabCategory, ViewData.PrefabOffsetFromObjectSurface);
            }
        }

        private GUIContent GetContentForSetPrefabOffsetFromObjectSurfaceInActiveCategoryButton()
        {
            var content = new GUIContent();
            content.text = "Set prefab offset from object surface";
            content.tooltip = "Pressing this button will change the offset from object surfaces for all prefabs which reside in the active category.";

            return content;
        }

        private void RenderPrefabOffsetFromObjectSurfaceField()
        {
            float newFloat = EditorGUILayout.FloatField(ViewData.PrefabOffsetFromObjectSurface);
            if (newFloat != ViewData.PrefabOffsetFromObjectSurface)
            {
                UndoEx.RecordForToolAction(ViewData);
                ViewData.PrefabOffsetFromObjectSurface = newFloat;
            }
        }
        #endregion
    }
}
#endif