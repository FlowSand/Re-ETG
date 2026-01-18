using System.Collections;
using System.Diagnostics;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/DemonWall/LeapLine1")]
public class DemonWallLeapLine1 : Script
    {
        private const int NumBullets = 24;

        protected override IEnumerator Top()
        {
            float num = 1f;
            for (int index = 0; index < 24; ++index)
                this.Fire(new Offset((float) ((double) index * (double) num - 11.5), transform: string.Empty), new Brave.BulletScript.Direction(-90f), new Brave.BulletScript.Speed(5f), (Bullet) new DemonWallLeapLine1.WaveBullet());
            return (IEnumerator) null;
        }

        private class WaveBullet : Bullet
        {
            private const float SinPeriod = 0.75f;
            private const float SinMagnitude = 1.5f;

            public WaveBullet()
                : base("leap")
            {
            }

            [DebuggerHidden]
            protected override IEnumerator Top()
            {
                // ISSUE: object of a compiler-generated type is created
                return (IEnumerator) new DemonWallLeapLine1.WaveBullet__Topc__Iterator0()
                {
                    _this = this
                };
            }
        }
    }

