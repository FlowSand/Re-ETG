// Decompiled with JetBrains decompiler
// Type: dfEasingFunctions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class dfEasingFunctions
    {
      public static dfEasingFunctions.EasingFunction GetFunction(dfEasingType easeType)
      {
        switch (easeType)
        {
          case dfEasingType.Linear:
            // ISSUE: reference to a compiler-generated field
            if (dfEasingFunctions.\u003C\u003Ef__mg\u0024cacheD == null)
            {
              // ISSUE: reference to a compiler-generated field
              dfEasingFunctions.\u003C\u003Ef__mg\u0024cacheD = new dfEasingFunctions.EasingFunction(dfEasingFunctions.linear);
            }
            // ISSUE: reference to a compiler-generated field
            return dfEasingFunctions.\u003C\u003Ef__mg\u0024cacheD;
          case dfEasingType.Bounce:
            // ISSUE: reference to a compiler-generated field
            if (dfEasingFunctions.\u003C\u003Ef__mg\u0024cache3 == null)
            {
              // ISSUE: reference to a compiler-generated field
              dfEasingFunctions.\u003C\u003Ef__mg\u0024cache3 = new dfEasingFunctions.EasingFunction(dfEasingFunctions.bounce);
            }
            // ISSUE: reference to a compiler-generated field
            return dfEasingFunctions.\u003C\u003Ef__mg\u0024cache3;
          case dfEasingType.BackEaseIn:
            // ISSUE: reference to a compiler-generated field
            if (dfEasingFunctions.\u003C\u003Ef__mg\u0024cache0 == null)
            {
              // ISSUE: reference to a compiler-generated field
              dfEasingFunctions.\u003C\u003Ef__mg\u0024cache0 = new dfEasingFunctions.EasingFunction(dfEasingFunctions.easeInBack);
            }
            // ISSUE: reference to a compiler-generated field
            return dfEasingFunctions.\u003C\u003Ef__mg\u0024cache0;
          case dfEasingType.BackEaseOut:
            // ISSUE: reference to a compiler-generated field
            if (dfEasingFunctions.\u003C\u003Ef__mg\u0024cache2 == null)
            {
              // ISSUE: reference to a compiler-generated field
              dfEasingFunctions.\u003C\u003Ef__mg\u0024cache2 = new dfEasingFunctions.EasingFunction(dfEasingFunctions.easeOutBack);
            }
            // ISSUE: reference to a compiler-generated field
            return dfEasingFunctions.\u003C\u003Ef__mg\u0024cache2;
          case dfEasingType.BackEaseInOut:
            // ISSUE: reference to a compiler-generated field
            if (dfEasingFunctions.\u003C\u003Ef__mg\u0024cache1 == null)
            {
              // ISSUE: reference to a compiler-generated field
              dfEasingFunctions.\u003C\u003Ef__mg\u0024cache1 = new dfEasingFunctions.EasingFunction(dfEasingFunctions.easeInOutBack);
            }
            // ISSUE: reference to a compiler-generated field
            return dfEasingFunctions.\u003C\u003Ef__mg\u0024cache1;
          case dfEasingType.CircEaseIn:
            // ISSUE: reference to a compiler-generated field
            if (dfEasingFunctions.\u003C\u003Ef__mg\u0024cache4 == null)
            {
              // ISSUE: reference to a compiler-generated field
              dfEasingFunctions.\u003C\u003Ef__mg\u0024cache4 = new dfEasingFunctions.EasingFunction(dfEasingFunctions.easeInCirc);
            }
            // ISSUE: reference to a compiler-generated field
            return dfEasingFunctions.\u003C\u003Ef__mg\u0024cache4;
          case dfEasingType.CircEaseOut:
            // ISSUE: reference to a compiler-generated field
            if (dfEasingFunctions.\u003C\u003Ef__mg\u0024cache6 == null)
            {
              // ISSUE: reference to a compiler-generated field
              dfEasingFunctions.\u003C\u003Ef__mg\u0024cache6 = new dfEasingFunctions.EasingFunction(dfEasingFunctions.easeOutCirc);
            }
            // ISSUE: reference to a compiler-generated field
            return dfEasingFunctions.\u003C\u003Ef__mg\u0024cache6;
          case dfEasingType.CircEaseInOut:
            // ISSUE: reference to a compiler-generated field
            if (dfEasingFunctions.\u003C\u003Ef__mg\u0024cache5 == null)
            {
              // ISSUE: reference to a compiler-generated field
              dfEasingFunctions.\u003C\u003Ef__mg\u0024cache5 = new dfEasingFunctions.EasingFunction(dfEasingFunctions.easeInOutCirc);
            }
            // ISSUE: reference to a compiler-generated field
            return dfEasingFunctions.\u003C\u003Ef__mg\u0024cache5;
          case dfEasingType.CubicEaseIn:
            // ISSUE: reference to a compiler-generated field
            if (dfEasingFunctions.\u003C\u003Ef__mg\u0024cache7 == null)
            {
              // ISSUE: reference to a compiler-generated field
              dfEasingFunctions.\u003C\u003Ef__mg\u0024cache7 = new dfEasingFunctions.EasingFunction(dfEasingFunctions.easeInCubic);
            }
            // ISSUE: reference to a compiler-generated field
            return dfEasingFunctions.\u003C\u003Ef__mg\u0024cache7;
          case dfEasingType.CubicEaseOut:
            // ISSUE: reference to a compiler-generated field
            if (dfEasingFunctions.\u003C\u003Ef__mg\u0024cache9 == null)
            {
              // ISSUE: reference to a compiler-generated field
              dfEasingFunctions.\u003C\u003Ef__mg\u0024cache9 = new dfEasingFunctions.EasingFunction(dfEasingFunctions.easeOutCubic);
            }
            // ISSUE: reference to a compiler-generated field
            return dfEasingFunctions.\u003C\u003Ef__mg\u0024cache9;
          case dfEasingType.CubicEaseInOut:
            // ISSUE: reference to a compiler-generated field
            if (dfEasingFunctions.\u003C\u003Ef__mg\u0024cache8 == null)
            {
              // ISSUE: reference to a compiler-generated field
              dfEasingFunctions.\u003C\u003Ef__mg\u0024cache8 = new dfEasingFunctions.EasingFunction(dfEasingFunctions.easeInOutCubic);
            }
            // ISSUE: reference to a compiler-generated field
            return dfEasingFunctions.\u003C\u003Ef__mg\u0024cache8;
          case dfEasingType.ExpoEaseIn:
            // ISSUE: reference to a compiler-generated field
            if (dfEasingFunctions.\u003C\u003Ef__mg\u0024cacheA == null)
            {
              // ISSUE: reference to a compiler-generated field
              dfEasingFunctions.\u003C\u003Ef__mg\u0024cacheA = new dfEasingFunctions.EasingFunction(dfEasingFunctions.easeInExpo);
            }
            // ISSUE: reference to a compiler-generated field
            return dfEasingFunctions.\u003C\u003Ef__mg\u0024cacheA;
          case dfEasingType.ExpoEaseOut:
            // ISSUE: reference to a compiler-generated field
            if (dfEasingFunctions.\u003C\u003Ef__mg\u0024cacheC == null)
            {
              // ISSUE: reference to a compiler-generated field
              dfEasingFunctions.\u003C\u003Ef__mg\u0024cacheC = new dfEasingFunctions.EasingFunction(dfEasingFunctions.easeOutExpo);
            }
            // ISSUE: reference to a compiler-generated field
            return dfEasingFunctions.\u003C\u003Ef__mg\u0024cacheC;
          case dfEasingType.ExpoEaseInOut:
            // ISSUE: reference to a compiler-generated field
            if (dfEasingFunctions.\u003C\u003Ef__mg\u0024cacheB == null)
            {
              // ISSUE: reference to a compiler-generated field
              dfEasingFunctions.\u003C\u003Ef__mg\u0024cacheB = new dfEasingFunctions.EasingFunction(dfEasingFunctions.easeInOutExpo);
            }
            // ISSUE: reference to a compiler-generated field
            return dfEasingFunctions.\u003C\u003Ef__mg\u0024cacheB;
          case dfEasingType.QuadEaseIn:
            // ISSUE: reference to a compiler-generated field
            if (dfEasingFunctions.\u003C\u003Ef__mg\u0024cacheE == null)
            {
              // ISSUE: reference to a compiler-generated field
              dfEasingFunctions.\u003C\u003Ef__mg\u0024cacheE = new dfEasingFunctions.EasingFunction(dfEasingFunctions.easeInQuad);
            }
            // ISSUE: reference to a compiler-generated field
            return dfEasingFunctions.\u003C\u003Ef__mg\u0024cacheE;
          case dfEasingType.QuadEaseOut:
            // ISSUE: reference to a compiler-generated field
            if (dfEasingFunctions.\u003C\u003Ef__mg\u0024cache10 == null)
            {
              // ISSUE: reference to a compiler-generated field
              dfEasingFunctions.\u003C\u003Ef__mg\u0024cache10 = new dfEasingFunctions.EasingFunction(dfEasingFunctions.easeOutQuad);
            }
            // ISSUE: reference to a compiler-generated field
            return dfEasingFunctions.\u003C\u003Ef__mg\u0024cache10;
          case dfEasingType.QuadEaseInOut:
            // ISSUE: reference to a compiler-generated field
            if (dfEasingFunctions.\u003C\u003Ef__mg\u0024cacheF == null)
            {
              // ISSUE: reference to a compiler-generated field
              dfEasingFunctions.\u003C\u003Ef__mg\u0024cacheF = new dfEasingFunctions.EasingFunction(dfEasingFunctions.easeInOutQuad);
            }
            // ISSUE: reference to a compiler-generated field
            return dfEasingFunctions.\u003C\u003Ef__mg\u0024cacheF;
          case dfEasingType.QuartEaseIn:
            // ISSUE: reference to a compiler-generated field
            if (dfEasingFunctions.\u003C\u003Ef__mg\u0024cache11 == null)
            {
              // ISSUE: reference to a compiler-generated field
              dfEasingFunctions.\u003C\u003Ef__mg\u0024cache11 = new dfEasingFunctions.EasingFunction(dfEasingFunctions.easeInQuart);
            }
            // ISSUE: reference to a compiler-generated field
            return dfEasingFunctions.\u003C\u003Ef__mg\u0024cache11;
          case dfEasingType.QuartEaseOut:
            // ISSUE: reference to a compiler-generated field
            if (dfEasingFunctions.\u003C\u003Ef__mg\u0024cache13 == null)
            {
              // ISSUE: reference to a compiler-generated field
              dfEasingFunctions.\u003C\u003Ef__mg\u0024cache13 = new dfEasingFunctions.EasingFunction(dfEasingFunctions.easeOutQuart);
            }
            // ISSUE: reference to a compiler-generated field
            return dfEasingFunctions.\u003C\u003Ef__mg\u0024cache13;
          case dfEasingType.QuartEaseInOut:
            // ISSUE: reference to a compiler-generated field
            if (dfEasingFunctions.\u003C\u003Ef__mg\u0024cache12 == null)
            {
              // ISSUE: reference to a compiler-generated field
              dfEasingFunctions.\u003C\u003Ef__mg\u0024cache12 = new dfEasingFunctions.EasingFunction(dfEasingFunctions.easeInOutQuart);
            }
            // ISSUE: reference to a compiler-generated field
            return dfEasingFunctions.\u003C\u003Ef__mg\u0024cache12;
          case dfEasingType.QuintEaseIn:
            // ISSUE: reference to a compiler-generated field
            if (dfEasingFunctions.\u003C\u003Ef__mg\u0024cache14 == null)
            {
              // ISSUE: reference to a compiler-generated field
              dfEasingFunctions.\u003C\u003Ef__mg\u0024cache14 = new dfEasingFunctions.EasingFunction(dfEasingFunctions.easeInQuint);
            }
            // ISSUE: reference to a compiler-generated field
            return dfEasingFunctions.\u003C\u003Ef__mg\u0024cache14;
          case dfEasingType.QuintEaseOut:
            // ISSUE: reference to a compiler-generated field
            if (dfEasingFunctions.\u003C\u003Ef__mg\u0024cache16 == null)
            {
              // ISSUE: reference to a compiler-generated field
              dfEasingFunctions.\u003C\u003Ef__mg\u0024cache16 = new dfEasingFunctions.EasingFunction(dfEasingFunctions.easeOutQuint);
            }
            // ISSUE: reference to a compiler-generated field
            return dfEasingFunctions.\u003C\u003Ef__mg\u0024cache16;
          case dfEasingType.QuintEaseInOut:
            // ISSUE: reference to a compiler-generated field
            if (dfEasingFunctions.\u003C\u003Ef__mg\u0024cache15 == null)
            {
              // ISSUE: reference to a compiler-generated field
              dfEasingFunctions.\u003C\u003Ef__mg\u0024cache15 = new dfEasingFunctions.EasingFunction(dfEasingFunctions.easeInOutQuint);
            }
            // ISSUE: reference to a compiler-generated field
            return dfEasingFunctions.\u003C\u003Ef__mg\u0024cache15;
          case dfEasingType.SineEaseIn:
            // ISSUE: reference to a compiler-generated field
            if (dfEasingFunctions.\u003C\u003Ef__mg\u0024cache17 == null)
            {
              // ISSUE: reference to a compiler-generated field
              dfEasingFunctions.\u003C\u003Ef__mg\u0024cache17 = new dfEasingFunctions.EasingFunction(dfEasingFunctions.easeInSine);
            }
            // ISSUE: reference to a compiler-generated field
            return dfEasingFunctions.\u003C\u003Ef__mg\u0024cache17;
          case dfEasingType.SineEaseOut:
            // ISSUE: reference to a compiler-generated field
            if (dfEasingFunctions.\u003C\u003Ef__mg\u0024cache19 == null)
            {
              // ISSUE: reference to a compiler-generated field
              dfEasingFunctions.\u003C\u003Ef__mg\u0024cache19 = new dfEasingFunctions.EasingFunction(dfEasingFunctions.easeOutSine);
            }
            // ISSUE: reference to a compiler-generated field
            return dfEasingFunctions.\u003C\u003Ef__mg\u0024cache19;
          case dfEasingType.SineEaseInOut:
            // ISSUE: reference to a compiler-generated field
            if (dfEasingFunctions.\u003C\u003Ef__mg\u0024cache18 == null)
            {
              // ISSUE: reference to a compiler-generated field
              dfEasingFunctions.\u003C\u003Ef__mg\u0024cache18 = new dfEasingFunctions.EasingFunction(dfEasingFunctions.easeInOutSine);
            }
            // ISSUE: reference to a compiler-generated field
            return dfEasingFunctions.\u003C\u003Ef__mg\u0024cache18;
          case dfEasingType.Spring:
            // ISSUE: reference to a compiler-generated field
            if (dfEasingFunctions.\u003C\u003Ef__mg\u0024cache1A == null)
            {
              // ISSUE: reference to a compiler-generated field
              dfEasingFunctions.\u003C\u003Ef__mg\u0024cache1A = new dfEasingFunctions.EasingFunction(dfEasingFunctions.spring);
            }
            // ISSUE: reference to a compiler-generated field
            return dfEasingFunctions.\u003C\u003Ef__mg\u0024cache1A;
          default:
            throw new NotImplementedException();
        }
      }

      private static float linear(float start, float end, float time) => Mathf.Lerp(start, end, time);

      private static float clerp(float start, float end, float time)
      {
        float num1 = 0.0f;
        float num2 = 360f;
        float num3 = Mathf.Abs((float) (((double) num2 - (double) num1) / 2.0));
        float num4;
        if ((double) end - (double) start < -(double) num3)
        {
          float num5 = (num2 - start + end) * time;
          num4 = start + num5;
        }
        else if ((double) end - (double) start > (double) num3)
        {
          float num6 = (float) -((double) num2 - (double) end + (double) start) * time;
          num4 = start + num6;
        }
        else
          num4 = start + (end - start) * time;
        return num4;
      }

      private static float spring(float start, float end, float time)
      {
        time = Mathf.Clamp01(time);
        time = (float) (((double) Mathf.Sin((float) ((double) time * 3.1415927410125732 * (0.20000000298023224 + 2.5 * (double) time * (double) time * (double) time))) * (double) Mathf.Pow(1f - time, 2.2f) + (double) time) * (1.0 + 1.2000000476837158 * (1.0 - (double) time)));
        return start + (end - start) * time;
      }

      private static float easeInQuad(float start, float end, float time)
      {
        end -= start;
        return end * time * time + start;
      }

      private static float easeOutQuad(float start, float end, float time)
      {
        end -= start;
        return (float) (-(double) end * (double) time * ((double) time - 2.0)) + start;
      }

      private static float easeInOutQuad(float start, float end, float time)
      {
        time /= 0.5f;
        end -= start;
        if ((double) time < 1.0)
          return end / 2f * time * time + start;
        --time;
        return (float) (-(double) end / 2.0 * ((double) time * ((double) time - 2.0) - 1.0)) + start;
      }

      private static float easeInCubic(float start, float end, float time)
      {
        end -= start;
        return end * time * time * time + start;
      }

      private static float easeOutCubic(float start, float end, float time)
      {
        --time;
        end -= start;
        return end * (float) ((double) time * (double) time * (double) time + 1.0) + start;
      }

      private static float easeInOutCubic(float start, float end, float time)
      {
        time /= 0.5f;
        end -= start;
        if ((double) time < 1.0)
          return end / 2f * time * time * time + start;
        time -= 2f;
        return (float) ((double) end / 2.0 * ((double) time * (double) time * (double) time + 2.0)) + start;
      }

      private static float easeInQuart(float start, float end, float time)
      {
        end -= start;
        return end * time * time * time * time + start;
      }

      private static float easeOutQuart(float start, float end, float time)
      {
        --time;
        end -= start;
        return (float) (-(double) end * ((double) time * (double) time * (double) time * (double) time - 1.0)) + start;
      }

      private static float easeInOutQuart(float start, float end, float time)
      {
        time /= 0.5f;
        end -= start;
        if ((double) time < 1.0)
          return end / 2f * time * time * time * time + start;
        time -= 2f;
        return (float) (-(double) end / 2.0 * ((double) time * (double) time * (double) time * (double) time - 2.0)) + start;
      }

      private static float easeInQuint(float start, float end, float time)
      {
        end -= start;
        return end * time * time * time * time * time + start;
      }

      private static float easeOutQuint(float start, float end, float time)
      {
        --time;
        end -= start;
        return end * (float) ((double) time * (double) time * (double) time * (double) time * (double) time + 1.0) + start;
      }

      private static float easeInOutQuint(float start, float end, float time)
      {
        time /= 0.5f;
        end -= start;
        if ((double) time < 1.0)
          return end / 2f * time * time * time * time * time + start;
        time -= 2f;
        return (float) ((double) end / 2.0 * ((double) time * (double) time * (double) time * (double) time * (double) time + 2.0)) + start;
      }

      private static float easeInSine(float start, float end, float time)
      {
        end -= start;
        return -end * Mathf.Cos((float) ((double) time / 1.0 * 1.5707963705062866)) + end + start;
      }

      private static float easeOutSine(float start, float end, float time)
      {
        end -= start;
        return end * Mathf.Sin((float) ((double) time / 1.0 * 1.5707963705062866)) + start;
      }

      private static float easeInOutSine(float start, float end, float time)
      {
        end -= start;
        return (float) (-(double) end / 2.0 * ((double) Mathf.Cos((float) (3.1415927410125732 * (double) time / 1.0)) - 1.0)) + start;
      }

      private static float easeInExpo(float start, float end, float time)
      {
        end -= start;
        return end * Mathf.Pow(2f, (float) (10.0 * ((double) time / 1.0 - 1.0))) + start;
      }

      private static float easeOutExpo(float start, float end, float time)
      {
        end -= start;
        return end * (float) (-(double) Mathf.Pow(2f, (float) (-10.0 * (double) time / 1.0)) + 1.0) + start;
      }

      private static float easeInOutExpo(float start, float end, float time)
      {
        time /= 0.5f;
        end -= start;
        if ((double) time < 1.0)
          return end / 2f * Mathf.Pow(2f, (float) (10.0 * ((double) time - 1.0))) + start;
        --time;
        return (float) ((double) end / 2.0 * (-(double) Mathf.Pow(2f, -10f * time) + 2.0)) + start;
      }

      private static float easeInCirc(float start, float end, float time)
      {
        end -= start;
        return (float) (-(double) end * ((double) Mathf.Sqrt((float) (1.0 - (double) time * (double) time)) - 1.0)) + start;
      }

      private static float easeOutCirc(float start, float end, float time)
      {
        --time;
        end -= start;
        return end * Mathf.Sqrt((float) (1.0 - (double) time * (double) time)) + start;
      }

      private static float easeInOutCirc(float start, float end, float time)
      {
        time /= 0.5f;
        end -= start;
        if ((double) time < 1.0)
          return (float) (-(double) end / 2.0 * ((double) Mathf.Sqrt((float) (1.0 - (double) time * (double) time)) - 1.0)) + start;
        time -= 2f;
        return (float) ((double) end / 2.0 * ((double) Mathf.Sqrt((float) (1.0 - (double) time * (double) time)) + 1.0)) + start;
      }

      private static float bounce(float start, float end, float time)
      {
        time /= 1f;
        end -= start;
        if ((double) time < 0.36363637447357178)
          return end * (121f / 16f * time * time) + start;
        if ((double) time < 0.72727274894714355)
        {
          time -= 0.545454562f;
          return end * (float) (121.0 / 16.0 * (double) time * (double) time + 0.75) + start;
        }
        if ((double) time < 10.0 / 11.0)
        {
          time -= 0.8181818f;
          return end * (float) (121.0 / 16.0 * (double) time * (double) time + 15.0 / 16.0) + start;
        }
        time -= 0.954545438f;
        return end * (float) (121.0 / 16.0 * (double) time * (double) time + 63.0 / 64.0) + start;
      }

      private static float easeInBack(float start, float end, float time)
      {
        end -= start;
        time /= 1f;
        float num = 1.70158f;
        return (float) ((double) end * (double) time * (double) time * (((double) num + 1.0) * (double) time - (double) num)) + start;
      }

      private static float easeOutBack(float start, float end, float time)
      {
        float num = 1.70158f;
        end -= start;
        time = (float) ((double) time / 1.0 - 1.0);
        return end * (float) ((double) time * (double) time * (((double) num + 1.0) * (double) time + (double) num) + 1.0) + start;
      }

      private static float easeInOutBack(float start, float end, float time)
      {
        float num1 = 1.70158f;
        end -= start;
        time /= 0.5f;
        if ((double) time < 1.0)
        {
          float num2 = num1 * 1.525f;
          return (float) ((double) end / 2.0 * ((double) time * (double) time * (((double) num2 + 1.0) * (double) time - (double) num2))) + start;
        }
        time -= 2f;
        float num3 = num1 * 1.525f;
        return (float) ((double) end / 2.0 * ((double) time * (double) time * (((double) num3 + 1.0) * (double) time + (double) num3) + 2.0)) + start;
      }

      private static float punch(float amplitude, float time)
      {
        if ((double) time == 0.0 || (double) time == 1.0)
          return 0.0f;
        float num1 = 0.3f;
        float num2 = num1 / 6.28318548f * Mathf.Asin(0.0f);
        return amplitude * Mathf.Pow(2f, -10f * time) * Mathf.Sin((float) (((double) time * 1.0 - (double) num2) * 6.2831854820251465) / num1);
      }

      public delegate float EasingFunction(float start, float end, float time);
    }

}
