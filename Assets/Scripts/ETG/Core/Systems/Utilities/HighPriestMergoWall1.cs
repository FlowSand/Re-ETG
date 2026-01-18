using System.Collections;
using System.Diagnostics;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/HighPriest/MergoWall1")]
public class HighPriestMergoWall1 : Script
    {
        private const int NumBullets = 20;

        protected override IEnumerator Top()
        {
            for (int index = 0; index < 20; ++index)
                this.Fire(new Brave.BulletScript.Direction(type: Brave.BulletScript.DirectionType.Relative), new Brave.BulletScript.Speed(4f), (Bullet) new HighPriestMergoWall1.BigBullet());
            return (IEnumerator) null;
        }

        public class BigBullet : Bullet
        {
            public BigBullet()
                : base("mergoWall")
            {
            }

            [DebuggerHidden]
            protected override IEnumerator Top()
            {
                // ISSUE: object of a compiler-generated type is created
                return (IEnumerator) new HighPriestMergoWall1.BigBullet__Topc__Iterator0()
                {
                    _this = this
                };
            }
        }
    }

