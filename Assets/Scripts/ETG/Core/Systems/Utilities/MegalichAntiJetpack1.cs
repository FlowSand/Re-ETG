using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

[InspectorDropdownName("Bosses/Megalich/AntiJetpack1")]
public class MegalichAntiJetpack1 : Script
  {
    private const int NumBullets = 30;
    private const int NumLines = 4;
    private const float RoomHalfWidth = 20f;

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new MegalichAntiJetpack1__Topc__Iterator0()
      {
        _this = this
      };
    }
  }

