// Decompiled with JetBrains decompiler
// Type: InControl.Internal.CodeWriter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

#nullable disable
namespace InControl.Internal;

internal class CodeWriter
{
  private const char NewLine = '\n';
  private int indent;
  private StringBuilder stringBuilder;

  public CodeWriter()
  {
    this.indent = 0;
    this.stringBuilder = new StringBuilder(4096 /*0x1000*/);
  }

  public void IncreaseIndent() => ++this.indent;

  public void DecreaseIndent() => --this.indent;

  public void Append(string code) => this.Append(false, code);

  public void Append(bool trim, string code)
  {
    if (trim)
      code = code.Trim();
    string[] strArray = Regex.Split(code, "\\r?\\n|\\n");
    int length = strArray.Length;
    for (int index = 0; index < length; ++index)
    {
      string str = strArray[index];
      string source = str;
      // ISSUE: reference to a compiler-generated field
      if (CodeWriter.<>f__mg_cache0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        CodeWriter.<>f__mg_cache0 = new Func<char, bool>(char.IsWhiteSpace);
      }
      // ISSUE: reference to a compiler-generated field
      Func<char, bool> fMgCache0 = CodeWriter.<>f__mg_cache0;
      if (!source.All<char>(fMgCache0))
      {
        this.stringBuilder.Append('\t', this.indent);
        this.stringBuilder.Append(str);
      }
      if (index < length - 1)
        this.stringBuilder.Append('\n');
    }
  }

  public void AppendLine(string code)
  {
    this.Append(code);
    this.stringBuilder.Append('\n');
  }

  public void AppendLine(int count) => this.stringBuilder.Append('\n', count);

  public void AppendFormat(string format, params object[] args)
  {
    this.Append(string.Format(format, args));
  }

  public void AppendLineFormat(string format, params object[] args)
  {
    this.AppendLine(string.Format(format, args));
  }

  public override string ToString() => this.stringBuilder.ToString();
}
