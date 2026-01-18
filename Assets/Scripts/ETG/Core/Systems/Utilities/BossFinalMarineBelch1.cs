using System.Collections;
using System.Diagnostics;

using FullInspector;
using UnityEngine;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/BossFinalMarine/Belch1")]
public class BossFinalMarineBelch1 : Script
    {
        private const int NumSnakes = 10;
        private const int NumBullets = 5;
        private const int BulletSpeed = 12;
        private const float SnakeMagnitude = 0.75f;
        private const float SnakePeriod = 3f;

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new BossFinalMarineBelch1__Topc__Iterator0()
            {
                _this = this
            };
        }

        public class SnakeBullet : Bullet
        {
            private int delay;
            private Vector2 target;

            public SnakeBullet(int delay, Vector2 target)
                : base()
            {
                this.delay = delay;
                this.target = target;
            }

            [DebuggerHidden]
            protected override IEnumerator Top()
            {
                // ISSUE: object of a compiler-generated type is created
                return (IEnumerator) new BossFinalMarineBelch1.SnakeBullet__Topc__Iterator0()
                {
                    _this = this
                };
            }
        }
    }

