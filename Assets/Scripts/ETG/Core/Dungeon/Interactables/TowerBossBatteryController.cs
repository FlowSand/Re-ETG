using System;

using UnityEngine;

#nullable disable

public class TowerBossBatteryController : DungeonPlaceableBehaviour
    {
        public TowerBossController tower;
        public TowerBossIrisController linkedIris;
        public float cycleTime = 5f;
        private tk2dSprite m_sprite;

        public bool IsVulnerable
        {
            get => this.healthHaver.IsVulnerable;
            set
            {
                if ((UnityEngine.Object) this.m_sprite == (UnityEngine.Object) null)
                    this.m_sprite = this.GetComponentInChildren<tk2dSprite>();
                this.healthHaver.IsVulnerable = value;
                if (value)
                    this.m_sprite.renderer.enabled = true;
                else
                    this.m_sprite.renderer.enabled = false;
            }
        }

        private void Start()
        {
            this.m_sprite = this.GetComponentInChildren<tk2dSprite>();
            this.healthHaver.IsVulnerable = false;
            this.healthHaver.persistsOnDeath = true;
            this.healthHaver.OnDamaged += new HealthHaver.OnDamagedEvent(this.Damaged);
            this.healthHaver.OnDeath += new Action<Vector2>(this.Die);
        }

        private void Damaged(
            float resultValue,
            float maxValue,
            CoreDamageTypes damageTypes,
            DamageCategory damageCategory,
            Vector2 damageDirection)
        {
        }

        private void Die(Vector2 finalDamageDirection)
        {
            this.linkedIris.Open();
            this.healthHaver.FullHeal();
            this.healthHaver.IsVulnerable = false;
        }

        protected override void OnDestroy() => base.OnDestroy();
    }

