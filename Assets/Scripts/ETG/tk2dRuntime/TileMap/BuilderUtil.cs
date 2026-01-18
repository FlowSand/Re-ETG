using System.Collections.Generic;

using UnityEngine;

using Dungeonator;

#nullable disable
namespace tk2dRuntime.TileMap
{
    public static class BuilderUtil
    {
        private static List<int> TilePrefabsX;
        private static List<int> TilePrefabsY;
        private static List<int> TilePrefabsLayer;
        private static List<GameObject> TilePrefabsInstance;
        private const int tileMask = 16777215;

        public static bool InitDataStore(tk2dTileMap tileMap)
        {
            bool flag1 = false;
            int numLayers = tileMap.data.NumLayers;
            if (tileMap.Layers == null)
            {
                tileMap.Layers = new Layer[numLayers];
                for (int index = 0; index < numLayers; ++index)
                    tileMap.Layers[index] = new Layer(tileMap.data.Layers[index].hash, tileMap.width, tileMap.height, tileMap.partitionSizeX, tileMap.partitionSizeY);
                flag1 = true;
            }
            else
            {
                Layer[] layerArray = new Layer[numLayers];
                for (int index1 = 0; index1 < numLayers; ++index1)
                {
                    LayerInfo layer = tileMap.data.Layers[index1];
                    bool flag2 = false;
                    for (int index2 = 0; index2 < tileMap.Layers.Length; ++index2)
                    {
                        if (tileMap.Layers[index2].hash == layer.hash)
                        {
                            layerArray[index1] = tileMap.Layers[index2];
                            flag2 = true;
                            break;
                        }
                    }
                    if (!flag2)
                        layerArray[index1] = new Layer(layer.hash, tileMap.width, tileMap.height, tileMap.partitionSizeX, tileMap.partitionSizeY);
                }
                int num1 = 0;
                foreach (Layer layer in layerArray)
                {
                    if (!layer.IsEmpty)
                        ++num1;
                }
                int num2 = 0;
                foreach (Layer layer in tileMap.Layers)
                {
                    if (!layer.IsEmpty)
                        ++num2;
                }
                if (num1 != num2)
                    flag1 = true;
                tileMap.Layers = layerArray;
            }
            if (tileMap.ColorChannel == null)
                tileMap.ColorChannel = new ColorChannel(tileMap.width, tileMap.height, tileMap.partitionSizeX, tileMap.partitionSizeY);
            return flag1;
        }

        private static GameObject GetExistingTilePrefabInstance(
            tk2dTileMap tileMap,
            int tileX,
            int tileY,
            int tileLayer)
        {
            int prefabsListCount = tileMap.GetTilePrefabsListCount();
            for (int index = 0; index < prefabsListCount; ++index)
            {
                int x;
                int y;
                int layer;
                GameObject instance;
                tileMap.GetTilePrefabsListItem(index, out x, out y, out layer, out instance);
                if (x == tileX && y == tileY && layer == tileLayer)
                    return instance;
            }
            return (GameObject) null;
        }

        public static void SpawnAnimatedTiles(tk2dTileMap tileMap, bool forceBuild)
        {
            int length = tileMap.Layers.Length;
            for (int layer1 = 0; layer1 < length; ++layer1)
            {
                Layer layer2 = tileMap.Layers[layer1];
                LayerInfo layer3 = tileMap.data.Layers[layer1];
                if (!layer2.IsEmpty && !layer3.skipMeshGeneration)
                {
                    for (int y = 0; y < layer2.numRows; ++y)
                    {
                        int baseY = y * layer2.divY;
                        for (int x = 0; x < layer2.numColumns; ++x)
                        {
                            int baseX = x * layer2.divX;
                            SpriteChunk chunk = layer2.GetChunk(x, y);
                            if (!chunk.IsEmpty && (forceBuild || chunk.Dirty))
                                BuilderUtil.SpawnAnimatedTilesForChunk(tileMap, chunk, baseX, baseY, layer1);
                        }
                    }
                }
            }
        }

