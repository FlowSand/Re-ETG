// Decompiled with JetBrains decompiler
// Type: DaikonForge.Tween.Interpolation.Interpolator`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Beebyte.Obfuscator;

#nullable disable
namespace DaikonForge.Tween.Interpolation;

public abstract class Interpolator<T>
{
  [Skip]
  public abstract T Add(T lhs, T rhs);

  [Skip]
  public abstract T Interpolate(T startValue, T endValue, float time);
}
