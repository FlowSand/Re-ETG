using System;

#nullable disable
namespace FullInspector
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class InspectorIndentAttribute : Attribute, IInspectorAttributeOrder
    {
        public double Order = 100.0;

        double IInspectorAttributeOrder.Order => this.Order;
    }
}
