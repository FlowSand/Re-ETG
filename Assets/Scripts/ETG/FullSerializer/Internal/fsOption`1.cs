using System;

#nullable disable
namespace FullSerializer.Internal
{
  public struct fsOption<T>
  {
    private bool _hasValue;
    private T _value;
    public static fsOption<T> Empty;

    public fsOption(T value)
    {
      _hasValue = true;
      _value = value;
    }

    public bool HasValue => this._hasValue;

    public bool IsEmpty => !this._hasValue;

    public T Value
    {
      get
      {
        if (this.IsEmpty)
          throw new InvalidOperationException("fsOption is empty");
        return this._value;
      }
    }
  }
}
