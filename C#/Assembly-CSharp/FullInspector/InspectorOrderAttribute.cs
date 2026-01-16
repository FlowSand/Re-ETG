// Decompiled with JetBrains decompiler
// Type: FullInspector.InspectorOrderAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullSerializer.Internal;
using System;
using System.Reflection;

#nullable disable
namespace FullInspector;

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
