using System;

using UnityEngine;

#nullable disable
namespace Dungeonator
{
    [Serializable]
    public class GenericCurrencyDropSettings
    {
        [PickupIdentifier]
        public int bronzeCoinId = -1;
        [PickupIdentifier]
        public int silverCoinId = -1;
        [PickupIdentifier]
        public int goldCoinId = -1;
        [PickupIdentifier]
        public int metaCoinId = -1;
        public WeightedIntCollection blackPhantomCoinDropChances;

        public GameObject bronzeCoinPrefab => PickupObjectDatabase.GetById(this.bronzeCoinId).gameObject;

        public GameObject silverCoinPrefab => PickupObjectDatabase.GetById(this.silverCoinId).gameObject;

        public GameObject goldCoinPrefab => PickupObjectDatabase.GetById(this.goldCoinId).gameObject;

        public GameObject metaCoinPrefab => PickupObjectDatabase.GetById(this.metaCoinId).gameObject;
    }
}
