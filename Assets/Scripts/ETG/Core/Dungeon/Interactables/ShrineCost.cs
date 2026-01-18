using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

[Serializable]
public class ShrineCost
  {
    public ShrineCost.CostType costType;
    public int cost;
    public bool AllowsArmorConversionForRobot;
    public StatModifier[] statMods;
    public string rngString;
    public float rngWeight = 1f;
    [PickupIdentifier]
    public int targetItemID;

    public bool CheckCost(PlayerController interactor)
    {
      switch (this.costType)
      {
        case ShrineCost.CostType.MONEY:
          return interactor.carriedConsumables.Currency >= this.cost;
        case ShrineCost.CostType.HEALTH:
          return this.AllowsArmorConversionForRobot && interactor.characterIdentity == PlayableCharacters.Robot ? (double) interactor.healthHaver.Armor > (double) (this.cost * 2) : (double) interactor.healthHaver.GetCurrentHealth() > (double) this.cost;
        case ShrineCost.CostType.ARMOR:
          return (double) interactor.healthHaver.Armor >= (double) this.cost;
        case ShrineCost.CostType.BLANK:
          return interactor.Blanks >= this.cost;
        case ShrineCost.CostType.KEY:
          return interactor.carriedConsumables.InfiniteKeys || interactor.carriedConsumables.KeyBullets >= this.cost;
        case ShrineCost.CostType.CURRENT_GUN:
          return (UnityEngine.Object) interactor.CurrentGun != (UnityEngine.Object) null && interactor.CurrentGun.CanActuallyBeDropped(interactor) && !interactor.CurrentGun.InfiniteAmmo;
        case ShrineCost.CostType.BEATEN_GAME:
          return !GameStatsManager.Instance.HasPast(GameManager.Instance.PrimaryPlayer.characterIdentity) || GameStatsManager.Instance.GetCharacterSpecificFlag(GameManager.Instance.PrimaryPlayer.characterIdentity, CharacterSpecificGungeonFlags.KILLED_PAST);
        case ShrineCost.CostType.STATS:
          return interactor.characterIdentity == PlayableCharacters.Robot && this.AllowsArmorConversionForRobot && this.statMods[0].statToBoost == PlayerStats.StatType.Health && (double) this.statMods[0].amount * -2.0 < (double) interactor.healthHaver.Armor || this.statMods[0].statToBoost != PlayerStats.StatType.Health || (double) this.statMods[0].amount * -1.0 < (double) interactor.healthHaver.GetMaxHealth();
        case ShrineCost.CostType.MONEY_PER_CURSE:
          return interactor.carriedConsumables.Currency >= this.cost * PlayerStats.GetTotalCurse();
        case ShrineCost.CostType.SPECIFIC_ITEM:
          bool flag = false;
          for (int index = 0; index < interactor.passiveItems.Count; ++index)
          {
            if (interactor.passiveItems[index].PickupObjectId == this.targetItemID)
              flag = true;
          }
          return flag;
        default:
          return false;
      }
    }

    public void ApplyCost(PlayerController interactor)
    {
      switch (this.costType)
      {
        case ShrineCost.CostType.MONEY:
          interactor.carriedConsumables.Currency -= this.cost;
          break;
        case ShrineCost.CostType.HEALTH:
          if (this.AllowsArmorConversionForRobot && interactor.characterIdentity == PlayableCharacters.Robot)
          {
            interactor.healthHaver.Armor -= (float) (this.cost * 2);
            break;
          }
          interactor.healthHaver.NextDamageIgnoresArmor = true;
          interactor.healthHaver.ApplyDamage((float) this.cost, Vector2.zero, StringTableManager.GetEnemiesString("#SHRINE"), damageCategory: DamageCategory.Environment, ignoreInvulnerabilityFrames: true);
          break;
        case ShrineCost.CostType.ARMOR:
          interactor.healthHaver.Armor -= (float) this.cost;
          break;
        case ShrineCost.CostType.BLANK:
          interactor.Blanks -= this.cost;
          break;
        case ShrineCost.CostType.KEY:
          if (interactor.carriedConsumables.InfiniteKeys)
            break;
          interactor.carriedConsumables.KeyBullets -= this.cost;
          break;
        case ShrineCost.CostType.CURRENT_GUN:
          interactor.inventory.DestroyCurrentGun();
          break;
        case ShrineCost.CostType.STATS:
          for (int index = 0; index < this.statMods.Length; ++index)
          {
            if (interactor.ownerlessStatModifiers == null)
              interactor.ownerlessStatModifiers = new List<StatModifier>();
            interactor.ownerlessStatModifiers.Add(this.statMods[index]);
          }
          if (interactor.characterIdentity == PlayableCharacters.Robot && this.AllowsArmorConversionForRobot && this.statMods[0].statToBoost == PlayerStats.StatType.Health && (double) this.statMods[0].amount * -2.0 < (double) interactor.healthHaver.Armor)
            interactor.healthHaver.Armor -= this.statMods[0].amount * -2f;
          interactor.stats.RecalculateStats(interactor);
          break;
        case ShrineCost.CostType.MONEY_PER_CURSE:
          interactor.carriedConsumables.Currency -= Mathf.FloorToInt((float) (this.cost * PlayerStats.GetTotalCurse()));
          break;
        case ShrineCost.CostType.SPECIFIC_ITEM:
          interactor.RemovePassiveItem(this.targetItemID);
          break;
      }
    }

    public enum CostType
    {
      MONEY,
      HEALTH,
      ARMOR,
      BLANK,
      KEY,
      CURRENT_GUN,
      BEATEN_GAME,
      STATS,
      MONEY_PER_CURSE,
      SPECIFIC_ITEM,
    }
  }

