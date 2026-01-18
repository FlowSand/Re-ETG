using System.Collections;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/BulletBros/JumpBurst1")]
public class BulletBrosJumpBurst1 : Script
    {
        private const int NumBullets = 12;

        protected override IEnumerator Top()
        {
            float num1 = this.RandomAngle();
            float num2 = 30f;
            for (int index = 0; index < 12; ++index)
                this.Fire(new Brave.BulletScript.Direction(num1 + (float) index * num2), new Brave.BulletScript.Speed(9f), new Bullet("jump", true));
            return (IEnumerator) null;
        }
    }

