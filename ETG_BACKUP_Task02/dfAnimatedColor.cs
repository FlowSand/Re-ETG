// Decompiled with JetBrains decompiler
// Type: dfAnimatedColor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class dfAnimatedColor(Color StartValue, Color EndValue, float Time) : dfAnimatedValue<Color>(StartValue, EndValue, Time)
{
  protected override Color Lerp(Color startValue, Color endValue, float time)
  {
    return Color.Lerp(startValue, endValue, time);
  }

  public static implicit operator dfAnimatedColor(Color value)
  {
    return new dfAnimatedColor(value, value, 0.0f);
  }
}
