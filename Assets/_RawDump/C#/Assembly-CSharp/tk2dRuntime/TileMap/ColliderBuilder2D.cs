// Decompiled with JetBrains decompiler
// Type: tk2dRuntime.TileMap.ColliderBuilder2D
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace tk2dRuntime.TileMap;

public static class ColliderBuilder2D
{
  public static void Build(tk2dTileMap tileMap, bool forceBuild)
  {
    bool flag = !forceBuild;
    int length = tileMap.Layers.Length;
    for (int index = 0; index < length; ++index)
    {
      Layer layer = tileMap.Layers[index];
      if (!layer.IsEmpty && tileMap.data.Layers[index].generateCollider)
      {
        for (int y = 0; y < layer.numRows; ++y)
        {
          int baseY = y * layer.divY;
          for (int x = 0; x < layer.numColumns; ++x)
          {
            int baseX = x * layer.divX;
            SpriteChunk chunk = layer.GetChunk(x, y);
            if ((!flag || chunk.Dirty) && !chunk.IsEmpty)
            {
              ColliderBuilder2D.BuildForChunk(tileMap, chunk, baseX, baseY);
              PhysicsMaterial2D physicsMaterial2D = tileMap.data.Layers[index].physicsMaterial2D;
              foreach (EdgeCollider2D edgeCollider in chunk.edgeColliders)
              {
                if ((UnityEngine.Object) edgeCollider != (UnityEngine.Object) null)
                  edgeCollider.sharedMaterial = physicsMaterial2D;
              }
            }
          }
        }
      }
    }
  }

  public static void BuildForChunk(tk2dTileMap tileMap, SpriteChunk chunk, int baseX, int baseY)
  {
    Vector2[] vertices = new Vector2[0];
    int[] indices = new int[0];
    List<Vector2[]> vector2ArrayList1 = new List<Vector2[]>();
    ColliderBuilder2D.BuildLocalMeshForChunk(tileMap, chunk, baseX, baseY, ref vertices, ref indices);
    if (indices.Length > 4)
    {
      vertices = ColliderBuilder2D.WeldVertices(vertices, ref indices);
      indices = ColliderBuilder2D.RemoveDuplicateEdges(indices);
    }
    List<Vector2[]> vector2ArrayList2 = ColliderBuilder2D.MergeEdges(vertices, indices);
    if ((UnityEngine.Object) chunk.meshCollider != (UnityEngine.Object) null)
    {
      tk2dUtil.DestroyImmediate((UnityEngine.Object) chunk.meshCollider);
      chunk.meshCollider = (MeshCollider) null;
    }
    if (vector2ArrayList2.Count == 0)
    {
      for (int index = 0; index < chunk.edgeColliders.Count; ++index)
      {
        if ((UnityEngine.Object) chunk.edgeColliders[index] != (UnityEngine.Object) null)
          tk2dUtil.DestroyImmediate((UnityEngine.Object) chunk.edgeColliders[index]);
      }
      chunk.edgeColliders.Clear();
    }
    else
    {
      int count1 = vector2ArrayList2.Count;
      for (int index = count1; index < chunk.edgeColliders.Count; ++index)
      {
        if ((UnityEngine.Object) chunk.edgeColliders[index] != (UnityEngine.Object) null)
          tk2dUtil.DestroyImmediate((UnityEngine.Object) chunk.edgeColliders[index]);
      }
      int count2 = chunk.edgeColliders.Count - count1;
      if (count2 > 0)
        chunk.edgeColliders.RemoveRange(chunk.edgeColliders.Count - count2, count2);
      for (int index = 0; index < chunk.edgeColliders.Count; ++index)
      {
        if ((UnityEngine.Object) chunk.edgeColliders[index] == (UnityEngine.Object) null)
          chunk.edgeColliders[index] = tk2dUtil.AddComponent<EdgeCollider2D>(chunk.gameObject);
      }
      while (chunk.edgeColliders.Count < count1)
        chunk.edgeColliders.Add(tk2dUtil.AddComponent<EdgeCollider2D>(chunk.gameObject));
      for (int index = 0; index < count1; ++index)
        chunk.edgeColliders[index].points = vector2ArrayList2[index];
    }
  }

