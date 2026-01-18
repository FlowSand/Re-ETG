using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using UnityEngine;

using SimplexNoise;

#nullable disable
namespace Dungeonator
{
    public class DungeonData
    {
        public CellData[][] cellData;
        private int m_width = -1;
        private int m_height = -1;
        public List<RoomHandler> rooms;
        public Dictionary<IntVector2, DungeonDoorController> doors;
        public RoomHandler Entrance;
        public RoomHandler Exit;
        public tk2dTileMap tilemap;
        private static List<CellData> s_neighborsList = new List<CellData>(8);
        private ParticleSystem m_sizzleSystem;

        public DungeonData(CellData[][] data) => this.cellData = data;

        public void ClearCachedCellData()
        {
            this.m_width = -1;
            this.m_height = -1;
        }

        public int Width
        {
            get
            {
                if (this.m_width == -1)
                    this.m_width = this.cellData.Length;
                return this.m_width;
            }
        }

        public int Height
        {
            get
            {
                if (this.m_height == -1)
                    this.m_height = this.cellData[0].Length;
                return this.m_height;
            }
        }

        public CellData this[IntVector2 key]
        {
            get => this.cellData[key.x][key.y];
            set => this.cellData[key.x][key.y] = value;
        }

        public CellData this[int x, int y]
        {
            get => this.cellData[x][y];
            set => this.cellData[x][y] = value;
        }

        public static DungeonData.Direction InvertDirection(DungeonData.Direction inDir)
        {
            switch (inDir)
            {
                case DungeonData.Direction.NORTH:
                    return DungeonData.Direction.SOUTH;
                case DungeonData.Direction.NORTHEAST:
                    return DungeonData.Direction.SOUTHWEST;
                case DungeonData.Direction.EAST:
                    return DungeonData.Direction.WEST;
                case DungeonData.Direction.SOUTHEAST:
                    return DungeonData.Direction.NORTHWEST;
                case DungeonData.Direction.SOUTH:
                    return DungeonData.Direction.NORTH;
                case DungeonData.Direction.SOUTHWEST:
                    return DungeonData.Direction.NORTHEAST;
                case DungeonData.Direction.WEST:
                    return DungeonData.Direction.EAST;
                case DungeonData.Direction.NORTHWEST:
                    return DungeonData.Direction.SOUTHEAST;
                default:
                    return inDir;
            }
        }

        public static DungeonData.Direction GetRandomCardinalDirection()
        {
            float num = UnityEngine.Random.value;
            if ((double) num < 0.25)
                return DungeonData.Direction.NORTH;
            if ((double) num < 0.5)
                return DungeonData.Direction.EAST;
            return (double) num < 0.75 ? DungeonData.Direction.SOUTH : DungeonData.Direction.WEST;
        }

        public static DungeonData.Direction GetCardinalFromVector2(Vector2 vec)
        {
            return DungeonData.GetDirectionFromVector2(BraveUtility.GetMajorAxis(vec));
        }

        public static DungeonData.Direction GetDirectionFromInts(int x, int y)
        {
            if (x == 0)
            {
                if (y > 0)
                    return DungeonData.Direction.NORTH;
                return y < 0 ? DungeonData.Direction.SOUTH : ~DungeonData.Direction.NORTH;
            }
            if (x < 0)
            {
                if (y > 0)
                    return DungeonData.Direction.NORTHWEST;
                return y < 0 ? DungeonData.Direction.SOUTHWEST : DungeonData.Direction.WEST;
            }
            if (y > 0)
                return DungeonData.Direction.NORTHEAST;
            return y < 0 ? DungeonData.Direction.SOUTHEAST : DungeonData.Direction.EAST;
        }

        public static DungeonData.Direction GetDirectionFromIntVector2(IntVector2 vec)
        {
            return DungeonData.GetDirectionFromInts(vec.x, vec.y);
        }

        public static DungeonData.Direction GetDirectionFromVector2(Vector2 vec)
        {
            if ((double) vec.x == 0.0)
            {
                if ((double) vec.y > 0.0)
                    return DungeonData.Direction.NORTH;
                return (double) vec.y < 0.0 ? DungeonData.Direction.SOUTH : ~DungeonData.Direction.NORTH;
            }
            if ((double) vec.x < 0.0)
            {
                if ((double) vec.y > 0.0)
                    return DungeonData.Direction.NORTHWEST;
                return (double) vec.y < 0.0 ? DungeonData.Direction.SOUTHWEST : DungeonData.Direction.WEST;
            }
            if ((double) vec.y > 0.0)
                return DungeonData.Direction.NORTHEAST;
            return (double) vec.y < 0.0 ? DungeonData.Direction.SOUTHEAST : DungeonData.Direction.EAST;
        }

        public static IntVector2 GetIntVector2FromDirection(DungeonData.Direction dir)
        {
            switch (dir)
            {
                case DungeonData.Direction.NORTH:
                    return IntVector2.Up;
                case DungeonData.Direction.NORTHEAST:
                    return IntVector2.Up + IntVector2.Right;
                case DungeonData.Direction.EAST:
                    return IntVector2.Right;
                case DungeonData.Direction.SOUTHEAST:
                    return IntVector2.Right + IntVector2.Down;
                case DungeonData.Direction.SOUTH:
                    return IntVector2.Down;
                case DungeonData.Direction.SOUTHWEST:
                    return IntVector2.Down + IntVector2.Left;
                case DungeonData.Direction.WEST:
                    return IntVector2.Left;
                case DungeonData.Direction.NORTHWEST:
                    return IntVector2.Left + IntVector2.Up;
                default:
                    return IntVector2.Zero;
            }
        }

        public static DungeonData.Direction GetInverseDirection(DungeonData.Direction dir)
        {
            switch (dir)
            {
                case DungeonData.Direction.NORTH:
                    return DungeonData.Direction.SOUTH;
                case DungeonData.Direction.NORTHEAST:
                    return DungeonData.Direction.SOUTHWEST;
                case DungeonData.Direction.EAST:
                    return DungeonData.Direction.WEST;
                case DungeonData.Direction.SOUTHEAST:
                    return DungeonData.Direction.NORTHWEST;
                case DungeonData.Direction.SOUTH:
                    return DungeonData.Direction.NORTH;
                case DungeonData.Direction.SOUTHWEST:
                    return DungeonData.Direction.NORTHEAST;
                case DungeonData.Direction.WEST:
                    return DungeonData.Direction.EAST;
                case DungeonData.Direction.NORTHWEST:
                    return DungeonData.Direction.SOUTHEAST;
                default:
                    return DungeonData.Direction.SOUTH;
            }
        }

        public static float GetAngleFromDirection(DungeonData.Direction dir)
        {
            switch (dir)
            {
                case DungeonData.Direction.NORTH:
                    return 90f;
                case DungeonData.Direction.NORTHEAST:
                    return 45f;
                case DungeonData.Direction.EAST:
                    return 0.0f;
                case DungeonData.Direction.SOUTHEAST:
                    return 315f;
                case DungeonData.Direction.SOUTH:
                    return 270f;
                case DungeonData.Direction.SOUTHWEST:
                    return 225f;
                case DungeonData.Direction.WEST:
                    return 180f;
                case DungeonData.Direction.NORTHWEST:
                    return 135f;
                default:
                    return 0.0f;
            }
        }

        public void InitializeCoreData(List<RoomHandler> r) => this.rooms = r;

        private void PreprocessDungeonWings()
        {
            if (this.Exit == null || this.Exit.area.PrototypeRoomCategory != PrototypeDungeonRoom.RoomCategory.EXIT)
                return;
            List<RoomHandler> pathBetweenNodes = SenseOfDirectionItem.FindPathBetweenNodes(this.Entrance, this.Exit, this.rooms);
            if (pathBetweenNodes == null)
                return;
            DungeonWingDefinition dungeonWingDefinition = (DungeonWingDefinition) null;
            if (GameManager.Instance.Dungeon.dungeonWingDefinitions.Length > 0)
                dungeonWingDefinition = GameManager.Instance.Dungeon.SelectWingDefinition(true);
            foreach (RoomHandler roomHandler in pathBetweenNodes)
            {
                roomHandler.IsOnCriticalPath = true;
                if (dungeonWingDefinition != null)
                    roomHandler.AssignRoomVisualType(dungeonWingDefinition.includedMaterialIndices.SelectByWeight(), true);
            }
            int num = 0;
            if (dungeonWingDefinition != null)
                dungeonWingDefinition = GameManager.Instance.Dungeon.SelectWingDefinition(false);
            foreach (RoomHandler roomHandler in pathBetweenNodes)
            {
                foreach (RoomHandler connectedRoom1 in roomHandler.connectedRooms)
                {
                    if (!connectedRoom1.IsOnCriticalPath && connectedRoom1.DungeonWingID == -1)
                    {
                        connectedRoom1.DungeonWingID = num;
                        if (dungeonWingDefinition != null)
                            connectedRoom1.AssignRoomVisualType(dungeonWingDefinition.includedMaterialIndices.SelectByWeight(), true);
                        Queue<RoomHandler> roomHandlerQueue = new Queue<RoomHandler>();
                        roomHandlerQueue.Enqueue(connectedRoom1);
                        while (roomHandlerQueue.Count > 0)
                        {
                            foreach (RoomHandler connectedRoom2 in roomHandlerQueue.Dequeue().connectedRooms)
                            {
                                if (!connectedRoom2.IsOnCriticalPath && connectedRoom2.DungeonWingID == -1)
                                {
                                    connectedRoom2.DungeonWingID = num;
                                    if (dungeonWingDefinition != null)
                                        connectedRoom2.AssignRoomVisualType(dungeonWingDefinition.includedMaterialIndices.SelectByWeight(), true);
                                    roomHandlerQueue.Enqueue(connectedRoom2);
                                }
                            }
                        }
                        ++num;
                        if (dungeonWingDefinition != null)
                            dungeonWingDefinition = GameManager.Instance.Dungeon.SelectWingDefinition(false);
                    }
                }
            }
            foreach (RoomHandler room in this.rooms)
            {
                if (room.IsOnCriticalPath)
                {
                    BraveUtility.DrawDebugSquare(room.area.basePosition.ToVector2(), room.area.basePosition.ToVector2() + room.area.dimensions.ToVector2(), Color.cyan, 1000f);
                }
                else
                {
                    Color color = new Color((float) (1.0 - (double) room.DungeonWingID / 7.0), (float) (1.0 - (double) room.DungeonWingID / 7.0), (float) room.DungeonWingID / 7f);
                    BraveUtility.DrawDebugSquare(room.area.basePosition.ToVector2(), room.area.basePosition.ToVector2() + room.area.dimensions.ToVector2(), color, 1000f);
                }
            }
        }

