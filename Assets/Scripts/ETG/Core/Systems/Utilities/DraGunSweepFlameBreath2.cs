using System.Collections;
using System.Diagnostics;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/DraGun/SweepFlameBreath2")]
public class DraGunSweepFlameBreath2 : Script
    {
        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new DraGunSweepFlameBreath2__Topc__Iterator0()
            {
                _this = this
            };
        }
    }

