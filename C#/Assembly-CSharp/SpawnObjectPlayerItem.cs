// Decompiled with JetBrains decompiler
// Type: SpawnObjectPlayerItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using UnityEngine;

#nullable disable
public class SpawnObjectPlayerItem : PlayerItem
{
  [Header("Spawn Object Settings")]
  public GameObject objectToSpawn;
  [EnemyIdentifier]
  public string enemyGuidToSpawn;
  public bool HasOverrideSynergyItem;
  [LongNumericEnum]
  [ShowInInspectorIf("HasOverrideSynergyItem", false)]
  public CustomSynergyType RequiredSynergy;
  [ShowInInspectorIf("HasOverrideSynergyItem", false)]
  public GameObject SynergyObjectToSpawn;
  public float tossForce;
  public bool canBounce = true;
  public bool IsCigarettes;
  [NonSerialized]
  public GameObject spawnedPlayerObject;
  public bool PreventCooldownWhileExtant;
  public bool RequireEnemiesInRoom;
  public bool SpawnRadialCopies;
  [ShowInInspectorIf("SpawnRadialCopies", false)]
  public int RadialCopiesToSpawn = 1;
  public string AudioEvent;
  public bool IsKageBunshinItem;
  private float m_elapsedCooldownWhileExtantTimer;

  public override bool CanBeUsed(PlayerController user)
  {
    return (!this.IsCigarettes || !(bool) (UnityEngine.Object) user || !(bool) (UnityEngine.Object) user.healthHaver || user.healthHaver.IsVulnerable) && (!this.RequireEnemiesInRoom || !(bool) (UnityEngine.Object) user || user.CurrentRoom == null || user.CurrentRoom.GetActiveEnemiesCount(RoomHandler.ActiveEnemyType.All) != 0) && base.CanBeUsed(user);
  }

  protected override void DoEffect(PlayerController user)
  {
    if (this.IsCigarettes)
    {
      user.healthHaver.ApplyDamage(0.5f, Vector2.zero, StringTableManager.GetEnemiesString("#SMOKING"), ignoreInvulnerabilityFrames: true);
      user.ownerlessStatModifiers.Add(new StatModifier()
      {
        statToBoost = PlayerStats.StatType.Coolness,
        modifyType = StatModifier.ModifyMethod.ADDITIVE,
        amount = 1f
      });
      user.stats.RecalculateStats(user);
    }
    else if (this.itemName == "Molotov" && (bool) (UnityEngine.Object) user && user.HasActiveBonusSynergy(CustomSynergyType.DOUBLE_MOLOTOV))
    {
      user.CurrentGun.GainAmmo(5);
      int num = (int) AkSoundEngine.PostEvent("Play_OBJ_ammo_pickup_01", this.gameObject);
      return;
    }
    if (this.IsKageBunshinItem && (bool) (UnityEngine.Object) user && user.HasActiveBonusSynergy(CustomSynergyType.KINJUTSU))
    {
      for (int index = 0; index < 3; ++index)
      {
        float angleFromAim = (float) (90 * (index + 1));
        this.DoSpawn(user, angleFromAim);
      }
      if (this.PreventCooldownWhileExtant)
        this.IsCurrentlyActive = true;
      if (string.IsNullOrEmpty(this.AudioEvent))
        return;
      int num = (int) AkSoundEngine.PostEvent(this.AudioEvent, this.gameObject);
    }
    else if (this.SpawnRadialCopies)
    {
      for (int index = 0; index < this.RadialCopiesToSpawn; ++index)
      {
        float angleFromAim = 360f / (float) this.RadialCopiesToSpawn * (float) index;
        this.DoSpawn(user, angleFromAim);
      }
    }
    else
    {
      this.DoSpawn(user, 0.0f);
      if (this.PreventCooldownWhileExtant)
        this.IsCurrentlyActive = true;
      if (string.IsNullOrEmpty(this.AudioEvent))
        return;
      int num = (int) AkSoundEngine.PostEvent(this.AudioEvent, this.gameObject);
    }
  }

  public override void Update()
  {
    if (this.IsCurrentlyActive && this.PreventCooldownWhileExtant && !(bool) (UnityEngine.Object) this.spawnedPlayerObject)
    {
      if ((double) this.m_elapsedCooldownWhileExtantTimer < 0.5)
      {
        this.m_elapsedCooldownWhileExtantTimer += BraveTime.DeltaTime;
      }
      else
      {
        Debug.LogError((object) "clearing the dillywop");
        this.m_elapsedCooldownWhileExtantTimer = 0.0f;
        this.IsCurrentlyActive = false;
      }
    }
    base.Update();
  }

