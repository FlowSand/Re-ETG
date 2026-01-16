// Decompiled with JetBrains decompiler
// Type: BraveMathCollege
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using TestSimpleRNG;
using UnityEngine;

#nullable disable
public static class BraveMathCollege
{
  private static float[] LowDiscrepancyPseudoRandoms = new float[20]
  {
    0.546f,
    0.153f,
    0.925f,
    0.471f,
    0.739f,
    0.062f,
    0.383f,
    0.817f,
    0.696f,
    0.205f,
    0.554f,
    0.847f,
    0.075f,
    0.639f,
    0.261f,
    0.938f,
    0.617f,
    0.183f,
    0.304f,
    0.795f
  };

  public static float GetLowDiscrepancyRandom(int iterator)
  {
    return BraveMathCollege.LowDiscrepancyPseudoRandoms[iterator % BraveMathCollege.LowDiscrepancyPseudoRandoms.Length];
  }

  private static float ANG_NoiseInternal(float freq)
  {
    float num = UnityEngine.Random.Range(0.0f, 6.28318548f);
    return Mathf.Sin(6.28318548f * freq + num);
  }

  private static float ANG_WeightedSumNoise(float[] amplitudes, float[] noises)
  {
    float num = 0.0f;
    for (int index = 0; index < noises.Length; ++index)
      num += amplitudes[index] * noises[index];
    return num;
  }

  private static float AdvancedNoiseGenerator(Func<float, float> amplitudeLambda)
  {
    float[] numArray = new float[30]
    {
      1f,
      2f,
      3f,
      4f,
      5f,
      6f,
      7f,
      8f,
      9f,
      10f,
      11f,
      12f,
      13f,
      14f,
      15f,
      16f,
      17f,
      18f,
      19f,
      20f,
      21f,
      22f,
      23f,
      24f,
      25f,
      26f,
      27f,
      28f,
      29f,
      30f
    };
    float[] amplitudes = new float[numArray.Length];
    for (int index = 0; index < numArray.Length; ++index)
      amplitudes[index] = amplitudeLambda(numArray[index] / (float) numArray.Length);
    float[] noises = new float[numArray.Length];
    for (int index = 0; index < numArray.Length; ++index)
      noises[index] = BraveMathCollege.ANG_NoiseInternal(numArray[index] / (float) numArray.Length);
    return BraveMathCollege.ANG_WeightedSumNoise(amplitudes, noises);
  }

  public static float GetRedNoise()
  {
    return BraveMathCollege.AdvancedNoiseGenerator((Func<float, float>) (f => 1f / f / f));
  }

  public static float GetPinkNoise()
  {
    return BraveMathCollege.AdvancedNoiseGenerator((Func<float, float>) (f => 1f / f));
  }

  public static float GetWhiteNoise()
  {
    return BraveMathCollege.AdvancedNoiseGenerator((Func<float, float>) (f => 1f));
  }

  public static float GetBlueNoise()
  {
    return BraveMathCollege.AdvancedNoiseGenerator((Func<float, float>) (f => f));
  }

  public static float GetVioletNoise()
  {
    return BraveMathCollege.AdvancedNoiseGenerator((Func<float, float>) (f => f * f));
  }

  public static float GetRandomByNormalDistribution(float mean, float stddev)
  {
    return (float) SimpleRNG.GetNormal((double) mean, (double) stddev);
  }

  public static float NormalDistributionAtPosition(float x, float mean, float stddev)
  {
    float oneOverTwoPi = 0.159154937f;
    float twoTimeStdDev = 2f * stddev;
    return BraveMathCollege.NormalDistributionAtPosition(x, oneOverTwoPi, mean, twoTimeStdDev);
  }

  public static float NormalDistributionAtPosition(
    float x,
    float oneOverTwoPi,
    float mean,
    float twoTimeStdDev)
  {
    return oneOverTwoPi * Mathf.Exp((float) (-((double) x - (double) mean) * ((double) x - (double) mean) / (2.0 * (double) twoTimeStdDev)));
  }

  public static float UnboundedLerp(float from, float to, float t) => (to - from) * t + from;

  public static float SmoothLerp(float from, float to, float t)
  {
    return Mathf.Lerp(from, to, Mathf.SmoothStep(0.0f, 1f, t));
  }

