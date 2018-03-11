#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class DecorPaintObjectPlacementBrushElementView : EntityView
    {
        #region Private Constant Variables
        private const float _prefabPreviewSize = 128.0f;
        #endregion

        #region Private Variables
        [NonSerialized]
        private DecorPaintObjectPlacementBrushElement _brushElement;
        #endregion

        #region Constructors
        public DecorPaintObjectPlacementBrushElementView(DecorPaintObjectPlacementBrushElement brushElement)
        {
            _brushElement = brushElement;

            SurroundWithBox = true;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            EditorGUILayout.BeginHorizontal();
            RenderPrefabPreviewBox();
            EditorGUILayout.BeginVertical();
            RenderAlignToSurfaceToggle();
            if (_brushElement.AlignToSurface) RenderAlignmentAxisSelectionPopup();
            if (_brushElement.AlignToSurface) RenderRotationOffsetSlider();
            RenderMissChanceField();
            RenderAlignToStrokeToggle();
            RenderRandomizeRotationToggle();
            _brushElement.ScaleRandomizationSettings.View.Render();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();
            if (!_brushElement.ScaleRandomizationSettings.RandomizeScale) RenderScaleField();
            _brushElement.SlopeSettings.View.Render();
        }
        #endregion

        #region Private Methods
        private void RenderPrefabPreviewBox()
        {
            GUILayout.Box(GetContentForPrefabPreviewBox(),
                             GetStyleForPrefabPreviewBox(), GUILayout.Width(_prefabPreviewSize), GUILayout.Height(_prefabPreviewSize));

            Rect previewBoxRect = GUILayoutUtility.GetLastRect();
            RenderIsElementActiveToggle(previewBoxRect);

            PrefabsToDecorPaintBrushEventHandler.Get().DestinationDecorPaintBrushElement = _brushElement;
            PrefabsToDecorPaintBrushEventHandler.Get().Handle(Event.current, previewBoxRect);
        }

        private GUIContent GetContentForPrefabPreviewBox()
        {
            var content = new GUIContent();
            content.text = "";

            if (_brushElement.Prefab == null)
            {
                content.text = "(No prefab)";
                content.tooltip = "Drop a prefab here to associate it with this brush element.";
                content.image = null;
            }
            else
            {
                content.tooltip = _brushElement.Prefab.Name;
                content.image = PrefabPreviewTextureCache.Get().GetPrefabPreviewTexture(_brushElement.Prefab);
            }

            return content;
        }

        private void RenderIsElementActiveToggle(Rect elementPreviewBoxRect)
        {
            elementPreviewBoxRect.x += 2.0f;
            bool newBool = GUI.Toggle(elementPreviewBoxRect, _brushElement.IsActive, GetContentForIsElementActiveToggle());
            if(newBool != _brushElement.IsActive)
            {
                UndoEx.RecordForToolAction(_brushElement);
                _brushElement.IsActive = newBool;
            }
        }

        private GUIContent GetContentForIsElementActiveToggle()
        {
            var content = new GUIContent();
            content.text = "";
            content.tooltip = "If this is checked, the element wil be taken into acount during object placement. Otherwise, it will be ignored.";

            return content;
        }

        private GUIStyle GetStyleForPrefabPreviewBox()
        {
            var style = new GUIStyle("Box");
            return style;
        }

        private void RenderAlignToSurfaceToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForAlignToSurfaceToggle(), _brushElement.AlignToSurface);
            if(newBool != _brushElement.AlignToSurface)
            {
                UndoEx.RecordForToolAction(_brushElement);
                _brushElement.AlignToSurface = newBool;
            }
        }

        private GUIContent GetContentForAlignToSurfaceToggle()
        {
            var content = new GUIContent();
            content.text = "Align to surface";
            content.tooltip = "If this is checked, objects will be aligned to the surface on which they sit based on the specified alignment axis.";

            return content;
        }

        private void RenderAlignmentAxisSelectionPopup()
        {
            CoordinateSystemAxis newAxis = (CoordinateSystemAxis)EditorGUILayout.EnumPopup(GetContentForAlignmentAxisSelectionPopup(), _brushElement.AlignmentAxis);
            if(newAxis != _brushElement.AlignmentAxis)
            {
                UndoEx.RecordForToolAction(_brushElement);
                _brushElement.AlignmentAxis = newAxis;
            }
        }

        private GUIContent GetContentForAlignmentAxisSelectionPopup()
        {
            var content = new GUIContent();
            content.text = "Alingment axis";
            content.tooltip = "Allows you to specify which of the object's local axes must be aligned with the paint surface normal.";

            return content;
        }

        private void RenderRotationOffsetSlider()
        {
            float newFloat = EditorGUILayout.Slider(GetContentForRotationOffsetSlider(), _brushElement.RotationOffsetInDegrees, DecorPaintObjectPlacementBrushElement.MinRotationOffsetInDegrees, DecorPaintObjectPlacementBrushElement.MaxRotationOffsetInDegrees);
            if(newFloat != _brushElement.RotationOffsetInDegrees)
            {
                UndoEx.RecordForToolAction(_brushElement);
                _brushElement.RotationOffsetInDegrees = newFloat;
            }
        }

        private GUIContent GetContentForRotationOffsetSlider()
        {
            var content = new GUIContent();
            content.text = "Rotation offset";
            content.tooltip = "This is a rotation offset in degrees around the specified alignment axis. " + 
                              "This is useful when the default rotation of the prefab is not satisfactory and can come in handy when stroke alignment " + 
                              "turned on.";

            return content;
        }

        private void RenderMissChanceField()
        {
            float newFloat = EditorGUILayout.Slider(GetContentForMissChanceSlider(), _brushElement.MissChance, DecorPaintObjectPlacementBrushElement.MinMissChance, DecorPaintObjectPlacementBrushElement.MaxMissChance);
            if(newFloat != _brushElement.MissChance)
            {
                UndoEx.RecordForToolAction(_brushElement);
                _brushElement.MissChance = newFloat;
            }
        }

        private GUIContent GetContentForMissChanceSlider()
        {
            var content = new GUIContent();
            content.text = "Miss chance";
            content.tooltip = "Allows you to control the probability that this element will be skipped during painting. The bigger the value, " + 
                              "the less the prefab will be used during a paint operation.";

            return content;
        }

        private void RenderAlignToStrokeToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForAlignToStrokeToggle(), _brushElement.AlignToStroke);
            if(newBool != _brushElement.AlignToStroke)
            {
                UndoEx.RecordForToolAction(_brushElement);
                _brushElement.AlignToStroke = newBool;
            }
        }

        private GUIContent GetContentForAlignToStrokeToggle()
        {
            var content = new GUIContent();
            content.text = "Align to stroke";
            content.tooltip = "If this is checked, the prefab's rotation will be adjusted such that it follows the stroke travel direction. " + 
                              "Note: If rotation randomization is also turned on, a small random rotation offset will also be applied.";

            return content;
        }

        private void RenderRandomizeRotationToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForRandomizeRotationsToggle(), _brushElement.RandomizeRotation);
            if(newBool != _brushElement.RandomizeRotation)
            {
                UndoEx.RecordForToolAction(_brushElement);
                _brushElement.RandomizeRotation = newBool;
            }
        }

        private GUIContent GetContentForRandomizeRotationsToggle()
        {
            var content = new GUIContent();
            content.text = "Randomize rotation";
            content.tooltip = "If this is checked, object rotations will be randomized. Note: The rotations will always happen around the normal of the paint surface.";

            return content;
        }

        private void RenderScaleField()
        {
            float newFloat = EditorGUILayout.FloatField(GetContentForScaleField(), _brushElement.Scale);
            if(newFloat != _brushElement.Scale)
            {
                UndoEx.RecordForToolAction(_brushElement);
                _brushElement.Scale = newFloat;
            }
        }

        private GUIContent GetContentForScaleField()
        {
            var content = new GUIContent();
            content.text = "Scale";
            content.tooltip = "When scale randomization is turned off, you can use this to control the scale of the objects.";

            return content;
        }
        #endregion
    }
}
#endif