// Decompiled with JetBrains decompiler
// Type: dfTweenVector3
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[AddComponentMenu("Daikon Forge/Tweens/Vector3")]
public class dfTweenVector3 : dfTweenComponent<Vector3>
{
  public override Vector3 offset(Vector3 lhs, Vector3 rhs) => lhs + rhs;

  public override Vector3 evaluate(Vector3 startValue, Vector3 endValue, float time)
  {
    return new Vector3(dfTweenComponent<Vector3>.Lerp(startValue.x, endValue.x, time), dfTweenComponent<Vector3>.Lerp(startValue.y, endValue.y, time), dfTweenComponent<Vector3>.Lerp(startValue.z, endValue.z, time));
  }
}
