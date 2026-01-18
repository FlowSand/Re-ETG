using System.Collections;
using System.Diagnostics;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/Meduzi/UziFire1")]
    public abstract class MeduziUziFire1 : Script
    {
        private const int NumBullets = 60;

[DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new MeduziUziFire1__Topc__Iterator0()
            {
                _this = this
            };
        }

        protected abstract string UnityAnimationName { get; }
    }

