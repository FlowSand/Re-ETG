using System.Collections;
using System.Diagnostics;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bird/EggVomit1")]
public class BirdEggVomit1 : Script
    {
        private const int NumBullets = 36;

        protected override IEnumerator Top()
        {
            float num = BraveMathCollege.ClampAngle360(this.Direction);
            this.Fire(new Brave.BulletScript.Direction((double) num <= 90.0 || (double) num > 180.0 ? 20f : 160f), new Brave.BulletScript.Speed(2f), (Bullet) new BirdEggVomit1.EggBullet());
            return (IEnumerator) null;
        }

        public class EggBullet : Bullet
        {
            private bool spawnedBursts;

            public EggBullet()
                : base("egg")
            {
            }

            [DebuggerHidden]
            protected override IEnumerator Top()
            {
                // ISSUE: object of a compiler-generated type is created
                return (IEnumerator) new BirdEggVomit1.EggBullet__Topc__Iterator0()
                {
                    _this = this
                };
            }

            public override void OnBulletDestruction(
                Bullet.DestroyType destroyType,
                SpeculativeRigidbody hitRigidbody,
                bool preventSpawningProjectiles)
            {
                if (this.spawnedBursts || preventSpawningProjectiles)
                    return;
                this.SpawnBursts();
            }

            private void SpawnBursts()
            {
                float num1 = this.RandomAngle();
                float num2 = 10f;
                for (int index = 0; index < 36; ++index)
                    this.Fire(new Brave.BulletScript.Direction(num1 + (float) index * num2), new Brave.BulletScript.Speed(9f), (Bullet) new BirdEggVomit1.AcceleratingBullet());
                float num3 = num1 + num2 / 2f;
                for (int index = 0; index < 36; ++index)
                    this.Fire(new Brave.BulletScript.Direction(num3 + (float) index * num2), new Brave.BulletScript.Speed(5f), (Bullet) new BirdEggVomit1.AcceleratingBullet());
                float num4 = num3 + num2 / 2f;
                for (int index = 0; index < 36; ++index)
                    this.Fire(new Brave.BulletScript.Direction(num4 + (float) index * num2), new Brave.BulletScript.Speed(1f), (Bullet) new BirdEggVomit1.AcceleratingBullet());
                this.spawnedBursts = true;
            }
        }

        public class AcceleratingBullet : Bullet
        {
            public AcceleratingBullet()
                : base()
            {
            }

            protected override IEnumerator Top()
            {
                this.ChangeSpeed(new Brave.BulletScript.Speed(9f), 180);
                return (IEnumerator) null;
            }
        }
    }

