// Decompiled with JetBrains decompiler
// Type: GunHandBasicShootBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable

public class GunHandBasicShootBehavior : BasicAttackBehavior
  {
    public bool LineOfSight = true;
    public bool FireAllGuns;
    public List<GunHandController> GunHands;

    public override void Start() => base.Start();

    public override void Upkeep() => base.Upkeep();

    public override BehaviorResult Update()
    {
      int num = (int) base.Update();
      BehaviorResult behaviorResult = base.Update();
      if (behaviorResult != BehaviorResult.Continue)
        return behaviorResult;
      if (!this.IsReady())
        return BehaviorResult.Continue;
      bool flag = this.LineOfSight && !this.m_aiActor.HasLineOfSightToTarget;
      if ((Object) this.m_aiActor.TargetRigidbody == (Object) null || flag)
      {
        for (int index = 0; index < this.GunHands.Count; ++index)
        {
          if ((bool) (Object) this.GunHands[index])
            this.GunHands[index].CeaseAttack();
        }
        return BehaviorResult.Continue;
      }
      if (this.FireAllGuns)
      {
        for (int index = 0; index < this.GunHands.Count; ++index)
        {
          if ((bool) (Object) this.GunHands[index])
            this.GunHands[index].StartFiring();
        }
        this.UpdateCooldowns();
        return BehaviorResult.SkipRemainingClassBehaviors;
      }
      GunHandController gunHandController = BraveUtility.RandomElement<GunHandController>(this.GunHands);
      if ((bool) (Object) gunHandController)
        gunHandController.StartFiring();
      this.UpdateCooldowns();
      return BehaviorResult.SkipRemainingClassBehaviors;
    }

    public override bool IsReady()
    {
      if (!base.IsReady())
        return false;
      if (this.FireAllGuns)
      {
        for (int index = 0; index < this.GunHands.Count; ++index)
        {
          if (!this.GunHands[index].IsReady)
            return false;
        }
        return true;
      }
      for (int index = 0; index < this.GunHands.Count; ++index)
      {
        if (this.GunHands[index].IsReady)
          return true;
      }
      return false;
    }
  }

