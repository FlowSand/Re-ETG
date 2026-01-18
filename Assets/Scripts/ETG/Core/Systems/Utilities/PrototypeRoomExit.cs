using System;
using System.Collections.Generic;

using UnityEngine;

using Dungeonator;

#nullable disable

[Serializable]
public class PrototypeRoomExit
    {
        [SerializeField]
        public DungeonData.Direction exitDirection;
        [SerializeField]
        public PrototypeRoomExit.ExitType exitType;
        [SerializeField]
        public PrototypeRoomExit.ExitGroup exitGroup;
        [SerializeField]
        public bool containsDoor = true;
        [SerializeField]
        public DungeonPlaceable specifiedDoor;
        [SerializeField]
        public int exitLength = 1;
        [SerializeField]
        public List<Vector2> containedCells;

        public PrototypeRoomExit(DungeonData.Direction d, Vector2 pos)
        {
            this.exitDirection = d;
            this.containedCells = new List<Vector2>();
            this.containedCells.Add(pos);
        }

        public IntVector2 GetHalfExitAttachPoint(int TotalExitLength)
        {
            Vector2 vector = new Vector2(float.MaxValue, float.MaxValue);
            for (int index = 0; index < this.containedCells.Count; ++index)
            {
                Vector2 containedCell = this.containedCells[index];
                if (this.exitDirection == DungeonData.Direction.EAST || this.exitDirection == DungeonData.Direction.WEST)
                {
                    if ((double) containedCell.y < (double) vector.y)
                        vector = containedCell;
                }
                else if ((double) containedCell.x < (double) vector.x)
                    vector = containedCell;
            }
            IntVector2 intVector2 = vector.ToIntVector2();
            if (this.exitDirection == DungeonData.Direction.SOUTH)
                intVector2 += IntVector2.Down;
            if (this.exitLength <= 2)
                this.exitLength = 3;
            return intVector2 + DungeonData.GetIntVector2FromDirection(this.exitDirection) * 2;
        }

        public IntVector2 GetExitAttachPoint()
        {
            Vector2 vector = new Vector2(float.MaxValue, float.MaxValue);
            for (int index = 0; index < this.containedCells.Count; ++index)
            {
                Vector2 containedCell = this.containedCells[index];
                if (this.exitDirection == DungeonData.Direction.EAST || this.exitDirection == DungeonData.Direction.WEST)
                {
                    if ((double) containedCell.y < (double) vector.y)
                        vector = containedCell;
                }
                else if ((double) containedCell.x < (double) vector.x)
                    vector = containedCell;
            }
            IntVector2 intVector2 = vector.ToIntVector2();
            if (this.exitDirection != DungeonData.Direction.SOUTH)
                ;
            return intVector2;
        }

        public IntVector2 GetExitOrigin(int TotalExitLength)
        {
            Vector2 vector = new Vector2(float.MaxValue, float.MaxValue);
            for (int index = 0; index < this.containedCells.Count; ++index)
            {
                Vector2 containedCell = this.containedCells[index];
                if (this.exitDirection == DungeonData.Direction.EAST || this.exitDirection == DungeonData.Direction.WEST)
                {
                    if ((double) containedCell.y < (double) vector.y)
                        vector = containedCell;
                }
                else if ((double) containedCell.x < (double) vector.x)
                    vector = containedCell;
            }
            IntVector2 intVector2 = vector.ToIntVector2();
            if (this.exitDirection == DungeonData.Direction.SOUTH)
                intVector2 += IntVector2.Down;
            if (TotalExitLength <= 2)
            {
                this.exitLength = 3;
                TotalExitLength = 3;
            }
            return intVector2 + DungeonData.GetIntVector2FromDirection(this.exitDirection) * (TotalExitLength - 1);
        }

        public int ExitCellCount => this.containedCells.Count;

        public static PrototypeRoomExit CreateMirror(
            PrototypeRoomExit source,
            IntVector2 sourceRoomDimensions)
        {
            PrototypeRoomExit mirror = new PrototypeRoomExit(source.exitDirection, Vector2.zero);
            mirror.containedCells.Clear();
            switch (source.exitDirection)
            {
                case DungeonData.Direction.NORTH:
                    mirror.exitDirection = DungeonData.Direction.NORTH;
                    break;
                case DungeonData.Direction.EAST:
                    mirror.exitDirection = DungeonData.Direction.WEST;
                    break;
                case DungeonData.Direction.SOUTH:
                    mirror.exitDirection = DungeonData.Direction.SOUTH;
                    break;
                case DungeonData.Direction.WEST:
                    mirror.exitDirection = DungeonData.Direction.EAST;
                    break;
                default:
                    mirror.exitDirection = source.exitDirection;
                    break;
            }
            mirror.exitType = source.exitType;
            mirror.exitGroup = source.exitGroup;
            mirror.containsDoor = source.containsDoor;
            mirror.specifiedDoor = source.specifiedDoor;
            mirror.exitLength = source.exitLength;
            for (int index = 0; index < source.containedCells.Count; ++index)
            {
                Vector2 containedCell = source.containedCells[index];
                containedCell.x = (float) (sourceRoomDimensions.x + 2) - (containedCell.x + 1f);
                mirror.containedCells.Add(containedCell);
            }
            return mirror;
        }

        public enum ExitType
        {
            NO_RESTRICTION,
            ENTRANCE_ONLY,
            EXIT_ONLY,
        }

        public enum ExitGroup
        {
            A,
            B,
            C,
            D,
            E,
            F,
            G,
            H,
        }
    }

