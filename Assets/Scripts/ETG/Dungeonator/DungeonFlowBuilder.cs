// Decompiled with JetBrains decompiler
// Type: Dungeonator.DungeonFlowBuilder
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable disable
namespace Dungeonator
{
  public class DungeonFlowBuilder
  {
    public List<RoomHandler> coreAreas;
    public List<RoomHandler> additionalAreas;
    public Dictionary<DungeonFlowNode, int> usedSubchainData = new Dictionary<DungeonFlowNode, int>();
    private Dictionary<RoomHandler, FlowNodeBuildData> roomToUndoDataMap = new Dictionary<RoomHandler, FlowNodeBuildData>();
    private Dictionary<FlowNodeBuildData, RoomHandler> dataToRoomMap = new Dictionary<FlowNodeBuildData, RoomHandler>();
    private List<DungeonChainStructure> m_cachedComposedChains;
    private SemioticLayoutManager m_layoutRef;
    private DungeonFlow m_flow;
    private List<FlowActionLine> m_actionLines;
    private ChainSetupData.ExitPreferenceMetric exitMetric = ChainSetupData.ExitPreferenceMetric.FARTHEST;
    private FlowBuilderDebugger m_debugger;

    public DungeonFlowBuilder(DungeonFlow flow, SemioticLayoutManager layout)
    {
      this.coreAreas = new List<RoomHandler>();
      this.additionalAreas = new List<RoomHandler>();
      this.m_flow = flow;
      this.m_layoutRef = layout;
    }

    public SemioticLayoutManager Layout => this.m_layoutRef;

    public RoomHandler StartRoom => this.coreAreas[0];

    public RoomHandler EndRoom => this.coreAreas[this.coreAreas.Count - 1];

    private int ContainsPrototypeRoom(PrototypeDungeonRoom r)
    {
      int num = 0;
      for (int index = 0; index < this.coreAreas.Count; ++index)
      {
        if ((UnityEngine.Object) this.coreAreas[index].area.prototypeRoom == (UnityEngine.Object) r)
          ++num;
      }
      for (int index = 0; index < this.additionalAreas.Count; ++index)
      {
        if ((UnityEngine.Object) this.additionalAreas[index].area.prototypeRoom == (UnityEngine.Object) r)
          ++num;
      }
      return num;
    }

    private PrototypeRoomExit RoomIsViableAtPosition(
      PrototypeDungeonRoom room,
      IntVector2 attachPoint,
      DungeonData.Direction newRoomExitDirection)
    {
      if (!room.CheckPrerequisites())
        return (PrototypeRoomExit) null;
      List<PrototypeRoomExit> unusedExitsOnSide = room.exitData.GetUnusedExitsOnSide(newRoomExitDirection);
      PrototypeRoomExit prototypeRoomExit = (PrototypeRoomExit) null;
      for (int index = 0; index < unusedExitsOnSide.Count; ++index)
      {
        if (unusedExitsOnSide[index].exitType == PrototypeRoomExit.ExitType.EXIT_ONLY)
          return (PrototypeRoomExit) null;
        if (this.m_layoutRef.CanPlaceRoomAtAttachPointByExit(room, unusedExitsOnSide[index], attachPoint))
        {
          prototypeRoomExit = unusedExitsOnSide[index];
          break;
        }
      }
      return prototypeRoomExit ?? (PrototypeRoomExit) null;
    }

    private Dictionary<WeightedRoom, PrototypeRoomExit> GetViableRoomsFromList(
      List<WeightedRoom> source,
      PrototypeDungeonRoom.RoomCategory category,
      IntVector2 attachPoint,
      DungeonData.Direction newRoomExitDirection)
    {
      Dictionary<WeightedRoom, PrototypeRoomExit> viableRoomsFromList = new Dictionary<WeightedRoom, PrototypeRoomExit>();
      List<int> intList = Enumerable.Range(0, source.Count).ToList<int>().GenerationShuffle<int>();
      for (int index1 = 0; index1 < source.Count; ++index1)
      {
        int index2 = intList[index1];
        WeightedRoom key = source[index2];
        PrototypeDungeonRoom room = key.room;
        if ((!Enum.IsDefined(typeof (PrototypeDungeonRoom.RoomCategory), (object) category) || room.category == category) && key.CheckPrerequisites() && room.CheckPrerequisites() && (!key.limitedCopies || this.ContainsPrototypeRoom(room) < key.maxCopies))
        {
          List<PrototypeRoomExit> unusedExitsOnSide = room.exitData.GetUnusedExitsOnSide(newRoomExitDirection);
          PrototypeRoomExit prototypeRoomExit = (PrototypeRoomExit) null;
          for (int index3 = 0; index3 < unusedExitsOnSide.Count; ++index3)
          {
            if (unusedExitsOnSide[index3].exitType != PrototypeRoomExit.ExitType.EXIT_ONLY && this.m_layoutRef.CanPlaceRoomAtAttachPointByExit(room, unusedExitsOnSide[index3], attachPoint))
            {
              prototypeRoomExit = unusedExitsOnSide[index3];
              break;
            }
          }
          if (prototypeRoomExit != null)
            viableRoomsFromList.Add(key, prototypeRoomExit);
        }
      }
      return viableRoomsFromList;
    }

