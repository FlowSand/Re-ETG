using System.Collections;
using System.Diagnostics;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/DraGun/Mac10Burst1")]
public class DraGunMac10Burst1 : Script
    {
        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new DraGunMac10Burst1__Topc__Iterator0()
            {
                _this = this
            };
        }
    }

