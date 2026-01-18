using System.Collections;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/BulletBros/JumpBurst2")]
public class BulletBrosJumpBurst2 : Script
    {
        private const int NumFastBullets = 18;
        private const int NumSlowBullets = 9;

        protected override IEnumerator Top()
        {
            float startAngle1 = this.RandomAngle();
            for (int i = 0; i < 18; ++i)
                this.Fire(new Brave.BulletScript.Direction(this.SubdivideCircle(startAngle1, 18, i)), new Brave.BulletScript.Speed(9f), new Bullet("jump", true));
            float startAngle2 = startAngle1 + 10f;
            for (int i = 0; i < 9; ++i)
                this.Fire(new Brave.BulletScript.Direction(this.SubdivideCircle(startAngle2, 9, i)), new Brave.BulletScript.Speed(), (Bullet) new SpeedChangingBullet("jump", 9f, 75, suppressVfx: true));
            return (IEnumerator) null;
        }
    }

