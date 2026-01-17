// Decompiled with JetBrains decompiler
// Type: PhysicsEngine
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using BraveDynamicTree;
using Dungeonator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Profiling;

#nullable disable
public class PhysicsEngine : MonoBehaviour
{
  public static CustomSampler csSortRigidbodies;
  public static CustomSampler csPreRigidbodyMovement;
  public static CustomSampler csPreprocessing;
  public static CustomSampler csInitialRigidbodyUpdates;
  public static CustomSampler csRigidbodyCollisions;
  public static CustomSampler csInitCollisions;
  public static CustomSampler csBuildStepList;
  public static CustomSampler csMovementRestrictions;
  public static CustomSampler csCollideWithOthers;
  public static CustomSampler csCollideWithTilemap;
  public static CustomSampler csCollideWithTilemapInner;
  public static CustomSampler csCanCollideWith;
  public static CustomSampler csRigidbodyPushing;
  public static CustomSampler csResolveCollision;
  public static CustomSampler csUpdatePositionVelocity;
  public static CustomSampler csHandleCarriedObjects;
  public static CustomSampler csEndChecks;
  public static CustomSampler csEndCleanup;
  public static CustomSampler csUpdateZDepth;
  public static CustomSampler csPostRigidbodyMovement;
  public static CustomSampler csHandleTriggerCollisions;
  public static CustomSampler csClearGhostCollisions;
  public static CustomSampler csRaycastTiles;
  public static CustomSampler csRaycastRigidbodies;
  public static CustomSampler csTreeCollisions;
  public static CustomSampler csRigidbodyTreeSearch;
  public static CustomSampler csProjectileTreeSearch;
  public static CustomSampler csUpdatePosition;
  public static CustomSampler csGetNextNearbyTile;
  public static CustomSampler csCollideWithTilemapSetup;
  public static CustomSampler csGetTiles;
  public static CustomSampler csCollideWithTilemapSingle;
  public tk2dTileMap TileMap;
  public int PixelsPerUnit = 16 /*0x10*/;
  private const int c_warnIterations = 5;
  private const int c_maxIterations = 50;
  public PhysicsEngine.DebugDrawType DebugDraw;
  [HideInInspector]
  public Color[] DebugColors = new Color[3]
  {
    Color.green,
    Color.magenta,
    Color.cyan
  };
  private List<SpeculativeRigidbody> m_rigidbodies = new List<SpeculativeRigidbody>();
  private b2DynamicTree m_rigidbodyTree = new b2DynamicTree();
  private b2DynamicTree m_projectileTree = new b2DynamicTree();
  private HashSet<IntVector2> m_debugTilesDrawnThisFrame = new HashSet<IntVector2>();
  private static List<SpeculativeRigidbody> c_boundedRigidbodies = new List<SpeculativeRigidbody>();
  private List<SpeculativeRigidbody> m_deregisterRigidBodies = new List<SpeculativeRigidbody>();
  private int m_frameCount;
  private int m_cachedProjectileMask;
  private static PhysicsEngine m_instance;
  public static LinearCastResult PendingCastResult;
  private SpeculativeRigidbody[] m_emptyIgnoreList = new SpeculativeRigidbody[0];
  private SpeculativeRigidbody[] m_singleIgnoreList = new SpeculativeRigidbody[1];
  private static PhysicsEngine.Raycaster m_raycaster = new PhysicsEngine.Raycaster();
  private SpeculativeRigidbody[] emptyIgnoreList = new SpeculativeRigidbody[0];
  private static PhysicsEngine.RigidbodyCaster m_rigidbodyCaster = new PhysicsEngine.RigidbodyCaster();
  private static SpeculativeRigidbody m_cwrqRigidbody;
  private static List<PixelCollider.StepData> m_cwrqStepList;
  private static CollisionData m_cwrqCollisionData;
  private int m_cachedDungeonWidth;
  private int m_cachedDungeonHeight;
  private PhysicsEngine.NearbyTileData m_nbt;

  public List<SpeculativeRigidbody> AllRigidbodies => this.m_rigidbodies;

  public static PhysicsEngine Instance
  {
    get => PhysicsEngine.m_instance;
    set => PhysicsEngine.m_instance = value;
  }

  public static bool HasInstance => (UnityEngine.Object) PhysicsEngine.m_instance != (UnityEngine.Object) null;

  public static bool SkipCollision { get; set; }

  public static bool? CollisionHaltsVelocity { get; set; }

  public static bool HaltRemainingMovement { get; set; }

  public static Vector2? PostSliceVelocity { get; set; }

  public float PixelUnitWidth => 1f / (float) this.PixelsPerUnit;

  public float HalfPixelUnitWidth => 0.5f / (float) this.PixelsPerUnit;

  public event System.Action OnPreRigidbodyMovement;

  public event System.Action OnPostRigidbodyMovement;

  private void Awake()
  {
    PhysicsEngine.m_instance = this;
    if ((UnityEngine.Object) this.TileMap == (UnityEngine.Object) null)
      this.TileMap = UnityEngine.Object.FindObjectOfType<tk2dTileMap>();
    this.m_cachedProjectileMask = CollisionMask.LayerToMask(CollisionLayer.Projectile);
  }

  [Conditional("PROFILE_PHYSICS")]
  public static void ProfileBegin(CustomSampler sampler)
  {
  }

  [Conditional("PROFILE_PHYSICS")]
  public static void ProfileEnd(CustomSampler sampler)
  {
  }

  private void Update()
  {
  }

  private void OnDestroy()
  {
    if (!((UnityEngine.Object) PhysicsEngine.m_instance == (UnityEngine.Object) this))
      return;
    PhysicsEngine.m_instance = (PhysicsEngine) null;
    PhysicsEngine.PendingCastResult = (LinearCastResult) null;
    this.m_deregisterRigidBodies.Clear();
    PhysicsEngine.c_boundedRigidbodies.Clear();
    PhysicsEngine.m_cwrqRigidbody = (SpeculativeRigidbody) null;
    PhysicsEngine.m_cwrqStepList = (List<PixelCollider.StepData>) null;
    PhysicsEngine.m_cwrqCollisionData = (CollisionData) null;
  }

