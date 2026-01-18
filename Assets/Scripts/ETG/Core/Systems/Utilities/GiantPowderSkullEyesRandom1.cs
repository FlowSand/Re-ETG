using System.Collections;
using System.Diagnostics;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/GiantPowderSkull/EyesRandom1")]
public class GiantPowderSkullEyesRandom1 : Script
    {
        private const int NumBullets = 50;
        private const float BulletRange = 150f;

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new GiantPowderSkullEyesRandom1__Topc__Iterator0()
            {
                _this = this
            };
        }
    }

