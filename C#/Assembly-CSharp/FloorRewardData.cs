// Decompiled with JetBrains decompiler
// Type: FloorRewardData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
[Serializable]
public class FloorRewardData
{
  public string Annotation;
  [EnumFlags]
  public GlobalDungeonData.ValidTilesets AssociatedTilesets;
  [Header("Currency Drops")]
  public float AverageCurrencyDropsThisFloor = 60f;
  public float CurrencyDropsStandardDeviation = 15f;
  public float MinimumCurrencyDropsThisFloor = 40f;
  [RewardManagerReset("Chest Type Chances", "Copy From Tier 0", "CopyChestChancesFromTierZero", 0)]
  public float D_Chest_Chance = 0.2f;
  public float C_Chest_Chance = 0.2f;
  public float B_Chest_Chance = 0.2f;
  public float A_Chest_Chance = 0.2f;
  public float S_Chest_Chance = 0.2f;
  [Header("Global Drops")]
  public float ChestSystem_ChestChanceLowerBound = 0.01f;
  public float ChestSystem_ChestChanceUpperBound = 0.2f;
  public float ChestSystem_Increment = 0.03f;
  [Space(3f)]
  public float GunVersusItemPercentChance = 0.5f;
  [Space(3f)]
  public float PercentOfRoomClearRewardsThatAreChests = 0.2f;
  public GenericLootTable SingleItemRewardTable;
  [Space(3f)]
  public float FloorChanceToDropAmmo = 1f / 16f;
  public float FloorChanceForSpreadAmmo = 0.5f;
  [RewardManagerReset("Global Drop Type Chances", "Copy From Tier 0", "CopyDropChestChancesFromTierZero", 2)]
  public float D_RoomChest_Chance = 0.2f;
  public float C_RoomChest_Chance = 0.2f;
  public float B_RoomChest_Chance = 0.2f;
  public float A_RoomChest_Chance = 0.2f;
  public float S_RoomChest_Chance = 0.2f;
  [RewardManagerReset("Boss Gun Qualities", "Copy From Tier 0", "CopyBossGunChancesFromTierZero", 0)]
  public float D_BossGun_Chance = 0.1f;
  public float C_BossGun_Chance = 0.3f;
  public float B_BossGun_Chance = 0.3f;
  public float A_BossGun_Chance = 0.2f;
  public float S_BossGun_Chance = 0.1f;
  [RewardManagerReset("Shop Gun/Item Qualities", "Copy From Tier 0", "CopyShopChancesFromTierZero", 0)]
  public float D_Shop_Chance = 0.1f;
  public float C_Shop_Chance = 0.3f;
  public float B_Shop_Chance = 0.3f;
  public float A_Shop_Chance = 0.2f;
  public float S_Shop_Chance = 0.1f;
  public float ReplaceFirstRewardWithPickup = 0.2f;
  [Header("Meta Currency")]
  public int MinMetaCurrencyFromBoss;
  public int MaxMetaCurrencyFromBoss;
  public bool AlternateItemChestChances;
  [ShowInInspectorIf("AlternateItemChestChances", false)]
  public float D_Item_Chest_Chance = 0.2f;
  [ShowInInspectorIf("AlternateItemChestChances", false)]
  public float C_Item_Chest_Chance = 0.2f;
  [ShowInInspectorIf("AlternateItemChestChances", false)]
  public float B_Item_Chest_Chance = 0.2f;
  [ShowInInspectorIf("AlternateItemChestChances", false)]
  public float A_Item_Chest_Chance = 0.2f;
  [ShowInInspectorIf("AlternateItemChestChances", false)]
  public float S_Item_Chest_Chance = 0.2f;
  [RewardManagerReset("For Bosses", "Copy From Tier 0", "CopyTertiaryBossSpawnsFromTierZero", 1)]
  public GenericLootTable FallbackBossLootTable;
  public List<TertiaryBossRewardSet> TertiaryBossRewardSets;

  public float SumChances()
  {
    return this.D_Chest_Chance + this.C_Chest_Chance + this.B_Chest_Chance + this.A_Chest_Chance + this.S_Chest_Chance;
  }

  public float SumRoomChances()
  {
    return this.D_RoomChest_Chance + this.C_RoomChest_Chance + this.B_RoomChest_Chance + this.A_RoomChest_Chance + this.S_RoomChest_Chance;
  }

  public float SumBossGunChances()
  {
    return this.D_BossGun_Chance + this.C_BossGun_Chance + this.B_BossGun_Chance + this.A_BossGun_Chance + this.S_BossGun_Chance;
  }

  public float SumShopChances()
  {
    return this.D_Shop_Chance + this.C_Shop_Chance + this.B_Shop_Chance + this.A_Shop_Chance + this.S_Shop_Chance;
  }

