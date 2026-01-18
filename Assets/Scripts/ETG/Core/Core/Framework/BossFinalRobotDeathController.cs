using System;

using UnityEngine;

#nullable disable

public class BossFinalRobotDeathController : BraveBehaviour
    {
        public void Start()
        {
            this.healthHaver.OnPreDeath += new Action<Vector2>(this.OnBossDeath);
            this.healthHaver.OverrideKillCamTime = new float?(1f);
        }

        protected override void OnDestroy() => base.OnDestroy();

        private void OnBossDeath(Vector2 dir)
        {
            UnityEngine.Object.FindObjectOfType<RobotPastController>().OnBossKilled(this.transform);
        }
    }

