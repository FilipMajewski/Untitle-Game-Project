#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class MeshCombinerView : EntityView
    {
        #region Private Variables
        [NonSerialized]
        private MeshCombiner _meshCombiner;
        #endregion

        #region Constructors
        public MeshCombinerView(MeshCombiner meshCombiner)
        {
            _meshCombiner = meshCombiner;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            _meshCombiner.MeshCombineSettings.View.Render();
            RenderCombineMeshesButton();
        }
        #endregion

        #region Private Methods
        private void RenderCombineMeshesButton()
        {
            if(GUILayout.Button(GetContentForCombineMeshesButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth * 0.5f)))
            {
                _meshCombiner.CombineRoots(Octave3DWorldBuilder.Instance.GetImmediateChildrenExcludingPlacementGuide(), Octave3DWorldBuilder.Instance.gameObject.name + " (Optimized)");
            }
        }

        private GUIContent GetContentForCombineMeshesButton()
        {
            var content = new GUIContent();
            content.text = "Combine meshes";
            content.tooltip = "Pressing this button will combine all meshes which share the same material.";

            return content;
        }
        #endregion
    }
}
#endif