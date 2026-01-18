// Decompiled with JetBrains decompiler
// Type: ReloadSwitchSynergyProcessor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

public class ReloadSwitchSynergyProcessor : MonoBehaviour
  {
    [PickupIdentifier]
    public int PartnerGunID;
    [LongNumericEnum]
    public CustomSynergyType RequiredSynergy;
    public bool ReloadsTargetGun = true;
    private Gun m_gun;

    public void Awake()
    {
      this.m_gun = this.GetComponent<Gun>();
      this.m_gun.OnPostFired += new Action<PlayerController, Gun>(this.HandlePostFired);
    }

    private void HandlePostFired(PlayerController sourcePlayer, Gun sourceGun)
    {
      if (!sourcePlayer.HasActiveBonusSynergy(this.RequiredSynergy) || this.m_gun.ClipShotsRemaining != 0)
        return;
      for (int index = 0; index < sourcePlayer.inventory.AllGuns.Count; ++index)
      {
        if (sourcePlayer.inventory.AllGuns[index].PickupObjectId == this.PartnerGunID && sourcePlayer.inventory.AllGuns[index].ammo > 0)
        {
          sourcePlayer.inventory.GunChangeForgiveness = true;
          sourcePlayer.ChangeToGunSlot(index);
          sourcePlayer.inventory.AllGuns[index].ForceImmediateReload(true);
          sourcePlayer.inventory.GunChangeForgiveness = false;
          break;
        }
      }
    }
  }

