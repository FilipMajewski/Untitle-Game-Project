#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class ObjectGroupDatabase : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private ObjectGroupCollection _objectGroups = new ObjectGroupCollection();

        [SerializeField]
        private bool _preserveGroupChildren = true;

        [SerializeField]
        private ObjectGroupDatabaseView _view;
        #endregion

        #region Public Properties
        public bool IsEmpty { get { return _objectGroups.IsEmpty; } }
        public int NumberOfGroups { get { return _objectGroups.NumberOfEntities; } }
        public ObjectGroup ActiveGroup { get { return _objectGroups.MarkedEntity; } }
        public int IndexOfActiveGroup { get { return _objectGroups.IndexOfMarkedEntity; } }
        public ObjectGroupDatabaseView View { get { return _view; } }
        public bool PreserveGroupChildren { get { return _preserveGroupChildren; } set { _preserveGroupChildren = value; } }
        #endregion

        #region Constructors
        public ObjectGroupDatabase()
        {
            _view = new ObjectGroupDatabaseView(this);
        }
        #endregion

        #region Public Methods
        public ObjectGroup CreateObjectGroup(string groupName)
        {
            if (!string.IsNullOrEmpty(groupName))
            {
                ObjectGroup newObjectGroup = ObjectGroupFactory.Create(groupName, GetAllObjectGroupNames());

                _objectGroups.AddEntity(newObjectGroup);
                if (NumberOfGroups == 1) SetActiveObjectGroup(newObjectGroup);

                return newObjectGroup;
            }

            return null;
        }

        public bool ContainsObjectGroup(GameObject groupParent)
        {
            return _objectGroups.ContainsEntityByPredicate(item => item.GroupParent == groupParent);
        }

        public bool ContainsObjectGroup(ObjectGroup objectGroup)
        {
            return _objectGroups.ContainsEntity(objectGroup);
        }

        public void RenameObjectGroup(ObjectGroup objectGroup, string newName)
        {
            if (ContainsObjectGroup(objectGroup)) _objectGroups.RenameEntity(objectGroup, newName);
        }

        public void RemoveAndDestroyObjectGroup(ObjectGroup objectGroup)
        {
            if (ContainsObjectGroup(objectGroup))
            {
                _objectGroups.RemoveEntity(objectGroup);

                GameObject groupParent = objectGroup.GroupParent;
                if (_preserveGroupChildren) groupParent.MoveImmediateChildrenUpOneLevel(true);
                if (groupParent != null) UndoEx.DestroyObjectImmediate(groupParent);
                UndoEx.DestroyObjectImmediate(objectGroup);
            }
        }

        public void RemoveAndDestroyAllObjectGroups()
        {
            List<ObjectGroup> allGroups = GetAllObjectGroups();
            var groupsToDestroy = new List<ObjectGroup>();
            foreach (ObjectGroup objectGroup in allGroups)
            {
                _objectGroups.RemoveEntity(objectGroup);
                groupsToDestroy.Add(objectGroup);
            }

            foreach (ObjectGroup objectGroup in allGroups)
            {
                GameObject groupParent = objectGroup.GroupParent;
                if (_preserveGroupChildren) groupParent.MoveImmediateChildrenUpOneLevel(true);
                if (groupParent != null) UndoEx.DestroyObjectImmediate(groupParent);
                UndoEx.DestroyObjectImmediate(objectGroup);
            }
        }

        public List<ObjectGroup> GetAllObjectGroups()
        {
            return _objectGroups.GetAllEntities();
        }

        public ObjectGroup GetObjectGroupByIndex(int groupIndex)
        {
            return _objectGroups.GetEntityByIndex(groupIndex);
        }

        public List<string> GetAllObjectGroupNames()
        {
            return _objectGroups.GetAllEntityNames();
        }

        public void SetActiveObjectGroup(ObjectGroup newActiveObjectGroup)
        {
            if (newActiveObjectGroup != null && !ContainsObjectGroup(newActiveObjectGroup)) return;

            _objectGroups.MarkEntity(newActiveObjectGroup);
        }

        public void RemoveGroupsWithNullParents()
        {
            _objectGroups.RemoveWithPredicate(item => item.GroupParent == null);

            if (!IsEmpty)
            {
                if (IndexOfActiveGroup < 0 || IndexOfActiveGroup >= NumberOfGroups) SetActiveObjectGroup(GetObjectGroupByIndex(0));
            }
            else SetActiveObjectGroup(null);
        }
        #endregion
    }
}
#endif