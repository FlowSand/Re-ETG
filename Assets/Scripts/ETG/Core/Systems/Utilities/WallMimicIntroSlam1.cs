using System.Collections;
using System.Diagnostics;

using FullInspector;

#nullable disable

[InspectorDropdownName("MimicWall/IntroSlam1")]
public class WallMimicIntroSlam1 : WallMimicSlam1
    {
        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new WallMimicIntroSlam1__Topc__Iterator0()
            {
                _this = this
            };
        }
    }

