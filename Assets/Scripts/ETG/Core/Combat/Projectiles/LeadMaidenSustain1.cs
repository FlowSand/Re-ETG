// Decompiled with JetBrains decompiler
// Type: LeadMaidenSustain1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Combat.Projectiles
{
    public class LeadMaidenSustain1 : Script
    {
      private const int NumWaves = 3;
      private const int NumBullets = 12;

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new LeadMaidenSustain1.<Top>c__Iterator0()
        {
          $this = this
        };
      }

      public class SpikeBullet : Bullet
      {
        private int m_fireTick;
        private float m_hitNormal;

        public SpikeBullet(int fireTick)
          : base()
        {
          this.m_fireTick = fireTick;
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new LeadMaidenSustain1.SpikeBullet.<Top>c__Iterator0()
          {
            $this = this
          };
        }

        private void OnCollision(CollisionData tileCollision)
        {
          this.Speed = 0.0f;
          this.m_hitNormal = tileCollision.Normal.ToAngle();
          PhysicsEngine.PostSliceVelocity = new Vector2?(new Vector2());
          this.Projectile.specRigidbody.OnCollision -= new Action<CollisionData>(this.OnCollision);
          if (!(bool) (UnityEngine.Object) tileCollision.OtherRigidbody)
            return;
          this.Vanish();
        }

        public override void OnBulletDestruction(
          Bullet.DestroyType destroyType,
          SpeculativeRigidbody hitRigidbody,
          bool preventSpawningProjectiles)
        {
          if (!(bool) (UnityEngine.Object) this.Projectile)
            return;
          this.Projectile.specRigidbody.OnCollision -= new Action<CollisionData>(this.OnCollision);
        }
      }
    }

}
