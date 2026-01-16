// Decompiled with JetBrains decompiler
// Type: AIBulletBank
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

#nullable disable
public class AIBulletBank : BraveBehaviour, IBulletManager
{
  public List<AIBulletBank.Entry> Bullets;
  public bool useDefaultBulletIfMissing;
  public List<Transform> transforms;
  [NonSerialized]
  public bool rampBullets;
  [NonSerialized]
  public float rampStartHeight = 2f;
  [NonSerialized]
  public float rampTime = 1f;
  [NonSerialized]
  public Gun OverrideGun;
  public Action<Projectile> OnProjectileCreated;
  public Action<string, Projectile> OnProjectileCreatedWithSource;
  public Vector2? FixedPlayerPosition;
  private GameObject m_cachedSoundChild;
  private float m_timeScale = 1f;
  private Vector2? m_cachedPlayerPosition;
  private bool m_playVfx = true;
  private bool m_playAudio = true;
  private bool m_playShells = true;
  private string m_cachedActorName;

  public event Action<Bullet, Projectile> OnBulletSpawned;

  public bool PlayVfx
  {
    get => this.m_playVfx;
    set => this.m_playVfx = value;
  }

  public bool PlayAudio
  {
    get => this.m_playAudio;
    set => this.m_playAudio = value;
  }

  public bool PlayShells
  {
    get => this.m_playShells;
    set => this.m_playShells = value;
  }

  public SpeculativeRigidbody FixedPlayerRigidbody { get; set; }

  public Vector2 FixedPlayerRigidbodyLastPosition { get; set; }

  public bool CollidesWithEnemies { get; set; }

  public SpeculativeRigidbody SpecificRigidbodyException { get; set; }

  public GameObject SoundChild
  {
    get
    {
      if (!(bool) (UnityEngine.Object) this.m_cachedSoundChild)
      {
        this.m_cachedSoundChild = new GameObject("sound child");
        this.m_cachedSoundChild.transform.parent = this.transform;
        this.m_cachedSoundChild.transform.localPosition = Vector3.zero;
      }
      return this.m_cachedSoundChild;
    }
  }

  public string ActorName
  {
    get => this.m_cachedActorName;
    set => this.m_cachedActorName = value;
  }

  public float TimeScale
  {
    get => this.m_timeScale;
    set => this.m_timeScale = value;
  }

  public bool SuppressPlayerVelocityAveraging { get; set; }

  public void Awake()
  {
    this.CollidesWithEnemies = (bool) (UnityEngine.Object) this.aiShooter && this.aiShooter.CanShootOtherEnemies;
    this.SpecificRigidbodyException = this.specRigidbody;
    if (this.Bullets == null)
      return;
    for (int index1 = 0; index1 < this.Bullets.Count; ++index1)
    {
      AIBulletBank.Entry bullet = this.Bullets[index1];
      if (bullet.preloadCount > 0)
      {
        Transform[] transformArray = new Transform[bullet.preloadCount];
        for (int index2 = 0; index2 < bullet.preloadCount; ++index2)
          transformArray[index2] = SpawnManager.PoolManager.Spawn(bullet.BulletObject.transform);
        for (int index3 = 0; index3 < bullet.preloadCount; ++index3)
          SpawnManager.PoolManager.Despawn(transformArray[index3]);
      }
    }
  }

  public void Start()
  {
    if ((bool) (UnityEngine.Object) this.aiActor)
      this.m_cachedActorName = this.aiActor.GetActorName();
    if (!(bool) (UnityEngine.Object) this.encounterTrackable || !string.IsNullOrEmpty(this.m_cachedActorName))
      return;
    this.m_cachedActorName = this.encounterTrackable.GetModifiedDisplayName();
  }

  public void Update()
  {
    if (!(bool) (UnityEngine.Object) this.FixedPlayerRigidbody || !(bool) (UnityEngine.Object) this.FixedPlayerRigidbody.healthHaver || !this.FixedPlayerRigidbody.healthHaver.IsAlive)
      return;
    this.FixedPlayerRigidbodyLastPosition = this.FixedPlayerRigidbody.GetUnitCenter(ColliderType.HitBox);
  }

  public void LateUpdate()
  {
    for (int index = 0; index < this.Bullets.Count; ++index)
    {
      this.Bullets[index].m_playedEffectsThisFrame = false;
      this.Bullets[index].m_playedAudioThisFrame = false;
      this.Bullets[index].m_playedShellsThisFrame = false;
    }
  }

