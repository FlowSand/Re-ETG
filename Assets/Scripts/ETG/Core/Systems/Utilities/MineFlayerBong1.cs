using System.Collections;
using System.Diagnostics;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/MineFlayer/Bong1")]
public class MineFlayerBong1 : Script
    {
        private const int NumBullets = 90;

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new MineFlayerBong1__Topc__Iterator0()
            {
                _this = this
            };
        }
    }

