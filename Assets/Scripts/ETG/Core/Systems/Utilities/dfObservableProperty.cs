// Decompiled with JetBrains decompiler
// Type: dfObservableProperty
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#nullable disable

public class dfObservableProperty : IObservableValue
  {
    private static object[] tempArray = new object[1];
    private object lastValue;
    private bool hasChanged;
    private object target;
    private FieldInfo fieldInfo;
    private PropertyInfo propertyInfo;
    private MethodInfo propertyGetter;
    private MethodInfo propertySetter;
    private System.Type propertyType;
    private bool canWrite;

    internal dfObservableProperty(object target, string memberName)
    {
      this.initMember(target, ((IEnumerable<MemberInfo>) target.GetType().GetMember(memberName, BindingFlags.Instance | BindingFlags.Public)).FirstOrDefault<MemberInfo>() ?? throw new ArgumentException("Invalid property or field name: " + memberName, nameof (memberName)));
    }

    internal dfObservableProperty(object target, FieldInfo field) => this.initField(target, field);

    internal dfObservableProperty(object target, PropertyInfo property)
    {
      this.initProperty(target, property);
    }

    internal dfObservableProperty(object target, MemberInfo member)
    {
      this.initMember(target, member);
    }

    public System.Type PropertyType
    {
      get => this.fieldInfo != null ? this.fieldInfo.FieldType : this.propertyInfo.PropertyType;
    }

    public object Value
    {
      get => this.getter();
      set
      {
        this.lastValue = value;
        this.setter(value);
        this.hasChanged = false;
      }
    }

    public bool HasChanged
    {
      get
      {
        if (this.hasChanged)
          return true;
        object objA = this.getter();
        this.hasChanged = !object.ReferenceEquals(objA, this.lastValue) && (objA == null || this.lastValue == null || !objA.Equals(this.lastValue));
        return this.hasChanged;
      }
    }

    public void ClearChangedFlag()
    {
      this.hasChanged = false;
      this.lastValue = this.getter();
    }

    private void initMember(object target, MemberInfo member)
    {
      if (member is FieldInfo)
        this.initField(target, (FieldInfo) member);
      else
        this.initProperty(target, (PropertyInfo) member);
    }

    private void initField(object target, FieldInfo field)
    {
      this.target = target;
      this.fieldInfo = field;
      this.Value = this.getter();
    }

    private void initProperty(object target, PropertyInfo property)
    {
      this.target = target;
      this.propertyInfo = property;
      this.propertyGetter = property.GetGetMethod();
      this.propertySetter = property.GetSetMethod();
      this.canWrite = this.propertySetter != null;
      this.Value = this.getter();
    }

    private object getter()
    {
      return this.propertyInfo != null ? this.getPropertyValue() : this.getFieldValue();
    }

    private void setter(object value)
    {
      if (this.propertyInfo != null)
        this.setPropertyValue(value);
      else
        this.setFieldValue(value);
    }

    private object getPropertyValue() => this.propertyGetter.Invoke(this.target, (object[]) null);

    private void setPropertyValue(object value)
    {
      if (!this.canWrite)
        return;
      if (this.propertyType == null)
        this.propertyType = this.propertyInfo.PropertyType;
      dfObservableProperty.tempArray[0] = value == null || this.propertyType.IsAssignableFrom(value.GetType()) ? value : Convert.ChangeType(value, this.propertyType);
      this.propertySetter.Invoke(this.target, dfObservableProperty.tempArray);
    }

    private void setFieldValue(object value)
    {
      if (this.fieldInfo.IsLiteral)
        return;
      if (this.propertyType == null)
        this.propertyType = this.fieldInfo.FieldType;
      if (value == null || this.propertyType.IsAssignableFrom(value.GetType()))
        this.fieldInfo.SetValue(this.target, value);
      else
        this.fieldInfo.SetValue(this.target, Convert.ChangeType(value, this.propertyType));
    }

    private void setFieldValueNOP(object value)
    {
    }

    private object getFieldValue() => this.fieldInfo.GetValue(this.target);

    private delegate object ValueGetter();

    private delegate void ValueSetter(object value);
  }

