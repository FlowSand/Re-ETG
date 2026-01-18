using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class AppearedInTheGungeonController : MonoBehaviour
  {
    public dfLabel[] itemNameLabels;
    public tk2dSprite itemSprite;
    private EncounterDatabaseEntry m_curTrackable;
    private bool m_isScalingDown;

    public void Appear(EncounterDatabaseEntry newPickup)
    {
      int num1 = (int) AkSoundEngine.PostEvent("Play_UI_card_open_01", this.gameObject);
      this.m_curTrackable = newPickup;
      dfPanel component = this.itemSprite.transform.parent.GetComponent<dfPanel>();
      tk2dSpriteCollectionData encounterIconCollection = AmmonomiconController.Instance.EncounterIconCollection;
      int spriteIdByName = encounterIconCollection.GetSpriteIdByName(newPickup.journalData.AmmonomiconSprite, 0);
      if (spriteIdByName < 0)
        spriteIdByName = encounterIconCollection.GetSpriteIdByName(AmmonomiconController.AmmonomiconErrorSprite);
      this.itemSprite.SetSprite(encounterIconCollection, spriteIdByName);
      this.itemSprite.transform.localScale = Vector3.one;
      Bounds untrimmedBounds = this.itemSprite.GetUntrimmedBounds();
      Vector2 vector2 = GameUIUtility.TK2DtoDF(untrimmedBounds.size.XY(), component.GUIManager.PixelsToUnits());
      this.itemSprite.scale = new Vector3(vector2.x / untrimmedBounds.size.x, vector2.y / untrimmedBounds.size.y, untrimmedBounds.size.z);
      this.itemSprite.ignoresTiltworldDepth = true;
      this.itemSprite.gameObject.SetLayerRecursively(LayerMask.NameToLayer("SecondaryGUI"));
      SpriteOutlineManager.AddScaledOutlineToSprite<tk2dSprite>((tk2dBaseSprite) this.itemSprite, Color.black, 0.1f, 0.05f);
      this.itemSprite.PlaceAtLocalPositionByAnchor(Vector3.zero, tk2dBaseSprite.Anchor.MiddleCenter);
      for (int index = 0; index < this.itemNameLabels.Length; ++index)
      {
        this.itemNameLabels[index].Text = newPickup.journalData.GetPrimaryDisplayName().ToUpperInvariant();
        float num2 = GameManager.Options.CurrentLanguage != StringTableManager.GungeonSupportedLanguages.JAPANESE ? 1f : 3f;
        float num3 = 10f;
        if (GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.KOREAN || GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.CHINESE || GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.JAPANESE)
          num3 = 6f;
        if ((double) this.itemNameLabels[index].Text.Length > (double) num3)
        {
          float t = ((float) this.itemNameLabels[index].Text.Length - num3) / num3;
          this.itemNameLabels[index].TextScale = Mathf.Lerp(2f, 1f, t) * num2;
          this.itemNameLabels[index].RelativePosition = this.itemNameLabels[index].RelativePosition.WithY(Mathf.Lerp(51f, 72f, t).Quantize(3f));
        }
        else
        {
          this.itemNameLabels[index].TextScale = 2f * num2;
          this.itemNameLabels[index].RelativePosition = this.itemNameLabels[index].RelativePosition.WithY(51f);
        }
      }
      this.itemNameLabels[0].PerformLayout();
      this.ShwoopOpen();
    }

    private void Update()
    {
      if (AmmonomiconController.Instance.IsOpen || this.m_curTrackable == null || this.m_isScalingDown)
        return;
      this.ShwoopClosed();
      GameManager.Instance.AcknowledgeKnownTrackable(this.m_curTrackable);
      this.m_curTrackable = (EncounterDatabaseEntry) null;
    }

    public void ShwoopOpen() => this.StartCoroutine(this.HandleShwoop(false));

    [DebuggerHidden]
    private IEnumerator HandleShwoop(bool reverse)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new AppearedInTheGungeonController__HandleShwoopc__Iterator0()
      {
        reverse = reverse,
        _this = this
      };
    }

    public void ShwoopClosed() => this.StartCoroutine(this.HandleShwoop(true));
  }

