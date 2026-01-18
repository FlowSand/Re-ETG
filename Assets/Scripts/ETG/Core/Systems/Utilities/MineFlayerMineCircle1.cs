using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using FullInspector;
using UnityEngine;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/MineFlayer/MineCircle1")]
public class MineFlayerMineCircle1 : Script
    {
        private const int FlightTime = 60;
        private const string EnemyGuid = "566ecca5f3b04945ac6ce1f26dedbf4f";
        private const float EnemyAngularSpeed = 30f;
        private const float EnemyAngularSpeedDelta = 5f;
        private const int BulletsPerSpray = 5;
        private static readonly float[] CircleRadii = new float[3]
        {
            4f,
            9f,
            14f
        };

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new MineFlayerMineCircle1__Topc__Iterator0()
            {
                _this = this
            };
        }

        private class MineBullet : Bullet
        {
            private float m_radius;
            private Vector2 m_goalPos;
            private List<AIActor> m_spawnedActors;

            public MineBullet(float radius, Vector2 goalPos, List<AIActor> spawnedActors)
                : base("mine")
            {
                this.m_radius = radius;
                this.m_goalPos = goalPos;
                this.m_spawnedActors = spawnedActors;
            }

            [DebuggerHidden]
            protected override IEnumerator Top()
            {
                // ISSUE: object of a compiler-generated type is created
                return (IEnumerator) new MineFlayerMineCircle1.MineBullet__Topc__Iterator0()
                {
                    _this = this
                };
            }
        }
    }