  protected override void OnDestroy()
  {
    if (!(bool) (UnityEngine.Object) this.aiActor || !(bool) (UnityEngine.Object) this.aiActor.TargetRigidbody || !PhysicsEngine.HasInstance)
      return;
    this.FixedPlayerRigidbody = this.aiActor.TargetRigidbody;
    this.FixedPlayerRigidbodyLastPosition = this.FixedPlayerRigidbody.GetUnitCenter(ColliderType.HitBox);
  }

  public GameObject CreateProjectileFromBank(
    Vector2 position,
    float direction,
    string bulletName,
    string spawnTransform = null,
    bool suppressVfx = false,
    bool firstBulletOfAttack = true,
    bool forceBlackBullet = false)
  {
    AIBulletBank.Entry bullet = this.GetBullet(bulletName);
    GameObject prefab = bullet.BulletObject;
    if (!(bool) (UnityEngine.Object) prefab && (bool) (UnityEngine.Object) this.aiShooter.CurrentGun)
      prefab = this.aiShooter.CurrentGun.singleModule.GetCurrentProjectile().gameObject;
    bool ignoresPools = false;
    Projectile component1 = prefab.GetComponent<Projectile>();
    if ((bool) (UnityEngine.Object) component1 && component1.BulletScriptSettings.preventPooling)
      ignoresPools = true;
    GameObject projectileFromBank = SpawnManager.SpawnProjectile(prefab, (Vector3) position, Quaternion.Euler(0.0f, 0.0f, direction), ignoresPools);
    Projectile component2 = projectileFromBank.GetComponent<Projectile>();
    if ((UnityEngine.Object) component2 != (UnityEngine.Object) null)
    {
      if (forceBlackBullet)
        component2.ForceBlackBullet = true;
      if ((bool) (UnityEngine.Object) this.gameActor)
        component2.SetOwnerSafe(this.gameActor, this.m_cachedActorName);
      else if ((bool) (UnityEngine.Object) this.encounterTrackable)
        component2.OwnerName = this.encounterTrackable.GetModifiedDisplayName();
      if (ignoresPools)
        component2.OnSpawned();
      if (bullet.suppressHitEffectsIfOffscreen || (bool) (UnityEngine.Object) this.healthHaver && this.healthHaver.IsBoss)
        component2.hitEffects.suppressHitEffectsIfOffscreen = true;
    }
    if (this.m_playAudio && bullet.PlayAudio)
    {
      bool flag = true;
      if (bullet.AudioLimitOncePerFrame)
        flag &= !bullet.m_playedAudioThisFrame;
      if (bullet.AudioLimitOncePerAttack)
        flag &= firstBulletOfAttack;
      if (flag)
      {
        if (!string.IsNullOrEmpty(bullet.AudioSwitch))
        {
          int num1 = (int) AkSoundEngine.SetSwitch("WPN_Guns", bullet.AudioSwitch, this.SoundChild);
          int num2 = (int) AkSoundEngine.PostEvent(bullet.AudioEvent, this.SoundChild);
        }
        else if ((bool) (UnityEngine.Object) this)
        {
          int num = (int) AkSoundEngine.PostEvent(bullet.AudioEvent, this.gameObject);
        }
        bullet.m_playedAudioThisFrame = true;
      }
    }
    if (this.m_playVfx && !suppressVfx && (!bullet.MuzzleLimitOncePerFrame || !bullet.m_playedEffectsThisFrame))
    {
      float zRotation = direction;
      if (bullet.MuzzleInheritsTransformDirection && !string.IsNullOrEmpty(spawnTransform))
        zRotation = this.GetTransformRotation(spawnTransform);
      if (bullet.MuzzleFlashEffects.type != VFXPoolType.None)
      {
        bullet.MuzzleFlashEffects.SpawnAtPosition((Vector3) position, zRotation);
        bullet.m_playedEffectsThisFrame = true;
      }
      else
      {
        Gun gun = (Gun) null;
        if ((bool) (UnityEngine.Object) this.aiShooter && this.aiShooter.enabled)
          gun = this.aiShooter.CurrentGun;
        if ((bool) (UnityEngine.Object) this.OverrideGun)
          gun = this.OverrideGun;
        if ((bool) (UnityEngine.Object) gun)
        {
          gun.HandleShootAnimation((ProjectileModule) null);
          gun.HandleShootEffects((ProjectileModule) null);
          bullet.m_playedEffectsThisFrame = true;
        }
      }
    }
    if (this.m_playShells && (!bullet.ShellsLimitOncePerFrame || !bullet.m_playedShellsThisFrame) && bullet.SpawnShells)
    {
      this.SpawnShellCasingAtPosition(bullet);
      bullet.m_playedShellsThisFrame = true;
    }
    Projectile component3 = projectileFromBank.GetComponent<Projectile>();
    if (bullet.OverrideProjectile)
    {
      component3.baseData.SetAll(bullet.ProjectileData);
      component3.UpdateSpeed();
    }
    if ((bool) (UnityEngine.Object) this.aiActor && this.aiActor.IsBlackPhantom)
    {
      component3.baseData.speed *= this.aiActor.BlackPhantomProperties.BulletSpeedMultiplier;
      component3.UpdateSpeed();
    }
    if (GameManager.Options.DebrisQuantity != GameOptions.GenericHighMedLowOption.HIGH)
      component3.damagesWalls = false;
    if ((bool) (UnityEngine.Object) this.healthHaver && this.healthHaver.IsBoss)
      component3.damagesWalls = false;
    bool flag1 = (bool) (UnityEngine.Object) this.aiActor && this.aiActor.CanTargetEnemies;
    component3.collidesWithEnemies = this.CollidesWithEnemies || bullet.forceCanHitEnemies || flag1;
    component3.UpdateCollisionMask();
    component3.specRigidbody.RegisterSpecificCollisionException(this.SpecificRigidbodyException);
    component3.SendInDirection(BraveMathCollege.DegreesToVector(direction), false);
    if (bullet.rampBullets)
    {
      if ((double) bullet.conditionalMinDegFromNorth <= 0.0 || (double) BraveMathCollege.AbsAngleBetween(90f, this.aiAnimator.FacingDirection) > (double) bullet.conditionalMinDegFromNorth)
        component3.Ramp(bullet.rampStartHeight, bullet.rampTime);
    }
    else if (this.rampBullets)
      component3.Ramp(this.rampStartHeight, this.rampTime);
    else if ((bool) (UnityEngine.Object) this.aiShooter && this.aiShooter.rampBullets)
      component3.Ramp(this.aiShooter.rampStartHeight, this.aiShooter.rampTime);
    if (this.OnProjectileCreated != null)
      this.OnProjectileCreated(component3);
    return projectileFromBank;
  }

