// Decompiled with JetBrains decompiler
// Type: PaydayDrillItem
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
    public class PaydayDrillItem : PlayerItem, IPaydayItem
    {
      public GameObject DrillVFXPrefab;
      public VFXPool VFXDustPoof;
      public VFXPool DisappearDrillPoof;
      public PrototypeDungeonRoom GenericFallbackCombatRoom;
      [Header("Timing")]
      public float DelayPreExpansion = 2.5f;
      public float DelayPostExpansionPreEnemies = 2f;
      [Header("Waves")]
      public DrillWaveDefinition[] D_Quality_Waves;
      public DrillWaveDefinition[] C_Quality_Waves;
      public DrillWaveDefinition[] B_Quality_Waves;
      public DrillWaveDefinition[] A_Quality_Waves;
      public DrillWaveDefinition[] S_Quality_Waves;
      private bool m_inEffect;
      [NonSerialized]
      public bool HasSetOrder;
      [NonSerialized]
      public string ID01;
      [NonSerialized]
      public string ID02;
      [NonSerialized]
      public string ID03;
      private Vector3 m_baseChestOffset = new Vector3(0.5f, 0.25f, 0.0f);
      private Vector3 m_largeChestOffset = new Vector3(7f / 16f, 1f / 16f, 0.0f);
      private string[] c_rewardRoomObjects = new string[2]
      {
        "Gungeon_Treasure_Dais(Clone)",
        "GodRay_Placeable(Clone)"
      };

      public void StoreData(string id1, string id2, string id3)
      {
        this.ID01 = id1;
        this.ID02 = id2;
        this.ID03 = id3;
        this.HasSetOrder = true;
      }

      public bool HasCachedData() => this.HasSetOrder;

      public string GetID(int placement)
      {
        if (placement == 0)
          return this.ID01;
        return placement == 1 ? this.ID02 : this.ID03;
      }

      public override void MidGameSerialize(List<object> data)
      {
        base.MidGameSerialize(data);
        data.Add((object) this.HasSetOrder);
        data.Add((object) this.ID01);
        data.Add((object) this.ID02);
        data.Add((object) this.ID03);
      }

      public override void MidGameDeserialize(List<object> data)
      {
        base.MidGameDeserialize(data);
        if (data.Count != 4)
          return;
        this.HasSetOrder = (bool) data[0];
        this.ID01 = (string) data[1];
        this.ID02 = (string) data[2];
        this.ID03 = (string) data[3];
      }

      public override bool CanBeUsed(PlayerController user)
      {
        if (!(bool) (UnityEngine.Object) user || user.CurrentRoom == null || user.CurrentRoom.CompletelyPreventLeaving || user.CurrentRoom.area.PrototypeLostWoodsRoom)
          return false;
        IPlayerInteractable nearestInteractable = user.CurrentRoom.GetNearestInteractable(user.CenterPosition, 1f, user);
        switch (nearestInteractable)
        {
          case InteractableLock _:
          case Chest _:
          case DungeonDoorController _:
            switch (nearestInteractable)
            {
              case InteractableLock _:
                InteractableLock interactableLock = nearestInteractable as InteractableLock;
                if ((bool) (UnityEngine.Object) interactableLock && !interactableLock.IsBusted && interactableLock.transform.position.GetAbsoluteRoom() == user.CurrentRoom && interactableLock.IsLocked && !interactableLock.HasBeenPicked && interactableLock.lockMode == InteractableLock.InteractableLockMode.NORMAL)
                  return base.CanBeUsed(user);
                break;
              case DungeonDoorController _:
                DungeonDoorController dungeonDoorController = nearestInteractable as DungeonDoorController;
                if ((UnityEngine.Object) dungeonDoorController != (UnityEngine.Object) null && dungeonDoorController.Mode == DungeonDoorController.DungeonDoorMode.COMPLEX && dungeonDoorController.isLocked && !dungeonDoorController.lockIsBusted)
                  return base.CanBeUsed(user);
                break;
              case Chest _:
                Chest chest = nearestInteractable as Chest;
                return (bool) (UnityEngine.Object) chest && chest.GetAbsoluteParentRoom() == user.CurrentRoom && chest.IsLocked && !chest.IsLockBroken && base.CanBeUsed(user);
            }
            break;
        }
        return false;
      }

      protected override void DoEffect(PlayerController user)
      {
        base.DoEffect(user);
        int num1 = (int) AkSoundEngine.PostEvent("Play_OBJ_paydaydrill_start_01", GameManager.Instance.gameObject);
        int num2 = (int) AkSoundEngine.PostEvent("Play_OBJ_paydaydrill_loop_01", GameManager.Instance.gameObject);
        IPlayerInteractable nearestInteractable = user.CurrentRoom.GetNearestInteractable(user.CenterPosition, 1f, user);
        switch (nearestInteractable)
        {
          case InteractableLock _:
          case Chest _:
          case DungeonDoorController _:
            switch (nearestInteractable)
            {
              case InteractableLock _:
                InteractableLock interactableLock = nearestInteractable as InteractableLock;
                if (interactableLock.lockMode == InteractableLock.InteractableLockMode.NORMAL)
                {
                  interactableLock.ForceUnlock();
                  int num3 = (int) AkSoundEngine.PostEvent("m_OBJ_lock_pick_01", GameManager.Instance.gameObject);
                }
                int num4 = (int) AkSoundEngine.PostEvent("Stop_OBJ_paydaydrill_loop_01", GameManager.Instance.gameObject);
                return;
              case DungeonDoorController _:
                DungeonDoorController dungeonDoorController = nearestInteractable as DungeonDoorController;
                if ((UnityEngine.Object) dungeonDoorController != (UnityEngine.Object) null && dungeonDoorController.Mode == DungeonDoorController.DungeonDoorMode.COMPLEX && dungeonDoorController.isLocked)
                {
                  dungeonDoorController.Unlock();
                  int num5 = (int) AkSoundEngine.PostEvent("m_OBJ_lock_pick_01", GameManager.Instance.gameObject);
                }
                int num6 = (int) AkSoundEngine.PostEvent("Stop_OBJ_paydaydrill_loop_01", GameManager.Instance.gameObject);
                return;
              case Chest _:
                Chest sourceChest = nearestInteractable as Chest;
                if (!sourceChest.IsLocked)
                  return;
                if (sourceChest.IsLockBroken)
                {
                  int num7 = (int) AkSoundEngine.PostEvent("Stop_OBJ_paydaydrill_loop_01", GameManager.Instance.gameObject);
                  return;
                }
                if (sourceChest.IsMimic && (bool) (UnityEngine.Object) sourceChest.majorBreakable)
                {
                  sourceChest.majorBreakable.ApplyDamage(1000f, Vector2.zero, false, ForceDamageOverride: true);
                  int num8 = (int) AkSoundEngine.PostEvent("Stop_OBJ_paydaydrill_loop_01", GameManager.Instance.gameObject);
                  return;
                }
                sourceChest.ForceKillFuse();
                sourceChest.PreventFuse = true;
                RoomHandler absoluteRoom = sourceChest.transform.position.GetAbsoluteRoom();
                this.m_inEffect = true;
                if (absoluteRoom.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.REWARD)
                {
                  GameManager.Instance.Dungeon.StartCoroutine(this.HandleSeamlessTransitionToCombatRoom(absoluteRoom, sourceChest));
                  return;
                }
                GameManager.Instance.Dungeon.StartCoroutine(this.HandleTransitionToFallbackCombatRoom(absoluteRoom, sourceChest));
                return;
              default:
                return;
            }
          default:
            int num9 = (int) AkSoundEngine.PostEvent("Stop_OBJ_paydaydrill_loop_01", GameManager.Instance.gameObject);
            break;
        }
      }

      [DebuggerHidden]
      protected IEnumerator HandleCombatWaves(Dungeon d, RoomHandler newRoom, Chest sourceChest)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new PaydayDrillItem.<HandleCombatWaves>c__Iterator0()
        {
          sourceChest = sourceChest,
          d = d,
          newRoom = newRoom,
          _this = this
        };
      }

      [DebuggerHidden]
      protected IEnumerator HandleTransitionToFallbackCombatRoom(
        RoomHandler sourceRoom,
        Chest sourceChest)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new PaydayDrillItem.<HandleTransitionToFallbackCombatRoom>c__Iterator1()
        {
          sourceChest = sourceChest,
          sourceRoom = sourceRoom,
          _this = this
        };
      }

      [DebuggerHidden]
      protected IEnumerator HandleSeamlessTransitionToCombatRoom(
        RoomHandler sourceRoom,
        Chest sourceChest)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new PaydayDrillItem.<HandleSeamlessTransitionToCombatRoom>c__Iterator2()
        {
          sourceChest = sourceChest,
          sourceRoom = sourceRoom,
          _this = this
        };
      }

      private void MoveObjectBetweenRooms(
        Transform foundObject,
        RoomHandler fromRoom,
        RoomHandler toRoom)
      {
        Vector2 vector2_1 = foundObject.position.XY() - fromRoom.area.basePosition.ToVector2();
        Vector2 vector2_2 = toRoom.area.basePosition.ToVector2() + vector2_1;
        foundObject.transform.position = (Vector3) vector2_2;
        if ((UnityEngine.Object) foundObject.parent == (UnityEngine.Object) fromRoom.hierarchyParent)
          foundObject.parent = toRoom.hierarchyParent;
        SpeculativeRigidbody component1 = foundObject.GetComponent<SpeculativeRigidbody>();
        if ((bool) (UnityEngine.Object) component1)
          component1.Reinitialize();
        tk2dBaseSprite component2 = foundObject.GetComponent<tk2dBaseSprite>();
        if (!(bool) (UnityEngine.Object) component2)
          return;
        component2.UpdateZDepth();
      }

      [DebuggerHidden]
      private IEnumerator HandleCombatRoomShrinking(RoomHandler targetRoom)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new PaydayDrillItem.<HandleCombatRoomShrinking>c__Iterator3()
        {
          targetRoom = targetRoom,
          _this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator HandleCombatRoomExpansion(
        RoomHandler sourceRoom,
        RoomHandler targetRoom,
        Chest sourceChest)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new PaydayDrillItem.<HandleCombatRoomExpansion>c__Iterator4()
        {
          targetRoom = targetRoom,
          sourceChest = sourceChest,
          _this = this
        };
      }

      private void ShrinkRoom(RoomHandler r)
      {
        Dungeon dungeon = GameManager.Instance.Dungeon;
        int num1 = (int) AkSoundEngine.PostEvent("Play_OBJ_stone_crumble_01", GameManager.Instance.gameObject);
        tk2dTileMap targetTilemap = (tk2dTileMap) null;
        HashSet<IntVector2> intVector2Set = new HashSet<IntVector2>();
        for (int x = -5; x < r.area.dimensions.x + 5; ++x)
        {
          for (int y = -5; y < r.area.dimensions.y + 5; ++y)
          {
            IntVector2 intVector2 = r.area.basePosition + new IntVector2(x, y);
            CellData cellData = !dungeon.data.CheckInBoundsAndValid(intVector2) ? (CellData) null : dungeon.data[intVector2];
            if (cellData != null && cellData.type != CellType.WALL && cellData.HasTypeNeighbor(dungeon.data, CellType.WALL))
              intVector2Set.Add(cellData.position);
          }
        }
        foreach (IntVector2 key in intVector2Set)
        {
          CellData cellData = dungeon.data[key];
          cellData.breakable = true;
          cellData.occlusionData.overrideOcclusion = true;
          cellData.occlusionData.cellOcclusionDirty = true;
          targetTilemap = dungeon.ConstructWallAtPosition(key.x, key.y);
          r.Cells.Remove(cellData.position);
          r.CellsWithoutExits.Remove(cellData.position);
          r.RawCells.Remove(cellData.position);
        }
        Pixelator.Instance.MarkOcclusionDirty();
        double num2 = (double) Pixelator.Instance.ProcessOcclusionChange(r.Epicenter, 1f, r, false);
        if (!(bool) (UnityEngine.Object) targetTilemap)
          return;
        dungeon.RebuildTilemap(targetTilemap);
      }

      private void ExpandRoom(RoomHandler r)
      {
        Dungeon dungeon = GameManager.Instance.Dungeon;
        int num1 = (int) AkSoundEngine.PostEvent("Play_OBJ_stone_crumble_01", GameManager.Instance.gameObject);
        tk2dTileMap targetTilemap = (tk2dTileMap) null;
        HashSet<IntVector2> intVector2Set = new HashSet<IntVector2>();
        for (int x = -5; x < r.area.dimensions.x + 5; ++x)
        {
          for (int y = -5; y < r.area.dimensions.y + 5; ++y)
          {
            IntVector2 intVector2 = r.area.basePosition + new IntVector2(x, y);
            CellData cellData = !dungeon.data.CheckInBoundsAndValid(intVector2) ? (CellData) null : dungeon.data[intVector2];
            if (cellData != null && cellData.type == CellType.WALL && cellData.HasTypeNeighbor(dungeon.data, CellType.FLOOR))
              intVector2Set.Add(cellData.position);
          }
        }
        foreach (IntVector2 key in intVector2Set)
        {
          CellData cellData = dungeon.data[key];
          cellData.breakable = true;
          cellData.occlusionData.overrideOcclusion = true;
          cellData.occlusionData.cellOcclusionDirty = true;
          targetTilemap = dungeon.DestroyWallAtPosition(key.x, key.y);
          if ((double) UnityEngine.Random.value < 0.25)
            this.VFXDustPoof.SpawnAtPosition(key.ToCenterVector3((float) key.y));
          r.Cells.Add(cellData.position);
          r.CellsWithoutExits.Add(cellData.position);
          r.RawCells.Add(cellData.position);
        }
        Pixelator.Instance.MarkOcclusionDirty();
        double num2 = (double) Pixelator.Instance.ProcessOcclusionChange(r.Epicenter, 1f, r, false);
        if (!(bool) (UnityEngine.Object) targetTilemap)
          return;
        dungeon.RebuildTilemap(targetTilemap);
      }
    }

}
