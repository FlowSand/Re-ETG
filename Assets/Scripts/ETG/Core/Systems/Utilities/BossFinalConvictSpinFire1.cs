using System.Collections;
using System.Diagnostics;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/BossFinalConvict/SpinFire1")]
public class BossFinalConvictSpinFire1 : Script
    {
        private const int NumBullets = 48 /*0x30*/;

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new BossFinalConvictSpinFire1__Topc__Iterator0()
            {
                _this = this
            };
        }
    }

