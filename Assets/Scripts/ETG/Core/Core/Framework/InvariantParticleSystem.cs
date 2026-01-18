using UnityEngine;

#nullable disable

[RequireComponent(typeof (ParticleSystem))]
public class InvariantParticleSystem : BraveBehaviour
    {
        private ParticleSystem m_particleSystem;

        public void Awake() => this.m_particleSystem = this.GetComponent<ParticleSystem>();

        public void Update()
        {
            this.m_particleSystem.Simulate(GameManager.INVARIANT_DELTA_TIME, true, false);
        }
    }

