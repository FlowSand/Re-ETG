// Decompiled with JetBrains decompiler
// Type: BulletCurtainParticleController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[ExecuteInEditMode]
public class BulletCurtainParticleController : MonoBehaviour
{
  public float LocalXMin = 3.5f;
  public float LocalXMax = 4.5f;
  public float LocalYMax = 2.5f;
  public float AccelFactor = 1f;
  public Transform rootTransform;
  private ParticleSystem m_system;
  private ParticleSystem.Particle[] m_particles;

  private void Start()
  {
    this.m_system = this.GetComponent<ParticleSystem>();
    if (this.m_particles != null)
      return;
    this.m_particles = new ParticleSystem.Particle[this.m_system.maxParticles];
  }

  private void Update() => this.ProcessParticles();

  private void ProcessParticles()
  {
    int particles = this.m_system.GetParticles(this.m_particles);
    Transform transform = this.m_system.transform;
    float num1 = 0.0f;
    if (Application.isPlaying)
      num1 = BraveTime.DeltaTime;
    for (int index = 0; index < particles; ++index)
    {
      Vector3 vector3 = transform.TransformPoint(this.m_particles[index].position);
      vector3 -= this.rootTransform.position;
      float num2 = (float) ((double) (this.m_particles[index].randomSeed % 30U) / 30.0 * 0.5);
      if (((double) vector3.x < (double) this.LocalXMin - (double) num2 || (double) vector3.x > (double) this.LocalXMax + (double) num2) && (double) vector3.y > 1.25)
        this.m_particles[index].velocity = this.m_particles[index].velocity.WithX(0.0f);
      else if ((double) vector3.x > (double) this.LocalXMin && (double) vector3.x < (double) this.LocalXMax && (double) vector3.y < (double) this.LocalYMax)
      {
        if ((double) vector3.x > ((double) this.LocalXMin + (double) this.LocalXMax) / 2.0)
          this.m_particles[index].velocity += Vector3.right * num1 * this.AccelFactor;
        else
          this.m_particles[index].velocity += Vector3.right * -1f * num1 * this.AccelFactor;
      }
    }
    this.m_system.SetParticles(this.m_particles, particles);
  }
}
