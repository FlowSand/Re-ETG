using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

[InspectorDropdownName("Bosses/HighPriest/CrossRandom1")]
public class HighPriestCrossRandom1 : Script
  {
    private const int NumBullets = 120;

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new HighPriestCrossRandom1__Topc__Iterator0()
      {
        _this = this
      };
    }
  }

