// Decompiled with JetBrains decompiler
// Type: SpeculativeRigidbody
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using BraveDynamicTree;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

public class SpeculativeRigidbody : BraveBehaviour, ICollidableObject, ILevelLoadedListener
  {
    public bool CollideWithTileMap = true;
    public bool CollideWithOthers = true;
    public Vector2 Velocity = new Vector2(0.0f, 0.0f);
    public bool CapVelocity;
    [ShowInInspectorIf("CapVelocity", false)]
    public Vector2 MaxVelocity;
    public bool ForceAlwaysUpdate;
    public bool CanPush;
    public bool CanBePushed;
    [ShowInInspectorIf("CanPush", false)]
    public float PushSpeedModifier = 1f;
    public bool CanCarry;
    public bool CanBeCarried = true;
    [NonSerialized]
    public bool ForceCarriesRigidbodies;
    public bool PreventPiercing;
    public bool SkipEmptyColliders;
    [HideInInspector]
    public tk2dBaseSprite TK2DSprite;
    public Action<SpeculativeRigidbody> OnPreMovement;
    public SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate OnPreRigidbodyCollision;
    public SpeculativeRigidbody.OnPreTileCollisionDelegate OnPreTileCollision;
    public Action<CollisionData> OnCollision;
    public SpeculativeRigidbody.OnRigidbodyCollisionDelegate OnRigidbodyCollision;
    public SpeculativeRigidbody.OnBeamCollisionDelegate OnBeamCollision;
    public SpeculativeRigidbody.OnTileCollisionDelegate OnTileCollision;
    public SpeculativeRigidbody.OnTriggerDelegate OnEnterTrigger;
    public SpeculativeRigidbody.OnTriggerDelegate OnTriggerCollision;
    public SpeculativeRigidbody.OnTriggerExitDelegate OnExitTrigger;
    public System.Action OnPathTargetReached;
    public Action<SpeculativeRigidbody, Vector2, IntVector2> OnPostRigidbodyMovement;
    public SpeculativeRigidbody.MovementRestrictorDelegate MovementRestrictor;
    public Action<BasicBeamController> OnHitByBeam;
    [NonSerialized]
    public bool RegenerateColliders;
    public bool RecheckTriggers;
    public bool UpdateCollidersOnRotation;
    public bool UpdateCollidersOnScale;
    [HideInInspector]
    public Vector2 AxialScale = Vector2.one;
    public SpeculativeRigidbody.DebugSettings DebugParams = new SpeculativeRigidbody.DebugSettings();
    [HideInInspector]
    public bool IgnorePixelGrid;
    [HideInInspector]
    public List<PixelCollider> PixelColliders;
    [NonSerialized]
    public int SortHash = -1;
    [NonSerialized]
    public int proxyId = -1;
    [NonSerialized]
    public SpeculativeRigidbody.RegistrationState PhysicsRegistration = SpeculativeRigidbody.RegistrationState.Deregistered;
    public Func<Vector2, Vector2, Vector2> ReflectProjectilesNormalGenerator;
    public Func<Vector2, Vector2, Vector2> ReflectBeamsNormalGenerator;
    private bool? m_cachedIsSimpleProjectile;
    [NonSerialized]
    public bool PathMode;
    [NonSerialized]
    public IntVector2 PathTarget;
    [NonSerialized]
    public float PathSpeed;
    [NonSerialized]
    public LinkedList<Vector3> PreviousPositions = new LinkedList<Vector3>();
    [NonSerialized]
    public Vector3 LastVelocity;
    [NonSerialized]
    public float LastRotation;
    [NonSerialized]
    public Vector2 LastScale;
    public Position m_position = new Position(0, 0);
    [NonSerialized]
    private List<SpeculativeRigidbody> m_specificCollisionExceptions;
    [NonSerialized]
    public List<SpeculativeRigidbody.TemporaryException> m_temporaryCollisionExceptions;
    [NonSerialized]
    private List<SpeculativeRigidbody> m_ghostCollisionExceptions;
    [NonSerialized]
    public List<SpeculativeRigidbody.PushedRigidbodyData> m_pushedRigidbodies = new List<SpeculativeRigidbody.PushedRigidbodyData>();
    [NonSerialized]
    private List<SpeculativeRigidbody> m_carriedRigidbodies;
    private bool m_initialized;

    public Position Position
    {
      get => this.m_position;
      set
      {
        this.m_position = value;
        this.UpdateColliderPositions();
        PhysicsEngine.UpdatePosition(this);
      }
    }

    public b2AABB b2AABB
    {
      get
      {
        int count = this.PixelColliders.Count;
        if (count == 1)
        {
          PixelCollider pixelCollider = this.PixelColliders[0];
          IntVector2 position = pixelCollider.Position;
          IntVector2 dimensions = pixelCollider.Dimensions;
          return new b2AABB((float) position.x * (1f / 16f), (float) position.y * (1f / 16f), (float) (position.x + dimensions.x - 1) * (1f / 16f), (float) (position.y + dimensions.y - 1) * (1f / 16f));
        }
        if (count > 1)
        {
          PixelCollider pixelCollider1 = this.PixelColliders[0];
          IntVector2 position = pixelCollider1.Position;
          IntVector2 dimensions1 = pixelCollider1.Dimensions;
          float a1 = (float) position.x;
          float a2 = (float) position.y;
          float a3 = (float) (position.x + dimensions1.x - 1);
          float a4 = (float) (position.y + dimensions1.y - 1);
          int index = 1;
          do
          {
            PixelCollider pixelCollider2 = this.PixelColliders[index];
            position = pixelCollider2.Position;
            IntVector2 dimensions2 = pixelCollider2.Dimensions;
            a1 = Mathf.Min(a1, (float) position.x);
            a2 = Mathf.Min(a2, (float) position.y);
            a3 = Mathf.Max(a3, (float) (position.x + dimensions2.x - 1));
            a4 = Mathf.Max(a4, (float) (position.y + dimensions2.y - 1));
            ++index;
          }
          while (index < count);
          return new b2AABB(a1 * (1f / 16f), a2 * (1f / 16f), a3 * (1f / 16f), a4 * (1f / 16f));
        }
        Debug.LogError((object) "Trying to access a b2AABB for a SpecRigidbody with NO COLLIDERS.");
        return new b2AABB(Vector2.zero, Vector2.zero);
      }
    }

    public PixelCollider PrimaryPixelCollider
    {
      get
      {
        return this.PixelColliders == null || this.PixelColliders.Count == 0 ? (PixelCollider) null : this.PixelColliders[0];
      }
    }

    public PixelCollider HitboxPixelCollider
    {
      get
      {
        for (int index = 0; index < this.PixelColliders.Count; ++index)
        {
          if (this.PixelColliders[index].Enabled && (!this.SkipEmptyColliders || this.PixelColliders[index].Height != 0 || this.PixelColliders[index].Width != 0) && (this.PixelColliders[index].CollisionLayer == CollisionLayer.EnemyHitBox || this.PixelColliders[index].CollisionLayer == CollisionLayer.PlayerHitBox))
            return this.PixelColliders[index];
        }
        for (int index = 0; index < this.PixelColliders.Count; ++index)
        {
          if (this.PixelColliders[index].Enabled && (!this.SkipEmptyColliders || this.PixelColliders[index].Height != 0 || this.PixelColliders[index].Width != 0) && (this.PixelColliders[index].CollisionLayer == CollisionLayer.BulletBlocker || this.PixelColliders[index].CollisionLayer == CollisionLayer.BulletBreakable))
            return this.PixelColliders[index];
        }
        for (int index = 0; index < this.PixelColliders.Count; ++index)
        {
          if (this.PixelColliders[index].Enabled && (!this.SkipEmptyColliders || this.PixelColliders[index].Height != 0 || this.PixelColliders[index].Width != 0) && this.PixelColliders[index].CollisionLayer == CollisionLayer.HighObstacle)
            return this.PixelColliders[index];
        }
        for (int index = 0; index < this.PixelColliders.Count; ++index)
        {
          if (this.PixelColliders[index].Enabled && (!this.SkipEmptyColliders || this.PixelColliders[index].Height != 0 || this.PixelColliders[index].Width != 0) && this.PixelColliders[index].CollisionLayer == CollisionLayer.Projectile)
            return this.PixelColliders[index];
        }
        return this.PrimaryPixelCollider;
      }
    }

    public PixelCollider GroundPixelCollider
    {
      get
      {
        for (int index = 0; index < this.PixelColliders.Count; ++index)
        {
          if (this.PixelColliders[index].Enabled && (this.PixelColliders[index].CollisionLayer == CollisionLayer.EnemyCollider || this.PixelColliders[index].CollisionLayer == CollisionLayer.EnemyHitBox))
            return this.PixelColliders[index];
        }
        for (int index = 0; index < this.PixelColliders.Count; ++index)
        {
          if (this.PixelColliders[index].Enabled && this.PixelColliders[index].CollisionLayer == CollisionLayer.PlayerCollider)
            return this.PixelColliders[index];
        }
        return (PixelCollider) null;
      }
    }

    public PixelCollider this[CollisionLayer layer]
    {
      get => this.PixelColliders.Find((Predicate<PixelCollider>) (c => c.CollisionLayer == layer));
    }

    public List<PixelCollider> GetPixelColliders() => this.PixelColliders;

    public void ForceRegenerate(bool? allowRotation = null, bool? allowScale = null)
    {
      if (!allowRotation.HasValue)
        allowRotation = new bool?(this.UpdateCollidersOnRotation);
      if (!allowScale.HasValue)
        allowScale = new bool?(this.UpdateCollidersOnScale);
      for (int index = 0; index < this.PixelColliders.Count; ++index)
        this.PixelColliders[index].Regenerate(this.transform, allowRotation.Value, allowScale.Value);
      this.RegenerateColliders = false;
      PhysicsEngine.Instance.Register(this);
      PhysicsEngine.UpdatePosition(this);
    }

    public Vector2 UnitTopLeft
    {
      get
      {
        PixelCollider primaryPixelCollider = this.PrimaryPixelCollider;
        return new Vector2((float) primaryPixelCollider.Position.x / (float) PhysicsEngine.Instance.PixelsPerUnit, (float) (primaryPixelCollider.Position.y + primaryPixelCollider.Height) / (float) PhysicsEngine.Instance.PixelsPerUnit);
      }
    }

    public Vector2 UnitTopCenter
    {
      get
      {
        PixelCollider primaryPixelCollider = this.PrimaryPixelCollider;
        return new Vector2(((float) primaryPixelCollider.Position.x + (float) primaryPixelCollider.Width / 2f) / (float) PhysicsEngine.Instance.PixelsPerUnit, (float) (primaryPixelCollider.Position.y + primaryPixelCollider.Height) / (float) PhysicsEngine.Instance.PixelsPerUnit);
      }
    }

    public Vector2 UnitTopRight
    {
      get
      {
        PixelCollider primaryPixelCollider = this.PrimaryPixelCollider;
        return new Vector2((float) (primaryPixelCollider.Position.x + primaryPixelCollider.Width) / (float) PhysicsEngine.Instance.PixelsPerUnit, (float) (primaryPixelCollider.Position.y + primaryPixelCollider.Height) / (float) PhysicsEngine.Instance.PixelsPerUnit);
      }
    }

    public Vector2 UnitCenterLeft
    {
      get
      {
        PixelCollider primaryPixelCollider = this.PrimaryPixelCollider;
        return new Vector2((float) primaryPixelCollider.Position.x / (float) PhysicsEngine.Instance.PixelsPerUnit, ((float) primaryPixelCollider.Position.y + (float) primaryPixelCollider.Height / 2f) / (float) PhysicsEngine.Instance.PixelsPerUnit);
      }
    }

    public Vector2 UnitCenter
    {
      get
      {
        PixelCollider primaryPixelCollider = this.PrimaryPixelCollider;
        return new Vector2(((float) primaryPixelCollider.Position.x + (float) primaryPixelCollider.Width / 2f) / (float) PhysicsEngine.Instance.PixelsPerUnit, ((float) primaryPixelCollider.Position.y + (float) primaryPixelCollider.Height / 2f) / (float) PhysicsEngine.Instance.PixelsPerUnit);
      }
    }

    public Vector2 UnitCenterRight
    {
      get
      {
        PixelCollider primaryPixelCollider = this.PrimaryPixelCollider;
        return new Vector2((float) (primaryPixelCollider.Position.x + primaryPixelCollider.Width) / (float) PhysicsEngine.Instance.PixelsPerUnit, ((float) primaryPixelCollider.Position.y + (float) primaryPixelCollider.Height / 2f) / (float) PhysicsEngine.Instance.PixelsPerUnit);
      }
    }

    public Vector2 UnitBottomLeft
    {
      get
      {
        PixelCollider primaryPixelCollider = this.PrimaryPixelCollider;
        return new Vector2((float) primaryPixelCollider.Position.x / (float) PhysicsEngine.Instance.PixelsPerUnit, (float) primaryPixelCollider.Position.y / (float) PhysicsEngine.Instance.PixelsPerUnit);
      }
    }

    public Vector2 UnitBottomCenter
    {
      get
      {
        PixelCollider primaryPixelCollider = this.PrimaryPixelCollider;
        return new Vector2(((float) primaryPixelCollider.Position.x + (float) primaryPixelCollider.Width / 2f) / (float) PhysicsEngine.Instance.PixelsPerUnit, (float) primaryPixelCollider.Position.y / (float) PhysicsEngine.Instance.PixelsPerUnit);
      }
    }

    public Vector2 UnitBottomRight
    {
      get
      {
        PixelCollider primaryPixelCollider = this.PrimaryPixelCollider;
        return new Vector2((float) (primaryPixelCollider.Position.x + primaryPixelCollider.Width) / (float) PhysicsEngine.Instance.PixelsPerUnit, (float) primaryPixelCollider.Position.y / (float) PhysicsEngine.Instance.PixelsPerUnit);
      }
    }

    public Vector2 UnitDimensions
    {
      get
      {
        return this.PrimaryPixelCollider.Dimensions.ToVector2() / (float) PhysicsEngine.Instance.PixelsPerUnit;
      }
    }

    public float UnitLeft
    {
      get => (float) this.PrimaryPixelCollider.MinX / (float) PhysicsEngine.Instance.PixelsPerUnit;
    }

    public float UnitRight
    {
      get
      {
        return (float) (this.PrimaryPixelCollider.MaxX + 1) / (float) PhysicsEngine.Instance.PixelsPerUnit;
      }
    }

    public float UnitBottom
    {
      get => (float) this.PrimaryPixelCollider.MinY / (float) PhysicsEngine.Instance.PixelsPerUnit;
    }

    public float UnitTop
    {
      get
      {
        return (float) (this.PrimaryPixelCollider.MaxY + 1) / (float) PhysicsEngine.Instance.PixelsPerUnit;
      }
    }

    public float UnitWidth
    {
      get
      {
        return (float) this.PrimaryPixelCollider.Dimensions.x / (float) PhysicsEngine.Instance.PixelsPerUnit;
      }
    }

    public float UnitHeight
    {
      get
      {
        return (float) this.PrimaryPixelCollider.Dimensions.y / (float) PhysicsEngine.Instance.PixelsPerUnit;
      }
    }

    public PixelCollider GetPixelCollider(ColliderType preferredCollider)
    {
      PixelCollider pixelCollider = (PixelCollider) null;
      switch (preferredCollider)
      {
        case ColliderType.Ground:
          pixelCollider = this.GroundPixelCollider;
          break;
        case ColliderType.HitBox:
          pixelCollider = this.HitboxPixelCollider;
          break;
      }
      if (pixelCollider == null)
        pixelCollider = this.PrimaryPixelCollider;
      return pixelCollider;
    }

    public Vector2 GetUnitCenter(ColliderType preferredCollider)
    {
      return this.GetPixelCollider(preferredCollider).UnitCenter;
    }

    public bool ReflectProjectiles { get; set; }

    public bool ReflectBeams { get; set; }

    public bool BlockBeams { get; set; }

    public bool IsSimpleProjectile
    {
      get
      {
        if (!this.m_cachedIsSimpleProjectile.HasValue)
        {
          this.m_cachedIsSimpleProjectile = new bool?(this.PixelColliders.Count == 1 && this.PixelColliders[0].CollisionLayer == CollisionLayer.Projectile);
          if ((bool) (UnityEngine.Object) this.projectile)
          {
            SpeculativeRigidbody speculativeRigidbody = this;
            bool? simpleProjectile = speculativeRigidbody.m_cachedIsSimpleProjectile;
            speculativeRigidbody.m_cachedIsSimpleProjectile = !this.projectile.collidesWithProjectiles ? simpleProjectile : new bool?(false);
          }
        }
        return this.m_cachedIsSimpleProjectile.Value;
      }
    }

    public bool HasTriggerCollisions { get; set; }

    public bool HasFrameSpecificCollisionExceptions { get; set; }

    public bool HasUnresolvedTriggerCollisions
    {
      get
      {
        if (this.RecheckTriggers)
          return true;
        for (int index1 = 0; index1 < this.PixelColliders.Count; ++index1)
        {
          for (int index2 = 0; index2 < this.PixelColliders[index1].TriggerCollisions.Count; ++index2)
          {
            if (!this.PixelColliders[index1].TriggerCollisions[index2].Notified)
              return true;
          }
        }
        return false;
      }
    }

    public float TimeRemaining { get; set; }

    public IntVector2 PixelsToMove { get; set; }

    public IntVector2 ImpartedPixelsToMove { get; set; }

    public bool CollidedX { get; set; }

    public bool CollidedY { get; set; }

    public List<SpeculativeRigidbody.PushedRigidbodyData> PushedRigidbodies
    {
      get => this.m_pushedRigidbodies;
    }

    private void Start() => this.Initialize();

    public void Initialize()
    {
      if (this.m_initialized)
        return;
      if ((UnityEngine.Object) this.TK2DSprite == (UnityEngine.Object) null)
        this.TK2DSprite = this.sprite;
      this.m_position.UnitPosition = (Vector2) this.transform.position;
      if (this.UpdateCollidersOnRotation && (UnityEngine.Object) this.transform != (UnityEngine.Object) null)
        this.LastRotation = this.transform.eulerAngles.z;
      this.ForceRegenerate();
      if ((UnityEngine.Object) this.TK2DSprite != (UnityEngine.Object) null)
        this.TK2DSprite.UpdateZDepth();
      if ((UnityEngine.Object) PhysicsEngine.Instance != (UnityEngine.Object) null)
        PhysicsEngine.Instance.Register(this);
      this.m_initialized = true;
    }

    public void Reinitialize()
    {
      if (!this.m_initialized)
      {
        this.Initialize();
      }
      else
      {
        this.m_position.UnitPosition = (Vector2) this.transform.position;
        for (int index = 0; index < this.PixelColliders.Count; ++index)
        {
          PixelCollider pixelCollider = this.PixelColliders[index];
          pixelCollider.Position = this.Position.PixelPosition + pixelCollider.Offset;
        }
        PhysicsEngine.Instance.Register(this);
        PhysicsEngine.UpdatePosition(this);
      }
    }

    protected override void OnDestroy()
    {
      if (PhysicsEngine.HasInstance)
        PhysicsEngine.Instance.Deregister(this);
      base.OnDestroy();
    }

    private void OnDrawGizmos()
    {
      if (this.DebugParams.ShowPosition)
      {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere((Vector3) this.Position.UnitPosition, 0.05f);
        if (this.DebugParams.PositionHistory > 0)
        {
          int num = 0;
          foreach (Vector3 previousPosition in this.PreviousPositions)
          {
            Gizmos.color = Color.Lerp(Color.green, Color.red, (float) num / (float) this.DebugParams.PositionHistory);
            Gizmos.DrawSphere(previousPosition, 0.05f);
            ++num;
          }
        }
      }
      if (!this.DebugParams.ShowVelocity || this.PrimaryPixelCollider == null)
        return;
      if ((double) this.Velocity.magnitude > 0.0)
        this.LastVelocity = (Vector3) this.Velocity;
      if ((double) this.LastVelocity.magnitude <= 0.0)
        return;
      Gizmos.color = Color.white;
      Vector3 from = (Vector3) this.Position.UnitPosition + new Vector3((float) this.PrimaryPixelCollider.Width, (float) this.PrimaryPixelCollider.Height, 0.0f) / (float) (PhysicsEngine.Instance.PixelsPerUnit * 2);
      Gizmos.DrawLine(from, from + this.LastVelocity.normalized);
    }

    public bool ContainsPoint(Vector2 point, int mask = 2147483647 /*0x7FFFFFFF*/, bool collideWithTriggers = false)
    {
      return this.ContainsPixel(PhysicsEngine.UnitToPixel(point), mask, collideWithTriggers);
    }

    public bool ContainsPixel(IntVector2 pixel, int mask = 2147483647 /*0x7FFFFFFF*/, bool collideWithTriggers = false)
    {
      for (int index = 0; index < this.GetPixelColliders().Count; ++index)
      {
        PixelCollider pixelCollider = this.GetPixelColliders()[index];
        if ((collideWithTriggers || !pixelCollider.IsTrigger) && pixelCollider.CanCollideWith(mask) && pixelCollider.ContainsPixel(pixel))
          return true;
      }
      return false;
    }

    public void RegisterSpecificCollisionException(SpeculativeRigidbody specRigidbody)
    {
      if (!(bool) (UnityEngine.Object) specRigidbody)
        return;
      if (this.m_specificCollisionExceptions == null)
        this.m_specificCollisionExceptions = new List<SpeculativeRigidbody>();
      if (this.m_specificCollisionExceptions.Contains(specRigidbody))
        return;
      this.m_specificCollisionExceptions.Add(specRigidbody);
    }

    public bool IsSpecificCollisionException(SpeculativeRigidbody specRigidbody)
    {
      return this.m_specificCollisionExceptions != null && this.m_specificCollisionExceptions.Count != 0 && this.m_specificCollisionExceptions.Contains(specRigidbody);
    }

    public void DeregisterSpecificCollisionException(SpeculativeRigidbody specRigidbody)
    {
      if ((UnityEngine.Object) specRigidbody == (UnityEngine.Object) null || this.m_specificCollisionExceptions == null)
        return;
      this.m_specificCollisionExceptions.Remove(specRigidbody);
    }

    public void ClearSpecificCollisionExceptions()
    {
      if (this.m_specificCollisionExceptions == null)
        return;
      this.m_specificCollisionExceptions.Clear();
    }

    public void ClearFrameSpecificCollisionExceptions()
    {
      for (int index = 0; index < this.PixelColliders.Count; ++index)
        this.PixelColliders[index].ClearFrameSpecificCollisionExceptions();
      this.HasFrameSpecificCollisionExceptions = false;
    }

    public List<SpeculativeRigidbody.TemporaryException> TemporaryCollisionExceptions
    {
      get => this.m_temporaryCollisionExceptions;
    }

    public void RegisterTemporaryCollisionException(
      SpeculativeRigidbody specRigidbody,
      float minTime = 0.01f,
      float? maxTime = null)
    {
      if (!(bool) (UnityEngine.Object) specRigidbody)
        return;
      if (this.m_temporaryCollisionExceptions == null)
        this.m_temporaryCollisionExceptions = new List<SpeculativeRigidbody.TemporaryException>();
      for (int index = 0; index < this.m_temporaryCollisionExceptions.Count; ++index)
      {
        if ((UnityEngine.Object) this.m_temporaryCollisionExceptions[index].SpecRigidbody == (UnityEngine.Object) specRigidbody)
        {
          SpeculativeRigidbody.TemporaryException collisionException = this.m_temporaryCollisionExceptions[index];
          collisionException.MinTimeRemaining = Mathf.Max(collisionException.MinTimeRemaining, minTime);
          if (maxTime.HasValue)
          {
            float? maxTimeRemaining = collisionException.MaxTimeRemaining;
            collisionException.MaxTimeRemaining = maxTimeRemaining.HasValue ? new float?(Math.Min(collisionException.MaxTimeRemaining.Value, maxTime.Value)) : maxTime;
          }
          this.m_temporaryCollisionExceptions[index] = collisionException;
          return;
        }
      }
      this.m_temporaryCollisionExceptions.Add(new SpeculativeRigidbody.TemporaryException(specRigidbody, minTime, maxTime));
    }

    public bool IsTemporaryCollisionException(SpeculativeRigidbody specRigidbody)
    {
      if (this.m_temporaryCollisionExceptions == null || this.m_temporaryCollisionExceptions.Count == 0)
        return false;
      for (int index = 0; index < this.m_temporaryCollisionExceptions.Count; ++index)
      {
        if ((UnityEngine.Object) this.m_temporaryCollisionExceptions[index].SpecRigidbody == (UnityEngine.Object) specRigidbody)
          return true;
      }
      return false;
    }

    public void DeregisterTemporaryCollisionException(SpeculativeRigidbody specRigidbody)
    {
      if (this.m_temporaryCollisionExceptions == null)
        return;
      for (int index = 0; index < this.m_temporaryCollisionExceptions.Count; ++index)
      {
        if ((UnityEngine.Object) this.m_temporaryCollisionExceptions[index].SpecRigidbody == (UnityEngine.Object) specRigidbody)
        {
          this.m_temporaryCollisionExceptions.RemoveAt(index);
          break;
        }
      }
    }

    public List<SpeculativeRigidbody> GhostCollisionExceptions => this.m_ghostCollisionExceptions;

    public void RegisterGhostCollisionException(SpeculativeRigidbody specRigidbody)
    {
      if (!(bool) (UnityEngine.Object) specRigidbody)
        return;
      if (this.m_ghostCollisionExceptions == null)
        this.m_ghostCollisionExceptions = new List<SpeculativeRigidbody>();
      if (this.m_ghostCollisionExceptions.Contains(specRigidbody))
        return;
      this.m_ghostCollisionExceptions.Add(specRigidbody);
    }

    public bool IsGhostCollisionException(SpeculativeRigidbody specRigidbody)
    {
      return (UnityEngine.Object) specRigidbody != (UnityEngine.Object) null && this.m_ghostCollisionExceptions != null && this.m_ghostCollisionExceptions.Contains(specRigidbody);
    }

    public void DeregisterGhostCollisionException(SpeculativeRigidbody specRigidbody)
    {
      if (this.m_ghostCollisionExceptions == null)
        return;
      this.m_ghostCollisionExceptions.Remove(specRigidbody);
    }

    public void DeregisterGhostCollisionException(int index)
    {
      if (this.m_ghostCollisionExceptions == null)
        return;
      this.m_ghostCollisionExceptions.RemoveAt(index);
    }

    public List<SpeculativeRigidbody> CarriedRigidbodies => this.m_carriedRigidbodies;

    public void RegisterCarriedRigidbody(SpeculativeRigidbody specRigidbody)
    {
      if (!(bool) (UnityEngine.Object) specRigidbody)
        return;
      if (this.m_carriedRigidbodies == null)
        this.m_carriedRigidbodies = new List<SpeculativeRigidbody>();
      if (this.m_carriedRigidbodies.Contains(specRigidbody))
        return;
      this.m_carriedRigidbodies.Add(specRigidbody);
    }

    public void DeregisterCarriedRigidbody(SpeculativeRigidbody specRigidbody)
    {
      if (this.m_carriedRigidbodies == null)
        return;
      this.m_carriedRigidbodies.Remove(specRigidbody);
    }

    public void ResetTriggerCollisionData()
    {
      this.HasTriggerCollisions = false;
      for (int index = 0; index < this.PixelColliders.Count; ++index)
      {
        this.PixelColliders[index].ResetTriggerCollisionData();
        if (this.PixelColliders[index].TriggerCollisions.Count > 0)
          this.HasTriggerCollisions = true;
      }
    }

    public void FlagCellsOccupied()
    {
      IntVector2 intVector2_1 = this.UnitBottomLeft.ToIntVector2(VectorConversions.Floor);
      PixelCollider primaryPixelCollider = this.PrimaryPixelCollider;
      IntVector2 intVector2_2 = new Vector2((float) (primaryPixelCollider.Position.x + (primaryPixelCollider.Width - 1)) / (float) PhysicsEngine.Instance.PixelsPerUnit, (float) (primaryPixelCollider.Position.y + (primaryPixelCollider.Height - 1)) / (float) PhysicsEngine.Instance.PixelsPerUnit).ToIntVector2(VectorConversions.Floor);
      for (int x = intVector2_1.x; x <= intVector2_2.x; ++x)
      {
        for (int y = intVector2_1.y; y <= intVector2_2.y; ++y)
          GameManager.Instance.Dungeon.data[new IntVector2(x, y)].isOccupied = true;
      }
    }

    public bool CanCollideWith(SpeculativeRigidbody otherRigidbody)
    {
      return (bool) (UnityEngine.Object) this && !((UnityEngine.Object) this == (UnityEngine.Object) otherRigidbody) && this.enabled && this.CollideWithOthers && (bool) (UnityEngine.Object) otherRigidbody && otherRigidbody.enabled && otherRigidbody.CollideWithOthers && !this.IsSpecificCollisionException(otherRigidbody) && !otherRigidbody.IsSpecificCollisionException(this) && !this.IsTemporaryCollisionException(otherRigidbody) && !otherRigidbody.IsTemporaryCollisionException(this);
    }

    public void AddCollisionLayerOverride(int mask)
    {
      for (int index = 0; index < this.PixelColliders.Count; ++index)
        this.PixelColliders[index].CollisionLayerCollidableOverride |= mask;
    }

    public void RemoveCollisionLayerOverride(int mask)
    {
      for (int index = 0; index < this.PixelColliders.Count; ++index)
        this.PixelColliders[index].CollisionLayerCollidableOverride &= ~mask;
    }

    public void AddCollisionLayerIgnoreOverride(int mask)
    {
      for (int index = 0; index < this.PixelColliders.Count; ++index)
        this.PixelColliders[index].CollisionLayerIgnoreOverride |= mask;
    }

    public void RemoveCollisionLayerIgnoreOverride(int mask)
    {
      for (int index = 0; index < this.PixelColliders.Count; ++index)
        this.PixelColliders[index].CollisionLayerIgnoreOverride &= ~mask;
    }

    public void UpdateColliderPositions()
    {
      for (int index = 0; index < this.PixelColliders.Count; ++index)
        this.PixelColliders[index].Position = this.Position.PixelPosition + this.PixelColliders[index].Offset;
    }

    public void BraveOnLevelWasLoaded()
    {
      this.PhysicsRegistration = SpeculativeRigidbody.RegistrationState.Unknown;
    }

    public void Cleanup()
    {
      if (this.m_specificCollisionExceptions != null)
        this.m_specificCollisionExceptions.Clear();
      if (this.m_temporaryCollisionExceptions != null)
        this.m_temporaryCollisionExceptions.Clear();
      if (this.m_ghostCollisionExceptions != null)
        this.m_ghostCollisionExceptions.Clear();
      this.m_pushedRigidbodies.Clear();
      if (this.m_carriedRigidbodies == null)
        return;
      this.m_carriedRigidbodies.Clear();
    }

    public void AlignWithRigidbodyBottomLeft(SpeculativeRigidbody otherRigidbody)
    {
      Vector2 vector2_1 = this.UnitBottomLeft - this.transform.position.XY();
      Vector2 vector2_2 = otherRigidbody.UnitBottomLeft - otherRigidbody.transform.position.XY();
      this.transform.position = (Vector3) (otherRigidbody.transform.position.XY() - vector2_1 + vector2_2);
      this.specRigidbody.Reinitialize();
    }

    public void AlignWithRigidbodyBottomCenter(
      SpeculativeRigidbody otherRigidbody,
      IntVector2? pixelOffset = null)
    {
      Vector2 vector2_1 = this.UnitBottomCenter - this.transform.position.XY();
      Vector2 vector2_2 = otherRigidbody.UnitBottomCenter - otherRigidbody.transform.position.XY();
      Vector2 vector2_3 = Vector2.zero;
      if (pixelOffset.HasValue)
        vector2_3 = PhysicsEngine.PixelToUnit(pixelOffset.Value);
      this.transform.position = (Vector3) (otherRigidbody.transform.position.XY() - vector2_1 + vector2_2 + vector2_3);
      this.specRigidbody.Reinitialize();
    }

    public delegate void OnPreRigidbodyCollisionDelegate(
      SpeculativeRigidbody myRigidbody,
      PixelCollider myPixelCollider,
      SpeculativeRigidbody otherRigidbody,
      PixelCollider otherPixelCollider);

    public delegate void OnPreTileCollisionDelegate(
      SpeculativeRigidbody myRigidbody,
      PixelCollider myPixelCollider,
      PhysicsEngine.Tile tile,
      PixelCollider tilePixelCollider);

    public delegate void OnRigidbodyCollisionDelegate(CollisionData rigidbodyCollision);

    public delegate void OnBeamCollisionDelegate(BeamController beam);

    public delegate void OnTileCollisionDelegate(CollisionData tileCollision);

    public delegate void OnTriggerDelegate(
      SpeculativeRigidbody specRigidbody,
      SpeculativeRigidbody sourceSpecRigidbody,
      CollisionData collisionData);

    public delegate void OnTriggerExitDelegate(
      SpeculativeRigidbody specRigidbody,
      SpeculativeRigidbody sourceSpecRigidbody);

    public delegate void MovementRestrictorDelegate(
      SpeculativeRigidbody specRigidbody,
      IntVector2 prevPixelOffset,
      IntVector2 pixelOffset,
      ref bool validLocation);

    public enum RegistrationState
    {
      Registered,
      DeregisterScheduled,
      Deregistered,
      Unknown,
    }

    [Serializable]
    public class DebugSettings
    {
      public bool ShowPosition;
      public int PositionHistory;
      public bool ShowVelocity;
      public bool ShowSlope;
    }

    public struct PushedRigidbodyData
    {
      public SpeculativeRigidbody SpecRigidbody;
      public bool PushedThisFrame;
      public IntVector2 Direction;

      public PushedRigidbodyData(SpeculativeRigidbody specRigidbody)
      {
        SpecRigidbody = specRigidbody;
        PushedThisFrame = false;
        Direction = IntVector2.Zero;
      }

      public bool CollidedX => this.Direction.x != 0;

      public bool CollidedY => this.Direction.y != 0;

      internal IntVector2 GetPushedPixelsToMove(IntVector2 pixelsToMove)
      {
        return IntVector2.Scale(this.Direction, pixelsToMove);
      }
    }

    public struct TemporaryException
    {
      public SpeculativeRigidbody SpecRigidbody;
      public float MinTimeRemaining;
      public float? MaxTimeRemaining;

      public TemporaryException(
        SpeculativeRigidbody specRigidbody,
        float minTime,
        float? maxTime)
      {
        SpecRigidbody = specRigidbody;
        MinTimeRemaining = minTime;
        MaxTimeRemaining = maxTime;
      }

      public bool HasEnded(SpeculativeRigidbody myRigidbody)
      {
        if (!(bool) (UnityEngine.Object) this.SpecRigidbody)
          return true;
        if (this.MaxTimeRemaining.HasValue)
        {
          ref SpeculativeRigidbody.TemporaryException local = ref this;
          float? maxTimeRemaining = local.MaxTimeRemaining;
          local.MaxTimeRemaining = !maxTimeRemaining.HasValue ? new float?() : new float?(maxTimeRemaining.GetValueOrDefault() - BraveTime.DeltaTime);
          if ((double) this.MaxTimeRemaining.Value <= 0.0)
            return true;
        }
        if ((double) this.MinTimeRemaining > 0.0)
        {
          this.MinTimeRemaining -= BraveTime.DeltaTime;
          return false;
        }
        for (int index1 = 0; index1 < myRigidbody.PixelColliders.Count; ++index1)
        {
          PixelCollider pixelCollider1 = myRigidbody.PixelColliders[index1];
          for (int index2 = 0; index2 < this.SpecRigidbody.PixelColliders.Count; ++index2)
          {
            PixelCollider pixelCollider2 = this.SpecRigidbody.PixelColliders[index2];
            if (pixelCollider1.CanCollideWith(pixelCollider2, true) && pixelCollider1.Overlaps(pixelCollider2))
              return false;
          }
        }
        return true;
      }
    }
  }

