using System.Collections;
using System.Diagnostics;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/BossFinalBullet/AgunimArc1")]
public class BossFinalBulletAgunimArc1 : Script
    {
        private const float NumBullets = 19f;
        private const int ArcTime = 15;
        private const float EllipseA = 2.25f;
        private const float EllipseB = 1.5f;

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new BossFinalBulletAgunimArc1__Topc__Iterator0()
            {
                _this = this
            };
        }
    }

