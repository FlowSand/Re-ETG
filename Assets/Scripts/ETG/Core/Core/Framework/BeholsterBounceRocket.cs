using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

#nullable disable

public class BeholsterBounceRocket : BraveBehaviour
    {
        public float modifiedAccelertionFactor = 0.5f;
        public float modifiedAccelerationTime = 1f;
        public AnimationCurve modifiedAccelerationCurve;
        private RobotechProjectile m_projectile;
        private bool m_modifyingAcceleration;
        private float m_modifiedAccelerationTimer;
        private float m_startAcceleration;
        private float m_endAcceleration;
        private bool m_destroyed;

        public void Start()
        {
            this.m_projectile = this.GetComponent<RobotechProjectile>();
            if ((bool) (UnityEngine.Object) this.m_projectile)
                this.m_projectile.OnDestruction += new Action<Projectile>(this.OnDestruction);
            BounceProjModifier component = this.GetComponent<BounceProjModifier>();
            if (!(bool) (UnityEngine.Object) component || !(bool) (UnityEngine.Object) this.m_projectile)
                return;
            component.OnBounce += new System.Action(this.OnBounce);
            this.m_startAcceleration = this.m_projectile.angularAcceleration * this.modifiedAccelertionFactor;
            this.m_endAcceleration = this.m_projectile.angularAcceleration;
        }

        public void Update()
        {
            if (!this.m_modifyingAcceleration)
                return;
            this.m_modifiedAccelerationTimer += BraveTime.DeltaTime;
            this.m_projectile.angularAcceleration = Mathf.Lerp(this.m_startAcceleration, this.m_endAcceleration, this.modifiedAccelerationCurve.Evaluate(this.m_modifiedAccelerationTimer / this.modifiedAccelerationTime));
            if ((double) this.m_modifiedAccelerationTimer <= (double) this.modifiedAccelerationTime)
                return;
            this.m_modifyingAcceleration = false;
            this.m_projectile.angularAcceleration = this.m_endAcceleration;
        }

        private void OnBounce()
        {
            this.m_modifyingAcceleration = true;
            this.m_modifiedAccelerationTimer = 0.0f;
        }

        private void OnDestruction(Projectile source)
        {
            this.m_destroyed = true;
            BeholsterBounceRocket[] objectsOfType = UnityEngine.Object.FindObjectsOfType<BeholsterBounceRocket>();
            ExplosiveModifier component1 = this.GetComponent<ExplosiveModifier>();
            if (objectsOfType.Length <= 1 || !(bool) (UnityEngine.Object) component1)
                return;
            float pushRadius = component1.explosionData.pushRadius;
            if (this.specRigidbody.PrimaryPixelCollider.ColliderGenerationMode == PixelCollider.PixelColliderGeneration.Circle)
                pushRadius += PhysicsEngine.PixelToUnit(this.specRigidbody.PrimaryPixelCollider.ManualDiameter) / 2f;
            for (int index = 0; index < ((IEnumerable<BeholsterBounceRocket>) objectsOfType).Count<BeholsterBounceRocket>(); ++index)
            {
                BeholsterBounceRocket beholsterBounceRocket = objectsOfType[index];
                if (!beholsterBounceRocket.m_destroyed && (double) Vector2.Distance(this.specRigidbody.UnitCenter, beholsterBounceRocket.specRigidbody.UnitCenter) < (double) pushRadius)
                {
                    RobotechProjectile component2 = beholsterBounceRocket.GetComponent<RobotechProjectile>();
                    LinearCastResult lcr = LinearCastResult.Pool.Allocate();
                    lcr.Contact = (this.specRigidbody.UnitCenter + beholsterBounceRocket.specRigidbody.UnitCenter) * 0.5f;
                    lcr.Normal = this.specRigidbody.UnitCenter - beholsterBounceRocket.specRigidbody.UnitCenter;
                    lcr.OtherPixelCollider = this.specRigidbody.PrimaryPixelCollider;
                    lcr.MyPixelCollider = beholsterBounceRocket.specRigidbody.PrimaryPixelCollider;
                    component2.ForceCollision(this.specRigidbody, lcr);
                    LinearCastResult.Pool.Free(ref lcr);
                }
            }
        }

        protected override void OnDestroy() => base.OnDestroy();
    }

