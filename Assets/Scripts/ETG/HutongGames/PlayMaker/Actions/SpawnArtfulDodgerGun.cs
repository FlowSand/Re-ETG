// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SpawnArtfulDodgerGun
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Spawns the Artful Dodger gun in the world or gives it directly to the player.")]
  [ActionCategory(".NPCs")]
  public class SpawnArtfulDodgerGun : FsmStateAction
  {
    public SpawnArtfulDodgerGun.Mode mode;
    [HutongGames.PlayMaker.Tooltip("Item to spawn.")]
    public FsmInt pickupId;
    public FsmInt numberOfBouncesAllowed = (FsmInt) 3;
    public FsmInt numberOfShotsAllowed = (FsmInt) 3;
    [HutongGames.PlayMaker.Tooltip("Loot table to choose an item from.")]
    public GenericLootTable lootTable;
    [HutongGames.PlayMaker.Tooltip("Offset from the TalkDoer to spawn the item at.")]
    public Vector2 spawnOffset;

    public override void Reset()
    {
      this.mode = SpawnArtfulDodgerGun.Mode.SpecifyPickup;
      this.pickupId = (FsmInt) -1;
      this.numberOfBouncesAllowed = (FsmInt) 3;
      this.numberOfShotsAllowed = (FsmInt) 3;
      this.lootTable = (GenericLootTable) null;
      this.spawnOffset = Vector2.zero;
    }

    public override string ErrorCheck()
    {
      string empty = string.Empty;
      if (this.mode == SpawnArtfulDodgerGun.Mode.SpecifyPickup && (Object) PickupObjectDatabase.GetById(this.pickupId.Value) == (Object) null)
        empty += "Invalid item ID.\n";
      if (this.mode == SpawnArtfulDodgerGun.Mode.LootTable && !(bool) (Object) this.lootTable)
        empty += "Invalid loot table.\n";
      return empty;
    }

    public override void OnEnter()
    {
      TalkDoerLite component1 = this.Owner.GetComponent<TalkDoerLite>();
      GameObject gameObject = (GameObject) null;
      if (this.mode == SpawnArtfulDodgerGun.Mode.SpecifyPickup)
        gameObject = PickupObjectDatabase.GetById(this.pickupId.Value).gameObject;
      else if (this.mode == SpawnArtfulDodgerGun.Mode.LootTable)
        gameObject = this.lootTable.SelectByWeightWithoutDuplicatesFullPrereqs((List<GameObject>) null);
      else
        Debug.LogError((object) "Tried to give an item to the player but no valid mode was selected.");
      PlayerController player = !(bool) (Object) component1.TalkingPlayer ? GameManager.Instance.PrimaryPlayer : component1.TalkingPlayer;
      Gun gun = (Gun) null;
      if ((bool) (Object) player.CurrentGun)
      {
        MimicGunController component2 = player.CurrentGun.GetComponent<MimicGunController>();
        if ((bool) (Object) component2)
          component2.ForceClearMimic(true);
      }
      if (LootEngine.TryGivePrefabToPlayer(gameObject, player, true))
        gun = player.GetComponentInChildren<ArtfulDodgerGunController>().GetComponent<Gun>();
      List<ArtfulDodgerRoomController> componentsAbsoluteInRoom = component1.GetAbsoluteParentRoom().GetComponentsAbsoluteInRoom<ArtfulDodgerRoomController>();
      ArtfulDodgerRoomController dodgerRoomController = componentsAbsoluteInRoom == null || componentsAbsoluteInRoom.Count <= 0 ? (ArtfulDodgerRoomController) null : componentsAbsoluteInRoom[0];
      gun.CurrentAmmo = !((Object) dodgerRoomController == (Object) null) ? Mathf.RoundToInt(dodgerRoomController.NumberShots) : this.numberOfShotsAllowed.Value;
      gun.gameObject.AddComponent<PostShootProjectileModifier>().NumberBouncesToSet = !((Object) dodgerRoomController == (Object) null) ? Mathf.RoundToInt(dodgerRoomController.NumberBounces) : this.numberOfBouncesAllowed.Value;
      dodgerRoomController.Activate(this.Fsm);
      this.Finish();
    }

    public enum Mode
    {
      SpecifyPickup,
      LootTable,
    }

    public enum SpawnLocation
    {
      GiveToPlayer,
      AtPlayer,
      AtTalkDoer,
      OffsetFromPlayer,
      OffsetFromTalkDoer,
    }
  }
}
