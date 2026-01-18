using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

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
        return (IEnumerator) new BashelliskSideWave1.WaveBullet__Topc__Iterator0()
        {
          _this = this
        };
      }
    }
  }

