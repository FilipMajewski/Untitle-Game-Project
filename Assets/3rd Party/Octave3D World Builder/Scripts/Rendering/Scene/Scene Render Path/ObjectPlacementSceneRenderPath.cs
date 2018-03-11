#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public class ObjectPlacementSceneRenderPath : SceneRenderPath
    {
        #region Public Methods
        public override void RenderGizmos()
        {
            ObjectPlacement.Get().RenderGizmos();
        }

        public override void RenderHandles()
        {
            ObjectPlacement.Get().RenderHandles();
        }
        #endregion
    }
}
#endif