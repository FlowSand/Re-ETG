// Decompiled with JetBrains decompiler
// Type: RedBarrelAwareness
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable
public class RedBarrelAwareness : OverrideBehaviorBase
{
  public bool AvoidRedBarrels = true;
  public bool ShootRedBarrels = true;
  public bool PushRedBarrels = true;
  protected List<MinorBreakable> m_roomRedBarrels;

  public override void Start()
  {
    base.Start();
    GameManager.Instance.Dungeon.StartCoroutine(this.Initialize());
  }

  [DebuggerHidden]
  private IEnumerator Initialize()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new RedBarrelAwareness.\u003CInitialize\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  public override BehaviorResult Update()
  {
    BehaviorResult behaviorResult = base.Update();
    if (behaviorResult != BehaviorResult.Continue)
      return behaviorResult;
    if ((Object) this.m_aiActor.OverrideTarget != (Object) null && !(bool) (Object) this.m_aiActor.OverrideTarget)
      this.m_aiActor.OverrideTarget = (SpeculativeRigidbody) null;
    for (int index = 0; index < this.m_roomRedBarrels.Count; ++index)
    {
      MinorBreakable roomRedBarrel = this.m_roomRedBarrels[index];
      if (!(bool) (Object) roomRedBarrel)
      {
        this.m_roomRedBarrels.RemoveAt(index);
        --index;
      }
      else if (roomRedBarrel.IsBroken)
      {
        if ((Object) this.m_aiActor.OverrideTarget == (Object) roomRedBarrel.specRigidbody)
          this.m_aiActor.OverrideTarget = (SpeculativeRigidbody) null;
        this.m_roomRedBarrels.RemoveAt(index);
        --index;
      }
    }
    if (this.AvoidRedBarrels)
      behaviorResult = this.HandleAvoidance();
    if (behaviorResult != BehaviorResult.Continue)
      return behaviorResult;
    if (this.ShootRedBarrels)
      behaviorResult = this.HandleShooting();
    if (behaviorResult != BehaviorResult.Continue)
      return behaviorResult;
    if (this.PushRedBarrels)
      behaviorResult = this.HandlePushing();
    return behaviorResult != BehaviorResult.Continue ? behaviorResult : behaviorResult;
  }

  protected BehaviorResult HandleAvoidance() => BehaviorResult.Continue;

  protected BehaviorResult HandleShooting()
  {
    if ((Object) this.m_aiActor.TargetRigidbody == (Object) null)
      return BehaviorResult.Continue;
    float desiredCombatDistance = this.m_aiActor.DesiredCombatDistance;
    for (int index = 0; index < this.m_roomRedBarrels.Count; ++index)
    {
      Vector2 unitCenter = this.m_roomRedBarrels[index].specRigidbody.UnitCenter;
      if (!GameManager.Instance.Dungeon.data.isTopWall((int) unitCenter.x, (int) unitCenter.y) && (double) Vector2.Distance(unitCenter, this.m_aiActor.TargetRigidbody.UnitCenter) <= (double) this.m_roomRedBarrels[index].explosionData.GetDefinedDamageRadius() && (double) Vector2.Distance(unitCenter, this.m_aiActor.specRigidbody.UnitCenter) <= (double) desiredCombatDistance * 1.25)
        this.m_aiActor.OverrideTarget = this.m_roomRedBarrels[index].specRigidbody;
    }
    return BehaviorResult.Continue;
  }

  protected BehaviorResult HandlePushing() => BehaviorResult.Continue;
}