  private void LateUpdate()
  {
    if ((double) UnityEngine.Time.timeScale == 0.0 || (double) BraveTime.DeltaTime == 0.0)
      return;
    DungeonData data = GameManager.Instance.Dungeon.data;
    this.m_cachedDungeonWidth = data.Width;
    this.m_cachedDungeonHeight = data.Height;
    ++this.m_frameCount;
    if (this.m_frameCount > 5)
    {
      this.SortRigidbodies();
      if (this.OnPreRigidbodyMovement != null)
        this.OnPreRigidbodyMovement();
      Dungeon dungeon = GameManager.Instance.Dungeon;
      for (int index1 = 0; index1 < this.m_rigidbodies.Count; ++index1)
      {
        SpeculativeRigidbody rigidbody = this.m_rigidbodies[index1];
        if (rigidbody.isActiveAndEnabled)
        {
          List<PixelCollider> pixelColliders = rigidbody.PixelColliders;
          int count = pixelColliders.Count;
          Transform transform = rigidbody.transform;
          if (rigidbody.PhysicsRegistration == SpeculativeRigidbody.RegistrationState.Unknown)
            this.InferRegistration(rigidbody);
          if (rigidbody.RegenerateColliders)
            rigidbody.ForceRegenerate();
          if (rigidbody.UpdateCollidersOnScale && transform.hasChanged || rigidbody.UpdateCollidersOnRotation)
          {
            float rotation = 0.0f;
            if (rigidbody.UpdateCollidersOnRotation)
              rotation = transform.eulerAngles.z;
            Vector2 scale1;
            if (rigidbody.UpdateCollidersOnScale)
            {
              Vector3 localScale = transform.localScale;
              scale1 = new Vector2(rigidbody.AxialScale.x * localScale.x, rigidbody.AxialScale.y * localScale.y);
            }
            else
              scale1 = Vector2.one;
            tk2dBaseSprite sprite = rigidbody.sprite;
            if ((bool) (UnityEngine.Object) sprite)
            {
              Vector2 scale2 = (Vector2) sprite.scale;
              scale1 = new Vector2(scale1.x * Mathf.Abs(scale2.x), scale1.y * Mathf.Abs(scale2.y));
            }
            if (rigidbody.UpdateCollidersOnRotation && (double) rotation != (double) rigidbody.LastRotation || rigidbody.UpdateCollidersOnScale && scale1 != rigidbody.LastScale)
            {
              rigidbody.LastRotation = rotation;
              rigidbody.LastScale = scale1;
              for (int index2 = 0; index2 < count; ++index2)
                pixelColliders[index2].SetRotationAndScale(rotation, scale1);
              rigidbody.UpdateColliderPositions();
            }
            transform.hasChanged = false;
          }
          List<SpeculativeRigidbody.TemporaryException> collisionExceptions = rigidbody.m_temporaryCollisionExceptions;
          if (collisionExceptions != null)
          {
            for (int index3 = collisionExceptions.Count - 1; index3 >= 0; --index3)
            {
              SpeculativeRigidbody.TemporaryException temporaryException = collisionExceptions[index3];
              if (temporaryException.HasEnded(rigidbody))
                rigidbody.DeregisterTemporaryCollisionException(collisionExceptions[index3].SpecRigidbody);
              else
                collisionExceptions[index3] = temporaryException;
            }
          }
          bool flag = false;
          for (int index4 = 0; index4 < count; ++index4)
          {
            PixelCollider pixelCollider = pixelColliders[index4];
            if (pixelCollider.ColliderGenerationMode == PixelCollider.PixelColliderGeneration.BagelCollider && !pixelCollider.BagleUseFirstFrameOnly && pixelCollider.m_lastSpriteDef != pixelCollider.Sprite.GetTrueCurrentSpriteDef())
            {
              pixelCollider.RegenerateFromBagelCollider(pixelCollider.Sprite, transform, pixelCollider.m_rotation);
              flag = true;
            }
          }
          if (flag)
            PhysicsEngine.UpdatePosition(rigidbody);
          if (rigidbody.HasTriggerCollisions)
            rigidbody.ResetTriggerCollisionData();
          if (rigidbody.HasFrameSpecificCollisionExceptions)
            rigidbody.ClearFrameSpecificCollisionExceptions();
          if (rigidbody.OnPreMovement != null)
            rigidbody.OnPreMovement(rigidbody);
        }
      }
      if ((UnityEngine.Object) this.m_nbt.tileMap == (UnityEngine.Object) null)
      {
        this.m_nbt.tileMap = this.TileMap;
        this.m_nbt.layerName = "Collision Layer";
        this.m_nbt.layer = BraveUtility.GetTileMapLayerByName("Collision Layer", this.TileMap);
      }
      float deltaTime = BraveTime.DeltaTime;
      for (int index5 = 0; index5 < this.m_rigidbodies.Count; ++index5)
      {
        SpeculativeRigidbody rigidbody = this.m_rigidbodies[index5];
        if ((bool) (UnityEngine.Object) rigidbody && rigidbody.isActiveAndEnabled)
        {
          Position position1 = rigidbody.m_position;
          Vector2 vector2_1 = new Vector2((float) position1.m_position.x * (1f / 16f) + position1.m_remainder.x, (float) position1.m_position.y * (1f / 16f) + position1.m_remainder.y);
          IntVector2 position2 = position1.m_position;
          if (rigidbody.CapVelocity)
          {
            Vector2 maxVelocity = rigidbody.MaxVelocity;
            if ((double) Mathf.Abs(rigidbody.Velocity.x) > (double) maxVelocity.x)
              rigidbody.Velocity.x = Mathf.Sign(rigidbody.Velocity.x) * maxVelocity.x;
            if ((double) Mathf.Abs(rigidbody.Velocity.y) > (double) maxVelocity.y)
              rigidbody.Velocity.y = Mathf.Sign(rigidbody.Velocity.y) * maxVelocity.y;
          }
          rigidbody.Velocity.x = !float.IsNaN(rigidbody.Velocity.x) ? Mathf.Clamp(rigidbody.Velocity.x, -1000f, 1000f) : 0.0f;
          rigidbody.Velocity.y = !float.IsNaN(rigidbody.Velocity.y) ? Mathf.Clamp(rigidbody.Velocity.y, -1000f, 1000f) : 0.0f;
          rigidbody.TimeRemaining = deltaTime;
          if (rigidbody.Velocity == Vector2.zero && rigidbody.ImpartedPixelsToMove == IntVector2.Zero && !rigidbody.ForceAlwaysUpdate && (!rigidbody.PathMode || (double) rigidbody.PathSpeed == 0.0))
            rigidbody.TimeRemaining = 0.0f;
          Vector2? nullable1 = new Vector2?();
          Vector2? nullable2 = nullable1;
          List<SpeculativeRigidbody.PushedRigidbodyData> pushedRigidbodies1 = rigidbody.m_pushedRigidbodies;
          int count = pushedRigidbodies1.Count;
          for (int index6 = 0; index6 < count; ++index6)
          {
            SpeculativeRigidbody.PushedRigidbodyData pushedRigidbodyData = pushedRigidbodies1[index6] with
            {
              PushedThisFrame = false
            };
            pushedRigidbodies1[index6] = pushedRigidbodyData;
          }
          if (rigidbody.PathMode)
            rigidbody.Velocity = (PhysicsEngine.PixelToUnit(rigidbody.PathTarget) - rigidbody.Position.UnitPosition).normalized * rigidbody.PathSpeed;
          int num1 = 0;
          while ((double) rigidbody.TimeRemaining > 0.0 && num1 < 50)
          {
            if (nullable2.HasValue)
            {
              rigidbody.Velocity = nullable2.Value;
              nullable1 = new Vector2?();
              nullable2 = nullable1;
            }
            float timeRemaining = rigidbody.TimeRemaining;
            float timeUsed = timeRemaining;
            bool flag1 = false;
            IntVector2 impartedPixelsToMove = rigidbody.ImpartedPixelsToMove;
            if (impartedPixelsToMove.x != 0 || impartedPixelsToMove.y != 0)
            {
              flag1 = true;
              timeUsed = 0.0f;
              rigidbody.PixelsToMove = impartedPixelsToMove;
            }
            else
            {
              Vector2 velocity = rigidbody.Velocity;
              Vector2 vector2_2 = new Vector2(velocity.x * timeRemaining, velocity.y * timeRemaining);
              IntVector2 position3 = rigidbody.m_position.m_position;
              Vector2 remainder = rigidbody.m_position.m_remainder;
              rigidbody.PixelsToMove = new IntVector2(Mathf.RoundToInt((float) (((double) position3.x * (1.0 / 16.0) + (double) remainder.x + (double) vector2_2.x) * 16.0)) - position3.x, Mathf.RoundToInt((float) (((double) position3.y * (1.0 / 16.0) + (double) remainder.y + (double) vector2_2.y) * 16.0)) - position3.y);
            }
            rigidbody.CollidedX = false;
            rigidbody.CollidedY = false;
            CollisionData nearestCollision = (CollisionData) null;
            bool flag2 = true;
            bool flag3 = true;
            List<PixelCollider.StepData> stepList = PixelCollider.m_stepList;
            if (flag1)
              PhysicsEngine.PixelMovementGenerator(rigidbody.PixelsToMove, stepList);
            else
              PhysicsEngine.PixelMovementGenerator(rigidbody.m_position.m_remainder, rigidbody.Velocity, rigidbody.PixelsToMove, stepList);
            if (rigidbody.PathMode)
            {
              float num2 = Vector2.Distance(PhysicsEngine.PixelToUnit(rigidbody.PathTarget), rigidbody.m_position.UnitPosition) / rigidbody.PathSpeed;
              if ((double) num2 <= (double) timeRemaining && (nearestCollision == null || (double) num2 < (double) nearestCollision.TimeUsed))
              {
                CollisionData.Pool.Free(ref nearestCollision);
                if (nearestCollision == null)
                  nearestCollision = CollisionData.Pool.Allocate();
                nearestCollision.collisionType = CollisionData.CollisionType.PathEnd;
                nearestCollision.NewPixelsToMove = rigidbody.PathTarget - rigidbody.m_position.PixelPosition;
                nearestCollision.CollidedX = (double) rigidbody.Velocity.x != 0.0;
                nearestCollision.CollidedY = (double) rigidbody.Velocity.y != 0.0;
                nearestCollision.MyRigidbody = rigidbody;
                nearestCollision.MyPixelCollider = rigidbody.PrimaryPixelCollider;
                nearestCollision.TimeUsed = num2;
              }
            }
            if (rigidbody.MovementRestrictor != null)
            {
              IntVector2 prevPixelOffset = IntVector2.Zero;
              IntVector2 zero = IntVector2.Zero;
              for (int index7 = 0; index7 < stepList.Count; ++index7)
              {
                bool validLocation = true;
                zero += stepList[index7].deltaPos;
                rigidbody.MovementRestrictor(rigidbody, prevPixelOffset, zero, ref validLocation);
                if (!validLocation)
                {
                  float num3 = 0.0f;
                  for (int index8 = 0; index8 <= index7; ++index8)
                    num3 += stepList[index8].deltaTime;
                  if (nearestCollision == null || (double) num3 < (double) nearestCollision.TimeUsed)
                  {
                    CollisionData.Pool.Free(ref nearestCollision);
                    if (nearestCollision == null)
                      nearestCollision = CollisionData.Pool.Allocate();
                    nearestCollision.collisionType = CollisionData.CollisionType.MovementRestriction;
                    nearestCollision.NewPixelsToMove = zero - stepList[index7].deltaPos;
                    nearestCollision.CollidedX = stepList[index7].deltaPos.x != 0;
                    nearestCollision.CollidedY = stepList[index7].deltaPos.y != 0;
                    nearestCollision.MyRigidbody = rigidbody;
                    nearestCollision.MyPixelCollider = rigidbody.PrimaryPixelCollider;
                    nearestCollision.TimeUsed = num3;
                    break;
                  }
                  break;
                }
                prevPixelOffset = zero;
              }
            }
            if (rigidbody.CanPush && rigidbody.PushedRigidbodies.Count > 0)
            {
              for (int index9 = 0; index9 < rigidbody.PushedRigidbodies.Count; ++index9)
              {
                SpeculativeRigidbody.PushedRigidbodyData pushedRigidbody = rigidbody.PushedRigidbodies[index9];
                if (pushedRigidbody.PushedThisFrame)
                {
                  IntVector2 pushedPixelsToMove = pushedRigidbody.GetPushedPixelsToMove(rigidbody.PixelsToMove);
                  CollisionData result;
                  if (this.RigidbodyCast(pushedRigidbody.SpecRigidbody, pushedPixelsToMove, out result))
                  {
                    result.collisionType = CollisionData.CollisionType.Pushable;
                    result.CollidedX = pushedRigidbody.CollidedX;
                    result.CollidedY = pushedRigidbody.CollidedY;
                    result.TimeUsed = 0.0f;
                    result.NewPixelsToMove = IntVector2.Zero;
                    for (int index10 = 0; index10 < stepList.Count && !(pushedRigidbody.GetPushedPixelsToMove(result.NewPixelsToMove) == result.NewPixelsToMove); ++index10)
                    {
                      result.NewPixelsToMove += stepList[index10].deltaPos;
                      result.TimeUsed += stepList[index10].deltaTime;
                    }
                    result.IsPushCollision = true;
                    if (nearestCollision == null || (double) result.TimeUsed < (double) nearestCollision.TimeUsed)
                      nearestCollision = result;
                  }
                }
              }
              if (flag1)
                PhysicsEngine.PixelMovementGenerator(rigidbody.PixelsToMove, stepList);
              else
                PhysicsEngine.PixelMovementGenerator(rigidbody, stepList);
            }
            if (rigidbody.CollideWithOthers)
              this.CollideWithRigidbodies(rigidbody, stepList, ref nearestCollision);
            if (rigidbody.CollideWithTileMap && (UnityEngine.Object) dungeon != (UnityEngine.Object) null && (UnityEngine.Object) this.TileMap != (UnityEngine.Object) null)
            {
              List<PixelCollider> pixelColliders = rigidbody.PixelColliders;
              for (int index11 = 0; index11 < pixelColliders.Count; ++index11)
              {
                PixelCollider pixelCollider = pixelColliders[index11];
                if (pixelCollider.Enabled && (pixelCollider.CollisionLayer == CollisionLayer.TileBlocker || (CollisionLayerMatrix.GetMask(pixelCollider.CollisionLayer) & 64 /*0x40*/) == 64 /*0x40*/))
                  this.CollideWithTilemap(rigidbody, pixelCollider, stepList, ref timeUsed, data, ref nearestCollision);
              }
            }
            if (rigidbody.CanPush && rigidbody.PushedRigidbodies.Count > 0)
            {
              IntVector2 pixelsToMove = nearestCollision == null ? rigidbody.PixelsToMove : nearestCollision.NewPixelsToMove;
              for (int index12 = 0; index12 < rigidbody.PushedRigidbodies.Count; ++index12)
              {
                SpeculativeRigidbody.PushedRigidbodyData pushedRigidbody = rigidbody.PushedRigidbodies[index12];
                if (pushedRigidbody.PushedThisFrame)
                {
                  SpeculativeRigidbody specRigidbody = pushedRigidbody.SpecRigidbody;
                  IntVector2 pushedPixelsToMove = pushedRigidbody.GetPushedPixelsToMove(pixelsToMove);
                  Position position4 = specRigidbody.Position;
                  position4.PixelPosition += pushedPixelsToMove;
                  specRigidbody.Position = position4;
                  specRigidbody.transform.position = specRigidbody.Position.GetPixelVector2().ToVector3ZUp(specRigidbody.transform.position.z);
                  if (specRigidbody.OnPostRigidbodyMovement != null)
                    specRigidbody.OnPostRigidbodyMovement(specRigidbody, PhysicsEngine.PixelToUnit(pushedPixelsToMove), pushedPixelsToMove);
                }
              }
              if (nearestCollision != null && nearestCollision.IsPushCollision && (bool) (UnityEngine.Object) nearestCollision.OtherRigidbody)
              {
                MinorBreakable minorBreakable = nearestCollision.OtherRigidbody.minorBreakable;
                if ((bool) (UnityEngine.Object) minorBreakable && !minorBreakable.isInvulnerableToGameActors)
                  minorBreakable.Break(-nearestCollision.Normal);
              }
            }
            if (nearestCollision != null)
            {
              if (!nearestCollision.Overlap && rigidbody.CanPush && (UnityEngine.Object) nearestCollision.OtherRigidbody != (UnityEngine.Object) null && nearestCollision.OtherRigidbody.CanBePushed)
              {
                int index13 = -1;
                for (int index14 = 0; index14 < rigidbody.PushedRigidbodies.Count; ++index14)
                {
                  if ((UnityEngine.Object) rigidbody.PushedRigidbodies[index14].SpecRigidbody == (UnityEngine.Object) nearestCollision.OtherRigidbody)
                  {
                    index13 = index14;
                    break;
                  }
                }
                if (index13 < 0)
                {
                  index13 = rigidbody.PushedRigidbodies.Count;
                  rigidbody.PushedRigidbodies.Add(new SpeculativeRigidbody.PushedRigidbodyData(nearestCollision.OtherRigidbody));
                }
                else
                  nearestCollision.TimeUsed = 0.0f;
                SpeculativeRigidbody.PushedRigidbodyData pushedRigidbody = rigidbody.PushedRigidbodies[index13] with
                {
                  Direction = !nearestCollision.CollidedX ? IntVector2.Up : IntVector2.Right,
                  PushedThisFrame = true
                };
                rigidbody.PushedRigidbodies[index13] = pushedRigidbody;
                nearestCollision.MyPixelCollider.RegisterFrameSpecificCollisionException(nearestCollision.MyRigidbody, nearestCollision.OtherPixelCollider);
                flag2 = false;
                flag3 = false;
                nullable2 = new Vector2?(rigidbody.Velocity);
                if (nearestCollision.CollidedX)
                  nullable2 = new Vector2?(nullable2.Value.WithX(nullable2.Value.x * rigidbody.PushSpeedModifier));
                if (nearestCollision.CollidedY)
                  nullable2 = new Vector2?(nullable2.Value.WithY(nullable2.Value.y * rigidbody.PushSpeedModifier));
              }
              bool? nullable3 = new bool?();
              PhysicsEngine.CollisionHaltsVelocity = nullable3;
              PhysicsEngine.HaltRemainingMovement = false;
              nullable1 = new Vector2?();
              PhysicsEngine.PostSliceVelocity = nullable1;
              CollisionData collisionData = (CollisionData) null;
              if (!nearestCollision.IsTriggerCollision)
              {
                if (rigidbody.OnCollision != null)
                  rigidbody.OnCollision(nearestCollision);
                if ((UnityEngine.Object) nearestCollision.OtherRigidbody != (UnityEngine.Object) null && nearestCollision.OtherRigidbody.OnCollision != null)
                {
                  if (collisionData == null)
                    collisionData = nearestCollision.GetInverse();
                  nearestCollision.OtherRigidbody.OnCollision(nearestCollision.GetInverse());
                }
              }
              if ((UnityEngine.Object) nearestCollision.OtherRigidbody != (UnityEngine.Object) null)
              {
                if (!nearestCollision.IsTriggerCollision)
                {
                  if (rigidbody.OnRigidbodyCollision != null)
                    rigidbody.OnRigidbodyCollision(nearestCollision);
                  if (nearestCollision.OtherRigidbody.OnRigidbodyCollision != null)
                  {
                    if (collisionData == null)
                      collisionData = nearestCollision.GetInverse();
                    nearestCollision.OtherRigidbody.OnRigidbodyCollision(nearestCollision.GetInverse());
                  }
                }
              }
              else if (nearestCollision.TileLayerName != null && rigidbody.OnTileCollision != null)
                rigidbody.OnTileCollision(nearestCollision);
              nullable3 = PhysicsEngine.CollisionHaltsVelocity;
              if (nullable3.HasValue)
                flag2 = PhysicsEngine.CollisionHaltsVelocity.Value;
              if ((UnityEngine.Object) nearestCollision.OtherRigidbody != (UnityEngine.Object) null && nearestCollision.IsTriggerCollision)
              {
                SpeculativeRigidbody otherRigidbody = nearestCollision.OtherRigidbody;
                rigidbody.PixelsToMove = nearestCollision.NewPixelsToMove;
                rigidbody.CollidedX = nearestCollision.CollidedX;
                rigidbody.CollidedY = nearestCollision.CollidedY;
                timeUsed = nearestCollision.TimeUsed;
                flag2 = false;
                flag3 = false;
                nearestCollision.MyPixelCollider.RegisterFrameSpecificCollisionException(rigidbody, nearestCollision.OtherPixelCollider);
                TriggerCollisionData triggerCollisionData1 = nearestCollision.MyPixelCollider.RegisterTriggerCollision(nearestCollision.MyRigidbody, nearestCollision.OtherRigidbody, nearestCollision.OtherPixelCollider);
                TriggerCollisionData triggerCollisionData2 = nearestCollision.OtherPixelCollider.RegisterTriggerCollision(nearestCollision.MyRigidbody, nearestCollision.MyRigidbody, nearestCollision.MyPixelCollider);
                if (triggerCollisionData1.FirstFrame)
                {
                  if (rigidbody.OnEnterTrigger != null)
                    rigidbody.OnEnterTrigger(otherRigidbody, rigidbody, nearestCollision);
                  if (otherRigidbody.OnEnterTrigger != null)
                  {
                    if (collisionData == null)
                      collisionData = nearestCollision.GetInverse();
                    otherRigidbody.OnEnterTrigger(rigidbody, otherRigidbody, nearestCollision.GetInverse());
                  }
                }
                if (triggerCollisionData1.FirstFrame || triggerCollisionData1.ContinuedCollision)
                {
                  if (rigidbody.OnTriggerCollision != null)
                    rigidbody.OnTriggerCollision(otherRigidbody, rigidbody, nearestCollision);
                  if (otherRigidbody.OnTriggerCollision != null)
                  {
                    if (collisionData == null)
                      collisionData = nearestCollision.GetInverse();
                    otherRigidbody.OnTriggerCollision(rigidbody, otherRigidbody, nearestCollision.GetInverse());
                  }
                }
                triggerCollisionData1.Notified = true;
                triggerCollisionData2.Notified = true;
              }
              else if ((bool) (UnityEngine.Object) nearestCollision.OtherRigidbody && (rigidbody.IsGhostCollisionException(nearestCollision.OtherRigidbody) || nearestCollision.OtherRigidbody.IsGhostCollisionException(rigidbody)))
              {
                if (!nearestCollision.Overlap)
                {
                  rigidbody.PixelsToMove = nearestCollision.NewPixelsToMove;
                  timeUsed = nearestCollision.TimeUsed;
                }
                else
                {
                  rigidbody.PixelsToMove = IntVector2.Zero;
                  timeUsed = 0.0f;
                }
                nearestCollision.MyPixelCollider.RegisterFrameSpecificCollisionException(rigidbody, nearestCollision.OtherPixelCollider);
              }
              else
              {
                rigidbody.CollidedX = nearestCollision.CollidedX;
                rigidbody.CollidedY = nearestCollision.CollidedY;
                rigidbody.PixelsToMove = nearestCollision.NewPixelsToMove;
                timeUsed = nearestCollision.TimeUsed;
              }
              if (collisionData != null)
                CollisionData.Pool.Free(ref collisionData);
              if (!flag1 && nearestCollision.collisionType != CollisionData.CollisionType.PathEnd)
              {
                float num4 = PhysicsEngine.PixelToUnit(1) / 2f;
                if (rigidbody.CollidedX && !rigidbody.CollidedY)
                  timeUsed = Mathf.Max(0.0f, timeUsed - Mathf.Abs(num4 / rigidbody.Velocity.x));
                else if (rigidbody.CollidedY && !rigidbody.CollidedX)
                  timeUsed = Mathf.Max(0.0f, timeUsed - Mathf.Abs(num4 / rigidbody.Velocity.y));
              }
            }
            if (flag1)
            {
              timeUsed = 0.0f;
              rigidbody.Position = new Position(rigidbody.Position.PixelPosition + rigidbody.PixelsToMove, rigidbody.Position.Remainder);
              rigidbody.ImpartedPixelsToMove -= rigidbody.PixelsToMove;
              if (nearestCollision == null || !nearestCollision.IsTriggerCollision)
              {
                if (rigidbody.CollidedX)
                  rigidbody.ImpartedPixelsToMove = rigidbody.ImpartedPixelsToMove.WithX(0);
                if (rigidbody.CollidedY)
                  rigidbody.ImpartedPixelsToMove = rigidbody.ImpartedPixelsToMove.WithY(0);
              }
            }
            else
            {
              Position position5 = rigidbody.Position;
              if (rigidbody.CollidedX && flag3)
                position5.X += rigidbody.PixelsToMove.x;
              else
                position5.UnitX += rigidbody.Velocity.x * timeUsed;
              if (rigidbody.CollidedY && flag3)
                position5.Y += rigidbody.PixelsToMove.y;
              else
                position5.UnitY += rigidbody.Velocity.y * timeUsed;
              if (flag2)
              {
                if (rigidbody.CollidedX)
                  rigidbody.Velocity.x = 0.0f;
                if (rigidbody.CollidedY)
                  rigidbody.Velocity.y = 0.0f;
              }
              nullable1 = PhysicsEngine.PostSliceVelocity;
              if (nullable1.HasValue)
              {
                rigidbody.Velocity = PhysicsEngine.PostSliceVelocity.Value;
                PhysicsEngine.PostSliceVelocity = new Vector2?();
              }
              rigidbody.Position = position5;
            }
            if (rigidbody.CarriedRigidbodies != null)
            {
              for (int index15 = 0; index15 < rigidbody.CarriedRigidbodies.Count; ++index15)
              {
                SpeculativeRigidbody carriedRigidbody = rigidbody.CarriedRigidbodies[index15];
                if (carriedRigidbody.CanBeCarried || rigidbody.ForceCarriesRigidbodies)
                  carriedRigidbody.ImpartedPixelsToMove += rigidbody.PixelsToMove;
              }
            }
            if (rigidbody.IgnorePixelGrid)
            {
              IntVector2 position6 = rigidbody.m_position.m_position;
              Vector2 remainder = rigidbody.m_position.m_remainder;
              Transform transform = rigidbody.transform;
              transform.position = new Vector3((float) position6.x * (1f / 16f) + remainder.x, (float) position6.y * (1f / 16f) + remainder.y, transform.position.z);
            }
            else
            {
              IntVector2 position7 = rigidbody.Position.m_position;
              Transform transform = rigidbody.transform;
              transform.position = new Vector3((float) position7.x * (1f / 16f), (float) position7.y * (1f / 16f), transform.position.z);
            }
            rigidbody.TimeRemaining -= timeUsed;
            if (rigidbody.PathMode && nearestCollision != null && nearestCollision.collisionType == CollisionData.CollisionType.PathEnd)
            {
              if (rigidbody.OnPathTargetReached != null)
              {
                rigidbody.OnPathTargetReached();
                if (rigidbody.PathMode)
                  rigidbody.Velocity = (PhysicsEngine.PixelToUnit(rigidbody.PathTarget) - rigidbody.Position.UnitPosition).normalized * rigidbody.PathSpeed;
              }
              else
                rigidbody.PathMode = false;
            }
            if (PhysicsEngine.HaltRemainingMovement || rigidbody.Velocity == Vector2.zero && rigidbody.ImpartedPixelsToMove == IntVector2.Zero && !rigidbody.PathMode && !rigidbody.HasUnresolvedTriggerCollisions)
              rigidbody.TimeRemaining = 0.0f;
            ++num1;
            if (nearestCollision != null)
              CollisionData.Pool.Free(ref nearestCollision);
          }
          List<SpeculativeRigidbody.PushedRigidbodyData> pushedRigidbodies2 = rigidbody.m_pushedRigidbodies;
          for (int index16 = pushedRigidbodies2.Count - 1; index16 >= 0; --index16)
          {
            if (!pushedRigidbodies2[index16].PushedThisFrame)
              pushedRigidbodies2.RemoveAt(index16);
          }
          IntVector2 intVector2 = rigidbody.Position.m_position - position2;
          if (rigidbody.OnPostRigidbodyMovement != null)
            rigidbody.OnPostRigidbodyMovement(rigidbody, rigidbody.m_position.UnitPosition - vector2_1, intVector2);
          if ((UnityEngine.Object) rigidbody.TK2DSprite != (UnityEngine.Object) null && (rigidbody.TK2DSprite.IsZDepthDirty || intVector2.x != 0 || intVector2.y != 0))
            rigidbody.TK2DSprite.UpdateZDepth();
          rigidbody.RecheckTriggers = false;
        }
      }
      if (this.OnPostRigidbodyMovement != null)
        this.OnPostRigidbodyMovement();
      for (int index17 = 0; index17 < this.m_rigidbodies.Count; ++index17)
      {
        SpeculativeRigidbody rigidbody = this.m_rigidbodies[index17];
        if ((bool) (UnityEngine.Object) rigidbody && rigidbody.HasTriggerCollisions)
        {
          for (int index18 = 0; index18 < rigidbody.PixelColliders.Count; ++index18)
          {
            PixelCollider pixelCollider1 = rigidbody.PixelColliders[index18];
            for (int index19 = pixelCollider1.TriggerCollisions.Count - 1; index19 >= 0; --index19)
            {
              TriggerCollisionData triggerCollision = pixelCollider1.TriggerCollisions[index19];
              PixelCollider pixelCollider2 = triggerCollision.PixelCollider;
              SpeculativeRigidbody specRigidbody = triggerCollision.SpecRigidbody;
              if (!triggerCollision.Notified)
              {
                if (!triggerCollision.FirstFrame && !triggerCollision.ContinuedCollision)
                {
                  if (rigidbody.OnExitTrigger != null)
                    rigidbody.OnExitTrigger(specRigidbody, rigidbody);
                  if (specRigidbody.OnExitTrigger != null)
                    specRigidbody.OnExitTrigger(rigidbody, specRigidbody);
                }
                triggerCollision.Notified = true;
                for (int index20 = 0; index20 < pixelCollider2.TriggerCollisions.Count; ++index20)
                {
                  if (pixelCollider2.TriggerCollisions[index20].PixelCollider == pixelCollider1)
                  {
                    pixelCollider2.TriggerCollisions[index20].Notified = true;
                    break;
                  }
                }
              }
              if (!triggerCollision.FirstFrame && !triggerCollision.ContinuedCollision)
              {
                pixelCollider1.TriggerCollisions.RemoveAt(index19);
                for (int index21 = 0; index21 < pixelCollider2.TriggerCollisions.Count; ++index21)
                {
                  if (pixelCollider2.TriggerCollisions[index21].PixelCollider == pixelCollider1)
                  {
                    pixelCollider2.TriggerCollisions.RemoveAt(index21);
                    --index21;
                  }
                }
              }
            }
          }
        }
      }
      for (int index22 = 0; index22 < this.m_rigidbodies.Count; ++index22)
      {
        SpeculativeRigidbody rigidbody = this.m_rigidbodies[index22];
        if (rigidbody.isActiveAndEnabled)
        {
          List<SpeculativeRigidbody> collisionExceptions = rigidbody.GhostCollisionExceptions;
          if (collisionExceptions != null)
          {
            for (int index23 = 0; index23 < collisionExceptions.Count; ++index23)
            {
              SpeculativeRigidbody speculativeRigidbody = collisionExceptions[index23];
              bool flag = false;
              if ((bool) (UnityEngine.Object) speculativeRigidbody)
              {
                for (int index24 = 0; index24 < rigidbody.PixelColliders.Count && !flag; ++index24)
                {
                  PixelCollider pixelCollider3 = rigidbody.PixelColliders[index24];
                  for (int index25 = 0; index25 < speculativeRigidbody.PixelColliders.Count && !flag; ++index25)
                  {
                    PixelCollider pixelCollider4 = speculativeRigidbody.PixelColliders[index25];
                    if (pixelCollider3.CanCollideWith(pixelCollider4, true))
                      flag |= pixelCollider3.Overlaps(pixelCollider4);
                  }
                }
              }
              if (!flag)
              {
                rigidbody.DeregisterGhostCollisionException(index23);
                --index23;
              }
            }
          }
        }
      }
      if (this.DebugDraw != PhysicsEngine.DebugDrawType.None)
        this.m_debugTilesDrawnThisFrame.Clear();
    }
    for (int index = 0; index < this.m_deregisterRigidBodies.Count; ++index)
      this.Deregister(this.m_deregisterRigidBodies[index]);
    this.m_deregisterRigidBodies.Clear();
  }

