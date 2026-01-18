// Decompiled with JetBrains decompiler
// Type: CrowdOfFansSystemController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using UnityEngine;

#nullable disable

public class CrowdOfFansSystemController : MonoBehaviour
  {
    public PlayerController Target;
    public int MaxFans = 100;
    private ParticleSystem m_system;
    private ParticleSystem.Particle[] m_particles;
    private Vector2[] m_offsets;
    private bool m_initialized;
    private int m_numEmitted;

    private void Start()
    {
      this.m_system = this.GetComponent<ParticleSystem>();
      if (this.m_particles == null)
        this.m_particles = new ParticleSystem.Particle[this.m_system.maxParticles];
      this.m_offsets = new Vector2[this.MaxFans];
      for (int index = 0; index < this.MaxFans; ++index)
        this.m_offsets[index] = Random.insideUnitCircle * 3f;
      this.m_system.Play();
    }

    public void Initialize(PlayerController p)
    {
      this.m_initialized = true;
      this.Target = p;
    }

    private void Update()
    {
      if (Dungeon.IsGenerating || !this.m_initialized)
        return;
      this.ProcessParticles();
    }

    private void ProcessParticles()
    {
      if (this.m_numEmitted < 10)
      {
        this.m_system.Emit(new ParticleSystem.EmitParams()
        {
          position = (this.Target.CenterPosition + this.m_offsets[this.m_numEmitted]).ToVector3ZisY(),
          velocity = Vector3.zero,
          startSize = this.m_system.startSize,
          startLifetime = this.m_system.startLifetime,
          startColor = (Color32) this.m_system.startColor
        }, 1);
        Debug.LogError((object) "emitting particle");
        ++this.m_numEmitted;
      }
      int particles = this.m_system.GetParticles(this.m_particles);
      for (int index = 0; index < particles; ++index)
      {
        Vector3 position = this.m_particles[index].position;
        Vector3 velocity = this.m_particles[index].velocity;
        Vector3 vector3ZisY = (this.Target.CenterPosition + this.m_offsets[index]).ToVector3ZisY();
        this.m_particles[index].position = vector3ZisY;
        this.m_particles[index].velocity = Vector3.zero;
      }
      this.m_system.SetParticles(this.m_particles, particles);
    }
  }

