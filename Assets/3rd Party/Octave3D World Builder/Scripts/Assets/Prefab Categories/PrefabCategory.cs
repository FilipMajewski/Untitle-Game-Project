﻿#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class PrefabCategory : ScriptableObject, INamedEntity
    {
        #region Private Variables
        [SerializeField]
        private PrefabCollection _prefabs = new PrefabCollection();
        [SerializeField]
        private PrefabFilter _prefabFilter;

        [SerializeField]
        private string _name;
        #endregion

        #region Public Properties
        public bool IsEmpty { get { return _prefabs.IsEmpty; } }
        public int NumberOfPrefabs { get { return _prefabs.NumberOfEntities; } }
        public Prefab ActivePrefab { get { return _prefabs.MarkedEntity; } }
        public int IndexOfActivePrefab { get { return _prefabs.IndexOfMarkedEntity; } }
        public string Name { get { return _name; } set { if (!string.IsNullOrEmpty(value)) _name = value; } }
        public PrefabFilter PrefabFilter
        {
            get
            {
                if (_prefabFilter == null) _prefabFilter = Octave3DWorldBuilder.Instance.CreateScriptableObject<PrefabFilter>();
                return _prefabFilter;
            }
        }
        #endregion

        #region Public Static Delegates
        public delegate bool PrefabActivationValidationDelegate(Prefab prefabToActivate);
        public static PrefabActivationValidationDelegate PrefabActivationValidationCallback;
        #endregion

        #region Public Methods
        public void AddPrefab(Prefab prefab)
        {
            if (!ContainsPrefab(prefab) && !ContainsPrefab(prefab.UnityPrefab))
            {
                if (!PrefabCategoryDatabase.Get().IsThereCategoryWhichContainsPrefab(prefab)) _prefabs.AddEntity(prefab);
                else
                {
                    PrefabCategory categoryWhichContainsPrefab = PrefabCategoryDatabase.Get().GetPrefabCategoryWhichContainsPrefab(prefab);
                    LogPrefabAlreadyExistsInCategoryMessage(categoryWhichContainsPrefab.Name);
                }
            }
        }

        public void AddPrefab(GameObject unityPrefab)
        {
            if (!ContainsPrefab(unityPrefab))
            {
                if (!PrefabCategoryDatabase.Get().IsThereCategoryWhichContainsPrefab(unityPrefab))
                {
                    Prefab prefab = PrefabFactory.Create(unityPrefab);
                    _prefabs.AddEntity(prefab);
                }
                else
                {
                    PrefabCategory categoryWhichContainsPrefab = PrefabCategoryDatabase.Get().GetPrefabCategoryWhichContainsPrefab(unityPrefab);
                    LogPrefabAlreadyExistsInCategoryMessage(categoryWhichContainsPrefab.Name);
                }
            }
        }

        public void AddPrefabCollection(List<Prefab> prefabs)
        {
            foreach (Prefab prefab in prefabs)
            {
                AddPrefab(prefab);
            }
        }

        public void AddPrefabCollection(List<GameObject> unityPrefabs)
        {
            foreach (GameObject unityPrefab in unityPrefabs)
            {
                AddPrefab(unityPrefab);
            }
        }

        public bool ContainsPrefab(Prefab prefab)
        {
            if (prefab == null) return false;
            return _prefabs.ContainsEntity(prefab) || ContainsPrefab(prefab.UnityPrefab);
        }

        public bool ContainsPrefab(GameObject unityPrefab)
        {
            return _prefabs.GetEntityByPredicate(item => item.UnityPrefab == unityPrefab) != null;
        }

        public void TransferPrefabToCategory(Prefab prefabToTransfer, PrefabCategory destinationCategory)
        {
            if (destinationCategory == this) return;

            if (ContainsPrefab(prefabToTransfer))
            {
                _prefabs.RemoveEntity(prefabToTransfer);
                destinationCategory.AddPrefab(prefabToTransfer);

                PrefabWasTransferredToCategoryMessage.SendToInterestedListeners(prefabToTransfer, this, destinationCategory);
            }
        }

        public void TransferAllPrefabsToCategory(PrefabCategory destinationCategory)
        {
            if (destinationCategory == this) return;

            List<Prefab> allPrefabs = GetAllPrefabs();
            foreach (Prefab prefab in allPrefabs)
            {
                TransferPrefabToCategory(prefab, destinationCategory);
            }
        }

        public void TransferPrefabCollectionToCategory(List<Prefab> prefabsCollection, PrefabCategory destinationCategory)
        {
            if (destinationCategory == this) return;

            foreach (Prefab prefab in prefabsCollection)
            {
                TransferPrefabToCategory(prefab, destinationCategory);
            }
        }

        public void RemoveAndDestroyPrefab(Prefab prefab)
        {
            if (ContainsPrefab(prefab))
            {
                _prefabs.RemoveEntity(prefab);
                EnsureActivePrefabPassesPrefabFilter();

                PrefabWasRemovedFromCategoryMessage.SendToInterestedListeners(this, prefab);
                UndoEx.DestroyObjectImmediate(prefab);;
            }
        }

        public void RemoveAndDestroyPrefab(GameObject unityPrefab)
        {
            List<Prefab> allPrfabs = _prefabs.GetAllEntities();
            List<Prefab> prefabs = allPrfabs.FindAll(item => item.UnityPrefab == unityPrefab);
            foreach (Prefab prefab in prefabs) RemoveAndDestroyPrefab(prefab);
        }

        public void RemoveAndDestroyAllPrefabs()
        {
            // Don't know why, but this has to be done in 2 separate passes. Otherwise, the remove operation
            // is not undone properly in some situations. So first we have to remove the prefabs from the
            // category and then we have to destroy the prefab instances in a second pass. It drives me insane,
            // but I just can't figure out why this is necessary :)
            List<Prefab> prefabs = GetAllPrefabs();
            foreach (Prefab prefab in prefabs)
            {
                _prefabs.RemoveEntity(prefab);
                PrefabWasRemovedFromCategoryMessage.SendToInterestedListeners(this, prefab);
            }

            EnsureActivePrefabPassesPrefabFilter();
            foreach (Prefab prefab in prefabs)
            {
                UndoEx.DestroyObjectImmediate(prefab);
            }
        }

        public void RemoveNullPrefabEntries()
        {
            List<Prefab> prefabsWithNullUnityPrefabs = _prefabs.GetEntitiesByPredicate(item => item != null && item.UnityPrefab == null);
   
            if(prefabsWithNullUnityPrefabs.Count != 0)
            {
                foreach (Prefab prefab in prefabsWithNullUnityPrefabs)
                {
                    _prefabs.RemoveEntity(prefab);
                    PrefabWasRemovedFromCategoryMessage.SendToInterestedListeners(this, prefab);

                    Octave3DWorldBuilder.DestroyImmediate(prefab);
                }

                EnsureActivePrefabIndexIsNotOutOfRange();
                EnsureActivePrefabPassesPrefabFilter();
            }
        }

        public int GetPrefabIndex(Prefab prefab)
        {
            return _prefabs.GetEntityIndex(prefab);
        }

        public Prefab GetPrefabByIndex(int index)
        {
            return _prefabs.GetEntityByIndex(index);
        }

        public List<Prefab> GetAllPrefabs()
        {
            return _prefabs.GetAllEntities();
        }

        public List<Prefab> GetFilteredPrefabs()
        {
            return PrefabFilter.GetFilteredPrefabs(GetAllPrefabs());
        }

        public Prefab GetPrefabByUnityPrefab(GameObject unityPrefab)
        {
            List<Prefab> prefabs = _prefabs.GetEntitiesByPredicate(item => item.UnityPrefab == unityPrefab);
            return prefabs.Count != 0 ? prefabs[0] : null;
        }

        public void SetActivePrefab(Prefab newActivePrefab)
        {
            if (newActivePrefab != null && !ContainsPrefab(newActivePrefab)) return;
            if (PrefabActivationValidationCallback != null && !PrefabActivationValidationCallback(newActivePrefab)) return;

            _prefabs.MarkEntity(newActivePrefab);
            NewPrefabWasActivatedMessage.SendToInterestedListeners(newActivePrefab);
        }

        public void RandomizeActivePrefab()
        {
            if(NumberOfPrefabs != 0)
            {
                int prefabIndex = UnityEngine.Random.Range(0, NumberOfPrefabs);
                Prefab newActivePrefab = GetPrefabByIndex(prefabIndex);

                _prefabs.MarkEntity(newActivePrefab);
                NewPrefabWasActivatedMessage.SendToInterestedListeners(newActivePrefab);
            }
        }
        #endregion

        #region Private Methods
        private void EnsureActivePrefabPassesPrefabFilter()
        {
            List<Prefab> filteredPrefabs = GetFilteredPrefabs();
            if (ActivePrefab != null && !filteredPrefabs.Contains(ActivePrefab))
            {
                PrefabCategoryActions.ActivateNextPrefabInPrefabCategory(this);
                if (ActivePrefab == null) PrefabCategoryActions.ActivatePreviousPrefabInPrefabCategory(this);
            }
        }

        private void OnDestroy()
        {
            if (_prefabFilter != null) UndoEx.DestroyObjectImmediate(_prefabFilter);
            DestroyAllPrefabs();
        }

        private void DestroyAllPrefabs()
        {
            List<Prefab> allPrefabs = GetAllPrefabs();
            foreach (Prefab prefab in allPrefabs)
            {
                if (prefab != null) UndoEx.DestroyObjectImmediate(prefab);
            }
        }

        private void LogPrefabAlreadyExistsInCategoryMessage(string categoryName)
        {
            Debug.LogWarning("A prefab can reside in only one category. The prefab already exists in the \'" + categoryName + "\' category.");
        }

        private void EnsureActivePrefabIndexIsNotOutOfRange()
        {
            if (NumberOfPrefabs == 0) SetActivePrefab(null);
            else
            {
                if (IndexOfActivePrefab < 0) SetActivePrefab(GetPrefabByIndex(0));
                else if (IndexOfActivePrefab >= NumberOfPrefabs) SetActivePrefab(GetPrefabByIndex(NumberOfPrefabs - 1));
            }
        }
        #endregion
    }
}
#endif