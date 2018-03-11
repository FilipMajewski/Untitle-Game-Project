#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public class ObjectVertexSnapSessionRenderer
    {
        #region Public Methods
        public void RenderGizmos(ObjectVertexSnapSession session, ObjectVertexSnapSessionRenderSettings renderSettings)
        {
            if (!session.IsActive) return;

            if(session.SourceGameObject != null)
            {
                if (renderSettings.RenderSourceVertex)
                {
                    Vector2 vertexScreenPos = SceneViewCamera.Camera.WorldToScreenPoint(session.SourceVertex);

                    #if !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2 && !UNITY_5_3 && !UNITY_5_3_3 && !UNITY_5_3_OR_NEWER
                    Square2D square = new Square2D(vertexScreenPos, renderSettings.SourceVertexRadiusInPixels * 2.0f);
                    GizmosEx.Render2DFilledSquare(square, renderSettings.SourceVertexFillColor);
                    GizmosEx.Render2DSquareBorderLines(square, renderSettings.SourceVertexBorderColor);
                    #else
                    Circle2D circle = new Circle2D(vertexScreenPos, renderSettings.SourceVertexRadiusInPixels);
                    GizmosEx.Render2DFilledCircle(circle, renderSettings.SourceVertexFillColor);
                    GizmosEx.Render2DCircleBorderLines(circle, renderSettings.SourceVertexBorderColor);
                    #endif
                }
            }

            if((session.DestinationGameObject != null) || session.DestinationGridCell != null)
            {
                if (renderSettings.RenderSnapPosition && session.State == ObjectVertexSnapSessionState.SnapToDestination)
                {
                    Vector2 snapPositionScreenPos = SceneViewCamera.Camera.WorldToScreenPoint(session.SnapPosition);

                    #if !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2 && !UNITY_5_3 && !UNITY_5_3_3 && !UNITY_5_3_OR_NEWER
                    Square2D square = new Square2D(snapPositionScreenPos, renderSettings.SourceVertexRadiusInPixels * 2.0f);
                    GizmosEx.Render2DFilledSquare(square, renderSettings.SourceVertexFillColor);
                    GizmosEx.Render2DSquareBorderLines(square, renderSettings.SourceVertexBorderColor);
                    #else
                    Circle2D circle = new Circle2D(snapPositionScreenPos, renderSettings.SnapPositionRadiusInPixels);
                    GizmosEx.Render2DFilledCircle(circle, renderSettings.SnapPositionFillColor);
                    GizmosEx.Render2DCircleBorderLines(circle, renderSettings.SnapPositionBorderColor);
                    #endif
                }
            }
        }
        #endregion
    }
}
#endif