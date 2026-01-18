using Brave.BulletScript;
using System.Collections;
using System.Diagnostics;

#nullable disable

public class KillithidDisruption1 : Script
  {
    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new KillithidDisruption1__Topc__Iterator0()
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
        return (IEnumerator) new KillithidDisruption1.AstralBullet__Topc__Iterator0()
        {
          _this = this
        };
      }
    }
  }

