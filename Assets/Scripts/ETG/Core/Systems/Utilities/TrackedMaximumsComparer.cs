using System.Collections.Generic;

#nullable disable

public class TrackedMaximumsComparer : IEqualityComparer<TrackedMaximums>
    {
        public bool Equals(TrackedMaximums x, TrackedMaximums y) => x == y;

        public int GetHashCode(TrackedMaximums obj) => (int) obj;
    }

