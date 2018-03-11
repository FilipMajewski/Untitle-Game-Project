#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class PrefabInstantiationSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private PrefabInstantiationSettings _settings;
        #endregion

        #region Constructors
        public PrefabInstantiationSettingsView(PrefabInstantiationSettings settings)
        {
            _settings = settings;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderInstantiatedObjectsAreStaticToggle();
        }
        #endregion

        #region Private Methods
        private void RenderInstantiatedObjectsAreStaticToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForInstantiatedObjectsAreStaticToggle(), _settings.InstantiatedObjectsAreStatic);
            if (newBool != _settings.InstantiatedObjectsAreStatic)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.InstantiatedObjectsAreStatic = newBool;
            }
        }

        private GUIContent GetContentForInstantiatedObjectsAreStaticToggle()
        {
            var content = new GUIContent();
            content.text = "Instantiated objects are static";
            content.tooltip = "If this is checked, objects instantiated from this prefab will be marked as static. Otherwise, " +
                              "they will be marked as dynamic. Note: This property affects the entire hierarchy of the prefab.";

            return content;
        }
        #endregion
    }
}
#endif