  public void PostWwiseEvent(string AudioEvent, string AudioSwitch = null)
  {
    if (!string.IsNullOrEmpty(AudioSwitch))
    {
      int num1 = (int) AkSoundEngine.SetSwitch("WPN_Guns", AudioSwitch, this.SoundChild);
      int num2 = (int) AkSoundEngine.PostEvent(AudioEvent, this.SoundChild);
    }
    else
    {
      if (!(bool) (UnityEngine.Object) this)
        return;
      int num = (int) AkSoundEngine.PostEvent(AudioEvent, this.gameObject);
    }
  }

  public AIBulletBank.Entry GetBullet(string bulletName = "default")
  {
    AIBulletBank.Entry bullet = (AIBulletBank.Entry) null;
    if (string.IsNullOrEmpty(bulletName))
      bulletName = "default";
    for (int index = 0; index < this.Bullets.Count; ++index)
    {
      if (string.Equals(this.Bullets[index].Name, bulletName, StringComparison.OrdinalIgnoreCase))
        bullet = this.Bullets[index];
    }
    if (bullet == null && this.useDefaultBulletIfMissing)
    {
      for (int index = 0; index < this.Bullets.Count; ++index)
      {
        if (this.Bullets[index].Name.ToLower() == "default")
          bullet = this.Bullets[index];
      }
      if (bullet == null && this.Bullets.Count > 0)
        bullet = this.Bullets[0];
    }
    if (bullet != null)
      return bullet;
    Debug.LogError((object) $"Missing bank entry for bullet: {bulletName}!");
    return (AIBulletBank.Entry) null;
  }

