using System.Collections;

using FullInspector;
using UnityEngine;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/MineFlayer/BellBursts1")]
public class MineFlayerBurst1 : Script
    {
        private const int NumBullets = 23;

        protected override IEnumerator Top()
        {
            float num1 = this.RandomAngle();
            float num2 = 15.652174f;
            float num3 = Random.Range(-3f, 3f);
            for (int index = 0; index < 23; ++index)
                this.Fire(new Brave.BulletScript.Direction(num1 + (float) index * num2), new Brave.BulletScript.Speed(6f + num3), (Bullet) new SpeedChangingBullet(16f + num3, 60));
            return (IEnumerator) null;
        }
    }

