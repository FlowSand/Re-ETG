// Decompiled with JetBrains decompiler
// Type: PowderSkullParticleController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class PowderSkullParticleController : BraveBehaviour
    {
      public AIAnimator ParentAnimator;
      public Transform RotationChild;
      public float VelocityFraction = 0.7f;
      private float m_rotationChildInitialRotation;
      private ParticleSystem m_system;
      private ParticleSystem.Particle[] m_particles;
      private Vector3 m_curPosition;
      private Vector3 m_lastPosition;

      public void Start()
      {
        this.m_lastPosition = this.transform.position;
        this.m_system = this.GetComponent<ParticleSystem>();
        if (this.m_particles == null)
          this.m_particles = new ParticleSystem.Particle[this.m_system.maxParticles];
        if (!((Object) this.RotationChild != (Object) null))
          return;
        this.m_rotationChildInitialRotation = this.RotationChild.localEulerAngles.x;
      }

      public void LateUpdate()
      {
        this.m_curPosition = this.transform.position;
        if ((Object) this.RotationChild != (Object) null && (Object) this.ParentAnimator != (Object) null)
          this.RotationChild.localRotation = Quaternion.Euler(this.m_rotationChildInitialRotation + (float) (BraveMathCollege.AngleToOctant(this.ParentAnimator.FacingDirection) * 45), 0.0f, 0.0f);
        Vector3 vector3 = this.m_curPosition - this.m_lastPosition;
        if ((double) BraveTime.DeltaTime > 0.0 && vector3 != Vector3.zero)
        {
          int particles = this.m_system.GetParticles(this.m_particles);
          for (int index = 0; index < particles; ++index)
            this.m_particles[index].position += vector3 * this.VelocityFraction;
          this.m_system.SetParticles(this.m_particles, particles);
        }
        this.m_lastPosition = this.m_curPosition;
      }
    }

}
