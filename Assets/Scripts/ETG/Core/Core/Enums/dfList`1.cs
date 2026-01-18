using System;
using System.Collections;
using System.Collections.Generic;

#nullable disable

public class dfList<T> : 
    IList<T>,
    IDisposable,
    IPoolable,
    ICollection<T>,
    IEnumerable<T>,
    IEnumerable
  {
    private static Queue<object> pool = new Queue<object>(1024 /*0x0400*/);
    private const int DEFAULT_CAPACITY = 128 /*0x80*/;
    private T[] items = new T[128 /*0x80*/];
    private int count;
    private bool isElementTypeValueType;
    private bool isElementTypePoolable;
    private bool autoReleaseItems;

    internal dfList()
    {
      this.isElementTypeValueType = typeof (T).IsValueType;
      this.isElementTypePoolable = typeof (IPoolable).IsAssignableFrom(typeof (T));
    }

    internal dfList(IList<T> listToClone)
      : this()
    {
      this.AddRange(listToClone);
    }

    internal dfList(int capacity)
      : this()
    {
      this.EnsureCapacity(capacity);
    }

    public static void ClearPool()
    {
      lock ((object) dfList<T>.pool)
      {
        dfList<T>.pool.Clear();
        dfList<T>.pool.TrimExcess();
      }
    }

    public static dfList<T> Obtain()
    {
      lock ((object) dfList<T>.pool)
        return dfList<T>.pool.Count == 0 ? new dfList<T>() : (dfList<T>) dfList<T>.pool.Dequeue();
    }

    public static dfList<T> Obtain(int capacity)
    {
      dfList<T> dfList = dfList<T>.Obtain();
      dfList.EnsureCapacity(capacity);
      return dfList;
    }

    public void ReleaseItems()
    {
      if (!this.isElementTypePoolable)
        throw new InvalidOperationException($"Element type {typeof (T).Name} does not implement the {typeof (IPoolable).Name} interface");
      for (int index = 0; index < this.count; ++index)
        ((object) this.items[index] as IPoolable).Release();
      this.Clear();
    }

    public void Release()
    {
      lock ((object) dfList<T>.pool)
      {
        if (this.autoReleaseItems && this.isElementTypePoolable)
        {
          this.autoReleaseItems = false;
          this.ReleaseItems();
        }
        else
          this.Clear();
        dfList<T>.pool.Enqueue((object) this);
      }
    }

    public bool AutoReleaseItems
    {
      get => this.autoReleaseItems;
      set => this.autoReleaseItems = value;
    }

    public int Count => this.count;

    internal int Capacity => this.items.Length;

    public bool IsReadOnly => false;

    public T this[int index]
    {
      get
      {
        return index >= 0 && index <= this.count - 1 ? this.items[index] : throw new IndexOutOfRangeException();
      }
      set
      {
        if (index < 0 || index > this.count - 1)
          throw new IndexOutOfRangeException();
        this.items[index] = value;
      }
    }

    internal T[] Items => this.items;

    public void Enqueue(T item)
    {
      lock ((object) this.items)
        this.Add(item);
    }

    public T Dequeue()
    {
      lock ((object) this.items)
      {
        if (this.count == 0)
          throw new IndexOutOfRangeException();
        T obj = this.items[0];
        this.RemoveAt(0);
        return obj;
      }
    }

    public T Pop()
    {
      lock ((object) this.items)
      {
        T obj = this.count != 0 ? this.items[this.count - 1] : throw new IndexOutOfRangeException();
        this.items[this.count - 1] = default (T);
        --this.count;
        return obj;
      }
    }

    public dfList<T> Clone()
    {
      dfList<T> dfList = dfList<T>.Obtain(this.count);
      Array.Copy((Array) this.items, 0, (Array) dfList.items, 0, this.count);
      dfList.count = this.count;
      return dfList;
    }

    public void Reverse() => Array.Reverse((Array) this.items, 0, this.count);

    public void Sort() => Array.Sort<T>(this.items, 0, this.count, (IComparer<T>) null);

    public void Sort(IComparer<T> comparer) => Array.Sort<T>(this.items, 0, this.count, comparer);

    public void Sort(Comparison<T> comparison)
    {
      if (comparison == null)
        throw new ArgumentNullException(nameof (comparison));
      if (this.count <= 0)
        return;
      using (dfList<T>.FunctorComparer functorComparer = dfList<T>.FunctorComparer.Obtain(comparison))
        Array.Sort<T>(this.items, 0, this.count, (IComparer<T>) functorComparer);
    }

    public void EnsureCapacity(int Size)
    {
      if (this.items.Length >= Size)
        return;
      Array.Resize<T>(ref this.items, Size / 128 /*0x80*/ * 128 /*0x80*/ + 128 /*0x80*/);
    }

    public void AddRange(dfList<T> list)
    {
      int count = list.count;
      this.EnsureCapacity(this.count + count);
      Array.Copy((Array) list.items, 0, (Array) this.items, this.count, count);
      this.count += count;
    }

    public void AddRange(IList<T> list)
    {
      int count = list.Count;
      this.EnsureCapacity(this.count + count);
      for (int index = 0; index < count; ++index)
        this.items[this.count++] = list[index];
    }

    public void AddRange(T[] list)
    {
      int length = list.Length;
      this.EnsureCapacity(this.count + length);
      Array.Copy((Array) list, 0, (Array) this.items, this.count, length);
      this.count += length;
    }

    public int IndexOf(T item) => Array.IndexOf<T>(this.items, item, 0, this.count);

    public void Insert(int index, T item)
    {
      this.EnsureCapacity(this.count + 1);
      if (index < this.count)
        Array.Copy((Array) this.items, index, (Array) this.items, index + 1, this.count - index);
      this.items[index] = item;
      ++this.count;
    }

    public void InsertRange(int index, T[] array)
    {
      if (array == null)
        throw new ArgumentNullException("items");
      if (index < 0 || index > this.count)
        throw new ArgumentOutOfRangeException(nameof (index));
      this.EnsureCapacity(this.count + array.Length);
      if (index < this.count)
        Array.Copy((Array) this.items, index, (Array) this.items, index + array.Length, this.count - index);
      array.CopyTo((Array) this.items, index);
      this.count += array.Length;
    }

    public void InsertRange(int index, dfList<T> list)
    {
      if (list == null)
        throw new ArgumentNullException("items");
      if (index < 0 || index > this.count)
        throw new ArgumentOutOfRangeException(nameof (index));
      this.EnsureCapacity(this.count + list.count);
      if (index < this.count)
        Array.Copy((Array) this.items, index, (Array) this.items, index + list.count, this.count - index);
      Array.Copy((Array) list.items, 0, (Array) this.items, index, list.count);
      this.count += list.count;
    }

    public void RemoveAll(Predicate<T> predicate)
    {
      int index = 0;
      while (index < this.count)
      {
        if (predicate(this.items[index]))
          this.RemoveAt(index);
        else
          ++index;
      }
    }

    public void RemoveAt(int index)
    {
      if (index >= this.count)
        throw new ArgumentOutOfRangeException();
      --this.count;
      if (index < this.count)
        Array.Copy((Array) this.items, index + 1, (Array) this.items, index, this.count - index);
      this.items[this.count] = default (T);
    }

    public void RemoveRange(int index, int length)
    {
      if (index < 0 || length < 0 || this.count - index < length)
        throw new ArgumentOutOfRangeException();
      if (this.count <= 0)
        return;
      this.count -= length;
      if (index < this.count)
        Array.Copy((Array) this.items, index + length, (Array) this.items, index, this.count - index);
      Array.Clear((Array) this.items, this.count, length);
    }

    public void Add(T item)
    {
      this.EnsureCapacity(this.count + 1);
      this.items[this.count++] = item;
    }

    public void Add(T item0, T item1)
    {
      this.EnsureCapacity(this.count + 2);
      this.items[this.count++] = item0;
      this.items[this.count++] = item1;
    }

    public void Add(T item0, T item1, T item2)
    {
      this.EnsureCapacity(this.count + 3);
      this.items[this.count++] = item0;
      this.items[this.count++] = item1;
      this.items[this.count++] = item2;
    }

    public void Clear()
    {
      if (!this.isElementTypeValueType)
        Array.Clear((Array) this.items, 0, this.items.Length);
      this.count = 0;
    }

    public void TrimExcess() => Array.Resize<T>(ref this.items, this.count);

    public bool Contains(T item)
    {
      if ((object) item == null)
      {
        for (int index = 0; index < this.count; ++index)
        {
          if ((object) this.items[index] == null)
            return true;
        }
        return false;
      }
      EqualityComparer<T> equalityComparer = EqualityComparer<T>.Default;
      for (int index = 0; index < this.count; ++index)
      {
        if (equalityComparer.Equals(this.items[index], item))
          return true;
      }
      return false;
    }

    public void CopyTo(T[] array) => this.CopyTo(array, 0);

    public void CopyTo(T[] array, int arrayIndex)
    {
      Array.Copy((Array) this.items, 0, (Array) array, arrayIndex, this.count);
    }

    public void CopyTo(int sourceIndex, T[] dest, int destIndex, int length)
    {
      if (sourceIndex + length > this.count)
        throw new IndexOutOfRangeException(nameof (sourceIndex));
      if (dest == null)
        throw new ArgumentNullException(nameof (dest));
      if (destIndex + length > dest.Length)
        throw new IndexOutOfRangeException(nameof (destIndex));
      Array.Copy((Array) this.items, sourceIndex, (Array) dest, destIndex, length);
    }

    public bool Remove(T item)
    {
      int index = this.IndexOf(item);
      if (index == -1)
        return false;
      this.RemoveAt(index);
      return true;
    }

    public List<T> ToList()
    {
      List<T> list = new List<T>(this.count);
      list.AddRange((IEnumerable<T>) this.ToArray());
      return list;
    }

    public T[] ToArray()
    {
      T[] destinationArray = new T[this.count];
      Array.Copy((Array) this.items, 0, (Array) destinationArray, 0, this.count);
      return destinationArray;
    }

    public T[] ToArray(int index, int length)
    {
      T[] dest = new T[this.count];
      if (this.count > 0)
        this.CopyTo(index, dest, 0, length);
      return dest;
    }

    public dfList<T> GetRange(int index, int length)
    {
      dfList<T> range = dfList<T>.Obtain(length);
      this.CopyTo(0, range.items, index, length);
      return range;
    }

    public bool Any(Func<T, bool> predicate)
    {
      for (int index = 0; index < this.count; ++index)
      {
        if (predicate(this.items[index]))
          return true;
      }
      return false;
    }

    public T First()
    {
      if (this.count == 0)
        throw new IndexOutOfRangeException();
      return this.items[0];
    }

    public T FirstOrDefault() => this.count > 0 ? this.items[0] : default (T);

    public T FirstOrDefault(Func<T, bool> predicate)
    {
      for (int index = 0; index < this.count; ++index)
      {
        if (predicate(this.items[index]))
          return this.items[index];
      }
      return default (T);
    }

    public T Last()
    {
      return this.count != 0 ? this.items[this.count - 1] : throw new IndexOutOfRangeException();
    }

    public T LastOrDefault() => this.count == 0 ? default (T) : this.items[this.count - 1];

    public T LastOrDefault(Func<T, bool> predicate)
    {
      T obj = default (T);
      for (int index = 0; index < this.count; ++index)
      {
        if (predicate(this.items[index]))
          obj = this.items[index];
      }
      return obj;
    }

    public dfList<T> Where(Func<T, bool> predicate)
    {
      dfList<T> dfList = dfList<T>.Obtain(this.count);
      for (int index = 0; index < this.count; ++index)
      {
        if (predicate(this.items[index]))
          dfList.Add(this.items[index]);
      }
      return dfList;
    }

    public int Matching(Func<T, bool> predicate)
    {
      int num = 0;
      for (int index = 0; index < this.count; ++index)
      {
        if (predicate(this.items[index]))
          ++num;
      }
      return num;
    }

    public dfList<TResult> Select<TResult>(Func<T, TResult> selector)
    {
      dfList<TResult> dfList = dfList<TResult>.Obtain(this.count);
      for (int index = 0; index < this.count; ++index)
        dfList.Add(selector(this.items[index]));
      return dfList;
    }

    public dfList<T> Concat(dfList<T> list)
    {
      dfList<T> dfList = dfList<T>.Obtain(this.count + list.count);
      dfList.AddRange(this);
      dfList.AddRange(list);
      return dfList;
    }

    public dfList<TResult> Convert<TResult>()
    {
      dfList<TResult> dfList = dfList<TResult>.Obtain(this.count);
      for (int index = 0; index < this.count; ++index)
        dfList.Add((TResult) System.Convert.ChangeType((object) this.items[index], typeof (TResult)));
      return dfList;
    }

    public void ForEach(Action<T> action)
    {
      int num = 0;
      while (num < this.Count)
        action(this.items[num++]);
    }

    public IEnumerator<T> GetEnumerator()
    {
      return (IEnumerator<T>) dfList<T>.PooledEnumerator.Obtain(this, (Func<T, bool>) null);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) dfList<T>.PooledEnumerator.Obtain(this, (Func<T, bool>) null);
    }

    public void Dispose() => this.Release();

    private class PooledEnumerator : 
      IEnumerator<T>,
      IEnumerable<T>,
      IEnumerator,
      IDisposable,
      IEnumerable
    {
      private static Queue<dfList<T>.PooledEnumerator> pool = new Queue<dfList<T>.PooledEnumerator>();
      private dfList<T> list;
      private Func<T, bool> predicate;
      private int currentIndex;
      private T currentValue;
      private bool isValid;

      public static dfList<T>.PooledEnumerator Obtain(dfList<T> list, Func<T, bool> predicate)
      {
        dfList<T>.PooledEnumerator pooledEnumerator = dfList<T>.PooledEnumerator.pool.Count <= 0 ? new dfList<T>.PooledEnumerator() : dfList<T>.PooledEnumerator.pool.Dequeue();
        pooledEnumerator.ResetInternal(list, predicate);
        return pooledEnumerator;
      }

      public void Release()
      {
        if (!this.isValid)
          return;
        this.isValid = false;
        dfList<T>.PooledEnumerator.pool.Enqueue(this);
      }

      public T Current
      {
        get
        {
          if (!this.isValid)
            throw new InvalidOperationException("The enumerator is no longer valid");
          return this.currentValue;
        }
      }

      private void ResetInternal(dfList<T> list, Func<T, bool> predicate)
      {
        this.isValid = true;
        this.list = list;
        this.predicate = predicate;
        this.currentIndex = 0;
        this.currentValue = default (T);
      }

      public void Dispose() => this.Release();

      object IEnumerator.Current => (object) this.Current;

      public bool MoveNext()
      {
        if (!this.isValid)
          throw new InvalidOperationException("The enumerator is no longer valid");
        while (this.currentIndex < this.list.Count)
        {
          T obj = this.list[this.currentIndex++];
          if (this.predicate == null || this.predicate(obj))
          {
            this.currentValue = obj;
            return true;
          }
        }
        this.Release();
        this.currentValue = default (T);
        return false;
      }

      public void Reset() => throw new NotImplementedException();

      public IEnumerator<T> GetEnumerator() => (IEnumerator<T>) this;

      IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this;
    }

    private class FunctorComparer : IComparer<T>, IDisposable
    {
      private static Queue<dfList<T>.FunctorComparer> pool = new Queue<dfList<T>.FunctorComparer>();
      private Comparison<T> comparison;

      public static dfList<T>.FunctorComparer Obtain(Comparison<T> comparison)
      {
        dfList<T>.FunctorComparer functorComparer = dfList<T>.FunctorComparer.pool.Count <= 0 ? new dfList<T>.FunctorComparer() : dfList<T>.FunctorComparer.pool.Dequeue();
        functorComparer.comparison = comparison;
        return functorComparer;
      }

      public void Release()
      {
        this.comparison = (Comparison<T>) null;
        if (dfList<T>.FunctorComparer.pool.Contains(this))
          return;
        dfList<T>.FunctorComparer.pool.Enqueue(this);
      }

      public int Compare(T x, T y) => this.comparison(x, y);

      public void Dispose() => this.Release();
    }
  }

