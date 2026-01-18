using FullInspector;
using System;
using UnityEngine;

#nullable disable

public class ShootBehavior : BasicAttackBehavior
  {
    public GameObject ShootPoint;
    [InspectorShowIf("ShowBulletScript")]
    public BulletScriptSelector BulletScript;
    [InspectorShowIf("ShowBulletName")]
    public string BulletName;
    [InspectorShowIf("IsSingleBullet")]
    public float LeadAmount;
    public ShootBehavior.StopType StopDuring;
    [InspectorShowIf("ShowImmobileDuringStop")]
    public bool ImmobileDuringStop;
    public float MoveSpeedModifier = 1f;
    public bool LockFacingDirection;
    [InspectorIndent]
    [InspectorShowIf("LockFacingDirection")]
    public bool ContinueAimingDuringTell;
    [InspectorIndent]
    [InspectorShowIf("LockFacingDirection")]
    public bool ReaimOnFire;
    public bool MultipleFireEvents;
    public bool RequiresTarget = true;
    public bool PreventTargetSwitching;
    public bool Uninterruptible;
    public bool ClearGoop;
    [InspectorIndent]
    [InspectorShowIf("ClearGoop")]
    public float ClearGoopRadius = 2f;
    [InspectorShowIf("ShowBulletName")]
    public bool ShouldOverrideFireDirection;
    [InspectorIndent]
    [InspectorShowIf("ShowOverrideFireDirection")]
    public float OverrideFireDirection;
    [InspectorCategory("Visuals")]
    public AIAnimator SpecifyAiAnimator;
    [InspectorCategory("Visuals")]
    public string ChargeAnimation;
    [InspectorCategory("Visuals")]
    [InspectorShowIf("ShowChargeTime")]
    public float ChargeTime;
    [InspectorCategory("Visuals")]
    public string TellAnimation;
    [InspectorCategory("Visuals")]
    public string FireAnimation;
    [InspectorCategory("Visuals")]
    public string PostFireAnimation;
    [InspectorCategory("Visuals")]
    public bool HideGun = true;
    [InspectorCategory("Visuals")]
    public bool OverrideBaseAnims;
    [InspectorShowIf("OverrideBaseAnims")]
    [InspectorIndent]
    [InspectorCategory("Visuals")]
    public string OverrideIdleAnim;
    [InspectorIndent]
    [InspectorCategory("Visuals")]
    [InspectorShowIf("OverrideBaseAnims")]
    public string OverrideMoveAnim;
    [InspectorCategory("Visuals")]
    public bool UseVfx;
    [InspectorCategory("Visuals")]
    [InspectorShowIf("UseVfx")]
    [InspectorIndent]
    public string ChargeVfx;
    [InspectorShowIf("UseVfx")]
    [InspectorCategory("Visuals")]
    [InspectorIndent]
    public string TellVfx;
    [InspectorCategory("Visuals")]
    [InspectorShowIf("UseVfx")]
    [InspectorIndent]
    public string FireVfx;
    [InspectorIndent]
    [InspectorCategory("Visuals")]
    [InspectorShowIf("UseVfx")]
    public string Vfx;
    [InspectorCategory("Visuals")]
    public GameObject[] EnabledDuringAttack;
    private SpeculativeRigidbody m_specRigidbody;
    private AIBulletBank m_bulletBank;
    private BulletScriptSource m_bulletSource;
    private float m_chargeTimer;
    private bool m_beganInactive;
    private bool m_isAimLocked;
    private float m_cachedMovementSpeed;
    private Vector2 m_cachedTargetCenter;
    private int m_goopExceptionId = -1;
    private ShootBehavior.State m_state;

    private bool ShowBulletScript() => string.IsNullOrEmpty(this.BulletName);

    private bool ShowBulletName() => this.BulletScript == null || this.BulletScript.IsNull;

    private bool ShowImmobileDuringStop() => this.StopDuring != ShootBehavior.StopType.None;

    private bool ShowChargeTime() => !string.IsNullOrEmpty(this.ChargeAnimation);

    private bool ShowOverrideFireDirection()
    {
      return this.ShowBulletName() && this.ShouldOverrideFireDirection;
    }

    public bool IsBulletScript
    {
      get => this.BulletScript != null && !string.IsNullOrEmpty(this.BulletScript.scriptTypeName);
    }

    public bool IsSingleBullet => !string.IsNullOrEmpty(this.BulletName);

    public override void Start()
    {
      base.Start();
      if ((bool) (UnityEngine.Object) this.SpecifyAiAnimator)
        this.m_aiAnimator = this.SpecifyAiAnimator;
      if (string.IsNullOrEmpty(this.TellAnimation))
        return;
      this.m_aiAnimator.spriteAnimator.AnimationEventTriggered += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.AnimEventTriggered);
      if (!(bool) (UnityEngine.Object) this.m_aiAnimator.ChildAnimator)
        return;
      this.m_aiAnimator.ChildAnimator.spriteAnimator.AnimationEventTriggered += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.AnimEventTriggered);
    }

    public override void Upkeep()
    {
      base.Upkeep();
      if (this.state != ShootBehavior.State.WaitingForCharge)
        return;
      this.DecrementTimer(ref this.m_chargeTimer);
    }

    public override BehaviorResult Update()
    {
      BehaviorResult behaviorResult = base.Update();
      if (behaviorResult != BehaviorResult.Continue)
        return behaviorResult;
      if (!this.IsReady() || this.RequiresTarget && (UnityEngine.Object) this.m_behaviorSpeculator.TargetRigidbody == (UnityEngine.Object) null)
        return BehaviorResult.Continue;
      if (this.UseVfx && !string.IsNullOrEmpty(this.Vfx))
        this.m_aiAnimator.PlayVfx(this.Vfx);
      if (!this.m_gameObject.activeSelf)
      {
        this.m_gameObject.SetActive(true);
        this.m_beganInactive = true;
      }
      if ((bool) (UnityEngine.Object) this.m_behaviorSpeculator.TargetRigidbody)
        this.m_cachedTargetCenter = this.m_behaviorSpeculator.TargetRigidbody.GetUnitCenter(ColliderType.HitBox);
      if (this.ClearGoop)
        this.SetGoopClearing(true);
      this.state = ShootBehavior.State.Idle;
      if (!string.IsNullOrEmpty(this.ChargeAnimation))
      {
        this.m_aiAnimator.PlayUntilFinished(this.ChargeAnimation, true);
        this.state = ShootBehavior.State.WaitingForCharge;
      }
      else if (!string.IsNullOrEmpty(this.TellAnimation))
      {
        if (!string.IsNullOrEmpty(this.TellAnimation))
          this.m_aiAnimator.PlayUntilCancelled(this.TellAnimation, true);
        else
          this.m_aiAnimator.PlayUntilFinished(this.TellAnimation, true);
        this.state = ShootBehavior.State.WaitingForTell;
        if (this.HideGun && (bool) (UnityEngine.Object) this.m_aiShooter)
          this.m_aiShooter.ToggleGunAndHandRenderers(false, "ShootBulletScript");
      }
      else
        this.Fire();
      if ((double) this.MoveSpeedModifier != 1.0)
      {
        this.m_cachedMovementSpeed = this.m_aiActor.MovementSpeed;
        this.m_aiActor.MovementSpeed *= this.MoveSpeedModifier;
      }
      if (this.LockFacingDirection)
      {
        this.m_aiAnimator.FacingDirection = (this.m_behaviorSpeculator.TargetRigidbody.GetUnitCenter(ColliderType.HitBox) - this.m_specRigidbody.GetUnitCenter(ColliderType.HitBox)).ToAngle();
        this.m_aiAnimator.LockFacingDirection = true;
      }
      if (this.PreventTargetSwitching && (bool) (UnityEngine.Object) this.m_aiActor)
        this.m_aiActor.SuppressTargetSwitch = true;
      this.m_updateEveryFrame = true;
      if (this.OverrideBaseAnims && (bool) (UnityEngine.Object) this.m_aiAnimator)
      {
        if (!string.IsNullOrEmpty(this.OverrideIdleAnim))
          this.m_aiAnimator.OverrideIdleAnimation = this.OverrideIdleAnim;
        if (!string.IsNullOrEmpty(this.OverrideMoveAnim))
          this.m_aiAnimator.OverrideMoveAnimation = this.OverrideMoveAnim;
      }
      return this.StopDuring == ShootBehavior.StopType.None || this.StopDuring == ShootBehavior.StopType.TellOnly ? BehaviorResult.RunContinuousInClass : BehaviorResult.RunContinuous;
    }

    public override ContinuousBehaviorResult ContinuousUpdate()
    {
      int num = (int) base.ContinuousUpdate();
      if ((bool) (UnityEngine.Object) this.m_behaviorSpeculator.TargetRigidbody)
        this.m_cachedTargetCenter = this.m_behaviorSpeculator.TargetRigidbody.GetUnitCenter(ColliderType.HitBox);
      if (this.state == ShootBehavior.State.WaitingForCharge)
      {
        if ((double) this.ChargeTime > 0.0 && (double) this.m_chargeTimer <= 0.0 || (double) this.ChargeTime <= 0.0 && !this.m_aiAnimator.IsPlaying(this.ChargeAnimation))
        {
          if (!string.IsNullOrEmpty(this.TellAnimation))
          {
            this.m_aiAnimator.PlayUntilFinished(this.TellAnimation, true);
            this.state = ShootBehavior.State.WaitingForTell;
          }
          else
            this.Fire();
        }
        return ContinuousBehaviorResult.Continue;
      }
      if (this.state == ShootBehavior.State.WaitingForTell)
      {
        if (this.LockFacingDirection && this.ContinueAimingDuringTell && !this.m_isAimLocked && (bool) (UnityEngine.Object) this.m_behaviorSpeculator.TargetRigidbody)
          this.m_aiAnimator.FacingDirection = (this.m_behaviorSpeculator.TargetRigidbody.GetUnitCenter(ColliderType.HitBox) - this.m_specRigidbody.GetUnitCenter(ColliderType.HitBox)).ToAngle();
        if (!this.m_aiAnimator.IsPlaying(this.TellAnimation))
          this.Fire();
        return ContinuousBehaviorResult.Continue;
      }
      if (this.state == ShootBehavior.State.Firing)
      {
        tk2dSpriteAnimationClip.WrapMode wrapMode;
        if (!this.IsBulletScriptEnded || !string.IsNullOrEmpty(this.TellAnimation) && this.m_aiAnimator.IsPlaying(this.TellAnimation) && this.m_aiAnimator.GetWrapType(this.TellAnimation, out wrapMode) && wrapMode == tk2dSpriteAnimationClip.WrapMode.Once || !string.IsNullOrEmpty(this.FireAnimation) && this.m_aiAnimator.IsPlaying(this.FireAnimation) && this.m_aiAnimator.GetWrapType(this.FireAnimation, out wrapMode) && wrapMode == tk2dSpriteAnimationClip.WrapMode.Once)
          return ContinuousBehaviorResult.Continue;
        if (string.IsNullOrEmpty(this.PostFireAnimation))
          return ContinuousBehaviorResult.Finished;
        this.state = ShootBehavior.State.WaitingForPostAnim;
        this.m_aiAnimator.PlayUntilFinished(this.PostFireAnimation);
        return ContinuousBehaviorResult.Continue;
      }
      return this.state == ShootBehavior.State.WaitingForPostAnim && this.m_aiAnimator.IsPlaying(this.PostFireAnimation) ? ContinuousBehaviorResult.Continue : ContinuousBehaviorResult.Finished;
    }

    public override void EndContinuousUpdate()
    {
      base.EndContinuousUpdate();
      this.CeaseFire();
      if (this.ClearGoop)
        this.SetGoopClearing(false);
      if (this.HideGun && (bool) (UnityEngine.Object) this.m_aiShooter)
        this.m_aiShooter.ToggleGunAndHandRenderers(true, "ShootBulletScript");
      if (!string.IsNullOrEmpty(this.ChargeAnimation))
        this.m_aiAnimator.EndAnimationIf(this.ChargeAnimation);
      if (!string.IsNullOrEmpty(this.TellAnimation))
        this.m_aiAnimator.EndAnimationIf(this.TellAnimation);
      if (!string.IsNullOrEmpty(this.FireAnimation))
        this.m_aiAnimator.EndAnimationIf(this.FireAnimation);
      if (this.UseVfx && !string.IsNullOrEmpty(this.Vfx))
        this.m_aiAnimator.StopVfx(this.Vfx);
      if (this.UseVfx && !string.IsNullOrEmpty(this.ChargeVfx))
        this.m_aiAnimator.StopVfx(this.ChargeVfx);
      if (this.UseVfx && !string.IsNullOrEmpty(this.TellVfx))
        this.m_aiAnimator.StopVfx(this.TellVfx);
      if (this.UseVfx && !string.IsNullOrEmpty(this.FireVfx))
        this.m_aiAnimator.StopVfx(this.FireVfx);
      if (this.EnabledDuringAttack != null)
      {
        for (int index = 0; index < this.EnabledDuringAttack.Length; ++index)
          this.EnabledDuringAttack[index].SetActive(false);
      }
      if (this.m_beganInactive)
      {
        this.m_aiAnimator.gameObject.SetActive(false);
        this.m_beganInactive = false;
      }
      if ((double) this.MoveSpeedModifier != 1.0)
        this.m_aiActor.MovementSpeed = this.m_cachedMovementSpeed;
      if (this.StopDuring == ShootBehavior.StopType.TellOnly)
        this.m_behaviorSpeculator.PreventMovement = false;
      if ((bool) (UnityEngine.Object) this.m_aiActor && this.StopDuring != ShootBehavior.StopType.None && this.ImmobileDuringStop)
        this.m_aiActor.knockbackDoer.SetImmobile(false, "ShootBulletScript");
      if (this.LockFacingDirection)
        this.m_aiAnimator.LockFacingDirection = false;
      if (this.PreventTargetSwitching && (bool) (UnityEngine.Object) this.m_aiActor)
        this.m_aiActor.SuppressTargetSwitch = false;
      if (this.OverrideBaseAnims && (bool) (UnityEngine.Object) this.m_aiAnimator)
      {
        if (!string.IsNullOrEmpty(this.OverrideIdleAnim))
          this.m_aiAnimator.OverrideIdleAnimation = (string) null;
        if (!string.IsNullOrEmpty(this.OverrideMoveAnim))
          this.m_aiAnimator.OverrideMoveAnimation = (string) null;
      }
      this.m_updateEveryFrame = false;
      this.state = ShootBehavior.State.Idle;
      this.UpdateCooldowns();
    }

    public override void Init(GameObject gameObject, AIActor aiActor, AIShooter aiShooter)
    {
      base.Init(gameObject, aiActor, aiShooter);
      this.m_specRigidbody = this.m_behaviorSpeculator.specRigidbody;
      this.m_bulletBank = this.m_behaviorSpeculator.bulletBank;
    }

    public override bool IsOverridable() => !this.Uninterruptible;

    private void Fire()
    {
      if (this.LockFacingDirection && this.ReaimOnFire && (bool) (UnityEngine.Object) this.m_behaviorSpeculator.TargetRigidbody)
        this.m_aiAnimator.FacingDirection = (this.m_behaviorSpeculator.TargetRigidbody.GetUnitCenter(ColliderType.HitBox) - this.m_specRigidbody.GetUnitCenter(ColliderType.HitBox)).ToAngle();
      if (!string.IsNullOrEmpty(this.FireAnimation))
      {
        this.m_aiAnimator.EndAnimation();
        this.m_aiAnimator.PlayUntilFinished(this.FireAnimation);
      }
      if (this.UseVfx && !string.IsNullOrEmpty(this.FireVfx))
        this.m_aiAnimator.PlayVfx(this.FireVfx);
      this.SpawnProjectiles();
      if (this.EnabledDuringAttack != null)
      {
        for (int index = 0; index < this.EnabledDuringAttack.Length; ++index)
          this.EnabledDuringAttack[index].SetActive(true);
      }
      if (this.StopDuring == ShootBehavior.StopType.TellOnly)
      {
        this.m_behaviorSpeculator.PreventMovement = false;
        if ((bool) (UnityEngine.Object) this.m_aiActor && this.ImmobileDuringStop)
          this.m_aiActor.knockbackDoer.SetImmobile(false, "ShootBulletScript");
      }
      else if (this.StopDuring != ShootBehavior.StopType.None)
        this.StopMoving();
      this.state = ShootBehavior.State.Firing;
      if (!this.HideGun || !(bool) (UnityEngine.Object) this.m_aiShooter)
        return;
      this.m_aiShooter.ToggleGunAndHandRenderers(false, "ShootBulletScript");
    }

    private void CeaseFire()
    {
      if (!this.IsBulletScript || !(bool) (UnityEngine.Object) this.m_bulletSource || this.m_bulletSource.IsEnded)
        return;
      this.m_bulletSource.ForceStop();
    }

    private void StopMoving()
    {
      if (!(bool) (UnityEngine.Object) this.m_aiActor)
        return;
      this.m_aiActor.ClearPath();
      if (this.StopDuring == ShootBehavior.StopType.TellOnly)
        this.m_behaviorSpeculator.PreventMovement = true;
      if (!this.ImmobileDuringStop)
        return;
      this.m_aiActor.knockbackDoer.SetImmobile(true, "ShootBulletScript");
    }

    protected override Vector2 GetOrigin(ShootBehavior.TargetAreaOrigin origin)
    {
      return origin == ShootBehavior.TargetAreaOrigin.ShootPoint ? this.ShootPoint.transform.position.XY() : base.GetOrigin(origin);
    }

    private void SpawnProjectiles()
    {
      if (this.IsBulletScript)
      {
        if (!(bool) (UnityEngine.Object) this.m_bulletSource)
          this.m_bulletSource = this.ShootPoint.GetOrAddComponent<BulletScriptSource>();
        this.m_bulletSource.BulletManager = this.m_bulletBank;
        this.m_bulletSource.BulletScript = this.BulletScript;
        this.m_bulletSource.Initialize();
      }
      else
      {
        if (!this.IsSingleBullet)
          return;
        AIBulletBank.Entry bullet = this.m_bulletBank.GetBullet(this.BulletName);
        GameObject bulletObject = bullet.BulletObject;
        Vector2 vector2_1 = this.m_cachedTargetCenter;
        if ((bool) (UnityEngine.Object) this.m_behaviorSpeculator.TargetRigidbody)
          vector2_1 = this.m_behaviorSpeculator.TargetRigidbody.GetUnitCenter(ColliderType.HitBox);
        float direction;
        if (this.ShouldOverrideFireDirection)
        {
          direction = this.OverrideFireDirection;
        }
        else
        {
          if ((double) this.LeadAmount > 0.0)
          {
            Vector2 position = (Vector2) this.ShootPoint.transform.position;
            float? overrideProjectileSpeed = !bullet.OverrideProjectile ? new float?() : new float?(bullet.ProjectileData.speed);
            Vector2 predictedTargetPosition = bulletObject.GetComponent<Projectile>().GetPredictedTargetPosition(vector2_1, this.m_behaviorSpeculator.TargetVelocity, new Vector2?(position), overrideProjectileSpeed);
            vector2_1 = Vector2.Lerp(vector2_1, predictedTargetPosition, this.LeadAmount);
          }
          Vector2 vector2_2 = vector2_1 - this.ShootPoint.transform.position.XY();
          direction = Mathf.Atan2(vector2_2.y, vector2_2.x) * 57.29578f;
        }
        GameObject projectileFromBank = this.m_bulletBank.CreateProjectileFromBank((Vector2) this.ShootPoint.transform.position, direction, this.BulletName);
        if (this.m_bulletBank.OnProjectileCreatedWithSource != null)
          this.m_bulletBank.OnProjectileCreatedWithSource(this.ShootPoint.transform.name, projectileFromBank.GetComponent<Projectile>());
        ArcProjectile component = projectileFromBank.GetComponent<ArcProjectile>();
        if (!(bool) (UnityEngine.Object) component)
          return;
        component.AdjustSpeedToHit(vector2_1);
      }
    }

    private void SetGoopClearing(bool value)
    {
      if (!this.ClearGoop || !(bool) (UnityEngine.Object) this.m_aiActor || !(bool) (UnityEngine.Object) this.m_aiActor.specRigidbody)
        return;
      if (value)
      {
        this.m_goopExceptionId = DeadlyDeadlyGoopManager.RegisterUngoopableCircle(this.m_aiActor.specRigidbody.UnitCenter, 2f);
      }
      else
      {
        if (this.m_goopExceptionId != -1)
          DeadlyDeadlyGoopManager.DeregisterUngoopableCircle(this.m_goopExceptionId);
        this.m_goopExceptionId = -1;
      }
    }

    public bool IsBulletScriptEnded
    {
      get
      {
        if (this.IsBulletScript)
          return this.m_bulletSource.IsEnded;
        return !this.IsSingleBullet || true;
      }
    }

    private ShootBehavior.State state
    {
      get => this.m_state;
      set
      {
        if (this.m_state == value)
          return;
        this.EndState(this.m_state);
        this.m_state = value;
        this.BeginState(this.m_state);
      }
    }

    private void BeginState(ShootBehavior.State state)
    {
      switch (state)
      {
        case ShootBehavior.State.WaitingForCharge:
          if (this.UseVfx && !string.IsNullOrEmpty(this.ChargeVfx))
            this.m_aiAnimator.PlayVfx(this.ChargeVfx);
          if (this.StopDuring == ShootBehavior.StopType.Charge)
            this.StopMoving();
          this.m_chargeTimer = this.ChargeTime;
          break;
        case ShootBehavior.State.WaitingForTell:
          if (this.UseVfx && !string.IsNullOrEmpty(this.TellVfx))
            this.m_aiAnimator.PlayVfx(this.TellVfx);
          if (this.StopDuring == ShootBehavior.StopType.Tell || this.StopDuring == ShootBehavior.StopType.TellOnly)
            this.StopMoving();
          this.m_isAimLocked = false;
          break;
      }
    }

    private void EndState(ShootBehavior.State state)
    {
      switch (state)
      {
        case ShootBehavior.State.WaitingForCharge:
          if (!this.UseVfx || string.IsNullOrEmpty(this.ChargeVfx))
            break;
          this.m_aiAnimator.StopVfx(this.ChargeVfx);
          break;
        case ShootBehavior.State.WaitingForTell:
          if (this.UseVfx && !string.IsNullOrEmpty(this.TellVfx))
            this.m_aiAnimator.StopVfx(this.TellVfx);
          if (!this.OverrideBaseAnims)
            break;
          if (!string.IsNullOrEmpty(this.OverrideIdleAnim))
            this.m_aiAnimator.OverrideIdleAnimation = this.OverrideIdleAnim;
          if (!string.IsNullOrEmpty(this.OverrideMoveAnim))
            this.m_aiAnimator.OverrideMoveAnimation = this.OverrideMoveAnim;
          if (string.IsNullOrEmpty(this.TellAnimation))
            break;
          this.m_aiAnimator.EndAnimationIf(this.TellAnimation);
          break;
        case ShootBehavior.State.Firing:
          if (!this.UseVfx || string.IsNullOrEmpty(this.FireVfx))
            break;
          this.m_aiAnimator.StopVfx(this.FireVfx);
          break;
      }
    }

    private void AnimEventTriggered(
      tk2dSpriteAnimator sprite,
      tk2dSpriteAnimationClip clip,
      int frameNum)
    {
      tk2dSpriteAnimationFrame frame = clip.GetFrame(frameNum);
      bool flag = this.state == ShootBehavior.State.WaitingForTell;
      if (this.MultipleFireEvents)
        flag |= this.state == ShootBehavior.State.Firing;
      if (flag && frame.eventInfo == "fire")
        this.Fire();
      if (!this.LockFacingDirection || !this.ContinueAimingDuringTell || !(frame.eventInfo == "stopAiming"))
        return;
      this.m_isAimLocked = true;
    }

    public enum StopType
    {
      None,
      Tell,
      Attack,
      Charge,
      TellOnly,
    }

    private enum State
    {
      Idle,
      WaitingForCharge,
      WaitingForTell,
      Firing,
      WaitingForPostAnim,
    }

    public enum TargetAreaOrigin
    {
      HitboxCenter,
      ShootPoint,
    }

    public abstract class FiringAreaStyle
    {
      public ShootBehavior.TargetAreaOrigin targetAreaOrigin;

      public abstract bool TargetInFiringArea(Vector2 origin, Vector2 targetCenter);

      public abstract void DrawDebugLines(Vector2 origin, Vector2 targetCenter, AIActor actor);
    }

    public class ArcFiringArea : ShootBehavior.FiringAreaStyle
    {
      public float StartAngle;
      public float SweepAngle;

      public override bool TargetInFiringArea(Vector2 origin, Vector2 targetCenter)
      {
        return BraveMathCollege.IsAngleWithinSweepArea((targetCenter - origin).ToAngle(), this.StartAngle, this.SweepAngle);
      }

      public override void DrawDebugLines(Vector2 origin, Vector2 targetCenter, AIActor actor)
      {
        ++BasicAttackBehavior.m_arcCount;
      }
    }

    public class RectFiringArea : ShootBehavior.FiringAreaStyle
    {
      public Vector2 AreaOriginOffset;
      public Vector2 AreaDimensions;

      private Vector2 offset
      {
        get
        {
          Vector2 areaOriginOffset = this.AreaOriginOffset;
          if ((double) this.AreaDimensions.x < 0.0)
            areaOriginOffset.x += this.AreaDimensions.x;
          if ((double) this.AreaDimensions.y < 0.0)
            areaOriginOffset.y += this.AreaDimensions.y;
          return areaOriginOffset;
        }
      }

      private Vector2 dimensions
      {
        get => new Vector2(Mathf.Abs(this.AreaDimensions.x), Mathf.Abs(this.AreaDimensions.y));
      }

      public override bool TargetInFiringArea(Vector2 origin, Vector2 targetCenter)
      {
        origin += this.offset;
        return ((double) targetCenter.x < (double) origin.x || (double) targetCenter.x > (double) origin.x + (double) this.dimensions.x || (double) targetCenter.y < (double) origin.y ? 1 : ((double) targetCenter.y > (double) origin.y + (double) this.dimensions.y ? 1 : 0)) == 0;
      }

      public override void DrawDebugLines(Vector2 origin, Vector2 targetCenter, AIActor actor)
      {
        origin += this.offset;
      }
    }
  }

