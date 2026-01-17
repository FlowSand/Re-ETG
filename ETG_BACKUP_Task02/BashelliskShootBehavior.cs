// Decompiled with JetBrains decompiler
// Type: BashelliskShootBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullInspector;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
[InspectorDropdownName("Bosses/Bashellisk/ShootBehavior")]
public class BashelliskShootBehavior : BasicAttackBehavior
{
  public BashelliskBodyController.ShootDirection shootDirection;
  public BulletScriptSelector BulletScript;
  public BashelliskShootBehavior.SegmentOrder segmentOrder = BashelliskShootBehavior.SegmentOrder.Sequence;
  [InspectorShowIf("ShowSegmentCount")]
  public int SegmentCount;
  [InspectorShowIf("ShowSegmentPercentage")]
  public float SegmentPercentage;
  public float SegmentDelay;
  public bool StopDuring;
  public bool WaitForAllSegmentsFree;
  private BashelliskHeadController m_bashellisk;
  private PooledLinkedList<BashelliskBodyController> m_segments = new PooledLinkedList<BashelliskBodyController>();
  private LinkedListNode<BashelliskBodyController> m_nextSegmentNode;
  private bool m_waitingToStart;
  private float m_segmentDelayTimer;

  private bool ShowSegmentCount() => (double) this.SegmentPercentage <= 0.0;

  private bool ShowSegmentPercentage() => this.SegmentCount <= 0;

  public override void Start()
  {
    base.Start();
    this.m_bashellisk = this.m_aiActor.GetComponent<BashelliskHeadController>();
  }

  public override void Upkeep()
  {
    base.Upkeep();
    this.m_segmentDelayTimer -= this.m_deltaTime * this.m_behaviorSpeculator.CooldownScale;
  }

  public override BehaviorResult Update()
  {
    BehaviorResult behaviorResult = base.Update();
    if (behaviorResult != BehaviorResult.Continue)
      return behaviorResult;
    if (!this.IsReady())
      return BehaviorResult.Continue;
    this.SelectSegments();
    this.m_waitingToStart = false;
    if (this.WaitForAllSegmentsFree)
    {
      this.m_waitingToStart = true;
      this.m_segmentDelayTimer = 0.0f;
    }
    else if ((double) this.SegmentDelay <= 0.0)
    {
      while (this.m_nextSegmentNode != null)
        this.FireNextSegment();
    }
    else
    {
      this.FireNextSegment();
      this.m_segmentDelayTimer = this.SegmentDelay;
    }
    this.m_updateEveryFrame = true;
    return this.StopDuring ? BehaviorResult.RunContinuous : BehaviorResult.RunContinuousInClass;
  }

  public override ContinuousBehaviorResult ContinuousUpdate()
  {
    int num = (int) base.ContinuousUpdate();
    if (this.m_waitingToStart)
    {
      for (LinkedListNode<BashelliskBodyController> linkedListNode = this.m_segments.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
      {
        if (linkedListNode.Value.IsShooting)
          return ContinuousBehaviorResult.Continue;
      }
      this.m_segmentDelayTimer = 0.0f;
      this.m_waitingToStart = false;
    }
    while ((double) this.m_segmentDelayTimer <= 0.0 && this.m_nextSegmentNode != null)
    {
      this.m_segmentDelayTimer += this.SegmentDelay;
      this.FireNextSegment();
    }
    return this.m_nextSegmentNode == null ? ContinuousBehaviorResult.Finished : ContinuousBehaviorResult.Continue;
  }

  public override void EndContinuousUpdate()
  {
    base.EndContinuousUpdate();
    this.m_updateEveryFrame = false;
    this.UpdateCooldowns();
  }

  public override bool IsOverridable() => false;

  private void SelectSegments()
  {
    int segmentCount = this.SegmentCount;
    if (this.SegmentCount <= 0)
      segmentCount = Mathf.RoundToInt(this.SegmentPercentage * (float) (this.m_bashellisk.Body.Count - 1));
    int num = Mathf.Max(1, segmentCount);
    this.m_segments.Clear();
    for (LinkedListNode<BashelliskSegment> next = this.m_bashellisk.Body.First.Next; next != null; next = next.Next)
      this.m_segments.AddLast(next.Value as BashelliskBodyController);
    if (this.segmentOrder == BashelliskShootBehavior.SegmentOrder.Sequence)
    {
      while (this.m_segments.Count > num)
        this.m_segments.RemoveLast();
    }
    else if (this.segmentOrder == BashelliskShootBehavior.SegmentOrder.Random)
    {
      for (int min = 0; min < num; ++min)
      {
        LinkedListNode<BashelliskBodyController> byIndexSlow = this.m_segments.GetByIndexSlow(Random.Range(min, this.m_segments.Count));
        this.m_segments.Remove(byIndexSlow, false);
        this.m_segments.AddFirst(byIndexSlow);
      }
      while (this.m_segments.Count > num)
        this.m_segments.RemoveLast();
    }
    else if (this.segmentOrder == BashelliskShootBehavior.SegmentOrder.RandomSequential)
    {
      while (this.m_segments.Count > num)
        this.m_segments.Remove(this.m_segments.GetByIndexSlow(Random.Range(0, this.m_segments.Count)), true);
    }
    for (LinkedListNode<BashelliskBodyController> linkedListNode = this.m_segments.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
    {
      linkedListNode.Value.shootDirection = this.shootDirection;
      linkedListNode.Value.UpdateShootDirection();
    }
    this.m_nextSegmentNode = this.m_segments.First;
  }

  private void FireNextSegment()
  {
    this.m_nextSegmentNode.Value.Fire(this.BulletScript);
    this.m_nextSegmentNode = this.m_nextSegmentNode.Next;
  }

  public enum SegmentOrder
  {
    Sequence = 10, // 0x0000000A
    Random = 20, // 0x00000014
    RandomSequential = 30, // 0x0000001E
  }
}
