// Decompiled with JetBrains decompiler
// Type: BlackHoleDoer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class BlackHoleDoer : SpawnObjectItem
    {
      [Header("Intro Settings")]
      public BlackHoleDoer.BlackHoleIntroStyle introStyle;
      [ShowInInspectorIf("introStyle", 0, false)]
      public float introDuration = 0.5f;
      [Header("Core Settings")]
      public float coreDuration = 5f;
      public float damageRadius = 0.5f;
      public float radius = 15f;
      public float gravitationalForce = 10f;
      public float gravitationalForceActors = 50f;
      public bool affectsBullets = true;
      public bool destroysBullets = true;
      public bool affectsDebris = true;
      public bool destroysDebris = true;
      public bool affectsEnemies = true;
      public float damageToEnemiesPerSecond = 30f;
      public bool affectsPlayer;
      public float damageToPlayerPerSecond;
      [Header("Outro Settings")]
      public BlackHoleDoer.BlackHoleOutroStyle outroStyle;
      [ShowInInspectorIf("outroStyle", 0, false)]
      public float outroDuration = 0.5f;
      [ShowInInspectorIf("outroStyle", 1, false)]
      public float novaForce = 50f;
      public float distortStrength = 0.01f;
      public float distortTimeScale = 0.5f;
      public float distortRadiusFactor = 1f;
      private int m_currentPhase;
      private bool m_currentPhaseInitiated;
      private float m_currentPhaseTimer = -1000f;
      private float m_radiusSquared;
      [Header("Synergy Settings")]
      public bool HasHellSynergy;
      [LongNumericEnum]
      public CustomSynergyType HellSynergy;
      public GameObject HellSynergyVFX;
      public GameObject OuterLimitsVFX;
      public GameObject OuterLimitsDamageVFX;
      private bool m_cachedOuterLimitsSynergy;
      private int m_planetsEaten;
      private float m_elapsed;
      private float m_fadeStartDistortStrength;
      private Material m_distortMaterial;

      private void Start()
      {
        this.m_distortMaterial = new Material(ShaderCache.Acquire("Brave/Internal/DistortionRadius"));
        this.m_distortMaterial.SetFloat("_Strength", this.distortStrength);
        this.m_distortMaterial.SetFloat("_TimePulse", this.distortTimeScale);
        this.m_distortMaterial.SetFloat("_RadiusFactor", this.distortRadiusFactor);
        this.m_distortMaterial.SetVector("_WaveCenter", this.GetCenterPointInScreenUV(this.sprite.WorldCenter));
        Pixelator.Instance.RegisterAdditionalRenderPass(this.m_distortMaterial);
        this.m_radiusSquared = this.radius * this.radius;
        this.m_currentPhase = 0;
        this.m_currentPhaseInitiated = false;
        this.m_currentPhaseTimer = -1000f;
        if (!this.HasHellSynergy || !this.SpawningPlayer.HasActiveBonusSynergy(this.HellSynergy))
          return;
        MeshRenderer component = UnityEngine.Object.Instantiate<GameObject>(this.HellSynergyVFX, this.transform.position + new Vector3(0.0f, -0.5f, 0.5f), Quaternion.Euler(45f, 0.0f, 0.0f), this.transform).GetComponent<MeshRenderer>();
        this.radius *= 2f;
        this.damageRadius *= 4f;
        this.gravitationalForceActors *= 4f;
        this.damageToEnemiesPerSecond *= 3f;
        this.StartCoroutine(this.HoldPortalOpen(component));
      }

      [DebuggerHidden]
      private IEnumerator HoldPortalOpen(MeshRenderer portal)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BlackHoleDoer__HoldPortalOpenc__Iterator0()
        {
          portal = portal,
          _this = this
        };
      }

      private Vector4 GetCenterPointInScreenUV(Vector2 centerPoint)
      {
        Vector3 viewportPoint = GameManager.Instance.MainCameraController.Camera.WorldToViewportPoint(centerPoint.ToVector3ZUp());
        return new Vector4(viewportPoint.x, viewportPoint.y, 0.0f, 0.0f);
      }

      private float GetDistanceToRigidbody(SpeculativeRigidbody other)
      {
        return Vector2.Distance(other.UnitCenter, this.specRigidbody.UnitCenter);
      }

      private Vector2 GetFrameAccelerationForRigidbody(
        Vector2 unitCenter,
        float currentDistance,
        float g)
      {
        Vector2 zero = Vector2.zero;
        float num1 = Mathf.Clamp01((float) (1.0 - (double) currentDistance / (double) this.radius));
        float num2 = g * num1 * num1;
        return (this.specRigidbody.UnitCenter - unitCenter).normalized * num2;
      }

      private bool AdjustDebrisVelocity(DebrisObject debris)
      {
        if (debris.IsPickupObject || (UnityEngine.Object) debris.GetComponent<BlackHoleDoer>() != (UnityEngine.Object) null)
          return false;
        float f = Vector2.SqrMagnitude(debris.sprite.WorldCenter - this.specRigidbody.UnitCenter);
        if ((double) f >= (double) this.m_radiusSquared)
          return false;
        float gravitationalForceActors = this.gravitationalForceActors;
        float currentDistance = Mathf.Sqrt(f);
        if ((double) currentDistance < (double) this.damageRadius)
        {
          UnityEngine.Object.Destroy((UnityEngine.Object) debris.gameObject);
          return true;
        }
        Vector2 accelerationForRigidbody = this.GetFrameAccelerationForRigidbody(debris.sprite.WorldCenter, currentDistance, gravitationalForceActors);
        float num = Mathf.Clamp(BraveTime.DeltaTime, 0.0f, 0.02f);
        if (debris.HasBeenTriggered)
          debris.ApplyVelocity(accelerationForRigidbody * num);
        else if ((double) currentDistance < (double) this.radius / 2.0)
          debris.Trigger((Vector3) (accelerationForRigidbody * num), 0.5f);
        return true;
      }

      private bool AdjustRigidbodyVelocity(SpeculativeRigidbody other)
      {
        Vector2 a = other.UnitCenter - this.specRigidbody.UnitCenter;
        float f = Vector2.SqrMagnitude(a);
        if ((double) f >= (double) this.m_radiusSquared)
          return false;
        float gravitationalForce = this.gravitationalForce;
        Vector2 velocity = other.Velocity;
        Projectile projectile = other.projectile;
        float g;
        if ((bool) (UnityEngine.Object) projectile)
        {
          projectile.collidesWithPlayer = false;
          if ((UnityEngine.Object) other.GetComponent<BlackHoleDoer>() != (UnityEngine.Object) null || velocity == Vector2.zero)
            return false;
          if ((double) f < 4.0 && (this.destroysBullets || this.m_cachedOuterLimitsSynergy))
          {
            if ((bool) (UnityEngine.Object) projectile.GetComponent<BecomeOrbitProjectileModifier>())
              ++this.m_planetsEaten;
            projectile.DieInAir();
            if (this.m_planetsEaten > 2)
            {
              if ((bool) (UnityEngine.Object) this.OuterLimitsVFX)
                SpawnManager.SpawnVFX(this.OuterLimitsVFX).GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor((Vector3) this.transform.position.XY(), tk2dBaseSprite.Anchor.MiddleCenter);
              this.transform.position.GetAbsoluteRoom().ApplyActionToNearbyEnemies(this.transform.position.XY(), 100f, new Action<AIActor, float>(this.OuterLimitsProcessEnemy));
              int num1 = (int) AkSoundEngine.PostEvent("Stop_SND_OBJ", this.gameObject);
              int num2 = (int) AkSoundEngine.PostEvent("Play_WPN_blackhole_impact_01", this.gameObject);
              int num3 = (int) AkSoundEngine.PostEvent("Play_OBJ_lightning_flash_01", this.gameObject);
              UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
            }
          }
          g = this.gravitationalForce;
        }
        else
        {
          if (!(bool) (UnityEngine.Object) other.aiActor)
            return false;
          g = this.gravitationalForceActors;
          if (!other.aiActor.enabled || !other.aiActor.HasBeenEngaged)
            return false;
          if ((double) BraveMathCollege.DistToRectangle(this.specRigidbody.UnitCenter, other.UnitBottomLeft, other.UnitDimensions) < (double) this.damageRadius)
            other.healthHaver.ApplyDamage(this.damageToEnemiesPerSecond * BraveTime.DeltaTime, a.normalized, string.Empty, damageCategory: DamageCategory.DamageOverTime);
          if (other.healthHaver.IsBoss)
            return false;
        }
        Vector2 vector2_1 = this.GetFrameAccelerationForRigidbody(other.UnitCenter, Mathf.Sqrt(f), g) * Mathf.Clamp(BraveTime.DeltaTime, 0.0f, 0.02f);
        Vector2 vector2_2 = velocity + vector2_1;
        if ((double) BraveTime.DeltaTime > 0.019999999552965164)
          vector2_2 *= 0.02f / BraveTime.DeltaTime;
        other.Velocity = vector2_2;
        if ((UnityEngine.Object) projectile != (UnityEngine.Object) null)
        {
          projectile.collidesWithPlayer = false;
          if (projectile.IsBulletScript)
            projectile.RemoveBulletScriptControl();
          if (vector2_2 != Vector2.zero)
          {
            projectile.Direction = vector2_2.normalized;
            projectile.Speed = Mathf.Max(3f, vector2_2.magnitude);
            other.Velocity = projectile.Direction * projectile.Speed;
            if (projectile.shouldRotate && ((double) vector2_2.x != 0.0 || (double) vector2_2.y != 0.0))
            {
              float num = BraveMathCollege.Atan2Degrees(projectile.Direction);
              if (!float.IsNaN(num) && !float.IsInfinity(num))
              {
                Quaternion quaternion = Quaternion.Euler(0.0f, 0.0f, num);
                if (!float.IsNaN(quaternion.x) && !float.IsNaN(quaternion.y))
                  projectile.transform.rotation = quaternion;
              }
            }
          }
        }
        return true;
      }

      public void OuterLimitsProcessEnemy(AIActor a, float b)
      {
        if (!(bool) (UnityEngine.Object) a || !a.IsNormalEnemy || !(bool) (UnityEngine.Object) a.healthHaver || a.IsGone)
          return;
        a.healthHaver.ApplyDamage(100f, Vector2.zero, "projectile");
        if (!((UnityEngine.Object) this.OuterLimitsDamageVFX != (UnityEngine.Object) null))
          return;
        a.PlayEffectOnActor(this.OuterLimitsDamageVFX, Vector3.zero, false, true);
      }

      private void LateUpdate()
      {
        this.m_elapsed += BraveTime.DeltaTime;
        this.m_cachedOuterLimitsSynergy = (bool) (UnityEngine.Object) GameManager.Instance.BestActivePlayer && GameManager.Instance.BestActivePlayer.HasActiveBonusSynergy(CustomSynergyType.OUTER_LIMITS);
        if ((bool) (UnityEngine.Object) this && (bool) (UnityEngine.Object) this.projectile && (double) this.m_elapsed > 9.0)
        {
          this.projectile.DieInAir();
        }
        else
        {
          if ((UnityEngine.Object) this.m_distortMaterial != (UnityEngine.Object) null)
            this.m_distortMaterial.SetVector("_WaveCenter", this.GetCenterPointInScreenUV(this.sprite.WorldCenter));
          switch (this.m_currentPhase)
          {
            case 0:
              this.LateUpdateIntro();
              break;
            case 1:
              this.LateUpdateCore();
              break;
            case 2:
              this.LateUpdateOutro();
              break;
            default:
              UnityEngine.Debug.LogError((object) ("Invalid State in BlackHoleDoer: " + this.m_currentPhase.ToString()));
              break;
          }
        }
      }

      private void LateUpdateIntro()
      {
        if (this.introStyle == BlackHoleDoer.BlackHoleIntroStyle.Instant)
        {
          this.m_currentPhase = 1;
        }
        else
        {
          if (this.introStyle != BlackHoleDoer.BlackHoleIntroStyle.Gradual)
            return;
          if (!this.m_currentPhaseInitiated)
          {
            this.m_currentPhaseInitiated = true;
            this.m_currentPhaseTimer = this.introDuration;
          }
          else if ((double) this.m_currentPhaseTimer > 0.0)
          {
            this.m_currentPhaseTimer -= BraveTime.DeltaTime;
          }
          else
          {
            this.m_currentPhase = 1;
            this.m_currentPhaseInitiated = false;
            this.m_currentPhaseTimer = -1000f;
          }
        }
      }

      private void LateUpdateCore()
      {
        if (!this.m_currentPhaseInitiated)
        {
          this.m_currentPhaseInitiated = true;
          this.m_currentPhaseTimer = this.coreDuration;
        }
        else if ((double) this.m_currentPhaseTimer > 0.0)
        {
          this.m_currentPhaseTimer -= BraveTime.DeltaTime;
          for (int index = 0; index < PhysicsEngine.Instance.AllRigidbodies.Count; ++index)
          {
            if (PhysicsEngine.Instance.AllRigidbodies[index].gameObject.activeSelf && PhysicsEngine.Instance.AllRigidbodies[index].enabled)
              this.AdjustRigidbodyVelocity(PhysicsEngine.Instance.AllRigidbodies[index]);
          }
          for (int index = 0; index < StaticReferenceManager.AllDebris.Count; ++index)
            this.AdjustDebrisVelocity(StaticReferenceManager.AllDebris[index]);
        }
        else
        {
          this.m_currentPhase = 2;
          this.m_currentPhaseInitiated = false;
          this.m_currentPhaseTimer = -1000f;
        }
      }

      private void LateUpdateOutro()
      {
        switch (this.outroStyle)
        {
          case BlackHoleDoer.BlackHoleOutroStyle.FadeAway:
            this.LateUpdateOutro_Fade();
            break;
          case BlackHoleDoer.BlackHoleOutroStyle.Nova:
            this.LateUpdateOutro_Nova();
            break;
        }
      }

      private void LateUpdateOutro_Fade()
      {
        if (!this.m_currentPhaseInitiated)
        {
          this.m_currentPhaseInitiated = true;
          this.m_currentPhaseTimer = this.outroDuration;
          this.m_fadeStartDistortStrength = this.m_distortMaterial.GetFloat("_Strength");
          this.outroDuration = this.spriteAnimator.GetClipByName("black_hole_out_item_vfx").BaseClipLength;
          this.spriteAnimator.PlayAndDestroyObject("black_hole_out_item_vfx");
        }
        else
        {
          if ((double) this.m_currentPhaseTimer <= 0.0)
            return;
          this.m_currentPhaseTimer -= BraveTime.DeltaTime;
          float t = (float) (1.0 - (double) this.m_currentPhaseTimer / (double) this.outroDuration);
          if (!((UnityEngine.Object) this.m_distortMaterial != (UnityEngine.Object) null))
            return;
          this.m_distortMaterial.SetFloat("_Strength", Mathf.Lerp(this.m_fadeStartDistortStrength, 0.0f, t));
        }
      }

      private void LateUpdateOutro_Nova()
      {
        if (!this.m_currentPhaseInitiated)
          this.m_currentPhaseInitiated = true;
        else
          UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
      }

      protected override void OnDestroy()
      {
        if ((UnityEngine.Object) Pixelator.Instance != (UnityEngine.Object) null)
          Pixelator.Instance.DeregisterAdditionalRenderPass(this.m_distortMaterial);
        base.OnDestroy();
      }

      public enum BlackHoleIntroStyle
      {
        Gradual,
        Instant,
      }

      public enum BlackHoleOutroStyle
      {
        FadeAway,
        Nova,
      }
    }

}
