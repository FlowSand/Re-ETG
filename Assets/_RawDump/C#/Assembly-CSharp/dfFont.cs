// Decompiled with JetBrains decompiler
// Type: dfFont
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

#nullable disable
[AddComponentMenu("Daikon Forge/User Interface/Font Definition")]
[dfHelp("http://www.daikonforge.com/docs/df-gui/classdf_font.html")]
[Serializable]
public class dfFont : dfFontBase
{
  [SerializeField]
  protected dfAtlas atlas;
  [SerializeField]
  protected string sprite;
  [SerializeField]
  protected string face = string.Empty;
  [SerializeField]
  protected int size;
  [SerializeField]
  protected bool bold;
  [SerializeField]
  protected bool italic;
  [SerializeField]
  protected string charset;
  [SerializeField]
  protected int stretchH;
  [SerializeField]
  protected bool smooth;
  [SerializeField]
  protected int aa;
  [SerializeField]
  protected int[] padding;
  [SerializeField]
  protected int[] spacing;
  [SerializeField]
  protected int outline;
  [SerializeField]
  protected int lineHeight;
  [SerializeField]
  private List<dfFont.GlyphDefinition> glyphs = new List<dfFont.GlyphDefinition>();
  [SerializeField]
  protected List<dfFont.GlyphKerning> kerning = new List<dfFont.GlyphKerning>();
  private Dictionary<int, dfFont.GlyphDefinition> glyphMap;
  private Dictionary<int, dfFont.GlyphKerningList> kerningMap;

  public List<dfFont.GlyphDefinition> Glyphs => this.glyphs;

  public List<dfFont.GlyphKerning> KerningInfo => this.kerning;

  public dfAtlas Atlas
  {
    get => this.atlas;
    set
    {
      if (!((UnityEngine.Object) value != (UnityEngine.Object) this.atlas))
        return;
      this.atlas = value;
      this.glyphMap = (Dictionary<int, dfFont.GlyphDefinition>) null;
    }
  }

  public override Material Material
  {
    get => this.Atlas.Material;
    set => throw new InvalidOperationException();
  }

  public override Texture Texture => (Texture) this.Atlas.Texture;

  public string Sprite
  {
    get => this.sprite;
    set
    {
      if (!(value != this.sprite))
        return;
      this.sprite = value;
      this.glyphMap = (Dictionary<int, dfFont.GlyphDefinition>) null;
    }
  }

  public override bool IsValid
  {
    get
    {
      return !((UnityEngine.Object) this.Atlas == (UnityEngine.Object) null) && !(this.Atlas[this.Sprite] == (dfAtlas.ItemInfo) null);
    }
  }

  public string FontFace => this.face;

  public override int FontSize
  {
    get => this.size;
    set => throw new InvalidOperationException();
  }

  public override int LineHeight
  {
    get => this.lineHeight;
    set => this.lineHeight = value;
  }

  public bool Bold => this.bold;

  public bool Italic => this.italic;

  public int[] Padding => this.padding;

  public int[] Spacing => this.spacing;

  public int Outline => this.outline;

  public int Count => this.glyphs.Count;

  public void OnEnable() => this.glyphMap = (Dictionary<int, dfFont.GlyphDefinition>) null;

  public override dfFontRendererBase ObtainRenderer() => dfFont.BitmappedFontRenderer.Obtain(this);

  public void AddKerning(int first, int second, int amount)
  {
    this.kerning.Add(new dfFont.GlyphKerning()
    {
      first = first,
      second = second,
      amount = amount
    });
  }

  public int GetKerning(char previousChar, char currentChar)
  {
    try
    {
      if (this.kerningMap == null)
        this.buildKerningMap();
      dfFont.GlyphKerningList glyphKerningList = (dfFont.GlyphKerningList) null;
      return !this.kerningMap.TryGetValue((int) previousChar, out glyphKerningList) ? 0 : glyphKerningList.GetKerning((int) previousChar, (int) currentChar);
    }
    finally
    {
    }
  }

  private void buildKerningMap()
  {
    Dictionary<int, dfFont.GlyphKerningList> dictionary = this.kerningMap = new Dictionary<int, dfFont.GlyphKerningList>();
    for (int index = 0; index < this.kerning.Count; ++index)
    {
      dfFont.GlyphKerning kerning = this.kerning[index];
      if (!dictionary.ContainsKey(kerning.first))
        dictionary[kerning.first] = new dfFont.GlyphKerningList();
      dictionary[kerning.first].Add(kerning);
    }
  }

