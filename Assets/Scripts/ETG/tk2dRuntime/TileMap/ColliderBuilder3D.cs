using System;
using System.Collections.Generic;

using UnityEngine;

#nullable disable
namespace tk2dRuntime.TileMap
{
    public static class ColliderBuilder3D
    {
        public static void Build(tk2dTileMap tileMap, bool forceBuild)
        {
            bool flag = !forceBuild;
            int length = tileMap.Layers.Length;
            for (int index = 0; index < length; ++index)
            {
                Layer layer = tileMap.Layers[index];
                if (!layer.IsEmpty && tileMap.data.Layers[index].generateCollider)
                {
                    for (int y = 0; y < layer.numRows; ++y)
                    {
                        int baseY = y * layer.divY;
                        for (int x = 0; x < layer.numColumns; ++x)
                        {
                            int baseX = x * layer.divX;
                            SpriteChunk chunk = layer.GetChunk(x, y);
                            if ((!flag || chunk.Dirty) && !chunk.IsEmpty)
                            {
                                ColliderBuilder3D.BuildForChunk(tileMap, chunk, baseX, baseY);
                                PhysicMaterial physicMaterial = tileMap.data.Layers[index].physicMaterial;
                                if ((UnityEngine.Object) chunk.meshCollider != (UnityEngine.Object) null)
                                    chunk.meshCollider.sharedMaterial = physicMaterial;
                            }
                        }
                    }
                }
            }
        }

        public static void BuildForChunk(tk2dTileMap tileMap, SpriteChunk chunk, int baseX, int baseY)
        {
            Vector3[] vertices = new Vector3[0];
            int[] indices = new int[0];
            ColliderBuilder3D.BuildLocalMeshForChunk(tileMap, chunk, baseX, baseY, ref vertices, ref indices);
            if (indices.Length > 6)
            {
                vertices = ColliderBuilder3D.WeldVertices(vertices, ref indices);
                indices = ColliderBuilder3D.RemoveDuplicateFaces(indices);
            }
            foreach (EdgeCollider2D edgeCollider in chunk.edgeColliders)
            {
                if ((UnityEngine.Object) edgeCollider != (UnityEngine.Object) null)
                    tk2dUtil.DestroyImmediate((UnityEngine.Object) edgeCollider);
            }
            chunk.edgeColliders.Clear();
            if (vertices.Length > 0)
            {
                if ((UnityEngine.Object) chunk.colliderMesh != (UnityEngine.Object) null)
                {
                    tk2dUtil.DestroyImmediate((UnityEngine.Object) chunk.colliderMesh);
                    chunk.colliderMesh = (Mesh) null;
                }
                if ((UnityEngine.Object) chunk.meshCollider == (UnityEngine.Object) null)
                {
                    chunk.meshCollider = chunk.gameObject.GetComponent<MeshCollider>();
                    if ((UnityEngine.Object) chunk.meshCollider == (UnityEngine.Object) null)
                        chunk.meshCollider = tk2dUtil.AddComponent<MeshCollider>(chunk.gameObject);
                }
                chunk.colliderMesh = tk2dUtil.CreateMesh();
                chunk.colliderMesh.vertices = vertices;
                chunk.colliderMesh.triangles = indices;
                chunk.colliderMesh.RecalculateBounds();
                chunk.meshCollider.sharedMesh = chunk.colliderMesh;
            }
            else
                chunk.DestroyColliderData(tileMap);
        }

