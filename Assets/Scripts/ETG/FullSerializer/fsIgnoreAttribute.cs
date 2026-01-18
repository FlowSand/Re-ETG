using System;

#nullable disable
namespace FullSerializer
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class fsIgnoreAttribute : Attribute
    {
    }
}
