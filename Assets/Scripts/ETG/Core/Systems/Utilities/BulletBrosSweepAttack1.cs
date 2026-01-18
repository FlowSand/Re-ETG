using System.Collections;
using System.Diagnostics;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/BulletBros/SweepAttack1")]
public class BulletBrosSweepAttack1 : Script
    {
        private const int NumBullets = 15;
        private const float ArcDegrees = 60f;

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new BulletBrosSweepAttack1__Topc__Iterator0()
            {
                _this = this
            };
        }
    }

