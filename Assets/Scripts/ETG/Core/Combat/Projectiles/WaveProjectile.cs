using UnityEngine;

#nullable disable

public class WaveProjectile : Projectile
    {
        public float amplitude = 1f;
        public float frequency = 2f;

        protected override void Move()
        {
            this.m_timeElapsed += this.LocalDeltaTime;
            this.specRigidbody.Velocity = (Vector2) (this.transform.right * this.baseData.speed + this.transform.up * ((float) ((!this.Inverted ? 1.0 : -1.0) * (double) this.amplitude * 2.0 * 3.1415927410125732) * this.frequency * Mathf.Cos((float) ((double) this.m_timeElapsed * 2.0 * 3.1415927410125732) * this.frequency)));
        }

        protected override void OnDestroy() => base.OnDestroy();
    }

