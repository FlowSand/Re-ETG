// Decompiled with JetBrains decompiler
// Type: FullInspector.Internal.fiLateBindings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Reflection;
using UnityEngine;

#nullable disable
namespace FullInspector.Internal;

public static class fiLateBindings
{
  private static bool VerifyBinding(string name, object obj)
  {
    if (obj != null)
      return true;
    if (fiUtility.IsEditor)
      Debug.Log((object) $"There is no binding for {name} even though we are in an editor");
    return false;
  }

  public static class _Bindings
  {
    public static Func<string, System.Type, UnityEngine.Object> _AssetDatabase_LoadAssetAtPath;
    public static Func<bool> _EditorApplication_isPlaying;
    public static Func<bool> _EditorApplication_isCompilingOrChangingToPlayMode;
    public static Action<Action> _EditorApplication_AddUpdateAction;
    public static Action<Action> _EditorApplication_RemUpdateAction;
    public static Func<double> _EditorApplication_timeSinceStartup;
    public static Func<string, string, string> _EditorPrefs_GetString;
    public static Action<string, string> _EditorPrefs_SetString;
    public static Action<UnityEngine.Object> _EditorUtility_SetDirty;
    public static Func<int, UnityEngine.Object> _EditorUtility_InstanceIdToObject;
    public static Func<UnityEngine.Object, bool> _EditorUtility_IsPersistent;
    public static Func<string, HideFlags, GameObject> _EditorUtility_CreateGameObjectWithHideFlags;
    public static Action _EditorGUI_BeginChangeCheck;
    public static Func<bool> _EditorGUI_EndChangeCheck;
    public static Action<bool> _EditorGUI_BeginDisabledGroup;
    public static Action _EditorGUI_EndDisabledGroup;
    public static fiLateBindings._Bindings._EditorGUI_Foldout_Type _EditorGUI_Foldout;
    public static Action<Rect, string, CommentType> _EditorGUI_HelpBox;
    public static fiLateBindings._Bindings._EditorGUI_Slider_Type<int> _EditorGUI_IntSlider;
    public static fiLateBindings._Bindings._EditorGUI_PopupType _EditorGUI_Popup;
    public static fiLateBindings._Bindings._EditorGUI_Slider_Type<float> _EditorGUI_Slider;
    public static Func<GUIStyle> _EditorStyles_label;
    public static Func<GUIStyle> _EditorStyles_foldout;
    public static Action<bool> _fiEditorGUI_PushHierarchyMode;
    public static Action _fiEditorGUI_PopHierarchyMode;
    public static Func<string, GameObject, GameObject> _PrefabUtility_CreatePrefab;
    public static Func<UnityEngine.Object, bool> _PrefabUtility_IsPrefab;
    public static fiLateBindings._Bindings._PropertyEditor_Edit_Type _PropertyEditor_Edit;
    public static fiLateBindings._Bindings._PropertyEditor_GetElementHeight_Type _PropertyEditor_GetElementHeight;
    public static fiLateBindings._Bindings._PropertyEditor_EditSkipUntilNot_Type _PropertyEditor_EditSkipUntilNot;
    public static fiLateBindings._Bindings._PropertyEditor_GetElementHeightSkipUntilNot_Type _PropertyEditor_GetElementHeightSkipUntilNot;
    public static Func<UnityEngine.Object> _Selection_activeObject;

    public delegate bool _EditorGUI_Foldout_Type(
      Rect rect,
      bool status,
      GUIContent label,
      bool toggleOnLabelClick,
      GUIStyle style);

    public delegate T _EditorGUI_Slider_Type<T>(
      Rect position,
      GUIContent label,
      T value,
      T leftValue,
      T rightValue);

    public delegate int _EditorGUI_PopupType(
      Rect position,
      GUIContent label,
      int selectedIndex,
      GUIContent[] displayedOptions);

    public delegate object _PropertyEditor_Edit_Type(
      System.Type objType,
      MemberInfo attrs,
      Rect rect,
      GUIContent label,
      object obj,
      fiGraphMetadataChild metadata,
      System.Type[] skippedEditors);

    public delegate float _PropertyEditor_GetElementHeight_Type(
      System.Type objType,
      MemberInfo attrs,
      GUIContent label,
      object obj,
      fiGraphMetadataChild metadata,
      System.Type[] skippedEditors);

