using System.Collections;
using System.Diagnostics;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/DraGun/FlameBreath1")]
public class DraGunFlameBreath1 : Script
    {
        private const int NumBullets = 80;

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new DraGunFlameBreath1__Topc__Iterator0()
            {
                _this = this
            };
        }
    }

