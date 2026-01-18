// Decompiled with JetBrains decompiler
// Type: GunNutSpectreCone1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

[InspectorDropdownName("GunNut/SpectreCone1")]
public class GunNutSpectreCone1 : Script
  {
    private const int NumBulletsMainWave = 25;

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new GunNutSpectreCone1__Topc__Iterator0()
      {
        _this = this
      };
    }

    public class BurstingBullet : Bullet
    {
      private const int NumBullets = 18;
      private bool m_isBlackPhantom;

      public BurstingBullet(bool isBlackPhantom)
        : base("bigBullet")
      {
        this.ForceBlackBullet = true;
        this.m_isBlackPhantom = isBlackPhantom;
      }

      public override void OnBulletDestruction(
        Bullet.DestroyType destroyType,
        SpeculativeRigidbody hitRigidbody,
        bool preventSpawningProjectiles)
      {
        if (preventSpawningProjectiles)
          return;
        float num1 = this.RandomAngle();
        float num2 = 20f;
        for (int index = 0; index < 18; ++index)
        {
          Bullet bullet = new Bullet();
          this.Fire(new Brave.BulletScript.Direction(num1 + (float) index * num2), new Brave.BulletScript.Speed(7f), bullet);
          if (!this.m_isBlackPhantom)
          {
            bullet.ForceBlackBullet = false;
            bullet.Projectile.ForceBlackBullet = false;
            bullet.Projectile.ReturnFromBlackBullet();
          }
        }
      }
    }
  }

