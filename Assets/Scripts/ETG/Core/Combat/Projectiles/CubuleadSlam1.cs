using Brave.BulletScript;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class CubuleadSlam1 : Script
  {
    private const int NumBullets = 11;

    protected override IEnumerator Top()
    {
      this.FireLine(45f);
      this.FireLine(135f);
      this.FireLine(225f);
      this.FireLine(315f);
      return (IEnumerator) null;
    }

    private void FireLine(float startingAngle)
    {
      float num1 = 9f;
      for (int index = 0; index < 11; ++index)
      {
        float num2 = Mathf.Atan((float) (((double) index * (double) num1 - 45.0) / 45.0)) * 57.29578f;
        float f = Mathf.Cos(num2 * ((float) Math.PI / 180f));
        float num3 = (double) Mathf.Abs(f) >= 0.0001 ? 1f / f : 1f;
        this.Fire(new Brave.BulletScript.Direction(num2 + startingAngle), new Brave.BulletScript.Speed(num3 * 9f), (Bullet) new CubuleadSlam1.ReversingBullet());
      }
    }

    public class ReversingBullet : Bullet
    {
      public ReversingBullet()
        : base("reversible")
      {
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new CubuleadSlam1.ReversingBullet__Topc__Iterator0()
        {
          _this = this
        };
      }

      private void OnPreDeath(Vector2 deathDir) => this.Vanish();

      public override void OnBulletDestruction(
        Bullet.DestroyType destroyType,
        SpeculativeRigidbody hitRigidbody,
        bool preventSpawningProjectiles)
      {
        if (!(bool) (UnityEngine.Object) this.BulletBank || !(bool) (UnityEngine.Object) this.BulletBank.healthHaver)
          return;
        this.BulletBank.healthHaver.OnPreDeath -= new Action<Vector2>(this.OnPreDeath);
      }
    }
  }

