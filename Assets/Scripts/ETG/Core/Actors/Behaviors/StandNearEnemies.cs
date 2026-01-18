using Dungeonator;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

public class StandNearEnemies : MovementBehaviorBase
  {
    public float PathInterval = 0.25f;
    public float DesiredDistance = 5f;
    private float m_repathTimer;
    private List<AIActor> m_roomEnemies = new List<AIActor>();
    private Vector2? m_targetPos;

    public override void Upkeep()
    {
      base.Upkeep();
      this.DecrementTimer(ref this.m_repathTimer);
    }

    public override BehaviorResult Update()
    {
      if ((double) this.m_repathTimer > 0.0)
        return this.m_targetPos.HasValue ? BehaviorResult.SkipRemainingClassBehaviors : BehaviorResult.Continue;
      this.UpdateTarget();
      if (this.m_targetPos.HasValue)
      {
        this.m_aiActor.PathfindToPosition(this.m_targetPos.Value);
        this.m_repathTimer = this.PathInterval;
      }
      return this.m_targetPos.HasValue ? BehaviorResult.SkipRemainingClassBehaviors : BehaviorResult.Continue;
    }

    private void UpdateTarget()
    {
      this.m_aiActor.ParentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All, ref this.m_roomEnemies);
      for (int index = this.m_roomEnemies.Count - 1; index >= 0; --index)
      {
        AIActor roomEnemy = this.m_roomEnemies[index];
        if (roomEnemy.IsHarmlessEnemy || !roomEnemy.IsNormalEnemy || roomEnemy.healthHaver.IsDead || (Object) roomEnemy == (Object) this.m_aiActor)
          this.m_roomEnemies.RemoveAt(index);
      }
      if (this.m_roomEnemies.Count <= 0)
      {
        this.m_targetPos = new Vector2?();
      }
      else
      {
        bool flag = false;
        while (!flag)
        {
          flag = true;
          Vector2 zero = Vector2.zero;
          int prevCount = 0;
          for (int index = 0; index < this.m_roomEnemies.Count; ++index)
            BraveMathCollege.WeightedAverage(this.m_roomEnemies[index].specRigidbody.UnitCenter, ref zero, ref prevCount);
          if (prevCount == 1)
          {
            Vector2 normalized = (this.m_aiActor.specRigidbody.UnitCenter - zero).normalized;
            BraveMathCollege.WeightedAverage(zero + normalized * this.DesiredDistance, ref zero, ref prevCount);
          }
          int index1 = -1;
          float num1 = float.MinValue;
          for (int index2 = 0; index2 < this.m_roomEnemies.Count; ++index2)
          {
            float num2 = Vector2.Distance(this.m_roomEnemies[index2].specRigidbody.UnitCenter, zero);
            if ((double) num2 > (double) this.DesiredDistance && (double) num2 > (double) num1)
            {
              index1 = index2;
              num1 = num2;
            }
          }
          if (index1 >= 0)
          {
            this.m_roomEnemies.RemoveAt(index1);
            flag = false;
          }
          this.m_targetPos = new Vector2?(zero);
        }
      }
    }
  }

