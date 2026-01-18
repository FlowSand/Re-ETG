using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using UnityEngine;

#nullable disable

public class BlobulordDeathController : BraveBehaviour
    {
        public List<GameObject> explosionVfx;
        public float explosionMidDelay = 0.3f;
        public int explosionCount = 10;
        public float finalScale = 0.1f;
        public GameObject bigExplosionVfx;
        public float crawlerSpawnDelay = 0.3f;
        [EnemyIdentifier]
        public string crawlerGuid;

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
        }

        [DebuggerHidden]
        private IEnumerator OnDeathExplosionsCR()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new BlobulordDeathController__OnDeathExplosionsCRc__Iterator0()
            {
                _this = this
            };
        }
    }

