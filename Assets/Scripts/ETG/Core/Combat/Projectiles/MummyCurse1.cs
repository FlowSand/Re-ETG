using System.Collections;
using System.Diagnostics;

using Brave.BulletScript;

#nullable disable

public class MummyCurse1 : Script
    {
        private const int AirTime = 180;

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new MummyCurse1__Topc__Iterator0()
            {
                _this = this
            };
        }

        public class SkullBullet : Bullet
        {
            private Script m_parentScript;

            public SkullBullet(Script parentScript)
                : base("skull")
            {
                this.m_parentScript = parentScript;
            }

            [DebuggerHidden]
            protected override IEnumerator Top()
            {
                // ISSUE: object of a compiler-generated type is created
                return (IEnumerator) new MummyCurse1.SkullBullet__Topc__Iterator0()
                {
                    _this = this
                };
            }

            public override void OnBulletDestruction(
                Bullet.DestroyType destroyType,
                SpeculativeRigidbody hitRigidbody,
                bool preventSpawningProjectiles)
            {
                if (this.m_parentScript == null)
                    return;
                this.m_parentScript.ForceEnd();
            }
        }
    }

