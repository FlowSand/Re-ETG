using FullSerializer.Internal;
using System;

#nullable disable
namespace FullInspector.Internal
{
  public static class TypeExtensions
  {
    public static bool IsNullableType(this Type type)
    {
      return type.Resolve().IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>);
    }

    private static bool CompareTypes(Type a, Type b)
    {
      if (a.Resolve().IsGenericType && b.Resolve().IsGenericType && (a.Resolve().IsGenericTypeDefinition || b.Resolve().IsGenericTypeDefinition))
      {
        a = a.GetGenericTypeDefinition();
        b = b.GetGenericTypeDefinition();
      }
      return a == b;
    }

    public static bool HasParent(this Type type, Type parentType)
    {
      if (TypeExtensions.CompareTypes(type, parentType))
        return false;
      if (parentType.IsAssignableFrom(type))
        return true;
      for (; type != null; type = type.Resolve().BaseType)
      {
        if (TypeExtensions.CompareTypes(type, parentType))
          return true;
        foreach (Type a in type.GetInterfaces())
        {
          if (TypeExtensions.CompareTypes(a, parentType))
            return true;
        }
      }
      return false;
    }

    public static Type GetInterface(this Type type, Type interfaceType)
    {
      if (interfaceType.Resolve().IsGenericType && !interfaceType.Resolve().IsGenericTypeDefinition)
        throw new ArgumentException("GetInterface requires that if the interface type is generic, then it must be the generic type definition, not a specific generic type instantiation");
      for (; type != null; type = type.Resolve().BaseType)
      {
        foreach (Type type1 in type.GetInterfaces())
        {
          if (type1.Resolve().IsGenericType)
          {
            if (interfaceType == type1.GetGenericTypeDefinition())
              return type1;
          }
          else if (interfaceType == type1)
            return type1;
        }
      }
      return (Type) null;
    }

    public static bool IsImplementationOf(this Type type, Type interfaceType)
    {
      if (interfaceType.Resolve().IsGenericType && !interfaceType.Resolve().IsGenericTypeDefinition)
        throw new ArgumentException("IsImplementationOf requires that if the interface type is generic, then it must be the generic type definition, not a specific generic type instantiation");
      if (type.Resolve().IsGenericType)
        type = type.GetGenericTypeDefinition();
      for (; type != null; type = type.Resolve().BaseType)
      {
        foreach (Type type1 in type.GetInterfaces())
        {
          if (type1.Resolve().IsGenericType)
          {
            if (interfaceType == type1.GetGenericTypeDefinition())
              return true;
          }
          else if (interfaceType == type1)
            return true;
        }
      }
      return false;
    }
  }
}
