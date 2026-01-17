// Decompiled with JetBrains decompiler
// Type: FullInspector.fiListUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace FullInspector;

public static class fiListUtility
{
  public static void Add<T>(ref IList list)
  {
    if (list.GetType().IsArray)
    {
      T[] array = (T[]) list;
      Array.Resize<T>(ref array, array.Length + 1);
      list = (IList) array;
    }
    else
      list.Add((object) default (T));
  }

  public static void InsertAt<T>(ref IList list, int index)
  {
    if (list.GetType().IsArray)
    {
      List<T> objList = new List<T>((IEnumerable<T>) list);
      objList.Insert(index, default (T));
      list = (IList) objList.ToArray();
    }
    else
      list.Insert(index, (object) default (T));
  }

  public static void RemoveAt<T>(ref IList list, int index)
  {
    if (list.GetType().IsArray)
    {
      List<T> objList = new List<T>((IEnumerable<T>) list);
      objList.RemoveAt(index);
      list = (IList) objList.ToArray();
    }
    else
      list.RemoveAt(index);
  }
}