  public static float Bilerp(float x0y0, float x1y0, float x0y1, float x1y1, float u, float v)
  {
    return Mathf.Lerp(Mathf.Lerp(x0y0, x1y0, u), Mathf.Lerp(x0y1, x1y1, u), v);
  }

  public static float DoubleLerp(float from, float intermediary, float to, float t)
  {
    return (double) t < 0.5 ? Mathf.Lerp(from, intermediary, t * 2f) : Mathf.Lerp(intermediary, to, (float) ((double) t * 2.0 - 1.0));
  }

  public static Vector2 DoubleLerp(Vector2 from, Vector2 intermediary, Vector2 to, float t)
  {
    return (double) t < 0.5 ? Vector2.Lerp(from, intermediary, t * 2f) : Vector2.Lerp(intermediary, to, (float) ((double) t * 2.0 - 1.0));
  }

  public static Vector3 DoubleLerp(Vector3 from, Vector3 intermediary, Vector3 to, float t)
  {
    return (double) t < 0.5 ? Vector3.Lerp(from, intermediary, t * 2f) : Vector3.Lerp(intermediary, to, (float) ((double) t * 2.0 - 1.0));
  }

  public static float DoubleLerpSmooth(float from, float intermediary, float to, float t)
  {
    return (double) t < 0.5 ? Mathf.Lerp(from, intermediary, Mathf.SmoothStep(0.0f, 1f, t * 2f)) : Mathf.Lerp(intermediary, to, Mathf.SmoothStep(0.0f, 1f, (float) ((double) t * 2.0 - 1.0)));
  }

  public static Vector2 DoubleLerpSmooth(Vector2 from, Vector2 intermediary, Vector2 to, float t)
  {
    return (double) t < 0.5 ? Vector2.Lerp(from, intermediary, Mathf.SmoothStep(0.0f, 1f, t * 2f)) : Vector2.Lerp(intermediary, to, Mathf.SmoothStep(0.0f, 1f, (float) ((double) t * 2.0 - 1.0)));
  }

  public static Vector3 DoubleLerpSmooth(Vector3 from, Vector3 intermediary, Vector3 to, float t)
  {
    return (double) t < 0.5 ? Vector3.Lerp(from, intermediary, Mathf.SmoothStep(0.0f, 1f, t * 2f)) : Vector3.Lerp(intermediary, to, Mathf.SmoothStep(0.0f, 1f, (float) ((double) t * 2.0 - 1.0)));
  }

  public static Vector2 VectorToCone(Vector2 source, float angleVariance)
  {
    return (Quaternion.Euler(0.0f, 0.0f, UnityEngine.Random.Range(-angleVariance, angleVariance)) * source.ToVector3ZUp()).XY();
  }

  public static float ActualSign(float f)
  {
    if ((double) f < 0.0)
      return -1f;
    return (double) f > 0.0 ? 1f : 0.0f;
  }

  public static int AngleToQuadrant(float angle)
  {
    angle = (float) (((double) angle + 360.0) % 360.0);
    angle += 45f;
    angle %= 360f;
    return (3 - Mathf.FloorToInt(angle / 90f) + 2) % 4;
  }

  public static int VectorToQuadrant(Vector2 inVec)
  {
    return BraveMathCollege.AngleToQuadrant(inVec.ToAngle());
  }

  public static int VectorToOctant(Vector2 inVec)
  {
    return (7 - Mathf.FloorToInt(((float) (((double) (Mathf.Atan2(inVec.y, inVec.x) * 57.29578f) + 360.0) % 360.0) + 22.5f) % 360f / 45f) + 3) % 8;
  }

  public static int VectorToSextant(Vector2 inVec)
  {
    return (5 - Mathf.FloorToInt((float) (((double) (Mathf.Atan2(inVec.y, inVec.x) * 57.29578f) + 360.0) % 360.0) % 360f / 60f) + 2) % 6;
  }

  public static int GreatestCommonDivisor(int a, int b)
  {
    return b == 0 ? a : BraveMathCollege.GreatestCommonDivisor(b, a % b);
  }

  public static int AngleToOctant(float angle) => (int) ((472.5 - (double) angle) / 45.0) % 8;

