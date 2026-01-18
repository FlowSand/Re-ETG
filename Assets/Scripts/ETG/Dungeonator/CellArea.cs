using System;
using System.Collections.Generic;

using UnityEngine;

#nullable disable
namespace Dungeonator
{
    public class CellArea
    {
        public IntVector2 basePosition;
        public IntVector2 dimensions;
        public Vector2 weightedOverlapMovementVector;
        public int variableBorderSizeX;
        public int variableBorderSizeY;
        public bool IsProceduralRoom = true;
        public string PrototypeRoomName;
        public PrototypeDungeonRoom.RoomCategory PrototypeRoomCategory = PrototypeDungeonRoom.RoomCategory.NORMAL;
        public PrototypeDungeonRoom.RoomNormalSubCategory PrototypeRoomNormalSubcategory;
        public PrototypeDungeonRoom.RoomSpecialSubCategory PrototypeRoomSpecialSubcategory;
        public PrototypeDungeonRoom.RoomBossSubCategory PrototypeRoomBossSubcategory;
        public bool PrototypeLostWoodsRoom;
        public RuntimePrototypeRoomData runtimePrototypeData;
        private PrototypeDungeonRoom m_prototypeRoom;
        public List<PrototypeRoomExit> instanceUsedExits;
        public Dictionary<PrototypeRoomExit, RuntimeRoomExitData> exitToLocalDataMap;
        public List<IntVector2> proceduralCells;
        private int borderDistance;

        public CellArea(IntVector2 p, IntVector2 d, int borderOffset = 0)
        {
            this.basePosition = p;
            this.dimensions = d;
            this.borderDistance = borderOffset;
            this.instanceUsedExits = new List<PrototypeRoomExit>();
            this.exitToLocalDataMap = new Dictionary<PrototypeRoomExit, RuntimeRoomExitData>();
        }

        public PrototypeDungeonRoom prototypeRoom
        {
            get => this.m_prototypeRoom;
            set
            {
                this.IsProceduralRoom = (UnityEngine.Object) value == (UnityEngine.Object) null && this.IsProceduralRoom;
                this.PrototypeRoomName = !((UnityEngine.Object) value == (UnityEngine.Object) null) ? value.name : this.PrototypeRoomName;
                this.PrototypeRoomCategory = !((UnityEngine.Object) value == (UnityEngine.Object) null) ? value.category : this.PrototypeRoomCategory;
                this.PrototypeRoomNormalSubcategory = !((UnityEngine.Object) value == (UnityEngine.Object) null) ? value.subCategoryNormal : this.PrototypeRoomNormalSubcategory;
                this.PrototypeRoomSpecialSubcategory = !((UnityEngine.Object) value == (UnityEngine.Object) null) ? value.subCategorySpecial : this.PrototypeRoomSpecialSubcategory;
                this.PrototypeRoomBossSubcategory = !((UnityEngine.Object) value == (UnityEngine.Object) null) ? value.subCategoryBoss : this.PrototypeRoomBossSubcategory;
                this.PrototypeLostWoodsRoom = !((UnityEngine.Object) value == (UnityEngine.Object) null) ? value.IsLostWoodsRoom : this.PrototypeLostWoodsRoom;
                this.m_prototypeRoom = value;
            }
        }

        public Vector2 UnitBottomLeft => this.basePosition.ToVector2();

        public Vector2 UnitCenter => this.basePosition.ToVector2() + this.dimensions.ToVector2() * 0.5f;

        public Vector2 UnitTopRight => (this.basePosition + this.dimensions).ToVector2();

        public float UnitLeft => (float) this.basePosition.x;

        public float UnitRight => (float) (this.basePosition.x + this.dimensions.x);

        public float UnitBottom => (float) this.basePosition.y;

        public float UnitTop => (float) (this.basePosition.y + this.dimensions.y);

        public Vector2 Center
        {
            get
            {
                return new Vector2((float) this.basePosition.x + (float) this.dimensions.x / 2f, (float) this.basePosition.y + (float) this.dimensions.y / 2f);
            }
        }

        public IntVector2 IntCenter => new IntVector2((int) this.Center.x, (int) this.Center.y);

        public bool Overlaps(CellArea other)
        {
            IntVector2 intVector2_1 = this.basePosition + this.dimensions;
            IntVector2 intVector2_2 = other.basePosition + other.dimensions;
            return this.basePosition.x < intVector2_2.x && intVector2_1.x > other.basePosition.x && this.basePosition.y < intVector2_2.y && intVector2_1.y > other.basePosition.y;
        }

