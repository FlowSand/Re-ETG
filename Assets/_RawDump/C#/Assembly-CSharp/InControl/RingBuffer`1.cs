// Decompiled with JetBrains decompiler
// Type: InControl.RingBuffer`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace InControl;

internal class RingBuffer<T>
{
  private int size;
  private T[] data;
  private int head;
  private int tail;
  private object sync;

  public RingBuffer(int size)
  {
    if (size <= 0)
      throw new ArgumentException("RingBuffer size must be 1 or greater.");
    this.size = size + 1;
    this.data = new T[this.size];
    this.head = 0;
    this.tail = 0;
    this.sync = new object();
  }

  public void Enqueue(T value)
  {
    lock (this.sync)
    {
      if (this.size > 1)
      {
        this.head = (this.head + 1) % this.size;
        if (this.head == this.tail)
          this.tail = (this.tail + 1) % this.size;
      }
      this.data[this.head] = value;
    }
  }

  public T Dequeue()
  {
    lock (this.sync)
    {
      if (this.size > 1 && this.tail != this.head)
        this.tail = (this.tail + 1) % this.size;
      return this.data[this.tail];
    }
  }
}
