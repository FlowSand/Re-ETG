using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

[InspectorDropdownName("Bosses/BossDoorMimic/Flames1")]
public class BossDoorMimicFlames1 : Script
  {
    private const int NumBullets = 70;

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new BossDoorMimicFlames1__Topc__Iterator0()
      {
        _this = this
      };
    }
  }