  public dfFont.GlyphDefinition GetGlyph(char id)
  {
    if (this.glyphMap == null)
    {
      this.glyphMap = new Dictionary<int, dfFont.GlyphDefinition>();
      for (int index = 0; index < this.glyphs.Count; ++index)
      {
        dfFont.GlyphDefinition glyph = this.glyphs[index];
        this.glyphMap[glyph.id] = glyph;
      }
    }
    dfFont.GlyphDefinition glyph1 = (dfFont.GlyphDefinition) null;
    this.glyphMap.TryGetValue((int) id, out glyph1);
    return glyph1;
  }

  private class GlyphKerningList
  {
    private Dictionary<int, int> list = new Dictionary<int, int>();

    public void Add(dfFont.GlyphKerning kerning) => this.list[kerning.second] = kerning.amount;

    public int GetKerning(int firstCharacter, int secondCharacter)
    {
      int kerning = 0;
      this.list.TryGetValue(secondCharacter, out kerning);
      return kerning;
    }
  }

  [Serializable]
  public class GlyphKerning : IComparable<dfFont.GlyphKerning>
  {
    public int first;
    public int second;
    public int amount;

    public int CompareTo(dfFont.GlyphKerning other)
    {
      return this.first == other.first ? this.second.CompareTo(other.second) : this.first.CompareTo(other.first);
    }
  }

  [Serializable]
  public class GlyphDefinition : IComparable<dfFont.GlyphDefinition>
  {
    [SerializeField]
    public int id;
    [SerializeField]
    public int x;
    [SerializeField]
    public int y;
    [SerializeField]
    public int width;
    [SerializeField]
    public int height;
    [SerializeField]
    public int xoffset;
    [SerializeField]
    public int yoffset;
    [SerializeField]
    public int xadvance;
    [SerializeField]
    public bool rotated;

    public int CompareTo(dfFont.GlyphDefinition other) => this.id.CompareTo(other.id);
  }

  public class BitmappedFontRenderer : dfFontRendererBase, IPoolable
  {
    private static Queue<dfFont.BitmappedFontRenderer> objectPool = new Queue<dfFont.BitmappedFontRenderer>();
    private static Vector2[] OUTLINE_OFFSETS = new Vector2[4]
    {
      new Vector2(-1f, -1f),
      new Vector2(-1f, 1f),
      new Vector2(1f, -1f),
      new Vector2(1f, 1f)
    };
    private static int[] TRIANGLE_INDICES = new int[6]
    {
      0,
      1,
      3,
      3,
      1,
      2
    };
    private static Stack<Color32> textColors = new Stack<Color32>();
    private dfList<dfFont.LineRenderInfo> lines;
    private dfList<dfMarkupToken> tokens;

    internal BitmappedFontRenderer()
    {
    }

    public int LineCount => this.lines.Count;

    public static dfFontRendererBase Obtain(dfFont font)
    {
      dfFont.BitmappedFontRenderer bitmappedFontRenderer = dfFont.BitmappedFontRenderer.objectPool.Count <= 0 ? new dfFont.BitmappedFontRenderer() : dfFont.BitmappedFontRenderer.objectPool.Dequeue();
      bitmappedFontRenderer.Reset();
      bitmappedFontRenderer.Font = (dfFontBase) font;
      return (dfFontRendererBase) bitmappedFontRenderer;
    }

    public override void Release()
    {
      this.Reset();
      if (this.tokens != null)
      {
        this.tokens.ReleaseItems();
        this.tokens.Release();
      }
      this.tokens = (dfList<dfMarkupToken>) null;
      if (this.lines != null)
      {
        this.lines.Release();
        this.lines = (dfList<dfFont.LineRenderInfo>) null;
      }
      dfFont.LineRenderInfo.ResetPool();
      this.BottomColor = new Color32?();
      dfFont.BitmappedFontRenderer.objectPool.Enqueue(this);
    }

    public override float[] GetCharacterWidths(string text)
    {
      float totalWidth = 0.0f;
      return this.GetCharacterWidths(text, 0, text.Length - 1, out totalWidth);
    }

