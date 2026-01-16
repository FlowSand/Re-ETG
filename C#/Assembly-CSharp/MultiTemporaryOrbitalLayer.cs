// Decompiled with JetBrains decompiler
// Type: MultiTemporaryOrbitalLayer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class MultiTemporaryOrbitalLayer
{
  public int targetNumberOrbitals;
  public float collisionDamage;
  public float circleRadius = 3f;
  public float rotationDegreesPerSecond = 360f;
  public GameObject deathVFX;
  protected GameObject m_orbitalPrefab;
  protected PlayerController m_player;
  protected List<SpeculativeRigidbody> m_orbitals;
  protected float m_elapsed;
  protected float m_traversedDistance;
  protected Vector3 m_currentShieldVelocity = Vector3.zero;
  protected Vector3 m_currentShieldCenterOffset = Vector3.zero;
  private Vector3 m_cachedOffsetBase;

  public void Initialize(PlayerController player, GameObject orbitalPrefab)
  {
    this.m_player = player;
    this.m_orbitalPrefab = orbitalPrefab;
    this.m_orbitals = new List<SpeculativeRigidbody>();
    for (int index = 0; index < this.targetNumberOrbitals; ++index)
    {
      Vector3 position = player.LockedApproximateSpriteCenter + Quaternion.Euler(0.0f, 0.0f, 360f / (float) this.targetNumberOrbitals * (float) index) * Vector3.right * this.circleRadius;
      GameObject gameObject = Object.Instantiate<GameObject>(orbitalPrefab, position, Quaternion.identity);
      gameObject.GetComponent<tk2dSprite>().HeightOffGround = 1.5f;
      tk2dSpriteAnimator component1 = gameObject.GetComponent<tk2dSpriteAnimator>();
      component1.PlayFromFrame(Random.Range(0, component1.DefaultClip.frames.Length));
      SpeculativeRigidbody component2 = gameObject.GetComponent<SpeculativeRigidbody>();
      component2.OnPreRigidbodyCollision += new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreCollision);
      component2.OnTriggerCollision += new SpeculativeRigidbody.OnTriggerDelegate(this.HandleCollision);
      component2.OnTileCollision += new SpeculativeRigidbody.OnTileCollisionDelegate(this.HandleTileCollision);
      this.m_orbitals.Add(component2);
    }
  }

  public void Disconnect()
  {
    if (this.m_orbitals == null || this.m_orbitals.Count == 0)
      return;
    for (int index = 0; index < this.m_orbitals.Count; ++index)
    {
      if ((bool) (Object) this.m_orbitals[index])
        this.m_orbitals[index].CollideWithTileMap = true;
    }
  }

  private void HandleTileCollision(CollisionData tileCollision)
  {
    this.DestroyKnife(tileCollision.MyRigidbody);
  }

  protected Vector3 GetTargetPositionForKniveID(Vector3 center, int i, float radiusToUse)
  {
    float num = (float) ((double) this.rotationDegreesPerSecond * (double) this.m_elapsed % 360.0);
    return center + Quaternion.Euler(0.0f, 0.0f, num + 360f / (float) this.m_orbitals.Count * (float) i) * Vector3.right * radiusToUse;
  }

  private void OnPreCollision(
    SpeculativeRigidbody myRigidbody,
    PixelCollider myCollider,
    SpeculativeRigidbody other,
    PixelCollider otherCollider)
  {
    Projectile component1 = other.GetComponent<Projectile>();
    if ((Object) component1 != (Object) null && component1.Owner is PlayerController)
      PhysicsEngine.SkipCollision = true;
    GameActor component2 = other.GetComponent<GameActor>();
    if (component2 is PlayerController)
      PhysicsEngine.SkipCollision = true;
    if (!(component2 is AIActor) || (component2 as AIActor).IsNormalEnemy)
      return;
    PhysicsEngine.SkipCollision = true;
  }

  private void HandleCollision(
    SpeculativeRigidbody other,
    SpeculativeRigidbody source,
    CollisionData collisionData)
  {
    if ((Object) other.GetComponent<AIActor>() != (Object) null)
    {
      other.GetComponent<HealthHaver>().ApplyDamage(this.collisionDamage, Vector2.zero, "Orbital Shield");
      this.DestroyKnife(source);
    }
    else
    {
      if (!((Object) other.GetComponent<Projectile>() != (Object) null))
        return;
      Projectile component = other.GetComponent<Projectile>();
      if (component.Owner is PlayerController)
        return;
      component.DieInAir();
      this.DestroyKnife(source);
    }
  }

  private void DestroyKnife(SpeculativeRigidbody source)
  {
    int index = this.m_orbitals.IndexOf(source);
    if (index != -1)
      this.m_orbitals.RemoveAt(index);
    source.sprite.PlayEffectOnSprite(this.deathVFX, Vector3.zero, false);
    --this.targetNumberOrbitals;
    Object.Destroy((Object) source.gameObject);
  }

  public void Update()
  {
    if (GameManager.Instance.IsLoadingLevel || Dungeon.IsGenerating)
      return;
    this.m_elapsed += BraveTime.DeltaTime;
    this.m_currentShieldCenterOffset += this.m_currentShieldVelocity * BraveTime.DeltaTime;
    this.m_cachedOffsetBase = this.m_player.LockedApproximateSpriteCenter;
    Vector3 center = this.m_cachedOffsetBase + this.m_currentShieldCenterOffset;
    float circleRadius = this.circleRadius;
    while (this.m_orbitals.Count < this.targetNumberOrbitals)
      this.AddOrbital();
    for (int index = 0; index < this.m_orbitals.Count; ++index)
    {
      if ((Object) this.m_orbitals[index] != (Object) null && (bool) (Object) this.m_orbitals[index])
      {
        Vector2 vector2 = (this.GetTargetPositionForKniveID(center, index, circleRadius) - this.m_orbitals[index].transform.position).XY() / BraveTime.DeltaTime;
        this.m_orbitals[index].Velocity = vector2;
        this.m_orbitals[index].sprite.UpdateZDepth();
      }
    }
  }

  private void AddOrbital()
  {
    GameObject gameObject = Object.Instantiate<GameObject>(this.m_orbitalPrefab, this.m_player.LockedApproximateSpriteCenter + Quaternion.Euler(0.0f, 0.0f, 360f / (float) this.targetNumberOrbitals * (float) (this.m_orbitals.Count - 1)) * Vector3.right * this.circleRadius, Quaternion.identity);
    gameObject.GetComponent<tk2dSprite>().HeightOffGround = 1.5f;
    tk2dSpriteAnimator component1 = gameObject.GetComponent<tk2dSpriteAnimator>();
    component1.PlayFromFrame(Random.Range(0, component1.DefaultClip.frames.Length));
    SpeculativeRigidbody component2 = gameObject.GetComponent<SpeculativeRigidbody>();
    component2.OnPreRigidbodyCollision += new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreCollision);
    component2.OnTriggerCollision += new SpeculativeRigidbody.OnTriggerDelegate(this.HandleCollision);
    component2.OnTileCollision += new SpeculativeRigidbody.OnTileCollisionDelegate(this.HandleTileCollision);
    this.m_orbitals.Add(component2);
  }
}
