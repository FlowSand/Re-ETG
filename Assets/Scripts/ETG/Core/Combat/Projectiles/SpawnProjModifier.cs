// Decompiled with JetBrains decompiler
// Type: SpawnProjModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using UnityEngine;

#nullable disable

public class SpawnProjModifier : MonoBehaviour
  {
    public bool PostprocessSpawnedProjectiles;
    [Header("Spawn in Flight")]
    public bool spawnProjectilesInFlight;
    public Projectile projectileToSpawnInFlight;
    public float inFlightSpawnCooldown = 1f;
    public float inFlightSpawnAngle = 90f;
    public Transform InFlightSourceTransform;
    public bool usesComplexSpawnInFlight;
    [ShowInInspectorIf("usesComplexSpawnInFlight", false)]
    public int numToSpawnInFlight = 2;
    [ShowInInspectorIf("usesComplexSpawnInFlight", false)]
    public bool fireRandomlyInAngle;
    [ShowInInspectorIf("usesComplexSpawnInFlight", false)]
    public bool inFlightAimAtEnemies;
    public string inFlightSpawnAnimation;
    [Header("Spawn on Collision")]
    public bool spawnProjectilesOnCollision;
    public SpawnProjModifier.CollisionSpawnStyle collisionSpawnStyle;
    public bool doOverrideObjectCollisionSpawnStyle;
    public SpawnProjModifier.CollisionSpawnStyle overrideObjectSpawnStyle;
    public bool spawnCollisionProjectilesOnBounce;
    public Projectile projectileToSpawnOnCollision;
    public bool UsesMultipleCollisionSpawnProjectiles;
    public Projectile[] collisionSpawnProjectiles;
    public int numberToSpawnOnCollison = 2;
    public int startAngle = 90;
    public bool randomRadialStartAngle;
    public bool spawnOnObjectCollisions = true;
    public bool alignToSurfaceNormal;
    public bool spawnProjecitlesOnDieInAir;
    public bool SpawnedProjectilesInheritData;
    [NonSerialized]
    public bool SpawnedProjectilesInheritAppearance;
    [NonSerialized]
    public float SpawnedProjectileScaleModifier = 1f;
    [Header("Audio")]
    public string spawnAudioEvent = string.Empty;
    private SpeculativeRigidbody m_srb;
    private Projectile p;
    private float elapsed;
    protected bool m_hasCheckedProjectile;
    protected Projectile m_projectile;

    private Vector2 SpawnPos
    {
      get
      {
        if ((bool) (UnityEngine.Object) this.m_srb)
          return this.m_srb.UnitCenter;
        if ((bool) (UnityEngine.Object) this.transform)
          return this.transform.position.XY();
        return (bool) (UnityEngine.Object) this.m_projectile ? (Vector2) this.m_projectile.LastPosition : GameManager.Instance.BestActivePlayer.CenterPosition;
      }
    }

    private void Update()
    {
      if ((UnityEngine.Object) this.p == (UnityEngine.Object) null)
        this.p = this.GetComponent<Projectile>();
      if ((UnityEngine.Object) this.m_srb == (UnityEngine.Object) null)
        this.m_srb = this.GetComponent<SpeculativeRigidbody>();
      if (!this.spawnProjectilesInFlight)
        return;
      this.elapsed += BraveTime.DeltaTime;
      if ((double) this.elapsed <= (double) this.inFlightSpawnCooldown)
        return;
      if (this.usesComplexSpawnInFlight)
      {
        this.elapsed -= this.inFlightSpawnCooldown;
        if (this.inFlightAimAtEnemies)
        {
          AIActor aiActor = (AIActor) null;
          RoomHandler roomFromPosition = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(this.transform.position.IntXY());
          for (int index = 0; index < this.numToSpawnInFlight; ++index)
          {
            AIActor randomActiveEnemy = roomFromPosition.GetRandomActiveEnemy(false);
            if ((UnityEngine.Object) randomActiveEnemy != (UnityEngine.Object) null && (UnityEngine.Object) randomActiveEnemy != (UnityEngine.Object) aiActor)
            {
              aiActor = randomActiveEnemy;
              this.SpawnProjectile(this.projectileToSpawnInFlight, this.SpawnPos.ToVector3ZUp(), BraveMathCollege.Atan2Degrees(randomActiveEnemy.CenterPosition - this.SpawnPos));
            }
            else
              break;
          }
        }
        else
        {
          for (int index = 0; index < this.numToSpawnInFlight; ++index)
          {
            float num = (float) ((double) this.inFlightSpawnAngle / (double) (this.numToSpawnInFlight - 1) * (double) index - (double) this.inFlightSpawnAngle / 2.0);
            if (this.fireRandomlyInAngle)
              num = (float) ((double) UnityEngine.Random.value * (double) this.inFlightSpawnAngle - (double) this.inFlightSpawnAngle / 2.0);
            this.SpawnProjectile(this.projectileToSpawnInFlight, this.SpawnPos.ToVector3ZUp(), this.p.transform.eulerAngles.z + num);
          }
        }
      }
      else if ((bool) (UnityEngine.Object) this.InFlightSourceTransform)
      {
        this.elapsed -= this.inFlightSpawnCooldown;
        this.SpawnProjectile(this.projectileToSpawnInFlight, this.InFlightSourceTransform.position, this.InFlightSourceTransform.eulerAngles.z);
      }
      else
      {
        this.elapsed -= this.inFlightSpawnCooldown;
        this.SpawnProjectile(this.projectileToSpawnInFlight, this.SpawnPos.ToVector3ZUp(), this.p.transform.eulerAngles.z + this.inFlightSpawnAngle);
        this.SpawnProjectile(this.projectileToSpawnInFlight, this.SpawnPos.ToVector3ZUp(), this.p.transform.eulerAngles.z - this.inFlightSpawnAngle);
      }
      if (!string.IsNullOrEmpty(this.inFlightSpawnAnimation))
        this.p.sprite.spriteAnimator.PlayForDuration(this.inFlightSpawnAnimation, -1f, this.p.sprite.spriteAnimator.CurrentClip.name, true);
      if (string.IsNullOrEmpty(this.spawnAudioEvent))
        return;
      int num1 = (int) AkSoundEngine.PostEvent(this.spawnAudioEvent, this.gameObject);
    }

    public void SpawnCollisionProjectiles(
      Vector2 contact,
      Vector2 normal,
      SpeculativeRigidbody collidedRigidbody,
      bool hitObject = false)
    {
      if (!(bool) (UnityEngine.Object) this || !(bool) (UnityEngine.Object) this.m_srb)
        return;
      SpawnProjModifier.CollisionSpawnStyle collisionSpawnStyle = this.collisionSpawnStyle;
      if (hitObject && this.doOverrideObjectCollisionSpawnStyle)
        collisionSpawnStyle = this.overrideObjectSpawnStyle;
      switch (collisionSpawnStyle)
      {
        case SpawnProjModifier.CollisionSpawnStyle.RADIAL:
          this.HandleSpawnRadial(contact, normal, collidedRigidbody);
          break;
        case SpawnProjModifier.CollisionSpawnStyle.FLAK_BURST:
          this.HandleSpawnFlakBurst(contact, normal, collidedRigidbody);
          break;
        case SpawnProjModifier.CollisionSpawnStyle.REVERSE_FLAK_BURST:
          this.HandleReverseSpawnFlakBurst(contact, normal, collidedRigidbody);
          break;
      }
      if (string.IsNullOrEmpty(this.spawnAudioEvent))
        return;
      int num = (int) AkSoundEngine.PostEvent(this.spawnAudioEvent, this.gameObject);
    }

    private void HandleReverseSpawnFlakBurst(
      Vector2 contact,
      Vector2 normal,
      SpeculativeRigidbody collidedRigidbody)
    {
      int num1 = UnityEngine.Random.Range(0, 20);
      Vector2 unitBottomLeft = this.m_srb.UnitBottomLeft;
      Vector2 unitTopRight = this.m_srb.UnitTopRight;
      for (int index = 0; index < this.numberToSpawnOnCollison; ++index)
      {
        Projectile proj = !this.UsesMultipleCollisionSpawnProjectiles ? this.projectileToSpawnOnCollision : this.collisionSpawnProjectiles[UnityEngine.Random.Range(0, this.collisionSpawnProjectiles.Length)];
        float num2 = (float) (15.0 - (double) BraveMathCollege.GetLowDiscrepancyRandom(index + num1) * 30.0);
        float num3 = BraveMathCollege.Atan2Degrees(normal) + num2;
        if (this.alignToSurfaceNormal)
          num3 = BraveMathCollege.Atan2Degrees(-1f * normal) + num2;
        Vector2 vector = new Vector2(UnityEngine.Random.Range(unitBottomLeft.x, unitTopRight.x), UnityEngine.Random.Range(unitBottomLeft.y, unitTopRight.y));
        this.SpawnProjectile(proj, vector.ToVector3ZUp(this.transform.position.z), 180f + num3, collidedRigidbody);
      }
    }

    private void HandleSpawnFlakBurst(
      Vector2 contact,
      Vector2 normal,
      SpeculativeRigidbody collidedRigidbody)
    {
      int num1 = UnityEngine.Random.Range(0, 20);
      Vector2 unitBottomLeft = this.m_srb.UnitBottomLeft;
      Vector2 unitTopRight = this.m_srb.UnitTopRight;
      for (int index = 0; index < this.numberToSpawnOnCollison; ++index)
      {
        Projectile proj = !this.UsesMultipleCollisionSpawnProjectiles ? this.projectileToSpawnOnCollision : this.collisionSpawnProjectiles[UnityEngine.Random.Range(0, this.collisionSpawnProjectiles.Length)];
        float num2 = (float) (15.0 - (double) BraveMathCollege.GetLowDiscrepancyRandom(index + num1) * 30.0);
        float zRotation = BraveMathCollege.Atan2Degrees(normal) + num2;
        Vector2 vector = new Vector2(UnityEngine.Random.Range(unitBottomLeft.x, unitTopRight.x), UnityEngine.Random.Range(unitBottomLeft.y, unitTopRight.y));
        this.SpawnProjectile(proj, vector.ToVector3ZUp(this.transform.position.z), zRotation, collidedRigidbody);
      }
    }

    private void HandleSpawnRadial(
      Vector2 contact,
      Vector2 normal,
      SpeculativeRigidbody collidedRigidbody)
    {
      float num1 = 360f / (float) this.numberToSpawnOnCollison;
      for (int index = 0; index < this.numberToSpawnOnCollison; ++index)
      {
        Projectile proj = !this.UsesMultipleCollisionSpawnProjectiles ? this.projectileToSpawnOnCollision : this.collisionSpawnProjectiles[UnityEngine.Random.Range(0, this.collisionSpawnProjectiles.Length)];
        float num2 = 0.5f;
        if (this.randomRadialStartAngle)
          num2 = (float) UnityEngine.Random.Range(0, 360);
        float zRotation = !this.alignToSurfaceNormal ? (float) ((double) this.p.transform.eulerAngles.z + (double) num2 + (double) this.startAngle + (double) num1 * (double) index) : (float) ((double) Mathf.Atan2(normal.y, normal.x) * 57.295780181884766 + (double) num2 + (double) this.startAngle + (double) num1 * (double) index);
        this.SpawnProjectile(proj, (contact + normal * 0.5f).ToVector3ZUp(this.transform.position.z), zRotation, collidedRigidbody);
      }
    }

    private void SpawnProjectile(
      Projectile proj,
      Vector3 spawnPosition,
      float zRotation,
      SpeculativeRigidbody collidedRigidbody = null)
    {
      Projectile component = SpawnManager.SpawnProjectile(proj.gameObject, spawnPosition, Quaternion.Euler(0.0f, 0.0f, zRotation)).GetComponent<Projectile>();
      if ((bool) (UnityEngine.Object) component)
      {
        component.SpawnedFromOtherPlayerProjectile = true;
        if (component is HelixProjectile)
          component.Inverted = (double) UnityEngine.Random.value < 0.5;
      }
      if (!this.m_hasCheckedProjectile)
      {
        this.m_hasCheckedProjectile = true;
        this.m_projectile = this.GetComponent<Projectile>();
      }
      if ((bool) (UnityEngine.Object) this.m_projectile && this.PostprocessSpawnedProjectiles && (bool) (UnityEngine.Object) this.m_projectile.Owner && this.m_projectile.Owner is PlayerController)
        (this.m_projectile.Owner as PlayerController).DoPostProcessProjectile(component);
      if (this.SpawnedProjectilesInheritAppearance && (bool) (UnityEngine.Object) component.sprite && (bool) (UnityEngine.Object) this.m_projectile.sprite)
      {
        component.shouldRotate = this.m_projectile.shouldRotate;
        component.shouldFlipHorizontally = this.m_projectile.shouldFlipHorizontally;
        component.shouldFlipVertically = this.m_projectile.shouldFlipVertically;
        component.sprite.SetSprite(this.m_projectile.sprite.Collection, this.m_projectile.sprite.spriteId);
        Vector2 vector = component.transform.position.XY() - component.sprite.WorldCenter;
        component.transform.position += vector.ToVector3ZUp();
        component.specRigidbody.Reinitialize();
      }
      if ((double) this.SpawnedProjectileScaleModifier != 1.0)
        component.AdditionalScaleMultiplier *= this.SpawnedProjectileScaleModifier;
      if ((bool) (UnityEngine.Object) this.m_projectile && (double) this.m_projectile.GetCachedBaseDamage > 0.0)
        component.baseData.damage *= Mathf.Min(this.m_projectile.baseData.damage / this.m_projectile.GetCachedBaseDamage, 1f);
      if ((bool) (UnityEngine.Object) this.p)
      {
        component.Owner = this.p.Owner;
        component.Shooter = this.p.Shooter;
        if (component is RobotechProjectile)
          (component as RobotechProjectile).initialOverrideTargetPoint = new Vector2?(spawnPosition.XY() + (Quaternion.Euler(0.0f, 0.0f, zRotation) * (Vector3) Vector2.right * 10f).XY());
        if (this.SpawnedProjectilesInheritData)
        {
          component.baseData.damage = Mathf.Max(component.baseData.damage, this.p.baseData.damage / (float) this.numberToSpawnOnCollison);
          component.baseData.speed = Mathf.Max(component.baseData.speed, this.p.baseData.speed / ((float) this.numberToSpawnOnCollison / 2f));
          component.baseData.force = Mathf.Max(component.baseData.force, this.p.baseData.force / (float) this.numberToSpawnOnCollison);
        }
      }
      if (!(bool) (UnityEngine.Object) component.specRigidbody)
        return;
      if ((bool) (UnityEngine.Object) collidedRigidbody)
        component.specRigidbody.RegisterTemporaryCollisionException(collidedRigidbody, 0.25f, new float?(0.5f));
      PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(component.specRigidbody);
    }

    public enum CollisionSpawnStyle
    {
      RADIAL,
      FLAK_BURST,
      REVERSE_FLAK_BURST,
    }
  }

