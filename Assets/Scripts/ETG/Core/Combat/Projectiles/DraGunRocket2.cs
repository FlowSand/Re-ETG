// Decompiled with JetBrains decompiler
// Type: DraGunRocket2
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Combat.Projectiles
{
    [InspectorDropdownName("Bosses/DraGun/Rocket2")]
    public class DraGunRocket2 : Script
    {
      private const int NumBullets = 42;

      protected override IEnumerator Top()
      {
        this.Fire(new Brave.BulletScript.Direction(-90f), new Brave.BulletScript.Speed(40f), (Bullet) new DraGunRocket2.Rocket());
        return (IEnumerator) null;
      }

      public class Rocket : Bullet
      {
        public Rocket()
          : base("rocket")
        {
        }

        protected override IEnumerator Top() => (IEnumerator) null;

        public override void OnBulletDestruction(
          Bullet.DestroyType destroyType,
          SpeculativeRigidbody hitRigidbody,
          bool preventSpawningProjectiles)
        {
          for (int i = 0; i < 42; ++i)
          {
            this.Fire(new Brave.BulletScript.Direction(this.SubdivideArc(-10f, 200f, 42, i)), new Brave.BulletScript.Speed(12f), new Bullet("default_novfx"));
            if (i < 41)
              this.Fire(new Brave.BulletScript.Direction(this.SubdivideArc(-10f, 200f, 42, i, true)), new Brave.BulletScript.Speed(8f), (Bullet) new SpeedChangingBullet("default_novfx", 12f, 60));
            this.Fire(new Brave.BulletScript.Direction(this.SubdivideArc(-10f, 200f, 42, i)), new Brave.BulletScript.Speed(4f), (Bullet) new SpeedChangingBullet("default_novfx", 12f, 60));
          }
          for (int index = 0; index < 5; ++index)
          {
            this.Fire(new Offset(new Vector2(0.0f, -1f), transform: string.Empty), new Brave.BulletScript.Direction(180f), new Brave.BulletScript.Speed((float) (16 /*0x10*/ - index * 4)), (Bullet) new SpeedChangingBullet("default_novfx", 12f, 60));
            this.Fire(new Offset(new Vector2(0.0f, -1f), transform: string.Empty), new Brave.BulletScript.Direction(), new Brave.BulletScript.Speed((float) (16 /*0x10*/ - index * 4)), (Bullet) new SpeedChangingBullet("default_novfx", 12f, 60));
          }
          for (int index = 0; index < 12; ++index)
            this.Fire(new Brave.BulletScript.Direction(index % 2 != 0 ? Random.Range(0.0f, 35f) : Random.Range(150f, 182f)), new Brave.BulletScript.Speed(Random.Range(4f, 12f)), (Bullet) new DraGunRocket2.ShrapnelBullet());
        }
      }

      public class ShrapnelBullet : Bullet
      {
        private const float WiggleMagnitude = 0.75f;
        private const float WigglePeriod = 3f;

        public ShrapnelBullet()
          : base("shrapnel")
        {
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new DraGunRocket2.ShrapnelBullet.\u003CTop\u003Ec__Iterator0()
          {
            \u0024this = this
          };
        }
      }
    }

}
