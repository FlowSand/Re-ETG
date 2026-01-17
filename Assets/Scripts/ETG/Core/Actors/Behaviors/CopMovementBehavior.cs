// Decompiled with JetBrains decompiler
// Type: CopMovementBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using UnityEngine;

#nullable disable

namespace ETG.Core.Actors.Behaviors
{
    public class CopMovementBehavior : MovementBehaviorBase
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
      private bool m_hasIdled;
      private bool m_isCatchingUp;
      private float m_catchUpTime;
      private int m_sequentialPathFails;
      private float m_repathTimer;

      public override void Upkeep()
      {
        base.Upkeep();
        this.DecrementTimer(ref this.m_repathTimer);
      }

      private void CatchUpMovementModifier(ref Vector2 voluntaryVel, ref Vector2 involuntaryVel)
      {
        this.m_catchUpTime += this.m_aiActor.LocalDeltaTime;
        voluntaryVel = voluntaryVel.normalized * Mathf.Lerp(this.CatchUpSpeed, this.CatchUpMaxSpeed, this.m_catchUpTime / this.CatchUpAccelTime);
      }

      public override BehaviorResult Update()
      {
        this.m_aiActor.DustUpInterval = Mathf.Lerp(0.5f, 0.125f, this.m_aiActor.specRigidbody.Velocity.magnitude / this.CatchUpSpeed);
        PlayerController primaryPlayer = GameManager.Instance.PrimaryPlayer;
        if (!(bool) (Object) primaryPlayer || primaryPlayer.CurrentRoom == null)
        {
          this.m_aiActor.ClearPath();
          return BehaviorResult.Continue;
        }
        if (!primaryPlayer.IsStealthed && primaryPlayer.CurrentRoom.IsSealed && this.m_aiActor.transform.position.GetAbsoluteRoom() == primaryPlayer.CurrentRoom && this.DisableInCombat)
        {
          IntVector2 intVector2 = this.m_aiActor.specRigidbody.UnitCenter.ToIntVector2(VectorConversions.Floor);
          if (GameManager.Instance.Dungeon.data.CheckInBoundsAndValid(intVector2) && !GameManager.Instance.Dungeon.data[intVector2].isExitCell)
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
        float num = Vector2.Distance(primaryPlayer.CenterPosition, this.m_aiActor.CenterPosition);
        if ((double) num <= (double) this.IdealRadius)
        {
          this.m_aiActor.ClearPath();
          if (this.m_isCatchingUp)
          {
            this.m_isCatchingUp = false;
            if (!string.IsNullOrEmpty(this.CatchUpOutAnimation))
              this.m_aiAnimator.PlayUntilFinished(this.CatchUpOutAnimation);
            this.m_aiActor.MovementModifiers -= new GameActor.MovementModifier(this.CatchUpMovementModifier);
          }
          if (!this.m_hasIdled && !this.m_aiAnimator.IsPlaying(this.CatchUpOutAnimation) && this.IdleAnimations.Length > 0)
          {
            this.m_hasIdled = true;
            this.m_aiAnimator.PlayUntilCancelled(this.IdleAnimations[Random.Range(0, this.IdleAnimations.Length)]);
          }
          return BehaviorResult.SkipRemainingClassBehaviors;
        }
        if ((double) num > 30.0)
        {
          this.m_sequentialPathFails = 0;
          this.m_aiActor.CompanionWarp((Vector3) this.m_aiActor.CompanionOwner.CenterPosition);
        }
        else
        {
          this.m_hasIdled = false;
          if (!this.m_isCatchingUp && (double) num > (double) this.CatchUpRadius)
          {
            this.m_isCatchingUp = true;
            this.m_catchUpTime = 0.0f;
            if (!string.IsNullOrEmpty(this.CatchUpAnimation))
              this.m_aiAnimator.PlayUntilFinished(this.CatchUpAnimation);
            else
              this.m_aiAnimator.EndAnimation();
            this.m_aiActor.MovementModifiers += new GameActor.MovementModifier(this.CatchUpMovementModifier);
          }
          else if (!this.m_isCatchingUp && (double) num < (double) this.CatchUpRadius)
            this.m_aiAnimator.EndAnimation();
          if ((double) this.m_repathTimer <= 0.0 && (bool) (Object) primaryPlayer && (bool) (Object) primaryPlayer.specRigidbody && !primaryPlayer.IsInMinecart)
          {
            this.m_repathTimer = this.PathInterval;
            this.m_aiActor.PathfindToPosition(primaryPlayer.specRigidbody.UnitCenter);
            if (this.m_aiActor.Path != null && (double) this.m_aiActor.Path.InaccurateLength > 50.0)
            {
              this.m_aiActor.ClearPath();
              this.m_sequentialPathFails = 0;
              this.m_aiActor.CompanionWarp((Vector3) this.m_aiActor.CompanionOwner.CenterPosition);
            }
            else if (this.m_aiActor.Path != null && !this.m_aiActor.Path.WillReachFinalGoal)
            {
              ++this.m_sequentialPathFails;
              CellData cellData = GameManager.Instance.Dungeon.data[this.m_aiActor.CompanionOwner.CenterPosition.ToIntVector2(VectorConversions.Floor)];
              if (this.m_sequentialPathFails > 3 && cellData != null && cellData.IsPassable)
              {
                this.m_sequentialPathFails = 0;
                this.m_aiActor.CompanionWarp((Vector3) this.m_aiActor.CompanionOwner.CenterPosition);
              }
            }
            else
              this.m_sequentialPathFails = 0;
          }
        }
        return BehaviorResult.SkipRemainingClassBehaviors;
      }
    }

}
