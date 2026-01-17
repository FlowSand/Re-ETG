// Decompiled with JetBrains decompiler
// Type: DaikonForge.Tween.TweenNamedProperty`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using DaikonForge.Tween.Interpolation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#nullable disable
namespace DaikonForge.Tween;

public class TweenNamedProperty<T>
{
  public static DaikonForge.Tween.Tween<T> Obtain(object target, string propertyName)
  {
    return TweenNamedProperty<T>.Obtain(target, propertyName, Interpolators.Get<T>());
  }

  public static DaikonForge.Tween.Tween<T> Obtain(
    object target,
    string propertyName,
    Interpolator<T> interpolator)
  {
    Type type = target != null ? target.GetType() : throw new ArgumentException("Target object cannot be NULL");
    MemberInfo member = TweenNamedProperty<T>.getMember(type, propertyName);
    if (member == null)
      throw new ArgumentException($"Failed to find property {type.Name}.{propertyName}");
    bool flag = false;
    if (member is FieldInfo)
    {
      if (((FieldInfo) member).FieldType != typeof (T))
        flag = true;
    }
    else if (((PropertyInfo) member).PropertyType != typeof (T))
      flag = true;
    if (flag)
      throw new InvalidCastException($"{type.Name}.{member.Name} cannot be cast to type {typeof (T).Name}");
    T obj = TweenNamedProperty<T>.get(target, type, member);
    return DaikonForge.Tween.Tween<T>.Obtain().SetStartValue(obj).SetEndValue(obj).SetInterpolator(interpolator).OnExecute(TweenNamedProperty<T>.set(target, type, member));
  }

  public static T GetCurrentValue(object target, string propertyName)
  {
    Type type = target.GetType();
    return TweenNamedProperty<T>.get(target, type, TweenNamedProperty<T>.getMember(type, propertyName) ?? throw new ArgumentException($"Failed to find property {type.Name}.{propertyName}"));
  }

  private static MethodInfo getGetMethod(PropertyInfo property) => property.GetGetMethod();

  private static MethodInfo getSetMethod(PropertyInfo property) => property.GetSetMethod();

  private static MemberInfo getMember(Type type, string propertyName)
  {
    return ((IEnumerable<MemberInfo>) type.GetMember(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)).FirstOrDefault<MemberInfo>();
  }

  private static T get(object target, Type type, MemberInfo member)
  {
    switch (member)
    {
      case PropertyInfo _:
        return (T) (TweenNamedProperty<T>.getGetMethod((PropertyInfo) member) ?? throw new ArgumentException($"Property {type.Name}.{member.Name} cannot be read")).Invoke(target, (object[]) null);
      case FieldInfo _:
        return (T) ((FieldInfo) member).GetValue(target);
      default:
        throw new ArgumentException($"Failed to find property {type.Name}.{member.Name}");
    }
  }

  private static TweenAssignmentCallback<T> set(object target, Type type, MemberInfo member)
  {
    switch (member)
    {
      case PropertyInfo _:
        return TweenNamedProperty<T>.setProperty(target, type, (PropertyInfo) member);
      case FieldInfo _:
        return TweenNamedProperty<T>.setField(target, type, (FieldInfo) member);
      default:
        throw new ArgumentException($"Failed to find property {type.Name}.{member.Name}");
    }
  }

  private static TweenAssignmentCallback<T> setField(object target, Type type, FieldInfo field)
  {
    if (field.IsLiteral)
      throw new ArgumentException($"Property {type.Name}.{field.Name} cannot be assigned");
    return (TweenAssignmentCallback<T>) (result => field.SetValue(target, (object) result));
  }

  private static TweenAssignmentCallback<T> setProperty(
    object target,
    Type type,
    PropertyInfo property)
  {
    MethodInfo setter = TweenNamedProperty<T>.getSetMethod(property);
    if (setter == null)
      throw new ArgumentException($"Property {type.Name}.{property.Name} cannot be assigned");
    object[] paramArray = new object[1];
    return (TweenAssignmentCallback<T>) (result =>
    {
      paramArray[0] = (object) result;
      setter.Invoke(target, paramArray);
    });
  }
}