        public static void SpawnAnimatedTilesForChunk(
            tk2dTileMap tileMap,
            SpriteChunk chunk,
            int baseX,
            int baseY,
            int layer)
        {
            LayerInfo layer1 = tileMap.data.Layers[layer];
            if (layer1.ForceNonAnimating)
                return;
            int[] spriteIds = chunk.spriteIds;
            float x1 = 0.0f;
            float y1 = 0.0f;
            tileMap.data.GetTileOffset(out x1, out y1);
            List<Material> materialList = new List<Material>();
            for (int index1 = 0; index1 < tileMap.partitionSizeY; ++index1)
            {
                for (int index2 = 0; index2 < tileMap.partitionSizeX; ++index2)
                {
                    int tileFromRawTile = BuilderUtil.GetTileFromRawTile(spriteIds[index1 * tileMap.partitionSizeX + index2]);
                    if (tileFromRawTile >= 0)
                    {
                        if (tileFromRawTile >= tileMap.SpriteCollectionInst.spriteDefinitions.Length)
                        {
                            Debug.Log((object) (tileFromRawTile.ToString() + " tile is oob"));
                        }
                        else
                        {
                            tk2dSpriteDefinition spriteDefinition = tileMap.SpriteCollectionInst.spriteDefinitions[tileFromRawTile];
                            if (spriteDefinition.metadata.usesAnimSequence && !materialList.Contains(spriteDefinition.materialInst))
                                materialList.Add(spriteDefinition.materialInst);
                        }
                    }
                }
            }
            while (materialList.Count > 0)
            {
                BuilderUtil.ProcGenMeshData genMeshData = (BuilderUtil.ProcGenMeshData) null;
                Material material = materialList[0];
                materialList.RemoveAt(0);
                List<TilemapAnimatorTileManager> tiles = new List<TilemapAnimatorTileManager>();
                bool flag1 = false;
                int unityLayer = layer1.unityLayer;
                for (int y2 = 0; y2 < tileMap.partitionSizeY; ++y2)
                {
                    for (int x2 = 0; x2 < tileMap.partitionSizeX; ++x2)
                    {
                        IntVector2 intVector2 = new IntVector2(baseX + x2, baseY + y2);
                        int tileFromRawTile = BuilderUtil.GetTileFromRawTile(spriteIds[y2 * tileMap.partitionSizeX + x2]);
                        if (tileFromRawTile >= 0)
                        {
                            if (tileFromRawTile >= tileMap.SpriteCollectionInst.spriteDefinitions.Length)
                            {
                                Debug.Log((object) (tileFromRawTile.ToString() + " tile is oob"));
                            }
                            else
                            {
                                tk2dSpriteDefinition spriteDefinition = tileMap.SpriteCollectionInst.spriteDefinitions[tileFromRawTile];
                                if (!((Object) spriteDefinition.materialInst != (Object) material))
                                {
                                    if (spriteDefinition.metadata.usesAnimSequence)
                                    {
                                        if (genMeshData == null)
                                            genMeshData = new BuilderUtil.ProcGenMeshData();
                                        TilemapAnimatorTileManager animatorTileManager1 = new TilemapAnimatorTileManager(tileMap.SpriteCollectionInst, tileFromRawTile, spriteDefinition.metadata, genMeshData.vertices.Count, spriteDefinition.uvs.Length, tileMap);
                                        animatorTileManager1.worldPosition = intVector2;
                                        if (TK2DTilemapChunkAnimator.PositionToAnimatorMap.ContainsKey(intVector2))
                                            TK2DTilemapChunkAnimator.PositionToAnimatorMap[intVector2].Add(animatorTileManager1);
                                        else
                                            TK2DTilemapChunkAnimator.PositionToAnimatorMap.Add(intVector2, new List<TilemapAnimatorTileManager>()
                                            {
                                                animatorTileManager1
                                            });
                                        bool flag2 = false;
                                        for (int index3 = 0; index3 < tiles.Count; ++index3)
                                        {
                                            TilemapAnimatorTileManager animatorTileManager2 = tiles[index3];
                                            List<IndexNeighborDependency> dependencies = animatorTileManager2.associatedCollection.GetDependencies(animatorTileManager2.associatedSpriteId);
                                            if (dependencies != null && dependencies.Count > 0)
                                            {
                                                for (int index4 = 0; index4 < dependencies.Count; ++index4)
                                                {
                                                    if (animatorTileManager2.worldPosition + DungeonData.GetIntVector2FromDirection(dependencies[index4].neighborDirection) == intVector2)
                                                    {
                                                        flag2 = true;
                                                        animatorTileManager2.linkedManagers.Add(animatorTileManager1);
                                                        goto label_35;
                                                    }
                                                }
                                            }
                                        }
    label_35:
                                        if (!flag2)
                                            tiles.Add(animatorTileManager1);
                                        if (BuilderUtil.AddSquareToAnimChunk(tileMap, chunk, spriteDefinition, baseX, baseY, x1, x2, y2, layer, genMeshData))
                                            flag1 = true;
                                    }
                                    if (spriteDefinition.metadata.usesPerTileVFX && (double) Random.value < (double) spriteDefinition.metadata.tileVFXChance)
                                        TileVFXManager.Instance.RegisterCellVFX(intVector2, spriteDefinition.metadata);
                                }
                            }
                        }
                    }
                }
                if (layer1.unityLayer == 19 || layer1.unityLayer == 20)
                    flag1 = false;
                if (genMeshData != null)
                {
                    GameObject gameObject = new GameObject("anim chunk data")
                    {
                        transform = {
                            parent = tileMap.Layers[layer].gameObject.transform
                        },
                        layer = !flag1 ? unityLayer : LayerMask.NameToLayer("ShadowCaster")
                    };
                    gameObject.transform.localPosition = new Vector3((float) baseX, (float) baseY, (float) baseY);
                    MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
                    MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
                    Mesh refMesh = new Mesh();
                    refMesh.vertices = genMeshData.vertices.ToArray();
                    refMesh.triangles = genMeshData.triangles.ToArray();
                    refMesh.uv = genMeshData.uvs.ToArray();
                    refMesh.colors = genMeshData.colors.ToArray();
                    refMesh.RecalculateBounds();
                    refMesh.RecalculateNormals();
                    meshFilter.mesh = refMesh;
                    meshRenderer.material = material;
                    for (int index = 0; index < meshRenderer.materials.Length; ++index)
                        meshRenderer.materials[index].renderQueue += layer1.renderQueueOffset;
                    gameObject.AddComponent<TK2DTilemapChunkAnimator>().Initialize(tiles, refMesh, tileMap);
                }
            }
        }

