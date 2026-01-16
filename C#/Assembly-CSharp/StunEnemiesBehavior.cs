// Decompiled with JetBrains decompiler
// Type: StunEnemiesBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable
public class StunEnemiesBehavior : AttackBehaviorBase
{
  public float StunDuration = 1f;
  public float Cooldown;
  public float minAttackDistance = 0.1f;
  public float maxAttackDistance = 1f;
  public string AnimationName;
  public GameObject StunVFX;
  private float m_cooldownTimer;
  private float m_minAttackDistance;
  private float m_maxAttackDistance;

  public override void Start()
  {
    base.Start();
    this.m_minAttackDistance = this.minAttackDistance;
    this.m_maxAttackDistance = this.maxAttackDistance;
  }

  public override BehaviorResult Update()
  {
    int num1 = (int) base.Update();
    bool flag = false;
    this.DecrementTimer(ref this.m_cooldownTimer);
    BehaviorResult behaviorResult = base.Update();
    if (behaviorResult != BehaviorResult.Continue)
      return behaviorResult;
    if ((double) this.m_cooldownTimer > 0.0 || !(bool) (Object) this.m_aiActor.TargetRigidbody)
      return BehaviorResult.Continue;
    SpeculativeRigidbody targetRigidbody = this.m_aiActor.TargetRigidbody;
    AIActor aiActor = targetRigidbody.aiActor;
    if ((bool) (Object) aiActor)
    {
      if (!aiActor.IsNormalEnemy)
        return BehaviorResult.Continue;
      HealthHaver healthHaver = targetRigidbody.healthHaver;
      if ((bool) (Object) healthHaver)
      {
        if (!healthHaver.IsVulnerable)
          return BehaviorResult.Continue;
        if (healthHaver.IsBoss)
          flag = GameManager.Instance.Dungeon.CellSupportsFalling((Vector3) targetRigidbody.UnitCenter);
      }
    }
    this.m_minAttackDistance = this.minAttackDistance;
    this.m_maxAttackDistance = this.maxAttackDistance;
    if (flag)
    {
      this.m_minAttackDistance = this.minAttackDistance;
      this.m_maxAttackDistance = this.maxAttackDistance + 1f;
    }
    Vector2 unitCenter = this.m_aiActor.specRigidbody.UnitCenter;
    Vector2 targetPoint = this.GetTargetPoint(this.m_aiActor.TargetRigidbody, unitCenter);
    float num2 = Vector2.Distance(unitCenter, targetPoint);
    bool lineOfSightToTarget = this.m_aiActor.HasLineOfSightToTarget;
    if ((double) num2 < (double) this.maxAttackDistance && lineOfSightToTarget)
    {
      BehaviorSpeculator component = targetRigidbody.GetComponent<BehaviorSpeculator>();
      if ((bool) (Object) component)
      {
        if (!string.IsNullOrEmpty(this.AnimationName) && !this.m_aiAnimator.IsPlaying(this.AnimationName))
        {
          this.m_aiAnimator.PlayUntilFinished(this.AnimationName);
          if ((bool) (Object) this.StunVFX)
            this.m_aiActor.StartCoroutine(this.HandleDelayedSpawnStunVFX(targetPoint));
        }
        component.Stun(this.StunDuration);
        this.m_cooldownTimer = this.Cooldown;
        this.m_updateEveryFrame = true;
        return BehaviorResult.RunContinuous;
      }
    }
    return BehaviorResult.Continue;
  }

  [DebuggerHidden]
  private IEnumerator HandleDelayedSpawnStunVFX(Vector2 targetPoint)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new StunEnemiesBehavior.\u003CHandleDelayedSpawnStunVFX\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  public override ContinuousBehaviorResult ContinuousUpdate()
  {
    SpeculativeRigidbody targetRigidbody = this.m_aiActor.TargetRigidbody;
    if (!(bool) (Object) targetRigidbody)
      return ContinuousBehaviorResult.Finished;
    bool flag = false;
    if ((bool) (Object) targetRigidbody && (bool) (Object) targetRigidbody.aiActor && (bool) (Object) targetRigidbody.healthHaver && targetRigidbody.healthHaver.IsBoss)
      flag = GameManager.Instance.Dungeon.CellSupportsFalling((Vector3) targetRigidbody.UnitCenter);
    this.m_minAttackDistance = this.minAttackDistance;
    this.m_maxAttackDistance = this.maxAttackDistance;
    if (flag)
    {
      this.m_minAttackDistance = this.minAttackDistance;
      this.m_maxAttackDistance = this.maxAttackDistance + 1f;
    }
    Vector2 unitCenter = this.m_aiActor.specRigidbody.UnitCenter;
    Vector2 targetPoint = this.GetTargetPoint(this.m_aiActor.TargetRigidbody, unitCenter);
    if ((double) Vector2.Distance(unitCenter, targetPoint) > (double) this.maxAttackDistance)
      return ContinuousBehaviorResult.Finished;
    this.m_aiActor.ClearPath();
    BehaviorSpeculator component = targetRigidbody.GetComponent<BehaviorSpeculator>();
    if ((bool) (Object) component)
    {
      if ((bool) (Object) component.healthHaver && !component.healthHaver.IsVulnerable)
        return ContinuousBehaviorResult.Finished;
      if (!string.IsNullOrEmpty(this.AnimationName) && !this.m_aiAnimator.IsPlaying(this.AnimationName))
      {
        this.m_aiAnimator.PlayUntilFinished(this.AnimationName);
        if ((bool) (Object) this.StunVFX)
          this.m_aiActor.StartCoroutine(this.HandleDelayedSpawnStunVFX(targetPoint));
      }
      if (component.IsStunned)
        component.UpdateStun(this.StunDuration);
      else
        component.Stun(this.StunDuration);
      this.m_cooldownTimer = this.Cooldown;
    }
    return ContinuousBehaviorResult.Continue;
  }

  public override void EndContinuousUpdate()
  {
    base.EndContinuousUpdate();
    this.m_aiActor.BehaviorOverridesVelocity = false;
    this.m_aiAnimator.LockFacingDirection = false;
    this.m_updateEveryFrame = false;
  }

  private Vector2 GetTargetPoint(SpeculativeRigidbody targetRigidbody, Vector2 myCenter)
  {
    PixelCollider hitboxPixelCollider = targetRigidbody.HitboxPixelCollider;
    return BraveMathCollege.ClosestPointOnRectangle(myCenter, hitboxPixelCollider.UnitBottomLeft, hitboxPixelCollider.UnitDimensions);
  }

  public override bool IsReady() => true;

  public override float GetMinReadyRange() => this.m_maxAttackDistance;

  public override float GetMaxRange() => this.m_maxAttackDistance;
}
