using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using FullInspector;
using UnityEngine;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/ResourcefulRat/Daggers1")]
public class ResourcefulRatDaggers1 : Script
    {
        private const int NumWaves = 3;
        private const int NumDaggersPerWave = 17;
        private const int AimDelay = 1;
        private const int VanishDelay = 15;
        private const int FireDelay = 25;
        private const float DaggerSpeed = 60f;
        private List<GameObject> m_reticles = new List<GameObject>();

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new ResourcefulRatDaggers1__Topc__Iterator0()
            {
                _this = this
            };
        }

        public override void OnForceEnded() => this.CleanupReticles();

        public Vector2 GetPredictedTargetPosition(float leadAmount, float speed, float fireDelay)
        {
            return BraveMathCollege.GetPredictedPosition(this.BulletManager.PlayerPosition() + this.BulletManager.PlayerVelocity() * fireDelay, this.BulletManager.PlayerVelocity(), this.Position, speed);
        }

        private void CleanupReticles()
        {
            for (int index = 0; index < this.m_reticles.Count; ++index)
                SpawnManager.Despawn(this.m_reticles[index]);
            this.m_reticles.Clear();
        }
    }

