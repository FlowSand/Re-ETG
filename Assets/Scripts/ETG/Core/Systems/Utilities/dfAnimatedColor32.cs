// Decompiled with JetBrains decompiler
// Type: dfAnimatedColor32
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class dfAnimatedColor32(Color32 StartValue, Color32 EndValue, float Time) : 
      dfAnimatedValue<Color32>(StartValue, EndValue, Time)
    {
      protected override Color32 Lerp(Color32 startValue, Color32 endValue, float time)
      {
        return (Color32) Color.Lerp((Color) startValue, (Color) endValue, time);
      }

      public static implicit operator dfAnimatedColor32(Color32 value)
      {
        return new dfAnimatedColor32(value, value, 0.0f);
      }
    }

}
