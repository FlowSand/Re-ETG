// Decompiled with JetBrains decompiler
// Type: dfVectorExtensions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

  public static class dfVectorExtensions
  {
    public static bool IsNaN(this Vector3 vector)
    {
      return float.IsNaN(vector.x) || float.IsNaN(vector.y) || float.IsNaN(vector.z);
    }

    public static Vector3 ClampRotation(this Vector3 euler)
    {
      if ((double) euler.x < 0.0)
        euler.x += 360f;
      if ((double) euler.x >= 360.0)
        euler.x -= 360f;
      if ((double) euler.y < 0.0)
        euler.y += 360f;
      if ((double) euler.y >= 360.0)
        euler.y -= 360f;
      if ((double) euler.z < 0.0)
        euler.z += 360f;
      if ((double) euler.z >= 360.0)
        euler.z -= 360f;
      return euler;
    }

    public static Vector2 Scale(this Vector2 vector, float x, float y)
    {
      return new Vector2(vector.x * x, vector.y * y);
    }

    public static Vector3 Scale(this Vector3 vector, float x, float y, float z)
    {
      return new Vector3(vector.x * x, vector.y * y, vector.z * z);
    }

    public static Vector3 FloorToInt(this Vector3 vector)
    {
      return new Vector3((float) Mathf.FloorToInt(vector.x), (float) Mathf.FloorToInt(vector.y), (float) Mathf.FloorToInt(vector.z));
    }

    public static Vector3 CeilToInt(this Vector3 vector)
    {
      return new Vector3((float) Mathf.CeilToInt(vector.x), (float) Mathf.CeilToInt(vector.y), (float) Mathf.CeilToInt(vector.z));
    }

    public static Vector2 FloorToInt(this Vector2 vector)
    {
      return new Vector2((float) Mathf.FloorToInt(vector.x), (float) Mathf.FloorToInt(vector.y));
    }

    public static Vector2 CeilToInt(this Vector2 vector)
    {
      return new Vector2((float) Mathf.CeilToInt(vector.x), (float) Mathf.CeilToInt(vector.y));
    }

    public static Vector3 RoundToInt(this Vector3 vector)
    {
      return new Vector3((float) Mathf.RoundToInt(vector.x), (float) Mathf.RoundToInt(vector.y), (float) Mathf.RoundToInt(vector.z));
    }

    public static Vector2 RoundToInt(this Vector2 vector)
    {
      return new Vector2((float) Mathf.RoundToInt(vector.x), (float) Mathf.RoundToInt(vector.y));
    }

    public static Vector2 Quantize(this Vector2 vector, float discreteValue)
    {
      vector.x = (float) Mathf.RoundToInt(vector.x / discreteValue) * discreteValue;
      vector.y = (float) Mathf.RoundToInt(vector.y / discreteValue) * discreteValue;
      return vector;
    }

    public static Vector2 Quantize(
      this Vector2 vector,
      float discreteValue,
      VectorConversions conversionMethod)
    {
      if (conversionMethod != VectorConversions.Ceil)
      {
        if (conversionMethod != VectorConversions.Floor)
          return vector.Quantize(discreteValue);
        vector.x = (float) Mathf.FloorToInt(vector.x / discreteValue) * discreteValue;
        vector.y = (float) Mathf.FloorToInt(vector.y / discreteValue) * discreteValue;
        return vector;
      }
      vector.x = (float) Mathf.CeilToInt(vector.x / discreteValue) * discreteValue;
      vector.y = (float) Mathf.CeilToInt(vector.y / discreteValue) * discreteValue;
      return vector;
    }

    public static Vector3 Quantize(
      this Vector3 vector,
      float discreteValue,
      VectorConversions conversionMethod)
    {
      if (conversionMethod != VectorConversions.Ceil)
      {
        if (conversionMethod != VectorConversions.Floor)
          return vector.Quantize(discreteValue);
        vector.x = (float) Mathf.FloorToInt(vector.x / discreteValue) * discreteValue;
        vector.y = (float) Mathf.FloorToInt(vector.y / discreteValue) * discreteValue;
        vector.z = (float) Mathf.FloorToInt(vector.z / discreteValue) * discreteValue;
        return vector;
      }
      vector.x = (float) Mathf.CeilToInt(vector.x / discreteValue) * discreteValue;
      vector.y = (float) Mathf.CeilToInt(vector.y / discreteValue) * discreteValue;
      vector.z = (float) Mathf.CeilToInt(vector.z / discreteValue) * discreteValue;
      return vector;
    }

    public static Vector3 Quantize(this Vector3 vector, float discreteValue)
    {
      vector.x = (float) Mathf.RoundToInt(vector.x / discreteValue) * discreteValue;
      vector.y = (float) Mathf.RoundToInt(vector.y / discreteValue) * discreteValue;
      vector.z = (float) Mathf.RoundToInt(vector.z / discreteValue) * discreteValue;
      return vector;
    }

    public static Vector3 QuantizeFloor(this Vector3 vector, float discreteValue)
    {
      vector.x = (float) Mathf.FloorToInt(vector.x / discreteValue) * discreteValue;
      vector.y = (float) Mathf.FloorToInt(vector.y / discreteValue) * discreteValue;
      vector.z = (float) Mathf.FloorToInt(vector.z / discreteValue) * discreteValue;
      return vector;
    }
  }

