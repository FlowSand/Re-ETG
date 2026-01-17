// Decompiled with JetBrains decompiler
// Type: Brave.BulletScript.Bullet
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable
namespace Brave.BulletScript;

public class Bullet
{
  public string BankName;
  public string SpawnTransform;
  public float Direction;
  public float Speed;
  public Vector2 Velocity;
  public bool AutoRotation;
  public float TimeScale = 1f;
  public IBulletManager BulletManager;
  public GameObject Parent;
  public Projectile Projectile;
  public bool DontDestroyGameObject;
  protected readonly List<Bullet.ITask> m_tasks = new List<Bullet.ITask>();
  private float m_timer;
  private bool m_hasFiredBullet;
  private const float c_idealFrameTime = 0.0166666675f;

  public Bullet(
    string bankName = null,
    bool suppressVfx = false,
    bool firstBulletOfAttack = false,
    bool forceBlackBullet = false)
  {
    this.BankName = bankName;
    this.SuppressVfx = suppressVfx;
    this.FirstBulletOfAttack = firstBulletOfAttack;
    this.ForceBlackBullet = forceBlackBullet;
  }

  public Transform RootTransform { get; set; }

  public Vector2 Position { get; set; }

  public Vector2 PredictedPosition
  {
    get
    {
      return new Vector2(this.Position.x + this.m_timer * this.Velocity.x, this.Position.y + this.m_timer * this.Velocity.y);
    }
  }

  public AIBulletBank BulletBank => this.BulletManager as AIBulletBank;

  public bool ManualControl { get; set; }

  public bool DisableMotion { get; set; }

  public bool Destroyed { get; set; }

  public bool SuppressVfx { get; set; }

  public bool FirstBulletOfAttack { get; set; }

  public bool ForceBlackBullet { get; set; }

  public bool EndOnBlank { get; set; }

  public int Tick { get; set; }

  public float AimDirection => (this.BulletManager.PlayerPosition() - this.Position).ToAngle();

  public bool IsOwnerAlive
  {
    get
    {
      AIActor aiActor = (AIActor) null;
      if ((bool) (UnityEngine.Object) this.BulletBank)
        aiActor = this.BulletBank.aiActor;
      if (!(bool) (UnityEngine.Object) aiActor && (bool) (UnityEngine.Object) this.Projectile)
        aiActor = this.Projectile.Owner as AIActor;
      return (bool) (UnityEngine.Object) aiActor && (bool) (UnityEngine.Object) aiActor.healthHaver && aiActor.healthHaver.IsAlive;
    }
  }

  public virtual void Initialize()
  {
    IEnumerator enumerator = this.Top();
    if (enumerator == null)
      return;
    this.m_tasks.Add((Bullet.ITask) new Bullet.Task(enumerator));
  }

  private float LocalDeltaTime
  {
    get
    {
      if ((bool) (UnityEngine.Object) this.Projectile)
        return this.Projectile.LocalDeltaTime;
      return (bool) (UnityEngine.Object) this.BulletBank && (bool) (UnityEngine.Object) this.BulletBank.aiActor ? this.BulletBank.aiActor.LocalDeltaTime : BraveTime.DeltaTime;
    }
  }

  public void FrameUpdate()
  {
    this.m_timer += this.LocalDeltaTime * this.TimeScale * Projectile.EnemyBulletSpeedMultiplier;
    while ((double) this.m_timer > 0.01666666753590107)
    {
      this.m_timer -= 0.0166666675f;
      this.DoTick();
    }
  }

  public void DoTick()
  {
    for (int index = 0; index < this.m_tasks.Count; ++index)
    {
      if (this.m_tasks[index] != null)
      {
        bool isFinished;
        this.m_tasks[index].Tick(out isFinished);
        if (isFinished && index < this.m_tasks.Count)
          this.m_tasks[index] = (Bullet.ITask) null;
      }
    }
    ++this.Tick;
    if (this.ManualControl)
      return;
    this.UpdateVelocity();
    this.UpdatePosition();
  }

