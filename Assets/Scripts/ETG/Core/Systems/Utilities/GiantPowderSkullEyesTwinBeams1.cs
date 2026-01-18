using System.Collections;
using System.Diagnostics;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/GiantPowderSkull/EyesTwinBeams1")]
public class GiantPowderSkullEyesTwinBeams1 : Script
    {
        private const float CoreSpread = 20f;
        private const float IncSpread = 10f;
        private const float TurnSpeed = 1f;
        private const float BulletSpeed = 18f;

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new GiantPowderSkullEyesTwinBeams1__Topc__Iterator0()
            {
                _this = this
            };
        }
    }

