// Decompiled with JetBrains decompiler
// Type: CrazedController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class CrazedController : BraveBehaviour
{
  [CheckDirectionalAnimation(null)]
  public string TellAnimation;
  public bool SpecifyTellDuration = true;
  [ShowInInspectorIf("SpecifyTellDuration", true)]
  public float TellDuration;
  public bool DoCharge = true;
  [ShowInInspectorIf("DoCharge", true)]
  public string CrazedAnimaton;
  [ShowInInspectorIf("DoCharge", true)]
  public float CrazedRunSpeed = -1f;
  public bool EnableBehavior;
  [ShowInInspectorIf("EnableBehavior", true)]
  public string BehaviorName;
  public bool TriggerWhenLastEnemy = true;
  public bool DisableHitAnims;
  private CrazedController.State m_state;
  private static List<AIActor> s_activeEnemies = new List<AIActor>();

  public void Update()
  {
    if (!(bool) (Object) this.aiActor || !this.aiActor.enabled || !(bool) (Object) this.healthHaver || this.healthHaver.IsDead)
      return;
    if (this.m_state == CrazedController.State.Idle)
    {
      if (!this.TriggerWhenLastEnemy || !this.behaviorSpeculator.enabled)
        return;
      this.aiActor.ParentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear, ref CrazedController.s_activeEnemies);
      bool flag = false;
      for (int index = 0; index < CrazedController.s_activeEnemies.Count; ++index)
      {
        if (CrazedController.s_activeEnemies[index].EnemyGuid != this.aiActor.EnemyGuid)
        {
          flag = true;
          break;
        }
      }
      if (flag)
        return;
      this.GoCrazed();
    }
    else if (this.m_state == CrazedController.State.Transforming)
    {
      this.behaviorSpeculator.GlobalCooldown = 1f;
      if (!this.behaviorSpeculator.enabled)
      {
        this.m_state = CrazedController.State.Idle;
      }
      else
      {
        if (this.aiAnimator.IsPlaying(this.TellAnimation))
          return;
        this.DoCrazedBehavior();
      }
    }
    else if (this.m_state != CrazedController.State.Crazed)
      ;
  }

  public void GoCrazed()
  {
    this.aiActor.ClearPath();
    this.behaviorSpeculator.GlobalCooldown = 1f;
    if (this.DisableHitAnims)
      this.aiActor.aiAnimator.HitAnimation.Type = DirectionalAnimation.DirectionType.None;
    if (!string.IsNullOrEmpty(this.TellAnimation))
    {
      if (!this.SpecifyTellDuration)
        this.aiAnimator.PlayUntilFinished(this.TellAnimation, true);
      else
        this.aiAnimator.PlayForDurationOrUntilFinished(this.TellAnimation, this.TellDuration, true);
      this.m_state = CrazedController.State.Transforming;
    }
    else
      this.DoCrazedBehavior();
  }

  private void DoCrazedBehavior()
  {
    this.behaviorSpeculator.GlobalCooldown = 0.0f;
    if (this.DoCharge)
    {
      this.aiAnimator.SetBaseAnim(this.CrazedAnimaton);
      this.behaviorSpeculator.MovementBehaviors.Clear();
      this.behaviorSpeculator.AttackBehaviors.Clear();
      this.behaviorSpeculator.MovementBehaviors.Add((MovementBehaviorBase) new SeekTargetBehavior()
      {
        StopWhenInRange = false,
        CustomRange = -1f,
        LineOfSight = false,
        ReturnToSpawn = false,
        SpawnTetherDistance = 0.0f,
        PathInterval = 0.25f
      });
      this.behaviorSpeculator.RefreshBehaviors();
      this.behaviorSpeculator.enabled = true;
      if ((double) this.CrazedRunSpeed > 0.0)
        this.aiActor.MovementSpeed = this.CrazedRunSpeed;
      this.aiActor.CollisionDamage = 0.5f;
    }
    if (this.EnableBehavior)
    {
      for (int index = 0; index < this.behaviorSpeculator.AttackBehaviors.Count; ++index)
      {
        if (this.behaviorSpeculator.AttackBehaviors[index] is AttackBehaviorGroup)
          this.ProcessAttackGroup(this.behaviorSpeculator.AttackBehaviors[index] as AttackBehaviorGroup);
      }
      this.behaviorSpeculator.enabled = true;
    }
    this.m_state = CrazedController.State.Crazed;
  }

  private void ProcessAttackGroup(AttackBehaviorGroup attackGroup)
  {
    for (int index = 0; index < attackGroup.AttackBehaviors.Count; ++index)
    {
      AttackBehaviorGroup.AttackGroupItem attackBehavior = attackGroup.AttackBehaviors[index];
      if (attackBehavior.NickName == this.BehaviorName)
        attackBehavior.Probability = 1f;
      if (attackBehavior.Behavior is AttackBehaviorGroup)
        this.ProcessAttackGroup(attackBehavior.Behavior as AttackBehaviorGroup);
    }
  }

  private enum State
  {
    Idle,
    Transforming,
    Crazed,
  }
}
