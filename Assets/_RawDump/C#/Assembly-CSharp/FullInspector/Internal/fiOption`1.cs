// Decompiled with JetBrains decompiler
// Type: FullInspector.Internal.fiOption`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace FullInspector.Internal;

public struct fiOption<T>(T value)
{
  private bool _hasValue = true;
  private T _value = value;
  public static fiOption<T> Empty = new fiOption<T>()
  {
    _hasValue = false,
    _value = default (T)
  };

  public bool HasValue => this._hasValue;

  public bool IsEmpty => !this._hasValue;

  public T Value
  {
    get
    {
      if (!this.HasValue)
        throw new InvalidOperationException("There is no value inside the option");
      return this._value;
    }
  }
}
