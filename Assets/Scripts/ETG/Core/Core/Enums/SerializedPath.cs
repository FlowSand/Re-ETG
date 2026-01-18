using System;
using System.Collections.Generic;

using UnityEngine;

using Dungeonator;

#nullable disable

[Serializable]
public class SerializedPath
    {
        public List<SerializedPathNode> nodes;
        public SerializedPath.SerializedPathWrapMode wrapMode;
        public float overrideSpeed = -1f;
        public int tilesetPathGrid = -1;

        public SerializedPath(IntVector2 cellPosition)
        {
            this.nodes = new List<SerializedPathNode>();
            this.nodes.Add(new SerializedPathNode(cellPosition));
        }

        public SerializedPath(SerializedPath prototypePath, IntVector2 basePositionAdjustment)
        {
            this.nodes = new List<SerializedPathNode>();
            for (int index = 0; index < prototypePath.nodes.Count; ++index)
                this.nodes.Add(new SerializedPathNode(prototypePath.nodes[index], basePositionAdjustment));
            this.wrapMode = prototypePath.wrapMode;
        }

        public static SerializedPath CreateMirror(
            SerializedPath source,
            IntVector2 roomDimensions,
            PrototypeDungeonRoom sourceRoom)
        {
            SerializedPath mirror = new SerializedPath(IntVector2.Zero);
            mirror.nodes.Clear();
            for (int index = 0; index < source.nodes.Count; ++index)
                mirror.nodes.Add(SerializedPathNode.CreateMirror(source.nodes[index], roomDimensions));
            mirror.wrapMode = source.wrapMode;
            mirror.overrideSpeed = source.overrideSpeed;
            mirror.tilesetPathGrid = source.tilesetPathGrid;
            int num = sourceRoom.paths.IndexOf(source);
            int a = 0;
            for (int index = 0; index < sourceRoom.placedObjects.Count; ++index)
            {
                if (num >= 0 && sourceRoom.placedObjects[index].assignedPathIDx == num)
                    a = Mathf.Max(a, sourceRoom.placedObjects[index].GetWidth(true));
            }
            for (int index1 = 0; index1 < sourceRoom.additionalObjectLayers.Count; ++index1)
            {
                for (int index2 = 0; index2 < sourceRoom.additionalObjectLayers[index1].placedObjects.Count; ++index2)
                {
                    if (num >= 0 && sourceRoom.additionalObjectLayers[index1].placedObjects[index2].assignedPathIDx == num)
                        a = Mathf.Max(a, sourceRoom.additionalObjectLayers[index1].placedObjects[index2].GetWidth(true));
                }
            }
            if (a > 0)
            {
                for (int index = 0; index < mirror.nodes.Count; ++index)
                {
                    SerializedPathNode node = mirror.nodes[index];
                    node.position += new IntVector2(-1, 0) * (a - 1);
                    mirror.nodes[index] = node;
                }
            }
            return mirror;
        }

        public void StampPathToTilemap(RoomHandler parentRoom)
        {
            if (this.tilesetPathGrid < 0 || this.tilesetPathGrid >= GameManager.Instance.Dungeon.pathGridDefinitions.Count)
                return;
            DungeonData data = GameManager.Instance.Dungeon.data;
            for (int index1 = 1; index1 < this.nodes.Count + 1; ++index1)
            {
                SerializedPathNode node1;
                SerializedPathNode node2;
                if (index1 == this.nodes.Count)
                {
                    if (this.wrapMode != SerializedPath.SerializedPathWrapMode.Loop)
                        break;
                    node1 = this.nodes[index1 - 1];
                    node2 = this.nodes[0];
                }
                else
                {
                    node1 = this.nodes[index1 - 1];
                    node2 = this.nodes[index1];
                }
                if (node1.position.x != node2.position.x && node1.position.y != node2.position.y)
                {
                    Debug.LogError((object) "Attempting to stamp a path grid to the tilemap and the path contains diagonals! This cannot be.");
                    break;
                }
                IntVector2 key1 = parentRoom.area.basePosition + node1.position;
                IntVector2 key2 = parentRoom.area.basePosition + node2.position;
                if (node1.position.x == node2.position.x)
                {
                    TileIndexGrid pathGridDefinition = GameManager.Instance.Dungeon.pathGridDefinitions[this.tilesetPathGrid];
                    if ((UnityEngine.Object) pathGridDefinition.PathFacewallStamp != (UnityEngine.Object) null)
                    {
                        for (int y = Mathf.Min(key1.y, key2.y); y < Mathf.Max(key1.y, key2.y); ++y)
                        {
                            if (data[key1.x, y].type != CellType.WALL && data[key1.x, y + 1].type == CellType.WALL)
                            {
                                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(pathGridDefinition.PathFacewallStamp, new Vector3((float) key1.x, (float) (y + 1), 0.0f) + pathGridDefinition.PathFacewallStamp.transform.position, Quaternion.identity);
                                gameObject.GetComponent<PlacedWallDecorator>().ConfigureOnPlacement(data.GetAbsoluteRoomFromPosition(gameObject.transform.position.IntXY()));
                            }
                        }
                    }
                }
                else
                {
                    TileIndexGrid pathGridDefinition = GameManager.Instance.Dungeon.pathGridDefinitions[this.tilesetPathGrid];
                    if ((UnityEngine.Object) pathGridDefinition.PathSidewallStamp != (UnityEngine.Object) null)
                    {
                        for (int x = Mathf.Min(key1.x, key2.x); x < Mathf.Max(key1.x, key2.x); ++x)
                        {
                            if (data[x, key1.y].type == CellType.FLOOR && data[x + 1, key1.y].type == CellType.WALL)
                                UnityEngine.Object.Instantiate<GameObject>(pathGridDefinition.PathSidewallStamp, new Vector3((float) (x + 1), (float) key1.y, 0.0f) + pathGridDefinition.PathSidewallStamp.transform.position, Quaternion.identity).GetComponent<tk2dSprite>().FlipX = true;
                            else if (data[x, key1.y].type == CellType.WALL && data[x + 1, key1.y].type == CellType.FLOOR)
                                UnityEngine.Object.Instantiate<GameObject>(pathGridDefinition.PathSidewallStamp, new Vector3((float) (x + 1), (float) key1.y, 0.0f) + pathGridDefinition.PathSidewallStamp.transform.position, Quaternion.identity);
                        }
                    }
                }
                if ((index1 == this.nodes.Count - 1 || index1 == 1) && this.wrapMode != SerializedPath.SerializedPathWrapMode.Loop)
                {
                    TileIndexGrid pathGridDefinition = GameManager.Instance.Dungeon.pathGridDefinitions[this.tilesetPathGrid];
                    if (index1 == this.nodes.Count - 1)
                    {
                        if (key1.y == key2.y && data[key2].type != CellType.WALL && data[key2 + BraveUtility.GetIntMajorAxis((key2 - key1).ToVector2())].type != CellType.WALL)
                            key2 += BraveUtility.GetIntMajorAxis((key2 - key1).ToVector2());
                    }
                    else if (index1 == 1 && key1.y == key2.y && data[key1].type != CellType.WALL && data[key1 + BraveUtility.GetIntMajorAxis((key1 - key2).ToVector2())].type != CellType.WALL)
                        key1 += BraveUtility.GetIntMajorAxis((key1 - key2).ToVector2());
                    int num = 1;
                    if (this.nodes.Count - 1 == 1)
                        num = 2;
                    for (int index2 = 0; index2 < num; ++index2)
                    {
                        IntVector2 key3 = index1 != 1 || index2 == 1 ? key2 : key1;
                        IntVector2 vec = index1 != 1 || index2 == 1 ? BraveUtility.GetIntMajorAxis(key2 - key1) : BraveUtility.GetIntMajorAxis(key1 - key2);
                        if (index1 == 1 && index2 != 1)
                            key3 += vec;
                        if (data[key3] != null && data[key3].type == CellType.FLOOR)
                        {
                            switch (DungeonData.GetDirectionFromIntVector2(vec))
                            {
                                case DungeonData.Direction.NORTH:
                                    if ((UnityEngine.Object) pathGridDefinition.PathStubNorth != (UnityEngine.Object) null)
                                    {
                                        UnityEngine.Object.Instantiate<GameObject>(pathGridDefinition.PathStubNorth, key3.ToVector3(), Quaternion.identity);
                                        continue;
                                    }
                                    continue;
                                case DungeonData.Direction.EAST:
                                    if ((UnityEngine.Object) pathGridDefinition.PathStubEast != (UnityEngine.Object) null)
                                    {
                                        UnityEngine.Object.Instantiate<GameObject>(pathGridDefinition.PathStubEast, key3.ToVector3(), Quaternion.identity);
                                        continue;
                                    }
                                    continue;
                                case DungeonData.Direction.SOUTH:
                                    if ((UnityEngine.Object) pathGridDefinition.PathStubSouth != (UnityEngine.Object) null)
                                    {
                                        UnityEngine.Object.Instantiate<GameObject>(pathGridDefinition.PathStubSouth, key3.ToVector3(), Quaternion.identity);
                                        continue;
                                    }
                                    continue;
                                case DungeonData.Direction.WEST:
                                    if ((UnityEngine.Object) pathGridDefinition.PathStubWest != (UnityEngine.Object) null)
                                    {
                                        UnityEngine.Object.Instantiate<GameObject>(pathGridDefinition.PathStubWest, key3.ToVector3(), Quaternion.identity);
                                        continue;
                                    }
                                    continue;
                                default:
                                    continue;
                            }
                        }
                    }
                }
                IntVector2 majorAxis = (key2 - key1).MajorAxis;
                while (key1 != key2)
                {
                    data[key1].cellVisualData.containsObjectSpaceStamp = true;
                    data[key1].cellVisualData.pathTilesetGridIndex = this.tilesetPathGrid;
                    data[key1].cellVisualData.hasStampedPath = true;
                    BraveUtility.DrawDebugSquare(key1.ToVector2(), Color.magenta, 1000f);
                    data[key1].fallingPrevented = true;
                    key1 += majorAxis;
                }
            }
        }

        public void ChangeNodePlacement(IntVector2 position)
        {
            for (int index = 0; index < this.nodes.Count; ++index)
            {
                if (this.nodes[index].position == position)
                {
                    int num = (int) (this.nodes[index].placement + 1) % Enum.GetValues(typeof (SerializedPathNode.SerializedNodePlacement)).Length;
                    SerializedPathNode node = this.nodes[index] with
                    {
                        placement = (SerializedPathNode.SerializedNodePlacement) num
                    };
                    this.nodes[index] = node;
                }
            }
        }

        public void ChangeNodePlacement(
            IntVector2 position,
            SerializedPathNode.SerializedNodePlacement placement)
        {
            for (int index = 0; index < this.nodes.Count; ++index)
            {
                if (this.nodes[index].position == position)
                {
                    SerializedPathNode node = this.nodes[index] with
                    {
                        placement = placement
                    };
                    this.nodes[index] = node;
                }
            }
        }

        public SerializedPathNode? GetNodeAtPoint(IntVector2 position, out int index)
        {
            for (int index1 = 0; index1 < this.nodes.Count; ++index1)
            {
                if (this.nodes[index1].position == position)
                {
                    index = index1;
                    return new SerializedPathNode?(this.nodes[index1]);
                }
            }
            index = -1;
            return new SerializedPathNode?();
        }

        public void AddPosition(IntVector2 position) => this.nodes.Add(new SerializedPathNode(position));

        public void AddPosition(IntVector2 position, IntVector2 previousPosition)
        {
            bool flag = false;
            for (int index = 0; index < this.nodes.Count; ++index)
            {
                if (this.nodes[index].position == previousPosition)
                {
                    flag = true;
                    this.nodes.Insert(index + 1, new SerializedPathNode(position)
                    {
                        placement = this.nodes[index].placement
                    });
                    break;
                }
            }
            if (flag)
                return;
            this.AddPosition(position);
        }

        public bool TranslatePosition(IntVector2 position, IntVector2 translation)
        {
            IntVector2 pos = position + translation;
            int index1 = -1;
            int num = -1;
            for (int index2 = 0; index2 < this.nodes.Count; ++index2)
            {
                if (this.nodes[index2].position == position)
                    index1 = index2;
                if (this.nodes[index2].position == pos)
                    num = index2;
            }
            if (index1 == -1 || num != -1)
                return false;
            this.nodes[index1] = new SerializedPathNode(pos)
            {
                placement = this.nodes[index1].placement,
                delayTime = this.nodes[index1].delayTime
            };
            return true;
        }

        public void RemovePosition(IntVector2 position)
        {
            for (int index = 0; index < this.nodes.Count; ++index)
            {
                if (this.nodes[index].position == position)
                {
                    this.nodes.RemoveAt(index);
                    --index;
                }
            }
        }

        public enum SerializedPathWrapMode
        {
            PingPong,
            Loop,
            Once,
        }
    }

