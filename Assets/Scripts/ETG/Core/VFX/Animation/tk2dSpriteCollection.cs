using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

[AddComponentMenu("2D Toolkit/Backend/tk2dSpriteCollection")]
public class tk2dSpriteCollection : MonoBehaviour
  {
    public const int CURRENT_VERSION = 4;
    [SerializeField]
    private tk2dSpriteCollectionDefinition[] textures;
    [SerializeField]
    private Texture2D[] textureRefs;
    public tk2dSpriteSheetSource[] spriteSheets;
    public tk2dSpriteCollectionFont[] fonts;
    public tk2dSpriteCollectionDefault defaults;
    public List<tk2dSpriteCollectionPlatform> platforms = new List<tk2dSpriteCollectionPlatform>();
    public bool managedSpriteCollection;
    public bool loadable;
    public tk2dSpriteCollection.AtlasFormat atlasFormat;
    public int maxTextureSize = 2048 /*0x0800*/;
    public bool forceTextureSize;
    public int forcedTextureWidth = 2048 /*0x0800*/;
    public int forcedTextureHeight = 2048 /*0x0800*/;
    public tk2dSpriteCollection.TextureCompression textureCompression;
    public int atlasWidth;
    public int atlasHeight;
    public bool forceSquareAtlas;
    public float atlasWastage;
    public bool allowMultipleAtlases;
    public bool removeDuplicates = true;
    public tk2dSpriteCollectionDefinition[] textureParams;
    public tk2dSpriteCollectionData spriteCollection;
    public bool premultipliedAlpha;
    public bool shouldGenerateTilemapReflectionData;
    public Material[] altMaterials;
    public Material[] atlasMaterials;
    public Texture2D[] atlasTextures;
    public TextAsset[] atlasTextureFiles = new TextAsset[0];
    [SerializeField]
    private bool useTk2dCamera;
    [SerializeField]
    private int targetHeight = 640;
    [SerializeField]
    private float targetOrthoSize = 10f;
    public tk2dSpriteCollectionSize sizeDef = tk2dSpriteCollectionSize.Default();
    public float globalScale = 1f;
    public float globalTextureRescale = 1f;
    public List<tk2dSpriteCollection.AttachPointTestSprite> attachPointTestSprites = new List<tk2dSpriteCollection.AttachPointTestSprite>();
    [SerializeField]
    private bool pixelPerfectPointSampled;
    public FilterMode filterMode = FilterMode.Bilinear;
    public TextureWrapMode wrapMode = TextureWrapMode.Clamp;
    public bool userDefinedTextureSettings;
    public bool mipmapEnabled;
    public int anisoLevel = 1;
    public tk2dSpriteDefinition.PhysicsEngine physicsEngine;
    public float physicsDepth = 0.1f;
    public bool disableTrimming;
    public bool disableRotation;
    public tk2dSpriteCollection.NormalGenerationMode normalGenerationMode;
    public int padAmount = -1;
    public bool autoUpdate = true;
    public float editorDisplayScale = 1f;
    public int version;
    public string assetName = string.Empty;

    public Texture2D[] DoNotUse__TextureRefs
    {
      get => this.textureRefs;
      set => this.textureRefs = value;
    }

    public bool HasPlatformData => this.platforms.Count > 1;

    public void Upgrade()
    {
      if (this.version == 4)
        return;
      Debug.Log((object) $"SpriteCollection '{this.name}' - Upgraded from version {this.version.ToString()}");
      if (this.version == 0)
      {
        this.filterMode = !this.pixelPerfectPointSampled ? FilterMode.Bilinear : FilterMode.Point;
        this.userDefinedTextureSettings = true;
      }
      if (this.version < 3 && this.textureRefs != null && this.textureParams != null && this.textureRefs.Length == this.textureParams.Length)
      {
        for (int index = 0; index < this.textureRefs.Length; ++index)
          this.textureParams[index].texture = this.textureRefs[index];
        this.textureRefs = (Texture2D[]) null;
      }
      if (this.version < 4)
        this.sizeDef.CopyFromLegacy(this.useTk2dCamera, this.targetOrthoSize, (float) this.targetHeight);
      this.version = 4;
    }

    public enum NormalGenerationMode
    {
      None,
      NormalsOnly,
      NormalsAndTangents,
    }

    public enum TextureCompression
    {
      Uncompressed,
      Reduced16Bit,
      Compressed,
      Dithered16Bit_Alpha,
      Dithered16Bit_NoAlpha,
    }

    public enum AtlasFormat
    {
      UnityTexture,
      Png,
    }

    [Serializable]
    public class AttachPointTestSprite
    {
      public string attachPointName = string.Empty;
      public tk2dSpriteCollectionData spriteCollection;
      public int spriteId = -1;

      public bool CompareTo(tk2dSpriteCollection.AttachPointTestSprite src)
      {
        return src.attachPointName == this.attachPointName && (UnityEngine.Object) src.spriteCollection == (UnityEngine.Object) this.spriteCollection && src.spriteId == this.spriteId;
      }

      public void CopyFrom(tk2dSpriteCollection.AttachPointTestSprite src)
      {
        this.attachPointName = src.attachPointName;
        this.spriteCollection = src.spriteCollection;
        this.spriteId = src.spriteId;
      }
    }
  }

