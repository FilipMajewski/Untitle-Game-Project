#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectSnapSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private ObjectSnapSettings _settings;
        #endregion

        #region Constructors
        public ObjectSnapSettingsView(ObjectSnapSettings settings)
        {
            _settings = settings;

            ToggleVisibilityBeforeRender = true;
            VisibilityToggleLabel = "Object Snap Settings";
            SurroundWithBox = true;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderIgnoreObjectsWhenSnappingToggle();
            RenderSnapToCursorHitPointToggle();
            RenderSnapCenterToCenterForXZGridToggle();
            RenderSnapCenterToCenterForObjectSurfaceToggle();

            EditorGUILayout.Separator();
            if (_settings.EnableObjectToObjectSnap) Octave3DWorldBuilder.Instance.ShowGUIHint("When object to object snapping is enabled it is recommened to also check the \'Snap to cursor hit point\' toggle. " + 
                                                                                              "This is not mandatory, but it can produce better snapping results.");
            RenderEnableObjectToObjectSnapToggle();
            Octave3DWorldBuilder.Instance.ShowGUIHint("It is recomenned to use the \'Box\' snap mode when dealing with tiles/blocks/cubes etc. Use \'Vertex\' when dealing with more " +
                                                      "irregular objects.");
            RenderObjectToObjectSnapModeSelectionPopup();
            RenderObjectToObjectSnapEpsilonField();

            EditorGUILayout.Separator();
            RenderXZSnapGridYOffsetField();
            RenderXZGridYOffsetStep();
            _settings.ObjectColliderSnapSurfaceGridSettings.View.Render();
        }
        #endregion

        #region Private Methods
        private void RenderIgnoreObjectsWhenSnappingToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForIgnoreObjectsWhenSnappingToggle(), _settings.IgnoreObjectsWhenSnapping);
            if(newBool != _settings.IgnoreObjectsWhenSnapping)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.IgnoreObjectsWhenSnapping = newBool;
            }
        }

        private GUIContent GetContentForIgnoreObjectsWhenSnappingToggle()
        {
            var content = new GUIContent();
            content.text = "Ignore objects when snapping (pivot point snapping)";
            content.tooltip = "If this is checked, you will no longer be able to snap along the surfaces of other objects.";

            return content;
        }

        private void RenderSnapToCursorHitPointToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForSnapToCursorHitPointToggle(), _settings.SnapToCursorHitPoint);
            if (newBool != _settings.SnapToCursorHitPoint)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.SnapToCursorHitPoint = newBool;
            }
        }

        private GUIContent GetContentForSnapToCursorHitPointToggle()
        {
            var content = new GUIContent();
            content.text = "Snap to cursor hit point";
            content.tooltip = "If this is checked, the active pivot point will be snapped to the intersection point between the mouse cursor and the hovered surface. "
                              + "Note: It is recommended to have this checked when object to object snapping is enabled because it can produce better snapping results.";

            return content;
        }

        private void RenderSnapCenterToCenterForXZGridToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForSnapCenterToCenterForXZGridToggle(), _settings.SnapCenterToCenterForXZGrid);
            if (newBool != _settings.SnapCenterToCenterForXZGrid)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.SnapCenterToCenterForXZGrid = newBool;
            }
        }

        private GUIContent GetContentForSnapCenterToCenterForXZGridToggle()
        {
            var content = new GUIContent();
            content.text = "Snap center to center (grid)";
            content.tooltip = "If this is checked, the tool will always snap the center pivot point to the center of the hovered XZ grid cell.";

            return content;
        }

        private void RenderSnapCenterToCenterForObjectSurfaceToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForSnapCenterToCenterForObjectSurfaceToggle(), _settings.SnapCenterToCenterForObjectSurface);
            if (newBool != _settings.SnapCenterToCenterForObjectSurface)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.SnapCenterToCenterForObjectSurface = newBool;
            }
        }

        private GUIContent GetContentForSnapCenterToCenterForObjectSurfaceToggle()
        {
            var content = new GUIContent();
            content.text = "Snap center to center (object surface)";
            content.tooltip = "If this is checked, the tool will always snap the center pivot point to the center of the hovered object surface.";

            return content;
        }

        private void RenderXZSnapGridYOffsetField()
        {
            float newOffset = EditorGUILayout.FloatField(GetContentForXZSnapGridYOffsetField(), _settings.XZSnapGridYOffset);
            if (newOffset != _settings.XZSnapGridYOffset)
            {
                _settings.XZSnapGridYOffset = newOffset;
                SceneView.RepaintAll();
            }
        }

        private GUIContent GetContentForXZSnapGridYOffsetField()
        {
            var content = new GUIContent();
            content.text = "Snap grid Y offset";
            content.tooltip = "Allows you to control the snap grid's Y offset.";

            return content;
        }

        private void RenderXZGridYOffsetStep()
        {
            float newFloat = EditorGUILayout.FloatField(GetContentForXZGridYOffsetStep(), _settings.XZGridYOffsetStep);
            if(newFloat != _settings.XZGridYOffsetStep)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.XZGridYOffsetStep = newFloat;
            }
        }

        private GUIContent GetContentForXZGridYOffsetStep()
        {
            var content = new GUIContent();
            content.text = "Snap grid Y offset step";
            content.tooltip = "The amount by which the grid Y position is adjusted up or down when using the shortcut keys.";

            return content;
        }

        private void RenderEnableObjectToObjectSnapToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForEnableObjectToObjectSnapToggle(), _settings.EnableObjectToObjectSnap);
            if(newBool != _settings.EnableObjectToObjectSnap)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.EnableObjectToObjectSnap = newBool;
            }
        }

        private GUIContent GetContentForEnableObjectToObjectSnapToggle()
        {
            var content = new GUIContent();
            content.text = "Enable object to object snap";
            content.tooltip = "If this is checked, the tool will snap the object placement guide to nearby objects based on a specified object snap epsilon.";

            return content;
        }

        private void RenderObjectToObjectSnapModeSelectionPopup()
        {
            ObjectToObjectSnapMode newMode = (ObjectToObjectSnapMode)EditorGUILayout.EnumPopup(GetContentForObjectToObjectSnapModeSelectionPopup(), _settings.ObjectToObjectSnapMode);
            if(newMode != _settings.ObjectToObjectSnapMode)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.ObjectToObjectSnapMode = newMode;
            }
        }

        private GUIContent GetContentForObjectToObjectSnapModeSelectionPopup()
        {
            var content = new GUIContent();
            content.text = "Object to object snap mode";
            content.tooltip = "Allows you to choose the way in which object to object snapping is performed. Choose \'Vertex\' when dealing with more irregular objects " + 
                              "and \'Box\' when working with tiles/blocks/cubes.";

            return content;
        }

        private void RenderObjectToObjectSnapEpsilonField()
        {
            float newFloat = EditorGUILayout.FloatField(GetContentForObjectToObjectSnapEpsilonField(), _settings.ObjectToObjectSnapEpsilon);
            if(newFloat != _settings.ObjectToObjectSnapEpsilon)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.ObjectToObjectSnapEpsilon = newFloat;
            }
        }

        private GUIContent GetContentForObjectToObjectSnapEpsilonField()
        {
            var content = new GUIContent();
            content.text = "Object to object snap epsilon";
            content.tooltip = "When object to object snapping is enabled, this value will be used to determine how close object vertices " + 
                              "have to be to one another in order to allow the object to snap.";

            return content;
        }
        #endregion
    }
}
#endif