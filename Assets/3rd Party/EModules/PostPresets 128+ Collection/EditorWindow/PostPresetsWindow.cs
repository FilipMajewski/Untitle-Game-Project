#if UNITY_EDITOR
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.PostProcessing;
using UnityEngine.SceneManagement;

namespace EModules
{

  public class PostPresetsWindow : EditorWindow
  {

    public Texture2D[] t;
    static PostPresetsWindow currentWindow;

    static Texture2D[] _mGradients;
    static Texture2D[] gradients
    {
      get {
        if (string.IsNullOrEmpty( EditorResourcesPath )) return new Texture2D[0];
        if (_mGradients == null)
          _mGradients = AssetDatabase.GetAllAssetPaths()
            .Where( p => p.StartsWith( m_EditorResourcesPath + "/ColorGradients" ) )
            .Select( p => AssetDatabase.LoadAssetAtPath( p, typeof( Texture2D ) ) as Texture2D )
            .Where( t => t )
            .OrderBy( t => t.name )
            .ToArray();
        return _mGradients;
      }
    }

    static  string m_EditorResourcesPath;
    static string EditorResourcesPath
    {
      get {
        if (string.IsNullOrEmpty( m_EditorResourcesPath ))
        {
          string path;

          if (SearchForEditorResourcesPath( out path ))
            m_EditorResourcesPath = path;
          else
          {
            EditorUtility.DisplayDialog( "PostPresetsWindow Instalation Error", "Unable to locate editor resources. Make sure the PostPresetsWindow package has been installed correctly.", "Ok" );
            return null;
          }
        }
        return m_EditorResourcesPath;
      }
    }

    [MenuItem( "PostPresets 128+/Presets Manager", false, 0 )]
    public static void Init()
    {
      if (string.IsNullOrEmpty( EditorResourcesPath )) return;

      _mGradients = null;
      foreach (var window in Resources.FindObjectsOfTypeAll<PostPresetsWindow>()) window.Close();

      currentWindow = GetWindow<PostPresetsWindow>( false, "PostPresets 128+", true );
      if (!EditorPrefs.GetBool( "EModules/PostPresets/Init", false ))
      {
        EditorPrefs.GetBool( "EModules/PostPresets/Init", true );
        var p  =currentWindow.position;
        p.width = 1100;
        p.height = 650;
        p.x = Screen.currentResolution.width / 2 - p.width / 2;
        p.y = Screen.currentResolution.height / 2 - p.height / 2;
        currentWindow.position = p;
      }

      foreach (var texture2D in Resources.FindObjectsOfTypeAll<Texture2D>().Where( t => t.name == "EModules/PostPresets/ScreenShot" ))
        DestroyImmediate( texture2D );
      ResetWindow();

      Undo.undoRedoPerformed -= UndoPerform;
      Undo.undoRedoPerformed += UndoPerform;
    }

    static void ResetWindow()
    {
      mayResetScroll = true;
      currentWindow.renderedScreen = new Texture2D[0];
      currentWindow.renderedDoubleCheck = new Texture2D[0];
      EditorPrefs.SetInt( "EModules/PostPresets/Scene", SceneManager.GetActiveScene().GetHashCode() );
    }
    static Scene activeScene;
    static void UndoPerform()
    {
      if (!currentWindow) currentWindow = Resources.FindObjectsOfTypeAll<PostPresetsWindow>().FirstOrDefault();
      if (!currentWindow) return;
      currentWindow.Repaint();
    }

    private void OnDestroy()
    {
      Undo.undoRedoPerformed -= UndoPerform;
    }

    static bool SearchForEditorResourcesPath(out string path)
    {
      var allPathes = AssetDatabase.GetAllAssetPaths();
      path = allPathes.FirstOrDefault( p => p.EndsWith( "PostPresetsWindow.cs" ) );
      if (string.IsNullOrEmpty( path )) return false;
      var tempPath = path.Remove( path.LastIndexOf( '/' ) );
      var candidates = allPathes.Where( p => p.StartsWith( tempPath ) );
      path = tempPath;
      if (candidates.Any( c => c.Contains( "ColorGradients" ) )) return true;
      tempPath = path.Remove( path.LastIndexOf( '/' ) );
      candidates = allPathes.Where( p => p.StartsWith( tempPath ) );
      path = tempPath;
      if (candidates.Any( c => c.Contains( "ColorGradients" ) )) return true;
      return false;
    }

