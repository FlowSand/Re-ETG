using Brave.BulletScript;
using System.Collections;
using System.Diagnostics;

#nullable disable

public class ForgeHammerCircle1 : Script
  {
    public int CircleBullets = 12;

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new ForgeHammerCircle1__Topc__Iterator0()
      {
        _this = this
      };
    }

    public class DefaultBullet : Bullet
    {
      public int spawnTime;

      public DefaultBullet(int spawnTime)
        : base()
      {
        this.spawnTime = spawnTime;
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ForgeHammerCircle1.DefaultBullet__Topc__Iterator0()
        {
          _this = this
        };
      }
    }
  }

