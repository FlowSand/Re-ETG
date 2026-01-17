// Decompiled with JetBrains decompiler
// Type: Dungeonator.LoopBuilderComposite
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

#nullable disable
namespace Dungeonator
{
  public class LoopBuilderComposite : ArbitraryFlowBuildData
  {
    protected const int MIN_PATH_THRESHOLD = 4;
    protected const int MIN_PATH_PHANTOM_THRESHOLD = 10;
    protected const int MAX_LOOP_DISTANCE_THRESHOLD = 30;
    protected const int MAX_PROC_RECTANGLE_AREA = 350;
    protected const int MAX_LOOP_DISTANCE_THRESHOLD_MINES = 50;
    public bool RequiresRegeneration;
    public LoopBuilderComposite.CompositeStyle loopStyle;
    protected IntVector2 m_dimensions;
    protected List<BuilderFlowNode> m_containedNodes;
    protected Dictionary<BuilderFlowNode, BuilderFlowNode> m_externalToInternalNodeMap;
    protected List<BuilderFlowNode> m_externalConnectedNodes;
    protected LoopFlowBuilder m_owner;
    protected DungeonFlow m_flow;
    protected bool LoopCompositeBuildSuccess;
    private const bool DO_PHANTOM_CORRIDORS = false;
    protected bool LinearCompositeBuildSuccess;
    public SemioticLayoutManager CompletedCanvas;

    public LoopBuilderComposite(
      List<BuilderFlowNode> containedNodes,
      DungeonFlow flow,
      LoopFlowBuilder owner,
      LoopBuilderComposite.CompositeStyle loop = LoopBuilderComposite.CompositeStyle.NON_LOOP)
    {
      this.loopStyle = loop;
      this.m_owner = owner;
      this.m_flow = flow;
      this.m_containedNodes = containedNodes;
      this.m_externalConnectedNodes = new List<BuilderFlowNode>();
      this.m_externalToInternalNodeMap = new Dictionary<BuilderFlowNode, BuilderFlowNode>();
      BuilderFlowNode[] array = this.m_containedNodes.ToArray();
      for (int index1 = 0; index1 < this.m_containedNodes.Count; ++index1)
      {
        BuilderFlowNode containedNode = this.m_containedNodes[index1];
        List<BuilderFlowNode> allConnectedNodes = containedNode.GetAllConnectedNodes(array);
        for (int index2 = 0; index2 < allConnectedNodes.Count; ++index2)
        {
          if (!this.m_externalConnectedNodes.Contains(allConnectedNodes[index2]))
          {
            this.m_externalToInternalNodeMap.Add(allConnectedNodes[index2], containedNode);
            this.m_externalConnectedNodes.Add(allConnectedNodes[index2]);
          }
        }
      }
    }

    protected static int GetMaxLoopDistanceThreshold()
    {
      return GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.MINEGEON || GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.OFFICEGEON ? 50 : 30;
    }

    public IntVector2 Dimensions => this.m_dimensions;

    public List<BuilderFlowNode> Nodes => this.m_containedNodes;

    public List<BuilderFlowNode> ExternalConnectedNodes => this.m_externalConnectedNodes;

    public BuilderFlowNode GetConnectedInternalNode(BuilderFlowNode external)
    {
      return this.m_externalToInternalNodeMap.ContainsKey(external) ? this.m_externalToInternalNodeMap[external] : (BuilderFlowNode) null;
    }

    protected static RoomHandler PlacePhantomRoom(
      PrototypeDungeonRoom room,
      SemioticLayoutManager layout,
      IntVector2 newRoomPosition)
    {
      IntVector2 d = new IntVector2(room.Width, room.Height);
      RoomHandler newRoom = new RoomHandler(new CellArea(newRoomPosition, d)
      {
        prototypeRoom = room,
        instanceUsedExits = new List<PrototypeRoomExit>()
      });
      newRoom.distanceFromEntrance = 0;
      newRoom.CalculateOpulence();
      newRoom.CanReceiveCaps = false;
      layout.StampCellAreaToLayout(newRoom);
      return newRoom;
    }

    public static RoomHandler PlaceRoom(
      BuilderFlowNode current,
      SemioticLayoutManager layout,
      IntVector2 newRoomPosition)
    {
      IntVector2 d = new IntVector2(current.assignedPrototypeRoom.Width, current.assignedPrototypeRoom.Height);
      CellArea a = new CellArea(newRoomPosition, d);
      a.prototypeRoom = current.assignedPrototypeRoom;
      a.instanceUsedExits = new List<PrototypeRoomExit>();
      if (current.usesOverrideCategory)
        a.PrototypeRoomCategory = current.overrideCategory;
      RoomHandler newRoom = new RoomHandler(a);
      newRoom.distanceFromEntrance = 0;
      newRoom.CalculateOpulence();
      newRoom.CanReceiveCaps = current.node.receivesCaps;
      current.instanceRoom = newRoom;
      if ((UnityEngine.Object) newRoom.area.prototypeRoom != (UnityEngine.Object) null && current.Category == PrototypeDungeonRoom.RoomCategory.SECRET && current.parentBuilderNode != null && current.parentBuilderNode.instanceRoom != null)
        newRoom.AssignRoomVisualType(current.parentBuilderNode.instanceRoom.RoomVisualSubtype);
      layout.StampCellAreaToLayout(newRoom);
      return newRoom;
    }

    public static void RemoveRoom(BuilderFlowNode current, SemioticLayoutManager layout)
    {
      if (current.instanceRoom == null)
        return;
      for (int index = 0; index < layout.Rooms.Count; ++index)
      {
        if (layout.Rooms[index].connectedRooms.Contains(current.instanceRoom))
          layout.Rooms[index].DeregisterConnectedRoom(current.instanceRoom, layout.Rooms[index].area.exitToLocalDataMap[layout.Rooms[index].GetExitConnectedToRoom(current.instanceRoom)]);
      }
      layout.StampCellAreaToLayout(current.instanceRoom, true);
      current.instanceRoom = (RoomHandler) null;
    }

    protected static void CleanupProceduralRoomConnectivity(
      RoomHandler room,
      SemioticLayoutManager layout)
    {
      for (int index = 0; index < room.connectedRooms.Count; ++index)
      {
        RoomHandler connectedRoom = room.connectedRooms[index];
        if (!layout.Rooms.Contains(connectedRoom))
        {
          PrototypeRoomExit key1 = (PrototypeRoomExit) null;
          foreach (PrototypeRoomExit key2 in room.connectedRoomsByExit.Keys)
          {
            if (room.connectedRoomsByExit[key2] == connectedRoom)
            {
              key1 = key2;
              break;
            }
          }
          if (key1 != null)
          {
            room.area.exitToLocalDataMap.Remove(key1);
            room.area.instanceUsedExits.Remove(key1);
            room.connectedRoomsByExit.Remove(key1);
          }
          room.childRooms.Remove(connectedRoom);
          room.connectedRooms.RemoveAt(index);
          --index;
        }
      }
    }

