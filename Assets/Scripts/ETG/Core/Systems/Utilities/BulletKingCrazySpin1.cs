using System.Collections;
using System.Diagnostics;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/BulletKing/CrazySpin1")]
public class BulletKingCrazySpin1 : Script
    {
        private const int NumWaves = 29;
        private const int NumBulletsPerWave = 6;
        private const float AngleDeltaEachWave = 37f;
        private const int NumBulletsFinalWave = 64;

        protected bool IsHard => this is BulletKingCrazySpinHard1;

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new BulletKingCrazySpin1__Topc__Iterator0()
            {
                _this = this
            };
        }
    }

