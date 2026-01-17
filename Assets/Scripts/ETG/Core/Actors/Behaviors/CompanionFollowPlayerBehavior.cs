// Decompiled with JetBrains decompiler
// Type: CompanionFollowPlayerBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using Pathfinding;
using System;
using UnityEngine;

#nullable disable

namespace ETG.Core.Actors.Behaviors
{
    public class CompanionFollowPlayerBehavior : MovementBehaviorBase
    {
      public float PathInterval = 0.25f;
      public bool DisableInCombat = true;
      public float IdealRadius = 3f;
      public float CatchUpRadius = 7f;
      public float CatchUpAccelTime = 5f;
      public float CatchUpSpeed = 7f;
      public float CatchUpMaxSpeed = 10f;
      public string CatchUpAnimation;
      public string CatchUpOutAnimation;
      public string[] IdleAnimations;
      public bool CanRollOverPits;
      public string RollAnimation = "roll";
      private bool m_isCatchingUp;
      private float m_catchUpTime;
      [NonSerialized]
      public bool TemporarilyDisabled;
      protected bool m_triedToPathOverPit;
      protected bool m_wasOverPit;
      protected bool m_groundRolling;
      private int m_sequentialPathFails;
      private float m_idleTimer = 2f;
      private float m_repathTimer;
      private CompanionController m_companionController;

      public override void Start()
      {
        base.Start();
        this.m_companionController = this.m_gameObject.GetComponent<CompanionController>();
      }

      public override void Upkeep()
      {
        base.Upkeep();
        this.DecrementTimer(ref this.m_repathTimer);
      }

      private void CatchUpMovementModifier(ref Vector2 voluntaryVel, ref Vector2 involuntaryVel)
      {
        if (this.DisableInCombat)
        {
          PlayerController playerController = GameManager.Instance.PrimaryPlayer;
          if ((bool) (UnityEngine.Object) this.m_aiActor && (bool) (UnityEngine.Object) this.m_aiActor.CompanionOwner)
            playerController = this.m_aiActor.CompanionOwner;
          if ((bool) (UnityEngine.Object) playerController && playerController.IsInCombat && (double) Vector2.Distance(playerController.CenterPosition, this.m_aiActor.CenterPosition) < (double) this.CatchUpRadius)
          {
            this.m_isCatchingUp = false;
            if (!string.IsNullOrEmpty(this.CatchUpOutAnimation))
              this.m_aiAnimator.PlayUntilFinished(this.CatchUpOutAnimation);
            this.m_aiActor.MovementModifiers -= new GameActor.MovementModifier(this.CatchUpMovementModifier);
            return;
          }
        }
        this.m_catchUpTime += this.m_aiActor.LocalDeltaTime;
        voluntaryVel = voluntaryVel.normalized * Mathf.Lerp(this.CatchUpSpeed, this.CatchUpMaxSpeed, this.m_catchUpTime / this.CatchUpAccelTime);
      }

      public override ContinuousBehaviorResult ContinuousUpdate()
      {
        if (!this.m_aiAnimator.IsPlaying(this.RollAnimation))
          return ContinuousBehaviorResult.Finished;
        if ((double) this.m_aiAnimator.CurrentClipProgress > 0.699999988079071)
        {
          this.m_aiActor.FallingProhibited = false;
          this.m_aiActor.BehaviorVelocity = this.m_aiActor.BehaviorVelocity.normalized * 2f;
        }
        return base.ContinuousUpdate();
      }

      public override void EndContinuousUpdate()
      {
        this.m_updateEveryFrame = false;
        this.m_triedToPathOverPit = false;
        this.m_groundRolling = false;
        this.m_aiActor.FallingProhibited = false;
        this.m_aiActor.BehaviorOverridesVelocity = false;
        base.EndContinuousUpdate();
      }

