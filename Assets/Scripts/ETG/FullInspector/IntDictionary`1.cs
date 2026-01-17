// Decompiled with JetBrains decompiler
// Type: FullInspector.IntDictionary`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullInspector.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

#nullable disable
namespace FullInspector;

internal class IntDictionary<TValue> : 
  IDictionary<int, TValue>,
  ICollection<KeyValuePair<int, TValue>>,
  IEnumerable<KeyValuePair<int, TValue>>,
  IEnumerable
{
  private List<fiOption<TValue>> _positives = new List<fiOption<TValue>>();
  private Dictionary<int, TValue> _negatives = new Dictionary<int, TValue>();

  public void Add(int key, TValue value)
  {
    if (key < 0)
    {
      this._negatives.Add(key, value);
    }
    else
    {
      while (key >= this._positives.Count)
        this._positives.Add(fiOption<TValue>.Empty);
      if (this._positives[key].HasValue)
        throw new Exception("Already have a key for " + (object) key);
      this._positives[key] = fiOption.Just<TValue>(value);
    }
  }

  public bool ContainsKey(int key)
  {
    if (key < 0)
      return this._negatives.ContainsKey(key);
    return key < this._positives.Count && this._positives[key].HasValue;
  }

  public ICollection<int> Keys => throw new NotSupportedException();

  public bool Remove(int key)
  {
    if (key < 0)
      return this._negatives.Remove(key);
    if (key >= this._positives.Count || this._positives[key].IsEmpty)
      return false;
    this._positives[key] = fiOption<TValue>.Empty;
    return true;
  }

  public bool TryGetValue(int key, out TValue value)
  {
    if (key < 0)
      return this._negatives.TryGetValue(key, out value);
    value = default (TValue);
    if (key >= this._positives.Count || this._positives[key].IsEmpty)
      return false;
    value = this._positives[key].Value;
    return true;
  }

  public ICollection<TValue> Values => throw new NotSupportedException();

  public TValue this[int key]
  {
    get
    {
      if (key < 0)
        return this._negatives[key];
      if (key >= this._positives.Count)
        throw new KeyNotFoundException(string.Empty + (object) key);
      if (this._positives[key].IsEmpty)
        throw new KeyNotFoundException(string.Empty + (object) key);
      return this._positives[key].Value;
    }
    set
    {
      if (key < 0)
      {
        this._negatives[key] = value;
      }
      else
      {
        while (key >= this._positives.Count)
          this._positives.Add(fiOption<TValue>.Empty);
        this._positives[key] = fiOption.Just<TValue>(value);
      }
    }
  }

  public void Add(KeyValuePair<int, TValue> item) => this.Add(item.Key, item.Value);

  public void Clear()
  {
    this._negatives.Clear();
    this._positives.Clear();
  }

  public bool Contains(KeyValuePair<int, TValue> item) => throw new NotSupportedException();

  public void CopyTo(KeyValuePair<int, TValue>[] array, int arrayIndex)
  {
    foreach (KeyValuePair<int, TValue> keyValuePair in this)
    {
      if (arrayIndex >= array.Length)
        break;
      array[arrayIndex++] = keyValuePair;
    }
  }

  public int Count
  {
    get
    {
      int count = this._negatives.Count;
      for (int index = 0; index < this._positives.Count; ++index)
      {
        if (this._positives[index].HasValue)
          ++count;
      }
      return count;
    }
  }

  public bool IsReadOnly => throw new NotSupportedException();

  public bool Remove(KeyValuePair<int, TValue> item) => throw new NotSupportedException();

  [DebuggerHidden]
  public IEnumerator<KeyValuePair<int, TValue>> GetEnumerator()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<KeyValuePair<int, TValue>>) new IntDictionary<TValue>.<GetEnumerator>c__Iterator0()
    {
      _this = this
    };
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
}
