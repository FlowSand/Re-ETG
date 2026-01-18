using System.Collections;
using System.Diagnostics;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/DraGun/AntiJetpack1")]
public class DraGunAntiJetpack1 : Script
    {
        private const int NumBullets = 30;
        private const int NumLines = 4;
        private const float RoomHalfWidth = 17.5f;

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new DraGunAntiJetpack1__Topc__Iterator0()
            {
                _this = this
            };
        }
    }

