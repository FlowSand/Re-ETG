using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using UnityEngine;

#nullable disable

    public abstract class BeamController : BraveBehaviour
    {
        public const float c_chanceTick = 1f;
[TogglesProperty("knockbackStrength", "Knockback")]
        public bool knocksShooterBack;
[HideInInspector]
        public float knockbackStrength = 10f;
[TogglesProperty("chargeDelay", "Charge Delay")]
        public bool usesChargeDelay;
[HideInInspector]
        public float chargeDelay;
        public float statusEffectChance = 1f;
        public float statusEffectAccumulateMultiplier = 1f;
[NonSerialized]
        public List<SpeculativeRigidbody> IgnoreRigidbodes = new List<SpeculativeRigidbody>();
[NonSerialized]
        public List<Tuple<SpeculativeRigidbody, float>> TimedIgnoreRigidbodies = new List<Tuple<SpeculativeRigidbody, float>>();
        private SpeculativeRigidbody[] m_ignoreRigidbodiesList;
        protected float m_chanceTick = -1000f;

        public GameActor Owner { get; set; }

        public Gun Gun { get; set; }

        public bool HitsPlayers { get; set; }

        public bool HitsEnemies { get; set; }

        public Vector2 Origin { get; set; }

        public Vector2 Direction { get; set; }

        public float DamageModifier { get; set; }

        public abstract bool ShouldUseAmmo { get; }

        public float ChanceBasedHomingRadius { get; set; }

        public float ChanceBasedHomingAngularVelocity { get; set; }

        public bool ChanceBasedShadowBullet { get; set; }

        public bool IsReflectedBeam { get; set; }

        protected override void OnDestroy() => base.OnDestroy();

        public abstract void LateUpdatePosition(Vector3 origin);

        public abstract void CeaseAttack();

        public abstract void DestroyBeam();

        public abstract void AdjustPlayerBeamTint(Color targetTintColor, int priority, float lerpTime = 0.0f);

        protected bool HandleChanceTick()
        {
            bool flag = false;
            if ((double) this.m_chanceTick <= 0.0)
            {
                this.ChanceBasedHomingRadius = 0.0f;
                this.ChanceBasedHomingAngularVelocity = 0.0f;
                this.ChanceBasedShadowBullet = false;
                (this.Owner as PlayerController).DoPostProcessBeamChanceTick(this);
                ++this.m_chanceTick;
                flag = true;
            }
            this.m_chanceTick -= BraveTime.DeltaTime;
            return flag;
        }

        protected SpeculativeRigidbody[] GetIgnoreRigidbodies()
        {
            PlayerController owner = this.Owner as PlayerController;
            int length = this.IgnoreRigidbodes.Count + this.TimedIgnoreRigidbodies.Count;
            if ((bool) (UnityEngine.Object) this.Owner && (bool) (UnityEngine.Object) this.Owner.specRigidbody)
                ++length;
            if ((bool) (UnityEngine.Object) owner && owner.IsInMinecart)
                ++length;
            if (this.m_ignoreRigidbodiesList == null || this.m_ignoreRigidbodiesList.Length != length)
                this.m_ignoreRigidbodiesList = new SpeculativeRigidbody[length];
            int num1 = 0;
            for (int index = 0; index < this.IgnoreRigidbodes.Count; ++index)
                this.m_ignoreRigidbodiesList[num1++] = this.IgnoreRigidbodes[index];
            for (int index = 0; index < this.TimedIgnoreRigidbodies.Count; ++index)
                this.m_ignoreRigidbodiesList[num1++] = this.TimedIgnoreRigidbodies[index].First;
            if ((bool) (UnityEngine.Object) this.Owner && (bool) (UnityEngine.Object) this.Owner.specRigidbody)
                this.m_ignoreRigidbodiesList[num1++] = this.Owner.specRigidbody;
            if ((bool) (UnityEngine.Object) owner && owner.IsInMinecart)
            {
                SpeculativeRigidbody[] ignoreRigidbodiesList = this.m_ignoreRigidbodiesList;
                int index = num1;
                int num2 = index + 1;
                SpeculativeRigidbody specRigidbody = owner.currentMineCart.specRigidbody;
                ignoreRigidbodiesList[index] = specRigidbody;
            }
            return this.m_ignoreRigidbodiesList;
        }

        public static BeamController FreeFireBeam(
            Projectile projectileToSpawn,
            PlayerController source,
            float targetAngle,
            float duration,
            bool skipChargeTime = false)
        {
            GameObject gameObject = SpawnManager.SpawnProjectile(projectileToSpawn.gameObject, (Vector3) source.CenterPosition, Quaternion.identity);
            gameObject.GetComponent<Projectile>().Owner = (GameActor) source;
            BeamController component = gameObject.GetComponent<BeamController>();
            if (skipChargeTime)
            {
                component.chargeDelay = 0.0f;
                component.usesChargeDelay = false;
            }
            component.Owner = (GameActor) source;
            component.HitsPlayers = false;
            component.HitsEnemies = true;
            Vector3 vector = (Vector3) BraveMathCollege.DegreesToVector(targetAngle);
            component.Direction = (Vector2) vector;
            component.Origin = source.CenterPosition;
            GameManager.Instance.Dungeon.StartCoroutine(BeamController.HandleFiringBeam(component, source, targetAngle, duration));
            return component;
        }

[DebuggerHidden]
        private static IEnumerator HandleFiringBeam(
            BeamController beam,
            PlayerController source,
            float targetAngle,
            float duration)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new BeamController__HandleFiringBeamc__Iterator0()
            {
                duration = duration,
                source = source,
                beam = beam
            };
        }
    }

