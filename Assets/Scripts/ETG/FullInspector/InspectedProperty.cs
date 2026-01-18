using FullInspector.Internal;
using FullSerializer;
using FullSerializer.Internal;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

#nullable disable
namespace FullInspector
{
  public sealed class InspectedProperty
  {
    public string Name;
    public string DisplayName;
    private bool? _isPublicCache;
    private bool? _isAutoPropertyCache;
    public System.Type StorageType;

    public InspectedProperty(PropertyInfo property)
    {
      this.MemberInfo = (MemberInfo) property;
      this.StorageType = property.PropertyType;
      this.CanWrite = property.GetSetMethod(true) != null;
      this.IsStatic = (property.GetGetMethod(true) ?? property.GetSetMethod(true)).IsStatic;
      this.SetupNames();
    }

    public InspectedProperty(FieldInfo field)
    {
      this.MemberInfo = (MemberInfo) field;
      this.StorageType = field.FieldType;
      this.CanWrite = !field.IsLiteral;
      this.IsStatic = field.IsStatic;
      this.SetupNames();
    }

    public MemberInfo MemberInfo { get; private set; }

    public bool IsPublic
    {
      get
      {
        if (!this._isPublicCache.HasValue)
        {
          if (this.MemberInfo is FieldInfo memberInfo1)
            this._isPublicCache = new bool?(memberInfo1.IsPublic);
          if (this.MemberInfo is PropertyInfo memberInfo2)
            this._isPublicCache = new bool?(memberInfo2.GetGetMethod(false) != null || memberInfo2.GetSetMethod(false) != null);
          if (!this._isPublicCache.HasValue)
            this._isPublicCache = new bool?(false);
        }
        return this._isPublicCache.Value;
      }
    }

    public bool IsAutoProperty
    {
      get
      {
        if (!this._isAutoPropertyCache.HasValue)
        {
          if (!(this.MemberInfo is PropertyInfo))
          {
            this._isAutoPropertyCache = new bool?(false);
          }
          else
          {
            string str = $"<{this.MemberInfo.Name}>k__BackingField";
            bool flag = false;
            foreach (FieldInfo declaredField in this.MemberInfo.DeclaringType.GetDeclaredFields())
            {
              if (fsPortableReflection.HasAttribute<CompilerGeneratedAttribute>((MemberInfo) declaredField) && declaredField.Name == str)
              {
                flag = true;
                break;
              }
            }
            this._isAutoPropertyCache = new bool?(flag);
          }
        }
        return this._isAutoPropertyCache.Value;
      }
    }

    public bool IsStatic { get; private set; }

    public bool CanWrite { get; private set; }

    public void Write(object context, object value)
    {
      try
      {
        FieldInfo memberInfo1 = this.MemberInfo as FieldInfo;
        PropertyInfo memberInfo2 = this.MemberInfo as PropertyInfo;
        if (memberInfo1 != null)
        {
          if (memberInfo1.IsLiteral)
            return;
          memberInfo1.SetValue(context, value);
        }
        else
          memberInfo2?.GetSetMethod(true)?.Invoke(context, new object[1]
          {
            value
          });
      }
      catch (Exception ex)
      {
        Debug.LogWarning((object) $"Caught exception when writing property {this.Name} with context={context} and value={value}");
        Debug.LogException(ex);
      }
    }

    public object Read(object context)
    {
      try
      {
        return this.MemberInfo is PropertyInfo ? ((PropertyInfo) this.MemberInfo).GetValue(context, new object[0]) : ((FieldInfo) this.MemberInfo).GetValue(context);
      }
      catch (Exception ex)
      {
        Debug.LogWarning((object) $"Caught exception when reading property {this.Name} with  context={context}; returning default value for {this.StorageType.CSharpName()}");
        Debug.LogException(ex);
        return this.DefaultValue;
      }
    }

    public object DefaultValue
    {
      get
      {
        return this.StorageType.Resolve().IsValueType ? InspectedType.Get(this.StorageType).CreateInstance() : (object) null;
      }
    }

    private void SetupNames()
    {
      this.Name = this.MemberInfo.Name;
      InspectorNameAttribute attribute = fsPortableReflection.GetAttribute<InspectorNameAttribute>(this.MemberInfo);
      if (attribute != null)
        this.DisplayName = attribute.DisplayName;
      if (!string.IsNullOrEmpty(this.DisplayName))
        return;
      this.DisplayName = fiDisplayNameMapper.Map(this.Name);
    }

    public override bool Equals(object obj)
    {
      return obj != null && obj is InspectedProperty inspectedProperty && this.StorageType == inspectedProperty.StorageType && this.Name == inspectedProperty.Name;
    }

    public bool Equals(InspectedProperty p)
    {
      return p != null && this.StorageType == p.StorageType && this.Name == p.Name;
    }

    public override int GetHashCode() => this.StorageType.GetHashCode() ^ this.Name.GetHashCode();
  }
}
