using System;

#nullable disable

[Serializable]
public class DamageTypeModifier
    {
        public CoreDamageTypes damageType;
        public float damageMultiplier = 1f;

        public DamageTypeModifier()
        {
        }

        public DamageTypeModifier(DamageTypeModifier other)
        {
            this.damageType = other.damageType;
            this.damageMultiplier = other.damageMultiplier;
        }
    }

