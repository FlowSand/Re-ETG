using System.Collections;
using System.Diagnostics;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/BossDoorMimic/Waves1")]
public class BossDoorMimicWaves1 : Script
    {
        private const int NumWaves = 7;
        private const int NumBulletsPerWave = 5;
        private const float Arc = 60f;

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new BossDoorMimicWaves1__Topc__Iterator0()
            {
                _this = this
            };
        }
    }

