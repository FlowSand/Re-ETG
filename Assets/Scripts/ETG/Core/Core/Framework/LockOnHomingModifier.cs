using System;

using UnityEngine;

#nullable disable

public class LockOnHomingModifier : BraveBehaviour
    {
        public float HomingRadius = 2f;
        public float AngularVelocity = 180f;
        public GameObject LockOnVFX;
        [NonSerialized]
        public AIActor lockOnTarget;
        protected Projectile m_projectile;

        private void Start()
        {
            if (!(bool) (UnityEngine.Object) this.m_projectile)
            {
                this.m_projectile = this.GetComponent<Projectile>();
                if (!(bool) (UnityEngine.Object) this.lockOnTarget && (bool) (UnityEngine.Object) this.m_projectile.PossibleSourceGun && (bool) (UnityEngine.Object) this.m_projectile.PossibleSourceGun.LastLaserSightEnemy)
                    this.lockOnTarget = this.m_projectile.PossibleSourceGun.LastLaserSightEnemy;
            }
            this.m_projectile.ModifyVelocity += new Func<Vector2, Vector2>(this.ModifyVelocity);
        }

        public void AssignTargetManually(AIActor enemy) => this.lockOnTarget = enemy;

        public void AssignProjectile(Projectile source) => this.m_projectile = source;

        private Vector2 ModifyVelocity(Vector2 inVel)
        {
            Vector2 vector2 = inVel;
            if (!(bool) (UnityEngine.Object) this.lockOnTarget)
                return inVel;
            Vector2 vector = this.lockOnTarget.CenterPosition - (!(bool) (UnityEngine.Object) this.sprite ? this.transform.position.XY() : this.sprite.WorldCenter);
            AIActor lockOnTarget = this.lockOnTarget;
            float num1 = Mathf.Sqrt(vector.sqrMagnitude);
            if ((double) num1 < (double) this.HomingRadius && (UnityEngine.Object) lockOnTarget != (UnityEngine.Object) null)
            {
                float num2 = (float) (1.0 - (double) num1 / (double) this.HomingRadius);
                float angle1 = vector.ToAngle();
                float angle2 = inVel.ToAngle();
                float maxDelta = this.AngularVelocity * num2 * this.m_projectile.LocalDeltaTime;
                float num3 = Mathf.MoveTowardsAngle(angle2, angle1, maxDelta);
                if (this.m_projectile is HelixProjectile)
                {
                    (this.m_projectile as HelixProjectile).AdjustRightVector(num3 - angle2);
                }
                else
                {
                    if (this.m_projectile.shouldRotate)
                        this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, num3);
                    vector2 = BraveMathCollege.DegreesToVector(num3, inVel.magnitude);
                }
                if (this.m_projectile.OverrideMotionModule != null)
                    this.m_projectile.OverrideMotionModule.AdjustRightVector(num3 - angle2);
            }
            return vector2 == Vector2.zero ? inVel : vector2;
        }

        protected override void OnDestroy()
        {
            if ((bool) (UnityEngine.Object) this.m_projectile)
                this.m_projectile.ModifyVelocity -= new Func<Vector2, Vector2>(this.ModifyVelocity);
            base.OnDestroy();
        }
    }

