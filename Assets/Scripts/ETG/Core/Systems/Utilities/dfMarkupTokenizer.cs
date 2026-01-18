using System;
using System.Collections.Generic;

#nullable disable

public class dfMarkupTokenizer : IDisposable, IPoolable
    {
        private static dfList<dfMarkupTokenizer> pool = new dfList<dfMarkupTokenizer>();
        private static List<string> validTags = new List<string>()
        {
            "color",
            "sprite"
        };
        private string source;
        private int index;

        public static dfList<dfMarkupToken> Tokenize(string source)
        {
            using (dfMarkupTokenizer dfMarkupTokenizer = dfMarkupTokenizer.pool.Count <= 0 ? new dfMarkupTokenizer() : dfMarkupTokenizer.pool.Pop())
                return dfMarkupTokenizer.tokenize(source);
        }

        public void Release()
        {
            this.source = (string) null;
            this.index = 0;
            if (dfMarkupTokenizer.pool.Contains(this))
                return;
            dfMarkupTokenizer.pool.Add(this);
        }

        private dfList<dfMarkupToken> tokenize(string source)
        {
            dfList<dfMarkupToken> dfList = dfList<dfMarkupToken>.Obtain();
            dfList.EnsureCapacity(this.estimateTokenCount(source));
            dfList.AutoReleaseItems = true;
            this.source = source;
            this.index = 0;
            while (this.index < source.Length)
            {
                char c = this.Peek();
                if (this.AtTagPosition())
                {
                    dfMarkupToken tag = this.parseTag();
                    if (tag != null)
                        dfList.Add(tag);
                }
                else
                {
                    dfMarkupToken dfMarkupToken = (dfMarkupToken) null;
                    if (char.IsWhiteSpace(c))
                    {
                        if (c != '\r')
                            dfMarkupToken = this.parseWhitespace();
                    }
                    else
                        dfMarkupToken = this.parseNonWhitespace();
                    if (dfMarkupToken == null)
                    {
                        int num = (int) this.Advance();
                    }
                    else
                        dfList.Add(dfMarkupToken);
                }
            }
            return dfList;
        }

        private int estimateTokenCount(string source)
        {
            if (string.IsNullOrEmpty(source))
                return 0;
            int num = 1;
            bool flag1 = char.IsWhiteSpace(source[0]);
            for (int index = 1; index < source.Length; ++index)
            {
                char c = source[index];
                if (char.IsControl(c) || c == '<')
                {
                    ++num;
                }
                else
                {
                    bool flag2 = char.IsWhiteSpace(c);
                    if (flag2 != flag1)
                    {
                        ++num;
                        flag1 = flag2;
                    }
                }
            }
            return num;
        }

        private bool AtTagPosition()
        {
            if (this.Peek() != '[')
                return false;
            char c = this.Peek(1);
            return c == '/' ? char.IsLetter(this.Peek(2)) && this.isValidTag(this.index + 2, true) : char.IsLetter(c) && this.isValidTag(this.index + 1, false);
        }

        private bool isValidTag(int index, bool endTag)
        {
            for (int index1 = 0; index1 < dfMarkupTokenizer.validTags.Count; ++index1)
            {
                string validTag = dfMarkupTokenizer.validTags[index1];
                bool flag = true;
                for (int index2 = 0; index2 < validTag.Length - 1 && index2 + index < this.source.Length - 1 && (endTag || this.source[index2 + index] != ' ') && this.source[index2 + index] != ']'; ++index2)
                {
                    if ((int) char.ToLowerInvariant(validTag[index2]) != (int) char.ToLowerInvariant(this.source[index2 + index]))
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                    return true;
            }
            return false;
        }

        private dfMarkupToken parseQuotedString()
        {
            char ch = this.Peek();
            switch (ch)
            {
                case '"':
                case '\'':
                    int num1 = (int) this.Advance();
                    int index1 = this.index;
                    int index2 = this.index;
                    while (this.index < this.source.Length && (int) this.Advance() != (int) ch)
                        ++index2;
                    if ((int) this.Peek() == (int) ch)
                    {
                        int num2 = (int) this.Advance();
                    }
                    return dfMarkupToken.Obtain(this.source, dfMarkupTokenType.Text, index1, index2);
                default:
                    return (dfMarkupToken) null;
            }
        }

        private dfMarkupToken parseNonWhitespace()
        {
            int index1 = this.index;
            int index2 = this.index;
            while (this.index < this.source.Length && !char.IsWhiteSpace(this.Advance()) && !this.AtTagPosition())
                ++index2;
            return dfMarkupToken.Obtain(this.source, dfMarkupTokenType.Text, index1, index2);
        }

        private dfMarkupToken parseWhitespace()
        {
            int index1 = this.index;
            int index2 = this.index;
            if (this.Peek() == '\n')
            {
                int num = (int) this.Advance();
                return dfMarkupToken.Obtain(this.source, dfMarkupTokenType.Newline, index1, index1);
            }
            while (this.index < this.source.Length)
            {
                char c = this.Advance();
                switch (c)
                {
                    case '\n':
                    case '\r':
                        goto label_6;
                    default:
                        if (char.IsWhiteSpace(c))
                        {
                            ++index2;
                            continue;
                        }
                        goto label_6;
                }
            }
    label_6:
            return dfMarkupToken.Obtain(this.source, dfMarkupTokenType.Whitespace, index1, index2);
        }

        private dfMarkupToken parseWord()
        {
            if (!char.IsLetter(this.Peek()))
                return (dfMarkupToken) null;
            int index1 = this.index;
            int index2 = this.index;
            while (this.index < this.source.Length && char.IsLetter(this.Advance()))
                ++index2;
            return dfMarkupToken.Obtain(this.source, dfMarkupTokenType.Text, index1, index2);
        }

        private dfMarkupToken parseTag()
        {
            if (this.Peek() != '[')
                return (dfMarkupToken) null;
            if (this.Peek(1) == '/')
                return this.parseEndTag();
            int num1 = (int) this.Advance();
            if (!char.IsLetterOrDigit(this.Peek()))
                return (dfMarkupToken) null;
            int index1 = this.index;
            int index2 = this.index;
            while (this.index < this.source.Length && char.IsLetterOrDigit(this.Advance()))
                ++index2;
            dfMarkupToken tag = dfMarkupToken.Obtain(this.source, dfMarkupTokenType.StartTag, index1, index2);
            if (this.index < this.source.Length && this.Peek() != ']')
            {
                if (char.IsWhiteSpace(this.Peek()))
                    this.parseWhitespace();
                int index3 = this.index;
                int index4 = this.index;
                if (this.Peek() == '"')
                {
                    dfMarkupToken quotedString = this.parseQuotedString();
                    tag.AddAttribute(quotedString, quotedString);
                }
                else
                {
                    while (this.index < this.source.Length && this.Advance() != ']')
                        ++index4;
                    dfMarkupToken key = dfMarkupToken.Obtain(this.source, dfMarkupTokenType.Text, index3, index4);
                    tag.AddAttribute(key, key);
                }
            }
            if (this.Peek() == ']')
            {
                int num2 = (int) this.Advance();
            }
            return tag;
        }

        private dfMarkupToken parseAttributeValue()
        {
            int index1 = this.index;
            int index2 = this.index;
            while (this.index < this.source.Length)
            {
                char c = this.Advance();
                if (c != ']' && !char.IsWhiteSpace(c))
                    ++index2;
                else
                    break;
            }
            return dfMarkupToken.Obtain(this.source, dfMarkupTokenType.Text, index1, index2);
        }

        private dfMarkupToken parseEndTag()
        {
            int num1 = (int) this.Advance(2);
            int index1 = this.index;
            int index2 = this.index;
            while (this.index < this.source.Length && char.IsLetterOrDigit(this.Advance()))
                ++index2;
            if (this.Peek() == ']')
            {
                int num2 = (int) this.Advance();
            }
            return dfMarkupToken.Obtain(this.source, dfMarkupTokenType.EndTag, index1, index2);
        }

        private char Peek() => this.Peek(0);

        private char Peek(int offset)
        {
            return this.index + offset > this.source.Length - 1 ? char.MinValue : this.source[this.index + offset];
        }

        private char Advance() => this.Advance(1);

        private char Advance(int amount)
        {
            this.index += amount;
            return this.Peek();
        }

        public void Dispose() => this.Release();
    }

