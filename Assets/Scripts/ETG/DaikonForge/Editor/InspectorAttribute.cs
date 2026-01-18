using System;

#nullable disable
namespace DaikonForge.Editor
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
  public class InspectorAttribute : Attribute, IComparable<InspectorAttribute>
  {
    public InspectorAttribute(string group)
    {
      this.Group = group;
      this.Order = int.MaxValue;
    }

    public InspectorAttribute(string category, int order)
    {
      this.Group = category;
      this.Order = order;
    }

    public string Group { get; set; }

    public int Order { get; set; }

    public string Label { get; set; }

    public string BackingField { get; set; }

    public string Tooltip { get; set; }

    public override string ToString()
    {
      return $"{this.Group} {this.Order} - {this.Label ?? this.BackingField ?? "(Unknown)"}";
    }

    public int CompareTo(InspectorAttribute other)
    {
      if (!string.Equals(this.Group, other.Group))
        return this.Group.CompareTo(other.Group);
      if (this.Order != other.Order)
        return this.Order.CompareTo(other.Order);
      string str = this.Label ?? this.BackingField;
      string strB = other.Label ?? other.BackingField;
      return !string.IsNullOrEmpty(str) && !string.IsNullOrEmpty(strB) ? str.CompareTo(strB) : 0;
    }
  }
}
