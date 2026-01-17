// Decompiled with JetBrains decompiler
// Type: SequentialAttackBehaviorGroup
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullInspector;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
[InspectorDropdownName(".Groups/SequentialAttackBehaviorGroup")]
public class SequentialAttackBehaviorGroup : AttackBehaviorBase
{
  public bool RunInClass;
  [InspectorCollectionRotorzFlags(HideRemoveButtons = true)]
  public List<AttackBehaviorBase> AttackBehaviors;
  public List<float> OverrideCooldowns;
  private int m_currentIndex = -1;
  private float m_overrideCooldownTimer;
  private SequentialAttackBehaviorGroup.State m_state;

  private AttackBehaviorBase currentBehavior => this.AttackBehaviors[this.m_currentIndex];

  public override void Start()
  {
    base.Start();
    for (int index = 0; index < this.AttackBehaviors.Count; ++index)
      this.AttackBehaviors[index].Start();
  }

  public override void Upkeep()
  {
    base.Upkeep();
    for (int index = 0; index < this.AttackBehaviors.Count; ++index)
      this.AttackBehaviors[index].Upkeep();
  }

  public override bool OverrideOtherBehaviors()
  {
    for (int index = 0; index < this.AttackBehaviors.Count; ++index)
    {
      if (!this.AttackBehaviors[index].OverrideOtherBehaviors())
        return false;
    }
    return true;
  }

  public override BehaviorResult Update()
  {
    BehaviorResult behaviorResult = base.Update();
    if (behaviorResult != BehaviorResult.Continue)
      return behaviorResult;
    if (!this.IsReady())
      return BehaviorResult.Continue;
    this.m_currentIndex = 0;
    this.m_state = SequentialAttackBehaviorGroup.State.Update;
    if (!this.StepBehaviors())
      return BehaviorResult.SkipAllRemainingBehaviors;
    return this.RunInClass ? BehaviorResult.RunContinuousInClass : BehaviorResult.RunContinuous;
  }

  public override ContinuousBehaviorResult ContinuousUpdate()
  {
    int num = (int) base.ContinuousUpdate();
    return this.StepBehaviors() ? ContinuousBehaviorResult.Continue : ContinuousBehaviorResult.Finished;
  }

  public override void EndContinuousUpdate()
  {
    base.EndContinuousUpdate();
    if (this.m_currentIndex < this.AttackBehaviors.Count)
      this.AttackBehaviors[this.m_currentIndex].EndContinuousUpdate();
    this.m_currentIndex = -1;
  }

  public override void Destroy()
  {
    for (int index = 0; index < this.AttackBehaviors.Count; ++index)
      this.AttackBehaviors[index].Destroy();
    base.Destroy();
  }

  private bool StepBehaviors()
  {
    if (this.m_state == SequentialAttackBehaviorGroup.State.Cooldown)
    {
      this.m_overrideCooldownTimer += this.m_deltaTime;
      if (this.m_currentIndex == this.AttackBehaviors.Count - 1)
        return false;
      bool flag = false;
      if (this.OverrideCooldowns != null && this.OverrideCooldowns.Count > 0)
        flag = (double) this.m_overrideCooldownTimer >= (double) this.OverrideCooldowns[this.m_currentIndex];
      else if (this.currentBehavior.IsReady())
        flag = true;
      if (!flag)
        return true;
      ++this.m_currentIndex;
      this.m_state = SequentialAttackBehaviorGroup.State.Update;
      return this.StepBehaviors();
    }
    if (this.m_state == SequentialAttackBehaviorGroup.State.Update)
    {
      BehaviorResult behaviorResult = this.currentBehavior.Update();
      switch (behaviorResult)
      {
        case BehaviorResult.Continue:
        case BehaviorResult.SkipRemainingClassBehaviors:
        case BehaviorResult.SkipAllRemainingBehaviors:
          this.m_state = SequentialAttackBehaviorGroup.State.Cooldown;
          this.m_overrideCooldownTimer = 0.0f;
          return this.StepBehaviors();
        case BehaviorResult.RunContinuousInClass:
        case BehaviorResult.RunContinuous:
          this.m_state = SequentialAttackBehaviorGroup.State.ContinuousUpdate;
          return true;
        default:
          Debug.LogError((object) ("Unrecognized BehaviorResult " + (object) behaviorResult));
          return false;
      }
    }
    else if (this.m_state == SequentialAttackBehaviorGroup.State.ContinuousUpdate)
    {
      ContinuousBehaviorResult continuousBehaviorResult = this.currentBehavior.ContinuousUpdate();
      switch (continuousBehaviorResult)
      {
        case ContinuousBehaviorResult.Continue:
          return true;
        case ContinuousBehaviorResult.Finished:
          this.currentBehavior.EndContinuousUpdate();
          this.m_state = SequentialAttackBehaviorGroup.State.Cooldown;
          this.m_overrideCooldownTimer = 0.0f;
          return this.StepBehaviors();
        default:
          Debug.LogError((object) ("Unrecognized BehaviorResult " + (object) continuousBehaviorResult));
          return false;
      }
    }
    else
    {
      Debug.LogError((object) ("Unrecognized State " + (object) this.m_state));
      return false;
    }
  }

  public override void Init(GameObject gameObject, AIActor aiActor, AIShooter aiShooter)
  {
    base.Init(gameObject, aiActor, aiShooter);
    for (int index = 0; index < this.AttackBehaviors.Count; ++index)
      this.AttackBehaviors[index].Init(gameObject, aiActor, aiShooter);
  }

  public override void SetDeltaTime(float deltaTime)
  {
    base.SetDeltaTime(deltaTime);
    for (int index = 0; index < this.AttackBehaviors.Count; ++index)
      this.AttackBehaviors[index].SetDeltaTime(deltaTime);
  }

  public override bool IsReady()
  {
    for (int index = 0; index < this.AttackBehaviors.Count; ++index)
    {
      if (!this.AttackBehaviors[index].IsReady())
        return false;
    }
    return true;
  }

  public override float GetMinReadyRange()
  {
    if (!this.IsReady())
      return -1f;
    float a = float.MaxValue;
    for (int index = 0; index < this.AttackBehaviors.Count; ++index)
      a = Mathf.Min(a, this.AttackBehaviors[index].GetMinReadyRange());
    return a;
  }

  public override float GetMaxRange()
  {
    float a = float.MinValue;
    for (int index = 0; index < this.AttackBehaviors.Count; ++index)
      a = Mathf.Max(a, this.AttackBehaviors[index].GetMaxRange());
    return a;
  }

  public override bool UpdateEveryFrame()
  {
    return this.m_currentIndex >= 0 && this.currentBehavior.UpdateEveryFrame();
  }

  public override bool IsOverridable()
  {
    return this.m_currentIndex < 0 || this.currentBehavior.IsOverridable();
  }

  public override void OnActorPreDeath()
  {
    base.OnActorPreDeath();
    for (int index = 0; index < this.AttackBehaviors.Count; ++index)
      this.AttackBehaviors[index].OnActorPreDeath();
  }

  public int Count => this.AttackBehaviors.Count;

  public AttackBehaviorBase GetAttackBehavior(int index) => this.AttackBehaviors[index];

  private enum State
  {
    Idle,
    Update,
    ContinuousUpdate,
    Cooldown,
  }
}
