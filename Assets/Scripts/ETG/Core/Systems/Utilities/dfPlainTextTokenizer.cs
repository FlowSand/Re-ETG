// Decompiled with JetBrains decompiler
// Type: dfPlainTextTokenizer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable

public class dfPlainTextTokenizer
  {
    private static dfPlainTextTokenizer singleton;

    public static dfList<dfMarkupToken> Tokenize(string source)
    {
      if (dfPlainTextTokenizer.singleton == null)
        dfPlainTextTokenizer.singleton = new dfPlainTextTokenizer();
      return dfPlainTextTokenizer.singleton.tokenize(source);
    }

    private dfList<dfMarkupToken> tokenize(string source)
    {
      dfList<dfMarkupToken> dfList = dfList<dfMarkupToken>.Obtain();
      dfList.EnsureCapacity(this.estimateTokenCount(source));
      dfList.AutoReleaseItems = true;
      int num = 0;
      int startIndex = 0;
      int length = source.Length;
      while (num < length)
      {
        if (source[num] == '\r')
        {
          ++num;
          startIndex = num;
        }
        else
        {
          while (num < length && !char.IsWhiteSpace(source[num]))
            ++num;
          if (num > startIndex)
          {
            dfList.Add(dfMarkupToken.Obtain(source, dfMarkupTokenType.Text, startIndex, num - 1));
            startIndex = num;
          }
          if (num < length && source[num] == '\n')
          {
            dfList.Add(dfMarkupToken.Obtain(source, dfMarkupTokenType.Newline, num, num));
            ++num;
            startIndex = num;
          }
          while (num < length && source[num] != '\n' && source[num] != '\r' && char.IsWhiteSpace(source[num]))
            ++num;
          if (num > startIndex)
          {
            dfList.Add(dfMarkupToken.Obtain(source, dfMarkupTokenType.Whitespace, startIndex, num - 1));
            startIndex = num;
          }
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
        if (char.IsControl(c))
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
  }

