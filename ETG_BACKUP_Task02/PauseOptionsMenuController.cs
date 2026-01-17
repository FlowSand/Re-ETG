// Decompiled with JetBrains decompiler
// Type: PauseOptionsMenuController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class PauseOptionsMenuController : MonoBehaviour
{
  public dfProgressBar MusicVolumeSlider;
  public dfProgressBar SoundVolumeSlider;
  public dfProgressBar UIVolumeSlider;
  public dfButton HeadphonesButton;
  public dfButton SpeakersButton;
  public dfButton AcceptButton;
  protected dfPanel m_panel;

  public bool IsVisible
  {
    get => this.m_panel.IsVisible;
    set
    {
      if (this.m_panel.IsVisible == value)
        return;
      this.InitializeFromOptions();
      this.m_panel.IsVisible = value;
      if (value)
        dfGUIManager.PushModal((dfControl) this.m_panel);
      else if ((Object) dfGUIManager.GetModalControl() == (Object) this.m_panel)
        dfGUIManager.PopModal();
      else
        Debug.LogError((object) "failure.");
    }
  }

  public void InitializeFromOptions()
  {
    Debug.Log((object) "initializing...");
    this.MusicVolumeSlider.Value = GameManager.Options.MusicVolume;
    this.SoundVolumeSlider.Value = GameManager.Options.SoundVolume;
    if ((Object) this.UIVolumeSlider != (Object) null)
      this.UIVolumeSlider.Value = GameManager.Options.UIVolume;
    switch (GameManager.Options.AudioHardware)
    {
      case GameOptions.AudioHardwareMode.SPEAKERS:
        this.SpeakersButton.ForceState(dfButton.ButtonState.Pressed);
        break;
      case GameOptions.AudioHardwareMode.HEADPHONES:
        this.HeadphonesButton.ForceState(dfButton.ButtonState.Pressed);
        break;
    }
  }

  private void Start()
  {
    this.m_panel = this.GetComponent<dfPanel>();
    this.InitializeFromOptions();
    this.MusicVolumeSlider.ValueChanged += (PropertyChangedEventHandler<float>) ((control, value) => GameManager.Options.MusicVolume = value);
    this.SoundVolumeSlider.ValueChanged += (PropertyChangedEventHandler<float>) ((control, value) => GameManager.Options.SoundVolume = value);
    if ((Object) this.UIVolumeSlider != (Object) null)
      this.UIVolumeSlider.ValueChanged += (PropertyChangedEventHandler<float>) ((control, value) => GameManager.Options.UIVolume = value);
    this.HeadphonesButton.Click += (MouseEventHandler) ((control, mouseEvent) =>
    {
      int num = (int) AkSoundEngine.PostEvent("Play_UI_menu_confirm_01", this.gameObject);
      this.HeadphonesButton.ForceState(dfButton.ButtonState.Pressed);
      this.SpeakersButton.ForceState(dfButton.ButtonState.Default);
      GameManager.Options.AudioHardware = GameOptions.AudioHardwareMode.HEADPHONES;
    });
    this.SpeakersButton.Click += (MouseEventHandler) ((control, mouseEvent) =>
    {
      int num = (int) AkSoundEngine.PostEvent("Play_UI_menu_confirm_01", this.gameObject);
      this.HeadphonesButton.ForceState(dfButton.ButtonState.Default);
      this.SpeakersButton.ForceState(dfButton.ButtonState.Pressed);
      GameManager.Options.AudioHardware = GameOptions.AudioHardwareMode.SPEAKERS;
    });
    this.AcceptButton.Click += (MouseEventHandler) ((control, mouseEvent) =>
    {
      this.IsVisible = false;
      GameUIRoot.Instance.ShowPauseMenu();
    });
  }
}