    public delegate object _PropertyEditor_EditSkipUntilNot_Type(
      System.Type[] skipUntilNot,
      System.Type objType,
      MemberInfo attrs,
      Rect rect,
      GUIContent label,
      object obj,
      fiGraphMetadataChild metadata);

    public delegate float _PropertyEditor_GetElementHeightSkipUntilNot_Type(
      System.Type[] skipUntilNot,
      System.Type objType,
      MemberInfo attrs,
      GUIContent label,
      object obj,
      fiGraphMetadataChild metadata);
  }

  public static class AssetDatabase
  {
    public static UnityEngine.Object LoadAssetAtPath(string path, System.Type type)
    {
      return fiLateBindings.VerifyBinding("AssetDatabase.LoadAssetAtPath", (object) fiLateBindings._Bindings._AssetDatabase_LoadAssetAtPath) ? fiLateBindings._Bindings._AssetDatabase_LoadAssetAtPath(path, type) : (UnityEngine.Object) null;
    }
  }

  public static class EditorApplication
  {
    public static bool isPlaying
    {
      get
      {
        return !fiLateBindings.VerifyBinding("EditorApplication.isPlaying", (object) fiLateBindings._Bindings._EditorApplication_isPlaying) || fiLateBindings._Bindings._EditorApplication_isPlaying();
      }
    }

    public static bool isCompilingOrChangingToPlayMode
    {
      get
      {
        return !fiLateBindings.VerifyBinding("EditorApplication.isCompilingOrChangingToPlayMode", (object) fiLateBindings._Bindings._EditorApplication_isCompilingOrChangingToPlayMode) || fiLateBindings._Bindings._EditorApplication_isCompilingOrChangingToPlayMode();
      }
    }

    public static double timeSinceStartup
    {
      get
      {
        return fiLateBindings.VerifyBinding("EditorApplication.timeSinceStartup", (object) fiLateBindings._Bindings._EditorApplication_timeSinceStartup) ? fiLateBindings._Bindings._EditorApplication_timeSinceStartup() : 0.0;
      }
    }

    public static void AddUpdateFunc(Action func)
    {
      if (!fiLateBindings.VerifyBinding("EditorApplication.AddUpdateFunc", (object) fiLateBindings._Bindings._EditorApplication_AddUpdateAction))
        return;
      fiLateBindings._Bindings._EditorApplication_AddUpdateAction(func);
    }

    public static void RemUpdateFunc(Action func)
    {
      if (!fiLateBindings.VerifyBinding("EditorApplication.RemUpdateFunc", (object) fiLateBindings._Bindings._EditorApplication_RemUpdateAction))
        return;
      fiLateBindings._Bindings._EditorApplication_RemUpdateAction(func);
    }
  }

  public static class EditorPrefs
  {
    public static string GetString(string key, string defaultValue)
    {
      return fiLateBindings.VerifyBinding("EditorPrefs.GetString", (object) fiLateBindings._Bindings._EditorPrefs_GetString) ? fiLateBindings._Bindings._EditorPrefs_GetString(key, defaultValue) : defaultValue;
    }

    public static void SetString(string key, string value)
    {
      if (!fiLateBindings.VerifyBinding("EditorPrefs.SetString", (object) fiLateBindings._Bindings._EditorPrefs_SetString))
        return;
      fiLateBindings._Bindings._EditorPrefs_SetString(key, value);
    }
  }

  public static class EditorUtility
  {
    public static void SetDirty(UnityEngine.Object unityObject)
    {
      if (!fiLateBindings.VerifyBinding("EditorUtility.SetDirty", (object) fiLateBindings._Bindings._EditorUtility_SetDirty))
        return;
      fiLateBindings._Bindings._EditorUtility_SetDirty(unityObject);
    }

    public static UnityEngine.Object InstanceIDToObject(int instanceId)
    {
      return fiLateBindings.VerifyBinding("EditorUtility.InstanceIdToObject", (object) fiLateBindings._Bindings._EditorUtility_InstanceIdToObject) ? fiLateBindings._Bindings._EditorUtility_InstanceIdToObject(instanceId) : (UnityEngine.Object) null;
    }

    public static bool IsPersistent(UnityEngine.Object unityObject)
    {
      return fiLateBindings.VerifyBinding("EditorUtility.IsPersistent", (object) fiLateBindings._Bindings._EditorUtility_IsPersistent) && fiLateBindings._Bindings._EditorUtility_IsPersistent(unityObject);
    }

