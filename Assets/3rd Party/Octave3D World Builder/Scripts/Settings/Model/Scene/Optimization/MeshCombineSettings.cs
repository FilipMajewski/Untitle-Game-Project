#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class MeshCombineSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private bool _combineOnlyStaticMeshes = true;
        [SerializeField]
        private bool _makeCombinedMeshesStatic = true;
        [SerializeField]
        private bool _ignoreInactiveObjects = true;
        [SerializeField]
        private bool _hideWireframe = true;
        [SerializeField]
        private bool _removeEmptyHierarchies = true;

        [SerializeField]
        private bool _weldVertexPositions = false;
        [SerializeField]
        private float _vertexPositionWeldEpsilon = MinVertexPositionWeldEpsilon;
        [SerializeField]
        private bool _weldVertexPositionsOnlyForCommonMaterial = false;

        [SerializeField]
        private bool _attachMeshCollidersToCombinedMeshes = true;
        [SerializeField]
        private bool _useConvexMeshColliders = false;

        [SerializeField]
        private string _nameOfParentObjects = "O3DWB_CmbMeshObj";
        [SerializeField]
        private string _nameOfCombinedMeshes = "O3DWB_CmbMesh";

        [SerializeField]
        private MeshCombineSettingsView _view;
        #endregion

        #region Public Static Properties
        public static float MinVertexPositionWeldEpsilon { get { return 1e-5f; } }
        public static float MaxVertexPositionWeldEpsilon { get { return 1e-1f; } }
        #endregion

        #region Public Properties
        public bool CombineOnlyStaticMeshes { get { return _combineOnlyStaticMeshes; } set { _combineOnlyStaticMeshes = value; } }
        public bool MakeCombinedMeshesStatic { get { return _makeCombinedMeshesStatic; } set { _makeCombinedMeshesStatic = value; } }
        public bool IgnoreInactiveGameObjects { get { return _ignoreInactiveObjects; } set { _ignoreInactiveObjects = value; } }
        public bool HideWireframe { get { return _hideWireframe; } set { _hideWireframe = value; } }
        public bool RemoveEmptyHierarchies { get { return _removeEmptyHierarchies; } set { _removeEmptyHierarchies = value; } }
        public bool WeldVertexPositions { get { return _weldVertexPositions; } set { _weldVertexPositions = value; } }
        public float VertexPositionWeldEpsilon { get { return _vertexPositionWeldEpsilon; } set { _vertexPositionWeldEpsilon = Mathf.Clamp(value, MinVertexPositionWeldEpsilon, MaxVertexPositionWeldEpsilon); } }
        public bool WeldVertexPositionsOnlyForCommonMaterial { get { return _weldVertexPositionsOnlyForCommonMaterial; } set { _weldVertexPositionsOnlyForCommonMaterial = value; } }
        public bool AttachMeshCollidersToCombinedMeshes { get { return _attachMeshCollidersToCombinedMeshes; } set { _attachMeshCollidersToCombinedMeshes = value; } }
        public bool UseConvexMeshColliders { get { return _useConvexMeshColliders; } set { _useConvexMeshColliders = value; } }
        public string NameOfParentObjects { get { return _nameOfParentObjects; } set { if (value != null) _nameOfParentObjects = value; } }
        public string NameOfCombinedMeshes { get { return _nameOfCombinedMeshes; } set { if (value != null) _nameOfCombinedMeshes = value; } }
        public MeshCombineSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public MeshCombineSettings()
        {
            _view = new MeshCombineSettingsView(this);
        }
        #endregion
    }
}
#endif