using System;

#nullable disable
namespace FullInspector
{
  [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
  public sealed class InspectorCategoryAttribute : Attribute
  {
    public string Category;

    public InspectorCategoryAttribute(string category) => this.Category = category;
  }
}
