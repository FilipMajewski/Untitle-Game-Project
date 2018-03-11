#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public class SceneRenderer
    {
        #region Public Methods
        public void RenderGizmos()
        {
            SceneRenderPathType sceneRenderPathType = InspectorGUIIdentifiers.GetSceneRenderPathTypeFromIdentifier(Octave3DWorldBuilder.Instance.Inspector.ActiveInspectorGUIIdentifier);
            SceneRenderPathFactory.Create(sceneRenderPathType).RenderGizmos();
        }

        public void RenderHandles()
        {
            SceneRenderPathType sceneRenderPathType = InspectorGUIIdentifiers.GetSceneRenderPathTypeFromIdentifier(Octave3DWorldBuilder.Instance.Inspector.ActiveInspectorGUIIdentifier);
            SceneRenderPathFactory.Create(sceneRenderPathType).RenderHandles();
        }
        #endregion
    }
}
#endif