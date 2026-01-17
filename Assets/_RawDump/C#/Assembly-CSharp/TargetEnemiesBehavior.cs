// Decompiled with JetBrains decompiler
// Type: TargetEnemiesBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class TargetEnemiesBehavior : TargetBehaviorBase
{
  public bool LineOfSight = true;
  public bool ObjectPermanence = true;
  public float SearchInterval = 0.25f;
  private float m_losTimer;

  public override void Start()
  {
  }

  public override void Upkeep()
  {
    base.Upkeep();
    this.DecrementTimer(ref this.m_losTimer);
  }

  public override BehaviorResult Update()
  {
    BehaviorResult behaviorResult = base.Update();
    if (behaviorResult != BehaviorResult.Continue)
      return behaviorResult;
    if ((double) this.m_losTimer > 0.0)
      return BehaviorResult.Continue;
    this.m_losTimer = this.SearchInterval;
    if ((bool) (Object) this.m_aiActor.PlayerTarget)
    {
      if (this.m_aiActor.PlayerTarget.IsFalling)
      {
        this.m_aiActor.PlayerTarget = (GameActor) null;
        this.m_aiActor.ClearPath();
        return BehaviorResult.SkipRemainingClassBehaviors;
      }
      if ((bool) (Object) this.m_aiActor.PlayerTarget.healthHaver && this.m_aiActor.PlayerTarget.healthHaver.IsDead)
      {
        this.m_aiActor.PlayerTarget = (GameActor) null;
        this.m_aiActor.ClearPath();
        return BehaviorResult.SkipRemainingClassBehaviors;
      }
    }
    else
      this.m_aiActor.PlayerTarget = (GameActor) null;
    if (!this.ObjectPermanence)
      this.m_aiActor.PlayerTarget = (GameActor) null;
    if ((Object) this.m_aiActor.PlayerTarget != (Object) null || !this.m_aiActor.CanTargetEnemies)
      return BehaviorResult.Continue;
    List<AIActor> activeEnemies = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(this.m_aiActor.GridPosition).GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
    if (activeEnemies != null && activeEnemies.Count > 0)
    {
      AIActor aiActor1 = (AIActor) null;
      float num = float.MaxValue;
      for (int index = 0; index < activeEnemies.Count; ++index)
      {
        AIActor aiActor2 = activeEnemies[index];
        if (!((Object) aiActor2 == (Object) this.m_aiActor))
        {
          float dist = Vector2.Distance(this.m_aiActor.CenterPosition, aiActor2.CenterPosition);
          if ((double) dist < (double) num)
          {
            if (this.LineOfSight)
            {
              int playerVisibilityMask = CollisionMask.StandardPlayerVisibilityMask;
              RaycastResult result;
              if (!PhysicsEngine.Instance.Raycast(this.m_aiActor.CenterPosition, aiActor2.CenterPosition - this.m_aiActor.CenterPosition, dist, out result, rayMask: playerVisibilityMask, ignoreRigidbody: this.m_aiActor.specRigidbody))
              {
                RaycastResult.Pool.Free(ref result);
                continue;
              }
              if ((Object) result.SpeculativeRigidbody == (Object) null || (Object) result.SpeculativeRigidbody.GetComponent<PlayerController>() == (Object) null)
              {
                RaycastResult.Pool.Free(ref result);
                continue;
              }
              RaycastResult.Pool.Free(ref result);
            }
            aiActor1 = aiActor2;
            num = dist;
          }
        }
      }
      this.m_aiActor.PlayerTarget = (GameActor) aiActor1;
    }
    if ((Object) this.m_aiShooter != (Object) null && (Object) this.m_aiActor.PlayerTarget != (Object) null)
      this.m_aiShooter.AimAtPoint(this.m_aiActor.PlayerTarget.CenterPosition);
    if (this.m_aiActor.HasBeenEngaged)
      return BehaviorResult.SkipRemainingClassBehaviors;
    this.m_aiActor.HasBeenEngaged = true;
    return BehaviorResult.SkipAllRemainingBehaviors;
  }
}
