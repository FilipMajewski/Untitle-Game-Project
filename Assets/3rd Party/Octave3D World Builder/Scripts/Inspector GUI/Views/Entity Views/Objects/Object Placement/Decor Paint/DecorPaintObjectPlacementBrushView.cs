#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class DecorPaintObjectPlacementBrushView : EntityView
    {
        #region Private Variables
        [NonSerialized]
        private DecorPaintObjectPlacementBrush _brush;
        #endregion

        #region Constructors
        public DecorPaintObjectPlacementBrushView(DecorPaintObjectPlacementBrush brush)
        {
            _brush = brush;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderNameChangeField();
            RenderRadiusField();
            RenderMaxNumberOfObjectsField();
            RenderDistanceBetweenObjectsField();
            RenderIgnoreObjectsOutsideOfPaintSurfaceToggle();
            RenderDestinationCategoryForElementPrefabsSelectionPopup();

            EditorGUILayout.Separator();
            if (_brush.IsEmpty) EditorGUILayoutEx.InformativeLabel("There are no brush elements available.");
            else
            {
                EditorGUILayout.BeginHorizontal();
                RenderAlignAllElementsToStrokeButton();
                RenderUnalignAllElementsToStrokeButton();
                EditorGUILayout.EndHorizontal();

                RenderBrushElementViews();
            }

            EditorGUILayout.BeginHorizontal();
            RenderCreateNewElementButton();
            if (!_brush.IsEmpty) RenderRemoveAllElementsButton();
            EditorGUILayout.EndHorizontal();
        }
        #endregion

        #region Private Methods
        private void RenderNameChangeField()
        {
            string newString = EditorGUILayout.TextField(GetContentForNameChangeField(), _brush.Name);
            if(newString != _brush.Name)
            {
                UndoEx.RecordForToolAction(_brush);
                DecorPaintObjectPlacementBrushDatabase.Get().RenameBrush(_brush, newString);
            }
        }

        private GUIContent GetContentForNameChangeField()
        {
            var content = new GUIContent();
            content.text = "Name";
            content.tooltip = "Allows you to change the name of the brush.";

            return content;
        }

        private void RenderRadiusField()
        {
            float newFloat = EditorGUILayout.FloatField(GetContentForRadiusField(), _brush.Radius);
            if(newFloat != _brush.Radius)
            {
                UndoEx.RecordForToolAction(_brush);
                _brush.Radius = newFloat;
            }
        }

        private GUIContent GetContentForRadiusField()
        {
            var content = new GUIContent();
            content.text = "Radius";
            content.tooltip = "Allows you to change the brush radius.";

            return content;
        }

        private void RenderMaxNumberOfObjectsField()
        {
            int newInt = EditorGUILayout.IntField(GetContentForMaxNumberOfObjectsField(), _brush.MaxNumberOfObjects);
            if(newInt != _brush.MaxNumberOfObjects)
            {
                UndoEx.RecordForToolAction(_brush);
                _brush.MaxNumberOfObjects = newInt;
            }
        }

        private GUIContent GetContentForMaxNumberOfObjectsField()
        {
            var content = new GUIContent();
            content.text = "Max objects";
            content.tooltip = "This is the maximum number of objects which can be placed inside the brush.";

            return content;
        }

        private void RenderDistanceBetweenObjectsField()
        {
            float newFloat = EditorGUILayout.FloatField(GetContentForDistanceBetweenObjectsField(), _brush.DistanceBetweenObjects);
            if(newFloat != _brush.DistanceBetweenObjects)
            {
                UndoEx.RecordForToolAction(_brush);
                _brush.DistanceBetweenObjects = newFloat;
            }
        }

        private GUIContent GetContentForDistanceBetweenObjectsField()
        {
            var content = new GUIContent();
            content.text = "Distance between objects";
            content.tooltip = "This is the distance between objects in world units.";

            return content;
        }

        private void RenderIgnoreObjectsOutsideOfPaintSurfaceToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForIgnoreObjectsOutsideOfPaintSurfaceToggle(), _brush.IgnoreObjectsOutsideOfPaintSurface);
            if(newBool != _brush.IgnoreObjectsOutsideOfPaintSurface)
            {
                UndoEx.RecordForToolAction(_brush);
                _brush.IgnoreObjectsOutsideOfPaintSurface = newBool;
            }
        }

        private GUIContent GetContentForIgnoreObjectsOutsideOfPaintSurfaceToggle()
        {
            var content = new GUIContent();
            content.text = "Ignore objects outside of paint surface";
            content.tooltip = "If this is checked, objects whose positions are generated outside the paint surface will not be placed in the scene. " + 
                              "Note: This only applies to mesh surfaces. For terrians, outside objects will always be ignored.";

            return content;
        }

        private void RenderDestinationCategoryForElementPrefabsSelectionPopup()
        {
            string newString = EditorGUILayoutEx.Popup(GetContentForDestinationCategoryForElementPrefabsSelectionPopup(), _brush.DestinationCategoryForElementPrefabs.Name, PrefabCategoryDatabase.Get().GetAllPrefabCategoryNames());
            if(newString != _brush.DestinationCategoryForElementPrefabs.Name)
            {
                UndoEx.RecordForToolAction(_brush);
                _brush.DestinationCategoryForElementPrefabs = PrefabCategoryDatabase.Get().GetPrefabCategoryByName(newString);
            }
        }

        private GUIContent GetContentForDestinationCategoryForElementPrefabsSelectionPopup()
        {
            var content = new GUIContent();
            content.text = "Destination prefab category";
            content.tooltip = "When a prefab is associated with a brush element, that prefab will be assigned to this category.";

            return content;
        }

        private void RenderAlignAllElementsToStrokeButton()
        {
            if (GUILayout.Button(GetContentForAlignAllElementsToStrokeButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth)))
            {
                List<DecorPaintObjectPlacementBrushElement> allBrushElements = _brush.GetAllBrushElements();
                foreach (DecorPaintObjectPlacementBrushElement brushElement in allBrushElements)
                {
                    UndoEx.RecordForToolAction(brushElement);
                    brushElement.AlignToStroke = true;
                }
            }
        }

        private GUIContent GetContentForAlignAllElementsToStrokeButton()
        {
            var content = new GUIContent();
            content.text = "Align all to stroke";
            content.tooltip = "Press this button if you want all elements to align to stroke.";

            return content;
        }

        private void RenderUnalignAllElementsToStrokeButton()
        {
            if (GUILayout.Button(GetContentForUnalignAllElementsToStrokeButton()))
            {
                List<DecorPaintObjectPlacementBrushElement> allBrushElements = _brush.GetAllBrushElements();
                foreach (DecorPaintObjectPlacementBrushElement brushElement in allBrushElements)
                {
                    UndoEx.RecordForToolAction(brushElement);
                    brushElement.AlignToStroke = false;
                }
            }
        }

        private GUIContent GetContentForUnalignAllElementsToStrokeButton()
        {
            var content = new GUIContent();
            content.text = "Unalign all";
            content.tooltip = "Press this button when you don't want any elements to align to stroke.";

            return content;
        }

        private void RenderBrushElementViews()
        {
            List<DecorPaintObjectPlacementBrushElement> allBrushElements = _brush.GetAllBrushElements();
            foreach(DecorPaintObjectPlacementBrushElement brushElement in allBrushElements)
            {
                brushElement.View.Render();
                RenderRemoveElementButton(brushElement);
            }
        }

        private void RenderRemoveElementButton(DecorPaintObjectPlacementBrushElement brushElement)
        {
            if(GUILayout.Button(GetContentForRemoveElementButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth)))
            {
                UndoEx.RecordForToolAction(_brush);
                _brush.RemoveAndDestroyElement(brushElement);
            }
        }

        private GUIContent GetContentForRemoveElementButton()
        {
            var content = new GUIContent();
            content.text = "Remove element";
            content.tooltip = "Removes the element from the brush.";

            return content;
        }
        
        private void RenderCreateNewElementButton()
        {
            if(GUILayout.Button(GetContentForCreateNewElementButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth)))
            {
                UndoEx.RecordForToolAction(_brush);
                _brush.CreateNewElement();
            }
        }

        private GUIContent GetContentForCreateNewElementButton()
        {
            var content = new GUIContent();
            content.text = "Create new element";
            content.tooltip = "Creates a new brush element. A brush element represents a prefab that can be instantiated while painting.";

            return content;
        }

        private void RenderRemoveAllElementsButton()
        {
            if (GUILayout.Button(GetContentForRemoveAllElementsButton()))
            {
                UndoEx.RecordForToolAction(_brush);
                _brush.RemoveAndDestroyAllElements();
            }
        }

        private GUIContent GetContentForRemoveAllElementsButton()
        {
            var content = new GUIContent();
            content.text = "Remove all elements";
            content.tooltip = "Removes all elements from the brush.";

            return content;
        }
        #endregion
    }
}
#endif