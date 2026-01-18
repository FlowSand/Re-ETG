using System.Collections;
using System.Diagnostics;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/TankTreader/GuySpray1")]
public class TankTreaderGuySpray1 : Script
    {
        private const int NumBullets = 42;

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new TankTreaderGuySpray1__Topc__Iterator0()
            {
                _this = this
            };
        }
    }

