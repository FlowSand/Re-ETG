// Decompiled with JetBrains decompiler
// Type: SackKnightAttackBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using UnityEngine;

#nullable disable

public class SackKnightAttackBehavior : AttackBehaviorBase
  {
    public float maxAttackDistance = 1f;
    public float minAttackDistance = 0.1f;
    public float SquireAttackDamage = 3f;
    public float HedgeKnightAttackDamage = 5f;
    public float KnightAttackDamage = 7f;
    public float KnightLieutenantAttackDamage = 7f;
    public float KnightCommanderAttackDamage = 7f;
    public float HolyKnightAttackDamage = 7f;
    public float MechAttackDamage = 20f;
    public float AngelicKnightAttackDuration = 5f;
    public float AngelicKnightAngleVariance = 30f;
    public float SquireCooldownTime = 3f;
    public float HedgeKnightCooldownTime = 1.75f;
    public float KnightCooldownTime = 0.5f;
    public float KnightLieutenantCooldownTime = 0.5f;
    public float KnightCommanderCooldownTime = 2f;
    public float HolyKnightCooldownTime = 2f;
    public float AngelicKnightCooldownTime = 1f;
    public float AngelicKnightDesiredDistance = 6f;
    public float MechCooldownTime = 2f;
    public float MechGunWeight = 1f;
    public float MechRocketWeight = 1f;
    public float MechSwordWeight = 1f;
    public string SwordHitVFX;
    public GameActorHealthEffect PoisonEffectForTrashSynergy;
    private SackKnightAttackBehavior.MechaJunkanAttackType m_mechAttack;
    private float m_angelShootElapsed;
    private float m_angelElapsed;
    private SeekTargetBehavior m_seekBehavior;
    private SackKnightController m_knight;
    private float m_elapsed;
    private int m_attackCounter;
    private float m_cooldownTimer;
    private SackKnightAttackBehavior.State m_state;
    private bool m_isTargetPitBoss;

    private float CurrentFormCooldown
    {
      get
      {
        switch (this.m_knight.CurrentForm)
        {
          case SackKnightController.SackKnightPhase.PEASANT:
          case SackKnightController.SackKnightPhase.SQUIRE:
            return this.SquireCooldownTime;
          case SackKnightController.SackKnightPhase.HEDGE_KNIGHT:
            return this.HedgeKnightCooldownTime;
          case SackKnightController.SackKnightPhase.KNIGHT:
            return this.KnightCooldownTime;
          case SackKnightController.SackKnightPhase.KNIGHT_LIEUTENANT:
            return this.KnightLieutenantCooldownTime;
          case SackKnightController.SackKnightPhase.KNIGHT_COMMANDER:
            return this.KnightCommanderCooldownTime;
          case SackKnightController.SackKnightPhase.HOLY_KNIGHT:
            return this.HolyKnightCooldownTime;
          case SackKnightController.SackKnightPhase.ANGELIC_KNIGHT:
            return this.AngelicKnightCooldownTime;
          case SackKnightController.SackKnightPhase.MECHAJUNKAN:
            return this.MechCooldownTime;
          default:
            return this.SquireCooldownTime;
        }
      }
    }

    public override void Start()
    {
      base.Start();
      this.m_knight = this.m_aiActor.GetComponent<SackKnightController>();
      BehaviorSpeculator behaviorSpeculator = this.m_aiActor.behaviorSpeculator;
      for (int index = 0; index < behaviorSpeculator.MovementBehaviors.Count; ++index)
      {
        if (behaviorSpeculator.MovementBehaviors[index] is SeekTargetBehavior)
          this.m_seekBehavior = behaviorSpeculator.MovementBehaviors[index] as SeekTargetBehavior;
      }
    }

    public override BehaviorResult Update()
    {
      int num1 = (int) base.Update();
      SpeculativeRigidbody targetRigidbody = this.m_aiActor.TargetRigidbody;
      this.m_isTargetPitBoss = (bool) (UnityEngine.Object) targetRigidbody && (bool) (UnityEngine.Object) targetRigidbody.aiActor && (bool) (UnityEngine.Object) targetRigidbody.healthHaver && targetRigidbody.healthHaver.IsBoss && GameManager.Instance.Dungeon.CellSupportsFalling((Vector3) targetRigidbody.UnitCenter);
      if (this.m_knight.CurrentForm == SackKnightController.SackKnightPhase.MECHAJUNKAN)
      {
        if (this.m_isTargetPitBoss)
        {
          this.minAttackDistance = 0.1f;
          this.maxAttackDistance = this.m_mechAttack != SackKnightAttackBehavior.MechaJunkanAttackType.SWORD ? 12f : 2.5f;
        }
        else
        {
          this.minAttackDistance = 0.1f;
          this.maxAttackDistance = this.m_mechAttack != SackKnightAttackBehavior.MechaJunkanAttackType.SWORD ? 12f : 1.5f;
        }
      }
      else if (this.m_knight.CurrentForm == SackKnightController.SackKnightPhase.ANGELIC_KNIGHT)
      {
        this.minAttackDistance = 0.1f;
        this.maxAttackDistance = 12f;
      }
      else if (this.m_isTargetPitBoss)
      {
        this.minAttackDistance = 0.1f;
        this.maxAttackDistance = 2f;
      }
      else
      {
        this.minAttackDistance = 0.1f;
        this.maxAttackDistance = 1f;
      }
      this.DecrementTimer(ref this.m_cooldownTimer);
      if (this.m_seekBehavior != null)
      {
        if (this.m_knight.CurrentForm == SackKnightController.SackKnightPhase.ANGELIC_KNIGHT)
        {
          this.m_seekBehavior.ExternalCooldownSource = false;
          this.m_seekBehavior.StopWhenInRange = true;
          this.m_seekBehavior.CustomRange = this.AngelicKnightDesiredDistance;
        }
        else
        {
          this.m_seekBehavior.ExternalCooldownSource = (double) this.m_cooldownTimer > 0.0;
          this.m_seekBehavior.StopWhenInRange = false;
          this.m_seekBehavior.CustomRange = -1f;
        }
      }
      BehaviorResult behaviorResult = base.Update();
      if (behaviorResult != BehaviorResult.Continue)
        return behaviorResult;
      if ((UnityEngine.Object) this.m_knight == (UnityEngine.Object) null || this.m_knight.CurrentForm == SackKnightController.SackKnightPhase.PEASANT || (double) this.m_cooldownTimer > 0.0 || !(bool) (UnityEngine.Object) this.m_aiActor.TargetRigidbody)
        return BehaviorResult.Continue;
      Vector2 unitCenter = this.m_aiActor.specRigidbody.UnitCenter;
      Vector2 targetPoint = this.GetTargetPoint(this.m_aiActor.TargetRigidbody, unitCenter);
      float num2 = Vector2.Distance(unitCenter, targetPoint);
      bool flag = this.m_knight.CurrentForm != SackKnightController.SackKnightPhase.ANGELIC_KNIGHT || this.m_aiActor.HasLineOfSightToTarget;
      if ((double) num2 >= (double) this.maxAttackDistance || !flag)
        return BehaviorResult.Continue;
      this.m_state = SackKnightAttackBehavior.State.Charging;
      if (this.m_knight.CurrentForm != SackKnightController.SackKnightPhase.ANGELIC_KNIGHT)
      {
        this.m_aiActor.ClearPath();
        this.m_aiActor.BehaviorOverridesVelocity = true;
        this.m_aiActor.BehaviorVelocity = Vector2.zero;
      }
      this.m_updateEveryFrame = true;
      this.m_elapsed = 0.0f;
      this.m_attackCounter = 0;
      return this.m_knight.CurrentForm == SackKnightController.SackKnightPhase.ANGELIC_KNIGHT ? BehaviorResult.RunContinuousInClass : BehaviorResult.RunContinuous;
    }

    private Vector2 GetTargetPoint(SpeculativeRigidbody targetRigidbody, Vector2 myCenter)
    {
      PixelCollider hitboxPixelCollider = targetRigidbody.HitboxPixelCollider;
      return BraveMathCollege.ClosestPointOnRectangle(myCenter, hitboxPixelCollider.UnitBottomLeft, hitboxPixelCollider.UnitDimensions);
    }

    private ContinuousBehaviorResult DoMechBlasters()
    {
      this.m_angelElapsed += BraveTime.DeltaTime;
      this.m_angelShootElapsed += BraveTime.DeltaTime;
      if (!this.m_aiAnimator.IsPlaying("fire"))
        this.m_aiAnimator.PlayUntilCancelled("fire", true);
      if ((double) this.m_angelShootElapsed > 0.10000000149011612)
      {
        if ((bool) (UnityEngine.Object) this.m_aiActor.TargetRigidbody)
        {
          float direction = BraveMathCollege.Atan2Degrees(this.m_aiActor.TargetRigidbody.UnitCenter - this.m_aiActor.CenterPosition);
          this.m_aiAnimator.LockFacingDirection = true;
          this.m_aiAnimator.FacingDirection = direction;
          GameObject projectileFromBank = this.m_aiActor.bulletBank.CreateProjectileFromBank((Vector2) this.m_aiActor.transform.Find("gun").position, direction, "blaster");
          this.m_aiAnimator.PlayVfx("mechGunVFX", new Vector2?((double) BraveMathCollege.AbsAngleBetween(this.m_aiAnimator.FacingDirection, 0.0f) <= 90.0 ? new Vector2(1f, 0.0f) : new Vector2(-1f, 0.0f)));
          if ((bool) (UnityEngine.Object) projectileFromBank && (bool) (UnityEngine.Object) projectileFromBank.GetComponent<Projectile>() && (bool) (UnityEngine.Object) this.m_aiShooter && this.m_aiShooter.PostProcessProjectile != null)
            this.m_aiShooter.PostProcessProjectile(projectileFromBank.GetComponent<Projectile>());
          int num1 = (int) AkSoundEngine.SetSwitch("WPN_Guns", "Sack", this.m_knight.gameObject);
          int num2 = (int) AkSoundEngine.PostEvent("Play_WPN_gun_shot_01", this.m_knight.gameObject);
        }
        else
          this.m_aiAnimator.LockFacingDirection = false;
        this.m_angelShootElapsed -= 0.1f;
      }
      if ((double) this.m_angelElapsed < 2.0)
        return ContinuousBehaviorResult.Continue;
      this.m_cooldownTimer = this.CurrentFormCooldown;
      this.m_state = SackKnightAttackBehavior.State.Idle;
      this.m_aiAnimator.EndAnimationIf("fire");
      return ContinuousBehaviorResult.Finished;
    }

    private ContinuousBehaviorResult DoMechRockets()
    {
      if (this.m_state == SackKnightAttackBehavior.State.Charging)
      {
        this.m_state = SackKnightAttackBehavior.State.Leaping;
        if (!(bool) (UnityEngine.Object) this.m_aiActor.TargetRigidbody || !this.m_aiActor.TargetRigidbody.enabled)
        {
          this.m_state = SackKnightAttackBehavior.State.Idle;
          this.m_aiAnimator.LockFacingDirection = false;
          return ContinuousBehaviorResult.Finished;
        }
        Vector2 unitCenter = this.m_aiActor.specRigidbody.UnitCenter;
        Vector2 targetPoint = this.GetTargetPoint(this.m_aiActor.TargetRigidbody, unitCenter);
        if ((double) Vector2.Distance(unitCenter, targetPoint) > (double) this.maxAttackDistance)
        {
          Vector2 b = unitCenter + (targetPoint - unitCenter).normalized * this.maxAttackDistance;
          Vector2.Distance(unitCenter, b);
        }
        this.m_aiActor.ClearPath();
        this.m_aiActor.BehaviorOverridesVelocity = true;
        this.m_aiActor.BehaviorVelocity = Vector2.zero;
        this.m_aiAnimator.LockFacingDirection = true;
        this.m_aiAnimator.PlayUntilFinished("rocket", true);
      }
      else if (this.m_state == SackKnightAttackBehavior.State.Leaping)
      {
        this.m_elapsed += this.m_deltaTime;
        float num1 = 1f;
        this.m_angelShootElapsed += BraveTime.DeltaTime;
        if ((double) this.m_angelShootElapsed > 0.10000000149011612)
        {
          if ((bool) (UnityEngine.Object) this.m_aiActor.TargetRigidbody)
          {
            float direction = BraveMathCollege.Atan2Degrees(this.m_aiActor.TargetRigidbody.UnitCenter - this.m_aiActor.CenterPosition) + UnityEngine.Random.Range(-this.AngelicKnightAngleVariance, this.AngelicKnightAngleVariance);
            GameObject projectileFromBank = this.m_aiActor.bulletBank.CreateProjectileFromBank(this.m_aiActor.CenterPosition + new Vector2(UnityEngine.Random.Range(-0.25f, 0.25f), 0.75f), direction, "mechRocket");
            if ((bool) (UnityEngine.Object) projectileFromBank)
            {
              RobotechProjectile component = projectileFromBank.GetComponent<RobotechProjectile>();
              component.Owner = (GameActor) this.m_aiActor.CompanionOwner;
              Vector2 dirVec = (Vector2) (Quaternion.Euler(0.0f, 0.0f, (float) UnityEngine.Random.Range(-25, 25)) * (Vector3) Vector2.up);
              component.ForceCurveDirection(dirVec, UnityEngine.Random.Range(0.04f, 0.06f));
              component.Ramp(4f, 0.5f);
              if ((bool) (UnityEngine.Object) this.m_aiShooter && this.m_aiShooter.PostProcessProjectile != null)
                this.m_aiShooter.PostProcessProjectile(projectileFromBank.GetComponent<Projectile>());
            }
            int num2 = (int) AkSoundEngine.SetSwitch("WPN_Guns", "Sack", this.m_knight.gameObject);
            int num3 = (int) AkSoundEngine.PostEvent("Play_WPN_gun_shot_01", this.m_knight.gameObject);
          }
          this.m_angelShootElapsed -= 0.1f;
        }
        if ((double) this.m_elapsed >= (double) num1)
        {
          this.m_cooldownTimer = this.CurrentFormCooldown;
          this.m_aiAnimator.LockFacingDirection = false;
          return ContinuousBehaviorResult.Finished;
        }
      }
      return ContinuousBehaviorResult.Continue;
    }

    public override ContinuousBehaviorResult ContinuousUpdate()
    {
      if (this.m_knight.CurrentForm == SackKnightController.SackKnightPhase.MECHAJUNKAN)
      {
        if (this.m_mechAttack == SackKnightAttackBehavior.MechaJunkanAttackType.GUN)
          return this.DoMechBlasters();
        if (this.m_mechAttack == SackKnightAttackBehavior.MechaJunkanAttackType.ROCKET)
          return this.DoMechRockets();
      }
      if (this.m_knight.CurrentForm == SackKnightController.SackKnightPhase.ANGELIC_KNIGHT)
      {
        this.HandleAngelAttackFrame();
        if ((double) this.m_angelElapsed >= (double) this.AngelicKnightCooldownTime)
        {
          this.m_cooldownTimer = this.CurrentFormCooldown;
          this.m_state = SackKnightAttackBehavior.State.Idle;
          this.m_aiAnimator.EndAnimationIf("attack");
          return ContinuousBehaviorResult.Finished;
        }
      }
      else
      {
        if (this.m_knight.CurrentForm == SackKnightController.SackKnightPhase.MECHAJUNKAN)
          this.m_aiAnimator.LockFacingDirection = true;
        if (this.m_state == SackKnightAttackBehavior.State.Charging)
        {
          this.m_state = SackKnightAttackBehavior.State.Leaping;
          if (!(bool) (UnityEngine.Object) this.m_aiActor.TargetRigidbody || !this.m_aiActor.TargetRigidbody.enabled)
          {
            this.m_state = SackKnightAttackBehavior.State.Idle;
            this.m_aiAnimator.LockFacingDirection = false;
            return ContinuousBehaviorResult.Finished;
          }
          Vector2 unitCenter = this.m_aiActor.specRigidbody.UnitCenter;
          Vector2 b = this.GetTargetPoint(this.m_aiActor.TargetRigidbody, unitCenter);
          float num = Vector2.Distance(unitCenter, b);
          if ((double) num > (double) this.maxAttackDistance)
          {
            b = unitCenter + (b - unitCenter).normalized * this.maxAttackDistance;
            num = Vector2.Distance(unitCenter, b);
          }
          this.m_aiActor.ClearPath();
          this.m_aiActor.BehaviorOverridesVelocity = true;
          this.m_aiActor.BehaviorVelocity = (b - unitCenter).normalized * (num / 0.25f);
          float angle = this.m_aiActor.BehaviorVelocity.ToAngle();
          this.m_aiAnimator.LockFacingDirection = true;
          this.m_aiAnimator.FacingDirection = angle;
          if (this.m_isTargetPitBoss)
            this.m_aiActor.BehaviorVelocity = Vector2.zero;
          this.m_aiActor.PathableTiles = CellTypes.FLOOR | CellTypes.PIT;
          this.m_aiActor.DoDustUps = false;
          this.m_aiAnimator.PlayUntilFinished("attack", true);
          if (this.m_knight.CurrentForm == SackKnightController.SackKnightPhase.MECHAJUNKAN)
          {
            string str1 = (double) BraveMathCollege.AbsAngleBetween(this.m_aiAnimator.FacingDirection, 0.0f) <= 90.0 ? "mechSwordR" : "mechSwordL";
            AIAnimator aiAnimator = this.m_aiAnimator;
            string str2 = str1;
            Vector2? nullable = new Vector2?(this.m_knight.transform.position.XY());
            string name = str2;
            Vector2? sourceNormal = new Vector2?();
            Vector2? sourceVelocity = new Vector2?();
            Vector2? position = nullable;
            aiAnimator.PlayVfx(name, sourceNormal, sourceVelocity, position);
          }
        }
        else if (this.m_state == SackKnightAttackBehavior.State.Leaping)
        {
          this.m_elapsed += this.m_deltaTime;
          float num = 0.25f;
          if (this.m_knight.CurrentForm == SackKnightController.SackKnightPhase.MECHAJUNKAN)
            num = 0.4f;
          if (this.m_knight.CurrentForm == SackKnightController.SackKnightPhase.KNIGHT_COMMANDER || this.m_knight.CurrentForm == SackKnightController.SackKnightPhase.HOLY_KNIGHT || this.m_knight.CurrentForm == SackKnightController.SackKnightPhase.ANGELIC_KNIGHT)
          {
            num = 1.2f;
            if ((double) this.m_elapsed >= 0.7 && this.m_attackCounter < 1)
            {
              this.m_attackCounter = 1;
              this.DoAttack();
            }
            if ((double) this.m_elapsed >= 0.949999988079071 && this.m_attackCounter < 2)
            {
              this.m_attackCounter = 2;
              this.DoAttack();
            }
          }
          if ((double) this.m_elapsed >= (double) num)
          {
            this.m_cooldownTimer = this.CurrentFormCooldown;
            this.m_aiAnimator.LockFacingDirection = false;
            return ContinuousBehaviorResult.Finished;
          }
        }
      }
      return ContinuousBehaviorResult.Continue;
    }

    private void HandleAngelAttackFrame()
    {
      this.m_angelElapsed += BraveTime.DeltaTime;
      this.m_angelShootElapsed += BraveTime.DeltaTime;
      if (!this.m_aiAnimator.IsPlaying("attack"))
        this.m_aiAnimator.PlayUntilCancelled("attack", true);
      if ((double) this.m_angelShootElapsed <= 0.10000000149011612)
        return;
      if ((bool) (UnityEngine.Object) this.m_aiActor.TargetRigidbody)
      {
        float num1 = BraveMathCollege.Atan2Degrees(this.m_aiActor.TargetRigidbody.UnitCenter - this.m_aiActor.CenterPosition);
        this.m_aiAnimator.LockFacingDirection = true;
        this.m_aiAnimator.FacingDirection = num1;
        float direction = num1 + UnityEngine.Random.Range(-this.AngelicKnightAngleVariance, this.AngelicKnightAngleVariance);
        GameObject projectileFromBank = this.m_aiActor.bulletBank.CreateProjectileFromBank((Vector2) (this.m_aiActor.bulletBank.GetTransform((double) BraveMathCollege.AbsAngleBetween(this.m_aiAnimator.FacingDirection, 0.0f) >= 90.0 ? "left shoot point" : "right shoot point").position + new Vector3(0.0f, (float) UnityEngine.Random.Range(-3, 4) / 16f)), direction, "angel");
        if ((bool) (UnityEngine.Object) projectileFromBank && (bool) (UnityEngine.Object) projectileFromBank.GetComponent<Projectile>() && (bool) (UnityEngine.Object) this.m_aiShooter && this.m_aiShooter.PostProcessProjectile != null)
          this.m_aiShooter.PostProcessProjectile(projectileFromBank.GetComponent<Projectile>());
        int num2 = (int) AkSoundEngine.SetSwitch("WPN_Guns", "Sack", this.m_knight.gameObject);
        int num3 = (int) AkSoundEngine.PostEvent("Play_WPN_gun_shot_01", this.m_knight.gameObject);
      }
      else
        this.m_aiAnimator.LockFacingDirection = false;
      this.m_angelShootElapsed -= 0.1f;
    }

    private void DoAttack()
    {
      SpeculativeRigidbody targetRigidbody = this.m_aiActor.TargetRigidbody;
      if (!(bool) (UnityEngine.Object) targetRigidbody || !targetRigidbody.enabled || !targetRigidbody.CollideWithOthers || !(bool) (UnityEngine.Object) targetRigidbody.healthHaver || (bool) (UnityEngine.Object) targetRigidbody.aiActor && targetRigidbody.aiActor.IsGone)
        return;
      float damage;
      switch (this.m_knight.CurrentForm)
      {
        case SackKnightController.SackKnightPhase.PEASANT:
        case SackKnightController.SackKnightPhase.SQUIRE:
          damage = this.SquireAttackDamage;
          break;
        case SackKnightController.SackKnightPhase.HEDGE_KNIGHT:
          damage = this.HedgeKnightAttackDamage;
          break;
        case SackKnightController.SackKnightPhase.KNIGHT:
          damage = this.KnightAttackDamage;
          break;
        case SackKnightController.SackKnightPhase.KNIGHT_LIEUTENANT:
          damage = this.KnightLieutenantAttackDamage;
          break;
        case SackKnightController.SackKnightPhase.KNIGHT_COMMANDER:
          damage = this.KnightCommanderAttackDamage / 3f;
          break;
        case SackKnightController.SackKnightPhase.HOLY_KNIGHT:
          damage = this.HolyKnightAttackDamage / 3f;
          break;
        case SackKnightController.SackKnightPhase.MECHAJUNKAN:
          damage = this.MechAttackDamage;
          break;
        default:
          damage = this.SquireAttackDamage;
          break;
      }
      if ((bool) (UnityEngine.Object) this.m_aiActor.CompanionOwner && PassiveItem.IsFlagSetForCharacter(this.m_aiActor.CompanionOwner, typeof (BattleStandardItem)))
        damage *= BattleStandardItem.BattleStandardCompanionDamageMultiplier;
      if (this.m_knight.CurrentForm == SackKnightController.SackKnightPhase.KNIGHT_COMMANDER || this.m_knight.CurrentForm == SackKnightController.SackKnightPhase.HOLY_KNIGHT || this.m_knight.CurrentForm == SackKnightController.SackKnightPhase.ANGELIC_KNIGHT)
      {
        VFXPool hitVFX = (VFXPool) null;
        if (!string.IsNullOrEmpty(this.SwordHitVFX))
        {
          AIAnimator.NamedVFXPool namedVfxPool = this.m_aiAnimator.OtherVFX.Find((Predicate<AIAnimator.NamedVFXPool>) (vfx => vfx.name == this.SwordHitVFX));
          if (namedVfxPool != null)
            hitVFX = namedVfxPool.vfxPool;
        }
        Exploder.DoRadialDamage(damage, (Vector3) this.m_aiActor.specRigidbody.UnitCenter, 2.5f, false, true, this.m_knight.CurrentForm == SackKnightController.SackKnightPhase.ANGELIC_KNIGHT, hitVFX);
      }
      else
      {
        targetRigidbody.healthHaver.ApplyDamage(damage, this.m_aiActor.specRigidbody.Velocity, "Ser Junkan");
        if (this.m_aiActor.CompanionOwner.HasActiveBonusSynergy(CustomSynergyType.TRASHJUNKAN) && (bool) (UnityEngine.Object) targetRigidbody.aiActor)
          targetRigidbody.aiActor.ApplyEffect((GameActorEffect) this.PoisonEffectForTrashSynergy);
        if (string.IsNullOrEmpty(this.SwordHitVFX))
          return;
        PixelCollider pixelCollider = targetRigidbody.GetPixelCollider(ColliderType.HitBox);
        Vector2 vector2_1 = BraveMathCollege.ClosestPointOnRectangle(this.m_aiActor.CenterPosition, pixelCollider.UnitBottomLeft, pixelCollider.UnitDimensions);
        Vector2 vector2_2 = vector2_1 - this.m_aiActor.CenterPosition;
        if (vector2_2 != Vector2.zero)
          vector2_1 += vector2_2.normalized * (3f / 16f);
        AIAnimator aiAnimator = this.m_aiAnimator;
        string swordHitVfx = this.SwordHitVFX;
        Vector2? nullable = new Vector2?(vector2_1);
        string name = swordHitVfx;
        Vector2? sourceNormal = new Vector2?();
        Vector2? sourceVelocity = new Vector2?();
        Vector2? position = nullable;
        aiAnimator.PlayVfx(name, sourceNormal, sourceVelocity, position);
      }
    }

    public override void EndContinuousUpdate()
    {
      base.EndContinuousUpdate();
      this.m_angelShootElapsed = 0.0f;
      this.m_angelElapsed = 0.0f;
      if (this.m_knight.CurrentForm != SackKnightController.SackKnightPhase.ANGELIC_KNIGHT)
      {
        if ((this.m_knight.CurrentForm == SackKnightController.SackKnightPhase.KNIGHT_COMMANDER || this.m_knight.CurrentForm == SackKnightController.SackKnightPhase.HOLY_KNIGHT) && this.m_attackCounter < 1)
          this.DoAttack();
        if ((this.m_knight.CurrentForm == SackKnightController.SackKnightPhase.KNIGHT_COMMANDER || this.m_knight.CurrentForm == SackKnightController.SackKnightPhase.HOLY_KNIGHT) && this.m_attackCounter < 2)
          this.DoAttack();
        this.DoAttack();
      }
      else
        this.m_aiAnimator.EndAnimation();
      if (this.m_knight.CurrentForm == SackKnightController.SackKnightPhase.MECHAJUNKAN)
        this.m_mechAttack = this.SelectNewMechAttack();
      this.m_state = SackKnightAttackBehavior.State.Idle;
      if (!this.m_aiActor.IsFlying)
        this.m_aiActor.PathableTiles = CellTypes.FLOOR;
      this.m_aiActor.DoDustUps = true;
      this.m_aiActor.BehaviorOverridesVelocity = false;
      this.m_aiAnimator.LockFacingDirection = false;
      this.m_updateEveryFrame = false;
    }

    private SackKnightAttackBehavior.MechaJunkanAttackType SelectNewMechAttack()
    {
      float num = UnityEngine.Random.value * (this.MechGunWeight + this.MechRocketWeight + this.MechSwordWeight);
      if ((double) num < (double) this.MechGunWeight)
        return SackKnightAttackBehavior.MechaJunkanAttackType.GUN;
      return (double) num < (double) this.MechGunWeight + (double) this.MechRocketWeight ? SackKnightAttackBehavior.MechaJunkanAttackType.ROCKET : SackKnightAttackBehavior.MechaJunkanAttackType.SWORD;
    }

    public override bool IsReady() => true;

    public override float GetMinReadyRange() => this.maxAttackDistance;

    public override float GetMaxRange() => this.maxAttackDistance;

    private enum MechaJunkanAttackType
    {
      SWORD,
      GUN,
      ROCKET,
    }

    private enum State
    {
      Idle,
      Charging,
      Leaping,
    }
  }

