// Decompiled with JetBrains decompiler
// Type: FullSerializer.fsMetaType
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullSerializer.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using UnityEngine;

#nullable disable
namespace FullSerializer;

public class fsMetaType
{
  private static Dictionary<System.Type, fsMetaType> _metaTypes = new Dictionary<System.Type, fsMetaType>();
  public System.Type ReflectedType;
  private bool _hasEmittedAotData;
  private bool? _hasDefaultConstructorCache;
  private bool _isDefaultConstructorPublic;

  private fsMetaType(System.Type reflectedType)
  {
    this.ReflectedType = reflectedType;
    List<fsMetaProperty> properties = new List<fsMetaProperty>();
    fsMetaType.CollectProperties(properties, reflectedType);
    this.Properties = properties.ToArray();
  }

  public static fsMetaType Get(System.Type type)
  {
    fsMetaType fsMetaType;
    if (!fsMetaType._metaTypes.TryGetValue(type, out fsMetaType))
    {
      fsMetaType = new fsMetaType(type);
      fsMetaType._metaTypes[type] = fsMetaType;
    }
    return fsMetaType;
  }

  public static void ClearCache() => fsMetaType._metaTypes = new Dictionary<System.Type, fsMetaType>();

  private static void CollectProperties(List<fsMetaProperty> properties, System.Type reflectedType)
  {
    bool flag = fsConfig.DefaultMemberSerialization == fsMemberSerialization.OptIn;
    bool annotationFreeValue = fsConfig.DefaultMemberSerialization == fsMemberSerialization.OptOut;
    fsObjectAttribute attribute = fsPortableReflection.GetAttribute<fsObjectAttribute>((MemberInfo) reflectedType);
    if (attribute != null)
    {
      flag = attribute.MemberSerialization == fsMemberSerialization.OptIn;
      annotationFreeValue = attribute.MemberSerialization == fsMemberSerialization.OptOut;
    }
    MemberInfo[] declaredMembers = reflectedType.GetDeclaredMembers();
    foreach (MemberInfo memberInfo in declaredMembers)
    {
      MemberInfo member = memberInfo;
      if (!((IEnumerable<System.Type>) fsConfig.IgnoreSerializeAttributes).Any<System.Type>((Func<System.Type, bool>) (t => fsPortableReflection.HasAttribute(member, t))))
      {
        PropertyInfo property = member as PropertyInfo;
        FieldInfo field = member as FieldInfo;
        if ((!flag || ((IEnumerable<System.Type>) fsConfig.SerializeAttributes).Any<System.Type>((Func<System.Type, bool>) (t => fsPortableReflection.HasAttribute(member, t)))) && (!annotationFreeValue || !((IEnumerable<System.Type>) fsConfig.IgnoreSerializeAttributes).Any<System.Type>((Func<System.Type, bool>) (t => fsPortableReflection.HasAttribute(member, t)))))
        {
          if (property != null)
          {
            if (fsMetaType.CanSerializeProperty(property, declaredMembers, annotationFreeValue))
              properties.Add(new fsMetaProperty(property));
          }
          else if (field != null && fsMetaType.CanSerializeField(field, annotationFreeValue))
            properties.Add(new fsMetaProperty(field));
        }
      }
    }
    if (reflectedType.Resolve().BaseType == null)
      return;
    fsMetaType.CollectProperties(properties, reflectedType.Resolve().BaseType);
  }

  private static bool IsAutoProperty(PropertyInfo property, MemberInfo[] members)
  {
    if (!property.CanWrite || !property.CanRead)
      return false;
    string str = $"<{property.Name}>k__BackingField";
    for (int index = 0; index < members.Length; ++index)
    {
      if (members[index].Name == str)
        return true;
    }
    return false;
  }