  protected void DoSpawn(PlayerController user, float angleFromAim)
  {
    if (!string.IsNullOrEmpty(this.enemyGuidToSpawn))
      this.objectToSpawn = EnemyDatabase.GetOrLoadByGuid(this.enemyGuidToSpawn).gameObject;
    GameObject original = this.objectToSpawn;
    if (this.HasOverrideSynergyItem && user.HasActiveBonusSynergy(this.RequiredSynergy))
      original = this.SynergyObjectToSpawn;
    Projectile component1 = original.GetComponent<Projectile>();
    this.m_elapsedCooldownWhileExtantTimer = 0.0f;
    if ((UnityEngine.Object) component1 != (UnityEngine.Object) null)
    {
      Vector2 v = (Vector2) (user.unadjustedAimPoint - user.LockedApproximateSpriteCenter);
      this.spawnedPlayerObject = UnityEngine.Object.Instantiate<GameObject>(original, (Vector3) user.specRigidbody.UnitCenter, Quaternion.Euler(0.0f, 0.0f, BraveMathCollege.Atan2Degrees(v)));
    }
    else if ((double) this.tossForce == 0.0)
    {
      GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(original, (Vector3) user.specRigidbody.UnitCenter, Quaternion.identity);
      this.spawnedPlayerObject = gameObject;
      tk2dBaseSprite component2 = gameObject.GetComponent<tk2dBaseSprite>();
      if ((UnityEngine.Object) component2 != (UnityEngine.Object) null)
      {
        component2.PlaceAtPositionByAnchor(user.specRigidbody.UnitCenter.ToVector3ZUp(component2.transform.position.z), tk2dBaseSprite.Anchor.MiddleCenter);
        if ((UnityEngine.Object) component2.specRigidbody != (UnityEngine.Object) null)
          component2.specRigidbody.RegisterGhostCollisionException(user.specRigidbody);
      }
      KageBunshinController component3 = gameObject.GetComponent<KageBunshinController>();
      if ((bool) (UnityEngine.Object) component3)
        component3.InitializeOwner(user);
      if (this.IsKageBunshinItem && user.HasActiveBonusSynergy(CustomSynergyType.KINJUTSU))
      {
        component3.UsesRotationInsteadOfInversion = true;
        component3.RotationAngle = angleFromAim;
      }
      gameObject.transform.position = gameObject.transform.position.Quantize(1f / 16f);
    }
    else
    {
      Vector3 vector3 = user.unadjustedAimPoint - user.LockedApproximateSpriteCenter;
      Vector3 unitCenter = (Vector3) user.specRigidbody.UnitCenter;
      if ((double) vector3.y > 0.0)
        unitCenter += Vector3.up * 0.25f;
      GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(original, unitCenter, Quaternion.identity);
      tk2dBaseSprite component4 = gameObject.GetComponent<tk2dBaseSprite>();
      if ((bool) (UnityEngine.Object) component4)
        component4.PlaceAtPositionByAnchor(unitCenter, tk2dBaseSprite.Anchor.MiddleCenter);
      this.spawnedPlayerObject = gameObject;
      Vector2 vector2 = (Vector2) (user.unadjustedAimPoint - user.LockedApproximateSpriteCenter);
      Vector2 spawnDirection = (Vector2) (Quaternion.Euler(0.0f, 0.0f, angleFromAim) * (Vector3) vector2);
      DebrisObject debrisObject = LootEngine.DropItemWithoutInstantiating(gameObject, gameObject.transform.position, spawnDirection, this.tossForce, false, disablePostprocessing: true);
      if ((bool) (UnityEngine.Object) gameObject.GetComponent<BlackHoleDoer>())
      {
        debrisObject.PreventFallingInPits = true;
        debrisObject.PreventAbsorption = true;
      }
      if ((double) vector3.y > 0.0 && (bool) (UnityEngine.Object) debrisObject)
      {
        debrisObject.additionalHeightBoost = -1f;
        if ((bool) (UnityEngine.Object) debrisObject.sprite)
          debrisObject.sprite.UpdateZDepth();
      }
      debrisObject.IsAccurateDebris = true;
      debrisObject.Priority = EphemeralObject.EphemeralPriority.Critical;
      debrisObject.bounceCount = !this.canBounce ? 0 : 1;
    }
    if (!(bool) (UnityEngine.Object) this.spawnedPlayerObject)
      return;
    PortableTurretController component5 = this.spawnedPlayerObject.GetComponent<PortableTurretController>();
    if ((bool) (UnityEngine.Object) component5)
      component5.sourcePlayer = this.LastOwner;
    Projectile componentInChildren1 = this.spawnedPlayerObject.GetComponentInChildren<Projectile>();
    if ((bool) (UnityEngine.Object) componentInChildren1)
    {
      componentInChildren1.Owner = (GameActor) this.LastOwner;
      componentInChildren1.TreatedAsNonProjectileForChallenge = true;
    }
    SpawnObjectItem componentInChildren2 = this.spawnedPlayerObject.GetComponentInChildren<SpawnObjectItem>();
    if (!(bool) (UnityEngine.Object) componentInChildren2)
      return;
    componentInChildren2.SpawningPlayer = this.LastOwner;
  }

  protected override void OnPreDrop(PlayerController user)
  {
    if ((UnityEngine.Object) this.spawnedPlayerObject != (UnityEngine.Object) null)
    {
      PortableTurretController component = this.spawnedPlayerObject.GetComponent<PortableTurretController>();
      if ((UnityEngine.Object) component != (UnityEngine.Object) null)
        component.NotifyDropped();
    }
    base.OnPreDrop(user);
  }

  protected override void OnDestroy() => base.OnDestroy();
}
