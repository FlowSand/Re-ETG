using System;

using FullInspector;
using UnityEngine;

#nullable disable

[InspectorDropdownName("Bosses/DraGun/RPGBehavior")]
public class DraGunRPGBehavior : BasicAttackBehavior
    {
        public float delay;
        public GameObject ShootPoint;
        public BulletScriptSelector BulletScript;
        public Animation unityAnimation;
        public string unityShootAnim;
        public AIAnimator aiAnimator;
        public string aiShootAnim;
        public bool overrideHeadPosition;
        [InspectorShowIf("overrideHeadPosition")]
        public float headPosition;
        private DraGunController m_dragun;
        private BulletScriptSource m_bulletSource;
        private float m_timer;
        private bool m_isAttacking;

        public override void Start()
        {
            base.Start();
            this.m_dragun = this.m_aiActor.GetComponent<DraGunController>();
            if (!(bool) (UnityEngine.Object) this.aiAnimator)
                return;
            this.aiAnimator.spriteAnimator.AnimationEventTriggered += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.AnimationEventTriggered);
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
            if ((double) this.delay <= 0.0)
            {
                this.StartThrow();
            }
            else
            {
                this.m_timer = this.delay;
                this.m_isAttacking = false;
            }
            this.m_updateEveryFrame = true;
            return BehaviorResult.RunContinuous;
        }

        public override ContinuousBehaviorResult ContinuousUpdate()
        {
            int num = (int) base.ContinuousUpdate();
            if (!this.m_isAttacking)
            {
                if ((double) this.m_timer <= 0.0)
                    this.StartThrow();
            }
            else
            {
                bool flag = true;
                if ((bool) (UnityEngine.Object) this.unityAnimation)
                    flag &= !this.unityAnimation.IsPlaying(this.unityShootAnim);
                if ((bool) (UnityEngine.Object) this.aiAnimator)
                    flag &= !this.aiAnimator.IsPlaying(this.aiShootAnim);
                if (flag)
                    return ContinuousBehaviorResult.Finished;
            }
            return ContinuousBehaviorResult.Continue;
        }

        public override void EndContinuousUpdate()
        {
            base.EndContinuousUpdate();
            if ((bool) (UnityEngine.Object) this.aiAnimator)
                this.aiAnimator.EndAnimation();
            if ((bool) (UnityEngine.Object) this.unityAnimation)
            {
                this.unityAnimation.Stop();
                this.unityAnimation.GetClip(this.unityShootAnim).SampleAnimation(this.unityAnimation.gameObject, 1000f);
                this.unityAnimation.GetComponent<DraGunArmController>().UnclipHandSprite();
            }
            if ((bool) (UnityEngine.Object) this.m_bulletSource)
                this.m_bulletSource.ForceStop();
            if (this.overrideHeadPosition)
                this.m_dragun.OverrideTargetX = new float?();
            this.m_isAttacking = false;
            this.m_updateEveryFrame = false;
            this.UpdateCooldowns();
        }

        private void AnimationEventTriggered(
            tk2dSpriteAnimator spriteAnimator,
            tk2dSpriteAnimationClip clip,
            int frame)
        {
            if (!this.m_isAttacking || !(clip.GetFrame(frame).eventInfo == "fire"))
                return;
            this.Fire();
        }

        private void StartThrow()
        {
            if ((bool) (UnityEngine.Object) this.unityAnimation)
                this.unityAnimation.Play(this.unityShootAnim);
            if ((bool) (UnityEngine.Object) this.aiAnimator)
                this.aiAnimator.PlayUntilCancelled(this.aiShootAnim);
            if (this.overrideHeadPosition)
                this.m_dragun.OverrideTargetX = new float?(this.headPosition);
            this.m_isAttacking = true;
        }

        private void Fire()
        {
            if (!(bool) (UnityEngine.Object) this.m_bulletSource)
                this.m_bulletSource = this.ShootPoint.GetOrAddComponent<BulletScriptSource>();
            this.m_bulletSource.BulletManager = this.m_aiActor.bulletBank;
            this.m_bulletSource.BulletScript = this.BulletScript;
            this.m_bulletSource.Initialize();
        }
    }

