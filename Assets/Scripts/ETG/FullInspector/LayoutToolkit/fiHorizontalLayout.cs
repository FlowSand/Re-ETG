// Decompiled with JetBrains decompiler
// Type: FullInspector.LayoutToolkit.fiHorizontalLayout
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace FullInspector.LayoutToolkit
{
  public class fiHorizontalLayout : fiLayout, IEnumerable
  {
    private List<fiHorizontalLayout.SectionItem> _items = new List<fiHorizontalLayout.SectionItem>();
    private fiLayout _defaultRule = (fiLayout) new fiVerticalLayout();

    public fiHorizontalLayout()
    {
    }

    public fiHorizontalLayout(fiLayout defaultRule) => this._defaultRule = defaultRule;

    public void Add(fiLayout rule) => this.ActualAdd(string.Empty, 0.0f, fiExpandMode.Expand, rule);

    public void Add(float width)
    {
      this.ActualAdd(string.Empty, width, fiExpandMode.Fixed, this._defaultRule);
    }

    public void Add(string id) => this.ActualAdd(id, 0.0f, fiExpandMode.Expand, this._defaultRule);

    public void Add(string id, float width)
    {
      this.ActualAdd(id, width, fiExpandMode.Fixed, this._defaultRule);
    }

    public void Add(string id, fiLayout rule) => this.ActualAdd(id, 0.0f, fiExpandMode.Expand, rule);

    public void Add(float width, fiLayout rule)
    {
      this.ActualAdd(string.Empty, width, fiExpandMode.Fixed, rule);
    }

    public void Add(string id, float width, fiLayout rule)
    {
      this.ActualAdd(id, width, fiExpandMode.Fixed, rule);
    }

    private void ActualAdd(string id, float width, fiExpandMode expandMode, fiLayout rule)
    {
      this._items.Add(new fiHorizontalLayout.SectionItem()
      {
        Id = id,
        MinWidth = width,
        ExpandMode = expandMode,
        Rule = rule
      });
    }

    private int ExpandCount
    {
      get
      {
        int expandCount = 0;
        for (int index = 0; index < this._items.Count; ++index)
        {
          if (this._items[index].ExpandMode == fiExpandMode.Expand)
            ++expandCount;
        }
        if (expandCount == 0)
          expandCount = 1;
        return expandCount;
      }
    }

    private float MinimumWidth
    {
      get
      {
        float minimumWidth = 0.0f;
        for (int index = 0; index < this._items.Count; ++index)
          minimumWidth += this._items[index].MinWidth;
        return minimumWidth;
      }
    }

    public override Rect GetSectionRect(string sectionId, Rect initial)
    {
      float num1 = initial.width - this.MinimumWidth;
      if ((double) num1 < 0.0)
        num1 = 0.0f;
      float num2 = 1f / (float) this.ExpandCount;
      for (int index = 0; index < this._items.Count; ++index)
      {
        fiHorizontalLayout.SectionItem sectionItem = this._items[index];
        float minWidth = sectionItem.MinWidth;
        if (sectionItem.ExpandMode == fiExpandMode.Expand)
          minWidth += num1 * num2;
        if (sectionItem.Id == sectionId || sectionItem.Rule.RespondsTo(sectionId))
        {
          initial.width = minWidth;
          initial = sectionItem.Rule.GetSectionRect(sectionId, initial);
          break;
        }
        initial.x += minWidth;
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
        float val1 = 0.0f;
        for (int index = 0; index < this._items.Count; ++index)
          val1 = Math.Max(val1, this._items[index].Rule.Height);
        return val1;
      }
    }

    IEnumerator IEnumerable.GetEnumerator() => throw new NotSupportedException();

    private struct SectionItem
    {
      public string Id;
      public float MinWidth;
      public fiExpandMode ExpandMode;
      public fiLayout Rule;
    }
  }
}
