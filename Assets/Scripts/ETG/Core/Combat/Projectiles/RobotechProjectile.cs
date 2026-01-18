// Decompiled with JetBrains decompiler
// Type: RobotechProjectile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

public class RobotechProjectile : Projectile
  {
    [Header("Robotech Params")]
    public float angularAcceleration = 220f;
    public float searchRadius = 10f;
    public float searchTime = 0.5f;
    public bool canLoseTarget = true;
    public bool reacquiresTargets;
    public bool targetAcquisitionRandom;
    public float counterCurveChance = 0.66f;
    public float counterCurveDuration = 0.2f;
    public float counterCurveMaxDistance = 7f;
    public float initialDumfireTime;
    public bool selectRandomAutoAimTarget;
    [NonSerialized]
    public Vector2? initialOverrideTargetPoint;
    private RobotechProjectile.Mode m_mode = RobotechProjectile.Mode.InitialTarget;
    private RobotechProjectile.CounterCurveState m_counterCurveState;
    private float m_counterCurveDeltaAngle;
    private float m_counterCurveTimer;
    private Vector2 m_counterCurveMandatedDirection;
    private IAutoAimTarget m_currentTarget;
    private Vector2 m_targetPoint;
    private float m_targetSearchTimer;
    private float m_initialDumbfireTimer;
    private bool m_hasGoodLock;
    private static List<AIActor> s_activeEnemies = new List<AIActor>();

    public override void Start()
    {
      base.Start();
      this.m_usesNormalMoveRegardless = true;
      if (this.initialOverrideTargetPoint.HasValue)
      {
        this.m_targetPoint = this.initialOverrideTargetPoint.Value;
        this.m_mode = RobotechProjectile.Mode.InitialTarget;
      }
      else if (this.Owner is PlayerController)
      {
        if (BraveInput.GetInstanceForPlayer((this.Owner as PlayerController).PlayerIDX).IsKeyboardAndMouse())
        {
          Ray ray = GameManager.Instance.MainCameraController.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
          float enter;
          if (new Plane(Vector3.forward, Vector3.zero).Raycast(ray, out enter))
          {
            this.m_targetPoint = (Vector2) ray.GetPoint(enter);
            this.m_targetPoint += UnityEngine.Random.insideUnitCircle.normalized * 0.7f;
          }
        }
        else
          this.m_targetPoint = (Vector2) (this.Owner as PlayerController).unadjustedAimPoint;
        this.m_mode = RobotechProjectile.Mode.InitialTarget;
      }
      else if (this.Owner is DumbGunShooter)
      {
        this.m_targetPoint = (Vector2) (this.Owner.transform.position + Vector3.right * 50f);
        this.m_mode = RobotechProjectile.Mode.InitialTarget;
      }
      else
      {
        this.m_currentTarget = (IAutoAimTarget) GameManager.Instance.GetPlayerClosestToPoint(this.Owner.transform.position.XY());
        this.m_mode = RobotechProjectile.Mode.TargetLocked;
      }
      if ((double) this.initialDumfireTime > 0.0)
        this.m_mode = RobotechProjectile.Mode.InitialDumbfire;
      TrailController componentInChildren = this.GetComponentInChildren<TrailController>();
      if ((bool) (UnityEngine.Object) componentInChildren)
      {
        Vector2 vector2 = this.m_targetPoint - this.transform.position.XY();
        if ((double) Mathf.Abs(Mathf.Atan2(vector2.y, vector2.x) * 57.29578f) > 90.0)
          componentInChildren.FlipUvsY = true;
      }
      this.specRigidbody.OnPreRigidbodyCollision += new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreRigidbodyCollision);
      this.m_targetSearchTimer = this.searchTime;
      if (this.Owner is DumbGunShooter)
        this.m_targetSearchTimer = -1000000f;
      this.UpdateCollisionMask();
    }

    public override void Update()
    {
      if (this.m_mode == RobotechProjectile.Mode.InitialDumbfire)
      {
        this.m_initialDumbfireTimer += this.LocalDeltaTime;
        if ((double) this.m_initialDumbfireTimer > (double) this.initialDumfireTime)
          this.m_mode = this.Owner is PlayerController || this.Owner is DumbGunShooter ? RobotechProjectile.Mode.InitialTarget : RobotechProjectile.Mode.TargetLocked;
      }
      else if (this.m_mode == RobotechProjectile.Mode.InitialTarget)
      {
        this.m_targetSearchTimer += this.LocalDeltaTime;
        if ((double) this.m_targetSearchTimer > (double) this.searchTime && (bool) (UnityEngine.Object) this.Owner && GameManager.HasInstance && !GameManager.Instance.IsLoadingLevel)
        {
          RoomHandler roomFromPosition = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(this.Owner.transform.position.IntXY(VectorConversions.Floor));
          if (this.selectRandomAutoAimTarget && roomFromPosition != null)
          {
            List<IAutoAimTarget> autoAimTargets = roomFromPosition.GetAutoAimTargets();
            if (autoAimTargets != null && autoAimTargets.Count > 0)
            {
              this.m_currentTarget = BraveUtility.RandomElement<IAutoAimTarget>(autoAimTargets);
              this.m_mode = RobotechProjectile.Mode.TargetLocked;
              this.m_hasGoodLock = false;
            }
          }
          if (this.m_mode == RobotechProjectile.Mode.InitialTarget)
          {
            if (RobotechProjectile.s_activeEnemies == null)
              RobotechProjectile.s_activeEnemies = new List<AIActor>();
            else
              RobotechProjectile.s_activeEnemies.Clear();
            roomFromPosition?.GetActiveEnemies(RoomHandler.ActiveEnemyType.All, ref RobotechProjectile.s_activeEnemies);
            if (RobotechProjectile.s_activeEnemies.Count > 0)
            {
              if (this.targetAcquisitionRandom)
              {
                for (int index = 0; index < RobotechProjectile.s_activeEnemies.Count; ++index)
                {
                  AIActor activeEnemy = RobotechProjectile.s_activeEnemies[index];
                  if (!(bool) (UnityEngine.Object) activeEnemy || !(bool) (UnityEngine.Object) activeEnemy.healthHaver || activeEnemy.healthHaver.IsDead || activeEnemy.IsGone)
                  {
                    RobotechProjectile.s_activeEnemies.RemoveAt(index);
                    --index;
                  }
                }
                if (RobotechProjectile.s_activeEnemies.Count > 0)
                {
                  this.m_currentTarget = (IAutoAimTarget) RobotechProjectile.s_activeEnemies[UnityEngine.Random.Range(0, RobotechProjectile.s_activeEnemies.Count)];
                  this.m_mode = RobotechProjectile.Mode.TargetLocked;
                  this.m_hasGoodLock = false;
                }
              }
              else
              {
                float num1 = float.MaxValue;
                for (int index = 0; index < RobotechProjectile.s_activeEnemies.Count; ++index)
                {
                  AIActor activeEnemy = RobotechProjectile.s_activeEnemies[index];
                  if ((bool) (UnityEngine.Object) activeEnemy && (bool) (UnityEngine.Object) activeEnemy.healthHaver && (bool) (UnityEngine.Object) activeEnemy.specRigidbody && !activeEnemy.healthHaver.IsDead && !activeEnemy.IsGone)
                  {
                    float num2 = Vector2.Distance(activeEnemy.specRigidbody.UnitCenter, this.m_targetPoint);
                    if ((double) num2 < (double) num1)
                    {
                      this.m_currentTarget = (IAutoAimTarget) activeEnemy;
                      num1 = num2;
                      this.m_mode = RobotechProjectile.Mode.TargetLocked;
                      this.m_hasGoodLock = false;
                    }
                  }
                }
              }
            }
          }
        }
      }
      else if (this.m_mode == RobotechProjectile.Mode.TargetLocked && (this.m_currentTarget == null || !this.m_currentTarget.IsValid))
        this.m_mode = !this.reacquiresTargets ? RobotechProjectile.Mode.Dumbfire : RobotechProjectile.Mode.InitialTarget;
      if (this.m_mode == RobotechProjectile.Mode.TargetLocked && this.m_currentTarget != null)
        this.m_targetPoint = this.m_currentTarget.AimCenter;
      if (this.canLoseTarget && (this.m_mode == RobotechProjectile.Mode.InitialTarget || this.m_mode == RobotechProjectile.Mode.TargetLocked) && (bool) (UnityEngine.Object) this)
      {
        float f = BraveMathCollege.ClampAngle180(Vector3.Angle((Vector3) (this.m_targetPoint - this.specRigidbody.UnitCenter), (Vector3) this.m_currentDirection));
        if (this.m_counterCurveState != RobotechProjectile.CounterCurveState.Active && this.m_counterCurveState != RobotechProjectile.CounterCurveState.Mandated)
        {
          if (!this.m_hasGoodLock && (double) Mathf.Abs(f) < 10.0)
            this.m_hasGoodLock = true;
          else if (this.m_hasGoodLock && (double) Mathf.Abs(f) > 90.0)
          {
            this.m_hasGoodLock = false;
            this.m_mode = RobotechProjectile.Mode.Dumbfire;
          }
        }
      }
      base.Update();
    }

    protected override void OnDestroy() => base.OnDestroy();

    public void ForceCurveDirection(Vector2 dirVec, float duration)
    {
      this.m_counterCurveState = RobotechProjectile.CounterCurveState.Mandated;
      this.m_counterCurveMandatedDirection = dirVec;
      this.counterCurveDuration = duration;
      this.m_counterCurveTimer = 0.0f;
      this.m_hasGoodLock = false;
    }

    protected override void Move()
    {
      Vector2 currentDirection = this.m_currentDirection;
      if (this.baseData.UsesCustomAccelerationCurve)
        this.m_currentSpeed = this.baseData.AccelerationCurve.Evaluate(Mathf.Clamp01(this.m_timeElapsed / this.baseData.CustomAccelerationCurveDuration)) * this.baseData.speed;
      if (this.m_mode == RobotechProjectile.Mode.InitialTarget || this.m_mode == RobotechProjectile.Mode.TargetLocked)
      {
        Vector2 vector2 = this.m_targetPoint - this.specRigidbody.UnitCenter;
        float num1 = Mathf.Atan2(this.m_currentDirection.y, this.m_currentDirection.x) * 57.29578f;
        float f = BraveMathCollege.ClampAngle180(Mathf.Atan2(vector2.y, vector2.x) * 57.29578f - num1);
        if (this.m_counterCurveState == RobotechProjectile.CounterCurveState.Ready && (double) Mathf.Abs(f) < 1.0)
        {
          if ((double) vector2.magnitude < (double) this.counterCurveMaxDistance)
            this.m_counterCurveState = RobotechProjectile.CounterCurveState.Done;
          else if ((double) UnityEngine.Random.value > (double) this.counterCurveChance)
          {
            this.m_counterCurveState = RobotechProjectile.CounterCurveState.Done;
          }
          else
          {
            this.m_counterCurveState = RobotechProjectile.CounterCurveState.Active;
            this.m_counterCurveDeltaAngle = this.angularAcceleration * ((double) UnityEngine.Random.value >= 0.5 ? 1f : -1f);
            this.m_counterCurveTimer = 0.0f;
            this.m_hasGoodLock = false;
          }
        }
        float num2 = num1;
        float z;
        if (this.m_counterCurveState == RobotechProjectile.CounterCurveState.Mandated)
        {
          this.m_counterCurveTimer += this.LocalDeltaTime;
          z = this.m_counterCurveMandatedDirection.ToAngle();
          if ((double) this.m_counterCurveTimer > (double) this.counterCurveDuration)
            this.m_counterCurveState = RobotechProjectile.CounterCurveState.Done;
        }
        else if (this.m_counterCurveState == RobotechProjectile.CounterCurveState.Active)
        {
          this.m_counterCurveTimer += this.LocalDeltaTime;
          z = num2 + this.m_counterCurveDeltaAngle * this.LocalDeltaTime;
          if ((double) this.m_counterCurveTimer > (double) this.counterCurveDuration)
            this.m_counterCurveState = RobotechProjectile.CounterCurveState.Done;
        }
        else
        {
          float num3 = Mathf.Sign(f) * Mathf.Min(Mathf.Abs(f), Mathf.Abs(this.angularAcceleration * this.LocalDeltaTime));
          z = num2 + num3;
        }
        this.m_currentDirection = (Vector2) (Quaternion.Euler(0.0f, 0.0f, z) * Vector3.right);
        if (this.shouldRotate)
          this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, z);
      }
      else if ((this.m_mode == RobotechProjectile.Mode.InitialDumbfire || this.m_mode == RobotechProjectile.Mode.Dumbfire) && this.shouldRotate)
        this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, Mathf.Atan2(this.m_currentDirection.y, this.m_currentDirection.x) * 57.29578f);
      this.specRigidbody.Velocity = this.m_currentDirection * this.m_currentSpeed;
      this.LastVelocity = this.specRigidbody.Velocity;
      if (this.OverrideMotionModule == null)
        return;
      this.OverrideMotionModule.AdjustRightVector(Mathf.DeltaAngle(BraveMathCollege.Atan2Degrees(currentDirection), BraveMathCollege.Atan2Degrees(this.LastVelocity)));
      this.OverrideMotionModule.Move((Projectile) this, this.transform, this.sprite, this.specRigidbody, ref this.m_timeElapsed, ref this.m_currentDirection, this.Inverted, this.shouldRotate);
      this.LastVelocity = this.specRigidbody.Velocity;
    }

    public override void SetNewShooter(SpeculativeRigidbody newShooter)
    {
      this.m_mode = RobotechProjectile.Mode.Dumbfire;
      base.SetNewShooter(newShooter);
    }

    protected virtual void OnPreRigidbodyCollision(
      SpeculativeRigidbody myRigidbody,
      PixelCollider myCollider,
      SpeculativeRigidbody otherRigidbody,
      PixelCollider otherCollider)
    {
      if (!this.collidesWithProjectiles || !(bool) (UnityEngine.Object) otherRigidbody.projectile || otherRigidbody.projectile.Owner is PlayerController)
        return;
      PhysicsEngine.SkipCollision = true;
    }

    private enum Mode
    {
      InitialDumbfire,
      InitialTarget,
      TargetLocked,
      Dumbfire,
    }

    private enum CounterCurveState
    {
      Ready,
      Active,
      Done,
      Mandated,
    }
  }

