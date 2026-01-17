// Decompiled with JetBrains decompiler
// Type: FullInspector.LayoutToolkit.fiCenterVertical
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace FullInspector.LayoutToolkit
{
  public class fiCenterVertical : fiLayout
  {
    private string _id;
    private fiLayout _centered;

    public fiCenterVertical(string id, fiLayout centered)
    {
      this._id = id;
      this._centered = centered;
    }

    public fiCenterVertical(fiLayout centered)
      : this(string.Empty, centered)
    {
    }

    public override bool RespondsTo(string sectionId)
    {
      return this._id == sectionId || this._centered.RespondsTo(sectionId);
    }

    public override Rect GetSectionRect(string sectionId, Rect initial)
    {
      float num = initial.height - this._centered.Height;
      initial.y += num / 2f;
      initial.height -= num;
      initial = this._centered.GetSectionRect(sectionId, initial);
      return initial;
    }

    public override float Height => this._centered.Height;
  }
}
