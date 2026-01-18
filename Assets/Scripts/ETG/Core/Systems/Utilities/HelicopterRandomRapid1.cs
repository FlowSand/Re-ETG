using System.Collections;
using System.Diagnostics;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/Helicopter/RandomBurstsRapid1")]
public class HelicopterRandomRapid1 : Script
    {
        private const int NumBullets = 6;
        private static string[] Transforms = new string[4]
        {
            "shoot point 1",
            "shoot point 2",
            "shoot point 3",
            "shoot point 4"
        };

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new HelicopterRandomRapid1__Topc__Iterator0()
            {
                _this = this
            };
        }
    }

