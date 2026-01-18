using System;
using System.Collections.Generic;

using UnityEngine;

#nullable disable

public class MirrorImageController : BraveBehaviour
    {
        public List<string> MirrorAnimations = new List<string>();
        private AIActor m_host;

        public void Awake()
        {
            this.aiActor.CanDropCurrency = false;
            this.aiActor.CanDropItems = false;
            this.aiActor.CollisionDamage = 0.0f;
            if ((bool) (UnityEngine.Object) this.aiActor.encounterTrackable)
                UnityEngine.Object.Destroy((UnityEngine.Object) this.aiActor.encounterTrackable);
            this.behaviorSpeculator.AttackCooldown = 10f;
            this.RegenerateCache();
        }

        public void Update()
        {
            this.behaviorSpeculator.AttackCooldown = 10f;
            if (!(bool) (UnityEngine.Object) this.m_host)
                return;
            if (this.m_host.behaviorSpeculator.ActiveContinuousAttackBehavior != null)
            {
                this.aiActor.ClearPath();
                this.behaviorSpeculator.GlobalCooldown = 1f;
            }
            else
                this.behaviorSpeculator.GlobalCooldown = 0.0f;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (!(bool) (UnityEngine.Object) this.m_host)
                return;
            this.m_host.healthHaver.OnPreDeath -= new Action<Vector2>(this.OnHostPreDeath);
            this.m_host.aiAnimator.OnPlayUntilFinished -= new AIAnimator.PlayUntilFinishedDelegate(this.PlayUntilFinished);
            this.m_host.aiAnimator.OnEndAnimationIf -= new AIAnimator.EndAnimationIfDelegate(this.EndAnimationIf);
            this.m_host.aiAnimator.OnPlayVfx -= new AIAnimator.PlayVfxDelegate(this.PlayVfx);
            this.m_host.aiAnimator.OnStopVfx -= new AIAnimator.StopVfxDelegate(this.StopVfx);
        }

        public void SetHost(AIActor host)
        {
            this.m_host = host;
            if (!(bool) (UnityEngine.Object) this.m_host)
                return;
            this.aiAnimator.CopyStateFrom(this.m_host.aiAnimator);
            this.m_host.aiAnimator.OnPlayUntilFinished += new AIAnimator.PlayUntilFinishedDelegate(this.PlayUntilFinished);
            this.m_host.aiAnimator.OnEndAnimationIf += new AIAnimator.EndAnimationIfDelegate(this.EndAnimationIf);
            this.m_host.aiAnimator.OnPlayVfx += new AIAnimator.PlayVfxDelegate(this.PlayVfx);
            this.m_host.aiAnimator.OnStopVfx += new AIAnimator.StopVfxDelegate(this.StopVfx);
            this.m_host.healthHaver.OnPreDeath += new Action<Vector2>(this.OnHostPreDeath);
        }

        private void OnHostPreDeath(Vector2 deathDir)
        {
            this.healthHaver.ApplyDamage(100000f, Vector2.zero, "Mirror Host Death", damageCategory: DamageCategory.Unstoppable);
        }

        private void PlayUntilFinished(
            string name,
            bool suppressHitStates,
            string overrideHitState,
            float warpClipDuration,
            bool skipChildAnimators)
        {
            if (!this.healthHaver.IsAlive || !this.MirrorAnimations.Contains(name))
                return;
            this.aiAnimator.PlayUntilFinished(name, suppressHitStates, overrideHitState, warpClipDuration, skipChildAnimators);
        }

        private void EndAnimationIf(string name)
        {
            if (!this.healthHaver.IsAlive)
                return;
            this.aiAnimator.EndAnimationIf(name);
        }

        private void PlayVfx(
            string name,
            Vector2? sourceNormal,
            Vector2? sourceVelocity,
            Vector2? position)
        {
            if (!this.healthHaver.IsAlive || !this.MirrorAnimations.Contains(name))
                return;
            this.aiAnimator.PlayVfx(name, sourceNormal, sourceVelocity, position);
        }

        private void StopVfx(string name)
        {
            if (!this.healthHaver.IsAlive || !this.MirrorAnimations.Contains(name))
                return;
            this.aiAnimator.StopVfx(name);
        }
    }

