using System;

using UnityEngine;

#nullable disable

public class BulletStatusEffectItem : PassiveItem
    {
        public float chanceOfActivating = 1f;
        public float chanceFromBeamPerSecond = 1f;
        public bool TintBullets;
        public bool TintBeams;
        public Color TintColor = Color.green;
        public int TintPriority = 5;
        public GameObject ParticlesToAdd;
        public bool AddsDamageType;
        [EnumFlags]
        public CoreDamageTypes DamageTypesToAdd;
        [Header("Status Effects")]
        public bool AppliesSpeedModifier;
        public GameActorSpeedEffect SpeedModifierEffect;
        public bool AppliesDamageOverTime;
        public GameActorHealthEffect HealthModifierEffect;
        public bool AppliesCharm;
        public GameActorCharmEffect CharmModifierEffect;
        public bool AppliesFreeze;
        public GameActorFreezeEffect FreezeModifierEffect;
        [ShowInInspectorIf("AppliesFreeze", false)]
        public bool FreezeScalesWithDamage;
        [ShowInInspectorIf("FreezeScalesWithDamage", false)]
        public float FreezeAmountPerDamage = 1f;
        public bool AppliesFire;
        public GameActorFireEffect FireModifierEffect;
        public bool ConfersElectricityImmunity;
        public bool AppliesTransmog;
        [EnemyIdentifier]
        public string TransmogTargetGuid;
        public BulletStatusEffectItemSynergy[] Synergies;
        private PlayerController m_player;
        private DamageTypeModifier m_electricityImmunity;

        public override void Pickup(PlayerController player)
        {
            if (this.m_pickedUp)
                return;
            this.m_player = player;
            if (this.ConfersElectricityImmunity)
            {
                this.m_electricityImmunity = new DamageTypeModifier();
                this.m_electricityImmunity.damageMultiplier = 0.0f;
                this.m_electricityImmunity.damageType = CoreDamageTypes.Electric;
                player.healthHaver.damageTypeModifiers.Add(this.m_electricityImmunity);
            }
            base.Pickup(player);
            player.PostProcessProjectile += new Action<Projectile, float>(this.PostProcessProjectile);
            player.PostProcessBeam += new Action<BeamController>(this.PostProcessBeam);
            player.PostProcessBeamTick += new Action<BeamController, SpeculativeRigidbody, float>(this.PostProcessBeamTick);
        }

        private void PostProcessProjectile(Projectile obj, float effectChanceScalar)
        {
            float num = this.chanceOfActivating;
            if ((double) this.chanceOfActivating < 1.0)
                num = this.chanceOfActivating * effectChanceScalar;
            if (this.AppliesFreeze || this.AppliesFire || this.AppliesDamageOverTime)
            {
                if ((bool) (UnityEngine.Object) this.m_player && this.m_player.HasActiveBonusSynergy(CustomSynergyType.ALPHA_STATUS) && this.m_player.CurrentGun.LastShotIndex == 0)
                    num = 1f;
                if ((bool) (UnityEngine.Object) this.m_player && this.m_player.HasActiveBonusSynergy(CustomSynergyType.OMEGA_STATUS) && this.m_player.CurrentGun.LastShotIndex == this.m_player.CurrentGun.ClipCapacity - 1)
                    num = 1f;
            }
            if (this.AppliesCharm && (bool) (UnityEngine.Object) this.Owner && this.Owner.HasActiveBonusSynergy(CustomSynergyType.UNBELIEVABLY_CHARMING))
                num = 1f;
            if (this.AppliesTransmog && (bool) (UnityEngine.Object) this.Owner && this.Owner.HasActiveBonusSynergy(CustomSynergyType.BE_A_CHICKEN))
                num *= 1.5f;
            if ((bool) (UnityEngine.Object) this.m_player)
            {
                for (int index = 0; index < this.Synergies.Length; ++index)
                {
                    if (this.m_player.HasActiveBonusSynergy(this.Synergies[index].RequiredSynergy))
                        num *= this.Synergies[index].ChanceMultiplier;
                }
            }
            if ((double) UnityEngine.Random.value >= (double) num)
                return;
            if (this.AddsDamageType)
                obj.damageTypes |= this.DamageTypesToAdd;
            if ((UnityEngine.Object) this.ParticlesToAdd != (UnityEngine.Object) null)
            {
                GameObject gameObject = SpawnManager.SpawnVFX(this.ParticlesToAdd, true);
                gameObject.transform.parent = obj.transform;
                gameObject.transform.localPosition = new Vector3(0.0f, 0.0f, 0.5f);
                ParticleKiller component = gameObject.GetComponent<ParticleKiller>();
                if ((UnityEngine.Object) component != (UnityEngine.Object) null)
                    component.Awake();
            }
            if (this.AppliesSpeedModifier)
                obj.statusEffectsToApply.Add((GameActorEffect) this.SpeedModifierEffect);
            if (this.AppliesDamageOverTime)
                obj.statusEffectsToApply.Add((GameActorEffect) this.HealthModifierEffect);
            if (this.AppliesFreeze)
            {
                GameActorFreezeEffect freezeModifierEffect = this.FreezeModifierEffect;
                if (this.FreezeScalesWithDamage)
                    freezeModifierEffect.FreezeAmount = obj.ModifiedDamage * this.FreezeAmountPerDamage;
                obj.statusEffectsToApply.Add((GameActorEffect) freezeModifierEffect);
            }
            if (this.AppliesCharm)
                obj.statusEffectsToApply.Add((GameActorEffect) this.CharmModifierEffect);
            if (this.AppliesFire)
                obj.statusEffectsToApply.Add((GameActorEffect) this.FireModifierEffect);
            if (this.AppliesTransmog && !obj.CanTransmogrify)
            {
                obj.CanTransmogrify = true;
                obj.ChanceToTransmogrify = 1f;
                obj.TransmogrifyTargetGuids = new string[1];
                obj.TransmogrifyTargetGuids[0] = this.TransmogTargetGuid;
            }
            if (!this.TintBullets)
                return;
            obj.AdjustPlayerProjectileTint(this.TintColor, this.TintPriority);
        }

        private void PostProcessBeam(BeamController beam)
        {
            if (!this.TintBeams)
                return;
            beam.AdjustPlayerBeamTint(this.TintColor.WithAlpha(this.TintColor.a / 2f), this.TintPriority);
        }

        private void PostProcessBeamTick(
            BeamController beam,
            SpeculativeRigidbody hitRigidbody,
            float tickRate)
        {
            GameActor gameActor = hitRigidbody.gameActor;
            if (!(bool) (UnityEngine.Object) gameActor || (double) UnityEngine.Random.value >= (double) BraveMathCollege.SliceProbability(this.chanceFromBeamPerSecond, tickRate))
                return;
            if (this.AppliesSpeedModifier)
                gameActor.ApplyEffect((GameActorEffect) this.SpeedModifierEffect);
            if (this.AppliesDamageOverTime)
                gameActor.ApplyEffect((GameActorEffect) this.HealthModifierEffect);
            if (this.AppliesFreeze)
                gameActor.ApplyEffect((GameActorEffect) this.FreezeModifierEffect);
            if (this.AppliesCharm)
                gameActor.ApplyEffect((GameActorEffect) this.CharmModifierEffect);
            if (this.AppliesFire)
                gameActor.ApplyEffect((GameActorEffect) this.FireModifierEffect);
            if (!this.AppliesTransmog || !(gameActor is AIActor))
                return;
            (gameActor as AIActor).Transmogrify(EnemyDatabase.GetOrLoadByGuid(this.TransmogTargetGuid), (GameObject) ResourceCache.Acquire("Global VFX/VFX_Item_Spawn_Poof"));
        }

        public override DebrisObject Drop(PlayerController player)
        {
            DebrisObject debrisObject = base.Drop(player);
            this.m_player = (PlayerController) null;
            debrisObject.GetComponent<BulletStatusEffectItem>().m_pickedUpThisRun = true;
            if (this.m_electricityImmunity != null)
                player.healthHaver.damageTypeModifiers.Remove(this.m_electricityImmunity);
            player.PostProcessProjectile -= new Action<Projectile, float>(this.PostProcessProjectile);
            player.PostProcessBeam -= new Action<BeamController>(this.PostProcessBeam);
            player.PostProcessBeamTick -= new Action<BeamController, SpeculativeRigidbody, float>(this.PostProcessBeamTick);
            return debrisObject;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (!(bool) (UnityEngine.Object) this.m_player)
                return;
            if (this.m_electricityImmunity != null)
                this.m_player.healthHaver.damageTypeModifiers.Remove(this.m_electricityImmunity);
            this.m_player.PostProcessProjectile -= new Action<Projectile, float>(this.PostProcessProjectile);
            this.m_player.PostProcessBeam -= new Action<BeamController>(this.PostProcessBeam);
            this.m_player.PostProcessBeamTick -= new Action<BeamController, SpeculativeRigidbody, float>(this.PostProcessBeamTick);
        }
    }

