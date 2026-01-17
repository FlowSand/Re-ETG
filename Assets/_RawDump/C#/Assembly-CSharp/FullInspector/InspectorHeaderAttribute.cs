// Decompiled with JetBrains decompiler
// Type: FullInspector.InspectorHeaderAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace FullInspector;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class InspectorHeaderAttribute : Attribute, IInspectorAttributeOrder
{
  public double Order = 75.0;
  public string Header;

  public InspectorHeaderAttribute(string header) => this.Header = header;

  double IInspectorAttributeOrder.Order => this.Order;
}
