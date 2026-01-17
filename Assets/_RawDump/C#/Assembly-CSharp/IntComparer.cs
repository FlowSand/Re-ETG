// Decompiled with JetBrains decompiler
// Type: IntComparer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
public class IntComparer : IComparable
{
  public IntComparer(int value) => this.m_value = value;

  public int m_value { get; private set; }

  int IComparable.CompareTo(object ob) => this.m_value - ((IntComparer) ob).m_value;
}
