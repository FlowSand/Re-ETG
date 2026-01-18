using System.Collections;
using System.Diagnostics;

using FullInspector;
using UnityEngine;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("MushroomGuy/GiantWaft1")]
public class MushroomGuyGiantWaft1 : Script
    {
        private const int NumWaftBullets = 50;
        private const int NumFastBullets = 25;
        private const float VerticalDriftVelocity = -0.5f;
        private const float WaftXPeriod = 3f;
        private const float WaftXMagnitude = 0.5f;
        private const float WaftYPeriod = 1f;
        private const float WaftYMagnitude = 0.125f;
        private const int WaftLifeTime = 300;

        protected override IEnumerator Top()
        {
            for (int index = 0; index < 50; ++index)
            {
                string bankName = (double) Random.value > 0.33000001311302185 ? "spore2" : "spore1";
                this.Fire(new Brave.BulletScript.Direction(this.RandomAngle()), new Brave.BulletScript.Speed((float) Random.Range(2, 14)), (Bullet) new MushroomGuyGiantWaft1.WaftBullet(bankName));
            }
            for (int index = 0; index < 25; ++index)
            {
                Bullet bullet = (Bullet) new SpeedChangingBullet((double) Random.value > 0.33000001311302185 ? "spore2" : "spore1", 10.2f, 75, 300);
                this.Fire(new Brave.BulletScript.Direction(this.RandomAngle()), new Brave.BulletScript.Speed((float) Random.Range(2, 16)), bullet);
                bullet.Projectile.spriteAnimator.Play();
            }
            return (IEnumerator) null;
        }

        public class WaftBullet : Bullet
        {
            public WaftBullet(string bankName) : base(bankName)
            {
            }

            [DebuggerHidden]
            protected override IEnumerator Top()
            {
                // ISSUE: object of a compiler-generated type is created
                return (IEnumerator) new MushroomGuyGiantWaft1.WaftBullet__Topc__Iterator0()
                {
                    _this = this
                };
            }
        }
    }

