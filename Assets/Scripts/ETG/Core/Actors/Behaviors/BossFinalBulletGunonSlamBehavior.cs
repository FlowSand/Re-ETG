using System;
using System.Collections.Generic;

using FullInspector;
using UnityEngine;

#nullable disable

[InspectorDropdownName("Bosses/BossFinalBullet/GunonSlamBehavior")]
public class BossFinalBulletGunonSlamBehavior : BasicAttackBehavior
    {
        public int numTraps = 6;
        [InspectorCategory("Visuals")]
        public string anim;
        private List<PitTrapController> m_traps;
        private bool m_slammed;

        public override void Start()
        {
            base.Start();
            this.m_aiAnimator.spriteAnimator.AnimationEventTriggered += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.AnimationEventTriggered);
            this.m_traps = new List<PitTrapController>((IEnumerable<PitTrapController>) UnityEngine.Object.FindObjectsOfType<PitTrapController>());
        }

        public override BehaviorResult Update()
        {
            BehaviorResult behaviorResult = base.Update();
            if (behaviorResult != BehaviorResult.Continue)
                return behaviorResult;
            if (!this.IsReady())
                return BehaviorResult.Continue;
            this.m_aiAnimator.PlayUntilFinished(this.anim, true);
            this.m_slammed = false;
            this.m_aiActor.ClearPath();
            this.m_updateEveryFrame = true;
            return BehaviorResult.RunContinuous;
        }

        public override ContinuousBehaviorResult ContinuousUpdate()
        {
            int num = (int) base.ContinuousUpdate();
            if (this.m_aiAnimator.IsPlaying(this.anim))
                return ContinuousBehaviorResult.Continue;
            if (!this.m_slammed)
                this.Slam();
            return ContinuousBehaviorResult.Finished;
        }

        public override void EndContinuousUpdate()
        {
            base.EndContinuousUpdate();
            this.m_aiAnimator.EndAnimationIf(this.anim);
            this.m_updateEveryFrame = false;
            this.UpdateCooldowns();
        }

        public override bool IsOverridable() => false;

        public override bool IsReady() => base.IsReady() && this.m_traps.Count > this.MinTrapsLeft();

        private void AnimationEventTriggered(
            tk2dSpriteAnimator animator,
            tk2dSpriteAnimationClip clip,
            int frame)
        {
            if (this.m_slammed || !(clip.GetFrame(frame).eventInfo == "fire"))
                return;
            this.Slam();
        }

        private void Slam()
        {
            int num = Mathf.Min(this.m_traps.Count - this.MinTrapsLeft(), 2);
            for (int index1 = 0; index1 < num; ++index1)
            {
                int index2 = UnityEngine.Random.Range(0, this.m_traps.Count);
                this.m_traps[index2].Trigger();
                this.m_traps.RemoveAt(index2);
            }
        }

        private int MinTrapsLeft()
        {
            return Mathf.RoundToInt(Mathf.Lerp((float) this.numTraps, 0.0f, Mathf.InverseLerp(1f, 0.33f, this.m_aiActor.healthHaver.GetCurrentHealthPercentage())));
        }
    }

