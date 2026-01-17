// Decompiled with JetBrains decompiler
// Type: GameUIItemController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.UI.HUD
{
    public class GameUIItemController : MonoBehaviour
    {
      public dfSprite ItemBoxSprite;
      public dfSprite ItemBoxFillSprite;
      public dfSprite ItemBoxFGSprite;
      public tk2dClippedSprite itemSprite;
      public GameObject ExtraItemCardPrefab;
      public List<dfControl> AdditionalItemBoxSprites = new List<dfControl>();
      public dfLabel ItemCountLabel;
      [NonSerialized]
      public bool temporarilyPreventVisible;
      public bool IsRightAligned;
      private Material itemSpriteMaterial;
      private tk2dSprite[] outlineSprites;
      private PlayerItem m_cachedItem;
      private bool m_initialized;
      private Material m_ClippedMaterial;
      private Material m_ClippedZWriteOffMaterial;
      private dfPanel m_panel;
      private float UI_OUTLINE_DEPTH = 1f;
      private tk2dSpriteDefinition m_cachedItemSpriteDefinition;
      private bool m_isCurrentlyFlipping;
      private float m_currentItemSpriteXOffset;
      private float m_currentItemSpriteZOffset;
      private bool m_deferCurrentItemSwap;
      private bool m_cardFlippedQueued;
      private const float FLIP_TIME = 0.15f;

      private void Update()
      {
        if (this.temporarilyPreventVisible && (bool) (UnityEngine.Object) this.itemSprite && this.itemSprite.renderer.enabled)
          this.ToggleRenderers(false);
        if (GameManager.Instance.IsLoadingLevel || !Minimap.Instance.IsFullscreen || !(bool) (UnityEngine.Object) this.itemSprite || !(bool) (UnityEngine.Object) this.itemSprite.renderer || !this.itemSprite.renderer.enabled)
          return;
        this.itemSprite.renderer.enabled = false;
      }

      private void Initialize()
      {
        this.m_panel = this.GetComponent<dfPanel>();
        this.itemSprite.usesOverrideMaterial = true;
        this.itemSpriteMaterial = this.itemSprite.renderer.material;
        SpriteOutlineManager.AddOutlineToSprite((tk2dBaseSprite) this.itemSprite, Color.white);
        this.outlineSprites = SpriteOutlineManager.GetOutlineSprites((tk2dBaseSprite) this.itemSprite);
        for (int index = 0; index < this.outlineSprites.Length; ++index)
        {
          if (this.outlineSprites.Length > 1)
          {
            float num1 = index != 1 ? 0.0f : 1f / 16f;
            float x = index != 3 ? num1 : -1f / 16f;
            float num2 = index != 0 ? 0.0f : 1f / 16f;
            float y = index != 2 ? num2 : -1f / 16f;
            this.outlineSprites[index].transform.localPosition = (new Vector3(x, y, 0.0f) * Pixelator.Instance.CurrentTileScale * 16f * GameUIRoot.Instance.PixelsToUnits()).WithZ(this.UI_OUTLINE_DEPTH);
          }
          this.outlineSprites[index].gameObject.layer = this.itemSprite.gameObject.layer;
        }
        this.m_ClippedMaterial = new Material(ShaderCache.Acquire("Daikon Forge/Clipped UI Shader"));
        this.m_ClippedZWriteOffMaterial = new Material(ShaderCache.Acquire("Daikon Forge/Clipped UI Shader ZWriteOff"));
        this.m_initialized = true;
      }

      public void ToggleRenderersOld(bool value)
      {
        if ((bool) (UnityEngine.Object) this.ItemBoxSprite)
          this.ItemBoxSprite.IsVisible = value;
        if ((bool) (UnityEngine.Object) this.ItemCountLabel)
          this.SetItemCountVisible(value);
        if (!((UnityEngine.Object) this.itemSprite != (UnityEngine.Object) null))
          return;
        this.itemSprite.renderer.enabled = value;
        this.outlineSprites = SpriteOutlineManager.GetOutlineSprites((tk2dBaseSprite) this.itemSprite);
        if (this.outlineSprites == null)
          return;
        for (int index = 0; index < this.outlineSprites.Length; ++index)
          this.outlineSprites[index].renderer.enabled = value;
      }

      public void ToggleRenderers(bool value)
      {
        this.itemSprite.renderer.enabled = value;
        if ((UnityEngine.Object) this.ItemBoxSprite != (UnityEngine.Object) null && (UnityEngine.Object) this.ItemBoxSprite.Parent != (UnityEngine.Object) null)
          this.ItemBoxSprite.IsVisible = value;
        if ((UnityEngine.Object) this.ItemBoxSprite != (UnityEngine.Object) null)
          this.ItemBoxSprite.IsVisible = value;
        if ((UnityEngine.Object) this.ItemCountLabel != (UnityEngine.Object) null && !value)
          this.SetItemCountVisible(value);
        for (int index = 0; index < this.AdditionalItemBoxSprites.Count; ++index)
        {
          this.AdditionalItemBoxSprites[index].IsVisible = value;
          this.AdditionalItemBoxSprites[index].IsVisible = value;
        }
        this.outlineSprites = SpriteOutlineManager.GetOutlineSprites((tk2dBaseSprite) this.itemSprite);
        for (int index = 0; index < this.outlineSprites.Length; ++index)
        {
          if ((bool) (UnityEngine.Object) this.outlineSprites[index])
            this.outlineSprites[index].renderer.enabled = value;
        }
      }

      private float ActualSign(float f)
      {
        if ((double) f < 0.0)
          return -1f;
        return (double) f > 0.0 ? 1f : 0.0f;
      }

      private void DoItemCardFlip(PlayerItem newItem, int change)
      {
        if (this.AdditionalItemBoxSprites.Count == 0)
          return;
        if (!this.m_isCurrentlyFlipping)
        {
          if (change > 0)
            this.StartCoroutine(this.HandleItemCardFlipReverse(newItem));
          else
            this.StartCoroutine(this.HandleItemCardFlip(newItem));
        }
        else
        {
          if (this.m_cardFlippedQueued)
            return;
          this.StartCoroutine(this.WaitForCurrentItemFlipToEnd(newItem, change));
        }
      }

      [DebuggerHidden]
      private IEnumerator WaitForCurrentItemFlipToEnd(PlayerItem newItem, int change)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new GameUIItemController__WaitForCurrentItemFlipToEndc__Iterator0()
        {
          change = change,
          newItem = newItem,
          _this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator HandleItemCardFlipReverse(PlayerItem newGun)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new GameUIItemController__HandleItemCardFlipReversec__Iterator1()
        {
          newGun = newGun,
          _this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator HandleItemCardFlip(PlayerItem newItem)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new GameUIItemController__HandleItemCardFlipc__Iterator2()
        {
          _this = this
        };
      }

      private void PostFlipReset(
        Transform newChild,
        Transform gbTransform,
        GameObject placeholderCardObject,
        tk2dClippedSprite oldGunSprite)
      {
        for (int index = 0; index < this.AdditionalItemBoxSprites.Count; ++index)
          (this.AdditionalItemBoxSprites[index] as dfTextureSprite).Material = this.m_ClippedZWriteOffMaterial;
        if ((bool) (UnityEngine.Object) newChild)
        {
          if ((bool) (UnityEngine.Object) gbTransform)
            newChild.parent = gbTransform;
          this.ItemBoxSprite.AddControl(newChild.GetComponent<dfControl>());
          newChild.GetComponent<dfControl>().RelativePosition = new Vector3((float) -this.AdditionalBoxOffsetPX * Pixelator.Instance.CurrentTileScale, 0.0f, 0.0f);
        }
        if ((bool) (UnityEngine.Object) placeholderCardObject)
          UnityEngine.Object.Destroy((UnityEngine.Object) placeholderCardObject);
        if ((bool) (UnityEngine.Object) oldGunSprite)
          UnityEngine.Object.Destroy((UnityEngine.Object) oldGunSprite.gameObject);
        this.m_currentItemSpriteXOffset = 0.0f;
        this.m_currentItemSpriteZOffset = 0.0f;
        this.ItemBoxSprite.IsVisible = true;
      }

      public void UpdateScale()
      {
        this.ItemBoxSprite.Size = this.ItemBoxSprite.SpriteInfo.sizeInPixels * Pixelator.Instance.CurrentTileScale;
        this.ItemBoxFillSprite.Size = new Vector2(3f, 26f) * Pixelator.Instance.CurrentTileScale;
        this.ItemBoxFGSprite.Size = this.ItemBoxFGSprite.SpriteInfo.sizeInPixels * Pixelator.Instance.CurrentTileScale;
        if (this.m_isCurrentlyFlipping)
          return;
        this.ItemBoxFGSprite.RelativePosition = this.ItemBoxSprite.RelativePosition;
        this.ItemBoxFillSprite.RelativePosition = this.ItemBoxSprite.RelativePosition + new Vector3(123f, 3f, 0.0f);
      }

      protected void RebuildExtraItemCards(PlayerItem current, List<PlayerItem> items)
      {
        float units = this.m_panel.PixelsToUnits();
        for (int index = 0; index < this.AdditionalItemBoxSprites.Count; ++index)
          UnityEngine.Object.Destroy((UnityEngine.Object) this.AdditionalItemBoxSprites[index].gameObject);
        this.AdditionalItemBoxSprites.Clear();
        dfControl dfControl = (dfControl) this.ItemBoxSprite;
        Transform transform = this.ItemBoxSprite.transform;
        for (int index = 0; index < items.Count - 1; ++index)
        {
          GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.ExtraItemCardPrefab);
          gameObject.transform.parent = transform;
          dfControl component = gameObject.GetComponent<dfControl>();
          dfControl.AddControl(component);
          component.RelativePosition = new Vector3((float) -this.AdditionalBoxOffsetPX * Pixelator.Instance.CurrentTileScale, 0.0f, 0.0f);
          dfControl = component;
          transform = gameObject.transform;
          this.AdditionalItemBoxSprites.Add(component);
        }
        float x = (float) (this.AdditionalBoxOffsetPX * this.AdditionalItemBoxSprites.Count) * Pixelator.Instance.CurrentTileScale * units;
        if (this.IsRightAligned)
          this.ItemBoxSprite.transform.position = this.ItemBoxSprite.transform.position.WithX(this.m_panel.transform.position.x + (float) -((double) this.ItemBoxSprite.Width * (double) units) + x);
        else
          this.ItemBoxSprite.transform.position = this.m_panel.transform.position + new Vector3(x, 0.0f, 0.0f);
        this.ItemBoxSprite.Invalidate();
      }

      public void DimItemSprite()
      {
        if ((UnityEngine.Object) this.m_cachedItem == (UnityEngine.Object) null)
          return;
        this.itemSprite.gameObject.SetActive(false);
      }

      public void UndimItemSprite()
      {
        if ((UnityEngine.Object) this.m_cachedItem == (UnityEngine.Object) null)
          return;
        this.itemSprite.gameObject.SetActive(true);
      }

      private int AdditionalBoxOffsetPX => this.IsRightAligned ? -2 : 2;

      private void UpdateItemSpriteScale()
      {
        this.itemSprite.scale = GameUIUtility.GetCurrentTK2D_DFScale(this.m_panel.GetManager()) * Vector3.one;
        for (int index = 0; index < this.outlineSprites.Length; ++index)
        {
          if (this.outlineSprites.Length > 1)
          {
            float num1 = index != 1 ? 0.0f : 1f / 16f;
            float x = index != 3 ? num1 : -1f / 16f;
            float num2 = index != 0 ? 0.0f : 1f / 16f;
            float y = index != 2 ? num2 : -1f / 16f;
            this.outlineSprites[index].transform.localPosition = (new Vector3(x, y, 0.0f) * Pixelator.Instance.CurrentTileScale * 16f * GameUIRoot.Instance.PixelsToUnits()).WithZ(this.UI_OUTLINE_DEPTH);
          }
          this.outlineSprites[index].scale = this.itemSprite.scale;
        }
      }

      public Vector3 GetOffsetVectorForItem(PlayerItem newItem, bool isFlippingGun)
      {
        tk2dSpriteDefinition spriteDefinition = this.m_cachedItemSpriteDefinition == null ? newItem.sprite.Collection.spriteDefinitions[newItem.sprite.spriteId] : this.m_cachedItemSpriteDefinition;
        Vector3 offsetVectorForItem = Vector3.Scale((-spriteDefinition.GetBounds().min + -spriteDefinition.GetBounds().extents).Quantize(1f / 16f), this.itemSprite.scale);
        if (isFlippingGun)
          offsetVectorForItem += new Vector3(this.m_currentItemSpriteXOffset, 0.0f, this.m_currentItemSpriteZOffset);
        return offsetVectorForItem;
      }

      private void UpdateItemSprite(PlayerItem newItem, int itemShift)
      {
        tk2dSprite component = newItem.GetComponent<tk2dSprite>();
        if ((UnityEngine.Object) newItem != (UnityEngine.Object) this.m_cachedItem)
          this.DoItemCardFlip(newItem, itemShift);
        this.UpdateItemSpriteScale();
        if (!this.m_deferCurrentItemSwap)
        {
          if (!this.itemSprite.renderer.enabled)
            this.ToggleRenderers(true);
          if (this.itemSprite.spriteId != component.spriteId || (UnityEngine.Object) this.itemSprite.Collection != (UnityEngine.Object) component.Collection)
          {
            this.itemSprite.SetSprite(component.Collection, component.spriteId);
            for (int index = 0; index < this.outlineSprites.Length; ++index)
            {
              this.outlineSprites[index].SetSprite(component.Collection, component.spriteId);
              SpriteOutlineManager.ForceUpdateOutlineMaterial((tk2dBaseSprite) this.outlineSprites[index], (tk2dBaseSprite) component);
            }
          }
        }
        this.itemSprite.transform.position = this.ItemBoxSprite.GetCenter() + this.GetOffsetVectorForItem(newItem, this.m_isCurrentlyFlipping);
        this.itemSprite.transform.position = this.itemSprite.transform.position.Quantize(this.ItemBoxSprite.PixelsToUnits() * 3f);
        if (newItem.PreventCooldownBar || !newItem.IsActive && !newItem.IsOnCooldown || this.m_isCurrentlyFlipping)
        {
          this.ItemBoxFillSprite.IsVisible = false;
          this.ItemBoxFGSprite.IsVisible = false;
          this.ItemBoxSprite.SpriteName = "weapon_box_02";
        }
        else
        {
          this.ItemBoxFillSprite.IsVisible = true;
          this.ItemBoxFGSprite.IsVisible = true;
          this.ItemBoxSprite.SpriteName = "weapon_box_02_cd";
        }
        this.ItemBoxFillSprite.FillAmount = !newItem.IsActive ? 1f - newItem.CooldownPercentage : 1f - newItem.ActivePercentage;
        PlayerController user = GameManager.Instance.PrimaryPlayer;
        if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER && this.IsRightAligned)
          user = GameManager.Instance.SecondaryPlayer;
        if (newItem.IsOnCooldown || !newItem.CanBeUsed(user))
        {
          Color color1 = this.itemSpriteMaterial.GetColor("_OverrideColor");
          Color color2 = new Color(0.0f, 0.0f, 0.0f, 0.8f);
          if (!(color1 != color2))
            return;
          this.itemSpriteMaterial.SetColor("_OverrideColor", color2);
          tk2dSprite[] outlineSprites = SpriteOutlineManager.GetOutlineSprites((tk2dBaseSprite) this.itemSprite);
          Color color3 = new Color(0.4f, 0.4f, 0.4f, 1f);
          for (int index = 0; index < outlineSprites.Length; ++index)
            outlineSprites[index].renderer.material.SetColor("_OverrideColor", color3);
        }
        else
        {
          Color color4 = this.itemSpriteMaterial.GetColor("_OverrideColor");
          Color color5 = new Color(0.0f, 0.0f, 0.0f, 0.0f);
          if (!(color4 != color5))
            return;
          this.itemSpriteMaterial.SetColor("_OverrideColor", color5);
          tk2dSprite[] outlineSprites = SpriteOutlineManager.GetOutlineSprites((tk2dBaseSprite) this.itemSprite);
          Color white = Color.white;
          for (int index = 0; index < outlineSprites.Length; ++index)
            outlineSprites[index].renderer.material.SetColor("_OverrideColor", white);
        }
      }

      public void TriggerUIDisabled()
      {
        if (!GameUIRoot.Instance.ForceHideItemPanel)
          return;
        this.itemSprite.renderer.enabled = false;
        for (int index = 0; index < this.outlineSprites.Length; ++index)
          this.outlineSprites[index].renderer.enabled = false;
        this.ItemBoxSprite.IsVisible = false;
        this.ItemBoxFillSprite.IsVisible = false;
        this.ItemBoxFGSprite.IsVisible = false;
      }

      private void SetItemCountVisible(bool val) => this.ItemCountLabel.IsVisible = val;

      public void UpdateItem(PlayerItem current, List<PlayerItem> items)
      {
        if (!this.m_initialized)
          this.Initialize();
        if (GameUIRoot.Instance.ForceHideItemPanel || this.temporarilyPreventVisible)
        {
          this.ToggleRenderers(false);
        }
        else
        {
          if (items.Count != 0 && items.Count - 1 != this.AdditionalItemBoxSprites.Count && GameUIRoot.Instance.GunventoryFolded)
            this.RebuildExtraItemCards(current, items);
          if ((UnityEngine.Object) current == (UnityEngine.Object) null || GameUIRoot.Instance.ForceHideItemPanel || this.temporarilyPreventVisible)
          {
            if (this.ItemBoxSprite.IsVisible)
            {
              this.itemSprite.renderer.enabled = false;
              for (int index = 0; index < this.outlineSprites.Length; ++index)
                this.outlineSprites[index].renderer.enabled = false;
              this.ItemBoxSprite.IsVisible = false;
              this.ItemBoxFillSprite.IsVisible = false;
              this.ItemBoxFGSprite.IsVisible = false;
            }
            this.SetItemCountVisible(false);
          }
          else
          {
            if ((!this.ItemBoxSprite.IsVisible || !this.itemSprite.renderer.enabled) && !this.m_isCurrentlyFlipping && !this.m_deferCurrentItemSwap)
            {
              this.itemSprite.renderer.enabled = true;
              for (int index = 0; index < this.outlineSprites.Length; ++index)
                this.outlineSprites[index].renderer.enabled = true;
              this.ItemBoxSprite.IsVisible = true;
            }
            if (current.canStack && current.numberOfUses > 1 && current.consumable || current.numberOfUses > 1 && current.UsesNumberOfUsesBeforeCooldown && !current.IsOnCooldown)
            {
              this.SetItemCountVisible(true);
              this.ItemCountLabel.Text = current.numberOfUses.ToString();
            }
            else
            {
              switch (current)
              {
                case EstusFlaskItem _:
                  EstusFlaskItem estusFlaskItem = current as EstusFlaskItem;
                  this.SetItemCountVisible(true);
                  this.ItemCountLabel.Text = estusFlaskItem.RemainingDrinks.ToString();
                  goto label_25;
                case RatPackItem _:
                  if (!current.IsOnCooldown)
                  {
                    RatPackItem ratPackItem = current as RatPackItem;
                    this.SetItemCountVisible(true);
                    this.ItemCountLabel.Text = ratPackItem.ContainedBullets.ToString();
                    goto label_25;
                  }
                  break;
              }
              this.SetItemCountVisible(false);
            }
    label_25:
            int itemShift = 0;
            if ((UnityEngine.Object) current != (UnityEngine.Object) this.m_cachedItem && items.Contains(this.m_cachedItem))
            {
              int num1 = items.IndexOf(this.m_cachedItem);
              int num2 = items.IndexOf(current);
              itemShift = items.Count != 2 ? (num2 != 0 || num1 != items.Count - 1 ? (num2 != items.Count - 1 || num1 != 0 ? num2 - num1 : -1) : 1) : -1;
            }
            else if ((UnityEngine.Object) current != (UnityEngine.Object) this.m_cachedItem)
              itemShift = -1;
            this.UpdateItemSprite(current, itemShift);
          }
          if (this.itemSprite.renderer.enabled && !this.ItemBoxSprite.IsVisible)
            this.ToggleRenderers(true);
          this.m_cachedItem = current;
        }
      }
    }

}
