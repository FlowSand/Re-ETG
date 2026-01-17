// Decompiled with JetBrains decompiler
// Type: TK2DDungeonAssembler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using tk2dRuntime.TileMap;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class TK2DDungeonAssembler
    {
      private TileIndices t;
      private Dictionary<TilesetIndexMetadata.TilesetFlagType, List<Tuple<int, TilesetIndexMetadata>>> m_metadataLookupTable;

      private bool HasMetadataForRoomType(TilesetIndexMetadata.TilesetFlagType flagType, int roomType)
      {
        if (this.m_metadataLookupTable[flagType] == null)
          return false;
        List<Tuple<int, TilesetIndexMetadata>> tupleList = this.m_metadataLookupTable[flagType];
        for (int index = 0; index < tupleList.Count; ++index)
        {
          if (tupleList[index].Second.dungeonRoomSubType == roomType || tupleList[index].Second.secondRoomSubType == roomType || tupleList[index].Second.thirdRoomSubType == roomType)
            return true;
        }
        return false;
      }

      public void Initialize(TileIndices indices)
      {
        this.m_metadataLookupTable = new Dictionary<TilesetIndexMetadata.TilesetFlagType, List<Tuple<int, TilesetIndexMetadata>>>();
        TilesetIndexMetadata.TilesetFlagType[] values = (TilesetIndexMetadata.TilesetFlagType[]) Enum.GetValues(typeof (TilesetIndexMetadata.TilesetFlagType));
        for (int index = 0; index < values.Length; ++index)
          this.m_metadataLookupTable.Add(values[index], indices.dungeonCollection.GetIndicesForTileType(values[index]));
        SecretRoomUtility.metadataLookupTableRef = this.m_metadataLookupTable;
        this.t = indices;
      }

      public bool BCheck(Dungeon d, int ix, int iy, int thresh)
      {
        return d.data.CheckInBounds(new IntVector2(ix, iy), 3 + thresh);
      }

      public bool BCheck(Dungeon d, int ix, int iy) => this.BCheck(d, ix, iy, 0);

      public static void RuntimeResizeTileMap(
        tk2dTileMap tileMap,
        int w,
        int h,
        int partitionSizeX,
        int partitionSizeY)
      {
        foreach (Layer layer in tileMap.Layers)
        {
          layer.DestroyGameData(tileMap);
          if ((UnityEngine.Object) layer.gameObject != (UnityEngine.Object) null)
          {
            tk2dUtil.DestroyImmediate((UnityEngine.Object) layer.gameObject);
            layer.gameObject = (GameObject) null;
          }
        }
        Layer[] layerArray = new Layer[tileMap.Layers.Length];
        for (int index = 0; index < tileMap.Layers.Length; ++index)
        {
          Layer layer1 = tileMap.Layers[index];
          layerArray[index] = new Layer(layer1.hash, w, h, partitionSizeX, partitionSizeY);
          Layer layer2 = layerArray[index];
          if (!layer1.IsEmpty)
          {
            int num1 = Mathf.Min(tileMap.height, h);
            int num2 = Mathf.Min(tileMap.width, w);
            for (int y = 0; y < num1; ++y)
            {
              for (int x = 0; x < num2; ++x)
                layer2.SetRawTile(x, y, layer1.GetRawTile(x, y));
            }
            layer2.Optimize();
          }
        }
        bool flag = tileMap.ColorChannel != null && !tileMap.ColorChannel.IsEmpty;
        ColorChannel colorChannel = new ColorChannel(w, h, partitionSizeX, partitionSizeY);
        if (flag)
        {
          int num3 = Mathf.Min(tileMap.height, h) + 1;
          int num4 = Mathf.Min(tileMap.width, w) + 1;
          for (int y = 0; y < num3; ++y)
          {
            for (int x = 0; x < num4; ++x)
              colorChannel.SetColor(x, y, tileMap.ColorChannel.GetColor(x, y));
          }
          colorChannel.Optimize();
        }
        tileMap.ColorChannel = colorChannel;
        tileMap.Layers = layerArray;
        tileMap.width = w;
        tileMap.height = h;
        tileMap.partitionSizeX = partitionSizeX;
        tileMap.partitionSizeY = partitionSizeY;
        tileMap.ForceBuild();
      }

      private StampIndexVariant GetIndexFromStampArray(CellData current, List<StampIndexVariant> list)
      {
        float uniqueHash = current.UniqueHash;
        foreach (StampIndexVariant indexFromStampArray in list)
        {
          uniqueHash -= indexFromStampArray.likelihood;
          if ((double) uniqueHash <= 0.0)
            return indexFromStampArray;
        }
        return list[0];
      }

      private TileIndexVariant GetIndexFromTileArray(CellData current, List<TileIndexVariant> list)
      {
        float uniqueHash = current.UniqueHash;
        float num1 = 0.0f;
        for (int index = 0; index < list.Count; ++index)
          num1 += list[index].likelihood;
        float num2 = uniqueHash * num1;
        for (int index = 0; index < list.Count; ++index)
        {
          num2 -= list[index].likelihood;
          if ((double) num2 <= 0.0)
            return list[index];
        }
        return list[0];
      }

      private int GetIndexFromTupleArray(
        CellData current,
        List<Tuple<int, TilesetIndexMetadata>> list,
        int roomTypeIndex)
      {
        float uniqueHash = current.UniqueHash;
        float num1 = 0.0f;
        for (int index = 0; index < list.Count; ++index)
        {
          if (list[index].Second.dungeonRoomSubType == roomTypeIndex || list[index].Second.secondRoomSubType == roomTypeIndex || list[index].Second.thirdRoomSubType == roomTypeIndex)
            num1 += list[index].Second.weight;
        }
        float num2 = uniqueHash * num1;
        for (int index = 0; index < list.Count; ++index)
        {
          if (list[index].Second.dungeonRoomSubType == roomTypeIndex || list[index].Second.secondRoomSubType == roomTypeIndex || list[index].Second.thirdRoomSubType == roomTypeIndex)
          {
            num2 -= list[index].Second.weight;
            if ((double) num2 <= 0.0)
              return list[index].First;
          }
        }
        return list[0].First;
      }

      private TilesetIndexMetadata GetMetadataFromTupleArray(
        CellData current,
        List<Tuple<int, TilesetIndexMetadata>> list,
        int roomTypeIndex,
        out int index)
      {
        if (list == null)
        {
          index = -1;
          return (TilesetIndexMetadata) null;
        }
        float num1 = 0.0f;
        for (int index1 = 0; index1 < list.Count; ++index1)
        {
          Tuple<int, TilesetIndexMetadata> tuple = list[index1];
          if (tuple.Second.dungeonRoomSubType == -1 || tuple.Second.dungeonRoomSubType == roomTypeIndex || tuple.Second.secondRoomSubType == roomTypeIndex || tuple.Second.thirdRoomSubType == roomTypeIndex)
            num1 += tuple.Second.weight;
        }
        float num2 = UnityEngine.Random.value * num1;
        for (int index2 = 0; index2 < list.Count; ++index2)
        {
          Tuple<int, TilesetIndexMetadata> tuple = list[index2];
          if (tuple.Second.dungeonRoomSubType == -1 || tuple.Second.dungeonRoomSubType == roomTypeIndex || tuple.Second.secondRoomSubType == roomTypeIndex || tuple.Second.thirdRoomSubType == roomTypeIndex)
          {
            num2 -= tuple.Second.weight;
            if ((double) num2 <= 0.0)
            {
              index = tuple.First;
              return tuple.Second;
            }
          }
        }
        index = list[0].First;
        return list[0].Second;
      }

      public void ClearData(tk2dTileMap map)
      {
        for (int layerId = 0; layerId < map.Layers.Length; ++layerId)
          map.DeleteSprites(layerId, 0, 0, map.width - 1, map.height - 1);
      }

      private void BuildBorderIndicesForCell(
        CellData current,
        Dungeon d,
        tk2dTileMap map,
        int ix,
        int iy)
      {
        if (!this.t.placeBorders || current.nearestRoom != null && (UnityEngine.Object) current.nearestRoom.area.prototypeRoom != (UnityEngine.Object) null && current.nearestRoom.area.prototypeRoom.preventBorders)
          return;
        if (this.BCheck(d, ix, iy, -2) && (current.type == CellType.WALL || d.data.isTopWall(ix, iy)) && !d.data.isFaceWallHigher(ix, iy) && !d.data.isFaceWallLower(ix, iy))
          this.BuildBorderIndex(current, d, map, ix, iy);
        if (!this.BCheck(d, ix, iy, -2) || current.type == CellType.WALL && !d.data.isAnyFaceWall(ix, iy) || d.data.isTopWall(ix, iy) || !((UnityEngine.Object) d.roomMaterialDefinitions[current.cellVisualData.roomVisualTypeIndex].outerCeilingBorderGrid != (UnityEngine.Object) null))
          return;
        this.BuildOuterBorderIndex(current, d, map, ix, iy);
      }

      public void ClearTileIndicesForCell(Dungeon d, tk2dTileMap map, int ix, int iy)
      {
        CellData cellData = !d.data.CheckInBoundsAndValid(ix, iy) ? (CellData) null : d.data[ix, iy];
        int x = cellData == null ? ix : cellData.positionInTilemap.x;
        int y = cellData == null ? iy : cellData.positionInTilemap.y;
        for (int index = 0; index < map.Layers.Length; ++index)
          map.Layers[index].SetTile(x, y, -1);
        if (!TK2DTilemapChunkAnimator.PositionToAnimatorMap.ContainsKey(cellData.positionInTilemap))
          return;
        for (int index = 0; index < TK2DTilemapChunkAnimator.PositionToAnimatorMap[cellData.positionInTilemap].Count; ++index)
        {
          TilemapAnimatorTileManager animatorTileManager = TK2DTilemapChunkAnimator.PositionToAnimatorMap[cellData.positionInTilemap][index];
          if ((bool) (UnityEngine.Object) animatorTileManager.animator)
          {
            TK2DTilemapChunkAnimator.PositionToAnimatorMap[cellData.positionInTilemap].RemoveAt(index);
            --index;
            UnityEngine.Object.Destroy((UnityEngine.Object) animatorTileManager.animator.gameObject);
            animatorTileManager.animator = (TK2DTilemapChunkAnimator) null;
          }
        }
      }

      public void BuildTileIndicesForCell(Dungeon d, tk2dTileMap map, int ix, int iy)
      {
        CellData cellData = d.data.cellData[ix][iy];
        if (cellData == null)
          return;
        this.BuildOcclusionPartitionIndex(cellData, d, map, ix, iy);
        cellData.isOccludedByTopWall = d.data.isTopWall(ix, iy);
        if (cellData.cellVisualData.hasAlreadyBeenTilemapped || cellData.cellVisualData.precludeAllTileDrawing)
          return;
        bool flag1 = this.BCheck(d, ix, iy, 3) && d.data[ix, iy - 2] != null && d.data[ix, iy - 2].isExitCell;
        if (cellData.nearestRoom != null && cellData.nearestRoom.PrecludeTilemapDrawing && (!cellData.nearestRoom.DrawPrecludedCeilingTiles || !cellData.isExitCell && !flag1))
        {
          if (cellData.nearestRoom.DrawPrecludedCeilingTiles)
          {
            this.BuildCollisionIndex(cellData, d, map, ix, iy);
            this.BuildBorderIndicesForCell(cellData, d, map, ix, iy);
          }
          cellData.cellVisualData.precludeAllTileDrawing = true;
        }
        else if (cellData.parentRoom != null && cellData.parentRoom.PrecludeTilemapDrawing && (!cellData.nearestRoom.DrawPrecludedCeilingTiles || !cellData.isExitCell && !flag1))
        {
          if (cellData.parentRoom.DrawPrecludedCeilingTiles)
          {
            this.BuildCollisionIndex(cellData, d, map, ix, iy);
            this.BuildBorderIndicesForCell(cellData, d, map, ix, iy);
          }
          cellData.cellVisualData.precludeAllTileDrawing = true;
        }
        else
        {
          DungeonMaterial materialDefinition = d.roomMaterialDefinitions[cellData.cellVisualData.roomVisualTypeIndex];
          if (materialDefinition.overrideStoneFloorType && cellData.cellVisualData.floorType == CellVisualData.CellFloorType.Stone)
            cellData.cellVisualData.floorType = materialDefinition.overrideFloorType;
          bool flag2 = cellData.type == CellType.FLOOR || d.data.isFaceWallLower(ix, iy);
          if (flag2)
            this.BuildFloorIndex(cellData, d, map, ix, iy);
          this.BuildDecoIndices(cellData, d, map, ix, iy);
          if (flag2)
            this.BuildFloorEdgeBorderTiles(cellData, d, map, ix, iy);
          this.BuildFeatureEdgeBorderTiles(cellData, d, map, ix, iy);
          this.BuildCollisionIndex(cellData, d, map, ix, iy);
          if (this.BCheck(d, ix, iy, -2))
            this.ProcessFacewallIndices(cellData, d, map, ix, iy);
          this.BuildBorderIndicesForCell(cellData, d, map, ix, iy);
          TileIndexGrid borderGrid = d.roomMaterialDefinitions[cellData.cellVisualData.roomVisualTypeIndex].pitBorderFlatGrid;
          TileIndexGrid pitBorderFlatGrid = materialDefinition.additionalPitBorderFlatGrid;
          PrototypeRoomPitEntry.PitBorderType pitBorderType = cellData.GetPitBorderType(d.data);
          switch (pitBorderType)
          {
            case PrototypeRoomPitEntry.PitBorderType.FLAT:
              borderGrid = materialDefinition.pitBorderFlatGrid;
              break;
            case PrototypeRoomPitEntry.PitBorderType.RAISED:
              borderGrid = materialDefinition.pitBorderRaisedGrid;
              break;
          }
          int index1 = pitBorderType != PrototypeRoomPitEntry.PitBorderType.RAISED ? GlobalDungeonData.patternLayerIndex : GlobalDungeonData.actorCollisionLayerIndex;
          int index2 = index1;
          bool wallsArePits = GameManager.Instance.Dungeon.debugSettings.WALLS_ARE_PITS;
          if (cellData.type == CellType.FLOOR)
          {
            if (d.tileIndices.tilesetId != GlobalDungeonData.ValidTilesets.WESTGEON && d.tileIndices.tilesetId != GlobalDungeonData.ValidTilesets.FINALGEON)
              this.BuildShadowIndex(cellData, d, map, ix, iy);
            if ((UnityEngine.Object) borderGrid != (UnityEngine.Object) null)
              this.HandlePitBorderTilePlacement(cellData, borderGrid, map.Layers[index1], map, d);
            if ((UnityEngine.Object) pitBorderFlatGrid != (UnityEngine.Object) null)
              this.HandlePitBorderTilePlacement(cellData, pitBorderFlatGrid, map.Layers[index2], map, d);
          }
          else if (cellData.type == CellType.PIT && d.tileIndices.tilesetId != GlobalDungeonData.ValidTilesets.WESTGEON && d.tileIndices.tilesetId != GlobalDungeonData.ValidTilesets.FINALGEON)
            this.BuildPitShadowIndex(cellData, d, map, ix, iy);
          if (cellData.type == CellType.PIT || wallsArePits && cellData.isExitCell)
          {
            TileIndexGrid pitLayoutGrid = materialDefinition.pitLayoutGrid;
            if ((UnityEngine.Object) pitLayoutGrid == (UnityEngine.Object) null)
              pitLayoutGrid = d.roomMaterialDefinitions[0].pitLayoutGrid;
            map.data.Layers[GlobalDungeonData.pitLayerIndex].ForceNonAnimating = true;
            this.HandlePitTilePlacement(cellData, pitLayoutGrid, map.Layers[GlobalDungeonData.pitLayerIndex], d);
            if ((UnityEngine.Object) borderGrid != (UnityEngine.Object) null)
              this.HandlePitBorderTilePlacement(cellData, borderGrid, map.Layers[index1], map, d);
            if ((UnityEngine.Object) pitBorderFlatGrid != (UnityEngine.Object) null)
              this.HandlePitBorderTilePlacement(cellData, pitBorderFlatGrid, map.Layers[index2], map, d);
          }
          if (d.data.isTopDiagonalWall(ix, iy))
          {
            if (cellData.diagonalWallType == DiagonalWallType.NORTHEAST)
              this.AssignSpecificColorsToTile(cellData.positionInTilemap.x, cellData.positionInTilemap.y, GlobalDungeonData.collisionLayerIndex, (Color32) new Color(0.0f, 0.5f, 1f), (Color32) new Color(0.0f, 1f, 1f), (Color32) new Color(0.0f, 1f, 1f), (Color32) new Color(0.0f, 1f, 1f), map);
            else if (cellData.diagonalWallType == DiagonalWallType.NORTHWEST)
              this.AssignSpecificColorsToTile(cellData.positionInTilemap.x, cellData.positionInTilemap.y, GlobalDungeonData.collisionLayerIndex, (Color32) new Color(0.0f, 1f, 1f), (Color32) new Color(0.0f, 0.5f, 1f), (Color32) new Color(0.0f, 1f, 1f), (Color32) new Color(0.0f, 1f, 1f), map);
          }
          if (cellData.cellVisualData.pathTilesetGridIndex > -1)
          {
            TileIndexGrid pathGridDefinition = d.pathGridDefinitions[cellData.cellVisualData.pathTilesetGridIndex];
            this.HandlePathTilePlacement(cellData, d, map, pathGridDefinition);
          }
          if (cellData.cellVisualData.UsesCustomIndexOverride01)
            map.SetTile(cellData.positionInTilemap.x, cellData.positionInTilemap.y, cellData.cellVisualData.CustomIndexOverride01Layer, cellData.cellVisualData.CustomIndexOverride01);
          if (d.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.JUNGLEGEON)
            this.BuildOcclusionLayerCenterJungle(cellData, d, map, ix, iy);
          if ((double) cellData.distanceFromNearestRoom < 4.0 && cellData.nearestRoom.area.PrototypeLostWoodsRoom)
            this.HandleLostWoodsMirroring(cellData, d, map, ix, iy);
          cellData.hasBeenGenerated = true;
        }
      }

      private bool CheckLostWoodsCellValidity(Dungeon d, IntVector2 p1, IntVector2 p2)
      {
        CellData cellData1 = d.data[p1];
        CellData cellData2 = d.data[p2];
        return cellData1 != null && cellData2 != null && cellData2.isExitCell == cellData1.isExitCell && cellData2.IsAnyFaceWall() == cellData1.IsAnyFaceWall() && cellData2.IsTopWall() == cellData1.IsTopWall() && cellData2.type == cellData1.type;
      }

      private void HandleLostWoodsMirroring(
        CellData current,
        Dungeon d,
        tk2dTileMap map,
        int ix,
        int iy)
      {
        RoomHandler nearestRoom = current.nearestRoom;
        IntVector2 intVector2 = new IntVector2(ix - current.nearestRoom.area.basePosition.x, iy - current.nearestRoom.area.basePosition.y);
        for (int index1 = 0; index1 < d.data.rooms.Count; ++index1)
        {
          RoomHandler room = d.data.rooms[index1];
          if (room != nearestRoom && room.area.PrototypeLostWoodsRoom)
          {
            CellData cellData = d.data[intVector2 + room.area.basePosition];
            if (cellData != null && current != null && cellData.isExitCell == current.isExitCell && cellData.IsAnyFaceWall() == current.IsAnyFaceWall() && cellData.IsTopWall() == current.IsTopWall() && cellData.type == current.type && this.CheckLostWoodsCellValidity(d, current.position + new IntVector2(0, 1), cellData.position + new IntVector2(0, 1)) && this.CheckLostWoodsCellValidity(d, current.position + new IntVector2(0, -1), cellData.position + new IntVector2(0, -1)) && this.CheckLostWoodsCellValidity(d, current.position + new IntVector2(1, 0), cellData.position + new IntVector2(1, 0)) && this.CheckLostWoodsCellValidity(d, current.position + new IntVector2(-1, 0), cellData.position + new IntVector2(-1, 0)) && !cellData.cellVisualData.hasAlreadyBeenTilemapped)
            {
              cellData.cellVisualData.hasAlreadyBeenTilemapped = true;
              for (int index2 = 0; index2 < map.Layers.Length; ++index2)
                map.Layers[index2].SetTile(cellData.positionInTilemap.x, cellData.positionInTilemap.y, map.Layers[index2].GetTile(current.positionInTilemap.x, current.positionInTilemap.y));
            }
          }
        }
      }

      private void HandlePathTilePlacement(
        CellData current,
        Dungeon d,
        tk2dTileMap map,
        TileIndexGrid pathGrid)
      {
        List<CellData> cellNeighbors = d.data.GetCellNeighbors(current, true);
        bool[] flagArray = new bool[8];
        for (int index = 0; index < flagArray.Length; ++index)
        {
          if (current.cellVisualData.pathTilesetGridIndex == cellNeighbors[index].cellVisualData.pathTilesetGridIndex)
            flagArray[index] = true;
        }
        int indexGivenSides = pathGrid.GetIndexGivenSides(!flagArray[0], !flagArray[2], !flagArray[4], !flagArray[6]);
        int index1 = GlobalDungeonData.patternLayerIndex;
        if (d.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.MINEGEON && current.type != CellType.PIT)
        {
          if (flagArray[0] == flagArray[4] && flagArray[0] != flagArray[2] && flagArray[0] != flagArray[6])
            indexGivenSides += !flagArray[0] ? 2 : 1;
        }
        else if (d.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.MINEGEON)
        {
          index1 = GlobalDungeonData.killLayerIndex;
          if (cellNeighbors[4] != null && !flagArray[4] && cellNeighbors[4].type == CellType.PIT)
          {
            int index2 = pathGrid.PathPitPosts.indices[cellNeighbors[4].cellVisualData.roomVisualTypeIndex];
            if (flagArray[0] && flagArray[2])
              index2 = pathGrid.PathPitPostsBL.indices[cellNeighbors[4].cellVisualData.roomVisualTypeIndex];
            else if (flagArray[0] && flagArray[6])
              index2 = pathGrid.PathPitPostsBR.indices[cellNeighbors[4].cellVisualData.roomVisualTypeIndex];
            map.Layers[GlobalDungeonData.killLayerIndex].SetTile(cellNeighbors[4].positionInTilemap.x, cellNeighbors[4].positionInTilemap.y, index2);
          }
        }
        map.Layers[index1].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, indexGivenSides);
      }

      private void BuildFeatureEdgeBorderTiles(
        CellData current,
        Dungeon d,
        tk2dTileMap map,
        int ix,
        int iy)
      {
        if (d.tileIndices.tilesetId != GlobalDungeonData.ValidTilesets.WESTGEON)
          return;
        TileIndexGrid facadeBorderGrid = d.roomMaterialDefinitions[1].exteriorFacadeBorderGrid;
        List<CellData> cellNeighbors = d.data.GetCellNeighbors(current, true);
        bool[] eightSides = new bool[8];
        for (int index = 0; index < eightSides.Length; ++index)
        {
          if (cellNeighbors[index] != null)
            eightSides[index] = cellNeighbors[index].cellVisualData.IsFeatureCell || cellNeighbors[index].cellVisualData.IsFeatureAdditional;
        }
        int indexGivenEightSides = facadeBorderGrid.GetIndexGivenEightSides(eightSides);
        if (indexGivenEightSides == -1)
          return;
        map.Layers[GlobalDungeonData.decalLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, indexGivenEightSides);
      }

      private void BuildFloorEdgeBorderTiles(
        CellData current,
        Dungeon d,
        tk2dTileMap map,
        int ix,
        int iy)
      {
        if (current.type != CellType.FLOOR && !d.data.isFaceWallLower(ix, iy))
          return;
        TileIndexGrid tileIndexGrid = d.roomMaterialDefinitions[current.cellVisualData.roomVisualTypeIndex].roomFloorBorderGrid;
        if (d.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.WESTGEON && current.cellVisualData.IsFacewallForInteriorTransition)
          tileIndexGrid = d.roomMaterialDefinitions[current.cellVisualData.InteriorTransitionIndex].exteriorFacadeBorderGrid;
        if (!((UnityEngine.Object) tileIndexGrid != (UnityEngine.Object) null))
          return;
        if (current.diagonalWallType == DiagonalWallType.NONE || !d.data.isFaceWallLower(ix, iy))
        {
          List<CellData> cellNeighbors = d.data.GetCellNeighbors(current, true);
          bool[] eightSides = new bool[8];
          for (int index = 0; index < eightSides.Length; ++index)
          {
            if (cellNeighbors[index] != null)
            {
              eightSides[index] = cellNeighbors[index].type == CellType.WALL && !d.data.isTopWall(cellNeighbors[index].position.x, cellNeighbors[index].position.y + 1) && cellNeighbors[index].diagonalWallType == DiagonalWallType.NONE;
              bool flag = cellNeighbors[index].isSecretRoomCell || d.data[cellNeighbors[index].position + IntVector2.Up].IsTopWall() && d.data[cellNeighbors[index].position + IntVector2.Up].isSecretRoomCell;
              eightSides[index] = eightSides[index] || flag != current.isSecretRoomCell;
            }
          }
          int indexGivenEightSides = tileIndexGrid.GetIndexGivenEightSides(eightSides);
          if (indexGivenEightSides == -1)
            return;
          map.Layers[GlobalDungeonData.decalLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, indexGivenEightSides);
        }
        else
        {
          int indexByWeight = tileIndexGrid.quadNubs.GetIndexByWeight();
          if (indexByWeight == -1)
            return;
          map.Layers[GlobalDungeonData.decalLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, indexByWeight);
        }
      }

      private void AssignSpecificColorsToTile(
        int ix,
        int iy,
        int layer,
        Color32 bottomLeft,
        Color32 bottomRight,
        Color32 topLeft,
        Color32 topRight,
        tk2dTileMap map)
      {
        if (!map.HasColorChannel())
          map.CreateColorChannel();
        ColorChannel colorChannel = map.ColorChannel;
        map.data.Layers[layer].useColor = true;
        colorChannel.SetTileColorGradient(ix, iy, bottomLeft, bottomRight, topLeft, topRight);
      }

      private void AssignColorGradientToTile(
        int ix,
        int iy,
        int layer,
        Color32 bottom,
        Color32 top,
        tk2dTileMap map)
      {
        if (!map.HasColorChannel())
          map.CreateColorChannel();
        ColorChannel colorChannel = map.ColorChannel;
        map.data.Layers[layer].useColor = true;
        colorChannel.SetTileColorGradient(ix, iy, bottom, bottom, top, top);
      }

      private void AssignColorOverrideToTile(
        int ix,
        int iy,
        int layer,
        Color32 color,
        tk2dTileMap map)
      {
        if (!map.HasColorChannel())
          map.CreateColorChannel();
        ColorChannel colorChannel = map.ColorChannel;
        map.data.Layers[layer].useColor = true;
        colorChannel.SetTileColorOverride(ix, iy, color);
      }

      private void ClearAllIndices(CellData current, Dungeon d, tk2dTileMap map, int ix, int iy)
      {
        for (int index = 0; index < map.Layers.Length; ++index)
          map.Layers[index].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, -1);
      }

      private bool CheckHasValidFloorGridForRoomSubType(List<TileIndexGrid> indexGrids, int roomType)
      {
        for (int index = 0; index < indexGrids.Count; ++index)
        {
          if (indexGrids[index].roomTypeRestriction == -1 || indexGrids[index].roomTypeRestriction == roomType)
            return true;
        }
        return false;
      }

      private RoomInternalMaterialTransition GetMaterialTransitionFromSubtypes(
        Dungeon d,
        int roomType,
        int cellType)
      {
        if (!d.roomMaterialDefinitions[roomType].usesInternalMaterialTransitions)
          return (RoomInternalMaterialTransition) null;
        if (roomType == cellType)
          return (RoomInternalMaterialTransition) null;
        for (int index = 0; index < d.roomMaterialDefinitions[roomType].internalMaterialTransitions.Length; ++index)
        {
          if (d.roomMaterialDefinitions[roomType].internalMaterialTransitions[index].materialTransition == cellType)
            return d.roomMaterialDefinitions[roomType].internalMaterialTransitions[index];
        }
        return (RoomInternalMaterialTransition) null;
      }

      private void BuildFloorIndex(CellData current, Dungeon d, tk2dTileMap map, int ix, int iy)
      {
        if (current.cellVisualData.inheritedOverrideIndex != -1)
          map.Layers[!current.cellVisualData.inheritedOverrideIndexIsFloor ? GlobalDungeonData.patternLayerIndex : GlobalDungeonData.floorLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, current.cellVisualData.inheritedOverrideIndex);
        if (current.cellVisualData.inheritedOverrideIndex == -1 || !current.cellVisualData.inheritedOverrideIndexIsFloor)
        {
          bool flag1 = true;
          TileIndexGrid randomGridFromArray1 = d.roomMaterialDefinitions[current.cellVisualData.roomVisualTypeIndex].GetRandomGridFromArray(d.roomMaterialDefinitions[current.cellVisualData.roomVisualTypeIndex].floorSquares);
          if ((UnityEngine.Object) randomGridFromArray1 == (UnityEngine.Object) null)
            flag1 = false;
          if (flag1)
          {
            for (int index1 = 0; index1 < 3; ++index1)
            {
              for (int index2 = 0; index2 < 3; ++index2)
              {
                if (!this.BCheck(d, ix + index1, iy + index2))
                  flag1 = false;
                if (d.data.isWall(ix + index1, iy + index2) || d.data.isAnyFaceWall(ix + index1, iy + index2))
                  flag1 = false;
                CellData cellData = d.data.cellData[ix + index1][iy + index2];
                if (cellData.HasWallNeighbor(includeTopwalls: false) || cellData.HasPitNeighbor(d.data))
                  flag1 = false;
                if (cellData.cellVisualData.roomVisualTypeIndex != current.cellVisualData.roomVisualTypeIndex)
                  flag1 = false;
                if (cellData.cellVisualData.inheritedOverrideIndex != -1)
                  flag1 = false;
                if (cellData.cellVisualData.floorType == CellVisualData.CellFloorType.Ice)
                  flag1 = false;
                if (cellData.doesDamage)
                  flag1 = false;
                if (!flag1)
                  break;
              }
              if (!flag1)
                break;
            }
          }
          if (flag1 && (double) current.UniqueHash < (double) d.roomMaterialDefinitions[current.cellVisualData.roomVisualTypeIndex].floorSquareDensity)
          {
            TileIndexGrid tileIndexGrid = randomGridFromArray1;
            int num = (double) current.UniqueHash >= 0.02500000037252903 ? 3 : 2;
            if (tileIndexGrid.topIndices.indices[0] == -1)
              num = 2;
            for (int index3 = 0; index3 < num; ++index3)
            {
              for (int index4 = 0; index4 < num; ++index4)
              {
                bool isNorthBorder = index4 == num - 1;
                bool isSouthBorder = index4 == 0;
                bool isEastBorder = index3 == num - 1;
                bool isWestBorder = index3 == 0;
                int indexGivenSides = tileIndexGrid.GetIndexGivenSides(isNorthBorder, isEastBorder, isSouthBorder, isWestBorder);
                if (this.BCheck(d, ix + index3, iy + index4) && !d.data.isFaceWallLower(ix + index3, iy + index4) && d.data.cellData[ix + index3][iy + index4].type != CellType.PIT)
                {
                  CellData cellData = d.data.cellData[ix + index3][iy + index4];
                  cellData.cellVisualData.inheritedOverrideIndex = indexGivenSides;
                  cellData.cellVisualData.inheritedOverrideIndexIsFloor = true;
                  map.Layers[GlobalDungeonData.floorLayerIndex].SetTile(cellData.positionInTilemap.x, cellData.positionInTilemap.y, indexGivenSides);
                }
              }
            }
          }
          else if (current.cellVisualData.floorType == CellVisualData.CellFloorType.Ice && d.roomMaterialDefinitions[current.cellVisualData.roomVisualTypeIndex].supportsIceSquares)
          {
            TileIndexGrid randomGridFromArray2 = d.roomMaterialDefinitions[current.cellVisualData.roomVisualTypeIndex].GetRandomGridFromArray(d.roomMaterialDefinitions[current.cellVisualData.roomVisualTypeIndex].iceGrids);
            List<CellData> cellNeighbors = d.data.GetCellNeighbors(current, true);
            bool isNorthBorder = cellNeighbors[0].cellVisualData.floorType != CellVisualData.CellFloorType.Ice;
            bool isNortheastBorder = cellNeighbors[1].cellVisualData.floorType != CellVisualData.CellFloorType.Ice;
            bool isEastBorder = cellNeighbors[2].cellVisualData.floorType != CellVisualData.CellFloorType.Ice;
            bool isSoutheastBorder = cellNeighbors[3].cellVisualData.floorType != CellVisualData.CellFloorType.Ice;
            bool isSouthBorder = cellNeighbors[4].cellVisualData.floorType != CellVisualData.CellFloorType.Ice;
            bool isSouthwestBorder = cellNeighbors[5].cellVisualData.floorType != CellVisualData.CellFloorType.Ice;
            bool isWestBorder = cellNeighbors[6].cellVisualData.floorType != CellVisualData.CellFloorType.Ice;
            bool isNorthwestBorder = cellNeighbors[7].cellVisualData.floorType != CellVisualData.CellFloorType.Ice;
            int indexGivenSides = randomGridFromArray2.GetIndexGivenSides(isNorthBorder, isNortheastBorder, isEastBorder, isSoutheastBorder, isSouthBorder, isSouthwestBorder, isWestBorder, isNorthwestBorder);
            map.Layers[GlobalDungeonData.floorLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, indexGivenSides);
            map.Layers[GlobalDungeonData.patternLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, indexGivenSides);
          }
          else if (current.doesDamage && d.roomMaterialDefinitions[current.cellVisualData.roomVisualTypeIndex].supportsLavaOrLavalikeSquares)
          {
            TileIndexGrid randomGridFromArray3 = d.roomMaterialDefinitions[current.cellVisualData.roomVisualTypeIndex].GetRandomGridFromArray(d.roomMaterialDefinitions[current.cellVisualData.roomVisualTypeIndex].lavaGrids);
            List<CellData> cellNeighbors = d.data.GetCellNeighbors(current, true);
            bool isNorthBorder = !cellNeighbors[0].doesDamage;
            bool isNortheastBorder = !cellNeighbors[1].doesDamage;
            bool isEastBorder = !cellNeighbors[2].doesDamage;
            bool isSoutheastBorder = !cellNeighbors[3].doesDamage;
            bool isSouthBorder = !cellNeighbors[4].doesDamage;
            bool isSouthwestBorder = !cellNeighbors[5].doesDamage;
            bool isWestBorder = !cellNeighbors[6].doesDamage;
            bool isNorthwestBorder = !cellNeighbors[7].doesDamage;
            int indexGivenSides = randomGridFromArray3.GetIndexGivenSides(isNorthBorder, isNortheastBorder, isEastBorder, isSoutheastBorder, isSouthBorder, isSouthwestBorder, isWestBorder, isNorthwestBorder);
            map.Layers[GlobalDungeonData.floorLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, indexGivenSides);
            map.Layers[GlobalDungeonData.patternLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, indexGivenSides);
          }
          else
          {
            RoomInternalMaterialTransition transitionFromSubtypes = current == null || current.parentRoom == null ? (RoomInternalMaterialTransition) null : this.GetMaterialTransitionFromSubtypes(d, current.parentRoom.RoomVisualSubtype, current.cellVisualData.roomVisualTypeIndex);
            if (transitionFromSubtypes != null)
            {
              List<CellData> cellNeighbors = d.data.GetCellNeighbors(current, true);
              bool isNorthBorder = cellNeighbors[0].cellVisualData.roomVisualTypeIndex == current.parentRoom.RoomVisualSubtype;
              bool isNortheastBorder = cellNeighbors[1].cellVisualData.roomVisualTypeIndex == current.parentRoom.RoomVisualSubtype;
              bool isEastBorder = cellNeighbors[2].cellVisualData.roomVisualTypeIndex == current.parentRoom.RoomVisualSubtype;
              bool isSoutheastBorder = cellNeighbors[3].cellVisualData.roomVisualTypeIndex == current.parentRoom.RoomVisualSubtype;
              bool isSouthBorder = cellNeighbors[4].cellVisualData.roomVisualTypeIndex == current.parentRoom.RoomVisualSubtype;
              bool isSouthwestBorder = cellNeighbors[5].cellVisualData.roomVisualTypeIndex == current.parentRoom.RoomVisualSubtype;
              bool isWestBorder = cellNeighbors[6].cellVisualData.roomVisualTypeIndex == current.parentRoom.RoomVisualSubtype;
              bool isNorthwestBorder = cellNeighbors[7].cellVisualData.roomVisualTypeIndex == current.parentRoom.RoomVisualSubtype;
              bool flag2 = isNorthBorder || isNortheastBorder || isEastBorder || isSoutheastBorder || isSouthBorder || isSouthwestBorder || isWestBorder || isNorthwestBorder;
              int tile = this.GetIndexFromTupleArray(current, this.m_metadataLookupTable[TilesetIndexMetadata.TilesetFlagType.FLOOR_TILE], current.cellVisualData.roomVisualTypeIndex);
              if (flag2)
                tile = transitionFromSubtypes.transitionGrid.GetIndexGivenSides(isNorthBorder, isNortheastBorder, isEastBorder, isSoutheastBorder, isSouthBorder, isSouthwestBorder, isWestBorder, isNorthwestBorder);
              map.Layers[GlobalDungeonData.floorLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, tile);
            }
            else
            {
              int indexFromTupleArray = this.GetIndexFromTupleArray(current, this.m_metadataLookupTable[TilesetIndexMetadata.TilesetFlagType.FLOOR_TILE], current.cellVisualData.roomVisualTypeIndex);
              map.Layers[GlobalDungeonData.floorLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, indexFromTupleArray);
            }
          }
        }
        if (!d.data.HasDoorAtPosition(new IntVector2(ix, iy)) && d.data[ix, iy].cellVisualData.doorFeetOverrideMode == 0)
          return;
        DungeonDoorController dungeonDoorController = (DungeonDoorController) null;
        IntVector2 key = new IntVector2(ix, iy);
        if (d.data.doors.ContainsKey(key))
          dungeonDoorController = d.data.doors[key];
        if (d.data[ix, iy].cellVisualData.doorFeetOverrideMode == 1 || (UnityEngine.Object) dungeonDoorController != (UnityEngine.Object) null && dungeonDoorController.northSouth)
        {
          int index = -1;
          this.GetMetadataFromTupleArray(current, this.m_metadataLookupTable[TilesetIndexMetadata.TilesetFlagType.DOOR_FEET_NS], -1, out index);
          map.Layers[GlobalDungeonData.decalLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, index);
          map.Layers[GlobalDungeonData.patternLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, index);
        }
        else
        {
          if (d.data[ix, iy].cellVisualData.doorFeetOverrideMode != 2 && (!((UnityEngine.Object) dungeonDoorController != (UnityEngine.Object) null) || dungeonDoorController.northSouth))
            return;
          int index = -1;
          this.GetMetadataFromTupleArray(current, this.m_metadataLookupTable[TilesetIndexMetadata.TilesetFlagType.DOOR_FEET_EW], -1, out index);
          map.Layers[GlobalDungeonData.patternLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, index);
          map.Layers[GlobalDungeonData.decalLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, index);
        }
      }

      private void BuildDecoIndices(CellData current, Dungeon d, tk2dTileMap map, int ix, int iy)
      {
        if (current.type != CellType.FLOOR && !current.IsLowerFaceWall() || d.data.isTopWall(ix, iy) || current.cellVisualData.floorTileOverridden || current.cellVisualData.inheritedOverrideIndex != -1)
          return;
        DungeonMaterial materialDefinition = d.roomMaterialDefinitions[current.cellVisualData.roomVisualTypeIndex];
        if (current.HasPitNeighbor(d.data))
          return;
        if (current.cellVisualData.isPattern)
        {
          List<CellData> cellNeighbors = d.data.GetCellNeighbors(current, true);
          bool[] flagArray = new bool[8];
          for (int index = 0; index < flagArray.Length; ++index)
            flagArray[index] = !cellNeighbors[index].cellVisualData.isPattern && cellNeighbors[index].type != CellType.WALL;
          TileIndexGrid tileIndexGrid = !materialDefinition.usesPatternLayer ? this.t.patternIndexGrid : materialDefinition.patternIndexGrid;
          current.cellVisualData.preventFloorStamping = true;
          if ((UnityEngine.Object) tileIndexGrid != (UnityEngine.Object) null)
            map.Layers[GlobalDungeonData.patternLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, tileIndexGrid.GetIndexGivenSides(flagArray[0], flagArray[1], flagArray[2], flagArray[3], flagArray[4], flagArray[5], flagArray[6], flagArray[7]));
        }
        if (!current.cellVisualData.isDecal)
          return;
        List<CellData> cellNeighbors1 = d.data.GetCellNeighbors(current, true);
        bool[] flagArray1 = new bool[8];
        for (int index = 0; index < flagArray1.Length; ++index)
          flagArray1[index] = !cellNeighbors1[index].cellVisualData.isDecal && cellNeighbors1[index].type != CellType.WALL;
        TileIndexGrid tileIndexGrid1 = !materialDefinition.usesDecalLayer ? this.t.decalIndexGrid : materialDefinition.decalIndexGrid;
        current.cellVisualData.preventFloorStamping = true;
        if (!((UnityEngine.Object) tileIndexGrid1 != (UnityEngine.Object) null))
          return;
        map.Layers[GlobalDungeonData.decalLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, tileIndexGrid1.GetIndexGivenSides(flagArray1[0], flagArray1[1], flagArray1[2], flagArray1[3], flagArray1[4], flagArray1[5], flagArray1[6], flagArray1[7]));
      }

      private bool IsValidJungleBorderCell(CellData current, Dungeon d, int ix, int iy)
      {
        return !current.cellVisualData.ceilingHasBeenProcessed && !this.IsCardinalBorder(current, d, ix, iy) && current.type == CellType.WALL && (iy < 2 || !d.data.isFaceWallLower(ix, iy)) && !d.data.isTopDiagonalWall(ix, iy);
      }

      private bool IsValidJungleOcclusionCell(CellData current, Dungeon d, int ix, int iy)
      {
        if (!this.BCheck(d, ix, iy, 1) || current.cellVisualData.ceilingHasBeenProcessed || current.cellVisualData.occlusionHasBeenProcessed)
          return false;
        if (current.type != CellType.WALL || this.IsCardinalBorder(current, d, ix, iy))
          return true;
        if (iy <= 2)
          return false;
        return d.data.isFaceWallLower(ix, iy) || d.data.isFaceWallHigher(ix, iy);
      }

      private void BuildOcclusionLayerCenterJungle(
        CellData current,
        Dungeon d,
        tk2dTileMap map,
        int ix,
        int iy)
      {
        if (!this.IsValidJungleOcclusionCell(current, d, ix, iy))
          return;
        bool flag1 = true;
        bool flag2 = true;
        if (!this.BCheck(d, ix, iy))
        {
          flag1 = false;
          flag2 = false;
        }
        if ((double) current.UniqueHash < 0.05000000074505806)
        {
          flag1 = false;
          flag2 = false;
        }
        for (int index1 = 0; index1 < 3; ++index1)
        {
          for (int index2 = 0; index2 < 3; ++index2)
          {
            if (!this.IsValidJungleOcclusionCell(d.data[ix + index1, iy + index2], d, ix + index1, iy + index2))
            {
              flag2 = false;
              if (index1 < 2 || index2 < 2)
                flag1 = false;
            }
            if (!flag2 && !flag1)
              break;
          }
          if (!flag2 && !flag1)
            break;
        }
        if (flag2 && (double) current.UniqueHash < 0.75)
        {
          map.Layers[GlobalDungeonData.occlusionPartitionIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, 352);
          map.Layers[GlobalDungeonData.occlusionPartitionIndex].SetTile(current.positionInTilemap.x + 1, current.positionInTilemap.y, 353);
          map.Layers[GlobalDungeonData.occlusionPartitionIndex].SetTile(current.positionInTilemap.x + 2, current.positionInTilemap.y, 354);
          d.data[ix + 1, iy].cellVisualData.occlusionHasBeenProcessed = true;
          d.data[ix + 2, iy].cellVisualData.occlusionHasBeenProcessed = true;
          map.Layers[GlobalDungeonData.occlusionPartitionIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y + 1, 330);
          map.Layers[GlobalDungeonData.occlusionPartitionIndex].SetTile(current.positionInTilemap.x + 1, current.positionInTilemap.y + 1, 331);
          map.Layers[GlobalDungeonData.occlusionPartitionIndex].SetTile(current.positionInTilemap.x + 2, current.positionInTilemap.y + 1, 332);
          d.data[ix, iy + 1].cellVisualData.occlusionHasBeenProcessed = true;
          d.data[ix + 1, iy + 1].cellVisualData.occlusionHasBeenProcessed = true;
          d.data[ix + 2, iy + 1].cellVisualData.occlusionHasBeenProcessed = true;
          map.Layers[GlobalDungeonData.occlusionPartitionIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y + 2, 308);
          map.Layers[GlobalDungeonData.occlusionPartitionIndex].SetTile(current.positionInTilemap.x + 1, current.positionInTilemap.y + 2, 309);
          map.Layers[GlobalDungeonData.occlusionPartitionIndex].SetTile(current.positionInTilemap.x + 2, current.positionInTilemap.y + 2, 310);
          d.data[ix, iy + 2].cellVisualData.occlusionHasBeenProcessed = true;
          d.data[ix + 1, iy + 2].cellVisualData.occlusionHasBeenProcessed = true;
          d.data[ix + 2, iy + 2].cellVisualData.occlusionHasBeenProcessed = true;
        }
        else if (flag1)
        {
          map.Layers[GlobalDungeonData.occlusionPartitionIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, 418);
          map.Layers[GlobalDungeonData.occlusionPartitionIndex].SetTile(current.positionInTilemap.x + 1, current.positionInTilemap.y, 419);
          d.data[ix + 1, iy].cellVisualData.occlusionHasBeenProcessed = true;
          map.Layers[GlobalDungeonData.occlusionPartitionIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y + 1, 396);
          map.Layers[GlobalDungeonData.occlusionPartitionIndex].SetTile(current.positionInTilemap.x + 1, current.positionInTilemap.y + 1, 397);
          d.data[ix, iy + 1].cellVisualData.occlusionHasBeenProcessed = true;
          d.data[ix + 1, iy + 1].cellVisualData.occlusionHasBeenProcessed = true;
        }
        else
          map.Layers[GlobalDungeonData.occlusionPartitionIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, 374);
        d.data[ix, iy].cellVisualData.occlusionHasBeenProcessed = true;
      }

      private void BuildBorderLayerCenterJungle(
        CellData current,
        Dungeon d,
        tk2dTileMap map,
        int ix,
        int iy)
      {
        if (!this.IsValidJungleBorderCell(current, d, ix, iy))
          return;
        bool flag1 = true;
        bool flag2 = true;
        if (!this.BCheck(d, ix, iy))
        {
          flag1 = false;
          flag2 = false;
        }
        if ((double) current.UniqueHash < 0.05000000074505806)
        {
          flag1 = false;
          flag2 = false;
        }
        for (int index1 = 0; index1 < 3; ++index1)
        {
          for (int index2 = 0; index2 < 3; ++index2)
          {
            if (!this.IsValidJungleBorderCell(d.data[ix + index1, iy + index2], d, ix + index1, iy + index2))
            {
              flag2 = false;
              if (index1 < 2 || index2 < 2)
                flag1 = false;
            }
            if (!flag2 && !flag1)
              break;
          }
          if (!flag2 && !flag1)
            break;
        }
        if (flag2 && (double) current.UniqueHash < 0.75)
        {
          map.Layers[GlobalDungeonData.borderLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, 352);
          map.Layers[GlobalDungeonData.occlusionPartitionIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, 352);
          map.Layers[GlobalDungeonData.borderLayerIndex].SetTile(current.positionInTilemap.x + 1, current.positionInTilemap.y, 353);
          map.Layers[GlobalDungeonData.occlusionPartitionIndex].SetTile(current.positionInTilemap.x + 1, current.positionInTilemap.y, 353);
          map.Layers[GlobalDungeonData.borderLayerIndex].SetTile(current.positionInTilemap.x + 2, current.positionInTilemap.y, 354);
          map.Layers[GlobalDungeonData.occlusionPartitionIndex].SetTile(current.positionInTilemap.x + 2, current.positionInTilemap.y, 354);
          d.data[ix + 1, iy].cellVisualData.ceilingHasBeenProcessed = true;
          d.data[ix + 2, iy].cellVisualData.ceilingHasBeenProcessed = true;
          map.Layers[GlobalDungeonData.borderLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y + 1, 330);
          map.Layers[GlobalDungeonData.occlusionPartitionIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y + 1, 330);
          map.Layers[GlobalDungeonData.borderLayerIndex].SetTile(current.positionInTilemap.x + 1, current.positionInTilemap.y + 1, 331);
          map.Layers[GlobalDungeonData.occlusionPartitionIndex].SetTile(current.positionInTilemap.x + 1, current.positionInTilemap.y + 1, 331);
          map.Layers[GlobalDungeonData.borderLayerIndex].SetTile(current.positionInTilemap.x + 2, current.positionInTilemap.y + 1, 332);
          map.Layers[GlobalDungeonData.occlusionPartitionIndex].SetTile(current.positionInTilemap.x + 2, current.positionInTilemap.y + 1, 332);
          d.data[ix, iy + 1].cellVisualData.ceilingHasBeenProcessed = true;
          d.data[ix + 1, iy + 1].cellVisualData.ceilingHasBeenProcessed = true;
          d.data[ix + 2, iy + 1].cellVisualData.ceilingHasBeenProcessed = true;
          map.Layers[GlobalDungeonData.borderLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y + 2, 308);
          map.Layers[GlobalDungeonData.occlusionPartitionIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y + 2, 308);
          map.Layers[GlobalDungeonData.borderLayerIndex].SetTile(current.positionInTilemap.x + 1, current.positionInTilemap.y + 2, 309);
          map.Layers[GlobalDungeonData.occlusionPartitionIndex].SetTile(current.positionInTilemap.x + 1, current.positionInTilemap.y + 2, 309);
          map.Layers[GlobalDungeonData.borderLayerIndex].SetTile(current.positionInTilemap.x + 2, current.positionInTilemap.y + 2, 310);
          map.Layers[GlobalDungeonData.occlusionPartitionIndex].SetTile(current.positionInTilemap.x + 2, current.positionInTilemap.y + 2, 310);
          d.data[ix, iy + 2].cellVisualData.ceilingHasBeenProcessed = true;
          d.data[ix + 1, iy + 2].cellVisualData.ceilingHasBeenProcessed = true;
          d.data[ix + 2, iy + 2].cellVisualData.ceilingHasBeenProcessed = true;
        }
        else if (flag1)
        {
          map.Layers[GlobalDungeonData.borderLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, 418);
          map.Layers[GlobalDungeonData.occlusionPartitionIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, 418);
          map.Layers[GlobalDungeonData.borderLayerIndex].SetTile(current.positionInTilemap.x + 1, current.positionInTilemap.y, 419);
          map.Layers[GlobalDungeonData.occlusionPartitionIndex].SetTile(current.positionInTilemap.x + 1, current.positionInTilemap.y, 419);
          d.data[ix + 1, iy].cellVisualData.ceilingHasBeenProcessed = true;
          map.Layers[GlobalDungeonData.borderLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y + 1, 396);
          map.Layers[GlobalDungeonData.occlusionPartitionIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y + 1, 396);
          map.Layers[GlobalDungeonData.borderLayerIndex].SetTile(current.positionInTilemap.x + 1, current.positionInTilemap.y + 1, 397);
          map.Layers[GlobalDungeonData.occlusionPartitionIndex].SetTile(current.positionInTilemap.x + 1, current.positionInTilemap.y + 1, 397);
          d.data[ix, iy + 1].cellVisualData.ceilingHasBeenProcessed = true;
          d.data[ix + 1, iy + 1].cellVisualData.ceilingHasBeenProcessed = true;
        }
        else
        {
          map.Layers[GlobalDungeonData.borderLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, 374);
          map.Layers[GlobalDungeonData.occlusionPartitionIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, 374);
        }
        d.data[ix, iy].cellVisualData.ceilingHasBeenProcessed = true;
      }

      private void BuildCollisionIndex(CellData current, Dungeon d, tk2dTileMap map, int ix, int iy)
      {
        if (current.type != CellType.WALL || iy >= 2 && d.data.isFaceWallLower(ix, iy) || d.data.isTopDiagonalWall(ix, iy))
          return;
        TileIndexGrid ceilingBorderGrid = d.roomMaterialDefinitions[current.cellVisualData.roomVisualTypeIndex].roomCeilingBorderGrid;
        if ((d.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.WESTGEON || d.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.OFFICEGEON) && current.nearestRoom != null)
          ceilingBorderGrid = d.roomMaterialDefinitions[current.nearestRoom.RoomVisualSubtype].roomCeilingBorderGrid;
        if ((UnityEngine.Object) ceilingBorderGrid == (UnityEngine.Object) null)
          ceilingBorderGrid = d.roomMaterialDefinitions[0].roomCeilingBorderGrid;
        map.Layers[GlobalDungeonData.collisionLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, ceilingBorderGrid.centerIndices.indices[0]);
      }

      private void BuildPitShadowIndex(CellData current, Dungeon d, tk2dTileMap map, int ix, int iy)
      {
        if (!d.roomMaterialDefinitions[current.cellVisualData.roomVisualTypeIndex].doPitAO || current != null && current.cellVisualData.hasStampedPath)
          return;
        int floorLayerIndex = GlobalDungeonData.floorLayerIndex;
        if (!this.BCheck(d, ix, iy, -2))
          return;
        CellData cellData1 = d.data.cellData[ix - 1][iy];
        CellData cellData2 = d.data.cellData[ix + 1][iy];
        CellData cellData3 = d.data.cellData[ix][iy + 1];
        CellData cellData4 = d.data.cellData[ix][iy + 2];
        CellData cellData5 = d.data.cellData[ix + 1][iy + 2];
        CellData cellData6 = d.data.cellData[ix + 1][iy + 1];
        CellData cellData7 = d.data.cellData[ix - 1][iy + 2];
        CellData cellData8 = d.data.cellData[ix - 1][iy + 1];
        DungeonMaterial materialDefinition = d.roomMaterialDefinitions[current.cellVisualData.roomVisualTypeIndex];
        bool flag1;
        bool flag2;
        bool flag3;
        bool flag4;
        bool flag5;
        if (materialDefinition.pitsAreOneDeep)
        {
          flag1 = cellData1.type != CellType.PIT;
          flag2 = cellData2.type != CellType.PIT;
          flag3 = cellData3.type != CellType.PIT;
          flag4 = cellData6.type != CellType.PIT;
          flag5 = cellData8.type != CellType.PIT;
        }
        else
        {
          flag1 = cellData3.type == CellType.PIT && cellData8.type != CellType.PIT;
          flag2 = cellData3.type == CellType.PIT && cellData6.type != CellType.PIT;
          flag3 = cellData4.type != CellType.PIT && cellData3.type == CellType.PIT;
          flag4 = cellData5.type != CellType.PIT && cellData6.type == CellType.PIT;
          flag5 = cellData7.type != CellType.PIT && cellData8.type == CellType.PIT;
        }
        if ((UnityEngine.Object) materialDefinition.pitfallVFXPrefab != (UnityEngine.Object) null && materialDefinition.pitfallVFXPrefab.name.ToLowerInvariant().Contains("splash"))
        {
          if (flag3 && flag1 && !flag2)
            map.Layers[floorLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, this.t.aoTileIndices.AOFloorWallUpAndLeft);
          else if (flag3 && flag2 && !flag1)
            map.Layers[floorLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, this.t.aoTileIndices.AOFloorWallUpAndRight);
          else if (flag3 && flag1 && flag2)
            map.Layers[floorLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, this.t.aoTileIndices.AOFloorWallUpAndBoth);
          else if (flag3 && !flag1 && !flag2)
            map.Layers[floorLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, this.t.aoTileIndices.AOFloorTileIndex);
        }
        else if (flag3 && flag1 && !flag2)
          map.Layers[floorLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, this.t.aoTileIndices.AOBottomWallTileLeftIndex);
        else if (flag3 && flag2 && !flag1)
          map.Layers[floorLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, this.t.aoTileIndices.AOBottomWallTileRightIndex);
        else if (flag3 && flag1 && flag2)
          map.Layers[floorLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, this.t.aoTileIndices.AOBottomWallTileBothIndex);
        else if (flag3 && !flag1 && !flag2)
          map.Layers[floorLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, this.t.aoTileIndices.AOBottomWallBaseTileIndex);
        if (!flag3)
        {
          bool flag6 = flag1 && !d.data.isTopWall(current.positionInTilemap.x - 1, current.positionInTilemap.y + 1);
          bool flag7 = flag2 && !d.data.isTopWall(current.positionInTilemap.x + 1, current.positionInTilemap.y + 1);
          if (flag6 && flag7)
            map.Layers[floorLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, this.t.aoTileIndices.AOFloorWallBoth);
          else if (flag6)
            map.Layers[floorLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, this.t.aoTileIndices.AOFloorWallLeft);
          else if (flag7)
            map.Layers[floorLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, this.t.aoTileIndices.AOFloorWallRight);
        }
        if (!flag3 && flag5 && !flag1 && !flag2 && !flag4)
          map.Layers[floorLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, this.t.aoTileIndices.AOFloorPizzaSliceLeft);
        else if (!flag3 && !flag5 && !flag1 && !flag2 && flag4)
          map.Layers[floorLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, this.t.aoTileIndices.AOFloorPizzaSliceRight);
        else if (!flag3 && flag5 && !flag2 && !flag1 && flag4)
          map.Layers[floorLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, this.t.aoTileIndices.AOFloorPizzaSliceBoth);
        else if (!flag3 && flag5 && !flag1 && flag2)
        {
          map.Layers[floorLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, this.t.aoTileIndices.AOFloorPizzaSliceLeftWallRight);
        }
        else
        {
          if (flag3 || !flag1 || flag2 || !flag4)
            return;
          map.Layers[floorLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, this.t.aoTileIndices.AOFloorPizzaSliceRightWallLeft);
        }
      }

      private void BuildShadowIndex(CellData current, Dungeon d, tk2dTileMap map, int ix, int iy)
      {
        if (!this.BCheck(d, ix, iy, -2))
          return;
        CellData cellData1 = d.data.cellData[ix - 1][iy];
        CellData cellData2 = d.data.cellData[ix + 1][iy];
        CellData cellData3 = d.data.cellData[ix][iy + 1];
        CellData cellData4 = d.data.cellData[ix + 1][iy + 1];
        CellData cellData5 = d.data.cellData[ix - 1][iy + 1];
        bool flag1 = cellData1.type == CellType.WALL && cellData1.diagonalWallType == DiagonalWallType.NONE;
        bool flag2 = cellData2.type == CellType.WALL && cellData2.diagonalWallType == DiagonalWallType.NONE;
        bool flag3 = cellData3.type == CellType.WALL;
        bool flag4 = cellData4.type == CellType.WALL && cellData4.diagonalWallType == DiagonalWallType.NONE;
        bool flag5 = cellData5.type == CellType.WALL && cellData5.diagonalWallType == DiagonalWallType.NONE;
        if (current.parentRoom != null && (UnityEngine.Object) current.parentRoom.area.prototypeRoom != (UnityEngine.Object) null && current.parentRoom.area.prototypeRoom.preventFacewallAO)
        {
          flag3 = false;
          flag4 = false;
          flag5 = false;
        }
        bool flag6 = cellData3.isSecretRoomCell != current.isSecretRoomCell;
        if (cellData3.diagonalWallType == DiagonalWallType.NONE)
        {
          if (flag3 && flag1 && !flag2)
            map.Layers[GlobalDungeonData.shadowLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, this.t.aoTileIndices.AOFloorWallUpAndLeft);
          else if (flag3 && flag2 && !flag1)
            map.Layers[GlobalDungeonData.shadowLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, this.t.aoTileIndices.AOFloorWallUpAndRight);
          else if (flag3 && flag1 && flag2)
            map.Layers[GlobalDungeonData.shadowLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, this.t.aoTileIndices.AOFloorWallUpAndBoth);
          else if (flag3 && !flag1 && !flag2)
            map.Layers[GlobalDungeonData.shadowLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, this.t.aoTileIndices.AOFloorTileIndex);
        }
        else if (cellData3.diagonalWallType == DiagonalWallType.NORTHEAST && cellData3.type == CellType.WALL)
        {
          map.Layers[GlobalDungeonData.shadowLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y + 1, this.t.aoTileIndices.AOFloorDiagonalWallNortheast);
          map.Layers[GlobalDungeonData.shadowLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, !flag2 ? this.t.aoTileIndices.AOFloorDiagonalWallNortheastLower : this.t.aoTileIndices.AOFloorDiagonalWallNortheastLowerJoint);
        }
        else if (cellData3.diagonalWallType == DiagonalWallType.NORTHWEST && cellData3.type == CellType.WALL)
        {
          map.Layers[GlobalDungeonData.shadowLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y + 1, this.t.aoTileIndices.AOFloorDiagonalWallNorthwest);
          map.Layers[GlobalDungeonData.shadowLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, !flag1 ? this.t.aoTileIndices.AOFloorDiagonalWallNorthwestLower : this.t.aoTileIndices.AOFloorDiagonalWallNorthwestLowerJoint);
        }
        if (!flag3)
        {
          bool flag7 = flag1 && !d.data.isTopWall(ix - 1, iy + 1);
          bool flag8 = flag2 && !d.data.isTopWall(ix + 1, iy + 1);
          if (flag7 && flag8)
            map.Layers[GlobalDungeonData.shadowLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, this.t.aoTileIndices.AOFloorWallBoth);
          else if (flag7)
            map.Layers[GlobalDungeonData.shadowLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, this.t.aoTileIndices.AOFloorWallLeft);
          else if (flag8)
            map.Layers[GlobalDungeonData.shadowLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, this.t.aoTileIndices.AOFloorWallRight);
        }
        if (!flag3 && flag5 && !flag6 && !flag1 && !flag2 && !flag4)
          map.Layers[GlobalDungeonData.shadowLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, this.t.aoTileIndices.AOFloorPizzaSliceLeft);
        else if (!flag3 && !flag5 && !flag1 && !flag2 && flag4 && !flag6)
          map.Layers[GlobalDungeonData.shadowLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, this.t.aoTileIndices.AOFloorPizzaSliceRight);
        else if (!flag3 && flag5 && !flag6 && !flag2 && !flag1 && flag4 && !flag6)
          map.Layers[GlobalDungeonData.shadowLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, this.t.aoTileIndices.AOFloorPizzaSliceBoth);
        else if (!flag3 && flag5 && !flag6 && !flag1 && flag2)
        {
          map.Layers[GlobalDungeonData.shadowLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, this.t.aoTileIndices.AOFloorPizzaSliceLeftWallRight);
        }
        else
        {
          if (flag3 || !flag1 || flag2 || !flag4 || flag6)
            return;
          map.Layers[GlobalDungeonData.shadowLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, this.t.aoTileIndices.AOFloorPizzaSliceRightWallLeft);
        }
      }

      public void ApplyTileStamp(
        int ix,
        int iy,
        TileStampData tsd,
        Dungeon d,
        tk2dTileMap map,
        int overrideTileLayerIndex = -1)
      {
        DungeonTileStampData.StampSpace occupySpace = tsd.occupySpace;
        for (int index1 = 0; index1 < tsd.width; ++index1)
        {
          for (int index2 = 0; index2 < tsd.height; ++index2)
          {
            CellVisualData cellVisualData = d.data.cellData[ix + index1][iy + index2].cellVisualData;
            switch (occupySpace)
            {
              case DungeonTileStampData.StampSpace.OBJECT_SPACE:
                if (d.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.FORGEGEON)
                {
                  if (cellVisualData.containsObjectSpaceStamp || cellVisualData.containsLight)
                    return;
                  break;
                }
                if (cellVisualData.containsObjectSpaceStamp)
                  return;
                break;
              case DungeonTileStampData.StampSpace.WALL_SPACE:
                if (cellVisualData.containsWallSpaceStamp || cellVisualData.containsLight)
                  return;
                break;
              case DungeonTileStampData.StampSpace.BOTH_SPACES:
                if (cellVisualData.containsObjectSpaceStamp || cellVisualData.containsWallSpaceStamp || cellVisualData.containsLight)
                  return;
                break;
            }
          }
        }
        for (int index3 = 0; index3 < tsd.width; ++index3)
        {
          for (int index4 = 0; index4 < tsd.height; ++index4)
          {
            CellData cellData = d.data.cellData[ix + index3][iy + index4];
            CellVisualData cellVisualData = cellData.cellVisualData;
            int index5 = occupySpace != DungeonTileStampData.StampSpace.OBJECT_SPACE ? GlobalDungeonData.wallStampLayerIndex : GlobalDungeonData.objectStampLayerIndex;
            if (d.data.isFaceWallHigher(ix + index3, iy + index4 - 1))
              index5 = GlobalDungeonData.aboveBorderLayerIndex;
            if (!d.data.isAnyFaceWall(ix + index3, iy + index4) && d.data.cellData[ix + index3][iy + index4].type == CellType.WALL)
              index5 = GlobalDungeonData.aboveBorderLayerIndex;
            if (overrideTileLayerIndex != -1)
              index5 = overrideTileLayerIndex;
            map.Layers[index5].SetTile(cellData.positionInTilemap.x, cellData.positionInTilemap.y, tsd.stampTileIndices[(tsd.height - 1 - index4) * tsd.width + index3]);
            if (occupySpace == DungeonTileStampData.StampSpace.OBJECT_SPACE)
              cellVisualData.containsObjectSpaceStamp = true;
            if (occupySpace == DungeonTileStampData.StampSpace.WALL_SPACE)
              cellVisualData.containsWallSpaceStamp = true;
            if (occupySpace == DungeonTileStampData.StampSpace.BOTH_SPACES)
            {
              cellVisualData.containsObjectSpaceStamp = true;
              cellVisualData.containsWallSpaceStamp = true;
            }
          }
        }
      }

      public void ApplyStampGeneric(
        int ix,
        int iy,
        StampDataBase sd,
        Dungeon d,
        tk2dTileMap map,
        bool flipX = false,
        int overrideTileLayerIndex = -1)
      {
        switch (sd)
        {
          case TileStampData _:
            this.ApplyTileStamp(ix, iy, sd as TileStampData, d, map, overrideTileLayerIndex);
            break;
          case SpriteStampData _:
            this.ApplySpriteStamp(ix, iy, sd as SpriteStampData, d, map);
            break;
          case ObjectStampData _:
            TK2DDungeonAssembler.ApplyObjectStamp(ix, iy, sd as ObjectStampData, d, map, flipX);
            break;
        }
      }

      public static GameObject ApplyObjectStamp(
        int ix,
        int iy,
        ObjectStampData osd,
        Dungeon d,
        tk2dTileMap map,
        bool flipX = false,
        bool isLightStamp = false)
      {
        DungeonTileStampData.StampSpace occupySpace = osd.occupySpace;
        for (int index1 = 0; index1 < osd.width; ++index1)
        {
          for (int index2 = 0; index2 < osd.height; ++index2)
          {
            CellData cellData = d.data.cellData[ix + index1][iy + index2];
            CellVisualData cellVisualData = cellData.cellVisualData;
            if (cellVisualData.forcedMatchingStyle != DungeonTileStampData.IntermediaryMatchingStyle.ANY && cellVisualData.forcedMatchingStyle != osd.intermediaryMatchingStyle)
              return (GameObject) null;
            if (osd.placementRule != DungeonTileStampData.StampPlacementRule.ALONG_LEFT_WALLS || !isLightStamp)
            {
              bool flag = cellVisualData.containsWallSpaceStamp;
              if (cellVisualData.facewallGridPreventsWallSpaceStamp && isLightStamp)
                flag = false;
              switch (occupySpace)
              {
                case DungeonTileStampData.StampSpace.OBJECT_SPACE:
                  if (cellVisualData.containsObjectSpaceStamp)
                    return (GameObject) null;
                  if (cellData.type == CellType.PIT)
                    return (GameObject) null;
                  continue;
                case DungeonTileStampData.StampSpace.WALL_SPACE:
                  if (flag || !isLightStamp && cellVisualData.containsLight)
                    return (GameObject) null;
                  continue;
                case DungeonTileStampData.StampSpace.BOTH_SPACES:
                  if (cellVisualData.containsObjectSpaceStamp || flag || !isLightStamp && cellVisualData.containsLight)
                    return (GameObject) null;
                  if (cellData.type == CellType.PIT)
                    return (GameObject) null;
                  continue;
                default:
                  continue;
              }
            }
          }
        }
        int index3 = occupySpace != DungeonTileStampData.StampSpace.OBJECT_SPACE ? GlobalDungeonData.wallStampLayerIndex : GlobalDungeonData.objectStampLayerIndex;
        float z = map.data.Layers[index3].z;
        Vector3 vector = osd.objectReference.transform.position;
        ObjectStampOptions component1 = osd.objectReference.GetComponent<ObjectStampOptions>();
        if ((UnityEngine.Object) component1 != (UnityEngine.Object) null)
          vector = component1.GetPositionOffset();
        GameObject gObj = UnityEngine.Object.Instantiate<GameObject>(osd.objectReference);
        gObj.transform.position = new Vector3((float) ix, (float) iy, z) + vector;
        if (!isLightStamp && osd.placementRule == DungeonTileStampData.StampPlacementRule.ALONG_LEFT_WALLS)
          gObj.transform.position = new Vector3((float) (ix + 1), (float) iy, z) + vector.WithX(-vector.x);
        tk2dSprite component2 = gObj.GetComponent<tk2dSprite>();
        RoomHandler roomFromPosition = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(new IntVector2(ix, iy));
        MinorBreakable componentInChildren = gObj.GetComponentInChildren<MinorBreakable>();
        if ((bool) (UnityEngine.Object) componentInChildren)
        {
          if (osd.placementRule == DungeonTileStampData.StampPlacementRule.ON_ANY_FLOOR)
            componentInChildren.IgnoredForPotShotsModifier = true;
          componentInChildren.IsDecorativeOnly = true;
        }
        gObj.GetInterface<IPlaceConfigurable>()?.ConfigureOnPlacement(roomFromPosition);
        SurfaceDecorator component3 = gObj.GetComponent<SurfaceDecorator>();
        if ((UnityEngine.Object) component3 != (UnityEngine.Object) null)
          component3.Decorate(roomFromPosition);
        if (flipX)
        {
          if ((UnityEngine.Object) component2 != (UnityEngine.Object) null)
          {
            component2.FlipX = true;
            float x = Mathf.Ceil(component2.GetBounds().size.x);
            gObj.transform.position += new Vector3(x, 0.0f, 0.0f);
          }
          else
            gObj.transform.localScale = Vector3.Scale(gObj.transform.localScale, new Vector3(-1f, 1f, 1f));
        }
        gObj.transform.parent = roomFromPosition == null ? (Transform) null : roomFromPosition.hierarchyParent;
        DepthLookupManager.ProcessRenderer(gObj.GetComponentInChildren<Renderer>());
        if ((UnityEngine.Object) component2 != (UnityEngine.Object) null)
          component2.UpdateZDepth();
        for (int index4 = 0; index4 < osd.width; ++index4)
        {
          for (int index5 = 0; index5 < osd.height; ++index5)
          {
            CellVisualData cellVisualData = d.data.cellData[ix + index4][iy + index5].cellVisualData;
            if (occupySpace == DungeonTileStampData.StampSpace.OBJECT_SPACE)
              cellVisualData.containsObjectSpaceStamp = true;
            if (occupySpace == DungeonTileStampData.StampSpace.WALL_SPACE)
              cellVisualData.containsWallSpaceStamp = true;
            if (occupySpace == DungeonTileStampData.StampSpace.BOTH_SPACES)
            {
              cellVisualData.containsObjectSpaceStamp = true;
              cellVisualData.containsWallSpaceStamp = true;
            }
          }
        }
        return gObj;
      }

      public void ApplySpriteStamp(int ix, int iy, SpriteStampData ssd, Dungeon d, tk2dTileMap map)
      {
        DungeonTileStampData.StampSpace occupySpace = ssd.occupySpace;
        for (int index1 = 0; index1 < ssd.width; ++index1)
        {
          for (int index2 = 0; index2 < ssd.height; ++index2)
          {
            CellVisualData cellVisualData = d.data.cellData[ix + index1][iy + index2].cellVisualData;
            switch (occupySpace)
            {
              case DungeonTileStampData.StampSpace.OBJECT_SPACE:
                if (cellVisualData.containsObjectSpaceStamp)
                  return;
                break;
              case DungeonTileStampData.StampSpace.WALL_SPACE:
                if (cellVisualData.containsWallSpaceStamp)
                  return;
                break;
              case DungeonTileStampData.StampSpace.BOTH_SPACES:
                if (cellVisualData.containsObjectSpaceStamp || cellVisualData.containsWallSpaceStamp)
                  return;
                break;
            }
          }
        }
        int index3 = occupySpace != DungeonTileStampData.StampSpace.OBJECT_SPACE ? GlobalDungeonData.wallStampLayerIndex : GlobalDungeonData.objectStampLayerIndex;
        float z = map.data.Layers[index3].z;
        SpriteRenderer r = new GameObject(ssd.spriteReference.name)
        {
          transform = {
            position = new Vector3((float) ix, (float) iy, z)
          }
        }.AddComponent<SpriteRenderer>();
        r.sprite = ssd.spriteReference;
        DepthLookupManager.ProcessRenderer((Renderer) r);
        for (int index4 = 0; index4 < ssd.width; ++index4)
        {
          for (int index5 = 0; index5 < ssd.height; ++index5)
          {
            CellVisualData cellVisualData = d.data.cellData[ix + index4][iy + index5].cellVisualData;
            if (occupySpace == DungeonTileStampData.StampSpace.OBJECT_SPACE)
              cellVisualData.containsObjectSpaceStamp = true;
            if (occupySpace == DungeonTileStampData.StampSpace.WALL_SPACE)
              cellVisualData.containsWallSpaceStamp = true;
            if (occupySpace == DungeonTileStampData.StampSpace.BOTH_SPACES)
            {
              cellVisualData.containsObjectSpaceStamp = true;
              cellVisualData.containsWallSpaceStamp = true;
            }
          }
        }
      }

      private TileIndexGrid GetCeilingBorderIndexGrid(CellData current, Dungeon d)
      {
        TileIndexGrid ceilingBorderGrid = d.roomMaterialDefinitions[current.cellVisualData.roomVisualTypeIndex].roomCeilingBorderGrid;
        if ((UnityEngine.Object) ceilingBorderGrid == (UnityEngine.Object) null)
          ceilingBorderGrid = d.roomMaterialDefinitions[0].roomCeilingBorderGrid;
        return ceilingBorderGrid;
      }

      private int GetCeilingCenterIndex(CellData current, TileIndexGrid gridToUse)
      {
        if (!gridToUse.CeilingBorderUsesDistancedCenters)
          return gridToUse.centerIndices.GetIndexByWeight();
        int count = gridToUse.centerIndices.indices.Count;
        int index = Mathf.Max(0, Mathf.Min(Mathf.FloorToInt(current.distanceFromNearestRoom) - 1, count - 1));
        return gridToUse.centerIndices.indices[index];
      }

      private void BuildOuterBorderIndex(
        CellData current,
        Dungeon d,
        tk2dTileMap map,
        int ix,
        int iy)
      {
        bool isNorthBorder = (d.data.isWall(ix, iy + 1) || d.data.isTopWall(ix, iy + 1)) && !d.data.isAnyFaceWall(ix, iy + 1);
        bool isNortheastBorder = (d.data.isWall(ix + 1, iy + 1) || d.data.isTopWall(ix + 1, iy + 1)) && !d.data.isAnyFaceWall(ix + 1, iy + 1);
        bool isEastBorder = (d.data.isWall(ix + 1, iy) || d.data.isTopWall(ix + 1, iy)) && !d.data.isAnyFaceWall(ix + 1, iy);
        bool isSoutheastBorder = (d.data.isWall(ix + 1, iy - 1) || d.data.isTopWall(ix + 1, iy - 1)) && !d.data.isAnyFaceWall(ix + 1, iy - 1);
        bool isSouthBorder = (d.data.isWall(ix, iy - 1) || d.data.isTopWall(ix, iy - 1)) && !d.data.isAnyFaceWall(ix, iy - 1);
        bool isSouthwestBorder = (d.data.isWall(ix - 1, iy - 1) || d.data.isTopWall(ix - 1, iy - 1)) && !d.data.isAnyFaceWall(ix - 1, iy - 1);
        bool isWestBorder = (d.data.isWall(ix - 1, iy) || d.data.isTopWall(ix - 1, iy)) && !d.data.isAnyFaceWall(ix - 1, iy);
        bool isNorthwestBorder = (d.data.isWall(ix - 1, iy + 1) || d.data.isTopWall(ix - 1, iy + 1)) && !d.data.isAnyFaceWall(ix - 1, iy + 1);
        int indexGivenSides = d.roomMaterialDefinitions[current.cellVisualData.roomVisualTypeIndex].outerCeilingBorderGrid.GetIndexGivenSides(isNorthBorder, isNortheastBorder, isEastBorder, isSoutheastBorder, isSouthBorder, isSouthwestBorder, isWestBorder, isNorthwestBorder);
        if (indexGivenSides == -1 || current.cellVisualData.shouldIgnoreWallDrawing)
          return;
        map.Layers[GlobalDungeonData.borderLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, indexGivenSides);
      }

      private bool IsCardinalBorder(CellData current, Dungeon d, int ix, int iy)
      {
        bool flag1 = d.data.isTopWall(ix, iy) && !d.data[ix, iy + 1].cellVisualData.shouldIgnoreBorders;
        bool flag2 = (!d.data.isWallRight(ix, iy) && !d.data.isRightTopWall(ix, iy) || d.data.isFaceWallHigher(ix + 1, iy) || d.data.isFaceWallLower(ix + 1, iy)) && !d.data[ix + 1, iy].cellVisualData.shouldIgnoreBorders;
        bool flag3 = iy > 3 && d.data.isFaceWallHigher(ix, iy - 1) && !d.data[ix, iy - 1].cellVisualData.shouldIgnoreBorders;
        bool flag4 = (!d.data.isWallLeft(ix, iy) && !d.data.isLeftTopWall(ix, iy) || d.data.isFaceWallHigher(ix - 1, iy) || d.data.isFaceWallLower(ix - 1, iy)) && !d.data[ix - 1, iy].cellVisualData.shouldIgnoreBorders;
        return flag1 || flag2 || flag3 || flag4;
      }

      private TileIndexGrid GetTypeBorderGridForBorderIndex(
        CellData current,
        Dungeon d,
        out int usedVisualType)
      {
        TileIndexGrid ceilingBorderGrid = d.roomMaterialDefinitions[current.cellVisualData.roomVisualTypeIndex].roomCeilingBorderGrid;
        usedVisualType = current.cellVisualData.roomVisualTypeIndex;
        if (d.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.WESTGEON)
        {
          if (current.nearestRoom != null && (double) current.distanceFromNearestRoom < 4.0)
          {
            if (current.cellVisualData.IsFacewallForInteriorTransition)
            {
              ceilingBorderGrid = d.roomMaterialDefinitions[current.cellVisualData.InteriorTransitionIndex].roomCeilingBorderGrid;
              usedVisualType = current.cellVisualData.InteriorTransitionIndex;
            }
            else if (!current.cellVisualData.IsFeatureCell)
            {
              ceilingBorderGrid = d.roomMaterialDefinitions[current.nearestRoom.RoomVisualSubtype].roomCeilingBorderGrid;
              usedVisualType = current.nearestRoom.RoomVisualSubtype;
            }
          }
        }
        else if (d.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.OFFICEGEON)
        {
          ceilingBorderGrid = d.roomMaterialDefinitions[current.nearestRoom.RoomVisualSubtype].roomCeilingBorderGrid;
          usedVisualType = current.nearestRoom.RoomVisualSubtype;
        }
        if ((UnityEngine.Object) ceilingBorderGrid == (UnityEngine.Object) null)
        {
          ceilingBorderGrid = d.roomMaterialDefinitions[0].roomCeilingBorderGrid;
          usedVisualType = 0;
        }
        return ceilingBorderGrid;
      }

      private void BuildOcclusionPartitionIndex(
        CellData current,
        Dungeon d,
        tk2dTileMap map,
        int ix,
        int iy)
      {
        if (current == null || current.cellVisualData.ceilingHasBeenProcessed || current.cellVisualData.occlusionHasBeenProcessed)
          return;
        int usedVisualType1 = -1;
        TileIndexGrid gridForBorderIndex = this.GetTypeBorderGridForBorderIndex(current, d, out usedVisualType1);
        if (!((UnityEngine.Object) gridForBorderIndex != (UnityEngine.Object) null))
          return;
        List<CellData> cellNeighbors = d.data.GetCellNeighbors(current, true);
        bool[] eightSides = new bool[8];
        int usedVisualType2 = -1;
        for (int index = 0; index < eightSides.Length; ++index)
        {
          if (cellNeighbors[index] != null)
          {
            this.GetTypeBorderGridForBorderIndex(cellNeighbors[index], d, out usedVisualType2);
            if (d.tileIndices.tilesetId != GlobalDungeonData.ValidTilesets.WESTGEON || usedVisualType2 == 0 || usedVisualType1 == 0)
              eightSides[index] = usedVisualType1 != usedVisualType2;
          }
        }
        int tile = gridForBorderIndex.GetIndexGivenEightSides(eightSides);
        if (tile == -1)
          tile = gridForBorderIndex.centerIndices.GetIndexByWeight();
        map.SetTile(current.positionInTilemap.x, current.positionInTilemap.y, GlobalDungeonData.occlusionPartitionIndex, tile);
      }

      private bool IsBorderCell(Dungeon d, int ix, int iy)
      {
        bool flag1 = d.data[ix, iy + 1].diagonalWallType != DiagonalWallType.NONE && (d.data[ix, iy + 1].IsTopWall() || d.data[ix, iy + 1].type == CellType.WALL);
        bool flag2 = d.data[ix + 1, iy].diagonalWallType != DiagonalWallType.NONE && (d.data[ix + 1, iy].IsTopWall() || d.data[ix + 1, iy].type == CellType.WALL);
        bool flag3 = d.data[ix, iy - 1].diagonalWallType != DiagonalWallType.NONE && (d.data[ix, iy - 1].IsTopWall() || d.data[ix, iy - 1].type == CellType.WALL);
        bool flag4 = d.data[ix - 1, iy].diagonalWallType != DiagonalWallType.NONE && (d.data[ix - 1, iy].IsTopWall() || d.data[ix - 1, iy].type == CellType.WALL);
        bool flag5 = d.data.isTopWall(ix, iy) && !d.data[ix, iy + 1].cellVisualData.shouldIgnoreBorders;
        bool flag6 = (!d.data.isWallRight(ix, iy) && !d.data.isRightTopWall(ix, iy) || d.data.isFaceWallHigher(ix + 1, iy) || d.data.isFaceWallLower(ix + 1, iy)) && !d.data[ix + 1, iy].cellVisualData.shouldIgnoreBorders;
        bool flag7 = iy > 3 && d.data.isFaceWallHigher(ix, iy - 1) && !d.data[ix, iy - 1].cellVisualData.shouldIgnoreBorders;
        bool flag8 = (!d.data.isWallLeft(ix, iy) && !d.data.isLeftTopWall(ix, iy) || d.data.isFaceWallHigher(ix - 1, iy) || d.data.isFaceWallLower(ix - 1, iy)) && !d.data[ix - 1, iy].cellVisualData.shouldIgnoreBorders;
        bool flag9 = (!flag1 || !flag2) && d.data.isTopWall(ix + 1, iy) && !d.data.isTopWall(ix, iy) && (d.data.isWall(ix, iy + 1) || d.data.isTopWall(ix, iy + 1)) && !d.data[ix + 1, iy + 1].cellVisualData.shouldIgnoreBorders;
        bool flag10 = (!flag1 || !flag4) && d.data.isTopWall(ix - 1, iy) && !d.data.isTopWall(ix, iy) && (d.data.isWall(ix, iy + 1) || d.data.isTopWall(ix, iy + 1)) && !d.data[ix - 1, iy + 1].cellVisualData.shouldIgnoreBorders;
        bool flag11 = (!flag3 || !flag2) && iy > 3 && d.data.isFaceWallHigher(ix + 1, iy - 1) && !d.data.isFaceWallHigher(ix, iy - 1) && !d.data[ix + 1, iy - 1].cellVisualData.shouldIgnoreBorders;
        bool flag12 = (!flag3 || !flag4) && iy > 3 && d.data.isFaceWallHigher(ix - 1, iy - 1) && !d.data.isFaceWallHigher(ix, iy - 1) && !d.data[ix - 1, iy - 1].cellVisualData.shouldIgnoreBorders;
        return flag5 || flag6 || flag8 || flag7 || flag9 || flag10 || flag11 || flag12;
      }

      private void HandleRatChunkOverhangs(Dungeon d, int ix, int iy, tk2dTileMap map)
      {
        bool flag1 = this.IsBorderCell(d, ix, iy + 1);
        bool flag2 = this.IsBorderCell(d, ix + 1, iy);
        bool flag3 = this.IsBorderCell(d, ix, iy - 1);
        bool flag4 = this.IsBorderCell(d, ix - 1, iy);
        if ((!flag1 || !flag2) && (!flag2 || !flag3) && (!flag3 || !flag4) && (!flag4 || !flag1))
          return;
        if (!flag1)
        {
          map.Layers[GlobalDungeonData.borderLayerIndex].SetTile(d.data[ix, iy + 1].positionInTilemap.x, d.data[ix, iy + 1].positionInTilemap.y, 312);
          d.data[ix, iy + 1].cellVisualData.ceilingHasBeenProcessed = true;
        }
        if (!flag2)
        {
          map.Layers[GlobalDungeonData.borderLayerIndex].SetTile(d.data[ix + 1, iy].positionInTilemap.x, d.data[ix + 1, iy].positionInTilemap.y, 315);
          d.data[ix + 1, iy].cellVisualData.ceilingHasBeenProcessed = true;
        }
        if (!flag3)
          map.Layers[GlobalDungeonData.borderLayerIndex].SetTile(d.data[ix, iy - 1].positionInTilemap.x, d.data[ix, iy - 1].positionInTilemap.y, 313);
        if (flag4)
          return;
        map.Layers[GlobalDungeonData.borderLayerIndex].SetTile(d.data[ix - 1, iy].positionInTilemap.x, d.data[ix - 1, iy].positionInTilemap.y, 314);
      }

      private void BuildBorderIndex(CellData current, Dungeon d, tk2dTileMap map, int ix, int iy)
      {
        if (current.cellVisualData.ceilingHasBeenProcessed)
          return;
        bool flag1 = d.data[ix, iy + 1] != null && d.data[ix, iy + 1].diagonalWallType != DiagonalWallType.NONE && (d.data[ix, iy + 1].IsTopWall() || d.data[ix, iy + 1].type == CellType.WALL);
        bool flag2 = d.data[ix + 1, iy] != null && d.data[ix + 1, iy].diagonalWallType != DiagonalWallType.NONE && (d.data[ix + 1, iy].IsTopWall() || d.data[ix + 1, iy].type == CellType.WALL);
        bool flag3 = d.data[ix, iy - 1] != null && d.data[ix, iy - 1].diagonalWallType != DiagonalWallType.NONE && (d.data[ix, iy - 1].IsTopWall() || d.data[ix, iy - 1].type == CellType.WALL);
        bool flag4 = d.data[ix - 1, iy] != null && d.data[ix - 1, iy].diagonalWallType != DiagonalWallType.NONE && (d.data[ix - 1, iy].IsTopWall() || d.data[ix - 1, iy].type == CellType.WALL);
        bool isNorthBorder = d.data.isTopWall(ix, iy) && d.data[ix, iy + 1] != null && !d.data[ix, iy + 1].cellVisualData.shouldIgnoreBorders;
        bool isEastBorder = (!d.data.isWallRight(ix, iy) && !d.data.isRightTopWall(ix, iy) || d.data.isFaceWallHigher(ix + 1, iy) || d.data.isFaceWallLower(ix + 1, iy)) && d.data[ix + 1, iy] != null && !d.data[ix + 1, iy].cellVisualData.shouldIgnoreBorders;
        bool isSouthBorder = iy > 3 && d.data.isFaceWallHigher(ix, iy - 1) && d.data[ix, iy - 1] != null && !d.data[ix, iy - 1].cellVisualData.shouldIgnoreBorders;
        bool isWestBorder = (!d.data.isWallLeft(ix, iy) && !d.data.isLeftTopWall(ix, iy) || d.data.isFaceWallHigher(ix - 1, iy) || d.data.isFaceWallLower(ix - 1, iy)) && d.data[ix - 1, iy] != null && !d.data[ix - 1, iy].cellVisualData.shouldIgnoreBorders;
        bool isNortheastBorder = (!flag1 || !flag2) && d.data.isTopWall(ix + 1, iy) && !d.data.isTopWall(ix, iy) && (d.data.isWall(ix, iy + 1) || d.data.isTopWall(ix, iy + 1)) && d.data[ix + 1, iy + 1] != null && !d.data[ix + 1, iy + 1].cellVisualData.shouldIgnoreBorders;
        bool isNorthwestBorder = (!flag1 || !flag4) && d.data.isTopWall(ix - 1, iy) && !d.data.isTopWall(ix, iy) && (d.data.isWall(ix, iy + 1) || d.data.isTopWall(ix, iy + 1)) && d.data[ix - 1, iy + 1] != null && !d.data[ix - 1, iy + 1].cellVisualData.shouldIgnoreBorders;
        bool isSoutheastBorder = (!flag3 || !flag2) && iy > 3 && d.data.isFaceWallHigher(ix + 1, iy - 1) && !d.data.isFaceWallHigher(ix, iy - 1) && d.data[ix + 1, iy - 1] != null && !d.data[ix + 1, iy - 1].cellVisualData.shouldIgnoreBorders;
        bool isSouthwestBorder = (!flag3 || !flag4) && iy > 3 && d.data.isFaceWallHigher(ix - 1, iy - 1) && !d.data.isFaceWallHigher(ix, iy - 1) && d.data[ix - 1, iy - 1] != null && !d.data[ix - 1, iy - 1].cellVisualData.shouldIgnoreBorders;
        int tile1 = -1;
        int usedVisualType1 = -1;
        TileIndexGrid gridForBorderIndex1 = this.GetTypeBorderGridForBorderIndex(current, d, out usedVisualType1);
        if (d.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.WESTGEON)
        {
          int usedVisualType2 = -1;
          if (!isNorthBorder)
            isNorthBorder = (UnityEngine.Object) gridForBorderIndex1 != (UnityEngine.Object) this.GetTypeBorderGridForBorderIndex(d.data[current.position + IntVector2.North], d, out usedVisualType2) && (usedVisualType2 == 0 || usedVisualType1 == 0);
          if (!isNortheastBorder)
            isNortheastBorder = (UnityEngine.Object) gridForBorderIndex1 != (UnityEngine.Object) this.GetTypeBorderGridForBorderIndex(d.data[current.position + IntVector2.NorthEast], d, out usedVisualType2) && (usedVisualType2 == 0 || usedVisualType1 == 0);
          if (!isEastBorder)
            isEastBorder = (UnityEngine.Object) gridForBorderIndex1 != (UnityEngine.Object) this.GetTypeBorderGridForBorderIndex(d.data[current.position + IntVector2.East], d, out usedVisualType2) && (usedVisualType2 == 0 || usedVisualType1 == 0);
          if (!isSoutheastBorder)
            isSoutheastBorder = (UnityEngine.Object) gridForBorderIndex1 != (UnityEngine.Object) this.GetTypeBorderGridForBorderIndex(d.data[current.position + IntVector2.SouthEast], d, out usedVisualType2) && (usedVisualType2 == 0 || usedVisualType1 == 0);
          if (!isSouthBorder)
            isSouthBorder = (UnityEngine.Object) gridForBorderIndex1 != (UnityEngine.Object) this.GetTypeBorderGridForBorderIndex(d.data[current.position + IntVector2.South], d, out usedVisualType2) && (usedVisualType2 == 0 || usedVisualType1 == 0);
          if (!isSouthwestBorder)
            isSouthwestBorder = (UnityEngine.Object) gridForBorderIndex1 != (UnityEngine.Object) this.GetTypeBorderGridForBorderIndex(d.data[current.position + IntVector2.SouthWest], d, out usedVisualType2) && (usedVisualType2 == 0 || usedVisualType1 == 0);
          if (!isWestBorder)
            isWestBorder = (UnityEngine.Object) gridForBorderIndex1 != (UnityEngine.Object) this.GetTypeBorderGridForBorderIndex(d.data[current.position + IntVector2.West], d, out usedVisualType2) && (usedVisualType2 == 0 || usedVisualType1 == 0);
          if (!isNorthwestBorder)
            isNorthwestBorder = (UnityEngine.Object) gridForBorderIndex1 != (UnityEngine.Object) this.GetTypeBorderGridForBorderIndex(d.data[current.position + IntVector2.NorthWest], d, out usedVisualType2) && (usedVisualType2 == 0 || usedVisualType1 == 0);
        }
        if (current.diagonalWallType == DiagonalWallType.NONE)
        {
          if (!isNorthBorder && !isNortheastBorder && !isEastBorder && !isSoutheastBorder && !isSouthBorder && !isSouthwestBorder && !isWestBorder && !isNorthwestBorder)
          {
            if (d.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.JUNGLEGEON)
            {
              this.BuildBorderLayerCenterJungle(current, d, map, ix, iy);
              tile1 = -1;
            }
            else if (gridForBorderIndex1.CeilingBorderUsesDistancedCenters)
            {
              int count = gridForBorderIndex1.centerIndices.indices.Count;
              int index = Mathf.Max(0, Mathf.Min(Mathf.FloorToInt(current.distanceFromNearestRoom) - 1, count - 1));
              tile1 = gridForBorderIndex1.centerIndices.indices[index];
            }
            else
            {
              tile1 = gridForBorderIndex1.centerIndices.GetIndexByWeight();
              if (d.tileIndices.globalSecondBorderTiles.Count > 0 && (double) current.distanceFromNearestRoom < 3.0 && (double) UnityEngine.Random.value > 0.5)
                tile1 = d.tileIndices.globalSecondBorderTiles[UnityEngine.Random.Range(0, d.tileIndices.globalSecondBorderTiles.Count)];
            }
          }
          else if (gridForBorderIndex1.UsesRatChunkBorders)
          {
            bool isTwoSouthEmpty = iy > 3;
            if (isTwoSouthEmpty)
              isTwoSouthEmpty = !d.data[ix, iy - 1].HasFloorNeighbor(d.data, includeDiagonals: true);
            TileIndexGrid.RatChunkResult result = TileIndexGrid.RatChunkResult.NONE;
            tile1 = !d.data[ix, iy].nearestRoom.area.PrototypeLostWoodsRoom ? gridForBorderIndex1.GetRatChunkIndexGivenSides(isNorthBorder, isNortheastBorder, isEastBorder, isSoutheastBorder, isSouthBorder, isSouthwestBorder, isWestBorder, isNorthwestBorder, isTwoSouthEmpty, out result) : gridForBorderIndex1.GetRatChunkIndexGivenSidesStatic(isNorthBorder, isNortheastBorder, isEastBorder, isSoutheastBorder, isSouthBorder, isSouthwestBorder, isWestBorder, isNorthwestBorder, isTwoSouthEmpty, out result);
            if (result == TileIndexGrid.RatChunkResult.CORNER)
              this.HandleRatChunkOverhangs(d, ix, iy, map);
          }
          else
            tile1 = gridForBorderIndex1.GetIndexGivenSides(isNorthBorder, isNortheastBorder, isEastBorder, isSoutheastBorder, isSouthBorder, isSouthwestBorder, isWestBorder, isNorthwestBorder);
        }
        else
        {
          switch (current.diagonalWallType)
          {
            case DiagonalWallType.NORTHEAST:
              if (isSouthBorder && isWestBorder)
              {
                tile1 = gridForBorderIndex1.diagonalBorderNE.GetIndexByWeight();
                break;
              }
              break;
            case DiagonalWallType.SOUTHEAST:
              tile1 = !isNorthBorder || !isWestBorder ? gridForBorderIndex1.GetIndexGivenSides(isNorthBorder, isNortheastBorder, isEastBorder, isSoutheastBorder, isSouthBorder, isSouthwestBorder, isWestBorder, isNorthwestBorder) : gridForBorderIndex1.diagonalBorderSE.GetIndexByWeight();
              break;
            case DiagonalWallType.SOUTHWEST:
              tile1 = !isNorthBorder || !isEastBorder ? gridForBorderIndex1.GetIndexGivenSides(isNorthBorder, isNortheastBorder, isEastBorder, isSoutheastBorder, isSouthBorder, isSouthwestBorder, isWestBorder, isNorthwestBorder) : gridForBorderIndex1.diagonalBorderSW.GetIndexByWeight();
              break;
            case DiagonalWallType.NORTHWEST:
              if (isSouthBorder && isEastBorder)
              {
                tile1 = gridForBorderIndex1.diagonalBorderNW.GetIndexByWeight();
                break;
              }
              break;
          }
        }
        TileIndexGrid gridForBorderIndex2 = this.GetTypeBorderGridForBorderIndex(current, d, out usedVisualType1);
        if (tile1 == -1)
          return;
        if (!current.cellVisualData.shouldIgnoreWallDrawing)
          map.Layers[GlobalDungeonData.borderLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, tile1);
        if (current.cellVisualData.shouldIgnoreWallDrawing)
          BraveUtility.DrawDebugSquare(current.position.ToVector2(), Color.blue, 1000f);
        if (isNorthBorder && current.diagonalWallType != DiagonalWallType.NONE)
        {
          int tile2 = -1;
          switch (current.diagonalWallType)
          {
            case DiagonalWallType.SOUTHEAST:
              tile2 = gridForBorderIndex2.diagonalCeilingSE.GetIndexByWeight();
              break;
            case DiagonalWallType.SOUTHWEST:
              tile2 = gridForBorderIndex2.diagonalCeilingSW.GetIndexByWeight();
              break;
          }
          if (tile2 != -1)
          {
            map.Layers[GlobalDungeonData.ceilingLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, tile2);
            this.AssignColorOverrideToTile(current.positionInTilemap.x, current.positionInTilemap.y, GlobalDungeonData.ceilingLayerIndex, (Color32) new Color(1f, 1f, 1f, 0.0f), map);
          }
          int ceilingCenterIndex = this.GetCeilingCenterIndex(current, gridForBorderIndex2);
          map.Layers[GlobalDungeonData.ceilingLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y - 1, ceilingCenterIndex);
          this.AssignColorOverrideToTile(current.positionInTilemap.x, current.positionInTilemap.y - 1, GlobalDungeonData.ceilingLayerIndex, (Color32) new Color(1f, 1f, 1f, 0.0f), map);
        }
        else if (isNorthBorder)
        {
          int ceilingCenterIndex = this.GetCeilingCenterIndex(current, gridForBorderIndex2);
          map.Layers[GlobalDungeonData.ceilingLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, ceilingCenterIndex);
          this.AssignColorOverrideToTile(current.positionInTilemap.x, current.positionInTilemap.y, GlobalDungeonData.ceilingLayerIndex, (Color32) new Color(1f, 1f, 1f, 0.0f), map);
        }
        else if (isSouthBorder && current.diagonalWallType != DiagonalWallType.NONE)
        {
          int tile3 = -1;
          switch (current.diagonalWallType)
          {
            case DiagonalWallType.NORTHEAST:
              tile3 = gridForBorderIndex2.diagonalCeilingNE.GetIndexByWeight();
              break;
            case DiagonalWallType.NORTHWEST:
              tile3 = gridForBorderIndex2.diagonalCeilingNW.GetIndexByWeight();
              break;
          }
          if (tile3 != -1)
          {
            map.Layers[GlobalDungeonData.ceilingLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, tile3);
            this.AssignColorOverrideToTile(current.positionInTilemap.x, current.positionInTilemap.y, GlobalDungeonData.ceilingLayerIndex, (Color32) new Color(1f, 1f, 1f, 0.0f), map);
          }
        }
        else if (isEastBorder || isWestBorder || isNortheastBorder || isNorthwestBorder || isSouthBorder || isSoutheastBorder || isSouthwestBorder)
        {
          int ceilingCenterIndex = this.GetCeilingCenterIndex(current, gridForBorderIndex2);
          map.Layers[GlobalDungeonData.ceilingLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, ceilingCenterIndex);
          this.AssignColorOverrideToTile(current.positionInTilemap.x, current.positionInTilemap.y, GlobalDungeonData.ceilingLayerIndex, (Color32) new Color(1f, 1f, 1f, 0.0f), map);
        }
        if (isNorthBorder || d.data[current.position + IntVector2.Up] != null && d.data[current.position + IntVector2.Up].IsTopWall())
          this.AssignColorOverrideToTile(current.positionInTilemap.x, current.positionInTilemap.y, GlobalDungeonData.borderLayerIndex, (Color32) new Color(1f, 1f, 1f, 0.0f), map);
        else
          this.AssignColorOverrideToTile(current.positionInTilemap.x, current.positionInTilemap.y, GlobalDungeonData.borderLayerIndex, (Color32) new Color(0.0f, 0.0f, 0.0f), map);
      }

      private bool ProcessFacewallNeighborMetadata(
        int ix,
        int iy,
        Dungeon d,
        List<IndexNeighborDependency> neighborDependencies,
        bool preventWallStamping = false)
      {
        bool flag1 = d.data.isFaceWallLower(ix, iy);
        d.data[ix, iy].cellVisualData.containsWallSpaceStamp |= preventWallStamping;
        bool flag2 = true;
        List<CellData> cellDataList = new List<CellData>();
        for (int index = 0; index < neighborDependencies.Count; ++index)
        {
          CellData cellData = d.data[new IntVector2(ix, iy) + DungeonData.GetIntVector2FromDirection(neighborDependencies[index].neighborDirection)];
          if (cellData.cellVisualData.faceWallOverrideIndex != -1 || !cellData.IsAnyFaceWall())
          {
            flag2 = false;
            break;
          }
          if (cellData.cellVisualData.roomVisualTypeIndex != d.data.cellData[ix][iy].cellVisualData.roomVisualTypeIndex)
          {
            flag2 = false;
            break;
          }
          if (cellData.position.y == iy && d.data.isFaceWallLower(cellData.position.x, cellData.position.y) != flag1)
          {
            flag2 = false;
            break;
          }
          cellDataList.Add(cellData);
          cellData.cellVisualData.containsWallSpaceStamp |= preventWallStamping;
          cellData.cellVisualData.faceWallOverrideIndex = neighborDependencies[index].neighborIndex;
        }
        if (!flag2)
        {
          for (int index = 0; index < cellDataList.Count; ++index)
            cellDataList[index].cellVisualData.faceWallOverrideIndex = -1;
        }
        return flag2;
      }

      private bool FaceWallTypesMatch(CellData c1, CellData c2)
      {
        return c1.IsLowerFaceWall() && c2.IsLowerFaceWall() || c1.IsUpperFacewall() && c2.IsUpperFacewall();
      }

      private bool IsNorthernmostColumnarFacewall(CellData current, Dungeon d, int ix, int iy)
      {
        for (CellData cellData = d.data[ix, iy + 1]; cellData != null && cellData.nearestRoom == current.nearestRoom; cellData = d.data[cellData.position.x, cellData.position.y + 1])
        {
          if (cellData.type == CellType.FLOOR)
            return false;
          if (!d.data.CheckInBounds(cellData.position.x, cellData.position.y + 1))
            return true;
        }
        return true;
      }

      private void ProcessFacewallType(
        CellData current,
        Dungeon d,
        tk2dTileMap map,
        int ix,
        int iy,
        TilesetIndexMetadata.TilesetFlagType wallType,
        TilesetIndexMetadata.TilesetFlagType tileOverrideType)
      {
        int roomTypeIndex = current.cellVisualData.roomVisualTypeIndex;
        if (d.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.WESTGEON && roomTypeIndex == 0)
        {
          bool flag = false;
          int num = -1;
          for (int index = 0; index < current.nearestRoom.connectedRooms.Count; ++index)
          {
            if (current.nearestRoom.GetDirectionToConnectedRoom(current.nearestRoom.connectedRooms[index]) == DungeonData.Direction.NORTH && current.nearestRoom.connectedRooms[index].RoomVisualSubtype != 0)
            {
              flag = true;
              num = current.nearestRoom.connectedRooms[index].RoomVisualSubtype;
              break;
            }
          }
          if (flag && current.cellVisualData.IsFacewallForInteriorTransition)
            roomTypeIndex = num;
        }
        CellData cellData1 = d.data.cellData[ix + 1][iy];
        CellData cellData2 = d.data.cellData[ix - 1][iy];
        if (current.cellVisualData.faceWallOverrideIndex != -1)
        {
          List<IndexNeighborDependency> dependencies = d.tileIndices.dungeonCollection.GetDependencies(current.cellVisualData.faceWallOverrideIndex);
          if (dependencies != null && dependencies.Count > 0 && current.IsUpperFacewall())
          {
            for (int index = 0; index < dependencies.Count; ++index)
            {
              if (dependencies[index].neighborDirection == DungeonData.Direction.NORTH)
              {
                d.data.cellData[ix][iy + 1].cellVisualData.UsesCustomIndexOverride01 = true;
                d.data.cellData[ix][iy + 1].cellVisualData.CustomIndexOverride01 = dependencies[index].neighborIndex;
                d.data.cellData[ix][iy + 1].cellVisualData.CustomIndexOverride01Layer = GlobalDungeonData.borderLayerIndex;
              }
            }
          }
          map.Layers[GlobalDungeonData.collisionLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, current.cellVisualData.faceWallOverrideIndex);
        }
        else
        {
          if (current.diagonalWallType != DiagonalWallType.NONE)
          {
            int index = -1;
            if (current.diagonalWallType == DiagonalWallType.NORTHEAST)
            {
              if (wallType == TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER)
              {
                this.GetMetadataFromTupleArray(current, this.m_metadataLookupTable[TilesetIndexMetadata.TilesetFlagType.DIAGONAL_FACEWALL_LOWER_NE], roomTypeIndex, out index);
                map.Layers[GlobalDungeonData.collisionLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, index);
              }
              else if (wallType == TilesetIndexMetadata.TilesetFlagType.FACEWALL_UPPER)
              {
                this.GetMetadataFromTupleArray(current, this.m_metadataLookupTable[TilesetIndexMetadata.TilesetFlagType.DIAGONAL_FACEWALL_UPPER_NE], roomTypeIndex, out index);
                map.Layers[GlobalDungeonData.collisionLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, index);
                this.GetMetadataFromTupleArray(current, this.m_metadataLookupTable[TilesetIndexMetadata.TilesetFlagType.DIAGONAL_FACEWALL_TOP_NE], roomTypeIndex, out index);
                map.Layers[GlobalDungeonData.collisionLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y + 1, index);
              }
            }
            else if (current.diagonalWallType == DiagonalWallType.NORTHWEST)
            {
              if (wallType == TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER)
              {
                this.GetMetadataFromTupleArray(current, this.m_metadataLookupTable[TilesetIndexMetadata.TilesetFlagType.DIAGONAL_FACEWALL_LOWER_NW], roomTypeIndex, out index);
                map.Layers[GlobalDungeonData.collisionLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, index);
              }
              else if (wallType == TilesetIndexMetadata.TilesetFlagType.FACEWALL_UPPER)
              {
                this.GetMetadataFromTupleArray(current, this.m_metadataLookupTable[TilesetIndexMetadata.TilesetFlagType.DIAGONAL_FACEWALL_UPPER_NW], roomTypeIndex, out index);
                map.Layers[GlobalDungeonData.collisionLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, index);
                this.GetMetadataFromTupleArray(current, this.m_metadataLookupTable[TilesetIndexMetadata.TilesetFlagType.DIAGONAL_FACEWALL_TOP_NW], roomTypeIndex, out index);
                map.Layers[GlobalDungeonData.collisionLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y + 1, index);
              }
            }
            else
              UnityEngine.Debug.LogError((object) "Attempting to stamp a facewall tile on a non-facewall diagonal type.");
            if (wallType == TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER)
            {
              if (current.diagonalWallType == DiagonalWallType.NORTHEAST)
              {
                this.AssignSpecificColorsToTile(current.positionInTilemap.x, current.positionInTilemap.y, GlobalDungeonData.collisionLayerIndex, (Color32) new Color(0.0f, 0.0f, 1f), (Color32) new Color(0.0f, 0.0f, 1f), (Color32) new Color(0.0f, 0.0f, 1f), (Color32) new Color(0.0f, 0.5f, 1f), map);
                return;
              }
              if (current.diagonalWallType != DiagonalWallType.NORTHWEST)
                return;
              this.AssignSpecificColorsToTile(current.positionInTilemap.x, current.positionInTilemap.y, GlobalDungeonData.collisionLayerIndex, (Color32) new Color(0.0f, 0.0f, 1f), (Color32) new Color(0.0f, 0.0f, 1f), (Color32) new Color(0.0f, 0.5f, 1f), (Color32) new Color(0.0f, 0.0f, 1f), map);
              return;
            }
            if (wallType != TilesetIndexMetadata.TilesetFlagType.FACEWALL_UPPER)
              return;
            if (current.diagonalWallType == DiagonalWallType.NORTHEAST)
            {
              this.AssignSpecificColorsToTile(current.positionInTilemap.x, current.positionInTilemap.y, GlobalDungeonData.collisionLayerIndex, (Color32) new Color(0.0f, 0.0f, 1f), (Color32) new Color(0.0f, 0.5f, 1f), (Color32) new Color(0.0f, 0.5f, 1f), (Color32) new Color(0.0f, 1f, 1f), map);
              return;
            }
            if (current.diagonalWallType != DiagonalWallType.NORTHWEST)
              return;
            this.AssignSpecificColorsToTile(current.positionInTilemap.x, current.positionInTilemap.y, GlobalDungeonData.collisionLayerIndex, (Color32) new Color(0.0f, 0.5f, 1f), (Color32) new Color(0.0f, 0.0f, 1f), (Color32) new Color(0.0f, 1f, 1f), (Color32) new Color(0.0f, 0.5f, 1f), map);
            return;
          }
          int index1 = -1;
          bool flag = false;
          int num = 0;
          while (!flag && num < 1000)
          {
            ++num;
            flag = true;
            TilesetIndexMetadata metadataFromTupleArray = this.GetMetadataFromTupleArray(current, this.m_metadataLookupTable[tileOverrideType], roomTypeIndex, out index1);
            List<IndexNeighborDependency> dependencies = d.tileIndices.dungeonCollection.GetDependencies(index1);
            if (metadataFromTupleArray != null && dependencies != null && dependencies.Count > 0)
              flag = this.ProcessFacewallNeighborMetadata(ix, iy, d, dependencies, metadataFromTupleArray.preventWallStamping);
          }
          if (d.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.OFFICEGEON && (tileOverrideType == TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER_RIGHTCORNER || tileOverrideType == TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER_LEFTCORNER || tileOverrideType == TilesetIndexMetadata.TilesetFlagType.FACEWALL_UPPER_RIGHTCORNER || tileOverrideType == TilesetIndexMetadata.TilesetFlagType.FACEWALL_UPPER_LEFTCORNER))
            current.cellVisualData.containsWallSpaceStamp = true;
          BraveUtility.Assert(index1 == -1, "FACEWALL INDEX -1, there are no facewalls defined");
          map.Layers[GlobalDungeonData.collisionLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, index1);
        }
        if (current.parentRoom == null || (UnityEngine.Object) current.parentRoom.area.prototypeRoom == (UnityEngine.Object) null || !current.parentRoom.area.prototypeRoom.preventFacewallAO)
        {
          if (wallType == TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER)
            map.Layers[GlobalDungeonData.shadowLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, this.t.aoTileIndices.AOBottomWallBaseTileIndex);
          bool flag1 = cellData1.type == CellType.WALL && cellData1.diagonalWallType == DiagonalWallType.NONE && (!d.data.isFaceWallRight(ix, iy) || wallType == TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER && cellData1.IsUpperFacewall());
          bool flag2 = cellData2.type == CellType.WALL && cellData2.diagonalWallType == DiagonalWallType.NONE && (!d.data.isFaceWallLeft(ix, iy) || wallType == TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER && cellData2.IsUpperFacewall());
          if (flag2 && flag1)
            map.Layers[GlobalDungeonData.shadowLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, wallType != TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER ? this.t.aoTileIndices.AOTopFacewallBothIndex : this.t.aoTileIndices.AOBottomWallTileBothIndex);
          else if (flag2)
            map.Layers[GlobalDungeonData.shadowLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, wallType != TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER ? this.t.aoTileIndices.AOTopFacewallLeftIndex : this.t.aoTileIndices.AOBottomWallTileLeftIndex);
          else if (flag1)
            map.Layers[GlobalDungeonData.shadowLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, wallType != TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER ? this.t.aoTileIndices.AOTopFacewallRightIndex : this.t.aoTileIndices.AOBottomWallTileRightIndex);
        }
        if (wallType == TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER)
          this.AssignColorGradientToTile(current.positionInTilemap.x, current.positionInTilemap.y, GlobalDungeonData.collisionLayerIndex, (Color32) new Color(0.0f, 0.0f, 1f), (Color32) new Color(0.0f, 0.5f, 1f), map);
        else
          this.AssignColorGradientToTile(current.positionInTilemap.x, current.positionInTilemap.y, GlobalDungeonData.collisionLayerIndex, (Color32) new Color(0.0f, 0.5f, 1f), (Color32) new Color(0.0f, 1f, 1f), map);
      }

      private int FindValidFacewallExpanse(
        int ix,
        int iy,
        Dungeon d,
        FacewallIndexGridDefinition gridDefinition)
      {
        int validFacewallExpanse = 0;
        int roomVisualTypeIndex = d.data[ix, iy].cellVisualData.roomVisualTypeIndex;
        while (d.data.isFaceWallLower(ix, iy) && d.data[ix, iy].cellVisualData.faceWallOverrideIndex == -1 && d.data[ix, iy].cellVisualData.roomVisualTypeIndex == roomVisualTypeIndex)
        {
          bool flag = !d.data.isFaceWallLeft(ix, iy) || !d.data.isFaceWallRight(ix, iy);
          if ((gridDefinition.canExistInCorners || !flag) && (!d.data[ix, iy - 2].isExitCell || gridDefinition.canBePlacedInExits))
          {
            ++ix;
            ++validFacewallExpanse;
            if (validFacewallExpanse >= gridDefinition.maxWidth || validFacewallExpanse > gridDefinition.minWidth && (double) UnityEngine.Random.value < (double) gridDefinition.perTileFailureRate)
              break;
          }
          else
            break;
        }
        return validFacewallExpanse;
      }

      private bool AssignFacewallGrid(
        CellData current,
        Dungeon d,
        tk2dTileMap map,
        int ix,
        int iy,
        FacewallIndexGridDefinition gridDefinition)
      {
        int validFacewallExpanse = this.FindValidFacewallExpanse(ix, iy, d, gridDefinition);
        if (validFacewallExpanse < gridDefinition.minWidth)
          return false;
        TileIndexGrid grid = gridDefinition.grid;
        int num1 = 0;
        int num2 = validFacewallExpanse;
        int num3 = num2;
        int num4 = 0;
        if (gridDefinition.hasIntermediaries)
          num3 = UnityEngine.Random.Range(gridDefinition.minIntermediaryBuffer, gridDefinition.maxIntermediaryBuffer + 1);
        bool flag1 = true;
        int index = 0;
        for (; num2 > 0; --num2)
        {
          CellData cellData1 = d.data[ix + num1, iy];
          CellData cellData2 = d.data[ix + num1, iy + 1];
          if (num4 > 0)
          {
            --num4;
            cellData1.cellVisualData.faceWallOverrideIndex = grid.doubleNubsBottom.GetIndexByWeight();
            cellData2.cellVisualData.faceWallOverrideIndex = grid.doubleNubsTop.GetIndexByWeight();
            if (num4 == 0)
            {
              flag1 = true;
              num3 = UnityEngine.Random.Range(gridDefinition.minIntermediaryBuffer, gridDefinition.maxIntermediaryBuffer + 1);
            }
          }
          else
          {
            bool flag2 = false;
            BraveUtility.DrawDebugSquare(cellData1.position.ToVector2(), Color.blue, 1000f);
            --num3;
            if (num3 <= 0)
            {
              if (gridDefinition.hasIntermediaries)
                num4 = UnityEngine.Random.Range(gridDefinition.minIntermediaryLength, gridDefinition.maxIntermediaryLength + 1);
              flag2 = true;
            }
            if (flag1)
            {
              cellData1.cellVisualData.faceWallOverrideIndex = grid.bottomLeftIndices.GetIndexByWeight();
              cellData2.cellVisualData.faceWallOverrideIndex = grid.topLeftIndices.GetIndexByWeight();
              cellData1.cellVisualData.containsWallSpaceStamp = true;
              cellData2.cellVisualData.containsWallSpaceStamp = true;
            }
            else if (flag2 || num2 == 1)
            {
              cellData1.cellVisualData.faceWallOverrideIndex = grid.bottomRightIndices.GetIndexByWeight();
              cellData2.cellVisualData.faceWallOverrideIndex = grid.topRightIndices.GetIndexByWeight();
              cellData1.cellVisualData.containsWallSpaceStamp = true;
              cellData2.cellVisualData.containsWallSpaceStamp = true;
              if (flag2 && num4 == 0)
                num3 = UnityEngine.Random.Range(gridDefinition.minIntermediaryBuffer, gridDefinition.maxIntermediaryBuffer + 1);
            }
            else
            {
              cellData1.cellVisualData.faceWallOverrideIndex = !gridDefinition.middleSectionSequential ? grid.bottomIndices.GetIndexByWeight() : grid.bottomIndices.indices[index];
              cellData2.cellVisualData.faceWallOverrideIndex = !gridDefinition.topsMatchBottoms ? (!gridDefinition.middleSectionSequential ? grid.topIndices.GetIndexByWeight() : grid.topIndices.indices[index]) : grid.topIndices.indices[grid.bottomIndices.GetIndexOfIndex(cellData1.cellVisualData.faceWallOverrideIndex)];
              index = (index + 1) % grid.bottomIndices.indices.Count;
              cellData1.cellVisualData.forcedMatchingStyle = gridDefinition.forcedStampMatchingStyle;
              cellData2.cellVisualData.forcedMatchingStyle = gridDefinition.forcedStampMatchingStyle;
            }
            flag1 = false;
            cellData1.cellVisualData.containsObjectSpaceStamp = cellData1.cellVisualData.containsObjectSpaceStamp || !gridDefinition.canAcceptFloorDecoration;
            cellData2.cellVisualData.containsObjectSpaceStamp = cellData2.cellVisualData.containsObjectSpaceStamp || !gridDefinition.canAcceptFloorDecoration;
            cellData1.cellVisualData.facewallGridPreventsWallSpaceStamp = !gridDefinition.canAcceptWallDecoration;
            cellData2.cellVisualData.facewallGridPreventsWallSpaceStamp = !gridDefinition.canAcceptWallDecoration;
            cellData1.cellVisualData.containsWallSpaceStamp = cellData1.cellVisualData.containsWallSpaceStamp || !gridDefinition.canAcceptWallDecoration;
            cellData2.cellVisualData.containsWallSpaceStamp = cellData2.cellVisualData.containsWallSpaceStamp || !gridDefinition.canAcceptWallDecoration;
          }
          ++num1;
        }
        return true;
      }

      private void ProcessFacewallIndices(
        CellData current,
        Dungeon d,
        tk2dTileMap map,
        int ix,
        int iy)
      {
        if (current.cellVisualData.shouldIgnoreWallDrawing)
          return;
        DungeonMaterial materialDefinition = d.roomMaterialDefinitions[current.cellVisualData.roomVisualTypeIndex];
        if (current.cellVisualData.IsFacewallForInteriorTransition)
          materialDefinition = d.roomMaterialDefinitions[current.cellVisualData.InteriorTransitionIndex];
        if (d.data.isSingleCellWall(ix, iy))
          map.Layers[GlobalDungeonData.collisionLayerIndex].SetTile(current.positionInTilemap.x, current.positionInTilemap.y, this.GetIndexFromTileArray(current, this.t.chestHighWallIndices).index);
        else if (d.data.isFaceWallLower(ix, iy))
        {
          if ((UnityEngine.Object) materialDefinition != (UnityEngine.Object) null && materialDefinition.usesFacewallGrids)
          {
            FacewallIndexGridDefinition facewallGrid = materialDefinition.facewallGrids[UnityEngine.Random.Range(0, materialDefinition.facewallGrids.Length)];
            if (current.cellVisualData.faceWallOverrideIndex == -1 && (double) UnityEngine.Random.value < (double) facewallGrid.chanceToPlaceIfPossible)
              this.AssignFacewallGrid(current, d, map, ix, iy, facewallGrid);
          }
          bool flag1 = d.data.isWallLeft(ix, iy) && !d.data.isFaceWallLeft(ix, iy);
          bool flag2 = d.data.isWallRight(ix, iy) && !d.data.isFaceWallRight(ix, iy);
          bool flag3 = !d.data.isWallLeft(ix, iy);
          bool flag4 = !d.data.isWallRight(ix, iy);
          if (flag3 && materialDefinition.forceEdgesDiagonal)
            current.diagonalWallType = DiagonalWallType.NORTHEAST;
          if (flag4 && materialDefinition.forceEdgesDiagonal)
            current.diagonalWallType = DiagonalWallType.NORTHWEST;
          if (flag3 && !flag4 && this.HasMetadataForRoomType(TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER_LEFTEDGE, current.cellVisualData.roomVisualTypeIndex))
            this.ProcessFacewallType(current, d, map, ix, iy, TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER, TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER_LEFTEDGE);
          else if (flag4 && !flag3 && this.HasMetadataForRoomType(TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER_RIGHTEDGE, current.cellVisualData.roomVisualTypeIndex))
            this.ProcessFacewallType(current, d, map, ix, iy, TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER, TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER_RIGHTEDGE);
          else if (flag1 && !flag2 && this.HasMetadataForRoomType(TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER_LEFTCORNER, current.cellVisualData.roomVisualTypeIndex))
            this.ProcessFacewallType(current, d, map, ix, iy, TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER, TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER_LEFTCORNER);
          else if (flag2 && !flag1 && this.HasMetadataForRoomType(TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER_RIGHTCORNER, current.cellVisualData.roomVisualTypeIndex))
            this.ProcessFacewallType(current, d, map, ix, iy, TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER, TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER_RIGHTCORNER);
          else
            this.ProcessFacewallType(current, d, map, ix, iy, TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER, TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER);
        }
        else
        {
          if (!d.data.isFaceWallHigher(ix, iy))
            return;
          bool flag5 = d.data.isWallLeft(ix, iy) && !d.data.isFaceWallLeft(ix, iy);
          bool flag6 = d.data.isWallRight(ix, iy) && !d.data.isFaceWallRight(ix, iy);
          bool flag7 = !d.data.isWallLeft(ix, iy) || d.data.isFaceWallLeft(ix, iy) && !d.data[ix - 1, iy].IsUpperFacewall();
          bool flag8 = !d.data.isWallRight(ix, iy) || d.data.isFaceWallRight(ix, iy) && !d.data[ix + 1, iy].IsUpperFacewall();
          if (flag7 && !flag8 && materialDefinition.forceEdgesDiagonal)
            current.diagonalWallType = DiagonalWallType.NORTHEAST;
          if (flag8 && !flag7 && materialDefinition.forceEdgesDiagonal)
            current.diagonalWallType = DiagonalWallType.NORTHWEST;
          if (flag7 && !flag8 && this.HasMetadataForRoomType(TilesetIndexMetadata.TilesetFlagType.FACEWALL_UPPER_LEFTEDGE, current.cellVisualData.roomVisualTypeIndex))
            this.ProcessFacewallType(current, d, map, ix, iy, TilesetIndexMetadata.TilesetFlagType.FACEWALL_UPPER, TilesetIndexMetadata.TilesetFlagType.FACEWALL_UPPER_LEFTEDGE);
          else if (flag8 && !flag7 && this.HasMetadataForRoomType(TilesetIndexMetadata.TilesetFlagType.FACEWALL_UPPER_RIGHTEDGE, current.cellVisualData.roomVisualTypeIndex))
            this.ProcessFacewallType(current, d, map, ix, iy, TilesetIndexMetadata.TilesetFlagType.FACEWALL_UPPER, TilesetIndexMetadata.TilesetFlagType.FACEWALL_UPPER_RIGHTEDGE);
          else if (flag5 && !flag6 && this.HasMetadataForRoomType(TilesetIndexMetadata.TilesetFlagType.FACEWALL_UPPER_LEFTCORNER, current.cellVisualData.roomVisualTypeIndex))
            this.ProcessFacewallType(current, d, map, ix, iy, TilesetIndexMetadata.TilesetFlagType.FACEWALL_UPPER, TilesetIndexMetadata.TilesetFlagType.FACEWALL_UPPER_LEFTCORNER);
          else if (flag6 && !flag5 && this.HasMetadataForRoomType(TilesetIndexMetadata.TilesetFlagType.FACEWALL_UPPER_RIGHTCORNER, current.cellVisualData.roomVisualTypeIndex))
            this.ProcessFacewallType(current, d, map, ix, iy, TilesetIndexMetadata.TilesetFlagType.FACEWALL_UPPER, TilesetIndexMetadata.TilesetFlagType.FACEWALL_UPPER_RIGHTCORNER);
          else
            this.ProcessFacewallType(current, d, map, ix, iy, TilesetIndexMetadata.TilesetFlagType.FACEWALL_UPPER, TilesetIndexMetadata.TilesetFlagType.FACEWALL_UPPER);
        }
      }

      [DebuggerHidden]
      public IEnumerator ConstructTK2DDungeon(Dungeon d, tk2dTileMap map)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new TK2DDungeonAssembler__ConstructTK2DDungeonc__Iterator0()
        {
          d = d,
          map = map,
          _this = this
        };
      }

      private void HandlePitBorderTilePlacement(
        CellData cell,
        TileIndexGrid borderGrid,
        Layer tileMapLayer,
        tk2dTileMap tileMap,
        Dungeon d)
      {
        if (borderGrid.PitBorderIsInternal)
        {
          if (cell.type != CellType.PIT)
            return;
          List<CellData> cellNeighbors = d.data.GetCellNeighbors(cell, true);
          bool flag1 = cellNeighbors[0] != null && cellNeighbors[0].type == CellType.PIT;
          bool flag2 = cellNeighbors[1] != null && cellNeighbors[1].type == CellType.PIT;
          bool flag3 = cellNeighbors[2] != null && cellNeighbors[2].type == CellType.PIT;
          bool flag4 = cellNeighbors[3] != null && cellNeighbors[3].type == CellType.PIT;
          bool flag5 = cellNeighbors[4] != null && cellNeighbors[4].type == CellType.PIT;
          bool flag6 = cellNeighbors[5] != null && cellNeighbors[5].type == CellType.PIT;
          bool flag7 = cellNeighbors[6] != null && cellNeighbors[6].type == CellType.PIT;
          bool flag8 = cellNeighbors[7] != null && cellNeighbors[7].type == CellType.PIT;
          int indexGivenSides = borderGrid.GetIndexGivenSides(!flag1, !flag2, !flag3, !flag4, !flag5, !flag6, !flag7, !flag8);
          tileMapLayer.SetTile(cell.positionInTilemap.x, cell.positionInTilemap.y, indexGivenSides);
        }
        else if (cell.type == CellType.PIT)
        {
          List<CellData> cellNeighbors = d.data.GetCellNeighbors(cell);
          bool isNorthBorder = cellNeighbors[0] != null && cellNeighbors[0].type == CellType.PIT;
          bool isEastBorder = cellNeighbors[1] != null && cellNeighbors[1].type == CellType.PIT;
          bool isSouthBorder = cellNeighbors[2] != null && cellNeighbors[2].type == CellType.PIT;
          bool isWestBorder = cellNeighbors[3] != null && cellNeighbors[3].type == CellType.PIT;
          int internalIndexGivenSides = borderGrid.GetInternalIndexGivenSides(isNorthBorder, isEastBorder, isSouthBorder, isWestBorder);
          if (internalIndexGivenSides == -1)
            return;
          tileMapLayer.SetTile(cell.positionInTilemap.x, cell.positionInTilemap.y, internalIndexGivenSides);
        }
        else
        {
          List<CellData> cellNeighbors = d.data.GetCellNeighbors(cell, true);
          bool isNorthBorder = cellNeighbors[0] != null && (cellNeighbors[0].type == CellType.PIT || cellNeighbors[0].cellVisualData.RequiresPitBordering);
          bool isNortheastBorder = cellNeighbors[1] != null && (cellNeighbors[1].type == CellType.PIT || cellNeighbors[1].cellVisualData.RequiresPitBordering);
          bool isEastBorder = cellNeighbors[2] != null && (cellNeighbors[2].type == CellType.PIT || cellNeighbors[2].cellVisualData.RequiresPitBordering);
          bool isSoutheastBorder = cellNeighbors[3] != null && (cellNeighbors[3].type == CellType.PIT || cellNeighbors[3].cellVisualData.RequiresPitBordering);
          bool isSouthBorder = cellNeighbors[4] != null && (cellNeighbors[4].type == CellType.PIT || cellNeighbors[4].cellVisualData.RequiresPitBordering);
          bool isSouthwestBorder = cellNeighbors[5] != null && (cellNeighbors[5].type == CellType.PIT || cellNeighbors[5].cellVisualData.RequiresPitBordering);
          bool isWestBorder = cellNeighbors[6] != null && (cellNeighbors[6].type == CellType.PIT || cellNeighbors[6].cellVisualData.RequiresPitBordering);
          bool isNorthwestBorder = cellNeighbors[7] != null && (cellNeighbors[7].type == CellType.PIT || cellNeighbors[7].cellVisualData.RequiresPitBordering);
          if (!isNorthBorder && !isNortheastBorder && !isEastBorder && !isSoutheastBorder && !isSouthBorder && !isSouthwestBorder && !isWestBorder && !isNorthwestBorder)
            return;
          int indexGivenSides = borderGrid.GetIndexGivenSides(isNorthBorder, isNortheastBorder, isEastBorder, isSoutheastBorder, isSouthBorder, isSouthwestBorder, isWestBorder, isNorthwestBorder);
          if (borderGrid.PitBorderOverridesFloorTile)
            tileMap.SetTile(cell.positionInTilemap.x, cell.positionInTilemap.y, GlobalDungeonData.floorLayerIndex, indexGivenSides);
          else
            tileMapLayer.SetTile(cell.positionInTilemap.x, cell.positionInTilemap.y, indexGivenSides);
          if (!borderGrid.PitBorderOverridesFloorTile)
            return;
          TileIndexGrid pitLayoutGrid = d.roomMaterialDefinitions[cell.cellVisualData.roomVisualTypeIndex].pitLayoutGrid;
          if ((UnityEngine.Object) pitLayoutGrid == (UnityEngine.Object) null)
            pitLayoutGrid = d.roomMaterialDefinitions[0].pitLayoutGrid;
          tileMap.Layers[GlobalDungeonData.pitLayerIndex].SetTile(cell.positionInTilemap.x, cell.positionInTilemap.y, pitLayoutGrid.centerIndices.GetIndexByWeight());
        }
      }

      private void HandlePitTilePlacement(
        CellData cell,
        TileIndexGrid pitGrid,
        Layer tileMapLayer,
        Dungeon d)
      {
        if ((UnityEngine.Object) pitGrid == (UnityEngine.Object) null)
          return;
        List<CellData> cellNeighbors = d.data.GetCellNeighbors(cell);
        bool flag1 = cellNeighbors[0] != null && cellNeighbors[0].type == CellType.PIT;
        bool flag2 = cellNeighbors[1] != null && cellNeighbors[1].type == CellType.PIT;
        bool flag3 = cellNeighbors[2] != null && cellNeighbors[2].type == CellType.PIT;
        bool flag4 = cellNeighbors[3] != null && cellNeighbors[3].type == CellType.PIT;
        bool flag5 = this.BCheck(d, cell.position.x, cell.position.y + 2) && d.data.cellData[cell.position.x][cell.position.y + 2].type == CellType.PIT;
        bool flag6 = this.BCheck(d, cell.position.x, cell.position.y + 3) && d.data.cellData[cell.position.x][cell.position.y + 3].type == CellType.PIT;
        if (cell.cellVisualData.pitOverrideIndex > -1)
        {
          tileMapLayer.SetTile(cell.positionInTilemap.x, cell.positionInTilemap.y, cell.cellVisualData.pitOverrideIndex);
        }
        else
        {
          if (GameManager.Instance.Dungeon.debugSettings.WALLS_ARE_PITS)
          {
            if (cellNeighbors[2] != null && cellNeighbors[2].isExitCell)
              flag3 = true;
            if (cellNeighbors[0] != null && cellNeighbors[0].isExitCell)
              flag1 = true;
            if (this.BCheck(d, cell.position.x, cell.position.y + 2) && d.data.cellData[cell.position.x][cell.position.y + 2].isExitCell)
              flag5 = true;
            if (this.BCheck(d, cell.position.x, cell.position.y + 3) && d.data.cellData[cell.position.x][cell.position.y + 3].isExitCell)
              flag6 = true;
          }
          int tile = pitGrid.GetIndexGivenSides(!flag1, !flag5, !flag6, !flag2, !flag3, !flag4);
          if (pitGrid.PitInternalSquareGrids.Count > 0 && (double) UnityEngine.Random.value < (double) pitGrid.PitInternalSquareOptions.PitSquareChance && (pitGrid.PitInternalSquareOptions.CanBeFlushLeft || flag4) && (pitGrid.PitInternalSquareOptions.CanBeFlushBottom || flag3) && flag2 && flag1 && flag5 && flag6)
          {
            bool flag7 = this.BCheck(d, cell.position.x + 2, cell.position.y) && d.data.cellData[cell.position.x + 2][cell.position.y].type == CellType.PIT;
            bool flag8 = this.BCheck(d, cell.position.x + 1, cell.position.y + 1) && d.data.cellData[cell.position.x + 1][cell.position.y + 1].type == CellType.PIT;
            bool flag9 = this.BCheck(d, cell.position.x + 1, cell.position.y + 2) && d.data.cellData[cell.position.x + 1][cell.position.y + 2].type == CellType.PIT;
            bool flag10 = this.BCheck(d, cell.position.x + 1, cell.position.y + 3) && d.data.cellData[cell.position.x + 1][cell.position.y + 3].type == CellType.PIT;
            if ((pitGrid.PitInternalSquareOptions.CanBeFlushRight || flag7) && flag8 && flag10 && flag9)
            {
              TileIndexGrid internalSquareGrid = pitGrid.PitInternalSquareGrids[UnityEngine.Random.Range(0, pitGrid.PitInternalSquareGrids.Count)];
              tile = internalSquareGrid.bottomLeftIndices.GetIndexByWeight();
              d.data.cellData[cell.position.x + 1][cell.position.y].cellVisualData.pitOverrideIndex = internalSquareGrid.bottomRightIndices.GetIndexByWeight();
              d.data.cellData[cell.position.x][cell.position.y + 1].cellVisualData.pitOverrideIndex = internalSquareGrid.topLeftIndices.GetIndexByWeight();
              d.data.cellData[cell.position.x + 1][cell.position.y + 1].cellVisualData.pitOverrideIndex = internalSquareGrid.topRightIndices.GetIndexByWeight();
            }
          }
          tileMapLayer.SetTile(cell.positionInTilemap.x, cell.positionInTilemap.y, tile);
        }
        if (!flag1 || flag5)
          return;
        this.AssignColorGradientToTile(cell.positionInTilemap.x, cell.positionInTilemap.y, GlobalDungeonData.pitLayerIndex, (Color32) new Color(1f, 1f, 1f, 1f), (Color32) new Color(0.0f, 0.0f, 0.0f, 1f), GameManager.Instance.Dungeon.MainTilemap);
      }
    }

}