      public override BehaviorResult Update()
      {
        if (!GameManager.HasInstance || GameManager.Instance.IsLoadingLevel)
          return BehaviorResult.SkipAllRemainingBehaviors;
        if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.END_TIMES)
        {
          this.m_aiActor.ClearPath();
          return BehaviorResult.SkipAllRemainingBehaviors;
        }
        if (this.TemporarilyDisabled)
          return BehaviorResult.Continue;
        this.DecrementTimer(ref this.m_idleTimer);
        this.m_aiActor.DustUpInterval = Mathf.Lerp(0.5f, 0.125f, this.m_aiActor.specRigidbody.Velocity.magnitude / this.CatchUpSpeed);
        PlayerController playerController = GameManager.Instance.PrimaryPlayer;
        if ((bool) (UnityEngine.Object) this.m_aiActor && (bool) (UnityEngine.Object) this.m_aiActor.CompanionOwner)
          playerController = this.m_aiActor.CompanionOwner;
        if (this.CanRollOverPits && this.m_triedToPathOverPit)
        {
          if (this.m_aiActor.IsOverPitAtAll && !this.m_wasOverPit)
          {
            Debug.Log((object) "running continuous");
            this.m_aiActor.FallingProhibited = true;
            this.m_aiAnimator.PlayUntilFinished(this.RollAnimation);
            Vector2 normalized = this.m_aiActor.specRigidbody.Velocity.normalized;
            this.m_aiActor.BehaviorOverridesVelocity = true;
            this.m_aiActor.BehaviorVelocity = normalized * 7f;
            this.m_aiActor.ClearPath();
            this.m_updateEveryFrame = true;
            return BehaviorResult.RunContinuous;
          }
          this.m_wasOverPit = this.m_aiActor.IsOverPitAtAll;
        }
        IntVector2 intVector2_1 = this.m_aiActor.specRigidbody.UnitCenter.ToIntVector2(VectorConversions.Floor);
        CellData cellData1 = GameManager.Instance.Dungeon.data[intVector2_1];
        if (cellData1 != null && cellData1.IsPlayerInaccessible)
        {
          if ((double) this.m_repathTimer <= 0.0)
          {
            this.m_repathTimer = this.PathInterval;
            RoomHandler roomFromPosition = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(intVector2_1);
            if (roomFromPosition != null)
            {
              IntVector2? nearestAvailableCell = roomFromPosition.GetNearestAvailableCell(intVector2_1.ToCenterVector2(), new IntVector2?(this.m_aiActor.Clearance), new CellTypes?(this.m_aiActor.PathableTiles), cellValidator: (CellValidator) (pos => !GameManager.Instance.Dungeon.data[pos].IsPlayerInaccessible));
              if (nearestAvailableCell.HasValue)
                this.m_aiActor.PathfindToPosition(nearestAvailableCell.Value.ToCenterVector2());
            }
          }
          return BehaviorResult.SkipRemainingClassBehaviors;
        }
        if (!(bool) (UnityEngine.Object) playerController)
          return BehaviorResult.Continue;
        if (!playerController.IsStealthed && playerController.CurrentRoom != null && playerController.CurrentRoom.HasActiveEnemies(RoomHandler.ActiveEnemyType.All) && (bool) (UnityEngine.Object) this.m_aiActor.TargetRigidbody && this.m_aiActor.transform.position.GetAbsoluteRoom() == playerController.CurrentRoom && this.DisableInCombat)
        {
          IntVector2 intVector2_2 = !(bool) (UnityEngine.Object) this.m_aiActor.specRigidbody ? this.m_aiActor.transform.position.IntXY(VectorConversions.Floor) : this.m_aiActor.specRigidbody.UnitCenter.ToIntVector2(VectorConversions.Floor);
          if (GameManager.Instance.Dungeon.data.CheckInBoundsAndValid(intVector2_2) && !GameManager.Instance.Dungeon.data[intVector2_2].isExitCell)
          {
            if (this.m_isCatchingUp)
            {
              this.m_isCatchingUp = false;
              if (!string.IsNullOrEmpty(this.CatchUpOutAnimation))
                this.m_aiAnimator.PlayUntilFinished(this.CatchUpOutAnimation);
              this.m_aiActor.MovementModifiers -= new GameActor.MovementModifier(this.CatchUpMovementModifier);
            }
            return BehaviorResult.Continue;
          }
        }
        bool flag1 = false;
        if ((bool) (UnityEngine.Object) this.m_companionController && this.m_companionController.IsBeingPet)
          flag1 = true;
        float num = Vector2.Distance(playerController.CenterPosition, this.m_aiActor.CenterPosition);
        if ((double) num <= (double) this.IdealRadius && !flag1)
        {
          this.m_aiActor.ClearPath();
          if (this.m_isCatchingUp)
          {
            this.m_isCatchingUp = false;
            if (!string.IsNullOrEmpty(this.CatchUpOutAnimation))
              this.m_aiAnimator.PlayUntilFinished(this.CatchUpOutAnimation);
            this.m_aiActor.MovementModifiers -= new GameActor.MovementModifier(this.CatchUpMovementModifier);
          }
          if ((double) this.m_idleTimer <= 0.0 && this.IdleAnimations != null && this.IdleAnimations.Length > 0)
          {
            this.m_aiAnimator.PlayUntilFinished(this.IdleAnimations[UnityEngine.Random.Range(0, this.IdleAnimations.Length)]);
            this.m_idleTimer = (float) UnityEngine.Random.Range(3, 10);
          }
          return BehaviorResult.SkipRemainingClassBehaviors;
        }
        if ((double) num > 30.0)
        {
          this.m_sequentialPathFails = 0;
          this.m_aiActor.CompanionWarp((Vector3) this.m_aiActor.CompanionOwner.CenterPosition);
        }
        else if (!this.m_isCatchingUp && (double) num > (double) this.CatchUpRadius)
        {
          this.m_isCatchingUp = true;
          this.m_catchUpTime = 0.0f;
          if (!string.IsNullOrEmpty(this.CatchUpAnimation))
            this.m_aiAnimator.PlayUntilFinished(this.CatchUpAnimation);
          this.m_aiActor.MovementModifiers += new GameActor.MovementModifier(this.CatchUpMovementModifier);
        }
        this.m_idleTimer = Mathf.Max(this.m_idleTimer, 2f);
        if ((double) this.m_repathTimer <= 0.0 && !playerController.IsOverPitAtAll && !playerController.IsInMinecart)
        {
          this.m_repathTimer = this.PathInterval;
          this.m_triedToPathOverPit = false;
          this.m_aiActor.FallingProhibited = false;
          if (flag1)
          {
            Vector2 vector2 = this.m_companionController.m_pettingDoer.specRigidbody.UnitCenter + this.m_companionController.m_petOffset;
            if ((double) Vector2.Distance(vector2, this.m_aiActor.specRigidbody.UnitCenter) < 0.079999998211860657)
              this.m_aiActor.ClearPath();
            else
              this.m_aiActor.PathfindToPosition(vector2, new Vector2?(vector2));
          }
          else
            this.m_aiActor.PathfindToPosition(playerController.specRigidbody.UnitCenter);
          if (this.m_aiActor.Path != null && (double) this.m_aiActor.Path.InaccurateLength > 50.0)
          {
            this.m_aiActor.ClearPath();
            this.m_sequentialPathFails = 0;
            this.m_aiActor.CompanionWarp((Vector3) this.m_aiActor.CompanionOwner.CenterPosition);
          }
          else if (this.m_aiActor.Path != null && !this.m_aiActor.Path.WillReachFinalGoal)
          {
            bool flag2 = false;
            if (this.CanRollOverPits)
            {
              this.m_aiActor.PathableTiles |= CellTypes.PIT;
              this.m_aiActor.PathfindToPosition(playerController.specRigidbody.UnitCenter);
              this.m_aiActor.PathableTiles &= ~CellTypes.PIT;
              if (this.m_aiActor.Path != null && this.m_aiActor.Path.WillReachFinalGoal)
              {
                this.m_triedToPathOverPit = true;
                this.m_aiActor.FallingProhibited = true;
                flag2 = true;
              }
            }
            if (!flag2)
            {
              ++this.m_sequentialPathFails;
              CellData cellData2 = GameManager.Instance.Dungeon.data[this.m_aiActor.CompanionOwner.CenterPosition.ToIntVector2(VectorConversions.Floor)];
              if (this.m_sequentialPathFails > 3 && cellData2 != null && cellData2.IsPassable)
              {
                this.m_sequentialPathFails = 0;
                this.m_aiActor.CompanionWarp((Vector3) this.m_aiActor.CompanionOwner.CenterPosition);
              }
            }
          }
          else
            this.m_sequentialPathFails = 0;
        }
        if (!(bool) (UnityEngine.Object) this.m_aiShooter || !(bool) (UnityEngine.Object) this.m_aiShooter.EquippedGun)
          ;
        return BehaviorResult.SkipRemainingClassBehaviors;
      }
    }

}