    public float[] GetCharacterWidths(
      string text,
      int startIndex,
      int endIndex,
      out float totalWidth)
    {
      totalWidth = 0.0f;
      dfFont font = (dfFont) this.Font;
      float[] characterWidths = new float[text.Length];
      float num1 = this.TextScale * this.PixelRatio;
      float num2 = (float) this.CharacterSpacing * num1;
      for (int index = startIndex; index <= endIndex; ++index)
      {
        dfFont.GlyphDefinition glyph = font.GetGlyph(text[index]);
        if (glyph != null)
        {
          if (index > 0)
          {
            characterWidths[index - 1] += num2;
            totalWidth += num2;
          }
          float num3 = (float) glyph.xadvance * num1;
          characterWidths[index] = num3;
          totalWidth += num3;
        }
      }
      return characterWidths;
    }

    public override Vector2 MeasureString(string text)
    {
      this.tokenize(text);
      dfList<dfFont.LineRenderInfo> linebreaks = this.calculateLinebreaks();
      int num = 0;
      int y = 0;
      for (int index = 0; index < linebreaks.Count; ++index)
      {
        num = Mathf.Max((int) linebreaks[index].lineWidth, num);
        y += (int) linebreaks[index].lineHeight;
      }
      return new Vector2((float) num, (float) y) * this.TextScale;
    }

    public override void Render(string text, dfRenderData destination)
    {
      dfFont.BitmappedFontRenderer.textColors.Clear();
      dfFont.BitmappedFontRenderer.textColors.Push((Color32) Color.white);
      this.tokenize(text);
      dfList<dfFont.LineRenderInfo> linebreaks = this.calculateLinebreaks();
      destination.EnsureCapacity(this.getAnticipatedVertCount(this.tokens));
      int b1 = 0;
      int b2 = 0;
      Vector3 vectorOffset = this.VectorOffset;
      float num = this.TextScale * this.PixelRatio;
      for (int index = 0; index < linebreaks.Count; ++index)
      {
        dfFont.LineRenderInfo lineRenderInfo = linebreaks[index];
        int count = destination.Vertices.Count;
        this.renderLine(linebreaks[index], dfFont.BitmappedFontRenderer.textColors, vectorOffset, destination);
        vectorOffset.y -= (float) this.Font.LineHeight * num;
        b1 = Mathf.Max((int) lineRenderInfo.lineWidth, b1);
        b2 += (int) lineRenderInfo.lineHeight;
        if ((double) lineRenderInfo.lineWidth * (double) this.TextScale > (double) this.MaxSize.x)
          this.clipRight(destination, count);
        if ((double) b2 * (double) this.TextScale > (double) this.MaxSize.y)
          this.clipBottom(destination, count);
      }
      this.RenderedSize = new Vector2(Mathf.Min(this.MaxSize.x, (float) b1), Mathf.Min(this.MaxSize.y, (float) b2)) * this.TextScale;
    }

    private int getAnticipatedVertCount(dfList<dfMarkupToken> tokens)
    {
      int num = 4 + (!this.Shadow ? 0 : 4) + (!this.Outline ? 0 : 4);
      int anticipatedVertCount = 0;
      for (int index = 0; index < tokens.Count; ++index)
      {
        dfMarkupToken token = tokens[index];
        if (token.TokenType == dfMarkupTokenType.Text)
          anticipatedVertCount += num * token.Length;
        else if (token.TokenType == dfMarkupTokenType.StartTag)
          anticipatedVertCount += 4;
      }
      return anticipatedVertCount;
    }

    private void renderLine(
      dfFont.LineRenderInfo line,
      Stack<Color32> colors,
      Vector3 position,
      dfRenderData destination)
    {
      float num = this.TextScale * this.PixelRatio;
      position.x += (float) this.calculateLineAlignment(line) * num;
      for (int startOffset = line.startOffset; startOffset <= line.endOffset; ++startOffset)
      {
        dfMarkupToken token = this.tokens[startOffset];
        switch (token.TokenType)
        {
          case dfMarkupTokenType.Text:
            this.renderText(token, colors.Peek(), position, destination);
            position += this.PerCharacterAccumulatedOffset * (float) token.Length;
            break;
          case dfMarkupTokenType.StartTag:
            if (token.Matches("sprite"))
            {
              this.renderSprite(token, colors.Peek(), position, destination);
              break;
            }
            if (token.Matches("color"))
            {
              colors.Push(this.parseColor(token));
              break;
            }
            break;
          case dfMarkupTokenType.EndTag:
            if (token.Matches("color") && colors.Count > 1)
            {
              colors.Pop();
              break;
            }
            break;
        }
        position.x += (float) token.Width * num;
      }
    }

