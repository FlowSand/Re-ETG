// Decompiled with JetBrains decompiler
// Type: BasicBeamController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class BasicBeamController : BeamController
    {
      public bool usesTelegraph;
      [ShowInInspectorIf("usesTelegraph", true)]
      public float telegraphTime;
      [ShowInInspectorIf("usesTelegraph", true)]
      public BasicBeamController.TelegraphAnims telegraphAnimations;
      [Header("Beam Structure")]
      public BasicBeamController.BeamBoneType boneType;
      [ShowInInspectorIf("boneType", 1, true)]
      public bool interpolateStretchedBones;
      public int penetration;
      public int reflections;
      public bool PenetratesCover;
      public bool angularKnockback;
      [ShowInInspectorIf("angularKnockback", true)]
      public float angularSpeedAvgWindow = 0.15f;
      public List<BasicBeamController.AngularKnockbackTier> angularKnockbackTiers;
      public float homingRadius;
      public float homingAngularVelocity;
      public float TimeToStatus = 0.5f;
      [Header("Beam Animations")]
      public BasicBeamController.BeamTileType TileType;
      [CheckAnimation(null)]
      public string beamAnimation;
      [CheckAnimation(null)]
      public string beamStartAnimation;
      [CheckAnimation(null)]
      public string beamEndAnimation;
      [Header("Beam Overlays")]
      [CheckAnimation(null)]
      public string muzzleAnimation;
      [CheckAnimation(null)]
      public string chargeAnimation;
      [ShowInInspectorIf("chargeAnimation", true)]
      public bool rotateChargeAnimation;
      [CheckAnimation(null)]
      public string impactAnimation;
      [Header("Persistence")]
      public BasicBeamController.BeamEndType endType;
      [ShowInInspectorIf("endType", 1, true)]
      public float decayNear;
      [ShowInInspectorIf("endType", 1, true)]
      public float decayFar;
      [ShowInInspectorIf("endType", 1, true)]
      public bool collisionSeparation;
      [ShowInInspectorIf("endType", 1, true)]
      public float breakAimAngle;
      [ShowInInspectorIf("endType", 2, true)]
      public float dissipateTime;
      [ShowInInspectorIf("endType", 2, true)]
      public BasicBeamController.TelegraphAnims dissipateAnimations;
      [Header("Collision")]
      public BasicBeamController.BeamCollisionType collisionType;
      [ShowInInspectorIf("collisionType", 0, true)]
      public float collisionRadius = 1.5f;
      [ShowInInspectorIf("collisionType", 1, true)]
      public int collisionLength = 320;
      [ShowInInspectorIf("collisionType", 1, true)]
      public int collisionWidth = 64 /*0x40*/;
      [Header("Particles")]
      public bool UsesDispersalParticles;
      [ShowInInspectorIf("UsesDispersalParticles", true)]
      public float DispersalDensity = 3f;
      [ShowInInspectorIf("UsesDispersalParticles", true)]
      public float DispersalMinCoherency = 0.2f;
      [ShowInInspectorIf("UsesDispersalParticles", true)]
      public float DispersalMaxCoherency = 1f;
      [ShowInInspectorIf("UsesDispersalParticles", true)]
      public GameObject DispersalParticleSystemPrefab;
      [ShowInInspectorIf("UsesDispersalParticles", true)]
      public float DispersalExtraImpactFactor = 1f;
      [Header("Nonsense")]
      public bool doesScreenDistortion;
      [ShowInInspectorIf("doesScreenDistortion", true)]
      public float startDistortionRadius = 0.3f;
      [ShowInInspectorIf("doesScreenDistortion", true)]
      public float endDistortionRadius = 0.2f;
      [ShowInInspectorIf("doesScreenDistortion", true)]
      public float startDistortionPower = 0.7f;
      [ShowInInspectorIf("doesScreenDistortion", true)]
      public float endDistortionPower = 0.5f;
      [ShowInInspectorIf("doesScreenDistortion", true)]
      public float distortionPulseSpeed = 25f;
      [ShowInInspectorIf("doesScreenDistortion", true)]
      public float minDistortionOffset;
      [ShowInInspectorIf("doesScreenDistortion", true)]
      public float distortionOffsetIncrease = 0.02f;
      public int overrideBeamQuadPixelWidth = -1;
      public bool FlipBeamSpriteLocal;
      [Header("Audio Flags")]
      public string startAudioEvent;
      public string endAudioEvent;
      [NonSerialized]
      private Material m_distortionMaterial;
      [NonSerialized]
      public Action<SpeculativeRigidbody, Vector2> OverrideHitChecks;
      [NonSerialized]
      public bool SkipPostProcessing;
      public float IgnoreTilesDistance = -1f;
      private bool m_hasToggledGunOutline;
      private int exceptionTracker;
      private static List<Vector2> s_goopPoints = new List<Vector2>();
      private Vector2? m_lastBeamEnd;
      private int m_currentTintPriority = -1;
      private Vector2 m_cachedRectangleOrigin;
      private Vector2 m_cachedRectangleDirection;
      private tk2dTiledSprite m_beamSprite;
      private Transform m_muzzleTransform;
      private tk2dSprite m_beamMuzzleSprite;
      private tk2dSpriteAnimator m_beamMuzzleAnimator;
      private Transform m_impactTransform;
      private tk2dSprite m_impactSprite;
      private tk2dSpriteAnimator m_impactAnimator;
      private Transform m_impact2Transform;
      private tk2dSprite m_impact2Sprite;
      private tk2dSpriteAnimator m_impact2Animator;
      private GoopModifier m_beamGoopModifier;
      private List<tk2dBaseSprite> m_pierceImpactSprites;
      private float m_chargeTimer;
      private float m_telegraphTimer;
      private float m_dissipateTimer;
      private KnockbackDoer m_enemyKnockback;
      private int m_enemyKnockbackId;
      private Vector3 m_beamSpriteDimensions;
      private int m_beamSpriteSubtileWidth;
      private float m_beamSpriteUnitWidth;
      private float m_uvOffset;
      private float? m_previousAngle;
      private float m_currentBeamDistance;
      private LinkedList<BasicBeamController.BeamBone> m_bones;
      private BasicBeamController m_reflectedBeam;
      private Vector2 m_lastHitNormal;
      private int m_beamQuadPixelWidth;
      private float m_beamQuadUnitWidth;
      private float m_sqrNewBoneThreshold;
      private float m_projectileScale = 1f;
      private float m_currentLuteScaleModifier = 1f;
      private float averageAngularVelocity;
      private ParticleSystem m_dispersalParticles;
      private const float c_minGoopDistance = 1.75f;
      private static float CurrentBeamHeightOffGround = 0.75f;
      private const float c_defaultBeamHeightOffGround = 0.75f;
      private const int c_defaultBeamQuadPixelWidth = 4;
      private static readonly List<IntVector2> s_pixelCloud = new List<IntVector2>();
      private static readonly List<IntVector2> s_lastPixelCloud = new List<IntVector2>();

      public bool playerStatsModified { get; set; }

      public bool SelfUpdate { get; set; }

      public BasicBeamController.BeamState State { get; set; }

      public float HeightOffset { get; set; }

      public float RampHeightOffset { get; set; }

      public bool ContinueBeamArtToWall { get; set; }

      public float BoneSpeed
      {
        get
        {
          return this.State == BasicBeamController.BeamState.Telegraphing ? -1f : this.projectile.baseData.speed;
        }
      }

      public override bool ShouldUseAmmo => this.State == BasicBeamController.BeamState.Firing;

      public string CurrentBeamAnimation
      {
        get
        {
          if (this.State == BasicBeamController.BeamState.Telegraphing)
            return this.telegraphAnimations.beamAnimation;
          return this.State == BasicBeamController.BeamState.Dissipating ? this.dissipateAnimations.beamAnimation : this.beamAnimation;
        }
      }

      public string CurrentBeamStartAnimation
      {
        get
        {
          if (this.State == BasicBeamController.BeamState.Telegraphing)
            return this.telegraphAnimations.beamStartAnimation;
          return this.State == BasicBeamController.BeamState.Dissipating ? this.dissipateAnimations.beamStartAnimation : this.beamStartAnimation;
        }
      }

      public string CurrentBeamEndAnimation
      {
        get
        {
          if (this.State == BasicBeamController.BeamState.Telegraphing)
            return this.telegraphAnimations.beamEndAnimation;
          return this.State == BasicBeamController.BeamState.Dissipating ? this.dissipateAnimations.beamEndAnimation : this.beamEndAnimation;
        }
      }

      public bool UsesChargeSprite => !string.IsNullOrEmpty(this.chargeAnimation);

      public bool UsesMuzzleSprite => !string.IsNullOrEmpty(this.muzzleAnimation);

      public bool UsesImpactSprite => !string.IsNullOrEmpty(this.impactAnimation);

      public bool UsesBeamStartAnimation => !string.IsNullOrEmpty(this.CurrentBeamStartAnimation);

      public bool UsesBeamEndAnimation => !string.IsNullOrEmpty(this.CurrentBeamEndAnimation);

      public bool UsesBones
      {
        get
        {
          return this.boneType == BasicBeamController.BeamBoneType.Projectile || this.IsHoming || this.ProjectileAndBeamMotionModule != null;
        }
      }

      public bool IsConnected => this.State != BasicBeamController.BeamState.Disconnected;

      public float HomingRadius => this.ChanceBasedHomingRadius + this.homingRadius;

      public float HomingAngularVelocity
      {
        get
        {
          float num = this.ChanceBasedHomingAngularVelocity + this.homingAngularVelocity;
          return (double) this.BoneSpeed < 0.0 ? num : num * (this.BoneSpeed / 40f);
        }
      }

      public bool IsHoming
      {
        get => (double) this.HomingRadius > 0.0 && (double) this.HomingAngularVelocity > 0.0;
      }

      public SpeculativeRigidbody ReflectedFromRigidbody { get; set; }

      public bool ShowImpactOnMaxDistanceEnd { get; set; }

      public bool IsBlackBullet { get; set; }

      public ProjectileAndBeamMotionModule ProjectileAndBeamMotionModule { get; set; }

      public float ProjectileScale
      {
        get => this.m_projectileScale;
        set => this.m_projectileScale = value;
      }

      public float ApproximateDistance => this.m_currentBeamDistance;

      public void Start()
      {
        if (this.UsesDispersalParticles && (UnityEngine.Object) this.m_dispersalParticles == (UnityEngine.Object) null)
          this.m_dispersalParticles = GlobalDispersalParticleManager.GetSystemForPrefab(this.DispersalParticleSystemPrefab);
        this.m_beamQuadPixelWidth = this.overrideBeamQuadPixelWidth <= 0 ? 4 : this.overrideBeamQuadPixelWidth;
        this.m_beamQuadUnitWidth = (float) this.m_beamQuadPixelWidth / 16f;
        this.m_sqrNewBoneThreshold = (float) ((double) this.m_beamQuadUnitWidth * (double) this.m_beamQuadUnitWidth * 2.0 * 2.0);
        this.m_beamSprite = this.gameObject.GetComponent<tk2dTiledSprite>();
        this.m_beamSprite.renderer.sortingLayerName = "Player";
        this.m_beamSpriteDimensions = this.m_beamSprite.GetUntrimmedBounds().size;
        this.m_beamSprite.dimensions = new Vector2(0.0f, this.m_beamSpriteDimensions.y * 16f);
        this.spriteAnimator.Play(this.beamAnimation);
        this.m_beamSprite.HeightOffGround = BasicBeamController.CurrentBeamHeightOffGround + this.HeightOffset;
        this.m_beamSprite.IsPerpendicular = false;
        this.m_beamSprite.usesOverrideMaterial = true;
        PlayerController owner1 = this.projectile.Owner as PlayerController;
        if ((bool) (UnityEngine.Object) owner1)
          this.m_projectileScale = owner1.BulletScaleModifier;
        if (this.IsConnected)
        {
          this.m_muzzleTransform = this.transform.Find("beam muzzle flare");
          if ((bool) (UnityEngine.Object) this.m_muzzleTransform)
          {
            this.m_beamMuzzleSprite = this.m_muzzleTransform.GetComponent<tk2dSprite>();
            this.m_beamMuzzleAnimator = this.m_muzzleTransform.GetComponent<tk2dSpriteAnimator>();
          }
          if (this.UsesChargeSprite || this.UsesMuzzleSprite)
          {
            if (!(bool) (UnityEngine.Object) this.m_muzzleTransform)
            {
              GameObject gameObject = new GameObject("beam muzzle flare");
              this.m_muzzleTransform = gameObject.transform;
              this.m_muzzleTransform.parent = this.transform;
              this.m_muzzleTransform.localPosition = new Vector3(0.0f, 0.0f, 0.05f);
              this.m_beamMuzzleSprite = gameObject.AddComponent<tk2dSprite>();
              this.m_beamMuzzleSprite.SetSprite(this.m_beamSprite.Collection, this.m_beamSprite.spriteId);
              this.m_beamMuzzleAnimator = gameObject.AddComponent<tk2dSpriteAnimator>();
              this.m_beamMuzzleAnimator.SetSprite(this.m_beamSprite.Collection, this.m_beamSprite.spriteId);
              this.m_beamMuzzleAnimator.Library = this.spriteAnimator.Library;
              this.m_beamSprite.AttachRenderer((tk2dBaseSprite) this.m_beamMuzzleSprite);
              this.m_beamMuzzleSprite.HeightOffGround = 0.05f;
              this.m_beamMuzzleSprite.IsPerpendicular = false;
              this.m_beamMuzzleSprite.usesOverrideMaterial = true;
            }
            this.m_muzzleTransform.localScale = new Vector3(this.m_projectileScale, this.m_projectileScale, 1f);
          }
          if ((bool) (UnityEngine.Object) this.m_muzzleTransform)
            this.m_muzzleTransform.gameObject.SetActive(false);
          if (this.usesChargeDelay)
          {
            this.renderer.enabled = false;
            this.State = BasicBeamController.BeamState.Charging;
            if (this.UsesChargeSprite)
            {
              this.m_beamMuzzleAnimator.Play(this.chargeAnimation);
              this.m_muzzleTransform.gameObject.SetActive(true);
            }
          }
          else if (this.usesTelegraph)
          {
            this.State = BasicBeamController.BeamState.Telegraphing;
            this.spriteAnimator.Play(this.CurrentBeamAnimation);
          }
          else
          {
            this.State = BasicBeamController.BeamState.Firing;
            if (this.UsesMuzzleSprite)
            {
              this.m_beamMuzzleAnimator.Play(this.muzzleAnimation);
              this.m_muzzleTransform.gameObject.SetActive(true);
            }
          }
          AIActor owner2 = this.Owner as AIActor;
          if ((bool) (UnityEngine.Object) owner2 && owner2.IsBlackPhantom)
            this.BecomeBlackBullet();
          if (GameManager.AUDIO_ENABLED && !string.IsNullOrEmpty(this.startAudioEvent))
          {
            int num = (int) AkSoundEngine.PostEvent(this.startAudioEvent, this.gameObject);
          }
        }
        else
        {
          this.m_muzzleTransform = this.transform.Find("beam muzzle flare");
          if ((bool) (UnityEngine.Object) this.m_muzzleTransform)
            this.m_muzzleTransform.gameObject.SetActive(false);
        }
        if (this.UsesImpactSprite)
        {
          this.m_impactTransform = this.transform.Find("beam impact vfx");
          if ((bool) (UnityEngine.Object) this.m_impactTransform)
          {
            this.m_impactSprite = this.m_impactTransform.GetComponent<tk2dSprite>();
            this.m_impactAnimator = this.m_impactTransform.GetComponent<tk2dSpriteAnimator>();
          }
          else
          {
            GameObject gameObject = new GameObject("beam impact vfx");
            this.m_impactTransform = gameObject.transform;
            this.m_impactTransform.parent = this.transform;
            this.m_impactTransform.localPosition = new Vector3(0.0f, 0.0f, 0.05f);
            this.m_impactSprite = gameObject.AddComponent<tk2dSprite>();
            this.m_impactSprite.SetSprite(this.m_beamSprite.Collection, this.m_beamSprite.spriteId);
            this.m_impactAnimator = gameObject.AddComponent<tk2dSpriteAnimator>();
            this.m_impactAnimator.SetSprite(this.m_beamSprite.Collection, this.m_beamSprite.spriteId);
            this.m_impactAnimator.Library = this.spriteAnimator.Library;
            this.m_beamSprite.AttachRenderer((tk2dBaseSprite) this.m_impactSprite);
            this.m_impactSprite.HeightOffGround = 0.05f;
            this.m_impactSprite.IsPerpendicular = true;
            this.m_impactSprite.usesOverrideMaterial = true;
          }
          this.m_impactTransform.localScale = new Vector3(this.m_projectileScale, this.m_projectileScale, 1f);
          this.m_impact2Transform = this.transform.Find("beam impact vfx 2");
          if ((bool) (UnityEngine.Object) this.m_impact2Transform)
          {
            this.m_impact2Sprite = this.m_impact2Transform.GetComponent<tk2dSprite>();
            this.m_impact2Animator = this.m_impact2Transform.GetComponent<tk2dSpriteAnimator>();
          }
          else
          {
            GameObject gameObject = new GameObject("beam impact vfx 2");
            this.m_impact2Transform = gameObject.transform;
            this.m_impact2Transform.parent = this.transform;
            this.m_impact2Transform.localPosition = new Vector3(0.0f, 0.0f, 0.05f);
            this.m_impact2Sprite = gameObject.AddComponent<tk2dSprite>();
            this.m_impact2Sprite.SetSprite(this.m_beamSprite.Collection, this.m_beamSprite.spriteId);
            this.m_impact2Animator = gameObject.AddComponent<tk2dSpriteAnimator>();
            this.m_impact2Animator.SetSprite(this.m_beamSprite.Collection, this.m_beamSprite.spriteId);
            this.m_impact2Animator.Library = this.spriteAnimator.Library;
            this.m_beamSprite.AttachRenderer((tk2dBaseSprite) this.m_impact2Sprite);
            this.m_impact2Sprite.HeightOffGround = 0.05f;
            this.m_impact2Sprite.IsPerpendicular = true;
            this.m_impact2Sprite.usesOverrideMaterial = true;
          }
          this.m_impact2Transform.localScale = new Vector3(this.m_projectileScale, this.m_projectileScale, 1f);
          if (!this.m_impactAnimator.IsPlaying(this.impactAnimation))
            this.m_impactAnimator.Play(this.impactAnimation);
          if (!this.m_impact2Animator.IsPlaying(this.impactAnimation))
            this.m_impact2Animator.Play(this.impactAnimation);
          if ((bool) (UnityEngine.Object) this.m_impactTransform)
            this.m_impactTransform.gameObject.SetActive(false);
          if ((bool) (UnityEngine.Object) this.m_impact2Transform)
            this.m_impact2Transform.gameObject.SetActive(false);
          for (int index = 0; index < this.transform.childCount; ++index)
          {
            Transform child = this.transform.GetChild(index);
            if (child.name.StartsWith("beam pierce impact vfx"))
            {
              if (this.m_pierceImpactSprites == null)
                this.m_pierceImpactSprites = new List<tk2dBaseSprite>();
              this.m_pierceImpactSprites.Add(child.GetComponent<tk2dBaseSprite>());
              this.m_pierceImpactSprites[this.m_pierceImpactSprites.Count - 1].gameObject.SetActive(false);
            }
          }
        }
        this.m_beamSprite.OverrideGetTiledSpriteGeomDesc = new tk2dTiledSprite.OverrideGetTiledSpriteGeomDescDelegate(this.GetTiledSpriteGeomDesc);
        this.m_beamSprite.OverrideSetTiledSpriteGeom = new tk2dTiledSprite.OverrideSetTiledSpriteGeomDelegate(this.SetTiledSpriteGeom);
        tk2dSpriteDefinition currentSpriteDef = this.m_beamSprite.GetCurrentSpriteDef();
        this.m_beamSpriteSubtileWidth = Mathf.RoundToInt(currentSpriteDef.untrimmedBoundsDataExtents.x / currentSpriteDef.texelSize.x) / this.m_beamQuadPixelWidth;
        this.m_beamSpriteUnitWidth = (float) Mathf.RoundToInt(currentSpriteDef.untrimmedBoundsDataExtents.x / currentSpriteDef.texelSize.x) / 16f;
        if (this.m_bones == null)
        {
          this.m_bones = new LinkedList<BasicBeamController.BeamBone>();
          this.m_bones.AddFirst(new BasicBeamController.BeamBone(0.0f, 0.0f, this.m_beamSpriteSubtileWidth - 1));
          this.m_bones.AddLast(new BasicBeamController.BeamBone(0.0f, 0.0f, -1));
          this.m_uvOffset = 1f;
          if (this.boneType == BasicBeamController.BeamBoneType.Projectile)
          {
            this.m_bones.First.Value.Position = this.Origin;
            this.m_bones.First.Value.Velocity = this.Direction.normalized * this.BoneSpeed;
            this.m_bones.Last.Value.Position = this.Origin;
            this.m_bones.Last.Value.Velocity = this.Direction.normalized * this.BoneSpeed;
          }
        }
        else
        {
          this.m_beamSprite.ForceBuild();
          this.m_beamSprite.UpdateZDepth();
        }
        this.m_beamGoopModifier = this.gameObject.GetComponent<GoopModifier>();
        if (this.IsConnected && this.Owner is PlayerController && !this.SkipPostProcessing)
          (this.Owner as PlayerController).DoPostProcessBeam((BeamController) this);
        if (this.IsConnected && this.Owner is AIActor && !this.SkipPostProcessing)
        {
          AIActor owner3 = this.Owner as AIActor;
          if ((bool) (UnityEngine.Object) owner3.CompanionOwner)
          {
            owner3.CompanionOwner.DoPostProcessBeam((BeamController) this);
            this.ProjectileScale *= owner3.CompanionOwner.BulletScaleModifier;
          }
        }
        this.ProjectileAndBeamMotionModule = this.projectile.OverrideMotionModule as ProjectileAndBeamMotionModule;
      }

      public void Update()
      {
        if (this.State == BasicBeamController.BeamState.Disconnected || this.State == BasicBeamController.BeamState.Dissipating)
        {
          if (this.boneType != BasicBeamController.BeamBoneType.Straight && (this.m_bones.Count == 2 && Mathf.Approximately(this.m_bones.First.Value.PosX, this.m_bones.Last.Value.PosX) || this.m_bones.Count < 2))
            this.DestroyBeam();
          if (this.State == BasicBeamController.BeamState.Dissipating && (double) this.m_dissipateTimer >= (double) this.dissipateTime)
            this.DestroyBeam();
        }
        if (!this.SelfUpdate)
          return;
        this.FrameUpdate();
      }

      protected override void OnDestroy()
      {
        if ((bool) (UnityEngine.Object) this.m_enemyKnockback)
          this.m_enemyKnockback.EndContinuousKnockback(this.m_enemyKnockbackId);
        this.m_enemyKnockback = (KnockbackDoer) null;
        this.m_enemyKnockbackId = -1;
        if ((bool) (UnityEngine.Object) this.m_reflectedBeam)
        {
          this.m_reflectedBeam.CeaseAttack();
          this.m_reflectedBeam = (BasicBeamController) null;
        }
        base.OnDestroy();
      }

      public void ForceChargeTimer(float val) => this.m_chargeTimer = val;

      public void FrameUpdate()
      {
        try
        {
          SpeculativeRigidbody[] ignoreRigidbodies = this.GetIgnoreRigidbodies();
          int num1 = 0;
          PixelCollider pixelCollider1;
          if (this.State == BasicBeamController.BeamState.Charging)
          {
            this.m_chargeTimer += BraveTime.DeltaTime;
            this.HandleBeamFrame(this.Origin, this.Direction, this.HitsPlayers, this.HitsEnemies, false, out pixelCollider1, ignoreRigidbodies);
          }
          else if (this.State == BasicBeamController.BeamState.Telegraphing)
          {
            this.m_telegraphTimer += BraveTime.DeltaTime;
            this.HandleBeamFrame(this.Origin, this.Direction, this.HitsPlayers, this.HitsEnemies, false, out pixelCollider1, ignoreRigidbodies);
          }
          else if (this.State == BasicBeamController.BeamState.Dissipating)
          {
            this.m_dissipateTimer += BraveTime.DeltaTime;
            this.HandleBeamFrame(this.Origin, this.Direction, this.HitsPlayers, this.HitsEnemies, false, out pixelCollider1, ignoreRigidbodies);
          }
          else
          {
            if (this.State != BasicBeamController.BeamState.Firing && this.State != BasicBeamController.BeamState.Disconnected)
              return;
            PixelCollider pixelCollider2;
            List<SpeculativeRigidbody> speculativeRigidbodyList = this.HandleBeamFrame(this.Origin, this.Direction, this.HitsPlayers, this.HitsEnemies, false, out pixelCollider2, ignoreRigidbodies);
            if (speculativeRigidbodyList != null && speculativeRigidbodyList.Count > 0)
            {
              float num2 = this.projectile.baseData.damage + this.DamageModifier;
              PlayerController owner1 = this.projectile.Owner as PlayerController;
              if ((bool) (UnityEngine.Object) owner1)
                num2 *= owner1.stats.GetStatValue(PlayerStats.StatType.RateOfFire);
              if (this.ChanceBasedShadowBullet)
                num2 *= 2f;
              for (int index1 = 0; index1 < speculativeRigidbodyList.Count; ++index1)
              {
                SpeculativeRigidbody hitRigidbody = speculativeRigidbodyList[index1];
                if (this.OverrideHitChecks != null)
                  this.OverrideHitChecks(hitRigidbody, this.Direction);
                else if ((bool) (UnityEngine.Object) hitRigidbody)
                {
                  if (this.Owner is AIActor)
                  {
                    if ((bool) (UnityEngine.Object) hitRigidbody.healthHaver)
                    {
                      if ((bool) (UnityEngine.Object) hitRigidbody.gameActor && hitRigidbody.gameActor is PlayerController)
                      {
                        bool flag1 = (bool) (UnityEngine.Object) this.Owner && (this.Owner as AIActor).IsBlackPhantom;
                        bool isAlive = hitRigidbody.healthHaver.IsAlive;
                        float num3 = (double) this.projectile.baseData.damage != 0.0 ? 0.5f : 0.0f;
                        HealthHaver healthHaver = hitRigidbody.healthHaver;
                        float num4 = num3;
                        Vector2 direction1 = this.Direction;
                        string actorName = (this.Owner as AIActor).GetActorName();
                        PixelCollider pixelCollider3 = pixelCollider2;
                        double damage = (double) num4;
                        Vector2 direction2 = direction1;
                        string sourceName = actorName;
                        int damageCategory = !flag1 ? 0 : 4;
                        PixelCollider hitPixelCollider = pixelCollider3;
                        healthHaver.ApplyDamage((float) damage, direction2, sourceName, damageCategory: (DamageCategory) damageCategory, hitPixelCollider: hitPixelCollider);
                        bool flag2 = isAlive && hitRigidbody.healthHaver.IsDead;
                        if (this.projectile.OnHitEnemy != null)
                          this.projectile.OnHitEnemy(this.projectile, hitRigidbody, flag2);
                      }
                      else
                      {
                        num2 = (!(bool) (UnityEngine.Object) hitRigidbody.aiActor ? this.projectile.baseData.damage : ProjectileData.FixedFallbackDamageToEnemies) + this.DamageModifier;
                        bool isAlive = hitRigidbody.healthHaver.IsAlive;
                        HealthHaver healthHaver = hitRigidbody.healthHaver;
                        float num5 = num2 * BraveTime.DeltaTime;
                        Vector2 direction3 = this.Direction;
                        string actorName = (this.Owner as AIActor).GetActorName();
                        PixelCollider pixelCollider4 = pixelCollider2;
                        double damage = (double) num5;
                        Vector2 direction4 = direction3;
                        string sourceName = actorName;
                        PixelCollider hitPixelCollider = pixelCollider4;
                        healthHaver.ApplyDamage((float) damage, direction4, sourceName, hitPixelCollider: hitPixelCollider);
                        bool flag = isAlive && hitRigidbody.healthHaver.IsDead;
                        if (this.projectile.OnHitEnemy != null)
                          this.projectile.OnHitEnemy(this.projectile, hitRigidbody, flag);
                      }
                    }
                  }
                  else if ((bool) (UnityEngine.Object) hitRigidbody.healthHaver)
                  {
                    float num6 = num2;
                    if (num1 >= 1)
                    {
                      int index2 = Mathf.Clamp(num1 - 1, 0, GameManager.Instance.PierceDamageScaling.Length - 1);
                      num6 *= GameManager.Instance.PierceDamageScaling[index2];
                    }
                    if (hitRigidbody.healthHaver.IsBoss && (bool) (UnityEngine.Object) this.projectile)
                      num6 *= this.projectile.BossDamageMultiplier;
                    if ((bool) (UnityEngine.Object) this.projectile && (double) this.projectile.BlackPhantomDamageMultiplier != 1.0 && (bool) (UnityEngine.Object) hitRigidbody.aiActor && hitRigidbody.aiActor.IsBlackPhantom)
                      num6 *= this.projectile.BlackPhantomDamageMultiplier;
                    bool isAlive = hitRigidbody.healthHaver.IsAlive;
                    string empty = string.Empty;
                    string str1 = !(bool) (UnityEngine.Object) this.projectile ? (!(this.Owner is AIActor) ? this.Owner.ActorName : (this.Owner as AIActor).GetActorName()) : this.projectile.OwnerName;
                    float num7 = num6 * BraveTime.DeltaTime;
                    if (this.angularKnockback)
                    {
                      BasicBeamController.AngularKnockbackTier knockbackTier = this.GetKnockbackTier();
                      if (knockbackTier != null)
                      {
                        num7 = num2 * knockbackTier.damageMultiplier;
                        SpeculativeRigidbody specRigidbody = hitRigidbody.healthHaver.specRigidbody;
                        if (Array.IndexOf<SpeculativeRigidbody>(ignoreRigidbodies, specRigidbody) >= 0)
                          num7 = 0.0f;
                        if ((double) num7 > 0.0)
                        {
                          int num8 = (int) AkSoundEngine.PostEvent("Play_WPN_woodbeam_impact_01", this.gameObject);
                          knockbackTier.hitRigidbodyVFX.SpawnAtPosition((Vector3) hitRigidbody.UnitCenter);
                          if (knockbackTier.additionalAmmoCost > 0 && (bool) (UnityEngine.Object) this.Gun)
                            this.Gun.LoseAmmo(knockbackTier.additionalAmmoCost);
                          if ((bool) (UnityEngine.Object) specRigidbody)
                            this.TimedIgnoreRigidbodies.Add(Tuple.Create<SpeculativeRigidbody, float>(specRigidbody, knockbackTier.ignoreHitRigidbodyTime));
                          else
                            this.TimedIgnoreRigidbodies.Add(Tuple.Create<SpeculativeRigidbody, float>(hitRigidbody, knockbackTier.ignoreHitRigidbodyTime));
                        }
                      }
                    }
                    HealthHaver healthHaver = hitRigidbody.healthHaver;
                    float num9 = num7;
                    Vector2 direction5 = this.Direction;
                    string str2 = str1;
                    CoreDamageTypes damageTypes1 = this.projectile.damageTypes;
                    PixelCollider pixelCollider5 = pixelCollider2;
                    double damage = (double) num9;
                    Vector2 direction6 = direction5;
                    string sourceName = str2;
                    int damageTypes2 = (int) damageTypes1;
                    PixelCollider hitPixelCollider = pixelCollider5;
                    healthHaver.ApplyDamage((float) damage, direction6, sourceName, (CoreDamageTypes) damageTypes2, hitPixelCollider: hitPixelCollider);
                    bool flag = isAlive && hitRigidbody.healthHaver.IsDead;
                    if (this.projectile.OnHitEnemy != null)
                      this.projectile.OnHitEnemy(this.projectile, hitRigidbody, flag);
                    ++num1;
                  }
                  if ((bool) (UnityEngine.Object) hitRigidbody.majorBreakable)
                  {
                    hitRigidbody.majorBreakable.ApplyDamage(num2 * BraveTime.DeltaTime, this.Direction, false);
                    Chest component = hitRigidbody.GetComponent<Chest>();
                    if ((bool) (UnityEngine.Object) component && BraveUtility.EnumFlagsContains((uint) this.projectile.damageTypes, 32U /*0x20*/) > 0 && component.ChestIdentifier == Chest.SpecialChestIdentifier.SECRET_RAINBOW)
                      component.RevealSecretRainbow();
                  }
                  if ((bool) (UnityEngine.Object) hitRigidbody.gameActor)
                  {
                    GameActor gameActor = hitRigidbody.gameActor;
                    gameActor.BeamStatusAmount += BraveTime.DeltaTime * 1.5f;
                    if ((double) gameActor.BeamStatusAmount > (double) this.TimeToStatus || this.Owner is AIActor)
                    {
                      if (this.projectile.AppliesSpeedModifier && (double) UnityEngine.Random.value < (double) BraveMathCollege.SliceProbability(this.statusEffectChance, BraveTime.DeltaTime))
                        gameActor.ApplyEffect((GameActorEffect) this.projectile.speedEffect);
                      if (this.projectile.AppliesPoison && (double) UnityEngine.Random.value < (double) BraveMathCollege.SliceProbability(this.statusEffectChance, BraveTime.DeltaTime))
                        gameActor.ApplyEffect((GameActorEffect) this.projectile.healthEffect);
                      if (this.projectile.AppliesCharm && (double) UnityEngine.Random.value < (double) BraveMathCollege.SliceProbability(this.statusEffectChance, BraveTime.DeltaTime))
                        gameActor.ApplyEffect((GameActorEffect) this.projectile.charmEffect);
                      if (this.projectile.AppliesFire && (double) UnityEngine.Random.value < (double) BraveMathCollege.SliceProbability(this.statusEffectChance, BraveTime.DeltaTime))
                      {
                        if (gameActor is PlayerController)
                        {
                          if (this.projectile.fireEffect.AffectsPlayers)
                            (gameActor as PlayerController).IsOnFire = true;
                        }
                        else
                          gameActor.ApplyEffect((GameActorEffect) this.projectile.fireEffect);
                      }
                      if (this.projectile.AppliesStun && (bool) (UnityEngine.Object) gameActor.behaviorSpeculator && (double) UnityEngine.Random.value < (double) BraveMathCollege.SliceProbability(this.statusEffectChance, BraveTime.DeltaTime))
                        gameActor.behaviorSpeculator.Stun(this.projectile.AppliedStunDuration);
                    }
                    if (this.projectile.AppliesFreeze)
                      gameActor.ApplyEffect((GameActorEffect) this.projectile.freezeEffect, BraveTime.DeltaTime * this.statusEffectAccumulateMultiplier);
                    if (this.projectile.AppliesBleed)
                      gameActor.ApplyEffect((GameActorEffect) this.projectile.bleedEffect, BraveTime.DeltaTime * this.statusEffectAccumulateMultiplier, this.projectile);
                  }
                  if ((bool) (UnityEngine.Object) this.m_beamGoopModifier)
                  {
                    if (this.m_beamGoopModifier.OnlyGoopOnEnemyCollision)
                    {
                      if ((UnityEngine.Object) hitRigidbody.aiActor != (UnityEngine.Object) null)
                        this.m_beamGoopModifier.SpawnCollisionGoop(hitRigidbody.UnitBottomCenter);
                    }
                    else
                      this.m_beamGoopModifier.SpawnCollisionGoop(hitRigidbody.UnitBottomCenter);
                  }
                  if (hitRigidbody.OnHitByBeam != null)
                    hitRigidbody.OnHitByBeam(this);
                  if (this.Owner is PlayerController)
                    (this.Owner as PlayerController).DoPostProcessBeamTick((BeamController) this, hitRigidbody, BraveTime.DeltaTime);
                  if (this.Owner is AIActor)
                  {
                    AIActor owner2 = this.Owner as AIActor;
                    if ((bool) (UnityEngine.Object) owner2.CompanionOwner)
                    {
                      owner2.CompanionOwner.DoPostProcessBeamTick((BeamController) this, hitRigidbody, BraveTime.DeltaTime);
                      if ((bool) (UnityEngine.Object) owner2.CompanionOwner.CurrentGun && owner2.CompanionOwner.CurrentGun.LuteCompanionBuffActive)
                      {
                        if ((double) this.m_currentLuteScaleModifier != 1.75)
                        {
                          this.m_currentLuteScaleModifier = 1.75f;
                          this.ProjectileScale *= this.m_currentLuteScaleModifier;
                        }
                      }
                      else if ((double) this.m_currentLuteScaleModifier != 1.0)
                      {
                        this.ProjectileScale /= this.m_currentLuteScaleModifier;
                        this.m_currentLuteScaleModifier = 1f;
                      }
                    }
                  }
                }
              }
            }
            if (this.angularKnockback)
            {
              if (speculativeRigidbodyList == null || speculativeRigidbodyList.Count <= 0)
                return;
              BasicBeamController.AngularKnockbackTier knockbackTier = this.GetKnockbackTier();
              if (knockbackTier == null)
                return;
              for (int index = 0; index < speculativeRigidbodyList.Count; ++index)
              {
                KnockbackDoer knockbackDoer = speculativeRigidbodyList[index].knockbackDoer;
                if ((bool) (UnityEngine.Object) knockbackDoer)
                {
                  Vector2 vector2 = knockbackDoer.specRigidbody.UnitCenter - this.Origin;
                  if ((double) knockbackTier.minAngularSpeed > 0.0)
                  {
                    if ((double) this.averageAngularVelocity > 0.0)
                      vector2 = vector2.Rotate(90f);
                    else if ((double) this.averageAngularVelocity < 0.0)
                      vector2 = vector2.Rotate(-90f);
                  }
                  knockbackDoer.ApplyKnockback(vector2, this.projectile.baseData.force * knockbackTier.knockbackMultiplier);
                }
              }
            }
            else
            {
              KnockbackDoer knockbackDoer = speculativeRigidbodyList == null || speculativeRigidbodyList.Count <= 0 ? (KnockbackDoer) null : speculativeRigidbodyList[0].knockbackDoer;
              if ((UnityEngine.Object) knockbackDoer != (UnityEngine.Object) this.m_enemyKnockback)
              {
                if ((bool) (UnityEngine.Object) this.m_enemyKnockback)
                  this.m_enemyKnockback.EndContinuousKnockback(this.m_enemyKnockbackId);
                if ((bool) (UnityEngine.Object) knockbackDoer)
                  this.m_enemyKnockbackId = knockbackDoer.ApplyContinuousKnockback(knockbackDoer.specRigidbody.UnitCenter - this.Origin, this.projectile.baseData.force);
                this.m_enemyKnockback = knockbackDoer;
              }
              if ((UnityEngine.Object) this.m_beamGoopModifier != (UnityEngine.Object) null)
                this.HandleGoopFrame(this.m_beamGoopModifier);
              this.HandleIgnitionAndFreezing();
            }
          }
        }
        catch (Exception ex)
        {
          throw new Exception($"Caught BasicBeamController.HandleBeamFrame() exception. i={this.exceptionTracker}, ex={ex.ToString()}");
        }
      }

      private BasicBeamController.AngularKnockbackTier GetKnockbackTier()
      {
        BasicBeamController.AngularKnockbackTier knockbackTier = (BasicBeamController.AngularKnockbackTier) null;
        for (int index = 0; index < this.angularKnockbackTiers.Count && (double) this.angularKnockbackTiers[index].minAngularSpeed < (double) Mathf.Abs(this.averageAngularVelocity); ++index)
          knockbackTier = this.angularKnockbackTiers[index];
        return knockbackTier;
      }

      public List<SpeculativeRigidbody> HandleBeamFrame(
        Vector2 origin,
        Vector2 direction,
        bool hitsPlayers,
        bool hitsEnemies,
        bool hitsProjectiles,
        out PixelCollider pixelCollider,
        params SpeculativeRigidbody[] ignoreRigidbodies)
      {
        this.exceptionTracker = 0;
        float num1 = Mathf.Atan2(direction.y, direction.x) * 57.29578f;
        List<SpeculativeRigidbody> speculativeRigidbodyList1 = new List<SpeculativeRigidbody>();
        pixelCollider = (PixelCollider) null;
        if (!(bool) (UnityEngine.Object) this.m_beamSprite)
          return speculativeRigidbodyList1;
        if (this.ProjectileAndBeamMotionModule is OrbitProjectileMotionModule)
          this.m_currentBeamDistance = (float) (30.0 + 6.2831854820251465 * (double) (this.ProjectileAndBeamMotionModule as OrbitProjectileMotionModule).BeamOrbitRadius);
        float num2 = 0.0f;
        if (this.angularKnockback)
        {
          float angle = direction.ToAngle();
          if ((double) angle <= 155.0 && (double) angle >= 25.0)
          {
            num2 = -1f;
            if (!this.m_hasToggledGunOutline && (bool) (UnityEngine.Object) this.Gun && (bool) (UnityEngine.Object) this.Gun.GetSprite())
            {
              this.m_hasToggledGunOutline = true;
              SpriteOutlineManager.RemoveOutlineFromSprite(this.Gun.GetSprite());
            }
          }
          else if (this.m_hasToggledGunOutline)
          {
            this.m_hasToggledGunOutline = false;
            SpriteOutlineManager.AddOutlineToSprite(this.Gun.GetSprite(), Color.black);
          }
        }
        this.m_beamSprite.HeightOffGround = BasicBeamController.CurrentBeamHeightOffGround + this.HeightOffset + num2;
        if (this.IsConnected && this.Owner is PlayerController && this.HandleChanceTick() && (double) this.m_chanceTick < -999.0)
        {
          this.m_bones.First.Value.HomingRadius = this.HomingRadius;
          this.m_bones.First.Value.HomingAngularVelocity = this.HomingAngularVelocity;
          this.m_bones.Last.Value.HomingRadius = this.HomingRadius;
          this.m_bones.Last.Value.HomingAngularVelocity = this.HomingAngularVelocity;
          this.m_chanceTick = 1f;
        }
        float z1 = origin.y - BasicBeamController.CurrentBeamHeightOffGround;
        this.transform.position = ((Vector3) origin).WithZ(z1).Quantize(1f / 16f);
        if (!this.m_previousAngle.HasValue)
          this.m_previousAngle = new float?(num1);
        if (this.angularKnockback && (double) BraveTime.DeltaTime > 0.0)
          this.averageAngularVelocity = BraveMathCollege.MovingAverageSpeed(this.averageAngularVelocity, BraveMathCollege.ClampAngle180(num1 - this.m_previousAngle.Value) / BraveTime.DeltaTime, BraveTime.DeltaTime, this.angularSpeedAvgWindow);
        if (this.State == BasicBeamController.BeamState.Charging)
        {
          if (this.UsesChargeSprite && this.rotateChargeAnimation)
            this.m_beamMuzzleAnimator.transform.rotation = Quaternion.Euler(0.0f, 0.0f, direction.ToAngle());
          if ((double) this.m_chargeTimer >= (double) this.chargeDelay)
          {
            this.GetComponent<Renderer>().enabled = true;
            if (this.UsesChargeSprite && this.rotateChargeAnimation)
              this.m_beamMuzzleAnimator.transform.rotation = Quaternion.identity;
            if (this.boneType == BasicBeamController.BeamBoneType.Projectile)
            {
              this.m_bones.First.Value.Position = this.Origin;
              this.m_bones.First.Value.Velocity = this.Direction.normalized * this.BoneSpeed;
              this.m_bones.Last.Value.Position = this.Origin;
              this.m_bones.Last.Value.Velocity = this.Direction.normalized * this.BoneSpeed;
            }
            if (this.usesTelegraph)
            {
              this.State = BasicBeamController.BeamState.Telegraphing;
              if ((bool) (UnityEngine.Object) this.m_beamMuzzleSprite)
                this.m_muzzleTransform.gameObject.SetActive(false);
            }
            else
            {
              if (this.UsesMuzzleSprite)
              {
                this.m_muzzleTransform.gameObject.SetActive(true);
                this.m_beamMuzzleAnimator.Play(this.muzzleAnimation);
              }
              else if ((bool) (UnityEngine.Object) this.m_beamMuzzleSprite)
                this.m_muzzleTransform.gameObject.SetActive(false);
              this.State = BasicBeamController.BeamState.Firing;
            }
          }
        }
        else
        {
          if (this.State == BasicBeamController.BeamState.Telegraphing && (double) this.m_telegraphTimer > (double) this.telegraphTime)
          {
            this.State = BasicBeamController.BeamState.Firing;
            this.spriteAnimator.Play(this.CurrentBeamAnimation);
            if (this.UsesMuzzleSprite)
            {
              this.m_beamMuzzleAnimator.Play(this.muzzleAnimation);
              this.m_muzzleTransform.gameObject.SetActive(true);
            }
          }
          int num3 = 0;
          if (this.boneType == BasicBeamController.BeamBoneType.Straight)
          {
            if ((double) this.BoneSpeed > 0.0)
            {
              float num4 = BraveTime.DeltaTime * this.BoneSpeed;
              this.m_currentBeamDistance = Mathf.Min(this.m_currentBeamDistance + num4, this.projectile.baseData.range);
              for (LinkedListNode<BasicBeamController.BeamBone> linkedListNode = this.m_bones.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
                linkedListNode.Value.PosX = Mathf.Min(linkedListNode.Value.PosX + num4, this.m_currentBeamDistance);
              this.m_bones.First.Value.PosX = Mathf.Max(0.0f, this.m_bones.First.Next.Value.PosX - this.m_beamQuadUnitWidth);
              while ((double) this.m_bones.First.Value.PosX != 0.0)
              {
                int subtileNum = this.m_bones.First.Value.SubtileNum - 1;
                if (subtileNum < 0)
                  subtileNum = this.m_beamSpriteSubtileWidth - 1;
                this.m_bones.AddFirst(new BasicBeamController.BeamBone(Mathf.Max(0.0f, this.m_bones.First.Value.PosX - this.m_beamQuadUnitWidth), 0.0f, subtileNum));
                ++num3;
              }
              while (this.m_bones.Count > 2 && (double) this.m_bones.Last.Previous.Value.PosX == (double) this.m_currentBeamDistance)
                this.m_bones.RemoveLast();
              if (this.TileType == BasicBeamController.BeamTileType.Flowing)
              {
                this.m_uvOffset -= num4 / this.m_beamSpriteUnitWidth;
                while ((double) this.m_uvOffset < 0.0)
                  ++this.m_uvOffset;
              }
            }
            else if ((double) this.BoneSpeed <= 0.0)
              this.m_currentBeamDistance = this.projectile.baseData.range;
          }
          else if (this.boneType == BasicBeamController.BeamBoneType.Projectile)
          {
            float b = BraveTime.DeltaTime * this.BoneSpeed;
            this.m_currentBeamDistance = Mathf.Min(this.m_currentBeamDistance + b, this.projectile.baseData.range);
            LinkedListNode<BasicBeamController.BeamBone> linkedListNode1 = this.m_bones.First;
            bool flag = false;
            for (; linkedListNode1 != null; linkedListNode1 = linkedListNode1.Next)
            {
              linkedListNode1.Value.ApplyHoming(this.ReflectedFromRigidbody);
              linkedListNode1.Value.PosX = Mathf.Min(linkedListNode1.Value.PosX + b, this.m_currentBeamDistance);
              linkedListNode1.Value.Position += linkedListNode1.Value.Velocity * BraveTime.DeltaTime;
              if (linkedListNode1.Value.HomingDampenMotion)
                flag = true;
            }
            if (flag)
            {
              LinkedListNode<BasicBeamController.BeamBone> next = this.m_bones.First.Next;
              Vector2 vector2_1 = this.m_bones.First.Value.Position;
              Vector2 vector2_2 = this.m_bones.First.Value.Velocity;
              for (; next != null; next = next.Next)
              {
                if (next.Next != null)
                {
                  Vector2 position1 = next.Next.Value.Position;
                  Vector2 velocity1 = next.Next.Value.Velocity;
                  Vector2 position2 = next.Value.Position;
                  Vector2 velocity2 = next.Value.Velocity;
                  if (next.Value.HomingDampenMotion)
                  {
                    next.Value.Position = 0.2f * position2 + 0.4f * position1 + 0.4f * vector2_1;
                    next.Value.Velocity = 0.2f * velocity2 + 0.4f * velocity1 + 0.4f * vector2_2;
                  }
                  vector2_1 = position2;
                  vector2_2 = velocity2;
                }
              }
            }
            if (this.interpolateStretchedBones && this.m_bones.Count > 1)
            {
              LinkedListNode<BasicBeamController.BeamBone> next = this.m_bones.First.Next;
              LinkedListNode<BasicBeamController.BeamBone> linkedListNode2 = (LinkedListNode<BasicBeamController.BeamBone>) null;
              for (; next != null; next = next.Next)
              {
                if ((double) Vector2.SqrMagnitude(next.Value.Position - next.Previous.Value.Position) > (double) this.m_sqrNewBoneThreshold)
                {
                  BasicBeamController.BeamBone beamBone = new BasicBeamController.BeamBone((float) (((double) next.Previous.Value.PosX + (double) next.Value.PosX) / 2.0), next.Value.RotationAngle, next.Value.SubtileNum);
                  if (next.Previous.Previous != null && next.Next != null)
                  {
                    Vector2 position3 = next.Previous.Previous.Value.Position;
                    Vector2 position4 = next.Next.Value.Position;
                    Vector2 position5 = next.Previous.Value.Position;
                    Vector2 p1 = position5 + (position5 - position3).normalized * this.m_beamQuadUnitWidth;
                    Vector2 position6 = next.Value.Position;
                    Vector2 p2 = position6 + (position6 - position4).normalized * this.m_beamQuadUnitWidth;
                    beamBone.Position = BraveMathCollege.CalculateBezierPoint(0.5f, position5, p1, p2, position6);
                  }
                  else
                    beamBone.Position = (next.Previous.Value.Position + next.Value.Position) / 2f;
                  beamBone.Velocity = (next.Previous.Value.Velocity + next.Value.Velocity) / 2f;
                  linkedListNode2 = this.m_bones.AddBefore(next, beamBone);
                }
              }
              if (linkedListNode2 != null)
              {
                for (LinkedListNode<BasicBeamController.BeamBone> linkedListNode3 = linkedListNode2; linkedListNode3 != null; linkedListNode3 = linkedListNode3.Previous)
                  linkedListNode3.Value.SubtileNum = linkedListNode3.Next.Value.SubtileNum != 0 ? linkedListNode3.Next.Value.SubtileNum - 1 : this.m_beamSpriteSubtileWidth - 1;
              }
            }
            if (this.State == BasicBeamController.BeamState.Telegraphing || this.State == BasicBeamController.BeamState.Firing || this.State == BasicBeamController.BeamState.Dissipating)
            {
              Vector2 origin1 = this.Origin;
              Vector2 position = this.m_bones.First.Value.Position;
              if (this.IsHoming)
                this.m_previousAngle = new float?(this.m_bones.First.Value.Velocity.ToAngle());
              float num5 = Mathf.Max(0.0f, this.m_bones.First.Next.Value.PosX - this.m_beamQuadUnitWidth);
              float t1 = Mathf.InverseLerp(0.0f, b, num5);
              float angle1 = this.m_previousAngle.Value + Mathf.Lerp(BraveMathCollege.ClampAngle180(num1 - this.m_previousAngle.Value), 0.0f, t1);
              this.m_bones.First.Value.PosX = num5;
              this.m_bones.First.Value.Position = Vector2.Lerp(origin1, position, Mathf.InverseLerp(0.0f, b, this.m_bones.First.Value.PosX));
              this.m_bones.First.Value.Velocity = BraveMathCollege.DegreesToVector(angle1, this.BoneSpeed);
              this.m_bones.First.Value.HomingRadius = this.HomingRadius;
              this.m_bones.First.Value.HomingAngularVelocity = this.HomingAngularVelocity;
              while ((double) this.m_bones.First.Value.PosX != 0.0)
              {
                int subtileNum = this.m_bones.First.Value.SubtileNum != 0 ? this.m_bones.First.Value.SubtileNum - 1 : this.m_beamSpriteSubtileWidth - 1;
                float posX = Mathf.Max(0.0f, this.m_bones.First.Value.PosX - this.m_beamQuadUnitWidth);
                float t2 = Mathf.InverseLerp(0.0f, b, posX);
                float angle2 = this.m_previousAngle.Value + Mathf.Lerp(BraveMathCollege.ClampAngle180(num1 - this.m_previousAngle.Value), 0.0f, t2);
                this.m_bones.AddFirst(new BasicBeamController.BeamBone(posX, 0.0f, subtileNum));
                this.m_bones.First.Value.Position = Vector2.Lerp(origin1, position, t2);
                this.m_bones.First.Value.Velocity = BraveMathCollege.DegreesToVector(angle2, this.BoneSpeed);
                this.m_bones.First.Value.HomingRadius = this.HomingRadius;
                this.m_bones.First.Value.HomingAngularVelocity = this.HomingAngularVelocity;
                ++num3;
              }
              if (this.TileType == BasicBeamController.BeamTileType.Flowing)
              {
                this.m_uvOffset -= b / this.m_beamSpriteUnitWidth;
                while ((double) this.m_uvOffset < 0.0)
                  ++this.m_uvOffset;
              }
            }
            else if (this.State == BasicBeamController.BeamState.Disconnected)
            {
              if ((double) this.decayNear > 0.0)
              {
                float num6 = this.m_bones.First.Value.PosX + this.decayNear * BraveTime.DeltaTime;
                this.m_bones.First.Value.PosX = num6;
                for (LinkedListNode<BasicBeamController.BeamBone> next = this.m_bones.First.Next; next != null && (double) next.Value.PosX < (double) num6; next = next.Next)
                  next.Value.PosX = num6;
              }
              if ((double) this.decayFar > 0.0)
              {
                this.m_currentBeamDistance -= this.decayFar * BraveTime.DeltaTime;
                for (LinkedListNode<BasicBeamController.BeamBone> linkedListNode4 = this.m_bones.Last; linkedListNode4 != null && (double) linkedListNode4.Value.PosX >= (double) this.m_currentBeamDistance; linkedListNode4 = linkedListNode4.Previous)
                  linkedListNode4.Value.PosX = this.m_currentBeamDistance;
              }
            }
            float posX1 = this.m_bones.First.Value.PosX;
            while (this.m_bones.Count > 2 && (double) this.m_bones.First.Next.Value.PosX <= (double) posX1)
              this.m_bones.RemoveFirst();
            while (this.m_bones.Count > 2 && (double) this.m_bones.Last.Previous.Value.PosX >= (double) this.m_currentBeamDistance)
              this.m_bones.RemoveLast();
          }
          if (this.UsesBones && (this.State == BasicBeamController.BeamState.Telegraphing || this.State == BasicBeamController.BeamState.Firing))
          {
            if (this.boneType == BasicBeamController.BeamBoneType.Straight)
            {
              this.m_bones.Clear();
              DungeonData data = GameManager.Instance.Dungeon.data;
              float posX = 0.0f;
              Vector2 position = origin;
              float angle = num1;
              float magnitude = (double) this.BoneSpeed >= 0.0 ? this.BoneSpeed : 40f;
              float overrideDeltaTime = this.m_beamQuadUnitWidth / magnitude;
              this.m_bones.AddLast(new BasicBeamController.BeamBone(posX, position, BraveMathCollege.DegreesToVector(angle, magnitude)));
              while ((double) posX < (double) this.m_currentBeamDistance)
              {
                posX = Mathf.Min(posX + this.m_beamQuadUnitWidth, this.m_currentBeamDistance);
                BasicBeamController.BeamBone bone = new BasicBeamController.BeamBone(posX, position, BraveMathCollege.DegreesToVector(angle, magnitude));
                bone.HomingRadius = this.HomingRadius;
                bone.HomingAngularVelocity = this.HomingAngularVelocity;
                this.m_bones.AddLast(bone);
                bone.ApplyHoming(overrideDeltaTime: overrideDeltaTime);
                bone.Position += bone.Velocity * overrideDeltaTime;
                position = bone.Position;
                if (this.ProjectileAndBeamMotionModule != null && this.ProjectileAndBeamMotionModule is OrbitProjectileMotionModule)
                  position += this.ProjectileAndBeamMotionModule.GetBoneOffset(bone, (BeamController) this, this.projectile.Inverted);
                angle = bone.Velocity.ToAngle();
                if ((double) posX > (double) this.IgnoreTilesDistance && data.isWall((int) position.x, (int) position.y) && !data.isAnyFaceWall((int) position.x, (int) position.y))
                {
                  this.m_currentBeamDistance = posX;
                  break;
                }
              }
            }
            for (LinkedListNode<BasicBeamController.BeamBone> linkedListNode = this.m_bones.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
            {
              Vector2 zero = Vector2.zero;
              if (linkedListNode.Next != null)
                zero += (linkedListNode.Next.Value.Position - linkedListNode.Value.Position).normalized;
              if (linkedListNode.Previous != null)
                zero += (linkedListNode.Value.Position - linkedListNode.Previous.Value.Position).normalized;
              linkedListNode.Value.RotationAngle = !(zero != Vector2.zero) ? 0.0f : BraveMathCollege.Atan2Degrees(zero);
            }
          }
          int mask1 = CollisionLayerMatrix.GetMask(CollisionLayer.Projectile);
          if (!hitsPlayers)
            mask1 &= ~CollisionMask.LayerToMask(CollisionLayer.PlayerHitBox, CollisionLayer.EnemyBulletBlocker);
          if (!hitsEnemies)
            mask1 &= ~CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox);
          if (hitsProjectiles)
            mask1 |= CollisionMask.LayerToMask(CollisionLayer.Projectile);
          int collisionMask = mask1 | CollisionMask.LayerToMask(CollisionLayer.BeamBlocker);
          if (this.m_pierceImpactSprites != null)
          {
            for (int index = 0; index < this.m_pierceImpactSprites.Count; ++index)
              this.m_pierceImpactSprites[index].gameObject.SetActive(false);
          }
          int index1 = 0;
          int penetration = this.penetration;
          UltraFortunesFavor ultraFortunesFavor = (UltraFortunesFavor) null;
          SpeculativeRigidbody speculativeRigidbody = (SpeculativeRigidbody) null;
          List<SpeculativeRigidbody> speculativeRigidbodyList2 = new List<SpeculativeRigidbody>((IEnumerable<SpeculativeRigidbody>) ignoreRigidbodies);
          int num7 = 0;
          Vector2 targetPoint;
          Vector2 targetNormal;
          SpeculativeRigidbody hitRigidbody;
          List<PointcastResult> boneCollisions;
          bool beamTarget;
          bool flag1;
          bool flag2;
          do
          {
            beamTarget = this.FindBeamTarget(origin, direction, this.m_currentBeamDistance, collisionMask, out targetPoint, out targetNormal, out hitRigidbody, out pixelCollider, out boneCollisions, (Func<SpeculativeRigidbody, bool>) null, speculativeRigidbodyList2.ToArray());
            flag1 = beamTarget;
            flag2 = beamTarget && (bool) (UnityEngine.Object) hitRigidbody;
            if (beamTarget && !(bool) (UnityEngine.Object) hitRigidbody && (bool) (UnityEngine.Object) this.m_beamGoopModifier && !this.m_beamGoopModifier.OnlyGoopOnEnemyCollision)
            {
              Vector3 pos = (Vector3) targetPoint;
              if ((double) targetNormal.y < 0.800000011920929)
                --pos.y;
              this.m_beamGoopModifier.SpawnCollisionGoop((Vector2) pos);
            }
            if (beamTarget && (bool) (UnityEngine.Object) hitRigidbody)
            {
              ultraFortunesFavor = hitRigidbody.ultraFortunesFavor;
              if ((bool) (UnityEngine.Object) ultraFortunesFavor)
                hitRigidbody = (SpeculativeRigidbody) null;
              if ((bool) (UnityEngine.Object) hitRigidbody && hitRigidbody.ReflectBeams)
              {
                speculativeRigidbody = hitRigidbody;
                hitRigidbody = (SpeculativeRigidbody) null;
                pixelCollider = (PixelCollider) null;
                flag2 = false;
              }
              if ((bool) (UnityEngine.Object) hitRigidbody && hitRigidbody.BlockBeams)
              {
                hitRigidbody = (SpeculativeRigidbody) null;
                pixelCollider = (PixelCollider) null;
                flag2 = false;
              }
              if ((bool) (UnityEngine.Object) hitRigidbody && hitRigidbody.PreventPiercing)
              {
                if (!(bool) (UnityEngine.Object) hitRigidbody.healthHaver && !(bool) (UnityEngine.Object) hitRigidbody.majorBreakable)
                  hitRigidbody = (SpeculativeRigidbody) null;
                flag2 = false;
              }
              if (hitsPlayers && (bool) (UnityEngine.Object) hitRigidbody)
              {
                PlayerController component = hitRigidbody.GetComponent<PlayerController>();
                if ((bool) (UnityEngine.Object) component && (component.spriteAnimator.QueryInvulnerabilityFrame() || !component.healthHaver.IsVulnerable || component.IsEthereal))
                {
                  speculativeRigidbodyList2.Add(hitRigidbody);
                  hitRigidbody = (SpeculativeRigidbody) null;
                  ++penetration;
                  component.HandleDodgedBeam((BeamController) this);
                }
              }
              if ((bool) (UnityEngine.Object) hitRigidbody && (bool) (UnityEngine.Object) hitRigidbody.minorBreakable)
              {
                if (targetNormal != Vector2.zero)
                  hitRigidbody.minorBreakable.Break(-targetNormal);
                else
                  hitRigidbody.minorBreakable.Break();
                if ((bool) (UnityEngine.Object) this.m_beamGoopModifier && !this.m_beamGoopModifier.OnlyGoopOnEnemyCollision)
                  this.m_beamGoopModifier.SpawnCollisionGoop(hitRigidbody.UnitBottomCenter);
                hitRigidbody = (SpeculativeRigidbody) null;
                ++penetration;
              }
              if ((bool) (UnityEngine.Object) hitRigidbody && pixelCollider != null && pixelCollider.CollisionLayer == CollisionLayer.BeamBlocker && (bool) (UnityEngine.Object) hitRigidbody.GetComponent<TorchController>())
              {
                speculativeRigidbodyList2.Add(hitRigidbody);
                hitRigidbody = (SpeculativeRigidbody) null;
                ++penetration;
              }
              if (this.PenetratesCover && (bool) (UnityEngine.Object) hitRigidbody && (bool) (UnityEngine.Object) hitRigidbody.majorBreakable && (bool) (UnityEngine.Object) hitRigidbody.transform.parent && (bool) (UnityEngine.Object) hitRigidbody.transform.parent.GetComponent<FlippableCover>())
              {
                flag2 = true;
                ++penetration;
              }
              if ((bool) (UnityEngine.Object) hitRigidbody)
              {
                speculativeRigidbodyList1.Add(hitRigidbody);
                speculativeRigidbodyList2.Add(hitRigidbody);
              }
              --penetration;
              if ((bool) (UnityEngine.Object) hitRigidbody && penetration >= 0 && !string.IsNullOrEmpty(this.impactAnimation))
              {
                if (this.m_pierceImpactSprites == null)
                  this.m_pierceImpactSprites = new List<tk2dBaseSprite>();
                if (index1 >= this.m_pierceImpactSprites.Count)
                  this.m_pierceImpactSprites.Add(this.CreatePierceImpactEffect());
                tk2dBaseSprite pierceImpactSprite = this.m_pierceImpactSprites[index1];
                pierceImpactSprite.gameObject.SetActive(true);
                float z2 = Mathf.Atan2(targetNormal.y, targetNormal.x) * 57.29578f;
                pierceImpactSprite.transform.rotation = Quaternion.Euler(0.0f, 0.0f, z2);
                pierceImpactSprite.transform.position = (Vector3) targetPoint;
                bool flag3 = (double) targetNormal.y < -0.5;
                pierceImpactSprite.IsPerpendicular = flag3;
                pierceImpactSprite.HeightOffGround = !flag3 ? 0.05f : 2f;
                pierceImpactSprite.UpdateZDepth();
                ++index1;
              }
              if ((bool) (UnityEngine.Object) hitRigidbody && (bool) (UnityEngine.Object) hitRigidbody.hitEffectHandler)
              {
                HitEffectHandler hitEffectHandler = hitRigidbody.hitEffectHandler;
                if (hitEffectHandler.additionalHitEffects != null && hitEffectHandler.additionalHitEffects.Length > 0)
                  hitEffectHandler.HandleAdditionalHitEffects(BraveMathCollege.DegreesToVector(this.Direction.ToAngle(), 8f), pixelCollider);
              }
              if ((bool) (UnityEngine.Object) hitRigidbody && hitRigidbody.OnBeamCollision != null)
                hitRigidbody.OnBeamCollision((BeamController) this);
            }
            ++num7;
          }
          while (flag2 && penetration >= 0 && num7 < 100);
          if (num7 >= 100)
            UnityEngine.Debug.LogErrorFormat("Infinite loop averted!  TELL RUBEL! {0} {1}", (object) this.Owner, (object) this);
          if (beamTarget && (bool) (UnityEngine.Object) hitRigidbody && (bool) (UnityEngine.Object) hitRigidbody.gameActor && this.ContinueBeamArtToWall && !hitRigidbody.BlockBeams)
          {
            int mask2 = CollisionMask.LayerToMask(CollisionLayer.HighObstacle, CollisionLayer.BulletBlocker);
            Func<SpeculativeRigidbody, bool> rigidbodyExcluder = (Func<SpeculativeRigidbody, bool>) (specRigidbody => (bool) (UnityEngine.Object) specRigidbody.gameActor);
            beamTarget = this.FindBeamTarget(origin, direction, this.m_currentBeamDistance, mask2, out targetPoint, out targetNormal, out SpeculativeRigidbody _, out PixelCollider _, out boneCollisions, rigidbodyExcluder, speculativeRigidbodyList2.ToArray());
          }
          if (beamTarget)
          {
            bool flag4 = false;
            Vector2 vector2_3 = new Vector2(-1f, -1f);
            Vector2 vector2_4 = Vector2.zero;
            HitEffectHandler hitEffectHandler = !((UnityEngine.Object) hitRigidbody != (UnityEngine.Object) null) ? (HitEffectHandler) null : hitRigidbody.hitEffectHandler;
            if (this.UsesBones)
            {
              int index2 = 0;
              bool flag5 = false;
              if (boneCollisions[index2].hitDirection == HitDirection.Forward && boneCollisions[index2].boneIndex == 0)
              {
                ++index2;
                if (boneCollisions.Count == 1)
                  flag5 = true;
              }
              if (flag5 || boneCollisions[index2].hitDirection == HitDirection.Backward)
              {
                Vector2 contact;
                float num8;
                LinkedListNode<BasicBeamController.BeamBone> linkedListNode5;
                if (flag5)
                {
                  contact = boneCollisions[0].hitResult.Contact;
                  num8 = this.m_bones.First.Value.PosX;
                  linkedListNode5 = this.m_bones.Last;
                }
                else
                {
                  LinkedListNode<BasicBeamController.BeamBone> linkedListNode6 = this.m_bones.First;
                  for (int index3 = 0; index3 < boneCollisions[index2].boneIndex; ++index3)
                    linkedListNode6 = linkedListNode6.Next;
                  contact = boneCollisions[index2].hitResult.Contact;
                  num8 = Mathf.Lerp(linkedListNode6.Value.PosX, linkedListNode6.Previous.Value.PosX, Mathf.Clamp01(Vector2.Distance(linkedListNode6.Value.Position, contact) / Vector2.Distance(linkedListNode6.Value.Position, linkedListNode6.Previous.Value.Position)));
                  linkedListNode5 = linkedListNode6.Previous;
                  flag4 = true;
                  vector2_3 = contact;
                  vector2_4 = boneCollisions[index2].hitResult.Normal;
                }
                for (; linkedListNode5 != null; linkedListNode5 = linkedListNode5.Previous)
                {
                  linkedListNode5.Value.PosX = num8;
                  linkedListNode5.Value.Position = contact;
                }
                flag1 = false;
                ++index2;
              }
              if (index2 < boneCollisions.Count)
              {
                if (boneCollisions[index2].hitDirection != HitDirection.Forward)
                  UnityEngine.Debug.LogError((object) "WTF?");
                LinkedListNode<BasicBeamController.BeamBone> linkedListNode = this.m_bones.First;
                for (int index4 = 0; index4 < boneCollisions[index2].boneIndex; ++index4)
                  linkedListNode = linkedListNode.Next;
                float t3 = 1f;
                if (linkedListNode.Next != null)
                  t3 = Mathf.Clamp01(Vector2.Distance(linkedListNode.Value.Position, targetPoint) / Vector2.Distance(linkedListNode.Value.Position, linkedListNode.Next.Value.Position));
                if (index2 + 1 < boneCollisions.Count && this.collisionSeparation && !(this.ProjectileAndBeamMotionModule is OrbitProjectileMotionModule))
                {
                  int index5 = index2 + 1;
                  LinkedListNode<BasicBeamController.BeamBone> startNode = this.m_bones.First;
                  for (int index6 = 0; index6 < boneCollisions[index5].boneIndex; ++index6)
                    startNode = startNode.Next;
                  Vector2 contact = boneCollisions[index5].hitResult.Contact;
                  float t4 = Mathf.Clamp01(Vector2.Distance(startNode.Value.Position, contact) / Vector2.Distance(startNode.Value.Position, startNode.Previous.Value.Position));
                  float newPosX = Mathf.Lerp(startNode.Value.PosX, startNode.Previous.Value.PosX, t4);
                  this.SeparateBeam(startNode, contact, newPosX);
                  flag1 = true;
                }
                this.m_currentBeamDistance = linkedListNode.Next == null ? linkedListNode.Value.PosX : Mathf.Lerp(linkedListNode.Value.PosX, linkedListNode.Next.Value.PosX, t3);
              }
            }
            else
            {
              this.m_currentBeamDistance = (targetPoint - origin).magnitude;
              if (this.m_bones.Count == 2)
              {
                this.m_bones.First.Value.PosX = 0.0f;
                this.m_bones.Last.Value.PosX = this.m_currentBeamDistance;
              }
            }
            for (LinkedListNode<BasicBeamController.BeamBone> linkedListNode = this.m_bones.Last; linkedListNode != null && (double) linkedListNode.Value.PosX >= (double) this.m_currentBeamDistance; linkedListNode = linkedListNode.Previous)
              linkedListNode.Value.PosX = Mathf.Min(this.m_currentBeamDistance, linkedListNode.Value.PosX);
            while (this.m_bones.Count > 2 && (double) this.m_bones.Last.Previous.Value.PosX == (double) this.m_currentBeamDistance)
              this.m_bones.RemoveLast();
            bool flag6 = (!((UnityEngine.Object) hitEffectHandler != (UnityEngine.Object) null) ? 0 : (hitEffectHandler.SuppressAllHitEffects ? 1 : 0)) == 0;
            if (this.UsesImpactSprite)
            {
              if (this.State != BasicBeamController.BeamState.Telegraphing && flag6)
              {
                if (!this.m_impactTransform.gameObject.activeSelf)
                  this.m_impactTransform.gameObject.SetActive(true);
                this.m_impactTransform.rotation = Quaternion.Euler(0.0f, 0.0f, Mathf.Atan2(targetNormal.y, targetNormal.x) * 57.29578f);
                this.m_impactTransform.position = (Vector3) targetPoint;
                bool flag7 = (double) targetNormal.y < -0.5;
                this.m_impactSprite.IsPerpendicular = flag7;
                this.m_impactSprite.HeightOffGround = !flag7 ? 0.05f : 2f;
                this.m_impactSprite.UpdateZDepth();
                if ((bool) (UnityEngine.Object) this.m_impact2Transform && flag4)
                {
                  if (!this.m_impact2Transform.gameObject.activeSelf)
                    this.m_impact2Transform.gameObject.SetActive(true);
                  this.m_impact2Transform.rotation = Quaternion.Euler(0.0f, 0.0f, Mathf.Atan2(vector2_4.y, vector2_4.x) * 57.29578f);
                  this.m_impact2Transform.position = (Vector3) vector2_3;
                  bool flag8 = (double) vector2_4.y < -0.5;
                  this.m_impact2Sprite.IsPerpendicular = flag8;
                  this.m_impact2Sprite.HeightOffGround = !flag8 ? 0.05f : 2f;
                  this.m_impact2Sprite.UpdateZDepth();
                }
              }
              else if (this.UsesImpactSprite)
              {
                if (this.m_impactTransform.gameObject.activeSelf)
                  this.m_impactTransform.gameObject.SetActive(false);
                if (this.m_impact2Transform.gameObject.activeSelf)
                  this.m_impact2Transform.gameObject.SetActive(false);
              }
            }
          }
          else if (this.ShowImpactOnMaxDistanceEnd)
          {
            if (!this.m_impactTransform.gameObject.activeSelf)
              this.m_impactTransform.gameObject.SetActive(true);
            Vector2 vector2 = this.m_bones.Count < 2 || !this.UsesBones ? this.Origin + this.Direction.normalized * this.m_currentBeamDistance : this.GetBonePosition(this.m_bones.Last.Value);
            this.m_impactTransform.rotation = Quaternion.identity;
            this.m_impactTransform.position = (Vector3) vector2;
            this.m_impactSprite.IsPerpendicular = false;
            this.m_impactSprite.HeightOffGround = 0.05f;
            this.m_impactSprite.UpdateZDepth();
            if (this.m_impact2Transform.gameObject.activeSelf)
              this.m_impact2Transform.gameObject.SetActive(false);
          }
          else if (this.UsesImpactSprite)
          {
            if (this.m_impactTransform.gameObject.activeSelf)
              this.m_impactTransform.gameObject.SetActive(false);
            if (this.m_impact2Transform.gameObject.activeSelf)
              this.m_impact2Transform.gameObject.SetActive(false);
          }
          if (this.UsesDispersalParticles)
          {
            if (this.boneType == BasicBeamController.BeamBoneType.Straight)
            {
              Vector2 bonePosition = this.GetBonePosition(this.m_bones.First.Value);
              Vector2 vector = bonePosition + BraveMathCollege.DegreesToVector(this.transform.eulerAngles.z).normalized * this.m_currentBeamDistance;
              this.DoDispersalParticles(bonePosition.ToVector3ZisY(), vector.ToVector3ZisY(), beamTarget);
            }
            else
            {
              for (LinkedListNode<BasicBeamController.BeamBone> boneNode = this.m_bones.First; boneNode != null; boneNode = boneNode.Next)
                this.DoDispersalParticles(boneNode, boneNode.Value.SubtileNum, beamTarget);
            }
          }
          this.exceptionTracker = 0;
          if ((this.reflections > 0 || (bool) (UnityEngine.Object) ultraFortunesFavor || (bool) (UnityEngine.Object) speculativeRigidbody) && flag1)
          {
            this.exceptionTracker = 100;
            if ((double) targetNormal.x == 0.0 && (double) targetNormal.y == 0.0)
              targetNormal = this.m_lastHitNormal;
            else
              this.m_lastHitNormal = targetNormal;
            this.exceptionTracker = 101;
            float num9 = BraveMathCollege.ClampAngle360(direction.ToAngle() + 180f);
            float angle3 = targetNormal.ToAngle();
            if ((bool) (UnityEngine.Object) ultraFortunesFavor)
            {
              angle3 = ultraFortunesFavor.GetBeamNormal(targetPoint).ToAngle();
              ultraFortunesFavor.HitFromPoint(targetPoint);
            }
            this.exceptionTracker = 102;
            if ((bool) (UnityEngine.Object) speculativeRigidbody && speculativeRigidbody.ReflectProjectilesNormalGenerator != null)
              angle3 = speculativeRigidbody.ReflectProjectilesNormalGenerator(targetPoint, targetNormal).ToAngle();
            float angle4 = num9 + 2f * BraveMathCollege.ClampAngle180(angle3 - num9);
            Vector2 vector2 = targetPoint + targetNormal.normalized * PhysicsEngine.PixelToUnit(2);
            if (!(bool) (UnityEngine.Object) this.m_reflectedBeam)
            {
              this.exceptionTracker = 103;
              this.m_reflectedBeam = this.CreateReflectedBeam(vector2, BraveMathCollege.DegreesToVector(angle4), !(bool) (UnityEngine.Object) ultraFortunesFavor);
            }
            else
            {
              this.exceptionTracker = 104;
              this.m_reflectedBeam.Origin = vector2;
              this.m_reflectedBeam.Direction = BraveMathCollege.DegreesToVector(angle4);
              this.m_reflectedBeam.LateUpdatePosition((Vector3) vector2);
            }
            this.exceptionTracker = 1041;
            if ((bool) (UnityEngine.Object) this.m_reflectedBeam)
            {
              this.m_reflectedBeam.penetration = penetration;
              this.exceptionTracker = 39;
              this.m_reflectedBeam.ReflectedFromRigidbody = !(bool) (UnityEngine.Object) speculativeRigidbody ? (SpeculativeRigidbody) null : speculativeRigidbody;
            }
          }
          else
          {
            this.exceptionTracker = 105;
            if ((bool) (UnityEngine.Object) this.m_reflectedBeam)
            {
              this.exceptionTracker = 106;
              this.m_reflectedBeam.CeaseAttack();
              this.m_reflectedBeam = (BasicBeamController) null;
            }
          }
        }
        this.exceptionTracker = 0;
        if (this.doesScreenDistortion)
        {
          if ((UnityEngine.Object) this.m_distortionMaterial == (UnityEngine.Object) null)
            this.m_distortionMaterial = new Material(ShaderCache.Acquire("Brave/Internal/DistortionLine"));
          Pixelator.Instance.RegisterAdditionalRenderPass(this.m_distortionMaterial);
          Vector2 vector = this.m_currentBeamDistance * direction.normalized + origin;
          Vector3 viewportPoint1 = GameManager.Instance.MainCameraController.Camera.WorldToViewportPoint(origin.ToVector3ZUp());
          Vector3 viewportPoint2 = GameManager.Instance.MainCameraController.Camera.WorldToViewportPoint(vector.ToVector3ZUp());
          Vector4 vector4_1 = new Vector4(viewportPoint1.x, viewportPoint1.y, this.startDistortionRadius, this.startDistortionPower);
          Vector4 vector4_2 = new Vector4(viewportPoint2.x, viewportPoint2.y, this.endDistortionRadius, this.endDistortionPower);
          this.m_distortionMaterial.SetVector("_WavePoint1", vector4_1);
          this.m_distortionMaterial.SetVector("_WavePoint2", vector4_2);
          this.m_distortionMaterial.SetFloat("_DistortProgress", (Mathf.Sin(UnityEngine.Time.realtimeSinceStartup * this.distortionPulseSpeed) + 1f) * this.distortionOffsetIncrease + this.minDistortionOffset);
        }
        Vector2 vector2_5 = new Vector2(this.m_currentBeamDistance * 16f, this.m_beamSprite.dimensions.y);
        if (vector2_5 != this.m_beamSprite.dimensions)
          this.m_beamSprite.dimensions = vector2_5;
        else
          this.m_beamSprite.ForceBuild();
        this.m_beamSprite.UpdateZDepth();
        this.m_previousAngle = new float?(num1);
        for (int index = this.TimedIgnoreRigidbodies.Count - 1; index >= 0; --index)
        {
          this.TimedIgnoreRigidbodies[index].Second -= BraveTime.DeltaTime;
          if ((double) this.TimedIgnoreRigidbodies[index].Second <= 0.0)
            this.TimedIgnoreRigidbodies.RemoveAt(index);
        }
        return speculativeRigidbodyList1;
      }

      private void DoDispersalParticles(Vector3 posStart, Vector3 posEnd, bool didImpact)
      {
        int num = Mathf.Max(Mathf.CeilToInt(Vector2.Distance(posStart.XY(), posEnd.XY()) * this.DispersalDensity), 1);
        for (int index = 0; index < num; ++index)
        {
          float t = (float) index / (float) num;
          Vector3 vector3_1 = Vector3.Lerp(posStart, posEnd, t);
          Vector3 vector3_2 = Vector3.Lerp(Quaternion.Euler(0.0f, 0.0f, Mathf.PerlinNoise(vector3_1.x / 3f, vector3_1.y / 3f) * 360f) * Vector3.right, UnityEngine.Random.insideUnitSphere, UnityEngine.Random.Range(this.DispersalMinCoherency, this.DispersalMaxCoherency));
          this.m_dispersalParticles.Emit(new ParticleSystem.EmitParams()
          {
            position = vector3_1,
            velocity = vector3_2 * this.m_dispersalParticles.startSpeed,
            startSize = this.m_dispersalParticles.startSize,
            startLifetime = this.m_dispersalParticles.startLifetime,
            startColor = (Color32) this.m_dispersalParticles.startColor
          }, 1);
        }
      }

      private void DoDispersalParticles(
        LinkedListNode<BasicBeamController.BeamBone> boneNode,
        int subtilesPerTile,
        bool didImpact)
      {
        if (!this.UsesDispersalParticles || boneNode.Value == null || boneNode.Next == null || boneNode.Next.Value == null)
          return;
        bool flag1 = boneNode == this.m_bones.First;
        Vector2 bonePosition1 = this.GetBonePosition(boneNode.Value);
        Vector3 vector3Zup1 = bonePosition1.ToVector3ZUp(bonePosition1.y);
        LinkedListNode<BasicBeamController.BeamBone> next = boneNode.Next;
        Vector2 bonePosition2 = this.GetBonePosition(next.Value);
        Vector3 vector3Zup2 = bonePosition2.ToVector3ZUp(bonePosition2.y);
        bool flag2 = next == this.m_bones.Last && didImpact;
        float num1 = flag1 || flag2 ? 3f : 1f;
        int num2 = Mathf.Max(Mathf.CeilToInt(Vector2.Distance(vector3Zup1.XY(), vector3Zup2.XY()) * this.DispersalDensity * num1), 1);
        if (flag2)
          num2 = Mathf.CeilToInt((float) num2 * this.DispersalExtraImpactFactor);
        for (int index = 0; index < num2; ++index)
        {
          float t = (float) index / (float) num2;
          if (flag1)
            t = Mathf.Lerp(0.0f, 0.5f, t);
          if (flag2)
            t = Mathf.Lerp(0.5f, 1f, t);
          Vector3 vector3_1 = Vector3.Lerp(vector3Zup1, vector3Zup2, t);
          Vector3 vector3_2 = Vector3.Lerp(Quaternion.Euler(0.0f, 0.0f, Mathf.PerlinNoise(vector3_1.x / 3f, vector3_1.y / 3f) * 360f) * Vector3.right, UnityEngine.Random.insideUnitSphere, UnityEngine.Random.Range(this.DispersalMinCoherency, this.DispersalMaxCoherency));
          this.m_dispersalParticles.Emit(new ParticleSystem.EmitParams()
          {
            position = vector3_1,
            velocity = vector3_2 * this.m_dispersalParticles.startSpeed,
            startSize = this.m_dispersalParticles.startSize,
            startLifetime = this.m_dispersalParticles.startLifetime,
            startColor = (Color32) this.m_dispersalParticles.startColor
          }, 1);
        }
      }

      private void HandleIgnitionAndFreezing()
      {
        if (!(bool) (UnityEngine.Object) this.projectile)
          return;
        if ((this.projectile.damageTypes | CoreDamageTypes.Ice) == this.projectile.damageTypes)
        {
          if (this.m_bones.Count > 2)
          {
            Vector3 vector = (Vector3) this.GetBonePosition(this.m_bones.First.Value);
            LinkedListNode<BasicBeamController.BeamBone> next = this.m_bones.First.Next;
            while (next != null)
            {
              Vector3 bonePosition = (Vector3) this.GetBonePosition(next.Value);
              DeadlyDeadlyGoopManager.FreezeGoopsLine(vector.XY(), bonePosition.XY(), 1f);
              next = next.Next;
              vector = bonePosition;
            }
          }
          else if (this.boneType == BasicBeamController.BeamBoneType.Straight)
          {
            Vector2 bonePosition = this.GetBonePosition(this.m_bones.First.Value);
            Vector2 p2 = bonePosition + BraveMathCollege.DegreesToVector(this.transform.eulerAngles.z).normalized * this.m_currentBeamDistance;
            DeadlyDeadlyGoopManager.FreezeGoopsLine(bonePosition, p2, 1f);
          }
        }
        if ((this.projectile.damageTypes | CoreDamageTypes.Fire) == this.projectile.damageTypes)
        {
          if (this.m_bones.Count > 2)
          {
            Vector3 vector = (Vector3) this.GetBonePosition(this.m_bones.First.Value);
            LinkedListNode<BasicBeamController.BeamBone> next = this.m_bones.First.Next;
            while (next != null)
            {
              Vector3 bonePosition = (Vector3) this.GetBonePosition(next.Value);
              DeadlyDeadlyGoopManager.IgniteGoopsLine(vector.XY(), bonePosition.XY(), 1f);
              next = next.Next;
              vector = bonePosition;
            }
          }
          else if (this.m_bones.Count != 2)
            ;
        }
        if ((this.projectile.damageTypes | CoreDamageTypes.Electric) != this.projectile.damageTypes)
          return;
        if (this.m_bones.Count > 2)
        {
          Vector3 vector = (Vector3) this.GetBonePosition(this.m_bones.First.Value);
          LinkedListNode<BasicBeamController.BeamBone> next = this.m_bones.First.Next;
          while (next != null)
          {
            Vector3 bonePosition = (Vector3) this.GetBonePosition(next.Value);
            DeadlyDeadlyGoopManager.ElectrifyGoopsLine(vector.XY(), bonePosition.XY(), 1f);
            next = next.Next;
            vector = bonePosition;
          }
        }
        else if (this.m_bones.Count != 2)
          ;
      }

      public void HandleGoopFrame(GoopModifier gooper)
      {
        if (gooper.IsSynergyContingent && !gooper.SynergyViable)
          return;
        if (gooper.SpawnGoopInFlight && this.m_bones.Count >= 2)
        {
          BasicBeamController.s_goopPoints.Clear();
          float a = -this.collisionRadius * this.m_projectileScale * PhysicsEngine.Instance.PixelUnitWidth;
          float b = this.collisionRadius * this.m_projectileScale * PhysicsEngine.Instance.PixelUnitWidth;
          int num1 = Mathf.Max(2, Mathf.CeilToInt((float) (((double) b - (double) a) / 0.25)));
          for (LinkedListNode<BasicBeamController.BeamBone> linkedListNode = this.m_bones.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
          {
            Vector2 bonePosition = this.GetBonePosition(linkedListNode.Value);
            Vector2 vector2 = new Vector2(0.0f, 1f).Rotate(linkedListNode.Value.RotationAngle);
            for (int index = 0; index < num1; ++index)
            {
              float num2 = Mathf.Lerp(a, b, (float) index / (float) (num1 - 1));
              BasicBeamController.s_goopPoints.Add(bonePosition + vector2 * num2 + gooper.spawnOffset);
            }
          }
          gooper.Manager.AddGoopPoints(BasicBeamController.s_goopPoints, gooper.InFlightSpawnRadius, this.Owner.specRigidbody.UnitCenter, 1.75f);
        }
        if (!gooper.SpawnAtBeamEnd)
          return;
        Vector2 p2 = this.m_bones.Count < 2 || !this.UsesBones ? this.Origin + this.Direction.normalized * this.m_currentBeamDistance : this.GetBonePosition(this.m_bones.Last.Value);
        if (this.m_lastBeamEnd.HasValue)
          gooper.Manager.AddGoopLine(this.m_lastBeamEnd.Value, p2, gooper.BeamEndRadius);
        this.m_lastBeamEnd = new Vector2?(p2);
      }

      public override void LateUpdatePosition(Vector3 origin)
      {
        if (this.m_previousAngle.HasValue)
        {
          float z = origin.y - BasicBeamController.CurrentBeamHeightOffGround;
          this.transform.position = origin.WithZ(z).Quantize(1f / 16f);
        }
        if (this.State != BasicBeamController.BeamState.Charging && this.State != BasicBeamController.BeamState.Telegraphing && this.State != BasicBeamController.BeamState.Firing && this.State != BasicBeamController.BeamState.Dissipating)
          return;
        this.Origin = (Vector2) origin;
        this.FrameUpdate();
      }

      private void CeaseAdditionalBehavior()
      {
        if (!this.angularKnockback || !this.m_hasToggledGunOutline || !(bool) (UnityEngine.Object) this.Gun || !(bool) (UnityEngine.Object) this.Gun.GetSprite())
          return;
        this.m_hasToggledGunOutline = false;
        SpriteOutlineManager.AddOutlineToSprite(this.Gun.GetSprite(), Color.black);
      }

      public override void CeaseAttack()
      {
        this.CeaseAdditionalBehavior();
        if (this.State == BasicBeamController.BeamState.Charging || this.State == BasicBeamController.BeamState.Telegraphing)
          this.DestroyBeam();
        else if (this.endType == BasicBeamController.BeamEndType.Vanish)
        {
          this.DestroyBeam();
        }
        else
        {
          if (this.endType == BasicBeamController.BeamEndType.Dissipate)
          {
            this.State = BasicBeamController.BeamState.Dissipating;
            this.spriteAnimator.Play(this.CurrentBeamAnimation);
            this.m_dissipateTimer = 0.0f;
            this.SelfUpdate = true;
          }
          else if (this.endType == BasicBeamController.BeamEndType.Persist)
          {
            this.State = BasicBeamController.BeamState.Disconnected;
            if (this.ProjectileAndBeamMotionModule is OrbitProjectileMotionModule)
              (this.ProjectileAndBeamMotionModule as OrbitProjectileMotionModule).BeamDestroyed();
            this.SelfUpdate = true;
          }
          if (!this.UsesChargeSprite && !this.UsesMuzzleSprite)
            return;
          this.m_muzzleTransform.gameObject.SetActive(false);
        }
      }

      public override void DestroyBeam()
      {
        if ((bool) (UnityEngine.Object) this.m_reflectedBeam)
        {
          this.m_reflectedBeam.CeaseAttack();
          this.m_reflectedBeam = (BasicBeamController) null;
        }
        if ((bool) (UnityEngine.Object) this.m_enemyKnockback && this.m_enemyKnockbackId >= 0)
        {
          this.m_enemyKnockback.EndContinuousKnockback(this.m_enemyKnockbackId);
          this.m_enemyKnockback = (KnockbackDoer) null;
          this.m_enemyKnockbackId = -1;
        }
        if (this.doesScreenDistortion && (UnityEngine.Object) this.m_distortionMaterial != (UnityEngine.Object) null)
          Pixelator.Instance.DeregisterAdditionalRenderPass(this.m_distortionMaterial);
        if (GameManager.AUDIO_ENABLED && !string.IsNullOrEmpty(this.endAudioEvent))
        {
          int num = (int) AkSoundEngine.PostEvent(this.endAudioEvent, this.gameObject);
        }
        if (this.ProjectileAndBeamMotionModule is OrbitProjectileMotionModule)
          (this.ProjectileAndBeamMotionModule as OrbitProjectileMotionModule).BeamDestroyed();
        UnityEngine.Object.Destroy((UnityEngine.Object) this.transform.gameObject);
      }

      public override void AdjustPlayerBeamTint(Color targetTintColor, int priority, float lerpTime = 0.0f)
      {
        if (!(this.Owner is PlayerController) || priority <= this.m_currentTintPriority)
          return;
        this.m_currentTintPriority = priority;
        this.ChangeTintColorShader((tk2dBaseSprite) this.m_beamSprite, lerpTime, targetTintColor);
        if ((bool) (UnityEngine.Object) this.m_beamMuzzleSprite)
          this.ChangeTintColorShader((tk2dBaseSprite) this.m_beamMuzzleSprite, lerpTime, targetTintColor);
        if ((bool) (UnityEngine.Object) this.m_impactSprite)
          this.ChangeTintColorShader((tk2dBaseSprite) this.m_impactSprite, lerpTime, targetTintColor);
        if (!(bool) (UnityEngine.Object) this.m_impact2Sprite)
          return;
        this.ChangeTintColorShader((tk2dBaseSprite) this.m_impact2Sprite, lerpTime, targetTintColor);
      }

      private void ChangeTintColorShader(tk2dBaseSprite baseSprite, float time, Color color)
      {
        baseSprite.OverrideMaterialMode = tk2dBaseSprite.SpriteMaterialOverrideMode.OVERRIDE_MATERIAL_SIMPLE;
        Material material = baseSprite.renderer.material;
        bool flag = material.HasProperty("_EmissivePower");
        float num1 = 0.0f;
        float num2 = 0.0f;
        if (flag)
        {
          num1 = material.GetFloat("_EmissivePower");
          num2 = material.GetFloat("_EmissiveColorPower");
        }
        Shader shader = flag ? ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive") : ShaderCache.Acquire("tk2d/CutoutVertexColorTintableTilted");
        if ((UnityEngine.Object) baseSprite.renderer.material.shader != (UnityEngine.Object) shader)
        {
          baseSprite.renderer.material.shader = shader;
          baseSprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
          if (flag)
          {
            baseSprite.renderer.material.SetFloat("_EmissivePower", num1);
            baseSprite.renderer.material.SetFloat("_EmissiveColorPower", num2);
          }
        }
        if ((double) time == 0.0)
          baseSprite.renderer.sharedMaterial.SetColor("_OverrideColor", color);
        else
          this.StartCoroutine(this.ChangeTintColorCR(baseSprite, time, color));
      }

      [DebuggerHidden]
      private IEnumerator ChangeTintColorCR(tk2dBaseSprite baseSprite, float time, Color color)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BasicBeamController.<ChangeTintColorCR>c__Iterator0()
        {
          baseSprite = baseSprite,
          time = time,
          color = color
        };
      }

      public void BecomeBlackBullet()
      {
        if (this.IsBlackBullet || !(bool) (UnityEngine.Object) this.sprite)
          return;
        this.IsBlackBullet = true;
        this.sprite.usesOverrideMaterial = true;
        this.sprite.renderer.material.SetFloat("_BlackBullet", 1f);
        this.sprite.renderer.material.SetFloat("_EmissivePower", -40f);
      }

      public void ReturnFromBlackBullet()
      {
        if (!this.IsBlackBullet)
          return;
        this.IsBlackBullet = false;
        this.sprite.renderer.material.SetFloat("_BlackBullet", 0.0f);
        this.sprite.usesOverrideMaterial = false;
        this.sprite.ForceUpdateMaterial();
      }

      public void GetTiledSpriteGeomDesc(
        out int numVertices,
        out int numIndices,
        tk2dSpriteDefinition spriteDef,
        Vector2 dimensions)
      {
        int num = Mathf.CeilToInt(dimensions.x / (float) this.m_beamQuadPixelWidth);
        if (this.TileType == BasicBeamController.BeamTileType.Flowing)
          num = this.m_bones.Count - 1;
        numVertices = num * 4;
        numIndices = num * 6;
      }

      public void SetTiledSpriteGeom(
        Vector3[] pos,
        Vector2[] uv,
        int offset,
        out Vector3 boundsCenter,
        out Vector3 boundsExtents,
        tk2dSpriteDefinition spriteDef,
        Vector3 scale,
        Vector2 dimensions,
        tk2dBaseSprite.Anchor anchor,
        float colliderOffsetZ,
        float colliderExtentZ)
      {
        boundsCenter = Vector3.zero;
        boundsExtents = Vector3.zero;
        int num1 = Mathf.RoundToInt(spriteDef.untrimmedBoundsDataExtents.x / spriteDef.texelSize.x) / this.m_beamQuadPixelWidth;
        int num2 = Mathf.CeilToInt(dimensions.x / (float) this.m_beamQuadPixelWidth);
        int num3 = Mathf.CeilToInt((float) num2 / (float) num1);
        if (this.TileType == BasicBeamController.BeamTileType.Flowing)
        {
          num2 = this.m_bones.Count - 1;
          num3 = this.m_bones.Count<BasicBeamController.BeamBone>((Func<BasicBeamController.BeamBone, bool>) (b => b.SubtileNum == 0));
          if (this.m_bones.First.Value.SubtileNum != 0)
            ++num3;
          if (this.m_bones.Last.Value.SubtileNum == 0)
            --num3;
        }
        Vector2 vector2_1 = new Vector2(dimensions.x * spriteDef.texelSize.x * scale.x, dimensions.y * spriteDef.texelSize.y * scale.y);
        Vector2 vector2_2 = Vector2.Scale(spriteDef.texelSize, (Vector2) scale) * 0.1f;
        int num4 = 0;
        Vector3 vector3_1 = Vector3.Scale(new Vector3((float) this.m_beamQuadPixelWidth * spriteDef.texelSize.x, spriteDef.untrimmedBoundsDataExtents.y, spriteDef.untrimmedBoundsDataExtents.z), scale);
        Vector3 zero = Vector3.zero;
        Quaternion quaternion = Quaternion.Euler(0.0f, 0.0f, this.Direction.ToAngle());
        LinkedListNode<BasicBeamController.BeamBone> linkedListNode = this.m_bones.First;
        for (int index1 = 0; index1 < num3; ++index1)
        {
          int num5 = 0;
          int num6 = num1 - 1;
          if (this.TileType == BasicBeamController.BeamTileType.GrowAtBeginning)
          {
            if (index1 == 0 && num2 % num1 != 0)
              num5 = num1 - num2 % num1;
          }
          else if (this.TileType == BasicBeamController.BeamTileType.GrowAtEnd)
          {
            if (index1 == num3 - 1 && num2 % num1 != 0)
              num6 = num2 % num1 - 1;
          }
          else if (this.TileType == BasicBeamController.BeamTileType.Flowing)
          {
            if (index1 == 0)
              num5 = linkedListNode.Value.SubtileNum;
            if (index1 == num3 - 1)
              num6 = this.m_bones.Last.Previous.Value.SubtileNum;
          }
          tk2dSpriteDefinition spriteDefinition = spriteDef;
          if (this.UsesBeamStartAnimation && index1 == 0)
          {
            tk2dSpriteAnimationClip clipByName = this.spriteAnimator.GetClipByName(this.CurrentBeamStartAnimation);
            spriteDefinition = this.m_beamSprite.Collection.spriteDefinitions[clipByName.frames[Mathf.Min(clipByName.frames.Length - 1, this.spriteAnimator.CurrentFrame)].spriteId];
          }
          if (this.UsesBeamEndAnimation && index1 == num3 - 1)
          {
            tk2dSpriteAnimationClip clipByName = this.spriteAnimator.GetClipByName(this.CurrentBeamEndAnimation);
            spriteDefinition = this.m_beamSprite.Collection.spriteDefinitions[clipByName.frames[Mathf.Min(clipByName.frames.Length - 1, this.spriteAnimator.CurrentFrame)].spriteId];
          }
          float t = 0.0f;
          if (index1 == 0)
          {
            if (this.TileType == BasicBeamController.BeamTileType.GrowAtBeginning)
              t = (float) (1.0 - (double) Mathf.Abs(vector2_1.x % (vector3_1.x * (float) num1)) / ((double) vector3_1.x * (double) num1));
            else if (this.TileType == BasicBeamController.BeamTileType.Flowing)
              t = this.m_uvOffset;
          }
          for (int index2 = num5; index2 <= num6; ++index2)
          {
            BasicBeamController.BeamBone bone1 = (BasicBeamController.BeamBone) null;
            BasicBeamController.BeamBone bone2 = (BasicBeamController.BeamBone) null;
            if (linkedListNode != null)
            {
              bone1 = linkedListNode.Value;
              if (linkedListNode.Next != null)
                bone2 = linkedListNode.Next.Value;
            }
            float num7 = 1f;
            if (this.TileType == BasicBeamController.BeamTileType.GrowAtBeginning)
            {
              if (index1 == 0 && index2 == 0 && (double) num2 * (double) vector3_1.x >= (double) Mathf.Abs(vector2_1.x) + (double) vector2_2.x)
                num7 = Mathf.Abs(vector2_1.x / vector3_1.x) - (float) (num2 - 1);
            }
            else if (this.TileType == BasicBeamController.BeamTileType.GrowAtEnd)
            {
              if ((double) Mathf.Abs(zero.x + vector3_1.x) > (double) Mathf.Abs(vector2_1.x) + (double) vector2_2.x)
                num7 = vector2_1.x % vector3_1.x / vector3_1.x;
            }
            else if (this.TileType == BasicBeamController.BeamTileType.Flowing)
            {
              if (index1 == 0 && linkedListNode == this.m_bones.First)
                num7 = (bone2.PosX - bone1.PosX) / this.m_beamQuadUnitWidth;
              else if (index1 == num3 - 1 && linkedListNode.Next.Next == null)
                num7 = (bone2.PosX - bone1.PosX) / this.m_beamQuadUnitWidth;
            }
            float z1 = 0.0f;
            if ((double) this.RampHeightOffset != 0.0 && (double) zero.x < 5.0)
              z1 = (float) ((1.0 - (double) zero.x / 5.0) * -(double) this.RampHeightOffset);
            if (this.UsesBones && bone2 != null)
            {
              float rotationAngle = bone1.RotationAngle;
              float z2 = bone2.RotationAngle;
              if ((double) Mathf.Abs(BraveMathCollege.ClampAngle180(z2 - rotationAngle)) > 90.0)
                z2 = BraveMathCollege.ClampAngle360(z2 + 180f);
              Vector2 bonePosition1 = this.GetBonePosition(bone1);
              Vector2 bonePosition2 = this.GetBonePosition(bone2);
              int num8 = offset + num4;
              Vector3[] vector3Array1 = pos;
              int index3 = num8;
              int num9 = index3 + 1;
              vector3Array1[index3] = Quaternion.Euler(0.0f, 0.0f, rotationAngle) * Vector3.Scale(new Vector3(0.0f, spriteDefinition.position0.y * this.m_projectileScale, z1), scale) + (Vector3) (bonePosition1 - this.transform.position.XY());
              Vector3[] vector3Array2 = pos;
              int index4 = num9;
              int num10 = index4 + 1;
              vector3Array2[index4] = Quaternion.Euler(0.0f, 0.0f, z2) * Vector3.Scale(new Vector3(0.0f, spriteDefinition.position1.y * this.m_projectileScale, z1), scale) + (Vector3) (bonePosition2 - this.transform.position.XY());
              Vector3[] vector3Array3 = pos;
              int index5 = num10;
              int num11 = index5 + 1;
              vector3Array3[index5] = Quaternion.Euler(0.0f, 0.0f, rotationAngle) * Vector3.Scale(new Vector3(0.0f, spriteDefinition.position2.y * this.m_projectileScale, z1), scale) + (Vector3) (bonePosition1 - this.transform.position.XY());
              Vector3[] vector3Array4 = pos;
              int index6 = num11;
              int num12 = index6 + 1;
              vector3Array4[index6] = Quaternion.Euler(0.0f, 0.0f, z2) * Vector3.Scale(new Vector3(0.0f, spriteDefinition.position3.y * this.m_projectileScale, z1), scale) + (Vector3) (bonePosition2 - this.transform.position.XY());
            }
            else if (this.boneType == BasicBeamController.BeamBoneType.Straight)
            {
              int num13 = offset + num4;
              Vector3[] vector3Array5 = pos;
              int index7 = num13;
              int num14 = index7 + 1;
              vector3Array5[index7] = quaternion * (zero + Vector3.Scale(new Vector3(0.0f, spriteDefinition.position0.y * this.m_projectileScale, z1), scale));
              Vector3[] vector3Array6 = pos;
              int index8 = num14;
              int num15 = index8 + 1;
              vector3Array6[index8] = quaternion * (zero + Vector3.Scale(new Vector3(num7 * vector3_1.x, spriteDefinition.position1.y * this.m_projectileScale, z1), scale));
              Vector3[] vector3Array7 = pos;
              int index9 = num15;
              int num16 = index9 + 1;
              vector3Array7[index9] = quaternion * (zero + Vector3.Scale(new Vector3(0.0f, spriteDefinition.position2.y * this.m_projectileScale, z1), scale));
              Vector3[] vector3Array8 = pos;
              int index10 = num16;
              int num17 = index10 + 1;
              vector3Array8[index10] = quaternion * (zero + Vector3.Scale(new Vector3(num7 * vector3_1.x, spriteDefinition.position3.y * this.m_projectileScale, z1), scale));
            }
            Vector2 vector2_3 = Vector2.Lerp(spriteDefinition.uvs[0], spriteDefinition.uvs[1], t);
            Vector2 vector2_4 = Vector2.Lerp(spriteDefinition.uvs[2], spriteDefinition.uvs[3], t + num7 / (float) num1);
            if (this.FlipBeamSpriteLocal && (double) this.Direction.x < 0.0)
            {
              float y = vector2_3.y;
              vector2_3.y = vector2_4.y;
              vector2_4.y = y;
            }
            int num18 = offset + num4;
            int num19;
            if (spriteDefinition.flipped == tk2dSpriteDefinition.FlipMode.Tk2d)
            {
              Vector2[] vector2Array1 = uv;
              int index11 = num18;
              int num20 = index11 + 1;
              vector2Array1[index11] = new Vector2(vector2_3.x, vector2_3.y);
              Vector2[] vector2Array2 = uv;
              int index12 = num20;
              int num21 = index12 + 1;
              vector2Array2[index12] = new Vector2(vector2_3.x, vector2_4.y);
              Vector2[] vector2Array3 = uv;
              int index13 = num21;
              int num22 = index13 + 1;
              vector2Array3[index13] = new Vector2(vector2_4.x, vector2_3.y);
              Vector2[] vector2Array4 = uv;
              int index14 = num22;
              num19 = index14 + 1;
              vector2Array4[index14] = new Vector2(vector2_4.x, vector2_4.y);
            }
            else if (spriteDefinition.flipped == tk2dSpriteDefinition.FlipMode.TPackerCW)
            {
              Vector2[] vector2Array5 = uv;
              int index15 = num18;
              int num23 = index15 + 1;
              vector2Array5[index15] = new Vector2(vector2_3.x, vector2_3.y);
              Vector2[] vector2Array6 = uv;
              int index16 = num23;
              int num24 = index16 + 1;
              vector2Array6[index16] = new Vector2(vector2_4.x, vector2_3.y);
              Vector2[] vector2Array7 = uv;
              int index17 = num24;
              int num25 = index17 + 1;
              vector2Array7[index17] = new Vector2(vector2_3.x, vector2_4.y);
              Vector2[] vector2Array8 = uv;
              int index18 = num25;
              num19 = index18 + 1;
              vector2Array8[index18] = new Vector2(vector2_4.x, vector2_4.y);
            }
            else
            {
              Vector2[] vector2Array9 = uv;
              int index19 = num18;
              int num26 = index19 + 1;
              vector2Array9[index19] = new Vector2(vector2_3.x, vector2_3.y);
              Vector2[] vector2Array10 = uv;
              int index20 = num26;
              int num27 = index20 + 1;
              vector2Array10[index20] = new Vector2(vector2_4.x, vector2_3.y);
              Vector2[] vector2Array11 = uv;
              int index21 = num27;
              int num28 = index21 + 1;
              vector2Array11[index21] = new Vector2(vector2_3.x, vector2_4.y);
              Vector2[] vector2Array12 = uv;
              int index22 = num28;
              num19 = index22 + 1;
              vector2Array12[index22] = new Vector2(vector2_4.x, vector2_4.y);
            }
            num4 += 4;
            zero.x += vector3_1.x * num7;
            t += num7 / (float) this.m_beamSpriteSubtileWidth;
            if (linkedListNode != null)
              linkedListNode = linkedListNode.Next;
          }
        }
        Vector3 lhs1 = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        Vector3 lhs2 = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        for (int index = 0; index < pos.Length; ++index)
        {
          lhs1 = Vector3.Min(lhs1, pos[index]);
          lhs2 = Vector3.Max(lhs2, pos[index]);
        }
        Vector3 vector3_2 = (lhs2 - lhs1) / 2f;
        boundsCenter = lhs1 + vector3_2;
        boundsExtents = vector3_2;
      }

      private bool FindBeamTarget(
        Vector2 origin,
        Vector2 direction,
        float distance,
        int collisionMask,
        out Vector2 targetPoint,
        out Vector2 targetNormal,
        out SpeculativeRigidbody hitRigidbody,
        out PixelCollider hitPixelCollider,
        out List<PointcastResult> boneCollisions,
        Func<SpeculativeRigidbody, bool> rigidbodyExcluder = null,
        params SpeculativeRigidbody[] ignoreRigidbodies)
      {
        bool flag1 = false;
        targetPoint = new Vector2(-1f, -1f);
        targetNormal = new Vector2(0.0f, 0.0f);
        hitRigidbody = (SpeculativeRigidbody) null;
        hitPixelCollider = (PixelCollider) null;
        if (this.collisionType == BasicBeamController.BeamCollisionType.Rectangle)
        {
          if (!(bool) (UnityEngine.Object) this.specRigidbody)
          {
            this.specRigidbody = this.gameObject.AddComponent<SpeculativeRigidbody>();
            this.specRigidbody.CollideWithTileMap = false;
            this.specRigidbody.CollideWithOthers = true;
            PixelCollider pixelCollider = new PixelCollider()
            {
              Enabled = false,
              CollisionLayer = CollisionLayer.PlayerBlocker
            };
            pixelCollider.Enabled = true;
            pixelCollider.IsTrigger = true;
            pixelCollider.ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual;
            pixelCollider.ManualOffsetX = 0;
            pixelCollider.ManualOffsetY = this.collisionWidth / -2;
            pixelCollider.ManualWidth = this.collisionLength;
            pixelCollider.ManualHeight = this.collisionWidth;
            this.specRigidbody.PixelColliders = new List<PixelCollider>(1);
            this.specRigidbody.PixelColliders.Add(pixelCollider);
            this.specRigidbody.Initialize();
          }
          if (this.m_cachedRectangleOrigin != origin || this.m_cachedRectangleDirection != direction)
          {
            this.specRigidbody.Position = new Position(origin);
            this.specRigidbody.PrimaryPixelCollider.SetRotationAndScale(direction.ToAngle(), Vector2.one);
            this.specRigidbody.UpdateColliderPositions();
            this.m_cachedRectangleOrigin = origin;
            this.m_cachedRectangleDirection = direction;
          }
          int mask = CollisionMask.LayerToMask(CollisionLayer.PlayerHitBox);
          if ((collisionMask & mask) == mask)
            this.specRigidbody.PrimaryPixelCollider.CollisionLayerIgnoreOverride &= ~CollisionMask.LayerToMask(CollisionLayer.PlayerHitBox, CollisionLayer.PlayerCollider);
          else
            this.specRigidbody.PrimaryPixelCollider.CollisionLayerIgnoreOverride |= CollisionMask.LayerToMask(CollisionLayer.PlayerHitBox, CollisionLayer.PlayerCollider);
          List<CollisionData> overlappingCollisions = new List<CollisionData>();
          this.specRigidbody.PrimaryPixelCollider.Enabled = true;
          bool flag2 = PhysicsEngine.Instance.OverlapCast(this.specRigidbody, overlappingCollisions, false, true, new int?(), new int?(), false, new Vector2?(), (Func<SpeculativeRigidbody, bool>) null, ignoreRigidbodies);
          this.specRigidbody.PrimaryPixelCollider.Enabled = false;
          boneCollisions = new List<PointcastResult>();
          if (!flag2)
            return false;
          targetNormal = overlappingCollisions[0].Normal;
          targetPoint = overlappingCollisions[0].Contact;
          hitRigidbody = overlappingCollisions[0].OtherRigidbody;
          hitPixelCollider = overlappingCollisions[0].OtherPixelCollider;
        }
        else if (this.UsesBones)
        {
          float minOffset = -this.collisionRadius * this.m_projectileScale * PhysicsEngine.Instance.PixelUnitWidth;
          float maxOffset = this.collisionRadius * this.m_projectileScale * PhysicsEngine.Instance.PixelUnitWidth;
          int num = Mathf.Max(2, Mathf.CeilToInt((float) (((double) maxOffset - (double) minOffset) / 0.25)));
          int ignoreTileBoneCount;
          if (!PhysicsEngine.Instance.Pointcast(this.GeneratePixelCloud(minOffset, maxOffset, (float) num, out ignoreTileBoneCount), this.GenerateLastPixelCloud(minOffset, maxOffset, (float) num), num, out boneCollisions, true, true, collisionMask, new CollisionLayer?(CollisionLayer.Projectile), false, rigidbodyExcluder, ignoreTileBoneCount, ignoreRigidbodies))
            return false;
          PointcastResult pointcastResult = boneCollisions[0];
          for (int index = 0; index < boneCollisions.Count; ++index)
          {
            if (boneCollisions[index].hitDirection == HitDirection.Forward && boneCollisions[index].boneIndex > 0)
            {
              pointcastResult = boneCollisions[index];
              break;
            }
          }
          targetPoint = pointcastResult.hitResult.Contact;
          targetNormal = pointcastResult.hitResult.Normal;
          hitRigidbody = pointcastResult.hitResult.SpeculativeRigidbody;
          hitPixelCollider = pointcastResult.hitResult.OtherPixelCollider;
        }
        else
        {
          float a = -this.collisionRadius * this.m_projectileScale * PhysicsEngine.Instance.PixelUnitWidth;
          float b = this.collisionRadius * this.m_projectileScale * PhysicsEngine.Instance.PixelUnitWidth;
          int num = Mathf.Max(2, Mathf.CeilToInt((float) (((double) b - (double) a) / 0.25)));
          RaycastResult raycastResult = (RaycastResult) null;
          for (int index = 0; index < num; ++index)
          {
            float y = Mathf.Lerp(a, b, (float) index / (float) (num - 1));
            RaycastResult result;
            if (PhysicsEngine.Instance.RaycastWithIgnores(origin + new Vector2(0.0f, y).Rotate(direction.ToAngle()), direction.normalized, distance, out result, rayMask: collisionMask, sourceLayer: new CollisionLayer?(CollisionLayer.Projectile), rigidbodyExcluder: rigidbodyExcluder, ignoreList: (ICollection<SpeculativeRigidbody>) ignoreRigidbodies))
            {
              flag1 = true;
              if (raycastResult == null || (double) result.Distance < (double) raycastResult.Distance)
              {
                RaycastResult.Pool.Free(ref raycastResult);
                raycastResult = result;
              }
              else
                RaycastResult.Pool.Free(ref result);
            }
          }
          boneCollisions = new List<PointcastResult>();
          if (!flag1)
            return false;
          targetNormal = raycastResult.Normal;
          targetPoint = origin + BraveMathCollege.DegreesToVector(direction.ToAngle(), raycastResult.Distance);
          hitRigidbody = raycastResult.SpeculativeRigidbody;
          hitPixelCollider = raycastResult.OtherPixelCollider;
          RaycastResult.Pool.Free(ref raycastResult);
        }
        if ((UnityEngine.Object) hitRigidbody == (UnityEngine.Object) null)
          return true;
        if ((bool) (UnityEngine.Object) hitRigidbody.minorBreakable && !hitRigidbody.minorBreakable.OnlyBrokenByCode)
          hitRigidbody.minorBreakable.Break(direction);
        DebrisObject component1 = hitRigidbody.GetComponent<DebrisObject>();
        if ((bool) (UnityEngine.Object) component1)
          component1.Trigger((Vector3) direction, 0.5f);
        TorchController component2 = hitRigidbody.GetComponent<TorchController>();
        if ((bool) (UnityEngine.Object) component2)
          component2.BeamCollision(this.projectile);
        if ((bool) (UnityEngine.Object) hitRigidbody.projectile && hitRigidbody.projectile.collidesWithProjectiles)
          hitRigidbody.projectile.BeamCollision(this.projectile);
        return true;
      }

      private List<IntVector2> GeneratePixelCloud(
        float minOffset,
        float maxOffset,
        float numOffsets,
        out int ignoreTileBoneCount)
      {
        ignoreTileBoneCount = -1;
        bool flag = false;
        BasicBeamController.s_pixelCloud.Clear();
        for (LinkedListNode<BasicBeamController.BeamBone> linkedListNode = this.m_bones.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
        {
          Vector2 bonePosition = this.GetBonePosition(linkedListNode.Value);
          Vector2 vector2 = new Vector2(0.0f, 1f).Rotate(linkedListNode.Value.RotationAngle);
          if ((double) this.IgnoreTilesDistance > 0.0 && !flag && (double) linkedListNode.Value.PosX > (double) this.IgnoreTilesDistance)
          {
            ignoreTileBoneCount = BasicBeamController.s_pixelCloud.Count;
            flag = true;
          }
          for (int index = 0; (double) index < (double) numOffsets; ++index)
          {
            float num = Mathf.Lerp(minOffset, maxOffset, (float) index / (numOffsets - 1f));
            BasicBeamController.s_pixelCloud.Add(PhysicsEngine.UnitToPixel(bonePosition + vector2 * num));
          }
        }
        if ((double) this.IgnoreTilesDistance > 0.0 && !flag)
          ignoreTileBoneCount = BasicBeamController.s_pixelCloud.Count;
        return BasicBeamController.s_pixelCloud;
      }

      private List<IntVector2> GenerateLastPixelCloud(
        float minOffset,
        float maxOffset,
        float numOffsets)
      {
        BasicBeamController.s_lastPixelCloud.Clear();
        for (LinkedListNode<BasicBeamController.BeamBone> linkedListNode = this.m_bones.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
        {
          Vector2 vector2_1 = this.GetBonePosition(linkedListNode.Value) - linkedListNode.Value.Velocity * BraveTime.DeltaTime;
          Vector2 vector2_2 = new Vector2(0.0f, 1f).Rotate(linkedListNode.Value.RotationAngle);
          for (int index = 0; (double) index < (double) numOffsets; ++index)
          {
            float num = Mathf.Lerp(minOffset, maxOffset, (float) index / (numOffsets - 1f));
            BasicBeamController.s_lastPixelCloud.Add(PhysicsEngine.UnitToPixel(vector2_1 + vector2_2 * num));
          }
        }
        return BasicBeamController.s_lastPixelCloud;
      }

      private Vector2 GetBonePosition(BasicBeamController.BeamBone bone)
      {
        if (!this.UsesBones)
          return this.Origin + BraveMathCollege.DegreesToVector(this.Direction.ToAngle(), bone.PosX);
        return this.ProjectileAndBeamMotionModule != null ? bone.Position + this.ProjectileAndBeamMotionModule.GetBoneOffset(bone, (BeamController) this, this.projectile.Inverted) : bone.Position;
      }

      public Vector2 GetPointOnBeam(float t)
      {
        if (this.m_bones.Count < 2)
          return this.Origin;
        return this.UsesBones ? this.Origin + this.Direction.normalized * (this.m_bones.Last.Value.Position - this.m_bones.First.Value.Position).magnitude * t : this.Origin + this.Direction.normalized * this.m_currentBeamDistance * t;
      }

      private void SeparateBeam(
        LinkedListNode<BasicBeamController.BeamBone> startNode,
        Vector2 newOrigin,
        float newPosX)
      {
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.gameObject);
        gameObject.name = this.gameObject.name + " (Split)";
        BasicBeamController component = gameObject.GetComponent<BasicBeamController>();
        component.State = BasicBeamController.BeamState.Disconnected;
        component.m_bones = new LinkedList<BasicBeamController.BeamBone>();
        component.SelfUpdate = true;
        component.projectile.Owner = this.projectile.Owner;
        component.Owner = this.Owner;
        component.Gun = this.Gun;
        component.HitsPlayers = this.HitsPlayers;
        component.HitsEnemies = this.HitsEnemies;
        component.Origin = this.Origin;
        component.Direction = this.Direction;
        component.DamageModifier = this.DamageModifier;
        component.GetComponent<tk2dTiledSprite>().dimensions = this.m_beamSprite.dimensions;
        component.m_previousAngle = this.m_previousAngle;
        component.m_currentBeamDistance = this.m_currentBeamDistance;
        component.reflections = this.reflections;
        component.Origin = newOrigin;
        component.m_bones.AddFirst(new BasicBeamController.BeamBone(startNode.Previous.Value)
        {
          Position = newOrigin,
          PosX = newPosX
        });
        LinkedListNode<BasicBeamController.BeamBone> previous = startNode.Previous;
        while (previous.Next != null)
        {
          LinkedListNode<BasicBeamController.BeamBone> next = previous.Next;
          this.m_bones.Remove(next);
          component.m_bones.AddLast(next);
        }
      }

      private BasicBeamController CreateReflectedBeam(
        Vector2 pos,
        Vector2 dir,
        bool decrementReflections = true)
      {
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.gameObject);
        gameObject.name = this.gameObject.name + " (Reflect)";
        BasicBeamController component = gameObject.GetComponent<BasicBeamController>();
        component.State = BasicBeamController.BeamState.Firing;
        component.IsReflectedBeam = true;
        component.Owner = this.Owner;
        component.Gun = this.Gun;
        component.HitsPlayers = this.HitsPlayers;
        component.HitsEnemies = this.HitsEnemies;
        component.Origin = pos;
        component.Direction = dir;
        component.DamageModifier = this.DamageModifier;
        component.usesChargeDelay = false;
        component.muzzleAnimation = string.Empty;
        component.chargeAnimation = string.Empty;
        component.beamStartAnimation = string.Empty;
        component.IgnoreTilesDistance = -1f;
        component.reflections = this.reflections;
        if (decrementReflections)
          --component.reflections;
        component.projectile.Owner = this.projectile.Owner;
        component.playerStatsModified = this.playerStatsModified;
        return component;
      }

      private tk2dBaseSprite CreatePierceImpactEffect()
      {
        GameObject gameObject = new GameObject("beam pierce impact vfx");
        Transform transform = gameObject.transform;
        transform.parent = this.transform;
        transform.localPosition = new Vector3(0.0f, 0.0f, 0.05f);
        transform.localScale = new Vector3(this.m_projectileScale, this.m_projectileScale, 1f);
        tk2dSprite attachment = gameObject.AddComponent<tk2dSprite>();
        attachment.SetSprite(this.m_beamSprite.Collection, this.m_beamSprite.spriteId);
        tk2dSpriteAnimator tk2dSpriteAnimator = gameObject.AddComponent<tk2dSpriteAnimator>();
        tk2dSpriteAnimator.SetSprite(this.m_beamSprite.Collection, this.m_beamSprite.spriteId);
        tk2dSpriteAnimator.Library = this.spriteAnimator.Library;
        tk2dSpriteAnimator.Play(this.impactAnimation);
        this.m_beamSprite.AttachRenderer((tk2dBaseSprite) attachment);
        attachment.HeightOffGround = 0.05f;
        attachment.IsPerpendicular = true;
        attachment.usesOverrideMaterial = true;
        return (tk2dBaseSprite) attachment;
      }

      public static void SetGlobalBeamHeight(float newDepth)
      {
        BasicBeamController.CurrentBeamHeightOffGround = newDepth;
      }

      public static void ResetGlobalBeamHeight()
      {
        BasicBeamController.CurrentBeamHeightOffGround = 0.75f;
      }

      [Serializable]
      public class AngularKnockbackTier
      {
        public float minAngularSpeed;
        public float damageMultiplier;
        public float knockbackMultiplier;
        public float ignoreHitRigidbodyTime;
        public VFXPool hitRigidbodyVFX;
        public int additionalAmmoCost;
      }

      public enum BeamState
      {
        Charging,
        Telegraphing,
        Firing,
        Dissipating,
        Disconnected,
      }

      public enum BeamBoneType
      {
        Straight = 0,
        Projectile = 2,
      }

      public enum BeamTileType
      {
        GrowAtEnd,
        GrowAtBeginning,
        Flowing,
      }

      public enum BeamEndType
      {
        Vanish,
        Persist,
        Dissipate,
      }

      public enum BeamCollisionType
      {
        Default,
        Rectangle,
      }

      public class BeamBone
      {
        public float PosX;
        public float RotationAngle;
        public Vector2 Position;
        public Vector2 Velocity;
        public int SubtileNum;
        public float HomingRadius;
        public float HomingAngularVelocity;
        public AIActor HomingTarget;
        public bool HomingDampenMotion;

        public BeamBone(float posX, float rotationAngle, int subtileNum)
        {
          this.PosX = posX;
          this.RotationAngle = rotationAngle;
          this.SubtileNum = subtileNum;
        }

        public BeamBone(float posX, Vector2 position, Vector2 velocity)
        {
          this.PosX = posX;
          this.Position = position;
          this.Velocity = velocity;
        }

        public BeamBone(BasicBeamController.BeamBone other)
        {
          this.PosX = other.PosX;
          this.RotationAngle = other.RotationAngle;
          this.Position = other.Position;
          this.Velocity = other.Velocity;
          this.SubtileNum = other.SubtileNum;
          this.HomingRadius = other.HomingRadius;
          this.HomingAngularVelocity = other.HomingAngularVelocity;
          this.HomingDampenMotion = other.HomingDampenMotion;
        }

        public void ApplyHoming(SpeculativeRigidbody ignoreRigidbody = null, float overrideDeltaTime = -1f)
        {
          if ((double) this.HomingRadius == 0.0 || (double) this.HomingAngularVelocity == 0.0)
            return;
          IntVector2 intVector2 = this.Position.ToIntVector2(VectorConversions.Floor);
          if (!GameManager.Instance.Dungeon.CellExists(intVector2))
            return;
          List<AIActor> activeEnemies = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(intVector2).GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
          if (activeEnemies == null || activeEnemies.Count == 0)
            return;
          float num1 = float.MaxValue;
          Vector2 vector = Vector2.zero;
          AIActor aiActor = (AIActor) null;
          for (int index = 0; index < activeEnemies.Count; ++index)
          {
            if ((bool) (UnityEngine.Object) activeEnemies[index] && !activeEnemies[index].healthHaver.IsDead && (!(bool) (UnityEngine.Object) ignoreRigidbody || !((UnityEngine.Object) activeEnemies[index].specRigidbody == (UnityEngine.Object) ignoreRigidbody)))
            {
              Vector2 vector2 = activeEnemies[index].CenterPosition - this.Position;
              float magnitude = vector2.magnitude;
              if ((double) magnitude < (double) num1 - 0.5)
              {
                vector = vector2;
                num1 = magnitude;
                aiActor = activeEnemies[index];
              }
            }
          }
          if ((double) num1 >= (double) this.HomingRadius || !((UnityEngine.Object) aiActor != (UnityEngine.Object) null))
            return;
          float num2 = (float) (1.0 - (double) num1 / (double) this.HomingRadius);
          this.Velocity = BraveMathCollege.DegreesToVector(Mathf.MoveTowardsAngle(this.Velocity.ToAngle(), vector.ToAngle(), (float) ((double) this.HomingAngularVelocity * (double) num2 * ((double) overrideDeltaTime < 0.0 ? (double) BraveTime.DeltaTime : (double) overrideDeltaTime))), this.Velocity.magnitude);
          if ((UnityEngine.Object) aiActor != (UnityEngine.Object) this.HomingTarget)
            this.HomingDampenMotion = true;
          this.HomingTarget = aiActor;
        }
      }

      [Serializable]
      public class TelegraphAnims
      {
        [CheckAnimation(null)]
        public string beamAnimation;
        [CheckAnimation(null)]
        public string beamStartAnimation;
        [CheckAnimation(null)]
        public string beamEndAnimation;
      }
    }

}
