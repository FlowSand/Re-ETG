// Decompiled with JetBrains decompiler
// Type: DemonFaceController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable
public class DemonFaceController : DungeonPlaceableBehaviour, IPlaceConfigurable
{
  public SpeculativeRigidbody interiorRigidbody;
  public LootData WaterRewardTable;
  private List<PlayerController> m_containedPlayers = new List<PlayerController>();
  public int RequiredCurrency = 100;
  public float RequiredCurse = 0.01f;
  private bool m_hasDrunkDeeplyFromTheSweetSweetWaters;

  private void Start()
  {
    this.specRigidbody.OnEnterTrigger += new SpeculativeRigidbody.OnTriggerDelegate(this.HandleTrigger);
    this.specRigidbody.OnExitTrigger += new SpeculativeRigidbody.OnTriggerExitDelegate(this.HandleTriggerExit);
    this.interiorRigidbody.OnPreRigidbodyCollision += new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.HandleRigidbodyCollision);
    this.interiorRigidbody.OnHitByBeam += new Action<BasicBeamController>(this.HandleBeam);
  }

  private void HandleRigidbodyCollision(
    SpeculativeRigidbody myRigidbody,
    PixelCollider myPixelCollider,
    SpeculativeRigidbody otherRigidbody,
    PixelCollider otherPixelCollider)
  {
    if (!(bool) (UnityEngine.Object) otherRigidbody.projectile)
      return;
    otherRigidbody.projectile.ForceDestruction();
    PhysicsEngine.SkipCollision = true;
  }

  private void HandleTriggerExit(
    SpeculativeRigidbody specRigidbody,
    SpeculativeRigidbody sourceSpecRigidbody)
  {
    if (!(bool) (UnityEngine.Object) specRigidbody)
      return;
    PlayerController component = specRigidbody.GetComponent<PlayerController>();
    if (!(bool) (UnityEngine.Object) component)
      return;
    this.m_containedPlayers.Remove(component);
  }

  private void HandleBeam(BasicBeamController obj)
  {
    if (!(bool) (UnityEngine.Object) obj.projectile || (obj.projectile.damageTypes | CoreDamageTypes.Water) != obj.projectile.damageTypes)
      return;
    this.HitWithWater();
  }

  private void HandleTrigger(
    SpeculativeRigidbody specRigidbody,
    SpeculativeRigidbody sourceSpecRigidbody,
    CollisionData collisionData)
  {
    PlayerController component = specRigidbody.GetComponent<PlayerController>();
    if ((bool) (UnityEngine.Object) component)
    {
      this.m_containedPlayers.Add(component);
      bool flag = false;
      if (component.carriedConsumables.Currency >= this.RequiredCurrency)
        flag = true;
      if ((double) PlayerStats.GetTotalCurse() >= (double) this.RequiredCurse)
        flag = true;
      if (flag)
        GameManager.Instance.Dungeon.StartCoroutine(this.HandleEjection(component, true));
      else
        GameManager.Instance.Dungeon.StartCoroutine(this.HandleEjection(component, false));
    }
    else
    {
      Projectile projectile = specRigidbody.projectile;
      if (!(bool) (UnityEngine.Object) projectile || (projectile.damageTypes | CoreDamageTypes.Water) != projectile.damageTypes)
        return;
      this.HitWithWater();
    }
  }

  private void WarpToBlackMarket(PlayerController triggerPlayer)
  {
    GameManager.Instance.platformInterface.AchievementUnlock(Achievement.REACH_BLACK_MARKET);
    for (int index = 0; index < GameManager.Instance.Dungeon.data.rooms.Count; ++index)
    {
      RoomHandler room = GameManager.Instance.Dungeon.data.rooms[index];
      if (room.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.SPECIAL && room.area.PrototypeRoomName == "Black Market")
      {
        triggerPlayer.AttemptTeleportToRoom(room, true, true);
        if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
          GameManager.Instance.GetOtherPlayer(triggerPlayer).AttemptTeleportToRoom(room, true);
      }
    }
  }

  [DebuggerHidden]
  private IEnumerator HandleEjection(PlayerController triggerPlayer, bool success)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new DemonFaceController.\u003CHandleEjection\u003Ec__Iterator0()
    {
      triggerPlayer = triggerPlayer,
      success = success,
      \u0024this = this
    };
  }

  private void HitWithWater()
  {
    if (this.m_hasDrunkDeeplyFromTheSweetSweetWaters)
      return;
    this.m_hasDrunkDeeplyFromTheSweetSweetWaters = true;
    GameManager.Instance.Dungeon.StartCoroutine(this.ProvideDumbReward());
  }

  [DebuggerHidden]
  private IEnumerator ProvideDumbReward()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new DemonFaceController.\u003CProvideDumbReward\u003Ec__Iterator1()
    {
      \u0024this = this
    };
  }

  public void ConfigureOnPlacement(RoomHandler room)
  {
    room.OptionalDoorTopDecorable = ResourceCache.Acquire("Global Prefabs/Purple_Lantern") as GameObject;
  }
}
