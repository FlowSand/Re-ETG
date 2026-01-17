// Decompiled with JetBrains decompiler
// Type: dfTweenVector2
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[AddComponentMenu("Daikon Forge/Tweens/Vector2")]
public class dfTweenVector2 : dfTweenComponent<Vector2>
{
  public override Vector2 offset(Vector2 lhs, Vector2 rhs) => lhs + rhs;

  public override Vector2 evaluate(Vector2 startValue, Vector2 endValue, float time)
  {
    return new Vector2(dfTweenComponent<Vector2>.Lerp(startValue.x, endValue.x, time), dfTweenComponent<Vector2>.Lerp(startValue.y, endValue.y, time));
  }
}
