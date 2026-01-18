using System.Collections.Generic;

#nullable disable

public class TrackedStatsComparer : IEqualityComparer<TrackedStats>
    {
        public bool Equals(TrackedStats x, TrackedStats y) => x == y;

        public int GetHashCode(TrackedStats obj) => (int) obj;
    }

