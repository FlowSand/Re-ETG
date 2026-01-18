// Decompiled with JetBrains decompiler
// Type: ShotgrubManAttack1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using System.Collections;
using System.Diagnostics;

#nullable disable

public class ShotgrubManAttack1 : Script
  {
    private const int NumBullets = 5;
    private const float Spread = 45f;
    private const int NumDeathBullets = 6;
    private const float GrubMagnitude = 0.75f;
    private const float GrubPeriod = 3f;

    protected override IEnumerator Top()
    {
      float num1 = -22.5f;
      float num2 = 9f;
      for (int index = 0; index < 5; ++index)
        this.Fire(new Brave.BulletScript.Direction(type: Brave.BulletScript.DirectionType.Aim), new Brave.BulletScript.Speed(9f), (Bullet) new ShotgrubManAttack1.GrossBullet(num1 + (float) index * num2));
      return (IEnumerator) null;
    }

    public class GrossBullet : Bullet
    {
      private float deltaAngle;

      public GrossBullet(float deltaAngle)
        : base("gross")
      {
        this.deltaAngle = deltaAngle;
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ShotgrubManAttack1.GrossBullet__Topc__Iterator0()
        {
          _this = this
        };
      }

      public override void OnBulletDestruction(
        Bullet.DestroyType destroyType,
        SpeculativeRigidbody hitRigidbody,
        bool preventSpawningProjectiles)
      {
        if (preventSpawningProjectiles)
          return;
        float num1 = this.RandomAngle();
        float num2 = 60f;
        for (int index = 0; index < 6; ++index)
          this.Fire(new Brave.BulletScript.Direction(num1 + num2 * (float) index), new Brave.BulletScript.Speed(8f), (Bullet) new ShotgrubManAttack1.GrubBullet());
      }
    }

    public class GrubBullet : Bullet
    {
      public GrubBullet()
        : base()
      {
        this.SuppressVfx = true;
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ShotgrubManAttack1.GrubBullet__Topc__Iterator0()
        {
          _this = this
        };
      }
    }
  }

