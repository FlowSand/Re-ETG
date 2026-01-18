using System;
using System.Collections.Generic;

using UnityEngine;

#nullable disable

public class GunTierController : MonoBehaviour, IGunInheritable
    {
        public int[] TierThresholds;
        public int TiersToDropOnDamage = 100;
        public int MaxTier = 3;
        public GameObject[] TierVFX;
        private Gun m_gun;
        private PlayerController m_playerOwner;
        private int m_kills;

        private int KillsToNextTier
        {
            get
            {
                int currentStrengthTier = this.m_gun.CurrentStrengthTier;
                return currentStrengthTier >= this.TierThresholds.Length ? int.MaxValue : this.TierThresholds[currentStrengthTier];
            }
        }

        private void Awake()
        {
            this.m_gun = this.GetComponent<Gun>();
            this.m_gun.OnInitializedWithOwner += new Action<GameActor>(this.OnGunInitialized);
            this.m_gun.OnDropped += new System.Action(this.OnGunDroppedOrDestroyed);
            if (!((UnityEngine.Object) this.m_gun.CurrentOwner != (UnityEngine.Object) null))
                return;
            this.OnGunInitialized(this.m_gun.CurrentOwner);
        }

        private void OnGunInitialized(GameActor obj)
        {
            if ((UnityEngine.Object) this.m_playerOwner != (UnityEngine.Object) null)
                this.OnGunDroppedOrDestroyed();
            if ((UnityEngine.Object) obj == (UnityEngine.Object) null || !(obj is PlayerController))
                return;
            this.m_playerOwner = obj as PlayerController;
            this.m_playerOwner.OnKilledEnemy += new Action<PlayerController>(this.OnEnemyKilled);
            this.m_playerOwner.healthHaver.OnDamaged += new HealthHaver.OnDamagedEvent(this.OnPlayerDamaged);
        }

        private void PlayTierVFX(PlayerController p)
        {
            if (this.m_gun.CurrentStrengthTier < 0 || this.m_gun.CurrentStrengthTier >= this.TierVFX.Length)
                return;
            p.PlayEffectOnActor(this.TierVFX[this.m_gun.CurrentStrengthTier], Vector3.up, alreadyMiddleCenter: true);
        }

        private void OnEnemyKilled(PlayerController obj)
        {
            if (!((UnityEngine.Object) obj.CurrentGun == (UnityEngine.Object) this.m_gun))
                return;
            ++this.m_kills;
            if (this.m_kills <= this.KillsToNextTier)
                return;
            this.m_gun.CurrentStrengthTier = Mathf.Clamp(this.m_gun.CurrentStrengthTier + 1, 0, this.MaxTier - 1);
            this.PlayTierVFX(obj);
        }

        private void OnDestroy() => this.OnGunDroppedOrDestroyed();

        private void OnGunDroppedOrDestroyed()
        {
            if (!((UnityEngine.Object) this.m_playerOwner != (UnityEngine.Object) null))
                return;
            this.m_playerOwner.healthHaver.OnDamaged -= new HealthHaver.OnDamagedEvent(this.OnPlayerDamaged);
            this.m_playerOwner.OnKilledEnemy -= new Action<PlayerController>(this.OnEnemyKilled);
            this.m_playerOwner = (PlayerController) null;
        }

        private void OnPlayerDamaged(
            float resultValue,
            float maxValue,
            CoreDamageTypes damageTypes,
            DamageCategory damageCategory,
            Vector2 damageDirection)
        {
            int num = Mathf.Clamp(this.m_gun.CurrentStrengthTier - this.TiersToDropOnDamage, 0, this.MaxTier - 1);
            if (this.m_gun.CurrentStrengthTier != num)
            {
                this.m_gun.CurrentStrengthTier = num;
                this.PlayTierVFX(this.m_gun.CurrentOwner as PlayerController);
            }
            if (this.m_gun.CurrentStrengthTier == 0)
                this.m_kills = 0;
            else
                this.m_kills = this.TierThresholds[this.m_gun.CurrentStrengthTier - 1];
        }

        public void InheritData(Gun sourceGun)
        {
            GunTierController component = sourceGun.GetComponent<GunTierController>();
            if (!(bool) (UnityEngine.Object) component)
                return;
            this.m_kills = component.m_kills;
        }

        public void MidGameSerialize(List<object> data, int dataIndex) => data.Add((object) this.m_kills);

        public void MidGameDeserialize(List<object> data, ref int dataIndex)
        {
            this.m_kills = (int) data[dataIndex];
            if ((UnityEngine.Object) this.m_gun == (UnityEngine.Object) null)
                this.m_gun = this.GetComponent<Gun>();
            while (this.m_kills > this.KillsToNextTier)
                this.m_gun.CurrentStrengthTier = Mathf.Clamp(this.m_gun.CurrentStrengthTier + 1, 0, this.MaxTier - 1);
            ++dataIndex;
        }
    }