    static GUIStyle Label;
    static GUIStyle Button;

    CachedFloat AutoRefresh = new CachedFloat("AutoRefresh", 1);
    CachedFloat scrollX = new CachedFloat("ScrollX");
    CachedFloat scrollY = new CachedFloat("ScrollY");
    CachedFloat presetScrollX = new CachedFloat("presetScrollX");
    CachedFloat presetScrollY = new CachedFloat("presetScrollY");
    Vector2 scroll;

    Dictionary<PostProcessingProfile, Editor> p_to_e = new Dictionary<PostProcessingProfile, Editor>();
    PostProcessingProfile profile;

    const int LAST_COUNT =5;
    List<PostProcessingProfile> _mLastList;
    List<PostProcessingProfile> LastList
    {
      get {
        if (_mLastList == null)
        {
          _mLastList = new List<PostProcessingProfile>();
          for (int i = 0 ; i < LAST_COUNT ; i++)
          {
            var guid = EditorPrefs.GetString( "EModules/PostPresets/LastProfiles" + i,"" );
            if (string.IsNullOrEmpty( guid )) continue;
            var path = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty( path )) continue;
            var pr = AssetDatabase.LoadAssetAtPath(path, typeof(PostProcessingProfile)) as PostProcessingProfile;
            if (pr) _mLastList.Add( pr );
          }
        }
        return _mLastList;
      }
      set {
        while (value.Count > LAST_COUNT) value.RemoveAt( LAST_COUNT );
        for (int i = 0 ; i < value.Count ; i++)
        {
          if (!value[i]) continue;
          var path = AssetDatabase.GetAssetPath( value[i] );
          if (string.IsNullOrEmpty( path )) continue;
          var guid = AssetDatabase.AssetPathToGUID(path);
          if (string.IsNullOrEmpty( guid )) continue;
          EditorPrefs.SetString( "EModules/PostPresets/LastProfiles" + i, guid );
        }
        _mLastList = value;
      }
    }

    void SetLast(PostProcessingProfile newProfile)
    {
      if (!newProfile) return;
      var list = LastList;
      list.Remove( newProfile );
      if (list.Count == 0) list.Add( newProfile ); else list.Insert( 0, newProfile );
      LastList = list;
    }
    PostProcessingBehaviour p;
    GUIContent _mGUIContent = new GUIContent();
    GUIContent CONTENT(string text, string tooltip)
    {
      _mGUIContent.text = text;
      _mGUIContent.tooltip = tooltip;
      return _mGUIContent;
    }

