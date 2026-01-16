// Decompiled with JetBrains decompiler
// Type: MiserlyProtectionItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
public class MiserlyProtectionItem : BasicStatPickup
{
  public override void Pickup(PlayerController player)
  {
    if (this.m_pickedUp)
      return;
    base.Pickup(player);
    player.OnItemPurchased += new Action<PlayerController, ShopItemController>(this.OnItemPurchased);
  }

  public void Break()
  {
    this.m_pickedUp = true;
    UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject, 1f);
  }

  private void OnItemPurchased(PlayerController player, ShopItemController obj)
  {
    if ((UnityEngine.Object) obj != (UnityEngine.Object) null && obj.item is MiserlyProtectionItem || player.HasActiveBonusSynergy(CustomSynergyType.MISERLY_PIGTECTION))
      return;
    player.DropPassiveItem((PassiveItem) this);
  }

  public override DebrisObject Drop(PlayerController player)
  {
    DebrisObject debrisObject = base.Drop(player);
    MiserlyProtectionItem component = debrisObject.GetComponent<MiserlyProtectionItem>();
    player.OnItemPurchased -= new Action<PlayerController, ShopItemController>(this.OnItemPurchased);
    component.m_pickedUpThisRun = true;
    if (!player.HasActiveBonusSynergy(CustomSynergyType.MISERLY_PIGTECTION))
      component.Break();
    return debrisObject;
  }

  protected override void OnDestroy() => base.OnDestroy();
}
