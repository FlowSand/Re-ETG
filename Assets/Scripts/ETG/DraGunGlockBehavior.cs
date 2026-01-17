// Decompiled with JetBrains decompiler
// Type: DraGunGlockBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullInspector;
using System;
using UnityEngine;

#nullable disable
[InspectorDropdownName("Bosses/DraGun/GlockBehavior")]
public class DraGunGlockBehavior : BasicAttackBehavior
{
  public GameObject shootPoint;
  public Animation unityAnimation;
  public AIAnimator aiAnimator;
  public DraGunGlockBehavior.GlockAttack[] attacks;
  private DraGunGlockBehavior.HandState m_state;
  private float m_delayTimer;
  private int m_attackIndex;
  private bool m_isShooting;
  private bool m_facingLeft;
  private string m_unityAnimPrefix;
  private BulletScriptSource m_bulletSource;

  private DraGunGlockBehavior.HandState State
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
    this.aiAnimator.spriteAnimator.AnimationEventTriggered += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.AnimationEventTriggered);
    this.m_facingLeft = this.aiAnimator.name.Contains("left", true);
    this.m_unityAnimPrefix = !this.m_facingLeft ? "DraGunRight" : "DraGunLeft";
  }

  public override BehaviorResult Update()
  {
    BehaviorResult behaviorResult = base.Update();
    if (behaviorResult != BehaviorResult.Continue)
      return behaviorResult;
    if (!this.IsReady())
      return BehaviorResult.Continue;
    this.m_attackIndex = -1;
    this.State = DraGunGlockBehavior.HandState.Intro;
    this.m_updateEveryFrame = true;
    return BehaviorResult.RunContinuous;
  }

  public override ContinuousBehaviorResult ContinuousUpdate()
  {
    int num = (int) base.ContinuousUpdate();
    if (this.State == DraGunGlockBehavior.HandState.Intro)
    {
      if (!this.aiAnimator.IsPlaying("glock_draw"))
      {
        this.State = DraGunGlockBehavior.HandState.Out;
        this.AdvanceAttack();
      }
    }
    else if (this.State == DraGunGlockBehavior.HandState.Out || this.State == DraGunGlockBehavior.HandState.In)
    {
      if (this.m_attackIndex >= this.attacks.Length)
        this.State = this.State != DraGunGlockBehavior.HandState.Out ? DraGunGlockBehavior.HandState.MoveToOut : DraGunGlockBehavior.HandState.Outro;
      else if ((double) this.m_delayTimer > 0.0)
      {
        this.m_delayTimer -= this.m_deltaTime;
        if ((double) this.m_delayTimer <= 0.0)
          this.HandleAim();
      }
      else if (!this.m_isShooting)
        this.Fire();
      else if (!this.aiAnimator.IsPlaying(this.State != DraGunGlockBehavior.HandState.Out ? "glock_fire_in" : "glock_fire_out"))
      {
        this.m_isShooting = false;
        this.AdvanceAttack();
      }
    }
    else if (this.State == DraGunGlockBehavior.HandState.MoveToIn)
    {
      if (!this.aiAnimator.IsPlaying("glock_flip_in"))
        this.State = DraGunGlockBehavior.HandState.In;
    }
    else if (this.State == DraGunGlockBehavior.HandState.MoveToOut)
    {
      if (!this.aiAnimator.IsPlaying("glock_flip_out"))
        this.State = DraGunGlockBehavior.HandState.Out;
    }
    else if (this.State == DraGunGlockBehavior.HandState.Outro && !this.aiAnimator.IsPlaying("glock_putaway"))
      return ContinuousBehaviorResult.Finished;
    return ContinuousBehaviorResult.Continue;
  }

  public override void EndContinuousUpdate()
  {
    base.EndContinuousUpdate();
    this.State = DraGunGlockBehavior.HandState.None;
    if ((bool) (UnityEngine.Object) this.m_bulletSource)
      this.m_bulletSource.ForceStop();
    if ((bool) (UnityEngine.Object) this.aiAnimator)
      this.aiAnimator.EndAnimation();
    if ((bool) (UnityEngine.Object) this.unityAnimation)
    {
      this.unityAnimation.Stop();
      this.unityAnimation.GetClip(this.m_unityAnimPrefix + "GlockPutAway").SampleAnimation(this.unityAnimation.gameObject, 1000f);
      this.unityAnimation.GetComponent<DraGunArmController>().ClipArmSprites();
    }
    this.m_updateEveryFrame = false;
    this.UpdateCooldowns();
  }

  public override bool IsOverridable() => false;

  private void AnimationEventTriggered(
    tk2dSpriteAnimator animator,
    tk2dSpriteAnimationClip clip,
    int frame)
  {
    if (!this.m_isShooting || !(clip.GetFrame(frame).eventInfo == "fire"))
      return;
    this.ShootBulletScript();
  }

  private void AdvanceAttack()
  {
    ++this.m_attackIndex;
    if (this.m_attackIndex >= this.attacks.Length)
      return;
    this.m_delayTimer = this.attacks[this.m_attackIndex].preDelay;
    if ((double) this.m_delayTimer > 0.0)
      return;
    this.HandleAim();
  }

  private void HandleAim()
  {
    if (this.m_attackIndex >= this.attacks.Length)
      return;
    DraGunGlockBehavior.FacingDirection facingDirection = this.attacks[this.m_attackIndex].dir;
    if (facingDirection == DraGunGlockBehavior.FacingDirection.Aim && (bool) (UnityEngine.Object) this.m_aiActor.TargetRigidbody)
      facingDirection = !this.m_facingLeft ? ((double) this.m_aiActor.TargetRigidbody.UnitCenter.x <= (double) this.m_aiActor.specRigidbody.UnitCenter.x + 12.5 ? DraGunGlockBehavior.FacingDirection.In : DraGunGlockBehavior.FacingDirection.Out) : ((double) this.m_aiActor.TargetRigidbody.UnitCenter.x >= (double) this.m_aiActor.specRigidbody.UnitCenter.x - 12.5 ? DraGunGlockBehavior.FacingDirection.In : DraGunGlockBehavior.FacingDirection.Out);
    if (facingDirection == DraGunGlockBehavior.FacingDirection.In)
    {
      if (this.State != DraGunGlockBehavior.HandState.Out)
        return;
      this.State = DraGunGlockBehavior.HandState.MoveToIn;
    }
    else
    {
      if (facingDirection != DraGunGlockBehavior.FacingDirection.Out || this.State != DraGunGlockBehavior.HandState.In)
        return;
      this.State = DraGunGlockBehavior.HandState.MoveToOut;
    }
  }

  private void Fire()
  {
    this.m_isShooting = true;
    if (this.State == DraGunGlockBehavior.HandState.In)
    {
      this.aiAnimator.PlayUntilCancelled("glock_fire_in");
    }
    else
    {
      if (this.State != DraGunGlockBehavior.HandState.Out)
        return;
      this.aiAnimator.PlayUntilCancelled("glock_fire_out");
    }
  }

  private void ShootBulletScript()
  {
    if (!(bool) (UnityEngine.Object) this.m_bulletSource)
      this.m_bulletSource = this.shootPoint.GetOrAddComponent<BulletScriptSource>();
    this.m_bulletSource.BulletManager = this.m_aiActor.bulletBank;
    this.m_bulletSource.BulletScript = this.attacks[this.m_attackIndex].bulletScript;
    this.m_bulletSource.Initialize();
  }

  private void BeginState(DraGunGlockBehavior.HandState state)
  {
    if (state == DraGunGlockBehavior.HandState.Intro)
    {
      this.aiAnimator.PlayUntilCancelled("glock_draw");
      if ((bool) (UnityEngine.Object) this.unityAnimation)
        this.unityAnimation.Play(this.m_unityAnimPrefix + "GlockDraw");
    }
    if (state == DraGunGlockBehavior.HandState.MoveToOut)
    {
      this.aiAnimator.PlayUntilCancelled("glock_flip_out");
      this.unityAnimation.Play(this.m_unityAnimPrefix + "GlockFlipOut");
    }
    else if (state == DraGunGlockBehavior.HandState.MoveToIn)
    {
      this.aiAnimator.PlayUntilCancelled("glock_flip_in");
      this.unityAnimation.Play(this.m_unityAnimPrefix + "GlockFlipIn");
    }
    else
    {
      if (state != DraGunGlockBehavior.HandState.Outro)
        return;
      this.aiAnimator.PlayUntilCancelled("glock_putaway");
      this.unityAnimation.Play(this.m_unityAnimPrefix + "GlockPutAway");
    }
  }

  private void EndState(DraGunGlockBehavior.HandState state)
  {
    if (state != DraGunGlockBehavior.HandState.In && state != DraGunGlockBehavior.HandState.Out || !this.m_isShooting || !(bool) (UnityEngine.Object) this.m_bulletSource)
      return;
    this.m_bulletSource.ForceStop();
  }

  private enum HandState
  {
    None,
    Intro,
    In,
    MoveToOut,
    Out,
    MoveToIn,
    Outro,
  }

  public enum FacingDirection
  {
    Out,
    In,
    Aim,
  }

  [Serializable]
  public class GlockAttack
  {
    public float preDelay;
    public DraGunGlockBehavior.FacingDirection dir;
    public BulletScriptSelector bulletScript;
  }
}
