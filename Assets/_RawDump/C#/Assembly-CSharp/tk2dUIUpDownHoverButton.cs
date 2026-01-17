// Decompiled with JetBrains decompiler
// Type: tk2dUIUpDownHoverButton
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
[AddComponentMenu("2D Toolkit/UI/tk2dUIUpDownHoverButton")]
public class tk2dUIUpDownHoverButton : tk2dUIBaseItemControl
{
  public GameObject upStateGO;
  public GameObject downStateGO;
  public GameObject hoverOverStateGO;
  [SerializeField]
  private bool useOnReleaseInsteadOfOnUp;
  private bool isDown;
  private bool isHover;
  public string SendMessageOnToggleOverMethodName = string.Empty;

  public bool UseOnReleaseInsteadOfOnUp => this.useOnReleaseInsteadOfOnUp;

  public bool IsOver
  {
    get => this.isDown || this.isHover;
    set
    {
      if (value == this.isDown && !this.isHover)
        return;
      if (value)
      {
        this.isHover = true;
        this.SetState();
        if (this.OnToggleOver != null)
          this.OnToggleOver(this);
      }
      else if (this.isDown && this.isHover)
      {
        this.isDown = false;
        this.isHover = false;
        this.SetState();
        if (this.OnToggleOver != null)
          this.OnToggleOver(this);
      }
      else if (this.isDown)
      {
        this.isDown = false;
        this.SetState();
        if (this.OnToggleOver != null)
          this.OnToggleOver(this);
      }
      else
      {
        this.isHover = false;
        this.SetState();
        if (this.OnToggleOver != null)
          this.OnToggleOver(this);
      }
      this.DoSendMessage(this.SendMessageOnToggleOverMethodName, (object) this);
    }
  }

  public event Action<tk2dUIUpDownHoverButton> OnToggleOver;

  private void Start() => this.SetState();

  private void OnEnable()
  {
    if (!(bool) (UnityEngine.Object) this.uiItem)
      return;
    this.uiItem.OnDown += new System.Action(this.ButtonDown);
    if (this.useOnReleaseInsteadOfOnUp)
      this.uiItem.OnRelease += new System.Action(this.ButtonUp);
    else
      this.uiItem.OnUp += new System.Action(this.ButtonUp);
    this.uiItem.OnHoverOver += new System.Action(this.ButtonHoverOver);
    this.uiItem.OnHoverOut += new System.Action(this.ButtonHoverOut);
  }

  private void OnDisable()
  {
    if (!(bool) (UnityEngine.Object) this.uiItem)
      return;
    this.uiItem.OnDown -= new System.Action(this.ButtonDown);
    if (this.useOnReleaseInsteadOfOnUp)
      this.uiItem.OnRelease -= new System.Action(this.ButtonUp);
    else
      this.uiItem.OnUp -= new System.Action(this.ButtonUp);
    this.uiItem.OnHoverOver -= new System.Action(this.ButtonHoverOver);
    this.uiItem.OnHoverOut -= new System.Action(this.ButtonHoverOut);
  }

  private void ButtonUp()
  {
    if (!this.isDown)
      return;
    this.isDown = false;
    this.SetState();
    if (this.isHover || this.OnToggleOver == null)
      return;
    this.OnToggleOver(this);
  }

  private void ButtonDown()
  {
    if (this.isDown)
      return;
    this.isDown = true;
    this.SetState();
    if (this.isHover || this.OnToggleOver == null)
      return;
    this.OnToggleOver(this);
  }

  private void ButtonHoverOver()
  {
    if (this.isHover)
      return;
    this.isHover = true;
    this.SetState();
    if (this.isDown || this.OnToggleOver == null)
      return;
    this.OnToggleOver(this);
  }

  private void ButtonHoverOut()
  {
    if (!this.isHover)
      return;
    this.isHover = false;
    this.SetState();
    if (this.isDown || this.OnToggleOver == null)
      return;
    this.OnToggleOver(this);
  }

  public void SetState()
  {
    tk2dUIBaseItemControl.ChangeGameObjectActiveStateWithNullCheck(this.upStateGO, !this.isDown && !this.isHover);
    if ((UnityEngine.Object) this.downStateGO == (UnityEngine.Object) this.hoverOverStateGO)
    {
      tk2dUIBaseItemControl.ChangeGameObjectActiveStateWithNullCheck(this.downStateGO, this.isDown || this.isHover);
    }
    else
    {
      tk2dUIBaseItemControl.ChangeGameObjectActiveStateWithNullCheck(this.downStateGO, this.isDown);
      tk2dUIBaseItemControl.ChangeGameObjectActiveStateWithNullCheck(this.hoverOverStateGO, this.isHover);
    }
  }

  public void InternalSetUseOnReleaseInsteadOfOnUp(bool state)
  {
    this.useOnReleaseInsteadOfOnUp = state;
  }
}
