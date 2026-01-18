using System;

#nullable disable
namespace FullInspector
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class InspectorCollapsedFoldoutAttribute : Attribute
    {
    }
}
