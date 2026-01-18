using System;

#nullable disable
namespace Dungeonator
{
    [Serializable]
    public struct CellDamageDefinition
    {
        public CoreDamageTypes damageTypes;
        public float damageToPlayersPerTick;
        public float damageToEnemiesPerTick;
        public float tickFrequency;
        public bool respectsFlying;
        public bool isPoison;

        public bool HasChanges()
        {
            return this.damageTypes != CoreDamageTypes.None || (double) this.damageToPlayersPerTick != 0.0 || (double) this.damageToEnemiesPerTick != 0.0 || (double) this.tickFrequency != 0.0 || this.respectsFlying || this.isPoison;
        }
    }
}
