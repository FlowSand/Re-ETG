// Decompiled with JetBrains decompiler
// Type: TigerProjectile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using Pathfinding;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class TigerProjectile : Projectile
  {
    public DirectionalAnimation animData;
    public float angularAcceleration = 10f;
    [NonSerialized]
    protected GameActor CurrentTarget;
    [NonSerialized]
    protected bool m_coroutineIsActive;
    private AIActor m_cachedTargetToEat;
    private float m_moveElapsed;
    private float m_pathTimer;
    private Path m_currentPath;
    private static Vector3 m_lastAssignedScale = Vector3.one;
    private float m_noTargetElapsed;

    public override void Start()
    {
      base.Start();
      this.hitEffects.HasProjectileDeathVFX = false;
      this.specRigidbody.CollideWithTileMap = false;
      TigerProjectile tigerProjectile1 = this;
      tigerProjectile1.OnHitEnemy = tigerProjectile1.OnHitEnemy + new Action<Projectile, SpeculativeRigidbody, bool>(this.HandleHitEnemy);
      TigerProjectile tigerProjectile2 = this;
      tigerProjectile2.OnWillKillEnemy = tigerProjectile2.OnWillKillEnemy + new Action<Projectile, SpeculativeRigidbody>(this.MaybeEatEnemy);
    }

    private void HandleHitEnemy(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
    {
      this.StartCoroutine(this.FrameDelayedDestruction());
    }

    [DebuggerHidden]
    private IEnumerator FrameDelayedDestruction()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new TigerProjectile__FrameDelayedDestructionc__Iterator0()
      {
        _this = this
      };
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
      if (!(bool) (UnityEngine.Object) this.m_cachedTargetToEat || (bool) (UnityEngine.Object) this.m_cachedTargetToEat.healthHaver && this.m_cachedTargetToEat.healthHaver.IsBoss)
        return;
      this.m_cachedTargetToEat.ForceDeath(Vector2.zero, false);
      this.m_cachedTargetToEat.StartCoroutine(this.HandleEat(this.m_cachedTargetToEat));
      this.m_cachedTargetToEat = (AIActor) null;
    }

    [DebuggerHidden]
    private IEnumerator HandleEat(AIActor targetEat)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new TigerProjectile__HandleEatc__Iterator1()
      {
        targetEat = targetEat
      };
    }

    [DebuggerHidden]
    private IEnumerator FindTarget()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new TigerProjectile__FindTargetc__Iterator2()
      {
        _this = this
      };
    }

    protected override void Move()
    {
      if (!this.m_coroutineIsActive)
        this.StartCoroutine(this.FindTarget());
      this.m_moveElapsed += BraveTime.DeltaTime;
      this.m_pathTimer -= BraveTime.DeltaTime;
      if (!this.specRigidbody.CollideWithTileMap && (double) this.m_moveElapsed > 0.5)
      {
        this.m_moveElapsed = 0.0f;
        this.specRigidbody.CollideWithTileMap = true;
        if (PhysicsEngine.Instance.OverlapCast(this.specRigidbody, collideWithRigidbodies: false))
          this.specRigidbody.CollideWithTileMap = false;
      }
      float num = 1f;
      if ((UnityEngine.Object) this.CurrentTarget != (UnityEngine.Object) null)
      {
        this.m_noTargetElapsed = 0.0f;
        if (this.m_currentPath == null || (double) this.m_pathTimer <= 0.0)
        {
          bool path = Pathfinder.Instance.GetPath(this.specRigidbody.Position.UnitPosition.ToIntVector2(VectorConversions.Floor), this.CurrentTarget.specRigidbody.UnitCenter.ToIntVector2(VectorConversions.Ceil), out this.m_currentPath, new IntVector2?(this.specRigidbody.UnitDimensions.ToIntVector2(VectorConversions.Ceil)), CellTypes.FLOOR | CellTypes.PIT);
          this.m_pathTimer = 0.25f;
          if (!path)
            this.m_currentPath = (Path) null;
          else
            this.m_currentPath.Smooth(this.specRigidbody.Position.UnitPosition, this.specRigidbody.UnitDimensions / 2f, CellTypes.FLOOR | CellTypes.PIT, true, this.specRigidbody.UnitDimensions.ToIntVector2(VectorConversions.Ceil));
        }
        if (this.m_currentPath != null && this.m_currentPath.Positions.Count > 2)
          this.m_currentDirection = Vector3.RotateTowards((Vector3) this.m_currentDirection, (Vector3) (this.m_currentPath.GetSecondCenterVector2() - this.specRigidbody.UnitCenter).normalized, this.angularAcceleration * ((float) Math.PI / 180f) * BraveTime.DeltaTime, 0.0f).XY().normalized;
        else
          this.m_currentDirection = Vector3.RotateTowards((Vector3) this.m_currentDirection, (Vector3) (this.CurrentTarget.specRigidbody.UnitCenter - this.specRigidbody.UnitCenter).normalized, this.angularAcceleration * ((float) Math.PI / 180f) * BraveTime.DeltaTime, 0.0f).XY().normalized;
      }
      else
        this.m_noTargetElapsed += BraveTime.DeltaTime;
      if ((double) this.m_noTargetElapsed > 3.0)
        this.DieInAir(true);
      this.specRigidbody.Velocity = this.m_currentDirection * this.baseData.speed * num;
      DirectionalAnimation.Info info = this.animData.GetInfo(this.specRigidbody.Velocity);
      if (!this.sprite.spriteAnimator.IsPlaying(info.name))
        this.sprite.spriteAnimator.Play(info.name);
      this.LastVelocity = this.specRigidbody.Velocity;
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
      component.scale = TigerProjectile.m_lastAssignedScale;
      component.UpdateZDepth();
      return gameObject;
    }

    protected override void HandleHitEffectsEnemy(
      SpeculativeRigidbody rigidbody,
      CollisionData lcr,
      bool playProjectileDeathVfx)
    {
      if (!(bool) (UnityEngine.Object) rigidbody || !(bool) (UnityEngine.Object) rigidbody.gameActor)
        return;
      Vector3 vector3_1 = rigidbody.UnitBottomCenter.ToVector3ZUp() - (Vector3) this.hitEffects.deathEnemy.effects[0].effects[0].effect.GetComponent<tk2dSprite>().GetRelativePositionFromAnchor(tk2dBaseSprite.Anchor.MiddleCenter);
      float num = 0.0f;
      if ((bool) (UnityEngine.Object) this.sprite)
        TigerProjectile.m_lastAssignedScale = this.sprite.scale;
      if ((double) lcr.Contact.x > (double) rigidbody.UnitCenter.x)
      {
        Vector3 vector3_2 = vector3_1 + (new Vector3(1f, 2f, 0.0f) - Vector3.Scale(new Vector3(2f, 4f, 0.0f), TigerProjectile.m_lastAssignedScale - Vector3.one));
        VFXComplex effect = this.hitEffects.deathEnemy.effects[0];
        Vector3 position = vector3_2;
        double zRotation = (double) num;
        Vector2? sourceNormal = new Vector2?(lcr.Normal);
        Vector2? sourceVelocity = new Vector2?(this.specRigidbody.Velocity);
        float? heightOffGround = new float?(-3f);
        // ISSUE: reference to a compiler-generated field
        if (TigerProjectile._f__mg_cache0 == null)
        {
          // ISSUE: reference to a compiler-generated field
          TigerProjectile._f__mg_cache0 = new VFXComplex.SpawnMethod(TigerProjectile.SpawnVFXBehind);
        }
        // ISSUE: reference to a compiler-generated field
        VFXComplex.SpawnMethod fMgCache0 = TigerProjectile._f__mg_cache0;
        effect.SpawnAtPosition(position, (float) zRotation, sourceNormal: sourceNormal, sourceVelocity: sourceVelocity, heightOffGround: heightOffGround, spawnMethod: fMgCache0);
      }
      else
      {
        Vector3 vector3_3 = vector3_1 + (new Vector3(-1f, 2f, 0.0f) - Vector3.Scale(new Vector3(2f, 4f, 0.0f), TigerProjectile.m_lastAssignedScale - Vector3.one));
        VFXComplex effect = this.hitEffects.deathEnemy.effects[1];
        Vector3 position = vector3_3;
        double zRotation = (double) num;
        Vector2? sourceNormal = new Vector2?(lcr.Normal);
        Vector2? sourceVelocity = new Vector2?(this.specRigidbody.Velocity);
        float? heightOffGround = new float?(-3f);
        // ISSUE: reference to a compiler-generated field
        if (TigerProjectile._f__mg_cache1 == null)
        {
          // ISSUE: reference to a compiler-generated field
          TigerProjectile._f__mg_cache1 = new VFXComplex.SpawnMethod(TigerProjectile.SpawnVFXBehind);
        }
        // ISSUE: reference to a compiler-generated field
        VFXComplex.SpawnMethod fMgCache1 = TigerProjectile._f__mg_cache1;
        effect.SpawnAtPosition(position, (float) zRotation, sourceNormal: sourceNormal, sourceVelocity: sourceVelocity, heightOffGround: heightOffGround, spawnMethod: fMgCache1);
      }
    }
  }

