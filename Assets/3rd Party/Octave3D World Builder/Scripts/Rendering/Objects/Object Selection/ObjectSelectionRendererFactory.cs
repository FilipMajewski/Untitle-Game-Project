#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public static class ObjectSelectionRendererFactory
    {
        #region Public Static Functions
        public static IObjectSelectionRenderer Create(ObjectSelectionRenderMode objectSelectionRenderMode)
        {
            switch (objectSelectionRenderMode)
            {
                case ObjectSelectionRenderMode.Box:

                    return new BoxObjectSelectionRenderer();

                case ObjectSelectionRenderMode.Rectangle:

                    return new RectangleObjectSelectionRenderer();

                default:

                    return null;
            }
        }
        #endregion
    }
}
#endif