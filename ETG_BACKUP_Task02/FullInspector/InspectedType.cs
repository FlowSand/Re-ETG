// Decompiled with JetBrains decompiler
// Type: FullInspector.InspectedType
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullInspector.Internal;
using FullSerializer;
using FullSerializer.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.Serialization;

#nullable disable
namespace FullInspector;

public sealed class InspectedType
{
  private static Dictionary<System.Type, InspectedType> _cachedMetadata = new Dictionary<System.Type, InspectedType>();
  private bool? _hasDefaultConstructorCache;
  private List<InspectedMember> _allMembers;
  private Dictionary<IInspectedMemberFilter, List<InspectedMember>> _cachedMembers;
  private Dictionary<IInspectedMemberFilter, List<InspectedProperty>> _cachedProperties;
  private Dictionary<IInspectedMemberFilter, List<InspectedMethod>> _cachedMethods;
  private bool _isArray;
  private Dictionary<IInspectedMemberFilter, Dictionary<string, List<InspectedMember>>> _categoryCache = new Dictionary<IInspectedMemberFilter, Dictionary<string, List<InspectedMember>>>();
  private Dictionary<string, InspectedProperty> _nameToProperty;
  private Dictionary<string, InspectedProperty> _formerlySerializedAsPropertyNames;

  static InspectedType() => InspectedType.InitializePropertyRemoval();

  internal InspectedType(System.Type type)
  {
    this.ReflectedType = type;
    this._isArray = type.IsArray;
    this.IsCollection = this._isArray || type.IsImplementationOf(typeof (ICollection<>));
    if (this.IsCollection)
      return;
    this._cachedMembers = new Dictionary<IInspectedMemberFilter, List<InspectedMember>>();
    this._cachedProperties = new Dictionary<IInspectedMemberFilter, List<InspectedProperty>>();
    this._cachedMethods = new Dictionary<IInspectedMemberFilter, List<InspectedMethod>>();
    this._allMembers = new List<InspectedMember>();
    if (this.ReflectedType.Resolve().BaseType != null)
      this._allMembers.AddRange((IEnumerable<InspectedMember>) InspectedType.Get(this.ReflectedType.Resolve().BaseType)._allMembers);
    List<InspectedMember> list = InspectedType.CollectUnorderedLocalMembers(type).ToList<InspectedMember>();
    InspectedType.StableSort<InspectedMember>((IList<InspectedMember>) list, (Func<InspectedMember, InspectedMember, int>) ((a, b) => Math.Sign(InspectorOrderAttribute.GetInspectorOrder(a.MemberInfo) - InspectorOrderAttribute.GetInspectorOrder(b.MemberInfo))));
    this._allMembers.AddRange((IEnumerable<InspectedMember>) list);
    this._nameToProperty = new Dictionary<string, InspectedProperty>();
    this._formerlySerializedAsPropertyNames = new Dictionary<string, InspectedProperty>();
    foreach (InspectedMember allMember in this._allMembers)
    {
      if (allMember.IsProperty)
      {
        if (fiSettings.EmitWarnings && this._nameToProperty.ContainsKey(allMember.Name))
          Debug.LogWarning((object) $"Duplicate property with name={allMember.Name} detected on {this.ReflectedType.CSharpName()}");
        this._nameToProperty[allMember.Name] = allMember.Property;
        foreach (FormerlySerializedAsAttribute customAttribute in allMember.MemberInfo.GetCustomAttributes(typeof (FormerlySerializedAsAttribute), true))
          this._nameToProperty[customAttribute.oldName] = allMember.Property;
      }
    }
  }

  public static InspectedType Get(System.Type type)
  {
    InspectedType inspectedType;
    if (!InspectedType._cachedMetadata.TryGetValue(type, out inspectedType))
    {
      inspectedType = new InspectedType(type);
      InspectedType._cachedMetadata[type] = inspectedType;
    }
    return inspectedType;
  }

  public bool HasDefaultConstructor
  {
    get
    {
      if (!this._hasDefaultConstructorCache.HasValue)
        this._hasDefaultConstructorCache = !this._isArray ? (!this.ReflectedType.Resolve().IsValueType ? new bool?(this.ReflectedType.GetDeclaredConstructor(fsPortableReflection.EmptyTypes) != null) : new bool?(true)) : new bool?(true);
      return this._hasDefaultConstructorCache.Value;
    }
  }

  public object CreateInstance()
  {
    if (typeof (ScriptableObject).IsAssignableFrom(this.ReflectedType))
      return (object) ScriptableObject.CreateInstance(this.ReflectedType);
    if (typeof (Component).IsAssignableFrom(this.ReflectedType))
    {
      GameObject activeObject = fiLateBindings.Selection.activeObject as GameObject;
      if ((UnityEngine.Object) activeObject != (UnityEngine.Object) null)
      {
        Component component = activeObject.GetComponent(this.ReflectedType);
        return (UnityEngine.Object) component != (UnityEngine.Object) null ? (object) component : (object) activeObject.AddComponent(this.ReflectedType);
      }
      Debug.LogWarning((object) ("No selected game object; constructing an unformatted instance (which will be null) for " + (object) this.ReflectedType));
      return FormatterServices.GetSafeUninitializedObject(this.ReflectedType);
    }
    if (!this.HasDefaultConstructor)
      return FormatterServices.GetSafeUninitializedObject(this.ReflectedType);
    if (this._isArray)
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

