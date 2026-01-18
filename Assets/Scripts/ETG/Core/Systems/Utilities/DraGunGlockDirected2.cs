using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

[InspectorDropdownName("Bosses/DraGun/GlockDirected2")]
public class DraGunGlockDirected2 : Script
  {
    protected virtual string BulletName => "glock";

    protected virtual bool IsHard => false;

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new DraGunGlockDirected2__Topc__Iterator0()
      {
        _this = this
      };
    }
  }

