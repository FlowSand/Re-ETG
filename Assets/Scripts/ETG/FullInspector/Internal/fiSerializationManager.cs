// Decompiled with JetBrains decompiler
// Type: FullInspector.Internal.fiSerializationManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace FullInspector.Internal;

public static class fiSerializationManager
{
  [NonSerialized]
  public static bool DisableAutomaticSerialization = false;
  private static readonly List<ISerializedObject> s_pendingDeserializations = new List<ISerializedObject>();
  private static readonly List<ISerializedObject> s_pendingSerializations = new List<ISerializedObject>();
  private static readonly Dictionary<ISerializedObject, fiSerializedObjectSnapshot> s_snapshots = new Dictionary<ISerializedObject, fiSerializedObjectSnapshot>();
  public static HashSet<UnityEngine.Object> DirtyForceSerialize = new HashSet<UnityEngine.Object>();
  private static HashSet<ISerializedObject> s_seen = new HashSet<ISerializedObject>();

  static fiSerializationManager()
  {
    if (!fiUtility.IsEditor)
      return;
    // ISSUE: reference to a compiler-generated field
    if (fiSerializationManager.<>f__mg_cache0 == null)
    {
      // ISSUE: reference to a compiler-generated field
      fiSerializationManager.<>f__mg_cache0 = new System.Action(fiSerializationManager.OnEditorUpdate);
    }
    // ISSUE: reference to a compiler-generated field
    fiLateBindings.EditorApplication.AddUpdateFunc(fiSerializationManager.<>f__mg_cache0);
  }

  private static bool SupportsMultithreading<TSerializer>() where TSerializer : BaseSerializer
  {
    return !fiSettings.ForceDisableMultithreadedSerialization && !fiUtility.IsUnity4 && fiSingletons.Get<TSerializer>().SupportsMultithreading;
  }

  public static void OnUnityObjectAwake<TSerializer>(ISerializedObject obj) where TSerializer : BaseSerializer
  {
    if (obj.IsRestored)
      return;
    fiSerializationManager.DoDeserialize(obj);
  }

  public static void OnUnityObjectDeserialize<TSerializer>(ISerializedObject obj) where TSerializer : BaseSerializer
  {
    if (fiSerializationManager.SupportsMultithreading<TSerializer>())
    {
      fiSerializationManager.DoDeserialize(obj);
    }
    else
    {
      if (!fiUtility.IsEditor)
        return;
      lock ((object) fiSerializationManager.s_pendingDeserializations)
        fiSerializationManager.s_pendingDeserializations.Add(obj);
    }
  }

  public static void OnUnityObjectSerialize<TSerializer>(ISerializedObject obj) where TSerializer : BaseSerializer
  {
    if (fiSerializationManager.SupportsMultithreading<TSerializer>())
    {
      fiSerializationManager.DoSerialize(obj);
    }
    else
    {
      if (!fiUtility.IsEditor)
        return;
      lock ((object) fiSerializationManager.s_pendingSerializations)
        fiSerializationManager.s_pendingSerializations.Add(obj);
    }
  }

  private static void OnEditorUpdate()
  {
    if (Application.isPlaying)
    {
      if (fiSerializationManager.s_pendingDeserializations.Count <= 0 && fiSerializationManager.s_pendingSerializations.Count <= 0 && fiSerializationManager.s_snapshots.Count <= 0)
        return;
      fiSerializationManager.s_pendingDeserializations.Clear();
      fiSerializationManager.s_pendingSerializations.Clear();
      fiSerializationManager.s_snapshots.Clear();
    }
    else
    {
      if (fiLateBindings.EditorApplication.isPlaying && BraveUtility.isLoadingLevel)
        return;
      while (fiSerializationManager.s_pendingDeserializations.Count > 0)
      {
        ISerializedObject pendingDeserialization;
        lock ((object) fiSerializationManager.s_pendingDeserializations)
        {
          pendingDeserialization = fiSerializationManager.s_pendingDeserializations[fiSerializationManager.s_pendingDeserializations.Count - 1];
          fiSerializationManager.s_pendingDeserializations.RemoveAt(fiSerializationManager.s_pendingDeserializations.Count - 1);
        }
        if ((object) (pendingDeserialization as UnityEngine.Object) == null || !((UnityEngine.Object) pendingDeserialization == (UnityEngine.Object) null))
          fiSerializationManager.DoDeserialize(pendingDeserialization);
      }
      while (fiSerializationManager.s_pendingSerializations.Count > 0)
      {
        ISerializedObject pendingSerialization;
        lock ((object) fiSerializationManager.s_pendingSerializations)
        {
          pendingSerialization = fiSerializationManager.s_pendingSerializations[fiSerializationManager.s_pendingSerializations.Count - 1];
          fiSerializationManager.s_pendingSerializations.RemoveAt(fiSerializationManager.s_pendingSerializations.Count - 1);
        }
        if ((object) (pendingSerialization as UnityEngine.Object) == null || !((UnityEngine.Object) pendingSerialization == (UnityEngine.Object) null))
          fiSerializationManager.DoSerialize(pendingSerialization);
      }
    }
  }

  private static void DoDeserialize(ISerializedObject obj) => obj.RestoreState();

  private static void DoSerialize(ISerializedObject obj)
  {
    if (fiSerializationManager.DisableAutomaticSerialization)
      return;
    bool flag = (object) (obj as UnityEngine.Object) != null && fiSerializationManager.DirtyForceSerialize.Contains((UnityEngine.Object) obj);
    if (flag)
      fiSerializationManager.DirtyForceSerialize.Remove((UnityEngine.Object) obj);
    if (!flag && (object) (obj as UnityEngine.Object) != null && !fiLateBindings.EditorApplication.isCompilingOrChangingToPlayMode)
    {
      UnityEngine.Object objA = (UnityEngine.Object) obj;
      if (objA is Component)
        objA = (UnityEngine.Object) ((Component) objA).gameObject;
      UnityEngine.Object objB = fiLateBindings.Selection.activeObject;
      if (objB is Component)
        objB = (UnityEngine.Object) ((Component) objB).gameObject;
      if (object.ReferenceEquals((object) objA, (object) objB))
        return;
    }
    fiSerializationManager.CheckForReset(obj);
    obj.SaveState();
  }

  private static void CheckForReset(ISerializedObject obj)
  {
  }

  private static bool IsNullOrEmpty<T>(IList<T> list) => list == null || list.Count == 0;
}
