using System;

#nullable disable

public class IntComparer : IComparable
    {
        public IntComparer(int value) => this.m_value = value;

        public int m_value { get; private set; }

        int IComparable.CompareTo(object ob) => this.m_value - ((IntComparer) ob).m_value;
    }

