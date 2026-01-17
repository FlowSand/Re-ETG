// Decompiled with JetBrains decompiler
// Type: dfAnimatedVector2
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class dfAnimatedVector2(Vector2 StartValue, Vector2 EndValue, float Time) : 
      dfAnimatedValue<Vector2>(StartValue, EndValue, Time)
    {
      protected override Vector2 Lerp(Vector2 startValue, Vector2 endValue, float time)
      {
        return Vector2.Lerp(startValue, endValue, time);
      }

      public static implicit operator dfAnimatedVector2(Vector2 value)
      {
        return new dfAnimatedVector2(value, value, 0.0f);
      }
    }

}