  public static Vector2 ReflectVectorAcrossNormal(Vector2 vector, Vector2 normal)
  {
    return (Vector2) (Quaternion.Euler(0.0f, 0.0f, (float) (180.0 + 2.0 * (((double) Mathf.Atan2(normal.y, normal.x) - (double) Mathf.Atan2(vector.y, vector.x)) * 57.295780181884766))) * (Vector3) vector);
  }

  public static Vector2 CircleCenterFromThreePoints(Vector2 a, Vector2 b, Vector2 c)
  {
    float num1 = b.y - a.y;
    float num2 = b.x - a.x;
    float num3 = c.y - b.y;
    float num4 = c.x - b.x;
    float num5 = num1 / num2;
    float num6 = num3 / num4;
    float x = (float) (((double) num5 * (double) num6 * ((double) a.y - (double) c.y) + (double) num6 * ((double) a.x + (double) b.x) - (double) num5 * ((double) b.x + (double) c.x)) / (2.0 * ((double) num6 - (double) num5)));
    float y = (float) (-1.0 * ((double) x - ((double) a.x + (double) b.x) / 2.0) / (double) num5 + ((double) a.y + (double) b.y) / 2.0);
    return new Vector2(x, y);
  }

  public static float QuantizeFloat(float input, float multiplesOf)
  {
    return Mathf.Round(input / multiplesOf) * multiplesOf;
  }

  public static float LinearToSmoothStepInterpolate(float from, float to, float t, int iterations)
  {
    float t1 = t;
    for (int index = 0; index < iterations; ++index)
      t1 = BraveMathCollege.LinearToSmoothStepInterpolate(from, to, t1);
    return t1;
  }

  public static float LinearToSmoothStepInterpolate(float from, float to, float t)
  {
    return Mathf.Lerp(Mathf.Lerp(from, to, t), Mathf.SmoothStep(from, to, t), t);
  }

  public static float SmoothStepToLinearStepInterpolate(float from, float to, float t)
  {
    return Mathf.Lerp(Mathf.SmoothStep(from, to, t), Mathf.Lerp(from, to, t), t);
  }

  public static float HermiteInterpolation(float t)
  {
    return (float) (-(double) t * (double) t * (double) t * 2.0 + (double) t * (double) t * 3.0);
  }

  public static bool LineSegmentRectangleIntersection(
    Vector2 p0,
    Vector2 p1,
    Vector2 rectMin,
    Vector2 rectMax,
    ref Vector2 result)
  {
    Vector2 zero1 = Vector2.zero;
    Vector2 zero2 = Vector2.zero;
    Vector2 zero3 = Vector2.zero;
    Vector2 zero4 = Vector2.zero;
    bool flag1 = BraveMathCollege.LineSegmentIntersection(p0, p1, rectMin, rectMin.WithX(rectMax.x), ref zero1);
    bool flag2 = BraveMathCollege.LineSegmentIntersection(p0, p1, rectMin.WithX(rectMax.x), rectMax, ref zero2);
    bool flag3 = BraveMathCollege.LineSegmentIntersection(p0, p1, rectMax, rectMin.WithY(rectMax.y), ref zero3);
    bool flag4 = BraveMathCollege.LineSegmentIntersection(p0, p1, rectMin, rectMin.WithY(rectMax.y), ref zero4);
    float num = float.MaxValue;
    bool flag5 = false;
    result = Vector2.zero;
    if (flag1 && (double) Vector2.Distance(p0, zero1) < (double) num)
    {
      num = Vector2.Distance(p0, zero1);
      result = zero1;
      flag5 = true;
    }
    if (flag2 && (double) Vector2.Distance(p0, zero2) < (double) num)
    {
      num = Vector2.Distance(p0, zero2);
      result = zero2;
      flag5 = true;
    }
    if (flag3 && (double) Vector2.Distance(p0, zero3) < (double) num)
    {
      num = Vector2.Distance(p0, zero3);
      result = zero3;
      flag5 = true;
    }
    if (flag4 && (double) Vector2.Distance(p0, zero4) < (double) num)
    {
      Vector2.Distance(p0, zero4);
      result = zero4;
      flag5 = true;
    }
    return flag5;
  }

