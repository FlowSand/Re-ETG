using System;
using System.Collections;
using System.Diagnostics;

using UnityEngine;

#nullable disable

public class AgunimDeathController : BraveBehaviour
    {
        public void Start()
        {
            this.healthHaver.ManualDeathHandling = true;
            this.healthHaver.OnPreDeath += new Action<Vector2>(this.OnBossDeath);
            this.healthHaver.OverrideKillCamTime = new float?(5f);
        }

        protected override void OnDestroy() => base.OnDestroy();

        private void OnBossDeath(Vector2 dir)
        {
            this.aiAnimator.ChildAnimator.gameObject.SetActive(false);
            this.aiAnimator.PlayUntilCancelled("death", true);
            this.StartCoroutine(this.HandlePostDeathExplosionCR());
            this.healthHaver.OnPreDeath -= new Action<Vector2>(this.OnBossDeath);
            this.StartCoroutine(this.HandlePostDeathExplosionCR());
        }

        [DebuggerHidden]
        private IEnumerator HandlePostDeathExplosionCR()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new AgunimDeathController__HandlePostDeathExplosionCRc__Iterator0()
            {
                _this = this
            };
        }
    }

