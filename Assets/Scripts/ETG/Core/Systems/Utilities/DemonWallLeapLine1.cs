// Decompiled with JetBrains decompiler
// Type: DemonWallLeapLine1
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
    [InspectorDropdownName("Bosses/DemonWall/LeapLine1")]
    public class DemonWallLeapLine1 : Script
    {
      private const int NumBullets = 24;

      protected override IEnumerator Top()
      {
        float num = 1f;
        for (int index = 0; index < 24; ++index)
          this.Fire(new Offset((float) ((double) index * (double) num - 11.5), transform: string.Empty), new Brave.BulletScript.Direction(-90f), new Brave.BulletScript.Speed(5f), (Bullet) new DemonWallLeapLine1.WaveBullet());
        return (IEnumerator) null;
      }

      private class WaveBullet : Bullet
      {
        private const float SinPeriod = 0.75f;
        private const float SinMagnitude = 1.5f;

        public WaveBullet()
          : base("leap")
        {
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new DemonWallLeapLine1.WaveBullet.<Top>c__Iterator0()
          {
            _this = this
          };
        }
      }
    }

}
