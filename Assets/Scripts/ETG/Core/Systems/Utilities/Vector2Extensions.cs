// Decompiled with JetBrains decompiler
// Type: Vector2Extensions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public static class Vector2Extensions
    {
      public static Vector2 min => new Vector2(float.MinValue, float.MinValue);

      public static Vector2 max => new Vector2(float.MaxValue, float.MaxValue);

      public static bool Approximately(this Vector2 vector, Vector2 other)
      {
        return Mathf.Approximately(vector.x, other.x) && Mathf.Approximately(vector.y, other.y);
      }

      public static float ComponentProduct(this Vector2 vector) => vector.x * vector.y;

      public static Vector2 WithX(this Vector2 vector, float x) => new Vector2(x, vector.y);

      public static Vector2 WithY(this Vector2 vector, float y) => new Vector2(vector.x, y);

      public static Vector2 Rotate(this Vector2 v, float degrees)
      {
        float num1 = Mathf.Sin(degrees * ((float) Math.PI / 180f));
        float num2 = Mathf.Cos(degrees * ((float) Math.PI / 180f));
        float x = v.x;
        float y = v.y;
        v.x = (float) ((double) num2 * (double) x - (double) num1 * (double) y);
        v.y = (float) ((double) num1 * (double) x + (double) num2 * (double) y);
        return v;
      }

      public static Vector4 ToVector4(this Vector2 vector)
      {
        return new Vector4(vector.x, vector.y, 0.0f, 0.0f);
      }

      public static IntVector2 ToIntVector2(this Vector2 vector, VectorConversions convertMethod = VectorConversions.Round)
      {
        if (convertMethod == VectorConversions.Ceil)
          return new IntVector2(Mathf.CeilToInt(vector.x), Mathf.CeilToInt(vector.y));
        return convertMethod == VectorConversions.Floor ? new IntVector2(Mathf.FloorToInt(vector.x), Mathf.FloorToInt(vector.y)) : new IntVector2(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y));
      }

      public static Vector3 ToVector3XUp(this Vector2 vector, float x = 0.0f)
      {
        return new Vector3(x, vector.x, vector.y);
      }

      public static Vector3 ToVector3YUp(this Vector2 vector, float y = 0.0f)
      {
        return new Vector3(vector.x, y, vector.y);
      }

      public static Vector3 ToVector3ZUp(this Vector2 vector, float z = 0.0f)
      {
        return new Vector3(vector.x, vector.y, z);
      }

      public static Vector3 ToVector3ZisY(this Vector2 vector, float offset = 0.0f)
      {
        return new Vector3(vector.x, vector.y, vector.y + offset);
      }

      public static bool IsWithin(this Vector2 vector, Vector2 min, Vector2 max)
      {
        return (double) vector.x >= (double) min.x && (double) vector.x <= (double) max.x && (double) vector.y >= (double) min.y && (double) vector.y <= (double) max.y;
      }

      public static Vector2 Clamp(Vector2 vector, Vector2 min, Vector2 max)
      {
        return new Vector2(Mathf.Clamp(vector.x, min.x, max.x), Mathf.Clamp(vector.y, min.y, max.y));
      }

      public static float ToAngle(this Vector2 vector) => BraveMathCollege.Atan2Degrees(vector);

      public static Vector2 Abs(this Vector2 vector)
      {
        return new Vector2(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
      }

      public static float Cross(Vector2 a, Vector2 b)
      {
        return (float) ((double) a.x * (double) b.y - (double) a.y * (double) b.x);
      }

      public static Vector2 Cross(Vector2 a, float s) => new Vector2(s * a.y, -s * a.x);

      public static Vector2 Cross(float s, Vector2 a) => new Vector2(-s * a.y, s * a.x);

      public static bool IsHorizontal(this Vector2 vector)
      {
        return (double) Mathf.Abs(vector.x) > 0.0 && (double) vector.y == 0.0;
      }

      public static Vector2 SmoothStep(Vector2 from, Vector2 to, float t)
      {
        return new Vector2(Mathf.SmoothStep(from.x, to.x, t), Mathf.SmoothStep(from.y, to.y, t));
      }

      public static float SqrDistance(Vector2 a, Vector2 b)
      {
        double num1 = (double) a.x - (double) b.x;
        double num2 = (double) a.y - (double) b.y;
        return (float) (num1 * num1 + num2 * num2);
      }
    }

}