        [DebuggerHidden]
        public IEnumerator Apply(
            TileIndices indices,
            TilemapDecoSettings decoSettings,
            tk2dTileMap tilemapRef)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new DungeonData__Applyc__Iterator0()
            {
                tilemapRef = tilemapRef,
                indices = indices,
                decoSettings = decoSettings,
                _this = this
            };
        }

        public void PostProcessFeatures()
        {
            foreach (RoomHandler room in this.rooms)
                room.PostProcessFeatures();
            this.HandleFloorSpecificCustomization();
        }

        private void HandleFloorSpecificCustomization()
        {
            if (GameManager.Instance.Dungeon.tileIndices.tilesetId != GlobalDungeonData.ValidTilesets.CASTLEGEON)
                return;
            FireplaceController fireplace = UnityEngine.Object.FindObjectOfType<FireplaceController>();
            if (!(bool) (UnityEngine.Object) fireplace)
                return;
            RoomHandler fireplaceRoom = fireplace.transform.position.GetAbsoluteRoom();
            List<MinorBreakable> targetBarrels = new List<MinorBreakable>();
            List<MinorBreakable> allMinorBreakables = StaticReferenceManager.AllMinorBreakables;
            int numToReplace = 2;
            Func<MinorBreakable, bool> func = (Func<MinorBreakable, bool>) (testBarrel =>
            {
                int index1 = -1;
                if (!GameStatsManager.Instance.GetFlag(GungeonFlags.FLAG_ROLLED_BARREL_INTO_FIREPLACE) && testBarrel.transform.position.GetAbsoluteRoom() == fireplaceRoom)
                    return false;
                bool flag = testBarrel.CastleReplacedWithWaterDrum;
                if (!flag)
                    return false;
                IntVector2 intVector2 = testBarrel.transform.position.IntXY(VectorConversions.Floor);
                if (!GameManager.Instance.Dungeon.data.CheckInBoundsAndValid(intVector2) || GameManager.Instance.Dungeon.data[intVector2].HasWallNeighbor())
                    return false;
                for (int index2 = 0; index2 < targetBarrels.Count; ++index2)
                {
                    if (targetBarrels[index2].transform.position.GetAbsoluteRoom() == testBarrel.transform.position.GetAbsoluteRoom())
                        flag = false;
                    else if (targetBarrels.Count >= numToReplace && (double) Vector2.Distance((Vector2) fireplace.transform.position, (Vector2) targetBarrels[index2].transform.position) > (double) Vector2.Distance((Vector2) fireplace.transform.position, (Vector2) testBarrel.transform.position))
                        index1 = index2;
                }
                if (flag && targetBarrels.Count < numToReplace)
                {
                    targetBarrels.Add(testBarrel);
                    return true;
                }
                if (!flag || index1 == -1)
                    return false;
                targetBarrels[index1] = testBarrel;
                return true;
            });
            for (int index = 0; index < allMinorBreakables.Count; ++index)
            {
                int num = func(allMinorBreakables[index]) ? 1 : 0;
            }
            DungeonPlaceable dungeonPlaceable = BraveResources.Load("Drum_Water_Castle", ".asset") as DungeonPlaceable;
            for (int index = 0; index < targetBarrels.Count; ++index)
            {
                Vector3 vector = targetBarrels[index].transform.position + new Vector3(0.75f, 0.0f, 0.0f);
                RoomHandler absoluteRoom = targetBarrels[index].transform.position.GetAbsoluteRoom();
                IntVector2 location = vector.IntXY(VectorConversions.Floor) - absoluteRoom.area.basePosition;
                GameObject gameObject = dungeonPlaceable.InstantiateObject(absoluteRoom, location);
                gameObject.transform.position = gameObject.transform.position;
                KickableObject componentInChildren = gameObject.GetComponentInChildren<KickableObject>();
                if ((bool) (UnityEngine.Object) componentInChildren)
                {
                    componentInChildren.specRigidbody.Reinitialize();
                    componentInChildren.rollSpeed = 3f;
                    componentInChildren.AllowTopWallTraversal = true;
                    absoluteRoom.RegisterInteractable((IPlayerInteractable) componentInChildren);
                }
                KickableObject component = targetBarrels[index].GetComponent<KickableObject>();
                if ((bool) (UnityEngine.Object) component)
                    component.ForceDeregister();
                UnityEngine.Object.Destroy((UnityEngine.Object) targetBarrels[index].gameObject);
            }
            if (targetBarrels.Count >= numToReplace)
                return;
            for (int index = 0; index < this.rooms.Count; ++index)
            {
                if (this.rooms[index].IsShop)
                {
                    IntVector2 bestRewardLocation = this.rooms[index].GetBestRewardLocation(IntVector2.One * 2, RoomHandler.RewardLocationStyle.Original, false);
                    KickableObject componentInChildren = dungeonPlaceable.InstantiateObject(this.rooms[index], bestRewardLocation - this.rooms[index].area.basePosition).GetComponentInChildren<KickableObject>();
                    if ((bool) (UnityEngine.Object) componentInChildren)
                    {
                        componentInChildren.rollSpeed = 3f;
                        componentInChildren.AllowTopWallTraversal = true;
                        this.rooms[index].RegisterInteractable((IPlayerInteractable) componentInChildren);
                    }
                }
            }
        }

        private void HandleTrapAreas()
        {
            foreach (PathingTrapController pathingTrapController in UnityEngine.Object.FindObjectsOfType<PathingTrapController>())
            {
                if (!(bool) (UnityEngine.Object) pathingTrapController.specRigidbody)
                    return;
                pathingTrapController.specRigidbody.Initialize();
                RoomHandler roomFromPosition = this.GetAbsoluteRoomFromPosition(pathingTrapController.specRigidbody.UnitCenter.ToIntVector2());
                PathMover component1 = pathingTrapController.GetComponent<PathMover>();
                Vector2 unitDimensions = pathingTrapController.specRigidbody.UnitDimensions;
                ResizableCollider component2 = pathingTrapController.GetComponent<ResizableCollider>();
                if ((bool) (UnityEngine.Object) component2)
                {
                    if (component2.IsHorizontal)
                        unitDimensions.x = component2.NumTiles;
                    else
                        unitDimensions.y = component2.NumTiles;
                }
                Vector2 vector2_1 = Vector2Extensions.max;
                Vector2 vector2_2 = Vector2Extensions.min;
                for (int index = 0; index < component1.Path.nodes.Count; ++index)
                {
                    Vector2 rhs = roomFromPosition.area.basePosition.ToVector2() + component1.Path.nodes[index].RoomPosition;
                    vector2_1 = Vector2.Min(vector2_1, rhs);
                    vector2_2 = Vector2.Max(vector2_2, rhs + unitDimensions);
                }
                IntVector2 intVector2_1 = vector2_1.ToIntVector2(VectorConversions.Floor);
                IntVector2 intVector2_2 = vector2_2.ToIntVector2(VectorConversions.Floor);
                for (int x = intVector2_1.x; x <= intVector2_2.x; ++x)
                {
                    for (int y = intVector2_1.y; y <= intVector2_2.y; ++y)
                        this[x, y].IsTrapZone = true;
                }
            }
            ProjectileTrapController[] objectsOfType = UnityEngine.Object.FindObjectsOfType<ProjectileTrapController>();
    label_27:
            for (int index = 0; index < objectsOfType.Length; ++index)
            {
                ProjectileTrapController projectileTrapController = objectsOfType[index];
                IntVector2 intVector2_3 = projectileTrapController.shootPoint.position.IntXY(VectorConversions.Floor);
                IntVector2 vector2FromDirection = DungeonData.GetIntVector2FromDirection(projectileTrapController.shootDirection);
                if (!(vector2FromDirection == IntVector2.Zero))
                {
                    IntVector2 intVector2_4 = intVector2_3;
                    while (true)
                    {
                        if (!this.CheckInBoundsAndValid(intVector2_4) || this.isWall(intVector2_4.x, intVector2_4.y))
                        {
                            if (!(intVector2_3 == intVector2_4))
                                goto label_27;
                        }
                        else
                            this[intVector2_4].IsTrapZone = true;
                        intVector2_4 += vector2FromDirection;
                    }
                }
            }
        }

        private void AddProceduralTeleporters()
        {
            List<List<RoomHandler>> roomHandlerListList = new List<List<RoomHandler>>();
            List<RoomHandler> roomsContainingTeleporters = new List<RoomHandler>();
            Func<RoomHandler, bool> func1 = (Func<RoomHandler, bool>) (r =>
            {
                if (r.area.IsProceduralRoom || !r.EverHadEnemies || r.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.REWARD || r.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.SECRET || r.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.BOSS || r.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.EXIT || r.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.NORMAL && r.area.PrototypeRoomNormalSubcategory == PrototypeDungeonRoom.RoomNormalSubCategory.TRAP)
                    return false;
                for (int index = 0; index < r.connectedRooms.Count; ++index)
                {
                    if (roomsContainingTeleporters.Contains(r.connectedRooms[index]))
                        return false;
                }
                return true;
            });
            for (int index1 = 0; index1 < this.rooms.Count; ++index1)
            {
                if (Minimap.Instance.HasTeleporterIcon(this.rooms[index1]))
                    roomsContainingTeleporters.Add(this.rooms[index1]);
                if (!roomsContainingTeleporters.Contains(this.rooms[index1]) && this.rooms[index1].connectedRooms.Count >= 4)
                {
                    this.rooms[index1].AddProceduralTeleporterToRoom();
                    roomsContainingTeleporters.Add(this.rooms[index1]);
                }
                if (this.rooms[index1].IsLoopMember)
                {
                    List<RoomHandler> roomHandlerList = (List<RoomHandler>) null;
                    for (int index2 = 0; index2 < roomHandlerListList.Count; ++index2)
                    {
                        if (roomHandlerListList[index2][0].LoopGuid.Equals(this.rooms[index1].LoopGuid))
                        {
                            roomHandlerList = roomHandlerListList[index2];
                            break;
                        }
                    }
                    if (roomHandlerList != null)
                        roomHandlerList.Add(this.rooms[index1]);
                    else
                        roomHandlerListList.Add(new List<RoomHandler>()
                        {
                            this.rooms[index1]
                        });
                }
                else if (!roomsContainingTeleporters.Contains(this.rooms[index1]) && this.rooms[index1].connectedRooms.Count == 1)
                {
                    if (func1(this.rooms[index1]))
                    {
                        this.rooms[index1].AddProceduralTeleporterToRoom();
                        roomsContainingTeleporters.Add(this.rooms[index1]);
                    }
                    else if (func1(this.rooms[index1].connectedRooms[0]))
                    {
                        this.rooms[index1].connectedRooms[0].AddProceduralTeleporterToRoom();
                        roomsContainingTeleporters.Add(this.rooms[index1].connectedRooms[0]);
                    }
                }
            }
            Func<RoomHandler, int> func2 = (Func<RoomHandler, int>) (r =>
            {
                int num1 = int.MaxValue;
                for (int index = 0; index < roomsContainingTeleporters.Count; ++index)
                {
                    int num2 = IntVector2.ManhattanDistance(roomsContainingTeleporters[index].Epicenter, r.Epicenter);
                    if (num2 < num1)
                        num1 = num2;
                }
                return num1;
            });
            for (int index3 = 0; index3 < roomHandlerListList.Count; ++index3)
            {
                List<RoomHandler> roomHandlerList = roomHandlerListList[index3];
                int num3 = Mathf.Max(1, Mathf.RoundToInt((float) roomHandlerList.Count / 4f));
                for (int index4 = 0; index4 < num3; ++index4)
                {
                    RoomHandler roomHandler = (RoomHandler) null;
                    int num4 = int.MinValue;
                    for (int index5 = 0; index5 < roomHandlerList.Count; ++index5)
                    {
                        if (func1(roomHandlerList[index5]))
                        {
                            int num5 = func2(roomHandlerList[index5]);
                            if (roomHandlerList[index5].connectedRooms.Count > 2)
                                num5 += 10;
                            if (num5 > num4)
                            {
                                roomHandler = roomHandlerList[index5];
                                num4 = num5;
                            }
                        }
                    }
                    if (roomHandler != null)
                    {
                        roomHandler.AddProceduralTeleporterToRoom();
                        if (!roomsContainingTeleporters.Contains(roomHandler))
                            roomsContainingTeleporters.Add(roomHandler);
                    }
                }
            }
        }

