using System;

#nullable disable

[Serializable]
public struct AmmoThresholdTransformation
    {
        public float AmmoPercentage;
        [PickupIdentifier]
        public int TargetGunID;
        public bool HasSynergyChange;
        [LongNumericEnum]
        public CustomSynergyType RequiredSynergy;
        public float SynergyAmmoPercentage;

        public float GetAmmoPercentage()
        {
            int count = -1;
            return this.HasSynergyChange && PlayerController.AnyoneHasActiveBonusSynergy(this.RequiredSynergy, out count) ? this.SynergyAmmoPercentage : this.AmmoPercentage;
        }
    }

