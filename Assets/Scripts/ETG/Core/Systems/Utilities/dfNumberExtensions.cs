using UnityEngine;

#nullable disable

  public static class dfNumberExtensions
  {
    public static int Quantize(this int value, int stepSize)
    {
      return stepSize <= 0 ? value : value / stepSize * stepSize;
    }

    public static float Quantize(this float value, float stepSize)
    {
      return (double) stepSize <= 0.0 ? value : Mathf.Floor(value / stepSize) * stepSize;
    }

    public static float Quantize(
      this float value,
      float stepSize,
      VectorConversions conversionMethod)
    {
      if ((double) stepSize <= 0.0)
        return value;
      if (conversionMethod == VectorConversions.Floor)
        return Mathf.Floor(value / stepSize) * stepSize;
      if (conversionMethod == VectorConversions.Ceil)
        return Mathf.Ceil(value / stepSize) * stepSize;
      return conversionMethod == VectorConversions.Round ? Mathf.Round(value / stepSize) * stepSize : Mathf.Round(value / stepSize) * stepSize;
    }

    public static int RoundToNearest(this int value, int stepSize)
    {
      if (stepSize <= 0)
        return value;
      int num = value / stepSize * stepSize;
      return value % stepSize >= stepSize / 2 ? num + stepSize : num;
    }

    public static float RoundToNearest(this float value, float stepSize)
    {
      if ((double) stepSize <= 0.0)
        return value;
      float num = Mathf.Floor(value / stepSize) * stepSize;
      return (double) (value - stepSize * Mathf.Floor(value / stepSize)) >= (double) stepSize * 0.5 - 1.4012984643248171E-45 ? num + stepSize : num;
    }
  }

