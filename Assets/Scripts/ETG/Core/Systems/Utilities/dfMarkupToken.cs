using System;

using UnityEngine;

#nullable disable

public class dfMarkupToken : IPoolable
    {
        private static dfList<dfMarkupToken> pool = new dfList<dfMarkupToken>();
        private bool inUse;
        private string value;
        private dfList<dfMarkupTokenAttribute> attributes = new dfList<dfMarkupTokenAttribute>();

        protected dfMarkupToken()
        {
        }

        public static dfMarkupToken Obtain(
            string source,
            dfMarkupTokenType type,
            int startIndex,
            int endIndex)
        {
            dfMarkupToken dfMarkupToken = dfMarkupToken.pool.Count <= 0 ? new dfMarkupToken() : dfMarkupToken.pool.Pop();
            dfMarkupToken.inUse = true;
            dfMarkupToken.Source = source;
            dfMarkupToken.TokenType = type;
            dfMarkupToken.StartOffset = startIndex;
            dfMarkupToken.EndOffset = Mathf.Min(source.Length - 1, endIndex);
            return dfMarkupToken;
        }

        public void Release()
        {
            if (!this.inUse)
                return;
            this.inUse = false;
            this.value = (string) null;
            this.Source = (string) null;
            this.TokenType = dfMarkupTokenType.Invalid;
            int num1 = 0;
            this.Height = num1;
            this.Width = num1;
            int num2 = 0;
            this.EndOffset = num2;
            this.StartOffset = num2;
            this.attributes.ReleaseItems();
            dfMarkupToken.pool.Add(this);
        }

        public int AttributeCount => this.attributes.Count;

        public dfMarkupTokenType TokenType { get; private set; }

        public string Source { get; private set; }

        public int StartOffset { get; private set; }

        public int EndOffset { get; private set; }

        public int Width { get; internal set; }

        public int Height { get; set; }

        public int Length => this.EndOffset - this.StartOffset + 1;

        public string Value
        {
            get
            {
                if (this.value == null)
                    this.value = this.Source.Substring(this.StartOffset, Mathf.Min(this.EndOffset - this.StartOffset + 1, this.Source.Length - this.StartOffset));
                return this.value;
            }
        }

        public char this[int index]
        {
            get
            {
                if (index < 0 || index >= this.Length)
                    throw new IndexOutOfRangeException(string.Format("Index {0} is out of range ({2}:{1})", (object) index, (object) this.Length, (object) this.Value));
                return this.Source[this.StartOffset + index];
            }
        }

        internal bool Matches(dfMarkupToken other)
        {
            int length = this.Length;
            if (length != other.Length)
                return false;
            for (int index = 0; index < length; ++index)
            {
                if ((int) char.ToLower(this.Source[this.StartOffset + index]) != (int) char.ToLower(other.Source[other.StartOffset + index]))
                    return false;
            }
            return true;
        }

        internal bool Matches(string value)
        {
            int length = this.Length;
            if (length != value.Length)
                return false;
            for (int index = 0; index < length; ++index)
            {
                if ((int) char.ToLower(this.Source[this.StartOffset + index]) != (int) char.ToLower(value[index]))
                    return false;
            }
            return true;
        }

        internal void AddAttribute(dfMarkupToken key, dfMarkupToken value)
        {
            this.attributes.Add(dfMarkupTokenAttribute.Obtain(key, value));
        }

        public dfMarkupTokenAttribute GetAttribute(int index)
        {
            if (index < 0 || index >= this.attributes.Count)
                throw new IndexOutOfRangeException("Invalid attribute index: " + (object) index);
            return this.attributes[index];
        }
    }

