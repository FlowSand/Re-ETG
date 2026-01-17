// Decompiled with JetBrains decompiler
// Type: FullInspector.InspectorTooltipAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace FullInspector
{
  [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field)]
  public sealed class InspectorTooltipAttribute : Attribute
  {
    public string Tooltip;

    public InspectorTooltipAttribute(string tooltip) => this.Tooltip = tooltip;
  }
}
