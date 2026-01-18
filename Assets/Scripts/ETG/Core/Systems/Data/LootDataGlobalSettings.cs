using System.Collections.Generic;

using UnityEngine;

#nullable disable

public class LootDataGlobalSettings : ScriptableObject
    {
        private static LootDataGlobalSettings m_instance;
        public float GunClassModifier = 0.2f;
        [SerializeField]
        public List<GunClassModifierOverride> GunClassOverrides;

        public static LootDataGlobalSettings Instance
        {
            get
            {
                if ((Object) LootDataGlobalSettings.m_instance == (Object) null)
                    LootDataGlobalSettings.m_instance = (LootDataGlobalSettings) BraveResources.Load("GlobalLootSettings", ".asset");
                return LootDataGlobalSettings.m_instance;
            }
        }

        public float GetModifierForClass(GunClass targetClass)
        {
            for (int index = 0; index < this.GunClassOverrides.Count; ++index)
            {
                if (this.GunClassOverrides[index].classToModify == targetClass)
                    return this.GunClassOverrides[index].modifier;
            }
            return this.GunClassModifier;
        }
    }

