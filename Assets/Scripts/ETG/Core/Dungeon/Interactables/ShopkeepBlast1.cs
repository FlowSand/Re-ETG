using Brave.BulletScript;
using System.Collections;
using UnityEngine;

#nullable disable

public class ShopkeepBlast1 : Script
  {
    private const int NumBulletsInBurst = 16 /*0x10*/;

    protected override IEnumerator Top()
    {
      this.FireBurst("left barrel");
      this.FireBurst("right barrel");
      this.QuadShot(this.AimDirection + Random.Range(-60f, 60f), !BraveUtility.RandomBool() ? "right barrel" : "left barrel", Random.Range(9f, 11f));
      return (IEnumerator) null;
    }

    private void FireBurst(string transform)
    {
      float num1 = 22.5f;
      float num2 = Random.Range((float) (-(double) num1 / 2.0), num1 / 2f);
      for (int index = 0; index < 16 /*0x10*/; ++index)
        this.Fire(new Offset(transform), new Brave.BulletScript.Direction(num2 + (float) index * num1, Brave.BulletScript.DirectionType.Relative), new Brave.BulletScript.Speed(9f), new Bullet(suppressVfx: index > 0));
    }

    private void QuadShot(float direction, string transform, float speed)
    {
      for (int index = 0; index < 4; ++index)
      {
        Bullet bullet = index != 0 ? (Bullet) new SpeedChangingBullet("bigBullet", speed, 120, suppressVfx: true) : (Bullet) new ShopkeepBlast1.BurstBullet("burstBullet", speed, 120, true);
        this.Fire(new Offset(transform), new Brave.BulletScript.Direction(direction), new Brave.BulletScript.Speed(speed - (float) index * 1.5f), bullet);
      }
    }

    private class BurstBullet : Bullet
    {
      private float m_newSpeed;
      private int m_term;

      public BurstBullet(string name, float newSpeed, int term, bool suppressVfx)
        : base(name, suppressVfx)
      {
        this.m_newSpeed = newSpeed;
        this.m_term = term;
      }

      protected override IEnumerator Top()
      {
        this.ChangeSpeed(new Brave.BulletScript.Speed(this.m_newSpeed), this.m_term);
        return (IEnumerator) null;
      }

      public override void OnBulletDestruction(
        Bullet.DestroyType destroyType,
        SpeculativeRigidbody hitRigidbody,
        bool preventSpawningProjectiles)
      {
        if (preventSpawningProjectiles)
          return;
        float num1 = 22.5f;
        float num2 = Random.Range((float) (-(double) num1 / 2.0), num1 / 2f);
        for (int index = 0; index < 16 /*0x10*/; ++index)
          this.Fire(new Brave.BulletScript.Direction(num2 + (float) index * num1, Brave.BulletScript.DirectionType.Relative), new Brave.BulletScript.Speed(9f), new Bullet(suppressVfx: true));
      }
    }
  }

