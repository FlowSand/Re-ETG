// Decompiled with JetBrains decompiler
// Type: GunRechargeSynergyProcessor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
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

}
