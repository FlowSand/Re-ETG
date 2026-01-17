// Decompiled with JetBrains decompiler
// Type: dfAnimatedValue`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public abstract class dfAnimatedValue<T> where T : struct
    {
      private T startValue;
      private T endValue;
      private float animLength = 1f;
      private float startTime;
      private bool isDone;
      private dfEasingType easingType;
      private dfEasingFunctions.EasingFunction easingFunction;

      protected internal dfAnimatedValue(T StartValue, T EndValue, float Time)
        : this()
      {
        this.startValue = StartValue;
        this.endValue = EndValue;
        this.animLength = Time;
      }

      protected internal dfAnimatedValue()
      {
        this.startTime = UnityEngine.Time.realtimeSinceStartup;
        this.easingFunction = dfEasingFunctions.GetFunction(this.easingType);
      }

      public bool IsDone => this.isDone;

      public float Length
      {
        get => this.animLength;
        set
        {
          this.animLength = value;
          this.startTime = UnityEngine.Time.realtimeSinceStartup;
          this.isDone = false;
        }
      }

      public T StartValue
      {
        get => this.startValue;
        set
        {
          this.startValue = value;
          this.startTime = UnityEngine.Time.realtimeSinceStartup;
          this.isDone = false;
        }
      }

      public T EndValue
      {
        get => this.endValue;
        set
        {
          this.endValue = value;
          this.startTime = UnityEngine.Time.realtimeSinceStartup;
          this.isDone = false;
        }
      }

      public T Value
      {
        get
        {
          float num = UnityEngine.Time.realtimeSinceStartup - this.startTime;
          if ((double) num < (double) this.animLength)
            return this.Lerp(this.startValue, this.endValue, this.easingFunction(0.0f, 1f, Mathf.Clamp01(num / this.animLength)));
          this.isDone = true;
          return this.endValue;
        }
      }

      public dfEasingType EasingType
      {
        get => this.easingType;
        set
        {
          this.easingType = value;
          this.easingFunction = dfEasingFunctions.GetFunction(this.easingType);
        }
      }

      protected abstract T Lerp(T start, T end, float time);

      public static implicit operator T(dfAnimatedValue<T> animated) => animated.Value;
    }

}
