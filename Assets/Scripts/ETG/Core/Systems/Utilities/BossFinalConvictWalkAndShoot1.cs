using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

[InspectorDropdownName("Bosses/BossFinalConvict/WalkAndShoot1")]
public class BossFinalConvictWalkAndShoot1 : Script
  {
    private const int NumBullets = 100;

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new BossFinalConvictWalkAndShoot1__Topc__Iterator0()
      {
        _this = this
      };
    }
  }

