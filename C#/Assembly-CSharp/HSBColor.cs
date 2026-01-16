// Decompiled with JetBrains decompiler
// Type: HSBColor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
[Serializable]
public struct HSBColor
{
  public float h;
  public float s;
  public float b;
  public float a;

  public HSBColor(float h, float s, float b, float a)
  {
    this.h = h;
    this.s = s;
    this.b = b;
    this.a = a;
  }

  public HSBColor(float h, float s, float b)
  {
    this.h = h;
    this.s = s;
    this.b = b;
    this.a = 1f;
  }

  public HSBColor(Color col)
  {
    HSBColor hsbColor = HSBColor.FromColor(col);
    this.h = hsbColor.h;
    this.s = hsbColor.s;
    this.b = hsbColor.b;
    this.a = hsbColor.a;
  }

  public static Color GetHue(Color color)
  {
    HSBColor hue = HSBColor.FromColor(color);
    hue.s = hue.b = 1f;
    return (Color) hue;
  }

  public static HSBColor FromColor(Color color)
  {
    HSBColor hsbColor = new HSBColor(0.0f, 0.0f, 0.0f, color.a);
    HSBColor.RGBToHSV(color, out hsbColor.h, out hsbColor.s, out hsbColor.b);
    return hsbColor;
  }

  public static Color ToColor(HSBColor hsbColor)
  {
    float num1 = hsbColor.b;
    float num2 = hsbColor.b;
    float num3 = hsbColor.b;
    if ((double) hsbColor.s != 0.0)
    {
      float b = hsbColor.b;
      float num4 = hsbColor.b * hsbColor.s;
      float num5 = hsbColor.b - num4;
      float num6 = hsbColor.h * 360f;
      if ((double) num6 < 60.0)
      {
        num1 = b;
        num2 = (float) ((double) num6 * (double) num4 / 60.0) + num5;
        num3 = num5;
      }
      else if ((double) num6 < 120.0)
      {
        num1 = (float) (-((double) num6 - 120.0) * (double) num4 / 60.0) + num5;
        num2 = b;
        num3 = num5;
      }
      else if ((double) num6 < 180.0)
      {
        num1 = num5;
        num2 = b;
        num3 = (float) (((double) num6 - 120.0) * (double) num4 / 60.0) + num5;
      }
      else if ((double) num6 < 240.0)
      {
        num1 = num5;
        num2 = (float) (-((double) num6 - 240.0) * (double) num4 / 60.0) + num5;
        num3 = b;
      }
      else if ((double) num6 < 300.0)
      {
        num1 = (float) (((double) num6 - 240.0) * (double) num4 / 60.0) + num5;
        num2 = num5;
        num3 = b;
      }
      else if ((double) num6 <= 360.0)
      {
        num1 = b;
        num2 = num5;
        num3 = (float) (-((double) num6 - 360.0) * (double) num4 / 60.0) + num5;
      }
      else
      {
        num1 = 0.0f;
        num2 = 0.0f;
        num3 = 0.0f;
      }
    }
    return new Color(Mathf.Clamp01(num1), Mathf.Clamp01(num2), Mathf.Clamp01(num3), hsbColor.a);
  }

  public Color ToColor() => HSBColor.ToColor(this);

  public static HSBColor Lerp(HSBColor a, HSBColor b, float t)
  {
    float h;
    float s;
    if ((double) a.b == 0.0)
    {
      h = b.h;
      s = b.s;
    }
    else if ((double) b.b == 0.0)
    {
      h = a.h;
      s = a.s;
    }
    else
    {
      if ((double) a.s == 0.0)
        h = b.h;
      else if ((double) b.s == 0.0)
      {
        h = a.h;
      }
      else
      {
        float num = Mathf.LerpAngle(a.h * 360f, b.h * 360f, t);
        while ((double) num < 0.0)
          num += 360f;
        while ((double) num > 360.0)
          num -= 360f;
        h = num / 360f;
      }
      s = Mathf.Lerp(a.s, b.s, t);
    }
    return new HSBColor(h, s, Mathf.Lerp(a.b, b.b, t), Mathf.Lerp(a.a, b.a, t));
  }

  private static void RGBToHSV(Color rgbColor, out float H, out float S, out float V)
  {
    if ((double) rgbColor.b > (double) rgbColor.g && (double) rgbColor.b > (double) rgbColor.r)
      HSBColor.RGBToHSVHelper(4f, rgbColor.b, rgbColor.r, rgbColor.g, out H, out S, out V);
    else if ((double) rgbColor.g > (double) rgbColor.r)
      HSBColor.RGBToHSVHelper(2f, rgbColor.g, rgbColor.b, rgbColor.r, out H, out S, out V);
    else
      HSBColor.RGBToHSVHelper(0.0f, rgbColor.r, rgbColor.g, rgbColor.b, out H, out S, out V);
  }

  private static void RGBToHSVHelper(
    float offset,
    float dominantcolor,
    float colorone,
    float colortwo,
    out float H,
    out float S,
    out float V)
  {
    V = dominantcolor;
    if ((double) V != 0.0)
    {
      float num1 = (double) colorone <= (double) colortwo ? colorone : colortwo;
      float num2 = V - num1;
      if ((double) num2 != 0.0)
      {
        S = num2 / V;
        H = offset + (colorone - colortwo) / num2;
      }
      else
      {
        S = 0.0f;
        H = offset + (colorone - colortwo);
      }
      H /= 6f;
      if ((double) H >= 0.0)
        return;
      ++H;
    }
    else
    {
      S = 0.0f;
      H = 0.0f;
    }
  }

  public static implicit operator HSBColor(Color color) => HSBColor.FromColor(color);

  public static implicit operator HSBColor(Color32 color) => HSBColor.FromColor((Color) color);

  public static implicit operator Color(HSBColor hsb) => hsb.ToColor();

  public static implicit operator Color32(HSBColor hsb) => (Color32) hsb.ToColor();

  public override string ToString() => $"H:{this.h}, S:{this.s}, B:{this.b}";
}