  public float DetermineCurrentMagnificence(bool isGenerationForMagnificence = false)
  {
    float currentMagnificence = 0.0f;
    if ((UnityEngine.Object) GameManager.Instance.PrimaryPlayer != (UnityEngine.Object) null)
      currentMagnificence += GameManager.Instance.PrimaryPlayer.stats.Magnificence;
    if ((UnityEngine.Object) GameManager.Instance.Dungeon != (UnityEngine.Object) null)
    {
      if (isGenerationForMagnificence)
        currentMagnificence += GameManager.Instance.Dungeon.GeneratedMagnificence * 2f;
      else
        currentMagnificence += GameManager.Instance.Dungeon.GeneratedMagnificence;
    }
    return currentMagnificence;
  }

  public PickupObject.ItemQuality GetTargetQualityFromChances(
    float fran,
    float dChance,
    float cChance,
    float bChance,
    float aChance,
    float sChance,
    bool isGenerationForMagnificence = false)
  {
    float currentMagnificence = this.DetermineCurrentMagnificence(isGenerationForMagnificence);
    if ((double) fran < (double) dChance)
      return MagnificenceConstants.ModifyQualityByMagnificence(PickupObject.ItemQuality.D, currentMagnificence, dChance, cChance, bChance);
    if ((double) fran < (double) dChance + (double) cChance)
      return MagnificenceConstants.ModifyQualityByMagnificence(PickupObject.ItemQuality.C, currentMagnificence, dChance, cChance, bChance);
    if ((double) fran < (double) dChance + (double) cChance + (double) bChance)
      return MagnificenceConstants.ModifyQualityByMagnificence(PickupObject.ItemQuality.B, currentMagnificence, dChance, cChance, bChance);
    return (double) fran < (double) dChance + (double) cChance + (double) bChance + (double) aChance ? MagnificenceConstants.ModifyQualityByMagnificence(PickupObject.ItemQuality.A, currentMagnificence, dChance, cChance, bChance) : MagnificenceConstants.ModifyQualityByMagnificence(PickupObject.ItemQuality.S, currentMagnificence, dChance, cChance, bChance);
  }

  public PickupObject.ItemQuality GetShopTargetQuality(bool useSeedRandom = false)
  {
    float num = this.SumShopChances();
    return this.GetTargetQualityFromChances((!useSeedRandom ? UnityEngine.Random.value : BraveRandom.GenerationRandomValue()) * num, this.D_Shop_Chance, this.C_Shop_Chance, this.B_Shop_Chance, this.A_Shop_Chance, this.S_Shop_Chance);
  }

  public PickupObject.ItemQuality GetRandomBossTargetQuality(System.Random safeRandom = null)
  {
    float num = this.SumBossGunChances();
    PickupObject.ItemQuality qualityFromChances = this.GetTargetQualityFromChances((safeRandom == null ? UnityEngine.Random.value : (float) safeRandom.NextDouble()) * num, this.D_BossGun_Chance, this.C_BossGun_Chance, this.B_BossGun_Chance, this.A_BossGun_Chance, this.S_BossGun_Chance);
    Debug.Log((object) (qualityFromChances.ToString() + " <= boss quality"));
    return qualityFromChances;
  }

  public PickupObject.ItemQuality GetRandomTargetQuality(
    bool isGenerationForMagnificence = false,
    bool forceDChanceZero = false)
  {
    float num = !forceDChanceZero ? this.SumChances() : this.C_Chest_Chance + this.B_Chest_Chance + this.A_Chest_Chance + this.S_Chest_Chance;
    return this.GetTargetQualityFromChances(!isGenerationForMagnificence ? UnityEngine.Random.value * num : BraveRandom.GenerationRandomValue() * num, !forceDChanceZero ? this.D_Chest_Chance : 0.0f, this.C_Chest_Chance, this.B_Chest_Chance, this.A_Chest_Chance, this.S_Chest_Chance, isGenerationForMagnificence);
  }

  public PickupObject.ItemQuality GetRandomRoomTargetQuality()
  {
    float fran = UnityEngine.Random.value * this.SumRoomChances();
    float dRoomChestChance = this.D_RoomChest_Chance;
    float cRoomChestChance = this.C_RoomChest_Chance;
    float bRoomChestChance = this.B_RoomChest_Chance;
    if (PassiveItem.IsFlagSetAtAll(typeof (AmazingChestAheadItem)))
    {
      float num = dRoomChestChance / 2f;
      dRoomChestChance -= num;
      cRoomChestChance += num / 2f;
      bRoomChestChance += num / 2f;
    }
    return this.GetTargetQualityFromChances(fran, dRoomChestChance, cRoomChestChance, bRoomChestChance, this.A_RoomChest_Chance, this.S_RoomChest_Chance);
  }
}
