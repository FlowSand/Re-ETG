using System.Collections;
using System.Diagnostics;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/BossFinalRogue/BarGauntlet1")]
public class BossFinalRogueBarGauntlet1 : Script
    {
        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new BossFinalRogueBarGauntlet1__Topc__Iterator0()
            {
                _this = this
            };
        }

        private void Fire(int gunNum, float lineWidth, int numBullets)
        {
            this.Fire(gunNum, gunNum, lineWidth, numBullets);
        }

        private void Fire(int shootPointNum, int gunNum, float bulletSeparation, int numBullets)
        {
            this.Fire(new Offset("bar gun " + (object) shootPointNum), new Brave.BulletScript.Direction(-90f), new Brave.BulletScript.Speed(8f), (Bullet) new BossFinalRogueBarGauntlet1.BarBullet(gunNum, this.Position.x, bulletSeparation, numBullets));
        }

        public class BarBullet : Bullet
        {
            private int m_gunNum;
            private float m_centerX;
            private float m_lineWidth;
            private int m_numBullets;

            public BarBullet(int gunNum, float centerX, float lineWidth, int numBullets)
                : base("bar")
            {
                this.m_gunNum = gunNum;
                this.m_centerX = centerX;
                this.m_lineWidth = lineWidth;
                this.m_numBullets = numBullets;
            }

            [DebuggerHidden]
            protected override IEnumerator Top()
            {
                // ISSUE: object of a compiler-generated type is created
                return (IEnumerator) new BossFinalRogueBarGauntlet1.BarBullet__Topc__Iterator0()
                {
                    _this = this
                };
            }
        }
    }

