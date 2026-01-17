// Decompiled with JetBrains decompiler
// Type: Pathfinding.Pathfinder
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace Pathfinding
{
  public class Pathfinder : MonoBehaviour
  {
    public static int MaxSteps = 40;
    public Pathfinder.DebugSettings Debug;
    private static List<RoomHandler> m_roomHandlers;
    public static readonly CellTypes s_defaultPassableCellTypes = CellTypes.FLOOR;
    public static Pathfinder Instance;
    private const int c_defaultTileWeight = 2;
    private int m_pass;
    private int m_width;
    private int m_height;
    private Pathfinder.PathNode[] m_nodes;
    private BinaryHeap<Pathfinder.PathNodeProxy> m_openList = new BinaryHeap<Pathfinder.PathNodeProxy>();
    private int m_nearestFailDist;
    private int m_nearestFailId;
    private Dictionary<RoomHandler, List<OccupiedCells>> m_registeredObstacles = new Dictionary<RoomHandler, List<OccupiedCells>>();
    private List<RoomHandler> m_dirtyRooms = new List<RoomHandler>();

    public static bool CellValidator_NoTopWalls(IntVector2 cellPos)
    {
      CellData cellData = GameManager.Instance.Dungeon.data[cellPos];
      return cellData == null || !cellData.IsTopWall();
    }

    public void Awake() => Pathfinder.Instance = this;

    public void Update()
    {
      if (!((UnityEngine.Object) GameManager.Instance.PrimaryPlayer != (UnityEngine.Object) null))
        return;
      RoomHandler currentRoom = GameManager.Instance.PrimaryPlayer.CurrentRoom;
      if (!this.m_dirtyRooms.Contains(currentRoom))
        return;
      this.RecalculateRoomClearances(currentRoom);
      this.m_dirtyRooms.Remove(currentRoom);
    }

    public void OnDestroy() => Pathfinder.Instance = (Pathfinder) null;

    public static void ClearPerLevelData()
    {
      if (Pathfinder.m_roomHandlers == null)
        return;
      Pathfinder.m_roomHandlers = (List<RoomHandler>) null;
    }

    public void Initialize(DungeonData dungeonData)
    {
      this.m_width = dungeonData.Width;
      this.m_height = dungeonData.Height;
      this.m_nodes = new Pathfinder.PathNode[this.m_width * this.m_height];
      for (int x = 0; x < this.m_width; ++x)
      {
        for (int y = 0; y < this.m_height; ++y)
          this.m_nodes[x + y * this.m_width] = new Pathfinder.PathNode(dungeonData.cellData[x][y], x, y);
      }
      this.RecalculateClearances();
    }

    public void InitializeRegion(
      DungeonData dungeonData,
      IntVector2 basePosition,
      IntVector2 dimensions)
    {
      int width = dungeonData.Width;
      int height = dungeonData.Height;
      Pathfinder.PathNode[] pathNodeArray = new Pathfinder.PathNode[width * height];
      for (int index1 = 0; index1 < this.m_width; ++index1)
      {
        for (int index2 = 0; index2 < this.m_height; ++index2)
          pathNodeArray[index1 + index2 * width] = this.m_nodes[index1 + index2 * this.m_width];
      }
      this.m_width = width;
      this.m_height = height;
      this.m_nodes = pathNodeArray;
      for (int x = basePosition.x - 3; x < basePosition.x + dimensions.x + 4; ++x)
      {
        for (int y = basePosition.y - 3; y < basePosition.y + dimensions.y + 4; ++y)
        {
          if (x + y * this.m_width < this.m_nodes.Length && x < dungeonData.cellData.Length && y < dungeonData.cellData[x].Length)
          {
            this.m_nodes[x + y * this.m_width] = new Pathfinder.PathNode(dungeonData.cellData[x][y], x, y);
            BraveUtility.DrawDebugSquare(new Vector2((float) x, (float) y), Color.red, 1000f);
          }
        }
      }
      this.RecalculateClearances(basePosition.x, basePosition.y, basePosition.x + dimensions.x - 1, basePosition.y + dimensions.y - 1);
    }

    public void RegisterObstacle(OccupiedCells cells, RoomHandler parentRoom)
    {
      if (this.m_registeredObstacles.ContainsKey(parentRoom))
        this.m_registeredObstacles[parentRoom].Add(cells);
      else
        this.m_registeredObstacles.Add(parentRoom, new List<OccupiedCells>()
        {
          cells
        });
      this.FlagRoomAsDirty(parentRoom);
    }

    public void DeregisterObstacle(OccupiedCells cells, RoomHandler parentRoom)
    {
      if (this.m_registeredObstacles.ContainsKey(parentRoom))
        this.m_registeredObstacles[parentRoom].Remove(cells);
      this.FlagRoomAsDirty(parentRoom);
    }

    public void FlagRoomAsDirty(RoomHandler room)
    {
      if (this.m_dirtyRooms.Contains(room))
        return;
      this.m_dirtyRooms.Add(room);
    }

    public void TryRecalculateRoomClearances(RoomHandler room)
    {
      if (!this.m_dirtyRooms.Contains(room))
        return;
      this.RecalculateRoomClearances(room);
      this.m_dirtyRooms.Remove(room);
    }

    public void RecalculateRoomClearances(RoomHandler room)
    {
      DungeonData data = GameManager.Instance.Dungeon.data;
      for (int index = 0; index < room.Cells.Count; ++index)
      {
        CellData cellData = data[room.Cells[index]];
        if (cellData != null)
          cellData.isOccupied = false;
      }
      if (this.m_registeredObstacles.ContainsKey(room))
      {
        List<OccupiedCells> registeredObstacle = this.m_registeredObstacles[room];
        for (int index = 0; index < registeredObstacle.Count; ++index)
          registeredObstacle[index].FlagCells();
      }
      if (this.m_nodes == null)
        return;
      int x = room.area.basePosition.x;
      int y = room.area.basePosition.y;
      int maxX = x + room.area.dimensions.x - 1;
      int maxY = y + room.area.dimensions.y - 1;
      this.RecalculateClearances(x, y, maxX, maxY);
    }

    public void RecalculateClearances()
    {
      this.RecalculateClearances(0, 0, this.m_width - 1, this.m_height - 1);
    }

    private void RecalculateClearances(int minX, int minY, int maxX, int maxY)
    {
      for (int index1 = minX; index1 <= maxX; ++index1)
      {
  label_19:
        for (int index2 = minY; index2 <= maxY; ++index2)
        {
          int index3 = index1 + index2 * this.m_width;
          if (!this.m_nodes[index3].IsPassable(Pathfinder.s_defaultPassableCellTypes, false))
          {
            this.m_nodes[index3].SquareClearance = 0;
          }
          else
          {
            int num1 = Mathf.Max(maxX - index1 + 1, maxY - index2 + 1);
            int num2;
            for (num2 = 1; num2 < num1; ++num2)
            {
              for (int index4 = 0; index4 <= num2; ++index4)
              {
                if (!this.m_nodes[index1 + index4 + (index2 + num2) * this.m_width].IsPassable(Pathfinder.s_defaultPassableCellTypes, false))
                {
                  this.m_nodes[index3].SquareClearance = num2;
                  goto label_19;
                }
              }
              for (int index5 = 0; index5 < num2; ++index5)
              {
                if (!this.m_nodes[index1 + num2 + (index2 + index5) * this.m_width].IsPassable(Pathfinder.s_defaultPassableCellTypes, false))
                {
                  this.m_nodes[index3].SquareClearance = num2;
                  goto label_19;
                }
              }
            }
            this.m_nodes[index3].SquareClearance = num2;
          }
        }
      }
    }

    public void Smooth(
      Path path,
      Vector2 startPos,
      Vector2 extents,
      CellTypes passableCellTypes,
      bool canPassOccupied,
      IntVector2 clearance)
    {
      if (path.Positions.Count < 2)
        return;
      foreach (IntVector2 position in path.Positions)
        path.PreSmoothedPositions.AddLast(position);
      extents -= Vector2.one * (0.5f / (float) PhysicsEngine.Instance.PixelsPerUnit);
      LinkedListNode<IntVector2> linkedListNode1 = (LinkedListNode<IntVector2>) null;
      LinkedListNode<IntVector2> linkedListNode2 = path.Positions.First;
      int num = 2;
      while (linkedListNode2 != null)
      {
        if (this.Walkable(startPos, Pathfinder.GetClearanceOffset(linkedListNode2.Value, clearance), extents, passableCellTypes, canPassOccupied, clearance, num > 0))
        {
          LinkedListNode<IntVector2> node = linkedListNode1;
          linkedListNode1 = linkedListNode2;
          linkedListNode2 = linkedListNode2.Next;
          if (node != null)
          {
            path.Positions.Remove(node);
            path.PreSmoothedPositions.AddLast(node.Value);
          }
          if (!canPassOccupied && this.m_nodes[this.GetNodeId(startPos.ToIntVector2(VectorConversions.Floor))].IsOccupied && !this.m_nodes[this.GetNodeId(linkedListNode1.Value)].IsOccupied)
          {
            linkedListNode2 = linkedListNode1;
            break;
          }
          --num;
        }
        else
        {
          linkedListNode2 = linkedListNode1;
          break;
        }
      }
      if (linkedListNode2 == null && linkedListNode1 != null)
        return;
      if (linkedListNode1 == null)
        linkedListNode1 = path.Positions.First;
      LinkedListNode<IntVector2> next = linkedListNode1.Next;
      while (next != null && next.Next != null)
      {
        if (this.Walkable(Pathfinder.GetClearanceOffset(linkedListNode1.Value, clearance), Pathfinder.GetClearanceOffset(next.Next.Value, clearance), extents, passableCellTypes, canPassOccupied, clearance))
        {
          LinkedListNode<IntVector2> node = next;
          next = next.Next;
          path.Positions.Remove(node);
          path.PreSmoothedPositions.AddLast(node.Value);
        }
        else
        {
          linkedListNode1 = next;
          next = next.Next;
        }
      }
    }

    public bool GetPath(
      IntVector2 start,
      IntVector2 end,
      out Path path,
      IntVector2? clearance = null,
      CellTypes passableCellTypes = CellTypes.FLOOR,
      CellValidator cellValidator = null,
      ExtraWeightingFunction extraWeightingFunction = null,
      bool canPassOccupied = false)
    {
      if (!clearance.HasValue)
        clearance = new IntVector2?(IntVector2.Zero);
      return this.GetPath(start, end, passableCellTypes, canPassOccupied, out path, clearance.Value, cellValidator, extraWeightingFunction);
    }

    public void UpdateActorPath(List<IntVector2> path)
    {
      for (int index = 0; index < path.Count; ++index)
        ++this.m_nodes[path[index].x + path[index].y * this.m_width].ActorPathCount;
    }

    public void RemoveActorPath(List<IntVector2> path)
    {
      if (path == null || path.Count == 0)
        return;
      for (int index = 0; index < path.Count; ++index)
      {
        --this.m_nodes[path[index].x + path[index].y * this.m_width].ActorPathCount;
        if (this.m_nodes[path[index].x + path[index].y * this.m_width].ActorPathCount < 0)
          UnityEngine.Debug.LogWarning((object) "Negative ActorPathCount!");
      }
    }

    public bool IsPassable(
      IntVector2 pos,
      IntVector2? clearance = null,
      CellTypes? passableCellTypes = null,
      bool canPassOccupied = false,
      CellValidator cellValidator = null)
    {
      if (!clearance.HasValue)
        clearance = new IntVector2?(IntVector2.One);
      if (!passableCellTypes.HasValue)
        passableCellTypes = new CellTypes?((CellTypes) 2147483647 /*0x7FFFFFFF*/);
      return this.NodeIsValid(pos.x, pos.y) && this.m_nodes[pos.x + pos.y * this.m_width].HasClearance(clearance.Value, passableCellTypes.Value, canPassOccupied) && this.m_nodes[pos.x + pos.y * this.m_width].IsPassable(passableCellTypes.Value, canPassOccupied, cellValidator);
    }

    public bool IsValidPathCell(IntVector2 pos)
    {
      return this.NodeIsValid(pos.x, pos.y) && this.m_nodes[pos.x + pos.y * this.m_width].CellData != null;
    }

    public static Vector2 GetClearanceOffset(IntVector2 pos, IntVector2 clearance)
    {
      return new Vector2((float) pos.x + (float) clearance.x / 2f, (float) pos.y + (float) clearance.y / 2f);
    }

    private bool GetPath(
      IntVector2 start,
      IntVector2 goal,
      CellTypes passableCellTypes,
      bool canPassOccupied,
      out Path path,
      IntVector2 clearance,
      CellValidator cellValidator = null,
      ExtraWeightingFunction extraWeightingFunction = null)
    {
      path = (Path) null;
      int nodeId1 = this.GetNodeId(goal);
      int nodeId2 = start.x + start.y * this.m_width;
      if (start == goal)
      {
        path = new Path();
        return true;
      }
      ++this.m_pass;
      this.m_openList.Clear();
      this.m_nodes[nodeId2].Pass = this.m_pass;
      this.m_nodes[nodeId2].ParentId = -1;
      this.m_nodes[nodeId2].Steps = 0;
      this.m_nodes[nodeId2].CombinedWeight = this.m_nodes[nodeId2].GetWeight(clearance, passableCellTypes);
      this.m_nodes[nodeId2].EstimatedRemainingDist = IntVector2.ManhattanDistance(this.m_nodes[nodeId2].Position, goal) * 2;
      this.m_nearestFailDist = this.m_nodes[nodeId2].EstimatedRemainingDist + this.m_nodes[nodeId2].ActorPathCount * 2 * 3;
      this.m_nearestFailId = nodeId2;
      this.m_openList.Add(new Pathfinder.PathNodeProxy(nodeId2, this.m_nodes[nodeId2].EstimatedCost));
      while (this.m_openList.Count > 0)
      {
        int nodeId3 = this.m_openList.Remove().NodeId;
        if (this.AtGoal(this.m_nodes[nodeId3].Position, this.m_nodes[nodeId1].Position, clearance))
        {
          path = this.RecreatePath(nodeId3, clearance);
          return true;
        }
        if (this.m_nodes[nodeId3].Steps < Pathfinder.MaxSteps)
        {
          IntVector2 position = this.m_nodes[nodeId3].Position;
          if (position.y < this.m_height - 1)
            this.VisitNode(nodeId3, this.GetNodeId(this.m_nodes[nodeId3].Position + IntVector2.Up), goal, passableCellTypes, canPassOccupied, clearance, cellValidator, extraWeightingFunction);
          if (position.y > 0)
            this.VisitNode(nodeId3, this.GetNodeId(this.m_nodes[nodeId3].Position + IntVector2.Down), goal, passableCellTypes, canPassOccupied, clearance, cellValidator, extraWeightingFunction);
          if (position.x > 0)
            this.VisitNode(nodeId3, this.GetNodeId(this.m_nodes[nodeId3].Position + IntVector2.Left), goal, passableCellTypes, canPassOccupied, clearance, cellValidator, extraWeightingFunction);
          if (position.x < this.m_width - 1)
            this.VisitNode(nodeId3, this.GetNodeId(this.m_nodes[nodeId3].Position + IntVector2.Right), goal, passableCellTypes, canPassOccupied, clearance, cellValidator, extraWeightingFunction);
        }
      }
      if (this.m_nearestFailId == nodeId2)
        return false;
      path = this.RecreatePath(this.m_nearestFailId, clearance);
      path.WillReachFinalGoal = false;
      return true;
    }

    private int GetNodeId(IntVector2 pos) => pos.x + pos.y * this.m_width;

    private int GetNodeId(int x, int y) => x + y * this.m_width;

    private bool NodeIsValid(int x, int y)
    {
      return (x < 0 || x >= this.m_width || y < 0 ? 1 : (y >= this.m_height ? 1 : 0)) == 0;
    }

    private bool AtGoal(IntVector2 currentPos, IntVector2 goalPos, IntVector2 clearance)
    {
      if (clearance == IntVector2.One)
        return currentPos == goalPos;
      IntVector2 intVector2 = goalPos - currentPos;
      return intVector2.x >= 0 && intVector2.y >= 0 && intVector2.x < clearance.x && intVector2.y < clearance.y;
    }

    private void VisitNode(
      int prevId,
      int nodeId,
      IntVector2 goal,
      CellTypes passableCellTypes,
      bool canPassOccupied,
      IntVector2 clearance,
      CellValidator cellValidator = null,
      ExtraWeightingFunction extraWeightingFunction = null)
    {
      if (this.m_nodes[nodeId].Pass == this.m_pass || this.m_nodes[nodeId].CellData == null || !this.m_nodes[nodeId].IsPassable(passableCellTypes, canPassOccupied, cellValidator) || !this.m_nodes[nodeId].HasClearance(clearance, passableCellTypes, canPassOccupied))
        return;
      this.m_nodes[nodeId].Pass = this.m_pass;
      this.m_nodes[nodeId].ParentId = prevId;
      this.m_nodes[nodeId].Steps = this.m_nodes[prevId].Steps + 1;
      this.m_nodes[nodeId].CombinedWeight = this.m_nodes[prevId].CombinedWeight + this.m_nodes[nodeId].GetWeight(clearance, passableCellTypes);
      this.m_nodes[nodeId].EstimatedRemainingDist = IntVector2.ManhattanDistance(this.m_nodes[nodeId].Position, goal) * 2;
      int num = this.m_nodes[nodeId].EstimatedRemainingDist + this.m_nodes[nodeId].ActorPathCount * 2 * 3;
      if (extraWeightingFunction != null)
      {
        IntVector2 thisStep = this.m_nodes[nodeId].Position - this.m_nodes[prevId].Position;
        IntVector2 prevStep = IntVector2.Zero;
        int parentId = this.m_nodes[prevId].ParentId;
        if (parentId != -1)
          prevStep = this.m_nodes[prevId].Position - this.m_nodes[parentId].Position;
        this.m_nodes[nodeId].CombinedWeight += extraWeightingFunction(prevStep, thisStep);
      }
      if (num < this.m_nearestFailDist)
      {
        this.m_nearestFailId = nodeId;
        this.m_nearestFailDist = num;
      }
      this.m_openList.Add(new Pathfinder.PathNodeProxy(nodeId, this.m_nodes[nodeId].EstimatedCost));
    }

    private Path RecreatePath(int destId, IntVector2 clearance)
    {
      LinkedList<IntVector2> positions = new LinkedList<IntVector2>();
      for (int index = destId; index >= 0; index = this.m_nodes[index].ParentId)
        positions.AddFirst(this.m_nodes[index].Position);
      return new Path(positions, clearance);
    }

    private bool Walkable(
      Vector2 start,
      Vector2 end,
      Vector2 extents,
      CellTypes passableCellTypes,
      bool canPassOccupied,
      IntVector2 clearance,
      bool ignoreWeightChecks = false)
    {
      if ((double) (end - start).magnitude < 0.20000000298023224)
        return true;
      Vector2 vector2_1 = (end - start).normalized / 5f;
      float magnitude = vector2_1.magnitude;
      float num1 = Vector2.Distance(start, end);
      float a1 = float.MaxValue;
      float a2 = 0.0f;
      bool flag1 = false;
      bool flag2 = false;
      for (int index1 = 0; index1 < 4; ++index1)
      {
        int x;
        int y;
        IntVector2 clearance1;
        switch (index1)
        {
          case 0:
            x = (int) ((double) start.x + (double) extents.x);
            y = (int) ((double) start.y + (double) extents.y);
            clearance1 = new IntVector2(1, 1);
            break;
          case 1:
            x = (int) ((double) start.x + (double) extents.x);
            y = (int) ((double) start.y - (double) extents.y);
            clearance1 = new IntVector2(1, clearance.y);
            break;
          case 2:
            x = (int) ((double) start.x - (double) extents.x);
            y = (int) ((double) start.y + (double) extents.y);
            clearance1 = new IntVector2(clearance.x, 1);
            break;
          default:
            x = (int) ((double) start.x - (double) extents.x);
            y = (int) ((double) start.y - (double) extents.y);
            clearance1 = clearance;
            break;
        }
        if (this.NodeIsValid(x, y))
        {
          int index2 = x + y * this.m_width;
          a2 = Mathf.Max(a2, (float) this.m_nodes[index2].GetWeight(clearance1, passableCellTypes));
          if (this.m_nodes[index2].IsOccupied)
            flag1 = true;
          if (this.m_nodes[index2].CellType == CellType.PIT)
            flag2 = true;
        }
      }
      if ((double) a2 > 0.0)
        a1 = a2;
      Vector2 vector2_2 = start;
      while ((double) num1 >= 0.0)
      {
        float num2 = 0.0f;
        bool flag3 = false;
        bool flag4 = false;
        for (int index3 = 0; index3 < 4; ++index3)
        {
          int x;
          int y;
          IntVector2 clearance2;
          switch (index3)
          {
            case 0:
              x = (int) ((double) vector2_2.x + (double) extents.x);
              y = (int) ((double) vector2_2.y + (double) extents.y);
              clearance2 = new IntVector2(1, 1);
              break;
            case 1:
              x = (int) ((double) vector2_2.x + (double) extents.x);
              y = (int) ((double) vector2_2.y - (double) extents.y);
              clearance2 = new IntVector2(1, clearance.y);
              break;
            case 2:
              x = (int) ((double) vector2_2.x - (double) extents.x);
              y = (int) ((double) vector2_2.y + (double) extents.y);
              clearance2 = new IntVector2(clearance.x, 1);
              break;
            default:
              x = (int) ((double) vector2_2.x - (double) extents.x);
              y = (int) ((double) vector2_2.y - (double) extents.y);
              clearance2 = clearance;
              break;
          }
          int index4 = x + y * this.m_width;
          if (!this.NodeIsValid(x, y) || !this.m_nodes[index4].IsPassable(!flag2 ? passableCellTypes : passableCellTypes | CellTypes.PIT, canPassOccupied || flag1) || !ignoreWeightChecks && (double) this.m_nodes[index4].GetWeight(clearance2, passableCellTypes) > (double) a1)
            return false;
          flag3 |= this.m_nodes[index4].IsOccupied;
          flag4 |= this.m_nodes[index4].CellType == CellType.PIT;
          num2 = Mathf.Max(num2, (float) this.m_nodes[index4].GetWeight(clearance, passableCellTypes));
        }
        vector2_2 += vector2_1;
        num1 -= magnitude;
        a1 = Mathf.Min(a1, num2);
        flag1 &= flag3;
        flag2 &= flag4;
      }
      return true;
    }

    private bool HasRectClearance(
      IntVector2 position,
      IntVector2 clearance,
      CellTypes passableCellTypes,
      bool canPassOccupied)
    {
      for (int x = position.x; x < position.x + clearance.x; ++x)
      {
        for (int y = position.y; y < position.y + clearance.y; ++y)
        {
          if (x < 0 || x >= this.m_width || y < 0 || y >= this.m_height || !this.m_nodes[x + y * this.m_width].IsPassable(passableCellTypes, canPassOccupied))
            return false;
        }
      }
      return true;
    }

    public static bool HasInstance => (UnityEngine.Object) Pathfinder.Instance != (UnityEngine.Object) null;

    private struct PathNode(CellData cellData, int x, int y) : IComparable<Pathfinder.PathNode>
    {
      public readonly CellData CellData = cellData;
      public IntVector2 Position = new IntVector2(x, y);
      public int Pass = 0;
      public int ParentId = 0;
      public int Steps = 0;
      public int CombinedWeight = 0;
      public int ActorPathCount = 0;
      public int EstimatedRemainingDist = 0;
      public int FailDist = 0;
      public int SquareClearance = 0;

      public int EstimatedCost => this.CombinedWeight + this.EstimatedRemainingDist + 2;

      public bool IsOccupied => this.CellData != null && this.CellData.isOccupied;

      public CellType CellType => this.CellData == null ? CellType.WALL : this.CellData.type;

      public int GetWeight(IntVector2 clearance, CellTypes passableCellTypes)
      {
        bool flag1 = (passableCellTypes & CellTypes.PIT) == CellTypes.PIT;
        bool flag2 = this.CellData.isOccludedByTopWall;
        bool flag3 = this.CellData.isNextToWall;
        bool flag4 = !flag1 && this.CellData.type == CellType.PIT && !this.CellData.fallingPrevented;
        if (clearance.x > 1 || clearance.y > 1)
        {
          for (int index1 = 0; index1 < clearance.x && !flag2; ++index1)
          {
            int num1 = this.CellData.position.x + index1;
            for (int index2 = 0; index2 < clearance.y && !flag2; ++index2)
            {
              if (index1 != 0 || index2 != 0)
              {
                int num2 = this.CellData.position.y + index2;
                if (num1 >= 0 && num1 < Pathfinder.Instance.m_width && num2 >= 0 && num2 < Pathfinder.Instance.m_height)
                {
                  CellData cellData = Pathfinder.Instance.m_nodes[num1 + num2 * Pathfinder.Instance.m_width].CellData;
                  if (cellData.isOccludedByTopWall)
                    flag2 = true;
                  else if (cellData.isNextToWall)
                    flag3 = true;
                  if (!flag1 && cellData.type == CellType.PIT && !cellData.fallingPrevented)
                    flag4 = true;
                }
              }
            }
          }
        }
        int weight = 2 + this.ActorPathCount;
        if (flag3)
          weight += 10;
        if (flag2)
          weight += 2000;
        if (flag4)
          weight += 10;
        return weight;
      }

      public bool IsPassable(
        CellTypes passableCellTypes,
        bool canPassOccupied,
        CellValidator cellValidator = null)
      {
        return (canPassOccupied || !this.IsOccupied) && ((passableCellTypes & (CellTypes) this.CellType) == (CellTypes) this.CellType || this.CellType == CellType.PIT && this.CellData.fallingPrevented && (passableCellTypes & CellTypes.FLOOR) == CellTypes.FLOOR) && (cellValidator == null || cellValidator(this.Position));
      }

      public bool HasClearance(
        IntVector2 clearance,
        CellTypes passableCellTypes,
        bool canPassOccupied)
      {
        return clearance.x == clearance.y && passableCellTypes == Pathfinder.s_defaultPassableCellTypes && !canPassOccupied ? clearance.x <= this.SquareClearance : Pathfinder.Instance.HasRectClearance(this.CellData.position, clearance, passableCellTypes, canPassOccupied);
      }

      public int CompareTo(Pathfinder.PathNode other) => this.EstimatedCost - other.EstimatedCost;
    }

    private struct PathNodeProxy(int nodeId, int estimatedCost) : IComparable<Pathfinder.PathNodeProxy>
    {
      public int NodeId = nodeId;
      public int EstimatedCost = estimatedCost;

      public int CompareTo(Pathfinder.PathNodeProxy other)
      {
        return this.EstimatedCost - other.EstimatedCost;
      }
    }

    [Serializable]
    public class DebugSettings
    {
      public bool DrawGrid;
      public bool DrawImpassable;
      public bool DrawWeights;
      public bool DrawRoomNums;
      public bool DrawPaths;
      public bool TestPath;
      public SpeculativeRigidbody TestPathOrigin;
      public Vector2 TestPathClearance = new Vector2(1f, 1f);
    }
  }
}