        private static bool AddSquareToAnimChunk(
            tk2dTileMap tileMap,
            SpriteChunk chunk,
            tk2dSpriteDefinition sprite,
            int baseX,
            int baseY,
            float xOffsetMult,
            int x,
            int y,
            int layer,
            BuilderUtil.ProcGenMeshData genMeshData)
        {
            bool animChunk = false;
            LayerInfo layer1 = tileMap.data.Layers[layer];
            int count = genMeshData.vertices.Count;
            float num = (float) (baseY + y & 1) * xOffsetMult;
            Vector3[] vector3Array = sprite.ConstructExpensivePositions();
            for (int index = 0; index < vector3Array.Length; ++index)
            {
                Vector3 vector = new Vector3(tileMap.data.tileSize.x * ((float) x + num), tileMap.data.tileSize.y * (float) y, 0.0f);
                Vector3 vector3_1 = BuilderUtil.ApplySpriteVertexTileFlags(tileMap, sprite, vector3Array[index], false, false, false);
                Vector3 vector3_2 = vector;
                IntVector2 intVector2 = vector.IntXY() + new IntVector2(baseX, baseY);
                CellData cellData = !GameManager.Instance.Dungeon.data.CheckInBounds(intVector2, 1) ? (CellData) null : GameManager.Instance.Dungeon.data[intVector2];
                if (cellData != null && cellData.IsAnyFaceWall())
                {
                    if (GameManager.Instance.Dungeon.data.isFaceWallHigher(cellData.position.x, cellData.position.y))
                    {
                        if (index > 1)
                            genMeshData.colors.Add(new Color(0.0f, 1f, 1f));
                        else
                            genMeshData.colors.Add(new Color(0.0f, 0.5f, 1f));
                    }
                    else if (index > 1)
                        genMeshData.colors.Add(new Color(0.0f, 0.5f, 1f));
                    else
                        genMeshData.colors.Add(new Color(0.0f, 0.0f, 1f));
                    animChunk = true;
                    BraveUtility.DrawDebugSquare(intVector2.ToVector2(), Color.blue, 1000f);
                }
                else
                    genMeshData.colors.Add(Color.black);
                Vector3 vector3_3;
                if (tileMap.isGungeonTilemap)
                {
                    if (cellData != null && GameManager.Instance.Dungeon.data.isAnyFaceWall(intVector2.x, intVector2.y))
                    {
                        Vector3 vector3_4 = !GameManager.Instance.Dungeon.data.isFaceWallHigher(intVector2.x, intVector2.y) ? new Vector3(0.0f, 0.0f, 1f) : new Vector3(0.0f, 0.0f, -1f);
                        if (cellData.diagonalWallType == DiagonalWallType.NORTHEAST)
                            vector3_4.z += (float) ((1.0 - (double) vector3_1.x) * 2.0);
                        else if (cellData.diagonalWallType == DiagonalWallType.NORTHWEST)
                            vector3_4.z += vector3_1.x * 2f;
                        vector3_3 = vector3_2 + (new Vector3(0.0f, 0.0f, vector.y - vector3_1.y) + vector3_1 + vector3_4);
                    }
                    else if (cellData != null && GameManager.Instance.Dungeon.data.isTopDiagonalWall(intVector2.x, intVector2.y) && layer1.name == "Collision Layer")
                    {
                        Vector3 vector3_5 = new Vector3(0.0f, 0.0f, -3f);
                        vector3_3 = vector3_2 + (new Vector3(0.0f, 0.0f, vector.y + vector3_1.y) + vector3_1 + vector3_5);
                    }
                    else if (layer1.name == "Pit Layer")
                    {
                        Vector3 vector3_6 = new Vector3(0.0f, 0.0f, 2f);
                        if (GameManager.Instance.Dungeon.data.CheckInBounds(intVector2.x, intVector2.y + 2))
                        {
                            if (GameManager.Instance.Dungeon.data.cellData[intVector2.x][intVector2.y + 1].type != CellType.PIT || GameManager.Instance.Dungeon.data.cellData[intVector2.x][intVector2.y + 2].type != CellType.PIT)
                            {
                                bool flag = GameManager.Instance.Dungeon.data.cellData[intVector2.x][intVector2.y + 1].type != CellType.PIT;
                                if (GameManager.Instance.Dungeon.debugSettings.WALLS_ARE_PITS && GameManager.Instance.Dungeon.data.cellData[intVector2.x][intVector2.y + 1].isExitCell)
                                    flag = false;
                                if (flag)
                                    vector3_6 = new Vector3(0.0f, 0.0f, 0.0f);
                                vector3_3 = vector3_2 + (new Vector3(0.0f, 0.0f, vector.y - vector3_1.y) + vector3_1 + vector3_6);
                            }
                            else
                                vector3_3 = vector3_2 + (new Vector3(0.0f, 0.0f, (float) ((double) vector.y + (double) vector3_1.y + 1.0)) + vector3_1);
                        }
                        else
                            vector3_3 = vector3_2 + (new Vector3(0.0f, 0.0f, (float) ((double) vector.y + (double) vector3_1.y + 1.0)) + vector3_1);
                    }
                    else
                        vector3_3 = vector3_2 + (new Vector3(0.0f, 0.0f, vector.y + vector3_1.y) + vector3_1);
                }
                else
                    vector3_3 = vector3_2 + vector3_1;
                genMeshData.vertices.Add(vector3_3);
                genMeshData.uvs.Add(sprite.uvs[index]);
            }
            bool flag1 = false;
            for (int index1 = 0; index1 < sprite.indices.Length; ++index1)
            {
                int index2 = !flag1 ? index1 : sprite.indices.Length - 1 - index1;
                genMeshData.triangles.Add(count + sprite.indices[index2]);
            }
            return animChunk;
        }

