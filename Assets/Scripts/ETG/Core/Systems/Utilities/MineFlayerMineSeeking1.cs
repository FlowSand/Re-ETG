using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using FullInspector;
using UnityEngine;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/MineFlayer/MineSeeking1")]
public class MineFlayerMineSeeking1 : Script
    {
        private const int FlightTime = 60;
        private const string EnemyGuid = "566ecca5f3b04945ac6ce1f26dedbf4f";
        private const float EnemyAngularSpeed = 30f;
        private const float EnemyAngularSpeedDelta = 5f;
        private const int BulletsPerSpray = 5;
        private static readonly float CircleRadius = 14f;

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new MineFlayerMineSeeking1__Topc__Iterator0()
            {
                _this = this
            };
        }

        private class MineBullet : Bullet
        {
            private Vector2 m_goalPos;
            private List<AIActor> m_spawnedActors;
            private int m_index;

            public MineBullet(Vector2 goalPos, List<AIActor> spawnedActors, int index)
                : base("mine")
            {
                this.m_goalPos = goalPos;
                this.m_spawnedActors = spawnedActors;
                this.m_index = index;
            }

            [DebuggerHidden]
            protected override IEnumerator Top()
            {
                // ISSUE: object of a compiler-generated type is created
                return (IEnumerator) new MineFlayerMineSeeking1.MineBullet__Topc__Iterator0()
                {
                    _this = this
                };
            }
        }
    }

