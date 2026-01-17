// Decompiled with JetBrains decompiler
// Type: UIKeyControls
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using InControl;
using System;
using UnityEngine;

#nullable disable
public class UIKeyControls : MonoBehaviour
{
  public dfControl up;
  public dfControl down;
  public dfControl left;
  public dfControl right;
  public bool selectOnAction;
  public bool clearRepeatingOnSelect;
  private dfControl selfControl;
  public System.Action OnUpDown;
  public System.Action OnDownDown;
  public System.Action OnLeftDown;
  public System.Action OnRightDown;
  public Action<dfControl> OnNewControlSelected;
  private static bool m_hasCheckedThisFrame;
  private static UIKeyControls m_lastFocusedUIKeyControl;
  private static float m_timer;
  private const float TIMER_THRESHOLD = 1.5f;
  private dfButton button;

  public void Awake()
  {
    this.button = this.GetComponent<dfButton>();
    this.selfControl = this.GetComponent<dfControl>();
    if (!this.clearRepeatingOnSelect)
      return;
    this.button.GotFocus += new FocusEventHandler(this.GotFocus);
  }

  private static void CheckForControllerFails()
  {
    if ((UnityEngine.Object) BraveInput.PrimaryPlayerInstance != (UnityEngine.Object) null && BraveInput.PrimaryPlayerInstance.ActiveActions != null && BraveInput.PrimaryPlayerInstance.ActiveActions.LastInputType != BindingSourceType.MouseBindingSource)
    {
      if ((UnityEngine.Object) UIKeyControls.m_lastFocusedUIKeyControl != (UnityEngine.Object) null && (UnityEngine.Object) dfGUIManager.ActiveControl != (UnityEngine.Object) null && (UnityEngine.Object) dfGUIManager.ActiveControl.GetComponent<UIKeyControls>() == (UnityEngine.Object) null && (UnityEngine.Object) dfGUIManager.ActiveControl.GetComponent<BraveOptionsMenuItem>() == (UnityEngine.Object) null)
      {
        UIKeyControls.m_timer += GameManager.INVARIANT_DELTA_TIME;
        if ((double) UIKeyControls.m_timer > 1.5)
          UIKeyControls.m_lastFocusedUIKeyControl.selfControl.Focus(true);
      }
      else
        UIKeyControls.m_timer = 0.0f;
    }
    else
      UIKeyControls.m_timer = 0.0f;
    UIKeyControls.m_hasCheckedThisFrame = true;
  }

  public void Update()
  {
    if (this.selfControl.HasFocus)
      UIKeyControls.m_lastFocusedUIKeyControl = this;
    if (UIKeyControls.m_hasCheckedThisFrame)
      return;
    UIKeyControls.CheckForControllerFails();
  }

  public void LateUpdate() => UIKeyControls.m_hasCheckedThisFrame = false;

  public void OnKeyDown(dfControl sender, dfKeyEventArgs args)
  {
    if (args.Used)
      return;
    if (args.KeyCode == KeyCode.UpArrow)
    {
      if ((bool) (UnityEngine.Object) this.up)
      {
        if (this.OnNewControlSelected != null)
          this.OnNewControlSelected(this.up);
        this.up.Focus(true);
      }
      if (this.OnUpDown != null)
        this.OnUpDown();
    }
    else if (args.KeyCode == KeyCode.DownArrow)
    {
      if ((bool) (UnityEngine.Object) this.down)
      {
        if (this.OnNewControlSelected != null)
          this.OnNewControlSelected(this.down);
        this.down.Focus(true);
      }
      if (this.OnDownDown != null)
        this.OnDownDown();
    }
    else if (args.KeyCode == KeyCode.LeftArrow)
    {
      if ((bool) (UnityEngine.Object) this.left)
      {
        if (this.OnNewControlSelected != null)
          this.OnNewControlSelected(this.left);
        this.left.Focus(true);
      }
      if (this.OnLeftDown != null)
        this.OnLeftDown();
    }
    else if (args.KeyCode == KeyCode.RightArrow)
    {
      if ((bool) (UnityEngine.Object) this.right)
      {
        if (this.OnNewControlSelected != null)
          this.OnNewControlSelected(this.right);
        this.right.Focus(true);
      }
      if (this.OnRightDown != null)
        this.OnRightDown();
    }
    if (!this.selectOnAction || !(bool) (UnityEngine.Object) this.button || args.KeyCode != KeyCode.Return)
      return;
    int num = (int) AkSoundEngine.PostEvent("Play_UI_menu_confirm_01", this.gameObject);
    this.button.DoClick();
  }

  private void GotFocus(dfControl control, dfFocusEventArgs args)
  {
    if (!this.clearRepeatingOnSelect)
      return;
    if ((UnityEngine.Object) BraveInput.PrimaryPlayerInstance != (UnityEngine.Object) null)
    {
      GungeonActions activeActions = BraveInput.PrimaryPlayerInstance.ActiveActions;
      activeActions.SelectUp.ResetRepeating();
      activeActions.SelectDown.ResetRepeating();
      activeActions.SelectLeft.ResetRepeating();
      activeActions.SelectRight.ResetRepeating();
    }
    if (!((UnityEngine.Object) BraveInput.SecondaryPlayerInstance != (UnityEngine.Object) null))
      return;
    GungeonActions activeActions1 = BraveInput.SecondaryPlayerInstance.ActiveActions;
    activeActions1.SelectUp.ResetRepeating();
    activeActions1.SelectDown.ResetRepeating();
    activeActions1.SelectLeft.ResetRepeating();
    activeActions1.SelectRight.ResetRepeating();
  }
}
