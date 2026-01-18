using System;
using UnityEngine;

#nullable disable

public class IntRect
  {
    private int x;
    private int y;
    private int dimX;
    private int dimY;

    public IntRect(int left, int bottom, int width, int height)
    {
      this.x = left;
      this.y = bottom;
      this.dimX = width;
      this.dimY = height;
    }

    public static IntRect FromTwoPoints(IntVector2 p1, IntVector2 p2)
    {
      IntVector2 intVector2_1 = IntVector2.Min(p1, p2);
      IntVector2 intVector2_2 = IntVector2.Max(p1, p2) - intVector2_1;
      return new IntRect(intVector2_1.x, intVector2_1.y, intVector2_2.x, intVector2_2.y);
    }

    public static IntRect Intersection(IntRect a, IntRect b, int expand = 0)
    {
      int left = Math.Max(a.x - expand, b.x - expand);
      int bottom = Math.Max(a.y - expand, b.y - expand);
      int width = Math.Min(a.Right + expand, b.Right + expand) - left;
      int height = Math.Min(a.Top + expand, b.Top + expand) - bottom;
      return height <= 0 || width <= 0 ? (IntRect) null : new IntRect(left, bottom, width, height);
    }

    public IntVector2 Dimensions => new IntVector2(this.Width, this.Height);

    public float Aspect => (float) this.Width / (float) this.Height;

    public int Area => this.Width * this.Height;

    public IntVector2[] FourPoints
    {
      get
      {
        return new IntVector2[4]
        {
          new IntVector2(this.x, this.y),
          new IntVector2(this.x, this.y + this.dimY),
          new IntVector2(this.x + this.dimX, this.y + this.dimY),
          new IntVector2(this.x + this.dimX, this.y)
        };
      }
    }

    public IntVector2 Min => new IntVector2(this.x, this.y);

    public IntVector2 Max => new IntVector2(this.x + this.dimX, this.y + this.dimY);

    public int Left
    {
      get => this.x;
      set => this.x = value;
    }

    public int Top => this.y + this.dimY;

    public int Right => this.x + this.dimX;

    public int Bottom
    {
      get => this.y;
      set => this.y = value;
    }

    public int Width => this.dimX;

    public int Height => this.dimY;

    public int Metric => Math.Max(this.Width, this.Height);

    public Vector2 Center
    {
      get
      {
        return new Vector2((float) this.x + (float) this.dimX / 2f, (float) this.y + (float) this.dimY / 2f);
      }
    }

    public bool Contains(Vector2 vec)
    {
      return (double) vec.x >= (double) this.x && (double) vec.x <= (double) (this.x + this.dimX) && (double) vec.y >= (double) this.y && (double) vec.y <= (double) (this.y + this.dimY);
    }
  }

