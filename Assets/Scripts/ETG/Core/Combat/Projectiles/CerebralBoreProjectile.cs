using System;
using System.Collections.Generic;

using UnityEngine;

using Dungeonator;

#nullable disable

public class CerebralBoreProjectile : Projectile
    {
        public ExplosionData explosionData;
        public float boreTime = 2f;
        public AnimationCurve boreCurve;
        public ParticleSystem SparksSystem;
        private AIActor m_targetEnemy;
        private bool m_hasExploded;
        private CerebralBoreProjectile.BoreMotionType m_currentMotionType;
        private Vector2 m_startPosition;
        private Vector2 m_initialAimPoint;
        private float m_currentBezierPoint;
        private HashSet<SpeculativeRigidbody> m_rigidbodiesDamagedInFlight = new HashSet<SpeculativeRigidbody>();
        private Vector2 m_targetTrackingPoint;
        private float m_bezierUpdateTimer;
        private float m_elapsedBore;

        public override void Start()
        {
            base.Start();
            this.specRigidbody.OnPreRigidbodyCollision += new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.ProcessPreCollision);
            this.AcquireTarget();
        }

        private void ProcessPreCollision(
            SpeculativeRigidbody myRigidbody,
            PixelCollider myPixelCollider,
            SpeculativeRigidbody otherRigidbody,
            PixelCollider otherPixelCollider)
        {
            if (this.m_currentMotionType == CerebralBoreProjectile.BoreMotionType.TRACKING)
            {
                if (!((UnityEngine.Object) otherRigidbody.aiActor != (UnityEngine.Object) null))
                    return;
                if ((UnityEngine.Object) otherRigidbody.aiActor != (UnityEngine.Object) this.m_targetEnemy && !this.m_rigidbodiesDamagedInFlight.Contains(otherRigidbody))
                {
                    bool killedTarget = false;
                    int num = (int) this.HandleDamage(otherRigidbody, otherPixelCollider, out killedTarget, (PlayerController) null);
                    this.m_rigidbodiesDamagedInFlight.Add(otherRigidbody);
                }
                PhysicsEngine.SkipCollision = true;
            }
            else
            {
                if (this.m_currentMotionType != CerebralBoreProjectile.BoreMotionType.BORING)
                    return;
                PhysicsEngine.SkipCollision = true;
            }
        }

        protected void AcquireTarget()
        {
            this.m_startPosition = this.transform.position.XY();
            this.m_initialAimPoint = this.m_startPosition + this.transform.right.XY() * 3f;
            this.m_currentBezierPoint = 0.0f;
            Func<AIActor, bool> isValid = (Func<AIActor, bool>) (targ => !targ.UniquePlayerTargetFlag);
            if (this.Owner is PlayerController)
            {
                PlayerController owner = this.Owner as PlayerController;
                if (owner.CurrentRoom == null)
                    return;
                List<AIActor> activeEnemies = owner.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
                if (activeEnemies == null)
                    return;
                this.m_targetEnemy = BraveUtility.GetClosestToPosition<AIActor>(activeEnemies, owner.unadjustedAimPoint.XY(), isValid);
            }
            else if ((bool) (UnityEngine.Object) this.Owner)
            {
                List<AIActor> activeEnemies = this.Owner.GetAbsoluteParentRoom().GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
                if (activeEnemies == null)
                    return;
                this.m_targetEnemy = BraveUtility.GetClosestToPosition<AIActor>(activeEnemies, this.transform.position.XY(), isValid);
            }
            if (!((UnityEngine.Object) this.m_targetEnemy != (UnityEngine.Object) null))
                return;
            this.m_targetEnemy.UniquePlayerTargetFlag = true;
        }

        protected override void Move()
        {
            if ((UnityEngine.Object) this.m_targetEnemy == (UnityEngine.Object) null || !(bool) (UnityEngine.Object) this.m_targetEnemy)
                this.AcquireTarget();
            switch (this.m_currentMotionType)
            {
                case CerebralBoreProjectile.BoreMotionType.TRACKING:
                    this.HandleTracking();
                    break;
                case CerebralBoreProjectile.BoreMotionType.BORING:
                    this.HandleBoring();
                    break;
            }
        }

        protected void HandleBoring()
        {
            if ((UnityEngine.Object) this.m_targetEnemy != (UnityEngine.Object) null)
                this.m_targetEnemy.UniquePlayerTargetFlag = false;
            this.m_elapsedBore += BraveTime.DeltaTime;
            float time = Mathf.Clamp01(this.m_elapsedBore / this.boreTime);
            if ((double) this.m_elapsedBore < (double) this.boreTime && (UnityEngine.Object) this.m_targetEnemy != (UnityEngine.Object) null && (bool) (UnityEngine.Object) this.m_targetEnemy && this.m_targetEnemy.healthHaver.IsAlive && this.m_targetEnemy.specRigidbody.CollideWithOthers)
            {
                if (!this.m_targetEnemy.healthHaver.IsBoss)
                {
                    this.m_targetEnemy.ClearPath();
                    if (this.m_targetEnemy.behaviorSpeculator.IsInterruptable)
                        this.m_targetEnemy.behaviorSpeculator.Interrupt();
                    this.m_targetEnemy.behaviorSpeculator.Stun(1f);
                }
                this.specRigidbody.Velocity = (this.m_targetEnemy.specRigidbody.HitboxPixelCollider.UnitTopCenter + new Vector2(0.0f, this.boreCurve.Evaluate(time)) - this.transform.PositionVector2()) / BraveTime.DeltaTime;
                this.LastVelocity = this.specRigidbody.Velocity;
                this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, BraveMathCollege.Atan2Degrees(Vector2.down));
            }
            else
            {
                if ((bool) (UnityEngine.Object) this.m_targetEnemy && this.m_targetEnemy.healthHaver.IsAlive)
                {
                    this.m_targetEnemy.healthHaver.ApplyDamage(this.ModifiedDamage, Vector2.down, this.OwnerName);
                    Exploder.Explode(this.specRigidbody.UnitCenter.ToVector3ZUp(), this.explosionData, Vector2.up);
                    this.m_hasExploded = true;
                }
                this.DieInAir();
            }
        }

        protected void HandleTracking()
        {
            float a = this.baseData.speed;
            if ((UnityEngine.Object) this.m_targetEnemy != (UnityEngine.Object) null)
            {
                this.m_bezierUpdateTimer -= BraveTime.DeltaTime;
                if ((double) this.m_bezierUpdateTimer <= 0.0)
                {
                    this.m_bezierUpdateTimer = 0.1f;
                    this.m_targetTrackingPoint = this.m_targetEnemy.sprite.WorldTopCenter;
                }
                Vector2 p1 = this.m_startPosition + (this.m_initialAimPoint - this.m_startPosition).normalized * 5f;
                Vector2 vector2 = this.m_targetTrackingPoint + Vector2.up * 4f;
                IntVector2 intVector2 = vector2.ToIntVector2(VectorConversions.Floor);
                Func<IntVector2, bool> func = (Func<IntVector2, bool>) (pos => GameManager.Instance.Dungeon.data[pos] == null || GameManager.Instance.Dungeon.data[pos].type == CellType.WALL);
                if (func(intVector2 + IntVector2.Down) || func(intVector2 + IntVector2.Down * 2))
                    vector2 = this.m_targetTrackingPoint + Vector2.down;
                float num = BraveMathCollege.EstimateBezierPathLength(this.m_startPosition, p1, vector2, this.m_targetTrackingPoint, 20) / this.baseData.speed;
                float t = this.m_currentBezierPoint + BraveTime.DeltaTime * 2f / num;
                this.m_currentBezierPoint += BraveTime.DeltaTime / num;
                if ((double) this.m_currentBezierPoint >= 1.0)
                {
                    this.specRigidbody.CollideWithTileMap = false;
                    this.SparksSystem.gameObject.SetActive(true);
                    this.m_currentMotionType = CerebralBoreProjectile.BoreMotionType.BORING;
                }
                Vector2 v = BraveMathCollege.CalculateBezierPoint(t, this.m_startPosition, p1, vector2, this.m_targetTrackingPoint) - this.transform.PositionVector2();
                a = Mathf.Min(a, v.magnitude / BraveTime.DeltaTime);
                this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, BraveMathCollege.Atan2Degrees(v).Quantize(3f));
            }
            this.specRigidbody.Velocity = (Vector2) (this.transform.right * a);
            this.LastVelocity = this.specRigidbody.Velocity;
        }

        protected override void OnDestroy()
        {
            if (!this.m_hasExploded && !GameManager.Instance.IsLoadingLevel)
                Exploder.Explode(this.specRigidbody.UnitCenter.ToVector3ZUp(), this.explosionData, Vector2.up);
            base.OnDestroy();
            int num = (int) AkSoundEngine.PostEvent("Stop_WPN_cerebralbore_loop_01", this.gameObject);
            if (!((UnityEngine.Object) this.m_targetEnemy != (UnityEngine.Object) null))
                return;
            this.m_targetEnemy.UniquePlayerTargetFlag = false;
        }

        private enum BoreMotionType
        {
            TRACKING,
            BORING,
        }
    }