  public static bool LineSegmentIntersection(
    Vector2 p0,
    Vector2 p1,
    Vector2 q0,
    Vector2 q1,
    ref Vector2 result)
  {
    Vector2 zero1 = Vector2.zero;
    Vector2 zero2 = Vector2.zero;
    zero1.x = p1.x - p0.x;
    zero1.y = p1.y - p0.y;
    zero2.x = q1.x - q0.x;
    zero2.y = q1.y - q0.y;
    float num1 = (float) ((-(double) zero1.y * ((double) p0.x - (double) q0.x) + (double) zero1.x * ((double) p0.y - (double) q0.y)) / (-(double) zero2.x * (double) zero1.y + (double) zero1.x * (double) zero2.y));
    float num2 = (float) (((double) zero2.x * ((double) p0.y - (double) q0.y) - (double) zero2.y * ((double) p0.x - (double) q0.x)) / (-(double) zero2.x * (double) zero1.y + (double) zero1.x * (double) zero2.y));
    result = Vector2.zero;
    if ((double) num1 < 0.0 || (double) num1 > 1.0 || (double) num2 < 0.0 || (double) num2 > 1.0)
      return false;
    result.x = p0.x + num2 * zero1.x;
    result.y = p0.y + num2 * zero1.y;
    return true;
  }

  public static Vector2 ClosestPointOnLineSegment(Vector2 p, Vector2 v, Vector2 w)
  {
    float sqrMagnitude = (w - v).sqrMagnitude;
    if ((double) sqrMagnitude == 0.0)
      return v;
    float num = Vector2.Dot(p - v, w - v) / sqrMagnitude;
    if ((double) num < 0.0)
      return v;
    return (double) num > 1.0 ? w : v + num * (w - v);
  }

  public static Vector2 ClosestPointOnRectangle(Vector2 point, Vector2 origin, Vector2 dimensions)
  {
    Vector2 vector2_1 = origin;
    Vector2 vector2_2 = new Vector2(origin.x + dimensions.x, origin.y);
    Vector2 vector2_3 = origin + dimensions;
    Vector2 vector2_4 = new Vector2(origin.x, origin.y + dimensions.y);
    Vector2 b1 = BraveMathCollege.ClosestPointOnLineSegment(point, vector2_1, vector2_2);
    float num1 = Vector2.Distance(point, b1);
    Vector2 vector2_5 = b1;
    Vector2 b2 = BraveMathCollege.ClosestPointOnLineSegment(point, vector2_2, vector2_3);
    float num2 = Vector2.Distance(point, b2);
    if ((double) num2 < (double) num1)
    {
      num1 = num2;
      vector2_5 = b2;
    }
    Vector2 b3 = BraveMathCollege.ClosestPointOnLineSegment(point, vector2_3, vector2_4);
    float num3 = Vector2.Distance(point, b3);
    if ((double) num3 < (double) num1)
    {
      num1 = num3;
      vector2_5 = b3;
    }
    Vector2 b4 = BraveMathCollege.ClosestPointOnLineSegment(point, vector2_4, vector2_1);
    if ((double) Vector2.Distance(point, b4) < (double) num1)
      vector2_5 = b4;
    return vector2_5;
  }

  public static Vector2 ClosestPointOnPolygon(List<Vector2> polygon, Vector2 point)
  {
    Vector2 vector2 = Vector2.zero;
    float num1 = float.MaxValue;
    for (int index = 0; index < polygon.Count; ++index)
    {
      Vector2 v = polygon[index];
      Vector2 w = polygon[(index + 1) % polygon.Count];
      Vector2 b = BraveMathCollege.ClosestPointOnLineSegment(point, v, w);
      float num2 = Vector2.Distance(point, b);
      if ((double) num2 < (double) num1)
      {
        num1 = num2;
        vector2 = b;
      }
    }
    return vector2;
  }

  public static float DistToRectangle(Vector2 point, Vector2 origin, Vector2 dimensions)
  {
    Vector2 b = BraveMathCollege.ClosestPointOnRectangle(point, origin, dimensions);
    return Vector2.Distance(point, b);
  }

  public static float DistBetweenRectangles(Vector2 o1, Vector2 d1, Vector2 o2, Vector2 d2)
  {
    Vector2 vector2_1 = Vector2.Min(o1, o2);
    Vector2 vector2_2 = Vector2.Max(o1 + d1, o2 + d2) - vector2_1;
    float num1 = vector2_2.x - (d1.x + d2.x);
    float num2 = vector2_2.y - (d1.y + d2.y);
    if ((double) num1 > 0.0 && (double) num2 > 0.0)
      return Mathf.Sqrt((float) ((double) num1 * (double) num1 + (double) num2 * (double) num2));
    if ((double) num1 > 0.0)
      return num1;
    return (double) num2 > 0.0 ? num2 : 0.0f;
  }

