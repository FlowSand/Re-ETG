using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

[Serializable]
public class GameActorFireEffect : GameActorHealthEffect
  {
    public const float BossMinResistance = 0.25f;
    public const float BossMaxResistance = 0.75f;
    public const float BossResistanceDelta = 0.025f;
    public List<GameObject> FlameVfx;
    public int flameNumPerSquareUnit = 10;
    public Vector2 flameBuffer = new Vector2(1f / 16f, 1f / 16f);
    public float flameFpsVariation = 0.5f;
    public float flameMoveChance = 0.2f;
    public bool IsGreenFire;
    private float m_particleTimer;
    private float m_emberCounter;

    public override bool ResistanceAffectsDuration => true;

    public static RuntimeGameActorEffectData ApplyFlamesToTarget(
      GameActor actor,
      GameActorFireEffect sourceEffect)
    {
      return new RuntimeGameActorEffectData()
      {
        actor = actor
      };
    }

    public override void OnEffectApplied(
      GameActor actor,
      RuntimeGameActorEffectData effectData,
      float partialAmount = 1f)
    {
      base.OnEffectApplied(actor, effectData, partialAmount);
      effectData.OnActorPreDeath = (Action<Vector2>) (dir => GameActorFireEffect.DestroyFlames(effectData));
      actor.healthHaver.OnPreDeath += effectData.OnActorPreDeath;
      if (this.FlameVfx == null || this.FlameVfx.Count <= 0)
        return;
      if (effectData.vfxObjects == null)
        effectData.vfxObjects = new List<Tuple<GameObject, float>>();
      effectData.OnFlameAnimationCompleted = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>) ((spriteAnimator, clip) =>
      {
        if (effectData.destroyVfx || !(bool) (UnityEngine.Object) actor)
        {
          spriteAnimator.AnimationCompleted -= effectData.OnFlameAnimationCompleted;
          UnityEngine.Object.Destroy((UnityEngine.Object) spriteAnimator.gameObject);
        }
        else
        {
          if ((double) UnityEngine.Random.value < (double) this.flameMoveChance)
          {
            Vector2 vector2_1 = actor.specRigidbody.HitboxPixelCollider.UnitDimensions / 2f;
            Vector2 vector2_2 = actor.specRigidbody.HitboxPixelCollider.UnitCenter + BraveUtility.RandomVector2(-vector2_1 + this.flameBuffer, vector2_1 - this.flameBuffer);
            spriteAnimator.transform.position = (Vector3) vector2_2;
          }
          spriteAnimator.Play(clip, 0.0f, clip.fps * UnityEngine.Random.Range(1f - this.flameFpsVariation, 1f + this.flameFpsVariation));
        }
      });
    }

    public override void EffectTick(GameActor actor, RuntimeGameActorEffectData effectData)
    {
      base.EffectTick(actor, effectData);
      if (GameManager.Options.ShaderQuality == GameOptions.GenericHighMedLowOption.HIGH && (bool) (UnityEngine.Object) effectData.actor && effectData.actor.specRigidbody.HitboxPixelCollider != null)
      {
        Vector2 unitBottomLeft = effectData.actor.specRigidbody.HitboxPixelCollider.UnitBottomLeft;
        Vector2 unitTopRight = effectData.actor.specRigidbody.HitboxPixelCollider.UnitTopRight;
        this.m_emberCounter += 30f * BraveTime.DeltaTime;
        if ((double) this.m_emberCounter > 1.0)
        {
          int num = Mathf.FloorToInt(this.m_emberCounter);
          this.m_emberCounter -= (float) num;
          GlobalSparksDoer.DoRandomParticleBurst(num, (Vector3) unitBottomLeft, (Vector3) unitTopRight, new Vector3(1f, 1f, 0.0f), 120f, 0.75f, systemType: GlobalSparksDoer.SparksType.EMBERS_SWIRLING);
        }
      }
      if ((bool) (UnityEngine.Object) actor && (bool) (UnityEngine.Object) actor.specRigidbody)
      {
        Vector2 unitDimensions = actor.specRigidbody.HitboxPixelCollider.UnitDimensions;
        Vector2 vector2 = unitDimensions / 2f;
        this.m_particleTimer += BraveTime.DeltaTime * (float) Mathf.RoundToInt((float) ((double) this.flameNumPerSquareUnit * 0.5 * (double) Mathf.Min(30f, Mathf.Min(unitDimensions.x * unitDimensions.y))));
        if ((double) this.m_particleTimer > 1.0)
        {
          int num = Mathf.FloorToInt(this.m_particleTimer);
          Vector2 lhs1 = actor.specRigidbody.HitboxPixelCollider.UnitBottomLeft;
          Vector2 lhs2 = actor.specRigidbody.HitboxPixelCollider.UnitTopRight;
          PixelCollider pixelCollider = actor.specRigidbody.GetPixelCollider(ColliderType.Ground);
          if (pixelCollider != null && pixelCollider.ColliderGenerationMode == PixelCollider.PixelColliderGeneration.Manual)
          {
            lhs1 = Vector2.Min(lhs1, pixelCollider.UnitBottomLeft);
            lhs2 = Vector2.Max(lhs2, pixelCollider.UnitTopRight);
          }
          Vector2 minPosition = lhs1 + Vector2.Min(vector2 * 0.15f, new Vector2(0.25f, 0.25f));
          Vector2 maxPosition = lhs2 - Vector2.Min(vector2 * 0.15f, new Vector2(0.25f, 0.25f));
          maxPosition.y -= Mathf.Min(vector2.y * 0.1f, 0.1f);
          GlobalSparksDoer.DoRandomParticleBurst(num, (Vector3) minPosition, (Vector3) maxPosition, Vector3.zero, 0.0f, 0.0f, systemType: !this.IsGreenFire ? GlobalSparksDoer.SparksType.STRAIGHT_UP_FIRE : GlobalSparksDoer.SparksType.STRAIGHT_UP_GREEN_FIRE);
          this.m_particleTimer -= Mathf.Floor(this.m_particleTimer);
        }
      }
      if (actor.IsGone)
        effectData.elapsed = 10000f;
      if (!actor.IsFalling && !actor.IsGone || effectData.vfxObjects == null || effectData.vfxObjects.Count <= 0)
        return;
      GameActorFireEffect.DestroyFlames(effectData);
    }

    public override void OnEffectRemoved(GameActor actor, RuntimeGameActorEffectData effectData)
    {
      base.OnEffectRemoved(actor, effectData);
      actor.healthHaver.OnPreDeath -= effectData.OnActorPreDeath;
      GameActorFireEffect.DestroyFlames(effectData);
    }

    public static void DestroyFlames(RuntimeGameActorEffectData effectData)
    {
      if (effectData.vfxObjects == null)
        return;
      if (!effectData.actor.IsFrozen)
      {
        for (int index = 0; index < effectData.vfxObjects.Count; ++index)
        {
          GameObject first = effectData.vfxObjects[index].First;
          if ((bool) (UnityEngine.Object) first)
            first.transform.parent = SpawnManager.Instance.VFX;
        }
      }
      effectData.vfxObjects.Clear();
      effectData.destroyVfx = true;
      if (GameManager.Options.ShaderQuality != GameOptions.GenericHighMedLowOption.HIGH || !(bool) (UnityEngine.Object) effectData.actor || !(bool) (UnityEngine.Object) effectData.actor.healthHaver || (double) effectData.actor.healthHaver.GetCurrentHealth() > 0.0 || effectData.actor.specRigidbody.HitboxPixelCollider == null)
        return;
      Vector2 unitBottomLeft = effectData.actor.specRigidbody.HitboxPixelCollider.UnitBottomLeft;
      Vector2 unitTopRight = effectData.actor.specRigidbody.HitboxPixelCollider.UnitTopRight;
      GlobalSparksDoer.DoRandomParticleBurst(Mathf.Max(1, (int) (75.0 * (((double) unitTopRight.x - (double) unitBottomLeft.x) * ((double) unitTopRight.y - (double) unitBottomLeft.y)))), (Vector3) unitBottomLeft, (Vector3) unitTopRight, new Vector3(1f, 1f, 0.0f), 120f, 0.75f, systemType: GlobalSparksDoer.SparksType.EMBERS_SWIRLING);
    }
  }

