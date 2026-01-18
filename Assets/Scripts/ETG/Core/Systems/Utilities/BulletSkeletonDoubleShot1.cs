using System.Collections;
using System.Diagnostics;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("BulletSkeleton/DoubleShot1")]
public class BulletSkeletonDoubleShot1 : Script
    {
        protected virtual bool IsHard => false;

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new BulletSkeletonDoubleShot1__Topc__Iterator0()
            {
                _this = this
            };
        }
    }

