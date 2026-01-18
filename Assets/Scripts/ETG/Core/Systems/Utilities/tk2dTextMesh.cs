// Decompiled with JetBrains decompiler
// Type: tk2dTextMesh
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Text;
using tk2dRuntime;
using UnityEngine;

#nullable disable

[RequireComponent(typeof (MeshFilter))]
[RequireComponent(typeof (MeshRenderer))]
[AddComponentMenu("2D Toolkit/Text/tk2dTextMesh")]
[ExecuteInEditMode]
public class tk2dTextMesh : MonoBehaviour, ISpriteCollectionForceBuild
  {
    private tk2dFontData _fontInst;
    private string _formattedText = string.Empty;
    [SerializeField]
    private tk2dFontData _font;
    [SerializeField]
    private string _text = string.Empty;
    [SerializeField]
    private Color _color = Color.white;
    [SerializeField]
    private Color _color2 = Color.white;
    [SerializeField]
    private bool _useGradient;
    [SerializeField]
    private int _textureGradient;
    [SerializeField]
    private TextAnchor _anchor = TextAnchor.LowerLeft;
    [SerializeField]
    private Vector3 _scale = new Vector3(1f, 1f, 1f);
    [SerializeField]
    private bool _kerning;
    [SerializeField]
    private int _maxChars = 16 /*0x10*/;
    [SerializeField]
    private bool _inlineStyling;
    [SerializeField]
    public bool supportsWooblyText;
    [SerializeField]
    public int[] woobleStartIndices;
    [SerializeField]
    public int[] woobleEndIndices;
    [SerializeField]
    public tk2dTextMesh.WoobleStyle[] woobleStyles;
    public int visibleCharacters = int.MaxValue;
    [SerializeField]
    private bool _formatting;
    [SerializeField]
    private int _wordWrapWidth;
    [SerializeField]
    private float spacing;
    [SerializeField]
    private float lineSpacing;
    [SerializeField]
    private tk2dTextMeshData data = new tk2dTextMeshData();
    private Vector3[] vertices;
    private Vector2[] uvs;
    private Vector2[] uv2;
    private Color32[] colors;
    private Color32[] untintedColors;
    private tk2dTextMesh.UpdateFlags updateFlags = tk2dTextMesh.UpdateFlags.UpdateBuffers;
    private Mesh mesh;
    private MeshFilter meshFilter;
    private Renderer _cachedRenderer;
    protected Vector2[] woobleBuffer;
    protected bool[] woobleRainbowBuffer;
    protected float[] woobleTimes;
    protected List<int> indices;
    private List<GameObject> m_inlineSprites = new List<GameObject>();
    private List<int> m_inlineSpriteIndices = new List<int>();
    private tk2dFontData m_defaultAssignedFont;

    public string FormattedText => this._formattedText;

    private void UpgradeData()
    {
      if (this.data.version != 1)
      {
        this.data.font = this._font;
        this.data.text = this._text;
        this.data.color = this._color;
        this.data.color2 = this._color2;
        this.data.useGradient = this._useGradient;
        this.data.textureGradient = this._textureGradient;
        this.data.anchor = this._anchor;
        this.data.scale = this._scale;
        this.data.kerning = this._kerning;
        this.data.maxChars = this._maxChars;
        this.data.inlineStyling = this._inlineStyling;
        this.data.formatting = this._formatting;
        this.data.wordWrapWidth = this._wordWrapWidth;
        this.data.spacing = this.spacing;
        this.data.lineSpacing = this.lineSpacing;
      }
      this.data.version = 1;
    }

    private static int GetInlineStyleCommandLength(int cmdSymbol)
    {
      int styleCommandLength = 0;
      switch (cmdSymbol)
      {
        case 67:
          styleCommandLength = 9;
          break;
        case 71:
          styleCommandLength = 17;
          break;
        case 99:
          styleCommandLength = 5;
          break;
        case 103:
          styleCommandLength = 9;
          break;
      }
      return styleCommandLength;
    }

    public string FormatText(string unformattedString)
    {
      string empty = string.Empty;
      this.FormatText(ref empty, unformattedString);
      return empty;
    }

    private void FormatText() => this.FormatText(ref this._formattedText, this.data.text);

    public string GetStrippedWoobleString(string _source)
    {
      for (int index1 = 0; index1 < _source.Length; ++index1)
      {
        if (_source[index1] == '{' && _source[index1 + 1] == 'w' && _source[index1 + 3] == '}')
        {
          int num = -1;
          for (int index2 = index1 + 3; index2 < _source.Length; ++index2)
          {
            if (_source[index2] == '{' && _source[index2 + 1] == 'w')
            {
              num = index2 - 5;
              _source = _source.Remove(index2, 3);
              break;
            }
          }
          if (num != -1)
            _source = _source.Remove(index1, 4);
        }
      }
      this.FormatText(ref this._formattedText, _source, true);
      return _source;
    }

    public string PreprocessWoobleSignifiers(string _source)
    {
      List<tk2dTextMesh.WoobleDefinition> woobleDefinitionList = new List<tk2dTextMesh.WoobleDefinition>();
      for (int index1 = 0; index1 < _source.Length; ++index1)
      {
        if (_source[index1] == '{' && _source[index1 + 1] == 'w' && _source[index1 + 3] == '}')
        {
          int num = -1;
          for (int index2 = index1 + 3; index2 < _source.Length; ++index2)
          {
            if (_source[index2] == '{' && _source[index2 + 1] == 'w')
            {
              num = index2 - 5;
              _source = _source.Remove(index2, 3);
              break;
            }
          }
          if (num != -1)
          {
            string str = _source.Substring(index1, 4);
            _source = _source.Remove(index1, 4);
            char ch = str[2];
            tk2dTextMesh.WoobleDefinition woobleDefinition = new tk2dTextMesh.WoobleDefinition();
            switch (ch)
            {
              case 'b':
                woobleDefinition.style = tk2dTextMesh.WoobleStyle.SEQUENTIAL_RAINBOW;
                break;
              case 'j':
                woobleDefinition.style = tk2dTextMesh.WoobleStyle.RANDOM_JITTER;
                break;
              case 'q':
                woobleDefinition.style = tk2dTextMesh.WoobleStyle.SEQUENTIAL;
                break;
              case 'r':
                woobleDefinition.style = tk2dTextMesh.WoobleStyle.RANDOM_SEQUENTIAL;
                break;
              case 's':
                woobleDefinition.style = tk2dTextMesh.WoobleStyle.SIMULTANEOUS;
                break;
            }
            woobleDefinition.startIndex = index1;
            woobleDefinition.endIndex = num;
            woobleDefinitionList.Add(woobleDefinition);
          }
        }
      }
      this.woobleStartIndices = new int[woobleDefinitionList.Count];
      this.woobleEndIndices = new int[woobleDefinitionList.Count];
      this.woobleStyles = new tk2dTextMesh.WoobleStyle[woobleDefinitionList.Count];
      for (int index = 0; index < woobleDefinitionList.Count; ++index)
      {
        this.woobleStartIndices[index] = woobleDefinitionList[index].startIndex;
        this.woobleEndIndices[index] = woobleDefinitionList[index].endIndex;
        this.woobleStyles[index] = woobleDefinitionList[index].style;
      }
      this.FormatText(ref this._formattedText, _source, true);
      return _source;
    }

    private void PushbackWooblesByAmount(int newCharIndex, int amt, int max)
    {
      for (int index = 0; index < this.woobleStyles.Length; ++index)
      {
        if (this.woobleStartIndices[index] >= newCharIndex)
          this.woobleStartIndices[index] = Mathf.Min(this.woobleStartIndices[index] + amt, max);
        if (this.woobleEndIndices[index] >= newCharIndex)
          this.woobleEndIndices[index] = Mathf.Min(this.woobleEndIndices[index] + amt, max);
      }
    }

    private void FormatText(ref string _targetString, string _source, bool doPushback = false)
    {
      this.InitInstance();
      if (!this.formatting || this.wordWrapWidth == 0 || this._fontInst.texelSize == Vector2.zero)
      {
        _targetString = _source;
      }
      else
      {
        float num1 = this._fontInst.texelSize.x * (float) this.wordWrapWidth;
        StringBuilder stringBuilder = new StringBuilder(_source.Length);
        float num2 = 0.0f;
        float num3 = 0.0f;
        int num4 = -1;
        int num5 = -1;
        bool flag1 = false;
        for (int index1 = 0; index1 < _source.Length; ++index1)
        {
          char key = _source[index1];
          tk2dFontChar tk2dFontChar = (tk2dFontChar) null;
          bool flag2 = key == '^';
          bool flag3 = false;
          if (key == '[' && index1 < _source.Length - 1 && _source[index1 + 1] != ']')
          {
            for (int index2 = index1; index2 < _source.Length; ++index2)
            {
              if (_source[index2] == ']')
              {
                flag3 = true;
                int num6 = index2 - index1 + 1;
                tk2dFontChar = tk2dTextGeomGen.GetSpecificSpriteCharDef(_source.Substring(index1 + 9, num6 - 10));
                for (int index3 = 0; index3 < num6; ++index3)
                {
                  if (index1 + index3 < _source.Length)
                    stringBuilder.Append(_source[index1 + index3]);
                }
                index1 += num6 - 1;
                break;
              }
            }
          }
          if (!flag3)
          {
            if (this._fontInst.useDictionary)
            {
              if (!this._fontInst.charDict.ContainsKey((int) key))
                key = char.MinValue;
              tk2dFontChar = this._fontInst.charDict[(int) key];
            }
            else
            {
              if ((int) key >= this._fontInst.chars.Length)
                key = char.MinValue;
              tk2dFontChar = this._fontInst.chars[(int) key];
            }
          }
          if (flag2)
            key = '^';
          if (flag1)
          {
            flag1 = false;
          }
          else
          {
            if (this.data.inlineStyling && key == '^' && index1 + 1 < _source.Length)
            {
              if (_source[index1 + 1] == '^')
              {
                flag1 = true;
                stringBuilder.Append('^');
              }
              else
              {
                int num7 = 1 + tk2dTextMesh.GetInlineStyleCommandLength((int) _source[index1 + 1]);
                for (int index4 = 0; index4 < num7; ++index4)
                {
                  if (index1 + index4 < _source.Length)
                    stringBuilder.Append(_source[index1 + index4]);
                }
                index1 += num7 - 1;
                continue;
              }
            }
            switch (key)
            {
              case '\n':
                num2 = 0.0f;
                num3 = 0.0f;
                num4 = stringBuilder.Length;
                num5 = index1;
                break;
              case ' ':
                num2 += (tk2dFontChar.advance + this.data.spacing) * this.data.scale.x;
                num3 = num2;
                num4 = stringBuilder.Length;
                num5 = index1;
                break;
              default:
                if ((double) num2 + (double) tk2dFontChar.p1.x * (double) this.data.scale.x > (double) num1)
                {
                  if ((double) num3 > 0.0)
                  {
                    num3 = 0.0f;
                    num2 = 0.0f;
                    stringBuilder.Remove(num4 + 1, stringBuilder.Length - num4 - 1);
                    stringBuilder.Append('\n');
                    index1 = num5;
                    if (doPushback)
                    {
                      this.PushbackWooblesByAmount(index1, 1, _source.Length);
                      continue;
                    }
                    continue;
                  }
                  stringBuilder.Append('\n');
                  if (doPushback)
                    this.PushbackWooblesByAmount(index1, 1, _source.Length);
                  num2 = (tk2dFontChar.advance + this.data.spacing) * this.data.scale.x;
                  break;
                }
                num2 += (tk2dFontChar.advance + this.data.spacing) * this.data.scale.x;
                break;
            }
            if (!flag3)
              stringBuilder.Append(key);
          }
        }
        _targetString = stringBuilder.ToString();
      }
    }

    private void SetNeedUpdate(tk2dTextMesh.UpdateFlags uf)
    {
      if (this.updateFlags == tk2dTextMesh.UpdateFlags.UpdateNone)
      {
        this.updateFlags |= uf;
        tk2dUpdateManager.QueueCommit(this);
      }
      else
        this.updateFlags |= uf;
    }

    public tk2dFontData font
    {
      get
      {
        this.UpgradeData();
        return this.data.font;
      }
      set
      {
        this.UpgradeData();
        this.data.font = value;
        this._fontInst = this.data.font.inst;
        this.SetNeedUpdate(tk2dTextMesh.UpdateFlags.UpdateText);
        this.UpdateMaterial();
      }
    }

    public bool formatting
    {
      get
      {
        this.UpgradeData();
        return this.data.formatting;
      }
      set
      {
        this.UpgradeData();
        if (this.data.formatting == value)
          return;
        this.data.formatting = value;
        this.SetNeedUpdate(tk2dTextMesh.UpdateFlags.UpdateText);
      }
    }

    public int wordWrapWidth
    {
      get
      {
        this.UpgradeData();
        return this.data.wordWrapWidth;
      }
      set
      {
        this.UpgradeData();
        if (this.data.wordWrapWidth == value)
          return;
        this.data.wordWrapWidth = value;
        this.SetNeedUpdate(tk2dTextMesh.UpdateFlags.UpdateText);
      }
    }

    public string text
    {
      get
      {
        this.UpgradeData();
        return this.data.text;
      }
      set
      {
        this.UpgradeData();
        this.data.text = value;
        this.SetNeedUpdate(tk2dTextMesh.UpdateFlags.UpdateText);
      }
    }

    public Color color
    {
      get
      {
        this.UpgradeData();
        return this.data.color;
      }
      set
      {
        this.UpgradeData();
        this.data.color = value;
        this.SetNeedUpdate(tk2dTextMesh.UpdateFlags.UpdateColors);
      }
    }

    public Color color2
    {
      get
      {
        this.UpgradeData();
        return this.data.color2;
      }
      set
      {
        this.UpgradeData();
        this.data.color2 = value;
        this.SetNeedUpdate(tk2dTextMesh.UpdateFlags.UpdateColors);
      }
    }

    public bool useGradient
    {
      get
      {
        this.UpgradeData();
        return this.data.useGradient;
      }
      set
      {
        this.UpgradeData();
        this.data.useGradient = value;
        this.SetNeedUpdate(tk2dTextMesh.UpdateFlags.UpdateColors);
      }
    }

    public TextAnchor anchor
    {
      get
      {
        this.UpgradeData();
        return this.data.anchor;
      }
      set
      {
        this.UpgradeData();
        this.data.anchor = value;
        this.SetNeedUpdate(tk2dTextMesh.UpdateFlags.UpdateText);
      }
    }

    public Vector3 scale
    {
      get
      {
        this.UpgradeData();
        return this.data.scale;
      }
      set
      {
        this.UpgradeData();
        this.data.scale = value;
        this.SetNeedUpdate(tk2dTextMesh.UpdateFlags.UpdateText);
      }
    }

    public bool kerning
    {
      get
      {
        this.UpgradeData();
        return this.data.kerning;
      }
      set
      {
        this.UpgradeData();
        this.data.kerning = value;
        this.SetNeedUpdate(tk2dTextMesh.UpdateFlags.UpdateText);
      }
    }

    public int maxChars
    {
      get
      {
        this.UpgradeData();
        return this.data.maxChars;
      }
      set
      {
        this.UpgradeData();
        this.data.maxChars = value;
        this.SetNeedUpdate(tk2dTextMesh.UpdateFlags.UpdateBuffers);
      }
    }

    public int textureGradient
    {
      get
      {
        this.UpgradeData();
        return this.data.textureGradient;
      }
      set
      {
        this.UpgradeData();
        this.data.textureGradient = value % this.font.gradientCount;
        this.SetNeedUpdate(tk2dTextMesh.UpdateFlags.UpdateText);
      }
    }

    public bool inlineStyling
    {
      get
      {
        this.UpgradeData();
        return this.data.inlineStyling;
      }
      set
      {
        this.UpgradeData();
        this.data.inlineStyling = value;
        this.SetNeedUpdate(tk2dTextMesh.UpdateFlags.UpdateText);
      }
    }

    public float Spacing
    {
      get
      {
        this.UpgradeData();
        return this.data.spacing;
      }
      set
      {
        this.UpgradeData();
        if ((double) this.data.spacing == (double) value)
          return;
        this.data.spacing = value;
        this.SetNeedUpdate(tk2dTextMesh.UpdateFlags.UpdateText);
      }
    }

    public float LineSpacing
    {
      get
      {
        this.UpgradeData();
        return this.data.lineSpacing;
      }
      set
      {
        this.UpgradeData();
        if ((double) this.data.lineSpacing == (double) value)
          return;
        this.data.lineSpacing = value;
        this.SetNeedUpdate(tk2dTextMesh.UpdateFlags.UpdateText);
      }
    }

    public int SortingOrder
    {
      get => this.CachedRenderer.sortingOrder;
      set
      {
        if (this.CachedRenderer.sortingOrder == value)
          return;
        this.data.renderLayer = value;
        this.CachedRenderer.sortingOrder = value;
      }
    }

    private void InitInstance()
    {
      if (!((UnityEngine.Object) this._fontInst == (UnityEngine.Object) null) || !((UnityEngine.Object) this.data.font != (UnityEngine.Object) null))
        return;
      this._fontInst = this.data.font.inst;
    }

    private Renderer CachedRenderer
    {
      get
      {
        if ((UnityEngine.Object) this._cachedRenderer == (UnityEngine.Object) null)
          this._cachedRenderer = this.GetComponent<Renderer>();
        return this._cachedRenderer;
      }
    }

    private void Awake()
    {
      this.UpgradeData();
      if ((UnityEngine.Object) this.data.font != (UnityEngine.Object) null)
        this._fontInst = this.data.font.inst;
      this.updateFlags = tk2dTextMesh.UpdateFlags.UpdateBuffers;
      if ((UnityEngine.Object) this.data.font != (UnityEngine.Object) null)
      {
        this.Init();
        this.UpdateMaterial();
      }
      this.updateFlags = tk2dTextMesh.UpdateFlags.UpdateNone;
    }

    private void Update()
    {
      if (!this.supportsWooblyText || !Application.isPlaying)
        return;
      this.UpdateWooblyTextBuffers();
    }

    protected void InitWooblyTextBuffers()
    {
      if (this.indices == null)
        this.indices = new List<int>();
      this.indices.Clear();
      int num1 = 0;
      if (this.woobleBuffer != null && this.woobleBuffer.Length == this.FormattedText.Length)
        return;
      if (this.woobleBuffer == null)
      {
        this.woobleBuffer = new Vector2[this.FormattedText.Length];
        this.woobleTimes = new float[this.FormattedText.Length];
        this.woobleRainbowBuffer = new bool[this.FormattedText.Length];
      }
      else
      {
        Array.Resize<float>(ref this.woobleTimes, this.FormattedText.Length);
        Array.Resize<Vector2>(ref this.woobleBuffer, this.FormattedText.Length);
        Array.Resize<bool>(ref this.woobleRainbowBuffer, this.FormattedText.Length);
      }
      for (int index1 = 0; index1 < this.woobleStartIndices.Length; ++index1)
      {
        int woobleStartIndex = this.woobleStartIndices[index1];
        int woobleEndIndex = this.woobleEndIndices[index1];
        switch (this.woobleStyles[index1])
        {
          case tk2dTextMesh.WoobleStyle.SEQUENTIAL:
            for (int index2 = woobleStartIndex; index2 <= woobleEndIndex; ++index2)
            {
              if (index2 >= 0 && index2 < this.woobleTimes.Length && woobleEndIndex + 1 - woobleStartIndex > 0)
              {
                float num2 = ((float) index2 - (float) woobleStartIndex * 1f) / (float) (woobleEndIndex + 1 - woobleStartIndex);
                this.woobleTimes[index2] = -1f * num2;
                this.woobleRainbowBuffer[index2] = false;
              }
            }
            break;
          case tk2dTextMesh.WoobleStyle.RANDOM_JITTER:
            for (int index3 = woobleStartIndex; index3 <= woobleEndIndex; ++index3)
            {
              int b = woobleEndIndex - woobleStartIndex;
              int num3 = Mathf.FloorToInt((float) ((double) b / 2.0 + 1.0));
              this.indices.Add(woobleStartIndex + num1);
              num1 = (num1 + num3) % Mathf.Max(1, b);
            }
            for (int index4 = woobleStartIndex; index4 <= woobleEndIndex; ++index4)
            {
              if (index4 >= 0 && index4 < this.woobleTimes.Length && woobleEndIndex + 1 - woobleStartIndex > 0)
              {
                float num4 = ((float) this.indices[index4 - woobleStartIndex] - (float) woobleStartIndex * 1f) / (float) (woobleEndIndex + 1 - woobleStartIndex);
                this.woobleTimes[index4] = -1f * num4;
                this.woobleRainbowBuffer[index4] = false;
              }
            }
            break;
          case tk2dTextMesh.WoobleStyle.RANDOM_SEQUENTIAL:
            for (int index5 = woobleStartIndex; index5 <= woobleEndIndex; ++index5)
            {
              int b = woobleEndIndex - woobleStartIndex;
              int num5 = Mathf.FloorToInt((float) ((double) b / 2.0 + 1.0));
              this.indices.Add(woobleStartIndex + num1);
              num1 = (num1 + num5) % Mathf.Max(1, b);
            }
            for (int index6 = woobleStartIndex; index6 <= woobleEndIndex; ++index6)
            {
              if (index6 >= 0 && index6 < this.woobleTimes.Length && woobleEndIndex + 1 - woobleStartIndex > 0)
              {
                float num6 = ((float) this.indices[index6 - woobleStartIndex] - (float) woobleStartIndex * 1f) / (float) (woobleEndIndex + 1 - woobleStartIndex);
                this.woobleTimes[index6] = -1f * num6;
                this.woobleRainbowBuffer[index6] = false;
              }
            }
            break;
          case tk2dTextMesh.WoobleStyle.SEQUENTIAL_RAINBOW:
            for (int index7 = woobleStartIndex; index7 <= woobleEndIndex; ++index7)
            {
              if (index7 >= 0 && index7 < this.woobleTimes.Length && woobleEndIndex + 1 - woobleStartIndex > 0)
              {
                float num7 = ((float) index7 - (float) woobleStartIndex * 1f) / (float) (woobleEndIndex + 1 - woobleStartIndex);
                this.woobleTimes[index7] = -1f * num7;
                this.woobleRainbowBuffer[index7] = true;
              }
            }
            break;
        }
      }
    }

    protected void UpdateWooblyTextBuffers()
    {
      if (this.woobleBuffer == null || this.woobleBuffer.Length != this.FormattedText.Length)
        this.InitWooblyTextBuffers();
      float num = 3f;
      for (int index1 = 0; index1 < this.woobleStartIndices.Length; ++index1)
      {
        int woobleStartIndex = this.woobleStartIndices[index1];
        int woobleEndIndex = this.woobleEndIndices[index1];
        switch (this.woobleStyles[index1])
        {
          case tk2dTextMesh.WoobleStyle.SEQUENTIAL:
          case tk2dTextMesh.WoobleStyle.RANDOM_SEQUENTIAL:
            for (int index2 = woobleStartIndex; index2 <= woobleEndIndex; ++index2)
            {
              if (index2 >= 0 && index2 < this.woobleTimes.Length)
              {
                this.woobleTimes[index2] = this.woobleTimes[index2] + BraveTime.DeltaTime * num;
                float y = (double) this.woobleTimes[index2] >= 0.0 ? (float) ((double) BraveMathCollege.HermiteInterpolation(Mathf.PingPong(this.woobleTimes[index2], 1f)) * 0.25 - 1.0 / 16.0) : 0.0f;
                this.woobleBuffer[index2] = new Vector2(0.0f, y);
              }
            }
            break;
          case tk2dTextMesh.WoobleStyle.SIMULTANEOUS:
            for (int index3 = woobleStartIndex; index3 <= woobleEndIndex; ++index3)
            {
              if (index3 >= 0 && index3 < this.woobleTimes.Length)
              {
                this.woobleTimes[index3] = this.woobleTimes[index3] + BraveTime.DeltaTime * num;
                float y = (double) this.woobleTimes[index3] >= 0.0 ? (float) ((double) BraveMathCollege.HermiteInterpolation(Mathf.PingPong(this.woobleTimes[index3], 1f)) * 0.25 - 1.0 / 16.0) : 0.0f;
                this.woobleBuffer[index3] = new Vector2(0.0f, y);
              }
            }
            break;
          case tk2dTextMesh.WoobleStyle.RANDOM_JITTER:
            for (int index4 = woobleStartIndex; index4 <= woobleEndIndex; ++index4)
            {
              if (index4 >= 0 && index4 < this.woobleTimes.Length)
              {
                this.woobleTimes[index4] = this.woobleTimes[index4] + BraveTime.DeltaTime * num;
                if ((double) this.woobleTimes[index4] > 1.0)
                {
                  --this.woobleTimes[index4];
                  this.woobleBuffer[index4] = Vector2.Scale(new Vector2(1f / 32f, 1f / 32f), BraveUtility.GetMajorAxis(UnityEngine.Random.insideUnitCircle.normalized));
                  this.woobleBuffer[index4].x = Mathf.Abs(this.woobleBuffer[index4].x);
                  this.woobleBuffer[index4 != woobleStartIndex ? index4 - 1 : woobleEndIndex] = Vector2.zero;
                }
              }
            }
            break;
          case tk2dTextMesh.WoobleStyle.SEQUENTIAL_RAINBOW:
            for (int index5 = woobleStartIndex; index5 <= woobleEndIndex; ++index5)
            {
              if (index5 >= 0 && index5 < this.woobleTimes.Length)
              {
                this.woobleTimes[index5] = this.woobleTimes[index5] + BraveTime.DeltaTime * num;
                float y = (double) this.woobleTimes[index5] >= 0.0 ? (float) ((double) BraveMathCollege.HermiteInterpolation(Mathf.PingPong(this.woobleTimes[index5], 1f)) * 0.25 - 1.0 / 16.0) : 0.0f;
                this.woobleBuffer[index5] = new Vector2(0.0f, y);
              }
            }
            break;
        }
      }
      this.SetNeedUpdate(tk2dTextMesh.UpdateFlags.UpdateText);
    }

    protected void OnDestroy()
    {
      if ((UnityEngine.Object) this.meshFilter == (UnityEngine.Object) null)
        this.meshFilter = this.GetComponent<MeshFilter>();
      if ((UnityEngine.Object) this.meshFilter != (UnityEngine.Object) null)
        this.mesh = this.meshFilter.sharedMesh;
      if (!(bool) (UnityEngine.Object) this.mesh)
        return;
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.mesh, true);
      this.meshFilter.mesh = (Mesh) null;
    }

    private bool useInlineStyling => this.inlineStyling && this._fontInst.textureGradients;

    public int NumDrawnCharacters()
    {
      int num = this.NumTotalCharacters();
      if (num > this.data.maxChars)
        num = this.data.maxChars;
      return num;
    }

    public int NumTotalCharacters()
    {
      this.InitInstance();
      if ((this.updateFlags & (tk2dTextMesh.UpdateFlags.UpdateText | tk2dTextMesh.UpdateFlags.UpdateBuffers)) != tk2dTextMesh.UpdateFlags.UpdateNone)
        this.FormatText();
      int num = 0;
      for (int index = 0; index < this._formattedText.Length; ++index)
      {
        int key = (int) this._formattedText[index];
        bool flag = key == 94;
        if (this._fontInst.useDictionary)
        {
          if (!this._fontInst.charDict.ContainsKey(key))
            key = 0;
        }
        else if (key >= this._fontInst.chars.Length)
          key = 0;
        if (flag)
          key = 94;
        if (key != 10)
        {
          if (this.data.inlineStyling && key == 94 && index + 1 < this._formattedText.Length)
          {
            if (this._formattedText[index + 1] == '^')
            {
              ++index;
            }
            else
            {
              index += tk2dTextMesh.GetInlineStyleCommandLength((int) this._formattedText[index + 1]);
              continue;
            }
          }
          ++num;
        }
      }
      return num;
    }

    [Obsolete("Use GetEstimatedMeshBoundsForString().size instead")]
    public Vector2 GetMeshDimensionsForString(string str)
    {
      return tk2dTextGeomGen.GetMeshDimensionsForString(str, tk2dTextGeomGen.Data(this.data, this._fontInst, this._formattedText));
    }

    public Bounds GetEstimatedMeshBoundsForString(string str)
    {
      this.InitInstance();
      tk2dTextGeomGen.GeomData geomData = tk2dTextGeomGen.Data(this.data, this._fontInst, this._formattedText);
      Vector2 dimensionsForString = tk2dTextGeomGen.GetMeshDimensionsForString(this.FormatText(str), geomData);
      float yanchorForHeight = tk2dTextGeomGen.GetYAnchorForHeight(dimensionsForString.y, geomData);
      float xanchorForWidth = tk2dTextGeomGen.GetXAnchorForWidth(dimensionsForString.x, geomData);
      float num = (this._fontInst.lineHeight + this.data.lineSpacing) * this.data.scale.y;
      return new Bounds(new Vector3(xanchorForWidth + dimensionsForString.x * 0.5f, yanchorForHeight + dimensionsForString.y * 0.5f + num, 0.0f), Vector3.Scale((Vector3) dimensionsForString, new Vector3(1f, -1f, 1f)));
    }

    public Bounds GetTrueBounds() => this.mesh.bounds;

    public void Init(bool force)
    {
      if (force)
        this.SetNeedUpdate(tk2dTextMesh.UpdateFlags.UpdateBuffers);
      this.Init();
    }

    private void UpdateRainbowStatus(bool[] rainbowValues)
    {
      if (Foyer.DoIntroSequence)
        return;
      bool flag = false;
      for (int index = 0; index < rainbowValues.Length; ++index)
        flag = flag || rainbowValues[index];
      if (flag)
        this.color = Color.white;
      else
        this.color = Color.black;
    }

    public void Init()
    {
      if (!(bool) (UnityEngine.Object) this._fontInst || (this.updateFlags & tk2dTextMesh.UpdateFlags.UpdateBuffers) == tk2dTextMesh.UpdateFlags.UpdateNone && !((UnityEngine.Object) this.mesh == (UnityEngine.Object) null))
        return;
      this._fontInst.InitDictionary();
      this.FormatText();
      tk2dTextGeomGen.GeomData geomData = tk2dTextGeomGen.Data(this.data, this._fontInst, this._formattedText);
      int numVertices;
      int numIndices;
      tk2dTextGeomGen.GetTextMeshGeomDesc(out numVertices, out numIndices, geomData);
      this.vertices = new Vector3[numVertices];
      this.uvs = new Vector2[numVertices];
      this.colors = new Color32[numVertices];
      this.untintedColors = new Color32[numVertices];
      if (this._fontInst.textureGradients)
        this.uv2 = new Vector2[numVertices];
      int[] indices = new int[numIndices];
      if (this.supportsWooblyText)
        this.InitWooblyTextBuffers();
      if (this.supportsWooblyText)
        this.UpdateRainbowStatus(this.woobleRainbowBuffer);
      int target = !this.supportsWooblyText ? tk2dTextGeomGen.SetTextMeshGeom(this.vertices, this.uvs, this.uv2, this.untintedColors, 0, geomData, this.visibleCharacters) : tk2dTextGeomGen.SetTextMeshGeom(this.vertices, this.uvs, this.uv2, this.untintedColors, 0, geomData, this.visibleCharacters, this.woobleBuffer, this.woobleRainbowBuffer);
      if (!this._fontInst.isPacked)
      {
        Color32 color = (Color32) this.data.color;
        Color32 color32_1 = (Color32) (!this.data.useGradient ? this.data.color : this.data.color2);
        for (int index = 0; index < numVertices; ++index)
        {
          Color32 color32_2 = index % 4 >= 2 ? color32_1 : color;
          byte r = (byte) ((int) this.untintedColors[index].r * (int) color32_2.r / (int) byte.MaxValue);
          byte g = (byte) ((int) this.untintedColors[index].g * (int) color32_2.g / (int) byte.MaxValue);
          byte b = (byte) ((int) this.untintedColors[index].b * (int) color32_2.b / (int) byte.MaxValue);
          byte a = (byte) ((int) this.untintedColors[index].a * (int) color32_2.a / (int) byte.MaxValue);
          if (this._fontInst.premultipliedAlpha)
          {
            r = (byte) ((int) r * (int) a / (int) byte.MaxValue);
            g = (byte) ((int) g * (int) a / (int) byte.MaxValue);
            b = (byte) ((int) b * (int) a / (int) byte.MaxValue);
          }
          this.colors[index] = new Color32(r, g, b, a);
        }
      }
      else
        this.colors = this.untintedColors;
      tk2dTextGeomGen.SetTextMeshIndices(indices, 0, 0, geomData, target);
      if ((UnityEngine.Object) this.mesh == (UnityEngine.Object) null)
      {
        if ((UnityEngine.Object) this.meshFilter == (UnityEngine.Object) null)
          this.meshFilter = this.GetComponent<MeshFilter>();
        this.mesh = new Mesh();
        this.mesh.hideFlags = HideFlags.DontSave;
        this.meshFilter.mesh = this.mesh;
      }
      else
        this.mesh.Clear();
      this.mesh.vertices = this.vertices;
      this.mesh.uv = this.uvs;
      if (this.font.textureGradients)
        this.mesh.uv2 = this.uv2;
      this.mesh.triangles = indices;
      this.mesh.colors32 = this.colors;
      this.mesh.RecalculateBounds();
      this.mesh.bounds = tk2dBaseSprite.AdjustedMeshBounds(this.mesh.bounds, this.data.renderLayer);
      this.updateFlags = tk2dTextMesh.UpdateFlags.UpdateNone;
    }

    public void Commit() => tk2dUpdateManager.FlushQueues();

    public void CheckFontsForLanguage()
    {
      this.InitInstance();
      if ((UnityEngine.Object) this.m_defaultAssignedFont == (UnityEngine.Object) null)
        this.m_defaultAssignedFont = this.font;
      tk2dFontData tk2dFontData = GameManager.Options.CurrentLanguage != StringTableManager.GungeonSupportedLanguages.JAPANESE ? (GameManager.Options.CurrentLanguage != StringTableManager.GungeonSupportedLanguages.RUSSIAN ? (GameManager.Options.CurrentLanguage != StringTableManager.GungeonSupportedLanguages.CHINESE ? (GameManager.Options.CurrentLanguage != StringTableManager.GungeonSupportedLanguages.KOREAN ? this.m_defaultAssignedFont : (ResourceCache.Acquire("Alternate Fonts/NanumGothic16TK2D") as GameObject).GetComponent<tk2dFont>().data) : (ResourceCache.Acquire("Alternate Fonts/SimSun12_TK2D") as GameObject).GetComponent<tk2dFont>().data) : (ResourceCache.Acquire("Alternate Fonts/PixelaCYR_15_TK2D") as GameObject).GetComponent<tk2dFont>().data) : (ResourceCache.Acquire("Alternate Fonts/JackeyFont_TK2D") as GameObject).GetComponent<tk2dFont>().data;
      if (!((UnityEngine.Object) tk2dFontData != (UnityEngine.Object) null) || !((UnityEngine.Object) this.font != (UnityEngine.Object) tk2dFontData))
        return;
      this.font = tk2dFontData;
      this.Init(true);
    }

    public void DoNotUse__CommitInternal()
    {
      this.InitInstance();
      this.CheckFontsForLanguage();
      if ((UnityEngine.Object) this._fontInst == (UnityEngine.Object) null)
        return;
      this._fontInst.InitDictionary();
      if ((this.updateFlags & tk2dTextMesh.UpdateFlags.UpdateBuffers) != tk2dTextMesh.UpdateFlags.UpdateNone || (UnityEngine.Object) this.mesh == (UnityEngine.Object) null)
      {
        this.Init();
      }
      else
      {
        if ((this.updateFlags & tk2dTextMesh.UpdateFlags.UpdateText) != tk2dTextMesh.UpdateFlags.UpdateNone)
        {
          this.FormatText();
          tk2dTextGeomGen.GeomData geomData = tk2dTextGeomGen.Data(this.data, this._fontInst, this._formattedText);
          if (this.supportsWooblyText && this.woobleBuffer.Length != this.FormattedText.Length)
            this.InitWooblyTextBuffers();
          if (this.supportsWooblyText)
            this.UpdateRainbowStatus(this.woobleRainbowBuffer);
          int num1 = !this.supportsWooblyText || !Application.isPlaying ? tk2dTextGeomGen.SetTextMeshGeom(this.vertices, this.uvs, this.uv2, this.untintedColors, 0, geomData, this.visibleCharacters) : tk2dTextGeomGen.SetTextMeshGeom(this.vertices, this.uvs, this.uv2, this.untintedColors, 0, geomData, this.visibleCharacters, this.woobleBuffer, this.woobleRainbowBuffer);
          float a1 = float.MaxValue;
          float a2 = float.MinValue;
          for (int index = 0; index < this.vertices.Length; ++index)
          {
            a1 = Mathf.Min(a1, this.vertices[index].x);
            a2 = Mathf.Max(a2, this.vertices[index].x);
          }
          int index1 = 0;
          int num2 = 0;
          int index2 = 0;
          for (int index3 = 0; index3 < geomData.formattedText.Length; ++index3)
          {
            string formattedText = geomData.formattedText;
            if (formattedText[index3] == '[' && index3 < formattedText.Length - 1 && formattedText[index3 + 1] != ']')
            {
              for (int index4 = index3; index4 < formattedText.Length; ++index4)
              {
                if (formattedText[index4] == ']')
                {
                  int num3 = index4 - index3;
                  string name = formattedText.Substring(index3 + 9, num3 - 10);
                  GameObject gameObject = this.m_inlineSprites.Count <= index1 ? (GameObject) UnityEngine.Object.Instantiate(BraveResources.Load("ControllerButtonSprite")) : this.m_inlineSprites[index1];
                  tk2dSprite component = gameObject.GetComponent<tk2dSprite>();
                  component.HeightOffGround = 3f;
                  DepthLookupManager.AssignRendererToSortingLayer(component.renderer, DepthLookupManager.GungeonSortingLayer.FOREGROUND);
                  gameObject.SetLayerRecursively(this.gameObject.layer);
                  component.spriteId = component.GetSpriteIdByName(name);
                  component.transform.parent = this.transform;
                  component.transform.localPosition = tk2dTextGeomGen.inlineSpriteOffsetsForLastString[index2];
                  if (!this.m_inlineSprites.Contains(gameObject))
                  {
                    this.m_inlineSprites.Add(gameObject);
                    this.m_inlineSpriteIndices.Add(index3 - num2);
                  }
                  else
                    this.m_inlineSpriteIndices[this.m_inlineSprites.IndexOf(gameObject)] = index3 - num2;
                  index3 += num3;
                  num2 += num3;
                  ++index2;
                  ++index1;
                  break;
                }
              }
            }
          }
          for (int index5 = index1; index5 < this.m_inlineSprites.Count; index5 = index5 - 1 + 1)
          {
            if (Application.isPlaying)
              UnityEngine.Object.Destroy((UnityEngine.Object) this.m_inlineSprites[index5]);
            else
              UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.m_inlineSprites[index5]);
            this.m_inlineSprites.RemoveAt(index5);
            this.m_inlineSpriteIndices.RemoveAt(index5);
          }
          for (int index6 = 0; index6 < this.m_inlineSprites.Count; ++index6)
          {
            if (this.m_inlineSpriteIndices[index6] > this.visibleCharacters)
              this.m_inlineSprites[index6].GetComponent<Renderer>().enabled = false;
            else
              this.m_inlineSprites[index6].GetComponent<Renderer>().enabled = true;
          }
          for (int index7 = num1; index7 < this.data.maxChars; ++index7)
            this.vertices[index7 * 4] = this.vertices[index7 * 4 + 1] = this.vertices[index7 * 4 + 2] = this.vertices[index7 * 4 + 3] = Vector3.zero;
          this.mesh.vertices = this.vertices;
          this.mesh.uv = this.uvs;
          if (this._fontInst.textureGradients)
            this.mesh.uv2 = this.uv2;
          if (this._fontInst.isPacked)
          {
            this.colors = this.untintedColors;
            this.mesh.colors32 = this.colors;
          }
          if (this.data.inlineStyling)
            this.SetNeedUpdate(tk2dTextMesh.UpdateFlags.UpdateColors);
          this.mesh.RecalculateBounds();
          this.mesh.bounds = tk2dBaseSprite.AdjustedMeshBounds(this.mesh.bounds, this.data.renderLayer);
        }
        if (!this.font.isPacked && (this.updateFlags & tk2dTextMesh.UpdateFlags.UpdateColors) != tk2dTextMesh.UpdateFlags.UpdateNone)
        {
          Color32 color = (Color32) this.data.color;
          Color32 color32_1 = (Color32) (!this.data.useGradient ? this.data.color : this.data.color2);
          for (int index = 0; index < this.colors.Length; ++index)
          {
            Color32 color32_2 = index % 4 >= 2 ? color32_1 : color;
            byte r = (byte) ((int) this.untintedColors[index].r * (int) color32_2.r / (int) byte.MaxValue);
            byte g = (byte) ((int) this.untintedColors[index].g * (int) color32_2.g / (int) byte.MaxValue);
            byte b = (byte) ((int) this.untintedColors[index].b * (int) color32_2.b / (int) byte.MaxValue);
            byte a = (byte) ((int) this.untintedColors[index].a * (int) color32_2.a / (int) byte.MaxValue);
            if (this._fontInst.premultipliedAlpha)
            {
              r = (byte) ((int) r * (int) a / (int) byte.MaxValue);
              g = (byte) ((int) g * (int) a / (int) byte.MaxValue);
              b = (byte) ((int) b * (int) a / (int) byte.MaxValue);
            }
            this.colors[index] = new Color32(r, g, b, a);
          }
          this.mesh.colors32 = this.colors;
        }
      }
      this.updateFlags = tk2dTextMesh.UpdateFlags.UpdateNone;
    }

    public void MakePixelPerfect()
    {
      float num1 = 1f;
      tk2dCamera tk2dCamera = tk2dCamera.CameraForLayer(this.gameObject.layer);
      if ((UnityEngine.Object) tk2dCamera != (UnityEngine.Object) null)
      {
        if (this._fontInst.version < 1)
          Debug.LogError((object) "Need to rebuild font.");
        float distance = this.transform.position.z - tk2dCamera.transform.position.z;
        float num2 = this._fontInst.invOrthoSize * this._fontInst.halfTargetHeight;
        num1 = tk2dCamera.GetSizeAtDistance(distance) * num2;
      }
      else if ((bool) (UnityEngine.Object) Camera.main)
        num1 = (!Camera.main.orthographic ? tk2dPixelPerfectHelper.CalculateScaleForPerspectiveCamera(Camera.main.fieldOfView, this.transform.position.z - Camera.main.transform.position.z) : Camera.main.orthographicSize) * this._fontInst.invOrthoSize;
      this.scale = new Vector3(Mathf.Sign(this.scale.x) * num1, Mathf.Sign(this.scale.y) * num1, Mathf.Sign(this.scale.z) * num1);
    }

    public bool UsesSpriteCollection(tk2dSpriteCollectionData spriteCollection)
    {
      return !((UnityEngine.Object) this.data.font != (UnityEngine.Object) null) || !((UnityEngine.Object) this.data.font.spriteCollection != (UnityEngine.Object) null) || (UnityEngine.Object) this.data.font.spriteCollection == (UnityEngine.Object) spriteCollection;
    }

    private void UpdateMaterial()
    {
      if (!((UnityEngine.Object) this.GetComponent<Renderer>().sharedMaterial != (UnityEngine.Object) this._fontInst.materialInst))
        return;
      this.GetComponent<Renderer>().material = this._fontInst.materialInst;
    }

    public void ForceBuild()
    {
      if ((UnityEngine.Object) this.data.font != (UnityEngine.Object) null)
      {
        this._fontInst = this.data.font.inst;
        this.UpdateMaterial();
      }
      this.Init(true);
    }

    public enum WoobleStyle
    {
      SEQUENTIAL,
      SIMULTANEOUS,
      RANDOM_JITTER,
      RANDOM_SEQUENTIAL,
      SEQUENTIAL_RAINBOW,
    }

    internal class WoobleDefinition
    {
      public int startIndex = -1;
      public int endIndex = -1;
      public tk2dTextMesh.WoobleStyle style;
    }

    [Flags]
    private enum UpdateFlags
    {
      UpdateNone = 0,
      UpdateText = 1,
      UpdateColors = 2,
      UpdateBuffers = 4,
    }
  }

}
