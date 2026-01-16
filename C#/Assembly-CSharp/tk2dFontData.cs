// Decompiled with JetBrains decompiler
// Type: tk2dFontData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
[AddComponentMenu("2D Toolkit/Backend/tk2dFontData")]
public class tk2dFontData : MonoBehaviour
{
  public const int CURRENT_VERSION = 2;
  [HideInInspector]
  public int version;
  public float lineHeight;
  public tk2dFontChar[] chars;
  [SerializeField]
  private List<int> charDictKeys;
  [SerializeField]
  private List<tk2dFontChar> charDictValues;
  public string[] fontPlatforms;
  public string[] fontPlatformGUIDs;
  private tk2dFontData platformSpecificData;
  public bool hasPlatformData;
  public bool managedFont;
  public bool needMaterialInstance;
  public bool isPacked;
  public bool premultipliedAlpha;
  public tk2dSpriteCollectionData spriteCollection;
  public Dictionary<int, tk2dFontChar> charDict;
  public bool useDictionary;
  public tk2dFontKerning[] kerning;
  public float largestWidth;
  public Material material;
  [NonSerialized]
  public Material materialInst;
  public Texture2D gradientTexture;
  public bool textureGradients;
  public int gradientCount = 1;
  public Vector2 texelSize;
  [HideInInspector]
  public float invOrthoSize = 1f;
  [HideInInspector]
  public float halfTargetHeight = 1f;

  public tk2dFontData inst
  {
    get
    {
      if ((UnityEngine.Object) this.platformSpecificData == (UnityEngine.Object) null || (UnityEngine.Object) this.platformSpecificData.materialInst == (UnityEngine.Object) null)
      {
        if (this.hasPlatformData)
        {
          string currentPlatform = tk2dSystem.CurrentPlatform;
          string guid = string.Empty;
          for (int index = 0; index < this.fontPlatforms.Length; ++index)
          {
            if (this.fontPlatforms[index] == currentPlatform)
            {
              guid = this.fontPlatformGUIDs[index];
              break;
            }
          }
          if (guid.Length == 0)
            guid = this.fontPlatformGUIDs[0];
          this.platformSpecificData = tk2dSystem.LoadResourceByGUID<tk2dFontData>(guid);
        }
        else
          this.platformSpecificData = this;
        this.platformSpecificData.Init();
      }
      return this.platformSpecificData;
    }
  }

  private void Init()
  {
    if (this.needMaterialInstance)
    {
      if ((bool) (UnityEngine.Object) this.spriteCollection)
      {
        tk2dSpriteCollectionData inst = this.spriteCollection.inst;
        for (int index = 0; index < inst.materials.Length; ++index)
        {
          if ((UnityEngine.Object) inst.materials[index] == (UnityEngine.Object) this.material)
          {
            this.materialInst = inst.materialInsts[index];
            break;
          }
        }
        if (!((UnityEngine.Object) this.materialInst == (UnityEngine.Object) null))
          return;
        Debug.LogError((object) "Fatal error - font from sprite collection is has an invalid material");
      }
      else
      {
        this.materialInst = UnityEngine.Object.Instantiate<Material>(this.material);
        this.materialInst.hideFlags = HideFlags.DontSave;
      }
    }
    else
      this.materialInst = this.material;
  }

  public void ResetPlatformData()
  {
    if (this.hasPlatformData && (bool) (UnityEngine.Object) this.platformSpecificData)
      this.platformSpecificData = (tk2dFontData) null;
    this.materialInst = (Material) null;
  }

  private void OnDestroy()
  {
    if (!this.needMaterialInstance || !((UnityEngine.Object) this.spriteCollection == (UnityEngine.Object) null))
      return;
    UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.materialInst);
  }

  public void InitDictionary()
  {
    if (!this.useDictionary || this.charDict != null)
      return;
    this.charDict = new Dictionary<int, tk2dFontChar>(this.charDictKeys.Count);
    for (int index = 0; index < this.charDictKeys.Count; ++index)
      this.charDict[this.charDictKeys[index]] = this.charDictValues[index];
  }

  public void SetDictionary(Dictionary<int, tk2dFontChar> dict)
  {
    this.charDictKeys = new List<int>((IEnumerable<int>) dict.Keys);
    this.charDictValues = new List<tk2dFontChar>();
    for (int index = 0; index < this.charDictKeys.Count; ++index)
      this.charDictValues.Add(dict[this.charDictKeys[index]]);
  }
}
