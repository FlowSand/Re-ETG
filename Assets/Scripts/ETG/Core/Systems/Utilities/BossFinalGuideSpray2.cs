using System.Collections;
using System.Diagnostics;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/BossFinalGuide/Spray2")]
public class BossFinalGuideSpray2 : Script
    {
        private const int NumBullets = 40;
        private const float SprayAngle = 110f;

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new BossFinalGuideSpray2__Topc__Iterator0()
            {
                _this = this
            };
        }
    }

