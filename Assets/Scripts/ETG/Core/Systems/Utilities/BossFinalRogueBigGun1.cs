using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

[InspectorDropdownName("Bosses/BossFinalRogue/BigGun1")]
public class BossFinalRogueBigGun1 : Script
  {
    private const float NumBullets = 26f;
    private const float NumFastBullets = 44f;

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new BossFinalRogueBigGun1__Topc__Iterator0()
      {
        _this = this
      };
    }
  }