    private void AddActionLine(FlowActionLine line)
    {
      if (this.m_actionLines == null)
        this.m_actionLines = new List<FlowActionLine>();
      this.m_actionLines.Add(line);
    }

    private bool CheckActionLineCrossings(Vector2 p1, Vector2 p2)
    {
      FlowActionLine other = new FlowActionLine(p1, p2);
      for (int index = 0; index < this.m_actionLines.Count; ++index)
      {
        if (this.m_actionLines[index].Crosses(other))
          return true;
      }
      return false;
    }

    private DungeonFlowNode SelectNodeByWeightingWithoutDuplicates(
      List<DungeonFlowNode> nodes,
      HashSet<DungeonFlowNode> duplicates)
    {
      float num1 = 0.0f;
      for (int index = 0; index < nodes.Count; ++index)
      {
        if (!duplicates.Contains(nodes[index]))
          num1 += nodes[index].percentChance;
      }
      float num2 = BraveRandom.GenerationRandomValue() * num1;
      float num3 = 0.0f;
      for (int index = 0; index < nodes.Count; ++index)
      {
        if (!duplicates.Contains(nodes[index]))
        {
          num3 += nodes[index].percentChance;
          if ((double) num3 > (double) num2)
            return nodes[index];
        }
      }
      return nodes[nodes.Count - 1];
    }

    private int SelectIndexByWeightingWithoutDuplicates(
      List<DungeonFlowBuilder.FlowRoomAttachData> chainRooms,
      HashSet<int> duplicates)
    {
      float num1 = 0.0f;
      for (int index = 0; index < chainRooms.Count; ++index)
      {
        if (!duplicates.Contains(index))
          num1 += chainRooms[index].weightedRoom.weight;
      }
      float num2 = BraveRandom.GenerationRandomValue() * num1;
      float num3 = 0.0f;
      for (int index = 0; index < chainRooms.Count; ++index)
      {
        if (!duplicates.Contains(index))
        {
          num3 += chainRooms[index].weightedRoom.weight;
          if ((double) num3 > (double) num2)
            return index;
        }
      }
      return chainRooms.Count - 1;
    }

    private int SelectIndexByWeighting(List<WeightedRoom> chainRooms)
    {
      float num1 = 0.0f;
      for (int index = 0; index < chainRooms.Count; ++index)
        num1 += chainRooms[index].weight;
      float num2 = BraveRandom.GenerationRandomValue() * num1;
      float num3 = 0.0f;
      for (int index = 0; index < chainRooms.Count; ++index)
      {
        num3 += chainRooms[index].weight;
        if ((double) num3 > (double) num2)
          return index;
      }
      return chainRooms.Count - 1;
    }

    private WeightedRoom GetViableRoomPrototype(
      PrototypeDungeonRoom.RoomCategory category,
      IntVector2 attachPoint,
      DungeonData.Direction extendDirection,
      ref PrototypeRoomExit exitRef,
      List<WeightedRoom> roomTable)
    {
      DungeonData.Direction newRoomExitDirection = (DungeonData.Direction) ((int) (extendDirection + 4) % 8);
      Dictionary<WeightedRoom, PrototypeRoomExit> viableRoomsFromList = this.GetViableRoomsFromList(roomTable, category, attachPoint, newRoomExitDirection);
      if (viableRoomsFromList.Keys.Count <= 0)
        return (WeightedRoom) null;
      WeightedRoom key = viableRoomsFromList.Keys.First<WeightedRoom>();
      exitRef = viableRoomsFromList[key];
      return key;
    }

    public void DebugActionLines()
    {
      for (int index = 0; index < this.m_actionLines.Count; ++index)
        Debug.DrawLine((Vector3) this.m_actionLines[index].point1, (Vector3) this.m_actionLines[index].point2, Color.yellow, 1000f);
    }

    private void RecomposeNodeStructure(
      FlowNodeBuildData currentNodeBuildData,
      DungeonChainStructure extantStructure,
      List<DungeonChainStructure> runningList)
    {
      DungeonFlowNode node = currentNodeBuildData.node;
      extantStructure.containedNodes.Add(currentNodeBuildData);
      if (!string.IsNullOrEmpty(node.loopTargetNodeGuid) || node.childNodeGuids.Count == 0)
      {
        runningList.Add(extantStructure);
        extantStructure = (DungeonChainStructure) null;
      }
      if (currentNodeBuildData.childBuildData == null)
        currentNodeBuildData.childBuildData = this.m_flow.GetNodeChildrenToBuild(node, this);
      for (int index = 0; index < currentNodeBuildData.childBuildData.Count; ++index)
      {
        FlowNodeBuildData currentNodeBuildData1 = currentNodeBuildData.childBuildData[index];
        DungeonChainStructure extantStructure1 = index != 0 ? (DungeonChainStructure) null : extantStructure;
        if (extantStructure1 == null)
        {
          extantStructure1 = new DungeonChainStructure();
          extantStructure1.parentNode = currentNodeBuildData;
        }
        this.RecomposeNodeStructure(currentNodeBuildData1, extantStructure1, runningList);
      }
    }

