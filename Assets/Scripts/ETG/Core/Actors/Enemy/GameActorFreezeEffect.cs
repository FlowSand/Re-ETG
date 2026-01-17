// Decompiled with JetBrains decompiler
// Type: GameActorFreezeEffect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Actors.Enemy
{
    [Serializable]
    public class GameActorFreezeEffect : GameActorEffect
    {
      public const float BossMinResistance = 0.6f;
      public const float BossMaxResistance = 1f;
      public const float BossResistanceDelta = 0.01f;
      public const float BossMaxFreezeAmount = 75f;
      public float FreezeAmount = 10f;
      public float UnfreezeDamagePercent = 0.333f;
      public List<GameObject> FreezeCrystals;
      [NonSerialized]
      public int crystalNum = 4;
      public int crystalRot = 8;
      public Vector2 crystalVariation = new Vector2(0.05f, 0.05f);
      public int debrisMinForce = 5;
      public int debrisMaxForce = 5;
      public float debrisAngleVariance = 15f;
      public GameObject vfxExplosion;

      public bool ShouldVanishOnDeath(GameActor actor)
      {
        return (!(bool) (UnityEngine.Object) actor.healthHaver || !actor.healthHaver.IsBoss) && (!(actor is AIActor) || !(actor as AIActor).IsSignatureEnemy);
      }

      public override void ApplyTint(GameActor actor)
      {
      }

      public override void OnEffectApplied(
        GameActor actor,
        RuntimeGameActorEffectData effectData,
        float partialAmount = 1f)
      {
        effectData.MovementModifier = (GameActor.MovementModifier) ((ref Vector2 volundaryVel, ref Vector2 involuntaryVel) =>
        {
          float num = Mathf.Clamp01((float) ((100.0 - (double) actor.FreezeAmount) / 100.0));
          volundaryVel *= num;
        });
        actor.MovementModifiers += effectData.MovementModifier;
        effectData.OnActorPreDeath = (Action<Vector2>) (dir =>
        {
          if (!actor.IsFrozen)
            return;
          this.DestroyCrystals(effectData, !actor.IsFalling);
          int num = (int) AkSoundEngine.PostEvent("Play_OBJ_crystal_shatter_01", GameManager.Instance.PrimaryPlayer.gameObject);
          actor.FreezeAmount = 0.0f;
          if (!this.ShouldVanishOnDeath(actor))
            return;
          if (actor is AIActor)
            (actor as AIActor).ForceDeath(dir, false);
          UnityEngine.Object.Destroy((UnityEngine.Object) actor.gameObject);
        });
        actor.healthHaver.OnPreDeath += effectData.OnActorPreDeath;
        actor.FreezeAmount += this.FreezeAmount * partialAmount;
      }

      public override void OnDarkSoulsAccumulate(
        GameActor actor,
        RuntimeGameActorEffectData effectData,
        float partialAmount = 1f,
        Projectile sourceProjectile = null)
      {
        if (effectData.actor.IsFrozen)
          return;
        actor.FreezeAmount += this.FreezeAmount * partialAmount;
        if (!actor.healthHaver.IsBoss)
          return;
        actor.FreezeAmount = Mathf.Min(actor.FreezeAmount, 75f);
      }

      public override void EffectTick(GameActor actor, RuntimeGameActorEffectData effectData)
      {
        if ((double) actor.FreezeAmount > 0.0)
        {
          actor.FreezeAmount = Mathf.Max(0.0f, actor.FreezeAmount - BraveTime.DeltaTime * actor.FreezeDispelFactor);
          if (!actor.IsFrozen)
          {
            if ((double) actor.FreezeAmount > 100.0 && actor.healthHaver.IsAlive)
            {
              actor.FreezeAmount = 100f;
              if (this.FreezeCrystals.Count > 0)
              {
                if (effectData.vfxObjects == null)
                  effectData.vfxObjects = new List<Tuple<GameObject, float>>();
                int num1 = this.crystalNum;
                if ((bool) (UnityEngine.Object) effectData.actor && (bool) (UnityEngine.Object) effectData.actor.specRigidbody && effectData.actor.specRigidbody.HitboxPixelCollider != null)
                  num1 = Mathf.Max(this.crystalNum, (int) ((double) this.crystalNum * (0.5 + (double) effectData.actor.specRigidbody.HitboxPixelCollider.UnitDimensions.x * (double) effectData.actor.specRigidbody.HitboxPixelCollider.UnitDimensions.y / 4.0)));
                for (int index = 0; index < num1; ++index)
                {
                  GameObject prefab = BraveUtility.RandomElement<GameObject>(this.FreezeCrystals);
                  Vector2 unitCenter1 = actor.specRigidbody.HitboxPixelCollider.UnitCenter;
                  Vector2 vector1 = BraveUtility.RandomVector2(-this.crystalVariation, this.crystalVariation);
                  Vector2 position = unitCenter1 + vector1;
                  float num2 = BraveMathCollege.QuantizeFloat(vector1.ToAngle(), 360f / (float) this.crystalRot);
                  Quaternion rotation = Quaternion.Euler(0.0f, 0.0f, num2);
                  GameObject gameObject = SpawnManager.SpawnVFX(prefab, (Vector3) position, rotation, true);
                  gameObject.transform.parent = actor.transform;
                  tk2dSprite component = gameObject.GetComponent<tk2dSprite>();
                  if ((bool) (UnityEngine.Object) component)
                  {
                    actor.sprite.AttachRenderer((tk2dBaseSprite) component);
                    component.HeightOffGround = 0.1f;
                  }
                  if ((bool) (UnityEngine.Object) effectData.actor && (bool) (UnityEngine.Object) effectData.actor.specRigidbody && effectData.actor.specRigidbody.HitboxPixelCollider != null)
                  {
                    Vector2 unitCenter2 = effectData.actor.specRigidbody.HitboxPixelCollider.UnitCenter;
                    float num3 = (float) index * (360f / (float) num1);
                    Vector2 normalized = BraveMathCollege.DegreesToVector(num3).normalized;
                    normalized.x *= effectData.actor.specRigidbody.HitboxPixelCollider.UnitDimensions.x / 2f;
                    normalized.y *= effectData.actor.specRigidbody.HitboxPixelCollider.UnitDimensions.y / 2f;
                    float magnitude = normalized.magnitude;
                    Vector2 vector2_1 = unitCenter2 + normalized;
                    Vector2 vector2_2 = unitCenter2 - vector2_1;
                    Vector2 vector2 = vector2_1 + vector2_2.normalized * (magnitude * UnityEngine.Random.Range(0.15f, 0.85f));
                    gameObject.transform.position = vector2.ToVector3ZUp();
                    gameObject.transform.rotation = Quaternion.Euler(0.0f, 0.0f, num3);
                  }
                  effectData.vfxObjects.Add(Tuple.Create<GameObject, float>(gameObject, num2));
                }
              }
              if (this.ShouldVanishOnDeath(actor))
                actor.StealthDeath = true;
              if ((bool) (UnityEngine.Object) actor.behaviorSpeculator)
              {
                if (actor.behaviorSpeculator.IsInterruptable)
                  actor.behaviorSpeculator.InterruptAndDisable();
                else
                  actor.behaviorSpeculator.enabled = false;
              }
              if (actor is AIActor)
              {
                AIActor aiActor = actor as AIActor;
                aiActor.ClearPath();
                aiActor.BehaviorOverridesVelocity = false;
              }
              actor.IsFrozen = true;
            }
          }
          else if (actor.IsFrozen)
          {
            if ((double) actor.FreezeAmount <= 0.0)
              return;
            if (actor.IsFalling)
            {
              if (effectData.vfxObjects != null && effectData.vfxObjects.Count > 0)
                this.DestroyCrystals(effectData, false);
              if ((bool) (UnityEngine.Object) actor.aiAnimator)
                actor.aiAnimator.FpsScale = 1f;
            }
          }
        }
        if (actor.healthHaver.IsDead)
          return;
        float num4 = !actor.healthHaver.IsBoss ? 100f : 75f;
        float num5 = !actor.IsFrozen ? Mathf.Clamp01((float) ((100.0 - (double) actor.FreezeAmount) / 100.0)) : 0.0f;
        float num6 = !actor.IsFrozen ? Mathf.Clamp01(actor.FreezeAmount / num4) : 1f;
        if ((bool) (UnityEngine.Object) actor.aiAnimator)
          actor.aiAnimator.FpsScale = !actor.IsFalling ? num5 : 1f;
        if ((bool) (UnityEngine.Object) actor.aiShooter)
          actor.aiShooter.AimTimeScale = num5;
        if ((bool) (UnityEngine.Object) actor.behaviorSpeculator)
          actor.behaviorSpeculator.CooldownScale = num5;
        if ((bool) (UnityEngine.Object) actor.bulletBank)
          actor.bulletBank.TimeScale = num5;
        if (!this.AppliesTint)
          return;
        float num7 = actor.FreezeAmount / actor.FreezeDispelFactor;
        Color overrideColor = this.TintColor;
        if ((double) num7 < 0.10000000149011612)
          overrideColor = Color.black;
        else if ((double) num7 < 0.20000000298023224)
          overrideColor = Color.white;
        overrideColor.a *= num6;
        actor.RegisterOverrideColor(overrideColor, this.effectIdentifier);
      }

      public override void OnEffectRemoved(GameActor actor, RuntimeGameActorEffectData effectData)
      {
        if (actor.IsFrozen)
        {
          actor.FreezeAmount = 0.0f;
          float resistanceForEffectType = actor.GetResistanceForEffectType(this.resistanceType);
          float damage = Mathf.Max(0.0f, (float) ((double) actor.healthHaver.GetMaxHealth() * (double) this.UnfreezeDamagePercent * (1.0 - (double) resistanceForEffectType)));
          actor.healthHaver.ApplyDamage(damage, Vector2.zero, "Freezer Burn", CoreDamageTypes.Ice, DamageCategory.DamageOverTime, true);
          this.DestroyCrystals(effectData, !actor.healthHaver.IsDead);
          if (this.AppliesTint)
            actor.DeregisterOverrideColor(this.effectIdentifier);
          if ((bool) (UnityEngine.Object) actor.behaviorSpeculator)
            actor.behaviorSpeculator.enabled = true;
          if (this.ShouldVanishOnDeath(actor))
            actor.StealthDeath = false;
          actor.IsFrozen = false;
        }
        actor.MovementModifiers -= effectData.MovementModifier;
        actor.healthHaver.OnPreDeath -= effectData.OnActorPreDeath;
        if ((bool) (UnityEngine.Object) actor.aiAnimator)
          actor.aiAnimator.FpsScale = 1f;
        if ((bool) (UnityEngine.Object) actor.aiShooter)
          actor.aiShooter.AimTimeScale = 1f;
        if ((bool) (UnityEngine.Object) actor.behaviorSpeculator)
          actor.behaviorSpeculator.CooldownScale = 1f;
        if ((bool) (UnityEngine.Object) actor.bulletBank)
          actor.bulletBank.TimeScale = 1f;
        tk2dSpriteAnimator spriteAnimator = actor.spriteAnimator;
        if (!(bool) (UnityEngine.Object) spriteAnimator || !(bool) (UnityEngine.Object) actor.aiAnimator || spriteAnimator.CurrentClip == null || spriteAnimator.IsPlaying(spriteAnimator.CurrentClip))
          return;
        actor.aiAnimator.PlayUntilFinished(actor.spriteAnimator.CurrentClip.name, skipChildAnimators: true);
      }

      public override bool IsFinished(
        GameActor actor,
        RuntimeGameActorEffectData effectData,
        float elapsedTime)
      {
        return (double) actor.FreezeAmount <= 0.0;
      }

      private void DestroyCrystals(RuntimeGameActorEffectData effectData, bool playVfxExplosion = true)
      {
        if (effectData.vfxObjects == null || effectData.vfxObjects.Count == 0)
          return;
        Vector2 zero = Vector2.zero;
        GameActor actor = effectData.actor;
        Vector2 position;
        if ((bool) (UnityEngine.Object) actor)
        {
          position = !(bool) (UnityEngine.Object) actor.specRigidbody ? actor.sprite.WorldCenter : actor.specRigidbody.HitboxPixelCollider.UnitCenter;
        }
        else
        {
          int num = 0;
          for (int index = 0; index < effectData.vfxObjects.Count; ++index)
          {
            if ((bool) (UnityEngine.Object) effectData.vfxObjects[index].First)
            {
              zero += effectData.vfxObjects[index].First.transform.position.XY();
              ++num;
            }
          }
          if (num == 0)
            return;
          position = zero / (float) num;
        }
        if (playVfxExplosion && (bool) (UnityEngine.Object) this.vfxExplosion)
        {
          tk2dSprite component = SpawnManager.SpawnVFX(this.vfxExplosion, (Vector3) position, Quaternion.identity).GetComponent<tk2dSprite>();
          if ((bool) (UnityEngine.Object) actor && (bool) (UnityEngine.Object) component)
          {
            actor.sprite.AttachRenderer((tk2dBaseSprite) component);
            component.HeightOffGround = 0.1f;
            component.UpdateZDepth();
          }
        }
        for (int index = 0; index < effectData.vfxObjects.Count; ++index)
        {
          GameObject first = effectData.vfxObjects[index].First;
          if ((bool) (UnityEngine.Object) first)
          {
            first.transform.parent = SpawnManager.Instance.VFX;
            DebrisObject orAddComponent = first.GetOrAddComponent<DebrisObject>();
            if ((bool) (UnityEngine.Object) actor)
              actor.sprite.AttachRenderer(orAddComponent.sprite);
            orAddComponent.sprite.IsPerpendicular = true;
            orAddComponent.DontSetLayer = true;
            orAddComponent.gameObject.SetLayerRecursively(LayerMask.NameToLayer("FG_Critical"));
            orAddComponent.angularVelocity = Mathf.Sign(UnityEngine.Random.value - 0.5f) * 125f;
            orAddComponent.angularVelocityVariance = 60f;
            orAddComponent.decayOnBounce = 0.5f;
            orAddComponent.bounceCount = 1;
            orAddComponent.canRotate = true;
            float angle = effectData.vfxObjects[index].Second + UnityEngine.Random.Range(-this.debrisAngleVariance, this.debrisAngleVariance);
            if (orAddComponent.name.Contains("tilt", true))
              angle += 45f;
            Vector2 vector2 = BraveMathCollege.DegreesToVector(angle) * (float) UnityEngine.Random.Range(this.debrisMinForce, this.debrisMaxForce);
            Vector3 startingForce = new Vector3(vector2.x, (double) vector2.y >= 0.0 ? 0.0f : vector2.y, (double) vector2.y <= 0.0 ? 0.0f : vector2.y);
            float startingHeight = !(bool) (UnityEngine.Object) actor ? 0.75f : first.transform.position.y - actor.specRigidbody.HitboxPixelCollider.UnitBottom;
            if ((bool) (UnityEngine.Object) orAddComponent.minorBreakable)
              orAddComponent.minorBreakable.enabled = true;
            orAddComponent.Trigger(startingForce, startingHeight);
          }
        }
        effectData.vfxObjects.Clear();
      }
    }

}