  public void Query(Vector2 worldMin, Vector2 worldMax, Func<SpeculativeRigidbody, bool> callback)
  {
    this.m_rigidbodyTree.Query(PhysicsEngine.GetSafeB2AABB(PhysicsEngine.UnitToPixel(worldMin), PhysicsEngine.UnitToPixel(worldMax)), callback);
  }

  public bool Raycast(
    Vector2 unitOrigin,
    Vector2 direction,
    float dist,
    out RaycastResult result,
    bool collideWithTiles = true,
    bool collideWithRigidbodies = true,
    int rayMask = 2147483647 /*0x7FFFFFFF*/,
    CollisionLayer? sourceLayer = null,
    bool collideWithTriggers = false,
    Func<SpeculativeRigidbody, bool> rigidbodyExcluder = null,
    SpeculativeRigidbody ignoreRigidbody = null)
  {
    bool flag;
    if ((UnityEngine.Object) ignoreRigidbody == (UnityEngine.Object) null)
    {
      flag = this.RaycastWithIgnores(new Position(unitOrigin), direction, dist, out result, collideWithTiles, collideWithRigidbodies, rayMask, sourceLayer, collideWithTriggers, rigidbodyExcluder, (ICollection<SpeculativeRigidbody>) this.m_emptyIgnoreList);
    }
    else
    {
      this.m_singleIgnoreList[0] = ignoreRigidbody;
      flag = this.RaycastWithIgnores(new Position(unitOrigin), direction, dist, out result, collideWithTiles, collideWithRigidbodies, rayMask, sourceLayer, collideWithTriggers, rigidbodyExcluder, (ICollection<SpeculativeRigidbody>) this.m_singleIgnoreList);
      this.m_singleIgnoreList[0] = (SpeculativeRigidbody) null;
    }
    return flag;
  }

  public bool RaycastWithIgnores(
    Vector2 unitOrigin,
    Vector2 direction,
    float dist,
    out RaycastResult result,
    bool collideWithTiles = true,
    bool collideWithRigidbodies = true,
    int rayMask = 2147483647 /*0x7FFFFFFF*/,
    CollisionLayer? sourceLayer = null,
    bool collideWithTriggers = false,
    Func<SpeculativeRigidbody, bool> rigidbodyExcluder = null,
    ICollection<SpeculativeRigidbody> ignoreList = null)
  {
    return this.RaycastWithIgnores(new Position(unitOrigin), direction, dist, out result, collideWithTiles, collideWithRigidbodies, rayMask, sourceLayer, collideWithTriggers, rigidbodyExcluder, ignoreList);
  }

  public bool RaycastWithIgnores(
    Position origin,
    Vector2 direction,
    float dist,
    out RaycastResult result,
    bool collideWithTiles = true,
    bool collideWithRigidbodies = true,
    int rayMask = 2147483647 /*0x7FFFFFFF*/,
    CollisionLayer? sourceLayer = null,
    bool collideWithTriggers = false,
    Func<SpeculativeRigidbody, bool> rigidbodyExcluder = null,
    ICollection<SpeculativeRigidbody> ignoreList = null)
  {
    PhysicsEngine.m_raycaster.SetAll(this, GameManager.Instance.Dungeon.data, origin, direction, dist, collideWithTiles, collideWithRigidbodies, rayMask, sourceLayer, collideWithTriggers, rigidbodyExcluder, ignoreList);
    bool flag = PhysicsEngine.m_raycaster.DoRaycast(out result);
    PhysicsEngine.m_raycaster.Clear();
    return flag;
  }

  public bool Pointcast(
    Vector2 point,
    out SpeculativeRigidbody result,
    bool collideWithTiles = true,
    bool collideWithRigidbodies = true,
    int rayMask = 2147483647 /*0x7FFFFFFF*/,
    CollisionLayer? sourceLayer = null,
    bool collideWithTriggers = false,
    params SpeculativeRigidbody[] ignoreList)
  {
    return this.Pointcast(PhysicsEngine.UnitToPixel(point), out result, collideWithTiles, collideWithRigidbodies, rayMask, sourceLayer, collideWithTriggers, ignoreList);
  }

  public bool Pointcast(
    IntVector2 point,
    out SpeculativeRigidbody result,
    bool collideWithTiles = true,
    bool collideWithRigidbodies = true,
    int rayMask = 2147483647 /*0x7FFFFFFF*/,
    CollisionLayer? sourceLayer = null,
    bool collideWithTriggers = false,
    params SpeculativeRigidbody[] ignoreList)
  {
    ICollidableObject tempResult = (ICollidableObject) null;
    Func<ICollidableObject, IntVector2, ICollidableObject> collideWithCollidable = (Func<ICollidableObject, IntVector2, ICollidableObject>) ((collidable, p) =>
    {
      SpeculativeRigidbody speculativeRigidbody = collidable as SpeculativeRigidbody;
      if ((bool) (UnityEngine.Object) speculativeRigidbody && !speculativeRigidbody.enabled)
        return (ICollidableObject) null;
      for (int index = 0; index < collidable.GetPixelColliders().Count; ++index)
      {
        PixelCollider pixelCollider = collidable.GetPixelColliders()[index];
        if ((collideWithTriggers || !pixelCollider.IsTrigger) && pixelCollider.CanCollideWith(rayMask, sourceLayer) && pixelCollider.ContainsPixel(p))
          return collidable;
      }
      return (ICollidableObject) null;
    });
    if (collideWithTiles && (bool) (UnityEngine.Object) this.TileMap)
    {
      int x;
      int y;
      this.TileMap.GetTileAtPosition((Vector3) PhysicsEngine.PixelToUnit(point), out x, out y);
      int tileMapLayerByName = BraveUtility.GetTileMapLayerByName("Collision Layer", this.TileMap);
      PhysicsEngine.Tile tile = this.GetTile(x, y, this.TileMap, tileMapLayerByName, "Collision Layer", GameManager.Instance.Dungeon.data);
      if (tile != null)
      {
        tempResult = collideWithCollidable((ICollidableObject) tile, point);
        if (tempResult != null)
        {
          result = tempResult as SpeculativeRigidbody;
          return true;
        }
      }
    }
    if (collideWithRigidbodies)
    {
      Func<SpeculativeRigidbody, bool> callback = (Func<SpeculativeRigidbody, bool>) (rigidbody =>
      {
        tempResult = collideWithCollidable((ICollidableObject) rigidbody, point);
        return tempResult == null;
      });
      this.m_rigidbodyTree.Query(PhysicsEngine.GetSafeB2AABB(point, point), callback);
      if (this.CollidesWithProjectiles(rayMask, sourceLayer))
        this.m_projectileTree.Query(PhysicsEngine.GetSafeB2AABB(point, point), callback);
    }
    result = tempResult as SpeculativeRigidbody;
    return (UnityEngine.Object) result != (UnityEngine.Object) null;
  }

  private static b2AABB GetSafeB2AABB(IntVector2 lowerBounds, IntVector2 upperBounds)
  {
    return new b2AABB(PhysicsEngine.PixelToUnit(lowerBounds - IntVector2.One), PhysicsEngine.PixelToUnit(upperBounds + 2 * IntVector2.One));
  }

  public bool Pointcast(
    List<IntVector2> points,
    List<IntVector2> lastFramePoints,
    int pointsWidth,
    out List<PointcastResult> pointResults,
    bool collideWithTiles = true,
    bool collideWithRigidbodies = true,
    int rayMask = 2147483647 /*0x7FFFFFFF*/,
    CollisionLayer? sourceLayer = null,
    bool collideWithTriggers = false,
    Func<SpeculativeRigidbody, bool> rigidbodyExcluder = null,
    int ignoreTileBoneCount = 0,
    params SpeculativeRigidbody[] ignoreList)
  {
    int tileMapLayerByName = BraveUtility.GetTileMapLayerByName("Collision Layer", this.TileMap);
    pointResults = new List<PointcastResult>();
    PhysicsEngine.c_boundedRigidbodies.Clear();
    if (collideWithRigidbodies)
    {
      IntVector2 pointMin = IntVector2.MaxValue;
      IntVector2 pointMax = IntVector2.MinValue;
      for (int index = 0; index < points.Count; ++index)
      {
        pointMin = IntVector2.Min(pointMin, points[index]);
        pointMax = IntVector2.Max(pointMax, points[index]);
      }
      Func<SpeculativeRigidbody, bool> callback = (Func<SpeculativeRigidbody, bool>) (rigidbody =>
      {
        if (!(bool) (UnityEngine.Object) rigidbody || !rigidbody.enabled || !rigidbody.CollideWithOthers || rigidbodyExcluder != null && rigidbodyExcluder(rigidbody) || Array.IndexOf<SpeculativeRigidbody>(ignoreList, rigidbody) >= 0)
          return true;
        for (int index = 0; index < rigidbody.PixelColliders.Count; ++index)
        {
          PixelCollider pixelCollider = rigidbody.PixelColliders[index];
          if ((collideWithTriggers || !pixelCollider.IsTrigger) && pixelCollider.CanCollideWith(rayMask, sourceLayer) && pixelCollider.AABBOverlaps(pointMin, pointMax - pointMin + IntVector2.One))
          {
            PhysicsEngine.c_boundedRigidbodies.Add(rigidbody);
            break;
          }
        }
        return true;
      });
      this.m_rigidbodyTree.Query(PhysicsEngine.GetSafeB2AABB(pointMin, pointMax), callback);
      if (this.CollidesWithProjectiles(rayMask, sourceLayer))
        this.m_projectileTree.Query(PhysicsEngine.GetSafeB2AABB(pointMin, pointMax), callback);
    }
    DungeonData data = GameManager.Instance.Dungeon.data;
    HitDirection[] hitDirectionArray = new HitDirection[pointsWidth];
    for (int index = 0; index < pointsWidth; ++index)
      hitDirectionArray[index] = HitDirection.Forward;
    for (int index1 = 0; index1 < points.Count - pointsWidth; ++index1)
    {
      Vector2 a = PhysicsEngine.PixelToUnit(points[index1]) + new Vector2(this.HalfPixelUnitWidth, this.HalfPixelUnitWidth);
      Vector2 b = PhysicsEngine.PixelToUnit(points[index1 + pointsWidth]) + new Vector2(this.HalfPixelUnitWidth, this.HalfPixelUnitWidth);
      int index2 = index1 % pointsWidth;
      for (int index3 = 0; (index1 < points.Count - 2 * pointsWidth ? (index3 < 2 ? 1 : 0) : (index3 <= 2 ? 1 : 0)) != 0; ++index3)
      {
        bool flag = false;
        IntVector2 pixel = PhysicsEngine.UnitToPixel(Vector2.Lerp(a, b, (float) index3 / 2f));
        if (collideWithTiles && (bool) (UnityEngine.Object) this.TileMap && index1 >= ignoreTileBoneCount)
        {
          int x;
          int y;
          this.TileMap.GetTileAtPosition((Vector3) PhysicsEngine.PixelToUnit(pixel), out x, out y);
          PhysicsEngine.Tile tile = this.GetTile(x, y, this.TileMap, tileMapLayerByName, "Collision Layer", data);
          if (tile != null && this.Pointcast_CoarsePass((ICollidableObject) tile, pixel, collideWithTriggers, rayMask, sourceLayer))
            flag = true;
        }
        if (collideWithRigidbodies && !flag)
        {
          for (int index4 = 0; index4 < PhysicsEngine.c_boundedRigidbodies.Count; ++index4)
          {
            if (this.Pointcast_CoarsePass((ICollidableObject) PhysicsEngine.c_boundedRigidbodies[index4], pixel, collideWithTriggers, rayMask, sourceLayer))
            {
              flag = true;
              break;
            }
          }
        }
        if (!flag && hitDirectionArray[index2] == HitDirection.Backward)
        {
          Vector2 unitOrigin = PhysicsEngine.PixelToUnit(PhysicsEngine.UnitToPixel(Vector2.Lerp(PhysicsEngine.PixelToUnit(lastFramePoints[index1]) + new Vector2(this.HalfPixelUnitWidth, this.HalfPixelUnitWidth), PhysicsEngine.PixelToUnit(lastFramePoints[index1 + pointsWidth]) + new Vector2(this.HalfPixelUnitWidth, this.HalfPixelUnitWidth), (float) index3 / 2f))) + new Vector2(this.HalfPixelUnitWidth, this.HalfPixelUnitWidth);
          Vector2 vector2 = PhysicsEngine.PixelToUnit(pixel) + new Vector2(this.HalfPixelUnitWidth, this.HalfPixelUnitWidth);
          Vector2 normalized = (vector2 - unitOrigin).normalized;
          float dist = (vector2 - unitOrigin).magnitude + 1.41421354f * this.PixelUnitWidth;
          RaycastResult result;
          flag = this.RaycastWithIgnores(unitOrigin, normalized, dist, out result, collideWithTiles, collideWithRigidbodies, rayMask, sourceLayer, collideWithTriggers, ignoreList: (ICollection<SpeculativeRigidbody>) ignoreList);
          RaycastResult.Pool.Free(ref result);
        }
        if (flag && hitDirectionArray[index2] == HitDirection.Forward)
        {
          int num;
          int index5;
          Vector2 unitOrigin;
          Vector2 vector2;
          if (index1 < pointsWidth && index3 == 0)
          {
            num = 0;
            index5 = 0;
            unitOrigin = PhysicsEngine.PixelToUnit(lastFramePoints[0]) + new Vector2(this.HalfPixelUnitWidth, this.HalfPixelUnitWidth);
            vector2 = PhysicsEngine.PixelToUnit(points[0]) + new Vector2(this.HalfPixelUnitWidth, this.HalfPixelUnitWidth);
          }
          else
          {
            num = index3 != 0 ? index1 : index1 - pointsWidth;
            index5 = num + pointsWidth;
            unitOrigin = PhysicsEngine.PixelToUnit(points[num]) + new Vector2(this.HalfPixelUnitWidth, this.HalfPixelUnitWidth);
            vector2 = PhysicsEngine.PixelToUnit(points[index5]) + new Vector2(this.HalfPixelUnitWidth, this.HalfPixelUnitWidth);
          }
          Vector2 normalized = (vector2 - unitOrigin).normalized;
          float dist = (vector2 - unitOrigin).magnitude + 1.41421354f * this.PixelUnitWidth;
          RaycastResult result;
          if (this.RaycastWithIgnores(unitOrigin, normalized, dist, out result, collideWithTiles && index5 >= ignoreTileBoneCount, collideWithRigidbodies, rayMask, sourceLayer, collideWithTriggers, ignoreList: (ICollection<SpeculativeRigidbody>) ignoreList))
          {
            PointcastResult pointcastResult = PointcastResult.Pool.Allocate();
            pointcastResult.SetAll(HitDirection.Forward, num, num / pointsWidth, result);
            pointResults.Add(pointcastResult);
            hitDirectionArray[index2] = HitDirection.Backward;
          }
        }
        else if (!flag && hitDirectionArray[index2] == HitDirection.Backward)
        {
          int index6 = index3 != 0 ? index1 : index1 - pointsWidth;
          int num1 = index6 + pointsWidth;
          Vector2 vector2 = PhysicsEngine.PixelToUnit(points[index6]) + new Vector2(this.HalfPixelUnitWidth, this.HalfPixelUnitWidth);
          Vector2 unitOrigin = PhysicsEngine.PixelToUnit(points[num1]) + new Vector2(this.HalfPixelUnitWidth, this.HalfPixelUnitWidth);
          Vector2 normalized = (vector2 - unitOrigin).normalized;
          float num2 = ((vector2 - unitOrigin).magnitude + 1.41421354f * this.PixelUnitWidth) * 3f;
          RaycastResult result;
          if (this.RaycastWithIgnores(unitOrigin, normalized, num2 * 3f, out result, collideWithTiles && index6 >= ignoreTileBoneCount, collideWithRigidbodies, rayMask, sourceLayer, collideWithTriggers, ignoreList: (ICollection<SpeculativeRigidbody>) ignoreList))
          {
            PointcastResult pointcastResult = PointcastResult.Pool.Allocate();
            pointcastResult.SetAll(HitDirection.Backward, num1, num1 / pointsWidth, result);
            pointResults.Add(pointcastResult);
            hitDirectionArray[index2] = HitDirection.Forward;
          }
        }
      }
    }
    if (pointsWidth > 1)
    {
      pointResults.Sort();
      List<PointcastResult> pointcastResultList = new List<PointcastResult>();
      int index7 = 0;
      int num3 = 0;
      int index8;
      for (; index7 < pointResults.Count; index7 = index8)
      {
        int num4 = num3;
        for (index8 = index7; index8 < pointResults.Count && pointResults[index7].boneIndex == pointResults[index8].boneIndex; ++index8)
        {
          if (pointResults[index8].hitDirection == HitDirection.Forward)
            ++num3;
          else if (pointResults[index8].hitDirection == HitDirection.Backward)
            --num3;
        }
        if (index7 == 0 && num3 > 0)
          pointcastResultList.Add(pointResults[index7]);
        else if (num4 == 0 && num3 > 0)
          pointcastResultList.Add(pointResults[index7]);
        else if (num4 >= 0 && num3 == 0)
          pointcastResultList.Add(pointResults[index7]);
      }
      pointResults = pointcastResultList;
    }
    return pointResults.Count > 0;
  }

