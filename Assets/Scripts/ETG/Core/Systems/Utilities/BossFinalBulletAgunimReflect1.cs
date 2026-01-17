// Decompiled with JetBrains decompiler
// Type: BossFinalBulletAgunimReflect1
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
    [InspectorDropdownName("Bosses/BossFinalBullet/AgunimReflect1")]
    public class BossFinalBulletAgunimReflect1 : Script
    {
      private const float FakeChance = 0.33f;
      private static bool WasLastShotFake;
      private const int FakeNumBullets = 5;
      private const float FakeRadius = 0.55f;
      private const float FakeSpinSpeed = 450f;

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BossFinalBulletAgunimReflect1.<Top>c__Iterator0()
        {
          _this = this
        };
      }

      public class RingBullet : Bullet
      {
        private float m_angle;

        public RingBullet(float angle = 0.0f)
          : base("ring")
        {
          this.m_angle = angle;
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new BossFinalBulletAgunimReflect1.RingBullet.<Top>c__Iterator0()
          {
            _this = this
          };
        }
      }
    }

}
