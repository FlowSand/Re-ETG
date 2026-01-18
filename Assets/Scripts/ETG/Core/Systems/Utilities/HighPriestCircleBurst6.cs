using System.Collections;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/HighPriest/CircleBurst6")]
public class HighPriestCircleBurst6 : Script
    {
        private const int NumBullets = 6;

        protected override IEnumerator Top()
        {
            float num1 = this.RandomAngle();
            float num2 = 60f;
            for (int index = 0; index < 6; ++index)
                this.Fire(new Brave.BulletScript.Direction(num1 + (float) index * num2), new Brave.BulletScript.Speed(9f), new Bullet("homingPop"));
            return (IEnumerator) null;
        }
    }

