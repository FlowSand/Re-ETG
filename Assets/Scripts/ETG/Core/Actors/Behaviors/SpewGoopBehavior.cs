using System;

using UnityEngine;

#nullable disable

public class SpewGoopBehavior : BasicAttackBehavior
    {
        public string spewAnimation;
        public Transform goopPoint;
        public GoopDefinition goopToUse;
        public float goopConeLength = 5f;
        public float goopConeArc = 45f;
        public AnimationCurve goopCurve;
        public float goopDuration = 0.5f;
        private float m_goopTimer;
        private bool m_hasGooped;

        public override void Start()
        {
            base.Start();
            this.m_aiActor.spriteAnimator.AnimationEventTriggered += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.AnimationEventTriggered);
        }

        public override void Upkeep()
        {
            base.Upkeep();
            this.DecrementTimer(ref this.m_goopTimer);
        }

        public override BehaviorResult Update()
        {
            BehaviorResult behaviorResult = base.Update();
            if (behaviorResult != BehaviorResult.Continue)
                return behaviorResult;
            if (!this.IsReady())
                return BehaviorResult.Continue;
            this.m_aiActor.ClearPath();
            this.m_aiActor.BehaviorVelocity = Vector2.zero;
            this.m_hasGooped = false;
            this.m_aiAnimator.PlayUntilFinished(this.spewAnimation);
            this.m_updateEveryFrame = true;
            return BehaviorResult.RunContinuous;
        }

        public override ContinuousBehaviorResult ContinuousUpdate()
        {
            int num = (int) base.ContinuousUpdate();
            return !this.m_hasGooped || (double) this.m_goopTimer > 0.0 ? ContinuousBehaviorResult.Continue : ContinuousBehaviorResult.Finished;
        }

        public override void EndContinuousUpdate()
        {
            base.EndContinuousUpdate();
            this.m_aiAnimator.EndAnimationIf(this.spewAnimation);
            this.m_updateEveryFrame = false;
            this.UpdateCooldowns();
        }

        private void AnimationEventTriggered(
            tk2dSpriteAnimator spriteAnimator,
            tk2dSpriteAnimationClip clip,
            int frame)
        {
            if (this.m_hasGooped || !(clip.GetFrame(frame).eventInfo == "spew"))
                return;
            DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(this.goopToUse).TimedAddGoopArc((Vector2) this.goopPoint.transform.position, this.goopConeLength, this.goopConeArc, (Vector2) this.goopPoint.transform.right, this.goopDuration, this.goopCurve);
            this.m_goopTimer = this.goopDuration;
            this.m_hasGooped = true;
        }
    }

