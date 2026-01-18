using System;
using System.Collections.Generic;

using UnityEngine;

#nullable disable

public class TBulonController : BraveBehaviour
    {
        public float newHealth = 50f;
        [CheckDirectionalAnimation(null)]
        public string transformAnim;
        [CheckDirectionalAnimation(null)]
        public string enrageAnim;
        public float overrideMoveSpeed = -1f;
        public float overrideWeight = -1f;
        public List<DamageTypeModifier> onFireDamageTypeModifiers;
        private TBulonController.State m_state;
        private GoopDoer m_goopDoer;
        private float m_startGoopRadius;

        public void Start()
        {
            this.healthHaver.minimumHealth = 1f;
            this.healthHaver.OnDamaged += new HealthHaver.OnDamagedEvent(this.OnDamaged);
            this.m_goopDoer = this.GetComponent<GoopDoer>();
        }

        public void Update()
        {
            if (!(bool) (UnityEngine.Object) this.aiActor || !(bool) (UnityEngine.Object) this.healthHaver || this.healthHaver.IsDead || this.m_state == TBulonController.State.Normal)
                return;
            if (this.m_state == TBulonController.State.Transforming)
            {
                this.sprite.ForceUpdateMaterial();
                if (this.aiAnimator.IsPlaying(this.transformAnim))
                    return;
                this.aiAnimator.PlayUntilFinished(this.enrageAnim, true);
                this.behaviorSpeculator.enabled = true;
                if ((double) this.overrideMoveSpeed >= 0.0)
                    this.aiActor.MovementSpeed = TurboModeController.MaybeModifyEnemyMovementSpeed(this.overrideMoveSpeed);
                if ((double) this.overrideWeight >= 0.0)
                    this.knockbackDoer.weight = this.overrideWeight;
                this.m_goopDoer.enabled = true;
                this.m_startGoopRadius = this.m_goopDoer.defaultGoopRadius;
                this.m_state = TBulonController.State.Enraged;
            }
            else
            {
                if (this.m_state != TBulonController.State.Enraged)
                    return;
                if (!this.aiAnimator.IsPlaying(this.enrageAnim))
                {
                    this.healthHaver.ManualDeathHandling = true;
                    this.aiActor.ForceDeath(Vector2.zero, false);
                    UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
                }
                else
                    this.m_goopDoer.defaultGoopRadius = Mathf.Lerp(this.m_startGoopRadius, 0.2f, this.aiAnimator.CurrentClipProgress);
            }
        }

        protected override void OnDestroy()
        {
            if ((bool) (UnityEngine.Object) this.healthHaver)
                this.healthHaver.OnDamaged -= new HealthHaver.OnDamagedEvent(this.OnDamaged);
            base.OnDestroy();
        }

        private void OnDamaged(
            float resultValue,
            float maxValue,
            CoreDamageTypes damageTypes,
            DamageCategory damageCategory,
            Vector2 damageDirection)
        {
            if (this.m_state != TBulonController.State.Normal || (double) resultValue != 1.0)
                return;
            this.aiAnimator.PlayUntilFinished(this.transformAnim, true);
            this.healthHaver.ApplyDamageModifiers(this.onFireDamageTypeModifiers);
            this.healthHaver.SetHealthMaximum(this.newHealth);
            this.healthHaver.ForceSetCurrentHealth(this.newHealth);
            this.healthHaver.minimumHealth = 0.0f;
            this.behaviorSpeculator.InterruptAndDisable();
            this.aiActor.ClearPath();
            this.aiAnimator.OtherAnimations.Find((Predicate<AIAnimator.NamedDirectionalAnimation>) (a => a.name == "pitfall")).anim.Prefix = "pitfall_hot";
            this.m_state = TBulonController.State.Transforming;
        }

        private enum State
        {
            Normal,
            Transforming,
            Enraged,
        }
    }