        public static void SpawnPrefabsForChunk(
            tk2dTileMap tileMap,
            SpriteChunk chunk,
            int baseX,
            int baseY,
            int layer,
            int[] prefabCounts)
        {
            int[] spriteIds = chunk.spriteIds;
            GameObject[] tilePrefabs = tileMap.data.tilePrefabs;
            Vector3 tileSize = tileMap.data.tileSize;
            Transform transform = chunk.gameObject.transform;
            float x = 0.0f;
            float y = 0.0f;
            tileMap.data.GetTileOffset(out x, out y);
            for (int index1 = 0; index1 < tileMap.partitionSizeY; ++index1)
            {
                float num = (float) (baseY + index1 & 1) * x;
                for (int index2 = 0; index2 < tileMap.partitionSizeX; ++index2)
                {
                    int tileFromRawTile = BuilderUtil.GetTileFromRawTile(spriteIds[index1 * tileMap.partitionSizeX + index2]);
                    if (tileFromRawTile >= 0 && tileFromRawTile < tilePrefabs.Length)
                    {
                        Object original = (Object) tilePrefabs[tileFromRawTile];
                        if (original != (Object) null)
                        {
                            ++prefabCounts[tileFromRawTile];
                            GameObject gameObject1 = BuilderUtil.GetExistingTilePrefabInstance(tileMap, baseX + index2, baseY + index1, layer);
                            bool flag1 = (Object) gameObject1 != (Object) null;
                            if ((Object) gameObject1 == (Object) null)
                                gameObject1 = Object.Instantiate(original, Vector3.zero, Quaternion.identity) as GameObject;
                            if ((Object) gameObject1 != (Object) null)
                            {
                                GameObject gameObject2 = original as GameObject;
                                Vector3 vector3 = new Vector3(tileSize.x * ((float) index2 + num), tileSize.y * (float) index1, 0.0f);
                                bool flag2 = false;
                                TileInfo tileInfoForSprite = tileMap.data.GetTileInfoForSprite(tileFromRawTile);
                                if (tileInfoForSprite != null)
                                    flag2 = tileInfoForSprite.enablePrefabOffset;
                                if (flag2 && (Object) gameObject2 != (Object) null)
                                    vector3 += gameObject2.transform.position;
                                if (!flag1)
                                    gameObject1.name = $"{original.name} {prefabCounts[tileFromRawTile].ToString()}";
                                tk2dUtil.SetTransformParent(gameObject1.transform, transform);
                                gameObject1.transform.localPosition = vector3;
                                BuilderUtil.TilePrefabsX.Add(baseX + index2);
                                BuilderUtil.TilePrefabsY.Add(baseY + index1);
                                BuilderUtil.TilePrefabsLayer.Add(layer);
                                BuilderUtil.TilePrefabsInstance.Add(gameObject1);
                            }
                        }
                    }
                }
            }
        }