  public static float ClampAngle360(float angleDeg)
  {
    angleDeg %= 360f;
    if ((double) angleDeg < 0.0)
      angleDeg += 360f;
    return angleDeg;
  }

  public static float ClampAngle180(float angleDeg)
  {
    angleDeg %= 360f;
    if ((double) angleDeg < -180.0)
      angleDeg += 360f;
    else if ((double) angleDeg > 180.0)
      angleDeg -= 360f;
    return angleDeg;
  }

  public static float ClampAngle2Pi(float angleRad)
  {
    angleRad %= 6.28318548f;
    if ((double) angleRad < 0.0)
      angleRad += 6.28318548f;
    return angleRad;
  }

  public static float ClampAnglePi(float angleRad)
  {
    angleRad %= 6.28318548f;
    if ((double) angleRad < -3.1415927410125732)
      angleRad += 6.28318548f;
    else if ((double) angleRad > 3.1415927410125732)
      angleRad -= 6.28318548f;
    return angleRad;
  }

  public static float Atan2Degrees(float y, float x) => Mathf.Atan2(y, x) * 57.29578f;

  public static float Atan2Degrees(Vector2 v) => Mathf.Atan2(v.y, v.x) * 57.29578f;

  public static float AbsAngleBetween(float a, float b)
  {
    return Mathf.Abs(BraveMathCollege.ClampAngle180(a - b));
  }

  public static Vector2 DegreesToVector(float angle, float magnitude = 1f)
  {
    float f = angle * ((float) Math.PI / 180f);
    return new Vector2(Mathf.Cos(f), Mathf.Sin(f)) * magnitude;
  }

  public static float GetNearestAngle(float angle, float[] options)
  {
    if (options == null || options.Length == 0)
      return angle;
    int index1 = 0;
    float num1 = BraveMathCollege.AbsAngleBetween(angle, options[0]);
    for (int index2 = 1; index2 < options.Length; ++index2)
    {
      float num2 = BraveMathCollege.AbsAngleBetween(angle, options[index2]);
      if ((double) num2 < (double) num1)
      {
        num1 = num2;
        index1 = index2;
      }
    }
    return options[index1];
  }

  public static float EstimateBezierPathLength(
    Vector2 p0,
    Vector2 p1,
    Vector2 p2,
    Vector2 p3,
    int divisions)
  {
    float num1 = 1f / (float) divisions;
    float num2 = 0.0f;
    for (int index = 0; index < divisions; ++index)
    {
      Vector2 bezierPoint1 = BraveMathCollege.CalculateBezierPoint(num1 * (float) index, p0, p1, p2, p3);
      Vector2 bezierPoint2 = BraveMathCollege.CalculateBezierPoint(num1 * (float) (index + 1), p0, p1, p2, p3);
      num2 += (bezierPoint2 - bezierPoint1).magnitude;
    }
    return num2;
  }

  public static Vector2 CalculateBezierPoint(
    float t,
    Vector2 p0,
    Vector2 p1,
    Vector2 p2,
    Vector2 p3)
  {
    float num1 = 1f - t;
    float num2 = t * t;
    float num3 = num1 * num1;
    float num4 = num3 * num1;
    float num5 = num2 * t;
    return num4 * p0 + 3f * num3 * t * p1 + 3f * num1 * num2 * p2 + num5 * p3;
  }