        public void PostGenerationCleanup()
        {
            for (int index1 = 0; index1 < this.Width; ++index1)
            {
                for (int index2 = 0; index2 < this.Height; ++index2)
                {
                    bool flag = true;
                    if (this.cellData[index1][index2] != null && this.cellData[index1][index2].cellVisualData.IsFeatureCell)
                        flag = false;
                    if (flag)
                    {
                        for (int index3 = -3; index3 <= 3; ++index3)
                        {
                            for (int index4 = -3; index4 <= 3; ++index4)
                            {
                                if (this.CheckInBounds(index1 + index3, index2 + index4) && this.cellData[index1 + index3][index2 + index4] != null && this.cellData[index1 + index3][index2 + index4].type != CellType.WALL)
                                    flag = false;
                                if (!flag)
                                    break;
                            }
                            if (!flag)
                                break;
                        }
                    }
                    if (flag)
                        this.cellData[index1][index2] = (CellData) null;
                    else if (this.cellData[index1][index2].type != CellType.WALL)
                    {
                        bool isNextToWall = this.cellData[index1][index2].isNextToWall;
                    }
                }
            }
        }

        public RoomHandler GetAbsoluteRoomFromPosition(IntVector2 pos)
        {
            CellData cellData = !this.CheckInBounds(pos) ? (CellData) null : this[pos];
            if (cellData == null)
            {
                float num = float.MaxValue;
                RoomHandler roomFromPosition = (RoomHandler) null;
                for (int index = 0; index < this.rooms.Count; ++index)
                {
                    float rectangle = BraveMathCollege.DistToRectangle(pos.ToCenterVector2(), this.rooms[index].area.basePosition.ToVector2(), this.rooms[index].area.dimensions.ToVector2());
                    if ((double) rectangle < (double) num)
                    {
                        num = rectangle;
                        roomFromPosition = this.rooms[index];
                    }
                }
                return roomFromPosition;
            }
            return cellData.parentRoom == null ? cellData.nearestRoom : cellData.parentRoom;
        }

        public RoomHandler GetRoomFromPosition(IntVector2 pos) => this[pos].parentRoom;

        public CellVisualData.CellFloorType GetFloorTypeFromPosition(IntVector2 pos)
        {
            return !this.CheckInBoundsAndValid(pos) ? CellVisualData.CellFloorType.Stone : this[pos].cellVisualData.floorType;
        }

        public CellType GetCellTypeSafe(IntVector2 pos)
        {
            if (!this.CheckInBounds(pos))
                return CellType.WALL;
            CellData cellData = this[pos];
            return cellData == null ? CellType.WALL : cellData.type;
        }

        public CellType GetCellTypeSafe(int x, int y)
        {
            if (!this.CheckInBounds(x, y))
                return CellType.WALL;
            CellData cellData = this[x, y];
            return cellData == null ? CellType.WALL : cellData.type;
        }

        public CellData GetCellSafe(IntVector2 pos)
        {
            return this.CheckInBounds(pos) ? this[pos] : (CellData) null;
        }

        public CellData GetCellSafe(int x, int y)
        {
            return this.CheckInBounds(x, y) ? this[x, y] : (CellData) null;
        }

        private static bool CheckCellNeedsAdditionalLight(
            List<IntVector2> positions,
            RoomHandler room,
            CellData currentCell)
        {
            int num = !room.area.IsProceduralRoom ? 10 : 20;
            if (currentCell.isExitCell || currentCell.type == CellType.WALL)
                return false;
            bool flag = true;
            for (int index = 0; index < positions.Count; ++index)
            {
                if (IntVector2.ManhattanDistance(positions[index] + room.area.basePosition, currentCell.position) <= num)
                {
                    flag = false;
                    break;
                }
            }
            if (flag)
                positions.Add(currentCell.position - room.area.basePosition);
            return flag;
        }

        private void PostprocessLightPositions(List<IntVector2> positions, RoomHandler room)
        {
            DungeonData.CheckCellNeedsAdditionalLight(positions, room, this[room.GetCenterCell()]);
            for (int index = 0; index < room.Cells.Count; ++index)
            {
                CellData currentCell = this[room.Cells[index]];
                DungeonData.CheckCellNeedsAdditionalLight(positions, room, currentCell);
            }
        }

