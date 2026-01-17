// Decompiled with JetBrains decompiler
// Type: tk2dUIItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
[AddComponentMenu("2D Toolkit/UI/Core/tk2dUIItem")]
public class tk2dUIItem : MonoBehaviour
{
  public GameObject sendMessageTarget;
  public string SendMessageOnDownMethodName = string.Empty;
  public string SendMessageOnUpMethodName = string.Empty;
  public string SendMessageOnClickMethodName = string.Empty;
  public string SendMessageOnReleaseMethodName = string.Empty;
  [SerializeField]
  private bool isChildOfAnotherUIItem;
  public bool registerPressFromChildren;
  public bool isHoverEnabled;
  public Transform[] editorExtraBounds = new Transform[0];
  public Transform[] editorIgnoreBounds = new Transform[0];
  private bool isPressed;
  private bool isHoverOver;
  private tk2dUITouch touch;
  private tk2dUIItem parentUIItem;

  public event System.Action OnDown;

  public event System.Action OnUp;

  public event System.Action OnClick;

  public event System.Action OnRelease;

  public event System.Action OnHoverOver;

  public event System.Action OnHoverOut;

  public event Action<tk2dUIItem> OnDownUIItem;

  public event Action<tk2dUIItem> OnUpUIItem;

  public event Action<tk2dUIItem> OnClickUIItem;

  public event Action<tk2dUIItem> OnReleaseUIItem;

  public event Action<tk2dUIItem> OnHoverOverUIItem;

  public event Action<tk2dUIItem> OnHoverOutUIItem;

  private void Awake()
  {
    if (!this.isChildOfAnotherUIItem)
      return;
    this.UpdateParent();
  }

  private void Start()
  {
    if ((UnityEngine.Object) tk2dUIManager.Instance == (UnityEngine.Object) null)
      Debug.LogError((object) "Unable to find tk2dUIManager. Please create a tk2dUIManager in the scene before proceeding.");
    if (!this.isChildOfAnotherUIItem || !((UnityEngine.Object) this.parentUIItem == (UnityEngine.Object) null))
      return;
    this.UpdateParent();
  }

  public bool IsPressed => this.isPressed;

  public tk2dUITouch Touch => this.touch;

  public tk2dUIItem ParentUIItem => this.parentUIItem;

  public void UpdateParent() => this.parentUIItem = this.GetParentUIItem();

  public void ManuallySetParent(tk2dUIItem newParentUIItem) => this.parentUIItem = newParentUIItem;

  public void RemoveParent() => this.parentUIItem = (tk2dUIItem) null;

  public bool Press(tk2dUITouch touch) => this.Press(touch, (tk2dUIItem) null);

  public bool Press(tk2dUITouch touch, tk2dUIItem sentFromChild)
  {
    if (this.isPressed)
      return false;
    if (!this.isPressed)
    {
      this.touch = touch;
      if ((this.registerPressFromChildren || (UnityEngine.Object) sentFromChild == (UnityEngine.Object) null) && this.enabled)
      {
        this.isPressed = true;
        if (this.OnDown != null)
          this.OnDown();
        if (this.OnDownUIItem != null)
          this.OnDownUIItem(this);
        this.DoSendMessage(this.SendMessageOnDownMethodName);
      }
      if ((UnityEngine.Object) this.parentUIItem != (UnityEngine.Object) null)
        this.parentUIItem.Press(touch, this);
    }
    return true;
  }

  public void UpdateTouch(tk2dUITouch touch)
  {
    this.touch = touch;
    if (!((UnityEngine.Object) this.parentUIItem != (UnityEngine.Object) null))
      return;
    this.parentUIItem.UpdateTouch(touch);
  }

  private void DoSendMessage(string methodName)
  {
    if (!((UnityEngine.Object) this.sendMessageTarget != (UnityEngine.Object) null) || methodName.Length <= 0)
      return;
    this.sendMessageTarget.SendMessage(methodName, (object) this, SendMessageOptions.RequireReceiver);
  }

  public void Release()
  {
    if (this.isPressed)
    {
      this.isPressed = false;
      if (this.OnUp != null)
        this.OnUp();
      if (this.OnUpUIItem != null)
        this.OnUpUIItem(this);
      this.DoSendMessage(this.SendMessageOnUpMethodName);
      if (this.OnClick != null)
        this.OnClick();
      if (this.OnClickUIItem != null)
        this.OnClickUIItem(this);
      this.DoSendMessage(this.SendMessageOnClickMethodName);
    }
    if (this.OnRelease != null)
      this.OnRelease();
    if (this.OnReleaseUIItem != null)
      this.OnReleaseUIItem(this);
    this.DoSendMessage(this.SendMessageOnReleaseMethodName);
    if (!((UnityEngine.Object) this.parentUIItem != (UnityEngine.Object) null))
      return;
    this.parentUIItem.Release();
  }

