using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

[InspectorDropdownName("Bosses/BossFinalRogue/SquareSpray1")]
public class BossFinalRogueSquareSpray1 : Script
  {
    private const float SprayAngle = 145f;
    private const float SpraySpeed = 120f;
    private const int SprayIterations = 4;

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new BossFinalRogueSquareSpray1__Topc__Iterator0()
      {
        _this = this
      };
    }
  }

