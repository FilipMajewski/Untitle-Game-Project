#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class ScenePreparationInspectorGUI : InspectorGUI
    {
        #region Public Methods
        public override void Render()
        {
            Octave3DWorldBuilder.Instance.MeshCombiner.View.Render();
        }
        #endregion
    }
}
#endif