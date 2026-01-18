using System;

#nullable disable

[Serializable]
public class CompanionTransformSynergy
    {
        [LongNumericEnum]
        public CustomSynergyType RequiredSynergy;
        [EnemyIdentifier]
        public string SynergyCompanionGuid;
    }

