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

