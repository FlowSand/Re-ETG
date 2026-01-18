// Decompiled with JetBrains decompiler
// Type: ObjectPool`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Diagnostics;
using System.Threading;

#nullable disable

public class ObjectPool<T> where T : class
  {
    private T _firstItem;
    private readonly ObjectPool<T>.Element[] _items;
    private readonly ObjectPool<T>.Factory _factory;
    private readonly ObjectPool<T>.Cleanup _cleanup;

    public ObjectPool(ObjectPool<T>.Factory factory, ObjectPool<T>.Cleanup cleanup = null)
      : this(factory, Environment.ProcessorCount * 2, cleanup)
    {
    }

    public ObjectPool(ObjectPool<T>.Factory factory, int size, ObjectPool<T>.Cleanup cleanup = null)
    {
      this._factory = factory;
      this._items = new ObjectPool<T>.Element[size - 1];
      this._cleanup = cleanup;
    }

    private T CreateInstance() => this._factory();

    public T Allocate()
    {
      T comparand = this._firstItem;
      if ((object) comparand == null || (object) comparand != (object) Interlocked.CompareExchange<T>(ref this._firstItem, (T) null, comparand))
        comparand = this.AllocateSlow();
      return comparand;
    }

    private T AllocateSlow()
    {
      ObjectPool<T>.Element[] items = this._items;
      for (int index = 0; index < items.Length; ++index)
      {
        T comparand = items[index].Value;
        if ((object) comparand != null && (object) comparand == (object) Interlocked.CompareExchange<T>(ref items[index].Value, (T) null, comparand))
          return comparand;
      }
      return this.CreateInstance();
    }

    public void Clear()
    {
      this._firstItem = (T) null;
      foreach (ObjectPool<T>.Element element in this._items)
        element.Value = (T) null;
    }

    public void Free(ref T obj)
    {
      if ((object) obj == null)
        return;
      if ((object) this._firstItem == null)
      {
        if (this._cleanup != null)
          this._cleanup(obj);
        this._firstItem = obj;
      }
      else
        this.FreeSlow(obj);
      obj = (T) null;
    }

    private void FreeSlow(T obj)
    {
      ObjectPool<T>.Element[] items = this._items;
      for (int index = 0; index < items.Length; ++index)
      {
        if ((object) items[index].Value == null)
        {
          if (this._cleanup != null)
            this._cleanup(obj);
          items[index].Value = obj;
          break;
        }
      }
    }

    [DebuggerDisplay("{Value,nq}")]
    private struct Element
    {
      public T Value;
    }

    public delegate T Factory() where T : class;

    public delegate void Cleanup(T obj) where T : class;
  }

