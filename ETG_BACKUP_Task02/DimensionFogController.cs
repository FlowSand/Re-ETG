// Decompiled with JetBrains decompiler
// Type: DimensionFogController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class DimensionFogController : BraveBehaviour
{
  public float radius;
  public float minRadius = 4f;
  public float growSpeed = 8f;
  public float contractSpeed = 1f;
  public float targetRadius;
  [Header("Main Particle System")]
  public ParticleSystem particleSystem;
  public float emissionRatePerArea = 0.2f;
  public float speedPerRadius = 0.33f;
  [Header("Bits Particle System")]
  public ParticleSystem bitsParticleSystem;
  public float bitsEmissionRatePerRadius = 5f;
  private DimensionFogController.State m_state = DimensionFogController.State.Contracting;

  public float ApparentRadius
  {
    get
    {
      return this.m_state == DimensionFogController.State.Growing ? Mathf.Max(0.0f, this.radius - 6f) : this.radius;
    }
  }

  public void Start()
  {
    BraveUtility.EnableEmission(this.particleSystem, false);
    BraveUtility.EnableEmission(this.bitsParticleSystem, false);
  }

  public void Update()
  {
    if (this.m_state == DimensionFogController.State.Growing)
    {
      this.radius = Mathf.MoveTowards(this.radius, this.targetRadius, this.growSpeed * BraveTime.DeltaTime);
      if ((double) this.radius >= (double) this.targetRadius)
      {
        this.targetRadius = 0.0f;
        this.m_state = DimensionFogController.State.Contracting;
      }
    }
    else if (this.m_state == DimensionFogController.State.Contracting)
    {
      this.radius = Mathf.MoveTowards(this.radius, this.minRadius, this.contractSpeed * BraveTime.DeltaTime);
      if ((double) this.radius <= (double) this.minRadius)
      {
        this.radius = 0.0f;
        this.targetRadius = 0.0f;
        this.m_state = DimensionFogController.State.Stable;
      }
    }
    else if (this.m_state == DimensionFogController.State.Stable && (double) this.targetRadius > 0.0)
    {
      this.radius = this.minRadius;
      this.m_state = DimensionFogController.State.Growing;
    }
    this.UpdateParticleSystem();
    this.UpdateBitsParticleSystem();
  }

  private void UpdateParticleSystem()
  {
    float emissionRate = this.emissionRatePerArea * (3.14159274f * this.radius * this.radius);
    BraveUtility.SetEmissionRate(this.particleSystem, emissionRate);
    this.particleSystem.startSpeed = this.speedPerRadius * this.radius;
    Vector3 vector3 = Quaternion.Euler(0.0f, 0.0f, (float) Random.Range(0, 360)) * new Vector3(this.radius, 0.0f);
    this.particleSystem.Emit(new ParticleSystem.EmitParams()
    {
      position = vector3,
      velocity = this.particleSystem.startSpeed * -vector3.normalized,
      startSize = this.particleSystem.startSize,
      startLifetime = this.particleSystem.startLifetime,
      startColor = (Color32) this.particleSystem.startColor
    }, (int) ((double) BraveTime.DeltaTime * (double) emissionRate));
  }

  private void UpdateBitsParticleSystem()
  {
    if (!(bool) (Object) this.bitsParticleSystem)
      return;
    float emissionRate = this.bitsEmissionRatePerRadius * this.radius;
    BraveUtility.SetEmissionRate(this.bitsParticleSystem, emissionRate);
    Vector3 vector3 = Quaternion.Euler(0.0f, 0.0f, (float) Random.Range(0, 360)) * new Vector3(this.radius, 0.0f);
    this.particleSystem.Emit(new ParticleSystem.EmitParams()
    {
      position = vector3,
      velocity = this.bitsParticleSystem.startSpeed * vector3.normalized,
      startSize = this.bitsParticleSystem.startSize,
      startLifetime = this.bitsParticleSystem.startLifetime,
      startColor = (Color32) this.bitsParticleSystem.startColor
    }, (int) ((double) BraveTime.DeltaTime * (double) emissionRate));
  }

  private enum State
  {
    Growing,
    Contracting,
    Stable,
  }
}
