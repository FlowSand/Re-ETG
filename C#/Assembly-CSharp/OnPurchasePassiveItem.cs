// Decompiled with JetBrains decompiler
// Type: OnPurchasePassiveItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
public class OnPurchasePassiveItem : PassiveItem
{
  public float ActivationChance = 0.5f;
  public bool DoesHeal = true;
  public float HealingAmount = 0.5f;

  public override void Pickup(PlayerController player)
  {
    if (this.m_pickedUp)
      return;
    base.Pickup(player);
    player.OnItemPurchased += new Action<PlayerController, ShopItemController>(this.OnItemPurchased);
  }

  private void OnItemPurchased(PlayerController player, ShopItemController obj)
  {
    if ((double) UnityEngine.Random.value >= (double) this.ActivationChance || !this.DoesHeal)
      return;
    player.healthHaver.ApplyHealing(this.HealingAmount);
  }

  public override DebrisObject Drop(PlayerController player)
  {
    DebrisObject debrisObject = base.Drop(player);
    OnPurchasePassiveItem component = debrisObject.GetComponent<OnPurchasePassiveItem>();
    player.OnItemPurchased -= new Action<PlayerController, ShopItemController>(this.OnItemPurchased);
    component.m_pickedUpThisRun = true;
    return debrisObject;
  }

  protected override void OnDestroy() => base.OnDestroy();
}
