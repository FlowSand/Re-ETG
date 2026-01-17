// Decompiled with JetBrains decompiler
// Type: PauseMenuController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.UI.HUD
{
    public class PauseMenuController : MonoBehaviour
    {
      public dfButton ExitToMainMenuButton;
      public dfButton ReturnToGameButton;
      public dfButton BestiaryButton;
      public dfButton QuickRestartButton;
      public dfButton QuitGameButton;
      public dfButton OptionsButton;
      public dfTextureSprite PauseBGSprite;
      public FullOptionsMenuController OptionsMenu;
      public List<GameObject> AdditionalMenuElementsToClear;
      public dfPanel metaCurrencyPanel;
      public AnimationCurve ShwoopInCurve;
      public AnimationCurve ShwoopOutCurve;
      public float DelayDFAnimatorsTime = 0.3f;
      private dfPanel m_panel;
      private bool m_buttonsOffsetForDoubleHeight;
      private const float c_FrenchVertOffsetUp = 18f;
      private const float c_FrenchVertOffsetDown = 24f;

      private void Start()
      {
        this.m_panel = this.GetComponent<dfPanel>();
        this.AdditionalMenuElementsToClear = new List<GameObject>();
        this.m_panel.IsVisibleChanged += new PropertyChangedEventHandler<bool>(this.OnVisibilityChanged);
        this.ExitToMainMenuButton.Click += new MouseEventHandler(this.DoExitToMainMenu);
        if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.FOYER)
        {
          this.ExitToMainMenuButton.Text = "#EXIT_COOP";
          if (GameManager.Instance.CurrentGameType == GameManager.GameType.SINGLE_PLAYER)
          {
            this.ExitToMainMenuButton.Disable();
            UIKeyControls component = this.ExitToMainMenuButton.GetComponent<UIKeyControls>();
            component.up.GetComponent<UIKeyControls>().down = component.down;
            component.down.GetComponent<UIKeyControls>().up = component.up;
          }
        }
        this.ReturnToGameButton.Click += new MouseEventHandler(this.DoReturnToGame);
        this.BestiaryButton.Click += new MouseEventHandler(this.DoShowBestiary);
        this.QuitGameButton.Click += new MouseEventHandler(this.DoQuitGameEntirely);
        if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.FOYER || GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.TUTORIAL || GameManager.Instance.CurrentGameMode == GameManager.GameMode.SUPERBOSSRUSH)
        {
          this.QuickRestartButton.Disable();
          UIKeyControls component = this.QuickRestartButton.GetComponent<UIKeyControls>();
          component.up.GetComponent<UIKeyControls>().down = component.down;
          component.down.GetComponent<UIKeyControls>().up = component.up;
        }
        else
          this.QuickRestartButton.Click += new MouseEventHandler(this.DoQuickRestart);
        this.OptionsButton.Click += new MouseEventHandler(this.DoShowOptions);
        if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.FOYER)
        {
          this.metaCurrencyPanel.IsVisible = true;
        }
        else
        {
          GameUIRoot.Instance.AddControlToMotionGroups((dfControl) this.metaCurrencyPanel, DungeonData.Direction.EAST, true);
          GameUIRoot.Instance.MoveNonCoreGroupOnscreen((dfControl) this.metaCurrencyPanel, true);
        }
      }

      private void OnVisibilityChanged(dfControl control, bool value)
      {
        if (!value)
          return;
        if (GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.FRENCH)
        {
          this.BestiaryButton.Text = this.BestiaryButton.ForceGetLocalizedValue("#AMMONOMICON");
          this.BestiaryButton.ModifyLocalizedText(this.BestiaryButton.Text.Replace(" ", "\n"));
          this.BestiaryButton.AutoSize = false;
          this.BestiaryButton.TextAlignment = TextAlignment.Center;
          this.BestiaryButton.Width = Mathf.Max(240f, this.BestiaryButton.Width);
          if (this.m_buttonsOffsetForDoubleHeight)
            return;
          this.m_buttonsOffsetForDoubleHeight = true;
          this.BestiaryButton.RelativePosition = this.BestiaryButton.RelativePosition - new Vector3(0.0f, 18f, 0.0f);
          this.ReturnToGameButton.RelativePosition = this.ReturnToGameButton.RelativePosition - new Vector3(0.0f, 18f, 0.0f);
          this.OptionsButton.RelativePosition = this.OptionsButton.RelativePosition + new Vector3(0.0f, 24f, 0.0f);
          this.ExitToMainMenuButton.RelativePosition = this.ExitToMainMenuButton.RelativePosition + new Vector3(0.0f, 24f, 0.0f);
          this.QuickRestartButton.RelativePosition = this.QuickRestartButton.RelativePosition + new Vector3(0.0f, 24f, 0.0f);
          this.QuitGameButton.RelativePosition = this.QuitGameButton.RelativePosition + new Vector3(0.0f, 24f, 0.0f);
        }
        else
        {
          if (!this.m_buttonsOffsetForDoubleHeight)
            return;
          this.BestiaryButton.Text = this.BestiaryButton.ForceGetLocalizedValue("#AMMONOMICON");
          this.BestiaryButton.AutoSize = true;
          this.BestiaryButton.TextAlignment = TextAlignment.Left;
          this.m_buttonsOffsetForDoubleHeight = false;
          this.BestiaryButton.RelativePosition = this.BestiaryButton.RelativePosition + new Vector3(0.0f, 18f, 0.0f);
          this.ReturnToGameButton.RelativePosition = this.ReturnToGameButton.RelativePosition + new Vector3(0.0f, 18f, 0.0f);
          this.OptionsButton.RelativePosition = this.OptionsButton.RelativePosition - new Vector3(0.0f, 24f, 0.0f);
          this.ExitToMainMenuButton.RelativePosition = this.ExitToMainMenuButton.RelativePosition - new Vector3(0.0f, 24f, 0.0f);
          this.QuickRestartButton.RelativePosition = this.QuickRestartButton.RelativePosition - new Vector3(0.0f, 24f, 0.0f);
          this.QuitGameButton.RelativePosition = this.QuitGameButton.RelativePosition - new Vector3(0.0f, 24f, 0.0f);
        }
      }

      private void RemoveQuitButtonAndRealignVertically()
      {
        this.QuitGameButton.Disable();
        Object.Destroy((Object) this.QuitGameButton.gameObject);
        this.ReturnToGameButton.RelativePosition = this.ReturnToGameButton.RelativePosition + new Vector3(0.0f, 9f, 0.0f);
        this.BestiaryButton.RelativePosition = this.BestiaryButton.RelativePosition + new Vector3(0.0f, 12f, 0.0f);
        this.OptionsButton.RelativePosition = this.OptionsButton.RelativePosition + new Vector3(0.0f, 15f, 0.0f);
        this.QuickRestartButton.RelativePosition = this.QuickRestartButton.RelativePosition + new Vector3(0.0f, 21f, 0.0f);
        this.ExitToMainMenuButton.RelativePosition = this.ExitToMainMenuButton.RelativePosition + new Vector3(0.0f, 24f, 0.0f);
      }

      public void ForceRevealMetaCurrencyPanel()
      {
        if (GameManager.Instance.CurrentLevelOverrideState != GameManager.LevelOverrideState.FOYER)
          return;
        GameUIRoot.Instance.MoveNonCoreGroupOnscreen((dfControl) this.metaCurrencyPanel, true);
      }

      public void ForceHideMetaCurrencyPanel()
      {
        if (GameManager.Instance.CurrentLevelOverrideState != GameManager.LevelOverrideState.FOYER)
          return;
        GameUIRoot.Instance.AddControlToMotionGroups((dfControl) this.metaCurrencyPanel, DungeonData.Direction.EAST, true);
        GameUIRoot.Instance.MoveNonCoreGroupOnscreen((dfControl) this.metaCurrencyPanel);
      }

      public void ToggleExitCoopButtonOnCoopChange()
      {
        if (GameManager.Instance.CurrentGameType == GameManager.GameType.SINGLE_PLAYER)
        {
          this.ExitToMainMenuButton.Disable();
          UIKeyControls component = this.ExitToMainMenuButton.GetComponent<UIKeyControls>();
          component.up.GetComponent<UIKeyControls>().down = component.down;
          component.down.GetComponent<UIKeyControls>().up = component.up;
        }
        else
        {
          this.ExitToMainMenuButton.Enable();
          UIKeyControls component = this.ExitToMainMenuButton.GetComponent<UIKeyControls>();
          if ((bool) (Object) component.up && !component.up.IsEnabled)
            component.up = component.up.GetComponent<UIKeyControls>().up;
          if ((bool) (Object) component.up)
            component.up.GetComponent<UIKeyControls>().down = (dfControl) this.ExitToMainMenuButton;
          if (!(bool) (Object) component.down)
            return;
          component.down.GetComponent<UIKeyControls>().up = (dfControl) this.ExitToMainMenuButton;
        }
      }

      public void ToggleVisibility(bool value)
      {
        if (value)
        {
          this.m_panel.IsVisible = value;
          this.PauseBGSprite.Parent.IsVisible = value;
        }
        else
        {
          this.m_panel.IsVisible = value;
          this.PauseBGSprite.Parent.IsVisible = value;
        }
      }

      private void HandleVisibilityChange(dfControl control, bool value)
      {
      }

      private void DoQuickRestart(dfControl control, dfMouseEventArgs mouseEvent)
      {
        this.StartCoroutine(this.HandleQuickRestart());
      }

      [DebuggerHidden]
      private IEnumerator HandleQuickRestart()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new PauseMenuController.<HandleQuickRestart>c__Iterator0()
        {
          $this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator HandleCloseGameEntirely()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new PauseMenuController.<HandleCloseGameEntirely>c__Iterator1()
        {
          $this = this
        };
      }

      public void ToggleBG(dfButton target)
      {
        if (StringTableManager.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.ENGLISH)
        {
          target.BackgroundSprite = string.Empty;
          target.Padding = new RectOffset(0, 0, 0, 0);
        }
        else
        {
          target.BackgroundSprite = "chamber_flash_small_001";
          target.Padding = new RectOffset(6, 6, 0, 0);
          target.NormalBackgroundColor = (Color32) Color.black;
          target.FocusBackgroundColor = (Color32) Color.black;
          target.HoverBackgroundColor = (Color32) Color.black;
          target.DisabledColor = (Color32) Color.black;
          target.PressedBackgroundColor = (Color32) Color.black;
        }
      }

      public void HandleBGs()
      {
        this.ToggleBG(this.OptionsButton);
        this.ToggleBG(this.QuickRestartButton);
        this.ToggleBG(this.ReturnToGameButton);
        this.ToggleBG(this.BestiaryButton);
        this.ToggleBG(this.ExitToMainMenuButton);
        if (!(bool) (Object) this.QuitGameButton)
          return;
        this.ToggleBG(this.QuitGameButton);
      }

      public void ShwoopOpen()
      {
        float num = !PunchoutController.IsActive || !PunchoutController.OverrideControlsButton ? 1f : 4f;
        this.HandleBGs();
        if (GameManager.Instance.CurrentLevelOverrideState != GameManager.LevelOverrideState.FOYER)
        {
          this.metaCurrencyPanel.IsVisible = true;
          if ((bool) (Object) this.metaCurrencyPanel && (bool) (Object) this.metaCurrencyPanel.Parent && (bool) (Object) this.metaCurrencyPanel.Parent.Parent)
            this.metaCurrencyPanel.Parent.Parent.BringToFront();
          GameUIRoot.Instance.MoveNonCoreGroupOnscreen((dfControl) this.metaCurrencyPanel);
        }
        this.ForceMaterialInvisibility();
        this.StartCoroutine(this.DelayTriggerAnimators(num));
        this.StartCoroutine(this.HandleBlockReveal(false, num));
        this.StartCoroutine(this.HandleShwoop(false, num));
        if (this.m_panel.ZOrder >= this.PauseBGSprite.Parent.ZOrder)
          return;
        this.m_panel.ZOrder = this.PauseBGSprite.Parent.ZOrder + 1;
      }

      private Material GrabBGRenderMaterial()
      {
        Material material = this.PauseBGSprite.RenderMaterial;
        for (int index = 0; index < this.PauseBGSprite.GUIManager.MeshRenderer.sharedMaterials.Length; ++index)
        {
          Material sharedMaterial = this.PauseBGSprite.GUIManager.MeshRenderer.sharedMaterials[index];
          if ((Object) sharedMaterial != (Object) null && (Object) sharedMaterial.shader != (Object) null && sharedMaterial.shader.name.Contains("MaskReveal"))
          {
            material = sharedMaterial;
            break;
          }
        }
        return material;
      }

      [DebuggerHidden]
      private IEnumerator DelayTriggerAnimators(float timeMultiplier = 1f)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new PauseMenuController.<DelayTriggerAnimators>c__Iterator2()
        {
          timeMultiplier = timeMultiplier,
          $this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator HandleBlockReveal(bool reverse, float timeMultiplier = 1f)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new PauseMenuController.<HandleBlockReveal>c__Iterator3()
        {
          reverse = reverse,
          timeMultiplier = timeMultiplier,
          $this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator HandleShwoop(bool reverse, float timeMultlier = 1f)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new PauseMenuController.<HandleShwoop>c__Iterator4()
        {
          reverse = reverse,
          timeMultlier = timeMultlier,
          $this = this
        };
      }

      public void ShwoopClosed()
      {
        this.StartCoroutine(this.HandleShwoop(true));
        this.StartCoroutine(this.HandleBlockReveal(true));
        if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.FOYER)
          return;
        GameUIRoot.Instance.MoveNonCoreGroupOnscreen((dfControl) this.metaCurrencyPanel, true);
      }

      private void DoQuitGameEntirely(dfControl control, dfMouseEventArgs mouseEvent)
      {
        this.StartCoroutine(this.HandleCloseGameEntirely());
      }

      public void ForceMaterialInvisibility()
      {
        Material material = this.GrabBGRenderMaterial();
        if ((Object) this.PauseBGSprite.Material != (Object) null)
          this.PauseBGSprite.Material.SetFloat("_RevealPercent", 0.0f);
        if ((Object) this.PauseBGSprite.RenderMaterial != (Object) null)
          this.PauseBGSprite.RenderMaterial.SetFloat("_RevealPercent", 0.0f);
        if (!((Object) material != (Object) null))
          return;
        material.SetFloat("_RevealPercent", 0.0f);
      }

      public void ForceMaterialVisibility()
      {
        Material material = this.GrabBGRenderMaterial();
        if ((Object) this.PauseBGSprite.Material != (Object) null)
          this.PauseBGSprite.Material.SetFloat("_RevealPercent", 1f);
        if ((Object) this.PauseBGSprite.RenderMaterial != (Object) null)
          this.PauseBGSprite.RenderMaterial.SetFloat("_RevealPercent", 1f);
        if (!((Object) material != (Object) null))
          return;
        material.SetFloat("_RevealPercent", 1f);
      }

      public void DoShowBestiaryToTarget(EncounterTrackable target)
      {
        this.ToggleVisibility(false);
        if ((Object) dfGUIManager.GetModalControl() != (Object) null)
          dfGUIManager.PopModal();
        AmmonomiconController.Instance.OpenAmmonomiconToTrackable(target);
      }

      public void DoShowBestiary(dfControl control, dfMouseEventArgs mouseEvent)
      {
        if (AmmonomiconController.Instance.IsClosing || AmmonomiconController.Instance.IsOpening)
          return;
        this.ToggleVisibility(false);
        if ((Object) dfGUIManager.GetModalControl() != (Object) null)
          dfGUIManager.PopModal();
        AmmonomiconController.Instance.OpenAmmonomicon(false, false);
      }

      private void DoReturnToGame(dfControl control, dfMouseEventArgs mouseEvent)
      {
        if (GameManager.Instance.IsLoadingLevel)
          return;
        GameManager.Instance.Unpause();
      }

      private void DoShowOptions(dfControl control, dfMouseEventArgs mouseEvent)
      {
        if (GameManager.Instance.IsLoadingLevel)
          return;
        this.ToggleVisibility(false);
        this.OptionsMenu.PreOptionsMenu.IsVisible = true;
      }

      private void DoExitToMainMenu(dfControl control, dfMouseEventArgs mouseEvent)
      {
        this.StartCoroutine(this.HandleExitToMainMenu());
      }

      [DebuggerHidden]
      private IEnumerator HandleExitToMainMenu()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new PauseMenuController.<HandleExitToMainMenu>c__Iterator5()
        {
          $this = this
        };
      }

      public void SetDefaultFocus() => this.ReturnToGameButton.Focus(true);

      public void RevertToBaseState()
      {
        this.HandleBGs();
        this.ToggleVisibility(true);
        if (GameManager.Instance.CurrentLevelOverrideState != GameManager.LevelOverrideState.FOYER)
          this.metaCurrencyPanel.IsVisible = this.m_panel.IsVisible;
        this.m_panel.IsInteractive = true;
        this.m_panel.IsEnabled = true;
        this.OptionsMenu.IsVisible = false;
        this.OptionsMenu.PreOptionsMenu.IsVisible = false;
        for (int index = 0; index < this.AdditionalMenuElementsToClear.Count; ++index)
          Object.Destroy((Object) this.AdditionalMenuElementsToClear[index]);
        this.AdditionalMenuElementsToClear.Clear();
        dfGUIManager.PopModalToControl((dfControl) this.m_panel, false);
        this.ForceMaterialVisibility();
        this.SetDefaultFocus();
        int num = (int) AkSoundEngine.PostEvent("Play_UI_menu_back_01", this.gameObject);
      }
    }

}
