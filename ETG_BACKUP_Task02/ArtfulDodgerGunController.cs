// Decompiled with JetBrains decompiler
// Type: ArtfulDodgerGunController
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
public class ArtfulDodgerGunController : MonoBehaviour
{
  private Gun m_gun;
  private Projectile m_lastProjectile;
  private RoomHandler m_startRoom;

  private void Awake()
  {
    this.m_gun = this.GetComponent<Gun>();
    this.m_gun.PostProcessProjectile += new Action<Projectile>(this.NotifyFired);
  }

  private void NotifyFired(Projectile obj)
  {
    this.m_lastProjectile = obj;
    GameStatsManager.Instance.RegisterStatChange(TrackedStats.WINCHESTER_SHOTS_FIRED, 1f);
  }

  private void Start()
  {
    this.m_startRoom = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(this.transform.position.IntXY());
    this.m_gun.DoubleWideLaserSight = true;
    List<ArtfulDodgerRoomController> componentsAbsoluteInRoom = this.m_startRoom.GetComponentsAbsoluteInRoom<ArtfulDodgerRoomController>();
    if (componentsAbsoluteInRoom == null || componentsAbsoluteInRoom.Count <= 0)
      return;
    componentsAbsoluteInRoom[0].gamePlayingPlayer = this.m_gun.CurrentOwner as PlayerController;
  }

  [DebuggerHidden]
  private IEnumerator HandleDelayedReward()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new ArtfulDodgerGunController.\u003CHandleDelayedReward\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  private void Update()
  {
    if ((bool) (UnityEngine.Object) this.m_gun && (bool) (UnityEngine.Object) this.m_gun.CurrentOwner)
      (this.m_gun.CurrentOwner as PlayerController).HighAccuracyAimMode = true;
    if (this.m_gun.ammo == 0)
    {
      if (!(this.m_gun.CurrentOwner is PlayerController))
        return;
      PlayerController currentOwner = this.m_gun.CurrentOwner as PlayerController;
      currentOwner.HighAccuracyAimMode = false;
      currentOwner.SuppressThisClick = true;
      currentOwner.inventory.DestroyGun(this.m_gun);
      currentOwner.StartCoroutine(this.HandleDelayedReward());
    }
    else
    {
      if (!((UnityEngine.Object) this.m_gun.CurrentOwner != (UnityEngine.Object) null) || !(this.m_gun.CurrentOwner is PlayerController))
        return;
      PlayerController currentOwner = this.m_gun.CurrentOwner as PlayerController;
      if (currentOwner.CurrentRoom == this.m_startRoom)
        return;
      currentOwner.HighAccuracyAimMode = false;
      currentOwner.SuppressThisClick = true;
      currentOwner.inventory.DestroyGun(this.m_gun);
      currentOwner.StartCoroutine(this.HandleDelayedReward());
    }
  }
}
