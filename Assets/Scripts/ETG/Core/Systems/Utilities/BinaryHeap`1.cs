// Decompiled with JetBrains decompiler
// Type: BinaryHeap`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class BinaryHeap<T> : ICollection<T>, IEnumerable<T>, IEnumerable where T : IComparable<T>
    {
      private const int c_defaultSize = 4;
      private T[] m_data = new T[4];
      private int m_count;
      private int m_capacity = 4;
      private bool m_sorted;

      public BinaryHeap()
      {
      }

      private BinaryHeap(T[] data, int count)
      {
        this.Capacity = count;
        this.m_count = count;
        Array.Copy((Array) data, (Array) this.m_data, count);
      }

      public int Count => this.m_count;

      public int Capacity
      {
        get => this.m_capacity;
        set
        {
          int capacity = this.m_capacity;
          this.m_capacity = Math.Max(value, this.m_count);
          if (this.m_capacity == capacity)
            return;
          T[] destinationArray = new T[this.m_capacity];
          Array.Copy((Array) this.m_data, (Array) destinationArray, this.m_count);
          this.m_data = destinationArray;
        }
      }

      public T Peek() => this.m_data[0];

      public void Clear()
      {
        this.m_count = 0;
        this.m_data = new T[this.m_capacity];
      }

      public void Add(T item)
      {
        if (this.m_count == this.m_capacity)
          this.Capacity *= 2;
        this.m_data[this.m_count] = item;
        this.UpHeap();
        ++this.m_count;
      }

      public T Remove()
      {
        if (this.m_count == 0)
          throw new InvalidOperationException("Cannot remove item, heap is empty.");
        T obj = this.m_data[0];
        --this.m_count;
        this.m_data[0] = this.m_data[this.m_count];
        this.m_data[this.m_count] = default (T);
        this.DownHeap();
        return obj;
      }

      private void UpHeap()
      {
        this.m_sorted = false;
        int index1 = this.m_count;
        T obj = this.m_data[index1];
        for (int index2 = BinaryHeap<T>.Parent(index1); index2 > -1 && obj.CompareTo(this.m_data[index2]) < 0; index2 = BinaryHeap<T>.Parent(index1))
        {
          this.m_data[index1] = this.m_data[index2];
          index1 = index2;
        }
        this.m_data[index1] = obj;
      }

      private void DownHeap()
      {
        this.m_sorted = false;
        int index1 = 0;
        T obj = this.m_data[index1];
        while (true)
        {
          int index2 = BinaryHeap<T>.Child1(index1);
          if (index2 < this.m_count)
          {
            int index3 = BinaryHeap<T>.Child2(index1);
            int index4 = index3 < this.m_count ? (this.m_data[index2].CompareTo(this.m_data[index3]) >= 0 ? index3 : index2) : index2;
            if (obj.CompareTo(this.m_data[index4]) > 0)
            {
              this.m_data[index1] = this.m_data[index4];
              index1 = index4;
            }
            else
              break;
          }
          else
            break;
        }
        this.m_data[index1] = obj;
      }

      private void EnsureSort()
      {
        if (this.m_sorted)
          return;
        Array.Sort<T>(this.m_data, 0, this.m_count);
        this.m_sorted = true;
      }

      private static int Parent(int index) => index - 1 >> 1;

      private static int Child1(int index) => (index << 1) + 1;

      private static int Child2(int index) => (index << 1) + 2;

      public BinaryHeap<T> Copy() => new BinaryHeap<T>(this.m_data, this.m_count);

      [DebuggerHidden]
      public IEnumerator<T> GetEnumerator()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator<T>) new BinaryHeap<T>.<GetEnumerator>c__Iterator0()
        {
          $this = this
        };
      }

      IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

      public bool Contains(T item)
      {
        this.EnsureSort();
        return Array.BinarySearch<T>(this.m_data, 0, this.m_count, item) >= 0;
      }

      public void CopyTo(T[] array, int arrayIndex)
      {
        this.EnsureSort();
        Array.Copy((Array) this.m_data, (Array) array, this.m_count);
      }

      public bool IsReadOnly => false;

      public bool Remove(T item)
      {
        this.EnsureSort();
        int destinationIndex = Array.BinarySearch<T>(this.m_data, 0, this.m_count, item);
        if (destinationIndex < 0)
          return false;
        Array.Copy((Array) this.m_data, destinationIndex + 1, (Array) this.m_data, destinationIndex, this.m_count - destinationIndex);
        this.m_data[this.m_count] = default (T);
        --this.m_count;
        return true;
      }
    }

}
