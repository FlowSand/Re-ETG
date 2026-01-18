using FullSerializer.Internal;
using System;
using System.Reflection;

#nullable disable
namespace FullInspector
{
  [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field)]
  public sealed class InspectorOrderAttribute : Attribute
  {
    public double Order;

    public InspectorOrderAttribute(double order) => this.Order = order;

    public static double GetInspectorOrder(MemberInfo memberInfo)
    {
      InspectorOrderAttribute attribute = fsPortableReflection.GetAttribute<InspectorOrderAttribute>(memberInfo);
      return attribute != null ? attribute.Order : double.MaxValue;
    }
  }
}
