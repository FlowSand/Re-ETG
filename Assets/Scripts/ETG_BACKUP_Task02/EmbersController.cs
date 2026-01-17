// Decompiled with JetBrains decompiler
// Type: EmbersController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
[ExecuteInEditMode]
public class EmbersController : MonoBehaviour
{
  private ParticleSystem m_system;
  private ParticleSystem.Particle[] m_particles;
  [NonSerialized]
  public List<Vector4> AdditionalVortices = new List<Vector4>();
  public float VortexScale = 1.5f;
  public float VortexSpeed = 2f;

  private void Start()
  {
    this.m_system = this.GetComponent<ParticleSystem>();
    if (this.m_particles != null)
      return;
    this.m_particles = new ParticleSystem.Particle[this.m_system.maxParticles];
  }

  private void Update() => this.ProcessParticles();

  private void ProcessVortex(
    int particleIndex,
    Vector2 particlePos,
    Vector2 vortex,
    float vortexScale,
    float speed)
  {
    float num1 = particlePos.x - vortex.x;
    Vector2 vector2 = new Vector2(particlePos.y - vortex.y, -num1);
    float num2 = Mathf.Clamp01((float) (1.0 - (double) vector2.magnitude / (double) vortexScale));
    Vector3 vector3 = vector2.normalized.ToVector3ZUp() * num2 * speed;
    this.m_particles[particleIndex].velocity += vector3;
  }

  private void ProcessParticles()
  {
    int particles = this.m_system.GetParticles(this.m_particles);
    float vortexScale = this.VortexScale;
    for (int particleIndex = 0; particleIndex < particles; ++particleIndex)
    {
      Vector3 position = this.m_particles[particleIndex].position;
      Vector2 vector2 = position.XY().Quantize(2f);
      float num1 = position.x - vector2.x;
      float num2 = position.y - vector2.y;
      float num3 = Mathf.Sin(position.x + position.y);
      float num4 = vortexScale * Mathf.Lerp(0.75f, 1.75f, (float) (((double) Mathf.Cos(position.x + position.y) + 1.0) / 2.0));
      float num5 = this.VortexSpeed * Mathf.Lerp(0.75f, 1.75f, (float) (((double) num3 + 1.0) / 2.0));
      if ((double) num3 > 0.0)
        num5 *= -1f;
      float num6 = -num2 * num5;
      float num7 = num1 * num5;
      float num8 = (float) (1.0 / (1.0 + ((double) num1 * (double) num1 + (double) num2 * (double) num2) / (double) num4));
      Vector3 vector3 = new Vector3(num6 - this.m_particles[particleIndex].velocity.x, num7 - this.m_particles[particleIndex].velocity.y, 0.0f) * num8;
      this.m_particles[particleIndex].velocity += vector3;
      if (this.AdditionalVortices.Count != 0)
      {
        for (int index = 0; index < this.AdditionalVortices.Count; ++index)
          this.ProcessVortex(particleIndex, (Vector2) position, new Vector2(this.AdditionalVortices[index].x, this.AdditionalVortices[index].y), this.AdditionalVortices[index].z, this.AdditionalVortices[index].w);
      }
    }
    this.m_system.SetParticles(this.m_particles, particles);
  }
}
