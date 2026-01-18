// Decompiled with JetBrains decompiler
// Type: tk2dSpriteGeomGen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

  public static class tk2dSpriteGeomGen
  {
    private static readonly int[] boxIndicesBack = new int[36]
    {
      0,
      1,
      2,
      2,
      1,
      3,
      6,
      5,
      4,
      7,
      5,
      6,
      3,
      7,
      6,
      2,
      3,
      6,
      4,
      5,
      1,
      4,
      1,
      0,
      6,
      4,
      0,
      6,
      0,
      2,
      1,
      7,
      3,
      5,
      7,
      1
    };
    private static readonly int[] boxIndicesFwd = new int[36]
    {
      2,
      1,
      0,
      3,
      1,
      2,
      4,
      5,
      6,
      6,
      5,
      7,
      6,
      7,
      3,
      6,
      3,
      2,
      1,
      5,
      4,
      0,
      1,
      4,
      0,
      4,
      6,
      2,
      0,
      6,
      3,
      7,
      1,
      1,
      7,
      5
    };
    private static readonly Vector3[] boxUnitVertices = new Vector3[8]
    {
      new Vector3(-1f, -1f, -1f),
      new Vector3(-1f, -1f, 1f),
      new Vector3(1f, -1f, -1f),
      new Vector3(1f, -1f, 1f),
      new Vector3(-1f, 1f, -1f),
      new Vector3(-1f, 1f, 1f),
      new Vector3(1f, 1f, -1f),
      new Vector3(1f, 1f, 1f)
    };
    private static Matrix4x4 boxScaleMatrix = Matrix4x4.identity;

    public static void SetSpriteColors(
      Color32[] dest,
      int offset,
      int numVertices,
      Color c,
      bool premulAlpha)
    {
      if (premulAlpha)
      {
        c.r *= c.a;
        c.g *= c.a;
        c.b *= c.a;
      }
      Color32 color32 = (Color32) c;
      for (int index = 0; index < numVertices; ++index)
        dest[offset + index] = color32;
    }

    public static Vector2 GetAnchorOffset(tk2dBaseSprite.Anchor anchor, float width, float height)
    {
      Vector2 zero = Vector2.zero;
      switch (anchor)
      {
        case tk2dBaseSprite.Anchor.LowerCenter:
        case tk2dBaseSprite.Anchor.MiddleCenter:
        case tk2dBaseSprite.Anchor.UpperCenter:
          zero.x = (float) (int) ((double) width / 2.0);
          break;
        case tk2dBaseSprite.Anchor.LowerRight:
        case tk2dBaseSprite.Anchor.MiddleRight:
        case tk2dBaseSprite.Anchor.UpperRight:
          zero.x = (float) (int) width;
          break;
      }
      switch (anchor)
      {
        case tk2dBaseSprite.Anchor.LowerLeft:
        case tk2dBaseSprite.Anchor.LowerCenter:
        case tk2dBaseSprite.Anchor.LowerRight:
          zero.y = (float) (int) height;
          break;
        case tk2dBaseSprite.Anchor.MiddleLeft:
        case tk2dBaseSprite.Anchor.MiddleCenter:
        case tk2dBaseSprite.Anchor.MiddleRight:
          zero.y = (float) (int) ((double) height / 2.0);
          break;
      }
      return zero;
    }

    public static void GetSpriteGeomDesc(
      out int numVertices,
      out int numIndices,
      tk2dSpriteDefinition spriteDef)
    {
      numVertices = 4;
      numIndices = spriteDef.indices.Length;
    }

    public static void SetSpriteGeom(
      Vector3[] pos,
      Vector2[] uv,
      Vector3[] norm,
      Vector4[] tang,
      int offset,
      tk2dSpriteDefinition spriteDef,
      Vector3 scale)
    {
      pos[offset] = Vector3.Scale(spriteDef.position0, scale);
      pos[offset + 1] = Vector3.Scale(spriteDef.position1, scale);
      pos[offset + 2] = Vector3.Scale(spriteDef.position2, scale);
      pos[offset + 3] = Vector3.Scale(spriteDef.position3, scale);
      for (int index = 0; index < spriteDef.uvs.Length; ++index)
        uv[offset + index] = spriteDef.uvs[index];
      if (norm != null && spriteDef.normals != null)
      {
        for (int index = 0; index < spriteDef.normals.Length; ++index)
          norm[offset + index] = spriteDef.normals[index];
      }
      if (tang == null || spriteDef.tangents == null)
        return;
      for (int index = 0; index < spriteDef.tangents.Length; ++index)
        tang[offset + index] = spriteDef.tangents[index];
    }

    public static void SetSpriteIndices(
      int[] indices,
      int offset,
      int vStart,
      tk2dSpriteDefinition spriteDef)
    {
      for (int index = 0; index < spriteDef.indices.Length; ++index)
        indices[offset + index] = vStart + spriteDef.indices[index];
    }

    public static void GetClippedSpriteGeomDesc(
      out int numVertices,
      out int numIndices,
      tk2dSpriteDefinition spriteDef)
    {
      numVertices = 4;
      numIndices = 6;
    }

    public static void SetClippedSpriteGeom(
      Vector3[] pos,
      Vector2[] uv,
      int offset,
      out Vector3 boundsCenter,
      out Vector3 boundsExtents,
      tk2dSpriteDefinition spriteDef,
      Vector3 scale,
      Vector2 clipBottomLeft,
      Vector2 clipTopRight,
      float colliderOffsetZ,
      float colliderExtentZ)
    {
      boundsCenter = Vector3.zero;
      boundsExtents = Vector3.zero;
      Vector3 vector3_1 = spriteDef.untrimmedBoundsDataCenter - spriteDef.untrimmedBoundsDataExtents * 0.5f;
      Vector3 vector3_2 = spriteDef.untrimmedBoundsDataCenter + spriteDef.untrimmedBoundsDataExtents * 0.5f;
      float num1 = Mathf.Lerp(vector3_1.x, vector3_2.x, clipBottomLeft.x);
      float num2 = Mathf.Lerp(vector3_1.x, vector3_2.x, clipTopRight.x);
      float num3 = Mathf.Lerp(vector3_1.y, vector3_2.y, clipBottomLeft.y);
      float num4 = Mathf.Lerp(vector3_1.y, vector3_2.y, clipTopRight.y);
      Vector3 boundsDataExtents = spriteDef.boundsDataExtents;
      Vector3 vector3_3 = spriteDef.boundsDataCenter - boundsDataExtents * 0.5f;
      float num5 = (num1 - vector3_3.x) / boundsDataExtents.x;
      float num6 = (num2 - vector3_3.x) / boundsDataExtents.x;
      float num7 = (num3 - vector3_3.y) / boundsDataExtents.y;
      float num8 = (num4 - vector3_3.y) / boundsDataExtents.y;
      Vector2 vector2_1 = new Vector2(Mathf.Clamp01(num5), Mathf.Clamp01(num7));
      Vector2 vector2_2 = new Vector2(Mathf.Clamp01(num6), Mathf.Clamp01(num8));
      Vector3 position0 = spriteDef.position0;
      Vector3 position3 = spriteDef.position3;
      Vector3 vector3_4 = new Vector3(Mathf.Lerp(position0.x, position3.x, vector2_1.x) * scale.x, Mathf.Lerp(position0.y, position3.y, vector2_1.y) * scale.y, position0.z * scale.z);
      Vector3 vector3_5 = new Vector3(Mathf.Lerp(position0.x, position3.x, vector2_2.x) * scale.x, Mathf.Lerp(position0.y, position3.y, vector2_2.y) * scale.y, position0.z * scale.z);
      boundsCenter.Set(vector3_4.x + (float) (((double) vector3_5.x - (double) vector3_4.x) * 0.5), vector3_4.y + (float) (((double) vector3_5.y - (double) vector3_4.y) * 0.5), colliderOffsetZ);
      boundsExtents.Set((float) (((double) vector3_5.x - (double) vector3_4.x) * 0.5), (float) (((double) vector3_5.y - (double) vector3_4.y) * 0.5), colliderExtentZ);
      pos[offset] = new Vector3(vector3_4.x, vector3_4.y, vector3_4.z);
      pos[offset + 1] = new Vector3(vector3_5.x, vector3_4.y, vector3_4.z);
      pos[offset + 2] = new Vector3(vector3_4.x, vector3_5.y, vector3_4.z);
      pos[offset + 3] = new Vector3(vector3_5.x, vector3_5.y, vector3_4.z);
      if (spriteDef.flipped == tk2dSpriteDefinition.FlipMode.Tk2d)
      {
        Vector2 vector2_3 = new Vector2(Mathf.Lerp(spriteDef.uvs[0].x, spriteDef.uvs[3].x, vector2_1.y), Mathf.Lerp(spriteDef.uvs[0].y, spriteDef.uvs[3].y, vector2_1.x));
        Vector2 vector2_4 = new Vector2(Mathf.Lerp(spriteDef.uvs[0].x, spriteDef.uvs[3].x, vector2_2.y), Mathf.Lerp(spriteDef.uvs[0].y, spriteDef.uvs[3].y, vector2_2.x));
        uv[offset] = new Vector2(vector2_3.x, vector2_3.y);
        uv[offset + 1] = new Vector2(vector2_3.x, vector2_4.y);
        uv[offset + 2] = new Vector2(vector2_4.x, vector2_3.y);
        uv[offset + 3] = new Vector2(vector2_4.x, vector2_4.y);
      }
      else if (spriteDef.flipped == tk2dSpriteDefinition.FlipMode.TPackerCW)
      {
        Vector2 vector2_5 = new Vector2(Mathf.Lerp(spriteDef.uvs[0].x, spriteDef.uvs[3].x, vector2_1.y), Mathf.Lerp(spriteDef.uvs[0].y, spriteDef.uvs[3].y, vector2_1.x));
        Vector2 vector2_6 = new Vector2(Mathf.Lerp(spriteDef.uvs[0].x, spriteDef.uvs[3].x, vector2_2.y), Mathf.Lerp(spriteDef.uvs[0].y, spriteDef.uvs[3].y, vector2_2.x));
        uv[offset] = new Vector2(vector2_5.x, vector2_5.y);
        uv[offset + 2] = new Vector2(vector2_6.x, vector2_5.y);
        uv[offset + 1] = new Vector2(vector2_5.x, vector2_6.y);
        uv[offset + 3] = new Vector2(vector2_6.x, vector2_6.y);
      }
      else
      {
        Vector2 vector2_7 = new Vector2(Mathf.Lerp(spriteDef.uvs[0].x, spriteDef.uvs[3].x, vector2_1.x), Mathf.Lerp(spriteDef.uvs[0].y, spriteDef.uvs[3].y, vector2_1.y));
        Vector2 vector2_8 = new Vector2(Mathf.Lerp(spriteDef.uvs[0].x, spriteDef.uvs[3].x, vector2_2.x), Mathf.Lerp(spriteDef.uvs[0].y, spriteDef.uvs[3].y, vector2_2.y));
        uv[offset] = new Vector2(vector2_7.x, vector2_7.y);
        uv[offset + 1] = new Vector2(vector2_8.x, vector2_7.y);
        uv[offset + 2] = new Vector2(vector2_7.x, vector2_8.y);
        uv[offset + 3] = new Vector2(vector2_8.x, vector2_8.y);
      }
    }

    public static void SetClippedSpriteIndices(
      int[] indices,
      int offset,
      int vStart,
      tk2dSpriteDefinition spriteDef)
    {
      indices[offset] = vStart;
      indices[offset + 1] = vStart + 3;
      indices[offset + 2] = vStart + 1;
      indices[offset + 3] = vStart + 2;
      indices[offset + 4] = vStart + 3;
      indices[offset + 5] = vStart;
    }

    public static void GetSlicedSpriteGeomDesc(
      out int numVertices,
      out int numIndices,
      tk2dSpriteDefinition spriteDef,
      bool borderOnly,
      bool tileStretchedSprite,
      Vector2 dimensions,
      Vector2 borderBottomLeft,
      Vector2 borderTopRight,
      float borderCornerBottom)
    {
      if (tileStretchedSprite)
      {
        tk2dSpriteGeomGen.GetSlicedTiledSpriteGeomDesc(out numVertices, out numIndices, spriteDef, borderOnly, dimensions, borderBottomLeft, borderTopRight, borderCornerBottom);
      }
      else
      {
        numVertices = 16 /*0x10*/;
        numIndices = !borderOnly ? 54 : 48 /*0x30*/;
      }
    }

    public static void SetSlicedSpriteGeom(
      Vector3[] pos,
      Vector2[] uv,
      int offset,
      out Vector3 boundsCenter,
      out Vector3 boundsExtents,
      tk2dSpriteDefinition spriteDef,
      bool borderOnly,
      Vector3 scale,
      Vector2 dimensions,
      Vector2 borderBottomLeft,
      Vector2 borderTopRight,
      float borderCornerBottom,
      tk2dBaseSprite.Anchor anchor,
      float colliderOffsetZ,
      float colliderExtentZ,
      Vector2 anchorOffset,
      bool tileStretchedSprite)
    {
      if (tileStretchedSprite)
      {
        tk2dSpriteGeomGen.SetSlicedTiledSpriteGeom(pos, uv, offset, out boundsCenter, out boundsExtents, spriteDef, borderOnly, scale, dimensions, borderBottomLeft, borderTopRight, borderCornerBottom, anchor, colliderOffsetZ, colliderExtentZ, anchorOffset);
      }
      else
      {
        boundsCenter = Vector3.zero;
        boundsExtents = Vector3.zero;
        float x1 = spriteDef.texelSize.x;
        float y1 = spriteDef.texelSize.y;
        float num1 = spriteDef.position1.x - spriteDef.position0.x;
        float num2 = spriteDef.position2.y - spriteDef.position0.y;
        float num3 = borderTopRight.y * num2;
        float y2 = borderBottomLeft.y * num2;
        float num4 = borderTopRight.x * num1;
        float x2 = borderBottomLeft.x * num1;
        float x3 = dimensions.x * x1;
        float y3 = dimensions.y * y1;
        float num5 = 0.0f;
        float num6 = 0.0f;
        switch (anchor)
        {
          case tk2dBaseSprite.Anchor.LowerCenter:
          case tk2dBaseSprite.Anchor.MiddleCenter:
          case tk2dBaseSprite.Anchor.UpperCenter:
            num5 = (float) -(int) ((double) dimensions.x / 2.0);
            break;
          case tk2dBaseSprite.Anchor.LowerRight:
          case tk2dBaseSprite.Anchor.MiddleRight:
          case tk2dBaseSprite.Anchor.UpperRight:
            num5 = (float) -(int) dimensions.x;
            break;
        }
        switch (anchor)
        {
          case tk2dBaseSprite.Anchor.MiddleLeft:
          case tk2dBaseSprite.Anchor.MiddleCenter:
          case tk2dBaseSprite.Anchor.MiddleRight:
            num6 = (float) -(int) ((double) dimensions.y / 2.0);
            break;
          case tk2dBaseSprite.Anchor.UpperLeft:
          case tk2dBaseSprite.Anchor.UpperCenter:
          case tk2dBaseSprite.Anchor.UpperRight:
            num6 = (float) -(int) dimensions.y;
            break;
        }
        float num7 = num5 - anchorOffset.x;
        float num8 = num6 - anchorOffset.y;
        float x4 = num7 * x1;
        float y4 = num8 * y1;
        boundsCenter.Set(scale.x * (x3 * 0.5f + x4), scale.y * (y3 * 0.5f + y4), colliderOffsetZ);
        boundsExtents.Set(scale.x * (x3 * 0.5f), scale.y * (y3 * 0.5f), colliderExtentZ);
        Vector2[] uvs = spriteDef.uvs;
        Vector2 vector2_1 = uvs[1] - uvs[0];
        Vector2 vector2_2 = uvs[2] - uvs[0];
        Vector3 vector3 = new Vector3(x4, y4, 0.0f);
        Vector3[] vector3Array = new Vector3[4]
        {
          vector3,
          vector3 + new Vector3(0.0f, y2, 0.0f),
          vector3 + new Vector3(0.0f, y3 - num3, 0.0f),
          vector3 + new Vector3(0.0f, y3, 0.0f)
        };
        Vector2[] vector2Array = new Vector2[4]
        {
          uvs[0],
          uvs[0] + vector2_2 * borderBottomLeft.y,
          uvs[0] + vector2_2 * (1f - borderTopRight.y),
          uvs[0] + vector2_2
        };
        for (int index1 = 0; index1 < 4; ++index1)
        {
          pos[offset + index1 * 4] = vector3Array[index1];
          pos[offset + index1 * 4 + 1] = vector3Array[index1] + new Vector3(x2, 0.0f, 0.0f);
          pos[offset + index1 * 4 + 2] = vector3Array[index1] + new Vector3(x3 - num4, 0.0f, 0.0f);
          pos[offset + index1 * 4 + 3] = vector3Array[index1] + new Vector3(x3, 0.0f, 0.0f);
          for (int index2 = 0; index2 < 4; ++index2)
            pos[offset + index1 * 4 + index2] = Vector3.Scale(pos[offset + index1 * 4 + index2], scale);
          uv[offset + index1 * 4] = vector2Array[index1];
          uv[offset + index1 * 4 + 1] = vector2Array[index1] + vector2_1 * borderBottomLeft.x;
          uv[offset + index1 * 4 + 2] = vector2Array[index1] + vector2_1 * (1f - borderTopRight.x);
          uv[offset + index1 * 4 + 3] = vector2Array[index1] + vector2_1;
        }
      }
    }

    public static void SetSlicedSpriteIndices(
      int[] indices,
      int offset,
      int vStart,
      tk2dSpriteDefinition spriteDef,
      bool borderOnly,
      bool tileStretchedSprite,
      Vector2 dimensions,
      Vector2 borderBottomLeft,
      Vector2 borderTopRight,
      float borderCornerBottom)
    {
      if (tileStretchedSprite)
      {
        tk2dSpriteGeomGen.SetSlicedTiledSpriteIndices(indices, offset, vStart, spriteDef, borderOnly, dimensions, borderBottomLeft, borderTopRight, borderCornerBottom);
      }
      else
      {
        int[] numArray = new int[54]
        {
          0,
          4,
          1,
          1,
          4,
          5,
          1,
          5,
          2,
          2,
          5,
          6,
          2,
          6,
          3,
          3,
          6,
          7,
          4,
          8,
          5,
          5,
          8,
          9,
          6,
          10,
          7,
          7,
          10,
          11,
          8,
          12,
          9,
          9,
          12,
          13,
          9,
          13,
          10,
          10,
          13,
          14,
          10,
          14,
          11,
          11,
          14,
          15,
          5,
          9,
          6,
          6,
          9,
          10
        };
        int length = numArray.Length;
        if (borderOnly)
          length -= 6;
        for (int index = 0; index < length; ++index)
          indices[offset + index] = vStart + numArray[index];
      }
    }

    public static void GetSlicedTiledSpriteGeomDesc(
      out int numVertices,
      out int numIndices,
      tk2dSpriteDefinition spriteDef,
      bool borderOnly,
      Vector2 dimensions,
      Vector2 borderBottomLeft,
      Vector2 borderTopRight,
      float borderCornerBottom)
    {
      float x = spriteDef.texelSize.x;
      float y = spriteDef.texelSize.y;
      float num1 = spriteDef.position1.x - spriteDef.position0.x;
      float num2 = spriteDef.position2.y - spriteDef.position0.y;
      float num3 = borderTopRight.y * num2;
      float num4 = borderBottomLeft.y * num2;
      float num5 = borderTopRight.x * num1;
      float num6 = borderBottomLeft.x * num1;
      float num7 = borderCornerBottom * num2;
      float num8 = dimensions.x * x;
      float num9 = dimensions.y * y;
      float num10 = num1 - num5 - num6;
      float num11 = num2 - num3 - num4 - num7;
      float f1 = (num8 - num5 - num6) / num10;
      float f2 = (num9 - num3 - num4) / num11;
      int num12 = Mathf.CeilToInt(f1);
      if ((double) num6 > 0.0)
        ++num12;
      if ((double) num5 > 0.0)
        ++num12;
      int num13 = Mathf.CeilToInt(f2);
      if ((double) num3 > 0.0)
        ++num13;
      if ((double) num4 > 0.0)
        ++num13;
      int num14 = num12 * num13;
      if (borderOnly)
        num14 -= Mathf.CeilToInt(f1) * Mathf.CeilToInt(f2);
      if ((double) borderCornerBottom > 0.0)
        num14 += num12;
      numVertices = num14 * 4;
      numIndices = num14 * 6;
    }

    public static void SetSlicedTiledSpriteGeom(
      Vector3[] pos,
      Vector2[] uv,
      int offset,
      out Vector3 boundsCenter,
      out Vector3 boundsExtents,
      tk2dSpriteDefinition spriteDef,
      bool borderOnly,
      Vector3 scale,
      Vector2 dimensions,
      Vector2 borderBottomLeft,
      Vector2 borderTopRight,
      float borderCornerBottom,
      tk2dBaseSprite.Anchor anchor,
      float colliderOffsetZ,
      float colliderExtentZ,
      Vector2 anchorOffset)
    {
      boundsCenter = Vector3.zero;
      boundsExtents = Vector3.zero;
      float x1 = spriteDef.texelSize.x;
      float y1 = spriteDef.texelSize.y;
      float num1 = spriteDef.position1.x - spriteDef.position0.x;
      float num2 = spriteDef.position2.y - spriteDef.position0.y;
      float num3 = borderTopRight.y * num2;
      float num4 = borderBottomLeft.y * num2;
      float num5 = borderTopRight.x * num1;
      float num6 = borderBottomLeft.x * num1;
      float num7 = borderCornerBottom * num2;
      float num8 = dimensions.x * x1;
      float num9 = dimensions.y * y1;
      float num10 = num1 - num5 - num6;
      float num11 = num2 - num3 - num4 - num7;
      int num12 = Mathf.CeilToInt((num8 - num5 - num6) / num10);
      int num13 = Mathf.CeilToInt((num9 - num3 - num4) / num11);
      float num14 = 0.0f;
      float num15 = 0.0f;
      switch (anchor)
      {
        case tk2dBaseSprite.Anchor.LowerCenter:
        case tk2dBaseSprite.Anchor.MiddleCenter:
        case tk2dBaseSprite.Anchor.UpperCenter:
          num14 = (float) -(int) ((double) dimensions.x / 2.0);
          break;
        case tk2dBaseSprite.Anchor.LowerRight:
        case tk2dBaseSprite.Anchor.MiddleRight:
        case tk2dBaseSprite.Anchor.UpperRight:
          num14 = (float) -(int) dimensions.x;
          break;
      }
      switch (anchor)
      {
        case tk2dBaseSprite.Anchor.MiddleLeft:
        case tk2dBaseSprite.Anchor.MiddleCenter:
        case tk2dBaseSprite.Anchor.MiddleRight:
          num15 = (float) -(int) ((double) dimensions.y / 2.0);
          break;
        case tk2dBaseSprite.Anchor.UpperLeft:
        case tk2dBaseSprite.Anchor.UpperCenter:
        case tk2dBaseSprite.Anchor.UpperRight:
          num15 = (float) -(int) dimensions.y;
          break;
      }
      float num16 = num14 - anchorOffset.x;
      float num17 = num15 - anchorOffset.y;
      float x2 = num16 * x1;
      float y2 = num17 * y1;
      boundsCenter.Set(scale.x * (num8 * 0.5f + x2), scale.y * (num9 * 0.5f + y2), colliderOffsetZ);
      boundsExtents.Set(scale.x * (num8 * 0.5f), scale.y * (num9 * 0.5f), colliderExtentZ);
      Vector2[] uvs = spriteDef.uvs;
      Vector2 vector2_1 = uvs[1] - uvs[0];
      Vector2 vector2_2 = uvs[2] - uvs[0];
      Vector3 vector3 = new Vector3(x2, y2, 0.0f);
      Vector3 zero1 = Vector3.zero;
      Vector3 zero2 = Vector3.zero;
      Vector2 zero3 = Vector2.zero;
      Vector2 zero4 = Vector2.zero;
      for (int index1 = 0; index1 < num12 + 2; ++index1)
      {
        if (index1 == 0)
        {
          if ((double) num6 != 0.0)
          {
            zero1.x = 0.0f;
            zero2.x = num6;
            zero3.x = uvs[0].x;
            zero4.x = uvs[0].x + vector2_1.x * borderBottomLeft.x;
          }
          else
            continue;
        }
        else if (index1 == num12 + 1)
        {
          if ((double) num5 != 0.0)
          {
            zero1.x = num8 - num5;
            zero2.x = num8;
            zero3.x = uvs[0].x + vector2_1.x * (1f - borderTopRight.x);
            zero4.x = uvs[0].x + vector2_1.x;
          }
          else
            continue;
        }
        else
        {
          zero1.x = num6 + (float) (index1 - 1) * num10;
          zero2.x = Mathf.Min(num6 + (float) index1 * num10, num8 - num5);
          zero3.x = uvs[0].x + vector2_1.x * borderBottomLeft.x;
          zero4.x = uvs[0].x + vector2_1.x * (1f - borderTopRight.x);
          if (index1 == num12)
            zero4.x = Mathf.Lerp(zero3.x, zero4.x, (zero2.x - zero1.x) / num10);
        }
        if ((double) borderCornerBottom > 0.0)
        {
          zero1.y = 0.0f;
          zero2.y = num7;
          zero3.y = uvs[0].y;
          zero4.y = (uvs[0] + vector2_2 * borderCornerBottom).y;
          pos[offset] = Vector3.Scale(vector3 + new Vector3(zero1.x, -num7, 2f * num7), scale);
          pos[offset + 1] = Vector3.Scale(vector3 + new Vector3(zero2.x, -num7, 2f * num7), scale);
          pos[offset + 2] = Vector3.Scale(vector3 + new Vector3(zero1.x, 0.0f, 0.0f), scale);
          pos[offset + 3] = Vector3.Scale(vector3 + new Vector3(zero2.x, 0.0f, 0.0f), scale);
          uv[offset] = new Vector2(zero3.x, zero3.y);
          uv[offset + 1] = new Vector2(zero4.x, zero3.y);
          uv[offset + 2] = new Vector2(zero3.x, zero4.y);
          uv[offset + 3] = new Vector2(zero4.x, zero4.y);
          offset += 4;
        }
        for (int index2 = 0; index2 < num13 + 2; ++index2)
        {
          if (index2 == 0)
          {
            if ((double) num4 != 0.0)
            {
              zero1.y = 0.0f;
              zero2.y = num4;
              zero3.y = (uvs[0] + vector2_2 * borderCornerBottom).y;
              zero4.y = (uvs[0] + vector2_2 * (borderBottomLeft.y + borderCornerBottom)).y;
            }
            else
              continue;
          }
          else if (index2 == num13 + 1)
          {
            if ((double) num3 != 0.0)
            {
              zero1.y = num9 - num3;
              zero2.y = num9;
              zero3.y = uvs[0].y + vector2_2.y * (1f - borderTopRight.y);
              zero4.y = uvs[0].y + vector2_2.y;
            }
            else
              continue;
          }
          else if (!borderOnly || index1 == 0 || index1 == num12 + 1)
          {
            zero1.y = num4 + (float) (index2 - 1) * num11;
            zero2.y = Mathf.Min(num4 + (float) index2 * num11, num9 - num3);
            zero3.y = uvs[0].y + vector2_2.y * (borderBottomLeft.y + borderCornerBottom);
            zero4.y = uvs[0].y + vector2_2.y * (1f - borderTopRight.y);
            if (index2 == num13)
              zero4.y = Mathf.Lerp(zero3.y, zero4.y, (zero2.y - zero1.y) / num11);
          }
          else
            continue;
          pos[offset] = Vector3.Scale(vector3 + new Vector3(zero1.x, zero1.y), scale);
          pos[offset + 1] = Vector3.Scale(vector3 + new Vector3(zero2.x, zero1.y), scale);
          pos[offset + 2] = Vector3.Scale(vector3 + new Vector3(zero1.x, zero2.y), scale);
          pos[offset + 3] = Vector3.Scale(vector3 + new Vector3(zero2.x, zero2.y), scale);
          uv[offset] = new Vector2(zero3.x, zero3.y);
          uv[offset + 1] = new Vector2(zero4.x, zero3.y);
          uv[offset + 2] = new Vector2(zero3.x, zero4.y);
          uv[offset + 3] = new Vector2(zero4.x, zero4.y);
          offset += 4;
        }
      }
    }

    public static void SetSlicedTiledSpriteIndices(
      int[] indices,
      int offset,
      int vStart,
      tk2dSpriteDefinition spriteDef,
      bool borderOnly,
      Vector2 dimensions,
      Vector2 borderBottomLeft,
      Vector2 borderTopRight,
      float borderCornerBottom)
    {
      int numIndices;
      tk2dSpriteGeomGen.GetSlicedTiledSpriteGeomDesc(out int _, out numIndices, spriteDef, borderOnly, dimensions, borderBottomLeft, borderTopRight, borderCornerBottom);
      int num = 0;
      for (int index = 0; index < numIndices; index += 6)
      {
        indices[offset + index] = vStart + spriteDef.indices[0] + num;
        indices[offset + index + 1] = vStart + spriteDef.indices[1] + num;
        indices[offset + index + 2] = vStart + spriteDef.indices[2] + num;
        indices[offset + index + 3] = vStart + spriteDef.indices[3] + num;
        indices[offset + index + 4] = vStart + spriteDef.indices[4] + num;
        indices[offset + index + 5] = vStart + spriteDef.indices[5] + num;
        num += 4;
      }
    }

    public static void GetTiledSpriteGeomDesc(
      out int numVertices,
      out int numIndices,
      tk2dSpriteDefinition spriteDef,
      Vector2 dimensions)
    {
      int num1 = (int) Mathf.Ceil(dimensions.x * spriteDef.texelSize.x / spriteDef.untrimmedBoundsDataExtents.x);
      int num2 = (int) Mathf.Ceil(dimensions.y * spriteDef.texelSize.y / spriteDef.untrimmedBoundsDataExtents.y);
      numVertices = num1 * num2 * 4;
      numIndices = num1 * num2 * 6;
    }

    public static void SetTiledSpriteGeom(
      Vector3[] pos,
      Vector2[] uv,
      int offset,
      out Vector3 boundsCenter,
      out Vector3 boundsExtents,
      tk2dSpriteDefinition spriteDef,
      Vector3 scale,
      Vector2 dimensions,
      tk2dBaseSprite.Anchor anchor,
      float colliderOffsetZ,
      float colliderExtentZ)
    {
      boundsCenter = Vector3.zero;
      boundsExtents = Vector3.zero;
      int num1 = (int) Mathf.Ceil(dimensions.x * spriteDef.texelSize.x / spriteDef.untrimmedBoundsDataExtents.x);
      int num2 = (int) Mathf.Ceil(dimensions.y * spriteDef.texelSize.y / spriteDef.untrimmedBoundsDataExtents.y);
      Vector2 vector2_1 = new Vector2(dimensions.x * spriteDef.texelSize.x * scale.x, dimensions.y * spriteDef.texelSize.y * scale.y);
      Vector2 vector2_2 = Vector2.Scale(spriteDef.texelSize, (Vector2) scale) * 0.1f;
      Vector3 zero1 = Vector3.zero;
      switch (anchor)
      {
        case tk2dBaseSprite.Anchor.LowerCenter:
        case tk2dBaseSprite.Anchor.MiddleCenter:
        case tk2dBaseSprite.Anchor.UpperCenter:
          zero1.x = (float) -((double) vector2_1.x / 2.0);
          break;
        case tk2dBaseSprite.Anchor.LowerRight:
        case tk2dBaseSprite.Anchor.MiddleRight:
        case tk2dBaseSprite.Anchor.UpperRight:
          zero1.x = -vector2_1.x;
          break;
      }
      switch (anchor)
      {
        case tk2dBaseSprite.Anchor.MiddleLeft:
        case tk2dBaseSprite.Anchor.MiddleCenter:
        case tk2dBaseSprite.Anchor.MiddleRight:
          zero1.y = (float) -((double) vector2_1.y / 2.0);
          break;
        case tk2dBaseSprite.Anchor.UpperLeft:
        case tk2dBaseSprite.Anchor.UpperCenter:
        case tk2dBaseSprite.Anchor.UpperRight:
          zero1.y = -vector2_1.y;
          break;
      }
      Vector3 vector3_1 = zero1;
      Vector3 vector3_2 = zero1 - Vector3.Scale(spriteDef.position0, scale);
      boundsCenter.Set(vector2_1.x * 0.5f + vector3_1.x, vector2_1.y * 0.5f + vector3_1.y, colliderOffsetZ);
      boundsExtents.Set(vector2_1.x * 0.5f, vector2_1.y * 0.5f, colliderExtentZ);
      int num3 = 0;
      Vector3 vector3_3 = Vector3.Scale(spriteDef.untrimmedBoundsDataExtents, scale);
      Vector3 zero2 = Vector3.zero;
      Vector3 vector3_4 = zero2;
      for (int index1 = 0; index1 < num2; ++index1)
      {
        vector3_4.x = zero2.x;
        for (int index2 = 0; index2 < num1; ++index2)
        {
          float x = 1f;
          float y = 1f;
          if ((double) Mathf.Abs(vector3_4.x + vector3_3.x) > (double) Mathf.Abs(vector2_1.x) + (double) vector2_2.x)
            x = vector2_1.x % vector3_3.x / vector3_3.x;
          if ((double) Mathf.Abs(vector3_4.y + vector3_3.y) > (double) Mathf.Abs(vector2_1.y) + (double) vector2_2.y)
            y = vector2_1.y % vector3_3.y / vector3_3.y;
          Vector3 vector3_5 = vector3_4 + vector3_2;
          if ((double) x != 1.0 || (double) y != 1.0)
          {
            Vector2 zero3 = Vector2.zero;
            Vector2 vector2_3 = new Vector2(x, y);
            Vector3 vector3_6 = new Vector3(Mathf.Lerp(spriteDef.position0.x, spriteDef.position3.x, zero3.x) * scale.x, Mathf.Lerp(spriteDef.position0.y, spriteDef.position3.y, zero3.y) * scale.y, spriteDef.position0.z * scale.z);
            Vector3 vector3_7 = new Vector3(Mathf.Lerp(spriteDef.position0.x, spriteDef.position3.x, vector2_3.x) * scale.x, Mathf.Lerp(spriteDef.position0.y, spriteDef.position3.y, vector2_3.y) * scale.y, spriteDef.position0.z * scale.z);
            pos[offset + num3] = vector3_5 + new Vector3(vector3_6.x, vector3_6.y, vector3_6.z);
            pos[offset + num3 + 1] = vector3_5 + new Vector3(vector3_7.x, vector3_6.y, vector3_6.z);
            pos[offset + num3 + 2] = vector3_5 + new Vector3(vector3_6.x, vector3_7.y, vector3_6.z);
            pos[offset + num3 + 3] = vector3_5 + new Vector3(vector3_7.x, vector3_7.y, vector3_6.z);
            if (spriteDef.flipped == tk2dSpriteDefinition.FlipMode.Tk2d)
            {
              Vector2 vector2_4 = new Vector2(Mathf.Lerp(spriteDef.uvs[0].x, spriteDef.uvs[3].x, zero3.y), Mathf.Lerp(spriteDef.uvs[0].y, spriteDef.uvs[3].y, zero3.x));
              Vector2 vector2_5 = new Vector2(Mathf.Lerp(spriteDef.uvs[0].x, spriteDef.uvs[3].x, vector2_3.y), Mathf.Lerp(spriteDef.uvs[0].y, spriteDef.uvs[3].y, vector2_3.x));
              uv[offset + num3] = new Vector2(vector2_4.x, vector2_4.y);
              uv[offset + num3 + 1] = new Vector2(vector2_4.x, vector2_5.y);
              uv[offset + num3 + 2] = new Vector2(vector2_5.x, vector2_4.y);
              uv[offset + num3 + 3] = new Vector2(vector2_5.x, vector2_5.y);
            }
            else if (spriteDef.flipped == tk2dSpriteDefinition.FlipMode.TPackerCW)
            {
              Vector2 vector2_6 = new Vector2(Mathf.Lerp(spriteDef.uvs[0].x, spriteDef.uvs[3].x, zero3.y), Mathf.Lerp(spriteDef.uvs[0].y, spriteDef.uvs[3].y, zero3.x));
              Vector2 vector2_7 = new Vector2(Mathf.Lerp(spriteDef.uvs[0].x, spriteDef.uvs[3].x, vector2_3.y), Mathf.Lerp(spriteDef.uvs[0].y, spriteDef.uvs[3].y, vector2_3.x));
              uv[offset + num3] = new Vector2(vector2_6.x, vector2_6.y);
              uv[offset + num3 + 2] = new Vector2(vector2_7.x, vector2_6.y);
              uv[offset + num3 + 1] = new Vector2(vector2_6.x, vector2_7.y);
              uv[offset + num3 + 3] = new Vector2(vector2_7.x, vector2_7.y);
            }
            else
            {
              Vector2 vector2_8 = new Vector2(Mathf.Lerp(spriteDef.uvs[0].x, spriteDef.uvs[3].x, zero3.x), Mathf.Lerp(spriteDef.uvs[0].y, spriteDef.uvs[3].y, zero3.y));
              Vector2 vector2_9 = new Vector2(Mathf.Lerp(spriteDef.uvs[0].x, spriteDef.uvs[3].x, vector2_3.x), Mathf.Lerp(spriteDef.uvs[0].y, spriteDef.uvs[3].y, vector2_3.y));
              uv[offset + num3] = new Vector2(vector2_8.x, vector2_8.y);
              uv[offset + num3 + 1] = new Vector2(vector2_9.x, vector2_8.y);
              uv[offset + num3 + 2] = new Vector2(vector2_8.x, vector2_9.y);
              uv[offset + num3 + 3] = new Vector2(vector2_9.x, vector2_9.y);
            }
          }
          else
          {
            pos[offset + num3] = vector3_5 + Vector3.Scale(spriteDef.position0, scale);
            pos[offset + num3 + 1] = vector3_5 + Vector3.Scale(spriteDef.position1, scale);
            pos[offset + num3 + 2] = vector3_5 + Vector3.Scale(spriteDef.position2, scale);
            pos[offset + num3 + 3] = vector3_5 + Vector3.Scale(spriteDef.position3, scale);
            uv[offset + num3] = spriteDef.uvs[0];
            uv[offset + num3 + 1] = spriteDef.uvs[1];
            uv[offset + num3 + 2] = spriteDef.uvs[2];
            uv[offset + num3 + 3] = spriteDef.uvs[3];
          }
          num3 += 4;
          vector3_4.x += vector3_3.x;
        }
        vector3_4.y += vector3_3.y;
      }
    }

    public static void SetTiledSpriteIndices(
      int[] indices,
      int offset,
      int vStart,
      tk2dSpriteDefinition spriteDef,
      Vector2 dimensions,
      tk2dTiledSprite.OverrideGetTiledSpriteGeomDescDelegate overrideGetTiledSpriteGeomDesc = null)
    {
      int numVertices;
      int numIndices;
      if (overrideGetTiledSpriteGeomDesc != null)
        overrideGetTiledSpriteGeomDesc(out numVertices, out numIndices, spriteDef, dimensions);
      else
        tk2dSpriteGeomGen.GetTiledSpriteGeomDesc(out numVertices, out numIndices, spriteDef, dimensions);
      int num = 0;
      for (int index = 0; index < numIndices; index += 6)
      {
        indices[offset + index] = vStart + spriteDef.indices[0] + num;
        indices[offset + index + 1] = vStart + spriteDef.indices[1] + num;
        indices[offset + index + 2] = vStart + spriteDef.indices[2] + num;
        indices[offset + index + 3] = vStart + spriteDef.indices[3] + num;
        indices[offset + index + 4] = vStart + spriteDef.indices[4] + num;
        indices[offset + index + 5] = vStart + spriteDef.indices[5] + num;
        num += 4;
      }
      for (int index = offset + numIndices; index < indices.Length; ++index)
        indices[index] = 0;
    }

    public static void SetBoxMeshData(
      Vector3[] pos,
      int[] indices,
      int posOffset,
      int indicesOffset,
      int vStart,
      Vector3 origin,
      Vector3 extents,
      Matrix4x4 mat,
      Vector3 baseScale)
    {
      tk2dSpriteGeomGen.boxScaleMatrix.m03 = origin.x * baseScale.x;
      tk2dSpriteGeomGen.boxScaleMatrix.m13 = origin.y * baseScale.y;
      tk2dSpriteGeomGen.boxScaleMatrix.m23 = origin.z * baseScale.z;
      tk2dSpriteGeomGen.boxScaleMatrix.m00 = extents.x * baseScale.x;
      tk2dSpriteGeomGen.boxScaleMatrix.m11 = extents.y * baseScale.y;
      tk2dSpriteGeomGen.boxScaleMatrix.m22 = extents.z * baseScale.z;
      Matrix4x4 matrix4x4 = mat * tk2dSpriteGeomGen.boxScaleMatrix;
      for (int index = 0; index < 8; ++index)
        pos[posOffset + index] = matrix4x4.MultiplyPoint(tk2dSpriteGeomGen.boxUnitVertices[index]);
      int[] numArray = (double) (mat.m00 * mat.m11 * mat.m22 * baseScale.x * baseScale.y * baseScale.z) < 0.0 ? tk2dSpriteGeomGen.boxIndicesBack : tk2dSpriteGeomGen.boxIndicesFwd;
      for (int index = 0; index < numArray.Length; ++index)
        indices[indicesOffset + index] = vStart + numArray[index];
    }

    public static void SetSpriteDefinitionMeshData(
      Vector3[] pos,
      int[] indices,
      int posOffset,
      int indicesOffset,
      int vStart,
      tk2dSpriteDefinition spriteDef,
      Matrix4x4 mat,
      Vector3 baseScale)
    {
      for (int index = 0; index < spriteDef.colliderVertices.Length; ++index)
      {
        Vector3 point = Vector3.Scale(spriteDef.colliderVertices[index], baseScale);
        Vector3 vector3 = mat.MultiplyPoint(point);
        pos[posOffset + index] = vector3;
      }
      int[] indices1 = spriteDef.indices;
      for (int index = 0; index < indices1.Length; ++index)
        indices[indicesOffset + index] = vStart + indices1[index];
    }

    public static void SetSpriteVertexNormals(
      Vector3[] pos,
      Vector3 pMin,
      Vector3 pMax,
      Vector3[] spriteDefNormals,
      Vector4[] spriteDefTangents,
      Vector3[] normals,
      Vector4[] tangents)
    {
      Vector3 vector3 = pMax - pMin;
      int length = pos.Length;
      for (int index = 0; index < length; ++index)
      {
        Vector3 po = pos[index];
        float num1 = (po.x - pMin.x) / vector3.x;
        float num2 = (po.y - pMin.y) / vector3.y;
        float num3 = (float) ((1.0 - (double) num1) * (1.0 - (double) num2));
        float num4 = num1 * (1f - num2);
        float num5 = (1f - num1) * num2;
        float num6 = num1 * num2;
        if (spriteDefNormals != null && spriteDefNormals.Length == 4 && index < normals.Length)
          normals[index] = spriteDefNormals[0] * num3 + spriteDefNormals[1] * num4 + spriteDefNormals[2] * num5 + spriteDefNormals[3] * num6;
        if (spriteDefTangents != null && spriteDefTangents.Length == 4 && index < tangents.Length)
          tangents[index] = spriteDefTangents[0] * num3 + spriteDefTangents[1] * num4 + spriteDefTangents[2] * num5 + spriteDefTangents[3] * num6;
      }
    }

    public static void SetSpriteVertexNormalsFast(
      Vector3[] pos,
      Vector3[] normals,
      Vector4[] tangents)
    {
      int length = pos.Length;
      Vector3 back = Vector3.back;
      Vector4 vector4 = new Vector4(1f, 0.0f, 0.0f, 1f);
      for (int index = 0; index < length; ++index)
      {
        normals[index] = back;
        tangents[index] = vector4;
      }
    }
  }

