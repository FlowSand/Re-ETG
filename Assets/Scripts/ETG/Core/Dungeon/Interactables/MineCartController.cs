// Decompiled with JetBrains decompiler
// Type: MineCartController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Dungeon.Interactables
{
    public class MineCartController : DungeonPlaceableBehaviour, IPlayerInteractable
    {
      [NonSerialized]
      public bool ForceActive;
      [DwarfConfigurable]
      public bool IsOnlyPlayerMinecart;
      [DwarfConfigurable]
      public bool AlwaysMoving;
      public tk2dSpriteAnimator childAnimator;
      public Transform attachTransform;
      public SpeculativeRigidbody carriedCargo;
      public bool MoveCarriedCargoIntoCart = true;
      public string HorizontalAnimationName;
      public string VerticalAnimationName;
      public float KnockbackStrengthPlayer = 3f;
      public float KnockbackStrengthEnemy = 10f;
      [DwarfConfigurable]
      public float MaxSpeed = 7f;
      public float TimeToMaxSpeed = 1f;
      private const float UnoccupiedSpeedDecay = 4f;
      private CartTurretController m_turret;
      public Transform Sparks_A;
      public Transform Sparks_B;
      [NonSerialized]
      public MineCartController.CartOccupationState occupation;
      protected GameActor m_rider;
      protected GameActor m_secondaryRider;
      protected PathMover m_pathMover;
      protected float m_elapsedOccupied;
      protected float m_elapsedSecondary;
      protected bool m_handlingQueuedAnimation;
      protected RoomHandler m_room;
      protected List<MineCartController> m_minecartsInRoom;
      private float m_justRolledInTimer;
      private bool m_hasHandledCornerAnimation;
      private bool m_wasPushedThisFrame;
      private SpeculativeRigidbody m_pusher;
      private bool m_cartSoundActive;
      private List<CollisionData> m_cachedCollisionList = new List<CollisionData>();
      private float m_lastAccelVector;
      private Dictionary<string, string> m_animationMap = new Dictionary<string, string>();
      protected Coroutine m_primaryLerpCoroutine;
      protected Coroutine m_secondaryLerpCoroutine;

      public float MaxSpeedEnemy => this.MaxSpeed;

      public float MaxSpeedPlayer => this.MaxSpeed;

      public GameActor CurrentInhabitant => this.m_rider;

      private void Awake()
      {
        if ((UnityEngine.Object) this.carriedCargo != (UnityEngine.Object) null && (UnityEngine.Object) this.carriedCargo.specRigidbody != (UnityEngine.Object) null)
        {
          this.specRigidbody.RegisterSpecificCollisionException(this.carriedCargo.specRigidbody);
          this.carriedCargo.specRigidbody.RegisterSpecificCollisionException(this.specRigidbody);
          this.m_turret = this.carriedCargo.GetComponent<CartTurretController>();
        }
        this.m_pathMover = this.GetComponent<PathMover>();
        this.m_pathMover.ForceCornerDelayHack = true;
        if (!(bool) (UnityEngine.Object) this.majorBreakable)
          return;
        this.majorBreakable.OnBreak += new System.Action(this.DestroyMineCart);
      }

      [DebuggerHidden]
      private IEnumerator Start()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new MineCartController.<Start>c__Iterator0()
        {
          $this = this
        };
      }

      private bool IsOnlyMinecartInRoom() => this.m_minecartsInRoom.Count == 1;

      private bool IsReachableFromPosition(Vector2 targetPoint)
      {
        Path path = new Path();
        Pathfinder.Instance.GetPath(targetPoint.ToIntVector2(VectorConversions.Floor), this.specRigidbody.UnitCenter.ToIntVector2(VectorConversions.Floor), out path, new IntVector2?(IntVector2.One));
        return path != null && path.WillReachFinalGoal;
      }

      private void HandlePlayerPitRespawn(PlayerController obj) => this.m_pathMover.WarpToStart();

      private void WarpToNearestPointOnPath(Vector2 targetPoint)
      {
        this.m_pathMover.WarpToNearestPoint(targetPoint);
        PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(this.specRigidbody);
      }

      private void HandlePitFall(Vector2 lastVec)
      {
        this.Evacuate(isPitfalling: true);
        this.EvacuateSecondary(isPitfalling: true);
        this.m_pathMover.Paused = true;
        this.StartCoroutine(this.StartFallAnimation(lastVec.ToIntVector2(VectorConversions.Floor).MajorAxis * 2, this.specRigidbody));
      }

      [DebuggerHidden]
      private IEnumerator StartFallAnimation(IntVector2 dir, SpeculativeRigidbody targetRigidbody)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new MineCartController.<StartFallAnimation>c__Iterator1()
        {
          targetRigidbody = targetRigidbody,
          dir = dir,
          $this = this
        };
      }

      protected override void OnDestroy()
      {
        this.StopSound();
        base.OnDestroy();
      }

      private void HandlePreRigidbodyCollision(
        SpeculativeRigidbody myRigidbody,
        PixelCollider myPixelCollider,
        SpeculativeRigidbody otherRigidbody,
        PixelCollider otherPixelCollider)
      {
        if ((bool) (UnityEngine.Object) otherRigidbody.minorBreakable && otherRigidbody.minorBreakable.isImpermeableToGameActors)
          PhysicsEngine.SkipCollision = true;
        if (this.occupation != MineCartController.CartOccupationState.EMPTY && this.occupation != MineCartController.CartOccupationState.PLAYER)
          return;
        if (otherRigidbody.gameActor is PlayerController)
        {
          PlayerController gameActor = otherRigidbody.gameActor as PlayerController;
          if (gameActor.IsDodgeRolling && (UnityEngine.Object) gameActor.previousMineCart != (UnityEngine.Object) null && (UnityEngine.Object) gameActor.previousMineCart != (UnityEngine.Object) this)
          {
            gameActor.ForceStopDodgeRoll();
            gameActor.ToggleGunRenderers(true, string.Empty);
            this.m_justRolledInTimer = 0.5f;
            this.BecomeOccupied(gameActor);
          }
          else if (this.occupation != MineCartController.CartOccupationState.EMPTY || !Mathf.Approximately(this.m_pathMover.PathSpeed, 0.0f))
            ;
        }
        else
        {
          if (!(otherRigidbody.gameActor is AIActor) || (otherRigidbody.gameActor as AIActor).IsNormalEnemy)
            return;
          PhysicsEngine.SkipCollision = true;
        }
      }

      private void HandleRigidbodyCollision(CollisionData rigidbodyCollision)
      {
        if (this.m_pathMover.Paused)
          return;
        Vector2 cone = BraveMathCollege.VectorToCone(-rigidbodyCollision.Normal, 15f);
        AIActor aiActor = rigidbodyCollision.OtherRigidbody.aiActor;
        if ((bool) (UnityEngine.Object) aiActor && (bool) (UnityEngine.Object) aiActor.healthHaver && aiActor.healthHaver.IsAlive && this.CurrentInhabitant is PlayerController && (double) this.specRigidbody.Velocity.magnitude > 2.0)
          aiActor.healthHaver.ApplyDamage(50f, cone, "Minecart Damage");
        if (!((UnityEngine.Object) rigidbodyCollision.OtherRigidbody.knockbackDoer != (UnityEngine.Object) null))
          return;
        if ((bool) (UnityEngine.Object) rigidbodyCollision.OtherRigidbody.gameActor && rigidbodyCollision.OtherRigidbody.gameActor is PlayerController)
          rigidbodyCollision.OtherRigidbody.knockbackDoer.ApplySourcedKnockback(cone, this.KnockbackStrengthPlayer * Mathf.Abs(this.m_pathMover.PathSpeed), this.gameObject);
        else
          rigidbodyCollision.OtherRigidbody.knockbackDoer.ApplySourcedKnockback(cone, this.KnockbackStrengthEnemy * Mathf.Abs(this.m_pathMover.PathSpeed), this.gameObject);
        if ((bool) (UnityEngine.Object) this.m_rider)
          this.m_rider.specRigidbody.RegisterTemporaryCollisionException(rigidbodyCollision.OtherRigidbody, 1f);
        if ((bool) (UnityEngine.Object) this.m_secondaryRider)
          this.m_secondaryRider.specRigidbody.RegisterTemporaryCollisionException(rigidbodyCollision.OtherRigidbody, 1f);
        this.specRigidbody.RegisterTemporaryCollisionException(rigidbodyCollision.OtherRigidbody, 1f);
      }

      private void Update()
      {
        if (!(bool) (UnityEngine.Object) this.m_pathMover)
          return;
        bool flag1 = GameManager.Instance.PlayerIsNearRoom(this.m_room);
        bool flag2 = (UnityEngine.Object) this.m_turret != (UnityEngine.Object) null && this.m_turret.Inactive;
        this.m_justRolledInTimer -= BraveTime.DeltaTime;
        if (flag2 || this.occupation == MineCartController.CartOccupationState.EMPTY && !this.ForceActive)
        {
          this.m_elapsedOccupied = 0.0f;
          this.m_elapsedSecondary = 0.0f;
          if ((double) this.m_pathMover.PathSpeed != 0.0)
          {
            this.m_pathMover.Paused = false;
            if (!this.m_wasPushedThisFrame)
              this.m_pathMover.PathSpeed = Mathf.MoveTowards(this.m_pathMover.PathSpeed, 0.0f, 4f * BraveTime.DeltaTime);
          }
          else if (!this.m_pathMover.Paused)
          {
            CellData cellData = GameManager.Instance.Dungeon.data[this.transform.position.IntXY()];
            if (cellData == null || cellData.type == CellType.WALL)
              this.m_pathMover.PathSpeed = Mathf.Sign(this.m_pathMover.PathSpeed) * this.MaxSpeedEnemy;
            this.m_pathMover.Paused = true;
          }
        }
        else if (this.occupation == MineCartController.CartOccupationState.CARGO)
        {
          if (flag1)
          {
            if (this.m_pathMover.Paused)
              this.m_pathMover.Paused = false;
            this.m_pathMover.PathSpeed = BraveMathCollege.SmoothLerp(0.0f, this.MaxSpeedEnemy, Mathf.Clamp01(this.m_elapsedOccupied / this.TimeToMaxSpeed));
            this.m_elapsedOccupied += BraveTime.DeltaTime;
            if (!(bool) (UnityEngine.Object) this.carriedCargo)
              this.occupation = MineCartController.CartOccupationState.EMPTY;
          }
          else
            this.m_pathMover.Paused = true;
        }
        else
        {
          if (this.m_pathMover.Paused)
            this.m_pathMover.Paused = false;
          if (this.occupation == MineCartController.CartOccupationState.PLAYER)
          {
            this.m_pathMover.PathSpeed = Mathf.Clamp(this.m_pathMover.PathSpeed, -this.MaxSpeedPlayer, this.MaxSpeedPlayer);
          }
          else
          {
            this.m_pathMover.PathSpeed = BraveMathCollege.SmoothLerp(0.0f, this.occupation != MineCartController.CartOccupationState.PLAYER ? this.MaxSpeedEnemy : this.MaxSpeedPlayer, Mathf.Clamp01(Mathf.Max(this.m_elapsedOccupied, this.m_elapsedSecondary) / this.TimeToMaxSpeed));
            if (this.ForceActive)
              this.m_pathMover.PathSpeed = this.MaxSpeedEnemy;
          }
          if ((UnityEngine.Object) this.m_rider != (UnityEngine.Object) null)
            this.m_elapsedOccupied += BraveTime.DeltaTime;
          if ((UnityEngine.Object) this.m_secondaryRider != (UnityEngine.Object) null)
            this.m_elapsedSecondary += BraveTime.DeltaTime;
          if (!GameManager.Instance.IsPaused)
          {
            if (this.occupation == MineCartController.CartOccupationState.PLAYER)
            {
              this.HandlePlayerRiderInput(this.m_rider, this.m_elapsedOccupied);
              this.HandlePlayerRiderInput(this.m_secondaryRider, this.m_elapsedSecondary);
            }
            if (!(bool) (UnityEngine.Object) this.m_rider || this.m_rider.healthHaver.IsDead)
              this.Evacuate();
            if (!(bool) (UnityEngine.Object) this.m_secondaryRider || this.m_secondaryRider.healthHaver.IsDead)
              this.EvacuateSecondary();
          }
        }
        if ((double) this.m_pathMover.AbsPathSpeed > 0.0 && flag1)
          this.StartSound();
        else
          this.StopSound();
        if (this.m_cartSoundActive)
        {
          int num = (int) AkSoundEngine.SetRTPCValue("Pitch_Minecart", this.m_pathMover.AbsPathSpeed / this.MaxSpeed);
        }
        Vector2 directionFromPreviousNode = PhysicsEngine.PixelToUnit(this.specRigidbody.PathTarget) - this.specRigidbody.Position.UnitPosition;
        if (!this.m_hasHandledCornerAnimation && !this.m_handlingQueuedAnimation && (double) directionFromPreviousNode.magnitude < 0.5)
        {
          Vector2 directionToNextNode = this.m_pathMover.GetNextTargetPosition() - PhysicsEngine.PixelToUnit(this.specRigidbody.PathTarget);
          this.m_hasHandledCornerAnimation = true;
          this.HandleTurnAnimation(directionFromPreviousNode, directionToNextNode);
        }
        this.HandlePushCarts();
        this.EnsureRiderPosition();
        this.m_wasPushedThisFrame = false;
        this.m_pusher = (SpeculativeRigidbody) null;
        this.UpdateSparksTransforms();
      }

      private void StartSound()
      {
        if (this.m_cartSoundActive)
          return;
        this.m_cartSoundActive = true;
        int num = (int) AkSoundEngine.PostEvent("Play_OBJ_minecart_loop_01", this.gameObject);
      }

      private void StopSound()
      {
        if (!this.m_cartSoundActive)
          return;
        this.m_cartSoundActive = false;
        int num = (int) AkSoundEngine.PostEvent("Stop_OBJ_minecart_loop_01", this.gameObject);
      }

      private void UpdateSparksTransforms()
      {
        if ((UnityEngine.Object) this.Sparks_A == (UnityEngine.Object) null)
          return;
        Vector2 velocity = this.specRigidbody.Velocity;
        if ((double) velocity.magnitude < 2.0)
        {
          this.Sparks_A.gameObject.SetActive(false);
          this.Sparks_B.gameObject.SetActive(false);
        }
        else
        {
          this.Sparks_A.GetComponent<Renderer>().enabled = true;
          this.Sparks_B.GetComponent<Renderer>().enabled = true;
          this.Sparks_A.gameObject.SetActive(true);
          this.Sparks_B.gameObject.SetActive(true);
          if (velocity.IsHorizontal())
          {
            ParticleSystem componentInChildren1 = this.Sparks_A.GetComponentInChildren<ParticleSystem>();
            ParticleSystem componentInChildren2 = this.Sparks_B.GetComponentInChildren<ParticleSystem>();
            this.Sparks_A.localPosition = new Vector3(23f / 16f, 0.375f, -1.125f);
            componentInChildren1.transform.localRotation = Quaternion.Euler(-30f, -125.25f, 55f);
            this.Sparks_B.localPosition = new Vector3(0.5f, 0.375f, -1.125f);
            componentInChildren2.transform.localRotation = Quaternion.Euler(-30f, -125.25f, 55f);
            if ((double) velocity.x < 0.0)
            {
              this.Sparks_B.localPosition = new Vector3(23f / 16f, 17f / 16f, -7f / 16f);
              this.Sparks_B.GetComponent<Renderer>().enabled = false;
              if (componentInChildren1.simulationSpace != ParticleSystemSimulationSpace.Local)
                return;
              componentInChildren1.transform.localRotation = Quaternion.Euler(-10f, 90f, 0.0f);
              componentInChildren2.transform.localRotation = Quaternion.Euler(-10f, 90f, 0.0f);
            }
            else
            {
              this.Sparks_A.localPosition = new Vector3(0.5f, 17f / 16f, -7f / 16f);
              this.Sparks_A.GetComponent<Renderer>().enabled = false;
              if (componentInChildren1.simulationSpace != ParticleSystemSimulationSpace.Local)
                return;
              componentInChildren1.transform.localRotation = Quaternion.Euler(-10f, -125.25f, 55f);
              componentInChildren2.transform.localRotation = Quaternion.Euler(-10f, -125.25f, 55f);
            }
          }
          else
          {
            this.Sparks_A.localPosition = new Vector3(0.625f, 0.125f, -1.375f);
            this.Sparks_A.GetComponentInChildren<ParticleSystem>().transform.localRotation = Quaternion.Euler(-45f, 0.0f, -45f);
            this.Sparks_B.localPosition = new Vector3(21f / 16f, 0.125f, -1.375f);
            this.Sparks_B.GetComponentInChildren<ParticleSystem>().transform.localRotation = Quaternion.Euler(-45f, 0.0f, -45f);
            if ((double) velocity.y > 0.0)
              return;
            this.Sparks_A.GetComponent<Renderer>().enabled = false;
            this.Sparks_B.GetComponent<Renderer>().enabled = false;
          }
        }
      }

      public void ApplyVelocity(float speed)
      {
        if ((UnityEngine.Object) this.m_pathMover == (UnityEngine.Object) null)
          this.m_pathMover = this.GetComponent<PathMover>();
        this.m_pathMover.Paused = false;
        this.m_pathMover.PathSpeed = Mathf.Max(this.MaxSpeedEnemy, this.m_pathMover.PathSpeed + speed);
      }

      protected void HandlePushCarts()
      {
        if ((double) this.m_pathMover.PathSpeed == 0.0)
          return;
        MineCartController mineCartController = this.CheckWillHitMineCart();
        if ((UnityEngine.Object) mineCartController != (UnityEngine.Object) null && (double) this.m_pathMover.AbsPathSpeed / (double) this.MaxSpeed < 0.30000001192092896)
        {
          this.m_pathMover.PathSpeed = 0.0f;
        }
        else
        {
          float f = (Mathf.Min(this.m_pathMover.AbsPathSpeed, this.MaxSpeedPlayer) + 1f) * Mathf.Sign(this.m_pathMover.PathSpeed);
          if (!((UnityEngine.Object) mineCartController != (UnityEngine.Object) null) || (double) Mathf.Abs(f) <= (double) Mathf.Abs(mineCartController.m_pathMover.PathSpeed) && (double) Mathf.Sign(f) == (double) Mathf.Sign(mineCartController.m_pathMover.PathSpeed))
            return;
          float parametrizedPathPosition1 = mineCartController.m_pathMover.GetParametrizedPathPosition();
          float parametrizedPathPosition2 = this.m_pathMover.GetParametrizedPathPosition();
          if (((double) this.m_pathMover.PathSpeed <= 0.0 || (double) parametrizedPathPosition1 <= (double) parametrizedPathPosition2) && ((double) parametrizedPathPosition1 >= 0.25 || (double) parametrizedPathPosition2 <= 0.75) && ((double) this.m_pathMover.PathSpeed >= 0.0 || (double) parametrizedPathPosition1 >= (double) parametrizedPathPosition2) && ((double) parametrizedPathPosition1 <= 0.75 || (double) parametrizedPathPosition2 >= 0.25))
            return;
          mineCartController.m_pathMover.Paused = false;
          mineCartController.m_pathMover.PathSpeed = f;
          mineCartController.m_wasPushedThisFrame = true;
          mineCartController.m_pusher = this.specRigidbody;
          mineCartController.HandlePushCarts();
        }
      }

      protected MineCartController CheckWillHitMineCart()
      {
        MineCartController mineCartController = (MineCartController) null;
        this.m_cachedCollisionList.Clear();
        IntVector2 intVector2 = (PhysicsEngine.UnitToPixel((PhysicsEngine.PixelToUnit(this.specRigidbody.PathTarget) - this.specRigidbody.Position.UnitPosition).normalized * this.specRigidbody.PathSpeed).ToVector2() * BraveTime.DeltaTime).ToIntVector2(VectorConversions.Ceil);
        SpeculativeRigidbody speculativeRigidbody1 = (SpeculativeRigidbody) null;
        SpeculativeRigidbody speculativeRigidbody2 = (SpeculativeRigidbody) null;
        if (this.occupation == MineCartController.CartOccupationState.CARGO)
          speculativeRigidbody1 = this.carriedCargo;
        else if (this.occupation != MineCartController.CartOccupationState.EMPTY)
        {
          if ((bool) (UnityEngine.Object) this.m_rider)
            speculativeRigidbody1 = this.m_rider.specRigidbody;
          if ((bool) (UnityEngine.Object) this.m_secondaryRider)
            speculativeRigidbody2 = this.m_secondaryRider.specRigidbody;
        }
        if (PhysicsEngine.Instance.OverlapCast(this.specRigidbody, this.m_cachedCollisionList, false, true, new int?(), new int?(), false, new Vector2?(), (Func<SpeculativeRigidbody, bool>) null, speculativeRigidbody1, speculativeRigidbody2, this.m_pusher))
        {
          for (int index = 0; index < this.m_cachedCollisionList.Count; ++index)
          {
            if ((bool) (UnityEngine.Object) this.m_cachedCollisionList[index].OtherRigidbody)
            {
              MineCartController component = this.m_cachedCollisionList[index].OtherRigidbody.GetComponent<MineCartController>();
              if (!((UnityEngine.Object) component == (UnityEngine.Object) null))
              {
                float parametrizedPathPosition1 = component.m_pathMover.GetParametrizedPathPosition();
                float parametrizedPathPosition2 = this.m_pathMover.GetParametrizedPathPosition();
                if ((double) this.m_pathMover.PathSpeed > 0.0 && (double) parametrizedPathPosition1 > (double) parametrizedPathPosition2 || (double) parametrizedPathPosition1 < 0.25 && (double) parametrizedPathPosition2 > 0.75 || (double) this.m_pathMover.PathSpeed < 0.0 && (double) parametrizedPathPosition1 < (double) parametrizedPathPosition2 || (double) parametrizedPathPosition1 > 0.75 && (double) parametrizedPathPosition2 < 0.25)
                  return component;
              }
            }
          }
        }
        CollisionData result;
        if (PhysicsEngine.Instance.RigidbodyCastWithIgnores(this.specRigidbody, intVector2, out result, false, true, new int?(), true, speculativeRigidbody1, speculativeRigidbody2, this.m_pusher))
        {
          mineCartController = result.OtherRigidbody.GetComponent<MineCartController>();
          if ((UnityEngine.Object) mineCartController != (UnityEngine.Object) null)
          {
            for (int index = 0; index < this.m_cachedCollisionList.Count; ++index)
            {
              if ((UnityEngine.Object) this.m_cachedCollisionList[index].OtherRigidbody == (UnityEngine.Object) mineCartController.specRigidbody)
              {
                mineCartController = (MineCartController) null;
                break;
              }
            }
          }
        }
        CollisionData.Pool.Free(ref result);
        return mineCartController;
      }

      protected void HandlePlayerRiderInput(GameActor targetRider, float targetElapsed)
      {
        if ((UnityEngine.Object) targetRider == (UnityEngine.Object) null)
          return;
        PlayerController playerController = targetRider as PlayerController;
        playerController.ZeroVelocityThisFrame = true;
        if ((double) targetElapsed <= (double) BraveTime.DeltaTime)
          return;
        GungeonActions activeActions = BraveInput.GetInstanceForPlayer(playerController.PlayerIDX).ActiveActions;
        if (activeActions.InteractAction.WasPressed && (double) this.m_justRolledInTimer <= 0.0)
        {
          if ((UnityEngine.Object) targetRider == (UnityEngine.Object) this.m_rider)
            this.Evacuate();
          else if ((UnityEngine.Object) targetRider == (UnityEngine.Object) this.m_secondaryRider)
            this.EvacuateSecondary();
        }
        if ((UnityEngine.Object) targetRider == (UnityEngine.Object) this.m_rider || (UnityEngine.Object) targetRider == (UnityEngine.Object) this.m_secondaryRider && (UnityEngine.Object) this.m_rider == (UnityEngine.Object) null)
        {
          float f = Vector2.Dot(BraveUtility.GetMajorAxis(this.m_pathMover.GetPositionOfNode(this.m_pathMover.CurrentIndex) - this.transform.position.XY()), activeActions.Move.Vector) * 15f * Mathf.Sign(this.m_pathMover.PathSpeed) * BraveTime.DeltaTime;
          if ((double) this.m_pathMover.AbsPathSpeed / (double) this.MaxSpeed > 0.10000000149011612 && (double) Mathf.Sign(f) != (double) this.m_lastAccelVector && (double) f != 0.0 && (double) Mathf.Sign(f) != (double) Mathf.Sign(this.m_pathMover.PathSpeed))
          {
            int num = (int) AkSoundEngine.PostEvent("Play_OBJ_minecart_brake_01", this.gameObject);
            this.m_lastAccelVector = Mathf.Sign(f);
          }
          this.m_pathMover.PathSpeed += f;
          if ((double) f == 0.0 && (double) this.m_pathMover.AbsPathSpeed / (double) this.MaxSpeedPlayer < 0.30000001192092896)
            this.m_pathMover.PathSpeed = Mathf.MoveTowards(this.m_pathMover.PathSpeed, 0.0f, 4f * BraveTime.DeltaTime);
        }
        if (!activeActions.DodgeRollAction.WasPressed || playerController.WasPausedThisFrame)
          return;
        if ((double) activeActions.Move.Vector.magnitude > 0.10000000149011612)
        {
          if ((UnityEngine.Object) targetRider == (UnityEngine.Object) this.m_rider)
          {
            this.Evacuate(true);
          }
          else
          {
            if (!((UnityEngine.Object) targetRider == (UnityEngine.Object) this.m_secondaryRider))
              return;
            this.EvacuateSecondary(true);
          }
        }
        else
        {
          Vector2 normalized = this.specRigidbody.Velocity.normalized;
          string empty = string.Empty;
          string animationName = (double) Mathf.Abs(normalized.x) >= 0.10000000149011612 ? ((double) normalized.y <= 0.10000000149011612 ? "dodge_left" : "dodge_left_bw") + (!playerController.ArmorlessAnimations || (double) playerController.healthHaver.Armor != 0.0 ? string.Empty : "_armorless") : ((double) normalized.y <= 0.10000000149011612 ? "dodge" : "dodge_bw") + (!playerController.ArmorlessAnimations || (double) playerController.healthHaver.Armor != 0.0 ? string.Empty : "_armorless");
          playerController.QueueSpecificAnimation(animationName);
        }
      }

      protected void SetAnimation(string animationName, float clipFpsFraction)
      {
        if (string.IsNullOrEmpty(animationName))
          return;
        float num = 4f;
        tk2dSpriteAnimationClip clipByName1 = this.spriteAnimator.GetClipByName(animationName);
        if (!this.spriteAnimator.IsPlaying(clipByName1))
          this.spriteAnimator.Play(clipByName1);
        this.spriteAnimator.ClipFps = Mathf.Max(num, BraveMathCollege.UnboundedLerp(num, clipByName1.fps, clipFpsFraction));
        string empty = string.Empty;
        string name;
        if (this.m_animationMap.ContainsKey(animationName))
        {
          name = this.m_animationMap[animationName];
        }
        else
        {
          name = animationName.Replace("_A", "_B");
          this.m_animationMap.Add(animationName, name);
        }
        tk2dSpriteAnimationClip clipByName2 = this.childAnimator.GetClipByName(name);
        if (!this.childAnimator.IsPlaying(clipByName2))
          this.childAnimator.Play(clipByName2);
        this.childAnimator.ClipFps = Mathf.Max(num, BraveMathCollege.UnboundedLerp(num, clipByName2.fps, clipFpsFraction));
      }

      public void HandleTurnAnimation(Vector2 directionFromPreviousNode, Vector2 directionToNextNode)
      {
        IntVector2 intMajorAxis1 = BraveUtility.GetIntMajorAxis(directionFromPreviousNode);
        IntVector2 intMajorAxis2 = BraveUtility.GetIntMajorAxis(directionToNextNode);
        float clipFpsFraction = 2f;
        if (intMajorAxis1 == IntVector2.North)
        {
          if (intMajorAxis2 == IntVector2.East)
            this.SetAnimation("minecart_turn_TL_VH_A", clipFpsFraction);
          else if (intMajorAxis2 == IntVector2.West)
            this.SetAnimation("minecart_turn_TR_VH_A", clipFpsFraction);
        }
        else if (intMajorAxis1 == IntVector2.East)
        {
          if (intMajorAxis2 == IntVector2.North)
            this.SetAnimation("minecart_turn_BR_HV_A", clipFpsFraction);
          else if (intMajorAxis2 == IntVector2.South)
            this.SetAnimation("minecart_turn_TR_HV_A", clipFpsFraction);
        }
        else if (intMajorAxis1 == IntVector2.South)
        {
          if (intMajorAxis2 == IntVector2.East)
            this.SetAnimation("minecart_turn_BL_VH_A", clipFpsFraction);
          else if (intMajorAxis2 == IntVector2.West)
            this.SetAnimation("minecart_turn_BR_VH_A", clipFpsFraction);
        }
        else if (intMajorAxis1 == IntVector2.West)
        {
          if (intMajorAxis2 == IntVector2.North)
            this.SetAnimation("minecart_turn_BL_HV_A", clipFpsFraction);
          else if (intMajorAxis2 == IntVector2.South)
            this.SetAnimation("minecart_turn_TL_HV_A", clipFpsFraction);
        }
        this.m_handlingQueuedAnimation = true;
      }

      public void HandleCornerReached(
        Vector2 directionFromPreviousNode,
        Vector2 directionToNextNode,
        bool hasNextNode)
      {
        this.m_pathMover.PathSpeed = Mathf.Sign(this.m_pathMover.PathSpeed) * ((double) this.m_pathMover.AbsPathSpeed <= (double) this.MaxSpeedEnemy ? this.m_pathMover.AbsPathSpeed + 1f : this.m_pathMover.AbsPathSpeed);
        if (!this.m_hasHandledCornerAnimation)
          this.HandleTurnAnimation(directionFromPreviousNode, directionToNextNode);
        this.m_hasHandledCornerAnimation = false;
        if (hasNextNode)
          return;
        if (GameManager.Instance.Dungeon.CellSupportsFalling((Vector3) this.specRigidbody.UnitCenter))
          this.HandlePitFall(directionFromPreviousNode);
        else
          this.StartCoroutine(this.DelayedWarpToStart());
      }

      [DebuggerHidden]
      private IEnumerator DelayedWarpToStart()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new MineCartController.<DelayedWarpToStart>c__Iterator2()
        {
          $this = this
        };
      }

      private void UpdateAnimations()
      {
        Vector2 velocity = this.specRigidbody.Velocity;
        float clipFpsFraction = this.m_pathMover.PathSpeed / (this.occupation != MineCartController.CartOccupationState.PLAYER ? this.MaxSpeedEnemy : this.MaxSpeedPlayer);
        if (this.m_handlingQueuedAnimation)
        {
          if (!this.spriteAnimator.IsPlaying(this.spriteAnimator.CurrentClip))
            this.m_handlingQueuedAnimation = false;
          if (this.spriteAnimator.CurrentClip == null || (double) this.spriteAnimator.ClipFps <= 0.0)
            this.m_handlingQueuedAnimation = false;
        }
        if ((double) velocity.x == 0.0 && (double) velocity.y == 0.0)
        {
          this.spriteAnimator.Stop();
          this.childAnimator.Stop();
        }
        else if ((double) Mathf.Abs(velocity.x) < (double) Mathf.Abs(velocity.y))
        {
          if (this.m_handlingQueuedAnimation)
            return;
          this.SetAnimation(this.VerticalAnimationName, clipFpsFraction);
        }
        else
        {
          if ((double) Mathf.Abs(velocity.y) >= (double) Mathf.Abs(velocity.x) || this.m_handlingQueuedAnimation)
            return;
          this.SetAnimation(this.HorizontalAnimationName, clipFpsFraction);
        }
      }

      private void LateUpdate()
      {
        if (!this.m_pathMover.Paused)
          this.UpdateAnimations();
        if (this.occupation == MineCartController.CartOccupationState.EMPTY)
        {
          if ((double) this.sprite.HeightOffGround != -1.0)
          {
            this.sprite.HeightOffGround = -1f;
            this.sprite.UpdateZDepth();
          }
          if ((double) this.childAnimator.sprite.HeightOffGround == 0.125)
            return;
          this.childAnimator.sprite.IsPerpendicular = false;
          this.childAnimator.sprite.HeightOffGround = 0.125f;
          this.childAnimator.sprite.UpdateZDepth();
        }
        else
        {
          if ((double) Mathf.Abs(this.specRigidbody.Velocity.y) > (double) Mathf.Abs(this.specRigidbody.Velocity.x))
          {
            if ((double) this.sprite.HeightOffGround != -1.25)
            {
              this.sprite.HeightOffGround = -1.25f;
              this.sprite.UpdateZDepth();
            }
          }
          else if ((double) this.sprite.HeightOffGround != -0.60000002384185791)
          {
            this.sprite.HeightOffGround = -0.6f;
            this.sprite.UpdateZDepth();
          }
          if ((double) this.childAnimator.sprite.HeightOffGround != -2.5)
          {
            this.childAnimator.sprite.IsPerpendicular = true;
            this.childAnimator.sprite.HeightOffGround = -2.5f;
            this.childAnimator.sprite.UpdateZDepth();
          }
          this.childAnimator.sprite.UpdateZDepth();
        }
      }

      public void BecomeCargoOccupied()
      {
        if (this.MoveCarriedCargoIntoCart)
        {
          this.carriedCargo.transform.position = this.attachTransform.position + (this.carriedCargo.transform.position.XY() - this.carriedCargo.sprite.WorldBottomCenter).ToVector3ZUp();
          this.carriedCargo.specRigidbody.Reinitialize();
        }
        this.carriedCargo.specRigidbody.RegisterSpecificCollisionException(this.specRigidbody);
        this.specRigidbody.RegisterSpecificCollisionException(this.carriedCargo.specRigidbody);
        this.carriedCargo.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.LowObstacle));
        if ((bool) (UnityEngine.Object) this.carriedCargo.knockbackDoer)
          this.carriedCargo.knockbackDoer.knockbackMultiplier = 0.0f;
        if ((bool) (UnityEngine.Object) this.carriedCargo.minorBreakable && this.carriedCargo.minorBreakable.explodesOnBreak)
          this.carriedCargo.minorBreakable.OnBreak += (System.Action) (() => this.DestroyMineCart());
        this.specRigidbody.RegisterCarriedRigidbody(this.carriedCargo.specRigidbody);
      }

      private void DestroyMineCart()
      {
        if ((bool) (UnityEngine.Object) this.carriedCargo && (bool) (UnityEngine.Object) this.carriedCargo.minorBreakable)
          this.carriedCargo.transform.parent = (Transform) null;
        this.Evacuate();
        this.EvacuateSecondary();
        this.GetAbsoluteParentRoom().DeregisterInteractable((IPlayerInteractable) this);
        this.m_pathMover.Paused = true;
        UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
      }

      public void BecomeOccupied(PlayerController player)
      {
        if (this.occupation == MineCartController.CartOccupationState.ENEMY)
          return;
        SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite);
        SpriteOutlineManager.RemoveOutlineFromSprite(this.childAnimator.sprite);
        if (this.occupation == MineCartController.CartOccupationState.PLAYER)
        {
          if ((UnityEngine.Object) player == (UnityEngine.Object) this.m_rider)
            return;
          player.currentMineCart = this;
          this.m_elapsedSecondary = 0.0f;
          if (player.IsDodgeRolling)
            player.ForceStopDodgeRoll();
          this.m_secondaryRider = (GameActor) player;
          player.CurrentInputState = PlayerInputState.NoMovement;
          player.ZeroVelocityThisFrame = true;
          this.AttachSecondaryRider();
          StaticReferenceManager.ActiveMineCarts.Add(player, this);
        }
        else
        {
          if (this.occupation != MineCartController.CartOccupationState.EMPTY)
            return;
          this.m_elapsedOccupied = 0.0f;
          player.currentMineCart = this;
          if (player.IsDodgeRolling)
            player.ForceStopDodgeRoll();
          this.m_rider = (GameActor) player;
          this.occupation = MineCartController.CartOccupationState.PLAYER;
          this.specRigidbody.PixelColliders[0].CollisionLayerCollidableOverride |= CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox);
          player.CurrentInputState = PlayerInputState.NoMovement;
          player.ZeroVelocityThisFrame = true;
          this.AttachRider();
          StaticReferenceManager.ActiveMineCarts.Add(player, this);
        }
      }

      public void BecomeOccupied(AIActor enemy)
      {
        if (this.occupation != MineCartController.CartOccupationState.EMPTY)
          return;
        this.m_elapsedOccupied = 0.0f;
        this.m_rider = (GameActor) enemy;
        this.occupation = MineCartController.CartOccupationState.ENEMY;
        this.AttachRider();
      }

      public void EvacuateSpecificPlayer(PlayerController p, bool usePitfallLogic = false)
      {
        if ((UnityEngine.Object) this.m_rider == (UnityEngine.Object) p)
          this.Evacuate(isPitfalling: usePitfallLogic);
        if (!((UnityEngine.Object) this.m_secondaryRider == (UnityEngine.Object) p))
          return;
        this.EvacuateSecondary(isPitfalling: usePitfallLogic);
      }

      private void Evacuate(bool doRoll = false, bool isPitfalling = false)
      {
        if (this.occupation == MineCartController.CartOccupationState.EMPTY)
          return;
        if (this.occupation == MineCartController.CartOccupationState.CARGO)
        {
          if (!(bool) (UnityEngine.Object) this.carriedCargo.minorBreakable)
            return;
          this.carriedCargo.minorBreakable.Break();
        }
        else
        {
          if ((bool) (UnityEngine.Object) this.m_rider)
          {
            this.specRigidbody.DeregisterCarriedRigidbody(this.m_rider.specRigidbody);
            if (this.occupation == MineCartController.CartOccupationState.PLAYER)
            {
              GameManager.Instance.MainCameraController.SetManualControl(false);
              PlayerController rider = this.m_rider as PlayerController;
              rider.currentMineCart = (MineCartController) null;
              rider.CurrentInputState = PlayerInputState.AllInput;
              StaticReferenceManager.ActiveMineCarts.Remove(rider);
              if (doRoll)
              {
                rider.ForceStartDodgeRoll();
                rider.previousMineCart = this;
              }
              else if (isPitfalling)
              {
                rider.previousMineCart = this;
              }
              else
              {
                Vector2 majorAxis = BraveUtility.GetMajorAxis(this.m_pathMover.GetPositionOfNode(this.m_pathMover.CurrentIndex) - this.m_pathMover.transform.position.XY());
                if ((double) (this.m_pathMover.GetPositionOfNode(this.m_pathMover.PreviousIndex) - this.m_pathMover.transform.position.XY()).magnitude < 1.5)
                  majorAxis *= -1f;
                Vector2 vector2 = majorAxis.normalized * -1f;
                if (this.m_primaryLerpCoroutine != null)
                  this.StopCoroutine(this.m_primaryLerpCoroutine);
                this.m_primaryLerpCoroutine = this.StartCoroutine(this.HandleLerpCameraPlayerPosition(rider, -vector2));
                rider.transform.position = rider.transform.position + (majorAxis.normalized * -1f).ToVector3ZUp();
                rider.specRigidbody.Reinitialize();
              }
            }
            if ((bool) (UnityEngine.Object) this.m_rider.knockbackDoer)
              this.m_rider.knockbackDoer.knockbackMultiplier = 1f;
            this.m_rider.FallingProhibited = false;
            this.m_rider.specRigidbody.DeregisterSpecificCollisionException(this.specRigidbody);
            this.specRigidbody.DeregisterSpecificCollisionException(this.m_rider.specRigidbody);
            this.m_rider.specRigidbody.RemoveCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.LowObstacle));
            this.m_rider.specRigidbody.RegisterGhostCollisionException(this.specRigidbody);
            this.specRigidbody.RegisterTemporaryCollisionException(this.m_rider.specRigidbody, 0.25f);
            this.m_rider = (GameActor) null;
          }
          if (!((UnityEngine.Object) this.m_secondaryRider == (UnityEngine.Object) null))
            return;
          this.occupation = MineCartController.CartOccupationState.EMPTY;
          this.specRigidbody.PixelColliders[0].CollisionLayerCollidableOverride &= ~CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox);
        }
      }

      [DebuggerHidden]
      private IEnumerator HandleLerpCameraPlayerPosition(
        PlayerController targetPlayer,
        Vector2 sourceOffset)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new MineCartController.<HandleLerpCameraPlayerPosition>c__Iterator3()
        {
          targetPlayer = targetPlayer,
          sourceOffset = sourceOffset
        };
      }

      private void EvacuateSecondary(bool doRoll = false, bool isPitfalling = false)
      {
        if (this.occupation != MineCartController.CartOccupationState.PLAYER || !((UnityEngine.Object) this.m_secondaryRider != (UnityEngine.Object) null))
          return;
        GameManager.Instance.MainCameraController.SetManualControl(false);
        this.specRigidbody.DeregisterCarriedRigidbody(this.m_secondaryRider.specRigidbody);
        PlayerController secondaryRider = this.m_secondaryRider as PlayerController;
        secondaryRider.currentMineCart = (MineCartController) null;
        secondaryRider.CurrentInputState = PlayerInputState.AllInput;
        StaticReferenceManager.ActiveMineCarts.Remove(secondaryRider);
        if (doRoll)
        {
          secondaryRider.ForceStartDodgeRoll();
          secondaryRider.previousMineCart = this;
        }
        else if (isPitfalling)
        {
          secondaryRider.previousMineCart = this;
        }
        else
        {
          Vector2 majorAxis = BraveUtility.GetMajorAxis(this.m_pathMover.GetPositionOfNode(this.m_pathMover.CurrentIndex) - this.m_pathMover.transform.position.XY());
          Vector2 vector2 = majorAxis.normalized * -1f;
          if (this.m_secondaryLerpCoroutine != null)
            this.StopCoroutine(this.m_secondaryLerpCoroutine);
          this.m_secondaryLerpCoroutine = this.StartCoroutine(this.HandleLerpCameraPlayerPosition(secondaryRider, -vector2));
          secondaryRider.transform.position = secondaryRider.transform.position + (majorAxis.normalized * -1f).ToVector3ZUp();
          secondaryRider.specRigidbody.Reinitialize();
        }
        if ((bool) (UnityEngine.Object) this.m_secondaryRider.knockbackDoer)
          this.m_secondaryRider.knockbackDoer.knockbackMultiplier = 1f;
        this.m_secondaryRider.FallingProhibited = false;
        this.m_secondaryRider.specRigidbody.DeregisterSpecificCollisionException(this.specRigidbody);
        this.specRigidbody.DeregisterSpecificCollisionException(this.m_secondaryRider.specRigidbody);
        this.m_secondaryRider.specRigidbody.RemoveCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.LowObstacle));
        this.m_secondaryRider.specRigidbody.RegisterGhostCollisionException(this.specRigidbody);
        this.specRigidbody.RegisterTemporaryCollisionException(this.m_secondaryRider.specRigidbody, 0.25f);
        this.m_secondaryRider = (GameActor) null;
        if (!((UnityEngine.Object) this.m_rider == (UnityEngine.Object) null))
          return;
        this.occupation = MineCartController.CartOccupationState.EMPTY;
        this.specRigidbody.PixelColliders[0].CollisionLayerCollidableOverride &= ~CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox);
      }

      protected void AttachSecondaryRider()
      {
        Vector2 vector = this.m_secondaryRider.transform.position.XY() - this.m_secondaryRider.specRigidbody.UnitBottomCenter + new Vector2(0.125f, 0.25f);
        if (this.m_secondaryRider is PlayerController)
        {
          Vector2 vector2 = (this.attachTransform.position + vector.ToVector3ZUp()).XY() - this.m_secondaryRider.transform.position.XY();
          if (this.m_secondaryLerpCoroutine != null)
            this.StopCoroutine(this.m_secondaryLerpCoroutine);
          this.m_secondaryLerpCoroutine = this.StartCoroutine(this.HandleLerpCameraPlayerPosition(this.m_secondaryRider as PlayerController, -vector2));
        }
        this.m_secondaryRider.transform.position = this.attachTransform.position + vector.ToVector3ZUp();
        this.m_secondaryRider.specRigidbody.Reinitialize();
        this.m_secondaryRider.specRigidbody.RegisterSpecificCollisionException(this.specRigidbody);
        this.specRigidbody.RegisterSpecificCollisionException(this.m_secondaryRider.specRigidbody);
        this.m_secondaryRider.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.LowObstacle));
        if ((bool) (UnityEngine.Object) this.m_secondaryRider.knockbackDoer)
          this.m_secondaryRider.knockbackDoer.knockbackMultiplier = 0.0f;
        this.m_secondaryRider.FallingProhibited = true;
        this.specRigidbody.RegisterCarriedRigidbody(this.m_secondaryRider.specRigidbody);
      }

      public void ForceUpdatePositions() => this.EnsureRiderPosition();

      protected void EnsureRiderPosition()
      {
        if ((UnityEngine.Object) this.m_rider != (UnityEngine.Object) null)
        {
          Vector2 a = this.attachTransform.position.XY() + (this.m_rider.transform.position.XY() - this.m_rider.specRigidbody.UnitBottomCenter);
          if ((double) Vector2.Distance(a, (Vector2) this.m_rider.transform.position) > 1.0 / 16.0)
          {
            this.m_rider.transform.position = (Vector3) a;
            this.m_rider.specRigidbody.Reinitialize();
          }
        }
        if (!((UnityEngine.Object) this.m_secondaryRider != (UnityEngine.Object) null))
          return;
        Vector2 a1 = this.attachTransform.position.XY() + (this.m_secondaryRider.transform.position.XY() - this.m_secondaryRider.specRigidbody.UnitBottomCenter + new Vector2(0.125f, 0.25f));
        if ((double) Vector2.Distance(a1, (Vector2) this.m_secondaryRider.transform.position) <= 1.0 / 16.0)
          return;
        this.m_secondaryRider.transform.position = (Vector3) a1;
        this.m_secondaryRider.specRigidbody.Reinitialize();
      }

      protected void AttachRider()
      {
        Vector2 vector = this.m_rider.transform.position.XY() - this.m_rider.specRigidbody.UnitBottomCenter;
        if (this.m_rider is PlayerController)
        {
          Vector2 vector2 = (this.attachTransform.position + vector.ToVector3ZUp()).XY() - this.m_rider.transform.position.XY();
          if (this.m_primaryLerpCoroutine != null)
            this.StopCoroutine(this.m_primaryLerpCoroutine);
          this.m_primaryLerpCoroutine = this.StartCoroutine(this.HandleLerpCameraPlayerPosition(this.m_rider as PlayerController, -vector2));
        }
        this.m_rider.transform.position = this.attachTransform.position + vector.ToVector3ZUp();
        this.m_rider.specRigidbody.Reinitialize();
        this.m_rider.specRigidbody.RegisterSpecificCollisionException(this.specRigidbody);
        this.specRigidbody.RegisterSpecificCollisionException(this.m_rider.specRigidbody);
        this.m_rider.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.LowObstacle));
        if ((bool) (UnityEngine.Object) this.m_rider.knockbackDoer)
          this.m_rider.knockbackDoer.knockbackMultiplier = 0.0f;
        this.m_rider.FallingProhibited = true;
        this.specRigidbody.RegisterCarriedRigidbody(this.m_rider.specRigidbody);
      }

      public float GetDistanceToPoint(Vector2 point)
      {
        return this.occupation == MineCartController.CartOccupationState.ENEMY || this.occupation == MineCartController.CartOccupationState.CARGO ? 1000f : Vector2.Distance(point, this.specRigidbody.UnitCenter) / 2f;
      }

      public void OnEnteredRange(PlayerController interactor)
      {
        if (this.occupation == MineCartController.CartOccupationState.PLAYER && ((UnityEngine.Object) interactor == (UnityEngine.Object) this.m_rider || (UnityEngine.Object) interactor == (UnityEngine.Object) this.m_secondaryRider))
          return;
        SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.white, 1.75f);
        SpriteOutlineManager.AddOutlineToSprite(this.childAnimator.sprite, Color.white);
      }

      public void OnExitRange(PlayerController interactor)
      {
        SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite, true);
        SpriteOutlineManager.RemoveOutlineFromSprite(this.childAnimator.sprite, true);
      }

      public void Interact(PlayerController interactor)
      {
        if (this.occupation == MineCartController.CartOccupationState.ENEMY || this.occupation == MineCartController.CartOccupationState.PLAYER && (UnityEngine.Object) this.m_rider == (UnityEngine.Object) interactor || this.occupation == MineCartController.CartOccupationState.PLAYER && (UnityEngine.Object) this.m_secondaryRider == (UnityEngine.Object) interactor)
          return;
        SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite);
        SpriteOutlineManager.RemoveOutlineFromSprite(this.childAnimator.sprite);
        this.BecomeOccupied(interactor);
      }

      public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
      {
        shouldBeFlipped = false;
        return string.Empty;
      }

      public float GetOverrideMaxDistance() => -1f;

      public enum CartOccupationState
      {
        EMPTY,
        PLAYER,
        ENEMY,
        CARGO,
      }
    }

}
