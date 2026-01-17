// Decompiled with JetBrains decompiler
// Type: PickupMover
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using Pathfinding;
using System;
using UnityEngine;

#nullable disable
public class PickupMover : BraveBehaviour
{
  public float pathInterval = 0.25f;
  public float acceleration = 2.5f;
  public float maxSpeed = 15f;
  public float minRadius;
  [NonSerialized]
  public bool moveIfRoomUnclear;
  [NonSerialized]
  public bool stopPathingOnContact;
  private RoomHandler m_room;
  private bool m_radiusValid;
  private bool m_shouldPath;
  private Path m_currentPath;
  private float m_pathTimer;
  private Vector2 m_lastPosition;
  private float m_currentSpeed;
  private const CellTypes c_pathableTiles = CellTypes.FLOOR | CellTypes.PIT;

  public void Start()
  {
    this.m_room = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(this.specRigidbody.UnitCenter.ToIntVector2(VectorConversions.Floor));
    if (this.m_room == null)
      UnityEngine.Object.Destroy((UnityEngine.Object) this);
    else if (this.moveIfRoomUnclear)
      this.m_shouldPath = true;
    else if (!this.m_room.HasActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear))
      this.m_shouldPath = true;
    else
      this.m_room.OnEnemiesCleared += new System.Action(this.RoomCleared);
    this.m_lastPosition = this.specRigidbody.UnitCenter;
  }

  private void TestRadius(PlayerController targetPlayer)
  {
    if (!(bool) (UnityEngine.Object) targetPlayer || this.m_radiusValid && !this.stopPathingOnContact)
      return;
    if (targetPlayer.CurrentRoom != this.m_room)
      this.m_radiusValid = true;
    if ((double) this.minRadius <= 0.0)
      this.m_radiusValid = true;
    else if ((double) Vector2.Distance(targetPlayer.CenterPosition, this.sprite.WorldCenter) > (double) this.minRadius)
    {
      this.m_radiusValid = true;
    }
    else
    {
      if (!this.stopPathingOnContact)
        return;
      this.m_shouldPath = false;
      UnityEngine.Object.Destroy((UnityEngine.Object) this);
    }
  }

  public void Update()
  {
    this.m_pathTimer -= BraveTime.DeltaTime;
    Vector2 unitCenter = this.specRigidbody.UnitCenter;
    PlayerController playerClosestToPoint = GameManager.Instance.GetActivePlayerClosestToPoint(unitCenter, true);
    this.TestRadius(playerClosestToPoint);
    if (this.m_shouldPath && this.m_radiusValid)
    {
      if ((bool) (UnityEngine.Object) this.debris)
        this.debris.enabled = false;
      if (!(bool) (UnityEngine.Object) playerClosestToPoint || playerClosestToPoint.IsFalling || !playerClosestToPoint.specRigidbody.CollideWithOthers)
      {
        this.m_currentSpeed = 0.0f;
      }
      else
      {
        if ((double) this.m_pathTimer <= 0.0)
        {
          IntVector2 intVector2_1 = unitCenter.ToIntVector2(VectorConversions.Floor);
          IntVector2 intVector2_2 = playerClosestToPoint.specRigidbody.UnitCenter.ToIntVector2(VectorConversions.Floor);
          if (Pathfinder.Instance.IsValidPathCell(intVector2_1) && Pathfinder.Instance.IsValidPathCell(intVector2_2) && Pathfinder.Instance.GetPath(intVector2_1, intVector2_2, out this.m_currentPath, new IntVector2?(IntVector2.One), CellTypes.FLOOR | CellTypes.PIT))
          {
            if (this.m_currentPath.Count == 0)
              this.m_currentPath = (Path) null;
            else
              this.m_currentPath.Smooth(this.specRigidbody.UnitCenter, this.specRigidbody.UnitDimensions / 2f, CellTypes.FLOOR | CellTypes.PIT, false, IntVector2.One);
          }
          this.m_pathTimer = this.pathInterval;
        }
        this.m_currentSpeed = Mathf.Min(this.m_currentSpeed + this.acceleration * BraveTime.DeltaTime, this.maxSpeed);
      }
      this.specRigidbody.Velocity = this.GetPathVelocityContribution(playerClosestToPoint);
    }
    else
    {
      if (!Mathf.Approximately(this.transform.position.x % (1f / 16f), 0.0f))
      {
        this.transform.position = this.transform.position.Quantize(1f / 16f);
        this.specRigidbody.Reinitialize();
      }
      this.m_currentSpeed = 0.0f;
      this.specRigidbody.Velocity = Vector2.zero;
    }
    this.m_lastPosition = unitCenter;
  }

  public void RoomCleared()
  {
    this.m_shouldPath = true;
    this.m_room.OnEnemiesCleared -= new System.Action(this.RoomCleared);
  }

  private Vector2 GetPathVelocityContribution(PlayerController targetPlayer)
  {
    if (!(bool) (UnityEngine.Object) targetPlayer)
      return Vector2.zero;
    if (this.m_currentPath == null || this.m_currentPath.Count == 0)
      return this.m_currentSpeed * (targetPlayer.specRigidbody.UnitCenter - this.specRigidbody.UnitCenter).normalized;
    Vector2 unitCenter = this.specRigidbody.UnitCenter;
    Vector2 firstCenterVector2 = this.m_currentPath.GetFirstCenterVector2();
    bool flag = false;
    if ((double) Vector2.Distance(unitCenter, firstCenterVector2) < (double) PhysicsEngine.PixelToUnit(1))
    {
      flag = true;
    }
    else
    {
      Vector2 b = BraveMathCollege.ClosestPointOnLineSegment(firstCenterVector2, this.m_lastPosition, unitCenter);
      if ((double) Vector2.Distance(firstCenterVector2, b) < (double) PhysicsEngine.PixelToUnit(1))
        flag = true;
    }
    if (flag)
    {
      this.m_currentPath.RemoveFirst();
      if (this.m_currentPath.Count == 0)
      {
        this.m_currentPath = (Path) null;
        return Vector2.zero;
      }
    }
    return this.m_currentSpeed * (firstCenterVector2 - unitCenter).normalized;
  }

  protected override void OnDestroy() => base.OnDestroy();
}
