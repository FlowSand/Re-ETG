using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using FullInspector;
using UnityEngine;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/Helicopter/Flames1")]
public class HelicopterFlames1 : Script
    {
        private static string[] s_Transforms = new string[4]
        {
            "shoot point 1",
            "shoot point 2",
            "shoot point 3",
            "shoot point 4"
        };
        public const int NumFlamesPerRow = 9;

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new HelicopterFlames1__Topc__Iterator0()
            {
                _this = this
            };
        }

        private class FlameBullet : Bullet
        {
            private Vector2 m_goalPos;
            private int m_flightTime;

            public FlameBullet(Vector2 goalPos, List<AIActor> spawnedActors, int flightTime)
                : base("flame")
            {
                this.m_goalPos = goalPos;
                this.m_flightTime = flightTime;
            }

            [DebuggerHidden]
            protected override IEnumerator Top()
            {
                // ISSUE: object of a compiler-generated type is created
                return (IEnumerator) new HelicopterFlames1.FlameBullet__Topc__Iterator0()
                {
                    _this = this
                };
            }
        }
    }

