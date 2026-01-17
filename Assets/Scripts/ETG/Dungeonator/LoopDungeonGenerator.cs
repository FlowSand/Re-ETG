// Decompiled with JetBrains decompiler
// Type: Dungeonator.LoopDungeonGenerator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable
namespace Dungeonator;

public class LoopDungeonGenerator : IDungeonGenerator
{
  public const bool c_ROOM_MIRRORING = false;
  public static int NUM_FAILS_COMPOSITE_REGENERATION;
  public static int NUM_FAILS_COMPOSITE_ATTACHMENT;
  public DungeonData DeferredGeneratedData;
  public bool RAPID_DEBUG_ITERATION_MODE;
  public int RAPID_DEBUG_ITERATION_INDEX;
  private SemioticDungeonGenSettings m_patternSettings;
  private DungeonFlow m_assignedFlow;
  private int m_lastAssignedSeed;
  private bool m_forceAssignedFlow;
  private Stopwatch m_timer;

  public LoopDungeonGenerator(Dungeon d, int dungeonSeed)
  {
    this.m_patternSettings = d.PatternSettings;
    this.m_assignedFlow = this.m_patternSettings.GetRandomFlow();
    this.m_lastAssignedSeed = dungeonSeed;
    Random.InitState(dungeonSeed);
    BraveRandom.InitializeWithSeed(dungeonSeed);
    GameManager.SEED_LABEL = dungeonSeed.ToString();
  }

  public void AssignFlow(DungeonFlow flow)
  {
    this.m_forceAssignedFlow = true;
    this.m_assignedFlow = flow;
  }

  protected void GetNewFlowForIteration()
  {
    if (this.m_forceAssignedFlow)
      return;
    this.m_assignedFlow = this.m_patternSettings.GetRandomFlow();
  }

  protected void RecalculateRoomDistances(RoomHandler entrance)
  {
    Queue<Tuple<RoomHandler, int>> tupleQueue = new Queue<Tuple<RoomHandler, int>>();
    List<RoomHandler> roomHandlerList = new List<RoomHandler>();
    tupleQueue.Enqueue(new Tuple<RoomHandler, int>(entrance, 0));
    while (tupleQueue.Count > 0)
    {
      Tuple<RoomHandler, int> tuple = tupleQueue.Dequeue();
      tuple.First.distanceFromEntrance = tuple.Second;
      roomHandlerList.Add(tuple.First);
      for (int index = 0; index < tuple.First.connectedRooms.Count; ++index)
      {
        RoomHandler connectedRoom = tuple.First.connectedRooms[index];
        if (!roomHandlerList.Contains(connectedRoom))
          tupleQueue.Enqueue(new Tuple<RoomHandler, int>(connectedRoom, tuple.Second + 1));
      }
    }
  }

  [DebuggerHidden]
  public IEnumerable GenerateDungeonLayoutDeferred()
  {
    // ISSUE: object of a compiler-generated type is created
    // ISSUE: variable of a compiler-generated type
    LoopDungeonGenerator__GenerateDungeonLayoutDeferredc__Iterator0 dungeonLayoutDeferred = new LoopDungeonGenerator__GenerateDungeonLayoutDeferredc__Iterator0()
    {
      _this = this
    };
    // ISSUE: reference to a compiler-generated field
    dungeonLayoutDeferred._PC = -2;
    return (IEnumerable) dungeonLayoutDeferred;
  }

  [DebuggerHidden]
  public IEnumerable<ProcessStatus> GenerateDungeonLayoutDeferred_Internal()
  {
    // ISSUE: object of a compiler-generated type is created
    // ISSUE: variable of a compiler-generated type
    LoopDungeonGenerator__GenerateDungeonLayoutDeferred_Internalc__Iterator1 deferredInternal = new LoopDungeonGenerator__GenerateDungeonLayoutDeferred_Internalc__Iterator1()
    {
      _this = this
    };
    // ISSUE: reference to a compiler-generated field
    deferredInternal._PC = -2;
    return (IEnumerable<ProcessStatus>) deferredInternal;
  }

  private int CreateCellDataIntelligently(
    CellData[][] cells,
    SemioticLayoutManager layout,
    IntVector2 span,
    IntVector2 offsetRequired)
  {
    int dataIntelligently = 0;
    float[] numArray = new float[(span.y + 20) * (span.x + 20)];
    for (int index1 = 0; index1 < span.x + 20; ++index1)
    {
      for (int index2 = 0; index2 < span.y + 20; ++index2)
        numArray[index2 * (span.x + 20) + index1] = 1000000f;
    }
    Queue<IntVector2> intVector2Queue = new Queue<IntVector2>();
    HashSet<IntVector2> intVector2Set = new HashSet<IntVector2>();
    foreach (IntVector2 occupiedCell in layout.OccupiedCells)
    {
      IntVector2 intVector2 = occupiedCell + offsetRequired;
      numArray[intVector2.y * (span.x + 20) + intVector2.x] = 0.0f;
      intVector2Queue.Enqueue(intVector2);
      intVector2Set.Add(intVector2);
    }
    IntVector2[] cardinalsAndOrdinals = IntVector2.CardinalsAndOrdinals;
    while (intVector2Queue.Count > 0)
    {
      IntVector2 intVector2_1 = intVector2Queue.Dequeue();
      intVector2Set.Remove(intVector2_1);
      float num1 = numArray[intVector2_1.y * (span.x + 20) + intVector2_1.x];
      for (int index = 0; index < cardinalsAndOrdinals.Length; ++index)
      {
        IntVector2 intVector2_2 = intVector2_1 + cardinalsAndOrdinals[index];
        if (intVector2_2.x >= 0 && intVector2_2.y >= 0 && intVector2_2.x < span.x + 20 && intVector2_2.y < span.y + 20)
        {
          float num2 = numArray[intVector2_2.y * (span.x + 20) + intVector2_2.x];
          float num3 = index % 2 != 1 ? num1 + 1f : num1 + 1.414f;
          if ((double) num2 > (double) num3)
          {
            numArray[intVector2_2.y * (span.x + 20) + intVector2_2.x] = num3;
            if (!intVector2Set.Contains(intVector2_2))
            {
              intVector2Queue.Enqueue(intVector2_2);
              intVector2Set.Add(intVector2_2);
            }
          }
        }
      }
    }
    for (int index = 0; index < cells.Length; ++index)
      cells[index] = new CellData[span.y + 20];
    for (int x = 0; x < span.x + 20; ++x)
    {
      for (int y = 0; y < span.y + 20; ++y)
      {
        if ((double) numArray[y * (span.x + 20) + x] <= 7.0)
        {
          cells[x][y] = new CellData(x, y);
          ++dataIntelligently;
        }
      }
    }
    return dataIntelligently;
  }

  public void GenerateDungeonLayoutThreaded()
  {
    IEnumerator enumerator = this.GenerateDungeonLayoutDeferred().GetEnumerator();
    do
      ;
    while (enumerator.MoveNext());
  }

  public DungeonData GenerateDungeonLayout()
  {
    IEnumerator enumerator = this.GenerateDungeonLayoutDeferred().GetEnumerator();
    do
      ;
    while (enumerator.MoveNext());
    return this.DeferredGeneratedData;
  }
}
