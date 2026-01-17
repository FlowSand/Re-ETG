// Decompiled with JetBrains decompiler
// Type: ParticleKiller
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.VFX.Animation
{
    public class ParticleKiller : MonoBehaviour
    {
      public bool deparent;
      [ShowInInspectorIf("deparent", true)]
      public bool parentPosition = true;
      [ShowInInspectorIf("deparent", true)]
      public bool parentRotation = true;
      [ShowInInspectorIf("deparent", true)]
      public bool parentScale = true;
      [ShowInInspectorIf("deparent", true)]
      public bool parentPositionYDepth;
      public bool destroyAfterTimer;
      public float destroyTimer = 5f;
      public bool disableEmitterOnParentDeath;
      public bool destroyOnNoParticlesRemaining;
      public bool destroyAfterStartLifetime;
      public bool disableEmitterOnParentGrounded;
      public bool overrideXRotation;
      [ShowInInspectorIf("overrideXRotation", false)]
      public float xRotation;
      [Header("Manual Subemitter")]
      public bool transferToSubEmitter;
      public float transferToSubEmitterTimer;
      public ParticleSystem subEmitter;
      private ParticleSystem m_particleSystem;
      private DebrisObject m_debrisParent;
      private Transform m_parentTransform;
      private int m_noParticleCounter;
      private int c_framesOfNoParticlesToDestroy = 30;

      public void Awake()
      {
        if ((UnityEngine.Object) this.transform.parent == (UnityEngine.Object) null)
          return;
        this.ForceInit();
      }

      public void ForceInit()
      {
        this.m_particleSystem = this.GetComponent<ParticleSystem>();
        this.m_parentTransform = this.transform.parent;
        if ((bool) (UnityEngine.Object) this.transform.parent)
        {
          DebrisObject component = this.transform.parent.GetComponent<DebrisObject>();
          if ((bool) (UnityEngine.Object) component)
          {
            this.m_debrisParent = component;
            if (component.detachedParticleSystems == null)
              component.detachedParticleSystems = new List<ParticleSystem>();
            component.detachedParticleSystems.Add(this.m_particleSystem);
          }
        }
        if (this.destroyAfterStartLifetime)
          this.StartCoroutine(this.TimedDespawn(this.m_particleSystem.startLifetime));
        if (this.destroyAfterTimer)
          this.StartCoroutine(this.TimedDespawn(this.destroyTimer));
        if (this.deparent)
          this.transform.parent = SpawnManager.Instance.VFX;
        if (!this.transferToSubEmitter)
          return;
        this.StartCoroutine(this.TimedTransferToSubEmitter(this.transferToSubEmitterTimer));
      }

      private void Start()
      {
        if (!(bool) (UnityEngine.Object) this.m_debrisParent || !this.disableEmitterOnParentGrounded)
          return;
        this.m_debrisParent.OnGrounded += new Action<DebrisObject>(this.DisableOnParentGrounded);
      }

      public void Update()
      {
        if (this.deparent)
        {
          if ((bool) (UnityEngine.Object) this.m_parentTransform)
          {
            if (this.parentPosition)
            {
              if (this.parentPositionYDepth)
                this.transform.position = this.m_parentTransform.position.WithZ(this.m_parentTransform.position.y);
              else
                this.transform.position = this.m_parentTransform.position;
            }
            if (this.parentRotation)
              this.transform.rotation = this.m_parentTransform.rotation;
            if (this.parentScale)
              this.transform.localScale = this.m_parentTransform.localScale;
          }
          else
            BraveUtility.EnableEmission(this.m_particleSystem, false);
        }
        if (!this.destroyOnNoParticlesRemaining || !(bool) (UnityEngine.Object) this.m_particleSystem)
          return;
        if (this.m_particleSystem.particleCount == 0)
        {
          ++this.m_noParticleCounter;
          if (this.m_noParticleCounter < this.c_framesOfNoParticlesToDestroy)
            return;
          SpawnManager.Despawn(this.gameObject);
        }
        else
          this.m_noParticleCounter = 0;
      }

      public void StopEmitting() => BraveUtility.EnableEmission(this.m_particleSystem, false);

      [DebuggerHidden]
      private IEnumerator TimedDespawn(float t)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ParticleKiller.<TimedDespawn>c__Iterator0()
        {
          t = t,
          _this = this
        };
      }

      private void DisableOnParentGrounded(DebrisObject obj)
      {
        BraveUtility.EnableEmission(this.m_particleSystem, false);
      }

      [DebuggerHidden]
      private IEnumerator TimedTransferToSubEmitter(float t)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ParticleKiller.<TimedTransferToSubEmitter>c__Iterator1()
        {
          t = t,
          _this = this
        };
      }

      private void TransferToSubEmitter()
      {
        ParticleSystem.Particle[] particles1 = new ParticleSystem.Particle[this.m_particleSystem.particleCount];
        int particles2 = this.m_particleSystem.GetParticles(particles1);
        for (int index = 0; index < particles2; ++index)
        {
          ParticleSystem.Particle particle = particles1[index];
          this.subEmitter.Emit(new ParticleSystem.EmitParams()
          {
            position = particle.position,
            rotation = particle.rotation,
            startSize = particle.size,
            startColor = particle.color
          }, 1);
        }
        this.m_particleSystem.Clear(false);
        this.m_particleSystem.Stop();
      }
    }

}
