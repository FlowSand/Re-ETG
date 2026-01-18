using System;

using UnityEngine;

#nullable disable

public class AuraOnReloadModifier : MonoBehaviour
    {
        public float AuraRadius = 5f;
        public CoreDamageTypes damageTypes;
        public float DamagePerSecond = 5f;
        public bool IgnitesEnemies;
        public GameActorFireEffect IgniteEffect;
        public bool DoRadialIndicatorAnyway;
        public bool HasRadiusSynergy;
        [LongNumericEnum]
        public CustomSynergyType RadiusSynergy;
        public float RadiusSynergyRadius = 10f;
        private Gun m_gun;
        private Action<AIActor, float> AuraAction;
        private bool m_radialIndicatorActive;
        private HeatIndicatorController m_radialIndicator;

        private void Awake()
        {
            this.m_gun = this.GetComponent<Gun>();
            this.m_gun.OnDropped += new System.Action(this.OnDropped);
            PlayerController currentOwner = this.m_gun.CurrentOwner as PlayerController;
            if (!(bool) (UnityEngine.Object) currentOwner)
                return;
            currentOwner.inventory.OnGunChanged += new GunInventory.OnGunChangedEvent(this.OnGunChanged);
        }

        private void Update()
        {
            if (this.m_gun.IsReloading && this.m_gun.CurrentOwner is PlayerController)
            {
                this.DoAura();
                if (!this.IgnitesEnemies && !this.DoRadialIndicatorAnyway)
                    return;
                this.HandleRadialIndicator();
            }
            else
                this.UnhandleRadialIndicator();
        }

        private void HandleRadialIndicator()
        {
            if (this.m_radialIndicatorActive)
                return;
            this.m_radialIndicatorActive = true;
            this.m_radialIndicator = ((GameObject) UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/HeatIndicator"), this.m_gun.CurrentOwner.CenterPosition.ToVector3ZisY(), Quaternion.identity, this.m_gun.CurrentOwner.transform)).GetComponent<HeatIndicatorController>();
            if (this.IgnitesEnemies)
                return;
            this.m_radialIndicator.CurrentColor = new Color(0.0f, 0.0f, 1f);
            this.m_radialIndicator.IsFire = false;
        }

        private void UnhandleRadialIndicator()
        {
            if (!this.m_radialIndicatorActive)
                return;
            this.m_radialIndicatorActive = false;
            if ((bool) (UnityEngine.Object) this.m_radialIndicator)
                this.m_radialIndicator.EndEffect();
            this.m_radialIndicator = (HeatIndicatorController) null;
        }

        protected virtual void DoAura()
        {
            bool didDamageEnemies = false;
            PlayerController currentOwner = this.m_gun.CurrentOwner as PlayerController;
            if (this.AuraAction == null)
                this.AuraAction = (Action<AIActor, float>) ((actor, dist) =>
                {
                    float damage = this.DamagePerSecond * BraveTime.DeltaTime;
                    if (this.IgnitesEnemies || (double) damage > 0.0)
                        didDamageEnemies = true;
                    if (this.IgnitesEnemies)
                        actor.ApplyEffect((GameActorEffect) this.IgniteEffect);
                    actor.healthHaver.ApplyDamage(damage, Vector2.zero, "Aura", this.damageTypes);
                });
            if ((UnityEngine.Object) currentOwner != (UnityEngine.Object) null && currentOwner.CurrentRoom != null)
            {
                float radius = this.AuraRadius;
                if (this.HasRadiusSynergy && currentOwner.HasActiveBonusSynergy(this.RadiusSynergy))
                    radius = this.RadiusSynergyRadius;
                if ((bool) (UnityEngine.Object) this.m_radialIndicator)
                    this.m_radialIndicator.CurrentRadius = radius;
                currentOwner.CurrentRoom.ApplyActionToNearbyEnemies(currentOwner.CenterPosition, radius, this.AuraAction);
            }
            if (!didDamageEnemies)
                return;
            currentOwner.DidUnstealthyAction();
        }

        private void OnDropped()
        {
            this.UnhandleRadialIndicator();
            PlayerController currentOwner = this.m_gun.CurrentOwner as PlayerController;
            if (!(bool) (UnityEngine.Object) currentOwner)
                return;
            currentOwner.inventory.OnGunChanged -= new GunInventory.OnGunChangedEvent(this.OnGunChanged);
        }

        private void OnGunChanged(
            Gun previous,
            Gun current,
            Gun previoussecondary,
            Gun currentsecondary,
            bool newgun)
        {
            if (!((UnityEngine.Object) current != (UnityEngine.Object) this) || !((UnityEngine.Object) currentsecondary != (UnityEngine.Object) this))
                return;
            this.UnhandleRadialIndicator();
        }

        private void OnDestroy()
        {
            this.m_gun.OnDropped -= new System.Action(this.OnDropped);
            PlayerController currentOwner = this.m_gun.CurrentOwner as PlayerController;
            if (!(bool) (UnityEngine.Object) currentOwner)
                return;
            currentOwner.inventory.OnGunChanged -= new GunInventory.OnGunChangedEvent(this.OnGunChanged);
        }
    }

