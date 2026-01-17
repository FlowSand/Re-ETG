// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetMouseButtonDown
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Sends an Event when the specified Mouse Button is pressed. Optionally store the button state in a bool variable.")]
[ActionCategory(ActionCategory.Input)]
public class GetMouseButtonDown : FsmStateAction
{
  [RequiredField]
  [HutongGames.PlayMaker.Tooltip("The mouse button to test.")]
  public MouseButton button;
  [HutongGames.PlayMaker.Tooltip("Event to send if the mouse button is down.")]
  public FsmEvent sendEvent;
  [HutongGames.PlayMaker.Tooltip("Store the button state in a Bool Variable.")]
  [UIHint(UIHint.Variable)]
  public FsmBool storeResult;
  [HutongGames.PlayMaker.Tooltip("Uncheck to run when entering the state.")]
  public bool inUpdateOnly;

  public override void Reset()
  {
    this.button = MouseButton.Left;
    this.sendEvent = (FsmEvent) null;
    this.storeResult = (FsmBool) null;
    this.inUpdateOnly = true;
  }

  public override void OnEnter()
  {
    if (this.inUpdateOnly)
      return;
    this.DoGetMouseButtonDown();
  }

  public override void OnUpdate() => this.DoGetMouseButtonDown();

  private void DoGetMouseButtonDown()
  {
    bool mouseButtonDown = Input.GetMouseButtonDown((int) this.button);
    if (mouseButtonDown)
      this.Fsm.Event(this.sendEvent);
    this.storeResult.Value = mouseButtonDown;
  }
}
