using System.Collections;
using System.Diagnostics;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/HighPriest/CrossSprinkler1")]
public class HighPriestCrossSprinkler1 : Script
    {
        private const int NumBullets = 105;

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new HighPriestCrossSprinkler1__Topc__Iterator0()
            {
                _this = this
            };
        }
    }

