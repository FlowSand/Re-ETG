using System;
using System.Collections.ObjectModel;

using FullInspector;
using UnityEngine;

#nullable disable

public class DeflectBulletsBehavior : BasicAttackBehavior
    {
        public float Radius;
        public float DeflectTime;
        public AnimationCurve RadiusCurve;
        public float force = 10f;
        [InspectorCategory("Visuals")]
        public string TellAnimation;
        [InspectorCategory("Visuals")]
        public string DeflectAnimation;
        [InspectorCategory("Visuals")]
        public string DeflectVfx;
        private DeflectBulletsBehavior.State m_state;
        private float m_timer;

        public override void Start()
        {
            base.Start();
            if (string.IsNullOrEmpty(this.TellAnimation))
                return;
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
            if (!this.IsReady() || !(bool) (UnityEngine.Object) this.m_aiActor.TargetRigidbody)
                return BehaviorResult.Continue;
            if (!string.IsNullOrEmpty(this.TellAnimation))
            {
                if (!string.IsNullOrEmpty(this.TellAnimation))
                    this.m_aiAnimator.PlayUntilFinished(this.TellAnimation);
                this.m_state = DeflectBulletsBehavior.State.WaitingForTell;
            }
            else
                this.StartDeflecting();
            this.m_aiActor.ClearPath();
            if ((bool) (UnityEngine.Object) this.m_aiActor && (bool) (UnityEngine.Object) this.m_aiActor.knockbackDoer)
                this.m_aiActor.knockbackDoer.SetImmobile(true, nameof (DeflectBulletsBehavior));
            this.m_updateEveryFrame = true;
            return BehaviorResult.RunContinuous;
        }

        public override ContinuousBehaviorResult ContinuousUpdate()
        {
            if (this.m_state == DeflectBulletsBehavior.State.WaitingForTell)
            {
                if (!this.m_aiAnimator.IsPlaying(this.TellAnimation))
                {
                    this.StartDeflecting();
                    return ContinuousBehaviorResult.Continue;
                }
            }
            else if (this.m_state == DeflectBulletsBehavior.State.Deflecting)
            {
                Vector2 unitCenter = this.m_aiActor.specRigidbody.HitboxPixelCollider.UnitCenter;
                float deflectRadius = this.RadiusCurve.Evaluate(Mathf.InverseLerp(this.DeflectTime, 0.0f, this.m_timer)) * this.Radius;
                ReadOnlyCollection<Projectile> allProjectiles = StaticReferenceManager.AllProjectiles;
                for (int index = 0; index < allProjectiles.Count; ++index)
                {
                    Projectile p = allProjectiles[index];
                    if (p.Owner is PlayerController && (bool) (UnityEngine.Object) p.specRigidbody && (double) Vector2.Distance(unitCenter, p.specRigidbody.UnitCenter) <= (double) deflectRadius)
                        this.AdjustProjectileVelocity(p, unitCenter, deflectRadius);
                }
                if ((double) this.m_timer <= 0.0)
                    return ContinuousBehaviorResult.Finished;
            }
            return ContinuousBehaviorResult.Continue;
        }

        public override void EndContinuousUpdate()
        {
            base.EndContinuousUpdate();
            if (!string.IsNullOrEmpty(this.TellAnimation))
                this.m_aiAnimator.EndAnimationIf(this.TellAnimation);
            if (!string.IsNullOrEmpty(this.DeflectAnimation))
                this.m_aiAnimator.EndAnimationIf(this.DeflectAnimation);
            if (!string.IsNullOrEmpty(this.DeflectVfx))
                this.m_aiAnimator.StopVfx(this.DeflectVfx);
            if ((bool) (UnityEngine.Object) this.m_aiActor && (bool) (UnityEngine.Object) this.m_aiActor.knockbackDoer)
                this.m_aiActor.knockbackDoer.SetImmobile(false, nameof (DeflectBulletsBehavior));
            this.m_state = DeflectBulletsBehavior.State.Idle;
            this.m_updateEveryFrame = false;
            this.UpdateCooldowns();
        }

        private void AnimEventTriggered(
            tk2dSpriteAnimator sprite,
            tk2dSpriteAnimationClip clip,
            int frameNum)
        {
            tk2dSpriteAnimationFrame frame = clip.GetFrame(frameNum);
            if (this.m_state != DeflectBulletsBehavior.State.WaitingForTell || !(frame.eventInfo == "deflect"))
                return;
            this.StartDeflecting();
        }

        private void StartDeflecting()
        {
            if (!string.IsNullOrEmpty(this.DeflectAnimation))
                this.m_aiAnimator.PlayUntilFinished(this.DeflectAnimation);
            if (!string.IsNullOrEmpty(this.DeflectVfx))
                this.m_aiAnimator.PlayVfx(this.DeflectVfx);
            this.m_timer = this.DeflectTime;
            this.m_state = DeflectBulletsBehavior.State.Deflecting;
        }

        private void AdjustProjectileVelocity(Projectile p, Vector2 deflectCenter, float deflectRadius)
        {
            Vector2 a = p.specRigidbody.UnitCenter - deflectCenter;
            float f = Vector2.SqrMagnitude(a);
            Vector2 velocity = p.specRigidbody.Velocity;
            if (velocity == Vector2.zero)
                return;
            float num1 = Mathf.Lerp(1f, 0.5f, Mathf.Sqrt(f) / deflectRadius);
            Vector2 vector2_1 = a.normalized * (this.force * velocity.magnitude * num1 * num1) * Mathf.Clamp(BraveTime.DeltaTime, 0.0f, 0.02f);
            Vector2 vector2_2 = velocity + vector2_1;
            if ((double) BraveTime.DeltaTime > 0.019999999552965164)
                vector2_2 *= 0.02f / BraveTime.DeltaTime;
            p.specRigidbody.Velocity = vector2_2;
            if (!(vector2_2 != Vector2.zero))
                return;
            p.Direction = vector2_2.normalized;
            p.Speed = velocity.magnitude;
            p.specRigidbody.Velocity = p.Direction * p.Speed;
            if (!p.shouldRotate || (double) vector2_2.x == 0.0 && (double) vector2_2.y == 0.0)
                return;
            float num2 = BraveMathCollege.Atan2Degrees(p.Direction);
            if (float.IsNaN(num2) || float.IsInfinity(num2))
                return;
            Quaternion quaternion = Quaternion.Euler(0.0f, 0.0f, num2);
            if (float.IsNaN(quaternion.x) || float.IsNaN(quaternion.y))
                return;
            p.transform.rotation = quaternion;
        }

        private enum State
        {
            Idle,
            WaitingForTell,
            Deflecting,
        }
    }