  private bool Pointcast_CoarsePass(
    ICollidableObject collidable,
    IntVector2 point,
    bool collideWithTriggers,
    int rayMask,
    CollisionLayer? sourceLayer)
  {
    for (int index = 0; index < collidable.GetPixelColliders().Count; ++index)
    {
      PixelCollider pixelCollider = collidable.GetPixelColliders()[index];
      if ((collideWithTriggers || !pixelCollider.IsTrigger) && pixelCollider.CanCollideWith(rayMask, sourceLayer) && pixelCollider.ContainsPixel(point))
        return true;
    }
    return false;
  }

  public bool RigidbodyCast(
    SpeculativeRigidbody rigidbody,
    IntVector2 pixelsToMove,
    out CollisionData result,
    bool collideWithTiles = true,
    bool collideWithRigidbodies = true,
    int? overrideCollisionMask = null,
    bool collideWithTriggers = false)
  {
    return this.RigidbodyCastWithIgnores(rigidbody, pixelsToMove, out result, collideWithTiles, collideWithRigidbodies, overrideCollisionMask, collideWithTriggers, this.emptyIgnoreList);
  }

  public bool RigidbodyCastWithIgnores(
    SpeculativeRigidbody rigidbody,
    IntVector2 pixelsToMove,
    out CollisionData result,
    bool collideWithTiles = true,
    bool collideWithRigidbodies = true,
    int? overrideCollisionMask = null,
    bool collideWithTriggers = false,
    params SpeculativeRigidbody[] ignoreList)
  {
    PhysicsEngine.m_rigidbodyCaster.SetAll(this, GameManager.Instance.Dungeon.data, rigidbody, pixelsToMove, collideWithTiles, collideWithRigidbodies, overrideCollisionMask, collideWithTriggers, ignoreList);
    bool flag = PhysicsEngine.m_rigidbodyCaster.DoRigidbodyCast(out result);
    PhysicsEngine.m_rigidbodyCaster.Clear();
    return flag;
  }

  public bool OverlapCast(
    SpeculativeRigidbody rigidbody,
    List<CollisionData> overlappingCollisions = null,
    bool collideWithTiles = true,
    bool collideWithRigidbodies = true,
    int? overrideCollisionMask = null,
    int? ignoreCollisionMask = null,
    bool collideWithTriggers = false,
    Vector2? overridePosition = null,
    Func<SpeculativeRigidbody, bool> rigidbodyExcluder = null,
    params SpeculativeRigidbody[] ignoreList)
  {
    List<CollisionData> tempOverlappingCollisions = new List<CollisionData>();
    if (!(bool) (UnityEngine.Object) rigidbody || rigidbody.PixelColliders.Count == 0)
    {
      overlappingCollisions?.Clear();
      return false;
    }
    IntVector2 intVector2_1 = IntVector2.Zero;
    if (overridePosition.HasValue)
    {
      intVector2_1 = new Position(overridePosition.Value).PixelPosition - rigidbody.Position.PixelPosition;
      for (int index = 0; index < rigidbody.PixelColliders.Count; ++index)
        rigidbody.PixelColliders[index].Position += intVector2_1;
    }
    IntVector2 intVector2_2 = IntVector2.MaxValue;
    IntVector2 intVector2_3 = IntVector2.MinValue;
    for (int index = 0; index < rigidbody.PixelColliders.Count; ++index)
    {
      PixelCollider pixelCollider = rigidbody.PixelColliders[index];
      intVector2_2 = IntVector2.Min(intVector2_2, pixelCollider.Min);
      intVector2_3 = IntVector2.Max(intVector2_3, pixelCollider.Max);
    }
    if (collideWithTiles && (bool) (UnityEngine.Object) this.TileMap)
    {
      IntVector2 pixel1 = intVector2_2 - IntVector2.One;
      IntVector2 pixel2 = intVector2_3 + IntVector2.One;
      this.InitNearbyTileCheck(PhysicsEngine.PixelToUnit(pixel1), PhysicsEngine.PixelToUnit(pixel2), this.TileMap);
      DungeonData data = GameManager.Instance.Dungeon.data;
      for (PhysicsEngine.Tile nextNearbyTile = this.GetNextNearbyTile(data); nextNearbyTile != null; nextNearbyTile = this.GetNextNearbyTile(data))
      {
        for (int index1 = 0; index1 < rigidbody.PixelColliders.Count; ++index1)
        {
          PixelCollider pixelCollider1 = rigidbody.PixelColliders[index1];
          for (int index2 = 0; index2 < nextNearbyTile.PixelColliders.Count; ++index2)
          {
            PixelCollider pixelCollider2 = nextNearbyTile.PixelColliders[index2];
            if (pixelCollider1.CanCollideWith(pixelCollider2) && pixelCollider1.AABBOverlaps(pixelCollider2) && pixelCollider1.Overlaps(pixelCollider2))
            {
              CollisionData collisionData = PhysicsEngine.SingleCollision(rigidbody, pixelCollider1, (ICollidableObject) nextNearbyTile, pixelCollider2, (List<PixelCollider.StepData>) null, false);
              if (collisionData != null)
                tempOverlappingCollisions.Add(collisionData);
            }
          }
        }
      }
    }
    if (collideWithRigidbodies)
    {
      Func<SpeculativeRigidbody, bool> callback = (Func<SpeculativeRigidbody, bool>) (otherRigidbody =>
      {
        if ((bool) (UnityEngine.Object) otherRigidbody && (UnityEngine.Object) otherRigidbody != (UnityEngine.Object) rigidbody && otherRigidbody.enabled && otherRigidbody.CollideWithOthers && Array.IndexOf<SpeculativeRigidbody>(ignoreList, otherRigidbody) < 0 && (rigidbodyExcluder == null || !rigidbodyExcluder(otherRigidbody)))
        {
          for (int index3 = 0; index3 < rigidbody.PixelColliders.Count; ++index3)
          {
            PixelCollider pixelCollider3 = rigidbody.PixelColliders[index3];
            for (int index4 = 0; index4 < otherRigidbody.PixelColliders.Count; ++index4)
            {
              PixelCollider pixelCollider4 = otherRigidbody.PixelColliders[index4];
              if (collideWithTriggers || !pixelCollider4.IsTrigger)
              {
                bool flag;
                if (overrideCollisionMask.HasValue || ignoreCollisionMask.HasValue)
                {
                  int mask = !overrideCollisionMask.HasValue ? CollisionLayerMatrix.GetMask(pixelCollider3.CollisionLayer) : overrideCollisionMask.Value;
                  if (ignoreCollisionMask.HasValue)
                    mask &= ~ignoreCollisionMask.Value;
                  flag = pixelCollider4.CanCollideWith(mask);
                }
                else
                  flag = pixelCollider3.CanCollideWith(pixelCollider4);
                if (flag && pixelCollider3.AABBOverlaps(pixelCollider4) && pixelCollider3.Overlaps(pixelCollider4))
                {
                  CollisionData collisionData = PhysicsEngine.SingleCollision(rigidbody, pixelCollider3, (ICollidableObject) otherRigidbody, pixelCollider4, (List<PixelCollider.StepData>) null, false);
                  if (collisionData != null)
                    tempOverlappingCollisions.Add(collisionData);
                }
              }
            }
          }
        }
        return true;
      });
      this.m_rigidbodyTree.Query(PhysicsEngine.GetSafeB2AABB(intVector2_2, intVector2_3), callback);
      if (this.CollidesWithProjectiles(rigidbody))
        this.m_projectileTree.Query(PhysicsEngine.GetSafeB2AABB(intVector2_2, intVector2_3), callback);
    }
    if (overridePosition.HasValue)
    {
      for (int index = 0; index < rigidbody.PixelColliders.Count; ++index)
        rigidbody.PixelColliders[index].Position -= intVector2_1;
    }
    bool flag1 = tempOverlappingCollisions.Count > 0;
    if (overlappingCollisions == null)
    {
      for (int index = 0; index < tempOverlappingCollisions.Count; ++index)
      {
        CollisionData collisionData = tempOverlappingCollisions[index];
        CollisionData.Pool.Free(ref collisionData);
      }
    }
    else
    {
      overlappingCollisions.Clear();
      overlappingCollisions.AddRange((IEnumerable<CollisionData>) tempOverlappingCollisions);
    }
    return flag1;
  }

  public void RegisterOverlappingGhostCollisionExceptions(
    SpeculativeRigidbody specRigidbody,
    int? overrideLayerMask = null,
    bool includeTriggers = false)
  {
    if (!this.m_rigidbodies.Contains(specRigidbody))
      specRigidbody.Reinitialize();
    List<SpeculativeRigidbody> overlappingRigidbodies = this.GetOverlappingRigidbodies(specRigidbody, overrideLayerMask, includeTriggers);
    for (int index = 0; index < overlappingRigidbodies.Count; ++index)
    {
      specRigidbody.RegisterGhostCollisionException(overlappingRigidbodies[index]);
      overlappingRigidbodies[index].RegisterGhostCollisionException(specRigidbody);
    }
  }

  public List<SpeculativeRigidbody> GetOverlappingRigidbodies(
    SpeculativeRigidbody specRigidbody,
    int? overrideLayerMask = null,
    bool includeTriggers = false)
  {
    List<SpeculativeRigidbody> overlappingRigidbodies = new List<SpeculativeRigidbody>();
    for (int index = 0; index < specRigidbody.PixelColliders.Count; ++index)
      overlappingRigidbodies.AddRange((IEnumerable<SpeculativeRigidbody>) this.GetOverlappingRigidbodies(specRigidbody.PixelColliders[index], overrideLayerMask, includeTriggers));
    for (int index1 = 0; index1 < overlappingRigidbodies.Count - 1; ++index1)
    {
      for (int index2 = overlappingRigidbodies.Count - 1; index2 > index1; --index2)
      {
        if ((UnityEngine.Object) overlappingRigidbodies[index1] == (UnityEngine.Object) overlappingRigidbodies[index2])
          overlappingRigidbodies.RemoveAt(index2);
      }
    }
    return overlappingRigidbodies;
  }

  public List<SpeculativeRigidbody> GetOverlappingRigidbodies(
    PixelCollider pixelCollider,
    int? overrideLayerMask = null,
    bool includeTriggers = false)
  {
    List<SpeculativeRigidbody> overlappingRigidbodies = new List<SpeculativeRigidbody>();
    Func<SpeculativeRigidbody, bool> callback = (Func<SpeculativeRigidbody, bool>) (rigidbody =>
    {
      if (rigidbody.PixelColliders.Contains(pixelCollider) || !includeTriggers && pixelCollider.IsTrigger)
        return true;
      for (int index = 0; index < rigidbody.PixelColliders.Count; ++index)
      {
        PixelCollider pixelCollider1 = rigidbody.PixelColliders[index];
        if (includeTriggers || !pixelCollider1.IsTrigger)
        {
          if (overrideLayerMask.HasValue)
          {
            int mask = CollisionMask.LayerToMask(pixelCollider1.CollisionLayer);
            if ((overrideLayerMask.Value & mask) != mask)
              continue;
          }
          else if (!pixelCollider.CanCollideWith(pixelCollider1))
            continue;
          if (pixelCollider.AABBOverlaps(pixelCollider1))
            overlappingRigidbodies.Add(rigidbody);
        }
      }
      return true;
    });
    this.m_rigidbodyTree.Query(PhysicsEngine.GetSafeB2AABB(pixelCollider.Min, pixelCollider.Max), callback);
    if (this.CollidesWithProjectiles(pixelCollider))
      this.m_projectileTree.Query(PhysicsEngine.GetSafeB2AABB(pixelCollider.Min, pixelCollider.Max), callback);
    return overlappingRigidbodies;
  }

  public List<ICollidableObject> GetOverlappingCollidableObjects(
    Vector2 min,
    Vector2 max,
    bool collideWithTiles = true,
    bool collideWithRigidbodies = true,
    int? layerMask = null,
    bool includeTriggers = false)
  {
    List<ICollidableObject> overlappingRigidbodies = new List<ICollidableObject>();
    PixelCollider aabbCollider = new PixelCollider();
    aabbCollider.RegenerateFromManual(min, IntVector2.Zero, new IntVector2(Mathf.CeilToInt((float) (16.0 * ((double) max.x - (double) min.x))), Mathf.CeilToInt((float) (16.0 * ((double) max.y - (double) min.y)))));
    if (collideWithTiles && (bool) (UnityEngine.Object) this.TileMap)
    {
      IntVector2 pixel1 = aabbCollider.Min - IntVector2.One;
      IntVector2 pixel2 = aabbCollider.Max + IntVector2.One;
      this.InitNearbyTileCheck(PhysicsEngine.PixelToUnit(pixel1), PhysicsEngine.PixelToUnit(pixel2), this.TileMap);
      DungeonData data = GameManager.Instance.Dungeon.data;
      for (PhysicsEngine.Tile nextNearbyTile = this.GetNextNearbyTile(data); nextNearbyTile != null; nextNearbyTile = this.GetNextNearbyTile(data))
      {
        for (int index = 0; index < nextNearbyTile.PixelColliders.Count; ++index)
        {
          PixelCollider pixelCollider = nextNearbyTile.PixelColliders[index];
          if ((!layerMask.HasValue || pixelCollider.CanCollideWith(layerMask.Value)) && aabbCollider.AABBOverlaps(pixelCollider) && aabbCollider.Overlaps(pixelCollider))
            overlappingRigidbodies.Add((ICollidableObject) nextNearbyTile);
        }
      }
    }
    if (collideWithRigidbodies)
    {
      Func<SpeculativeRigidbody, bool> callback = (Func<SpeculativeRigidbody, bool>) (rigidbody =>
      {
        for (int index = 0; index < rigidbody.PixelColliders.Count; ++index)
        {
          PixelCollider pixelCollider = rigidbody.PixelColliders[index];
          if (includeTriggers || !pixelCollider.IsTrigger)
          {
            if (layerMask.HasValue)
            {
              int mask = CollisionMask.LayerToMask(pixelCollider.CollisionLayer);
              if ((layerMask.Value & mask) != mask)
                continue;
            }
            if (aabbCollider.AABBOverlaps(pixelCollider) && aabbCollider.Overlaps(pixelCollider))
              overlappingRigidbodies.Add((ICollidableObject) rigidbody);
          }
        }
        return true;
      });
      this.m_rigidbodyTree.Query(PhysicsEngine.GetSafeB2AABB(aabbCollider.Min, aabbCollider.Max), callback);
      int mask1 = CollisionMask.LayerToMask(CollisionLayer.Projectile);
      if (!layerMask.HasValue || (layerMask.Value & mask1) == mask1)
        this.m_projectileTree.Query(PhysicsEngine.GetSafeB2AABB(aabbCollider.Min, aabbCollider.Max), callback);
    }
    return overlappingRigidbodies;
  }

  public void Register(SpeculativeRigidbody rigidbody)
  {
    if ((UnityEngine.Object) rigidbody == (UnityEngine.Object) null)
      return;
    if (rigidbody.PhysicsRegistration == SpeculativeRigidbody.RegistrationState.Unknown)
      this.InferRegistration(rigidbody);
    switch (rigidbody.PhysicsRegistration)
    {
      case SpeculativeRigidbody.RegistrationState.DeregisterScheduled:
        this.m_deregisterRigidBodies.Remove(rigidbody);
        rigidbody.PhysicsRegistration = SpeculativeRigidbody.RegistrationState.Registered;
        break;
      case SpeculativeRigidbody.RegistrationState.Deregistered:
        this.m_rigidbodies.Add(rigidbody);
        rigidbody.proxyId = !rigidbody.IsSimpleProjectile ? this.m_rigidbodyTree.CreateProxy(rigidbody.b2AABB, rigidbody) : this.m_projectileTree.CreateProxy(rigidbody.b2AABB, rigidbody);
        rigidbody.PhysicsRegistration = SpeculativeRigidbody.RegistrationState.Registered;
        break;
    }
  }

