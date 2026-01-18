using Dungeonator;
using FullInspector;
using UnityEngine;

#nullable disable

  public abstract class BasicAttackBehavior : AttackBehaviorBase
  {
    public static bool DrawDebugFiringArea;
[InspectorCategory("Conditions")]
[InspectorTooltip("Time before THIS behavior may be run again.")]
    public float Cooldown = 1f;
[InspectorTooltip("Time variance added to the base cooldown.")]
[InspectorCategory("Conditions")]
    public float CooldownVariance;
[InspectorCategory("Conditions")]
[InspectorTooltip("Time before ATTACK behaviors may be run again.")]
    public float AttackCooldown;
[InspectorTooltip("Time before ANY behavior may be run again.")]
[InspectorCategory("Conditions")]
    public float GlobalCooldown;
[InspectorCategory("Conditions")]
[InspectorTooltip("Time after the enemy becomes active before this attack can be used for the first time.")]
    public float InitialCooldown;
[InspectorTooltip("Time variance added to the initial cooldown.")]
[InspectorCategory("Conditions")]
    public float InitialCooldownVariance;
[InspectorCategory("Conditions")]
[InspectorTooltip("Name of the cooldown group to use; all behaviors on this BehaviorSpeculator with a matching group will use this cooldown value.")]
[InspectorShowIf("ShowGroupCooldown")]
    public string GroupName;
[InspectorTooltip("Time before any behaviors with a matching group name may be run again.")]
[InspectorShowIf("ShowGroupCooldown")]
[InspectorCategory("Conditions")]
    public float GroupCooldown;
[InspectorCategory("Conditions")]
[InspectorTooltip("Minimum range")]
    public float MinRange;
[InspectorTooltip("Range")]
[InspectorCategory("Conditions")]
    public float Range;
[InspectorTooltip("Minimum distance from a wall")]
[InspectorCategory("Conditions")]
    public float MinWallDistance;
[InspectorCategory("Conditions")]
[InspectorTooltip("If the room contains more than this number of enemies, this attack wont be used.")]
    public float MaxEnemiesInRoom;
[InspectorShowIf("ShowMinHealthThreshold")]
[InspectorCategory("Conditions")]
[InspectorTooltip("The minimum amount of health an enemy can have and still use this attack.\n(Raising this means the enemy wont use this attack at low health)")]
    public float MinHealthThreshold;
[InspectorTooltip("The maximum amount of health an enemy can have and still use this attack.\n(Lowering this means the enemy wont use this attack until they lose health)")]
[InspectorShowIf("ShowMaxHealthThreshold")]
[InspectorCategory("Conditions")]
    public float MaxHealthThreshold = 1f;
[InspectorCategory("Conditions")]
[InspectorTooltip("The attack can only be used once each time a new health threshold is met")]
[InspectorShowIf("ShowHealthThresholds")]
    public float[] HealthThresholds = new float[0];
[InspectorTooltip("If true, the attack can build up multiple uses by passing multiple thresholds in quick succession")]
[InspectorCategory("Conditions")]
[InspectorShowIf("ShowHealthThresholds")]
    public bool AccumulateHealthThresholds = true;
[InspectorCategory("Conditions")]
[InspectorShowIf("ShowTargetArea")]
    public ShootBehavior.FiringAreaStyle targetAreaStyle;
[InspectorCategory("Conditions")]
[InspectorTooltip("The attack can only be used for Black Phantom versions of this enemy")]
    public bool IsBlackPhantom;
[InspectorShowIf("ShowResetCooldownOnDamage")]
[InspectorCategory("Conditions")]
[InspectorTooltip("Resets the appropriate cooldowns when the actor takes damage.")]
[InspectorNullable]
    public BasicAttackBehavior.ResetCooldownOnDamage resetCooldownOnDamage;
[InspectorCategory("Conditions")]
[InspectorTooltip("Require line of sight to target. Expensive! Use for companions.")]
    public bool RequiresLineOfSight;
[InspectorTooltip("This attack can only be used this number of times.")]
[InspectorCategory("Conditions")]
    public int MaxUsages;
    protected float m_cooldownTimer;
    protected float m_resetCooldownOnDamageCooldown;
    protected BehaviorSpeculator m_behaviorSpeculator;
    protected int m_healthThresholdCredits;
    protected float m_lowestRecordedHealthPercentage = float.MaxValue;
    protected int m_numTimesUsed;
    protected static int m_arcCount;
    protected static int m_lastFrame;

    private bool ShowGroupCooldown()
    {
      return (double) this.GroupCooldown > 0.0 || !string.IsNullOrEmpty(this.GroupName);
    }

    private bool ShowMinHealthThreshold() => (double) this.MinHealthThreshold != 0.0;

    private bool ShowMaxHealthThreshold() => (double) this.MaxHealthThreshold != 1.0;

    private bool ShowHealthThresholds() => this.HealthThresholds.Length > 0;

    private bool ShowTargetArea() => this.targetAreaStyle != null;

    private bool ShowResetCooldownOnDamage() => this.resetCooldownOnDamage != null;

    public override void Start()
    {
      base.Start();
      this.m_cooldownTimer = this.InitialCooldown;
      if ((double) this.InitialCooldownVariance <= 0.0)
        return;
      this.m_cooldownTimer += Random.Range(-this.InitialCooldownVariance, this.InitialCooldownVariance);
    }

    public override void Upkeep()
    {
      base.Upkeep();
      this.DecrementTimer(ref this.m_cooldownTimer, true);
      this.DecrementTimer(ref this.m_resetCooldownOnDamageCooldown, true);
      if (this.HealthThresholds.Length > 0)
      {
        float healthPercentage = this.m_aiActor.healthHaver.GetCurrentHealthPercentage();
        if ((double) healthPercentage < (double) this.m_lowestRecordedHealthPercentage)
        {
          for (int index = 0; index < this.HealthThresholds.Length; ++index)
          {
            if ((double) this.HealthThresholds[index] >= (double) healthPercentage && (double) this.HealthThresholds[index] < (double) this.m_lowestRecordedHealthPercentage)
              ++this.m_healthThresholdCredits;
          }
          this.m_lowestRecordedHealthPercentage = healthPercentage;
        }
      }
      if (!BasicAttackBehavior.DrawDebugFiringArea)
        return;
      if (UnityEngine.Time.frameCount != BasicAttackBehavior.m_lastFrame)
      {
        BasicAttackBehavior.m_arcCount = 0;
        BasicAttackBehavior.m_lastFrame = UnityEngine.Time.frameCount;
      }
      if (!(bool) (Object) this.m_aiActor.TargetRigidbody || this.targetAreaStyle == null)
        return;
      this.targetAreaStyle.DrawDebugLines(this.GetOrigin(this.targetAreaStyle.targetAreaOrigin), this.m_aiActor.TargetRigidbody.GetUnitCenter(ColliderType.HitBox), this.m_aiActor);
    }

    public override BehaviorResult Update()
    {
      BehaviorResult behaviorResult = base.Update();
      return behaviorResult != BehaviorResult.Continue ? behaviorResult : BehaviorResult.Continue;
    }

    public override void Init(GameObject gameObject, AIActor aiActor, AIShooter aiShooter)
    {
      base.Init(gameObject, aiActor, aiShooter);
      this.m_behaviorSpeculator = gameObject.GetComponent<BehaviorSpeculator>();
      if (this.resetCooldownOnDamage == null)
        return;
      this.m_aiActor.healthHaver.OnDamaged += new HealthHaver.OnDamagedEvent(this.OnDamaged);
    }

    public override bool IsReady()
    {
      if ((double) this.MinHealthThreshold > 0.0 && (double) this.m_aiActor.healthHaver.GetCurrentHealthPercentage() < (double) this.MinHealthThreshold || (double) this.MaxHealthThreshold < 1.0 && (double) this.m_aiActor.healthHaver.GetCurrentHealthPercentage() > (double) this.MaxHealthThreshold || this.HealthThresholds.Length > 0 && this.m_healthThresholdCredits <= 0 || !string.IsNullOrEmpty(this.GroupName) && (double) this.m_behaviorSpeculator.GetGroupCooldownTimer(this.GroupName) > 0.0 || this.IsBlackPhantom && !this.m_aiActor.IsBlackPhantom || (double) this.MinRange > 0.0 && (!(bool) (Object) this.m_aiActor.TargetRigidbody || (double) Vector2.Distance(this.m_aiActor.specRigidbody.UnitCenter, this.m_aiActor.TargetRigidbody.GetUnitCenter(ColliderType.HitBox)) < (double) this.MinRange) || (double) this.Range > 0.0 && (!(bool) (Object) this.m_aiActor.TargetRigidbody || (double) Vector2.Distance(this.m_aiActor.specRigidbody.UnitCenter, this.m_aiActor.TargetRigidbody.GetUnitCenter(ColliderType.HitBox)) > (double) this.Range))
        return false;
      if ((double) this.MinWallDistance > 0.0)
      {
        PixelCollider hitboxPixelCollider = this.m_aiActor.specRigidbody.HitboxPixelCollider;
        CellArea area = this.m_aiActor.ParentRoom.area;
        if ((double) hitboxPixelCollider.UnitLeft - (double) area.UnitLeft < (double) this.MinWallDistance || (double) area.UnitRight - (double) hitboxPixelCollider.UnitRight < (double) this.MinWallDistance || (double) hitboxPixelCollider.UnitBottom - (double) area.UnitBottom < (double) this.MinWallDistance || (double) area.UnitTop - (double) hitboxPixelCollider.UnitTop < (double) this.MinWallDistance)
          return false;
      }
      return ((double) this.MaxEnemiesInRoom <= 0.0 || (double) this.m_aiActor.ParentRoom.GetActiveEnemiesCount(RoomHandler.ActiveEnemyType.All) <= (double) this.MaxEnemiesInRoom) && this.TargetInFiringArea() && (!this.RequiresLineOfSight || this.TargetInLineOfSight()) && (this.MaxUsages <= 0 || this.m_numTimesUsed < this.MaxUsages) && (double) this.m_cooldownTimer <= 0.0;
    }

    public override float GetMinReadyRange()
    {
      return (double) this.Range > 0.0 && this.IsReady() ? this.Range : -1f;
    }

    public override float GetMaxRange() => (double) this.Range > 0.0 ? this.Range : -1f;

    public override void OnActorPreDeath()
    {
      if (this.resetCooldownOnDamage != null)
        this.m_aiActor.healthHaver.OnDamaged -= new HealthHaver.OnDamagedEvent(this.OnDamaged);
      base.OnActorPreDeath();
    }

    protected virtual void UpdateCooldowns()
    {
      this.m_cooldownTimer = this.Cooldown;
      if ((double) this.CooldownVariance > 0.0)
        this.m_cooldownTimer += Random.Range(-this.CooldownVariance, this.CooldownVariance);
      if ((double) this.AttackCooldown > 0.0)
        this.m_behaviorSpeculator.AttackCooldown = this.AttackCooldown;
      if ((double) this.GlobalCooldown > 0.0)
        this.m_behaviorSpeculator.GlobalCooldown = this.GlobalCooldown;
      if ((double) this.GroupCooldown > 0.0)
        this.m_behaviorSpeculator.SetGroupCooldown(this.GroupName, this.GroupCooldown);
      if (this.HealthThresholds.Length > 0 && this.m_healthThresholdCredits > 0)
        this.m_healthThresholdCredits = !this.AccumulateHealthThresholds ? 0 : this.m_healthThresholdCredits - 1;
      ++this.m_numTimesUsed;
    }

    protected virtual Vector2 GetOrigin(ShootBehavior.TargetAreaOrigin origin)
    {
      if (origin == ShootBehavior.TargetAreaOrigin.ShootPoint)
        Debug.LogWarning((object) "ColliderType.ShootPoint is not supported for base BasicAttackBehaviors!");
      return this.m_aiActor.specRigidbody.GetUnitCenter(ColliderType.HitBox);
    }

    protected bool TargetInLineOfSight() => this.m_aiActor.HasLineOfSightToTarget;

    protected bool TargetInFiringArea()
    {
      if (this.targetAreaStyle == null)
        return true;
      return (bool) (Object) this.m_aiActor.TargetRigidbody && this.targetAreaStyle.TargetInFiringArea(this.GetOrigin(this.targetAreaStyle.targetAreaOrigin), this.m_aiActor.TargetRigidbody.GetUnitCenter(ColliderType.HitBox));
    }

    private void OnDamaged(
      float resultValue,
      float maxValue,
      CoreDamageTypes damageTypes,
      DamageCategory damageCategory,
      Vector2 damageDirection)
    {
      if (this.resetCooldownOnDamage == null || (double) this.m_resetCooldownOnDamageCooldown > 0.0)
        return;
      bool flag = false;
      if (this.resetCooldownOnDamage.Cooldown && (double) this.m_cooldownTimer > 0.0)
      {
        this.m_cooldownTimer = 0.0f;
        flag = true;
      }
      if (this.resetCooldownOnDamage.AttackCooldown && (double) this.m_behaviorSpeculator.AttackCooldown > 0.0)
      {
        this.m_behaviorSpeculator.AttackCooldown = 0.0f;
        flag = true;
      }
      if (this.resetCooldownOnDamage.GlobalCooldown && (double) this.m_behaviorSpeculator.GlobalCooldown > 0.0)
      {
        this.m_behaviorSpeculator.GlobalCooldown = 0.0f;
        flag = true;
      }
      if (this.resetCooldownOnDamage.GroupCooldown && (double) this.m_behaviorSpeculator.GetGroupCooldownTimer(this.GroupName) > 0.0)
      {
        this.m_behaviorSpeculator.SetGroupCooldown(this.GroupName, 0.0f);
        flag = true;
      }
      if (!flag || (double) this.resetCooldownOnDamage.ResetCooldown <= 0.0)
        return;
      this.m_resetCooldownOnDamageCooldown = this.resetCooldownOnDamage.ResetCooldown;
    }

public class ResetCooldownOnDamage
    {
      public bool Cooldown = true;
      public bool AttackCooldown;
      public bool GlobalCooldown;
      public bool GroupCooldown;
      [InspectorTooltip("If set, cooldowns can not be reset again for this amount of time after taking damage.")]
      public float ResetCooldown;
    }
  }

