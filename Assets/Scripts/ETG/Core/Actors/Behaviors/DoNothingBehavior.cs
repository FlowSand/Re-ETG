using UnityEngine;

#nullable disable

public class DoNothingBehavior : BasicAttackBehavior
    {
        public float DoNothingTimer = 2f;
        private float m_doNothingTimer;
        private bool m_hasDoneNothing;

        public override void Start() => base.Start();

        public override void Upkeep()
        {
            base.Upkeep();
            this.DecrementTimer(ref this.m_doNothingTimer);
        }

        public override BehaviorResult Update()
        {
            int num = (int) base.Update();
            if (this.m_hasDoneNothing)
                return BehaviorResult.Continue;
            this.m_doNothingTimer = this.DoNothingTimer;
            if ((bool) (Object) this.m_aiActor)
                this.m_aiActor.ClearPath();
            return BehaviorResult.RunContinuous;
        }

        public override ContinuousBehaviorResult ContinuousUpdate()
        {
            int num = (int) base.ContinuousUpdate();
            if ((double) this.m_doNothingTimer > 0.0)
                return ContinuousBehaviorResult.Continue;
            this.m_hasDoneNothing = true;
            return ContinuousBehaviorResult.Finished;
        }
    }

