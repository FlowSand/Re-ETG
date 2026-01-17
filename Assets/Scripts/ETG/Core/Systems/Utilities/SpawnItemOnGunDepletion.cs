// Decompiled with JetBrains decompiler
// Type: SpawnItemOnGunDepletion
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class SpawnItemOnGunDepletion : MonoBehaviour
    {
      public bool IsSynergyContingent;
      public CustomSynergyType SynergyToCheck;
      public bool UsesSpecificItem;
      [PickupIdentifier]
      public int SpecificItemId;
      protected Gun m_gun;

      private void Start() => this.m_gun = this.GetComponent<Gun>();

      private void Update()
      {
        if (!this.enabled || !(bool) (Object) this.m_gun || this.m_gun.ammo > 0 || !(this.m_gun.CurrentOwner is PlayerController))
          return;
        PlayerController currentOwner = this.m_gun.CurrentOwner as PlayerController;
        if (this.IsSynergyContingent && !currentOwner.HasActiveBonusSynergy(this.SynergyToCheck))
          return;
        if (this.UsesSpecificItem)
          LootEngine.TryGivePrefabToPlayer(PickupObjectDatabase.GetById(this.SpecificItemId).gameObject, currentOwner);
        else if ((bool) (Object) currentOwner && currentOwner.CurrentRoom != null)
        {
          Chest chest = GameManager.Instance.RewardManager.SpawnTotallyRandomChest(currentOwner.CurrentRoom.GetBestRewardLocation(IntVector2.One * 3, RoomHandler.RewardLocationStyle.PlayerCenter));
          if ((bool) (Object) chest)
            chest.IsLocked = false;
        }
        currentOwner.inventory.RemoveGunFromInventory(this.m_gun);
        Object.Destroy((Object) this.m_gun.gameObject);
      }
    }

}
