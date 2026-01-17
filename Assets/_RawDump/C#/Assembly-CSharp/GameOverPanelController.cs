// Decompiled with JetBrains decompiler
// Type: GameOverPanelController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class GameOverPanelController : TimeInvariantMonoBehaviour
{
  public dfButton QuickRestartButton;
  public dfButton MainMenuButton;
  public tk2dSprite deathGuyLeft;
  public tk2dSprite deathGuyRight;
  private dfPanel m_panel;

  private void Start()
  {
    this.m_panel = this.GetComponent<dfPanel>();
    this.QuickRestartButton.Click += new MouseEventHandler(this.DoQuickRestart);
    this.MainMenuButton.Click += new MouseEventHandler(this.DoMainMenu);
  }

  private void DoMainMenu(dfControl control, dfMouseEventArgs mouseEvent)
  {
    if (!this.m_panel.IsVisible)
      return;
    this.m_panel.IsVisible = false;
    dfGUIManager.PopModal();
    Pixelator.Instance.DoFinalNonFadedLayer = false;
    GameUIRoot.Instance.ToggleUICamera(false);
    Pixelator.Instance.FadeToBlack(0.15f);
    GameManager.Instance.DelayedLoadMainMenu(0.15f);
    int num = (int) AkSoundEngine.PostEvent("Play_UI_menu_cancel_01", this.gameObject);
  }

  private void DoQuickRestart(dfControl control, dfMouseEventArgs mouseEvent)
  {
    if (!this.m_panel.IsVisible)
      return;
    this.m_panel.IsVisible = false;
    dfGUIManager.PopModal();
    Pixelator.Instance.DoFinalNonFadedLayer = false;
    GameUIRoot.Instance.ToggleUICamera(false);
    Pixelator.Instance.FadeToBlack(0.15f);
    GameManager.Instance.DelayedQuickRestart(0.15f);
    int num = (int) AkSoundEngine.PostEvent("Play_UI_menu_characterselect_01", this.gameObject);
  }

  public void Activate()
  {
    this.QuickRestartButton.Focus(true);
    this.deathGuyLeft.ignoresTiltworldDepth = true;
    this.deathGuyRight.ignoresTiltworldDepth = true;
    this.UpdateDeathGuys();
  }

  protected void UpdateDeathGuys()
  {
    this.deathGuyLeft.scale = GameUIUtility.GetCurrentTK2D_DFScale(this.m_panel.GetManager()) * Vector3.one;
    this.deathGuyRight.scale = this.deathGuyLeft.scale.WithX(this.deathGuyLeft.scale.x * -1f);
    Vector3 vector3 = (this.m_panel.Size.ToVector3ZUp() + new Vector3(36f, 0.0f, 0.0f)) * this.m_panel.PixelsToUnits() * 0.5f;
    this.deathGuyLeft.transform.position = this.m_panel.transform.position - vector3 + new Vector3(-this.deathGuyLeft.GetBounds().size.x, 0.0f, 0.0f);
    this.deathGuyRight.transform.position = this.m_panel.transform.position + new Vector3(vector3.x, -vector3.y, vector3.z) + new Vector3(this.deathGuyRight.GetBounds().size.x, 0.0f, 0.0f);
    this.deathGuyLeft.renderer.enabled = true;
    this.deathGuyRight.renderer.enabled = true;
  }

  protected override void InvariantUpdate(float realDeltaTime)
  {
    if (!this.m_panel.IsVisible)
      return;
    this.UpdateDeathGuys();
    if (this.deathGuyLeft.renderer.enabled)
      this.deathGuyLeft.spriteAnimator.UpdateAnimation(realDeltaTime);
    if (this.deathGuyRight.renderer.enabled)
      this.deathGuyRight.spriteAnimator.UpdateAnimation(realDeltaTime);
    GameUIRoot.Instance.ForceClearReload();
  }

  public void Deactivate()
  {
    this.deathGuyRight.renderer.enabled = false;
    this.deathGuyLeft.renderer.enabled = false;
  }

  protected override void OnDestroy() => base.OnDestroy();
}