    void OnGUI()
    {
      if (!SceneManager.GetActiveScene().IsValid()) return;

      if (!currentWindow) currentWindow = Resources.FindObjectsOfTypeAll<PostPresetsWindow>().FirstOrDefault();
      if (!currentWindow) return;

      if (SceneManager.GetActiveScene().GetHashCode() != EditorPrefs.GetInt( "EModules/PostPresets/Scene", -1 )) ResetWindow();

      if (Label == null)
      {
        Label = new GUIStyle( GUI.skin.label );
        Label.fontSize = 14;
        Label.fontStyle = FontStyle.Bold;

        Button = new GUIStyle( GUI.skin.button );
        // Button.fontSize = 14;
        var t = new Texture2D(1,1,TextureFormat.ARGB32,false,true);
        t.hideFlags = HideFlags.DontSave;
        t.SetPixel( 0, 0, new Color( 0, 0.1f, 0.4f, 0.3f ) );
        t.Apply();
        Button.normal.background = null;
        Button.hover.background = null;
        Button.focused.background = null;
        Button.active.background = t;
      }


      if (!Camera.main)
      {
        GUILayout.Label( "No Camera", Label );
        return;
      }
      p = Camera.main.GetComponent<PostProcessingBehaviour>();


      GUILayout.Space( 10 );

      GUILayout.BeginHorizontal();
      GUILayout.BeginVertical( GUILayout.Width( 350 ) );
      GUILayout.Label( "Camera: " + Camera.main.name, Label );

      if (!p)
      {
        if (GUILayout.Button( "Add PostProcessingBehaviour Script", GUILayout.Height( 200 ) ))
        {
          Undo.RecordObject( Camera.main.gameObject, "Add PostProcessingBehaviour Script" );
          Camera.main.gameObject.AddComponent<PostProcessingBehaviour>();
          EditorUtility.SetDirty( Camera.main.gameObject );
        }
        return;
      }
      profile = p.profile;


      profile = (PostProcessingProfile)EditorGUILayout.ObjectField( profile, typeof( PostProcessingProfile ), false );
      if (p.profile != profile)
      {
        Undo.RecordObject( p, "Change PostProcessing Profile" );
        p.profile = profile;
        SetLast( p.profile );
        EditorUtility.SetDirty( p );
        EditorUtility.SetDirty( Camera.main.gameObject );
      }

      GUILayout.BeginHorizontal( GUILayout.Width( 325 ) );
      for (int i = 0 ; i < LastList.Count ; i++)
      {
        if (!LastList[i]) continue;
        var al = Button.alignment;
        Button.alignment = TextAnchor.MiddleLeft;
        var result = GUILayout.Button( CONTENT(LastList[i].name, "Set " + LastList[i].name) , Button, GUILayout.Width( 325 / LastList.Count ), GUILayout.Height( 14 ) );
        Button.alignment = al;
        if (result)
        {
          Undo.RecordObject( p, "Change PostProcessing Profile" );
          p.profile = LastList[i];
          EditorUtility.SetDirty( p );
          EditorUtility.SetDirty( Camera.main.gameObject );
        }
        EditorGUIUtility.AddCursorRect( GUILayoutUtility.GetLastRect(), MouseCursor.Link );

      }
      GUILayout.EndHorizontal();

      //GUILayout.Space( 10 );
      GUILayout.BeginHorizontal( GUILayout.Width( 325 ) );
      if (GUILayout.Button( CONTENT( "Default Profile", "Set Default Profile" ) ))
      {
        var defaultProfile =  AssetDatabase.LoadAssetAtPath( EditorResourcesPath + "/CameraProfile.asset", typeof( PostProcessingProfile ) ) as PostProcessingProfile;
        if (!defaultProfile) defaultProfile = CreateProfile( EditorResourcesPath + "/CameraProfile.asset" );
        Undo.RecordObject( p, "Change PostProcessing Profile" );
        p.profile = defaultProfile;
        SetLast( p.profile );
        EditorUtility.SetDirty( p );
        EditorUtility.SetDirty( Camera.main.gameObject );
      }
      GUI.enabled = p.profile;
      if (GUILayout.Button( CONTENT( "New Copy", "Create and set a copy of current profile" ) ))
      {
        var json = EditorJsonUtility.ToJson(p.profile);
        var newProfile = CreateProfile(AssetDatabase.GenerateUniqueAssetPath( "Assets/NewCameraProfile.asset" ) );
        EditorJsonUtility.FromJsonOverwrite( json, newProfile );
        EditorUtility.SetDirty( newProfile );
        Undo.RecordObject( p, "Change PostProcessing Profile" );
        p.profile = newProfile;
        SetLast( p.profile );
        EditorUtility.SetDirty( p );
        EditorUtility.SetDirty( Camera.main.gameObject );
      }
      GUI.enabled = true;


      GUILayout.EndHorizontal();

      if (!profile)
      {
        return;
      }
      if (!p_to_e.ContainsKey( profile ))
        p_to_e.Add( profile, Editor.CreateEditor( profile ) );
      var e = p_to_e[profile];
      if (!e)
      {
        GUILayout.Label( "Internal Plugin Error", Label );
        return;
      }


      AutoRefresh.Set( EditorGUILayout.ToggleLeft( "Automatic refresh when changing", AutoRefresh == 1 ) ? 1 : 0 );


      GUILayout.Space( 10 );

      scroll.x = scrollX;
      scroll.y = scrollY;
      scroll = GUILayout.BeginScrollView( scroll, alwaysShowVertical: true, alwaysShowHorizontal: false );
      scrollX.Set( scroll.x );
      scrollY.Set( scroll.y );
      e.OnInspectorGUI();
      GUILayout.EndScrollView();
      if (GUILayout.Button( "http://emodules.me/", Button )) Application.OpenURL( "http://emodules.me/" );
      EditorGUIUtility.AddCursorRect( GUILayoutUtility.GetLastRect(), MouseCursor.Link );


      GUILayout.EndVertical();
      GUILayout.Space( 10 );

      if (profile)
      {
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        DrawPostProcessingModelButton( "antialiasing", profile.antialiasing );
        DrawPostProcessingModelButton( "ambient\nOcclusion", profile.ambientOcclusion );
        DrawPostProcessingModelButton( "bloom", profile.bloom );
        DrawPostProcessingModelButton( "screenSpace\nReflection", profile.screenSpaceReflection );
        DrawPostProcessingModelButton( "depthOfField", profile.depthOfField );
        DrawPostProcessingModelButton( "fog", profile.fog );
        DrawPostProcessingModelButton( "color\nGrading", profile.colorGrading );
        GUILayout.EndHorizontal();

        //new GUIContent("blend"),

        GUILayout.Space( 20 );

        DrawPresets();

        GUILayout.EndVertical();
      }
      GUILayout.EndHorizontal();



    }