  public void HandleBulletDestruction(
    Bullet.DestroyType destroyType,
    SpeculativeRigidbody hitRigidbody,
    bool allowProjectileSpawns)
  {
    this.Destroyed = true;
    bool preventSpawningProjectiles = ((!allowProjectileSpawns ? 1 : 0) | (destroyType != Bullet.DestroyType.HitRigidbody || !(bool) (UnityEngine.Object) hitRigidbody ? 0 : ((bool) (UnityEngine.Object) hitRigidbody.GetComponent<PlayerOrbital>() ? 1 : 0))) != 0;
    this.OnBulletDestruction(destroyType, hitRigidbody, preventSpawningProjectiles);
  }

  public virtual void OnBulletDestruction(
    Bullet.DestroyType destroyType,
    SpeculativeRigidbody hitRigidbody,
    bool preventSpawningProjectiles)
  {
  }

  public bool IsEnded
  {
    get
    {
      for (int index = 0; index < this.m_tasks.Count; ++index)
      {
        if (this.m_tasks[index] != null)
          return false;
      }
      return true;
    }
  }

  public void ForceEnd()
  {
    this.OnForceEnded();
    this.m_tasks.Clear();
  }

  public virtual void OnForceEnded()
  {
  }

  public virtual void OnForceRemoved()
  {
  }

  public float GetAimDirection(string transform)
  {
    Vector2 vector2 = this.BulletManager.TransformOffset(this.Position, transform);
    return (this.BulletManager.PlayerPosition() - vector2).ToAngle();
  }

  public float GetAimDirection(float leadAmount, float speed)
  {
    return this.GetAimDirection(this.Position, leadAmount, speed);
  }

  public float GetAimDirection(string transform, float leadAmount, float speed)
  {
    return this.GetAimDirection(this.BulletManager.TransformOffset(this.Position, transform), leadAmount, speed);
  }

  public float GetAimDirection(Vector2 position, float leadAmount, float speed)
  {
    Vector2 targetOrigin = this.BulletManager.PlayerPosition();
    Vector2 predictedPosition = BraveMathCollege.GetPredictedPosition(targetOrigin, this.BulletManager.PlayerVelocity(), position, speed);
    targetOrigin = new Vector2(targetOrigin.x + (predictedPosition.x - targetOrigin.x) * leadAmount, targetOrigin.y + (predictedPosition.y - targetOrigin.y) * leadAmount);
    return (targetOrigin - position).ToAngle();
  }

  public Vector2 GetPredictedTargetPosition(float leadAmount, float speed)
  {
    Vector2 position = this.Position;
    Vector2 targetOrigin = this.BulletManager.PlayerPosition();
    Vector2 predictedPosition = BraveMathCollege.GetPredictedPosition(targetOrigin, this.BulletManager.PlayerVelocity(), position, speed);
    targetOrigin = new Vector2(targetOrigin.x + (predictedPosition.x - targetOrigin.x) * leadAmount, targetOrigin.y + (predictedPosition.y - targetOrigin.y) * leadAmount);
    return targetOrigin;
  }

  public Vector2 GetPredictedTargetPositionExact(float leadAmount, float speed)
  {
    this.BulletBank.SuppressPlayerVelocityAveraging = true;
    Vector2 position = this.Position;
    Vector2 targetOrigin = this.BulletManager.PlayerPosition();
    Vector2 predictedPosition = BraveMathCollege.GetPredictedPosition(targetOrigin, this.BulletManager.PlayerVelocity(), position, speed);
    targetOrigin = new Vector2(targetOrigin.x + (predictedPosition.x - targetOrigin.x) * leadAmount, targetOrigin.y + (predictedPosition.y - targetOrigin.y) * leadAmount);
    this.BulletBank.SuppressPlayerVelocityAveraging = false;
    return targetOrigin;
  }

  public void PostWwiseEvent(string AudioEvent, string SwitchName = null)
  {
    if (!(bool) (UnityEngine.Object) this.BulletBank)
      return;
    this.BulletBank.PostWwiseEvent(AudioEvent, SwitchName);
  }

  public void Fire(Bullet bullet = null)
  {
    this.Fire((Offset) null, (Brave.BulletScript.Direction) null, (Brave.BulletScript.Speed) null, bullet);
  }

  public void Fire(Offset offset = null, Bullet bullet = null)
  {
    this.Fire(offset, (Brave.BulletScript.Direction) null, (Brave.BulletScript.Speed) null, bullet);
  }

