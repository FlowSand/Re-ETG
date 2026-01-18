using System.Collections.Generic;

#nullable disable
namespace FullInspector.Internal
{
  public class fiStackValue<T>
  {
    private readonly Stack<T> _stack = new Stack<T>();

    public void Push(T value) => this._stack.Push(value);

    public T Pop() => this._stack.Count > 0 ? this._stack.Pop() : default (T);

    public T Value
    {
      get => this._stack.Peek();
      set
      {
        this.Pop();
        this.Push(value);
      }
    }
  }
}
