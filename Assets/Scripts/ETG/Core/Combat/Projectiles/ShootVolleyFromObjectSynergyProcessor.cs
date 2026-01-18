using UnityEngine;

#nullable disable

public class ShootVolleyFromObjectSynergyProcessor : MonoBehaviour
    {
        [LongNumericEnum]
        public CustomSynergyType RequiredSynergy;
        public ShootVolleyFromObjectSynergyProcessor.TriggerType trigger;
        public bool usePlayerAim;
        public ProjectileModule singleModule;
        public ProjectileVolleyData volley;
        public float cooldown = 3f;
        public float maxRange = 30f;
        public Transform optionalShootPoint;
        private float m_cooldown;
        private PlayerController m_player;

        private void Awake() => this.m_cooldown = this.cooldown;

        private void Start()
        {
            PlayerOrbital component = this.GetComponent<PlayerOrbital>();
            if ((bool) (Object) component)
                this.m_player = component.Owner;
            if ((bool) (Object) this.m_player)
                return;
            this.m_player = this.GetComponentInParent<PlayerController>();
        }

        private void Update()
        {
            this.m_cooldown -= BraveTime.DeltaTime;
            if ((double) this.m_cooldown > 0.0)
                return;
            bool flag1 = false;
            if (this.trigger == ShootVolleyFromObjectSynergyProcessor.TriggerType.CONTINUOUS)
            {
                this.m_cooldown = this.cooldown;
                flag1 = true;
            }
            else if (this.trigger == ShootVolleyFromObjectSynergyProcessor.TriggerType.ON_SHOOT)
                flag1 = (bool) (Object) this.m_player && this.m_player.IsFiring;
            if (!flag1)
                return;
            int count = -1;
            if (!PlayerController.AnyoneHasActiveBonusSynergy(this.RequiredSynergy, out count))
                return;
            if (this.trigger == ShootVolleyFromObjectSynergyProcessor.TriggerType.ON_SHOOT)
                this.m_cooldown = this.cooldown;
            Vector2 vector2_1 = !(bool) (Object) this.optionalShootPoint ? this.transform.position.XY() : this.optionalShootPoint.position.XY();
            bool flag2 = false;
            Vector2 vector2_2 = Vector2.up;
            if (this.usePlayerAim)
            {
                flag2 = true;
                Vector2 vector2_3 = this.m_player.unadjustedAimPoint.XY();
                if (!BraveInput.GetInstanceForPlayer(this.m_player.PlayerIDX).IsKeyboardAndMouse() && (bool) (Object) this.m_player.CurrentGun)
                    vector2_3 = this.m_player.CenterPosition + BraveMathCollege.DegreesToVector(this.m_player.CurrentGun.CurrentAngle, 10f);
                vector2_2 = vector2_3 - vector2_1;
            }
            else
            {
                float nearestDistance = -1f;
                AIActor nearestEnemy = vector2_1.GetAbsoluteRoom().GetNearestEnemy(vector2_1, out nearestDistance);
                if ((bool) (Object) nearestEnemy)
                {
                    vector2_2 = nearestEnemy.CenterPosition - vector2_1;
                    flag2 = (double) nearestDistance < (double) this.maxRange;
                }
            }
            if (!flag2)
                return;
            if ((bool) (Object) this.volley)
                VolleyUtility.FireVolley(this.volley, vector2_1, vector2_2, (GameActor) GameManager.Instance.BestActivePlayer);
            else
                VolleyUtility.ShootSingleProjectile(this.singleModule, (ProjectileVolleyData) null, vector2_1, vector2_2.ToAngle(), 0.0f, (GameActor) GameManager.Instance.BestActivePlayer);
        }

        public enum TriggerType
        {
            CONTINUOUS,
            ON_SHOOT,
        }
    }