  public void Fire(Offset offset = null, Brave.BulletScript.Speed speed = null, Bullet bullet = null)
  {
    this.Fire(offset, (Brave.BulletScript.Direction) null, speed, bullet);
  }

  public void Fire(Offset offset = null, Brave.BulletScript.Direction direction = null, Bullet bullet = null)
  {
    this.Fire(offset, direction, (Brave.BulletScript.Speed) null, bullet);
  }

  public void Fire(Brave.BulletScript.Direction direction = null, Bullet bullet = null)
  {
    this.Fire((Offset) null, direction, (Brave.BulletScript.Speed) null, bullet);
  }

  public void Fire(Brave.BulletScript.Direction direction = null, Brave.BulletScript.Speed speed = null, Bullet bullet = null)
  {
    this.Fire((Offset) null, direction, speed, bullet);
  }

  public void Fire(Offset offset = null, Brave.BulletScript.Direction direction = null, Brave.BulletScript.Speed speed = null, Bullet bullet = null)
  {
    if (bullet == null)
      bullet = new Bullet();
    if (!this.m_hasFiredBullet)
      bullet.FirstBulletOfAttack = true;
    bullet.BulletManager = this.BulletManager;
    if (this is Script)
      bullet.RootTransform = this.RootTransform;
    bullet.Position = this.Position;
    bullet.Direction = this.Direction;
    bullet.Speed = this.Speed;
    bullet.m_timer = this.m_timer - this.LocalDeltaTime;
    bullet.EndOnBlank = this.EndOnBlank;
    float? overrideBaseDirection = new float?();
    if (offset != null)
    {
      overrideBaseDirection = offset.GetDirection(this);
      if (!string.IsNullOrEmpty(offset.transform))
      {
        bullet.SpawnTransform = offset.transform;
        Transform transform = this.BulletBank.GetTransform(offset.transform);
        if ((bool) (UnityEngine.Object) transform)
          bullet.RootTransform = transform;
      }
    }
    bullet.Position = offset == null ? this.Position : offset.GetPosition(this);
    bullet.Direction = direction == null ? 0.0f : direction.GetDirection(this, overrideBaseDirection);
    bullet.Speed = speed == null ? 0.0f : speed.GetSpeed(this);
    this.BulletManager.BulletSpawnedHandler(bullet);
    if ((bool) (UnityEngine.Object) this.Projectile && this.Projectile.IsBlackBullet && (bool) (UnityEngine.Object) bullet.Projectile)
    {
      bullet.Projectile.ForceBlackBullet = true;
      bullet.Projectile.BecomeBlackBullet();
    }
    this.m_hasFiredBullet = true;
  }

  protected void ChangeSpeed(Brave.BulletScript.Speed speed, int term = 1)
  {
    if (term <= 1)
      this.Speed = speed.GetSpeed(this);
    else
      this.m_tasks.Add((Bullet.ITask) new Bullet.Task(this.ChangeSpeedTask(speed, term)));
  }

  protected void ChangeDirection(Brave.BulletScript.Direction direction, int term = 1)
  {
    if (term <= 1)
      this.Direction = direction.GetDirection(this);
    else
      this.m_tasks.Add((Bullet.ITask) new Bullet.Task(this.ChangeDirectionTask(direction, term)));
  }

  protected void StartTask(IEnumerator enumerator)
  {
    this.m_tasks.Add((Bullet.ITask) new Bullet.Task(enumerator));
  }

  protected int Wait(int frames) => frames;

  protected int Wait(float frames) => Mathf.CeilToInt(frames);

  public void Vanish(bool suppressInAirEffects = false)
  {
    this.Destroyed = true;
    this.BulletManager.DestroyBullet(this, suppressInAirEffects);
  }

  protected virtual IEnumerator Top() => (IEnumerator) null;

  protected void UpdateVelocity()
  {
    float f = this.Direction * ((float) Math.PI / 180f);
    this.Velocity.x = Mathf.Cos(f) * this.Speed;
    this.Velocity.y = Mathf.Sin(f) * this.Speed;
  }

  protected void UpdatePosition()
  {
    Vector2 position = this.Position;
    position.x += this.Velocity.x / 60f;
    position.y += this.Velocity.y / 60f;
    this.Position = position;
  }

  protected float RandomAngle() => (float) UnityEngine.Random.Range(0, 360);

