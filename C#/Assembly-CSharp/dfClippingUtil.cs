// Decompiled with JetBrains decompiler
// Type: dfClippingUtil
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
internal class dfClippingUtil
{
  private static int[] inside = new int[3];
  private static dfClippingUtil.ClipTriangle[] clipSource = dfClippingUtil.initClipBuffer(1024 /*0x0400*/);
  private static dfClippingUtil.ClipTriangle[] clipDest = dfClippingUtil.initClipBuffer(1024 /*0x0400*/);

  public static void Clip(IList<Plane> planes, dfRenderData source, dfRenderData dest)
  {
    dest.EnsureCapacity(dest.Vertices.Count + source.Vertices.Count);
    int count1 = source.Triangles.Count;
    Vector3[] items1 = source.Vertices.Items;
    int[] items2 = source.Triangles.Items;
    Vector2[] items3 = source.UV.Items;
    Color32[] items4 = source.Colors.Items;
    Matrix4x4 transform = source.Transform;
    int count2 = planes.Count;
    for (int index1 = 0; index1 < count1; index1 += 3)
    {
      for (int index2 = 0; index2 < 3; ++index2)
      {
        int index3 = items2[index1 + index2];
        dfClippingUtil.clipSource[0].corner[index2] = transform.MultiplyPoint(items1[index3]);
        dfClippingUtil.clipSource[0].uv[index2] = items3[index3];
        dfClippingUtil.clipSource[0].color[index2] = items4[index3];
      }
      int count3 = 1;
      for (int index4 = 0; index4 < count2; ++index4)
      {
        Plane plane = planes[index4];
        count3 = dfClippingUtil.clipToPlane(ref plane, dfClippingUtil.clipSource, dfClippingUtil.clipDest, count3);
        dfClippingUtil.ClipTriangle[] clipSource = dfClippingUtil.clipSource;
        dfClippingUtil.clipSource = dfClippingUtil.clipDest;
        dfClippingUtil.clipDest = clipSource;
      }
      for (int index5 = 0; index5 < count3; ++index5)
        dfClippingUtil.clipSource[index5].CopyTo(dest);
    }
  }

  private static int clipToPlane(
    ref Plane plane,
    dfClippingUtil.ClipTriangle[] source,
    dfClippingUtil.ClipTriangle[] dest,
    int count)
  {
    int destIndex = 0;
    for (int index = 0; index < count; ++index)
      destIndex += dfClippingUtil.clipToPlane(ref plane, ref source[index], dest, destIndex);
    return destIndex;
  }

