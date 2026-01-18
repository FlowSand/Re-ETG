using System.Collections.Generic;

using UnityEngine;

using Dungeonator;

#nullable disable

public class RobotDave
    {
        private static DungeonPlaceable m_trapData;
        private static DungeonPlaceable m_horizontalTable;
        private static DungeonPlaceable m_verticalTable;

        public static RobotRoomFeature GetFeatureFromType(RobotDave.RobotFeatureType type)
        {
            switch (type)
            {
                case RobotDave.RobotFeatureType.FLAT_EXPANSE:
                    return (RobotRoomFeature) new FlatExpanseFeature();
                case RobotDave.RobotFeatureType.COLUMN_SAWBLADE:
                    return (RobotRoomFeature) new RobotRoomColumnFeature();
                case RobotDave.RobotFeatureType.PIT_BORDER:
                    return (RobotRoomFeature) new RobotRoomSurroundingPitFeature();
                case RobotDave.RobotFeatureType.TRAP_PLUS:
                    return (RobotRoomFeature) new RobotRoomTrapPlusFeature();
                case RobotDave.RobotFeatureType.TRAP_SQUARE:
                    return (RobotRoomFeature) new RobotRoomTrapSquareFeature();
                case RobotDave.RobotFeatureType.CORNER_COLUMNS:
                    return (RobotRoomFeature) new RobotRoomCornerColumnsFeature();
                case RobotDave.RobotFeatureType.PIT_INNER:
                    return (RobotRoomFeature) new RobotRoomInnerPitFeature();
                case RobotDave.RobotFeatureType.TABLES_EDGE:
                    return (RobotRoomFeature) new RobotRoomTablesFeature();
                case RobotDave.RobotFeatureType.ROLLING_LOG_VERTICAL:
                    return (RobotRoomFeature) new RobotRoomRollingLogsVerticalFeature();
                case RobotDave.RobotFeatureType.ROLLING_LOG_HORIZONTAL:
                    return (RobotRoomFeature) new RobotRoomRollingLogsHorizontalFeature();
                case RobotDave.RobotFeatureType.CASTLE_CHANDELIER:
                    return (RobotRoomFeature) new RobotRoomChandelierFeature();
                case RobotDave.RobotFeatureType.MINES_CAVE_IN:
                    return (RobotRoomFeature) new RobotRoomCaveInFeature();
                case RobotDave.RobotFeatureType.MINES_SQUARE_CART:
                    return (RobotRoomFeature) new RobotRoomMineCartSquareFeature();
                case RobotDave.RobotFeatureType.MINES_DOUBLE_CART:
                    return (RobotRoomFeature) new RobotRoomMineCartSquareDoubleFeature();
                case RobotDave.RobotFeatureType.MINES_TURRET_CART:
                    return (RobotRoomFeature) new RobotRoomMineCartSquareTurretFeature();
                case RobotDave.RobotFeatureType.CONVEYOR_HORIZONTAL:
                    return (RobotRoomFeature) new RobotRoomConveyorHorizontalFeature();
                case RobotDave.RobotFeatureType.CONVEYOR_VERTICAL:
                    return (RobotRoomFeature) new RobotRoomConveyorVerticalFeature();
                default:
                    return (RobotRoomFeature) new FlatExpanseFeature();
            }
        }

        public static DungeonPlaceableBehaviour GetPitTrap()
        {
            if ((Object) RobotDave.m_trapData == (Object) null)
                RobotDave.m_trapData = (DungeonPlaceable) BraveResources.Load("RobotDaveTraps", ".asset");
            return RobotDave.m_trapData.variantTiers[1].nonDatabasePlaceable.GetComponent<DungeonPlaceableBehaviour>();
        }

        public static DungeonPlaceableBehaviour GetSpikesTrap()
        {
            if ((Object) RobotDave.m_trapData == (Object) null)
                RobotDave.m_trapData = (DungeonPlaceable) BraveResources.Load("RobotDaveTraps", ".asset");
            return RobotDave.m_trapData.variantTiers[2].nonDatabasePlaceable.GetComponent<DungeonPlaceableBehaviour>();
        }

        public static DungeonPlaceableBehaviour GetFloorFlameTrap()
        {
            if ((Object) RobotDave.m_trapData == (Object) null)
                RobotDave.m_trapData = (DungeonPlaceable) BraveResources.Load("RobotDaveTraps", ".asset");
            return RobotDave.m_trapData.variantTiers[3].nonDatabasePlaceable.GetComponent<DungeonPlaceableBehaviour>();
        }

        public static DungeonPlaceableBehaviour GetSawbladePrefab()
        {
            if ((Object) RobotDave.m_trapData == (Object) null)
                RobotDave.m_trapData = (DungeonPlaceable) BraveResources.Load("RobotDaveTraps", ".asset");
            return RobotDave.m_trapData.variantTiers[0].nonDatabasePlaceable.GetComponent<DungeonPlaceableBehaviour>();
        }

        public static DungeonPlaceableBehaviour GetMineCartTurretPrefab()
        {
            if ((Object) RobotDave.m_trapData == (Object) null)
                RobotDave.m_trapData = (DungeonPlaceable) BraveResources.Load("RobotDaveTraps", ".asset");
            return RobotDave.m_trapData.variantTiers[11].nonDatabasePlaceable.GetComponent<DungeonPlaceableBehaviour>();
        }

        public static DungeonPlaceableBehaviour GetMineCartPrefab()
        {
            if ((Object) RobotDave.m_trapData == (Object) null)
                RobotDave.m_trapData = (DungeonPlaceable) BraveResources.Load("RobotDaveTraps", ".asset");
            return RobotDave.m_trapData.variantTiers[6].nonDatabasePlaceable.GetComponent<DungeonPlaceableBehaviour>();
        }

        public static DungeonPlaceableBehaviour GetCaveInPrefab()
        {
            if ((Object) RobotDave.m_trapData == (Object) null)
                RobotDave.m_trapData = (DungeonPlaceable) BraveResources.Load("RobotDaveTraps", ".asset");
            return RobotDave.m_trapData.variantTiers[7].nonDatabasePlaceable.GetComponent<DungeonPlaceableBehaviour>();
        }

        public static DungeonPlaceableBehaviour GetChandelierPrefab()
        {
            if ((Object) RobotDave.m_trapData == (Object) null)
                RobotDave.m_trapData = (DungeonPlaceable) BraveResources.Load("RobotDaveTraps", ".asset");
            return RobotDave.m_trapData.variantTiers[8].nonDatabasePlaceable.GetComponent<DungeonPlaceableBehaviour>();
        }

        public static DungeonPlaceableBehaviour GetHorizontalConveyorPrefab()
        {
            if ((Object) RobotDave.m_trapData == (Object) null)
                RobotDave.m_trapData = (DungeonPlaceable) BraveResources.Load("RobotDaveTraps", ".asset");
            return RobotDave.m_trapData.variantTiers[9].nonDatabasePlaceable.GetComponent<DungeonPlaceableBehaviour>();
        }

        public static DungeonPlaceableBehaviour GetVerticalConveyorPrefab()
        {
            if ((Object) RobotDave.m_trapData == (Object) null)
                RobotDave.m_trapData = (DungeonPlaceable) BraveResources.Load("RobotDaveTraps", ".asset");
            return RobotDave.m_trapData.variantTiers[10].nonDatabasePlaceable.GetComponent<DungeonPlaceableBehaviour>();
        }

        public static DungeonPlaceableBehaviour GetRollingLogVertical()
        {
            if ((Object) RobotDave.m_trapData == (Object) null)
                RobotDave.m_trapData = (DungeonPlaceable) BraveResources.Load("RobotDaveTraps", ".asset");
            bool flag = false;
            ResizableCollider component = RobotDave.m_trapData.variantTiers[!flag ? 4 : 12].nonDatabasePlaceable.GetComponent<ResizableCollider>();
            return (bool) (Object) component ? (DungeonPlaceableBehaviour) component : RobotDave.m_trapData.variantTiers[!flag ? 4 : 12].nonDatabasePlaceable.GetComponent<DungeonPlaceableBehaviour>();
        }

        public static DungeonPlaceableBehaviour GetRollingLogHorizontal()
        {
            if ((Object) RobotDave.m_trapData == (Object) null)
                RobotDave.m_trapData = (DungeonPlaceable) BraveResources.Load("RobotDaveTraps", ".asset");
            bool flag = false;
            ResizableCollider component = RobotDave.m_trapData.variantTiers[!flag ? 5 : 13].nonDatabasePlaceable.GetComponent<ResizableCollider>();
            return (bool) (Object) component ? (DungeonPlaceableBehaviour) component : RobotDave.m_trapData.variantTiers[!flag ? 5 : 13].nonDatabasePlaceable.GetComponent<DungeonPlaceableBehaviour>();
        }

        public static DungeonPlaceable GetHorizontalTable()
        {
            if ((Object) RobotDave.m_horizontalTable == (Object) null)
                RobotDave.m_horizontalTable = (DungeonPlaceable) BraveResources.Load("RobotTableHorizontal", ".asset");
            return RobotDave.m_horizontalTable;
        }

        public static DungeonPlaceable GetVerticalTable()
        {
            if ((Object) RobotDave.m_verticalTable == (Object) null)
                RobotDave.m_verticalTable = (DungeonPlaceable) BraveResources.Load("RobotTableVertical", ".asset");
            return RobotDave.m_verticalTable;
        }

        protected static void ResetForNewProcess()
        {
            RobotDave.m_trapData = (DungeonPlaceable) null;
            RobotDave.m_horizontalTable = (DungeonPlaceable) null;
            RobotDave.m_verticalTable = (DungeonPlaceable) null;
            RobotRoomSurroundingPitFeature.BeenUsed = false;
        }

        public static void ApplyFeatureToDwarfRegion(
            PrototypeDungeonRoom extantRoom,
            IntVector2 basePosition,
            IntVector2 dimensions,
            RobotDaveIdea idea,
            RobotDave.RobotFeatureType specificFeature,
            int targetObjectLayer)
        {
            RobotDave.ResetForNewProcess();
            RobotDave.ClearDataForRegion(extantRoom, idea, basePosition, dimensions, targetObjectLayer);
            RobotRoomFeature robotRoomFeature1 = specificFeature == RobotDave.RobotFeatureType.NONE ? RobotDave.SelectFeatureForZone(idea, basePosition, dimensions, false, 1) : RobotDave.GetFeatureFromType(specificFeature);
            robotRoomFeature1.LocalBasePosition = basePosition;
            robotRoomFeature1.LocalDimensions = dimensions;
            RobotRoomFeature robotRoomFeature2 = (RobotRoomFeature) null;
            if (specificFeature == RobotDave.RobotFeatureType.NONE && robotRoomFeature1.CanContainOtherFeature())
            {
                IntVector2 basePos = robotRoomFeature1.LocalBasePosition + new IntVector2(robotRoomFeature1.RequiredInsetForOtherFeature(), robotRoomFeature1.RequiredInsetForOtherFeature());
                IntVector2 dim = robotRoomFeature1.LocalDimensions - new IntVector2(robotRoomFeature1.RequiredInsetForOtherFeature() * 2, robotRoomFeature1.RequiredInsetForOtherFeature() * 2);
                robotRoomFeature2 = RobotDave.SelectFeatureForZone(idea, basePos, dim, true, 1);
                robotRoomFeature2.LocalBasePosition = basePos;
                robotRoomFeature2.LocalDimensions = dim;
            }
            robotRoomFeature1.Develop(extantRoom, idea, targetObjectLayer);
            robotRoomFeature2?.Develop(extantRoom, idea, targetObjectLayer);
        }

        public static void DwarfProcessIdea(
            PrototypeDungeonRoom extantRoom,
            RobotDaveIdea idea,
            IntVector2 desiredDimensions)
        {
            RobotDave.ResetForNewProcess();
            RobotDave.ProcessBasicRoomData(extantRoom, idea, desiredDimensions);
            List<RobotRoomFeature> robotRoomFeatureList = RobotDave.RequestRidiculousNumberOfFeatures(extantRoom, idea, false);
            for (int index = 0; index < robotRoomFeatureList.Count; ++index)
                robotRoomFeatureList[index].Develop(extantRoom, idea, -1);
            RobotDave.PlaceEnemiesInRoom(extantRoom, idea);
        }

        public static PrototypeDungeonRoom RuntimeProcessIdea(
            RobotDaveIdea idea,
            IntVector2 desiredDimensions)
        {
            RobotDave.ResetForNewProcess();
            PrototypeDungeonRoom instance = ScriptableObject.CreateInstance<PrototypeDungeonRoom>();
            RobotDave.ProcessBasicRoomData(instance, idea, desiredDimensions);
            List<RobotRoomFeature> robotRoomFeatureList = RobotDave.RequestRidiculousNumberOfFeatures(instance, idea, true);
            for (int index = 0; index < robotRoomFeatureList.Count; ++index)
                robotRoomFeatureList[index].Develop(instance, idea, -1);
            RobotDave.PlaceEnemiesInRoom(instance, idea);
            instance.roomEvents.Add(new RoomEventDefinition(RoomEventTriggerCondition.ON_ENTER_WITH_ENEMIES, RoomEventTriggerAction.SEAL_ROOM));
            instance.roomEvents.Add(new RoomEventDefinition(RoomEventTriggerCondition.ON_ENEMIES_CLEARED, RoomEventTriggerAction.UNSEAL_ROOM));
            return instance;
        }

        protected static PrototypePlacedObjectData PlacePlaceable(
            DungeonPlaceable item,
            PrototypeDungeonRoom room,
            IntVector2 position)
        {
            if ((Object) item == (Object) null || (Object) room == (Object) null)
                return (PrototypePlacedObjectData) null;
            if (room.CheckRegionOccupiedExcludeWallsAndPits(position.x, position.y, item.GetWidth(), item.GetHeight(), false))
                return (PrototypePlacedObjectData) null;
            Vector2 vector2 = position.ToVector2();
            PrototypePlacedObjectData placedObjectData = new PrototypePlacedObjectData();
            placedObjectData.fieldData = new List<PrototypePlacedObjectFieldData>();
            placedObjectData.instancePrerequisites = new DungeonPrerequisite[0];
            placedObjectData.placeableContents = item;
            placedObjectData.contentsBasePosition = vector2;
            int count = room.placedObjects.Count;
            room.placedObjects.Add(placedObjectData);
            room.placedObjectPositions.Add(vector2);
            for (int index1 = 0; index1 < item.GetWidth(); ++index1)
            {
                for (int index2 = 0; index2 < item.GetHeight(); ++index2)
                    room.ForceGetCellDataAtPoint(position.x + index1, position.y + index2).placedObjectRUBELIndex = count;
            }
            return placedObjectData;
        }

        private static void PlaceEnemiesInRoom(PrototypeDungeonRoom room, RobotDaveIdea idea)
        {
            if (idea.ValidEasyEnemyPlaceables == null || idea.ValidEasyEnemyPlaceables.Length == 0)
                return;
            int a = Mathf.Clamp(Mathf.CeilToInt((float) (room.Width * room.Height) / 45f), 1, 6);
            if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.CASTLEGEON)
            {
                a = Mathf.Min(a, 3);
                if ((double) Random.value < 0.10000000149011612)
                    return;
            }
            int num1 = 0;
            if (a > 3 && idea.ValidHardEnemyPlaceables != null && idea.ValidHardEnemyPlaceables.Length > 0 && (double) Random.value < 0.5)
            {
                ++num1;
                a -= 2;
            }
            if (a > 3)
                a = Random.Range(3, a + 1);
            int num2 = Mathf.FloorToInt((float) room.Width / 5f);
            int num3 = Mathf.FloorToInt((float) room.Height / 5f);
            int num4 = Mathf.CeilToInt((float) num2 / 2f);
            int num5 = Mathf.CeilToInt((float) num3 / 2f);
            List<IntVector2> input = new List<IntVector2>();
            for (int index1 = 0; index1 < 4; ++index1)
            {
                for (int index2 = 0; index2 < 4; ++index2)
                {
                    for (int index3 = 0; index3 < 3; ++index3)
                    {
                        int num6 = Random.Range(-num4, num4 + 1);
                        int num7 = Random.Range(-num5, num5 + 1);
                        input.Add(new IntVector2(num2 * (index1 + 1) + num6, num3 * (index2 + 1) + num7));
                    }
                }
            }
            List<IntVector2> intVector2List = input.GenerationShuffle<IntVector2>();
            for (int index4 = 0; index4 < a; ++index4)
            {
                DungeonPlaceable easyEnemyPlaceable = idea.ValidEasyEnemyPlaceables[Random.Range(0, idea.ValidEasyEnemyPlaceables.Length)];
                int index5 = 0;
                while (index5 < intVector2List.Count && RobotDave.PlacePlaceable(easyEnemyPlaceable, room, intVector2List[index5]) == null)
                    ++index5;
            }
            for (int index6 = 0; index6 < num1; ++index6)
            {
                DungeonPlaceable hardEnemyPlaceable = idea.ValidHardEnemyPlaceables[Random.Range(0, idea.ValidHardEnemyPlaceables.Length)];
                int index7 = 0;
                while (index7 < intVector2List.Count && RobotDave.PlacePlaceable(hardEnemyPlaceable, room, intVector2List[index7]) == null)
                    ++index7;
            }
        }

        private static RobotRoomFeature SelectFeatureForZone(
            RobotDaveIdea idea,
            IntVector2 basePos,
            IntVector2 dim,
            bool isInternal,
            int numFeatures)
        {
            List<RobotRoomFeature> robotRoomFeatureList = new List<RobotRoomFeature>();
            FlatExpanseFeature flatExpanseFeature = new FlatExpanseFeature();
            if (flatExpanseFeature.AcceptableInIdea(idea, dim, isInternal, numFeatures))
                robotRoomFeatureList.Add((RobotRoomFeature) flatExpanseFeature);
            RobotRoomColumnFeature roomColumnFeature = new RobotRoomColumnFeature();
            if (roomColumnFeature.AcceptableInIdea(idea, dim, isInternal, numFeatures))
                robotRoomFeatureList.Add((RobotRoomFeature) roomColumnFeature);
            RobotRoomSurroundingPitFeature surroundingPitFeature = new RobotRoomSurroundingPitFeature();
            if (surroundingPitFeature.AcceptableInIdea(idea, dim, isInternal, numFeatures))
                robotRoomFeatureList.Add((RobotRoomFeature) surroundingPitFeature);
            RobotRoomTrapPlusFeature roomTrapPlusFeature = new RobotRoomTrapPlusFeature();
            if (roomTrapPlusFeature.AcceptableInIdea(idea, dim, isInternal, numFeatures))
                robotRoomFeatureList.Add((RobotRoomFeature) roomTrapPlusFeature);
            RobotRoomTrapSquareFeature trapSquareFeature = new RobotRoomTrapSquareFeature();
            if (trapSquareFeature.AcceptableInIdea(idea, dim, isInternal, numFeatures))
                robotRoomFeatureList.Add((RobotRoomFeature) trapSquareFeature);
            RobotRoomCornerColumnsFeature cornerColumnsFeature = new RobotRoomCornerColumnsFeature();
            if (cornerColumnsFeature.AcceptableInIdea(idea, dim, isInternal, numFeatures))
                robotRoomFeatureList.Add((RobotRoomFeature) cornerColumnsFeature);
            RobotRoomInnerPitFeature roomInnerPitFeature = new RobotRoomInnerPitFeature();
            if (roomInnerPitFeature.AcceptableInIdea(idea, dim, isInternal, numFeatures))
                robotRoomFeatureList.Add((RobotRoomFeature) roomInnerPitFeature);
            RobotRoomTablesFeature roomTablesFeature = new RobotRoomTablesFeature();
            if (roomTablesFeature.AcceptableInIdea(idea, dim, isInternal, numFeatures))
                robotRoomFeatureList.Add((RobotRoomFeature) roomTablesFeature);
            RobotRoomRollingLogsVerticalFeature logsVerticalFeature = new RobotRoomRollingLogsVerticalFeature();
            if (logsVerticalFeature.AcceptableInIdea(idea, dim, isInternal, numFeatures))
                robotRoomFeatureList.Add((RobotRoomFeature) logsVerticalFeature);
            RobotRoomRollingLogsHorizontalFeature horizontalFeature = new RobotRoomRollingLogsHorizontalFeature();
            if (horizontalFeature.AcceptableInIdea(idea, dim, isInternal, numFeatures))
                robotRoomFeatureList.Add((RobotRoomFeature) horizontalFeature);
            RobotRoomMineCartSquareFeature cartSquareFeature = new RobotRoomMineCartSquareFeature();
            if (cartSquareFeature.AcceptableInIdea(idea, dim, isInternal, numFeatures))
                robotRoomFeatureList.Add((RobotRoomFeature) cartSquareFeature);
            RobotRoomCaveInFeature roomCaveInFeature = new RobotRoomCaveInFeature();
            if (roomCaveInFeature.AcceptableInIdea(idea, dim, isInternal, numFeatures))
                robotRoomFeatureList.Add((RobotRoomFeature) roomCaveInFeature);
            RobotRoomMineCartSquareDoubleFeature squareDoubleFeature = new RobotRoomMineCartSquareDoubleFeature();
            if (squareDoubleFeature.AcceptableInIdea(idea, dim, isInternal, numFeatures))
                robotRoomFeatureList.Add((RobotRoomFeature) squareDoubleFeature);
            return robotRoomFeatureList.Count == 0 ? (RobotRoomFeature) null : robotRoomFeatureList[Random.Range(0, robotRoomFeatureList.Count)];
        }

        private static List<RobotRoomFeature> RequestRidiculousNumberOfFeatures(
            PrototypeDungeonRoom room,
            RobotDaveIdea idea,
            bool isRuntime)
        {
            List<RobotRoomFeature> robotRoomFeatureList = new List<RobotRoomFeature>();
            int num1 = room.Width * room.Height;
            int numFeatures;
            if (num1 <= 49)
            {
                numFeatures = 1;
            }
            else
            {
                float num2 = Random.value;
                if ((double) num2 < 0.5)
                {
                    numFeatures = 1;
                }
                else
                {
                    float num3 = (float) room.Width / ((float) room.Height * 1f);
                    numFeatures = num1 < 100 || (double) num3 > 1.75 || (double) num3 < 0.60000002384185791 ? 2 : ((double) num2 >= 0.75 ? 2 : 2);
                }
            }
            List<IntVector2> intVector2List1 = new List<IntVector2>();
            List<IntVector2> intVector2List2 = new List<IntVector2>();
            switch (numFeatures)
            {
                case 1:
                    intVector2List1.Add(IntVector2.Zero);
                    intVector2List2.Add(new IntVector2(room.Width, room.Height));
                    break;
                case 2:
                    float f1 = (float) room.Width / 2f;
                    float f2 = (float) room.Height / 2f;
                    if (room.Width > room.Height)
                    {
                        intVector2List1.Add(IntVector2.Zero);
                        intVector2List2.Add(new IntVector2(Mathf.FloorToInt(f1), room.Height));
                        intVector2List1.Add(new IntVector2(Mathf.FloorToInt(f1), 0));
                        intVector2List2.Add(new IntVector2(Mathf.CeilToInt(f1), room.Height));
                        break;
                    }
                    intVector2List1.Add(IntVector2.Zero);
                    intVector2List2.Add(new IntVector2(room.Width, Mathf.FloorToInt(f2)));
                    intVector2List1.Add(new IntVector2(0, Mathf.FloorToInt(f2)));
                    intVector2List2.Add(new IntVector2(room.Width, Mathf.CeilToInt(f2)));
                    break;
                case 4:
                    float f3 = (float) room.Width / 2f;
                    float f4 = (float) room.Height / 2f;
                    bool flag = (double) Random.value < 0.5;
                    int x1 = !flag ? Mathf.CeilToInt(f3) : Mathf.FloorToInt(f3);
                    int x2 = flag ? Mathf.CeilToInt(f3) : Mathf.FloorToInt(f3);
                    int y1 = !flag ? Mathf.CeilToInt(f4) : Mathf.FloorToInt(f4);
                    int y2 = flag ? Mathf.CeilToInt(f4) : Mathf.FloorToInt(f4);
                    intVector2List1.Add(IntVector2.Zero);
                    intVector2List2.Add(new IntVector2(x1, y1));
                    intVector2List1.Add(new IntVector2(x1, 0));
                    intVector2List2.Add(new IntVector2(x2, y1));
                    intVector2List1.Add(new IntVector2(0, y1));
                    intVector2List2.Add(new IntVector2(x1, y2));
                    intVector2List1.Add(new IntVector2(x1, y1));
                    intVector2List2.Add(new IntVector2(x2, y2));
                    break;
            }
            for (int index = 0; index < numFeatures; ++index)
            {
                IntVector2 basePos = intVector2List1[index];
                IntVector2 dim = intVector2List2[index];
                RobotRoomFeature robotRoomFeature = RobotDave.SelectFeatureForZone(idea, basePos, dim, false, numFeatures);
                if (robotRoomFeature != null)
                {
                    robotRoomFeature.LocalBasePosition = basePos;
                    robotRoomFeature.LocalDimensions = dim;
                    robotRoomFeature.Use();
                    robotRoomFeatureList.Add(robotRoomFeature);
                }
            }
            int count = robotRoomFeatureList.Count;
            for (int index = 0; index < count; ++index)
            {
                if (robotRoomFeatureList[index].CanContainOtherFeature())
                {
                    IntVector2 basePos = robotRoomFeatureList[index].LocalBasePosition + new IntVector2(robotRoomFeatureList[index].RequiredInsetForOtherFeature(), robotRoomFeatureList[index].RequiredInsetForOtherFeature());
                    IntVector2 dim = robotRoomFeatureList[index].LocalDimensions - new IntVector2(robotRoomFeatureList[index].RequiredInsetForOtherFeature() * 2, robotRoomFeatureList[index].RequiredInsetForOtherFeature() * 2);
                    RobotRoomFeature robotRoomFeature = RobotDave.SelectFeatureForZone(idea, basePos, dim, true, numFeatures);
                    if (robotRoomFeature != null)
                    {
                        robotRoomFeature.LocalBasePosition = basePos;
                        robotRoomFeature.LocalDimensions = dim;
                        robotRoomFeature.Use();
                        robotRoomFeatureList.Add(robotRoomFeature);
                    }
                }
            }
            return robotRoomFeatureList;
        }

        private static void ClearDataForRegion(
            PrototypeDungeonRoom room,
            RobotDaveIdea idea,
            IntVector2 basePosition,
            IntVector2 desiredDimensions,
            int targetObjectLayer)
        {
            for (int x = basePosition.x; x < basePosition.x + desiredDimensions.x; ++x)
            {
                for (int y = basePosition.y; y < basePosition.y + desiredDimensions.y; ++y)
                {
                    PrototypeDungeonRoomCellData cellDataAtPoint = room.ForceGetCellDataAtPoint(x, y);
                    cellDataAtPoint.state = CellType.FLOOR;
                    if (targetObjectLayer == -1)
                        cellDataAtPoint.placedObjectRUBELIndex = -1;
                    else if (cellDataAtPoint.additionalPlacedObjectIndices.Count > targetObjectLayer)
                        cellDataAtPoint.additionalPlacedObjectIndices[targetObjectLayer] = -1;
                    cellDataAtPoint.doesDamage = false;
                    cellDataAtPoint.damageDefinition = new CellDamageDefinition();
                    cellDataAtPoint.appearance = new PrototypeDungeonRoomCellAppearance();
                }
            }
            if (targetObjectLayer == -1)
            {
                for (int index = 0; index < room.placedObjects.Count; ++index)
                {
                    Vector2 placedObjectPosition = room.placedObjectPositions[index];
                    if ((double) placedObjectPosition.x >= (double) basePosition.x && (double) placedObjectPosition.x < (double) (basePosition.x + desiredDimensions.x) && (double) placedObjectPosition.y >= (double) basePosition.y && (double) placedObjectPosition.y < (double) (basePosition.y + desiredDimensions.y))
                    {
                        if (room.placedObjects[index].assignedPathIDx >= 0)
                            room.RemovePathAt(room.placedObjects[index].assignedPathIDx);
                        room.placedObjectPositions.RemoveAt(index);
                        room.placedObjects.RemoveAt(index);
                        --index;
                    }
                }
            }
            else
            {
                PrototypeRoomObjectLayer additionalObjectLayer = room.additionalObjectLayers[targetObjectLayer];
                for (int index = 0; index < additionalObjectLayer.placedObjects.Count; ++index)
                {
                    Vector2 objectBasePosition = additionalObjectLayer.placedObjectBasePositions[index];
                    if ((double) objectBasePosition.x >= (double) basePosition.x && (double) objectBasePosition.x < (double) (basePosition.x + desiredDimensions.x) && (double) objectBasePosition.y >= (double) basePosition.y && (double) objectBasePosition.y < (double) (basePosition.y + desiredDimensions.y))
                    {
                        if (additionalObjectLayer.placedObjects[index].assignedPathIDx >= 0)
                            room.RemovePathAt(additionalObjectLayer.placedObjects[index].assignedPathIDx);
                        additionalObjectLayer.placedObjectBasePositions.RemoveAt(index);
                        additionalObjectLayer.placedObjects.RemoveAt(index);
                        --index;
                    }
                }
            }
        }

        private static void ProcessBasicRoomData(
            PrototypeDungeonRoom room,
            RobotDaveIdea idea,
            IntVector2 desiredDimensions)
        {
            room.category = PrototypeDungeonRoom.RoomCategory.NORMAL;
            room.Width = desiredDimensions.x;
            room.Height = desiredDimensions.y;
            for (int ix = 0; ix < room.Width; ++ix)
            {
                for (int iy = 0; iy < room.Height; ++iy)
                {
                    PrototypeDungeonRoomCellData cellDataAtPoint = room.ForceGetCellDataAtPoint(ix, iy);
                    cellDataAtPoint.state = CellType.FLOOR;
                    cellDataAtPoint.placedObjectRUBELIndex = -1;
                    cellDataAtPoint.doesDamage = false;
                    cellDataAtPoint.damageDefinition = new CellDamageDefinition();
                    cellDataAtPoint.appearance = new PrototypeDungeonRoomCellAppearance();
                }
            }
            room.exitData = new PrototypeRoomExitData();
            room.pits = new List<PrototypeRoomPitEntry>();
            room.placedObjects = new List<PrototypePlacedObjectData>();
            room.placedObjectPositions = new List<Vector2>();
            room.additionalObjectLayers = new List<PrototypeRoomObjectLayer>();
            room.eventTriggerAreas = new List<PrototypeEventTriggerArea>();
            room.roomEvents = new List<RoomEventDefinition>();
            room.paths = new List<SerializedPath>();
            room.prerequisites = new List<DungeonPrerequisite>();
            room.excludedOtherRooms = new List<PrototypeDungeonRoom>();
            room.rectangularFeatures = new List<PrototypeRectangularFeature>();
        }

        public enum RobotFeatureType
        {
            NONE = 0,
            FLAT_EXPANSE = 5,
            COLUMN_SAWBLADE = 10, // 0x0000000A
            PIT_BORDER = 15, // 0x0000000F
            TRAP_PLUS = 20, // 0x00000014
            TRAP_SQUARE = 25, // 0x00000019
            CORNER_COLUMNS = 30, // 0x0000001E
            PIT_INNER = 35, // 0x00000023
            TABLES_EDGE = 40, // 0x00000028
            ROLLING_LOG_VERTICAL = 45, // 0x0000002D
            ROLLING_LOG_HORIZONTAL = 50, // 0x00000032
            CASTLE_CHANDELIER = 60, // 0x0000003C
            MINES_CAVE_IN = 70, // 0x00000046
            MINES_SQUARE_CART = 75, // 0x0000004B
            MINES_DOUBLE_CART = 76, // 0x0000004C
            MINES_TURRET_CART = 77, // 0x0000004D
            CONVEYOR_HORIZONTAL = 110, // 0x0000006E
            CONVEYOR_VERTICAL = 115, // 0x00000073
        }
    }

