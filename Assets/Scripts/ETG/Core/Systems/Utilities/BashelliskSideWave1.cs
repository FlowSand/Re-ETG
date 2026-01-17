// Decompiled with JetBrains decompiler
// Type: BashelliskSideWave1
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
    [InspectorDropdownName("Bosses/Bashellisk/SideWave1")]
    public class BashelliskSideWave1 : Script
    {
      protected override IEnumerator Top()
      {
        this.Fire(new Brave.BulletScript.Direction(-90f, Brave.BulletScript.DirectionType.Relative), new Brave.BulletScript.Speed(9f), (Bullet) new BashelliskSideWave1.WaveBullet());
        this.Fire(new Brave.BulletScript.Direction(90f, Brave.BulletScript.DirectionType.Relative), new Brave.BulletScript.Speed(9f), (Bullet) new BashelliskSideWave1.WaveBullet());
        return (IEnumerator) null;
      }

      public class WaveBullet : Bullet
      {
        public WaveBullet()
          : base("bigBullet")
        {
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new BashelliskSideWave1.WaveBullet.<Top>c__Iterator0()
          {
            $this = this
          };
        }
      }
    }

}
