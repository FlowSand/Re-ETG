using System.Collections;
using System.Diagnostics;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/BossFinalRobot/Uzi1")]
public class BossFinalRobotUzi1 : Script
    {
        private const float NumBullets = 70f;
        private float NarrowAngle = 60f;
        private float NarrowAngleChance = 0.5f;

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new BossFinalRobotUzi1__Topc__Iterator0()
            {
                _this = this
            };
        }
    }

