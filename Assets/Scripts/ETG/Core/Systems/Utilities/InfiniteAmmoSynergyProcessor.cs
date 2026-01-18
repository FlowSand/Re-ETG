using UnityEngine;

#nullable disable

public class InfiniteAmmoSynergyProcessor : MonoBehaviour
  {
    [LongNumericEnum]
    public CustomSynergyType RequiredSynergy;
    public bool PreventsReload = true;
    private bool m_processed;
    private Gun m_gun;
    private float m_cachedReloadTime = -1f;

    public void Awake() => this.m_gun = this.GetComponent<Gun>();

    public void Update()
    {
      bool flag = (bool) (Object) this.m_gun && this.m_gun.OwnerHasSynergy(this.RequiredSynergy);
      if (flag && !this.m_processed)
      {
        this.m_gun.GainAmmo(this.m_gun.AdjustedMaxAmmo);
        this.m_gun.InfiniteAmmo = true;
        this.m_processed = true;
        if (!this.PreventsReload)
          return;
        this.m_cachedReloadTime = this.m_gun.reloadTime;
        this.m_gun.reloadTime = 0.0f;
      }
      else
      {
        if (flag || !this.m_processed)
          return;
        this.m_gun.InfiniteAmmo = false;
        this.m_processed = false;
        if (!this.PreventsReload)
          return;
        this.m_gun.reloadTime = this.m_cachedReloadTime;
      }
    }
  }

