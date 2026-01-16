// Decompiled with JetBrains decompiler
// Type: DazedBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using Pathfinding;
using UnityEngine;

#nullable disable
public class DazedBehavior : OverrideBehaviorBase
{
  public float PointReachedPauseTime = 0.5f;
  public float PathInterval = 0.5f;
  private float m_repathTimer;
  private float m_pauseTimer;
  private IntVector2? m_targetPos;

  public override void Start() => base.Start();

  public override void Upkeep() => base.Upkeep();

  public override bool OverrideOtherBehaviors() => true;

  public override BehaviorResult Update()
  {
    this.m_repathTimer -= this.m_aiActor.LocalDeltaTime;
    this.m_pauseTimer -= this.m_aiActor.LocalDeltaTime;
    BehaviorResult behaviorResult = base.Update();
    if (behaviorResult != BehaviorResult.Continue)
      return behaviorResult;
    if (!this.m_targetPos.HasValue && (double) this.m_repathTimer > 0.0 || (double) this.m_pauseTimer > 0.0)
      return BehaviorResult.SkipAllRemainingBehaviors;
    if (this.m_targetPos.HasValue && this.m_aiActor.PathComplete)
    {
      this.m_targetPos = new IntVector2?();
      if ((double) this.PointReachedPauseTime > 0.0)
      {
        this.m_pauseTimer = this.PointReachedPauseTime;
        return BehaviorResult.SkipAllRemainingBehaviors;
      }
    }
    if ((double) this.m_repathTimer <= 0.0)
    {
      this.m_repathTimer = this.PathInterval;
      if (this.m_targetPos.HasValue && !this.SimpleCellValidator(this.m_targetPos.Value))
        this.m_targetPos = new IntVector2?();
      if (!this.m_targetPos.HasValue)
        this.m_targetPos = this.m_aiActor.ParentRoom.GetRandomAvailableCell(new IntVector2?(this.m_aiActor.Clearance), new CellTypes?(this.m_aiActor.PathableTiles), cellValidator: new CellValidator(this.SimpleCellValidator));
      if (!this.m_targetPos.HasValue)
        return BehaviorResult.SkipAllRemainingBehaviors;
      this.m_aiActor.PathfindToPosition(this.m_targetPos.Value.ToCenterVector2());
    }
    return BehaviorResult.SkipAllRemainingBehaviors;
  }

  private bool SimpleCellValidator(IntVector2 c)
  {
    if ((double) Vector2.Distance(c.ToVector2(), this.m_aiActor.CenterPosition) > 4.0)
      return false;
    for (int index1 = 0; index1 < this.m_aiActor.Clearance.x; ++index1)
    {
      for (int index2 = 0; index2 < this.m_aiActor.Clearance.y; ++index2)
      {
        if (GameManager.Instance.Dungeon.data.isTopWall(c.x + index1, c.y + index2))
          return false;
      }
    }
    return true;
  }
}
