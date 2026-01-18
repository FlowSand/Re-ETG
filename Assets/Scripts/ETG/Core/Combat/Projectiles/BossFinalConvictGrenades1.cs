using System.Collections;

using FullInspector;
using UnityEngine;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/BossFinalConvict/Grenades1")]
    public abstract class BossFinalConvictGrenades1 : Script
    {
        private const int NumBullets = 5;
        private float? m_playerDist;
        private readonly float m_minAngle;
        private readonly float m_maxAngle;

        public BossFinalConvictGrenades1(float minAngle, float maxAngle)
        {
            this.m_minAngle = minAngle;
            this.m_maxAngle = maxAngle;
        }

        protected override IEnumerator Top()
        {
            int i = 2;
            this.FireGrenade(i);
            for (int index = 1; index <= 2; ++index)
            {
                this.FireGrenade(i + index);
                this.FireGrenade(i - index);
            }
            return (IEnumerator) null;
        }

        private void FireGrenade(int i)
        {
            float num = Mathf.Lerp(this.m_minAngle, this.m_maxAngle, (float) i / 4f);
            Bullet bullet = new Bullet("grenade");
            this.Fire(new Brave.BulletScript.Direction(num), new Brave.BulletScript.Speed(1f), bullet);
            ArcProjectile projectile = bullet.Projectile as ArcProjectile;
            if (!this.m_playerDist.HasValue)
            {
                float timeInFlight = projectile.GetTimeInFlight();
                this.m_playerDist = new float?(Vector2.Distance(this.Position, this.BulletManager.PlayerPosition() + this.BulletManager.PlayerVelocity() * timeInFlight));
            }
            projectile.AdjustSpeedToHit(this.Position + BraveMathCollege.DegreesToVector(num, this.m_playerDist.Value));
        }
    }

