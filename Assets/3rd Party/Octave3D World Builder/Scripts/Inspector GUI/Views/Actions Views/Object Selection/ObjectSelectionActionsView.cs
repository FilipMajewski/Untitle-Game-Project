#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class ObjectSelectionActionsView : ActionsView
    {
        #region Private Variables
        [SerializeField]
        private ObjectSelectionActionsViewData _viewData;
        #endregion

        #region Private Properties
        private ObjectSelectionActionsViewData ViewData
        {
            get
            {
                if (_viewData == null) _viewData = Octave3DWorldBuilder.Instance.CreateScriptableObject<ObjectSelectionActionsViewData>();
                return _viewData;
            }
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            EditorGUILayout.BeginHorizontal();
            RenderAssignSelectionToLayerButton();
            RenderSelectionAssignmentLayerSelectionPopup();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            RenderMakeSelectionStaticButton();
            RenderMakeSelectionDynamicButton();
            EditorGUILayout.EndHorizontal();

            RenderInvertSelectionButton();
        }
        #endregion

        #region Private Methods
        private void RenderAssignSelectionToLayerButton()
        {
            if(GUILayout.Button(GetContentForAssignSelectionToLayerButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth * 0.7f)))
            {
                ObjectSelectionActions.AssignSelectedObjectsToLayer(ViewData.SelectionAssignmentLayer);
            }
        }

        private GUIContent GetContentForAssignSelectionToLayerButton()
        {
            var content = new GUIContent();
            content.text = "Assign selection to layer";
            content.tooltip = "Assigns the current object selection to the layer specified in the adjacent popup.";

            return content;
        }

        private void RenderSelectionAssignmentLayerSelectionPopup()
        {
            int newInt = EditorGUILayout.LayerField(GetContentForSelectionAssignmentLayerSelectionPopup(), ViewData.SelectionAssignmentLayer);
            if(newInt != ViewData.SelectionAssignmentLayer)
            {
                UndoEx.RecordForToolAction(ViewData);
                ViewData.SelectionAssignmentLayer = newInt;
            }
        }

        private GUIContent GetContentForSelectionAssignmentLayerSelectionPopup()
        {
            var content = new GUIContent();
            content.text = "";
            content.tooltip = "";

            return content;
        }

        private void RenderMakeSelectionStaticButton()
        {
            if(GUILayout.Button(GetContentForMakeSelectionStaticButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth * 0.7f)))
            {
                List<GameObject> allSelectedObjects = ObjectSelection.Get().GetAllSelectedGameObjects();
                UndoEx.RecordForToolAction(allSelectedObjects);
                ObjectActions.MakeObjectsStatic(allSelectedObjects);
            }
        }

        private GUIContent GetContentForMakeSelectionStaticButton()
        {
            var content = new GUIContent();
            content.text = "Make selection static";
            content.tooltip = "Pressing this button will mark all selected objects as static.";

            return content;
        }

        private void RenderMakeSelectionDynamicButton()
        {
            if (GUILayout.Button(GetContentForMakeSelectionDynamicButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth * 0.7f)))
            {
                List<GameObject> allSelectedObjects = ObjectSelection.Get().GetAllSelectedGameObjects();
                UndoEx.RecordForToolAction(allSelectedObjects);
                ObjectActions.MakeObjectsDynamic(allSelectedObjects);
            }
        }

        private GUIContent GetContentForMakeSelectionDynamicButton()
        {
            var content = new GUIContent();
            content.text = "Make selection dynamic";
            content.tooltip = "Pressing this button will mark all selected objects as dynamic.";

            return content;
        }

        private void RenderInvertSelectionButton()
        {
            if (GUILayout.Button(GetContentForInvertSelection(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth * 0.7f)))
            {
                UndoEx.RecordForToolAction(ObjectSelection.Get());
                ObjectSelectionActions.InvertSelection();
            }
        }

        private GUIContent GetContentForInvertSelection()
        {
            var content = new GUIContent();
            content.text = "Invert selection";
            content.tooltip = "Pressing this button will deselect all currently selected objects and select only the ones which are not currently selected.";

            return content;
        }
        #endregion
    }
}
#endif