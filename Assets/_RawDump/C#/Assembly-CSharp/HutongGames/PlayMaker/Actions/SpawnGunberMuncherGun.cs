// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SpawnGunberMuncherGun
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Spawns a pickup (gun or item) in the world or gives it directly to the player.")]
[ActionCategory(".NPCs")]
public class SpawnGunberMuncherGun : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("Where to spawn the item at.")]
  public SpawnGunberMuncherGun.SpawnLocation spawnLocation;
  [HutongGames.PlayMaker.Tooltip("Offset from the TalkDoer to spawn the item at.")]
  public Vector2 spawnOffset;
  [NonSerialized]
  public Gun firstRecycledGun;
  [NonSerialized]
  public Gun secondRecycledGun;

  public override void Reset()
  {
    this.spawnLocation = SpawnGunberMuncherGun.SpawnLocation.GiveToPlayer;
    this.spawnOffset = Vector2.zero;
  }

  public override string ErrorCheck() => string.Empty;

  public override void OnEnter()
  {
    TalkDoerLite component = this.Owner.GetComponent<TalkDoerLite>();
    GameObject itemToSpawn = (GameObject) null;
    PlayerController player = !(bool) (UnityEngine.Object) component.TalkingPlayer ? GameManager.Instance.PrimaryPlayer : component.TalkingPlayer;
    if (this.spawnLocation == SpawnGunberMuncherGun.SpawnLocation.GiveToPlayer)
    {
      LootEngine.TryGivePrefabToPlayer(itemToSpawn, player);
    }
    else
    {
      Vector2 spawnPosition;
      if (this.spawnLocation == SpawnGunberMuncherGun.SpawnLocation.AtPlayer || this.spawnLocation == SpawnGunberMuncherGun.SpawnLocation.OffsetFromPlayer)
        spawnPosition = player.specRigidbody.UnitCenter;
      else if (this.spawnLocation == SpawnGunberMuncherGun.SpawnLocation.AtTalkDoer || this.spawnLocation == SpawnGunberMuncherGun.SpawnLocation.OffsetFromTalkDoer)
      {
        spawnPosition = !((UnityEngine.Object) component.specRigidbody != (UnityEngine.Object) null) ? component.sprite.WorldCenter : component.specRigidbody.UnitCenter;
      }
      else
      {
        Debug.LogError((object) "Tried to give an item to the player but no valid spawn location was selected.");
        spawnPosition = GameManager.Instance.PrimaryPlayer.CenterPosition;
      }
      if (this.spawnLocation == SpawnGunberMuncherGun.SpawnLocation.OffsetFromPlayer || this.spawnLocation == SpawnGunberMuncherGun.SpawnLocation.OffsetFromTalkDoer)
        spawnPosition += this.spawnOffset;
      LootEngine.SpewLoot(itemToSpawn, (Vector3) spawnPosition);
    }
    this.Finish();
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
