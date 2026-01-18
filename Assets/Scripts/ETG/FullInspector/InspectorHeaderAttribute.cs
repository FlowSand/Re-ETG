using System;

#nullable disable
namespace FullInspector
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
  public class InspectorHeaderAttribute : Attribute, IInspectorAttributeOrder
  {
    public double Order = 75.0;
    public string Header;

    public InspectorHeaderAttribute(string header) => this.Header = header;

    double IInspectorAttributeOrder.Order => this.Order;
  }
}
