using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

[InspectorDropdownName("Bosses/BulletBros/TridentAttack1")]
public class BulletBrosTridentAttack1 : Script
  {
    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new BulletBrosTridentAttack1__Topc__Iterator0()
      {
        _this = this
      };
    }
  }

