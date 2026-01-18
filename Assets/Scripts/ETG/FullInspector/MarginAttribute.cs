using System;

#nullable disable
namespace FullInspector
{
    [Obsolete("Please use [InspectorMargin] instead of [Margin]")]
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class MarginAttribute : Attribute, IInspectorAttributeOrder
    {
        public int Margin;
        public double Order;

        public MarginAttribute(int margin) => this.Margin = margin;

        double IInspectorAttributeOrder.Order => this.Order;
    }
}
