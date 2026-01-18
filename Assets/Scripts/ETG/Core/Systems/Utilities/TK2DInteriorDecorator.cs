// Decompiled with JetBrains decompiler
// Type: TK2DInteriorDecorator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class TK2DInteriorDecorator
    {
      private TK2DDungeonAssembler m_assembler;
      private Dictionary<DungeonTileStampData.StampPlacementRule, IntVector2> wallPlacementOffsets;
      private List<TK2DInteriorDecorator.ViableStampCategorySet> viableCategorySets;
      private List<DungeonTileStampData.StampPlacementRule> validNorthernPlacements;
      private List<DungeonTileStampData.StampPlacementRule> validEasternPlacements;
      private List<DungeonTileStampData.StampPlacementRule> validWesternPlacements;
      private List<DungeonTileStampData.StampPlacementRule> validSouthernPlacements;
      private bool DEBUG_DRAW;
      private List<StampDataBase> roomUsedStamps = new List<StampDataBase>();
      private List<StampDataBase> expanseUsedStamps = new List<StampDataBase>();

      public TK2DInteriorDecorator(TK2DDungeonAssembler assembler) => this.m_assembler = assembler;

      private void DecorateRoomExit(
        RoomHandler r,
        RuntimeRoomExitData usedExit,
        Dungeon d,
        tk2dTileMap map)
      {
        RoomHandler roomHandler = r.connectedRoomsByExit[usedExit.referencedExit];
        if (usedExit.referencedExit.exitDirection != DungeonData.Direction.NORTH)
          return;
        IntVector2 intVector2_1 = r.area.basePosition + usedExit.ExitOrigin - IntVector2.One;
        int val1 = 0;
        while (d.data.isFaceWallLower(intVector2_1.x - val1 - 1, intVector2_1.y))
          ++val1;
        int val2 = 0;
        while (d.data.isFaceWallLower(intVector2_1.x + usedExit.referencedExit.ExitCellCount + val2, intVector2_1.y))
          ++val2;
        int maxWidth = Math.Min(val1, val2);
        int num = 0;
        if (maxWidth <= 0)
          return;
        for (int index = 0; index < 3; ++index)
        {
          IntVector2 intVector2_2 = IntVector2.Zero;
          StampDataBase stampDataComplex;
          if (index == 0 || index == 2)
          {
            stampDataComplex = d.stampData.GetStampDataComplex(DungeonTileStampData.StampPlacementRule.BELOW_LOWER_FACEWALL, DungeonTileStampData.StampSpace.BOTH_SPACES, DungeonTileStampData.StampCategory.STRUCTURAL, roomHandler.opulence, r.RoomVisualSubtype, maxWidth);
          }
          else
          {
            stampDataComplex = d.stampData.GetStampDataComplex(DungeonTileStampData.StampPlacementRule.BELOW_LOWER_FACEWALL, DungeonTileStampData.StampSpace.OBJECT_SPACE, DungeonTileStampData.StampCategory.MUNDANE, roomHandler.opulence, r.RoomVisualSubtype, maxWidth);
            intVector2_2 = IntVector2.Up;
          }
          IntVector2 intVector2_3 = intVector2_1 + IntVector2.Down + IntVector2.Left * (stampDataComplex.width + num) + intVector2_2;
          IntVector2 intVector2_4 = intVector2_1 + IntVector2.Down + IntVector2.Right * (usedExit.referencedExit.ExitCellCount + num) + intVector2_2;
          switch (stampDataComplex)
          {
            case TileStampData _:
              this.m_assembler.ApplyTileStamp(intVector2_3.x, intVector2_3.y, stampDataComplex as TileStampData, d, map);
              this.m_assembler.ApplyTileStamp(intVector2_4.x, intVector2_4.y, stampDataComplex as TileStampData, d, map);
              break;
            case SpriteStampData _:
              this.m_assembler.ApplySpriteStamp(intVector2_3.x, intVector2_3.y, stampDataComplex as SpriteStampData, d, map);
              this.m_assembler.ApplySpriteStamp(intVector2_4.x, intVector2_4.y, stampDataComplex as SpriteStampData, d, map);
              break;
            case ObjectStampData _:
              Debug.Log((object) "object instantiate");
              TK2DDungeonAssembler.ApplyObjectStamp(intVector2_3.x, intVector2_3.y, stampDataComplex as ObjectStampData, d, map);
              TK2DDungeonAssembler.ApplyObjectStamp(intVector2_4.x, intVector2_4.y, stampDataComplex as ObjectStampData, d, map);
              break;
          }
          maxWidth -= stampDataComplex.width;
          num += stampDataComplex.width;
          if (maxWidth <= 0)
            break;
        }
      }

      public static void PlaceLightDecorationForCell(
        Dungeon d,
        tk2dTileMap map,
        CellData currentCell,
        IntVector2 currentPosition)
      {
        if (!currentCell.cellVisualData.containsLight || currentCell.cellVisualData.lightDirection == DungeonData.Direction.SOUTH || currentCell.cellVisualData.lightDirection == ~DungeonData.Direction.NORTH)
          return;
        DungeonTileStampData.StampPlacementRule stampPlacementRule = DungeonTileStampData.StampPlacementRule.ON_LOWER_FACEWALL;
        bool flipX = false;
        if (currentCell.cellVisualData.lightDirection == DungeonData.Direction.EAST)
        {
          stampPlacementRule = DungeonTileStampData.StampPlacementRule.ALONG_LEFT_WALLS;
          flipX = true;
        }
        else if (currentCell.cellVisualData.lightDirection == DungeonData.Direction.WEST)
          stampPlacementRule = DungeonTileStampData.StampPlacementRule.ALONG_LEFT_WALLS;
        LightStampData osd = stampPlacementRule != DungeonTileStampData.StampPlacementRule.ALONG_LEFT_WALLS ? currentCell.cellVisualData.facewallLightStampData : currentCell.cellVisualData.sidewallLightStampData;
        if (osd == null)
          return;
        GameObject gameObject = TK2DDungeonAssembler.ApplyObjectStamp(currentPosition.x, currentPosition.y, (ObjectStampData) osd, d, map, flipX, true);
        if ((bool) (UnityEngine.Object) gameObject)
        {
          TorchController component = gameObject.GetComponent<TorchController>();
          if (!(bool) (UnityEngine.Object) component)
            return;
          component.Cell = currentCell;
        }
        else
        {
          if (!((UnityEngine.Object) currentCell.cellVisualData.lightObject != (UnityEngine.Object) null))
            return;
          ShadowSystem componentInChildren = currentCell.cellVisualData.lightObject.GetComponentInChildren<ShadowSystem>();
          if (!(bool) (UnityEngine.Object) componentInChildren)
            return;
          for (int index = 0; index < componentInChildren.PersonalCookies.Count; index = index - 1 + 1)
          {
            componentInChildren.PersonalCookies[index].enabled = false;
            componentInChildren.PersonalCookies.RemoveAt(index);
          }
        }
      }

      public void PlaceLightDecoration(Dungeon d, tk2dTileMap map)
      {
        for (int x = 0; x < d.data.Width; ++x)
        {
          for (int y = 1; y < d.data.Height; ++y)
          {
            IntVector2 intVector2 = new IntVector2(x, y);
            CellData currentCell = d.data[intVector2];
            if (currentCell != null)
              TK2DInteriorDecorator.PlaceLightDecorationForCell(d, map, currentCell, intVector2);
          }
        }
      }

      protected bool IsValidPondCell(CellData cell, RoomHandler parentRoom, Dungeon d)
      {
        return cell != null && parentRoom.ContainsPosition(cell.position) && cell.type == CellType.FLOOR && !cell.doesDamage && !cell.HasNonTopWallWallNeighbor() && !cell.HasPitNeighbor(d.data) && !cell.isOccupied && !cell.cellVisualData.floorTileOverridden && cell.cellVisualData.roomVisualTypeIndex == parentRoom.RoomVisualSubtype;
      }

      protected bool IsValidChannelCell(CellData cell, RoomHandler parentRoom, Dungeon d)
      {
        return cell != null && parentRoom.ContainsPosition(cell.position) && cell.type == CellType.FLOOR && !cell.doesDamage && !cell.HasPitNeighbor(d.data) && !cell.isOccupied && !cell.cellVisualData.floorTileOverridden && cell.cellVisualData.roomVisualTypeIndex == parentRoom.RoomVisualSubtype;
      }

      public void DigChannels(RoomHandler r, Dungeon d, tk2dTileMap map)
      {
        if (!d.roomMaterialDefinitions[r.RoomVisualSubtype].supportsChannels || d.roomMaterialDefinitions[r.RoomVisualSubtype].channelGrids.Length == 0)
          return;
        DungeonMaterial materialDefinition = d.roomMaterialDefinitions[r.RoomVisualSubtype];
        TileIndexGrid channelGrid = materialDefinition.channelGrids[UnityEngine.Random.Range(0, d.roomMaterialDefinitions[r.RoomVisualSubtype].channelGrids.Length)];
        if ((UnityEngine.Object) channelGrid == (UnityEngine.Object) null)
          return;
        int num1 = UnityEngine.Random.Range(materialDefinition.minChannelPools, materialDefinition.maxChannelPools);
        List<IntVector2> intVector2List1 = new List<IntVector2>();
        HashSet<IntVector2> intVector2Set1 = new HashSet<IntVector2>();
        for (int index = 0; index < num1; ++index)
        {
          HashSet<IntVector2> intVector2Set2 = new HashSet<IntVector2>();
          int num2 = UnityEngine.Random.Range(2, 5);
          int num3 = UnityEngine.Random.Range(2, 5);
          int num4 = UnityEngine.Random.Range(0, r.area.dimensions.x - num2);
          int num5 = UnityEngine.Random.Range(0, r.area.dimensions.y - num3);
          IntVector2 intVector2_1 = r.area.basePosition + new IntVector2(num4 + num2 / 2, num5 + num3 / 2);
          bool flag = false;
          if (num4 >= 0 && num5 >= 0)
          {
            for (int x = num4; x < num4 + num2; ++x)
            {
              for (int y = num5; y < num5 + num3; ++y)
              {
                IntVector2 key = r.area.basePosition + new IntVector2(x, y);
                if (this.IsValidPondCell(d.data[key], r, d))
                {
                  intVector2Set2.Add(key);
                }
                else
                {
                  flag = true;
                  goto label_14;
                }
              }
            }
          }
    label_14:
          if (!flag && intVector2Set2.Count > 5)
          {
            intVector2List1.Add(intVector2_1);
            foreach (IntVector2 intVector2_2 in intVector2Set2)
              intVector2Set1.Add(intVector2_2);
          }
          else if ((double) UnityEngine.Random.value < (double) materialDefinition.channelTenacity)
            --index;
        }
        for (int index1 = 0; index1 < intVector2List1.Count; ++index1)
        {
          int num6 = UnityEngine.Random.Range(1, 4);
          for (int index2 = 0; index2 < num6; ++index2)
          {
            HashSet<IntVector2> intVector2Set3 = new HashSet<IntVector2>();
            IntVector2 intVector2_3 = intVector2List1[index1];
            bool flag;
            do
            {
              int num7 = UnityEngine.Random.Range(3, 8);
              List<IntVector2> intVector2List2 = new List<IntVector2>((IEnumerable<IntVector2>) IntVector2.Cardinals);
              IntVector2 intVector2_4 = intVector2List2[UnityEngine.Random.Range(0, 4)];
              intVector2List2.Remove(intVector2_4);
              intVector2List2.Remove(intVector2_4 * -1);
              flag = false;
              for (int index3 = 0; index3 < num7; ++index3)
              {
                IntVector2 key = intVector2_3 + intVector2_4;
                CellData cell = d.data[key];
                if (cell == null || cell.type == CellType.WALL)
                {
                  flag = true;
                  break;
                }
                if (this.IsValidChannelCell(cell, r, d) && !intVector2Set3.Contains(key))
                {
                  if (intVector2List2.Count < 3)
                  {
                    intVector2List2 = new List<IntVector2>((IEnumerable<IntVector2>) IntVector2.Cardinals);
                    intVector2List2.Remove(intVector2_4);
                    intVector2List2.Remove(intVector2_4 * -1);
                  }
                  intVector2_3 = key;
                  intVector2Set1.Add(key);
                  intVector2Set3.Add(key);
                }
                else if (intVector2List2.Count > 1)
                {
                  intVector2_4 = intVector2List2[UnityEngine.Random.Range(0, intVector2List2.Count)];
                  intVector2List2.Remove(intVector2_4);
                  intVector2List2.Remove(intVector2_4 * -1);
                }
                else
                {
                  flag = true;
                  break;
                }
              }
            }
            while (!flag);
          }
        }
        IntVector2[] cardinalsAndOrdinals = IntVector2.CardinalsAndOrdinals;
        foreach (IntVector2 key in intVector2Set1)
        {
          bool[] flagArray = new bool[8];
          int num8 = 0;
          for (int index = 0; index < flagArray.Length; ++index)
          {
            flagArray[index] = !intVector2Set1.Contains(key + cardinalsAndOrdinals[index]);
            if (flagArray[index])
              ++num8;
          }
          if (num8 == 1)
          {
            for (int index = 0; index < flagArray.Length; index += 2)
            {
              if (d.data[key + cardinalsAndOrdinals[index]].type == CellType.WALL)
              {
                flagArray[index] = true;
                ++num8;
              }
            }
          }
          int indexGivenSides = channelGrid.GetIndexGivenSides(flagArray[0], flagArray[1], flagArray[2], flagArray[3], flagArray[4], flagArray[5], flagArray[6], flagArray[7]);
          map.SetTile(key.x, key.y, GlobalDungeonData.patternLayerIndex, indexGivenSides);
          d.data[key].cellVisualData.floorType = CellVisualData.CellFloorType.Water;
          d.data[key].cellVisualData.IsChannel = true;
        }
      }

      public void ProcessHardcodedUpholstery(RoomHandler r, Dungeon d, tk2dTileMap map)
      {
        DungeonMaterial materialDefinition = d.roomMaterialDefinitions[r.RoomVisualSubtype];
        if (materialDefinition.carpetGrids.Length == 0)
          return;
        HashSet<IntVector2> cellsToEncarpet = new HashSet<IntVector2>();
        TileIndexGrid carpetGrid = materialDefinition.carpetGrids[UnityEngine.Random.Range(0, materialDefinition.carpetGrids.Length)];
        for (int x = 0; x < r.area.dimensions.x; ++x)
        {
          for (int y = 0; y < r.area.dimensions.y; ++y)
          {
            IntVector2 key = r.area.basePosition + new IntVector2(x, y);
            CellData cellData = d.data[key];
            if (cellData != null && cellData.type == CellType.FLOOR && cellData.parentRoom == r && cellData.cellVisualData.IsPhantomCarpet && !cellData.HasPitNeighbor(d.data))
            {
              cellsToEncarpet.Add(key);
              BraveUtility.DrawDebugSquare(cellData.position.ToVector2(), cellData.position.ToVector2() + Vector2.one, Color.yellow, 1000f);
            }
          }
        }
        this.ApplyCarpetedHashset(cellsToEncarpet, carpetGrid, d, map);
      }

      public void UpholsterRoom(RoomHandler r, Dungeon d, tk2dTileMap map)
      {
        DungeonMaterial materialDefinition = d.roomMaterialDefinitions[r.RoomVisualSubtype];
        if (!materialDefinition.supportsUpholstery || materialDefinition.carpetGrids.Length == 0)
          return;
        TileIndexGrid carpetGrid = d.roomMaterialDefinitions[r.RoomVisualSubtype].carpetGrids[UnityEngine.Random.Range(0, d.roomMaterialDefinitions[r.RoomVisualSubtype].carpetGrids.Length)];
        if ((UnityEngine.Object) carpetGrid == (UnityEngine.Object) null)
          return;
        HashSet<IntVector2> intVector2Set = new HashSet<IntVector2>();
        if (materialDefinition.carpetIsMainFloor)
        {
          for (int x = 0; x < r.area.dimensions.x; ++x)
          {
            for (int y = 0; y < r.area.dimensions.y; ++y)
            {
              IntVector2 key = r.area.basePosition + new IntVector2(x, y);
              CellData cellData = d.data[key];
              if (cellData != null && cellData.type == CellType.FLOOR && cellData.parentRoom == r && !cellData.doesDamage && !cellData.cellVisualData.floorTileOverridden && cellData.cellVisualData.roomVisualTypeIndex == r.RoomVisualSubtype)
              {
                bool flag1 = cellData.HasWallNeighbor(includeTopwalls: false) || cellData.HasPitNeighbor(d.data);
                bool flag2 = cellData.HasPhantomCarpetNeighbor();
                if (!flag1 && !flag2)
                  intVector2Set.Add(key);
              }
            }
          }
          intVector2Set = Carpetron.PostprocessFullRoom(intVector2Set);
        }
        else
        {
          bool flag = true;
          List<Tuple<IntVector2, IntVector2>> tupleList = new List<Tuple<IntVector2, IntVector2>>();
          Tuple<IntVector2, IntVector2> tuple1 = (Tuple<IntVector2, IntVector2>) null;
          int num = 1;
          if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.CASTLEGEON)
            num = 2;
          while (flag)
          {
            Tuple<IntVector2, IntVector2> tuple2 = Carpetron.MaxSubmatrix(d.data.cellData, r.area.basePosition, r.area.dimensions, visualSubtype: r.RoomVisualSubtype);
            IntVector2 intVector2 = tuple2.Second + IntVector2.One - tuple2.First;
            if (intVector2.x * intVector2.y >= 15 && intVector2.x >= 3 && intVector2.y >= 3)
            {
              if (tuple1 != null && tuple1.Second + IntVector2.One - tuple1.First != intVector2)
              {
                --num;
                if (num <= 0)
                  break;
              }
              for (int x = tuple2.First.x; x < tuple2.Second.x + 1; ++x)
              {
                for (int y = tuple2.First.y; y < tuple2.Second.y + 1; ++y)
                {
                  IntVector2 key = r.area.basePosition + new IntVector2(x, y);
                  d.data[key].cellVisualData.floorTileOverridden = true;
                }
              }
              tupleList.Add(tuple2);
              tuple1 = tuple2;
            }
            else
              break;
          }
          if (tupleList.Count == 1)
          {
            Tuple<IntVector2, IntVector2> bonusRect = (Tuple<IntVector2, IntVector2>) null;
            tupleList[0] = Carpetron.PostprocessSubmatrix(tupleList[0], out bonusRect);
            if (bonusRect != null)
              tupleList.Add(bonusRect);
          }
          for (int index = 0; index < tupleList.Count; ++index)
          {
            Tuple<IntVector2, IntVector2> tuple3 = tupleList[index];
            for (int x = tuple3.First.x; x < tuple3.Second.x + 1; ++x)
            {
              for (int y = tuple3.First.y; y < tuple3.Second.y + 1; ++y)
              {
                IntVector2 intVector2 = r.area.basePosition + new IntVector2(x, y);
                intVector2Set.Add(intVector2);
              }
            }
          }
        }
        this.ApplyCarpetedHashset(intVector2Set, carpetGrid, d, map);
      }

      private void ApplyCarpetedHashset(
        HashSet<IntVector2> cellsToEncarpet,
        TileIndexGrid carpetGrid,
        Dungeon d,
        tk2dTileMap map)
      {
        IntVector2[] cardinalsAndOrdinals = IntVector2.CardinalsAndOrdinals;
        Dictionary<IntVector2, int> dictionary = new Dictionary<IntVector2, int>((IEqualityComparer<IntVector2>) new IntVector2EqualityComparer());
        if (carpetGrid.CenterIndicesAreStrata)
        {
          HashSet<IntVector2> intVector2Set1 = new HashSet<IntVector2>();
          HashSet<IntVector2> intVector2Set2 = new HashSet<IntVector2>();
          HashSet<IntVector2> intVector2Set3 = new HashSet<IntVector2>();
          foreach (IntVector2 intVector2 in cellsToEncarpet)
          {
            bool[] flagArray = new bool[8];
            for (int index = 0; index < flagArray.Length; ++index)
              flagArray[index] = !cellsToEncarpet.Contains(intVector2 + cardinalsAndOrdinals[index]);
            if (flagArray[0] || flagArray[1] || flagArray[2] || flagArray[3] || flagArray[4] || flagArray[5] || flagArray[6] || flagArray[7])
              intVector2Set2.Add(intVector2);
          }
          int num = 0;
          while (intVector2Set2.Count > 0)
          {
            foreach (IntVector2 intVector2 in intVector2Set2)
            {
              intVector2Set1.Add(intVector2);
              for (int index = 0; index < 8; ++index)
              {
                IntVector2 key = intVector2 + cardinalsAndOrdinals[index];
                if (!intVector2Set1.Contains(key) && !intVector2Set2.Contains(key) && !intVector2Set3.Contains(key) && cellsToEncarpet.Contains(key))
                {
                  intVector2Set3.Add(key);
                  dictionary.Add(key, carpetGrid.centerIndices.indices[Mathf.Clamp(num, 0, carpetGrid.centerIndices.indices.Count - 1)]);
                }
              }
            }
            intVector2Set2 = intVector2Set3;
            intVector2Set3 = new HashSet<IntVector2>();
            ++num;
          }
          if (num < 3)
            dictionary.Clear();
        }
        foreach (IntVector2 key in cellsToEncarpet)
        {
          bool[] flagArray = new bool[8];
          for (int index = 0; index < flagArray.Length; ++index)
            flagArray[index] = !cellsToEncarpet.Contains(key + cardinalsAndOrdinals[index]);
          bool flag = !flagArray[0] && !flagArray[1] && !flagArray[2] && !flagArray[3] && !flagArray[4] && !flagArray[5] && !flagArray[6] && !flagArray[7];
          int tile = !dictionary.ContainsKey(key) ? (!flag || !carpetGrid.CenterIndicesAreStrata ? carpetGrid.GetIndexGivenSides(flagArray[0], flagArray[1], flagArray[2], flagArray[3], flagArray[4], flagArray[5], flagArray[6], flagArray[7]) : carpetGrid.centerIndices.indices[0]) : dictionary[key];
          map.SetTile(key.x, key.y, GlobalDungeonData.patternLayerIndex, tile);
          d.data[key].cellVisualData.floorType = CellVisualData.CellFloorType.Carpet;
          d.data[key].cellVisualData.floorTileOverridden = true;
        }
      }

      public void HandleRoomDecorationMinimal(RoomHandler r, Dungeon d, tk2dTileMap map)
      {
        this.roomUsedStamps.Clear();
        if ((UnityEngine.Object) r.area.prototypeRoom == (UnityEngine.Object) null)
          return;
        if (this.viableCategorySets == null)
        {
          this.BuildStampLookupTable(d);
          this.BuildValidPlacementLists();
        }
        this.ProcessHardcodedUpholstery(r, d, map);
        this.roomUsedStamps.Clear();
      }

      public void HandleRoomDecoration(RoomHandler r, Dungeon d, tk2dTileMap map)
      {
        this.roomUsedStamps.Clear();
        this.ProcessHardcodedUpholstery(r, d, map);
        if ((UnityEngine.Object) r.area.prototypeRoom == (UnityEngine.Object) null || !r.area.prototypeRoom.preventAddedDecoLayering)
        {
          this.UpholsterRoom(r, d, map);
          if (!r.ForcePreventChannels)
            this.DigChannels(r, d, map);
        }
        if (this.viableCategorySets == null)
        {
          this.BuildStampLookupTable(d);
          this.BuildValidPlacementLists();
        }
        for (int index = 0; index < r.area.instanceUsedExits.Count; ++index)
        {
          PrototypeRoomExit instanceUsedExit = r.area.instanceUsedExits[index];
          RoomHandler roomHandler = r.connectedRoomsByExit[instanceUsedExit];
          if (roomHandler != null && (!((UnityEngine.Object) roomHandler.area.prototypeRoom != (UnityEngine.Object) null) || roomHandler.area.PrototypeRoomCategory != PrototypeDungeonRoom.RoomCategory.SECRET))
            this.DecorateRoomExit(r, r.area.exitToLocalDataMap[instanceUsedExit], d, map);
        }
        List<TK2DInteriorDecorator.WallExpanse> wallExpanseList1 = r.GatherExpanses(DungeonData.Direction.NORTH, false);
        for (int index1 = 0; index1 < wallExpanseList1.Count; ++index1)
        {
          TK2DInteriorDecorator.WallExpanse wallExpanse1 = wallExpanseList1[index1];
          TK2DInteriorDecorator.WallExpanse? nullable = new TK2DInteriorDecorator.WallExpanse?();
          int index2 = -1;
          for (int index3 = index1 + 1; index3 < wallExpanseList1.Count; ++index3)
          {
            TK2DInteriorDecorator.WallExpanse wallExpanse2 = wallExpanseList1[index3];
            if (wallExpanse1.basePosition.y == wallExpanse2.basePosition.y && wallExpanse1.width == wallExpanse2.width)
            {
              bool flag = true;
              for (int index4 = 0; index4 < wallExpanse2.width; ++index4)
              {
                if (d.data[r.area.basePosition + wallExpanse1.basePosition + IntVector2.Up + IntVector2.Right * index4].cellVisualData.forcedMatchingStyle != d.data[r.area.basePosition + wallExpanse2.basePosition + IntVector2.Up + IntVector2.Right * index4].cellVisualData.forcedMatchingStyle)
                {
                  flag = false;
                  break;
                }
              }
              if (flag)
              {
                nullable = new TK2DInteriorDecorator.WallExpanse?(wallExpanse2);
                index2 = index3;
              }
            }
          }
          if (nullable.HasValue)
          {
            wallExpanse1.hasMirror = true;
            wallExpanse1.mirroredExpanseBasePosition = nullable.Value.basePosition;
            wallExpanse1.mirroredExpanseWidth = nullable.Value.width;
            wallExpanseList1.RemoveAt(index2);
            wallExpanseList1[index1] = wallExpanse1;
          }
        }
        this.wallPlacementOffsets = new Dictionary<DungeonTileStampData.StampPlacementRule, IntVector2>();
        this.wallPlacementOffsets.Add(DungeonTileStampData.StampPlacementRule.BELOW_LOWER_FACEWALL, IntVector2.Zero);
        this.wallPlacementOffsets.Add(DungeonTileStampData.StampPlacementRule.BELOW_LOWER_FACEWALL_LEFT_CORNER, IntVector2.Zero);
        this.wallPlacementOffsets.Add(DungeonTileStampData.StampPlacementRule.BELOW_LOWER_FACEWALL_RIGHT_CORNER, IntVector2.Zero);
        this.wallPlacementOffsets.Add(DungeonTileStampData.StampPlacementRule.ON_LOWER_FACEWALL, IntVector2.Up);
        this.wallPlacementOffsets.Add(DungeonTileStampData.StampPlacementRule.ON_UPPER_FACEWALL, IntVector2.Up * 2);
        this.wallPlacementOffsets.Add(DungeonTileStampData.StampPlacementRule.ON_ANY_FLOOR, IntVector2.Zero);
        this.wallPlacementOffsets.Add(DungeonTileStampData.StampPlacementRule.ON_ANY_CEILING, IntVector2.Zero);
        this.wallPlacementOffsets.Add(DungeonTileStampData.StampPlacementRule.ALONG_LEFT_WALLS, IntVector2.Left);
        this.wallPlacementOffsets.Add(DungeonTileStampData.StampPlacementRule.ALONG_RIGHT_WALLS, IntVector2.Zero);
        this.wallPlacementOffsets.Add(DungeonTileStampData.StampPlacementRule.ON_TOPWALL, IntVector2.Zero);
        for (int index5 = 0; index5 < wallExpanseList1.Count; ++index5)
        {
          this.expanseUsedStamps.Clear();
          TK2DInteriorDecorator.WallExpanse expanse = wallExpanseList1[index5];
          if (expanse.hasMirror)
            this.DecorateExpanseRandom(expanse, r, d, map);
          else if (expanse.width > 2)
          {
            float num = UnityEngine.Random.value;
            for (int index6 = 0; index6 < expanse.width; ++index6)
            {
              if (d.data[r.area.basePosition + expanse.basePosition + IntVector2.Up + IntVector2.Right * index6].cellVisualData.forcedMatchingStyle != DungeonTileStampData.IntermediaryMatchingStyle.ANY)
                num = 1000f;
            }
            if ((double) num < (double) d.stampData.SymmetricFrameChance)
              this.DecorateExpanseSymmetricFrame(1, expanse, r, d, map);
            else if ((double) num >= (double) d.stampData.SymmetricFrameChance && (double) num < (double) d.stampData.SymmetricFrameChance + (double) d.stampData.SymmetricCompleteChance)
              this.DecorateExpanseSymmetric(expanse, r, d, map);
            else
              this.DecorateExpanseRandom(expanse, r, d, map);
          }
          else
            this.DecorateExpanseRandom(expanse, r, d, map);
        }
        this.DecorateCeilingCorners(r, d, map);
        List<TK2DInteriorDecorator.WallExpanse> wallExpanseList2 = r.GatherExpanses(DungeonData.Direction.EAST, false);
        List<TK2DInteriorDecorator.WallExpanse> wallExpanseList3 = r.GatherExpanses(DungeonData.Direction.WEST, false);
        for (int index = 0; index < wallExpanseList2.Count; ++index)
        {
          TK2DInteriorDecorator.WallExpanse wallExpanse = wallExpanseList2[index];
          if (wallExpanse.width > 1)
          {
            --wallExpanse.width;
            wallExpanseList2[index] = wallExpanse;
          }
          else
          {
            wallExpanseList2.RemoveAt(index);
            --index;
          }
        }
        for (int index = 0; index < wallExpanseList3.Count; ++index)
        {
          TK2DInteriorDecorator.WallExpanse wallExpanse = wallExpanseList3[index];
          if (wallExpanse.width > 1)
          {
            --wallExpanse.width;
            wallExpanseList3[index] = wallExpanse;
          }
          else
          {
            wallExpanseList3.RemoveAt(index);
            --index;
          }
        }
        for (int index7 = 0; index7 < wallExpanseList2.Count; ++index7)
        {
          this.expanseUsedStamps.Clear();
          TK2DInteriorDecorator.WallExpanse expanse = wallExpanseList2[index7];
          TK2DInteriorDecorator.WallExpanse? nullable = new TK2DInteriorDecorator.WallExpanse?();
          for (int index8 = 0; index8 < wallExpanseList3.Count; ++index8)
          {
            TK2DInteriorDecorator.WallExpanse wallExpanse = wallExpanseList3[index8];
            if (wallExpanse.basePosition.y == expanse.basePosition.y && wallExpanse.width == expanse.width)
            {
              nullable = new TK2DInteriorDecorator.WallExpanse?(wallExpanse);
              wallExpanseList3.RemoveAt(index8);
              break;
            }
          }
          int num1 = 1;
          while (true)
          {
            int viableWidth = expanse.width - num1;
            if (viableWidth != 0)
            {
              IntVector2 basePosition1 = r.area.basePosition + expanse.basePosition + num1 * IntVector2.Up;
              StampDataBase placedStamp1 = (StampDataBase) null;
              TK2DInteriorDecorator.DecorateErrorCode decorateErrorCode = this.DecorateWallSection(basePosition1, viableWidth, r, d, map, this.validEasternPlacements, expanse, out placedStamp1, Mathf.Max(0.55f, r.RoomMaterial.stampFailChance), true);
              if (decorateErrorCode != TK2DInteriorDecorator.DecorateErrorCode.FAILED_SPACE)
              {
                if (placedStamp1 == null || decorateErrorCode == TK2DInteriorDecorator.DecorateErrorCode.FAILED_CHANCE)
                {
                  ++num1;
                }
                else
                {
                  if (nullable.HasValue)
                  {
                    IntVector2 basePosition2 = r.area.basePosition + nullable.Value.basePosition + (expanse.width - viableWidth) * IntVector2.Up;
                    StampDataBase placedStamp2 = (StampDataBase) null;
                    int num2 = (int) this.DecorateWallSection(basePosition2, viableWidth, r, d, map, this.validWesternPlacements, nullable.Value, out placedStamp2, 0.0f, true);
                  }
                  num1 += placedStamp1.height;
                }
              }
              else
                break;
            }
            else
              break;
          }
        }
        for (int index = 0; index < wallExpanseList3.Count; ++index)
        {
          this.expanseUsedStamps.Clear();
          TK2DInteriorDecorator.WallExpanse expanse = wallExpanseList3[index];
          int num = 1;
          while (true)
          {
            int viableWidth = expanse.width - num;
            if (viableWidth != 0)
            {
              IntVector2 basePosition = r.area.basePosition + expanse.basePosition + num * IntVector2.Up;
              StampDataBase placedStamp = (StampDataBase) null;
              TK2DInteriorDecorator.DecorateErrorCode decorateErrorCode = this.DecorateWallSection(basePosition, viableWidth, r, d, map, this.validWesternPlacements, expanse, out placedStamp, Mathf.Max(0.55f, r.RoomMaterial.stampFailChance), true);
              if (decorateErrorCode != TK2DInteriorDecorator.DecorateErrorCode.FAILED_SPACE)
              {
                if (placedStamp == null || decorateErrorCode == TK2DInteriorDecorator.DecorateErrorCode.FAILED_CHANCE)
                  ++num;
                else
                  num += placedStamp.height;
              }
              else
                break;
            }
            else
              break;
          }
        }
        List<TK2DInteriorDecorator.WallExpanse> wallExpanseList4 = r.GatherExpanses(DungeonData.Direction.SOUTH);
    label_89:
        for (int index = 0; index < wallExpanseList4.Count; ++index)
        {
          this.expanseUsedStamps.Clear();
          TK2DInteriorDecorator.WallExpanse expanse = wallExpanseList4[index];
          int num = 1;
          while (true)
          {
            StampDataBase placedStamp;
            do
            {
              int viableWidth = Mathf.FloorToInt((float) (expanse.width - 2 * num) / 2f);
              if (viableWidth != 0)
              {
                IntVector2 basePosition = r.area.basePosition + expanse.basePosition + num * IntVector2.Right;
                placedStamp = (StampDataBase) null;
                TK2DInteriorDecorator.DecorateErrorCode decorateErrorCode = this.DecorateWallSection(basePosition, viableWidth, r, d, map, this.validSouthernPlacements, expanse, out placedStamp, 0.5f);
                if (decorateErrorCode != TK2DInteriorDecorator.DecorateErrorCode.FAILED_SPACE)
                {
                  if (placedStamp == null || decorateErrorCode == TK2DInteriorDecorator.DecorateErrorCode.FAILED_CHANCE)
                  {
                    ++num;
                  }
                  else
                  {
                    IntVector2 intVector2 = r.area.basePosition + expanse.basePosition + (expanse.width - num - placedStamp.width) * IntVector2.Right + this.wallPlacementOffsets[placedStamp.placementRule];
                    this.m_assembler.ApplyStampGeneric(intVector2.x, intVector2.y, placedStamp, d, map, overrideTileLayerIndex: GlobalDungeonData.aboveBorderLayerIndex);
                    num += placedStamp.width;
                  }
                }
                else
                  goto label_89;
              }
              else
                goto label_89;
            }
            while (placedStamp.width != 1);
            num += 2;
          }
        }
        for (int x = 2; x < r.area.dimensions.x - 2; ++x)
        {
          for (int y = 2; y < r.area.dimensions.y - 2; ++y)
          {
            IntVector2 basePosition = r.area.basePosition + new IntVector2(x, y);
            CellData cellData = d.data.cellData[basePosition.x][basePosition.y];
            if (cellData != null && cellData.type == CellType.FLOOR && !cellData.cellVisualData.floorTileOverridden && !cellData.cellVisualData.preventFloorStamping)
            {
              StampDataBase placedStamp = (StampDataBase) null;
              float failChance = 0.8f;
              if (d.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.OFFICEGEON)
                failChance = 0.99f;
              this.DecorateFloorSquare(basePosition, r, d, map, out placedStamp, failChance);
            }
          }
        }
        this.roomUsedStamps.Clear();
      }

      private void DecorateCeilingCorners(RoomHandler r, Dungeon d, tk2dTileMap map)
      {
        if ((UnityEngine.Object) d.tileIndices.edgeDecorationTiles == (UnityEngine.Object) null || r == d.data.Entrance || r == d.data.Exit)
          return;
        CellArea area = r.area;
        for (int x = 0; x < area.dimensions.x; ++x)
        {
          for (int y = 0; y < area.dimensions.y; ++y)
          {
            IntVector2 intVector2 = area.basePosition + new IntVector2(x, y);
            CellData d1 = d.data.cellData[intVector2.x][intVector2.y];
            if (d1 != null && d1.type != CellType.WALL && d1.diagonalWallType == DiagonalWallType.NONE)
            {
              List<CellData> cellNeighbors = d.data.GetCellNeighbors(d1);
              bool isNorthBorder = cellNeighbors[0] != null && cellNeighbors[0].type == CellType.WALL && cellNeighbors[0].diagonalWallType == DiagonalWallType.NONE;
              bool isEastBorder = cellNeighbors[1] != null && cellNeighbors[1].type == CellType.WALL && cellNeighbors[1].diagonalWallType == DiagonalWallType.NONE;
              bool isSouthBorder = cellNeighbors[2] != null && cellNeighbors[2].type == CellType.WALL && cellNeighbors[2].diagonalWallType == DiagonalWallType.NONE;
              bool isWestBorder = cellNeighbors[3] != null && cellNeighbors[3].type == CellType.WALL && cellNeighbors[3].diagonalWallType == DiagonalWallType.NONE;
              int indexGivenSides = d.tileIndices.edgeDecorationTiles.GetIndexGivenSides(isNorthBorder, isEastBorder, isSouthBorder, isWestBorder);
              bool flag = (double) UnityEngine.Random.value < 0.25;
              if (indexGivenSides != -1 && flag)
              {
                int num = !isNorthBorder ? 1 : 2;
                map.SetTile(intVector2.x, intVector2.y + num, GlobalDungeonData.aboveBorderLayerIndex, indexGivenSides);
              }
            }
          }
        }
      }

      private void DecorateExpanseSymmetricFrame(
        int frameIterations,
        TK2DInteriorDecorator.WallExpanse expanse,
        RoomHandler r,
        Dungeon d,
        tk2dTileMap map)
      {
        int num = 0;
        for (int index = 0; index < frameIterations; ++index)
        {
          int viableWidth = Mathf.FloorToInt((float) (expanse.width - 2 * num) / 2f);
          if (viableWidth != 0)
          {
            IntVector2 basePosition = r.area.basePosition + expanse.basePosition + num * IntVector2.Right;
            StampDataBase placedStamp = (StampDataBase) null;
            TK2DInteriorDecorator.DecorateErrorCode decorateErrorCode = this.DecorateWallSection(basePosition, viableWidth, r, d, map, this.validNorthernPlacements, expanse, out placedStamp, r.RoomMaterial.stampFailChance);
            if (decorateErrorCode != TK2DInteriorDecorator.DecorateErrorCode.FAILED_SPACE)
            {
              if (placedStamp == null || decorateErrorCode == TK2DInteriorDecorator.DecorateErrorCode.FAILED_CHANCE)
              {
                ++num;
              }
              else
              {
                if (placedStamp.indexOfSymmetricPartner != -1)
                  placedStamp = (StampDataBase) d.stampData.objectStamps[placedStamp.indexOfSymmetricPartner];
                IntVector2 intVector2 = r.area.basePosition + expanse.basePosition + (expanse.width - num - placedStamp.width) * IntVector2.Right + this.wallPlacementOffsets[placedStamp.placementRule];
                if (!placedStamp.preventRoomRepeats)
                {
                  this.m_assembler.ApplyStampGeneric(intVector2.x, intVector2.y, placedStamp, d, map);
                }
                else
                {
                  StampDataBase forRoomDuplication = d.stampData.AttemptGetSimilarStampForRoomDuplication(placedStamp, this.roomUsedStamps, r.RoomVisualSubtype);
                  if (forRoomDuplication != null)
                  {
                    this.m_assembler.ApplyStampGeneric(intVector2.x, intVector2.y, forRoomDuplication, d, map);
                    this.roomUsedStamps.Add(forRoomDuplication);
                  }
                }
                if (this.DEBUG_DRAW)
                {
                  BraveUtility.DrawDebugSquare(basePosition.ToVector2(), (basePosition + IntVector2.Up + placedStamp.width * IntVector2.Right).ToVector2(), Color.red, 1000f);
                  BraveUtility.DrawDebugSquare(intVector2.ToVector2(), (intVector2 + IntVector2.Up + placedStamp.width * IntVector2.Right).ToVector2(), Color.red, 1000f);
                }
                num += placedStamp.width;
              }
            }
            else
              break;
          }
          else
            break;
        }
        int w = expanse.width - 2 * num;
        if (w <= 0)
          return;
        this.DecorateExpanseRandom(new TK2DInteriorDecorator.WallExpanse(expanse.basePosition + num * IntVector2.Right, w), r, d, map);
      }

      private void DecorateExpanseSymmetric(
        TK2DInteriorDecorator.WallExpanse expanse,
        RoomHandler r,
        Dungeon d,
        tk2dTileMap map)
      {
        int num = 0;
        while (true)
        {
          int viableWidth = Mathf.FloorToInt((float) (expanse.width - 2 * num) / 2f);
          if (viableWidth != 0)
          {
            IntVector2 basePosition = r.area.basePosition + expanse.basePosition + num * IntVector2.Right;
            StampDataBase placedStamp = (StampDataBase) null;
            TK2DInteriorDecorator.DecorateErrorCode decorateErrorCode = this.DecorateWallSection(basePosition, viableWidth, r, d, map, this.validNorthernPlacements, expanse, out placedStamp, r.RoomMaterial.stampFailChance);
            if (decorateErrorCode != TK2DInteriorDecorator.DecorateErrorCode.FAILED_SPACE)
            {
              if (placedStamp == null || decorateErrorCode == TK2DInteriorDecorator.DecorateErrorCode.FAILED_CHANCE)
              {
                ++num;
              }
              else
              {
                if (placedStamp.indexOfSymmetricPartner != -1)
                  placedStamp = (StampDataBase) d.stampData.objectStamps[placedStamp.indexOfSymmetricPartner];
                IntVector2 intVector2 = r.area.basePosition + expanse.basePosition + (expanse.width - num - placedStamp.width) * IntVector2.Right + this.wallPlacementOffsets[placedStamp.placementRule];
                if (!placedStamp.preventRoomRepeats)
                {
                  this.m_assembler.ApplyStampGeneric(intVector2.x, intVector2.y, placedStamp, d, map);
                }
                else
                {
                  StampDataBase forRoomDuplication = d.stampData.AttemptGetSimilarStampForRoomDuplication(placedStamp, this.roomUsedStamps, r.RoomVisualSubtype);
                  if (forRoomDuplication != null)
                  {
                    this.m_assembler.ApplyStampGeneric(intVector2.x, intVector2.y, forRoomDuplication, d, map);
                    this.roomUsedStamps.Add(forRoomDuplication);
                  }
                }
                if (this.DEBUG_DRAW)
                {
                  BraveUtility.DrawDebugSquare(basePosition.ToVector2(), (basePosition + IntVector2.Up + placedStamp.width * IntVector2.Right).ToVector2(), Color.yellow, 1000f);
                  BraveUtility.DrawDebugSquare(intVector2.ToVector2(), (intVector2 + IntVector2.Up + placedStamp.width * IntVector2.Right).ToVector2(), Color.yellow, 1000f);
                }
                num += placedStamp.width;
              }
            }
            else
              goto label_2;
          }
          else
            break;
        }
        return;
    label_2:;
      }

      private void DecorateExpanseRandom(
        TK2DInteriorDecorator.WallExpanse expanse,
        RoomHandler r,
        Dungeon d,
        tk2dTileMap map)
      {
        int basePlacement = 0;
        while (true)
        {
          int viableWidth = expanse.width - basePlacement;
          if (viableWidth != 0)
          {
            IntVector2 basePosition = r.area.basePosition + expanse.basePosition + basePlacement * IntVector2.Right;
            StampDataBase placedStamp = (StampDataBase) null;
            TK2DInteriorDecorator.DecorateErrorCode decorateErrorCode = this.DecorateWallSection(basePosition, viableWidth, r, d, map, this.validNorthernPlacements, expanse, out placedStamp, r.RoomMaterial.stampFailChance);
            if (decorateErrorCode != TK2DInteriorDecorator.DecorateErrorCode.FAILED_SPACE)
            {
              if (placedStamp == null || decorateErrorCode == TK2DInteriorDecorator.DecorateErrorCode.FAILED_CHANCE)
              {
                ++basePlacement;
              }
              else
              {
                if (expanse.hasMirror)
                {
                  IntVector2 intVector2_1 = r.area.basePosition + expanse.GetPositionInMirroredExpanse(basePlacement, placedStamp.width);
                  Debug.DrawLine(intVector2_1.ToVector3(), intVector2_1.ToVector3() + new Vector3(1f, 1f, 0.0f), Color.cyan, 1000f);
                  if (placedStamp.indexOfSymmetricPartner != -1)
                    placedStamp = (StampDataBase) d.stampData.objectStamps[placedStamp.indexOfSymmetricPartner];
                  IntVector2 intVector2_2 = intVector2_1 + this.wallPlacementOffsets[placedStamp.placementRule];
                  if (!placedStamp.preventRoomRepeats)
                  {
                    this.m_assembler.ApplyStampGeneric(intVector2_2.x, intVector2_2.y, placedStamp, d, map);
                  }
                  else
                  {
                    StampDataBase forRoomDuplication = d.stampData.AttemptGetSimilarStampForRoomDuplication(placedStamp, this.roomUsedStamps, r.RoomVisualSubtype);
                    if (forRoomDuplication != null)
                    {
                      this.m_assembler.ApplyStampGeneric(intVector2_2.x, intVector2_2.y, forRoomDuplication, d, map);
                      this.roomUsedStamps.Add(forRoomDuplication);
                    }
                  }
                }
                if (this.DEBUG_DRAW)
                  BraveUtility.DrawDebugSquare(basePosition.ToVector2(), (basePosition + IntVector2.Up + placedStamp.width * IntVector2.Right).ToVector2(), Color.magenta, 1000f);
                basePlacement += placedStamp.width;
              }
            }
            else
              goto label_2;
          }
          else
            break;
        }
        return;
    label_2:;
      }

      private DungeonTileStampData.StampSpace GetValidSpaceForSection(
        IntVector2 basePosition,
        int viableWidth,
        Dungeon d)
      {
        List<DungeonTileStampData.StampSpace> stampSpaceList = new List<DungeonTileStampData.StampSpace>();
        stampSpaceList.Add(DungeonTileStampData.StampSpace.OBJECT_SPACE);
        bool flag = true;
        for (int index = 0; index < viableWidth; ++index)
        {
          IntVector2 intVector2_1 = basePosition + IntVector2.Up + IntVector2.Right * index;
          CellVisualData cellVisualData = d.data.cellData[intVector2_1.x][intVector2_1.y].cellVisualData;
          if (cellVisualData.containsWallSpaceStamp)
          {
            flag = false;
            break;
          }
          IntVector2 intVector2_2 = intVector2_1 + IntVector2.Up;
          cellVisualData = d.data.cellData[intVector2_2.x][intVector2_2.y].cellVisualData;
          if (cellVisualData.containsWallSpaceStamp)
          {
            flag = false;
            break;
          }
        }
        if (flag)
        {
          stampSpaceList.Add(DungeonTileStampData.StampSpace.WALL_SPACE);
          stampSpaceList.Add(DungeonTileStampData.StampSpace.BOTH_SPACES);
        }
        return stampSpaceList[UnityEngine.Random.Range(0, stampSpaceList.Count)];
      }

      private void BuildValidPlacementLists()
      {
        this.validNorthernPlacements = new List<DungeonTileStampData.StampPlacementRule>();
        this.validNorthernPlacements.Add(DungeonTileStampData.StampPlacementRule.ABOVE_UPPER_FACEWALL);
        this.validNorthernPlacements.Add(DungeonTileStampData.StampPlacementRule.BELOW_LOWER_FACEWALL);
        this.validNorthernPlacements.Add(DungeonTileStampData.StampPlacementRule.ON_LOWER_FACEWALL);
        this.validNorthernPlacements.Add(DungeonTileStampData.StampPlacementRule.ON_UPPER_FACEWALL);
        this.validEasternPlacements = new List<DungeonTileStampData.StampPlacementRule>();
        this.validEasternPlacements.Add(DungeonTileStampData.StampPlacementRule.ALONG_RIGHT_WALLS);
        this.validEasternPlacements.Add(DungeonTileStampData.StampPlacementRule.BELOW_LOWER_FACEWALL);
        this.validWesternPlacements = new List<DungeonTileStampData.StampPlacementRule>();
        this.validWesternPlacements.Add(DungeonTileStampData.StampPlacementRule.ALONG_LEFT_WALLS);
        this.validWesternPlacements.Add(DungeonTileStampData.StampPlacementRule.BELOW_LOWER_FACEWALL);
        this.validSouthernPlacements = new List<DungeonTileStampData.StampPlacementRule>();
        this.validSouthernPlacements.Add(DungeonTileStampData.StampPlacementRule.ON_TOPWALL);
      }

      private void BuildStampLookupTable(Dungeon d)
      {
        this.viableCategorySets = new List<TK2DInteriorDecorator.ViableStampCategorySet>();
        for (int index = 0; index < d.stampData.stamps.Length; ++index)
        {
          StampDataBase stamp = (StampDataBase) d.stampData.stamps[index];
          TK2DInteriorDecorator.ViableStampCategorySet stampCategorySet = new TK2DInteriorDecorator.ViableStampCategorySet(stamp.stampCategory, stamp.placementRule, stamp.occupySpace);
          if (!this.viableCategorySets.Contains(stampCategorySet))
            this.viableCategorySets.Add(stampCategorySet);
        }
        for (int index = 0; index < d.stampData.spriteStamps.Length; ++index)
        {
          StampDataBase spriteStamp = (StampDataBase) d.stampData.spriteStamps[index];
          TK2DInteriorDecorator.ViableStampCategorySet stampCategorySet = new TK2DInteriorDecorator.ViableStampCategorySet(spriteStamp.stampCategory, spriteStamp.placementRule, spriteStamp.occupySpace);
          if (!this.viableCategorySets.Contains(stampCategorySet))
            this.viableCategorySets.Add(stampCategorySet);
        }
        for (int index = 0; index < d.stampData.objectStamps.Length; ++index)
        {
          StampDataBase objectStamp = (StampDataBase) d.stampData.objectStamps[index];
          TK2DInteriorDecorator.ViableStampCategorySet stampCategorySet = new TK2DInteriorDecorator.ViableStampCategorySet(objectStamp.stampCategory, objectStamp.placementRule, objectStamp.occupySpace);
          if (!this.viableCategorySets.Contains(stampCategorySet))
            this.viableCategorySets.Add(stampCategorySet);
        }
      }

      private TK2DInteriorDecorator.ViableStampCategorySet GetCategorySet(
        List<DungeonTileStampData.StampPlacementRule> validRules)
      {
        List<TK2DInteriorDecorator.ViableStampCategorySet> stampCategorySetList = new List<TK2DInteriorDecorator.ViableStampCategorySet>();
        for (int index = 0; index < this.viableCategorySets.Count; ++index)
        {
          if (validRules.Contains(this.viableCategorySets[index].placement))
            stampCategorySetList.Add(this.viableCategorySets[index]);
        }
        return stampCategorySetList.Count == 0 ? (TK2DInteriorDecorator.ViableStampCategorySet) null : stampCategorySetList[UnityEngine.Random.Range(0, stampCategorySetList.Count)];
      }

      private bool CheckExpanseStampValidity(
        TK2DInteriorDecorator.WallExpanse expanse,
        StampDataBase stamp)
      {
        if (stamp.preventRoomRepeats && this.roomUsedStamps.Contains(stamp))
          return false;
        int intermediaryStamps = stamp.preferredIntermediaryStamps;
        for (int index1 = 0; index1 < intermediaryStamps; ++index1)
        {
          int index2 = this.expanseUsedStamps.Count - (1 + index1);
          if (index2 >= 0)
          {
            if (stamp.intermediaryMatchingStyle == DungeonTileStampData.IntermediaryMatchingStyle.ANY)
            {
              if (this.expanseUsedStamps[index2] == stamp)
                return false;
            }
            else if (this.expanseUsedStamps[index2].intermediaryMatchingStyle == stamp.intermediaryMatchingStyle)
              return false;
          }
          else
            break;
        }
        return true;
      }

      private bool DecorateFloorSquare(
        IntVector2 basePosition,
        RoomHandler r,
        Dungeon d,
        tk2dTileMap map,
        out StampDataBase placedStamp,
        float failChance = 0.2f)
      {
        if ((double) UnityEngine.Random.value < (double) failChance)
        {
          placedStamp = (StampDataBase) null;
          return true;
        }
        placedStamp = (StampDataBase) null;
        List<DungeonTileStampData.StampPlacementRule> stampPlacementRuleList = new List<DungeonTileStampData.StampPlacementRule>();
        stampPlacementRuleList.Add(DungeonTileStampData.StampPlacementRule.ON_ANY_FLOOR);
        TK2DInteriorDecorator.ViableStampCategorySet categorySet = this.GetCategorySet(stampPlacementRuleList);
        if (categorySet == null)
          return false;
        StampDataBase stampDataComplex = d.stampData.GetStampDataComplex(stampPlacementRuleList, categorySet.space, categorySet.category, r.opulence, r.RoomVisualSubtype, 1);
        if (stampDataComplex == null)
          return false;
        IntVector2 intVector2 = basePosition + this.wallPlacementOffsets[stampDataComplex.placementRule];
        this.m_assembler.ApplyStampGeneric(intVector2.x, intVector2.y, stampDataComplex, d, map);
        placedStamp = stampDataComplex;
        return true;
      }

      private TK2DInteriorDecorator.DecorateErrorCode DecorateWallSection(
        IntVector2 basePosition,
        int viableWidth,
        RoomHandler r,
        Dungeon d,
        tk2dTileMap map,
        List<DungeonTileStampData.StampPlacementRule> validRules,
        TK2DInteriorDecorator.WallExpanse expanse,
        out StampDataBase placedStamp,
        float failChance = 0.2f,
        bool excludeWallSpace = false)
      {
        if (GameManager.Options.DebrisQuantity == GameOptions.GenericHighMedLowOption.VERY_LOW)
          failChance = Mathf.Min(failChance * 2f, 0.75f);
        if ((double) UnityEngine.Random.value < (double) failChance)
        {
          placedStamp = (StampDataBase) null;
          return TK2DInteriorDecorator.DecorateErrorCode.FAILED_CHANCE;
        }
        StampDataBase stampDataBase = (StampDataBase) null;
        if (validRules.Contains(DungeonTileStampData.StampPlacementRule.BELOW_LOWER_FACEWALL))
        {
          if (d.data.GetCellTypeSafe(basePosition + IntVector2.Left) == CellType.WALL)
            validRules.Add(DungeonTileStampData.StampPlacementRule.BELOW_LOWER_FACEWALL_LEFT_CORNER);
          if (d.data.GetCellTypeSafe(basePosition + IntVector2.Right) == CellType.WALL)
            validRules.Add(DungeonTileStampData.StampPlacementRule.BELOW_LOWER_FACEWALL_RIGHT_CORNER);
        }
        for (int index = 0; index < 10; ++index)
        {
          if (!d.data.CheckInBoundsAndValid(basePosition) || !d.data.CheckInBoundsAndValid(basePosition + IntVector2.Up))
          {
            stampDataBase = (StampDataBase) null;
            break;
          }
          if (d.data[basePosition + IntVector2.Up].cellVisualData.forcedMatchingStyle == DungeonTileStampData.IntermediaryMatchingStyle.ANY)
          {
            stampDataBase = d.stampData.GetStampDataSimple(validRules, r.opulence, r.RoomVisualSubtype, viableWidth, excludeWallSpace, this.roomUsedStamps);
            if (stampDataBase != null && stampDataBase.requiresForcedMatchingStyle)
              continue;
          }
          else
          {
            BraveUtility.DrawDebugSquare((basePosition + IntVector2.Up).ToVector2() + new Vector2(0.2f, 0.2f), (basePosition + IntVector2.Up + IntVector2.One).ToVector2() + new Vector2(-0.2f, -0.2f), Color.red, 1000f);
            stampDataBase = d.stampData.GetStampDataSimpleWithForcedRule(validRules, d.data[basePosition + IntVector2.Up].cellVisualData.forcedMatchingStyle, r.opulence, r.RoomVisualSubtype, viableWidth, excludeWallSpace);
            if (stampDataBase != null && stampDataBase.intermediaryMatchingStyle != d.data[basePosition + IntVector2.Up].cellVisualData.forcedMatchingStyle)
              break;
          }
          if (stampDataBase != null)
          {
            if (!excludeWallSpace || stampDataBase.width <= 1)
            {
              if (!this.CheckExpanseStampValidity(expanse, stampDataBase))
                stampDataBase = (StampDataBase) null;
              else
                break;
            }
          }
          else
            break;
        }
        validRules.Remove(DungeonTileStampData.StampPlacementRule.BELOW_LOWER_FACEWALL_LEFT_CORNER);
        validRules.Remove(DungeonTileStampData.StampPlacementRule.BELOW_LOWER_FACEWALL_RIGHT_CORNER);
        if (stampDataBase == null)
        {
          placedStamp = (StampDataBase) null;
          return TK2DInteriorDecorator.DecorateErrorCode.FAILED_SPACE;
        }
        this.expanseUsedStamps.Add(stampDataBase);
        this.roomUsedStamps.Add(stampDataBase);
        IntVector2 intVector2 = basePosition + this.wallPlacementOffsets[stampDataBase.placementRule];
        int overrideTileLayerIndex = stampDataBase.placementRule != DungeonTileStampData.StampPlacementRule.ALONG_LEFT_WALLS && stampDataBase.placementRule != DungeonTileStampData.StampPlacementRule.ALONG_RIGHT_WALLS && stampDataBase.placementRule != DungeonTileStampData.StampPlacementRule.ON_TOPWALL ? -1 : GlobalDungeonData.aboveBorderLayerIndex;
        this.m_assembler.ApplyStampGeneric(intVector2.x, intVector2.y, stampDataBase, d, map, overrideTileLayerIndex: overrideTileLayerIndex);
        placedStamp = stampDataBase;
        return TK2DInteriorDecorator.DecorateErrorCode.ALL_OK;
      }

      protected class ViableStampCategorySet
      {
        public DungeonTileStampData.StampCategory category;
        public DungeonTileStampData.StampPlacementRule placement;
        public DungeonTileStampData.StampSpace space;

        public ViableStampCategorySet(
          DungeonTileStampData.StampCategory c,
          DungeonTileStampData.StampPlacementRule p,
          DungeonTileStampData.StampSpace s)
        {
          this.category = c;
          this.placement = p;
          this.space = s;
        }

        public override int GetHashCode()
        {
          return 1597 * this.category.GetHashCode() + 5347 * this.placement.GetHashCode() + 13 * this.space.GetHashCode();
        }

        public override bool Equals(object obj)
        {
          if (!(obj is TK2DInteriorDecorator.ViableStampCategorySet))
            return false;
          TK2DInteriorDecorator.ViableStampCategorySet stampCategorySet = obj as TK2DInteriorDecorator.ViableStampCategorySet;
          return stampCategorySet.category == this.category && stampCategorySet.space == this.space && stampCategorySet.placement == this.placement;
        }
      }

      public enum DecorateErrorCode
      {
        ALL_OK,
        FAILED_SPACE,
        FAILED_CHANCE,
      }

      public struct WallExpanse
      {
        public IntVector2 basePosition;
        public int width;
        public bool hasMirror;
        public IntVector2 mirroredExpanseBasePosition;
        public int mirroredExpanseWidth;

        public WallExpanse(IntVector2 bp, int w)
        {
          basePosition = bp;
          width = w;
          hasMirror = false;
          mirroredExpanseBasePosition = IntVector2.Zero;
          mirroredExpanseWidth = 0;
        }

        public IntVector2 GetPositionInMirroredExpanse(int basePlacement, int stampWidth)
        {
          return this.mirroredExpanseBasePosition + this.mirroredExpanseWidth * IntVector2.Right + (basePlacement + stampWidth) * IntVector2.Left;
        }
      }
    }

}
