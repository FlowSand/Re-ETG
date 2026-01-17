// Decompiled with JetBrains decompiler
// Type: SecretRoomBuilder
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections.Generic;
using tk2dRuntime.TileMap;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class SecretRoomBuilder
    {
      private const float CEILING_HEIGHT_OFFSET = -3.01f;
      private const float BORDER_HEIGHT_OFFSET = -3.02f;

      private static HashSet<IntVector2> GetRoomCeilingCells(RoomHandler room)
      {
        List<IntVector2> representationIncFacewalls = room.area.prototypeRoom.GetCellRepresentationIncFacewalls();
        HashSet<IntVector2> roomCeilingCells = new HashSet<IntVector2>();
        List<IntVector2> intVector2List = new List<IntVector2>((IEnumerable<IntVector2>) IntVector2.CardinalsAndOrdinals);
        foreach (IntVector2 intVector2_1 in representationIncFacewalls)
        {
          roomCeilingCells.Add(room.area.basePosition + intVector2_1);
          foreach (IntVector2 intVector2_2 in intVector2List)
            roomCeilingCells.Add(room.area.basePosition + intVector2_1 + intVector2_2);
        }
        intVector2List.Add(IntVector2.Up * 2);
        intVector2List.Add(IntVector2.Up * 3);
        intVector2List.Add(IntVector2.Up * 2 + IntVector2.Right);
        intVector2List.Add(IntVector2.Up * 3 + IntVector2.Right);
        intVector2List.Add(IntVector2.Up * 2 + IntVector2.Left);
        intVector2List.Add(IntVector2.Up * 3 + IntVector2.Left);
        foreach (PrototypeRoomExit instanceUsedExit in room.area.instanceUsedExits)
        {
          RuntimeExitDefinition runtimeExitDefinition = room.exitDefinitionsByExit[room.area.exitToLocalDataMap[instanceUsedExit]];
          if (!room.area.exitToLocalDataMap[instanceUsedExit].oneWayDoor)
          {
            DungeonData.Direction dir = runtimeExitDefinition.upstreamRoom != room ? runtimeExitDefinition.downstreamExit.referencedExit.exitDirection : runtimeExitDefinition.upstreamExit.referencedExit.exitDirection;
            IntVector2 vector2FromDirection = DungeonData.GetIntVector2FromDirection(dir);
            HashSet<IntVector2> cellsForRoom = runtimeExitDefinition.GetCellsForRoom(room);
            bool flag = !Dungeon.IsGenerating && runtimeExitDefinition.upstreamRoom.area.prototypeRoom.category == PrototypeDungeonRoom.RoomCategory.SECRET && runtimeExitDefinition.downstreamRoom.area.prototypeRoom.category == PrototypeDungeonRoom.RoomCategory.SECRET;
            if (flag)
            {
              int a = int.MaxValue;
              foreach (IntVector2 intVector2 in cellsForRoom)
                a = Mathf.Min(a, intVector2.y);
              foreach (IntVector2 intVector2 in runtimeExitDefinition.GetCellsForOtherRoom(room))
              {
                if (a - intVector2.y <= 4)
                  cellsForRoom.Add(intVector2);
              }
            }
            foreach (IntVector2 intVector2_3 in cellsForRoom)
            {
              roomCeilingCells.Add(intVector2_3);
              foreach (IntVector2 intVector2_4 in intVector2List)
              {
                if (intVector2_4.x != 0 && intVector2_4.x == vector2FromDirection.x || intVector2_4.y != 0 && intVector2_4.y == vector2FromDirection.y)
                {
                  if (flag)
                  {
                    if (room == runtimeExitDefinition.upstreamRoom)
                      BraveUtility.DrawDebugSquare(intVector2_3.ToVector2() + intVector2_4.ToVector2(), Color.yellow, 1000f);
                    else if (room == runtimeExitDefinition.downstreamRoom)
                      BraveUtility.DrawDebugSquare(intVector2_3.ToVector2() + intVector2_4.ToVector2() + new Vector2(0.1f, 0.1f), intVector2_3.ToVector2() + intVector2_4.ToVector2() + new Vector2(0.9f, 0.9f), Color.cyan, 1000f);
                  }
                }
                else
                  roomCeilingCells.Add(intVector2_3 + intVector2_4);
              }
            }
            if (dir != DungeonData.Direction.SOUTH)
            {
              RoomHandler r = runtimeExitDefinition.upstreamRoom != room ? runtimeExitDefinition.upstreamRoom : runtimeExitDefinition.downstreamRoom;
              foreach (IntVector2 intVector2_5 in runtimeExitDefinition.GetCellsForRoom(r))
              {
                roomCeilingCells.Add(intVector2_5);
                foreach (IntVector2 intVector2_6 in intVector2List)
                {
                  if ((intVector2_6.x == 0 || intVector2_6.x != vector2FromDirection.x) && (intVector2_6.y == 0 || (double) Mathf.Sign((float) intVector2_6.y) != (double) vector2FromDirection.y))
                    roomCeilingCells.Add(intVector2_5 + intVector2_6);
                }
              }
            }
          }
        }
        return roomCeilingCells;
      }

      private static bool IsFaceWallHigher(int x, int y, DungeonData data, HashSet<IntVector2> cells)
      {
        return !cells.Contains(new IntVector2(x, y)) && (data.cellData[x][y].type == CellType.WALL || data.cellData[x][y].isSecretRoomCell) && data.cellData[x][y - 2].type != CellType.WALL && !data.cellData[x][y - 2].isSecretRoomCell;
      }

      private static bool IsTopWall(int x, int y, DungeonData data, HashSet<IntVector2> cells)
      {
        return data.cellData[x][y].type != CellType.WALL && (data.cellData[x][y - 1].type == CellType.WALL || cells.Contains(new IntVector2(x, y - 1))) && !cells.Contains(new IntVector2(x, y + 1));
      }

      private static bool IsWall(int x, int y, DungeonData data, HashSet<IntVector2> cells)
      {
        return cells.Contains(new IntVector2(x, y)) || data[x, y].type == CellType.WALL;
      }

      private static bool IsTopWallOrSecret(int x, int y, DungeonData data, HashSet<IntVector2> cells)
      {
        return data[x, y].type != CellType.WALL && !data[x, y].isSecretRoomCell && SecretRoomBuilder.IsWallOrSecret(x, y - 1, data, cells);
      }

      private static bool IsWallOrSecret(int x, int y, DungeonData data, HashSet<IntVector2> cells)
      {
        return data[x, y].type == CellType.WALL || data[x, y].isSecretRoomCell || cells.Contains(new IntVector2(x, y));
      }

      private static bool IsFaceWallHigherOrSecret(
        int x,
        int y,
        DungeonData data,
        HashSet<IntVector2> cells)
      {
        return SecretRoomBuilder.IsFaceWallHigher(x, y, data, cells);
      }

      public static int GetIndexFromTupleArray(
        CellData current,
        List<Tuple<int, TilesetIndexMetadata>> list,
        int roomTypeIndex)
      {
        float num1 = 0.0f;
        for (int index = 0; index < list.Count; ++index)
        {
          Tuple<int, TilesetIndexMetadata> tuple = list[index];
          if (!tuple.Second.usesAnimSequence && (tuple.Second.dungeonRoomSubType == roomTypeIndex || tuple.Second.secondRoomSubType == roomTypeIndex || tuple.Second.thirdRoomSubType == roomTypeIndex))
            num1 += tuple.Second.weight;
        }
        float num2 = current.UniqueHash * num1;
        for (int index = 0; index < list.Count; ++index)
        {
          Tuple<int, TilesetIndexMetadata> tuple = list[index];
          if (!tuple.Second.usesAnimSequence && (tuple.Second.dungeonRoomSubType == roomTypeIndex || tuple.Second.secondRoomSubType == roomTypeIndex || tuple.Second.thirdRoomSubType == roomTypeIndex))
          {
            num2 -= tuple.Second.weight;
            if ((double) num2 <= 0.0)
              return tuple.First;
          }
        }
        return list[0].First;
      }

      private static TileIndexGrid GetBorderGridForCellPosition(IntVector2 position, DungeonData data)
      {
        TileIndexGrid ceilingBorderGrid = GameManager.Instance.Dungeon.roomMaterialDefinitions[data.cellData[position.x][position.y].cellVisualData.roomVisualTypeIndex].roomCeilingBorderGrid;
        if ((Object) ceilingBorderGrid == (Object) null)
          ceilingBorderGrid = GameManager.Instance.Dungeon.roomMaterialDefinitions[0].roomCeilingBorderGrid;
        return ceilingBorderGrid;
      }

      private static void AddCeilingTileAtPosition(
        IntVector2 position,
        TileIndexGrid indexGrid,
        List<Vector3> verts,
        List<int> tris,
        List<Vector2> uvs,
        List<Color> colors,
        out Material ceilingMaterial,
        tk2dSpriteCollectionData spriteData)
      {
        int tileFromRawTile = BuilderUtil.GetTileFromRawTile(indexGrid.centerIndices.GetIndexByWeight());
        tk2dSpriteDefinition spriteDefinition = spriteData.spriteDefinitions[tileFromRawTile];
        ceilingMaterial = spriteDefinition.material;
        int count = verts.Count;
        Vector3 vector3_1 = position.ToVector3((float) position.y - 2.4f);
        Vector3[] vector3Array = spriteDefinition.ConstructExpensivePositions();
        for (int index = 0; index < vector3Array.Length; ++index)
        {
          Vector3 vector3_2 = vector3Array[index].WithZ(vector3Array[index].y);
          verts.Add(vector3_1 + vector3_2);
          uvs.Add(spriteDefinition.uvs[index]);
          colors.Add(Color.black);
        }
        for (int index = 0; index < spriteDefinition.indices.Length; ++index)
          tris.Add(count + spriteDefinition.indices[index]);
      }

      private static void AddTileAtPosition(
        IntVector2 position,
        int index,
        List<Vector3> verts,
        List<int> tris,
        List<Vector2> uvs,
        List<Color> colors,
        out Material targetMaterial,
        tk2dSpriteCollectionData spriteData,
        float zOffset,
        bool tilted,
        Color topColor,
        Color bottomColor)
      {
        int tileFromRawTile = BuilderUtil.GetTileFromRawTile(index);
        tk2dSpriteDefinition spriteDefinition = spriteData.spriteDefinitions[tileFromRawTile];
        targetMaterial = spriteDefinition.material;
        int count = verts.Count;
        Vector3 vector3_1 = position.ToVector3((float) position.y + zOffset);
        Vector3[] vector3Array = spriteDefinition.ConstructExpensivePositions();
        for (int index1 = 0; index1 < vector3Array.Length; ++index1)
        {
          Vector3 vector3_2 = !tilted ? vector3Array[index1].WithZ(vector3Array[index1].y) : vector3Array[index1].WithZ(-vector3Array[index1].y);
          verts.Add(vector3_1 + vector3_2);
          uvs.Add(spriteDefinition.uvs[index1]);
        }
        colors.Add(bottomColor);
        colors.Add(bottomColor);
        colors.Add(topColor);
        colors.Add(topColor);
        for (int index2 = 0; index2 < spriteDefinition.indices.Length; ++index2)
          tris.Add(count + spriteDefinition.indices[index2]);
      }

      private static void AddTileAtPosition(
        IntVector2 position,
        int index,
        List<Vector3> verts,
        List<int> tris,
        List<Vector2> uvs,
        List<Color> colors,
        ref Material targetMaterial,
        tk2dSpriteCollectionData spriteData,
        float zOffset,
        bool tilted = false)
      {
        int tileFromRawTile = BuilderUtil.GetTileFromRawTile(index);
        if (tileFromRawTile < 0 || tileFromRawTile >= spriteData.spriteDefinitions.Length)
        {
          Debug.Log((object) $"{tileFromRawTile.ToString()} index is out of bounds in SecretRoomBuilder, of indices: {spriteData.spriteDefinitions.Length.ToString()}");
        }
        else
        {
          tk2dSpriteDefinition spriteDefinition = spriteData.spriteDefinitions[tileFromRawTile];
          targetMaterial = spriteDefinition.material;
          int count = verts.Count;
          Vector3 vector3_1 = position.ToVector3((float) position.y + zOffset);
          Vector3[] vector3Array = spriteDefinition.ConstructExpensivePositions();
          for (int index1 = 0; index1 < vector3Array.Length; ++index1)
          {
            Vector3 vector3_2 = !tilted ? vector3Array[index1].WithZ(vector3Array[index1].y) : vector3Array[index1].WithZ(-vector3Array[index1].y);
            verts.Add(vector3_1 + vector3_2);
            uvs.Add(spriteDefinition.uvs[index1]);
            colors.Add(Color.black);
          }
          for (int index2 = 0; index2 < spriteDefinition.indices.Length; ++index2)
            tris.Add(count + spriteDefinition.indices[index2]);
        }
      }

      private static GameObject GenerateRoomDoorMesh(
        RuntimeExitDefinition exit,
        RoomHandler room,
        DungeonData dungeonData)
      {
        return SecretRoomBuilder.GenerateWallMesh(exit.GetDirectionFromRoom(room), exit.upstreamRoom != room ? exit.GetUpstreamBasePosition() : exit.GetDownstreamBasePosition(), dungeonData: dungeonData);
      }

      public static GameObject GenerateWallMesh(
        DungeonData.Direction exitDirection,
        IntVector2 exitBasePosition,
        string objectName = "secret room door object",
        DungeonData dungeonData = null,
        bool abridged = false)
      {
        if (dungeonData == null)
          dungeonData = GameManager.Instance.Dungeon.data;
        Mesh mesh = new Mesh();
        List<Vector3> verts = new List<Vector3>();
        List<int> tris1 = new List<int>();
        List<int> tris2 = new List<int>();
        List<int> tris3 = new List<int>();
        List<int> tris4 = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        List<Color> colors = new List<Color>();
        Material ceilingMaterial = (Material) null;
        Material targetMaterial1 = (Material) null;
        Material targetMaterial2 = (Material) null;
        Material targetMaterial3 = (Material) null;
        tk2dSpriteCollectionData dungeonCollection = GameManager.Instance.Dungeon.tileIndices.dungeonCollection;
        TileIndexGrid gridForCellPosition = SecretRoomBuilder.GetBorderGridForCellPosition(exitBasePosition, dungeonData);
        CellData current = dungeonData[exitBasePosition];
        switch (exitDirection)
        {
          case DungeonData.Direction.NORTH:
            SecretRoomBuilder.AddCeilingTileAtPosition(exitBasePosition, gridForCellPosition, verts, tris1, uvs, colors, out ceilingMaterial, dungeonCollection);
            SecretRoomBuilder.AddCeilingTileAtPosition(exitBasePosition + IntVector2.Right, gridForCellPosition, verts, tris1, uvs, colors, out ceilingMaterial, dungeonCollection);
            SecretRoomBuilder.AddTileAtPosition(exitBasePosition, gridForCellPosition.leftCapIndices.GetIndexByWeight(), verts, tris2, uvs, colors, ref targetMaterial1, dungeonCollection, -2.45f);
            SecretRoomBuilder.AddTileAtPosition(exitBasePosition + IntVector2.Right, gridForCellPosition.rightCapIndices.GetIndexByWeight(), verts, tris2, uvs, colors, ref targetMaterial1, dungeonCollection, -2.45f);
            int indexFromTupleArray1 = SecretRoomBuilder.GetIndexFromTupleArray(current, SecretRoomUtility.metadataLookupTableRef[TilesetIndexMetadata.TilesetFlagType.FACEWALL_UPPER], current.cellVisualData.roomVisualTypeIndex);
            SecretRoomBuilder.AddTileAtPosition(exitBasePosition + IntVector2.Down, indexFromTupleArray1, verts, tris3, uvs, colors, out targetMaterial2, dungeonCollection, -0.4f, true, new Color(0.0f, 1f, 1f), new Color(0.0f, 0.5f, 1f));
            int indexFromTupleArray2 = SecretRoomBuilder.GetIndexFromTupleArray(current, SecretRoomUtility.metadataLookupTableRef[TilesetIndexMetadata.TilesetFlagType.FACEWALL_UPPER], current.cellVisualData.roomVisualTypeIndex);
            SecretRoomBuilder.AddTileAtPosition(exitBasePosition + IntVector2.Down + IntVector2.Right, indexFromTupleArray2, verts, tris3, uvs, colors, out targetMaterial2, dungeonCollection, -0.4f, true, new Color(0.0f, 1f, 1f), new Color(0.0f, 0.5f, 1f));
            int indexFromTupleArray3 = SecretRoomBuilder.GetIndexFromTupleArray(current, SecretRoomUtility.metadataLookupTableRef[TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER], current.cellVisualData.roomVisualTypeIndex);
            SecretRoomBuilder.AddTileAtPosition(exitBasePosition + IntVector2.Down * 2, indexFromTupleArray3, verts, tris3, uvs, colors, out targetMaterial2, dungeonCollection, 1.6f, true, new Color(0.0f, 0.5f, 1f), new Color(0.0f, 0.0f, 1f));
            int indexFromTupleArray4 = SecretRoomBuilder.GetIndexFromTupleArray(current, SecretRoomUtility.metadataLookupTableRef[TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER], current.cellVisualData.roomVisualTypeIndex);
            SecretRoomBuilder.AddTileAtPosition(exitBasePosition + IntVector2.Down * 2 + IntVector2.Right, indexFromTupleArray4, verts, tris3, uvs, colors, out targetMaterial2, dungeonCollection, 1.6f, true, new Color(0.0f, 0.5f, 1f), new Color(0.0f, 0.0f, 1f));
            break;
          case DungeonData.Direction.EAST:
            if (!abridged)
              SecretRoomBuilder.AddCeilingTileAtPosition(exitBasePosition + IntVector2.Down, gridForCellPosition, verts, tris1, uvs, colors, out ceilingMaterial, dungeonCollection);
            if (!abridged)
              SecretRoomBuilder.AddCeilingTileAtPosition(exitBasePosition + IntVector2.Zero, gridForCellPosition, verts, tris1, uvs, colors, out ceilingMaterial, dungeonCollection);
            SecretRoomBuilder.AddCeilingTileAtPosition(exitBasePosition + IntVector2.Up, gridForCellPosition, verts, tris1, uvs, colors, out ceilingMaterial, dungeonCollection);
            SecretRoomBuilder.AddCeilingTileAtPosition(exitBasePosition + IntVector2.Up * 2, gridForCellPosition, verts, tris1, uvs, colors, out ceilingMaterial, dungeonCollection);
            if (!abridged)
              SecretRoomBuilder.AddCeilingTileAtPosition(exitBasePosition + IntVector2.Up * 3, gridForCellPosition, verts, tris1, uvs, colors, out ceilingMaterial, dungeonCollection);
            SecretRoomBuilder.AddTileAtPosition(exitBasePosition + IntVector2.Up, gridForCellPosition.bottomCapIndices.GetIndexByWeight(), verts, tris2, uvs, colors, ref targetMaterial1, dungeonCollection, -2.45f);
            SecretRoomBuilder.AddTileAtPosition(exitBasePosition + IntVector2.Up * 2, gridForCellPosition.verticalIndices.GetIndexByWeight(), verts, tris2, uvs, colors, ref targetMaterial1, dungeonCollection, -2.45f);
            if (!abridged)
              SecretRoomBuilder.AddTileAtPosition(exitBasePosition + IntVector2.Up * 3, gridForCellPosition.topCapIndices.GetIndexByWeight(), verts, tris2, uvs, colors, ref targetMaterial1, dungeonCollection, -2.45f);
            Color color1 = new Color(0.0f, 0.0f, 1f, 0.0f);
            SecretRoomBuilder.AddTileAtPosition(exitBasePosition + IntVector2.Down + IntVector2.Right, GameManager.Instance.Dungeon.tileIndices.aoTileIndices.AOFloorWallLeft, verts, tris4, uvs, colors, out targetMaterial3, dungeonCollection, 1.55f, true, color1, color1);
            SecretRoomBuilder.AddTileAtPosition(exitBasePosition + IntVector2.Right, GameManager.Instance.Dungeon.tileIndices.aoTileIndices.AOFloorWallLeft, verts, tris4, uvs, colors, out targetMaterial3, dungeonCollection, 1.55f, true, color1, color1);
            if (!abridged)
            {
              SecretRoomBuilder.AddTileAtPosition(exitBasePosition + IntVector2.Up + IntVector2.Right, GameManager.Instance.Dungeon.tileIndices.aoTileIndices.AOFloorWallLeft, verts, tris4, uvs, colors, out targetMaterial3, dungeonCollection, 1.55f, true, color1, color1);
              break;
            }
            break;
          case DungeonData.Direction.SOUTH:
            SecretRoomBuilder.AddCeilingTileAtPosition(exitBasePosition + IntVector2.Up * 2, gridForCellPosition, verts, tris1, uvs, colors, out ceilingMaterial, dungeonCollection);
            SecretRoomBuilder.AddCeilingTileAtPosition(exitBasePosition + IntVector2.Up * 2 + IntVector2.Right, gridForCellPosition, verts, tris1, uvs, colors, out ceilingMaterial, dungeonCollection);
            SecretRoomBuilder.AddTileAtPosition(exitBasePosition + IntVector2.Up * 2, gridForCellPosition.leftCapIndices.GetIndexByWeight(), verts, tris2, uvs, colors, ref targetMaterial1, dungeonCollection, -2.45f);
            SecretRoomBuilder.AddTileAtPosition(exitBasePosition + IntVector2.Up * 2 + IntVector2.Right, gridForCellPosition.rightCapIndices.GetIndexByWeight(), verts, tris2, uvs, colors, ref targetMaterial1, dungeonCollection, -2.45f);
            int indexFromTupleArray5 = SecretRoomBuilder.GetIndexFromTupleArray(current, SecretRoomUtility.metadataLookupTableRef[TilesetIndexMetadata.TilesetFlagType.FACEWALL_UPPER], current.cellVisualData.roomVisualTypeIndex);
            SecretRoomBuilder.AddTileAtPosition(exitBasePosition + IntVector2.Up, indexFromTupleArray5, verts, tris3, uvs, colors, out targetMaterial2, dungeonCollection, -0.4f, true, new Color(0.0f, 1f, 1f), new Color(0.0f, 0.5f, 1f));
            int indexFromTupleArray6 = SecretRoomBuilder.GetIndexFromTupleArray(current, SecretRoomUtility.metadataLookupTableRef[TilesetIndexMetadata.TilesetFlagType.FACEWALL_UPPER], current.cellVisualData.roomVisualTypeIndex);
            SecretRoomBuilder.AddTileAtPosition(exitBasePosition + IntVector2.Up + IntVector2.Right, indexFromTupleArray6, verts, tris3, uvs, colors, out targetMaterial2, dungeonCollection, -0.4f, true, new Color(0.0f, 1f, 1f), new Color(0.0f, 0.5f, 1f));
            int indexFromTupleArray7 = SecretRoomBuilder.GetIndexFromTupleArray(current, SecretRoomUtility.metadataLookupTableRef[TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER], current.cellVisualData.roomVisualTypeIndex);
            SecretRoomBuilder.AddTileAtPosition(exitBasePosition, indexFromTupleArray7, verts, tris3, uvs, colors, out targetMaterial2, dungeonCollection, 1.6f, true, new Color(0.0f, 0.5f, 1f), new Color(0.0f, 0.0f, 1f));
            int indexFromTupleArray8 = SecretRoomBuilder.GetIndexFromTupleArray(current, SecretRoomUtility.metadataLookupTableRef[TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER], current.cellVisualData.roomVisualTypeIndex);
            SecretRoomBuilder.AddTileAtPosition(exitBasePosition + IntVector2.Right, indexFromTupleArray8, verts, tris3, uvs, colors, out targetMaterial2, dungeonCollection, 1.6f, true, new Color(0.0f, 0.5f, 1f), new Color(0.0f, 0.0f, 1f));
            Color color2 = new Color(0.0f, 0.0f, 1f, 0.0f);
            SecretRoomBuilder.AddTileAtPosition(exitBasePosition, GameManager.Instance.Dungeon.tileIndices.aoTileIndices.AOBottomWallBaseTileIndex, verts, tris4, uvs, colors, out targetMaterial3, dungeonCollection, 1.55f, true, color2, color2);
            SecretRoomBuilder.AddTileAtPosition(exitBasePosition + IntVector2.Right, GameManager.Instance.Dungeon.tileIndices.aoTileIndices.AOBottomWallBaseTileIndex, verts, tris4, uvs, colors, out targetMaterial3, dungeonCollection, 1.55f, true, color2, color2);
            SecretRoomBuilder.AddTileAtPosition(exitBasePosition + IntVector2.Down, GameManager.Instance.Dungeon.tileIndices.aoTileIndices.AOFloorTileIndex, verts, tris4, uvs, colors, out targetMaterial3, dungeonCollection, 1.55f, false, color2, color2);
            SecretRoomBuilder.AddTileAtPosition(exitBasePosition + IntVector2.Down + IntVector2.Right, GameManager.Instance.Dungeon.tileIndices.aoTileIndices.AOFloorTileIndex, verts, tris4, uvs, colors, out targetMaterial3, dungeonCollection, 1.55f, false, color2, color2);
            break;
          case DungeonData.Direction.WEST:
            if (!abridged)
              SecretRoomBuilder.AddCeilingTileAtPosition(exitBasePosition + IntVector2.Down, gridForCellPosition, verts, tris1, uvs, colors, out ceilingMaterial, dungeonCollection);
            if (!abridged)
              SecretRoomBuilder.AddCeilingTileAtPosition(exitBasePosition + IntVector2.Zero, gridForCellPosition, verts, tris1, uvs, colors, out ceilingMaterial, dungeonCollection);
            SecretRoomBuilder.AddCeilingTileAtPosition(exitBasePosition + IntVector2.Up, gridForCellPosition, verts, tris1, uvs, colors, out ceilingMaterial, dungeonCollection);
            SecretRoomBuilder.AddCeilingTileAtPosition(exitBasePosition + IntVector2.Up * 2, gridForCellPosition, verts, tris1, uvs, colors, out ceilingMaterial, dungeonCollection);
            if (!abridged)
              SecretRoomBuilder.AddCeilingTileAtPosition(exitBasePosition + IntVector2.Up * 3, gridForCellPosition, verts, tris1, uvs, colors, out ceilingMaterial, dungeonCollection);
            SecretRoomBuilder.AddTileAtPosition(exitBasePosition + IntVector2.Up, gridForCellPosition.bottomCapIndices.GetIndexByWeight(), verts, tris2, uvs, colors, ref targetMaterial1, dungeonCollection, -2.45f);
            SecretRoomBuilder.AddTileAtPosition(exitBasePosition + IntVector2.Up * 2, gridForCellPosition.verticalIndices.GetIndexByWeight(), verts, tris2, uvs, colors, ref targetMaterial1, dungeonCollection, -2.45f);
            if (!abridged)
              SecretRoomBuilder.AddTileAtPosition(exitBasePosition + IntVector2.Up * 3, gridForCellPosition.topCapIndices.GetIndexByWeight(), verts, tris2, uvs, colors, ref targetMaterial1, dungeonCollection, -2.45f);
            Color color3 = new Color(0.0f, 0.0f, 1f, 0.0f);
            SecretRoomBuilder.AddTileAtPosition(exitBasePosition + IntVector2.Down + IntVector2.Left, GameManager.Instance.Dungeon.tileIndices.aoTileIndices.AOFloorWallRight, verts, tris4, uvs, colors, out targetMaterial3, dungeonCollection, 1.55f, true, color3, color3);
            SecretRoomBuilder.AddTileAtPosition(exitBasePosition + IntVector2.Left, GameManager.Instance.Dungeon.tileIndices.aoTileIndices.AOFloorWallRight, verts, tris4, uvs, colors, out targetMaterial3, dungeonCollection, 1.55f, true, color3, color3);
            if (!abridged)
            {
              SecretRoomBuilder.AddTileAtPosition(exitBasePosition + IntVector2.Up + IntVector2.Left, GameManager.Instance.Dungeon.tileIndices.aoTileIndices.AOFloorWallRight, verts, tris4, uvs, colors, out targetMaterial3, dungeonCollection, 1.55f, true, color3, color3);
              break;
            }
            break;
        }
        Vector3 lhs = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        for (int index = 0; index < verts.Count; ++index)
          lhs = Vector3.Min(lhs, verts[index]);
        lhs.x = (float) Mathf.FloorToInt(lhs.x);
        lhs.y = (float) Mathf.FloorToInt(lhs.y);
        lhs.z = (float) Mathf.FloorToInt(lhs.z);
        for (int index = 0; index < verts.Count; ++index)
          verts[index] -= lhs;
        mesh.vertices = verts.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.colors = colors.ToArray();
        mesh.subMeshCount = 4;
        mesh.SetTriangles(tris1.ToArray(), 0);
        mesh.SetTriangles(tris2.ToArray(), 1);
        mesh.SetTriangles(tris3.ToArray(), 2);
        mesh.SetTriangles(tris4.ToArray(), 3);
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        GameObject gameObject = new GameObject(objectName);
        gameObject.SetLayerRecursively(LayerMask.NameToLayer("FG_Critical"));
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        gameObject.transform.position = lhs;
        meshFilter.mesh = mesh;
        meshRenderer.materials = new Material[4]
        {
          ceilingMaterial,
          targetMaterial1,
          targetMaterial2,
          targetMaterial3
        };
        gameObject.SetLayerRecursively(LayerMask.NameToLayer("ShadowCaster"));
        return gameObject;
      }

      public static GameObject GenerateRoomCeilingMesh(
        HashSet<IntVector2> cells,
        string objectName = "secret room ceiling object",
        DungeonData dungeonData = null,
        bool mimicCheck = false)
      {
        if (dungeonData == null)
          dungeonData = GameManager.Instance.Dungeon.data;
        Mesh mesh = new Mesh();
        List<Vector3> vector3List = new List<Vector3>();
        List<int> intList1 = new List<int>();
        List<int> intList2 = new List<int>();
        List<Vector2> vector2List = new List<Vector2>();
        Material material1 = (Material) null;
        Material material2 = (Material) null;
        tk2dSpriteCollectionData dungeonCollection = GameManager.Instance.Dungeon.tileIndices.dungeonCollection;
        Vector3 vector3_1 = new Vector3(0.0f, 0.0f, -3.01f);
        Vector3 vector3_2 = new Vector3(0.0f, 0.0f, -3.02f);
        foreach (IntVector2 cell in cells)
        {
          TileIndexGrid gridForCellPosition = SecretRoomBuilder.GetBorderGridForCellPosition(cell, dungeonData);
          int tileFromRawTile1 = BuilderUtil.GetTileFromRawTile(gridForCellPosition.centerIndices.GetIndexByWeight());
          tk2dSpriteDefinition spriteDefinition1 = dungeonCollection.spriteDefinitions[tileFromRawTile1];
          if ((Object) material1 == (Object) null)
            material1 = spriteDefinition1.material;
          int count1 = vector3List.Count;
          Vector3 vector3_3 = cell.ToVector3((float) cell.y);
          Vector3[] vector3Array1 = spriteDefinition1.ConstructExpensivePositions();
          for (int index = 0; index < vector3Array1.Length; ++index)
          {
            Vector3 vector3_4 = vector3Array1[index].WithZ(vector3Array1[index].y);
            vector3List.Add(vector3_3 + vector3_4 + vector3_1);
            vector2List.Add(spriteDefinition1.uvs[index]);
          }
          for (int index = 0; index < spriteDefinition1.indices.Length; ++index)
            intList1.Add(count1 + spriteDefinition1.indices[index]);
          int x = cell.x;
          int y = cell.y;
          bool isNorthBorder = SecretRoomBuilder.IsTopWall(x, y, dungeonData, cells);
          bool isNortheastBorder = SecretRoomBuilder.IsTopWall(x + 1, y, dungeonData, cells) && !SecretRoomBuilder.IsTopWall(x, y, dungeonData, cells) && (SecretRoomBuilder.IsWall(x, y + 1, dungeonData, cells) || SecretRoomBuilder.IsTopWall(x, y + 1, dungeonData, cells));
          bool isEastBorder = !SecretRoomBuilder.IsWall(x + 1, y, dungeonData, cells) && !SecretRoomBuilder.IsTopWall(x + 1, y, dungeonData, cells) || SecretRoomBuilder.IsFaceWallHigher(x + 1, y, dungeonData, cells);
          bool isSoutheastBorder = y > 3 && SecretRoomBuilder.IsFaceWallHigher(x + 1, y - 1, dungeonData, cells) && !SecretRoomBuilder.IsFaceWallHigher(x, y - 1, dungeonData, cells);
          bool isSouthBorder = y > 3 && SecretRoomBuilder.IsFaceWallHigher(x, y - 1, dungeonData, cells);
          bool isSouthwestBorder = y > 3 && SecretRoomBuilder.IsFaceWallHigher(x - 1, y - 1, dungeonData, cells) && !SecretRoomBuilder.IsFaceWallHigher(x, y - 1, dungeonData, cells);
          bool isWestBorder = !SecretRoomBuilder.IsWall(x - 1, y, dungeonData, cells) && !SecretRoomBuilder.IsTopWall(x - 1, y, dungeonData, cells) || SecretRoomBuilder.IsFaceWallHigher(x - 1, y, dungeonData, cells);
          bool isNorthwestBorder = SecretRoomBuilder.IsTopWall(x - 1, y, dungeonData, cells) && !SecretRoomBuilder.IsTopWall(x, y, dungeonData, cells) && (SecretRoomBuilder.IsWall(x, y + 1, dungeonData, cells) || SecretRoomBuilder.IsTopWall(x, y + 1, dungeonData, cells));
          if (mimicCheck)
          {
            isNorthBorder = SecretRoomBuilder.IsTopWallOrSecret(x, y, dungeonData, cells);
            isNortheastBorder = SecretRoomBuilder.IsTopWallOrSecret(x + 1, y, dungeonData, cells) && !SecretRoomBuilder.IsTopWallOrSecret(x, y, dungeonData, cells) && (SecretRoomBuilder.IsWallOrSecret(x, y + 1, dungeonData, cells) || SecretRoomBuilder.IsTopWallOrSecret(x, y + 1, dungeonData, cells));
            isEastBorder = !SecretRoomBuilder.IsWallOrSecret(x + 1, y, dungeonData, cells) && !SecretRoomBuilder.IsTopWallOrSecret(x + 1, y, dungeonData, cells) || SecretRoomBuilder.IsFaceWallHigherOrSecret(x + 1, y, dungeonData, cells);
            isSoutheastBorder = y > 3 && SecretRoomBuilder.IsFaceWallHigherOrSecret(x + 1, y - 1, dungeonData, cells) && !SecretRoomBuilder.IsFaceWallHigherOrSecret(x, y - 1, dungeonData, cells);
            isSouthBorder = y > 3 && SecretRoomBuilder.IsFaceWallHigherOrSecret(x, y - 1, dungeonData, cells);
            isSouthwestBorder = y > 3 && SecretRoomBuilder.IsFaceWallHigherOrSecret(x - 1, y - 1, dungeonData, cells) && !SecretRoomBuilder.IsFaceWallHigherOrSecret(x, y - 1, dungeonData, cells);
            isWestBorder = !SecretRoomBuilder.IsWallOrSecret(x - 1, y, dungeonData, cells) && !SecretRoomBuilder.IsTopWallOrSecret(x - 1, y, dungeonData, cells) || SecretRoomBuilder.IsFaceWallHigherOrSecret(x - 1, y, dungeonData, cells);
            isNorthwestBorder = SecretRoomBuilder.IsTopWallOrSecret(x - 1, y, dungeonData, cells) && !SecretRoomBuilder.IsTopWallOrSecret(x, y, dungeonData, cells) && (SecretRoomBuilder.IsWallOrSecret(x, y + 1, dungeonData, cells) || SecretRoomBuilder.IsTopWallOrSecret(x, y + 1, dungeonData, cells));
          }
          if (isNorthBorder || isNortheastBorder || isEastBorder || isSoutheastBorder || isSouthBorder || isSouthwestBorder || isWestBorder || isNorthwestBorder)
          {
            int rawTile = gridForCellPosition.GetIndexGivenSides(isNorthBorder, isNortheastBorder, isEastBorder, isSoutheastBorder, isSouthBorder, isSouthwestBorder, isWestBorder, isNorthwestBorder);
            if (gridForCellPosition.UsesRatChunkBorders)
            {
              bool isTwoSouthEmpty = y > 3;
              if (isTwoSouthEmpty)
                isTwoSouthEmpty = !dungeonData[x, y - 1].HasFloorNeighbor(dungeonData, includeDiagonals: true);
              TileIndexGrid.RatChunkResult result = TileIndexGrid.RatChunkResult.NONE;
              rawTile = gridForCellPosition.GetRatChunkIndexGivenSides(isNorthBorder, isNortheastBorder, isEastBorder, isSoutheastBorder, isSouthBorder, isSouthwestBorder, isWestBorder, isNorthwestBorder, isTwoSouthEmpty, out result);
            }
            int tileFromRawTile2 = BuilderUtil.GetTileFromRawTile(rawTile);
            tk2dSpriteDefinition spriteDefinition2 = dungeonCollection.spriteDefinitions[tileFromRawTile2];
            if ((Object) material2 == (Object) null)
              material2 = spriteDefinition2.material;
            int count2 = vector3List.Count;
            Vector3 vector3_5 = cell.ToVector3((float) cell.y);
            Vector3[] vector3Array2 = spriteDefinition2.ConstructExpensivePositions();
            for (int index = 0; index < vector3Array2.Length; ++index)
            {
              Vector3 vector3_6 = vector3Array2[index].WithZ(vector3Array2[index].y);
              vector3List.Add(vector3_5 + vector3_6 + vector3_2);
              vector2List.Add(spriteDefinition2.uvs[index]);
            }
            for (int index = 0; index < spriteDefinition2.indices.Length; ++index)
              intList2.Add(count2 + spriteDefinition2.indices[index]);
          }
        }
        Vector3 lhs = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        for (int index = 0; index < vector3List.Count; ++index)
          lhs = Vector3.Min(lhs, vector3List[index]);
        for (int index = 0; index < vector3List.Count; ++index)
          vector3List[index] -= lhs;
        mesh.vertices = vector3List.ToArray();
        mesh.uv = vector2List.ToArray();
        mesh.subMeshCount = 2;
        mesh.SetTriangles(intList1.ToArray(), 0);
        mesh.SetTriangles(intList2.ToArray(), 1);
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        GameObject roomCeilingMesh = new GameObject(objectName);
        MeshFilter meshFilter = roomCeilingMesh.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = roomCeilingMesh.AddComponent<MeshRenderer>();
        roomCeilingMesh.transform.position = lhs;
        meshFilter.mesh = mesh;
        meshRenderer.materials = new Material[2]
        {
          material1,
          material2
        };
        return roomCeilingMesh;
      }

      private static HashSet<IntVector2> CorrectForDoubledSecretRoomness(
        RoomHandler room,
        DungeonData data)
      {
        HashSet<IntVector2> intVector2Set = new HashSet<IntVector2>();
        if (room.area.instanceUsedExits.Count == 1)
        {
          RuntimeExitDefinition runtimeExitDefinition = room.exitDefinitionsByExit[room.area.exitToLocalDataMap[room.area.instanceUsedExits[0]]];
          if (runtimeExitDefinition.downstreamRoom == room && runtimeExitDefinition.upstreamRoom.area.prototypeRoom.category == PrototypeDungeonRoom.RoomCategory.SECRET)
          {
            List<IntVector2> representationIncFacewalls = runtimeExitDefinition.upstreamRoom.area.prototypeRoom.GetCellRepresentationIncFacewalls();
            List<IntVector2> intVector2List = new List<IntVector2>((IEnumerable<IntVector2>) IntVector2.CardinalsAndOrdinals);
            foreach (IntVector2 intVector2_1 in representationIncFacewalls)
            {
              intVector2Set.Add(runtimeExitDefinition.upstreamRoom.area.basePosition + intVector2_1);
              foreach (IntVector2 intVector2_2 in intVector2List)
                intVector2Set.Add(runtimeExitDefinition.upstreamRoom.area.basePosition + intVector2_1 + intVector2_2);
            }
          }
        }
        List<IntVector2> intVector2List1 = new List<IntVector2>();
        foreach (IntVector2 key in intVector2Set)
        {
          if (data[key] != null && data[key].isSecretRoomCell && !data[key].isExitCell)
            data[key].isSecretRoomCell = false;
          else
            intVector2List1.Add(key);
        }
        foreach (IntVector2 intVector2 in intVector2List1)
          intVector2Set.Remove(intVector2);
        return intVector2Set;
      }

      public static GameObject BuildRoomCover(
        RoomHandler room,
        tk2dTileMap tileMap,
        DungeonData dungeonData)
      {
        HashSet<IntVector2> intVector2Set = (HashSet<IntVector2>) null;
        if (!Dungeon.IsGenerating)
          intVector2Set = SecretRoomBuilder.CorrectForDoubledSecretRoomness(room, dungeonData);
        HashSet<IntVector2> roomCeilingCells = SecretRoomBuilder.GetRoomCeilingCells(room);
        GameObject roomCeilingMesh = SecretRoomBuilder.GenerateRoomCeilingMesh(roomCeilingCells, dungeonData: dungeonData);
        List<SecretRoomDoorBeer> secretRoomDoorBeerList = new List<SecretRoomDoorBeer>();
        for (int index = 0; index < room.area.instanceUsedExits.Count; ++index)
        {
          PrototypeRoomExit instanceUsedExit = room.area.instanceUsedExits[index];
          if (!room.area.exitToLocalDataMap[instanceUsedExit].oneWayDoor)
          {
            RuntimeExitDefinition exit = room.exitDefinitionsByExit[room.area.exitToLocalDataMap[instanceUsedExit]];
            if (Dungeon.IsGenerating || exit.downstreamRoom == room || !((Object) exit.downstreamRoom.area.prototypeRoom != (Object) null) || exit.downstreamRoom.area.prototypeRoom.category != PrototypeDungeonRoom.RoomCategory.SECRET)
            {
              SecretRoomDoorBeer secretRoomDoorBeer = SecretRoomBuilder.GenerateRoomDoorMesh(exit, room, dungeonData).AddComponent<SecretRoomDoorBeer>();
              secretRoomDoorBeer.exitDef = room.exitDefinitionsByExit[room.area.exitToLocalDataMap[instanceUsedExit]];
              secretRoomDoorBeer.linkedRoom = room.connectedRoomsByExit[instanceUsedExit];
              secretRoomDoorBeerList.Add(secretRoomDoorBeer);
            }
          }
        }
        GameObject gameObject = new GameObject("Secret Room");
        gameObject.transform.position = roomCeilingMesh.transform.position;
        roomCeilingMesh.transform.parent = gameObject.transform;
        SecretRoomManager secretRoomManager = gameObject.AddComponent<SecretRoomManager>();
        List<IntVector2> ceilingCellList = new List<IntVector2>((IEnumerable<IntVector2>) roomCeilingCells);
        secretRoomManager.InitializeCells(ceilingCellList);
        List<SecretRoomExitData> secretRoomExitDataList = SecretRoomUtility.BuildRoomExitColliders(room);
        for (int index = 0; index < secretRoomExitDataList.Count; ++index)
          secretRoomDoorBeerList[index].collider = secretRoomExitDataList[index];
        secretRoomManager.ceilingRenderer = roomCeilingMesh.GetComponent<Renderer>();
        for (int index = 0; index < secretRoomDoorBeerList.Count; ++index)
          secretRoomManager.doorObjects.Add(secretRoomDoorBeerList[index]);
        secretRoomManager.room = room;
        room.secretRoomManager = secretRoomManager;
        string roomName = room.GetRoomName();
        if (!string.IsNullOrEmpty(roomName) && roomName.Contains("SewersEntrance"))
        {
          secretRoomManager.revealStyle = SecretRoomManager.SecretRoomRevealStyle.FireplacePuzzle;
          secretRoomManager.InitializeForStyle();
        }
        else if (SecretRoomUtility.FloorHasComplexPuzzle || GameManager.Instance.Dungeon.SecretRoomComplexTriggers.Count == 0)
        {
          secretRoomManager.InitializeForStyle();
        }
        else
        {
          SecretRoomUtility.FloorHasComplexPuzzle = true;
          secretRoomManager.revealStyle = SecretRoomManager.SecretRoomRevealStyle.ComplexPuzzle;
          secretRoomManager.InitializeForStyle();
        }
        if (intVector2Set != null && intVector2Set.Count > 0)
        {
          foreach (IntVector2 key in intVector2Set)
            dungeonData[key].isSecretRoomCell = true;
        }
        return (GameObject) null;
      }
    }

}
