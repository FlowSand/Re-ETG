using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

[InspectorDropdownName("Bosses/BossFinalMarine/SpinFire1")]
public class BossFinalMarineSpinFire1 : Script
  {
    private const int NumWaves = 25;
    private const int NumBulletsPerWave = 6;
    private const float AngleDeltaEachWave = 37f;
    private const int NumBulletsFinalWave = 64 /*0x40*/;

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new BossFinalMarineSpinFire1__Topc__Iterator0()
      {
        _this = this
      };
    }
  }