    PostProcessingProfile CreateProfile(string path)
    {
      var profile = ScriptableObject.CreateInstance<PostProcessingProfile>();
      profile.name = Path.GetFileName( path );
      profile.fog.enabled = true;
      AssetDatabase.CreateAsset( profile, path );
      return profile;
    }

    void DrawPostProcessingModelButton(string name, PostProcessingModel model)
    {
      var rect =  EditorGUILayout.GetControlRect(GUILayout.Width(100), GUILayout.Height(40));
      if (model.enabled) EditorGUI.DrawRect( rect, new Color( 0.33f, 0.6f, 0.8f, 0.4f ) );
      EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
      if (GUI.Button( rect, CONTENT( name, "enable/disable " + name.Replace( '\n', ' ' ) ), Button ))
      {
        Undo.RecordObject( profile, "enable/disable " + name );
        model.enabled = !model.enabled;
        EditorUtility.SetDirty( profile );
        Repaint();
      }
    }
    Texture2D[] renderedDoubleCheck = new Texture2D[0];
    Texture2D[] renderedScreen = new Texture2D[0];
    static int renderedIndex;
    static bool mayResetScroll =false;
    static string oldCheck;
    static RenderTexture m_historyTexture;

    void DrawPresets()
    {
      var XXX = 3;
      int cellHeight = 125 + 16;
      var WIN_HEIGHT = position.height - 60;
      int displaingCount = Mathf.CeilToInt(WIN_HEIGHT / cellHeight) * XXX;

      var l = profile.userLut.settings.lut;
      int selectIndex = -1;
      for (int i = 0 ; i < gradients.Length ; i++)
      {
        if (l == gradients[i])
        {
          selectIndex = i;
          break;
        }
      }
      if (mayResetScroll)
      {
        mayResetScroll = false;
        if (selectIndex / XXX * cellHeight + cellHeight - WIN_HEIGHT > presetScrollY) presetScrollY.Set( selectIndex / XXX * cellHeight + cellHeight - WIN_HEIGHT );
        if (selectIndex / XXX * cellHeight < presetScrollY) presetScrollY.Set( selectIndex / XXX * cellHeight );
      }


      scroll.x = presetScrollX;
      scroll.y = presetScrollY;
      scroll = GUILayout.BeginScrollView( scroll );
      presetScrollX.Set( scroll.x );
      presetScrollY.Set( scroll.y );

      GUILayout.BeginVertical();
      var height = Mathf.Ceil( gradients.Length / (float)XXX);
      bool wasRender = false;
      int firstI = -1;
      bool needRepaint = false;

      var newCheck = EditorJsonUtility.ToJson(profile);
      if (AutoRefresh == 1 && !string.Equals( newCheck, oldCheck, StringComparison.Ordinal ))
      {
        oldCheck = newCheck;
        renderedDoubleCheck = new Texture2D[gradients.Length];
      }



      for (int y = 0 ; y < height ; y++)
      {
        var line = EditorGUILayout.GetControlRect( GUILayout.Height(cellHeight));
        var renderTexture = line.y + line.height >= scroll.y && line.y <= scroll.y + WIN_HEIGHT;
        for (int x = 0 ; x < XXX ; x++)
        {
          var i = x + y * XXX;
          if (i >= gradients.Length) break;

          var rect = line;
          rect.width = line.width / XXX;
          rect.x = rect.width * x;

          if (!renderTexture) continue;

          if (firstI == -1) firstI = i;

          if (renderedScreen.Length != gradients.Length)
          {
            System.Array.Resize( ref renderedScreen, gradients.Length );
            renderedDoubleCheck = new Texture2D[gradients.Length];
          }

          if (renderedIndex < firstI) renderedIndex = firstI;

          if (!wasRender && renderedIndex % displaingCount == (i - firstI))
          {
            renderedIndex++;
            if ((int)rect.width - 10 > 0 && !renderedDoubleCheck[i])
            {
              bool NEED_TAA = false;
              if (profile.antialiasing.enabled && profile.antialiasing.settings.method == AntialiasingModel.Method.Taa)
              {
                var m_Taa = typeof( PostProcessingBehaviour ).GetField( "m_Taa", (BindingFlags)int.MaxValue );
                var  m_ResetHistory = typeof( TaaComponent ).GetField( "m_ResetHistory", (BindingFlags)int.MaxValue );
                var  m_HistoryTexture = typeof( TaaComponent ).GetField( "m_HistoryTexture", (BindingFlags)int.MaxValue );
                if (m_Taa != null && m_ResetHistory != null && m_HistoryTexture != null)
                {
                  var taa = m_Taa.GetValue( p ) as TaaComponent;
                  if (taa != null)
                  {
                    var his = m_HistoryTexture.GetValue( taa ) as RenderTexture;
                    if (his)
                    {
                      NEED_TAA = true;
                      m_historyTexture = his;
                      m_HistoryTexture.SetValue( taa, null );
                    }
                  }
                }
              }

              var oldLutEnable = profile.userLut.enabled;
              var oldLut = profile.userLut.settings.lut;
              var oldBlend = profile.userLut.settings.contribution;
              profile.userLut.enabled = true;
              SetLut( gradients[i] );
              SetLutAmount( 1 );
              var oldAnti = profile.antialiasing.enabled;

              profile.antialiasing.enabled = false;
              renderedDoubleCheck[i] = renderedScreen[i] = TakeScreen( Camera.main, (int)rect.width - 10, 125 );
              profile.antialiasing.enabled = oldAnti;
              profile.userLut.enabled = oldLutEnable;
              SetLut( oldLut );
              SetLutAmount( oldBlend );

              if (NEED_TAA)
              {
                var m_Taa = typeof( PostProcessingBehaviour ).GetField( "m_Taa", (BindingFlags)int.MaxValue );
                var  m_ResetHistory = typeof( TaaComponent ).GetField( "m_ResetHistory", (BindingFlags)int.MaxValue );
                var  m_HistoryTexture = typeof( TaaComponent ).GetField( "m_HistoryTexture", (BindingFlags)int.MaxValue );
                var taa = m_Taa.GetValue( p );
                m_ResetHistory.SetValue( taa, false );
                m_HistoryTexture.SetValue( taa, m_historyTexture );
              }
            }
            wasRender = true;
          }

          var screen =  renderedScreen[i];

          if (!renderedDoubleCheck[i]) needRepaint = true;

          if (DrawCell( rect, screen, gradients[i].name, selectIndex == i ))
          {
            Undo.RecordObject( profile, "set " + gradients[i].name );
            SetLut( gradients[i] );
            if (!profile.userLut.enabled) SetLutAmount( 1 );
            profile.userLut.enabled = true;
            EditorUtility.SetDirty( profile );
            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
          }
        }
      }
      GUILayout.EndVertical();

     
      if (!wasRender)
      {
        // renderedIndex = firstI;
        renderedIndex++;
        //needRepaint = true;
      }
      if (needRepaint) Repaint();
      GUILayout.EndScrollView();
    }