        public void ReplicateLighting(CellData sourceCell, CellData targetCell)
        {
            Vector3 position = sourceCell.cellVisualData.lightObject.transform.position - sourceCell.position.ToVector2().ToVector3ZisY() + targetCell.position.ToVector2().ToVector3ZisY();
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(sourceCell.cellVisualData.lightObject, position, Quaternion.identity);
            gameObject.transform.parent = sourceCell.cellVisualData.lightObject.transform.parent;
            if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.FORGEGEON)
                this[targetCell.position + IntVector2.Down].cellVisualData.containsObjectSpaceStamp = true;
            targetCell.cellVisualData.containsLight = true;
            targetCell.cellVisualData.lightObject = gameObject;
            targetCell.cellVisualData.facewallLightStampData = sourceCell.cellVisualData.facewallLightStampData;
            targetCell.cellVisualData.sidewallLightStampData = sourceCell.cellVisualData.sidewallLightStampData;
        }

        public void GenerateLightsForRoom(
            TilemapDecoSettings decoSettings,
            RoomHandler rh,
            Transform lightParent,
            DungeonData.LightGenerationStyle style = DungeonData.LightGenerationStyle.STANDARD)
        {
            if (!GameManager.Instance.Dungeon.roomMaterialDefinitions[rh.RoomVisualSubtype].useLighting)
                return;
            bool flag1 = decoSettings.lightCookies.Length > 0;
            List<Tuple<IntVector2, float>> tupleList = new List<Tuple<IntVector2, float>>();
            bool flag2 = false;
            List<IntVector2> intVector2List;
            int count1;
            if (rh.area != null && !rh.area.IsProceduralRoom && !rh.area.prototypeRoom.usesProceduralLighting)
            {
                intVector2List = rh.GatherManualLightPositions();
                count1 = intVector2List.Count;
            }
            else
            {
                flag2 = true;
                intVector2List = rh.GatherOptimalLightPositions(decoSettings);
                count1 = intVector2List.Count;
                if (rh.area != null && (UnityEngine.Object) rh.area.prototypeRoom != (UnityEngine.Object) null)
                    this.PostprocessLightPositions(intVector2List, rh);
            }
            if ((UnityEngine.Object) rh.area.prototypeRoom != (UnityEngine.Object) null)
            {
                for (int index = 0; index < rh.area.instanceUsedExits.Count; ++index)
                {
                    RuntimeRoomExitData exitToLocalData = rh.area.exitToLocalDataMap[rh.area.instanceUsedExits[index]];
                    RuntimeExitDefinition runtimeExitDefinition = rh.exitDefinitionsByExit[exitToLocalData];
                    if (exitToLocalData.TotalExitLength > 4 && !runtimeExitDefinition.containsLight)
                    {
                        IntVector2 first = !exitToLocalData.jointedExit ? runtimeExitDefinition.GetLinearMidpoint(rh) : exitToLocalData.ExitOrigin - IntVector2.One;
                        tupleList.Add(new Tuple<IntVector2, float>(first, 0.5f));
                        runtimeExitDefinition.containsLight = true;
                    }
                }
            }
            GlobalDungeonData.ValidTilesets tilesetId = GameManager.Instance.Dungeon.tileIndices.tilesetId;
            float cullingPercentage = decoSettings.lightCullingPercentage;
            if (flag2 && (double) cullingPercentage > 0.0)
            {
                int num1 = Mathf.FloorToInt((float) intVector2List.Count * cullingPercentage);
                int num2 = Mathf.FloorToInt((float) tupleList.Count * cullingPercentage);
                if (num1 == 0 && num2 == 0 && intVector2List.Count + tupleList.Count > 4)
                    num1 = 1;
                for (; num1 > 0 && intVector2List.Count > 0; --num1)
                    intVector2List.RemoveAt(UnityEngine.Random.Range(0, intVector2List.Count));
                for (; num2 > 0 && tupleList.Count > 0; --num2)
                    tupleList.RemoveAt(UnityEngine.Random.Range(0, tupleList.Count));
            }
            int count2 = intVector2List.Count;
            if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.NONE && (tilesetId == GlobalDungeonData.ValidTilesets.MINEGEON || tilesetId == GlobalDungeonData.ValidTilesets.GUNGEON || tilesetId == GlobalDungeonData.ValidTilesets.CATACOMBGEON) && (flag2 || rh.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.NORMAL || rh.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.HUB || rh.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.CONNECTOR))
                intVector2List.AddRange((IEnumerable<IntVector2>) rh.GatherPitLighting(decoSettings, intVector2List));
            for (int index1 = 0; index1 < intVector2List.Count + tupleList.Count; ++index1)
            {
                IntVector2 negOne = IntVector2.NegOne;
                float num3 = 1f;
                bool flag3 = false;
                if (index1 < intVector2List.Count && index1 >= count2)
                {
                    flag3 = true;
                    num3 = 0.6f;
                }
                IntVector2 intVector2;
                if (index1 < intVector2List.Count)
                {
                    intVector2 = rh.area.basePosition + intVector2List[index1];
                }
                else
                {
                    intVector2 = rh.area.basePosition + tupleList[index1 - intVector2List.Count].First;
                    num3 = tupleList[index1 - intVector2List.Count].Second;
                }
                bool flag4 = false;
                if (flag1 && flag2 && intVector2 == rh.GetCenterCell())
                    flag4 = true;
                IntVector2 key = intVector2 + IntVector2.Up;
                bool flag5 = index1 >= count1;
                bool flag6 = false;
                Vector3 vector3 = Vector3.zero;
                if (this[intVector2 + IntVector2.Up].type == CellType.WALL)
                {
                    this[key].cellVisualData.lightDirection = DungeonData.Direction.NORTH;
                    vector3 = Vector3.down;
                }
                else if (this[intVector2 + IntVector2.Right].type == CellType.WALL)
                    this[key].cellVisualData.lightDirection = DungeonData.Direction.EAST;
                else if (this[intVector2 + IntVector2.Left].type == CellType.WALL)
                    this[key].cellVisualData.lightDirection = DungeonData.Direction.WEST;
                else if (this[intVector2 + IntVector2.Down].type == CellType.WALL)
                {
                    flag6 = true;
                    this[key].cellVisualData.lightDirection = DungeonData.Direction.SOUTH;
                }
                else
                    this[key].cellVisualData.lightDirection = ~DungeonData.Direction.NORTH;
                int index2 = rh.RoomVisualSubtype;
                float num4 = 0.0f;
                if ((UnityEngine.Object) rh.area.prototypeRoom != (UnityEngine.Object) null)
                {
                    PrototypeDungeonRoomCellData dungeonRoomCellData = index1 >= intVector2List.Count ? rh.area.prototypeRoom.ForceGetCellDataAtPoint(tupleList[index1 - intVector2List.Count].First.x, tupleList[index1 - intVector2List.Count].First.y) : rh.area.prototypeRoom.ForceGetCellDataAtPoint(intVector2List[index1].x, intVector2List[index1].y);
                    if (dungeonRoomCellData != null && dungeonRoomCellData.containsManuallyPlacedLight)
                    {
                        index2 = dungeonRoomCellData.lightStampIndex;
                        num4 = (float) dungeonRoomCellData.lightPixelsOffsetY / 16f;
                    }
                }
                if (index2 < 0 || index2 >= GameManager.Instance.Dungeon.roomMaterialDefinitions.Length)
                    index2 = 0;
                DungeonMaterial materialDefinition = GameManager.Instance.Dungeon.roomMaterialDefinitions[index2];
                int outIndex = -1;
                GameObject original;
                if (style == DungeonData.LightGenerationStyle.FORCE_COLOR || style == DungeonData.LightGenerationStyle.RAT_HALLWAY)
                {
                    outIndex = 0;
                    original = materialDefinition.lightPrefabs.elements[0].gameObject;
                }
                else
                    original = materialDefinition.lightPrefabs.SelectByWeight(out outIndex);
                if (!materialDefinition.facewallLightStamps[outIndex].CanBeTopWallLight && flag6 || !materialDefinition.facewallLightStamps[outIndex].CanBeCenterLight && flag5)
                {
                    if (outIndex >= materialDefinition.facewallLightStamps.Count)
                        outIndex = 0;
                    outIndex = materialDefinition.facewallLightStamps[outIndex].FallbackIndex;
                    original = materialDefinition.lightPrefabs.elements[outIndex].gameObject;
                }
                GameObject gameObject1 = UnityEngine.Object.Instantiate<GameObject>(original, key.ToVector3(0.0f), Quaternion.identity);
                gameObject1.transform.parent = lightParent;
                gameObject1.transform.position = key.ToCenterVector3((float) key.y + decoSettings.lightHeight) + new Vector3(0.0f, num4, 0.0f) + vector3;
                ShadowSystem componentInChildren1 = gameObject1.GetComponentInChildren<ShadowSystem>();
                Light componentInChildren2 = gameObject1.GetComponentInChildren<Light>();
                if ((UnityEngine.Object) componentInChildren2 != (UnityEngine.Object) null)
                    componentInChildren2.intensity *= num3;
                if (style == DungeonData.LightGenerationStyle.FORCE_COLOR || style == DungeonData.LightGenerationStyle.RAT_HALLWAY)
                {
                    SceneLightManager component = gameObject1.GetComponent<SceneLightManager>();
                    if ((bool) (UnityEngine.Object) component)
                    {
                        Color[] colorArray = new Color[1]
                        {
                            component.validColors[0]
                        };
                        component.validColors = colorArray;
                    }
                }
                if (flag3 && (UnityEngine.Object) componentInChildren1 != (UnityEngine.Object) null)
                {
                    if ((bool) (UnityEngine.Object) componentInChildren2)
                        componentInChildren2.range += GameManager.Instance.Dungeon.tileIndices.tilesetId != GlobalDungeonData.ValidTilesets.CATACOMBGEON ? 3f : 5f;
                    componentInChildren1.ignoreCustomFloorLight = true;
                }
                if (flag4 && flag1 && (UnityEngine.Object) componentInChildren1 != (UnityEngine.Object) null)
                {
                    componentInChildren1.uLightCookie = decoSettings.GetRandomLightCookie();
                    componentInChildren1.uLightCookieAngle = UnityEngine.Random.Range(0.0f, 6.28f);
                    componentInChildren2.intensity *= 1.5f;
                }
                if (this[key].cellVisualData.lightDirection == DungeonData.Direction.NORTH)
                {
                    bool flag7 = true;
                    for (int index3 = -2; index3 < 3; ++index3)
                    {
                        if (this[key + IntVector2.Right * index3].type == CellType.FLOOR)
                        {
                            flag7 = false;
                            break;
                        }
                    }
                    if (flag7 && (bool) (UnityEngine.Object) componentInChildren1)
                    {
                        GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>((GameObject) BraveResources.Load("Global VFX/Wall_Light_Cookie"));
                        Transform transform = gameObject2.transform;
                        transform.parent = gameObject1.transform;
                        transform.localPosition = Vector3.zero;
                        componentInChildren1.PersonalCookies.Add(gameObject2.GetComponent<Renderer>());
                    }
                }
                CellData cellData = this[key + new IntVector2(0, Mathf.RoundToInt(num4))];
                if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.FORGEGEON)
                    this[cellData.position + IntVector2.Down].cellVisualData.containsObjectSpaceStamp = true;
                BraveUtility.DrawDebugSquare(cellData.position.ToVector2(), Color.magenta, 1000f);
                cellData.cellVisualData.containsLight = true;
                cellData.cellVisualData.lightObject = gameObject1;
                LightStampData facewallLightStamp = materialDefinition.facewallLightStamps[outIndex];
                LightStampData sidewallLightStamp = materialDefinition.sidewallLightStamps[outIndex];
                cellData.cellVisualData.facewallLightStampData = facewallLightStamp;
                cellData.cellVisualData.sidewallLightStampData = sidewallLightStamp;
            }
        }

        [DebuggerHidden]
        private IEnumerator GenerateLights(TilemapDecoSettings decoSettings)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new DungeonData__GenerateLightsc__Iterator1()
            {
                decoSettings = decoSettings,
                _this = this
            };
        }

        private void GenerateInterestingVisuals(TilemapDecoSettings dungeonDecoSettings)
        {
            List<IntVector2> patchPoints = new List<IntVector2>();
            for (int x = 0; x < this.Width; ++x)
            {
                for (int y = 0; y < this.Height; ++y)
                {
                    if ((double) UnityEngine.Random.value < (double) dungeonDecoSettings.decoPatchFrequency)
                        patchPoints.Add(new IntVector2(x, y));
                }
            }
            Func<IntVector2, int> func = (Func<IntVector2, int>) (a =>
            {
                int a1 = int.MaxValue;
                for (int index = 0; index < patchPoints.Count; ++index)
                {
                    int b = IntVector2.ManhattanDistance(patchPoints[index], a);
                    a1 = Mathf.Min(a1, b);
                }
                return a1;
            });
            for (int index1 = 0; index1 < this.Width; ++index1)
            {
                for (int index2 = 0; index2 < this.Height; ++index2)
                {
                    CellData cellData = this.cellData[index1][index2];
                    if (cellData != null && (cellData.type != CellType.WALL || cellData.IsLowerFaceWall()) && !cellData.doesDamage)
                    {
                        TilemapDecoSettings.DecoStyle decalLayerStyle = dungeonDecoSettings.decalLayerStyle;
                        int decalSize = dungeonDecoSettings.decalSize;
                        int decalSpacing = dungeonDecoSettings.decalSpacing;
                        TilemapDecoSettings.DecoStyle patternLayerStyle = dungeonDecoSettings.patternLayerStyle;
                        int patternSize = dungeonDecoSettings.patternSize;
                        int patternSpacing = dungeonDecoSettings.patternSpacing;
                        bool flag1 = false;
                        bool flag2 = false;
                        if (cellData.cellVisualData.roomVisualTypeIndex >= 0 && cellData.cellVisualData.roomVisualTypeIndex < GameManager.Instance.Dungeon.roomMaterialDefinitions.Length)
                        {
                            DungeonMaterial materialDefinition = GameManager.Instance.Dungeon.roomMaterialDefinitions[cellData.cellVisualData.roomVisualTypeIndex];
                            if (materialDefinition.usesDecalLayer)
                            {
                                flag1 = true;
                                decalLayerStyle = materialDefinition.decalLayerStyle;
                                decalSize = materialDefinition.decalSize;
                                decalSpacing = materialDefinition.decalSpacing;
                            }
                            if (materialDefinition.usesPatternLayer)
                            {
                                flag2 = true;
                                patternLayerStyle = materialDefinition.patternLayerStyle;
                                patternSize = materialDefinition.patternSize;
                                patternSpacing = materialDefinition.patternSpacing;
                            }
                        }
                        float num1 = (float) ((double) decalSize / 10.0 - 0.34999999403953552);
                        float num2 = (float) ((double) patternSize / 10.0 - 0.34999999403953552);
                        if (flag1)
                        {
                            switch (decalLayerStyle)
                            {
                                case TilemapDecoSettings.DecoStyle.GROW_FROM_WALLS:
                                    if (cellData.HasMossyNeighbor(this) && (double) UnityEngine.Random.value > (double) (10 - decalSize) / 10.0)
                                        cellData.cellVisualData.isDecal = true;
                                    if (cellData.IsLowerFaceWall())
                                    {
                                        cellData.cellVisualData.isDecal = true;
                                        break;
                                    }
                                    break;
                                case TilemapDecoSettings.DecoStyle.PERLIN_NOISE:
                                    if ((double) Noise.Generate((float) cellData.position.x / (4f + (float) decalSpacing), (float) cellData.position.y / (4f + (float) decalSpacing)) < (double) num1)
                                    {
                                        cellData.cellVisualData.isDecal = true;
                                        break;
                                    }
                                    break;
                                case TilemapDecoSettings.DecoStyle.HORIZONTAL_STRIPES:
                                    if (cellData.position.y % (decalSize + decalSpacing) < decalSize)
                                    {
                                        cellData.cellVisualData.isDecal = true;
                                        break;
                                    }
                                    break;
                                case TilemapDecoSettings.DecoStyle.VERTICAL_STRIPES:
                                    if (cellData.position.x % (decalSize + decalSpacing) < decalSize)
                                    {
                                        cellData.cellVisualData.isDecal = true;
                                        break;
                                    }
                                    break;
                                case TilemapDecoSettings.DecoStyle.AROUND_LIGHTS:
                                    if (cellData.cellVisualData.distanceToNearestLight <= decalSize && (double) UnityEngine.Random.value > (double) ((float) cellData.cellVisualData.distanceToNearestLight / ((float) decalSize * 1f)))
                                    {
                                        cellData.cellVisualData.isDecal = true;
                                        break;
                                    }
                                    break;
                                case TilemapDecoSettings.DecoStyle.PATCHES:
                                    int num3 = func(cellData.position);
                                    if (num3 < decalSize || num3 == decalSize && (double) UnityEngine.Random.value > 0.5)
                                    {
                                        cellData.cellVisualData.isDecal = true;
                                        break;
                                    }
                                    break;
                                case TilemapDecoSettings.DecoStyle.NONE:
                                    cellData.cellVisualData.isDecal = false;
                                    break;
                            }
                        }
                        if (flag2)
                        {
                            switch (patternLayerStyle)
                            {
                                case TilemapDecoSettings.DecoStyle.GROW_FROM_WALLS:
                                    if (cellData.HasPatternNeighbor(this) && (double) UnityEngine.Random.value > (double) (10 - patternSize) / 10.0)
                                        cellData.cellVisualData.isPattern = true;
                                    if (cellData.IsLowerFaceWall())
                                    {
                                        cellData.cellVisualData.isPattern = true;
                                        continue;
                                    }
                                    continue;
                                case TilemapDecoSettings.DecoStyle.PERLIN_NOISE:
                                    if ((double) Noise.Generate((float) cellData.position.x / (4f + (float) patternSpacing), (float) cellData.position.y / (4f + (float) patternSpacing)) < (double) num2)
                                    {
                                        cellData.cellVisualData.isPattern = true;
                                        continue;
                                    }
                                    continue;
                                case TilemapDecoSettings.DecoStyle.HORIZONTAL_STRIPES:
                                    if (cellData.position.y % (patternSize + patternSpacing) < patternSize)
                                    {
                                        cellData.cellVisualData.isPattern = true;
                                        continue;
                                    }
                                    continue;
                                case TilemapDecoSettings.DecoStyle.VERTICAL_STRIPES:
                                    if (cellData.position.x % (patternSize + patternSpacing) < patternSize)
                                    {
                                        cellData.cellVisualData.isPattern = true;
                                        continue;
                                    }
                                    continue;
                                case TilemapDecoSettings.DecoStyle.AROUND_LIGHTS:
                                    if (cellData.cellVisualData.distanceToNearestLight <= patternSize)
                                    {
                                        cellData.cellVisualData.isPattern = true;
                                        continue;
                                    }
                                    continue;
                                case TilemapDecoSettings.DecoStyle.PATCHES:
                                    int num4 = func(cellData.position);
                                    if (num4 < patternSize || num4 == patternSize && (double) UnityEngine.Random.value > 0.5)
                                    {
                                        cellData.cellVisualData.isPattern = true;
                                        continue;
                                    }
                                    continue;
                                case TilemapDecoSettings.DecoStyle.NONE:
                                    cellData.cellVisualData.isPattern = false;
                                    continue;
                                default:
                                    continue;
                            }
                        }
                    }
                }
            }
            if (dungeonDecoSettings.patternExpansion > 0)
            {
                for (int index3 = 0; index3 < dungeonDecoSettings.patternExpansion; ++index3)
                {
                    HashSet<CellData> cellDataSet = new HashSet<CellData>();
                    for (int index4 = 0; index4 < this.Width; ++index4)
                    {
                        for (int index5 = 0; index5 < this.Height; ++index5)
                        {
                            CellData cellData = this.cellData[index4][index5];
                            if (cellData != null && !cellData.cellVisualData.isPattern && cellData.HasPatternNeighbor(this) && !cellData.doesDamage)
                                cellDataSet.Add(cellData);
                        }
                    }
                    foreach (CellData cellData in cellDataSet)
                        cellData.cellVisualData.isPattern = true;
                }
            }
            if (dungeonDecoSettings.decalExpansion > 0)
            {
                for (int index6 = 0; index6 < dungeonDecoSettings.decalExpansion; ++index6)
                {
                    HashSet<CellData> cellDataSet = new HashSet<CellData>();
                    for (int index7 = 0; index7 < this.Width; ++index7)
                    {
                        for (int index8 = 0; index8 < this.Height; ++index8)
                        {
                            CellData cellData = this.cellData[index7][index8];
                            if (cellData != null && !cellData.cellVisualData.isDecal && cellData.HasMossyNeighbor(this) && !cellData.doesDamage)
                                cellDataSet.Add(cellData);
                        }
                    }
                    foreach (CellData cellData in cellDataSet)
                        cellData.cellVisualData.isDecal = true;
                }
            }
            if (!dungeonDecoSettings.debug_view)
                return;
            for (int index9 = 0; index9 < this.Width; ++index9)
            {
                for (int index10 = 0; index10 < this.Height; ++index10)
                {
                    CellData cellData = this.cellData[index9][index10];
                    if (cellData != null)
                    {
                        if (cellData.cellVisualData.isDecal && cellData.cellVisualData.isPattern)
                            this.DebugDrawCross(cellData.position.ToCenterVector3(-10f), Color.grey);
                        else if (cellData.cellVisualData.isDecal)
                            this.DebugDrawCross(cellData.position.ToCenterVector3(-10f), Color.green);
                        else if (cellData.cellVisualData.isPattern)
                            this.DebugDrawCross(cellData.position.ToCenterVector3(-10f), Color.red);
                    }
                }
            }
        }

        private void DoRoomDistanceDebug()
        {
            for (int index1 = 0; index1 < this.Width; ++index1)
            {
                for (int index2 = 0; index2 < this.Height; ++index2)
                {
                    CellData cellData = this.cellData[index1][index2];
                    Vector3 centerVector3 = cellData.position.ToCenterVector3(-10f);
                    if ((double) cellData.distanceFromNearestRoom <= (double) Pixelator.Instance.perimeterTileWidth)
                    {
                        Color crosscolor = new Color(cellData.distanceFromNearestRoom / 7f, cellData.distanceFromNearestRoom / 7f, cellData.distanceFromNearestRoom / 7f);
                        this.DebugDrawCross(centerVector3, crosscolor);
                    }
                }
            }
        }

        private void DebugDrawCross(Vector3 centerPoint, Color crosscolor)
        {
            UnityEngine.Debug.DrawLine(centerPoint + new Vector3(-0.5f, 0.0f, 0.0f), centerPoint + new Vector3(0.5f, 0.0f, 0.0f), crosscolor, 1000f);
            UnityEngine.Debug.DrawLine(centerPoint + new Vector3(0.0f, -0.5f, 0.0f), centerPoint + new Vector3(0.0f, 0.5f, 0.0f), crosscolor, 1000f);
        }

        private void FloodFillDungeonInterior()
        {
            Stack<CellData> cellDataStack = new Stack<CellData>();
            for (int index = 0; index < this.rooms.Count; ++index)
            {
                if (this.rooms[index] == this.Entrance || this.rooms[index].IsStartOfWarpWing)
                    cellDataStack.Push(this[this.rooms[index].GetRandomAvailableCellDumb()]);
            }
            while (cellDataStack.Count > 0)
            {
                CellData d = cellDataStack.Pop();
                if (d.type != CellType.WALL)
                {
                    List<CellData> cellNeighbors = this.GetCellNeighbors(d);
                    d.isGridConnected = true;
                    for (int index = 0; index < cellNeighbors.Count; ++index)
                    {
                        if (cellNeighbors[index] != null && cellNeighbors[index].type != CellType.WALL && !cellNeighbors[index].isGridConnected)
                            cellDataStack.Push(cellNeighbors[index]);
                    }
                }
            }
        }

        private void FloodFillDungeonExterior()
        {
            Stack<IntVector2> intVector2Stack = new Stack<IntVector2>();
            HashSet<IntVector2> intVector2Set = new HashSet<IntVector2>();
            intVector2Stack.Push(IntVector2.Zero);
            while (intVector2Stack.Count > 0)
            {
                IntVector2 key = intVector2Stack.Pop();
                intVector2Set.Add(key);
                CellData cellData = this[key];
                if (cellData == null || cellData.type == CellType.WALL && !cellData.breakable || cellData.isExitCell)
                {
                    if (cellData != null)
                        cellData.isRoomInternal = false;
                    for (int index = 0; index < IntVector2.Cardinals.Length; ++index)
                    {
                        IntVector2 intVector2 = key + IntVector2.Cardinals[index];
                        if (!intVector2Set.Contains(intVector2) && intVector2.x >= 0 && intVector2.y >= 0 && intVector2.x < this.Width && intVector2.y < this.Height)
                            intVector2Stack.Push(intVector2);
                    }
                }
            }
        }

        private void ComputeRoomDistanceData()
        {
            Queue<CellData> cellDataQueue = new Queue<CellData>();
            HashSet<CellData> cellDataSet = new HashSet<CellData>();
            for (int index1 = 0; index1 < this.cellData.Length; ++index1)
            {
                for (int index2 = 0; index2 < this.cellData[index1].Length; ++index2)
                {
                    CellData cellData = this.cellData[index1][index2];
                    if (cellData != null && (double) cellData.distanceFromNearestRoom == 1.0)
                    {
                        cellDataQueue.Enqueue(cellData);
                        cellDataSet.Add(cellData);
                    }
                }
            }
            while (cellDataQueue.Count > 0)
            {
                CellData d = cellDataQueue.Dequeue();
                cellDataSet.Remove(d);
                List<CellData> cellNeighbors = this.GetCellNeighbors(d, true);
                for (int index = 0; index < cellNeighbors.Count; ++index)
                {
                    CellData cellData = cellNeighbors[index];
                    if (cellData != null)
                    {
                        float num = index % 2 != 1 ? d.distanceFromNearestRoom + 1f : d.distanceFromNearestRoom + 1.414f;
                        if ((double) cellData.distanceFromNearestRoom > (double) num)
                        {
                            cellData.distanceFromNearestRoom = num;
                            cellData.nearestRoom = d.nearestRoom;
                            if (!cellDataSet.Contains(cellData))
                            {
                                cellDataQueue.Enqueue(cellData);
                                cellDataSet.Add(cellData);
                            }
                        }
                    }
                }
            }
        }

        private void CalculatePerRoomOcclusionData()
        {
        }

        private HashSet<CellData> ComputeRoomDistanceHorizon(HashSet<CellData> horizon, float dist)
        {
            float num1 = dist;
            HashSet<CellData> roomDistanceHorizon = new HashSet<CellData>();
            foreach (CellData d in horizon)
            {
                List<CellData> cellNeighbors = this.GetCellNeighbors(d, true);
                for (int index = 0; index < cellNeighbors.Count; ++index)
                {
                    CellData cellData = cellNeighbors[index];
                    if (cellData != null && !roomDistanceHorizon.Contains(cellData))
                    {
                        float num2 = index % 2 != 1 ? num1 + 1f : num1 + 1.414f;
                        if ((double) cellData.distanceFromNearestRoom > (double) num2)
                        {
                            cellData.distanceFromNearestRoom = num2;
                            cellData.nearestRoom = d.nearestRoom;
                            roomDistanceHorizon.Add(cellData);
                        }
                    }
                }
            }
            return roomDistanceHorizon;
        }

        public void CheckIntegrity()
        {
            for (int x = 0; x < this.Width; ++x)
            {
                for (int y = 0; y < this.Height; ++y)
                {
                    if (y > 1 && y < this.Height - 1 && this.cellData[x][y + 1] != null && this.isSingleCellWall(x, y))
                    {
                        this.cellData[x][y + 1].type = CellType.WALL;
                        RoomHandler parentRoom = this.cellData[x][y].parentRoom;
                        if (parentRoom != null)
                        {
                            IntVector2 intVector2 = new IntVector2(x, y + 1);
                            if (parentRoom.RawCells.Remove(intVector2))
                            {
                                parentRoom.Cells.Remove(intVector2);
                                parentRoom.CellsWithoutExits.Remove(intVector2);
                            }
                        }
                    }
                    if (this.cellData[x][y] != null && this.cellData[x][y].type == CellType.FLOOR)
                    {
                        bool flag = false;
                        foreach (CellData cellNeighbor in this.GetCellNeighbors(this.cellData[x][y]))
                        {
                            if (cellNeighbor != null && cellNeighbor.type == CellType.FLOOR)
                                flag = true;
                        }
                        if (!flag)
                            this.cellData[x][y].type = CellType.WALL;
                    }
                }
            }
            this.ExciseElbows();
        }

        protected void ExciseElbows()
        {
            bool flag1 = true;
            List<CellData> cellDataList = new List<CellData>();
            int num1 = 0;
            while (flag1 && num1 < 1000)
            {
                ++num1;
                cellDataList.Clear();
                flag1 = false;
                for (int x = 0; x < this.Width; ++x)
                {
                    for (int y = 0; y < this.Height; ++y)
                    {
                        if (this.cellData[x][y] != null && (this.cellData[x][y].isExitCell || this.cellData[x][y].parentRoom != null && this.cellData[x][y].parentRoom.area.IsProceduralRoom))
                        {
                            if (this.cellData[x][y].isExitCell && !GameManager.Instance.Dungeon.UsesWallWarpWingDoors)
                            {
                                bool flag2 = false;
                                RoomHandler roomFromPosition = this.GetAbsoluteRoomFromPosition(new IntVector2(x, y));
                                foreach (RoomHandler connectedRoom in roomFromPosition.connectedRooms)
                                {
                                    RuntimeExitDefinition forConnectedRoom = roomFromPosition.GetExitDefinitionForConnectedRoom(connectedRoom);
                                    if ((forConnectedRoom.upstreamExit != null && forConnectedRoom.upstreamExit.isWarpWingStart || forConnectedRoom.downstreamExit != null && forConnectedRoom.downstreamExit.isWarpWingStart) && forConnectedRoom.ContainsPosition(new IntVector2(x, y)))
                                    {
                                        flag2 = true;
                                        break;
                                    }
                                }
                                if (flag2)
                                    continue;
                            }
                            int num2 = 0;
                            int num3 = 0;
                            for (int index = 0; index < 4; ++index)
                            {
                                if (this[new IntVector2(x, y) + IntVector2.Cardinals[index]].type != CellType.WALL)
                                {
                                    ++num2;
                                    if (this[new IntVector2(x, y) + 2 * IntVector2.Cardinals[index]].type != CellType.WALL)
                                        ++num3;
                                }
                            }
                            if (num2 == 2 && num3 != 2)
                                cellDataList.Add(this.cellData[x][y]);
                        }
                    }
                }
                if (cellDataList.Count > 0)
                    flag1 = true;
                foreach (CellData cellData in cellDataList)
                {
                    BraveUtility.DrawDebugSquare(cellData.position.ToVector2(), Color.yellow, 1000f);
                    cellData.type = CellType.WALL;
                    for (int index1 = 0; index1 < this.rooms.Count; ++index1)
                    {
                        RoomHandler room = this.rooms[index1];
                        room.RawCells.Remove(cellData.position);
                        room.Cells.Remove(cellData.position);
                        room.CellsWithoutExits.Remove(cellData.position);
                        if (room.area.instanceUsedExits != null)
                        {
                            for (int index2 = 0; index2 < room.area.instanceUsedExits.Count; ++index2)
                            {
                                if (room.area.exitToLocalDataMap.ContainsKey(room.area.instanceUsedExits[index2]))
                                    room.exitDefinitionsByExit[room.area.exitToLocalDataMap[room.area.instanceUsedExits[index2]]].RemovePosition(cellData.position);
                            }
                        }
                    }
                }
            }
        }

        public int GetRoomVisualTypeAtPosition(Vector2 position)
        {
            return this.GetRoomVisualTypeAtPosition(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y));
        }

        public int GetRoomVisualTypeAtPosition(IntVector2 position)
        {
            return this[position].cellVisualData.roomVisualTypeIndex;
        }

        public int GetRoomVisualTypeAtPosition(int x, int y)
        {
            return x < 0 || x >= this.Width || y < 0 || y >= this.Height || this.cellData[x][y] == null ? 0 : this.cellData[x][y].cellVisualData.roomVisualTypeIndex;
        }

        public void FakeRegisterDoorFeet(IntVector2 position, bool northSouth)
        {
            if (northSouth)
            {
                this[position].cellVisualData.doorFeetOverrideMode = 1;
                this[position + IntVector2.Right].cellVisualData.doorFeetOverrideMode = 1;
            }
            else
            {
                this[position].cellVisualData.doorFeetOverrideMode = 2;
                this[position + IntVector2.Up].cellVisualData.doorFeetOverrideMode = 2;
            }
        }

        public void RegisterDoor(
            IntVector2 position,
            DungeonDoorController door,
            IntVector2 subsidiaryDoorPosition)
        {
            if (this.doors == null)
                this.doors = new Dictionary<IntVector2, DungeonDoorController>((IEqualityComparer<IntVector2>) new IntVector2EqualityComparer());
            if (this.doors.ContainsKey(position))
                return;
            this.doors.Add(position, door);
            this[position].isDoorFrameCell = true;
            if (door.northSouth)
            {
                this.doors.Add(position + IntVector2.Right, door);
                this[position + IntVector2.Right].isDoorFrameCell = true;
                this[position + IntVector2.Up].isDoorFrameCell = true;
                this[position + IntVector2.UpRight].isDoorFrameCell = true;
                this[position].isExitNonOccluder = true;
                this[position + IntVector2.Right].isExitNonOccluder = true;
            }
            else
            {
                this.doors.Add(position + IntVector2.Up, door);
                this[position + IntVector2.Up].isDoorFrameCell = true;
                for (int x = -2; x < 3; ++x)
                {
                    this[position + new IntVector2(x, 1)].isExitNonOccluder = true;
                    if (Math.Abs(x) < 2)
                    {
                        this[position + IntVector2.Right * x].isExitNonOccluder = true;
                        this[position + new IntVector2(x, 2)].isExitNonOccluder = true;
                    }
                }
            }
            if (!((UnityEngine.Object) door.subsidiaryDoor != (UnityEngine.Object) null))
                return;
            this.doors.Add(subsidiaryDoorPosition, door.subsidiaryDoor);
            this[subsidiaryDoorPosition].isDoorFrameCell = true;
            if (door.subsidiaryDoor.northSouth)
            {
                this.doors.Add(subsidiaryDoorPosition + IntVector2.Right, door.subsidiaryDoor);
                this[subsidiaryDoorPosition + IntVector2.Right].isDoorFrameCell = true;
                this[subsidiaryDoorPosition + IntVector2.Up].isDoorFrameCell = true;
                this[subsidiaryDoorPosition + IntVector2.UpRight].isDoorFrameCell = true;
            }
            else
            {
                this.doors.Add(subsidiaryDoorPosition + IntVector2.Up, door.subsidiaryDoor);
                this[subsidiaryDoorPosition + IntVector2.Up].isDoorFrameCell = true;
            }
        }

        public DungeonDoorController GetDoorAtPosition(IntVector2 position)
        {
            if (this.doors == null)
                return (DungeonDoorController) null;
            return !this.doors.ContainsKey(position) ? (DungeonDoorController) null : this.doors[position];
        }

        public bool HasDoorAtPosition(IntVector2 position)
        {
            return this.doors != null && this.doors.ContainsKey(position);
        }

        public void DestroyDoorAtPosition(IntVector2 position)
        {
            DungeonDoorController door = this.doors[position];
            this.doors.Remove(position);
            if (door.northSouth)
                this.doors.Remove(position + IntVector2.Right);
            else
                this.doors.Remove(position + IntVector2.Up);
            UnityEngine.Object.Destroy((UnityEngine.Object) door.gameObject);
        }

        public List<CellData> GetCellNeighbors(CellData d, bool getDiagonals = false)
        {
            if (!getDiagonals)
                ;
            DungeonData.s_neighborsList.Clear();
            int num = !getDiagonals ? 2 : 1;
            for (int dir = 0; dir < 8; dir += num)
                DungeonData.s_neighborsList.Add(this.GetCellInDirection(d, (DungeonData.Direction) dir));
            return DungeonData.s_neighborsList;
        }

        public bool CheckLineForCellType(IntVector2 p1, IntVector2 p2, CellType t)
        {
            List<CellData> linearCells = this.GetLinearCells(p1, p2);
            for (int index = 0; index < linearCells.Count; ++index)
            {
                if (linearCells[index].type == t)
                    return true;
            }
            return false;
        }

        public List<CellData> GetLinearCells(IntVector2 p1, IntVector2 p2)
        {
            HashSet<CellData> collection = new HashSet<CellData>();
            int y = p1.y;
            int x = p1.x;
            int num1 = p2.x - p1.x;
            int num2 = p2.y - p1.y;
            collection.Add(this.cellData[x][y]);
            int num3;
            if (num2 < 0)
            {
                num3 = -1;
                num2 = -num2;
            }
            else
                num3 = 1;
            int num4;
            if (num1 < 0)
            {
                num4 = -1;
                num1 = -num1;
            }
            else
                num4 = 1;
            int num5 = 2 * num2;
            int num6 = 2 * num1;
            if (num6 >= num5)
            {
                int num7 = num1;
                int num8 = num1;
                for (int index = 0; index < num1; ++index)
                {
                    x += num4;
                    num7 += num5;
                    if (num7 > num6)
                    {
                        y += num3;
                        num7 -= num6;
                        if (num7 + num8 < num6)
                            collection.Add(this.cellData[x][y - num3]);
                        else if (num7 + num8 > num6)
                        {
                            collection.Add(this.cellData[x - num4][y]);
                        }
                        else
                        {
                            collection.Add(this.cellData[x][y - num3]);
                            collection.Add(this.cellData[x - num4][y]);
                        }
                    }
                    collection.Add(this.cellData[x][y]);
                    num8 = num7;
                }
            }
            else
            {
                int num9 = num2;
                int num10 = num9;
                for (int index = 0; index < num2; ++index)
                {
                    y += num3;
                    num9 += num6;
                    if (num9 > num5)
                    {
                        x += num4;
                        num9 -= num5;
                        if (num9 + num10 < num5)
                            collection.Add(this.cellData[x - num4][y]);
                        else if (num9 + num10 > num5)
                        {
                            collection.Add(this.cellData[x][y - num3]);
                        }
                        else
                        {
                            collection.Add(this.cellData[x - num4][y]);
                            collection.Add(this.cellData[x][y - num3]);
                        }
                    }
                    collection.Add(this.cellData[x][y]);
                    num10 = num9;
                }
            }
            return new List<CellData>((IEnumerable<CellData>) collection);
        }

        public CellData GetCellInDirection(CellData d, DungeonData.Direction dir)
        {
            IntVector2 position = d.position;
            switch (dir)
            {
                case DungeonData.Direction.NORTH:
                    position += IntVector2.Up;
                    break;
                case DungeonData.Direction.NORTHEAST:
                    position += IntVector2.Up + IntVector2.Right;
                    break;
                case DungeonData.Direction.EAST:
                    position += IntVector2.Right;
                    break;
                case DungeonData.Direction.SOUTHEAST:
                    position += IntVector2.Right + IntVector2.Down;
                    break;
                case DungeonData.Direction.SOUTH:
                    position += IntVector2.Down;
                    break;
                case DungeonData.Direction.SOUTHWEST:
                    position += IntVector2.Down + IntVector2.Left;
                    break;
                case DungeonData.Direction.WEST:
                    position += IntVector2.Left;
                    break;
                case DungeonData.Direction.NORTHWEST:
                    position += IntVector2.Left + IntVector2.Up;
                    break;
                default:
                    UnityEngine.Debug.LogError((object) ("Switching on invalid direction in GetCellInDirection: " + dir.ToString()));
                    break;
            }
            return !this.CheckInBounds(position) ? (CellData) null : this.cellData[position.x][position.y];
        }

        public bool CheckInBounds(int x, int y) => x >= 0 && x < this.Width && y >= 0 && y < this.Height;

        public bool CheckInBounds(IntVector2 vec)
        {
            return vec.x >= 0 && vec.x < this.Width && vec.y >= 0 && vec.y < this.Height;
        }

        public bool CheckInBoundsAndValid(int x, int y)
        {
            return x >= 0 && x < this.Width && y >= 0 && y < this.Height && this[x, y] != null;
        }

        public bool CheckInBoundsAndValid(IntVector2 vec)
        {
            return vec.x >= 0 && vec.x < this.Width && vec.y >= 0 && vec.y < this.Height && this[vec] != null;
        }

        public bool CheckInBounds(IntVector2 vec, int distanceThresh)
        {
            return vec.x >= distanceThresh && vec.x < this.Width - distanceThresh && vec.y >= distanceThresh && vec.y < this.Height - distanceThresh;
        }

        public void DistributeComplexSecretPuzzleItems(
            List<PickupObject> requiredObjects,
            RoomHandler puzzleRoom,
            bool preferSignatureEnemies = false,
            float preferBossesChance = 0.0f)
        {
            int index1 = 0;
            bool flag = (double) UnityEngine.Random.value < (double) preferBossesChance;
            List<AIActor> aiActorList1 = new List<AIActor>();
            List<AIActor> aiActorList2 = new List<AIActor>();
            List<AIActor> aiActorList3 = new List<AIActor>();
            for (int index2 = 0; index2 < StaticReferenceManager.AllEnemies.Count; ++index2)
            {
                AIActor allEnemy = StaticReferenceManager.AllEnemies[index2];
                if (allEnemy.IsNormalEnemy && !allEnemy.IsHarmlessEnemy && !allEnemy.IsInReinforcementLayer && (bool) (UnityEngine.Object) allEnemy.healthHaver && (puzzleRoom == null || allEnemy.ParentRoom != puzzleRoom))
                {
                    if (allEnemy.healthHaver.IsBoss)
                    {
                        aiActorList3.Add(allEnemy);
                    }
                    else
                    {
                        aiActorList1.Add(StaticReferenceManager.AllEnemies[index2]);
                        if (StaticReferenceManager.AllEnemies[index2].IsSignatureEnemy && preferSignatureEnemies)
                            aiActorList2.Add(StaticReferenceManager.AllEnemies[index2]);
                    }
                }
            }
            AIActor target = (AIActor) null;
            for (; index1 < requiredObjects.Count; ++index1)
            {
                if (flag && aiActorList3.Count > 0)
                    target = aiActorList3[UnityEngine.Random.Range(0, aiActorList3.Count)];
                else if (aiActorList2.Count > 0)
                    target = aiActorList2[UnityEngine.Random.Range(0, aiActorList2.Count)];
                else if (aiActorList1.Count > 0)
                    target = aiActorList1[UnityEngine.Random.Range(0, aiActorList1.Count)];
                if ((UnityEngine.Object) target != (UnityEngine.Object) null)
                {
                    target.AdditionalSimpleItemDrops.Add(requiredObjects[index1]);
                    if (requiredObjects[index1] is RobotArmBalloonsItem)
                        (requiredObjects[index1] as RobotArmBalloonsItem).AttachBalloonToGameActor((GameActor) target);
                    aiActorList3.Remove(target);
                    aiActorList2.Remove(target);
                    aiActorList1.Remove(target);
                }
                else
                {
                    UnityEngine.Debug.LogError((object) "Failed to attach an item to any enemy!");
                    break;
                }
            }
        }

        public void SolidifyLavaInRadius(Vector2 position, float radius)
        {
            int num = Mathf.CeilToInt(radius);
            IntVector2 intVector2_1 = position.ToIntVector2(VectorConversions.Floor) + new IntVector2(-num, -num);
            IntVector2 intVector2_2 = position.ToIntVector2(VectorConversions.Ceil) + new IntVector2(num, num);
            for (int x = intVector2_1.x; x <= intVector2_2.x; ++x)
            {
                for (int y = intVector2_1.y; y <= intVector2_2.y; ++y)
                {
                    Vector2 b = new Vector2((float) x + 0.5f, (float) y + 0.5f);
                    if ((double) Vector2.Distance(position, b) <= (double) radius)
                        this.SolidifyLavaInCell(new IntVector2(x, y));
                }
            }
        }

        private void InitializeSizzleSystem()
        {
            GameObject gameObject = GameObject.Find("Gungeon_Sizzle_Main");
            if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
            {
                gameObject = (GameObject) UnityEngine.Object.Instantiate(BraveResources.Load("Particles/Gungeon_Sizzle_Main"), Vector3.zero, Quaternion.identity);
                gameObject.name = "Gungeon_Sizzle_Main";
            }
            this.m_sizzleSystem = gameObject.GetComponent<ParticleSystem>();
        }

        private void SpawnWorldParticle(ParticleSystem system, Vector3 worldPos)
        {
            system.Emit(new ParticleSystem.EmitParams()
            {
                position = worldPos,
                velocity = Vector3.zero,
                startSize = system.startSize,
                startLifetime = system.startLifetime,
                startColor = (Color32) system.startColor,
                randomSeed = (uint) ((double) UnityEngine.Random.value * 4294967296.0)
            }, 1);
        }

        public void SolidifyLavaInCell(IntVector2 cellPosition)
        {
            if (!this.CheckInBounds(cellPosition))
                return;
            CellData cellData = this[cellPosition];
            if (cellData == null || !cellData.doesDamage)
                return;
            cellData.doesDamage = false;
            if ((UnityEngine.Object) this.m_sizzleSystem == (UnityEngine.Object) null)
                this.InitializeSizzleSystem();
            this.SpawnWorldParticle(this.m_sizzleSystem, cellPosition.ToCenterVector3((float) cellPosition.y) + UnityEngine.Random.insideUnitCircle.ToVector3ZUp() / 3f);
            if ((double) UnityEngine.Random.value >= 0.5)
                return;
            this.SpawnWorldParticle(this.m_sizzleSystem, cellPosition.ToCenterVector3((float) cellPosition.y) + UnityEngine.Random.insideUnitCircle.ToVector3ZUp() / 3f);
        }

        public void TriggerFloorAnimationsInCell(IntVector2 cellPosition)
        {
            if (!this.CheckInBounds(cellPosition))
                return;
            CellData cellData = this[cellPosition];
            if (cellData == null || cellData.type != CellType.FLOOR || !TK2DTilemapChunkAnimator.PositionToAnimatorMap.ContainsKey(cellPosition))
                return;
            for (int index = 0; index < TK2DTilemapChunkAnimator.PositionToAnimatorMap[cellPosition].Count; ++index)
                TK2DTilemapChunkAnimator.PositionToAnimatorMap[cellPosition][index].TriggerAnimationSequence();
        }

        public void UntriggerFloorAnimationsInCell(IntVector2 cellPosition)
        {
            if (!this.CheckInBounds(cellPosition))
                return;
            CellData cellData = this[cellPosition];
            if (cellData == null || cellData.type != CellType.FLOOR || !TK2DTilemapChunkAnimator.PositionToAnimatorMap.ContainsKey(cellPosition))
                return;
            for (int index = 0; index < TK2DTilemapChunkAnimator.PositionToAnimatorMap[cellPosition].Count; ++index)
                TK2DTilemapChunkAnimator.PositionToAnimatorMap[cellPosition][index].UntriggerAnimationSequence();
        }

        public void GenerateLocksAndKeys(RoomHandler start, RoomHandler exit)
        {
            this.PlaceSingleLockAndKey(new ReducedDungeonGraph());
        }

        public void PlaceSingleLockAndKey(ReducedDungeonGraph graph)
        {
        }

        public void GetObjectOcclusionAndObstruction(
            Vector2 sourcePoint,
            Vector2 listenerPoint,
            out float occlusion,
            out float obstruction)
        {
            occlusion = 0.0f;
            obstruction = 0.0f;
            IntVector2 intVector2_1 = sourcePoint.ToIntVector2(VectorConversions.Floor);
            IntVector2 intVector2_2 = listenerPoint.ToIntVector2(VectorConversions.Floor);
            if (!this.CheckInBounds(intVector2_1) || !this.CheckInBounds(intVector2_2))
                return;
            RoomHandler roomFromPosition1 = this.GetAbsoluteRoomFromPosition(intVector2_1);
            RoomHandler roomFromPosition2 = this.GetAbsoluteRoomFromPosition(intVector2_2);
            if (roomFromPosition1 == null || roomFromPosition2 == null || roomFromPosition1 == roomFromPosition2)
                return;
            occlusion = 0.5f;
            obstruction = 0.5f;
        }

        public bool isDeadEnd(int x, int y)
        {
            int num = 0;
            if (this.CheckInBounds(x, y - 1) && this.cellData[x][y - 1].type == CellType.FLOOR)
                ++num;
            if (this.CheckInBounds(x + 1, y) && this.cellData[x + 1][y].type == CellType.FLOOR)
                ++num;
            if (this.CheckInBounds(x, y + 1) && this.cellData[x][y + 1].type == CellType.FLOOR)
                ++num;
            if (this.CheckInBounds(x - 1, y) && this.cellData[x - 1][y].type == CellType.FLOOR)
                ++num;
            return num <= 1;
        }

        public bool isSingleCellWall(int x, int y)
        {
            return this.CheckInBounds(x, y) && this.CheckInBounds(x, y - 1) && this.CheckInBounds(x, y + 1) && this.cellData[x][y] != null && this.cellData[x][y - 1] != null && this.cellData[x][y + 1] != null && this.cellData[x][y].type == CellType.WALL && this.cellData[x][y - 1].type != CellType.WALL && this.cellData[x][y + 1].type != CellType.WALL;
        }

        public bool isTopDiagonalWall(int x, int y)
        {
            if (!this.isFaceWallHigher(x, y - 1))
                return false;
            return this.cellData[x][y].diagonalWallType == DiagonalWallType.NORTHEAST || this.cellData[x][y].diagonalWallType == DiagonalWallType.NORTHWEST;
        }

        public bool isAnyFaceWall(int x, int y)
        {
            return this.isFaceWallLower(x, y) || this.isFaceWallHigher(x, y);
        }

        public bool isFaceWallLower(int x, int y)
        {
            return this.CheckInBoundsAndValid(x, y) && this.CheckInBoundsAndValid(x, y - 1) && this.cellData[x][y].type == CellType.WALL && this.cellData[x][y - 1].type != CellType.WALL;
        }

        public bool isFaceWallHigher(int x, int y)
        {
            return this.CheckInBoundsAndValid(x, y) && this.CheckInBoundsAndValid(x, y - 1) && this.CheckInBoundsAndValid(x, y - 2) && this.cellData[x][y].type == CellType.WALL && this.cellData[x][y - 1].type == CellType.WALL && this.cellData[x][y - 2].type != CellType.WALL;
        }

        public bool isPlainEmptyCell(int x, int y)
        {
            if (!this.CheckInBounds(x, y))
                return false;
            CellData cellData = this.cellData[x][y];
            return cellData != null && !cellData.isExitCell && !this.isTopWall(x, y) && !cellData.isOccupied && cellData.IsPassable && !cellData.containsTrap && !cellData.IsTrapZone && !cellData.PreventRewardSpawn && !cellData.doesDamage && !cellData.cellVisualData.hasStampedPath & !cellData.forceDisallowGoop;
        }

        public bool isWallUp(int x, int y)
        {
            return this.CheckInBoundsAndValid(x, y + 1) && this.cellData[x][y + 1].type == CellType.WALL;
        }

        public bool isWall(int x, int y)
        {
            if (!this.CheckInBounds(x, y))
                return false;
            return this.cellData[x][y] == null || this.cellData[x][y].type == CellType.WALL;
        }

        public bool isWall(IntVector2 pos)
        {
            if (!this.CheckInBounds(pos))
                return false;
            return this.cellData[pos.x][pos.y] == null || this.cellData[pos.x][pos.y].type == CellType.WALL;
        }

        public bool isWallRight(int x, int y)
        {
            return this.CheckInBoundsAndValid(x + 1, y) && this.cellData[x + 1][y].type == CellType.WALL;
        }

        public bool isWallLeft(int x, int y)
        {
            return this.CheckInBoundsAndValid(x - 1, y) && this.cellData[x - 1][y].type == CellType.WALL;
        }

        public bool isWallUpRight(int x, int y)
        {
            return this.CheckInBoundsAndValid(x + 1, y + 1) && this.cellData[x + 1][y + 1].type == CellType.WALL;
        }

        public bool isWallUpLeft(int x, int y)
        {
            return this.CheckInBoundsAndValid(x - 1, y + 1) && this.cellData[x - 1][y + 1].type == CellType.WALL;
        }

        public bool isWallDownRight(int x, int y)
        {
            return this.CheckInBoundsAndValid(x + 1, y - 1) && this.cellData[x + 1][y - 1].type == CellType.WALL;
        }

        public bool isWallDownLeft(int x, int y)
        {
            return this.CheckInBoundsAndValid(x - 1, y - 1) && this.cellData[x - 1][y - 1].type == CellType.WALL;
        }

        public bool isFaceWallRight(int x, int y) => this.isAnyFaceWall(x + 1, y);

        public bool isFaceWallLeft(int x, int y) => this.isAnyFaceWall(x - 1, y);

        public bool isShadowFloor(int x, int y)
        {
            return this.CheckInBoundsAndValid(x, y) && this.CheckInBoundsAndValid(x, y + 1) && this.cellData[x][y].type == CellType.FLOOR && this.cellData[x][y + 1].type == CellType.WALL;
        }

        public bool isTopWall(int x, int y)
        {
            return this.CheckInBoundsAndValid(x, y) && this.CheckInBoundsAndValid(x, y - 1) && this.cellData[x][y].type != CellType.WALL && this.cellData[x][y - 1].type == CellType.WALL;
        }

        public bool isLeftTopWall(int x, int y)
        {
            return this.CheckInBoundsAndValid(x - 1, y) && this.CheckInBoundsAndValid(x - 1, y - 1) && this.cellData[x - 1][y].type != CellType.WALL && this.cellData[x - 1][y - 1].type == CellType.WALL;
        }

        public bool isRightTopWall(int x, int y)
        {
            return this.CheckInBoundsAndValid(x + 1, y) && this.CheckInBoundsAndValid(x + 1, y - 1) && this.cellData[x + 1][y].type != CellType.WALL && this.cellData[x + 1][y - 1].type == CellType.WALL;
        }

        public bool hasTopWall(int x, int y)
        {
            return this.CheckInBoundsAndValid(x, y) && this.CheckInBoundsAndValid(x, y + 1) && this.cellData[x][y].type == CellType.WALL && this.cellData[x][y + 1].type != CellType.WALL;
        }

        public bool leftHasTopWall(int x, int y)
        {
            return this.CheckInBoundsAndValid(x - 1, y) && this.CheckInBoundsAndValid(x - 1, y + 1) && this.cellData[x - 1][y].type == CellType.WALL && this.cellData[x - 1][y + 1].type != CellType.WALL;
        }

        public bool rightHasTopWall(int x, int y)
        {
            return this.CheckInBoundsAndValid(x + 1, y) && this.CheckInBoundsAndValid(x + 1, y + 1) && this.cellData[x + 1][y].type == CellType.WALL && this.cellData[x + 1][y + 1].type != CellType.WALL;
        }

        public bool isPit(int x, int y)
        {
            if (!this.CheckInBoundsAndValid(x, y))
                return false;
            CellData cellData = this.cellData[x][y];
            return cellData != null && cellData.type == CellType.PIT;
        }

        public bool isLeftSideWall(int x, int y) => this.isWall(x, y) && !this.isWall(x + 1, y);

        public bool isRightSideWall(int x, int y) => this.isWall(x, y) && !this.isWall(x - 1, y);

        public enum Direction
        {
            NORTH,
            NORTHEAST,
            EAST,
            SOUTHEAST,
            SOUTH,
            SOUTHWEST,
            WEST,
            NORTHWEST,
        }

        public enum LightGenerationStyle
        {
            STANDARD,
            FORCE_COLOR,
            RAT_HALLWAY,
        }
    }
}
