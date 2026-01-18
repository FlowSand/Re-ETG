using System;

#nullable disable

public class ActiveAmmunitionData
  {
    public int ShotsRemaining;
    public float DamageModifier = 1f;
    public float SpeedModifier = 1f;
    public float RangeModifier = 1f;
    public Action<Projectile, Gun> OnAmmoModification;

    public void HandleAmmunition(Projectile p, Gun g)
    {
      p.baseData.damage *= this.DamageModifier;
      p.baseData.speed *= this.SpeedModifier;
      p.baseData.range *= this.RangeModifier;
      if (this.OnAmmoModification == null)
        return;
      this.OnAmmoModification(p, g);
    }
  }

