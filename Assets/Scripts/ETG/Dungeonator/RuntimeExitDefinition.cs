using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace Dungeonator
{
  public class RuntimeExitDefinition
  {
    public RuntimeRoomExitData upstreamExit;
    public RuntimeRoomExitData downstreamExit;
    private const int SUBDOOR_CORRIDOR_THRESHOLD = 7;
    public RoomHandler upstreamRoom;
    public RoomHandler downstreamRoom;
    public DungeonDoorController linkedDoor;
    public bool containsLight;
    public bool isCriticalPath;
    public HashSet<IntVector2> ExitOccluderCells;
    public HashSet<IntVector2> IntermediaryCells;
    protected HashSet<IntVector2> m_upstreamCells;
    protected HashSet<IntVector2> m_downstreamCells;
    private bool m_westgeonProcessed;

    public RuntimeExitDefinition(
      RuntimeRoomExitData uExit,
      RuntimeRoomExitData dExit,
      RoomHandler upstream,
      RoomHandler downstream)
    {
      if (upstream.distanceFromEntrance <= downstream.distanceFromEntrance || downstream.area.IsProceduralRoom)
      {
        this.upstreamExit = uExit;
        this.downstreamExit = dExit;
        this.upstreamRoom = upstream;
        this.downstreamRoom = downstream;
      }
      else
      {
        this.upstreamExit = dExit;
        this.downstreamExit = uExit;
        this.upstreamRoom = downstream;
        this.downstreamRoom = upstream;
      }
      if (upstream.IsOnCriticalPath && downstream.IsOnCriticalPath)
      {
        this.isCriticalPath = true;
        if (uExit != null)
          uExit.isCriticalPath = true;
        if (dExit != null)
          dExit.isCriticalPath = true;
      }
      this.CalculateCellData();
      if (!this.isCriticalPath)
        return;
      if (this.upstreamExit != null && this.upstreamExit.referencedExit != null)
        BraveUtility.DrawDebugSquare((this.upstreamExit.referencedExit.GetExitOrigin(0) - IntVector2.One + this.upstreamRoom.area.basePosition + -3 * DungeonData.GetIntVector2FromDirection(this.upstreamExit.referencedExit.exitDirection)).ToVector2(), Color.red, 1000f);
      if (this.downstreamExit == null || this.downstreamExit.referencedExit == null)
        return;
      BraveUtility.DrawDebugSquare((this.downstreamExit.referencedExit.GetExitOrigin(0) - IntVector2.One + this.downstreamRoom.area.basePosition + -3 * DungeonData.GetIntVector2FromDirection(this.downstreamExit.referencedExit.exitDirection)).ToVector2(), Color.blue, 1000f);
    }

    public RoomHandler.VisibilityStatus Visibility
    {
      get
      {
        return this.IsVisibleFromRoom(GameManager.Instance.PrimaryPlayer.CurrentRoom) || this.ExitOccluderCells.Contains(GameManager.Instance.PrimaryPlayer.transform.position.IntXY(VectorConversions.Floor)) ? RoomHandler.VisibilityStatus.CURRENT : RoomHandler.VisibilityStatus.OBSCURED;
      }
    }

    public bool ContainsPosition(IntVector2 position)
    {
      if (this.m_upstreamCells != null && this.m_upstreamCells.Contains(position))
        return true;
      return this.m_downstreamCells != null && this.m_downstreamCells.Contains(position);
    }

    public void RemovePosition(IntVector2 position)
    {
      if (this.m_upstreamCells != null)
        this.m_upstreamCells.Remove(position);
      if (this.m_downstreamCells != null)
        this.m_downstreamCells.Remove(position);
      if (this.IntermediaryCells == null)
        return;
      this.IntermediaryCells.Remove(position);
    }

    public IntVector2 GetLinearMidpoint(RoomHandler baseRoom)
    {
      if (baseRoom == this.upstreamRoom)
        return this.upstreamExit.jointedExit || this.downstreamExit == null ? this.upstreamExit.ExitOrigin - IntVector2.One : this.upstreamExit.referencedExit.GetExitOrigin((this.upstreamExit.TotalExitLength + this.downstreamExit.TotalExitLength) / 2) - IntVector2.One;
      if (baseRoom == this.downstreamRoom)
        return this.downstreamExit.jointedExit || this.upstreamExit == null ? this.downstreamExit.ExitOrigin - IntVector2.One : this.downstreamExit.referencedExit.GetExitOrigin((this.upstreamExit.TotalExitLength + this.downstreamExit.TotalExitLength) / 2) - IntVector2.One;
      Debug.LogError((object) "SHOULD NEVER OCCUR. LIGHTING PLACEMENT ERROR.");
      return IntVector2.Zero;
    }

    public DungeonData.Direction GetDirectionFromRoom(RoomHandler sourceRoom)
    {
      if (sourceRoom == this.upstreamRoom)
        return this.upstreamExit == null || this.upstreamExit.referencedExit == null ? (DungeonData.Direction) ((int) (this.downstreamExit.referencedExit.exitDirection + 4) % 8) : this.upstreamExit.referencedExit.exitDirection;
      if (sourceRoom == this.downstreamRoom)
        return this.downstreamExit == null || this.downstreamExit.referencedExit == null ? (DungeonData.Direction) ((int) (this.upstreamExit.referencedExit.exitDirection + 4) % 8) : this.downstreamExit.referencedExit.exitDirection;
      Debug.LogError((object) "This should never happen.");
      return ~DungeonData.Direction.NORTH;
    }

    public void GetExitLine(RoomHandler sourceRoom, out Vector2 p1, out Vector2 p2)
    {
      p1 = sourceRoom.GetCellAdjacentToExit(this).ToVector2();
      switch (this.GetDirectionFromRoom(sourceRoom))
      {
        case DungeonData.Direction.NORTH:
          p1 += new Vector2(0.0f, 1f);
          p2 = p1 + new Vector2(2f, 0.0f);
          break;
        case DungeonData.Direction.EAST:
          p1 += new Vector2(1f, 0.0f);
          p2 = p1 + new Vector2(0.0f, 2f);
          break;
        case DungeonData.Direction.SOUTH:
          p2 = p1 + new Vector2(2f, 0.0f);
          break;
        case DungeonData.Direction.WEST:
          p2 = p1 + new Vector2(0.0f, 2f);
          break;
        default:
          Debug.LogError((object) "This should never happen.");
          p2 = Vector2.zero;
          break;
      }
    }

    public HashSet<IntVector2> GetCellsForRoom(RoomHandler r)
    {
      if (r == this.upstreamRoom)
        return this.m_upstreamCells;
      return r == this.downstreamRoom ? this.m_downstreamCells : (HashSet<IntVector2>) null;
    }

    public HashSet<IntVector2> GetCellsForOtherRoom(RoomHandler r)
    {
      if (r == this.upstreamRoom)
        return this.m_downstreamCells;
      return r == this.downstreamRoom ? this.m_upstreamCells : (HashSet<IntVector2>) null;
    }

    private void PlaceExitDecorables(
      RoomHandler targetRoom,
      RuntimeRoomExitData targetExit,
      RoomHandler otherRoom)
    {
      if (otherRoom.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.SPECIAL && otherRoom.area.prototypeRoom.subCategorySpecial == PrototypeDungeonRoom.RoomSpecialSubCategory.WEIRD_SHOP)
        otherRoom.OptionalDoorTopDecorable = ResourceCache.Acquire("Global Prefabs/Purple_Lantern") as GameObject;
      GameObject original = !((Object) otherRoom.area.prototypeRoom != (Object) null) || !((Object) otherRoom.area.prototypeRoom.doorTopDecorable != (Object) null) ? otherRoom.OptionalDoorTopDecorable : otherRoom.area.prototypeRoom.doorTopDecorable;
      if (targetExit == null || targetExit.referencedExit == null || targetRoom == null || otherRoom == null || targetRoom.IsSecretRoom || otherRoom.IsSecretRoom || !((Object) otherRoom.area.prototypeRoom != (Object) null) || !((Object) original != (Object) null))
        return;
      IntVector2 intVector2 = targetExit.referencedExit.GetExitOrigin(0) - IntVector2.One + targetRoom.area.basePosition + -3 * DungeonData.GetIntVector2FromDirection(targetExit.referencedExit.exitDirection);
      Vector2 vector2_1 = intVector2.ToVector2();
      Vector2 vector2_2 = intVector2.ToVector2();
      switch (targetExit.referencedExit.exitDirection)
      {
        case DungeonData.Direction.NORTH:
          vector2_1 += new Vector2(-1.5f, 3.5f);
          vector2_2 += new Vector2(2.5f, 3.5f);
          break;
        case DungeonData.Direction.EAST:
          vector2_1 += new Vector2(1.5f, -0.5f);
          vector2_2 += new Vector2(1.5f, 4.5f);
          break;
        case DungeonData.Direction.SOUTH:
          vector2_1 += new Vector2(-1.5f, 0.5f);
          vector2_2 += new Vector2(2.5f, 0.5f);
          break;
        case DungeonData.Direction.WEST:
          vector2_1 += new Vector2(-1.5f, -0.5f);
          vector2_2 += new Vector2(-1.5f, 4.5f);
          break;
      }
      if ((double) Random.value < 0.40000000596046448)
        Object.Instantiate<GameObject>(original, vector2_1.ToVector3ZUp() + original.transform.position, Quaternion.identity);
      else if ((double) Random.value < 0.800000011920929)
      {
        Object.Instantiate<GameObject>(original, vector2_2.ToVector3ZUp() + original.transform.position, Quaternion.identity);
      }
      else
      {
        Object.Instantiate<GameObject>(original, vector2_1.ToVector3ZUp() + original.transform.position, Quaternion.identity);
        Object.Instantiate<GameObject>(original, vector2_2.ToVector3ZUp() + original.transform.position, Quaternion.identity);
      }
    }

    public void ProcessWestgeonData()
    {
      if (this.m_westgeonProcessed)
        return;
      this.m_westgeonProcessed = true;
      if (GameManager.Instance.Dungeon.tileIndices.tilesetId != GlobalDungeonData.ValidTilesets.WESTGEON)
        return;
      if (this.upstreamExit != null && this.upstreamExit.referencedExit != null && this.upstreamExit.referencedExit.exitDirection == DungeonData.Direction.NORTH && this.upstreamRoom.RoomVisualSubtype == 0 && this.downstreamRoom.RoomVisualSubtype != 0)
        this.ProcessWestgeonSection(this.upstreamExit.referencedExit.GetExitOrigin(0) - IntVector2.One + this.upstreamRoom.area.basePosition + -2 * DungeonData.GetIntVector2FromDirection(this.upstreamExit.referencedExit.exitDirection), this.downstreamRoom);
      if (this.downstreamExit == null || this.downstreamExit.referencedExit == null || this.downstreamExit.referencedExit.exitDirection != DungeonData.Direction.NORTH || this.downstreamRoom.RoomVisualSubtype != 0 || this.upstreamRoom.RoomVisualSubtype == 0)
        return;
      this.ProcessWestgeonSection(this.downstreamExit.referencedExit.GetExitOrigin(0) - IntVector2.One + this.downstreamRoom.area.basePosition + -2 * DungeonData.GetIntVector2FromDirection(this.downstreamExit.referencedExit.exitDirection), this.upstreamRoom);
    }

    private void ProcessWestgeonSection(IntVector2 exitConnection, RoomHandler inheritRoom)
    {
      IntVector2 key1 = exitConnection + IntVector2.Left;
      IntVector2 key2 = exitConnection + IntVector2.Right * 2;
      IntVector2 intVector2_1 = key1;
      IntVector2 intVector2_2 = key2;
      CellData cellData1 = GameManager.Instance.Dungeon.data[key1];
      int num = -1;
      for (; cellData1 != null && cellData1.IsLowerFaceWall(); cellData1 = GameManager.Instance.Dungeon.data[cellData1.position + IntVector2.Left])
      {
        intVector2_1 = cellData1.position;
        cellData1.cellVisualData.IsFacewallForInteriorTransition = true;
        cellData1.cellVisualData.InteriorTransitionIndex = inheritRoom.RoomVisualSubtype;
        num = inheritRoom.RoomVisualSubtype;
        for (CellData cellData2 = GameManager.Instance.Dungeon.data[cellData1.position + IntVector2.Up]; cellData2 != null && (cellData2.IsUpperFacewall() || cellData2.type == CellType.WALL) && (cellData2.nearestRoom == this.upstreamRoom || cellData2.nearestRoom == this.downstreamRoom); cellData2 = GameManager.Instance.Dungeon.data[cellData2.position + IntVector2.Up])
        {
          cellData2.cellVisualData.IsFacewallForInteriorTransition = true;
          cellData2.cellVisualData.InteriorTransitionIndex = inheritRoom.RoomVisualSubtype;
          if (!GameManager.Instance.Dungeon.data.CheckInBounds(cellData2.position + IntVector2.Up))
            break;
        }
      }
      for (CellData cellData3 = GameManager.Instance.Dungeon.data[key2]; cellData3 != null && cellData3.IsLowerFaceWall(); cellData3 = GameManager.Instance.Dungeon.data[cellData3.position + IntVector2.Right])
      {
        intVector2_2 = cellData3.position;
        cellData3.cellVisualData.IsFacewallForInteriorTransition = true;
        cellData3.cellVisualData.InteriorTransitionIndex = inheritRoom.RoomVisualSubtype;
        num = inheritRoom.RoomVisualSubtype;
        for (CellData cellData4 = GameManager.Instance.Dungeon.data[cellData3.position + IntVector2.Up]; cellData4 != null && (cellData4.IsUpperFacewall() || cellData4.type == CellType.WALL) && (cellData4.nearestRoom == this.upstreamRoom || cellData4.nearestRoom == this.downstreamRoom); cellData4 = GameManager.Instance.Dungeon.data[cellData4.position + IntVector2.Up])
        {
          cellData4.cellVisualData.IsFacewallForInteriorTransition = true;
          cellData4.cellVisualData.InteriorTransitionIndex = inheritRoom.RoomVisualSubtype;
          if (!GameManager.Instance.Dungeon.data.CheckInBounds(cellData4.position + IntVector2.Up))
            break;
        }
      }
      if (num == -1)
        return;
      IntVector2 intVector2_3 = intVector2_1 + IntVector2.Down;
      IntVector2 intVector2_4 = intVector2_2 + IntVector2.Down;
      for (int x = intVector2_3.x; x <= intVector2_4.x; ++x)
      {
        GameManager.Instance.Dungeon.data[x, intVector2_4.y].cellVisualData.IsFacewallForInteriorTransition = true;
        GameManager.Instance.Dungeon.data[x, intVector2_4.y].cellVisualData.InteriorTransitionIndex = num;
      }
    }

    private bool CheckRowIsFloor(int minX, int maxX, int iy)
    {
      for (int x = minX; x <= maxX; ++x)
      {
        if (GameManager.Instance.Dungeon.data[x, iy].type != CellType.FLOOR)
          return false;
      }
      return true;
    }

    public IntVector2 GetDownstreamBasePosition()
    {
      if (this.upstreamRoom.area.IsProceduralRoom)
        return this.GetUpstreamBasePosition();
      IntVector2 rhs = IntVector2.Zero;
      switch (this.upstreamExit.referencedExit.exitDirection)
      {
        case DungeonData.Direction.NORTH:
          rhs = new IntVector2(int.MaxValue, int.MinValue);
          foreach (IntVector2 downstreamCell in this.m_downstreamCells)
            rhs = new IntVector2(Mathf.Min(rhs.x, downstreamCell.x), Mathf.Max(rhs.y, downstreamCell.y));
          rhs += IntVector2.Up;
          break;
        case DungeonData.Direction.EAST:
          rhs = new IntVector2(int.MinValue, int.MaxValue);
          using (HashSet<IntVector2>.Enumerator enumerator = this.m_downstreamCells.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              IntVector2 current = enumerator.Current;
              rhs = new IntVector2(Mathf.Max(rhs.x, current.x), Mathf.Min(rhs.y, current.y));
            }
            break;
          }
        case DungeonData.Direction.SOUTH:
          rhs = new IntVector2(int.MaxValue, int.MaxValue);
          using (HashSet<IntVector2>.Enumerator enumerator = this.m_downstreamCells.GetEnumerator())
          {
            while (enumerator.MoveNext())
              rhs = IntVector2.Min(enumerator.Current, rhs);
            break;
          }
        case DungeonData.Direction.WEST:
          rhs = new IntVector2(int.MaxValue, int.MaxValue);
          using (HashSet<IntVector2>.Enumerator enumerator = this.m_downstreamCells.GetEnumerator())
          {
            while (enumerator.MoveNext())
              rhs = IntVector2.Min(enumerator.Current, rhs);
            break;
          }
      }
      return rhs;
    }

    public IntVector2 GetUpstreamBasePosition()
    {
      if (this.downstreamExit == null || this.downstreamExit.referencedExit == null)
        return this.GetDownstreamBasePosition();
      IntVector2 rhs = IntVector2.Zero;
      switch (this.downstreamExit.referencedExit.exitDirection)
      {
        case DungeonData.Direction.NORTH:
          rhs = new IntVector2(int.MaxValue, int.MinValue);
          foreach (IntVector2 upstreamCell in this.m_upstreamCells)
            rhs = new IntVector2(Mathf.Min(rhs.x, upstreamCell.x), Mathf.Max(rhs.y, upstreamCell.y));
          rhs += IntVector2.Up;
          break;
        case DungeonData.Direction.EAST:
          rhs = new IntVector2(int.MinValue, int.MaxValue);
          using (HashSet<IntVector2>.Enumerator enumerator = this.m_upstreamCells.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              IntVector2 current = enumerator.Current;
              rhs = new IntVector2(Mathf.Max(rhs.x, current.x), Mathf.Min(rhs.y, current.y));
            }
            break;
          }
        case DungeonData.Direction.SOUTH:
          rhs = new IntVector2(int.MaxValue, int.MaxValue);
          using (HashSet<IntVector2>.Enumerator enumerator = this.m_upstreamCells.GetEnumerator())
          {
            while (enumerator.MoveNext())
              rhs = IntVector2.Min(enumerator.Current, rhs);
            break;
          }
        case DungeonData.Direction.WEST:
          rhs = new IntVector2(int.MaxValue, int.MaxValue);
          using (HashSet<IntVector2>.Enumerator enumerator = this.m_upstreamCells.GetEnumerator())
          {
            while (enumerator.MoveNext())
              rhs = IntVector2.Min(enumerator.Current, rhs);
            break;
          }
      }
      return rhs;
    }

    public IntVector2 GetDownstreamNearDoorPosition()
    {
      if (this.upstreamRoom.area.IsProceduralRoom || this.downstreamExit == null || this.downstreamExit.referencedExit == null)
        return this.GetUpstreamNearDoorPosition();
      IntVector2 nearDoorPosition = IntVector2.Zero;
      switch (this.downstreamExit.referencedExit.exitDirection)
      {
        case DungeonData.Direction.NORTH:
          nearDoorPosition = new IntVector2(int.MaxValue, int.MaxValue);
          using (HashSet<IntVector2>.Enumerator enumerator = this.m_downstreamCells.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              IntVector2 current = enumerator.Current;
              if (current.y < nearDoorPosition.y || current.y == nearDoorPosition.y && current.x < nearDoorPosition.x)
                nearDoorPosition = current;
            }
            break;
          }
        case DungeonData.Direction.EAST:
          nearDoorPosition = new IntVector2(int.MaxValue, int.MaxValue);
          using (HashSet<IntVector2>.Enumerator enumerator = this.m_downstreamCells.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              IntVector2 current = enumerator.Current;
              if (current.x < nearDoorPosition.x || current.x == nearDoorPosition.x && current.y < nearDoorPosition.y)
                nearDoorPosition = current;
            }
            break;
          }
        case DungeonData.Direction.SOUTH:
          nearDoorPosition = new IntVector2(int.MaxValue, int.MinValue);
          using (HashSet<IntVector2>.Enumerator enumerator = this.m_downstreamCells.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              IntVector2 current = enumerator.Current;
              if (current.y > nearDoorPosition.y || current.y == nearDoorPosition.y && current.x < nearDoorPosition.x)
                nearDoorPosition = current;
            }
            break;
          }
        case DungeonData.Direction.WEST:
          nearDoorPosition = new IntVector2(int.MinValue, int.MaxValue);
          using (HashSet<IntVector2>.Enumerator enumerator = this.m_downstreamCells.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              IntVector2 current = enumerator.Current;
              if (current.x > nearDoorPosition.x || current.x == nearDoorPosition.x && current.y < nearDoorPosition.y)
                nearDoorPosition = current;
            }
            break;
          }
      }
      return nearDoorPosition;
    }

    public IntVector2 GetUpstreamNearDoorPosition()
    {
      if (this.upstreamExit == null || this.upstreamExit.referencedExit == null)
        return this.GetDownstreamNearDoorPosition();
      IntVector2 nearDoorPosition = IntVector2.Zero;
      switch (this.upstreamExit.referencedExit.exitDirection)
      {
        case DungeonData.Direction.NORTH:
          nearDoorPosition = new IntVector2(int.MaxValue, int.MaxValue);
          using (HashSet<IntVector2>.Enumerator enumerator = this.m_upstreamCells.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              IntVector2 current = enumerator.Current;
              if (current.y < nearDoorPosition.y || current.y == nearDoorPosition.y && current.x < nearDoorPosition.x)
                nearDoorPosition = current;
            }
            break;
          }
        case DungeonData.Direction.EAST:
          nearDoorPosition = new IntVector2(int.MaxValue, int.MaxValue);
          using (HashSet<IntVector2>.Enumerator enumerator = this.m_upstreamCells.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              IntVector2 current = enumerator.Current;
              if (current.x < nearDoorPosition.x || current.x == nearDoorPosition.x && current.y < nearDoorPosition.y)
                nearDoorPosition = current;
            }
            break;
          }
        case DungeonData.Direction.SOUTH:
          nearDoorPosition = new IntVector2(int.MaxValue, int.MinValue);
          using (HashSet<IntVector2>.Enumerator enumerator = this.m_upstreamCells.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              IntVector2 current = enumerator.Current;
              if (current.y > nearDoorPosition.y || current.y == nearDoorPosition.y && current.x < nearDoorPosition.x)
                nearDoorPosition = current;
            }
            break;
          }
        case DungeonData.Direction.WEST:
          nearDoorPosition = new IntVector2(int.MinValue, int.MaxValue);
          using (HashSet<IntVector2>.Enumerator enumerator = this.m_upstreamCells.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              IntVector2 current = enumerator.Current;
              if (current.x > nearDoorPosition.x || current.x == nearDoorPosition.x && current.y < nearDoorPosition.y)
                nearDoorPosition = current;
            }
            break;
          }
      }
      return nearDoorPosition;
    }

    public bool IsVisibleFromRoom(RoomHandler room)
    {
      if (Pixelator.Instance.UseTexturedOcclusion)
        return true;
      if ((Object) this.linkedDoor == (Object) null)
      {
        if (room == this.upstreamRoom)
        {
          if (this.downstreamRoom != null && (Object) this.downstreamRoom.secretRoomManager != (Object) null)
            return this.downstreamRoom.secretRoomManager.IsOpen;
        }
        else if (room == this.downstreamRoom && this.upstreamRoom != null && (Object) this.upstreamRoom.secretRoomManager != (Object) null)
          return this.upstreamRoom.secretRoomManager.IsOpen;
        return room == this.downstreamRoom;
      }
      if (room == this.upstreamRoom)
      {
        if ((Object) this.linkedDoor.subsidiaryBlocker == (Object) null && (Object) this.linkedDoor.subsidiaryDoor == (Object) null)
          return !this.m_upstreamCells.Contains(this.GetDoorPositionForExit(this.upstreamExit, this.upstreamRoom)) || this.linkedDoor.IsOpenForVisibilityTest;
        if ((Object) this.linkedDoor.subsidiaryBlocker != (Object) null)
        {
          if (this.linkedDoor.IsOpenForVisibilityTest && !this.linkedDoor.subsidiaryBlocker.isSealed)
            return true;
        }
        else if ((Object) this.linkedDoor.subsidiaryDoor != (Object) null && this.linkedDoor.IsOpenForVisibilityTest && this.linkedDoor.subsidiaryDoor.IsOpenForVisibilityTest)
          return true;
        if (this.m_upstreamCells.Contains(this.GetDoorPositionForExit(this.upstreamExit, this.upstreamRoom)))
        {
          if (this.linkedDoor.IsOpenForVisibilityTest)
            return true;
        }
        else if ((Object) this.linkedDoor.subsidiaryBlocker != (Object) null)
        {
          if (!this.linkedDoor.subsidiaryBlocker.isSealed)
            return true;
        }
        else if ((Object) this.linkedDoor.subsidiaryDoor != (Object) null && this.linkedDoor.subsidiaryDoor.IsOpenForVisibilityTest)
          return true;
        return false;
      }
      if (room != this.downstreamRoom)
        return false;
      if ((Object) this.linkedDoor.subsidiaryBlocker == (Object) null && (Object) this.linkedDoor.subsidiaryDoor == (Object) null)
        return !this.m_downstreamCells.Contains(this.GetDoorPositionForExit(this.upstreamExit, this.upstreamRoom)) || this.linkedDoor.IsOpenForVisibilityTest;
      if ((Object) this.linkedDoor.subsidiaryBlocker != (Object) null)
      {
        if (this.linkedDoor.IsOpenForVisibilityTest && !this.linkedDoor.subsidiaryBlocker.isSealed)
          return true;
      }
      else if ((Object) this.linkedDoor.subsidiaryDoor != (Object) null && this.linkedDoor.IsOpenForVisibilityTest && this.linkedDoor.subsidiaryDoor.IsOpenForVisibilityTest)
        return true;
      if (this.m_upstreamCells.Contains(this.GetDoorPositionForExit(this.upstreamExit, this.upstreamRoom)))
      {
        if ((Object) this.linkedDoor.subsidiaryBlocker != (Object) null)
        {
          if (!this.linkedDoor.subsidiaryBlocker.isSealed)
            return true;
        }
        else if ((Object) this.linkedDoor.subsidiaryDoor != (Object) null && this.linkedDoor.subsidiaryDoor.IsOpenForVisibilityTest)
          return true;
      }
      else if (this.linkedDoor.IsOpenForVisibilityTest)
        return true;
      return false;
    }

    public void ProcessExitDecorables()
    {
      if (this.upstreamRoom != null && ((Object) this.upstreamRoom.secretRoomManager != (Object) null || this.upstreamRoom.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.SECRET) || this.downstreamRoom != null && ((Object) this.downstreamRoom.secretRoomManager != (Object) null || this.downstreamRoom.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.SECRET))
        return;
      this.PlaceExitDecorables(this.upstreamRoom, this.upstreamExit, this.downstreamRoom);
      this.PlaceExitDecorables(this.downstreamRoom, this.downstreamExit, this.upstreamRoom);
    }

    public void StampCellVisualTypes(DungeonData dungeonData)
    {
      if (GameManager.Instance.Dungeon.UsesWallWarpWingDoors && (this.upstreamExit != null && this.upstreamExit.isWarpWingStart || this.downstreamExit != null && this.downstreamExit.isWarpWingStart))
        this.GenerateWarpWingPortals();
      foreach (IntVector2 intVector2 in this.GetCellsForRoom(this.upstreamRoom))
      {
        if (this.upstreamExit.referencedExit.exitDirection == DungeonData.Direction.NORTH || this.upstreamExit.referencedExit.exitDirection == DungeonData.Direction.SOUTH)
        {
          dungeonData[intVector2.x, intVector2.y].cellVisualData.roomVisualTypeIndex = this.upstreamRoom.RoomVisualSubtype;
          dungeonData[intVector2.x - 1, intVector2.y + 2].cellVisualData.roomVisualTypeIndex = this.upstreamRoom.RoomVisualSubtype;
          dungeonData[intVector2.x + 1, intVector2.y + 2].cellVisualData.roomVisualTypeIndex = this.upstreamRoom.RoomVisualSubtype;
        }
        else
        {
          dungeonData[intVector2.x, intVector2.y].cellVisualData.roomVisualTypeIndex = this.upstreamRoom.RoomVisualSubtype;
          if (dungeonData[intVector2.x, intVector2.y + 1].type == CellType.WALL)
            dungeonData[intVector2.x, intVector2.y + 1].cellVisualData.roomVisualTypeIndex = this.upstreamRoom.RoomVisualSubtype;
          if (dungeonData[intVector2.x, intVector2.y + 2].type == CellType.WALL)
            dungeonData[intVector2.x, intVector2.y + 2].cellVisualData.roomVisualTypeIndex = this.upstreamRoom.RoomVisualSubtype;
          if (dungeonData[intVector2.x, intVector2.y + 3].type == CellType.WALL)
            dungeonData[intVector2.x, intVector2.y + 3].cellVisualData.roomVisualTypeIndex = this.upstreamRoom.RoomVisualSubtype;
        }
      }
      if (this.downstreamRoom != null && this.downstreamExit != null)
      {
        foreach (IntVector2 intVector2 in this.GetCellsForRoom(this.downstreamRoom))
        {
          if (this.downstreamExit.referencedExit.exitDirection == DungeonData.Direction.NORTH || this.downstreamExit.referencedExit.exitDirection == DungeonData.Direction.SOUTH)
          {
            dungeonData[intVector2.x, intVector2.y].cellVisualData.roomVisualTypeIndex = this.downstreamRoom.RoomVisualSubtype;
            dungeonData[intVector2.x - 1, intVector2.y + 1].cellVisualData.roomVisualTypeIndex = this.downstreamRoom.RoomVisualSubtype;
            dungeonData[intVector2.x + 1, intVector2.y + 1].cellVisualData.roomVisualTypeIndex = this.downstreamRoom.RoomVisualSubtype;
          }
          else
          {
            dungeonData[intVector2.x, intVector2.y].cellVisualData.roomVisualTypeIndex = this.downstreamRoom.RoomVisualSubtype;
            if (dungeonData[intVector2.x, intVector2.y + 1].type == CellType.WALL)
              dungeonData[intVector2.x, intVector2.y + 1].cellVisualData.roomVisualTypeIndex = this.downstreamRoom.RoomVisualSubtype;
            if (dungeonData[intVector2.x, intVector2.y + 2].type == CellType.WALL)
              dungeonData[intVector2.x, intVector2.y + 2].cellVisualData.roomVisualTypeIndex = this.downstreamRoom.RoomVisualSubtype;
            if (dungeonData[intVector2.x, intVector2.y + 3].type == CellType.WALL)
              dungeonData[intVector2.x, intVector2.y + 3].cellVisualData.roomVisualTypeIndex = this.downstreamRoom.RoomVisualSubtype;
          }
        }
      }
      if (this.IntermediaryCells == null || this.IntermediaryCells.Count <= 0)
        return;
      foreach (IntVector2 intermediaryCell in this.IntermediaryCells)
      {
        dungeonData[intermediaryCell.x, intermediaryCell.y].cellVisualData.roomVisualTypeIndex = this.upstreamRoom.RoomVisualSubtype;
        for (int index1 = -1; index1 < 2; ++index1)
        {
          if (index1 == 0 || dungeonData[intermediaryCell.x + index1, intermediaryCell.y].type == CellType.WALL)
          {
            for (int index2 = !this.upstreamExit.jointedExit || this.upstreamExit.referencedExit.exitDirection != DungeonData.Direction.SOUTH && this.downstreamExit.referencedExit.exitDirection != DungeonData.Direction.SOUTH ? 2 : 0; index2 < 4; ++index2)
            {
              if (dungeonData[intermediaryCell.x + index1, intermediaryCell.y + index2].type == CellType.WALL)
                dungeonData[intermediaryCell.x + index1, intermediaryCell.y + index2].cellVisualData.roomVisualTypeIndex = this.upstreamRoom.RoomVisualSubtype;
            }
          }
        }
      }
    }

    protected void CleanupCellDataForWarpWingExits()
    {
      IntVector2 lhs1 = new IntVector2(int.MaxValue, int.MaxValue);
      IntVector2 lhs2 = new IntVector2(int.MinValue, int.MinValue);
      foreach (IntVector2 upstreamCell in this.m_upstreamCells)
      {
        lhs1 = IntVector2.Min(lhs1, upstreamCell);
        lhs2 = IntVector2.Max(lhs2, upstreamCell);
      }
      for (int x = lhs1.x; x <= lhs2.x; ++x)
      {
        for (int y = lhs1.y; y <= lhs2.y; ++y)
          this.m_upstreamCells.Add(new IntVector2(x, y));
      }
    }

    protected void CalculateCellData()
    {
      this.m_upstreamCells = new HashSet<IntVector2>();
      this.m_downstreamCells = new HashSet<IntVector2>();
      this.ExitOccluderCells = new HashSet<IntVector2>();
      DungeonData data = GameManager.Instance.Dungeon.data;
      IntVector2 doorPositionForExit = this.GetDoorPositionForExit(this.upstreamExit, this.upstreamRoom);
      bool flag1 = this.RequiresSubDoor();
      IntVector2 intVector2_1 = IntVector2.Zero;
      if (flag1)
        intVector2_1 = this.GetSubDoorPositionForExit(this.upstreamExit, this.upstreamRoom);
      if (flag1)
        this.IntermediaryCells = new HashSet<IntVector2>();
      if (this.upstreamExit != null)
      {
        IntVector2 vector2FromDirection = DungeonData.GetIntVector2FromDirection(this.upstreamExit.referencedExit.exitDirection);
        int num1 = !this.upstreamExit.jointedExit || this.upstreamExit.referencedExit.exitDirection == DungeonData.Direction.WEST ? 0 : 1;
        if ((Object) this.downstreamRoom.area.prototypeRoom == (Object) null && this.downstreamRoom.area.IsProceduralRoom && this.downstreamRoom.area.proceduralCells == null && this.upstreamExit.referencedExit.exitDirection != DungeonData.Direction.EAST)
          --num1;
        bool flag2 = false;
        bool flag3 = false;
        bool flag4 = false;
        int num2 = 2;
        for (int index1 = 0; index1 < this.upstreamExit.TotalExitLength + num1; ++index1)
        {
          for (int index2 = 0; index2 < this.upstreamExit.referencedExit.containedCells.Count; ++index2)
          {
            IntVector2 intVector2_2 = this.upstreamExit.referencedExit.containedCells[index2].ToIntVector2() - IntVector2.One + this.upstreamRoom.area.basePosition + vector2FromDirection * index1;
            if (intVector2_2 == doorPositionForExit)
              flag2 = true;
            if (flag1)
            {
              if (intVector2_2 == intVector2_1)
                flag4 = true;
              if (flag4 || flag2)
                this.IntermediaryCells.Add(intVector2_2);
            }
            if (flag3)
              this.m_downstreamCells.Add(intVector2_2);
            else
              this.m_upstreamCells.Add(intVector2_2);
            if (index1 <= num2 && data.CheckInBoundsAndValid(intVector2_2))
              data[intVector2_2].occlusionData.sharedRoomAndExitCell = true;
          }
          if (flag2)
            flag3 = true;
        }
      }
      if (this.downstreamExit != null)
      {
        IntVector2 vector2FromDirection = DungeonData.GetIntVector2FromDirection(this.downstreamExit.referencedExit.exitDirection);
        int num3 = !this.downstreamExit.jointedExit || this.downstreamExit.referencedExit.exitDirection == DungeonData.Direction.WEST ? 0 : 1;
        if ((Object) this.downstreamRoom.area.prototypeRoom == (Object) null && this.downstreamRoom.area.IsProceduralRoom && this.downstreamRoom.area.proceduralCells == null && this.upstreamExit.referencedExit.exitDirection != DungeonData.Direction.EAST)
          --num3;
        bool flag5 = false;
        bool flag6 = false;
        bool flag7 = false;
        int num4 = 2;
        for (int index3 = 0; index3 < this.downstreamExit.TotalExitLength + num3; ++index3)
        {
          for (int index4 = 0; index4 < this.downstreamExit.referencedExit.containedCells.Count; ++index4)
          {
            IntVector2 intVector2_3 = this.downstreamExit.referencedExit.containedCells[index4].ToIntVector2() - IntVector2.One + this.downstreamRoom.area.basePosition + vector2FromDirection * index3;
            if (intVector2_3 == doorPositionForExit)
              flag5 = true;
            if (flag1)
            {
              if (intVector2_3 == intVector2_1)
                flag7 = true;
              if (flag7 || flag5)
                this.IntermediaryCells.Add(intVector2_3);
            }
            if (flag6)
              this.m_upstreamCells.Add(intVector2_3);
            else
              this.m_downstreamCells.Add(intVector2_3);
            if (index3 <= num4)
            {
              if (!data.CheckInBoundsAndValid(intVector2_3))
              {
                Debug.Log((object) $"{(object) intVector2_3} is out of bounds for {(this.upstreamRoom == null ? (object) "null" : (object) this.upstreamRoom.GetRoomName())} | {(this.downstreamRoom == null ? (object) "null" : (object) this.downstreamRoom.GetRoomName())}");
                Debug.Log((object) $"{(object) this.upstreamRoom.area.basePosition}|{(object) this.downstreamRoom.area.basePosition}");
              }
              else
                data[intVector2_3].occlusionData.sharedRoomAndExitCell = true;
            }
          }
          if (flag5)
            flag6 = true;
        }
      }
      if (this.downstreamExit != null)
      {
        IntVector2 intVector2_4 = this.upstreamRoom.area.basePosition + this.upstreamExit.ExitOrigin - IntVector2.One;
        if (this.upstreamExit.referencedExit.exitDirection == DungeonData.Direction.EAST || this.downstreamExit.referencedExit.exitDirection == DungeonData.Direction.EAST)
          intVector2_4 += IntVector2.Right;
        if (this.upstreamExit.referencedExit.exitDirection == DungeonData.Direction.NORTH || this.downstreamExit.referencedExit.exitDirection == DungeonData.Direction.NORTH)
          intVector2_4 += IntVector2.Up;
        IntVector2 intVector2_5 = this.downstreamRoom.area.basePosition + this.downstreamExit.ExitOrigin - IntVector2.One;
        if (this.upstreamExit.referencedExit.exitDirection == DungeonData.Direction.EAST || this.downstreamExit.referencedExit.exitDirection == DungeonData.Direction.EAST)
          intVector2_5 += IntVector2.Right;
        if (this.upstreamExit.referencedExit.exitDirection == DungeonData.Direction.NORTH || this.downstreamExit.referencedExit.exitDirection == DungeonData.Direction.NORTH)
          intVector2_5 += IntVector2.Up;
        if (this.IntermediaryCells == null)
          this.IntermediaryCells = new HashSet<IntVector2>();
        if (!this.m_upstreamCells.Contains(intVector2_4) && !this.m_downstreamCells.Contains(intVector2_4) && !this.IntermediaryCells.Contains(intVector2_4))
        {
          this.m_upstreamCells.Add(intVector2_4);
          this.IntermediaryCells.Add(intVector2_4);
        }
        if (!this.m_upstreamCells.Contains(intVector2_5) && !this.m_downstreamCells.Contains(intVector2_5) && !this.IntermediaryCells.Contains(intVector2_5))
        {
          this.m_downstreamCells.Add(intVector2_5);
          this.IntermediaryCells.Add(intVector2_5);
        }
      }
      if (this.upstreamRoom != null && !this.upstreamRoom.area.IsProceduralRoom && this.upstreamRoom.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.SECRET || this.downstreamRoom != null && !this.downstreamRoom.area.IsProceduralRoom && this.downstreamRoom.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.SECRET)
        this.CorrectForSecretRoomDoorlessness();
      if (this.upstreamExit != null && this.upstreamExit.isWarpWingStart || this.downstreamExit != null && this.downstreamExit.isWarpWingStart)
        this.CleanupCellDataForWarpWingExits();
      if (this.m_upstreamCells != null)
      {
        foreach (IntVector2 upstreamCell in this.m_upstreamCells)
          this.ExitOccluderCells.Add(upstreamCell);
      }
      if (this.m_downstreamCells != null)
      {
        foreach (IntVector2 downstreamCell in this.m_downstreamCells)
          this.ExitOccluderCells.Add(downstreamCell);
      }
      if (this.IntermediaryCells != null)
      {
        foreach (IntVector2 intermediaryCell in this.IntermediaryCells)
          this.ExitOccluderCells.Add(intermediaryCell);
      }
      HashSet<IntVector2> intVector2Set = new HashSet<IntVector2>();
      foreach (IntVector2 exitOccluderCell in this.ExitOccluderCells)
      {
        if (this.ExitOccluderCells.Contains(exitOccluderCell + IntVector2.Right * 2) || this.ExitOccluderCells.Contains(exitOccluderCell + IntVector2.Left * 2) || this.ExitOccluderCells.Contains(exitOccluderCell + IntVector2.Left) && this.ExitOccluderCells.Contains(exitOccluderCell + IntVector2.Right))
        {
          intVector2Set.Add(exitOccluderCell + IntVector2.Up);
          intVector2Set.Add(exitOccluderCell + IntVector2.Up * 2);
        }
      }
      foreach (IntVector2 intVector2_6 in intVector2Set)
        this.ExitOccluderCells.Add(intVector2_6);
      foreach (IntVector2 exitOccluderCell in this.ExitOccluderCells)
      {
        if (data.CheckInBoundsAndValid(exitOccluderCell))
          data[exitOccluderCell].occlusionData.occlusionParentDefintion = this;
      }
    }

    public void CorrectForSecretRoomDoorlessness()
    {
      DungeonData data = GameManager.Instance.Dungeon.data;
      IntVector2[] cardinals = IntVector2.Cardinals;
      foreach (IntVector2 upstreamCell in this.m_upstreamCells)
      {
        if (data.CheckInBoundsAndValid(upstreamCell))
        {
          data[upstreamCell].occlusionData.sharedRoomAndExitCell = true;
          data[upstreamCell].parentRoom = this.upstreamRoom;
          data[upstreamCell].parentArea = this.upstreamRoom.area;
        }
      }
      foreach (IntVector2 downstreamCell in this.m_downstreamCells)
      {
        if (data.CheckInBoundsAndValid(downstreamCell))
        {
          data[downstreamCell].occlusionData.sharedRoomAndExitCell = true;
          data[downstreamCell].parentRoom = this.upstreamRoom;
          data[downstreamCell].parentArea = this.upstreamRoom.area;
        }
      }
      foreach (IntVector2 downstreamCell in this.m_downstreamCells)
      {
        for (int index = 0; index < cardinals.Length; ++index)
        {
          IntVector2 intVector2 = downstreamCell + cardinals[index];
          if (!this.m_downstreamCells.Contains(intVector2) && this.m_upstreamCells.Contains(intVector2))
          {
            if (this.IntermediaryCells == null)
              this.IntermediaryCells = new HashSet<IntVector2>();
            this.IntermediaryCells.Add(intVector2);
            this.IntermediaryCells.Add(downstreamCell);
          }
        }
      }
    }

    protected DungeonData.Direction GetSubsidiaryDoorDirection()
    {
      return !this.upstreamExit.jointedExit ? this.downstreamExit.referencedExit.exitDirection : (this.upstreamExit.oneWayDoor || this.upstreamExit.referencedExit.exitDirection != DungeonData.Direction.EAST && this.upstreamExit.referencedExit.exitDirection != DungeonData.Direction.WEST ? this.downstreamExit.referencedExit.exitDirection : this.upstreamExit.referencedExit.exitDirection);
    }

    protected void GenerateSubsidiaryDoor(
      DungeonData dungeonData,
      DungeonPlaceable sourcePlaceable,
      DungeonDoorController mainDoor,
      Transform doorParentTransform)
    {
      IntVector2 doorPositionForExit = this.GetSubDoorPositionForExit(this.upstreamExit, this.upstreamRoom);
      if (dungeonData.HasDoorAtPosition(doorPositionForExit))
      {
        Debug.LogError((object) "Attempting to generate subdoor for position twice.");
      }
      else
      {
        DungeonData.Direction subsidiaryDoorDirection = this.GetSubsidiaryDoorDirection();
        IntVector2 location = doorPositionForExit - this.upstreamRoom.area.basePosition;
        GameObject gameObject = sourcePlaceable.InstantiateObjectDirectional(this.upstreamRoom, location, subsidiaryDoorDirection);
        gameObject.transform.parent = doorParentTransform;
        DungeonDoorController component = gameObject.GetComponent<DungeonDoorController>();
        component.exitDefinition = this;
        mainDoor.subsidiaryDoor = component;
        component.parentDoor = mainDoor;
      }
    }

    public void GenerateStandaloneRoomBlocker(DungeonData dungeonData, Transform parentTransform)
    {
      if ((Object) GameManager.Instance.Dungeon.phantomBlockerDoorObjects == (Object) null)
        return;
      RuntimeRoomExitData upstreamExit = this.upstreamExit;
      int TotalExitLength = upstreamExit.TotalExitLength + upstreamExit.linkedExit.TotalExitLength - 3;
      if (upstreamExit.referencedExit.exitDirection == DungeonData.Direction.SOUTH)
        --TotalExitLength;
      IntVector2 location = upstreamExit.referencedExit.GetExitOrigin(TotalExitLength) - IntVector2.One;
      IntVector2 position = location + this.upstreamRoom.area.basePosition;
      if (dungeonData.HasDoorAtPosition(position))
      {
        Debug.LogError((object) "Attempting to generate subdoor for position twice.");
      }
      else
      {
        DungeonData.Direction exitDirection = this.upstreamExit.referencedExit.exitDirection;
        GameObject gameObject = GameManager.Instance.Dungeon.phantomBlockerDoorObjects.InstantiateObjectDirectional(this.upstreamRoom, location, exitDirection);
        gameObject.transform.parent = parentTransform;
        DungeonDoorSubsidiaryBlocker component = gameObject.GetComponent<DungeonDoorSubsidiaryBlocker>();
        if (this.downstreamRoom != null)
          this.downstreamRoom.standaloneBlockers.Add(component);
        if (this.upstreamRoom == null)
          return;
        this.upstreamRoom.standaloneBlockers.Add(component);
      }
    }

    public void GenerateSecretRoomBlocker(
      DungeonData dungeonData,
      SecretRoomManager secretManager,
      SecretRoomDoorBeer secretDoor,
      Transform parentTransform)
    {
      if ((Object) GameManager.Instance.Dungeon.phantomBlockerDoorObjects == (Object) null)
        return;
      RuntimeRoomExitData runtimeRoomExitData = secretManager.room != this.upstreamRoom ? this.downstreamExit : this.upstreamExit;
      int TotalExitLength = runtimeRoomExitData.TotalExitLength + runtimeRoomExitData.linkedExit.TotalExitLength - 3;
      if (runtimeRoomExitData.referencedExit.exitDirection == DungeonData.Direction.SOUTH)
        --TotalExitLength;
      IntVector2 location = runtimeRoomExitData.referencedExit.GetExitOrigin(TotalExitLength) - IntVector2.One;
      IntVector2 position = location + secretManager.room.area.basePosition;
      if (dungeonData.HasDoorAtPosition(position))
      {
        Debug.LogError((object) "Attempting to generate subdoor for position twice.");
      }
      else
      {
        DungeonData.Direction direction = secretDoor.exitDef.upstreamRoom != secretManager.room ? secretDoor.exitDef.downstreamExit.referencedExit.exitDirection : secretDoor.exitDef.upstreamExit.referencedExit.exitDirection;
        GameObject gameObject = GameManager.Instance.Dungeon.phantomBlockerDoorObjects.InstantiateObjectDirectional(secretManager.room, location, direction);
        gameObject.transform.parent = parentTransform;
        DungeonDoorSubsidiaryBlocker component = gameObject.GetComponent<DungeonDoorSubsidiaryBlocker>();
        component.ToggleRenderers(false);
        secretDoor.subsidiaryBlocker = component;
      }
    }

    protected void GeneratePhantomDoorBlocker(
      DungeonData dungeonData,
      DungeonDoorController mainDoor,
      Transform doorParentTransform)
    {
      if ((Object) GameManager.Instance.Dungeon.phantomBlockerDoorObjects == (Object) null || GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.OFFICEGEON && mainDoor.OneWayDoor)
        return;
      IntVector2 doorPositionForExit = this.GetSubDoorPositionForExit(this.upstreamExit, this.upstreamRoom);
      if (dungeonData.HasDoorAtPosition(doorPositionForExit))
      {
        Debug.LogError((object) "Attempting to generate subdoor for position twice.");
      }
      else
      {
        DungeonData.Direction direction = !this.upstreamExit.jointedExit ? this.downstreamExit.referencedExit.exitDirection : (this.upstreamExit.referencedExit.exitDirection == DungeonData.Direction.EAST || this.upstreamExit.referencedExit.exitDirection == DungeonData.Direction.WEST ? this.upstreamExit.referencedExit.exitDirection : this.downstreamExit.referencedExit.exitDirection);
        GameObject gameObject = GameManager.Instance.Dungeon.phantomBlockerDoorObjects.InstantiateObjectDirectional(this.upstreamRoom, doorPositionForExit - this.upstreamRoom.area.basePosition, direction);
        gameObject.transform.parent = doorParentTransform;
        DungeonDoorSubsidiaryBlocker component = gameObject.GetComponent<DungeonDoorSubsidiaryBlocker>();
        mainDoor.subsidiaryBlocker = component;
        component.parentDoor = mainDoor;
      }
    }

    private void GenerateWarpWingPortals()
    {
      bool flag = false;
      if (GameManager.Instance.Dungeon.UsesWallWarpWingDoors && this.upstreamRoom != null && this.downstreamRoom != null)
      {
        if (this.m_upstreamCells != null)
          this.m_upstreamCells.Clear();
        if (this.m_downstreamCells != null)
          this.m_downstreamCells.Clear();
        if (this.IntermediaryCells != null)
          this.IntermediaryCells.Clear();
        int wallClearanceWidth = GameManager.Instance.Dungeon.WarpWingDoorPrefab.GetComponent<PlacedWallDecorator>().wallClearanceWidth;
        List<TK2DInteriorDecorator.WallExpanse> wallExpanseList1 = this.upstreamRoom.GatherExpanses(DungeonData.Direction.NORTH, disallowPits: true);
        List<TK2DInteriorDecorator.WallExpanse> wallExpanseList2 = this.downstreamRoom.GatherExpanses(DungeonData.Direction.NORTH, disallowPits: true);
        Debug.Log((object) $"{(object) wallExpanseList1.Count}|{(object) wallExpanseList2.Count}| req width: {(object) wallClearanceWidth}");
        for (int index = 0; index < wallExpanseList1.Count; ++index)
        {
          if (wallExpanseList1[index].width < wallClearanceWidth)
          {
            wallExpanseList1.RemoveAt(index);
            --index;
          }
        }
        for (int index = 0; index < wallExpanseList2.Count; ++index)
        {
          if (wallExpanseList2[index].width < wallClearanceWidth)
          {
            wallExpanseList2.RemoveAt(index);
            --index;
          }
        }
        Debug.Log((object) $"{(object) wallExpanseList1.Count}|{(object) wallExpanseList2.Count}| post cull");
        TK2DInteriorDecorator.WallExpanse? nullable1 = wallExpanseList1.Count <= 0 ? new TK2DInteriorDecorator.WallExpanse?() : new TK2DInteriorDecorator.WallExpanse?(wallExpanseList1[Random.Range(0, wallExpanseList1.Count)]);
        TK2DInteriorDecorator.WallExpanse? nullable2 = wallExpanseList2.Count <= 0 ? new TK2DInteriorDecorator.WallExpanse?() : new TK2DInteriorDecorator.WallExpanse?(wallExpanseList2[Random.Range(0, wallExpanseList2.Count)]);
        if (nullable1.HasValue && nullable2.HasValue)
        {
          GameObject warpWingDoorPrefab = GameManager.Instance.Dungeon.WarpWingDoorPrefab;
          int num1 = 0;
          if (nullable1.Value.width > wallClearanceWidth)
            num1 = Mathf.CeilToInt((float) ((double) nullable1.Value.width / 2.0 - (double) wallClearanceWidth / 2.0));
          int num2 = 0;
          if (nullable2.Value.width > wallClearanceWidth)
            num2 = Mathf.CeilToInt((float) ((double) nullable2.Value.width / 2.0 - (double) wallClearanceWidth / 2.0));
          Vector3 vector3_1 = nullable1.Value.basePosition.ToVector3() + Vector3.right * (float) num1 + Vector3.up;
          Vector3 vector3_2 = nullable2.Value.basePosition.ToVector3() + Vector3.right * (float) num2 + Vector3.up;
          WarpPointHandler component1 = Object.Instantiate<GameObject>(warpWingDoorPrefab, this.upstreamRoom.area.basePosition.ToVector3() + vector3_1 + warpWingDoorPrefab.transform.localPosition, Quaternion.identity).GetComponent<WarpPointHandler>();
          WarpPointHandler component2 = Object.Instantiate<GameObject>(warpWingDoorPrefab, this.downstreamRoom.area.basePosition.ToVector3() + vector3_2 + warpWingDoorPrefab.transform.localPosition, Quaternion.identity).GetComponent<WarpPointHandler>();
          PlacedWallDecorator component3 = component1.GetComponent<PlacedWallDecorator>();
          if ((bool) (Object) component3)
            component3.ConfigureOnPlacement(this.upstreamRoom);
          PlacedWallDecorator component4 = component2.GetComponent<PlacedWallDecorator>();
          if ((bool) (Object) component4)
            component4.ConfigureOnPlacement(this.downstreamRoom);
          component1.GetComponent<PlacedWallDecorator>().ConfigureOnPlacement(this.upstreamRoom);
          component2.GetComponent<PlacedWallDecorator>().ConfigureOnPlacement(this.downstreamRoom);
          component1.spawnOffset = new Vector2(0.0f, -0.25f);
          component2.spawnOffset = new Vector2(0.0f, -0.25f);
          component1.SetTarget(component2);
          component2.SetTarget(component1);
          flag = true;
        }
      }
      if (flag)
        return;
      GameObject original = (GameObject) BraveResources.Load("Global Prefabs/WarpWing_Portal");
      GameObject gameObject1 = Object.Instantiate<GameObject>(original);
      GameObject gameObject2 = Object.Instantiate<GameObject>(original);
      WarpWingPortalController component5 = gameObject1.GetComponent<WarpWingPortalController>();
      WarpWingPortalController component6 = gameObject2.GetComponent<WarpWingPortalController>();
      component5.pairedPortal = component6;
      component5.parentRoom = this.upstreamRoom;
      component5.parentExit = this.upstreamExit;
      component6.pairedPortal = component5;
      component6.parentRoom = this.downstreamRoom;
      component6.parentExit = this.downstreamExit;
      this.upstreamExit.warpWingPortal = component5;
      this.downstreamExit.warpWingPortal = component6;
      IntVector2 doorPositionForExit1 = this.GetDoorPositionForExit(this.upstreamExit, this.upstreamRoom, true);
      IntVector2 doorPositionForExit2 = this.GetDoorPositionForExit(this.downstreamExit, this.downstreamRoom, true);
      IntVector2 intVector2_1 = doorPositionForExit1 + DungeonData.GetIntVector2FromDirection(this.upstreamExit.referencedExit.exitDirection) * 3;
      IntVector2 intVector2_2 = doorPositionForExit2 + DungeonData.GetIntVector2FromDirection(this.downstreamExit.referencedExit.exitDirection) * 3;
      component5.transform.position = intVector2_1.ToVector3();
      component6.transform.position = intVector2_2.ToVector3();
      RoomHandler.unassignedInteractableObjects.Add((IPlayerInteractable) component5);
      RoomHandler.unassignedInteractableObjects.Add((IPlayerInteractable) component6);
    }

    public void GenerateDoorsForExit(DungeonData dungeonData, Transform doorParentTransform)
    {
      if (!GameManager.Instance.Dungeon.UsesWallWarpWingDoors && (this.upstreamExit != null && this.upstreamExit.isWarpWingStart || this.downstreamExit != null && this.downstreamExit.isWarpWingStart))
        this.GenerateWarpWingPortals();
      else if (this.upstreamExit != null && this.upstreamExit.isWarpWingStart || this.downstreamExit != null && this.downstreamExit.isWarpWingStart)
        return;
      bool flag = false;
      if (this.upstreamRoom != null && (Object) this.upstreamRoom.area.prototypeRoom != (Object) null && this.upstreamRoom.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.SECRET)
        flag = true;
      if (this.downstreamRoom != null && (Object) this.downstreamRoom.area.prototypeRoom != (Object) null && this.downstreamRoom.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.SECRET)
        flag = true;
      if (flag && (this.upstreamExit == null || !this.upstreamExit.oneWayDoor) && (this.downstreamExit == null || !this.downstreamExit.oneWayDoor))
        return;
      if (this.upstreamRoom.area.PrototypeLostWoodsRoom || this.downstreamRoom.area.PrototypeLostWoodsRoom)
      {
        this.GenerateStandaloneRoomBlocker(dungeonData, doorParentTransform);
      }
      else
      {
        IntVector2 doorPositionForExit = this.GetDoorPositionForExit(this.upstreamExit, this.upstreamRoom);
        if (dungeonData.HasDoorAtPosition(doorPositionForExit))
        {
          Debug.LogError((object) "Attempting to generate door for position twice.");
        }
        else
        {
          DungeonData.Direction direction = this.downstreamExit != null ? (this.upstreamExit != null ? (!this.upstreamExit.jointedExit ? this.upstreamExit.referencedExit.exitDirection : (this.upstreamExit.oneWayDoor || this.upstreamExit.referencedExit.exitDirection != DungeonData.Direction.EAST && this.upstreamExit.referencedExit.exitDirection != DungeonData.Direction.WEST ? this.upstreamExit.referencedExit.exitDirection : this.downstreamExit.referencedExit.exitDirection)) : this.downstreamExit.referencedExit.exitDirection) : this.upstreamExit.referencedExit.exitDirection;
          GameObject gameObject1;
          DungeonPlaceable dungeonPlaceable;
          if (this.upstreamExit != null && this.upstreamExit.oneWayDoor)
          {
            IntVector2 location = doorPositionForExit - this.upstreamRoom.area.basePosition;
            if (direction == DungeonData.Direction.EAST || direction == DungeonData.Direction.WEST)
              location += IntVector2.Down;
            gameObject1 = GameManager.Instance.Dungeon.oneWayDoorObjects.InstantiateObjectDirectional(this.upstreamRoom, location, direction);
            dungeonPlaceable = GameManager.Instance.Dungeon.oneWayDoorObjects;
          }
          else if (this.upstreamExit != null && this.upstreamExit.isLockedDoor && (Object) GameManager.Instance.Dungeon.lockedDoorObjects != (Object) null)
          {
            gameObject1 = GameManager.Instance.Dungeon.lockedDoorObjects.InstantiateObjectDirectional(this.upstreamRoom, doorPositionForExit - this.upstreamRoom.area.basePosition, direction);
            dungeonPlaceable = GameManager.Instance.Dungeon.lockedDoorObjects;
          }
          else if (this.downstreamExit != null && (Object) this.downstreamExit.referencedExit.specifiedDoor != (Object) null)
          {
            IntVector2 location = doorPositionForExit - this.downstreamRoom.area.basePosition;
            dungeonPlaceable = this.downstreamExit.referencedExit.specifiedDoor;
            if (dungeonPlaceable.variantTiers.Count > 0 && (Object) dungeonPlaceable.variantTiers[0].nonDatabasePlaceable != (Object) null)
            {
              DungeonDoorController component = dungeonPlaceable.variantTiers[0].nonDatabasePlaceable.GetComponent<DungeonDoorController>();
              if ((Object) component != (Object) null && component.Mode == DungeonDoorController.DungeonDoorMode.FINAL_BOSS_DOOR)
                location += IntVector2.Right;
            }
            gameObject1 = dungeonPlaceable.InstantiateObjectDirectional(this.downstreamRoom, location, direction);
          }
          else if (this.upstreamExit != null && (Object) this.upstreamExit.referencedExit.specifiedDoor != (Object) null)
          {
            IntVector2 location = doorPositionForExit - this.upstreamRoom.area.basePosition;
            dungeonPlaceable = this.upstreamExit.referencedExit.specifiedDoor;
            if (dungeonPlaceable.variantTiers.Count > 0 && (Object) dungeonPlaceable.variantTiers[0].nonDatabasePlaceable != (Object) null)
            {
              DungeonDoorController component = dungeonPlaceable.variantTiers[0].nonDatabasePlaceable.GetComponent<DungeonDoorController>();
              if ((Object) component != (Object) null && component.Mode == DungeonDoorController.DungeonDoorMode.FINAL_BOSS_DOOR)
                location += IntVector2.Right;
            }
            gameObject1 = dungeonPlaceable.InstantiateObjectDirectional(this.upstreamRoom, location, direction);
          }
          else
          {
            if (flag)
              return;
            IntVector2 location = doorPositionForExit - this.upstreamRoom.area.basePosition;
            Dungeon dungeon = GameManager.Instance.Dungeon;
            if ((Object) dungeon.alternateDoorObjectsNakatomi != (Object) null)
            {
              if (this.downstreamRoom != null && (this.downstreamRoom.RoomVisualSubtype == 7 || this.downstreamRoom.RoomVisualSubtype == 8 || this.upstreamRoom.RoomVisualSubtype == 7 || this.upstreamRoom.RoomVisualSubtype == 8))
              {
                gameObject1 = dungeon.alternateDoorObjectsNakatomi.InstantiateObjectDirectional(this.upstreamRoom, location, direction);
                dungeonPlaceable = dungeon.alternateDoorObjectsNakatomi;
              }
              else
              {
                gameObject1 = dungeon.doorObjects.InstantiateObjectDirectional(this.upstreamRoom, location, direction);
                dungeonPlaceable = dungeon.doorObjects;
              }
            }
            else
            {
              gameObject1 = dungeon.doorObjects.InstantiateObjectDirectional(this.upstreamRoom, location, direction);
              dungeonPlaceable = dungeon.doorObjects;
            }
          }
          if ((Object) dungeonPlaceable == (Object) null)
            return;
          gameObject1.transform.parent = doorParentTransform;
          DungeonDoorController component1 = gameObject1.GetComponent<DungeonDoorController>();
          if ((Object) dungeonPlaceable == (Object) GameManager.Instance.Dungeon.lockedDoorObjects && this.upstreamExit.referencedExit.exitDirection == DungeonData.Direction.EAST)
            component1.FlipLockToOtherSide();
          if (this.downstreamExit != null && this.downstreamExit.oneWayDoor || this.upstreamExit != null && this.upstreamExit.oneWayDoor)
          {
            component1.OneWayDoor = true;
            GameObject gameObject2 = Object.Instantiate<GameObject>(GameManager.Instance.Dungeon.oneWayDoorPressurePlate);
            Vector3 vector3 = Vector3.zero;
            if (direction == DungeonData.Direction.WEST || direction == DungeonData.Direction.EAST)
              vector3 = Vector3.up;
            gameObject2.transform.position = component1.transform.position + (DungeonData.GetIntVector2FromDirection(direction) * 2).ToVector3() + vector3;
            PressurePlate component2 = gameObject2.GetComponent<PressurePlate>();
            component1.AssignPressurePlate(component2);
          }
          foreach (IntVector2 upstreamCell in this.m_upstreamCells)
            dungeonData[upstreamCell].exitDoor = component1;
          foreach (IntVector2 downstreamCell in this.m_downstreamCells)
            dungeonData[downstreamCell].exitDoor = component1;
          component1.upstreamRoom = this.upstreamRoom;
          component1.downstreamRoom = this.downstreamRoom;
          component1.exitDefinition = this;
          this.upstreamRoom.connectedDoors.Add(component1);
          this.downstreamRoom.connectedDoors.Add(component1);
          IntVector2 intVector2 = IntVector2.Zero;
          if (component1.Mode == DungeonDoorController.DungeonDoorMode.ONE_WAY_DOOR_ONLY_UNSEALS)
            this.GeneratePhantomDoorBlocker(dungeonData, component1, doorParentTransform);
          else if (this.RequiresSubDoor() && !flag)
          {
            DungeonPlaceable sourcePlaceable = !((Object) dungeonPlaceable == (Object) GameManager.Instance.Dungeon.lockedDoorObjects) ? dungeonPlaceable : GameManager.Instance.Dungeon.doorObjects;
            intVector2 = this.GetSubDoorPositionForExit(this.upstreamExit, this.upstreamRoom);
            if (component1.SupportsSubsidiaryDoors)
            {
              this.GenerateSubsidiaryDoor(dungeonData, sourcePlaceable, component1, doorParentTransform);
            }
            else
            {
              DungeonData.Direction subsidiaryDoorDirection = this.GetSubsidiaryDoorDirection();
              bool northSouth = subsidiaryDoorDirection == DungeonData.Direction.NORTH || subsidiaryDoorDirection == DungeonData.Direction.SOUTH;
              dungeonData.FakeRegisterDoorFeet(intVector2, northSouth);
            }
          }
          dungeonData.RegisterDoor(doorPositionForExit, component1, intVector2);
          this.linkedDoor = component1;
        }
      }
    }

    protected bool RequiresSubDoor()
    {
      return this.upstreamExit != null && this.downstreamExit != null && (this.upstreamExit.jointedExit || this.upstreamExit.TotalExitLength + this.downstreamExit.TotalExitLength > 7);
    }

    protected IntVector2 GetSubDoorPositionForExit(RuntimeRoomExitData exit, RoomHandler owner)
    {
      return exit.jointedExit ? (!exit.oneWayDoor && (exit.linkedExit == null || exit.referencedExit.exitDirection == DungeonData.Direction.EAST || exit.referencedExit.exitDirection == DungeonData.Direction.WEST) ? exit.referencedExit.GetExitAttachPoint() - IntVector2.One + DungeonData.GetIntVector2FromDirection(exit.referencedExit.exitDirection) + owner.area.basePosition : exit.linkedExit.referencedExit.GetExitAttachPoint() - IntVector2.One + DungeonData.GetIntVector2FromDirection(exit.linkedExit.referencedExit.exitDirection) + owner.connectedRoomsByExit[exit.referencedExit].area.basePosition) : (exit.linkedExit != null && exit.TotalExitLength + exit.linkedExit.TotalExitLength > 7 ? exit.linkedExit.referencedExit.GetExitAttachPoint() - IntVector2.One + DungeonData.GetIntVector2FromDirection(exit.linkedExit.referencedExit.exitDirection) + owner.connectedRoomsByExit[exit.referencedExit].area.basePosition : IntVector2.MaxValue);
    }

    protected IntVector2 GetDoorPositionForExit(
      RuntimeRoomExitData exit,
      RoomHandler owner,
      bool overrideSecretRoomHandling = false)
    {
      if (exit == null)
        Debug.LogError((object) ("THIS EXIT ISN'T REAL. IT ISNT REAAAALLLLLLL: " + owner.GetRoomName()));
      RoomHandler roomHandler = owner.connectedRoomsByExit[exit.referencedExit];
      if (!overrideSecretRoomHandling)
      {
        if (owner != null && !owner.area.IsProceduralRoom && owner.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.SECRET && exit.linkedExit != null && exit.linkedExit.referencedExit != null)
          return exit.linkedExit.referencedExit.GetExitAttachPoint() - IntVector2.One + DungeonData.GetIntVector2FromDirection(exit.linkedExit.referencedExit.exitDirection) + owner.connectedRoomsByExit[exit.referencedExit].area.basePosition;
        if (roomHandler != null && !roomHandler.area.IsProceduralRoom && roomHandler.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.SECRET && exit.linkedExit != null && exit.linkedExit.referencedExit != null)
          return exit.referencedExit.GetExitAttachPoint() - IntVector2.One + DungeonData.GetIntVector2FromDirection(exit.referencedExit.exitDirection) + owner.area.basePosition;
      }
      return !exit.oneWayDoor && exit.jointedExit && exit.linkedExit != null && (exit.referencedExit.exitDirection == DungeonData.Direction.EAST || exit.referencedExit.exitDirection == DungeonData.Direction.WEST) ? exit.linkedExit.referencedExit.GetExitAttachPoint() - IntVector2.One + DungeonData.GetIntVector2FromDirection(exit.linkedExit.referencedExit.exitDirection) + owner.connectedRoomsByExit[exit.referencedExit].area.basePosition : exit.referencedExit.GetExitAttachPoint() - IntVector2.One + DungeonData.GetIntVector2FromDirection(exit.referencedExit.exitDirection) + owner.area.basePosition;
    }
  }
}
