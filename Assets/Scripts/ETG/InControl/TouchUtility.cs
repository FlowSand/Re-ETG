// Decompiled with JetBrains decompiler
// Type: InControl.TouchUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace InControl
{
  public static class TouchUtility
  {
    public static Vector2 AnchorToViewPoint(TouchControlAnchor touchControlAnchor)
    {
      switch (touchControlAnchor)
      {
        case TouchControlAnchor.TopLeft:
          return new Vector2(0.0f, 1f);
        case TouchControlAnchor.CenterLeft:
          return new Vector2(0.0f, 0.5f);
        case TouchControlAnchor.BottomLeft:
          return new Vector2(0.0f, 0.0f);
        case TouchControlAnchor.TopCenter:
          return new Vector2(0.5f, 1f);
        case TouchControlAnchor.Center:
          return new Vector2(0.5f, 0.5f);
        case TouchControlAnchor.BottomCenter:
          return new Vector2(0.5f, 0.0f);
        case TouchControlAnchor.TopRight:
          return new Vector2(1f, 1f);
        case TouchControlAnchor.CenterRight:
          return new Vector2(1f, 0.5f);
        case TouchControlAnchor.BottomRight:
          return new Vector2(1f, 0.0f);
        default:
          return Vector2.zero;
      }
    }

    public static Vector2 RoundVector(Vector2 vector)
    {
      return new Vector2(Mathf.Round(vector.x), Mathf.Round(vector.y));
    }
  }
}
