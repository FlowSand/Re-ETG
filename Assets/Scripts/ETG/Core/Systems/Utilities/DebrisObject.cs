// Decompiled with JetBrains decompiler
// Type: DebrisObject
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

namespace ETG.Core.Systems.Utilities
{
    public class DebrisObject : EphemeralObject
    {
      public static List<SpeculativeRigidbody> SRB_Walls = new List<SpeculativeRigidbody>();
      public static List<SpeculativeRigidbody> SRB_Pits = new List<SpeculativeRigidbody>();
      private float ACCURATE_DEBRIS_THRESHOLD = 0.25f;
      public string audioEventName;
      [NonSerialized]
      public bool IsCorpse;
      public bool playAnimationOnTrigger;
      public bool usesDirectionalFallAnimations;
      [ShowInInspectorIf("usesDirectionalFallAnimations", false)]
      public DebrisDirectionalAnimationInfo directionalAnimationData;
      public bool breaksOnFall = true;
      [ShowInInspectorIf("breaksOnFall", false)]
      public float breakOnFallChance = 1f;
      public bool changesCollisionLayer;
      public CollisionLayer groundedCollisionLayer = CollisionLayer.LowObstacle;
      public DebrisObject.DebrisFollowupAction followupBehavior;
      public string followupIdentifier;
      public bool collisionStopsBullets;
      public bool animatePitFall;
      public bool pitFallSplash;
      public float inertialMass = 1f;
      public float motionMultiplier = 1f;
      public bool canRotate = true;
      public float angularVelocity = 360f;
      public float angularVelocityVariance;
      public int bounceCount = 1;
      public float additionalBounceEnglish;
      public float decayOnBounce = 0.5f;
      public GameObject optionalBounceVFX;
      public tk2dSprite shadowSprite;
      [HideInInspector]
      public bool killTranslationOnBounce;
      public Action<DebrisObject> OnTouchedGround;
      public Action<DebrisObject> OnBounced;
      public Action<DebrisObject> OnGrounded;
      public bool usesLifespan;
      public float lifespanMin = 1f;
      public float lifespanMax = 1f;
      public bool shouldUseSRBMotion;
      public bool removeSRBOnGrounded;
      [NonSerialized]
      public bool PreventFallingInPits;
      public DebrisObject.DebrisPlacementOptions placementOptions;
      public bool DoesGoopOnRest;
      [ShowInInspectorIf("DoesGoopOnRest", false)]
      public GoopDefinition AssignedGoop;
      [ShowInInspectorIf("DoesGoopOnRest", false)]
      public float GoopRadius = 1f;
      [HideInInspector]
      public MinorBreakableGroupManager groupManager;
      [HideInInspector]
      public float additionalHeightBoost;
      [HideInInspector]
      public List<ParticleSystem> detachedParticleSystems;
      public System.Action OnTriggered;
      protected Bounds m_spriteBounds;
      protected float m_currentLifespan;
      protected float m_initialWorldDepth;
      [SerializeField]
      protected float m_finalWorldDepth = -1.5f;
      protected float m_startingHeightOffGround;
      protected bool m_hasBeenTriggered;
      protected bool isStatic = true;
      protected bool doesDecay;
      protected Vector3 m_startPosition;
      protected Vector3 m_velocity;
      protected Vector3 m_frameVelocity;
      protected Vector3 m_currentPosition;
      protected static DebrisObject.PitFallPoint[] m_STATIC_PitfallPoints;
      protected Transform m_transform;
      protected Renderer m_renderer;
      protected bool onGround;
      protected bool isFalling;
      protected bool isPitFalling;
      [NonSerialized]
      public bool PreventAbsorption;
      protected bool m_isPickupObject;
      protected bool m_forceUseFinalDepth;
      protected bool accurateDebris;
      protected bool m_recentlyBouncedOffTopwall;
      protected bool m_wasFacewallFixed;
      protected bool m_collisionsInitialized;
      protected bool m_forceCheckGrounded;
      protected bool m_isOnScreen = true;
      protected Dungeon m_dungeonRef;
      public bool ForceUpdateIfDisabled;
      private static int fgNonsenseLayerID = -1;
      private SpeculativeRigidbody m_platform;

      public static void ClearPerLevelData()
      {
        StaticReferenceManager.AllDebris.Clear();
        DebrisObject.m_STATIC_PitfallPoints = (DebrisObject.PitFallPoint[]) null;
        DebrisObject.SRB_Pits.Clear();
        DebrisObject.SRB_Walls.Clear();
        DebrisObject.m_STATIC_PitfallPoints = (DebrisObject.PitFallPoint[]) null;
      }

      public bool Static => this.isStatic;

      public float GravityOverride { get; set; }

      public bool HasBeenTriggered => this.m_hasBeenTriggered;

      public bool IsPickupObject => this.m_isPickupObject;

      public bool IsAccurateDebris
      {
        get => this.accurateDebris;
        set => this.accurateDebris = value;
      }

      public void ForceUpdatePitfall() => this.m_forceCheckGrounded = true;

      public bool DontSetLayer { get; set; }

      protected override void Awake()
      {
        base.Awake();
        if (DebrisObject.m_STATIC_PitfallPoints == null)
        {
          DebrisObject.m_STATIC_PitfallPoints = new DebrisObject.PitFallPoint[5];
          for (int index = 0; index < 5; ++index)
            DebrisObject.m_STATIC_PitfallPoints[index] = new DebrisObject.PitFallPoint((CellData) null, Vector3.zero);
        }
        this.m_dungeonRef = GameManager.Instance.Dungeon;
        StaticReferenceManager.AllDebris.Add(this);
      }

