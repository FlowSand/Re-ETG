using System.Collections;
using System.Diagnostics;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("R2G2/Burst1")]
public class R2G2Burst1 : Script
    {
        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new R2G2Burst1__Topc__Iterator0()
            {
                _this = this
            };
        }
    }

