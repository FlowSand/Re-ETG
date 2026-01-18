using System.Collections;
using System.Diagnostics;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/Meduzi/SpinFire1")]
public class MeduziSpinFire1 : Script
    {
        private const int NumWaves = 29;
        private const int NumBulletsPerWave = 6;
        private const float AngleDeltaEachWave = -37f;

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new MeduziSpinFire1__Topc__Iterator0()
            {
                _this = this
            };
        }
    }