    protected static void FinalizeProceduralRoomConnectivity(
      RuntimeRoomExitData exitLData,
      RuntimeRoomExitData exitRData,
      RoomHandler initialRoom,
      RoomHandler finalRoom,
      RoomHandler newProceduralRoom)
    {
      PrototypeRoomExit referencedExit1 = exitLData.referencedExit;
      PrototypeRoomExit referencedExit2 = exitRData.referencedExit;
      initialRoom.area.instanceUsedExits.Add(referencedExit1);
      finalRoom.area.instanceUsedExits.Add(referencedExit2);
      initialRoom.area.exitToLocalDataMap.Add(referencedExit1, exitLData);
      finalRoom.area.exitToLocalDataMap.Add(referencedExit2, exitRData);
      newProceduralRoom.parentRoom = initialRoom;
      newProceduralRoom.childRooms.Add(finalRoom);
      newProceduralRoom.connectedRooms.Add(initialRoom);
      newProceduralRoom.connectedRooms.Add(finalRoom);
      initialRoom.childRooms.Add(newProceduralRoom);
      initialRoom.connectedRooms.Add(newProceduralRoom);
      initialRoom.connectedRoomsByExit.Add(referencedExit1, newProceduralRoom);
      finalRoom.childRooms.Add(newProceduralRoom);
      finalRoom.connectedRooms.Add(newProceduralRoom);
      finalRoom.connectedRoomsByExit.Add(referencedExit2, newProceduralRoom);
    }

    public static RoomHandler PlaceProceduralPathRoom(
      IntRect rect,
      RuntimeRoomExitData exitL,
      RuntimeRoomExitData exitR,
      RoomHandler initialRoom,
      RoomHandler finalRoom,
      SemioticLayoutManager layout)
    {
      RoomHandler roomHandler = new RoomHandler(new CellArea(rect.Min, rect.Dimensions));
      roomHandler.distanceFromEntrance = finalRoom.distanceFromEntrance + 1;
      roomHandler.CalculateOpulence();
      layout.StampCellAreaToLayout(roomHandler);
      layout.StampComplexExitToLayout(exitL, initialRoom.area);
      layout.StampComplexExitToLayout(exitR, finalRoom.area);
      LoopBuilderComposite.FinalizeProceduralRoomConnectivity(exitL, exitR, initialRoom, finalRoom, roomHandler);
      return roomHandler;
    }

    protected static List<IntVector2> ComposeRoomFromPath(
      List<IntVector2> path,
      PrototypeRoomExit exitL,
      PrototypeRoomExit exitR)
    {
      if (path.Count < 2)
        return new List<IntVector2>((IEnumerable<IntVector2>) path);
      List<List<IntVector2>> intVector2ListList = new List<List<IntVector2>>();
      List<IntVector2> intVector2List = new List<IntVector2>();
      IntVector2 intVector2_1 = path[1] - path[0];
      intVector2List.Add(path[0]);
      for (int index = 1; index < path.Count; ++index)
      {
        IntVector2 intVector2_2 = path[index] - path[index - 1];
        if (intVector2_2 != intVector2_1)
        {
          intVector2_1 = intVector2_2;
          intVector2ListList.Add(intVector2List);
          intVector2List = new List<IntVector2>();
          intVector2List.Add(path[index - 1]);
        }
        intVector2List.Add(path[index]);
      }
      intVector2ListList.Add(intVector2List);
      HashSet<IntVector2> source = new HashSet<IntVector2>();
      for (int index1 = 0; index1 < intVector2ListList.Count; ++index1)
      {
        IntVector2 intVector2_3 = (intVector2ListList[index1][1] - intVector2ListList[index1][0]).x == 0 ? IntVector2.Right : IntVector2.Up;
        for (int index2 = 0; index2 < intVector2ListList[index1].Count; ++index2)
        {
          if (index2 == 0 || index2 == intVector2ListList[index1].Count - 1)
          {
            source.Add(intVector2ListList[index1][index2]);
            source.Add(intVector2ListList[index1][index2] + IntVector2.Right);
            source.Add(intVector2ListList[index1][index2] + IntVector2.Up);
            source.Add(intVector2ListList[index1][index2] + IntVector2.One);
          }
          else
          {
            source.Add(intVector2ListList[index1][index2]);
            source.Add(intVector2ListList[index1][index2] + intVector2_3);
          }
        }
      }
      return source.ToList<IntVector2>();
    }

    protected static void ConnectPathToExits(
      List<IntVector2> inputPath,
      RuntimeRoomExitData exitL,
      RuntimeRoomExitData exitR,
      RoomHandler initialRoom,
      RoomHandler finalRoom)
    {
      IntVector2 intVector2_1 = initialRoom.area.basePosition + exitL.ExitOrigin - IntVector2.One;
      IntVector2 intVector2_2 = finalRoom.area.basePosition + exitR.ExitOrigin - IntVector2.One;
      if (intVector2_1.x == inputPath[inputPath.Count - 1].x || intVector2_1.y == inputPath[inputPath.Count - 1].y)
      {
        IntVector2 majorAxis = (intVector2_1 - inputPath[inputPath.Count - 1]).MajorAxis;
        while (intVector2_1 != inputPath[inputPath.Count - 1])
          inputPath.Add(inputPath[inputPath.Count - 1] + majorAxis);
      }
      if (intVector2_2.x != inputPath[0].x && intVector2_2.y != inputPath[0].y)
        return;
      IntVector2 majorAxis1 = (intVector2_2 - inputPath[0]).MajorAxis;
      while (intVector2_2 != inputPath[0])
        inputPath.Insert(0, inputPath[0] + majorAxis1);
    }

