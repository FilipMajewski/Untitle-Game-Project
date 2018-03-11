using UnityEngine;

#if UNITY_EDITOR
namespace O3DWB
{
    public class XZGridRenderer
    {
        #region Public Methods
        public void RenderGizmos(XZGrid grid, CameraViewVolume cameraViewVolume)
        {
            if (!grid.RenderSettings.IsVisible) return;

            var visibleCellRangeCalculator = new XZVisibleGridCellRangeCalculator();
            XZVisibleGridCellRange visibleCellRange = visibleCellRangeCalculator.Calculate(grid, cameraViewVolume);

            GizmosMatrix.Push(grid.TransformMatrix.ToMatrix4x4x);
            RenderGridPlane(grid, visibleCellRange);
            RenderGridCellLines(grid, visibleCellRange);
            grid.RenderableCoordinateSystem.RenderGizmos();
            GizmosMatrix.Pop();
        }
        #endregion

        #region Private Methods
        private void RenderGridPlane(XZGrid grid, XZVisibleGridCellRange visibleCellRange)
        {
            int minCellIndexX = visibleCellRange.XAxisVisibleCellRange.Min;
            int maxCellIndexX = visibleCellRange.XAxisVisibleCellRange.Max;

            int minCellIndexZ = visibleCellRange.ZAxisVisibleCellRange.Min;
            int maxCellIndexZ = visibleCellRange.ZAxisVisibleCellRange.Max;

            float numberOfCellsOnX = maxCellIndexX - minCellIndexX + 1.0f;
            float numberOfCellsOnZ = maxCellIndexZ - minCellIndexZ + 1.0f;

            XZGridCellSizeSettings gridCellSizeSettings = grid.CellSizeSettings;
            Vector3 planeCenter = (XZGrid.ModelSpaceRightAxis * (minCellIndexX * gridCellSizeSettings.CellSizeX) + XZGrid.ModelSpaceLookAxis * (minCellIndexZ * gridCellSizeSettings.CellSizeZ) +
                                   XZGrid.ModelSpaceRightAxis * (maxCellIndexX * gridCellSizeSettings.CellSizeX) + XZGrid.ModelSpaceLookAxis * (maxCellIndexZ * gridCellSizeSettings.CellSizeZ)) * 0.5f;
            Vector3 planeSize = new Vector3(numberOfCellsOnX * gridCellSizeSettings.CellSizeX, 0.0f, numberOfCellsOnZ * gridCellSizeSettings.CellSizeZ);

            GizmosColor.Push(grid.RenderSettings.PlaneColor);
            Gizmos.DrawCube(planeCenter, planeSize);
            GizmosColor.Pop();
        }

        private void RenderGridCellLines(XZGrid grid, XZVisibleGridCellRange visibleCellRange)
        {
            int minCellIndexX = visibleCellRange.XAxisVisibleCellRange.Min;
            int maxCellIndexX = visibleCellRange.XAxisVisibleCellRange.Max;
            float numberOfCellsOnX = maxCellIndexX - minCellIndexX + 1.0f;

            int minCellIndexZ = visibleCellRange.ZAxisVisibleCellRange.Min;
            int maxCellIndexZ = visibleCellRange.ZAxisVisibleCellRange.Max;
            float numberOfCellsOnZ = maxCellIndexZ - minCellIndexZ + 1.0f;

            XZGridCellSizeSettings gridCellSizeSettings = grid.CellSizeSettings;
            float halfCellSizeX = gridCellSizeSettings.CellSizeX * 0.5f;
            float halfCellSizeZ = gridCellSizeSettings.CellSizeZ * 0.5f;
          
            XZGridRenderSettings gridRenderSettings = grid.RenderSettings;

            Vector3 startPointOnX, startPointOnZ;
            Vector3 lineCubeSize = Vector3.zero;
  
            // Render the grid lines which extend along the grid's X axis
            GizmosColor.Push(grid.RenderSettings.CellLineColor);
            startPointOnX = XZGrid.ModelSpaceRightAxis * (minCellIndexX * gridCellSizeSettings.CellSizeX - halfCellSizeX);
            lineCubeSize.z = gridRenderSettings.CellLineThickness;
            int maxLineIndex = maxCellIndexZ + 1;
            for (int lineIndex = minCellIndexZ; lineIndex <= maxLineIndex; ++lineIndex)
            {
                startPointOnZ = XZGrid.ModelSpaceLookAxis * (lineIndex * gridCellSizeSettings.CellSizeZ - halfCellSizeZ);
                Vector3 firstPoint = startPointOnX + startPointOnZ;
                Vector3 secondPoint = firstPoint + XZGrid.ModelSpaceRightAxis * (numberOfCellsOnX * gridCellSizeSettings.CellSizeX);

                lineCubeSize.x = (firstPoint - secondPoint).magnitude + gridRenderSettings.CellLineThickness;
                Gizmos.DrawCube((firstPoint + secondPoint) * 0.5f, lineCubeSize);
            }

            // Render the grid lines which extend along the grid's Z axis
            startPointOnZ = XZGrid.ModelSpaceLookAxis * (minCellIndexZ * gridCellSizeSettings.CellSizeZ - halfCellSizeZ);
            lineCubeSize.x = gridRenderSettings.CellLineThickness;
            maxLineIndex = maxCellIndexX + 1;
            for (int lineIndex = minCellIndexX; lineIndex <= maxLineIndex; ++lineIndex)
            {
                startPointOnX = XZGrid.ModelSpaceRightAxis * (lineIndex * gridCellSizeSettings.CellSizeX - halfCellSizeX);
                Vector3 firstPoint = startPointOnX + startPointOnZ;
                Vector3 secondPoint = firstPoint + XZGrid.ModelSpaceLookAxis * (numberOfCellsOnZ * gridCellSizeSettings.CellSizeZ);

                lineCubeSize.z = (firstPoint - secondPoint).magnitude;
                Gizmos.DrawCube((firstPoint + secondPoint) * 0.5f, lineCubeSize);
            }
            GizmosColor.Pop();
        }
        #endregion
    }
}
#endif