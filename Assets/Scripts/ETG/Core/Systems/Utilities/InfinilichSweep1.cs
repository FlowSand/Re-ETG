using System.Collections;
using System.Diagnostics;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/Infinilich/Sweep1")]
public class InfinilichSweep1 : Script
    {
        private bool m_isFinished;

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new InfinilichSweep1__Topc__Iterator0()
            {
                _this = this
            };
        }

        [DebuggerHidden]
        private IEnumerator VerticalAttacks()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new InfinilichSweep1__VerticalAttacksc__Iterator1()
            {
                _this = this
            };
        }
    }

