using System;

#nullable disable

[Serializable]
public class PassiveGooperSynergy
    {
        public CustomSynergyType RequiredSynergy;
        public GoopDefinition overrideGoopType;
        public DamageTypeModifier AdditionalDamageModifier;
        [NonSerialized]
        public bool m_processed;
    }