  public void Deregister(SpeculativeRigidbody rigidbody)
  {
    if ((UnityEngine.Object) rigidbody == (UnityEngine.Object) null)
      return;
    if (rigidbody.PhysicsRegistration == SpeculativeRigidbody.RegistrationState.Unknown)
      this.InferRegistration(rigidbody);
    if (rigidbody.PhysicsRegistration == SpeculativeRigidbody.RegistrationState.Deregistered)
      return;
    if (rigidbody.PhysicsRegistration == SpeculativeRigidbody.RegistrationState.DeregisterScheduled)
      this.m_deregisterRigidBodies.Remove(rigidbody);
    this.m_rigidbodies.Remove(rigidbody);
    if (rigidbody.proxyId >= 0)
    {
      if (rigidbody.IsSimpleProjectile)
        this.m_projectileTree.DestroyProxy(rigidbody.proxyId);
      else
        this.m_rigidbodyTree.DestroyProxy(rigidbody.proxyId);
      rigidbody.proxyId = -1;
    }
    rigidbody.PhysicsRegistration = SpeculativeRigidbody.RegistrationState.Deregistered;
  }

  public void DeregisterWhenAvailable(SpeculativeRigidbody rigidbody)
  {
    if ((UnityEngine.Object) rigidbody == (UnityEngine.Object) null)
      return;
    if (rigidbody.PhysicsRegistration == SpeculativeRigidbody.RegistrationState.Unknown)
      this.InferRegistration(rigidbody);
    if (rigidbody.PhysicsRegistration != SpeculativeRigidbody.RegistrationState.Registered)
      return;
    this.m_deregisterRigidBodies.Add(rigidbody);
    rigidbody.PhysicsRegistration = SpeculativeRigidbody.RegistrationState.DeregisterScheduled;
  }

  private void InferRegistration(SpeculativeRigidbody rigidbody)
  {
    if (this.m_deregisterRigidBodies.Contains(rigidbody))
      rigidbody.PhysicsRegistration = SpeculativeRigidbody.RegistrationState.DeregisterScheduled;
    else if (this.m_rigidbodies.Contains(rigidbody))
      rigidbody.PhysicsRegistration = SpeculativeRigidbody.RegistrationState.Registered;
    else
      rigidbody.PhysicsRegistration = SpeculativeRigidbody.RegistrationState.Deregistered;
  }

  private void CollideWithRigidbodies(
    SpeculativeRigidbody rigidbody,
    List<PixelCollider.StepData> stepList,
    ref CollisionData nearestCollision)
  {
    if (!(bool) (UnityEngine.Object) rigidbody || !rigidbody.enabled)
      return;
    b2AABB b2Aabb = rigidbody.b2AABB;
    IntVector2 pixelsToMove = rigidbody.PixelsToMove;
    if (pixelsToMove.x < 0)
    {
      b2Aabb.lowerBound.x += (float) ((double) pixelsToMove.x * (1.0 / 16.0) - 1.0 / 16.0);
      b2Aabb.upperBound.x += 1f / 16f;
    }
    else
    {
      b2Aabb.lowerBound.x -= 1f / 16f;
      b2Aabb.upperBound.x += (float) ((double) pixelsToMove.x * (1.0 / 16.0) + 1.0 / 16.0);
    }
    if (pixelsToMove.y < 0)
    {
      b2Aabb.lowerBound.y += (float) ((double) pixelsToMove.Y * (1.0 / 16.0) - 1.0 / 16.0);
      b2Aabb.upperBound.y += 1f / 16f;
    }
    else
    {
      b2Aabb.lowerBound.y -= 1f / 16f;
      b2Aabb.upperBound.y += (float) ((double) pixelsToMove.y * (1.0 / 16.0) + 1.0 / 16.0);
    }
    PhysicsEngine.m_cwrqRigidbody = rigidbody;
    PhysicsEngine.m_cwrqStepList = stepList;
    PhysicsEngine.m_cwrqCollisionData = nearestCollision;
    b2DynamicTree rigidbodyTree = this.m_rigidbodyTree;
    b2AABB aabb1 = b2Aabb;
    // ISSUE: reference to a compiler-generated field
    if (PhysicsEngine.\u003C\u003Ef__mg\u0024cache0 == null)
    {
      // ISSUE: reference to a compiler-generated field
      PhysicsEngine.\u003C\u003Ef__mg\u0024cache0 = new Func<SpeculativeRigidbody, bool>(PhysicsEngine.CollideWithRigidbodiesQuery);
    }
    // ISSUE: reference to a compiler-generated field
    Func<SpeculativeRigidbody, bool> fMgCache0 = PhysicsEngine.\u003C\u003Ef__mg\u0024cache0;
    rigidbodyTree.Query(aabb1, fMgCache0);
    if (this.CollidesWithProjectiles(rigidbody))
    {
      b2DynamicTree projectileTree = this.m_projectileTree;
      b2AABB aabb2 = b2Aabb;
      // ISSUE: reference to a compiler-generated field
      if (PhysicsEngine.\u003C\u003Ef__mg\u0024cache1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PhysicsEngine.\u003C\u003Ef__mg\u0024cache1 = new Func<SpeculativeRigidbody, bool>(PhysicsEngine.CollideWithRigidbodiesQuery);
      }
      // ISSUE: reference to a compiler-generated field
      Func<SpeculativeRigidbody, bool> fMgCache1 = PhysicsEngine.\u003C\u003Ef__mg\u0024cache1;
      projectileTree.Query(aabb2, fMgCache1);
    }
    nearestCollision = PhysicsEngine.m_cwrqCollisionData;
    PhysicsEngine.m_cwrqRigidbody = (SpeculativeRigidbody) null;
    PhysicsEngine.m_cwrqStepList = (List<PixelCollider.StepData>) null;
    PhysicsEngine.m_cwrqCollisionData = (CollisionData) null;
  }

  private static bool CollideWithRigidbodiesQuery(SpeculativeRigidbody otherRigidbody)
  {
    for (int index1 = 0; index1 < PhysicsEngine.m_cwrqRigidbody.PixelColliders.Count; ++index1)
    {
      PixelCollider pixelCollider1 = PhysicsEngine.m_cwrqRigidbody.PixelColliders[index1];
      for (int index2 = 0; index2 < otherRigidbody.PixelColliders.Count; ++index2)
      {
        PixelCollider pixelCollider2 = otherRigidbody.PixelColliders[index2];
        CollisionData collisionData = PhysicsEngine.SingleCollision(PhysicsEngine.m_cwrqRigidbody, pixelCollider1, (ICollidableObject) otherRigidbody, pixelCollider2, PhysicsEngine.m_cwrqStepList, true);
        if (collisionData != null)
        {
          if (PhysicsEngine.m_cwrqCollisionData == null || (double) collisionData.TimeUsed < (double) PhysicsEngine.m_cwrqCollisionData.TimeUsed)
          {
            CollisionData.Pool.Free(ref PhysicsEngine.m_cwrqCollisionData);
            PhysicsEngine.m_cwrqCollisionData = collisionData;
          }
          else
            CollisionData.Pool.Free(ref collisionData);
        }
      }
    }
    return true;
  }

  private void CollideWithTilemap(
    SpeculativeRigidbody rigidbody,
    PixelCollider pixelCollider,
    List<PixelCollider.StepData> stepList,
    ref float timeUsed,
    DungeonData dungeonData,
    ref CollisionData nearestCollision)
  {
    Position position = rigidbody.m_position;
    IntVector2 intVector2 = pixelCollider.m_offset + pixelCollider.m_transformOffset;
    float positionX = (float) ((double) position.m_position.x * (1.0 / 16.0) + (double) position.m_remainder.x + (double) intVector2.x * (1.0 / 16.0));
    float positionY = (float) ((double) position.m_position.y * (1.0 / 16.0) + (double) position.m_remainder.y + (double) intVector2.y * (1.0 / 16.0));
    IntVector2 pixelsToMove = rigidbody.PixelsToMove;
    float num1 = positionX + (float) pixelsToMove.x * (1f / 16f);
    float num2 = positionY + (float) pixelsToMove.y * (1f / 16f);
    IntVector2 dimensions = pixelCollider.m_dimensions;
    float worldMinX;
    float worldMaxX;
    if ((double) positionX < (double) num1)
    {
      worldMinX = positionX - 0.25f;
      worldMaxX = (float) ((double) num1 + 0.25 + (double) dimensions.x * (1.0 / 16.0));
    }
    else
    {
      worldMinX = num1 - 0.25f;
      worldMaxX = (float) ((double) positionX + 0.25 + (double) dimensions.x * (1.0 / 16.0));
    }
    float worldMinY;
    float worldMaxY;
    if ((double) positionY < (double) num2)
    {
      worldMinY = positionY - 0.25f;
      worldMaxY = (float) ((double) num2 + 0.25 + (double) dimensions.y * (1.0 / 16.0));
    }
    else
    {
      worldMinY = num2 - 0.25f;
      worldMaxY = (float) ((double) positionY + 0.25 + (double) dimensions.y * (1.0 / 16.0));
    }
    this.InitNearbyTileCheck(worldMinX, worldMinY, worldMaxX, worldMaxY, this.TileMap, dimensions, positionX, positionY, pixelsToMove, dungeonData);
    for (PhysicsEngine.Tile nextNearbyTile = this.GetNextNearbyTile(dungeonData); nextNearbyTile != null; nextNearbyTile = this.GetNextNearbyTile(dungeonData))
    {
      for (int index = 0; index < nextNearbyTile.PixelColliders.Count; ++index)
      {
        CollisionData collisionData = PhysicsEngine.SingleCollision(rigidbody, pixelCollider, (ICollidableObject) nextNearbyTile, nextNearbyTile.PixelColliders[index], stepList, true);
        if (collisionData != null)
        {
          if (nearestCollision == null || (double) collisionData.TimeUsed < (double) nearestCollision.TimeUsed)
          {
            CollisionData.Pool.Free(ref nearestCollision);
            nearestCollision = collisionData;
          }
          else
            CollisionData.Pool.Free(ref collisionData);
          this.m_nbt.Finish(dungeonData);
        }
      }
    }
    this.CleanupNearbyTileCheck();
  }

  private static CollisionData SingleCollision(
    SpeculativeRigidbody rigidbody,
    PixelCollider collider,
    ICollidableObject otherCollidable,
    PixelCollider otherCollider,
    List<PixelCollider.StepData> stepList,
    bool doPreCollision)
  {
    if (collider == null || otherCollider == null)
      return (CollisionData) null;
    if (!collider.AABBOverlaps(otherCollider, rigidbody.PixelsToMove))
      return (CollisionData) null;
    if (!otherCollidable.CanCollideWith(rigidbody))
      return (CollisionData) null;
    if (!otherCollider.CanCollideWith(collider))
      return (CollisionData) null;
    LinearCastResult result = (LinearCastResult) null;
    CollisionData collisionData = (CollisionData) null;
    if (otherCollider.DirectionIgnorer != null || !collider.Overlaps(otherCollider))
    {
      if (!collider.LinearCast(otherCollider, rigidbody.PixelsToMove, stepList, out result))
        result = (LinearCastResult) null;
    }
    else if (collider.IsTrigger || otherCollider.IsTrigger)
    {
      result = LinearCastResult.Pool.Allocate();
      result.Contact = rigidbody.UnitCenter;
      result.Normal = Vector2.up;
      result.MyPixelCollider = collider;
      result.OtherPixelCollider = otherCollider;
      result.TimeUsed = 0.0f;
      result.CollidedX = true;
      result.CollidedY = true;
      result.NewPixelsToMove = IntVector2.Zero;
      result.Overlap = true;
    }
    else
    {
      IntVector2[] cardinalsAndOrdinals = IntVector2.CardinalsAndOrdinals;
      int num1 = 0;
      int num2 = 1;
      while (true)
      {
        do
        {
          for (int index = 0; index < cardinalsAndOrdinals.Length; ++index)
          {
            if (!collider.Overlaps(otherCollider, cardinalsAndOrdinals[index] * num2))
            {
              result = LinearCastResult.Pool.Allocate();
              result.Contact = rigidbody.UnitCenter;
              result.Normal = cardinalsAndOrdinals[index].ToVector2().normalized;
              result.MyPixelCollider = collider;
              result.OtherPixelCollider = otherCollider;
              result.TimeUsed = 0.0f;
              result.CollidedX = true;
              result.CollidedY = true;
              result.NewPixelsToMove = IntVector2.Zero;
              result.Overlap = true;
              goto label_21;
            }
          }
          ++num2;
          ++num1;
        }
        while (num1 <= 100);
        UnityEngine.Debug.LogError((object) $"FREEZE AVERTED!  TELL RUBEL!  (you're welcome) [{rigidbody.name}] & [{(!(otherCollidable is SpeculativeRigidbody) ? (object) "tile" : (object) ((UnityEngine.Object) otherCollidable).name)}]");
      }
    }
label_21:
    if (result != null)
    {
      if (doPreCollision)
      {
        switch (otherCollidable)
        {
          case SpeculativeRigidbody _:
            SpeculativeRigidbody speculativeRigidbody = otherCollidable as SpeculativeRigidbody;
            PhysicsEngine.SkipCollision = false;
            PhysicsEngine.PendingCastResult = result;
            if (rigidbody.OnPreRigidbodyCollision != null)
              rigidbody.OnPreRigidbodyCollision(rigidbody, collider, speculativeRigidbody, otherCollider);
            if (speculativeRigidbody.OnPreRigidbodyCollision != null)
              speculativeRigidbody.OnPreRigidbodyCollision(speculativeRigidbody, otherCollider, rigidbody, collider);
            if (PhysicsEngine.SkipCollision)
            {
              LinearCastResult.Pool.Free(ref result);
              return (CollisionData) null;
            }
            break;
          case PhysicsEngine.Tile _:
            PhysicsEngine.Tile tile = otherCollidable as PhysicsEngine.Tile;
            PhysicsEngine.SkipCollision = false;
            PhysicsEngine.PendingCastResult = result;
            if (rigidbody.OnPreTileCollision != null)
              rigidbody.OnPreTileCollision(rigidbody, collider, tile, otherCollider);
            if (PhysicsEngine.SkipCollision)
            {
              LinearCastResult.Pool.Free(ref result);
              return (CollisionData) null;
            }
            break;
        }
      }
      collisionData = CollisionData.Pool.Allocate();
      collisionData.SetAll(result);
      collisionData.MyRigidbody = rigidbody;
      switch (otherCollidable)
      {
        case SpeculativeRigidbody _:
          collisionData.collisionType = CollisionData.CollisionType.Rigidbody;
          collisionData.OtherRigidbody = (SpeculativeRigidbody) otherCollidable;
          break;
        case PhysicsEngine.Tile _:
          collisionData.collisionType = CollisionData.CollisionType.TileMap;
          collisionData.TileLayerName = ((PhysicsEngine.Tile) otherCollidable).LayerName;
          collisionData.TilePosition = ((PhysicsEngine.Tile) otherCollidable).Position;
          break;
      }
      LinearCastResult.Pool.Free(ref result);
    }
    return collisionData;
  }

  private bool CollidesWithProjectiles(int mask, CollisionLayer? sourceLayer)
  {
    if ((mask & this.m_cachedProjectileMask) != this.m_cachedProjectileMask)
      return false;
    return !sourceLayer.HasValue || (CollisionLayerMatrix.GetMask(sourceLayer.Value) & this.m_cachedProjectileMask) == this.m_cachedProjectileMask;
  }

  private bool CollidesWithProjectiles(SpeculativeRigidbody specRigidbody)
  {
    List<PixelCollider> pixelColliders = specRigidbody.PixelColliders;
    for (int index = 0; index < pixelColliders.Count; ++index)
    {
      PixelCollider pixelCollider = pixelColliders[index];
      if ((CollisionLayerMatrix.GetMask(pixelCollider.CollisionLayer) & this.m_cachedProjectileMask) == this.m_cachedProjectileMask || (pixelCollider.CollisionLayerCollidableOverride & this.m_cachedProjectileMask) == this.m_cachedProjectileMask)
        return true;
    }
    return false;
  }

  private bool CollidesWithProjectiles(PixelCollider pixelCollider)
  {
    return (CollisionLayerMatrix.GetMask(pixelCollider.CollisionLayer) & this.m_cachedProjectileMask) == this.m_cachedProjectileMask;
  }

  public void ClearAllCachedTiles()
  {
    Dungeon dungeon = GameManager.Instance.Dungeon;
    DungeonData data = dungeon.data;
    for (int x = 0; x < dungeon.Width; ++x)
    {
      for (int y = 0; y < dungeon.Height; ++y)
      {
        CellData cellData = data[x, y];
        if (cellData != null)
        {
          cellData.HasCachedPhysicsTile = false;
          cellData.CachedPhysicsTile = (PhysicsEngine.Tile) null;
        }
      }
    }
  }

