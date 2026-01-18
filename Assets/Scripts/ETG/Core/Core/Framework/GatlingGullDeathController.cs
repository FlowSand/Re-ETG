using System;

using UnityEngine;

#nullable disable

public class GatlingGullDeathController : BraveBehaviour
    {
        public void Start() => this.healthHaver.OnPreDeath += new Action<Vector2>(this.OnBossDeath);

        protected override void OnDestroy()
        {
            if (GameManager.HasInstance)
                this.Cleanup();
            base.OnDestroy();
        }

        private void OnBossDeath(Vector2 dir) => this.Cleanup();

        private void Cleanup()
        {
            foreach (SkyRocket skyRocket in UnityEngine.Object.FindObjectsOfType<SkyRocket>())
                skyRocket.DieInAir();
        }
    }

