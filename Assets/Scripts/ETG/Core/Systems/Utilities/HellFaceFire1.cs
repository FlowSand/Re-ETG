using Brave.BulletScript;
using System.Collections;
using System.Diagnostics;

#nullable disable

public class HellFaceFire1 : Script
  {
    public const int NumEyeBullets = 8;

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new HellFaceFire1__Topc__Iterator0()
      {
        _this = this
      };
    }
  }

