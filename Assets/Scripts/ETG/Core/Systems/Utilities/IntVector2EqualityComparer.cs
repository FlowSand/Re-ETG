using System.Collections.Generic;

#nullable disable

public class IntVector2EqualityComparer : IEqualityComparer<IntVector2>
  {
    public bool Equals(IntVector2 a, IntVector2 b) => a.x == b.x && a.y == b.y;

    public int GetHashCode(IntVector2 obj)
    {
      return (17 * 23 + obj.x.GetHashCode()) * 23 + obj.y.GetHashCode();
    }
  }

