using System.Collections.Generic;

using UnityEngine;

#nullable disable

public class VolleyReplacementSynergyProcessor : MonoBehaviour
    {
        [LongNumericEnum]
        public CustomSynergyType RequiredSynergy;
        public ProjectileVolleyData SynergyVolley;
        private ProjectileVolleyData m_cachedSourceVolley;
        private ProjectileModule m_cachedSingleModule;
        private Gun m_gun;
        private bool m_transformed;

        private void Awake()
        {
            this.m_gun = this.GetComponent<Gun>();
            if ((Object) this.m_gun.RawSourceVolley != (Object) null)
                this.m_cachedSourceVolley = this.m_gun.RawSourceVolley;
            else
                this.m_cachedSingleModule = this.m_gun.singleModule;
        }

        private void Update()
        {
            PlayerController currentOwner = this.m_gun.CurrentOwner as PlayerController;
            if (!this.m_transformed && (bool) (Object) currentOwner && currentOwner.HasActiveBonusSynergy(this.RequiredSynergy))
            {
                this.m_transformed = true;
                ProjectileVolleyData volley = this.m_gun.Volley;
                this.m_gun.RawSourceVolley = !(bool) (Object) volley ? this.SynergyVolley : DuctTapeItem.TransferDuctTapeModules(volley, this.SynergyVolley, this.m_gun);
                currentOwner.stats.RecalculateStats(currentOwner);
            }
            else
            {
                if (!this.m_transformed || (bool) (Object) currentOwner && currentOwner.HasActiveBonusSynergy(this.RequiredSynergy))
                    return;
                this.m_transformed = false;
                ProjectileVolleyData volley = this.m_gun.Volley;
                if ((bool) (Object) volley)
                {
                    ProjectileVolleyData instance = ScriptableObject.CreateInstance<ProjectileVolleyData>();
                    if ((Object) this.m_cachedSourceVolley != (Object) null)
                    {
                        instance.InitializeFrom(this.m_cachedSourceVolley);
                    }
                    else
                    {
                        instance.projectiles = new List<ProjectileModule>();
                        instance.projectiles.Add(this.m_cachedSingleModule);
                    }
                    this.m_gun.RawSourceVolley = DuctTapeItem.TransferDuctTapeModules(volley, instance, this.m_gun);
                }
                else
                    this.m_gun.RawSourceVolley = !((Object) this.m_cachedSourceVolley != (Object) null) ? (ProjectileVolleyData) null : this.m_cachedSourceVolley;
                if (!(bool) (Object) currentOwner)
                    return;
                currentOwner.stats.RecalculateStats(currentOwner);
            }
        }
    }

