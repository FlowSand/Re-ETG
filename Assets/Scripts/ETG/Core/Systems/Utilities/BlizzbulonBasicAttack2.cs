using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

[InspectorDropdownName("Blizzbulon/BasicAttack2")]
public class BlizzbulonBasicAttack2 : Script
  {
    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new BlizzbulonBasicAttack2__Topc__Iterator0()
      {
        _this = this
      };
    }
  }

