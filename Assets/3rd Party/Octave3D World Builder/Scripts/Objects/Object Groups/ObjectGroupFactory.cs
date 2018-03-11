#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public static class ObjectGroupFactory
    {
        #region Public Static Functions
        public static ObjectGroup Create(string name, List<string> existingGroupNames)
        {
            if (!string.IsNullOrEmpty(name)) return CreateNewObjectGroupWithUniqueName(name, existingGroupNames);
            else
            {
                Debug.LogWarning("Null or empty object group names are not allowed. Please specify a valid object group name.");
                return null;
            }
        }
        #endregion

        #region Private Static Functions
        private static ObjectGroup CreateNewObjectGroupWithUniqueName(string name, List<string> existingGroupNames)
        {
            GameObject groupParent = new GameObject();
            UndoEx.RegisterCreatedGameObject(groupParent);
            groupParent.transform.parent = Octave3DWorldBuilder.Instance.transform;

            ObjectGroup newObjectGroup = Octave3DWorldBuilder.Instance.CreateScriptableObject<ObjectGroup>();
            newObjectGroup.Name = UniqueEntityNameGenerator.GenerateUniqueName(name, existingGroupNames);
            newObjectGroup.GroupParent = groupParent;

            return newObjectGroup;
        }
        #endregion
    }
}
#endif