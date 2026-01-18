using System.Collections;

using Brave.BulletScript;

#nullable disable

public class CircleBurst12 : Script
    {
        private const int NumBullets = 12;

        protected override IEnumerator Top()
        {
            float num1 = this.RandomAngle();
            float num2 = 30f;
            for (int index = 0; index < 12; ++index)
                this.Fire(new Brave.BulletScript.Direction(num1 + (float) index * num2), new Brave.BulletScript.Speed(9f), (Bullet) null);
            return (IEnumerator) null;
        }
    }

