using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

[InspectorDropdownName("BulletShark/BigWake1")]
public class BulletSharkBigWake1 : Script
  {
    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new BulletSharkBigWake1__Topc__Iterator0()
      {
        _this = this
      };
    }
  }

