// Decompiled with JetBrains decompiler
// Type: dfTriangleClippingRegion
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
internal class dfTriangleClippingRegion : IDisposable
{
  private static Queue<dfTriangleClippingRegion> pool = new Queue<dfTriangleClippingRegion>();
  private static dfList<Plane> intersectedPlanes = new dfList<Plane>(32 /*0x20*/);
  private dfList<Plane> planes;

  private dfTriangleClippingRegion() => this.planes = new dfList<Plane>();

  public static dfTriangleClippingRegion Obtain()
  {
    return dfTriangleClippingRegion.pool.Count > 0 ? dfTriangleClippingRegion.pool.Dequeue() : new dfTriangleClippingRegion();
  }

  public static dfTriangleClippingRegion Obtain(dfTriangleClippingRegion parent, dfControl control)
  {
    dfTriangleClippingRegion triangleClippingRegion = dfTriangleClippingRegion.pool.Count <= 0 ? new dfTriangleClippingRegion() : dfTriangleClippingRegion.pool.Dequeue();
    triangleClippingRegion.planes.AddRange(control.GetClippingPlanes());
    if (parent != null)
      triangleClippingRegion.planes.AddRange(parent.planes);
    return triangleClippingRegion;
  }

  public void Release()
  {
    this.planes.Clear();
    if (dfTriangleClippingRegion.pool.Contains(this))
      return;
    dfTriangleClippingRegion.pool.Enqueue(this);
  }

  public bool PerformClipping(
    dfRenderData dest,
    ref Bounds bounds,
    uint checksum,
    dfRenderData controlData)
  {
    if (this.planes == null || this.planes.Count == 0)
    {
      dest.Merge(controlData);
      return true;
    }
    if ((int) controlData.Checksum == (int) checksum)
    {
      if (controlData.Intersection == dfIntersectionType.Inside)
      {
        dest.Merge(controlData);
        return true;
      }
      if (controlData.Intersection == dfIntersectionType.None)
        return false;
    }
    bool flag = false;
    dfIntersectionType type;
    dfList<Plane> planes = this.TestIntersection(bounds, out type);
    switch (type)
    {
      case dfIntersectionType.Inside:
        dest.Merge(controlData);
        flag = true;
        break;
      case dfIntersectionType.Intersecting:
        this.clipToPlanes(planes, controlData, dest, checksum);
        flag = true;
        break;
    }
    controlData.Checksum = checksum;
    controlData.Intersection = type;
    return flag;
  }

  public dfList<Plane> TestIntersection(Bounds bounds, out dfIntersectionType type)
  {
    if (this.planes == null || this.planes.Count == 0)
    {
      type = dfIntersectionType.Inside;
      return (dfList<Plane>) null;
    }
    dfTriangleClippingRegion.intersectedPlanes.Clear();
    Vector3 center = bounds.center;
    Vector3 extents = bounds.extents;
    bool flag = false;
    int count = this.planes.Count;
    Plane[] items = this.planes.Items;
    for (int index = 0; index < count; ++index)
    {
      Plane plane = items[index];
      Vector3 normal = plane.normal;
      float distance = plane.distance;
      float num = (float) ((double) extents.x * (double) Mathf.Abs(normal.x) + (double) extents.y * (double) Mathf.Abs(normal.y) + (double) extents.z * (double) Mathf.Abs(normal.z));
      float f = Vector3.Dot(normal, center) + distance;
      if ((double) Mathf.Abs(f) <= (double) num)
      {
        flag = true;
        dfTriangleClippingRegion.intersectedPlanes.Add(plane);
      }
      else if ((double) f < -(double) num)
      {
        type = dfIntersectionType.None;
        return (dfList<Plane>) null;
      }
    }
    if (flag)
    {
      type = dfIntersectionType.Intersecting;
      return dfTriangleClippingRegion.intersectedPlanes;
    }
    type = dfIntersectionType.Inside;
    return (dfList<Plane>) null;
  }

  public void clipToPlanes(
    dfList<Plane> planes,
    dfRenderData data,
    dfRenderData dest,
    uint controlChecksum)
  {
    if (data == null || data.Vertices.Count == 0)
      return;
    if (planes == null || planes.Count == 0)
      dest.Merge(data);
    else
      dfClippingUtil.Clip((IList<Plane>) planes, data, dest);
  }

  public void Dispose() => this.Release();
}