    private void DecomposeLoopSubchains(List<DungeonChainStructure> subchains)
    {
      for (int index1 = 0; index1 < subchains.Count; ++index1)
      {
        DungeonChainStructure subchain = subchains[index1];
        if (subchain.optionalRequiredNode != null && subchain.containedNodes.Count > 1)
        {
          int count = subchain.containedNodes.Count;
          int num = BraveRandom.GenerationRandomRange(1, count - 1);
          List<FlowNodeBuildData> flowNodeBuildDataList = new List<FlowNodeBuildData>();
          for (int index2 = count - 1; index2 >= num; --index2)
          {
            flowNodeBuildDataList.Add(subchain.containedNodes[index2]);
            subchain.containedNodes.RemoveAt(index2);
          }
          DungeonChainStructure dungeonChainStructure = new DungeonChainStructure();
          subchain.optionalRequiredNode.childBuildData.Add(flowNodeBuildDataList[0]);
          dungeonChainStructure.parentNode = subchain.optionalRequiredNode;
          dungeonChainStructure.containedNodes = flowNodeBuildDataList;
          dungeonChainStructure.optionalRequiredNode = subchain.containedNodes[subchain.containedNodes.Count - 1];
          subchain.optionalRequiredNode = dungeonChainStructure.containedNodes[dungeonChainStructure.containedNodes.Count - 1];
          if (dungeonChainStructure.containedNodes.Count >= subchain.containedNodes.Count)
            subchains.Insert(index1 + 1, dungeonChainStructure);
          else
            subchains.Insert(index1, dungeonChainStructure);
          ++index1;
        }
      }
    }

    protected List<DungeonChainStructure> ComposeBuildOrderSimple()
    {
      List<DungeonChainStructure> dungeonChainStructureList = new List<DungeonChainStructure>();
      List<FlowNodeBuildData> flowNodeBuildDataList = new List<FlowNodeBuildData>();
      Stack<FlowNodeBuildData> flowNodeBuildDataStack = new Stack<FlowNodeBuildData>();
      flowNodeBuildDataStack.Push(new FlowNodeBuildData(this.m_flow.FirstNode));
      while (flowNodeBuildDataStack.Count > 0)
      {
        FlowNodeBuildData flowNodeBuildData = flowNodeBuildDataStack.Pop();
        flowNodeBuildDataList.Add(flowNodeBuildData);
        if (flowNodeBuildData.childBuildData == null)
          flowNodeBuildData.childBuildData = this.m_flow.GetNodeChildrenToBuild(flowNodeBuildData.node, this);
        for (int index = 0; index < flowNodeBuildData.childBuildData.Count; ++index)
        {
          if (!flowNodeBuildDataStack.Contains(flowNodeBuildData.childBuildData[index]))
            flowNodeBuildDataStack.Push(flowNodeBuildData.childBuildData[index]);
        }
      }
      dungeonChainStructureList.Add(new DungeonChainStructure()
      {
        containedNodes = flowNodeBuildDataList
      });
      return dungeonChainStructureList;
    }

    public List<DungeonChainStructure> ComposeBuildOrder()
    {
      if (this.m_cachedComposedChains != null)
        return this.m_cachedComposedChains;
      this.m_cachedComposedChains = (List<DungeonChainStructure>) null;
      return this.ComposeBuildOrderSimple();
    }

    public bool Build(RoomHandler startRoom)
    {
      this.m_debugger = new FlowBuilderDebugger();
      this.coreAreas.Add(startRoom);
      List<DungeonChainStructure> dungeonChainStructureList = this.ComposeBuildOrder();
      bool flag = true;
      for (int index = 0; index < dungeonChainStructureList.Count; ++index)
      {
        DungeonChainStructure dungeonChainStructure = dungeonChainStructureList[index];
        RoomHandler roomToExtendFrom = startRoom;
        if (index > 0)
          roomToExtendFrom = dungeonChainStructure.parentNode.room;
        flag = this.BuildNode(dungeonChainStructure.containedNodes[0], roomToExtendFrom, (DungeonChainStructure) null, true);
        if (!flag)
          break;
      }
      if (!flag)
        this.coreAreas.RemoveAt(0);
      this.m_debugger.FinalizeLog();
      return flag;
    }