      public override void Start()
      {
        base.Start();
        if (DebrisObject.fgNonsenseLayerID == -1)
          DebrisObject.fgNonsenseLayerID = LayerMask.NameToLayer("FG_Nonsense");
        this.sprite.gameObject.SetLayerRecursively(DebrisObject.fgNonsenseLayerID);
        this.m_spriteBounds = this.sprite.GetBounds();
        if (!this.m_isPickupObject)
          this.m_isPickupObject = (UnityEngine.Object) this.GetComponent<PickupObject>() != (UnityEngine.Object) null;
        if (this.m_isPickupObject || (double) this.m_spriteBounds.size.x > (double) this.ACCURATE_DEBRIS_THRESHOLD || (double) this.m_spriteBounds.size.y > (double) this.ACCURATE_DEBRIS_THRESHOLD)
          this.accurateDebris = true;
        if ((UnityEngine.Object) this.sprite == (UnityEngine.Object) null)
          this.sprite = (tk2dBaseSprite) this.GetComponentInChildren<tk2dSprite>();
        if ((UnityEngine.Object) this.sprite != (UnityEngine.Object) null)
          DepthLookupManager.AssignRendererToSortingLayer(this.sprite.renderer, DepthLookupManager.GungeonSortingLayer.PLAYFIELD);
        if ((UnityEngine.Object) this.specRigidbody != (UnityEngine.Object) null && (UnityEngine.Object) this.GetComponent<MinorBreakable>() == (UnityEngine.Object) null)
          this.InitializeForCollisions();
        if (this.shouldUseSRBMotion || this.DontSetLayer)
          return;
        this.gameObject.SetLayerRecursively(LayerMask.NameToLayer("FG_Nonsense"));
      }

      public override void OnDespawned()
      {
        this.m_hasBeenTriggered = false;
        base.OnDespawned();
      }

      protected override void OnDestroy()
      {
        StaticReferenceManager.AllDebris.Remove(this);
        if ((bool) (UnityEngine.Object) this.specRigidbody)
          this.specRigidbody.OnPreRigidbodyCollision -= new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreCollision);
        base.OnDestroy();
      }

      public void FlagAsPickup() => this.m_isPickupObject = true;

      public void AssignFinalWorldDepth(float depth)
      {
        this.m_finalWorldDepth = depth;
        this.m_forceUseFinalDepth = true;
      }

