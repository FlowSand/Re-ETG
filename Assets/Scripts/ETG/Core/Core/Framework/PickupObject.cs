// Decompiled with JetBrains decompiler
// Type: PickupObject
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Serialization;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public abstract class PickupObject : BraveBehaviour
    {
      public static bool RatBeatenAtPunchout;
      [DisableInInspector]
      public int PickupObjectId = -1;
      public PickupObject.ItemQuality quality;
      public float additionalMagnificenceModifier;
      public bool ItemSpansBaseQualityTiers;
      [HideInInspectorIf("ItemSpansBaseQualityTiers", false)]
      public bool ItemRespectsHeartMagnificence;
      public LootModData[] associatedItemChanceMods;
      public ContentSource contentSource;
      public bool ShouldBeExcludedFromShops;
      public bool CanBeDropped = true;
      public bool PreventStartingOwnerFromDropping;
      public bool PersistsOnDeath;
      public bool RespawnsIfPitfall;
      public bool PreventSaveSerialization;
      public bool IgnoredByRat;
      [NonSerialized]
      public bool ClearIgnoredByRatFlagOnPickup;
      [NonSerialized]
      public bool IsBeingSold;
      [LongEnum]
      public GungeonFlags SaveFlagToSetOnAcquisition;
      [SerializeField]
      protected string itemName;
      protected static int s_lastRainbowPickupFrame = -1;
      [NonSerialized]
      public bool HasBeenStatProcessed;
      [HideInInspector]
      public int ForcedPositionInAmmonomicon = -1;
      public bool UsesCustomCost;
      [FormerlySerializedAs("costInStore")]
      public int CustomCost;
      public bool PersistsOnPurchase;
      public bool CanBeSold = true;
      [NonSerialized]
      public bool HasProcessedStatMods;
      protected Color m_alienPickupColor = new Color(1f, 1f, 0.0f, 1f);
      public static bool ItemIsBeingTakenByRat;
      protected bool m_isBeingEyedByRat;
      protected int m_numberTimesRatTheftAttempted;

      public bool CanActuallyBeDropped(PlayerController owner)
      {
        if (!this.CanBeDropped || this is Gun && (UnityEngine.Object) owner.CurrentGun == (UnityEngine.Object) this && owner.inventory.GunLocked.Value)
          return false;
        if ((bool) (UnityEngine.Object) owner)
        {
          for (int index = 0; index < owner.startingGunIds.Count; ++index)
          {
            if (owner.startingGunIds[index] == this.PickupObjectId)
              return !this.PreventStartingOwnerFromDropping;
          }
          for (int index = 0; index < owner.startingAlternateGunIds.Count; ++index)
          {
            if (owner.startingAlternateGunIds[index] == this.PickupObjectId)
              return !this.PreventStartingOwnerFromDropping;
          }
          for (int index = 0; index < owner.startingPassiveItemIds.Count; ++index)
          {
            if (owner.startingPassiveItemIds[index] == this.PickupObjectId)
              return !this.PreventStartingOwnerFromDropping;
          }
          for (int index = 0; index < owner.startingActiveItemIds.Count; ++index)
          {
            if (owner.startingActiveItemIds[index] == this.PickupObjectId)
              return !this.PreventStartingOwnerFromDropping;
          }
        }
        return true;
      }

      public virtual string DisplayName => this.itemName;

      public string EncounterNameOrDisplayName
      {
        get
        {
          EncounterTrackable component = this.GetComponent<EncounterTrackable>();
          return (bool) (UnityEngine.Object) component ? component.GetModifiedDisplayName() : this.itemName;
        }
      }

      public int PurchasePrice
      {
        get => this.UsesCustomCost ? this.CustomCost : GlobalDungeonData.GetBasePrice(this.quality);
      }

      public bool PrerequisitesMet()
      {
        if (this.quality == PickupObject.ItemQuality.EXCLUDED)
          return false;
        EncounterTrackable component = this.GetComponent<EncounterTrackable>();
        return (UnityEngine.Object) component == (UnityEngine.Object) null || component.PrerequisitesMet();
      }

      public virtual bool ShouldBeDestroyedOnExistence(bool isForEnemyInventory = false) => false;

      protected void HandleEncounterable(PlayerController player)
      {
        EncounterTrackable component = this.GetComponent<EncounterTrackable>();
        if ((UnityEngine.Object) component != (UnityEngine.Object) null)
        {
          component.HandleEncounter();
          if ((bool) (UnityEngine.Object) this && this.PickupObjectId == GlobalItemIds.FinishedGun)
            GameStatsManager.Instance.SingleIncrementDifferentiator(PickupObjectDatabase.GetById(GlobalItemIds.UnfinishedGun).encounterTrackable);
        }
        if (GameManager.Instance.CurrentLevelOverrideState != GameManager.LevelOverrideState.FOYER)
          this.HandleMagnficence();
        if (this.SaveFlagToSetOnAcquisition == GungeonFlags.NONE)
          return;
        GameStatsManager.Instance.SetFlag(this.SaveFlagToSetOnAcquisition, true);
      }

      protected void HandleMagnficence()
      {
        GameManager.Instance.PrimaryPlayer.stats.AddFloorMagnificence(this.additionalMagnificenceModifier);
        if (this.ItemRespectsHeartMagnificence)
          return;
        switch (this.quality)
        {
          case PickupObject.ItemQuality.COMMON:
            GameManager.Instance.PrimaryPlayer.stats.AddFloorMagnificence(0.0f);
            break;
          case PickupObject.ItemQuality.A:
            GameManager.Instance.PrimaryPlayer.stats.AddFloorMagnificence(1f);
            break;
          case PickupObject.ItemQuality.S:
            GameManager.Instance.PrimaryPlayer.stats.AddFloorMagnificence(1f);
            break;
        }
      }

      protected void HandleLootMods(PlayerController player)
      {
        if (this.associatedItemChanceMods == null)
          return;
        for (int index = 0; index < this.associatedItemChanceMods.Length; ++index)
          player.lootModData.Add(this.associatedItemChanceMods[index]);
      }

      public abstract void Pickup(PlayerController player);

      protected override void OnDestroy() => base.OnDestroy();

      protected bool ShouldBeTakenByRat(Vector2 point)
      {
        if (GameManager.Instance.IsLoadingLevel || Dungeon.IsGenerating || !this.gameObject.activeSelf || this.IgnoredByRat || this is NotePassiveItem || this is AmmoPickup && this.transform.position.GetAbsoluteRoom().IsSecretRoom || GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.RATGEON || PickupObject.RatBeatenAtPunchout && !PassiveItem.IsFlagSetAtAll(typeof (RingOfResourcefulRatItem)) || this.transform.position == Vector3.zero || PickupObject.ItemIsBeingTakenByRat)
          return false;
        if (GameManager.Instance.AllPlayers != null)
        {
          for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
          {
            if (GameManager.Instance.AllPlayers[index].PlayerIsRatTransformed || (double) Vector2.Distance(point, GameManager.Instance.AllPlayers[index].CenterPosition) < 10.0)
              return false;
          }
        }
        if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.FOYER || GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.CHARACTER_PAST || GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.END_TIMES || GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.TUTORIAL || ((UnityEngine.Object) GameManager.Instance.PrimaryPlayer == (UnityEngine.Object) null || GameManager.Instance.PrimaryPlayer.healthHaver.IsDead) && ((UnityEngine.Object) GameManager.Instance.SecondaryPlayer == (UnityEngine.Object) null || GameManager.Instance.SecondaryPlayer.healthHaver.IsDead) || (bool) (UnityEngine.Object) this.encounterTrackable && this.encounterTrackable.UsesPurpleNotifications || this is SilencerItem)
          return false;
        if (this is RobotUnlockTelevisionItem || this.m_numberTimesRatTheftAttempted == 0)
          return GameManager.Instance.GetPlayerClosestToPoint(point).CurrentRoom != GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(this.transform.position.IntXY());
        RoomHandler currentRoom = GameManager.Instance.GetPlayerClosestToPoint(point).CurrentRoom;
        RoomHandler roomFromPosition = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(this.transform.position.IntXY());
        return currentRoom != roomFromPosition && !currentRoom.connectedRooms.Contains(roomFromPosition);
      }

      public bool IsBeingEyedByRat => this.m_isBeingEyedByRat;

      [DebuggerHidden]
      protected IEnumerator HandleRatTheft()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new PickupObject__HandleRatTheftc__Iterator0()
        {
          _this = this
        };
      }

      public static void HandlePickupCurseParticles(tk2dBaseSprite targetSprite, float zOffset = 0.0f)
      {
        if (!(bool) (UnityEngine.Object) targetSprite)
          return;
        Vector3 vector3ZisY1 = targetSprite.WorldBottomLeft.ToVector3ZisY(zOffset);
        Vector3 vector3ZisY2 = targetSprite.WorldTopRight.ToVector3ZisY(zOffset);
        GlobalSparksDoer.DoRandomParticleBurst(Mathf.CeilToInt(Mathf.Max(1f, 25f * (float) (((double) vector3ZisY2.y - (double) vector3ZisY1.y) * ((double) vector3ZisY2.x - (double) vector3ZisY1.x)) * BraveTime.DeltaTime)), vector3ZisY1, vector3ZisY2, Vector3.up / 2f, 120f, 0.2f, startLifetime: new float?(UnityEngine.Random.Range(0.8f, 1.25f)), systemType: GlobalSparksDoer.SparksType.BLACK_PHANTOM_SMOKE);
      }

      protected void HandlePickupCurseParticles()
      {
        if (!(bool) (UnityEngine.Object) this || !(bool) (UnityEngine.Object) this.sprite)
          return;
        bool flag = false;
        switch (this)
        {
          case Gun _:
            Gun gun = this as Gun;
            for (int index = 0; index < gun.passiveStatModifiers.Length; ++index)
            {
              if (gun.passiveStatModifiers[index].statToBoost == PlayerStats.StatType.Curse && (double) gun.passiveStatModifiers[index].amount > 0.0)
              {
                flag = true;
                break;
              }
            }
            break;
          case PlayerItem _:
            PlayerItem playerItem = this as PlayerItem;
            for (int index = 0; index < playerItem.passiveStatModifiers.Length; ++index)
            {
              if (playerItem.passiveStatModifiers[index].statToBoost == PlayerStats.StatType.Curse && (double) playerItem.passiveStatModifiers[index].amount > 0.0)
              {
                flag = true;
                break;
              }
            }
            break;
          case PassiveItem _:
            PassiveItem passiveItem = this as PassiveItem;
            for (int index = 0; index < passiveItem.passiveStatModifiers.Length; ++index)
            {
              if (passiveItem.passiveStatModifiers[index].statToBoost == PlayerStats.StatType.Curse && (double) passiveItem.passiveStatModifiers[index].amount > 0.0)
              {
                flag = true;
                break;
              }
            }
            break;
        }
        if (!flag)
          return;
        PickupObject.HandlePickupCurseParticles(this.sprite);
      }

      protected void OnSharedPickup()
      {
        if (!this.IgnoredByRat || !this.ClearIgnoredByRatFlagOnPickup)
          return;
        this.IgnoredByRat = false;
      }

      public virtual void MidGameSerialize(List<object> data)
      {
      }

      public virtual void MidGameDeserialize(List<object> data)
      {
      }

      public enum ItemQuality
      {
        EXCLUDED = -100, // 0xFFFFFF9C
        SPECIAL = -50, // 0xFFFFFFCE
        COMMON = 0,
        D = 1,
        C = 2,
        B = 3,
        A = 4,
        S = 5,
      }
    }

}
