using System.Collections;
using System.Diagnostics;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/Lich/SpinFire1")]
public class LichSpinFire1 : Script
    {
        private const int NumWaves = 60;
        private const int NumBulletsPerWave = 6;
        private const float AngleDeltaEachWave = 9f;

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new LichSpinFire1__Topc__Iterator0()
            {
                _this = this
            };
        }
    }

