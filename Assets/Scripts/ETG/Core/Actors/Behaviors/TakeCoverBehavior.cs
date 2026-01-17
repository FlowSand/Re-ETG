// Decompiled with JetBrains decompiler
// Type: TakeCoverBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Actors.Behaviors
{
    public class TakeCoverBehavior : MovementBehaviorBase
    {
      protected static FlippableCover[] allCover;
      protected static HashSet<FlippableCover> ClaimedCover = new HashSet<FlippableCover>();
      public float PathInterval = 0.25f;
      public bool LineOfSightToLeaveCover = true;
      public float MaxCoverDistance = 10f;
      public float MaxCoverDistanceToTarget = 10f;
      public float FlipCoverDistance = 1f;
      public float InsideCoverTime = 2f;
      public float OutsideCoverTime = 1f;
      public float PopOutSpeedMultiplier = 1f;
      public float PopInSpeedMultiplier = 1f;
      public float InitialCoverChance = 0.33f;
      public float RepeatingCoverChance = 0.05f;
      public float RepeatingCoverInterval = 1f;
      private TakeCoverBehavior.CoverState m_state;
      private int m_tableQuadrant;
      private float m_repathTimer;
      private float m_coverTimer;
      private float m_seekTimer;
      private float m_failedLineOfSightTimer;
      private float m_cachedSpeed;
      private FlippableCover m_claimedCover;
      private Vector2 m_coverPosition;
      private Vector2 m_popOutPosition;
      private string[] coverAnimations = new string[4]
      {
        "cover_idle_right",
        "cover_idle_right",
        "cover_idle_left",
        "cover_idle_left"
      };
      private string[] emergeAnimations = new string[4]
      {
        "cover_leap_right",
        "cover_leap_right",
        "cover_leap_left",
        "cover_leap_left"
      };

      public static void ClearPerLevelData()
      {
        TakeCoverBehavior.allCover = (FlippableCover[]) null;
        TakeCoverBehavior.ClaimedCover.Clear();
      }

      private bool LastEnemyAndCantSeePlayer
      {
        get
        {
          return this.m_aiActor.ParentRoom.GetActiveEnemiesCount(RoomHandler.ActiveEnemyType.All) == 1 && (double) this.m_failedLineOfSightTimer > 1.0;
        }
      }

      public override void Start()
      {
        if (TakeCoverBehavior.allCover == null || TakeCoverBehavior.allCover.Length == 0)
          TakeCoverBehavior.allCover = Object.FindObjectsOfType<FlippableCover>();
        this.m_cachedSpeed = this.m_aiActor.MovementSpeed;
        this.m_state = TakeCoverBehavior.CoverState.Disinterested;
        if ((double) Random.value >= (double) this.InitialCoverChance)
          return;
        this.SearchForCover();
        this.m_seekTimer = this.RepeatingCoverInterval;
      }

      public override void Upkeep()
      {
        base.Upkeep();
        this.DecrementTimer(ref this.m_repathTimer);
        this.DecrementTimer(ref this.m_coverTimer);
        this.DecrementTimer(ref this.m_seekTimer);
      }

      public override void Destroy()
      {
        base.Destroy();
        if (!((Object) this.m_claimedCover != (Object) null))
          return;
        TakeCoverBehavior.ClaimedCover.Remove(this.m_claimedCover);
      }

      public override BehaviorResult Update()
      {
        if ((Object) this.m_aiActor.TargetRigidbody == (Object) null)
          return BehaviorResult.Continue;
        this.m_aiShooter.OverrideAimPoint = new Vector2?();
        bool flag1 = this.m_aiActor.CanTargetEnemies && !this.m_aiActor.CanTargetPlayers;
        if (this.m_state == TakeCoverBehavior.CoverState.Disinterested)
        {
          if (flag1 || this.LastEnemyAndCantSeePlayer || (double) this.m_seekTimer != 0.0)
            return BehaviorResult.Continue;
          this.m_seekTimer = this.RepeatingCoverInterval;
          if ((double) Random.value < (double) this.RepeatingCoverChance)
          {
            this.SearchForCover();
            if ((Object) this.m_claimedCover != (Object) null)
              return BehaviorResult.SkipRemainingClassBehaviors;
          }
          return BehaviorResult.Continue;
        }
        bool flag2 = !(bool) (Object) this.m_claimedCover || this.m_claimedCover.IsBroken;
        int tableQuadrant = this.m_tableQuadrant;
        Vector2 unitCenter1 = this.m_aiActor.specRigidbody.UnitCenter;
        Vector2 unitCenter2 = this.m_aiActor.TargetRigidbody.GetUnitCenter(ColliderType.HitBox);
        Vector2? nullable = !flag2 ? this.CalculateCoverPosition(unitCenter2) : new Vector2?();
        bool flag3 = false;
        if ((bool) (Object) this.m_claimedCover)
          flag3 = (double) Vector2.Distance(this.m_claimedCover.specRigidbody.UnitCenter, unitCenter2) >= (double) this.MaxCoverDistanceToTarget;
        bool flag4 = this.m_state == TakeCoverBehavior.CoverState.InCover && (double) this.m_coverTimer <= 0.0 && this.LastEnemyAndCantSeePlayer;
        if (flag2 || !nullable.HasValue || flag1 || (double) this.m_aiActor.aiAnimator.FpsScale < 1.0 || flag3 || flag4)
        {
          this.BecomeDisinterested(tableQuadrant);
          return BehaviorResult.Continue;
        }
        if (this.m_state != TakeCoverBehavior.CoverState.PopOut)
          this.m_coverPosition = nullable.Value;
        if ((bool) (Object) this.m_claimedCover && !this.m_claimedCover.IsFlipped && (double) Vector2.Distance(this.m_coverPosition, unitCenter1) < (double) this.FlipCoverDistance && this.DesiredFlipDirection == this.m_claimedCover.GetFlipDirection(this.m_aiActor.specRigidbody))
          this.m_claimedCover.Flip(this.m_aiActor.specRigidbody);
        if (this.m_state == TakeCoverBehavior.CoverState.MovingToCover)
        {
          if ((double) this.m_repathTimer == 0.0)
          {
            if ((double) Vector2.Distance(unitCenter1, this.m_coverPosition) > (double) PhysicsEngine.PixelToUnit(2) && !this.m_aiActor.PathfindToPosition(this.m_coverPosition, new Vector2?(this.m_coverPosition)))
            {
              this.BecomeDisinterested(tableQuadrant);
              this.m_aiActor.ClearPath();
              return BehaviorResult.Continue;
            }
            this.m_repathTimer = this.PathInterval;
          }
          if (this.m_aiActor.PathComplete)
          {
            this.m_state = TakeCoverBehavior.CoverState.InCover;
            this.m_coverTimer = this.InsideCoverTime;
            this.m_repathTimer = 0.0f;
            this.m_failedLineOfSightTimer = 0.0f;
          }
        }
        else if (this.m_state == TakeCoverBehavior.CoverState.InCover)
        {
          if (!this.m_aiActor.HasLineOfSightToTarget)
            this.m_aiShooter.OverrideAimDirection = new Vector2?(IntVector2.Cardinals[this.m_tableQuadrant / 2 * 2 + 1].ToVector2());
          if ((double) this.m_coverTimer == 0.0)
          {
            this.m_popOutPosition = this.CalculatePopOutPosition(unitCenter2);
            if (!this.LineOfSightToLeaveCover || this.m_aiActor.HasLineOfSightToTargetFromPosition(this.m_popOutPosition))
            {
              this.m_state = TakeCoverBehavior.CoverState.PopOut;
              this.m_coverTimer = this.OutsideCoverTime;
              this.m_repathTimer = 0.0f;
              this.m_aiActor.MovementSpeed = this.m_cachedSpeed * this.PopOutSpeedMultiplier;
              this.m_aiAnimator.PlayForDuration(this.emergeAnimations[this.m_tableQuadrant], this.OutsideCoverTime);
              return BehaviorResult.SkipRemainingClassBehaviors;
            }
            if (this.LineOfSightToLeaveCover)
              this.m_failedLineOfSightTimer += this.m_deltaTime;
          }
          if ((double) this.m_repathTimer == 0.0)
          {
            if ((double) Vector2.Distance(unitCenter1, this.m_coverPosition) > (double) PhysicsEngine.PixelToUnit(2))
            {
              bool position = this.m_aiActor.PathfindToPosition(this.m_coverPosition, new Vector2?(this.m_coverPosition));
              this.m_aiAnimator.EndAnimationIf(this.coverAnimations[tableQuadrant]);
              if (!position)
              {
                this.BecomeDisinterested(tableQuadrant);
                this.m_aiActor.ClearPath();
                return BehaviorResult.Continue;
              }
            }
            this.m_repathTimer = this.PathInterval;
          }
          if (this.m_aiActor.PathComplete && !this.m_aiActor.spriteAnimator.IsPlaying(this.coverAnimations[this.m_tableQuadrant]))
            this.m_aiAnimator.PlayUntilFinished(this.coverAnimations[this.m_tableQuadrant]);
        }
        else if (this.m_state == TakeCoverBehavior.CoverState.PopOut)
        {
          if ((double) this.m_coverTimer == 0.0)
          {
            this.m_state = TakeCoverBehavior.CoverState.InCover;
            this.m_coverTimer = this.InsideCoverTime;
            this.m_repathTimer = 0.0f;
            this.m_failedLineOfSightTimer = 0.0f;
            this.m_aiActor.MovementSpeed = this.m_cachedSpeed * this.PopInSpeedMultiplier;
          }
          else if ((double) this.m_repathTimer == 0.0)
          {
            if ((double) Vector2.Distance(this.m_aiActor.specRigidbody.UnitCenter, this.m_popOutPosition) < 2.0)
              this.m_aiActor.FakePathToPosition(this.m_popOutPosition);
            else if (!this.m_aiActor.PathfindToPosition(this.m_popOutPosition, new Vector2?(this.m_popOutPosition)))
            {
              this.BecomeDisinterested(tableQuadrant);
              this.m_aiActor.ClearPath();
              return BehaviorResult.Continue;
            }
            this.m_repathTimer = this.PathInterval;
          }
        }
        return BehaviorResult.SkipRemainingClassBehaviors;
      }

      private DungeonData.Direction DesiredFlipDirection
      {
        get
        {
          if (this.m_tableQuadrant == 0)
            return DungeonData.Direction.SOUTH;
          if (this.m_tableQuadrant == 1)
            return DungeonData.Direction.WEST;
          if (this.m_tableQuadrant == 2)
            return DungeonData.Direction.NORTH;
          if (this.m_tableQuadrant == 3)
            return DungeonData.Direction.EAST;
          Debug.LogError((object) "Unknown flip direction!");
          return DungeonData.Direction.NORTH;
        }
      }

      protected void SearchForCover()
      {
        if ((Object) this.m_claimedCover != (Object) null)
          TakeCoverBehavior.ClaimedCover.Remove(this.m_claimedCover);
        this.m_claimedCover = (FlippableCover) null;
        if (!(bool) (Object) this.m_aiActor.TargetRigidbody)
          return;
        RoomHandler roomFromPosition1 = GameManager.Instance.Dungeon.GetRoomFromPosition(this.m_aiActor.specRigidbody.UnitCenter.ToIntVector2(VectorConversions.Floor));
        Vector2 unitCenter = this.m_aiActor.TargetRigidbody.GetUnitCenter(ColliderType.HitBox);
        float num1 = float.MaxValue;
        for (int index = 0; index < TakeCoverBehavior.allCover.Length; ++index)
        {
          if ((bool) (Object) TakeCoverBehavior.allCover[index] && !TakeCoverBehavior.allCover[index].IsBroken && !TakeCoverBehavior.ClaimedCover.Contains(TakeCoverBehavior.allCover[index]))
          {
            RoomHandler roomFromPosition2 = GameManager.Instance.Dungeon.GetRoomFromPosition(TakeCoverBehavior.allCover[index].transform.position.IntXY(VectorConversions.Floor));
            if (roomFromPosition1 == roomFromPosition2)
            {
              float num2 = Vector2.Distance(this.m_aiActor.specRigidbody.UnitCenter, TakeCoverBehavior.allCover[index].specRigidbody.UnitCenter);
              float num3 = Vector2.Distance(TakeCoverBehavior.allCover[index].specRigidbody.UnitCenter, unitCenter);
              if ((double) num2 < (double) this.MaxCoverDistance && (double) num2 < (double) num1 && (double) num3 < (double) this.MaxCoverDistanceToTarget)
              {
                num1 = num2;
                this.m_claimedCover = TakeCoverBehavior.allCover[index];
              }
            }
          }
        }
        if (!((Object) this.m_claimedCover != (Object) null))
          return;
        TakeCoverBehavior.ClaimedCover.Add(this.m_claimedCover);
        this.m_repathTimer = 0.0f;
        this.m_state = TakeCoverBehavior.CoverState.MovingToCover;
      }

      protected Vector2? CalculateCoverPosition(Vector2 targetPosition)
      {
        Vector2? coverPosition = new Vector2?();
        PixelCollider primaryPixelCollider = this.m_aiActor.specRigidbody.PrimaryPixelCollider;
        PixelCollider pixelCollider1 = this.m_aiActor.specRigidbody[CollisionLayer.EnemyHitBox];
        PixelCollider pixelCollider2 = !this.m_claimedCover.IsFlipped ? this.m_claimedCover.specRigidbody.PrimaryPixelCollider : this.m_claimedCover.specRigidbody[CollisionLayer.LowObstacle];
        Vector2 unitCenter = pixelCollider2.UnitCenter;
        Vector2 vector = targetPosition - unitCenter;
        Vector2 vector2_1 = BraveUtility.GetMajorAxis(vector);
        for (int index = 0; index < 2; ++index)
        {
          Vector2 vector2_2;
          if ((double) vector2_1.x != 0.0)
          {
            if (pixelCollider2.Height >= pixelCollider1.Height)
            {
              vector2_2 = new Vector2((double) vector2_1.x <= 0.0 ? Mathf.Ceil(pixelCollider2.UnitRight) : Mathf.Floor(pixelCollider2.UnitLeft), pixelCollider2.UnitCenter.y);
              this.m_tableQuadrant = (double) vector2_1.x <= 0.0 ? 1 : 3;
            }
            else
              goto label_7;
          }
          else if (pixelCollider2.Width >= pixelCollider1.Width)
          {
            vector2_2 = new Vector2(pixelCollider2.UnitCenter.x, (double) vector2_1.y <= 0.0 ? Mathf.Ceil(pixelCollider2.UnitTop) : Mathf.Floor(pixelCollider2.UnitBottom));
            this.m_tableQuadrant = (double) vector2_1.y <= 0.0 ? 0 : 2;
          }
          else
            goto label_7;
          coverPosition = new Vector2?(vector2_2 + Vector2.Scale(-vector2_1, primaryPixelCollider.UnitDimensions / 2f));
          break;
    label_7:
          vector2_1 = BraveUtility.GetMinorAxis(vector);
        }
        if (coverPosition.HasValue)
          return coverPosition;
        Debug.LogError((object) "Didn't find a valid cover position!");
        return new Vector2?(this.m_claimedCover.transform.position.XY());
      }

      protected Vector2 CalculatePopOutPosition(Vector2 targetPosition)
      {
        PixelCollider primaryPixelCollider = this.m_aiActor.specRigidbody.PrimaryPixelCollider;
        PixelCollider hitboxPixelCollider = this.m_aiActor.specRigidbody.HitboxPixelCollider;
        PixelCollider pixelCollider = this.m_claimedCover.specRigidbody[CollisionLayer.BulletBlocker];
        Vector2 coverPosition = this.m_coverPosition;
        Vector2 vector2_1 = targetPosition - pixelCollider.UnitCenter;
        Vector2 vector2_2 = primaryPixelCollider.UnitDimensions / 2f;
        if (this.m_tableQuadrant == 0 || this.m_tableQuadrant == 2)
          coverPosition.x = (double) vector2_1.x >= 0.0 ? pixelCollider.UnitRight + vector2_2.x : pixelCollider.UnitLeft - vector2_2.x;
        else
          coverPosition.y = (double) vector2_1.y >= 0.0 ? pixelCollider.UnitTop + vector2_2.y : pixelCollider.UnitBottom - hitboxPixelCollider.UnitDimensions.y + vector2_2.y;
        return coverPosition;
      }

      private void BecomeDisinterested(int previousTableQuadrant)
      {
        this.m_state = TakeCoverBehavior.CoverState.Disinterested;
        this.m_seekTimer = this.RepeatingCoverInterval;
        this.m_aiShooter.OverrideAimPoint = new Vector2?();
        this.m_aiActor.MovementSpeed = this.m_cachedSpeed;
        this.m_aiAnimator.EndAnimationIf(this.coverAnimations[previousTableQuadrant]);
        this.m_aiAnimator.EndAnimationIf(this.emergeAnimations[previousTableQuadrant]);
      }

      private enum CoverState
      {
        Disinterested,
        MovingToCover,
        InCover,
        PopOut,
      }
    }

}
