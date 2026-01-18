using System.Collections.Generic;

#nullable disable

public class GungeonFlagsComparer : IEqualityComparer<GungeonFlags>
    {
        public bool Equals(GungeonFlags x, GungeonFlags y) => x == y;

        public int GetHashCode(GungeonFlags obj) => (int) obj;
    }

