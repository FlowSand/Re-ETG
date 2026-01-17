// Decompiled with JetBrains decompiler
// Type: PrototypeRoomExitData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Data
{
    [Serializable]
    public class PrototypeRoomExitData
    {
      [SerializeField]
      public List<PrototypeRoomExit> exits;

      public void MirrorData(PrototypeRoomExitData source, IntVector2 sourceDimensions)
      {
        this.exits = new List<PrototypeRoomExit>();
        for (int index = 0; index < source.exits.Count; ++index)
          this.exits.Add(PrototypeRoomExit.CreateMirror(source.exits[index], sourceDimensions));
      }

      public bool HasDefinedExitGroups() => this.GetDefinedExitGroups().Count > 1;

      public List<PrototypeRoomExit.ExitGroup> GetDefinedExitGroups()
      {
        List<PrototypeRoomExit.ExitGroup> definedExitGroups = new List<PrototypeRoomExit.ExitGroup>();
        for (int index = 0; index < this.exits.Count; ++index)
        {
          if (!definedExitGroups.Contains(this.exits[index].exitGroup))
            definedExitGroups.Add(this.exits[index].exitGroup);
        }
        return definedExitGroups;
      }

      public bool ProcessExitPosition(int ix, int iy, PrototypeDungeonRoom parent)
      {
        if (this[ix + 1, iy + 1] != null)
          return this.RemoveExitPosition(ix, iy, parent);
        this.AddExitPosition(ix, iy, parent);
        return true;
      }

      public void AddExitPosition(int ix, int iy, PrototypeDungeonRoom parent)
      {
        IntVector2 intVector2 = new IntVector2(ix, iy);
        HashSet<IntVector2> cellRepresentation = this.ExitToCellRepresentation();
        cellRepresentation.Add(intVector2 + IntVector2.One);
        this.exits = this.CellToExitRepresentation(cellRepresentation, parent);
        foreach (PrototypeDungeonRoomCellData dungeonRoomCellData in parent.FullCellData)
        {
          dungeonRoomCellData.conditionalOnParentExit = false;
          dungeonRoomCellData.parentExitIndex = -1;
        }
      }

      public bool RemoveExitPosition(int ix, int iy, PrototypeDungeonRoom parent)
      {
        IntVector2 intVector2 = new IntVector2(ix + 1, iy + 1);
        HashSet<IntVector2> cellRepresentation = this.ExitToCellRepresentation();
        bool flag = cellRepresentation.Remove(intVector2);
        this.exits = this.CellToExitRepresentation(cellRepresentation, parent);
        foreach (PrototypeDungeonRoomCellData dungeonRoomCellData in parent.FullCellData)
        {
          dungeonRoomCellData.conditionalOnParentExit = false;
          dungeonRoomCellData.parentExitIndex = -1;
        }
        return flag;
      }

      public void TranslateAllExits(int xOffset, int yOffset, PrototypeDungeonRoom parent)
      {
        IntVector2 intVector2 = new IntVector2(xOffset, yOffset);
        List<IntVector2> collection = new List<IntVector2>((IEnumerable<IntVector2>) this.ExitToCellRepresentation());
        for (int index = 0; index < collection.Count; ++index)
          collection[index] += intVector2;
        this.exits = this.CellToExitRepresentation(new HashSet<IntVector2>((IEnumerable<IntVector2>) collection), parent);
      }

      public void HandleRowColumnShift(
        int rowXCoord,
        int xShift,
        int columnYCoord,
        int yShift,
        PrototypeDungeonRoom parent)
      {
        IntVector2 intVector2_1 = new IntVector2(xShift, yShift);
        HashSet<IntVector2> cellRepresentation = this.ExitToCellRepresentation();
        if (cellRepresentation.Count == 0)
          return;
        List<IntVector2> collection = new List<IntVector2>((IEnumerable<IntVector2>) cellRepresentation);
        for (int index = collection.Count - 1; index >= 0; --index)
        {
          IntVector2 intVector2_2 = collection[index];
          if (intVector2_2.x > rowXCoord && intVector2_2.y > columnYCoord)
            collection[index] = intVector2_2 + intVector2_1;
          else if (intVector2_2.x == rowXCoord || intVector2_2.y == columnYCoord)
          {
            if (xShift == -1 || yShift == -1)
              collection.RemoveAt(index);
            else
              collection[index] = intVector2_2 + intVector2_1;
          }
        }
        this.exits = this.CellToExitRepresentation(new HashSet<IntVector2>((IEnumerable<IntVector2>) collection), parent);
      }

      private List<PrototypeRoomExit> CellToExitRepresentation(
        HashSet<IntVector2> cells,
        PrototypeDungeonRoom parent)
      {
        int length1 = parent.Width + 2;
        int length2 = parent.Height + 2;
        bool[,] exitMatrix = new bool[length1, length2];
        foreach (IntVector2 cell in cells)
          exitMatrix[cell.x, cell.y] = true;
        HashSet<IntVector2> closedSet = new HashSet<IntVector2>();
        List<PrototypeRoomExit> exitRepresentation = new List<PrototypeRoomExit>();
        for (int x = 0; x < length1; ++x)
        {
          for (int y = 0; y < length2; ++y)
          {
            IntVector2 intVector2 = new IntVector2(x, y);
            if (!closedSet.Contains(intVector2))
            {
              closedSet.Add(intVector2);
              if (exitMatrix[x, y])
              {
                DungeonData.Direction floorDirection = parent.GetFloorDirection(x - 1, y - 1);
                if (floorDirection == DungeonData.Direction.SOUTHWEST)
                {
                  Debug.LogError((object) "An exit was defined with no nearby floor tile. This is unsupported behavior.");
                }
                else
                {
                  PrototypeRoomExit prototypeRoomExit = new PrototypeRoomExit((DungeonData.Direction) ((int) (floorDirection + 4) % 8), intVector2.ToVector2());
                  this.RecurseFindExits(exitMatrix, intVector2 + IntVector2.Up, closedSet, prototypeRoomExit);
                  this.RecurseFindExits(exitMatrix, intVector2 + IntVector2.Right, closedSet, prototypeRoomExit);
                  this.RecurseFindExits(exitMatrix, intVector2 + IntVector2.Down, closedSet, prototypeRoomExit);
                  this.RecurseFindExits(exitMatrix, intVector2 + IntVector2.Left, closedSet, prototypeRoomExit);
                  PrototypeRoomExit previouslyDefinedExit = this.FindPreviouslyDefinedExit(prototypeRoomExit);
                  if (previouslyDefinedExit != null)
                  {
                    prototypeRoomExit.exitGroup = previouslyDefinedExit.exitGroup;
                    prototypeRoomExit.exitType = previouslyDefinedExit.exitType;
                    prototypeRoomExit.containsDoor = previouslyDefinedExit.containsDoor;
                    prototypeRoomExit.specifiedDoor = previouslyDefinedExit.specifiedDoor;
                  }
                  exitRepresentation.Add(prototypeRoomExit);
                }
              }
            }
          }
        }
        return exitRepresentation;
      }

      private PrototypeRoomExit FindPreviouslyDefinedExit(PrototypeRoomExit newExit)
      {
        for (int index1 = 0; index1 < newExit.containedCells.Count; ++index1)
        {
          for (int index2 = 0; index2 < this.exits.Count; ++index2)
          {
            if (this.exits[index2].containedCells.Contains(newExit.containedCells[index1]))
              return this.exits[index2];
          }
        }
        return (PrototypeRoomExit) null;
      }

      private void RecurseFindExits(
        bool[,] exitMatrix,
        IntVector2 coords,
        HashSet<IntVector2> closedSet,
        PrototypeRoomExit currentExit)
      {
        if (coords.x < 0 || coords.y < 0 || coords.x >= exitMatrix.GetLength(0) || coords.y >= exitMatrix.GetLength(1) || closedSet.Contains(coords))
          return;
        if (exitMatrix[coords.x, coords.y])
        {
          currentExit.containedCells.Add(coords.ToVector2());
          closedSet.Add(coords);
          this.RecurseFindExits(exitMatrix, coords + IntVector2.Up, closedSet, currentExit);
          this.RecurseFindExits(exitMatrix, coords + IntVector2.Right, closedSet, currentExit);
          this.RecurseFindExits(exitMatrix, coords + IntVector2.Down, closedSet, currentExit);
          this.RecurseFindExits(exitMatrix, coords + IntVector2.Left, closedSet, currentExit);
        }
        else
          closedSet.Add(coords);
      }

      private HashSet<IntVector2> ExitToCellRepresentation()
      {
        HashSet<IntVector2> cellRepresentation = new HashSet<IntVector2>();
        for (int index1 = 0; index1 < this.exits.Count; ++index1)
        {
          PrototypeRoomExit exit = this.exits[index1];
          for (int index2 = 0; index2 < exit.containedCells.Count; ++index2)
          {
            Vector2 containedCell = exit.containedCells[index2];
            IntVector2 intVector2 = new IntVector2(Mathf.RoundToInt(containedCell.x), Mathf.RoundToInt(containedCell.y));
            cellRepresentation.Add(intVector2);
          }
        }
        return cellRepresentation;
      }

      public PrototypeRoomExit this[int xOffset, int yOffset]
      {
        get
        {
          if (this.exits == null)
            return (PrototypeRoomExit) null;
          IntVector2 intVector2 = new IntVector2(xOffset, yOffset);
          for (int index = this.exits.Count - 1; index >= 0; --index)
          {
            PrototypeRoomExit exit = this.exits[index];
            if (exit == null || exit.containedCells == null)
              this.exits.RemoveAt(index);
            else if (exit.containedCells.Contains(intVector2.ToVector2()))
              return exit;
          }
          return (PrototypeRoomExit) null;
        }
      }

      public List<PrototypeRoomExit> GetUnusedExitsFromInstance(CellArea instance)
      {
        List<PrototypeRoomExit> exitsFromInstance = new List<PrototypeRoomExit>();
        for (int index = 0; index < this.exits.Count; ++index)
        {
          if (this.exits[index].exitType != PrototypeRoomExit.ExitType.ENTRANCE_ONLY && !instance.instanceUsedExits.Contains(this.exits[index]))
            exitsFromInstance.Add(this.exits[index]);
        }
        return exitsFromInstance;
      }

      public List<PrototypeRoomExit> GetUnusedExitsOnSide(DungeonData.Direction exitDir)
      {
        List<PrototypeRoomExit> unusedExitsOnSide = new List<PrototypeRoomExit>();
        for (int index = 0; index < this.exits.Count; ++index)
        {
          if (this.exits[index].exitDirection == exitDir)
            unusedExitsOnSide.Add(this.exits[index]);
        }
        return unusedExitsOnSide;
      }
    }

}
