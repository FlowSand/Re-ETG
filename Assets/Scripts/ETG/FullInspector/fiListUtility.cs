using System;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace FullInspector
{
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
}
