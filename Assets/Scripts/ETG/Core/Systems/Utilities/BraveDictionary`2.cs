using System;
using System.Collections.Generic;

#nullable disable

public class BraveDictionary<TKey, TValue>
  {
    private List<TKey> m_keys = new List<TKey>();
    private List<TValue> m_values = new List<TValue>();

    public int Count => this.m_keys.Count;

    public List<TKey> Keys => this.m_keys;

    public List<TValue> Values => this.m_values;

    public bool TryGetValue(TKey key, out TValue value)
    {
      value = default (TValue);
      if ((object) key == null)
        return false;
      for (int index = 0; index < this.m_keys.Count; ++index)
      {
        if (this.m_keys[index].Equals((object) key))
        {
          value = this.m_values[index];
          return true;
        }
      }
      return false;
    }

    public TValue this[TKey key]
    {
      get
      {
        if ((object) key == null)
          throw new ArgumentNullException();
        for (int index = 0; index < this.m_keys.Count; ++index)
        {
          if (this.m_keys[index].Equals((object) key))
            return this.m_values[index];
        }
        throw new KeyNotFoundException();
      }
      set
      {
        if ((object) key == null)
          throw new ArgumentNullException();
        for (int index = 0; index < this.m_keys.Count; ++index)
        {
          if (this.m_keys[index].Equals((object) key))
            this.m_values[index] = value;
        }
        this.m_keys.Add(key);
        this.m_values.Add(value);
      }
    }
  }

