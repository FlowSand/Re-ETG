using System;
using System.Collections.Generic;

#nullable disable
namespace Dungeonator
{
    public class CellData
    {
        public IntVector2 position;
        public CellType type;
        public DiagonalWallType diagonalWallType;
        public IntVector2 positionInTilemap;
        public bool breakable;
        public bool fallingPrevented;
        public List<SpeculativeRigidbody> platforms;
        public bool containsTrap;
        public bool forceAllowGoop;
        public bool forceDisallowGoop;
        public bool isWallMimicHideout;
        public bool doesDamage;
        public CellDamageDefinition damageDefinition;
        public bool isExitCell;
        public DungeonData.Direction exitDirection;
        public bool isDoorFrameCell;
        public bool isExitNonOccluder;
        public DungeonDoorController exitDoor;
        public RoomHandler connectedRoom1;
        public RoomHandler connectedRoom2;
        public bool isSecretRoomCell;
        public RoomHandler nearestRoom;
        public float distanceFromNearestRoom = float.MaxValue;
        public CellVisualData cellVisualData;
        public bool isOccupied;
        public bool isOccludedByTopWall;
        private bool? m_isNextToWall;
        public CellOcclusionData occlusionData;
        public CellArea parentArea;
        public RoomHandler parentRoom;
        [NonSerialized]
        public RoomHandler targetPitfallRoom;
        public bool hasBeenGenerated;
        public bool isRoomInternal = true;
        public bool isGridConnected;
        public float lastSplashTime = -1f;
        public bool IsFireplaceCell;
        public bool PreventRewardSpawn;
        public bool IsTrapZone;
        public bool IsPlayerInaccessible;
        public Action<CellData> OnCellGooped;
        public bool HasCachedPhysicsTile;
        public PhysicsEngine.Tile CachedPhysicsTile;
        [NonSerialized]
        private bool m_hasCachedUpperFacewallness;
        [NonSerialized]
        private bool m_upperFacewallness;
        [NonSerialized]
        private bool m_hasCachedLowerFacewallness;
        [NonSerialized]
        private bool m_lowerFacewallness;
        private bool? m_cachedFacewallness;

        public CellData(int x, int y, CellType t = CellType.WALL)
        {
            this.position = new IntVector2(x, y);
            this.positionInTilemap = new IntVector2(x, y);
            this.type = t;
            this.cellVisualData = new CellVisualData();
            this.cellVisualData.distanceToNearestLight = 100;
            this.cellVisualData.faceWallOverrideIndex = -1;
            this.cellVisualData.pitOverrideIndex = -1;
            this.cellVisualData.inheritedOverrideIndex = -1;
            this.cellVisualData.pathTilesetGridIndex = -1;
            this.cellVisualData.forcedMatchingStyle = DungeonTileStampData.IntermediaryMatchingStyle.ANY;
            this.cellVisualData.RatChunkBorderIndex = -1;
            this.occlusionData = new CellOcclusionData(this);
        }

        public CellData(IntVector2 p, CellType t = CellType.WALL)
        {
            this.position = p;
            this.positionInTilemap = p;
            this.type = t;
            this.cellVisualData = new CellVisualData();
            this.cellVisualData.distanceToNearestLight = 100;
            this.cellVisualData.faceWallOverrideIndex = -1;
            this.cellVisualData.pitOverrideIndex = -1;
            this.cellVisualData.inheritedOverrideIndex = -1;
            this.cellVisualData.pathTilesetGridIndex = -1;
            this.cellVisualData.forcedMatchingStyle = DungeonTileStampData.IntermediaryMatchingStyle.ANY;
            this.cellVisualData.RatChunkBorderIndex = -1;
            this.occlusionData = new CellOcclusionData(this);
        }

        public bool isNextToWall
        {
            get
            {
                if (!this.m_isNextToWall.HasValue)
                    this.m_isNextToWall = new bool?(this.HasWallNeighbor(includeTopwalls: false));
                return this.m_isNextToWall.Value;
            }
        }

        public bool cachedCanContainTeleporter { get; set; }

        public bool IsPassable => !this.isOccupied && this.type == CellType.FLOOR;

