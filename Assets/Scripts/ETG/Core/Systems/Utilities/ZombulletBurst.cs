using System.Collections;
using System.Diagnostics;

using Brave.BulletScript;

#nullable disable

public class ZombulletBurst : Script
    {
        private const int NumBullets = 18;

        protected override IEnumerator Top()
        {
            float num1 = this.RandomAngle();
            float num2 = 20f;
            for (int index = 0; index < 18; ++index)
                this.Fire(new Brave.BulletScript.Direction(num1 + (float) index * num2), new Brave.BulletScript.Speed(7f), (Bullet) new ZombulletBurst.OscillatingBullet());
            return (IEnumerator) null;
        }

        private class OscillatingBullet : Bullet
        {
            public OscillatingBullet()
                : base()
            {
            }

            [DebuggerHidden]
            protected override IEnumerator Top()
            {
                // ISSUE: object of a compiler-generated type is created
                return (IEnumerator) new ZombulletBurst.OscillatingBullet__Topc__Iterator0()
                {
                    _this = this
                };
            }
        }
    }

