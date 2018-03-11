#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class RectangleObjectSelectionRenderer : ObjectSelectionRenderer
    {
        #region Public Methods
        public override void Render(List<GameObject> selectedObjects)
        {
            ObjectSelectionRenderSettings selectionRenderSettings = ObjectSelectionRenderSettings.Get();
            ObjectSelectionRectangleRenderModeSettings selectionRectangleRenderModeSettings = selectionRenderSettings.RectangleRenderModeSettings;

            SceneViewCamera sceneViewCamera = SceneViewCamera.Instance;
            Camera camera = SceneViewCamera.Camera;
            Color rectangleFillColor = selectionRectangleRenderModeSettings.RectangleFillColor;
            Color rectangleBorderColor = selectionRectangleRenderModeSettings.RectangleBorderColor;

            bool renderRectangleInteriors = rectangleFillColor.a != 0.0f;
            bool renderBorders = rectangleBorderColor.a != 0.0f;
            if (!renderBorders && !renderRectangleInteriors) return;

            if(renderRectangleInteriors && renderBorders)
            {
                foreach (GameObject gameObject in selectedObjects)
                {
                    if (!gameObject.activeInHierarchy) continue;
                    if (!sceneViewCamera.IsGameObjectVisible(gameObject)) continue;

                    Rect screenRectangle = gameObject.GetScreenRectangle(camera);
                    GizmosEx.Render2DFilledRectangle(screenRectangle, rectangleFillColor);
                    GizmosEx.Render2DRectangleBorderLines(screenRectangle, rectangleBorderColor);
                }
            }
            else
            if(renderRectangleInteriors && !renderBorders)
            {
                foreach (GameObject gameObject in selectedObjects)
                {
                    if (!gameObject.activeInHierarchy) continue;
                    if (!sceneViewCamera.IsGameObjectVisible(gameObject)) continue;

                    Rect screenRectangle = gameObject.GetScreenRectangle(camera);
                    GizmosEx.Render2DFilledRectangle(screenRectangle, rectangleFillColor);
                }
            }
            else
            if(renderBorders && !renderRectangleInteriors)
            {
                foreach (GameObject gameObject in selectedObjects)
                {
                    if (!gameObject.activeInHierarchy) continue;
                    if (!sceneViewCamera.IsGameObjectVisible(gameObject)) continue;

                    Rect screenRectangle = gameObject.GetScreenRectangle(camera);
                    GizmosEx.Render2DRectangleBorderLines(screenRectangle, rectangleBorderColor);
                }
            }
        }
        #endregion
    }
}
#endif