  private void SpawnShellCasingAtPosition(AIBulletBank.Entry bankEntry)
  {
    if (!((UnityEngine.Object) bankEntry.ShellPrefab != (UnityEngine.Object) null))
      return;
    float angle = BraveMathCollege.ClampAngle360(bankEntry.ShellTransform.eulerAngles.z);
    Vector3 position = bankEntry.ShellTransform.position;
    GameObject gameObject = SpawnManager.SpawnDebris(bankEntry.ShellPrefab, position, Quaternion.Euler(0.0f, 0.0f, !bankEntry.DontRotateShell ? angle : 0.0f));
    ShellCasing component1 = gameObject.GetComponent<ShellCasing>();
    if ((UnityEngine.Object) component1 != (UnityEngine.Object) null)
      component1.Trigger();
    DebrisObject component2 = gameObject.GetComponent<DebrisObject>();
    if (!((UnityEngine.Object) component2 != (UnityEngine.Object) null))
      return;
    float magnitude = bankEntry.ShellForce + UnityEngine.Random.Range(-bankEntry.ShellForceVariance, bankEntry.ShellForceVariance);
    Vector3 vector = (Vector3) BraveMathCollege.DegreesToVector(angle, magnitude);
    Vector3 startingForce = new Vector3(vector.x, 0.0f, vector.y);
    float num1 = this.specRigidbody.UnitBottom + bankEntry.ShellGroundOffset;
    float num2 = (float) ((double) position.y - (double) this.transform.position.y + 0.20000000298023224);
    float startingHeight = (float) ((double) component2.transform.position.y - (double) num1 + (double) UnityEngine.Random.value * 0.5);
    component2.additionalHeightBoost = num2 - startingHeight;
    component2.Trigger(startingForce, startingHeight);
  }

  public Vector2 PlayerPosition()
  {
    if (this.FixedPlayerPosition.HasValue)
      return this.FixedPlayerPosition.Value;
    if ((bool) (UnityEngine.Object) this.FixedPlayerRigidbody)
    {
      if (!(bool) (UnityEngine.Object) this.FixedPlayerRigidbody.healthHaver)
        return this.FixedPlayerRigidbody.Velocity;
      return this.FixedPlayerRigidbody.healthHaver.IsAlive ? this.FixedPlayerRigidbody.GetUnitCenter(ColliderType.HitBox) : this.FixedPlayerRigidbodyLastPosition;
    }
    if (!(bool) (UnityEngine.Object) this.aiActor)
    {
      PlayerController playerClosestToPoint = GameManager.Instance.GetActivePlayerClosestToPoint((Vector2) (!(bool) (UnityEngine.Object) this.transform ? BraveUtility.ScreenCenterWorldPoint() : this.transform.position));
      if (!(bool) (UnityEngine.Object) playerClosestToPoint)
        return (Vector2) BraveUtility.ScreenCenterWorldPoint();
      this.m_cachedPlayerPosition = new Vector2?(playerClosestToPoint.specRigidbody.GetUnitCenter(ColliderType.HitBox));
      return this.m_cachedPlayerPosition.Value;
    }
    if ((bool) (UnityEngine.Object) this.aiActor.TargetRigidbody)
    {
      this.m_cachedPlayerPosition = new Vector2?(this.aiActor.TargetRigidbody.GetUnitCenter(ColliderType.HitBox));
      return this.m_cachedPlayerPosition.Value;
    }
    return this.m_cachedPlayerPosition.HasValue ? this.m_cachedPlayerPosition.Value : (Vector2) BraveUtility.ScreenCenterWorldPoint();
  }

  public Vector2 PlayerVelocity()
  {
    if ((bool) (UnityEngine.Object) this.FixedPlayerRigidbody)
    {
      if ((bool) (UnityEngine.Object) this.FixedPlayerRigidbody.healthHaver && !this.FixedPlayerRigidbody.healthHaver.IsAlive)
        return Vector2.zero;
      PlayerController gameActor = this.FixedPlayerRigidbody.gameActor as PlayerController;
      return (bool) (UnityEngine.Object) gameActor ? gameActor.AverageVelocity : this.FixedPlayerRigidbody.Velocity;
    }
    if (!(bool) (UnityEngine.Object) this.aiActor)
    {
      PlayerController playerClosestToPoint = GameManager.Instance.GetActivePlayerClosestToPoint((Vector2) (!(bool) (UnityEngine.Object) this.transform ? BraveUtility.ScreenCenterWorldPoint() : this.transform.position));
      if (!(bool) (UnityEngine.Object) playerClosestToPoint)
        return Vector2.zero;
      return this.SuppressPlayerVelocityAveraging ? playerClosestToPoint.Velocity : playerClosestToPoint.AverageVelocity;
    }
    if (!(bool) (UnityEngine.Object) this.aiActor.TargetRigidbody)
      return Vector2.zero;
    PlayerController gameActor1 = this.aiActor.TargetRigidbody.gameActor as PlayerController;
    if (!(bool) (UnityEngine.Object) gameActor1)
      return this.aiActor.TargetRigidbody.Velocity;
    return this.SuppressPlayerVelocityAveraging ? gameActor1.Velocity : gameActor1.AverageVelocity;
  }

