using System.Collections;
using System.Diagnostics;

using Brave.BulletScript;

#nullable disable

public class BabyDragunBurst1 : Script
    {
        private const int NumBullets = 7;
        private const float HalfArc = 15f;

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new BabyDragunBurst1__Topc__Iterator0()
            {
                _this = this
            };
        }
    }

