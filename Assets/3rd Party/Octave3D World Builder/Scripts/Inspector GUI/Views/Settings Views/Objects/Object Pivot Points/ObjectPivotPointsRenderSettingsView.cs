﻿#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectPivotPointsRenderSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private ObjectPivotPointsRenderSettings _settings;
        #endregion

        #region Constructors
        public ObjectPivotPointsRenderSettingsView(ObjectPivotPointsRenderSettings settings)
        {
            _settings = settings;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderShapeTypeSelectionPopup();
            RenderPivotPointSizeField();
            _settings.ProjectedBoxFacePivotPointsRenderSettings.View.Render();
        }
        #endregion

        #region Private Methods     
        private void RenderShapeTypeSelectionPopup()
        {
            #if !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2 && !UNITY_5_3 && !UNITY_5_3_3 && !UNITY_5_3_OR_NEWER
            if (_settings.ShapeType == ObjectPivotPointShapeType.Circle) EditorGUILayout.HelpBox("The \'Circle\' shape type is not supported with this version of Unity (5.0 or above is required). The circles will still " + 
                                                                                                 "be drawn, but they will not be filled.", UnityEditor.MessageType.Info);
            #endif
            ObjectPivotPointShapeType newShapeType = (ObjectPivotPointShapeType)EditorGUILayout.EnumPopup(GetContentForShapeTypeSelectionPopup(), _settings.ShapeType);
            if(newShapeType != _settings.ShapeType)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.ShapeType = newShapeType;

                SceneView.RepaintAll();
            }
        }

        private GUIContent GetContentForShapeTypeSelectionPopup()
        {
            var content = new GUIContent();
            content.text = "Shape type";
            content.tooltip = "Allows you to choose the pivot point shape.";

            return content;
        }

        private void RenderPivotPointSizeField()
        {
            float newFloat = EditorGUILayout.FloatField(GetContentForPivotPointSizeField(), _settings.PivotPointSizeInPixels);
            if(newFloat != _settings.PivotPointSizeInPixels)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.PivotPointSizeInPixels = newFloat;

                SceneView.RepaintAll();
            }
        }

        private GUIContent GetContentForPivotPointSizeField()
        {
            var content = new GUIContent();
            content.text = "Size (pixels)";
            content.tooltip = "Allows you to specify the size in pixels for a pivot point. For a circle, this is the diameter and for a square this represents the square side length.";

            return content;
        }
        #endregion
    }
}
#endif