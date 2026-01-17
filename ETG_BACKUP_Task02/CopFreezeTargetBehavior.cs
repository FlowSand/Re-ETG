// Decompiled with JetBrains decompiler
// Type: CopFreezeTargetBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class CopFreezeTargetBehavior : BasicAttackBehavior
{
  public float FreezeRadius = 7f;
  public float FreezeDelayTime = 2f;
  public GameActorFreezeEffect FreezeEffect;
  private float m_freezeTimer;

  public override void Start() => base.Start();

  public override void Upkeep() => base.Upkeep();

  public override BehaviorResult Update()
  {
    BehaviorResult behaviorResult = base.Update();
    if (behaviorResult != BehaviorResult.Continue)
      return behaviorResult;
    if (!this.IsReady() || (Object) this.m_aiActor.TargetRigidbody == (Object) null)
      return BehaviorResult.Continue;
    float num = Vector2.Distance(this.m_aiActor.CenterPosition, this.m_aiActor.TargetRigidbody.UnitCenter);
    if ((double) num > (double) this.FreezeRadius || (double) num < 2.0)
      return BehaviorResult.Continue;
    this.DoFreeze();
    this.m_aiAnimator.FacingDirection = (this.m_aiActor.TargetRigidbody.GetUnitCenter(ColliderType.HitBox) - this.m_aiActor.specRigidbody.GetUnitCenter(ColliderType.HitBox)).ToAngle();
    this.m_aiAnimator.LockFacingDirection = true;
    if (!this.m_aiAnimator.IsPlaying("freeze"))
      this.m_aiAnimator.PlayUntilCancelled("freeze");
    this.m_aiActor.ClearPath();
    this.m_freezeTimer = this.FreezeDelayTime;
    return BehaviorResult.RunContinuous;
  }

  private void DoFreeze()
  {
    if (!(bool) (Object) this.m_aiActor.TargetRigidbody.aiActor || this.m_aiActor.TargetRigidbody.aiActor.IsFrozen)
      return;
    Debug.Log((object) "DOING COP FREEZE");
    this.m_aiActor.TargetRigidbody.aiActor.ApplyEffect((GameActorEffect) this.FreezeEffect);
  }

  public override ContinuousBehaviorResult ContinuousUpdate()
  {
    int num = (int) base.ContinuousUpdate();
    this.DecrementTimer(ref this.m_freezeTimer);
    if ((double) this.m_freezeTimer <= 0.0 || (Object) this.m_aiActor.TargetRigidbody == (Object) null)
    {
      this.m_aiAnimator.EndAnimationIf("freeze");
      return ContinuousBehaviorResult.Finished;
    }
    this.DoFreeze();
    return ContinuousBehaviorResult.Continue;
  }

  public override void EndContinuousUpdate()
  {
    base.EndContinuousUpdate();
    if ((bool) (Object) this.m_aiShooter)
      this.m_aiShooter.ToggleGunRenderers(true, "ShootBulletScript");
    this.m_aiAnimator.LockFacingDirection = false;
    this.m_aiAnimator.EndAnimation();
    this.UpdateCooldowns();
  }

  public override void OnActorPreDeath() => base.OnActorPreDeath();
}
