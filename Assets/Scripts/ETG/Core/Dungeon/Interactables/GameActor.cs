// Decompiled with JetBrains decompiler
// Type: GameActor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Dungeon.Interactables
{
    public abstract class GameActor : DungeonPlaceableBehaviour, IAutoAimTarget
    {
      [Header("Actor Shared Properties")]
      public string ActorName;
      public string OverrideDisplayName;
      [EnumFlags]
      public CoreActorTypes actorTypes;
      [Space(3f)]
      public bool HasShadow = true;
      [ShowInInspectorIf("HasShadow", true)]
      public GameObject ShadowPrefab;
      [ShowInInspectorIf("HasShadow", true)]
      public GameObject ShadowObject;
      [ShowInInspectorIf("HasShadow", true)]
      public float ShadowHeightOffGround;
      [ShowInInspectorIf("HasShadow", true)]
      public Transform ShadowParent;
      [ShowInInspectorIf("HasShadow", true)]
      public Vector3 ActorShadowOffset;
      [Space(3f)]
      public bool DoDustUps;
      [ShowInInspectorIf("DoDustUps", true)]
      public float DustUpInterval;
      [ShowInInspectorIf("DoDustUps", true)]
      public GameObject OverrideDustUp;
      [Space(3f)]
      public float FreezeDispelFactor = 20f;
      public bool ImmuneToAllEffects;
      public ActorEffectResistance[] EffectResistances;
      public const float OUTLINE_DEPTH = 0.1f;
      public const float GUN_DEPTH = 0.075f;
      public const float ACTOR_VFX_DEPTH = 0.2f;
      public const float BACKFACING_ANGLE_MAX = 155f;
      public const float BACKFACING_ANGLE_MIN = 25f;
      public const float BACKWARDS_ANGLE_MAX = 120f;
      public const float BACKWARDS_ANGLE_MIN = 60f;
      public const float FORWARDS_ANGLE_MAX = -60f;
      public const float FORWARDS_ANGLE_MIN = -120f;
      public const float FLIP_LEFT_THRESHOLD_FRONT = 105f;
      public const float FLIP_RIGHT_THRESHOLD_FRONT = 75f;
      public const float FLIP_LEFT_THRESHOLD_BACK = 105f;
      public const float FLIP_RIGHT_THRESHOLD_BACK = 75f;
      [NonSerialized]
      public bool FallingProhibited;
      private GameObject m_stealthVfx;
      [NonSerialized]
      public float actorReflectionAdditionalOffset;
      protected GoopDefinition m_currentGoop;
      protected bool m_currentGoopFrozen;
      [NonSerialized]
      public Vector2 ImpartedVelocity;
      protected int m_overrideColorID;
      protected int m_overrideFlatColorID;
      protected int m_specialFlagsID;
      protected int m_stencilID;
      [NonSerialized]
      public bool IsOverPitAtAll;
      public Func<bool, bool> OnAboutToFall;
      public bool OverrideColorOverridden;
      private List<string> m_overrideColorSources = new List<string>();
      private List<Color> m_overrideColorStack = new List<Color>();
      protected Material m_colorOverridenMaterial;
      protected Shader m_colorOverridenShader;
      [NonSerialized]
      public float BeamStatusAmount;
      protected Vector2 m_cachedPosition;
      protected bool m_isFalling;
      protected OverridableBool m_isFlying = new OverridableBool(false);
      protected OverridableBool m_isStealthed = new OverridableBool(false);
      protected float m_dustUpTimer;
      protected List<MovingPlatform> m_supportingPlatforms = new List<MovingPlatform>();
      protected List<GameActorEffect> m_activeEffects = new List<GameActorEffect>();
      protected List<RuntimeGameActorEffectData> m_activeEffectData = new List<RuntimeGameActorEffectData>();

      public abstract Gun CurrentGun { get; }

      public abstract Transform GunPivot { get; }

      public virtual Transform SecondaryGunPivot => this.GunPivot;

      public abstract bool SpriteFlipped { get; }

      public abstract Vector3 SpriteDimensions { get; }

      public List<MovingPlatform> SupportingPlatforms => this.m_supportingPlatforms;

      public Vector2 CenterPosition
      {
        get
        {
          if ((bool) (UnityEngine.Object) this.specRigidbody && this.specRigidbody.HitboxPixelCollider != null)
            return this.specRigidbody.HitboxPixelCollider.UnitCenter;
          return (bool) (UnityEngine.Object) this.sprite ? this.sprite.WorldCenter : this.transform.position.XY();
        }
      }

      public bool IsFalling => this.m_isFalling;

      public virtual bool IsFlying => this.m_isFlying.Value;

      public bool IsGrounded
      {
        get => this.spriteAnimator.QueryGroundedFrame() && !this.IsFlying && !this.FallingProhibited;
      }

      public bool IsStealthed => this.m_isStealthed.Value;

      public float GetResistanceForEffectType(EffectResistanceType resistType)
      {
        if (resistType == EffectResistanceType.None)
          return 0.0f;
        for (int index = 0; index < this.EffectResistances.Length; ++index)
        {
          if (this.EffectResistances[index].resistType == resistType)
            return this.EffectResistances[index].resistAmount;
        }
        return 0.0f;
      }

      public void SetIsStealthed(bool value, string reason)
      {
        bool isStealthed = this.IsStealthed;
        this.m_isStealthed.SetOverride(reason, value);
        if (this.IsStealthed == isStealthed)
          return;
        if (this.IsStealthed)
        {
          this.m_stealthVfx = this.PlayEffectOnActor(BraveResources.Load<GameObject>("Global VFX/VFX_Stealthed"), new Vector3(0.0f, 1.375f, 0.0f), alreadyMiddleCenter: true);
        }
        else
        {
          if (!(bool) (UnityEngine.Object) this.m_stealthVfx)
            return;
          UnityEngine.Object.Destroy((UnityEngine.Object) this.m_stealthVfx);
        }
      }

      public void SetIsFlying(bool value, string reason, bool adjustShadow = true, bool modifyPathing = false)
      {
        this.m_isFlying.SetOverride(reason, value);
        if (adjustShadow && this.HasShadow && (bool) (UnityEngine.Object) this.ShadowObject)
        {
          if (value)
            this.ShadowObject.transform.position += new Vector3(0.0f, -0.3f, 0.0f);
          else
            this.ShadowObject.transform.position += new Vector3(0.0f, 0.3f, 0.0f);
        }
        this.specRigidbody.CanBeCarried = !this.m_isFlying.Value;
        AIActor aiActor = this as AIActor;
        if (!modifyPathing || !(bool) (UnityEngine.Object) aiActor)
          return;
        if (value)
          aiActor.PathableTiles |= CellTypes.PIT;
        else
          aiActor.PathableTiles &= ~CellTypes.PIT;
      }

      protected virtual float DustUpMultiplier => 1f;

      public GoopDefinition CurrentGoop => this.m_currentGoop;

      public float FreezeAmount { get; set; }

      public float CheeseAmount { get; set; }

      public event GameActor.MovementModifier MovementModifiers;

      public bool StealthDeath { get; set; }

      public bool IsFrozen { get; set; }

      public bool IsCheezen { get; set; }

      public bool IsGone { get; set; }

      public bool SuppressEffectUpdates { get; set; }

      public float FacingDirection
      {
        get
        {
          if ((bool) (UnityEngine.Object) this.aiAnimator)
            return this.aiAnimator.FacingDirection;
          if (!(this.gameActor is PlayerController))
            return -90f;
          PlayerController gameActor = this.gameActor as PlayerController;
          return (gameActor.unadjustedAimPoint.XY() - gameActor.specRigidbody.GetUnitCenter(ColliderType.HitBox)).ToAngle();
        }
      }

      public bool PreventAutoAimVelocity { get; set; }

      public virtual void Awake()
      {
        this.m_overrideColorID = Shader.PropertyToID("_OverrideColor");
        this.RegisterOverrideColor(new Color(1f, 1f, 1f, 0.0f), "base");
      }

      public virtual void Start()
      {
        if (!(bool) (UnityEngine.Object) this.specRigidbody)
          return;
        this.specRigidbody.Initialize();
      }

      public virtual void Update()
      {
        this.BeamStatusAmount = Mathf.Max(0.0f, this.BeamStatusAmount - BraveTime.DeltaTime / 2f);
        if (!this.SuppressEffectUpdates)
        {
          for (int index = 0; index < this.m_activeEffects.Count; ++index)
          {
            GameActorEffect activeEffect = this.m_activeEffects[index];
            if (activeEffect != null && this.m_activeEffectData != null && index < this.m_activeEffectData.Count)
            {
              RuntimeGameActorEffectData effectData = this.m_activeEffectData[index];
              if (effectData != null)
              {
                activeEffect.EffectTick(this, effectData);
                if ((UnityEngine.Object) effectData.instanceOverheadVFX != (UnityEngine.Object) null)
                {
                  if ((bool) (UnityEngine.Object) this.healthHaver && this.healthHaver.IsAlive && (bool) (UnityEngine.Object) effectData.instanceOverheadVFX)
                  {
                    Vector2 vector2 = this.transform.position.XY();
                    if (activeEffect.PlaysVFXOnActor)
                    {
                      if ((bool) (UnityEngine.Object) this.specRigidbody && this.specRigidbody.HitboxPixelCollider != null)
                        vector2 = this.specRigidbody.HitboxPixelCollider.UnitBottomCenter.Quantize(1f / 16f);
                      effectData.instanceOverheadVFX.transform.position = (Vector3) vector2;
                    }
                    else
                    {
                      if ((bool) (UnityEngine.Object) this.specRigidbody && this.specRigidbody.HitboxPixelCollider != null)
                        vector2 = this.specRigidbody.HitboxPixelCollider.UnitTopCenter.Quantize(1f / 16f);
                      effectData.instanceOverheadVFX.transform.position = (Vector3) vector2;
                    }
                    effectData.instanceOverheadVFX.renderer.enabled = !this.IsGone;
                  }
                  else if ((bool) (UnityEngine.Object) effectData.instanceOverheadVFX)
                    UnityEngine.Object.Destroy((UnityEngine.Object) effectData.instanceOverheadVFX.gameObject);
                }
                float num = 1f;
                if (activeEffect is GameActorCharmEffect && PassiveItem.IsFlagSetAtAll(typeof (BattleStandardItem)))
                  num /= BattleStandardItem.BattleStandardCharmDurationMultiplier;
                effectData.elapsed += BraveTime.DeltaTime * num;
                effectData.tickCounter += BraveTime.DeltaTime;
                if (activeEffect.IsFinished(this, effectData, effectData.elapsed))
                  this.RemoveEffect(activeEffect);
              }
            }
          }
        }
        if (this.DoDustUps && !GameManager.Instance.IsLoadingLevel && (bool) (UnityEngine.Object) this.specRigidbody)
        {
          bool flag1 = (double) this.specRigidbody.Velocity.magnitude > 0.0 && !this.m_isFalling && !this.IsFlying;
          bool flag2 = false;
          Vector2 unitBottomCenter = this.specRigidbody.PrimaryPixelCollider.UnitBottomCenter;
          IntVector2 intVector2 = unitBottomCenter.ToIntVector2(VectorConversions.Floor);
          if (GameManager.Instance.Dungeon.data.CheckInBoundsAndValid(intVector2))
          {
            CellVisualData.CellFloorType cellFloorType = GameManager.Instance.Dungeon.data[intVector2].cellVisualData.floorType;
            if (flag1 && this is PlayerController)
              flag1 = ((flag1 ? 1 : 0) & (this.spriteAnimator.QueryGroundedFrame() ? 1 : (this.IsFlying ? 1 : 0))) != 0 & !GameManager.Instance.Dungeon.CellIsPit((Vector3) this.specRigidbody.UnitCenter) & ((PlayerController) this).IsVisible & cellFloorType != CellVisualData.CellFloorType.Ice;
            PlayerController playerController = this as PlayerController;
            if ((bool) (UnityEngine.Object) playerController && playerController.IsGhost)
            {
              flag1 = true;
              flag2 = true;
            }
            else if ((bool) (UnityEngine.Object) playerController && playerController.IsSlidingOverSurface)
              flag1 = false;
            if (flag1)
            {
              this.m_dustUpTimer += BraveTime.DeltaTime;
              if ((double) this.m_dustUpTimer >= (double) this.DustUpInterval / (double) this.DustUpMultiplier)
              {
                if ((bool) (UnityEngine.Object) this.OverrideDustUp)
                {
                  SpawnManager.SpawnVFX(this.OverrideDustUp, (Vector3) unitBottomCenter, Quaternion.identity);
                  this.m_dustUpTimer = 0.0f;
                }
                else if (flag2)
                {
                  SpawnManager.SpawnVFX(ResourceCache.Acquire("Global VFX/GhostDustUp") as GameObject, (Vector3) unitBottomCenter, Quaternion.identity);
                  this.m_dustUpTimer = 0.0f;
                }
                else
                {
                  SharedDungeonSettings sharedSettingsPrefab = GameManager.Instance.Dungeon.sharedSettingsPrefab;
                  DustUpVFX dungeonDustups = GameManager.Instance.Dungeon.dungeonDustups;
                  Color color = Color.clear;
                  bool flag3 = false;
                  bool flag4 = false;
                  if ((UnityEngine.Object) this.m_currentGoop != (UnityEngine.Object) null)
                  {
                    if (this.m_currentGoopFrozen)
                    {
                      cellFloorType = CellVisualData.CellFloorType.Ice;
                    }
                    else
                    {
                      cellFloorType = !this.m_currentGoop.usesWaterVfx ? CellVisualData.CellFloorType.ThickGoop : CellVisualData.CellFloorType.Water;
                      flag3 = this.m_currentGoop.AppliesCheese;
                      flag4 = this.m_currentGoop.AppliesSpeedModifierContinuously && this.m_currentGoop.playerStepsChangeLifetime && this.m_currentGoop.SpeedModifierEffect.effectIdentifier.StartsWith("phase web", StringComparison.Ordinal);
                    }
                    color = (Color) this.m_currentGoop.baseColor32;
                  }
                  if (cellFloorType == CellVisualData.CellFloorType.Water && (UnityEngine.Object) dungeonDustups.waterDustup != (UnityEngine.Object) null)
                  {
                    GameObject gameObject = SpawnManager.SpawnVFX(dungeonDustups.waterDustup, (Vector3) unitBottomCenter, Quaternion.identity);
                    if ((bool) (UnityEngine.Object) gameObject)
                    {
                      Renderer component = gameObject.GetComponent<Renderer>();
                      if ((bool) (UnityEngine.Object) component)
                      {
                        gameObject.GetComponent<tk2dBaseSprite>().OverrideMaterialMode = tk2dBaseSprite.SpriteMaterialOverrideMode.OVERRIDE_MATERIAL_SIMPLE;
                        component.material.SetColor(this.m_overrideColorID, color);
                      }
                    }
                    if ((UnityEngine.Object) dungeonDustups.additionalWaterDustup != (UnityEngine.Object) null)
                      SpawnManager.SpawnVFX(dungeonDustups.additionalWaterDustup, (Vector3) unitBottomCenter, Quaternion.identity, true);
                  }
                  else
                  {
                    switch (cellFloorType)
                    {
                      case CellVisualData.CellFloorType.Ice:
                        break;
                      case CellVisualData.CellFloorType.ThickGoop:
                        if (flag3)
                        {
                          if ((UnityEngine.Object) sharedSettingsPrefab.additionalCheeseDustup != (UnityEngine.Object) null)
                          {
                            SpawnManager.SpawnVFX(sharedSettingsPrefab.additionalCheeseDustup, (Vector3) (unitBottomCenter + new Vector2(1f / 16f, -0.25f)), Quaternion.identity, true);
                            break;
                          }
                          break;
                        }
                        if (flag4 && (UnityEngine.Object) sharedSettingsPrefab.additionalWebDustup != (UnityEngine.Object) null)
                        {
                          SpawnManager.SpawnVFX(sharedSettingsPrefab.additionalWebDustup, (Vector3) (unitBottomCenter + new Vector2(1f / 16f, -0.25f)), Quaternion.identity, true);
                          break;
                        }
                        break;
                      default:
                        SpawnManager.SpawnVFX(dungeonDustups.runDustup, (Vector3) unitBottomCenter, Quaternion.identity);
                        break;
                    }
                  }
                  this.m_dustUpTimer = 0.0f;
                  if (flag4)
                    this.m_dustUpTimer = -this.DustUpInterval / this.DustUpMultiplier;
                }
              }
            }
            else if ((bool) (UnityEngine.Object) playerController && playerController.IsSlidingOverSurface)
            {
              this.m_dustUpTimer += BraveTime.DeltaTime;
              if ((double) this.m_dustUpTimer >= (double) this.DustUpInterval / (double) this.DustUpMultiplier)
              {
                DustUpVFX dungeonDustups = GameManager.Instance.Dungeon.dungeonDustups;
                GameObject gameObject = SpawnManager.SpawnVFX(GameManager.Instance.Dungeon.sharedSettingsPrefab.additionalTableDustup, (Vector3) (unitBottomCenter + new Vector2(1f / 16f, 0.25f)), Quaternion.identity);
                if ((bool) (UnityEngine.Object) gameObject)
                {
                  tk2dBaseSprite component = gameObject.GetComponent<tk2dBaseSprite>();
                  if ((bool) (UnityEngine.Object) component)
                  {
                    component.HeightOffGround = 0.0f;
                    component.UpdateZDepth();
                  }
                }
                this.m_dustUpTimer = 0.0f;
              }
            }
          }
        }
        if (GameManager.Instance.IsLoadingLevel)
          return;
        this.HandleGoopChecks();
      }

      public void ApplyEffect(
        GameActorEffect effect,
        float sourcePartialAmount = 1f,
        Projectile sourceProjectile = null)
      {
        if (this.ImmuneToAllEffects || !effect.AffectsPlayers && this is PlayerController || !effect.AffectsEnemies && this is AIActor)
          return;
        float num = sourcePartialAmount;
        EffectResistanceType resistType = effect.resistanceType;
        if (resistType == EffectResistanceType.None)
        {
          if (effect.effectIdentifier == "poison")
            resistType = EffectResistanceType.Poison;
          if (effect.effectIdentifier == "fire")
            resistType = EffectResistanceType.Fire;
          if (effect.effectIdentifier == "freeze")
            resistType = EffectResistanceType.Freeze;
          if (effect.effectIdentifier == "charm")
            resistType = EffectResistanceType.Charm;
        }
        float partialAmount = num * (1f - this.GetResistanceForEffectType(resistType));
        if ((double) partialAmount == 0.0 || effect is GameActorCharmEffect && (UnityEngine.Object) this.healthHaver != (UnityEngine.Object) null && this.healthHaver.IsBoss)
          return;
        for (int index = 0; index < this.m_activeEffects.Count; ++index)
        {
          if (this.m_activeEffects[index].effectIdentifier == effect.effectIdentifier)
          {
            switch (effect.stackMode)
            {
              case GameActorEffect.EffectStackingMode.Refresh:
                this.m_activeEffectData[index].elapsed = 0.0f;
                return;
              case GameActorEffect.EffectStackingMode.Stack:
                this.m_activeEffectData[index].elapsed -= effect.duration;
                if ((double) effect.maxStackedDuration <= 0.0)
                  return;
                this.m_activeEffectData[index].elapsed = Mathf.Max(effect.duration - effect.maxStackedDuration, this.m_activeEffectData[index].elapsed);
                return;
              case GameActorEffect.EffectStackingMode.Ignore:
                return;
              case GameActorEffect.EffectStackingMode.DarkSoulsAccumulate:
                effect.OnDarkSoulsAccumulate(this, this.m_activeEffectData[index], partialAmount, sourceProjectile);
                return;
              default:
                return;
            }
          }
        }
        RuntimeGameActorEffectData gameActorEffectData = new RuntimeGameActorEffectData();
        gameActorEffectData.actor = this;
        effect.ApplyTint(this);
        if ((UnityEngine.Object) effect.OverheadVFX != (UnityEngine.Object) null)
        {
          GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(effect.OverheadVFX);
          tk2dBaseSprite component = gameObject.GetComponent<tk2dBaseSprite>();
          gameObject.transform.parent = this.transform;
          if (this.healthHaver.IsBoss)
          {
            gameObject.transform.position = (Vector3) this.specRigidbody.HitboxPixelCollider.UnitTopCenter;
          }
          else
          {
            Bounds bounds = this.sprite.GetBounds();
            Vector3 vector3 = this.transform.position + new Vector3((float) (((double) bounds.max.x + (double) bounds.min.x) / 2.0), bounds.max.y, 0.0f).Quantize(1f / 16f);
            if (effect.PlaysVFXOnActor)
              vector3.y = this.transform.position.y + bounds.min.y;
            gameObject.transform.position = this.sprite.WorldCenter.ToVector3ZUp().WithY(vector3.y);
          }
          component.HeightOffGround = 0.5f;
          this.sprite.AttachRenderer(component);
          gameActorEffectData.instanceOverheadVFX = gameObject.GetComponent<tk2dBaseSprite>();
          if (this.IsGone)
            gameActorEffectData.instanceOverheadVFX.renderer.enabled = false;
        }
        this.m_activeEffects.Add(effect);
        this.m_activeEffectData.Add(gameActorEffectData);
        effect.OnEffectApplied(this, this.m_activeEffectData[this.m_activeEffectData.Count - 1], partialAmount);
      }

      public GameActorEffect GetEffect(string effectIdentifier)
      {
        for (int index = 0; index < this.m_activeEffects.Count; ++index)
        {
          if (this.m_activeEffects[index].effectIdentifier == effectIdentifier)
            return this.m_activeEffects[index];
        }
        return (GameActorEffect) null;
      }

      public GameActorEffect GetEffect(EffectResistanceType resistanceType)
      {
        for (int index = 0; index < this.m_activeEffects.Count; ++index)
        {
          if (this.m_activeEffects[index].resistanceType == resistanceType)
            return this.m_activeEffects[index];
        }
        return (GameActorEffect) null;
      }

      public void RemoveEffect(string effectIdentifier)
      {
        for (int index = this.m_activeEffects.Count - 1; index >= 0; --index)
        {
          if (this.m_activeEffects[index].effectIdentifier == effectIdentifier)
            this.RemoveEffect(index);
        }
      }

      public void RemoveEffect(GameActorEffect effect)
      {
        for (int index = 0; index < this.m_activeEffects.Count; ++index)
        {
          if (this.m_activeEffects[index].effectIdentifier == effect.effectIdentifier)
          {
            this.RemoveEffect(index);
            break;
          }
        }
      }

      public void RemoveAllEffects(bool ignoreDeathCheck = false)
      {
        for (int index = this.m_activeEffects.Count - 1; index >= 0; --index)
          this.RemoveEffect(index, ignoreDeathCheck);
      }

      private void RemoveEffect(int index, bool ignoreDeathCheck = false)
      {
        if (!ignoreDeathCheck && (bool) (UnityEngine.Object) this.healthHaver && this.healthHaver.IsDead)
          return;
        GameActorEffect activeEffect = this.m_activeEffects[index];
        activeEffect.OnEffectRemoved(this, this.m_activeEffectData[index]);
        if (activeEffect.AppliesTint)
          this.DeregisterOverrideColor(activeEffect.effectIdentifier);
        if (activeEffect.AppliesOutlineTint && this is AIActor)
          (this as AIActor).ClearOverrideOutlineColor();
        this.m_activeEffects.RemoveAt(index);
        if ((bool) (UnityEngine.Object) this.m_activeEffectData[index].instanceOverheadVFX)
          UnityEngine.Object.Destroy((UnityEngine.Object) this.m_activeEffectData[index].instanceOverheadVFX.gameObject);
        this.m_activeEffectData.RemoveAt(index);
      }

      protected Vector2 ApplyMovementModifiers(Vector2 voluntaryVel, Vector2 involuntaryVel)
      {
        if (this.MovementModifiers != null)
          this.MovementModifiers(ref voluntaryVel, ref involuntaryVel);
        return voluntaryVel + involuntaryVel;
      }

      protected override void OnDestroy() => base.OnDestroy();

      public void SetResistance(EffectResistanceType resistType, float resistAmount)
      {
        bool flag = false;
        for (int index = 0; index < this.aiActor.EffectResistances.Length; ++index)
        {
          if (this.aiActor.EffectResistances[index].resistType == resistType)
          {
            this.aiActor.EffectResistances[index].resistAmount = resistAmount;
            flag = true;
          }
        }
        if (flag)
          return;
        this.aiActor.EffectResistances = BraveUtility.AppendArray<ActorEffectResistance>(this.aiActor.EffectResistances, new ActorEffectResistance()
        {
          resistType = resistType,
          resistAmount = resistAmount
        });
      }

      public bool IsValid
      {
        get
        {
          if (!(bool) (UnityEngine.Object) this || this.healthHaver.IsDead || this.IsFalling || this.IsGone || !this.specRigidbody.enabled || this.specRigidbody.GetPixelCollider(ColliderType.HitBox) == null)
            return false;
          AIActor aiActor = this as AIActor;
          return !(bool) (UnityEngine.Object) aiActor || aiActor.IsWorthShootingAt;
        }
      }

      public Vector2 AimCenter => this.specRigidbody.GetUnitCenter(ColliderType.HitBox);

      public Vector2 Velocity
      {
        get => this.PreventAutoAimVelocity ? Vector2.zero : this.specRigidbody.Velocity;
      }

      public bool IgnoreForSuperDuperAutoAim => false;

      public float MinDistForSuperDuperAutoAim => 0.0f;

      protected void HandleGoopChecks()
      {
        this.m_currentGoop = (GoopDefinition) null;
        this.m_currentGoopFrozen = false;
        if ((UnityEngine.Object) GameManager.Instance.Dungeon == (UnityEngine.Object) null)
          return;
        List<DeadlyDeadlyGoopManager> roomGoops = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(this.specRigidbody.UnitCenter.ToIntVector2()).RoomGoops;
        if (roomGoops == null)
          return;
        for (int index = 0; index < roomGoops.Count; ++index)
        {
          if (roomGoops[index].ProcessGameActor(this))
          {
            this.m_currentGoop = roomGoops[index].goopDefinition;
            this.m_currentGoopFrozen = roomGoops[index].IsPositionFrozen(this.specRigidbody.UnitCenter);
          }
        }
      }

      public void ForceFall() => this.Fall();

      protected virtual void Fall()
      {
        if ((UnityEngine.Object) this.healthHaver != (UnityEngine.Object) null)
          this.healthHaver.EndFlashEffects();
        this.m_isFalling = true;
        if (!this.HasShadow || !(bool) (UnityEngine.Object) this.ShadowObject)
          return;
        this.ShadowObject.GetComponent<Renderer>().enabled = false;
      }

      public virtual void RecoverFromFall()
      {
        this.m_isFalling = false;
        if (!this.HasShadow || !(bool) (UnityEngine.Object) this.ShadowObject)
          return;
        this.ShadowObject.GetComponent<Renderer>().enabled = true;
      }

      protected virtual bool QueryGroundedFrame()
      {
        return !(bool) (UnityEngine.Object) this.spriteAnimator || this.spriteAnimator.QueryGroundedFrame();
      }

      protected void HandlePitChecks()
      {
        if ((UnityEngine.Object) GameManager.Instance.Dungeon == (UnityEngine.Object) null || GameManager.Instance.Dungeon.data == null)
          return;
        bool flag1 = this.QueryGroundedFrame() && !this.IsFlying && !this.FallingProhibited;
        PlayerController playerController = this as PlayerController;
        if ((bool) (UnityEngine.Object) playerController && playerController.CurrentRoom != null && playerController.CurrentRoom.RoomFallValidForMaintenance())
          flag1 = true;
        if (this.m_isFalling)
          return;
        Rect source = new Rect();
        source.min = PhysicsEngine.PixelToUnitMidpoint(this.specRigidbody.PrimaryPixelCollider.LowerLeft);
        source.max = PhysicsEngine.PixelToUnitMidpoint(this.specRigidbody.PrimaryPixelCollider.UpperRight);
        Rect rect = new Rect(source);
        this.ModifyPitVectors(ref rect);
        Dungeon dungeon = GameManager.Instance.Dungeon;
        bool flag2 = dungeon.ShouldReallyFall((Vector3) rect.min);
        bool flag3 = dungeon.ShouldReallyFall(new Vector3(rect.xMax, rect.yMin));
        bool flag4 = dungeon.ShouldReallyFall(new Vector3(rect.xMin, rect.yMax));
        bool flag5 = dungeon.ShouldReallyFall((Vector3) rect.max);
        bool flag6 = dungeon.ShouldReallyFall((Vector3) rect.center);
        this.IsOverPitAtAll = flag2 || flag3 || flag4 || flag5 || flag6;
        if (this.IsOverPitAtAll)
        {
          bool flag7 = flag2 | dungeon.data.isWall((int) rect.xMin, (int) rect.yMin);
          bool flag8 = flag3 | dungeon.data.isWall((int) rect.xMax, (int) rect.yMin);
          bool flag9 = flag4 | dungeon.data.isWall((int) rect.xMin, (int) rect.yMax);
          bool flag10 = flag5 | dungeon.data.isWall((int) rect.xMax, (int) rect.yMax);
          bool flag11 = flag6 | dungeon.data.isWall((int) rect.center.x, (int) rect.center.y);
          bool flag12 = flag7 && flag8 && flag9 && flag10 && flag11;
          bool flag13 = this.OnAboutToFall == null || this.OnAboutToFall(!flag12);
          if (flag12 && flag1 && flag13)
          {
            this.Fall();
            return;
          }
        }
        bool flag14 = true;
        for (int index = 0; index < this.SupportingPlatforms.Count; ++index)
        {
          if (!this.SupportingPlatforms[index].StaticForPitfall)
          {
            flag14 = false;
            break;
          }
        }
        if (!flag14)
          return;
        if (this.SupportingPlatforms.Count > 0)
          this.m_cachedPosition = this.SupportingPlatforms[0].specRigidbody.UnitCenter;
        else if ((double) Vector3.Distance((Vector3) this.m_cachedPosition, (Vector3) this.specRigidbody.Position.GetPixelVector2()) > 3.0)
        {
          bool flag15 = dungeon.CellSupportsFalling((Vector3) source.min) || dungeon.PositionInCustomPitSRB((Vector3) source.min);
          bool flag16 = dungeon.CellSupportsFalling(new Vector3(source.xMax, source.yMin)) || dungeon.PositionInCustomPitSRB(new Vector3(source.xMax, source.yMin));
          bool flag17 = dungeon.CellSupportsFalling(new Vector3(source.xMin, source.yMax)) || dungeon.PositionInCustomPitSRB(new Vector3(source.xMin, source.yMax));
          bool flag18 = dungeon.CellSupportsFalling((Vector3) source.max) || dungeon.PositionInCustomPitSRB((Vector3) source.max);
          bool flag19 = dungeon.CellSupportsFalling((Vector3) source.center) || dungeon.PositionInCustomPitSRB((Vector3) source.center);
          IntVector2 intVector2 = source.min.ToIntVector2(VectorConversions.Floor);
          bool flag20 = dungeon.data.CheckInBoundsAndValid(intVector2) && dungeon.data[intVector2].type == CellType.FLOOR;
          if (flag15 || flag16 || flag17 || flag18 || flag19 || !flag20)
            return;
          this.m_cachedPosition = this.specRigidbody.Position.GetPixelVector2();
        }
        else
        {
          bool flag21 = dungeon.CellIsNearPit((Vector3) source.min) || dungeon.PositionInCustomPitSRB((Vector3) source.min);
          bool flag22 = dungeon.CellIsNearPit(new Vector3(source.xMax, source.yMin)) || dungeon.PositionInCustomPitSRB(new Vector3(source.xMax, source.yMin));
          bool flag23 = dungeon.CellIsNearPit(new Vector3(source.xMin, source.yMax)) || dungeon.PositionInCustomPitSRB(new Vector3(source.xMin, source.yMax));
          bool flag24 = dungeon.CellIsNearPit((Vector3) source.max) || dungeon.PositionInCustomPitSRB((Vector3) source.max);
          bool flag25 = dungeon.CellIsNearPit((Vector3) source.center) || dungeon.PositionInCustomPitSRB((Vector3) source.center);
          IntVector2 intVector2 = source.min.ToIntVector2(VectorConversions.Floor);
          bool flag26 = dungeon.data.CheckInBoundsAndValid(intVector2) && dungeon.data[intVector2].type == CellType.FLOOR;
          if (flag21 || flag22 || flag23 || flag24 || flag25 || !flag26)
            return;
          this.m_cachedPosition = this.specRigidbody.Position.GetPixelVector2();
        }
      }

      public void PlaySmallExplosionsStyleEffect(GameObject vfxPrefab, int count, float midDelay)
      {
        if (!(bool) (UnityEngine.Object) this.sprite)
          return;
        this.StartCoroutine(this.HandleSmallExplosionsStyleEffect(vfxPrefab, count, midDelay));
      }

      [DebuggerHidden]
      private IEnumerator HandleSmallExplosionsStyleEffect(
        GameObject vfxPrefab,
        int explosionCount,
        float explosionMidDelay)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new GameActor.<HandleSmallExplosionsStyleEffect>c__Iterator0()
        {
          explosionCount = explosionCount,
          vfxPrefab = vfxPrefab,
          explosionMidDelay = explosionMidDelay,
          _this = this
        };
      }

      protected virtual void ModifyPitVectors(ref Rect rect)
      {
      }

      public GameObject PlayEffectOnActor(
        GameObject effect,
        Vector3 offset,
        bool attached = true,
        bool alreadyMiddleCenter = false,
        bool useHitbox = false)
      {
        GameObject gameObject = SpawnManager.SpawnVFX(effect);
        tk2dBaseSprite component1 = gameObject.GetComponent<tk2dBaseSprite>();
        Vector3 vector3 = !useHitbox || !(bool) (UnityEngine.Object) this.specRigidbody || this.specRigidbody.HitboxPixelCollider == null ? this.sprite.WorldCenter.ToVector3ZUp() : this.specRigidbody.HitboxPixelCollider.UnitCenter.ToVector3ZUp();
        if (!alreadyMiddleCenter)
          component1.PlaceAtPositionByAnchor(vector3 + offset, tk2dBaseSprite.Anchor.MiddleCenter);
        else
          component1.transform.position = vector3 + offset;
        if (attached)
        {
          gameObject.transform.parent = this.transform;
          component1.HeightOffGround = 0.2f;
          this.sprite.AttachRenderer(component1);
          if (this is PlayerController)
          {
            SmartOverheadVFXController component2 = gameObject.GetComponent<SmartOverheadVFXController>();
            if ((UnityEngine.Object) component2 != (UnityEngine.Object) null)
              component2.Initialize(this as PlayerController, offset);
          }
        }
        if (!alreadyMiddleCenter)
          gameObject.transform.localPosition = gameObject.transform.localPosition.QuantizeFloor(1f / 16f);
        return gameObject;
      }

      public GameObject PlayFairyEffectOnActor(
        GameObject effect,
        Vector3 offset,
        float duration,
        bool alreadyMiddleCenter = false)
      {
        if (this.sprite.FlipX)
          offset += new Vector3(this.sprite.GetBounds().extents.x * 2f, 0.0f, 0.0f);
        GameObject gameObject = SpawnManager.SpawnVFX(effect, true);
        tk2dBaseSprite component = gameObject.GetComponent<tk2dBaseSprite>();
        gameObject.transform.parent = this.transform;
        component.HeightOffGround = 0.2f;
        this.sprite.AttachRenderer(component);
        this.StartCoroutine(this.HandleFairyFlyEffect(component, offset, duration, alreadyMiddleCenter));
        return gameObject;
      }

      [DebuggerHidden]
      protected IEnumerator HandleFairyFlyEffect(
        tk2dBaseSprite instantiated,
        Vector3 offset,
        float duration,
        bool alreadyMiddleCenter)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new GameActor.<HandleFairyFlyEffect>c__Iterator1()
        {
          instantiated = instantiated,
          duration = duration,
          offset = offset,
          _this = this
        };
      }

      public Color CurrentOverrideColor
      {
        get
        {
          if (this.m_overrideColorStack.Count == 0)
            this.RegisterOverrideColor(new Color(1f, 1f, 1f, 0.0f), "base");
          return this.m_overrideColorStack[this.m_overrideColorStack.Count - 1];
        }
      }

      public bool HasSourcedOverrideColor(string source)
      {
        return this.m_overrideColorSources.Contains(source);
      }

      public bool HasOverrideColor()
      {
        return this.m_overrideColorSources.Count != 0 && (this.m_overrideColorSources.Count != 1 || !(this.m_overrideColorSources[0] == "base"));
      }

      public void RegisterOverrideColor(Color overrideColor, string source)
      {
        int index = this.m_overrideColorSources.IndexOf(source);
        if (index >= 0)
        {
          this.m_overrideColorStack[index] = overrideColor;
        }
        else
        {
          this.m_overrideColorSources.Add(source);
          this.m_overrideColorStack.Add(overrideColor);
        }
        this.OnOverrideColorsChanged();
      }

      public void DeregisterOverrideColor(string source)
      {
        int index = this.m_overrideColorSources.IndexOf(source);
        if (index >= 0)
        {
          this.m_overrideColorStack.RemoveAt(index);
          this.m_overrideColorSources.RemoveAt(index);
        }
        this.OnOverrideColorsChanged();
      }

      public void OnOverrideColorsChanged()
      {
        if (this.OverrideColorOverridden)
          return;
        for (int index = 0; index < this.healthHaver.bodySprites.Count; ++index)
        {
          if ((bool) (UnityEngine.Object) this.healthHaver.bodySprites[index])
          {
            this.healthHaver.bodySprites[index].usesOverrideMaterial = true;
            this.healthHaver.bodySprites[index].renderer.material.SetColor(this.m_overrideColorID, this.CurrentOverrideColor);
          }
        }
        if (!(bool) (UnityEngine.Object) this.renderer || !(bool) (UnityEngine.Object) this.renderer.material)
          return;
        this.m_colorOverridenMaterial = this.renderer.material;
        this.m_colorOverridenShader = this.m_colorOverridenMaterial.shader;
      }

      public void ToggleShadowVisiblity(bool value)
      {
        if (!(bool) (UnityEngine.Object) this.ShadowObject)
          return;
        this.ShadowObject.GetComponent<Renderer>().enabled = value;
      }

      protected GameObject GenerateDefaultBlobShadow(float heightOffset = 0.0f)
      {
        if ((bool) (UnityEngine.Object) this.ShadowObject)
        {
          BraveUtility.Log("We are trying to generate a GameActor shadow when we already have one!", Color.red, BraveUtility.LogVerbosity.IMPORTANT);
          return this.ShadowObject;
        }
        Transform transform = this.transform;
        SpeculativeRigidbody componentInChildren = this.gameObject.GetComponentInChildren<SpeculativeRigidbody>();
        if ((bool) (UnityEngine.Object) componentInChildren)
          componentInChildren.Reinitialize();
        if ((bool) (UnityEngine.Object) this.ShadowPrefab)
        {
          GameObject defaultBlobShadow = UnityEngine.Object.Instantiate<GameObject>(this.ShadowPrefab);
          defaultBlobShadow.transform.parent = transform;
          defaultBlobShadow.transform.localPosition = !(bool) (UnityEngine.Object) this.specRigidbody ? Vector3.zero : this.specRigidbody.UnitCenter.ToVector3ZUp() - this.transform.position.WithZ(0.0f);
          DepthLookupManager.ProcessRenderer(defaultBlobShadow.GetComponent<Renderer>(), DepthLookupManager.GungeonSortingLayer.BACKGROUND);
          if ((UnityEngine.Object) this.aiActor != (UnityEngine.Object) null && this.aiActor.ActorName == "Gatling Gull")
            defaultBlobShadow.GetComponent<tk2dBaseSprite>().HeightOffGround = 6f;
          else if ((double) this.ShadowHeightOffGround != 0.0)
            defaultBlobShadow.GetComponent<tk2dBaseSprite>().HeightOffGround = this.ShadowHeightOffGround;
          this.ShadowObject = defaultBlobShadow;
          defaultBlobShadow.transform.position = defaultBlobShadow.transform.position.Quantize(1f / 16f);
          return defaultBlobShadow;
        }
        if ((UnityEngine.Object) transform.Find("PlayerSprite") != (UnityEngine.Object) null)
        {
          if ((UnityEngine.Object) transform.Find("PlayerShadow") != (UnityEngine.Object) null)
            return transform.Find("PlayerShadow").gameObject;
          PlayerController component = transform.GetComponent<PlayerController>();
          GameObject defaultBlobShadow = (GameObject) UnityEngine.Object.Instantiate(ResourceCache.Acquire("DefaultShadowSprite"));
          defaultBlobShadow.transform.parent = transform;
          defaultBlobShadow.transform.localPosition = Vector3.zero;
          defaultBlobShadow.transform.localPosition = new Vector3(component.SpriteBottomCenter.x - transform.position.x, 0.0f, 0.1f);
          defaultBlobShadow.GetComponent<tk2dSprite>().HeightOffGround = -0.1f;
          DepthLookupManager.ProcessRenderer(defaultBlobShadow.GetComponent<Renderer>(), DepthLookupManager.GungeonSortingLayer.PLAYFIELD);
          this.ShadowObject = defaultBlobShadow;
          defaultBlobShadow.transform.position = defaultBlobShadow.transform.position.Quantize(1f / 16f);
          return defaultBlobShadow;
        }
        if (!((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null))
          return (GameObject) null;
        GameObject defaultBlobShadow1 = (GameObject) UnityEngine.Object.Instantiate(ResourceCache.Acquire("DefaultShadowSprite"));
        defaultBlobShadow1.transform.parent = transform;
        float y = componentInChildren.UnitBottomLeft.y - transform.position.y + heightOffset;
        Vector3 vector3 = new Vector3(componentInChildren.UnitCenter.x - transform.position.x, y, 0.1f);
        defaultBlobShadow1.transform.localPosition = vector3;
        defaultBlobShadow1.GetComponent<tk2dSprite>().HeightOffGround = (float) (-(double) heightOffset * 2.0) + this.ShadowHeightOffGround;
        DepthLookupManager.ProcessRenderer(defaultBlobShadow1.GetComponent<Renderer>(), DepthLookupManager.GungeonSortingLayer.PLAYFIELD);
        this.ShadowObject = defaultBlobShadow1;
        defaultBlobShadow1.transform.position = defaultBlobShadow1.transform.position.Quantize(1f / 16f);
        this.ShadowObject.transform.localPosition += this.ActorShadowOffset;
        return defaultBlobShadow1;
      }

      public delegate void MovementModifier(ref Vector2 volundaryVel, ref Vector2 involuntaryVel);
    }

}
