#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class Octave3DScene : IMessageListener
    {
        #region Private Variables
        [SerializeField]
        private GameObjectSphereTree _gameObjectSphereTree = new GameObjectSphereTree(2);
        
        [SerializeField]
        private Octave3DMeshDatabase _octave3DMeshDatabase = new Octave3DMeshDatabase();
        #endregion

        #region Public Static Properties
        public static Vector3 VolumeSizeForNonMeshObjects { get { return Vector3.one; } }
        #endregion

        #region Public Properties
        public Octave3DMeshDatabase Octave3DMeshDatabase { get { return _octave3DMeshDatabase; } }
        #endregion

        #region Constructors
        public Octave3DScene()
        {
            MessageListenerRegistration.PerformRegistrationForOctave3DScene(this);
        }
        #endregion

        #region Public Static Functions
        public static Octave3DScene Get()
        {
            return Octave3DWorldBuilder.Instance.Octave3DScene;
        }
        #endregion

        #region Public Methods
        public void RenderGizmosDebug()
        {
            _gameObjectSphereTree.RenderGizmosDebug();
        }

        public void Refresh(bool showProgress)
        {
            _gameObjectSphereTree.Rebuild(showProgress);
        }

        public void Update()
        {
            _gameObjectSphereTree.Update();
        }

        public void OnSceneGUI()
        {
            _gameObjectSphereTree.OnSceneGUI();
        }

        public List<GameObjectRayHit> RaycastAllBox(Ray ray)
        {
            return _gameObjectSphereTree.RaycastAllBox(ray);
        }

        public List<GameObjectRayHit> RaycastAllSprite(Ray ray)
        {
            return _gameObjectSphereTree.RaycastAllSprite(ray);
        }

        public List<GameObjectRayHit> RaycastAllMesh(Ray ray)
        {
            return _gameObjectSphereTree.RaycastAllMesh(ray);
        }

        public List<GameObject> OverlapSphere(Sphere sphere, ObjectOverlapPrecision overlapPrecision = ObjectOverlapPrecision.ObjectBox)
        {
            return _gameObjectSphereTree.OverlapSphere(sphere, overlapPrecision);
        }

        public List<GameObject> OverlapBox(OrientedBox box, ObjectOverlapPrecision overlapPrecision = ObjectOverlapPrecision.ObjectBox)
        {
            return _gameObjectSphereTree.OverlapBox(box, overlapPrecision);
        }

        public List<GameObject> OverlapBox(Box box, ObjectOverlapPrecision overlapPrecision = ObjectOverlapPrecision.ObjectBox)
        {
            return _gameObjectSphereTree.OverlapBox(box, overlapPrecision);
        }

        public List<GameObject> InstantiateObjectHirarchiesFromPlacementDataCollection(List<ObjectPlacementData> objectPlacementDataCollection)
        {
            if (objectPlacementDataCollection.Count == 0) return new List<GameObject>();

            var instantiatedHierarchyRoots = ObjectInstantiation.InstantiateObjectHirarchiesFromPlacementDataCollection(objectPlacementDataCollection);
            _gameObjectSphereTree.RegisterGameObjectHierarchies(instantiatedHierarchyRoots);
            return instantiatedHierarchyRoots;
        }

        public GameObject InstantiateObjectHierarchyFromPlacementData(ObjectPlacementData objectPlacementData)
        {
            GameObject instantiatedObject = ObjectInstantiation.InstantiateObjectHierarchyFromPlacementData(objectPlacementData);
            _gameObjectSphereTree.RegisterGameObjectHierarchy(instantiatedObject);
            return instantiatedObject;
        }

        public GameObject InstantiateObjectHierarchyFromPrefab(Prefab prefab, Transform transformData)
        {
            GameObject instantiatedObject = ObjectInstantiation.InstantiateObjectHierarchyFromPrefab(prefab, transformData.position, transformData.rotation, transformData.lossyScale);
            _gameObjectSphereTree.RegisterGameObjectHierarchy(instantiatedObject);
            return instantiatedObject;
        }

        public GameObject InstantiateObjectHierarchyPrefab(Prefab prefab, Vector3 worldPosition, Quaternion worldRotation, Vector3 worldScale)
        {
             GameObject instantiatedObject = ObjectInstantiation.InstantiateObjectHierarchyFromPrefab(prefab, worldPosition, worldRotation, worldScale);
             _gameObjectSphereTree.RegisterGameObjectHierarchy(instantiatedObject);
             return instantiatedObject;
        }

        public void RegisterObjectHierarchy(GameObject hierarchyRoot)
        {
            _gameObjectSphereTree.RegisterGameObjectHierarchy(hierarchyRoot);
        }
       
        public void RegisterObjectHierarchies(List<GameObject> roots)
        {
            foreach(var root in roots)
            {
                RegisterObjectHierarchy(root);
            }
        }
        #endregion

        #region Message Handlers
        public void RespondToMessage(Message message)
        {
            switch(message.Type)
            {
                case MessageType.ToolWasReset:

                    RespondToMessage(message as ToolWasResetMessage);
                    break;

                case MessageType.ToolWasSelected:

                    RespondToMessage(message as ToolWasSelectedMessage);
                    break;
            }
        }

        private void RespondToMessage(ToolWasResetMessage message)
        {
            _gameObjectSphereTree.RegisterGameObjectHierarchies(Octave3DWorldBuilder.Instance.GetImmediateWorkingObjects());
        }

        private void RespondToMessage(ToolWasSelectedMessage message)
        {
            // Note: It is possible that the user may have changed the transform of the
            //       objects while the tool was deselected, so we need to ensure that the
            //       tree is up to date.
            _gameObjectSphereTree.HandleTransformChangesForAllRegisteredObjects();
      
            // It may be possible that the user has added objects to the scene while the tool object
            // was deselected, so we will instruct the camera to update its visibility.
            SceneViewCamera.Instance.SetObjectVisibilityDirty();
        }
        #endregion
    }
}
#endif