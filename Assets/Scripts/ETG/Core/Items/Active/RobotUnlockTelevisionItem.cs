// Decompiled with JetBrains decompiler
// Type: RobotUnlockTelevisionItem
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

public class RobotUnlockTelevisionItem : PlayerItem
  {
    public DodgeRollStats RollStats;
    protected PlayerController m_owner;
    protected PlayerController.DodgeRollState m_dodgeRollState;

    public override void Pickup(PlayerController player)
    {
      this.m_owner = player;
      player.OnRollStarted += new Action<PlayerController, Vector2>(this.HandleRoll);
      base.Pickup(player);
    }

    protected override void OnPreDrop(PlayerController user)
    {
      user.OnRollStarted -= new Action<PlayerController, Vector2>(this.HandleRoll);
      this.m_owner = (PlayerController) null;
      base.OnPreDrop(user);
    }

    private void HandleRoll(PlayerController arg1, Vector2 arg2)
    {
      this.m_owner.DropActiveItem((PlayerItem) this, 0.0f).inertialMass = 1E+07f;
      int num = (int) AkSoundEngine.PostEvent("Play_OBJ_item_throw_01", GameManager.Instance.gameObject);
    }

    protected override void OnDestroy()
    {
      if ((UnityEngine.Object) this.m_owner != (UnityEngine.Object) null)
        this.m_owner.OnRollStarted -= new Action<PlayerController, Vector2>(this.HandleRoll);
      base.OnDestroy();
    }

    protected override void DoEffect(PlayerController user)
    {
      int num = (int) AkSoundEngine.PostEvent("Play_OBJ_item_throw_01", GameManager.Instance.gameObject);
      DebrisObject debrisObject = this.m_owner.DropActiveItem((PlayerItem) this, 7f);
      GameObject gameObject = debrisObject.gameObject;
      UnityEngine.Object.Destroy((UnityEngine.Object) debrisObject);
      SpeculativeRigidbody speculativeRigidbody = gameObject.AddComponent<SpeculativeRigidbody>();
      speculativeRigidbody.transform.position = user.specRigidbody.UnitBottomLeft.ToVector3ZisY();
      speculativeRigidbody.PixelColliders = new List<PixelCollider>();
      speculativeRigidbody.PixelColliders.Add(new PixelCollider()
      {
        ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
        ManualOffsetX = 2,
        ManualOffsetY = 3,
        ManualWidth = 11,
        ManualHeight = 10,
        CollisionLayer = CollisionLayer.LowObstacle,
        Enabled = true,
        IsTrigger = false
      });
      speculativeRigidbody.Reinitialize();
      PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(speculativeRigidbody);
      speculativeRigidbody.RegisterSpecificCollisionException(user.specRigidbody);
      user.StartCoroutine(this.HandleDodgeRoll(speculativeRigidbody, user.unadjustedAimPoint.XY() - user.sprite.WorldCenter));
    }

    private bool HandlePitfall(SpeculativeRigidbody targetRigidbody)
    {
      if (!GameManager.Instance.Dungeon.ShouldReallyFall((Vector3) targetRigidbody.UnitCenter))
        return false;
      DebrisObject debrisObject = targetRigidbody.gameObject.AddComponent<DebrisObject>();
      debrisObject.canRotate = false;
      debrisObject.Trigger(Vector3.zero, 0.01f);
      UnityEngine.Object.Destroy((UnityEngine.Object) targetRigidbody);
      return true;
    }

    [DebuggerHidden]
    private IEnumerator HandleDodgeRoll(SpeculativeRigidbody targetRigidbody, Vector2 direction)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new RobotUnlockTelevisionItem__HandleDodgeRollc__Iterator0()
      {
        targetRigidbody = targetRigidbody,
        direction = direction,
        _this = this
      };
    }

    private void OnGrounded(SpeculativeRigidbody targetRigidbody)
    {
      PlayerItem component = targetRigidbody.GetComponent<PlayerItem>();
      if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
        return;
      component.ForceAsExtant = true;
      if (RoomHandler.unassignedInteractableObjects.Contains((IPlayerInteractable) component))
        return;
      RoomHandler.unassignedInteractableObjects.Add((IPlayerInteractable) component);
    }

    private float GetDodgeRollSpeed(
      float dodgeRollTimer,
      AnimationCurve speedCurve,
      float rollTime,
      float rollDistance)
    {
      float time1 = Mathf.Clamp01((dodgeRollTimer - BraveTime.DeltaTime) / rollTime);
      float time2 = Mathf.Clamp01(dodgeRollTimer / rollTime);
      return (Mathf.Clamp01(speedCurve.Evaluate(time2)) - Mathf.Clamp01(speedCurve.Evaluate(time1))) * rollDistance / BraveTime.DeltaTime;
    }

    private void RollPitMovementRestrictor(
      SpeculativeRigidbody specRigidbody,
      IntVector2 prevPixelOffset,
      IntVector2 pixelOffset,
      ref bool validLocation)
    {
      if (!validLocation || this.m_dodgeRollState != PlayerController.DodgeRollState.OnGround)
        return;
      Func<IntVector2, bool> func = (Func<IntVector2, bool>) (pixel =>
      {
        Vector2 unitMidpoint = PhysicsEngine.PixelToUnitMidpoint(pixel);
        if (!GameManager.Instance.Dungeon.CellSupportsFalling((Vector3) unitMidpoint))
          return false;
        List<SpeculativeRigidbody> platformsAt = GameManager.Instance.Dungeon.GetPlatformsAt((Vector3) unitMidpoint);
        if (platformsAt != null)
        {
          for (int index = 0; index < platformsAt.Count; ++index)
          {
            if (platformsAt[index].PrimaryPixelCollider.ContainsPixel(pixel))
              return false;
          }
        }
        IntVector2 intVector2 = unitMidpoint.ToIntVector2(VectorConversions.Floor);
        return !GameManager.Instance.Dungeon.data.isTopWall(intVector2.x, intVector2.y) || true;
      });
      PixelCollider primaryPixelCollider = specRigidbody.PrimaryPixelCollider;
      if (primaryPixelCollider == null)
        return;
      IntVector2 intVector2_1 = pixelOffset - prevPixelOffset;
      if (intVector2_1 == IntVector2.Down && func(primaryPixelCollider.LowerLeft + pixelOffset) && func(primaryPixelCollider.LowerRight + pixelOffset) && (!func(primaryPixelCollider.UpperRight + prevPixelOffset) || !func(primaryPixelCollider.UpperLeft + prevPixelOffset)))
        validLocation = false;
      else if (intVector2_1 == IntVector2.Right && func(primaryPixelCollider.LowerRight + pixelOffset) && func(primaryPixelCollider.UpperRight + pixelOffset) && (!func(primaryPixelCollider.UpperLeft + prevPixelOffset) || !func(primaryPixelCollider.LowerLeft + prevPixelOffset)))
        validLocation = false;
      else if (intVector2_1 == IntVector2.Up && func(primaryPixelCollider.UpperRight + pixelOffset) && func(primaryPixelCollider.UpperLeft + pixelOffset) && (!func(primaryPixelCollider.LowerLeft + prevPixelOffset) || !func(primaryPixelCollider.LowerRight + prevPixelOffset)))
      {
        validLocation = false;
      }
      else
      {
        if (!(intVector2_1 == IntVector2.Left) || !func(primaryPixelCollider.UpperLeft + pixelOffset) || !func(primaryPixelCollider.LowerLeft + pixelOffset) || func(primaryPixelCollider.LowerRight + prevPixelOffset) && func(primaryPixelCollider.UpperRight + prevPixelOffset))
          return;
        validLocation = false;
      }
    }
  }

