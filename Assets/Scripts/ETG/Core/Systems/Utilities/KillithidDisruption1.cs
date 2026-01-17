// Decompiled with JetBrains decompiler
// Type: KillithidDisruption1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using System.Collections;
using System.Diagnostics;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class KillithidDisruption1 : Script
    {
      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new KillithidDisruption1.<Top>c__Iterator0()
        {
          _this = this
        };
      }

      public class AstralBullet : Bullet
      {
        public AstralBullet()
          : base("disruption")
        {
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new KillithidDisruption1.AstralBullet.<Top>c__Iterator0()
          {
            _this = this
          };
        }
      }
    }

}
