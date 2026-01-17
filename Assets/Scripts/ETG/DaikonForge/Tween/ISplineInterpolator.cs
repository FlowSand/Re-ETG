// Decompiled with JetBrains decompiler
// Type: DaikonForge.Tween.ISplineInterpolator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace DaikonForge.Tween
{
  public interface ISplineInterpolator
  {
    Vector3 Evaluate(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t);
  }
}
