// Decompiled with JetBrains decompiler
// Type: MidGameSaveData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullSerializer;
using InControl;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Data
{
    public class MidGameSaveData
    {
      [fsProperty]
      public GlobalDungeonData.ValidTilesets levelSaved = GlobalDungeonData.ValidTilesets.CASTLEGEON;
      [fsProperty]
      public GameManager.GameType savedGameType;
      [fsProperty]
      public GameManager.GameMode savedGameMode;
      [fsProperty]
      public int LastShortcutFloorLoaded;
      [fsProperty]
      public MidGamePlayerData playerOneData;
      [fsProperty]
      public MidGamePlayerData playerTwoData;
      [fsProperty]
      public GameStats PriorSessionStats;
      [fsProperty]
      public MidGameStaticShopData StaticShopData;
      [fsProperty]
      public RunData RunData;
      [fsProperty]
      public string midGameSaveGuid;
      [fsProperty]
      public bool invalidated;
      public static InputDevice ContinuePressedDevice;
      public static bool IsInitializingPlayerData;

      public MidGameSaveData()
      {
      }

      public MidGameSaveData(
        PlayerController p1,
        PlayerController p2,
        GlobalDungeonData.ValidTilesets targetLevel,
        string midGameSaveGuid)
      {
        this.midGameSaveGuid = midGameSaveGuid;
        this.levelSaved = targetLevel;
        this.savedGameMode = GameManager.Instance.CurrentGameMode;
        if (this.savedGameMode == GameManager.GameMode.SHORTCUT)
          this.LastShortcutFloorLoaded = GameManager.Instance.LastShortcutFloorLoaded;
        this.savedGameType = !((Object) p2 != (Object) null) ? GameManager.GameType.SINGLE_PLAYER : GameManager.GameType.COOP_2_PLAYER;
        this.playerOneData = new MidGamePlayerData(p1);
        if (this.savedGameType == GameManager.GameType.COOP_2_PLAYER)
          this.playerTwoData = new MidGamePlayerData(p2);
        this.PriorSessionStats = GameStatsManager.Instance.MoveSessionStatsToSavedSessionStats();
        this.StaticShopData = BaseShopController.GetStaticShopDataForMidGameSave();
        this.RunData = GameManager.Instance.RunData;
      }

      public bool IsValid() => !this.invalidated;

      public void Invalidate() => this.invalidated = true;

      public void Revalidate() => this.invalidated = false;

      public GameObject GetPlayerOnePrefab()
      {
        string path = CharacterSelectController.GetCharacterPathFromIdentity(this.playerOneData.CharacterIdentity);
        if (this.levelSaved == GlobalDungeonData.ValidTilesets.FINALGEON && this.playerOneData.CharacterIdentity == PlayableCharacters.Pilot)
          path = "PlayerRogueShip";
        return (GameObject) BraveResources.Load(path);
      }

      public void LoadPreGenDataFromMidGameSave() => GameManager.Instance.RunData = this.RunData;

      public void LoadDataFromMidGameSave(PlayerController p1, PlayerController p2)
      {
        if (this.StaticShopData != null)
          BaseShopController.LoadFromMidGameSave(this.StaticShopData);
        GameManager.Instance.CurrentGameMode = this.savedGameMode;
        GameManager.Instance.LastShortcutFloorLoaded = this.LastShortcutFloorLoaded;
        GameStatsManager.Instance.AssignMidGameSavedSessionStats(this.PriorSessionStats);
        if ((bool) (Object) p1)
          PassiveItem.DecrementFlag(p1, typeof (SevenLeafCloverItem));
        if ((bool) (Object) p2)
          PassiveItem.DecrementFlag(p2, typeof (SevenLeafCloverItem));
        this.InitializePlayerData(p1, this.playerOneData, true);
        if (this.savedGameType == GameManager.GameType.COOP_2_PLAYER && (bool) (Object) p2)
        {
          this.InitializePlayerData(p2, this.playerTwoData, false);
          BraveInput.ReassignAllControllers(MidGameSaveData.ContinuePressedDevice);
        }
        MidGameSaveData.ContinuePressedDevice = (InputDevice) null;
      }

      public void InitializePlayerData(
        PlayerController p1,
        MidGamePlayerData playerData,
        bool isPlayerOne)
      {
        MidGameSaveData.IsInitializingPlayerData = true;
        p1.MasteryTokensCollectedThisRun = playerData.MasteryTokensCollected;
        p1.CharacterUsesRandomGuns = playerData.CharacterUsesRandomGuns;
        p1.HasTakenDamageThisRun = playerData.HasTakenDamageThisRun;
        p1.HasFiredNonStartingGun = playerData.HasFiredNonStartingGun;
        ParadoxPortalController component = ((GameObject) ResourceCache.Acquire("Global Prefabs/VFX_ParadoxPortal")).GetComponent<ParadoxPortalController>();
        p1.portalEeveeTex = component.CosmicTex;
        p1.IsTemporaryEeveeForUnlock = playerData.IsTemporaryEeveeForUnlock;
        ChallengeManager.ChallengeModeType = playerData.ChallengeMode;
        if (this.levelSaved == GlobalDungeonData.ValidTilesets.FINALGEON)
          p1.CharacterUsesRandomGuns = false;
        if (this.levelSaved != GlobalDungeonData.ValidTilesets.FINALGEON || !(p1 is PlayerSpaceshipController))
        {
          p1.inventory.DestroyAllGuns();
          p1.RemoveAllPassiveItems();
          p1.RemoveAllActiveItems();
          if (playerData.passiveItems != null)
          {
            for (int index = 0; index < playerData.passiveItems.Count; ++index)
            {
              EncounterTrackable.SuppressNextNotification = true;
              LootEngine.GivePrefabToPlayer(PickupObjectDatabase.GetById(playerData.passiveItems[index].PickupID).gameObject, p1);
            }
          }
          if (playerData.activeItems != null)
          {
            for (int index = 0; index < playerData.activeItems.Count; ++index)
            {
              EncounterTrackable.SuppressNextNotification = true;
              LootEngine.GivePrefabToPlayer(PickupObjectDatabase.GetById(playerData.activeItems[index].PickupID).gameObject, p1);
            }
          }
          if (playerData.guns != null)
          {
            for (int index = 0; index < playerData.guns.Count; ++index)
            {
              EncounterTrackable.SuppressNextNotification = true;
              LootEngine.GivePrefabToPlayer(PickupObjectDatabase.GetById(playerData.guns[index].PickupID).gameObject, p1);
            }
            for (int index1 = 0; index1 < playerData.guns.Count; ++index1)
            {
              for (int index2 = 0; index2 < p1.inventory.AllGuns.Count; ++index2)
              {
                if (p1.inventory.AllGuns[index2].PickupObjectId == playerData.guns[index1].PickupID)
                {
                  p1.inventory.AllGuns[index2].MidGameDeserialize(playerData.guns[index1].SerializedData);
                  for (int index3 = 0; index3 < playerData.guns[index1].DuctTapedGunIDs.Count; ++index3)
                  {
                    Gun byId = PickupObjectDatabase.GetById(playerData.guns[index1].DuctTapedGunIDs[index3]) as Gun;
                    if ((bool) (Object) byId)
                      DuctTapeItem.DuctTapeGuns(byId, p1.inventory.AllGuns[index2]);
                  }
                  p1.inventory.AllGuns[index2].ammo = playerData.guns[index1].CurrentAmmo;
                  TransformGunSynergyProcessor[] componentsInChildren = p1.inventory.AllGuns[index2].GetComponentsInChildren<TransformGunSynergyProcessor>();
                  for (int index4 = 0; index4 < componentsInChildren.Length; ++index4)
                  {
                    componentsInChildren[index4].ShouldResetAmmoAfterTransformation = true;
                    componentsInChildren[index4].ResetAmmoCount = playerData.guns[index1].CurrentAmmo;
                  }
                }
              }
            }
          }
          if ((double) playerData.CurrentHealth <= 0.0 && (double) playerData.CurrentArmor <= 0.0)
          {
            p1.healthHaver.Armor = 0.0f;
            p1.DieOnMidgameLoad();
          }
          else
          {
            p1.healthHaver.ForceSetCurrentHealth(playerData.CurrentHealth);
            p1.healthHaver.Armor = playerData.CurrentArmor;
          }
          if (isPlayerOne)
          {
            p1.carriedConsumables.KeyBullets = playerData.CurrentKeys;
            p1.carriedConsumables.Currency = playerData.CurrentCurrency;
          }
          p1.Blanks = Mathf.Max(p1.Blanks, playerData.CurrentBlanks);
          if (playerData.activeItems != null)
          {
            for (int index5 = 0; index5 < playerData.activeItems.Count; ++index5)
            {
              for (int index6 = 0; index6 < p1.activeItems.Count; ++index6)
              {
                if (playerData.activeItems[index5].PickupID == p1.activeItems[index6].PickupObjectId)
                {
                  p1.activeItems[index6].MidGameDeserialize(playerData.activeItems[index5].SerializedData);
                  p1.activeItems[index6].CurrentDamageCooldown = playerData.activeItems[index5].DamageCooldown;
                  p1.activeItems[index6].CurrentRoomCooldown = playerData.activeItems[index5].RoomCooldown;
                  p1.activeItems[index6].CurrentTimeCooldown = playerData.activeItems[index5].TimeCooldown;
                  if (p1.activeItems[index6].consumable && playerData.activeItems[index5].NumberOfUses > 0)
                    p1.activeItems[index6].numberOfUses = playerData.activeItems[index5].NumberOfUses;
                }
              }
            }
          }
          if (playerData.passiveItems != null)
          {
            for (int index7 = 0; index7 < playerData.passiveItems.Count; ++index7)
            {
              for (int index8 = 0; index8 < p1.passiveItems.Count; ++index8)
              {
                if (playerData.passiveItems[index7].PickupID == p1.passiveItems[index8].PickupObjectId)
                  p1.passiveItems[index8].MidGameDeserialize(playerData.passiveItems[index7].SerializedData);
              }
            }
          }
          if (playerData.ownerlessStatModifiers != null)
          {
            if (p1.ownerlessStatModifiers == null)
              p1.ownerlessStatModifiers = new List<StatModifier>();
            for (int index = 0; index < playerData.ownerlessStatModifiers.Count; ++index)
              p1.ownerlessStatModifiers.Add(playerData.ownerlessStatModifiers[index]);
          }
          if (this.levelSaved == GlobalDungeonData.ValidTilesets.FINALGEON && p1.characterIdentity != PlayableCharacters.Gunslinger)
            p1.ResetToFactorySettings(true, true);
          if ((bool) (Object) p1 && (Object) p1.stats != (Object) null)
            p1.stats.RecalculateStats(p1);
          if (playerData.HasBloodthirst)
            p1.gameObject.GetOrAddComponent<Bloodthirst>();
        }
        MidGameSaveData.IsInitializingPlayerData = false;
        EncounterTrackable.SuppressNextNotification = false;
      }
    }

}
