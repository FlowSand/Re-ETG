using System;

using FullInspector;
using UnityEngine;

#nullable disable

[InspectorDropdownName("Bosses/DraGun/GrenadeBehavior")]
public class DraGunGrenadeBehavior : BasicAttackBehavior
    {
        public float delay;
        public float delay2;
        public GameObject ShootPoint;
        public BulletScriptSelector BulletScript;
        public Animation unityAnimation;
        public string unityShootAnim;
        public AIAnimator aiAnimator;
        public string aiShootAnim;
        public Animation unityAnimation2;
        public string unityShootAnim2;
        public AIAnimator aiAnimator2;
        public string aiShootAnim2;
        public bool overrideHeadPosition;
        [InspectorShowIf("overrideHeadPosition")]
        public float headPosition;
        private DraGunController m_dragun;
        private BulletScriptSource m_bulletSource;
        private float m_timer;
        private bool m_isAttacking;
        private bool m_isAttacking2;

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
                this.StartAttack();
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
                    this.StartAttack();
            }
            else if (!this.m_isAttacking2)
            {
                if ((double) this.m_timer <= 0.0)
                    this.StartAttack2();
            }
            else
            {
                bool flag = true;
                if ((bool) (UnityEngine.Object) this.unityAnimation)
                    flag &= !this.unityAnimation.IsPlaying(this.unityShootAnim);
                if ((bool) (UnityEngine.Object) this.aiAnimator)
                    flag &= !this.aiAnimator.IsPlaying(this.aiShootAnim);
                if ((bool) (UnityEngine.Object) this.unityAnimation2)
                    flag &= !this.unityAnimation2.IsPlaying(this.unityShootAnim2);
                if ((bool) (UnityEngine.Object) this.aiAnimator2)
                    flag &= !this.aiAnimator2.IsPlaying(this.aiShootAnim2);
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
            if ((bool) (UnityEngine.Object) this.aiAnimator2)
                this.aiAnimator2.EndAnimation();
            if ((bool) (UnityEngine.Object) this.unityAnimation2)
            {
                this.unityAnimation2.Stop();
                this.unityAnimation2.GetClip(this.unityShootAnim2).SampleAnimation(this.unityAnimation2.gameObject, 1000f);
                this.unityAnimation2.GetComponent<DraGunArmController>().UnclipHandSprite();
            }
            if ((bool) (UnityEngine.Object) this.m_bulletSource)
                this.m_bulletSource.ForceStop();
            if (this.overrideHeadPosition)
                this.m_dragun.OverrideTargetX = new float?();
            this.m_isAttacking = false;
            this.m_isAttacking2 = false;
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

        private void StartAttack()
        {
            if ((bool) (UnityEngine.Object) this.unityAnimation)
                this.unityAnimation.Play(this.unityShootAnim);
            if ((bool) (UnityEngine.Object) this.aiAnimator)
                this.aiAnimator.PlayUntilCancelled(this.aiShootAnim);
            if (this.overrideHeadPosition)
                this.m_dragun.OverrideTargetX = new float?(this.headPosition);
            this.m_isAttacking = true;
            if ((double) this.delay2 <= 0.0)
            {
                this.StartAttack2();
            }
            else
            {
                this.m_timer = this.delay2;
                this.m_isAttacking2 = false;
            }
        }

        private void StartAttack2()
        {
            if ((bool) (UnityEngine.Object) this.unityAnimation2)
                this.unityAnimation2.Play(this.unityShootAnim2);
            if ((bool) (UnityEngine.Object) this.aiAnimator2)
                this.aiAnimator2.PlayUntilCancelled(this.aiShootAnim2);
            this.m_isAttacking2 = true;
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

