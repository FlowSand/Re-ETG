// Decompiled with JetBrains decompiler
// Type: ObjectDatabase`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

  public abstract class ObjectDatabase<T> : ScriptableObject where T : UnityEngine.Object
  {
    public List<T> Objects;

    public int InternalGetId(T obj) => this.Objects.IndexOf(obj);

    public T InternalGetById(int id)
    {
      return id < 0 || id >= this.Objects.Count ? (T) null : this.Objects[id];
    }

    public T InternalGetByName(string name)
    {
      return this.Objects.Find((Predicate<T>) (obj => (UnityEngine.Object) obj != (UnityEngine.Object) null && obj.name.Equals(name, StringComparison.OrdinalIgnoreCase)));
    }
  }

