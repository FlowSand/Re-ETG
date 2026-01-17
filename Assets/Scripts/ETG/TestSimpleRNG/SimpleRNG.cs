// Decompiled with JetBrains decompiler
// Type: TestSimpleRNG.SimpleRNG
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace TestSimpleRNG;

public class SimpleRNG
{
  private static uint m_w = 521288629;
  private static uint m_z = 362436069;

  public static void SetSeed(uint u, uint v)
  {
    if (u != 0U)
      SimpleRNG.m_w = u;
    if (v == 0U)
      return;
    SimpleRNG.m_z = v;
  }

  public static void SetSeed(uint u) => SimpleRNG.m_w = u;

  public static void SetSeedFromSystemTime()
  {
    long fileTime = DateTime.Now.ToFileTime();
    SimpleRNG.SetSeed((uint) (fileTime >> 16 /*0x10*/), (uint) (fileTime % 4294967296L /*0x0100000000*/));
  }

  public static double GetUniform()
  {
    return ((double) SimpleRNG.GetUint() + 1.0) * 2.3283064354544941E-10;
  }

  private static uint GetUint()
  {
    SimpleRNG.m_z = (uint) (36969 * ((int) SimpleRNG.m_z & (int) ushort.MaxValue)) + (SimpleRNG.m_z >> 16 /*0x10*/);
    SimpleRNG.m_w = (uint) (18000 * ((int) SimpleRNG.m_w & (int) ushort.MaxValue)) + (SimpleRNG.m_w >> 16 /*0x10*/);
    return (SimpleRNG.m_z << 16 /*0x10*/) + SimpleRNG.m_w;
  }

  public static double GetNormal()
  {
    double uniform1 = SimpleRNG.GetUniform();
    double uniform2 = SimpleRNG.GetUniform();
    return Math.Sqrt(-2.0 * Math.Log(uniform1)) * Math.Sin(2.0 * Math.PI * uniform2);
  }

  public static double GetNormal(double mean, double standardDeviation)
  {
    if (standardDeviation <= 0.0)
      throw new ArgumentOutOfRangeException($"Shape must be positive. Received {standardDeviation}.");
    return mean + standardDeviation * SimpleRNG.GetNormal();
  }

  public static double GetExponential() => -Math.Log(SimpleRNG.GetUniform());

  public static double GetExponential(double mean)
  {
    if (mean <= 0.0)
      throw new ArgumentOutOfRangeException($"Mean must be positive. Received {mean}.");
    return mean * SimpleRNG.GetExponential();
  }

  public static double GetGamma(double shape, double scale)
  {
    if (shape >= 1.0)
    {
      double num1 = shape - 1.0 / 3.0;
      double num2 = 1.0 / Math.Sqrt(9.0 * num1);
      double d;
      double uniform;
      double num3;
      do
      {
        double normal;
        double num4;
        do
        {
          normal = SimpleRNG.GetNormal();
          num4 = 1.0 + num2 * normal;
        }
        while (num4 <= 0.0);
        d = num4 * num4 * num4;
        uniform = SimpleRNG.GetUniform();
        num3 = normal * normal;
      }
      while (uniform >= 1.0 - 0.0331 * num3 * num3 && Math.Log(uniform) >= 0.5 * num3 + num1 * (1.0 - d + Math.Log(d)));
      return scale * num1 * d;
    }
    double num = shape > 0.0 ? SimpleRNG.GetGamma(shape + 1.0, 1.0) : throw new ArgumentOutOfRangeException($"Shape must be positive. Received {shape}.");
    double uniform1 = SimpleRNG.GetUniform();
    return scale * num * Math.Pow(uniform1, 1.0 / shape);
  }

  public static double GetChiSquare(double degreesOfFreedom)
  {
    return SimpleRNG.GetGamma(0.5 * degreesOfFreedom, 2.0);
  }

  public static double GetInverseGamma(double shape, double scale)
  {
    return 1.0 / SimpleRNG.GetGamma(shape, 1.0 / scale);
  }

  public static double GetWeibull(double shape, double scale)
  {
    if (shape <= 0.0 || scale <= 0.0)
      throw new ArgumentOutOfRangeException($"Shape and scale parameters must be positive. Recieved shape {shape} and scale{scale}.");
    return scale * Math.Pow(-Math.Log(SimpleRNG.GetUniform()), 1.0 / shape);
  }

  public static double GetCauchy(double median, double scale)
  {
    if (scale <= 0.0)
      throw new ArgumentException($"Scale must be positive. Received {scale}.");
    double uniform = SimpleRNG.GetUniform();
    return median + scale * Math.Tan(Math.PI * (uniform - 0.5));
  }

  public static double GetStudentT(double degreesOfFreedom)
  {
    if (degreesOfFreedom <= 0.0)
      throw new ArgumentException($"Degrees of freedom must be positive. Received {degreesOfFreedom}.");
    return SimpleRNG.GetNormal() / Math.Sqrt(SimpleRNG.GetChiSquare(degreesOfFreedom) / degreesOfFreedom);
  }

  public static double GetLaplace(double mean, double scale)
  {
    double uniform = SimpleRNG.GetUniform();
    return uniform < 0.5 ? mean + scale * Math.Log(2.0 * uniform) : mean - scale * Math.Log(2.0 * (1.0 - uniform));
  }

  public static double GetLogNormal(double mu, double sigma)
  {
    return Math.Exp(SimpleRNG.GetNormal(mu, sigma));
  }

  public static double GetBeta(double a, double b)
  {
    double num = a > 0.0 && b > 0.0 ? SimpleRNG.GetGamma(a, 1.0) : throw new ArgumentOutOfRangeException($"Beta parameters must be positive. Received {a} and {b}.");
    double gamma = SimpleRNG.GetGamma(b, 1.0);
    return num / (num + gamma);
  }
}