        public float UniqueHash
        {
            get
            {
                int num1 = 0 + this.position.x;
                int num2 = num1 + (num1 << 10);
                int num3 = (num2 ^ num2 >> 6) + this.position.y;
                int num4 = num3 + (num3 << 10);
                int num5 = num4 ^ num4 >> 6;
                int num6 = num5 + (num5 << 3);
                int num7 = num6 ^ num6 >> 11;
                return (float) ((double) (uint) (num7 + (num7 << 15)) * 1.0 / 4294967296.0);
            }
        }

        public bool HasPhantomCarpetNeighbor(bool includeDiagonals = true)
        {
            List<CellData> cellNeighbors = GameManager.Instance.Dungeon.data.GetCellNeighbors(this, includeDiagonals);
            for (int index = 0; index < cellNeighbors.Count; ++index)
            {
                if (cellNeighbors[index] != null && cellNeighbors[index].cellVisualData.IsPhantomCarpet)
                    return true;
            }
            return false;
        }

        public bool SurroundedByPits(bool includeDiagonals = true)
        {
            List<CellData> cellNeighbors = GameManager.Instance.Dungeon.data.GetCellNeighbors(this, includeDiagonals);
            for (int index = 0; index < cellNeighbors.Count; ++index)
            {
                if (cellNeighbors[index] != null && cellNeighbors[index].type != CellType.PIT)
                    return false;
            }
            return true;
        }

        public bool HasFloorNeighbor(DungeonData d, bool includeTopwalls = false, bool includeDiagonals = false)
        {
            List<CellData> cellNeighbors = d.GetCellNeighbors(this, includeDiagonals);
            for (int index = 0; index < cellNeighbors.Count; ++index)
            {
                if (cellNeighbors[index] != null && cellNeighbors[index].type == CellType.FLOOR && (includeTopwalls || !cellNeighbors[index].IsTopWall()))
                    return true;
            }
            return false;
        }

        public bool HasWallNeighbor(bool includeDiagonals = true, bool includeTopwalls = true)
        {
            List<CellData> cellNeighbors = GameManager.Instance.Dungeon.data.GetCellNeighbors(this, includeDiagonals);
            for (int index = 0; index < cellNeighbors.Count; ++index)
            {
                if (cellNeighbors[index] != null)
                {
                    if (!includeTopwalls)
                    {
                        if (includeDiagonals)
                        {
                            if (index >= 3 && index <= 5)
                                continue;
                        }
                        else if (index == 2)
                            continue;
                    }
                    if (cellNeighbors[index].type == CellType.WALL)
                        return true;
                }
            }
            return false;
        }

        public bool HasPitNeighbor(DungeonData d)
        {
            List<CellData> cellNeighbors = d.GetCellNeighbors(this, true);
            for (int index = 0; index < cellNeighbors.Count; ++index)
            {
                if (cellNeighbors[index] != null && cellNeighbors[index].type == CellType.PIT)
                    return true;
            }
            return false;
        }

        public PrototypeRoomPitEntry.PitBorderType GetPitBorderType(DungeonData d)
        {
            if (this.parentArea != null && this.parentArea.IsProceduralRoom)
                return PrototypeRoomPitEntry.PitBorderType.FLAT;
            if (this.type == CellType.PIT)
            {
                PrototypeRoomPitEntry prototypeRoomPitEntry = (PrototypeRoomPitEntry) null;
                if (this.parentArea != null && !this.parentArea.IsProceduralRoom)
                    prototypeRoomPitEntry = this.parentArea.prototypeRoom.GetPitEntryFromPosition(this.position - this.parentArea.basePosition + IntVector2.One);
                if (prototypeRoomPitEntry != null)
                    return prototypeRoomPitEntry.borderType;
            }
            else if (this.type != CellType.WALL || this.breakable)
            {
                foreach (CellData cellNeighbor in d.GetCellNeighbors(this, true))
                {
                    if (this.parentArea != null && cellNeighbor != null && cellNeighbor.parentArea != null && !((UnityEngine.Object) cellNeighbor.parentArea.prototypeRoom == (UnityEngine.Object) null) && cellNeighbor.type == CellType.PIT)
                    {
                        PrototypeRoomPitEntry entryFromPosition = cellNeighbor.parentArea.prototypeRoom.GetPitEntryFromPosition(cellNeighbor.position - this.parentArea.basePosition + IntVector2.One);
                        if (entryFromPosition != null)
                            return entryFromPosition.borderType;
                    }
                }
            }
            return PrototypeRoomPitEntry.PitBorderType.NONE;
        }

