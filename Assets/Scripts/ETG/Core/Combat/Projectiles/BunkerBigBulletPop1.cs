using System.Collections;
using System.Diagnostics;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/Bunker/BigBulletPop1")]
public class BunkerBigBulletPop1 : Script
    {
        protected override IEnumerator Top()
        {
            this.Fire(new Offset("left shooter"), new Brave.BulletScript.Direction(type: Brave.BulletScript.DirectionType.Relative), new Brave.BulletScript.Speed(9f), (Bullet) new BunkerBigBulletPop1.BigBullet());
            return (IEnumerator) null;
        }

        public class BigBullet : Bullet
        {
            public BigBullet()
                : base("default_black")
            {
            }

            [DebuggerHidden]
            protected override IEnumerator Top()
            {
                // ISSUE: object of a compiler-generated type is created
                return (IEnumerator) new BunkerBigBulletPop1.BigBullet__Topc__Iterator0()
                {
                    _this = this
                };
            }

            public override void OnBulletDestruction(
                Bullet.DestroyType destroyType,
                SpeculativeRigidbody hitRigidbody,
                bool preventSpawningProjectiles)
            {
                if (preventSpawningProjectiles)
                    return;
                float num = this.RandomAngle();
                for (int index = 0; index < 8; ++index)
                    this.Fire(new Brave.BulletScript.Direction(num + (float) (index * 45)), new Brave.BulletScript.Speed(9f), new Bullet("default3"));
            }
        }
    }