    private void renderText(
      dfMarkupToken token,
      Color32 color,
      Vector3 position,
      dfRenderData destination)
    {
      try
      {
        dfList<Vector3> vertices = destination.Vertices;
        dfList<int> triangles = destination.Triangles;
        dfList<Color32> colors = destination.Colors;
        dfList<Vector2> uv = destination.UV;
        dfFont font = (dfFont) this.Font;
        dfAtlas.ItemInfo atla = font.Atlas[font.sprite];
        Texture texture = font.Texture;
        float num1 = 1f / (float) texture.width;
        float num2 = 1f / (float) texture.height;
        float num3 = this.TextScale * this.PixelRatio;
        char previousChar = char.MinValue;
        Color32 color32_1 = this.applyOpacity((Color32) this.multiplyColors((Color) color, (Color) this.DefaultColor));
        Color32 color32_2 = color32_1;
        if (this.BottomColor.HasValue)
          color32_2 = this.applyOpacity((Color32) this.multiplyColors((Color) color, (Color) this.BottomColor.Value));
        int index1 = 0;
        while (index1 < token.Length)
        {
          char ch = token[index1];
          if (ch != char.MinValue)
          {
            dfFont.GlyphDefinition glyph = font.GetGlyph(ch);
            if (glyph != null)
            {
              int kerning = font.GetKerning(previousChar, ch);
              float x1 = position.x + (float) (glyph.xoffset + kerning) * num3;
              float y1 = position.y - (float) glyph.yoffset * num3;
              float num4 = (float) glyph.width * num3;
              float num5 = (float) glyph.height * num3;
              float x2 = x1 + num4;
              float y2 = y1 - num5;
              Vector3 vector3_1 = new Vector3(x1, y1);
              Vector3 vector3_2 = new Vector3(x2, y1);
              Vector3 vector3_3 = new Vector3(x2, y2);
              Vector3 vector3_4 = new Vector3(x1, y2);
              float x3 = atla.region.x + (float) glyph.x * num1;
              float y3 = atla.region.yMax - (float) glyph.y * num2;
              float x4 = x3 + (float) glyph.width * num1;
              float y4 = y3 - (float) glyph.height * num2;
              if (this.Shadow)
              {
                dfFont.BitmappedFontRenderer.addTriangleIndices(vertices, triangles);
                Vector3 vector3_5 = (Vector3) this.ShadowOffset * num3;
                vertices.Add(vector3_1 + vector3_5);
                vertices.Add(vector3_2 + vector3_5);
                vertices.Add(vector3_3 + vector3_5);
                vertices.Add(vector3_4 + vector3_5);
                Color32 color32_3 = this.applyOpacity(this.ShadowColor);
                colors.Add(color32_3);
                colors.Add(color32_3);
                colors.Add(color32_3);
                colors.Add(color32_3);
                uv.Add(new Vector2(x3, y3));
                uv.Add(new Vector2(x4, y3));
                uv.Add(new Vector2(x4, y4));
                uv.Add(new Vector2(x3, y4));
              }
              if (this.Outline)
              {
                for (int index2 = 0; index2 < dfFont.BitmappedFontRenderer.OUTLINE_OFFSETS.Length; ++index2)
                {
                  dfFont.BitmappedFontRenderer.addTriangleIndices(vertices, triangles);
                  Vector3 vector3_6 = (Vector3) dfFont.BitmappedFontRenderer.OUTLINE_OFFSETS[index2] * (float) this.OutlineSize * num3;
                  vertices.Add(vector3_1 + vector3_6);
                  vertices.Add(vector3_2 + vector3_6);
                  vertices.Add(vector3_3 + vector3_6);
                  vertices.Add(vector3_4 + vector3_6);
                  Color32 color32_4 = this.applyOpacity(this.OutlineColor);
                  colors.Add(color32_4);
                  colors.Add(color32_4);
                  colors.Add(color32_4);
                  colors.Add(color32_4);
                  uv.Add(new Vector2(x3, y3));
                  uv.Add(new Vector2(x4, y3));
                  uv.Add(new Vector2(x4, y4));
                  uv.Add(new Vector2(x3, y4));
                }
              }
              dfFont.BitmappedFontRenderer.addTriangleIndices(vertices, triangles);
              vertices.Add(vector3_1);
              vertices.Add(vector3_2);
              vertices.Add(vector3_3);
              vertices.Add(vector3_4);
              colors.Add(color32_1);
              colors.Add(color32_1);
              colors.Add(color32_2);
              colors.Add(color32_2);
              if (destination.Glitchy)
              {
                float num6 = x4 - x3;
                float num7 = y4 - y3;
                float num8 = UnityEngine.Random.value * num6 * UnityEngine.Random.Range(2f, 3f);
                float num9 = UnityEngine.Random.value * num7 * UnityEngine.Random.Range(2f, 3f);
                float x5 = num6 * UnityEngine.Random.Range(2f, 5f);
                float y5 = num7 * UnityEngine.Random.Range(2f, 5f);
                Vector2 vector2 = new Vector2(x3 - num8, y3 - num9);
                uv.Add(vector2);
                uv.Add(vector2 + new Vector2(x5, 0.0f));
                uv.Add(vector2 + new Vector2(x5, y5));
                uv.Add(vector2 + new Vector2(0.0f, y5));
              }
              else
              {
                uv.Add(new Vector2(x3, y3));
                uv.Add(new Vector2(x4, y3));
                uv.Add(new Vector2(x4, y4));
                uv.Add(new Vector2(x3, y4));
              }
              position.x += (float) (glyph.xadvance + kerning + this.CharacterSpacing) * num3;
              position += this.PerCharacterAccumulatedOffset;
            }
          }
          ++index1;
          previousChar = ch;
        }
      }
      finally
      {
      }
    }

