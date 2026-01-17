// Decompiled with JetBrains decompiler
// Type: RewardManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class RewardManager : ScriptableObject
{
  [NonSerialized]
  public static float AdditionalHeartTierMagnificence;
  [SerializeField]
  public List<global::FloorRewardData> FloorRewardData;
  [Header("Chest Definitions")]
  public Chest D_Chest;
  public Chest C_Chest;
  public Chest B_Chest;
  public Chest A_Chest;
  public Chest S_Chest;
  public Chest Rainbow_Chest;
  public Chest Synergy_Chest;
  [Header("Loot Table Definitions")]
  public GenericLootTable GunsLootTable;
  public GenericLootTable ItemsLootTable;
  [Header("Global Currency Settings")]
  public float BossGoldCoinChance = 0.0003f;
  public float PowerfulGoldCoinChance = 0.000125f;
  public float NormalGoldCoinChance = 5E-05f;
  [Space(5f)]
  public int RobotMinCurrencyPerHealthItem = 5;
  public int RobotMaxCurrencyPerHealthItem = 10;
  [Header("Synergy Settings")]
  public float GlobalSynerchestChance = 0.02f;
  public float SynergyCompletionMultiplier = 1f;
  public bool SynergyCompletionIgnoresQualities;
  [Header("Additional Settings")]
  public float EarlyChestChanceIfNotChump = 0.2f;
  public float RoomClearRainbowChance = 0.0001f;
  [PickupIdentifier]
  public int FullHeartIdPrefab = -1;
  [PickupIdentifier]
  public int HalfHeartIdPrefab = -1;
  public float SinglePlayerPickupIncrementModifier = 1.25f;
  public float CoopPickupIncrementModifier = 1.5f;
  public float CoopAmmoChanceModifier = 1.5f;
  public float GunMimicMimicGunChance = 1f / 1000f;
  [Header("Bonus Enemy Spawn Settings")]
  public BonusEnemySpawns KeybulletsChances;
  public BonusEnemySpawns ChanceBulletChances;
  public BonusEnemySpawns WallMimicChances;
  [Header("Heart Magnificence Settings")]
  public float OneOrTwoHeartMagMultiplier = 0.333f;
  public float ThreeOrMoreHeartMagMultiplier = 0.1f;
  [Header("Chest Destruction Settings")]
  public float ChestDowngradeChance = 0.25f;
  public float ChestHalfHeartChance = 0.2f;
  public float ChestJunkChance = 0.45f;
  public float ChestExplosionChance = 0.1f;
  public float ChestJunkanUnlockedChance = 0.05f;
  public float HasKeyJunkMultiplier = 3f;
  public float HasJunkanJunkMultiplier = 1.5f;
  [Header("Data References (for Brents)")]
  [EnemyIdentifier]
  public string FacelessCultistGuid;
  public float FacelessChancePerFloor = 0.15f;
  [Header("Bowler Notes")]
  public GameObject BowlerNotePostRainbow;
  public GameObject BowlerNoteChest;
  public GameObject BowlerNoteOtherSource;
  public GameObject BowlerNoteMimic;
  public GameObject BowlerNoteShop;
  public GameObject BowlerNoteBoss;
  [Header("Demo Mode Stuff For Pax EAST 2018")]
  [EnemyIdentifier]
  public string PhaseSpiderGUID;
  [EnemyIdentifier]
  public string ChancebulonGUID;
  [EnemyIdentifier]
  public string DisplacerBeastGUID;
  [EnemyIdentifier]
  public string GripmasterGUID;
  public List<EnemyReplacementTier> ReplacementTiers;
  [NonSerialized]
  public Dictionary<GlobalDungeonData.ValidTilesets, FloorRewardManifest> SeededRunManifests = new Dictionary<GlobalDungeonData.ValidTilesets, FloorRewardManifest>();

  public PickupObject FullHeartPrefab => PickupObjectDatabase.GetById(this.FullHeartIdPrefab);

  public PickupObject HalfHeartPrefab => PickupObjectDatabase.GetById(this.HalfHeartIdPrefab);

  public global::FloorRewardData CurrentRewardData
  {
    get
    {
      return this.GetRewardDataForFloor(GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId);
    }
  }

  private global::FloorRewardData GetRewardDataForFloor(
    GlobalDungeonData.ValidTilesets targetTileset)
  {
    global::FloorRewardData rewardDataForFloor = (global::FloorRewardData) null;
    for (int index = 0; index < this.FloorRewardData.Count; ++index)
    {
      if ((this.FloorRewardData[index].AssociatedTilesets | targetTileset) == this.FloorRewardData[index].AssociatedTilesets)
        rewardDataForFloor = this.FloorRewardData[index];
    }
    if (rewardDataForFloor == null)
      rewardDataForFloor = this.FloorRewardData[0];
    return rewardDataForFloor;
  }

  public GameObject GetShopItemResourcefulRatStyle(
    List<GameObject> excludedObjects = null,
    System.Random safeRandom = null)
  {
    PickupObject.ItemQuality targetQuality = PickupObject.ItemQuality.D;
    switch (GameManager.Instance.Dungeon.tileIndices.tilesetId)
    {
      case GlobalDungeonData.ValidTilesets.GUNGEON:
        targetQuality = PickupObject.ItemQuality.D;
        break;
      case GlobalDungeonData.ValidTilesets.MINEGEON:
        targetQuality = PickupObject.ItemQuality.C;
        break;
      case GlobalDungeonData.ValidTilesets.CATACOMBGEON:
        targetQuality = PickupObject.ItemQuality.B;
        break;
      case GlobalDungeonData.ValidTilesets.FORGEGEON:
        targetQuality = PickupObject.ItemQuality.B;
        break;
    }
    return this.GetRawItem(this.GunsLootTable, targetQuality, excludedObjects, true, safeRandom);
  }

  public GameObject GetRewardObjectShopStyle(
    PlayerController player,
    bool forceGun = false,
    bool forceItem = false,
    List<GameObject> excludedObjects = null)
  {
    global::FloorRewardData currentRewardData = this.CurrentRewardData;
    bool flag = (!GameManager.Instance.IsSeeded ? (double) UnityEngine.Random.value : (double) BraveRandom.GenerationRandomValue()) > 0.5;
    if (forceGun)
      flag = true;
    if (forceItem)
      flag = false;
    PickupObject.ItemQuality shopTargetQuality = currentRewardData.GetShopTargetQuality(GameManager.Instance.IsSeeded);
    System.Random safeRandom = (System.Random) null;
    if (GameManager.Instance.IsSeeded)
      safeRandom = BraveRandom.GeneratorRandom;
    if (flag)
    {
      List<GameObject> gameObjectList = new List<GameObject>();
      this.ExcludeUnfinishedGunIfNecessary(gameObjectList);
      return this.GetItemForPlayer(player, this.GunsLootTable, shopTargetQuality, excludedObjects, safeRandom: safeRandom, additionalExcludedObjects: gameObjectList);
    }
    List<GameObject> gameObjectList1 = new List<GameObject>();
    this.BuildExcludedShopList(gameObjectList1);
    return this.GetItemForPlayer(player, this.ItemsLootTable, shopTargetQuality, excludedObjects, safeRandom: safeRandom, additionalExcludedObjects: gameObjectList1);
  }

  private void ExcludeUnfinishedGunIfNecessary(List<GameObject> excluded)
  {
    for (int index = 0; index < this.GunsLootTable.defaultItemDrops.elements.Count; ++index)
    {
      WeightedGameObject element = this.GunsLootTable.defaultItemDrops.elements[index];
      if ((bool) (UnityEngine.Object) element.gameObject)
      {
        PickupObject component = element.gameObject.GetComponent<PickupObject>();
        if ((bool) (UnityEngine.Object) component && component.PickupObjectId == GlobalItemIds.UnfinishedGun && GameStatsManager.HasInstance && GameStatsManager.Instance.GetFlag(GungeonFlags.ITEMSPECIFIC_AMMONOMICON_COMPLETE))
          excluded.Add(element.gameObject);
      }
    }
  }

  private void BuildExcludedShopList(List<GameObject> excluded)
  {
    for (int index = 0; index < this.ItemsLootTable.defaultItemDrops.elements.Count; ++index)
    {
      WeightedGameObject element = this.ItemsLootTable.defaultItemDrops.elements[index];
      if ((bool) (UnityEngine.Object) element.gameObject)
      {
        PickupObject component = element.gameObject.GetComponent<PickupObject>();
        if ((bool) (UnityEngine.Object) component && component.ShouldBeExcludedFromShops)
          excluded.Add(element.gameObject);
        else if ((bool) (UnityEngine.Object) component && component.PickupObjectId == GlobalItemIds.UnfinishedGun && GameStatsManager.HasInstance && GameStatsManager.Instance.GetFlag(GungeonFlags.ITEMSPECIFIC_AMMONOMICON_COMPLETE))
          excluded.Add(element.gameObject);
      }
    }
  }

  public bool IsBossRewardForcedGun()
  {
    if (GameManager.Instance.CurrentGameMode != GameManager.GameMode.BOSSRUSH)
    {
      bool flag = true;
      for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
      {
        if ((bool) (UnityEngine.Object) GameManager.Instance.AllPlayers[index] && (GameManager.Instance.AllPlayers[index].HasReceivedNewGunThisFloor || GameManager.Instance.AllPlayers[index].CharacterUsesRandomGuns))
          flag = false;
      }
      if (flag)
      {
        Debug.LogWarning((object) "Forcing boss drop GUN!");
        return true;
      }
    }
    return false;
  }

  private float ItemVsGunChanceBossReward => 0.625f;

  public GameObject GetRewardObjectForBossSeeded(List<PickupObject> AlreadyGenerated, bool forceGun)
  {
    global::FloorRewardData currentRewardData = this.CurrentRewardData;
    bool flag = forceGun || (double) BraveRandom.GenerationRandomValue() > (double) this.ItemVsGunChanceBossReward;
    return flag ? this.GetItemForSeededRun(this.GunsLootTable, currentRewardData.GetRandomBossTargetQuality(BraveRandom.GeneratorRandom), AlreadyGenerated, BraveRandom.GeneratorRandom, true) : this.GetItemForSeededRun(!flag ? this.ItemsLootTable : this.GunsLootTable, this.GetDaveStyleItemQuality(), AlreadyGenerated, BraveRandom.GeneratorRandom, true);
  }

  public GameObject GetRewardObjectBossStyle(PlayerController player)
  {
    global::FloorRewardData currentRewardData = this.CurrentRewardData;
    bool flag = GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId != GlobalDungeonData.ValidTilesets.CASTLEGEON || GameManager.Instance.CurrentGameType != GameManager.GameType.SINGLE_PLAYER || !(bool) (UnityEngine.Object) player || player.inventory == null || player.inventory.GunCountModified > 3 ? (GameManager.Instance.CurrentGameType != GameManager.GameType.SINGLE_PLAYER || !(bool) (UnityEngine.Object) player || player.inventory == null || player.inventory.GunCountModified > 2 ? (double) UnityEngine.Random.value > (double) this.ItemVsGunChanceBossReward : (double) UnityEngine.Random.value > 0.30000001192092896) : (double) UnityEngine.Random.value > 0.20000000298023224;
    if (this.IsBossRewardForcedGun())
      flag = true;
    if ((GameManager.Instance.CurrentGameMode == GameManager.GameMode.BOSSRUSH || GameManager.Instance.CurrentGameMode == GameManager.GameMode.SUPERBOSSRUSH) && !GameManager.Instance.Dungeon.HasGivenBossrushGun)
    {
      GameManager.Instance.Dungeon.HasGivenBossrushGun = true;
      flag = true;
    }
    if (!flag)
      return this.GetRewardItemDaveStyle(player, true);
    PickupObject.ItemQuality bossTargetQuality = currentRewardData.GetRandomBossTargetQuality();
    return this.GetItemForPlayer(player, this.GunsLootTable, bossTargetQuality, (List<GameObject>) null, rewardSource: RewardManager.RewardSource.BOSS_PEDESTAL);
  }

  private PickupObject.ItemQuality GetDaveStyleItemQuality()
  {
    float num1 = 0.1f;
    float num2 = 0.4f;
    float num3 = 0.7f;
    float num4 = 0.95f;
    float num5 = UnityEngine.Random.value;
    PickupObject.ItemQuality styleItemQuality = PickupObject.ItemQuality.D;
    if ((double) num5 > (double) num1 && (double) num5 <= (double) num2)
      styleItemQuality = PickupObject.ItemQuality.C;
    else if ((double) num5 > (double) num2 && (double) num5 <= (double) num3)
      styleItemQuality = PickupObject.ItemQuality.B;
    else if ((double) num5 > (double) num3 && (double) num5 <= (double) num4)
      styleItemQuality = PickupObject.ItemQuality.A;
    else if ((double) num5 > (double) num4)
      styleItemQuality = PickupObject.ItemQuality.S;
    return styleItemQuality;
  }

  private GameObject GetRewardItemDaveStyle(PlayerController player, bool bossStyle = false)
  {
    PickupObject.ItemQuality styleItemQuality = this.GetDaveStyleItemQuality();
    Debug.Log((object) ("Get Reward Item Dave Style: " + styleItemQuality.ToString()));
    RewardManager.RewardSource rewardSource = !bossStyle ? RewardManager.RewardSource.UNSPECIFIED : RewardManager.RewardSource.BOSS_PEDESTAL;
    return this.GetItemForPlayer(player, this.ItemsLootTable, styleItemQuality, (List<GameObject>) null, bossStyle: bossStyle, rewardSource: rewardSource);
  }

  public GameObject GetRewardObjectDaveStyle(PlayerController player)
  {
    global::FloorRewardData currentRewardData = this.CurrentRewardData;
    if ((double) UnityEngine.Random.value <= 0.5)
      return this.GetRewardItemDaveStyle(player);
    PickupObject.ItemQuality randomTargetQuality = currentRewardData.GetRandomTargetQuality();
    return this.GetItemForPlayer(player, this.GunsLootTable, randomTargetQuality, (List<GameObject>) null);
  }

  public static bool PlayerHasItemInSynergyContainingOtherItem(
    PlayerController player,
    PickupObject prefab)
  {
    bool usesStartingItem = false;
    return RewardManager.PlayerHasItemInSynergyContainingOtherItem(player, prefab, ref usesStartingItem);
  }

  public static bool TestItemWouldCompleteSpecificSynergy(
    AdvancedSynergyEntry entry,
    PickupObject newPickup)
  {
    return entry.ActivationStatus != SynergyEntry.SynergyActivation.INACTIVE && !entry.SynergyIsAvailable(GameManager.Instance.PrimaryPlayer, GameManager.Instance.SecondaryPlayer) && entry.SynergyIsAvailable(GameManager.Instance.PrimaryPlayer, GameManager.Instance.SecondaryPlayer, newPickup.PickupObjectId);
  }

  public static bool AnyPlayerHasItemInSynergyContainingOtherItem(
    PickupObject prefab,
    ref bool usesStartingItem)
  {
    for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
    {
      PlayerController allPlayer = GameManager.Instance.AllPlayers[index];
      if ((bool) (UnityEngine.Object) allPlayer && RewardManager.PlayerHasItemInSynergyContainingOtherItem(allPlayer, prefab, ref usesStartingItem))
        return true;
    }
    return false;
  }

  public static bool PlayerHasItemInSynergyContainingOtherItem(
    PlayerController player,
    PickupObject prefab,
    ref bool usesStartingItem)
  {
    int pickupObjectId = prefab.PickupObjectId;
    foreach (AdvancedSynergyEntry synergy in GameManager.Instance.SynergyManager.synergies)
    {
      if (synergy.ActivationStatus != SynergyEntry.SynergyActivation.INACTIVE && synergy.ActivationStatus != SynergyEntry.SynergyActivation.DEMO && synergy.ActivationStatus != SynergyEntry.SynergyActivation.ACTIVE_UNBOOSTED && synergy.ContainsPickup(pickupObjectId))
      {
        bool flag1 = false;
        for (int index = 0; index < player.inventory.AllGuns.Count; ++index)
        {
          bool flag2 = synergy.ContainsPickup(player.inventory.AllGuns[index].PickupObjectId);
          if (flag2)
            flag2 = RewardManager.TestItemWouldCompleteSpecificSynergy(synergy, prefab);
          flag1 |= flag2;
          if (flag2)
            usesStartingItem |= player.startingGunIds.Contains(player.inventory.AllGuns[index].PickupObjectId);
          if (flag2)
            usesStartingItem |= player.startingAlternateGunIds.Contains(player.inventory.AllGuns[index].PickupObjectId);
        }
        if (!flag1)
        {
          for (int index = 0; index < player.activeItems.Count; ++index)
          {
            bool flag3 = synergy.ContainsPickup(player.activeItems[index].PickupObjectId);
            if (flag3)
              flag3 = RewardManager.TestItemWouldCompleteSpecificSynergy(synergy, prefab);
            flag1 |= flag3;
            if (flag3)
              usesStartingItem |= player.startingActiveItemIds.Contains(player.activeItems[index].PickupObjectId);
          }
        }
        if (!flag1)
        {
          for (int index = 0; index < player.passiveItems.Count; ++index)
          {
            bool flag4 = synergy.ContainsPickup(player.passiveItems[index].PickupObjectId);
            if (flag4)
              flag4 = RewardManager.TestItemWouldCompleteSpecificSynergy(synergy, prefab);
            flag1 |= flag4;
            if (flag4)
              usesStartingItem |= player.startingPassiveItemIds.Contains(player.passiveItems[index].PickupObjectId);
          }
        }
        if (!flag1 && SynercacheManager.UseCachedSynergyIDs)
        {
          for (int index = 0; index < SynercacheManager.LastCachedSynergyIDs.Count; ++index)
            flag1 = flag1 | synergy.ContainsPickup(SynercacheManager.LastCachedSynergyIDs[index]) | synergy.ContainsPickup(SynercacheManager.LastCachedSynergyIDs[index]);
        }
        if (flag1)
          return true;
      }
    }
    return false;
  }

  public static bool CheckQualityForItem(
    PickupObject prefab,
    PlayerController player,
    PickupObject.ItemQuality targetQuality,
    bool completesSynergy,
    RewardManager.RewardSource source)
  {
    bool flag1 = prefab.quality == targetQuality;
    if (!(bool) (UnityEngine.Object) player)
      return flag1;
    bool flag2 = completesSynergy || GameManager.Instance.RewardManager.SynergyCompletionIgnoresQualities;
    if (GameStatsManager.Instance.GetNumberOfSynergiesEncounteredThisRun() == 0 && source == RewardManager.RewardSource.BOSS_PEDESTAL)
      flag2 = true;
    if (!flag1 && flag2 && RewardManager.PlayerHasItemInSynergyContainingOtherItem(player, prefab))
      flag1 = true;
    return flag1;
  }

  public static bool AnyPlayerHasItem(int id)
  {
    for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
    {
      PlayerController allPlayer = GameManager.Instance.AllPlayers[index];
      if ((bool) (UnityEngine.Object) allPlayer && (allPlayer.HasPassiveItem(id) || allPlayer.HasActiveItem(id) || allPlayer.HasGun(id)))
        return true;
    }
    return false;
  }

  public static float GetMultiplierForItem(
    PickupObject prefab,
    PlayerController player,
    bool completesSynergy)
  {
    if (!(bool) (UnityEngine.Object) prefab)
      return 1f;
    float multiplierForItem = 1f;
    int pickupObjectId = prefab.PickupObjectId;
    if ((UnityEngine.Object) player == (UnityEngine.Object) null)
      return multiplierForItem;
    bool flag = false;
    float num = SynergyFactorConstants.GetSynergyFactor();
    if (completesSynergy)
    {
      if (RewardManager.AnyPlayerHasItem(prefab.PickupObjectId) || prefab is BasicStatPickup && (prefab as BasicStatPickup).IsMasteryToken)
        return 0.0f;
      num = 1E+08f;
    }
    if ((double) num > 1.0 || flag)
    {
      bool usesStartingItem = false;
      if (RewardManager.AnyPlayerHasItemInSynergyContainingOtherItem(prefab, ref usesStartingItem))
      {
        if (completesSynergy && usesStartingItem)
          num = 10000f;
        else if (usesStartingItem)
          num = 1f;
        multiplierForItem *= num;
      }
    }
    for (int index = 0; index < player.lootModData.Count; ++index)
    {
      if (player.lootModData[index].AssociatedPickupId == pickupObjectId)
        multiplierForItem *= player.lootModData[index].DropRateMultiplier;
    }
    return multiplierForItem;
  }

  public GameObject GetRawItem(
    GenericLootTable tableToUse,
    PickupObject.ItemQuality targetQuality,
    List<GameObject> excludedObjects,
    bool ignorePlayerTraits = false,
    System.Random safeRandom = null)
  {
    bool flag1 = false;
    while (targetQuality >= PickupObject.ItemQuality.COMMON)
    {
      if (targetQuality > PickupObject.ItemQuality.COMMON)
        flag1 = true;
      List<WeightedGameObject> compiledRawItems = tableToUse.GetCompiledRawItems();
      List<KeyValuePair<WeightedGameObject, float>> keyValuePairList = new List<KeyValuePair<WeightedGameObject, float>>();
      float num1 = 0.0f;
      for (int index = 0; index < compiledRawItems.Count; ++index)
      {
        if ((UnityEngine.Object) compiledRawItems[index].gameObject != (UnityEngine.Object) null)
        {
          PickupObject component = compiledRawItems[index].gameObject.GetComponent<PickupObject>();
          if (!((UnityEngine.Object) component == (UnityEngine.Object) null))
          {
            bool flag2 = component.quality == targetQuality;
            if ((UnityEngine.Object) component != (UnityEngine.Object) null && flag2)
            {
              bool flag3 = true;
              float weight = compiledRawItems[index].weight;
              if (excludedObjects == null || !excludedObjects.Contains(component.gameObject))
              {
                if (!component.PrerequisitesMet())
                  flag3 = false;
                if (flag3)
                {
                  num1 += weight;
                  KeyValuePair<WeightedGameObject, float> keyValuePair = new KeyValuePair<WeightedGameObject, float>(compiledRawItems[index], weight);
                  keyValuePairList.Add(keyValuePair);
                }
              }
            }
          }
        }
      }
      if ((double) num1 > 0.0 && keyValuePairList.Count > 0)
      {
        float num2;
        if (ignorePlayerTraits)
        {
          float num3 = (float) safeRandom.NextDouble();
          num2 = num1 * num3;
        }
        else
          num2 = num1 * UnityEngine.Random.value;
        for (int index = 0; index < keyValuePairList.Count; ++index)
        {
          num2 -= keyValuePairList[index].Value;
          if ((double) num2 <= 0.0)
            return keyValuePairList[index].Key.gameObject;
        }
        return keyValuePairList[keyValuePairList.Count - 1].Key.gameObject;
      }
      --targetQuality;
      if (targetQuality < PickupObject.ItemQuality.COMMON && !flag1)
        targetQuality = PickupObject.ItemQuality.D;
    }
    return (GameObject) null;
  }

  public GameObject GetItemForPlayer(
    PlayerController player,
    GenericLootTable tableToUse,
    PickupObject.ItemQuality targetQuality,
    List<GameObject> excludedObjects,
    bool ignorePlayerTraits = false,
    System.Random safeRandom = null,
    bool bossStyle = false,
    List<GameObject> additionalExcludedObjects = null,
    bool forceSynergyCompletion = false,
    RewardManager.RewardSource rewardSource = RewardManager.RewardSource.UNSPECIFIED)
  {
    bool flag1 = false;
    while (targetQuality >= PickupObject.ItemQuality.COMMON)
    {
      if (targetQuality > PickupObject.ItemQuality.COMMON)
        flag1 = true;
      List<WeightedGameObject> compiledRawItems = tableToUse.GetCompiledRawItems();
      List<KeyValuePair<WeightedGameObject, float>> keyValuePairList1 = new List<KeyValuePair<WeightedGameObject, float>>();
      float num1 = 0.0f;
      List<KeyValuePair<WeightedGameObject, float>> keyValuePairList2 = new List<KeyValuePair<WeightedGameObject, float>>();
      float num2 = 0.0f;
      for (int index = 0; index < compiledRawItems.Count; ++index)
      {
        if ((UnityEngine.Object) compiledRawItems[index].gameObject != (UnityEngine.Object) null)
        {
          PickupObject component1 = compiledRawItems[index].gameObject.GetComponent<PickupObject>();
          if (!((UnityEngine.Object) component1 == (UnityEngine.Object) null) && (!bossStyle || !(component1 is GungeonMapItem)))
          {
            bool flag2 = RewardManager.CheckQualityForItem(component1, player, targetQuality, forceSynergyCompletion, rewardSource);
            if ((component1.ItemSpansBaseQualityTiers || component1.ItemRespectsHeartMagnificence) && targetQuality != PickupObject.ItemQuality.D && targetQuality != PickupObject.ItemQuality.COMMON && targetQuality != PickupObject.ItemQuality.S)
              flag2 = true;
            if (!ignorePlayerTraits && component1 is SpiceItem && (bool) (UnityEngine.Object) player && player.spiceCount > 0)
            {
              Debug.Log((object) "BAM spicing it up");
              flag2 = true;
            }
            if ((UnityEngine.Object) component1 != (UnityEngine.Object) null && flag2)
            {
              bool flag3 = true;
              float weight = compiledRawItems[index].weight;
              bool flag4;
              if (excludedObjects != null && excludedObjects.Contains(component1.gameObject))
                flag4 = false;
              else if (additionalExcludedObjects != null && additionalExcludedObjects.Contains(component1.gameObject))
              {
                flag4 = false;
              }
              else
              {
                if (!component1.PrerequisitesMet())
                  flag3 = false;
                if (component1 is Gun)
                {
                  Gun gun = component1 as Gun;
                  if (gun.InfiniteAmmo && !gun.CanBeDropped && gun.quality == PickupObject.ItemQuality.SPECIAL)
                  {
                    flag4 = false;
                    continue;
                  }
                  GunClass gunClass = gun.gunClass;
                  if (!ignorePlayerTraits && gunClass != GunClass.NONE)
                  {
                    int p = (UnityEngine.Object) player == (UnityEngine.Object) null || player.inventory == null ? 0 : player.inventory.ContainsGunOfClass(gunClass, true);
                    float modifierForClass = LootDataGlobalSettings.Instance.GetModifierForClass(gunClass);
                    weight *= Mathf.Pow(modifierForClass, (float) p);
                  }
                }
                if (!ignorePlayerTraits)
                {
                  float multiplierForItem = RewardManager.GetMultiplierForItem(component1, player, forceSynergyCompletion);
                  weight *= multiplierForItem;
                }
                bool flag5 = !GameManager.Instance.IsSeeded;
                EncounterTrackable component2 = component1.GetComponent<EncounterTrackable>();
                if ((UnityEngine.Object) component2 != (UnityEngine.Object) null && flag5)
                {
                  int num3 = 0;
                  if (Application.isPlaying)
                    num3 = GameStatsManager.Instance.QueryEncounterableDifferentiator(component2);
                  if (num3 > 0 || Application.isPlaying && GameManager.Instance.ExtantShopTrackableGuids.Contains(component2.EncounterGuid))
                  {
                    flag3 = false;
                    num2 += weight;
                    KeyValuePair<WeightedGameObject, float> keyValuePair = new KeyValuePair<WeightedGameObject, float>(compiledRawItems[index], weight);
                    keyValuePairList2.Add(keyValuePair);
                  }
                  else if (Application.isPlaying && GameStatsManager.Instance.QueryEncounterable(component2) == 0 && GameStatsManager.Instance.QueryEncounterableAnnouncement(component2.EncounterGuid))
                    weight *= 10f;
                }
                if (component1.ItemSpansBaseQualityTiers || component1.ItemRespectsHeartMagnificence)
                {
                  if ((double) RewardManager.AdditionalHeartTierMagnificence >= 3.0)
                    weight *= this.ThreeOrMoreHeartMagMultiplier;
                  else if ((double) RewardManager.AdditionalHeartTierMagnificence >= 1.0)
                    weight *= this.OneOrTwoHeartMagMultiplier;
                }
                if (flag3)
                {
                  num1 += weight;
                  KeyValuePair<WeightedGameObject, float> keyValuePair = new KeyValuePair<WeightedGameObject, float>(compiledRawItems[index], weight);
                  keyValuePairList1.Add(keyValuePair);
                }
              }
            }
          }
        }
      }
      if (keyValuePairList1.Count == 0 && keyValuePairList2.Count > 0)
      {
        keyValuePairList1 = keyValuePairList2;
        num1 = num2;
      }
      if ((double) num1 > 0.0 && keyValuePairList1.Count > 0)
      {
        float num4;
        if (ignorePlayerTraits)
        {
          float num5 = (float) safeRandom.NextDouble();
          Debug.LogError((object) ("safe random: " + (object) num5));
          num4 = num1 * num5;
        }
        else
          num4 = num1 * UnityEngine.Random.value;
        for (int index = 0; index < keyValuePairList1.Count; ++index)
        {
          num4 -= keyValuePairList1[index].Value;
          if ((double) num4 <= 0.0)
            return keyValuePairList1[index].Key.gameObject;
        }
        return keyValuePairList1[keyValuePairList1.Count - 1].Key.gameObject;
      }
      --targetQuality;
      if (targetQuality < PickupObject.ItemQuality.COMMON && !flag1)
        targetQuality = PickupObject.ItemQuality.D;
    }
    return (GameObject) null;
  }

  public GameObject GetItemForSeededRun(
    GenericLootTable tableToUse,
    PickupObject.ItemQuality targetQuality,
    List<PickupObject> AlreadyGeneratedItems,
    System.Random safeRandom,
    bool bossStyle = false)
  {
    bool flag1 = false;
    while (targetQuality >= PickupObject.ItemQuality.COMMON)
    {
      if (targetQuality > PickupObject.ItemQuality.COMMON)
        flag1 = true;
      List<WeightedGameObject> compiledRawItems = tableToUse.GetCompiledRawItems();
      List<KeyValuePair<WeightedGameObject, float>> keyValuePairList = new List<KeyValuePair<WeightedGameObject, float>>();
      float num1 = 0.0f;
      for (int index1 = 0; index1 < compiledRawItems.Count; ++index1)
      {
        if ((UnityEngine.Object) compiledRawItems[index1].gameObject != (UnityEngine.Object) null)
        {
          PickupObject component = compiledRawItems[index1].gameObject.GetComponent<PickupObject>();
          if (!((UnityEngine.Object) component == (UnityEngine.Object) null) && (!bossStyle || !(component is GungeonMapItem)))
          {
            bool flag2 = component.quality == targetQuality;
            if ((component.ItemSpansBaseQualityTiers || component.ItemRespectsHeartMagnificence) && targetQuality != PickupObject.ItemQuality.D && targetQuality != PickupObject.ItemQuality.COMMON && targetQuality != PickupObject.ItemQuality.S)
              flag2 = true;
            if ((UnityEngine.Object) component != (UnityEngine.Object) null && flag2)
            {
              bool flag3 = true;
              float weight = compiledRawItems[index1].weight;
              bool flag4;
              if (AlreadyGeneratedItems != null && AlreadyGeneratedItems.Contains(component))
              {
                flag4 = false;
              }
              else
              {
                if (!component.PrerequisitesMet())
                  flag3 = false;
                if (component is Gun)
                {
                  Gun gun = component as Gun;
                  if (gun.InfiniteAmmo && !gun.CanBeDropped && gun.quality == PickupObject.ItemQuality.SPECIAL)
                  {
                    flag4 = false;
                    continue;
                  }
                }
                if (!GameManager.Instance.RewardManager.IsItemInSeededManifests(component))
                {
                  float num2 = 1f;
                  if (AlreadyGeneratedItems != null)
                  {
                    for (int index2 = 0; index2 < AlreadyGeneratedItems.Count; ++index2)
                    {
                      for (int index3 = 0; index3 < AlreadyGeneratedItems[index2].associatedItemChanceMods.Length; ++index3)
                      {
                        if (AlreadyGeneratedItems[index2].associatedItemChanceMods[index3].AssociatedPickupId == component.PickupObjectId)
                          num2 *= AlreadyGeneratedItems[index2].associatedItemChanceMods[index3].DropRateMultiplier;
                      }
                    }
                  }
                  float num3 = weight * num2;
                  if (component.ItemSpansBaseQualityTiers || component.ItemRespectsHeartMagnificence)
                  {
                    if ((double) RewardManager.AdditionalHeartTierMagnificence >= 3.0)
                      num3 *= this.ThreeOrMoreHeartMagMultiplier;
                    else if ((double) RewardManager.AdditionalHeartTierMagnificence >= 1.0)
                      num3 *= this.OneOrTwoHeartMagMultiplier;
                  }
                  if (flag3)
                  {
                    num1 += num3;
                    KeyValuePair<WeightedGameObject, float> keyValuePair = new KeyValuePair<WeightedGameObject, float>(compiledRawItems[index1], num3);
                    keyValuePairList.Add(keyValuePair);
                  }
                }
              }
            }
          }
        }
      }
      if ((double) num1 > 0.0 && keyValuePairList.Count > 0)
      {
        float num4 = (float) safeRandom.NextDouble();
        float num5 = num1 * num4;
        for (int index = 0; index < keyValuePairList.Count; ++index)
        {
          num5 -= keyValuePairList[index].Value;
          if ((double) num5 <= 0.0)
            return keyValuePairList[index].Key.gameObject;
        }
        return keyValuePairList[keyValuePairList.Count - 1].Key.gameObject;
      }
      --targetQuality;
      if (targetQuality < PickupObject.ItemQuality.COMMON && !flag1)
        targetQuality = PickupObject.ItemQuality.D;
    }
    return (GameObject) null;
  }

  private Chest GetTargetChestPrefab(PickupObject.ItemQuality targetQuality)
  {
    Chest targetChestPrefab = (Chest) null;
    switch (targetQuality)
    {
      case PickupObject.ItemQuality.D:
        targetChestPrefab = this.D_Chest;
        break;
      case PickupObject.ItemQuality.C:
        targetChestPrefab = this.C_Chest;
        break;
      case PickupObject.ItemQuality.B:
        targetChestPrefab = this.B_Chest;
        break;
      case PickupObject.ItemQuality.A:
        targetChestPrefab = this.A_Chest;
        break;
      case PickupObject.ItemQuality.S:
        targetChestPrefab = this.S_Chest;
        break;
    }
    return targetChestPrefab;
  }

  private Chest SpawnInternal(
    IntVector2 position,
    float gunVersusItemPercentChance,
    PickupObject.ItemQuality targetQuality,
    Chest overrideChestPrefab = null)
  {
    Chest chestPrefab = overrideChestPrefab ?? this.GetTargetChestPrefab(targetQuality);
    GenericLootTable genericLootTable = (double) UnityEngine.Random.value >= (double) gunVersusItemPercentChance ? this.ItemsLootTable : this.GunsLootTable;
    Chest chest = Chest.Spawn(chestPrefab, position);
    chest.lootTable.lootTable = genericLootTable;
    if (chest.lootTable.canDropMultipleItems && chest.lootTable.overrideItemLootTables != null && chest.lootTable.overrideItemLootTables.Count > 0)
      chest.lootTable.overrideItemLootTables[0] = genericLootTable;
    return chest;
  }

  public Chest SpawnRoomClearChestAt(IntVector2 position)
  {
    global::FloorRewardData rewardDataForFloor = this.GetRewardDataForFloor(GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId);
    PickupObject.ItemQuality roomTargetQuality = rewardDataForFloor.GetRandomRoomTargetQuality();
    int count = -1;
    if ((roomTargetQuality == PickupObject.ItemQuality.D || roomTargetQuality == PickupObject.ItemQuality.C) && PlayerController.AnyoneHasActiveBonusSynergy(CustomSynergyType.DOUBLE_CHEST_FRIENDS, out count))
      roomTargetQuality = rewardDataForFloor.GetRandomRoomTargetQuality();
    Chest overrideChestPrefab = (Chest) null;
    if ((double) UnityEngine.Random.value < (double) this.RoomClearRainbowChance)
      overrideChestPrefab = this.Rainbow_Chest;
    return this.SpawnInternal(position, rewardDataForFloor.GunVersusItemPercentChance, roomTargetQuality, overrideChestPrefab);
  }

  public DebrisObject SpawnTotallyRandomItem(
    Vector2 position,
    PickupObject.ItemQuality startQuality = PickupObject.ItemQuality.D,
    PickupObject.ItemQuality endQuality = PickupObject.ItemQuality.S)
  {
    PickupObject.ItemQuality targetQuality = (PickupObject.ItemQuality) UnityEngine.Random.Range((int) startQuality, (int) (endQuality + 1));
    return LootEngine.SpawnItem(this.GetItemForPlayer(GameManager.Instance.PrimaryPlayer, (double) UnityEngine.Random.value >= 0.5 ? this.ItemsLootTable : this.GunsLootTable, targetQuality, (List<GameObject>) null).gameObject, (Vector3) position, Vector2.zero, 0.0f);
  }

  public Chest SpawnTotallyRandomChest(IntVector2 position)
  {
    PickupObject.ItemQuality targetQuality = (PickupObject.ItemQuality) UnityEngine.Random.Range(1, 6);
    if (PassiveItem.IsFlagSetAtAll(typeof (SevenLeafCloverItem)))
      targetQuality = (double) UnityEngine.Random.value >= 0.5 ? PickupObject.ItemQuality.S : PickupObject.ItemQuality.A;
    return this.SpawnInternal(position, 0.5f, targetQuality);
  }

  public PickupObject.ItemQuality GetQualityFromChest(Chest c)
  {
    if (this.CompareChest(c, this.D_Chest))
      return PickupObject.ItemQuality.D;
    if (this.CompareChest(c, this.C_Chest))
      return PickupObject.ItemQuality.C;
    if (this.CompareChest(c, this.B_Chest))
      return PickupObject.ItemQuality.B;
    if (this.CompareChest(c, this.A_Chest))
      return PickupObject.ItemQuality.A;
    return this.CompareChest(c, this.S_Chest) ? PickupObject.ItemQuality.S : PickupObject.ItemQuality.EXCLUDED;
  }

  private bool CompareChest(Chest c1, Chest c2)
  {
    return (double) c1.lootTable.D_Chance == (double) c2.lootTable.D_Chance && (double) c1.lootTable.C_Chance == (double) c2.lootTable.C_Chance && (double) c1.lootTable.B_Chance == (double) c2.lootTable.B_Chance && (double) c1.lootTable.A_Chance == (double) c2.lootTable.A_Chance && (double) c1.lootTable.S_Chance == (double) c2.lootTable.S_Chance;
  }

  public Chest SpawnRewardChestAt(
    IntVector2 position,
    float overrideGunVsItemChance = -1f,
    PickupObject.ItemQuality excludedQuality = PickupObject.ItemQuality.EXCLUDED)
  {
    global::FloorRewardData rewardDataForFloor = this.GetRewardDataForFloor(GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId);
    PickupObject.ItemQuality targetQuality = rewardDataForFloor.GetRandomTargetQuality();
    if (PassiveItem.IsFlagSetAtAll(typeof (SevenLeafCloverItem)))
      targetQuality = (double) UnityEngine.Random.value >= 0.5 ? PickupObject.ItemQuality.S : PickupObject.ItemQuality.A;
    return this.SpawnInternal(position, (double) overrideGunVsItemChance < 0.0 ? rewardDataForFloor.GunVersusItemPercentChance : overrideGunVsItemChance, targetQuality);
  }

  public Chest GenerationSpawnRewardChestAt(
    IntVector2 positionInRoom,
    RoomHandler targetRoom,
    PickupObject.ItemQuality? targetQuality = null,
    float overrideMimicChance = -1f)
  {
    System.Random generatorRandom = !GameManager.Instance.IsSeeded ? (System.Random) null : BraveRandom.GeneratorRandom;
    global::FloorRewardData rewardDataForFloor = this.GetRewardDataForFloor(GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId);
    bool forceDChanceZero = StaticReferenceManager.DChestsSpawnedInTotal >= 2;
    if (!targetQuality.HasValue)
    {
      targetQuality = new PickupObject.ItemQuality?(rewardDataForFloor.GetRandomTargetQuality(true, forceDChanceZero));
      if (PassiveItem.IsFlagSetAtAll(typeof (SevenLeafCloverItem)))
        targetQuality = new PickupObject.ItemQuality?((generatorRandom == null ? (double) UnityEngine.Random.value : generatorRandom.NextDouble()) >= 0.5 ? PickupObject.ItemQuality.S : PickupObject.ItemQuality.A);
    }
    if ((targetQuality.GetValueOrDefault() != PickupObject.ItemQuality.D ? 0 : (targetQuality.HasValue ? 1 : 0)) != 0 && StaticReferenceManager.DChestsSpawnedOnFloor >= 1 && GameManager.Instance.Dungeon.tileIndices.tilesetId != GlobalDungeonData.ValidTilesets.CASTLEGEON)
      targetQuality = new PickupObject.ItemQuality?(PickupObject.ItemQuality.C);
    Vector2 vector = Vector2.zero;
    if ((targetQuality.GetValueOrDefault() != PickupObject.ItemQuality.A ? 0 : (targetQuality.HasValue ? 1 : 0)) != 0 || (targetQuality.GetValueOrDefault() != PickupObject.ItemQuality.S ? 0 : (targetQuality.HasValue ? 1 : 0)) != 0)
      vector = new Vector2(-0.5f, 0.0f);
    Chest chest = this.GetTargetChestPrefab(targetQuality.Value);
    if (GameStatsManager.Instance.GetFlag(GungeonFlags.SYNERGRACE_UNLOCKED) && GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId != GlobalDungeonData.ValidTilesets.CASTLEGEON && (generatorRandom == null ? (double) UnityEngine.Random.value : generatorRandom.NextDouble()) < (double) this.GlobalSynerchestChance)
    {
      chest = this.Synergy_Chest;
      vector = new Vector2(-3f / 16f, 0.0f);
    }
    Chest.GeneralChestType generalChestType = (double) BraveRandom.GenerationRandomValue() >= (double) rewardDataForFloor.GunVersusItemPercentChance ? Chest.GeneralChestType.ITEM : Chest.GeneralChestType.WEAPON;
    if (StaticReferenceManager.ItemChestsSpawnedOnFloor > 0 && StaticReferenceManager.WeaponChestsSpawnedOnFloor == 0)
      generalChestType = Chest.GeneralChestType.WEAPON;
    else if (StaticReferenceManager.WeaponChestsSpawnedOnFloor > 0 && StaticReferenceManager.ItemChestsSpawnedOnFloor == 0)
      generalChestType = Chest.GeneralChestType.ITEM;
    GenericLootTable genericLootTable = generalChestType != Chest.GeneralChestType.WEAPON ? this.ItemsLootTable : this.GunsLootTable;
    GameObject gameObject = DungeonPlaceableUtility.InstantiateDungeonPlaceable(chest.gameObject, targetRoom, positionInRoom, true);
    gameObject.transform.position += vector.ToVector3ZUp();
    Chest component = gameObject.GetComponent<Chest>();
    if ((double) overrideMimicChance >= 0.0)
      component.overrideMimicChance = overrideMimicChance;
    foreach (Component componentsInChild in gameObject.GetComponentsInChildren(typeof (IPlaceConfigurable)))
    {
      if (componentsInChild is IPlaceConfigurable placeConfigurable)
        placeConfigurable.ConfigureOnPlacement(targetRoom);
    }
    if ((targetQuality.GetValueOrDefault() != PickupObject.ItemQuality.A ? 0 : (targetQuality.HasValue ? 1 : 0)) != 0)
    {
      ++GameManager.Instance.Dungeon.GeneratedMagnificence;
      ++component.GeneratedMagnificence;
    }
    else if ((targetQuality.GetValueOrDefault() != PickupObject.ItemQuality.S ? 0 : (targetQuality.HasValue ? 1 : 0)) != 0)
    {
      ++GameManager.Instance.Dungeon.GeneratedMagnificence;
      ++component.GeneratedMagnificence;
    }
    if ((bool) (UnityEngine.Object) component.specRigidbody)
      component.specRigidbody.Reinitialize();
    component.ChestType = generalChestType;
    component.lootTable.lootTable = genericLootTable;
    if (component.lootTable.canDropMultipleItems && component.lootTable.overrideItemLootTables != null && component.lootTable.overrideItemLootTables.Count > 0)
      component.lootTable.overrideItemLootTables[0] = genericLootTable;
    if ((targetQuality.GetValueOrDefault() != PickupObject.ItemQuality.D ? 0 : (targetQuality.HasValue ? 1 : 0)) != 0 && !component.IsMimic)
    {
      ++StaticReferenceManager.DChestsSpawnedOnFloor;
      ++StaticReferenceManager.DChestsSpawnedInTotal;
      component.IsLocked = true;
      if ((bool) (UnityEngine.Object) component.LockAnimator)
        component.LockAnimator.renderer.enabled = true;
    }
    targetRoom.RegisterInteractable((IPlayerInteractable) component);
    if (this.SeededRunManifests.ContainsKey(GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId))
      component.GenerationDetermineContents(this.SeededRunManifests[GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId], generatorRandom);
    return component;
  }

  public bool IsItemInSeededManifests(PickupObject testItem)
  {
    foreach (KeyValuePair<GlobalDungeonData.ValidTilesets, FloorRewardManifest> seededRunManifest in this.SeededRunManifests)
    {
      if (seededRunManifest.Value.CheckManifestDifferentiator(testItem))
        return true;
    }
    return false;
  }

  public FloorRewardManifest GetSeededManifestForCurrentFloor()
  {
    GlobalDungeonData.ValidTilesets tilesetId = GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId;
    return this.SeededRunManifests != null && this.SeededRunManifests.ContainsKey(tilesetId) ? this.SeededRunManifests[tilesetId] : (FloorRewardManifest) null;
  }

  public void CopyBossGunChancesFromTierZero(int targetTier)
  {
    this.FloorRewardData[targetTier].D_BossGun_Chance = this.FloorRewardData[0].D_BossGun_Chance;
    this.FloorRewardData[targetTier].C_BossGun_Chance = this.FloorRewardData[0].C_BossGun_Chance;
    this.FloorRewardData[targetTier].B_BossGun_Chance = this.FloorRewardData[0].B_BossGun_Chance;
    this.FloorRewardData[targetTier].A_BossGun_Chance = this.FloorRewardData[0].A_BossGun_Chance;
    this.FloorRewardData[targetTier].S_BossGun_Chance = this.FloorRewardData[0].S_BossGun_Chance;
  }

  public void CopyShopChancesFromTierZero(int targetTier)
  {
    this.FloorRewardData[targetTier].D_Shop_Chance = this.FloorRewardData[0].D_Shop_Chance;
    this.FloorRewardData[targetTier].C_Shop_Chance = this.FloorRewardData[0].C_Shop_Chance;
    this.FloorRewardData[targetTier].B_Shop_Chance = this.FloorRewardData[0].B_Shop_Chance;
    this.FloorRewardData[targetTier].A_Shop_Chance = this.FloorRewardData[0].A_Shop_Chance;
    this.FloorRewardData[targetTier].S_Shop_Chance = this.FloorRewardData[0].S_Shop_Chance;
  }

  public void CopyChestChancesFromTierZero(int targetTier)
  {
    this.FloorRewardData[targetTier].D_Chest_Chance = this.FloorRewardData[0].D_Chest_Chance;
    this.FloorRewardData[targetTier].C_Chest_Chance = this.FloorRewardData[0].C_Chest_Chance;
    this.FloorRewardData[targetTier].B_Chest_Chance = this.FloorRewardData[0].B_Chest_Chance;
    this.FloorRewardData[targetTier].A_Chest_Chance = this.FloorRewardData[0].A_Chest_Chance;
    this.FloorRewardData[targetTier].S_Chest_Chance = this.FloorRewardData[0].S_Chest_Chance;
  }

  public void CopyDropChestChancesFromTierZero(int targetTier)
  {
    this.FloorRewardData[targetTier].D_RoomChest_Chance = this.FloorRewardData[0].D_RoomChest_Chance;
    this.FloorRewardData[targetTier].C_RoomChest_Chance = this.FloorRewardData[0].C_RoomChest_Chance;
    this.FloorRewardData[targetTier].B_RoomChest_Chance = this.FloorRewardData[0].B_RoomChest_Chance;
    this.FloorRewardData[targetTier].A_RoomChest_Chance = this.FloorRewardData[0].A_RoomChest_Chance;
    this.FloorRewardData[targetTier].S_RoomChest_Chance = this.FloorRewardData[0].S_RoomChest_Chance;
  }

  public void CopyTertiaryBossSpawnsFromTierZero(int targetTier)
  {
    this.FloorRewardData[targetTier].TertiaryBossRewardSets = new List<TertiaryBossRewardSet>((IEnumerable<TertiaryBossRewardSet>) this.FloorRewardData[0].TertiaryBossRewardSets);
  }

  public enum RewardSource
  {
    UNSPECIFIED,
    BOSS_PEDESTAL,
  }
}
