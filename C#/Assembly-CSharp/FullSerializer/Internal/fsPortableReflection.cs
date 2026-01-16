// Decompiled with JetBrains decompiler
// Type: FullSerializer.Internal.fsPortableReflection
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

#nullable disable
namespace FullSerializer.Internal;

public static class fsPortableReflection
{
  public static Type[] EmptyTypes = new Type[0];
  private static IDictionary<fsPortableReflection.AttributeQuery, Attribute> _cachedAttributeQueries = (IDictionary<fsPortableReflection.AttributeQuery, Attribute>) new Dictionary<fsPortableReflection.AttributeQuery, Attribute>((IEqualityComparer<fsPortableReflection.AttributeQuery>) new fsPortableReflection.AttributeQueryComparator());
  private static BindingFlags DeclaredFlags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

  public static bool HasAttribute(MemberInfo element, Type attributeType)
  {
    return fsPortableReflection.GetAttribute(element, attributeType) != null;
  }

  public static bool HasAttribute<TAttribute>(MemberInfo element)
  {
    return fsPortableReflection.HasAttribute(element, typeof (TAttribute));
  }

  public static Attribute GetAttribute(MemberInfo element, Type attributeType)
  {
    fsPortableReflection.AttributeQuery key = new fsPortableReflection.AttributeQuery()
    {
      MemberInfo = element,
      AttributeType = attributeType
    };
    Attribute attribute;
    if (!fsPortableReflection._cachedAttributeQueries.TryGetValue(key, out attribute))
    {
      attribute = (Attribute) ((IEnumerable<object>) element.GetCustomAttributes(attributeType, true)).FirstOrDefault<object>();
      fsPortableReflection._cachedAttributeQueries[key] = attribute;
    }
    return attribute;
  }

  public static TAttribute GetAttribute<TAttribute>(MemberInfo element) where TAttribute : Attribute
  {
    return (TAttribute) fsPortableReflection.GetAttribute(element, typeof (TAttribute));
  }

  public static PropertyInfo GetDeclaredProperty(this Type type, string propertyName)
  {
    PropertyInfo[] declaredProperties = type.GetDeclaredProperties();
    for (int index = 0; index < declaredProperties.Length; ++index)
    {
      if (declaredProperties[index].Name == propertyName)
        return declaredProperties[index];
    }
    return (PropertyInfo) null;
  }

  public static MethodInfo GetDeclaredMethod(this Type type, string methodName)
  {
    MethodInfo[] declaredMethods = type.GetDeclaredMethods();
    for (int index = 0; index < declaredMethods.Length; ++index)
    {
      if (declaredMethods[index].Name == methodName)
        return declaredMethods[index];
    }
    return (MethodInfo) null;
  }

  public static ConstructorInfo GetDeclaredConstructor(this Type type, Type[] parameters)
  {
    foreach (ConstructorInfo declaredConstructor in type.GetDeclaredConstructors())
    {
      ParameterInfo[] parameters1 = declaredConstructor.GetParameters();
      if (parameters.Length == parameters1.Length)
      {
        for (int index = 0; index < parameters1.Length; ++index)
        {
          if (parameters1[index].ParameterType == parameters[index])
            ;
        }
        return declaredConstructor;
      }
    }
    return (ConstructorInfo) null;
  }

  public static ConstructorInfo[] GetDeclaredConstructors(this Type type)
  {
    return type.GetConstructors(fsPortableReflection.DeclaredFlags);
  }

  public static MemberInfo[] GetFlattenedMember(this Type type, string memberName)
  {
    List<MemberInfo> memberInfoList = new List<MemberInfo>();
    for (; type != null; type = type.Resolve().BaseType)
    {
      MemberInfo[] declaredMembers = type.GetDeclaredMembers();
      for (int index = 0; index < declaredMembers.Length; ++index)
      {
        if (declaredMembers[index].Name == memberName)
          memberInfoList.Add(declaredMembers[index]);
      }
    }
    return memberInfoList.ToArray();
  }

  public static MethodInfo GetFlattenedMethod(this Type type, string methodName)
  {
    for (; type != null; type = type.Resolve().BaseType)
    {
      MethodInfo[] declaredMethods = type.GetDeclaredMethods();
      for (int index = 0; index < declaredMethods.Length; ++index)
      {
        if (declaredMethods[index].Name == methodName)
          return declaredMethods[index];
      }
    }
    return (MethodInfo) null;
  }

  [DebuggerHidden]
  public static IEnumerable<MethodInfo> GetFlattenedMethods(this Type type, string methodName)
  {
    // ISSUE: object of a compiler-generated type is created
    // ISSUE: variable of a compiler-generated type
    fsPortableReflection.\u003CGetFlattenedMethods\u003Ec__Iterator0 flattenedMethods = new fsPortableReflection.\u003CGetFlattenedMethods\u003Ec__Iterator0()
    {
      type = type,
      methodName = methodName,
      \u003C\u0024\u003Etype = type
    };
    // ISSUE: reference to a compiler-generated field
    flattenedMethods.\u0024PC = -2;
    return (IEnumerable<MethodInfo>) flattenedMethods;
  }

  public static PropertyInfo GetFlattenedProperty(this Type type, string propertyName)
  {
    for (; type != null; type = type.Resolve().BaseType)
    {
      PropertyInfo[] declaredProperties = type.GetDeclaredProperties();
      for (int index = 0; index < declaredProperties.Length; ++index)
      {
        if (declaredProperties[index].Name == propertyName)
          return declaredProperties[index];
      }
    }
    return (PropertyInfo) null;
  }

  public static MemberInfo GetDeclaredMember(this Type type, string memberName)
  {
    MemberInfo[] declaredMembers = type.GetDeclaredMembers();
    for (int index = 0; index < declaredMembers.Length; ++index)
    {
      if (declaredMembers[index].Name == memberName)
        return declaredMembers[index];
    }
    return (MemberInfo) null;
  }

  public static MethodInfo[] GetDeclaredMethods(this Type type)
  {
    return type.GetMethods(fsPortableReflection.DeclaredFlags);
  }

  public static PropertyInfo[] GetDeclaredProperties(this Type type)
  {
    return type.GetProperties(fsPortableReflection.DeclaredFlags);
  }

  public static FieldInfo[] GetDeclaredFields(this Type type)
  {
    return type.GetFields(fsPortableReflection.DeclaredFlags);
  }

  public static MemberInfo[] GetDeclaredMembers(this Type type)
  {
    return type.GetMembers(fsPortableReflection.DeclaredFlags);
  }

  public static MemberInfo AsMemberInfo(Type type) => (MemberInfo) type;

  public static bool IsType(MemberInfo member) => member is Type;

  public static Type AsType(MemberInfo member) => (Type) member;

  public static Type Resolve(this Type type) => type;

  private struct AttributeQuery
  {
    public MemberInfo MemberInfo;
    public Type AttributeType;
  }

  private class AttributeQueryComparator : IEqualityComparer<fsPortableReflection.AttributeQuery>
  {
    public bool Equals(fsPortableReflection.AttributeQuery x, fsPortableReflection.AttributeQuery y)
    {
      return x.MemberInfo == y.MemberInfo && x.AttributeType == y.AttributeType;
    }

    public int GetHashCode(fsPortableReflection.AttributeQuery obj)
    {
      return obj.MemberInfo.GetHashCode() + 17 * obj.AttributeType.GetHashCode();
    }
  }
}