    protected void ShuffleExitsByMetric(
      ref List<PrototypeRoomExit> unusedExits,
      PrototypeRoomExit previouslyUsedExit)
    {
      switch (this.exitMetric)
      {
        case ChainSetupData.ExitPreferenceMetric.RANDOM:
          unusedExits = unusedExits.GenerationShuffle<PrototypeRoomExit>();
          break;
        case ChainSetupData.ExitPreferenceMetric.HORIZONTAL:
          break;
        case ChainSetupData.ExitPreferenceMetric.VERTICAL:
          Debug.LogError((object) "Vertical not yet implemented");
          break;
        case ChainSetupData.ExitPreferenceMetric.FARTHEST:
          if (previouslyUsedExit == null)
          {
            unusedExits = unusedExits.GenerationShuffle<PrototypeRoomExit>();
            break;
          }
          unusedExits = unusedExits.OrderBy<PrototypeRoomExit, int>((Func<PrototypeRoomExit, int>) (a => IntVector2.ManhattanDistance(a.GetExitOrigin(a.exitLength), previouslyUsedExit.GetExitOrigin(previouslyUsedExit.exitLength)))).ToList<PrototypeRoomExit>();
          break;
        case ChainSetupData.ExitPreferenceMetric.NEAREST:
          if (previouslyUsedExit == null)
          {
            unusedExits = unusedExits.GenerationShuffle<PrototypeRoomExit>();
            break;
          }
          unusedExits = unusedExits.OrderByDescending<PrototypeRoomExit, float>((Func<PrototypeRoomExit, float>) (a => Vector2.Distance(a.GetExitOrigin(a.exitLength).ToVector2(), previouslyUsedExit.GetExitOrigin(previouslyUsedExit.exitLength).ToVector2()))).ToList<PrototypeRoomExit>();
          break;
        default:
          unusedExits = unusedExits.GenerationShuffle<PrototypeRoomExit>();
          break;
      }
    }

    private List<DungeonFlowBuilder.FlowRoomAttachData> GetViableRoomsForExits(
      CellArea areaToExtendFrom,
      PrototypeDungeonRoom.RoomCategory nextRoomCategory,
      List<PrototypeRoomExit> unusedExits,
      List<WeightedRoom> roomTable)
    {
      List<DungeonFlowBuilder.FlowRoomAttachData> viableRoomsForExits = new List<DungeonFlowBuilder.FlowRoomAttachData>();
      for (int index = 0; index < unusedExits.Count; ++index)
      {
        PrototypeRoomExit unusedExit = unusedExits[index];
        IntVector2 attachPoint = areaToExtendFrom.basePosition + unusedExit.GetExitOrigin(unusedExit.exitLength) - IntVector2.One;
        DungeonData.Direction newRoomExitDirection = (DungeonData.Direction) ((int) (unusedExit.exitDirection + 4) % 8);
        Dictionary<WeightedRoom, PrototypeRoomExit> viableRoomsFromList = this.GetViableRoomsFromList(roomTable, nextRoomCategory, attachPoint, newRoomExitDirection);
        foreach (WeightedRoom key in viableRoomsFromList.Keys)
          viableRoomsForExits.Add(new DungeonFlowBuilder.FlowRoomAttachData(key, viableRoomsFromList[key], unusedExit));
      }
      return viableRoomsForExits;
    }

    private void RecursivelyUnstampChildren(FlowNodeBuildData buildData)
    {
      for (int index = 0; index < buildData.childBuildData.Count; ++index)
        this.RecursivelyUnstampChildren(buildData.childBuildData[index]);
      if (buildData.room != null)
      {
        this.m_layoutRef.StampCellAreaToLayout(buildData.room, true);
        if (buildData.room.flowActionLine != null)
        {
          this.m_actionLines.Remove(buildData.room.flowActionLine);
          buildData.room.flowActionLine = (FlowActionLine) null;
        }
        if (this.coreAreas.Contains(buildData.room))
          this.coreAreas.Remove(buildData.room);
        this.roomToUndoDataMap.Remove(buildData.room);
        buildData.UnmarkExits();
      }
      buildData.unbuilt = true;
      this.dataToRoomMap.Remove(buildData);
      buildData.room = (RoomHandler) null;
    }

    private bool HandleNodeChildren(
      FlowNodeBuildData originalNodeBuildData,
      DungeonChainStructure chain)
    {
      originalNodeBuildData.MarkExits();
      originalNodeBuildData.unbuilt = false;
      FlowActionLine line = new FlowActionLine(originalNodeBuildData.room.GetCenterCell().ToCenterVector2(), originalNodeBuildData.sourceRoom.GetCenterCell().ToCenterVector2());
      this.AddActionLine(line);
      originalNodeBuildData.room.flowActionLine = line;
      bool flag = true;
      if (chain != null)
      {
        int index = chain.containedNodes.IndexOf(originalNodeBuildData) + 1;
        if (index >= chain.containedNodes.Count)
        {
          if (chain.optionalRequiredNode != null && chain.optionalRequiredNode.room != null && !GameManager.Instance.Dungeon.debugSettings.DISABLE_LOOPS)
            flag = this.BuildLoopNode(originalNodeBuildData, chain.optionalRequiredNode, chain);
        }
        else
        {
          flag = this.BuildNode(chain.containedNodes[index], originalNodeBuildData.room, chain);
          if (chain.containedNodes[index].node.priority == DungeonFlowNode.NodePriority.OPTIONAL)
            flag = true;
        }
      }
      else
      {
        for (int index = 0; index < originalNodeBuildData.childBuildData.Count; ++index)
        {
          flag = this.BuildNode(originalNodeBuildData.childBuildData[index], originalNodeBuildData.room, (DungeonChainStructure) null);
          if (originalNodeBuildData.childBuildData[index].node.priority == DungeonFlowNode.NodePriority.OPTIONAL)
            flag = true;
          if (!flag)
            break;
        }
      }
      return flag;
    }

