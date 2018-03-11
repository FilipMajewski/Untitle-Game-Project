#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace O3DWB
{
    public class ToolSupervisor
    {
        #region Constructors
        public ToolSupervisor()
        {
            EditorApplication.projectWindowChanged -= RemoveNullPrefabReferences;
            EditorApplication.projectWindowChanged += RemoveNullPrefabReferences;
        }
        #endregion

        #region Public Static Functions
        public static ToolSupervisor Get()
        {
            return Octave3DWorldBuilder.Instance.ToolSupervisor;
        }
        #endregion

        #region Public Methods
        public void Supervise()
        {
            RemoveInvalidEntityRefrences();
        }
        #endregion

        #region Private Methods
        private void RemoveInvalidEntityRefrences()
        {
            RemoveNullGameObjectReferences();
            Octave3DWorldBuilder.Instance.RemoveNullScriptableObjects();
        }

        private void RemoveNullGameObjectReferences()
        {
            ObjectSelection.Get().RemoveNullGameObjectEntries();
            Octave3DWorldBuilder.Instance.PlacementObjectGroupDatabase.RemoveGroupsWithNullParents();
            ObjectSnapping.Get().ObjectSnapMask.RemoveInvalidEntries();
            DecorPaintObjectPlacement.Get().DecorPaintMask.RemoveInvalidEntries();

            if(PrefabTagDatabase.Get().ContainsNullEntries())
            {
                Debug.Log("Detected null prefab tag references. This bug has been fixed and should never happen. If you are reading this, please contact me.");

                List<PrefabCategory> allPrefabCategories = PrefabCategoryDatabase.Get().GetAllPrefabCategories();
                foreach(var category in allPrefabCategories)
                {
                    List<Prefab> allPrefabsInCategory = category.GetAllPrefabs();
                    foreach(var prefab in allPrefabsInCategory)
                    {
                        prefab.TagAssociations.RemoveNullEntries();
                    }
                }

                PrefabTagDatabase.Get().RemoveNullEntries();
            }
        }

        private void RemoveNullPrefabReferences()
        {
            PrefabPreviewTextureCache.Get().DestroyTexturesForNullPrefabEntries();
            PrefabCategoryDatabase.Get().RemoveNullPrefabEntriesInAllCategories();
        }
        #endregion
    }
}
#endif