using System;

#nullable disable
namespace FullInspector
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class InspectorMarginAttribute : Attribute, IInspectorAttributeOrder
    {
        public int Margin;
        public double Order;

        public InspectorMarginAttribute(int margin) => this.Margin = margin;

        double IInspectorAttributeOrder.Order => this.Order;
    }
}
