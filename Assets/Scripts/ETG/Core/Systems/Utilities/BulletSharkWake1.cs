using System.Collections;
using System.Diagnostics;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("BulletShark/Wake1")]
public class BulletSharkWake1 : Script
    {
        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new BulletSharkWake1__Topc__Iterator0()
            {
                _this = this
            };
        }
    }

