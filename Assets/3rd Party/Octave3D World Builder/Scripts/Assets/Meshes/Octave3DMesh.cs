#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class Octave3DMesh
    {
        #region Private Variables
        [SerializeField]
        private Mesh _mesh;
        [SerializeField]
        private Vector3[] _vertexPositions;
        [SerializeField]
        private int[] _vertexIndices;
        [SerializeField]
        private int _numberOfTriangles;

        [NonSerialized]
        private MeshSphereTree _meshSphereTree;
        #endregion

        #region Public Properties
        public Mesh Mesh { get { return _mesh; } }
        public int NumberOfTriangles { get { return _numberOfTriangles; } }
        public Vector3[] VertexPositions { get { return _vertexPositions.Clone() as Vector3[]; } }
        public int[] VertexIndices { get { return _vertexIndices.Clone() as int[]; } }
        #endregion

        #region Constructors
        public Octave3DMesh()
        {
            _meshSphereTree = new MeshSphereTree(this);
        }

        public Octave3DMesh(Mesh mesh)
        {
            _mesh = mesh;
            _vertexPositions = _mesh.vertices;
            _vertexIndices = _mesh.triangles;
            _numberOfTriangles = (int)(_vertexIndices.Length / 3);

            _meshSphereTree = new MeshSphereTree(this);
        }
        #endregion

        #region Public Methods
        public Box GetBox()
        {
            if (_mesh == null) return Box.GetInvalid();
            return new Box(_mesh.bounds);
        }

        public OrientedBox GetOrientedBox(TransformMatrix transformMatrix)
        {
            if (_mesh == null) return OrientedBox.GetInvalid();

            OrientedBox orientedBox = new OrientedBox(GetBox());
            orientedBox.Transform(transformMatrix);

            return orientedBox;
        }

        public void RenderGizmosDebug(TransformMatrix meshTransformMatrix)
        {
            _meshSphereTree.RenderGizmosDebug(meshTransformMatrix);
        }

        public Triangle3D GetTriangle(int triangleIndex)
        {
            int baseIndex = triangleIndex * 3;
            return new Triangle3D(_vertexPositions[_vertexIndices[baseIndex]], _vertexPositions[_vertexIndices[baseIndex + 1]], _vertexPositions[_vertexIndices[baseIndex + 2]]);
        }

        public MeshRayHit Raycast(Ray ray, TransformMatrix meshTransformMatrix)
        {
            // Note: I can't think of a situation in which negative scale would be useful,
            //       so we're going to set the scale to a positive value.
            //meshTransformMatrix.Scale = meshTransformMatrix.Scale.GetVectorWithPositiveComponents();
            return _meshSphereTree.Raycast(ray, meshTransformMatrix);
        }

        public List<Vector3> GetOverlappedWorldVerts(Box box, TransformMatrix meshTransformMatrix)
        {
            return _meshSphereTree.GetOverlappedWorldVerts(box.ToOrientedBox(), meshTransformMatrix);
        }
        #endregion
    }
}
#endif