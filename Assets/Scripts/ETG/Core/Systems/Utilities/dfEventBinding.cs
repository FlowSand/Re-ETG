using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/Data Binding/Event Binding")]
[Serializable]
public class dfEventBinding : MonoBehaviour, IDataBindingComponent
    {
        public dfComponentMemberInfo DataSource;
        public dfComponentMemberInfo DataTarget;
        public bool AutoBind = true;
        public bool AutoUnbind = true;
        private bool isBound;
        private Component sourceComponent;
        private Component targetComponent;
        private EventInfo eventInfo;
        private FieldInfo eventField;
        private Delegate eventDelegate;
        private MethodInfo handlerProxy;
        private ParameterInfo[] handlerParameters;

        public bool IsBound => this.isBound;

        public void OnEnable()
        {
            if (!this.AutoBind || this.DataSource == null || this.isBound || !this.DataSource.IsValid || !this.DataTarget.IsValid)
                return;
            this.Bind();
        }

        public void Start()
        {
            if (!this.AutoBind || this.DataSource == null || this.isBound || !this.DataSource.IsValid || !this.DataTarget.IsValid)
                return;
            this.Bind();
        }

        public void OnDisable()
        {
            if (!this.AutoUnbind)
                return;
            this.Unbind();
        }

        public void OnDestroy() => this.Unbind();

        public void Bind()
        {
            if (this.isBound || this.DataSource == null)
                return;
            if (!this.DataSource.IsValid || !this.DataTarget.IsValid)
            {
                Debug.LogError((object) $"Invalid event binding configuration - Source:{this.DataSource}, Target:{this.DataTarget}");
            }
            else
            {
                this.sourceComponent = this.DataSource.Component;
                this.targetComponent = this.DataTarget.Component;
                MethodInfo method = this.DataTarget.GetMethod();
                if (method == null)
                    Debug.LogError((object) $"Event handler not found: {this.targetComponent.GetType().Name}.{this.DataTarget.MemberName}");
                else if (this.bindToEventProperty(method))
                {
                    this.isBound = true;
                }
                else
                {
                    if (!this.bindToEventField(method))
                        return;
                    this.isBound = true;
                }
            }
        }

        public void Unbind()
        {
            if (!this.isBound)
                return;
            this.isBound = false;
            if (this.eventField != null)
                this.eventField.SetValue((object) this.sourceComponent, (object) Delegate.Remove((Delegate) this.eventField.GetValue((object) this.sourceComponent), this.eventDelegate));
            else if (this.eventInfo != null)
                this.eventInfo.GetRemoveMethod().Invoke((object) this.sourceComponent, new object[1]
                {
                    (object) this.eventDelegate
                });
            this.eventInfo = (EventInfo) null;
            this.eventField = (FieldInfo) null;
            this.eventDelegate = (Delegate) null;
            this.handlerProxy = (MethodInfo) null;
            this.sourceComponent = (Component) null;
            this.targetComponent = (Component) null;
        }

        public override string ToString()
        {
            return $"Bind {(this.DataSource == null || !((UnityEngine.Object) this.DataSource.Component != (UnityEngine.Object) null) ? "[null]" : this.DataSource.Component.GetType().Name)}.{(this.DataSource == null || string.IsNullOrEmpty(this.DataSource.MemberName) ? "[null]" : this.DataSource.MemberName)} -> {(this.DataTarget == null || !((UnityEngine.Object) this.DataTarget.Component != (UnityEngine.Object) null) ? "[null]" : this.DataTarget.Component.GetType().Name)}.{(this.DataTarget == null || string.IsNullOrEmpty(this.DataTarget.MemberName) ? "[null]" : this.DataTarget.MemberName)}";
        }

        [dfEventProxy]
        [HideInInspector]
        public void NotificationEventProxy() => this.callProxyEventHandler();

        [HideInInspector]
        [dfEventProxy]
        public void GenericCallbackProxy(object sender) => this.callProxyEventHandler(sender);

        [HideInInspector]
        [dfEventProxy]
        public void AnimationEventProxy(dfTweenPlayableBase tween)
        {
            this.callProxyEventHandler((object) tween);
        }

        [dfEventProxy]
        [HideInInspector]
        public void MouseEventProxy(dfControl control, dfMouseEventArgs mouseEvent)
        {
            this.callProxyEventHandler((object) control, (object) mouseEvent);
        }

        [dfEventProxy]
        [HideInInspector]
        public void KeyEventProxy(dfControl control, dfKeyEventArgs keyEvent)
        {
            this.callProxyEventHandler((object) control, (object) keyEvent);
        }

        [dfEventProxy]
        [HideInInspector]
        public void DragEventProxy(dfControl control, dfDragEventArgs dragEvent)
        {
            this.callProxyEventHandler((object) control, (object) dragEvent);
        }

        [dfEventProxy]
        [HideInInspector]
        public void ChildControlEventProxy(dfControl container, dfControl child)
        {
            this.callProxyEventHandler((object) container, (object) child);
        }

        [dfEventProxy]
        [HideInInspector]
        public void FocusEventProxy(dfControl control, dfFocusEventArgs args)
        {
            this.callProxyEventHandler((object) control, (object) args);
        }

        [HideInInspector]
        [dfEventProxy]
        public void PropertyChangedProxy(dfControl control, int value)
        {
            this.callProxyEventHandler((object) control, (object) value);
        }

        [HideInInspector]
        [dfEventProxy]
        public void PropertyChangedProxy(dfControl control, float value)
        {
            this.callProxyEventHandler((object) control, (object) value);
        }

        [dfEventProxy]
        [HideInInspector]
        public void PropertyChangedProxy(dfControl control, bool value)
        {
            this.callProxyEventHandler((object) control, (object) value);
        }

        [HideInInspector]
        [dfEventProxy]
        public void PropertyChangedProxy(dfControl control, string value)
        {
            this.callProxyEventHandler((object) control, (object) value);
        }

        [HideInInspector]
        [dfEventProxy]
        public void PropertyChangedProxy(dfControl control, Vector2 value)
        {
            this.callProxyEventHandler((object) control, (object) value);
        }

        [HideInInspector]
        [dfEventProxy]
        public void PropertyChangedProxy(dfControl control, Vector3 value)
        {
            this.callProxyEventHandler((object) control, (object) value);
        }

        [dfEventProxy]
        [HideInInspector]
        public void PropertyChangedProxy(dfControl control, Vector4 value)
        {
            this.callProxyEventHandler((object) control, (object) value);
        }

        [dfEventProxy]
        [HideInInspector]
        public void PropertyChangedProxy(dfControl control, Quaternion value)
        {
            this.callProxyEventHandler((object) control, (object) value);
        }

        [HideInInspector]
        [dfEventProxy]
        public void PropertyChangedProxy(dfControl control, dfButton.ButtonState value)
        {
            this.callProxyEventHandler((object) control, (object) value);
        }

        [dfEventProxy]
        [HideInInspector]
        public void PropertyChangedProxy(dfControl control, dfPivotPoint value)
        {
            this.callProxyEventHandler((object) control, (object) value);
        }

        [HideInInspector]
        [dfEventProxy]
        public void PropertyChangedProxy(dfControl control, Texture value)
        {
            this.callProxyEventHandler((object) control, (object) value);
        }

        [dfEventProxy]
        [HideInInspector]
        public void PropertyChangedProxy(dfControl control, Texture2D value)
        {
            this.callProxyEventHandler((object) control, (object) value);
        }

        [HideInInspector]
        [dfEventProxy]
        public void PropertyChangedProxy(dfControl control, Material value)
        {
            this.callProxyEventHandler((object) control, (object) value);
        }

        [HideInInspector]
        [dfEventProxy]
        public void SystemEventHandlerProxy(object sender, EventArgs args)
        {
            this.callProxyEventHandler(sender, (object) args);
        }

        private bool bindToEventField(MethodInfo eventHandler)
        {
            this.eventField = dfEventBinding.getField(this.sourceComponent, this.DataSource.MemberName);
            if (this.eventField == null)
                return false;
            try
            {
                MethodInfo method = this.eventField.FieldType.GetMethod("Invoke");
                ParameterInfo[] parameters1 = method.GetParameters();
                ParameterInfo[] parameters2 = eventHandler.GetParameters();
                this.eventDelegate = parameters1.Length != parameters2.Length || method.ReturnType != eventHandler.ReturnType ? this.createEventProxyDelegate((object) this.targetComponent, this.eventField.FieldType, parameters1, eventHandler) : Delegate.CreateDelegate(this.eventField.FieldType, (object) this.targetComponent, eventHandler, true);
                this.eventField.SetValue((object) this.sourceComponent, (object) Delegate.Combine(this.eventDelegate, (Delegate) this.eventField.GetValue((object) this.sourceComponent)));
            }
            catch (Exception ex)
            {
                this.enabled = false;
                Debug.LogError((object) $"Event binding failed - Failed to create event handler for {this.DataSource} ({eventHandler}) - {ex.ToString()}", (UnityEngine.Object) this);
                return false;
            }
            return true;
        }

        private bool bindToEventProperty(MethodInfo eventHandler)
        {
            this.eventInfo = this.sourceComponent.GetType().GetEvent(this.DataSource.MemberName);
            if (this.eventInfo == null)
                return false;
            try
            {
                System.Type eventHandlerType = this.eventInfo.EventHandlerType;
                MethodInfo addMethod = this.eventInfo.GetAddMethod();
                MethodInfo method = eventHandlerType.GetMethod("Invoke");
                ParameterInfo[] parameters1 = method.GetParameters();
                ParameterInfo[] parameters2 = eventHandler.GetParameters();
                this.eventDelegate = parameters1.Length != parameters2.Length || method.ReturnType != eventHandler.ReturnType ? this.createEventProxyDelegate((object) this.targetComponent, eventHandlerType, parameters1, eventHandler) : Delegate.CreateDelegate(eventHandlerType, (object) this.targetComponent, eventHandler, true);
                addMethod.Invoke((object) this.DataSource.Component, new object[1]
                {
                    (object) this.eventDelegate
                });
            }
            catch (Exception ex)
            {
                this.enabled = false;
                Debug.LogError((object) $"Event binding failed - Failed to create event handler for {this.DataSource} ({eventHandler}) - {ex.ToString()}", (UnityEngine.Object) this);
                return false;
            }
            return true;
        }

        private void callProxyEventHandler(params object[] arguments)
        {
            if (this.handlerProxy == null)
                return;
            if (this.handlerParameters.Length == 0)
                arguments = (object[]) null;
            object routine = this.handlerProxy.Invoke((object) this.targetComponent, arguments);
            if (!(routine is IEnumerator) || !(this.targetComponent is MonoBehaviour))
                return;
            ((MonoBehaviour) this.targetComponent).StartCoroutine((IEnumerator) routine);
        }

        private static FieldInfo getField(Component component, string fieldName)
        {
            if ((UnityEngine.Object) component == (UnityEngine.Object) null)
                throw new ArgumentNullException(nameof (component));
            return ((IEnumerable<FieldInfo>) component.GetType().GetAllFields()).FirstOrDefault<FieldInfo>((Func<FieldInfo, bool>) (f => f.Name == fieldName));
        }

        private Delegate createEventProxyDelegate(
            object target,
            System.Type delegateType,
            ParameterInfo[] eventParams,
            MethodInfo eventHandler)
        {
            MethodInfo method = ((IEnumerable<MethodInfo>) typeof (dfEventBinding).GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)).Where<MethodInfo>((Func<MethodInfo, bool>) (m => m.IsDefined(typeof (dfEventProxyAttribute), true) && this.signatureIsCompatible(eventParams, m.GetParameters()))).FirstOrDefault<MethodInfo>();
            if (method == null)
                return (Delegate) null;
            this.handlerProxy = eventHandler;
            this.handlerParameters = eventHandler.GetParameters();
            return Delegate.CreateDelegate(delegateType, (object) this, method, true);
        }

        private bool signatureIsCompatible(ParameterInfo[] lhs, ParameterInfo[] rhs)
        {
            if (lhs == null || rhs == null || lhs.Length != rhs.Length)
                return false;
            for (int index = 0; index < lhs.Length; ++index)
            {
                if (!this.areTypesCompatible(lhs[index], rhs[index]))
                    return false;
            }
            return true;
        }

        private bool areTypesCompatible(ParameterInfo lhs, ParameterInfo rhs)
        {
            return lhs.ParameterType.Equals(rhs.ParameterType) || lhs.ParameterType.IsAssignableFrom(rhs.ParameterType);
        }
    }

