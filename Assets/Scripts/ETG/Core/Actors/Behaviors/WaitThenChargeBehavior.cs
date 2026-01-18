using UnityEngine;

#nullable disable

public class WaitThenChargeBehavior : MovementBehaviorBase
    {
        public float Delay;
        private float m_delayTimer;
        private bool m_isCharging;
        private float m_chargeDirection;
        private Vector2 m_center;

        public override void Start()
        {
            base.Start();
            this.m_delayTimer = this.Delay;
        }

        public override void Upkeep()
        {
            base.Upkeep();
            this.DecrementTimer(ref this.m_delayTimer);
        }

        public override BehaviorResult Update()
        {
            if (this.m_isCharging)
            {
                this.m_aiActor.BehaviorOverridesVelocity = true;
                this.m_aiActor.BehaviorVelocity = BraveMathCollege.DegreesToVector(this.m_chargeDirection, this.m_aiActor.MovementSpeed);
            }
            else if ((double) this.m_delayTimer <= 0.0)
            {
                this.m_isCharging = true;
                this.m_chargeDirection = !(bool) (Object) this.m_aiActor.TargetRigidbody ? Random.Range(0.0f, 360f) : (this.m_aiActor.behaviorSpeculator.TargetRigidbody.GetUnitCenter(ColliderType.HitBox) - this.m_aiActor.specRigidbody.GetUnitCenter(ColliderType.HitBox)).ToAngle();
                this.m_aiActor.BehaviorOverridesVelocity = true;
                this.m_aiActor.BehaviorVelocity = BraveMathCollege.DegreesToVector(this.m_chargeDirection, this.m_aiActor.MovementSpeed);
            }
            return BehaviorResult.SkipRemainingClassBehaviors;
        }
    }

