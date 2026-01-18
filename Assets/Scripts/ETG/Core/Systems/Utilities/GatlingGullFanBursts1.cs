using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

[InspectorDropdownName("Bosses/GatlingGull/FanBursts1")]
public class GatlingGullFanBursts1 : Script
  {
    private const int NumWaves = 2;
    private const int NumBulletsPerWave = 20;
    private const float WaveArcLength = 130f;

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new GatlingGullFanBursts1__Topc__Iterator0()
      {
        _this = this
      };
    }
  }

