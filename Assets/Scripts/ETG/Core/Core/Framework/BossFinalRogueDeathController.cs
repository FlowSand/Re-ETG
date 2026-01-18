using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using UnityEngine;

#nullable disable

public class BossFinalRogueDeathController : BraveBehaviour
    {
        public List<GameObject> explosionVfx;
        public float explosionMidDelay = 0.3f;
        public int explosionCount = 10;
        [Space(12f)]
        public List<GameObject> bigExplosionVfx;
        public float bigExplosionMidDelay = 0.3f;
        public int bigExplosionCount = 10;
        public GameObject DeathStarExplosionVFX;

        public void Start()
        {
            this.healthHaver.ManualDeathHandling = true;
            this.healthHaver.OnPreDeath += new Action<Vector2>(this.OnBossDeath);
        }

        protected override void OnDestroy() => base.OnDestroy();

        private void OnBossDeath(Vector2 dir)
        {
            this.behaviorSpeculator.enabled = false;
            this.aiActor.BehaviorOverridesVelocity = true;
            this.aiActor.BehaviorVelocity = Vector2.zero;
            this.aiAnimator.PlayUntilCancelled("die");
            this.StartCoroutine(this.Drift());
            this.StartCoroutine(this.OnDeathExplosionsCR());
        }

        [DebuggerHidden]
        private IEnumerator Drift()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new BossFinalRogueDeathController__Driftc__Iterator0()
            {
                _this = this
            };
        }

        [DebuggerHidden]
        private IEnumerator OnDeathExplosionsCR()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new BossFinalRogueDeathController__OnDeathExplosionsCRc__Iterator1()
            {
                _this = this
            };
        }
    }

