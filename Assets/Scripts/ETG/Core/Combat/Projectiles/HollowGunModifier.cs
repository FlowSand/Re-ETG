// Decompiled with JetBrains decompiler
// Type: HollowGunModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

namespace ETG.Core.Combat.Projectiles
{
    public class HollowGunModifier : MonoBehaviour
    {
      public float DamageMultiplier = 1.5f;
      private Gun m_gun;

      public void Awake()
      {
        this.m_gun = this.GetComponent<Gun>();
        this.m_gun.PostProcessProjectile += new Action<Projectile>(this.HandleProcessProjectile);
      }

      private void HandleProcessProjectile(Projectile obj)
      {
        if (!(this.m_gun.CurrentOwner is PlayerController) || !(this.m_gun.CurrentOwner as PlayerController).IsDarkSoulsHollow)
          return;
        obj.baseData.damage *= this.DamageMultiplier;
      }
    }

}
