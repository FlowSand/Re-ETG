// Decompiled with JetBrains decompiler
// Type: HealthHaver
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Serialization;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class HealthHaver : BraveBehaviour
    {
      protected const float c_flashTime = 0.04f;
      protected const float c_flashDowntime = 0.2f;
      protected const float c_incorporealityFlashOnTime = 0.12f;
      protected const float c_incorporealityFlashOffTime = 0.12f;
      protected const float c_bossDpsCapWindow = 3f;
      public Action<HealthHaver, HealthHaver.ModifyDamageEventArgs> ModifyDamage;
      public Action<HealthHaver, HealthHaver.ModifyHealingEventArgs> ModifyHealing;
      [TogglesProperty("quantizedIncrement", null)]
      public bool quantizeHealth;
      [HideInInspector]
      public float quantizedIncrement = 0.5f;
      public bool flashesOnDamage = true;
      [TogglesProperty("incorporealityTime", "Incorporeality Period")]
      public bool incorporealityOnDamage;
      [HideInInspector]
      public float incorporealityTime = 1f;
      public bool PreventAllDamage;
      [HideInInspector]
      public bool persistsOnDeath;
      [NonSerialized]
      protected float m_curseHealthMaximum = float.MaxValue;
      [NonSerialized]
      public bool HasCrest;
      [NonSerialized]
      public bool HasRatchetHealthBar;
      [SerializeField]
      protected float maximumHealth = 10f;
      [HideInInspector]
      [SerializeField]
      protected float currentHealth = 10f;
      [SerializeField]
      protected float currentArmor;
      [SerializeField]
      [TogglesProperty("invulnerabilityPeriod", "Invulnerability Period")]
      protected bool usesInvulnerabilityPeriod;
      [HideInInspector]
      [SerializeField]
      protected float invulnerabilityPeriod = 0.5f;
      [ShowInInspectorIf("usesInvulnerabilityPeriod", true)]
      public bool useFortunesFavorInvulnerability;
      public GameObject deathEffect;
      public string damagedAudioEvent = string.Empty;
      public string overrideDeathAudioEvent = string.Empty;
      public string overrideDeathAnimation = string.Empty;
      [Space(5f)]
      public bool shakesCameraOnDamage;
      [ShowInInspectorIf("shakesCameraOnDamage", false)]
      public ScreenShakeSettings cameraShakeOnDamage;
      [Header("Damage Overrides")]
      public List<DamageTypeModifier> damageTypeModifiers;
      public bool healthIsNumberOfHits;
      public bool OnlyAllowSpecialBossDamage;
      [Header("BulletScript")]
      [FormerlySerializedAs("spawnsBulletMl")]
      public bool spawnBulletScript;
      [FormerlySerializedAs("chanceToSpawnBulletMl")]
      [ShowInInspectorIf("spawnBulletScript", true)]
      public float chanceToSpawnBulletScript;
      [FormerlySerializedAs("overrideDeathAnimBulletMl")]
      [ShowInInspectorIf("spawnBulletScript", true)]
      public string overrideDeathAnimBulletScript;
      [ShowInInspectorIf("spawnBulletScript", true)]
      [FormerlySerializedAs("noCorpseWhenBulletMlDeath")]
      public bool noCorpseWhenBulletScriptDeath;
      [FormerlySerializedAs("bulletMlType")]
      [ShowInInspectorIf("spawnBulletScript", true)]
      public HealthHaver.BulletScriptType bulletScriptType;
      public BulletScriptSelector bulletScript;
      [Header("For Bosses")]
      public HealthHaver.BossBarType bossHealthBar;
      public string overrideBossName;
      public bool forcePreventVictoryMusic;
      [NonSerialized]
      public string lastIncurredDamageSource;
      [NonSerialized]
      public Vector2 lastIncurredDamageDirection;
      [NonSerialized]
      public bool NextShotKills;
      protected List<Material> materialsToFlash;
      protected List<Material> outlineMaterialsToFlash;
      protected List<Material> materialsToEnableBrightnessClampOn;
      protected List<Color> sourceColors;
      protected bool isPlayerCharacter;
      private bool m_isFlashing;
      private bool m_isIncorporeal;
      private float m_damageCap = -1f;
      private float m_bossDpsCap = -1f;
      private float m_recentBossDps;
      private PlayerController m_player;
      [NonSerialized]
      public float minimumHealth;
      [NonSerialized]
      public List<tk2dBaseSprite> bodySprites = new List<tk2dBaseSprite>();
      [NonSerialized]
      public List<SpeculativeRigidbody> bodyRigidbodies;
      [NonSerialized]
      public float AllDamageMultiplier = 1f;
      [NonSerialized]
      private Dictionary<PixelCollider, tk2dBaseSprite> m_independentDamageFlashers;
      protected bool vulnerable = true;
      public float GlobalPixelColliderDamageMultiplier = 1f;
      [NonSerialized]
      public bool NextDamageIgnoresArmor;
      private bool isFirstFrame = true;
      private static int m_hitBarkLimiter;
      private Coroutine m_flashOnHitCoroutine;

      public event Action<Vector2> OnPreDeath;

      public event Action<Vector2> OnDeath;

      public event HealthHaver.OnDamagedEvent OnDamaged;

      public event HealthHaver.OnHealthChangedEvent OnHealthChanged;

      public float CursedMaximum
      {
        get => this.m_curseHealthMaximum;
        set
        {
          this.m_curseHealthMaximum = value;
          this.currentHealth = Mathf.Min(this.currentHealth, this.AdjustedMaxHealth);
          if (this.OnHealthChanged == null)
            return;
          this.OnHealthChanged(this.GetCurrentHealth(), this.GetMaxHealth());
        }
      }

      protected float AdjustedMaxHealth
      {
        get => this.GetMaxHealth();
        set => this.maximumHealth = value;
      }

      public float GetMaxHealth() => Mathf.Min(this.CursedMaximum, this.maximumHealth);

      public float GetCurrentHealth() => this.currentHealth;

      public void ForceSetCurrentHealth(float h)
      {
        this.currentHealth = h;
        this.currentHealth = Mathf.Min(this.currentHealth, this.GetMaxHealth());
        if (this.OnHealthChanged == null)
          return;
        this.OnHealthChanged(this.currentHealth, this.GetMaxHealth());
      }

      public float Armor
      {
        get => this.currentArmor;
        set
        {
          if ((bool) (UnityEngine.Object) this.m_player && !this.m_player.ForceZeroHealthState && this.IsDead)
            return;
          this.currentArmor = value;
          if (this.OnHealthChanged == null)
            return;
          this.OnHealthChanged(this.currentHealth, this.AdjustedMaxHealth);
        }
      }

      public float GetCurrentHealthPercentage() => this.currentHealth / this.AdjustedMaxHealth;

      public bool IsVulnerable
      {
        get
        {
          if (this.isPlayerCharacter && this.m_player.rollStats.additionalInvulnerabilityFrames > 0)
          {
            for (int framesBack = 1; framesBack <= this.m_player.rollStats.additionalInvulnerabilityFrames; ++framesBack)
            {
              if (this.spriteAnimator.QueryPreviousInvulnerabilityFrame(framesBack))
                return false;
            }
          }
          return this.vulnerable && !this.spriteAnimator.QueryInvulnerabilityFrame();
        }
        set => this.vulnerable = value;
      }

      public bool IsAlive => (double) this.GetCurrentHealth() > 0.0 || (double) this.Armor > 0.0;

      public bool IsDead => (double) this.GetCurrentHealth() <= 0.0 && (double) this.Armor <= 0.0;

      public bool ManualDeathHandling { get; set; }

      public bool DisableStickyFriction { get; set; }

      public bool IsBoss => this.bossHealthBar != HealthHaver.BossBarType.None;

      public bool IsSubboss => this.bossHealthBar == HealthHaver.BossBarType.SubbossBar;

      public bool UsesSecondaryBossBar => this.bossHealthBar == HealthHaver.BossBarType.SecondaryBar;

      public bool UsesVerticalBossBar => this.bossHealthBar == HealthHaver.BossBarType.VerticalBar;

      public bool HasHealthBar
      {
        get
        {
          return this.bossHealthBar != HealthHaver.BossBarType.None && this.bossHealthBar != HealthHaver.BossBarType.SecretBar && this.bossHealthBar != HealthHaver.BossBarType.SubbossBar;
        }
      }

      public Vector2? OverrideKillCamPos { get; set; }

      public float? OverrideKillCamTime { get; set; }

      public bool TrackDuringDeath { get; set; }

      public bool SuppressContinuousKillCamBulletDestruction { get; set; }

      public bool SuppressDeathSounds { get; set; }

      public bool CanCurrentlyBeKilled
      {
        get => this.IsVulnerable && !this.PreventAllDamage && (double) this.minimumHealth <= 0.0;
      }

      public bool PreventCooldownGainFromDamage { get; set; }

      public bool TrackPixelColliderDamage { get; private set; }

      public Dictionary<PixelCollider, float> PixelColliderDamage { get; private set; }

      public void AddTrackedDamagePixelCollider(PixelCollider pixelCollider)
      {
        this.TrackPixelColliderDamage = true;
        if (this.PixelColliderDamage == null)
          this.PixelColliderDamage = new Dictionary<PixelCollider, float>();
        this.PixelColliderDamage.Add(pixelCollider, 0.0f);
      }

      public void Awake()
      {
        StaticReferenceManager.AllHealthHavers.Add(this);
        if (GameManager.Instance.InTutorial)
        {
          if (this.name.StartsWith("BulletMan"))
            this.maximumHealth = 10f;
          if (this.name.StartsWith("BulletShotgunMan"))
            this.maximumHealth = 15f;
        }
        this.currentHealth = this.AdjustedMaxHealth;
        this.RegisterBodySprite(this.sprite);
        if (!this.IsBoss)
          return;
        this.aiActor.SetResistance(EffectResistanceType.Freeze, Mathf.Max(this.aiActor.GetResistanceForEffectType(EffectResistanceType.Freeze), 0.6f));
        this.aiActor.SetResistance(EffectResistanceType.Fire, Mathf.Max(this.aiActor.GetResistanceForEffectType(EffectResistanceType.Fire), 0.25f));
        if (!(bool) (UnityEngine.Object) this.knockbackDoer)
          return;
        this.knockbackDoer.SetImmobile(true, "Like-a-boss");
      }

      private void Start()
      {
        if ((UnityEngine.Object) this.spriteAnimator == (UnityEngine.Object) null)
          this.spriteAnimator = this.GetComponentInChildren<tk2dSpriteAnimator>();
        this.m_player = this.GetComponent<PlayerController>();
        if ((UnityEngine.Object) this.m_player != (UnityEngine.Object) null)
          this.isPlayerCharacter = true;
        GameLevelDefinition loadedLevelDefinition = GameManager.Instance.GetLastLoadedLevelDefinition();
        if (loadedLevelDefinition == null)
          return;
        this.m_damageCap = loadedLevelDefinition.damageCap;
        if (!this.IsBoss || this.IsSubboss || (double) loadedLevelDefinition.bossDpsCap <= 0.0)
          return;
        float num = 1f;
        if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
          num = (float) (((double) GameManager.Instance.COOP_ENEMY_HEALTH_MULTIPLIER + 2.0) / 2.0);
        this.m_bossDpsCap = loadedLevelDefinition.bossDpsCap * num;
      }

      public void Update()
      {
        this.isFirstFrame = false;
        if ((double) this.m_bossDpsCap <= 0.0 || (double) this.m_recentBossDps <= 0.0)
          return;
        this.m_recentBossDps = Mathf.Max(0.0f, this.m_recentBossDps - this.m_bossDpsCap * BraveTime.DeltaTime);
      }

      protected override void OnDestroy()
      {
        StaticReferenceManager.AllHealthHavers.Remove(this);
        this.spriteAnimator.AnimationEventTriggered -= new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.BulletScriptEventTriggered);
        base.OnDestroy();
      }

      public void RegisterBodySprite(
        tk2dBaseSprite sprite,
        bool flashIndependentlyOnDamage = false,
        int flashPixelCollider = 0)
      {
        if (!this.bodySprites.Contains(sprite))
          this.bodySprites.Add(sprite);
        if (!flashIndependentlyOnDamage)
          return;
        if (this.m_independentDamageFlashers == null)
          this.m_independentDamageFlashers = new Dictionary<PixelCollider, tk2dBaseSprite>();
        this.m_independentDamageFlashers.Add(this.specRigidbody.PixelColliders[flashPixelCollider], sprite);
      }

      public void ApplyHealing(float healing)
      {
        if (this.isPlayerCharacter && this.m_player.IsGhost)
          return;
        if (this.ModifyHealing != null)
        {
          HealthHaver.ModifyHealingEventArgs healingEventArgs = new HealthHaver.ModifyHealingEventArgs()
          {
            InitialHealing = healing,
            ModifiedHealing = healing
          };
          this.ModifyHealing(this, healingEventArgs);
          healing = healingEventArgs.ModifiedHealing;
        }
        this.currentHealth += healing;
        if (this.quantizeHealth)
          this.currentHealth = BraveMathCollege.QuantizeFloat(this.currentHealth, this.quantizedIncrement);
        if ((double) this.currentHealth > (double) this.AdjustedMaxHealth)
          this.currentHealth = this.AdjustedMaxHealth;
        if (this.OnHealthChanged == null)
          return;
        this.OnHealthChanged(this.currentHealth, this.AdjustedMaxHealth);
      }

      public void FullHeal()
      {
        this.currentHealth = this.AdjustedMaxHealth;
        if (this.OnHealthChanged == null)
          return;
        this.OnHealthChanged(this.currentHealth, this.AdjustedMaxHealth);
      }

      public void SetHealthMaximum(
        float targetValue,
        float? amountOfHealthToGain = null,
        bool keepHealthPercentage = false)
      {
        if ((double) targetValue == (double) this.maximumHealth)
          return;
        float healthPercentage = this.GetCurrentHealthPercentage();
        if (!keepHealthPercentage)
        {
          if (amountOfHealthToGain.HasValue)
            this.currentHealth += amountOfHealthToGain.Value;
          else if ((double) targetValue > (double) this.maximumHealth)
            this.currentHealth += targetValue - this.maximumHealth;
        }
        this.maximumHealth = targetValue;
        if (keepHealthPercentage)
        {
          this.currentHealth = healthPercentage * this.AdjustedMaxHealth;
          if (amountOfHealthToGain.HasValue)
            this.currentHealth += amountOfHealthToGain.Value;
        }
        this.currentHealth = Mathf.Min(this.currentHealth, this.AdjustedMaxHealth);
        if (this.quantizeHealth)
        {
          this.currentHealth = BraveMathCollege.QuantizeFloat(this.currentHealth, this.quantizedIncrement);
          this.maximumHealth = BraveMathCollege.QuantizeFloat(this.maximumHealth, this.quantizedIncrement);
        }
        if (this.OnHealthChanged == null)
          return;
        this.OnHealthChanged(this.currentHealth, this.AdjustedMaxHealth);
      }

      public void ApplyDamage(
        float damage,
        Vector2 direction,
        string sourceName,
        CoreDamageTypes damageTypes = CoreDamageTypes.None,
        DamageCategory damageCategory = DamageCategory.Normal,
        bool ignoreInvulnerabilityFrames = false,
        PixelCollider hitPixelCollider = null,
        bool ignoreDamageCaps = false)
      {
        this.ApplyDamageDirectional(damage, direction, sourceName, damageTypes, damageCategory, ignoreInvulnerabilityFrames, hitPixelCollider, ignoreDamageCaps);
      }

      public float GetDamageModifierForType(CoreDamageTypes damageTypes)
      {
        float damageModifierForType = 1f;
        for (int index = 0; index < this.damageTypeModifiers.Count; ++index)
        {
          if ((damageTypes & this.damageTypeModifiers[index].damageType) == this.damageTypeModifiers[index].damageType)
            damageModifierForType *= this.damageTypeModifiers[index].damageMultiplier;
        }
        if (this.isPlayerCharacter && (bool) (UnityEngine.Object) this.m_player && (bool) (UnityEngine.Object) this.m_player.CurrentGun && this.m_player.CurrentGun.currentGunDamageTypeModifiers != null)
        {
          for (int index = 0; index < this.m_player.CurrentGun.currentGunDamageTypeModifiers.Length; ++index)
          {
            if ((damageTypes & this.m_player.CurrentGun.currentGunDamageTypeModifiers[index].damageType) == this.m_player.CurrentGun.currentGunDamageTypeModifiers[index].damageType)
              damageModifierForType *= this.m_player.CurrentGun.currentGunDamageTypeModifiers[index].damageMultiplier;
          }
        }
        return damageModifierForType;
      }

      private bool BossHealthSanityCheck(float rawDamage)
      {
        if (GameManager.Instance.PrimaryPlayer.healthHaver.IsDead)
        {
          if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
          {
            if ((!(bool) (UnityEngine.Object) GameManager.Instance.SecondaryPlayer || GameManager.Instance.SecondaryPlayer.healthHaver.IsDead) && (double) this.GetCurrentHealth() <= (double) rawDamage)
              return false;
          }
          else if ((double) this.GetCurrentHealth() <= (double) rawDamage)
            return false;
        }
        return true;
      }

      public List<PixelCollider> DamageableColliders { get; set; }

      protected void ApplyDamageDirectional(
        float damage,
        Vector2 direction,
        string damageSource,
        CoreDamageTypes damageTypes,
        DamageCategory damageCategory = DamageCategory.Normal,
        bool ignoreInvulnerabilityFrames = false,
        PixelCollider hitPixelCollider = null,
        bool ignoreDamageCaps = false)
      {
        if ((double) this.GetCurrentHealth() > (double) this.GetMaxHealth())
        {
          UnityEngine.Debug.Log((object) ("Something went wrong in HealthHaver, but we caught it! " + (object) this.currentHealth));
          this.currentHealth = this.GetMaxHealth();
        }
        if (this.PreventAllDamage && damageCategory == DamageCategory.Unstoppable)
          this.PreventAllDamage = false;
        if (this.PreventAllDamage || (bool) (UnityEngine.Object) this.m_player && this.m_player.IsGhost || hitPixelCollider != null && this.DamageableColliders != null && !this.DamageableColliders.Contains(hitPixelCollider) || this.IsBoss && !this.BossHealthSanityCheck(damage) || this.isFirstFrame)
          return;
        if (ignoreInvulnerabilityFrames)
        {
          if (!this.vulnerable)
            return;
        }
        else if (!this.IsVulnerable)
          return;
        if ((double) damage <= 0.0)
          return;
        damage *= this.GetDamageModifierForType(damageTypes);
        damage *= this.AllDamageMultiplier;
        if (this.OnlyAllowSpecialBossDamage && (damageTypes & CoreDamageTypes.SpecialBossDamage) != CoreDamageTypes.SpecialBossDamage)
          damage = 0.0f;
        if (this.IsBoss && !string.IsNullOrEmpty(damageSource))
        {
          switch (damageSource)
          {
            case "primaryplayer":
              damage *= GameManager.Instance.PrimaryPlayer.stats.GetStatValue(PlayerStats.StatType.DamageToBosses);
              break;
            case "secondaryplayer":
              damage *= GameManager.Instance.SecondaryPlayer.stats.GetStatValue(PlayerStats.StatType.DamageToBosses);
              break;
          }
        }
        if ((bool) (UnityEngine.Object) this.m_player && !ignoreInvulnerabilityFrames)
          damage = Mathf.Min(damage, 0.5f);
        if ((bool) (UnityEngine.Object) this.m_player && damageCategory == DamageCategory.BlackBullet)
          damage = 1f;
        if (this.ModifyDamage != null)
        {
          HealthHaver.ModifyDamageEventArgs modifyDamageEventArgs = new HealthHaver.ModifyDamageEventArgs()
          {
            InitialDamage = damage,
            ModifiedDamage = damage
          };
          this.ModifyDamage(this, modifyDamageEventArgs);
          damage = modifyDamageEventArgs.ModifiedDamage;
        }
        if (!(bool) (UnityEngine.Object) this.m_player && !ignoreInvulnerabilityFrames && (double) damage <= 999.0 && !ignoreDamageCaps)
        {
          if ((double) this.m_damageCap > 0.0)
            damage = Mathf.Min(this.m_damageCap, damage);
          if ((double) this.m_bossDpsCap > 0.0)
          {
            damage = Mathf.Min(damage, this.m_bossDpsCap * 3f - this.m_recentBossDps);
            this.m_recentBossDps += damage;
          }
        }
        if ((double) damage <= 0.0)
          return;
        if (this.NextShotKills)
          damage = 100000f;
        if ((double) damage > 0.0 && this.HasCrest)
          this.HasCrest = false;
        if (this.healthIsNumberOfHits)
          damage = 1f;
        if (!this.NextDamageIgnoresArmor && !this.NextShotKills && (double) this.Armor > 0.0)
        {
          --this.Armor;
          damage = 0.0f;
          if (this.isPlayerCharacter)
            this.m_player.OnLostArmor();
        }
        this.NextDamageIgnoresArmor = false;
        float b = damage;
        if ((double) b > 999.0)
          b = 0.0f;
        float damageDone = Mathf.Min(this.currentHealth, b);
        if (this.TrackPixelColliderDamage)
        {
          if (hitPixelCollider != null)
          {
            float num;
            if (this.PixelColliderDamage.TryGetValue(hitPixelCollider, out num))
              this.PixelColliderDamage[hitPixelCollider] = num + damage;
          }
          else if ((double) damage <= 999.0)
          {
            float num = damage * this.GlobalPixelColliderDamageMultiplier;
            List<PixelCollider> pixelColliderList = new List<PixelCollider>((IEnumerable<PixelCollider>) this.PixelColliderDamage.Keys);
            for (int index = 0; index < pixelColliderList.Count; ++index)
            {
              PixelCollider pixelCollider = pixelColliderList[index];
              Dictionary<PixelCollider, float> pixelColliderDamage;
              PixelCollider key;
              (pixelColliderDamage = this.PixelColliderDamage)[key = pixelCollider] = pixelColliderDamage[key] + num;
            }
          }
        }
        this.currentHealth -= damage;
        if (this.isPlayerCharacter)
          UnityEngine.Debug.Log((object) $"{(object) this.currentHealth}||{(object) damage}");
        if (this.quantizeHealth)
          this.currentHealth = BraveMathCollege.QuantizeFloat(this.currentHealth, this.quantizedIncrement);
        this.currentHealth = Mathf.Clamp(this.currentHealth, this.minimumHealth, this.AdjustedMaxHealth);
        if (!this.isPlayerCharacter)
        {
          for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
            GameManager.Instance.AllPlayers[index].OnAnyEnemyTookAnyDamage(damageDone, (double) this.currentHealth <= 0.0 && (double) this.Armor <= 0.0, this);
          if (!string.IsNullOrEmpty(damageSource))
          {
            switch (damageSource)
            {
              case "primaryplayer":
              case "Player ID 0":
                GameManager.Instance.PrimaryPlayer.OnDidDamage(damage, (double) this.currentHealth <= 0.0 && (double) this.Armor <= 0.0, this);
                break;
              case "secondaryplayer":
              case "Player ID 1":
                GameManager.Instance.SecondaryPlayer.OnDidDamage(damage, (double) this.currentHealth <= 0.0 && (double) this.Armor <= 0.0, this);
                break;
            }
          }
        }
        if (this.flashesOnDamage && (UnityEngine.Object) this.spriteAnimator != (UnityEngine.Object) null && !this.m_isFlashing)
        {
          if (this.m_flashOnHitCoroutine != null)
            this.StopCoroutine(this.m_flashOnHitCoroutine);
          this.m_flashOnHitCoroutine = (Coroutine) null;
          if (this.materialsToFlash == null)
          {
            this.materialsToFlash = new List<Material>();
            this.outlineMaterialsToFlash = new List<Material>();
            this.sourceColors = new List<Color>();
          }
          if ((bool) (UnityEngine.Object) this.gameActor)
          {
            for (int index = 0; index < this.materialsToFlash.Count; ++index)
              this.materialsToFlash[index].SetColor("_OverrideColor", this.gameActor.CurrentOverrideColor);
          }
          if (this.outlineMaterialsToFlash != null)
          {
            for (int index = 0; index < this.outlineMaterialsToFlash.Count; ++index)
            {
              if (index >= this.sourceColors.Count)
              {
                UnityEngine.Debug.LogError((object) "NOT ENOUGH SOURCE COLORS");
                break;
              }
              this.outlineMaterialsToFlash[index].SetColor("_OverrideColor", this.sourceColors[index]);
            }
          }
          this.m_flashOnHitCoroutine = this.StartCoroutine(this.FlashOnHit(damageCategory, hitPixelCollider));
        }
        if (this.incorporealityOnDamage && !this.m_isIncorporeal)
          this.StartCoroutine("IncorporealityOnHit");
        this.lastIncurredDamageSource = damageSource;
        this.lastIncurredDamageDirection = direction;
        if (this.shakesCameraOnDamage)
          GameManager.Instance.MainCameraController.DoScreenShake(this.cameraShakeOnDamage, new Vector2?(this.specRigidbody.UnitCenter));
        if (this.NextShotKills)
          this.Armor = 0.0f;
        if (this.OnDamaged != null)
          this.OnDamaged(this.currentHealth, this.AdjustedMaxHealth, damageTypes, damageCategory, direction);
        if (this.OnHealthChanged != null)
          this.OnHealthChanged(this.currentHealth, this.AdjustedMaxHealth);
        if ((double) this.currentHealth == 0.0 && (double) this.Armor == 0.0)
        {
          this.NextShotKills = false;
          if (!this.SuppressDeathSounds)
          {
            int num1 = (int) AkSoundEngine.PostEvent("Play_ENM_death", this.gameObject);
            int num2 = (int) AkSoundEngine.PostEvent(string.IsNullOrEmpty(this.overrideDeathAudioEvent) ? "Play_CHR_general_death_01" : this.overrideDeathAudioEvent, this.gameObject);
          }
          this.Die(direction);
        }
        else if (this.usesInvulnerabilityPeriod)
          this.StartCoroutine(this.HandleInvulnerablePeriod());
        if (damageCategory != DamageCategory.Normal && damageCategory != DamageCategory.Collision)
          return;
        if ((double) this.currentHealth <= 0.0 && (double) this.Armor <= 0.0)
        {
          if (this.DisableStickyFriction)
            return;
          StickyFrictionManager.Instance.RegisterDeathStickyFriction();
        }
        else if (this.isPlayerCharacter)
          StickyFrictionManager.Instance.RegisterPlayerDamageStickyFriction(damage);
        else
          StickyFrictionManager.Instance.RegisterOtherDamageStickyFriction(damage);
      }

      public void Die(Vector2 finalDamageDirection)
      {
        this.EndFlashEffects();
        bool flag = false;
        if (this.spawnBulletScript && (!(bool) (UnityEngine.Object) this.gameActor || !this.gameActor.IsFalling) && ((double) this.chanceToSpawnBulletScript >= 1.0 || (double) UnityEngine.Random.value < (double) this.chanceToSpawnBulletScript))
        {
          flag = true;
          if (this.noCorpseWhenBulletScriptDeath)
            this.aiActor.CorpseObject = (GameObject) null;
          if (this.bulletScriptType == HealthHaver.BulletScriptType.OnAnimEvent)
            this.spriteAnimator.AnimationEventTriggered += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.BulletScriptEventTriggered);
        }
        if (this.OnPreDeath != null)
          this.OnPreDeath(finalDamageDirection);
        if (flag && this.bulletScriptType == HealthHaver.BulletScriptType.OnPreDeath)
          SpawnManager.SpawnBulletScript((GameActor) this.aiActor, this.bulletScript);
        if ((double) this.GetCurrentHealth() > 0.0 || (double) this.Armor > 0.0)
          return;
        this.IsVulnerable = false;
        if ((UnityEngine.Object) this.deathEffect != (UnityEngine.Object) null)
          SpawnManager.SpawnVFX(this.deathEffect, this.transform.position, Quaternion.identity);
        if (this.IsBoss)
          this.EndBossState(true);
        if (this.ManualDeathHandling)
          return;
        if ((UnityEngine.Object) this.spriteAnimator != (UnityEngine.Object) null)
        {
          string name = !flag || string.IsNullOrEmpty(this.overrideDeathAnimBulletScript) ? this.overrideDeathAnimation : this.overrideDeathAnimBulletScript;
          if (!string.IsNullOrEmpty(name))
          {
            tk2dSpriteAnimationClip clip;
            if ((UnityEngine.Object) this.aiAnimator != (UnityEngine.Object) null)
            {
              this.aiAnimator.PlayUntilCancelled(name);
              clip = this.spriteAnimator.CurrentClip;
            }
            else
            {
              clip = this.spriteAnimator.GetClipByName(this.overrideDeathAnimation);
              if (clip != null)
                this.spriteAnimator.Play(clip);
            }
            if (clip != null && !this.isPlayerCharacter && (!(bool) (UnityEngine.Object) this.gameActor || !this.gameActor.IsFalling))
            {
              this.spriteAnimator.AnimationEventTriggered += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.DeathEventTriggered);
              this.spriteAnimator.AnimationCompleted += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.DeathAnimationComplete);
              return;
            }
          }
          else
          {
            if ((UnityEngine.Object) this.aiAnimator != (UnityEngine.Object) null)
              this.aiAnimator.enabled = false;
            float angle = finalDamageDirection.ToAngle();
            tk2dSpriteAnimationClip clip;
            if ((UnityEngine.Object) this.aiAnimator != (UnityEngine.Object) null && this.aiAnimator.HasDirectionalAnimation("death"))
            {
              if (!this.aiAnimator.LockFacingDirection)
              {
                this.aiAnimator.LockFacingDirection = true;
                this.aiAnimator.FacingDirection = (float) (((double) angle + 180.0) % 360.0);
              }
              this.aiAnimator.PlayUntilCancelled("death");
              clip = this.spriteAnimator.CurrentClip;
            }
            else if ((bool) (UnityEngine.Object) this.gameActor && this.gameActor is PlayerSpaceshipController)
            {
              Exploder.DoDefaultExplosion((Vector3) this.gameActor.CenterPosition, Vector2.zero);
              clip = (tk2dSpriteAnimationClip) null;
            }
            else
            {
              clip = this.GetDeathClip(BraveMathCollege.ClampAngle360(angle + 22.5f));
              if (clip != null)
                this.spriteAnimator.Play(clip);
            }
            if (clip != null && !this.isPlayerCharacter && (!(bool) (UnityEngine.Object) this.gameActor || !this.gameActor.IsFalling))
            {
              this.spriteAnimator.AnimationEventTriggered += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.DeathEventTriggered);
              this.spriteAnimator.AnimationCompleted += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.DeathAnimationComplete);
              return;
            }
          }
        }
        if (this.spawnBulletScript && this.bulletScriptType == HealthHaver.BulletScriptType.OnDeath && (!(bool) (UnityEngine.Object) this.gameActor || !this.gameActor.IsFalling))
          SpawnManager.SpawnBulletScript((GameActor) this.aiActor, this.bulletScript);
        this.FinalizeDeath();
      }

      public void EndBossState(bool triggerKillCam)
      {
        bool flag = false;
        List<AIActor> activeEnemies = this.aiActor.ParentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
        for (int index = 0; index < activeEnemies.Count; ++index)
        {
          HealthHaver healthHaver = activeEnemies[index].healthHaver;
          if ((bool) (UnityEngine.Object) healthHaver && healthHaver.IsBoss && healthHaver.IsAlive && (UnityEngine.Object) activeEnemies[index] != (UnityEngine.Object) this.aiActor)
          {
            flag = true;
            break;
          }
        }
        if (this.HasHealthBar)
          (!this.healthHaver.UsesVerticalBossBar ? (this.healthHaver.UsesSecondaryBossBar ? GameUIRoot.Instance.bossController2 : GameUIRoot.Instance.bossController) : GameUIRoot.Instance.bossControllerSide).DeregisterBossHealthHaver(this);
        if (flag)
          return;
        if (triggerKillCam)
          GameUIRoot.Instance.TriggerBossKillCam((Projectile) null, this.specRigidbody);
        if (this.HasHealthBar)
        {
          GameUIRoot.Instance.bossController.DisableBossHealth();
          GameUIRoot.Instance.bossController2.DisableBossHealth();
          GameUIRoot.Instance.bossControllerSide.DisableBossHealth();
        }
        if (triggerKillCam)
        {
          if (this.forcePreventVictoryMusic)
            return;
          GameManager.Instance.DungeonMusicController.EndBossMusic();
        }
        else
          GameManager.Instance.DungeonMusicController.EndBossMusicNoVictory();
      }

      public tk2dSpriteAnimationClip GetDeathClip(float damageAngle)
      {
        if (!(bool) (UnityEngine.Object) this.spriteAnimator)
          return (tk2dSpriteAnimationClip) null;
        int index = Mathf.Max(0, Mathf.Min(Mathf.FloorToInt(BraveMathCollege.ClampAngle360(damageAngle) / 45f), 7));
        tk2dSpriteAnimationClip deathClip = ((this.spriteAnimator.GetClipByName(new string[8]
        {
          "die_right",
          "die_back_right",
          "die_back",
          "die_back_left",
          "die_left",
          "die_front_left",
          "die_front",
          "die_front_right"
        }[index]) ?? (index == 7 || index == 0 || index == 1 || index == 2 ? this.spriteAnimator.GetClipByName("die_right") : this.spriteAnimator.GetClipByName("die_left"))) ?? this.spriteAnimator.GetClipByName("death")) ?? this.spriteAnimator.GetClipByName("die");
        if (this.isPlayerCharacter && this.m_player.hasArmorlessAnimations && (double) this.m_player.healthHaver.Armor == 0.0)
          deathClip = this.spriteAnimator.GetClipByName("death_armorless");
        return deathClip;
      }

      public void EndFlashEffects()
      {
        if (this.m_flashOnHitCoroutine != null)
          this.StopCoroutine(this.m_flashOnHitCoroutine);
        this.m_flashOnHitCoroutine = (Coroutine) null;
        this.EndFlashOnHit();
        this.StopCoroutine("IncorporealityOnHit");
        this.EndIncorporealityOnHit();
      }

      private void BulletScriptEventTriggered(
        tk2dSpriteAnimator sprite,
        tk2dSpriteAnimationClip clip,
        int frameNum)
      {
        if (!(clip.GetFrame(frameNum).eventInfo == "fire"))
          return;
        SpawnManager.SpawnBulletScript((GameActor) this.aiActor, this.bulletScript);
      }

      public void UpdateCachedOutlineColor(Material m, Color c)
      {
        if (this.outlineMaterialsToFlash == null || !this.outlineMaterialsToFlash.Contains(m))
          return;
        int index = this.outlineMaterialsToFlash.IndexOf(m);
        if (this.sourceColors == null || this.sourceColors.Count <= index || index < 0)
          return;
        this.sourceColors[index] = c;
      }

      [DebuggerHidden]
      private IEnumerator FlashOnHit(
        DamageCategory sourceDamageCategory,
        PixelCollider hitPixelCollider)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new HealthHaver.<FlashOnHit>c__Iterator0()
        {
          sourceDamageCategory = sourceDamageCategory,
          hitPixelCollider = hitPixelCollider,
          _this = this
        };
      }

      private void EndFlashOnHit()
      {
        if (this.m_flashOnHitCoroutine != null)
          this.StopCoroutine(this.m_flashOnHitCoroutine);
        this.m_flashOnHitCoroutine = (Coroutine) null;
        if ((bool) (UnityEngine.Object) this.gameActor)
          this.gameActor.OverrideColorOverridden = false;
        if (this.materialsToFlash != null && this.materialsToFlash.Count > 0 && (bool) (UnityEngine.Object) this.gameActor)
        {
          for (int index = 0; index < this.materialsToFlash.Count; ++index)
            this.materialsToFlash[index].SetColor("_OverrideColor", this.gameActor.CurrentOverrideColor);
        }
        if (this.outlineMaterialsToFlash != null && this.outlineMaterialsToFlash.Count > 0)
        {
          for (int index = 0; index < this.outlineMaterialsToFlash.Count; ++index)
            this.outlineMaterialsToFlash[index].SetColor("_OverrideColor", this.sourceColors[index]);
        }
        if (this.materialsToEnableBrightnessClampOn != null && this.materialsToEnableBrightnessClampOn.Count > 0)
        {
          for (int index = 0; index < this.materialsToEnableBrightnessClampOn.Count; ++index)
          {
            this.materialsToEnableBrightnessClampOn[index].DisableKeyword("BRIGHTNESS_CLAMP_OFF");
            this.materialsToEnableBrightnessClampOn[index].EnableKeyword("BRIGHTNESS_CLAMP_ON");
          }
        }
        this.m_isFlashing = false;
      }

      [DebuggerHidden]
      private IEnumerator IncorporealityOnHit()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new HealthHaver.<IncorporealityOnHit>c__Iterator1()
        {
          _this = this
        };
      }

      private void EndIncorporealityOnHit()
      {
        PlayerController component = this.GetComponent<PlayerController>();
        if ((UnityEngine.Object) component == (UnityEngine.Object) null)
          return;
        int mask = CollisionMask.LayerToMask(CollisionLayer.EnemyCollider, CollisionLayer.EnemyHitBox, CollisionLayer.Projectile);
        component.IsVisible = true;
        component.specRigidbody.RemoveCollisionLayerIgnoreOverride(mask);
        this.m_isIncorporeal = false;
      }

      private void DeathEventTriggered(
        tk2dSpriteAnimator animator,
        tk2dSpriteAnimationClip clip,
        int frameNo)
      {
        if (!(clip.GetFrame(frameNo).eventInfo == "disableColliders"))
          return;
        for (int index = 0; index < this.specRigidbody.PixelColliders.Count; ++index)
          this.specRigidbody.PixelColliders[index].Enabled = false;
      }

      public void DeathAnimationComplete(
        tk2dSpriteAnimator spriteAnimator,
        tk2dSpriteAnimationClip clip)
      {
        this.FinalizeDeath();
      }

      private void FinalizeDeath()
      {
        if (this.OnDeath != null)
          this.OnDeath(this.lastIncurredDamageDirection);
        if ((bool) (UnityEngine.Object) this.aiActor && this.aiActor.IsFalling && !this.aiActor.HasSplashed)
        {
          GameManager.Instance.Dungeon.tileIndices.DoSplashAtPosition(this.sprite.WorldCenter);
          this.aiActor.HasSplashed = true;
        }
        if (GameManager.Instance.InTutorial && !this.isPlayerCharacter)
          GameManager.BroadcastRoomTalkDoerFsmEvent("enemyKilled");
        if (this.persistsOnDeath)
          return;
        UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
      }

      public void TriggerInvulnerabilityPeriod(float overrideTime = -1f)
      {
        if (!this.usesInvulnerabilityPeriod)
          return;
        this.StartCoroutine(this.HandleInvulnerablePeriod(overrideTime));
      }

      [DebuggerHidden]
      protected IEnumerator HandleInvulnerablePeriod(float overrideTime = -1f)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new HealthHaver.<HandleInvulnerablePeriod>c__Iterator2()
        {
          overrideTime = overrideTime,
          _this = this
        };
      }

      public void ApplyDamageModifiers(List<DamageTypeModifier> newDamageTypeModifiers)
      {
        for (int index1 = 0; index1 < newDamageTypeModifiers.Count; ++index1)
        {
          DamageTypeModifier damageTypeModifier1 = newDamageTypeModifiers[index1];
          bool flag = false;
          for (int index2 = 0; index2 < this.damageTypeModifiers.Count; ++index2)
          {
            DamageTypeModifier damageTypeModifier2 = this.damageTypeModifiers[index2];
            if (damageTypeModifier1.damageType == damageTypeModifier2.damageType)
            {
              damageTypeModifier2.damageMultiplier = damageTypeModifier1.damageMultiplier;
              flag = true;
              break;
            }
          }
          if (!flag)
            this.damageTypeModifiers.Add(new DamageTypeModifier(damageTypeModifier1));
        }
      }

      public int NumBodyRigidbodies
      {
        get
        {
          if (this.bodyRigidbodies != null)
            return this.bodyRigidbodies.Count;
          return (bool) (UnityEngine.Object) this.specRigidbody ? 1 : 0;
        }
      }

      public SpeculativeRigidbody GetBodyRigidbody(int index)
      {
        return this.bodyRigidbodies != null ? this.bodyRigidbodies[index] : this.specRigidbody;
      }

      public class ModifyDamageEventArgs : EventArgs
      {
        public float InitialDamage;
        public float ModifiedDamage;
      }

      public class ModifyHealingEventArgs : EventArgs
      {
        public float InitialHealing;
        public float ModifiedHealing;
      }

      public enum BossBarType
      {
        None,
        MainBar,
        SecondaryBar,
        CombinedBar,
        SecretBar,
        VerticalBar,
        SubbossBar,
      }

      public delegate void OnDamagedEvent(
        float resultValue,
        float maxValue,
        CoreDamageTypes damageTypes,
        DamageCategory damageCategory,
        Vector2 damageDirection);

      public delegate void OnHealthChangedEvent(float resultValue, float maxValue);

      public enum BulletScriptType
      {
        OnPreDeath,
        OnDeath,
        OnAnimEvent,
      }
    }

}
