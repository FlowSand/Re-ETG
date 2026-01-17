// Decompiled with JetBrains decompiler
// Type: SilencerInstance
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Combat.Projectiles
{
    public class SilencerInstance : MonoBehaviour
    {
      public static float? s_MaxRadiusLimiter;
      private Camera m_camera;
      private Material m_distortionMaterial;
      private float dIntensity;
      private float dRadius;
      public bool ForceNoDamage;
      public bool UsesCustomProjectileCallback;
      public Action<Projectile> OnCustomBlankedProjectile;

      public void TriggerSilencer(
        Vector2 centerPoint,
        float expandSpeed,
        float maxRadius,
        GameObject silencerVFX,
        float distIntensity,
        float distRadius,
        float pushForce,
        float pushRadius,
        float knockbackForce,
        float knockbackRadius,
        float additionalTimeAtMaxRadius,
        PlayerController user,
        bool breaksWalls = true,
        bool skipBreakables = false)
      {
        bool flag = true;
        float damage = 10f;
        float num1 = 7f;
        float num2 = 1f;
        if ((double) maxRadius < 5.0)
        {
          flag = true;
          damage = 10f;
          num1 = maxRadius;
        }
        if (SilencerInstance.s_MaxRadiusLimiter.HasValue)
          maxRadius = SilencerInstance.s_MaxRadiusLimiter.Value;
        bool shouldReflectInstead = false;
        if ((UnityEngine.Object) user != (UnityEngine.Object) null)
        {
          for (int index = 0; index < user.passiveItems.Count; ++index)
          {
            BlankModificationItem passiveItem = user.passiveItems[index] as BlankModificationItem;
            if ((UnityEngine.Object) passiveItem != (UnityEngine.Object) null)
            {
              if (passiveItem.BlankReflectsEnemyBullets)
                shouldReflectInstead = true;
              if (passiveItem.MakeBlankDealDamage)
              {
                flag = true;
                damage += passiveItem.BlankDamage;
                num1 = Mathf.Max(num1, passiveItem.BlankDamageRadius);
              }
              num2 *= passiveItem.BlankForceMultiplier;
              this.ProcessBlankModificationItemAdditionalEffects(passiveItem, centerPoint, user);
            }
          }
        }
        if ((bool) (UnityEngine.Object) user && user.HasActiveBonusSynergy(CustomSynergyType.ELDER_BLANK_BULLETS))
          shouldReflectInstead = true;
        this.dIntensity = distIntensity;
        this.dRadius = distRadius;
        this.m_camera = GameManager.Instance.MainCameraController.GetComponent<Camera>();
        if ((UnityEngine.Object) silencerVFX != (UnityEngine.Object) null)
          UnityEngine.Object.Destroy((UnityEngine.Object) UnityEngine.Object.Instantiate<GameObject>(silencerVFX, centerPoint.ToVector3ZUp(centerPoint.y), Quaternion.identity), 1f);
        Exploder.DoRadialPush(centerPoint.ToVector3ZUp(), pushForce, pushRadius);
        Exploder.DoRadialKnockback(centerPoint.ToVector3ZUp(), knockbackForce * num2, knockbackRadius);
        if (!skipBreakables)
          Exploder.DoRadialMinorBreakableBreak(centerPoint.ToVector3ZUp(), knockbackRadius);
        if (breaksWalls)
        {
          RoomHandler roomFromPosition = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(centerPoint.ToIntVector2(VectorConversions.Floor));
          for (int index = 0; index < StaticReferenceManager.AllMajorBreakables.Count; ++index)
          {
            if (StaticReferenceManager.AllMajorBreakables[index].IsSecretDoor && GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(StaticReferenceManager.AllMajorBreakables[index].transform.position.IntXY(VectorConversions.Floor)) == roomFromPosition)
            {
              StaticReferenceManager.AllMajorBreakables[index].ApplyDamage(1E+10f, Vector2.zero, false, true, true);
              StaticReferenceManager.AllMajorBreakables[index].ApplyDamage(1E+10f, Vector2.zero, false, true, true);
              StaticReferenceManager.AllMajorBreakables[index].ApplyDamage(1E+10f, Vector2.zero, false, true, true);
            }
          }
        }
        if (flag && !this.ForceNoDamage)
          Exploder.DoRadialDamage(damage, centerPoint.ToVector3ZUp(), num1, false, true);
        if ((double) distIntensity > 0.0)
        {
          this.m_distortionMaterial = new Material(ShaderCache.Acquire("Brave/Internal/DistortionWave"));
          this.m_distortionMaterial.SetVector("_WaveCenter", this.GetCenterPointInScreenUV(centerPoint));
          Pixelator.Instance.RegisterAdditionalRenderPass(this.m_distortionMaterial);
        }
        if ((double) maxRadius > 10.0)
        {
          List<BulletScriptSource> bulletScriptSources = StaticReferenceManager.AllBulletScriptSources;
          for (int index = 0; index < bulletScriptSources.Count; ++index)
          {
            BulletScriptSource bulletScriptSource = bulletScriptSources[index];
            if (!bulletScriptSource.IsEnded && bulletScriptSource.RootBullet != null && bulletScriptSource.RootBullet.EndOnBlank)
              bulletScriptSource.ForceStop();
          }
          ReadOnlyCollection<Projectile> allProjectiles = StaticReferenceManager.AllProjectiles;
          for (int index = allProjectiles.Count - 1; index >= 0; --index)
          {
            Projectile projectile = allProjectiles[index];
            if ((bool) (UnityEngine.Object) projectile.braveBulletScript && projectile.braveBulletScript.bullet != null && projectile.braveBulletScript.bullet.EndOnBlank)
            {
              if (this.UsesCustomProjectileCallback && this.OnCustomBlankedProjectile != null)
                this.OnCustomBlankedProjectile(projectile);
              projectile.DieInAir(killedEarly: true);
            }
          }
        }
        Pixelator.Instance.StartCoroutine(this.BackupDistortionCleanup());
        this.StartCoroutine(this.HandleSilence(centerPoint, expandSpeed, maxRadius, additionalTimeAtMaxRadius, user, shouldReflectInstead));
      }

      private void ProcessBlankModificationItemAdditionalEffects(
        BlankModificationItem bmi,
        Vector2 centerPoint,
        PlayerController user)
      {
        List<AIActor> activeEnemies = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(centerPoint.ToIntVector2()).GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
        if ((double) bmi.RegainAmmoFraction > 0.0)
        {
          for (int index = 0; index < user.inventory.AllGuns.Count; ++index)
          {
            Gun allGun = user.inventory.AllGuns[index];
            if (allGun.CanGainAmmo)
              allGun.GainAmmo(Mathf.CeilToInt((float) allGun.AdjustedMaxAmmo * bmi.RegainAmmoFraction));
          }
        }
        if (activeEnemies == null)
          return;
        for (int index = 0; index < activeEnemies.Count; ++index)
        {
          AIActor aiActor = activeEnemies[index];
          if ((double) Vector2.Distance(centerPoint, aiActor.CenterPosition) <= (double) bmi.BlankDamageRadius)
          {
            if ((double) bmi.BlankStunTime > 0.0 && (bool) (UnityEngine.Object) aiActor.behaviorSpeculator)
              aiActor.behaviorSpeculator.Stun(bmi.BlankStunTime);
            if ((double) bmi.BlankFireChance > 0.0 && (double) UnityEngine.Random.value < (double) bmi.BlankFireChance)
            {
              UnityEngine.Debug.Log((object) "appling fire...");
              aiActor.ApplyEffect((GameActorEffect) bmi.BlankFireEffect);
            }
            if ((double) bmi.BlankPoisonChance > 0.0 && (double) UnityEngine.Random.value < (double) bmi.BlankPoisonChance)
              aiActor.ApplyEffect((GameActorEffect) bmi.BlankPoisonEffect);
            if ((double) bmi.BlankFreezeChance > 0.0 && (double) UnityEngine.Random.value < (double) bmi.BlankFreezeChance)
              aiActor.ApplyEffect((GameActorEffect) bmi.BlankFreezeEffect);
          }
        }
      }

      private Vector4 GetCenterPointInScreenUV(Vector2 centerPoint)
      {
        Vector3 viewportPoint = this.m_camera.WorldToViewportPoint(centerPoint.ToVector3ZUp());
        return new Vector4(viewportPoint.x, viewportPoint.y, this.dRadius, this.dIntensity);
      }

      [DebuggerHidden]
      private IEnumerator HandleSilence(
        Vector2 centerPoint,
        float expandSpeed,
        float maxRadius,
        float additionalTimeAtMaxRadius,
        PlayerController user,
        bool shouldReflectInstead)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new SilencerInstance.\u003CHandleSilence\u003Ec__Iterator0()
        {
          maxRadius = maxRadius,
          expandSpeed = expandSpeed,
          centerPoint = centerPoint,
          user = user,
          shouldReflectInstead = shouldReflectInstead,
          additionalTimeAtMaxRadius = additionalTimeAtMaxRadius,
          \u0024this = this
        };
      }

      private void CleanupDistortion()
      {
        if (!((UnityEngine.Object) Pixelator.Instance != (UnityEngine.Object) null) || !((UnityEngine.Object) this.m_distortionMaterial != (UnityEngine.Object) null))
          return;
        Pixelator.Instance.DeregisterAdditionalRenderPass(this.m_distortionMaterial);
        UnityEngine.Object.Destroy((UnityEngine.Object) this.m_distortionMaterial);
        this.m_distortionMaterial = (Material) null;
      }

      private void OnDestroy() => this.CleanupDistortion();

      [DebuggerHidden]
      private IEnumerator BackupDistortionCleanup()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new SilencerInstance.\u003CBackupDistortionCleanup\u003Ec__Iterator1()
        {
          \u0024this = this
        };
      }

      public static void DestroyBulletsInRange(
        Vector2 centerPoint,
        float radius,
        bool destroysEnemyBullets,
        bool destroysPlayerBullets,
        PlayerController user = null,
        bool reflectsBullets = false,
        float? previousRadius = null,
        bool useCallback = false,
        Action<Projectile> callback = null)
      {
        float num1 = radius * radius;
        float num2 = !previousRadius.HasValue ? 0.0f : previousRadius.Value * previousRadius.Value;
        List<Projectile> projectileList = new List<Projectile>();
        ReadOnlyCollection<Projectile> allProjectiles = StaticReferenceManager.AllProjectiles;
        for (int index = 0; index < allProjectiles.Count; ++index)
        {
          Projectile projectile = allProjectiles[index];
          if ((bool) (UnityEngine.Object) projectile && (bool) (UnityEngine.Object) projectile.sprite)
          {
            float sqrMagnitude = (projectile.sprite.WorldCenter - centerPoint).sqrMagnitude;
            if ((double) sqrMagnitude <= (double) num1 && !projectile.ImmuneToBlanks && (!previousRadius.HasValue || !projectile.ImmuneToSustainedBlanks || (double) sqrMagnitude >= (double) num2))
            {
              if ((UnityEngine.Object) projectile.Owner != (UnityEngine.Object) null)
              {
                if (projectile.isFakeBullet || projectile.Owner is AIActor || (UnityEngine.Object) projectile.Shooter != (UnityEngine.Object) null && (UnityEngine.Object) projectile.Shooter.aiActor != (UnityEngine.Object) null || projectile.ForcePlayerBlankable)
                {
                  if (destroysEnemyBullets)
                    projectileList.Add(projectile);
                }
                else if (projectile.Owner is PlayerController)
                {
                  if (destroysPlayerBullets && (UnityEngine.Object) projectile.Owner != (UnityEngine.Object) user)
                    projectileList.Add(projectile);
                }
                else
                  UnityEngine.Debug.LogError((object) "Silencer is trying to process a bullet that is owned by something that is neither man nor beast!");
              }
              else if (destroysEnemyBullets)
                projectileList.Add(projectile);
            }
          }
        }
        for (int index = 0; index < projectileList.Count; ++index)
        {
          if (!destroysPlayerBullets && reflectsBullets)
          {
            PassiveReflectItem.ReflectBullet(projectileList[index], true, (GameActor) user, 10f);
          }
          else
          {
            if ((bool) (UnityEngine.Object) projectileList[index] && (bool) (UnityEngine.Object) projectileList[index].GetComponent<ChainLightningModifier>())
              UnityEngine.Object.Destroy((UnityEngine.Object) projectileList[index].GetComponent<ChainLightningModifier>());
            if (useCallback && callback != null)
              callback(projectileList[index]);
            projectileList[index].DieInAir(killedEarly: true);
          }
        }
        List<BasicTrapController> allTriggeredTraps = StaticReferenceManager.AllTriggeredTraps;
        for (int index = allTriggeredTraps.Count - 1; index >= 0; --index)
        {
          BasicTrapController basicTrapController = allTriggeredTraps[index];
          if ((bool) (UnityEngine.Object) basicTrapController && basicTrapController.triggerOnBlank && (double) (basicTrapController.CenterPoint() - centerPoint).sqrMagnitude < (double) num1)
            basicTrapController.Trigger();
        }
      }
    }

}
