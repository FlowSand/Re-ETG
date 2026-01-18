using System;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace FullInspector
{
    public static class fiGraphMetadataCallbacks
    {
        public static Action<fiGraphMetadata, IList, int> ListMetadataCallback = (Action<fiGraphMetadata, IList, int>) ((m, l, i) => { });
        public static Action<fiGraphMetadata, InspectedProperty> PropertyMetadataCallback = (Action<fiGraphMetadata, InspectedProperty>) ((m, p) => { });

        public static IList Cast<T>(IList<T> list)
        {
            return list is IList ? (IList) list : (IList) new fiGraphMetadataCallbacks.ListWrapper<T>(list);
        }

        private sealed class ListWrapper<T> : IList, ICollection, IEnumerable
        {
            private readonly IList<T> _list;

            public ListWrapper(IList<T> list) => this._list = list;

            public int Add(object value)
            {
                this._list.Add((T) value);
                return this._list.Count - 1;
            }

            public void Clear() => this._list.Clear();

            public bool Contains(object value) => this._list.Contains((T) value);

            public int IndexOf(object value) => this._list.IndexOf((T) value);

            public void Insert(int index, object value) => this._list.Insert(index, (T) value);

            public bool IsFixedSize => false;

            public bool IsReadOnly => this._list.IsReadOnly;

            public void Remove(object value) => this._list.Remove((T) value);

            public void RemoveAt(int index) => this._list.RemoveAt(index);

            public object this[int index]
            {
                get => (object) this._list[index];
                set => this._list[index] = (T) value;
            }

            public void CopyTo(Array array, int index) => this._list.CopyTo((T[]) array, index);

            public int Count => this._list.Count;

            public bool IsSynchronized => false;

            public object SyncRoot => (object) this;

            public IEnumerator GetEnumerator() => (IEnumerator) this._list.GetEnumerator();
        }
    }
}
