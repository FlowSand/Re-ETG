// Decompiled with JetBrains decompiler
// Type: FullInspector.MarginAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace FullInspector;

[Obsolete("Please use [InspectorMargin] instead of [Margin]")]
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class MarginAttribute : Attribute, IInspectorAttributeOrder
{
  public int Margin;
  public double Order;

  public MarginAttribute(int margin) => this.Margin = margin;

  double IInspectorAttributeOrder.Order => this.Order;
}
