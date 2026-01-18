// Decompiled with JetBrains decompiler
// Type: DemonWallController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using UnityEngine;

#nullable disable

public class DemonWallController : BraveBehaviour
  {
    private DemonWallController.State m_state;
    private bool m_isMotionRestricted;
    private int m_cachedCameraMinY;
    private RoomHandler m_room;
    private int m_leftId;
    private int m_rightId;

    public bool IsCameraLocked => this.m_state == DemonWallController.State.LockCamera;

    public void Start()
    {
      this.specRigidbody.OnCollision += new Action<CollisionData>(this.OnCollision);
      this.aiActor.ManualKnockbackHandling = true;
      this.aiActor.ParentRoom.Entered += new RoomHandler.OnEnteredEventHandler(this.PlayerEnteredRoom);
      this.aiActor.healthHaver.OnPreDeath += new Action<Vector2>(this.OnPreDeath);
      this.CameraPos = this.specRigidbody.UnitCenter + new Vector2(0.0f, (float) ((double) this.specRigidbody.HitboxPixelCollider.UnitDimensions.y - (double) GameManager.Instance.MainCameraController.Camera.orthographicSize + 0.5));
      this.m_room = this.aiActor.ParentRoom;
    }

    public void Update()
    {
      if (this.m_state == DemonWallController.State.Intro)
      {
        if (this.specRigidbody.CollideWithOthers)
        {
          this.m_state = DemonWallController.State.LockCamera;
          CameraController cameraController = GameManager.Instance.MainCameraController;
          cameraController.SetManualControl(true);
          cameraController.OverridePosition = (Vector3) this.CameraPos;
          Vector2 unitBottomCenter = this.specRigidbody.UnitBottomCenter;
          this.m_leftId = DeadlyDeadlyGoopManager.RegisterUngoopableCircle(unitBottomCenter + new Vector2(-1.5f, 1.5f), 1.5f);
          this.m_rightId = DeadlyDeadlyGoopManager.RegisterUngoopableCircle(unitBottomCenter + new Vector2(1.5f, 1.5f), 1.5f);
        }
      }
      else if (this.m_state == DemonWallController.State.LockCamera)
      {
        Vector2 unitBottomCenter = this.specRigidbody.UnitBottomCenter;
        DeadlyDeadlyGoopManager.UpdateUngoopableCircle(this.m_leftId, unitBottomCenter + new Vector2(-1.5f, 1.5f), 1.5f);
        DeadlyDeadlyGoopManager.UpdateUngoopableCircle(this.m_rightId, unitBottomCenter + new Vector2(1.5f, 1.5f), 1.5f);
        this.MarkInaccessible(true);
      }
      this.m_cachedCameraMinY = PhysicsEngine.UnitToPixel(BraveUtility.ViewportToWorldpoint(new Vector2(0.5f, 0.0f), ViewportType.Camera).y);
    }

    protected override void OnDestroy()
    {
      this.RestrictMotion(false);
      this.ModifyCamera(false);
      this.MarkInaccessible(false);
      if ((bool) (UnityEngine.Object) this.specRigidbody)
        this.specRigidbody.OnCollision -= new Action<CollisionData>(this.OnCollision);
      if ((bool) (UnityEngine.Object) this.aiActor && this.aiActor.ParentRoom != null)
        this.aiActor.ParentRoom.Entered -= new RoomHandler.OnEnteredEventHandler(this.PlayerEnteredRoom);
      if ((bool) (UnityEngine.Object) this.aiActor && (bool) (UnityEngine.Object) this.aiActor.healthHaver)
        this.aiActor.healthHaver.OnPreDeath -= new Action<Vector2>(this.OnPreDeath);
      base.OnDestroy();
    }

    public Vector2 CameraPos { get; set; }

    private void OnCollision(CollisionData rigidbodyCollision)
    {
      if (rigidbodyCollision.collisionType != CollisionData.CollisionType.Rigidbody || this.aiActor.IsFrozen)
        return;
      PlayerController component = rigidbodyCollision.OtherRigidbody.GetComponent<PlayerController>();
      MajorBreakable majorBreakable = rigidbodyCollision.OtherRigidbody.majorBreakable;
      AIActor aiActor = rigidbodyCollision.OtherRigidbody.aiActor;
      if (!this.healthHaver.IsDead && (UnityEngine.Object) component != (UnityEngine.Object) null)
      {
        Vector2 direction = -Vector2.up;
        IntVector2 intVector2 = component.specRigidbody.UnitBottomCenter.ToIntVector2(VectorConversions.Floor);
        if (GameManager.Instance.Dungeon.data.isTopWall(intVector2.x, intVector2.y))
          component.healthHaver.ApplyDamage(1000f, direction, this.aiActor.GetActorName(), damageCategory: DamageCategory.Collision, ignoreInvulnerabilityFrames: true);
        component.healthHaver.ApplyDamage(this.aiActor.CollisionDamage, direction, this.aiActor.GetActorName(), damageCategory: DamageCategory.Collision);
        component.knockbackDoer.ApplySourcedKnockback(direction, this.aiActor.CollisionKnockbackStrength, this.gameObject, true);
        if ((bool) (UnityEngine.Object) this.knockbackDoer)
          this.knockbackDoer.ApplySourcedKnockback(-direction, component.collisionKnockbackStrength, this.gameObject);
        this.aiActor.CollisionVFX.SpawnAtPosition((Vector3) rigidbodyCollision.Contact, sourceNormal: new Vector2?(Vector2.zero), sourceVelocity: new Vector2?(Vector2.zero), heightOffGround: new float?(2f));
        component.specRigidbody.RegisterGhostCollisionException(this.specRigidbody);
      }
      if ((bool) (UnityEngine.Object) aiActor && (bool) (UnityEngine.Object) aiActor.CompanionOwner)
      {
        Debug.LogError((object) "knocking back companion");
        aiActor.knockbackDoer.ApplySourcedKnockback(Vector2.down, 50f, this.gameObject, true);
      }
      if (!(bool) (UnityEngine.Object) majorBreakable)
        return;
      majorBreakable.ApplyDamage(1000f, Vector2.down, true, ForceDamageOverride: true);
    }

    private void PlayerEnteredRoom(PlayerController playerController) => this.RestrictMotion(true);

    private void OnPreDeath(Vector2 finalDirection)
    {
      this.RestrictMotion(false);
      this.MarkInaccessible(false);
      this.aiActor.ParentRoom.Entered -= new RoomHandler.OnEnteredEventHandler(this.PlayerEnteredRoom);
      if (this.m_state != DemonWallController.State.LockCamera)
        return;
      this.m_state = DemonWallController.State.Dead;
    }

    private void PlayerMovementRestrictor(
      SpeculativeRigidbody playerSpecRigidbody,
      IntVector2 prevPixelOffset,
      IntVector2 pixelOffset,
      ref bool validLocation)
    {
      if (!validLocation)
        return;
      int maxY = playerSpecRigidbody.PixelColliders[1].MaxY;
      int minY = this.specRigidbody.PixelColliders[1].MinY;
      if (maxY + pixelOffset.y >= minY && pixelOffset.y > prevPixelOffset.y)
      {
        validLocation = false;
      }
      else
      {
        IntVector2 intVector2 = pixelOffset - prevPixelOffset;
        CellArea area = this.aiActor.ParentRoom.area;
        if (intVector2.x < 0)
        {
          if (playerSpecRigidbody.PixelColliders[0].MinX + pixelOffset.x < area.basePosition.x * 16 /*0x10*/)
          {
            validLocation = false;
            return;
          }
        }
        else if (intVector2.x > 0 && playerSpecRigidbody.PixelColliders[0].MaxX + pixelOffset.x > (area.basePosition.x + area.dimensions.x) * 16 /*0x10*/ - 1)
        {
          validLocation = false;
          return;
        }
        if (intVector2.y >= 0 || playerSpecRigidbody.PixelColliders[0].MinY + pixelOffset.y >= this.m_cachedCameraMinY)
          return;
        validLocation = false;
      }
    }

    private void RestrictMotion(bool value)
    {
      if (this.m_isMotionRestricted == value)
        return;
      if (value)
      {
        if (!GameManager.HasInstance || GameManager.Instance.IsLoadingLevel || GameManager.IsReturningToBreach)
          return;
        for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
          GameManager.Instance.AllPlayers[index].specRigidbody.MovementRestrictor += new SpeculativeRigidbody.MovementRestrictorDelegate(this.PlayerMovementRestrictor);
      }
      else
      {
        if (!GameManager.HasInstance || GameManager.IsReturningToBreach)
          return;
        for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
        {
          PlayerController allPlayer = GameManager.Instance.AllPlayers[index];
          if ((bool) (UnityEngine.Object) allPlayer)
            allPlayer.specRigidbody.MovementRestrictor -= new SpeculativeRigidbody.MovementRestrictorDelegate(this.PlayerMovementRestrictor);
        }
      }
      this.m_isMotionRestricted = value;
    }

    public void ModifyCamera(bool value)
    {
      if (!GameManager.HasInstance || GameManager.Instance.IsLoadingLevel || GameManager.IsReturningToBreach)
        return;
      if (value)
        GameManager.Instance.MainCameraController.SetManualControl(true, false);
      else
        GameManager.Instance.MainCameraController.SetManualControl(false);
    }

    private void MarkInaccessible(bool inaccessible)
    {
      if (!GameManager.HasInstance || GameManager.Instance.IsLoadingLevel || GameManager.IsReturningToBreach || !(bool) (UnityEngine.Object) this.aiActor || this.aiActor.ParentRoom == null)
        return;
      DungeonData data = GameManager.Instance.Dungeon.data;
      IntVector2 basePosition = this.m_room.area.basePosition;
      IntVector2 intVector2 = this.m_room.area.basePosition + this.aiActor.ParentRoom.area.dimensions - IntVector2.One;
      if (inaccessible && (bool) (UnityEngine.Object) this.specRigidbody)
        basePosition.y = (int) this.specRigidbody.UnitBottomCenter.y - 3;
      for (int x = basePosition.x; x <= intVector2.x; ++x)
      {
        for (int y = basePosition.y; y <= intVector2.y; ++y)
        {
          if (data.CheckInBoundsAndValid(x, y))
            data[x, y].IsPlayerInaccessible = inaccessible;
        }
      }
    }

    private enum State
    {
      Intro,
      LockCamera,
      Dead,
    }
  }

