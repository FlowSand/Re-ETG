// Decompiled with JetBrains decompiler
// Type: FullSerializer.Internal.fsMetaProperty
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Reflection;

#nullable disable
namespace FullSerializer.Internal;

public class fsMetaProperty
{
  private MemberInfo _memberInfo;

  internal fsMetaProperty(FieldInfo field)
  {
    this._memberInfo = (MemberInfo) field;
    this.StorageType = field.FieldType;
    this.JsonName = fsMetaProperty.GetJsonName((MemberInfo) field);
    this.JsonDeserializeOnly = fsMetaProperty.GetJsonDeserializeOnly((MemberInfo) field);
    this.MemberName = field.Name;
    this.IsPublic = field.IsPublic;
    this.CanRead = true;
    this.CanWrite = true;
  }

  internal fsMetaProperty(PropertyInfo property)
  {
    this._memberInfo = (MemberInfo) property;
    this.StorageType = property.PropertyType;
    this.JsonName = fsMetaProperty.GetJsonName((MemberInfo) property);
    this.JsonDeserializeOnly = fsMetaProperty.GetJsonDeserializeOnly((MemberInfo) property);
    this.MemberName = property.Name;
    this.IsPublic = property.GetGetMethod() != null && property.GetGetMethod().IsPublic && property.GetSetMethod() != null && property.GetSetMethod().IsPublic;
    this.CanRead = property.CanRead;
    this.CanWrite = property.CanWrite;
  }

  public Type StorageType { get; private set; }

  public bool CanRead { get; private set; }

  public bool CanWrite { get; private set; }

  public string JsonName { get; private set; }

  public bool JsonDeserializeOnly { get; set; }

  public string MemberName { get; private set; }

  public bool IsPublic { get; private set; }

  public void Write(object context, object value)
  {
    FieldInfo memberInfo1 = this._memberInfo as FieldInfo;
    PropertyInfo memberInfo2 = this._memberInfo as PropertyInfo;
    if (memberInfo1 != null)
      memberInfo1.SetValue(context, value);
    else
      memberInfo2?.GetSetMethod(true)?.Invoke(context, new object[1]
      {
        value
      });
  }

  public object Read(object context)
  {
    return this._memberInfo is PropertyInfo ? ((PropertyInfo) this._memberInfo).GetValue(context, new object[0]) : ((FieldInfo) this._memberInfo).GetValue(context);
  }

  private static string GetJsonName(MemberInfo member)
  {
    fsPropertyAttribute attribute = fsPortableReflection.GetAttribute<fsPropertyAttribute>(member);
    return attribute != null && !string.IsNullOrEmpty(attribute.Name) ? attribute.Name : member.Name;
  }

  private static bool GetJsonDeserializeOnly(MemberInfo member)
  {
    fsPropertyAttribute attribute = fsPortableReflection.GetAttribute<fsPropertyAttribute>(member);
    return attribute != null && attribute.DeserializeOnly;
  }
}
