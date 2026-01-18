using FullInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

public class ShootBeamBehavior : BasicAttackBehavior
  {
    public ShootBeamBehavior.BeamSelection beamSelection;
    [InspectorShowIf("ShowSpecificBeamShooter")]
    public AIBeamShooter specificBeamShooter;
    public float firingTime;
    public bool stopWhileFiring;
    public ShootBeamBehavior.InitialAimType initialAimType;
    public float initialAimOffset;
    [InspectorIndent]
    [InspectorShowIf("ShowRandomInitialAimOffsetSign")]
    public bool randomInitialAimOffsetSign;
    public bool restrictBeamLengthToAim;
    [InspectorIndent]
    [InspectorShowIf("restrictBeamLengthToAim")]
    public float beamLengthOFfset;
    [InspectorIndent]
    [InspectorShowIf("restrictBeamLengthToAim")]
    public float beamLengthSinMagnitude;
    [InspectorIndent]
    [InspectorShowIf("restrictBeamLengthToAim")]
    public float beamLengthSinPeriod;
    [InspectorHeader("Tracking")]
    public ShootBeamBehavior.TrackingType trackingType;
    [InspectorShowIf("ShowFollowVars")]
    public float maxUnitTurnRate;
    [InspectorShowIf("ShowFollowVars")]
    public float unitTurnRateAcceleration;
    [InspectorShowIf("ShowFollowVars")]
    public float minUnitRadius = 5f;
    [InspectorShowIf("ShowFollowVars")]
    public bool useDegreeCatchUp;
    [InspectorShowIf("ShowDegCatchUpVars")]
    [InspectorIndent]
    public float minDegreesForCatchUp;
    [InspectorIndent]
    [InspectorShowIf("ShowDegCatchUpVars")]
    public float degreeCatchUpSpeed;
    [InspectorShowIf("ShowFollowVars")]
    public bool useUnitCatchUp;
    [InspectorShowIf("ShowUnitCatchUpVars")]
    [InspectorIndent]
    public float minUnitForCatchUp;
    [InspectorShowIf("ShowUnitCatchUpVars")]
    [InspectorIndent]
    public float maxUnitForCatchUp;
    [InspectorShowIf("ShowUnitCatchUpVars")]
    [InspectorIndent]
    public float unitCatchUpSpeed;
    [InspectorShowIf("ShowFollowVars")]
    public bool useUnitOvershoot;
    [InspectorIndent]
    [InspectorShowIf("ShowUnitOvershootVars")]
    public float minUnitForOvershoot;
    [InspectorShowIf("ShowUnitOvershootVars")]
    [InspectorIndent]
    public float unitOvershootTime;
    [InspectorShowIf("ShowUnitOvershootVars")]
    [InspectorIndent]
    public float unitOvershootSpeed;
    [InspectorShowIf("ShowDegRate")]
    public float maxDegTurnRate;
    [InspectorShowIf("ShowDegAccel")]
    public float degTurnRateAcceleration;
    [InspectorCategory("Visuals")]
    public string TellAnimation;
    [InspectorCategory("Visuals")]
    public string FireAnimation;
    [InspectorCategory("Visuals")]
    public string PostFireAnimation;
    private List<AIBeamShooter> m_allBeamShooters;
    private readonly List<AIBeamShooter> m_currentBeamShooters = new List<AIBeamShooter>();
    private float m_timer;
    private float m_firingTime;
    private Vector2 m_targetPosition;
    private float m_currentUnitTurnRate;
    private float m_currentDegTurnRate;
    private float m_unitOvershootFixedDirection;
    private float m_unitOvershootTimer;
    private SpeculativeRigidbody m_backupTarget;
    private ShootBeamBehavior.State m_state;

    private bool ShowSpecificBeamShooter()
    {
      return this.beamSelection == ShootBeamBehavior.BeamSelection.Specify;
    }

    private bool ShowFollowVars() => this.trackingType == ShootBeamBehavior.TrackingType.Follow;

    private bool ShowDegRate()
    {
      return this.trackingType == ShootBeamBehavior.TrackingType.ConstantTurn || this.trackingType == ShootBeamBehavior.TrackingType.AccelTurn;
    }

    private bool ShowDegAccel() => this.trackingType == ShootBeamBehavior.TrackingType.AccelTurn;

    private bool ShowDegCatchUpVars()
    {
      return this.trackingType == ShootBeamBehavior.TrackingType.Follow && this.useDegreeCatchUp;
    }

    private bool ShowUnitCatchUpVars()
    {
      return this.trackingType == ShootBeamBehavior.TrackingType.Follow && this.useUnitCatchUp;
    }

    private bool ShowUnitOvershootVars()
    {
      return this.trackingType == ShootBeamBehavior.TrackingType.Follow && this.useUnitOvershoot;
    }

    private bool ShowRandomInitialAimOffsetSign() => (double) this.initialAimOffset > 0.0;

    public override void Start()
    {
      base.Start();
      this.m_allBeamShooters = new List<AIBeamShooter>((IEnumerable<AIBeamShooter>) this.m_aiActor.GetComponents<AIBeamShooter>());
      if (string.IsNullOrEmpty(this.TellAnimation))
        return;
      this.m_aiAnimator.spriteAnimator.AnimationEventTriggered += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.AnimEventTriggered);
      if (!(bool) (UnityEngine.Object) this.m_aiAnimator.ChildAnimator)
        return;
      this.m_aiAnimator.ChildAnimator.spriteAnimator.AnimationEventTriggered += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.AnimEventTriggered);
    }

    public override void Upkeep()
    {
      base.Upkeep();
      if ((bool) (UnityEngine.Object) this.m_aiActor.TargetRigidbody)
      {
        this.m_targetPosition = this.m_aiActor.TargetRigidbody.GetUnitCenter(ColliderType.HitBox);
        this.m_backupTarget = this.m_aiActor.TargetRigidbody;
      }
      else
      {
        if (!(bool) (UnityEngine.Object) this.m_backupTarget)
          return;
        this.m_targetPosition = this.m_backupTarget.GetUnitCenter(ColliderType.HitBox);
      }
    }

    public override BehaviorResult Update()
    {
      int num = (int) base.Update();
      BehaviorResult behaviorResult = base.Update();
      if (behaviorResult != BehaviorResult.Continue)
        return behaviorResult;
      if (!this.IsReady())
        return BehaviorResult.Continue;
      if (!string.IsNullOrEmpty(this.TellAnimation))
      {
        this.m_aiAnimator.PlayUntilFinished(this.TellAnimation, true);
        this.state = ShootBeamBehavior.State.WaitingForTell;
      }
      else
        this.Fire();
      this.m_updateEveryFrame = true;
      return BehaviorResult.RunContinuous;
    }

    public override ContinuousBehaviorResult ContinuousUpdate()
    {
      int num1 = (int) base.ContinuousUpdate();
      if (this.state == ShootBeamBehavior.State.WaitingForTell)
      {
        if (!this.m_aiAnimator.IsPlaying(this.TellAnimation))
          this.Fire();
        return ContinuousBehaviorResult.Continue;
      }
      if (this.state == ShootBeamBehavior.State.Firing)
      {
        this.m_firingTime += this.m_deltaTime;
        this.m_timer -= this.m_deltaTime;
        if ((double) this.m_timer <= 0.0 || !this.m_currentBeamShooters[0].IsFiringLaser)
        {
          this.StopLasers();
          if (string.IsNullOrEmpty(this.PostFireAnimation))
            return ContinuousBehaviorResult.Finished;
          this.state = ShootBeamBehavior.State.WaitingForPostAnim;
          this.m_aiAnimator.PlayUntilFinished(this.PostFireAnimation);
          return ContinuousBehaviorResult.Continue;
        }
        float num2 = 0.0f;
        if (this.trackingType == ShootBeamBehavior.TrackingType.Follow)
        {
          AIBeamShooter currentBeamShooter = this.m_currentBeamShooters[0];
          Vector2 laserFiringCenter = currentBeamShooter.LaserFiringCenter;
          float num3 = Mathf.Max(this.minUnitRadius, Vector2.Distance(this.m_targetPosition, laserFiringCenter));
          float f1 = BraveMathCollege.ClampAngle180((this.m_targetPosition - laserFiringCenter).ToAngle() - currentBeamShooter.LaserAngle);
          float f2 = (float) ((double) f1 * (double) num3 * (Math.PI / 180.0));
          float max = this.maxUnitTurnRate;
          float num4 = Mathf.Sign(f1);
          if ((double) this.m_unitOvershootTimer > 0.0)
          {
            num4 = this.m_unitOvershootFixedDirection;
            this.m_unitOvershootTimer -= this.m_deltaTime;
            max = this.unitOvershootSpeed;
          }
          this.m_currentUnitTurnRate = Mathf.Clamp(this.m_currentUnitTurnRate + num4 * this.unitTurnRateAcceleration * this.m_deltaTime, -max, max);
          float num5 = (float) ((double) this.m_currentUnitTurnRate / (double) num3 * 57.295780181884766);
          float a = 0.0f;
          if (this.useDegreeCatchUp && (double) Mathf.Abs(f1) > (double) this.minDegreesForCatchUp)
          {
            float b = Mathf.InverseLerp(this.minDegreesForCatchUp, 180f, Mathf.Abs(f1)) * this.degreeCatchUpSpeed;
            a = Mathf.Max(a, b);
          }
          if (this.useUnitCatchUp && (double) Mathf.Abs(f2) > (double) this.minUnitForCatchUp)
          {
            float b = (float) ((double) (Mathf.InverseLerp(this.minUnitForCatchUp, this.maxUnitForCatchUp, Mathf.Abs(f2)) * this.unitCatchUpSpeed) / (double) num3 * 57.295780181884766);
            a = Mathf.Max(a, b);
          }
          if (this.useUnitOvershoot && (double) Mathf.Abs(f2) < (double) this.minUnitForOvershoot)
          {
            this.m_unitOvershootFixedDirection = (double) this.m_currentUnitTurnRate <= 0.0 ? -1f : 1f;
            this.m_unitOvershootTimer = this.unitOvershootTime;
          }
          float num6 = a * Mathf.Sign(f1);
          num2 = num5 + num6;
        }
        else if (this.trackingType == ShootBeamBehavior.TrackingType.ConstantTurn)
          num2 = this.maxDegTurnRate;
        else if (this.trackingType == ShootBeamBehavior.TrackingType.AccelTurn)
        {
          this.m_currentDegTurnRate = Mathf.Clamp(this.m_currentDegTurnRate + this.degTurnRateAcceleration * this.m_deltaTime, -this.maxDegTurnRate, this.maxDegTurnRate);
          num2 = this.m_currentDegTurnRate;
        }
        for (int index = 0; index < this.m_currentBeamShooters.Count; ++index)
        {
          AIBeamShooter currentBeamShooter = this.m_currentBeamShooters[index];
          currentBeamShooter.LaserAngle = BraveMathCollege.ClampAngle360(currentBeamShooter.LaserAngle + num2 * this.m_deltaTime);
          if (this.restrictBeamLengthToAim && (bool) (UnityEngine.Object) this.m_aiActor.TargetRigidbody)
          {
            float magnitude = (this.m_aiActor.TargetRigidbody.GetUnitCenter(ColliderType.HitBox) - currentBeamShooter.LaserFiringCenter).magnitude;
            currentBeamShooter.MaxBeamLength = magnitude + this.beamLengthOFfset;
            if ((double) this.beamLengthSinMagnitude > 0.0 && (double) this.beamLengthSinPeriod > 0.0)
            {
              currentBeamShooter.MaxBeamLength += Mathf.Sin((float) ((double) this.m_firingTime / (double) this.beamLengthSinPeriod * 3.1415927410125732)) * this.beamLengthSinMagnitude;
              if ((double) currentBeamShooter.MaxBeamLength < 0.0)
                currentBeamShooter.MaxBeamLength = 0.0f;
            }
          }
        }
        return ContinuousBehaviorResult.Continue;
      }
      return this.state == ShootBeamBehavior.State.WaitingForPostAnim && !this.m_aiAnimator.IsPlaying(this.PostFireAnimation) ? ContinuousBehaviorResult.Finished : ContinuousBehaviorResult.Continue;
    }

    public override void EndContinuousUpdate()
    {
      base.EndContinuousUpdate();
      if (!string.IsNullOrEmpty(this.TellAnimation))
        this.m_aiAnimator.EndAnimationIf(this.TellAnimation);
      if (!string.IsNullOrEmpty(this.FireAnimation))
        this.m_aiAnimator.EndAnimationIf(this.FireAnimation);
      if (!string.IsNullOrEmpty(this.PostFireAnimation))
        this.m_aiAnimator.EndAnimationIf(this.PostFireAnimation);
      this.StopLasers();
      this.state = ShootBeamBehavior.State.Idle;
      this.m_aiAnimator.LockFacingDirection = false;
      this.m_updateEveryFrame = false;
      this.UpdateCooldowns();
    }

    public override void OnActorPreDeath()
    {
      base.OnActorPreDeath();
      this.StopLasers();
    }

    private void AnimEventTriggered(
      tk2dSpriteAnimator sprite,
      tk2dSpriteAnimationClip clip,
      int frameNum)
    {
      tk2dSpriteAnimationFrame frame = clip.GetFrame(frameNum);
      if (this.state != ShootBeamBehavior.State.WaitingForTell || !(frame.eventInfo == "fire"))
        return;
      this.Fire();
    }

    private void Fire()
    {
      if (!string.IsNullOrEmpty(this.FireAnimation))
      {
        this.m_aiAnimator.EndAnimation();
        this.m_aiAnimator.PlayUntilFinished(this.FireAnimation);
      }
      if (this.stopWhileFiring)
        this.m_aiActor.ClearPath();
      if (this.beamSelection == ShootBeamBehavior.BeamSelection.All)
        this.m_currentBeamShooters.AddRange((IEnumerable<AIBeamShooter>) this.m_allBeamShooters);
      else if (this.beamSelection == ShootBeamBehavior.BeamSelection.Random)
        this.m_currentBeamShooters.Add(BraveUtility.RandomElement<AIBeamShooter>(this.m_allBeamShooters));
      else if (this.beamSelection == ShootBeamBehavior.BeamSelection.Specify)
        this.m_currentBeamShooters.Add(this.specificBeamShooter);
      float facingDirection = this.m_currentBeamShooters[0].CurrentAiAnimator.FacingDirection;
      float num1 = !this.randomInitialAimOffsetSign ? 1f : BraveUtility.RandomSign();
      for (int index = 0; index < this.m_currentBeamShooters.Count; ++index)
      {
        AIBeamShooter currentBeamShooter = this.m_currentBeamShooters[index];
        if (this.restrictBeamLengthToAim && (bool) (UnityEngine.Object) this.m_aiActor.TargetRigidbody)
        {
          float magnitude = (this.m_aiActor.TargetRigidbody.GetUnitCenter(ColliderType.HitBox) - currentBeamShooter.LaserFiringCenter).magnitude;
          currentBeamShooter.MaxBeamLength = magnitude;
        }
        float num2 = 0.0f;
        if (this.initialAimType == ShootBeamBehavior.InitialAimType.FacingDirection)
          num2 = facingDirection;
        else if (this.initialAimType == ShootBeamBehavior.InitialAimType.Aim)
        {
          if ((bool) (UnityEngine.Object) this.m_aiActor.TargetRigidbody)
            num2 = (this.m_aiActor.TargetRigidbody.GetUnitCenter(ColliderType.HitBox) - currentBeamShooter.LaserFiringCenter).ToAngle();
        }
        else if (this.initialAimType == ShootBeamBehavior.InitialAimType.Absolute)
          num2 = 0.0f;
        else if (this.initialAimType == ShootBeamBehavior.InitialAimType.Transform)
          num2 = currentBeamShooter.beamTransform.eulerAngles.z;
        float laserAngle = num2 + num1 * this.initialAimOffset;
        currentBeamShooter.StartFiringLaser(laserAngle);
      }
      this.m_timer = this.firingTime;
      this.m_currentUnitTurnRate = 0.0f;
      this.m_currentDegTurnRate = 0.0f;
      this.m_firingTime = 0.0f;
      this.state = ShootBeamBehavior.State.Firing;
    }

    private void StopLasers()
    {
      for (int index = 0; index < this.m_currentBeamShooters.Count; ++index)
        this.m_currentBeamShooters[index].StopFiringLaser();
      this.m_currentBeamShooters.Clear();
    }

    private ShootBeamBehavior.State state
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

    private void BeginState(ShootBeamBehavior.State state)
    {
    }

    private void EndState(ShootBeamBehavior.State state)
    {
    }

    public enum BeamSelection
    {
      All,
      Random,
      Specify,
    }

    public enum TrackingType
    {
      Follow,
      ConstantTurn,
      AccelTurn,
    }

    public enum InitialAimType
    {
      FacingDirection,
      Aim,
      Absolute,
      Transform,
    }

    private enum State
    {
      Idle,
      WaitingForTell,
      Firing,
      WaitingForPostAnim,
    }
  }

