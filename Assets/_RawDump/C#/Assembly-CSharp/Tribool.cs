// Decompiled with JetBrains decompiler
// Type: Tribool
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
public struct Tribool(int v)
{
  public static Tribool Complete = new Tribool(2);
  public static Tribool Ready = new Tribool(1);
  public static Tribool Unready = new Tribool(0);
  public int value = v;

  public override string ToString() => string.Format("[Tribool] " + this.value.ToString());

  public static bool operator true(Tribool a) => a.value == 1;

  public static bool operator false(Tribool a) => a.value == 0;

  public static bool operator !(Tribool a) => a.value == 0;

  public static bool operator ==(Tribool a, Tribool b) => a.value == b.value;

  public static bool operator !=(Tribool a, Tribool b) => a.value != b.value;

  public static Tribool operator ++(Tribool a) => new Tribool(Math.Min(2, a.value + 1));

  public override bool Equals(object obj)
  {
    return obj is Tribool tribool ? this == tribool : base.Equals(obj);
  }

  public override int GetHashCode() => this.value;
}