    private IntRect GetSpanningRectangle(IntVector2 p1, IntVector2 p2, out bool valid)
    {
      int left = Math.Min(p1.x, p2.x);
      int bottom = Math.Min(p1.y, p2.y);
      int num1 = Math.Max(p1.x, p2.x);
      int num2 = Math.Max(p1.y, p2.y);
      IntRect rectangle = new IntRect(left, bottom, num1 - left, num2 - bottom);
      valid = this.m_layoutRef.CanPlaceRectangle(rectangle);
      return rectangle;
    }

    private bool BuildLoopNode(
      FlowNodeBuildData chainEndData,
      FlowNodeBuildData loopTargetData,
      DungeonChainStructure chain)
    {
      RoomHandler room1 = chainEndData.room;
      RoomHandler room2 = loopTargetData.room;
      CellArea area1 = room1.area;
      CellArea area2 = room2.area;
      if ((UnityEngine.Object) area2.prototypeRoom != (UnityEngine.Object) null && (UnityEngine.Object) area1.prototypeRoom != (UnityEngine.Object) null)
      {
        List<PrototypeRoomExit> exitsFromInstance1 = area1.prototypeRoom.exitData.GetUnusedExitsFromInstance(area1);
        List<PrototypeRoomExit> exitsFromInstance2 = area2.prototypeRoom.exitData.GetUnusedExitsFromInstance(area2);
        List<DungeonFlowBuilder.LoopPathData> loopPathDataList = new List<DungeonFlowBuilder.LoopPathData>();
        for (int index1 = 0; index1 < exitsFromInstance1.Count; ++index1)
        {
          for (int index2 = 0; index2 < exitsFromInstance2.Count; ++index2)
          {
            PrototypeRoomExit initialExit = exitsFromInstance1[index1];
            PrototypeRoomExit finalExit = exitsFromInstance2[index2];
            List<IntVector2> path = this.m_layoutRef.TraceHallway(area1.basePosition + initialExit.GetExitOrigin(initialExit.exitLength) - IntVector2.One + DungeonData.GetIntVector2FromDirection(initialExit.exitDirection), area2.basePosition + finalExit.GetExitOrigin(finalExit.exitLength) - IntVector2.One + DungeonData.GetIntVector2FromDirection(finalExit.exitDirection), initialExit.exitDirection, finalExit.exitDirection);
            if (path != null)
              loopPathDataList.Add(new DungeonFlowBuilder.LoopPathData(path, initialExit, finalExit));
          }
        }
        if (loopPathDataList.Count > 0)
        {
          DungeonFlowBuilder.LoopPathData loopPathData = loopPathDataList[0];
          for (int index = 0; index < loopPathDataList.Count; ++index)
          {
            if (loopPathDataList[index].path.Count < loopPathData.path.Count)
              loopPathData = loopPathDataList[index];
          }
          IntVector2 p = new IntVector2(int.MaxValue, int.MaxValue);
          IntVector2 d = new IntVector2(int.MinValue, int.MinValue);
          for (int index = 0; index < loopPathData.path.Count; ++index)
          {
            p.x = Math.Min(p.x, loopPathData.path[index].x);
            p.y = Math.Min(p.y, loopPathData.path[index].y);
            d.x = Math.Max(d.x, loopPathData.path[index].x);
            d.y = Math.Max(d.y, loopPathData.path[index].y);
          }
          for (int index = 0; index < loopPathData.path.Count; ++index)
            loopPathData.path[index] -= p;
          RoomHandler newRoom = new RoomHandler(new CellArea(p, d)
          {
            proceduralCells = loopPathData.path
          });
          newRoom.distanceFromEntrance = room2.distanceFromEntrance + 1;
          newRoom.CalculateOpulence();
          this.coreAreas.Add(newRoom);
          this.m_layoutRef.StampCellAreaToLayout(newRoom);
          room1.area.instanceUsedExits.Add(loopPathData.initialExit);
          room2.area.instanceUsedExits.Add(loopPathData.finalExit);
          newRoom.parentRoom = room1;
          newRoom.childRooms.Add(room2);
          room1.childRooms.Add(newRoom);
          room1.connectedRooms.Add(newRoom);
          room1.connectedRoomsByExit.Add(loopPathData.initialExit, newRoom);
          room2.childRooms.Add(newRoom);
          room2.connectedRooms.Add(newRoom);
          room2.connectedRoomsByExit.Add(loopPathData.finalExit, newRoom);
          return true;
        }
        if (exitsFromInstance2.Count == 0 || exitsFromInstance1.Count == 0)
          BraveUtility.Log("No free exits to generate loop. No loop generated.", Color.cyan, BraveUtility.LogVerbosity.CHATTY);
        else
          BraveUtility.Log("All loops failed. No loop generated.", Color.cyan, BraveUtility.LogVerbosity.CHATTY);
      }
      else
        Debug.LogError((object) "Procedural rooms not implemented in loop generation yet!");
      return false;
    }

