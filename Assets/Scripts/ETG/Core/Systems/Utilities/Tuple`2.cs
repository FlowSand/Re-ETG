#nullable disable

    public sealed class Tuple<T1, T2>
    {
        public T1 First;
        public T2 Second;

        public Tuple(T1 first, T2 second)
        {
            this.First = first;
            this.Second = second;
        }

        public override string ToString() => $"[{this.First}, {this.Second}]";
    }

