// Decompiled with JetBrains decompiler
// Type: PreOptionsMenuController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.UI.HUD
{
    public class PreOptionsMenuController : MonoBehaviour
    {
      public dfButton TabAudioSelector;
      public dfButton TabVideoSelector;
      public dfButton TabGameplaySelector;
      public dfButton TabControlsSelector;
      public dfLabel HeaderLabel;
      public FullOptionsMenuController FullOptionsMenu;
      private dfPanel m_panel;
      private float m_timeOpen;

      public bool IsVisible
      {
        get => this.m_panel.IsVisible;
        set
        {
          if (this.m_panel.IsVisible == value)
            return;
          if (value)
          {
            this.m_panel.IsVisible = value;
            this.ShwoopOpen();
            this.ShowPreOptionsMenu();
          }
          else
          {
            this.m_timeOpen = 0.0f;
            this.ShwoopClosed();
            if ((Object) dfGUIManager.GetModalControl() == (Object) this.m_panel)
              dfGUIManager.PopModal();
            else
              UnityEngine.Debug.LogError((object) "failure.");
          }
        }
      }

      public void MakeVisibleWithoutAnim()
      {
        if (this.m_panel.IsVisible)
          return;
        this.m_panel.IsVisible = true;
        if (!this.HeaderLabel.Text.StartsWith("#"))
          this.HeaderLabel.ModifyLocalizedText(this.HeaderLabel.Text.ToUpperInvariant());
        this.m_panel.Opacity = 1f;
        this.m_panel.transform.localScale = Vector3.one;
        this.m_panel.MakePixelPerfect();
        this.ShowPreOptionsMenu();
      }

      private void ShowPreOptionsMenu()
      {
        dfGUIManager.PushModal((dfControl) this.m_panel);
        this.TabGameplaySelector.Focus(true);
      }

      public void ReturnToPreOptionsMenu()
      {
        this.FullOptionsMenu.IsVisible = false;
        this.IsVisible = true;
        this.TabGameplaySelector.Focus(true);
        int num = (int) AkSoundEngine.PostEvent("Play_UI_menu_back_01", this.gameObject);
        dfGUIManager.PopModalToControl((dfControl) this.m_panel, false);
      }

      public void ToggleToPanel(dfScrollPanel targetPanel, bool val, bool force = false)
      {
        if (!force && (double) this.m_timeOpen < 0.20000000298023224)
          return;
        this.FullOptionsMenu.ToggleToPanel(targetPanel, val);
        this.m_panel.IsVisible = false;
      }

      private void Awake()
      {
        this.m_panel = this.GetComponent<dfPanel>();
        this.TabAudioSelector.Click += (MouseEventHandler) ((control, mouseEvent) => this.ToggleToPanel(this.FullOptionsMenu.TabAudio, false));
        this.TabVideoSelector.Click += (MouseEventHandler) ((control, mouseEvent) => this.ToggleToPanel(this.FullOptionsMenu.TabVideo, false));
        this.TabGameplaySelector.Click += (MouseEventHandler) ((control, mouseEvent) => this.ToggleToPanel(this.FullOptionsMenu.TabGameplay, false));
        this.TabControlsSelector.Click += (MouseEventHandler) ((control, mouseEvent) => this.ToggleToPanel(this.FullOptionsMenu.TabControls, false));
      }

      private void Update()
      {
        if (this.IsVisible)
          this.m_timeOpen += GameManager.INVARIANT_DELTA_TIME;
        else
          this.m_timeOpen = 0.0f;
      }

      public void ShwoopOpen()
      {
        if (!this.HeaderLabel.Text.StartsWith("#"))
          this.HeaderLabel.ModifyLocalizedText(this.HeaderLabel.Text.ToUpperInvariant());
        this.StartCoroutine(this.HandleShwoop(false));
      }

      [DebuggerHidden]
      private IEnumerator HandleShwoop(bool reverse)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new PreOptionsMenuController__HandleShwoopc__Iterator0()
        {
          reverse = reverse,
          _this = this
        };
      }

      public void ShwoopClosed() => this.StartCoroutine(this.HandleShwoop(true));
    }

}
