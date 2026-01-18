using System;
using System.Collections.Generic;

using UnityEngine;

#nullable disable

[Serializable]
public class PrototypeRoomPitEntry
    {
        public List<Vector2> containedCells;
        public PrototypeRoomPitEntry.PitBorderType borderType;

        public PrototypeRoomPitEntry(IEnumerable<Vector2> cells)
        {
            this.containedCells = new List<Vector2>(cells);
        }

        public PrototypeRoomPitEntry(Vector2 cell)
        {
            this.containedCells = new List<Vector2>();
            this.containedCells.Add(cell);
        }

        public PrototypeRoomPitEntry CreateMirror(IntVector2 roomDimensions)
        {
            PrototypeRoomPitEntry mirror = new PrototypeRoomPitEntry(Vector2.zero);
            mirror.containedCells.Clear();
            mirror.borderType = this.borderType;
            for (int index = 0; index < this.containedCells.Count; ++index)
            {
                Vector2 containedCell = this.containedCells[index];
                containedCell.x = (float) roomDimensions.x - (containedCell.x + 1f);
                mirror.containedCells.Add(containedCell);
            }
            return mirror;
        }

        public bool IsAdjoining(Vector2 cell)
        {
            foreach (Vector2 containedCell in this.containedCells)
            {
                if (Mathf.Approximately(Vector2.Distance(cell, containedCell), 1f))
                    return true;
            }
            return false;
        }

        public bool IsAdjoining(IEnumerable<Vector2> cells)
        {
            foreach (Vector2 cell in cells)
            {
                foreach (Vector2 containedCell in this.containedCells)
                {
                    if (Mathf.Approximately(Vector2.Distance(cell, containedCell), 1f))
                        return true;
                }
            }
            return false;
        }

        public void AddCells(IEnumerable<Vector2> cells)
        {
            foreach (Vector2 cell in cells)
            {
                if (!this.containedCells.Contains(cell))
                    this.containedCells.Add(cell);
            }
        }

        public enum PitBorderType
        {
            FLAT,
            RAISED,
            NONE,
        }
    }

