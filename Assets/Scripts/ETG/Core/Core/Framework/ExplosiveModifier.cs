using UnityEngine;

#nullable disable

public class ExplosiveModifier : BraveBehaviour
    {
        public bool doExplosion = true;
        [SerializeField]
        public ExplosionData explosionData;
        public bool doDistortionWave;
        [ShowInInspectorIf("doDistortionWave", true)]
        public float distortionIntensity = 1f;
        [ShowInInspectorIf("doDistortionWave", true)]
        public float distortionRadius = 1f;
        [ShowInInspectorIf("doDistortionWave", true)]
        public float maxDistortionRadius = 10f;
        [ShowInInspectorIf("doDistortionWave", true)]
        public float distortionDuration = 0.5f;
        public bool IgnoreQueues;

        public void Explode(Vector2 sourceNormal, bool ignoreDamageCaps = false, CollisionData cd = null)
        {
            if ((bool) (Object) this.projectile && (bool) (Object) this.projectile.Owner)
            {
                if (this.projectile.Owner is PlayerController)
                {
                    for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
                    {
                        PlayerController allPlayer = GameManager.Instance.AllPlayers[index];
                        if ((bool) (Object) allPlayer && (bool) (Object) allPlayer.specRigidbody)
                            this.explosionData.ignoreList.Add(allPlayer.specRigidbody);
                    }
                }
                else
                    this.explosionData.ignoreList.Add(this.projectile.Owner.specRigidbody);
            }
            Vector3 vector3 = cd == null ? this.specRigidbody.UnitCenter.ToVector3ZUp() : cd.Contact.ToVector3ZUp();
            if (this.doExplosion)
            {
                CoreDamageTypes damageTypes = CoreDamageTypes.None;
                if (this.explosionData.doDamage && (double) this.explosionData.damageRadius < 10.0 && (bool) (Object) this.projectile)
                {
                    if (this.projectile.AppliesFreeze)
                        damageTypes |= CoreDamageTypes.Ice;
                    if (this.projectile.AppliesFire)
                        damageTypes |= CoreDamageTypes.Fire;
                    if (this.projectile.AppliesPoison)
                        damageTypes |= CoreDamageTypes.Poison;
                    if (this.projectile.statusEffectsToApply != null)
                    {
                        for (int index = 0; index < this.projectile.statusEffectsToApply.Count; ++index)
                        {
                            switch (this.projectile.statusEffectsToApply[index])
                            {
                                case GameActorFreezeEffect _:
                                    damageTypes |= CoreDamageTypes.Ice;
                                    break;
                                case GameActorFireEffect _:
                                    damageTypes |= CoreDamageTypes.Fire;
                                    break;
                                case GameActorHealthEffect _:
                                    damageTypes |= CoreDamageTypes.Poison;
                                    break;
                            }
                        }
                    }
                }
                Exploder.Explode(vector3, this.explosionData, sourceNormal, ignoreQueues: this.IgnoreQueues, damageTypes: damageTypes, ignoreDamageCaps: ignoreDamageCaps);
            }
            if (!this.doDistortionWave)
                return;
            Exploder.DoDistortionWave((Vector2) vector3, this.distortionIntensity, this.distortionRadius, this.maxDistortionRadius, this.distortionDuration);
        }

        protected override void OnDestroy() => base.OnDestroy();
    }

