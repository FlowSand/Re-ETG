// Decompiled with JetBrains decompiler
// Type: AdditionalBraveLight
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class AdditionalBraveLight : BraveBehaviour
    {
      public Color LightColor = Color.white;
      public float LightIntensity = 3f;
      public float LightRadius = 5f;
      public bool FadeOnActorDeath;
      [ShowInInspectorIf("FadeOnActorDeath", true)]
      public AIActor SpecifyActor;
      public bool TriggeredOnBulletBank;
      public bool UseProjectileCreatedEvent;
      public AIBulletBank RelevantBulletBank;
      public float BulletBankHoldTime;
      public float BulletBankFadeTime = 0.5f;
      public string BulletBankTransformName;
      public float BulletBankIntensity = 3f;
      public bool UsesCone;
      public float LightAngle = 180f;
      public float LightOrient;
      public bool UsesCustomMaterial;
      public Material CustomLightMaterial;
      private bool m_initialized;
      private Coroutine m_activeCoroutine;
      private bool isFading;

      private void Awake()
      {
        if (!this.TriggeredOnBulletBank)
          return;
        this.LightIntensity = 0.0f;
      }

      [DebuggerHidden]
      public IEnumerator Start()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new AdditionalBraveLight.<Start>c__Iterator0()
        {
          $this = this
        };
      }

      public void Initialize()
      {
        if (this.m_initialized)
          return;
        if (this.TriggeredOnBulletBank && (UnityEngine.Object) this.RelevantBulletBank != (UnityEngine.Object) null)
        {
          if (this.UseProjectileCreatedEvent)
            this.RelevantBulletBank.OnProjectileCreatedWithSource += new Action<string, Projectile>(this.HandleProjectileCreated);
          else
            this.RelevantBulletBank.OnBulletSpawned += new Action<Bullet, Projectile>(this.HandleBulletSpawned);
        }
        Pixelator.Instance.AdditionalBraveLights.Add(this);
        if (this.FadeOnActorDeath)
        {
          if (!(bool) (UnityEngine.Object) this.SpecifyActor)
            this.SpecifyActor = this.aiActor;
          this.SpecifyActor.healthHaver.OnPreDeath += new Action<Vector2>(this.OnPreDeath);
        }
        this.m_initialized = true;
      }

      private void HandleProjectileCreated(string arg1, Projectile arg2)
      {
        if (!(arg1 == this.BulletBankTransformName))
          return;
        if (this.m_activeCoroutine != null)
          this.StopCoroutine(this.m_activeCoroutine);
        this.m_activeCoroutine = this.StartCoroutine(this.HandleBulletBankFade());
      }

      public void ManuallyDoBulletSpawnedFade()
      {
        if (this.m_activeCoroutine != null)
          this.StopCoroutine(this.m_activeCoroutine);
        this.m_activeCoroutine = this.StartCoroutine(this.HandleBulletBankFade());
      }

      public void EndEarly()
      {
        if (this.m_activeCoroutine == null)
          return;
        this.isFading = true;
      }

      private void HandleBulletSpawned(Bullet arg1, Projectile arg2)
      {
        if (!((UnityEngine.Object) arg1.RootTransform != (UnityEngine.Object) null) || !(this.BulletBankTransformName == arg1.RootTransform.name))
          return;
        if (this.m_activeCoroutine != null)
          this.StopCoroutine(this.m_activeCoroutine);
        this.m_activeCoroutine = this.StartCoroutine(this.HandleBulletBankFade());
      }

      [DebuggerHidden]
      private IEnumerator HandleBulletBankFade()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new AdditionalBraveLight.<HandleBulletBankFade>c__Iterator1()
        {
          $this = this
        };
      }

      protected override void OnDestroy()
      {
        if (Pixelator.HasInstance)
          Pixelator.Instance.AdditionalBraveLights.Remove(this);
        if (this.FadeOnActorDeath)
          this.SpecifyActor.healthHaver.OnPreDeath -= new Action<Vector2>(this.OnPreDeath);
        base.OnDestroy();
      }

      private void OnPreDeath(Vector2 deathDir)
      {
        this.StartCoroutine(this.FadeLight(this.SpecifyActor.healthHaver.GetDeathClip(deathDir.ToAngle()).BaseClipLength));
      }

      [DebuggerHidden]
      private IEnumerator FadeLight(float fadeTime)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new AdditionalBraveLight.<FadeLight>c__Iterator2()
        {
          fadeTime = fadeTime,
          $this = this
        };
      }
    }

}
