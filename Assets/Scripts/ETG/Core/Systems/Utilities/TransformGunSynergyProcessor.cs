using System;

using UnityEngine;

using Dungeonator;

#nullable disable

public class TransformGunSynergyProcessor : MonoBehaviour
    {
        [LongNumericEnum]
        public CustomSynergyType SynergyToCheck;
        [PickupIdentifier(typeof (Gun))]
        public int NonSynergyGunId = -1;
        [PickupIdentifier(typeof (Gun))]
        public int SynergyGunId = -1;
        private Gun m_gun;
        private bool m_transformed;
        [NonSerialized]
        public bool ShouldResetAmmoAfterTransformation;
        [NonSerialized]
        public int ResetAmmoCount;

        private void Awake() => this.m_gun = this.GetComponent<Gun>();

        private void Update()
        {
            if (Dungeon.IsGenerating || Dungeon.ShouldAttemptToLoadFromMidgameSave)
                return;
            if ((bool) (UnityEngine.Object) this.m_gun && this.m_gun.CurrentOwner is PlayerController)
            {
                PlayerController currentOwner = this.m_gun.CurrentOwner as PlayerController;
                if (!this.m_gun.enabled)
                    return;
                if (currentOwner.HasActiveBonusSynergy(this.SynergyToCheck) && !this.m_transformed)
                {
                    this.m_transformed = true;
                    this.m_gun.TransformToTargetGun(PickupObjectDatabase.GetById(this.SynergyGunId) as Gun);
                    if (this.ShouldResetAmmoAfterTransformation)
                        this.m_gun.ammo = this.ResetAmmoCount;
                }
                else if (!currentOwner.HasActiveBonusSynergy(this.SynergyToCheck) && this.m_transformed)
                {
                    this.m_transformed = false;
                    this.m_gun.TransformToTargetGun(PickupObjectDatabase.GetById(this.NonSynergyGunId) as Gun);
                    if (this.ShouldResetAmmoAfterTransformation)
                        this.m_gun.ammo = this.ResetAmmoCount;
                }
            }
            else if ((bool) (UnityEngine.Object) this.m_gun && !(bool) (UnityEngine.Object) this.m_gun.CurrentOwner && this.m_transformed)
            {
                this.m_transformed = false;
                this.m_gun.TransformToTargetGun(PickupObjectDatabase.GetById(this.NonSynergyGunId) as Gun);
                if (this.ShouldResetAmmoAfterTransformation)
                    this.m_gun.ammo = this.ResetAmmoCount;
            }
            this.ShouldResetAmmoAfterTransformation = false;
        }
    }

