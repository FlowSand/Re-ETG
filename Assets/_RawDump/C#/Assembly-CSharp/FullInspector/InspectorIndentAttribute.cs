// Decompiled with JetBrains decompiler
// Type: FullInspector.InspectorIndentAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace FullInspector;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class InspectorIndentAttribute : Attribute, IInspectorAttributeOrder
{
  public double Order = 100.0;

  double IInspectorAttributeOrder.Order => this.Order;
}
