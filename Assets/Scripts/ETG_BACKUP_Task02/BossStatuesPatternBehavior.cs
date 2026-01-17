// Decompiled with JetBrains decompiler
// Type: BossStatuesPatternBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public abstract class BossStatuesPatternBehavior : BasicAttackBehavior
{
  public string numStatues = "2-4";
  public float OverrideMoveSpeed = -1f;
  public bool waitForStartingPositions = true;
  public BossStatuesPatternBehavior.StatueAttack attackType;
  protected BossStatuesPatternBehavior.PatternState m_state;
  protected int[] numStatuesArray;
  protected BossStatuesController m_statuesController;
  protected List<BossStatueController> m_activeStatues;
  protected int m_activeStatueCount;
  protected float m_stateTimer;
  protected float m_timeElapsed;

  protected BossStatuesPatternBehavior.PatternState State
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

  public override void Start()
  {
    base.Start();
    this.m_statuesController = this.m_gameObject.GetComponent<BossStatuesController>();
    this.m_activeStatues = new List<BossStatueController>((IEnumerable<BossStatueController>) this.m_statuesController.allStatues);
    this.UpdateNumStatuesArray();
  }

  public override void Upkeep()
  {
    base.Upkeep();
    this.DecrementTimer(ref this.m_stateTimer);
  }

  public override BehaviorResult Update()
  {
    BehaviorResult behaviorResult = base.Update();
    if (behaviorResult != BehaviorResult.Continue)
      return behaviorResult;
    if (!this.IsReady())
      return BehaviorResult.Continue;
    this.m_updateEveryFrame = true;
    this.RefreshActiveStatues();
    this.InitPositions();
    if (this.attackType != null)
      this.attackType.Start(this.m_activeStatues);
    this.State = !this.waitForStartingPositions ? BossStatuesPatternBehavior.PatternState.InProgress : BossStatuesPatternBehavior.PatternState.MovingToStartingPosition;
    return BehaviorResult.RunContinuous;
  }

  public override ContinuousBehaviorResult ContinuousUpdate()
  {
    int num = (int) base.ContinuousUpdate();
    if (this.State != BossStatuesPatternBehavior.PatternState.Ending && this.AnyStatuesHaveDied())
    {
      for (int index = 0; index < this.m_activeStatueCount; ++index)
      {
        BossStatueController activeStatue = this.m_activeStatues[index];
        if ((bool) (UnityEngine.Object) activeStatue)
          activeStatue.ForceStopBulletScript();
      }
      this.OnStatueDeath();
      this.State = BossStatuesPatternBehavior.PatternState.Ending;
      return ContinuousBehaviorResult.Continue;
    }
    if (this.State == BossStatuesPatternBehavior.PatternState.MovingToStartingPosition)
    {
      if ((double) this.m_stateTimer <= 0.0)
      {
        this.SetActiveState(BossStatueController.StatueState.StandStill);
        if (this.AreAllGroundedAndReadyToJump())
          this.State = BossStatuesPatternBehavior.PatternState.InProgress;
      }
    }
    else if (this.State == BossStatuesPatternBehavior.PatternState.InProgress)
    {
      float timeElapsed = this.m_timeElapsed;
      this.m_timeElapsed += this.m_deltaTime;
      this.UpdatePositions();
      if (this.attackType != null)
        this.attackType.Update(timeElapsed, this.m_timeElapsed, this.m_activeStatues);
      if (this.IsFinished())
        this.State = BossStatuesPatternBehavior.PatternState.Ending;
    }
    return this.State == BossStatuesPatternBehavior.PatternState.Ending && this.AreAllGroundedAndReadyToJump() ? ContinuousBehaviorResult.Finished : ContinuousBehaviorResult.Continue;
  }

  public override void EndContinuousUpdate()
  {
    base.EndContinuousUpdate();
    for (int index = 0; index < this.m_activeStatueCount; ++index)
    {
      this.m_activeStatues[index].ClearQueuedAttacks();
      this.m_activeStatues[index].State = BossStatueController.StatueState.WaitForAttack;
    }
    this.m_updateEveryFrame = false;
    this.UpdateCooldowns();
    this.State = BossStatuesPatternBehavior.PatternState.Idle;
  }

  public override bool IsOverridable() => false;

  public override bool IsReady()
  {
    if (!base.IsReady() || Array.IndexOf<int>(this.numStatuesArray, this.m_statuesController.NumLivingStatues) < 0)
      return false;
    for (int index = 0; index < this.m_activeStatues.Count; ++index)
    {
      BossStatueController activeStatue = this.m_activeStatues[index];
      if ((bool) (UnityEngine.Object) activeStatue && activeStatue.healthHaver.IsAlive && activeStatue.IsTransforming)
        return false;
    }
    return true;
  }

  protected virtual void BeginState(BossStatuesPatternBehavior.PatternState state)
  {
    switch (state)
    {
      case BossStatuesPatternBehavior.PatternState.MovingToStartingPosition:
        this.m_statuesController.IsTransitioning = true;
        this.m_stateTimer = 0.0f;
        this.SetActiveState(BossStatueController.StatueState.HopToTarget);
        float effectiveMoveSpeed = this.m_statuesController.GetEffectiveMoveSpeed(this.m_statuesController.transitionMoveSpeed);
        for (int index = 0; index < this.m_activeStatueCount; ++index)
          this.m_stateTimer = Mathf.Max(this.m_stateTimer, this.m_activeStatues[index].DistancetoTarget / effectiveMoveSpeed);
        break;
      case BossStatuesPatternBehavior.PatternState.InProgress:
        this.m_timeElapsed = 0.0f;
        this.SetActiveState(BossStatueController.StatueState.HopToTarget);
        if ((double) this.OverrideMoveSpeed > 0.0)
          this.m_statuesController.OverrideMoveSpeed = new float?(this.OverrideMoveSpeed);
        if (this.attackType == null)
          break;
        this.attackType.Update(-0.02f, 0.0f, this.m_activeStatues);
        break;
      case BossStatuesPatternBehavior.PatternState.Ending:
        this.SetActiveState(BossStatueController.StatueState.StandStill);
        break;
    }
  }

  protected virtual void EndState(BossStatuesPatternBehavior.PatternState state)
  {
    switch (state)
    {
      case BossStatuesPatternBehavior.PatternState.MovingToStartingPosition:
        this.m_statuesController.IsTransitioning = false;
        break;
      case BossStatuesPatternBehavior.PatternState.InProgress:
        if ((double) this.OverrideMoveSpeed <= 0.0)
          break;
        this.m_statuesController.OverrideMoveSpeed = new float?();
        break;
    }
  }

  protected void SetActiveState(BossStatueController.StatueState newState)
  {
    for (int index = 0; index < this.m_activeStatueCount; ++index)
    {
      BossStatueController activeStatue = this.m_activeStatues[index];
      if ((bool) (UnityEngine.Object) activeStatue && activeStatue.healthHaver.IsAlive)
        activeStatue.State = newState;
    }
  }

  protected bool AreAllGrounded()
  {
    for (int index = 0; index < this.m_activeStatueCount; ++index)
    {
      BossStatueController activeStatue = this.m_activeStatues[index];
      if ((bool) (UnityEngine.Object) activeStatue && activeStatue.healthHaver.IsAlive && !activeStatue.IsGrounded)
        return false;
    }
    return true;
  }

  protected bool AreAllGroundedAndReadyToJump()
  {
    for (int index = 0; index < this.m_activeStatueCount; ++index)
    {
      BossStatueController activeStatue = this.m_activeStatues[index];
      if ((bool) (UnityEngine.Object) activeStatue && activeStatue.healthHaver.IsAlive && (!activeStatue.IsGrounded || !activeStatue.ReadyToJump))
        return false;
    }
    return true;
  }

  private bool AnyStatuesHaveDied()
  {
    for (int index = 0; index < this.m_activeStatueCount; ++index)
    {
      BossStatueController activeStatue = this.m_activeStatues[index];
      if (!(bool) (UnityEngine.Object) activeStatue || activeStatue.healthHaver.IsDead)
        return true;
    }
    return false;
  }

  private void RefreshActiveStatues()
  {
    for (int index = this.m_activeStatues.Count - 1; index >= 0; --index)
    {
      if (!(bool) (UnityEngine.Object) this.m_activeStatues[index] || this.m_activeStatues[index].healthHaver.IsDead)
        this.m_activeStatues.RemoveAt(index);
    }
    this.m_activeStatueCount = this.m_activeStatues.Count;
  }

  private void UpdateNumStatuesArray()
  {
    this.numStatuesArray = BraveUtility.ParsePageNums(this.numStatues);
  }

  protected abstract void InitPositions();

  protected abstract void UpdatePositions();

  protected abstract bool IsFinished();

  protected virtual void OnStatueDeath()
  {
  }

  protected void ReorderStatues(Vector2[] positions)
  {
    int[] numList = new int[this.m_activeStatueCount];
    for (int index = 0; index < this.m_activeStatueCount; ++index)
      numList[index] = index;
    float num1 = float.MaxValue;
    int[] destinationArray = new int[this.m_activeStatueCount];
    do
    {
      int index1 = 0;
      float num2 = 0.0f;
      for (int index2 = 0; index2 < this.m_activeStatueCount; ++index2)
      {
        if ((bool) (UnityEngine.Object) this.m_activeStatues[index1])
          num2 += Vector2.Distance(this.m_activeStatues[index1].GroundPosition, positions[numList[index2]]);
        ++index1;
      }
      if ((double) num2 < (double) num1)
      {
        num1 = num2;
        Array.Copy((Array) numList, (Array) destinationArray, this.m_activeStatueCount);
      }
    }
    while (BraveMathCollege.NextPermutation(ref numList));
    List<BossStatueController> statueControllerList = new List<BossStatueController>((IEnumerable<BossStatueController>) this.m_activeStatues);
    for (int index = 0; index < this.m_activeStatueCount; ++index)
      this.m_activeStatues[index] = statueControllerList[destinationArray[index]];
  }

  protected enum PatternState
  {
    Idle,
    MovingToStartingPosition,
    InProgress,
    Ending,
  }

  [Serializable]
  public abstract class StatueAttack
  {
    public virtual void Start(List<BossStatueController> statues)
    {
    }

    public abstract void Update(
      float prevTimeElapsed,
      float timeElapsed,
      List<BossStatueController> statues);
  }

  [Serializable]
  public class TimedAttacks : BossStatuesPatternBehavior.StatueAttack
  {
    public List<BossStatuesPatternBehavior.TimedAttacks.TimedAttack> attacks;

    public override void Update(
      float prevTimeElapsed,
      float timeElapsed,
      List<BossStatueController> statues)
    {
      for (int index = 0; index < this.attacks.Count; ++index)
      {
        BossStatuesPatternBehavior.TimedAttacks.TimedAttack attack = this.attacks[index];
        if ((double) prevTimeElapsed < (double) attack.delay && (double) timeElapsed >= (double) attack.delay && attack.index < statues.Count)
          statues[attack.index].QueuedBulletScript.Add(attack.bulletScript);
      }
    }

    [Serializable]
    public class TimedAttack
    {
      public int index;
      public float delay;
      public BulletScriptSelector bulletScript;
    }
  }

  [Serializable]
  public class ConstantAttacks : BossStatuesPatternBehavior.StatueAttack
  {
    public List<BossStatuesPatternBehavior.ConstantAttacks.ConstantAttackGroup> attacks;
    [NonSerialized]
    private int[] m_bulletScriptIndices;

    public override void Start(List<BossStatueController> statues)
    {
      this.m_bulletScriptIndices = new int[statues.Count];
    }

    public override void Update(
      float prevTimeElapsed,
      float timeElapsed,
      List<BossStatueController> statues)
    {
      for (int index1 = 0; index1 < this.attacks.Count; ++index1)
      {
        BossStatuesPatternBehavior.ConstantAttacks.ConstantAttackGroup attack = this.attacks[index1];
        int index2 = attack.index;
        if (index2 < statues.Count && statues[index2].QueuedBulletScript.Count == 0)
        {
          int index3 = (this.m_bulletScriptIndices[index2] + 1) % attack.bulletScript.Count;
          this.m_bulletScriptIndices[index2] = index3;
          BulletScriptSelector bulletScriptSelector = attack.bulletScript[index3];
          statues[index2].QueuedBulletScript.Add(bulletScriptSelector);
        }
      }
    }

    [Serializable]
    public class ConstantAttackGroup
    {
      public int index;
      public List<BulletScriptSelector> bulletScript;
    }
  }
}
