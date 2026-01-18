using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

[InspectorDropdownName("Bosses/BossFinalRobot/M16")]
public class BossFinalRobotM16 : Script
  {
    private const float NumBullets = 23f;
    private const int ArcTime = 54;
    private const float ShotVariance = 6f;
    private const float EllipseA = 2.92f;
    private const float EllipseB = 2.03f;

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new BossFinalRobotM16__Topc__Iterator0()
      {
        _this = this
      };
    }
  }

