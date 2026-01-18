using System.Collections;
using System.Diagnostics;

using Brave.BulletScript;

#nullable disable

public class PedestalMimicThrowStuff1 : Script
    {
        private static readonly string[] BulletNames = new string[3]
        {
            "boot",
            "gun",
            "sponge"
        };
        private const float HomingSpeed = 12f;

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new PedestalMimicThrowStuff1__Topc__Iterator0()
            {
                _this = this
            };
        }

        public class AcceleratingBullet : Bullet
        {
            public AcceleratingBullet()
                : base("default")
            {
            }

            [DebuggerHidden]
            protected override IEnumerator Top()
            {
                // ISSUE: object of a compiler-generated type is created
                return (IEnumerator) new PedestalMimicThrowStuff1.AcceleratingBullet__Topc__Iterator0()
                {
                    _this = this
                };
            }
        }

        public class HomingShot : Bullet
        {
            public HomingShot(string bulletName) : base(bulletName)
            {
            }

            [DebuggerHidden]
            protected override IEnumerator Top()
            {
                // ISSUE: object of a compiler-generated type is created
                return (IEnumerator) new PedestalMimicThrowStuff1.HomingShot__Topc__Iterator0()
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
                for (int index = 0; index < 8; ++index)
                    this.Fire(new Brave.BulletScript.Direction((float) (index * 45)), new Brave.BulletScript.Speed(8f), (Bullet) new SpeedChangingBullet(10f, 120, 600));
            }
        }
    }

