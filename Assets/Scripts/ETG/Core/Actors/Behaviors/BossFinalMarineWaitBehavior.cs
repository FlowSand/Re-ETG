using FullInspector;

using Dungeonator;

#nullable disable

[InspectorDropdownName("Bosses/BossFinalMarine/WaitBehavior")]
public class BossFinalMarineWaitBehavior : AttackBehaviorBase
    {
        public float time;
        private float m_waitTimer;

        public override void Upkeep()
        {
            base.Upkeep();
            this.DecrementTimer(ref this.m_waitTimer);
        }

        public override BehaviorResult Update()
        {
            int num = (int) base.Update();
            BehaviorResult behaviorResult = base.Update();
            if (behaviorResult != BehaviorResult.Continue)
                return behaviorResult;
            this.m_waitTimer = this.time;
            this.m_updateEveryFrame = true;
            return BehaviorResult.RunContinuous;
        }

        public override ContinuousBehaviorResult ContinuousUpdate()
        {
            return (double) this.m_waitTimer <= 0.0 || this.m_aiActor.ParentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All).Count <= 1 ? ContinuousBehaviorResult.Finished : ContinuousBehaviorResult.Continue;
        }

        public override void EndContinuousUpdate()
        {
            base.EndContinuousUpdate();
            this.m_updateEveryFrame = false;
        }

        public override bool IsReady() => true;

        public override float GetMinReadyRange() => 0.0f;

        public override float GetMaxRange() => float.MaxValue;
    }

