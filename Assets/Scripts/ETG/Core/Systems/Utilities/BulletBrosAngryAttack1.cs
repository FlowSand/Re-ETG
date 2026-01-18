using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

[InspectorDropdownName("Bosses/BulletBros/AngryAttack1")]
public class BulletBrosAngryAttack1 : Script
  {
    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new BulletBrosAngryAttack1__Topc__Iterator0()
      {
        _this = this
      };
    }
  }

