using UnityEngine;

#nullable disable

public class ResourcefulRatChamberBehavior : OverrideBehaviorBase
    {
        public float HealthThresholdPhaseTwo = 0.66f;
        public float HealthThresholdPhaseThree = 0.33f;
        private bool m_isActive;
        private int m_currentPhase = 1;

        public override void Start()
        {
            base.Start();
            this.m_updateEveryFrame = true;
            this.m_ignoreGlobalCooldown = true;
        }

        public override void Upkeep() => base.Upkeep();

        private bool ReadyForNextPhase()
        {
            return this.m_currentPhase == 1 && (double) this.m_aiActor.healthHaver.GetCurrentHealthPercentage() < (double) this.HealthThresholdPhaseTwo || this.m_currentPhase == 2 && (double) this.m_aiActor.healthHaver.GetCurrentHealthPercentage() < (double) this.HealthThresholdPhaseThree;
        }

        public override bool OverrideOtherBehaviors() => this.ReadyForNextPhase() || this.m_isActive;

        public override BehaviorResult Update()
        {
            int num = (int) base.Update();
            if (!this.ReadyForNextPhase())
                return BehaviorResult.Continue;
            ++this.m_currentPhase;
            this.m_aiActor.MovementModifiers += new GameActor.MovementModifier(this.m_aiActor_MovementModifiers);
            this.m_aiActor.BehaviorOverridesVelocity = false;
            this.m_aiAnimator.LockFacingDirection = false;
            this.m_aiActor.healthHaver.IsVulnerable = false;
            this.m_aiActor.specRigidbody.CollideWithOthers = false;
            Vector2 vector2_1 = this.m_aiActor.ParentRoom.area.basePosition.ToVector2() + this.m_aiActor.ParentRoom.area.dimensions.ToVector2().WithY(0.0f) / 2f;
            Vector2 vector2_2 = new Vector2(0.0f, 35f);
            if (this.m_currentPhase == 3)
                vector2_2 = new Vector2(0.0f, 52f);
            this.m_aiActor.PathfindToPosition(vector2_1 + vector2_2);
            this.m_isActive = true;
            return BehaviorResult.RunContinuous;
        }

        private void m_aiActor_MovementModifiers(ref Vector2 volundaryVel, ref Vector2 involuntaryVel)
        {
            volundaryVel *= 4f;
        }

        public override bool IsOverridable() => false;

        public override ContinuousBehaviorResult ContinuousUpdate()
        {
            int num = (int) base.ContinuousUpdate();
            if (!this.m_aiActor.PathComplete)
                return ContinuousBehaviorResult.Continue;
            this.m_aiActor.MovementModifiers -= new GameActor.MovementModifier(this.m_aiActor_MovementModifiers);
            this.m_aiActor.healthHaver.IsVulnerable = true;
            this.m_aiActor.specRigidbody.CollideWithOthers = true;
            this.m_isActive = false;
            return ContinuousBehaviorResult.Finished;
        }

        public override void Destroy() => base.Destroy();
    }

