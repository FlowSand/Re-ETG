using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

[InspectorDropdownName("Chancebulon/CriticalHit1")]
public class ChancebulonCriticalHit1 : Script
  {
    private const int NumBullets = 12;

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new ChancebulonCriticalHit1__Topc__Iterator0()
      {
        _this = this
      };
    }
  }

