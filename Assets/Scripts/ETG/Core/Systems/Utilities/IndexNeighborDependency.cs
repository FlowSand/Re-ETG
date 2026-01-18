using System;

using Dungeonator;

#nullable disable

[Serializable]
public class IndexNeighborDependency
    {
        public DungeonData.Direction neighborDirection;
        public int neighborIndex = -1;
    }

