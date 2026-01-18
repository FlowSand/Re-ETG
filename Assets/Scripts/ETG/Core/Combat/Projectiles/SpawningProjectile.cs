using UnityEngine;

using Dungeonator;

#nullable disable

public class SpawningProjectile : Projectile
    {
        public float startingHeight = 1f;
        public float gravity = -10f;
        [EnemyIdentifier]
        public string enemyGuid;
        private Vector3 m_current3DVelocity;
        private float m_kinematicTimer;

        public override void Start()
        {
            base.Start();
            this.m_current3DVelocity = (this.m_currentDirection * this.m_currentSpeed).ToVector3ZUp();
        }

        protected override void Move()
        {
            this.m_kinematicTimer += BraveTime.DeltaTime;
            this.m_current3DVelocity.x = this.m_currentDirection.x;
            this.m_current3DVelocity.y = this.m_currentDirection.y;
            this.m_current3DVelocity.z = this.gravity * this.m_kinematicTimer;
            if ((double) (this.startingHeight + 0.5f * this.gravity * this.m_kinematicTimer * this.m_kinematicTimer) < 0.0)
            {
                Vector2 unitCenter = this.specRigidbody.UnitCenter;
                IntVector2 intVector2 = unitCenter.ToIntVector2(VectorConversions.Floor);
                RoomHandler roomFromPosition = GameManager.Instance.Dungeon.GetRoomFromPosition(intVector2);
                AIActor aiActor = AIActor.Spawn(EnemyDatabase.GetOrLoadByGuid(this.enemyGuid), intVector2, roomFromPosition, true);
                if ((bool) (Object) aiActor && (bool) (Object) aiActor.aiAnimator)
                {
                    aiActor.aiAnimator.PlayDefaultSpawnState();
                    this.hitEffects.HandleEnemyImpact((Vector3) unitCenter, 0.0f, (Transform) null, Vector2.zero, Vector2.zero, true);
                }
                if (this.IsBlackBullet && (bool) (Object) aiActor)
                    aiActor.ForceBlackPhantom = true;
                Object.Destroy((Object) this.gameObject);
            }
            else
            {
                this.m_currentDirection = this.m_current3DVelocity.XY();
                Vector2 vector2 = this.m_current3DVelocity.XY().normalized * this.m_currentSpeed;
                this.specRigidbody.Velocity = new Vector2(vector2.x, vector2.y + this.m_current3DVelocity.z);
                this.LastVelocity = this.m_current3DVelocity.XY();
            }
        }

        protected override void OnDestroy() => base.OnDestroy();
    }

