using System;
using System.Linq;
using System.Reflection;

using FullInspector.Internal;
using FullSerializer.Internal;

#nullable disable
namespace FullInspector
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class InspectorCollectionAddItemAttributesAttribute : Attribute
    {
        public MemberInfo AttributeProvider;

        public InspectorCollectionAddItemAttributesAttribute(Type attributes)
        {
            this.AttributeProvider = typeof (fiICollectionAttributeProvider).Resolve().IsAssignableFrom(attributes.Resolve()) ? fiAttributeProvider.Create(((fiICollectionAttributeProvider) Activator.CreateInstance(attributes)).GetAttributes().ToArray<object>()) : throw new ArgumentException("Must be an instance of FullInspector.fiICollectionAttributeProvider", nameof (attributes));
        }
    }
}
