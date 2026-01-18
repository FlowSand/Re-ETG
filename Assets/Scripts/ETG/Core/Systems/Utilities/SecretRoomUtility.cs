using System.Collections.Generic;

using tk2dRuntime.TileMap;
using UnityEngine;

using Dungeonator;

#nullable disable

    public static class SecretRoomUtility
    {
        public static Dictionary<TilesetIndexMetadata.TilesetFlagType, List<Tuple<int, TilesetIndexMetadata>>> metadataLookupTableRef;
        public static bool FloorHasComplexPuzzle;

        private static bool IsSolid(CellData cell) => cell.type == CellType.WALL || cell.isSecretRoomCell;

        private static bool IsFaceWallHigher(int x, int y, CellData[][] t)
        {
            return SecretRoomUtility.IsSolid(t[x][y]) && SecretRoomUtility.IsSolid(t[x][y - 1]) && !SecretRoomUtility.IsSolid(t[x][y - 2]);
        }

        private static bool IsFaceWallLower(int x, int y, CellData[][] t)
        {
            return SecretRoomUtility.IsSolid(t[x][y]) && !SecretRoomUtility.IsSolid(t[x][y - 1]);
        }

        public static void ClearPerLevelData() => SecretRoomUtility.FloorHasComplexPuzzle = false;

        public static int GetIndexFromTupleArray(
            CellData current,
            List<Tuple<int, TilesetIndexMetadata>> list,
            int roomTypeIndex,
            float forcedRand = -1f)
        {
            float num1 = 0.0f;
            for (int index = 0; index < list.Count; ++index)
            {
                Tuple<int, TilesetIndexMetadata> tuple = list[index];
                if (!tuple.Second.usesAnimSequence && (tuple.Second.dungeonRoomSubType == roomTypeIndex || tuple.Second.secondRoomSubType == roomTypeIndex || tuple.Second.thirdRoomSubType == roomTypeIndex))
                    num1 += tuple.Second.weight;
            }
            float num2 = current.UniqueHash * num1;
            if ((double) forcedRand >= 0.0)
                num2 = forcedRand * num1;
            for (int index = 0; index < list.Count; ++index)
            {
                Tuple<int, TilesetIndexMetadata> tuple = list[index];
                if (!tuple.Second.usesAnimSequence && (tuple.Second.dungeonRoomSubType == roomTypeIndex || tuple.Second.secondRoomSubType == roomTypeIndex || tuple.Second.thirdRoomSubType == roomTypeIndex))
                {
                    num2 -= tuple.Second.weight;
                    if ((double) num2 <= 0.0)
                        return tuple.First;
                }
            }
            return list[0].First;
        }

        private static bool IsSecretDoorTopBorder(CellData cellToCheck, DungeonData data)
        {
            return cellToCheck.isSecretRoomCell && (data.cellData[cellToCheck.position.x][cellToCheck.position.y - 1].type == CellType.FLOOR && !data.cellData[cellToCheck.position.x][cellToCheck.position.y - 1].isSecretRoomCell || data.cellData[cellToCheck.position.x][cellToCheck.position.y - 2].type == CellType.FLOOR && !data.cellData[cellToCheck.position.x][cellToCheck.position.y - 2].isSecretRoomCell);
        }

        private static int GetIndexGivenCell(
            IntVector2 position,
            List<IntVector2> cellRepresentation,
            DungeonData data,
            out int facewall,
            out int sidewall)
        {
            facewall = 0;
            sidewall = 0;
            TileIndexGrid gridForCellPosition = SecretRoomUtility.GetBorderGridForCellPosition(position, data);
            CellData cellData = data.cellData[position.x][position.y];
            List<CellData> cellNeighbors = data.GetCellNeighbors(cellData);
            if (cellNeighbors[1].type == CellType.FLOOR && !cellNeighbors[1].isSecretRoomCell)
                sidewall = 1;
            if (cellNeighbors[3].type == CellType.FLOOR && !cellNeighbors[3].isSecretRoomCell)
                sidewall = -1;
            if (cellNeighbors[2].type == CellType.FLOOR && !cellNeighbors[2].isSecretRoomCell)
            {
                facewall = 1;
                return SecretRoomUtility.GetIndexFromTupleArray(cellData, SecretRoomUtility.metadataLookupTableRef[TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER], cellData.cellVisualData.roomVisualTypeIndex);
            }
            if (data.cellData[cellData.position.x][cellData.position.y - 2].type == CellType.FLOOR && !data.cellData[cellData.position.x][cellData.position.y - 2].isSecretRoomCell)
            {
                facewall = 2;
                return SecretRoomUtility.GetIndexFromTupleArray(cellData, SecretRoomUtility.metadataLookupTableRef[TilesetIndexMetadata.TilesetFlagType.FACEWALL_UPPER], cellData.cellVisualData.roomVisualTypeIndex);
            }
            bool[] flagArray = new bool[4];
            for (int index = 0; index < 4; ++index)
            {
                bool flag1 = SecretRoomUtility.IsFaceWallHigher(cellNeighbors[index].position.x, cellNeighbors[index].position.y, data.cellData) || SecretRoomUtility.IsFaceWallLower(cellNeighbors[index].position.x, cellNeighbors[index].position.y, data.cellData);
                bool flag2 = cellNeighbors[index].type != CellType.WALL && !cellNeighbors[index].isSecretRoomCell && SecretRoomUtility.IsSolid(data.cellData[cellNeighbors[index].position.x][cellNeighbors[index].position.y - 1]);
                if ((cellNeighbors[index].type != CellType.WALL || flag1) && !cellNeighbors[index].isSecretRoomCell && !flag2)
                    flagArray[index] = true;
                if (SecretRoomUtility.IsSecretDoorTopBorder(cellNeighbors[index], data))
                {
                    flagArray[index] = true;
                    facewall = 3;
                }
                if (flagArray[index] && (cellData.type != CellType.WALL && !cellData.IsTopWall() || cellData.IsAnyFaceWall()))
                    facewall = 3;
            }
            return gridForCellPosition.GetIndexGivenSides(flagArray[0], flagArray[1], flagArray[2], flagArray[3]);
        }

        private static TileIndexGrid GetBorderGridForCellPosition(IntVector2 position, DungeonData data)
        {
            TileIndexGrid ceilingBorderGrid = GameManager.Instance.Dungeon.roomMaterialDefinitions[data.cellData[position.x][position.y].cellVisualData.roomVisualTypeIndex].roomCeilingBorderGrid;
            if ((Object) ceilingBorderGrid == (Object) null)
                ceilingBorderGrid = GameManager.Instance.Dungeon.roomMaterialDefinitions[0].roomCeilingBorderGrid;
            return ceilingBorderGrid;
        }

        private static void BuildRoomCoverExitIndices(
            RoomHandler room,
            tk2dTileMap tileMap,
            DungeonData dungeonData,
            List<IntVector2> cellRepresentation,
            HashSet<SecretRoomUtility.IntVector2WithIndex> ceilingCells,
            HashSet<SecretRoomUtility.IntVector2WithIndex> borderCells,
            HashSet<SecretRoomUtility.IntVector2WithIndex> facewallCells)
        {
            for (int index1 = 0; index1 < room.area.instanceUsedExits.Count; ++index1)
            {
                PrototypeRoomExit instanceUsedExit = room.area.instanceUsedExits[index1];
                RuntimeRoomExitData exitToLocalData1 = room.area.exitToLocalDataMap[instanceUsedExit];
                PrototypeRoomExit exitConnectedToRoom = room.connectedRoomsByExit[instanceUsedExit].GetExitConnectedToRoom(room);
                RuntimeRoomExitData exitToLocalData2 = room.connectedRoomsByExit[instanceUsedExit].area.exitToLocalDataMap[exitConnectedToRoom];
                int num = exitToLocalData1.TotalExitLength + exitToLocalData2.TotalExitLength - 1;
                if (instanceUsedExit.exitDirection == DungeonData.Direction.NORTH)
                    num += 2;
                for (int index2 = 0; index2 < instanceUsedExit.containedCells.Count; ++index2)
                {
                    for (int index3 = 0; index3 < num; ++index3)
                    {
                        IntVector2 intVector2_1 = instanceUsedExit.containedCells[index2].ToIntVector2() + room.area.basePosition - IntVector2.One;
                        List<IntVector2> intVector2List = new List<IntVector2>();
                        IntVector2 intVector2_2;
                        if (instanceUsedExit.exitDirection == DungeonData.Direction.NORTH)
                        {
                            intVector2_2 = intVector2_1 + IntVector2.Up * index3;
                            intVector2List.Add(intVector2_2 + IntVector2.Left);
                            intVector2List.Add(intVector2_2 + IntVector2.Right);
                        }
                        else if (instanceUsedExit.exitDirection == DungeonData.Direction.SOUTH)
                        {
                            intVector2_2 = intVector2_1 + IntVector2.Down * index3;
                            if (index3 < num - 2)
                            {
                                intVector2List.Add(intVector2_2 + IntVector2.Left);
                                intVector2List.Add(intVector2_2 + IntVector2.Right);
                            }
                        }
                        else if (instanceUsedExit.exitDirection == DungeonData.Direction.EAST)
                        {
                            intVector2_2 = intVector2_1 + IntVector2.Right * index3;
                            intVector2List.Add(intVector2_2 + IntVector2.Up);
                            intVector2List.Add(intVector2_2 + IntVector2.Up * 2);
                            intVector2List.Add(intVector2_2 + IntVector2.Up * 3);
                        }
                        else
                        {
                            intVector2_2 = intVector2_1 + IntVector2.Left * index3;
                            intVector2List.Add(intVector2_2 + IntVector2.Up);
                            intVector2List.Add(intVector2_2 + IntVector2.Up * 2);
                            intVector2List.Add(intVector2_2 + IntVector2.Up * 3);
                        }
                        intVector2List.Add(intVector2_2);
                        for (int index4 = 0; index4 < intVector2List.Count; ++index4)
                        {
                            int facewall = 0;
                            int sidewall = 0;
                            int indexGivenCell = SecretRoomUtility.GetIndexGivenCell(intVector2List[index4], cellRepresentation, dungeonData, out facewall, out sidewall);
                            TileIndexGrid gridForCellPosition = SecretRoomUtility.GetBorderGridForCellPosition(intVector2List[index4], dungeonData);
                            if (facewall > 0)
                            {
                                SecretRoomUtility.IntVector2WithIndex vector2WithIndex = new SecretRoomUtility.IntVector2WithIndex(intVector2List[index4], indexGivenCell);
                                vector2WithIndex.facewallID = facewall;
                                vector2WithIndex.sidewallID = sidewall;
                                switch (facewall)
                                {
                                    case 1:
                                        vector2WithIndex.meshColor = new Color[4]
                                        {
                                            new Color(0.0f, 0.0f, 1f),
                                            new Color(0.0f, 0.0f, 1f),
                                            new Color(0.0f, 0.5f, 1f),
                                            new Color(0.0f, 0.5f, 1f)
                                        };
                                        facewallCells.Add(vector2WithIndex);
                                        continue;
                                    case 2:
                                        vector2WithIndex.meshColor = new Color[4]
                                        {
                                            new Color(0.0f, 0.5f, 1f),
                                            new Color(0.0f, 0.5f, 1f),
                                            new Color(0.0f, 1f, 1f),
                                            new Color(0.0f, 1f, 1f)
                                        };
                                        facewallCells.Add(vector2WithIndex);
                                        continue;
                                    case 3:
                                        if (!gridForCellPosition.centerIndices.indices.Contains(vector2WithIndex.index))
                                            facewallCells.Add(vector2WithIndex);
                                        ceilingCells.Add(new SecretRoomUtility.IntVector2WithIndex(intVector2List[index4], gridForCellPosition.centerIndices.GetIndexByWeight())
                                        {
                                            zOffset = 1.5f
                                        });
                                        continue;
                                    default:
                                        continue;
                                }
                            }
                            else if (gridForCellPosition.centerIndices.indices.Contains(indexGivenCell))
                            {
                                ceilingCells.Add(new SecretRoomUtility.IntVector2WithIndex(intVector2List[index4], indexGivenCell)
                                {
                                    sidewallID = sidewall
                                });
                            }
                            else
                            {
                                SecretRoomUtility.IntVector2WithIndex vector2WithIndex1 = new SecretRoomUtility.IntVector2WithIndex(intVector2List[index4], indexGivenCell);
                                vector2WithIndex1.sidewallID = sidewall;
                                ++vector2WithIndex1.zOffset;
                                borderCells.Add(vector2WithIndex1);
                                SecretRoomUtility.IntVector2WithIndex vector2WithIndex2 = new SecretRoomUtility.IntVector2WithIndex(intVector2List[index4], gridForCellPosition.centerIndices.GetIndexByWeight());
                                vector2WithIndex2.sidewallID = sidewall;
                                vector2WithIndex2.zOffset += 0.75f;
                                ceilingCells.Add(vector2WithIndex2);
                            }
                        }
                    }
                }
            }
        }

        private static Mesh BuildAOMesh(
            HashSet<SecretRoomUtility.IntVector2WithIndex> facewallIndices,
            out Material material)
        {
            material = (Material) null;
            Mesh mesh = new Mesh();
            List<Vector3> vector3List = new List<Vector3>();
            List<int> intList = new List<int>();
            List<Vector2> vector2List = new List<Vector2>();
            List<Color> colorList = new List<Color>();
            tk2dSpriteCollectionData dungeonCollection = GameManager.Instance.Dungeon.tileIndices.dungeonCollection;
            HashSet<IntVector2> intVector2Set = new HashSet<IntVector2>();
            foreach (SecretRoomUtility.IntVector2WithIndex facewallIndex in facewallIndices)
            {
                if (!intVector2Set.Contains(facewallIndex.position))
                {
                    intVector2Set.Add(facewallIndex.position);
                    int index1 = -1;
                    Vector3 zero = Vector3.zero;
                    if (facewallIndex.facewallID == 1)
                        index1 = BuilderUtil.GetTileFromRawTile(GameManager.Instance.Dungeon.tileIndices.aoTileIndices.AOFloorTileIndex);
                    else if (facewallIndex.sidewallID == 1)
                    {
                        index1 = BuilderUtil.GetTileFromRawTile(GameManager.Instance.Dungeon.tileIndices.aoTileIndices.AOFloorWallLeft);
                        zero += Vector3.right + Vector3.down + Vector3.forward;
                    }
                    else if (facewallIndex.sidewallID == -1)
                        index1 = BuilderUtil.GetTileFromRawTile(GameManager.Instance.Dungeon.tileIndices.aoTileIndices.AOFloorWallRight);
                    if (index1 != -1)
                    {
                        tk2dSpriteDefinition spriteDefinition1 = dungeonCollection.spriteDefinitions[index1];
                        if ((Object) material == (Object) null)
                            material = spriteDefinition1.material;
                        int count1 = vector3List.Count;
                        Vector3 vector3_1 = facewallIndex.position.ToVector3((float) (facewallIndex.position.y - 1)) + zero;
                        --vector3_1.y;
                        Vector3[] vector3Array1 = spriteDefinition1.ConstructExpensivePositions();
                        for (int index2 = 0; index2 < vector3Array1.Length; ++index2)
                        {
                            Vector3 vector = vector3Array1[index2];
                            vector = vector.WithZ(vector.y);
                            if (facewallIndex.facewallID == 1)
                                vector.z += 2f;
                            vector3List.Add(vector3_1 + vector + facewallIndex.GetOffset());
                            vector2List.Add(spriteDefinition1.uvs[index2]);
                            colorList.Add(facewallIndex.meshColor[index2 % 4]);
                        }
                        for (int index3 = 0; index3 < spriteDefinition1.indices.Length; ++index3)
                            intList.Add(count1 + spriteDefinition1.indices[index3]);
                        if (facewallIndex.facewallID == 1)
                        {
                            int tileFromRawTile = BuilderUtil.GetTileFromRawTile(GameManager.Instance.Dungeon.tileIndices.aoTileIndices.AOBottomWallBaseTileIndex);
                            tk2dSpriteDefinition spriteDefinition2 = dungeonCollection.spriteDefinitions[tileFromRawTile];
                            int count2 = vector3List.Count;
                            Vector3 vector3_2 = facewallIndex.position.ToVector3((float) facewallIndex.position.y);
                            Vector3[] vector3Array2 = spriteDefinition2.ConstructExpensivePositions();
                            for (int index4 = 0; index4 < vector3Array2.Length; ++index4)
                            {
                                Vector3 vector = vector3Array2[index4];
                                vector = vector.WithZ(-vector.y);
                                if (facewallIndex.facewallID == 1)
                                    vector.z += 2f;
                                vector3List.Add(vector3_2 + vector + facewallIndex.GetOffset());
                                vector2List.Add(spriteDefinition2.uvs[index4]);
                                colorList.Add(facewallIndex.meshColor[index4 % 4]);
                            }
                            for (int index5 = 0; index5 < spriteDefinition2.indices.Length; ++index5)
                                intList.Add(count2 + spriteDefinition2.indices[index5]);
                        }
                    }
                }
            }
            mesh.vertices = vector3List.ToArray();
            mesh.triangles = intList.ToArray();
            mesh.uv = vector2List.ToArray();
            mesh.colors = colorList.ToArray();
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            return mesh;
        }

        private static Mesh BuildTargetMesh(
            HashSet<SecretRoomUtility.IntVector2WithIndex> cellIndices,
            out Material material,
            bool facewall = false)
        {
            material = (Material) null;
            Mesh mesh = new Mesh();
            List<Vector3> vector3List1 = new List<Vector3>();
            List<int> intList = new List<int>();
            List<Vector2> vector2List = new List<Vector2>();
            List<Color> colorList = new List<Color>();
            List<Vector3> vector3List2 = new List<Vector3>();
            tk2dSpriteCollectionData dungeonCollection = GameManager.Instance.Dungeon.tileIndices.dungeonCollection;
            HashSet<IntVector2> intVector2Set = new HashSet<IntVector2>();
            foreach (SecretRoomUtility.IntVector2WithIndex cellIndex in cellIndices)
            {
                if (!intVector2Set.Contains(cellIndex.position))
                {
                    intVector2Set.Add(cellIndex.position);
                    int tileFromRawTile = BuilderUtil.GetTileFromRawTile(cellIndex.index);
                    tk2dSpriteDefinition spriteDefinition = dungeonCollection.spriteDefinitions[tileFromRawTile];
                    if ((Object) material == (Object) null)
                        material = spriteDefinition.material;
                    int count = vector3List1.Count;
                    Vector3 vector3_1 = cellIndex.position.ToVector3((float) cellIndex.position.y);
                    Vector3[] vector3Array = spriteDefinition.ConstructExpensivePositions();
                    for (int index = 0; index < vector3Array.Length; ++index)
                    {
                        Vector3 vector = vector3Array[index];
                        Vector3 vector3_2;
                        if (facewall)
                        {
                            if (cellIndex.facewallID > 2)
                            {
                                vector3_2 = vector.WithZ(vector.y);
                                vector3_2.z -= 2.25f;
                            }
                            else
                            {
                                vector3_2 = vector.WithZ(-vector.y);
                                if (cellIndex.facewallID == 1)
                                    vector3_2.z += 2f;
                            }
                        }
                        else
                            vector3_2 = vector.WithZ(vector.y);
                        vector3List1.Add(vector3_1 + vector3_2 + cellIndex.GetOffset());
                        vector3List2.Add(Vector3.back);
                        vector2List.Add(spriteDefinition.uvs[index]);
                        colorList.Add(cellIndex.meshColor[index % 4]);
                    }
                    for (int index = 0; index < spriteDefinition.indices.Length; ++index)
                        intList.Add(count + spriteDefinition.indices[index]);
                }
            }
            mesh.vertices = vector3List1.ToArray();
            mesh.triangles = intList.ToArray();
            mesh.normals = vector3List2.ToArray();
            mesh.uv = vector2List.ToArray();
            mesh.colors = colorList.ToArray();
            mesh.RecalculateBounds();
            return mesh;
        }

        private static GameObject CreateObjectForMesh(
            Mesh meshTarget,
            Material materialTarget,
            float zHeight,
            Transform parentObject,
            bool ao = false)
        {
            GameObject objectForMesh = new GameObject("Secret Room Mesh");
            objectForMesh.transform.position = new Vector3(0.0f, 0.0f, zHeight);
            objectForMesh.transform.parent = parentObject;
            MeshFilter meshFilter = objectForMesh.AddComponent<MeshFilter>();
            MeshRenderer r = objectForMesh.AddComponent<MeshRenderer>();
            meshFilter.sharedMesh = meshTarget;
            r.sharedMaterial = materialTarget;
            DepthLookupManager.ProcessRenderer((Renderer) r, DepthLookupManager.GungeonSortingLayer.FOREGROUND);
            if (!ao)
                objectForMesh.layer = LayerMask.NameToLayer("ShadowCaster");
            return objectForMesh;
        }

        public static List<SecretRoomExitData> BuildRoomExitColliders(RoomHandler room)
        {
            List<SecretRoomExitData> secretRoomExitDataList = new List<SecretRoomExitData>();
            if (!room.area.IsProceduralRoom)
            {
                for (int index = 0; index < room.area.instanceUsedExits.Count; ++index)
                {
                    if (!room.area.exitToLocalDataMap[room.area.instanceUsedExits[index]].oneWayDoor)
                    {
                        RuntimeExitDefinition runtimeExitDefinition = room.exitDefinitionsByExit[room.area.exitToLocalDataMap[room.area.instanceUsedExits[index]]];
                        if (Dungeon.IsGenerating || runtimeExitDefinition.downstreamRoom == room || !((Object) runtimeExitDefinition.downstreamRoom.area.prototypeRoom != (Object) null) || runtimeExitDefinition.downstreamRoom.area.prototypeRoom.category != PrototypeDungeonRoom.RoomCategory.SECRET)
                        {
                            GameObject g = new GameObject("secret exit collider");
                            SpeculativeRigidbody speculativeRigidbody = g.AddComponent<SpeculativeRigidbody>();
                            g.AddComponent<PersistentVFXManagerBehaviour>();
                            speculativeRigidbody.CollideWithTileMap = false;
                            speculativeRigidbody.CollideWithOthers = true;
                            speculativeRigidbody.PixelColliders = new List<PixelCollider>();
                            PrototypeRoomExit instanceUsedExit = room.area.instanceUsedExits[index];
                            RuntimeRoomExitData exitToLocalData = room.area.exitToLocalDataMap[instanceUsedExit];
                            RoomHandler roomHandler = room.connectedRoomsByExit[instanceUsedExit];
                            PrototypeRoomExit exitConnectedToRoom = roomHandler.GetExitConnectedToRoom(room);
                            int num1 = roomHandler.area.exitToLocalDataMap[exitConnectedToRoom].TotalExitLength - 1;
                            IntVector2 intVector2_1 = IntVector2.Zero;
                            IntVector2 intVector2_2 = IntVector2.Zero;
                            int num2 = 0;
                            if (instanceUsedExit.exitDirection == DungeonData.Direction.NORTH)
                            {
                                intVector2_1 = room.area.basePosition + exitToLocalData.ExitOrigin + IntVector2.Left + num1 * IntVector2.Up;
                                intVector2_2 = new IntVector2(2, 1);
                                num2 = 8;
                            }
                            else if (instanceUsedExit.exitDirection == DungeonData.Direction.EAST)
                            {
                                intVector2_1 = room.area.basePosition + exitToLocalData.ExitOrigin + IntVector2.NegOne + num1 * IntVector2.Right;
                                intVector2_2 = new IntVector2(1, 4);
                            }
                            else if (instanceUsedExit.exitDirection == DungeonData.Direction.WEST)
                            {
                                intVector2_1 = room.area.basePosition + exitToLocalData.ExitOrigin + IntVector2.NegOne + num1 * IntVector2.Left;
                                intVector2_2 = new IntVector2(1, 4);
                            }
                            else if (instanceUsedExit.exitDirection == DungeonData.Direction.SOUTH)
                            {
                                IntVector2 intVector2_3 = room.area.basePosition + exitToLocalData.ExitOrigin + IntVector2.NegOne + num1 * IntVector2.Down;
                                speculativeRigidbody.PixelColliders.Add(new PixelCollider()
                                {
                                    ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                                    CollisionLayer = CollisionLayer.LowObstacle,
                                    ManualOffsetX = 0,
                                    ManualOffsetY = -16,
                                    ManualWidth = 32 /*0x20*/,
                                    ManualHeight = 16 /*0x10*/
                                });
                                intVector2_1 = intVector2_3 + IntVector2.Up;
                                intVector2_2 = new IntVector2(2, 1);
                            }
                            g.transform.position = intVector2_1.ToVector3();
                            speculativeRigidbody.PixelColliders.Add(new PixelCollider()
                            {
                                ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                                CollisionLayer = CollisionLayer.HighObstacle,
                                ManualWidth = 16 /*0x10*/ * intVector2_2.x,
                                ManualHeight = 16 /*0x10*/ * intVector2_2.y + num2
                            });
                            speculativeRigidbody.ForceRegenerate();
                            secretRoomExitDataList.Add(new SecretRoomExitData(g, instanceUsedExit.exitDirection));
                        }
                    }
                }
            }
            else
                Debug.LogError((object) "no support for secret procedural rooms yet.");
            return secretRoomExitDataList;
        }

internal class IntVector2WithIndexEqualityComparer : 
            IEqualityComparer<SecretRoomUtility.IntVector2WithIndex>
        {
            public bool Equals(
                SecretRoomUtility.IntVector2WithIndex v1,
                SecretRoomUtility.IntVector2WithIndex v2)
            {
                return v1.position == v2.position;
            }

            public int GetHashCode(SecretRoomUtility.IntVector2WithIndex v1) => v1.position.GetHashCode();
        }

        internal class IntVector2WithIndex
        {
            public IntVector2 position;
            public int index;
            public float zOffset;
            public Color[] meshColor;
            public int facewallID;
            public int sidewallID;

            public IntVector2WithIndex(IntVector2 vec, int i)
            {
                this.position = vec;
                this.index = i;
                this.meshColor = new Color[4]
                {
                    Color.black,
                    Color.black,
                    Color.black,
                    Color.black
                };
            }

            public IntVector2WithIndex(IntVector2 vec, int i, Color c)
            {
                this.position = vec;
                this.index = i;
                this.meshColor = new Color[4]{ c, c, c, c };
            }

            public IntVector2WithIndex(IntVector2 vec, int i, Color bottom, Color top)
            {
                this.position = vec;
                this.index = i;
                this.meshColor = new Color[4]
                {
                    bottom,
                    bottom,
                    top,
                    top
                };
            }

            public Vector3 GetOffset() => new Vector3(0.0f, 0.0f, this.zOffset);
        }
    }

