// Decompiled with JetBrains decompiler
// Type: SpawnItemOnRoomClearItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

namespace ETG.Core.Items.Passive
{
    public class SpawnItemOnRoomClearItem : PassiveItem
    {
      public float chanceToSpawn = 0.05f;
      [PickupIdentifier]
      public int spawnItemId = -1;
      public bool requirePlayerDamaged;

      public override void Pickup(PlayerController player)
      {
        if (this.m_pickedUp)
          return;
        player.OnRoomClearEvent += new Action<PlayerController>(this.RoomCleared);
        base.Pickup(player);
      }

      private void RoomCleared(PlayerController obj)
      {
        float num = UnityEngine.Random.value;
        if (this.requirePlayerDamaged && (double) obj.healthHaver.GetCurrentHealthPercentage() >= 1.0 || obj.CurrentRoom.PlayerHasTakenDamageInThisRoom)
          return;
        if ((bool) (UnityEngine.Object) this.Owner && this.Owner.HasActiveBonusSynergy(CustomSynergyType.THE_COIN_KING) && this.itemName == "Crown of the Coin King")
          this.chanceToSpawn *= 2f;
        if ((double) num >= (double) this.chanceToSpawn)
          return;
        LootEngine.SpawnItem(PickupObjectDatabase.GetById(this.spawnItemId).gameObject, (Vector3) obj.specRigidbody.UnitCenter, Vector2.up, 1f, false, true);
      }

      public override DebrisObject Drop(PlayerController player)
      {
        DebrisObject debrisObject = base.Drop(player);
        player.OnRoomClearEvent -= new Action<PlayerController>(this.RoomCleared);
        debrisObject.GetComponent<SpawnItemOnRoomClearItem>().m_pickedUpThisRun = true;
        return debrisObject;
      }

      protected override void OnDestroy() => base.OnDestroy();
    }

}
