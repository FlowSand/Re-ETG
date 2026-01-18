using UnityEngine;

#nullable disable

public class ExplodeInRadius : AttackBehaviorBase
    {
        public float explodeDistance = 1f;
        public float explodeCountDown;
        public bool stopMovement;
        public float minLifetime;
        protected float m_closeEnoughToExplodeTimer;
        protected float m_explodeTime;
        protected float m_lifetime;
        protected float m_elapsed;

        public override void Start()
        {
            base.Start();
            tk2dSpriteAnimationClip clipByName = this.m_gameObject.GetComponent<tk2dSpriteAnimator>().GetClipByName("explode");
            if (clipByName == null)
                return;
            this.m_explodeTime = (float) clipByName.frames.Length / clipByName.fps;
        }

        public override void Upkeep()
        {
            base.Upkeep();
            if ((double) this.minLifetime <= 0.0)
                return;
            this.m_lifetime += this.m_deltaTime;
        }

        public override BehaviorResult Update()
        {
            if (this.m_aiActor.healthHaver.IsDead)
                return BehaviorResult.SkipAllRemainingBehaviors;
            if ((double) this.minLifetime > 0.0 && (double) this.m_lifetime < (double) this.minLifetime)
                return BehaviorResult.Continue;
            if ((Object) this.m_aiActor.TargetRigidbody != (Object) null && (double) this.m_aiActor.DistanceToTarget < (double) this.explodeDistance)
            {
                this.m_closeEnoughToExplodeTimer += this.m_deltaTime;
                if ((double) this.m_closeEnoughToExplodeTimer > (double) this.explodeCountDown)
                {
                    this.m_aiAnimator.PlayForDuration("explode", this.m_explodeTime);
                    if (this.stopMovement)
                        this.m_aiActor.ClearPath();
                    this.m_updateEveryFrame = true;
                    return BehaviorResult.RunContinuous;
                }
            }
            else
                this.m_closeEnoughToExplodeTimer = 0.0f;
            return BehaviorResult.Continue;
        }

        public override ContinuousBehaviorResult ContinuousUpdate()
        {
            if ((double) this.m_elapsed >= (double) this.m_explodeTime)
                return ContinuousBehaviorResult.Finished;
            this.m_elapsed += this.m_deltaTime;
            return ContinuousBehaviorResult.Continue;
        }

        public override void EndContinuousUpdate()
        {
            base.EndContinuousUpdate();
            if (this.m_aiActor.healthHaver.PreventAllDamage)
                this.m_aiActor.healthHaver.PreventAllDamage = false;
            ExplodeOnDeath component = this.m_aiActor.GetComponent<ExplodeOnDeath>();
            if ((bool) (Object) component && component.LinearChainExplosion)
            {
                component.ChainIsReversed = false;
                component.explosionData.damage = 5f;
            }
            if (this.m_aiActor.healthHaver.IsAlive)
                this.m_aiActor.healthHaver.ApplyDamage(float.MaxValue, Vector2.zero, "self-immolation", CoreDamageTypes.Fire, DamageCategory.Unstoppable, true);
            this.m_updateEveryFrame = false;
        }

        public override bool IsReady() => true;

        public override float GetMinReadyRange() => -1f;

        public override float GetMaxRange() => -1f;
    }