    void SetLut(Texture2D lut)
    {
      var s=  profile.userLut.settings;
      s.lut = lut;
      profile.userLut.settings = s;
    }
    void SetLutAmount(float value)
    {
      var s=  profile.userLut.settings;
      s.contribution = value;
      profile.userLut.settings = s;
    }


    bool DrawCell(Rect rect, Texture2D tex, string name, bool selected)
    {
      rect = Shrink( rect, 2 );
      if (Event.current.type == EventType.Repaint) GUI.skin.window.Draw( rect, new GUIContent(), 0 );
      rect = Shrink( rect, 2 );

      if (selected) EditorGUI.DrawRect( rect, Color.red );

      var tr = rect;
      var lr = tr;
      tr.height -= 16;
      tr = Shrink( tr, 2 );
      if (tex) GUI.DrawTexture( tr, tex );
      else GUI.DrawTexture( tr, Texture2D.blackTexture );

      lr.y += lr.height -= 16;
      lr.height = 16;
      GUI.Label( lr, name );

      if (selected)
      {
        lr.width /= 2;
        lr.x += lr.width;
        lr.width -= 10;
        var newAmount = GUI.HorizontalSlider( lr, profile.userLut.settings.contribution,  0,1);
        if (newAmount != profile.userLut.settings.contribution)
        {
          Undo.RecordObject( profile, "set post blend" );
          SetLutAmount( newAmount );
          EditorUtility.SetDirty( profile );
          SceneView.RepaintAll();
        }
      }

      //  if (selected) GUI.DrawTexture( rect, Button.active.background );

      return GUI.Button( rect, "", Button );
    }

