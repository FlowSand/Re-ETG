using System;

#nullable disable

[Serializable]
public struct NumericSynergyMultiplier
    {
        [LongNumericEnum]
        public CustomSynergyType RequiredSynergy;
        public float SynergyMultiplier;
    }

