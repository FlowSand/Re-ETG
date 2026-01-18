using UnityEngine;

using Dungeonator;

#nullable disable

public class ProjectileTrapController : BasicTrapController
    {
        public ProjectileModule projectileModule;
        public ProjectileData overrideProjectileData;
        public DungeonData.Direction shootDirection;
        public VFXPool shootVfx;
        public Transform shootPoint;

        public override void Start()
        {
            base.Start();
            StaticReferenceManager.AllProjectileTraps.Add(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            StaticReferenceManager.AllProjectileTraps.Remove(this);
        }

        public override GameObject InstantiateObject(
            RoomHandler targetRoom,
            IntVector2 loc,
            bool deferConfiguration)
        {
            return base.InstantiateObject(targetRoom, loc, deferConfiguration);
        }

        protected override void TriggerTrap(SpeculativeRigidbody target)
        {
            base.TriggerTrap(target);
            if (this.projectileModule.shootStyle == ProjectileModule.ShootStyle.Beam)
            {
                Debug.LogWarning((object) "Unsupported shootstyle Beam.");
            }
            else
            {
                Vector2 vector2 = DungeonData.GetIntVector2FromDirection(this.shootDirection).ToVector2();
                this.ShootProjectileInDirection(this.shootPoint.position, vector2);
                this.shootVfx.SpawnAtLocalPosition(Vector3.zero, vector2.ToAngle(), this.shootPoint);
            }
        }

        private void ShootProjectileInDirection(Vector3 spawnPosition, Vector2 direction)
        {
            int num = (int) AkSoundEngine.PostEvent("Play_TRP_bullet_shot_01", this.gameObject);
            float z = Mathf.Atan2(direction.y, direction.x) * 57.29578f;
            Projectile component = SpawnManager.SpawnProjectile(this.projectileModule.GetCurrentProjectile().gameObject, spawnPosition, Quaternion.Euler(0.0f, 0.0f, z)).GetComponent<Projectile>();
            if (this.overrideProjectileData != null)
                component.baseData.SetAll(this.overrideProjectileData);
            component.Shooter = this.specRigidbody;
            component.TrapOwner = this;
        }
    }