    Rect Shrink(Rect r, int value)
    {
      r.x += value;
      r.y += value;
      r.width -= value * 2;
      r.height -= value * 2;
      return r;
    }

    Texture2D TakeScreen(Camera camera, int resWidth, int resHeight)
    {
      var rt = new RenderTexture(resWidth, resHeight, 24);
      var oldT = camera.targetTexture;
      camera.targetTexture = rt;
      var screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false, true);
      camera.Render();
      RenderTexture.active = rt;
      screenShot.ReadPixels( new Rect( 0, 0, resWidth, resHeight ), 0, 0 );
      camera.targetTexture = oldT;
      RenderTexture.active = null;
      DestroyImmediate( rt );
      screenShot.hideFlags = HideFlags.DontSave;
      screenShot.name = "EModules/PostPresets/ScreenShot";
      screenShot.Apply();
      return screenShot;
    }

  }


  struct CachedFloat
  {
    string _key;
    float? _value;
    readonly float _defaultValue;

    public CachedFloat(string key)
    {
      this._key = "EModules/PostPresets/" + key;
      this._value = null;
      this._defaultValue = 0;
    }
    public CachedFloat(string key, float value)
    {
      this._key = "EModules/PostPresets/" + key;
      this._value = null;
      this._defaultValue = value;
    }

    public static implicit operator float(CachedFloat cf) { return cf._value ?? (cf._value = EditorPrefs.GetFloat( cf._key, cf._defaultValue )).Value; }
    public void Set(float value)
    {
      this._value = value;
      EditorPrefs.SetFloat( _key, value );
    }
  }
}

#endif