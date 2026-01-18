using FullInspector;
using System;
using UnityEngine;

#nullable disable

[InspectorDropdownName("Bosses/BossFinalRobot/PoisonBehavior")]
public class BossFinalRobotPoisonBehavior : BasicAttackBehavior
  {
    public float initialAimDirection;
    public float turnRate = 360f;
    public float totalTurnDegrees = 360f;
    public int divisions = 6;
    [InspectorCategory("Visuals")]
    public string tellAnimation;
    private AIBeamShooter m_beamShooter;
    private float m_turnedDegrees;
    private float m_nextToggleDegrees;
    private float m_toggleWidthDegrees;
    private BossFinalRobotPoisonBehavior.State m_state;

    public override void Start()
    {
      base.Start();
      this.m_beamShooter = this.m_aiActor.GetComponent<AIBeamShooter>();
      if (string.IsNullOrEmpty(this.tellAnimation))
        return;
      this.m_aiAnimator.spriteAnimator.AnimationEventTriggered += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.AnimEventTriggered);
    }

    public override BehaviorResult Update()
    {
      int num = (int) base.Update();
      BehaviorResult behaviorResult = base.Update();
      if (behaviorResult != BehaviorResult.Continue)
        return behaviorResult;
      if (!this.IsReady())
        return BehaviorResult.Continue;
      this.m_aiActor.ClearPath();
      this.m_turnedDegrees = 0.0f;
      this.m_toggleWidthDegrees = 360f / (float) this.divisions;
      this.m_nextToggleDegrees = UnityEngine.Random.Range(0.0f, this.m_toggleWidthDegrees);
      this.m_beamShooter.LaserAngle = this.initialAimDirection;
      if (!string.IsNullOrEmpty(this.tellAnimation))
      {
        this.m_aiAnimator.PlayUntilFinished(this.tellAnimation, true);
        this.m_state = BossFinalRobotPoisonBehavior.State.WaitingForTell;
      }
      else
        this.m_state = BossFinalRobotPoisonBehavior.State.Firing;
      this.m_updateEveryFrame = true;
      return BehaviorResult.RunContinuous;
    }

    public override ContinuousBehaviorResult ContinuousUpdate()
    {
      int num1 = (int) base.ContinuousUpdate();
      if (this.m_state == BossFinalRobotPoisonBehavior.State.WaitingForTell)
      {
        if (!this.m_aiAnimator.IsPlaying(this.tellAnimation))
          this.m_state = BossFinalRobotPoisonBehavior.State.Firing;
      }
      else if (this.m_state == BossFinalRobotPoisonBehavior.State.Firing)
      {
        float num2 = Mathf.Sign(this.turnRate);
        float num3 = Mathf.Abs(this.turnRate * this.m_deltaTime);
        float laserAngle = this.m_beamShooter.LaserAngle;
        bool flag = this.m_beamShooter.IsFiringLaser;
        while ((double) num3 > 0.0)
        {
          if ((double) num3 < (double) this.m_nextToggleDegrees)
          {
            this.m_turnedDegrees += num3;
            laserAngle += num3 * num2;
            this.m_nextToggleDegrees -= num3;
            num3 = 0.0f;
          }
          else
          {
            this.m_turnedDegrees += this.m_nextToggleDegrees;
            laserAngle += this.m_nextToggleDegrees * num2;
            num3 -= this.m_nextToggleDegrees;
            this.m_nextToggleDegrees = this.m_toggleWidthDegrees;
            if (flag)
            {
              this.m_beamShooter.StopFiringLaser();
              flag = false;
            }
            else
            {
              this.m_beamShooter.StartFiringLaser(this.m_beamShooter.LaserAngle);
              if ((bool) (UnityEngine.Object) this.m_beamShooter.LaserBeam)
                this.m_beamShooter.LaserBeam.projectile.ImmuneToSustainedBlanks = true;
              flag = true;
            }
          }
        }
        this.m_beamShooter.LaserAngle = BraveMathCollege.ClampAngle360(laserAngle);
        if ((double) this.m_turnedDegrees >= (double) this.totalTurnDegrees)
        {
          if (string.IsNullOrEmpty(this.tellAnimation) || !this.m_aiAnimator.IsPlaying(this.tellAnimation))
            return ContinuousBehaviorResult.Finished;
          if ((bool) (UnityEngine.Object) this.m_beamShooter && this.m_beamShooter.IsFiringLaser)
            this.m_beamShooter.StopFiringLaser();
          this.m_state = BossFinalRobotPoisonBehavior.State.WaitingForAnim;
          return ContinuousBehaviorResult.Continue;
        }
      }
      else if (this.m_state == BossFinalRobotPoisonBehavior.State.WaitingForAnim && !this.m_aiAnimator.IsPlaying(this.tellAnimation))
        return ContinuousBehaviorResult.Finished;
      return ContinuousBehaviorResult.Continue;
    }

    public override void EndContinuousUpdate()
    {
      base.EndContinuousUpdate();
      if ((bool) (UnityEngine.Object) this.m_beamShooter && this.m_beamShooter.IsFiringLaser)
        this.m_beamShooter.StopFiringLaser();
      if (!string.IsNullOrEmpty(this.tellAnimation))
        this.m_aiAnimator.EndAnimationIf(this.tellAnimation);
      this.m_state = BossFinalRobotPoisonBehavior.State.None;
      this.m_aiAnimator.LockFacingDirection = false;
      this.m_updateEveryFrame = false;
      this.UpdateCooldowns();
    }

    public override void OnActorPreDeath()
    {
      base.OnActorPreDeath();
      this.m_beamShooter.StopFiringLaser();
    }

    private void AnimEventTriggered(
      tk2dSpriteAnimator sprite,
      tk2dSpriteAnimationClip clip,
      int frameNum)
    {
      tk2dSpriteAnimationFrame frame = clip.GetFrame(frameNum);
      if (this.m_state != BossFinalRobotPoisonBehavior.State.WaitingForTell || !(frame.eventInfo == "fire"))
        return;
      this.m_state = BossFinalRobotPoisonBehavior.State.Firing;
    }

    private enum State
    {
      None,
      WaitingForTell,
      Firing,
      WaitingForAnim,
    }
  }

