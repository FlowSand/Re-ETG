// Decompiled with JetBrains decompiler
// Type: dfTweenVector4
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [AddComponentMenu("Daikon Forge/Tweens/Vector4")]
    public class dfTweenVector4 : dfTweenComponent<Vector4>
    {
      public override Vector4 offset(Vector4 lhs, Vector4 rhs) => lhs + rhs;

      public override Vector4 evaluate(Vector4 startValue, Vector4 endValue, float time)
      {
        return new Vector4(dfTweenComponent<Vector4>.Lerp(startValue.x, endValue.x, time), dfTweenComponent<Vector4>.Lerp(startValue.y, endValue.y, time), dfTweenComponent<Vector4>.Lerp(startValue.z, endValue.z, time), dfTweenComponent<Vector4>.Lerp(startValue.w, endValue.w, time));
      }
    }

}
