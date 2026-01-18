using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/Data Binding/Proxy Data Object")]
[Serializable]
public class dfDataObjectProxy : MonoBehaviour, IDataBindingComponent
  {
    [SerializeField]
    protected string typeName;
    private object data;

    public event dfDataObjectProxy.DataObjectChangedHandler DataChanged;

    public bool IsBound => this.data != null;

    public string TypeName
    {
      get => this.typeName;
      set
      {
        if (!(this.typeName != value))
          return;
        this.typeName = value;
        this.Data = (object) null;
      }
    }

    public System.Type DataType => this.getTypeFromName(this.typeName);

    public object Data
    {
      get => this.data;
      set
      {
        if (object.ReferenceEquals(value, this.data))
          return;
        this.data = value;
        if (value != null)
          this.typeName = value.GetType().Name;
        if (this.DataChanged == null)
          return;
        this.DataChanged(value);
      }
    }

    public void Start()
    {
      if (this.DataType != null)
        return;
      Debug.LogError((object) ("Unable to retrieve System.Type reference for type: " + this.TypeName));
    }

    public System.Type GetPropertyType(string propertyName)
    {
      System.Type dataType = this.DataType;
      if (dataType == null)
        return (System.Type) null;
      MemberInfo memberInfo = ((IEnumerable<MemberInfo>) dataType.GetMember(propertyName, BindingFlags.Instance | BindingFlags.Public)).FirstOrDefault<MemberInfo>();
      switch (memberInfo)
      {
        case FieldInfo _:
          return ((FieldInfo) memberInfo).FieldType;
        case PropertyInfo _:
          return ((PropertyInfo) memberInfo).PropertyType;
        default:
          return (System.Type) null;
      }
    }

    public dfObservableProperty GetProperty(string PropertyName)
    {
      return this.data == null ? (dfObservableProperty) null : new dfObservableProperty(this.data, PropertyName);
    }

    private System.Type getTypeFromName(string nameOfType)
    {
      if (nameOfType == null)
        throw new ArgumentNullException(nameof (nameOfType));
      return ((IEnumerable<System.Type>) this.GetType().GetAssembly().GetTypes()).FirstOrDefault<System.Type>((Func<System.Type, bool>) (t => t.Name == nameOfType));
    }

    private static System.Type getTypeFromQualifiedName(string typeName)
    {
      System.Type type = System.Type.GetType(typeName);
      if (type != null)
        return type;
      if (typeName.IndexOf('.') == -1)
        return (System.Type) null;
      return Assembly.Load(new AssemblyName(typeName.Substring(0, typeName.IndexOf('.'))))?.GetType(typeName);
    }

    public void Bind()
    {
    }

    public void Unbind()
    {
    }

    [dfEventCategory("Data Changed")]
    public delegate void DataObjectChangedHandler(object data);
  }

