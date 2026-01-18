using System;
using System.Collections;
using System.Diagnostics;

using UnityEngine;

using Brave.BulletScript;

#nullable disable

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
            return (IEnumerator) new AdditionalBraveLight__Startc__Iterator0()
            {
                _this = this
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
            return (IEnumerator) new AdditionalBraveLight__HandleBulletBankFadec__Iterator1()
            {
                _this = this
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
            return (IEnumerator) new AdditionalBraveLight__FadeLightc__Iterator2()
            {
                fadeTime = fadeTime,
                _this = this
            };
        }
    }

