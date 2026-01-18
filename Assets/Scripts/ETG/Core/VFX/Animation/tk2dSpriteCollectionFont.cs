using System;
using UnityEngine;

#nullable disable

[Serializable]
public class tk2dSpriteCollectionFont
  {
    public bool active;
    public TextAsset bmFont;
    public Texture2D texture;
    public bool dupeCaps;
    public bool flipTextureY;
    public int charPadX;
    public tk2dFontData data;
    public tk2dFont editorData;
    public int materialId;
    public bool useGradient;
    public Texture2D gradientTexture;
    public int gradientCount = 1;

    public void CopyFrom(tk2dSpriteCollectionFont src)
    {
      this.active = src.active;
      this.bmFont = src.bmFont;
      this.texture = src.texture;
      this.dupeCaps = src.dupeCaps;
      this.flipTextureY = src.flipTextureY;
      this.charPadX = src.charPadX;
      this.data = src.data;
      this.editorData = src.editorData;
      this.materialId = src.materialId;
      this.gradientCount = src.gradientCount;
      this.gradientTexture = src.gradientTexture;
      this.useGradient = src.useGradient;
    }

    public string Name
    {
      get
      {
        if ((UnityEngine.Object) this.bmFont == (UnityEngine.Object) null || (UnityEngine.Object) this.texture == (UnityEngine.Object) null)
          return "Empty";
        return (UnityEngine.Object) this.data == (UnityEngine.Object) null ? this.bmFont.name + " (Inactive)" : this.bmFont.name;
      }
    }

    public bool InUse
    {
      get
      {
        return this.active && (UnityEngine.Object) this.bmFont != (UnityEngine.Object) null && (UnityEngine.Object) this.texture != (UnityEngine.Object) null && (UnityEngine.Object) this.data != (UnityEngine.Object) null && (UnityEngine.Object) this.editorData != (UnityEngine.Object) null;
      }
    }
  }

