using System;

#nullable disable

[Serializable]
public struct SynergyBurstLimit
    {
        [LongNumericEnum]
        public CustomSynergyType RequiredSynergy;
        public int limit;
    }