  private PhysicsEngine.Tile GetTile(
    int x,
    int y,
    tk2dTileMap tileMap,
    int layer,
    string layerName,
    DungeonData dungeonData)
  {
    CellData cellData1;
    if (x < 0 || x >= this.m_cachedDungeonWidth || y < 0 || y >= this.m_cachedDungeonHeight || (cellData1 = dungeonData.cellData[x][y]) == null)
      return (PhysicsEngine.Tile) null;
    if (cellData1.HasCachedPhysicsTile)
      return cellData1.CachedPhysicsTile;
    if (cellData1.type == CellType.WALL && GameManager.Instance.Dungeon.data.CheckInBoundsAndValid(cellData1.position + IntVector2.Up))
    {
      CellData cellData2 = GameManager.Instance.Dungeon.data[cellData1.position + IntVector2.Up];
      if (cellData2 != null && cellData2.isOccludedByTopWall && (cellData2.diagonalWallType == DiagonalWallType.SOUTHEAST || cellData2.diagonalWallType == DiagonalWallType.SOUTHWEST))
      {
        PhysicsEngine.Tile tile = this.GetTile(x, y + 1, tileMap, layer, layerName, dungeonData);
        cellData2.HasCachedPhysicsTile = true;
        cellData2.CachedPhysicsTile = tile;
        return tile;
      }
    }
    int tile1 = this.GetTile(layer, cellData1.positionInTilemap.x, cellData1.positionInTilemap.y);
    List<PixelCollider> pixelColliders = new List<PixelCollider>();
    Vector2 vector2 = Vector2.Scale(new Vector2((float) x, (float) y), tileMap.data.tileSize.XY());
    IntVector2 pixelPosition = new Position((Vector2) tileMap.transform.position + vector2).PixelPosition;
    if (tile1 >= 0)
    {
      tk2dSpriteDefinition spriteDefinition = tileMap.SpriteCollectionInst.spriteDefinitions[tile1];
      if (spriteDefinition.IsTileSquare)
      {
        PixelCollider pixelCollider = new PixelCollider()
        {
          IsTileCollider = true,
          CollisionLayer = spriteDefinition.collisionLayer
        };
        pixelCollider.RegenerateFromManual(tileMap.transform, new IntVector2(0, 0), new IntVector2(16 /*0x10*/, 16 /*0x10*/));
        pixelCollider.Position = pixelPosition;
        pixelColliders.Add(pixelCollider);
      }
      else
      {
        PixelCollider pixelCollider = new PixelCollider()
        {
          IsTileCollider = true,
          CollisionLayer = spriteDefinition.collisionLayer
        };
        pixelCollider.RegenerateFrom3dCollider(spriteDefinition.colliderVertices, tileMap.transform);
        pixelCollider.Position = pixelPosition;
        pixelColliders.Add(pixelCollider);
      }
    }
    else if (cellData1.cellVisualData.precludeAllTileDrawing && cellData1.type == CellType.WALL)
    {
      PixelCollider pixelCollider = new PixelCollider()
      {
        IsTileCollider = true,
        CollisionLayer = CollisionLayer.HighObstacle
      };
      pixelCollider.RegenerateFromManual(tileMap.transform, new IntVector2(0, 0), new IntVector2(16 /*0x10*/, 16 /*0x10*/));
      pixelCollider.Position = pixelPosition;
      pixelColliders.Add(pixelCollider);
    }
    if (cellData1.isOccludedByTopWall && !GameManager.Instance.IsFoyer)
    {
      if (cellData1.diagonalWallType == DiagonalWallType.SOUTHEAST || cellData1.diagonalWallType == DiagonalWallType.SOUTHWEST)
      {
        PixelCollider pixelCollider1 = new PixelCollider()
        {
          IsTileCollider = true,
          CollisionLayer = CollisionLayer.EnemyBlocker
        };
        pixelCollider1.RegenerateFromManual(tileMap.transform, new IntVector2(0, 0), new IntVector2(16 /*0x10*/, 28));
        pixelCollider1.Position = pixelPosition + new IntVector2(0, -16);
        pixelColliders.Add(pixelCollider1);
        if (cellData1.diagonalWallType == DiagonalWallType.SOUTHEAST)
        {
          PixelCollider pixelCollider2 = new PixelCollider()
          {
            IsTileCollider = true,
            CollisionLayer = CollisionLayer.EnemyBulletBlocker
          };
          int y1 = 14;
          pixelCollider2.RegenerateFromLine(tileMap.transform, new IntVector2(1, y1 - 16 /*0x10*/), new IntVector2(16 /*0x10*/, y1 - 1));
          pixelCollider2.Position = pixelPosition + new IntVector2(0, y1 - 16 /*0x10*/);
          pixelColliders.Add(pixelCollider2);
          PixelCollider pixelCollider3 = new PixelCollider()
          {
            IsTileCollider = true,
            CollisionLayer = CollisionLayer.EnemyBulletBlocker
          };
          pixelCollider3.RegenerateFromManual(tileMap.transform, new IntVector2(1, y1 - 16 /*0x10*/), new IntVector2(16 /*0x10*/, y1));
          pixelCollider3.Position = pixelPosition + new IntVector2(0, -16);
          pixelColliders.Add(pixelCollider3);
          PixelCollider pixelCollider4 = new PixelCollider()
          {
            IsTileCollider = true,
            CollisionLayer = CollisionLayer.HighObstacle
          };
          int y2 = 8;
          pixelCollider4.RegenerateFromLine(tileMap.transform, new IntVector2(1, y2 - 16 /*0x10*/), new IntVector2(16 /*0x10*/, y2 - 1));
          pixelCollider4.Position = pixelPosition + new IntVector2(0, y2 - 16 /*0x10*/);
          pixelColliders.Add(pixelCollider4);
          PixelCollider pixelCollider5 = new PixelCollider()
          {
            IsTileCollider = true,
            CollisionLayer = CollisionLayer.HighObstacle
          };
          pixelCollider5.RegenerateFromManual(tileMap.transform, new IntVector2(1, y2 - 16 /*0x10*/), new IntVector2(16 /*0x10*/, y2));
          pixelCollider5.Position = pixelPosition + new IntVector2(0, -16);
          pixelColliders.Add(pixelCollider5);
        }
        else if (cellData1.diagonalWallType == DiagonalWallType.SOUTHWEST)
        {
          PixelCollider pixelCollider6 = new PixelCollider()
          {
            IsTileCollider = true,
            CollisionLayer = CollisionLayer.EnemyBulletBlocker
          };
          int y3 = 14;
          pixelCollider6.RegenerateFromLine(tileMap.transform, new IntVector2(0, y3 - 1), new IntVector2(15, y3 - 16 /*0x10*/));
          pixelCollider6.Position = pixelPosition + new IntVector2(0, y3 - 16 /*0x10*/);
          pixelColliders.Add(pixelCollider6);
          PixelCollider pixelCollider7 = new PixelCollider()
          {
            IsTileCollider = true,
            CollisionLayer = CollisionLayer.EnemyBulletBlocker
          };
          pixelCollider7.RegenerateFromManual(tileMap.transform, new IntVector2(1, y3 - 16 /*0x10*/), new IntVector2(16 /*0x10*/, y3));
          pixelCollider7.Position = pixelPosition + new IntVector2(0, -16);
          pixelColliders.Add(pixelCollider7);
          PixelCollider pixelCollider8 = new PixelCollider()
          {
            IsTileCollider = true,
            CollisionLayer = CollisionLayer.HighObstacle
          };
          int y4 = 8;
          pixelCollider8.RegenerateFromLine(tileMap.transform, new IntVector2(0, y4 - 1), new IntVector2(15, y4 - 16 /*0x10*/));
          pixelCollider8.Position = pixelPosition + new IntVector2(0, y4 - 16 /*0x10*/);
          pixelColliders.Add(pixelCollider8);
          PixelCollider pixelCollider9 = new PixelCollider()
          {
            IsTileCollider = true,
            CollisionLayer = CollisionLayer.HighObstacle
          };
          pixelCollider9.RegenerateFromManual(tileMap.transform, new IntVector2(1, y4 - 16 /*0x10*/), new IntVector2(16 /*0x10*/, y4));
          pixelCollider9.Position = pixelPosition + new IntVector2(0, -16);
          pixelColliders.Add(pixelCollider9);
        }
      }
      else
      {
        PixelCollider pixelCollider10 = new PixelCollider()
        {
          IsTileCollider = true,
          CollisionLayer = CollisionLayer.EnemyBlocker
        };
        pixelCollider10.RegenerateFromManual(tileMap.transform, new IntVector2(0, 0), new IntVector2(16 /*0x10*/, 12));
        pixelCollider10.Position = pixelPosition;
        pixelColliders.Add(pixelCollider10);
        PixelCollider pixelCollider11 = new PixelCollider()
        {
          IsTileCollider = true,
          CollisionLayer = CollisionLayer.EnemyBulletBlocker
        };
        pixelCollider11.RegenerateFromManual(tileMap.transform, new IntVector2(0, 0), new IntVector2(16 /*0x10*/, 14));
        pixelCollider11.Position = pixelPosition;
        pixelColliders.Add(pixelCollider11);
        PixelCollider pixelCollider12 = new PixelCollider()
        {
          IsTileCollider = true,
          CollisionLayer = CollisionLayer.PlayerBlocker
        };
        pixelCollider12.RegenerateFromManual(tileMap.transform, new IntVector2(0, 0), new IntVector2(16 /*0x10*/, 8));
        pixelCollider12.Position = pixelPosition;
        pixelColliders.Add(pixelCollider12);
      }
    }
    if (cellData1.IsLowerFaceWall() && !GameManager.Instance.IsFoyer && cellData1.diagonalWallType != DiagonalWallType.SOUTHEAST && cellData1.diagonalWallType == DiagonalWallType.SOUTHWEST)
    {
      if (!GameManager.Instance.Dungeon.data.isWall(cellData1.position.x - 1, cellData1.position.y))
      {
        PixelCollider pixelCollider = new PixelCollider()
        {
          IsTileCollider = true,
          CollisionLayer = CollisionLayer.BulletBlocker
        };
        pixelCollider.RegenerateFromManual(tileMap.transform, new IntVector2(0, 0), new IntVector2(3, 10));
        pixelCollider.Position = pixelPosition + new IntVector2(0, 6);
        pixelCollider.DirectionIgnorer = (Func<IntVector2, bool>) (dir => dir.x >= 0 || dir.y <= 0);
        pixelCollider.NormalModifier = (Func<Vector2, Vector2>) (normal => (double) normal.x > 0.0 ? Vector2.down : normal);
        pixelColliders.Add(pixelCollider);
      }
      if (!GameManager.Instance.Dungeon.data.isWall(cellData1.position.x + 1, cellData1.position.y))
      {
        PixelCollider pixelCollider = new PixelCollider()
        {
          IsTileCollider = true,
          CollisionLayer = CollisionLayer.BulletBlocker
        };
        pixelCollider.RegenerateFromManual(tileMap.transform, new IntVector2(0, 0), new IntVector2(3, 10));
        pixelCollider.Position = pixelPosition + new IntVector2(13, 6);
        pixelCollider.DirectionIgnorer = (Func<IntVector2, bool>) (dir => dir.x <= 0 || dir.y <= 0);
        pixelCollider.NormalModifier = (Func<Vector2, Vector2>) (normal => (double) normal.x < 0.0 ? Vector2.down : normal);
        pixelColliders.Add(pixelCollider);
      }
    }
    if (pixelColliders.Count == 0)
    {
      cellData1.HasCachedPhysicsTile = true;
      cellData1.CachedPhysicsTile = (PhysicsEngine.Tile) null;
      return (PhysicsEngine.Tile) null;
    }
    PhysicsEngine.Tile tile2 = new PhysicsEngine.Tile(pixelColliders, x, y, layerName);
    cellData1.HasCachedPhysicsTile = true;
    cellData1.CachedPhysicsTile = tile2;
    return tile2;
  }

  private void InitNearbyTileCheck(Vector2 worldMin, Vector2 worldMax, tk2dTileMap tileMap)
  {
    if ((UnityEngine.Object) this.m_nbt.tileMap == (UnityEngine.Object) null)
    {
      this.m_nbt.tileMap = tileMap;
      this.m_nbt.layerName = "Collision Layer";
      this.m_nbt.layer = BraveUtility.GetTileMapLayerByName("Collision Layer", this.TileMap);
    }
    this.m_nbt.Init(worldMin.x, worldMin.y, worldMax.x, worldMax.y);
  }

  private void InitNearbyTileCheck(
    float worldMinX,
    float worldMinY,
    float worldMaxX,
    float worldMaxY,
    tk2dTileMap tileMap,
    IntVector2 pixelColliderDimensions,
    float positionX,
    float positionY,
    IntVector2 pixelsToMove,
    DungeonData dungeonData)
  {
    if ((UnityEngine.Object) this.m_nbt.tileMap == (UnityEngine.Object) null)
    {
      this.m_nbt.tileMap = tileMap;
      this.m_nbt.layerName = "Collision Layer";
      this.m_nbt.layer = BraveUtility.GetTileMapLayerByName("Collision Layer", this.TileMap);
    }
    this.m_nbt.Init(worldMinX, worldMinY, worldMaxX, worldMaxY, pixelColliderDimensions, positionX, positionY, pixelsToMove, dungeonData);
  }

  private PhysicsEngine.Tile GetNextNearbyTile(DungeonData dungeonData)
  {
    return this.m_nbt.GetNextNearbyTile(dungeonData);
  }

  private void CleanupNearbyTileCheck() => this.m_nbt.Cleanup();

  private int GetTile(int layer, int x, int y)
  {
    return x >= 0 && x < this.TileMap.width && y >= 0 && y < this.TileMap.height ? this.TileMap.Layers[layer].GetTile(x, y) : -1;
  }

  public static void PixelMovementGenerator(
    IntVector2 pixelsToMove,
    List<PixelCollider.StepData> steps)
  {
    steps.Clear();
    IntVector2 zero = IntVector2.Zero;
    float num = 1f / (float) (Mathf.Abs(pixelsToMove.x) + Mathf.Abs(pixelsToMove.y));
    while (zero.x != pixelsToMove.x || zero.y != pixelsToMove.y)
    {
      IntVector2 intVector2 = zero.x != pixelsToMove.x ? (zero.y != pixelsToMove.y ? ((double) Mathf.Abs((float) zero.x / (float) pixelsToMove.x) >= (double) Mathf.Abs((float) zero.y / (float) pixelsToMove.y) ? new IntVector2(0, Math.Sign(pixelsToMove.y)) : new IntVector2(Math.Sign(pixelsToMove.x), 0)) : new IntVector2(Math.Sign(pixelsToMove.x), 0)) : new IntVector2(0, Math.Sign(pixelsToMove.y));
      zero += intVector2;
      steps.Add(new PixelCollider.StepData()
      {
        deltaPos = intVector2,
        deltaTime = num
      });
    }
  }

  public static void PixelMovementGenerator(
    SpeculativeRigidbody rigidbody,
    List<PixelCollider.StepData> stepList)
  {
    PhysicsEngine.PixelMovementGenerator(rigidbody.m_position.m_remainder, rigidbody.Velocity, rigidbody.PixelsToMove, stepList);
  }

  private static void PixelMovementGenerator(
    Vector2 remainder,
    Vector2 velocity,
    IntVector2 pixelsToMove,
    List<PixelCollider.StepData> stepList)
  {
    stepList.Clear();
    float num = 1f / 32f;
    IntVector2 intVector2;
    intVector2.x = 0;
    intVector2.y = 0;
    int x = Math.Sign(pixelsToMove.x);
    int y = Math.Sign(pixelsToMove.y);
    if (pixelsToMove.y == 0)
    {
      while (intVector2.x != pixelsToMove.x)
      {
        float deltaTime = Mathf.Max(0.0f, ((float) x * num - remainder.x) / velocity.x);
        intVector2.x += x;
        remainder.x = (float) x * -num;
        remainder.y += deltaTime * velocity.y;
        stepList.Add(new PixelCollider.StepData(new IntVector2(x, 0), deltaTime));
      }
    }
    else if (pixelsToMove.x == 0)
    {
      while (intVector2.y != pixelsToMove.y)
      {
        float deltaTime = Mathf.Max(0.0f, ((float) y * num - remainder.y) / velocity.y);
        intVector2.y += y;
        remainder.x += deltaTime * velocity.x;
        remainder.y = (float) y * -num;
        stepList.Add(new PixelCollider.StepData(new IntVector2(0, y), deltaTime));
      }
    }
    else
    {
      while (intVector2.x != pixelsToMove.x || intVector2.y != pixelsToMove.y)
      {
        float deltaTime1 = Mathf.Max(0.0f, ((float) x * num - remainder.x) / velocity.x);
        float deltaTime2 = Mathf.Max(0.0f, ((float) y * num - remainder.y) / velocity.y);
        if (intVector2.x != pixelsToMove.x && (intVector2.y == pixelsToMove.y || (double) deltaTime1 < (double) deltaTime2))
        {
          intVector2.x += x;
          remainder.x = (float) x * -num;
          remainder.y += deltaTime1 * velocity.y;
          stepList.Add(new PixelCollider.StepData(new IntVector2(x, 0), deltaTime1));
        }
        else
        {
          intVector2.y += y;
          remainder.x += deltaTime2 * velocity.x;
          remainder.y = (float) y * -num;
          stepList.Add(new PixelCollider.StepData(new IntVector2(0, y), deltaTime2));
        }
      }
    }
  }

  private void SortRigidbodies()
  {
    bool flag = false;
    for (int index = 0; index < this.m_rigidbodies.Count; ++index)
    {
      SpeculativeRigidbody rigidbody = this.m_rigidbodies[index];
      int num = (!rigidbody.CanBePushed ? 0 : 1) << (!rigidbody.CanPush ? 0 : 1) << 1 + (!rigidbody.CanCarry ? 0 : 1) << 2;
      if (num != rigidbody.SortHash)
        flag = true;
      rigidbody.SortHash = num;
    }
    if (!flag)
      return;
    this.m_rigidbodies.Sort((Comparison<SpeculativeRigidbody>) ((lhs, rhs) =>
    {
      if (lhs.CanCarry && !rhs.CanCarry)
        return -1;
      if (!lhs.CanCarry && rhs.CanCarry)
        return 1;
      if (lhs.CanPush && !rhs.CanPush)
        return -1;
      if (!lhs.CanPush && rhs.CanPush)
        return 1;
      if (lhs.CanPush && rhs.CanPush)
      {
        if (!lhs.CanBePushed && rhs.CanBePushed)
          return -1;
        if (lhs.CanBePushed && !rhs.CanBePushed)
          return 1;
      }
      return 0;
    }));
  }

