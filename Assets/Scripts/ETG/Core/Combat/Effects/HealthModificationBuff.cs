using System.Collections;
using System.Diagnostics;

using UnityEngine;

#nullable disable

public class HealthModificationBuff : AppliedEffectBase
    {
        public HealthModificationBuff.HealthModificationType type;
        public bool supportsMultipleInstances;
        [Tooltip("Time between damage or healing ticks.")]
        public float tickPeriod;
        [Tooltip("How long each application lasts.")]
        public float lifetime;
        [Tooltip("Damage or healing at start of duration.")]
        public float healthChangeAtStart;
        [Tooltip("Damage or healing at end of duration.")]
        public float healthChangeAtEnd;
        [Tooltip("The maximum length of time this debuff can be extended to by repeat applications.")]
        public float maxLifetime;
        public GameObject vfx;
        public float ChanceToApplyVFX = 1f;
        private float elapsed;
        private GameObject instantiatedVFX;
        private HealthHaver hh;
        private bool wasDuplicate;

        private void InitializeSelf(
            float startChange,
            float endChange,
            float length,
            float period,
            float maxLength)
        {
            this.hh = this.GetComponent<HealthHaver>();
            this.healthChangeAtStart = startChange;
            this.healthChangeAtEnd = endChange;
            this.tickPeriod = period;
            this.lifetime = length;
            this.maxLifetime = maxLength;
            if ((Object) this.hh != (Object) null)
                this.StartCoroutine(this.ApplyModification());
            else
                Object.Destroy((Object) this);
        }

        public override void Initialize(AppliedEffectBase source)
        {
            if (source is HealthModificationBuff)
            {
                HealthModificationBuff modificationBuff = source as HealthModificationBuff;
                this.InitializeSelf(modificationBuff.healthChangeAtStart, modificationBuff.healthChangeAtEnd, modificationBuff.lifetime, modificationBuff.tickPeriod, modificationBuff.maxLifetime);
                this.type = modificationBuff.type;
                if (!((Object) modificationBuff.vfx != (Object) null))
                    return;
                bool flag = true;
                if (this.wasDuplicate && (double) this.ChanceToApplyVFX < 1.0 && (double) Random.value > (double) this.ChanceToApplyVFX)
                    flag = false;
                if (!flag)
                    return;
                this.instantiatedVFX = SpawnManager.SpawnVFX(modificationBuff.vfx, this.transform.position, Quaternion.identity);
                tk2dSprite component1 = this.instantiatedVFX.GetComponent<tk2dSprite>();
                tk2dSprite component2 = this.GetComponent<tk2dSprite>();
                if ((Object) component1 != (Object) null && (Object) component2 != (Object) null)
                {
                    component2.AttachRenderer((tk2dBaseSprite) component1);
                    component1.HeightOffGround = 0.1f;
                    component1.IsPerpendicular = true;
                    component1.usesOverrideMaterial = true;
                }
                BuffVFXAnimator component3 = this.instantiatedVFX.GetComponent<BuffVFXAnimator>();
                if (!((Object) component3 != (Object) null))
                    return;
                component3.Initialize(this.GetComponent<GameActor>());
            }
            else
                Object.Destroy((Object) this);
        }

        public void ExtendLength(float time)
        {
            this.lifetime = Mathf.Min(this.lifetime + time, this.elapsed + this.maxLifetime);
        }

        public override void AddSelfToTarget(GameObject target)
        {
            if ((Object) target.GetComponent<HealthHaver>() == (Object) null)
                return;
            bool flag = false;
            HealthModificationBuff[] components = target.GetComponents<HealthModificationBuff>();
            for (int index = 0; index < components.Length; ++index)
            {
                if (components[index].type == this.type)
                {
                    if (!this.supportsMultipleInstances)
                    {
                        components[index].ExtendLength(this.lifetime);
                        return;
                    }
                    flag = true;
                }
            }
            HealthModificationBuff modificationBuff = target.AddComponent<HealthModificationBuff>();
            modificationBuff.wasDuplicate = flag;
            modificationBuff.Initialize((AppliedEffectBase) this);
        }

        [DebuggerHidden]
        private IEnumerator ApplyModification()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new HealthModificationBuff__ApplyModificationc__Iterator0()
            {
                _this = this
            };
        }

        public enum HealthModificationType
        {
            BLEED,
            POISON,
            REGEN,
            UNIQUE,
        }
    }

