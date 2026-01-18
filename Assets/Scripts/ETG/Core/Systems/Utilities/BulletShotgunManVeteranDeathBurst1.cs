using Brave.BulletScript;
using FullInspector;
using System.Collections;

#nullable disable

[InspectorDropdownName("BulletShotgunMan/VeteranDeathBurst1")]
public class BulletShotgunManVeteranDeathBurst1 : Script
  {
    protected override IEnumerator Top()
    {
      int numBullets1 = 5;
      for (int i = 0; i < numBullets1; ++i)
        this.Fire(new Brave.BulletScript.Direction(this.SubdivideCircle(0.0f, numBullets1, i)), new Brave.BulletScript.Speed(6.5f), new Bullet("flashybullet"));
      int numBullets2 = 5;
      for (int i = 0; i < numBullets2; ++i)
        this.Fire(new Brave.BulletScript.Direction(this.SubdivideCircle(0.0f, numBullets2, i, offset: true)), new Brave.BulletScript.Speed(10f), new Bullet("flashybullet"));
      int num = 3;
      for (int index = 0; index < num; ++index)
        this.Fire(new Brave.BulletScript.Direction(this.RandomAngle()), new Brave.BulletScript.Speed(12f), new Bullet("flashybullet"));
      return (IEnumerator) null;
    }
  }

