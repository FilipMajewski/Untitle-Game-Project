#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class XZGrid : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private XZGridRenderSettings _renderSettings;
        [SerializeField]
        private XZGridCellSizeSettings _cellSizeSettings;
        [SerializeField]
        private XZGridDimensionSettings _dimensionSettings = new XZGridDimensionSettings();

        [SerializeField]
        private RenderableCoordinateSystem _renderableCoordinateSystem = new RenderableCoordinateSystem();

        private XZGridRenderer _renderer = new XZGridRenderer();
        #endregion

        #region Public Static Properties
        public static Vector3 ModelSpaceRightAxis { get { return Vector3.right; } }
        public static Vector3 ModeSpacePlaneNormal { get { return Vector3.up; } }
        public static Vector3 ModelSpaceLookAxis { get { return Vector3.forward; } }

        public static string ModelSpaceRightAxisName { get { return "X"; } }
        public static string ModelSpaceUpAxisName { get { return "Y"; } }
        public static string ModelSpaceLookAxisName { get { return "Z"; } }
        #endregion

        #region Public Properties
        public XZGridRenderSettings RenderSettings
        {
            get
            {
                if (_renderSettings == null) _renderSettings = Octave3DWorldBuilder.Instance.CreateScriptableObject<XZGridRenderSettings>();
                return _renderSettings;
            }
        }
        public XZGridCellSizeSettings CellSizeSettings
        {
            get
            {
                if (_cellSizeSettings == null) _cellSizeSettings = Octave3DWorldBuilder.Instance.CreateScriptableObject<XZGridCellSizeSettings>();
                return _cellSizeSettings;
            }
        }
        public XZGridDimensionSettings DimensionSettings { get { return _dimensionSettings; } }

        public TransformMatrix TransformMatrix { get { return _renderableCoordinateSystem.TransformMatrix; } }
        public RenderableCoordinateSystem RenderableCoordinateSystem { get { return _renderableCoordinateSystem; } }
        public Plane Plane { get { return new Plane(TransformMatrix.GetNormalizedUpAxis(), TransformMatrix.Translation); } }
        public Quaternion Rotation { get { return _renderableCoordinateSystem.GetRotation(); } }
        #endregion

        #region Public Methods
        public void SnapToPoint(Vector3 point)
        {
            Plane gridPlane = Plane;
            float planeDistanceToPoint = gridPlane.GetDistanceToPoint(point);
            SetOriginPosition(GetOriginPosition() + gridPlane.normal * planeDistanceToPoint);
        }

        public void SetTransformMatrix(TransformMatrix transformMatrix)
        {
            _renderableCoordinateSystem.SetTransformMatrix(transformMatrix);
        }

        public void Translate(Vector3 translationAmount)
        {
            _renderableCoordinateSystem.Translate(translationAmount);
        }

        public void SetYOriginPosition(float yOriginPosition)
        {
            Vector3 originPosition = GetOriginPosition();
            originPosition.y = yOriginPosition;
            SetOriginPosition(originPosition);
        }

        public void SetOriginPosition(Vector3 originPosition)
        {
            _renderableCoordinateSystem.SetOriginPosition(originPosition);
        }

        public Vector3 GetOriginPosition()
        {
            return _renderableCoordinateSystem.GetOriginPosition();
        }

        public void SetRotation(Quaternion rotation)
        {
            _renderableCoordinateSystem.SetRotation(rotation);
        }

        public void RenderGizmos()
        {
            _renderer.RenderGizmos(this, SceneViewCamera.Instance.GetViewVolume());
        }

        public XZGridCell GetCellFromPoint(Vector3 point)
        {
            int cellIndexX, cellIndexZ;
            CalculateCellIndicesFromPoint(point, out cellIndexX, out cellIndexZ);
            XZOrientedQuad3D cellQuad = CalculateCellQuad(cellIndexX, cellIndexZ);

            return new XZGridCell(cellIndexX, cellIndexZ, this, cellQuad);
        }

        public List<XZGridCell> GetCellsFromPoints(List<Vector3> points)
        {
            var gridCells = new List<XZGridCell>(points.Count);
            foreach (Vector3 point in points)
            {
                gridCells.Add(GetCellFromPoint(point));
            }

            return gridCells;
        }

        public void CalculateCellIndicesFromPoint(Vector3 point, out int cellIndexX, out int cellIndexZ)
        {
            // Retrieve the point in the grid local space. This makes things easier when caculating the grid indices
            // since only a simple division is required.
            Vector3 gridSpacePoint = TransformMatrix.MultiplyPointInverse(point);

            // Note: Add 0.5 so that the first cell along each dimension has its center set to the grid's origin.
            cellIndexX = Mathf.FloorToInt(gridSpacePoint.x / CellSizeSettings.CellSizeX + 0.5f);
            cellIndexZ = Mathf.FloorToInt(gridSpacePoint.z / CellSizeSettings.CellSizeZ + 0.5f);
        }

        public XZOrientedQuad3D CalculateCellQuad(int cellIndexX, int cellIndexZ)
        {
            Vector3 quadGridSpaceCenter = ModelSpaceRightAxis * CellSizeSettings.CellSizeX * cellIndexX + ModelSpaceLookAxis * CellSizeSettings.CellSizeZ * cellIndexZ;
            Vector2 quadXZSize = new Vector2(CellSizeSettings.CellSizeX, CellSizeSettings.CellSizeZ);

            XZOrientedQuad3D orientedQuad = new XZOrientedQuad3D(quadGridSpaceCenter, quadXZSize);
            orientedQuad.Transform(TransformMatrix);

            return orientedQuad;
        }
        #endregion
    }
}
#endif