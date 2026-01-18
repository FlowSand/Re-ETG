using System;

using FullInspector;
using UnityEngine;

#nullable disable

[InspectorDropdownName("Bosses/GiantPowderSkull/ArmosBehavior")]
public class GiantPowderSkullArmosBehavior : BasicAttackBehavior
    {
        public GameObject shootPoint;
        public BulletScriptSelector bulletScript;
        public float time = 8f;
        public float speed = 6f;
        public float startingAngle = -90f;
        public float rotationSpeed = -180f;
        [InspectorCategory("Visuals")]
        public string armosAnim;
        private bool m_isRunning;
        private float m_timer;
        private float m_currentAngle;
        private BulletScriptSource m_bulletScriptSource;

        public override void Start()
        {
            base.Start();
            this.m_aiAnimator.spriteAnimator.AnimationEventTriggered += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.AnimEventTriggered);
        }

        public override void Upkeep()
        {
            base.Upkeep();
            this.DecrementTimer(ref this.m_timer);
        }

        public override BehaviorResult Update()
        {
            BehaviorResult behaviorResult = base.Update();
            if (behaviorResult != BehaviorResult.Continue)
                return behaviorResult;
            if (!this.IsReady())
                return BehaviorResult.Continue;
            this.m_aiAnimator.PlayUntilFinished(this.armosAnim);
            this.m_timer = this.time;
            this.m_isRunning = true;
            this.m_aiActor.ClearPath();
            this.m_aiActor.BehaviorOverridesVelocity = true;
            this.m_currentAngle = this.startingAngle;
            this.m_aiActor.BehaviorVelocity = BraveMathCollege.DegreesToVector(this.m_currentAngle, this.speed);
            this.m_updateEveryFrame = true;
            return BehaviorResult.RunContinuous;
        }

        public override ContinuousBehaviorResult ContinuousUpdate()
        {
            int num = (int) base.ContinuousUpdate();
            if ((double) this.m_timer <= 0.0)
            {
                this.m_aiAnimator.EndAnimation();
                return ContinuousBehaviorResult.Finished;
            }
            this.m_currentAngle = BraveMathCollege.ClampAngle180(this.m_currentAngle + this.rotationSpeed * this.m_deltaTime);
            this.m_aiActor.BehaviorVelocity = BraveMathCollege.DegreesToVector(this.m_currentAngle, this.speed);
            return ContinuousBehaviorResult.Continue;
        }

        public override void EndContinuousUpdate()
        {
            base.EndContinuousUpdate();
            this.m_isRunning = false;
            this.m_updateEveryFrame = false;
            this.m_aiActor.BehaviorOverridesVelocity = false;
            this.UpdateCooldowns();
        }

        private void ShootBulletScript()
        {
            if (!(bool) (UnityEngine.Object) this.m_bulletScriptSource)
                this.m_bulletScriptSource = this.shootPoint.GetOrAddComponent<BulletScriptSource>();
            this.m_bulletScriptSource.BulletManager = this.m_aiActor.bulletBank;
            this.m_bulletScriptSource.BulletScript = this.bulletScript;
            this.m_bulletScriptSource.Initialize();
        }

        private void AnimEventTriggered(
            tk2dSpriteAnimator sprite,
            tk2dSpriteAnimationClip clip,
            int frameNum)
        {
            if (!this.m_isRunning || !(clip.GetFrame(frameNum).eventInfo == "fire"))
                return;
            this.ShootBulletScript();
        }
    }

