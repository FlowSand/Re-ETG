// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SpawnPickup
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Spawns a pickup (gun or item) in the world or gives it directly to the player.")]
[ActionCategory(".NPCs")]
public class SpawnPickup : FsmStateAction
{
  public SpawnPickup.Mode mode;
  [HutongGames.PlayMaker.Tooltip("Item to spawn.")]
  public FsmInt pickupId;
  [HutongGames.PlayMaker.Tooltip("Loot table to choose an item from.")]
  public GenericLootTable lootTable;
  [HutongGames.PlayMaker.Tooltip("Where to spawn the item at.")]
  public SpawnPickup.SpawnLocation spawnLocation;
  [HutongGames.PlayMaker.Tooltip("Offset from the TalkDoer to spawn the item at.")]
  public Vector2 spawnOffset;

  public override void Reset()
  {
    this.mode = SpawnPickup.Mode.SpecifyPickup;
    this.pickupId = (FsmInt) -1;
    this.lootTable = (GenericLootTable) null;
    this.spawnLocation = SpawnPickup.SpawnLocation.GiveToPlayer;
    this.spawnOffset = Vector2.zero;
  }

  public override string ErrorCheck()
  {
    string empty = string.Empty;
    if (this.mode == SpawnPickup.Mode.SpecifyPickup && (Object) PickupObjectDatabase.GetById(this.pickupId.Value) == (Object) null)
      empty += "Invalid item ID.\n";
    if (this.mode == SpawnPickup.Mode.LootTable && !(bool) (Object) this.lootTable)
      empty += "Invalid loot table.\n";
    return empty;
  }

  public override void OnEnter()
  {
    TalkDoerLite component = this.Owner.GetComponent<TalkDoerLite>();
    PlayerController player = !(bool) (Object) component.TalkingPlayer ? GameManager.Instance.PrimaryPlayer : component.TalkingPlayer;
    GameObject gameObject = (GameObject) null;
    bool flag = false;
    if (this.mode == SpawnPickup.Mode.SpecifyPickup)
      gameObject = PickupObjectDatabase.GetById(this.pickupId.Value).gameObject;
    else if (this.mode == SpawnPickup.Mode.LootTable)
    {
      gameObject = this.lootTable.SelectByWeightWithoutDuplicatesFullPrereqs((List<GameObject>) null);
      flag = true;
    }
    else if (this.mode == SpawnPickup.Mode.DaveStyleFloorReward)
    {
      gameObject = GameManager.Instance.RewardManager.GetRewardObjectDaveStyle(player);
      flag = true;
    }
    else
      Debug.LogError((object) "Tried to give an item to the player but no valid mode was selected.");
    if (flag && GameStatsManager.Instance.IsRainbowRun)
      LootEngine.SpawnBowlerNote(GameManager.Instance.RewardManager.BowlerNoteOtherSource, GameManager.Instance.PrimaryPlayer.CenterPosition + new Vector2(-0.5f, -0.5f), player.CurrentRoom, true);
    else if (this.spawnLocation == SpawnPickup.SpawnLocation.GiveToPlayer)
      LootEngine.TryGivePrefabToPlayer(gameObject, player);
    else if (this.spawnLocation == SpawnPickup.SpawnLocation.GiveToBothPlayers)
    {
      LootEngine.TryGivePrefabToPlayer(gameObject, GameManager.Instance.PrimaryPlayer);
      if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
        LootEngine.TryGivePrefabToPlayer(gameObject, GameManager.Instance.SecondaryPlayer);
    }
    else
    {
      Vector2 vector2;
      if (this.spawnLocation == SpawnPickup.SpawnLocation.AtPlayer || this.spawnLocation == SpawnPickup.SpawnLocation.OffsetFromPlayer)
        vector2 = player.specRigidbody.UnitCenter;
      else if (this.spawnLocation == SpawnPickup.SpawnLocation.AtTalkDoer || this.spawnLocation == SpawnPickup.SpawnLocation.OffsetFromTalkDoer)
        vector2 = !((Object) component.specRigidbody != (Object) null) ? component.sprite.WorldCenter : component.specRigidbody.UnitCenter;
      else if (this.spawnLocation == SpawnPickup.SpawnLocation.RoomSpawnPoint)
      {
        vector2 = player.CurrentRoom.GetBestRewardLocation(IntVector2.One, RoomHandler.RewardLocationStyle.Original, false).ToVector2();
      }
      else
      {
        Debug.LogError((object) "Tried to give an item to the player but no valid spawn location was selected.");
        vector2 = GameManager.Instance.PrimaryPlayer.CenterPosition;
      }
      if (this.spawnLocation == SpawnPickup.SpawnLocation.OffsetFromPlayer || this.spawnLocation == SpawnPickup.SpawnLocation.OffsetFromTalkDoer)
        vector2 += this.spawnOffset;
      LootEngine.SpawnItem(gameObject, (Vector3) vector2, Vector2.zero, 0.0f);
      LootEngine.DoDefaultItemPoof(vector2);
    }
    this.Finish();
  }

  public enum Mode
  {
    SpecifyPickup,
    LootTable,
    DaveStyleFloorReward,
  }

  public enum SpawnLocation
  {
    GiveToPlayer,
    AtPlayer,
    AtTalkDoer,
    OffsetFromPlayer,
    OffsetFromTalkDoer,
    RoomSpawnPoint,
    GiveToBothPlayers,
  }
}
