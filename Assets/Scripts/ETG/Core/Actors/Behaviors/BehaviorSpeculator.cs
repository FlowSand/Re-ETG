// Decompiled with JetBrains decompiler
// Type: BehaviorSpeculator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using FullInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable disable

public class BehaviorSpeculator : BaseBehavior<FullSerializerSerializer>
  {
    public bool InstantFirstTick;
    public float TickInterval = 0.1f;
    public float PostAwakenDelay;
    public bool RemoveDelayOnReinforce;
    public bool OverrideStartingFacingDirection;
    [ShowInInspectorIf("OverrideStartingFacingDirection", false)]
    public float StartingFacingDirection = -90f;
    public bool SkipTimingDifferentiator;
    [InspectorCollectionRotorzFlags(HideRemoveButtons = true)]
    [InspectorHeader("Behaviors")]
    public List<OverrideBehaviorBase> OverrideBehaviors;
    [InspectorCollectionRotorzFlags(HideRemoveButtons = true)]
    public List<TargetBehaviorBase> TargetBehaviors;
    [InspectorCollectionRotorzFlags(HideRemoveButtons = true)]
    public List<MovementBehaviorBase> MovementBehaviors;
    [InspectorCollectionRotorzFlags(HideRemoveButtons = true)]
    public List<AttackBehaviorBase> AttackBehaviors;
    [InspectorCollectionRotorzFlags(HideRemoveButtons = true)]
    public List<BehaviorBase> OtherBehaviors;
    private float m_localTimeScale = 1f;
    private GameActor m_playerTarget;
    private bool m_isFirstUpdate;
    private float m_postAwakenDelay;
    private float m_tickTimer;
    private float m_attackCooldownTimer;
    private float m_globalCooldownTimer;
    private float m_stunTimer;
    private tk2dSpriteAnimator m_extantStunVFX;
    private BraveDictionary<string, float> m_groupCooldownTimers;
    private float m_cooldownScale = 1f;
    private List<BehaviorBase> m_behaviors = new List<BehaviorBase>();
    private BehaviorBase m_activeContinuousBehavior;
    private Dictionary<IList, BehaviorBase> m_classSpecificContinuousBehavior = new Dictionary<IList, BehaviorBase>();
    private AIActor m_aiActor;

    public float LocalTimeScale
    {
      get => (bool) (UnityEngine.Object) this.m_aiActor ? this.m_aiActor.LocalTimeScale : this.m_localTimeScale;
      set
      {
        if ((bool) (UnityEngine.Object) this.m_aiActor)
          this.m_aiActor.LocalTimeScale = value;
        else
          this.m_localTimeScale = value;
      }
    }

    public float LocalDeltaTime
    {
      get
      {
        return (UnityEngine.Object) this.m_aiActor != (UnityEngine.Object) null ? this.m_aiActor.LocalDeltaTime : BraveTime.DeltaTime * this.LocalTimeScale;
      }
    }

    public float AttackCooldown
    {
      get => this.m_attackCooldownTimer;
      set => this.m_attackCooldownTimer = value;
    }

    public float GlobalCooldown
    {
      get => this.m_globalCooldownTimer;
      set => this.m_globalCooldownTimer = value;
    }

    public float CooldownScale
    {
      get => this.m_cooldownScale;
      set => this.m_cooldownScale = value;
    }

    public BehaviorBase ActiveContinuousAttackBehavior
    {
      get
      {
        if (this.m_activeContinuousBehavior is AttackBehaviorBase)
          return this.m_activeContinuousBehavior;
        return this.m_classSpecificContinuousBehavior.ContainsKey((IList) this.AttackBehaviors) ? this.m_classSpecificContinuousBehavior[(IList) this.AttackBehaviors] : (BehaviorBase) null;
      }
    }

    public bool IsInterruptable
    {
      get
      {
        bool isInterruptable = true;
        if (this.m_activeContinuousBehavior != null)
          isInterruptable &= this.m_activeContinuousBehavior.IsOverridable();
        if (this.m_classSpecificContinuousBehavior.Count <= 0)
          return isInterruptable;
        if (this.m_classSpecificContinuousBehavior.ContainsKey((IList) this.OverrideBehaviors))
          isInterruptable &= this.m_classSpecificContinuousBehavior[(IList) this.OverrideBehaviors].IsOverridable();
        if (this.m_classSpecificContinuousBehavior.ContainsKey((IList) this.TargetBehaviors))
          isInterruptable &= this.m_classSpecificContinuousBehavior[(IList) this.TargetBehaviors].IsOverridable();
        if (this.m_classSpecificContinuousBehavior.ContainsKey((IList) this.MovementBehaviors))
          isInterruptable &= this.m_classSpecificContinuousBehavior[(IList) this.MovementBehaviors].IsOverridable();
        if (this.m_classSpecificContinuousBehavior.ContainsKey((IList) this.AttackBehaviors))
          isInterruptable &= this.m_classSpecificContinuousBehavior[(IList) this.AttackBehaviors].IsOverridable();
        if (this.m_classSpecificContinuousBehavior.ContainsKey((IList) this.OtherBehaviors))
          isInterruptable &= this.m_classSpecificContinuousBehavior[(IList) this.OtherBehaviors].IsOverridable();
        return isInterruptable;
      }
    }

    public GameActor PlayerTarget
    {
      get => (bool) (UnityEngine.Object) this.m_aiActor ? this.m_aiActor.PlayerTarget : this.m_playerTarget;
      set
      {
        if ((bool) (UnityEngine.Object) this.m_aiActor)
          this.m_aiActor.PlayerTarget = value;
        else
          this.m_playerTarget = value;
      }
    }

    public SpeculativeRigidbody TargetRigidbody
    {
      get
      {
        if ((bool) (UnityEngine.Object) this.m_aiActor)
          return this.m_aiActor.TargetRigidbody;
        return (bool) (UnityEngine.Object) this.m_playerTarget ? this.m_playerTarget.specRigidbody : (SpeculativeRigidbody) null;
      }
    }

    public Vector2 TargetVelocity
    {
      get
      {
        if ((bool) (UnityEngine.Object) this.m_aiActor)
          return this.m_aiActor.TargetVelocity;
        return (bool) (UnityEngine.Object) this.m_playerTarget ? this.m_playerTarget.specRigidbody.Velocity : Vector2.zero;
      }
    }

    public bool PreventMovement { get; set; }

    public FleePlayerData FleePlayerData { get; set; }

    public AttackBehaviorGroup AttackBehaviorGroup
    {
      get
      {
        if (this.AttackBehaviors == null)
          return (AttackBehaviorGroup) null;
        for (int index = 0; index < this.AttackBehaviors.Count; ++index)
        {
          if (this.AttackBehaviors[index] is AttackBehaviorGroup)
            return this.AttackBehaviors[index] as AttackBehaviorGroup;
        }
        return (AttackBehaviorGroup) null;
      }
    }

    private void Start()
    {
      this.m_aiActor = this.GetComponent<AIActor>();
      if ((bool) (UnityEngine.Object) this.m_aiActor)
        this.m_aiActor.healthHaver.OnPreDeath += new Action<Vector2>(this.OnPreDeath);
      if ((bool) (UnityEngine.Object) this.healthHaver)
        this.healthHaver.OnDamaged += new HealthHaver.OnDamagedEvent(this.OnDamaged);
      if (this.OverrideStartingFacingDirection && (bool) (UnityEngine.Object) this.aiAnimator)
        this.aiAnimator.FacingDirection = this.StartingFacingDirection;
      if ((bool) (UnityEngine.Object) this.m_aiActor)
        this.m_aiActor.specRigidbody.Initialize();
      this.RegisterBehaviors((IList) this.OverrideBehaviors);
      this.RegisterBehaviors((IList) this.TargetBehaviors);
      this.RegisterBehaviors((IList) this.MovementBehaviors);
      this.RegisterBehaviors((IList) this.AttackBehaviors);
      this.RegisterBehaviors((IList) this.OtherBehaviors);
      this.StartBehaviors();
      if (this.InstantFirstTick)
        this.m_tickTimer = this.TickInterval;
      this.m_postAwakenDelay = this.PostAwakenDelay;
    }

    private void Update()
    {
      if ((bool) (UnityEngine.Object) this.m_aiActor)
      {
        if (!this.m_aiActor.enabled || this.m_aiActor.healthHaver.IsDead || !this.m_aiActor.HasBeenAwoken)
          return;
        if ((double) this.m_postAwakenDelay > 0.0 && (!this.RemoveDelayOnReinforce || !this.aiActor.IsInReinforcementLayer))
        {
          this.m_postAwakenDelay = Mathf.Max(0.0f, this.m_postAwakenDelay - this.LocalDeltaTime);
          return;
        }
        if ((double) this.m_aiActor.SpeculatorDelayTime > 0.0)
        {
          this.m_aiActor.SpeculatorDelayTime = Mathf.Max(0.0f, this.m_aiActor.SpeculatorDelayTime - this.LocalDeltaTime);
          return;
        }
      }
      if (!this.m_isFirstUpdate)
      {
        this.FirstUpdate();
        this.m_isFirstUpdate = true;
      }
      this.m_tickTimer += this.LocalDeltaTime;
      this.m_globalCooldownTimer = Mathf.Max(0.0f, this.m_globalCooldownTimer - this.LocalDeltaTime);
      this.m_attackCooldownTimer = Mathf.Max(0.0f, this.m_attackCooldownTimer - this.LocalDeltaTime);
      this.m_stunTimer = Mathf.Max(0.0f, this.m_stunTimer - this.LocalDeltaTime);
      this.UpdateStunVFX();
      if (this.m_groupCooldownTimers != null)
      {
        for (int index = 0; index < this.m_groupCooldownTimers.Count; ++index)
          this.m_groupCooldownTimers.Values[index] = Mathf.Max(0.0f, this.m_groupCooldownTimers.Values[index] - this.LocalDeltaTime);
      }
      bool isTick = (double) this.m_tickTimer > (double) this.TickInterval;
      bool onGlobalCooldown = (double) this.m_globalCooldownTimer > 0.0;
      this.UpkeepBehaviors(isTick);
      if (!this.IsStunned)
        this.UpdateBehaviors(isTick, onGlobalCooldown);
      if (!isTick)
        return;
      this.m_tickTimer = 0.0f;
    }

    protected virtual void UpdateStunVFX()
    {
      if ((double) this.m_stunTimer <= 0.0 && (UnityEngine.Object) this.m_extantStunVFX != (UnityEngine.Object) null)
      {
        SpawnManager.Despawn(this.m_extantStunVFX.gameObject);
        this.m_extantStunVFX = (tk2dSpriteAnimator) null;
      }
      else
      {
        if ((double) this.m_stunTimer <= 0.0 || !((UnityEngine.Object) this.m_extantStunVFX != (UnityEngine.Object) null))
          return;
        this.m_extantStunVFX.transform.position = this.aiActor.sprite.WorldTopCenter.ToVector3ZUp(this.m_extantStunVFX.transform.position.z);
      }
    }

    private void OnPreDeath(Vector2 dir)
    {
      if (this.IsStunned)
      {
        this.m_stunTimer = 0.0f;
        this.UpdateStunVFX();
      }
      for (int index = 0; index < this.m_behaviors.Count; ++index)
        this.m_behaviors[index].OnActorPreDeath();
      this.Interrupt();
    }

    private void OnDamaged(
      float resultValue,
      float maxValue,
      CoreDamageTypes damageTypes,
      DamageCategory damageCategory,
      Vector2 damageDirection)
    {
      this.m_postAwakenDelay = 0.0f;
      if (!(bool) (UnityEngine.Object) this.healthHaver)
        return;
      this.healthHaver.OnDamaged -= new HealthHaver.OnDamagedEvent(this.OnDamaged);
    }

    protected override void OnDestroy()
    {
      for (int index = 0; index < this.m_behaviors.Count; ++index)
        this.m_behaviors[index].Destroy();
      if ((bool) (UnityEngine.Object) this.m_aiActor)
        this.m_aiActor.healthHaver.OnPreDeath -= new Action<Vector2>(this.OnPreDeath);
      if ((bool) (UnityEngine.Object) this.healthHaver)
        this.healthHaver.OnDamaged -= new HealthHaver.OnDamagedEvent(this.OnDamaged);
      base.OnDestroy();
    }

    public void Stun(float duration, bool createVFX = true)
    {
      if ((bool) (UnityEngine.Object) this.aiActor && (bool) (UnityEngine.Object) this.aiActor.healthHaver && this.aiActor.healthHaver.IsBoss || (bool) (UnityEngine.Object) this.healthHaver && !this.healthHaver.IsVulnerable || this.ImmuneToStun)
        return;
      this.m_stunTimer = Mathf.Max(this.m_stunTimer, duration);
      if ((double) this.m_stunTimer <= 0.0)
        return;
      if (this.IsInterruptable)
        this.Interrupt();
      if ((UnityEngine.Object) this.m_extantStunVFX == (UnityEngine.Object) null && createVFX)
        this.m_extantStunVFX = this.aiActor.PlayEffectOnActor(ResourceCache.Acquire("Global VFX/VFX_Stun") as GameObject, (Vector3) (this.aiActor.sprite.WorldTopCenter - this.aiActor.CenterPosition).WithX(0.0f), alreadyMiddleCenter: true).GetComponent<tk2dSpriteAnimator>();
      this.aiActor.ClearPath();
    }

    public void UpdateStun(float maxStunTime)
    {
      if (!this.IsStunned || (bool) (UnityEngine.Object) this.aiActor && (bool) (UnityEngine.Object) this.aiActor.healthHaver && this.aiActor.healthHaver.IsBoss || (bool) (UnityEngine.Object) this.healthHaver && !this.healthHaver.IsVulnerable || this.ImmuneToStun)
        return;
      this.m_stunTimer = maxStunTime;
    }

    public void EndStun()
    {
      this.m_stunTimer = 0.0f;
      this.UpdateStunVFX();
    }

    public bool IsStunned => (double) this.m_stunTimer > 0.0;

    public bool ImmuneToStun { get; set; }

    public float GetDesiredCombatDistance()
    {
      float a1 = -1f;
      for (int index = 0; index < this.MovementBehaviors.Count; ++index)
      {
        float desiredCombatDistance = this.MovementBehaviors[index].DesiredCombatDistance;
        if ((double) desiredCombatDistance > -1.0)
          a1 = (double) a1 >= 0.0 ? Mathf.Min(a1, desiredCombatDistance) : this.MovementBehaviors[index].DesiredCombatDistance;
      }
      float a2 = a1;
      float a3 = float.MinValue;
      for (int index = 0; index < this.AttackBehaviors.Count; ++index)
      {
        float minReadyRange = this.AttackBehaviors[index].GetMinReadyRange();
        if ((double) minReadyRange >= 0.0)
          a2 = Mathf.Min(a2, minReadyRange);
        a3 = Mathf.Max(a3, this.AttackBehaviors[index].GetMaxRange());
      }
      if ((double) a2 < 2147483648.0)
        return a2;
      return (double) a3 > (double) int.MinValue ? a3 : -1f;
    }

    public void Interrupt()
    {
      if (this.m_activeContinuousBehavior != null)
        this.EndContinuousBehavior();
      if (this.m_classSpecificContinuousBehavior.Count <= 0)
        return;
      if (this.m_classSpecificContinuousBehavior.ContainsKey((IList) this.OverrideBehaviors))
        this.EndClassSpecificContinuousBehavior((IList) this.OverrideBehaviors);
      if (this.m_classSpecificContinuousBehavior.ContainsKey((IList) this.TargetBehaviors))
        this.EndClassSpecificContinuousBehavior((IList) this.TargetBehaviors);
      if (this.m_classSpecificContinuousBehavior.ContainsKey((IList) this.MovementBehaviors))
        this.EndClassSpecificContinuousBehavior((IList) this.MovementBehaviors);
      if (this.m_classSpecificContinuousBehavior.ContainsKey((IList) this.AttackBehaviors))
        this.EndClassSpecificContinuousBehavior((IList) this.AttackBehaviors);
      if (!this.m_classSpecificContinuousBehavior.ContainsKey((IList) this.OtherBehaviors))
        return;
      this.EndClassSpecificContinuousBehavior((IList) this.OtherBehaviors);
    }

    public void InterruptAndDisable()
    {
      this.Interrupt();
      this.enabled = false;
    }

    public void RefreshBehaviors()
    {
      List<BehaviorBase> behaviors = this.m_behaviors;
      this.m_behaviors = new List<BehaviorBase>();
      this.RefreshBehaviors((IList) this.OverrideBehaviors, behaviors);
      this.RefreshBehaviors((IList) this.TargetBehaviors, behaviors);
      this.RefreshBehaviors((IList) this.MovementBehaviors, behaviors);
      this.RefreshBehaviors((IList) this.AttackBehaviors, behaviors);
      this.RefreshBehaviors((IList) this.OtherBehaviors, behaviors);
    }

    public event Action<string> AnimationEventTriggered;

    public void TriggerAnimationEvent(string eventInfo)
    {
      if (this.AnimationEventTriggered == null)
        return;
      this.AnimationEventTriggered(eventInfo);
    }

    public void SetGroupCooldown(string groupName, float newCooldown)
    {
      if (this.m_groupCooldownTimers == null)
        this.m_groupCooldownTimers = new BraveDictionary<string, float>();
      float num;
      if (this.m_groupCooldownTimers.TryGetValue(groupName, out num))
      {
        if ((double) num >= (double) newCooldown)
          return;
        this.m_groupCooldownTimers[groupName] = newCooldown;
      }
      else
        this.m_groupCooldownTimers[groupName] = newCooldown;
    }

    public float GetGroupCooldownTimer(string groupName)
    {
      float num;
      return this.m_groupCooldownTimers == null || !this.m_groupCooldownTimers.TryGetValue(groupName, out num) ? 0.0f : num;
    }

    private void RegisterBehaviors(IList behaviors)
    {
      if (behaviors == null)
        behaviors = (IList) new BehaviorBase[0];
      for (int index = 0; index < behaviors.Count; ++index)
        this.m_behaviors.Add(behaviors[index] as BehaviorBase);
    }

    private void StartBehaviors()
    {
      for (int index = 0; index < this.m_behaviors.Count; ++index)
      {
        this.m_behaviors[index].Init(this.gameObject, this.m_aiActor, this.aiShooter);
        this.m_behaviors[index].Start();
      }
    }

    private void UpkeepBehaviors(bool isTick)
    {
      for (int index = 0; index < this.m_behaviors.Count; ++index)
      {
        if (isTick || this.m_behaviors[index].UpdateEveryFrame())
        {
          this.m_behaviors[index].SetDeltaTime(!this.m_behaviors[index].UpdateEveryFrame() ? this.m_tickTimer : this.LocalDeltaTime);
          this.m_behaviors[index].Upkeep();
        }
      }
      if (this.m_activeContinuousBehavior != null && this.m_activeContinuousBehavior.IsOverridable())
      {
        for (int index = 0; index < this.m_behaviors.Count; ++index)
        {
          if ((isTick || this.m_behaviors[index].UpdateEveryFrame()) && this.m_behaviors[index] != this.m_activeContinuousBehavior && this.m_behaviors[index].OverrideOtherBehaviors())
          {
            this.EndContinuousBehavior();
            break;
          }
        }
      }
      else
      {
        if (this.m_classSpecificContinuousBehavior.Count <= 0)
          return;
        KeyValuePair<IList, BehaviorBase> keyValuePair = this.m_classSpecificContinuousBehavior.First<KeyValuePair<IList, BehaviorBase>>();
        IList key = keyValuePair.Key;
        BehaviorBase behaviorBase1 = keyValuePair.Value;
        if (!behaviorBase1.IsOverridable())
          return;
        for (int index = 0; index < key.Count; ++index)
        {
          BehaviorBase behaviorBase2 = key[index] as BehaviorBase;
          if ((isTick || behaviorBase2.UpdateEveryFrame()) && key[index] != behaviorBase1 && behaviorBase2.OverrideOtherBehaviors())
          {
            this.EndClassSpecificContinuousBehavior(key);
            break;
          }
        }
      }
    }

    private void UpdateBehaviors(bool isTick, bool onGlobalCooldown)
    {
      if (this.m_activeContinuousBehavior != null && this.m_classSpecificContinuousBehavior.Count > 1)
        BraveUtility.Log("Trying to activate a class specific continuous behavior at the same time as a global continuous behavior; this isn't supported.", UnityEngine.Color.red, BraveUtility.LogVerbosity.IMPORTANT);
      if (this.m_activeContinuousBehavior != null)
      {
        if (!isTick && !this.m_activeContinuousBehavior.UpdateEveryFrame() || onGlobalCooldown && !this.m_activeContinuousBehavior.IgnoreGlobalCooldown() || this.m_activeContinuousBehavior.ContinuousUpdate() != ContinuousBehaviorResult.Finished)
          return;
        this.EndContinuousBehavior();
      }
      else if (this.UpdateBehaviorClass((IList) this.OverrideBehaviors, isTick, onGlobalCooldown) == BehaviorResult.SkipAllRemainingBehaviors || this.UpdateBehaviorClass((IList) this.TargetBehaviors, isTick, onGlobalCooldown) == BehaviorResult.SkipAllRemainingBehaviors || !this.PreventMovement && this.UpdateBehaviorClass((IList) this.MovementBehaviors, isTick, onGlobalCooldown) == BehaviorResult.SkipAllRemainingBehaviors || (double) this.m_attackCooldownTimer <= 0.0 && this.UpdateBehaviorClass((IList) this.AttackBehaviors, isTick, onGlobalCooldown) == BehaviorResult.SkipAllRemainingBehaviors || this.UpdateBehaviorClass((IList) this.OtherBehaviors, isTick, onGlobalCooldown) != BehaviorResult.SkipAllRemainingBehaviors)
        ;
    }

    private BehaviorResult UpdateBehaviorClass(IList behaviors, bool isTick, bool onGlobalCooldown)
    {
      if (behaviors == null)
        return BehaviorResult.Continue;
      if (this.m_classSpecificContinuousBehavior.ContainsKey(behaviors))
      {
        BehaviorBase behaviorBase = this.m_classSpecificContinuousBehavior[behaviors];
        if ((isTick || behaviorBase.UpdateEveryFrame()) && (!onGlobalCooldown || behaviorBase.IgnoreGlobalCooldown()) && behaviorBase.ContinuousUpdate() == ContinuousBehaviorResult.Finished)
          this.EndClassSpecificContinuousBehavior(behaviors);
        return BehaviorResult.SkipRemainingClassBehaviors;
      }
      for (int index = 0; index < behaviors.Count; ++index)
      {
        BehaviorBase behavior = behaviors[index] as BehaviorBase;
        if ((isTick || behavior.UpdateEveryFrame()) && (!onGlobalCooldown || behavior.IgnoreGlobalCooldown()))
        {
          switch (behavior.Update())
          {
            case BehaviorResult.SkipRemainingClassBehaviors:
              return BehaviorResult.SkipRemainingClassBehaviors;
            case BehaviorResult.SkipAllRemainingBehaviors:
              return BehaviorResult.SkipAllRemainingBehaviors;
            case BehaviorResult.RunContinuousInClass:
              if (this.m_classSpecificContinuousBehavior.ContainsKey(behaviors))
                BraveUtility.Log("Trying to overwrite the current class continuous behaviors; this shouldn't happen.", UnityEngine.Color.red, BraveUtility.LogVerbosity.IMPORTANT);
              this.m_classSpecificContinuousBehavior[behaviors] = behavior;
              return BehaviorResult.SkipRemainingClassBehaviors;
            case BehaviorResult.RunContinuous:
              if (this.m_activeContinuousBehavior != null)
                BraveUtility.Log("Trying to overwrite the current continuous behaviors; this shouldn't happen.", UnityEngine.Color.red, BraveUtility.LogVerbosity.IMPORTANT);
              this.m_activeContinuousBehavior = behavior;
              return BehaviorResult.SkipAllRemainingBehaviors;
            default:
              continue;
          }
        }
      }
      return BehaviorResult.Continue;
    }

    private void EndContinuousBehavior()
    {
      if (this.m_activeContinuousBehavior == null)
        return;
      BehaviorBase continuousBehavior = this.m_activeContinuousBehavior;
      this.m_activeContinuousBehavior = (BehaviorBase) null;
      continuousBehavior.EndContinuousUpdate();
    }

    private void EndClassSpecificContinuousBehavior(IList key)
    {
      BehaviorBase behaviorBase;
      if (!this.m_classSpecificContinuousBehavior.TryGetValue(key, out behaviorBase))
        return;
      this.m_classSpecificContinuousBehavior.Remove(key);
      behaviorBase.EndContinuousUpdate();
    }

    private void RefreshBehaviors(IList behaviors, List<BehaviorBase> oldBehaviors)
    {
      for (int index = 0; index < behaviors.Count; ++index)
      {
        if (!oldBehaviors.Contains(behaviors[index] as BehaviorBase))
        {
          BehaviorBase behavior = behaviors[index] as BehaviorBase;
          behavior.Init(this.gameObject, this.m_aiActor, this.aiShooter);
          behavior.Start();
        }
        this.m_behaviors.Add(behaviors[index] as BehaviorBase);
      }
    }

    private void FirstUpdate()
    {
      if (this.SkipTimingDifferentiator || !(bool) (UnityEngine.Object) this.aiActor || (bool) (UnityEngine.Object) this.healthHaver && this.healthHaver.IsBoss || this.aiActor.ParentRoom == null)
        return;
      int num = 0;
      List<AIActor> activeEnemies = this.aiActor.ParentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
      if (activeEnemies == null)
        return;
      for (int index = 0; index < activeEnemies.Count; ++index)
      {
        if ((bool) (UnityEngine.Object) activeEnemies[index] && activeEnemies[index].EnemyGuid == this.aiActor.EnemyGuid)
        {
          ++num;
          if (num == 1 && (UnityEngine.Object) activeEnemies[index] == (UnityEngine.Object) this.aiActor)
            return;
        }
      }
      if (num <= 1)
        return;
      float quickestCooldown = float.MaxValue;
      this.ProcessAttacks((Action<AttackBehaviorBase>) (attackBase =>
      {
        BasicAttackBehavior basicAttackBehavior = attackBase as BasicAttackBehavior;
        if (attackBase is SequentialAttackBehaviorGroup)
        {
          SequentialAttackBehaviorGroup attackBehaviorGroup = attackBase as SequentialAttackBehaviorGroup;
          basicAttackBehavior = attackBehaviorGroup.AttackBehaviors[attackBehaviorGroup.AttackBehaviors.Count - 1] as BasicAttackBehavior;
        }
        if (basicAttackBehavior == null)
          return;
        if ((double) basicAttackBehavior.CooldownVariance < 0.20000000298023224)
          basicAttackBehavior.CooldownVariance = 0.2f;
        float[] numArray = new float[4]
        {
          basicAttackBehavior.Cooldown,
          basicAttackBehavior.GlobalCooldown,
          basicAttackBehavior.GroupCooldown,
          basicAttackBehavior.InitialCooldown
        };
        quickestCooldown = Mathf.Min(quickestCooldown, Mathf.Max(numArray));
      }), true);
      if ((double) quickestCooldown >= 3.4028234663852886E+38 || this.InstantFirstTick)
        return;
      this.AttackCooldown = UnityEngine.Random.Range(0.0f, Mathf.Max(quickestCooldown, 4f));
    }

    private void ProcessAttacks(Action<AttackBehaviorBase> func, bool skipSimultaneous = false)
    {
      for (int index = 0; index < this.AttackBehaviors.Count; ++index)
        this.ProcessAttacksRecursive(this.AttackBehaviors[index], func, skipSimultaneous);
    }

    private void ProcessAttacksRecursive(
      AttackBehaviorBase attack,
      Action<AttackBehaviorBase> func,
      bool skipSimultaneous)
    {
      if (attack is IAttackBehaviorGroup)
      {
        IAttackBehaviorGroup attackBehaviorGroup = attack as IAttackBehaviorGroup;
        if (skipSimultaneous && attack is SimultaneousAttackBehaviorGroup)
          return;
        for (int index = 0; index < attackBehaviorGroup.Count; ++index)
          this.ProcessAttacksRecursive(attackBehaviorGroup.GetAttackBehavior(index), func, skipSimultaneous);
      }
      else
        func(attack);
    }
  }

