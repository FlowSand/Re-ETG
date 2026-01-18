using System.Collections;
using System.Diagnostics;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/BossFinalBullet/AgunimReflect1")]
public class BossFinalBulletAgunimReflect1 : Script
    {
        private const float FakeChance = 0.33f;
        private static bool WasLastShotFake;
        private const int FakeNumBullets = 5;
        private const float FakeRadius = 0.55f;
        private const float FakeSpinSpeed = 450f;

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new BossFinalBulletAgunimReflect1__Topc__Iterator0()
            {
                _this = this
            };
        }

        public class RingBullet : Bullet
        {
            private float m_angle;

            public RingBullet(float angle = 0.0f)
                : base("ring")
            {
                this.m_angle = angle;
            }

            [DebuggerHidden]
            protected override IEnumerator Top()
            {
                // ISSUE: object of a compiler-generated type is created
                return (IEnumerator) new BossFinalBulletAgunimReflect1.RingBullet__Topc__Iterator0()
                {
                    _this = this
                };
            }
        }
    }