  public List<InspectedMember> GetMembers(IInspectedMemberFilter filter)
  {
    this.VerifyNotCollection();
    List<InspectedMember> members;
    if (!this._cachedMembers.TryGetValue(filter, out members))
    {
      members = new List<InspectedMember>();
      for (int index = 0; index < this._allMembers.Count; ++index)
      {
        InspectedMember allMember = this._allMembers[index];
        if (!allMember.IsProperty ? filter.IsInterested(allMember.Method) : filter.IsInterested(allMember.Property))
          members.Add(allMember);
      }
      this._cachedMembers[filter] = members;
    }
    return members;
  }

  public List<InspectedProperty> GetProperties(IInspectedMemberFilter filter)
  {
    this.VerifyNotCollection();
    List<InspectedProperty> list;
    if (!this._cachedProperties.TryGetValue(filter, out list))
    {
      list = this.GetMembers(filter).Where<InspectedMember>((Func<InspectedMember, bool>) (member => member.IsProperty)).Select<InspectedMember, InspectedProperty>((Func<InspectedMember, InspectedProperty>) (member => member.Property)).ToList<InspectedProperty>();
      this._cachedProperties[filter] = list;
    }
    return list;
  }

  public List<InspectedMethod> GetMethods(IInspectedMemberFilter filter)
  {
    this.VerifyNotCollection();
    List<InspectedMethod> list;
    if (!this._cachedMethods.TryGetValue(filter, out list))
    {
      list = this.GetMembers(filter).Where<InspectedMember>((Func<InspectedMember, bool>) (member => member.IsMethod)).Select<InspectedMember, InspectedMethod>((Func<InspectedMember, InspectedMethod>) (member => member.Method)).ToList<InspectedMethod>();
      this._cachedMethods[filter] = list;
    }
    return list;
  }

  private void VerifyNotCollection()
  {
    if (this.IsCollection)
      throw new InvalidOperationException($"Operation not valid -- {(object) this.ReflectedType} is a collection");
  }

  public static void StableSort<T>(IList<T> list, Func<T, T, int> comparator)
  {
    for (int index1 = 1; index1 < list.Count; ++index1)
    {
      T obj = list[index1];
      int index2;
      for (index2 = index1 - 1; index2 >= 0 && comparator(list[index2], obj) > 0; --index2)
        list[index2 + 1] = list[index2];
      list[index2 + 1] = obj;
    }
  }

  private static List<InspectedMember> CollectUnorderedLocalMembers(System.Type reflectedType)
  {
    List<InspectedMember> inspectedMemberList = new List<InspectedMember>();
    foreach (MemberInfo declaredMember in reflectedType.GetDeclaredMembers())
    {
      PropertyInfo property = declaredMember as PropertyInfo;
      FieldInfo field = declaredMember as FieldInfo;
      if (property != null)
      {
        MethodInfo getMethod = property.GetGetMethod(true);
        MethodInfo setMethod = property.GetSetMethod(true);
        if ((getMethod == null || getMethod == getMethod.GetBaseDefinition()) && (setMethod == null || setMethod == setMethod.GetBaseDefinition()))
          inspectedMemberList.Add(new InspectedMember(new InspectedProperty(property)));
      }
      else if (field != null)
        inspectedMemberList.Add(new InspectedMember(new InspectedProperty(field)));
    }
    foreach (MethodInfo declaredMethod in reflectedType.GetDeclaredMethods())
    {
      if (declaredMethod == declaredMethod.GetBaseDefinition())
        inspectedMemberList.Add(new InspectedMember(new InspectedMethod(declaredMethod)));
    }
    return inspectedMemberList;
  }

  public System.Type ReflectedType { get; private set; }

  public bool IsCollection { get; private set; }

  public Dictionary<string, List<InspectedMember>> GetCategories(IInspectedMemberFilter filter)
  {
    this.VerifyNotCollection();
    Dictionary<string, List<InspectedMember>> categories;
    if (!this._categoryCache.TryGetValue(filter, out categories))
    {
      categories = new Dictionary<string, List<InspectedMember>>();
      this._categoryCache[filter] = categories;
      foreach (InspectedMember member in this.GetMembers(filter))
      {
        bool flag = false;
        foreach (InspectorCategoryAttribute customAttribute in member.MemberInfo.GetCustomAttributes(typeof (InspectorCategoryAttribute), true))
        {
          if (!categories.ContainsKey(customAttribute.Category))
          {
            categories[customAttribute.Category] = new List<InspectedMember>();
            if (customAttribute.Category == "Conditions")
              categories["Cond. (All)"] = new List<InspectedMember>();
          }
          categories[customAttribute.Category].Add(member);
          if (customAttribute.Category == "Conditions")
            categories["Cond. (All)"].Add(member);
          flag = true;
        }
        if (!flag)
        {
          if (!categories.ContainsKey("Default"))
            categories["Default"] = new List<InspectedMember>();
          categories["Default"].Add(member);
        }
      }
    }
    if (categories.Count == 1 && categories.ContainsKey("Default"))
      categories.Clear();
    return categories;
  }