    private void renderSprite(
      dfMarkupToken token,
      Color32 color,
      Vector3 position,
      dfRenderData destination)
    {
      try
      {
        dfList<Vector3> vertices = destination.Vertices;
        dfList<int> triangles = destination.Triangles;
        dfList<Color32> colors = destination.Colors;
        dfList<Vector2> uv = destination.UV;
        dfAtlas.ItemInfo atla = ((dfFont) this.Font).Atlas[token.GetAttribute(0).Value.Value];
        if (atla == (dfAtlas.ItemInfo) null)
          return;
        float num1 = (float) token.Height * this.TextScale * this.PixelRatio;
        float num2 = (float) token.Width * this.TextScale * this.PixelRatio;
        float x = position.x;
        float y = position.y;
        if (this.Font.IsSpriteScaledUIFont())
          y = position.y + (float) ((double) token.Height * (double) this.TextScale * (double) this.PixelRatio * 0.20000000298023224);
        int count = vertices.Count;
        vertices.Add(new Vector3(x, y));
        vertices.Add(new Vector3(x + num2, y));
        vertices.Add(new Vector3(x + num2, y - num1));
        vertices.Add(new Vector3(x, y - num1));
        triangles.Add(count);
        triangles.Add(count + 1);
        triangles.Add(count + 3);
        triangles.Add(count + 3);
        triangles.Add(count + 1);
        triangles.Add(count + 2);
        Color32 color32 = !this.ColorizeSymbols ? this.applyOpacity(this.DefaultColor) : this.applyOpacity(color);
        colors.Add(color32);
        colors.Add(color32);
        colors.Add(color32);
        colors.Add(color32);
        Rect region = atla.region;
        uv.Add(new Vector2(region.x, region.yMax));
        uv.Add(new Vector2(region.xMax, region.yMax));
        uv.Add(new Vector2(region.xMax, region.y));
        uv.Add(new Vector2(region.x, region.y));
      }
      finally
      {
      }
    }

    private Color32 parseColor(dfMarkupToken token)
    {
      Color color1 = Color.white;
      if (token.AttributeCount == 1)
      {
        string color2 = token.GetAttribute(0).Value.Value;
        if (color2.Length == 7 && color2[0] == '#')
        {
          uint result = 0;
          uint.TryParse(color2.Substring(1), NumberStyles.HexNumber, (IFormatProvider) null, out result);
          color1 = (Color) this.UIntToColor(result | 4278190080U /*0xFF000000*/);
        }
        else
          color1 = dfMarkupStyle.ParseColor(color2, (Color) this.DefaultColor);
      }
      return this.applyOpacity((Color32) color1);
    }

    private Color32 UIntToColor(uint color)
    {
      byte a = (byte) (color >> 24);
      return new Color32((byte) (color >> 16 /*0x10*/), (byte) (color >> 8), (byte) (color >> 0), a);
    }