  private static bool CanSerializeProperty(
    PropertyInfo property,
    MemberInfo[] members,
    bool annotationFreeValue)
  {
    if (typeof (Delegate).IsAssignableFrom(property.PropertyType))
      return false;
    MethodInfo getMethod = property.GetGetMethod(false);
    MethodInfo setMethod = property.GetSetMethod(false);
    if (getMethod != null && getMethod.IsStatic || setMethod != null && setMethod.IsStatic)
      return false;
    if (((IEnumerable<System.Type>) fsConfig.SerializeAttributes).Any<System.Type>((Func<System.Type, bool>) (t => fsPortableReflection.HasAttribute((MemberInfo) property, t))))
      return true;
    if (!property.CanRead || !property.CanWrite)
      return false;
    return (fsConfig.SerializeNonAutoProperties || fsMetaType.IsAutoProperty(property, members)) && getMethod != null && (fsConfig.SerializeNonPublicSetProperties || setMethod != null) || annotationFreeValue;
  }

  private static bool CanSerializeField(FieldInfo field, bool annotationFreeValue)
  {
    return !typeof (Delegate).IsAssignableFrom(field.FieldType) && !field.IsDefined(typeof (CompilerGeneratedAttribute), false) && !field.IsStatic && (((IEnumerable<System.Type>) fsConfig.SerializeAttributes).Any<System.Type>((Func<System.Type, bool>) (t => fsPortableReflection.HasAttribute((MemberInfo) field, t))) || annotationFreeValue || field.IsPublic);
  }

  public bool EmitAotData()
  {
    if (this._hasEmittedAotData)
      return false;
    this._hasEmittedAotData = true;
    for (int index = 0; index < this.Properties.Length; ++index)
    {
      if (!this.Properties[index].IsPublic)
        return false;
    }
    if (!this.HasDefaultConstructor)
      return false;
    fsAotCompilationManager.AddAotCompilation(this.ReflectedType, this.Properties, this._isDefaultConstructorPublic);
    return true;
  }

  public fsMetaProperty[] Properties { get; private set; }

  public bool HasDefaultConstructor
  {
    get
    {
      if (!this._hasDefaultConstructorCache.HasValue)
      {
        if (this.ReflectedType.Resolve().IsArray)
        {
          this._hasDefaultConstructorCache = new bool?(true);
          this._isDefaultConstructorPublic = true;
        }
        else if (this.ReflectedType.Resolve().IsValueType)
        {
          this._hasDefaultConstructorCache = new bool?(true);
          this._isDefaultConstructorPublic = true;
        }
        else
        {
          ConstructorInfo declaredConstructor = this.ReflectedType.GetDeclaredConstructor(fsPortableReflection.EmptyTypes);
          this._hasDefaultConstructorCache = new bool?(declaredConstructor != null);
          if (declaredConstructor != null)
            this._isDefaultConstructorPublic = declaredConstructor.IsPublic;
        }
      }
      return this._hasDefaultConstructorCache.Value;
    }
  }

  public object CreateInstance()
  {
    if (this.ReflectedType.Resolve().IsInterface || this.ReflectedType.Resolve().IsAbstract)
      throw new Exception("Cannot create an instance of an interface or abstract type for " + (object) this.ReflectedType);
    if (typeof (ScriptableObject).IsAssignableFrom(this.ReflectedType))
      return (object) ScriptableObject.CreateInstance(this.ReflectedType);
    if (typeof (string) == this.ReflectedType)
      return (object) string.Empty;
    if (!this.HasDefaultConstructor)
      return FormatterServices.GetSafeUninitializedObject(this.ReflectedType);
    if (this.ReflectedType.Resolve().IsArray)
      return (object) Array.CreateInstance(this.ReflectedType.GetElementType(), 0);
    try
    {
      return Activator.CreateInstance(this.ReflectedType, true);
    }
    catch (MissingMethodException ex)
    {
      throw new InvalidOperationException($"Unable to create instance of {(object) this.ReflectedType}; there is no default constructor", (Exception) ex);
    }
    catch (TargetInvocationException ex)
    {
      throw new InvalidOperationException($"Constructor of {(object) this.ReflectedType} threw an exception when creating an instance", (Exception) ex);
    }
    catch (MemberAccessException ex)
    {
      throw new InvalidOperationException("Unable to access constructor of " + (object) this.ReflectedType, (Exception) ex);
    }
  }
}
