using System.Collections;
using System.Diagnostics;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/Bunker/DirectFire1")]
public class BunkerDirectFire1 : Script
    {
        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new BunkerDirectFire1__Topc__Iterator0()
            {
                _this = this
            };
        }
    }

