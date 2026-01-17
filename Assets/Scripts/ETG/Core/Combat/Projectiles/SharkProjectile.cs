// Decompiled with JetBrains decompiler
// Type: SharkProjectile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Combat.Projectiles
{
    public class SharkProjectile : Projectile
    {
      public VFXPool enemyNotKilledPool;
      public DirectionalAnimation animData;
      public GoopDefinition waterGoop;
      public float goopRadius = 1f;
      public float angularAcceleration = 10f;
      public ParticleSystem ParticlesPrefab;
      [NonSerialized]
      protected GameActor CurrentTarget;
      [NonSerialized]
      protected bool m_coroutineIsActive;
      protected bool CanCrossPits;
      private AIActor m_cachedTargetToEat;
      protected Vector2? m_lastGoopPosition;
      protected Path m_currentPath;
      protected float m_pathTimer;
      protected Vector2? m_overridePathEnd;
      private static Vector3 m_lastAssignedScale = Vector3.one;
      private float m_noTargetElapsed;

      public override void Start()
      {
        base.Start();
        this.StartCoroutine(this.AddLowObstacleCollider());
        this.specRigidbody.OnPreRigidbodyCollision += new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.PassThroughTables);
        RoomHandler roomFromPosition = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(this.transform.position.XY().ToIntVector2());
        if (roomFromPosition != null && roomFromPosition.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.BOSS)
          this.CanCrossPits = true;
        if (!this.CanCrossPits)
          this.specRigidbody.MovementRestrictor += new SpeculativeRigidbody.MovementRestrictorDelegate(this.NoPits);
        SharkProjectile sharkProjectile1 = this;
        sharkProjectile1.OnHitEnemy = sharkProjectile1.OnHitEnemy + new Action<Projectile, SpeculativeRigidbody, bool>(this.HandleHitEnemy);
        SharkProjectile sharkProjectile2 = this;
        sharkProjectile2.OnWillKillEnemy = sharkProjectile2.OnWillKillEnemy + new Action<Projectile, SpeculativeRigidbody>(this.MaybeEatEnemy);
      }

      private void PassThroughTables(
        SpeculativeRigidbody myRigidbody,
        PixelCollider myPixelCollider,
        SpeculativeRigidbody otherRigidbody,
        PixelCollider otherPixelCollider)
      {
        if (!(bool) (UnityEngine.Object) otherRigidbody.GetComponent<FlippableCover>())
          return;
        PhysicsEngine.SkipCollision = true;
      }

      [DebuggerHidden]
      private IEnumerator AddLowObstacleCollider()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new SharkProjectile.<AddLowObstacleCollider>c__Iterator0()
        {
          _this = this
        };
      }

      private void HandleHitEnemy(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
      {
        if ((bool) (UnityEngine.Object) this.Owner && this.Owner is PlayerController && (this.Owner as PlayerController).HasActiveBonusSynergy(CustomSynergyType.EXPLOSIVE_SHARKS))
          Exploder.DoDefaultExplosion(this.specRigidbody.UnitCenter.ToVector3ZisY(), Vector2.zero);
        this.StartCoroutine(this.FrameDelayedDestruction());
      }

      [DebuggerHidden]
      private IEnumerator FrameDelayedDestruction()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new SharkProjectile.<FrameDelayedDestruction>c__Iterator1()
        {
          _this = this
        };
      }

      private void NoPits(
        SpeculativeRigidbody specRigidbody,
        IntVector2 prevPixelOffset,
        IntVector2 pixelOffset,
        ref bool validLocation)
      {
        if (!validLocation)
          return;
        Func<IntVector2, bool> func = (Func<IntVector2, bool>) (pixel =>
        {
          Vector2 unitMidpoint = PhysicsEngine.PixelToUnitMidpoint(pixel);
          if (!GameManager.Instance.Dungeon.CellSupportsFalling((Vector3) unitMidpoint))
            return false;
          List<SpeculativeRigidbody> platformsAt = GameManager.Instance.Dungeon.GetPlatformsAt((Vector3) unitMidpoint);
          if (platformsAt != null)
          {
            for (int index = 0; index < platformsAt.Count; ++index)
            {
              if (platformsAt[index].PrimaryPixelCollider.ContainsPixel(pixel))
                return false;
            }
          }
          return true;
        });
        PixelCollider primaryPixelCollider = specRigidbody.PrimaryPixelCollider;
        if (primaryPixelCollider != null)
        {
          IntVector2 intVector2 = pixelOffset - prevPixelOffset;
          if (intVector2 == IntVector2.Down && func(primaryPixelCollider.LowerLeft + pixelOffset) && func(primaryPixelCollider.LowerRight + pixelOffset) && (!func(primaryPixelCollider.UpperRight + prevPixelOffset) || !func(primaryPixelCollider.UpperLeft + prevPixelOffset)))
            validLocation = false;
          else if (intVector2 == IntVector2.Right && func(primaryPixelCollider.LowerRight + pixelOffset) && func(primaryPixelCollider.UpperRight + pixelOffset) && (!func(primaryPixelCollider.UpperLeft + prevPixelOffset) || !func(primaryPixelCollider.LowerLeft + prevPixelOffset)))
            validLocation = false;
          else if (intVector2 == IntVector2.Up && func(primaryPixelCollider.UpperRight + pixelOffset) && func(primaryPixelCollider.UpperLeft + pixelOffset) && (!func(primaryPixelCollider.LowerLeft + prevPixelOffset) || !func(primaryPixelCollider.LowerRight + prevPixelOffset)))
            validLocation = false;
          else if (intVector2 == IntVector2.Left && func(primaryPixelCollider.UpperLeft + pixelOffset) && func(primaryPixelCollider.LowerLeft + pixelOffset) && (!func(primaryPixelCollider.LowerRight + prevPixelOffset) || !func(primaryPixelCollider.UpperRight + prevPixelOffset)))
            validLocation = false;
        }
        if (validLocation)
          return;
        this.ForceBounce((pixelOffset - prevPixelOffset).ToVector2());
      }

      private void ForceBounce(Vector2 normal)
      {
        BounceProjModifier component = this.GetComponent<BounceProjModifier>();
        float angle1 = (-this.specRigidbody.Velocity).ToAngle();
        float angle2 = normal.ToAngle();
        float angle3 = BraveMathCollege.ClampAngle360(angle1 + (float) (2.0 * ((double) angle2 - (double) angle1)));
        if (this.shouldRotate)
          this.transform.Rotate(0.0f, 0.0f, angle3 - angle1);
        this.m_currentDirection = BraveMathCollege.DegreesToVector(angle3);
        this.m_currentSpeed *= 1f - component.percentVelocityToLoseOnBounce;
        if ((bool) (UnityEngine.Object) this.braveBulletScript && this.braveBulletScript.bullet != null)
        {
          this.braveBulletScript.bullet.Direction = angle3;
          this.braveBulletScript.bullet.Speed *= 1f - component.percentVelocityToLoseOnBounce;
        }
        if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
          return;
        component.Bounce((Projectile) this, this.specRigidbody.UnitCenter);
      }

      private void MaybeEatEnemy(Projectile selfProjectile, SpeculativeRigidbody enemyRigidbody)
      {
        if (!(bool) (UnityEngine.Object) enemyRigidbody)
          return;
        AIActor aiActor = enemyRigidbody.aiActor;
        if (!(bool) (UnityEngine.Object) aiActor || !(bool) (UnityEngine.Object) aiActor.healthHaver || aiActor.healthHaver.IsBoss)
          return;
        this.m_cachedTargetToEat = aiActor;
        aiActor.healthHaver.ManualDeathHandling = true;
        aiActor.healthHaver.OnPreDeath += new Action<Vector2>(this.EatEnemy);
      }

      private void EatEnemy(Vector2 dirVec)
      {
        if ((bool) (UnityEngine.Object) this.m_cachedTargetToEat)
        {
          this.m_cachedTargetToEat.ForceDeath(Vector2.zero, false);
          this.m_cachedTargetToEat.StartCoroutine(this.HandleEat(this.m_cachedTargetToEat));
        }
        this.m_cachedTargetToEat = (AIActor) null;
      }

      [DebuggerHidden]
      private IEnumerator HandleEat(AIActor targetEat)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new SharkProjectile.<HandleEat>c__Iterator2()
        {
          targetEat = targetEat
        };
      }

      [DebuggerHidden]
      private IEnumerator FindTarget()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new SharkProjectile.<FindTarget>c__Iterator3()
        {
          _this = this
        };
      }

      protected override void Move()
      {
        if (!this.m_coroutineIsActive)
          this.StartCoroutine(this.FindTarget());
        float num = 1f;
        this.m_pathTimer -= this.LocalDeltaTime;
        if ((bool) (UnityEngine.Object) this.sprite && (double) this.sprite.HeightOffGround != -1.0)
        {
          this.sprite.HeightOffGround = -1f;
          this.sprite.UpdateZDepth();
        }
        if ((UnityEngine.Object) this.CurrentTarget != (UnityEngine.Object) null)
        {
          if ((double) this.m_pathTimer <= 0.0 || this.m_currentPath == null)
          {
            CellTypes passableCellTypes = CellTypes.FLOOR;
            if (this.CanCrossPits)
              passableCellTypes |= CellTypes.PIT;
            Pathfinder.Instance.GetPath(this.specRigidbody.UnitCenter.ToIntVector2(VectorConversions.Floor), this.CurrentTarget.specRigidbody.UnitCenter.ToIntVector2(VectorConversions.Floor), out this.m_currentPath, new IntVector2?(IntVector2.One), passableCellTypes);
            this.m_pathTimer = 0.5f;
            if (this.m_currentPath != null && this.m_currentPath.Count > 0 && this.m_currentPath.WillReachFinalGoal)
              this.m_currentPath.Smooth(this.specRigidbody.UnitCenter, new Vector2(0.25f, 0.25f), passableCellTypes, false, IntVector2.One);
          }
          Vector2 vector2 = Vector2.zero;
          if (this.m_currentPath != null && this.m_currentPath.WillReachFinalGoal && this.m_currentPath.Count > 0)
            vector2 = this.GetPathVelocityContribution();
          else
            this.CurrentTarget = (GameActor) null;
          this.m_noTargetElapsed = 0.0f;
          this.m_currentDirection = Vector3.RotateTowards((Vector3) this.m_currentDirection, (Vector3) vector2, this.angularAcceleration * ((float) Math.PI / 180f) * BraveTime.DeltaTime, 0.0f).XY().normalized;
          num = (float) (0.25 + (1.0 - (double) Mathf.Clamp01(Mathf.Abs(Vector2.Angle(this.m_currentDirection, vector2)) / 60f)) * 0.75);
        }
        else
          this.m_noTargetElapsed += BraveTime.DeltaTime;
        if ((double) this.m_noTargetElapsed > 5.0)
          this.DieInAir(true);
        this.specRigidbody.Velocity = this.m_currentDirection * this.m_currentSpeed * num;
        DirectionalAnimation.Info info = this.animData.GetInfo(this.specRigidbody.Velocity);
        if (!this.sprite.spriteAnimator.IsPlaying(info.name))
          this.sprite.spriteAnimator.Play(info.name);
        if (this.m_lastGoopPosition.HasValue)
          DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(this.waterGoop).AddGoopLine(this.m_lastGoopPosition.Value, this.specRigidbody.UnitCenter, this.goopRadius);
        this.m_lastGoopPosition = new Vector2?(this.specRigidbody.UnitCenter);
        this.LastVelocity = this.specRigidbody.Velocity;
      }

      private bool GetNextTargetPosition(out Vector2 targetPos)
      {
        if (this.m_currentPath != null && this.m_currentPath.Count > 0)
        {
          targetPos = this.m_currentPath.GetFirstCenterVector2();
          return true;
        }
        if (this.m_overridePathEnd.HasValue)
        {
          targetPos = this.m_overridePathEnd.Value;
          return true;
        }
        targetPos = Vector2.zero;
        return false;
      }

      private Vector2 GetPathTarget()
      {
        Vector2 unitCenter = this.specRigidbody.UnitCenter;
        Vector2 pathTarget = unitCenter;
        float num1 = this.baseData.speed * this.LocalDeltaTime;
        Vector2 vector2 = unitCenter;
        Vector2 targetPos = unitCenter;
        while ((double) num1 > 0.0 && this.GetNextTargetPosition(out targetPos))
        {
          float num2 = Vector2.Distance(targetPos, unitCenter);
          if ((double) num2 < (double) num1)
          {
            num1 -= num2;
            vector2 = targetPos;
            pathTarget = vector2;
            if (this.m_currentPath != null && this.m_currentPath.Count > 0)
              this.m_currentPath.RemoveFirst();
            else
              this.m_overridePathEnd = new Vector2?();
          }
          else
          {
            pathTarget = (targetPos - vector2).normalized * num1 + vector2;
            break;
          }
        }
        return pathTarget;
      }

      private Vector2 GetPathVelocityContribution()
      {
        if ((this.m_currentPath == null || this.m_currentPath.Count == 0) && !this.m_overridePathEnd.HasValue)
          return Vector2.zero;
        Vector2 unitCenter = this.specRigidbody.UnitCenter;
        Vector2 vector2 = this.GetPathTarget() - unitCenter;
        return (double) this.baseData.speed * (double) this.LocalDeltaTime > (double) vector2.magnitude ? vector2 / this.LocalDeltaTime : this.baseData.speed * vector2.normalized;
      }

      protected override void OnDestroy() => base.OnDestroy();

      public static GameObject SpawnVFXBehind(
        GameObject prefab,
        Vector3 position,
        Quaternion rotation,
        bool ignoresPools)
      {
        GameObject gameObject = SpawnManager.SpawnVFX(prefab, position, rotation, ignoresPools);
        tk2dBaseSprite component = gameObject.GetComponent<tk2dBaseSprite>();
        component.scale = SharkProjectile.m_lastAssignedScale;
        component.depthUsesTrimmedBounds = true;
        component.HeightOffGround = -3f;
        component.UpdateZDepth();
        return gameObject;
      }

      protected override void HandleHitEffectsEnemy(
        SpeculativeRigidbody rigidbody,
        CollisionData lcr,
        bool playProjectileDeathVfx)
      {
        if (this.hitEffects.alwaysUseMidair)
        {
          this.HandleHitEffectsMidair();
        }
        else
        {
          if ((UnityEngine.Object) rigidbody.gameActor == (UnityEngine.Object) null)
            return;
          Vector3 vector3_1 = rigidbody.UnitBottomCenter.ToVector3ZUp() - (Vector3) this.hitEffects.enemy.effects[0].effects[0].effect.GetComponent<tk2dSprite>().GetRelativePositionFromAnchor(tk2dBaseSprite.Anchor.MiddleCenter);
          float zRotation1 = 0.0f;
          if ((bool) (UnityEngine.Object) this.sprite)
            SharkProjectile.m_lastAssignedScale = this.sprite.scale;
          if ((bool) (UnityEngine.Object) rigidbody.healthHaver && (double) rigidbody.healthHaver.GetCurrentHealth() <= 0.0)
          {
            if ((double) lcr.Contact.x > (double) rigidbody.UnitCenter.x)
            {
              Vector3 vector3_2 = vector3_1 + (new Vector3(1.125f, 0.5f, 0.0f) - Vector3.Scale(new Vector3(2.25f, 1f, 0.0f), SharkProjectile.m_lastAssignedScale - Vector3.one));
              VFXComplex effect = this.hitEffects.enemy.effects[0];
              Vector3 position = vector3_2;
              double zRotation2 = (double) zRotation1;
              Vector2? sourceNormal = new Vector2?(lcr.Normal);
              Vector2? sourceVelocity = new Vector2?(this.specRigidbody.Velocity);
              float? heightOffGround = new float?(-3f);
              // ISSUE: reference to a compiler-generated field
              if (SharkProjectile._f__mg_cache0 == null)
              {
                // ISSUE: reference to a compiler-generated field
                SharkProjectile._f__mg_cache0 = new VFXComplex.SpawnMethod(SharkProjectile.SpawnVFXBehind);
              }
              // ISSUE: reference to a compiler-generated field
              VFXComplex.SpawnMethod fMgCache0 = SharkProjectile._f__mg_cache0;
              effect.SpawnAtPosition(position, (float) zRotation2, sourceNormal: sourceNormal, sourceVelocity: sourceVelocity, heightOffGround: heightOffGround, spawnMethod: fMgCache0);
            }
            else
            {
              Vector3 vector3_3 = vector3_1 + (new Vector3(1.125f, 0.5f, 0.0f) - Vector3.Scale(new Vector3(2.25f, 1f, 0.0f), SharkProjectile.m_lastAssignedScale - Vector3.one));
              VFXComplex effect = this.hitEffects.enemy.effects[1];
              Vector3 position = vector3_3;
              double zRotation3 = (double) zRotation1;
              Vector2? sourceNormal = new Vector2?(lcr.Normal);
              Vector2? sourceVelocity = new Vector2?(this.specRigidbody.Velocity);
              float? heightOffGround = new float?(-3f);
              // ISSUE: reference to a compiler-generated field
              if (SharkProjectile._f__mg_cache1 == null)
              {
                // ISSUE: reference to a compiler-generated field
                SharkProjectile._f__mg_cache1 = new VFXComplex.SpawnMethod(SharkProjectile.SpawnVFXBehind);
              }
              // ISSUE: reference to a compiler-generated field
              VFXComplex.SpawnMethod fMgCache1 = SharkProjectile._f__mg_cache1;
              effect.SpawnAtPosition(position, (float) zRotation3, sourceNormal: sourceNormal, sourceVelocity: sourceVelocity, heightOffGround: heightOffGround, spawnMethod: fMgCache1);
            }
            if (!((UnityEngine.Object) this.ParticlesPrefab != (UnityEngine.Object) null))
              return;
            SpawnManager.SpawnParticleSystem(this.ParticlesPrefab.gameObject, rigidbody.UnitBottomCenter.ToVector3ZUp(rigidbody.UnitBottomCenter.y), Quaternion.identity).GetComponent<ParticleSystem>().Play();
          }
          else
            this.enemyNotKilledPool.SpawnAtPosition(rigidbody.UnitCenter.ToVector3ZUp(), zRotation1, sourceNormal: new Vector2?(lcr.Normal), sourceVelocity: new Vector2?(this.specRigidbody.Velocity));
        }
      }
    }

}
