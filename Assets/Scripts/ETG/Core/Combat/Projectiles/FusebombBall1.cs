using System.Collections;

using FullInspector;
using UnityEngine;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/Fusebomb/Ball1")]
public class FusebombBall1 : Script
    {
        protected override IEnumerator Top()
        {
            Random.Range(-30f, 30f);
            this.Fire(new Brave.BulletScript.Direction(), new Brave.BulletScript.Speed(3f), (Bullet) new FusebombBall1.RollyBall());
            return (IEnumerator) null;
        }

        private class RollyBall : Bullet
        {
            private const float TargetSpeed = 12f;

            public RollyBall()
                : base("ball")
            {
            }

            protected override IEnumerator Top()
            {
                float direction = (float) -Random.Range(20, 55);
                this.ChangeSpeed(new Brave.BulletScript.Speed(12f), 60);
                this.ChangeDirection(new Brave.BulletScript.Direction(direction), 60);
                return (IEnumerator) null;
            }

            public override void OnForceRemoved()
            {
                this.Speed = 12f;
                if (!(bool) (Object) this.Projectile || !(bool) (Object) this.Projectile.specRigidbody || !(this.Projectile.specRigidbody.Velocity != Vector2.zero))
                    return;
                this.Projectile.specRigidbody.Velocity = this.Projectile.specRigidbody.Velocity.normalized * 12f;
            }
        }
    }

