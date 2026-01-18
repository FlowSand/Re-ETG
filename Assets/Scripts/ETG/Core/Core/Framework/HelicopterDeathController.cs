using System;
using System.Collections;
using System.Diagnostics;

using UnityEngine;

#nullable disable

public class HelicopterDeathController : BraveBehaviour
    {
        public ScreenShakeSettings screenShake;
        public GameObject explosionVfx;
        private float explosionMidDelay = 0.1f;
        private int explosionCount = 35;
        public GameObject bigExplosionVfx;
        private float bigExplosionMidDelay = 0.2f;
        private int bigExplosionCount = 10;
        private bool m_isDestroyed;

        public void Start()
        {
            this.healthHaver.ManualDeathHandling = true;
            this.healthHaver.OnPreDeath += new Action<Vector2>(this.OnBossDeath);
        }

        private void OnBossDeath(Vector2 dir)
        {
            this.StartCoroutine(this.HandleBossDeath());
            int num = (int) AkSoundEngine.PostEvent("Play_State_Volume_Lower_01", this.gameObject);
        }

        [DebuggerHidden]
        private IEnumerator HandleBossDeath()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new HelicopterDeathController__HandleBossDeathc__Iterator0()
            {
                _this = this
            };
        }

        [DebuggerHidden]
        private IEnumerator HandleLittleExplosionsCR()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new HelicopterDeathController__HandleLittleExplosionsCRc__Iterator1()
            {
                _this = this
            };
        }

        [DebuggerHidden]
        private IEnumerator HandleBigExplosionsCR()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new HelicopterDeathController__HandleBigExplosionsCRc__Iterator2()
            {
                _this = this
            };
        }

        [DebuggerHidden]
        private IEnumerator HandleFlightPitfall()
        {
            // ISSUE: object of a compiler-generated type is created
            // ISSUE: variable of a compiler-generated type
            HelicopterDeathController__HandleFlightPitfallc__Iterator3 pitfallCIterator3 = new HelicopterDeathController__HandleFlightPitfallc__Iterator3();
            return (IEnumerator) pitfallCIterator3;
        }

        [DebuggerHidden]
        private IEnumerator SinkCR()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new HelicopterDeathController__SinkCRc__Iterator4()
            {
                _this = this
            };
        }

        private Vector2 RandomExplosionPos()
        {
            Vector2 position = (Vector2) this.transform.position;
            switch (UnityEngine.Random.Range(0, 8))
            {
                case 0:
                    return position + BraveUtility.RandomVector2(new Vector2(0.75f, 4.625f), new Vector2(3.875f, 5.25f));
                case 1:
                    return position + BraveUtility.RandomVector2(new Vector2(5.625f, 4.625f), new Vector2(8.75f, 5.25f));
                default:
                    return position + BraveUtility.RandomVector2(new Vector2(3.875f, 2f), new Vector2(5.625f, 8.375f));
            }
        }
    }