    public static RoomHandler PlaceProceduralPathRoom(
      List<IntVector2> inputPath,
      RuntimeRoomExitData exitL,
      RuntimeRoomExitData exitR,
      RoomHandler initialRoom,
      RoomHandler finalRoom,
      SemioticLayoutManager layout)
    {
      IntVector2 p = new IntVector2(int.MaxValue, int.MaxValue);
      IntVector2 intVector2 = new IntVector2(int.MinValue, int.MinValue);
      LoopBuilderComposite.ConnectPathToExits(inputPath, exitL, exitR, initialRoom, finalRoom);
      List<IntVector2> intVector2List = LoopBuilderComposite.ComposeRoomFromPath(inputPath, exitL.referencedExit, exitR.referencedExit);
      for (int index = 0; index < intVector2List.Count; ++index)
      {
        p.x = Math.Min(p.x, intVector2List[index].x);
        p.y = Math.Min(p.y, intVector2List[index].y);
        intVector2.x = Math.Max(intVector2.x, intVector2List[index].x);
        intVector2.y = Math.Max(intVector2.y, intVector2List[index].y);
      }
      for (int index = 0; index < intVector2List.Count; ++index)
        intVector2List[index] -= p;
      RoomHandler roomHandler = new RoomHandler(new CellArea(p, intVector2 - p)
      {
        proceduralCells = intVector2List
      });
      roomHandler.distanceFromEntrance = finalRoom.distanceFromEntrance + 1;
      roomHandler.CalculateOpulence();
      layout.StampCellAreaToLayout(roomHandler);
      LoopBuilderComposite.FinalizeProceduralRoomConnectivity(exitL, exitR, initialRoom, finalRoom, roomHandler);
      return roomHandler;
    }

    protected List<Tuple<RuntimeRoomExitData, RuntimeRoomExitData>> GetNumberOfIdealExitPairs(
      BuilderFlowNode parentNode,
      BuilderFlowNode currentNode,
      IntVector2 previousNodeBasePosition,
      int numExits)
    {
      List<Tuple<RuntimeRoomExitData, RuntimeRoomExitData>> exitPairsSimple = this.GetExitPairsSimple(parentNode, currentNode, previousNodeBasePosition);
      List<PrototypeRoomExit> prototypeRoomExitList = new List<PrototypeRoomExit>();
      for (int index1 = 0; index1 < numExits; ++index1)
      {
        float num1 = float.MinValue;
        PrototypeRoomExit prototypeRoomExit = (PrototypeRoomExit) null;
        for (int index2 = 0; index2 < parentNode.assignedPrototypeRoom.exitData.exits.Count; ++index2)
        {
          PrototypeRoomExit exit = parentNode.assignedPrototypeRoom.exitData.exits[index2];
          if (!parentNode.instanceRoom.area.instanceUsedExits.Contains(exit) && !prototypeRoomExitList.Contains(exit))
          {
            int num2 = 0;
            for (int index3 = 0; index3 < parentNode.instanceRoom.area.instanceUsedExits.Count; ++index3)
              num2 += IntVector2.ManhattanDistance(parentNode.instanceRoom.area.instanceUsedExits[index3].GetExitOrigin(0), exit.GetExitOrigin(0));
            for (int index4 = 0; index4 < prototypeRoomExitList.Count; ++index4)
              num2 += IntVector2.ManhattanDistance(prototypeRoomExitList[index4].GetExitOrigin(0), exit.GetExitOrigin(0));
            float num3 = (float) num2 / (float) parentNode.instanceRoom.area.instanceUsedExits.Count;
            if ((double) num3 > (double) num1)
            {
              num1 = num3;
              prototypeRoomExit = exit;
            }
          }
        }
        if (prototypeRoomExit != null)
          prototypeRoomExitList.Add(prototypeRoomExit);
        else
          break;
      }
      for (int index = 0; index < exitPairsSimple.Count; ++index)
      {
        if (!prototypeRoomExitList.Contains(exitPairsSimple[index].First.referencedExit))
        {
          exitPairsSimple.RemoveAt(index);
          --index;
        }
      }
      return exitPairsSimple;
    }

    protected List<Tuple<RuntimeRoomExitData, RuntimeRoomExitData>> GetExitPairsPreferDistance(
      BuilderFlowNode parentNode,
      BuilderFlowNode currentNode,
      IntVector2 previousNodeBasePosition)
    {
      List<Tuple<RuntimeRoomExitData, RuntimeRoomExitData>> exitPairsSimple = this.GetExitPairsSimple(parentNode, currentNode, previousNodeBasePosition);
      if (parentNode.instanceRoom.area.instanceUsedExits.Count < 1)
        return exitPairsSimple;
      Dictionary<PrototypeRoomExit, float> exitsToAverageDistanceMap = new Dictionary<PrototypeRoomExit, float>();
      for (int index1 = 0; index1 < parentNode.assignedPrototypeRoom.exitData.exits.Count; ++index1)
      {
        PrototypeRoomExit exit = parentNode.assignedPrototypeRoom.exitData.exits[index1];
        if (!parentNode.instanceRoom.area.instanceUsedExits.Contains(exit))
        {
          int num1 = 0;
          for (int index2 = 0; index2 < parentNode.instanceRoom.area.instanceUsedExits.Count; ++index2)
            num1 += IntVector2.ManhattanDistance(parentNode.instanceRoom.area.instanceUsedExits[index2].GetExitOrigin(0), exit.GetExitOrigin(0));
          float num2 = (float) num1 / (float) parentNode.instanceRoom.area.instanceUsedExits.Count;
          exitsToAverageDistanceMap.Add(exit, num2);
        }
      }
      return exitPairsSimple.OrderByDescending<Tuple<RuntimeRoomExitData, RuntimeRoomExitData>, float>((Func<Tuple<RuntimeRoomExitData, RuntimeRoomExitData>, float>) (tuple => exitsToAverageDistanceMap[tuple.First.referencedExit])).ToList<Tuple<RuntimeRoomExitData, RuntimeRoomExitData>>();
    }

