using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

[InspectorDropdownName("Bosses/MetalGearRat/JumpPound1")]
public class MetalGearRatJumpPound1 : Script
  {
    private const int NumWaves = 3;
    private const int NumBullets = 43;
    private const float EllipseA = 6f;
    private const float EllipseB = 2f;

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new MetalGearRatJumpPound1__Topc__Iterator0()
      {
        _this = this
      };
    }
  }

