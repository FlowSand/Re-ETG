using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using UnityEngine;

#nullable disable

public class TransformingGunModifier : MonoBehaviour
    {
        [PickupIdentifier]
        public int BaseGunID;
        public bool TransformsOnAmmoThresholds;
        public List<AmmoThresholdTransformation> AmmoThresholdTransformations;
        public bool TransformsOnDamageDealt;
        public bool TransformationsAreTimeLimited;
        [ShowInInspectorIf("TransformationsAreTimeLimited", false)]
        public float TransformationDuration = 10f;
        public bool TransformationsAreAmmoLimited;
        [ShowInInspectorIf("TransformationsAreAmmoLimited", false)]
        public int TransformationAmmoCount = 10;
        private Gun m_gun;
        private float m_previousAmmoPercentage = 1f;

        [DebuggerHidden]
        private IEnumerator Start()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new TransformingGunModifier__Startc__Iterator0()
            {
                _this = this
            };
        }

        private float GetMaxAmmoSansInfinity(Gun g)
        {
            if ((Object) g.CurrentOwner == (Object) null || !(g.CurrentOwner is PlayerController))
                return (float) g.GetBaseMaxAmmo();
            if (g.RequiresFundsToShoot)
                return (float) g.ClipShotsRemaining;
            return (Object) (g.CurrentOwner as PlayerController).stats != (Object) null ? (float) Mathf.RoundToInt((g.CurrentOwner as PlayerController).stats.GetStatValue(PlayerStats.StatType.AmmoCapacityMultiplier) * (float) g.GetBaseMaxAmmo()) : (float) g.GetBaseMaxAmmo();
        }

        private void HandlePostFired(PlayerController arg1, Gun arg2)
        {
            if (!arg2.enabled)
                return;
            float num = (float) this.m_gun.CurrentAmmo / (1f * this.GetMaxAmmoSansInfinity(this.m_gun));
            AmmoThresholdTransformation? nullable = new AmmoThresholdTransformation?();
            for (int index = 0; index < this.AmmoThresholdTransformations.Count; ++index)
            {
                AmmoThresholdTransformation thresholdTransformation = this.AmmoThresholdTransformations[index];
                if ((double) num <= (double) thresholdTransformation.GetAmmoPercentage())
                {
                    if (!nullable.HasValue)
                        nullable = new AmmoThresholdTransformation?(thresholdTransformation);
                    else if ((double) thresholdTransformation.GetAmmoPercentage() < (double) nullable.Value.GetAmmoPercentage())
                        nullable = new AmmoThresholdTransformation?(thresholdTransformation);
                }
            }
            if (nullable.HasValue)
            {
                Gun byId = PickupObjectDatabase.GetById(nullable.Value.TargetGunID) as Gun;
                if ((bool) (Object) byId && byId.shootAnimation != this.m_gun.shootAnimation)
                    this.m_gun.TransformToTargetGun(byId);
            }
            this.m_previousAmmoPercentage = num;
        }
    }