        public bool OverlapsWithUnitBorder(CellArea other)
        {
            IntVector2 intVector2_1 = this.basePosition + IntVector2.NegOne;
            IntVector2 intVector2_2 = this.basePosition + this.dimensions + IntVector2.One * 2;
            IntVector2 intVector2_3 = other.basePosition + IntVector2.NegOne;
            IntVector2 intVector2_4 = other.basePosition + other.dimensions + IntVector2.One * 2;
            return intVector2_1.x < intVector2_4.x && intVector2_2.x > intVector2_3.x && intVector2_1.y < intVector2_4.y && intVector2_2.y > intVector2_3.y;
        }

        public bool ContainsWithUnitBorder(IntVector2 point)
        {
            return point.x >= this.basePosition.x - 1 && point.x <= this.basePosition.x + 1 && point.y >= this.basePosition.y - 1 && point.y <= this.basePosition.y + 1;
        }

        public bool Contains(IntVector2 point)
        {
            return point.x >= this.basePosition.x + this.borderDistance && point.x <= this.basePosition.x + this.dimensions.x - this.borderDistance && point.y >= this.basePosition.y + this.borderDistance && point.y <= this.basePosition.y + this.dimensions.y - this.borderDistance;
        }

        public bool CellOnBorder(IntVector2 pos)
        {
            bool flag = false;
            if (pos.x < this.basePosition.x + 1 && this.Contains(pos) || pos.x >= this.basePosition.x + this.dimensions.x - 1 && this.Contains(pos) || pos.y < this.basePosition.y + 1 && this.Contains(pos) || pos.y >= this.basePosition.y + this.dimensions.y - 1 && this.Contains(pos))
                flag = true;
            return flag;
        }

        public int CheckSharedEdge(
            CellArea other,
            int lengthOfSharedEdge,
            out IntVector2 position,
            out DungeonData.Direction dir)
        {
            int x = Math.Max(this.basePosition.x, other.basePosition.x);
            int num1 = Math.Min(this.basePosition.x + this.dimensions.x, other.basePosition.x + other.dimensions.x) - x;
            if (num1 >= lengthOfSharedEdge)
            {
                if (other.basePosition.y > this.basePosition.y)
                {
                    dir = DungeonData.Direction.NORTH;
                    position = new IntVector2(x, this.basePosition.y + this.dimensions.y);
                }
                else
                {
                    dir = DungeonData.Direction.SOUTH;
                    position = new IntVector2(x, this.basePosition.y);
                }
                return num1;
            }
            int y = Math.Max(this.basePosition.y, other.basePosition.y);
            int num2 = Math.Min(this.basePosition.y + this.dimensions.y, other.basePosition.y + other.dimensions.y) - y;
            if (num2 >= lengthOfSharedEdge)
            {
                if (other.basePosition.x > this.basePosition.x)
                {
                    dir = DungeonData.Direction.EAST;
                    position = new IntVector2(this.basePosition.x + this.dimensions.x, y);
                }
                else
                {
                    dir = DungeonData.Direction.WEST;
                    position = new IntVector2(this.basePosition.x, y);
                }
                return num2;
            }
            dir = DungeonData.Direction.NORTHWEST;
            position = IntVector2.Zero;
            return -1;
        }

        public bool LineIntersect(IntVector2 p1, IntVector2 p2)
        {
            if (this.LineIntersectsLine(p1, p2, this.basePosition, this.basePosition + new IntVector2(0, this.dimensions.y)) || this.LineIntersectsLine(p1, p2, this.basePosition, this.basePosition + new IntVector2(this.dimensions.x, 0)) || this.LineIntersectsLine(p1, p2, this.basePosition + new IntVector2(0, this.dimensions.y), this.basePosition + this.dimensions) || this.LineIntersectsLine(p1, p2, this.basePosition + new IntVector2(this.dimensions.x, 0), this.basePosition + this.dimensions))
                return true;
            return this.Contains(p1) && this.Contains(p2);
        }

        private bool LineIntersectsLine(
            IntVector2 l1p1,
            IntVector2 l1p2,
            IntVector2 l2p1,
            IntVector2 l2p2)
        {
            float num1 = (float) ((l1p1.Y - l2p1.Y) * (l2p2.X - l2p1.X) - (l1p1.X - l2p1.X) * (l2p2.Y - l2p1.Y));
            float num2 = (float) ((l1p2.X - l1p1.X) * (l2p2.Y - l2p1.Y) - (l1p2.Y - l1p1.Y) * (l2p2.X - l2p1.X));
            if ((double) num2 == 0.0)
                return false;
            float num3 = num1 / num2;
            float num4 = (float) ((l1p1.Y - l2p1.Y) * (l1p2.X - l1p1.X) - (l1p1.X - l2p1.X) * (l1p2.Y - l1p1.Y)) / num2;
            return (double) num3 >= 0.0 && (double) num3 <= 1.0 && (double) num4 >= 0.0 && (double) num4 <= 1.0;
        }
    }
}
