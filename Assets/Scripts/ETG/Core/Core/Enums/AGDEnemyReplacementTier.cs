using Dungeonator;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

[Serializable]
public class AGDEnemyReplacementTier
  {
    public string Name;
    public DungeonPrerequisite[] Prereqs;
    [EnumFlags]
    public GlobalDungeonData.ValidTilesets TargetTileset;
    public float ChanceToReplace = 0.2f;
    public int MaxPerFloor = -1;
    public int MaxPerRun = -1;
    public bool TargetAllSignatureEnemies;
    public bool TargetAllNonSignatureEnemies;
    [EnemyIdentifier]
    public List<string> TargetGuids;
    [EnemyIdentifier]
    public List<string> ReplacementGuids;
    [Header("Exclusion Rules")]
    public bool RoomMustHaveColumns;
    public int RoomMinEnemyCount = -1;
    public int RoomMaxEnemyCount = -1;
    public int RoomMinSize = -1;
    [EnemyIdentifier]
    public List<string> RoomCantContain;
    [Header("Extras")]
    public bool RemoveAllOtherEnemies;

    public bool ExcludeForPrereqs() => !DungeonPrerequisite.CheckConditionsFulfilled(this.Prereqs);

    public bool ExcludeRoomForColumns(DungeonData data, RoomHandler room)
    {
      if (!this.RoomMustHaveColumns)
        return false;
      for (int index1 = 0; index1 < room.area.dimensions.x; ++index1)
      {
        for (int index2 = 0; index2 < room.area.dimensions.y; ++index2)
        {
          CellData cellData = data[room.area.basePosition.x + index1, room.area.basePosition.y + index2];
          if (cellData != null && cellData.type == CellType.WALL && cellData.isRoomInternal)
            return false;
        }
      }
      return true;
    }

    public bool ExcludeRoomForEnemies(RoomHandler room, List<AIActor> activeEnemies)
    {
      if (this.RoomCantContain.Count <= 0)
        return false;
      for (int index = 0; index < activeEnemies.Count; ++index)
      {
        AIActor activeEnemy = activeEnemies[index];
        if ((bool) (UnityEngine.Object) activeEnemy && this.RoomCantContain.Contains(activeEnemy.EnemyGuid))
          return true;
      }
      return false;
    }

    public bool ExcludeRoom(RoomHandler room)
    {
      return this.RoomMinSize > 0 && (room.area.dimensions.x < this.RoomMinSize || room.area.dimensions.y < this.RoomMinSize) || this.RoomMinEnemyCount > 0 && room.GetActiveEnemiesCount(RoomHandler.ActiveEnemyType.All) < this.RoomMinEnemyCount || this.RoomMaxEnemyCount > 0 && room.GetActiveEnemiesCount(RoomHandler.ActiveEnemyType.All) > this.RoomMaxEnemyCount;
    }
  }

