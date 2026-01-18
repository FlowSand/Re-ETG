using UnityEngine;

#nullable disable

public class Flake : BraveBehaviour
    {
        public float lifetime;
        public Vector2 velocity;
        public Vector2 velocityVariance;
        private float m_timer;
        private Vector2 m_velocity;

        public void Start()
        {
            this.m_velocity = this.velocity;
            this.m_velocity.x += Random.Range(-this.velocityVariance.x, this.velocityVariance.x);
            this.m_velocity.y += Random.Range(-this.velocityVariance.y, this.velocityVariance.y);
        }

        public void Update()
        {
            this.m_timer += BraveTime.DeltaTime;
            this.transform.position += (Vector3) (this.m_velocity * BraveTime.DeltaTime);
            this.sprite.color = this.sprite.color with
            {
                a = Mathf.Min(1f, Mathf.Lerp(2f, 0.0f, this.m_timer / this.lifetime))
            };
            if ((double) this.m_timer <= (double) this.lifetime)
                return;
            Object.Destroy((Object) this.gameObject);
        }

        protected override void OnDestroy() => base.OnDestroy();
    }

