using FullInspector;
using UnityEngine;

#nullable disable

[InspectorDropdownName("Bosses/Bashellisk/SeekTargetBehavior")]
public class BashelliskSeekTargetBehavior : RangedMovementBehavior
    {
        public float turnTime = 1f;
        public bool slither;
        public float slitherPeriod;
        public float slitherMagnitude;
        public float minPickupDelay;
        public float maxPickupDelay;
        public float snapDist;
        public float snapTurnTime;
        public bool snapSlither;
        private BashelliskSeekTargetBehavior.SeekState m_state;
        private BashelliskHeadController m_head;
        private Vector2 m_targetCenter;
        private BashelliskBodyPickupController m_desiredPickup;
        private bool m_snapToTarget;
        private float m_slitherCounter;
        private float m_direction = -90f;
        private float m_slitherDirection;
        private float m_angularVelocity;
        private float m_pickupConsiderationTimer;

        public override void Start()
        {
            base.Start();
            this.m_head = this.m_aiActor.GetComponent<BashelliskHeadController>();
            this.m_updateEveryFrame = true;
            if (!TurboModeController.IsActive)
                return;
            this.turnTime /= TurboModeController.sEnemyMovementSpeedMultiplier;
            this.snapTurnTime /= TurboModeController.sEnemyMovementSpeedMultiplier;
        }

        public override void Upkeep()
        {
            base.Upkeep();
            this.m_aiActor.BehaviorOverridesVelocity = true;
            if (!this.m_head.IsMidPickup)
                this.m_aiActor.BehaviorVelocity = Vector2.zero;
            this.m_aiAnimator.LockFacingDirection = true;
            this.DecrementTimer(ref this.m_pickupConsiderationTimer);
            this.m_slitherCounter += this.m_deltaTime * this.m_aiActor.behaviorSpeculator.CooldownScale;
        }

        public override BehaviorResult Update()
        {
            this.UpdateState();
            if (this.m_head.IsMidPickup)
                return BehaviorResult.Continue;
            if (this.m_head.ReinitMovementDirection)
            {
                this.m_direction = this.m_head.aiAnimator.FacingDirection;
                this.m_head.ReinitMovementDirection = false;
            }
            this.m_direction = Mathf.SmoothDampAngle(this.m_direction, (this.m_targetCenter - this.m_aiActor.specRigidbody.UnitCenter).ToAngle(), ref this.m_angularVelocity, !this.m_snapToTarget ? this.turnTime : this.snapTurnTime);
            if ((!this.m_snapToTarget ? (this.slither ? 1 : 0) : (this.snapSlither ? 1 : 0)) != 0)
                this.m_slitherDirection = Mathf.Sin(this.m_slitherCounter * 3.14159274f / this.slitherPeriod) * this.slitherMagnitude;
            this.m_aiActor.BehaviorVelocity = BraveMathCollege.DegreesToVector(this.m_direction + this.m_slitherDirection, this.m_aiActor.MovementSpeed);
            return BehaviorResult.Continue;
        }

        private BashelliskSeekTargetBehavior.SeekState State
        {
            get => this.m_state;
            set
            {
                this.EndState(this.m_state);
                this.m_state = value;
                this.BeginState(this.m_state);
            }
        }

        private void BeginState(BashelliskSeekTargetBehavior.SeekState state)
        {
            switch (state)
            {
                case BashelliskSeekTargetBehavior.SeekState.ConsideringPickup:
                    this.m_pickupConsiderationTimer = Random.Range(this.minPickupDelay, this.maxPickupDelay);
                    break;
                case BashelliskSeekTargetBehavior.SeekState.SeekPickup:
                    this.m_head.CanPickup = true;
                    this.m_desiredPickup = this.m_head.AvailablePickups.GetByIndexSlow(Random.Range(0, this.m_head.AvailablePickups.Count)).Value;
                    if (!(bool) (Object) this.m_desiredPickup)
                        break;
                    this.m_targetCenter = this.m_desiredPickup.specRigidbody.GetUnitCenter(ColliderType.HitBox);
                    break;
            }
        }

        private void UpdateState()
        {
            this.m_snapToTarget = false;
            if (this.State == BashelliskSeekTargetBehavior.SeekState.SeekPlayer)
            {
                if ((bool) (Object) this.m_aiActor.TargetRigidbody)
                    this.m_targetCenter = this.m_aiActor.TargetRigidbody.GetUnitCenter(ColliderType.HitBox);
                if (this.m_head.AvailablePickups.Count <= 0)
                    return;
                this.State = BashelliskSeekTargetBehavior.SeekState.ConsideringPickup;
                this.UpdateState();
            }
            else if (this.State == BashelliskSeekTargetBehavior.SeekState.ConsideringPickup)
            {
                if (this.m_head.AvailablePickups.Count == 0)
                {
                    this.State = BashelliskSeekTargetBehavior.SeekState.SeekPlayer;
                    this.UpdateState();
                }
                else
                {
                    if ((double) this.m_pickupConsiderationTimer > 0.0)
                        return;
                    this.State = BashelliskSeekTargetBehavior.SeekState.SeekPickup;
                    this.UpdateState();
                }
            }
            else
            {
                if (this.State != BashelliskSeekTargetBehavior.SeekState.SeekPickup)
                    return;
                if ((bool) (Object) this.m_desiredPickup && this.m_desiredPickup.aiActor.CanTargetPlayers)
                {
                    if ((double) Vector2.Distance(this.m_aiActor.specRigidbody.UnitCenter, this.m_targetCenter) >= (double) this.snapDist)
                        return;
                    this.m_snapToTarget = true;
                }
                else
                {
                    this.State = BashelliskSeekTargetBehavior.SeekState.SeekPlayer;
                    this.UpdateState();
                }
            }
        }

        private void EndState(BashelliskSeekTargetBehavior.SeekState state)
        {
            if (state != BashelliskSeekTargetBehavior.SeekState.SeekPickup)
                return;
            this.m_head.CanPickup = false;
        }

        private enum SeekState
        {
            SeekPlayer,
            ConsideringPickup,
            SeekPickup,
        }
    }

