using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/Tweens/Tween Event Binding")]
[Serializable]
public class dfTweenEventBinding : MonoBehaviour
    {
        public Component Tween;
        public Component EventSource;
        public string StartEvent;
        public string StopEvent;
        public string ResetEvent;
        private bool isBound;
        private dfEventBinding startEventBinding;
        private dfEventBinding stopEventBinding;
        private dfEventBinding resetEventBinding;

        private void OnEnable()
        {
            if (!this.isValid())
                return;
            this.Bind();
        }

        private void Start()
        {
            if (!this.isValid())
                return;
            this.Bind();
        }

        private void OnDisable() => this.Unbind();

        public void Bind()
        {
            if (this.isBound && !this.isValid())
                return;
            this.isBound = true;
            if (!string.IsNullOrEmpty(this.StartEvent))
                this.startEventBinding = this.bindEvent(this.StartEvent, "Play");
            if (!string.IsNullOrEmpty(this.StopEvent))
                this.stopEventBinding = this.bindEvent(this.StopEvent, "Stop");
            if (string.IsNullOrEmpty(this.ResetEvent))
                return;
            this.resetEventBinding = this.bindEvent(this.ResetEvent, "Reset");
        }

        public void Unbind()
        {
            if (!this.isBound)
                return;
            this.isBound = false;
            if ((UnityEngine.Object) this.startEventBinding != (UnityEngine.Object) null)
            {
                this.startEventBinding.Unbind();
                this.startEventBinding = (dfEventBinding) null;
            }
            if ((UnityEngine.Object) this.stopEventBinding != (UnityEngine.Object) null)
            {
                this.stopEventBinding.Unbind();
                this.stopEventBinding = (dfEventBinding) null;
            }
            if (!((UnityEngine.Object) this.resetEventBinding != (UnityEngine.Object) null))
                return;
            this.resetEventBinding.Unbind();
            this.resetEventBinding = (dfEventBinding) null;
        }

        private bool isValid()
        {
            if ((UnityEngine.Object) this.Tween == (UnityEngine.Object) null || !(this.Tween is dfTweenComponentBase) || (UnityEngine.Object) this.EventSource == (UnityEngine.Object) null || string.IsNullOrEmpty(this.StartEvent) && string.IsNullOrEmpty(this.StopEvent) && string.IsNullOrEmpty(this.ResetEvent))
                return false;
            System.Type type = this.EventSource.GetType();
            return (string.IsNullOrEmpty(this.StartEvent) || this.getField(type, this.StartEvent) != null) && (string.IsNullOrEmpty(this.StopEvent) || this.getField(type, this.StopEvent) != null) && (string.IsNullOrEmpty(this.ResetEvent) || this.getField(type, this.ResetEvent) != null);
        }

        private FieldInfo getField(System.Type type, string fieldName)
        {
            return ((IEnumerable<FieldInfo>) type.GetAllFields()).Where<FieldInfo>((Func<FieldInfo, bool>) (f => f.Name == fieldName)).FirstOrDefault<FieldInfo>();
        }

        private void unbindEvent(FieldInfo eventField, Delegate eventDelegate)
        {
            Delegate @delegate = Delegate.Remove((Delegate) eventField.GetValue((object) this.EventSource), eventDelegate);
            eventField.SetValue((object) this.EventSource, (object) @delegate);
        }

        private dfEventBinding bindEvent(string eventName, string handlerName)
        {
            if (this.Tween.GetType().GetMethod(handlerName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) == null)
                throw new MissingMemberException("Method not found: " + handlerName);
            dfEventBinding dfEventBinding = this.gameObject.AddComponent<dfEventBinding>();
            dfEventBinding.hideFlags = HideFlags.HideAndDontSave | HideFlags.HideInInspector;
            dfEventBinding.DataSource = new dfComponentMemberInfo()
            {
                Component = this.EventSource,
                MemberName = eventName
            };
            dfEventBinding.DataTarget = new dfComponentMemberInfo()
            {
                Component = this.Tween,
                MemberName = handlerName
            };
            dfEventBinding.Bind();
            return dfEventBinding;
        }
    }

