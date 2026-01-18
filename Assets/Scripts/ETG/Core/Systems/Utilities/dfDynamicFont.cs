using System;
using System.Collections.Generic;
using System.Globalization;

using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/User Interface/Dynamic Font")]
[ExecuteInEditMode]
[Serializable]
public class dfDynamicFont : dfFontBase
    {
        private static List<dfDynamicFont> loadedFonts = new List<dfDynamicFont>();
        [SerializeField]
        private Font baseFont;
        [SerializeField]
        private Material material;
        [SerializeField]
        private Shader shader;
        [SerializeField]
        private int baseFontSize = -1;
        [SerializeField]
        private int baseline = -1;
        [SerializeField]
        private int lineHeight;
        protected dfList<dfDynamicFont.FontCharacterRequest> requests = new dfList<dfDynamicFont.FontCharacterRequest>();

        public override Material Material
        {
            get
            {
                if ((UnityEngine.Object) this.baseFont != (UnityEngine.Object) null && (UnityEngine.Object) this.material != (UnityEngine.Object) null)
                {
                    this.material.mainTexture = this.baseFont.material.mainTexture;
                    this.material.shader = this.Shader;
                }
                return this.material;
            }
            set
            {
                if (!((UnityEngine.Object) value != (UnityEngine.Object) this.material))
                    return;
                this.material = value;
                dfGUIManager.RefreshAll();
            }
        }

        public Shader Shader
        {
            get
            {
                if ((UnityEngine.Object) this.shader == (UnityEngine.Object) null)
                    this.shader = Shader.Find("Daikon Forge/Dynamic Font Shader");
                return this.shader;
            }
            set
            {
                this.shader = value;
                dfGUIManager.RefreshAll();
            }
        }

        public override Texture Texture => this.baseFont.material.mainTexture;

        public override bool IsValid
        {
            get
            {
                return (UnityEngine.Object) this.baseFont != (UnityEngine.Object) null && (UnityEngine.Object) this.Material != (UnityEngine.Object) null && (UnityEngine.Object) this.Texture != (UnityEngine.Object) null;
            }
        }

        public override int FontSize
        {
            get => this.baseFontSize;
            set
            {
                if (value == this.baseFontSize)
                    return;
                this.baseFontSize = value;
                dfGUIManager.RefreshAll();
            }
        }

        public override int LineHeight
        {
            get => this.lineHeight;
            set
            {
                if (value == this.lineHeight)
                    return;
                this.lineHeight = value;
                dfGUIManager.RefreshAll();
            }
        }

        public override dfFontRendererBase ObtainRenderer()
        {
            return dfDynamicFont.DynamicFontRenderer.Obtain(this);
        }

        public Font BaseFont
        {
            get => this.baseFont;
            set
            {
                if (!((UnityEngine.Object) value != (UnityEngine.Object) this.baseFont))
                    return;
                this.baseFont = value;
                dfGUIManager.RefreshAll();
            }
        }

        public int Baseline
        {
            get => this.baseline;
            set
            {
                if (value == this.baseline)
                    return;
                this.baseline = value;
                dfGUIManager.RefreshAll();
            }
        }

        public int Descent => this.LineHeight - this.baseline;

        public static dfDynamicFont FindByName(string name)
        {
            for (int index = 0; index < dfDynamicFont.loadedFonts.Count; ++index)
            {
                if (string.Equals(dfDynamicFont.loadedFonts[index].name, name, StringComparison.OrdinalIgnoreCase))
                    return dfDynamicFont.loadedFonts[index];
            }
            GameObject gameObject = BraveResources.Load(name) as GameObject;
            if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
                return (dfDynamicFont) null;
            dfDynamicFont component = gameObject.GetComponent<dfDynamicFont>();
            if ((UnityEngine.Object) component == (UnityEngine.Object) null)
                return (dfDynamicFont) null;
            dfDynamicFont.loadedFonts.Add(component);
            return component;
        }

        public Vector2 MeasureText(string text, int size, FontStyle style)
        {
            this.RequestCharacters(text, size, style);
            Vector2 vector2 = new Vector2(0.0f, (float) Mathf.CeilToInt((float) this.Baseline * ((float) size / (float) this.FontSize)));
            CharacterInfo info = new CharacterInfo();
            for (int index = 0; index < text.Length; ++index)
            {
                this.BaseFont.GetCharacterInfo(text[index], out info, size, style);
                float num = Mathf.Ceil((float) info.maxX);
                if (text[index] == ' ')
                    num = Mathf.Ceil((float) info.advance);
                else if (text[index] == '\t')
                    num += (float) (size * 4);
                vector2.x += num;
            }
            return vector2;
        }

        public void RequestCharacters(string text, int size, FontStyle style)
        {
            if ((UnityEngine.Object) this.baseFont == (UnityEngine.Object) null)
                throw new NullReferenceException("Base Font not assigned: " + this.name);
            dfFontManager.Invalidate((dfFontBase) this);
            this.baseFont.RequestCharactersInTexture(text, size, style);
        }

        public virtual void AddCharacterRequest(string characters, int fontSize, FontStyle style)
        {
            dfFontManager.FlagPendingRequests((dfFontBase) this);
            dfDynamicFont.FontCharacterRequest characterRequest = dfDynamicFont.FontCharacterRequest.Obtain();
            characterRequest.Characters = characters;
            characterRequest.FontSize = fontSize;
            characterRequest.FontStyle = style;
            this.requests.Add(characterRequest);
        }

        public virtual void FlushCharacterRequests()
        {
            for (int index = 0; index < this.requests.Count; ++index)
            {
                dfDynamicFont.FontCharacterRequest request = this.requests[index];
                this.baseFont.RequestCharactersInTexture(request.Characters, request.FontSize, request.FontStyle);
            }
            this.requests.ReleaseItems();
        }

        protected class FontCharacterRequest : IPoolable
        {
            private static dfList<dfDynamicFont.FontCharacterRequest> pool = new dfList<dfDynamicFont.FontCharacterRequest>();
            public string Characters;
            public int FontSize;
            public FontStyle FontStyle;

            public static dfDynamicFont.FontCharacterRequest Obtain()
            {
                return dfDynamicFont.FontCharacterRequest.pool.Count > 0 ? dfDynamicFont.FontCharacterRequest.pool.Pop() : new dfDynamicFont.FontCharacterRequest();
            }

            public void Release()
            {
                this.Characters = (string) null;
                this.FontSize = 0;
                this.FontStyle = FontStyle.Normal;
                dfDynamicFont.FontCharacterRequest.pool.Add(this);
            }
        }

        public class DynamicFontRenderer : dfFontRendererBase, IPoolable
        {
            private static Queue<dfDynamicFont.DynamicFontRenderer> objectPool = new Queue<dfDynamicFont.DynamicFontRenderer>();
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
            private dfList<dfDynamicFont.LineRenderInfo> lines;
            private dfList<dfMarkupToken> tokens;
            private bool inUse;

            internal DynamicFontRenderer()
            {
            }

            public int LineCount => this.lines.Count;

            public dfAtlas SpriteAtlas { get; set; }

            public dfRenderData SpriteBuffer { get; set; }

            public static dfFontRendererBase Obtain(dfDynamicFont font)
            {
                dfDynamicFont.DynamicFontRenderer dynamicFontRenderer = dfDynamicFont.DynamicFontRenderer.objectPool.Count <= 0 ? new dfDynamicFont.DynamicFontRenderer() : dfDynamicFont.DynamicFontRenderer.objectPool.Dequeue();
                dynamicFontRenderer.Reset();
                dynamicFontRenderer.Font = (dfFontBase) font;
                dynamicFontRenderer.inUse = true;
                return (dfFontRendererBase) dynamicFontRenderer;
            }

            public override void Release()
            {
                if (!this.inUse)
                    return;
                this.inUse = false;
                this.Reset();
                if (this.tokens != null)
                {
                    this.tokens.Release();
                    this.tokens = (dfList<dfMarkupToken>) null;
                }
                if (this.lines != null)
                {
                    this.lines.ReleaseItems();
                    this.lines.Release();
                    this.lines = (dfList<dfDynamicFont.LineRenderInfo>) null;
                }
                this.BottomColor = new Color32?();
                dfDynamicFont.DynamicFontRenderer.objectPool.Enqueue(this);
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
                dfDynamicFont font = (dfDynamicFont) this.Font;
                int size = Mathf.CeilToInt((float) font.FontSize * this.TextScale);
                float[] characterWidths = new float[text.Length];
                float num1 = 0.0f;
                float num2 = 0.0f;
                font.RequestCharacters(text, size, FontStyle.Normal);
                CharacterInfo info = new CharacterInfo();
                int index = startIndex;
                while (index <= endIndex)
                {
                    if (font.BaseFont.GetCharacterInfo(text[index], out info, size, FontStyle.Normal))
                    {
                        if (text[index] == '\t')
                            num2 += (float) this.TabSize;
                        else if (text[index] == ' ')
                            num2 += (float) info.advance;
                        else
                            num2 += (float) info.maxX;
                        characterWidths[index] = (num2 - num1) * this.PixelRatio;
                    }
                    ++index;
                    num1 = num2;
                }
                return characterWidths;
            }

            public override Vector2 MeasureString(string text)
            {
                dfDynamicFont font = (dfDynamicFont) this.Font;
                int size = Mathf.CeilToInt((float) font.FontSize * this.TextScale);
                font.RequestCharacters(text, size, FontStyle.Normal);
                this.tokenize(text);
                dfList<dfDynamicFont.LineRenderInfo> linebreaks = this.calculateLinebreaks();
                float num = 0.0f;
                float y = 0.0f;
                for (int index = 0; index < linebreaks.Count; ++index)
                {
                    num = Mathf.Max(linebreaks[index].lineWidth, num);
                    y += linebreaks[index].lineHeight;
                }
                this.tokens.Release();
                this.tokens = (dfList<dfMarkupToken>) null;
                return new Vector2(num, y);
            }

            public override void Render(string text, dfRenderData destination)
            {
                dfDynamicFont.DynamicFontRenderer.textColors.Clear();
                dfDynamicFont.DynamicFontRenderer.textColors.Push((Color32) Color.white);
                dfDynamicFont font = (dfDynamicFont) this.Font;
                int size = Mathf.CeilToInt((float) font.FontSize * this.TextScale);
                font.RequestCharacters(text, size, FontStyle.Normal);
                this.tokenize(text);
                dfList<dfDynamicFont.LineRenderInfo> linebreaks = this.calculateLinebreaks();
                destination.EnsureCapacity(this.getAnticipatedVertCount(this.tokens));
                int b1 = 0;
                int b2 = 0;
                Vector3 position = (this.VectorOffset / this.PixelRatio).CeilToInt();
                for (int index = 0; index < linebreaks.Count; ++index)
                {
                    dfDynamicFont.LineRenderInfo lineRenderInfo = linebreaks[index];
                    int count1 = destination.Vertices.Count;
                    int count2 = this.SpriteBuffer == null ? 0 : this.SpriteBuffer.Vertices.Count;
                    this.renderLine(linebreaks[index], dfDynamicFont.DynamicFontRenderer.textColors, position, destination);
                    position.y -= lineRenderInfo.lineHeight;
                    b1 = Mathf.Max((int) lineRenderInfo.lineWidth, b1);
                    b2 += Mathf.CeilToInt(lineRenderInfo.lineHeight);
                    if ((double) lineRenderInfo.lineWidth > (double) this.MaxSize.x)
                    {
                        this.clipRight(destination, count1);
                        this.clipRight(this.SpriteBuffer, count2);
                    }
                    this.clipBottom(destination, count1);
                    this.clipBottom(this.SpriteBuffer, count2);
                }
                this.RenderedSize = new Vector2(Mathf.Min(this.MaxSize.x, (float) b1), Mathf.Min(this.MaxSize.y, (float) b2)) * this.TextScale;
                this.tokens.Release();
                this.tokens = (dfList<dfMarkupToken>) null;
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
                dfDynamicFont.LineRenderInfo line,
                Stack<Color32> colors,
                Vector3 position,
                dfRenderData destination)
            {
                position.x += (float) this.calculateLineAlignment(line);
                for (int startOffset = line.startOffset; startOffset <= line.endOffset; ++startOffset)
                {
                    dfMarkupToken token = this.tokens[startOffset];
                    switch (token.TokenType)
                    {
                        case dfMarkupTokenType.Text:
                            this.renderText(token, colors.Peek(), position, destination);
                            break;
                        case dfMarkupTokenType.StartTag:
                            if (token.Matches("sprite") && (UnityEngine.Object) this.SpriteAtlas != (UnityEngine.Object) null && this.SpriteBuffer != null)
                            {
                                this.renderSprite(token, colors.Peek(), position, this.SpriteBuffer);
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
                    position.x += (float) token.Width;
                    if (token.Width > 0)
                        position += this.PerCharacterAccumulatedOffset;
                }
            }

            private void renderText(
                dfMarkupToken token,
                Color32 color,
                Vector3 position,
                dfRenderData renderData)
            {
                try
                {
                    dfDynamicFont font = (dfDynamicFont) this.Font;
                    int size = Mathf.CeilToInt((float) font.FontSize * this.TextScale);
                    FontStyle style = FontStyle.Normal;
                    CharacterInfo info = new CharacterInfo();
                    int descent = font.Descent;
                    dfList<Vector3> vertices = renderData.Vertices;
                    dfList<int> triangles = renderData.Triangles;
                    dfList<Vector2> uv = renderData.UV;
                    dfList<Color32> colors = renderData.Colors;
                    float x1 = position.x;
                    float y1 = position.y;
                    renderData.Material = font.Material;
                    Color32 color32_1 = this.applyOpacity((Color32) this.multiplyColors((Color) color, (Color) this.DefaultColor));
                    Color32 color32_2 = color32_1;
                    if (this.BottomColor.HasValue)
                        color32_2 = this.applyOpacity((Color32) this.multiplyColors((Color) color, (Color) this.BottomColor.Value));
                    for (int index1 = 0; index1 < token.Length; ++index1)
                    {
                        if (index1 > 0)
                            x1 += (float) this.CharacterSpacing * this.TextScale;
                        if (font.baseFont.GetCharacterInfo(token[index1], out info, size, style))
                        {
                            int num = font.FontSize + info.maxY - size + descent;
                            float x2 = x1 + (float) info.minX;
                            float y2 = y1 + (float) num;
                            float x3 = x2 + (float) info.glyphWidth;
                            float y3 = y2 - (float) info.glyphHeight;
                            Vector3 vector3_1 = new Vector3(x2, y2) * this.PixelRatio;
                            Vector3 vector3_2 = new Vector3(x3, y2) * this.PixelRatio;
                            Vector3 vector3_3 = new Vector3(x3, y3) * this.PixelRatio;
                            Vector3 vector3_4 = new Vector3(x2, y3) * this.PixelRatio;
                            if (this.Shadow)
                            {
                                dfDynamicFont.DynamicFontRenderer.addTriangleIndices(vertices, triangles);
                                Vector3 vector3_5 = (Vector3) this.ShadowOffset * this.PixelRatio;
                                vertices.Add(vector3_1 + vector3_5);
                                vertices.Add(vector3_2 + vector3_5);
                                vertices.Add(vector3_3 + vector3_5);
                                vertices.Add(vector3_4 + vector3_5);
                                Color32 color32_3 = this.applyOpacity(this.ShadowColor);
                                colors.Add(color32_3);
                                colors.Add(color32_3);
                                colors.Add(color32_3);
                                colors.Add(color32_3);
                                dfDynamicFont.DynamicFontRenderer.addUVCoords(uv, info);
                            }
                            if (this.Outline)
                            {
                                for (int index2 = 0; index2 < dfDynamicFont.DynamicFontRenderer.OUTLINE_OFFSETS.Length; ++index2)
                                {
                                    dfDynamicFont.DynamicFontRenderer.addTriangleIndices(vertices, triangles);
                                    Vector3 vector3_6 = (Vector3) dfDynamicFont.DynamicFontRenderer.OUTLINE_OFFSETS[index2] * (float) this.OutlineSize * this.PixelRatio;
                                    vertices.Add(vector3_1 + vector3_6);
                                    vertices.Add(vector3_2 + vector3_6);
                                    vertices.Add(vector3_3 + vector3_6);
                                    vertices.Add(vector3_4 + vector3_6);
                                    Color32 color32_4 = this.applyOpacity(this.OutlineColor);
                                    colors.Add(color32_4);
                                    colors.Add(color32_4);
                                    colors.Add(color32_4);
                                    colors.Add(color32_4);
                                    dfDynamicFont.DynamicFontRenderer.addUVCoords(uv, info);
                                }
                            }
                            dfDynamicFont.DynamicFontRenderer.addTriangleIndices(vertices, triangles);
                            vertices.Add(vector3_1);
                            vertices.Add(vector3_2);
                            vertices.Add(vector3_3);
                            vertices.Add(vector3_4);
                            colors.Add(color32_1);
                            colors.Add(color32_1);
                            colors.Add(color32_2);
                            colors.Add(color32_2);
                            dfDynamicFont.DynamicFontRenderer.addUVCoords(uv, info);
                            x1 += (float) Mathf.CeilToInt((float) info.maxX);
                        }
                    }
                }
                finally
                {
                }
            }

            private static void addUVCoords(dfList<Vector2> uvs, CharacterInfo glyph)
            {
                uvs.Add(glyph.uvTopLeft);
                uvs.Add(glyph.uvTopRight);
                uvs.Add(glyph.uvBottomRight);
                uvs.Add(glyph.uvBottomLeft);
            }

            private void renderSprite(
                dfMarkupToken token,
                Color32 color,
                Vector3 position,
                dfRenderData destination)
            {
                try
                {
                    dfAtlas.ItemInfo spriteAtla = this.SpriteAtlas[token.GetAttribute(0).Value.Value];
                    if (spriteAtla == (dfAtlas.ItemInfo) null)
                        return;
                    dfSprite.renderSprite(this.SpriteBuffer, new dfSprite.RenderOptions()
                    {
                        atlas = this.SpriteAtlas,
                        color = color,
                        fillAmount = 1f,
                        flip = dfSpriteFlip.None,
                        offset = position,
                        pixelsToUnits = this.PixelRatio,
                        size = new Vector2((float) token.Width, (float) token.Height),
                        spriteInfo = spriteAtla
                    });
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
                        color1 = (Color) this.UIntToColor(result | 4278190080U);
                    }
                    else
                        color1 = dfMarkupStyle.ParseColor(color2, (Color) this.DefaultColor);
                }
                return this.applyOpacity((Color32) color1);
            }

            private Color32 UIntToColor(uint color)
            {
                byte a = (byte) (color >> 24);
                return new Color32((byte) (color >> 16), (byte) (color >> 8), (byte) (color >> 0), a);
            }

            private dfList<dfDynamicFont.LineRenderInfo> calculateLinebreaks()
            {
                try
                {
                    if (this.lines != null)
                        return this.lines;
                    this.lines = dfList<dfDynamicFont.LineRenderInfo>.Obtain();
                    dfDynamicFont font = (dfDynamicFont) this.Font;
                    int num1 = 0;
                    int start = 0;
                    int num2 = 0;
                    int num3 = 0;
                    float num4 = (float) font.Baseline * this.TextScale;
                    while (num2 < this.tokens.Count && (double) this.lines.Count * (double) num4 <= (double) this.MaxSize.y + (double) num4)
                    {
                        dfMarkupToken token = this.tokens[num2];
                        dfMarkupTokenType tokenType = token.TokenType;
                        if (tokenType == dfMarkupTokenType.Newline)
                        {
                            this.lines.Add(dfDynamicFont.LineRenderInfo.Obtain(start, num2));
                            int num5;
                            num2 = num5 = num2 + 1;
                            num1 = num5;
                            start = num5;
                            num3 = 0;
                        }
                        else
                        {
                            int num6 = Mathf.CeilToInt((float) token.Width);
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
                                    this.lines.Add(dfDynamicFont.LineRenderInfo.Obtain(start, num1 - 1));
                                    int num8;
                                    num1 = num8 = num1 + 1;
                                    num2 = num8;
                                    start = num8;
                                    num3 = 0;
                                }
                                else
                                {
                                    this.lines.Add(dfDynamicFont.LineRenderInfo.Obtain(start, num1 - 1));
                                    int num9;
                                    num2 = num9 = num2 + 1;
                                    num1 = num9;
                                    start = num9;
                                    num3 = 0;
                                }
                            }
                            else
                            {
                                if (tokenType == dfMarkupTokenType.Whitespace)
                                    num1 = num2;
                                num3 += num6;
                                ++num2;
                            }
                        }
                    }
                    if (start < this.tokens.Count)
                        this.lines.Add(dfDynamicFont.LineRenderInfo.Obtain(start, this.tokens.Count - 1));
                    for (int index = 0; index < this.lines.Count; ++index)
                        this.calculateLineSize(this.lines[index]);
                    return this.lines;
                }
                finally
                {
                }
            }

            private int calculateLineAlignment(dfDynamicFont.LineRenderInfo line)
            {
                float lineWidth = line.lineWidth;
                if (this.TextAlign == TextAlignment.Left || (double) lineWidth < 1.0)
                    return 0;
                return Mathf.CeilToInt(Mathf.Max(0.0f, this.TextAlign != TextAlignment.Right ? (float) (((double) this.MaxSize.x - (double) lineWidth) * 0.5) : this.MaxSize.x - lineWidth));
            }

            private void calculateLineSize(dfDynamicFont.LineRenderInfo line)
            {
                dfDynamicFont font = (dfDynamicFont) this.Font;
                line.lineHeight = (float) font.Baseline * this.TextScale;
                int num = 0;
                for (int startOffset = line.startOffset; startOffset <= line.endOffset; ++startOffset)
                    num += this.tokens[startOffset].Width;
                line.lineWidth = (float) num;
            }

            private void tokenize(string text)
            {
                try
                {
                    this.tokens = !this.ProcessMarkup ? dfPlainTextTokenizer.Tokenize(text) : dfMarkupTokenizer.Tokenize(text);
                    for (int index = 0; index < this.tokens.Count; ++index)
                        this.calculateTokenRenderSize(this.tokens[index]);
                }
                finally
                {
                }
            }

            private void calculateTokenRenderSize(dfMarkupToken token)
            {
                try
                {
                    float f1 = 0.0f;
                    dfDynamicFont font = (dfDynamicFont) this.Font;
                    CharacterInfo info = new CharacterInfo();
                    if (token.TokenType == dfMarkupTokenType.Text)
                    {
                        int size = Mathf.CeilToInt((float) font.FontSize * this.TextScale);
                        for (int index = 0; index < token.Length; ++index)
                        {
                            char ch = token[index];
                            font.baseFont.GetCharacterInfo(ch, out info, size, FontStyle.Normal);
                            if (ch == '\t')
                                f1 += (float) this.TabSize;
                            else
                                f1 += ch == ' ' ? (float) info.advance + (float) this.CharacterSpacing * this.TextScale : (float) info.maxX;
                        }
                        if (token.Length > 2)
                            f1 += (float) ((token.Length - 2) * this.CharacterSpacing) * this.TextScale;
                        token.Height = this.Font.LineHeight;
                        token.Width = Mathf.CeilToInt(f1);
                    }
                    else if (token.TokenType == dfMarkupTokenType.Whitespace)
                    {
                        int size = Mathf.CeilToInt((float) font.FontSize * this.TextScale);
                        float num = (float) this.CharacterSpacing * this.TextScale;
                        for (int index = 0; index < token.Length; ++index)
                        {
                            char ch = token[index];
                            switch (ch)
                            {
                                case '\t':
                                    f1 += (float) this.TabSize;
                                    break;
                                case ' ':
                                    font.baseFont.GetCharacterInfo(ch, out info, size, FontStyle.Normal);
                                    f1 += (float) info.advance + num;
                                    break;
                            }
                        }
                        token.Height = this.Font.LineHeight;
                        token.Width = Mathf.CeilToInt(f1);
                    }
                    else
                    {
                        if (token.TokenType != dfMarkupTokenType.StartTag || !token.Matches("sprite") || !((UnityEngine.Object) this.SpriteAtlas != (UnityEngine.Object) null) || token.AttributeCount != 1)
                            return;
                        Texture2D texture = this.SpriteAtlas.Texture;
                        float f2 = (float) font.Baseline * this.TextScale;
                        dfAtlas.ItemInfo spriteAtla = this.SpriteAtlas[token.GetAttribute(0).Value.Value];
                        if (spriteAtla != (dfAtlas.ItemInfo) null)
                        {
                            float num = (float) ((double) spriteAtla.region.width * (double) texture.width / ((double) spriteAtla.region.height * (double) texture.height));
                            f1 = (float) Mathf.CeilToInt(f2 * num);
                        }
                        token.Height = Mathf.CeilToInt(f2);
                        token.Width = Mathf.CeilToInt(f1);
                    }
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
                if (destination == null)
                    return;
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
                if (destination == null)
                    return;
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
                        uv[index1 + 3] = Vector2.Lerp(uv[index1 + 3], uv[index1], t);
                        uv[index1 + 2] = Vector2.Lerp(uv[index1 + 2], uv[index1 + 1], t);
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
                foreach (int triangleIndex in dfDynamicFont.DynamicFontRenderer.TRIANGLE_INDICES)
                    triangles.Add(count + triangleIndex);
            }

            private Color multiplyColors(Color lhs, Color rhs)
            {
                return new Color(lhs.r * rhs.r, lhs.g * rhs.g, lhs.b * rhs.b, lhs.a * rhs.a);
            }
        }

        private class LineRenderInfo : IPoolable
        {
            public int startOffset;
            public int endOffset;
            public float lineWidth;
            public float lineHeight;
            private static dfList<dfDynamicFont.LineRenderInfo> pool = new dfList<dfDynamicFont.LineRenderInfo>();

            private LineRenderInfo()
            {
            }

            public int length => this.endOffset - this.startOffset + 1;

            public static dfDynamicFont.LineRenderInfo Obtain(int start, int end)
            {
                dfDynamicFont.LineRenderInfo lineRenderInfo = dfDynamicFont.LineRenderInfo.pool.Count <= 0 ? new dfDynamicFont.LineRenderInfo() : dfDynamicFont.LineRenderInfo.pool.Pop();
                lineRenderInfo.startOffset = start;
                lineRenderInfo.endOffset = end;
                lineRenderInfo.lineHeight = 0.0f;
                return lineRenderInfo;
            }

            public void Release()
            {
                this.startOffset = this.endOffset = 0;
                this.lineWidth = this.lineHeight = 0.0f;
                dfDynamicFont.LineRenderInfo.pool.Add(this);
            }
        }
    }

