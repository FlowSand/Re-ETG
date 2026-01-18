using System.Collections;
using System.Diagnostics;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/BulletKing/BigBulletUp1")]
public class BulletKingBigBulletUp1 : Script
    {
        private const int NumMediumBullets = 8;
        private const int NumSmallBullets = 8;

        protected bool IsHard => this is BulletKingBigBulletUpHard1;

        protected override IEnumerator Top()
        {
            this.Fire(new Offset(1f / 16f, 57f / 16f, transform: string.Empty), new Brave.BulletScript.Direction(90f), new Brave.BulletScript.Speed(3f), (Bullet) new BulletKingBigBulletUp1.BigBullet(this.IsHard));
            return (IEnumerator) null;
        }

        public class BigBullet : Bullet
        {
            private bool m_isHard;

            public BigBullet(bool isHard)
                : base("bigBullet")
            {
                this.m_isHard = isHard;
            }

            [DebuggerHidden]
            protected override IEnumerator Top()
            {
                // ISSUE: object of a compiler-generated type is created
                return (IEnumerator) new BulletKingBigBulletUp1.BigBullet__Topc__Iterator0()
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
                float startAngle = this.RandomAngle();
                for (int i = 0; i < 8; ++i)
                    this.Fire(new Brave.BulletScript.Direction(!this.m_isHard ? this.SubdivideCircle(startAngle, 8, i) : this.SubdivideArc(this.AimDirection - 120f, 240f, 8, i)), new Brave.BulletScript.Speed(6f), (Bullet) new BulletKingBigBulletUp1.MediumBullet());
            }
        }

        public class MediumBullet : Bullet
        {
            public MediumBullet()
                : base("quad")
            {
            }

            [DebuggerHidden]
            protected override IEnumerator Top()
            {
                // ISSUE: object of a compiler-generated type is created
                return (IEnumerator) new BulletKingBigBulletUp1.MediumBullet__Topc__Iterator0()
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
                float num1 = this.RandomAngle();
                float num2 = 45f;
                for (int index = 0; index < 8; ++index)
                    this.Fire(new Brave.BulletScript.Direction(num1 + (float) index * num2), new Brave.BulletScript.Speed(10f), new Bullet("default_novfx"));
            }
        }
    }

