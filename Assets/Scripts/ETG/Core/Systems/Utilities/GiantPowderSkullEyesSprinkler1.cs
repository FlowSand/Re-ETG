using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

[InspectorDropdownName("Bosses/GiantPowderSkull/EyesSprinkler1")]
public class GiantPowderSkullEyesSprinkler1 : Script
  {
    private const int NumBullets = 75;
    private const float DeltaAngle1 = 12f;
    private const float DeltaAngle2 = 16f;

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new GiantPowderSkullEyesSprinkler1__Topc__Iterator0()
      {
        _this = this
      };
    }
  }

