// Decompiled with JetBrains decompiler
// Type: FullSerializer.fsContext
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace FullSerializer
{
  public sealed class fsContext
  {
    private readonly Dictionary<Type, object> _contextObjects = new Dictionary<Type, object>();

    public void Reset() => this._contextObjects.Clear();

    public void Set<T>(T obj) => this._contextObjects[typeof (T)] = (object) obj;

    public bool Has<T>() => this._contextObjects.ContainsKey(typeof (T));

    public T Get<T>()
    {
      object obj;
      if (this._contextObjects.TryGetValue(typeof (T), out obj))
        return (T) obj;
      throw new InvalidOperationException("There is no context object of type " + (object) typeof (T));
    }
  }
}
