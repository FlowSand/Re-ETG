using UnityEngine;

#nullable disable

public class SupplyDropItem : PlayerItem
  {
    public GenericLootTable itemTableToUse;
    public CustomSynergyType improvementSynergy;
    public GenericLootTable synergyItemTableToUse01;
    public GenericLootTable synergyItemTableToUse02;
    public bool IsAmmoDrop;

    public override bool CanBeUsed(PlayerController user)
    {
      if (this.IsAmmoDrop)
      {
        if (user.HasActiveBonusSynergy(this.improvementSynergy))
          return true;
        if ((Object) user.CurrentGun == (Object) null || user.CurrentGun.InfiniteAmmo || !user.CurrentGun.CanGainAmmo || user.CurrentGun.CurrentAmmo == user.CurrentGun.AdjustedMaxAmmo)
          return false;
      }
      return (user.CurrentRoom == null || !user.InExitCell && (!user.CurrentRoom.area.IsProceduralRoom || user.CurrentRoom.area.proceduralCells == null)) && base.CanBeUsed(user);
    }

    protected override void DoEffect(PlayerController user)
    {
      IntVector2 key1 = user.SpawnEmergencyCrate(this.itemTableToUse);
      if (user.HasActiveBonusSynergy(this.improvementSynergy))
      {
        GameManager.Instance.Dungeon.data[key1].PreventRewardSpawn = true;
        IntVector2 key2 = user.SpawnEmergencyCrate(this.synergyItemTableToUse01);
        GameManager.Instance.Dungeon.data[key2].PreventRewardSpawn = true;
        user.SpawnEmergencyCrate(this.synergyItemTableToUse02);
        GameManager.Instance.Dungeon.data[key1].PreventRewardSpawn = false;
        GameManager.Instance.Dungeon.data[key2].PreventRewardSpawn = false;
      }
      int num = (int) AkSoundEngine.PostEvent("Play_OBJ_supplydrop_activate_01", this.gameObject);
    }

    protected override void OnDestroy() => base.OnDestroy();
  }

