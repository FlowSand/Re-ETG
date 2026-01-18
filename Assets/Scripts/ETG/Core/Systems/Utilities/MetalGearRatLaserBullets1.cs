using System.Collections;
using System.Diagnostics;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/MetalGearRat/LaserBullets1")]
public class MetalGearRatLaserBullets1 : Script
    {
        private const int NumBullets = 12;

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new MetalGearRatLaserBullets1__Topc__Iterator0()
            {
                _this = this
            };
        }

        public class LaserBullet : Bullet
        {
            public LaserBullet()
                : base()
            {
            }

            [DebuggerHidden]
            protected override IEnumerator Top()
            {
                // ISSUE: object of a compiler-generated type is created
                return (IEnumerator) new MetalGearRatLaserBullets1.LaserBullet__Topc__Iterator0()
                {
                    _this = this
                };
            }
        }
    }

