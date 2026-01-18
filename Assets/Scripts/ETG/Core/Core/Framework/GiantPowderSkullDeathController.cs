using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using UnityEngine;

#nullable disable

public class GiantPowderSkullDeathController : BraveBehaviour
    {
        public List<GameObject> explosionVfx;
        public float explosionMidDelay = 0.3f;
        public int explosionCount = 10;
        public GameObject bigExplosionVfx;

        public void Start()
        {
            this.healthHaver.ManualDeathHandling = true;
            this.healthHaver.OnPreDeath += new Action<Vector2>(this.OnBossDeath);
        }

        protected override void OnDestroy() => base.OnDestroy();

        private void OnBossDeath(Vector2 dir)
        {
            this.aiAnimator.PlayUntilFinished("death", true);
            this.StartCoroutine(this.OnDeathExplosionsCR());
            this.StartCoroutine(this.HandleParticleSystemsCR());
        }

        [DebuggerHidden]
        private IEnumerator OnDeathExplosionsCR()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new GiantPowderSkullDeathController__OnDeathExplosionsCRc__Iterator0()
            {
                _this = this
            };
        }

        [DebuggerHidden]
        private IEnumerator HandleParticleSystemsCR()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new GiantPowderSkullDeathController__HandleParticleSystemsCRc__Iterator1()
            {
                _this = this
            };
        }
    }

