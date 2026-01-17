// Decompiled with JetBrains decompiler
// Type: dfTweenRotation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[AddComponentMenu("Daikon Forge/Tweens/Rotation")]
public class dfTweenRotation : dfTweenComponent<Quaternion>
{
  public override Quaternion offset(Quaternion lhs, Quaternion rhs) => lhs * rhs;

  public override Quaternion evaluate(Quaternion startValue, Quaternion endValue, float time)
  {
    return Quaternion.Euler(dfTweenRotation.LerpEuler(startValue.eulerAngles, endValue.eulerAngles, time));
  }

  private static Vector3 LerpEuler(Vector3 startValue, Vector3 endValue, float time)
  {
    return new Vector3(dfTweenRotation.LerpAngle(startValue.x, endValue.x, time), dfTweenRotation.LerpAngle(startValue.y, endValue.y, time), dfTweenRotation.LerpAngle(startValue.z, endValue.z, time));
  }

  private static float LerpAngle(float startValue, float endValue, float time)
  {
    float num = Mathf.Repeat(endValue - startValue, 360f);
    if ((double) num > 180.0)
      num -= 360f;
    return startValue + num * time;
  }
}
