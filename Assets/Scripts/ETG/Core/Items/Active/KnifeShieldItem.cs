using UnityEngine;

#nullable disable

public class KnifeShieldItem : PlayerItem
    {
        [Header("Knife Properties")]
        public int numKnives = 5;
        public float knifeHealth = 0.5f;
        public float knifeDamage = 5f;
        public float circleRadius = 3f;
        public float rotationDegreesPerSecond = 360f;
        [Header("Thrown Properties")]
        public float throwSpeed = 10f;
        public float throwRange = 25f;
        public float throwRadius = 3f;
        public float radiusChangeDistance = 3f;
        public GameObject knifePrefab;
        public GameObject knifeDeathVFX;
        protected KnifeShieldEffect m_extantEffect;
        protected KnifeShieldEffect m_secondaryEffect;

        protected override void DoEffect(PlayerController user)
        {
            this.m_extantEffect = this.CreateEffect(user);
            if (user.HasActiveBonusSynergy(CustomSynergyType.TWO_BLADES))
                this.m_secondaryEffect = this.CreateEffect(user, 1.25f, -1f);
            int num = (int) AkSoundEngine.PostEvent("Play_OBJ_daggershield_start_01", this.gameObject);
        }

        private KnifeShieldEffect CreateEffect(
            PlayerController user,
            float radiusMultiplier = 1f,
            float rotationSpeedMultiplier = 1f)
        {
            KnifeShieldEffect effect = new GameObject("knife shield effect")
            {
                transform = {
                    position = user.LockedApproximateSpriteCenter,
                    parent = user.transform
                }
            }.AddComponent<KnifeShieldEffect>();
            effect.numKnives = this.numKnives;
            effect.remainingHealth = this.knifeHealth;
            effect.knifeDamage = this.knifeDamage;
            effect.circleRadius = this.circleRadius * radiusMultiplier;
            effect.rotationDegreesPerSecond = this.rotationDegreesPerSecond * rotationSpeedMultiplier;
            effect.throwSpeed = this.throwSpeed;
            effect.throwRange = this.throwRange;
            effect.throwRadius = this.throwRadius;
            effect.radiusChangeDistance = this.radiusChangeDistance;
            effect.deathVFX = this.knifeDeathVFX;
            effect.Initialize(user, this.knifePrefab);
            return effect;
        }

        public override void Update()
        {
            base.Update();
            if ((Object) this.m_extantEffect != (Object) null && !this.m_extantEffect.IsActive)
                this.m_extantEffect = (KnifeShieldEffect) null;
            if (!((Object) this.m_secondaryEffect != (Object) null) || this.m_secondaryEffect.IsActive)
                return;
            this.m_secondaryEffect = (KnifeShieldEffect) null;
        }

        protected override void DoOnCooldownEffect(PlayerController user)
        {
            if ((Object) this.m_extantEffect != (Object) null && this.m_extantEffect.IsActive)
                this.m_extantEffect.ThrowShield();
            if (!((Object) this.m_secondaryEffect != (Object) null) || !this.m_secondaryEffect.IsActive)
                return;
            this.m_secondaryEffect.ThrowShield();
        }

        protected override void OnDestroy() => base.OnDestroy();
    }

