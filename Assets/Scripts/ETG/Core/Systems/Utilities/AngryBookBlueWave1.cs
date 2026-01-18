using System.Collections;
using System.Diagnostics;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("AngryBook/BlueWave1")]
public class AngryBookBlueWave1 : Script
    {
        public int NumBullets = 32;

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new AngryBookBlueWave1__Topc__Iterator0()
            {
                _this = this
            };
        }

        public class WaveBullet : Bullet
        {
            public WaveBullet()
                : base()
            {
            }

            [DebuggerHidden]
            protected override IEnumerator Top()
            {
                // ISSUE: object of a compiler-generated type is created
                return (IEnumerator) new AngryBookBlueWave1.WaveBullet__Topc__Iterator0()
                {
                    _this = this
                };
            }
        }
    }

