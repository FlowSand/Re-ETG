using System;
using System.Collections.Generic;

using UnityEngine;

using Dungeonator;

#nullable disable
namespace tk2dRuntime.TileMap
{
    [Serializable]
    public class SpriteChunk
    {
        public static Dictionary<LayerInfo, List<SpriteChunk>> s_roomChunks;
        private bool dirty;
        public int startX;
        public int startY;
        public int endX;
        public int endY;
        public RoomHandler roomReference;
        public int[] spriteIds;
        public bool[] chunkPreprocessFlags;
        public GameObject gameObject;
        public Mesh mesh;
        public MeshCollider meshCollider;
        public Mesh colliderMesh;
        public List<EdgeCollider2D> edgeColliders = new List<EdgeCollider2D>();

        public SpriteChunk(int sX, int sY, int eX, int eY)
        {
            this.startX = sX;
            this.startY = sY;
            this.endX = eX;
            this.endY = eY;
            this.spriteIds = new int[0];
        }

        public static void ClearPerLevelData()
        {
            SpriteChunk.s_roomChunks = (Dictionary<LayerInfo, List<SpriteChunk>>) null;
        }

        public int Width => this.endX - this.startX;

        public int Height => this.endY - this.startY;

        public bool Dirty
        {
            get => this.dirty;
            set => this.dirty = value;
        }

        public bool IsEmpty => this.spriteIds.Length == 0;

        public bool IrrelevantToGameplay
        {
            get
            {
                float a = float.MaxValue;
                for (int startX = this.startX; startX < this.endX; ++startX)
                {
                    for (int startY = this.startY; startY < this.endY; ++startY)
                    {
                        IntVector2 intVector2 = new IntVector2(startX + RenderMeshBuilder.CurrentCellXOffset, startY + RenderMeshBuilder.CurrentCellYOffset);
                        if (GameManager.Instance.Dungeon.data.CheckInBoundsAndValid(intVector2))
                            a = Mathf.Min(a, GameManager.Instance.Dungeon.data[intVector2].distanceFromNearestRoom);
                    }
                }
                return (double) a > 15.0;
            }
        }

        public bool HasGameData
        {
            get
            {
                return (UnityEngine.Object) this.gameObject != (UnityEngine.Object) null || (UnityEngine.Object) this.mesh != (UnityEngine.Object) null || (UnityEngine.Object) this.meshCollider != (UnityEngine.Object) null || (UnityEngine.Object) this.colliderMesh != (UnityEngine.Object) null || this.edgeColliders.Count > 0;
            }
        }

        public void DestroyGameData(tk2dTileMap tileMap)
        {
            if ((UnityEngine.Object) this.mesh != (UnityEngine.Object) null)
                tileMap.DestroyMesh(this.mesh);
            if ((UnityEngine.Object) this.gameObject != (UnityEngine.Object) null)
                tk2dUtil.DestroyImmediate((UnityEngine.Object) this.gameObject);
            this.gameObject = (GameObject) null;
            this.mesh = (Mesh) null;
            this.DestroyColliderData(tileMap);
        }

        public void DestroyColliderData(tk2dTileMap tileMap)
        {
            if ((UnityEngine.Object) this.colliderMesh != (UnityEngine.Object) null)
                tileMap.DestroyMesh(this.colliderMesh);
            if ((UnityEngine.Object) this.meshCollider != (UnityEngine.Object) null && (UnityEngine.Object) this.meshCollider.sharedMesh != (UnityEngine.Object) null && (UnityEngine.Object) this.meshCollider.sharedMesh != (UnityEngine.Object) this.colliderMesh)
                tileMap.DestroyMesh(this.meshCollider.sharedMesh);
            if ((UnityEngine.Object) this.meshCollider != (UnityEngine.Object) null)
                tk2dUtil.DestroyImmediate((UnityEngine.Object) this.meshCollider);
            this.meshCollider = (MeshCollider) null;
            this.colliderMesh = (Mesh) null;
            if (this.edgeColliders.Count <= 0)
                return;
            for (int index = 0; index < this.edgeColliders.Count; ++index)
                tk2dUtil.DestroyImmediate((UnityEngine.Object) this.edgeColliders[index]);
            this.edgeColliders.Clear();
        }
    }
}
