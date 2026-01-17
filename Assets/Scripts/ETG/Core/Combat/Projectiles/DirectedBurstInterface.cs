// Decompiled with JetBrains decompiler
// Type: DirectedBurstInterface
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Combat.Projectiles
{
    [Serializable]
    public class DirectedBurstInterface
    {
      public PlayerItemProjectileInterface ProjectileInterface;
      public int MinToSpawnPerWave = 10;
      public int MaxToSpawnPerWave = 10;
      public int NumberWaves = 1;
      public float TimeBetweenWaves = 1f;
      public bool SpiralWaves;
      public float AngleSubtended = 30f;
      public bool UseShotgunStyleVelocityModifier;
      public bool ForceAllowGoop;

      public void DoBurst(PlayerController source, float aimAngle)
      {
        if (this.NumberWaves == 1 && !this.SpiralWaves)
          this.ImmediateBurst(this.ProjectileInterface.GetProjectile(source), source, aimAngle);
        else
          source.StartCoroutine(this.HandleBurst(this.ProjectileInterface.GetProjectile(source), source, aimAngle));
      }

      private void ImmediateBurst(
        Projectile projectileToSpawn,
        PlayerController source,
        float aimAngle)
      {
        if ((UnityEngine.Object) projectileToSpawn == (UnityEngine.Object) null)
          return;
        int num1 = UnityEngine.Random.Range(this.MinToSpawnPerWave, this.MaxToSpawnPerWave);
        float num2 = this.AngleSubtended / (float) num1;
        float num3 = (float) -((double) this.AngleSubtended / 2.0) + aimAngle;
        bool flag = (UnityEngine.Object) projectileToSpawn.GetComponent<BeamController>() != (UnityEngine.Object) null;
        for (int index = 0; index < num1; ++index)
        {
          float targetAngle = num3 + num2 * (float) index;
          if (flag)
            source.StartCoroutine(this.HandleFireShortBeam(projectileToSpawn, source, targetAngle, 1f * (float) this.NumberWaves));
          else
            this.DoSingleProjectile(projectileToSpawn, source, targetAngle);
        }
      }

      [DebuggerHidden]
      private IEnumerator HandleBurst(
        Projectile projectileToSpawn,
        PlayerController source,
        float aimAngle)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new DirectedBurstInterface.<HandleBurst>c__Iterator0()
        {
          projectileToSpawn = projectileToSpawn,
          aimAngle = aimAngle,
          source = source,
          _this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator HandleFireShortBeam(
        Projectile projectileToSpawn,
        PlayerController source,
        float targetAngle,
        float duration)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new DirectedBurstInterface.<HandleFireShortBeam>c__Iterator1()
        {
          projectileToSpawn = projectileToSpawn,
          source = source,
          targetAngle = targetAngle,
          duration = duration,
          _this = this
        };
      }

      private void DoSingleProjectile(
        Projectile projectileToSpawn,
        PlayerController source,
        float targetAngle)
      {
        Vector2 position = !(bool) (UnityEngine.Object) source.CurrentGun || !(bool) (UnityEngine.Object) source.CurrentGun.barrelOffset ? source.specRigidbody.UnitCenter : source.CurrentGun.barrelOffset.position.XY();
        Projectile component = SpawnManager.SpawnProjectile(projectileToSpawn.gameObject, (Vector3) position, Quaternion.Euler(0.0f, 0.0f, targetAngle)).GetComponent<Projectile>();
        component.Owner = (GameActor) source;
        component.Shooter = source.specRigidbody;
        if (this.MinToSpawnPerWave == 1 && this.MaxToSpawnPerWave == 1 && this.NumberWaves == 1 && !this.SpiralWaves && this.ProjectileInterface.UseCurrentGunProjectile && source.HasActiveBonusSynergy(CustomSynergyType.DOUBLE_HOLSTER))
        {
          HomingModifier homingModifier = component.gameObject.GetComponent<HomingModifier>();
          if ((UnityEngine.Object) homingModifier == (UnityEngine.Object) null)
          {
            homingModifier = component.gameObject.AddComponent<HomingModifier>();
            homingModifier.HomingRadius = 0.0f;
            homingModifier.AngularVelocity = 0.0f;
          }
          homingModifier.HomingRadius += 20f;
          homingModifier.AngularVelocity += 1080f;
        }
        if (this.UseShotgunStyleVelocityModifier)
          component.baseData.speed *= (float) (1.0 + (double) UnityEngine.Random.Range(-15f, 15f) / 100.0);
        source.DoPostProcessProjectile(component);
        this.InternalPostProcessProjectile(component);
      }

      private void InternalPostProcessProjectile(Projectile proj)
      {
        if (!(bool) (UnityEngine.Object) proj || this.ForceAllowGoop)
          return;
        GoopModifier component = proj.GetComponent<GoopModifier>();
        if (!(bool) (UnityEngine.Object) component)
          return;
        UnityEngine.Object.Destroy((UnityEngine.Object) component);
      }

      private BeamController BeginFiringBeam(
        Projectile projectileToSpawn,
        PlayerController source,
        float targetAngle)
      {
        GameObject gameObject = SpawnManager.SpawnProjectile(projectileToSpawn.gameObject, (Vector3) source.CenterPosition, Quaternion.identity);
        Projectile component1 = gameObject.GetComponent<Projectile>();
        component1.Owner = (GameActor) source;
        BeamController component2 = gameObject.GetComponent<BeamController>();
        component2.Owner = (GameActor) source;
        component2.HitsPlayers = false;
        component2.HitsEnemies = true;
        Vector3 vector = (Vector3) BraveMathCollege.DegreesToVector(targetAngle);
        component2.Direction = (Vector2) vector;
        component2.Origin = source.CenterPosition;
        this.InternalPostProcessProjectile(component1);
        return component2;
      }

      private void ContinueFiringBeam(BeamController beam, PlayerController source)
      {
        beam.Origin = source.CenterPosition;
        beam.LateUpdatePosition((Vector3) source.CenterPosition);
      }

      private void CeaseBeam(BeamController beam) => beam.CeaseAttack();
    }

}
