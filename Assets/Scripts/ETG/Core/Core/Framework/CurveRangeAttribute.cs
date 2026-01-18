using UnityEngine;

#nullable disable

public class CurveRangeAttribute : PropertyAttribute
  {
    public Color Color;
    public Rect Range;

    public CurveRangeAttribute(float xMin, float yMin, float xMax, float yMax)
    {
      this.Range = new Rect(xMin, yMin, xMax, yMax);
      this.Color = Color.green;
    }

    public CurveRangeAttribute(float xMin, float yMin, float xMax, float yMax, Color color)
    {
      this.Range = new Rect(xMin, yMin, xMax, yMax);
      this.Color = color;
    }
  }

