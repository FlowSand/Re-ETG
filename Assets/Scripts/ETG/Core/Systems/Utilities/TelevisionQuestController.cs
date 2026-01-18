// Decompiled with JetBrains decompiler
// Type: TelevisionQuestController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using UnityEngine;

#nullable disable

  public static class TelevisionQuestController
  {
    public static void RemoveMaintenanceRoomBackpack()
    {
      bool flag = false;
      if (GameStatsManager.Instance.GetFlag(GungeonFlags.SHERPA_READY_FOR_UNLOCKS))
      {
        switch (GameManager.Instance.Dungeon.tileIndices.tilesetId)
        {
          case GlobalDungeonData.ValidTilesets.GUNGEON:
            if (!GameStatsManager.Instance.GetFlag(GungeonFlags.SHERPA_UNLOCK1_COMPLETE))
            {
              flag = true;
              break;
            }
            break;
          case GlobalDungeonData.ValidTilesets.MINEGEON:
            if (!GameStatsManager.Instance.GetFlag(GungeonFlags.SHERPA_UNLOCK2_COMPLETE) && GameStatsManager.Instance.GetFlag(GungeonFlags.SHERPA_UNLOCK1_COMPLETE))
            {
              flag = true;
              break;
            }
            break;
          case GlobalDungeonData.ValidTilesets.CATACOMBGEON:
            if (!GameStatsManager.Instance.GetFlag(GungeonFlags.SHERPA_UNLOCK3_COMPLETE) && GameStatsManager.Instance.GetFlag(GungeonFlags.SHERPA_UNLOCK2_COMPLETE))
            {
              flag = true;
              break;
            }
            break;
          case GlobalDungeonData.ValidTilesets.FORGEGEON:
            if (!GameStatsManager.Instance.GetFlag(GungeonFlags.SHERPA_UNLOCK4_COMPLETE) && GameStatsManager.Instance.GetFlag(GungeonFlags.SHERPA_UNLOCK3_COMPLETE))
            {
              flag = true;
              break;
            }
            break;
        }
      }
      if (flag)
        return;
      GameObject gameObject = GameObject.Find("MaintenanceRoom(Clone)");
      if (!((Object) gameObject != (Object) null))
        return;
      Transform transform = gameObject.transform.Find("Pack");
      if (!((Object) transform != (Object) null))
        return;
      transform.gameObject.SetActive(false);
    }

    public static void HandlePuzzleSetup()
    {
      if (GameManager.Instance.Dungeon.tileIndices.tilesetId != GlobalDungeonData.ValidTilesets.GUNGEON || GameManager.Instance.CurrentLevelOverrideState != GameManager.LevelOverrideState.NONE || !GameStatsManager.Instance.GetFlag(GungeonFlags.SHERPA_UNLOCK1_COMPLETE))
        return;
      RoomHandler targetRoom = (RoomHandler) null;
      for (int index = 0; index < GameManager.Instance.Dungeon.data.rooms.Count; ++index)
      {
        string roomName = GameManager.Instance.Dungeon.data.rooms[index].GetRoomName();
        if (roomName != null && roomName.Contains("Maintenance"))
          targetRoom = GameManager.Instance.Dungeon.data.rooms[index];
      }
      if (targetRoom == null)
        return;
      bool success = false;
      IntVector2 visibleClearSpot = targetRoom.GetCenteredVisibleClearSpot(2, 2, out success, true);
      if (!success)
        return;
      DungeonPlaceableUtility.InstantiateDungeonPlaceable(BraveResources.Load("Global Prefabs/Global Items/BustedTelevisionPlaceable") as GameObject, targetRoom, visibleClearSpot - targetRoom.area.basePosition, false);
    }
  }

