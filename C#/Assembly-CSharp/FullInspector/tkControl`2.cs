// Decompiled with JetBrains decompiler
// Type: FullInspector.tkControl`2
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullSerializer.Internal;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

#nullable disable
namespace FullInspector;

public abstract class tkControl<T, TContext> : tkIControl
{
  private int _uniqueId;
  private List<tkStyle<T, TContext>> _styles;

  protected fiGraphMetadata GetInstanceMetadata(fiGraphMetadata metadata)
  {
    return metadata.Enter(this._uniqueId).Metadata;
  }

  public System.Type ContextType => typeof (TContext);

  protected abstract T DoEdit(Rect rect, T obj, TContext context, fiGraphMetadata metadata);

  protected abstract float DoGetHeight(T obj, TContext context, fiGraphMetadata metadata);

  public virtual bool ShouldShow(T obj, TContext context, fiGraphMetadata metadata) => true;

  public tkStyle<T, TContext> Style
  {
    set
    {
      this.Styles = new List<tkStyle<T, TContext>>()
      {
        value
      };
    }
  }

  public List<tkStyle<T, TContext>> Styles
  {
    get
    {
      if (this._styles == null)
        this._styles = new List<tkStyle<T, TContext>>();
      return this._styles;
    }
    set => this._styles = value;
  }

  public T Edit(Rect rect, T obj, TContext context, fiGraphMetadata metadata)
  {
    if (this.Styles == null)
      return this.DoEdit(rect, obj, context, metadata);
    for (int index = 0; index < this.Styles.Count; ++index)
      this.Styles[index].Activate(obj, context);
    T obj1 = this.DoEdit(rect, obj, context, metadata);
    for (int index = 0; index < this.Styles.Count; ++index)
      this.Styles[index].Deactivate(obj, context);
    return obj1;
  }

  public object Edit(Rect rect, object obj, object context, fiGraphMetadata metadata)
  {
    return (object) this.Edit(rect, (T) obj, (TContext) context, metadata);
  }

  public float GetHeight(T obj, TContext context, fiGraphMetadata metadata)
  {
    if (this.Styles == null)
      return this.DoGetHeight(obj, context, metadata);
    for (int index = 0; index < this.Styles.Count; ++index)
      this.Styles[index].Activate(obj, context);
    float height = this.DoGetHeight(obj, context, metadata);
    for (int index = 0; index < this.Styles.Count; ++index)
      this.Styles[index].Deactivate(obj, context);
    return height;
  }

  public float GetHeight(object obj, object context, fiGraphMetadata metadata)
  {
    return this.GetHeight((T) obj, (TContext) context, metadata);
  }

  void tkIControl.InitializeId(ref int nextId)
  {
    int num;
    nextId = (num = nextId) + 1;
    this._uniqueId = num;
    foreach (tkIControl memberChildControl in this.NonMemberChildControls)
      memberChildControl.InitializeId(ref nextId);
    for (System.Type type = this.GetType(); type != null; type = type.Resolve().BaseType)
    {
      foreach (MemberInfo declaredMember in type.GetDeclaredMembers())
      {
        System.Type memberType;
        if (tkControl<T, TContext>.TryGetMemberType(declaredMember, out memberType))
        {
          if (typeof (tkIControl).IsAssignableFrom(memberType))
          {
            tkIControl tkIcontrol;
            if (tkControl<T, TContext>.TryReadValue<tkIControl>(declaredMember, (object) this, out tkIcontrol) && tkIcontrol != null)
              tkIcontrol.InitializeId(ref nextId);
          }
          else
          {
            IEnumerable<tkIControl> tkIcontrols;
            if (typeof (IEnumerable<tkIControl>).IsAssignableFrom(memberType) && tkControl<T, TContext>.TryReadValue<IEnumerable<tkIControl>>(declaredMember, (object) this, out tkIcontrols) && tkIcontrols != null)
            {
              foreach (tkIControl tkIcontrol in tkIcontrols)
                tkIcontrol.InitializeId(ref nextId);
            }
          }
        }
      }
    }
  }

  protected virtual IEnumerable<tkIControl> NonMemberChildControls
  {
    get
    {
      tkControl<T, TContext>.\u003C\u003Ec__Iterator0 cIterator0 = new tkControl<T, TContext>.\u003C\u003Ec__Iterator0();
      tkControl<T, TContext>.\u003C\u003Ec__Iterator0 memberChildControls = cIterator0;
      memberChildControls.\u0024PC = -2;
      return (IEnumerable<tkIControl>) memberChildControls;
    }
  }

  private static bool TryReadValue<TValue>(MemberInfo member, object context, out TValue value)
  {
    switch (member)
    {
      case FieldInfo _:
        value = (TValue) ((FieldInfo) member).GetValue(context);
        return true;
      case PropertyInfo _:
        value = (TValue) ((PropertyInfo) member).GetValue(context, (object[]) null);
        return true;
      default:
        value = default (TValue);
        return false;
    }
  }

  private static bool TryGetMemberType(MemberInfo member, out System.Type memberType)
  {
    switch (member)
    {
      case FieldInfo _:
        memberType = ((FieldInfo) member).FieldType;
        return true;
      case PropertyInfo _:
        memberType = ((PropertyInfo) member).PropertyType;
        return true;
      default:
        memberType = (System.Type) null;
        return false;
    }
  }
}
