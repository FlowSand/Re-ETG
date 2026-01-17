// Decompiled with JetBrains decompiler
// Type: AmmonomiconPokedexEntry
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.UI.Ammonomicon
{
    public class AmmonomiconPokedexEntry : MonoBehaviour
    {
      public bool IsEquipmentPage;
      public bool ForceEncounterState;
      public AmmonomiconPokedexEntry.EncounterState encounterState;
      public EncounterDatabaseEntry linkedEncounterTrackable;
      public int pickupID = -1;
      public dfSprite questionMarkSprite;
      public List<AdvancedSynergyEntry> activeSynergies;
      private tk2dClippedSprite m_childSprite;
      private dfButton m_button;
      private dfSlicedSprite m_bgSprite;
      [NonSerialized]
      public bool IsGunderfury;
      private const string c_flatSprite = "big_box_page_flat_001";
      private const string c_raisedSprite = "big_box_page_raised_001";
      private const string c_raisedSelectedSprite = "big_box_page_raised_selected_001";
      private dfInputManager m_inputAdapter;
      private List<tk2dClippedSprite> extantSynergyArrows = new List<tk2dClippedSprite>();

      public tk2dClippedSprite ChildSprite => this.m_childSprite;

      private void Awake()
      {
        this.m_button = this.GetComponent<dfButton>();
        this.m_bgSprite = this.GetComponentInChildren<dfSlicedSprite>();
        this.m_button.PrecludeUpdateCycle = true;
        this.m_bgSprite.PrecludeUpdateCycle = true;
        this.questionMarkSprite.PrecludeUpdateCycle = true;
        this.m_button.MouseHover += new MouseEventHandler(this.m_button_MouseHover);
        this.m_button.Click += new MouseEventHandler(this.m_button_Click);
        this.m_button.LostFocus += new FocusEventHandler(this.m_button_LostFocus);
        this.m_button.GotFocus += new FocusEventHandler(this.m_button_GotFocus);
        this.m_button.ControlClippingChanged += new PropertyChangedEventHandler<bool>(this.m_button_ControlClippingChanged);
      }

      private void m_button_ControlClippingChanged(dfControl control, bool value)
      {
        if (this.encounterState == AmmonomiconPokedexEntry.EncounterState.UNKNOWN)
          return;
        this.m_childSprite.renderer.enabled = !value;
        if (this.IsGunderfury)
        {
          int spriteIdByName = this.m_childSprite.Collection.GetSpriteIdByName($"gunderfury_LV{(object) (GunderfuryController.GetCurrentTier() + 1)}0_idle_001", -1);
          if (spriteIdByName != this.m_childSprite.spriteId)
          {
            this.m_childSprite.SetSprite(spriteIdByName);
            this.m_childSprite.PlaceAtLocalPositionByAnchor(Vector3.zero, tk2dBaseSprite.Anchor.MiddleCenter);
          }
        }
        this.UpdateClipping(this.m_childSprite);
        for (int index = 0; index < this.extantSynergyArrows.Count; ++index)
          this.UpdateClipping(this.extantSynergyArrows[index]);
      }

      private void UpdateClipping(tk2dClippedSprite targetSprite)
      {
        if (GameManager.Instance.IsLoadingLevel || !this.m_button.IsVisible)
          return;
        Vector3[] corners = this.m_button.Parent.Parent.Parent.GetCorners();
        float x1 = corners[0].x;
        float y1 = corners[0].y;
        float x2 = corners[3].x;
        float y2 = corners[3].y;
        Bounds untrimmedBounds1 = targetSprite.GetUntrimmedBounds();
        untrimmedBounds1.center += targetSprite.transform.position;
        float x3 = Mathf.Clamp01((x1 - untrimmedBounds1.min.x) / untrimmedBounds1.size.x);
        float y3 = Mathf.Clamp01((y2 - untrimmedBounds1.min.y) / untrimmedBounds1.size.y);
        float x4 = Mathf.Clamp01((x2 - untrimmedBounds1.min.x) / untrimmedBounds1.size.x);
        float y4 = Mathf.Clamp01((y1 - untrimmedBounds1.min.y) / untrimmedBounds1.size.y);
        targetSprite.clipBottomLeft = new Vector2(x3, y3);
        targetSprite.clipTopRight = new Vector2(x4, y4);
        if (!SpriteOutlineManager.HasOutline((tk2dBaseSprite) targetSprite))
          return;
        tk2dClippedSprite[] outlineSprites = SpriteOutlineManager.GetOutlineSprites<tk2dClippedSprite>((tk2dBaseSprite) targetSprite);
        for (int index = 0; index < outlineSprites.Length; ++index)
        {
          Bounds untrimmedBounds2 = outlineSprites[index].GetUntrimmedBounds();
          untrimmedBounds2.center += outlineSprites[index].transform.position;
          float x5 = Mathf.Clamp01((x1 - untrimmedBounds2.min.x) / untrimmedBounds2.size.x);
          float y5 = Mathf.Clamp01((y2 - untrimmedBounds2.min.y) / untrimmedBounds2.size.y);
          float x6 = Mathf.Clamp01((x2 - untrimmedBounds2.min.x) / untrimmedBounds2.size.x);
          float y6 = Mathf.Clamp01((y1 - untrimmedBounds2.min.y) / untrimmedBounds2.size.y);
          outlineSprites[index].clipBottomLeft = new Vector2(x5, y5);
          outlineSprites[index].clipTopRight = new Vector2(x6, y6);
        }
      }

      private void LateUpdate()
      {
        this.UpdateClipping(this.m_childSprite);
        for (int index = 0; index < this.extantSynergyArrows.Count; ++index)
          this.UpdateClipping(this.extantSynergyArrows[index]);
      }

      public void UpdateEncounterState()
      {
        if (GameStatsManager.Instance.QueryEncounterable(this.linkedEncounterTrackable) == 0)
        {
          if (this.linkedEncounterTrackable.PrerequisitesMet() && !this.linkedEncounterTrackable.journalData.SuppressKnownState && !this.linkedEncounterTrackable.journalData.IsEnemy)
            this.SetEncounterState(AmmonomiconPokedexEntry.EncounterState.KNOWN);
          else
            this.SetEncounterState(AmmonomiconPokedexEntry.EncounterState.UNKNOWN);
        }
        else if (this.linkedEncounterTrackable.PrerequisitesMet())
          this.SetEncounterState(AmmonomiconPokedexEntry.EncounterState.ENCOUNTERED);
        else
          this.SetEncounterState(AmmonomiconPokedexEntry.EncounterState.UNKNOWN);
      }

      public void ForceFocus() => this.m_button.Focus(true);

      public void SetEncounterState(AmmonomiconPokedexEntry.EncounterState st)
      {
        if (this.IsEquipmentPage)
          return;
        if (!this.ForceEncounterState)
          this.encounterState = st;
        switch (this.encounterState)
        {
          case AmmonomiconPokedexEntry.EncounterState.ENCOUNTERED:
            this.m_childSprite.usesOverrideMaterial = true;
            this.m_childSprite.renderer.material.shader = ShaderCache.Acquire("Brave/AmmonomiconSpriteListShader");
            this.m_childSprite.renderer.material.DisableKeyword("BRIGHTNESS_CLAMP_ON");
            this.m_childSprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_OFF");
            this.m_childSprite.renderer.material.SetFloat("_SpriteScale", this.m_childSprite.scale.x);
            this.m_childSprite.renderer.material.SetFloat("_Saturation", 1f);
            this.m_childSprite.renderer.material.SetColor("_OverrideColor", new Color(0.4f, 0.4f, 0.4f, 0.0f));
            this.m_childSprite.renderer.enabled = true;
            this.questionMarkSprite.IsVisible = false;
            break;
          case AmmonomiconPokedexEntry.EncounterState.KNOWN:
            this.m_childSprite.usesOverrideMaterial = true;
            this.m_childSprite.renderer.material.shader = ShaderCache.Acquire("Brave/AmmonomiconSpriteListShader");
            this.m_childSprite.renderer.material.DisableKeyword("BRIGHTNESS_CLAMP_ON");
            this.m_childSprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_OFF");
            this.m_childSprite.renderer.material.SetFloat("_SpriteScale", this.m_childSprite.scale.x);
            this.m_childSprite.renderer.material.SetFloat("_Saturation", 0.0f);
            this.m_childSprite.renderer.material.SetColor("_OverrideColor", new Color(0.4f, 0.4f, 0.4f, 0.0f));
            this.m_childSprite.renderer.enabled = true;
            this.questionMarkSprite.IsVisible = false;
            break;
          case AmmonomiconPokedexEntry.EncounterState.UNKNOWN:
            this.m_childSprite.renderer.enabled = false;
            this.questionMarkSprite.IsVisible = true;
            break;
        }
      }

      private void m_button_GotFocus(dfControl control, dfFocusEventArgs args)
      {
        int num = (int) AkSoundEngine.PostEvent("Play_UI_menu_select_01", this.gameObject);
        if (SpriteOutlineManager.HasOutline((tk2dBaseSprite) this.m_childSprite))
        {
          SpriteOutlineManager.RemoveOutlineFromSprite((tk2dBaseSprite) this.m_childSprite, true);
          SpriteOutlineManager.AddScaledOutlineToSprite<tk2dClippedSprite>((tk2dBaseSprite) this.m_childSprite, Color.white, 0.1f, 0.0f);
        }
        this.m_bgSprite.SpriteName = "big_box_page_raised_selected_001";
        AmmonomiconController.Instance.BestInteractingLeftPageRenderer.LastFocusTarget = (IAmmonomiconFocusable) this.m_button;
        if (this.encounterState == AmmonomiconPokedexEntry.EncounterState.ENCOUNTERED)
          AmmonomiconController.Instance.BestInteractingRightPageRenderer.SetRightDataPageTexts((tk2dBaseSprite) this.m_childSprite, this.linkedEncounterTrackable);
        else if (this.encounterState == AmmonomiconPokedexEntry.EncounterState.KNOWN)
        {
          AmmonomiconController.Instance.BestInteractingRightPageRenderer.SetRightDataPageUnknown();
          AmmonomiconController.Instance.BestInteractingRightPageRenderer.SetRightDataPageName((tk2dBaseSprite) this.m_childSprite, this.linkedEncounterTrackable);
        }
        else
          AmmonomiconController.Instance.BestInteractingRightPageRenderer.SetRightDataPageUnknown();
        if (AmmonomiconController.Instance.BestInteractingLeftPageRenderer.pageType != AmmonomiconPageRenderer.PageType.EQUIPMENT_LEFT)
          return;
        this.UpdateSynergyHighlights();
      }

      private void UpdateSynergyHighlights()
      {
        if (GameManager.Instance.IsSelectingCharacter)
          return;
        List<AmmonomiconPokedexEntry> pokedexEntries = AmmonomiconController.Instance.BestInteractingLeftPageRenderer.GetPokedexEntries();
        List<AmmonomiconPokedexEntry> ammonomiconPokedexEntryList = new List<AmmonomiconPokedexEntry>();
        if (this.activeSynergies == null)
          return;
        for (int index1 = 0; index1 < this.activeSynergies.Count; ++index1)
        {
          PlayerController playerController = GameManager.Instance.BestActivePlayer;
          for (int index2 = 0; index2 < GameManager.Instance.AllPlayers.Length; ++index2)
          {
            if (GameManager.Instance.AllPlayers[index2].PlayerIDX == GameManager.Instance.LastPausingPlayerID)
              playerController = GameManager.Instance.AllPlayers[index2];
          }
          AdvancedSynergyEntry activeSynergy = this.activeSynergies[index1];
          if (activeSynergy.ContainsPickup(this.pickupID) && pokedexEntries != null)
          {
            for (int index3 = 0; index3 < pokedexEntries.Count; ++index3)
            {
              if (pokedexEntries[index3].pickupID >= 0 && pokedexEntries[index3].pickupID != this.pickupID && activeSynergy.ContainsPickup(pokedexEntries[index3].pickupID))
              {
                tk2dClippedSprite page = AmmonomiconController.Instance.CurrentLeftPageRenderer.AddSpriteToPage<tk2dClippedSprite>(AmmonomiconController.Instance.EncounterIconCollection, AmmonomiconController.Instance.EncounterIconCollection.GetSpriteIdByName("synergy_ammonomicon_arrow_001"));
                page.SetSprite("synergy_ammonomicon_arrow_001");
                Bounds bounds = pokedexEntries[index3].m_childSprite.GetBounds();
                pokedexEntries[index3].m_childSprite.GetUntrimmedBounds();
                Vector3 size = bounds.size;
                page.transform.position = (pokedexEntries[index3].m_childSprite.WorldCenter.ToVector3ZisY() + new Vector3(-8f * pokedexEntries[index3].m_bgSprite.PixelsToUnits(), (float) ((double) size.y / 2.0 + 32.0 * (double) pokedexEntries[index3].m_bgSprite.PixelsToUnits()), 0.0f)).WithZ(-0.65f);
                page.transform.parent = this.m_childSprite.transform.parent;
                this.extantSynergyArrows.Add(page);
                pokedexEntries[index3].ChangeOutlineColor(SynergyDatabase.SynergyBlue);
                ammonomiconPokedexEntryList.Add(pokedexEntries[index3]);
              }
            }
          }
        }
        if (pokedexEntries == null)
          return;
        for (int index = 0; index < pokedexEntries.Count; ++index)
        {
          if ((UnityEngine.Object) pokedexEntries[index] != (UnityEngine.Object) this && !ammonomiconPokedexEntryList.Contains(pokedexEntries[index]) && SpriteOutlineManager.HasOutline((tk2dBaseSprite) pokedexEntries[index].m_childSprite))
          {
            SpriteOutlineManager.RemoveOutlineFromSprite((tk2dBaseSprite) pokedexEntries[index].m_childSprite, true);
            SpriteOutlineManager.AddScaledOutlineToSprite<tk2dClippedSprite>((tk2dBaseSprite) pokedexEntries[index].m_childSprite, Color.black, 0.1f, 0.05f);
          }
        }
      }

      private void m_button_LostFocus(dfControl control, dfFocusEventArgs args)
      {
        for (int index = 0; index < this.extantSynergyArrows.Count; ++index)
          UnityEngine.Object.Destroy((UnityEngine.Object) this.extantSynergyArrows[index].gameObject);
        this.extantSynergyArrows.Clear();
        if (SpriteOutlineManager.HasOutline((tk2dBaseSprite) this.m_childSprite))
        {
          SpriteOutlineManager.RemoveOutlineFromSprite((tk2dBaseSprite) this.m_childSprite, true);
          SpriteOutlineManager.AddScaledOutlineToSprite<tk2dClippedSprite>((tk2dBaseSprite) this.m_childSprite, Color.black, 0.1f, 0.05f);
        }
        this.m_bgSprite.SpriteName = "big_box_page_flat_001";
      }

      private void m_button_Click(dfControl control, dfMouseEventArgs mouseEvent)
      {
        this.m_button.Focus(true);
      }

      private void m_button_MouseHover(dfControl control, dfMouseEventArgs mouseEvent)
      {
      }

      public void ChangeOutlineColor(Color targetColor)
      {
        SpriteOutlineManager.RemoveOutlineFromSprite((tk2dBaseSprite) this.m_childSprite, true);
        SpriteOutlineManager.AddScaledOutlineToSprite<tk2dClippedSprite>((tk2dBaseSprite) this.m_childSprite, targetColor, 0.1f, 0.0f);
      }

      public void AssignSprite(tk2dClippedSprite sprit)
      {
        this.m_childSprite = sprit;
        this.m_childSprite.ignoresTiltworldDepth = true;
        this.m_childSprite.transform.position += new Vector3(0.0f, 0.0f, -0.5f);
        if (this.encounterState == AmmonomiconPokedexEntry.EncounterState.UNKNOWN)
          return;
        this.m_childSprite.renderer.enabled = !this.m_button.IsControlClipped;
      }

      public enum EncounterState
      {
        ENCOUNTERED,
        KNOWN,
        UNKNOWN,
      }
    }

}
