// Decompiled with JetBrains decompiler
// Type: dfAnimatedFloat
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
  public class dfAnimatedFloat : dfAnimatedValue<float>
  {
    public dfAnimatedFloat(float StartValue, float EndValue, float Time) : base(StartValue, EndValue, Time)
    {
    }

    protected override float Lerp(float startValue, float endValue, float time)
    {
      return Mathf.Lerp(startValue, endValue, time);
    }

    public static implicit operator dfAnimatedFloat(float value)
    {
      return new dfAnimatedFloat(value, value, 0.0f);
    }
  }
}
