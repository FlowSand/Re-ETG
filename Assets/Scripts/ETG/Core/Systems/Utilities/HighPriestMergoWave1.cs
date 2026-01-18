using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

[InspectorDropdownName("Bosses/HighPriest/MergoWave1")]
public class HighPriestMergoWave1 : Script
  {
    private const int NumBullets = 15;
    private const float Angle = 120f;

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new HighPriestMergoWave1__Topc__Iterator0()
      {
        _this = this
      };
    }
  }

