using System.Collections;
using System.Diagnostics;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/GatlingGull/FanSpray1")]
public class GatlingGullFanSpray1 : Script
    {
        private const float SprayAngle = 90f;
        private const float SpraySpeed = 150f;
        private const int SprayIterations = 4;

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new GatlingGullFanSpray1__Topc__Iterator0()
            {
                _this = this
            };
        }
    }

