using System.Collections;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Kaliber/ThreeHeads1")]
public class KaliberThreeHeads1 : Script
    {
        private const int NumBullets = 28;

        protected override IEnumerator Top()
        {
            float num1 = this.RandomAngle();
            float num2 = 12.8571424f;
            for (int index = 0; index < 28; ++index)
                this.Fire(new Brave.BulletScript.Direction(num1 + (float) index * num2), new Brave.BulletScript.Speed(9f), new Bullet("burst"));
            return (IEnumerator) null;
        }
    }

