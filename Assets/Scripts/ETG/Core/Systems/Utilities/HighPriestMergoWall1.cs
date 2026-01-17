// Decompiled with JetBrains decompiler
// Type: HighPriestMergoWall1
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
    [InspectorDropdownName("Bosses/HighPriest/MergoWall1")]
    public class HighPriestMergoWall1 : Script
    {
      private const int NumBullets = 20;

      protected override IEnumerator Top()
      {
        for (int index = 0; index < 20; ++index)
          this.Fire(new Brave.BulletScript.Direction(type: Brave.BulletScript.DirectionType.Relative), new Brave.BulletScript.Speed(4f), (Bullet) new HighPriestMergoWall1.BigBullet());
        return (IEnumerator) null;
      }

      public class BigBullet : Bullet
      {
        public BigBullet()
          : base("mergoWall")
        {
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new HighPriestMergoWall1.BigBullet.<Top>c__Iterator0()
          {
            _this = this
          };
        }
      }
    }

}
