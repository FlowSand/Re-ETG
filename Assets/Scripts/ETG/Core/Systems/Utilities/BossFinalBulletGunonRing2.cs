// Decompiled with JetBrains decompiler
// Type: BossFinalBulletGunonRing2
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
    [InspectorDropdownName("Bosses/BossFinalBullet/GunonRing2")]
    public class BossFinalBulletGunonRing2 : Script
    {
      private const float ExpandSpeed = 3f;
      private const float ExpandAcceleration = -0.3f;
      private const float RotationalSpeed = 13f;

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BossFinalBulletGunonRing2.\u003CTop\u003Ec__Iterator0()
        {
          \u0024this = this
        };
      }

      public class BatBullet : Bullet
      {
        private float m_angle;
        private int m_index;
        private BossFinalBulletGunonRing1 m_parentScript;

        public BatBullet()
          : base("bat")
        {
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new BossFinalBulletGunonRing2.BatBullet.\u003CTop\u003Ec__Iterator0()
          {
            \u0024this = this
          };
        }
      }

      public class FireBullet : Bullet
      {
        public FireBullet()
          : base("fire")
        {
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new BossFinalBulletGunonRing2.FireBullet.\u003CTop\u003Ec__Iterator0()
          {
            \u0024this = this
          };
        }
      }
    }

}
