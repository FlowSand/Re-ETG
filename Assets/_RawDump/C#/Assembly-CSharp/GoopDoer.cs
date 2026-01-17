// Decompiled with JetBrains decompiler
// Type: GoopDoer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
public class GoopDoer : BraveBehaviour
{
  public GoopDefinition goopDefinition;
  public GoopDoer.PositionSource positionSource;
  [ShowInInspectorIf("positionSource", 3, false)]
  public GameObject goopCenter;
  public GoopDoer.UpdateTiming updateTiming;
  public float updateFrequency = 0.05f;
  public bool isTimed;
  [ShowInInspectorIf("isTimed", true)]
  public float goopTime = 1f;
  [Header("Triggers")]
  public bool updateOnPreDeath;
  public bool updateOnDeath;
  public bool updateOnAnimFrames;
  public bool updateOnCollision;
  public bool updateOnGrounded;
  public bool updateOnDestroy;
  [Header("Global Settings")]
  public float defaultGoopRadius = 1f;
  public bool suppressSplashes;
  public bool goopSizeVaries;
  [ShowInInspectorIf("goopSizeVaries", false)]
  public float varyCycleTime = 1f;
  [ShowInInspectorIf("goopSizeVaries", false)]
  public float radiusMin = 0.5f;
  [ShowInInspectorIf("goopSizeVaries", false)]
  public float radiusMax = 1f;
  [ShowInInspectorIf("goopSizeVaries", false)]
  public bool goopSizeRandom;
  [Header("Particles")]
  public bool UsesDispersalParticles;
  [ShowInInspectorIf("UsesDispersalParticles", false)]
  public float DispersalDensity = 3f;
  [ShowInInspectorIf("UsesDispersalParticles", false)]
  public float DispersalMinCoherency = 0.2f;
  [ShowInInspectorIf("UsesDispersalParticles", false)]
  public float DispersalMaxCoherency = 1f;
  [ShowInInspectorIf("UsesDispersalParticles", false)]
  public GameObject DispersalParticleSystemPrefab;
  private float m_updateTimer;
  private DeadlyDeadlyGoopManager m_gooper;
  private ParticleSystem m_dispersalParticles;
  private Vector2 m_lastGoopPosition = Vector2.zero;
  private float m_timeSinceLastGoop = 10f;

