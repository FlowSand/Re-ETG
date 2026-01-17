// Decompiled with JetBrains decompiler
// Type: CurveRangeAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

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
