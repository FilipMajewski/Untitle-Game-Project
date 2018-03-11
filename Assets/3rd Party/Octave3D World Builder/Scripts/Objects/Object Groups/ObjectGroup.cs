#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectGroup : ScriptableObject, INamedEntity
    {
        #region Private Variables
        [SerializeField]
        private string _name;
        [SerializeField]
        private GameObject _groupParent;

        [SerializeField]
        private ObjectGroupView _view;
        #endregion

        #region Public Properties
        public string Name
        { 
            get { return _name; } 
            set 
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _name = value;
                    if (_groupParent != null) _groupParent.name = value;
                }
            } 
        }
        public GameObject GroupParent 
        { 
            get 
            {
                // Note: This seems like the best place to ensure that the object's name is
                //       the same as the group name. Names can differ when the user changes
                //       the name via the GUI and then performs and Undo. Another solution
                //       would be to capture the Undo event and adjust the name there.
                if (_groupParent != null && _groupParent.name != _name) _groupParent.name = _name;
                return _groupParent; 
            } 
            set 
            {
                if (value != null)
                {
                    _groupParent = value;
                    _groupParent.name = Name;
                }
            } 
        }
        public ObjectGroupView View { get { return _view; } }
        #endregion

        #region Constructors
        public ObjectGroup()
        {
            _view = new ObjectGroupView(this);
        }
        #endregion
    }
}
#endif