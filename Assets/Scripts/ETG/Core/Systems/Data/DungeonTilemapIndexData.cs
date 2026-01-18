using System;

using UnityEngine;

using Dungeonator;

#nullable disable

[Serializable]
public class DungeonTilemapIndexData : ScriptableObject
    {
        public tk2dSpriteCollectionData dungeonCollection;
        public AOTileIndices aoTileIndices;
    }

