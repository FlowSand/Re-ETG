// Decompiled with JetBrains decompiler
// Type: DaikonForge.Tween.CatmullRomSpline
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace DaikonForge.Tween
{
  public class CatmullRomSpline : ISplineInterpolator
  {
    public Vector3 Evaluate(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
    {
      return 0.5f * (2f * b + (-a + c) * t + (2f * a - 5f * b + 4f * c - d) * t * t + (-a + 3f * b - 3f * c + d) * t * t * t);
    }
  }
}
