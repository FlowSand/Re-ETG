using System;
using UnityEngine;

#nullable disable

[Serializable]
public class BagelCollider
  {
    public int width;
    public int height;
    [SerializeField]
    private int minX;
    [SerializeField]
    private int minY;
    [SerializeField]
    private int actualWidth;
    [SerializeField]
    private int actualHeight;
    [SerializeField]
    private bool[] bagelCollider;

    public BagelCollider(BagelCollider source)
    {
      this.width = source.width;
      this.height = source.height;
      this.minX = source.minX;
      this.minY = source.minY;
      this.actualWidth = source.actualWidth;
      this.actualHeight = source.actualHeight;
      this.bagelCollider = new bool[source.bagelCollider.Length];
      for (int index = 0; index < source.bagelCollider.Length; ++index)
        this.bagelCollider[index] = source.bagelCollider[index];
    }

    public BagelCollider(int width, int height)
    {
      this.width = width;
      this.height = height;
      this.actualWidth = 0;
      this.actualHeight = 0;
      this.bagelCollider = new bool[0];
    }

    public bool this[int x, int y]
    {
      get
      {
        if (this.actualWidth == 0 && this.bagelCollider != null && this.bagelCollider.Length > 0)
        {
          this.actualWidth = this.width;
          this.actualHeight = this.height;
        }
        return x >= this.minX && x < this.minX + this.actualWidth && y >= this.minY && y < this.minY + this.actualHeight && this.bagelCollider[(y - this.minY) * this.actualWidth + (x - this.minX)];
      }
    }
  }

