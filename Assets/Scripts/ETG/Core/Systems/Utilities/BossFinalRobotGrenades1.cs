using System.Collections;
using System.Diagnostics;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/BossFinalRobot/Grenades1")]
public class BossFinalRobotGrenades1 : Script
    {
        private const int NumBullets = 4;

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new BossFinalRobotGrenades1__Topc__Iterator0()
            {
                _this = this
            };
        }
    }

