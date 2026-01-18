using Brave.BulletScript;
using FullInspector;
using System.Collections;

#nullable disable

[InspectorDropdownName("Bosses/GatlingGull/BigShot1")]
public class GatlingGullBigShot1 : Script
  {
    private const int NumDeathBullets = 32 /*0x20*/;

    protected override IEnumerator Top()
    {
      this.Fire(new Brave.BulletScript.Direction(type: Brave.BulletScript.DirectionType.Aim), new Brave.BulletScript.Speed(10f), (Bullet) new GatlingGullBigShot1.BigBullet());
      return (IEnumerator) null;
    }

    private class BigBullet : Bullet
    {
      public BigBullet()
        : base("bigBullet")
      {
      }

      public override void OnBulletDestruction(
        Bullet.DestroyType destroyType,
        SpeculativeRigidbody hitRigidbody,
        bool preventSpawningProjectiles)
      {
        if (preventSpawningProjectiles)
          return;
        float num1 = this.RandomAngle();
        float num2 = 11.25f;
        for (int index = 0; index < 32 /*0x20*/; ++index)
          this.Fire(new Brave.BulletScript.Direction(num1 + num2 * (float) index), new Brave.BulletScript.Speed(10f), (Bullet) null);
      }
    }
  }

