using System;

using UnityEngine;

#nullable disable
namespace tk2dRuntime.TileMap
{
    [Serializable]
    public class LayerInfo
    {
        public string name;
        public int hash;
        public bool useColor;
        public bool generateCollider;
        public float z = 0.1f;
        public int unityLayer;
        public int renderQueueOffset;
        public string sortingLayerName = string.Empty;
        public int sortingOrder;
        [NonSerialized]
        public bool ForceNonAnimating;
        public bool overrideChunkable;
        public int overrideChunkXOffset;
        public int overrideChunkYOffset;
        [NonSerialized]
        public bool[] preprocessedFlags;
        public bool skipMeshGeneration;
        public PhysicMaterial physicMaterial;
        public PhysicsMaterial2D physicsMaterial2D;

        public LayerInfo()
        {
            this.unityLayer = 0;
            this.useColor = true;
            this.generateCollider = true;
            this.skipMeshGeneration = false;
        }
    }
}
