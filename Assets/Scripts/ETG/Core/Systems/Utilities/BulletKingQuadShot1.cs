using System.Collections;
using System.Diagnostics;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/BulletKing/QuadShot1")]
public class BulletKingQuadShot1 : Script
    {
        public bool IsHard => this is BulletKingQuadShotHard1;

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new BulletKingQuadShot1__Topc__Iterator0()
            {
                _this = this
            };
        }

        private void QuadShot(float x, float y, float direction)
        {
            for (int index = 0; index < 4; ++index)
                this.Fire(new Offset(x, y, transform: string.Empty), new Brave.BulletScript.Direction(direction - 90f), new Brave.BulletScript.Speed((float) (9.0 - (double) index * 1.5)), (Bullet) new BulletKingQuadShot1.QuadBullet(this.IsHard, index));
        }

        public class QuadBullet : Bullet
        {
            private bool m_isHard;
            private int m_index;

            public QuadBullet(bool isHard, int index)
                : base("quad")
            {
                this.m_isHard = isHard;
                this.m_index = index;
            }

            [DebuggerHidden]
            protected override IEnumerator Top()
            {
                // ISSUE: object of a compiler-generated type is created
                return (IEnumerator) new BulletKingQuadShot1.QuadBullet__Topc__Iterator0()
                {
                    _this = this
                };
            }
        }
    }

