// Decompiled with JetBrains decompiler
// Type: dfMarkupBoxText
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

#nullable disable

public class dfMarkupBoxText : dfMarkupBox
  {
    public dfMarkupBoxText(
      dfMarkupElement element,
      dfMarkupDisplayType display,
      dfMarkupStyle style) : base(element, display, style)
    {
    }

    private static int[] TRIANGLE_INDICES = new int[6]
    {
      0,
      1,
      2,
      0,
      2,
      3
    };
    private static Queue<dfMarkupBoxText> objectPool = new Queue<dfMarkupBoxText>();
    private static Regex whitespacePattern = new Regex("\\s+");
    private dfRenderData renderData = new dfRenderData();
    private bool isWhitespace;

    public string Text { get; private set; }

    public bool IsWhitespace => this.isWhitespace;

    public static dfMarkupBoxText Obtain(
      dfMarkupElement element,
      dfMarkupDisplayType display,
      dfMarkupStyle style)
    {
      if (dfMarkupBoxText.objectPool.Count <= 0)
        return new dfMarkupBoxText(element, display, style);
      dfMarkupBoxText dfMarkupBoxText = dfMarkupBoxText.objectPool.Dequeue();
      dfMarkupBoxText.Element = element;
      dfMarkupBoxText.Display = display;
      dfMarkupBoxText.Style = style;
      dfMarkupBoxText.Position = Vector2.zero;
      dfMarkupBoxText.Size = Vector2.zero;
      dfMarkupBoxText.Baseline = (int) ((double) style.FontSize * 1.1000000238418579);
      dfMarkupBoxText.Margins = new dfMarkupBorders();
      dfMarkupBoxText.Padding = new dfMarkupBorders();
      return dfMarkupBoxText;
    }

    public override void Release()
    {
      base.Release();
      this.Text = string.Empty;
      this.renderData.Clear();
      dfMarkupBoxText.objectPool.Enqueue(this);
    }

    internal void SetText(string text)
    {
      this.Text = text;
      if ((Object) this.Style.Font == (Object) null)
        return;
      this.isWhitespace = dfMarkupBoxText.whitespacePattern.IsMatch(this.Text);
      string text1 = this.Style.PreserveWhitespace || !this.isWhitespace ? this.Text : " ";
      int fontSize = this.Style.FontSize;
      Vector2 vector2 = new Vector2(0.0f, (float) this.Style.LineHeight);
      this.Style.Font.RequestCharacters(text1, this.Style.FontSize, this.Style.FontStyle);
      CharacterInfo info = new CharacterInfo();
      for (int index = 0; index < text1.Length; ++index)
      {
        if (this.Style.Font.BaseFont.GetCharacterInfo(text1[index], out info, fontSize, this.Style.FontStyle))
        {
          float a = (float) info.maxX;
          if (text1[index] == ' ')
            a = Mathf.Max(a, (float) fontSize * 0.33f);
          else if (text1[index] == '\t')
            a += (float) (fontSize * 3);
          vector2.x += a;
        }
      }
      this.Size = vector2;
      dfDynamicFont font = this.Style.Font;
      float num = (float) fontSize / (float) font.FontSize;
      this.Baseline = Mathf.CeilToInt((float) font.Baseline * num);
    }

    protected override dfRenderData OnRebuildRenderData()
    {
      this.renderData.Clear();
      if ((Object) this.Style.Font == (Object) null)
        return (dfRenderData) null;
      if (this.Style.TextDecoration == dfMarkupTextDecoration.Underline)
        this.renderUnderline();
      this.renderText(this.Text);
      return this.renderData;
    }

    private void renderUnderline()
    {
    }

    private void renderText(string text)
    {
      dfDynamicFont font = this.Style.Font;
      int fontSize = this.Style.FontSize;
      FontStyle fontStyle = this.Style.FontStyle;
      CharacterInfo info = new CharacterInfo();
      dfList<Vector3> vertices = this.renderData.Vertices;
      dfList<int> triangles = this.renderData.Triangles;
      dfList<Vector2> uv = this.renderData.UV;
      dfList<Color32> colors = this.renderData.Colors;
      float num1 = (float) fontSize / (float) font.FontSize;
      float num2 = (float) font.Descent * num1;
      float num3 = 0.0f;
      font.RequestCharacters(text, fontSize, fontStyle);
      this.renderData.Material = font.Material;
      for (int index = 0; index < text.Length; ++index)
      {
        if (font.BaseFont.GetCharacterInfo(text[index], out info, fontSize, fontStyle))
        {
          dfMarkupBoxText.addTriangleIndices(vertices, triangles);
          float num4 = (float) (font.FontSize + info.maxY - fontSize) + num2;
          float x1 = num3 + (float) info.minX;
          float y1 = num4;
          float x2 = x1 + (float) info.glyphWidth;
          float y2 = y1 - (float) info.glyphHeight;
          Vector3 vector3_1 = new Vector3(x1, y1);
          Vector3 vector3_2 = new Vector3(x2, y1);
          Vector3 vector3_3 = new Vector3(x2, y2);
          Vector3 vector3_4 = new Vector3(x1, y2);
          vertices.Add(vector3_1);
          vertices.Add(vector3_2);
          vertices.Add(vector3_3);
          vertices.Add(vector3_4);
          Color color = this.Style.Color;
          colors.Add((Color32) color);
          colors.Add((Color32) color);
          colors.Add((Color32) color);
          colors.Add((Color32) color);
          uv.Add(info.uvTopLeft);
          uv.Add(info.uvTopRight);
          uv.Add(info.uvBottomRight);
          uv.Add(info.uvBottomLeft);
          num3 += (float) Mathf.CeilToInt((float) info.maxX);
        }
      }
    }

    private static void addTriangleIndices(dfList<Vector3> verts, dfList<int> triangles)
    {
      int count = verts.Count;
      foreach (int triangleIndex in dfMarkupBoxText.TRIANGLE_INDICES)
        triangles.Add(count + triangleIndex);
    }
  }

