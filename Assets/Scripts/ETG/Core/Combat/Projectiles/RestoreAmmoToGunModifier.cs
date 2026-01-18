using System;
using UnityEngine;

#nullable disable

public class RestoreAmmoToGunModifier : MonoBehaviour
  {
    public float ChanceToWork = 1f;
    public int AmmoToGain = 1;
    public bool RequiresSynergy;
    [LongNumericEnum]
    public CustomSynergyType RequiredSynergy;
    public string RegainAmmoAnimation;
    private Projectile m_projectile;

    private void Start()
    {
      this.m_projectile = this.GetComponent<Projectile>();
      this.m_projectile.OnHitEnemy += new Action<Projectile, SpeculativeRigidbody, bool>(this.HandleHitEnemy);
    }

    private void HandleHitEnemy(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
    {
      if (!(bool) (UnityEngine.Object) this.m_projectile.PossibleSourceGun || this.RequiresSynergy && !arg1.PossibleSourceGun.OwnerHasSynergy(this.RequiredSynergy) || (double) UnityEngine.Random.value >= (double) this.ChanceToWork)
        return;
      this.m_projectile.PossibleSourceGun.GainAmmo(this.AmmoToGain);
      if (string.IsNullOrEmpty(this.RegainAmmoAnimation))
        return;
      this.m_projectile.PossibleSourceGun.spriteAnimator.PlayForDuration(this.RegainAmmoAnimation, -1f, this.m_projectile.PossibleSourceGun.idleAnimation);
    }
  }

