using System.Collections;
using System.Diagnostics;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/HighPriest/AmuletPulses1")]
public class HighPriestAmuletPulses1 : Script
    {
        private const int NumBullets = 25;

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new HighPriestAmuletPulses1__Topc__Iterator0()
            {
                _this = this
            };
        }

        public class VibratingBullet : Bullet
        {
            public VibratingBullet()
                : base("amuletRing")
            {
            }

            [DebuggerHidden]
            protected override IEnumerator Top()
            {
                // ISSUE: object of a compiler-generated type is created
                return (IEnumerator) new HighPriestAmuletPulses1.VibratingBullet__Topc__Iterator0()
                {
                    _this = this
                };
            }
        }
    }

