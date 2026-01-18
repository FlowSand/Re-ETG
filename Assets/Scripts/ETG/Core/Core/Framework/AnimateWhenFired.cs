using System;

using UnityEngine;

using Brave.BulletScript;

#nullable disable

public class AnimateWhenFired : BraveBehaviour
    {
        [Header("Trigger")]
        public AnimateWhenFired.TriggerType trigger;
        [ShowInInspectorIf("trigger", 0, false)]
        public AIBulletBank specifyBulletBank;
        [ShowInInspectorIf("trigger", 0, false)]
        public string transformName;
        [Header("Animation")]
        public AIAnimator specifyAiAnimator;
        [CheckDirectionalAnimation(null)]
        public string fireAnim;

        public void Start()
        {
            if (!(bool) (UnityEngine.Object) this.specifyAiAnimator)
                this.specifyAiAnimator = this.aiAnimator;
            if (this.trigger != AnimateWhenFired.TriggerType.BulletBankTransform)
                return;
            if (!(bool) (UnityEngine.Object) this.specifyBulletBank)
                this.specifyBulletBank = this.bulletBank;
            this.specifyBulletBank.OnBulletSpawned += new Action<Bullet, Projectile>(this.BulletSpawned);
        }

        protected override void OnDestroy()
        {
            if (this.trigger == AnimateWhenFired.TriggerType.BulletBankTransform && (bool) (UnityEngine.Object) this.specifyBulletBank)
                this.specifyBulletBank.OnBulletSpawned += new Action<Bullet, Projectile>(this.BulletSpawned);
            base.OnDestroy();
        }

        private void BulletSpawned(Bullet bullet, Projectile projectile)
        {
            if (!(this.transformName == bullet.SpawnTransform))
                return;
            this.specifyAiAnimator.PlayUntilFinished(this.fireAnim);
        }

        public enum TriggerType
        {
            BulletBankTransform,
        }
    }

