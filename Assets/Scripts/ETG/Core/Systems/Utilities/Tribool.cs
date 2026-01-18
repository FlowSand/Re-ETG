using System;

#nullable disable

public struct Tribool
    {
        public static Tribool Complete = new Tribool(2);
        public static Tribool Ready = new Tribool(1);
        public static Tribool Unready = new Tribool(0);
        public int value;

        public Tribool(int v)
        {
            value = v;
        }

        public override string ToString() => string.Format("[Tribool] " + this.value.ToString());

        public static bool operator true(Tribool a) => a.value == 1;

        public static bool operator false(Tribool a) => a.value == 0;

        public static bool operator !(Tribool a) => a.value == 0;

        public static bool operator ==(Tribool a, Tribool b) => a.value == b.value;

        public static bool operator !=(Tribool a, Tribool b) => a.value != b.value;

        public static Tribool operator ++(Tribool a) => new Tribool(Math.Min(2, a.value + 1));

        public override bool Equals(object obj)
        {
            return obj is Tribool tribool ? this == tribool : base.Equals(obj);
        }

        public override int GetHashCode() => this.value;
    }

