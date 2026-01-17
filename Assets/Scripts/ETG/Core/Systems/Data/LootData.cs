// Decompiled with JetBrains decompiler
// Type: LootData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Data
{
    [Serializable]
    public class LootData
    {
      public GenericLootTable lootTable;
      public List<GenericLootTable> overrideItemLootTables;
      [NonSerialized]
      public List<PickupObject.ItemQuality> overrideItemQualities;
      public float Common_Chance;
      public float D_Chance;
      public float C_Chance;
      public float B_Chance;
      public float A_Chance;
      public float S_Chance;
      public bool CompletesSynergy;
      public bool canDropMultipleItems;
      public bool onlyOneGunCanDrop = true;
      [ShowInInspectorIf("canDropMultipleItems", false)]
      public WeightedIntCollection multipleItemDropChances;
      [NonSerialized]
      public bool ForceNotCommon;
      [NonSerialized]
      public bool PreferGunDrop;
      [NonSerialized]
      public int LastGenerationNumSynergiesCalculated;

      private void ClearPerDropData()
      {
        this.PreferGunDrop = false;
        this.ForceNotCommon = false;
      }

      public PickupObject GetSingleItemForPlayer(PlayerController player, int tierShift = 0)
      {
        GameObject itemForPlayer = this.GetItemForPlayer(player, this.lootTable, (List<GameObject>) null, tierShift);
        this.ClearPerDropData();
        return (UnityEngine.Object) itemForPlayer != (UnityEngine.Object) null ? itemForPlayer.GetComponent<PickupObject>() : (PickupObject) null;
      }

      public List<PickupObject> GetItemsForPlayer(
        PlayerController player,
        int tierShift = 0,
        GenericLootTable OverrideDropTable = null,
        System.Random generatorRandom = null)
      {
        this.LastGenerationNumSynergiesCalculated = 0;
        List<GameObject> excludedObjects = new List<GameObject>();
        List<PickupObject> itemsForPlayer = new List<PickupObject>();
        int num = !this.canDropMultipleItems ? 1 : this.multipleItemDropChances.SelectByWeight(generatorRandom);
        bool excludeGuns = false;
        for (int index = 0; index < num; ++index)
        {
          GameObject itemForPlayer;
          if (num > 1 && this.overrideItemLootTables.Count > index && (UnityEngine.Object) this.overrideItemLootTables[index] != (UnityEngine.Object) null)
          {
            PickupObject.ItemQuality? overrideQuality = new PickupObject.ItemQuality?();
            if (this.overrideItemQualities != null && this.overrideItemQualities.Count > index)
              overrideQuality = new PickupObject.ItemQuality?(this.overrideItemQualities[index]);
            itemForPlayer = this.GetItemForPlayer(player, this.overrideItemLootTables[index], excludedObjects, tierShift, excludeGuns, overrideQuality, generatorRandom);
          }
          else
            itemForPlayer = this.GetItemForPlayer(player, this.lootTable, excludedObjects, tierShift, excludeGuns, generatorRandom: generatorRandom);
          if ((UnityEngine.Object) itemForPlayer != (UnityEngine.Object) null)
          {
            PickupObject component = itemForPlayer.GetComponent<PickupObject>();
            if (component is Gun && this.onlyOneGunCanDrop)
              excludeGuns = true;
            itemsForPlayer.Add(component);
            excludedObjects.Add(itemForPlayer);
          }
          this.ClearPerDropData();
        }
        if (GameStatsManager.Instance.GetFlag(GungeonFlags.ITEMSPECIFIC_AMMONOMICON_COMPLETE))
        {
          for (int index = 0; index < itemsForPlayer.Count; ++index)
          {
            if (itemsForPlayer[index].PickupObjectId == GlobalItemIds.UnfinishedGun)
              itemsForPlayer[index] = PickupObjectDatabase.GetById(GlobalItemIds.FinishedGun);
          }
        }
        return itemsForPlayer;
      }

      public GameObject GetItemForPlayer(
        PlayerController player,
        GenericLootTable tableToUse,
        List<GameObject> excludedObjects,
        int tierShift = 0,
        bool excludeGuns = false,
        PickupObject.ItemQuality? overrideQuality = null,
        System.Random generatorRandom = null)
      {
        PickupObject.ItemQuality targetQuality = (PickupObject.ItemQuality) Mathf.Min(5, Mathf.Max(0, (!overrideQuality.HasValue ? (int) this.GetTargetItemQuality(player, generatorRandom) : (int) overrideQuality.Value) + tierShift));
        bool flag1 = false;
        bool flag2 = GameStatsManager.HasInstance && GameStatsManager.Instance.IsRainbowRun;
        List<int> forceExcludedIds = GameManager.Instance.RainbowRunForceExcludedIDs;
        List<int> forceIncludedIds = GameManager.Instance.RainbowRunForceIncludedIDs;
        if (this.CompletesSynergy)
          SynercacheManager.UseCachedSynergyIDs = true;
        while (targetQuality >= PickupObject.ItemQuality.COMMON)
        {
          if (targetQuality > PickupObject.ItemQuality.COMMON)
            flag1 = true;
          List<WeightedGameObject> compiledRawItems = tableToUse.GetCompiledRawItems();
          List<KeyValuePair<WeightedGameObject, float>> keyValuePairList1 = new List<KeyValuePair<WeightedGameObject, float>>();
          float num1 = 0.0f;
          List<KeyValuePair<WeightedGameObject, float>> keyValuePairList2 = new List<KeyValuePair<WeightedGameObject, float>>();
          float num2 = 0.0f;
          for (int index1 = 0; index1 < compiledRawItems.Count; ++index1)
          {
            if ((UnityEngine.Object) compiledRawItems[index1].gameObject != (UnityEngine.Object) null)
            {
              PickupObject component1 = compiledRawItems[index1].gameObject.GetComponent<PickupObject>();
              bool flag3 = RewardManager.CheckQualityForItem(component1, player, targetQuality, this.CompletesSynergy, RewardManager.RewardSource.UNSPECIFIED);
              if ((component1.ItemSpansBaseQualityTiers || component1.ItemRespectsHeartMagnificence) && targetQuality != PickupObject.ItemQuality.D && targetQuality != PickupObject.ItemQuality.COMMON && targetQuality != PickupObject.ItemQuality.S)
                flag3 = true;
              if (component1 is SpiceItem && (UnityEngine.Object) player != (UnityEngine.Object) null && player.spiceCount > 0)
                flag3 = true;
              if ((UnityEngine.Object) component1 != (UnityEngine.Object) null && flag3)
              {
                bool flag4 = true;
                float weight = compiledRawItems[index1].weight;
                bool flag5;
                if (excludedObjects != null && excludedObjects.Contains(component1.gameObject))
                {
                  flag5 = false;
                }
                else
                {
                  if (flag2)
                  {
                    if (forceExcludedIds != null && forceExcludedIds.Contains(component1.PickupObjectId))
                    {
                      flag5 = false;
                      continue;
                    }
                    if ((targetQuality == PickupObject.ItemQuality.D || targetQuality == PickupObject.ItemQuality.C) && forceIncludedIds != null && !forceIncludedIds.Contains(component1.PickupObjectId))
                    {
                      flag5 = false;
                      continue;
                    }
                  }
                  if (component1 is Gun && excludeGuns)
                  {
                    flag5 = false;
                  }
                  else
                  {
                    if (!component1.PrerequisitesMet())
                      flag4 = false;
                    if (component1 is Gun)
                    {
                      Gun gun = component1 as Gun;
                      if (gun.InfiniteAmmo && !gun.CanBeDropped && gun.quality == PickupObject.ItemQuality.SPECIAL)
                      {
                        flag5 = false;
                        continue;
                      }
                      GunClass gunClass = gun.gunClass;
                      if (gunClass != GunClass.NONE)
                      {
                        int p = !((UnityEngine.Object) player == (UnityEngine.Object) null) ? player.inventory.ContainsGunOfClass(gunClass, true) : 0;
                        float modifierForClass = LootDataGlobalSettings.Instance.GetModifierForClass(gunClass);
                        weight *= Mathf.Pow(modifierForClass, (float) p);
                      }
                      if (this.PreferGunDrop)
                        weight *= 1000f;
                    }
                    float multiplierForItem = RewardManager.GetMultiplierForItem(component1, player, this.CompletesSynergy);
                    if (this.CompletesSynergy && (double) multiplierForItem > 100000.0)
                      ++this.LastGenerationNumSynergiesCalculated;
                    float num3 = weight * multiplierForItem;
                    if (RoomHandler.unassignedInteractableObjects != null)
                    {
                      for (int index2 = 0; index2 < RoomHandler.unassignedInteractableObjects.Count; ++index2)
                      {
                        IPlayerInteractable interactableObject = RoomHandler.unassignedInteractableObjects[index2];
                        if (interactableObject is PickupObject)
                        {
                          PickupObject pickupObject = interactableObject as PickupObject;
                          if ((bool) (UnityEngine.Object) pickupObject && pickupObject.PickupObjectId == component1.PickupObjectId)
                          {
                            flag4 = false;
                            num2 += num3;
                            KeyValuePair<WeightedGameObject, float> keyValuePair = new KeyValuePair<WeightedGameObject, float>(compiledRawItems[index1], num3);
                            keyValuePairList2.Add(keyValuePair);
                            break;
                          }
                        }
                      }
                    }
                    if (GameManager.Instance.IsSeeded)
                    {
                      if (GameManager.Instance.RewardManager.IsItemInSeededManifests(component1))
                      {
                        flag4 = false;
                        num2 += num3;
                        KeyValuePair<WeightedGameObject, float> keyValuePair = new KeyValuePair<WeightedGameObject, float>(compiledRawItems[index1], num3);
                        keyValuePairList2.Add(keyValuePair);
                      }
                    }
                    else
                    {
                      EncounterTrackable component2 = component1.GetComponent<EncounterTrackable>();
                      if ((UnityEngine.Object) component2 != (UnityEngine.Object) null)
                      {
                        int num4 = 0;
                        if (Application.isPlaying)
                          num4 = GameStatsManager.Instance.QueryEncounterableDifferentiator(component2);
                        if (this.CompletesSynergy)
                          num4 = 0;
                        if (num4 > 0 || Application.isPlaying && GameManager.Instance.ExtantShopTrackableGuids.Contains(component2.EncounterGuid))
                        {
                          flag4 = false;
                          num2 += num3;
                          KeyValuePair<WeightedGameObject, float> keyValuePair = new KeyValuePair<WeightedGameObject, float>(compiledRawItems[index1], num3);
                          keyValuePairList2.Add(keyValuePair);
                        }
                        else if (Application.isPlaying && GameStatsManager.Instance.QueryEncounterable(component2) == 0 && GameStatsManager.Instance.QueryEncounterableAnnouncement(component2.EncounterGuid))
                          num3 *= 10f;
                      }
                    }
                    if (component1.ItemSpansBaseQualityTiers || component1.ItemRespectsHeartMagnificence)
                    {
                      if ((double) RewardManager.AdditionalHeartTierMagnificence >= 3.0)
                        num3 *= GameManager.Instance.RewardManager.ThreeOrMoreHeartMagMultiplier;
                      else if ((double) RewardManager.AdditionalHeartTierMagnificence >= 1.0)
                        num3 *= GameManager.Instance.RewardManager.OneOrTwoHeartMagMultiplier;
                    }
                    if (flag4)
                    {
                      num1 += num3;
                      KeyValuePair<WeightedGameObject, float> keyValuePair = new KeyValuePair<WeightedGameObject, float>(compiledRawItems[index1], num3);
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
            float num5 = num1 * (generatorRandom == null ? UnityEngine.Random.value : (float) generatorRandom.NextDouble());
            for (int index = 0; index < keyValuePairList1.Count; ++index)
            {
              num5 -= keyValuePairList1[index].Value;
              if ((double) num5 <= 0.0)
              {
                Debug.Log((object) $"returning item {(!((UnityEngine.Object) keyValuePairList1[index].Key.gameObject != (UnityEngine.Object) null) ? "noll" : keyValuePairList1[index].Key.gameObject.name)} #{index.ToString()} of {(object) keyValuePairList1.Count}|{targetQuality.ToString()}");
                SynercacheManager.UseCachedSynergyIDs = false;
                return keyValuePairList1[index].Key.gameObject;
              }
            }
            Debug.Log((object) "returning last possible item");
            SynercacheManager.UseCachedSynergyIDs = false;
            return keyValuePairList1[keyValuePairList1.Count - 1].Key.gameObject;
          }
          --targetQuality;
          if (targetQuality < PickupObject.ItemQuality.COMMON && !flag1)
            targetQuality = PickupObject.ItemQuality.D;
        }
        SynercacheManager.UseCachedSynergyIDs = false;
        Debug.LogError((object) "Failed to get any item at all.");
        return (GameObject) null;
      }

      protected PickupObject.ItemQuality GetTargetItemTier(System.Random generatorRandom)
      {
        float num1 = !this.ForceNotCommon ? this.Common_Chance : 0.0f;
        float dChance = this.D_Chance;
        float cChance = this.C_Chance;
        float bChance = this.B_Chance;
        float aChance = this.A_Chance;
        float chance = this.S_Chance;
        float num2 = num1 + dChance + cChance + bChance + aChance + chance;
        if ((double) num2 == 0.0)
          return PickupObject.ItemQuality.D;
        float num3 = num2 * (generatorRandom == null ? UnityEngine.Random.value : (float) generatorRandom.NextDouble());
        float num4 = 0.0f + num1;
        if ((double) num4 > (double) num3)
          return PickupObject.ItemQuality.COMMON;
        float num5 = num4 + dChance;
        if ((double) num5 > (double) num3)
          return PickupObject.ItemQuality.D;
        float num6 = num5 + cChance;
        if ((double) num6 > (double) num3)
          return PickupObject.ItemQuality.C;
        float num7 = num6 + bChance;
        if ((double) num7 > (double) num3)
          return PickupObject.ItemQuality.B;
        float num8 = num7 + aChance;
        if ((double) num8 > (double) num3)
          return PickupObject.ItemQuality.A;
        return (double) (num8 + chance) > (double) num3 ? PickupObject.ItemQuality.S : PickupObject.ItemQuality.S;
      }

      protected PickupObject.ItemQuality GetTargetItemQuality(
        PlayerController player,
        System.Random generatorRandom)
      {
        return this.GetTargetItemTier(generatorRandom);
      }
    }

}
