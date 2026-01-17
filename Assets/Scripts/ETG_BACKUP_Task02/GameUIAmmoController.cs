// Decompiled with JetBrains decompiler
// Type: GameUIAmmoController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Serialization;

#nullable disable
public class GameUIAmmoController : BraveBehaviour
{
  [SerializeField]
  [BetterList]
  public GameUIAmmoType[] ammoTypes;
  [FormerlySerializedAs("GunAmmoBottomCapSprite")]
  public dfSprite InitialBottomCapSprite;
  [FormerlySerializedAs("GunAmmoTopCapSprite")]
  public dfSprite InitialTopCapSprite;
  public dfSprite GunBoxSprite;
  public dfSprite GunQuickSwitchIcon;
  public GameObject ExtraGunCardPrefab;
  public List<dfControl> AdditionalGunBoxSprites = new List<dfControl>();
  public dfLabel GunClipCountLabel;
  public dfLabel GunAmmoCountLabel;
  public dfSprite GunCooldownForegroundSprite;
  public dfSprite GunCooldownFillSprite;
  public dfSpriteAnimation AmmoBurstVFX;
  [NonSerialized]
  public bool temporarilyPreventVisible;
  [NonSerialized]
  public bool forceInvisiblePermanent;
  private List<dfTiledSprite> fgSpritesForModules = new List<dfTiledSprite>();
  private List<dfTiledSprite> bgSpritesForModules = new List<dfTiledSprite>();
  private List<List<dfTiledSprite>> addlFgSpritesForModules = new List<List<dfTiledSprite>>();
  private List<List<dfTiledSprite>> addlBgSpritesForModules = new List<List<dfTiledSprite>>();
  private List<dfSprite> topCapsForModules = new List<dfSprite>();
  private List<dfSprite> bottomCapsForModules = new List<dfSprite>();
  private List<GameUIAmmoType.AmmoType> cachedAmmoTypesForModules = new List<GameUIAmmoType.AmmoType>();
  private List<string> cachedCustomAmmoTypesForModules = new List<string>();
  private dfPanel m_panel;
  private List<GameUIAmmoType> m_additionalAmmoTypeDefinitions = new List<GameUIAmmoType>();
  public tk2dClippedSprite[] gunSprites;
  public bool IsLeftAligned;
  private Gun m_cachedGun;
  private List<int> m_cachedModuleShotsRemaining = new List<int>();
  private int m_cachedMaxAmmo;
  private int m_cachedTotalAmmo;
  private int m_cachedNumberModules = 1;
  private bool m_cachedUndertaleness;
  private tk2dSprite[][] outlineSprites;
  private bool m_initialized;
  private Material m_ClippedMaterial;
  private Material m_ClippedZWriteOffMaterial;
  private float UI_OUTLINE_DEPTH = 1f;
  private static int NumberOfAdditionalGunCards = 3;
  private List<dfSprite> m_additionalRegisteredSprites = new List<dfSprite>();
  private tk2dSpriteDefinition m_cachedGunSpriteDefinition;
  private bool m_isCurrentlyFlipping;
  private bool m_currentFlipReverse;
  private float m_currentGunSpriteXOffset;
  private float m_currentGunSpriteZOffset;
  private bool m_deferCurrentGunSwap;
  private bool m_cardFlippedQueued;
  private const float FLIP_TIME = 0.15f;
  private tk2dSprite m_extantNoAmmoIcon;
  public bool SuppressNextGunFlip;
  private const int NUM_PIXELS_PER_MODULE = -10;

  private void Initialize()
  {
    this.m_panel = this.GetComponent<dfPanel>();
    this.outlineSprites = new tk2dSprite[this.gunSprites.Length][];
    for (int index1 = 0; index1 < this.gunSprites.Length; ++index1)
    {
      tk2dClippedSprite gunSprite = this.gunSprites[index1];
      SpriteOutlineManager.AddOutlineToSprite((tk2dBaseSprite) gunSprite, Color.white, 2f);
      this.outlineSprites[index1] = SpriteOutlineManager.GetOutlineSprites((tk2dBaseSprite) gunSprite);
      for (int index2 = 0; index2 < this.outlineSprites[index1].Length; ++index2)
      {
        if (this.outlineSprites[index1].Length > 1)
        {
          float num1 = index2 != 1 ? 0.0f : 1f / 16f;
          float x = index2 != 3 ? num1 : -1f / 16f;
          float num2 = index2 != 0 ? 0.0f : 1f / 16f;
          float y = index2 != 2 ? num2 : -1f / 16f;
          this.outlineSprites[index1][index2].transform.localPosition = (new Vector3(x, y, 0.0f) * Pixelator.Instance.CurrentTileScale * 16f * GameUIRoot.Instance.PixelsToUnits()).WithZ(this.UI_OUTLINE_DEPTH);
        }
        this.outlineSprites[index1][index2].gameObject.layer = gunSprite.gameObject.layer;
      }
    }
    this.m_ClippedMaterial = new Material(ShaderCache.Acquire("Daikon Forge/Clipped UI Shader"));
    this.m_ClippedZWriteOffMaterial = new Material(ShaderCache.Acquire("Daikon Forge/Clipped UI Shader ZWriteOff"));
    this.topCapsForModules.Add(this.InitialTopCapSprite);
    this.bottomCapsForModules.Add(this.InitialBottomCapSprite);
    this.m_panel.SendToBack();
    this.m_initialized = true;
  }

  public dfSprite RegisterNewAdditionalSprite(string spriteName)
  {
    dfSprite component = UnityEngine.Object.Instantiate<GameObject>(this.InitialBottomCapSprite.gameObject).GetComponent<dfSprite>();
    this.InitialBottomCapSprite.Parent.AddControl((dfControl) component);
    component.SpriteName = spriteName;
    component.Size = component.SpriteInfo.sizeInPixels * Pixelator.Instance.CurrentTileScale;
    this.m_additionalRegisteredSprites.Add(component);
    this.UpdateAdditionalSprites();
    return component;
  }

  public void DeregisterAdditionalSprite(dfSprite sprite)
  {
    if (!this.m_additionalRegisteredSprites.Contains(sprite))
      return;
    this.m_additionalRegisteredSprites.Remove(sprite);
    UnityEngine.Object.Destroy((UnityEngine.Object) sprite.gameObject);
    this.UpdateAdditionalSprites();
  }

  private void UpdateAdditionalSprites()
  {
    float currentTileScale = Pixelator.Instance.CurrentTileScale;
    Vector3 position = this.GunAmmoCountLabel.Position;
    Vector3 vector3_1 = Vector3.zero;
    Vector3 vector3_2 = !this.IsLeftAligned ? position + new Vector3(this.GunAmmoCountLabel.Size.x, 4f * currentTileScale, 0.0f) : position + new Vector3(0.0f, 4f * currentTileScale, 0.0f);
    int num = !this.IsLeftAligned ? -1 : 1;
    for (int index = 0; index < this.m_additionalRegisteredSprites.Count; ++index)
    {
      Vector2 size = this.m_additionalRegisteredSprites[index].Size;
      if (this.IsLeftAligned)
      {
        this.m_additionalRegisteredSprites[index].Position = vector3_2 + (float) num * vector3_1;
        vector3_1 += new Vector3(size.x + currentTileScale, 0.0f, 0.0f);
      }
      else
      {
        Vector3 vector3_3 = vector3_1 + new Vector3(size.x, 0.0f, 0.0f);
        this.m_additionalRegisteredSprites[index].Position = vector3_2 + (float) num * vector3_3;
        vector3_1 = vector3_3 + new Vector3(currentTileScale, 0.0f, 0.0f);
      }
    }
  }

