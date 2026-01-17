// Decompiled with JetBrains decompiler
// Type: ProjectileTrailRendererController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable
public class ProjectileTrailRendererController : BraveBehaviour
{
  public TrailRenderer trailRenderer;
  public CustomTrailRenderer customTrailRenderer;
  public float desiredLength;
  private float? m_previousTrailSpeed;
  private bool isStopping;

  public void Awake()
  {
    this.m_previousTrailSpeed = new float?();
    this.projectile.TrailRendererController = this;
  }

  public void Start() => this.TryUpdateTrailLength();

  public void LateUpdate() => this.TryUpdateTrailLength();

  public void OnSpawned()
  {
    if ((bool) (Object) this.customTrailRenderer)
      this.customTrailRenderer.Reenable();
    this.TryUpdateTrailLength();
  }

  public void OnDespawned()
  {
    this.m_previousTrailSpeed = new float?();
    if (!(bool) (Object) this.customTrailRenderer)
      return;
    this.customTrailRenderer.Clear();
    this.isStopping = false;
    this.StopAllCoroutines();
  }

  public void Stop()
  {
    if (!(bool) (Object) this.customTrailRenderer)
      return;
    this.StartCoroutine(this.StopGracefully());
  }

  [DebuggerHidden]
  private IEnumerator StopGracefully()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new ProjectileTrailRendererController.\u003CStopGracefully\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  private void TryUpdateTrailLength()
  {
    if (this.isStopping)
      return;
    float? nullable = new float?();
    if (!nullable.HasValue && (bool) (Object) this.projectile.braveBulletScript && this.projectile.braveBulletScript.bullet != null && !this.projectile.braveBulletScript.bullet.ManualControl)
      nullable = new float?(this.projectile.braveBulletScript.bullet.Speed);
    if (!nullable.HasValue && (bool) (Object) this.specRigidbody)
      nullable = new float?(this.specRigidbody.Velocity.magnitude);
    if (!nullable.HasValue || this.m_previousTrailSpeed.HasValue && ((double) nullable.GetValueOrDefault() != (double) this.m_previousTrailSpeed.Value ? 1 : (!nullable.HasValue ? 1 : 0)) == 0)
      return;
    this.m_previousTrailSpeed = nullable;
    if ((bool) (Object) this.trailRenderer)
      this.trailRenderer.time = ((double) nullable.GetValueOrDefault() != 0.0 ? 0 : (nullable.HasValue ? 1 : 0)) == 0 ? this.desiredLength / nullable.Value : this.desiredLength;
    if (!(bool) (Object) this.customTrailRenderer)
      return;
    this.customTrailRenderer.lifeTime = ((double) nullable.GetValueOrDefault() != 0.0 ? 0 : (nullable.HasValue ? 1 : 0)) == 0 ? this.desiredLength / nullable.Value : this.desiredLength;
  }
}
