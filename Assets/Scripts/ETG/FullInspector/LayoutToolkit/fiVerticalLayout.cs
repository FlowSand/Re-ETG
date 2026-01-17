// Decompiled with JetBrains decompiler
// Type: FullInspector.LayoutToolkit.fiVerticalLayout
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace FullInspector.LayoutToolkit;

public class fiVerticalLayout : fiLayout, IEnumerable
{
  private List<fiVerticalLayout.SectionItem> _items = new List<fiVerticalLayout.SectionItem>();

  public void Add(fiLayout rule) => this.Add(string.Empty, rule);

  public void Add(string sectionId, fiLayout rule)
  {
    this._items.Add(new fiVerticalLayout.SectionItem()
    {
      Id = sectionId,
      Rule = rule
    });
  }

  public void Add(string sectionId, float height)
  {
    this.Add(sectionId, (fiLayout) new fiLayoutHeight(sectionId, height));
  }

  public void Add(float height) => this.Add(string.Empty, height);

  public override Rect GetSectionRect(string sectionId, Rect initial)
  {
    for (int index = 0; index < this._items.Count; ++index)
    {
      fiVerticalLayout.SectionItem sectionItem = this._items[index];
      if (sectionItem.Id == sectionId || sectionItem.Rule.RespondsTo(sectionId))
      {
        if (sectionItem.Rule.RespondsTo(sectionId))
        {
          initial = sectionItem.Rule.GetSectionRect(sectionId, initial);
          break;
        }
        initial.height = sectionItem.Rule.Height;
        break;
      }
      initial.y += sectionItem.Rule.Height;
    }
    return initial;
  }

  public override bool RespondsTo(string sectionId)
  {
    for (int index = 0; index < this._items.Count; ++index)
    {
      if (this._items[index].Id == sectionId || this._items[index].Rule.RespondsTo(sectionId))
        return true;
    }
    return false;
  }

  public override float Height
  {
    get
    {
      float height = 0.0f;
      for (int index = 0; index < this._items.Count; ++index)
        height += this._items[index].Rule.Height;
      return height;
    }
  }

  IEnumerator IEnumerable.GetEnumerator() => throw new NotSupportedException();

  private struct SectionItem
  {
    public string Id;
    public fiLayout Rule;
  }
}