  public void CurrentOverUIItem(tk2dUIItem overUIItem)
  {
    if (!((UnityEngine.Object) overUIItem != (UnityEngine.Object) this))
      return;
    if (this.isPressed)
    {
      if (this.CheckIsUIItemChildOfMe(overUIItem))
        return;
      this.Exit();
      if (!((UnityEngine.Object) this.parentUIItem != (UnityEngine.Object) null))
        return;
      this.parentUIItem.CurrentOverUIItem(overUIItem);
    }
    else
    {
      if (!((UnityEngine.Object) this.parentUIItem != (UnityEngine.Object) null))
        return;
      this.parentUIItem.CurrentOverUIItem(overUIItem);
    }
  }

  public bool CheckIsUIItemChildOfMe(tk2dUIItem uiItem)
  {
    tk2dUIItem tk2dUiItem = (tk2dUIItem) null;
    bool flag = false;
    if ((UnityEngine.Object) uiItem != (UnityEngine.Object) null)
      tk2dUiItem = uiItem.parentUIItem;
    for (; (UnityEngine.Object) tk2dUiItem != (UnityEngine.Object) null; tk2dUiItem = tk2dUiItem.parentUIItem)
    {
      if ((UnityEngine.Object) tk2dUiItem == (UnityEngine.Object) this)
      {
        flag = true;
        break;
      }
    }
    return flag;
  }

  public void Exit()
  {
    if (!this.isPressed)
      return;
    this.isPressed = false;
    if (this.OnUp != null)
      this.OnUp();
    if (this.OnUpUIItem != null)
      this.OnUpUIItem(this);
    this.DoSendMessage(this.SendMessageOnUpMethodName);
  }

  public bool HoverOver(tk2dUIItem prevHover)
  {
    bool flag = false;
    tk2dUIItem tk2dUiItem = (tk2dUIItem) null;
    if (!this.isHoverOver)
    {
      if (this.OnHoverOver != null)
        this.OnHoverOver();
      if (this.OnHoverOverUIItem != null)
        this.OnHoverOverUIItem(this);
      this.isHoverOver = true;
    }
    if ((UnityEngine.Object) prevHover == (UnityEngine.Object) this)
      flag = true;
    if ((UnityEngine.Object) this.parentUIItem != (UnityEngine.Object) null && this.parentUIItem.isHoverEnabled)
      tk2dUiItem = this.parentUIItem;
    return !((UnityEngine.Object) tk2dUiItem == (UnityEngine.Object) null) && tk2dUiItem.HoverOver(prevHover) || flag;
  }

  public void HoverOut(tk2dUIItem currHoverButton)
  {
    if (this.isHoverOver)
    {
      if (this.OnHoverOut != null)
        this.OnHoverOut();
      if (this.OnHoverOutUIItem != null)
        this.OnHoverOutUIItem(this);
      this.isHoverOver = false;
    }
    if (!((UnityEngine.Object) this.parentUIItem != (UnityEngine.Object) null) || !this.parentUIItem.isHoverEnabled)
      return;
    if ((UnityEngine.Object) currHoverButton == (UnityEngine.Object) null)
    {
      this.parentUIItem.HoverOut(currHoverButton);
    }
    else
    {
      if (this.parentUIItem.CheckIsUIItemChildOfMe(currHoverButton) || !((UnityEngine.Object) currHoverButton != (UnityEngine.Object) this.parentUIItem))
        return;
      this.parentUIItem.HoverOut(currHoverButton);
    }
  }

  private tk2dUIItem GetParentUIItem()
  {
    for (Transform parent = this.transform.parent; (UnityEngine.Object) parent != (UnityEngine.Object) null; parent = parent.parent)
    {
      tk2dUIItem component = parent.GetComponent<tk2dUIItem>();
      if ((UnityEngine.Object) component != (UnityEngine.Object) null)
        return component;
    }
    return (tk2dUIItem) null;
  }

  public void SimulateClick()
  {
    if (this.OnDown != null)
      this.OnDown();
    if (this.OnDownUIItem != null)
      this.OnDownUIItem(this);
    this.DoSendMessage(this.SendMessageOnDownMethodName);
    if (this.OnUp != null)
      this.OnUp();
    if (this.OnUpUIItem != null)
      this.OnUpUIItem(this);
    this.DoSendMessage(this.SendMessageOnUpMethodName);
    if (this.OnClick != null)
      this.OnClick();
    if (this.OnClickUIItem != null)
      this.OnClickUIItem(this);
    this.DoSendMessage(this.SendMessageOnClickMethodName);
    if (this.OnRelease != null)
      this.OnRelease();
    if (this.OnReleaseUIItem != null)
      this.OnReleaseUIItem(this);
    this.DoSendMessage(this.SendMessageOnReleaseMethodName);
  }

  public void InternalSetIsChildOfAnotherUIItem(bool state) => this.isChildOfAnotherUIItem = state;

  public bool InternalGetIsChildOfAnotherUIItem() => this.isChildOfAnotherUIItem;
}
