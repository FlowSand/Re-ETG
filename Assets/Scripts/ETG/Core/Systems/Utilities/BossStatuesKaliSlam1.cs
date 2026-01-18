using System.Collections;
using System.Diagnostics;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/BossStatues/KaliSlam1")]
public class BossStatuesKaliSlam1 : Script
    {
        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new BossStatuesKaliSlam1__Topc__Iterator0()
            {
                _this = this
            };
        }

        public class SpiralBullet1 : Bullet
        {
            public SpiralBullet1()
                : base("spiralbullet1")
            {
            }

            [DebuggerHidden]
            protected override IEnumerator Top()
            {
                // ISSUE: object of a compiler-generated type is created
                return (IEnumerator) new BossStatuesKaliSlam1.SpiralBullet1__Topc__Iterator0()
                {
                    _this = this
                };
            }
        }

        public class SpiralBullet2 : Bullet
        {
            public SpiralBullet2()
                : base("spiralbullet2")
            {
            }

            [DebuggerHidden]
            protected override IEnumerator Top()
            {
                // ISSUE: object of a compiler-generated type is created
                return (IEnumerator) new BossStatuesKaliSlam1.SpiralBullet2__Topc__Iterator0()
                {
                    _this = this
                };
            }
        }

        public class SpiralBullet3 : Bullet
        {
            public SpiralBullet3()
                : base("spiralbullet3")
            {
            }

            [DebuggerHidden]
            protected override IEnumerator Top()
            {
                // ISSUE: object of a compiler-generated type is created
                return (IEnumerator) new BossStatuesKaliSlam1.SpiralBullet3__Topc__Iterator0()
                {
                    _this = this
                };
            }
        }
    }