  public static int LineCircleIntersections(
    Vector2 center,
    float radius,
    Vector2 p1,
    Vector2 p2,
    out Vector2 i1,
    out Vector2 i2)
  {
    float num1 = p2.x - p1.x;
    float num2 = p2.y - p1.y;
    float num3 = (float) ((double) num1 * (double) num1 + (double) num2 * (double) num2);
    float num4 = (float) (2.0 * ((double) num1 * ((double) p1.x - (double) center.x) + (double) num2 * ((double) p1.y - (double) center.y)));
    float num5 = (float) (((double) p1.x - (double) center.x) * ((double) p1.x - (double) center.x) + ((double) p1.y - (double) center.y) * ((double) p1.y - (double) center.y) - (double) radius * (double) radius);
    float d = (float) ((double) num4 * (double) num4 - 4.0 * (double) num3 * (double) num5);
    if ((double) num3 <= 1.0000000116860974E-07 || (double) d < 0.0)
    {
      i1 = new Vector2(float.NaN, float.NaN);
      i2 = new Vector2(float.NaN, float.NaN);
      return 0;
    }
    if ((double) d == 0.0)
    {
      float num6 = (float) (-(double) num4 / (2.0 * (double) num3));
      i1 = new Vector2(p1.x + num6 * num1, p1.y + num6 * num2);
      i2 = new Vector2(float.NaN, float.NaN);
      return 1;
    }
    float num7 = (float) ((-(double) num4 + Math.Sqrt((double) d)) / (2.0 * (double) num3));
    i1 = new Vector2(p1.x + num7 * num1, p1.y + num7 * num2);
    float num8 = (float) ((-(double) num4 - Math.Sqrt((double) d)) / (2.0 * (double) num3));
    i2 = new Vector2(p1.x + num8 * num1, p1.y + num8 * num2);
    return 2;
  }

  public static int LineSegmentCircleIntersections(
    Vector2 center,
    float radius,
    Vector2 p1,
    Vector2 p2,
    out Vector2 i1,
    out Vector2 i2)
  {
    int num1 = BraveMathCollege.LineCircleIntersections(center, radius, p1, p2, out i1, out i2);
    if (num1 == 0)
    {
      i1 = new Vector2(float.NaN, float.NaN);
      i2 = new Vector2(float.NaN, float.NaN);
      return 0;
    }
    Vector2 vector2_1 = Vector2.Min(p1, p2);
    Vector2 vector2_2 = Vector2.Max(p1, p2);
    int num2 = 0;
    if (num1 >= 1)
    {
      if ((double) vector2_1.x <= (double) i1.x && (double) vector2_2.x >= (double) i1.x && (double) vector2_1.y <= (double) i1.y && (double) vector2_2.y >= (double) i1.y)
        ++num2;
      else
        i1 = new Vector2(float.NaN, float.NaN);
    }
    if (num1 >= 2)
    {
      if ((double) vector2_1.x <= (double) i2.x && (double) vector2_2.x >= (double) i2.x && (double) vector2_1.y <= (double) i2.y && (double) vector2_2.y >= (double) i2.y)
      {
        ++num2;
        if (num2 == 1)
        {
          i1 = i2;
          i2 = new Vector2(float.NaN, float.NaN);
        }
      }
      else
        i2 = new Vector2(float.NaN, float.NaN);
    }
    return num2;
  }

  public static Vector2 ClosestLineCircleIntersect(
    Vector2 center,
    float radius,
    Vector2 lineStart,
    Vector2 lineEnd)
  {
    Vector2 i1;
    Vector2 i2;
    switch (BraveMathCollege.LineCircleIntersections(center, radius, lineStart, lineEnd, out i1, out i2))
    {
      case 1:
        return i1;
      case 2:
        return (double) Vector2.Distance(i1, lineStart) < (double) Vector2.Distance(i2, lineStart) ? i1 : i2;
      default:
        return Vector2.zero;
    }
  }

  public static float SliceProbability(float chancePerSecond, float tickTime)
  {
    return 1f - Mathf.Pow(1f - chancePerSecond, tickTime);
  }

  public static bool AABBContains(Vector2 min, Vector2 max, Vector2 point)
  {
    return (double) point.x >= (double) min.x && (double) point.x <= (double) max.x && (double) point.y >= (double) min.y && (double) point.y <= (double) max.y;
  }

  public static float AABBDistance(Vector2 aMin, Vector2 aMax, Vector2 bMin, Vector2 bMax)
  {
    Vector2 a = new Vector2((float) (((double) bMin.x + (double) bMax.x) / 2.0), (float) (((double) bMin.y + (double) bMax.y) / 2.0));
    Vector2 b = new Vector2((float) (((double) aMin.x + (double) aMax.x) / 2.0), (float) (((double) aMin.y + (double) aMax.y) / 2.0));
    if ((double) a.x < (double) aMin.x)
      a.x = aMin.x;
    if ((double) a.x > (double) aMax.x)
      a.x = aMax.x;
    if ((double) a.y < (double) aMin.y)
      a.y = aMin.y;
    if ((double) a.y > (double) aMax.y)
      a.y = aMax.y;
    if ((double) b.x < (double) aMin.x)
      b.x = aMin.x;
    if ((double) b.x > (double) aMax.x)
      b.x = aMax.x;
    if ((double) b.y < (double) aMin.y)
      b.y = aMin.y;
    if ((double) b.y > (double) aMax.y)
      b.y = aMax.y;
    return Vector2.Distance(a, b);
  }

