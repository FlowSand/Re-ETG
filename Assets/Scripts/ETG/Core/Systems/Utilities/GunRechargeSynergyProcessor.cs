using System;
using UnityEngine;

#nullable disable

public class GunRechargeSynergyProcessor : MonoBehaviour
  {
    public CustomSynergyType SynergyToCheck;
    public float CDR_Multiplier = 1f;
    protected Gun m_gun;

    private void Awake()
    {
      this.m_gun = this.GetComponent<Gun>();
      if (!(bool) (UnityEngine.Object) this.m_gun)
        return;
      this.m_gun.ModifyActiveCooldownDamage += new Func<float, float>(this.HandleActiveCooldownModification);
    }

    private float HandleActiveCooldownModification(float inDamage)
    {
      return (bool) (UnityEngine.Object) this.m_gun && this.m_gun.CurrentOwner is PlayerController && (this.m_gun.CurrentOwner as PlayerController).HasActiveBonusSynergy(this.SynergyToCheck) ? inDamage * this.CDR_Multiplier : inDamage;
    }
  }

