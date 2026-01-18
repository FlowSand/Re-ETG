using System.Collections;
using System.Diagnostics;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/GiantPowderSkull/CannonVolley1")]
public class GiantPowderSkullCannonVolley1 : Script
    {
        private const int NumBullets = 5;
        private const float HalfWidth = 4.5f;

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new GiantPowderSkullCannonVolley1__Topc__Iterator0()
            {
                _this = this
            };
        }
    }

