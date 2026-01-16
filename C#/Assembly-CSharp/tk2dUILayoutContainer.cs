// Decompiled with JetBrains decompiler
// Type: tk2dUILayoutContainer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public abstract class tk2dUILayoutContainer : tk2dUILayout
{
  protected Vector2 innerSize = Vector2.zero;

  public Vector2 GetInnerSize() => this.innerSize;

  protected abstract void DoChildLayout();

  public event System.Action OnChangeContent;

  public override void Reshape(Vector3 dMin, Vector3 dMax, bool updateChildren)
  {
    tk2dUILayoutContainer uiLayoutContainer1 = this;
    uiLayoutContainer1.bMin = uiLayoutContainer1.bMin + dMin;
    tk2dUILayoutContainer uiLayoutContainer2 = this;
    uiLayoutContainer2.bMax = uiLayoutContainer2.bMax + dMax;
    Vector3 vector3 = new Vector3(this.bMin.x, this.bMax.y);
    this.transform.position += vector3;
    tk2dUILayoutContainer uiLayoutContainer3 = this;
    uiLayoutContainer3.bMin = uiLayoutContainer3.bMin - vector3;
    tk2dUILayoutContainer uiLayoutContainer4 = this;
    uiLayoutContainer4.bMax = uiLayoutContainer4.bMax - vector3;
    this.DoChildLayout();
    if (this.OnChangeContent == null)
      return;
    this.OnChangeContent();
  }

  public void AddLayout(tk2dUILayout layout, tk2dUILayoutItem item)
  {
    item.gameObj = layout.gameObject;
    item.layout = layout;
    this.layoutItems.Add(item);
    layout.gameObject.transform.parent = this.transform;
    this.Refresh();
  }

  public void AddLayoutAtIndex(tk2dUILayout layout, tk2dUILayoutItem item, int index)
  {
    item.gameObj = layout.gameObject;
    item.layout = layout;
    this.layoutItems.Insert(index, item);
    layout.gameObject.transform.parent = this.transform;
    this.Refresh();
  }

  public void RemoveLayout(tk2dUILayout layout)
  {
    foreach (tk2dUILayoutItem layoutItem in this.layoutItems)
    {
      if ((UnityEngine.Object) layoutItem.layout == (UnityEngine.Object) layout)
      {
        this.layoutItems.Remove(layoutItem);
        layout.gameObject.transform.parent = (Transform) null;
        break;
      }
    }
    this.Refresh();
  }
}
