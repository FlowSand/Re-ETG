// Decompiled with JetBrains decompiler
// Type: Dungeonator.FlowActionLine
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Dungeonator
{
  public class FlowActionLine
  {
    public Vector2 point1;
    public Vector2 point2;

    public FlowActionLine(Vector2 p1, Vector2 p2)
    {
      this.point1 = p1;
      this.point2 = p2;
    }

    protected bool OnSegment(Vector2 p, Vector2 q, Vector2 r)
    {
      return (double) q.x <= (double) Mathf.Max(p.x, r.x) && (double) q.x >= (double) Mathf.Min(p.x, r.x) && (double) q.y <= (double) Mathf.Max(p.y, r.y) && (double) q.y >= (double) Mathf.Min(p.y, r.y);
    }

    protected int GetOrientation(Vector2 p, Vector2 q, Vector2 r)
    {
      float num = (float) (((double) q.y - (double) p.y) * ((double) r.x - (double) q.x) - ((double) q.x - (double) p.x) * ((double) r.y - (double) q.y));
      if ((double) num == 0.0)
        return 0;
      return (double) num > 0.0 ? 1 : 2;
    }

    public bool Crosses(FlowActionLine other)
    {
      int orientation1 = this.GetOrientation(this.point1, this.point2, other.point1);
      int orientation2 = this.GetOrientation(this.point1, this.point2, other.point2);
      int orientation3 = this.GetOrientation(other.point1, other.point2, this.point1);
      int orientation4 = this.GetOrientation(other.point1, other.point2, this.point2);
      return orientation1 != orientation2 && orientation3 != orientation4 || orientation1 == 0 && this.OnSegment(this.point1, other.point1, this.point2) || orientation2 == 0 && this.OnSegment(this.point1, other.point2, this.point2) || orientation3 == 0 && this.OnSegment(other.point1, this.point1, other.point2) || orientation4 == 0 && this.OnSegment(other.point1, this.point2, other.point2);
    }
  }
}
