// Decompiled with JetBrains decompiler
// Type: FullInspector.Internal.fiEither`2
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace FullInspector.Internal
{
  public struct fiEither<TA, TB>
  {
    private TA _valueA;
    private TB _valueB;
    private bool _hasA;

    public fiEither(TA valueA)
    {
      this._hasA = true;
      this._valueA = valueA;
      this._valueB = default (TB);
    }

    public fiEither(TB valueB)
    {
      this._hasA = false;
      this._valueA = default (TA);
      this._valueB = valueB;
    }

    public TA ValueA
    {
      get
      {
        if (!this.IsA)
          throw new InvalidOperationException("Either does not contain value A");
        return this._valueA;
      }
    }

    public TB ValueB
    {
      get
      {
        if (!this.IsB)
          throw new InvalidOperationException("Either does not contain value B");
        return this._valueB;
      }
    }

    public bool IsA => this._hasA;

    public bool IsB => !this._hasA;
  }
}
