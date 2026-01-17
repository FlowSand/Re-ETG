// Decompiled with JetBrains decompiler
// Type: DaikonForge.Tween.TweenEasingFunctions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace DaikonForge.Tween;

public class TweenEasingFunctions
{
  public static float Linear(float t) => t;

  public static float Spring(float t)
  {
    float num = Mathf.Clamp01(t);
    return (float) (((double) Mathf.Sin((float) ((double) num * 3.1415927410125732 * (0.20000000298023224 + 2.5 * (double) num * (double) num * (double) num))) * (double) Mathf.Pow(1f - num, 2.2f) + (double) num) * (1.0 + 1.2000000476837158 * (1.0 - (double) num)));
  }

  public static float EaseInQuad(float t) => t * t;

  public static float EaseOutQuad(float t) => (float) (-1.0 * (double) t * ((double) t - 2.0));

  public static float EaseInOutQuad(float t)
  {
    t /= 0.5f;
    if ((double) t < 1.0)
      return 0.5f * t * t;
    --t;
    return (float) (-0.5 * ((double) t * ((double) t - 2.0) - 1.0));
  }

  public static float EaseInCubic(float t) => t * t * t;

  public static float EaseOutCubic(float t)
  {
    --t;
    return (float) ((double) t * (double) t * (double) t + 1.0);
  }

  public static float EaseInOutCubic(float t)
  {
    t /= 0.5f;
    if ((double) t < 1.0)
      return 0.5f * t * t * t;
    t -= 2f;
    return (float) (0.5 * ((double) t * (double) t * (double) t + 2.0));
  }

  public static float EaseInQuart(float t) => t * t * t * t;

  public static float EaseOutQuart(float t)
  {
    --t;
    return (float) (-1.0 * ((double) t * (double) t * (double) t * (double) t - 1.0));
  }

  public static float EaseInOutQuart(float t)
  {
    t /= 0.5f;
    if ((double) t < 1.0)
      return 0.5f * t * t * t * t;
    t -= 2f;
    return (float) (-0.5 * ((double) t * (double) t * (double) t * (double) t - 2.0));
  }

  public static float EaseInQuint(float t) => t * t * t * t * t;

  public static float EaseOutQuint(float t)
  {
    --t;
    return (float) ((double) t * (double) t * (double) t * (double) t * (double) t + 1.0);
  }

  public static float EaseInOutQuint(float t)
  {
    t /= 0.5f;
    if ((double) t < 1.0)
      return 0.5f * t * t * t * t * t;
    t -= 2f;
    return (float) (0.5 * ((double) t * (double) t * (double) t * (double) t * (double) t + 2.0));
  }

  public static float EaseInSine(float t)
  {
    return (float) (-1.0 * (double) Mathf.Cos((float) ((double) t / 1.0 * 1.5707963705062866)) + 1.0);
  }

  public static float EaseOutSine(float t)
  {
    return Mathf.Sin((float) ((double) t / 1.0 * 1.5707963705062866));
  }

  public static float EaseInOutSine(float t)
  {
    return (float) (-0.5 * ((double) Mathf.Cos((float) (3.1415927410125732 * (double) t / 1.0)) - 1.0));
  }

  public static float EaseInExpo(float t)
  {
    return Mathf.Pow(2f, (float) (10.0 * ((double) t / 1.0 - 1.0)));
  }

  public static float EaseOutExpo(float t)
  {
    return (float) (-(double) Mathf.Pow(2f, (float) (-10.0 * (double) t / 1.0)) + 1.0);
  }

  public static float EaseInOutExpo(float t)
  {
    t /= 0.5f;
    if ((double) t < 1.0)
      return 0.5f * Mathf.Pow(2f, (float) (10.0 * ((double) t - 1.0)));
    --t;
    return (float) (0.5 * (-(double) Mathf.Pow(2f, -10f * t) + 2.0));
  }

  public static float EaseInCirc(float t)
  {
    return (float) (-1.0 * ((double) Mathf.Sqrt((float) (1.0 - (double) t * (double) t)) - 1.0));
  }

