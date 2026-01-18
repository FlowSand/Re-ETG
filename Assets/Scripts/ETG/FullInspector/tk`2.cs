using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using FullInspector.Internal;
using FullSerializer;
using FullSerializer.Internal;
using UnityEngine;

#nullable disable
namespace FullInspector
{
    public class tk<T, TContext>
    {
        public static tk<T, TContext>.Value<TValue> Val<TValue>(
            tk<T, TContext>.Value<TValue>.GeneratorNoContext generator)
        {
            return (tk<T, TContext>.Value<TValue>) generator;
        }

        public static tk<T, TContext>.Value<TValue> Val<TValue>(
            tk<T, TContext>.Value<TValue>.Generator generator)
        {
            return (tk<T, TContext>.Value<TValue>) generator;
        }

        public static tk<T, TContext>.Value<TValue> Val<TValue>(TValue value)
        {
            return (tk<T, TContext>.Value<TValue>) value;
        }

        public class Box : tkControl<T, TContext>
        {
            [ShowInInspector]
            private readonly tkControl<T, TContext> _control;

            public Box(tkControl<T, TContext> control) => this._control = control;

            protected override T DoEdit(Rect rect, T obj, TContext context, fiGraphMetadata metadata)
            {
                GUI.Box(rect, string.Empty);
                return this._control.Edit(rect, obj, context, metadata);
            }

            protected override float DoGetHeight(T obj, TContext context, fiGraphMetadata metadata)
            {
                return this._control.GetHeight(obj, context, metadata);
            }
        }

        public class Button : tkControl<T, TContext>
        {
            [ShowInInspector]
            private readonly tk<T, TContext>.Value<fiGUIContent> _label;
            private readonly bool _enabled;
            private readonly Action<T, TContext> _onClick;

            public Button(string methodName)
            {
                InspectedMethod foundMethod = (InspectedMethod) null;
                foreach (InspectedMethod method in InspectedType.Get(typeof (T)).GetMethods(InspectedMemberFilters.All))
                {
                    if (method.Method.Name == methodName)
                        foundMethod = method;
                }
                if (foundMethod != null)
                {
                    this._label = (tk<T, TContext>.Value<fiGUIContent>) (fiGUIContent) foundMethod.DisplayLabel;
                    this._enabled = true;
                    this._onClick = (Action<T, TContext>) ((o, c) => foundMethod.Invoke((object) o));
                }
                else
                {
                    Debug.LogError((object) $"Unable to find method {methodName} on {typeof (T).CSharpName()}");
                    this._label = (tk<T, TContext>.Value<fiGUIContent>) new fiGUIContent($"{methodName} (unable to find on {typeof (T).CSharpName()})");
                    this._enabled = false;
                    this._onClick = (Action<T, TContext>) ((o, c) => { });
                }
            }

            public Button(tk<T, TContext>.Value<fiGUIContent> label, Action<T, TContext> onClick)
            {
                this._enabled = true;
                this._label = label;
                this._onClick = onClick;
            }

            public Button(fiGUIContent label, Action<T, TContext> onClick)
                : this(tk<T, TContext>.Val<fiGUIContent>(label), onClick)
            {
            }

            protected override T DoEdit(Rect rect, T obj, TContext context, fiGraphMetadata metadata)
            {
                fiLateBindings.EditorGUI.BeginDisabledGroup(!this._enabled);
                if (GUI.Button(rect, (GUIContent) this._label.GetCurrentValue(obj, context)))
                    this._onClick(obj, context);
                fiLateBindings.EditorGUI.EndDisabledGroup();
                return obj;
            }

            protected override float DoGetHeight(T obj, TContext context, fiGraphMetadata metadata) => 18f;
        }

        public class CenterVertical : tkControl<T, TContext>
        {
            [ShowInInspector]
            private readonly tkControl<T, TContext> _centered;

            public CenterVertical(tkControl<T, TContext> centered) => this._centered = centered;

            protected override T DoEdit(Rect rect, T obj, TContext context, fiGraphMetadata metadata)
            {
                float num = rect.height - this._centered.GetHeight(obj, context, metadata);
                rect.y += num / 2f;
                rect.height -= num;
                return this._centered.Edit(rect, obj, context, metadata);
            }

            protected override float DoGetHeight(T obj, TContext context, fiGraphMetadata metadata)
            {
                return this._centered.GetHeight(obj, context, metadata);
            }
        }

        public class Color : tk<T, TContext>.ColorIf
        {
            public Color(tk<T, TContext>.Value<UnityEngine.Color> color)
                : base(tk<T, TContext>.Val<bool>((tk<T, TContext>.Value<bool>.GeneratorNoContext) (o => true)), color)
            {
            }
        }

        public class ColorIf : tk<T, TContext>.ConditionalStyle
        {
            public ColorIf(
                tk<T, TContext>.Value<bool> shouldActivate,
                tk<T, TContext>.Value<UnityEngine.Color> color)
                : base(new Func<T, TContext, bool>(shouldActivate.GetCurrentValue), (Func<T, TContext, object>) ((obj, context) =>
            {
                UnityEngine.Color color1 = GUI.color;
                GUI.color = color.GetCurrentValue(obj, context);
                return (object) color1;
            }), (Action<T, TContext, object>) ((obj, context, state) => GUI.color = (UnityEngine.Color) state))
            {
            }

            public ColorIf(
                tk<T, TContext>.Value<bool>.Generator shouldActivate,
                tk<T, TContext>.Value<UnityEngine.Color> color)
                : this(tk<T, TContext>.Val<bool>(shouldActivate), color)
            {
            }

            public ColorIf(
                tk<T, TContext>.Value<bool>.GeneratorNoContext shouldActivate,
                tk<T, TContext>.Value<UnityEngine.Color> color)
                : this(tk<T, TContext>.Val<bool>(shouldActivate), color)
            {
            }
        }

        public class Comment : tkControl<T, TContext>
        {
            private readonly tk<T, TContext>.Value<string> _comment;
            private readonly CommentType _commentType;

            public Comment(tk<T, TContext>.Value<string> comment, CommentType commentType)
            {
                this._comment = comment;
                this._commentType = commentType;
            }

            protected override T DoEdit(Rect rect, T obj, TContext context, fiGraphMetadata metadata)
            {
                string currentValue = this._comment.GetCurrentValue(obj, context);
                fiLateBindings.EditorGUI.HelpBox(rect, currentValue, this._commentType);
                return obj;
            }

            protected override float DoGetHeight(T obj, TContext context, fiGraphMetadata metadata)
            {
                return (float) fiCommentUtility.GetCommentHeight(this._comment.GetCurrentValue(obj, context), this._commentType);
            }
        }

        public class ConditionalStyle : tkStyle<T, TContext>
        {
            private readonly Func<T, TContext, bool> _shouldActivate;
            private readonly Func<T, TContext, object> _activate;
            private readonly Action<T, TContext, object> _deactivate;
            private readonly fiStackValue<bool> _activatedStack = new fiStackValue<bool>();
            private readonly fiStackValue<object> _activationStateStack = new fiStackValue<object>();

            public ConditionalStyle(
                Func<T, TContext, bool> shouldActivate,
                Func<T, TContext, object> activate,
                Action<T, TContext, object> deactivate)
            {
                this._shouldActivate = shouldActivate;
                this._activate = activate;
                this._deactivate = deactivate;
            }

            public override void Activate(T obj, TContext context)
            {
                bool flag = this._shouldActivate(obj, context);
                this._activatedStack.Push(flag);
                if (!flag)
                    return;
                this._activationStateStack.Push(this._activate(obj, context));
            }

            public override void Deactivate(T obj, TContext context)
            {
                if (!this._activatedStack.Pop())
                    return;
                this._deactivate(obj, context, this._activationStateStack.Pop());
            }
        }

        public class Context : tkControl<T, TContext>
        {
            [ShowInInspector]
            private tkControl<T, TContext> _control;
            [ShowInInspector]
            private readonly fiStackValue<T> _data = new fiStackValue<T>();

            public tkControl<T, TContext> With(tkControl<T, TContext> control)
            {
                this._control = control;
                return (tkControl<T, TContext>) this;
            }

            public T Data => this._data.Value;

            protected override T DoEdit(Rect rect, T obj, TContext context, fiGraphMetadata metadata)
            {
                this._data.Push(obj);
                obj = this._control.Edit(rect, obj, context, metadata);
                this._data.Pop();
                return obj;
            }

            protected override float DoGetHeight(T obj, TContext context, fiGraphMetadata metadata)
            {
                this._data.Push(obj);
                float height = this._control.GetHeight(obj, context, metadata);
                this._data.Pop();
                return height;
            }
        }

        public class DefaultInspector : tkControl<T, TContext>
        {
            private readonly System.Type type_fitkControlPropertyEditor = TypeCache.FindType("FullInspector.Internal.tkControlPropertyEditor");
            private readonly System.Type type_IObjectPropertyEditor = TypeCache.FindType("FullInspector.Modules.Common.IObjectPropertyEditor");

            protected override T DoEdit(Rect rect, T obj, TContext context, fiGraphMetadata metadata)
            {
                return (T) fiLateBindings.PropertyEditor.EditSkipUntilNot(new System.Type[2]
                {
                    this.type_fitkControlPropertyEditor,
                    this.type_IObjectPropertyEditor
                }, typeof (T), (MemberInfo) typeof (T).Resolve(), rect, GUIContent.none, (object) obj, new fiGraphMetadataChild()
                {
                    Metadata = metadata
                });
            }

            protected override float DoGetHeight(T obj, TContext context, fiGraphMetadata metadata)
            {
                return fiLateBindings.PropertyEditor.GetElementHeightSkipUntilNot(new System.Type[2]
                {
                    this.type_fitkControlPropertyEditor,
                    this.type_IObjectPropertyEditor
                }, typeof (T), (MemberInfo) typeof (T).Resolve(), GUIContent.none, (object) obj, new fiGraphMetadataChild()
                {
                    Metadata = metadata
                });
            }
        }

        public class DisableHierarchyMode : tkControl<T, TContext>
        {
            private tkControl<T, TContext> _childControl;

            public DisableHierarchyMode(tkControl<T, TContext> childControl)
            {
                this._childControl = childControl;
            }

            protected override T DoEdit(Rect rect, T obj, TContext context, fiGraphMetadata metadata)
            {
                fiLateBindings.fiEditorGUI.PushHierarchyMode(false);
                T obj1 = this._childControl.Edit(rect, obj, context, metadata);
                fiLateBindings.fiEditorGUI.PopHierarchyMode();
                return obj1;
            }

            protected override float DoGetHeight(T obj, TContext context, fiGraphMetadata metadata)
            {
                return this._childControl.GetHeight(obj, context, metadata);
            }
        }

        public class Empty : tkControl<T, TContext>
        {
            [ShowInInspector]
            private readonly tk<T, TContext>.Value<float> _height;

            public Empty()
                : this((tk<T, TContext>.Value<float>) 0.0f)
            {
            }

            public Empty(tk<T, TContext>.Value<float> height) => this._height = height;

            protected override T DoEdit(Rect rect, T obj, TContext context, fiGraphMetadata metadata)
            {
                return obj;
            }

            protected override float DoGetHeight(T obj, TContext context, fiGraphMetadata metadata)
            {
                return this._height.GetCurrentValue(obj, context);
            }
        }

        public class EnabledIf : tk<T, TContext>.ConditionalStyle
        {
            public EnabledIf(tk<T, TContext>.Value<bool> isEnabled)
                : base((Func<T, TContext, bool>) ((o, c) => !isEnabled.GetCurrentValue(o, c)), (Func<T, TContext, object>) ((obj, context) =>
            {
                fiLateBindings.EditorGUI.BeginDisabledGroup(true);
                return (object) null;
            }), (Action<T, TContext, object>) ((obj, context, state) => fiLateBindings.EditorGUI.EndDisabledGroup()))
            {
            }

            public EnabledIf(tk<T, TContext>.Value<bool>.Generator isEnabled)
                : this(tk<T, TContext>.Val<bool>(isEnabled))
            {
            }

            public EnabledIf(
                tk<T, TContext>.Value<bool>.GeneratorNoContext isEnabled)
                : this(tk<T, TContext>.Val<bool>(isEnabled))
            {
            }
        }

        public class Foldout : tkControl<T, TContext>
        {
            private readonly GUIStyle _foldoutStyle;
            [ShowInInspector]
            private readonly fiGUIContent _label;
            [ShowInInspector]
            private readonly tkControl<T, TContext> _control;
            [ShowInInspector]
            private readonly bool _defaultToExpanded;
            private bool _doNotIndentChildControl;
            public bool? HierarchyMode;

            public Foldout(fiGUIContent label, tkControl<T, TContext> control)
                : this(label, FontStyle.Normal, control)
            {
            }

            public Foldout(fiGUIContent label, FontStyle fontStyle, tkControl<T, TContext> control)
                : this(label, fontStyle, true, control)
            {
            }

            public Foldout(
                fiGUIContent label,
                FontStyle fontStyle,
                bool defaultToExpanded,
                tkControl<T, TContext> control)
            {
                this._label = label;
                this._foldoutStyle = new GUIStyle(fiLateBindings.EditorStyles.foldout)
                {
                    fontStyle = fontStyle
                };
                this._defaultToExpanded = defaultToExpanded;
                this._control = control;
            }

            [ShowInInspector]
            public bool IndentChildControl
            {
                get => !this._doNotIndentChildControl;
                set => this._doNotIndentChildControl = !value;
            }

            private tkFoldoutMetadata GetMetadata(fiGraphMetadata metadata)
            {
                bool wasCreated;
                tkFoldoutMetadata persistentMetadata = this.GetInstanceMetadata(metadata).GetPersistentMetadata<tkFoldoutMetadata>(out wasCreated);
                if (wasCreated)
                    persistentMetadata.IsExpanded = this._defaultToExpanded;
                return persistentMetadata;
            }

            protected override T DoEdit(Rect rect, T obj, TContext context, fiGraphMetadata metadata)
            {
                tkFoldoutMetadata metadata1 = this.GetMetadata(metadata);
                if (this.HierarchyMode.HasValue)
                    fiLateBindings.fiEditorGUI.PushHierarchyMode(this.HierarchyMode.Value);
                Rect rect1 = rect;
            rect1.height = fiLateBindings.EditorGUIUtility.singleLineHeight;
                metadata1.IsExpanded = fiLateBindings.EditorGUI.Foldout(rect1, metadata1.IsExpanded, (GUIContent) this._label, true, this._foldoutStyle);
                if (metadata1.IsExpanded)
                {
                    float num = fiLateBindings.EditorGUIUtility.singleLineHeight + fiLateBindings.EditorGUIUtility.standardVerticalSpacing;
                    Rect rect2 = rect;
                    if (this.IndentChildControl)
                    {
                        rect2.x += 15f;
                        rect2.width -= 15f;
                    }
                    rect2.y += num;
                    rect2.height -= num;
                    obj = this._control.Edit(rect2, obj, context, metadata);
                }
                if (this.HierarchyMode.HasValue)
                    fiLateBindings.fiEditorGUI.PopHierarchyMode();
                return obj;
            }

            protected override float DoGetHeight(T obj, TContext context, fiGraphMetadata metadata)
            {
                tkFoldoutMetadata metadata1 = this.GetMetadata(metadata);
                float height = fiLateBindings.EditorGUIUtility.singleLineHeight;
                if (metadata1.IsExpanded)
                    height = height + fiLateBindings.EditorGUIUtility.standardVerticalSpacing + this._control.GetHeight(obj, context, metadata);
                return height;
            }
        }

        public class HorizontalGroup : tkControl<T, TContext>, IEnumerable
        {
            [ShowInInspector]
            private readonly List<tk<T, TContext>.HorizontalGroup.SectionItem> _items = new List<tk<T, TContext>.HorizontalGroup.SectionItem>();
            private static readonly tkControl<T, TContext> DefaultRule = (tkControl<T, TContext>) new tk<T, TContext>.VerticalGroup();

            protected override IEnumerable<tkIControl> NonMemberChildControls
            {
                get
                {
                    tk_T_TContext__HorizontalGroup___c__Iterator0 memberChildControls = new tk_T_TContext__HorizontalGroup___c__Iterator0()
                    {
                        _this = this
                    };
                    memberChildControls._PC = -2;
                    return (IEnumerable<tkIControl>) memberChildControls;
                }
            }

            public void Add(tkControl<T, TContext> rule) => this.InternalAdd(false, 0.0f, 1f, rule);

            public void Add(bool matchParentHeight, tkControl<T, TContext> rule)
            {
                this.InternalAdd(matchParentHeight, 0.0f, 1f, rule);
            }

            public void Add(float width)
            {
                this.InternalAdd(false, width, 0.0f, tk<T, TContext>.HorizontalGroup.DefaultRule);
            }

            public void Add(float width, tkControl<T, TContext> rule)
            {
                this.InternalAdd(false, width, 0.0f, rule);
            }

            private void InternalAdd(
                bool matchParentHeight,
                float width,
                float fillStrength,
                tkControl<T, TContext> rule)
            {
                if ((double) width < 0.0)
                    throw new ArgumentException("width must be >= 0");
                if ((double) fillStrength < 0.0)
                    throw new ArgumentException("fillStrength must be >= 0");
                this._items.Add(new tk<T, TContext>.HorizontalGroup.SectionItem()
                {
                    MatchParentHeight = matchParentHeight,
                    MinWidth = width,
                    FillStrength = fillStrength,
                    Rule = rule
                });
            }

            private void DoLayout(Rect rect, T obj, TContext context, fiGraphMetadata metadata)
            {
                for (int index = 0; index < this._items.Count; ++index)
                {
                    tk<T, TContext>.HorizontalGroup.SectionItem sectionItem = this._items[index];
                    sectionItem.Layout_IsFlexible = (double) sectionItem.FillStrength > 0.0;
                    this._items[index] = sectionItem;
                }
    label_3:
                float num1 = 0.0f;
                float num2 = 0.0f;
                for (int index = 0; index < this._items.Count; ++index)
                {
                    tk<T, TContext>.HorizontalGroup.SectionItem sectionItem = this._items[index];
                    if (sectionItem.Rule.ShouldShow(obj, context, metadata))
                    {
                        if (sectionItem.Layout_IsFlexible)
                            num2 += sectionItem.FillStrength;
                        else
                            num1 += sectionItem.MinWidth;
                    }
                }
                float num3 = rect.width - num1;
                for (int index = 0; index < this._items.Count; ++index)
                {
                    tk<T, TContext>.HorizontalGroup.SectionItem sectionItem = this._items[index];
                    if (sectionItem.Rule.ShouldShow(obj, context, metadata) && sectionItem.Layout_IsFlexible)
                    {
                        sectionItem.Layout_FlexibleWidth = num3 * sectionItem.FillStrength / num2;
                        this._items[index] = sectionItem;
                        if ((double) sectionItem.Layout_FlexibleWidth < (double) sectionItem.MinWidth)
                        {
                            sectionItem.Layout_IsFlexible = false;
                            this._items[index] = sectionItem;
                            goto label_3;
                        }
                    }
                }
            }

            protected override T DoEdit(Rect rect, T obj, TContext context, fiGraphMetadata metadata)
            {
                this.DoLayout(rect, obj, context, metadata);
                for (int index = 0; index < this._items.Count; ++index)
                {
                    tk<T, TContext>.HorizontalGroup.SectionItem sectionItem = this._items[index];
                    if (sectionItem.Rule.ShouldShow(obj, context, metadata))
                    {
                        float layoutWidth = sectionItem.Layout_Width;
                        Rect rect1 = rect;
            rect1.width = layoutWidth;
                        if (!sectionItem.MatchParentHeight)
                            rect1.height = sectionItem.Rule.GetHeight(obj, context, metadata);
                        obj = sectionItem.Rule.Edit(rect1, obj, context, metadata);
                        rect.x += layoutWidth;
                    }
                }
                return obj;
            }

            protected override float DoGetHeight(T obj, TContext context, fiGraphMetadata metadata)
            {
                float val1 = 0.0f;
                for (int index = 0; index < this._items.Count; ++index)
                {
                    tk<T, TContext>.HorizontalGroup.SectionItem sectionItem = this._items[index];
                    if (sectionItem.Rule.ShouldShow(obj, context, metadata))
                        val1 = Math.Max(val1, sectionItem.Rule.GetHeight(obj, context, metadata));
                }
                return val1;
            }

            IEnumerator IEnumerable.GetEnumerator() => throw new NotSupportedException();

            private struct SectionItem
            {
                private float _minWidth;
                private float _fillStrength;
                public bool MatchParentHeight;
                public tkControl<T, TContext> Rule;
                public bool Layout_IsFlexible;
                public float Layout_FlexibleWidth;

                [ShowInInspector]
                public float MinWidth
                {
                    get => this._minWidth;
                    set => this._minWidth = Math.Max(value, 0.0f);
                }

                [ShowInInspector]
                public float FillStrength
                {
                    get => this._fillStrength;
                    set => this._fillStrength = Math.Max(value, 0.0f);
                }

                public float Layout_Width
                {
                    get => this.Layout_IsFlexible ? this.Layout_FlexibleWidth : this.MinWidth;
                }
            }
        }

        public class Indent : tkControl<T, TContext>
        {
            [ShowInInspector]
            private readonly tk<T, TContext>.Value<float> _indent;
            [ShowInInspector]
            private readonly tkControl<T, TContext> _control;

            public Indent(tkControl<T, TContext> control)
                : this((tk<T, TContext>.Value<float>) 15f, control)
            {
            }

            public Indent(tk<T, TContext>.Value<float> indent, tkControl<T, TContext> control)
            {
                this._indent = indent;
                this._control = control;
            }

            protected override T DoEdit(Rect rect, T obj, TContext context, fiGraphMetadata metadata)
            {
                float currentValue = this._indent.GetCurrentValue(obj, context);
                rect.x += currentValue;
                rect.width -= currentValue;
                return this._control.Edit(rect, obj, context, metadata);
            }

            protected override float DoGetHeight(T obj, TContext context, fiGraphMetadata metadata)
            {
                return this._control.GetHeight(obj, context, metadata);
            }
        }

        public class IntSlider : tkControl<T, TContext>
        {
            private readonly tk<T, TContext>.Value<int> _min;
            private readonly tk<T, TContext>.Value<int> _max;
            private readonly Func<T, TContext, int> _getValue;
            private readonly Action<T, TContext, int> _setValue;
            private readonly tk<T, TContext>.Value<fiGUIContent> _label;

            public IntSlider(
                tk<T, TContext>.Value<int> min,
                tk<T, TContext>.Value<int> max,
                Func<T, TContext, int> getValue,
                Action<T, TContext, int> setValue)
                : this((tk<T, TContext>.Value<fiGUIContent>) fiGUIContent.Empty, min, max, getValue, setValue)
            {
            }

            public IntSlider(
                tk<T, TContext>.Value<fiGUIContent> label,
                tk<T, TContext>.Value<int> min,
                tk<T, TContext>.Value<int> max,
                Func<T, TContext, int> getValue,
                Action<T, TContext, int> setValue)
            {
                this._label = label;
                this._min = min;
                this._max = max;
                this._getValue = getValue;
                this._setValue = setValue;
            }

            protected override T DoEdit(Rect rect, T obj, TContext context, fiGraphMetadata metadata)
            {
                int num1 = this._getValue(obj, context);
                int currentValue1 = this._min.GetCurrentValue(obj, context);
                int currentValue2 = this._max.GetCurrentValue(obj, context);
                fiLateBindings.EditorGUI.BeginChangeCheck();
                int num2 = fiLateBindings.EditorGUI.IntSlider(rect, (GUIContent) this._label.GetCurrentValue(obj, context), num1, currentValue1, currentValue2);
                if (fiLateBindings.EditorGUI.EndChangeCheck())
                    this._setValue(obj, context, num2);
                return obj;
            }

            protected override float DoGetHeight(T obj, TContext context, fiGraphMetadata metadata)
            {
                return fiLateBindings.EditorGUIUtility.singleLineHeight;
            }
        }

        public class Label : tkControl<T, TContext>
        {
            public tk<T, TContext>.Value<fiGUIContent> GUIContent;
            [ShowInInspector]
            private readonly FontStyle _fontStyle;
            [ShowInInspector]
            private readonly tkControl<T, TContext> _control;
            public bool InlineControl;

            public Label(fiGUIContent label)
                : this(label, FontStyle.Normal, (tkControl<T, TContext>) null)
            {
            }

            public Label(tk<T, TContext>.Value<fiGUIContent> label)
                : this(label, FontStyle.Normal, (tkControl<T, TContext>) null)
            {
            }

            public Label(
                tk<T, TContext>.Value<fiGUIContent>.Generator label)
                : this(label, FontStyle.Normal, (tkControl<T, TContext>) null)
            {
            }

            public Label(fiGUIContent label, FontStyle fontStyle)
                : this(label, fontStyle, (tkControl<T, TContext>) null)
            {
            }

            public Label(tk<T, TContext>.Value<fiGUIContent> label, FontStyle fontStyle)
                : this(label, fontStyle, (tkControl<T, TContext>) null)
            {
            }

            public Label(
                tk<T, TContext>.Value<fiGUIContent>.Generator label,
                FontStyle fontStyle)
                : this(label, fontStyle, (tkControl<T, TContext>) null)
            {
            }

            public Label(fiGUIContent label, tkControl<T, TContext> control)
                : this(label, FontStyle.Normal, control)
            {
            }

            public Label(tk<T, TContext>.Value<fiGUIContent> label, tkControl<T, TContext> control)
                : this(label, FontStyle.Normal, control)
            {
            }

            public Label(
                tk<T, TContext>.Value<fiGUIContent>.Generator label,
                tkControl<T, TContext> control)
                : this(label, FontStyle.Normal, control)
            {
            }

            public Label(fiGUIContent label, FontStyle fontStyle, tkControl<T, TContext> control)
                : this(tk<T, TContext>.Val<fiGUIContent>(label), fontStyle, control)
            {
            }

            public Label(
                tk<T, TContext>.Value<fiGUIContent> label,
                FontStyle fontStyle,
                tkControl<T, TContext> control)
            {
                this.GUIContent = label;
                this._fontStyle = fontStyle;
                this._control = control;
            }

            public Label(
                tk<T, TContext>.Value<fiGUIContent>.Generator label,
                FontStyle fontStyle,
                tkControl<T, TContext> control)
                : this(tk<T, TContext>.Val<fiGUIContent>(label), fontStyle, control)
            {
            }

            protected override T DoEdit(Rect rect, T obj, TContext context, fiGraphMetadata metadata)
            {
                fiGUIContent currentValue = this.GUIContent.GetCurrentValue(obj, context);
                Rect position = rect;
                Rect rect1 = rect;
                bool flag = false;
                if (this._control != null && !currentValue.IsEmpty)
                {
                    position.height = fiLateBindings.EditorGUIUtility.singleLineHeight;
                    if (this.InlineControl)
                    {
                        position.width = fiGUI.PushLabelWidth((GUIContent) currentValue, position.width);
                        flag = true;
                        rect1.x += position.width;
                        rect1.width -= position.width;
                    }
                    else
                    {
                        float num = position.height + fiLateBindings.EditorGUIUtility.standardVerticalSpacing;
                        rect1.x += 15f;
                        rect1.width -= 15f;
                        rect1.y += num;
                        rect1.height -= num;
                    }
                }
                if (!currentValue.IsEmpty)
                {
                    GUIStyle label = fiLateBindings.EditorStyles.label;
                    FontStyle fontStyle = label.fontStyle;
                    label.fontStyle = this._fontStyle;
                    GUI.Label(position, (GUIContent) currentValue, label);
                    label.fontStyle = fontStyle;
                }
                if (this._control != null)
                    this._control.Edit(rect1, obj, context, metadata);
                if (flag)
                {
                    double num1 = (double) fiGUI.PopLabelWidth();
                }
                return obj;
            }

            protected override float DoGetHeight(T obj, TContext context, fiGraphMetadata metadata)
            {
                float height1 = 0.0f;
                if (!this.GUIContent.GetCurrentValue(obj, context).IsEmpty)
                    height1 += fiLateBindings.EditorGUIUtility.singleLineHeight;
                if (this._control != null)
                {
                    float height2 = this._control.GetHeight(obj, context, metadata);
                    if (!this.InlineControl)
                        height1 += fiLateBindings.EditorGUIUtility.standardVerticalSpacing + height2;
                }
                return height1;
            }
        }

        public class Margin : tkControl<T, TContext>
        {
            [ShowInInspector]
            private readonly tk<T, TContext>.Value<float> _left;
            [ShowInInspector]
            private readonly tk<T, TContext>.Value<float> _top;
            [ShowInInspector]
            private readonly tk<T, TContext>.Value<float> _right;
            [ShowInInspector]
            private readonly tk<T, TContext>.Value<float> _bottom;
            [ShowInInspector]
            private readonly tkControl<T, TContext> _control;

            public Margin(tk<T, TContext>.Value<float> margin, tkControl<T, TContext> control)
                : this(margin, margin, margin, margin, control)
            {
            }

            public Margin(
                tk<T, TContext>.Value<float> left,
                tk<T, TContext>.Value<float> top,
                tkControl<T, TContext> control)
                : this(left, top, left, top, control)
            {
            }

            public Margin(
                tk<T, TContext>.Value<float> left,
                tk<T, TContext>.Value<float> top,
                tk<T, TContext>.Value<float> right,
                tk<T, TContext>.Value<float> bottom,
                tkControl<T, TContext> control)
            {
                this._left = left;
                this._top = top;
                this._right = right;
                this._bottom = bottom;
                this._control = control;
            }

            protected override T DoEdit(Rect rect, T obj, TContext context, fiGraphMetadata metadata)
            {
                float currentValue1 = this._left.GetCurrentValue(obj, context);
                float currentValue2 = this._right.GetCurrentValue(obj, context);
                float currentValue3 = this._top.GetCurrentValue(obj, context);
                float currentValue4 = this._bottom.GetCurrentValue(obj, context);
                rect.x += currentValue1;
                rect.width -= currentValue1 + currentValue2;
                rect.y += currentValue3;
                rect.height -= currentValue3 + currentValue4;
                return this._control.Edit(rect, obj, context, metadata);
            }

            protected override float DoGetHeight(T obj, TContext context, fiGraphMetadata metadata)
            {
                float currentValue1 = this._top.GetCurrentValue(obj, context);
                float currentValue2 = this._bottom.GetCurrentValue(obj, context);
                return this._control.GetHeight(obj, context, metadata) + currentValue1 + currentValue2;
            }
        }

        public class Popup : tkControl<T, TContext>
        {
            private readonly tk<T, TContext>.Value<fiGUIContent> _label;
            private readonly tk<T, TContext>.Value<GUIContent[]> _options;
            private readonly tk<T, TContext>.Value<int> _currentSelection;
            private readonly tk<T, TContext>.Popup.OnSelectionChanged _onSelectionChanged;

            public Popup(
                tk<T, TContext>.Value<fiGUIContent> label,
                tk<T, TContext>.Value<GUIContent[]> options,
                tk<T, TContext>.Value<int> currentSelection,
                tk<T, TContext>.Popup.OnSelectionChanged onSelectionChanged)
            {
                this._label = label;
                this._options = options;
                this._currentSelection = currentSelection;
                this._onSelectionChanged = onSelectionChanged;
            }

            protected override T DoEdit(Rect rect, T obj, TContext context, fiGraphMetadata metadata)
            {
                fiGUIContent currentValue1 = this._label.GetCurrentValue(obj, context);
                int currentValue2 = this._currentSelection.GetCurrentValue(obj, context);
                GUIContent[] currentValue3 = this._options.GetCurrentValue(obj, context);
                int selected = fiLateBindings.EditorGUI.Popup(rect, currentValue1.AsGUIContent, currentValue2, currentValue3);
                if (currentValue2 != selected)
                    obj = this._onSelectionChanged(obj, context, selected);
                return obj;
            }

            protected override float DoGetHeight(T obj, TContext context, fiGraphMetadata metadata)
            {
                return fiLateBindings.EditorGUIUtility.singleLineHeight;
            }

            public delegate T OnSelectionChanged(T obj, TContext context, int selected);
        }

        public class PropertyEditor : tkControl<T, TContext>
        {
            private MemberInfo _attributes;
            private Func<T, TContext, object> _getValue;
            private Action<T, TContext, object> _setValue;
            private tk<T, TContext>.Value<fiGUIContent> _label;
            private System.Type _fieldType;
            private string _errorMessage;

            public PropertyEditor(string memberName) => this.InitializeFromMemberName(memberName);

            public PropertyEditor(fiGUIContent label, string memberName)
                : this(memberName)
            {
                this._label = (tk<T, TContext>.Value<fiGUIContent>) label;
            }

            public PropertyEditor(tk<T, TContext>.Value<fiGUIContent> label, string memberName)
                : this(memberName)
            {
                this._label = label;
            }

            public PropertyEditor(
                fiGUIContent label,
                System.Type fieldType,
                MemberInfo attributes,
                Func<T, TContext, object> getValue,
                Action<T, TContext, object> setValue)
            {
                this._label = (tk<T, TContext>.Value<fiGUIContent>) label;
                this._fieldType = fieldType;
                this._attributes = attributes;
                this._getValue = getValue;
                this._setValue = setValue;
            }

            private void InitializeFromMemberName(string memberName)
            {
                InspectedProperty property = InspectedType.Get(typeof (T)).GetPropertyByName(memberName);
                if (property == null)
                {
                    this._errorMessage = $"Unable to locate member `{memberName}` on type `{typeof (T).CSharpName()}`";
                    this._fieldType = typeof (T);
                    this._attributes = (MemberInfo) null;
                    this._getValue = (Func<T, TContext, object>) ((o, c) => (object) default (T));
                    this._setValue = (Action<T, TContext, object>) ((o, c, v) => { });
                    this._label = (tk<T, TContext>.Value<fiGUIContent>) new fiGUIContent(memberName + " (unable to locate)");
                }
                else
                {
                    this._fieldType = property.StorageType;
                    this._attributes = property.MemberInfo;
                    this._getValue = (Func<T, TContext, object>) ((o, c) => property.Read((object) o));
                    this._setValue = (Action<T, TContext, object>) ((o, c, v) => property.Write((object) o, v));
                    this._label = (tk<T, TContext>.Value<fiGUIContent>) new fiGUIContent(property.DisplayName);
                }
            }

            public static tk<T, TContext>.PropertyEditor Create<TEdited>(
                fiGUIContent label,
                MemberInfo attributes,
                Func<T, TContext, TEdited> getValue,
                Action<T, TContext, TEdited> setValue)
            {
                return new tk<T, TContext>.PropertyEditor(label, typeof (TEdited), attributes, (Func<T, TContext, object>) ((o, c) => (object) getValue(o, c)), (Action<T, TContext, object>) ((o, c, v) => setValue(o, c, (TEdited) v)));
            }

            public static tk<T, TContext>.PropertyEditor Create<TEdited>(
                fiGUIContent label,
                Func<T, TContext, TEdited> getValue)
            {
                return new tk<T, TContext>.PropertyEditor(label, typeof (TEdited), (MemberInfo) null, (Func<T, TContext, object>) ((o, c) => (object) getValue(o, c)), (Action<T, TContext, object>) null);
            }

            public static tk<T, TContext>.PropertyEditor Create<TEdited>(
                fiGUIContent label,
                Func<T, TContext, TEdited> getValue,
                Action<T, TContext, TEdited> setValue)
            {
                return new tk<T, TContext>.PropertyEditor(label, typeof (TEdited), (MemberInfo) null, (Func<T, TContext, object>) ((o, c) => (object) getValue(o, c)), (Action<T, TContext, object>) ((o, c, v) => setValue(o, c, (TEdited) v)));
            }

            protected override T DoEdit(Rect rect, T obj, TContext context, fiGraphMetadata metadata)
            {
                if (this._errorMessage != null)
                {
                    fiLateBindings.EditorGUI.HelpBox(rect, this._errorMessage, CommentType.Error);
                    return obj;
                }
                fiLateBindings.EditorGUI.BeginChangeCheck();
                fiLateBindings.EditorGUI.BeginDisabledGroup(this._setValue == null);
                object obj1 = this._getValue(obj, context);
                fiGraphMetadataChild metadata1 = new fiGraphMetadataChild()
                {
                    Metadata = this.GetInstanceMetadata(metadata)
                };
                fiGUIContent currentValue = this._label.GetCurrentValue(obj, context);
                object obj2 = fiLateBindings.PropertyEditor.Edit(this._fieldType, this._attributes, rect, (GUIContent) currentValue, obj1, metadata1);
                fiLateBindings.EditorGUI.EndDisabledGroup();
                if (fiLateBindings.EditorGUI.EndChangeCheck() && this._setValue != null)
                    this._setValue(obj, context, obj2);
                return obj;
            }

            protected override float DoGetHeight(T obj, TContext context, fiGraphMetadata metadata)
            {
                if (this._errorMessage != null)
                    return (float) fiCommentUtility.GetCommentHeight(this._errorMessage, CommentType.Error);
                object obj1 = this._getValue(obj, context);
                fiGraphMetadataChild metadata1 = new fiGraphMetadataChild()
                {
                    Metadata = this.GetInstanceMetadata(metadata)
                };
                return fiLateBindings.PropertyEditor.GetElementHeight(this._fieldType, this._attributes, (GUIContent) this._label.GetCurrentValue(obj, context), obj1, metadata1);
            }
        }

        public class StyleProxy : tkControl<T, TContext>
        {
            public tkControl<T, TContext> Control;

            public StyleProxy()
            {
            }

            public StyleProxy(tkControl<T, TContext> control) => this.Control = control;

            protected override T DoEdit(Rect rect, T obj, TContext context, fiGraphMetadata metadata)
            {
                return this.Control.Edit(rect, obj, context, metadata);
            }

            protected override float DoGetHeight(T obj, TContext context, fiGraphMetadata metadata)
            {
                return this.Control.GetHeight(obj, context, metadata);
            }
        }

        public class ReadOnly : tk<T, TContext>.ReadOnlyIf
        {
            public ReadOnly()
                : base(tk<T, TContext>.Val<bool>((tk<T, TContext>.Value<bool>.GeneratorNoContext) (o => true)))
            {
            }
        }

        public class ReadOnlyIf : tk<T, TContext>.ConditionalStyle
        {
            public ReadOnlyIf(tk<T, TContext>.Value<bool> isReadOnly)
                : base(new Func<T, TContext, bool>(isReadOnly.GetCurrentValue), (Func<T, TContext, object>) ((obj, context) =>
            {
                fiLateBindings.EditorGUI.BeginDisabledGroup(true);
                return (object) null;
            }), (Action<T, TContext, object>) ((obj, context, state) => fiLateBindings.EditorGUI.EndDisabledGroup()))
            {
            }

            public ReadOnlyIf(tk<T, TContext>.Value<bool>.Generator isReadOnly)
                : this(tk<T, TContext>.Val<bool>(isReadOnly))
            {
            }

            public ReadOnlyIf(
                tk<T, TContext>.Value<bool>.GeneratorNoContext isReadOnly)
                : this(tk<T, TContext>.Val<bool>(isReadOnly))
            {
            }
        }

        public class ShowIf : tkControl<T, TContext>
        {
            private readonly tk<T, TContext>.Value<bool> _shouldDisplay;
            [ShowInInspector]
            private readonly tkControl<T, TContext> _control;

            public ShowIf(tk<T, TContext>.Value<bool> shouldDisplay, tkControl<T, TContext> control)
            {
                this._shouldDisplay = shouldDisplay;
                this._control = control;
            }

            public ShowIf(
                tk<T, TContext>.Value<bool>.Generator shouldDisplay,
                tkControl<T, TContext> control)
                : this(new tk<T, TContext>.Value<bool>(shouldDisplay), control)
            {
            }

            public ShowIf(
                tk<T, TContext>.Value<bool>.GeneratorNoContext shouldDisplay,
                tkControl<T, TContext> control)
                : this(new tk<T, TContext>.Value<bool>(shouldDisplay), control)
            {
            }

            protected override T DoEdit(Rect rect, T obj, TContext context, fiGraphMetadata metadata)
            {
                return this._control.Edit(rect, obj, context, metadata);
            }

            protected override float DoGetHeight(T obj, TContext context, fiGraphMetadata metadata)
            {
                return this._control.GetHeight(obj, context, metadata);
            }

            public override bool ShouldShow(T obj, TContext context, fiGraphMetadata metadata)
            {
                return this._shouldDisplay.GetCurrentValue(obj, context);
            }
        }

        public class Slider : tkControl<T, TContext>
        {
            private readonly tk<T, TContext>.Value<float> _min;
            private readonly tk<T, TContext>.Value<float> _max;
            private readonly Func<T, TContext, float> _getValue;
            private readonly Action<T, TContext, float> _setValue;
            private readonly tk<T, TContext>.Value<fiGUIContent> _label;

            public Slider(
                tk<T, TContext>.Value<float> min,
                tk<T, TContext>.Value<float> max,
                Func<T, TContext, float> getValue,
                Action<T, TContext, float> setValue)
                : this((tk<T, TContext>.Value<fiGUIContent>) fiGUIContent.Empty, min, max, getValue, setValue)
            {
            }

            public Slider(
                tk<T, TContext>.Value<fiGUIContent> label,
                tk<T, TContext>.Value<float> min,
                tk<T, TContext>.Value<float> max,
                Func<T, TContext, float> getValue,
                Action<T, TContext, float> setValue)
            {
                this._label = label;
                this._min = min;
                this._max = max;
                this._getValue = getValue;
                this._setValue = setValue;
            }

            protected override T DoEdit(Rect rect, T obj, TContext context, fiGraphMetadata metadata)
            {
                float num1 = this._getValue(obj, context);
                float currentValue1 = this._min.GetCurrentValue(obj, context);
                float currentValue2 = this._max.GetCurrentValue(obj, context);
                fiLateBindings.EditorGUI.BeginChangeCheck();
                float num2 = fiLateBindings.EditorGUI.Slider(rect, (GUIContent) this._label.GetCurrentValue(obj, context), num1, currentValue1, currentValue2);
                if (fiLateBindings.EditorGUI.EndChangeCheck())
                    this._setValue(obj, context, num2);
                return obj;
            }

            protected override float DoGetHeight(T obj, TContext context, fiGraphMetadata metadata)
            {
                return fiLateBindings.EditorGUIUtility.singleLineHeight;
            }
        }

        public class VerticalGroup : tkControl<T, TContext>, IEnumerable
        {
            [ShowInInspector]
            private readonly List<tk<T, TContext>.VerticalGroup.SectionItem> _items = new List<tk<T, TContext>.VerticalGroup.SectionItem>();
            private readonly float _marginBetweenItems;

            public VerticalGroup()
                : this(fiLateBindings.EditorGUIUtility.standardVerticalSpacing)
            {
            }

            public VerticalGroup(float marginBetweenItems) => this._marginBetweenItems = marginBetweenItems;

            protected override IEnumerable<tkIControl> NonMemberChildControls
            {
                get
                {
                    tk_T_TContext__VerticalGroup___c__Iterator0 memberChildControls = new tk_T_TContext__VerticalGroup___c__Iterator0()
                    {
                        _this = this
                    };
                    memberChildControls._PC = -2;
                    return (IEnumerable<tkIControl>) memberChildControls;
                }
            }

            public void Add(tkControl<T, TContext> rule) => this.InternalAdd(rule);

            private void InternalAdd(tkControl<T, TContext> rule)
            {
                this._items.Add(new tk<T, TContext>.VerticalGroup.SectionItem()
                {
                    Rule = rule
                });
            }

            protected override T DoEdit(Rect rect, T obj, TContext context, fiGraphMetadata metadata)
            {
                for (int index = 0; index < this._items.Count; ++index)
                {
                    tk<T, TContext>.VerticalGroup.SectionItem sectionItem = this._items[index];
                    if (sectionItem.Rule.ShouldShow(obj, context, metadata))
                    {
                        float height = sectionItem.Rule.GetHeight(obj, context, metadata);
                        Rect rect1 = rect;
            rect1.height = height;
                        obj = sectionItem.Rule.Edit(rect1, obj, context, metadata);
                        rect.y += height;
                        rect.y += this._marginBetweenItems;
                    }
                }
                return obj;
            }

            protected override float DoGetHeight(T obj, TContext context, fiGraphMetadata metadata)
            {
                float height = 0.0f;
                for (int index = 0; index < this._items.Count; ++index)
                {
                    tk<T, TContext>.VerticalGroup.SectionItem sectionItem = this._items[index];
                    if (sectionItem.Rule.ShouldShow(obj, context, metadata))
                    {
                        height += sectionItem.Rule.GetHeight(obj, context, metadata);
                        if (index != this._items.Count - 1)
                            height += this._marginBetweenItems;
                    }
                }
                return height;
            }

            IEnumerator IEnumerable.GetEnumerator() => throw new NotSupportedException();

            private struct SectionItem
            {
                public tkControl<T, TContext> Rule;
            }
        }

        public struct Value<TValue>
        {
            private tk<T, TContext>.Value<TValue>.Generator _generator;
            private TValue _direct;

            public Value(tk<T, TContext>.Value<TValue>.Generator generator)
            {
                this._generator = generator;
                this._direct = default (TValue);
            }

            public Value(
                tk<T, TContext>.Value<TValue>.GeneratorNoContext generator)
            {
                this._generator = (tk<T, TContext>.Value<TValue>.Generator) ((o, context) => generator(o));
                this._direct = default (TValue);
            }

            public TValue GetCurrentValue(T instance, TContext context)
            {
                return this._generator == null ? this._direct : this._generator(instance, context);
            }

            public static implicit operator tk<T, TContext>.Value<TValue>(TValue direct)
            {
                return new tk<T, TContext>.Value<TValue>()
                {
                    _generator = (tk<T, TContext>.Value<TValue>.Generator) null,
                    _direct = direct
                };
            }

            public static implicit operator tk<T, TContext>.Value<TValue>(
                tk<T, TContext>.Value<TValue>.Generator generator)
            {
                return new tk<T, TContext>.Value<TValue>()
                {
                    _generator = generator,
                    _direct = default (TValue)
                };
            }

            public static implicit operator tk<T, TContext>.Value<TValue>(
                tk<T, TContext>.Value<TValue>.GeneratorNoContext generator)
            {
                return new tk<T, TContext>.Value<TValue>()
                {
                    _generator = (tk<T, TContext>.Value<TValue>.Generator) ((obj, context) => generator(obj)),
                    _direct = default (TValue)
                };
            }

            public static implicit operator tk<T, TContext>.Value<TValue>(Func<T, int, TValue> generator)
            {
                return new tk<T, TContext>.Value<TValue>();
            }

            public static implicit operator tk<T, TContext>.Value<TValue>(Func<T, TValue> generator)
            {
                return new tk<T, TContext>.Value<TValue>()
                {
                    _generator = (tk<T, TContext>.Value<TValue>.Generator) ((obj, context) => generator(obj)),
                    _direct = default (TValue)
                };
            }

            public delegate TValue Generator(T input, TContext context);

            public delegate TValue GeneratorNoContext(T input);
        }
    }
}
