using UnityEngine;

using Dungeonator;

#nullable disable

public class ShieldPlayerBehavior : AttackBehaviorBase
    {
        public float BlankRadius = 5f;
        public float Cooldown = 10f;
        public string AnimationName = "block";
        public float AnimationTime = 0.5f;
        private float m_cooldownTimer;
        private GameObject BlankVFXPrefab;
        private SeekTargetBehavior m_seekBehavior;
        private float m_elapsed;
        private ShieldPlayerBehavior.State m_state;

        public override void Start()
        {
            base.Start();
            BehaviorSpeculator behaviorSpeculator = this.m_aiActor.behaviorSpeculator;
            for (int index = 0; index < behaviorSpeculator.MovementBehaviors.Count; ++index)
            {
                if (behaviorSpeculator.MovementBehaviors[index] is SeekTargetBehavior)
                    this.m_seekBehavior = behaviorSpeculator.MovementBehaviors[index] as SeekTargetBehavior;
            }
        }

        public override void Upkeep() => base.Upkeep();

        public override BehaviorResult Update()
        {
            int num = (int) base.Update();
            this.DecrementTimer(ref this.m_cooldownTimer);
            if (this.m_seekBehavior != null)
                this.m_seekBehavior.ExternalCooldownSource = (double) this.m_cooldownTimer <= 0.0;
            BehaviorResult behaviorResult = base.Update();
            if (behaviorResult != BehaviorResult.Continue)
                return behaviorResult;
            if ((double) this.m_cooldownTimer > 0.0 || !(bool) (Object) this.m_aiActor.TargetRigidbody || !this.CheckPlayerProjectileRadius())
                return BehaviorResult.Continue;
            this.m_state = ShieldPlayerBehavior.State.Charging;
            this.m_aiActor.ClearPath();
            this.m_aiActor.BehaviorOverridesVelocity = true;
            this.m_aiActor.BehaviorVelocity = Vector2.zero;
            this.m_updateEveryFrame = true;
            this.m_elapsed = 0.0f;
            return BehaviorResult.RunContinuous;
        }

        private Projectile GetNearestEnemyProjectile(PlayerController player)
        {
            Vector2 centerPosition = this.m_aiActor.CompanionOwner.CenterPosition;
            float num1 = this.BlankRadius * this.BlankRadius;
            Projectile nearestEnemyProjectile = (Projectile) null;
            float num2 = num1;
            for (int index = 0; index < StaticReferenceManager.AllProjectiles.Count; ++index)
            {
                Projectile allProjectile = StaticReferenceManager.AllProjectiles[index];
                if ((bool) (Object) allProjectile && allProjectile.collidesWithPlayer && (bool) (Object) allProjectile.specRigidbody && !(allProjectile.Owner is PlayerController))
                {
                    float sqrMagnitude = (centerPosition - allProjectile.specRigidbody.UnitCenter).sqrMagnitude;
                    if ((double) sqrMagnitude < (double) num1 && (double) sqrMagnitude < (double) num2)
                    {
                        nearestEnemyProjectile = allProjectile;
                        num2 = sqrMagnitude;
                    }
                }
            }
            return nearestEnemyProjectile;
        }

        private bool CheckPlayerProjectileRadius()
        {
            Vector2 centerPosition = this.m_aiActor.CompanionOwner.CenterPosition;
            float num = this.BlankRadius * this.BlankRadius;
            for (int index = 0; index < StaticReferenceManager.AllProjectiles.Count; ++index)
            {
                Projectile allProjectile = StaticReferenceManager.AllProjectiles[index];
                if ((bool) (Object) allProjectile && allProjectile.collidesWithPlayer && (bool) (Object) allProjectile.specRigidbody && !(allProjectile.Owner is PlayerController) && (double) (centerPosition - allProjectile.specRigidbody.UnitCenter).sqrMagnitude < (double) num)
                    return true;
            }
            return false;
        }

        private Vector2 GetTargetPoint(SpeculativeRigidbody targetRigidbody, Vector2 myCenter)
        {
            PixelCollider hitboxPixelCollider = targetRigidbody.HitboxPixelCollider;
            return BraveMathCollege.ClosestPointOnRectangle(myCenter, hitboxPixelCollider.UnitBottomLeft, hitboxPixelCollider.UnitDimensions);
        }

        public override ContinuousBehaviorResult ContinuousUpdate()
        {
            if (this.m_state == ShieldPlayerBehavior.State.Charging)
            {
                Projectile nearestEnemyProjectile = this.GetNearestEnemyProjectile(this.m_aiActor.CompanionOwner);
                this.m_state = ShieldPlayerBehavior.State.Leaping;
                if (!(bool) (Object) nearestEnemyProjectile)
                {
                    this.m_state = ShieldPlayerBehavior.State.Idle;
                    return ContinuousBehaviorResult.Finished;
                }
                Vector2 unitCenter = this.m_aiActor.specRigidbody.UnitCenter;
                Vector2 centerPosition = this.m_aiActor.CompanionOwner.CenterPosition;
                float num = Vector2.Distance(unitCenter, centerPosition);
                this.m_aiActor.ClearPath();
                this.m_aiActor.BehaviorOverridesVelocity = true;
                this.m_aiActor.BehaviorVelocity = (centerPosition - unitCenter).normalized * (num / 0.25f);
                float angle = this.m_aiActor.BehaviorVelocity.ToAngle();
                this.m_aiAnimator.LockFacingDirection = true;
                this.m_aiAnimator.FacingDirection = angle;
                this.m_aiActor.PathableTiles = CellTypes.FLOOR | CellTypes.PIT;
                this.m_aiActor.DoDustUps = false;
                if ((double) this.AnimationTime <= 0.0)
                    this.m_aiAnimator.PlayUntilFinished(this.AnimationName, true);
                else
                    this.m_aiAnimator.PlayForDuration(this.AnimationName, this.AnimationTime, true);
            }
            else if (this.m_state == ShieldPlayerBehavior.State.Leaping)
            {
                this.m_elapsed += this.m_deltaTime;
                if ((double) this.m_elapsed >= 0.25)
                {
                    this.m_cooldownTimer = this.Cooldown;
                    return ContinuousBehaviorResult.Finished;
                }
            }
            return ContinuousBehaviorResult.Continue;
        }

        public override void EndContinuousUpdate()
        {
            base.EndContinuousUpdate();
            this.DoMicroBlank();
            this.m_state = ShieldPlayerBehavior.State.Idle;
            this.m_aiActor.PathableTiles = CellTypes.FLOOR;
            this.m_aiActor.DoDustUps = true;
            this.m_aiActor.BehaviorOverridesVelocity = false;
            this.m_aiAnimator.LockFacingDirection = false;
            this.m_updateEveryFrame = false;
        }

        private void DoMicroBlank()
        {
            if ((Object) this.BlankVFXPrefab == (Object) null)
                this.BlankVFXPrefab = (GameObject) BraveResources.Load("Global VFX/BlankVFX_Ghost");
            int num = (int) AkSoundEngine.PostEvent("Play_OBJ_silenceblank_small_01", this.m_aiActor.gameObject);
            new GameObject("silencer").AddComponent<SilencerInstance>().TriggerSilencer(this.m_aiActor.specRigidbody.UnitCenter, 20f, this.BlankRadius, this.BlankVFXPrefab, 0.0f, 3f, 3f, 3f, 30f, 3f, 0.25f, this.m_aiActor.CompanionOwner, false);
        }

        public override bool IsReady() => true;

        public override float GetMinReadyRange() => -1f;

        public override float GetMaxRange() => float.MaxValue;

        private enum State
        {
            Idle,
            Charging,
            Leaping,
        }
    }