  private static void BuildLocalMeshForChunk(
    tk2dTileMap tileMap,
    SpriteChunk chunk,
    int baseX,
    int baseY,
    ref Vector2[] vertices,
    ref int[] indices)
  {
    List<Vector2> vector2List = new List<Vector2>();
    List<int> intList = new List<int>();
    Vector2[] vector2Array = new Vector2[4];
    int[] numArray1 = new int[8]{ 0, 1, 1, 2, 2, 3, 3, 0 };
    int[] numArray2 = new int[8]{ 0, 3, 3, 2, 2, 1, 1, 0 };
    int length = tileMap.SpriteCollectionInst.spriteDefinitions.Length;
    Vector2 vector2_1 = (Vector2) new Vector3(tileMap.data.tileSize.x, tileMap.data.tileSize.y);
    GameObject[] tilePrefabs = tileMap.data.tilePrefabs;
    float x = 0.0f;
    float y = 0.0f;
    tileMap.data.GetTileOffset(out x, out y);
    int[] spriteIds = chunk.spriteIds;
    for (int index1 = 0; index1 < tileMap.partitionSizeY; ++index1)
    {
      float num = (float) (baseY + index1 & 1) * x;
      for (int index2 = 0; index2 < tileMap.partitionSizeX; ++index2)
      {
        int rawTile = spriteIds[index1 * tileMap.partitionSizeX + index2];
        int tileFromRawTile = BuilderUtil.GetTileFromRawTile(rawTile);
        Vector2 vector2_2 = new Vector2(vector2_1.x * ((float) index2 + num), vector2_1.y * (float) index1);
        if (tileFromRawTile >= 0 && tileFromRawTile < length && !(bool) (UnityEngine.Object) tilePrefabs[tileFromRawTile])
        {
          bool flipH = BuilderUtil.IsRawTileFlagSet(rawTile, tk2dTileFlags.FlipX);
          bool flipV = BuilderUtil.IsRawTileFlagSet(rawTile, tk2dTileFlags.FlipY);
          bool rot90 = BuilderUtil.IsRawTileFlagSet(rawTile, tk2dTileFlags.Rot90);
          bool flag = false;
          if (flipH)
            flag = !flag;
          if (flipV)
            flag = !flag;
          tk2dSpriteDefinition spriteDefinition = tileMap.SpriteCollectionInst.spriteDefinitions[tileFromRawTile];
          int count = vector2List.Count;
          if (spriteDefinition.colliderType == tk2dSpriteDefinition.ColliderType.Box)
          {
            Vector3 colliderVertex1 = spriteDefinition.colliderVertices[0];
            Vector3 colliderVertex2 = spriteDefinition.colliderVertices[1];
            Vector3 vector3_1 = colliderVertex1 - colliderVertex2;
            Vector3 vector3_2 = colliderVertex1 + colliderVertex2;
            vector2Array[0] = new Vector2(vector3_1.x, vector3_1.y);
            vector2Array[1] = new Vector2(vector3_2.x, vector3_1.y);
            vector2Array[2] = new Vector2(vector3_2.x, vector3_2.y);
            vector2Array[3] = new Vector2(vector3_1.x, vector3_2.y);
            for (int index3 = 0; index3 < 4; ++index3)
              vector2List.Add(BuilderUtil.ApplySpriteVertexTileFlags(tileMap, spriteDefinition, vector2Array[index3], flipH, flipV, rot90) + vector2_2);
            int[] numArray3 = !flag ? numArray1 : numArray2;
            for (int index4 = 0; index4 < 8; ++index4)
              intList.Add(count + numArray3[index4]);
          }
          else if (spriteDefinition.colliderType != tk2dSpriteDefinition.ColliderType.Mesh)
            ;
        }
      }
    }
    vertices = vector2List.ToArray();
    indices = intList.ToArray();
  }

  private static int CompareWeldVertices(Vector2 a, Vector2 b)
  {
    float num = 0.01f;
    float f1 = a.x - b.x;
    if ((double) Mathf.Abs(f1) > (double) num)
      return (int) Mathf.Sign(f1);
    float f2 = a.y - b.y;
    return (double) Mathf.Abs(f2) > (double) num ? (int) Mathf.Sign(f2) : 0;
  }