    public static GameObject CreateGameObjectWithHideFlags(string name, HideFlags hideFlags)
    {
      if (fiLateBindings.VerifyBinding("EditorUtility.CreateGameObjectWithHideFlags", (object) fiLateBindings._Bindings._EditorUtility_CreateGameObjectWithHideFlags))
        return fiLateBindings._Bindings._EditorUtility_CreateGameObjectWithHideFlags(name, hideFlags);
      GameObject objectWithHideFlags = new GameObject(name);
      objectWithHideFlags.hideFlags = hideFlags;
      return objectWithHideFlags;
    }
  }

  public static class EditorGUI
  {
    public static void BeginChangeCheck()
    {
      if (!fiLateBindings.VerifyBinding("EditorGUI.BeginChangeCheck", (object) fiLateBindings._Bindings._EditorGUI_BeginDisabledGroup))
        return;
      fiLateBindings._Bindings._EditorGUI_BeginChangeCheck();
    }

    public static bool EndChangeCheck()
    {
      return fiLateBindings.VerifyBinding("EditorGUI.EndChangeCheck", (object) fiLateBindings._Bindings._EditorGUI_EndDisabledGroup) && fiLateBindings._Bindings._EditorGUI_EndChangeCheck();
    }

    public static void BeginDisabledGroup(bool disabled)
    {
      if (!fiLateBindings.VerifyBinding("EditorGUI.BeginDisabledGroup", (object) fiLateBindings._Bindings._EditorGUI_BeginDisabledGroup))
        return;
      fiLateBindings._Bindings._EditorGUI_BeginDisabledGroup(disabled);
    }

    public static void EndDisabledGroup()
    {
      if (!fiLateBindings.VerifyBinding("EditorGUI.EndDisabledGroup", (object) fiLateBindings._Bindings._EditorGUI_EndDisabledGroup))
        return;
      fiLateBindings._Bindings._EditorGUI_EndDisabledGroup();
    }

    public static bool Foldout(
      Rect rect,
      bool state,
      GUIContent label,
      bool toggleOnLabelClick,
      GUIStyle style)
    {
      return !fiLateBindings.VerifyBinding("EditorGUI.Foldout", (object) fiLateBindings._Bindings._EditorGUI_Foldout) || fiLateBindings._Bindings._EditorGUI_Foldout(rect, state, label, toggleOnLabelClick, style);
    }

    public static void HelpBox(Rect rect, string message, CommentType commentType)
    {
      if (!fiLateBindings.VerifyBinding("EditorGUI.HelpBox", (object) fiLateBindings._Bindings._EditorGUI_HelpBox))
        return;
      fiLateBindings._Bindings._EditorGUI_HelpBox(rect, message, commentType);
    }

    public static int IntSlider(
      Rect position,
      GUIContent label,
      int value,
      int leftValue,
      int rightValue)
    {
      return fiLateBindings.VerifyBinding("EditorGUI.IntSlider", (object) fiLateBindings._Bindings._EditorGUI_IntSlider) ? fiLateBindings._Bindings._EditorGUI_IntSlider(position, label, value, leftValue, rightValue) : value;
    }

    public static int Popup(
      Rect position,
      GUIContent label,
      int selectedIndex,
      GUIContent[] displayedOptions)
    {
      return fiLateBindings.VerifyBinding("EditorGUI.Popup", (object) fiLateBindings._Bindings._EditorGUI_Popup) ? fiLateBindings._Bindings._EditorGUI_Popup(position, label, selectedIndex, displayedOptions) : selectedIndex;
    }

    public static float Slider(
      Rect position,
      GUIContent label,
      float value,
      float leftValue,
      float rightValue)
    {
      return fiLateBindings.VerifyBinding("EditorGUI.Slider", (object) fiLateBindings._Bindings._EditorGUI_Slider) ? fiLateBindings._Bindings._EditorGUI_Slider(position, label, value, leftValue, rightValue) : value;
    }
  }

  public static class EditorGUIUtility
  {
    public static float standardVerticalSpacing = 2f;
    public static float singleLineHeight = 16f;
  }

  public static class EditorStyles
  {
    public static GUIStyle label
    {
      get
      {
        return fiLateBindings.VerifyBinding("EditorStyles.label", (object) fiLateBindings._Bindings._EditorStyles_label) ? fiLateBindings._Bindings._EditorStyles_label() : new GUIStyle();
      }
    }

