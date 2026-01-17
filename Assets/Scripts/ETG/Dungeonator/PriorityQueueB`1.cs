// Decompiled with JetBrains decompiler
// Type: Dungeonator.PriorityQueueB`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;

#nullable disable
namespace Dungeonator
{
  public class PriorityQueueB<T> : IPriorityQueue<T>
  {
    protected List<T> InnerList = new List<T>();
    protected IComparer<T> mComparer;

    public PriorityQueueB() => this.mComparer = (IComparer<T>) Comparer<T>.Default;

    public PriorityQueueB(IComparer<T> comparer) => this.mComparer = comparer;

    public PriorityQueueB(IComparer<T> comparer, int capacity)
    {
      this.mComparer = comparer;
      this.InnerList.Capacity = capacity;
    }

    protected void SwitchElements(int i, int j)
    {
      T inner = this.InnerList[i];
      this.InnerList[i] = this.InnerList[j];
      this.InnerList[j] = inner;
    }

    protected virtual int OnCompare(int i, int j)
    {
      return this.mComparer.Compare(this.InnerList[i], this.InnerList[j]);
    }

    public int Push(T item)
    {
      int i = this.InnerList.Count;
      this.InnerList.Add(item);
      int j;
      for (; i != 0; i = j)
      {
        j = (i - 1) / 2;
        if (this.OnCompare(i, j) < 0)
          this.SwitchElements(i, j);
        else
          break;
      }
      return i;
    }

    public T Pop()
    {
      T inner = this.InnerList[0];
      int i = 0;
      this.InnerList[0] = this.InnerList[this.InnerList.Count - 1];
      this.InnerList.RemoveAt(this.InnerList.Count - 1);
      while (true)
      {
        int j1 = i;
        int j2 = 2 * i + 1;
        int j3 = 2 * i + 2;
        if (this.InnerList.Count > j2 && this.OnCompare(i, j2) > 0)
          i = j2;
        if (this.InnerList.Count > j3 && this.OnCompare(i, j3) > 0)
          i = j3;
        if (i != j1)
          this.SwitchElements(i, j1);
        else
          break;
      }
      return inner;
    }

    public void Update(int i)
    {
      int i1;
      int j1;
      for (i1 = i; i1 != 0; i1 = j1)
      {
        j1 = (i1 - 1) / 2;
        if (this.OnCompare(i1, j1) < 0)
          this.SwitchElements(i1, j1);
        else
          break;
      }
      if (i1 < i)
        return;
      while (true)
      {
        int j2 = i1;
        int j3 = 2 * i1 + 1;
        int j4 = 2 * i1 + 2;
        if (this.InnerList.Count > j3 && this.OnCompare(i1, j3) > 0)
          i1 = j3;
        if (this.InnerList.Count > j4 && this.OnCompare(i1, j4) > 0)
          i1 = j4;
        if (i1 != j2)
          this.SwitchElements(i1, j2);
        else
          break;
      }
    }

    public T Peek() => this.InnerList.Count > 0 ? this.InnerList[0] : default (T);

    public void Clear() => this.InnerList.Clear();

    public int Count => this.InnerList.Count;

    public void RemoveLocation(T item)
    {
      int index1 = -1;
      for (int index2 = 0; index2 < this.InnerList.Count; ++index2)
      {
        if (this.mComparer.Compare(this.InnerList[index2], item) == 0)
          index1 = index2;
      }
      if (index1 == -1)
        return;
      this.InnerList.RemoveAt(index1);
    }

    public T this[int index]
    {
      get => this.InnerList[index];
      set
      {
        this.InnerList[index] = value;
        this.Update(index);
      }
    }
  }
}