  public static float AABBDistanceSquared(Vector2 aMin, Vector2 aMax, Vector2 bMin, Vector2 bMax)
  {
    Vector2 vector2_1 = new Vector2((float) (((double) bMin.x + (double) bMax.x) / 2.0), (float) (((double) bMin.y + (double) bMax.y) / 2.0));
    Vector2 vector2_2 = new Vector2((float) (((double) aMin.x + (double) aMax.x) / 2.0), (float) (((double) aMin.y + (double) aMax.y) / 2.0));
    if ((double) vector2_1.x < (double) aMin.x)
      vector2_1.x = aMin.x;
    if ((double) vector2_1.x > (double) aMax.x)
      vector2_1.x = aMax.x;
    if ((double) vector2_1.y < (double) aMin.y)
      vector2_1.y = aMin.y;
    if ((double) vector2_1.y > (double) aMax.y)
      vector2_1.y = aMax.y;
    if ((double) vector2_2.x < (double) bMin.x)
      vector2_2.x = bMin.x;
    if ((double) vector2_2.x > (double) bMax.x)
      vector2_2.x = bMax.x;
    if ((double) vector2_2.y < (double) bMin.y)
      vector2_2.y = bMin.y;
    if ((double) vector2_2.y > (double) bMax.y)
      vector2_2.y = bMax.y;
    return Vector2.SqrMagnitude(vector2_1 - vector2_2);
  }

  public static Vector2 GetPredictedPosition(
    Vector2 targetOrigin,
    Vector2 targetVelocity,
    float time)
  {
    return targetOrigin + targetVelocity * time;
  }

  public static Vector2 GetPredictedPosition(
    Vector2 targetOrigin,
    Vector2 targetVelocity,
    Vector2 aimOrigin,
    float firingSpeed)
  {
    float magnitude = targetVelocity.magnitude;
    if ((double) magnitude < 9.9999997473787516E-06)
      return targetOrigin;
    Vector2 vector = aimOrigin - targetOrigin;
    float num1 = targetVelocity.ToAngle() - vector.ToAngle();
    float f = Mathf.Asin(magnitude * Mathf.Sin(num1 * ((float) Math.PI / 180f)) / firingSpeed) * 57.29578f;
    if (float.IsNaN(f))
      return targetOrigin;
    float num2 = BraveMathCollege.ClampAngle360((targetOrigin - aimOrigin).ToAngle());
    float num3 = BraveMathCollege.ClampAngle360(180f - f - num1);
    if ((double) num3 < 0.0001 || (double) num3 > 359.99990844726563)
      return targetOrigin;
    float num4 = Vector2.Distance(aimOrigin, targetOrigin) * Mathf.Sin(f * ((float) Math.PI / 180f)) / Mathf.Sin(num3 * ((float) Math.PI / 180f)) / magnitude;
    return (double) num4 < 0.0 ? targetOrigin : aimOrigin + BraveMathCollege.DegreesToVector(num2 - f, firingSpeed * num4);
  }

  public static bool NextPermutation(ref int[] numList)
  {
    int index1 = -1;
    for (int index2 = numList.Length - 2; index2 >= 0; --index2)
    {
      if (numList[index2] < numList[index2 + 1])
      {
        index1 = index2;
        break;
      }
    }
    if (index1 < 0)
      return false;
    int index3 = -1;
    for (int index4 = numList.Length - 1; index4 >= 0; --index4)
    {
      if (numList[index1] < numList[index4])
      {
        index3 = index4;
        break;
      }
    }
    int num1 = numList[index1];
    numList[index1] = numList[index3];
    numList[index3] = num1;
    int index5 = index1 + 1;
    for (int index6 = numList.Length - 1; index5 < index6; --index6)
    {
      int num2 = numList[index5];
      numList[index5] = numList[index6];
      numList[index6] = num2;
      ++index5;
    }
    return true;
  }

