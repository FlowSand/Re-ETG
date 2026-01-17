// Decompiled with JetBrains decompiler
// Type: tk2dRuntime.TileMap.RenderMeshBuilder
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable
namespace tk2dRuntime.TileMap;

public static class RenderMeshBuilder
{
  public static int CurrentCellXOffset;
  public static int CurrentCellYOffset;

  public static void BuildForChunk(
    tk2dTileMap tileMap,
    SpriteChunk chunk,
    bool useColor,
    bool skipPrefabs,
    int baseX,
    int baseY,
    LayerInfo layerData)
  {
    Dungeon dungeon = GameManager.Instance.Dungeon;
    List<Vector3> vector3List = new List<Vector3>();
    List<Color> colorList = new List<Color>();
    List<Vector2> vector2List = new List<Vector2>();
    if (layerData.preprocessedFlags == null || layerData.preprocessedFlags.Length == 0)
      layerData.preprocessedFlags = new bool[tileMap.width * tileMap.height];
    int[] spriteIds = chunk.spriteIds;
    Vector3 tileSize = tileMap.data.tileSize;
    int length = tileMap.SpriteCollectionInst.spriteDefinitions.Length;
    Object[] tilePrefabs = (Object[]) tileMap.data.tilePrefabs;
    tk2dSpriteDefinition firstValidDefinition = tileMap.SpriteCollectionInst.FirstValidDefinition;
    bool flag1 = firstValidDefinition != null && firstValidDefinition.normals != null && firstValidDefinition.normals.Length > 0;
    Color32 color32 = (Color32) (!useColor || tileMap.ColorChannel == null ? Color.white : tileMap.ColorChannel.clearColor);
    int x0;
    int x1;
    int dx;
    int y0;
    int y1;
    int dy;
    BuilderUtil.GetLoopOrder(tileMap.data.sortMethod, chunk.Width, chunk.Height, out x0, out x1, out dx, out y0, out y1, out dy);
    float x2 = 0.0f;
    float y2 = 0.0f;
    tileMap.data.GetTileOffset(out x2, out y2);
    List<int>[] intListArray = new List<int>[tileMap.SpriteCollectionInst.materials.Length];
    for (int index = 0; index < intListArray.Length; ++index)
      intListArray[index] = new List<int>();
    IntVector2 intVector2_1 = new IntVector2(layerData.overrideChunkXOffset, layerData.overrideChunkYOffset);
    int num1 = tileMap.partitionSizeX + 1;
    for (int y3 = y0; y3 != y1; y3 += dy)
    {
      float num2 = (float) (baseY + y3 & 1) * x2;
      for (int x3 = x0; x3 != x1; x3 += dx)
      {
        Vector3 vector = new Vector3(tileSize.x * ((float) x3 + num2), tileSize.y * (float) y3, 0.0f);
        IntVector2 intVector2_2 = IntVector2.Zero;
        if (tileMap.isGungeonTilemap)
        {
          intVector2_2 = vector.IntXY() + new IntVector2(baseX, baseY);
          if ((chunk.roomReference == null || chunk.roomReference.ContainsPosition(intVector2_2 - intVector2_1)) && intVector2_2.y * tileMap.width + intVector2_2.x < layerData.preprocessedFlags.Length && !layerData.preprocessedFlags[intVector2_2.y * tileMap.width + intVector2_2.x])
            layerData.preprocessedFlags[intVector2_2.y * tileMap.width + intVector2_2.x] = true;
          else
            continue;
        }
        int rawTile = spriteIds[y3 * chunk.Width + x3];
        int tileFromRawTile = BuilderUtil.GetTileFromRawTile(rawTile);
        bool flipH = BuilderUtil.IsRawTileFlagSet(rawTile, tk2dTileFlags.FlipX);
        bool flipV = BuilderUtil.IsRawTileFlagSet(rawTile, tk2dTileFlags.FlipY);
        bool rot90 = BuilderUtil.IsRawTileFlagSet(rawTile, tk2dTileFlags.Rot90);
        ColorChunk colorChunk = !tileMap.isGungeonTilemap ? tileMap.ColorChannel.GetChunk(Mathf.FloorToInt((float) baseX / (float) tileMap.partitionSizeX), Mathf.FloorToInt((float) baseY / (float) tileMap.partitionSizeY)) : tileMap.ColorChannel.GetChunk(Mathf.FloorToInt((float) intVector2_2.x / (float) tileMap.partitionSizeX), Mathf.FloorToInt((float) intVector2_2.y / (float) tileMap.partitionSizeY));
        bool flag2 = useColor;
        if (colorChunk == null || colorChunk.colors.Length == 0 && colorChunk.colorOverrides.GetLength(0) == 0)
          flag2 = false;
        if (tileFromRawTile >= 0 && tileFromRawTile < length && (!skipPrefabs || !(bool) tilePrefabs[tileFromRawTile]))
        {
          tk2dSpriteDefinition spriteDefinition = tileMap.SpriteCollectionInst.spriteDefinitions[tileFromRawTile];
          if (layerData.ForceNonAnimating || !spriteDefinition.metadata.usesAnimSequence)
          {
            int count = vector3List.Count;
            Vector3[] vector3Array = spriteDefinition.ConstructExpensivePositions();
            for (int index1 = 0; index1 < vector3Array.Length; ++index1)
            {
              Vector3 vector3_1 = BuilderUtil.ApplySpriteVertexTileFlags(tileMap, spriteDefinition, vector3Array[index1], flipH, flipV, rot90);
              if (flag2)
              {
                IntVector2 intVector2_3 = new IntVector2(x3, y3);
                if (tileMap.isGungeonTilemap)
                  intVector2_3 = new IntVector2(intVector2_2.x % tileMap.partitionSizeX, intVector2_2.y % tileMap.partitionSizeY);
                int index2 = index1 % 4;
                Color32 colorOverride = colorChunk.colorOverrides[intVector2_3.y * num1 + intVector2_3.x, index2];
                if (tileMap.isGungeonTilemap && ((int) colorOverride.r != (int) color32.r || (int) colorOverride.g != (int) color32.g || (int) colorOverride.b != (int) color32.b || (int) colorOverride.a != (int) color32.a))
                {
                  Color color = (Color) colorOverride;
                  colorList.Add(color);
                }
                else
                {
                  Color color1 = (Color) colorChunk.colors[intVector2_3.y * num1 + intVector2_3.x];
                  Color color2 = (Color) colorChunk.colors[intVector2_3.y * num1 + intVector2_3.x + 1];
                  Color color3 = (Color) colorChunk.colors[(intVector2_3.y + 1) * num1 + intVector2_3.x];
                  Color color4 = (Color) colorChunk.colors[(intVector2_3.y + 1) * num1 + (intVector2_3.x + 1)];
                  Vector3 vector3_2 = vector3_1 - spriteDefinition.untrimmedBoundsDataCenter + tileMap.data.tileSize * 0.5f;
                  float t1 = Mathf.Clamp01(vector3_2.x / tileMap.data.tileSize.x);
                  float t2 = Mathf.Clamp01(vector3_2.y / tileMap.data.tileSize.y);
                  Color color5 = Color.Lerp(Color.Lerp(color1, color2, t1), Color.Lerp(color3, color4, t1), t2);
                  colorList.Add(color5);
                }
              }
              else
                colorList.Add(Color.black);
              Vector3 vector3_3 = vector;
              Vector3 vector3_4;
              if (tileMap.isGungeonTilemap)
              {
                IntVector2 intVector2_4 = vector.IntXY() + new IntVector2(baseX + RenderMeshBuilder.CurrentCellXOffset, baseY + RenderMeshBuilder.CurrentCellYOffset);
                if (dungeon.data.CheckInBounds(intVector2_4, 1) && dungeon.data.isAnyFaceWall(intVector2_4.x, intVector2_4.y))
                {
                  Vector3 vector3_5 = !dungeon.data.isFaceWallHigher(intVector2_4.x, intVector2_4.y) ? new Vector3(0.0f, 0.0f, 1f) : new Vector3(0.0f, 0.0f, -1f);
                  CellData cellData = dungeon.data[intVector2_4];
                  if (cellData.diagonalWallType == DiagonalWallType.NORTHEAST)
                    vector3_5.z += (float) ((1.0 - (double) vector3_1.x) * 2.0);
                  else if (cellData.diagonalWallType == DiagonalWallType.NORTHWEST)
                    vector3_5.z += vector3_1.x * 2f;
                  vector3_4 = vector3_3 + (new Vector3(0.0f, 0.0f, vector.y - vector3_1.y) + vector3_1 + vector3_5);
                }
                else if (dungeon.data.CheckInBounds(intVector2_4, 1) && dungeon.data.isTopDiagonalWall(intVector2_4.x, intVector2_4.y) && layerData.name == "Collision Layer")
                {
                  Vector3 vector3_6 = new Vector3(0.0f, 0.0f, -3f);
                  vector3_4 = vector3_3 + (new Vector3(0.0f, 0.0f, vector.y + vector3_1.y) + vector3_1 + vector3_6);
                }
                else if (layerData.name == "AOandShadows")
                {
                  if (dungeon.data.CheckInBounds(intVector2_4, 1) && dungeon.data[intVector2_4] != null && dungeon.data[intVector2_4].type == CellType.PIT)
                  {
                    Vector3 vector3_7 = new Vector3(0.0f, 0.0f, 2.5f);
                    vector3_4 = vector3_3 + (new Vector3(0.0f, 0.0f, vector.y + vector3_1.y) + vector3_1 + vector3_7);
                  }
                  else
                  {
                    Vector3 vector3_8 = new Vector3(0.0f, 0.0f, 1f);
                    vector3_4 = vector3_3 + (new Vector3(0.0f, 0.0f, vector.y + vector3_1.y) + vector3_1 + vector3_8);
                  }
                }
                else if (layerData.name == "Pit Layer")
                {
                  Vector3 vector3_9 = new Vector3(0.0f, 0.0f, 2f);
                  if (dungeon.data.CheckInBounds(intVector2_4.x, intVector2_4.y + 2))
                  {
                    if (dungeon.data.cellData[intVector2_4.x][intVector2_4.y + 1].type != CellType.PIT || dungeon.data.cellData[intVector2_4.x][intVector2_4.y + 2].type != CellType.PIT)
                    {
                      bool flag3 = dungeon.data.cellData[intVector2_4.x][intVector2_4.y + 1].type != CellType.PIT;
                      if (dungeon.debugSettings.WALLS_ARE_PITS && dungeon.data.cellData[intVector2_4.x][intVector2_4.y + 1].isExitCell)
                        flag3 = false;
                      if (flag3)
                        vector3_9 = new Vector3(0.0f, 0.0f, 0.0f);
                      vector3_4 = vector3_3 + (new Vector3(0.0f, 0.0f, vector.y - vector3_1.y) + vector3_1 + vector3_9);
                    }
                    else
                      vector3_4 = vector3_3 + (new Vector3(0.0f, 0.0f, (float) ((double) vector.y + (double) vector3_1.y + 1.0)) + vector3_1);
                  }
                  else
                    vector3_4 = vector3_3 + (new Vector3(0.0f, 0.0f, (float) ((double) vector.y + (double) vector3_1.y + 1.0)) + vector3_1);
                }
                else
                  vector3_4 = vector3_3 + (new Vector3(0.0f, 0.0f, vector.y + vector3_1.y) + vector3_1);
              }
              else
                vector3_4 = vector3_3 + vector3_1;
              vector3List.Add(vector3_4);
              vector2List.Add(spriteDefinition.uvs[index1]);
            }
            bool flag4 = false;
            if (flipH)
              flag4 = !flag4;
            if (flipV)
              flag4 = !flag4;
            List<int> intList = intListArray[spriteDefinition.materialId];
            for (int index3 = 0; index3 < spriteDefinition.indices.Length; ++index3)
            {
              int index4 = !flag4 ? index3 : spriteDefinition.indices.Length - 1 - index3;
              intList.Add(count + spriteDefinition.indices[index4]);
            }
          }
        }
      }
    }
    if ((Object) chunk.mesh == (Object) null)
      chunk.mesh = tk2dUtil.CreateMesh();
    chunk.mesh.vertices = vector3List.ToArray();
    chunk.mesh.uv = vector2List.ToArray();
    chunk.mesh.colors = colorList.ToArray();
    List<Material> materialList = new List<Material>();
    int index5 = 0;
    int num3 = 0;
    foreach (List<int> intList in intListArray)
    {
      if (intList.Count > 0)
      {
        materialList.Add(tileMap.SpriteCollectionInst.materialInsts[index5]);
        ++num3;
      }
      ++index5;
    }
    if (num3 > 0)
    {
      chunk.mesh.subMeshCount = num3;
      chunk.gameObject.GetComponent<Renderer>().materials = materialList.ToArray();
      int submesh = 0;
      foreach (List<int> intList in intListArray)
      {
        if (intList.Count > 0)
        {
          chunk.mesh.SetTriangles(intList.ToArray(), submesh);
          ++submesh;
        }
      }
    }
    chunk.mesh.RecalculateBounds();
    if (flag1)
      chunk.mesh.RecalculateNormals();
    if (tileMap.isGungeonTilemap)
      chunk.gameObject.transform.position = chunk.gameObject.transform.position.WithZ((float) baseY + chunk.gameObject.transform.position.z);
    chunk.gameObject.GetComponent<MeshFilter>().sharedMesh = chunk.mesh;
  }

  [DebuggerHidden]
  public static IEnumerator Build(tk2dTileMap tileMap, bool editMode, bool forceBuild)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new RenderMeshBuilder__Buildc__Iterator0()
    {
      editMode = editMode,
      forceBuild = forceBuild,
      tileMap = tileMap
    };
  }
}
