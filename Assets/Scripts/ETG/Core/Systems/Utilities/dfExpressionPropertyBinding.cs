using System;
using System.Collections.Generic;

using UnityEngine;

#nullable disable

[Obsolete("The expression binding functionality is no longer supported and may be removed in future versions of DFGUI")]
[Serializable]
public class dfExpressionPropertyBinding : MonoBehaviour, IDataBindingComponent
    {
        public Component DataSource;
        public dfComponentMemberInfo DataTarget;
        [SerializeField]
        protected string expression;
        private Delegate compiledExpression;
        private dfObservableProperty targetProperty;
        private bool isBound;

        public bool IsBound => this.isBound;

        public string Expression
        {
            get => this.expression;
            set
            {
                if (string.Equals(value, this.expression))
                    return;
                this.Unbind();
                this.expression = value;
            }
        }

        public void OnDisable() => this.Unbind();

        public void Update()
        {
            if (this.isBound)
            {
                this.evaluate();
            }
            else
            {
                if (!((UnityEngine.Object) this.DataSource != (UnityEngine.Object) null) || string.IsNullOrEmpty(this.expression) || !this.DataTarget.IsValid)
                    return;
                this.Bind();
            }
        }

        public void Unbind()
        {
            if (!this.isBound)
                return;
            this.compiledExpression = (Delegate) null;
            this.targetProperty = (dfObservableProperty) null;
            this.isBound = false;
        }

        public void Bind()
        {
            if (this.isBound || this.DataSource is dfDataObjectProxy && ((dfDataObjectProxy) this.DataSource).Data == null)
                return;
            dfScriptEngineSettings settings = new dfScriptEngineSettings()
            {
                Constants = new Dictionary<string, object>()
                {
                    {
                        "Application",
                        (object) typeof (Application)
                    },
                    {
                        "Color",
                        (object) typeof (Color)
                    },
                    {
                        "Color32",
                        (object) typeof (Color32)
                    },
                    {
                        "Random",
                        (object) typeof (UnityEngine.Random)
                    },
                    {
                        "Time",
                        (object) typeof (UnityEngine.Time)
                    },
                    {
                        "ScriptableObject",
                        (object) typeof (ScriptableObject)
                    },
                    {
                        "Vector2",
                        (object) typeof (Vector2)
                    },
                    {
                        "Vector3",
                        (object) typeof (Vector3)
                    },
                    {
                        "Vector4",
                        (object) typeof (Vector4)
                    },
                    {
                        "Quaternion",
                        (object) typeof (Quaternion)
                    },
                    {
                        "Matrix",
                        (object) typeof (Matrix4x4)
                    },
                    {
                        "Mathf",
                        (object) typeof (Mathf)
                    }
                }
            };
            if (this.DataSource is dfDataObjectProxy)
            {
                dfDataObjectProxy dataSource = this.DataSource as dfDataObjectProxy;
                settings.AddVariable(new dfScriptVariable("source", (object) null, dataSource.DataType));
            }
            else
                settings.AddVariable(new dfScriptVariable("source", (object) this.DataSource));
            this.compiledExpression = dfScriptEngine.CompileExpression(this.expression, settings);
            this.targetProperty = this.DataTarget.GetProperty();
            this.isBound = (object) this.compiledExpression != null && this.targetProperty != null;
        }

        private void evaluate()
        {
            try
            {
                object obj = (object) this.DataSource;
                if (obj is dfDataObjectProxy)
                    obj = ((dfDataObjectProxy) obj).Data;
                this.targetProperty.Value = this.compiledExpression.DynamicInvoke(obj);
            }
            catch (Exception ex)
            {
                Debug.LogError((object) ex);
            }
        }

        public override string ToString()
        {
            return $"Bind [expression] -> {(this.DataTarget == null || !((UnityEngine.Object) this.DataTarget.Component != (UnityEngine.Object) null) ? (object) "[null]" : (object) this.DataTarget.Component.GetType().Name)}.{(this.DataTarget == null || string.IsNullOrEmpty(this.DataTarget.MemberName) ? (object) "[null]" : (object) this.DataTarget.MemberName)}";
        }
    }