    protected List<Tuple<RuntimeRoomExitData, RuntimeRoomExitData>> GetExitPairsSimple(
      BuilderFlowNode parentNode,
      BuilderFlowNode currentNode,
      IntVector2 previousNodeBasePosition)
    {
      List<Tuple<RuntimeRoomExitData, RuntimeRoomExitData>> input1 = new List<Tuple<RuntimeRoomExitData, RuntimeRoomExitData>>();
      List<Tuple<RuntimeRoomExitData, RuntimeRoomExitData>> input2 = new List<Tuple<RuntimeRoomExitData, RuntimeRoomExitData>>();
      bool flag1 = true;
      if (parentNode.Category == PrototypeDungeonRoom.RoomCategory.SECRET || currentNode.Category == PrototypeDungeonRoom.RoomCategory.SECRET)
        flag1 = false;
      if (parentNode.Category == PrototypeDungeonRoom.RoomCategory.BOSS || currentNode.Category == PrototypeDungeonRoom.RoomCategory.BOSS)
        flag1 = false;
      if (GameManager.Instance.CurrentGameMode == GameManager.GameMode.BOSSRUSH)
        flag1 = true;
      List<PrototypeRoomExit.ExitGroup> definedExitGroups = parentNode.assignedPrototypeRoom.exitData.GetDefinedExitGroups();
      bool flag2 = definedExitGroups.Count > 1;
      for (int index = 0; index < parentNode.instanceRoom.area.instanceUsedExits.Count; ++index)
        definedExitGroups.Remove(parentNode.instanceRoom.area.instanceUsedExits[index].exitGroup);
      if (definedExitGroups.Count == 0)
        flag2 = false;
      for (int index1 = 0; index1 < parentNode.assignedPrototypeRoom.exitData.exits.Count; ++index1)
      {
        RuntimeRoomExitData first = new RuntimeRoomExitData(parentNode.assignedPrototypeRoom.exitData.exits[index1]);
        if (flag2)
        {
          bool flag3 = false;
          for (int index2 = 0; index2 < parentNode.instanceRoom.area.instanceUsedExits.Count; ++index2)
          {
            if (parentNode.instanceRoom.area.instanceUsedExits[index2].exitGroup == first.referencedExit.exitGroup)
            {
              flag3 = true;
              break;
            }
          }
          if (flag3)
            continue;
        }
        for (int index3 = 0; index3 < currentNode.assignedPrototypeRoom.exitData.exits.Count; ++index3)
        {
          RuntimeRoomExitData second = new RuntimeRoomExitData(currentNode.assignedPrototypeRoom.exitData.exits[index3]);
          if (!parentNode.exitToNodeMap.ContainsKey(first.referencedExit) && !currentNode.exitToNodeMap.ContainsKey(second.referencedExit))
          {
            if (parentNode.node.childNodeGuids.Contains(currentNode.node.guidAsString))
            {
              if (first.referencedExit.exitType == PrototypeRoomExit.ExitType.ENTRANCE_ONLY || second.referencedExit.exitType == PrototypeRoomExit.ExitType.EXIT_ONLY)
                continue;
            }
            else if (currentNode.node.childNodeGuids.Contains(parentNode.node.guidAsString) && (second.referencedExit.exitType == PrototypeRoomExit.ExitType.ENTRANCE_ONLY || first.referencedExit.exitType == PrototypeRoomExit.ExitType.EXIT_ONLY))
              continue;
            if (first.referencedExit.exitDirection == DungeonData.Direction.EAST && second.referencedExit.exitDirection == DungeonData.Direction.WEST || first.referencedExit.exitDirection == DungeonData.Direction.WEST && second.referencedExit.exitDirection == DungeonData.Direction.EAST || first.referencedExit.exitDirection == DungeonData.Direction.NORTH && second.referencedExit.exitDirection == DungeonData.Direction.SOUTH || first.referencedExit.exitDirection == DungeonData.Direction.SOUTH && second.referencedExit.exitDirection == DungeonData.Direction.NORTH)
            {
              Tuple<RuntimeRoomExitData, RuntimeRoomExitData> tuple = new Tuple<RuntimeRoomExitData, RuntimeRoomExitData>(first, second);
              input1.Add(tuple);
            }
            else if (first.referencedExit.exitDirection != second.referencedExit.exitDirection)
            {
              Tuple<RuntimeRoomExitData, RuntimeRoomExitData> tuple = new Tuple<RuntimeRoomExitData, RuntimeRoomExitData>(first, second);
              input2.Add(tuple);
            }
          }
        }
      }
      List<Tuple<RuntimeRoomExitData, RuntimeRoomExitData>> exitPairsSimple = input1.GenerationShuffle<Tuple<RuntimeRoomExitData, RuntimeRoomExitData>>();
      if (flag1)
        exitPairsSimple.AddRange((IEnumerable<Tuple<RuntimeRoomExitData, RuntimeRoomExitData>>) input2.GenerationShuffle<Tuple<RuntimeRoomExitData, RuntimeRoomExitData>>());
      return exitPairsSimple;
    }

    protected List<Tuple<RuntimeRoomExitData, RuntimeRoomExitData>> GetExitPairsForNode(
      BuilderFlowNode placedNode,
      BuilderFlowNode nextNode,
      IntVector2 previousRoomBasePosition,
      BuilderFlowNode currentLoopTargetNode,
      IntVector2 currentLoopTargetBasePosition,
      RoomHandler currentLoopTargetRoom,
      List<FlowActionLine> actionLines,
      bool minimizeLoopDistance)
    {
      List<Tuple<RuntimeRoomExitData, RuntimeRoomExitData>> exitPairsSimple = this.GetExitPairsSimple(placedNode, nextNode, previousRoomBasePosition);
      int[] numArray = new int[exitPairsSimple.Count];
      for (int index1 = 0; index1 < exitPairsSimple.Count; ++index1)
      {
        Tuple<RuntimeRoomExitData, RuntimeRoomExitData> tuple = exitPairsSimple[index1];
        IntVector2 intVector2 = previousRoomBasePosition + tuple.First.ExitOrigin - IntVector2.One - (tuple.Second.ExitOrigin - IntVector2.One);
        int b = int.MaxValue;
        for (int index2 = 0; index2 < nextNode.assignedPrototypeRoom.exitData.exits.Count; ++index2)
        {
          for (int index3 = 0; index3 < currentLoopTargetNode.assignedPrototypeRoom.exitData.exits.Count; ++index3)
          {
            int a = 0;
            PrototypeRoomExit exit1 = nextNode.assignedPrototypeRoom.exitData.exits[index2];
            PrototypeRoomExit exit2 = currentLoopTargetNode.assignedPrototypeRoom.exitData.exits[index3];
            if (exit1 != tuple.Second.referencedExit && !nextNode.exitToNodeMap.ContainsKey(exit1) && !currentLoopTargetNode.exitToNodeMap.ContainsKey(exit2))
            {
              if (minimizeLoopDistance)
                a = IntVector2.ManhattanDistance(currentLoopTargetBasePosition + exit2.GetExitOrigin(exit2.exitLength) - IntVector2.One, intVector2 + exit1.GetExitOrigin(exit1.exitLength) - IntVector2.One);
              b = Mathf.Min(a, b);
            }
          }
        }
        numArray[index1] = b;
      }
      if (minimizeLoopDistance)
      {
        List<Tuple<int, Tuple<RuntimeRoomExitData, RuntimeRoomExitData>>> tupleList = new List<Tuple<int, Tuple<RuntimeRoomExitData, RuntimeRoomExitData>>>();
        for (int index = 0; index < exitPairsSimple.Count; ++index)
          tupleList.Add(new Tuple<int, Tuple<RuntimeRoomExitData, RuntimeRoomExitData>>(numArray[index], exitPairsSimple[index]));
        tupleList.Sort((Comparison<Tuple<int, Tuple<RuntimeRoomExitData, RuntimeRoomExitData>>>) ((a, b) => a.First.CompareTo(b.First)));
        for (int index = 0; index < exitPairsSimple.Count; ++index)
          exitPairsSimple[index] = tupleList[index].Second;
      }
      return exitPairsSimple;
    }