        private static void BuildLocalMeshForChunk(
            tk2dTileMap tileMap,
            SpriteChunk chunk,
            int baseX,
            int baseY,
            ref Vector3[] vertices,
            ref int[] indices)
        {
            List<Vector3> vector3List = new List<Vector3>();
            List<int> intList = new List<int>();
            int length = tileMap.SpriteCollectionInst.spriteDefinitions.Length;
            Vector3 tileSize = tileMap.data.tileSize;
            GameObject[] tilePrefabs = tileMap.data.tilePrefabs;
            float x = 0.0f;
            float y = 0.0f;
            tileMap.data.GetTileOffset(out x, out y);
            int[] spriteIds = chunk.spriteIds;
            for (int index1 = 0; index1 < chunk.Height; ++index1)
            {
                float num = (float) (baseY + index1 & 1) * x;
                for (int index2 = 0; index2 < chunk.Width; ++index2)
                {
                    int rawTile = spriteIds[index1 * chunk.Width + index2];
                    int tileFromRawTile = BuilderUtil.GetTileFromRawTile(rawTile);
                    Vector3 vector3_1 = new Vector3(tileSize.x * ((float) index2 + num), tileSize.y * (float) index1, 0.0f);
                    if (tileFromRawTile >= 0 && tileFromRawTile < length && !(bool) (UnityEngine.Object) tilePrefabs[tileFromRawTile])
                    {
                        bool flipH = BuilderUtil.IsRawTileFlagSet(rawTile, tk2dTileFlags.FlipX);
                        bool flipV = BuilderUtil.IsRawTileFlagSet(rawTile, tk2dTileFlags.FlipY);
                        bool rot90 = BuilderUtil.IsRawTileFlagSet(rawTile, tk2dTileFlags.Rot90);
                        bool flag = false;
                        if (flipH)
                            flag = !flag;
                        if (flipV)
                            flag = !flag;
                        tk2dSpriteDefinition spriteDefinition = tileMap.SpriteCollectionInst.spriteDefinitions[tileFromRawTile];
                        int count = vector3List.Count;
                        if (spriteDefinition.colliderType == tk2dSpriteDefinition.ColliderType.Box)
                        {
                            Vector3 colliderVertex1 = spriteDefinition.colliderVertices[0];
                            Vector3 colliderVertex2 = spriteDefinition.colliderVertices[1];
                            Vector3 vector3_2 = colliderVertex1 - colliderVertex2;
                            Vector3 vector3_3 = colliderVertex1 + colliderVertex2;
                            Vector3[] vector3Array = new Vector3[8]
                            {
                                new Vector3(vector3_2.x, vector3_2.y, vector3_2.z),
                                new Vector3(vector3_2.x, vector3_2.y, vector3_3.z),
                                new Vector3(vector3_3.x, vector3_2.y, vector3_2.z),
                                new Vector3(vector3_3.x, vector3_2.y, vector3_3.z),
                                new Vector3(vector3_2.x, vector3_3.y, vector3_2.z),
                                new Vector3(vector3_2.x, vector3_3.y, vector3_3.z),
                                new Vector3(vector3_3.x, vector3_3.y, vector3_2.z),
                                new Vector3(vector3_3.x, vector3_3.y, vector3_3.z)
                            };
                            for (int index3 = 0; index3 < 8; ++index3)
                            {
                                Vector3 vector3_4 = BuilderUtil.ApplySpriteVertexTileFlags(tileMap, spriteDefinition, vector3Array[index3], flipH, flipV, rot90);
                                vector3List.Add(vector3_4 + vector3_1);
                            }
                            int[] numArray = new int[24]
                            {
                                2,
                                1,
                                0,
                                3,
                                1,
                                2,
                                4,
                                5,
                                6,
                                6,
                                5,
                                7,
                                6,
                                7,
                                3,
                                6,
                                3,
                                2,
                                1,
                                5,
                                4,
                                0,
                                1,
                                4
                            };
                            for (int index4 = 0; index4 < numArray.Length; ++index4)
                            {
                                int index5 = !flag ? index4 : numArray.Length - 1 - index4;
                                intList.Add(count + numArray[index5]);
                            }
                        }
                        else if (spriteDefinition.colliderType != tk2dSpriteDefinition.ColliderType.Mesh)
                            ;
                    }
                }
            }
            vertices = vector3List.ToArray();
            indices = intList.ToArray();
        }

        private static int CompareWeldVertices(Vector3 a, Vector3 b)
        {
            float num = 0.01f;
            float f1 = a.x - b.x;
            if ((double) Mathf.Abs(f1) > (double) num)
                return (int) Mathf.Sign(f1);
            float f2 = a.y - b.y;
            if ((double) Mathf.Abs(f2) > (double) num)
                return (int) Mathf.Sign(f2);
            float f3 = a.z - b.z;
            return (double) Mathf.Abs(f3) > (double) num ? (int) Mathf.Sign(f3) : 0;
        }

        private static Vector3[] WeldVertices(Vector3[] vertices, ref int[] indices)
        {
            int[] array = new int[vertices.Length];
            for (int index = 0; index < vertices.Length; ++index)
                array[index] = index;
            Array.Sort<int>(array, (Comparison<int>) ((a, b) => ColliderBuilder3D.CompareWeldVertices(vertices[a], vertices[b])));
            List<Vector3> vector3List = new List<Vector3>();
            int[] numArray = new int[vertices.Length];
            Vector3 b1 = vertices[array[0]];
            vector3List.Add(b1);
            numArray[array[0]] = vector3List.Count - 1;
            for (int index = 1; index < array.Length; ++index)
            {
                Vector3 vertex = vertices[array[index]];
                if (ColliderBuilder3D.CompareWeldVertices(vertex, b1) != 0)
                {
                    b1 = vertex;
                    vector3List.Add(b1);
                    numArray[array[index]] = vector3List.Count - 1;
                }
                numArray[array[index]] = vector3List.Count - 1;
            }
            for (int index = 0; index < indices.Length; ++index)
                indices[index] = numArray[indices[index]];
            return vector3List.ToArray();
        }

        private static int CompareDuplicateFaces(int[] indices, int face0index, int face1index)
        {
            for (int index = 0; index < 3; ++index)
            {
                int num = indices[face0index + index] - indices[face1index + index];
                if (num != 0)
                    return num;
            }
            return 0;
        }

        private static int[] RemoveDuplicateFaces(int[] indices)
        {
            int[] sortedFaceIndices = new int[indices.Length];
            for (int index = 0; index < indices.Length; index += 3)
            {
                int[] array = new int[3]
                {
                    indices[index],
                    indices[index + 1],
                    indices[index + 2]
                };
                Array.Sort<int>(array);
                sortedFaceIndices[index] = array[0];
                sortedFaceIndices[index + 1] = array[1];
                sortedFaceIndices[index + 2] = array[2];
            }
            int[] array1 = new int[indices.Length / 3];
            for (int index = 0; index < indices.Length; index += 3)
                array1[index / 3] = index;
            Array.Sort<int>(array1, (Comparison<int>) ((a, b) => ColliderBuilder3D.CompareDuplicateFaces(sortedFaceIndices, a, b)));
            List<int> intList = new List<int>();
            for (int index1 = 0; index1 < array1.Length; ++index1)
            {
                if (index1 != array1.Length - 1 && ColliderBuilder3D.CompareDuplicateFaces(sortedFaceIndices, array1[index1], array1[index1 + 1]) == 0)
                {
                    ++index1;
                }
                else
                {
                    for (int index2 = 0; index2 < 3; ++index2)
                        intList.Add(indices[array1[index1] + index2]);
                }
            }
            return intList.ToArray();
        }
    }
}