    private bool BuildNode(
      FlowNodeBuildData nodeBuildData,
      RoomHandler roomToExtendFrom,
      DungeonChainStructure chain,
      bool initial = false)
    {
      DungeonFlowNode node = nodeBuildData.node;
      if (node == (DungeonFlowNode) null)
        return true;
      if (this.dataToRoomMap.ContainsKey(nodeBuildData))
      {
        Debug.LogError((object) "FAILURE");
        this.RecursivelyUnstampChildren(nodeBuildData);
      }
      if (node.nodeType != DungeonFlowNode.ControlNodeType.ROOM)
      {
        switch (node.nodeType)
        {
          case DungeonFlowNode.ControlNodeType.SUBCHAIN:
            Debug.Break();
            break;
          case DungeonFlowNode.ControlNodeType.SELECTOR:
            Debug.Break();
            break;
        }
      }
      else
      {
        PrototypeDungeonRoom.RoomCategory nextRoomCategory = !nodeBuildData.usesOverrideCategory ? node.roomCategory : nodeBuildData.overrideCategory;
        CellArea area = roomToExtendFrom.area;
        if ((UnityEngine.Object) area.prototypeRoom != (UnityEngine.Object) null)
        {
          List<PrototypeRoomExit> exitsFromInstance = area.prototypeRoom.exitData.GetUnusedExitsFromInstance(area);
          PrototypeRoomExit previouslyUsedExit = (PrototypeRoomExit) null;
          if (area.instanceUsedExits.Count != 0)
            previouslyUsedExit = area.instanceUsedExits[BraveRandom.GenerationRandomRange(0, area.instanceUsedExits.Count)];
          if (chain != null && chain.optionalRequiredNode != null && chain.optionalRequiredNode.room != null)
          {
            for (int index = 0; index < exitsFromInstance.Count; ++index)
            {
              Vector2 centerVector2_1 = (area.basePosition + exitsFromInstance[index].GetExitOrigin(exitsFromInstance[index].exitLength) - IntVector2.One).ToCenterVector2();
              Vector2 centerVector2_2 = chain.optionalRequiredNode.room.GetCenterCell().ToCenterVector2();
              Vector2 p2 = centerVector2_2 + (centerVector2_1 - centerVector2_2).normalized;
              if (this.CheckActionLineCrossings(centerVector2_1, p2))
              {
                exitsFromInstance.RemoveAt(index);
                --index;
              }
            }
          }
          this.ShuffleExitsByMetric(ref exitsFromInstance, previouslyUsedExit);
          List<WeightedRoom> roomTable;
          if (node.UsesGlobalBossData)
            roomTable = GameManager.Instance.BossManager.SelectBossTable().GetCompiledList();
          else if ((UnityEngine.Object) node.overrideExactRoom != (UnityEngine.Object) null)
          {
            WeightedRoom weightedRoom = new WeightedRoom();
            weightedRoom.room = node.overrideExactRoom;
            weightedRoom.weight = 1f;
            roomTable = new List<WeightedRoom>();
            roomTable.Add(weightedRoom);
            nextRoomCategory = node.overrideExactRoom.category;
          }
          else
          {
            GenericRoomTable genericRoomTable = this.m_flow.fallbackRoomTable;
            if ((UnityEngine.Object) node.overrideRoomTable != (UnityEngine.Object) null)
              genericRoomTable = node.overrideRoomTable;
            roomTable = genericRoomTable.GetCompiledList();
          }
          List<DungeonFlowBuilder.FlowRoomAttachData> viableRoomsForExits = this.GetViableRoomsForExits(area, nextRoomCategory, exitsFromInstance, roomTable);
          int count = viableRoomsForExits.Count;
          for (int index = 0; index < count; ++index)
          {
            DungeonFlowBuilder.FlowRoomAttachData flowRoomAttachData = viableRoomsForExits[index];
            if (this.ContainsPrototypeRoom(flowRoomAttachData.weightedRoom.room) > 0)
            {
              viableRoomsForExits.RemoveAt(index);
              viableRoomsForExits.Add(flowRoomAttachData);
              --index;
              --count;
            }
          }
          if (viableRoomsForExits == null || viableRoomsForExits.Count == 0)
            return false;
          if (nodeBuildData.childBuildData == null && this.m_flow.IsPartOfSubchain(nodeBuildData.node))
            nodeBuildData.childBuildData = this.m_flow.GetNodeChildrenToBuild(nodeBuildData.node, this);
          List<FlowNodeBuildData> childBuildData = nodeBuildData.childBuildData;
          int num = 0;
          for (int index = 0; index < childBuildData.Count; ++index)
            num += childBuildData[index].node.priority != DungeonFlowNode.NodePriority.MANDATORY ? 0 : 1;
          HashSet<int> duplicates = new HashSet<int>();
          for (int index1 = 0; index1 < viableRoomsForExits.Count; ++index1)
          {
            int index2 = this.SelectIndexByWeightingWithoutDuplicates(viableRoomsForExits, duplicates);
            duplicates.Add(index2);
            PrototypeDungeonRoom room = viableRoomsForExits[index2].weightedRoom.room;
            if (room.exitData.exits.Count >= num + 1)
            {
              PrototypeRoomExit exitOfNewRoom = viableRoomsForExits[index2].exitOfNewRoom;
              PrototypeRoomExit exitToUse = viableRoomsForExits[index2].exitToUse;
              IntVector2 b = area.basePosition + exitToUse.GetExitOrigin(exitToUse.exitLength) - IntVector2.One;
              IntVector2 p = b - (exitOfNewRoom.GetExitOrigin(exitOfNewRoom.exitLength) - IntVector2.One);
              if (chain != null && chain.optionalRequiredNode != null && chain.optionalRequiredNode.room != null)
              {
                if (chain.previousLoopDistanceMetric == int.MaxValue)
                  chain.previousLoopDistanceMetric = IntVector2.ManhattanDistance(chain.optionalRequiredNode.room.GetCenterCell(), b);
                int val1 = int.MaxValue;
                for (int index3 = 0; index3 < room.exitData.exits.Count; ++index3)
                {
                  if (room.exitData.exits[index3] != exitOfNewRoom)
                    val1 = Math.Min(val1, IntVector2.ManhattanDistance(chain.optionalRequiredNode.room.GetCenterCell(), p + room.exitData.exits[index3].GetExitOrigin(room.exitData.exits[index3].exitLength) - IntVector2.One));
                }
                if (val1 <= chain.previousLoopDistanceMetric)
                  chain.previousLoopDistanceMetric = val1;
                else
                  continue;
              }
              CellArea a = new CellArea(p, new IntVector2(room.Width, room.Height));
              a.prototypeRoom = room;
              a.instanceUsedExits = new List<PrototypeRoomExit>();
              if (nodeBuildData.usesOverrideCategory)
                a.PrototypeRoomCategory = nodeBuildData.overrideCategory;
              RoomHandler roomHandler = new RoomHandler(a);
              roomHandler.distanceFromEntrance = roomToExtendFrom.distanceFromEntrance + 1;
              roomHandler.CalculateOpulence();
              roomHandler.CanReceiveCaps = node.receivesCaps;
              this.coreAreas.Add(roomHandler);
              this.m_layoutRef.StampCellAreaToLayout(roomHandler);
              nodeBuildData.room = roomHandler;
              nodeBuildData.roomEntrance = exitOfNewRoom;
              nodeBuildData.sourceExit = exitToUse;
              nodeBuildData.sourceRoom = roomToExtendFrom;
              this.roomToUndoDataMap.Add(roomHandler, nodeBuildData);
              this.dataToRoomMap.Add(nodeBuildData, roomHandler);
              this.m_debugger.Log(roomToExtendFrom, roomHandler);
              this.m_debugger.LogMonoHeapStatus();
              if (this.HandleNodeChildren(nodeBuildData, chain))
                return true;
              this.m_debugger.Log(roomToExtendFrom.area.prototypeRoom.name + " is falling back...");
              this.RecursivelyUnstampChildren(nodeBuildData);
            }
          }
        }
        else
        {
          Debug.LogError((object) "Procedural room handling not yet implemented!");
          return false;
        }
      }
      this.m_debugger.Log(roomToExtendFrom.area.prototypeRoom.name + " completely failed.");
      return false;
    }

