using Dungeonator;
using System;
using UnityEngine;

#nullable disable

[Serializable]
public class DungeonTilemapIndexData : ScriptableObject
  {
    public tk2dSpriteCollectionData dungeonCollection;
    public AOTileIndices aoTileIndices;
  }

