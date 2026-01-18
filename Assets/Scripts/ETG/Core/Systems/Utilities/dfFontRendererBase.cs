using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

  public abstract class dfFontRendererBase : IDisposable
  {
    public dfFontBase Font { get; protected set; }

    public Vector2 MaxSize { get; set; }

    public float PixelRatio { get; set; }

    public float TextScale { get; set; }

    public int CharacterSpacing { get; set; }

    public Vector3 VectorOffset { get; set; }

    public Vector3 PerCharacterAccumulatedOffset { get; set; }

    public bool ProcessMarkup { get; set; }

    public bool WordWrap { get; set; }

    public bool MultiLine { get; set; }

    public bool OverrideMarkupColors { get; set; }

    public bool ColorizeSymbols { get; set; }

    public TextAlignment TextAlign { get; set; }

    public Color32 DefaultColor { get; set; }

    public Color32? BottomColor { get; set; }

    public float Opacity { get; set; }

    public bool Outline { get; set; }

    public int OutlineSize { get; set; }

    public Color32 OutlineColor { get; set; }

    public bool Shadow { get; set; }

    public Color32 ShadowColor { get; set; }

    public Vector2 ShadowOffset { get; set; }

    public int TabSize { get; set; }

    public List<int> TabStops { get; set; }

    public Vector2 RenderedSize { get; internal set; }

    public int LinesRendered { get; internal set; }

    public abstract void Release();

    public abstract float[] GetCharacterWidths(string text);

    public abstract Vector2 MeasureString(string text);

    public abstract void Render(string text, dfRenderData destination);

    protected virtual void Reset()
    {
      this.Font = (dfFontBase) null;
      this.PixelRatio = 0.0f;
      this.TextScale = 1f;
      this.CharacterSpacing = 0;
      this.VectorOffset = Vector3.zero;
      this.PerCharacterAccumulatedOffset = Vector3.zero;
      this.ProcessMarkup = false;
      this.WordWrap = false;
      this.MultiLine = false;
      this.OverrideMarkupColors = false;
      this.ColorizeSymbols = false;
      this.TextAlign = TextAlignment.Left;
      this.DefaultColor = (Color32) Color.white;
      this.BottomColor = new Color32?();
      this.Opacity = 1f;
      this.Outline = false;
      this.Shadow = false;
    }

    public void Dispose() => this.Release();
  }

