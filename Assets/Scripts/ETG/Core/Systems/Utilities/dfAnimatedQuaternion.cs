// Decompiled with JetBrains decompiler
// Type: dfAnimatedQuaternion
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

public class dfAnimatedQuaternion : dfAnimatedValue<Quaternion>
{
  public dfAnimatedQuaternion(Quaternion StartValue, Quaternion EndValue, float Time) : base(StartValue, EndValue, Time)
  {
  }

  protected override Quaternion Lerp(Quaternion startValue, Quaternion endValue, float time)
    {
      return Quaternion.Lerp(startValue, endValue, time);
    }

    public static implicit operator dfAnimatedQuaternion(Quaternion value)
    {
      return new dfAnimatedQuaternion(value, value, 0.0f);
    }
  }

