// Decompiled with JetBrains decompiler
// Type: dfMarkupStyle
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public struct dfMarkupStyle
    {
      private static Dictionary<string, Color> namedColors = new Dictionary<string, Color>()
      {
        {
          "aqua",
          (Color) dfMarkupStyle.UIntToColor((uint) ushort.MaxValue)
        },
        {
          "black",
          Color.black
        },
        {
          "blue",
          Color.blue
        },
        {
          "cyan",
          Color.cyan
        },
        {
          "fuchsia",
          (Color) dfMarkupStyle.UIntToColor(16711935U)
        },
        {
          "gray",
          Color.gray
        },
        {
          "green",
          Color.green
        },
        {
          "lime",
          (Color) dfMarkupStyle.UIntToColor(65280U)
        },
        {
          "magenta",
          Color.magenta
        },
        {
          "maroon",
          (Color) dfMarkupStyle.UIntToColor(8388608U /*0x800000*/)
        },
        {
          "navy",
          (Color) dfMarkupStyle.UIntToColor(128U /*0x80*/)
        },
        {
          "olive",
          (Color) dfMarkupStyle.UIntToColor(8421376U /*0x808000*/)
        },
        {
          "orange",
          (Color) dfMarkupStyle.UIntToColor(16753920U)
        },
        {
          "purple",
          (Color) dfMarkupStyle.UIntToColor(8388736U /*0x800080*/)
        },
        {
          "red",
          Color.red
        },
        {
          "silver",
          (Color) dfMarkupStyle.UIntToColor(12632256U /*0xC0C0C0*/)
        },
        {
          "teal",
          (Color) dfMarkupStyle.UIntToColor(32896U)
        },
        {
          "white",
          Color.white
        },
        {
          "yellow",
          Color.yellow
        }
      };
      internal int Version;
      public dfRichTextLabel Host;
      public dfAtlas Atlas;
      public dfDynamicFont Font;
      public int FontSize;
      public FontStyle FontStyle;
      public dfMarkupTextDecoration TextDecoration;
      public dfMarkupTextAlign Align;
      public dfMarkupVerticalAlign VerticalAlign;
      public Color Color;
      public Color BackgroundColor;
      public float Opacity;
      public bool PreserveWhitespace;
      public bool Preformatted;
      public int WordSpacing;
      public int CharacterSpacing;
      private int lineHeight;

      public dfMarkupStyle(dfDynamicFont Font, int FontSize, FontStyle FontStyle)
      {
        Version = 0;
        Host = (dfRichTextLabel) null;
        Atlas = (dfAtlas) null;
        this.Font = Font;
        this.FontSize = FontSize;
        this.FontStyle = FontStyle;
        TextDecoration = dfMarkupTextDecoration.None;
        Align = dfMarkupTextAlign.Left;
        VerticalAlign = dfMarkupVerticalAlign.Baseline;
        Color = Color.white;
        BackgroundColor = Color.clear;
        Opacity = 1f;
        PreserveWhitespace = false;
        Preformatted = false;
        WordSpacing = 0;
        CharacterSpacing = 0;
        lineHeight = 0;
      }

      public int LineHeight
      {
        get
        {
          return this.lineHeight == 0 ? Mathf.CeilToInt((float) this.FontSize) : Mathf.Max(this.FontSize, this.lineHeight);
        }
        set => this.lineHeight = value;
      }

      public static dfMarkupTextDecoration ParseTextDecoration(string value)
      {
        switch (value)
        {
          case "underline":
            return dfMarkupTextDecoration.Underline;
          case "overline":
            return dfMarkupTextDecoration.Overline;
          case "line-through":
            return dfMarkupTextDecoration.LineThrough;
          default:
            return dfMarkupTextDecoration.None;
        }
      }

      public static dfMarkupVerticalAlign ParseVerticalAlignment(string value)
      {
        switch (value)
        {
          case "top":
            return dfMarkupVerticalAlign.Top;
          case "center":
          case "middle":
            return dfMarkupVerticalAlign.Middle;
          case "bottom":
            return dfMarkupVerticalAlign.Bottom;
          default:
            return dfMarkupVerticalAlign.Baseline;
        }
      }

      public static dfMarkupTextAlign ParseTextAlignment(string value)
      {
        switch (value)
        {
          case "right":
            return dfMarkupTextAlign.Right;
          case "center":
            return dfMarkupTextAlign.Center;
          case "justify":
            return dfMarkupTextAlign.Justify;
          default:
            return dfMarkupTextAlign.Left;
        }
      }

      public static FontStyle ParseFontStyle(string value, FontStyle baseStyle)
      {
        switch (value)
        {
          case "normal":
            return FontStyle.Normal;
          case "bold":
            if (baseStyle == FontStyle.Normal)
              return FontStyle.Bold;
            if (baseStyle == FontStyle.Italic)
              return FontStyle.BoldAndItalic;
            break;
          case "italic":
            if (baseStyle == FontStyle.Normal)
              return FontStyle.Italic;
            if (baseStyle == FontStyle.Bold)
              return FontStyle.BoldAndItalic;
            break;
        }
        return baseStyle;
      }

      public static int ParseSize(string value, int baseValue)
      {
        if (value.Length > 1 && value.EndsWith("%"))
        {
          int result;
          if (int.TryParse(value.TrimEnd('%'), out result))
            return (int) ((double) baseValue * ((double) result / 100.0));
        }
        if (value.EndsWith("px"))
          value = value.Substring(0, value.Length - 2);
        int result1;
        return int.TryParse(value, out result1) ? result1 : baseValue;
      }

      public static Color ParseColor(string color, Color defaultColor)
      {
        Color color1 = defaultColor;
        if (color.StartsWith("#"))
        {
          uint result = 0;
          color1 = !uint.TryParse(color.Substring(1), NumberStyles.HexNumber, (IFormatProvider) null, out result) ? Color.red : (Color) dfMarkupStyle.UIntToColor(result);
        }
        else
        {
          Color color2;
          if (dfMarkupStyle.namedColors.TryGetValue(color.ToLowerInvariant(), out color2))
            color1 = color2;
        }
        return color1;
      }

      private static Color32 UIntToColor(uint color)
      {
        return new Color32((byte) (color >> 16 /*0x10*/), (byte) (color >> 8), (byte) (color >> 0), byte.MaxValue);
      }
    }

}
