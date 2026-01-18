using System.Collections.Generic;

#nullable disable
namespace InControl
{
  internal class ThreadSafeQueue<T>
  {
    private object sync;
    private Queue<T> data;

    public ThreadSafeQueue()
    {
      this.sync = new object();
      this.data = new Queue<T>();
    }

    public ThreadSafeQueue(int capacity)
    {
      this.sync = new object();
      this.data = new Queue<T>(capacity);
    }

    public void Enqueue(T item)
    {
      lock (this.sync)
        this.data.Enqueue(item);
    }

    public bool Dequeue(out T item)
    {
      lock (this.sync)
      {
        if (this.data.Count > 0)
        {
          item = this.data.Dequeue();
          return true;
        }
      }
      item = default (T);
      return false;
    }

    public T Dequeue()
    {
      lock (this.sync)
      {
        if (this.data.Count > 0)
          return this.data.Dequeue();
      }
      return default (T);
    }

    public int Dequeue(ref IList<T> list)
    {
      lock (this.sync)
      {
        int count = this.data.Count;
        for (int index = 0; index < count; ++index)
          list.Add(this.data.Dequeue());
        return count;
      }
    }
  }
}
