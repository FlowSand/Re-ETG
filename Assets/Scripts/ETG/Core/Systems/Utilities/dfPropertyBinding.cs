// Decompiled with JetBrains decompiler
// Type: dfPropertyBinding
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/Data Binding/Property Binding")]
[Serializable]
public class dfPropertyBinding : MonoBehaviour, IDataBindingComponent
  {
    public dfComponentMemberInfo DataSource;
    public dfComponentMemberInfo DataTarget;
    public string FormatString;
    public bool TwoWay;
    public bool AutoBind = true;
    public bool AutoUnbind = true;
    protected dfObservableProperty sourceProperty;
    protected dfObservableProperty targetProperty;
    protected bool isBound;
    protected bool useFormatString;

    public bool IsBound => this.isBound;

    public virtual void OnEnable()
    {
      if (!this.AutoBind || this.DataSource == null || this.DataTarget == null || this.isBound || !this.DataSource.IsValid || !this.DataTarget.IsValid)
        return;
      this.Bind();
    }

    public virtual void Start()
    {
      if (!this.AutoBind || this.DataSource == null || this.DataTarget == null || this.isBound || !this.DataSource.IsValid || !this.DataTarget.IsValid)
        return;
      this.Bind();
    }

    public virtual void OnDisable()
    {
      if (!this.AutoUnbind)
        return;
      this.Unbind();
    }

    public virtual void OnDestroy() => this.Unbind();

    public virtual void Update()
    {
      if (this.sourceProperty == null || this.targetProperty == null)
        return;
      if (this.sourceProperty.HasChanged)
      {
        this.targetProperty.Value = this.formatValue(this.sourceProperty.Value);
        this.sourceProperty.ClearChangedFlag();
      }
      else
      {
        if (!this.TwoWay || !this.targetProperty.HasChanged)
          return;
        this.sourceProperty.Value = this.targetProperty.Value;
        this.targetProperty.ClearChangedFlag();
      }
    }

    public static dfPropertyBinding Bind(
      Component sourceComponent,
      string sourceProperty,
      Component targetComponent,
      string targetProperty)
    {
      return dfPropertyBinding.Bind(sourceComponent.gameObject, sourceComponent, sourceProperty, targetComponent, targetProperty);
    }

    public static dfPropertyBinding Bind(
      GameObject hostObject,
      Component sourceComponent,
      string sourceProperty,
      Component targetComponent,
      string targetProperty)
    {
      if ((UnityEngine.Object) hostObject == (UnityEngine.Object) null)
        throw new ArgumentNullException(nameof (hostObject));
      if ((UnityEngine.Object) sourceComponent == (UnityEngine.Object) null)
        throw new ArgumentNullException(nameof (sourceComponent));
      if ((UnityEngine.Object) targetComponent == (UnityEngine.Object) null)
        throw new ArgumentNullException(nameof (targetComponent));
      if (string.IsNullOrEmpty(sourceProperty))
        throw new ArgumentNullException(nameof (sourceProperty));
      if (string.IsNullOrEmpty(targetProperty))
        throw new ArgumentNullException(nameof (targetProperty));
      dfPropertyBinding dfPropertyBinding = hostObject.AddComponent<dfPropertyBinding>();
      dfPropertyBinding.DataSource = new dfComponentMemberInfo()
      {
        Component = sourceComponent,
        MemberName = sourceProperty
      };
      dfPropertyBinding.DataTarget = new dfComponentMemberInfo()
      {
        Component = targetComponent,
        MemberName = targetProperty
      };
      dfPropertyBinding.Bind();
      return dfPropertyBinding;
    }

    public virtual bool CanSynchronize()
    {
      return this.DataSource != null && this.DataTarget != null && (this.DataSource.IsValid || this.DataTarget.IsValid) && this.DataTarget.GetMemberType() == this.DataSource.GetMemberType();
    }

    public virtual void Bind()
    {
      if (this.isBound)
        return;
      if (!this.DataSource.IsValid || !this.DataTarget.IsValid)
      {
        Debug.LogError((object) $"Invalid data binding configuration - Source:{this.DataSource}, Target:{this.DataTarget}");
      }
      else
      {
        this.sourceProperty = this.DataSource.GetProperty();
        this.targetProperty = this.DataTarget.GetProperty();
        this.isBound = this.sourceProperty != null && this.targetProperty != null;
        if (!this.isBound)
          return;
        if (this.targetProperty.PropertyType == typeof (string) && this.sourceProperty.PropertyType != typeof (string))
          this.useFormatString = !string.IsNullOrEmpty(this.FormatString);
        this.targetProperty.Value = this.formatValue(this.sourceProperty.Value);
      }
    }

    public virtual void Unbind()
    {
      if (!this.isBound)
        return;
      this.sourceProperty = (dfObservableProperty) null;
      this.targetProperty = (dfObservableProperty) null;
      this.isBound = false;
    }

    private object formatValue(object value)
    {
      try
      {
        if (this.useFormatString)
        {
          if (!string.IsNullOrEmpty(this.FormatString))
            return (object) string.Format(this.FormatString, value);
        }
      }
      catch (FormatException ex)
      {
        Debug.LogError((object) ex, (UnityEngine.Object) this);
        if (Application.isPlaying)
          this.enabled = false;
      }
      return value;
    }

    public override string ToString()
    {
      return $"Bind {(this.DataSource == null || !((UnityEngine.Object) this.DataSource.Component != (UnityEngine.Object) null) ? "[null]" : this.DataSource.Component.GetType().Name)}.{(this.DataSource == null || string.IsNullOrEmpty(this.DataSource.MemberName) ? "[null]" : this.DataSource.MemberName)} -> {(this.DataTarget == null || !((UnityEngine.Object) this.DataTarget.Component != (UnityEngine.Object) null) ? "[null]" : this.DataTarget.Component.GetType().Name)}.{(this.DataTarget == null || string.IsNullOrEmpty(this.DataTarget.MemberName) ? "[null]" : this.DataTarget.MemberName)}";
    }
  }

