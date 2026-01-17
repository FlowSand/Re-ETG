// Decompiled with JetBrains decompiler
// Type: ActiveAmmunitionData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

namespace ETG.Core.Combat.Projectiles
{
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

}
