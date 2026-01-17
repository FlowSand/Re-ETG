// Decompiled with JetBrains decompiler
// Type: KeyValueList`2
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
    public class KeyValueList<K, V> : IList, ICollection, IEnumerable
    {
      private List<K> keyList = new List<K>();
      private List<V> valList = new List<V>();

      public KeyValueList()
      {
      }

      public KeyValueList(ref List<K> keyListRef, ref List<V> valListRef)
      {
        this.keyList = keyListRef;
        this.valList = valListRef;
      }

      public KeyValueList(KeyValueList<K, V> otherKeyValueList) => this.AddRange(otherKeyValueList);

      public bool TryGetValue(K key, out V value)
      {
        int index = this.keyList.IndexOf(key);
        if (index == -1)
        {
          value = default (V);
          return false;
        }
        value = this.valList[index];
        return true;
      }

      public int Add(object value)
      {
        throw new NotImplementedException("Use KeyValueList[key] = value or insert");
      }

      public void Clear()
      {
        this.keyList.Clear();
        this.valList.Clear();
      }

      public bool Contains(V value) => this.valList.Contains(value);

      public bool ContainsKey(K key) => this.keyList.Contains(key);

      public int IndexOf(K key) => this.keyList.IndexOf(key);

      public void Insert(int index, K key, V value)
      {
        if (this.keyList.Contains(key))
          throw new Exception("Cannot insert duplicate key.");
        this.keyList.Insert(index, key);
        this.valList.Insert(index, value);
      }

      public void Insert(int index, KeyValuePair<K, V> kvp) => this.Insert(index, kvp.Key, kvp.Value);

      public void Insert(int index, object value)
      {
        throw new NotImplementedException("Use Insert(K key, V value) or Insert(KeyValuePair<K, V>)");
      }

      public void Remove(K key)
      {
        int index = this.keyList.IndexOf(key);
        if (index == -1)
          throw new KeyNotFoundException();
        this.keyList.RemoveAt(index);
        this.valList.RemoveAt(index);
      }

      public void Remove(object value) => throw new NotImplementedException("Use Remove(K key)");

      public void RemoveAt(int index)
      {
        this.keyList.RemoveAt(index);
        this.valList.RemoveAt(index);
      }

      public V this[K key]
      {
        get
        {
          V v;
          if (this.TryGetValue(key, out v))
            return v;
          throw new KeyNotFoundException();
        }
        set
        {
          int index = this.keyList.IndexOf(key);
          if (index == -1)
          {
            this.keyList.Add(key);
            this.valList.Add(value);
          }
          else
            this.valList[index] = value;
        }
      }

      public V GetAt(int index)
      {
        if (index >= this.valList.Count)
          throw new IndexOutOfRangeException();
        return this.valList[index];
      }

      public void SetAt(int index, V value)
      {
        if (index >= this.valList.Count)
          throw new IndexOutOfRangeException();
        this.valList[index] = value;
      }

      public void CopyTo(V[] array, int index) => this.valList.CopyTo(array, index);

      public void CopyTo(KeyValueList<K, V> otherKeyValueList, int index)
      {
        foreach (KeyValuePair<K, V> keyValuePair in this)
          otherKeyValueList[keyValuePair.Key] = keyValuePair.Value;
      }

      public void AddRange(KeyValueList<K, V> otherKeyValueList) => otherKeyValueList.CopyTo(this, 0);

      public int Count => this.valList.Count;

      [DebuggerHidden]
      public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator<KeyValuePair<K, V>>) new KeyValueList<K, V>.<GetEnumerator>c__Iterator1()
        {
          _this = this
        };
      }

      [DebuggerHidden]
      IEnumerator IEnumerable.GetEnumerator()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new KeyValueList<K, V>.<System_Collections_IEnumerable_GetEnumerator>c__Iterator0()
        {
          _this = this
        };
      }

      public override string ToString()
      {
        string[] strArray = new string[this.keyList.Count];
        string format = "{0}:{1}";
        int index = 0;
        foreach (KeyValuePair<K, V> keyValuePair in this)
        {
          strArray[index] = string.Format(format, (object) keyValuePair.Key, (object) keyValuePair.Value);
          ++index;
        }
        return $"[{string.Join(", ", strArray)}]";
      }

      public bool IsFixedSize => false;

      public bool IsReadOnly => false;

      public bool IsSynchronized => throw new NotImplementedException();

      public object SyncRoot => throw new NotImplementedException();

      public bool Contains(object value) => throw new NotImplementedException();

      public int IndexOf(object value) => throw new NotImplementedException();

      object IList.this[int index]
      {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
      }

      public void CopyTo(Array array, int index) => throw new NotImplementedException();
    }

}
