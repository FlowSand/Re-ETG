// Decompiled with JetBrains decompiler
// Type: CathedralCrestItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

public class CathedralCrestItem : PassiveItem
  {
    public override void Pickup(PlayerController player)
    {
      if (this.m_pickedUp)
        return;
      base.Pickup(player);
      player.healthHaver.HasCrest = true;
      player.OnReceivedDamage += new Action<PlayerController>(this.PlayerDamaged);
      ++player.healthHaver.Armor;
    }

    private void PlayerDamaged(PlayerController obj)
    {
      obj.healthHaver.HasCrest = false;
      obj.RemovePassiveItem(this.PickupObjectId);
    }

    public override DebrisObject Drop(PlayerController player)
    {
      DebrisObject debrisObject = base.Drop(player);
      player.healthHaver.HasCrest = false;
      if ((bool) (UnityEngine.Object) debrisObject)
      {
        CathedralCrestItem component = debrisObject.GetComponent<CathedralCrestItem>();
        if ((bool) (UnityEngine.Object) component)
          component.m_pickedUpThisRun = true;
      }
      player.OnReceivedDamage -= new Action<PlayerController>(this.PlayerDamaged);
      return debrisObject;
    }

    protected override void OnDestroy()
    {
      if (this.m_pickedUp && GameManager.HasInstance && (bool) (UnityEngine.Object) this.Owner)
      {
        this.Owner.healthHaver.HasCrest = false;
        this.Owner.OnReceivedDamage -= new Action<PlayerController>(this.PlayerDamaged);
      }
      base.OnDestroy();
    }
  }