  private static int inverseClipToPlane(
    ref Plane plane,
    ref dfClippingUtil.ClipTriangle triangle,
    dfClippingUtil.ClipTriangle[] dest,
    int destIndex)
  {
    Vector3[] corner = triangle.corner;
    int num1 = 0;
    int num2 = 0;
    Vector3 lhs = plane.normal * -1f;
    float distance = plane.distance;
    for (int index = 0; index < 3; ++index)
    {
      if ((double) Vector3.Dot(lhs, corner[index]) + (double) distance > 0.0)
        dfClippingUtil.inside[num1++] = index;
      else
        num2 = index;
    }
    switch (num1)
    {
      case 0:
        return 0;
      case 1:
        int index1 = dfClippingUtil.inside[0];
        int index2 = (index1 + 1) % 3;
        int index3 = (index1 + 2) % 3;
        Vector3 origin1 = corner[index1];
        Vector3 vector3_1 = corner[index2];
        Vector3 vector3_2 = corner[index3];
        Vector2 a1 = triangle.uv[index1];
        Vector2 b1 = triangle.uv[index2];
        Vector2 b2 = triangle.uv[index3];
        Color32 a2 = triangle.color[index1];
        Color32 b3 = triangle.color[index2];
        Color32 b4 = triangle.color[index3];
        float enter1 = 0.0f;
        Vector3 vector3_3 = vector3_1 - origin1;
        Ray ray1 = new Ray(origin1, vector3_3.normalized);
        plane.Raycast(ray1, out enter1);
        float t1 = enter1 / vector3_3.magnitude;
        Vector3 point1 = ray1.GetPoint(enter1);
        Vector2 vector2_1 = Vector2.Lerp(a1, b1, t1);
        Color32 color32_1 = Color32.Lerp(a2, b3, t1);
        Vector3 vector3_4 = vector3_2 - origin1;
        ray1 = new Ray(origin1, vector3_4.normalized);
        plane.Raycast(ray1, out enter1);
        float t2 = enter1 / vector3_4.magnitude;
        Vector3 point2 = ray1.GetPoint(enter1);
        Vector2 vector2_2 = Vector2.Lerp(a1, b2, t2);
        Color32 color32_2 = Color32.Lerp(a2, b4, t2);
        dfClippingUtil.ClipTriangle clipTriangle1 = dest[destIndex];
        clipTriangle1.corner[0] = origin1;
        clipTriangle1.corner[1] = point1;
        clipTriangle1.corner[2] = point2;
        clipTriangle1.uv[0] = a1;
        clipTriangle1.uv[1] = vector2_1;
        clipTriangle1.uv[2] = vector2_2;
        clipTriangle1.color[0] = a2;
        clipTriangle1.color[1] = color32_1;
        clipTriangle1.color[2] = color32_2;
        return 1;
      case 3:
        dfClippingUtil.ClipTriangle clipTriangle2 = dest[destIndex];
        Array.Copy((Array) triangle.corner, 0, (Array) clipTriangle2.corner, 0, 3);
        Array.Copy((Array) triangle.uv, 0, (Array) clipTriangle2.uv, 0, 3);
        Array.Copy((Array) triangle.color, 0, (Array) clipTriangle2.color, 0, 3);
        return 1;
      default:
        int index4 = num2;
        int index5 = (index4 + 1) % 3;
        int index6 = (index4 + 2) % 3;
        Vector3 origin2 = corner[index4];
        Vector3 vector3_5 = corner[index5];
        Vector3 vector3_6 = corner[index6];
        Vector2 a3 = triangle.uv[index4];
        Vector2 b5 = triangle.uv[index5];
        Vector2 b6 = triangle.uv[index6];
        Color32 a4 = triangle.color[index4];
        Color32 b7 = triangle.color[index5];
        Color32 b8 = triangle.color[index6];
        Vector3 vector3_7 = vector3_5 - origin2;
        Ray ray2 = new Ray(origin2, vector3_7.normalized);
        float enter2 = 0.0f;
        plane.Raycast(ray2, out enter2);
        float t3 = enter2 / vector3_7.magnitude;
        Vector3 point3 = ray2.GetPoint(enter2);
        Vector2 vector2_3 = Vector2.Lerp(a3, b5, t3);
        Color32 color32_3 = Color32.Lerp(a4, b7, t3);
        Vector3 vector3_8 = vector3_6 - origin2;
        ray2 = new Ray(origin2, vector3_8.normalized);
        plane.Raycast(ray2, out enter2);
        float t4 = enter2 / vector3_8.magnitude;
        Vector3 point4 = ray2.GetPoint(enter2);
        Vector2 vector2_4 = Vector2.Lerp(a3, b6, t4);
        Color32 color32_4 = Color32.Lerp(a4, b8, t4);
        dfClippingUtil.ClipTriangle clipTriangle3 = dest[destIndex];
        clipTriangle3.corner[0] = point3;
        clipTriangle3.corner[1] = vector3_5;
        clipTriangle3.corner[2] = point4;
        clipTriangle3.uv[0] = vector2_3;
        clipTriangle3.uv[1] = b5;
        clipTriangle3.uv[2] = vector2_4;
        clipTriangle3.color[0] = color32_3;
        clipTriangle3.color[1] = b7;
        clipTriangle3.color[2] = color32_4;
        clipTriangle3 = dest[++destIndex];
        clipTriangle3.corner[0] = point4;
        clipTriangle3.corner[1] = vector3_5;
        clipTriangle3.corner[2] = vector3_6;
        clipTriangle3.uv[0] = vector2_4;
        clipTriangle3.uv[1] = b5;
        clipTriangle3.uv[2] = b6;
        clipTriangle3.color[0] = color32_4;
        clipTriangle3.color[1] = b7;
        clipTriangle3.color[2] = b8;
        return 2;
    }
  }

