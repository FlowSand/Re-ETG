using UnityEngine;

#nullable disable

public class ShootPlayerProjectiles : MonoBehaviour
    {
        public ProjectileVolleyData Volley;
        public Transform ShootPoint;
        public float ShootCooldown = 1f;
        public ShootPlayerProjectiles.ArbitraryShootStyle style;
        public bool RequiresAnimation;
        private tk2dSpriteAnimator m_animator;
        private float m_cooldown;

        private void Start()
        {
            this.m_cooldown = Random.Range(0.0f, this.ShootCooldown);
            this.m_animator = this.GetComponent<tk2dSpriteAnimator>();
        }

        private void Update()
        {
            if (this.RequiresAnimation && !this.m_animator.IsPlaying(this.m_animator.CurrentClip))
            {
                Object.Destroy((Object) this);
            }
            else
            {
                this.m_cooldown -= BraveTime.DeltaTime;
                if ((double) this.m_cooldown > 0.0)
                    return;
                VolleyUtility.FireVolley(this.Volley, this.ShootPoint.position.XY(), Random.insideUnitCircle.normalized, (GameActor) GameManager.Instance.BestActivePlayer);
                this.m_cooldown += this.ShootCooldown;
            }
        }

        public enum ArbitraryShootStyle
        {
            RANDOM,
        }
    }

