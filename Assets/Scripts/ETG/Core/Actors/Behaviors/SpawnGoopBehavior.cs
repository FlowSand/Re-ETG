using System.Collections.Generic;
using UnityEngine;

#nullable disable

public class SpawnGoopBehavior : BasicAttackBehavior
  {
    public List<Vector2> roomOffsets;
    public GoopDefinition goopToUse;
    public float goopRadius = 3f;
    public float goopSpeed = 0.5f;

    public override BehaviorResult Update()
    {
      BehaviorResult behaviorResult = base.Update();
      if (behaviorResult != BehaviorResult.Continue)
        return behaviorResult;
      if (!this.IsReady())
        return BehaviorResult.Continue;
      Vector2 center = this.m_aiActor.ParentRoom.area.UnitBottomLeft + BraveUtility.RandomElement<Vector2>(this.roomOffsets);
      DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(this.goopToUse).TimedAddGoopCircle(center, this.goopRadius, this.goopSpeed);
      this.UpdateCooldowns();
      return BehaviorResult.SkipAllRemainingBehaviors;
    }
  }