  public static float EaseOutCirc(float t)
  {
    --t;
    return Mathf.Sqrt((float) (1.0 - (double) t * (double) t));
  }

  public static float EaseInOutCirc(float t)
  {
    t /= 0.5f;
    if ((double) t < 1.0)
      return (float) (-0.5 * ((double) Mathf.Sqrt((float) (1.0 - (double) t * (double) t)) - 1.0));
    t -= 2f;
    return (float) (0.5 * ((double) Mathf.Sqrt((float) (1.0 - (double) t * (double) t)) + 1.0));
  }

  public static float EaseInBack(float t)
  {
    t /= 1f;
    float num = 1.70158f;
    return (float) ((double) t * (double) t * (((double) num + 1.0) * (double) t - (double) num));
  }

  public static float EaseOutBack(float t)
  {
    float num = 1.70158f;
    t = (float) ((double) t / 1.0 - 1.0);
    return (float) ((double) t * (double) t * (((double) num + 1.0) * (double) t + (double) num) + 1.0);
  }

  public static float EaseInOutBack(float t)
  {
    float num1 = 1.70158f;
    t /= 0.5f;
    if ((double) t < 1.0)
    {
      float num2 = num1 * 1.525f;
      return (float) (0.5 * ((double) t * (double) t * (((double) num2 + 1.0) * (double) t - (double) num2)));
    }
    t -= 2f;
    float num3 = num1 * 1.525f;
    return (float) (0.5 * ((double) t * (double) t * (((double) num3 + 1.0) * (double) t + (double) num3) + 2.0));
  }

  public static float Bounce(float t)
  {
    t /= 1f;
    if ((double) t < 0.36363637447357178)
      return 121f / 16f * t * t;
    if ((double) t < 0.72727274894714355)
    {
      t -= 0.545454562f;
      return (float) (121.0 / 16.0 * (double) t * (double) t + 0.75);
    }
    if ((double) t < 10.0 / 11.0)
    {
      t -= 0.8181818f;
      return (float) (121.0 / 16.0 * (double) t * (double) t + 15.0 / 16.0);
    }
    t -= 0.954545438f;
    return (float) (121.0 / 16.0 * (double) t * (double) t + 63.0 / 64.0);
  }

  public static float Punch(float t)
  {
    if ((double) t == 0.0 || (double) t == 1.0)
      return 0.0f;
    float num1 = 0.3f;
    float num2 = num1 / 6.28318548f * Mathf.Asin(0.0f);
    return Mathf.Pow(2f, -10f * t) * Mathf.Sin((float) (((double) t * 1.0 - (double) num2) * 6.2831854820251465) / num1);
  }

