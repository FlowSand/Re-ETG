// Decompiled with JetBrains decompiler
// Type: FullInspector.Internal.fiFactory`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace FullInspector.Internal;

public class fiFactory<T> where T : new()
{
  private Stack<T> _reusable = new Stack<T>();
  private Action<T> _reset;

  public fiFactory(Action<T> reset) => this._reset = reset;

  public T GetInstance() => this._reusable.Count == 0 ? new T() : this._reusable.Pop();

  public void ReuseInstance(T instance)
  {
    this._reset(instance);
    this._reusable.Push(instance);
  }
}