  public static Vector2 ClampToBounds(Vector2 value, Vector2 min, Vector2 max)
  {
    return new Vector2(Mathf.Clamp(value.x, min.x, max.x), Mathf.Clamp(value.y, min.y, max.y));
  }

  public static Vector2 ClampSafe(Vector2 value, float min, float max)
  {
    return new Vector2(BraveMathCollege.ClampSafe(value.x, min, max), BraveMathCollege.ClampSafe(value.y, min, max));
  }

  public static float ClampSafe(float value, float min, float max)
  {
    return float.IsNaN(value) ? 0.0f : Mathf.Clamp(value, min, max);
  }

  public static float WeightedAverage(float newValue, ref float prevAverage, ref int prevCount)
  {
    ++prevCount;
    prevAverage = (float) ((double) prevAverage * (((double) prevCount - 1.0) / (double) prevCount) + (double) newValue / (double) prevCount);
    return prevAverage;
  }

  public static Vector2 WeightedAverage(
    Vector2 newValue,
    ref Vector2 prevAverage,
    ref int prevCount)
  {
    ++prevCount;
    prevAverage = prevAverage * (((float) prevCount - 1f) / (float) prevCount) + newValue / (float) prevCount;
    return prevAverage;
  }

  public static float MovingAverage(float avg, float newValue, int n)
  {
    if ((double) avg == 0.0)
      return newValue;
    float num = 1f / (float) n;
    return avg + num * (newValue - avg);
  }

  public static Vector2 MovingAverage(Vector2 avg, Vector2 newValue, int n)
  {
    if (avg == Vector2.zero)
      return newValue;
    float num = 1f / (float) n;
    return avg + num * (newValue - avg);
  }

  public static float MovingAverageSpeed(
    float movingAverage,
    float newSpeed,
    float newDeltaTime,
    float n)
  {
    if ((double) newDeltaTime <= 0.0)
      return movingAverage;
    if ((double) movingAverage == 0.0 || (double) newDeltaTime >= (double) n)
      return newSpeed;
    float num = newDeltaTime / n;
    return movingAverage + num * (newSpeed - movingAverage);
  }

  public static Vector3 LShapedMoveTowards(
    Vector3 current,
    Vector3 target,
    float maxDeltaX,
    float maxDeltaY)
  {
    return Mathf.RoundToInt(current.x) != Mathf.RoundToInt(target.x) && Mathf.RoundToInt(current.y) != Mathf.RoundToInt(target.y) ? ((double) target.y > (double) current.y ? Vector3.MoveTowards(current, target.WithX(current.x), maxDeltaX) : Vector3.MoveTowards(current, target.WithY(current.y), maxDeltaY)) : (Mathf.RoundToInt(current.y) == Mathf.RoundToInt(target.y) ? Vector3.MoveTowards(current, target, maxDeltaX) : Vector3.MoveTowards(current, target, maxDeltaY));
  }

  public static bool IsAngleWithinSweepArea(float testAngle, float startAngle, float sweepAngle)
  {
    if ((double) sweepAngle > 360.0 || (double) sweepAngle < -360.0)
      return true;
    float num = Mathf.Sign(sweepAngle);
    float f = BraveMathCollege.ClampAngle180(testAngle - startAngle);
    if ((double) Mathf.Sign(f) != (double) num)
      f += num * 360f;
    return (double) num > 0.0 ? (double) f < (double) sweepAngle : (double) f > (double) sweepAngle;
  }

  public static Vector2 GetEllipsePoint(Vector2 center, float a, float b, float angle)
  {
    Vector2 ellipsePoint = center;
    float num1 = BraveMathCollege.ClampAngle360(angle);
    float num2 = Mathf.Tan(num1 * ((float) Math.PI / 180f));
    float num3 = (double) num1 < 90.0 || (double) num1 >= 270.0 ? 1f : -1f;
    float num4 = Mathf.Sqrt((float) ((double) b * (double) b + (double) a * (double) a * ((double) num2 * (double) num2)));
    ellipsePoint.x += num3 * a * b / num4;
    ellipsePoint.y += num3 * a * b * num2 / num4;
    return ellipsePoint;
  }

  public static Vector2 GetEllipsePointSmooth(Vector2 center, float a, float b, float angle)
  {
    return center + Vector2.Scale(new Vector2(a, b), BraveMathCollege.DegreesToVector(angle));
  }
}