  public InspectedProperty GetPropertyByName(string name)
  {
    this.VerifyNotCollection();
    InspectedProperty inspectedProperty;
    return !this._nameToProperty.TryGetValue(name, out inspectedProperty) ? (InspectedProperty) null : inspectedProperty;
  }

  public InspectedProperty GetPropertyByFormerlySerializedName(string name)
  {
    this.VerifyNotCollection();
    InspectedProperty inspectedProperty;
    return !this._formerlySerializedAsPropertyNames.TryGetValue(name, out inspectedProperty) ? (InspectedProperty) null : inspectedProperty;
  }

  private static void InitializePropertyRemoval()
  {
    InspectedType.RemoveProperty<IntPtr>("m_value");
    InspectedType.RemoveProperty<UnityEngine.Object>("m_UnityRuntimeReferenceData");
    InspectedType.RemoveProperty<UnityEngine.Object>("m_UnityRuntimeErrorString");
    InspectedType.RemoveProperty<UnityEngine.Object>("name");
    InspectedType.RemoveProperty<UnityEngine.Object>("hideFlags");
    InspectedType.RemoveProperty<Component>("active");
    InspectedType.RemoveProperty<Component>("tag");
    InspectedType.RemoveProperty<Behaviour>("enabled");
    InspectedType.RemoveProperty<MonoBehaviour>("useGUILayout");
  }

  public static void RemoveProperty<T>(string propertyName)
  {
    InspectedType inspectedType = InspectedType.Get(typeof (T));
    inspectedType._nameToProperty.Remove(propertyName);
    inspectedType._cachedMembers = new Dictionary<IInspectedMemberFilter, List<InspectedMember>>();
    inspectedType._cachedMethods = new Dictionary<IInspectedMemberFilter, List<InspectedMethod>>();
    inspectedType._cachedProperties = new Dictionary<IInspectedMemberFilter, List<InspectedProperty>>();
    for (int index = 0; index < inspectedType._allMembers.Count; ++index)
    {
      InspectedMember allMember = inspectedType._allMembers[index];
      if (propertyName == allMember.Name)
      {
        inspectedType._allMembers.RemoveAt(index);
        break;
      }
    }
  }

  private static bool IsSimpleTypeThatUnityCanSerialize(System.Type type)
  {
    return !InspectedType.IsPrimitiveSkippedByUnity(type) && (type.Resolve().IsPrimitive || type == typeof (string));
  }

  private static bool IsPrimitiveSkippedByUnity(System.Type type)
  {
    return type == typeof (ushort) || type == typeof (uint) || type == typeof (ulong) || type == typeof (sbyte);
  }

  public static bool IsSerializedByUnity(InspectedProperty property)
  {
    if (property.MemberInfo is PropertyInfo || property.IsStatic || (property.MemberInfo as FieldInfo).IsInitOnly || !property.IsPublic && !property.MemberInfo.IsDefined(typeof (SerializeField), true))
      return false;
    System.Type storageType = property.StorageType;
    if (InspectedType.IsSimpleTypeThatUnityCanSerialize(storageType) || typeof (UnityEngine.Object).IsAssignableFrom(storageType) && !storageType.Resolve().IsGenericType || storageType.IsArray && !storageType.GetElementType().IsArray && InspectedType.IsSimpleTypeThatUnityCanSerialize(storageType.GetElementType()))
      return true;
    return storageType.Resolve().IsGenericType && storageType.GetGenericTypeDefinition() == typeof (List<>) && InspectedType.IsSimpleTypeThatUnityCanSerialize(storageType.GetGenericArguments()[0]);
  }

  public static bool IsSerializedByFullInspector(InspectedProperty property)
  {
    if (property.IsStatic || typeof (BaseObject).Resolve().IsAssignableFrom(property.StorageType.Resolve()))
      return false;
    MemberInfo memberInfo = property.MemberInfo;
    if (fsPortableReflection.HasAttribute<NonSerializedAttribute>(memberInfo) || fsPortableReflection.HasAttribute<NotSerializedAttribute>(memberInfo))
      return false;
    foreach (System.Type optOutAnnotation in fiInstalledSerializerManager.SerializationOptOutAnnotations)
    {
      if (memberInfo.IsDefined(optOutAnnotation, true))
        return false;
    }
    if (fsPortableReflection.HasAttribute<SerializeField>(memberInfo) || fsPortableReflection.HasAttribute<SerializableAttribute>(memberInfo))
      return true;
    foreach (System.Type serializationOptInAnnotation in fiInstalledSerializerManager.SerializationOptInAnnotations)
    {
      if (memberInfo.IsDefined(serializationOptInAnnotation, true))
        return true;
    }
    return (!(property.MemberInfo is PropertyInfo) || fiSettings.SerializeAutoProperties && property.IsAutoProperty) && property.IsPublic;
  }
}
