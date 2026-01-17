// Decompiled with JetBrains decompiler
// Type: BossFinalGuideClap1
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
    [InspectorDropdownName("Bosses/BossFinalGuide/Clap1")]
    public class BossFinalGuideClap1 : Script
    {
      private const int NumBolts = 25;
      private const int BoltSpeed = 20;
      private Vector2 m_roomMin;
      private Vector2 m_roomMax;
      private int[] m_quarters = new int[4]{ 0, 1, 2, 3 };
      private int m_quarterIndex = 4;

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BossFinalGuideClap1.<Top>c__Iterator0()
        {
          $this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator FireBolt()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BossFinalGuideClap1.<FireBolt>c__Iterator1()
        {
          $this = this
        };
      }

      private class LightningBullet : Bullet
      {
        public LightningBullet()
          : base("lightning")
        {
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new BossFinalGuideClap1.LightningBullet.<Top>c__Iterator0()
          {
            $this = this
          };
        }

        public override void OnBulletDestruction(
          Bullet.DestroyType destroyType,
          SpeculativeRigidbody hitRigidbody,
          bool preventSpawningProjectiles)
        {
          if (!(bool) (Object) this.Projectile || !(bool) (Object) this.Projectile.specRigidbody)
            return;
          this.Projectile.specRigidbody.RemoveCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.HighObstacle));
        }
      }
    }

}
