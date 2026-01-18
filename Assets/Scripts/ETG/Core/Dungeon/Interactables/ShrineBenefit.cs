using System;
using System.Collections.Generic;

using UnityEngine;

#nullable disable

[Serializable]
public class ShrineBenefit
    {
        public ShrineBenefit.BenefitType benefitType;
        public float amount;
        [ShowInInspectorIf("benefitType", 5, false)]
        public bool appliesToAllGuns;
        public StatModifier[] statMods;
        public string rngString;
        public float rngWeight = 1f;
        [PickupIdentifier]
        public int targetItemID;
        [PickupIdentifier]
        public int TurkeyCompanionForCompanionShrine;
        [NonSerialized]
        public bool IsRNGChest;

        public void ApplyBenefit(PlayerController interactor)
        {
            int num1 = Mathf.RoundToInt(this.amount);
            switch (this.benefitType)
            {
                case ShrineBenefit.BenefitType.MONEY:
                    interactor.carriedConsumables.Currency += num1;
                    break;
                case ShrineBenefit.BenefitType.HEALTH:
                    if ((double) interactor.healthHaver.GetCurrentHealthPercentage() >= 1.0)
                    {
                        ++interactor.Blanks;
                        break;
                    }
                    interactor.healthHaver.ApplyHealing(this.amount);
                    break;
                case ShrineBenefit.BenefitType.ARMOR:
                    interactor.healthHaver.Armor += this.amount;
                    break;
                case ShrineBenefit.BenefitType.BLANK:
                    interactor.Blanks += num1;
                    break;
                case ShrineBenefit.BenefitType.KEY:
                    interactor.carriedConsumables.KeyBullets += num1;
                    break;
                case ShrineBenefit.BenefitType.AMMO_PERCENTAGE:
                    interactor.ResetTarnisherClipCapacity();
                    if (this.appliesToAllGuns)
                    {
                        for (int index = 0; index < interactor.inventory.AllGuns.Count; ++index)
                        {
                            if (interactor.inventory.AllGuns[index].CanGainAmmo)
                            {
                                int amt = Mathf.FloorToInt((float) interactor.inventory.AllGuns[index].AdjustedMaxAmmo * this.amount);
                                if (amt <= 0)
                                {
                                    int num2 = (int) AkSoundEngine.PostEvent("Play_OBJ_ammo_pickup_01", interactor.gameObject);
                                    amt = Mathf.FloorToInt((float) interactor.inventory.AllGuns[index].ammo * this.amount);
                                }
                                if (amt <= 0)
                                {
                                    Debug.LogError((object) "Shrine is attempting to give negative ammo!");
                                    amt = 1;
                                }
                                interactor.inventory.AllGuns[index].GainAmmo(amt);
                            }
                        }
                        break;
                    }
                    if (!((UnityEngine.Object) interactor.inventory.CurrentGun != (UnityEngine.Object) null) || !interactor.inventory.CurrentGun.CanGainAmmo)
                        break;
                    int amt1 = Mathf.FloorToInt((float) interactor.inventory.CurrentGun.AdjustedMaxAmmo * this.amount);
                    if (amt1 <= 0)
                    {
                        int num3 = (int) AkSoundEngine.PostEvent("Play_OBJ_ammo_pickup_01", interactor.gameObject);
                        amt1 = Mathf.FloorToInt((float) interactor.inventory.CurrentGun.ammo * this.amount);
                    }
                    if (amt1 <= 0)
                    {
                        Debug.LogError((object) "Shrine is attempting to give negative ammo!");
                        amt1 = 1;
                    }
                    interactor.inventory.CurrentGun.GainAmmo(amt1);
                    break;
                case ShrineBenefit.BenefitType.STATS:
                    for (int index = 0; index < this.statMods.Length; ++index)
                    {
                        if (interactor.ownerlessStatModifiers == null)
                            interactor.ownerlessStatModifiers = new List<StatModifier>();
                        interactor.ownerlessStatModifiers.Add(this.statMods[index]);
                    }
                    interactor.stats.RecalculateStats(interactor);
                    break;
                case ShrineBenefit.BenefitType.CLEANSE_CURSE:
                    interactor.ownerlessStatModifiers.Add(new StatModifier()
                    {
                        amount = Mathf.Min(this.amount, (float) (PlayerStats.GetTotalCurse() * -1)),
                        modifyType = StatModifier.ModifyMethod.ADDITIVE,
                        statToBoost = PlayerStats.StatType.Curse
                    });
                    interactor.stats.RecalculateStats(interactor);
                    break;
                case ShrineBenefit.BenefitType.SPAWN_CHEST:
                    IntVector2 position = interactor.CurrentRoom.GetBestRewardLocation(new IntVector2(2, 3)) + new IntVector2(0, 2);
                    if (this.IsRNGChest)
                    {
                        Chest chest = GameManager.Instance.RewardManager.SpawnTotallyRandomChest(position);
                        if (!((UnityEngine.Object) chest != (UnityEngine.Object) null))
                            break;
                        chest.RegisterChestOnMinimap(interactor.CurrentRoom);
                        break;
                    }
                    Chest chest1 = GameManager.Instance.RewardManager.SpawnRewardChestAt(position);
                    if (!((UnityEngine.Object) chest1 != (UnityEngine.Object) null))
                        break;
                    chest1.RegisterChestOnMinimap(interactor.CurrentRoom);
                    break;
                case ShrineBenefit.BenefitType.SPECIFIC_ITEM:
                    LootEngine.GivePrefabToPlayer(PickupObjectDatabase.GetById(this.targetItemID).gameObject, interactor);
                    break;
                case ShrineBenefit.BenefitType.COMPANION:
                    GameStatsManager.Instance.RegisterStatChange(TrackedStats.TIMES_COMPANION_SHRINED, 1f);
                    CompanionItem companionItem = LootEngine.GetItemOfTypeAndQuality<CompanionItem>(PickupObject.ItemQuality.A, GameManager.Instance.RewardManager.ItemsLootTable, true);
                    if ((double) GameStatsManager.Instance.GetPlayerStatValue(TrackedStats.TIMES_COMPANION_SHRINED) >= 2.0)
                    {
                        GameStatsManager.Instance.SetFlag(GungeonFlags.ITEMSPECIFIC_TURKEY, true);
                        if ((double) GameStatsManager.Instance.GetPlayerStatValue(TrackedStats.TIMES_COMPANION_SHRINED) == 2.0)
                            companionItem = PickupObjectDatabase.GetById(this.TurkeyCompanionForCompanionShrine) as CompanionItem;
                    }
                    if (GameStatsManager.Instance.IsRainbowRun)
                    {
                        LootEngine.SpawnBowlerNote(GameManager.Instance.RewardManager.BowlerNoteOtherSource, (Vector2) (interactor.transform.position + new Vector3(0.0f, -0.5f, 0.0f)), interactor.CurrentRoom, true);
                        break;
                    }
                    if (!(bool) (UnityEngine.Object) companionItem)
                        break;
                    LootEngine.GivePrefabToPlayer(companionItem.gameObject, interactor);
                    break;
                case ShrineBenefit.BenefitType.BLOODTHIRST:
                    interactor.gameObject.GetOrAddComponent<Bloodthirst>();
                    break;
            }
        }

        public enum BenefitType
        {
            MONEY,
            HEALTH,
            ARMOR,
            BLANK,
            KEY,
            AMMO_PERCENTAGE,
            STATS,
            CLEANSE_CURSE,
            SPAWN_CHEST,
            SPECIFIC_ITEM,
            COMPANION,
            BLOODTHIRST,
        }
    }

