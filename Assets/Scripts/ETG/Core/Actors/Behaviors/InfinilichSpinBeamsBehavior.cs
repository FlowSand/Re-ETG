using FullInspector;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

[InspectorDropdownName("Bosses/Infinilich/SpinBeamsBehavior")]
public class InfinilichSpinBeamsBehavior : BasicAttackBehavior
  {
    public float SetupTime = 1f;
    public float FlightSpeed = 6f;
    public GameObject ShootPoint;
    public BulletScriptSelector BulletScript;
    private InfinilichSpinBeamsBehavior.SpinState m_state;
    private Vector2 m_startPoint;
    private Vector2 m_targetPoint;
    private List<Vector2> m_futureTargets = new List<Vector2>();
    private float m_setupTime;
    private float m_setupTimer;
    private BulletScriptSource m_bulletSource;

    private InfinilichSpinBeamsBehavior.SpinState State
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
      this.State = InfinilichSpinBeamsBehavior.SpinState.ArmsIn;
      this.m_updateEveryFrame = true;
      return BehaviorResult.RunContinuous;
    }

    public override ContinuousBehaviorResult ContinuousUpdate()
    {
      int num1 = (int) base.ContinuousUpdate();
      if (this.State == InfinilichSpinBeamsBehavior.SpinState.ArmsIn)
      {
        if (!this.m_aiAnimator.IsPlaying("arms_in"))
        {
          this.m_startPoint = this.m_aiActor.specRigidbody.UnitCenter;
          this.m_targetPoint = this.m_aiActor.ParentRoom.area.Center + new Vector2(0.0f, 11f);
          Vector2 vector = this.m_targetPoint - this.m_startPoint;
          this.m_setupTime = Mathf.Min(this.SetupTime, 1.5f * vector.magnitude / this.FlightSpeed);
          this.m_aiAnimator.FacingDirection = vector.ToAngle();
          this.State = InfinilichSpinBeamsBehavior.SpinState.GoToStartPoint;
          return ContinuousBehaviorResult.Continue;
        }
      }
      else if (this.State == InfinilichSpinBeamsBehavior.SpinState.GoToStartPoint)
      {
        Vector2 unitCenter = this.m_aiActor.specRigidbody.UnitCenter;
        Vector2 vector2 = Vector2Extensions.SmoothStep(this.m_startPoint, this.m_targetPoint, this.m_setupTimer / this.m_setupTime);
        if ((double) this.m_setupTimer > (double) this.m_setupTime)
        {
          this.m_aiActor.BehaviorVelocity = Vector2.zero;
          this.m_targetPoint = this.m_aiActor.ParentRoom.area.Center - new Vector2(0.0f, 11f);
          this.m_futureTargets.Clear();
          this.m_futureTargets.Add(this.m_aiActor.ParentRoom.area.Center + new Vector2(0.0f, 11f));
          this.m_futureTargets.Add(this.m_aiActor.ParentRoom.area.Center);
          this.State = InfinilichSpinBeamsBehavior.SpinState.BeamMode;
          return ContinuousBehaviorResult.Continue;
        }
        this.m_aiActor.BehaviorVelocity = (vector2 - unitCenter) / BraveTime.DeltaTime;
        this.m_aiAnimator.FacingDirection = this.m_aiActor.BehaviorVelocity.ToAngle();
        this.m_setupTimer += this.m_deltaTime;
      }
      else if (this.State == InfinilichSpinBeamsBehavior.SpinState.BeamMode)
      {
        Vector2 vector = this.m_targetPoint - this.m_aiActor.specRigidbody.UnitCenter;
        float magnitude = vector.magnitude;
        if ((double) magnitude < 0.10000000149011612)
        {
          if (this.m_futureTargets.Count > 0)
          {
            this.m_targetPoint = this.m_futureTargets[0];
            this.m_futureTargets.RemoveAt(0);
            this.m_aiActor.BehaviorVelocity = Vector2.zero;
            return ContinuousBehaviorResult.Continue;
          }
          this.m_aiActor.BehaviorVelocity = Vector2.zero;
          this.State = InfinilichSpinBeamsBehavior.SpinState.ArmsOut;
          return ContinuousBehaviorResult.Continue;
        }
        float num2 = this.FlightSpeed;
        if ((double) magnitude < (double) this.FlightSpeed * (double) this.m_deltaTime)
          num2 = magnitude / this.m_deltaTime;
        this.m_aiActor.BehaviorVelocity = vector.WithX(0.0f).normalized * num2;
      }
      else if (this.State == InfinilichSpinBeamsBehavior.SpinState.ArmsOut && !this.m_aiAnimator.IsPlaying("arms_out"))
        return ContinuousBehaviorResult.Finished;
      return ContinuousBehaviorResult.Continue;
    }

    public override void EndContinuousUpdate()
    {
      base.EndContinuousUpdate();
      this.State = InfinilichSpinBeamsBehavior.SpinState.None;
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

    private void BeginState(InfinilichSpinBeamsBehavior.SpinState state)
    {
      switch (state)
      {
        case InfinilichSpinBeamsBehavior.SpinState.ArmsIn:
          this.m_aiActor.ClearPath();
          this.m_aiActor.BehaviorOverridesVelocity = true;
          this.m_aiActor.BehaviorVelocity = Vector2.zero;
          this.m_aiAnimator.PlayUntilCancelled("arms_in");
          break;
        case InfinilichSpinBeamsBehavior.SpinState.GoToStartPoint:
          this.m_aiAnimator.LockFacingDirection = true;
          this.m_aiAnimator.PlayUntilCancelled("spin");
          this.m_setupTimer = 0.0f;
          break;
        case InfinilichSpinBeamsBehavior.SpinState.BeamMode:
          this.m_aiAnimator.FacingDirection = -90f;
          this.Fire();
          break;
        case InfinilichSpinBeamsBehavior.SpinState.ArmsOut:
          this.m_aiAnimator.PlayUntilFinished("arms_out");
          break;
      }
    }

    private void EndState(InfinilichSpinBeamsBehavior.SpinState state)
    {
      if (state != InfinilichSpinBeamsBehavior.SpinState.BeamMode || !(bool) (Object) this.m_bulletSource || this.m_bulletSource.IsEnded)
        return;
      this.m_bulletSource.ForceStop();
    }

    private enum SpinState
    {
      None,
      ArmsIn,
      GoToStartPoint,
      BeamMode,
      ArmsOut,
    }
  }

