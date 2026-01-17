// Decompiled with JetBrains decompiler
// Type: MetalGearRatMissiles1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

namespace ETG.Core.Combat.Projectiles
{
    [InspectorDropdownName("Bosses/MetalGearRat/Missiles1")]
    public class MetalGearRatMissiles1 : Script
    {
      private const int NumDeathBullets = 8;
      private static int[] xOffsets = new int[8]
      {
        0,
        -4,
        -7,
        -11,
        -14,
        -18,
        -21,
        -28
      };
      private static int[] yOffsets = new int[8]
      {
        0,
        -7,
        0,
        -7,
        0,
        -7,
        0,
        0
      };

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new MetalGearRatMissiles1__Topc__Iterator0()
        {
          _this = this
        };
      }

      private class HomingBullet : Bullet
      {
        private int m_fireDelay;

        public HomingBullet(int fireDelay = 0)
          : base("missile")
        {
          this.m_fireDelay = fireDelay;
        }

        public override void Initialize()
        {
          this.Projectile.spriteAnimator.StopAndResetFrameToDefault();
          BraveUtility.EnableEmission(this.Projectile.ParticleTrail, false);
          base.Initialize();
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new MetalGearRatMissiles1.HomingBullet__Topc__Iterator0()
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
          float num2 = 45f;
          for (int index = 0; index < 8; ++index)
          {
            this.Fire(new Brave.BulletScript.Direction(num1 + num2 * (float) index), new Brave.BulletScript.Speed(11f), (Bullet) null);
            this.PostWwiseEvent("Play_WPN_smallrocket_impact_01");
          }
        }
      }
    }

}
