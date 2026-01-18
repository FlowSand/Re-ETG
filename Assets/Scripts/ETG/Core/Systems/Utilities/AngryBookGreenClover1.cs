using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

[InspectorDropdownName("AngryBook/GreenClover1")]
public class AngryBookGreenClover1 : Script
  {
    public int NumBullets = 60;

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new AngryBookGreenClover1__Topc__Iterator0()
      {
        _this = this
      };
    }

    public class WaveBullet : Bullet
    {
      public int spawnTime;

      public WaveBullet(int spawnTime)
        : base()
      {
        this.spawnTime = spawnTime;
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new AngryBookGreenClover1.WaveBullet__Topc__Iterator0()
        {
          _this = this
        };
      }
    }
  }

