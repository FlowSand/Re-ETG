using System.Collections;
using System.Diagnostics;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/GiantPowderSkull/RollSlam1")]
public class GiantPowderSkullRollSlam1 : Script
    {
        private const float OffsetDist = 1.5f;

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new GiantPowderSkullRollSlam1__Topc__Iterator0()
            {
                _this = this
            };
        }
    }

