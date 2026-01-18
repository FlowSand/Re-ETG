using System.Collections;
using System.Diagnostics;

using Brave.BulletScript;

#nullable disable

public class MetBurst1 : Script
    {
        private const int NumBullets = 3;

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new MetBurst1__Topc__Iterator0()
            {
                _this = this
            };
        }
    }

