using System;

#nullable disable

public class HomingBulletsPassiveItem : PassiveItem
    {
        public float ActivationChance = 1f;
        public float homingRadius = 5f;
        public float homingAngularVelocity = 360f;
        public bool SynergyIncreasesDamageIfNotActive;
        [LongNumericEnum]
        public CustomSynergyType SynergyRequired;
        public float SynergyDamageMultiplier = 1.5f;
        protected PlayerController m_player;

        public override void Pickup(PlayerController player)
        {
            if (this.m_pickedUp)
                return;
            this.m_player = player;
            base.Pickup(player);
            player.PostProcessProjectile += new Action<Projectile, float>(this.PostProcessProjectile);
            player.PostProcessBeamChanceTick += new Action<BeamController>(this.PostProcessBeamChanceTick);
        }

        private void PostProcessProjectile(Projectile obj, float effectChanceScalar)
        {
            if ((double) UnityEngine.Random.value > (double) this.ActivationChance * (double) effectChanceScalar)
            {
                if (!this.SynergyIncreasesDamageIfNotActive || !(bool) (UnityEngine.Object) this.m_player || !this.m_player.HasActiveBonusSynergy(this.SynergyRequired))
                    return;
                obj.baseData.damage *= this.SynergyDamageMultiplier;
                obj.RuntimeUpdateScale(this.SynergyDamageMultiplier);
            }
            else
            {
                HomingModifier homingModifier = obj.gameObject.GetComponent<HomingModifier>();
                if ((UnityEngine.Object) homingModifier == (UnityEngine.Object) null)
                {
                    homingModifier = obj.gameObject.AddComponent<HomingModifier>();
                    homingModifier.HomingRadius = 0.0f;
                    homingModifier.AngularVelocity = 0.0f;
                }
                float num = !this.SynergyIncreasesDamageIfNotActive || !(bool) (UnityEngine.Object) this.m_player || !this.m_player.HasActiveBonusSynergy(this.SynergyRequired) ? 1f : 2f;
                homingModifier.HomingRadius += this.homingRadius * num;
                homingModifier.AngularVelocity += this.homingAngularVelocity * num;
            }
        }

        private void PostProcessBeamChanceTick(BeamController beam)
        {
            if ((double) UnityEngine.Random.value > (double) this.ActivationChance)
                return;
            beam.ChanceBasedHomingRadius += this.homingRadius;
            beam.ChanceBasedHomingAngularVelocity += this.homingAngularVelocity;
        }

        public override DebrisObject Drop(PlayerController player)
        {
            DebrisObject debrisObject = base.Drop(player);
            debrisObject.GetComponent<HomingBulletsPassiveItem>().m_pickedUpThisRun = true;
            this.m_player = (PlayerController) null;
            player.PostProcessProjectile -= new Action<Projectile, float>(this.PostProcessProjectile);
            player.PostProcessBeamChanceTick -= new Action<BeamController>(this.PostProcessBeamChanceTick);
            return debrisObject;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (!(bool) (UnityEngine.Object) this.m_player)
                return;
            this.m_player.PostProcessProjectile -= new Action<Projectile, float>(this.PostProcessProjectile);
            this.m_player.PostProcessBeamChanceTick -= new Action<BeamController>(this.PostProcessBeamChanceTick);
        }
    }