  private static Vector2[] WeldVertices(Vector2[] vertices, ref int[] indices)
  {
    int[] array = new int[vertices.Length];
    for (int index = 0; index < vertices.Length; ++index)
      array[index] = index;
    Array.Sort<int>(array, (Comparison<int>) ((a, b) => ColliderBuilder2D.CompareWeldVertices(vertices[a], vertices[b])));
    List<Vector2> vector2List = new List<Vector2>();
    int[] numArray = new int[vertices.Length];
    Vector2 b1 = vertices[array[0]];
    vector2List.Add(b1);
    numArray[array[0]] = vector2List.Count - 1;
    for (int index = 1; index < array.Length; ++index)
    {
      Vector2 vertex = vertices[array[index]];
      if (ColliderBuilder2D.CompareWeldVertices(vertex, b1) != 0)
      {
        b1 = vertex;
        vector2List.Add(b1);
        numArray[array[index]] = vector2List.Count - 1;
      }
      numArray[array[index]] = vector2List.Count - 1;
    }
    for (int index = 0; index < indices.Length; ++index)
      indices[index] = numArray[indices[index]];
    return vector2List.ToArray();
  }

  private static int CompareDuplicateFaces(int[] indices, int face0index, int face1index)
  {
    for (int index = 0; index < 2; ++index)
    {
      int num = indices[face0index + index] - indices[face1index + index];
      if (num != 0)
        return num;
    }
    return 0;
  }

  private static int[] RemoveDuplicateEdges(int[] indices)
  {
    int[] sortedFaceIndices = new int[indices.Length];
    for (int index = 0; index < indices.Length; index += 2)
    {
      if (indices[index] > indices[index + 1])
      {
        sortedFaceIndices[index] = indices[index + 1];
        sortedFaceIndices[index + 1] = indices[index];
      }
      else
      {
        sortedFaceIndices[index] = indices[index];
        sortedFaceIndices[index + 1] = indices[index + 1];
      }
    }
    int[] array = new int[indices.Length / 2];
    for (int index = 0; index < indices.Length; index += 2)
      array[index / 2] = index;
    Array.Sort<int>(array, (Comparison<int>) ((a, b) => ColliderBuilder2D.CompareDuplicateFaces(sortedFaceIndices, a, b)));
    List<int> intList = new List<int>();
    for (int index1 = 0; index1 < array.Length; ++index1)
    {
      if (index1 != array.Length - 1 && ColliderBuilder2D.CompareDuplicateFaces(sortedFaceIndices, array[index1], array[index1 + 1]) == 0)
      {
        ++index1;
      }
      else
      {
        for (int index2 = 0; index2 < 2; ++index2)
          intList.Add(indices[array[index1] + index2]);
      }
    }
    return intList.ToArray();
  }

  private static List<Vector2[]> MergeEdges(Vector2[] verts, int[] indices)
  {
    List<Vector2[]> vector2ArrayList = new List<Vector2[]>();
    List<Vector2> vector2List = new List<Vector2>();
    List<int> intList = new List<int>();
    Vector2 zero1 = Vector2.zero;
    Vector2 zero2 = Vector2.zero;
    bool[] flagArray = new bool[indices.Length / 2];
    bool flag = true;
    while (flag)
    {
      flag = false;
      for (int index1 = 0; index1 < flagArray.Length; ++index1)
      {
        if (!flagArray[index1])
        {
          flagArray[index1] = true;
          int index2 = indices[index1 * 2];
          int index3 = indices[index1 * 2 + 1];
          Vector2 rhs = (verts[index3] - verts[index2]).normalized;
          intList.Add(index2);
          intList.Add(index3);
          for (int index4 = index1 + 1; index4 < flagArray.Length; ++index4)
          {
            if (!flagArray[index4])
            {
              int index5 = indices[index4 * 2];
              if (index5 == index3)
              {
                int index6 = indices[index4 * 2 + 1];
                Vector2 normalized = (verts[index6] - verts[index5]).normalized;
                if ((double) Vector2.Dot(normalized, rhs) > 0.99900001287460327)
                  intList.RemoveAt(intList.Count - 1);
                intList.Add(index6);
                flagArray[index4] = true;
                rhs = normalized;
                index4 = index1;
                index3 = index6;
              }
            }
          }
          flag = true;
          break;
        }
      }
      if (flag)
      {
        vector2List.Clear();
        vector2List.Capacity = Mathf.Max(vector2List.Capacity, intList.Count);
        for (int index = 0; index < intList.Count; ++index)
          vector2List.Add(verts[intList[index]]);
        vector2ArrayList.Add(vector2List.ToArray());
        intList.Clear();
      }
    }
    return vector2ArrayList;
  }
}
