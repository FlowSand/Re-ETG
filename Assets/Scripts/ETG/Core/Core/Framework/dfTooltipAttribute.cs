using System;

#nullable disable

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field)]
public class dfTooltipAttribute : Attribute
  {
    public dfTooltipAttribute(string tooltip) => this.Tooltip = tooltip;

    public string Tooltip { get; private set; }
  }

