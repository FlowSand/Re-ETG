using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

[InspectorDropdownName("Bosses/BulletKing/DirectedFireLeft")]
public class BulletKingDirectedFireLeft : BulletKingDirectedFire
  {
    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new BulletKingDirectedFireLeft__Topc__Iterator0()
      {
        _this = this
      };
    }
  }

