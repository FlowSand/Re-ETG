// Decompiled with JetBrains decompiler
// Type: DraGunMac10Behavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullInspector;
using System;
using UnityEngine;

#nullable disable
[InspectorDropdownName("Bosses/DraGun/Mac10Behavior")]
public class DraGunMac10Behavior : BasicAttackBehavior
{
  public GameObject ShootPoint;
  public BulletScriptSelector BulletScript;
  public DraGunMac10Behavior.FireType fireType;
  public Animation unityAnimation;
  [InspectorShowIf("UseUnityAnimation")]
  [InspectorIndent]
  public string unityIntroAnim;
  [InspectorIndent]
  [InspectorShowIf("UseUnityAnimation")]
  public string unityShootAnim;
  [InspectorIndent]
  [InspectorShowIf("UseUnityAnimation")]
  public string unityOutroAnim;
  public AIAnimator aiAnimator;
  [InspectorIndent]
  [InspectorShowIf("UseAiAnimator")]
  public bool useAnimationDirection;
  [InspectorIndent]
  [InspectorShowIf("UseAiAnimator")]
  public string aiIntroAnim;
  [InspectorIndent]
  [InspectorShowIf("UseAiAnimator")]
  public string aiShootAnim;
  [InspectorShowIf("UseAiAnimator")]
  [InspectorIndent]
  public string aiOutroAnim;
  private DraGunMac10Behavior.HandShootState m_state;
  private bool m_isShooting;
  private BulletScriptSource m_bulletSource;

  private bool UseUnityAnimation() => (UnityEngine.Object) this.unityAnimation != (UnityEngine.Object) null;

  private bool UseAiAnimator() => (UnityEngine.Object) this.aiAnimator != (UnityEngine.Object) null;

  private DraGunMac10Behavior.HandShootState State
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

