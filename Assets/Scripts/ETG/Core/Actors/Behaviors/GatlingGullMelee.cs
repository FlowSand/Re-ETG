using System;

using FullInspector;
using UnityEngine;

#nullable disable

[InspectorDropdownName("Bosses/GatlingGull/Melee")]
public class GatlingGullMelee : BasicAttackBehavior
    {
        public float TriggerDistance = 4f;
        public float Damage = 1f;
        public float KnockbackForce = 30f;
        public GameObject CenterPoint;
        public float DamageDistance;
        public float StickyFriction = 0.1f;

        public override void Start() => base.Start();

        public override void Upkeep() => base.Upkeep();

        public override BehaviorResult Update()
        {
            BehaviorResult behaviorResult = base.Update();
            if (behaviorResult != BehaviorResult.Continue)
                return behaviorResult;
            if (!this.IsReady() || !(bool) (UnityEngine.Object) this.m_aiActor.TargetRigidbody || (double) (this.m_aiActor.TargetRigidbody.GetUnitCenter(ColliderType.HitBox) - this.m_aiActor.specRigidbody.UnitCenter).magnitude >= (double) this.TriggerDistance)
                return BehaviorResult.Continue;
            this.m_aiAnimator.PlayUntilFinished("melee", true);
            this.m_aiActor.ClearPath();
            this.m_aiActor.spriteAnimator.AnimationEventTriggered += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.HandleAnimationEvent);
            return BehaviorResult.RunContinuous;
        }

        public override ContinuousBehaviorResult ContinuousUpdate()
        {
            int num = (int) base.ContinuousUpdate();
            return !this.m_aiActor.spriteAnimator.IsPlaying("melee") ? ContinuousBehaviorResult.Finished : ContinuousBehaviorResult.Continue;
        }

        public override void EndContinuousUpdate()
        {
            base.EndContinuousUpdate();
            this.UpdateCooldowns();
            this.m_aiActor.spriteAnimator.AnimationEventTriggered -= new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.HandleAnimationEvent);
        }

        public override bool IsOverridable() => false;

        private void HandleAnimationEvent(
            tk2dSpriteAnimator animator,
            tk2dSpriteAnimationClip clip,
            int frameNo)
        {
            if (!(clip.GetFrame(frameNo).eventInfo == "melee_hit"))
                return;
            SpeculativeRigidbody targetRigidbody = this.m_aiActor.TargetRigidbody;
            if (!(bool) (UnityEngine.Object) targetRigidbody)
                return;
            Vector2 vector2 = targetRigidbody.GetUnitCenter(ColliderType.HitBox) - this.CenterPoint.transform.position.XY();
            if ((double) vector2.magnitude >= (double) this.DamageDistance)
                return;
            PlayerController gameActor = !(bool) (UnityEngine.Object) targetRigidbody.gameActor ? (PlayerController) null : targetRigidbody.gameActor as PlayerController;
            if ((bool) (UnityEngine.Object) targetRigidbody.healthHaver && targetRigidbody.healthHaver.IsVulnerable && (!(bool) (UnityEngine.Object) gameActor || !gameActor.IsEthereal))
            {
                targetRigidbody.healthHaver.ApplyDamage(this.Damage, vector2.normalized, this.m_aiActor.GetActorName());
                if ((bool) (UnityEngine.Object) targetRigidbody.knockbackDoer)
                    targetRigidbody.knockbackDoer.ApplyKnockback(vector2.normalized, this.KnockbackForce);
                StickyFrictionManager.Instance.RegisterCustomStickyFriction(this.StickyFriction, 0.0f, false);
            }
            if (!(bool) (UnityEngine.Object) targetRigidbody.majorBreakable)
                return;
            targetRigidbody.majorBreakable.ApplyDamage(1000f, vector2.normalized, true);
        }
    }

