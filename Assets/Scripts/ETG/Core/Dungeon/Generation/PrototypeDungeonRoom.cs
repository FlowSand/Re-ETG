using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Serialization;

using Dungeonator;

#nullable disable

[Serializable]
public class PrototypeDungeonRoom : ScriptableObject, ISerializationCallbackReceiver
    {
        [HideInInspector]
        public int RoomId = -1;
        [SerializeField]
        public string QAID;
        [SerializeField]
        public string GUID;
        [SerializeField]
        public bool PreventMirroring;
        [NonSerialized]
        public PrototypeDungeonRoom MirrorSource;
        public PrototypeDungeonRoom.RoomCategory category = PrototypeDungeonRoom.RoomCategory.NORMAL;
        public PrototypeDungeonRoom.RoomNormalSubCategory subCategoryNormal;
        public PrototypeDungeonRoom.RoomBossSubCategory subCategoryBoss;
        public PrototypeDungeonRoom.RoomSpecialSubCategory subCategorySpecial = PrototypeDungeonRoom.RoomSpecialSubCategory.STANDARD_SHOP;
        public PrototypeDungeonRoom.RoomSecretSubCategory subCategorySecret;
        public PrototypeRoomExitData exitData;
        public List<PrototypeRoomPitEntry> pits;
        public List<PrototypePlacedObjectData> placedObjects;
        public List<Vector2> placedObjectPositions;
        public List<PrototypeRoomObjectLayer> additionalObjectLayers = new List<PrototypeRoomObjectLayer>();
        [NonSerialized]
        public List<PrototypeRoomObjectLayer> runtimeAdditionalObjectLayers;
        public List<PrototypeEventTriggerArea> eventTriggerAreas;
        public List<RoomEventDefinition> roomEvents;
        public List<SerializedPath> paths = new List<SerializedPath>();
        public GlobalDungeonData.ValidTilesets overriddenTilesets;
        public List<DungeonPrerequisite> prerequisites;
        public int RequiredCurseLevel = -1;
        public bool InvalidInCoop;
        public List<PrototypeDungeonRoom> excludedOtherRooms = new List<PrototypeDungeonRoom>();
        public List<PrototypeRectangularFeature> rectangularFeatures = new List<PrototypeRectangularFeature>();
        public bool usesProceduralLighting = true;
        public bool usesProceduralDecoration = true;
        public bool cullProceduralDecorationOnWeakPlatforms;
        public bool allowFloorDecoration = true;
        public bool allowWallDecoration = true;
        public bool preventAddedDecoLayering;
        public bool precludeAllTilemapDrawing;
        public bool drawPrecludedCeilingTiles;
        public bool preventFacewallAO;
        public bool preventBorders;
        public bool usesCustomAmbientLight;
        [ColorUsage(true, true, 0.0f, 8f, 0.125f, 3f)]
        public Color customAmbientLight = Color.white;
        public bool ForceAllowDuplicates;
        public GameObject doorTopDecorable;
        public SharedInjectionData requiredInjectionData;
        public RuntimeInjectionFlags injectionFlags;
        public bool IsLostWoodsRoom;
        public bool UseCustomMusicState;
        public DungeonFloorMusicController.DungeonMusicState OverrideMusicState = DungeonFloorMusicController.DungeonMusicState.CALM;
        public bool UseCustomMusic;
        public string CustomMusicEvent;
        public bool UseCustomMusicSwitch;
        public string CustomMusicSwitch;
        public int overrideRoomVisualType = -1;
        public bool overrideRoomVisualTypeForSecretRooms;
        public IntVector2 rewardChestSpawnPosition = new IntVector2(-1, -1);
        public GameObject associatedMinimapIcon;
        [SerializeField]
        private int m_width = 5;
        [SerializeField]
        private int m_height = 5;
        [NonSerialized]
        private PrototypeDungeonRoomCellData[] m_cellData;
        [FormerlySerializedAs("m_cellData")]
        [SerializeField]
        private PrototypeDungeonRoomCellData[] m_OLDcellData;
        [SerializeField]
        private CellType[] m_serializedCellType;
        [SerializeField]
        private List<int> m_serializedCellDataIndices;
        [SerializeField]
        private List<PrototypeDungeonRoomCellData> m_serializedCellDataData;
        [SerializeField]
        [HideInInspector]
        private List<IntVector2> m_cachedRepresentationIncFacewalls;

        private static Vector2 MirrorPosition(Vector2 position, PrototypeDungeonRoom room)
        {
            int num = Mathf.RoundToInt(position.x);
            return new Vector2((float) (room.Width - 1 - num), position.y);
        }

        public static bool GameObjectCanBeMirrored(GameObject g)
        {
            return !(bool) (UnityEngine.Object) g.GetComponent<ConveyorBelt>() && !(bool) (UnityEngine.Object) g.GetComponent<ForgeFlamePipeController>() && !(bool) (UnityEngine.Object) g.GetComponent<ForgeCrushDoorController>() && !(bool) (UnityEngine.Object) g.GetComponent<ForgeHammerController>() && !(bool) (UnityEngine.Object) g.GetComponentInChildren<ProjectileTrapController>();
        }

        private static bool CanPlacedObjectBeMirrored(PrototypePlacedObjectData data)
        {
            if (Mathf.Abs(data.xMPxOffset) > 0)
                return false;
            if ((bool) (UnityEngine.Object) data.placeableContents)
                return data.placeableContents.IsValidMirrorPlaceable();
            if ((bool) (UnityEngine.Object) data.nonenemyBehaviour)
                return PrototypeDungeonRoom.GameObjectCanBeMirrored(data.nonenemyBehaviour.gameObject);
            return !(bool) (UnityEngine.Object) data.unspecifiedContents || PrototypeDungeonRoom.GameObjectCanBeMirrored(data.unspecifiedContents);
        }

        public static bool IsValidMirrorTarget(PrototypeDungeonRoom target)
        {
            if (target.category == PrototypeDungeonRoom.RoomCategory.BOSS || target.category == PrototypeDungeonRoom.RoomCategory.ENTRANCE || target.category == PrototypeDungeonRoom.RoomCategory.EXIT || target.category == PrototypeDungeonRoom.RoomCategory.REWARD || target.category == PrototypeDungeonRoom.RoomCategory.SPECIAL || target.category == PrototypeDungeonRoom.RoomCategory.SECRET || target.PreventMirroring || target.IsLostWoodsRoom || target.precludeAllTilemapDrawing || target.drawPrecludedCeilingTiles || target.overriddenTilesets != (GlobalDungeonData.ValidTilesets) 0 || target.paths.Count > 0)
                return false;
            for (int index = 0; index < target.placedObjects.Count; ++index)
            {
                if (!PrototypeDungeonRoom.CanPlacedObjectBeMirrored(target.placedObjects[index]))
                    return false;
            }
            for (int index1 = 0; index1 < target.additionalObjectLayers.Count; ++index1)
            {
                for (int index2 = 0; index2 < target.additionalObjectLayers[index1].placedObjects.Count; ++index2)
                {
                    if (!PrototypeDungeonRoom.CanPlacedObjectBeMirrored(target.additionalObjectLayers[index1].placedObjects[index2]))
                        return false;
                }
            }
            return true;
        }

        public static PrototypeDungeonRoom MirrorRoom(PrototypeDungeonRoom sourceRoom)
        {
            IntVector2 intVector2 = new IntVector2(sourceRoom.m_width, sourceRoom.m_height);
            PrototypeDungeonRoom instance = ScriptableObject.CreateInstance<PrototypeDungeonRoom>();
            instance.MirrorSource = sourceRoom;
            instance.category = sourceRoom.category;
            instance.subCategoryNormal = sourceRoom.subCategoryNormal;
            instance.subCategoryBoss = sourceRoom.subCategoryBoss;
            instance.subCategorySecret = sourceRoom.subCategorySecret;
            instance.subCategorySpecial = sourceRoom.subCategorySpecial;
            instance.usesProceduralLighting = sourceRoom.usesProceduralLighting;
            instance.usesProceduralDecoration = sourceRoom.usesProceduralDecoration;
            instance.cullProceduralDecorationOnWeakPlatforms = sourceRoom.cullProceduralDecorationOnWeakPlatforms;
            instance.allowFloorDecoration = sourceRoom.allowFloorDecoration;
            instance.allowWallDecoration = sourceRoom.allowWallDecoration;
            instance.preventAddedDecoLayering = sourceRoom.preventAddedDecoLayering;
            instance.precludeAllTilemapDrawing = sourceRoom.precludeAllTilemapDrawing;
            instance.drawPrecludedCeilingTiles = sourceRoom.drawPrecludedCeilingTiles;
            instance.preventFacewallAO = sourceRoom.preventFacewallAO;
            instance.preventBorders = sourceRoom.preventBorders;
            instance.usesCustomAmbientLight = sourceRoom.usesCustomAmbientLight;
            instance.customAmbientLight = sourceRoom.customAmbientLight;
            instance.ForceAllowDuplicates = sourceRoom.ForceAllowDuplicates;
            instance.doorTopDecorable = sourceRoom.doorTopDecorable;
            instance.requiredInjectionData = sourceRoom.requiredInjectionData;
            instance.injectionFlags = sourceRoom.injectionFlags;
            instance.IsLostWoodsRoom = sourceRoom.IsLostWoodsRoom;
            instance.UseCustomMusicState = sourceRoom.UseCustomMusicState;
            instance.OverrideMusicState = sourceRoom.OverrideMusicState;
            instance.UseCustomMusic = sourceRoom.UseCustomMusic;
            instance.CustomMusicEvent = sourceRoom.CustomMusicEvent;
            instance.RequiredCurseLevel = sourceRoom.RequiredCurseLevel;
            instance.InvalidInCoop = sourceRoom.InvalidInCoop;
            instance.overrideRoomVisualType = sourceRoom.overrideRoomVisualType;
            instance.overrideRoomVisualTypeForSecretRooms = sourceRoom.overrideRoomVisualTypeForSecretRooms;
            instance.rewardChestSpawnPosition = sourceRoom.rewardChestSpawnPosition;
            instance.rewardChestSpawnPosition.x = intVector2.x - (instance.rewardChestSpawnPosition.x + 1);
            instance.associatedMinimapIcon = sourceRoom.associatedMinimapIcon;
            instance.overriddenTilesets = sourceRoom.overriddenTilesets;
            instance.excludedOtherRooms = new List<PrototypeDungeonRoom>();
            for (int index = 0; index < sourceRoom.excludedOtherRooms.Count; ++index)
                instance.excludedOtherRooms.Add(sourceRoom.excludedOtherRooms[index]);
            instance.prerequisites = new List<DungeonPrerequisite>();
            for (int index = 0; index < sourceRoom.prerequisites.Count; ++index)
                instance.prerequisites.Add(sourceRoom.prerequisites[index]);
            instance.m_width = sourceRoom.m_width;
            instance.m_height = sourceRoom.m_height;
            instance.m_serializedCellType = new CellType[sourceRoom.m_serializedCellType.Length];
            instance.m_serializedCellDataIndices = new List<int>();
            instance.m_serializedCellDataData = new List<PrototypeDungeonRoomCellData>();
            for (int index1 = 0; index1 < sourceRoom.m_serializedCellType.Length; ++index1)
            {
                int num1 = index1;
                int num2 = num1 % sourceRoom.m_width;
                int num3 = Mathf.FloorToInt((float) (num1 / sourceRoom.m_width));
                int num4 = sourceRoom.m_width - (num2 + 1);
                int index2 = num3 * sourceRoom.m_width + num4;
                instance.m_serializedCellType[index2] = sourceRoom.m_serializedCellType[index1];
            }
            for (int index = 0; index < sourceRoom.m_serializedCellDataIndices.Count; ++index)
            {
                int serializedCellDataIndex = sourceRoom.m_serializedCellDataIndices[index];
                int num5 = serializedCellDataIndex % sourceRoom.m_width;
                int num6 = Mathf.FloorToInt((float) (serializedCellDataIndex / sourceRoom.m_width));
                int num7 = sourceRoom.m_width - (num5 + 1);
                int num8 = num6 * sourceRoom.m_width + num7;
                instance.m_serializedCellDataIndices.Add(num8);
                PrototypeDungeonRoomCellData source = sourceRoom.m_serializedCellDataData[index];
                PrototypeDungeonRoomCellData dungeonRoomCellData = new PrototypeDungeonRoomCellData(source.str, source.state);
                dungeonRoomCellData.MirrorData(source);
                instance.m_serializedCellDataData.Add(dungeonRoomCellData);
            }
            List<int> intList = new List<int>((IEnumerable<int>) instance.m_serializedCellDataIndices);
            intList.Sort();
            List<PrototypeDungeonRoomCellData> dungeonRoomCellDataList = new List<PrototypeDungeonRoomCellData>((IEnumerable<PrototypeDungeonRoomCellData>) instance.m_serializedCellDataData);
            for (int index3 = 0; index3 < intList.Count; ++index3)
            {
                int num = intList[index3];
                int index4 = instance.m_serializedCellDataIndices.IndexOf(num);
                dungeonRoomCellDataList[index3] = instance.m_serializedCellDataData[index4];
            }
            instance.m_serializedCellDataIndices = intList;
            instance.m_serializedCellDataData = dungeonRoomCellDataList;
            instance.exitData = new PrototypeRoomExitData();
            instance.exitData.MirrorData(sourceRoom.exitData, intVector2);
            instance.pits = new List<PrototypeRoomPitEntry>();
            for (int index = 0; index < sourceRoom.pits.Count; ++index)
                instance.pits.Add(sourceRoom.pits[index].CreateMirror(intVector2));
            instance.eventTriggerAreas = new List<PrototypeEventTriggerArea>();
            for (int index = 0; index < sourceRoom.eventTriggerAreas.Count; ++index)
                instance.eventTriggerAreas.Add(sourceRoom.eventTriggerAreas[index].CreateMirror(intVector2));
            instance.roomEvents = new List<RoomEventDefinition>();
            for (int index = 0; index < sourceRoom.roomEvents.Count; ++index)
                instance.roomEvents.Add(new RoomEventDefinition(sourceRoom.roomEvents[index].condition, sourceRoom.roomEvents[index].action));
            instance.placedObjects = new List<PrototypePlacedObjectData>();
            for (int index = 0; index < sourceRoom.placedObjects.Count; ++index)
                instance.placedObjects.Add(sourceRoom.placedObjects[index].CreateMirror(intVector2));
            instance.placedObjectPositions = new List<Vector2>();
            for (int index = 0; index < sourceRoom.placedObjectPositions.Count; ++index)
            {
                Vector2 placedObjectPosition = sourceRoom.placedObjectPositions[index];
                placedObjectPosition.x = (float) intVector2.x - (placedObjectPosition.x + (float) instance.placedObjects[index].GetWidth(true));
                instance.placedObjectPositions.Add(placedObjectPosition);
            }
            instance.additionalObjectLayers = new List<PrototypeRoomObjectLayer>();
            for (int index = 0; index < sourceRoom.additionalObjectLayers.Count; ++index)
                instance.additionalObjectLayers.Add(PrototypeRoomObjectLayer.CreateMirror(sourceRoom.additionalObjectLayers[index], intVector2));
            instance.rectangularFeatures = new List<PrototypeRectangularFeature>();
            for (int index = 0; index < sourceRoom.rectangularFeatures.Count; ++index)
                instance.rectangularFeatures.Add(PrototypeRectangularFeature.CreateMirror(sourceRoom.rectangularFeatures[index], intVector2));
            instance.paths = new List<SerializedPath>();
            for (int index = 0; index < sourceRoom.paths.Count; ++index)
                instance.paths.Add(SerializedPath.CreateMirror(sourceRoom.paths[index], intVector2, sourceRoom));
            instance.OnAfterDeserialize();
            instance.UpdatePrecalculatedData();
            return instance;
        }

        public void RemovePathAt(int id)
        {
            this.paths.RemoveAt(id);
            for (int index = 0; index < this.placedObjects.Count; ++index)
            {
                if (this.placedObjects[index].assignedPathIDx == id)
                    this.placedObjects[index].assignedPathIDx = -1;
                else if (this.placedObjects[index].assignedPathIDx > id)
                    --this.placedObjects[index].assignedPathIDx;
            }
            foreach (PrototypeRoomObjectLayer additionalObjectLayer in this.additionalObjectLayers)
            {
                for (int index = 0; index < additionalObjectLayer.placedObjects.Count; ++index)
                {
                    if (additionalObjectLayer.placedObjects[index].assignedPathIDx == id)
                        additionalObjectLayer.placedObjects[index].assignedPathIDx = -1;
                    else if (additionalObjectLayer.placedObjects[index].assignedPathIDx > id)
                        --additionalObjectLayer.placedObjects[index].assignedPathIDx;
                }
            }
        }

        public bool ContainsEnemies
        {
            get
            {
                if (this.placedObjects != null)
                {
                    for (int index = 0; index < this.placedObjects.Count; ++index)
                    {
                        PrototypePlacedObjectData placedObject = this.placedObjects[index];
                        if ((UnityEngine.Object) placedObject.placeableContents != (UnityEngine.Object) null && placedObject.placeableContents.ContainsEnemy || !string.IsNullOrEmpty(placedObject.enemyBehaviourGuid) && EnemyDatabase.GetEntry(placedObject.enemyBehaviourGuid) != null)
                            return true;
                    }
                }
                return false;
            }
        }

        public int MinDifficultyRating
        {
            get
            {
                int difficultyRating = 0;
                for (int index = 0; index < this.placedObjects.Count; ++index)
                {
                    PrototypePlacedObjectData placedObject = this.placedObjects[index];
                    if (placedObject == null)
                    {
                        Debug.LogError((object) ("Null object on room: " + this.name));
                    }
                    else
                    {
                        if ((UnityEngine.Object) placedObject.placeableContents != (UnityEngine.Object) null)
                            difficultyRating += placedObject.placeableContents.GetMinimumDifficulty();
                        if ((UnityEngine.Object) placedObject.nonenemyBehaviour != (UnityEngine.Object) null)
                            difficultyRating += placedObject.nonenemyBehaviour.GetMinimumDifficulty();
                        if (!string.IsNullOrEmpty(placedObject.enemyBehaviourGuid))
                            difficultyRating = difficultyRating;
                    }
                }
                return difficultyRating;
            }
        }

        public int MaxDifficultyRating
        {
            get
            {
                int difficultyRating = 0;
                for (int index = 0; index < this.placedObjects.Count; ++index)
                {
                    PrototypePlacedObjectData placedObject = this.placedObjects[index];
                    if ((UnityEngine.Object) placedObject.placeableContents != (UnityEngine.Object) null)
                        difficultyRating += placedObject.placeableContents.GetMaximumDifficulty();
                    if ((UnityEngine.Object) placedObject.nonenemyBehaviour != (UnityEngine.Object) null)
                        difficultyRating += placedObject.nonenemyBehaviour.GetMaximumDifficulty();
                    if (!string.IsNullOrEmpty(placedObject.enemyBehaviourGuid))
                        difficultyRating = difficultyRating;
                }
                return difficultyRating;
            }
        }

        public void OnBeforeSerialize()
        {
            this.m_serializedCellType = new CellType[this.m_cellData.Length];
            this.m_serializedCellDataIndices = new List<int>();
            this.m_serializedCellDataData = new List<PrototypeDungeonRoomCellData>();
            for (int index = 0; index < this.m_cellData.Length; ++index)
            {
                PrototypeDungeonRoomCellData dungeonRoomCellData = this.m_cellData[index];
                this.m_serializedCellType[index] = dungeonRoomCellData.state;
                if (dungeonRoomCellData.HasChanges())
                {
                    this.m_serializedCellDataIndices.Add(index);
                    this.m_serializedCellDataData.Add(dungeonRoomCellData);
                }
            }
        }

        public void OnAfterDeserialize()
        {
            if (this.m_OLDcellData != null && this.m_OLDcellData.Length > 0)
            {
                this.m_cellData = this.m_OLDcellData;
                this.m_OLDcellData = new PrototypeDungeonRoomCellData[0];
            }
            else
            {
                this.m_cellData = new PrototypeDungeonRoomCellData[this.m_serializedCellType.Length];
                int index1 = 0;
                for (int index2 = 0; index2 < this.m_serializedCellType.Length; ++index2)
                {
                    if (index1 < this.m_serializedCellDataIndices.Count && this.m_serializedCellDataIndices[index1] == index2)
                        this.m_cellData[index2] = this.m_serializedCellDataData[index1++];
                    else
                        this.m_cellData[index2] = new PrototypeDungeonRoomCellData()
                        {
                            appearance = new PrototypeDungeonRoomCellAppearance(),
                            state = this.m_serializedCellType[index2]
                        };
                }
            }
        }

        public PrototypeDungeonRoomCellData[] FullCellData
        {
            get => this.m_cellData;
            set => this.m_cellData = value;
        }

        public int Width
        {
            get => this.m_width;
            set
            {
                this.RecalculateCellDataArray(value, this.m_height);
                this.m_width = value;
            }
        }

        public int Height
        {
            get => this.m_height;
            set
            {
                this.RecalculateCellDataArray(this.m_width, value);
                this.m_height = value;
            }
        }

        public bool CheckPrerequisites()
        {
            if (this.InvalidInCoop && GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER || this.RequiredCurseLevel > 0 && PlayerStats.GetTotalCurse() < this.RequiredCurseLevel)
                return false;
            for (int index = 0; index < this.prerequisites.Count; ++index)
            {
                if (!this.prerequisites[index].CheckConditionsFulfilled())
                    return false;
            }
            return true;
        }

        public PrototypeEventTriggerArea AddEventTriggerArea(IEnumerable<IntVector2> cells)
        {
            PrototypeEventTriggerArea eventTriggerArea = new PrototypeEventTriggerArea(cells);
            this.eventTriggerAreas.Add(eventTriggerArea);
            return eventTriggerArea;
        }

        public List<PrototypeEventTriggerArea> GetEventTriggerAreasAtPosition(IntVector2 position)
        {
            List<PrototypeEventTriggerArea> triggerAreasAtPosition = (List<PrototypeEventTriggerArea>) null;
            foreach (PrototypeEventTriggerArea eventTriggerArea in this.eventTriggerAreas)
            {
                if (eventTriggerArea.triggerCells.Contains(position.ToVector2()))
                {
                    if (triggerAreasAtPosition == null)
                        triggerAreasAtPosition = new List<PrototypeEventTriggerArea>();
                    triggerAreasAtPosition.Add(eventTriggerArea);
                }
            }
            return triggerAreasAtPosition;
        }

        public void RemoveEventTriggerArea(PrototypeEventTriggerArea peta)
        {
            int num = this.eventTriggerAreas.IndexOf(peta);
            if (num < 0)
                return;
            this.eventTriggerAreas.Remove(peta);
            foreach (PrototypePlacedObjectData placedObject in this.placedObjects)
            {
                for (int index = 0; index < placedObject.linkedTriggerAreaIDs.Count; ++index)
                {
                    if (placedObject.linkedTriggerAreaIDs[index] == num)
                    {
                        placedObject.linkedTriggerAreaIDs.RemoveAt(index);
                        --index;
                    }
                    else if (placedObject.linkedTriggerAreaIDs[index] > num)
                        --placedObject.linkedTriggerAreaIDs[index];
                }
            }
            foreach (PrototypeRoomObjectLayer additionalObjectLayer in this.additionalObjectLayers)
            {
                foreach (PrototypePlacedObjectData placedObject in additionalObjectLayer.placedObjects)
                {
                    for (int index = 0; index < placedObject.linkedTriggerAreaIDs.Count; ++index)
                    {
                        if (placedObject.linkedTriggerAreaIDs[index] == num)
                        {
                            placedObject.linkedTriggerAreaIDs.RemoveAt(index);
                            --index;
                        }
                        else if (placedObject.linkedTriggerAreaIDs[index] > num)
                            --placedObject.linkedTriggerAreaIDs[index];
                    }
                }
            }
        }

        public bool DoesUnsealOnClear()
        {
            for (int index = 0; index < this.roomEvents.Count; ++index)
            {
                if (this.roomEvents[index].condition == RoomEventTriggerCondition.ON_ENEMIES_CLEARED && this.roomEvents[index].action == RoomEventTriggerAction.UNSEAL_ROOM)
                    return true;
            }
            return false;
        }

        public bool ContainsPit()
        {
            for (int index = 0; index < this.m_cellData.Length; ++index)
            {
                if (this.m_cellData[index].state == CellType.PIT)
                    return true;
            }
            return false;
        }

        public PrototypeRoomPitEntry GetPitEntryFromPosition(IntVector2 position)
        {
            Vector2 vector2 = position.ToVector2();
            foreach (PrototypeRoomPitEntry pit in this.pits)
            {
                if (pit.containedCells.Contains(vector2))
                    return pit;
            }
            return (PrototypeRoomPitEntry) null;
        }

        public void RedefineAllPitEntries()
        {
            this.pits = new List<PrototypeRoomPitEntry>();
            for (int index1 = 0; index1 < this.Width; ++index1)
            {
                for (int index2 = 0; index2 < this.Height; ++index2)
                {
                    if (this.ForceGetCellDataAtPoint(index1, index2).state == CellType.PIT)
                        this.HandlePitCellsAddition((IEnumerable<IntVector2>) new IntVector2[1]
                        {
                            new IntVector2(index1, index2)
                        });
                }
            }
        }

        public void HandlePitCellsAddition(IEnumerable<IntVector2> cells)
        {
            if (this.pits == null)
                this.pits = new List<PrototypeRoomPitEntry>();
            List<Vector2> cells1 = new List<Vector2>();
            foreach (IntVector2 cell in cells)
                cells1.Add(cell.ToVector2());
            for (int index = this.pits.Count - 1; index >= 0; --index)
            {
                if (this.pits[index].IsAdjoining((IEnumerable<Vector2>) cells1))
                {
                    cells1.AddRange((IEnumerable<Vector2>) this.pits[index].containedCells);
                    this.pits.RemoveAt(index);
                }
            }
            this.pits.Add(new PrototypeRoomPitEntry((IEnumerable<Vector2>) cells1));
        }

        public void HandlePitCellsRemoval(IEnumerable<IntVector2> cells)
        {
            if (this.pits == null)
                this.pits = new List<PrototypeRoomPitEntry>();
            HashSet<Vector2> collection = new HashSet<Vector2>();
            foreach (PrototypeRoomPitEntry pit in this.pits)
            {
                foreach (Vector2 containedCell in pit.containedCells)
                    collection.Add(containedCell);
            }
            this.pits.Clear();
            foreach (IntVector2 cell in cells)
                collection.Remove(cell.ToVector2());
            List<Vector2> vector2List = new List<Vector2>((IEnumerable<Vector2>) collection);
            while (vector2List.Count > 0)
            {
                Vector2 cell = vector2List[0];
                vector2List.RemoveAt(0);
                PrototypeRoomPitEntry prototypeRoomPitEntry = new PrototypeRoomPitEntry(cell);
                bool flag = true;
                while (flag)
                {
                    flag = false;
                    for (int index = vector2List.Count - 1; index >= 0; --index)
                    {
                        if (prototypeRoomPitEntry.IsAdjoining(vector2List[index]))
                        {
                            flag = true;
                            prototypeRoomPitEntry.containedCells.Add(vector2List[index]);
                            vector2List.RemoveAt(index);
                        }
                    }
                }
                this.pits.Add(prototypeRoomPitEntry);
            }
        }

        public void ClearAllObjectData()
        {
            for (int index = 0; index < this.m_cellData.Length; ++index)
            {
                PrototypeDungeonRoomCellData dungeonRoomCellData = this.m_cellData[index];
                dungeonRoomCellData.placedObjectRUBELIndex = -1;
                dungeonRoomCellData.additionalPlacedObjectIndices.Clear();
            }
        }

        public void DeleteRow(int yRow)
        {
            for (int index1 = 0; index1 < this.m_width; ++index1)
            {
                for (int index2 = yRow + 1; index2 < this.m_height; ++index2)
                    this.m_cellData[(index2 - 1) * this.m_width + index1] = this.m_cellData[index2 * this.m_width + index1];
            }
            this.exitData.HandleRowColumnShift(-1, 0, yRow + 1, -1, this);
            --this.Height;
            this.TranslateAllObjectBasePositions(0, -1, 0, this.Width, yRow + 1, this.Height + 1);
        }

        public void DeleteColumn(int xCol)
        {
            for (int index1 = 0; index1 < this.m_height; ++index1)
            {
                for (int index2 = xCol + 1; index2 < this.m_width; ++index2)
                    this.m_cellData[index1 * this.m_width + (index2 - 1)] = this.m_cellData[index1 * this.m_width + index2];
            }
            --this.Width;
            this.exitData.HandleRowColumnShift(xCol + 1, -1, -1, 0, this);
            this.TranslateAllObjectBasePositions(-1, 0, xCol + 1, this.Width + 1, 0, this.Height);
        }

        public bool CheckRegionOccupied(int xPos, int yPos, int w, int h)
        {
            for (int index1 = 0; index1 < w; ++index1)
            {
                for (int index2 = 0; index2 < h; ++index2)
                {
                    PrototypeDungeonRoomCellData cellDataAtPoint = this.ForceGetCellDataAtPoint(xPos + index1, yPos + index2);
                    if (cellDataAtPoint == null || cellDataAtPoint.IsOccupied)
                        return true;
                }
            }
            return false;
        }

        public bool CheckRegionOccupiedExcludeWallsAndPits(
            int xPos,
            int yPos,
            int w,
            int h,
            bool includeTopwalls = true)
        {
            for (int index1 = 0; index1 < w; ++index1)
            {
                for (int index2 = 0; index2 < h; ++index2)
                {
                    PrototypeDungeonRoomCellData cellDataAtPoint1 = this.ForceGetCellDataAtPoint(xPos + index1, yPos + index2);
                    if (cellDataAtPoint1 == null || cellDataAtPoint1.state != CellType.FLOOR || cellDataAtPoint1.IsOccupied)
                        return true;
                    if (!includeTopwalls)
                    {
                        PrototypeDungeonRoomCellData cellDataAtPoint2 = this.ForceGetCellDataAtPoint(xPos + index1, yPos + index2 - 1);
                        if (cellDataAtPoint2 == null || cellDataAtPoint2.state == CellType.WALL)
                            return true;
                    }
                }
            }
            return false;
        }

        public bool CheckRegionOccupied(int xPos, int yPos, int w, int h, int objectLayerIndex)
        {
            for (int index1 = 0; index1 < w; ++index1)
            {
                for (int index2 = 0; index2 < h; ++index2)
                {
                    if (this.ForceGetCellDataAtPoint(xPos + index1, yPos + index2).IsOccupiedAtLayer(objectLayerIndex))
                        return true;
                }
            }
            return false;
        }

        public List<IntVector2> GetCellRepresentation(IntVector2 worldBasePosition)
        {
            List<IntVector2> cellRepresentation = new List<IntVector2>();
            for (int index1 = 0; index1 < this.m_height; ++index1)
            {
                for (int index2 = 0; index2 < this.m_width; ++index2)
                {
                    PrototypeDungeonRoomCellData cellDataAtPoint = this.GetCellDataAtPoint(index2, index1);
                    if (cellDataAtPoint != null && (cellDataAtPoint.state == CellType.FLOOR || cellDataAtPoint.state == CellType.PIT || cellDataAtPoint.state == CellType.WALL && cellDataAtPoint.breakable))
                    {
                        IntVector2 intVector2 = worldBasePosition + new IntVector2(index2, index1);
                        cellRepresentation.Add(intVector2);
                    }
                }
            }
            return cellRepresentation;
        }

        public void UpdatePrecalculatedData()
        {
            HashSet<IntVector2> collection = new HashSet<IntVector2>();
            for (int index1 = 0; index1 < this.m_height; ++index1)
            {
                for (int index2 = 0; index2 < this.m_width; ++index2)
                {
                    PrototypeDungeonRoomCellData cellDataAtPoint = this.GetCellDataAtPoint(index2, index1);
                    if (cellDataAtPoint != null && (cellDataAtPoint.state == CellType.FLOOR || cellDataAtPoint.state == CellType.PIT || cellDataAtPoint.state == CellType.WALL && cellDataAtPoint.breakable))
                    {
                        IntVector2 intVector2 = new IntVector2(index2, index1);
                        collection.Add(intVector2);
                        collection.Add(intVector2 + IntVector2.Up);
                        collection.Add(intVector2 + IntVector2.Up * 2);
                        collection.Add(new IntVector2(intVector2.x + 1, intVector2.y));
                        collection.Add(new IntVector2(intVector2.x + 1, intVector2.y + 1));
                        collection.Add(new IntVector2(intVector2.x + 1, intVector2.y + 2));
                        collection.Add(new IntVector2(intVector2.x - 1, intVector2.y));
                        collection.Add(new IntVector2(intVector2.x - 1, intVector2.y + 1));
                        collection.Add(new IntVector2(intVector2.x - 1, intVector2.y + 2));
                        collection.Add(new IntVector2(intVector2.x, intVector2.y + 3));
                        collection.Add(new IntVector2(intVector2.x, intVector2.y - 1));
                        collection.Add(new IntVector2(intVector2.x - 1, intVector2.y - 1));
                        collection.Add(new IntVector2(intVector2.x + 1, intVector2.y - 1));
                        collection.Add(new IntVector2(intVector2.x - 1, intVector2.y + 3));
                        collection.Add(new IntVector2(intVector2.x + 1, intVector2.y + 3));
                    }
                }
            }
            UnityEngine.Random.InitState(this.name.GetHashCode());
            this.m_cachedRepresentationIncFacewalls = new List<IntVector2>((IEnumerable<IntVector2>) collection).Shuffle<IntVector2>();
        }

        public List<IntVector2> GetCellRepresentationIncFacewalls()
        {
            if (this.m_cachedRepresentationIncFacewalls != null && this.m_cachedRepresentationIncFacewalls.Count > 0)
                return this.m_cachedRepresentationIncFacewalls;
            Debug.LogError((object) $"PROTOTYPE DUNGEON ROOM: {this.name} IS MISSING PRECALCULATED DATA.");
            return (List<IntVector2>) null;
        }

        public PrototypeDungeonRoomCellData GetCellDataAtPoint(int ix, int iy)
        {
            return this.ForceGetCellDataAtPoint(ix, iy);
        }

        public PrototypeDungeonRoomCellData ForceGetCellDataAtPoint(int ix, int iy)
        {
            if (this.m_cellData == null || this.m_cellData.Length != this.m_width * this.m_height)
                this.InitializeArray(this.m_width, this.m_height);
            if (iy < 0 || ix < 0 || ix >= this.m_width || iy >= this.m_height)
                return (PrototypeDungeonRoomCellData) null;
            return iy * this.m_width + ix < 0 || iy * this.m_width + ix >= this.m_cellData.Length ? (PrototypeDungeonRoomCellData) null : this.m_cellData[iy * this.m_width + ix];
        }

        public PrototypeRoomExit GetExitDataAtPoint(int ix, int iy) => this.exitData[ix, iy];

        private bool IsValidCellDataPosition(int ix, int iy)
        {
            return iy >= 0 && iy < this.m_height && ix >= 0 && ix < this.m_width;
        }

        public bool ProcessExitPosition(int ix, int iy)
        {
            return this.exitData.ProcessExitPosition(ix, iy, this);
        }

        public bool HasFloorNeighbor(int ix, int iy)
        {
            if (this.m_cellData == null || this.m_cellData.Length != this.m_width * this.m_height)
                this.InitializeArray(this.m_width, this.m_height);
            if (ix == -1 || iy == -1 || ix == this.m_width || iy == this.m_height)
                return this.BoundaryHasFloorNeighbor(ix, iy);
            return iy < this.m_height - 1 && this.IsValidCellDataPosition(ix, iy + 1) && this.m_cellData[(iy + 1) * this.m_width + ix].state == CellType.FLOOR || iy > 0 && this.IsValidCellDataPosition(ix, iy - 1) && this.m_cellData[(iy - 1) * this.m_width + ix].state == CellType.FLOOR || ix < this.m_width - 1 && this.IsValidCellDataPosition(ix + 1, iy) && this.m_cellData[iy * this.m_width + (ix + 1)].state == CellType.FLOOR || ix > 0 && this.IsValidCellDataPosition(ix - 1, iy) && this.m_cellData[iy * this.m_width + (ix - 1)].state == CellType.FLOOR;
        }

        public bool HasBreakableNeighbor(int ix, int iy)
        {
            if (this.m_cellData == null || this.m_cellData.Length != this.m_width * this.m_height)
                this.InitializeArray(this.m_width, this.m_height);
            return iy < this.m_height - 1 && this.IsValidCellDataPosition(ix, iy + 1) && this.m_cellData[(iy + 1) * this.m_width + ix].breakable || iy > 0 && this.IsValidCellDataPosition(ix, iy - 1) && this.m_cellData[(iy - 1) * this.m_width + ix].breakable || ix < this.m_width - 1 && this.IsValidCellDataPosition(ix + 1, iy) && this.m_cellData[iy * this.m_width + (ix + 1)].breakable || ix > 0 && this.IsValidCellDataPosition(ix - 1, iy) && this.m_cellData[iy * this.m_width + (ix - 1)].breakable;
        }

        public bool HasNonWallNeighbor(int ix, int iy)
        {
            if (this.m_cellData == null || this.m_cellData.Length != this.m_width * this.m_height)
                this.InitializeArray(this.m_width, this.m_height);
            return this.IsValidCellDataPosition(ix, iy + 1) && this.m_cellData[(iy + 1) * this.m_width + ix].state != CellType.WALL || this.IsValidCellDataPosition(ix, iy - 1) && this.m_cellData[(iy - 1) * this.m_width + ix].state != CellType.WALL || this.IsValidCellDataPosition(ix + 1, iy) && this.m_cellData[iy * this.m_width + (ix + 1)].state != CellType.WALL || this.IsValidCellDataPosition(ix - 1, iy) && this.m_cellData[iy * this.m_width + (ix - 1)].state != CellType.WALL;
        }

        public bool HasNonWallNeighborWithDiagonals(int ix, int iy)
        {
            if (this.m_cellData == null || this.m_cellData.Length != this.m_width * this.m_height)
                this.InitializeArray(this.m_width, this.m_height);
            return this.IsValidCellDataPosition(ix, iy + 1) && this.m_cellData[(iy + 1) * this.m_width + ix].state != CellType.WALL || this.IsValidCellDataPosition(ix, iy - 1) && this.m_cellData[(iy - 1) * this.m_width + ix].state != CellType.WALL || this.IsValidCellDataPosition(ix + 1, iy) && this.m_cellData[iy * this.m_width + (ix + 1)].state != CellType.WALL || this.IsValidCellDataPosition(ix - 1, iy) && this.m_cellData[iy * this.m_width + (ix - 1)].state != CellType.WALL || this.IsValidCellDataPosition(ix + 1, iy + 1) && this.m_cellData[(iy + 1) * this.m_width + (ix + 1)].state != CellType.WALL || this.IsValidCellDataPosition(ix + 1, iy - 1) && this.m_cellData[(iy - 1) * this.m_width + (ix + 1)].state != CellType.WALL || this.IsValidCellDataPosition(ix - 1, iy + 1) && this.m_cellData[(iy + 1) * this.m_width + (ix - 1)].state != CellType.WALL || this.IsValidCellDataPosition(ix - 1, iy - 1) && this.m_cellData[(iy - 1) * this.m_width + (ix - 1)].state != CellType.WALL;
        }

        private bool BoundaryHasFloorNeighbor(int ix, int iy)
        {
            return ix == -1 && this.IsValidCellDataPosition(ix + 1, iy) && this.m_cellData[iy * this.m_width + (ix + 1)].state == CellType.FLOOR || ix == this.m_width && this.IsValidCellDataPosition(ix - 1, iy) && this.m_cellData[iy * this.m_width + (ix - 1)].state == CellType.FLOOR || iy == -1 && this.IsValidCellDataPosition(ix, iy + 1) && this.m_cellData[(iy + 1) * this.m_width + ix].state == CellType.FLOOR || iy == this.m_height && this.IsValidCellDataPosition(ix, iy - 1) && this.m_cellData[(iy - 1) * this.m_width + ix].state == CellType.FLOOR;
        }

        public DungeonData.Direction GetFloorDirection(int ix, int iy)
        {
            if (iy < this.m_height - 1 && this.IsValidCellDataPosition(ix, iy + 1) && this.m_cellData[(iy + 1) * this.m_width + ix].state == CellType.FLOOR)
                return DungeonData.Direction.NORTH;
            if (iy > 0 && this.IsValidCellDataPosition(ix, iy - 1) && this.m_cellData[(iy - 1) * this.m_width + ix].state == CellType.FLOOR)
                return DungeonData.Direction.SOUTH;
            if (ix < this.m_width - 1 && this.IsValidCellDataPosition(ix + 1, iy) && this.m_cellData[iy * this.m_width + (ix + 1)].state == CellType.FLOOR)
                return DungeonData.Direction.EAST;
            return ix > 0 && this.IsValidCellDataPosition(ix - 1, iy) && this.m_cellData[iy * this.m_width + (ix - 1)].state == CellType.FLOOR ? DungeonData.Direction.WEST : DungeonData.Direction.SOUTHWEST;
        }

        private void InitializeArray(int w, int h)
        {
            this.m_cellData = new PrototypeDungeonRoomCellData[w * h];
            for (int index1 = 0; index1 < w; ++index1)
            {
                for (int index2 = 0; index2 < h; ++index2)
                    this.m_cellData[index2 * w + index1] = new PrototypeDungeonRoomCellData(string.Empty, CellType.FLOOR);
            }
        }

        public void TranslateAndResize(int newWidth, int newHeight, int xTrans, int yTrans)
        {
            this.RecalculateCellDataArray(newWidth, newHeight, xTrans, yTrans);
            int endX = Math.Max(this.m_width, newWidth);
            int endY = Math.Max(this.m_height, newHeight);
            this.m_width = newWidth;
            this.m_height = newHeight;
            this.exitData.TranslateAllExits(xTrans, yTrans, this);
            this.TranslateAllObjectBasePositions(xTrans, yTrans, 0, endX, 0, endY);
        }

        private void TranslateAllObjectBasePositions(
            int deltaX,
            int deltaY,
            int startX,
            int endX,
            int startY,
            int endY)
        {
            foreach (PrototypePlacedObjectData placedObject in this.placedObjects)
            {
                if ((double) placedObject.contentsBasePosition.x >= (double) startX && (double) placedObject.contentsBasePosition.x < (double) endX && (double) placedObject.contentsBasePosition.y >= (double) startY && (double) placedObject.contentsBasePosition.y < (double) endY)
                    placedObject.contentsBasePosition += new Vector2((float) deltaX, (float) deltaY);
            }
            for (int index = 0; index < this.placedObjectPositions.Count; ++index)
            {
                if ((double) this.placedObjectPositions[index].x >= (double) startX && (double) this.placedObjectPositions[index].x < (double) endX && (double) this.placedObjectPositions[index].y >= (double) startY && (double) this.placedObjectPositions[index].y < (double) endY)
                    this.placedObjectPositions[index] += new Vector2((float) deltaX, (float) deltaY);
            }
            foreach (PrototypeRoomObjectLayer additionalObjectLayer in this.additionalObjectLayers)
            {
                foreach (PrototypePlacedObjectData placedObject in additionalObjectLayer.placedObjects)
                {
                    if ((double) placedObject.contentsBasePosition.x >= (double) startX && (double) placedObject.contentsBasePosition.x < (double) endX && (double) placedObject.contentsBasePosition.y >= (double) startY && (double) placedObject.contentsBasePosition.y < (double) endY)
                        placedObject.contentsBasePosition += new Vector2((float) deltaX, (float) deltaY);
                }
                for (int index = 0; index < additionalObjectLayer.placedObjectBasePositions.Count; ++index)
                {
                    if ((double) additionalObjectLayer.placedObjectBasePositions[index].x >= (double) startX && (double) additionalObjectLayer.placedObjectBasePositions[index].x < (double) endX && (double) additionalObjectLayer.placedObjectBasePositions[index].y >= (double) startY && (double) additionalObjectLayer.placedObjectBasePositions[index].y < (double) endY)
                        additionalObjectLayer.placedObjectBasePositions[index] += new Vector2((float) deltaX, (float) deltaY);
                }
            }
            this.ClearAndRebuildObjectCellData();
        }

        public List<PrototypeRoomExit> GetExitsMatchingDirection(
            DungeonData.Direction dir,
            PrototypeRoomExit.ExitType exitType)
        {
            List<PrototypeRoomExit> matchingDirection = new List<PrototypeRoomExit>();
            for (int index = 0; index < this.exitData.exits.Count; ++index)
            {
                if (this.exitData.exits[index].exitDirection == dir)
                {
                    if (exitType == PrototypeRoomExit.ExitType.NO_RESTRICTION)
                        matchingDirection.Add(this.exitData.exits[index]);
                    else if (exitType == PrototypeRoomExit.ExitType.EXIT_ONLY && this.exitData.exits[index].exitType != PrototypeRoomExit.ExitType.ENTRANCE_ONLY)
                        matchingDirection.Add(this.exitData.exits[index]);
                    else if (exitType == PrototypeRoomExit.ExitType.ENTRANCE_ONLY && this.exitData.exits[index].exitType != PrototypeRoomExit.ExitType.EXIT_ONLY)
                        matchingDirection.Add(this.exitData.exits[index]);
                }
            }
            return matchingDirection;
        }

        public List<Tuple<PrototypeRoomExit, PrototypeRoomExit>> GetExitPairsMatchingDirections(
            DungeonData.Direction dir1,
            DungeonData.Direction dir2)
        {
            List<Tuple<PrototypeRoomExit, PrototypeRoomExit>> matchingDirections = new List<Tuple<PrototypeRoomExit, PrototypeRoomExit>>();
            for (int index1 = 0; index1 < this.exitData.exits.Count; ++index1)
            {
                PrototypeRoomExit exit1 = this.exitData.exits[index1];
                for (int index2 = 0; index2 < this.exitData.exits.Count; ++index2)
                {
                    PrototypeRoomExit exit2 = this.exitData.exits[index2];
                    if (exit1.exitDirection == dir1 && exit2.exitDirection == dir2)
                        matchingDirections.Add(new Tuple<PrototypeRoomExit, PrototypeRoomExit>(exit1, exit2));
                }
            }
            return matchingDirections;
        }

        public void ClearAndRebuildObjectCellData()
        {
            if (this.m_cellData == null)
                return;
            foreach (PrototypeDungeonRoomCellData dungeonRoomCellData in this.m_cellData)
            {
                dungeonRoomCellData.placedObjectRUBELIndex = -1;
                dungeonRoomCellData.additionalPlacedObjectIndices.Clear();
            }
            this.RebuildObjectCellData();
        }

        public void RebuildObjectCellData()
        {
            using (List<PrototypePlacedObjectData>.Enumerator enumerator = this.placedObjects.GetEnumerator())
            {
    label_22:
                while (enumerator.MoveNext())
                {
                    PrototypePlacedObjectData current = enumerator.Current;
                    Vector2 contentsBasePosition = current.contentsBasePosition;
                    for (int index1 = 0; index1 < this.Height; ++index1)
                    {
                        for (int index2 = 0; index2 < this.Width; ++index2)
                        {
                            Vector2 vector2 = new Vector2((float) index2, (float) index1);
                            PrototypeDungeonRoomCellData cellDataAtPoint = this.ForceGetCellDataAtPoint(index2, index1);
                            if (cellDataAtPoint != null && cellDataAtPoint.placedObjectRUBELIndex >= 0 && this.placedObjects[cellDataAtPoint.placedObjectRUBELIndex] == current)
                            {
                                if (contentsBasePosition != vector2)
                                    cellDataAtPoint.placedObjectRUBELIndex = -1;
                                else
                                    goto label_22;
                            }
                        }
                    }
                    if (current == null)
                    {
                        Debug.LogError((object) "null object data at placed object index!");
                    }
                    else
                    {
                        for (int x = (int) contentsBasePosition.x; (double) x < (double) contentsBasePosition.x + (double) current.GetWidth(); ++x)
                        {
                            for (int y = (int) contentsBasePosition.y; (double) y < (double) contentsBasePosition.y + (double) current.GetHeight(); ++y)
                            {
                                if (y * this.Width + x >= 0 && y * this.Width + x < this.m_cellData.Length)
                                {
                                    PrototypeDungeonRoomCellData cellDataAtPoint = this.ForceGetCellDataAtPoint(x, y);
                                    if (cellDataAtPoint != null)
                                        cellDataAtPoint.placedObjectRUBELIndex = this.placedObjects.IndexOf(current);
                                }
                            }
                        }
                    }
                }
            }
            foreach (PrototypeRoomObjectLayer additionalObjectLayer in this.additionalObjectLayers)
            {
                int index3 = this.additionalObjectLayers.IndexOf(additionalObjectLayer);
    label_52:
                for (int index4 = 0; index4 < additionalObjectLayer.placedObjects.Count; ++index4)
                {
                    PrototypePlacedObjectData placedObject = additionalObjectLayer.placedObjects[index4];
                    Vector2 contentsBasePosition = placedObject.contentsBasePosition;
                    for (int index5 = 0; index5 < this.Height; ++index5)
                    {
                        for (int index6 = 0; index6 < this.Width; ++index6)
                        {
                            Vector2 vector2 = new Vector2((float) index6, (float) index5);
                            PrototypeDungeonRoomCellData cellDataAtPoint = this.ForceGetCellDataAtPoint(index6, index5);
                            if (cellDataAtPoint != null)
                            {
                                int index7 = cellDataAtPoint.additionalPlacedObjectIndices.Count <= index3 ? -1 : cellDataAtPoint.additionalPlacedObjectIndices[index3];
                                if (index7 >= 0 && additionalObjectLayer.placedObjects[index7] == placedObject)
                                {
                                    if (contentsBasePosition != vector2)
                                        cellDataAtPoint.additionalPlacedObjectIndices[index3] = -1;
                                    else
                                        goto label_52;
                                }
                            }
                        }
                    }
                    if (placedObject == null)
                    {
                        Debug.LogError((object) ("null object data at placed object index in layer: " + (object) this.additionalObjectLayers.IndexOf(additionalObjectLayer)));
                    }
                    else
                    {
                        for (int x = (int) contentsBasePosition.x; (double) x < (double) contentsBasePosition.x + (double) placedObject.GetWidth(); ++x)
                        {
                            for (int y = (int) contentsBasePosition.y; (double) y < (double) contentsBasePosition.y + (double) placedObject.GetHeight(); ++y)
                            {
                                if (y * this.Width + x >= 0 && y * this.Width + x < this.m_cellData.Length)
                                {
                                    PrototypeDungeonRoomCellData cellDataAtPoint = this.ForceGetCellDataAtPoint(x, y);
                                    if (cellDataAtPoint != null)
                                    {
                                        if (cellDataAtPoint.additionalPlacedObjectIndices.Count <= index3)
                                        {
                                            while (cellDataAtPoint.additionalPlacedObjectIndices.Count <= index3)
                                                cellDataAtPoint.additionalPlacedObjectIndices.Add(-1);
                                        }
                                        cellDataAtPoint.additionalPlacedObjectIndices[index3] = additionalObjectLayer.placedObjects.IndexOf(placedObject);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void RecalculateCellDataArray(int newWidth, int newHeight, int xTrans = 0, int yTrans = 0)
        {
            if (this.m_cellData == null)
            {
                this.InitializeArray(newWidth, newHeight);
            }
            else
            {
                PrototypeDungeonRoomCellData[] dungeonRoomCellDataArray = new PrototypeDungeonRoomCellData[newWidth * newHeight];
                for (int ix = 0; ix < this.m_width; ++ix)
                {
                    for (int iy = 0; iy < this.m_height; ++iy)
                    {
                        if (ix < newWidth && iy < newHeight)
                        {
                            int num1 = ix + xTrans;
                            int num2 = iy + yTrans;
                            if (num1 >= 0 && num1 < newWidth && num2 >= 0 && num2 < newHeight)
                                dungeonRoomCellDataArray[num2 * newWidth + num1] = this.ForceGetCellDataAtPoint(ix, iy);
                        }
                    }
                }
                for (int index1 = 0; index1 < newWidth; ++index1)
                {
                    for (int index2 = 0; index2 < newHeight; ++index2)
                    {
                        if (dungeonRoomCellDataArray[index2 * newWidth + index1] == null)
                            dungeonRoomCellDataArray[index2 * newWidth + index1] = new PrototypeDungeonRoomCellData(string.Empty, CellType.WALL);
                    }
                }
                this.m_cellData = dungeonRoomCellDataArray;
            }
        }

        public enum RoomCategory
        {
            CONNECTOR,
            HUB,
            NORMAL,
            BOSS,
            REWARD,
            SPECIAL,
            SECRET,
            ENTRANCE,
            EXIT,
        }

        public enum RoomNormalSubCategory
        {
            COMBAT,
            TRAP,
        }

        public enum RoomBossSubCategory
        {
            FLOOR_BOSS,
            MINI_BOSS,
        }

        public enum RoomSpecialSubCategory
        {
            UNSPECIFIED_SPECIAL,
            STANDARD_SHOP,
            WEIRD_SHOP,
            MAIN_STORY,
            NPC_STORY,
            CATACOMBS_BRIDGE_ROOM,
        }

        public enum RoomSecretSubCategory
        {
            UNSPECIFIED_SECRET,
        }
    }

