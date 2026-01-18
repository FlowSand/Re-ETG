// Decompiled with JetBrains decompiler
// Type: MimicIntroFire1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class MimicIntroFire1 : Script
  {
    protected override IEnumerator Top()
    {
      this.Fire(new Brave.BulletScript.Direction(this.AimDirection), new Brave.BulletScript.Speed(8f), (Bullet) new MimicIntroFire1.BigBullet((bool) (Object) this.BulletBank && (bool) (Object) this.BulletBank.aiActor && this.BulletBank.aiActor.IsBlackPhantom));
      return (IEnumerator) null;
    }

    public class BigBullet : Bullet
    {
      private bool m_isBlackPhantom;

      public BigBullet(bool isBlackPhantom)
        : base("bigbullet")
      {
        this.ForceBlackBullet = true;
        this.m_isBlackPhantom = isBlackPhantom;
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new MimicIntroFire1.BigBullet__Topc__Iterator0()
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
        for (int index = 0; index < 8; ++index)
        {
          Bullet bullet = (Bullet) new SpeedChangingBullet(10f, 120, 600);
          this.Fire(new Brave.BulletScript.Direction((float) (index * 45)), new Brave.BulletScript.Speed(8f), bullet);
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