    [DebuggerHidden]
    protected IEnumerable BuildLoopComposite(SemioticLayoutManager layout, IntVector2 startPosition)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      LoopBuilderComposite__BuildLoopCompositec__Iterator0 compositeCIterator0 = new LoopBuilderComposite__BuildLoopCompositec__Iterator0()
      {
        layout = layout,
        startPosition = startPosition,
        _this = this
      };
      // ISSUE: reference to a compiler-generated field
      compositeCIterator0._PC = -2;
      return (IEnumerable) compositeCIterator0;
    }

    protected static IntRect GetExitRect(
      PrototypeRoomExit closestExitL,
      PrototypeRoomExit closestExitR,
      IntVector2 closestExitPositionL,
      IntVector2 closestExitPositionR)
    {
      IntVector2 intVector2_1 = IntVector2.Min(closestExitPositionL, closestExitPositionR);
      IntVector2 intVector2_2 = IntVector2.Max(closestExitPositionL, closestExitPositionR);
      if (closestExitPositionL.x < closestExitPositionR.x)
      {
        if (closestExitL.exitDirection == DungeonData.Direction.EAST)
          intVector2_1 += IntVector2.Right;
        if (closestExitR.exitDirection == DungeonData.Direction.NORTH || closestExitR.exitDirection == DungeonData.Direction.SOUTH)
          intVector2_2 += IntVector2.Right * 2;
      }
      else
      {
        if (closestExitR.exitDirection == DungeonData.Direction.EAST)
          intVector2_1 += IntVector2.Right;
        if (closestExitL.exitDirection == DungeonData.Direction.NORTH || closestExitL.exitDirection == DungeonData.Direction.SOUTH)
          intVector2_2 += IntVector2.Right * 2;
      }
      if (closestExitPositionL.y < closestExitPositionR.y)
      {
        if (closestExitR.exitDirection == DungeonData.Direction.EAST || closestExitR.exitDirection == DungeonData.Direction.WEST)
          intVector2_2 += IntVector2.Up * 2;
        else if (closestExitR.exitDirection == DungeonData.Direction.SOUTH)
          intVector2_2 += IntVector2.Up;
      }
      else if (closestExitL.exitDirection == DungeonData.Direction.EAST || closestExitL.exitDirection == DungeonData.Direction.WEST)
        intVector2_2 += IntVector2.Up * 2;
      else if (closestExitL.exitDirection == DungeonData.Direction.SOUTH)
        intVector2_2 += IntVector2.Up;
      return new IntRect(intVector2_1.x, intVector2_1.y, intVector2_2.x - intVector2_1.x, intVector2_2.y - intVector2_1.y);
    }