  private void RepositionOutlines(dfControl arg1, Vector3 arg2, Vector3 arg3)
  {
    if (this.outlineSprites == null)
      return;
    for (int index1 = 0; index1 < this.gunSprites.Length; ++index1)
    {
      for (int index2 = 0; index2 < this.outlineSprites.Length; ++index2)
        this.outlineSprites[index1][index2].gameObject.layer = this.gunSprites[index1].gameObject.layer;
    }
  }

  public void DimGunSprite()
  {
    for (int index = 0; index < this.gunSprites.Length; ++index)
      this.gunSprites[index].gameObject.SetActive(false);
    if (!((UnityEngine.Object) this.m_extantNoAmmoIcon != (UnityEngine.Object) null))
      return;
    this.m_extantNoAmmoIcon.gameObject.SetActive(false);
  }

  public void UndimGunSprite()
  {
    for (int index = 0; index < this.gunSprites.Length; ++index)
      this.gunSprites[index].gameObject.SetActive(true);
    if (!((UnityEngine.Object) this.m_extantNoAmmoIcon != (UnityEngine.Object) null))
      return;
    this.m_extantNoAmmoIcon.gameObject.SetActive(true);
  }

  private float ActualSign(float f)
  {
    if ((double) f < 0.0)
      return -1f;
    return (double) f > 0.0 ? 1f : 0.0f;
  }

  public dfSprite DefaultAmmoFGSprite
  {
    get
    {
      return this.fgSpritesForModules == null || this.fgSpritesForModules.Count == 0 ? (dfSprite) null : (dfSprite) this.fgSpritesForModules[0];
    }
  }

  public void UpdateScale()
  {
    float currentTileScale = Pixelator.Instance.CurrentTileScale;
    this.GunBoxSprite.Size = this.GunBoxSprite.SpriteInfo.sizeInPixels * currentTileScale;
    Vector2 vector2 = new Vector2(currentTileScale, currentTileScale);
    for (int index = 0; index < this.fgSpritesForModules.Count; ++index)
    {
      if ((bool) (UnityEngine.Object) this.fgSpritesForModules[index])
      {
        this.fgSpritesForModules[index].TileScale = vector2;
        this.bgSpritesForModules[index].TileScale = vector2;
      }
    }
    for (int index1 = 0; index1 < this.addlFgSpritesForModules.Count; ++index1)
    {
      List<dfTiledSprite> spritesForModule1 = this.addlFgSpritesForModules[index1];
      List<dfTiledSprite> spritesForModule2 = this.addlBgSpritesForModules[index1];
      for (int index2 = 0; index2 < spritesForModule1.Count; ++index2)
      {
        spritesForModule1[index2].TileScale = vector2;
        spritesForModule2[index2].TileScale = vector2;
      }
    }
    for (int index = 0; index < this.topCapsForModules.Count; ++index)
    {
      this.topCapsForModules[index].Size = this.topCapsForModules[index].SpriteInfo.sizeInPixels * currentTileScale;
      this.bottomCapsForModules[index].Size = this.bottomCapsForModules[index].SpriteInfo.sizeInPixels * currentTileScale;
    }
    if ((UnityEngine.Object) this.GunClipCountLabel != (UnityEngine.Object) null)
      this.GunClipCountLabel.TextScale = currentTileScale;
    if (!((UnityEngine.Object) this.GunAmmoCountLabel != (UnityEngine.Object) null))
      return;
    this.GunAmmoCountLabel.TextScale = currentTileScale;
  }

  public void SetAmmoCountLabelColor(Color targetcolor)
  {
    this.GunAmmoCountLabel.Color = (Color32) targetcolor;
    this.GunAmmoCountLabel.BottomColor = (Color32) targetcolor;
  }

  public void ToggleRenderers(bool value)
  {
    if (!this.m_initialized)
      return;
    if ((UnityEngine.Object) this.GunBoxSprite != (UnityEngine.Object) null && (UnityEngine.Object) this.GunBoxSprite.Parent != (UnityEngine.Object) null)
      this.GunBoxSprite.IsVisible = value;
    if ((UnityEngine.Object) this.GunBoxSprite != (UnityEngine.Object) null)
      this.GunBoxSprite.IsVisible = value;
    if ((UnityEngine.Object) this.GunQuickSwitchIcon != (UnityEngine.Object) null && !value)
      this.GunQuickSwitchIcon.IsVisible = value;
    for (int index = 0; index < this.fgSpritesForModules.Count; ++index)
    {
      if ((bool) (UnityEngine.Object) this.fgSpritesForModules[index])
      {
        this.fgSpritesForModules[index].IsVisible = value;
        this.bgSpritesForModules[index].IsVisible = value;
      }
    }
    for (int index1 = 0; index1 < this.addlFgSpritesForModules.Count; ++index1)
    {
      List<dfTiledSprite> spritesForModule1 = this.addlFgSpritesForModules[index1];
      List<dfTiledSprite> spritesForModule2 = this.addlBgSpritesForModules[index1];
      for (int index2 = 0; index2 < spritesForModule1.Count; ++index2)
      {
        spritesForModule1[index2].IsVisible = value;
        spritesForModule2[index2].IsVisible = value;
      }
    }
    if ((UnityEngine.Object) this.m_extantNoAmmoIcon != (UnityEngine.Object) null)
      this.m_extantNoAmmoIcon.renderer.enabled = value;
    if ((UnityEngine.Object) this.GunAmmoCountLabel != (UnityEngine.Object) null)
      this.GunAmmoCountLabel.IsVisible = value;
    for (int index = 0; index < this.topCapsForModules.Count; ++index)
    {
      this.topCapsForModules[index].IsVisible = value;
      this.bottomCapsForModules[index].IsVisible = value;
    }
    if ((UnityEngine.Object) this.GunClipCountLabel != (UnityEngine.Object) null)
      this.GunClipCountLabel.IsVisible = value;
    for (int index3 = 0; index3 < this.gunSprites.Length; ++index3)
    {
      tk2dClippedSprite gunSprite = this.gunSprites[index3];
      if (gunSprite.renderer.enabled != value)
      {
        gunSprite.renderer.enabled = value;
        this.outlineSprites[index3] = SpriteOutlineManager.GetOutlineSprites((tk2dBaseSprite) gunSprite);
        for (int index4 = 0; index4 < this.outlineSprites[index3].Length; ++index4)
        {
          if ((bool) (UnityEngine.Object) this.outlineSprites[index3][index4] && (bool) (UnityEngine.Object) this.outlineSprites[index3][index4].renderer)
            this.outlineSprites[index3][index4].renderer.enabled = value;
        }
      }
    }
  }

