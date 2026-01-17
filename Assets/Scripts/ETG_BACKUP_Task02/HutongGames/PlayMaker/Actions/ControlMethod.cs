// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.ControlMethod
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(".Brave")]
[Tooltip("Checks what controller is being used.")]
public class ControlMethod : FsmStateAction
{
  [Tooltip("Event to send if the keyboard and mouse are being used.")]
  public FsmEvent keyboardAndMouse;
  [Tooltip("Event to send when a controller is being used.")]
  public FsmEvent controller;

  public override void Reset()
  {
    this.keyboardAndMouse = (FsmEvent) null;
    this.controller = (FsmEvent) null;
  }

  public override void OnEnter()
  {
    if (BraveInput.GetInstanceForPlayer(0).IsKeyboardAndMouse())
      this.Fsm.Event(this.keyboardAndMouse);
    else
      this.Fsm.Event(this.controller);
    this.Finish();
  }
}