        public static void SpawnPrefabs(tk2dTileMap tileMap, bool forceBuild)
        {
            BuilderUtil.TilePrefabsX = new List<int>();
            BuilderUtil.TilePrefabsY = new List<int>();
            BuilderUtil.TilePrefabsLayer = new List<int>();
            BuilderUtil.TilePrefabsInstance = new List<GameObject>();
            int[] prefabCounts = new int[tileMap.data.tilePrefabs.Length];
            int length = tileMap.Layers.Length;
            for (int layer1 = 0; layer1 < length; ++layer1)
            {
                Layer layer2 = tileMap.Layers[layer1];
                LayerInfo layer3 = tileMap.data.Layers[layer1];
                if (!layer2.IsEmpty && !layer3.skipMeshGeneration)
                {
                    for (int y = 0; y < layer2.numRows; ++y)
                    {
                        int baseY = y * layer2.divY;
                        for (int x = 0; x < layer2.numColumns; ++x)
                        {
                            int baseX = x * layer2.divX;
                            SpriteChunk chunk = layer2.GetChunk(x, y);
                            if (!chunk.IsEmpty && (forceBuild || chunk.Dirty))
                                BuilderUtil.SpawnPrefabsForChunk(tileMap, chunk, baseX, baseY, layer1, prefabCounts);
                        }
                    }
                }
            }
            tileMap.SetTilePrefabsList(BuilderUtil.TilePrefabsX, BuilderUtil.TilePrefabsY, BuilderUtil.TilePrefabsLayer, BuilderUtil.TilePrefabsInstance);
        }