  private static int clipToPlane(
    ref Plane plane,
    ref dfClippingUtil.ClipTriangle triangle,
    dfClippingUtil.ClipTriangle[] dest,
    int destIndex)
  {
    Vector3[] corner = triangle.corner;
    int num1 = 0;
    int num2 = 0;
    Vector3 normal = plane.normal;
    float distance = plane.distance;
    for (int index = 0; index < 3; ++index)
    {
      if ((double) Vector3.Dot(normal, corner[index]) + (double) distance > 0.0)
        dfClippingUtil.inside[num1++] = index;
      else
        num2 = index;
    }
    switch (num1)
    {
      case 0:
        return 0;
      case 1:
        int index1 = dfClippingUtil.inside[0];
        int index2 = (index1 + 1) % 3;
        int index3 = (index1 + 2) % 3;
        Vector3 origin1 = corner[index1];
        Vector3 vector3_1 = corner[index2];
        Vector3 vector3_2 = corner[index3];
        Vector2 a1 = triangle.uv[index1];
        Vector2 b1 = triangle.uv[index2];
        Vector2 b2 = triangle.uv[index3];
        Color32 a2 = triangle.color[index1];
        Color32 b3 = triangle.color[index2];
        Color32 b4 = triangle.color[index3];
        float enter1 = 0.0f;
        Vector3 vector3_3 = vector3_1 - origin1;
        Ray ray1 = new Ray(origin1, vector3_3.normalized);
        plane.Raycast(ray1, out enter1);
        float t1 = enter1 / vector3_3.magnitude;
        Vector3 point1 = ray1.GetPoint(enter1);
        Vector2 vector2_1 = Vector2.Lerp(a1, b1, t1);
        Color32 color32_1 = Color32.Lerp(a2, b3, t1);
        vector3_3 = vector3_2 - origin1;
        ray1 = new Ray(origin1, vector3_3.normalized);
        plane.Raycast(ray1, out enter1);
        float t2 = enter1 / vector3_3.magnitude;
        Vector3 point2 = ray1.GetPoint(enter1);
        Vector2 vector2_2 = Vector2.Lerp(a1, b2, t2);
        Color32 color32_2 = Color32.Lerp(a2, b4, t2);
        dfClippingUtil.ClipTriangle clipTriangle1 = dest[destIndex];
        clipTriangle1.corner[0] = origin1;
        clipTriangle1.corner[1] = point1;
        clipTriangle1.corner[2] = point2;
        clipTriangle1.uv[0] = a1;
        clipTriangle1.uv[1] = vector2_1;
        clipTriangle1.uv[2] = vector2_2;
        clipTriangle1.color[0] = a2;
        clipTriangle1.color[1] = color32_1;
        clipTriangle1.color[2] = color32_2;
        return 1;
      case 3:
        dfClippingUtil.ClipTriangle clipTriangle2 = dest[destIndex];
        Array.Copy((Array) triangle.corner, 0, (Array) clipTriangle2.corner, 0, 3);
        Array.Copy((Array) triangle.uv, 0, (Array) clipTriangle2.uv, 0, 3);
        Array.Copy((Array) triangle.color, 0, (Array) clipTriangle2.color, 0, 3);
        return 1;
      default:
        int index4 = num2;
        int index5 = (index4 + 1) % 3;
        int index6 = (index4 + 2) % 3;
        Vector3 origin2 = corner[index4];
        Vector3 vector3_4 = corner[index5];
        Vector3 vector3_5 = corner[index6];
        Vector2 a3 = triangle.uv[index4];
        Vector2 b5 = triangle.uv[index5];
        Vector2 b6 = triangle.uv[index6];
        Color32 a4 = triangle.color[index4];
        Color32 b7 = triangle.color[index5];
        Color32 b8 = triangle.color[index6];
        Vector3 vector3_6 = vector3_4 - origin2;
        Ray ray2 = new Ray(origin2, vector3_6.normalized);
        float enter2 = 0.0f;
        plane.Raycast(ray2, out enter2);
        float t3 = enter2 / vector3_6.magnitude;
        Vector3 point3 = ray2.GetPoint(enter2);
        Vector2 vector2_3 = Vector2.Lerp(a3, b5, t3);
        Color32 color32_3 = Color32.Lerp(a4, b7, t3);
        Vector3 vector3_7 = vector3_5 - origin2;
        ray2 = new Ray(origin2, vector3_7.normalized);
        plane.Raycast(ray2, out enter2);
        float t4 = enter2 / vector3_7.magnitude;
        Vector3 point4 = ray2.GetPoint(enter2);
        Vector2 vector2_4 = Vector2.Lerp(a3, b6, t4);
        Color32 color32_4 = Color32.Lerp(a4, b8, t4);
        dfClippingUtil.ClipTriangle clipTriangle3 = dest[destIndex];
        clipTriangle3.corner[0] = point3;
        clipTriangle3.corner[1] = vector3_4;
        clipTriangle3.corner[2] = point4;
        clipTriangle3.uv[0] = vector2_3;
        clipTriangle3.uv[1] = b5;
        clipTriangle3.uv[2] = vector2_4;
        clipTriangle3.color[0] = color32_3;
        clipTriangle3.color[1] = b7;
        clipTriangle3.color[2] = color32_4;
        clipTriangle3 = dest[++destIndex];
        clipTriangle3.corner[0] = point4;
        clipTriangle3.corner[1] = vector3_4;
        clipTriangle3.corner[2] = vector3_5;
        clipTriangle3.uv[0] = vector2_4;
        clipTriangle3.uv[1] = b5;
        clipTriangle3.uv[2] = b6;
        clipTriangle3.color[0] = color32_4;
        clipTriangle3.color[1] = b7;
        clipTriangle3.color[2] = b8;
        return 2;
    }
  }

