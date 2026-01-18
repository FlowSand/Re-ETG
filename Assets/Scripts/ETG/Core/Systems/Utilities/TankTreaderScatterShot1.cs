using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

[InspectorDropdownName("Bosses/TankTreader/ScatterShot1")]
public class TankTreaderScatterShot1 : Script
  {
    private const int AirTime = 30;
    private const int NumDeathBullets = 16 /*0x10*/;

    protected override IEnumerator Top()
    {
      this.Fire(new Brave.BulletScript.Direction(type: Brave.BulletScript.DirectionType.Aim), new Brave.BulletScript.Speed(12f), (Bullet) new TankTreaderScatterShot1.ScatterBullet());
      return (IEnumerator) null;
    }

    private class ScatterBullet : Bullet
    {
      public ScatterBullet()
        : base("scatterBullet")
      {
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new TankTreaderScatterShot1.ScatterBullet__Topc__Iterator0()
        {
          _this = this
        };
      }
    }

    private class LittleScatterBullet : Bullet
    {
      public LittleScatterBullet()
        : base()
      {
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new TankTreaderScatterShot1.LittleScatterBullet__Topc__Iterator0()
        {
          _this = this
        };
      }
    }
  }