    public void AppendCapChains()
    {
      List<RoomHandler> roomsWithViableExits = new List<RoomHandler>();
      List<PrototypeRoomExit> viableExitsToCap = new List<PrototypeRoomExit>();
      for (int index1 = 0; index1 < this.coreAreas.Count; ++index1)
      {
        PrototypeDungeonRoom prototypeRoom = this.coreAreas[index1].area.prototypeRoom;
        if (!((UnityEngine.Object) prototypeRoom == (UnityEngine.Object) null) && this.coreAreas[index1].CanReceiveCaps)
        {
          for (int index2 = 0; index2 < prototypeRoom.exitData.exits.Count; ++index2)
          {
            if (!this.coreAreas[index1].area.instanceUsedExits.Contains(prototypeRoom.exitData.exits[index2]) && prototypeRoom.exitData.exits[index2].exitType != PrototypeRoomExit.ExitType.ENTRANCE_ONLY)
            {
              roomsWithViableExits.Add(this.coreAreas[index1]);
              viableExitsToCap.Add(prototypeRoom.exitData.exits[index2]);
            }
          }
        }
      }
      List<int> source = Enumerable.Range(0, roomsWithViableExits.Count).ToList<int>().GenerationShuffle<int>();
      roomsWithViableExits = source.Select<int, RoomHandler>((Func<int, RoomHandler>) (index => roomsWithViableExits[index])).ToList<RoomHandler>();
      viableExitsToCap = source.Select<int, PrototypeRoomExit>((Func<int, PrototypeRoomExit>) (index => viableExitsToCap[index])).ToList<PrototypeRoomExit>();
      for (int index = 0; index < viableExitsToCap.Count; ++index)
      {
        List<DungeonFlowNode> capChainRootNodes = this.m_flow.GetCapChainRootNodes(this);
        if (capChainRootNodes == null || capChainRootNodes.Count == 0)
          break;
        HashSet<DungeonFlowNode> duplicates = new HashSet<DungeonFlowNode>();
        DungeonFlowNode dungeonFlowNode = this.SelectNodeByWeightingWithoutDuplicates(capChainRootNodes, duplicates);
        duplicates.Add(dungeonFlowNode);
        if (this.BuildNode(new FlowNodeBuildData(dungeonFlowNode)
        {
          childBuildData = this.m_flow.GetNodeChildrenToBuild(dungeonFlowNode, this)
        }, roomsWithViableExits[index], (DungeonChainStructure) null))
        {
          if (this.usedSubchainData.ContainsKey(dungeonFlowNode))
            ++this.usedSubchainData[dungeonFlowNode];
          else
            this.usedSubchainData.Add(dungeonFlowNode, 1);
        }
      }
    }