  public void Start()
  {
    this.m_gooper = DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(this.goopDefinition);
    if (this.updateOnAnimFrames && (bool) (UnityEngine.Object) this.spriteAnimator)
      this.spriteAnimator.AnimationEventTriggered += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.HandleAnimationEvent);
    if (this.updateOnPreDeath && (bool) (UnityEngine.Object) this.healthHaver)
      this.healthHaver.OnPreDeath += new Action<Vector2>(this.OnPreDeath);
    if (this.updateOnDeath && (bool) (UnityEngine.Object) this.healthHaver)
      this.healthHaver.OnDeath += new Action<Vector2>(this.OnDeath);
    if (this.updateOnCollision)
      this.specRigidbody.OnCollision += new Action<CollisionData>(this.OnCollision);
    if (this.updateOnGrounded)
    {
      if ((bool) (UnityEngine.Object) this.debris)
        this.debris.OnGrounded += new Action<DebrisObject>(this.OnDebrisGrounded);
      if (this.projectile is ArcProjectile)
        (this.projectile as ArcProjectile).OnGrounded += new System.Action(this.OnProjectileGrounded);
    }
    if (!this.UsesDispersalParticles || !((UnityEngine.Object) this.m_dispersalParticles == (UnityEngine.Object) null))
      return;
    this.m_dispersalParticles = GlobalDispersalParticleManager.GetSystemForPrefab(this.DispersalParticleSystemPrefab);
  }

  public void Update()
  {
    this.m_timeSinceLastGoop += BraveTime.DeltaTime;
    if (!this.ShouldUpdate() || (UnityEngine.Object) this.aiActor != (UnityEngine.Object) null && !this.aiActor.HasBeenEngaged || (UnityEngine.Object) this.aiActor != (UnityEngine.Object) null && !this.aiActor.HasBeenAwoken)
      return;
    this.m_updateTimer -= BraveTime.DeltaTime;
    if ((double) this.m_updateTimer > 0.0)
      return;
    this.GoopItUp();
    this.m_updateTimer = this.updateFrequency;
  }

  protected override void OnDestroy()
  {
    if (this.updateOnDestroy && (UnityEngine.Object) this.m_gooper != (UnityEngine.Object) null)
      this.GoopItUp();
    if (this.updateOnAnimFrames && (bool) (UnityEngine.Object) this.spriteAnimator)
      this.spriteAnimator.AnimationEventTriggered -= new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.HandleAnimationEvent);
    if (this.updateOnPreDeath && (bool) (UnityEngine.Object) this.healthHaver)
      this.healthHaver.OnPreDeath -= new Action<Vector2>(this.OnPreDeath);
    if (this.updateOnDeath && (bool) (UnityEngine.Object) this.healthHaver)
      this.healthHaver.OnDeath -= new Action<Vector2>(this.OnDeath);
    if (this.updateOnGrounded)
    {
      if ((bool) (UnityEngine.Object) this.debris)
        this.debris.OnGrounded -= new Action<DebrisObject>(this.OnDebrisGrounded);
      if (this.projectile is ArcProjectile)
        (this.projectile as ArcProjectile).OnGrounded -= new System.Action(this.OnProjectileGrounded);
    }
    base.OnDestroy();
  }

  private void HandleAnimationEvent(
    tk2dSpriteAnimator animator,
    tk2dSpriteAnimationClip clip,
    int frameNum)
  {
    if (!(clip.GetFrame(frameNum).eventInfo == "goop"))
      return;
    this.GoopItUp();
  }

  private void OnPreDeath(Vector2 finalDamageDirection) => this.GoopItUp();

  private void OnDeath(Vector2 finalDamageDirection) => this.GoopItUp();

  private void OnCollision(CollisionData collisionData) => this.GoopItUp();

  private void OnDebrisGrounded(DebrisObject debrisObject) => this.GoopItUp();

  private void OnProjectileGrounded() => this.GoopItUp();

  private bool ShouldUpdate()
  {
    if (this.updateTiming == GoopDoer.UpdateTiming.Always)
      return true;
    if (this.updateTiming != GoopDoer.UpdateTiming.IfMoving)
      return false;
    return (double) Mathf.Abs(this.specRigidbody.Velocity.x) > 9.9999997473787516E-05 || (double) Mathf.Abs(this.specRigidbody.Velocity.y) > 9.9999997473787516E-05;
  }

  private void GoopItUp()
  {
    float radius1 = this.defaultGoopRadius;
    Vector2 vector2_1 = (Vector2) this.transform.position;
    if (this.positionSource == GoopDoer.PositionSource.SpriteCenter)
      vector2_1 = this.sprite.WorldCenter;
    else if (this.positionSource == GoopDoer.PositionSource.GroundCenter)
      vector2_1 = this.specRigidbody.GetUnitCenter(ColliderType.Ground);
    else if (this.positionSource == GoopDoer.PositionSource.HitBoxCenter)
    {
      if (this.specRigidbody.HitboxPixelCollider == null || !this.specRigidbody.HitboxPixelCollider.Enabled)
        return;
      vector2_1 = this.specRigidbody.GetUnitCenter(ColliderType.HitBox);
    }
    else if (this.positionSource == GoopDoer.PositionSource.SpecifyGameObject)
      vector2_1 = this.goopCenter.transform.position.XY();
    if (this.isTimed && this.m_lastGoopPosition != Vector2.zero && (double) Vector2.Distance(vector2_1, this.m_lastGoopPosition) < 0.20000000298023224 && (double) this.m_timeSinceLastGoop < (double) this.goopTime)
      return;
    if (this.goopSizeVaries)
      radius1 = !this.goopSizeRandom ? BraveMathCollege.SmoothLerp(this.radiusMin, this.radiusMax, Mathf.PingPong(UnityEngine.Time.time, this.varyCycleTime) / this.varyCycleTime) : UnityEngine.Random.Range(this.radiusMin, this.radiusMax);
    if (this.isTimed)
    {
      this.m_gooper.TimedAddGoopCircle(vector2_1, radius1, this.goopTime, this.suppressSplashes);
    }
    else
    {
      DeadlyDeadlyGoopManager gooper = this.m_gooper;
      Vector2 vector2_2 = vector2_1;
      float num1 = radius1;
      bool suppressSplashes = this.suppressSplashes;
      Vector2 center = vector2_2;
      double radius2 = (double) num1;
      int num2 = suppressSplashes ? 1 : 0;
      gooper.AddGoopCircle(center, (float) radius2, suppressSplashes: num2 != 0);
    }
    if (this.UsesDispersalParticles)
      this.DoDispersalParticles(vector2_1, radius1);
    this.m_lastGoopPosition = vector2_1;
    this.m_timeSinceLastGoop = 0.0f;
  }

  private void DoDispersalParticles(Vector2 posStart, float radius)
  {
    int num = Mathf.RoundToInt(radius * radius * this.DispersalDensity);
    for (int index = 0; index < num; ++index)
    {
      Vector3 vector3_1 = (Vector3) (posStart + UnityEngine.Random.insideUnitCircle * UnityEngine.Random.Range(0.0f, radius));
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

  public enum PositionSource
  {
    SpriteCenter,
    GroundCenter,
    HitBoxCenter,
    SpecifyGameObject,
  }

  public enum UpdateTiming
  {
    Always,
    IfMoving,
    TriggerOnly,
  }
}
