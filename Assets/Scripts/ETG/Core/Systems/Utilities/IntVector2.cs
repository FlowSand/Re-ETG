using System;
using UnityEngine;

#nullable disable

[Serializable]
public struct IntVector2
  {
    public static IntVector2 Zero = new IntVector2(0, 0);
    public static IntVector2 One = new IntVector2(1, 1);
    public static IntVector2 NegOne = new IntVector2(-1, -1);
    public static IntVector2 Up = new IntVector2(0, 1);
    public static IntVector2 Right = new IntVector2(1, 0);
    public static IntVector2 Down = new IntVector2(0, -1);
    public static IntVector2 Left = new IntVector2(-1, 0);
    public static IntVector2 UpRight = new IntVector2(1, 1);
    public static IntVector2 DownRight = new IntVector2(1, -1);
    public static IntVector2 DownLeft = new IntVector2(-1, -1);
    public static IntVector2 UpLeft = new IntVector2(-1, 1);
    public static IntVector2 North = new IntVector2(0, 1);
    public static IntVector2 East = new IntVector2(1, 0);
    public static IntVector2 South = new IntVector2(0, -1);
    public static IntVector2 West = new IntVector2(-1, 0);
    public static IntVector2 NorthEast = new IntVector2(1, 1);
    public static IntVector2 SouthEast = new IntVector2(1, -1);
    public static IntVector2 SouthWest = new IntVector2(-1, -1);
    public static IntVector2 NorthWest = new IntVector2(-1, 1);
    public static IntVector2 MinValue = new IntVector2(int.MinValue, int.MinValue);
    public static IntVector2 MaxValue = new IntVector2(int.MaxValue, int.MaxValue);
    public static IntVector2[] m_cachedCardinals;
    public static IntVector2[] m_cachedOrdinals;
    public static IntVector2[] m_cachedCardinalsAndOrdinals;
    public int x;
    public int y;

    public IntVector2(int x, int y)
    {
      this.x = x;
      this.y = y;
    }

    public static IntVector2[] Cardinals
    {
      get
      {
        if (IntVector2.m_cachedCardinals == null)
          IntVector2.m_cachedCardinals = new IntVector2[4]
          {
            IntVector2.North,
            IntVector2.East,
            IntVector2.South,
            IntVector2.West
          };
        return IntVector2.m_cachedCardinals;
      }
    }

    public static IntVector2[] Ordinals
    {
      get
      {
        if (IntVector2.m_cachedOrdinals == null)
          IntVector2.m_cachedOrdinals = new IntVector2[4]
          {
            IntVector2.NorthEast,
            IntVector2.SouthEast,
            IntVector2.SouthWest,
            IntVector2.NorthWest
          };
        return IntVector2.m_cachedOrdinals;
      }
    }

    public static IntVector2[] CardinalsAndOrdinals
    {
      get
      {
        if (IntVector2.m_cachedCardinalsAndOrdinals == null)
          IntVector2.m_cachedCardinalsAndOrdinals = new IntVector2[8]
          {
            IntVector2.North,
            IntVector2.NorthEast,
            IntVector2.East,
            IntVector2.SouthEast,
            IntVector2.South,
            IntVector2.SouthWest,
            IntVector2.West,
            IntVector2.NorthWest
          };
        return IntVector2.m_cachedCardinalsAndOrdinals;
      }
    }

    public int X => this.x;

    public int Y => this.y;

    public IntVector2 MajorAxis
    {
      get
      {
        if (this.x == 0 && this.y == 0)
          return IntVector2.Zero;
        return Mathf.Abs(this.x) > Mathf.Abs(this.y) ? new IntVector2(Math.Sign(this.x), 0) : new IntVector2(0, Math.Sign(this.y));
      }
    }

    public Vector2 ToVector2() => new Vector2((float) this.x, (float) this.y);

    public Vector2 ToVector2(float xOffset, float yOffset)
    {
      return new Vector2((float) this.x + xOffset, (float) this.y + yOffset);
    }

    public Vector2 ToCenterVector2()
    {
      return (Vector2) new Vector3((float) this.x + 0.5f, (float) this.y + 0.5f);
    }

    public Vector3 ToVector3() => new Vector3((float) this.x, (float) this.y, 0.0f);

    public Vector3 ToVector3(float height) => new Vector3((float) this.x, (float) this.y, height);

    public Vector3 ToCenterVector3(float height)
    {
      return new Vector3((float) this.x + 0.5f, (float) this.y + 0.5f, height);
    }

    public int sqrMagnitude => this.x * this.x + this.y * this.y;

    public bool IsWithin(IntVector2 min, IntVector2 max)
    {
      return this.x >= min.x && this.x <= max.x && this.y >= min.y && this.y <= max.y;
    }

    public int ComponentSum => Math.Abs(this.x) + Math.Abs(this.y);

    public override string ToString() => $"{this.x},{this.y}";

    public bool Equals(IntVector2 other) => this.x == other.x && this.y == other.y;

    public override bool Equals(object obj)
    {
      return obj is IntVector2 intVector2 ? this == intVector2 : base.Equals(obj);
    }

    public override int GetHashCode() => 100267 * this.x + 200233 * this.y;

    public float GetHashedRandomValue()
    {
      int num1 = 0 + this.x;
      int num2 = num1 + (num1 << 10);
      int num3 = (num2 ^ num2 >> 6) + this.y;
      int num4 = num3 + (num3 << 10);
      int num5 = num4 ^ num4 >> 6;
      int num6 = num5 + (num5 << 3);
      int num7 = num6 ^ num6 >> 11;
      return (float) ((double) (uint) (num7 + (num7 << 15)) * 1.0 / 4294967296.0);
    }

    public IntVector2 WithX(int newX) => new IntVector2(newX, this.y);

    public IntVector2 WithY(int newY) => new IntVector2(this.x, newY);

    public static IntVector2 operator +(IntVector2 a, IntVector2 b)
    {
      return new IntVector2(a.x + b.x, a.y + b.y);
    }

    public static IntVector2 operator -(IntVector2 a, IntVector2 b)
    {
      return new IntVector2(a.x - b.x, a.y - b.y);
    }

    public static IntVector2 operator *(IntVector2 a, int b) => new IntVector2(a.x * b, a.y * b);

    public static IntVector2 operator *(int a, IntVector2 b) => new IntVector2(a * b.x, a * b.y);

    public static IntVector2 operator /(IntVector2 a, int b) => new IntVector2(a.x / b, a.y / b);

    public static IntVector2 operator -(IntVector2 a) => new IntVector2(-a.x, -a.y);

    public static bool operator ==(IntVector2 a, IntVector2 b) => a.x == b.x && a.y == b.y;

    public static bool operator !=(IntVector2 a, IntVector2 b) => a.x != b.x || a.y != b.y;

    public static int ManhattanDistance(IntVector2 a, IntVector2 b)
    {
      return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
    }

    public static int ManhattanDistance(IntVector2 a, int bx, int by)
    {
      return Math.Abs(a.x - bx) + Math.Abs(a.y - by);
    }

    public static int ManhattanDistance(int ax, int ay, int bx, int by)
    {
      return Math.Abs(ax - bx) + Math.Abs(ay - by);
    }

    public static float Distance(IntVector2 a, IntVector2 b)
    {
      return Mathf.Sqrt((float) ((b.y - a.y) * (b.y - a.y) + (b.x - a.x) * (b.x - a.x)));
    }

    public static float Distance(IntVector2 a, int bx, int by)
    {
      return Mathf.Sqrt((float) ((by - a.y) * (by - a.y) + (bx - a.x) * (bx - a.x)));
    }

    public static float Distance(int ax, int ay, int bx, int by)
    {
      return Mathf.Sqrt((float) ((by - ay) * (by - ay) + (bx - ax) * (bx - ax)));
    }

    public static float DistanceSquared(IntVector2 a, IntVector2 b)
    {
      return (float) ((b.y - a.y) * (b.y - a.y) + (b.x - a.x) * (b.x - a.x));
    }

    public static float DistanceSquared(IntVector2 a, int bx, int by)
    {
      return (float) ((by - a.y) * (by - a.y) + (bx - a.x) * (bx - a.x));
    }

    public static float DistanceSquared(int ax, int ay, int bx, int by)
    {
      return (float) ((by - ay) * (by - ay) + (bx - ax) * (bx - ax));
    }

    public void Clamp(IntVector2 min, IntVector2 max)
    {
      this.x = Math.Max(this.x, min.x);
      this.y = Math.Max(this.y, min.y);
      this.x = Math.Min(this.x, max.x);
      this.y = Math.Min(this.y, max.y);
    }

    public static IntVector2 Scale(IntVector2 lhs, IntVector2 rhs)
    {
      return new IntVector2(lhs.x * rhs.x, lhs.y * rhs.y);
    }

    public static IntVector2 Min(IntVector2 lhs, IntVector2 rhs)
    {
      return new IntVector2(Mathf.Min(lhs.x, rhs.x), Mathf.Min(lhs.y, rhs.y));
    }

    public static IntVector2 Max(IntVector2 lhs, IntVector2 rhs)
    {
      return new IntVector2(Mathf.Max(lhs.x, rhs.x), Mathf.Max(lhs.y, rhs.y));
    }

    public static bool AABBOverlap(
      IntVector2 posA,
      IntVector2 dimensionsA,
      IntVector2 posB,
      IntVector2 dimensionsB)
    {
      return posA.x + dimensionsA.x - 1 >= posB.x && posA.x <= posB.x + dimensionsB.x - 1 && posA.y + dimensionsA.y - 1 >= posB.y && posA.y <= posB.y + dimensionsB.y - 1;
    }

    public static bool AABBOverlapWithArea(
      IntVector2 posA,
      IntVector2 dimensionsA,
      IntVector2 posB,
      IntVector2 dimensionsB,
      out int cellsOverlapping)
    {
      if (posA.x + dimensionsA.x - 1 < posB.x || posA.x > posB.x + dimensionsB.x - 1 || posA.y + dimensionsA.y - 1 < posB.y || posA.y > posB.y + dimensionsB.y - 1)
      {
        cellsOverlapping = 0;
        return false;
      }
      int num1 = Mathf.Max(0, Mathf.Min(posA.x + dimensionsA.x, posB.x + dimensionsB.x) - Mathf.Max(posA.x, posB.x));
      int num2 = Mathf.Max(0, Mathf.Min(posA.y + dimensionsA.y, posB.y + dimensionsB.y) - Mathf.Max(posA.y, posB.y));
      cellsOverlapping = num1 * num2;
      return true;
    }

    public static explicit operator Vector2(IntVector2 v) => new Vector2((float) v.x, (float) v.y);
  }

