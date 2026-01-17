// Decompiled with JetBrains decompiler
// Type: MimicGunMimicModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class MimicGunMimicModifier : MonoBehaviour
    {
      private Gun m_gun;
      private bool m_initialized;

      private void Start() => this.m_gun = this.GetComponent<Gun>();

      private void Update()
      {
        if (this.m_initialized || !((Object) this.m_gun.CurrentOwner != (Object) null))
          return;
        PlayerController currentOwner = this.m_gun.CurrentOwner as PlayerController;
        if (currentOwner.IsGunLocked)
        {
          Object.Destroy((Object) this);
        }
        else
        {
          currentOwner.inventory.AddGunToInventory(PickupObjectDatabase.GetById(GlobalItemIds.GunMimicID) as Gun, true).GetComponent<MimicGunController>().Initialize(currentOwner, this.m_gun);
          this.m_initialized = true;
          Object.Destroy((Object) this);
        }
      }
    }

}
