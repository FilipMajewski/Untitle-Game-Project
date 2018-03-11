#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private bool _attachPlacedObjectsToActiveObjectGroup = false;
        [SerializeField]
        private ObjectPlacementMode _objectPlacementMode = ObjectPlacementMode.PointAndClick;
        [SerializeField]
        private bool _makePlacedObjectsChildrenOfHoveredObject = false;
        [SerializeField]
        private ObjectIntersectionSettings _objectIntersectionSettings;
        [SerializeField]
        private ObjectPlacementGuideSettings _objectPlacementGuideSettings;

        [SerializeField]
        private DecorPaintObjectPlacementSettings _decorPaintObjectPlacementSettings;
        [SerializeField]
        private PointAndClickObjectPlacementSettings _pointAndClickPlacementSettings;
        [SerializeField]
        private PathObjectPlacementSettings _pathPlacementSettings;
        [SerializeField]
        private BlockObjectPlacementSettings _blockObjectPlacementSettings;

        [SerializeField]
        private ObjectPlacementSettingsView _view;
        #endregion

        #region Public Properties
        public bool AttachPlacedObjectsToActiveObjectGroup { get { return _attachPlacedObjectsToActiveObjectGroup; } set { _attachPlacedObjectsToActiveObjectGroup = value; } }
        public ObjectPlacementMode ObjectPlacementMode 
        { 
            get { return _objectPlacementMode; } 
            set
            {
                _objectPlacementMode = value;
                ObjectPlacementModeWasChangedMessage.SendToInterestedListeners(_objectPlacementMode);
            }
        }
        public bool MakePlacedObjectsChildrenOfHoveredObject { get { return _makePlacedObjectsChildrenOfHoveredObject; } set { _makePlacedObjectsChildrenOfHoveredObject = value; } }
        public ObjectIntersectionSettings ObjectIntersectionSettings
        {
            get
            {
                if (_objectIntersectionSettings == null) _objectIntersectionSettings = Octave3DWorldBuilder.Instance.CreateScriptableObject<ObjectIntersectionSettings>();
                return _objectIntersectionSettings;
            }
        }
        public ObjectPlacementGuideSettings ObjectPlacementGuideSettings
        {
            get
            {
                if (_objectPlacementGuideSettings == null) _objectPlacementGuideSettings = Octave3DWorldBuilder.Instance.CreateScriptableObject<ObjectPlacementGuideSettings>();
                return _objectPlacementGuideSettings;
            }
        }
        public DecorPaintObjectPlacementSettings DecorPaintObjectPlacementSettings
        {
            get
            {
                if (_decorPaintObjectPlacementSettings == null) _decorPaintObjectPlacementSettings = Octave3DWorldBuilder.Instance.CreateScriptableObject<DecorPaintObjectPlacementSettings>();
                return _decorPaintObjectPlacementSettings;
            }
        }
        public PointAndClickObjectPlacementSettings PointAndClickPlacementSettings
        {
            get
            {
                if (_pointAndClickPlacementSettings == null) _pointAndClickPlacementSettings = Octave3DWorldBuilder.Instance.CreateScriptableObject<PointAndClickObjectPlacementSettings>();
                return _pointAndClickPlacementSettings;
            }
        }
        public PathObjectPlacementSettings PathPlacementSettings
        {
            get
            {
                if (_pathPlacementSettings == null) _pathPlacementSettings = Octave3DWorldBuilder.Instance.CreateScriptableObject<PathObjectPlacementSettings>();
                return _pathPlacementSettings;
            }
        }
        public BlockObjectPlacementSettings BlockObjectPlacementSettings
        {
            get
            {
                if (_blockObjectPlacementSettings == null) _blockObjectPlacementSettings = Octave3DWorldBuilder.Instance.CreateScriptableObject<BlockObjectPlacementSettings>();
                return _blockObjectPlacementSettings;
            }
        }

        public ObjectPlacementSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public ObjectPlacementSettings()
        {
            _view = new ObjectPlacementSettingsView(this);
        }
        #endregion

        #region Public Static Functions
        public static ObjectPlacementSettings Get()
        {
            return ObjectPlacement.Get().Settings;
        }
        #endregion
    }
}
#endif