      public void InitializeForCollisions()
      {
        if (this.m_collisionsInitialized)
          return;
        this.m_collisionsInitialized = true;
        if (!((UnityEngine.Object) this.specRigidbody != (UnityEngine.Object) null) || !((UnityEngine.Object) this.GetComponent<MinorBreakable>() == (UnityEngine.Object) null))
          return;
        this.specRigidbody.OnPreRigidbodyCollision += new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreCollision);
      }

      public void OnPreCollision(
        SpeculativeRigidbody myRigidbody,
        PixelCollider myCollider,
        SpeculativeRigidbody otherRigidbody,
        PixelCollider otherCollider)
      {
        if (!this.m_hasBeenTriggered)
        {
          this.shouldUseSRBMotion = true;
          Vector2 normalized = otherRigidbody.Velocity.normalized;
          float num = Mathf.Min(otherRigidbody.Velocity.magnitude, 5f);
          Vector2 vector2 = normalized * num;
          this.Trigger(Quaternion.Euler(0.0f, 0.0f, Mathf.Lerp(-30f, 30f, UnityEngine.Random.value)) * (vector2.normalized * UnityEngine.Random.Range(num * 0.75f, num * 1.25f)).ToVector3ZUp(1f), 0.5f);
          if (this.collisionStopsBullets)
            return;
          PhysicsEngine.SkipCollision = true;
        }
        else
          this.specRigidbody.OnPreRigidbodyCollision -= new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreCollision);
      }

      public void OnAnimationCompleted(tk2dSpriteAnimator a, tk2dSpriteAnimationClip c)
      {
        if (this.followupBehavior == DebrisObject.DebrisFollowupAction.FollowupAnimation)
          this.spriteAnimator.Play(this.followupIdentifier);
        this.spriteAnimator.AnimationCompleted = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>) null;
      }

      public void ForceReinitializePosition()
      {
        Vector2 vector2 = this.m_transform.position.XY();
        this.m_startPosition = new Vector3(vector2.x, vector2.y - this.m_startingHeightOffGround, this.m_startingHeightOffGround);
        this.m_currentPosition = this.m_startPosition;
      }

      public void Trigger(Vector3 startingForce, float startingHeight, float angularVelocityModifier = 1f)
      {
        if (this.m_hasBeenTriggered)
          return;
        if ((UnityEngine.Object) this.specRigidbody != (UnityEngine.Object) null && this.specRigidbody.enabled)
        {
          this.shouldUseSRBMotion = true;
          if (this.specRigidbody.PrimaryPixelCollider.CollisionLayer == CollisionLayer.BulletBlocker || this.specRigidbody.PrimaryPixelCollider.CollisionLayer == CollisionLayer.BulletBreakable)
            this.specRigidbody.CollideWithOthers = false;
        }
        else if ((UnityEngine.Object) this.specRigidbody == (UnityEngine.Object) null)
          this.shouldUseSRBMotion = false;
        if ((UnityEngine.Object) this.groupManager != (UnityEngine.Object) null)
          this.groupManager.DeregisterDebris(this);
        this.m_transform = this.transform;
        this.m_renderer = this.renderer;
        if ((UnityEngine.Object) this.sprite == (UnityEngine.Object) null)
          this.sprite = (tk2dBaseSprite) this.GetComponentInChildren<tk2dSprite>();
        this.m_initialWorldDepth = this.sprite.HeightOffGround;
        this.m_startingHeightOffGround = startingHeight;
        Vector2 vector2 = this.m_transform.position.XY();
        this.m_startPosition = new Vector3(vector2.x, vector2.y - startingHeight, startingHeight);
        this.m_currentPosition = this.m_startPosition;
        this.m_velocity = startingForce / this.inertialMass;
        if (this.usesLifespan)
          this.m_currentLifespan = UnityEngine.Random.Range(this.lifespanMin, this.lifespanMax);
        this.angularVelocity = this.canRotate ? this.angularVelocity + UnityEngine.Random.Range(-this.angularVelocityVariance, this.angularVelocityVariance) : 0.0f;
        this.angularVelocity *= angularVelocityModifier;
        this.m_hasBeenTriggered = true;
        this.isStatic = false;
        if (this.followupBehavior == DebrisObject.DebrisFollowupAction.FollowupAnimation && !string.IsNullOrEmpty(this.followupIdentifier))
        {
          this.spriteAnimator.AnimationCompleted += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.OnAnimationCompleted);
          this.spriteAnimator.Play();
        }
        else if (this.playAnimationOnTrigger)
        {
          if (this.usesDirectionalFallAnimations)
            this.spriteAnimator.Play(this.directionalAnimationData.GetAnimationForVector(startingForce.XY()));
          else
            this.spriteAnimator.Play();
        }
        if (this.OnTriggered == null)
          return;
        this.OnTriggered();
      }

      public void ClearVelocity()
      {
        this.m_velocity = Vector3.zero;
        this.m_frameVelocity = Vector3.zero;
      }

      public void ApplyFrameVelocity(Vector2 vel)
      {
        if (!this.enabled || !this.m_hasBeenTriggered || (double) this.m_currentPosition.z > 0.0)
          return;
        this.doesDecay = true;
        this.isStatic = false;
        this.m_frameVelocity += new Vector3(vel.x, vel.y, 0.0f) / this.inertialMass;
      }

      public void ApplyVelocity(Vector2 vel)
      {
        if (!this.enabled || !this.m_hasBeenTriggered || (double) this.m_currentPosition.z > 0.0)
          return;
        this.doesDecay = true;
        this.isStatic = false;
        this.angularVelocity = 0.0f;
        if (this.canRotate)
          this.angularVelocity = (float) UnityEngine.Random.Range(30, 90);
        this.m_velocity += new Vector3(vel.x, vel.y, 0.0f) / this.inertialMass;
      }

      protected CellData GetCellFromPosition(Vector3 p)
      {
        if ((UnityEngine.Object) this.m_dungeonRef == (UnityEngine.Object) null)
          this.m_dungeonRef = GameManager.Instance.Dungeon;
        IntVector2 intVector2 = p.IntXY(VectorConversions.Floor);
        return !this.m_dungeonRef.data.CheckInBounds(intVector2) ? (CellData) null : this.m_dungeonRef.data[intVector2];
      }

      protected bool CheckPositionFacewall(Vector3 position)
      {
        CellData cellFromPosition = this.GetCellFromPosition(position);
        if (cellFromPosition != null && cellFromPosition.IsAnyFaceWall())
          return true;
        for (int index = 0; index < DebrisObject.SRB_Walls.Count; ++index)
        {
          if (DebrisObject.SRB_Walls[index].ContainsPoint((Vector2) position))
            return true;
        }
        return false;
      }

      protected Tuple<CellData, Vector3> GetCellPositionTupleFromPosition(Vector3 p)
      {
        return Tuple.Create<CellData, Vector3>(this.GetCellFromPosition(p), p);
      }

      protected bool CheckCurrentCellsFacewall(Vector3 currentPosition)
      {
        Quaternion rotation = this.m_transform.rotation;
        currentPosition += rotation * this.m_spriteBounds.min;
        return this.CheckPositionFacewall(currentPosition + rotation * (0.5f * this.m_spriteBounds.size)) || this.accurateDebris && (this.CheckPositionFacewall(currentPosition) || this.CheckPositionFacewall(currentPosition + rotation * new Vector3(this.m_spriteBounds.size.x, 0.0f, 0.0f)) || this.CheckPositionFacewall(currentPosition + rotation * this.m_spriteBounds.size) || this.CheckPositionFacewall(currentPosition + rotation * new Vector3(0.0f, this.m_spriteBounds.size.y, 0.0f)));
      }

      private void RecalculateStaticTargetCells_ProcessPosition(
        Vector3 position,
        int index,
        DebrisObject.PitFallPoint[] targetArray)
      {
        DebrisObject.PitFallPoint target = targetArray[index];
        CellData cellFromPosition = this.GetCellFromPosition(position);
        target.cellData = cellFromPosition;
        target.position = position;
        target.inPit = false;
      }

      protected void RecalculateStaticTargetCells(Vector3 newPosition, Quaternion newRotation)
      {
        if (DebrisObject.m_STATIC_PitfallPoints == null)
          return;
        newPosition.z = 0.0f;
        newPosition += newRotation * this.m_spriteBounds.min;
        this.RecalculateStaticTargetCells_ProcessPosition(newPosition + newRotation * (0.5f * this.m_spriteBounds.size), 0, DebrisObject.m_STATIC_PitfallPoints);
        if (!this.accurateDebris)
          return;
        this.RecalculateStaticTargetCells_ProcessPosition(newPosition, 1, DebrisObject.m_STATIC_PitfallPoints);
        this.RecalculateStaticTargetCells_ProcessPosition(newPosition + newRotation * new Vector3(this.m_spriteBounds.size.x, 0.0f, 0.0f), 2, DebrisObject.m_STATIC_PitfallPoints);
        this.RecalculateStaticTargetCells_ProcessPosition(newPosition + newRotation * this.m_spriteBounds.size, 3, DebrisObject.m_STATIC_PitfallPoints);
        this.RecalculateStaticTargetCells_ProcessPosition(newPosition + newRotation * new Vector3(0.0f, this.m_spriteBounds.size.y, 0.0f), 4, DebrisObject.m_STATIC_PitfallPoints);
      }

      protected void HandleRotation(float adjustedDeltaTime)
      {
        if (!this.canRotate)
          return;
        int num = (double) this.m_velocity.x <= 0.0 ? 1 : -1;
        this.m_transform.RotateAround((Vector3) this.sprite.WorldCenter, Vector3.forward, this.angularVelocity * adjustedDeltaTime * (float) num);
        if (!this.IsPickupObject)
          return;
        this.sprite.ForceRotationRebuild();
      }

      protected virtual void UpdateVelocity(float adjustedDeltaTime)
      {
        if ((double) this.m_currentPosition.z <= 0.0)
          return;
        this.m_velocity += new Vector3(0.0f, 0.0f, -1f) * ((double) this.GravityOverride == 0.0 ? 10f : this.GravityOverride) * adjustedDeltaTime;
      }

      protected void HandleWallOrPitDeflection(
        IntVector2 currentGridCell,
        CellData nextCell,
        float adjustedDeltaTime)
      {
        if (this.name.Contains("Bomb"))
          UnityEngine.Debug.Log((object) "deflecto detecto");
        if (nextCell.IsAnyFaceWall() && !this.m_recentlyBouncedOffTopwall)
        {
          if (nextCell.position.x != currentGridCell.x)
            this.m_velocity.x = (float) (-(double) this.m_velocity.x * (1.0 - (double) this.decayOnBounce));
          this.m_velocity.y = (float) (-(double) Mathf.Abs(this.m_velocity.y) * (1.0 - (double) this.decayOnBounce));
          this.m_frameVelocity = Vector3.zero;
        }
        else
        {
          if (nextCell.position.x != currentGridCell.x)
          {
            this.m_velocity.x = (float) (-(double) this.m_velocity.x * (1.0 - (double) this.decayOnBounce));
            this.m_frameVelocity = Vector3.zero;
          }
          if (nextCell.position.y == currentGridCell.y)
            return;
          this.m_velocity.y = (float) (-(double) this.m_velocity.y * (1.0 - (double) this.decayOnBounce));
          this.m_frameVelocity = Vector3.zero;
        }
      }

      public Vector3 UnadjustedDebrisPosition => this.m_currentPosition;

      public void IncrementZHeight(float amount)
      {
        if (!this.HasBeenTriggered)
          return;
        this.isStatic = false;
        this.m_currentPosition.z += amount;
      }

      protected void ConvertYToZHeight(float amount)
      {
        this.m_currentPosition.y -= amount;
        this.m_currentPosition.z += amount;
      }

      protected bool CheckPitfallPointsForPit(
        ref DebrisObject.PitFallPoint[] p,
        ref SpeculativeRigidbody newPlatform)
      {
        if (!(bool) (UnityEngine.Object) this.m_transform || DebrisObject.m_STATIC_PitfallPoints == null)
          return false;
        this.RecalculateStaticTargetCells(this.m_transform.position, this.m_transform.rotation);
        p = DebrisObject.m_STATIC_PitfallPoints;
        int num1 = !this.accurateDebris ? 1 : 5;
        int num2 = 0;
        int num3 = 0;
        for (int index1 = 0; index1 < num1; ++index1)
        {
          CellData cellData = p[index1].cellData;
          if (cellData != null)
          {
            if (cellData.type == CellType.PIT && !this.PreventFallingInPits)
            {
              SpeculativeRigidbody platform = (SpeculativeRigidbody) null;
              if (GameManager.Instance.Dungeon.IsPixelOnPlatform(p[index1].position, out platform))
              {
                newPlatform = platform;
              }
              else
              {
                ++num2;
                p[index1].inPit = true;
                if (cellData.fallingPrevented)
                {
                  ++num3;
                  continue;
                }
                continue;
              }
            }
            if (DebrisObject.SRB_Pits != null && !this.PreventFallingInPits)
            {
              for (int index2 = 0; index2 < DebrisObject.SRB_Pits.Count; ++index2)
              {
                if (DebrisObject.SRB_Pits[index2].ContainsPoint((Vector2) p[index1].position, collideWithTriggers: true))
                {
                  p[index1].inPit = true;
                  ++num2;
                  break;
                }
              }
            }
          }
        }
        if (num2 <= Mathf.FloorToInt((float) num1 / 2f))
          return false;
        if (num2 - num3 > Mathf.FloorToInt((float) num1 / 2f))
          return true;
        this.m_forceCheckGrounded = true;
        return false;
      }

      protected void ForceCheckForPitfall()
      {
        this.m_forceCheckGrounded = false;
        DebrisObject.PitFallPoint[] p = (DebrisObject.PitFallPoint[]) null;
        SpeculativeRigidbody newPlatform = (SpeculativeRigidbody) null;
        if (this.PreventFallingInPits || !this.CheckPitfallPointsForPit(ref p, ref newPlatform))
          return;
        this.FallIntoPit(p);
      }

      protected void EnsurePickupsAreNicelyDistant(float realDeltaTime)
      {
        if (!this.IsPickupObject || !this.onGround)
          return;
        PickupObject component = this.GetComponent<PickupObject>();
        if ((UnityEngine.Object) component != (UnityEngine.Object) null && component is CurrencyPickup)
          return;
        for (int index = 0; index < StaticReferenceManager.AllDebris.Count; ++index)
        {
          DebrisObject allDebri = StaticReferenceManager.AllDebris[index];
          if ((bool) (UnityEngine.Object) allDebri && allDebri.IsPickupObject && (UnityEngine.Object) allDebri != (UnityEngine.Object) this && allDebri.onGround)
            this.MovePickupAwayFromObject((BraveBehaviour) allDebri, 1.5f, 0.5f);
        }
      }

      private void MovePickupAwayFromObject(BraveBehaviour otherObject, float minDist, float power)
      {
        Vector2 vector2 = (!((UnityEngine.Object) otherObject.sprite != (UnityEngine.Object) null) ? otherObject.transform.position.XY() + new Vector2(0.5f, 0.5f) : otherObject.sprite.WorldCenter) - (!((UnityEngine.Object) this.sprite != (UnityEngine.Object) null) ? this.transform.position.XY() + new Vector2(0.5f, 0.5f) : this.sprite.WorldCenter);
        if ((double) vector2.magnitude >= (double) minDist)
          return;
        if (otherObject is DebrisObject)
          ((DebrisObject) otherObject).ApplyFrameVelocity(power * vector2.normalized);
        this.ApplyFrameVelocity(power * vector2.normalized * -1f);
      }

      protected override void InvariantUpdate(float realDeltaTime)
      {
        if (!this.enabled && !this.ForceUpdateIfDisabled || !this.m_hasBeenTriggered || this.isPitFalling)
          return;
        if (this.IsPickupObject && (bool) (UnityEngine.Object) this.sprite && !this.isFalling)
          this.sprite.HeightOffGround = Mathf.Max(this.sprite.HeightOffGround, -1f);
        if ((double) this.motionMultiplier <= 0.0)
        {
          this.m_currentPosition.z = 0.0f;
          this.m_velocity = Vector3.zero;
          this.m_frameVelocity = Vector3.zero;
          this.isStatic = true;
          this.OnBecameGrounded();
        }
        SpeculativeRigidbody platform1 = this.m_platform;
        this.m_platform = (SpeculativeRigidbody) null;
        if (this.usesLifespan)
        {
          this.m_currentLifespan -= realDeltaTime;
          if ((double) this.m_currentLifespan <= 0.0)
            UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
        }
        if (this.m_forceCheckGrounded && this.onGround)
          this.ForceCheckForPitfall();
        if (this.onGround)
          this.EnsurePickupsAreNicelyDistant(realDeltaTime);
        if (this.isStatic)
        {
          this.m_platform = platform1;
        }
        else
        {
          this.m_forceCheckGrounded = false;
          IntVector2 vec = new IntVector2(Mathf.FloorToInt(this.m_transform.position.x), Mathf.FloorToInt(this.m_transform.position.y));
          if (this.IsCorpse && (bool) (UnityEngine.Object) this.sprite)
            vec = this.sprite.WorldCenter.ToIntVector2(VectorConversions.Floor);
          if ((UnityEngine.Object) GameManager.Instance.Dungeon != (UnityEngine.Object) null && !GameManager.Instance.Dungeon.data.CheckInBounds(vec))
          {
            UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
          }
          else
          {
            CellData cellData1 = !((UnityEngine.Object) GameManager.Instance.Dungeon != (UnityEngine.Object) null) ? (CellData) null : GameManager.Instance.Dungeon.data.cellData[vec.x][vec.y];
            if (cellData1 == null)
            {
              if (this.accurateDebris)
                UnityEngine.Debug.LogError((object) ("Destroying large debris for being outside valid cell ranges! " + this.name));
              this.MaybeRespawnIfImportant();
              UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
            }
            else
            {
              float adjustedDeltaTime = Mathf.Clamp(realDeltaTime * this.motionMultiplier, 0.0f, 0.1f);
              bool flag1 = this.CheckCurrentCellsFacewall(this.m_transform.position);
              if ((double) this.m_currentPosition.z > 0.0 || (double) this.m_velocity.z > 0.0 || this.doesDecay || this.isFalling)
              {
                if ((double) this.m_currentPosition.z <= 0.0 && !this.isFalling)
                {
                  this.m_velocity.z = 0.0f;
                  this.m_currentPosition.z = 0.0f;
                }
                this.HandleRotation(adjustedDeltaTime);
                Vector3 newPosition = this.m_currentPosition + this.m_velocity * adjustedDeltaTime + this.m_frameVelocity * adjustedDeltaTime;
                if ((UnityEngine.Object) GameManager.Instance.Dungeon != (UnityEngine.Object) null)
                {
                  this.RecalculateStaticTargetCells(newPosition, this.m_transform.rotation);
                  DebrisObject.PitFallPoint[] staticPitfallPoints = DebrisObject.m_STATIC_PitfallPoints;
                  int num1 = 0;
                  int num2 = 0;
                  int num3 = !this.accurateDebris ? 1 : 5;
                  for (int index1 = 0; index1 < num3; ++index1)
                  {
                    if (staticPitfallPoints != null && staticPitfallPoints.Length > index1 && staticPitfallPoints[index1] != null)
                    {
                      CellData cellData2 = staticPitfallPoints[index1].cellData;
                      if (cellData2 != null)
                      {
                        bool flag2 = (double) this.m_currentPosition.z <= 0.0 ? cellData2.type == CellType.WALL : cellData2.type == CellType.WALL && !cellData2.IsLowerFaceWall();
                        bool flag3 = this.m_isPickupObject && GameManager.Instance.Dungeon.data.isTopWall(cellData2.position.x, cellData2.position.y);
                        if (this.m_isPickupObject)
                        {
                          bool flag4 = cellData2.parentArea != null && cellData2.parentArea.PrototypeRoomSpecialSubcategory == PrototypeDungeonRoom.RoomSpecialSubCategory.WEIRD_SHOP;
                          flag2 = flag2 || !flag4 && cellData2.type == CellType.PIT;
                        }
                        if (flag3)
                        {
                          this.m_recentlyBouncedOffTopwall = true;
                          this.m_velocity.y = Mathf.Abs(this.m_velocity.z) + 1f;
                          flag2 = true;
                        }
                        if (flag2)
                        {
                          CellData cellFromPosition = this.GetCellFromPosition(this.m_currentPosition + this.m_transform.rotation * (0.5f * this.m_spriteBounds.size));
                          this.HandleWallOrPitDeflection(cellFromPosition == null ? vec : cellFromPosition.position, cellData2, adjustedDeltaTime);
                          break;
                        }
                        bool flag5 = cellData2.type == CellType.PIT;
                        if (!flag5)
                        {
                          for (int index2 = 0; index2 < DebrisObject.SRB_Pits.Count; ++index2)
                          {
                            if (DebrisObject.SRB_Pits[index2].ContainsPoint((Vector2) staticPitfallPoints[index1].position, collideWithTriggers: true))
                            {
                              flag5 = true;
                              break;
                            }
                          }
                        }
                        if ((double) this.m_currentPosition.z <= 0.0 && flag5 && !this.PreventFallingInPits)
                        {
                          SpeculativeRigidbody platform2;
                          if (GameManager.Instance.Dungeon.IsPixelOnPlatform(staticPitfallPoints[index1].position, out platform2))
                          {
                            this.m_platform = platform2;
                          }
                          else
                          {
                            ++num1;
                            staticPitfallPoints[index1].inPit = true;
                            if (cellData2.fallingPrevented)
                              ++num2;
                          }
                        }
                      }
                    }
                  }
                  if (num1 > Mathf.FloorToInt((float) num3 / 2f) && !this.PreventFallingInPits)
                  {
                    if (!this.PreventFallingInPits && num1 - num2 > Mathf.FloorToInt((float) num3 / 2f))
                      this.FallIntoPit(staticPitfallPoints);
                    else
                      this.m_forceCheckGrounded = true;
                  }
                  if (this.m_isPickupObject && GameManager.Instance.Dungeon.data.isTopWall(cellData1.position.x, cellData1.position.y))
                  {
                    this.m_recentlyBouncedOffTopwall = true;
                    this.m_velocity.y = Mathf.Abs(this.m_velocity.z) + 1f;
                  }
                }
                Vector3 vector3 = this.m_currentPosition + this.m_velocity * adjustedDeltaTime + this.m_frameVelocity * adjustedDeltaTime;
                this.m_frameVelocity = Vector3.zero;
                if (this.shouldUseSRBMotion)
                {
                  this.m_transform.position = new Vector3(vector3.x, vector3.y + vector3.z, this.m_transform.position.z);
                  if (this.IsPickupObject)
                    this.m_transform.position = this.m_transform.position.Quantize(1f / 16f);
                  if ((bool) (UnityEngine.Object) this.sprite && (bool) (UnityEngine.Object) this.shadowSprite)
                  {
                    this.m_transform.position = this.m_transform.position.Quantize(1f / 16f);
                    this.shadowSprite.PlaceAtPositionByAnchor((Vector3) this.sprite.WorldBottomCenter.WithY(this.m_transform.position.y + 1f / 16f), tk2dBaseSprite.Anchor.MiddleCenter);
                    this.shadowSprite.transform.position = (this.shadowSprite.transform.position + new Vector3(0.0f, -vector3.z, 0.0f)).Quantize(1f / 16f);
                  }
                  this.specRigidbody.Reinitialize();
                }
                else
                {
                  this.m_transform.position = new Vector3(vector3.x, vector3.y + vector3.z, this.m_transform.position.z);
                  if ((bool) (UnityEngine.Object) this.sprite && (bool) (UnityEngine.Object) this.shadowSprite)
                  {
                    this.m_transform.position = this.m_transform.position.Quantize(1f / 16f);
                    this.shadowSprite.PlaceAtPositionByAnchor((Vector3) this.sprite.WorldBottomCenter.WithY(this.m_transform.position.y + 1f / 16f), tk2dBaseSprite.Anchor.MiddleCenter);
                    this.shadowSprite.transform.position = (this.shadowSprite.transform.position + new Vector3(0.0f, -vector3.z, 0.0f)).Quantize(1f / 16f);
                  }
                }
                this.UpdateVelocity(adjustedDeltaTime);
                this.m_currentPosition = vector3;
                if (!this.onGround && !this.isFalling)
                  this.sprite.HeightOffGround = this.m_currentPosition.z + this.additionalHeightBoost;
                if (this.doesDecay)
                {
                  this.m_velocity *= 0.97f;
                  if ((double) this.m_velocity.magnitude < 0.5)
                  {
                    this.doesDecay = false;
                    this.m_velocity = Vector3.zero;
                  }
                }
              }
              else
              {
                SpeculativeRigidbody newPlatform = (SpeculativeRigidbody) null;
                if (this.OnTouchedGround != null)
                  this.OnTouchedGround(this);
                DebrisObject.PitFallPoint[] p = (DebrisObject.PitFallPoint[]) null;
                if ((UnityEngine.Object) GameManager.Instance.Dungeon == (UnityEngine.Object) null)
                {
                  UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject, 1f);
                  this.isStatic = true;
                }
                else if (flag1 && !this.m_recentlyBouncedOffTopwall && !this.m_wasFacewallFixed)
                {
                  while ((double) this.m_currentPosition.z < 0.0)
                    this.ConvertYToZHeight(0.5f);
                  this.isStatic = false;
                  this.m_wasFacewallFixed = true;
                }
                else if (this.m_isPickupObject && cellData1.IsTopWall())
                {
                  this.m_recentlyBouncedOffTopwall = true;
                  this.ConvertYToZHeight(0.5f);
                  this.m_velocity.y = Mathf.Max(1f, Mathf.Abs(this.m_velocity.y));
                }
                else if (cellData1.type == CellType.WALL && !this.m_isPickupObject && !this.IsAccurateDebris)
                {
                  this.MaybeRespawnIfImportant();
                  UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
                }
                else if (!this.PreventFallingInPits && this.CheckPitfallPointsForPit(ref p, ref newPlatform))
                  this.FallIntoPit(p);
                else if (!this.m_isPickupObject && !this.PreventAbsorption && cellData1.cellVisualData.floorType == CellVisualData.CellFloorType.Water && cellData1.cellVisualData.absorbsDebris)
                {
                  if ((bool) (UnityEngine.Object) this.sprite)
                    GameManager.Instance.Dungeon.DoSplashDustupAtPosition(this.sprite.WorldCenter);
                  else
                    GameManager.Instance.Dungeon.DoSplashDustupAtPosition(this.transform.position.XY());
                  UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
                }
                else if (this.bounceCount > 0 && (UnityEngine.Object) this.GetComponent<MinorBreakable>() == (UnityEngine.Object) null)
                {
                  if (!string.IsNullOrEmpty(this.audioEventName))
                  {
                    int num = (int) AkSoundEngine.PostEvent(this.audioEventName, this.gameObject);
                  }
                  this.m_velocity = this.m_velocity.WithZ(Mathf.Min(5f, this.m_velocity.z * -1f)) * (1f - this.decayOnBounce);
                  if (this.killTranslationOnBounce)
                    this.m_velocity = Vector3.zero.WithZ(this.m_velocity.z);
                  if (this.canRotate && (double) this.additionalBounceEnglish > 0.0)
                    this.angularVelocity += Mathf.Sign(this.angularVelocity) * this.additionalBounceEnglish;
                  if ((UnityEngine.Object) this.optionalBounceVFX != (UnityEngine.Object) null)
                    SpawnManager.SpawnVFX(this.optionalBounceVFX).GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor((Vector3) this.sprite.WorldCenter, tk2dBaseSprite.Anchor.MiddleCenter);
                  if (this.DoesGoopOnRest)
                    DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(this.AssignedGoop).AddGoopCircle(this.sprite.WorldCenter, this.GoopRadius);
                  this.m_currentPosition = this.m_currentPosition.WithZ(0.05f);
                  if (this.OnBounced != null)
                    this.OnBounced(this);
                  --this.bounceCount;
                }
                else if (this.m_isPickupObject && cellData1.isOccupied && this.IsVitalPickup())
                {
                  this.m_velocity = new Vector3(0.0f, -3f, 1f);
                  this.ConvertYToZHeight(2f);
                }
                else
                {
                  if ((UnityEngine.Object) newPlatform != (UnityEngine.Object) null)
                    this.m_platform = newPlatform;
                  this.OnBecameGrounded();
                }
              }
              if ((UnityEngine.Object) platform1 != (UnityEngine.Object) null && (UnityEngine.Object) this.m_platform == (UnityEngine.Object) null)
                this.transform.parent = !SpawnManager.HasInstance ? (Transform) null : SpawnManager.Instance.VFX;
              if (!(bool) (UnityEngine.Object) this.sprite)
                return;
              this.sprite.UpdateZDepth();
            }
          }
        }
      }

      protected void OnBecameGrounded()
      {
        this.isStatic = true;
        if (this.detachedParticleSystems != null && this.detachedParticleSystems.Count > 0)
        {
          for (int index = 0; index < this.detachedParticleSystems.Count; ++index)
          {
            if ((bool) (UnityEngine.Object) this.detachedParticleSystems[index])
              BraveUtility.EnableEmission(this.detachedParticleSystems[index], false);
          }
        }
        if ((UnityEngine.Object) this.GetComponent<BlackHoleDoer>() == (UnityEngine.Object) null)
        {
          GunParticleSystemController systemController = (GunParticleSystemController) null;
          if (this.IsPickupObject)
            systemController = this.GetComponentInChildren<GunParticleSystemController>();
          ParticleSystem[] componentsInChildren = this.GetComponentsInChildren<ParticleSystem>();
          if (componentsInChildren != null && componentsInChildren.Length > 0)
          {
            for (int index = 0; index < componentsInChildren.Length; ++index)
            {
              if (!(bool) (UnityEngine.Object) systemController || !((UnityEngine.Object) systemController.TargetSystem == (UnityEngine.Object) componentsInChildren[index]))
              {
                componentsInChildren[index].Stop();
                UnityEngine.Object.Destroy((UnityEngine.Object) componentsInChildren[index]);
              }
            }
          }
        }
        if (!this.onGround && !string.IsNullOrEmpty(this.audioEventName))
        {
          int num = (int) AkSoundEngine.PostEvent(this.audioEventName, this.gameObject);
        }
        this.onGround = true;
        if (this.shouldUseSRBMotion)
        {
          this.specRigidbody.Velocity = Vector2.zero;
          PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(this.specRigidbody);
        }
        this.sprite.attachParent = (tk2dBaseSprite) null;
        if (!this.m_isPickupObject)
        {
          this.sprite.IsPerpendicular = false;
          this.sprite.HeightOffGround = this.m_finalWorldDepth;
          this.sprite.SortingOrder = 0;
        }
        else if (this.m_forceUseFinalDepth)
          this.sprite.HeightOffGround = this.m_finalWorldDepth;
        this.sprite.UpdateZDepth();
        if (this.changesCollisionLayer && (UnityEngine.Object) this.specRigidbody != (UnityEngine.Object) null)
        {
          this.specRigidbody.PrimaryPixelCollider.CollisionLayer = this.groundedCollisionLayer;
          this.specRigidbody.ForceRegenerate();
        }
        if (this.breaksOnFall && (double) UnityEngine.Random.value < (double) this.breakOnFallChance)
        {
          MinorBreakable component = this.GetComponent<MinorBreakable>();
          if ((UnityEngine.Object) component != (UnityEngine.Object) null)
          {
            component.heightOffGround = 0.05f;
            component.Break(this.m_velocity.XY().normalized * 1.5f);
          }
        }
        if (this.DoesGoopOnRest)
          DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(this.AssignedGoop).AddGoopCircle(this.sprite.WorldCenter, this.GoopRadius);
        if (this.removeSRBOnGrounded)
        {
          this.shouldUseSRBMotion = false;
          if ((UnityEngine.Object) this.specRigidbody != (UnityEngine.Object) null)
          {
            this.specRigidbody.enabled = false;
            UnityEngine.Object.Destroy((UnityEngine.Object) this.specRigidbody);
          }
        }
        if ((UnityEngine.Object) this.m_platform != (UnityEngine.Object) null)
          this.transform.parent = this.m_platform.transform;
        if (this.OnGrounded != null)
          this.OnGrounded(this);
        switch (this.followupBehavior)
        {
          case DebrisObject.DebrisFollowupAction.GroundedAnimation:
            if (!string.IsNullOrEmpty(this.followupIdentifier))
            {
              this.spriteAnimator.Play(this.followupIdentifier);
              break;
            }
            break;
          case DebrisObject.DebrisFollowupAction.GroundedSprite:
            if (!string.IsNullOrEmpty(this.followupIdentifier))
            {
              this.sprite.SetSprite(this.followupIdentifier);
              break;
            }
            break;
          case DebrisObject.DebrisFollowupAction.StopAnimationOnGrounded:
            this.spriteAnimator.Stop();
            break;
        }
        if (this.m_isPickupObject)
        {
          RoomHandler roomFromPosition1 = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(this.transform.position.IntXY(VectorConversions.Floor));
          GameObject gameObjectWithTag = GameObject.FindGameObjectWithTag("SellCellController");
          SellCellController sellCellController = (SellCellController) null;
          if ((UnityEngine.Object) gameObjectWithTag != (UnityEngine.Object) null)
            sellCellController = gameObjectWithTag.GetComponent<SellCellController>();
          PickupObject componentInChildren = this.GetComponentInChildren<PickupObject>();
          if (!((UnityEngine.Object) sellCellController != (UnityEngine.Object) null))
            return;
          RoomHandler roomFromPosition2 = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(sellCellController.transform.position.IntXY());
          if (roomFromPosition1 != roomFromPosition2)
            return;
          sellCellController.AttemptSellItem(componentInChildren);
        }
        else
        {
          if (this.Priority <= EphemeralObject.EphemeralPriority.Middling || GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.END_TIMES)
            return;
          this.sprite.HeightOffGround -= UnityEngine.Random.Range(0.0f, 20f);
        }
      }

      public void FadeToOverrideColor(Color targetColor, float duration, float startAlpha = 0.0f)
      {
        this.StartCoroutine(this.HandleOverrideColorFade(targetColor, duration, startAlpha));
      }

      [DebuggerHidden]
      private IEnumerator HandleOverrideColorFade(Color targetColor, float duration, float startAlpha = 0.0f)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new DebrisObject.<HandleOverrideColorFade>c__Iterator0()
        {
          targetColor = targetColor,
          startAlpha = startAlpha,
          duration = duration,
          _this = this
        };
      }

      protected void FallIntoPit(DebrisObject.PitFallPoint[] nextCells = null)
      {
        if (this.isFalling)
          return;
        this.isFalling = true;
        if (this.animatePitFall)
        {
          this.StartAnimatedPitFall(nextCells, (Vector2) this.m_velocity);
        }
        else
        {
          if ((bool) (UnityEngine.Object) this.m_renderer)
          {
            DepthLookupManager.AssignRendererToSortingLayer(this.m_renderer, DepthLookupManager.GungeonSortingLayer.BACKGROUND);
            this.m_renderer.sortingOrder = 0;
            this.m_renderer.material.renderQueue = 2450;
          }
          this.gameObject.SetLayerRecursively(LayerMask.NameToLayer("BG_Critical"));
          this.additionalHeightBoost = GameManager.PIT_DEPTH;
          if ((bool) (UnityEngine.Object) this.sprite)
          {
            this.sprite.HeightOffGround = GameManager.PIT_DEPTH;
            this.sprite.usesOverrideMaterial = true;
            this.sprite.IsPerpendicular = false;
            this.sprite.UpdateZDepth();
          }
          if ((bool) (UnityEngine.Object) this.m_renderer)
            this.m_renderer.material.shader = ShaderCache.Acquire("Brave/DebrisPitfallShader");
          float duration = 0.25f;
          if (GameManager.Instance.IsFoyer)
            duration = 2f;
          else if (GameManager.Instance.Dungeon.IsEndTimes)
            duration = 5f;
          if ((bool) (UnityEngine.Object) this.sprite && GameManager.Instance.Dungeon.tileIndices.PitAtPositionIsWater(this.sprite.WorldCenter))
          {
            this.StartCoroutine(this.HandleSplashDeath());
          }
          else
          {
            this.FadeToOverrideColor(Color.black, duration, 0.5f);
            if ((bool) (UnityEngine.Object) this.GetComponent<NPCCellKeyItem>())
              this.GetComponent<NPCCellKeyItem>().IsBeingDestroyed = true;
            this.MaybeRespawnIfImportant();
            UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject, duration + 0.1f);
          }
        }
      }

      [DebuggerHidden]
      private IEnumerator HandleSplashDeath()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new DebrisObject.<HandleSplashDeath>c__Iterator1()
        {
          _this = this
        };
      }

      private void StartAnimatedPitFall(DebrisObject.PitFallPoint[] nextCells, Vector2 velocity)
      {
        List<IntVector2> intVector2List = new List<IntVector2>();
        if (nextCells != null && this.accurateDebris)
        {
          if (nextCells[0].inPit && nextCells[1].inPit && nextCells[4].inPit)
            intVector2List.Add(IntVector2.Left);
          else if (nextCells[0].inPit && nextCells[2].inPit && nextCells[3].inPit)
            intVector2List.Add(IntVector2.Right);
          else if (nextCells[0].inPit && nextCells[3].inPit && nextCells[4].inPit)
            intVector2List.Add(IntVector2.Up);
          else if (nextCells[0].inPit && nextCells[1].inPit && nextCells[2].inPit)
            intVector2List.Add(IntVector2.Down);
        }
        if (intVector2List.Count == 0)
          intVector2List.Add(BraveUtility.GetIntMajorAxis(velocity));
        this.StartCoroutine(this.StartFallAnimation(!intVector2List.Contains(BraveUtility.GetIntMajorAxis(velocity)) ? intVector2List[0] : BraveUtility.GetIntMajorAxis(velocity), velocity));
      }

      [DebuggerHidden]
      private IEnumerator StartFallAnimation(IntVector2 dir, Vector2 debrisVelocity)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new DebrisObject.<StartFallAnimation>c__Iterator2()
        {
          dir = dir,
          debrisVelocity = debrisVelocity,
          _this = this
        };
      }

      private bool IsVitalPickup()
      {
        if ((bool) (UnityEngine.Object) this && this.IsPickupObject)
        {
          PickupObject componentInChildren = this.GetComponentInChildren<PickupObject>();
          if ((bool) (UnityEngine.Object) componentInChildren && (componentInChildren is CurrencyPickup && (componentInChildren as CurrencyPickup).IsMetaCurrency || componentInChildren is NPCCellKeyItem))
            return true;
        }
        return false;
      }

      public void ForceDestroyAndMaybeRespawn()
      {
        this.MaybeRespawnIfImportant();
        UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
      }

      private void MaybeRespawnIfImportant()
      {
        if (!(bool) (UnityEngine.Object) this || !this.IsPickupObject)
          return;
        PickupObject componentInChildren = this.GetComponentInChildren<PickupObject>();
        if (!(bool) (UnityEngine.Object) componentInChildren || !componentInChildren.RespawnsIfPitfall)
          return;
        bool flag = false;
        if (componentInChildren is CurrencyPickup)
        {
          (componentInChildren as CurrencyPickup).ForceSetPickedUp();
          List<RewardPedestal> componentsAbsoluteInRoom = this.transform.position.GetAbsoluteRoom().GetComponentsAbsoluteInRoom<RewardPedestal>();
          if ((componentInChildren as CurrencyPickup).IsMetaCurrency && componentsAbsoluteInRoom != null && componentsAbsoluteInRoom.Count > 0)
          {
            flag = true;
            LootEngine.SpawnItem(PickupObjectDatabase.GetById(componentInChildren.PickupObjectId).gameObject, (componentsAbsoluteInRoom[0].specRigidbody.UnitCenter + UnityEngine.Random.insideUnitCircle.normalized * 3f).ToVector3ZisY(), Vector2.zero, 0.0f, doDefaultItemPoof: true);
          }
        }
        if (flag)
          return;
        PlayerController bestActivePlayer = GameManager.Instance.BestActivePlayer;
        if ((bool) (UnityEngine.Object) bestActivePlayer)
          LootEngine.SpawnItem(PickupObjectDatabase.GetById(componentInChildren.PickupObjectId).gameObject, bestActivePlayer.CenterPosition.ToVector3ZUp(), Vector2.zero, 0.0f, doDefaultItemPoof: !(componentInChildren is CurrencyPickup));
        else
          LootEngine.SpawnItem(PickupObjectDatabase.GetById(componentInChildren.PickupObjectId).gameObject, this.transform.position.GetAbsoluteRoom().GetCenteredVisibleClearSpot(2, 2).ToVector3(), Vector2.zero, 0.0f, doDefaultItemPoof: !(componentInChildren is CurrencyPickup));
      }

      [Serializable]
      public struct DebrisPlacementOptions
      {
        public bool canBeRotated;
        public bool canBeFlippedHorizontally;
        public bool canBeFlippedVertically;
      }

      public enum DebrisFollowupAction
      {
        None,
        FollowupAnimation,
        GroundedAnimation,
        GroundedSprite,
        StopAnimationOnGrounded,
      }

      protected class PitFallPoint
      {
        public CellData cellData;
        public Vector3 position;
        public bool inPit;

        public PitFallPoint(CellData cellData, Vector3 position)
        {
          this.cellData = cellData;
          this.position = position;
        }
      }
    }

}
