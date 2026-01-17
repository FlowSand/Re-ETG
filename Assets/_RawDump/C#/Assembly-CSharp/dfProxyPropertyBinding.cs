// Decompiled with JetBrains decompiler
// Type: dfProxyPropertyBinding
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
[AddComponentMenu("Daikon Forge/Data Binding/Proxy Property Binding")]
[Serializable]
public class dfProxyPropertyBinding : MonoBehaviour, IDataBindingComponent
{
  public dfComponentMemberInfo DataSource;
  public dfComponentMemberInfo DataTarget;
  public bool TwoWay;
  private dfObservableProperty sourceProperty;
  private dfObservableProperty targetProperty;
  private bool isBound;
  private bool eventsAttached;

  public bool IsBound => this.isBound;

  public void Awake()
  {
  }

  public void OnEnable()
  {
    if (this.isBound || !this.IsDataSourceValid() || !this.DataTarget.IsValid)
      return;
    this.Bind();
  }

  public void Start()
  {
    if (this.isBound || !this.IsDataSourceValid() || !this.DataTarget.IsValid)
      return;
    this.Bind();
  }

  public void OnDisable() => this.Unbind();

  public void Update()
  {
    if (this.sourceProperty == null || this.targetProperty == null)
      return;
    if (this.sourceProperty.HasChanged)
    {
      this.targetProperty.Value = this.sourceProperty.Value;
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

  public void Bind()
  {
    if (this.isBound)
      return;
    if (!this.IsDataSourceValid())
      Debug.LogError((object) $"Invalid data binding configuration - Source:{this.DataSource}, Target:{this.DataTarget}");
    else if (!this.DataTarget.IsValid)
    {
      Debug.LogError((object) $"Invalid data binding configuration - Source:{this.DataSource}, Target:{this.DataTarget}");
    }
    else
    {
      this.sourceProperty = (this.DataSource.Component as dfDataObjectProxy).GetProperty(this.DataSource.MemberName);
      this.targetProperty = this.DataTarget.GetProperty();
      this.isBound = this.sourceProperty != null && this.targetProperty != null;
      if (this.isBound)
        this.targetProperty.Value = this.sourceProperty.Value;
      this.attachEvent();
    }
  }

  public void Unbind()
  {
    if (!this.isBound)
      return;
    this.detachEvent();
    this.sourceProperty = (dfObservableProperty) null;
    this.targetProperty = (dfObservableProperty) null;
    this.isBound = false;
  }

  private bool IsDataSourceValid()
  {
    return this.DataSource != null || (UnityEngine.Object) this.DataSource.Component != (UnityEngine.Object) null || !string.IsNullOrEmpty(this.DataSource.MemberName) || (this.DataSource.Component as dfDataObjectProxy).Data != null;
  }

  private void attachEvent()
  {
    if (this.eventsAttached)
      return;
    this.eventsAttached = true;
    dfDataObjectProxy component = this.DataSource.Component as dfDataObjectProxy;
    if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
      return;
    component.DataChanged += new dfDataObjectProxy.DataObjectChangedHandler(this.handle_DataChanged);
  }

  private void detachEvent()
  {
    if (!this.eventsAttached)
      return;
    this.eventsAttached = false;
    dfDataObjectProxy component = this.DataSource.Component as dfDataObjectProxy;
    if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
      return;
    component.DataChanged -= new dfDataObjectProxy.DataObjectChangedHandler(this.handle_DataChanged);
  }

  private void handle_DataChanged(object data)
  {
    this.Unbind();
    if (!this.IsDataSourceValid())
      return;
    this.Bind();
  }

  public override string ToString()
  {
    return $"Bind {(this.DataSource == null || !((UnityEngine.Object) this.DataSource.Component != (UnityEngine.Object) null) ? "[null]" : this.DataSource.Component.GetType().Name)}.{(this.DataSource == null || string.IsNullOrEmpty(this.DataSource.MemberName) ? "[null]" : this.DataSource.MemberName)} -> {(this.DataTarget == null || !((UnityEngine.Object) this.DataTarget.Component != (UnityEngine.Object) null) ? "[null]" : this.DataTarget.Component.GetType().Name)}.{(this.DataTarget == null || string.IsNullOrEmpty(this.DataTarget.MemberName) ? "[null]" : this.DataTarget.MemberName)}";
  }
}