  public void BulletSpawnedHandler(Bullet bullet)
  {
    string bulletName = string.IsNullOrEmpty(bullet.BankName) ? "default" : bullet.BankName;
    GameObject projectileFromBank = this.CreateProjectileFromBank(bullet.Position, bullet.Direction, bulletName, bullet.SpawnTransform, bullet.SuppressVfx, bullet.FirstBulletOfAttack, bullet.ForceBlackBullet);
    Projectile component = projectileFromBank.GetComponent<Projectile>();
    bullet.Projectile = component;
    if ((bool) (UnityEngine.Object) component && component.BulletScriptSettings.overrideMotion)
      return;
    component.specRigidbody.Velocity = Vector2.zero;
    BulletScriptBehavior bulletScriptBehavior = projectileFromBank.GetComponent<BulletScriptBehavior>();
    if (!(bool) (UnityEngine.Object) bulletScriptBehavior)
    {
      bulletScriptBehavior = projectileFromBank.AddComponent<BulletScriptBehavior>();
      component.braveBulletScript = bulletScriptBehavior;
    }
    component.IsBulletScript = true;
    if (this.OnBulletSpawned != null)
      this.OnBulletSpawned(bullet, component);
    bullet.Parent = projectileFromBank;
    bullet.Initialize();
    bulletScriptBehavior.Initialize(bullet);
  }

  public void RemoveBullet(Bullet deadBullet)
  {
    if (deadBullet.DontDestroyGameObject)
      return;
    if ((bool) (UnityEngine.Object) deadBullet.Projectile && SpawnManager.HasInstance)
      deadBullet.Projectile.DieInAir();
    else
      UnityEngine.Object.Destroy((UnityEngine.Object) deadBullet.Parent);
  }

  public void DestroyBullet(Bullet deadBullet, bool suppressInAirEffects = false)
  {
    if ((UnityEngine.Object) deadBullet.Parent == (UnityEngine.Object) null)
      return;
    BulletScriptBehavior component = deadBullet.Parent.GetComponent<BulletScriptBehavior>();
    if (deadBullet.DontDestroyGameObject)
    {
      if (!(bool) (UnityEngine.Object) component)
        return;
      component.bullet = (Bullet) null;
    }
    else if ((bool) (UnityEngine.Object) deadBullet.Projectile && SpawnManager.HasInstance)
    {
      deadBullet.Projectile.DieInAir(suppressInAirEffects);
      if (!(bool) (UnityEngine.Object) component)
        return;
      component.bullet = (Bullet) null;
    }
    else
      UnityEngine.Object.Destroy((UnityEngine.Object) deadBullet.Parent);
  }

  public Transform GetTransform(string transformName)
  {
    for (int index = 0; index < this.transforms.Count; ++index)
    {
      if (this.transforms[index].name == transformName)
        return this.transforms[index];
    }
    return (Transform) null;
  }

  public Vector2 TransformOffset(Vector2 pos, string transformName)
  {
    Transform transform = (Transform) null;
    for (int index = 0; index < this.transforms.Count; ++index)
    {
      if (this.transforms[index].name == transformName)
        transform = this.transforms[index];
    }
    return (UnityEngine.Object) transform == (UnityEngine.Object) null ? pos : transform.position.XY();
  }

  public float GetTransformRotation(string transformName)
  {
    Transform transform = (Transform) null;
    for (int index = 0; index < this.transforms.Count; ++index)
    {
      if (this.transforms[index].name == transformName)
        transform = this.transforms[index];
    }
    return (UnityEngine.Object) transform == (UnityEngine.Object) null ? 0.0f : transform.eulerAngles.z;
  }

  public Animation GetUnityAnimation() => this.unityAnimation;

