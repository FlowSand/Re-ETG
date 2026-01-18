using System.Collections;
using System.Diagnostics;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/Bashellisk/RandomLeadShots1")]
public class BashelliskRandomLeadShots1 : Script
    {
        public int NumBullets = 10;
        public float BulletSpeed = 14f;

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new BashelliskRandomLeadShots1__Topc__Iterator0()
            {
                _this = this
            };
        }
    }

