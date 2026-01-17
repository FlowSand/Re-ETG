// Decompiled with JetBrains decompiler
// Type: MetalGearRatTailgun1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [InspectorDropdownName("Bosses/MetalGearRat/Tailgun1")]
    public class MetalGearRatTailgun1 : Script
    {
      private const int NumTargetBullets = 16 /*0x10*/;
      private const float TargetRadius = 3f;
      private const float TargetLegLength = 2.5f;
      public const int TargetTrackTime = 360;
      private const float TargetRotationSpeed = 80f;
      private const int BigOneHeight = 30;
      private const int NumDeathWaves = 4;
      private const int NumDeathBullets = 39;

      private bool Center { get; set; }

      private bool Done { get; set; }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new MetalGearRatTailgun1__Topc__Iterator0()
        {
          _this = this
        };
      }

      public class TargetDummy : Bullet
      {
        public TargetDummy()
          : base()
        {
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new MetalGearRatTailgun1.TargetDummy__Topc__Iterator0()
          {
            _this = this
          };
        }
      }

      public class TargetBullet : Bullet
      {
        private MetalGearRatTailgun1 m_parent;
        private MetalGearRatTailgun1.TargetDummy m_targetDummy;

        public TargetBullet(MetalGearRatTailgun1 parent, MetalGearRatTailgun1.TargetDummy targetDummy)
          : base("target")
        {
          this.m_parent = parent;
          this.m_targetDummy = targetDummy;
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new MetalGearRatTailgun1.TargetBullet__Topc__Iterator0()
          {
            _this = this
          };
        }
      }

      private class BigBullet : Bullet
      {
        public BigBullet()
          : base("big_one")
        {
        }

        public override void Initialize()
        {
          this.Projectile.spriteAnimator.StopAndResetFrameToDefault();
          base.Initialize();
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new MetalGearRatTailgun1.BigBullet__Topc__Iterator0()
          {
            _this = this
          };
        }

        public override void OnBulletDestruction(
          Bullet.DestroyType destroyType,
          SpeculativeRigidbody hitRigidbody,
          bool preventSpawningProjectiles)
        {
          if (!preventSpawningProjectiles)
            ;
        }
      }
    }

}