    protected static RoomHandler AttemptLoopClosure(
      SemioticLayoutManager layout,
      RoomHandler previousRoomL,
      RoomHandler previousRoomR,
      IntVector2 previousRoomLBasePosition,
      IntVector2 previousRoomRBasePosition,
      int depth,
      DungeonFlow flow)
    {
      List<Tuple<PrototypeRoomExit, PrototypeRoomExit>> seqB = new List<Tuple<PrototypeRoomExit, PrototypeRoomExit>>();
      List<int> seqA = new List<int>();
      List<PrototypeRoomExit.ExitGroup> definedExitGroups1 = previousRoomL.area.prototypeRoom.exitData.GetDefinedExitGroups();
      bool flag1 = definedExitGroups1.Count > 1;
      for (int index = 0; index < previousRoomL.area.instanceUsedExits.Count; ++index)
        definedExitGroups1.Remove(previousRoomL.area.instanceUsedExits[index].exitGroup);
      if (definedExitGroups1.Count == 0)
        flag1 = false;
      List<PrototypeRoomExit.ExitGroup> definedExitGroups2 = previousRoomR.area.prototypeRoom.exitData.GetDefinedExitGroups();
      bool flag2 = definedExitGroups2.Count > 1;
      for (int index = 0; index < previousRoomR.area.instanceUsedExits.Count; ++index)
        definedExitGroups2.Remove(previousRoomR.area.instanceUsedExits[index].exitGroup);
      if (definedExitGroups2.Count == 0)
        flag2 = false;
      for (int index1 = 0; index1 < previousRoomL.area.prototypeRoom.exitData.exits.Count; ++index1)
      {
        PrototypeRoomExit exit1 = previousRoomL.area.prototypeRoom.exitData.exits[index1];
        if (flag1)
        {
          bool flag3 = false;
          for (int index2 = 0; index2 < previousRoomL.area.instanceUsedExits.Count; ++index2)
          {
            if (previousRoomL.area.instanceUsedExits[index2].exitGroup == exit1.exitGroup)
            {
              flag3 = true;
              break;
            }
          }
          if (flag3)
            continue;
        }
        for (int index3 = 0; index3 < previousRoomR.area.prototypeRoom.exitData.exits.Count; ++index3)
        {
          PrototypeRoomExit exit2 = previousRoomR.area.prototypeRoom.exitData.exits[index3];
          if (flag2)
          {
            bool flag4 = false;
            for (int index4 = 0; index4 < previousRoomR.area.instanceUsedExits.Count; ++index4)
            {
              if (previousRoomR.area.instanceUsedExits[index4].exitGroup == exit2.exitGroup)
              {
                flag4 = true;
                break;
              }
            }
            if (flag4)
              continue;
          }
          if (!previousRoomL.area.instanceUsedExits.Contains(exit1) && !previousRoomR.area.instanceUsedExits.Contains(exit2))
          {
            int num = IntVector2.ManhattanDistance(previousRoomLBasePosition + exit1.GetExitOrigin(exit1.exitLength + 3) - IntVector2.One, previousRoomRBasePosition + exit2.GetExitOrigin(exit2.exitLength + 3) - IntVector2.One);
            seqB.Add(new Tuple<PrototypeRoomExit, PrototypeRoomExit>(exit1, exit2));
            seqA.Add(num);
          }
        }
      }
      // ISSUE: object of a compiler-generated type is created
      List<Tuple<PrototypeRoomExit, PrototypeRoomExit>> list = seqA.Zip<int, Tuple<PrototypeRoomExit, PrototypeRoomExit>, __AnonType1<int, Tuple<PrototypeRoomExit, PrototypeRoomExit>>>((IEnumerable<Tuple<PrototypeRoomExit, PrototypeRoomExit>>) seqB, (Func<int, Tuple<PrototypeRoomExit, PrototypeRoomExit>, __AnonType1<int, Tuple<PrototypeRoomExit, PrototypeRoomExit>>>) ((d, p) => new __AnonType1<int, Tuple<PrototypeRoomExit, PrototypeRoomExit>>(d, p))).OrderBy<__AnonType1<int, Tuple<PrototypeRoomExit, PrototypeRoomExit>>, int>((Func<__AnonType1<int, Tuple<PrototypeRoomExit, PrototypeRoomExit>>, int>) (v => v.Dist)).Select<__AnonType1<int, Tuple<PrototypeRoomExit, PrototypeRoomExit>>, Tuple<PrototypeRoomExit, PrototypeRoomExit>>((Func<__AnonType1<int, Tuple<PrototypeRoomExit, PrototypeRoomExit>>, Tuple<PrototypeRoomExit, PrototypeRoomExit>>) (v => v.Pair)).ToList<Tuple<PrototypeRoomExit, PrototypeRoomExit>>();
      RuntimeRoomExitData closestExitL = (RuntimeRoomExitData) null;
      RuntimeRoomExitData closestExitR = (RuntimeRoomExitData) null;
      List<IntVector2> path = (List<IntVector2>) null;
      int num1 = int.MaxValue;
      for (int index5 = 0; index5 < list.Count; ++index5)
      {
        PrototypeRoomExit first = list[index5].First;
        PrototypeRoomExit second = list[index5].Second;
        IntVector2 vector2FromDirection1 = DungeonData.GetIntVector2FromDirection(first.exitDirection);
        IntVector2 vector2FromDirection2 = DungeonData.GetIntVector2FromDirection(second.exitDirection);
        int num2 = first.exitDirection == DungeonData.Direction.SOUTH || first.exitDirection == DungeonData.Direction.WEST ? 2 : 3;
        int num3 = second.exitDirection == DungeonData.Direction.SOUTH || second.exitDirection == DungeonData.Direction.WEST ? 2 : 3;
        IntVector2 intVector2_1 = previousRoomLBasePosition + first.GetExitOrigin(first.exitLength + num2) - IntVector2.One;
        IntVector2 intVector2_2 = previousRoomRBasePosition + second.GetExitOrigin(second.exitLength + num3) - IntVector2.One;
        if (IntVector2.ManhattanDistance(intVector2_1, intVector2_2) < LoopBuilderComposite.GetMaxLoopDistanceThreshold())
        {
          RuntimeRoomExitData exitL = new RuntimeRoomExitData(first);
          exitL.additionalExitLength = 3;
          RuntimeRoomExitData exitR = new RuntimeRoomExitData(second);
          exitR.additionalExitLength = 3;
          IntRect exitRect = LoopBuilderComposite.GetExitRect(first, second, intVector2_1, intVector2_2);
          bool flag5 = exitRect.Width > 6 && exitRect.Height > 6 && (exitRect.Width > 12 || exitRect.Height > 12) && exitRect.Area < 350 && (double) exitRect.Aspect < 5.0 && (double) exitRect.Aspect > 0.20000000298023224;
          if (vector2FromDirection1 == vector2FromDirection2)
            flag5 = false;
          RuntimeRoomExitData exit3 = new RuntimeRoomExitData(first);
          exit3.additionalExitLength = 1;
          RuntimeRoomExitData exit4 = new RuntimeRoomExitData(second);
          exit4.additionalExitLength = 1;
          layout.StampComplexExitTemporary(exit3, previousRoomL.area);
          layout.StampComplexExitTemporary(exit4, previousRoomR.area);
          if (flag5 && layout.CanPlaceRectangle(exitRect))
          {
            IntVector2 intVector2_3 = vector2FromDirection1 + vector2FromDirection2;
            IntRect rect = exitRect;
            for (int index6 = 0; index6 < 5; ++index6)
            {
              int num4 = intVector2_3.x >= 0 ? 0 : index6;
              int num5 = intVector2_3.x <= 0 ? 0 : index6;
              int num6 = intVector2_3.y >= 0 ? 0 : index6;
              int num7 = intVector2_3.y <= 0 ? 0 : index6;
              if (intVector2_3 == IntVector2.Zero)
              {
                if (vector2FromDirection1.y == 0 && vector2FromDirection2.y == 0)
                {
                  num6 = index6;
                  num7 = index6;
                }
                else
                {
                  num4 = index6;
                  num5 = index6;
                }
              }
              IntRect rectangle = new IntRect(exitRect.Left - num4, exitRect.Bottom - num6, exitRect.Width + num4 + num5, exitRect.Height + num6 + num7);
              if (rectangle.Area < 350 && (double) rectangle.Aspect < 5.0 && (double) rectangle.Aspect > 0.20000000298023224 && layout.CanPlaceRectangle(rectangle))
                rect = rectangle;
            }
            layout.ClearTemporary();
            return LoopBuilderComposite.PlaceProceduralPathRoom(rect, exitL, exitR, previousRoomL, previousRoomR, layout);
          }
          layout.ClearTemporary();
          List<IntVector2> positions = layout.PathfindHallwayCompact(intVector2_1, DungeonData.GetIntVector2FromDirection(first.exitDirection), intVector2_2);
          layout.ClearTemporary();
          if (positions != null && !layout.CanPlaceRawCellPositions(positions))
            positions = (List<IntVector2>) null;
          if (positions != null && positions.Count > 0 && positions.Count < num1)
          {
            exitL.additionalExitLength = 0;
            exitR.additionalExitLength = 0;
            closestExitL = exitL;
            closestExitR = exitR;
            path = positions;
            num1 = path.Count;
          }
        }
      }
      if (num1 > LoopBuilderComposite.GetMaxLoopDistanceThreshold())
        return (RoomHandler) null;
      return path != null && path.Count > 0 ? LoopBuilderComposite.ConstructPhantomCorridor(path, closestExitL, closestExitR, previousRoomL, previousRoomR, layout, depth, flow) : (RoomHandler) null;
    }

    protected static RoomHandler ConstructPhantomCorridor(
      List<IntVector2> path,
      RuntimeRoomExitData closestExitL,
      RuntimeRoomExitData closestExitR,
      RoomHandler previousRoomL,
      RoomHandler previousRoomR,
      SemioticLayoutManager layout,
      int depth,
      DungeonFlow flow)
    {
      return path.Count < 4 ? (RoomHandler) null : LoopBuilderComposite.PlaceProceduralPathRoom(path, closestExitL, closestExitR, previousRoomL, previousRoomR, layout);
    }

