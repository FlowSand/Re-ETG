// Decompiled with JetBrains decompiler
// Type: tk2dTextMeshData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

[Serializable]
public class tk2dTextMeshData
  {
    public int version;
    public tk2dFontData font;
    public string text = string.Empty;
    public Color color = Color.white;
    public Color color2 = Color.white;
    public bool useGradient;
    public int textureGradient;
    public TextAnchor anchor = TextAnchor.LowerLeft;
    public int renderLayer;
    public Vector3 scale = Vector3.one;
    public bool kerning;
    public int maxChars = 16 /*0x10*/;
    public bool inlineStyling;
    public bool formatting;
    public int wordWrapWidth;
    public float spacing;
    public float lineSpacing;
  }

