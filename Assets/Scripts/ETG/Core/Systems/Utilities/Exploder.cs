using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class Exploder : MonoBehaviour
  {
    public static System.Action OnExplosionTriggered;
    private static bool ExplosionIsExtant;

    public static bool IsExplosionOccurring()
    {
      return Exploder.ExplosionIsExtant || ExplosionManager.Instance.QueueCount > 0;
    }

    public static void Explode(
      Vector3 position,
      ExplosionData data,
      Vector2 sourceNormal,
      System.Action onExplosionBegin = null,
      bool ignoreQueues = false,
      CoreDamageTypes damageTypes = CoreDamageTypes.None,
      bool ignoreDamageCaps = false)
    {
      if (data.useDefaultExplosion && data != GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultExplosionData)
        Exploder.DoDefaultExplosion(position, sourceNormal, onExplosionBegin, ignoreQueues, damageTypes, ignoreDamageCaps);
      else
        new GameObject("temp_explosion_processor", new System.Type[1]
        {
          typeof (Exploder)
        }).GetComponent<Exploder>().DoExplode(position, data, sourceNormal, onExplosionBegin, ignoreQueues, damageTypes, ignoreDamageCaps);
    }

    public static void DoDefaultExplosion(
      Vector3 position,
      Vector2 sourceNormal,
      System.Action onExplosionBegin = null,
      bool ignoreQueues = false,
      CoreDamageTypes damageTypes = CoreDamageTypes.None,
      bool ignoreDamageCaps = false)
    {
      Exploder.Explode(position, GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultExplosionData, sourceNormal, onExplosionBegin, ignoreQueues, damageTypes);
    }

    protected void DoExplode(
      Vector3 position,
      ExplosionData data,
      Vector2 sourceNormal,
      System.Action onExplosionBegin = null,
      bool ignoreQueues = false,
      CoreDamageTypes damageTypes = CoreDamageTypes.None,
      bool ignoreDamageCaps = false)
    {
      this.StartCoroutine(this.HandleExplosion(position, data, sourceNormal, onExplosionBegin, ignoreQueues, damageTypes, ignoreDamageCaps));
    }

    public static void DoRadialMajorBreakableDamage(float damage, Vector3 position, float radius)
    {
      List<MajorBreakable> allMajorBreakables = StaticReferenceManager.AllMajorBreakables;
      float num = radius * radius;
      if (allMajorBreakables == null)
        return;
      for (int index = 0; index < allMajorBreakables.Count; ++index)
      {
        MajorBreakable majorBreakable = allMajorBreakables[index];
        if ((bool) (UnityEngine.Object) majorBreakable && majorBreakable.enabled && !majorBreakable.IgnoreExplosions)
        {
          Vector2 sourceDirection = majorBreakable.CenterPoint - position.XY();
          if ((double) sourceDirection.sqrMagnitude < (double) num)
            majorBreakable.ApplyDamage(damage, sourceDirection, false, true);
        }
      }
    }

    public static void DoRadialIgnite(
      GameActorFireEffect fire,
      Vector3 position,
      float radius,
      VFXPool hitVFX = null)
    {
      List<HealthHaver> allHealthHavers = StaticReferenceManager.AllHealthHavers;
      if (allHealthHavers == null)
        return;
      float num = radius * radius;
      for (int index = 0; index < allHealthHavers.Count; ++index)
      {
        HealthHaver healthHaver = allHealthHavers[index];
        if ((bool) (UnityEngine.Object) healthHaver && healthHaver.gameObject.activeSelf && (bool) (UnityEngine.Object) healthHaver.aiActor)
        {
          AIActor aiActor = healthHaver.aiActor;
          if (!aiActor.IsGone && aiActor.isActiveAndEnabled && (double) (aiActor.CenterPosition - position.XY()).sqrMagnitude <= (double) num)
          {
            aiActor.ApplyEffect((GameActorEffect) fire);
            if (hitVFX != null)
            {
              if (aiActor.specRigidbody.HitboxPixelCollider != null)
              {
                PixelCollider pixelCollider = aiActor.specRigidbody.GetPixelCollider(ColliderType.HitBox);
                Vector2 position1 = BraveMathCollege.ClosestPointOnRectangle((Vector2) position, pixelCollider.UnitBottomLeft, pixelCollider.UnitDimensions);
                hitVFX.SpawnAtPosition((Vector3) position1);
              }
              else
                hitVFX.SpawnAtPosition((Vector3) aiActor.CenterPosition);
            }
          }
        }
      }
    }

    public static void DoRadialDamage(
      float damage,
      Vector3 position,
      float radius,
      bool damagePlayers,
      bool damageEnemies,
      bool ignoreDamageCaps = false,
      VFXPool hitVFX = null)
    {
      List<HealthHaver> allHealthHavers = StaticReferenceManager.AllHealthHavers;
      if (allHealthHavers == null)
        return;
      for (int index1 = 0; index1 < allHealthHavers.Count; ++index1)
      {
        HealthHaver healthHaver1 = allHealthHavers[index1];
        if ((bool) (UnityEngine.Object) healthHaver1 && healthHaver1.gameObject.activeSelf && (!(bool) (UnityEngine.Object) healthHaver1.aiActor || !healthHaver1.aiActor.IsGone) && (!(bool) (UnityEngine.Object) healthHaver1.aiActor || healthHaver1.aiActor.isActiveAndEnabled))
        {
          for (int index2 = 0; index2 < healthHaver1.NumBodyRigidbodies; ++index2)
          {
            SpeculativeRigidbody bodyRigidbody = healthHaver1.GetBodyRigidbody(index2);
            Vector2 vector2_1 = healthHaver1.transform.position.XY() - position.XY();
            bool flag1 = false;
            bool flag2 = false;
            float num1;
            if (bodyRigidbody.HitboxPixelCollider != null)
            {
              vector2_1 = bodyRigidbody.HitboxPixelCollider.UnitCenter - position.XY();
              num1 = BraveMathCollege.DistToRectangle(position.XY(), bodyRigidbody.HitboxPixelCollider.UnitBottomLeft, bodyRigidbody.HitboxPixelCollider.UnitDimensions);
            }
            else
            {
              vector2_1 = healthHaver1.transform.position.XY() - position.XY();
              num1 = vector2_1.magnitude;
            }
            if ((double) num1 < (double) radius)
            {
              PlayerController component = healthHaver1.GetComponent<PlayerController>();
              if ((UnityEngine.Object) component != (UnityEngine.Object) null)
              {
                bool flag3 = true;
                if (PassiveItem.ActiveFlagItems.ContainsKey(component) && PassiveItem.ActiveFlagItems[component].ContainsKey(typeof (HelmetItem)) && (double) num1 > (double) radius * (double) HelmetItem.EXPLOSION_RADIUS_MULTIPLIER)
                  flag3 = false;
                if (Exploder.IsPlayerBlockedByWall(component, (Vector2) position))
                  flag3 = false;
                if (damagePlayers && flag3 && !component.IsEthereal)
                {
                  HealthHaver healthHaver2 = healthHaver1;
                  float num2 = 0.5f;
                  Vector2 vector2_2 = vector2_1;
                  string enemiesString = StringTableManager.GetEnemiesString("#EXPLOSION");
                  CoreDamageTypes coreDamageTypes = CoreDamageTypes.None;
                  DamageCategory damageCategory = DamageCategory.Normal;
                  bool flag4 = ignoreDamageCaps;
                  double damage1 = (double) num2;
                  Vector2 direction = vector2_2;
                  string sourceName = enemiesString;
                  int damageTypes = (int) coreDamageTypes;
                  int num3 = (int) damageCategory;
                  int num4 = flag4 ? 1 : 0;
                  healthHaver2.ApplyDamage((float) damage1, direction, sourceName, (CoreDamageTypes) damageTypes, (DamageCategory) num3, ignoreDamageCaps: num4 != 0);
                  flag2 = true;
                }
              }
              else if (damageEnemies)
              {
                AIActor aiActor = healthHaver1.aiActor;
                if (damagePlayers || !(bool) (UnityEngine.Object) aiActor || aiActor.IsNormalEnemy)
                {
                  HealthHaver healthHaver3 = healthHaver1;
                  float num5 = damage;
                  Vector2 vector2_3 = vector2_1;
                  string enemiesString = StringTableManager.GetEnemiesString("#EXPLOSION");
                  CoreDamageTypes coreDamageTypes = CoreDamageTypes.None;
                  DamageCategory damageCategory = DamageCategory.Normal;
                  bool flag5 = ignoreDamageCaps;
                  double damage2 = (double) num5;
                  Vector2 direction = vector2_3;
                  string sourceName = enemiesString;
                  int damageTypes = (int) coreDamageTypes;
                  int num6 = (int) damageCategory;
                  int num7 = flag5 ? 1 : 0;
                  healthHaver3.ApplyDamage((float) damage2, direction, sourceName, (CoreDamageTypes) damageTypes, (DamageCategory) num6, ignoreDamageCaps: num7 != 0);
                  flag2 = true;
                }
              }
              flag1 = true;
            }
            if (flag2 && hitVFX != null)
            {
              if (bodyRigidbody.HitboxPixelCollider != null)
              {
                PixelCollider pixelCollider = bodyRigidbody.GetPixelCollider(ColliderType.HitBox);
                Vector2 position1 = BraveMathCollege.ClosestPointOnRectangle((Vector2) position, pixelCollider.UnitBottomLeft, pixelCollider.UnitDimensions);
                hitVFX.SpawnAtPosition((Vector3) position1);
              }
              else
                hitVFX.SpawnAtPosition((Vector3) healthHaver1.transform.position.XY());
            }
            if (flag1)
              break;
          }
        }
      }
    }

    private static bool IsPlayerBlockedByWall(PlayerController attachedPlayer, Vector2 explosionPos)
    {
      Vector2 centerPosition = attachedPlayer.CenterPosition;
      RaycastResult result;
      bool flag1 = PhysicsEngine.Instance.Raycast(explosionPos, centerPosition - explosionPos, Vector2.Distance(centerPosition, explosionPos), out result, collideWithRigidbodies: false);
      RaycastResult.Pool.Free(ref result);
      if (!flag1)
        return false;
      Vector2 unitTopCenter = attachedPlayer.specRigidbody.HitboxPixelCollider.UnitTopCenter;
      bool flag2 = PhysicsEngine.Instance.Raycast(explosionPos, unitTopCenter - explosionPos, Vector2.Distance(unitTopCenter, explosionPos), out result, collideWithRigidbodies: false);
      RaycastResult.Pool.Free(ref result);
      if (!flag2)
        return false;
      Vector2 unitBottomCenter = attachedPlayer.specRigidbody.PrimaryPixelCollider.UnitBottomCenter;
      bool flag3 = PhysicsEngine.Instance.Raycast(explosionPos, unitBottomCenter - explosionPos, Vector2.Distance(unitBottomCenter, explosionPos), out result, collideWithRigidbodies: false);
      RaycastResult.Pool.Free(ref result);
      return flag3;
    }

    public static void DoRadialMinorBreakableBreak(Vector3 position, float radius)
    {
      float num = radius * radius;
      List<MinorBreakable> allMinorBreakables = StaticReferenceManager.AllMinorBreakables;
      if (allMinorBreakables == null)
        return;
      for (int index = 0; index < allMinorBreakables.Count; ++index)
      {
        MinorBreakable minorBreakable = allMinorBreakables[index];
        if ((bool) (UnityEngine.Object) minorBreakable && !minorBreakable.resistsExplosions && !minorBreakable.OnlyBrokenByCode)
        {
          Vector2 vector2 = minorBreakable.CenterPoint - position.XY();
          if ((double) vector2.sqrMagnitude < (double) num)
            minorBreakable.Break(vector2.normalized);
        }
      }
    }

    public static void DoRadialPush(Vector3 position, float force, float radius)
    {
      float num1 = radius * radius;
      for (int index = 0; index < StaticReferenceManager.AllDebris.Count; ++index)
      {
        Vector2 vector2 = StaticReferenceManager.AllDebris[index].transform.position.XY() - position.XY();
        if ((double) vector2.sqrMagnitude < (double) num1)
        {
          float num2 = (float) (1.0 - (double) vector2.magnitude / (double) radius);
          StaticReferenceManager.AllDebris[index].ApplyVelocity(vector2.normalized * num2 * force * (float) (1.0 + (double) UnityEngine.Random.value / 5.0));
        }
      }
    }

    public static void DoRadialKnockback(Vector3 position, float force, float radius)
    {
      List<AIActor> allEnemies = StaticReferenceManager.AllEnemies;
      if (allEnemies == null)
        return;
      for (int index = 0; index < allEnemies.Count; ++index)
      {
        Vector2 vector2 = allEnemies[index].CenterPosition - position.XY();
        float magnitude = vector2.magnitude;
        if ((double) magnitude < (double) radius)
        {
          KnockbackDoer knockbackDoer = allEnemies[index].knockbackDoer;
          if ((bool) (UnityEngine.Object) knockbackDoer)
          {
            float num = (float) (1.0 - (double) magnitude / (double) radius);
            knockbackDoer.ApplyKnockback(vector2.normalized, num * force);
          }
        }
      }
    }

    public static void DoDistortionWave(
      Vector2 center,
      float distortionIntensity,
      float distortionRadius,
      float maxRadius,
      float duration)
    {
      Exploder component = new GameObject("temp_explosion_processor", new System.Type[1]
      {
        typeof (Exploder)
      }).GetComponent<Exploder>();
      component.StartCoroutine(component.DoDistortionWaveLocal(center, distortionIntensity, distortionRadius, maxRadius, duration));
    }

    private Vector4 GetCenterPointInScreenUV(Vector2 centerPoint, float dIntensity, float dRadius)
    {
      Vector3 viewportPoint = GameManager.Instance.MainCameraController.Camera.WorldToViewportPoint(centerPoint.ToVector3ZUp());
      return new Vector4(viewportPoint.x, viewportPoint.y, dRadius, dIntensity);
    }

    [DebuggerHidden]
    private IEnumerator DoDistortionWaveLocal(
      Vector2 center,
      float distortionIntensity,
      float distortionRadius,
      float maxRadius,
      float duration)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new Exploder__DoDistortionWaveLocalc__Iterator0()
      {
        center = center,
        distortionIntensity = distortionIntensity,
        distortionRadius = distortionRadius,
        duration = duration,
        maxRadius = maxRadius,
        _this = this
      };
    }

    public static void DoLinearPush(Vector2 p1, Vector2 p2, float force, float radius)
    {
      float num1 = radius * radius;
      for (int index = 0; index < StaticReferenceManager.AllDebris.Count; ++index)
      {
        Vector2 vector2_1 = StaticReferenceManager.AllDebris[index].transform.position.XY();
        float num2 = vector2_1.x - p1.x;
        float num3 = vector2_1.y - p1.y;
        float num4 = p2.x - p1.x;
        float num5 = p2.y - p1.y;
        float num6 = (float) ((double) num2 * (double) num4 + (double) num3 * (double) num5);
        float num7 = (float) ((double) num4 * (double) num4 + (double) num5 * (double) num5);
        float num8 = -1f;
        if ((double) num7 != 0.0)
          num8 = num6 / num7;
        float num9;
        float num10;
        if ((double) num8 < 0.0)
        {
          num9 = p1.x;
          num10 = p1.y;
        }
        else if ((double) num8 > 1.0)
        {
          num9 = p2.x;
          num10 = p2.y;
        }
        else
        {
          num9 = p1.x + num8 * num4;
          num10 = p1.y + num8 * num5;
        }
        Vector2 vector2_2 = new Vector2(vector2_1.x - num9, vector2_1.y - num10);
        if ((double) vector2_2.sqrMagnitude < (double) num1)
        {
          float num11 = (float) (1.0 - (double) vector2_2.magnitude / (double) radius);
          StaticReferenceManager.AllDebris[index].ApplyVelocity(vector2_2.normalized * num11 * force * (float) (1.0 + (double) UnityEngine.Random.value / 5.0));
        }
      }
    }

    [DebuggerHidden]
    private IEnumerator HandleCurrentExplosionNotification(float t)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new Exploder__HandleCurrentExplosionNotificationc__Iterator1()
      {
        t = t
      };
    }

    [DebuggerHidden]
    private IEnumerator HandleBulletDeletionFrames(
      Vector3 centerPosition,
      float bulletDeletionSqrRadius,
      float duration)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new Exploder__HandleBulletDeletionFramesc__Iterator2()
      {
        bulletDeletionSqrRadius = bulletDeletionSqrRadius,
        duration = duration,
        centerPosition = centerPosition
      };
    }

    [DebuggerHidden]
    private IEnumerator HandleCirc(tk2dSprite AdditiveCircSprite, float targetScale, float duration)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new Exploder__HandleCircc__Iterator3()
      {
        AdditiveCircSprite = AdditiveCircSprite,
        targetScale = targetScale,
        duration = duration
      };
    }

    [DebuggerHidden]
    private IEnumerator HandleExplosion(
      Vector3 position,
      ExplosionData data,
      Vector2 sourceNormal,
      System.Action onExplosionBegin,
      bool ignoreQueues,
      CoreDamageTypes damageTypes,
      bool ignoreDamageCaps)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new Exploder__HandleExplosionc__Iterator4()
      {
        data = data,
        damageTypes = damageTypes,
        ignoreQueues = ignoreQueues,
        onExplosionBegin = onExplosionBegin,
        position = position,
        sourceNormal = sourceNormal,
        ignoreDamageCaps = ignoreDamageCaps,
        _this = this
      };
    }
  }

