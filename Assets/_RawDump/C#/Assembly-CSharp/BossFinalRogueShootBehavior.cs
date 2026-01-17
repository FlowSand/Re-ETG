// Decompiled with JetBrains decompiler
// Type: BossFinalRogueShootBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullInspector;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
[InspectorDropdownName("Bosses/BossFinalRogue/ShootBehavior")]
public class BossFinalRogueShootBehavior : BasicAttackBehavior
{
  public bool SuppressBaseGuns;
  public List<BossFinalRogueGunController> Guns;
  public bool CheckPlayerInArea;
  [InspectorShowIf("CheckPlayerInArea")]
  public ShootBehavior.FiringAreaStyle playerArea;
  [InspectorShowIf("CheckPlayerInArea")]
  public float playerAreaSetupTime;
  public bool EndIfAnyGunsFinish;
  private BossFinalRogueController m_ship;
  private float m_checkPlayerInAreaTimer;

  public override void Start()
  {
    base.Start();
    this.m_ship = this.m_aiActor.GetComponent<BossFinalRogueController>();
  }

  public override void Upkeep()
  {
    base.Upkeep();
    if (this.CheckPlayerInArea && BasicAttackBehavior.DrawDebugFiringArea && (bool) (Object) this.m_aiActor.TargetRigidbody)
      this.playerArea.DrawDebugLines(this.GetOrigin(this.playerArea.targetAreaOrigin), this.m_aiActor.TargetRigidbody.GetUnitCenter(ColliderType.HitBox), this.m_aiActor);
    if (!this.CheckPlayerInArea)
      return;
    this.DecrementTimer(ref this.m_checkPlayerInAreaTimer);
  }

  public override BehaviorResult Update()
  {
    int num = (int) base.Update();
    BehaviorResult behaviorResult = base.Update();
    if (behaviorResult != BehaviorResult.Continue)
      return behaviorResult;
    if (!this.IsReady())
      return BehaviorResult.Continue;
    for (int index = 0; index < this.Guns.Count; ++index)
      this.Guns[index].Fire();
    for (int index = 0; index < this.Guns.Count; ++index)
    {
      if (!this.Guns[index].IsFinished)
      {
        if (this.SuppressBaseGuns)
          this.m_ship.SuppressBaseGuns = true;
        if (this.CheckPlayerInArea)
          this.m_checkPlayerInAreaTimer = this.playerAreaSetupTime;
        this.m_updateEveryFrame = true;
        return BehaviorResult.RunContinuousInClass;
      }
    }
    this.UpdateCooldowns();
    return BehaviorResult.SkipRemainingClassBehaviors;
  }

  public override ContinuousBehaviorResult ContinuousUpdate()
  {
    if (this.CheckPlayerInArea && (double) this.m_checkPlayerInAreaTimer <= 0.0 && !this.TargetStillInFiringArea())
      return ContinuousBehaviorResult.Finished;
    if (this.EndIfAnyGunsFinish)
    {
      for (int index = 0; index < this.Guns.Count; ++index)
      {
        if (this.Guns[index].IsFinished)
          return ContinuousBehaviorResult.Finished;
      }
      return ContinuousBehaviorResult.Continue;
    }
    for (int index = 0; index < this.Guns.Count; ++index)
    {
      if (!this.Guns[index].IsFinished)
        return ContinuousBehaviorResult.Continue;
    }
    return ContinuousBehaviorResult.Finished;
  }

  public override void EndContinuousUpdate()
  {
    if (this.SuppressBaseGuns)
      this.m_ship.SuppressBaseGuns = false;
    for (int index = 0; index < this.Guns.Count; ++index)
      this.Guns[index].CeaseFire();
    this.m_updateEveryFrame = false;
    this.UpdateCooldowns();
  }

  protected bool TargetStillInFiringArea()
  {
    if (this.playerArea == null)
      return true;
    return (bool) (Object) this.m_aiActor.TargetRigidbody && this.playerArea.TargetInFiringArea(this.GetOrigin(this.playerArea.targetAreaOrigin), this.m_aiActor.TargetRigidbody.GetUnitCenter(ColliderType.HitBox));
  }
}
