// Decompiled with JetBrains decompiler
// Type: dfRenderData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

public class dfRenderData : IDisposable
  {
    private static Queue<dfRenderData> pool = new Queue<dfRenderData>();
    public Material Material;
    public Shader Shader;
    public Matrix4x4 Transform;
    public dfList<Vector3> Vertices;
    public dfList<Vector2> UV;
    public dfList<Vector3> Normals;
    public dfList<Vector4> Tangents;
    public dfList<int> Triangles;
    public dfList<Color32> Colors;
    public uint Checksum;
    public dfIntersectionType Intersection;
    public bool Glitchy;

    internal dfRenderData()
      : this(32 /*0x20*/)
    {
    }

    internal dfRenderData(int capacity)
    {
      this.Vertices = new dfList<Vector3>(capacity);
      this.Triangles = new dfList<int>(capacity);
      this.Normals = new dfList<Vector3>(capacity);
      this.Tangents = new dfList<Vector4>(capacity);
      this.UV = new dfList<Vector2>(capacity);
      this.Colors = new dfList<Color32>(capacity);
      this.Transform = Matrix4x4.identity;
    }

    public static dfRenderData Obtain()
    {
      lock ((object) dfRenderData.pool)
        return dfRenderData.pool.Count <= 0 ? new dfRenderData() : dfRenderData.pool.Dequeue();
    }

    public static void FlushObjectPool()
    {
      lock ((object) dfRenderData.pool)
      {
        while (dfRenderData.pool.Count > 0)
        {
          dfRenderData dfRenderData = dfRenderData.pool.Dequeue();
          dfRenderData.Vertices.TrimExcess();
          dfRenderData.Triangles.TrimExcess();
          dfRenderData.UV.TrimExcess();
          dfRenderData.Colors.TrimExcess();
        }
      }
    }

    public void Release()
    {
      lock ((object) dfRenderData.pool)
      {
        this.Clear();
        dfRenderData.pool.Enqueue(this);
      }
    }

    public void Clear()
    {
      this.Material = (Material) null;
      this.Shader = (Shader) null;
      this.Transform = Matrix4x4.identity;
      this.Checksum = 0U;
      this.Intersection = dfIntersectionType.None;
      this.Vertices.Clear();
      this.UV.Clear();
      this.Triangles.Clear();
      this.Colors.Clear();
      this.Normals.Clear();
      this.Tangents.Clear();
    }

    public bool IsValid()
    {
      int count = this.Vertices.Count;
      return count > 0 && count <= 65000 && this.UV.Count == count && this.Colors.Count == count;
    }

    public void EnsureCapacity(int capacity)
    {
      this.Vertices.EnsureCapacity(capacity);
      this.Triangles.EnsureCapacity(Mathf.RoundToInt((float) capacity * 1.5f));
      this.UV.EnsureCapacity(capacity);
      this.Colors.EnsureCapacity(capacity);
      if (this.Normals != null)
        this.Normals.EnsureCapacity(capacity);
      if (this.Tangents == null)
        return;
      this.Tangents.EnsureCapacity(capacity);
    }

    public void Merge(dfRenderData buffer) => this.Merge(buffer, true);

    public void Merge(dfRenderData buffer, bool transformVertices)
    {
      int count1 = this.Vertices.Count;
      this.Vertices.AddRange(buffer.Vertices);
      if (transformVertices)
      {
        Vector3[] items = this.Vertices.Items;
        int count2 = buffer.Vertices.Count;
        Matrix4x4 transform = buffer.Transform;
        for (int index = count1; index < count1 + count2; ++index)
          items[index] = transform.MultiplyPoint(items[index]);
      }
      int count3 = this.Triangles.Count;
      this.Triangles.AddRange(buffer.Triangles);
      int count4 = buffer.Triangles.Count;
      int[] items1 = this.Triangles.Items;
      for (int index = count3; index < count3 + count4; ++index)
        items1[index] += count1;
      this.UV.AddRange(buffer.UV);
      this.Colors.AddRange(buffer.Colors);
      this.Normals.AddRange(buffer.Normals);
      this.Tangents.AddRange(buffer.Tangents);
    }

    internal void ApplyTransform(Matrix4x4 transform)
    {
      int count = this.Vertices.Count;
      Vector3[] items1 = this.Vertices.Items;
      for (int index = 0; index < count; ++index)
        items1[index] = transform.MultiplyPoint(items1[index]);
      if (this.Normals.Count <= 0)
        return;
      Vector3[] items2 = this.Normals.Items;
      for (int index = 0; index < count; ++index)
        items2[index] = transform.MultiplyVector(items2[index]);
    }

    public override string ToString()
    {
      return $"V:{this.Vertices.Count} T:{this.Triangles.Count} U:{this.UV.Count} C:{this.Colors.Count}";
    }

    public void Dispose() => this.Release();
  }

