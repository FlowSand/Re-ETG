// Decompiled with JetBrains decompiler
// Type: AdvancedShrineController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Dungeon.Interactables
{
    public class AdvancedShrineController : 
      DungeonPlaceableBehaviour,
      IPlayerInteractable,
      IPlaceConfigurable
    {
      public string displayTextKey;
      public string acceptOptionKey;
      public string declineOptionKey;
      public string spentOptionKey = "#SHRINE_GENERIC_SPENT";
      public bool IsBlankShrine;
      public bool IsRNGShrine;
      public bool IsHealthArmorSwapShrine;
      public bool IsJunkShrine;
      public bool IsBloodShrine;
      public bool IsGlassShrine;
      public List<ShrineCost> Costs;
      public List<ShrineBenefit> Benefits;
      public bool CanBeReused;
      public bool IsCleanseShrine;
      public bool IsLegendaryHeroShrine;
      public bool IncrementMoneyCostEachUse;
      public int IncrementMoneyCostAmount = 10;
      public bool ShattersOnUse;
      public GameObject ShatterSystem;
      public tk2dSprite ShatterSpriteDisable;
      public tk2dBaseSprite AlternativeOutlineTarget;
      public Transform talkPoint;
      public GameObject onPlayerVFX;
      public Vector3 playerVFXOffset;
      public tk2dBaseSprite EncounterNotificationSprite;
      private RoomHandler m_parentRoom;
      private GameObject m_instanceMinimapIcon;
      private int m_useCount;
      private int m_totalUseCount;
      private const float ChanceToGoApeshit = 0.001f;
      private float m_curChanceToBlankChestIntoExistence = 0.9f;

      public void ConfigureOnPlacement(RoomHandler room)
      {
        this.m_parentRoom = room;
        if (!this.IsLegendaryHeroShrine)
        {
          room.OptionalDoorTopDecorable = ResourceCache.Acquire("Global Prefabs/Shrine_Lantern") as GameObject;
          if (!room.IsOnCriticalPath && room.connectedRooms.Count == 1)
          {
            room.ShouldAttemptProceduralLock = true;
            room.AttemptProceduralLockChance = Mathf.Max(room.AttemptProceduralLockChance, Random.Range(0.3f, 0.5f));
          }
        }
        this.RegisterMinimapIcon();
      }

      public void Start()
      {
        if ((bool) (Object) this.specRigidbody)
          this.specRigidbody.PreventPiercing = true;
        if (StaticReferenceManager.AllAdvancedShrineControllers.Contains(this))
          return;
        StaticReferenceManager.AllAdvancedShrineControllers.Add(this);
      }

      public void RegisterMinimapIcon()
      {
        this.m_instanceMinimapIcon = Minimap.Instance.RegisterRoomIcon(this.m_parentRoom, (GameObject) BraveResources.Load("Global Prefabs/Minimap_Shrine_Icon"));
      }

      public void GetRidOfMinimapIcon()
      {
        if (!((Object) this.m_instanceMinimapIcon != (Object) null))
          return;
        Minimap.Instance.DeregisterRoomIcon(this.m_parentRoom, this.m_instanceMinimapIcon);
        this.m_instanceMinimapIcon = (GameObject) null;
      }

      private bool CheckCosts(PlayerController interactor)
      {
        bool flag = true;
        for (int index = 0; index < this.Costs.Count; ++index)
        {
          if (!this.Costs[index].CheckCost(interactor))
          {
            flag = false;
            break;
          }
        }
        return flag;
      }

      private bool CheckAndApplyCosts(PlayerController interactor)
      {
        if (!this.CheckCosts(interactor))
          return false;
        for (int index = 0; index < this.Costs.Count; ++index)
          this.Costs[index].ApplyCost(interactor);
        return true;
      }

      private void ResetForReuse() => --this.m_useCount;

      private ShrineCost GetRandomCost()
      {
        float num1 = 0.0f;
        for (int index = 0; index < this.Costs.Count; ++index)
          num1 += this.Costs[index].rngWeight;
        float num2 = Random.value * num1;
        float num3 = 0.0f;
        for (int index = 0; index < this.Costs.Count; ++index)
        {
          num3 += this.Costs[index].rngWeight;
          if ((double) num3 >= (double) num2)
            return this.Costs[index];
        }
        return this.Costs[this.Costs.Count - 1];
      }

      private ShrineBenefit GetRandomBenefit()
      {
        float num1 = 0.0f;
        for (int index = 0; index < this.Benefits.Count; ++index)
          num1 += this.Benefits[index].rngWeight;
        float num2 = Random.value * num1;
        float num3 = 0.0f;
        for (int index = 0; index < this.Benefits.Count; ++index)
        {
          num3 += this.Benefits[index].rngWeight;
          if ((double) num3 >= (double) num2)
            return this.Benefits[index];
        }
        return this.Benefits[this.Benefits.Count - 1];
      }

      private void DoShrineEffect(PlayerController player)
      {
        if (this.IsJunkShrine)
          GameStatsManager.Instance.SetFlag(GungeonFlags.ITEMSPECIFIC_SER_JUNKAN_UNLOCKED, true);
        if (this.IsGlassShrine)
        {
          GameStatsManager.Instance.SetFlag(GungeonFlags.ITEMSPECIFIC_GLASS_SHRINE, true);
          int num1 = (int) AkSoundEngine.PostEvent("Play_OBJ_mirror_shatter_01", this.gameObject);
          int num2 = (int) AkSoundEngine.PostEvent("Play_OBJ_crystal_shatter_01", this.gameObject);
        }
        if (this.IsBloodShrine)
        {
          GameStatsManager.Instance.RegisterStatChange(TrackedStats.TIMES_BLOOD_SHRINED, 1f);
          if ((double) GameStatsManager.Instance.GetPlayerStatValue(TrackedStats.TIMES_BLOOD_SHRINED) >= 2.0)
            GameStatsManager.Instance.SetFlag(GungeonFlags.ITEMSPECIFIC_LIFE_ORB, true);
        }
        if (this.IsHealthArmorSwapShrine)
        {
          int num = (int) AkSoundEngine.PostEvent("Play_OBJ_shrine_accept_01", this.gameObject);
          player.HealthAndArmorSwapped = !player.HealthAndArmorSwapped;
          if ((Object) this.onPlayerVFX != (Object) null)
            player.PlayEffectOnActor(this.onPlayerVFX, this.playerVFXOffset);
          if ((Object) this.transform.parent != (Object) null)
          {
            EncounterTrackable component = this.transform.parent.gameObject.GetComponent<EncounterTrackable>();
            if ((Object) component != (Object) null)
            {
              if ((Object) this.m_instanceMinimapIcon == (Object) null && (Object) this.EncounterNotificationSprite == (Object) null)
                this.RegisterMinimapIcon();
              component.ForceDoNotification(this.EncounterNotificationSprite ?? this.m_instanceMinimapIcon.GetComponent<tk2dBaseSprite>());
            }
          }
        }
        else if (this.IsRNGShrine)
        {
          if ((double) Random.value < 1.0 / 1000.0)
          {
            player.healthHaver.TriggerInvulnerabilityPeriod();
            player.knockbackDoer.ApplyKnockback(player.CenterPosition - this.specRigidbody.UnitCenter, 150f);
            Exploder.DoDefaultExplosion((Vector3) this.specRigidbody.UnitCenter, Vector2.zero);
            StatModifier statModifier1 = new StatModifier();
            statModifier1.statToBoost = PlayerStats.StatType.Health;
            statModifier1.modifyType = StatModifier.ModifyMethod.ADDITIVE;
            statModifier1.amount = Mathf.Min(0.0f, (float) (-1.0 * ((double) Mathf.Ceil(player.healthHaver.GetMaxHealth()) - 1.0)));
            StatModifier statModifier2 = new StatModifier();
            statModifier2.statToBoost = PlayerStats.StatType.Damage;
            statModifier2.modifyType = StatModifier.ModifyMethod.MULTIPLICATIVE;
            statModifier2.amount = 4f;
            StatModifier statModifier3 = new StatModifier()
            {
              statToBoost = PlayerStats.StatType.Curse,
              modifyType = StatModifier.ModifyMethod.ADDITIVE,
              amount = 10f
            };
            player.ownerlessStatModifiers.Add(statModifier1);
            player.ownerlessStatModifiers.Add(statModifier2);
            player.stats.RecalculateStats(player);
            Object.Destroy((Object) this.gameObject);
          }
          else
          {
            int num = (int) AkSoundEngine.PostEvent("Play_OBJ_shrine_accept_01", this.gameObject);
            ShrineCost randomCost = this.GetRandomCost();
            ShrineBenefit randomBenefit = this.GetRandomBenefit();
            if (randomCost.costType == ShrineCost.CostType.HEALTH)
            {
              randomCost.cost = Random.Range(1, 3);
              if ((double) randomCost.cost >= (double) player.healthHaver.GetCurrentHealth())
                randomCost.cost = 1;
            }
            if (randomCost.costType == ShrineCost.CostType.STATS && (double) player.healthHaver.GetMaxHealth() > 2.0)
              randomCost.cost = Random.Range(1, 3);
            if (randomCost.costType == ShrineCost.CostType.BLANK)
              randomCost.cost = Random.Range(1, player.Blanks + 1);
            if (randomCost.costType == ShrineCost.CostType.MONEY)
              randomCost.cost = Mathf.FloorToInt((float) player.carriedConsumables.Currency * Random.Range(0.25f, 1f));
            if (randomBenefit.benefitType == ShrineBenefit.BenefitType.MONEY)
              randomBenefit.amount = (float) Random.Range(20, 100);
            if (randomBenefit.benefitType == ShrineBenefit.BenefitType.HEALTH)
              randomBenefit.amount = (float) Mathf.RoundToInt(Random.Range(1f, player.healthHaver.GetMaxHealth()));
            if (randomBenefit.benefitType == ShrineBenefit.BenefitType.STATS)
            {
              if (randomBenefit.statMods[0].statToBoost == PlayerStats.StatType.Health)
                randomBenefit.statMods[0].amount = (float) Random.Range(1, 3);
              else if (randomBenefit.statMods[0].statToBoost == PlayerStats.StatType.MovementSpeed)
                randomBenefit.statMods[0].amount = Random.Range(1.5f, 4f);
              else if (randomBenefit.statMods[0].statToBoost == PlayerStats.StatType.Damage)
                randomBenefit.statMods[0].amount = Random.Range(1.2f, 1.5f);
            }
            if (randomBenefit.benefitType == ShrineBenefit.BenefitType.BLANK)
              randomBenefit.amount = (float) Random.Range(1, 11);
            if (randomBenefit.benefitType == ShrineBenefit.BenefitType.ARMOR)
              randomBenefit.amount = (float) Random.Range(1, 4);
            if (randomBenefit.benefitType == ShrineBenefit.BenefitType.SPAWN_CHEST)
              randomBenefit.IsRNGChest = true;
            string empty = string.Empty;
            string str1;
            if (randomCost.CheckCost(player))
            {
              randomCost.ApplyCost(player);
              str1 = empty + StringTableManager.GetItemsString(randomCost.rngString);
            }
            else
              str1 = empty + StringTableManager.GetItemsString("#SHRINE_DICE_BAD_FAIL");
            string str2 = str1 + " + ";
            randomBenefit.ApplyBenefit(player);
            string description = str2 + StringTableManager.GetItemsString(randomBenefit.rngString);
            if ((Object) this.m_instanceMinimapIcon == (Object) null)
              this.RegisterMinimapIcon();
            if ((Object) this.EncounterNotificationSprite != (Object) null)
              GameUIRoot.Instance.notificationController.DoCustomNotification(StringTableManager.GetItemsString("#SHRINE_DICE_ENCNAME"), description, this.EncounterNotificationSprite.Collection, this.EncounterNotificationSprite.spriteId);
            else
              GameUIRoot.Instance.notificationController.DoCustomNotification(StringTableManager.GetItemsString("#SHRINE_DICE_ENCNAME"), description, this.m_instanceMinimapIcon.GetComponent<tk2dBaseSprite>().Collection, this.m_instanceMinimapIcon.GetComponent<tk2dBaseSprite>().spriteId);
          }
          GameStatsManager.Instance.RegisterStatChange(TrackedStats.DICE_SHRINES_USED, 1f);
          if ((double) GameStatsManager.Instance.GetPlayerStatValue(TrackedStats.DICE_SHRINES_USED) >= 2.0 && (double) GameStatsManager.Instance.GetPlayerStatValue(TrackedStats.TIMES_REACHED_FORGE) >= 1.0)
            GameStatsManager.Instance.SetFlag(GungeonFlags.DAISUKE_IS_UNLOCKABLE, true);
        }
        else if (this.IsLegendaryHeroShrine)
        {
          int totalCurse = PlayerStats.GetTotalCurse();
          int num = this.m_useCount <= 0 ? 5 : 9;
          if (totalCurse >= 5)
            num = 9;
          UnityEngine.Debug.LogError((object) $"total curse: {(object) totalCurse}|{(object) num}|{(object) this.m_useCount}");
          if (totalCurse < num)
          {
            player.ownerlessStatModifiers.Add(new StatModifier()
            {
              statToBoost = PlayerStats.StatType.Curse,
              amount = (float) (num - totalCurse),
              modifyType = StatModifier.ModifyMethod.ADDITIVE
            });
            player.stats.RecalculateStats(player);
          }
        }
        else
        {
          if (!this.CheckAndApplyCosts(player))
          {
            this.ResetForReuse();
            return;
          }
          int num = (int) AkSoundEngine.PostEvent("Play_OBJ_shrine_accept_01", this.gameObject);
          for (int index = 0; index < this.Benefits.Count; ++index)
            this.Benefits[index].ApplyBenefit(player);
          if (this.IncrementMoneyCostEachUse)
          {
            for (int index = 0; index < this.Costs.Count; ++index)
            {
              if (this.Costs[index].costType == ShrineCost.CostType.MONEY)
                this.Costs[index].cost += this.IncrementMoneyCostAmount;
            }
          }
          if ((Object) this.onPlayerVFX != (Object) null)
            player.PlayEffectOnActor(this.onPlayerVFX, this.playerVFXOffset);
          if ((Object) this.transform.parent != (Object) null)
          {
            EncounterTrackable component = this.transform.parent.gameObject.GetComponent<EncounterTrackable>();
            if ((Object) component != (Object) null)
            {
              if ((Object) this.m_instanceMinimapIcon == (Object) null && (Object) this.EncounterNotificationSprite == (Object) null)
                this.RegisterMinimapIcon();
              component.ForceDoNotification(this.EncounterNotificationSprite ?? this.m_instanceMinimapIcon.GetComponent<tk2dBaseSprite>());
            }
          }
        }
        if (!this.CanBeReused)
          this.GetRidOfMinimapIcon();
        if (!this.ShattersOnUse)
          return;
        this.ShatterSpriteDisable.renderer.enabled = false;
        this.ShatterSystem.SetActive(true);
        this.ShatterSystem.GetComponent<ParticleSystem>().Play();
      }

      public float GetDistanceToPoint(Vector2 point)
      {
        if ((Object) this.sprite == (Object) null)
          return 100f;
        Vector3 b = (Vector3) BraveMathCollege.ClosestPointOnRectangle(point, this.specRigidbody.UnitBottomLeft, this.specRigidbody.UnitDimensions);
        return this.IsLegendaryHeroShrine && (double) point.y > (double) b.y + 0.5 ? 1000f : Vector2.Distance(point, (Vector2) b) / 1.5f;
      }

      public float GetOverrideMaxDistance() => -1f;

      public void OnEnteredRange(PlayerController interactor)
      {
        if ((Object) this.AlternativeOutlineTarget != (Object) null)
          SpriteOutlineManager.AddOutlineToSprite(this.AlternativeOutlineTarget, Color.white);
        else
          SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.white);
      }

      public void OnExitRange(PlayerController interactor)
      {
        if ((Object) this.AlternativeOutlineTarget != (Object) null)
          SpriteOutlineManager.RemoveOutlineFromSprite(this.AlternativeOutlineTarget);
        else
          SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite);
      }

      [DebuggerHidden]
      private IEnumerator HandleShrineConversation(PlayerController interactor)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new AdvancedShrineController__HandleShrineConversationc__Iterator0()
        {
          interactor = interactor,
          _this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator HandleSpentText(PlayerController interactor)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new AdvancedShrineController__HandleSpentTextc__Iterator1()
        {
          interactor = interactor,
          _this = this
        };
      }

      public void Interact(PlayerController interactor)
      {
        if (TextBoxManager.HasTextBox(this.talkPoint))
          return;
        if (this.m_useCount > 0 || this.IsBlankShrine)
        {
          if (string.IsNullOrEmpty(this.spentOptionKey))
            return;
          this.StartCoroutine(this.HandleSpentText(interactor));
        }
        else
        {
          ++this.m_useCount;
          this.StartCoroutine(this.HandleShrineConversation(interactor));
        }
      }

      public void OnBlank()
      {
        if (!this.IsBlankShrine || (double) Random.value >= (double) this.m_curChanceToBlankChestIntoExistence)
          return;
        ++this.m_useCount;
        this.m_curChanceToBlankChestIntoExistence = Mathf.Max(0.25f, this.m_curChanceToBlankChestIntoExistence - 0.45f);
        RoomHandler absoluteRoom = this.transform.position.GetAbsoluteRoom();
        IntVector2? randomAvailableCell = absoluteRoom.GetRandomAvailableCell(new IntVector2?(IntVector2.One * 4), new CellTypes?(CellTypes.FLOOR));
        IntVector2? nullable = !randomAvailableCell.HasValue ? new IntVector2?() : new IntVector2?(randomAvailableCell.GetValueOrDefault() + IntVector2.One);
        if (nullable.HasValue)
          GameManager.Instance.RewardManager.SpawnRoomClearChestAt(nullable.Value);
        else
          GameManager.Instance.RewardManager.SpawnRoomClearChestAt(absoluteRoom.GetBestRewardLocation(new IntVector2(3, 3), RoomHandler.RewardLocationStyle.Original) + IntVector2.Up);
      }

      public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
      {
        shouldBeFlipped = false;
        return string.Empty;
      }

      protected override void OnDestroy()
      {
        StaticReferenceManager.AllAdvancedShrineControllers.Remove(this);
        base.OnDestroy();
      }
    }

}
