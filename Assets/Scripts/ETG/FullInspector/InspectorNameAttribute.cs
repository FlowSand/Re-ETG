using System;

#nullable disable
namespace FullInspector
{
  [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field)]
  public sealed class InspectorNameAttribute : Attribute
  {
    public string DisplayName;

    public InspectorNameAttribute(string displayName) => this.DisplayName = displayName;
  }
}
