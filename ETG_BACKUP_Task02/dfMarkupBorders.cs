// Decompiled with JetBrains decompiler
// Type: dfMarkupBorders
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Text.RegularExpressions;

#nullable disable
public struct dfMarkupBorders(int left, int right, int top, int bottom)
{
  public int left = left;
  public int top = top;
  public int right = right;
  public int bottom = bottom;

  public int horizontal => this.left + this.right;

  public int vertical => this.top + this.bottom;

  public static dfMarkupBorders Parse(string value)
  {
    dfMarkupBorders dfMarkupBorders = new dfMarkupBorders();
    value = Regex.Replace(value, "\\s+", " ");
    string[] strArray = value.Split(' ');
    if (strArray.Length == 1)
    {
      int size = dfMarkupStyle.ParseSize(value, 0);
      dfMarkupBorders.left = dfMarkupBorders.right = size;
      dfMarkupBorders.top = dfMarkupBorders.bottom = size;
    }
    else if (strArray.Length == 2)
    {
      int size1 = dfMarkupStyle.ParseSize(strArray[0], 0);
      dfMarkupBorders.top = dfMarkupBorders.bottom = size1;
      int size2 = dfMarkupStyle.ParseSize(strArray[1], 0);
      dfMarkupBorders.left = dfMarkupBorders.right = size2;
    }
    else if (strArray.Length == 3)
    {
      int size3 = dfMarkupStyle.ParseSize(strArray[0], 0);
      dfMarkupBorders.top = size3;
      int size4 = dfMarkupStyle.ParseSize(strArray[1], 0);
      dfMarkupBorders.left = dfMarkupBorders.right = size4;
      int size5 = dfMarkupStyle.ParseSize(strArray[2], 0);
      dfMarkupBorders.bottom = size5;
    }
    else if (strArray.Length == 4)
    {
      int size6 = dfMarkupStyle.ParseSize(strArray[0], 0);
      dfMarkupBorders.top = size6;
      int size7 = dfMarkupStyle.ParseSize(strArray[1], 0);
      dfMarkupBorders.right = size7;
      int size8 = dfMarkupStyle.ParseSize(strArray[2], 0);
      dfMarkupBorders.bottom = size8;
      int size9 = dfMarkupStyle.ParseSize(strArray[3], 0);
      dfMarkupBorders.left = size9;
    }
    return dfMarkupBorders;
  }

  public override string ToString()
  {
    return $"[T:{this.top},R:{this.right},L:{this.left},B:{this.bottom}]";
  }
}