  public bool IsFlipping => this.m_isCurrentlyFlipping;

  private void DoGunCardFlip(Gun newGun, int change)
  {
    if (this.AdditionalGunBoxSprites.Count == 0 || this.AdditionalGunBoxSprites.Count > 10)
      return;
    if (!this.m_isCurrentlyFlipping && GameUIRoot.Instance.GunventoryFolded)
    {
      if (change > 0)
        this.StartCoroutine(this.HandleGunCardFlipReverse(newGun));
      else
        this.StartCoroutine(this.HandleGunCardFlip(newGun));
    }
    else if (this.m_cardFlippedQueued)
      ;
  }

  private Transform GetChildestOfTransforms(Transform parent)
  {
    Transform source = parent;
    while ((UnityEngine.Object) source != (UnityEngine.Object) null && source.childCount > 0)
      source = this.GetFirstValidChild(source);
    return source;
  }

  [DebuggerHidden]
  private IEnumerator WaitForCurrentGunFlipToEnd(Gun newGun, int change)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new GameUIAmmoController.\u003CWaitForCurrentGunFlipToEnd\u003Ec__Iterator0()
    {
      change = change,
      newGun = newGun,
      \u0024this = this
    };
  }

  [DebuggerHidden]
  private IEnumerator HandleGunCardFlipReverse(Gun newGun)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new GameUIAmmoController.\u003CHandleGunCardFlipReverse\u003Ec__Iterator1()
    {
      newGun = newGun,
      \u0024this = this
    };
  }

  private Transform GetFirstValidChild(Transform source)
  {
    for (int index = 0; index < source.childCount; ++index)
    {
      if ((bool) (UnityEngine.Object) source.GetChild(index) && (!(bool) (UnityEngine.Object) this.GunQuickSwitchIcon || !((UnityEngine.Object) source.GetChild(index) == (UnityEngine.Object) this.GunQuickSwitchIcon.transform)))
        return source.GetChild(index);
    }
    return (Transform) null;
  }

  public void UpdateNoAmmoIcon()
  {
    if (!((UnityEngine.Object) this.m_extantNoAmmoIcon != (UnityEngine.Object) null))
      return;
    this.m_extantNoAmmoIcon.scale = this.gunSprites[0].scale;
    this.m_extantNoAmmoIcon.transform.position = this.GunBoxSprite.GetCenter().Quantize(1f / 16f * this.m_extantNoAmmoIcon.scale.x).WithZ(this.m_panel.transform.position.z - 3f);
  }

  public void AddNoAmmoIcon()
  {
    if (!((UnityEngine.Object) this.m_extantNoAmmoIcon == (UnityEngine.Object) null))
      return;
    this.gunSprites[0].renderer.material.SetFloat("_Saturation", 0.0f);
    SpriteOutlineManager.ToggleOutlineRenderers((tk2dBaseSprite) this.gunSprites[0], false);
    tk2dSprite component = ((GameObject) UnityEngine.Object.Instantiate(BraveResources.Load("Global Prefabs/NoAmmoIcon"))).GetComponent<tk2dSprite>();
    component.transform.parent = this.m_panel.transform;
    component.HeightOffGround = 5f;
    component.OverrideMaterialMode = tk2dBaseSprite.SpriteMaterialOverrideMode.OVERRIDE_MATERIAL_SIMPLE;
    component.scale = this.gunSprites[0].scale;
    component.transform.position = this.GunBoxSprite.GetCenter().Quantize(1f / 16f * component.scale.x);
    this.m_extantNoAmmoIcon = component;
  }

  public void ClearNoAmmoIcon()
  {
    if (!((UnityEngine.Object) this.m_extantNoAmmoIcon != (UnityEngine.Object) null))
      return;
    if (this.m_isCurrentlyFlipping && this.m_currentFlipReverse)
    {
      this.m_extantNoAmmoIcon.renderer.enabled = false;
    }
    else
    {
      SpriteOutlineManager.ToggleOutlineRenderers((tk2dBaseSprite) this.gunSprites[0], true);
      this.gunSprites[0].renderer.material.SetFloat("_Saturation", 1f);
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_extantNoAmmoIcon.gameObject);
      this.m_extantNoAmmoIcon = (tk2dSprite) null;
    }
  }

  [DebuggerHidden]
  private IEnumerator HandleGunCardFlip(Gun newGun)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new GameUIAmmoController.\u003CHandleGunCardFlip\u003Ec__Iterator2()
    {
      newGun = newGun,
      \u0024this = this
    };
  }

  private void PostFlipReset(
    Transform newChild,
    Transform gbTransform,
    GameObject placeholderCardObject,
    tk2dClippedSprite[] oldGunSprites,
    Gun newGun)
  {
    for (int index = 0; index < this.AdditionalGunBoxSprites.Count; ++index)
      (this.AdditionalGunBoxSprites[index] as dfTextureSprite).Material = this.m_ClippedZWriteOffMaterial;
    if ((bool) (UnityEngine.Object) newChild)
    {
      newChild.parent = gbTransform;
      this.GunBoxSprite.AddControl(newChild.GetComponent<dfControl>());
      newChild.GetComponent<dfControl>().RelativePosition = new Vector3((float) this.AdditionalBoxOffsetPx * Pixelator.Instance.CurrentTileScale, 0.0f, 0.0f);
    }
    UnityEngine.Object.Destroy((UnityEngine.Object) placeholderCardObject);
    for (int index = 0; index < oldGunSprites.Length; ++index)
      UnityEngine.Object.Destroy((UnityEngine.Object) oldGunSprites[index].gameObject);
    this.m_currentGunSpriteXOffset = 0.0f;
    this.m_currentGunSpriteZOffset = 0.0f;
    this.GunBoxSprite.enabled = true;
    this.UpdateGunSprite(newGun, 0);
  }

  private void UpdateGunSprite(Gun newGun, int change, Gun secondaryGun = null)
  {
    if ((UnityEngine.Object) newGun != (UnityEngine.Object) this.m_cachedGun && !this.SuppressNextGunFlip && (UnityEngine.Object) this.m_cachedGun != (UnityEngine.Object) null)
      this.DoGunCardFlip(newGun, change);
    this.SuppressNextGunFlip = false;
    if (newGun.CurrentAmmo == 0)
    {
      this.AddNoAmmoIcon();
      this.UpdateNoAmmoIcon();
    }
    else
      this.ClearNoAmmoIcon();
    for (int currentIndex = 0; currentIndex < this.gunSprites.Length; ++currentIndex)
    {
      Gun gun = currentIndex <= 0 || !(bool) (UnityEngine.Object) secondaryGun ? newGun : secondaryGun;
      tk2dBaseSprite sprite = gun.GetSprite();
      int num1 = sprite.spriteId;
      tk2dSpriteCollectionData collection = sprite.Collection;
      if (gun.OnlyUsesIdleInWeaponBox)
        num1 = gun.DefaultSpriteID;
      else if ((bool) (UnityEngine.Object) gun.weaponPanelSpriteOverride)
        num1 = gun.weaponPanelSpriteOverride.GetMatch(num1);
      tk2dClippedSprite gunSprite = this.gunSprites[currentIndex];
      gunSprite.scale = GameUIUtility.GetCurrentTK2D_DFScale(this.m_panel.GetManager()) * Vector3.one;
      for (int index = 0; index < this.outlineSprites[currentIndex].Length; ++index)
      {
        if (this.outlineSprites[currentIndex].Length > 1)
        {
          float num2 = index != 1 ? 0.0f : 1f / 16f;
          float x = index != 3 ? num2 : -1f / 16f;
          float num3 = index != 0 ? 0.0f : 1f / 16f;
          float y = index != 2 ? num3 : -1f / 16f;
          this.outlineSprites[currentIndex][index].transform.localPosition = (new Vector3(x, y, 0.0f) * Pixelator.Instance.CurrentTileScale * 16f * GameUIRoot.Instance.PixelsToUnits()).WithZ(this.UI_OUTLINE_DEPTH);
        }
        this.outlineSprites[currentIndex][index].scale = gunSprite.scale;
      }
      if (!this.m_deferCurrentGunSwap)
      {
        if (!gunSprite.renderer.enabled)
          this.ToggleRenderers(true);
        if (gunSprite.spriteId != num1 || (UnityEngine.Object) gunSprite.Collection != (UnityEngine.Object) collection)
        {
          gunSprite.SetSprite(collection, num1);
          if (gunSprite.OverrideMaterialMode != tk2dBaseSprite.SpriteMaterialOverrideMode.OVERRIDE_MATERIAL_SIMPLE)
          {
            gunSprite.OverrideMaterialMode = tk2dBaseSprite.SpriteMaterialOverrideMode.OVERRIDE_MATERIAL_SIMPLE;
            if ((bool) (UnityEngine.Object) gunSprite.renderer && gunSprite.renderer.material.shader.name.Contains("Gonner"))
              gunSprite.renderer.material.shader = Shader.Find("tk2d/CutoutVertexColorTilted");
          }
          gunSprite.renderer.material.EnableKeyword("SATURATION_ON");
          for (int index = 0; index < this.outlineSprites[currentIndex].Length; ++index)
          {
            this.outlineSprites[currentIndex][index].SetSprite(collection, num1);
            SpriteOutlineManager.ForceUpdateOutlineMaterial((tk2dBaseSprite) this.outlineSprites[currentIndex][index], sprite);
          }
        }
      }
      Vector3 center = this.GunBoxSprite.GetCenter();
      if ((bool) (UnityEngine.Object) secondaryGun)
        center += this.CalculateLocalOffsetForGunInDualWieldMode(newGun, secondaryGun, currentIndex);
      gunSprite.transform.position = center + this.GetOffsetVectorForGun(currentIndex <= 0 || !(bool) (UnityEngine.Object) secondaryGun ? newGun : secondaryGun, this.m_isCurrentlyFlipping);
      gunSprite.transform.position = gunSprite.transform.position.Quantize(this.GunBoxSprite.PixelsToUnits() * 3f);
    }
    if (!newGun.UsesRechargeLikeActiveItem && !newGun.IsUndertaleGun)
    {
      this.GunCooldownFillSprite.IsVisible = false;
      this.GunCooldownForegroundSprite.IsVisible = false;
    }
    else
    {
      this.GunCooldownForegroundSprite.RelativePosition = this.GunBoxSprite.RelativePosition;
      this.GunCooldownFillSprite.RelativePosition = this.GunBoxSprite.RelativePosition + new Vector3(123f, 3f, 0.0f);
      this.GunCooldownFillSprite.ZOrder = this.GunBoxSprite.ZOrder + 1;
      this.GunCooldownForegroundSprite.ZOrder = this.GunCooldownFillSprite.ZOrder + 1;
      this.GunCooldownFillSprite.IsVisible = true;
      this.GunCooldownForegroundSprite.IsVisible = true;
    }
    if (!newGun.UsesRechargeLikeActiveItem && !newGun.IsUndertaleGun)
      return;
    this.GunCooldownFillSprite.FillAmount = newGun.CurrentActiveItemChargeAmount;
  }

  private Vector3 CalculateLocalOffsetForGunInDualWieldMode(
    Gun primary,
    Gun secondary,
    int currentIndex)
  {
    float units = this.GunBoxSprite.PixelsToUnits();
    Vector2 vector2 = this.GunBoxSprite.Size * 0.5f * units;
    Bounds bounds1 = primary.GetSprite().GetBounds();
    Bounds bounds2 = secondary.GetSprite().GetBounds();
    Bounds bounds3 = currentIndex != 0 ? bounds2 : bounds1;
    Vector3 vector3 = (Vector3) (vector2 + new Vector2(-8f * units, -8f * units) - Vector2.Scale(bounds3.extents.XY(), this.gunSprites[0].scale.XY()));
    return currentIndex == 0 ? new Vector3(vector3.x, -vector3.y, 0.0f) : new Vector3(-vector3.x, vector3.y, 0.0f);
  }

  public Vector2 GetOffsetVectorForSpecificSprite(tk2dBaseSprite targetSprite, bool isFlippingGun)
  {
    tk2dSpriteDefinition currentSpriteDef = targetSprite.GetCurrentSpriteDef();
    Vector3 forSpecificSprite = Vector3.Scale(-currentSpriteDef.GetBounds().min + -currentSpriteDef.GetBounds().extents, this.gunSprites[0].scale);
    if (isFlippingGun)
      forSpecificSprite += new Vector3(this.m_currentGunSpriteXOffset, 0.0f, this.m_currentGunSpriteZOffset);
    return (Vector2) forSpecificSprite;
  }

  public Vector3 GetOffsetVectorForGun(Gun newGun, bool isFlippingGun)
  {
    tk2dSpriteDefinition spriteDefinition = this.m_cachedGunSpriteDefinition == null || isFlippingGun ? newGun.GetSprite().Collection.spriteDefinitions[newGun.DefaultSpriteID] : this.m_cachedGunSpriteDefinition;
    Vector3 offsetVectorForGun = Vector3.Scale(-spriteDefinition.GetBounds().min + -spriteDefinition.GetBounds().extents, this.gunSprites[0].scale);
    if (isFlippingGun)
      offsetVectorForGun += new Vector3(this.m_currentGunSpriteXOffset, 0.0f, this.m_currentGunSpriteZOffset);
    return offsetVectorForGun;
  }

  private int GUN_BOX_EXTRA_PX_OFFSET => this.IsLeftAligned ? -9 : 9;

  private int AdditionalBoxOffsetPx => this.IsLeftAligned ? -2 : 2;

  protected void RebuildExtraGunCards(GunInventory guns)
  {
    UnityEngine.Debug.Log((object) "REBUILDING EXTRA GUN CARDS");
    float units = this.m_panel.PixelsToUnits();
    for (int index = 0; index < this.AdditionalGunBoxSprites.Count; ++index)
    {
      this.AdditionalGunBoxSprites[index].transform.parent = (Transform) null;
      UnityEngine.Object.Destroy((UnityEngine.Object) this.AdditionalGunBoxSprites[index].gameObject);
    }
    this.AdditionalGunBoxSprites.Clear();
    dfControl dfControl = (dfControl) this.GunBoxSprite;
    Transform transform = this.GunBoxSprite.transform;
    int num1 = Mathf.Min(guns.AllGuns.Count - 1, GameUIAmmoController.NumberOfAdditionalGunCards);
    for (int index = 0; index < num1; ++index)
    {
      GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.ExtraGunCardPrefab);
      gameObject.transform.parent = transform;
      dfControl component = gameObject.GetComponent<dfControl>();
      dfControl.AddControl(component);
      component.RelativePosition = new Vector3((float) this.AdditionalBoxOffsetPx * Pixelator.Instance.CurrentTileScale, 0.0f, 0.0f);
      dfControl = component;
      transform = gameObject.transform;
      this.AdditionalGunBoxSprites.Add(component);
    }
    float num2 = (float) ((!this.IsLeftAligned ? 1.0 : -1.0) * (double) Pixelator.Instance.CurrentTileScale * (double) (this.m_cachedNumberModules - 1) * -10.0) * units;
    float num3 = (float) (-this.AdditionalBoxOffsetPx * this.AdditionalGunBoxSprites.Count + -this.GUN_BOX_EXTRA_PX_OFFSET) * Pixelator.Instance.CurrentTileScale * units;
    if (this.IsLeftAligned)
      this.GunBoxSprite.transform.position = this.GunBoxSprite.transform.position.WithX(this.m_panel.transform.position.x - this.m_panel.Width * units + num3 + num2);
    else
      this.GunBoxSprite.transform.position = this.m_panel.transform.position + new Vector3(num3 + num2, 0.0f, 0.0f);
    this.GunBoxSprite.Invalidate();
  }

  private GameUIAmmoType GetUIAmmoType(GameUIAmmoType.AmmoType sourceType, string customType)
  {
    GameUIAmmoType[] ammoTypes = this.ammoTypes;
    if (this.IsLeftAligned)
    {
      for (int index = 0; index < GameUIRoot.Instance.ammoControllers.Count; ++index)
      {
        if (!GameUIRoot.Instance.ammoControllers[index].IsLeftAligned)
        {
          ammoTypes = GameUIRoot.Instance.ammoControllers[index].ammoTypes;
          break;
        }
      }
    }
    for (int index = 0; index < ammoTypes.Length; ++index)
    {
      if (sourceType == GameUIAmmoType.AmmoType.CUSTOM)
      {
        if (ammoTypes[index].ammoType == GameUIAmmoType.AmmoType.CUSTOM && ammoTypes[index].customAmmoType == customType)
          return ammoTypes[index];
      }
      else if (ammoTypes[index].ammoType == sourceType)
        return ammoTypes[index];
    }
    return ammoTypes[0];
  }

  public void TriggerUIDisabled()
  {
    if (!GameUIRoot.Instance.ForceHideGunPanel)
      return;
    this.ToggleRenderers(false);
  }

  private void CleanupLists(int numberModules)
  {
    for (int index = this.fgSpritesForModules.Count - 1; index >= numberModules; --index)
    {
      if ((bool) (UnityEngine.Object) this.fgSpritesForModules[index])
      {
        UnityEngine.Object.Destroy((UnityEngine.Object) this.fgSpritesForModules[index].gameObject);
        this.fgSpritesForModules.RemoveAt(index);
      }
      if ((bool) (UnityEngine.Object) this.bgSpritesForModules[index])
      {
        UnityEngine.Object.Destroy((UnityEngine.Object) this.bgSpritesForModules[index].gameObject);
        this.bgSpritesForModules.RemoveAt(index);
      }
      this.cachedAmmoTypesForModules.RemoveAt(index);
      this.cachedCustomAmmoTypesForModules.RemoveAt(index);
    }
    for (int index1 = this.addlFgSpritesForModules.Count - 1; index1 >= numberModules; --index1)
    {
      if (this.addlFgSpritesForModules[index1] != null)
      {
        for (int index2 = this.addlFgSpritesForModules[index1].Count - 1; index2 >= 0; --index2)
        {
          UnityEngine.Object.Destroy((UnityEngine.Object) this.addlFgSpritesForModules[index1][index2].gameObject);
          UnityEngine.Object.Destroy((UnityEngine.Object) this.addlBgSpritesForModules[index1][index2].gameObject);
        }
        this.addlFgSpritesForModules.RemoveAt(index1);
        this.addlBgSpritesForModules.RemoveAt(index1);
      }
    }
    for (int index = this.topCapsForModules.Count - 1; index >= numberModules; --index)
    {
      if ((bool) (UnityEngine.Object) this.topCapsForModules[index])
      {
        UnityEngine.Object.Destroy((UnityEngine.Object) this.topCapsForModules[index].gameObject);
        this.topCapsForModules.RemoveAt(index);
      }
      if ((bool) (UnityEngine.Object) this.bottomCapsForModules[index])
      {
        UnityEngine.Object.Destroy((UnityEngine.Object) this.bottomCapsForModules[index].gameObject);
        this.bottomCapsForModules.RemoveAt(index);
      }
    }
    for (int index = this.m_cachedModuleShotsRemaining.Count - 1; index >= numberModules; --index)
      this.m_cachedModuleShotsRemaining.RemoveAt(index);
  }

  private void EnsureInitialization(int usedModuleIndex)
  {
    if (usedModuleIndex >= this.fgSpritesForModules.Count)
      this.fgSpritesForModules.Add((dfTiledSprite) null);
    if (usedModuleIndex >= this.bgSpritesForModules.Count)
      this.bgSpritesForModules.Add((dfTiledSprite) null);
    if (usedModuleIndex >= this.addlFgSpritesForModules.Count)
      this.addlFgSpritesForModules.Add(new List<dfTiledSprite>());
    if (usedModuleIndex >= this.addlBgSpritesForModules.Count)
      this.addlBgSpritesForModules.Add(new List<dfTiledSprite>());
    if (usedModuleIndex >= this.cachedAmmoTypesForModules.Count)
      this.cachedAmmoTypesForModules.Add(GameUIAmmoType.AmmoType.SMALL_BULLET);
    if (usedModuleIndex >= this.cachedCustomAmmoTypesForModules.Count)
      this.cachedCustomAmmoTypesForModules.Add(string.Empty);
    if (usedModuleIndex >= this.topCapsForModules.Count)
    {
      dfSprite dfSprite1 = this.topCapsForModules[0].Parent.AddPrefab(this.topCapsForModules[0].gameObject) as dfSprite;
      dfSprite dfSprite2 = this.bottomCapsForModules[0].Parent.AddPrefab(this.bottomCapsForModules[0].gameObject) as dfSprite;
      this.topCapsForModules.Add(dfSprite1);
      this.bottomCapsForModules.Add(dfSprite2);
    }
    if (usedModuleIndex < this.m_cachedModuleShotsRemaining.Count)
      return;
    this.m_cachedModuleShotsRemaining.Add(0);
  }

  public void UpdateUIGun(GunInventory guns, int inventoryShift)
  {
    if (!this.m_initialized)
      this.Initialize();
    if (guns.AllGuns.Count != 0 && guns.AllGuns.Count - 1 != this.AdditionalGunBoxSprites.Count && GameUIRoot.Instance.GunventoryFolded && !this.m_isCurrentlyFlipping && (guns.AllGuns.Count - 1 < this.AdditionalGunBoxSprites.Count || this.AdditionalGunBoxSprites.Count < GameUIAmmoController.NumberOfAdditionalGunCards))
      this.RebuildExtraGunCards(guns);
    Gun currentGun1 = guns.CurrentGun;
    Gun currentSecondaryGun = guns.CurrentSecondaryGun;
    if ((UnityEngine.Object) currentGun1 == (UnityEngine.Object) null || GameUIRoot.Instance.ForceHideGunPanel || this.temporarilyPreventVisible || this.forceInvisiblePermanent)
    {
      this.ToggleRenderers(false);
    }
    else
    {
      this.GunQuickSwitchIcon.IsVisible = false;
      int numberModules = 0;
      for (int index = 0; index < currentGun1.Volley.projectiles.Count; ++index)
      {
        ProjectileModule projectile = currentGun1.Volley.projectiles[index];
        if (projectile == currentGun1.DefaultModule || projectile.IsDuctTapeModule && projectile.ammoCost > 0)
          ++numberModules;
      }
      if ((bool) (UnityEngine.Object) currentSecondaryGun)
      {
        for (int index = 0; index < currentSecondaryGun.Volley.projectiles.Count; ++index)
        {
          ProjectileModule projectile = currentSecondaryGun.Volley.projectiles[index];
          if (projectile == currentSecondaryGun.DefaultModule || projectile.IsDuctTapeModule && projectile.ammoCost > 0)
            ++numberModules;
        }
      }
      bool didChangeGun = (UnityEngine.Object) currentGun1 != (UnityEngine.Object) this.m_cachedGun || currentGun1.DidTransformGunThisFrame;
      currentGun1.DidTransformGunThisFrame = false;
      this.UpdateGunSprite(currentGun1, inventoryShift, currentSecondaryGun);
      if (numberModules != this.m_cachedNumberModules)
      {
        int num = numberModules - this.m_cachedNumberModules;
        float x = (float) ((!this.IsLeftAligned ? 1.0 : -1.0) * (double) Pixelator.Instance.CurrentTileScale * (double) num * -10.0);
        dfLabel gunAmmoCountLabel = this.GunAmmoCountLabel;
        gunAmmoCountLabel.RelativePosition = gunAmmoCountLabel.RelativePosition + new Vector3(x, 0.0f, 0.0f);
        dfSprite gunBoxSprite = this.GunBoxSprite;
        gunBoxSprite.RelativePosition = gunBoxSprite.RelativePosition + new Vector3(x, 0.0f, 0.0f);
      }
      if (this.m_cachedTotalAmmo != currentGun1.CurrentAmmo || this.m_cachedMaxAmmo != currentGun1.AdjustedMaxAmmo || this.m_cachedUndertaleness != currentGun1.IsUndertaleGun)
      {
        if (currentGun1.IsUndertaleGun)
        {
          if (!this.IsLeftAligned && this.m_cachedMaxAmmo == int.MaxValue)
            this.GunAmmoCountLabel.RelativePosition = this.GunAmmoCountLabel.RelativePosition + new Vector3(3f, 0.0f, 0.0f);
          this.GunAmmoCountLabel.Text = "0/0";
        }
        else if (currentGun1.InfiniteAmmo)
        {
          if (!this.IsLeftAligned && (!(bool) (UnityEngine.Object) this.m_cachedGun || !this.m_cachedGun.InfiniteAmmo))
            this.GunAmmoCountLabel.RelativePosition = this.GunAmmoCountLabel.RelativePosition + new Vector3(-3f, 0.0f, 0.0f);
          this.GunAmmoCountLabel.ProcessMarkup = true;
          this.GunAmmoCountLabel.ColorizeSymbols = false;
          this.GunAmmoCountLabel.Text = "[sprite \"infinite-big\"]";
        }
        else if (currentGun1.AdjustedMaxAmmo > 0)
        {
          if (!this.IsLeftAligned && this.m_cachedMaxAmmo == int.MaxValue)
            this.GunAmmoCountLabel.RelativePosition = this.GunAmmoCountLabel.RelativePosition + new Vector3(3f, 0.0f, 0.0f);
          this.GunAmmoCountLabel.Text = $"{currentGun1.CurrentAmmo.ToString()}/{currentGun1.AdjustedMaxAmmo.ToString()}";
        }
        else
        {
          if (!this.IsLeftAligned && this.m_cachedMaxAmmo == int.MaxValue)
            this.GunAmmoCountLabel.RelativePosition = this.GunAmmoCountLabel.RelativePosition + new Vector3(3f, 0.0f, 0.0f);
          this.GunAmmoCountLabel.Text = currentGun1.CurrentAmmo.ToString();
        }
      }
      this.CleanupLists(numberModules);
      int num1 = 0;
      int count = currentGun1.Volley.projectiles.Count;
      if ((bool) (UnityEngine.Object) currentSecondaryGun)
        count += currentSecondaryGun.Volley.projectiles.Count;
      for (int index1 = 0; index1 < count; ++index1)
      {
        Gun currentGun2 = index1 < currentGun1.Volley.projectiles.Count ? currentGun1 : currentSecondaryGun;
        int index2 = !((UnityEngine.Object) currentGun2 == (UnityEngine.Object) currentGun1) ? index1 - currentGun1.Volley.projectiles.Count : index1;
        ProjectileModule projectile = currentGun2.Volley.projectiles[index2];
        if (projectile == currentGun2.DefaultModule || projectile.IsDuctTapeModule && projectile.ammoCost > 0)
        {
          this.EnsureInitialization(num1);
          dfTiledSprite spritesForModule1 = this.fgSpritesForModules[num1];
          dfTiledSprite spritesForModule2 = this.bgSpritesForModules[num1];
          List<dfTiledSprite> spritesForModule3 = this.addlFgSpritesForModules[num1];
          List<dfTiledSprite> spritesForModule4 = this.addlBgSpritesForModules[num1];
          dfSprite topCapsForModule = this.topCapsForModules[num1];
          dfSprite bottomCapsForModule = this.bottomCapsForModules[num1];
          GameUIAmmoType.AmmoType ammoTypesForModule1 = this.cachedAmmoTypesForModules[num1];
          string ammoTypesForModule2 = this.cachedCustomAmmoTypesForModules[num1];
          int cachedShotsInClip = this.m_cachedModuleShotsRemaining[num1];
          this.UpdateAmmoUIForModule(ref spritesForModule1, ref spritesForModule2, spritesForModule3, spritesForModule4, topCapsForModule, bottomCapsForModule, projectile, currentGun2, ref ammoTypesForModule1, ref ammoTypesForModule2, ref cachedShotsInClip, didChangeGun, numberModules - (num1 + 1));
          this.fgSpritesForModules[num1] = spritesForModule1;
          this.bgSpritesForModules[num1] = spritesForModule2;
          this.cachedAmmoTypesForModules[num1] = ammoTypesForModule1;
          this.cachedCustomAmmoTypesForModules[num1] = ammoTypesForModule2;
          this.m_cachedModuleShotsRemaining[num1] = cachedShotsInClip;
          ++num1;
        }
      }
      if (currentGun1.IsHeroSword)
      {
        for (int index = 0; index < this.bgSpritesForModules.Count; ++index)
        {
          this.fgSpritesForModules[index].IsVisible = false;
          this.bgSpritesForModules[index].IsVisible = false;
        }
        for (int index = 0; index < this.topCapsForModules.Count; ++index)
        {
          this.topCapsForModules[index].IsVisible = false;
          this.bottomCapsForModules[index].IsVisible = false;
        }
      }
      else if (!this.bottomCapsForModules[0].IsVisible)
      {
        for (int index = 0; index < this.bgSpritesForModules.Count; ++index)
        {
          this.fgSpritesForModules[index].IsVisible = true;
          this.bgSpritesForModules[index].IsVisible = true;
        }
        for (int index = 0; index < this.topCapsForModules.Count; ++index)
        {
          this.topCapsForModules[index].IsVisible = true;
          this.bottomCapsForModules[index].IsVisible = true;
        }
      }
      this.GunClipCountLabel.IsVisible = false;
      this.m_cachedGun = currentGun1;
      this.m_cachedNumberModules = numberModules;
      this.m_cachedTotalAmmo = currentGun1.CurrentAmmo;
      this.m_cachedMaxAmmo = currentGun1.AdjustedMaxAmmo;
      this.m_cachedUndertaleness = currentGun1.IsUndertaleGun;
      this.UpdateAdditionalSprites();
    }
  }

  private void UpdateAmmoUIForModule(
    ref dfTiledSprite currentAmmoFGSprite,
    ref dfTiledSprite currentAmmoBGSprite,
    List<dfTiledSprite> AddlModuleFGSprites,
    List<dfTiledSprite> AddlModuleBGSprites,
    dfSprite ModuleTopCap,
    dfSprite ModuleBottomCap,
    ProjectileModule module,
    Gun currentGun,
    ref GameUIAmmoType.AmmoType cachedAmmoTypeForModule,
    ref string cachedCustomAmmoTypeForModule,
    ref int cachedShotsInClip,
    bool didChangeGun,
    int numberRemaining)
  {
    int b1 = module.GetModNumberOfShotsInClip(currentGun.CurrentOwner) > 0 ? module.GetModNumberOfShotsInClip(currentGun.CurrentOwner) - currentGun.RuntimeModuleData[module].numberShotsFired : currentGun.ammo;
    if (b1 > currentGun.ammo)
      b1 = currentGun.ammo;
    int num1 = module.GetModNumberOfShotsInClip(currentGun.CurrentOwner) > 0 ? module.GetModNumberOfShotsInClip(currentGun.CurrentOwner) : currentGun.AdjustedMaxAmmo;
    if (currentGun.RequiresFundsToShoot)
    {
      b1 = Mathf.FloorToInt((float) (currentGun.CurrentOwner as PlayerController).carriedConsumables.Currency / (float) currentGun.CurrencyCostPerShot);
      num1 = Mathf.FloorToInt((float) (currentGun.CurrentOwner as PlayerController).carriedConsumables.Currency / (float) currentGun.CurrencyCostPerShot);
    }
    if ((UnityEngine.Object) currentAmmoFGSprite == (UnityEngine.Object) null || didChangeGun || module.ammoType != cachedAmmoTypeForModule || module.customAmmoType != cachedCustomAmmoTypeForModule)
    {
      this.m_additionalAmmoTypeDefinitions.Clear();
      if ((UnityEngine.Object) currentAmmoFGSprite != (UnityEngine.Object) null)
        UnityEngine.Object.Destroy((UnityEngine.Object) currentAmmoFGSprite.gameObject);
      if ((UnityEngine.Object) currentAmmoBGSprite != (UnityEngine.Object) null)
        UnityEngine.Object.Destroy((UnityEngine.Object) currentAmmoBGSprite.gameObject);
      for (int index = 0; index < AddlModuleBGSprites.Count; ++index)
      {
        UnityEngine.Object.Destroy((UnityEngine.Object) AddlModuleBGSprites[index].gameObject);
        UnityEngine.Object.Destroy((UnityEngine.Object) AddlModuleFGSprites[index].gameObject);
      }
      AddlModuleBGSprites.Clear();
      AddlModuleFGSprites.Clear();
      GameUIAmmoType uiAmmoType1 = this.GetUIAmmoType(module.ammoType, module.customAmmoType);
      GameObject gameObject1 = UnityEngine.Object.Instantiate<GameObject>(uiAmmoType1.ammoBarFG.gameObject);
      GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(uiAmmoType1.ammoBarBG.gameObject);
      gameObject1.transform.parent = this.GunBoxSprite.transform.parent;
      gameObject2.transform.parent = this.GunBoxSprite.transform.parent;
      gameObject1.name = uiAmmoType1.ammoBarFG.name;
      gameObject2.name = uiAmmoType1.ammoBarBG.name;
      currentAmmoFGSprite = gameObject1.GetComponent<dfTiledSprite>();
      currentAmmoBGSprite = gameObject2.GetComponent<dfTiledSprite>();
      this.m_panel.AddControl((dfControl) currentAmmoFGSprite);
      this.m_panel.AddControl((dfControl) currentAmmoBGSprite);
      currentAmmoFGSprite.EnableBlackLineFix = module.shootStyle == ProjectileModule.ShootStyle.Beam;
      currentAmmoBGSprite.EnableBlackLineFix = currentAmmoFGSprite.EnableBlackLineFix;
      if (module.usesOptionalFinalProjectile)
      {
        GameUIAmmoType uiAmmoType2 = this.GetUIAmmoType(module.finalAmmoType, module.finalCustomAmmoType);
        this.m_additionalAmmoTypeDefinitions.Add(uiAmmoType2);
        GameObject gameObject3 = UnityEngine.Object.Instantiate<GameObject>(uiAmmoType2.ammoBarFG.gameObject);
        GameObject gameObject4 = UnityEngine.Object.Instantiate<GameObject>(uiAmmoType2.ammoBarBG.gameObject);
        gameObject3.transform.parent = this.GunBoxSprite.transform.parent;
        gameObject4.transform.parent = this.GunBoxSprite.transform.parent;
        gameObject3.name = uiAmmoType2.ammoBarFG.name;
        gameObject4.name = uiAmmoType2.ammoBarBG.name;
        AddlModuleFGSprites.Add(gameObject3.GetComponent<dfTiledSprite>());
        AddlModuleBGSprites.Add(gameObject4.GetComponent<dfTiledSprite>());
        this.m_panel.AddControl((dfControl) AddlModuleFGSprites[0]);
        this.m_panel.AddControl((dfControl) AddlModuleBGSprites[0]);
      }
    }
    float currentTileScale = Pixelator.Instance.CurrentTileScale;
    int finalProjectiles = !module.usesOptionalFinalProjectile ? 0 : module.GetModifiedNumberOfFinalProjectiles(currentGun.CurrentOwner);
    int b2 = num1 - finalProjectiles;
    int b3 = Mathf.Max(0, b1 - finalProjectiles);
    int num2 = Mathf.Min(finalProjectiles, b1);
    int a = 125;
    if (module.shootStyle == ProjectileModule.ShootStyle.Beam)
    {
      a = 500;
      finalProjectiles = Mathf.CeilToInt((float) finalProjectiles / 2f);
      b2 = Mathf.CeilToInt((float) b2 / 2f);
      b3 = Mathf.CeilToInt((float) b3 / 2f);
      num2 = Mathf.CeilToInt((float) num2 / 2f);
    }
    int num3 = Mathf.Min(a, b2);
    int num4 = Mathf.Min(a, b3);
    currentAmmoBGSprite.Size = new Vector2(currentAmmoBGSprite.SpriteInfo.sizeInPixels.x * currentTileScale, currentAmmoBGSprite.SpriteInfo.sizeInPixels.y * currentTileScale * (float) num3);
    currentAmmoFGSprite.Size = new Vector2(currentAmmoFGSprite.SpriteInfo.sizeInPixels.x * currentTileScale, currentAmmoFGSprite.SpriteInfo.sizeInPixels.y * currentTileScale * (float) num4);
    for (int index = 0; index < AddlModuleBGSprites.Count; ++index)
    {
      AddlModuleBGSprites[index].Size = new Vector2(AddlModuleBGSprites[index].SpriteInfo.sizeInPixels.x * currentTileScale, AddlModuleBGSprites[index].SpriteInfo.sizeInPixels.y * currentTileScale * (float) finalProjectiles);
      AddlModuleFGSprites[index].Size = new Vector2(AddlModuleFGSprites[index].SpriteInfo.sizeInPixels.x * currentTileScale, AddlModuleFGSprites[index].SpriteInfo.sizeInPixels.y * currentTileScale * (float) num2);
    }
    if (!didChangeGun && (UnityEngine.Object) this.AmmoBurstVFX != (UnityEngine.Object) null && cachedShotsInClip > b1 && !currentGun.IsReloading)
    {
      int num5 = cachedShotsInClip - b1;
      for (int index = 0; index < num5; ++index)
      {
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.AmmoBurstVFX.gameObject);
        dfSprite component1 = gameObject.GetComponent<dfSprite>();
        dfSpriteAnimation component2 = gameObject.GetComponent<dfSpriteAnimation>();
        component1.ZOrder = currentAmmoFGSprite.ZOrder + 1;
        float num6 = component1.Size.y / 2f;
        currentAmmoFGSprite.AddControl((dfControl) component1);
        component1.transform.position = currentAmmoFGSprite.GetCenter();
        component1.RelativePosition = component1.RelativePosition.WithY((float) index * currentAmmoFGSprite.SpriteInfo.sizeInPixels.y * currentTileScale - num6);
        if (num4 == 0 && finalProjectiles > 0)
          component1.RelativePosition = component1.RelativePosition + new Vector3(0.0f, AddlModuleFGSprites[0].SpriteInfo.sizeInPixels.y * currentTileScale * Mathf.Max(0.0f, (float) (finalProjectiles - num2) - 0.5f), 0.0f);
        component2.Play();
      }
    }
    float x = -Pixelator.Instance.CurrentTileScale + (float) ((double) currentTileScale * (double) numberRemaining * -10.0);
    float y = 0.0f;
    float num7 = AddlModuleBGSprites.Count <= 0 ? 0.0f : AddlModuleBGSprites[0].Size.y;
    if (this.IsLeftAligned)
      ModuleBottomCap.RelativePosition = this.m_panel.Size.WithX(0.0f).ToVector3ZUp() - ModuleBottomCap.Size.WithX(0.0f).ToVector3ZUp() + new Vector3(-x, y, 0.0f);
    else
      ModuleBottomCap.RelativePosition = this.m_panel.Size.ToVector3ZUp() - ModuleBottomCap.Size.ToVector3ZUp() + new Vector3(x, -y, 0.0f);
    ModuleTopCap.RelativePosition = ModuleBottomCap.RelativePosition + new Vector3(0.0f, (float) (-(double) currentAmmoBGSprite.Size.y + -(double) num7 + -(double) ModuleTopCap.Size.y), 0.0f);
    float num8 = ModuleTopCap.Size.x / 2f;
    float num9 = BraveMathCollege.QuantizeFloat(currentAmmoBGSprite.Size.x / 2f - num8, currentTileScale);
    float num10 = currentAmmoFGSprite.Size.x / 2f - num8;
    currentAmmoBGSprite.RelativePosition = ModuleTopCap.RelativePosition + new Vector3(-num9, ModuleTopCap.Size.y, 0.0f);
    currentAmmoFGSprite.RelativePosition = ModuleTopCap.RelativePosition + new Vector3(-num10, ModuleTopCap.Size.y + currentAmmoFGSprite.SpriteInfo.sizeInPixels.y * (float) (num3 - num4) * currentTileScale, 0.0f);
    currentAmmoFGSprite.ZOrder = currentAmmoBGSprite.ZOrder + 1;
    if (AddlModuleBGSprites.Count > 0)
    {
      float num11 = BraveMathCollege.QuantizeFloat(AddlModuleBGSprites[0].Size.x / 2f - num8, currentTileScale);
      AddlModuleBGSprites[0].RelativePosition = ModuleTopCap.RelativePosition + new Vector3(-num11, ModuleTopCap.Size.y + currentAmmoBGSprite.Size.y, 0.0f);
      float num12 = AddlModuleFGSprites[0].Size.x / 2f - num8;
      AddlModuleFGSprites[0].RelativePosition = ModuleTopCap.RelativePosition + new Vector3(-num12, (float) ((double) ModuleTopCap.Size.y + (double) currentAmmoBGSprite.Size.y + (double) AddlModuleFGSprites[0].SpriteInfo.sizeInPixels.y * (double) (finalProjectiles - num2) * (double) currentTileScale), 0.0f);
    }
    cachedAmmoTypeForModule = module.ammoType;
    cachedCustomAmmoTypeForModule = module.customAmmoType;
    cachedShotsInClip = b1;
  }

  protected override void OnDestroy() => base.OnDestroy();
}
