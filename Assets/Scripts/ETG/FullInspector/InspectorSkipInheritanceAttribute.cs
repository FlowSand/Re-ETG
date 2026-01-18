using System;

#nullable disable
namespace FullInspector
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class InspectorSkipInheritanceAttribute : Attribute, IInspectorAttributeOrder
    {
        double IInspectorAttributeOrder.Order => double.MinValue;
    }
}
