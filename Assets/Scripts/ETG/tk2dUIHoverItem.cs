// Decompiled with JetBrains decompiler
// Type: tk2dUIHoverItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
[AddComponentMenu("2D Toolkit/UI/tk2dUIHoverItem")]
public class tk2dUIHoverItem : tk2dUIBaseItemControl
{
  public GameObject outStateGO;
  public GameObject overStateGO;
  private bool isOver;
  public string SendMessageOnToggleHoverMethodName = string.Empty;

  public event Action<tk2dUIHoverItem> OnToggleHover;

  public bool IsOver
  {
    get => this.isOver;
    set
    {
      if (this.isOver == value)
        return;
      this.isOver = value;
      this.SetState();
      if (this.OnToggleHover != null)
        this.OnToggleHover(this);
      this.DoSendMessage(this.SendMessageOnToggleHoverMethodName, (object) this);
    }
  }

  private void Start() => this.SetState();

  private void OnEnable()
  {
    if (!(bool) (UnityEngine.Object) this.uiItem)
      return;
    this.uiItem.OnHoverOver += new System.Action(this.HoverOver);
    this.uiItem.OnHoverOut += new System.Action(this.HoverOut);
  }

  private void OnDisable()
  {
    if (!(bool) (UnityEngine.Object) this.uiItem)
      return;
    this.uiItem.OnHoverOver -= new System.Action(this.HoverOver);
    this.uiItem.OnHoverOut -= new System.Action(this.HoverOut);
  }

  private void HoverOver() => this.IsOver = true;

  private void HoverOut() => this.IsOver = false;

  public void SetState()
  {
    tk2dUIBaseItemControl.ChangeGameObjectActiveStateWithNullCheck(this.overStateGO, this.isOver);
    tk2dUIBaseItemControl.ChangeGameObjectActiveStateWithNullCheck(this.outStateGO, !this.isOver);
  }
}