  public static void UpdatePosition(SpeculativeRigidbody specRigidbody)
  {
    Vector2 displacement = specRigidbody.Velocity * UnityEngine.Random.Range(0.8f, 1.2f);
    if (specRigidbody.IsSimpleProjectile)
      PhysicsEngine.Instance.m_projectileTree.MoveProxy(specRigidbody.proxyId, specRigidbody.b2AABB, displacement);
    else
      PhysicsEngine.Instance.m_rigidbodyTree.MoveProxy(specRigidbody.proxyId, specRigidbody.b2AABB, displacement);
  }

  public static float PixelToUnit(int pixel) => (float) pixel / 16f;

  public static Vector2 PixelToUnit(IntVector2 pixel) => (Vector2) pixel / 16f;

  public static float PixelToUnitMidpoint(int pixel)
  {
    return (float) pixel / 16f + PhysicsEngine.Instance.HalfPixelUnitWidth;
  }

  public static Vector2 PixelToUnitMidpoint(IntVector2 pixel)
  {
    return (Vector2) pixel / 16f + new Vector2(PhysicsEngine.Instance.HalfPixelUnitWidth, PhysicsEngine.Instance.HalfPixelUnitWidth);
  }

  public static int UnitToPixel(float unit) => (int) ((double) unit * 16.0);

  public static IntVector2 UnitToPixel(Vector2 unit)
  {
    return new IntVector2(PhysicsEngine.UnitToPixel(unit.x), PhysicsEngine.UnitToPixel(unit.y));
  }

  public static int UnitRoundToPixel(float unit) => Mathf.RoundToInt(unit * 16f);

  public static IntVector2 UnitRoundToPixel(Vector2 unit)
  {
    return new IntVector2(PhysicsEngine.UnitRoundToPixel(unit.x), PhysicsEngine.UnitRoundToPixel(unit.y));
  }

  private static Vector2 GetSlopeScalar(float slope) => new Vector2(1f, slope).normalized;

  private static Vector2 RotateXTowardSlope(Vector2 v, float slope)
  {
    return PhysicsEngine.GetSlopeScalar(slope) * v.x + new Vector2(0.0f, v.y);
  }

  public enum DebugDrawType
  {
    None,
    Boundaries,
    FullPixels,
  }

  private class Raycaster
  {
    private PhysicsEngine physicsEngine;
    private DungeonData dungeonData;
    private Position origin;
    private Vector2 direction;
    private float dist;
    private bool collideWithTiles;
    private bool collideWithRigidbodies;
    private int rayMask;
    private CollisionLayer? sourceLayer;
    private bool collideWithTriggers;
    private Func<SpeculativeRigidbody, bool> rigidbodyExcluder;
    private ICollection<SpeculativeRigidbody> ignoreList;
    private RaycastResult nearestRigidbodyHit;
    private Vector2 p1;
    private float p1p2Dist;
    private Func<b2RayCastInput, SpeculativeRigidbody, float> queryPointer;

    public Raycaster()
    {
      this.queryPointer = new Func<b2RayCastInput, SpeculativeRigidbody, float>(this.RaycastAtRigidbodiesQuery);
    }

    public void SetAll(
      PhysicsEngine physicsEngine,
      DungeonData dungeonData,
      Position origin,
      Vector2 direction,
      float dist,
      bool collideWithTiles,
      bool collideWithRigidbodies,
      int rayMask,
      CollisionLayer? sourceLayer,
      bool collideWithTriggers,
      Func<SpeculativeRigidbody, bool> rigidbodyExcluder,
      ICollection<SpeculativeRigidbody> ignoreList)
    {
      this.physicsEngine = physicsEngine;
      this.dungeonData = dungeonData;
      this.origin = origin;
      this.direction = direction;
      this.dist = dist;
      this.collideWithTiles = collideWithTiles;
      this.collideWithRigidbodies = collideWithRigidbodies;
      this.rayMask = rayMask;
      this.sourceLayer = sourceLayer;
      this.collideWithTriggers = collideWithTriggers;
      this.rigidbodyExcluder = rigidbodyExcluder;
      this.ignoreList = ignoreList;
    }

    public void Clear()
    {
      this.physicsEngine = (PhysicsEngine) null;
      this.rigidbodyExcluder = (Func<SpeculativeRigidbody, bool>) null;
      this.ignoreList = (ICollection<SpeculativeRigidbody>) null;
    }

    public bool DoRaycast(out RaycastResult result)
    {
      result = (RaycastResult) null;
      this.direction.Normalize();
      if (this.collideWithTiles && (bool) (UnityEngine.Object) this.physicsEngine.TileMap)
      {
        string str = "Collision Layer";
        int tileMapLayerByName = BraveUtility.GetTileMapLayerByName(str, this.physicsEngine.TileMap);
        IntVector2 pixel1 = PhysicsEngine.UnitToPixel(this.origin.UnitPosition);
        IntVector2 pixel2 = PhysicsEngine.UnitToPixel(this.direction.normalized * this.dist);
        IntVector2 zero = IntVector2.Zero;
        IntVector2 pos1 = pixel1 / this.physicsEngine.PixelsPerUnit;
        RaycastResult raycastResult1 = this.RaycastAtTile(pos1, tileMapLayerByName, str, this.rayMask, this.sourceLayer, this.origin, this.direction, this.dist, this.dungeonData);
        if (raycastResult1 != null && (result == null || (double) raycastResult1.Distance < (double) result.Distance))
        {
          RaycastResult.Pool.Free(ref result);
          result = raycastResult1;
        }
        else
          RaycastResult.Pool.Free(ref raycastResult1);
        IntVector2 intVector2_1 = pos1;
        while (zero.x != pixel2.x || zero.y != pixel2.y)
        {
          IntVector2 intVector2_2 = zero.x != pixel2.x ? (zero.y != pixel2.y ? ((double) Mathf.Abs((float) zero.x / (float) pixel2.x) >= (double) Mathf.Abs((float) zero.y / (float) pixel2.y) ? new IntVector2(0, Math.Sign(pixel2.y)) : new IntVector2(Math.Sign(pixel2.x), 0)) : new IntVector2(Math.Sign(pixel2.x), 0)) : new IntVector2(0, Math.Sign(pixel2.y));
          zero += intVector2_2;
          IntVector2 pos2 = (pixel1 + zero) / this.physicsEngine.PixelsPerUnit;
          if (pos2 != intVector2_1)
          {
            RaycastResult raycastResult2 = this.RaycastAtTile(pos2, tileMapLayerByName, str, this.rayMask, this.sourceLayer, this.origin, this.direction, this.dist, this.dungeonData);
            if (raycastResult2 != null && (result == null || (double) raycastResult2.Distance < (double) result.Distance))
            {
              if (raycastResult2.OtherPixelCollider.NormalModifier == null)
                raycastResult2.Normal = -intVector2_2.ToVector2().normalized;
              RaycastResult.Pool.Free(ref result);
              result = raycastResult2;
            }
            else
              RaycastResult.Pool.Free(ref raycastResult2);
            intVector2_1 = pos2;
          }
        }
      }
      if (this.collideWithRigidbodies)
      {
        this.nearestRigidbodyHit = (RaycastResult) null;
        this.p1 = this.origin.UnitPosition;
        Vector2 vector2 = this.p1 + this.direction * this.dist;
        this.p1p2Dist = Vector2.Distance(this.p1, vector2);
        this.physicsEngine.m_rigidbodyTree.RayCast(new b2RayCastInput(this.p1, vector2), this.queryPointer);
        if (this.physicsEngine.CollidesWithProjectiles(this.rayMask, this.sourceLayer))
          this.physicsEngine.m_projectileTree.RayCast(new b2RayCastInput(this.p1, vector2), this.queryPointer);
        if (this.nearestRigidbodyHit != null)
        {
          if (result == null || (double) this.nearestRigidbodyHit.Distance < (double) result.Distance)
          {
            RaycastResult.Pool.Free(ref result);
            result = this.nearestRigidbodyHit;
          }
          else
            RaycastResult.Pool.Free(ref this.nearestRigidbodyHit);
        }
      }
      return result != null;
    }

    private RaycastResult RaycastAtTile(
      IntVector2 pos,
      int layer,
      string layerName,
      int rayMask,
      CollisionLayer? sourceLayer,
      Position origin,
      Vector2 direction,
      float dist,
      DungeonData dungeonData)
    {
      PhysicsEngine.Tile tile = this.physicsEngine.GetTile(pos.x, pos.y, this.physicsEngine.TileMap, layer, layerName, dungeonData);
      RaycastResult raycastResult = (RaycastResult) null;
      if (tile == null || tile.PixelColliders == null || tile.PixelColliders.Count == 0)
        return (RaycastResult) null;
      for (int index = 0; index < tile.PixelColliders.Count; ++index)
      {
        PixelCollider pixelCollider = tile.PixelColliders[index];
        if (pixelCollider.CanCollideWith(rayMask, sourceLayer))
        {
          RaycastResult result;
          if (pixelCollider.Raycast(origin.UnitPosition, direction, dist, out result))
          {
            if (raycastResult == null || (double) result.Distance < (double) raycastResult.Distance)
            {
              RaycastResult.Pool.Free(ref raycastResult);
              raycastResult = result;
            }
            else
              RaycastResult.Pool.Free(ref result);
          }
          else
            RaycastResult.Pool.Free(ref result);
        }
      }
      return raycastResult;
    }

    private float RaycastAtRigidbodiesQuery(
      b2RayCastInput rayCastInput,
      SpeculativeRigidbody rigidbody)
    {
      float num = rayCastInput.maxFraction;
      if ((bool) (UnityEngine.Object) rigidbody && rigidbody.enabled && rigidbody.CollideWithOthers && (this.ignoreList == null || !this.ignoreList.Contains(rigidbody)) && (this.rigidbodyExcluder == null || !this.rigidbodyExcluder(rigidbody)))
      {
        for (int index = 0; index < rigidbody.PixelColliders.Count; ++index)
        {
          PixelCollider pixelCollider = rigidbody.PixelColliders[index];
          if ((this.collideWithTriggers || !pixelCollider.IsTrigger) && pixelCollider.CanCollideWith(this.rayMask, this.sourceLayer))
          {
            RaycastResult result;
            if (pixelCollider.Raycast(this.origin.UnitPosition, this.direction, this.dist, out result))
            {
              if (this.nearestRigidbodyHit == null || (double) result.Distance < (double) this.nearestRigidbodyHit.Distance)
              {
                RaycastResult.Pool.Free(ref this.nearestRigidbodyHit);
                this.nearestRigidbodyHit = result;
                this.nearestRigidbodyHit.SpeculativeRigidbody = rigidbody;
                num = Vector2.Distance(this.p1, this.nearestRigidbodyHit.Contact) / this.p1p2Dist;
              }
              else
                RaycastResult.Pool.Free(ref result);
            }
            else
              RaycastResult.Pool.Free(ref result);
          }
        }
      }
      return num;
    }
  }

  public enum PointCollisionState
  {
    Clean,
    HitBeforeNext,
    Hit,
  }

  private class RigidbodyCaster
  {
    private PhysicsEngine physicsEngine;
    private DungeonData dungeonData;
    private SpeculativeRigidbody rigidbody;
    private IntVector2 pixelsToMove;
    private bool collideWithTiles;
    private bool collideWithRigidbodies;
    private int? overrideCollisionMask;
    private bool collideWithTriggers;
    private SpeculativeRigidbody[] ignoreList;
    private CollisionData tempResult;
    private List<PixelCollider.StepData> stepList;
    private Func<SpeculativeRigidbody, bool> callbackPointer;

    public RigidbodyCaster()
    {
      this.callbackPointer = new Func<SpeculativeRigidbody, bool>(this.RigidbodyCollisionCallback);
    }

    public void SetAll(
      PhysicsEngine physicsEngine,
      DungeonData dungeonData,
      SpeculativeRigidbody rigidbody,
      IntVector2 pixelsToMove,
      bool collideWithTiles,
      bool collideWithRigidbodies,
      int? overrideCollisionMask,
      bool collideWithTriggers,
      SpeculativeRigidbody[] ignoreList)
    {
      this.physicsEngine = physicsEngine;
      this.dungeonData = dungeonData;
      this.rigidbody = rigidbody;
      this.pixelsToMove = pixelsToMove;
      this.collideWithTiles = collideWithTiles;
      this.collideWithRigidbodies = collideWithRigidbodies;
      this.overrideCollisionMask = overrideCollisionMask;
      this.collideWithTriggers = collideWithTriggers;
      this.ignoreList = ignoreList;
    }

    public void Clear()
    {
      this.physicsEngine = (PhysicsEngine) null;
      this.rigidbody = (SpeculativeRigidbody) null;
      this.ignoreList = (SpeculativeRigidbody[]) null;
    }

    public bool DoRigidbodyCast(out CollisionData result)
    {
      this.tempResult = (CollisionData) null;
      if (!(bool) (UnityEngine.Object) this.rigidbody || this.rigidbody.PixelColliders.Count == 0)
      {
        result = (CollisionData) null;
        return false;
      }
      this.stepList = PixelCollider.m_stepList;
      PhysicsEngine.PixelMovementGenerator(this.pixelsToMove, this.stepList);
      IntVector2 lhs1 = IntVector2.MaxValue;
      IntVector2 lhs2 = IntVector2.MinValue;
      for (int index = 0; index < this.rigidbody.PixelColliders.Count; ++index)
      {
        PixelCollider pixelCollider = this.rigidbody.PixelColliders[index];
        lhs1 = IntVector2.Min(lhs1, pixelCollider.Min);
        lhs2 = IntVector2.Max(lhs2, pixelCollider.Max);
      }
      IntVector2 lowerBounds = IntVector2.Min(lhs1, lhs1 + this.pixelsToMove);
      IntVector2 upperBounds = IntVector2.Max(lhs2, lhs2 + this.pixelsToMove);
      if (this.collideWithTiles && (bool) (UnityEngine.Object) this.physicsEngine.TileMap)
      {
        this.physicsEngine.InitNearbyTileCheck(PhysicsEngine.PixelToUnit(lowerBounds - IntVector2.One), PhysicsEngine.PixelToUnit(upperBounds + IntVector2.One), this.physicsEngine.TileMap);
        for (PhysicsEngine.Tile nextNearbyTile = this.physicsEngine.GetNextNearbyTile(this.dungeonData); nextNearbyTile != null; nextNearbyTile = this.physicsEngine.GetNextNearbyTile(this.dungeonData))
        {
          for (int index1 = 0; index1 < this.rigidbody.PixelColliders.Count; ++index1)
          {
            PixelCollider pixelCollider1 = this.rigidbody.PixelColliders[index1];
            if (this.collideWithTriggers || !pixelCollider1.IsTrigger)
            {
              for (int index2 = 0; index2 < nextNearbyTile.PixelColliders.Count; ++index2)
              {
                PixelCollider pixelCollider2 = nextNearbyTile.PixelColliders[index2];
                LinearCastResult result1;
                if (pixelCollider1.CanCollideWith(pixelCollider2) && pixelCollider1.AABBOverlaps(pixelCollider2, this.pixelsToMove) && pixelCollider1.LinearCast(pixelCollider2, this.rigidbody.PixelsToMove, this.stepList, out result1))
                {
                  if (this.tempResult == null || (double) result1.TimeUsed < (double) this.tempResult.TimeUsed)
                  {
                    if (this.tempResult == null)
                      this.tempResult = CollisionData.Pool.Allocate();
                    this.tempResult.SetAll(result1);
                    this.tempResult.collisionType = CollisionData.CollisionType.TileMap;
                    this.tempResult.MyRigidbody = this.rigidbody;
                    this.tempResult.TileLayerName = "Collision Layer";
                    this.tempResult.TilePosition = nextNearbyTile.Position;
                  }
                  LinearCastResult.Pool.Free(ref result1);
                }
              }
            }
          }
        }
      }
      if (this.collideWithRigidbodies)
      {
        this.physicsEngine.m_rigidbodyTree.Query(PhysicsEngine.GetSafeB2AABB(lowerBounds, upperBounds), this.callbackPointer);
        if (this.overrideCollisionMask.HasValue)
        {
          if ((this.overrideCollisionMask.Value & this.physicsEngine.m_cachedProjectileMask) == this.physicsEngine.m_cachedProjectileMask)
            this.physicsEngine.m_projectileTree.Query(PhysicsEngine.GetSafeB2AABB(lowerBounds, upperBounds), this.callbackPointer);
        }
        else if (this.physicsEngine.CollidesWithProjectiles(this.rigidbody))
          this.physicsEngine.m_projectileTree.Query(PhysicsEngine.GetSafeB2AABB(lowerBounds, upperBounds), this.callbackPointer);
      }
      result = this.tempResult;
      return result != null;
    }

