// Decompiled with JetBrains decompiler
// Type: dfAnimatedInt
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class dfAnimatedInt(int StartValue, int EndValue, float Time) : dfAnimatedValue<int>(StartValue, EndValue, Time)
{
  protected override int Lerp(int startValue, int endValue, float time)
  {
    return Mathf.RoundToInt(Mathf.Lerp((float) startValue, (float) endValue, time));
  }

  public static implicit operator dfAnimatedInt(int value) => new dfAnimatedInt(value, value, 0.0f);
}
