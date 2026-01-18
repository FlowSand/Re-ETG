using UnityEngine;

#nullable disable

[ExecuteInEditMode]
public class RedMatterParticleController : MonoBehaviour
    {
        private ParticleSystem m_system;
        private ParticleSystem.Particle[] m_particles;
        public Transform target;
        public float VortexScale = 1.5f;
        public float VortexSpeed = 2f;

        private void Awake()
        {
            this.m_system = this.GetComponent<ParticleSystem>();
            if (this.m_particles != null)
                return;
            this.m_particles = new ParticleSystem.Particle[this.m_system.maxParticles];
        }

        public void ProcessParticles()
        {
            int particles = this.m_system.GetParticles(this.m_particles);
            float vortexScale = this.VortexScale;
            for (int index = 0; index < particles; ++index)
            {
                Vector3 position = this.m_particles[index].position;
                float t1 = Mathf.Lerp(0.0f, 1f, (float) (1.0 - ((double) this.m_particles[index].remainingLifetime - ((double) this.m_particles[index].startLifetime - 1.0))));
                float t2 = (float) (1.0 - ((double) this.m_particles[index].remainingLifetime - 0.5) / (double) this.m_particles[index].startLifetime);
                Vector3 b = !((Object) this.target == (Object) null) ? (this.target.position - position).normalized * Mathf.Lerp(this.m_particles[index].velocity.magnitude, (this.target.position - position).magnitude * 10f, t2) : this.m_particles[index].velocity;
                this.m_particles[index].velocity = Vector3.Lerp(this.m_particles[index].velocity, b, t1);
                if ((double) (this.target.position - position).sqrMagnitude <= 1.0)
                    this.m_particles[index].remainingLifetime -= 0.1f;
            }
            this.m_system.SetParticles(this.m_particles, particles);
        }
    }

