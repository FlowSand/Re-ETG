using System.Collections;
using System.Diagnostics;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/BossFinalGuide/Spray1")]
public class BossFinalGuideSpray1 : Script
    {
        private const float SprayAngle = 90f;
        private const float SpraySpeed = 110f;
        private const int SprayIterations = 3;

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new BossFinalGuideSpray1__Topc__Iterator0()
            {
                _this = this
            };
        }
    }

