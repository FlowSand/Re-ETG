using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using UnityEngine;

#nullable disable

public class GunonDeathController : BraveBehaviour
    {
        public List<GameObject> explosionVfx;
        public float explosionMidDelay = 0.3f;
        public int explosionCount = 10;

        public void Start()
        {
            this.healthHaver.ManualDeathHandling = true;
            this.healthHaver.OnPreDeath += new Action<Vector2>(this.OnBossDeath);
            this.healthHaver.OverrideKillCamTime = new float?(5f);
        }

        protected override void OnDestroy() => base.OnDestroy();

        private void OnBossDeath(Vector2 dir)
        {
            this.aiAnimator.PlayUntilCancelled("death", true);
            this.StartCoroutine(this.HandleBossDeath());
            this.healthHaver.OnPreDeath -= new Action<Vector2>(this.OnBossDeath);
            int num = (int) AkSoundEngine.PostEvent("Play_BOSS_lichB_explode_01", this.gameObject);
        }

        [DebuggerHidden]
        private IEnumerator HandleBossDeath()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new GunonDeathController__HandleBossDeathc__Iterator0()
            {
                _this = this
            };
        }
    }

