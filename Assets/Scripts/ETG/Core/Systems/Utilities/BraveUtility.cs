// Decompiled with JetBrains decompiler
// Type: BraveUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public static class BraveUtility
    {
      private const float c_screenWidthTiles = 30f;
      private const float c_screenHeightTiles = 16.875f;
      public static BraveUtility.LogVerbosity verbosity = BraveUtility.LogVerbosity.IMPORTANT;

      public static void DrawDebugSquare(Vector2 min, Color color)
      {
        BraveUtility.DrawDebugSquare(min.x, min.x + 1f, min.y, min.y + 1f, color);
      }

      public static void DrawDebugSquare(Vector2 min, Vector2 max, Color color)
      {
        BraveUtility.DrawDebugSquare(min.x, max.x, min.y, max.y, color);
      }

      public static void DrawDebugSquare(float minX, float maxX, float minY, float maxY, Color color)
      {
        UnityEngine.Debug.DrawLine(new Vector3(minX, minY, 0.0f), new Vector3(maxX, minY, 0.0f), color);
        UnityEngine.Debug.DrawLine(new Vector3(minX, maxY, 0.0f), new Vector3(maxX, maxY, 0.0f), color);
        UnityEngine.Debug.DrawLine(new Vector3(minX, minY, 0.0f), new Vector3(minX, maxY, 0.0f), color);
        UnityEngine.Debug.DrawLine(new Vector3(maxX, minY, 0.0f), new Vector3(maxX, maxY, 0.0f), color);
      }

      public static void DrawDebugSquare(Vector2 min, Color color, float duration)
      {
        BraveUtility.DrawDebugSquare(min.x, min.x + 1f, min.y, min.y + 1f, color, duration);
      }

      public static void DrawDebugSquare(Vector2 min, Vector2 max, Color color, float duration)
      {
        BraveUtility.DrawDebugSquare(min.x, max.x, min.y, max.y, color, duration);
      }

      public static void DrawDebugSquare(
        float minX,
        float maxX,
        float minY,
        float maxY,
        Color color,
        float duration)
      {
        UnityEngine.Debug.DrawLine(new Vector3(minX, minY, 0.0f), new Vector3(maxX, minY, 0.0f), color, duration);
        UnityEngine.Debug.DrawLine(new Vector3(minX, maxY, 0.0f), new Vector3(maxX, maxY, 0.0f), color, duration);
        UnityEngine.Debug.DrawLine(new Vector3(minX, minY, 0.0f), new Vector3(minX, maxY, 0.0f), color, duration);
        UnityEngine.Debug.DrawLine(new Vector3(maxX, minY, 0.0f), new Vector3(maxX, maxY, 0.0f), color, duration);
      }

      public static Vector3 GetMousePosition()
      {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float enter;
        new Plane(Vector3.back, Vector3.zero).Raycast(ray, out enter);
        return ray.GetPoint(enter);
      }

      public static Vector3 ViewportToWorldpoint(Vector2 viewportPos, ViewportType viewportType)
      {
        if (viewportType == ViewportType.Camera)
        {
          Ray ray = Camera.main.ViewportPointToRay((Vector3) viewportPos);
          float enter;
          new Plane(Vector3.back, Vector3.zero).Raycast(ray, out enter);
          return ray.GetPoint(enter);
        }
        if (viewportType != ViewportType.Gameplay)
          throw new ArgumentException("Unknown viewport type: " + (object) viewportType);
        Vector2 vector2_1 = (Vector2) BraveUtility.ScreenCenterWorldPoint();
        Vector2 vector2_2 = new Vector2((float) (30.0 * ((double) viewportPos.x - 0.5)), (float) (16.875 * ((double) viewportPos.y - 0.5)));
        float overrideZoomScale = GameManager.Instance.MainCameraController.OverrideZoomScale;
        if ((double) overrideZoomScale != 1.0 && (double) overrideZoomScale != 0.0)
          vector2_2 /= overrideZoomScale;
        return (Vector3) (vector2_1 + vector2_2);
      }

      public static Vector2 WorldPointToViewport(Vector3 worldPoint, ViewportType viewportType)
      {
        if (viewportType == ViewportType.Camera)
          return (Vector2) Camera.main.WorldToViewportPoint(worldPoint);
        if (viewportType != ViewportType.Gameplay)
          throw new ArgumentException("Unknown viewport type: " + (object) viewportType);
        Vector2 worldpoint = (Vector2) BraveUtility.ViewportToWorldpoint(new Vector2(0.0f, 0.0f), ViewportType.Gameplay);
        Vector2 vector2 = (Vector2) BraveUtility.ViewportToWorldpoint(new Vector2(1f, 1f), ViewportType.Gameplay) - worldpoint;
        return new Vector2((worldPoint.x - worldpoint.x) / vector2.x, (worldPoint.y - worldpoint.y) / vector2.y);
      }

      public static Vector3 ScreenCenterWorldPoint()
      {
        return BraveUtility.ViewportToWorldpoint(new Vector2(0.5f, 0.5f), ViewportType.Camera);
      }

      public static bool PointIsVisible(
        Vector2 flatPoint,
        float percentBuffer,
        ViewportType viewportType)
      {
        Vector2 worldpoint1 = (Vector2) BraveUtility.ViewportToWorldpoint(new Vector2(0.0f, 0.0f), viewportType);
        Vector2 worldpoint2 = (Vector2) BraveUtility.ViewportToWorldpoint(new Vector2(1f, 1f), viewportType);
        Vector2 vector2 = (worldpoint2 - worldpoint1) * percentBuffer;
        return (double) flatPoint.x > (double) worldpoint1.x - (double) vector2.x && (double) flatPoint.x < (double) worldpoint2.x + (double) vector2.x && (double) flatPoint.y > (double) worldpoint1.y - (double) vector2.y && (double) flatPoint.y < (double) worldpoint2.y + (double) vector2.y;
      }

      public static Vector3 GetMinimapViewportPosition(Vector2 pos)
      {
        float num1 = pos.x / (float) Screen.width;
        float num2 = pos.y / (float) Screen.height;
        return (Vector3) new Vector2((float) (((double) num1 - 0.5) / (double) BraveCameraUtility.GetRect().width + 0.5), (float) (((double) num2 - 0.5) / (double) BraveCameraUtility.GetRect().height + 0.5));
      }

      public static Vector2[] ResizeArray(Vector2[] a, int size)
      {
        Vector2[] vector2Array = new Vector2[size];
        for (int index = 0; index < size; ++index)
          vector2Array[index] = a[index];
        return vector2Array;
      }

      public static Vector2 GetClosestPoint(Vector2 a, Vector2 b, Vector2 p)
      {
        Vector2 lhs = p - a;
        Vector2 rhs = b - a;
        float num = Vector2.Dot(lhs, rhs) / rhs.sqrMagnitude;
        return a + rhs * num;
      }

      public static bool LineIntersectsLine(
        Vector2 a1,
        Vector2 a2,
        Vector2 b1,
        Vector2 b2,
        out Vector2 intersection)
      {
        intersection = Vector2.zero;
        Vector2 vector2_1 = a2 - a1;
        Vector2 vector2_2 = b2 - b1;
        float num1 = (float) ((double) vector2_1.x * (double) vector2_2.y - (double) vector2_1.y * (double) vector2_2.x);
        if ((double) num1 == 0.0)
          return false;
        Vector2 vector2_3 = b1 - a1;
        float num2 = (float) ((double) vector2_3.x * (double) vector2_2.y - (double) vector2_3.y * (double) vector2_2.x) / num1;
        if ((double) num2 < 0.0 || (double) num2 > 1.0)
          return false;
        float num3 = (float) ((double) vector2_3.x * (double) vector2_1.y - (double) vector2_3.y * (double) vector2_1.x) / num1;
        if ((double) num3 < 0.0 || (double) num3 > 1.0)
          return false;
        intersection = a1 + num2 * vector2_1;
        return true;
      }

      public static bool LineIntersectsAABB(
        Vector2 l1,
        Vector2 l2,
        Vector2 bOrigin,
        Vector2 bSize,
        out Vector2 intersection)
      {
        intersection = new Vector2();
        float num = float.MaxValue;
        Vector2 intersection1;
        if (BraveUtility.LineIntersectsLine(l1, l2, bOrigin, bOrigin + new Vector2(0.0f, bSize.y), out intersection1))
        {
          float sqrMagnitude = (l1 - intersection1).sqrMagnitude;
          if ((double) sqrMagnitude < (double) num)
          {
            intersection = intersection1;
            num = sqrMagnitude;
          }
        }
        if (BraveUtility.LineIntersectsLine(l1, l2, bOrigin + new Vector2(0.0f, bSize.y), bOrigin + bSize, out intersection1))
        {
          float sqrMagnitude = (l1 - intersection1).sqrMagnitude;
          if ((double) sqrMagnitude < (double) num)
          {
            intersection = intersection1;
            num = sqrMagnitude;
          }
        }
        if (BraveUtility.LineIntersectsLine(l1, l2, bOrigin + bSize, bOrigin + new Vector2(bSize.x, 0.0f), out intersection1))
        {
          float sqrMagnitude = (l1 - intersection1).sqrMagnitude;
          if ((double) sqrMagnitude < (double) num)
          {
            intersection = intersection1;
            num = sqrMagnitude;
          }
        }
        if (BraveUtility.LineIntersectsLine(l1, l2, bOrigin + new Vector2(bSize.x, 0.0f), bOrigin, out intersection1))
        {
          float sqrMagnitude = (l1 - intersection1).sqrMagnitude;
          if ((double) sqrMagnitude < (double) num)
          {
            intersection = intersection1;
            num = sqrMagnitude;
          }
        }
        return (double) num != 3.4028234663852886E+38;
      }

      public static bool GreaterThanAlongMajorAxis(Vector2 lhs, Vector2 rhs, Vector2 axis)
      {
        Vector2 majorAxis = BraveUtility.GetMajorAxis(axis);
        Vector2.Scale(lhs, majorAxis);
        Vector2.Scale(rhs, majorAxis);
        return (double) (lhs.x + lhs.y) > (double) (rhs.x + rhs.y);
      }

      public static Vector2 GetMajorAxis(Vector2 vector)
      {
        return (double) Mathf.Abs(vector.x) > (double) Mathf.Abs(vector.y) ? new Vector2(Mathf.Sign(vector.x), 0.0f) : new Vector2(0.0f, Mathf.Sign(vector.y));
      }

      public static IntVector2 GetMajorAxis(IntVector2 vector)
      {
        return Mathf.Abs(vector.x) > Mathf.Abs(vector.y) ? new IntVector2(Math.Sign(vector.x), 0) : new IntVector2(0, Math.Sign(vector.y));
      }

      public static IntVector2 GetIntMajorAxis(IntVector2 vector)
      {
        return BraveUtility.GetIntMajorAxis(vector.ToVector2());
      }

      public static IntVector2 GetIntMajorAxis(Vector2 vector)
      {
        return (double) Mathf.Abs(vector.x) > (double) Mathf.Abs(vector.y) ? new IntVector2(Math.Sign(vector.x), 0) : new IntVector2(0, Math.Sign(vector.y));
      }

      public static Vector2 GetMinorAxis(Vector2 vector)
      {
        return (double) Mathf.Abs(vector.x) <= (double) Mathf.Abs(vector.y) ? new Vector2(Mathf.Sign(vector.x), 0.0f) : new Vector2(0.0f, Mathf.Sign(vector.y));
      }

      public static IntVector2 GetMinorAxis(IntVector2 vector)
      {
        return Mathf.Abs(vector.x) <= Mathf.Abs(vector.y) ? new IntVector2(Math.Sign(vector.x), 0) : new IntVector2(0, Math.Sign(vector.y));
      }

      public static IntVector2 GetIntMinorAxis(Vector2 vector)
      {
        return (double) Mathf.Abs(vector.x) <= (double) Mathf.Abs(vector.y) ? new IntVector2(Math.Sign(vector.x), 0) : new IntVector2(0, Math.Sign(vector.y));
      }

      public static Vector2 GetPerp(Vector2 v) => new Vector2(-v.y, v.x);

      public static Vector2 QuantizeVector(Vector2 vec)
      {
        int unitsPerUnit = !((UnityEngine.Object) PhysicsEngine.Instance == (UnityEngine.Object) null) ? PhysicsEngine.Instance.PixelsPerUnit : 16 /*0x10*/;
        return BraveUtility.QuantizeVector(vec, (float) unitsPerUnit);
      }

      public static Vector2 QuantizeVector(Vector2 vec, float unitsPerUnit)
      {
        return new Vector2(Mathf.Round(vec.x * unitsPerUnit), Mathf.Round(vec.y * unitsPerUnit)) / unitsPerUnit;
      }

      public static Vector3 QuantizeVector(Vector3 vec)
      {
        return BraveUtility.QuantizeVector(vec, (float) PhysicsEngine.Instance.PixelsPerUnit);
      }

      public static Vector3 QuantizeVector(Vector3 vec, float unitsPerUnit)
      {
        return new Vector3(Mathf.Round(vec.x * unitsPerUnit), Mathf.Round(vec.y * unitsPerUnit), Mathf.Round(vec.z * unitsPerUnit)) / unitsPerUnit;
      }

      public static int GCD(int a, int b)
      {
        int num;
        for (; b != 0; b = num)
        {
          num = a % b;
          a = b;
        }
        return a;
      }

      public static int GetTileMapLayerByName(string name, tk2dTileMap tileMap)
      {
        for (int index = 0; index < tileMap.data.tileMapLayers.Count; ++index)
        {
          if (tileMap.data.tileMapLayers[index].name == name)
            return index;
        }
        return -1;
      }

      public static T GetClosestToPosition<T>(List<T> sources, Vector2 pos, params T[] excluded) where T : BraveBehaviour
      {
        return BraveUtility.GetClosestToPosition<T>(sources, pos, (Func<T, bool>) null, excluded);
      }

      public static T GetClosestToPosition<T>(
        List<T> sources,
        Vector2 pos,
        Func<T, bool> isValid,
        params T[] excluded)
        where T : BraveBehaviour
      {
        return BraveUtility.GetClosestToPosition<T>(sources, pos, isValid, -1f, excluded);
      }

      public static T GetClosestToPosition<T>(
        List<T> sources,
        Vector2 pos,
        Func<T, bool> isValid,
        float maxDistance,
        params T[] excluded)
        where T : BraveBehaviour
      {
        T closestToPosition = (T) null;
        float num1 = float.MaxValue;
        if (sources == null)
          return closestToPosition;
        for (int index = 0; index < sources.Count; ++index)
        {
          if ((bool) (UnityEngine.Object) sources[index] && (excluded == null || excluded.Length >= sources.Count || Array.IndexOf<T>(excluded, sources[index]) == -1) && (isValid == null || isValid(sources[index])))
          {
            float num2 = !((UnityEngine.Object) sources[index].sprite != (UnityEngine.Object) null) ? Vector2.SqrMagnitude(sources[index].transform.position.XY() - pos) : Vector2.SqrMagnitude(sources[index].sprite.WorldCenter - pos);
            if (((double) maxDistance <= 0.0 || (double) num2 <= (double) maxDistance) && (double) num2 < (double) num1)
            {
              closestToPosition = sources[index];
              num1 = num2;
            }
          }
        }
        return closestToPosition;
      }

      public static T[][] MultidimensionalArrayResize<T>(
        T[][] original,
        int oldWidth,
        int oldHeight,
        int newWidth,
        int newHeight)
      {
        T[][] objArray = new T[newWidth][];
        for (int index = 0; index < newWidth; ++index)
          objArray[index] = new T[newHeight];
        int num1 = Mathf.Min(oldWidth, newWidth);
        int num2 = Mathf.Min(oldHeight, newHeight);
        for (int index1 = 0; index1 < num1; ++index1)
        {
          for (int index2 = 0; index2 < num2; ++index2)
            objArray[index1][index2] = original[index1][index2];
        }
        return objArray;
      }

      public static T[,] MultidimensionalArrayResize<T>(T[,] original, int rows, int cols)
      {
        T[,] objArray = new T[rows, cols];
        int num1 = Mathf.Min(rows, original.GetLength(0));
        int num2 = Mathf.Min(cols, original.GetLength(1));
        for (int index1 = 0; index1 < num1; ++index1)
        {
          for (int index2 = 0; index2 < num2; ++index2)
            objArray[index1, index2] = original[index1, index2];
        }
        return objArray;
      }

      public static int[] ParsePageNums(string str)
      {
        string[] strArray1 = str.Split(',');
        List<int> intList = new List<int>(strArray1.Length);
        for (int index1 = 0; index1 < strArray1.Length; ++index1)
        {
          string s = strArray1[index1].Trim();
          int result1;
          if (int.TryParse(s, out result1))
          {
            intList.Add(result1);
          }
          else
          {
            string[] strArray2 = s.Split('-');
            int result2;
            int result3;
            if (strArray2.Length > 1 && int.TryParse(strArray2[0], out result2) && int.TryParse(strArray2[1], out result3) && result3 >= result2)
            {
              for (int index2 = result2; index2 <= result3; ++index2)
                intList.Add(index2);
            }
          }
        }
        return intList.ToArray();
      }

      public static int EnumFlagsContains(uint data, uint valToFind)
      {
        return ((int) data & (int) valToFind) == (int) valToFind ? 1 : 0;
      }

      public static void Assert(bool assert, string s, bool pauseEditor = false)
      {
        if (!assert)
          return;
        BraveUtility.Log(s, Color.red, BraveUtility.LogVerbosity.IMPORTANT);
      }

      public static void Log(string s, Color c, BraveUtility.LogVerbosity v = BraveUtility.LogVerbosity.VERBOSE)
      {
      }

      public static List<T> GenerationShuffle<T>(this List<T> input)
      {
        for (int index1 = input.Count - 1; index1 > 1; --index1)
        {
          int index2 = BraveRandom.GenerationRandomRange(0, index1);
          T obj = input[index1];
          input[index1] = input[index2];
          input[index2] = obj;
        }
        return input;
      }

      public static List<T> Shuffle<T>(this List<T> input)
      {
        for (int index1 = input.Count - 1; index1 > 1; --index1)
        {
          int index2 = UnityEngine.Random.Range(0, index1);
          T obj = input[index1];
          input[index1] = input[index2];
          input[index2] = obj;
        }
        return input;
      }

      public static List<T> SafeShuffle<T>(this List<T> input)
      {
        System.Random random = new System.Random();
        for (int index1 = input.Count - 1; index1 > 1; --index1)
        {
          int index2 = random.Next(index1);
          T obj = input[index1];
          input[index1] = input[index2];
          input[index2] = obj;
        }
        return input;
      }

      public static T RandomElement<T>(List<T> list) => list[UnityEngine.Random.Range(0, list.Count)];

      public static T RandomElement<T>(T[] array) => array[UnityEngine.Random.Range(0, array.Length)];

      public static bool RandomBool() => (double) UnityEngine.Random.value >= 0.5;

      public static float RandomSign() => (double) UnityEngine.Random.value <= 0.5 ? -1f : 1f;

      public static float RandomAngle() => UnityEngine.Random.Range(0.0f, 360f);

      public static Vector2 RandomVector2(Vector2 min, Vector2 max)
      {
        return new Vector2(UnityEngine.Random.Range(min.x, max.x), UnityEngine.Random.Range(min.y, max.y));
      }

      public static Vector2 RandomVector2(Vector2 min, Vector2 max, Vector2 padding)
      {
        if ((double) padding.x < 0.0 && (double) padding.y < 0.0)
        {
          if (BraveUtility.RandomBool())
            padding.x *= -1f;
          else
            padding.y *= -1f;
        }
        return new Vector2((double) padding.x < 0.0 ? (!BraveUtility.RandomBool() ? UnityEngine.Random.Range(max.x + padding.x, max.x) : UnityEngine.Random.Range(min.x, min.x - padding.x)) : UnityEngine.Random.Range(min.x + padding.x, max.x - padding.x), (double) padding.y < 0.0 ? (!BraveUtility.RandomBool() ? UnityEngine.Random.Range(max.y + padding.y, max.y) : UnityEngine.Random.Range(min.y, min.y - padding.y)) : UnityEngine.Random.Range(min.y + padding.y, max.y - padding.y));
      }

      public static void RandomizeList<T>(List<T> list, int startIndex = 0, int length = -1)
      {
        int max = length >= 0 ? startIndex + length : list.Count;
        for (int index1 = startIndex; index1 < max - 1; ++index1)
        {
          int index2 = UnityEngine.Random.Range(index1 + 1, max);
          T obj = list[index1];
          list[index1] = list[index2];
          list[index2] = obj;
        }
      }

      public static void RandomizeArray<T>(T[] array, int startIndex = 0, int length = -1)
      {
        int max = length >= 0 ? startIndex + length : array.Length;
        for (int index1 = startIndex; index1 < max - 1; ++index1)
        {
          int index2 = UnityEngine.Random.Range(index1 + 1, max);
          T obj = array[index1];
          array[index1] = array[index2];
          array[index2] = obj;
        }
      }

      public static bool isLoadingLevel => Application.isLoadingLevel;

      public static void DrawDebugSquare(IntVector2 pos, Color col)
      {
        UnityEngine.Debug.DrawLine((Vector3) pos.ToVector2(), (Vector3) (pos.ToVector2() + Vector2.up), col, 1000f);
        UnityEngine.Debug.DrawLine((Vector3) pos.ToVector2(), (Vector3) (pos.ToVector2() + Vector2.right), col, 1000f);
        UnityEngine.Debug.DrawLine((Vector3) (pos.ToVector2() + Vector2.up), (Vector3) (pos.ToVector2() + Vector2.right + Vector2.up), col, 1000f);
        UnityEngine.Debug.DrawLine((Vector3) (pos.ToVector2() + Vector2.right), (Vector3) (pos.ToVector2() + Vector2.right + Vector2.up), col, 1000f);
      }

      private static string ColorToHex(Color col)
      {
        float num1 = col.r * (float) byte.MaxValue;
        float num2 = col.g * (float) byte.MaxValue;
        float num3 = col.b * (float) byte.MaxValue;
        return BraveUtility.GetHex(Mathf.FloorToInt(num1 / 16f)) + BraveUtility.GetHex(Mathf.RoundToInt(num1 % 16f)) + BraveUtility.GetHex(Mathf.FloorToInt(num2 / 16f)) + BraveUtility.GetHex(Mathf.RoundToInt(num2 % 16f)) + BraveUtility.GetHex(Mathf.FloorToInt(num3 / 16f)) + BraveUtility.GetHex(Mathf.RoundToInt(num3 % 16f));
      }

      public static string ColorToHexWithAlpha(Color col)
      {
        float num1 = col.r * (float) byte.MaxValue;
        float num2 = col.g * (float) byte.MaxValue;
        float num3 = col.b * (float) byte.MaxValue;
        float num4 = col.a * (float) byte.MaxValue;
        return BraveUtility.GetHex(Mathf.FloorToInt(num1 / 16f)) + BraveUtility.GetHex(Mathf.RoundToInt(num1 % 16f)) + BraveUtility.GetHex(Mathf.FloorToInt(num2 / 16f)) + BraveUtility.GetHex(Mathf.RoundToInt(num2 % 16f)) + BraveUtility.GetHex(Mathf.FloorToInt(num3 / 16f)) + BraveUtility.GetHex(Mathf.RoundToInt(num3 % 16f)) + BraveUtility.GetHex(Mathf.FloorToInt(num4 / 16f)) + BraveUtility.GetHex(Mathf.RoundToInt(num4 % 16f));
      }

      public static void AssignPositionalSoundTracking(GameObject obj)
      {
        if (!((UnityEngine.Object) obj.GetComponent<AkGameObj>() == (UnityEngine.Object) null))
          ;
      }

      public static bool DX11Supported() => SystemInfo.graphicsShaderLevel >= 50;

      private static string GetHex(int d)
      {
        d = Mathf.Min(15, Mathf.Max(0, d));
        string str = "0123456789ABCDEF";
        return string.Empty + (object) str[d];
      }

      public static string DecrementString(string str)
      {
        string baseStr;
        string suffixStr;
        BraveUtility.SplitNumericSuffix(str, out baseStr, out suffixStr);
        if (suffixStr.Length == 0)
          return str;
        int num = Mathf.Max(0, int.Parse(suffixStr) - 1);
        return baseStr + num.ToString("X" + (object) suffixStr.Length);
      }

      public static string IncrementString(string str)
      {
        string baseStr;
        string suffixStr;
        BraveUtility.SplitNumericSuffix(str, out baseStr, out suffixStr);
        if (suffixStr.Length == 0)
          return str;
        int num = Mathf.Max(0, int.Parse(suffixStr) + 1);
        return baseStr + num.ToString("X" + (object) suffixStr.Length);
      }

      public static void SplitNumericSuffix(string str, out string baseStr, out string suffixStr)
      {
        int num = 0;
        for (int index = str.Length - 1; index >= 0; --index)
        {
          if (!char.IsDigit(str[index]))
          {
            num = index + 1;
            break;
          }
        }
        if (num >= str.Length)
        {
          baseStr = str;
          suffixStr = string.Empty;
        }
        else
        {
          baseStr = str.Substring(0, num);
          suffixStr = str.Substring(num);
        }
      }

      public static List<int> GetPathCorners(List<IntVector2> path)
      {
        List<int> pathCorners = new List<int>();
        for (int index = 1; index < path.Count - 1; ++index)
        {
          IntVector2 intVector2_1 = path[index - 1];
          IntVector2 intVector2_2 = path[index];
          IntVector2 intVector2_3 = path[index + 1];
          if (intVector2_2 - intVector2_1 != intVector2_3 - intVector2_2)
            pathCorners.Add(index);
        }
        return pathCorners;
      }

      [DebuggerHidden]
      public static IEnumerable<T> Zip<A, B, T>(
        this IEnumerable<A> seqA,
        IEnumerable<B> seqB,
        Func<A, B, T> func)
      {
        // ISSUE: object of a compiler-generated type is created
        // ISSUE: variable of a compiler-generated type
        BraveUtility.<Zip>c__Iterator0<A, B, T> zipCIterator0 = new BraveUtility.<Zip>c__Iterator0<A, B, T>()
        {
          seqA = seqA,
          seqB = seqB,
          func = func
        };
        // ISSUE: reference to a compiler-generated field
        zipCIterator0.$PC = -2;
        return (IEnumerable<T>) zipCIterator0;
      }

      public static int GetNthIndexOf(string s, char t, int n)
      {
        int num = 0;
        for (int index = 0; index < s.Length; ++index)
        {
          if ((int) s[index] == (int) t)
          {
            ++num;
            if (num == n)
              return index;
          }
        }
        return -1;
      }

      public static void Swap<T>(ref T v1, ref T v2)
      {
        T obj = v1;
        v1 = v2;
        v2 = obj;
      }

      public static Color GetRainbowLerp(float t)
      {
        t %= 1f;
        t *= 6f;
        if ((double) t < 1.0)
          return Color.Lerp(Color.red, new Color(1f, 0.5f, 0.0f), t % 1f);
        if ((double) t < 2.0)
          return Color.Lerp(new Color(1f, 0.5f, 0.0f), Color.yellow, t % 1f);
        if ((double) t < 3.0)
          return Color.Lerp(Color.yellow, Color.green, t % 1f);
        if ((double) t < 4.0)
          return Color.Lerp(Color.green, Color.blue, t % 1f);
        if ((double) t < 5.0)
          return Color.Lerp(Color.blue, new Color(0.5f, 0.0f, 1f), t % 1f);
        return (double) t < 6.0 ? Color.Lerp(new Color(0.5f, 0.0f, 1f), Color.red, t % 1f) : Color.red;
      }

      public static Color GetRainbowColor(int index)
      {
        switch (index)
        {
          case 0:
            return Color.red;
          case 1:
            return new Color(1f, 0.5f, 0.0f, 1f);
          case 2:
            return Color.yellow;
          case 3:
            return Color.green;
          case 4:
            return Color.blue;
          case 5:
            return Color.magenta;
          case 6:
            return new Color(0.5f, 0.0f, 1f);
          case 7:
            return Color.grey;
          case 8:
            return Color.white;
          default:
            return Color.white;
        }
      }

      public static T[] AppendArray<T>(T[] oldArray, T newElement)
      {
        T[] destinationArray = new T[oldArray.Length + 1];
        Array.Copy((Array) oldArray, (Array) destinationArray, oldArray.Length);
        destinationArray[destinationArray.Length - 1] = newElement;
        return destinationArray;
      }

      public static int SequentialRandomRange(
        int min,
        int max,
        int lastValue,
        int? maxDistFromLast = null,
        bool excludeLastValue = false)
      {
        if (maxDistFromLast.HasValue)
        {
          min = Mathf.Max(min, lastValue - maxDistFromLast.Value);
          max = Mathf.Min(max, lastValue + maxDistFromLast.Value + 1);
        }
        if (excludeLastValue)
          --max;
        int num = UnityEngine.Random.Range(min, max);
        if (excludeLastValue && num >= lastValue)
          ++num;
        return num;
      }

      public static int SmartListResizer(
        int currentSize,
        int desiredSize,
        int minGrowingSize = 100,
        int forceMultipleOf = 0)
      {
        int num = currentSize != 0 ? (currentSize >= minGrowingSize || desiredSize >= minGrowingSize ? (desiredSize >= currentSize * 2 ? desiredSize + currentSize : currentSize * 2) : minGrowingSize) : desiredSize;
        if (forceMultipleOf > 0 && num % forceMultipleOf > 0)
          num += forceMultipleOf - num % forceMultipleOf;
        return num;
      }

      public static void EnableEmission(ParticleSystem ps, bool enabled)
      {
        ps.emission.enabled = enabled;
      }

      public static float GetEmissionRate(ParticleSystem ps) => ps.emission.rate.constant;

      public static void SetEmissionRate(ParticleSystem ps, float emissionRate)
      {
        ps.emission.rate = (ParticleSystem.MinMaxCurve) emissionRate;
      }

      public enum LogVerbosity
      {
        NONE,
        IMPORTANT,
        CHATTY,
        VERBOSE,
      }
    }

}