    private dfList<dfFont.LineRenderInfo> calculateLinebreaks()
    {
      try
      {
        if (this.lines != null)
          return this.lines;
        this.lines = dfList<dfFont.LineRenderInfo>.Obtain();
        int num1 = 0;
        int start = 0;
        int num2 = 0;
        int num3 = 0;
        float num4 = (float) this.Font.LineHeight * this.TextScale;
        while (num2 < this.tokens.Count && (double) this.lines.Count * (double) num4 < (double) this.MaxSize.y)
        {
          dfMarkupToken token = this.tokens[num2];
          dfMarkupTokenType tokenType = token.TokenType;
          if (tokenType == dfMarkupTokenType.Newline)
          {
            this.lines.Add(dfFont.LineRenderInfo.Obtain(start, num2));
            int num5;
            num2 = num5 = num2 + 1;
            num1 = num5;
            start = num5;
            num3 = 0;
          }
          else
          {
            int num6 = Mathf.CeilToInt((float) token.Width * this.TextScale);
            int num7;
            if (this.WordWrap && num1 > start)
            {
              switch (tokenType)
              {
                case dfMarkupTokenType.Text:
                  num7 = 1;
                  break;
                case dfMarkupTokenType.StartTag:
                  num7 = token.Matches("sprite") ? 1 : 0;
                  break;
                default:
                  num7 = 0;
                  break;
              }
            }
            else
              num7 = 0;
            if (num7 != 0 && (double) (num3 + num6) >= (double) this.MaxSize.x)
            {
              if (num1 > start)
              {
                this.lines.Add(dfFont.LineRenderInfo.Obtain(start, num1 - 1));
                int num8;
                num1 = num8 = num1 + 1;
                num2 = num8;
                start = num8;
                num3 = 0;
              }
              else
              {
                this.lines.Add(dfFont.LineRenderInfo.Obtain(start, num1 - 1));
                int num9;
                num2 = num9 = num2 + 1;
                num1 = num9;
                start = num9;
                num3 = 0;
              }
              if (GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.JAPANESE || GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.CHINESE)
                --start;
            }
            else
            {
              switch (tokenType)
              {
                case dfMarkupTokenType.Text:
                  if (GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.JAPANESE || GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.CHINESE)
                  {
                    num1 = num2;
                    break;
                  }
                  break;
                case dfMarkupTokenType.Whitespace:
                  num1 = num2;
                  break;
              }
              num3 += num6;
              ++num2;
            }
          }
        }
        if (start < this.tokens.Count)
          this.lines.Add(dfFont.LineRenderInfo.Obtain(start, this.tokens.Count - 1));
        for (int index = 0; index < this.lines.Count; ++index)
          this.calculateLineSize(this.lines[index]);
        return this.lines;
      }
      finally
      {
      }
    }

    private int calculateLineAlignment(dfFont.LineRenderInfo line)
    {
      float lineWidth = line.lineWidth;
      if (this.TextAlign == TextAlignment.Left || (double) lineWidth == 0.0)
        return 0;
      return Mathf.Max(0, this.TextAlign != TextAlignment.Right ? Mathf.FloorToInt((float) (((double) this.MaxSize.x / (double) this.TextScale - (double) lineWidth) * 0.5)) : Mathf.FloorToInt(this.MaxSize.x / this.TextScale - lineWidth));
    }

    private void calculateLineSize(dfFont.LineRenderInfo line)
    {
      line.lineHeight = (float) this.Font.LineHeight;
      int num = 0;
      for (int startOffset = line.startOffset; startOffset <= line.endOffset; ++startOffset)
        num += this.tokens[startOffset].Width;
      line.lineWidth = (float) num;
    }

    private dfList<dfMarkupToken> tokenize(string text)
    {
      try
      {
        if (this.tokens != null)
        {
          if (object.ReferenceEquals((object) this.tokens[0].Source, (object) text))
            return this.tokens;
          this.tokens.ReleaseItems();
          this.tokens.Release();
        }
        this.tokens = GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.JAPANESE || GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.CHINESE ? dfJapaneseMarkupTokenizer.Tokenize(text) : (!this.ProcessMarkup ? dfPlainTextTokenizer.Tokenize(text) : dfMarkupTokenizer.Tokenize(text));
        for (int index = 0; index < this.tokens.Count; ++index)
          this.calculateTokenRenderSize(this.tokens[index]);
        return this.tokens;
      }
      finally
      {
      }
    }