    private bool RigidbodyCollisionCallback(SpeculativeRigidbody otherRigidbody)
    {
      if ((bool) (UnityEngine.Object) otherRigidbody && (UnityEngine.Object) otherRigidbody != (UnityEngine.Object) this.rigidbody && otherRigidbody.enabled && otherRigidbody.CollideWithOthers && Array.IndexOf<SpeculativeRigidbody>(this.ignoreList, otherRigidbody) < 0)
      {
        for (int index1 = 0; index1 < this.rigidbody.PixelColliders.Count; ++index1)
        {
          PixelCollider pixelCollider1 = this.rigidbody.PixelColliders[index1];
          if (this.collideWithTriggers || !pixelCollider1.IsTrigger)
          {
            for (int index2 = 0; index2 < otherRigidbody.PixelColliders.Count; ++index2)
            {
              PixelCollider pixelCollider2 = otherRigidbody.PixelColliders[index2];
              LinearCastResult result;
              if ((this.collideWithTriggers || !pixelCollider2.IsTrigger) && (!this.overrideCollisionMask.HasValue ? pixelCollider1.CanCollideWith(pixelCollider2) : pixelCollider2.CanCollideWith(this.overrideCollisionMask.Value)) && pixelCollider1.AABBOverlaps(pixelCollider2, this.pixelsToMove) && pixelCollider1.LinearCast(pixelCollider2, this.rigidbody.PixelsToMove, this.stepList, out result))
              {
                if (this.tempResult == null || (double) result.TimeUsed < (double) this.tempResult.TimeUsed)
                {
                  if (this.tempResult == null)
                    this.tempResult = CollisionData.Pool.Allocate();
                  this.tempResult.SetAll(result);
                  this.tempResult.collisionType = CollisionData.CollisionType.Rigidbody;
                  this.tempResult.MyRigidbody = this.rigidbody;
                  this.tempResult.OtherRigidbody = otherRigidbody;
                }
                LinearCastResult.Pool.Free(ref result);
              }
            }
          }
        }
      }
      return true;
    }
  }

  private struct NearbyTileData
  {
    public tk2dTileMap tileMap;
    public int layer;
    public string layerName;
    private PhysicsEngine.NearbyTileData.Type type;
    private int minPlotX;
    private int minPlotY;
    private int maxPlotX;
    private int maxPlotY;
    private int baseX;
    private int baseY;
    private int width;
    private int i;
    private int imax;
    private bool finished;
    private int x;
    private int y;
    private int extentsX;
    private int extentsY;
    private int endX;
    private int endY;
    private int deltaX;
    private int deltaY;
    private int xStep;
    private int yStep;
    private float deltaError;
    private float error;
    private static List<PhysicsEngine.Tile> m_tiles = new List<PhysicsEngine.Tile>();

    public void Init(float worldMinX, float worldMinY, float worldMaxX, float worldMaxY)
    {
      this.type = PhysicsEngine.NearbyTileData.Type.FullRect;
      this.baseX = (int) worldMinX;
      this.baseY = (int) worldMinY;
      this.width = (int) worldMaxX - this.baseX + 1;
      int num = (int) worldMaxY - this.baseY + 1;
      this.i = 0;
      this.imax = this.width * num;
    }

    public void Init(
      float worldMinX,
      float worldMinY,
      float worldMaxX,
      float worldMaxY,
      IntVector2 pixelColliderDimensions,
      float positionX,
      float positionY,
      IntVector2 pixelsToMove,
      DungeonData dungeonData)
    {
      this.finished = false;
      this.minPlotX = (int) worldMinX;
      this.minPlotY = (int) worldMinY;
      this.maxPlotX = (int) worldMaxX;
      this.maxPlotY = (int) worldMaxY;
      if (((int) worldMaxX - (int) worldMinX + 1) * ((int) worldMaxY - (int) worldMinY + 1) <= 6)
      {
        this.type = PhysicsEngine.NearbyTileData.Type.FullRectPrecalc;
        this.GetAllNearbyTiles(worldMinX, worldMinY, worldMaxX, worldMaxY, dungeonData);
      }
      else
      {
        float num1 = (float) ((double) pixelColliderDimensions.x * (1.0 / 16.0) * 0.5);
        float num2 = (float) ((double) pixelColliderDimensions.y * (1.0 / 16.0) * 0.5);
        float num3 = positionX + num1;
        float num4 = positionY + num2;
        this.x = (int) num3;
        this.y = (int) num4;
        this.endX = (int) ((double) num3 + (double) pixelsToMove.x * (1.0 / 16.0));
        this.endY = (int) ((double) num4 + (double) pixelsToMove.y * (1.0 / 16.0));
        this.deltaX = this.endX - this.x;
        this.deltaY = this.endY - this.y;
        this.extentsX = Mathf.CeilToInt(num1 + 0.25f);
        this.extentsY = Mathf.CeilToInt(num2 + 0.25f);
        if (this.deltaX == 0)
        {
          this.type = PhysicsEngine.NearbyTileData.Type.BresenhamVertical;
          this.yStep = (int) Mathf.Sign((float) this.deltaY);
          for (int index1 = -this.extentsY; index1 < 0; ++index1)
          {
            for (int index2 = -this.extentsX; index2 <= this.extentsX; ++index2)
              this.Plot(this.x + index2, this.y + this.yStep * index1, Color.blue, dungeonData);
          }
        }
        else if (this.deltaY == 0)
        {
          this.type = PhysicsEngine.NearbyTileData.Type.BresenhamHorizontal;
          this.xStep = (int) Mathf.Sign((float) this.deltaX);
          for (int index3 = -this.extentsX; index3 < 0; ++index3)
          {
            for (int index4 = -this.extentsY; index4 <= this.extentsY; ++index4)
              this.Plot(this.x + this.xStep * index3, this.y + index4, Color.blue, dungeonData);
          }
        }
        else if (Mathf.Abs(this.deltaX) >= Mathf.Abs(this.deltaY))
        {
          this.type = PhysicsEngine.NearbyTileData.Type.BresenhamShallow;
          this.xStep = (int) Mathf.Sign((float) this.deltaX);
          this.yStep = (int) Mathf.Sign((float) this.deltaY);
          for (int index5 = -this.extentsX; index5 < 0; ++index5)
          {
            for (int index6 = -this.extentsY; index6 <= this.extentsY; ++index6)
              this.Plot(this.x + this.xStep * index5, this.y + index6, Color.blue, dungeonData);
          }
          this.deltaError = Mathf.Abs((float) this.deltaY / (float) this.deltaX);
          this.error = 0.0f;
        }
        else
        {
          this.type = PhysicsEngine.NearbyTileData.Type.BresenhamSteep;
          this.xStep = (int) Mathf.Sign((float) this.deltaX);
          this.yStep = (int) Mathf.Sign((float) this.deltaY);
          for (int index7 = -this.extentsY; index7 < 0; ++index7)
          {
            for (int index8 = -this.extentsX; index8 <= this.extentsX; ++index8)
              this.Plot(this.x + index8, this.y + this.yStep * index7, Color.blue, dungeonData);
          }
          this.deltaError = Mathf.Abs((float) this.deltaX / (float) this.deltaY);
          this.error = 0.0f;
        }
      }
    }

    private void Plot(int x, int y, Color color, DungeonData dungeonData, bool core = false)
    {
      CellData cellData;
      if (x < 0 || x < this.minPlotX || x >= PhysicsEngine.m_instance.m_cachedDungeonWidth || y < 0 || y >= PhysicsEngine.m_instance.m_cachedDungeonHeight || x < this.minPlotX || x > this.maxPlotX || y < this.minPlotY || y > this.maxPlotY || (cellData = dungeonData.cellData[x][y]) == null || cellData.HasCachedPhysicsTile && cellData.CachedPhysicsTile == null)
        return;
      if (cellData.HasCachedPhysicsTile)
      {
        PhysicsEngine.NearbyTileData.m_tiles.Add(cellData.CachedPhysicsTile);
      }
      else
      {
        PhysicsEngine.Tile tile = PhysicsEngine.Instance.GetTile(x, y, this.tileMap, this.layer, this.layerName, dungeonData);
        if (tile == null)
          return;
        PhysicsEngine.NearbyTileData.m_tiles.Add(tile);
      }
    }

    public void Finish(DungeonData dungeonData, bool preMove = false)
    {
      if (this.finished)
        return;
      int x = this.x;
      int y = this.y;
      switch (this.type)
      {
        case PhysicsEngine.NearbyTileData.Type.BresenhamVertical:
          if (!preMove)
            y -= this.yStep;
          for (int index1 = 1; index1 <= this.extentsY; ++index1)
          {
            for (int index2 = -this.extentsX; index2 <= this.extentsX; ++index2)
              this.Plot(x + index2, y + this.yStep * index1, Color.blue, dungeonData);
          }
          break;
        case PhysicsEngine.NearbyTileData.Type.BresenhamHorizontal:
          if (!preMove)
            x -= this.xStep;
          for (int index3 = 1; index3 <= this.extentsX; ++index3)
          {
            for (int index4 = -this.extentsY; index4 <= this.extentsY; ++index4)
              this.Plot(x + this.xStep * index3, y + index4, Color.blue, dungeonData);
          }
          break;
        case PhysicsEngine.NearbyTileData.Type.BresenhamShallow:
          if (!preMove)
            x -= this.xStep;
          for (int index5 = 1; index5 <= this.extentsX; ++index5)
          {
            for (int index6 = -this.extentsY; index6 <= this.extentsY; ++index6)
              this.Plot(x + this.xStep * index5, y + index6, Color.blue, dungeonData);
          }
          break;
        case PhysicsEngine.NearbyTileData.Type.BresenhamSteep:
          if (!preMove)
            y -= this.yStep;
          for (int index7 = 1; index7 <= this.extentsY; ++index7)
          {
            for (int index8 = -this.extentsX; index8 <= this.extentsX; ++index8)
              this.Plot(x + index8, y + this.yStep * index7, Color.blue, dungeonData);
          }
          break;
      }
      this.finished = true;
    }

    public PhysicsEngine.Tile GetNextNearbyTile(DungeonData dungeonData)
    {
      switch (this.type)
      {
        case PhysicsEngine.NearbyTileData.Type.FullRect:
          while (this.i < this.imax)
          {
            PhysicsEngine.Tile tile = PhysicsEngine.Instance.GetTile(this.baseX + this.i % this.width, this.baseY + this.i / this.width, this.tileMap, this.layer, this.layerName, dungeonData);
            ++this.i;
            if (tile != null)
              return tile;
          }
          return (PhysicsEngine.Tile) null;
        case PhysicsEngine.NearbyTileData.Type.FullRectPrecalc:
          if (PhysicsEngine.NearbyTileData.m_tiles.Count <= 0)
            return (PhysicsEngine.Tile) null;
          int index1 = PhysicsEngine.NearbyTileData.m_tiles.Count - 1;
          PhysicsEngine.Tile tile1 = PhysicsEngine.NearbyTileData.m_tiles[index1];
          PhysicsEngine.NearbyTileData.m_tiles.RemoveAt(index1);
          return tile1;
        case PhysicsEngine.NearbyTileData.Type.BresenhamVertical:
          while (!this.finished && PhysicsEngine.NearbyTileData.m_tiles.Count == 0)
          {
            for (int index2 = -this.extentsX; index2 <= this.extentsX; ++index2)
              this.Plot(this.x + index2, this.y, Color.yellow, dungeonData, index2 == 0);
            if (this.y == this.endY)
              this.Finish(dungeonData, true);
            this.y += this.yStep;
          }
          if (PhysicsEngine.NearbyTileData.m_tiles.Count <= 0)
            return (PhysicsEngine.Tile) null;
          int index3 = PhysicsEngine.NearbyTileData.m_tiles.Count - 1;
          PhysicsEngine.Tile tile2 = PhysicsEngine.NearbyTileData.m_tiles[index3];
          PhysicsEngine.NearbyTileData.m_tiles.RemoveAt(index3);
          return tile2;
        case PhysicsEngine.NearbyTileData.Type.BresenhamHorizontal:
          while (!this.finished && PhysicsEngine.NearbyTileData.m_tiles.Count == 0)
          {
            for (int index4 = -this.extentsY; index4 <= this.extentsY; ++index4)
              this.Plot(this.x, this.y + index4, Color.yellow, dungeonData, index4 == 0);
            if (this.x == this.endX)
              this.Finish(dungeonData, true);
            this.x += this.xStep;
          }
          if (PhysicsEngine.NearbyTileData.m_tiles.Count <= 0)
            return (PhysicsEngine.Tile) null;
          int index5 = PhysicsEngine.NearbyTileData.m_tiles.Count - 1;
          PhysicsEngine.Tile tile3 = PhysicsEngine.NearbyTileData.m_tiles[index5];
          PhysicsEngine.NearbyTileData.m_tiles.RemoveAt(index5);
          return tile3;
        case PhysicsEngine.NearbyTileData.Type.BresenhamShallow:
          while (!this.finished && PhysicsEngine.NearbyTileData.m_tiles.Count == 0)
          {
            if ((double) this.error >= 0.5)
            {
              for (int index6 = 1; index6 <= this.extentsX; ++index6)
                this.Plot(this.x - this.xStep + this.xStep * index6, this.y - this.yStep * this.extentsY, Color.blue, dungeonData);
              this.y += this.yStep;
              --this.error;
              for (int index7 = -1; index7 >= -this.extentsX; --index7)
                this.Plot(this.x + this.xStep * index7, this.y + this.yStep * this.extentsY, Color.blue, dungeonData);
            }
            for (int index8 = -this.extentsY; index8 <= this.extentsY; ++index8)
              this.Plot(this.x, this.y + index8, Color.green, dungeonData, index8 == 0);
            this.error += this.deltaError;
            if (this.x == this.endX)
              this.Finish(dungeonData, true);
            this.x += this.xStep;
          }
          if (PhysicsEngine.NearbyTileData.m_tiles.Count <= 0)
            return (PhysicsEngine.Tile) null;
          int index9 = PhysicsEngine.NearbyTileData.m_tiles.Count - 1;
          PhysicsEngine.Tile tile4 = PhysicsEngine.NearbyTileData.m_tiles[index9];
          PhysicsEngine.NearbyTileData.m_tiles.RemoveAt(index9);
          return tile4;
        case PhysicsEngine.NearbyTileData.Type.BresenhamSteep:
          while (!this.finished && PhysicsEngine.NearbyTileData.m_tiles.Count == 0)
          {
            if ((double) this.error >= 0.5)
            {
              for (int index10 = 1; index10 <= this.extentsY; ++index10)
                this.Plot(this.x - this.xStep * this.extentsX, this.y - this.yStep + this.yStep * index10, Color.blue, dungeonData);
              this.x += this.xStep;
              --this.error;
              for (int index11 = -1; index11 >= -this.extentsY; --index11)
                this.Plot(this.x + this.xStep * this.extentsX, this.y + this.yStep * index11, Color.blue, dungeonData);
            }
            for (int index12 = -this.extentsX; index12 <= this.extentsX; ++index12)
              this.Plot(this.x + index12, this.y, Color.green, dungeonData, index12 == 0);
            this.error += this.deltaError;
            if (this.y == this.endY)
              this.Finish(dungeonData, true);
            this.y += this.yStep;
          }
          if (PhysicsEngine.NearbyTileData.m_tiles.Count <= 0)
            return (PhysicsEngine.Tile) null;
          int index13 = PhysicsEngine.NearbyTileData.m_tiles.Count - 1;
          PhysicsEngine.Tile tile5 = PhysicsEngine.NearbyTileData.m_tiles[index13];
          PhysicsEngine.NearbyTileData.m_tiles.RemoveAt(index13);
          return tile5;
        default:
          return (PhysicsEngine.Tile) null;
      }
    }

    private void GetAllNearbyTiles(
      float worldMinX,
      float worldMinY,
      float worldMaxX,
      float worldMaxY,
      DungeonData dungeonData)
    {
      this.baseX = (int) worldMinX;
      this.baseY = (int) worldMinY;
      this.width = (int) worldMaxX - this.baseX + 1;
      this.imax = this.width * ((int) worldMaxY - this.baseY + 1);
      for (this.i = 0; this.i < this.imax; ++this.i)
      {
        int x = this.baseX + this.i % this.width;
        int y = this.baseY + this.i / this.width;
        CellData cellData;
        if (x >= 0 && x < PhysicsEngine.m_instance.m_cachedDungeonWidth && y >= 0 && y < PhysicsEngine.m_instance.m_cachedDungeonHeight && (cellData = dungeonData.cellData[x][y]) != null && (!cellData.HasCachedPhysicsTile || cellData.CachedPhysicsTile != null))
        {
          if (cellData.HasCachedPhysicsTile)
          {
            PhysicsEngine.NearbyTileData.m_tiles.Add(cellData.CachedPhysicsTile);
          }
          else
          {
            PhysicsEngine.Tile tile = PhysicsEngine.Instance.GetTile(x, y, this.tileMap, this.layer, this.layerName, dungeonData);
            if (tile != null)
              PhysicsEngine.NearbyTileData.m_tiles.Add(tile);
          }
        }
      }
    }

    public void Cleanup() => PhysicsEngine.NearbyTileData.m_tiles.Clear();

    private enum Type
    {
      FullRect,
      FullRectPrecalc,
      BresenhamVertical,
      BresenhamHorizontal,
      BresenhamShallow,
      BresenhamSteep,
    }
  }

  public class Tile : ICollidableObject
  {
    public int X;
    public int Y;
    public string LayerName;
    public List<PixelCollider> PixelColliders = new List<PixelCollider>();

    public Tile()
    {
    }

    public Tile(List<PixelCollider> pixelColliders, int x, int y, string layerName)
    {
      this.PixelColliders = pixelColliders;
      this.X = x;
      this.Y = y;
      this.LayerName = layerName;
    }

    public IntVector2 Position => new IntVector2(this.X, this.Y);

    public PixelCollider PrimaryPixelCollider
    {
      get
      {
        if (this.PixelColliders == null || this.PixelColliders.Count == 0)
          return (PixelCollider) null;
        return this.PixelColliders[0].CollisionLayer == CollisionLayer.EnemyBlocker ? (PixelCollider) null : this.PixelColliders[0];
      }
    }

    public bool CanCollideWith(SpeculativeRigidbody rigidbody) => true;

    public List<PixelCollider> GetPixelColliders() => this.PixelColliders;

    public override bool Equals(object obj) => base.Equals(obj);

    public override int GetHashCode()
    {
      return this.LayerName.GetHashCode() & this.X.GetHashCode() & this.Y.GetHashCode();
    }
  }
}
