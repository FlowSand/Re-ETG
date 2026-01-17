// Decompiled with JetBrains decompiler
// Type: BossFinalBulletGunonQuickShot1
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
    [InspectorDropdownName("Bosses/BossFinalBullet/GunonQuickShot1")]
    public class BossFinalBulletGunonQuickShot1 : Script
    {
      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BossFinalBulletGunonQuickShot1.\u003CTop\u003Ec__Iterator0()
        {
          \u0024this = this
        };
      }

      public class BatBullet : Bullet
      {
        public BatBullet()
          : base("hipbat")
        {
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new BossFinalBulletGunonQuickShot1.BatBullet.\u003CTop\u003Ec__Iterator0()
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
          return (IEnumerator) new BossFinalBulletGunonQuickShot1.FireBullet.\u003CTop\u003Ec__Iterator0()
          {
            \u0024this = this
          };
        }
      }
    }

}
