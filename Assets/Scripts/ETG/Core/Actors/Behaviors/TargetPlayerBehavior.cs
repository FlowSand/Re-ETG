using Dungeonator;
using FullInspector;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

public class TargetPlayerBehavior : TargetBehaviorBase
  {
    public float Radius = 10f;
    public bool LineOfSight = true;
    public bool ObjectPermanence = true;
    public float SearchInterval = 0.25f;
    public bool PauseOnTargetSwitch;
    [InspectorShowIf("PauseOnTargetSwitch")]
    public float PauseTime = 0.25f;
    private const float PLAYER_REFRESH_TIMER = 1f;
    private float m_losTimer;
    private float m_coopRefreshSearchTimer;
    private float m_prevDistToTarget;
    private PlayerController m_previousPlayer;
    private SpeculativeRigidbody m_specRigidbody;
    private BehaviorSpeculator m_behaviorSpeculator;

    public override void Start()
    {
    }

    public override void Upkeep()
    {
      base.Upkeep();
      this.DecrementTimer(ref this.m_losTimer);
      this.DecrementTimer(ref this.m_coopRefreshSearchTimer);
    }

    public override BehaviorResult Update()
    {
      BehaviorResult behaviorResult = base.Update();
      if (behaviorResult != BehaviorResult.Continue)
        return behaviorResult;
      if ((double) this.m_losTimer > 0.0)
        return BehaviorResult.Continue;
      this.m_losTimer = this.SearchInterval;
      if ((bool) (Object) this.m_behaviorSpeculator.PlayerTarget)
      {
        if (this.m_behaviorSpeculator.PlayerTarget.IsFalling)
        {
          this.m_behaviorSpeculator.PlayerTarget = (GameActor) null;
          if ((bool) (Object) this.m_aiActor)
            this.m_aiActor.ClearPath();
          return BehaviorResult.SkipRemainingClassBehaviors;
        }
        if ((bool) (Object) this.m_behaviorSpeculator.PlayerTarget.healthHaver && (this.m_behaviorSpeculator.PlayerTarget.healthHaver.IsDead || this.m_behaviorSpeculator.PlayerTarget.healthHaver.PreventAllDamage))
        {
          this.m_behaviorSpeculator.PlayerTarget = (GameActor) null;
          if ((bool) (Object) this.m_aiActor)
            this.m_aiActor.ClearPath();
          return BehaviorResult.SkipRemainingClassBehaviors;
        }
      }
      else
        this.m_behaviorSpeculator.PlayerTarget = (GameActor) null;
      if (!this.ObjectPermanence)
        this.m_behaviorSpeculator.PlayerTarget = (GameActor) null;
      if ((Object) this.m_behaviorSpeculator.PlayerTarget != (Object) null && this.m_behaviorSpeculator.PlayerTarget.IsStealthed)
        this.m_behaviorSpeculator.PlayerTarget = (GameActor) null;
      if (GameManager.Instance.AllPlayers.Length > 1 && (double) this.m_coopRefreshSearchTimer <= 0.0)
        this.m_behaviorSpeculator.PlayerTarget = (GameActor) null;
      if (this.m_behaviorSpeculator.PlayerTarget is AIActor)
      {
        float num = Vector2.Distance(this.m_specRigidbody.UnitCenter, this.m_behaviorSpeculator.PlayerTarget.specRigidbody.UnitCenter);
        if ((double) this.m_prevDistToTarget + 3.0 < (double) num)
          this.m_behaviorSpeculator.PlayerTarget = (GameActor) null;
        this.m_prevDistToTarget = num;
        if ((bool) (Object) this.m_aiActor && !this.m_aiActor.IsNormalEnemy && (bool) (Object) this.m_aiActor.CompanionOwner && this.m_behaviorSpeculator.PlayerTarget is AIActor && this.m_behaviorSpeculator.PlayerTarget.GetAbsoluteParentRoom() != this.m_aiActor.CompanionOwner.CurrentRoom)
          this.m_behaviorSpeculator.PlayerTarget = (GameActor) null;
      }
      if ((Object) this.m_behaviorSpeculator.PlayerTarget != (Object) null)
        return BehaviorResult.Continue;
      PlayerController playerController = GameManager.Instance.GetActivePlayerClosestToPoint(this.m_specRigidbody.UnitCenter);
      if ((bool) (Object) this.m_aiActor && this.m_aiActor.SuppressTargetSwitch)
        playerController = this.m_previousPlayer;
      if (!(bool) (Object) this.m_aiActor || this.m_aiActor.CanTargetPlayers && !this.m_aiActor.CanTargetEnemies)
      {
        if ((Object) playerController == (Object) null)
          return BehaviorResult.Continue;
        this.m_behaviorSpeculator.PlayerTarget = (GameActor) playerController;
        if (GameManager.Instance.AllPlayers.Length > 1)
          this.m_coopRefreshSearchTimer = 1f;
      }
      else if (this.m_aiActor.CanTargetEnemies && !this.m_aiActor.CanTargetPlayers)
      {
        List<AIActor> activeEnemies = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(this.m_aiActor.GridPosition).GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
        if (activeEnemies != null && activeEnemies.Count > 0)
        {
          AIActor aiActor1 = (AIActor) null;
          float num1 = -1f;
          if (!(bool) (Object) this.m_aiActor || this.m_aiActor.IsNormalEnemy || !(bool) (Object) this.m_aiActor.CompanionOwner || !this.m_aiActor.CompanionOwner.IsStealthed)
          {
            for (int index = 0; index < activeEnemies.Count; ++index)
            {
              AIActor aiActor2 = activeEnemies[index];
              if ((bool) (Object) aiActor2 && aiActor2.IsNormalEnemy && !aiActor2.IsGone && !aiActor2.IsHarmlessEnemy && !((Object) aiActor2 == (Object) this.m_aiActor) && (!(bool) (Object) aiActor2.healthHaver || !aiActor2.healthHaver.PreventAllDamage))
              {
                float num2 = Vector2.Distance(this.m_specRigidbody.UnitCenter, aiActor2.specRigidbody.UnitCenter);
                if ((Object) aiActor1 == (Object) null || (double) num2 < (double) num1)
                {
                  aiActor1 = aiActor2;
                  num1 = num2;
                }
              }
            }
          }
          if ((bool) (Object) aiActor1)
          {
            this.m_behaviorSpeculator.PlayerTarget = (GameActor) aiActor1;
            this.m_prevDistToTarget = num1;
          }
        }
      }
      else if (!this.m_aiActor.CanTargetEnemies || !this.m_aiActor.CanTargetPlayers)
        ;
      if ((Object) this.m_aiShooter != (Object) null && (Object) this.m_behaviorSpeculator.PlayerTarget != (Object) null)
        this.m_aiShooter.AimAtPoint(this.m_behaviorSpeculator.PlayerTarget.CenterPosition);
      if ((bool) (Object) this.m_aiActor && this.PauseOnTargetSwitch && this.m_aiActor.HasBeenEngaged && (bool) (Object) this.m_previousPlayer && (bool) (Object) playerController && (Object) this.m_previousPlayer != (Object) playerController)
      {
        this.m_aiActor.behaviorSpeculator.AttackCooldown = Mathf.Max(this.m_aiActor.behaviorSpeculator.AttackCooldown, this.PauseTime);
        return BehaviorResult.SkipAllRemainingBehaviors;
      }
      this.m_previousPlayer = playerController;
      if (!(bool) (Object) this.m_aiActor || this.m_aiActor.HasBeenEngaged)
        return BehaviorResult.SkipRemainingClassBehaviors;
      this.m_aiActor.HasBeenEngaged = true;
      return BehaviorResult.SkipAllRemainingBehaviors;
    }

    public override void Init(GameObject gameObject, AIActor aiActor, AIShooter aiShooter)
    {
      base.Init(gameObject, aiActor, aiShooter);
      this.m_specRigidbody = gameObject.GetComponent<SpeculativeRigidbody>();
      this.m_behaviorSpeculator = gameObject.GetComponent<BehaviorSpeculator>();
    }
  }