    public bool AttemptAppendExtraRoom(ExtraIncludedRoomData extraRoomData)
    {
      List<RoomHandler> roomsWithViableExits = new List<RoomHandler>();
      List<PrototypeRoomExit> viableExitsToCap = new List<PrototypeRoomExit>();
      for (int index1 = 0; index1 < this.coreAreas.Count; ++index1)
      {
        PrototypeDungeonRoom prototypeRoom = this.coreAreas[index1].area.prototypeRoom;
        if (!((UnityEngine.Object) prototypeRoom == (UnityEngine.Object) null))
        {
          for (int index2 = 0; index2 < prototypeRoom.exitData.exits.Count; ++index2)
          {
            if (!this.coreAreas[index1].area.instanceUsedExits.Contains(prototypeRoom.exitData.exits[index2]) && prototypeRoom.exitData.exits[index2].exitType != PrototypeRoomExit.ExitType.ENTRANCE_ONLY)
            {
              roomsWithViableExits.Add(this.coreAreas[index1]);
              viableExitsToCap.Add(prototypeRoom.exitData.exits[index2]);
            }
          }
        }
      }
      List<int> source = Enumerable.Range(0, roomsWithViableExits.Count).ToList<int>().GenerationShuffle<int>();
      roomsWithViableExits = source.Select<int, RoomHandler>((Func<int, RoomHandler>) (index => roomsWithViableExits[index])).ToList<RoomHandler>();
      viableExitsToCap = source.Select<int, PrototypeRoomExit>((Func<int, PrototypeRoomExit>) (index => viableExitsToCap[index])).ToList<PrototypeRoomExit>();
      for (int index = 0; index < viableExitsToCap.Count; ++index)
      {
        PrototypeRoomExit key1 = viableExitsToCap[index];
        IntVector2 attachPoint = roomsWithViableExits[index].area.basePosition + key1.GetExitOrigin(key1.exitLength) - IntVector2.One;
        DungeonData.Direction newRoomExitDirection = (DungeonData.Direction) ((int) (key1.exitDirection + 4) % 8);
        PrototypeRoomExit key2 = this.RoomIsViableAtPosition(extraRoomData.room, attachPoint, newRoomExitDirection);
        if (key2 != null)
        {
          CellArea a = new CellArea(attachPoint - (key2.GetExitOrigin(key2.exitLength) - IntVector2.One), new IntVector2(extraRoomData.room.Width, extraRoomData.room.Height));
          a.prototypeRoom = extraRoomData.room;
          a.instanceUsedExits = new List<PrototypeRoomExit>();
          RoomHandler newRoom = new RoomHandler(a);
          newRoom.distanceFromEntrance = roomsWithViableExits[index].distanceFromEntrance + 1;
          newRoom.CalculateOpulence();
          this.additionalAreas.Add(newRoom);
          this.m_layoutRef.StampCellAreaToLayout(newRoom);
          a.instanceUsedExits.Add(key2);
          roomsWithViableExits[index].area.instanceUsedExits.Add(key1);
          newRoom.parentRoom = roomsWithViableExits[index];
          newRoom.connectedRooms.Add(roomsWithViableExits[index]);
          newRoom.connectedRoomsByExit.Add(key2, roomsWithViableExits[index]);
          roomsWithViableExits[index].childRooms.Add(newRoom);
          roomsWithViableExits[index].connectedRooms.Add(newRoom);
          roomsWithViableExits[index].connectedRoomsByExit.Add(key1, newRoom);
          return true;
        }
      }
      return false;
    }

    private struct FlowRoomAttachData(
      WeightedRoom w,
      PrototypeRoomExit exitOfNew,
      PrototypeRoomExit exitOfOld)
    {
      public WeightedRoom weightedRoom = w;
      public PrototypeRoomExit exitOfNewRoom = exitOfNew;
      public PrototypeRoomExit exitToUse = exitOfOld;
    }

    internal class LoopPathData
    {
      public List<IntVector2> path;
      public PrototypeRoomExit initialExit;
      public PrototypeRoomExit finalExit;

      public LoopPathData(
        List<IntVector2> path,
        PrototypeRoomExit initialExit,
        PrototypeRoomExit finalExit)
      {
        this.path = path;
        this.initialExit = initialExit;
        this.finalExit = finalExit;
      }
    }
  }
}