    private void calculateTokenRenderSize(dfMarkupToken token)
    {
      try
      {
        dfFont font = (dfFont) this.Font;
        int num1 = 0;
        char previousChar = char.MinValue;
        bool flag1 = token.TokenType == dfMarkupTokenType.Whitespace || token.TokenType == dfMarkupTokenType.Text;
        bool flag2 = false;
        if (flag1)
        {
          int index = 0;
          while (index < token.Length)
          {
            char ch = token[index];
            if (ch == '\t')
            {
              num1 += this.TabSize;
            }
            else
            {
              dfFont.GlyphDefinition glyph = font.GetGlyph(ch);
              if (glyph != null)
              {
                if (index > 0)
                  num1 = num1 + font.GetKerning(previousChar, ch) + this.CharacterSpacing;
                num1 += glyph.xadvance;
              }
            }
            ++index;
            previousChar = ch;
          }
        }
        else if (token.TokenType == dfMarkupTokenType.StartTag && token.Matches("sprite"))
        {
          if (token.AttributeCount < 1)
            throw new Exception("Missing sprite name in markup");
          Texture texture = font.Texture;
          int lineHeight = font.LineHeight;
          string key = token.GetAttribute(0).Value.Value;
          dfAtlas.ItemInfo atla = font.atlas[key];
          if (atla != (dfAtlas.ItemInfo) null)
          {
            float num2 = (float) ((double) atla.region.width * (double) texture.width / ((double) atla.region.height * (double) texture.height));
            num1 = Mathf.CeilToInt((float) lineHeight * num2);
            float num3 = 1f;
            if (this.Font.IsSpriteScaledUIFont())
              num3 = 5f;
            flag2 = true;
            token.Height = Mathf.CeilToInt(atla.region.height * (float) texture.height * num3);
            token.Width = Mathf.CeilToInt(atla.region.width * (float) texture.width * num3);
          }
        }
        if (flag2)
          return;
        token.Height = this.Font.LineHeight;
        token.Width = num1;
      }
      finally
      {
      }
    }

    private float getTabStop(float position)
    {
      float num = this.PixelRatio * this.TextScale;
      if (this.TabStops != null && this.TabStops.Count > 0)
      {
        for (int index = 0; index < this.TabStops.Count; ++index)
        {
          if ((double) this.TabStops[index] * (double) num > (double) position)
            return (float) this.TabStops[index] * num;
        }
      }
      return this.TabSize > 0 ? position + (float) this.TabSize * num : position + (float) (this.Font.FontSize * 4) * num;
    }

    private void clipRight(dfRenderData destination, int startIndex)
    {
      float b = this.VectorOffset.x + this.MaxSize.x * this.PixelRatio;
      dfList<Vector3> vertices = destination.Vertices;
      dfList<Vector2> uv = destination.UV;
      for (int index1 = startIndex; index1 < vertices.Count; index1 += 4)
      {
        Vector3 vector3_1 = vertices[index1];
        Vector3 vector3_2 = vertices[index1 + 1];
        Vector3 vector3_3 = vertices[index1 + 2];
        Vector3 vector3_4 = vertices[index1 + 3];
        float num1 = vector3_2.x - vector3_1.x;
        if ((double) vector3_2.x > (double) b)
        {
          float t = (float) (1.0 - ((double) b - (double) vector3_2.x + (double) num1) / (double) num1);
          dfList<Vector3> dfList1 = vertices;
          int index2 = index1;
          vector3_1 = new Vector3(Mathf.Min(vector3_1.x, b), vector3_1.y, vector3_1.z);
          Vector3 vector3_5 = vector3_1;
          dfList1[index2] = vector3_5;
          dfList<Vector3> dfList2 = vertices;
          int index3 = index1 + 1;
          vector3_2 = new Vector3(Mathf.Min(vector3_2.x, b), vector3_2.y, vector3_2.z);
          Vector3 vector3_6 = vector3_2;
          dfList2[index3] = vector3_6;
          dfList<Vector3> dfList3 = vertices;
          int index4 = index1 + 2;
          vector3_3 = new Vector3(Mathf.Min(vector3_3.x, b), vector3_3.y, vector3_3.z);
          Vector3 vector3_7 = vector3_3;
          dfList3[index4] = vector3_7;
          dfList<Vector3> dfList4 = vertices;
          int index5 = index1 + 3;
          vector3_4 = new Vector3(Mathf.Min(vector3_4.x, b), vector3_4.y, vector3_4.z);
          Vector3 vector3_8 = vector3_4;
          dfList4[index5] = vector3_8;
          float x = Mathf.Lerp(uv[index1 + 1].x, uv[index1].x, t);
          uv[index1 + 1] = new Vector2(x, uv[index1 + 1].y);
          uv[index1 + 2] = new Vector2(x, uv[index1 + 2].y);
          float num2 = vector3_2.x - vector3_1.x;
        }
      }
    }

