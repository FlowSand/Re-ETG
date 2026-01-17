// Decompiled with JetBrains decompiler
// Type: dfAnimatedVector4
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class dfAnimatedVector4(Vector4 StartValue, Vector4 EndValue, float Time) : 
  dfAnimatedValue<Vector4>(StartValue, EndValue, Time)
{
  protected override Vector4 Lerp(Vector4 startValue, Vector4 endValue, float time)
  {
    return Vector4.Lerp(startValue, endValue, time);
  }

  public static implicit operator dfAnimatedVector4(Vector4 value)
  {
    return new dfAnimatedVector4(value, value, 0.0f);
  }
}
