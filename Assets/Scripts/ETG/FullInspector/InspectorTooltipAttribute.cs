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
