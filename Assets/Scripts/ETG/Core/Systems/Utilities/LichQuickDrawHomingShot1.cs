using System.Collections;
using System.Diagnostics;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/Lich/QuickDrawHomingShot1")]
public class LichQuickDrawHomingShot1 : Script
    {
        protected override IEnumerator Top()
        {
            this.Fire(new Brave.BulletScript.Direction(this.GetAimDirection(1f, 16f)), new Brave.BulletScript.Speed(16f), (Bullet) new LichQuickDrawHomingShot1.FastHomingShot());
            return (IEnumerator) null;
        }

        public class FastHomingShot : Bullet
        {
            public FastHomingShot()
                : base("quickHoming")
            {
            }

            [DebuggerHidden]
            protected override IEnumerator Top()
            {
                // ISSUE: object of a compiler-generated type is created
                return (IEnumerator) new LichQuickDrawHomingShot1.FastHomingShot__Topc__Iterator0()
                {
                    _this = this
                };
            }
        }
    }

