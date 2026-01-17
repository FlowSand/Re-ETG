// Decompiled with JetBrains decompiler
// Type: InstantlyDamageAllProjectile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Combat.Projectiles
{
    public class InstantlyDamageAllProjectile : Projectile
    {
      public bool DoesWhiteFlash;
      public bool DoesCameraFlash;
      public bool DoesAmbientVFX;
      public float AmbientVFXTime;
      public GameObject AmbientVFX;
      public float minTimeBetweenAmbientVFX = 0.1f;
      public GameObject DamagedEnemyVFX;
      [Header("Radial Slow Options")]
      public bool DoesRadialSlow;
      [ShowInInspectorIf("DoesRadialSlow", false)]
      public float RadialSlowInTime;
      [ShowInInspectorIf("DoesRadialSlow", false)]
      public float RadialSlowHoldTime = 1f;
      [ShowInInspectorIf("DoesRadialSlow", false)]
      public float RadialSlowOutTime = 0.5f;
      [ShowInInspectorIf("DoesRadialSlow", false)]
      public float RadialSlowTimeModifier = 0.25f;
      private float m_ambientTimer;

      protected override void Move()
      {
        if (this.DoesWhiteFlash)
          Pixelator.Instance.FadeToColor(0.1f, Color.white, true, 0.1f);
        if (this.DoesCameraFlash)
        {
          StickyFrictionManager.Instance.RegisterCustomStickyFriction(0.125f, 0.0f, false);
          Pixelator.Instance.TimedFreezeFrame(0.25f, 0.125f);
        }
        if (this.DoesAmbientVFX && (double) this.AmbientVFXTime > 0.0 && (UnityEngine.Object) this.AmbientVFX != (UnityEngine.Object) null)
          GameManager.Instance.Dungeon.StartCoroutine(this.HandleAmbientSpawnTime((Vector2) this.transform.position, this.AmbientVFXTime));
        this.transform.position.GetAbsoluteRoom().ApplyActionToNearbyEnemies(this.transform.position.XY(), 100f, new Action<AIActor, float>(this.ProcessEnemy));
        ReadOnlyCollection<Projectile> allProjectiles = StaticReferenceManager.AllProjectiles;
        for (int index = allProjectiles.Count - 1; index >= 0; --index)
        {
          Projectile projectile = allProjectiles[index];
          if ((bool) (UnityEngine.Object) projectile && projectile.collidesWithProjectiles && (!projectile.collidesOnlyWithPlayerProjectiles || this.Owner is PlayerController))
          {
            BounceProjModifier component = projectile.GetComponent<BounceProjModifier>();
            if ((bool) (UnityEngine.Object) component)
            {
              if (component.numberOfBounces <= 0)
              {
                projectile.DieInAir();
              }
              else
              {
                projectile.Direction *= -1f;
                float angle = projectile.Direction.ToAngle();
                if (this.shouldRotate)
                  this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle);
                projectile.Speed *= 1f - component.percentVelocityToLoseOnBounce;
                if ((bool) (UnityEngine.Object) this.braveBulletScript && this.braveBulletScript.bullet != null)
                {
                  this.braveBulletScript.bullet.Direction = angle;
                  this.braveBulletScript.bullet.Speed *= 1f - component.percentVelocityToLoseOnBounce;
                }
                component.Bounce((Projectile) this, projectile.specRigidbody.UnitCenter);
              }
            }
          }
        }
        this.DieInAir();
      }

      protected void HandleAmbientVFXSpawn(Vector2 centerPoint, float radius)
      {
        if ((UnityEngine.Object) this.AmbientVFX == (UnityEngine.Object) null)
          return;
        bool flag = false;
        this.m_ambientTimer -= BraveTime.DeltaTime;
        if ((double) this.m_ambientTimer <= 0.0)
        {
          flag = true;
          this.m_ambientTimer = this.minTimeBetweenAmbientVFX;
        }
        if (!flag)
          return;
        SpawnManager.SpawnVFX(this.AmbientVFX, (Vector3) (centerPoint + UnityEngine.Random.insideUnitCircle * radius), Quaternion.identity);
      }

      [DebuggerHidden]
      protected IEnumerator HandleAmbientSpawnTime(Vector2 centerPoint, float remainingTime)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new InstantlyDamageAllProjectile__HandleAmbientSpawnTimec__Iterator0()
        {
          remainingTime = remainingTime,
          centerPoint = centerPoint,
          _this = this
        };
      }

      public void ProcessEnemy(AIActor a, float b)
      {
        if (!(bool) (UnityEngine.Object) a || !a.IsNormalEnemy || !(bool) (UnityEngine.Object) a.healthHaver || a.IsGone)
          return;
        if ((bool) (UnityEngine.Object) this.Owner)
          a.healthHaver.ApplyDamage(this.ModifiedDamage, Vector2.zero, this.OwnerName, this.damageTypes);
        else
          a.healthHaver.ApplyDamage(this.ModifiedDamage, Vector2.zero, "projectile", this.damageTypes);
        if (this.DoesRadialSlow)
          this.ApplySlowToEnemy(a);
        if (this.AppliesStun && a.healthHaver.IsAlive && (bool) (UnityEngine.Object) a.behaviorSpeculator && (double) UnityEngine.Random.value < (double) this.StunApplyChance)
          a.behaviorSpeculator.Stun(this.AppliedStunDuration);
        if (!((UnityEngine.Object) this.DamagedEnemyVFX != (UnityEngine.Object) null))
          return;
        a.PlayEffectOnActor(this.DamagedEnemyVFX, Vector3.zero, false, true);
      }

      protected void ApplySlowToEnemy(AIActor target)
      {
        target.StartCoroutine(this.ProcessSlow(target));
      }

      [DebuggerHidden]
      private IEnumerator ProcessSlow(AIActor target)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new InstantlyDamageAllProjectile__ProcessSlowc__Iterator1()
        {
          target = target,
          _this = this
        };
      }
    }

}