        public bool IsSideWallAdjacent()
        {
            return this.type != CellType.WALL && (GameManager.Instance.Dungeon.data[this.position + IntVector2.Right].type == CellType.WALL || GameManager.Instance.Dungeon.data[this.position + IntVector2.Left].type == CellType.WALL);
        }

        public bool IsLowerFaceWall()
        {
            if (Dungeon.IsGenerating)
                return GameManager.Instance.Dungeon.data.isFaceWallLower(this.position.x, this.position.y);
            if (!this.m_hasCachedLowerFacewallness)
            {
                this.m_lowerFacewallness = GameManager.Instance.Dungeon.data.isFaceWallLower(this.position.x, this.position.y);
                this.m_hasCachedLowerFacewallness = true;
            }
            return this.m_lowerFacewallness;
        }

        public bool IsUpperFacewall()
        {
            if (Dungeon.IsGenerating)
                return GameManager.Instance.Dungeon.data.isFaceWallHigher(this.position.x, this.position.y);
            if (!this.m_hasCachedUpperFacewallness)
            {
                this.m_upperFacewallness = GameManager.Instance.Dungeon.data.isFaceWallHigher(this.position.x, this.position.y);
                this.m_hasCachedUpperFacewallness = true;
            }
            return this.m_upperFacewallness;
        }

        public bool IsAnyFaceWall()
        {
            if (this.m_cachedFacewallness.HasValue)
                return this.m_cachedFacewallness.Value;
            bool flag = GameManager.Instance.Dungeon.data.isAnyFaceWall(this.position.x, this.position.y);
            if (!Dungeon.IsGenerating)
                this.m_cachedFacewallness = new bool?(flag);
            return flag;
        }

        public bool IsTopWall()
        {
            return GameManager.Instance.Dungeon.data.isTopWall(this.position.x, this.position.y);
        }

        public bool HasPassableNeighbor(DungeonData d)
        {
            foreach (CellData cellNeighbor in d.GetCellNeighbors(this))
            {
                if (cellNeighbor != null && cellNeighbor.IsPassable)
                    return true;
            }
            return false;
        }

        public CellData GetExitNeighbor()
        {
            foreach (CellData cellNeighbor in GameManager.Instance.Dungeon.data.GetCellNeighbors(this))
            {
                if (cellNeighbor != null && cellNeighbor.isExitCell)
                    return cellNeighbor;
            }
            return (CellData) null;
        }

        public bool HasNonTopWallWallNeighbor()
        {
            List<CellData> cellNeighbors = GameManager.Instance.Dungeon.data.GetCellNeighbors(this, true);
            for (int index = 0; index < cellNeighbors.Count; ++index)
            {
                if ((index < 3 || index > 5) && cellNeighbors[index].type == CellType.WALL)
                    return true;
            }
            return false;
        }

        public bool HasTypeNeighbor(DungeonData d, CellType t)
        {
            foreach (CellData cellNeighbor in d.GetCellNeighbors(this))
            {
                if (cellNeighbor != null && cellNeighbor.type == t)
                    return true;
            }
            return false;
        }

        public bool HasFaceWallNeighbor(DungeonData d)
        {
            foreach (CellData cellNeighbor in d.GetCellNeighbors(this))
            {
                if (cellNeighbor != null && d.isAnyFaceWall(cellNeighbor.position.x, cellNeighbor.position.y))
                    return true;
            }
            return false;
        }

        public bool HasMossyNeighbor(DungeonData d)
        {
            if (this.type == CellType.WALL)
                return false;
            foreach (CellData cellNeighbor in d.GetCellNeighbors(this))
            {
                if (cellNeighbor != null && (cellNeighbor.type == CellType.WALL || cellNeighbor.cellVisualData.isDecal))
                    return true;
            }
            return false;
        }

        public bool HasPatternNeighbor(DungeonData d)
        {
            if (this.type == CellType.WALL)
                return false;
            foreach (CellData cellNeighbor in d.GetCellNeighbors(this))
            {
                if (cellNeighbor != null && (cellNeighbor.type == CellType.WALL || cellNeighbor.cellVisualData.isPattern))
                    return true;
            }
            return false;
        }
    }
}
