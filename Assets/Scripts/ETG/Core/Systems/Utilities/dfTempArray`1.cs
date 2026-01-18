// Decompiled with JetBrains decompiler
// Type: dfTempArray`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;

#nullable disable

internal class dfTempArray<T>
  {
    private static List<T[]> cache = new List<T[]>(32 /*0x20*/);

    public static void Clear() => dfTempArray<T>.cache.Clear();

    public static T[] Obtain(int length) => dfTempArray<T>.Obtain(length, 128 /*0x80*/);

    public static T[] Obtain(int length, int maxCacheSize)
    {
      lock ((object) dfTempArray<T>.cache)
      {
        for (int index = 0; index < dfTempArray<T>.cache.Count; ++index)
        {
          T[] objArray = dfTempArray<T>.cache[index];
          if (objArray.Length == length)
          {
            if (index > 0)
            {
              dfTempArray<T>.cache.RemoveAt(index);
              dfTempArray<T>.cache.Insert(0, objArray);
            }
            return objArray;
          }
        }
        if (dfTempArray<T>.cache.Count >= maxCacheSize)
          dfTempArray<T>.cache.RemoveAt(dfTempArray<T>.cache.Count - 1);
        T[] objArray1 = new T[length];
        dfTempArray<T>.cache.Insert(0, objArray1);
        return objArray1;
      }
    }
  }