    [DebuggerHidden]
    protected IEnumerable BuildCompositeDepthFirst(
      SemioticLayoutManager layout,
      IntVector2 startPosition)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      LoopBuilderComposite__BuildCompositeDepthFirstc__Iterator1 depthFirstCIterator1 = new LoopBuilderComposite__BuildCompositeDepthFirstc__Iterator1()
      {
        layout = layout,
        startPosition = startPosition,
        _this = this
      };
      // ISSUE: reference to a compiler-generated field
      depthFirstCIterator1._PC = -2;
      return (IEnumerable) depthFirstCIterator1;
    }

    protected bool BuildComposite(SemioticLayoutManager layout, IntVector2 startPosition)
    {
      BuilderFlowNode containedNode = this.m_containedNodes[0];
      this.AcquireRoomIfNecessary(containedNode);
      RoomHandler pRoom = LoopBuilderComposite.PlaceRoom(containedNode, layout, startPosition);
      Queue<LoopBuilderComposite.CompositeNodeBuildData> buildQueue = new Queue<LoopBuilderComposite.CompositeNodeBuildData>();
      for (int index = 0; index < containedNode.childBuilderNodes.Count; ++index)
      {
        BuilderFlowNode childBuilderNode = containedNode.childBuilderNodes[index];
        if (this.m_containedNodes.Contains(childBuilderNode))
          buildQueue.Enqueue(new LoopBuilderComposite.CompositeNodeBuildData(childBuilderNode, containedNode, pRoom, startPosition));
      }
      bool flag = true;
      while (buildQueue.Count > 0)
      {
        LoopBuilderComposite.CompositeNodeBuildData currentBuildData = buildQueue.Dequeue();
        flag = this.BuildCompositeNode(layout, currentBuildData, buildQueue);
        if (!flag)
          break;
      }
      return flag;
    }

    protected bool AcquireRoomDirectionalIfNecessary(
      BuilderFlowNode buildNode,
      BuilderFlowNode parentNode)
    {
      if (!buildNode.AcquiresRoomAsNecessary)
        return true;
      PrototypeDungeonRoom.RoomCategory category = !buildNode.usesOverrideCategory ? buildNode.node.roomCategory : buildNode.overrideCategory;
      GenericRoomTable genericRoomTable = this.m_flow.fallbackRoomTable;
      if ((UnityEngine.Object) buildNode.node.overrideRoomTable != (UnityEngine.Object) null)
        genericRoomTable = buildNode.node.overrideRoomTable;
      this.m_owner.ClearPlacedRoomData(buildNode);
      PrototypeDungeonRoom assignedPrototypeRoom = parentNode.assignedPrototypeRoom;
      List<DungeonData.Direction> exitDirections = new List<DungeonData.Direction>();
      for (int index = 0; index < assignedPrototypeRoom.exitData.exits.Count; ++index)
      {
        if (!parentNode.exitToNodeMap.ContainsKey(assignedPrototypeRoom.exitData.exits[index]))
        {
          DungeonData.Direction direction = (DungeonData.Direction) ((int) (assignedPrototypeRoom.exitData.exits[index].exitDirection + 4) % 8);
          exitDirections.Add(direction);
        }
      }
      PrototypeDungeonRoom roomByExitDirection = this.m_owner.GetAvailableRoomByExitDirection(category, buildNode.Connectivity, exitDirections, genericRoomTable.GetCompiledList());
      if ((UnityEngine.Object) roomByExitDirection != (UnityEngine.Object) null)
      {
        buildNode.assignedPrototypeRoom = roomByExitDirection;
        this.m_owner.NotifyPlacedRoomData(roomByExitDirection);
        return true;
      }
      UnityEngine.Debug.LogError((object) $"Failed to acquire a prototype room. This means the list is too sparse for the relevant category ({category.ToString()}) or something has gone terribly wrong. We should be falling back gracefully, though.");
      return false;
    }

    protected void AcquireRoomIfNecessary(BuilderFlowNode buildNode)
    {
      if (!buildNode.AcquiresRoomAsNecessary)
        return;
      PrototypeDungeonRoom.RoomCategory category = !buildNode.usesOverrideCategory ? buildNode.node.roomCategory : buildNode.overrideCategory;
      GenericRoomTable genericRoomTable = this.m_flow.fallbackRoomTable;
      if ((UnityEngine.Object) buildNode.node.overrideRoomTable != (UnityEngine.Object) null)
        genericRoomTable = buildNode.node.overrideRoomTable;
      this.m_owner.ClearPlacedRoomData(buildNode);
      PrototypeDungeonRoom availableRoom = this.m_owner.GetAvailableRoom(category, buildNode.Connectivity, genericRoomTable.GetCompiledList());
      if ((UnityEngine.Object) availableRoom != (UnityEngine.Object) null)
      {
        buildNode.assignedPrototypeRoom = availableRoom;
        this.m_owner.NotifyPlacedRoomData(availableRoom);
      }
      else
        UnityEngine.Debug.LogError((object) $"Failed to acquire a prototype room. This means the list is too sparse for the relevant category ({category.ToString()}) or something has gone terribly wrong.");
    }

    protected static void HandleAdditionalRoomPlacementData(
      Tuple<RuntimeRoomExitData, RuntimeRoomExitData> exitPair,
      BuilderFlowNode nextNode,
      BuilderFlowNode previousNode,
      SemioticLayoutManager layout)
    {
      if (previousNode.nodeToExitMap.ContainsKey(nextNode))
        previousNode.nodeToExitMap.Remove(nextNode);
      if (nextNode.nodeToExitMap.ContainsKey(previousNode))
        nextNode.nodeToExitMap.Remove(previousNode);
      previousNode.exitToNodeMap.Add(exitPair.First.referencedExit, nextNode);
      previousNode.nodeToExitMap.Add(nextNode, exitPair.First.referencedExit);
      nextNode.exitToNodeMap.Add(exitPair.Second.referencedExit, previousNode);
      nextNode.nodeToExitMap.Add(previousNode, exitPair.Second.referencedExit);
      layout.StampComplexExitToLayout(exitPair.Second, nextNode.instanceRoom.area);
      layout.StampComplexExitToLayout(exitPair.First, previousNode.instanceRoom.area);
      exitPair.First.linkedExit = exitPair.Second;
      exitPair.Second.linkedExit = exitPair.First;
      if (previousNode.parentBuilderNode == nextNode && previousNode.node.forcedDoorType == DungeonFlowNode.ForcedDoorType.ONE_WAY || nextNode.parentBuilderNode == previousNode && nextNode.node.forcedDoorType == DungeonFlowNode.ForcedDoorType.ONE_WAY)
      {
        exitPair.First.oneWayDoor = true;
        exitPair.Second.oneWayDoor = true;
      }
      if (previousNode.parentBuilderNode == nextNode && previousNode.node.forcedDoorType == DungeonFlowNode.ForcedDoorType.LOCKED || nextNode.parentBuilderNode == previousNode && nextNode.node.forcedDoorType == DungeonFlowNode.ForcedDoorType.LOCKED)
      {
        exitPair.First.isLockedDoor = true;
        exitPair.Second.isLockedDoor = true;
      }
      previousNode.instanceRoom.RegisterConnectedRoom(nextNode.instanceRoom, exitPair.First);
      nextNode.instanceRoom.RegisterConnectedRoom(previousNode.instanceRoom, exitPair.Second);
    }

    protected static void UnhandleAdditionalRoomPlacementData(
      Tuple<RuntimeRoomExitData, RuntimeRoomExitData> exitPair,
      BuilderFlowNode nextNode,
      BuilderFlowNode previousNode,
      SemioticLayoutManager layout)
    {
      previousNode.exitToNodeMap.Remove(exitPair.First.referencedExit);
      previousNode.nodeToExitMap.Remove(nextNode);
      nextNode.exitToNodeMap.Remove(exitPair.Second.referencedExit);
      nextNode.nodeToExitMap.Remove(previousNode);
      layout.StampComplexExitToLayout(exitPair.Second, nextNode.instanceRoom.area, true);
      layout.StampComplexExitToLayout(exitPair.First, previousNode.instanceRoom.area, true);
      exitPair.First.linkedExit = (RuntimeRoomExitData) null;
      exitPair.Second.linkedExit = (RuntimeRoomExitData) null;
      exitPair.First.oneWayDoor = false;
      exitPair.Second.oneWayDoor = false;
      exitPair.First.isLockedDoor = false;
      exitPair.Second.isLockedDoor = false;
      previousNode.instanceRoom.DeregisterConnectedRoom(nextNode.instanceRoom, exitPair.First);
      nextNode.instanceRoom.DeregisterConnectedRoom(previousNode.instanceRoom, exitPair.Second);
    }

    [DebuggerHidden]
    protected IEnumerable<ProcessStatus> BuildCompositeNodeDepthFirst(
      SemioticLayoutManager layout,
      LoopBuilderComposite.CompositeNodeBuildData currentBuildData)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      LoopBuilderComposite__BuildCompositeNodeDepthFirstc__Iterator2 depthFirstCIterator2 = new LoopBuilderComposite__BuildCompositeNodeDepthFirstc__Iterator2()
      {
        currentBuildData = currentBuildData,
        layout = layout,
        _this = this
      };
      // ISSUE: reference to a compiler-generated field
      depthFirstCIterator2._PC = -2;
      return (IEnumerable<ProcessStatus>) depthFirstCIterator2;
    }

    protected bool BuildCompositeNode(
      SemioticLayoutManager layout,
      LoopBuilderComposite.CompositeNodeBuildData currentBuildData,
      Queue<LoopBuilderComposite.CompositeNodeBuildData> buildQueue)
    {
      if (!this.AcquireRoomDirectionalIfNecessary(currentBuildData.node, currentBuildData.parentNode))
        return false;
      List<Tuple<RuntimeRoomExitData, RuntimeRoomExitData>> exitPairsSimple = this.GetExitPairsSimple(currentBuildData.parentNode, currentBuildData.node, currentBuildData.parentBasePosition);
      BuilderFlowNode node = currentBuildData.node;
      BuilderFlowNode parentNode = currentBuildData.parentNode;
      for (int index1 = 0; index1 < exitPairsSimple.Count; ++index1)
      {
        Tuple<RuntimeRoomExitData, RuntimeRoomExitData> exitPair = exitPairsSimple[index1];
        IEnumerator<ProcessStatus> enumerator = layout.CanPlaceRoomAtAttachPointByExit(node.assignedPrototypeRoom, exitPair.Second, currentBuildData.parentBasePosition, exitPair.First).GetEnumerator();
        bool flag = false;
        while (enumerator.MoveNext())
        {
          switch (enumerator.Current)
          {
            case ProcessStatus.Fail:
              flag = false;
              continue;
            case ProcessStatus.Success:
              flag = true;
              continue;
            default:
              continue;
          }
        }
        if (flag)
        {
          IntVector2 intVector2 = currentBuildData.parentBasePosition + exitPair.First.ExitOrigin - IntVector2.One - (exitPair.Second.ExitOrigin - IntVector2.One);
          RoomHandler pRoom = LoopBuilderComposite.PlaceRoom(node, layout, intVector2);
          LoopBuilderComposite.HandleAdditionalRoomPlacementData(exitPair, node, parentNode, layout);
          for (int index2 = 0; index2 < node.childBuilderNodes.Count; ++index2)
          {
            BuilderFlowNode childBuilderNode = node.childBuilderNodes[index2];
            if (this.m_containedNodes.Contains(childBuilderNode))
            {
              LoopBuilderComposite.CompositeNodeBuildData compositeNodeBuildData = new LoopBuilderComposite.CompositeNodeBuildData(childBuilderNode, node, pRoom, intVector2);
              buildQueue.Enqueue(compositeNodeBuildData);
            }
          }
          return true;
        }
      }
      return false;
    }

    protected void PostprocessLoopDirectionality()
    {
      bool flag = false;
      for (int index = 0; index < this.m_containedNodes.Count; ++index)
      {
        if (this.m_containedNodes[index].loopConnectedBuilderNode != null && this.m_containedNodes.Contains(this.m_containedNodes[index].loopConnectedBuilderNode) && this.m_containedNodes[index].node.loopTargetIsOneWay)
          flag = true;
      }
      for (int index = 0; index < this.m_containedNodes.Count; ++index)
      {
        if (this.m_containedNodes[index].instanceRoom != null)
          this.m_containedNodes[index].instanceRoom.LoopIsUnidirectional = flag;
      }
    }

    [DebuggerHidden]
    public IEnumerable Build(IntVector2 startPosition)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      LoopBuilderComposite__Buildc__Iterator3 buildCIterator3 = new LoopBuilderComposite__Buildc__Iterator3()
      {
        startPosition = startPosition,
        _this = this
      };
      // ISSUE: reference to a compiler-generated field
      buildCIterator3._PC = -2;
      return (IEnumerable) buildCIterator3;
    }

    public enum CompositeStyle
    {
      NON_LOOP,
      LOOP,
    }

    protected class CompositeNodeBuildData
    {
      public BuilderFlowNode node;
      public BuilderFlowNode parentNode;
      public RoomHandler parentRoom;
      public IntVector2 parentBasePosition;
      public Tuple<RuntimeRoomExitData, RuntimeRoomExitData> connectionTuple;

      public CompositeNodeBuildData(
        BuilderFlowNode n,
        BuilderFlowNode parent,
        RoomHandler pRoom,
        IntVector2 pbp)
      {
        this.node = n;
        this.parentNode = parent;
        this.parentRoom = pRoom;
        this.parentBasePosition = pbp;
      }
    }
  }
}