  [Serializable]
  public class Entry
  {
    public string Name;
    public GameObject BulletObject;
    public bool OverrideProjectile;
    [ShowInInspectorIf("OverrideProjectile", false)]
    public ProjectileData ProjectileData;
    [FormerlySerializedAs("BulletMlAudio")]
    public bool PlayAudio;
    [ShowInInspectorIf("PlayAudio", true)]
    [FormerlySerializedAs("BulletMlAudioSwitch")]
    public string AudioSwitch;
    [ShowInInspectorIf("PlayAudio", true)]
    [FormerlySerializedAs("BulletMlAudioEvent")]
    public string AudioEvent;
    [ShowInInspectorIf("PlayAudio", true)]
    [FormerlySerializedAs("LimitBulletMlAudio")]
    public bool AudioLimitOncePerFrame = true;
    [ShowInInspectorIf("PlayAudio", true)]
    public bool AudioLimitOncePerAttack;
    public VFXPool MuzzleFlashEffects;
    [ShowInInspectorIf("MuzzleFlashEffects", true)]
    [FormerlySerializedAs("LimitBulletMlVfx")]
    public bool MuzzleLimitOncePerFrame = true;
    [ShowInInspectorIf("MuzzleFlashEffects", true)]
    public bool MuzzleInheritsTransformDirection;
    public bool SpawnShells;
    [ShowInInspectorIf("SpawnShells", true)]
    public Transform ShellTransform;
    [ShowInInspectorIf("SpawnShells", true)]
    public GameObject ShellPrefab;
    [ShowInInspectorIf("SpawnShells", true)]
    public float ShellForce = 1.75f;
    [ShowInInspectorIf("SpawnShells", true)]
    public float ShellForceVariance = 0.75f;
    [ShowInInspectorIf("SpawnShells", true)]
    public bool DontRotateShell;
    [ShowInInspectorIf("SpawnShells", true)]
    public float ShellGroundOffset;
    [ShowInInspectorIf("SpawnShells", true)]
    public bool ShellsLimitOncePerFrame;
    public bool rampBullets;
    [ShowInInspectorIf("rampBullets", true)]
    public float rampStartHeight = 2f;
    [ShowInInspectorIf("rampBullets", true)]
    public float rampTime = 1f;
    [ShowInInspectorIf("rampBullets", true)]
    public float conditionalMinDegFromNorth;
    public bool forceCanHitEnemies;
    public bool suppressHitEffectsIfOffscreen;
    public int preloadCount;

    public Entry()
    {
    }

    public Entry(AIBulletBank.Entry other)
    {
      this.Name = other.Name;
      this.BulletObject = other.BulletObject;
      this.OverrideProjectile = other.OverrideProjectile;
      this.ProjectileData = new ProjectileData(other.ProjectileData);
      this.PlayAudio = other.PlayAudio;
      this.AudioSwitch = other.AudioSwitch;
      this.AudioEvent = other.AudioEvent;
      this.AudioLimitOncePerFrame = other.AudioLimitOncePerFrame;
      this.AudioLimitOncePerAttack = other.AudioLimitOncePerAttack;
      this.MuzzleFlashEffects = other.MuzzleFlashEffects;
      this.MuzzleLimitOncePerFrame = other.MuzzleLimitOncePerFrame;
      this.MuzzleInheritsTransformDirection = other.MuzzleInheritsTransformDirection;
      this.SpawnShells = other.SpawnShells;
      this.ShellTransform = other.ShellTransform;
      this.ShellPrefab = other.ShellPrefab;
      this.ShellForce = other.ShellForce;
      this.ShellForceVariance = other.ShellForceVariance;
      this.DontRotateShell = other.DontRotateShell;
      this.ShellGroundOffset = other.ShellGroundOffset;
      this.ShellsLimitOncePerFrame = other.ShellsLimitOncePerFrame;
      this.rampBullets = other.rampBullets;
      this.rampStartHeight = other.rampStartHeight;
      this.rampTime = other.rampTime;
      this.conditionalMinDegFromNorth = other.conditionalMinDegFromNorth;
      this.forceCanHitEnemies = other.forceCanHitEnemies;
      this.suppressHitEffectsIfOffscreen = other.suppressHitEffectsIfOffscreen;
      this.preloadCount = other.preloadCount;
    }

    public bool m_playedAudioThisFrame { get; set; }

    public bool m_playedEffectsThisFrame { get; set; }

    public bool m_playedShellsThisFrame { get; set; }
  }
}
