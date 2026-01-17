// Decompiled with JetBrains decompiler
// Type: InfinilichNegativePlatformingBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullInspector;
using UnityEngine;

#nullable disable

namespace ETG.Core.Actors.Behaviors
{
    [InspectorDropdownName("Bosses/Infinilich/NegativePlatformingBehavior")]
    public class InfinilichNegativePlatformingBehavior : BasicAttackBehavior
    {
      public float SetupTime = 1f;
      public float FlightSpeed = 6f;
      public GameObject ShootPoint;
      public BulletScriptSelector BulletScript;
      private InfinilichNegativePlatformingBehavior.SpinState m_state;
      private Vector2 m_startPoint;
      private Vector2 m_targetPoint;
      private float m_setupTime;
      private float m_setupTimer;
      private BulletScriptSource m_bulletSource;

      private InfinilichNegativePlatformingBehavior.SpinState State
      {
        get => this.m_state;
        set
        {
          if (this.m_state == value)
            return;
          this.EndState(this.m_state);
          this.m_state = value;
          this.BeginState(this.m_state);
        }
      }

      public override BehaviorResult Update()
      {
        BehaviorResult behaviorResult = base.Update();
        if (behaviorResult != BehaviorResult.Continue)
          return behaviorResult;
        if (!this.IsReady() || !(bool) (Object) this.m_aiActor.TargetRigidbody)
          return BehaviorResult.Continue;
        this.State = InfinilichNegativePlatformingBehavior.SpinState.ArmsIn;
        this.m_updateEveryFrame = true;
        return BehaviorResult.RunContinuous;
      }

      public override ContinuousBehaviorResult ContinuousUpdate()
      {
        int num = (int) base.ContinuousUpdate();
        if (this.State == InfinilichNegativePlatformingBehavior.SpinState.ArmsIn)
        {
          if (!this.m_aiAnimator.IsPlaying("arms_in"))
          {
            this.m_startPoint = this.m_aiActor.specRigidbody.UnitCenter;
            this.m_targetPoint = this.m_aiActor.ParentRoom.area.Center + new Vector2(0.0f, -1.5f);
            this.m_setupTime = Mathf.Min(this.SetupTime, 1.5f * (this.m_targetPoint - this.m_startPoint).magnitude / this.FlightSpeed);
            this.State = InfinilichNegativePlatformingBehavior.SpinState.GoToStartPoint;
            return ContinuousBehaviorResult.Continue;
          }
        }
        else if (this.State == InfinilichNegativePlatformingBehavior.SpinState.GoToStartPoint)
        {
          Vector2 unitCenter = this.m_aiActor.specRigidbody.UnitCenter;
          Vector2 vector2 = Vector2Extensions.SmoothStep(this.m_startPoint, this.m_targetPoint, this.m_setupTimer / this.m_setupTime);
          if ((double) this.m_setupTimer > (double) this.m_setupTime)
          {
            this.m_aiActor.BehaviorVelocity = Vector2.zero;
            this.State = InfinilichNegativePlatformingBehavior.SpinState.BulletScript;
            return ContinuousBehaviorResult.Continue;
          }
          this.m_aiActor.BehaviorVelocity = (vector2 - unitCenter) / BraveTime.DeltaTime;
          this.m_setupTimer += this.m_deltaTime;
        }
        else if (this.State == InfinilichNegativePlatformingBehavior.SpinState.BulletScript)
        {
          if (this.m_bulletSource.IsEnded)
          {
            this.State = InfinilichNegativePlatformingBehavior.SpinState.ArmsOut;
            return ContinuousBehaviorResult.Continue;
          }
        }
        else if (this.State == InfinilichNegativePlatformingBehavior.SpinState.ArmsOut && !this.m_aiAnimator.IsPlaying("arms_out"))
          return ContinuousBehaviorResult.Finished;
        return ContinuousBehaviorResult.Continue;
      }

      public override void EndContinuousUpdate()
      {
        base.EndContinuousUpdate();
        this.State = InfinilichNegativePlatformingBehavior.SpinState.None;
        this.m_aiActor.BehaviorOverridesVelocity = false;
        this.m_updateEveryFrame = false;
        this.UpdateCooldowns();
      }

      private void Fire()
      {
        if (!(bool) (Object) this.m_bulletSource)
          this.m_bulletSource = this.ShootPoint.GetOrAddComponent<BulletScriptSource>();
        this.m_bulletSource.BulletManager = this.m_aiActor.bulletBank;
        this.m_bulletSource.BulletScript = this.BulletScript;
        this.m_bulletSource.Initialize();
      }

      private void BeginState(
        InfinilichNegativePlatformingBehavior.SpinState state)
      {
        switch (state)
        {
          case InfinilichNegativePlatformingBehavior.SpinState.ArmsIn:
            this.m_aiActor.ClearPath();
            this.m_aiActor.BehaviorOverridesVelocity = true;
            this.m_aiActor.BehaviorVelocity = Vector2.zero;
            this.m_aiAnimator.PlayUntilCancelled("arms_in");
            break;
          case InfinilichNegativePlatformingBehavior.SpinState.GoToStartPoint:
            this.m_aiAnimator.PlayUntilCancelled("spin");
            this.m_setupTimer = 0.0f;
            break;
          case InfinilichNegativePlatformingBehavior.SpinState.BulletScript:
            this.Fire();
            break;
          case InfinilichNegativePlatformingBehavior.SpinState.ArmsOut:
            this.m_aiAnimator.PlayUntilFinished("arms_out");
            break;
        }
      }

      private void EndState(
        InfinilichNegativePlatformingBehavior.SpinState state)
      {
        if (state != InfinilichNegativePlatformingBehavior.SpinState.BulletScript || !(bool) (Object) this.m_bulletSource || this.m_bulletSource.IsEnded)
          return;
        this.m_bulletSource.ForceStop();
      }

      private enum SpinState
      {
        None,
        ArmsIn,
        GoToStartPoint,
        BulletScript,
        ArmsOut,
      }
    }

}
