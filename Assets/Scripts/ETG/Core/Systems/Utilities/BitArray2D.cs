// Decompiled with JetBrains decompiler
// Type: BitArray2D
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

public class BitArray2D
  {
    private int m_width;
    private int m_height;
    private bool[] m_bits;
    private float[] m_floats;
    private float c_sizeScalar = 2f;

    public BitArray2D(bool useBackingFloats = false) => this.UsesBackingFloats = useBackingFloats;

    public int Width => this.m_width;

    public int Height => this.m_height;

    public bool IsEmpty => this.m_width == 0 && this.m_height == 0;

    public bool IsValid { get; set; }

    public bool IsAabb { get; set; }

    public bool UsesBackingFloats { get; set; }

    public bool ReadOnly { get; set; }

    public void Reinitialize(int width, int height, bool fixedSize = false)
    {
      this.m_width = width;
      this.m_height = height;
      int length = this.m_width * this.m_height;
      if (this.m_bits == null || length > this.m_bits.Length)
      {
        if (!fixedSize)
          length = (int) ((double) length * (double) this.c_sizeScalar);
        this.m_bits = new bool[length];
      }
      if (this.UsesBackingFloats && (this.m_floats == null || length > this.m_floats.Length))
        this.m_floats = new float[length];
      this.IsValid = true;
    }

    public void ReinitializeWithDefault(
      int width,
      int height,
      bool defaultValue,
      float defaultFloatValue = 0.0f,
      bool fixedSize = false)
    {
      this.m_width = width;
      this.m_height = height;
      int length1 = this.m_width * this.m_height;
      if (!defaultValue && (this.m_bits == null || length1 > this.m_bits.Length))
      {
        if (!fixedSize)
          length1 = (int) ((double) length1 * (double) this.c_sizeScalar);
        this.m_bits = new bool[length1];
      }
      if (this.UsesBackingFloats && (this.m_floats == null || length1 > this.m_floats.Length))
        this.m_floats = new float[length1];
      int length2 = this.m_width * this.m_height;
      if (!defaultValue)
        Array.Clear((Array) this.m_bits, 0, length2);
      else
        this.IsAabb = true;
      if (this.UsesBackingFloats)
      {
        for (int index = 0; index < length2; ++index)
          this.m_floats[index] = defaultFloatValue;
      }
      this.IsValid = true;
    }

    public bool this[int x, int y]
    {
      get => this.IsAabb || this.m_bits[x + y * this.m_width];
      set => this.m_bits[x + y * this.m_width] = value;
    }

    public float GetFloat(int x, int y) => this.m_floats[x + y * this.m_width];

    public void SetFloat(int x, int y, float value) => this.m_floats[x + y * this.m_width] = value;

    public void SetCircle(int x0, int y0, int radius, bool value, SetBackingFloatFunc floatFunc = null)
    {
      int num1 = radius;
      int num2 = 0;
      int num3 = 1 - num1;
      while (num2 <= num1)
      {
        this.SetColumn(num1 + x0, num2 + y0, -num2 + y0, value, floatFunc);
        this.SetColumn(num2 + x0, num1 + y0, -num1 + y0, value, floatFunc);
        this.SetColumn(-num2 + x0, num1 + y0, -num1 + y0, value, floatFunc);
        this.SetColumn(-num1 + x0, num2 + y0, -num2 + y0, value, floatFunc);
        ++num2;
        if (num3 <= 0)
        {
          num3 += 2 * num2 + 1;
        }
        else
        {
          --num1;
          num3 += 2 * (num2 - num1) + 1;
        }
      }
    }

    public void SetColumn(int x, int y0, int y1, bool value, SetBackingFloatFunc floatFunc = null)
    {
      if (y0 > y1)
        BraveUtility.Swap<int>(ref y0, ref y1);
      for (int y = y0; y <= y1; ++y)
        this.SetSafe(x, y, value, floatFunc);
    }

    public void SetSafe(int x, int y, bool value, SetBackingFloatFunc floatFunc = null)
    {
      if (x < 0 || x >= this.m_width || y < 0 || y >= this.m_height)
        return;
      int index = x + y * this.m_width;
      this.m_bits[index] = value;
      if (floatFunc == null)
        return;
      this.m_floats[index] = floatFunc(x, y, value, this.m_floats[index]);
    }
  }

