// Decompiled with JetBrains decompiler
// Type: dfTweenColor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [AddComponentMenu("Daikon Forge/Tweens/Color")]
    public class dfTweenColor : dfTweenComponent<Color>
    {
      public override Color offset(Color lhs, Color rhs) => lhs + rhs;

      public override Color evaluate(Color startValue, Color endValue, float time)
      {
        Vector4 vector4_1 = (Vector4) startValue;
        Vector4 vector4_2 = (Vector4) endValue;
        return (Color) new Vector4(dfTweenComponent<Color>.Lerp(vector4_1.x, vector4_2.x, time), dfTweenComponent<Color>.Lerp(vector4_1.y, vector4_2.y, time), dfTweenComponent<Color>.Lerp(vector4_1.z, vector4_2.z, time), dfTweenComponent<Color>.Lerp(vector4_1.w, vector4_2.w, time));
      }
    }

}
