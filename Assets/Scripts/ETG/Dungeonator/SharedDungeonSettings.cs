using System.Collections.Generic;

using UnityEngine;

#nullable disable
namespace Dungeonator
{
    public class SharedDungeonSettings : MonoBehaviour
    {
        [Header("Currency")]
        public GenericCurrencyDropSettings currencyDropSettings;
        [Header("Boss Chests")]
        public WeightedGameObjectCollection ChestsForBosses;
        [Header("Mimics")]
        public List<DungeonPrerequisite> MimicPrerequisites = new List<DungeonPrerequisite>();
        public float MimicChance = 0.05f;
        public float MimicChancePerCurseLevel = 0.01f;
        [Header("Pedestal Mimics")]
        public List<DungeonPrerequisite> PedestalMimicPrerequisites = new List<DungeonPrerequisite>();
        public float PedestalMimicChance = 0.05f;
        public float PedestalMimicChancePerCurseLevel = 0.01f;
        [Header("Pot Fairies")]
        public List<DungeonPrerequisite> PotFairyPrerequisites = new List<DungeonPrerequisite>();
        [EnemyIdentifier]
        public string PotFairyGuid;
        public float PotFairyChance = 0.005f;
        [Header("Cross-Dungeon Settings")]
        public RobotDaveIdea DefaultProceduralIdea;
        public ExplosionData DefaultExplosionData;
        public ExplosionData DefaultSmallExplosionData;
        public GameActorFreezeEffect DefaultFreezeExplosionEffect;
        public GoopDefinition DefaultFreezeGoop;
        public GoopDefinition DefaultFireGoop;
        public GoopDefinition DefaultPoisonGoop;
        public GameObject additionalCheeseDustup;
        public GameObject additionalWebDustup;
        public GameObject additionalTableDustup;
        public GameActorCharmEffect DefaultPermanentCharmEffect;

        public List<GameObject> GetCurrencyToDrop(
            int amountToDrop,
            bool isMetaCurrency = false,
            bool randomAmounts = false)
        {
            List<GameObject> currencyToDrop = new List<GameObject>();
            int currencyValue1 = this.currencyDropSettings.goldCoinPrefab.GetComponent<CurrencyPickup>().currencyValue;
            int currencyValue2 = this.currencyDropSettings.silverCoinPrefab.GetComponent<CurrencyPickup>().currencyValue;
            int currencyValue3 = this.currencyDropSettings.bronzeCoinPrefab.GetComponent<CurrencyPickup>().currencyValue;
            int num1 = 1;
            while (amountToDrop > 0)
            {
                GameObject gameObject;
                if (isMetaCurrency)
                {
                    amountToDrop -= num1;
                    gameObject = this.currencyDropSettings.metaCoinPrefab;
                }
                else if (randomAmounts)
                {
                    if (amountToDrop >= currencyValue1)
                    {
                        float num2 = Random.value;
                        if ((double) num2 < 0.05000000074505806)
                        {
                            amountToDrop -= currencyValue1;
                            gameObject = this.currencyDropSettings.goldCoinPrefab;
                        }
                        else if ((double) num2 < 0.25)
                        {
                            amountToDrop -= currencyValue2;
                            gameObject = this.currencyDropSettings.silverCoinPrefab;
                        }
                        else
                        {
                            amountToDrop -= currencyValue3;
                            gameObject = this.currencyDropSettings.bronzeCoinPrefab;
                        }
                    }
                    else if (amountToDrop >= currencyValue2)
                    {
                        if ((double) Random.value < 0.25)
                        {
                            amountToDrop -= currencyValue2;
                            gameObject = this.currencyDropSettings.silverCoinPrefab;
                        }
                        else
                        {
                            amountToDrop -= currencyValue3;
                            gameObject = this.currencyDropSettings.bronzeCoinPrefab;
                        }
                    }
                    else if (amountToDrop >= currencyValue3)
                    {
                        amountToDrop -= currencyValue3;
                        gameObject = this.currencyDropSettings.bronzeCoinPrefab;
                    }
                    else
                    {
                        amountToDrop = 0;
                        break;
                    }
                }
                else if (amountToDrop >= currencyValue1)
                {
                    amountToDrop -= currencyValue1;
                    gameObject = this.currencyDropSettings.goldCoinPrefab;
                }
                else if (amountToDrop >= currencyValue2)
                {
                    amountToDrop -= currencyValue2;
                    gameObject = this.currencyDropSettings.silverCoinPrefab;
                }
                else if (amountToDrop >= currencyValue3)
                {
                    amountToDrop -= currencyValue3;
                    gameObject = this.currencyDropSettings.bronzeCoinPrefab;
                }
                else
                {
                    amountToDrop = 0;
                    break;
                }
                if ((Object) gameObject != (Object) null)
                    currencyToDrop.Add(gameObject);
            }
            return currencyToDrop;
        }

        public bool RandomShouldBecomeMimic(float overrideChance = -1f)
        {
            for (int index = 0; index < this.MimicPrerequisites.Count; ++index)
            {
                if (!this.MimicPrerequisites[index].CheckConditionsFulfilled())
                    return false;
            }
            float num1;
            if ((double) overrideChance >= 0.0)
            {
                num1 = overrideChance;
            }
            else
            {
                num1 = this.MimicChance + this.MimicChancePerCurseLevel * (float) PlayerStats.GetTotalCurse();
                if (PassiveItem.IsFlagSetAtAll(typeof (MimicToothNecklaceItem)))
                    num1 += 10f;
            }
            float num2 = Random.value;
            Debug.Log((object) $"mimic {(object) num2}|{(object) num1}");
            return (double) num2 <= (double) num1;
        }

        public bool RandomShouldBecomePedestalMimic(float overrideChance = -1f)
        {
            for (int index = 0; index < this.PedestalMimicPrerequisites.Count; ++index)
            {
                if (!this.PedestalMimicPrerequisites[index].CheckConditionsFulfilled())
                    return false;
            }
            float num1;
            if ((double) overrideChance >= 0.0)
            {
                num1 = overrideChance;
            }
            else
            {
                num1 = this.PedestalMimicChance + this.PedestalMimicChancePerCurseLevel * (float) PlayerStats.GetTotalCurse();
                if (PassiveItem.IsFlagSetAtAll(typeof (MimicToothNecklaceItem)))
                    num1 += 10f;
            }
            float num2 = Random.value;
            Debug.Log((object) $"pedestal mimic {(object) num2}|{(object) num1}");
            return (double) num2 <= (double) num1;
        }

        public bool RandomShouldSpawnPotFairy()
        {
            for (int index = 0; index < this.PotFairyPrerequisites.Count; ++index)
            {
                if (!this.PotFairyPrerequisites[index].CheckConditionsFulfilled())
                    return false;
            }
            return (double) Random.value <= (double) this.PotFairyChance;
        }
    }
}