  public override void Start()
  {
    base.Start();
    if (this.fireType == DraGunMac10Behavior.FireType.tk2dAnimEvent)
      this.aiAnimator.spriteAnimator.AnimationEventTriggered += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.tk2dAnimationEventTriggered);
    if (this.fireType != DraGunMac10Behavior.FireType.UnityAnimEvent)
      return;
    this.m_aiActor.behaviorSpeculator.AnimationEventTriggered += new Action<string>(this.UnityAnimationEventTriggered);
  }

  public override BehaviorResult Update()
  {
    BehaviorResult behaviorResult = base.Update();
    if (behaviorResult != BehaviorResult.Continue)
      return behaviorResult;
    if (!this.IsReady())
      return BehaviorResult.Continue;
    this.State = DraGunMac10Behavior.HandShootState.Intro;
    if ((bool) (UnityEngine.Object) this.aiAnimator && this.useAnimationDirection)
      this.aiAnimator.UseAnimatedFacingDirection = true;
    this.m_updateEveryFrame = true;
    return BehaviorResult.RunContinuous;
  }

  public override ContinuousBehaviorResult ContinuousUpdate()
  {
    int num = (int) base.ContinuousUpdate();
    if (this.State == DraGunMac10Behavior.HandShootState.Intro)
    {
      bool flag = true;
      if (this.UseUnityAnimation())
        flag &= !this.unityAnimation.IsPlaying(this.unityIntroAnim);
      if (this.UseAiAnimator())
        flag &= !this.aiAnimator.IsPlaying(this.aiIntroAnim);
      if (flag)
        this.State = DraGunMac10Behavior.HandShootState.Shooting;
    }
    else if (this.State == DraGunMac10Behavior.HandShootState.Shooting)
    {
      if (this.fireType == DraGunMac10Behavior.FireType.Immediate && !this.m_isShooting)
        this.ShootBulletScript();
      bool flag = true;
      if ((bool) (UnityEngine.Object) this.unityAnimation)
        flag &= !this.unityAnimation.IsPlaying(this.unityShootAnim);
      if ((bool) (UnityEngine.Object) this.aiAnimator)
        flag = ((flag ? 1 : 0) & (!this.aiAnimator.IsPlaying(this.aiShootAnim) ? 1 : (this.aiAnimator.spriteAnimator.CurrentClip.wrapMode == tk2dSpriteAnimationClip.WrapMode.Loop ? 1 : 0))) != 0;
      if (flag)
        this.State = DraGunMac10Behavior.HandShootState.Outro;
    }
    else if (this.State == DraGunMac10Behavior.HandShootState.Outro)
    {
      bool flag = true;
      if ((bool) (UnityEngine.Object) this.unityAnimation)
        flag &= !this.unityAnimation.IsPlaying(this.unityOutroAnim);
      if ((bool) (UnityEngine.Object) this.aiAnimator)
        flag = ((flag ? 1 : 0) & (!this.aiAnimator.IsPlaying(this.aiOutroAnim) ? 1 : (this.aiAnimator.spriteAnimator.CurrentClip.wrapMode == tk2dSpriteAnimationClip.WrapMode.Loop ? 1 : 0))) != 0;
      if (flag)
        return ContinuousBehaviorResult.Finished;
    }
    return ContinuousBehaviorResult.Continue;
  }

  public override void EndContinuousUpdate()
  {
    base.EndContinuousUpdate();
    this.State = DraGunMac10Behavior.HandShootState.None;
    if ((bool) (UnityEngine.Object) this.m_bulletSource)
      this.m_bulletSource.ForceStop();
    if ((bool) (UnityEngine.Object) this.aiAnimator)
    {
      this.aiAnimator.EndAnimation();
      if (this.useAnimationDirection)
        this.aiAnimator.UseAnimatedFacingDirection = false;
    }
    if ((bool) (UnityEngine.Object) this.unityAnimation)
    {
      this.unityAnimation.Stop();
      this.unityAnimation.GetClip(this.unityOutroAnim).SampleAnimation(this.unityAnimation.gameObject, 1000f);
      this.unityAnimation.GetComponent<DraGunArmController>().ClipArmSprites();
    }
    this.m_updateEveryFrame = false;
    this.UpdateCooldowns();
  }

  public override bool IsOverridable() => false;

  private void tk2dAnimationEventTriggered(
    tk2dSpriteAnimator animator,
    tk2dSpriteAnimationClip clip,
    int frame)
  {
    this.UnityAnimationEventTriggered(clip.GetFrame(frame).eventInfo);
  }

  private void UnityAnimationEventTriggered(string eventInfo)
  {
    switch (eventInfo)
    {
      case "fire":
        this.ShootBulletScript();
        break;
      case "cease_fire":
        if (!(bool) (UnityEngine.Object) this.m_bulletSource)
          break;
        this.m_bulletSource.ForceStop();
        break;
    }
  }

  private void ShootBulletScript()
  {
    if (string.IsNullOrEmpty(this.BulletScript.scriptTypeName))
      return;
    if (!(bool) (UnityEngine.Object) this.m_bulletSource)
      this.m_bulletSource = this.ShootPoint.GetOrAddComponent<BulletScriptSource>();
    this.m_bulletSource.BulletManager = this.m_aiActor.bulletBank;
    this.m_bulletSource.BulletScript = this.BulletScript;
    this.m_bulletSource.Initialize();
    this.m_isShooting = true;
  }

  private void BeginState(DraGunMac10Behavior.HandShootState state)
  {
    switch (state)
    {
      case DraGunMac10Behavior.HandShootState.Intro:
        if ((bool) (UnityEngine.Object) this.unityAnimation)
          this.unityAnimation.Play(this.unityIntroAnim);
        if (!(bool) (UnityEngine.Object) this.aiAnimator)
          break;
        this.aiAnimator.PlayUntilCancelled(this.aiIntroAnim);
        break;
      case DraGunMac10Behavior.HandShootState.Shooting:
        if ((bool) (UnityEngine.Object) this.unityAnimation)
          this.unityAnimation.Play(this.unityShootAnim);
        if ((bool) (UnityEngine.Object) this.aiAnimator)
          this.aiAnimator.PlayUntilCancelled(this.aiShootAnim);
        this.m_isShooting = false;
        break;
      case DraGunMac10Behavior.HandShootState.Outro:
        if ((bool) (UnityEngine.Object) this.unityAnimation)
          this.unityAnimation.Play(this.unityOutroAnim);
        if (!(bool) (UnityEngine.Object) this.aiAnimator)
          break;
        this.aiAnimator.PlayUntilCancelled(this.aiOutroAnim);
        break;
    }
  }

  private void EndState(DraGunMac10Behavior.HandShootState state)
  {
    if (state != DraGunMac10Behavior.HandShootState.Shooting || !this.m_isShooting || !(bool) (UnityEngine.Object) this.m_bulletSource)
      return;
    this.m_bulletSource.ForceStop();
  }

  private enum HandShootState
  {
    None,
    Intro,
    Shooting,
    Outro,
  }

  public enum FireType
  {
    Immediate,
    tk2dAnimEvent,
    UnityAnimEvent,
  }
}
