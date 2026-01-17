// Decompiled with JetBrains decompiler
// Type: dfEventDrivenPropertyBinding
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [AddComponentMenu("Daikon Forge/Data Binding/Event-Driven Property Binding")]
    [Serializable]
    public class dfEventDrivenPropertyBinding : dfPropertyBinding
    {
      public string SourceEventName;
      public string TargetEventName;
      protected dfEventBinding sourceEventBinding;
      protected dfEventBinding targetEventBinding;

      public override void Update()
      {
      }

      public static dfEventDrivenPropertyBinding Bind(
        Component sourceComponent,
        string sourceProperty,
        string sourceEvent,
        Component targetComponent,
        string targetProperty,
        string targetEvent)
      {
        return dfEventDrivenPropertyBinding.Bind(sourceComponent.gameObject, sourceComponent, sourceProperty, sourceEvent, targetComponent, targetProperty, targetEvent);
      }

      public static dfEventDrivenPropertyBinding Bind(
        GameObject hostObject,
        Component sourceComponent,
        string sourceProperty,
        string sourceEvent,
        Component targetComponent,
        string targetProperty,
        string targetEvent)
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
        if (string.IsNullOrEmpty(sourceEvent))
          throw new ArgumentNullException(nameof (sourceEvent));
        dfEventDrivenPropertyBinding drivenPropertyBinding = hostObject.AddComponent<dfEventDrivenPropertyBinding>();
        drivenPropertyBinding.DataSource = new dfComponentMemberInfo()
        {
          Component = sourceComponent,
          MemberName = sourceProperty
        };
        drivenPropertyBinding.DataTarget = new dfComponentMemberInfo()
        {
          Component = targetComponent,
          MemberName = targetProperty
        };
        drivenPropertyBinding.SourceEventName = sourceEvent;
        drivenPropertyBinding.TargetEventName = targetEvent;
        drivenPropertyBinding.Bind();
        return drivenPropertyBinding;
      }

      public override void Bind()
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
          if (this.sourceProperty == null || this.targetProperty == null)
            return;
          if (!string.IsNullOrEmpty(this.SourceEventName) && this.SourceEventName.Trim() != string.Empty)
            this.bindSourceEvent();
          if (!string.IsNullOrEmpty(this.TargetEventName) && this.TargetEventName.Trim() != string.Empty)
            this.bindTargetEvent();
          else if (this.targetProperty.PropertyType == typeof (string) && this.sourceProperty.PropertyType != typeof (string))
            this.useFormatString = !string.IsNullOrEmpty(this.FormatString);
          this.MirrorSourceProperty();
          this.isBound = (UnityEngine.Object) this.sourceEventBinding != (UnityEngine.Object) null;
        }
      }

      public override void Unbind()
      {
        if (!this.isBound)
          return;
        this.isBound = false;
        if ((UnityEngine.Object) this.sourceEventBinding != (UnityEngine.Object) null)
        {
          this.sourceEventBinding.Unbind();
          UnityEngine.Object.Destroy((UnityEngine.Object) this.sourceEventBinding);
          this.sourceEventBinding = (dfEventBinding) null;
        }
        if (!((UnityEngine.Object) this.targetEventBinding != (UnityEngine.Object) null))
          return;
        this.targetEventBinding.Unbind();
        UnityEngine.Object.Destroy((UnityEngine.Object) this.targetEventBinding);
        this.targetEventBinding = (dfEventBinding) null;
      }

      public void MirrorSourceProperty()
      {
        this.targetProperty.Value = this.formatValue(this.sourceProperty.Value);
      }

      public void MirrorTargetProperty() => this.sourceProperty.Value = this.targetProperty.Value;

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

      private void bindSourceEvent()
      {
        this.sourceEventBinding = this.gameObject.AddComponent<dfEventBinding>();
        this.sourceEventBinding.hideFlags = HideFlags.HideAndDontSave | HideFlags.HideInInspector;
        this.sourceEventBinding.DataSource = new dfComponentMemberInfo()
        {
          Component = this.DataSource.Component,
          MemberName = this.SourceEventName
        };
        this.sourceEventBinding.DataTarget = new dfComponentMemberInfo()
        {
          Component = (Component) this,
          MemberName = "MirrorSourceProperty"
        };
        this.sourceEventBinding.Bind();
      }

      private void bindTargetEvent()
      {
        this.targetEventBinding = this.gameObject.AddComponent<dfEventBinding>();
        this.targetEventBinding.hideFlags = HideFlags.HideAndDontSave | HideFlags.HideInInspector;
        this.targetEventBinding.DataSource = new dfComponentMemberInfo()
        {
          Component = this.DataTarget.Component,
          MemberName = this.TargetEventName
        };
        this.targetEventBinding.DataTarget = new dfComponentMemberInfo()
        {
          Component = (Component) this,
          MemberName = "MirrorTargetProperty"
        };
        this.targetEventBinding.Bind();
      }

      public override string ToString()
      {
        return $"Bind {(this.DataSource == null || !((UnityEngine.Object) this.DataSource.Component != (UnityEngine.Object) null) ? "[null]" : this.DataSource.Component.GetType().Name)}.{(this.DataSource == null || string.IsNullOrEmpty(this.DataSource.MemberName) ? "[null]" : this.DataSource.MemberName)} -> {(this.DataTarget == null || !((UnityEngine.Object) this.DataTarget.Component != (UnityEngine.Object) null) ? "[null]" : this.DataTarget.Component.GetType().Name)}.{(this.DataTarget == null || string.IsNullOrEmpty(this.DataTarget.MemberName) ? "[null]" : this.DataTarget.MemberName)}";
      }
    }

}
