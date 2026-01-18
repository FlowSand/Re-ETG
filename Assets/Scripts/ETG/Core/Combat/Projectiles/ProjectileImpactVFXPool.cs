// Decompiled with JetBrains decompiler
// Type: ProjectileImpactVFXPool
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.Serialization;

#nullable disable

[Serializable]
public class ProjectileImpactVFXPool
  {
    public bool alwaysUseMidair;
    [HideInInspectorIf("alwaysUseMidair", false)]
    public VFXPool tileMapVertical;
    [HideInInspectorIf("alwaysUseMidair", false)]
    public VFXPool tileMapHorizontal;
    [HideInInspectorIf("alwaysUseMidair", false)]
    public VFXPool enemy;
    public bool suppressMidairDeathVfx;
    public GameObject overrideMidairDeathVFX;
    [ShowInInspectorIf("overrideMidairDeathVFX", false)]
    public bool midairInheritsRotation;
    [ShowInInspectorIf("overrideMidairDeathVFX", false)]
    public bool midairInheritsVelocity;
    [ShowInInspectorIf("overrideMidairDeathVFX", false)]
    public bool midairInheritsFlip;
    [ShowInInspectorIf("overrideMidairDeathVFX", false)]
    public int overrideMidairZHeight = -1;
    public GameObject overrideEarlyDeathVfx;
    public bool HasProjectileDeathVFX;
    [ShowInInspectorIf("HasProjectileDeathVFX", true)]
    public VFXPool deathTileMapVertical;
    [ShowInInspectorIf("HasProjectileDeathVFX", true)]
    public VFXPool deathTileMapHorizontal;
    [ShowInInspectorIf("HasProjectileDeathVFX", true)]
    public VFXPool deathEnemy;
    [ShowInInspectorIf("HasProjectileDeathVFX", true)]
    [FormerlySerializedAs("ProjectileDeathVFX")]
    public VFXPool deathAny;
    [ShowInInspectorIf("HasProjectileDeathVFX", true)]
    public bool CenterDeathVFXOnProjectile;
    [NonSerialized]
    public bool suppressHitEffectsIfOffscreen;

    public GameObject SpawnVFXEnemy(
      GameObject prefab,
      Vector3 position,
      Quaternion rotation,
      bool ignoresPools)
    {
      GameObject gameObject = SpawnManager.SpawnVFX(prefab, position, rotation, ignoresPools);
      if (gameObject.CompareTag("DefaultEnemyHitVFX"))
      {
        tk2dSpriteAnimator component = gameObject.GetComponent<tk2dSpriteAnimator>();
        component.deferNextStartClip = true;
        component.Play("Dust_Impact_Enemy");
      }
      return gameObject;
    }

    public void HandleProjectileDeathVFX(
      Vector3 position,
      float rotation,
      Transform enemyTransform,
      Vector2 sourceNormal,
      Vector2 sourceVelocity,
      bool isObject = false)
    {
      if (this.suppressHitEffectsIfOffscreen && !GameManager.Instance.MainCameraController.PointIsVisible((Vector2) position, 0.05f))
        return;
      if (isObject)
        this.deathAny.SpawnAtPosition(position, rotation, enemyTransform, new Vector2?(sourceNormal), new Vector2?(sourceVelocity), spawnMethod: new VFXComplex.SpawnMethod(this.SpawnVFXEnemy));
      else
        this.deathAny.SpawnAtPosition(position, rotation, enemyTransform, new Vector2?(sourceNormal), new Vector2?(sourceVelocity));
    }

    public void HandleEnemyImpact(
      Vector3 position,
      float rotation,
      Transform enemyTransform,
      Vector2 sourceNormal,
      Vector2 sourceVelocity,
      bool playProjectileDeathVfx,
      bool isObject = false)
    {
      if (this.suppressHitEffectsIfOffscreen && !GameManager.Instance.MainCameraController.PointIsVisible((Vector2) position, 0.05f))
        return;
      float? heightOffGround = new float?();
      if ((double) Projectile.CurrentProjectileDepth != 0.800000011920929)
        heightOffGround = new float?(Projectile.CurrentProjectileDepth);
      if (isObject)
      {
        this.enemy.SpawnAtPosition(position, rotation, enemyTransform, new Vector2?(sourceNormal), new Vector2?(sourceVelocity), heightOffGround, spawnMethod: new VFXComplex.SpawnMethod(this.SpawnVFXEnemy));
        if (!playProjectileDeathVfx || !this.HasProjectileDeathVFX)
          return;
        this.deathEnemy.SpawnAtPosition(position, rotation, enemyTransform, new Vector2?(sourceNormal), new Vector2?(sourceVelocity), heightOffGround, spawnMethod: new VFXComplex.SpawnMethod(this.SpawnVFXEnemy));
      }
      else
      {
        this.enemy.SpawnAtPosition(position, rotation, enemyTransform, new Vector2?(sourceNormal), new Vector2?(sourceVelocity), heightOffGround);
        if (!playProjectileDeathVfx || !this.HasProjectileDeathVFX)
          return;
        this.deathEnemy.SpawnAtPosition(position, rotation, enemyTransform, new Vector2?(sourceNormal), new Vector2?(sourceVelocity), heightOffGround, spawnMethod: new VFXComplex.SpawnMethod(this.SpawnVFXEnemy));
      }
    }

    public void HandleTileMapImpactVertical(
      Vector3 position,
      float heightOffGroundOffset,
      float rotation,
      Vector2 sourceNormal,
      Vector2 sourceVelocity,
      bool playProjectileDeathVfx,
      Transform parent = null,
      VFXComplex.SpawnMethod overrideSpawnMethod = null,
      VFXComplex.SpawnMethod overrideDeathSpawnMethod = null)
    {
      if (this.suppressHitEffectsIfOffscreen && !GameManager.Instance.MainCameraController.PointIsVisible((Vector2) position, 0.05f))
        return;
      float num1 = rotation + 90f;
      if (!this.HasTileMapVerticalEffects)
      {
        GameManager.Instance.Dungeon.roomMaterialDefinitions[GameManager.Instance.Dungeon.data.GetRoomVisualTypeAtPosition(position.XY())].SpawnRandomVertical(position, num1, parent, sourceNormal, sourceVelocity);
      }
      else
      {
        float yPositionAtGround1 = (float) Mathf.FloorToInt(position.y) - heightOffGroundOffset;
        this.tileMapVertical.SpawnAtPosition(position.x, yPositionAtGround1, position.y - yPositionAtGround1, num1, parent, new Vector2?(sourceNormal), new Vector2?(sourceVelocity), spawnMethod: overrideSpawnMethod);
        if (!playProjectileDeathVfx || !this.HasProjectileDeathVFX)
          return;
        VFXPool deathTileMapVertical = this.deathTileMapVertical;
        float x = position.x;
        float num2 = yPositionAtGround1;
        float num3 = position.y - yPositionAtGround1;
        float num4 = num1;
        Transform transform = parent;
        Vector2? nullable1 = new Vector2?(sourceNormal);
        Vector2? nullable2 = new Vector2?(sourceVelocity);
        VFXComplex.SpawnMethod spawnMethod1 = overrideDeathSpawnMethod;
        double xPosition = (double) x;
        double yPositionAtGround2 = (double) num2;
        double heightOffGround = (double) num3;
        double zRotation = (double) num4;
        Transform parent1 = transform;
        Vector2? sourceNormal1 = nullable1;
        Vector2? sourceVelocity1 = nullable2;
        VFXComplex.SpawnMethod spawnMethod2 = spawnMethod1;
        deathTileMapVertical.SpawnAtPosition((float) xPosition, (float) yPositionAtGround2, (float) heightOffGround, (float) zRotation, parent1, sourceNormal1, sourceVelocity1, spawnMethod: spawnMethod2);
      }
    }

    public void HandleTileMapImpactHorizontal(
      Vector3 position,
      float rotation,
      Vector2 sourceNormal,
      Vector2 sourceVelocity,
      bool playProjectileDeathVfx,
      Transform parent = null,
      VFXComplex.SpawnMethod overrideSpawnMethod = null,
      VFXComplex.SpawnMethod overrideDeathSpawnMethod = null)
    {
      if (this.suppressHitEffectsIfOffscreen && !GameManager.Instance.MainCameraController.PointIsVisible((Vector2) position, 0.05f))
        return;
      if (!this.HasTileMapHorizontalEffects)
      {
        GameManager.Instance.Dungeon.roomMaterialDefinitions[GameManager.Instance.Dungeon.data.GetRoomVisualTypeAtPosition(position.XY())].SpawnRandomHorizontal(position, rotation, parent, sourceNormal, sourceVelocity);
      }
      else
      {
        this.tileMapHorizontal.SpawnAtPosition(position, rotation, parent, new Vector2?(sourceNormal), new Vector2?(sourceVelocity), spawnMethod: overrideSpawnMethod);
        if (!playProjectileDeathVfx || !this.HasProjectileDeathVFX)
          return;
        VFXPool tileMapHorizontal = this.deathTileMapHorizontal;
        Vector3 vector3 = position;
        float num = rotation;
        Transform transform = parent;
        Vector2? nullable1 = new Vector2?(sourceNormal);
        Vector2? nullable2 = new Vector2?(sourceVelocity);
        VFXComplex.SpawnMethod spawnMethod1 = overrideDeathSpawnMethod;
        Vector3 position1 = vector3;
        double zRotation = (double) num;
        Transform parent1 = transform;
        Vector2? sourceNormal1 = nullable1;
        Vector2? sourceVelocity1 = nullable2;
        float? heightOffGround = new float?();
        VFXComplex.SpawnMethod spawnMethod2 = spawnMethod1;
        tileMapHorizontal.SpawnAtPosition(position1, (float) zRotation, parent1, sourceNormal1, sourceVelocity1, heightOffGround, spawnMethod: spawnMethod2);
      }
    }

    public bool HasTileMapVerticalEffects
    {
      get
      {
        if (this.tileMapVertical.type != VFXPoolType.None)
          return true;
        return this.HasProjectileDeathVFX && this.deathTileMapVertical.type != VFXPoolType.None;
      }
    }

    public bool HasTileMapHorizontalEffects
    {
      get
      {
        if (this.tileMapHorizontal.type != VFXPoolType.None)
          return true;
        return this.HasProjectileDeathVFX && this.deathTileMapHorizontal.type != VFXPoolType.None;
      }
    }
  }

