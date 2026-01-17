// Decompiled with JetBrains decompiler
// Type: MeduziStoneEyesBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullInspector;
using System;
using UnityEngine;

#nullable disable
[InspectorDropdownName("Bosses/Meduzi/StoneEyesBehavior")]
public class MeduziStoneEyesBehavior : BasicAttackBehavior
{
  public GameObject shootPoint;
  public float distortionMaxRadius = 20f;
  public float distortionDuration = 1.5f;
  public float stoneDuration = 3f;
  [InspectorCategory("Visuals")]
  public string anim;
  [InspectorCategory("Visuals")]
  public float distortionIntensity = 0.5f;
  [InspectorCategory("Visuals")]
  public float distortionThickness = 0.04f;
  private MeduziStoneEyesBehavior.State m_state;
  private Vector2 m_distortionCenter;
  private float m_timer;
  private float m_prevWaveDist;

  public override void Start()
  {
    base.Start();
    this.m_aiAnimator.spriteAnimator.AnimationEventTriggered += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.AnimationEventTriggered);
  }

  public override void Upkeep() => base.Upkeep();

  public override BehaviorResult Update()
  {
    BehaviorResult behaviorResult = base.Update();
    if (behaviorResult != BehaviorResult.Continue)
      return behaviorResult;
    if (!this.IsReady())
      return BehaviorResult.Continue;
    this.m_aiAnimator.PlayUntilFinished(this.anim, true);
    this.m_state = MeduziStoneEyesBehavior.State.WaitingToFire;
    this.m_aiActor.ClearPath();
    this.m_updateEveryFrame = true;
    return BehaviorResult.RunContinuous;
  }

  public override ContinuousBehaviorResult ContinuousUpdate()
  {
    int num1 = (int) base.ContinuousUpdate();
    if (this.m_state == MeduziStoneEyesBehavior.State.Firing)
    {
      this.m_timer -= BraveTime.DeltaTime;
      float smoothStepInterpolate = BraveMathCollege.LinearToSmoothStepInterpolate(0.0f, this.distortionMaxRadius, (float) (1.0 - (double) this.m_timer / (double) this.distortionDuration));
      for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
      {
        PlayerController allPlayer = GameManager.Instance.AllPlayers[index];
        if (!allPlayer.healthHaver.IsDead && !allPlayer.spriteAnimator.QueryInvulnerabilityFrame() && allPlayer.healthHaver.IsVulnerable)
        {
          Vector2 unitCenter = allPlayer.specRigidbody.GetUnitCenter(ColliderType.HitBox);
          float num2 = Vector2.Distance(unitCenter, this.m_distortionCenter);
          if ((double) num2 >= (double) this.m_prevWaveDist - 0.25 && (double) num2 <= (double) smoothStepInterpolate + 0.25)
          {
            float angle = (unitCenter - this.m_distortionCenter).ToAngle();
            if ((double) BraveMathCollege.AbsAngleBetween(allPlayer.FacingDirection, angle) >= 45.0)
              allPlayer.CurrentStoneGunTimer = this.stoneDuration;
          }
        }
      }
      this.m_prevWaveDist = smoothStepInterpolate;
    }
    return this.m_aiAnimator.IsPlaying(this.anim) || (double) this.m_timer > 0.0 ? ContinuousBehaviorResult.Continue : ContinuousBehaviorResult.Finished;
  }

  public override void EndContinuousUpdate()
  {
    base.EndContinuousUpdate();
    this.m_state = MeduziStoneEyesBehavior.State.Idle;
    this.m_aiAnimator.EndAnimationIf(this.anim);
    this.m_updateEveryFrame = false;
    this.UpdateCooldowns();
  }

  public override bool IsOverridable() => false;

  private void AnimationEventTriggered(
    tk2dSpriteAnimator animator,
    tk2dSpriteAnimationClip clip,
    int frame)
  {
    if (this.m_state != MeduziStoneEyesBehavior.State.WaitingToFire || !(clip.GetFrame(frame).eventInfo == "fire"))
      return;
    this.m_distortionCenter = this.shootPoint.transform.position.XY();
    Exploder.DoDistortionWave(this.m_distortionCenter, this.distortionIntensity, this.distortionThickness, this.distortionMaxRadius, this.distortionDuration);
    this.m_timer = this.distortionDuration - BraveTime.DeltaTime;
    this.m_state = MeduziStoneEyesBehavior.State.Firing;
    this.m_prevWaveDist = 0.0f;
  }

  private enum State
  {
    Idle,
    WaitingToFire,
    Firing,
  }
}
