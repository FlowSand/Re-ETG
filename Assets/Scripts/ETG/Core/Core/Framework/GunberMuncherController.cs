// Decompiled with JetBrains decompiler
// Type: GunberMuncherController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class GunberMuncherController : BraveBehaviour
    {
      public int RequiredNumberOfGuns = 2;
      public GenericLootTable LootTable;
      public List<GunberMuncherRecipe> DefinedRecipes;
      public AnimationCurve QualityDistribution;
      public bool IsProcessing;
      public bool CanBeReused;
      [PickupIdentifier]
      public int evilMuncherReward;
      public float evilMuncherPostRewardChance = 0.07f;
      public GameObject PoopSteamPrefab;
      [NonSerialized]
      private Gun m_first;
      [NonSerialized]
      private Gun m_second;
      [NonSerialized]
      private int m_gunsTossed;

      public bool ShouldGiveReward { get; set; }

      private void Start()
      {
        if (this.RequiredNumberOfGuns > 2)
          this.m_gunsTossed = (int) GameStatsManager.Instance.GetPlayerStatValue(TrackedStats.GUNBERS_EVIL_MUNCHED);
        Minimap.Instance.RegisterRoomIcon(this.transform.position.GetAbsoluteRoom(), (GameObject) ResourceCache.Acquire("Global Prefabs/Minimap_Muncher_Icon"));
      }

      [DebuggerHidden]
      public IEnumerator DoReward(PlayerController player)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new GunberMuncherController__DoRewardc__Iterator0()
        {
          player = player,
          _this = this
        };
      }

      private void DoSteamOnGrounded(DebrisObject obj)
      {
        SpawnManager.SpawnVFX(this.PoopSteamPrefab, obj.sprite.WorldCenter.ToVector3ZUp(obj.sprite.WorldCenter.y - 1f), Quaternion.identity);
      }

      protected GameObject GetRecipeItem()
      {
        PickupObject pickupObject = (PickupObject) null;
        for (int index = 0; index < this.DefinedRecipes.Count; ++index)
        {
          if (this.DefinedRecipes[index].gunIDs_A.Contains(this.m_first.PickupObjectId) && this.DefinedRecipes[index].gunIDs_B.Contains(this.m_second.PickupObjectId))
          {
            pickupObject = PickupObjectDatabase.GetById(this.DefinedRecipes[index].resultID);
            if ((UnityEngine.Object) pickupObject != (UnityEngine.Object) null && this.DefinedRecipes[index].flagToSet != GungeonFlags.NONE)
            {
              GameStatsManager.Instance.SetFlag(this.DefinedRecipes[index].flagToSet, true);
              break;
            }
            break;
          }
          if (this.DefinedRecipes[index].gunIDs_A.Contains(this.m_second.PickupObjectId) && this.DefinedRecipes[index].gunIDs_B.Contains(this.m_first.PickupObjectId))
          {
            pickupObject = PickupObjectDatabase.GetById(this.DefinedRecipes[index].resultID);
            if ((UnityEngine.Object) pickupObject != (UnityEngine.Object) null && this.DefinedRecipes[index].flagToSet != GungeonFlags.NONE)
            {
              GameStatsManager.Instance.SetFlag(this.DefinedRecipes[index].flagToSet, true);
              break;
            }
            break;
          }
        }
        return (UnityEngine.Object) pickupObject != (UnityEngine.Object) null ? pickupObject.gameObject : (GameObject) null;
      }

      protected GameObject GetItemForPlayer(PlayerController player)
      {
        if (this.RequiredNumberOfGuns > 2 && !GameStatsManager.Instance.GetFlag(GungeonFlags.MUNCHER_EVIL_COMPLETE))
          return PickupObjectDatabase.GetById(this.evilMuncherReward).gameObject;
        if ((UnityEngine.Object) this.m_first != (UnityEngine.Object) null && (UnityEngine.Object) this.m_second != (UnityEngine.Object) null)
        {
          GameObject recipeItem = this.GetRecipeItem();
          if ((UnityEngine.Object) recipeItem != (UnityEngine.Object) null)
            return recipeItem;
        }
        PickupObject.ItemQuality itemQuality = (PickupObject.ItemQuality) Mathf.Min(5, Mathf.Max(0, (int) this.DetermineQualityToSpawn()));
        bool flag1 = false;
        while (itemQuality >= PickupObject.ItemQuality.COMMON)
        {
          if (itemQuality > PickupObject.ItemQuality.COMMON)
            flag1 = true;
          List<WeightedGameObject> compiledRawItems = this.LootTable.GetCompiledRawItems();
          List<KeyValuePair<WeightedGameObject, float>> keyValuePairList1 = new List<KeyValuePair<WeightedGameObject, float>>();
          float num1 = 0.0f;
          List<KeyValuePair<WeightedGameObject, float>> keyValuePairList2 = new List<KeyValuePair<WeightedGameObject, float>>();
          float num2 = 0.0f;
          for (int index = 0; index < compiledRawItems.Count; ++index)
          {
            if ((UnityEngine.Object) compiledRawItems[index].gameObject != (UnityEngine.Object) null)
            {
              PickupObject component1 = compiledRawItems[index].gameObject.GetComponent<PickupObject>();
              bool flag2 = component1.quality == itemQuality;
              if ((UnityEngine.Object) component1 != (UnityEngine.Object) null && flag2)
              {
                bool flag3 = true;
                float weight = compiledRawItems[index].weight;
                if (!component1.PrerequisitesMet())
                  flag3 = false;
                if (!component1.CanActuallyBeDropped(player))
                  flag3 = false;
                EncounterTrackable component2 = component1.GetComponent<EncounterTrackable>();
                if ((UnityEngine.Object) component2 != (UnityEngine.Object) null)
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
          if (keyValuePairList1.Count == 0 && keyValuePairList2.Count > 0)
          {
            keyValuePairList1 = keyValuePairList2;
            num1 = num2;
          }
          if ((double) num1 > 0.0 && keyValuePairList1.Count > 0)
          {
            float num4 = num1 * UnityEngine.Random.value;
            for (int index = 0; index < keyValuePairList1.Count; ++index)
            {
              num4 -= keyValuePairList1[index].Value;
              if ((double) num4 <= 0.0)
                return keyValuePairList1[index].Key.gameObject;
            }
            return keyValuePairList1[keyValuePairList1.Count - 1].Key.gameObject;
          }
          --itemQuality;
          if (itemQuality < PickupObject.ItemQuality.COMMON && !flag1)
            itemQuality = PickupObject.ItemQuality.D;
        }
        UnityEngine.Debug.LogError((object) "Failed to get any item at all.");
        return (GameObject) null;
      }

      private PickupObject.ItemQuality DetermineQualityToSpawn()
      {
        if ((UnityEngine.Object) this.m_first == (UnityEngine.Object) null && this.m_gunsTossed >= this.RequiredNumberOfGuns)
          return (double) UnityEngine.Random.value > 0.25 ? PickupObject.ItemQuality.A : PickupObject.ItemQuality.S;
        if ((UnityEngine.Object) this.m_first == (UnityEngine.Object) null || (UnityEngine.Object) this.m_second == (UnityEngine.Object) null)
        {
          UnityEngine.Debug.LogError((object) "Problem of type 2 in Gunber Muncher!");
          if ((UnityEngine.Object) this.m_first != (UnityEngine.Object) null)
            return this.m_first.quality;
          return (UnityEngine.Object) this.m_second != (UnityEngine.Object) null ? this.m_second.quality : PickupObject.ItemQuality.C;
        }
        int quality1 = (int) this.m_first.quality;
        int quality2 = (int) this.m_second.quality;
        int num1 = Mathf.Min(quality1, quality2);
        int num2 = Mathf.Max(quality1, quality2);
        bool flag = num2 < 5;
        float num3 = 1f / (float) (num2 - num1 + 1);
        float num4 = UnityEngine.Random.value;
        float time = 0.0f;
        int qualityToSpawn = -1;
        for (int index = num1; index <= num2; ++index)
        {
          time += num3;
          if ((double) this.QualityDistribution.Evaluate(time) > (double) num4)
          {
            qualityToSpawn = index;
            break;
          }
        }
        if (qualityToSpawn == -1)
          qualityToSpawn = num2;
        if (flag && (double) UnityEngine.Random.value > 0.949999988079071)
          qualityToSpawn = Mathf.Min(qualityToSpawn + 1, 5);
        return (PickupObject.ItemQuality) qualityToSpawn;
      }

      public void TossPlayerEquippedGun(PlayerController player)
      {
        if (!((UnityEngine.Object) player.CurrentGun != (UnityEngine.Object) null) || !player.CurrentGun.CanActuallyBeDropped(player))
          return;
        Gun currentGun = player.CurrentGun;
        if (this.RequiredNumberOfGuns == 2)
        {
          if ((UnityEngine.Object) this.m_first == (UnityEngine.Object) null)
            this.m_first = PickupObjectDatabase.Instance.InternalGetById(currentGun.PickupObjectId) as Gun;
          else if ((UnityEngine.Object) this.m_second == (UnityEngine.Object) null)
            this.m_second = PickupObjectDatabase.Instance.InternalGetById(currentGun.PickupObjectId) as Gun;
          else
            UnityEngine.Debug.LogError((object) "GUNBER MUNCHER FAIL TYPE 1");
        }
        else
        {
          GameStatsManager.Instance.RegisterStatChange(TrackedStats.GUNBERS_EVIL_MUNCHED, 1f);
          ++this.m_gunsTossed;
        }
        this.TossObjectIntoPot(player, currentGun.GetSprite(), (Vector3) player.CenterPosition);
        player.inventory.DestroyCurrentGun();
      }

      public void TossObjectIntoPot(
        PlayerController player,
        tk2dBaseSprite spriteSource,
        Vector3 startPosition)
      {
        this.StartCoroutine(this.HandleObjectPotToss(player, spriteSource, startPosition));
      }

      [DebuggerHidden]
      private IEnumerator HandleObjectPotToss(
        PlayerController player,
        tk2dBaseSprite spriteSource,
        Vector3 startPosition)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new GunberMuncherController__HandleObjectPotTossc__Iterator1()
        {
          spriteSource = spriteSource,
          startPosition = startPosition,
          player = player,
          _this = this
        };
      }
    }

}
