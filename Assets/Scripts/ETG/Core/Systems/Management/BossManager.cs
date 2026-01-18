using System.Collections.Generic;
using UnityEngine;

#nullable disable

public class BossManager : ScriptableObject
  {
    public static bool HasOverriddenCoreBoss;
    public static PrototypeDungeonRoom PriorFloorSelectedBossRoom;
    [SerializeField]
    public List<BossFloorEntry> BossFloorData;
    [SerializeField]
    public List<OverrideBossFloorEntry> OverrideBosses;

    private BossFloorEntry GetBossDataForFloor(GlobalDungeonData.ValidTilesets targetTileset)
    {
      BossFloorEntry bossDataForFloor = (BossFloorEntry) null;
      for (int index = 0; index < this.BossFloorData.Count; ++index)
      {
        if ((this.BossFloorData[index].AssociatedTilesets | targetTileset) == this.BossFloorData[index].AssociatedTilesets)
          bossDataForFloor = this.BossFloorData[index];
      }
      if (bossDataForFloor == null)
        bossDataForFloor = this.BossFloorData[0];
      return bossDataForFloor;
    }

    public PrototypeDungeonRoom SelectBossRoom()
    {
      if ((Object) BossManager.PriorFloorSelectedBossRoom != (Object) null)
        return BossManager.PriorFloorSelectedBossRoom;
      GenericRoomTable genericRoomTable = this.SelectBossTable();
      if ((Object) genericRoomTable == (Object) null)
        genericRoomTable = this.GetBossDataForFloor(GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId).Bosses[0].TargetRoomTable;
      if (!BossManager.HasOverriddenCoreBoss)
      {
        for (int index = 0; index < this.OverrideBosses.Count; ++index)
        {
          if (this.OverrideBosses[index].GlobalPrereqsValid(GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId) && (double) BraveRandom.GenerationRandomValue() < (double) this.OverrideBosses[index].ChanceToOverride)
          {
            BossManager.HasOverriddenCoreBoss = true;
            Debug.Log((object) ("Boss overridden: " + this.OverrideBosses[index].Annotation));
            genericRoomTable = this.OverrideBosses[index].TargetRoomTable;
            break;
          }
        }
      }
      if (GameStatsManager.Instance.LastBossEncounteredMap.ContainsKey(GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId))
        GameStatsManager.Instance.LastBossEncounteredMap[GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId] = genericRoomTable.name;
      else
        GameStatsManager.Instance.LastBossEncounteredMap.Add(GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId, genericRoomTable.name);
      WeightedRoom weightedRoom = genericRoomTable.SelectByWeight();
      if (weightedRoom == null && (Object) genericRoomTable != (Object) null && genericRoomTable.includedRooms.elements.Count > 0)
        weightedRoom = genericRoomTable.includedRooms.elements[0];
      if (weightedRoom == null)
      {
        Debug.LogError((object) "BOSS FAILED TO SELECT");
        return (PrototypeDungeonRoom) null;
      }
      BossManager.PriorFloorSelectedBossRoom = weightedRoom.room;
      return weightedRoom.room;
    }

    public GenericRoomTable SelectBossTable()
    {
      return this.GetBossDataForFloor(GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId).SelectBoss().TargetRoomTable;
    }
  }

