using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

#nullable disable

public class dfMarkupString : dfMarkupElement
  {
    private static StringBuilder buffer = new StringBuilder();
    private static Regex whitespacePattern = new Regex("\\s+");
    private static Queue<dfMarkupString> objectPool = new Queue<dfMarkupString>();
    private bool isWhitespace;

    public dfMarkupString(string text)
    {
      this.Text = this.processWhitespace(dfMarkupEntity.Replace(text));
      this.isWhitespace = dfMarkupString.whitespacePattern.IsMatch(this.Text);
    }

    public string Text { get; private set; }

    public bool IsWhitespace => this.isWhitespace;

    public override string ToString() => this.Text;

    internal dfMarkupElement SplitWords()
    {
      dfMarkupTagSpan dfMarkupTagSpan = dfMarkupTagSpan.Obtain();
      int index = 0;
      int startIndex = 0;
      int length = this.Text.Length;
      while (index < length)
      {
        while (index < length && !char.IsWhiteSpace(this.Text[index]))
          ++index;
        if (index > startIndex)
        {
          dfMarkupTagSpan.AddChildNode((dfMarkupElement) dfMarkupString.Obtain(this.Text.Substring(startIndex, index - startIndex)));
          startIndex = index;
        }
        while (index < length && this.Text[index] != '\n' && char.IsWhiteSpace(this.Text[index]))
          ++index;
        if (index > startIndex)
        {
          dfMarkupTagSpan.AddChildNode((dfMarkupElement) dfMarkupString.Obtain(this.Text.Substring(startIndex, index - startIndex)));
          startIndex = index;
        }
        if (index < length && this.Text[index] == '\n')
        {
          dfMarkupTagSpan.AddChildNode((dfMarkupElement) dfMarkupString.Obtain("\n"));
          startIndex = ++index;
        }
      }
      return (dfMarkupElement) dfMarkupTagSpan;
    }

    protected override void _PerformLayoutImpl(dfMarkupBox container, dfMarkupStyle style)
    {
      if ((Object) style.Font == (Object) null)
        return;
      string text = style.PreserveWhitespace || !this.isWhitespace ? this.Text : " ";
      dfMarkupBoxText box = dfMarkupBoxText.Obtain((dfMarkupElement) this, dfMarkupDisplayType.inline, style);
      box.SetText(text);
      container.AddChild((dfMarkupBox) box);
    }

    internal static dfMarkupString Obtain(string text)
    {
      if (dfMarkupString.objectPool.Count <= 0)
        return new dfMarkupString(text);
      dfMarkupString dfMarkupString = dfMarkupString.objectPool.Dequeue();
      dfMarkupString.Text = dfMarkupEntity.Replace(text);
      dfMarkupString.isWhitespace = dfMarkupString.whitespacePattern.IsMatch(dfMarkupString.Text);
      return dfMarkupString;
    }

    internal override void Release()
    {
      base.Release();
      dfMarkupString.objectPool.Enqueue(this);
    }

    private string processWhitespace(string text)
    {
      dfMarkupString.buffer.Length = 0;
      dfMarkupString.buffer.Append(text);
      dfMarkupString.buffer.Replace("\r\n", "\n");
      dfMarkupString.buffer.Replace("\r", "\n");
      dfMarkupString.buffer.Replace("\t", "    ");
      return dfMarkupString.buffer.ToString();
    }
  }

