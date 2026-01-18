using System.Collections;
using System.Diagnostics;

using Brave.BulletScript;

#nullable disable

public class MimicBlackMiniguns1 : Script
    {
        private const int NumBursts = 10;
        private const int NumBulletsInBurst = 16;

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new MimicBlackMiniguns1__Topc__Iterator0()
            {
                _this = this
            };
        }

        private void FireBurst(string transform)
        {
            float num1 = this.RandomAngle();
            float num2 = 22.5f;
            for (int index = 0; index < 16; ++index)
                this.Fire(new Offset(transform), new Brave.BulletScript.Direction(num1 + (float) index * num2), new Brave.BulletScript.Speed(9f));
        }

        private void QuadShot(float direction, string transform, float speed)
        {
            for (int index = 0; index < 4; ++index)
                this.Fire(new Offset(transform), new Brave.BulletScript.Direction(direction), new Brave.BulletScript.Speed(speed - (float) index * 1.5f), (Bullet) new SpeedChangingBullet("bigBullet", speed, 120));
        }
    }

