#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class Inspector : ScriptableObject, IMessageListener
    {
        #region Private Variables
        [SerializeField]
        private Editor _editorWindow;

        [SerializeField]
        private InspectorGUIIdentifier _activeInspectorGUIIdentifier = InspectorGUIIdentifier.ObjectPlacement;

        [SerializeField]
        private InspectorGUISelectionToolbar _inpectorGUISelectionTolbar = new InspectorGUISelectionToolbar();

        [SerializeField]
        private ObjectPlacementInspectorGUI _objectPlacementInspectorGUI = new ObjectPlacementInspectorGUI();
        [SerializeField]
        private ObjectEraseInspectorGUI _objectEraseInspectorGUI = new ObjectEraseInspectorGUI();
        [SerializeField]
        private ObjectSnappingInspectorGUI _objectSnappingInspectorGUI = new ObjectSnappingInspectorGUI();
        [SerializeField]
        private ObjectSelectionInspectorGUI _objectSelectionInspectorGUI = new ObjectSelectionInspectorGUI();
        [SerializeField]
        private ScenePreparationInspectorGUI _scenePreparationInspectorGUI = new ScenePreparationInspectorGUI();
        #endregion

        #region Public Properties
        public InspectorGUIIdentifier ActiveInspectorGUIIdentifier 
        { 
            get { return _activeInspectorGUIIdentifier; } 
            set 
            { 
                _activeInspectorGUIIdentifier = value;
                InspectorGUIWasChangedMessage.SendToInterestedListeners(_activeInspectorGUIIdentifier);

                SceneView.RepaintAll();
                if (_editorWindow != null) _editorWindow.Repaint();
            }
        }
        public InspectorGUISelectionToolbar InspectorGUISelectionToolbar { get { return _inpectorGUISelectionTolbar; } }
        public ObjectPlacementInspectorGUI ObjectPlacementInspectorGUI { get { return _objectPlacementInspectorGUI; } }
        public ObjectEraseInspectorGUI ObjectEraseInspectorGUI { get { return _objectEraseInspectorGUI; } }
        public ObjectSnappingInspectorGUI ObjectSnappingInspectorGUI { get { return _objectSnappingInspectorGUI; } }
        public ObjectSelectionInspectorGUI ObjectSelectionInspectorGUI { get { return _objectSelectionInspectorGUI; } }
        public ScenePreparationInspectorGUI ScenePreparationInspectorGUI { get { return _scenePreparationInspectorGUI; } }
        public Editor EditorWindow { get { return _editorWindow; } set { if (value != null) _editorWindow = value; } }
        #endregion

        #region Constructors
        public Inspector()
        {
            _inpectorGUISelectionTolbar.ButtonScale = 0.25f;
        }
        #endregion

        #region Public Static Functions
        public static Inspector Get()
        {
            return Octave3DWorldBuilder.Instance.Inspector;
        }
        #endregion

        #region Public Methods
        public void Repaint()
        {
            if (_editorWindow != null) _editorWindow.Repaint();
        }

        public void Render()
        {
            RenderShowGUIHintsToggle();
            Octave3DWorldBuilder.Instance.ShowGUIHint("In order to use the hotkeys, the scene view window must have focus. This means that if you click on the " +
                                                      "Inspector or an Editor Window to modify settings, you will then have to click again inside the scene view " +
                                                      "window before you can use any hotkeys. Any mouse button can be used for the click. Another way to work around this " + 
                                                      "is to perform a dummy keypress which will transfer the focus back to the scene view window.");
            Octave3DWorldBuilder.Instance.ShowGUIHint("Almost all controls have tooltips which can contain useful info. Hover the controls with the mouse cursor to allow the tooltips to appear.");

            _inpectorGUISelectionTolbar.Render();
            RenderActionControls();

            GetActiveGUI().Render();
        }

        public void Initialize()
        {
            _objectPlacementInspectorGUI.Initialize();
            _objectEraseInspectorGUI.Initialize();
            _objectSnappingInspectorGUI.Initialize();
            _objectSelectionInspectorGUI.Initialize();
            _scenePreparationInspectorGUI.Initialize();
        }
        #endregion

        #region Private Methods
        private void OnEnable()
        {
            MessageListenerRegistration.PerformRegistrationForInspector(this);
        }

        private void RenderShowGUIHintsToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForShowGUIHintsToggle(), Octave3DWorldBuilder.Instance.ShowGUIHints);
            if(newBool != Octave3DWorldBuilder.Instance.ShowGUIHints)
            {
                UndoEx.RecordForToolAction(Octave3DWorldBuilder.Instance);
                Octave3DWorldBuilder.Instance.ShowGUIHints = newBool;
            }
        }

        private GUIContent GetContentForShowGUIHintsToggle()
        {
            var content = new GUIContent();
            content.text = "Show GUI hints";
            content.tooltip = "If this is checked, the GUI will display message boxes that contain useful hints about how to use the tool.";

            return content;
        }

        private void RenderActionControls()
        {
            EditorGUILayout.BeginHorizontal();
            RenderHideWireframeButton();
            RenderRefreshGUITexturesButton();
            RenderRefreshSceneButton();
            EditorGUILayout.EndHorizontal();
        }

        private void RenderHideWireframeButton()
        {
            if (GUILayout.Button(GetContentForHideWireframeButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth * 0.6f)))
            {
                Octave3DWorldBuilder.Instance.HideWireframeForAllWorkingObjectsAndPlacementGuide();
            }
        }

        private void RenderRefreshGUITexturesButton()
        {
            if (GUILayout.Button(GetContentForRefreshGUITexturesButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth * 0.6f)))
            {
                Octave3DWorldBuilder.Instance.ToolResources.PrefabPreviewTextureCache.DisposeTextures();
                Octave3DWorldBuilder.Instance.ToolResources.TextureCache.DisposeTextures();
            }
        }

        private void RenderRefreshSceneButton()
        {
            if (GUILayout.Button(GetContentForRefreshSceneButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth * 0.6f)))
            {
                Octave3DScene.Get().Refresh(true);
            }
        }

        private GUIContent GetContentForRefreshGUITexturesButton()
        {
            var content = new GUIContent();
            content.text = "Refresh GUI textures";
            content.tooltip = "Pressing this button will refresh all GUI textures. Useful in some cases when the textures suddenly become dark. " + 
                              "This can happen when switching between play mode and the editor.";

            return content;
        }

        private GUIContent GetContentForHideWireframeButton()
        {
            var content = new GUIContent();
            content.text = "Hide wireframe";
            content.tooltip = "Hides the object wireframe which is displayed in the scene view. Useful when manually attaching child objects to the Octave3D game object. " + 
                              "The tool will automatically hide the wireframe for all objects that you place in the scene using the Octave3D interface.";

            return content;
        }

        private GUIContent GetContentForRefreshSceneButton()
        {
            var content = new GUIContent();
            content.text = "Refresh scene";
            content.tooltip = "Refreshes the internal scene data. One use case for this button is when you are working with 2D sprites and you cahnge the pivot point for one " +
                              "or more sprites. In that case the internal representation of the sprite objects needs to be rebuilt and pressing this button will do that.";

            return content;
        }

        private InspectorGUI GetActiveGUI()
        {
            switch(_activeInspectorGUIIdentifier)
            {
                case InspectorGUIIdentifier.ObjectPlacement:

                    return _objectPlacementInspectorGUI;

                case InspectorGUIIdentifier.ObjectErase:

                    return _objectEraseInspectorGUI;

                case InspectorGUIIdentifier.ObjectSelection:

                    return _objectSelectionInspectorGUI;

                case InspectorGUIIdentifier.ObjectSnapping:

                    return _objectSnappingInspectorGUI;

                case InspectorGUIIdentifier.ScenePreparation:

                    return _scenePreparationInspectorGUI;

                default:

                    return null;
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

                case MessageType.ToolWasEnabled:

                    RespondToMessage(message as ToolWasEnabledMessage);
                    break;

                case MessageType.ToolWasStarted:

                    RespondToMessage(message as ToolWasStartedMessage);
                    break;
            }
        }

        private void RespondToMessage(ToolWasResetMessage message)
        {
            Initialize();
        }

        private void RespondToMessage(ToolWasEnabledMessage message)
        {
        }

        private void RespondToMessage(ToolWasStartedMessage message)
        {
            Initialize();
        }
        #endregion
    }
}
#endif