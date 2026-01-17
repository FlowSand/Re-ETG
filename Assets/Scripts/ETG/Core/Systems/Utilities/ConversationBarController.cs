// Decompiled with JetBrains decompiler
// Type: ConversationBarController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class ConversationBarController : MonoBehaviour
    {
      public Color selectedColor = Color.white;
      public Color unselectedColor = Color.gray;
      public dfButton textOption1;
      public dfButton textOption2;
      public dfSprite reticleLeft1;
      public dfSprite reticleRight1;
      public dfSprite reticleLeft2;
      public dfSprite reticleRight2;
      public dfSprite portraitSprite;
      public Texture2D EeveeTex;
      private dfSprite m_conversationBarSprite;
      private bool m_isActive;
      private PlayerController m_lastAssignedPlayer;
      private bool m_temporarilyHidden;
      private dfPanel m_motionGroup;
      private bool m_portraitAdjustedForSmallUI;

      public bool IsActive => this.m_isActive;

      public void HideBar()
      {
        this.m_isActive = false;
        if ((Object) this.m_conversationBarSprite == (Object) null)
          this.m_conversationBarSprite = this.GetComponent<dfSprite>();
        if ((Object) this.m_motionGroup != (Object) null)
          GameUIRoot.Instance.MoveNonCoreGroupOnscreen((dfControl) this.m_conversationBarSprite, true);
        this.StartCoroutine(this.DelayedHide());
      }

      [DebuggerHidden]
      private IEnumerator DelayedHide()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ConversationBarController.<DelayedHide>c__Iterator0()
        {
          _this = this
        };
      }

      public void SetSelectedResponse(int selectedResponse)
      {
        if (selectedResponse == 0)
        {
          this.textOption1.TextColor = (Color32) this.selectedColor;
          this.textOption2.TextColor = (Color32) this.unselectedColor;
          this.reticleLeft1.IsVisible = false;
          this.reticleRight1.IsVisible = false;
          this.reticleLeft2.IsVisible = false;
          this.reticleRight2.IsVisible = false;
        }
        else
        {
          if (selectedResponse != 1)
            return;
          this.textOption1.TextColor = (Color32) this.unselectedColor;
          this.textOption2.TextColor = (Color32) this.selectedColor;
          this.reticleLeft1.IsVisible = false;
          this.reticleRight1.IsVisible = false;
          this.reticleLeft2.IsVisible = false;
          this.reticleRight2.IsVisible = false;
        }
      }

      public void LateUpdate()
      {
        if (this.m_temporarilyHidden && !GameManager.Instance.IsPaused)
        {
          GameUIRoot.Instance.MoveNonCoreGroupOnscreen((dfControl) this.m_conversationBarSprite);
          this.m_temporarilyHidden = false;
        }
        if (!this.textOption1.IsVisible)
          return;
        if (!this.m_temporarilyHidden && GameManager.Instance.IsPaused)
        {
          GameUIRoot.Instance.MoveNonCoreGroupOnscreen((dfControl) this.m_conversationBarSprite, true);
          this.m_temporarilyHidden = true;
        }
        else
        {
          if (!BraveInput.GetInstanceForPlayer(this.m_lastAssignedPlayer.PlayerIDX).IsKeyboardAndMouse())
            return;
          Vector2 point = Input.mousePosition.WithY((float) Screen.height - Input.mousePosition.y).XY();
          if (this.textOption1.IsVisible && this.textOption1.GetScreenRect().Contains(point))
          {
            this.HandleOptionHover((dfControl) this.textOption1);
            if (Input.GetMouseButtonDown(0))
            {
              this.HandleOptionClick((dfControl) this.textOption1);
              this.m_lastAssignedPlayer.SuppressThisClick = true;
            }
          }
          if (!this.textOption2.IsVisible || !this.textOption2.GetScreenRect().Contains(point))
            return;
          this.HandleOptionHover((dfControl) this.textOption2);
          if (!Input.GetMouseButtonDown(0))
            return;
          this.HandleOptionClick((dfControl) this.textOption2);
          this.m_lastAssignedPlayer.SuppressThisClick = true;
        }
      }

      private void UpdateScaleAndPosition(dfControl c, float newScalar, bool doVerticalAdjustment = true)
      {
        if ((double) c.transform.localScale.x == (double) newScalar)
          return;
        float x = c.transform.localScale.x;
        c.transform.localScale = new Vector3(newScalar, newScalar, 1f);
        c.RelativePosition = new Vector3(c.RelativePosition.x * (newScalar / x), !doVerticalAdjustment ? c.RelativePosition.y : c.RelativePosition.y + ((double) x >= (double) newScalar ? -c.Height : c.Height), c.RelativePosition.z);
      }

      public void ShowBar(PlayerController interactingPlayer, string[] responses)
      {
        GameUIRoot.Instance.notificationController.ForceHide();
        this.UpdateScaleAndPosition((dfControl) this.reticleLeft1, 1f / GameUIRoot.GameUIScalar);
        this.UpdateScaleAndPosition((dfControl) this.reticleLeft2, 1f / GameUIRoot.GameUIScalar);
        this.UpdateScaleAndPosition((dfControl) this.portraitSprite, 1f / GameUIRoot.GameUIScalar);
        bool flag = false;
        if (!(bool) (Object) this.m_conversationBarSprite)
        {
          flag = true;
          this.m_conversationBarSprite = this.GetComponent<dfSprite>();
          this.m_motionGroup = GameUIRoot.Instance.AddControlToMotionGroups((dfControl) this.m_conversationBarSprite, DungeonData.Direction.SOUTH, true);
        }
        if ((double) this.m_conversationBarSprite.Parent.transform.localScale.x != 1.0 / (double) GameUIRoot.GameUIScalar)
        {
          this.m_conversationBarSprite.Parent.transform.localScale = new Vector3(1f / GameUIRoot.GameUIScalar, 1f / GameUIRoot.GameUIScalar, 1f);
          if (flag)
            this.m_conversationBarSprite.Parent.RelativePosition = this.m_conversationBarSprite.Parent.RelativePosition.WithY(this.m_conversationBarSprite.Parent.Height * 3f);
        }
        if (interactingPlayer.characterIdentity == PlayableCharacters.Eevee)
        {
          Material material = Object.Instantiate<Material>(this.portraitSprite.Atlas.Material);
          material.shader = Shader.Find("Brave/Internal/GlitchEevee");
          material.SetTexture("_EeveeTex", (Texture) this.EeveeTex);
          material.SetFloat("_WaveIntensity", 0.1f);
          material.SetFloat("_ColorIntensity", 0.015f);
          this.portraitSprite.OverrideMaterial = material;
        }
        else
          this.portraitSprite.OverrideMaterial = (Material) null;
        this.m_isActive = true;
        this.m_lastAssignedPlayer = interactingPlayer;
        GameUIRoot.Instance.MoveNonCoreGroupOnscreen((dfControl) this.m_conversationBarSprite);
        this.m_conversationBarSprite.BringToFront();
        if (interactingPlayer.characterIdentity == PlayableCharacters.Eevee)
        {
          switch (Random.Range(0, 4))
          {
            case 0:
              this.portraitSprite.SpriteName = "talking_bar_character_window_rogue_003";
              break;
            case 1:
              this.portraitSprite.SpriteName = "talking_bar_character_window_marine_003";
              break;
            case 2:
              this.portraitSprite.SpriteName = "talking_bar_character_window_guide_003";
              break;
            case 3:
              this.portraitSprite.SpriteName = "talking_bar_character_window_convict_003";
              break;
            default:
              this.portraitSprite.SpriteName = "talking_bar_character_window_guide_003";
              break;
          }
        }
        else
          this.portraitSprite.SpriteName = interactingPlayer.uiPortraitName;
        if (GameManager.Options.SmallUIEnabled)
        {
          if (!this.m_portraitAdjustedForSmallUI)
          {
            this.portraitSprite.Size = this.portraitSprite.Size / 2f;
            this.portraitSprite.RelativePosition = this.portraitSprite.RelativePosition - new Vector3(0.0f, this.portraitSprite.Size.y * 2f, 0.0f);
            this.m_portraitAdjustedForSmallUI = true;
          }
        }
        else if (this.m_portraitAdjustedForSmallUI)
        {
          this.portraitSprite.RelativePosition = this.portraitSprite.RelativePosition + new Vector3(0.0f, this.portraitSprite.Size.y * 2f, 0.0f);
          this.portraitSprite.Size = this.portraitSprite.Size * 2f;
          this.m_portraitAdjustedForSmallUI = false;
        }
        this.m_conversationBarSprite.IsVisible = true;
        this.textOption1.IsVisible = true;
        this.textOption1.Text = responses[0];
        this.reticleRight1.RelativePosition = this.reticleLeft1.RelativePosition.WithX((float) ((double) this.reticleLeft1.RelativePosition.x + (double) this.reticleLeft1.Width + (double) this.textOption1.GetAutosizeWidth() + 24.0));
        if (responses != null && responses.Length > 1)
        {
          this.textOption2.IsVisible = true;
          this.textOption2.Text = responses[1];
          this.reticleRight2.RelativePosition = this.reticleLeft2.RelativePosition.WithX((float) ((double) this.reticleLeft2.RelativePosition.x + (double) this.reticleLeft2.Width + (double) this.textOption2.GetAutosizeWidth() + 24.0));
        }
        else
        {
          this.textOption2.IsVisible = false;
          this.textOption2.Text = string.Empty;
        }
      }

      private void HandleOptionHover(dfControl control)
      {
        if ((Object) control == (Object) this.textOption1)
          GameUIRoot.Instance.SetConversationResponse(0);
        if (!((Object) control == (Object) this.textOption2))
          return;
        GameUIRoot.Instance.SetConversationResponse(1);
      }

      private void HandleOptionClick(dfControl control)
      {
        if ((Object) control == (Object) this.textOption1)
          GameUIRoot.Instance.SetConversationResponse(0);
        if ((Object) control == (Object) this.textOption2)
          GameUIRoot.Instance.SetConversationResponse(1);
        GameUIRoot.Instance.SelectConversationResponse();
      }
    }

}
