using UnityEngine;

#nullable disable

public class IncrediblySimpleShootBehavior : BasicAttackBehavior
    {
        public Vector2 ShootDirection = Vector2.right;
        public WeaponType WeaponType;
        public string OverrideBulletName;
        public string OverrideAnimation;
        public string OverrideDirectionalAnimation;

        public override void Start() => base.Start();

        public override void Upkeep() => base.Upkeep();

        private void HandleAIShootVolley()
        {
            this.m_aiShooter.ShootInDirection(this.ShootDirection, this.OverrideBulletName);
        }

        private void HandleAIShoot()
        {
            this.m_aiShooter.ShootInDirection(this.ShootDirection, this.OverrideBulletName);
        }

        public override BehaviorResult Update()
        {
            int num = (int) base.Update();
            BehaviorResult behaviorResult = base.Update();
            if (behaviorResult != BehaviorResult.Continue)
                return behaviorResult;
            if (!this.IsReady())
                return BehaviorResult.Continue;
            this.HandleAIShoot();
            if (!string.IsNullOrEmpty(this.OverrideDirectionalAnimation))
            {
                if ((Object) this.m_aiAnimator != (Object) null)
                    this.m_aiAnimator.PlayUntilFinished(this.OverrideDirectionalAnimation, true);
                else
                    this.m_aiActor.spriteAnimator.PlayForDuration(this.OverrideDirectionalAnimation, -1f, this.m_aiActor.spriteAnimator.CurrentClip.name);
            }
            else if (!string.IsNullOrEmpty(this.OverrideAnimation))
            {
                if ((Object) this.m_aiAnimator != (Object) null)
                    this.m_aiAnimator.PlayUntilFinished(this.OverrideAnimation);
                else
                    this.m_aiActor.spriteAnimator.PlayForDuration(this.OverrideAnimation, -1f, this.m_aiActor.spriteAnimator.CurrentClip.name);
            }
            this.UpdateCooldowns();
            return BehaviorResult.SkipRemainingClassBehaviors;
        }
    }

