// Decompiled with JetBrains decompiler
// Type: BulletManMagicAstral2
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
    [InspectorDropdownName("BulletManMagic/Astral2")]
    public class BulletManMagicAstral2 : Script
    {
      protected override IEnumerator Top()
      {
        this.Fire((Bullet) new BulletManMagicAstral2.AstralBullet());
        return (IEnumerator) null;
      }

      public class AstralBullet : Bullet
      {
        private const int NumBullets = 18;

        public AstralBullet()
          : base("astral")
        {
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new BulletManMagicAstral2.AstralBullet.\u003CTop\u003Ec__Iterator0()
          {
            \u0024this = this
          };
        }
      }
    }

}