    private void clipBottom(dfRenderData destination, int startIndex)
    {
      float b = this.VectorOffset.y - this.MaxSize.y * this.PixelRatio;
      dfList<Vector3> vertices = destination.Vertices;
      dfList<Vector2> uv = destination.UV;
      dfList<Color32> colors = destination.Colors;
      for (int index1 = startIndex; index1 < vertices.Count; index1 += 4)
      {
        Vector3 vector3_1 = vertices[index1];
        Vector3 vector3_2 = vertices[index1 + 1];
        Vector3 vector3_3 = vertices[index1 + 2];
        Vector3 vector3_4 = vertices[index1 + 3];
        float num = vector3_1.y - vector3_4.y;
        if ((double) vector3_4.y <= (double) b)
        {
          float t = (float) (1.0 - (double) Mathf.Abs(-b + vector3_1.y) / (double) num);
          dfList<Vector3> dfList1 = vertices;
          int index2 = index1;
          vector3_1 = new Vector3(vector3_1.x, Mathf.Max(vector3_1.y, b), vector3_2.z);
          Vector3 vector3_5 = vector3_1;
          dfList1[index2] = vector3_5;
          dfList<Vector3> dfList2 = vertices;
          int index3 = index1 + 1;
          vector3_2 = new Vector3(vector3_2.x, Mathf.Max(vector3_2.y, b), vector3_2.z);
          Vector3 vector3_6 = vector3_2;
          dfList2[index3] = vector3_6;
          dfList<Vector3> dfList3 = vertices;
          int index4 = index1 + 2;
          vector3_3 = new Vector3(vector3_3.x, Mathf.Max(vector3_3.y, b), vector3_3.z);
          Vector3 vector3_7 = vector3_3;
          dfList3[index4] = vector3_7;
          dfList<Vector3> dfList4 = vertices;
          int index5 = index1 + 3;
          vector3_4 = new Vector3(vector3_4.x, Mathf.Max(vector3_4.y, b), vector3_4.z);
          Vector3 vector3_8 = vector3_4;
          dfList4[index5] = vector3_8;
          float y = Mathf.Lerp(uv[index1 + 3].y, uv[index1].y, t);
          uv[index1 + 3] = new Vector2(uv[index1 + 3].x, y);
          uv[index1 + 2] = new Vector2(uv[index1 + 2].x, y);
          Color color = Color.Lerp((Color) colors[index1 + 3], (Color) colors[index1], t);
          colors[index1 + 3] = (Color32) color;
          colors[index1 + 2] = (Color32) color;
        }
      }
    }

    private Color32 applyOpacity(Color32 color)
    {
      color.a = (byte) ((double) this.Opacity * (double) byte.MaxValue);
      return color;
    }

    private static void addTriangleIndices(dfList<Vector3> verts, dfList<int> triangles)
    {
      int count = verts.Count;
      for (int index = 0; index < dfFont.BitmappedFontRenderer.TRIANGLE_INDICES.Length; ++index)
        triangles.Add(count + dfFont.BitmappedFontRenderer.TRIANGLE_INDICES[index]);
    }

    private Color multiplyColors(Color lhs, Color rhs)
    {
      return new Color(lhs.r * rhs.r, lhs.g * rhs.g, lhs.b * rhs.b, lhs.a * rhs.a);
    }
  }

  private class LineRenderInfo
  {
    public int startOffset;
    public int endOffset;
    public float lineWidth;
    public float lineHeight;
    private static dfList<dfFont.LineRenderInfo> pool = new dfList<dfFont.LineRenderInfo>();
    private static int poolIndex = 0;

    private LineRenderInfo()
    {
    }

    public int length => this.endOffset - this.startOffset + 1;

    public static void ResetPool() => dfFont.LineRenderInfo.poolIndex = 0;

    public static dfFont.LineRenderInfo Obtain(int start, int end)
    {
      if (dfFont.LineRenderInfo.poolIndex >= dfFont.LineRenderInfo.pool.Count - 1)
        dfFont.LineRenderInfo.pool.Add(new dfFont.LineRenderInfo());
      dfFont.LineRenderInfo lineRenderInfo = dfFont.LineRenderInfo.pool[dfFont.LineRenderInfo.poolIndex++];
      lineRenderInfo.startOffset = start;
      lineRenderInfo.endOffset = end;
      lineRenderInfo.lineHeight = 0.0f;
      return lineRenderInfo;
    }
  }
}
