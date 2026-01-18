// Decompiled with JetBrains decompiler
// Type: dfTweenColor32
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/Tweens/Color32")]
public class dfTweenColor32 : dfTweenComponent<Color32>
  {
    public override Color32 offset(Color32 lhs, Color32 rhs) => (Color32) ((Color) lhs + (Color) rhs);

    public override Color32 evaluate(Color32 startValue, Color32 endValue, float time)
    {
      Vector4 vector4_1 = (Vector4) (Color) startValue;
      Vector4 vector4_2 = (Vector4) (Color) endValue;
      return (Color32) (Color) new Vector4(dfTweenComponent<Color32>.Lerp(vector4_1.x, vector4_2.x, time), dfTweenComponent<Color32>.Lerp(vector4_1.y, vector4_2.y, time), dfTweenComponent<Color32>.Lerp(vector4_1.z, vector4_2.z, time), dfTweenComponent<Color32>.Lerp(vector4_1.w, vector4_2.w, time));
    }
  }

