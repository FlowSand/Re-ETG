using System;

#nullable disable
namespace FullInspector.Internal
{
    public struct fiOption<T>
    {
        private bool _hasValue;
        private T _value;
        public static fiOption<T> Empty = new fiOption<T>()
        {
            _hasValue = false,
            _value = default (T)
        };

        public fiOption(T value)
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
                if (!this.HasValue)
                    throw new InvalidOperationException("There is no value inside the option");
                return this._value;
            }
        }
    }
}
