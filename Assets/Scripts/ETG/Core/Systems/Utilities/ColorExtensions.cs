using UnityEngine;

#nullable disable

  public static class ColorExtensions
  {
    public static bool EqualsNonAlloc(this Color32 color, Color32 other)
    {
      return (int) color.r == (int) other.r && (int) color.g == (int) other.g && (int) color.b == (int) other.b && (int) color.a == (int) other.a;
    }

    public static Color SmoothStep(Color start, Color end, float t)
    {
      return new Color(Mathf.SmoothStep(start.r, end.r, t), Mathf.SmoothStep(start.g, end.g, t), Mathf.SmoothStep(start.b, end.b, t), Mathf.SmoothStep(start.a, end.a, t));
    }
  }

