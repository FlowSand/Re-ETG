// Decompiled with JetBrains decompiler
// Type: GlobalSparksDoer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public static class GlobalSparksDoer
    {
      private static ParticleSystem m_particles;
      private static ParticleSystem m_phantomParticles;
      private static ParticleSystem m_chaffParticles;
      private static ParticleSystem m_sparkleParticles;
      private static ParticleSystem m_fireParticles;
      private static ParticleSystem m_darkMagicParticles;
      public static ParticleSystem EmberParticles;
      private static ParticleSystem m_bloodParticles;
      private static ParticleSystem m_greenFireParticles;
      private static ParticleSystem m_redMatterParticles;

      public static RedMatterParticleController GetRedMatterController()
      {
        return (bool) (Object) GlobalSparksDoer.m_redMatterParticles ? GlobalSparksDoer.m_redMatterParticles.GetComponent<RedMatterParticleController>() : (RedMatterParticleController) null;
      }

      public static EmbersController GetEmbersController()
      {
        if ((Object) GlobalSparksDoer.EmberParticles == (Object) null)
          GlobalSparksDoer.InitializeParticles(GlobalSparksDoer.SparksType.EMBERS_SWIRLING);
        return GlobalSparksDoer.EmberParticles.GetComponent<EmbersController>();
      }

      public static void DoSingleParticle(
        Vector3 position,
        Vector3 direction,
        float? startSize = null,
        float? startLifetime = null,
        Color? startColor = null,
        GlobalSparksDoer.SparksType systemType = GlobalSparksDoer.SparksType.SPARKS_ADDITIVE_DEFAULT)
      {
        ParticleSystem particleSystem = GlobalSparksDoer.m_particles;
        switch (systemType)
        {
          case GlobalSparksDoer.SparksType.SPARKS_ADDITIVE_DEFAULT:
            particleSystem = GlobalSparksDoer.m_particles;
            break;
          case GlobalSparksDoer.SparksType.BLACK_PHANTOM_SMOKE:
            particleSystem = GlobalSparksDoer.m_phantomParticles;
            break;
          case GlobalSparksDoer.SparksType.FLOATY_CHAFF:
            particleSystem = GlobalSparksDoer.m_chaffParticles;
            break;
          case GlobalSparksDoer.SparksType.SOLID_SPARKLES:
            particleSystem = GlobalSparksDoer.m_sparkleParticles;
            break;
          case GlobalSparksDoer.SparksType.EMBERS_SWIRLING:
            particleSystem = GlobalSparksDoer.EmberParticles;
            break;
          case GlobalSparksDoer.SparksType.STRAIGHT_UP_FIRE:
            particleSystem = GlobalSparksDoer.m_fireParticles;
            break;
          case GlobalSparksDoer.SparksType.DARK_MAGICKS:
            particleSystem = GlobalSparksDoer.m_darkMagicParticles;
            break;
          case GlobalSparksDoer.SparksType.BLOODY_BLOOD:
            particleSystem = GlobalSparksDoer.m_bloodParticles;
            break;
          case GlobalSparksDoer.SparksType.STRAIGHT_UP_GREEN_FIRE:
            particleSystem = GlobalSparksDoer.m_greenFireParticles;
            break;
          case GlobalSparksDoer.SparksType.RED_MATTER:
            particleSystem = GlobalSparksDoer.m_redMatterParticles;
            break;
        }
        if ((Object) particleSystem == (Object) null)
          particleSystem = GlobalSparksDoer.InitializeParticles(systemType);
        ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams()
        {
          position = position,
          velocity = direction,
          startSize = !startSize.HasValue ? particleSystem.startSize : startSize.Value,
          startLifetime = !startLifetime.HasValue ? particleSystem.startLifetime : startLifetime.Value,
          startColor = (Color32) (!startColor.HasValue ? particleSystem.startColor : startColor.Value),
          randomSeed = (uint) Random.Range(1, 1000)
        };
        particleSystem.Emit(emitParams, 1);
      }

      public static void DoRandomParticleBurst(
        int num,
        Vector3 minPosition,
        Vector3 maxPosition,
        Vector3 direction,
        float angleVariance,
        float magnitudeVariance,
        float? startSize = null,
        float? startLifetime = null,
        Color? startColor = null,
        GlobalSparksDoer.SparksType systemType = GlobalSparksDoer.SparksType.SPARKS_ADDITIVE_DEFAULT)
      {
        for (int index = 0; index < num; ++index)
          GlobalSparksDoer.DoSingleParticle(new Vector3(Random.Range(minPosition.x, maxPosition.x), Random.Range(minPosition.y, maxPosition.y), Random.Range(minPosition.z, maxPosition.z)), Quaternion.Euler(0.0f, 0.0f, Random.Range(-angleVariance, angleVariance)) * (direction.normalized * Random.Range(direction.magnitude - magnitudeVariance, direction.magnitude + magnitudeVariance)), startSize, startLifetime, startColor, systemType);
      }

      public static void DoLinearParticleBurst(
        int num,
        Vector3 minPosition,
        Vector3 maxPosition,
        float angleVariance,
        float baseMagnitude,
        float magnitudeVariance,
        float? startSize = null,
        float? startLifetime = null,
        Color? startColor = null,
        GlobalSparksDoer.SparksType systemType = GlobalSparksDoer.SparksType.SPARKS_ADDITIVE_DEFAULT)
      {
        for (int index = 0; index < num; ++index)
        {
          Vector3 position = Vector3.Lerp(minPosition, maxPosition, ((float) index + 1f) / (float) num);
          Vector3 vector3Zup = Random.insideUnitCircle.normalized.ToVector3ZUp();
          Vector3 direction = Quaternion.Euler(0.0f, 0.0f, Random.Range(-angleVariance, angleVariance)) * (vector3Zup.normalized * Random.Range(baseMagnitude - magnitudeVariance, vector3Zup.magnitude + magnitudeVariance));
          GlobalSparksDoer.DoSingleParticle(position, direction, startSize, startLifetime, startColor, systemType);
        }
      }

      public static void DoRadialParticleBurst(
        int num,
        Vector3 minPosition,
        Vector3 maxPosition,
        float angleVariance,
        float baseMagnitude,
        float magnitudeVariance,
        float? startSize = null,
        float? startLifetime = null,
        Color? startColor = null,
        GlobalSparksDoer.SparksType systemType = GlobalSparksDoer.SparksType.SPARKS_ADDITIVE_DEFAULT)
      {
        for (int index = 0; index < num; ++index)
        {
          Vector3 position = new Vector3(Random.Range(minPosition.x, maxPosition.x), Random.Range(minPosition.y, maxPosition.y), Random.Range(minPosition.z, maxPosition.z));
          Vector3 vector3 = position - (maxPosition + minPosition) / 2f;
          Vector3 direction = Quaternion.Euler(0.0f, 0.0f, Random.Range(-angleVariance, angleVariance)) * (vector3.normalized * Random.Range(baseMagnitude - magnitudeVariance, vector3.magnitude + magnitudeVariance));
          GlobalSparksDoer.DoSingleParticle(position, direction, startSize, startLifetime, startColor, systemType);
        }
      }

      public static void EmitFromRegion(
        GlobalSparksDoer.EmitRegionStyle emitStyle,
        float numPerSecond,
        float duration,
        Vector3 minPosition,
        Vector3 maxPosition,
        Vector3 direction,
        float angleVariance,
        float magnitudeVariance,
        float? startSize = null,
        float? startLifetime = null,
        Color? startColor = null,
        GlobalSparksDoer.SparksType systemType = GlobalSparksDoer.SparksType.SPARKS_ADDITIVE_DEFAULT)
      {
        GameUIRoot.Instance.StartCoroutine(GlobalSparksDoer.HandleEmitFromRegion(emitStyle, numPerSecond, duration, minPosition, maxPosition, direction, angleVariance, magnitudeVariance, startSize, startLifetime, startColor, systemType));
      }

      [DebuggerHidden]
      private static IEnumerator HandleEmitFromRegion(
        GlobalSparksDoer.EmitRegionStyle emitStyle,
        float numPerSecond,
        float duration,
        Vector3 minPosition,
        Vector3 maxPosition,
        Vector3 direction,
        float angleVariance,
        float magnitudeVariance,
        float? startSize = null,
        float? startLifetime = null,
        Color? startColor = null,
        GlobalSparksDoer.SparksType systemType = GlobalSparksDoer.SparksType.SPARKS_ADDITIVE_DEFAULT)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new GlobalSparksDoer.<HandleEmitFromRegion>c__Iterator0()
        {
          duration = duration,
          numPerSecond = numPerSecond,
          emitStyle = emitStyle,
          minPosition = minPosition,
          maxPosition = maxPosition,
          direction = direction,
          angleVariance = angleVariance,
          magnitudeVariance = magnitudeVariance,
          startSize = startSize,
          startLifetime = startLifetime,
          startColor = startColor,
          systemType = systemType
        };
      }

      private static ParticleSystem InitializeParticles(GlobalSparksDoer.SparksType targetType)
      {
        switch (targetType)
        {
          case GlobalSparksDoer.SparksType.SPARKS_ADDITIVE_DEFAULT:
            GlobalSparksDoer.m_particles = ((GameObject) Object.Instantiate(ResourceCache.Acquire("Global VFX/SparkSystem"), Vector3.zero, Quaternion.identity)).GetComponent<ParticleSystem>();
            return GlobalSparksDoer.m_particles;
          case GlobalSparksDoer.SparksType.BLACK_PHANTOM_SMOKE:
            GlobalSparksDoer.m_phantomParticles = ((GameObject) Object.Instantiate(ResourceCache.Acquire("Global VFX/PhantomSystem"), Vector3.zero, Quaternion.identity)).GetComponent<ParticleSystem>();
            return GlobalSparksDoer.m_phantomParticles;
          case GlobalSparksDoer.SparksType.FLOATY_CHAFF:
            GlobalSparksDoer.m_chaffParticles = ((GameObject) Object.Instantiate(ResourceCache.Acquire("Global VFX/ChaffSystem"), Vector3.zero, Quaternion.identity)).GetComponent<ParticleSystem>();
            return GlobalSparksDoer.m_chaffParticles;
          case GlobalSparksDoer.SparksType.SOLID_SPARKLES:
            GlobalSparksDoer.m_sparkleParticles = ((GameObject) Object.Instantiate(ResourceCache.Acquire("Global VFX/SparklesSystem"), Vector3.zero, Quaternion.identity)).GetComponent<ParticleSystem>();
            return GlobalSparksDoer.m_sparkleParticles;
          case GlobalSparksDoer.SparksType.EMBERS_SWIRLING:
            GlobalSparksDoer.EmberParticles = ((GameObject) Object.Instantiate(ResourceCache.Acquire("Global VFX/EmberSystem"), Vector3.zero, Quaternion.identity)).GetComponent<ParticleSystem>();
            return GlobalSparksDoer.EmberParticles;
          case GlobalSparksDoer.SparksType.STRAIGHT_UP_FIRE:
            GlobalSparksDoer.m_fireParticles = ((GameObject) Object.Instantiate(ResourceCache.Acquire("Global VFX/GlobalFireSystem"), Vector3.zero, Quaternion.identity)).GetComponent<ParticleSystem>();
            return GlobalSparksDoer.m_fireParticles;
          case GlobalSparksDoer.SparksType.DARK_MAGICKS:
            GlobalSparksDoer.m_darkMagicParticles = ((GameObject) Object.Instantiate(ResourceCache.Acquire("Global VFX/DarkMagicSystem"), Vector3.zero, Quaternion.identity)).GetComponent<ParticleSystem>();
            return GlobalSparksDoer.m_darkMagicParticles;
          case GlobalSparksDoer.SparksType.BLOODY_BLOOD:
            GlobalSparksDoer.m_bloodParticles = ((GameObject) Object.Instantiate(ResourceCache.Acquire("Global VFX/BloodSystem"), Vector3.zero, Quaternion.identity)).GetComponent<ParticleSystem>();
            return GlobalSparksDoer.m_bloodParticles;
          case GlobalSparksDoer.SparksType.STRAIGHT_UP_GREEN_FIRE:
            GlobalSparksDoer.m_greenFireParticles = ((GameObject) Object.Instantiate(ResourceCache.Acquire("Global VFX/GlobalGreenFireSystem"), (Vector3) Vector2.zero, Quaternion.identity)).GetComponent<ParticleSystem>();
            return GlobalSparksDoer.m_greenFireParticles;
          case GlobalSparksDoer.SparksType.RED_MATTER:
            GlobalSparksDoer.m_redMatterParticles = ((GameObject) Object.Instantiate(ResourceCache.Acquire("Global VFX/GlobalRedMatterSystem"), (Vector3) Vector2.zero, Quaternion.identity)).GetComponent<ParticleSystem>();
            return GlobalSparksDoer.m_redMatterParticles;
          default:
            return GlobalSparksDoer.m_particles;
        }
      }

      public static void CleanupOnSceneTransition()
      {
        GlobalSparksDoer.m_particles = (ParticleSystem) null;
        GlobalSparksDoer.m_phantomParticles = (ParticleSystem) null;
        GlobalSparksDoer.m_chaffParticles = (ParticleSystem) null;
        GlobalSparksDoer.m_sparkleParticles = (ParticleSystem) null;
        GlobalSparksDoer.m_fireParticles = (ParticleSystem) null;
        GlobalSparksDoer.m_darkMagicParticles = (ParticleSystem) null;
        GlobalSparksDoer.m_bloodParticles = (ParticleSystem) null;
        GlobalSparksDoer.EmberParticles = (ParticleSystem) null;
        GlobalSparksDoer.m_greenFireParticles = (ParticleSystem) null;
        GlobalSparksDoer.m_redMatterParticles = (ParticleSystem) null;
      }

      public enum EmitRegionStyle
      {
        RANDOM,
        RADIAL,
      }

      public enum SparksType
      {
        SPARKS_ADDITIVE_DEFAULT,
        BLACK_PHANTOM_SMOKE,
        FLOATY_CHAFF,
        SOLID_SPARKLES,
        EMBERS_SWIRLING,
        STRAIGHT_UP_FIRE,
        DARK_MAGICKS,
        BLOODY_BLOOD,
        STRAIGHT_UP_GREEN_FIRE,
        RED_MATTER,
      }
    }

}