  private static dfClippingUtil.ClipTriangle[] initClipBuffer(int size)
  {
    dfClippingUtil.ClipTriangle[] clipTriangleArray = new dfClippingUtil.ClipTriangle[size];
    for (int index = 0; index < size; ++index)
    {
      clipTriangleArray[index].corner = new Vector3[3];
      clipTriangleArray[index].uv = new Vector2[3];
      clipTriangleArray[index].color = new Color32[3];
    }
    return clipTriangleArray;
  }

  protected struct ClipTriangle
  {
    public Vector3[] corner;
    public Vector2[] uv;
    public Color32[] color;

    public void CopyTo(ref dfClippingUtil.ClipTriangle target)
    {
      Array.Copy((Array) this.corner, 0, (Array) target.corner, 0, 3);
      Array.Copy((Array) this.uv, 0, (Array) target.uv, 0, 3);
      Array.Copy((Array) this.color, 0, (Array) target.color, 0, 3);
    }

    public void CopyTo(dfRenderData buffer)
    {
      int count = buffer.Vertices.Count;
      buffer.Vertices.AddRange(this.corner);
      buffer.UV.AddRange(this.uv);
      buffer.Colors.AddRange(this.color);
      buffer.Triangles.Add(count, count + 1, count + 2);
    }
  }
}
