// Decompiled with JetBrains decompiler
// Type: MomentumProjectile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class MomentumProjectile : Projectile
{
  public float momentumFraction = 0.35f;

  public override void Start()
  {
    base.Start();
    if (!(bool) (Object) this.Owner || !(bool) (Object) this.Owner.specRigidbody)
      return;
    this.m_currentDirection = this.m_currentDirection.normalized * (1f - this.momentumFraction) + this.Owner.specRigidbody.Velocity.normalized * this.momentumFraction;
    this.m_currentDirection = this.m_currentDirection.normalized;
  }

  protected override void Move()
  {
    this.m_timeElapsed += this.LocalDeltaTime;
    if ((double) this.angularVelocity != 0.0)
      this.transform.RotateAround((Vector3) this.transform.position.XY(), Vector3.forward, this.angularVelocity * this.LocalDeltaTime);
    if (this.baseData.UsesCustomAccelerationCurve)
      this.m_currentSpeed = this.baseData.AccelerationCurve.Evaluate(Mathf.Clamp01((this.m_timeElapsed - this.baseData.IgnoreAccelCurveTime) / this.baseData.CustomAccelerationCurveDuration)) * this.baseData.speed;
    this.specRigidbody.Velocity = this.m_currentDirection * this.m_currentSpeed;
    this.m_currentSpeed *= (float) (1.0 - (double) this.baseData.damping * (double) this.LocalDeltaTime);
    this.LastVelocity = this.specRigidbody.Velocity;
  }

  protected override void OnDestroy() => base.OnDestroy();
}
