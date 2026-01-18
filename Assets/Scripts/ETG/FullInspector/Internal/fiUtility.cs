using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;

#nullable disable
namespace FullInspector.Internal
{
  public static class fiUtility
  {
    private static bool? _cachedIsEditor;
    private static bool? _isUnity4;

    public static string CombinePaths(string a, string b) => Path.Combine(a, b).Replace('\\', '/');

    public static string CombinePaths(string a, string b, string c)
    {
      return Path.Combine(Path.Combine(a, b), c).Replace('\\', '/');
    }

    public static string CombinePaths(string a, string b, string c, string d)
    {
      return Path.Combine(Path.Combine(Path.Combine(a, b), c), d).Replace('\\', '/');
    }

    public static bool NearlyEqual(float a, float b) => fiUtility.NearlyEqual(a, b, float.Epsilon);

    public static bool NearlyEqual(float a, float b, float epsilon)
    {
      float num1 = Math.Abs(a);
      float num2 = Math.Abs(b);
      float num3 = Math.Abs(a - b);
      if ((double) a == (double) b)
        return true;
      return (double) a == 0.0 || (double) b == 0.0 || (double) num3 < -3.4028234663852886E+38 ? (double) num3 < (double) epsilon * double.MinValue : (double) num3 / ((double) num1 + (double) num2) < (double) epsilon;
    }

    public static void DestroyObject(UnityEngine.Object obj)
    {
      if (Application.isPlaying)
        UnityEngine.Object.Destroy(obj);
      else
        UnityEngine.Object.DestroyImmediate(obj, true);
    }

    public static void DestroyObject<T>(ref T obj) where T : UnityEngine.Object
    {
      fiUtility.DestroyObject((UnityEngine.Object) obj);
      obj = (T) null;
    }

    public static string StripLeadingWhitespace(this string s)
    {
      return new Regex("^\\s+", RegexOptions.Multiline).Replace(s, string.Empty);
    }

    public static bool IsEditor
    {
      get
      {
        if (!fiUtility._cachedIsEditor.HasValue)
          fiUtility._cachedIsEditor = new bool?(System.Type.GetType("UnityEditor.Editor, UnityEditor", false) != null);
        return fiUtility._cachedIsEditor.Value;
      }
    }

    public static bool IsMainThread
    {
      get
      {
        if (!fiUtility.IsEditor)
          throw new InvalidOperationException("Only available in the editor");
        return Thread.CurrentThread.ManagedThreadId == 1;
      }
    }

    public static bool IsUnity4
    {
      get
      {
        if (!fiUtility._isUnity4.HasValue)
          fiUtility._isUnity4 = new bool?(System.Type.GetType("UnityEngine.RuntimeInitializeOnLoadMethodAttribute, UnityEngine", false) == null);
        return fiUtility._isUnity4.Value;
      }
    }

    public static Dictionary<TKey, TValue> CreateDictionary<TKey, TValue>(
      IList<TKey> keys,
      IList<TValue> values)
    {
      Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
      if (keys != null && values != null)
      {
        for (int index = 0; index < Mathf.Min(keys.Count, values.Count); ++index)
        {
          if (!object.ReferenceEquals((object) keys[index], (object) null))
            dictionary[keys[index]] = values[index];
        }
      }
      return dictionary;
    }

    public static void Swap<T>(ref T a, ref T b)
    {
      T obj = a;
      a = b;
      b = obj;
    }
  }
}
