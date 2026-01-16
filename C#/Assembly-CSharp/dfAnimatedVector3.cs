// Decompiled with JetBrains decompiler
// Type: dfAnimatedVector3
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class dfAnimatedVector3(Vector3 StartValue, Vector3 EndValue, float Time) : 
  dfAnimatedValue<Vector3>(StartValue, EndValue, Time)
{
  protected override Vector3 Lerp(Vector3 startValue, Vector3 endValue, float time)
  {
    return Vector3.Lerp(startValue, endValue, time);
  }

  public static implicit operator dfAnimatedVector3(Vector3 value)
  {
    return new dfAnimatedVector3(value, value, 0.0f);
  }
}
