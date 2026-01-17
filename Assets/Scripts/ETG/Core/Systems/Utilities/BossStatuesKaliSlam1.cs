// Decompiled with JetBrains decompiler
// Type: BossStatuesKaliSlam1
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
    [InspectorDropdownName("Bosses/BossStatues/KaliSlam1")]
    public class BossStatuesKaliSlam1 : Script
    {
      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BossStatuesKaliSlam1.<Top>c__Iterator0()
        {
          $this = this
        };
      }

      public class SpiralBullet1 : Bullet
      {
        public SpiralBullet1()
          : base("spiralbullet1")
        {
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new BossStatuesKaliSlam1.SpiralBullet1.<Top>c__Iterator0()
          {
            $this = this
          };
        }
      }

      public class SpiralBullet2 : Bullet
      {
        public SpiralBullet2()
          : base("spiralbullet2")
        {
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new BossStatuesKaliSlam1.SpiralBullet2.<Top>c__Iterator0()
          {
            $this = this
          };
        }
      }

      public class SpiralBullet3 : Bullet
      {
        public SpiralBullet3()
          : base("spiralbullet3")
        {
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new BossStatuesKaliSlam1.SpiralBullet3.<Top>c__Iterator0()
          {
            $this = this
          };
        }
      }
    }

}
