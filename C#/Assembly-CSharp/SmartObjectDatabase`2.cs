// Decompiled with JetBrains decompiler
// Type: SmartObjectDatabase`2
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

#nullable disable
[fiInspectorOnly]
public abstract class SmartObjectDatabase<T, U> : ScriptableObject
  where T : UnityEngine.Object
  where U : DatabaseEntry
{
  [InspectorCollectionRotorzFlags(DisableReordering = true, ShowIndices = true)]
  public List<T> Objects;
  [InspectorCollectionRotorzFlags(HideRemoveButtons = true)]
  [FormerlySerializedAs("GoodObjects")]
  public List<U> Entries;

  public T InternalGetByName(string name)
  {
    U u = this.Entries.Find((Predicate<U>) (obj => (object) obj != null && obj.name.Equals(name, StringComparison.OrdinalIgnoreCase)));
    return (object) u != null ? u.GetPrefab<T>() : (T) null;
  }

  public T InternalGetByGuid(string guid)
  {
    U u = this.Entries.Find((Predicate<U>) (ds => (object) ds != null && ds.myGuid == guid));
    return (object) u != null ? u.GetPrefab<T>() : (T) null;
  }

  public U InternalGetDataByGuid(string guid)
  {
    return this.Entries.Find((Predicate<U>) (ds => (object) ds != null && ds.myGuid == guid));
  }

  public void DropReferences()
  {
    for (int index = 0; index < this.Entries.Count; ++index)
      this.Entries[index].DropReference();
  }
}
