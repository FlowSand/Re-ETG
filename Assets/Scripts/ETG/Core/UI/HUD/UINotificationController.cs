using Dungeonator;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class UINotificationController : MonoBehaviour
  {
    public tk2dBaseSprite notificationObjectSprite;
    public tk2dBaseSprite notificationSynergySprite;
    public dfSprite ObjectBoxSprite;
    public dfSprite CrosshairSprite;
    public dfSprite StickerSprite;
    public dfSprite BoxSprite;
    public dfLabel NameLabel;
    public dfLabel DescriptionLabel;
    public dfLabel CenterLabel;
    public dfAnimationClip SilverAnimClip;
    public dfAnimationClip GoldAnimClip;
    public dfAnimationClip PurpleAnimClip;
    [Header("Synergues")]
    public dfAnimationClip SynergyTransformClip;
    public dfAnimationClip SynergyBoxTransformClip;
    public dfAnimationClip SynergyCrosshairTransformClip;
    private tk2dSprite[] outlineSprites;
    private tk2dSprite[] synergyOutlineSprites;
    private dfPanel m_panel;
    private List<IEnumerator> m_queuedNotifications = new List<IEnumerator>();
    private List<NotificationParams> m_queuedNotificationParams = new List<NotificationParams>();
    private IEnumerator m_currentNotificationProcess;
    private bool m_doingNotification;
    private dfFontBase EnglishFont;
    private dfFontBase OtherLanguageFont;
    private Vector3 NameBasePos;
    private Vector3 DescBasePos;
    private StringTableManager.GungeonSupportedLanguages m_cachedLanguage;
    private bool m_isCurrentlyExpanded;
    private bool m_textsLowered;

    public dfPanel Panel => this.m_panel;

    public bool IsDoingNotification => this.m_doingNotification;

    private void Start()
    {
      if (!((Object) this.EnglishFont == (Object) null))
        return;
      this.EnglishFont = this.DescriptionLabel.DefaultAssignedFont;
      this.OtherLanguageFont = this.DescriptionLabel.GUIManager.DefaultFont;
      this.NameBasePos = this.NameLabel.RelativePosition;
      this.DescBasePos = this.DescriptionLabel.RelativePosition;
    }

    public void Initialize()
    {
      this.m_panel = this.GetComponent<dfPanel>();
      GameUIRoot.Instance.AddControlToMotionGroups((dfControl) this.m_panel, DungeonData.Direction.SOUTH, true);
      this.notificationObjectSprite.HeightOffGround = GameUIRoot.Instance.transform.position.y - 2f;
      this.notificationSynergySprite.HeightOffGround = GameUIRoot.Instance.transform.position.y - 1f;
      SpriteOutlineManager.AddOutlineToSprite(this.notificationObjectSprite, Color.white, -1f);
      this.outlineSprites = SpriteOutlineManager.GetOutlineSprites(this.notificationObjectSprite);
      for (int index = 0; index < this.outlineSprites.Length; ++index)
      {
        tk2dSprite outlineSprite = this.outlineSprites[index];
        if ((bool) (Object) outlineSprite)
        {
          outlineSprite.gameObject.layer = this.notificationObjectSprite.gameObject.layer;
          outlineSprite.renderer.enabled = this.notificationObjectSprite.renderer.enabled;
          outlineSprite.HeightOffGround = -0.25f;
        }
      }
      SpriteOutlineManager.AddOutlineToSprite(this.notificationSynergySprite, Color.white, -1f);
      this.synergyOutlineSprites = SpriteOutlineManager.GetOutlineSprites(this.notificationSynergySprite);
      for (int index = 0; index < this.synergyOutlineSprites.Length; ++index)
      {
        tk2dSprite synergyOutlineSprite = this.synergyOutlineSprites[index];
        if ((bool) (Object) synergyOutlineSprite)
        {
          synergyOutlineSprite.gameObject.layer = this.notificationSynergySprite.gameObject.layer;
          synergyOutlineSprite.renderer.enabled = this.notificationSynergySprite.renderer.enabled;
          synergyOutlineSprite.HeightOffGround = -0.25f;
        }
      }
      this.notificationObjectSprite.UpdateZDepth();
      this.notificationSynergySprite.UpdateZDepth();
      this.CheckLanguageFonts();
      this.StartCoroutine(this.BG_CoroutineProcessor());
    }

    public void ForceHide()
    {
      if (!this.IsDoingNotification)
        return;
      GameUIRoot.Instance.MoveNonCoreGroupOnscreen((dfControl) this.m_panel, true);
      this.m_currentNotificationProcess = (IEnumerator) null;
      this.m_queuedNotifications.Clear();
    }

    [DebuggerHidden]
    private IEnumerator BG_CoroutineProcessor()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new UINotificationController__BG_CoroutineProcessorc__Iterator0()
      {
        _this = this
      };
    }

    private float ActualSign(float f)
    {
      if ((double) Mathf.Abs(f) < 9.9999997473787516E-05)
        return 0.0f;
      if ((double) f < 0.0)
        return -1f;
      return (double) f > 0.0 ? 1f : 0.0f;
    }

    private void CheckLanguageFonts()
    {
      if ((Object) this.EnglishFont == (Object) null)
      {
        this.EnglishFont = this.DescriptionLabel.Font;
        this.OtherLanguageFont = this.DescriptionLabel.GUIManager.DefaultFont;
        this.NameBasePos = this.NameLabel.RelativePosition;
        this.DescBasePos = this.DescriptionLabel.RelativePosition;
      }
      switch (StringTableManager.CurrentLanguage)
      {
        case StringTableManager.GungeonSupportedLanguages.ENGLISH:
          if (this.m_cachedLanguage != StringTableManager.GungeonSupportedLanguages.ENGLISH)
          {
            this.NameLabel.RelativePosition = this.NameBasePos;
            this.NameLabel.Font = this.EnglishFont;
            this.NameLabel.TextScale = 0.6f;
            this.DescriptionLabel.RelativePosition = this.DescBasePos;
            this.DescriptionLabel.Font = this.EnglishFont;
            this.DescriptionLabel.TextScale = 0.6f;
            break;
          }
          break;
        case StringTableManager.GungeonSupportedLanguages.JAPANESE:
          if (this.m_cachedLanguage != StringTableManager.GungeonSupportedLanguages.JAPANESE)
          {
            this.NameLabel.RelativePosition = this.NameBasePos + new Vector3(0.0f, -9f, 0.0f);
            this.NameLabel.TextScale = 3f;
            this.DescriptionLabel.RelativePosition = this.DescBasePos + new Vector3(0.0f, -6f, 0.0f);
            this.DescriptionLabel.TextScale = 3f;
            break;
          }
          break;
        case StringTableManager.GungeonSupportedLanguages.KOREAN:
          if (this.m_cachedLanguage != StringTableManager.CurrentLanguage)
          {
            this.NameLabel.RelativePosition = this.NameBasePos + new Vector3(0.0f, -9f, 0.0f);
            this.DescriptionLabel.RelativePosition = this.DescBasePos + new Vector3(0.0f, -6f, 0.0f);
            break;
          }
          break;
        case StringTableManager.GungeonSupportedLanguages.RUSSIAN:
          if (this.m_cachedLanguage != StringTableManager.CurrentLanguage)
          {
            this.NameLabel.RelativePosition = this.NameBasePos + new Vector3(0.0f, -12f, 0.0f);
            this.NameLabel.TextScale = 3f;
            this.DescriptionLabel.RelativePosition = this.DescBasePos + new Vector3(0.0f, -9f, 0.0f);
            this.DescriptionLabel.TextScale = 3f;
            break;
          }
          break;
        case StringTableManager.GungeonSupportedLanguages.CHINESE:
          if (StringTableManager.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.CHINESE)
          {
            this.NameLabel.RelativePosition = this.NameBasePos + new Vector3(0.0f, -24f, 0.0f);
            this.NameLabel.TextScale = 3f;
            this.DescriptionLabel.RelativePosition = this.DescBasePos + new Vector3(0.0f, -12f, 0.0f);
            this.DescriptionLabel.TextScale = 3f;
            break;
          }
          break;
        default:
          if (this.m_cachedLanguage != StringTableManager.CurrentLanguage)
          {
            this.NameLabel.RelativePosition = this.NameBasePos + new Vector3(0.0f, -12f, 0.0f);
            this.NameLabel.Font = this.OtherLanguageFont;
            this.NameLabel.TextScale = 3f;
            this.DescriptionLabel.RelativePosition = this.DescBasePos + new Vector3(0.0f, -9f, 0.0f);
            this.DescriptionLabel.Font = this.OtherLanguageFont;
            this.DescriptionLabel.TextScale = 3f;
            break;
          }
          break;
      }
      this.m_cachedLanguage = StringTableManager.CurrentLanguage;
    }

    private void SetWidths()
    {
      bool flag = GameManager.Options.CurrentLanguage != StringTableManager.GungeonSupportedLanguages.ENGLISH && GameManager.Options.CurrentLanguage != StringTableManager.GungeonSupportedLanguages.JAPANESE && GameManager.Options.CurrentLanguage != StringTableManager.GungeonSupportedLanguages.KOREAN && GameManager.Options.CurrentLanguage != StringTableManager.GungeonSupportedLanguages.CHINESE;
      if (flag && !this.m_isCurrentlyExpanded)
      {
        this.m_isCurrentlyExpanded = true;
        this.m_panel.Width += 126f;
        this.NameLabel.Width += 126f;
        this.DescriptionLabel.Width += 126f;
        this.BoxSprite.Width += 42f;
        this.m_panel.PerformLayout();
        GameUIRoot.Instance.MoveNonCoreGroupImmediately((dfControl) this.m_panel);
        GameUIRoot.Instance.UpdateControlMotionGroup((dfControl) this.m_panel);
        GameUIRoot.Instance.MoveNonCoreGroupImmediately((dfControl) this.m_panel, true);
      }
      else
      {
        if (flag || !this.m_isCurrentlyExpanded)
          return;
        this.m_isCurrentlyExpanded = false;
        this.NameLabel.Width -= 126f;
        this.DescriptionLabel.Width -= 126f;
        this.BoxSprite.Width -= 42f;
        this.m_panel.Width -= 126f;
        this.m_panel.PerformLayout();
        GameUIRoot.Instance.MoveNonCoreGroupImmediately((dfControl) this.m_panel);
        GameUIRoot.Instance.UpdateControlMotionGroup((dfControl) this.m_panel);
        GameUIRoot.Instance.MoveNonCoreGroupImmediately((dfControl) this.m_panel, true);
      }
    }

    public void DoNotification(EncounterTrackable trackable, bool onlyIfSynergy = false)
    {
      if (!(bool) (Object) trackable)
        return;
      this.CheckLanguageFonts();
      this.SetWidths();
      tk2dBaseSprite component1 = trackable.GetComponent<tk2dBaseSprite>();
      NotificationParams notifyParams1 = new NotificationParams();
      notifyParams1.EncounterGuid = trackable.EncounterGuid;
      PickupObject component2 = trackable.GetComponent<PickupObject>();
      if ((bool) (Object) component2)
        notifyParams1.pickupId = component2.PickupObjectId;
      notifyParams1.SpriteCollection = component1.Collection;
      notifyParams1.SpriteID = component1.spriteId;
      NotificationParams notifyParams2 = this.SetupTexts(trackable, notifyParams1);
      notifyParams2.OnlyIfSynergy = onlyIfSynergy;
      if (StringTableManager.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.JAPANESE)
      {
        this.NameLabel.RelativePosition = this.NameBasePos + new Vector3(0.0f, -9f, 0.0f);
        this.NameLabel.TextScale = 3f;
        this.DescriptionLabel.RelativePosition = this.DescBasePos + new Vector3(0.0f, -6f, 0.0f);
        this.DescriptionLabel.TextScale = 3f;
      }
      this.DoNotificationInternal(notifyParams2);
    }

    private void DoNotificationInternal(NotificationParams notifyParams)
    {
      this.m_queuedNotifications.Add(this.HandleNotification(notifyParams));
      this.m_queuedNotificationParams.Add(notifyParams);
      this.StartCoroutine(this.PruneQueuedNotifications());
    }

    [DebuggerHidden]
    private IEnumerator PruneQueuedNotifications()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new UINotificationController__PruneQueuedNotificationsc__Iterator1()
      {
        _this = this
      };
    }

    public void DoCustomNotification(
      string header,
      string description,
      tk2dSpriteCollectionData collection,
      int spriteId,
      UINotificationController.NotificationColor notifyColor = UINotificationController.NotificationColor.SILVER,
      bool allowQueueing = false,
      bool forceSingleLine = false)
    {
      this.CheckLanguageFonts();
      this.DoNotificationInternal(new NotificationParams()
      {
        SpriteCollection = collection,
        SpriteID = spriteId,
        PrimaryTitleString = header.ToUpperInvariant(),
        SecondaryDescriptionString = description,
        isSingleLine = forceSingleLine,
        forcedColor = notifyColor
      });
    }

    public void AttemptSynergyAttachment(AdvancedSynergyEntry e)
    {
      for (int index = this.m_queuedNotificationParams.Count - 1; index >= 0; --index)
      {
        NotificationParams notificationParam = this.m_queuedNotificationParams[index];
        if (!string.IsNullOrEmpty(notificationParam.EncounterGuid))
        {
          EncounterDatabaseEntry entry = EncounterDatabase.GetEntry(notificationParam.EncounterGuid);
          int id = entry == null ? -1 : entry.pickupObjectId;
          if (id >= 0 && e.ContainsPickup(id))
          {
            notificationParam.HasAttachedSynergy = true;
            notificationParam.AttachedSynergy = e;
            this.m_queuedNotificationParams[index] = notificationParam;
            break;
          }
        }
      }
    }

    private NotificationParams SetupTexts(
      EncounterTrackable trackable,
      NotificationParams notifyParams)
    {
      string str1 = trackable.name;
      string str2 = "???";
      if (!string.IsNullOrEmpty(trackable.journalData.GetPrimaryDisplayName()))
      {
        str1 = trackable.journalData.GetPrimaryDisplayName();
        if (str1.Contains("®"))
          str1 = str1.Replace('®', '@');
      }
      else
      {
        PickupObject component = trackable.GetComponent<PickupObject>();
        if ((Object) component != (Object) null)
          str1 = component.DisplayName;
      }
      if ((Object) trackable.GetComponent<SpiceItem>() != (Object) null)
      {
        int spiceCount = GameManager.Instance.PrimaryPlayer.spiceCount;
        if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
          spiceCount += GameManager.Instance.SecondaryPlayer.spiceCount;
        str2 = trackable.journalData.GetCustomNotificationPanelDescription(Mathf.Min(spiceCount, 3));
      }
      else if (!string.IsNullOrEmpty(trackable.journalData.GetNotificationPanelDescription()))
        str2 = trackable.journalData.GetNotificationPanelDescription();
      notifyParams.PrimaryTitleString = str1.ToUpperInvariant();
      notifyParams.SecondaryDescriptionString = str2;
      return notifyParams;
    }

    private void ResetSynergySprite()
    {
      this.notificationSynergySprite.renderer.enabled = false;
      this.notificationSynergySprite.scale = GameUIUtility.GetCurrentTK2D_DFScale(this.m_panel.GetManager()) * Vector3.one;
      for (int index = 0; index < this.synergyOutlineSprites.Length; ++index)
      {
        this.synergyOutlineSprites[index].renderer.enabled = false;
        this.synergyOutlineSprites[index].transform.localPosition = (new Vector3(this.ActualSign(this.synergyOutlineSprites[index].transform.localPosition.x) * (1f / 16f), this.ActualSign(this.synergyOutlineSprites[index].transform.localPosition.y) * (1f / 16f), 0.0f) * Pixelator.Instance.CurrentTileScale * 16f * GameUIRoot.Instance.PixelsToUnits()).WithZ(1f);
        this.synergyOutlineSprites[index].scale = this.notificationObjectSprite.scale;
      }
      this.notificationSynergySprite.PlaceAtPositionByAnchor(this.ObjectBoxSprite.GetCenter(), tk2dBaseSprite.Anchor.MiddleCenter);
    }

    private void SetupSynergySprite(tk2dSpriteCollectionData collection, int spriteId)
    {
      this.notificationSynergySprite.SetSprite(collection, spriteId);
      this.notificationSynergySprite.PlaceAtPositionByAnchor(this.ObjectBoxSprite.GetCenter(), tk2dBaseSprite.Anchor.MiddleCenter);
      this.notificationSynergySprite.transform.localPosition = this.notificationSynergySprite.transform.localPosition.Quantize(this.BoxSprite.PixelsToUnits() * 3f);
    }

    private void SetupSprite(tk2dSpriteCollectionData collection, int spriteId)
    {
      this.ResetSynergySprite();
      if ((Object) collection == (Object) null || spriteId < 0)
      {
        this.notificationObjectSprite.renderer.enabled = false;
        for (int index = 0; index < this.outlineSprites.Length; ++index)
          this.outlineSprites[index].renderer.enabled = false;
      }
      else
      {
        this.notificationObjectSprite.renderer.enabled = false;
        this.notificationObjectSprite.SetSprite(collection, spriteId);
        this.notificationObjectSprite.scale = GameUIUtility.GetCurrentTK2D_DFScale(this.m_panel.GetManager()) * Vector3.one;
        for (int index = 0; index < this.outlineSprites.Length; ++index)
        {
          this.outlineSprites[index].renderer.enabled = false;
          this.outlineSprites[index].transform.localPosition = (new Vector3(this.ActualSign(this.outlineSprites[index].transform.localPosition.x) * (1f / 16f), this.ActualSign(this.outlineSprites[index].transform.localPosition.y) * (1f / 16f), 0.0f) * Pixelator.Instance.CurrentTileScale * 16f * GameUIRoot.Instance.PixelsToUnits()).WithZ(1f);
          this.outlineSprites[index].scale = this.notificationObjectSprite.scale;
        }
        this.notificationObjectSprite.PlaceAtPositionByAnchor(this.ObjectBoxSprite.GetCenter(), tk2dBaseSprite.Anchor.MiddleCenter);
        this.notificationObjectSprite.transform.localPosition = this.notificationObjectSprite.transform.localPosition.Quantize(this.BoxSprite.PixelsToUnits() * 3f);
      }
    }

    private void ToggleSynergyStatus(bool synergy)
    {
      if (!synergy)
        return;
      this.CrosshairSprite.SpriteName = "crosshair_synergy";
      this.CrosshairSprite.Size = this.CrosshairSprite.SpriteInfo.sizeInPixels * 3f;
      this.BoxSprite.SpriteName = "notification_box_synergy";
      this.ObjectBoxSprite.IsVisible = false;
      this.StickerSprite.IsVisible = false;
    }

    private void ToggleGoldStatus(bool gold)
    {
      this.CrosshairSprite.SpriteName = !gold ? "crosshair" : "crosshair_gold";
      this.CrosshairSprite.Size = this.CrosshairSprite.SpriteInfo.sizeInPixels * 3f;
      this.BoxSprite.SpriteName = !gold ? "notification_box" : "notification_box_gold_001";
      this.ObjectBoxSprite.IsVisible = true;
      this.ObjectBoxSprite.SpriteName = !gold ? "object_box" : "object_box_gold_001";
      this.StickerSprite.IsVisible = gold;
    }

    private void TogglePurpleStatus(bool purple)
    {
      if (!purple)
        return;
      this.CrosshairSprite.SpriteName = "crosshair_gold";
      this.CrosshairSprite.Size = this.CrosshairSprite.SpriteInfo.sizeInPixels * 3f;
      this.BoxSprite.SpriteName = "notification_box_purp_001";
      this.ObjectBoxSprite.IsVisible = true;
      this.ObjectBoxSprite.SpriteName = "object_box_purp_001";
      this.StickerSprite.IsVisible = false;
    }

    private void DisableRenderers()
    {
      this.notificationObjectSprite.renderer.enabled = false;
      SpriteOutlineManager.ToggleOutlineRenderers(this.notificationObjectSprite, false);
      this.m_panel.IsVisible = false;
    }

    public void ForceToFront()
    {
      if (!(bool) (Object) this.m_panel)
        return;
      this.m_panel.Parent.BringToFront();
    }

    private int GetIDOfOwnedSynergizingItem(int sourceID, AdvancedSynergyEntry syn)
    {
      for (int index1 = 0; index1 < GameManager.Instance.AllPlayers.Length; ++index1)
      {
        PlayerController allPlayer = GameManager.Instance.AllPlayers[index1];
        for (int index2 = 0; index2 < allPlayer.inventory.AllGuns.Count; ++index2)
        {
          int pickupObjectId = allPlayer.inventory.AllGuns[index2].PickupObjectId;
          if (pickupObjectId != sourceID && syn.ContainsPickup(pickupObjectId))
            return pickupObjectId;
        }
        for (int index3 = 0; index3 < allPlayer.activeItems.Count; ++index3)
        {
          int pickupObjectId = allPlayer.activeItems[index3].PickupObjectId;
          if (pickupObjectId != sourceID && syn.ContainsPickup(pickupObjectId))
            return pickupObjectId;
        }
        for (int index4 = 0; index4 < allPlayer.passiveItems.Count; ++index4)
        {
          int pickupObjectId = allPlayer.passiveItems[index4].PickupObjectId;
          if (pickupObjectId != sourceID && syn.ContainsPickup(pickupObjectId))
            return pickupObjectId;
        }
      }
      return -1;
    }

    [DebuggerHidden]
    private IEnumerator HandleNotification(NotificationParams notifyParams)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new UINotificationController__HandleNotificationc__Iterator2()
      {
        notifyParams = notifyParams,
        _this = this
      };
    }

    public enum NotificationColor
    {
      SILVER,
      GOLD,
      PURPLE,
    }
  }

