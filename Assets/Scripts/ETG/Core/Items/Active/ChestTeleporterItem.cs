// Decompiled with JetBrains decompiler
// Type: ChestTeleporterItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Items.Active
{
    public class ChestTeleporterItem : PlayerItem
    {
      public GameObject TeleportVFX;
      public float ChanceToBossFoyerAndUpgrade = 0.5f;
      private List<CachedChestData> m_chestos = new List<CachedChestData>();
      private bool m_isSpawning;

      public override void Pickup(PlayerController player)
      {
        base.Pickup(player);
        player.OnNewFloorLoaded += new Action<PlayerController>(this.HandleNewFloorLoaded);
      }

      protected override void OnPreDrop(PlayerController user)
      {
        user.OnNewFloorLoaded -= new Action<PlayerController>(this.HandleNewFloorLoaded);
        base.OnPreDrop(user);
      }

      protected override void OnDestroy()
      {
        if ((bool) (UnityEngine.Object) this.LastOwner)
          this.LastOwner.OnNewFloorLoaded -= new Action<PlayerController>(this.HandleNewFloorLoaded);
        base.OnDestroy();
      }

      private void HandleNewFloorLoaded(PlayerController obj)
      {
        if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.CHARACTER_PAST)
          return;
        this.StartCoroutine(this.LaunchChestSpawns());
      }

      public static RoomHandler FindBossFoyer()
      {
        RoomHandler roomHandler = (RoomHandler) null;
        foreach (RoomHandler room in GameManager.Instance.Dungeon.data.rooms)
        {
          if (room.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.BOSS && room.area.PrototypeRoomBossSubcategory == PrototypeDungeonRoom.RoomBossSubCategory.FLOOR_BOSS)
          {
            roomHandler = room;
            break;
          }
        }
        for (int index = 0; index < roomHandler.connectedRooms.Count; ++index)
        {
          if (roomHandler.connectedRooms[index].distanceFromEntrance <= roomHandler.distanceFromEntrance)
            return roomHandler.connectedRooms[index];
        }
        return (RoomHandler) null;
      }

      [DebuggerHidden]
      private IEnumerator LaunchChestSpawns()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ChestTeleporterItem__LaunchChestSpawnsc__Iterator0()
        {
          _this = this
        };
      }

      public override bool CanBeUsed(PlayerController user)
      {
        if (!(bool) (UnityEngine.Object) user || user.CurrentRoom == null)
          return false;
        IPlayerInteractable nearestInteractable = user.CurrentRoom.GetNearestInteractable(user.CenterPosition, 1f, user);
        if (!(nearestInteractable is Chest))
          return false;
        Chest chest = nearestInteractable as Chest;
        return (bool) (UnityEngine.Object) chest && !chest.IsOpen && chest.GetAbsoluteParentRoom() == user.CurrentRoom && chest.ChestIdentifier != Chest.SpecialChestIdentifier.RAT && base.CanBeUsed(user);
      }

      protected override void DoEffect(PlayerController user)
      {
        if (!(bool) (UnityEngine.Object) user || user.CurrentRoom == null)
          return;
        IPlayerInteractable nearestInteractable = user.CurrentRoom.GetNearestInteractable(user.CenterPosition, 1f, user);
        int num = (int) AkSoundEngine.PostEvent("Play_OBJ_chestwarp_use_01", this.gameObject);
        if (!(nearestInteractable is Chest))
          return;
        Chest chest = nearestInteractable as Chest;
        if (!(bool) (UnityEngine.Object) chest || chest.IsOpen || chest.GetAbsoluteParentRoom() != user.CurrentRoom)
          return;
        CachedChestData cachedChestData = new CachedChestData(chest);
        SpawnManager.SpawnVFX(this.TeleportVFX, (Vector3) chest.sprite.WorldCenter, Quaternion.identity, true);
        user.CurrentRoom.DeregisterInteractable((IPlayerInteractable) chest);
        chest.DeregisterChestOnMinimap();
        if ((bool) (UnityEngine.Object) chest.majorBreakable)
          chest.majorBreakable.TemporarilyInvulnerable = true;
        UnityEngine.Object.Destroy((UnityEngine.Object) chest.gameObject, 0.8f);
        this.m_chestos.Add(cachedChestData);
      }

      public override void MidGameSerialize(List<object> data)
      {
        base.MidGameSerialize(data);
        data.Add((object) this.m_chestos.Count);
        for (int index = 0; index < this.m_chestos.Count; ++index)
          data.Add((object) this.m_chestos[index].Serialize());
      }

      public override void MidGameDeserialize(List<object> data)
      {
        base.MidGameDeserialize(data);
        int num = (int) data[0];
        this.m_chestos.Clear();
        for (int index = 1; index < num + 1; ++index)
          this.m_chestos.Add(new CachedChestData((string) data[index]));
        if (this.m_chestos.Count <= 0)
          return;
        this.StartCoroutine(this.LaunchChestSpawns());
      }
    }

}
