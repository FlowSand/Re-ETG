using Dungeonator;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

[ExecuteInEditMode]
public class HelicopterSnowController : MonoBehaviour
  {
    private ParticleSystem m_system;
    private ParticleSystem.Particle[] m_particles;
    public Vector3 WorldSpaceVortexCenter;
    public float VortexRadius = 5f;
    public float VortexSpeed = 5f;
    private AIActor m_helicopter;

    private void Start()
    {
      this.m_system = this.GetComponent<ParticleSystem>();
      if (this.m_particles != null)
        return;
      this.m_particles = new ParticleSystem.Particle[this.m_system.main.maxParticles];
    }

    private void OnEnable()
    {
      RoomHandler absoluteRoom = this.transform.position.GetAbsoluteRoom();
      if (absoluteRoom == null)
        return;
      List<AIActor> activeEnemies = absoluteRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
      if (activeEnemies == null || activeEnemies.Count <= 0)
        return;
      for (int index = 0; index < activeEnemies.Count; ++index)
      {
        if ((bool) (Object) activeEnemies[index] && (bool) (Object) activeEnemies[index].healthHaver && activeEnemies[index].healthHaver.IsBoss)
          this.m_helicopter = activeEnemies[index];
      }
    }

    private void Update() => this.ProcessParticles();

    private void ProcessParticles()
    {
      int particles = this.m_system.GetParticles(this.m_particles);
      if ((bool) (Object) this.m_helicopter)
        this.WorldSpaceVortexCenter = (Vector3) (this.m_helicopter.specRigidbody.UnitCenter + new Vector2(0.0f, 1.5f));
      float num1 = this.VortexRadius * this.VortexRadius;
      if (!(bool) (Object) this.m_helicopter)
        num1 = -1f;
      for (int index = 0; index < particles; ++index)
      {
        Vector3 position = this.m_particles[index].position;
        Vector3 spaceVortexCenter = this.WorldSpaceVortexCenter;
        float num2 = position.x - spaceVortexCenter.x;
        float num3 = position.y - spaceVortexCenter.y;
        float num4 = (float) ((double) num2 * (double) num2 + (double) num3 * (double) num3);
        if ((double) num4 < (double) num1)
        {
          float vortexSpeed = this.VortexSpeed;
          Vector3 vector3 = new Vector3(-num3 * vortexSpeed, num2 * vortexSpeed, 0.0f) * (float) (1.0 / (1.0 + (double) num4 / (double) num1));
          this.m_particles[index].velocity += vector3;
        }
      }
      this.m_system.SetParticles(this.m_particles, particles);
    }
  }

