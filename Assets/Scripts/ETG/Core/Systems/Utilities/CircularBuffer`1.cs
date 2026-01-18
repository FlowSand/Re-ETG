using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

#nullable disable

public class CircularBuffer<T> : IEnumerable<T>, IEnumerable
    {
        private T[] m_buffer;
        private int m_head;
        private int m_tail;

        public CircularBuffer(int capacity)
        {
            this.m_buffer = new T[capacity];
            this.m_head = capacity - 1;
        }

        public int Count { get; private set; }

        public int Capacity
        {
            get => this.m_buffer.Length;
            set
            {
                if (value == this.m_buffer.Length)
                    return;
                T[] objArray = new T[value];
                int num = 0;
                while (this.Count > 0 && num < value)
                    objArray[num++] = this.Dequeue();
                this.m_buffer = objArray;
                this.Count = num;
                this.m_head = num - 1;
                this.m_tail = 0;
            }
        }

        public T Enqueue(T item)
        {
            this.m_head = (this.m_head + 1) % this.Capacity;
            T obj = this.m_buffer[this.m_head];
            this.m_buffer[this.m_head] = item;
            if (this.Count == this.Capacity)
                this.m_tail = (this.m_tail + 1) % this.Capacity;
            else
                ++this.Count;
            return obj;
        }

        public T Dequeue()
        {
            if (this.Count == 0)
                throw new InvalidOperationException("queue exhausted");
            T obj = this.m_buffer[this.m_tail];
            this.m_buffer[this.m_tail] = default (T);
            this.m_tail = (this.m_tail + 1) % this.Capacity;
            --this.Count;
            return obj;
        }

        public void Clear()
        {
            this.m_head = this.Capacity - 1;
            this.m_tail = 0;
            this.Count = 0;
        }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= this.Count)
                    throw new ArgumentOutOfRangeException(nameof (index));
                return this.m_buffer[(this.m_tail + index) % this.Capacity];
            }
            set
            {
                if (index < 0 || index >= this.Count)
                    throw new ArgumentOutOfRangeException(nameof (index));
                this.m_buffer[(this.m_tail + index) % this.Capacity] = value;
            }
        }

        public int IndexOf(T item)
        {
            for (int index = 0; index < this.Count; ++index)
            {
                if (object.Equals((object) item, (object) this[index]))
                    return index;
            }
            return -1;
        }

        public void Insert(int index, T item)
        {
            if (index < 0 || index > this.Count)
                throw new ArgumentOutOfRangeException(nameof (index));
            if (this.Count == index)
            {
                this.Enqueue(item);
            }
            else
            {
                T obj = this[this.Count - 1];
                for (int index1 = index; index1 < this.Count - 2; ++index1)
                    this[index1 + 1] = this[index1];
                this[index] = item;
                this.Enqueue(obj);
            }
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= this.Count)
                throw new ArgumentOutOfRangeException(nameof (index));
            for (int index1 = index; index1 > 0; --index1)
                this[index1] = this[index1 - 1];
            this.Dequeue();
        }

        [DebuggerHidden]
        public IEnumerator<T> GetEnumerator()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator<T>) new CircularBuffer_T__GetEnumeratorc__Iterator0()
            {
                _this = this
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
    }

