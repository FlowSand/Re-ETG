using System.Collections;
using System.Diagnostics;

using FullInspector;

#nullable disable

[InspectorDropdownName("ManfredsRival/ShieldSlam2")]
public class ManfredsRivalShieldSlam2 : ManfredsRivalShieldSlam1
    {
        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new ManfredsRivalShieldSlam2__Topc__Iterator0()
            {
                _this = this
            };
        }
    }

