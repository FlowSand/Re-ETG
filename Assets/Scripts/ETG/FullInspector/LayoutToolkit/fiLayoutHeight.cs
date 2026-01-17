// Decompiled with JetBrains decompiler
// Type: FullInspector.LayoutToolkit.fiLayoutHeight
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace FullInspector.LayoutToolkit
{
  public class fiLayoutHeight : fiLayout
  {
    private string _id;
    private float _height;

    public fiLayoutHeight(float height)
    {
      this._id = string.Empty;
      this._height = height;
    }

    public fiLayoutHeight(string sectionId, float height)
    {
      this._id = sectionId;
      this._height = height;
    }

    public override bool RespondsTo(string sectionId) => this._id == sectionId;

    public override Rect GetSectionRect(string sectionId, Rect initial)
    {
      initial.height = this._height;
      return initial;
    }

    public void SetHeight(float height) => this._height = height;

    public override float Height => this._height;
  }
}
