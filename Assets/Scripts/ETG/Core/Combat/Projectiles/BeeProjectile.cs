using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class BeeProjectile : Projectile
  {
    public float angularAcceleration = 10f;
    public float searchRadius = 10f;
    public GameActor CurrentTarget;
    protected bool m_coroutineIsActive;
    protected AIActor m_previouslyHitEnemy;

    public override void Start()
    {
      base.Start();
      BeeProjectile beeProjectile1 = this;
      beeProjectile1.OnHitEnemy = beeProjectile1.OnHitEnemy + new Action<Projectile, SpeculativeRigidbody, bool>(this.HandleHitEnemy);
      BeeProjectile beeProjectile2 = this;
      beeProjectile2.ModifyVelocity = beeProjectile2.ModifyVelocity + new Func<Vector2, Vector2>(this.ModifyVelocityLocal);
    }

    private void HandleHitEnemy(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
    {
      this.m_previouslyHitEnemy = (AIActor) null;
      this.CurrentTarget = (GameActor) null;
      if (!(bool) (UnityEngine.Object) arg2 || !(bool) (UnityEngine.Object) arg2.aiActor)
        return;
      this.m_previouslyHitEnemy = arg2.aiActor;
    }

    [DebuggerHidden]
    private IEnumerator FindTarget()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new BeeProjectile__FindTargetc__Iterator0()
      {
        _this = this
      };
    }

    private Vector2 ModifyVelocityLocal(Vector2 inVel)
    {
      if (!this.m_coroutineIsActive)
        this.StartCoroutine(this.FindTarget());
      float num = 1f;
      inVel = this.m_currentDirection;
      Vector2 from = inVel;
      if ((UnityEngine.Object) this.CurrentTarget != (UnityEngine.Object) null && !this.CurrentTarget.IsGone)
      {
        Vector2 normalized = (this.CurrentTarget.specRigidbody.GetUnitCenter(ColliderType.HitBox) - this.specRigidbody.UnitCenter).normalized;
        from = Vector3.RotateTowards((Vector3) inVel, (Vector3) normalized, this.angularAcceleration * ((float) Math.PI / 180f) * BraveTime.DeltaTime, 0.0f).XY().normalized;
        num = (float) (0.25 + (1.0 - (double) Mathf.Clamp01(Mathf.Abs(Vector2.Angle(from, normalized)) / 60f)) * 0.75);
      }
      Vector2 v = from * this.m_currentSpeed * num;
      if (this.OverrideMotionModule != null)
        this.OverrideMotionModule.AdjustRightVector(Mathf.DeltaAngle(BraveMathCollege.Atan2Degrees(inVel), BraveMathCollege.Atan2Degrees(v)));
      return v;
    }

    protected override void OnDestroy() => base.OnDestroy();
  }