    public static GUIStyle foldout
    {
      get
      {
        return fiLateBindings.VerifyBinding("EditorStyles.foldout", (object) fiLateBindings._Bindings._EditorStyles_foldout) ? fiLateBindings._Bindings._EditorStyles_foldout() : new GUIStyle();
      }
    }
  }

  public static class fiEditorGUI
  {
    public static void PushHierarchyMode(bool state)
    {
      if (!fiLateBindings.VerifyBinding("fiEditorGUI.PushHierarchyMode", (object) fiLateBindings._Bindings._fiEditorGUI_PushHierarchyMode))
        return;
      fiLateBindings._Bindings._fiEditorGUI_PushHierarchyMode(state);
    }

    public static void PopHierarchyMode()
    {
      if (!fiLateBindings.VerifyBinding("fiEditorGUI.PopHierarchyMode", (object) fiLateBindings._Bindings._fiEditorGUI_PopHierarchyMode))
        return;
      fiLateBindings._Bindings._fiEditorGUI_PopHierarchyMode();
    }
  }

  public static class PrefabUtility
  {
    public static GameObject CreatePrefab(string path, GameObject template)
    {
      return fiLateBindings.VerifyBinding("PrefabUtility.CreatePrefab", (object) fiLateBindings._Bindings._PrefabUtility_CreatePrefab) ? fiLateBindings._Bindings._PrefabUtility_CreatePrefab(path, template) : (GameObject) null;
    }

    public static bool IsPrefab(UnityEngine.Object unityObject)
    {
      return fiLateBindings.VerifyBinding("PrefabUtility.IsPrefab", (object) fiLateBindings._Bindings._PrefabUtility_IsPrefab) && fiLateBindings._Bindings._PrefabUtility_IsPrefab(unityObject);
    }
  }

  public static class PropertyEditor
  {
    public static object Edit(
      System.Type objType,
      MemberInfo attrs,
      Rect rect,
      GUIContent label,
      object obj,
      fiGraphMetadataChild metadata,
      params System.Type[] skippedEditors)
    {
      return fiLateBindings.VerifyBinding("PropertyEditor.Edit", (object) fiLateBindings._Bindings._PropertyEditor_Edit) ? fiLateBindings._Bindings._PropertyEditor_Edit(objType, attrs, rect, label, obj, metadata, skippedEditors) : obj;
    }

    public static float GetElementHeight(
      System.Type objType,
      MemberInfo attrs,
      GUIContent label,
      object obj,
      fiGraphMetadataChild metadata,
      params System.Type[] skippedEditors)
    {
      return fiLateBindings.VerifyBinding("PropertyEditor.GetElementHeight", (object) fiLateBindings._Bindings._PropertyEditor_GetElementHeight) ? fiLateBindings._Bindings._PropertyEditor_GetElementHeight(objType, attrs, label, obj, metadata, skippedEditors) : 0.0f;
    }

    public static object EditSkipUntilNot(
      System.Type[] skipUntilNot,
      System.Type objType,
      MemberInfo attrs,
      Rect rect,
      GUIContent label,
      object obj,
      fiGraphMetadataChild metadata)
    {
      return fiLateBindings.VerifyBinding("PropertyEditor.EditSkipUntilNot", (object) fiLateBindings._Bindings._PropertyEditor_EditSkipUntilNot) ? fiLateBindings._Bindings._PropertyEditor_EditSkipUntilNot(skipUntilNot, objType, attrs, rect, label, obj, metadata) : obj;
    }

    public static float GetElementHeightSkipUntilNot(
      System.Type[] skipUntilNot,
      System.Type objType,
      MemberInfo attrs,
      GUIContent label,
      object obj,
      fiGraphMetadataChild metadata)
    {
      return fiLateBindings.VerifyBinding("PropertyEditor.GetElementHeightSkipUntilNot", (object) fiLateBindings._Bindings._PropertyEditor_GetElementHeightSkipUntilNot) ? fiLateBindings._Bindings._PropertyEditor_GetElementHeightSkipUntilNot(skipUntilNot, objType, attrs, label, obj, metadata) : 0.0f;
    }
  }

  public static class Selection
  {
    public static UnityEngine.Object activeObject
    {
      get
      {
        return fiLateBindings.VerifyBinding("Selection.activeObject", (object) fiLateBindings._Bindings._Selection_activeObject) ? fiLateBindings._Bindings._Selection_activeObject() : (UnityEngine.Object) null;
      }
    }
  }
}
