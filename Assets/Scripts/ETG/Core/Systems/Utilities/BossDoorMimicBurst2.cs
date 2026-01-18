using System.Collections;
using System.Diagnostics;

using FullInspector;
using UnityEngine;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/BossDoorMimic/Burst2")]
public class BossDoorMimicBurst2 : Script
    {
        private const int NumBursts = 5;
        private const int NumBullets = 36;

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new BossDoorMimicBurst2__Topc__Iterator0()
            {
                _this = this
            };
        }

        public class BurstBullet : Bullet
        {
            private Vector2 m_addtionalVelocity;

            public BurstBullet(Vector2 additionalVelocity)
                : base("burst")
            {
                this.m_addtionalVelocity = additionalVelocity;
            }

            [DebuggerHidden]
            protected override IEnumerator Top()
            {
                // ISSUE: object of a compiler-generated type is created
                return (IEnumerator) new BossDoorMimicBurst2.BurstBullet__Topc__Iterator0()
                {
                    _this = this
                };
            }
        }
    }

