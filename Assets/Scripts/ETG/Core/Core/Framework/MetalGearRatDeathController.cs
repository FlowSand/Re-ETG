using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using UnityEngine;

#nullable disable

public class MetalGearRatDeathController : BraveBehaviour
    {
        public GameObject PunchoutMinigamePrefab;
        public List<GameObject> explosionVfx;
        public float explosionMidDelay = 0.3f;
        public int explosionCount = 10;
        private bool m_challengesSuppressed;

        public void Start()
        {
            this.healthHaver.ManualDeathHandling = true;
            this.healthHaver.OnPreDeath += new Action<Vector2>(this.OnBossDeath);
            this.healthHaver.OverrideKillCamTime = new float?(3.5f);
        }

        protected override void OnDestroy()
        {
            if (ChallengeManager.CHALLENGE_MODE_ACTIVE && this.m_challengesSuppressed)
            {
                ChallengeManager.Instance.SuppressChallengeStart = false;
                this.m_challengesSuppressed = false;
            }
            base.OnDestroy();
        }

        private void OnBossDeath(Vector2 dir)
        {
            this.aiAnimator.PlayUntilCancelled("death");
            this.aiAnimator.PlayVfx("death");
            GameManager.Instance.StartCoroutine(this.OnDeathExplosionsCR());
            GameManager.Instance.StartCoroutine(this.OnDeathCR());
        }

        [DebuggerHidden]
        private IEnumerator OnDeathExplosionsCR()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new MetalGearRatDeathController__OnDeathExplosionsCRc__Iterator0()
            {
                _this = this
            };
        }

        [DebuggerHidden]
        private IEnumerator OnDeathCR()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new MetalGearRatDeathController__OnDeathCRc__Iterator1()
            {
                _this = this
            };
        }
    }

