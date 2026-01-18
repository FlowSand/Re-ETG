using System.Collections;

using Brave.BulletScript;

#nullable disable

public class GripMasterRing1 : Script
    {
        protected override IEnumerator Top()
        {
            float aimDirection = this.AimDirection;
            int numBullets1 = 16;
            for (int i = 0; i < numBullets1; ++i)
                this.Fire(new Brave.BulletScript.Direction(this.SubdivideCircle(aimDirection, numBullets1, i)), new Brave.BulletScript.Speed(9f), (Bullet) null);
            float sweepAngle = 135f;
            float startAngle = aimDirection - sweepAngle / 2f;
            int numBullets2 = 7;
            for (int i = 0; i < numBullets2 - 1; ++i)
                this.Fire(new Brave.BulletScript.Direction(this.SubdivideArc(startAngle, sweepAngle, numBullets2, i, true)), new Brave.BulletScript.Speed(17f), (Bullet) new SpeedChangingBullet(9f, 30));
            for (int i = 0; i < numBullets2 - 1; ++i)
                this.Fire(new Brave.BulletScript.Direction(this.SubdivideArc(startAngle, sweepAngle, numBullets2, i, true)), new Brave.BulletScript.Speed(1f), (Bullet) new SpeedChangingBullet(9f, 30));
            return (IEnumerator) null;
        }
    }