        public static void HideTileMapPrefabs(tk2dTileMap tileMap)
        {
            if ((Object) tileMap.renderData == (Object) null || tileMap.Layers == null)
                return;
            if ((Object) tileMap.PrefabsRoot == (Object) null)
            {
                GameObject gameObject1 = tk2dUtil.CreateGameObject("Prefabs");
                tileMap.PrefabsRoot = gameObject1;
                GameObject gameObject2 = gameObject1;
                gameObject2.transform.parent = tileMap.renderData.transform;
                gameObject2.transform.localPosition = Vector3.zero;
                gameObject2.transform.localRotation = Quaternion.identity;
                gameObject2.transform.localScale = Vector3.one;
            }
            int prefabsListCount = tileMap.GetTilePrefabsListCount();
            bool[] flagArray = new bool[prefabsListCount];
            for (int index1 = 0; index1 < tileMap.Layers.Length; ++index1)
            {
                Layer layer = tileMap.Layers[index1];
                for (int index2 = 0; index2 < layer.spriteChannel.chunks.Length; ++index2)
                {
                    SpriteChunk chunk = layer.spriteChannel.chunks[index2];
                    if (!((Object) chunk.gameObject == (Object) null))
                    {
                        Transform transform = chunk.gameObject.transform;
                        int childCount = transform.childCount;
                        for (int index3 = 0; index3 < childCount; ++index3)
                        {
                            GameObject gameObject = transform.GetChild(index3).gameObject;
                            for (int index4 = 0; index4 < prefabsListCount; ++index4)
                            {
                                GameObject instance;
                                tileMap.GetTilePrefabsListItem(index4, out int _, out int _, out int _, out instance);
                                if ((Object) instance == (Object) gameObject)
                                {
                                    flagArray[index4] = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            Object[] tilePrefabs = (Object[]) tileMap.data.tilePrefabs;
            List<int> xs = new List<int>();
            List<int> ys = new List<int>();
            List<int> layers = new List<int>();
            List<GameObject> instances = new List<GameObject>();
            for (int index5 = 0; index5 < prefabsListCount; ++index5)
            {
                int x;
                int y;
                int layer;
                GameObject instance;
                tileMap.GetTilePrefabsListItem(index5, out x, out y, out layer, out instance);
                if (!flagArray[index5])
                {
                    int index6 = x < 0 || x >= tileMap.width || y < 0 || y >= tileMap.height ? -1 : tileMap.GetTile(x, y, layer);
                    if (index6 >= 0 && index6 < tilePrefabs.Length && tilePrefabs[index6] != (Object) null)
                        flagArray[index5] = true;
                }
                if (flagArray[index5])
                {
                    xs.Add(x);
                    ys.Add(y);
                    layers.Add(layer);
                    instances.Add(instance);
                    tk2dUtil.SetTransformParent(instance.transform, tileMap.PrefabsRoot.transform);
                }
            }
            tileMap.SetTilePrefabsList(xs, ys, layers, instances);
        }

        private static Vector3 GetTilePosition(tk2dTileMap tileMap, int x, int y)
        {
            return new Vector3(tileMap.data.tileSize.x * (float) x, tileMap.data.tileSize.y * (float) y, 0.0f);
        }

        public static void CreateOverrideChunkData(
            SpriteChunk chunk,
            tk2dTileMap tileMap,
            int layerId,
            string overrideChunkName)
        {
            Layer layer = tileMap.Layers[layerId];
            bool flag = layer.IsEmpty || chunk.IsEmpty;
            if (flag && chunk.HasGameData)
                chunk.DestroyGameData(tileMap);
            else if (!flag && (Object) chunk.gameObject == (Object) null)
            {
                string name = $"Chunk_{overrideChunkName}_{tileMap.data.Layers[layerId].name}";
                GameObject go = chunk.gameObject = tk2dUtil.CreateGameObject(name);
                go.transform.parent = layer.gameObject.transform;
                MeshFilter meshFilter = tk2dUtil.AddComponent<MeshFilter>(go);
                tk2dUtil.AddComponent<MeshRenderer>(go);
                chunk.mesh = tk2dUtil.CreateMesh();
                meshFilter.mesh = chunk.mesh;
            }
            if ((Object) chunk.gameObject != (Object) null)
            {
                Vector3 vector3 = new Vector3((float) chunk.startX, (float) chunk.startY, 0.0f);
                chunk.gameObject.transform.localPosition = vector3;
                chunk.gameObject.transform.localRotation = Quaternion.identity;
                chunk.gameObject.transform.localScale = Vector3.one;
                chunk.gameObject.layer = tileMap.data.Layers[layerId].unityLayer;
            }
            if (!((Object) chunk.gameObject != (Object) null) || chunk.roomReference == null)
                return;
            chunk.gameObject.transform.parent = chunk.roomReference.hierarchyParent;
        }

        public static void CreateRenderData(
            tk2dTileMap tileMap,
            bool editMode,
            Dictionary<Layer, bool> layersActive)
        {
            if ((Object) tileMap.renderData == (Object) null)
            {
                GameObject gameObject = GameObject.Find(tileMap.name + " Render Data");
                tileMap.renderData = !((Object) gameObject != (Object) null) ? tk2dUtil.CreateGameObject(tileMap.name + " Render Data") : gameObject;
            }
            tileMap.renderData.transform.position = tileMap.transform.position;
            float num1 = 0.0f;
            int index = 0;
            foreach (Layer layer in tileMap.Layers)
            {
                float z = tileMap.data.Layers[index].z;
                if (index != 0)
                    num1 -= z;
                if (layer.IsEmpty && (Object) layer.gameObject != (Object) null)
                {
                    tk2dUtil.DestroyImmediate((Object) layer.gameObject);
                    layer.gameObject = (GameObject) null;
                }
                else if (!layer.IsEmpty && (Object) layer.gameObject == (Object) null)
                {
                    Transform transform = tileMap.renderData.transform.Find(tileMap.data.Layers[index].name);
                    if ((Object) transform != (Object) null)
                        layer.gameObject = transform.gameObject;
                    else
                        (layer.gameObject = tk2dUtil.CreateGameObject(string.Empty)).transform.parent = tileMap.renderData.transform;
                }
                int unityLayer = tileMap.data.Layers[index].unityLayer;
                if ((Object) layer.gameObject != (Object) null)
                {
                    if (!editMode && layersActive.ContainsKey(layer) && layer.gameObject.activeSelf != layersActive[layer])
                        layer.gameObject.SetActive(layersActive[layer]);
                    layer.gameObject.name = tileMap.data.Layers[index].name;
                    layer.gameObject.transform.localPosition = new Vector3(0.0f, 0.0f, !tileMap.data.layersFixedZ ? num1 : -z);
                    layer.gameObject.transform.localRotation = Quaternion.identity;
                    layer.gameObject.transform.localScale = Vector3.one;
                    layer.gameObject.layer = unityLayer;
                }
                int x0;
                int x1;
                int dx;
                int y0;
                int y1;
                int dy;
                BuilderUtil.GetLoopOrder(tileMap.data.sortMethod, layer.numColumns, layer.numRows, out x0, out x1, out dx, out y0, out y1, out dy);
                float num2 = 0.0f;
                for (int y = y0; y != y1; y += dy)
                {
                    for (int x = x0; x != x1; x += dx)
                    {
                        SpriteChunk chunk = layer.GetChunk(x, y);
                        bool flag = layer.IsEmpty || chunk.IsEmpty;
                        if (editMode)
                            flag = false;
                        if (flag && chunk.HasGameData)
                            chunk.DestroyGameData(tileMap);
                        else if (!flag && (Object) chunk.gameObject == (Object) null)
                        {
                            string name = $"Chunk {y.ToString()} {x.ToString()}";
                            GameObject go = chunk.gameObject = tk2dUtil.CreateGameObject(name);
                            go.transform.parent = layer.gameObject.transform;
                            MeshFilter meshFilter = tk2dUtil.AddComponent<MeshFilter>(go);
                            tk2dUtil.AddComponent<MeshRenderer>(go);
                            chunk.mesh = tk2dUtil.CreateMesh();
                            meshFilter.mesh = chunk.mesh;
                        }
                        if ((Object) chunk.gameObject != (Object) null)
                        {
                            Vector3 tilePosition = BuilderUtil.GetTilePosition(tileMap, x * tileMap.partitionSizeX, y * tileMap.partitionSizeY);
                            tilePosition.z += num2;
                            chunk.gameObject.transform.localPosition = tilePosition;
                            chunk.gameObject.transform.localRotation = Quaternion.identity;
                            chunk.gameObject.transform.localScale = Vector3.one;
                            chunk.gameObject.layer = unityLayer;
                            if (editMode)
                                chunk.DestroyColliderData(tileMap);
                        }
                        num2 -= 1E-06f;
                    }
                }
                ++index;
            }
        }

        public static void GetLoopOrder(
            tk2dTileMapData.SortMethod sortMethod,
            int w,
            int h,
            out int x0,
            out int x1,
            out int dx,
            out int y0,
            out int y1,
            out int dy)
        {
            switch (sortMethod)
            {
                case tk2dTileMapData.SortMethod.BottomLeft:
                    x0 = 0;
                    x1 = w;
                    dx = 1;
                    y0 = 0;
                    y1 = h;
                    dy = 1;
                    break;
                case tk2dTileMapData.SortMethod.TopLeft:
                    x0 = 0;
                    x1 = w;
                    dx = 1;
                    y0 = h - 1;
                    y1 = -1;
                    dy = -1;
                    break;
                case tk2dTileMapData.SortMethod.BottomRight:
                    x0 = w - 1;
                    x1 = -1;
                    dx = -1;
                    y0 = 0;
                    y1 = h;
                    dy = 1;
                    break;
                case tk2dTileMapData.SortMethod.TopRight:
                    x0 = w - 1;
                    x1 = -1;
                    dx = -1;
                    y0 = h - 1;
                    y1 = -1;
                    dy = -1;
                    break;
                default:
                    Debug.LogError((object) "Unhandled sort method");
                    goto case tk2dTileMapData.SortMethod.BottomLeft;
            }
        }

        public static int GetTileFromRawTile(int rawTile)
        {
            return rawTile == -1 ? -1 : rawTile & 16777215;
        }

        public static bool IsRawTileFlagSet(int rawTile, tk2dTileFlags flag)
        {
            return rawTile != -1 && ((tk2dTileFlags) rawTile & flag) != tk2dTileFlags.None;
        }

        public static void SetRawTileFlag(ref int rawTile, tk2dTileFlags flag, bool setValue)
        {
            if (rawTile == -1)
                return;
            rawTile = !setValue ? (int) ((tk2dTileFlags) rawTile & ~flag) : (int) ((tk2dTileFlags) rawTile | flag);
        }

        public static void InvertRawTileFlag(ref int rawTile, tk2dTileFlags flag)
        {
            if (rawTile == -1)
                return;
            bool flag1 = ((tk2dTileFlags) rawTile & flag) == tk2dTileFlags.None;
            rawTile = !flag1 ? (int) ((tk2dTileFlags) rawTile & ~flag) : (int) ((tk2dTileFlags) rawTile | flag);
        }

        public static Vector3 ApplySpriteVertexTileFlags(
            tk2dTileMap tileMap,
            tk2dSpriteDefinition spriteDef,
            Vector3 pos,
            bool flipH,
            bool flipV,
            bool rot90)
        {
            float num1 = tileMap.data.tileOrigin.x + 0.5f * tileMap.data.tileSize.x;
            float num2 = tileMap.data.tileOrigin.y + 0.5f * tileMap.data.tileSize.y;
            float num3 = pos.x - num1;
            float num4 = pos.y - num2;
            if (rot90)
            {
                float num5 = num3;
                num3 = num4;
                num4 = -num5;
            }
            if (flipH)
                num3 *= -1f;
            if (flipV)
                num4 *= -1f;
            pos.x = num1 + num3;
            pos.y = num2 + num4;
            return pos;
        }

        public static Vector2 ApplySpriteVertexTileFlags(
            tk2dTileMap tileMap,
            tk2dSpriteDefinition spriteDef,
            Vector2 pos,
            bool flipH,
            bool flipV,
            bool rot90)
        {
            float num1 = tileMap.data.tileOrigin.x + 0.5f * tileMap.data.tileSize.x;
            float num2 = tileMap.data.tileOrigin.y + 0.5f * tileMap.data.tileSize.y;
            float num3 = pos.x - num1;
            float num4 = pos.y - num2;
            if (rot90)
            {
                float num5 = num3;
                num3 = num4;
                num4 = -num5;
            }
            if (flipH)
                num3 *= -1f;
            if (flipV)
                num4 *= -1f;
            pos.x = num1 + num3;
            pos.y = num2 + num4;
            return pos;
        }

        internal class ProcGenMeshData
        {
            public List<Vector3> vertices = new List<Vector3>();
            public List<int> triangles = new List<int>();
            public List<Vector2> uvs = new List<Vector2>();
            public List<Color> colors = new List<Color>();
        }
    }
}