  protected float SubdivideRange(
    float startValue,
    float endValue,
    int numDivisions,
    int i,
    bool offset = false)
  {
    return Mathf.Lerp(startValue, endValue, ((float) i + (!offset ? 0.0f : 0.5f)) / (float) (numDivisions - 1));
  }

  protected float SubdivideArc(
    float startAngle,
    float sweepAngle,
    int numBullets,
    int i,
    bool offset = false)
  {
    return startAngle + Mathf.Lerp(0.0f, sweepAngle, ((float) i + (!offset ? 0.0f : 0.5f)) / (float) (numBullets - 1));
  }

  protected float SubdivideCircle(
    float startAngle,
    int numBullets,
    int i,
    float direction = 1f,
    bool offset = false)
  {
    return startAngle + direction * Mathf.Lerp(0.0f, 360f, ((float) i + (!offset ? 0.0f : 0.5f)) / (float) numBullets);
  }

  protected bool IsPointInTile(Vector2 point)
  {
    if (!GameManager.Instance.Dungeon.data.CheckInBoundsAndValid((int) point.x, (int) point.y))
      return true;
    int mask = CollisionMask.LayerToMask(CollisionLayer.HighObstacle, CollisionLayer.BulletBlocker);
    return PhysicsEngine.Instance.Pointcast(point, out SpeculativeRigidbody _, true, false, mask, new CollisionLayer?(CollisionLayer.Projectile), false);
  }

  [DebuggerHidden]
  private IEnumerator ChangeSpeedTask(Brave.BulletScript.Speed speed, int term)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new Bullet__ChangeSpeedTaskc__Iterator0()
    {
      speed = speed,
      term = term,
      _this = this
    };
  }

  [DebuggerHidden]
  private IEnumerator ChangeDirectionTask(Brave.BulletScript.Direction direction, int term)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new Bullet__ChangeDirectionTaskc__Iterator1()
    {
      direction = direction,
      term = term,
      _this = this
    };
  }

  public enum DestroyType
  {
    DieInAir,
    HitRigidbody,
    HitTile,
  }

  protected interface ITask
  {
    void Tick(out bool isFinished);
  }

  protected class Task : Bullet.ITask
  {
    private int m_wait;
    private IEnumerator m_currentEnum;
    private List<IEnumerator> m_enumStack;

    public Task(IEnumerator enumerator) => this.m_currentEnum = enumerator;

    public void Tick(out bool isFinished)
    {
      if (this.m_wait > 0)
      {
        --this.m_wait;
        isFinished = false;
      }
      else if (!this.m_currentEnum.MoveNext())
      {
        if (this.m_enumStack == null || this.m_enumStack.Count == 1)
        {
          isFinished = true;
        }
        else
        {
          this.m_enumStack.RemoveAt(this.m_enumStack.Count - 1);
          this.m_currentEnum = this.m_enumStack[this.m_enumStack.Count - 1];
          this.Tick(out isFinished);
        }
      }
      else
      {
        isFinished = false;
        object current = this.m_currentEnum.Current;
        switch (current)
        {
          case int num:
            this.m_wait = num - 1;
            break;
          case null:
            break;
          case IEnumerator _:
            if (this.m_enumStack == null)
            {
              this.m_enumStack = new List<IEnumerator>();
              this.m_enumStack.Add(this.m_currentEnum);
            }
            this.m_enumStack.Add(current as IEnumerator);
            this.m_currentEnum = current as IEnumerator;
            this.Tick(out isFinished);
            break;
          default:
            UnityEngine.Debug.LogError((object) ("Unknown return type from BulletScript: " + current));
            break;
        }
      }
    }
  }

  protected class TaskLite : Bullet.ITask
  {
    private int m_wait;
    private BulletLite m_currentBullet;
    private int m_state;

    public TaskLite(BulletLite bullet) => this.m_currentBullet = bullet;

    public void Tick(out bool isFinished)
    {
      if (this.m_wait > 0)
      {
        --this.m_wait;
        isFinished = false;
      }
      else
      {
        if (this.m_currentBullet.Tick == 0)
          this.m_currentBullet.Start();
        int num = this.m_currentBullet.Update(ref this.m_state);
        if (num == -1)
        {
          isFinished = true;
        }
        else
        {
          isFinished = false;
          this.m_wait = num - 1;
        }
      }
    }
  }
}
