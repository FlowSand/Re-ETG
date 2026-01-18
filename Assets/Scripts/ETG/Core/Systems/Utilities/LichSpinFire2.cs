using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

[InspectorDropdownName("Bosses/Lich/SpinFire2")]
public class LichSpinFire2 : Script
  {
    private const int NumWaves = 6;
    private const int NumBulletsPerWave = 48 /*0x30*/;

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new LichSpinFire2__Topc__Iterator0()
      {
        _this = this
      };
    }
  }

