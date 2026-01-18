using System.Collections;
using System.Diagnostics;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/ResourcefulRat/SpinFire1")]
public class ResourcefulRatSpinFire1 : Script
    {
        private const float NumBullets = 23f;
        private const int ArcTime = 70;
        private const float SpreadAngle = 6f;
        private const float BulletSpeed = 16f;
        private const float EllipseA = 1.39f;
        private const float EllipseB = 0.92f;

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new ResourcefulRatSpinFire1__Topc__Iterator0()
            {
                _this = this
            };
        }
    }

