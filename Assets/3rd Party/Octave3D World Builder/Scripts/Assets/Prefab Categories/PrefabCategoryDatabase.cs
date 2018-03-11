﻿#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class PrefabCategoryDatabase : ScriptableObject, IMessageListener
    {
        #region Private Variables
        [SerializeField]
        private PrefabCategoryCollection _prefabCategories = new PrefabCategoryCollection();
        #endregion

        #region Public Static Properties
        public static string DefaultPrefabCategoryName { get { return "Default"; } }
        #endregion

        #region Public Properties
        public bool IsEmpty { get { return _prefabCategories.IsEmpty; } }
        public int NumberOfCategories { get { return _prefabCategories.NumberOfEntities; } }
        public PrefabCategory ActivePrefabCategory { get { return _prefabCategories.MarkedEntity; } }
        public int IndexOfActiveCategory { get { return _prefabCategories.IndexOfMarkedEntity; } }
        #endregion

        #region Public Static Functions
        public static PrefabCategoryDatabase Get()
        {
            return Octave3DWorldBuilder.Instance.PrefabCategoryDatabase;
        }
        #endregion

        #region Public Methods
        public PrefabCategory GetDefaultPrefabCategory()
        {
            return GetPrefabCategoryByName(DefaultPrefabCategoryName);
        }

        public PrefabCategory CreatePrefabCategory(string categoryName)
        {
            if(!string.IsNullOrEmpty(categoryName))
            {
                PrefabCategory newPrefabCategory = PrefabCategoryFactory.Create(categoryName, GetAllPrefabCategoryNames());

                _prefabCategories.AddEntity(newPrefabCategory);
                return newPrefabCategory;
            }

            return null;
        }

        public bool ContainsPrefabCategory(PrefabCategory prefabCategory)
        {
            return _prefabCategories.ContainsEntity(prefabCategory);
        }

        public void RemoveAndDestroyPrefabCategory(PrefabCategory prefabCategory)
        {
            if (CanPrefabCategoryBeRemovedFromDatabase(prefabCategory))
            {
                if (ContainsPrefabCategory(prefabCategory))
                {
                    _prefabCategories.RemoveEntity(prefabCategory);
                    PrefabCategoryWasRemovedFromDatabaseMessage.SendToInterestedListeners(prefabCategory);

                    UndoEx.DestroyObjectImmediate(prefabCategory);
                }
            }
        }

        public void RemoveAndDestroyAllPrefabCategories()
        {
            List<PrefabCategory> allPrefabCategories = GetAllPrefabCategories();
            var prefabCategoriesToDestroy = new List<PrefabCategory>();
            foreach (PrefabCategory prefabCategory in allPrefabCategories)
            {
                if (CanPrefabCategoryBeRemovedFromDatabase(prefabCategory))
                {
                    _prefabCategories.RemoveEntity(prefabCategory);
                    PrefabCategoryWasRemovedFromDatabaseMessage.SendToInterestedListeners(prefabCategory);

                    prefabCategoriesToDestroy.Add(prefabCategory);
                }
            }

            foreach (PrefabCategory prefabCategory in prefabCategoriesToDestroy)
            {
                UndoEx.DestroyObjectImmediate(prefabCategory);
            }
        }

        public void RenamePrefabCategory(PrefabCategory prefabCategory, string newName)
        {
            if (ContainsPrefabCategory(prefabCategory)) _prefabCategories.RenameEntity(prefabCategory, newName);
        }

        public void RemoveAndDestroyEmptyPrefabCategories()
        {
            List<PrefabCategory> allPrefabCategories = GetAllPrefabCategories();
            var prefabCategoriesToDestroy = new List<PrefabCategory>();
            foreach (PrefabCategory prefabCategory in allPrefabCategories)
            {
                if (prefabCategory.IsEmpty && CanPrefabCategoryBeRemovedFromDatabase(prefabCategory))
                {
                    _prefabCategories.RemoveEntity(prefabCategory);
                    PrefabCategoryWasRemovedFromDatabaseMessage.SendToInterestedListeners(prefabCategory);

                    prefabCategoriesToDestroy.Add(prefabCategory);
                }
            }

            foreach (PrefabCategory prefabCategory in prefabCategoriesToDestroy)
            {
                UndoEx.DestroyObjectImmediate(prefabCategory);
            }
        }

        public void RemoveAndDestroyAllPrefabsInAllCategories()
        {
            List<PrefabCategory> allPrefabCategories = GetAllPrefabCategories();
            foreach (PrefabCategory prefabCategory in allPrefabCategories)
            {
                prefabCategory.RemoveAndDestroyAllPrefabs();
            }
        }

        public void RemoveNullPrefabEntriesInAllCategories()
        {
            List<PrefabCategory> allPrefabCategories = GetAllPrefabCategories();
            foreach (PrefabCategory prefabCategory in allPrefabCategories)
            {
                prefabCategory.RemoveNullPrefabEntries();
            }
        }

        public PrefabCategory GetPrefabCategoryByIndex(int index)
        {
            return _prefabCategories.GetEntityByIndex(index);
        }

        public PrefabCategory GetPrefabCategoryByName(string name)
        {
            return _prefabCategories.GetEntityByName(name);
        }

        public PrefabCategory GetPrefabCategoryWhichContainsPrefab(GameObject unityPrefab)
        {
            List<PrefabCategory> prefabCategories = _prefabCategories.GetEntitiesByPredicate(item => item.ContainsPrefab(unityPrefab));
            return prefabCategories.Count != 0 ? prefabCategories[0] : null;
        }

        public PrefabCategory GetPrefabCategoryWhichContainsPrefab(Prefab prefab)
        {
            List<PrefabCategory> prefabCategories = _prefabCategories.GetEntitiesByPredicate(item => item.ContainsPrefab(prefab));
            return prefabCategories.Count != 0 ? prefabCategories[0] : null;
        }

        public bool IsThereCategoryWhichContainsPrefab(GameObject unityPrefab)
        {
            PrefabCategory prefabCategory = GetPrefabCategoryWhichContainsPrefab(unityPrefab);
            return prefabCategory != null;
        }

        public bool IsThereCategoryWhichContainsPrefab(Prefab prefab)
        {
            PrefabCategory prefabCategory = GetPrefabCategoryWhichContainsPrefab(prefab);
            return prefabCategory != null;
        }

        public List<PrefabCategory> GetAllPrefabCategories()
        {
            return _prefabCategories.GetAllEntities();
        }

        public List<string> GetAllPrefabCategoryNames()
        {
            return _prefabCategories.GetAllEntityNames();
        }

        public void SetActivePrefabCategory(PrefabCategory newActivePrefabCategory)
        {
            if (newActivePrefabCategory == null || !ContainsPrefabCategory(newActivePrefabCategory)) return;

            _prefabCategories.MarkEntity(newActivePrefabCategory);
            NewPrefabCategoryWasActivatedMessage.SendToInterestedListeners(newActivePrefabCategory);
        }

        public bool CanPrefabCategoryBeRenamed(PrefabCategory prefabCategory)
        {
            if (prefabCategory.Name == DefaultPrefabCategoryName) return false;
            return true;
        }

        public bool CanPrefabCategoryBeRemovedFromDatabase(PrefabCategory prefabCategory)
        {
            if (prefabCategory.Name == DefaultPrefabCategoryName) return false;
            return true;
        }

        public void RecordAllPrefabCategoriesForUndo()
        {
            List<PrefabCategory> allPrefabCategories = GetAllPrefabCategories();
            foreach(PrefabCategory prefabCategory in allPrefabCategories)
            {
                UndoEx.RecordForToolAction(prefabCategory);
            }
        }

        public void ActivateNextPrefabCategory()
        {
            if(NumberOfCategories == 0) return;
            int indexOfActiveCategory = IndexOfActiveCategory;

            ++indexOfActiveCategory;
            if (indexOfActiveCategory >= NumberOfCategories) indexOfActiveCategory = NumberOfCategories - 1;
            SetActivePrefabCategory(GetPrefabCategoryByIndex(indexOfActiveCategory));
        }

        public void ActivatePreviousPrefabCategory()
        {
            if (NumberOfCategories == 0) return;
            int indexOfActiveCategory = IndexOfActiveCategory;

            --indexOfActiveCategory;
            if (indexOfActiveCategory < 0) indexOfActiveCategory = 0;
            SetActivePrefabCategory(GetPrefabCategoryByIndex(indexOfActiveCategory));
        }
        #endregion

        #region Message Handlers
        public void RespondToMessage(Message message)
        {
            switch(message.Type)
            {
                case MessageType.PrefabTagWasRemovedFromDatabase:

                    RespondToMessage(message as PrefabTagWasRemovedFromDatabaseMessage);
                    break;
            }
        }

        private void RespondToMessage(PrefabTagWasRemovedFromDatabaseMessage message)
        {
            List<PrefabCategory> allPrefabCategories = _prefabCategories.GetAllEntities();
            foreach (PrefabCategory prefabCategory in allPrefabCategories)
            {
                List<Prefab> allPrefabsInCategory = prefabCategory.GetAllPrefabs();
                foreach (Prefab prefab in allPrefabsInCategory)
                {
                    prefab.TagAssociations.RemoveNullEntries();
                }
            }
        }
        #endregion

        #region Private Methods
        private void OnEnable()
        {
            CreateDefaultCategoryIfNoCategoryExists();
            MessageListenerRegistration.PerformRegistrationForPrefabCategoryDatabase(this);
        }

        private void CreateDefaultCategoryIfNoCategoryExists()
        {
            if (IsEmpty)
            {
                CreatePrefabCategory(PrefabCategoryDatabase.DefaultPrefabCategoryName);
                SetActivePrefabCategory(_prefabCategories.GetEntityByIndex(0));
            }
        }
        #endregion
    }
}
#endif