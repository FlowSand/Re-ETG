using System.Collections;
using System.Diagnostics;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/HighPriest/FaceShoot1")]
public class HighPriestFaceShoot1 : Script
    {
        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new HighPriestFaceShoot1__Topc__Iterator0()
            {
                _this = this
            };
        }

        public class FastHomingShot : Bullet
        {
            public FastHomingShot()
                : base("quickHoming")
            {
            }

            [DebuggerHidden]
            protected override IEnumerator Top()
            {
                // ISSUE: object of a compiler-generated type is created
                return (IEnumerator) new HighPriestFaceShoot1.FastHomingShot__Topc__Iterator0()
                {
                    _this = this
                };
            }
        }
    }

