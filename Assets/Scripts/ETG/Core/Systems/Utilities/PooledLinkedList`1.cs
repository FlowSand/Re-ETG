// Decompiled with JetBrains decompiler
// Type: PooledLinkedList`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class PooledLinkedList<T> : ICollection<T>, IEnumerable<T>, IEnumerable
    {
      private LinkedList<T> m_list = new LinkedList<T>();
      private LinkedList<T> m_pool = new LinkedList<T>();

      public void ClearPool() => this.m_pool.Clear();

      public LinkedListNode<T> GetByIndexSlow(int index)
      {
        LinkedListNode<T> byIndexSlow = this.m_list.Count != 0 ? this.m_list.First : throw new IndexOutOfRangeException();
        for (int index1 = 0; index1 < index; ++index1)
        {
          byIndexSlow = byIndexSlow.Next;
          if (byIndexSlow == null)
            throw new IndexOutOfRangeException();
        }
        return byIndexSlow;
      }

      public void AddAfter(LinkedListNode<T> node, T value) => this.m_list.AddAfter(node, value);

      public void AddAfter(LinkedListNode<T> node, LinkedListNode<T> newNode)
      {
        this.m_list.AddAfter(node, newNode);
      }

      public void AddBefore(LinkedListNode<T> node, T value) => this.m_list.AddBefore(node, value);

      public void AddBefore(LinkedListNode<T> node, LinkedListNode<T> newNode)
      {
        this.m_list.AddBefore(node, newNode);
      }

      public void AddFirst(T value)
      {
        if (this.m_pool.Count > 0)
        {
          LinkedListNode<T> first = this.m_pool.First;
          this.m_pool.RemoveFirst();
          first.Value = value;
          this.m_list.AddFirst(first);
        }
        else
          this.m_list.AddFirst(value);
      }

      public void AddFirst(LinkedListNode<T> node) => this.m_list.AddFirst(node);

      public void AddLast(T value)
      {
        if (this.m_pool.Count > 0)
        {
          LinkedListNode<T> first = this.m_pool.First;
          this.m_pool.RemoveFirst();
          first.Value = value;
          this.m_list.AddLast(first);
        }
        else
          this.m_list.AddLast(value);
      }

      public void AddLast(LinkedListNode<T> node) => this.m_list.AddLast(node);

      public bool Remove(T value)
      {
        LinkedListNode<T> node = this.m_list.Find(value);
        if (node == null)
          return false;
        this.m_list.Remove(node);
        node.Value = default (T);
        this.m_pool.AddLast(node);
        return true;
      }

      public void Remove(LinkedListNode<T> node, bool returnToPool)
      {
        this.m_list.Remove(node);
        if (!returnToPool)
          return;
        node.Value = default (T);
        this.m_pool.AddLast(node);
      }

      public void RemoveFirst()
      {
        LinkedListNode<T> first = this.m_list.First;
        this.m_list.Remove(first);
        first.Value = default (T);
        this.m_pool.AddLast(first);
      }

      public void RemoveLast()
      {
        LinkedListNode<T> last = this.m_list.Last;
        this.m_list.Remove(last);
        last.Value = default (T);
        this.m_pool.AddLast(last);
      }

      public LinkedListNode<T> First => this.m_list.First;

      public LinkedListNode<T> Last => this.m_list.Last;

      public int Count => this.m_list.Count;

      public bool IsReadOnly => ((ICollection<T>) this.m_list).IsReadOnly;

      void ICollection<T>.Add(T item) => this.AddLast(item);

      public void Clear()
      {
        while (this.m_list.Count > 0)
          this.RemoveLast();
      }

      public bool Contains(T item) => this.m_list.Contains(item);

      public void CopyTo(T[] array, int arrayIndex) => this.m_list.CopyTo(array, arrayIndex);

      public IEnumerator<T> GetEnumerator() => (IEnumerator<T>) this.m_list.GetEnumerator();

      IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) this.m_list).GetEnumerator();
    }

}
