// Decompiled with JetBrains decompiler
// Type: dfRadialSprite
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

[dfTooltip("Implements a sprite that can be filled in a radial fashion instead of strictly horizontally or vertically like other sprite classes. Useful for spell cooldown effects, map effects, etc.")]
[dfCategory("Basic Controls")]
[AddComponentMenu("Daikon Forge/User Interface/Sprite/Radial")]
[ExecuteInEditMode]
[dfHelp("http://www.daikonforge.com/docs/df-gui/classdf_radial_sprite.html")]
[Serializable]
public class dfRadialSprite : dfSprite
  {
    private static Vector3[] baseVerts = new Vector3[8]
    {
      new Vector3(0.0f, 0.5f, 0.0f),
      new Vector3(0.5f, 0.5f, 0.0f),
      new Vector3(0.5f, 0.0f, 0.0f),
      new Vector3(0.5f, -0.5f, 0.0f),
      new Vector3(0.0f, -0.5f, 0.0f),
      new Vector3(-0.5f, -0.5f, 0.0f),
      new Vector3(-0.5f, 0.0f, 0.0f),
      new Vector3(-0.5f, 0.5f, 0.0f)
    };
    [SerializeField]
    protected dfPivotPoint fillOrigin = dfPivotPoint.MiddleCenter;

    public dfPivotPoint FillOrigin
    {
      get => this.fillOrigin;
      set
      {
        if (value == this.fillOrigin)
          return;
        this.fillOrigin = value;
        this.Invalidate();
      }
    }

    protected override void OnRebuildRenderData()
    {
      if ((UnityEngine.Object) this.Atlas == (UnityEngine.Object) null || this.SpriteInfo == (dfAtlas.ItemInfo) null)
        return;
      this.renderData.Material = this.Atlas.Material;
      List<Vector3> verts = (List<Vector3>) null;
      List<int> indices = (List<int>) null;
      List<Vector2> uv = (List<Vector2>) null;
      this.buildMeshData(ref verts, ref indices, ref uv);
      Color32[] list = this.buildColors(verts.Count);
      this.renderData.Vertices.AddRange((IList<Vector3>) verts);
      this.renderData.Triangles.AddRange((IList<int>) indices);
      this.renderData.UV.AddRange((IList<Vector2>) uv);
      this.renderData.Colors.AddRange(list);
    }

    private void buildMeshData(ref List<Vector3> verts, ref List<int> indices, ref List<Vector2> uv)
    {
      List<Vector3> vector3List = verts = new List<Vector3>();
      verts.AddRange((IEnumerable<Vector3>) dfRadialSprite.baseVerts);
      int num1;
      int index1;
      switch (this.fillOrigin)
      {
        case dfPivotPoint.TopLeft:
          num1 = 4;
          index1 = 5;
          vector3List.RemoveAt(6);
          vector3List.RemoveAt(0);
          break;
        case dfPivotPoint.TopCenter:
          num1 = 6;
          index1 = 0;
          break;
        case dfPivotPoint.TopRight:
          num1 = 4;
          index1 = 0;
          vector3List.RemoveAt(2);
          vector3List.RemoveAt(0);
          break;
        case dfPivotPoint.MiddleLeft:
          num1 = 6;
          index1 = 6;
          break;
        case dfPivotPoint.MiddleCenter:
          num1 = 8;
          vector3List.Add(vector3List[0]);
          vector3List.Insert(0, Vector3.zero);
          index1 = 0;
          break;
        case dfPivotPoint.MiddleRight:
          num1 = 6;
          index1 = 2;
          break;
        case dfPivotPoint.BottomLeft:
          num1 = 4;
          index1 = 4;
          vector3List.RemoveAt(6);
          vector3List.RemoveAt(4);
          break;
        case dfPivotPoint.BottomCenter:
          num1 = 6;
          index1 = 4;
          break;
        case dfPivotPoint.BottomRight:
          num1 = 4;
          index1 = 2;
          vector3List.RemoveAt(4);
          vector3List.RemoveAt(2);
          break;
        default:
          throw new NotImplementedException();
      }
      this.makeFirst(vector3List, index1);
      List<int> intList = indices = this.buildTriangles(vector3List);
      float stepSize = 1f / (float) num1;
      float num2 = this.fillAmount.Quantize(stepSize);
      for (int index2 = Mathf.CeilToInt(num2 / stepSize) + 1; index2 < num1; ++index2)
      {
        if (this.invertFill)
        {
          intList.RemoveRange(0, 3);
        }
        else
        {
          vector3List.RemoveAt(vector3List.Count - 1);
          intList.RemoveRange(intList.Count - 3, 3);
        }
      }
      if ((double) this.fillAmount < 1.0)
      {
        int index3 = intList[!this.invertFill ? intList.Count - 2 : 2];
        int index4 = intList[!this.invertFill ? intList.Count - 1 : 1];
        float t = (this.FillAmount - num2) / stepSize;
        vector3List[index4] = Vector3.Lerp(vector3List[index3], vector3List[index4], t);
      }
      uv = this.buildUV(vector3List);
      float units = this.PixelsToUnits();
      Vector2 b = units * this.size;
      Vector3 vector3 = this.pivot.TransformToCenter(this.size) * units;
      for (int index5 = 0; index5 < vector3List.Count; ++index5)
        vector3List[index5] = Vector3.Scale(vector3List[index5], (Vector3) b) + vector3;
    }

    private void makeFirst(List<Vector3> list, int index)
    {
      if (index == 0)
        return;
      List<Vector3> range = list.GetRange(index, list.Count - index);
      list.RemoveRange(index, list.Count - index);
      list.InsertRange(0, (IEnumerable<Vector3>) range);
    }

    private List<int> buildTriangles(List<Vector3> verts)
    {
      List<int> intList = new List<int>();
      int count = verts.Count;
      for (int index = 1; index < count - 1; ++index)
      {
        intList.Add(0);
        intList.Add(index);
        intList.Add(index + 1);
      }
      return intList;
    }

    private List<Vector2> buildUV(List<Vector3> verts)
    {
      dfAtlas.ItemInfo spriteInfo = this.SpriteInfo;
      if (spriteInfo == (dfAtlas.ItemInfo) null)
        return (List<Vector2>) null;
      Rect rect = spriteInfo.region;
      if (this.flip.IsSet(dfSpriteFlip.FlipHorizontal))
        rect = new Rect(rect.xMax, rect.y, -rect.width, rect.height);
      if (this.flip.IsSet(dfSpriteFlip.FlipVertical))
        rect = new Rect(rect.x, rect.yMax, rect.width, -rect.height);
      Vector2 vector2_1 = new Vector2(rect.x, rect.y);
      Vector2 vector2_2 = new Vector2(0.5f, 0.5f);
      Vector2 b = new Vector2(rect.width, rect.height);
      List<Vector2> vector2List = new List<Vector2>(verts.Count);
      for (int index = 0; index < verts.Count; ++index)
      {
        Vector2 a = (Vector2) verts[index] + vector2_2;
        vector2List.Add(Vector2.Scale(a, b) + vector2_1);
      }
      return vector2List;
    }

    private Color32[] buildColors(int vertCount)
    {
      Color32 color32 = this.ApplyOpacity(!this.IsEnabled ? this.disabledColor : this.color);
      Color32[] color32Array = new Color32[vertCount];
      for (int index = 0; index < color32Array.Length; ++index)
        color32Array[index] = color32;
      return color32Array;
    }
  }

