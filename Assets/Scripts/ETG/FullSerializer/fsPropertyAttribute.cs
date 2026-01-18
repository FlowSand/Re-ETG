using System;

#nullable disable
namespace FullSerializer
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
  public sealed class fsPropertyAttribute : Attribute
  {
    public string Name;
    public bool DeserializeOnly;

    public fsPropertyAttribute()
      : this(string.Empty)
    {
    }

    public fsPropertyAttribute(string name) => this.Name = name;

    public fsPropertyAttribute(bool deserializeOnly) => this.DeserializeOnly = deserializeOnly;
  }
}
