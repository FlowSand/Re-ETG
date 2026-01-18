using System;
using System.Collections.Generic;

using UnityEngine;

#nullable disable

[Serializable]
public class ExplosionData
    {
        public bool useDefaultExplosion;
        public bool doDamage = true;
        public bool forceUseThisRadius;
        [ShowInInspectorIf("doDamage", true)]
        public float damageRadius = 4.5f;
        [ShowInInspectorIf("doDamage", true)]
        public float damageToPlayer = 0.5f;
        [ShowInInspectorIf("doDamage", true)]
        public float damage = 25f;
        public bool breakSecretWalls;
        [ShowInInspectorIf("breakSecretWalls", true)]
        public float secretWallsRadius = 4.5f;
        public bool forcePreventSecretWallDamage;
        public bool doDestroyProjectiles = true;
        public bool doForce = true;
        [ShowInInspectorIf("doForce", true)]
        public float pushRadius = 6f;
        [ShowInInspectorIf("doForce", true)]
        public float force = 100f;
        [ShowInInspectorIf("doForce", true)]
        public float debrisForce = 50f;
        [ShowInInspectorIf("doForce", true)]
        public bool preventPlayerForce;
        public float explosionDelay = 0.1f;
        public bool usesComprehensiveDelay;
        [ShowInInspectorIf("usesComprehensiveDelay", false)]
        public float comprehensiveDelay;
        public GameObject effect;
        public bool doScreenShake = true;
        [ShowInInspectorIf("doScreenShake", true)]
        public ScreenShakeSettings ss;
        public bool doStickyFriction = true;
        public bool doExplosionRing = true;
        public bool isFreezeExplosion;
        [ShowInInspectorIf("isFreezeExplosion", false)]
        public float freezeRadius = 5f;
        public GameActorFreezeEffect freezeEffect;
        public bool playDefaultSFX = true;
        public bool IsChandelierExplosion;
        public bool rotateEffectToNormal;
        [HideInInspector]
        public List<SpeculativeRigidbody> ignoreList;
        [HideInInspector]
        public GameObject overrideRangeIndicatorEffect;

        public void CopyFrom(ExplosionData source)
        {
            this.doDamage = source.doDamage;
            this.forceUseThisRadius = source.forceUseThisRadius;
            this.damageRadius = source.damageRadius;
            this.damageToPlayer = source.damageToPlayer;
            this.damage = source.damage;
            this.breakSecretWalls = source.breakSecretWalls;
            this.secretWallsRadius = source.secretWallsRadius;
            this.doDestroyProjectiles = source.doDestroyProjectiles;
            this.doForce = source.doForce;
            this.pushRadius = source.pushRadius;
            this.force = source.force;
            this.debrisForce = source.debrisForce;
            this.explosionDelay = source.explosionDelay;
            this.effect = source.effect;
            this.doScreenShake = source.doScreenShake;
            this.ss = source.ss;
            this.doStickyFriction = source.doStickyFriction;
            this.doExplosionRing = source.doExplosionRing;
            this.isFreezeExplosion = source.isFreezeExplosion;
            this.freezeRadius = source.freezeRadius;
            this.freezeEffect = source.freezeEffect;
            this.playDefaultSFX = source.playDefaultSFX;
            this.IsChandelierExplosion = source.IsChandelierExplosion;
            this.ignoreList = new List<SpeculativeRigidbody>();
        }

        public float GetDefinedDamageRadius()
        {
            if (this.forceUseThisRadius || !(bool) (UnityEngine.Object) this.effect)
                return this.damageRadius;
            ExplosionRadiusDefiner component = this.effect.GetComponent<ExplosionRadiusDefiner>();
            return (bool) (UnityEngine.Object) component ? component.Radius : this.damageRadius;
        }
    }

