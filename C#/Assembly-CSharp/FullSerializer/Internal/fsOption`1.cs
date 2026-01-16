// Decompiled with JetBrains decompiler
// Type: FullSerializer.Internal.fsOption`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace FullSerializer.Internal;

public struct fsOption<T>(T value)
{
  private bool _hasValue = true;
  private T _value = value;
  public static fsOption<T> Empty;

  public bool HasValue => this._hasValue;

  public bool IsEmpty => !this._hasValue;

  public T Value
  {
    get
    {
      if (this.IsEmpty)
        throw new InvalidOperationException("fsOption is empty");
      return this._value;
    }
  }
}
