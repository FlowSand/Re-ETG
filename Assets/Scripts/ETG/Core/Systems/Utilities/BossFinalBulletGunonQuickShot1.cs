using System.Collections;
using System.Diagnostics;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/BossFinalBullet/GunonQuickShot1")]
public class BossFinalBulletGunonQuickShot1 : Script
    {
        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new BossFinalBulletGunonQuickShot1__Topc__Iterator0()
            {
                _this = this
            };
        }

        public class BatBullet : Bullet
        {
            public BatBullet()
                : base("hipbat")
            {
            }

            [DebuggerHidden]
            protected override IEnumerator Top()
            {
                // ISSUE: object of a compiler-generated type is created
                return (IEnumerator) new BossFinalBulletGunonQuickShot1.BatBullet__Topc__Iterator0()
                {
                    _this = this
                };
            }
        }

        public class FireBullet : Bullet
        {
            public FireBullet()
                : base("fire")
            {
            }

            [DebuggerHidden]
            protected override IEnumerator Top()
            {
                // ISSUE: object of a compiler-generated type is created
                return (IEnumerator) new BossFinalBulletGunonQuickShot1.FireBullet__Topc__Iterator0()
                {
                    _this = this
                };
            }
        }
    }