  public static TweenEasingCallback GetFunction(EasingType easeType)
  {
    switch (easeType)
    {
      case EasingType.Linear:
        // ISSUE: reference to a compiler-generated field
        if (TweenEasingFunctions.<>f__mg$cacheD == null)
        {
          // ISSUE: reference to a compiler-generated field
          TweenEasingFunctions.<>f__mg$cacheD = new TweenEasingCallback(TweenEasingFunctions.Linear);
        }
        // ISSUE: reference to a compiler-generated field
        return TweenEasingFunctions.<>f__mg$cacheD;
      case EasingType.Bounce:
        // ISSUE: reference to a compiler-generated field
        if (TweenEasingFunctions.<>f__mg$cache3 == null)
        {
          // ISSUE: reference to a compiler-generated field
          TweenEasingFunctions.<>f__mg$cache3 = new TweenEasingCallback(TweenEasingFunctions.Bounce);
        }
        // ISSUE: reference to a compiler-generated field
        return TweenEasingFunctions.<>f__mg$cache3;
      case EasingType.BackEaseIn:
        // ISSUE: reference to a compiler-generated field
        if (TweenEasingFunctions.<>f__mg$cache0 == null)
        {
          // ISSUE: reference to a compiler-generated field
          TweenEasingFunctions.<>f__mg$cache0 = new TweenEasingCallback(TweenEasingFunctions.EaseInBack);
        }
        // ISSUE: reference to a compiler-generated field
        return TweenEasingFunctions.<>f__mg$cache0;
      case EasingType.BackEaseOut:
        // ISSUE: reference to a compiler-generated field
        if (TweenEasingFunctions.<>f__mg$cache2 == null)
        {
          // ISSUE: reference to a compiler-generated field
          TweenEasingFunctions.<>f__mg$cache2 = new TweenEasingCallback(TweenEasingFunctions.EaseOutBack);
        }
        // ISSUE: reference to a compiler-generated field
        return TweenEasingFunctions.<>f__mg$cache2;
      case EasingType.BackEaseInOut:
        // ISSUE: reference to a compiler-generated field
        if (TweenEasingFunctions.<>f__mg$cache1 == null)
        {
          // ISSUE: reference to a compiler-generated field
          TweenEasingFunctions.<>f__mg$cache1 = new TweenEasingCallback(TweenEasingFunctions.EaseInOutBack);
        }
        // ISSUE: reference to a compiler-generated field
        return TweenEasingFunctions.<>f__mg$cache1;
      case EasingType.CircEaseIn:
        // ISSUE: reference to a compiler-generated field
        if (TweenEasingFunctions.<>f__mg$cache4 == null)
        {
          // ISSUE: reference to a compiler-generated field
          TweenEasingFunctions.<>f__mg$cache4 = new TweenEasingCallback(TweenEasingFunctions.EaseInCirc);
        }
        // ISSUE: reference to a compiler-generated field
        return TweenEasingFunctions.<>f__mg$cache4;
      case EasingType.CircEaseOut:
        // ISSUE: reference to a compiler-generated field
        if (TweenEasingFunctions.<>f__mg$cache6 == null)
        {
          // ISSUE: reference to a compiler-generated field
          TweenEasingFunctions.<>f__mg$cache6 = new TweenEasingCallback(TweenEasingFunctions.EaseOutCirc);
        }
        // ISSUE: reference to a compiler-generated field
        return TweenEasingFunctions.<>f__mg$cache6;
      case EasingType.CircEaseInOut:
        // ISSUE: reference to a compiler-generated field
        if (TweenEasingFunctions.<>f__mg$cache5 == null)
        {
          // ISSUE: reference to a compiler-generated field
          TweenEasingFunctions.<>f__mg$cache5 = new TweenEasingCallback(TweenEasingFunctions.EaseInOutCirc);
        }
        // ISSUE: reference to a compiler-generated field
        return TweenEasingFunctions.<>f__mg$cache5;
      case EasingType.CubicEaseIn:
        // ISSUE: reference to a compiler-generated field
        if (TweenEasingFunctions.<>f__mg$cache7 == null)
        {
          // ISSUE: reference to a compiler-generated field
          TweenEasingFunctions.<>f__mg$cache7 = new TweenEasingCallback(TweenEasingFunctions.EaseInCubic);
        }
        // ISSUE: reference to a compiler-generated field
        return TweenEasingFunctions.<>f__mg$cache7;
      case EasingType.CubicEaseOut:
        // ISSUE: reference to a compiler-generated field
        if (TweenEasingFunctions.<>f__mg$cache9 == null)
        {
          // ISSUE: reference to a compiler-generated field
          TweenEasingFunctions.<>f__mg$cache9 = new TweenEasingCallback(TweenEasingFunctions.EaseOutCubic);
        }
        // ISSUE: reference to a compiler-generated field
        return TweenEasingFunctions.<>f__mg$cache9;
      case EasingType.CubicEaseInOut:
        // ISSUE: reference to a compiler-generated field
        if (TweenEasingFunctions.<>f__mg$cache8 == null)
        {
          // ISSUE: reference to a compiler-generated field
          TweenEasingFunctions.<>f__mg$cache8 = new TweenEasingCallback(TweenEasingFunctions.EaseInOutCubic);
        }
        // ISSUE: reference to a compiler-generated field
        return TweenEasingFunctions.<>f__mg$cache8;
      case EasingType.ExpoEaseIn:
        // ISSUE: reference to a compiler-generated field
        if (TweenEasingFunctions.<>f__mg$cacheA == null)
        {
          // ISSUE: reference to a compiler-generated field
          TweenEasingFunctions.<>f__mg$cacheA = new TweenEasingCallback(TweenEasingFunctions.EaseInExpo);
        }
        // ISSUE: reference to a compiler-generated field
        return TweenEasingFunctions.<>f__mg$cacheA;
      case EasingType.ExpoEaseOut:
        // ISSUE: reference to a compiler-generated field
        if (TweenEasingFunctions.<>f__mg$cacheC == null)
        {
          // ISSUE: reference to a compiler-generated field
          TweenEasingFunctions.<>f__mg$cacheC = new TweenEasingCallback(TweenEasingFunctions.EaseOutExpo);
        }
        // ISSUE: reference to a compiler-generated field
        return TweenEasingFunctions.<>f__mg$cacheC;
      case EasingType.ExpoEaseInOut:
        // ISSUE: reference to a compiler-generated field
        if (TweenEasingFunctions.<>f__mg$cacheB == null)
        {
          // ISSUE: reference to a compiler-generated field
          TweenEasingFunctions.<>f__mg$cacheB = new TweenEasingCallback(TweenEasingFunctions.EaseInOutExpo);
        }
        // ISSUE: reference to a compiler-generated field
        return TweenEasingFunctions.<>f__mg$cacheB;
      case EasingType.QuadEaseIn:
        // ISSUE: reference to a compiler-generated field
        if (TweenEasingFunctions.<>f__mg$cacheE == null)
        {
          // ISSUE: reference to a compiler-generated field
          TweenEasingFunctions.<>f__mg$cacheE = new TweenEasingCallback(TweenEasingFunctions.EaseInQuad);
        }
        // ISSUE: reference to a compiler-generated field
        return TweenEasingFunctions.<>f__mg$cacheE;
      case EasingType.QuadEaseOut:
        // ISSUE: reference to a compiler-generated field
        if (TweenEasingFunctions.<>f__mg$cache10 == null)
        {
          // ISSUE: reference to a compiler-generated field
          TweenEasingFunctions.<>f__mg$cache10 = new TweenEasingCallback(TweenEasingFunctions.EaseOutQuad);
        }
        // ISSUE: reference to a compiler-generated field
        return TweenEasingFunctions.<>f__mg$cache10;
      case EasingType.QuadEaseInOut:
        // ISSUE: reference to a compiler-generated field
        if (TweenEasingFunctions.<>f__mg$cacheF == null)
        {
          // ISSUE: reference to a compiler-generated field
          TweenEasingFunctions.<>f__mg$cacheF = new TweenEasingCallback(TweenEasingFunctions.EaseInOutQuad);
        }
        // ISSUE: reference to a compiler-generated field
        return TweenEasingFunctions.<>f__mg$cacheF;
      case EasingType.QuartEaseIn:
        // ISSUE: reference to a compiler-generated field
        if (TweenEasingFunctions.<>f__mg$cache11 == null)
        {
          // ISSUE: reference to a compiler-generated field
          TweenEasingFunctions.<>f__mg$cache11 = new TweenEasingCallback(TweenEasingFunctions.EaseInQuart);
        }
        // ISSUE: reference to a compiler-generated field
        return TweenEasingFunctions.<>f__mg$cache11;
      case EasingType.QuartEaseOut:
        // ISSUE: reference to a compiler-generated field
        if (TweenEasingFunctions.<>f__mg$cache13 == null)
        {
          // ISSUE: reference to a compiler-generated field
          TweenEasingFunctions.<>f__mg$cache13 = new TweenEasingCallback(TweenEasingFunctions.EaseOutQuart);
        }
        // ISSUE: reference to a compiler-generated field
        return TweenEasingFunctions.<>f__mg$cache13;
      case EasingType.QuartEaseInOut:
        // ISSUE: reference to a compiler-generated field
        if (TweenEasingFunctions.<>f__mg$cache12 == null)
        {
          // ISSUE: reference to a compiler-generated field
          TweenEasingFunctions.<>f__mg$cache12 = new TweenEasingCallback(TweenEasingFunctions.EaseInOutQuart);
        }
        // ISSUE: reference to a compiler-generated field
        return TweenEasingFunctions.<>f__mg$cache12;
      case EasingType.QuintEaseIn:
        // ISSUE: reference to a compiler-generated field
        if (TweenEasingFunctions.<>f__mg$cache14 == null)
        {
          // ISSUE: reference to a compiler-generated field
          TweenEasingFunctions.<>f__mg$cache14 = new TweenEasingCallback(TweenEasingFunctions.EaseInQuint);
        }
        // ISSUE: reference to a compiler-generated field
        return TweenEasingFunctions.<>f__mg$cache14;
      case EasingType.QuintEaseOut:
        // ISSUE: reference to a compiler-generated field
        if (TweenEasingFunctions.<>f__mg$cache16 == null)
        {
          // ISSUE: reference to a compiler-generated field
          TweenEasingFunctions.<>f__mg$cache16 = new TweenEasingCallback(TweenEasingFunctions.EaseOutQuint);
        }
        // ISSUE: reference to a compiler-generated field
        return TweenEasingFunctions.<>f__mg$cache16;
      case EasingType.QuintEaseInOut:
        // ISSUE: reference to a compiler-generated field
        if (TweenEasingFunctions.<>f__mg$cache15 == null)
        {
          // ISSUE: reference to a compiler-generated field
          TweenEasingFunctions.<>f__mg$cache15 = new TweenEasingCallback(TweenEasingFunctions.EaseInOutQuint);
        }
        // ISSUE: reference to a compiler-generated field
        return TweenEasingFunctions.<>f__mg$cache15;
      case EasingType.SineEaseIn:
        // ISSUE: reference to a compiler-generated field
        if (TweenEasingFunctions.<>f__mg$cache17 == null)
        {
          // ISSUE: reference to a compiler-generated field
          TweenEasingFunctions.<>f__mg$cache17 = new TweenEasingCallback(TweenEasingFunctions.EaseInSine);
        }
        // ISSUE: reference to a compiler-generated field
        return TweenEasingFunctions.<>f__mg$cache17;
      case EasingType.SineEaseOut:
        // ISSUE: reference to a compiler-generated field
        if (TweenEasingFunctions.<>f__mg$cache19 == null)
        {
          // ISSUE: reference to a compiler-generated field
          TweenEasingFunctions.<>f__mg$cache19 = new TweenEasingCallback(TweenEasingFunctions.EaseOutSine);
        }
        // ISSUE: reference to a compiler-generated field
        return TweenEasingFunctions.<>f__mg$cache19;
      case EasingType.SineEaseInOut:
        // ISSUE: reference to a compiler-generated field
        if (TweenEasingFunctions.<>f__mg$cache18 == null)
        {
          // ISSUE: reference to a compiler-generated field
          TweenEasingFunctions.<>f__mg$cache18 = new TweenEasingCallback(TweenEasingFunctions.EaseInOutSine);
        }
        // ISSUE: reference to a compiler-generated field
        return TweenEasingFunctions.<>f__mg$cache18;
      case EasingType.Spring:
        // ISSUE: reference to a compiler-generated field
        if (TweenEasingFunctions.<>f__mg$cache1A == null)
        {
          // ISSUE: reference to a compiler-generated field
          TweenEasingFunctions.<>f__mg$cache1A = new TweenEasingCallback(TweenEasingFunctions.Spring);
        }
        // ISSUE: reference to a compiler-generated field
        return TweenEasingFunctions.<>f__mg$cache1A;
      default:
        throw new NotImplementedException();
    }
  }
}
