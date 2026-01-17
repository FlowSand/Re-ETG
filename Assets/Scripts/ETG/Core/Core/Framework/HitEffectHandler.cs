// Decompiled with JetBrains decompiler
// Type: HitEffectHandler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class HitEffectHandler : BraveBehaviour
    {
      public DungeonMaterial overrideMaterialDefinition;
      public VFXComplex overrideHitEffect;
      public VFXPool overrideHitEffectPool;
      public HitEffectHandler.AdditionalHitEffect[] additionalHitEffects;
      public bool SuppressAllHitEffects;
      private bool m_isTrackingDelays;

      public bool SuppressAdditionalHitEffects { get; set; }

      protected override void OnDestroy() => base.OnDestroy();

      public void HandleAdditionalHitEffects(Vector2 projVelocity, PixelCollider hitPixelCollider)
      {
        if (this.SuppressAdditionalHitEffects)
          return;
        for (int index = 0; index < this.additionalHitEffects.Length; ++index)
        {
          HitEffectHandler.AdditionalHitEffect additionalHitEffect = this.additionalHitEffects[index];
          if ((double) additionalHitEffect.delayTimer <= 0.0 && ((double) additionalHitEffect.chance >= 1.0 || (double) UnityEngine.Random.value <= (double) additionalHitEffect.chance))
          {
            if (additionalHitEffect.specificPixelCollider)
            {
              int num = this.specRigidbody.PixelColliders.IndexOf(hitPixelCollider);
              if (additionalHitEffect.pixelColliderIndex != num)
                continue;
            }
            float angle = (!additionalHitEffect.flipNormals ? projVelocity : -projVelocity).ToAngle();
            if (additionalHitEffect.spawnOnGround)
            {
              Vector2 position = this.specRigidbody.UnitCenter + BraveMathCollege.DegreesToVector(angle + UnityEngine.Random.Range(-additionalHitEffect.angleVariance, additionalHitEffect.angleVariance), UnityEngine.Random.Range(additionalHitEffect.minDistance, additionalHitEffect.maxDistance));
              additionalHitEffect.hitEffect.SpawnAtPosition((Vector3) position, angle);
            }
            else
            {
              Vector2 localPosition = (!(bool) (UnityEngine.Object) additionalHitEffect.transform ? this.specRigidbody.GetUnitCenter(ColliderType.HitBox) : additionalHitEffect.transform.position.XY()) - this.transform.position.XY() + BraveMathCollege.DegreesToVector(angle, additionalHitEffect.radius);
              if (additionalHitEffect.doForce)
              {
                Vector2 vector2 = BraveMathCollege.DegreesToVector(angle + UnityEngine.Random.Range(-additionalHitEffect.angleVariance, additionalHitEffect.angleVariance), UnityEngine.Random.Range(additionalHitEffect.minForce, additionalHitEffect.maxForce)) + new Vector2(0.0f, additionalHitEffect.additionalVerticalForce);
                Vector2 normalized = (Vector2) (Quaternion.Euler(0.0f, 0.0f, 90f) * (Vector3) vector2).normalized;
                additionalHitEffect.hitEffect.SpawnAtLocalPosition((Vector3) localPosition, angle, this.transform, new Vector2?(normalized), new Vector2?(vector2));
              }
              else
                additionalHitEffect.hitEffect.SpawnAtLocalPosition((Vector3) localPosition, angle, this.transform);
            }
            if ((double) additionalHitEffect.delay > 0.0)
            {
              additionalHitEffect.delayTimer = additionalHitEffect.delay;
              if (!this.m_isTrackingDelays)
                this.StartCoroutine(this.TrackDelaysCR());
            }
          }
        }
      }

      [DebuggerHidden]
      private IEnumerator TrackDelaysCR()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new HitEffectHandler.\u003CTrackDelaysCR\u003Ec__Iterator0()
        {
          \u0024this = this
        };
      }

      [Serializable]
      public class AdditionalHitEffect
      {
        public VFXPool hitEffect;
        public float chance = 1f;
        public Transform transform;
        public bool flipNormals;
        public float radius;
        public float delay;
        public float angleVariance;
        public bool doForce;
        [ShowInInspectorIf("doForce", true)]
        public float minForce;
        [ShowInInspectorIf("doForce", true)]
        public float maxForce;
        [ShowInInspectorIf("doForce", true)]
        public float additionalVerticalForce;
        public bool spawnOnGround;
        [ShowInInspectorIf("spawnOnGround", true)]
        public float minDistance;
        [ShowInInspectorIf("spawnOnGround", true)]
        public float maxDistance;
        public bool specificPixelCollider;
        [ShowInInspectorIf("specificPixelCollider", false)]
        public int pixelColliderIndex;
        [NonSerialized]
        public float delayTimer;
      }
    }

}
