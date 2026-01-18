// Decompiled with JetBrains decompiler
// Type: RadialBurstInterface
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

[Serializable]
public class RadialBurstInterface
  {
    public PlayerItemProjectileInterface ProjectileInterface;
    public int MinToSpawnPerWave = 10;
    public int MaxToSpawnPerWave = 10;
    public int NumberWaves = 1;
    public int NumberSubwaves = 1;
    public float TimeBetweenWaves = 1f;
    public bool SpiralWaves;
    public bool AlignFirstShot;
    public float AlignOffset;
    public bool SweepBeams;
    public float BeamSweepDegrees = 360f;
    public bool AimFirstAtNearestEnemy;
    public bool FixOverlapCollision;
    public bool ForceAllowGoop;
    public Action<Projectile> CustomPostProcessProjectile;

    public void DoBurst(
      PlayerController source,
      Vector2? overrideSpawnPoint = null,
      Vector2? spawnPointOffset = null)
    {
      if (this.NumberWaves == 1 && !this.SpiralWaves)
        this.ImmediateBurst(this.ProjectileInterface.GetProjectile(source), source, overrideSpawnPoint, spawnPointOffset);
      else
        source.StartCoroutine(this.HandleBurst(this.ProjectileInterface.GetProjectile(source), source, overrideSpawnPoint, spawnPointOffset));
    }

    private AIActor GetNearestEnemy(Vector2 sourcePoint)
    {
      RoomHandler absoluteRoom = sourcePoint.GetAbsoluteRoom();
      float nearestDistance = 0.0f;
      return absoluteRoom.GetNearestEnemy(sourcePoint, out nearestDistance, excludeDying: true);
    }

    private void ImmediateBurst(
      Projectile projectileToSpawn,
      PlayerController source,
      Vector2? overrideSpawnPoint,
      Vector2? spawnPointOffset = null)
    {
      if ((UnityEngine.Object) projectileToSpawn == (UnityEngine.Object) null)
        return;
      int num1 = UnityEngine.Random.Range(this.MinToSpawnPerWave, this.MaxToSpawnPerWave);
      int radialBurstLimit = projectileToSpawn.GetRadialBurstLimit(source);
      if (radialBurstLimit < num1)
        num1 = radialBurstLimit;
      float max = 360f / (float) num1;
      float num2 = UnityEngine.Random.Range(0.0f, max);
      if (this.AlignFirstShot && (bool) (UnityEngine.Object) source && (bool) (UnityEngine.Object) source.CurrentGun)
        num2 = source.CurrentGun.CurrentAngle + this.AlignOffset;
      if (this.AimFirstAtNearestEnemy)
      {
        Vector2 vector2 = !overrideSpawnPoint.HasValue ? source.CenterPosition : overrideSpawnPoint.Value;
        Vector2 sourcePoint = !spawnPointOffset.HasValue ? vector2 : vector2 + spawnPointOffset.Value;
        AIActor nearestEnemy = this.GetNearestEnemy(sourcePoint);
        if ((bool) (UnityEngine.Object) nearestEnemy)
          num2 = Vector2.Angle(Vector2.right, nearestEnemy.CenterPosition - sourcePoint);
      }
      bool flag = (UnityEngine.Object) projectileToSpawn.GetComponent<BeamController>() != (UnityEngine.Object) null;
      for (int index = 0; index < num1; ++index)
      {
        float targetAngle = num2 + max * (float) index;
        if (flag)
          source.StartCoroutine(this.HandleFireShortBeam(projectileToSpawn, source, targetAngle, 1f * (float) this.NumberWaves, overrideSpawnPoint, spawnPointOffset));
        else
          this.DoSingleProjectile(projectileToSpawn, source, targetAngle, overrideSpawnPoint, spawnPointOffset);
      }
    }

    [DebuggerHidden]
    private IEnumerator HandleBurst(
      Projectile projectileToSpawn,
      PlayerController source,
      Vector2? overrideSpawnPoint,
      Vector2? spawnPointOffset = null)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new RadialBurstInterface__HandleBurstc__Iterator0()
      {
        projectileToSpawn = projectileToSpawn,
        source = source,
        overrideSpawnPoint = overrideSpawnPoint,
        spawnPointOffset = spawnPointOffset,
        _this = this
      };
    }

    [DebuggerHidden]
    private IEnumerator HandleFireShortBeam(
      Projectile projectileToSpawn,
      PlayerController source,
      float targetAngle,
      float duration,
      Vector2? overrideSpawnPoint,
      Vector2? spawnPointOffset = null)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new RadialBurstInterface__HandleFireShortBeamc__Iterator1()
      {
        projectileToSpawn = projectileToSpawn,
        source = source,
        targetAngle = targetAngle,
        overrideSpawnPoint = overrideSpawnPoint,
        spawnPointOffset = spawnPointOffset,
        duration = duration,
        _this = this
      };
    }

    private void DoSingleProjectile(
      Projectile projectileToSpawn,
      PlayerController source,
      float targetAngle,
      Vector2? overrideSpawnPoint,
      Vector2? spawnPointOffset = null)
    {
      Vector2 vector2 = !overrideSpawnPoint.HasValue ? source.specRigidbody.UnitCenter : overrideSpawnPoint.Value;
      Vector2 position = !spawnPointOffset.HasValue ? vector2 : vector2 + spawnPointOffset.Value;
      Projectile component = SpawnManager.SpawnProjectile(projectileToSpawn.gameObject, (Vector3) position, Quaternion.Euler(0.0f, 0.0f, targetAngle)).GetComponent<Projectile>();
      component.Owner = (GameActor) source;
      component.Shooter = source.specRigidbody;
      source.DoPostProcessProjectile(component);
      if (this.CustomPostProcessProjectile != null)
        this.CustomPostProcessProjectile(component);
      this.InternalPostProcessProjectile(component);
    }

    private void InternalPostProcessProjectile(Projectile proj)
    {
      if ((bool) (UnityEngine.Object) proj && !this.ForceAllowGoop)
      {
        GoopModifier component = proj.GetComponent<GoopModifier>();
        if ((bool) (UnityEngine.Object) component)
          UnityEngine.Object.Destroy((UnityEngine.Object) component);
      }
      if (!this.FixOverlapCollision || !(bool) (UnityEngine.Object) proj || !(bool) (UnityEngine.Object) proj.specRigidbody)
        return;
      PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(proj.specRigidbody);
    }

    private BeamController BeginFiringBeam(
      Projectile projectileToSpawn,
      PlayerController source,
      float targetAngle,
      Vector2? overrideSpawnPoint,
      Vector2? spawnPointOffset = null)
    {
      Vector2 vector2 = !overrideSpawnPoint.HasValue ? source.CenterPosition : overrideSpawnPoint.Value;
      Vector2 position = !spawnPointOffset.HasValue ? vector2 : vector2 + spawnPointOffset.Value;
      GameObject gameObject = SpawnManager.SpawnProjectile(projectileToSpawn.gameObject, (Vector3) position, Quaternion.identity);
      Projectile component1 = gameObject.GetComponent<Projectile>();
      component1.Owner = (GameActor) source;
      BeamController component2 = gameObject.GetComponent<BeamController>();
      component2.Owner = (GameActor) source;
      component2.HitsPlayers = false;
      component2.HitsEnemies = true;
      Vector3 vector = (Vector3) BraveMathCollege.DegreesToVector(targetAngle);
      component2.Direction = (Vector2) vector;
      component2.Origin = position;
      this.InternalPostProcessProjectile(component1);
      return component2;
    }

    private void ContinueFiringBeam(
      BeamController beam,
      PlayerController source,
      Vector2? overrideSpawnPoint,
      Vector2? spawnPointOffset = null)
    {
      Vector2 vector2 = !overrideSpawnPoint.HasValue ? source.CenterPosition : overrideSpawnPoint.Value;
      Vector2 origin = !spawnPointOffset.HasValue ? vector2 : vector2 + spawnPointOffset.Value;
      beam.Origin = origin;
      beam.LateUpdatePosition((Vector3) origin);
    }

    private void CeaseBeam(BeamController beam) => beam.CeaseAttack();
  }

