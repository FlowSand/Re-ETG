// Decompiled with JetBrains decompiler
// Type: MimicAwakenBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
public class MimicAwakenBehavior : BasicAttackBehavior
{
  public string awakenAnim;
  public GameObject ShootPoint;
  public BulletScriptSelector BulletScript;
  private bool m_hasFired;
  private BulletScriptSource m_bulletScriptSource;

  public override void Start()
  {
    base.Start();
    this.m_aiAnimator.spriteAnimator.AnimationEventTriggered += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.AnimEventTriggered);
  }

  public override void Upkeep()
  {
    base.Upkeep();
    this.m_aiActor.HasBeenEngaged = true;
    this.m_aiActor.CollisionDamage = 0.0f;
  }

  public override BehaviorResult Update()
  {
    BehaviorResult behaviorResult = base.Update();
    if (behaviorResult != BehaviorResult.Continue)
      return behaviorResult;
    if (this.m_hasFired || !this.IsReady())
      return BehaviorResult.Continue;
    if (!(bool) (UnityEngine.Object) this.m_aiActor.GetComponent<WallMimicController>())
    {
      this.m_aiAnimator.LockFacingDirection = true;
      this.m_aiAnimator.FacingDirection = -90f;
    }
    this.m_aiActor.ClearPath();
    this.m_aiAnimator.PlayUntilFinished(this.awakenAnim, true);
    return BehaviorResult.RunContinuous;
  }

  public override ContinuousBehaviorResult ContinuousUpdate()
  {
    int num = (int) base.ContinuousUpdate();
    return !this.m_aiAnimator.IsPlaying(this.awakenAnim) ? ContinuousBehaviorResult.Finished : ContinuousBehaviorResult.Continue;
  }

  public override void EndContinuousUpdate()
  {
    base.EndContinuousUpdate();
    this.m_aiAnimator.LockFacingDirection = false;
    this.m_aiActor.CollisionDamage = 0.5f;
    this.m_aiActor.knockbackDoer.weight = 35f;
    this.m_hasFired = true;
    this.m_aiAnimator.spriteAnimator.AnimationEventTriggered -= new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.AnimEventTriggered);
  }

  private void ShootBulletScript()
  {
    if (!(bool) (UnityEngine.Object) this.m_bulletScriptSource)
      this.m_bulletScriptSource = this.ShootPoint.GetOrAddComponent<BulletScriptSource>();
    this.m_bulletScriptSource.BulletManager = this.m_aiActor.bulletBank;
    this.m_bulletScriptSource.BulletScript = this.BulletScript;
    this.m_bulletScriptSource.Initialize();
  }

  private void AnimEventTriggered(
    tk2dSpriteAnimator sprite,
    tk2dSpriteAnimationClip clip,
    int frameNum)
  {
    if (!(clip.GetFrame(frameNum).eventInfo == "fire"))
      return;
    this.ShootBulletScript();
  }
}
